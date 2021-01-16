using Microsoft.AspNetCore.Mvc;
using Neo4j.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text;
using Neo4jClient;
using Neo4jClient.Cypher;
using MedicallWaste.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MedicallWaste.Controllers
{
    [Produces("application/json")]
    [Route("[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IDriver driver;
        public readonly IGraphClient client;

        public UserController(IDriver _driver, IGraphClient graphClient)
        {
            driver = _driver;
            client = graphClient;
        }

        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        [HttpGet(nameof(GetAllUsers))]
        public async Task<IActionResult> GetAllUsers()
        {
            var query = new Neo4jClient.Cypher.CypherQuery("MATCH (users:ApplicationUser) RETURN users", new Dictionary<string, object>(), CypherResultMode.Set);
            IList<ApplicationUser> users = ((IRawGraphClient)client).ExecuteGetCypherResults<ApplicationUser>(query).ToList();

            return Ok(users);
        }

        [HttpPost(nameof(CreateUser))]
        public async Task<IActionResult> CreateUser([FromBody]ApplicationUser user)
        {
            var session = driver.AsyncSession();

            var statementText = new StringBuilder();
            statementText.Append("CREATE (a:ApplicationUser {username: $username, password: $password, firstname: $firstname, lastname: $lastname})");
            var statementParameters = new Dictionary<string, object>
            {
                {"username", user.username},
                {"password", user.password},
                {"firstname", user.firstname },
                {"lastname", user.lastname }
            };
            var result = await session.WriteTransactionAsync(tx => tx.RunAsync(statementText.ToString(), statementParameters));
            return StatusCode(201, "Node has been created in the database");
        }

        [HttpPost(nameof(Login))]
        public async Task<IActionResult> Login([FromBody]LoginDTO login)
        {
            var user = new Neo4jClient.Cypher.CypherQuery("MATCH (user:ApplicationUser) WHERE user.username = '" + login.username + "' and user.password = '" + login.password + "' RETURN user", new Dictionary<string, object>(), CypherResultMode.Set);
            ApplicationUser applicationUsers = ((IRawGraphClient)client).ExecuteGetCypherResults<ApplicationUser>(user).FirstOrDefault();
            if(applicationUsers==null)
            {
                return BadRequest("Wrong username or password");
            }
            else
            {
                return Ok(applicationUsers);
            }
        }

        [HttpPost(nameof(WorksAtTransport))]
        public async Task<IActionResult> WorksAtTransport(string username, Guid transportGuid)
        {
            var works = new Neo4jClient.Cypher.CypherQuery("MATCH(u: ApplicationUser), (t: TransportCompany) WHERE u.username = '" + username + "' AND t.guid = '" + transportGuid + "' CREATE (u)-[r: WORKS_AT]->(t) RETURN type(r)", new Dictionary<string, object>(), CypherResultMode.Set);
            ((IRawGraphClient)client).ExecuteCypher(works);

            return Ok(works);
        }

        [HttpPost(nameof(WorksAtLandfill))]
        public async Task<IActionResult> WorksAtLandfill(string username, Guid landfillGuid)
        {
            var works = new Neo4jClient.Cypher.CypherQuery("MATCH(u: ApplicationUser), (l: LandfillOrganization) WHERE u.username = '" + username + "' AND l.guid = '" + landfillGuid + "' CREATE (u)-[r: WORKS_AT]->(l) RETURN type(r)", new Dictionary<string, object>(), CypherResultMode.Set);
            ((IRawGraphClient)client).ExecuteCypher(works);

            return Ok(works);
        }

        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }

      

    }
}
