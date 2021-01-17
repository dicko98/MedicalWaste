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
        public async Task<IActionResult> Get(string username)
        {
            var query = new Neo4jClient.Cypher.CypherQuery("MATCH (user:ApplicationUser) where user.username = '" + username + "' RETURN user", new Dictionary<string, object>(), CypherResultMode.Set);
            ApplicationUser user = ((IRawGraphClient)client).ExecuteGetCypherResults<ApplicationUser>(query).FirstOrDefault();

            return Ok(user);
        }

        [HttpGet(nameof(GetAllUsers))]
        public async Task<IActionResult> GetAllUsers()
        {
            var query = new Neo4jClient.Cypher.CypherQuery("MATCH (users:ApplicationUser) RETURN users", new Dictionary<string, object>(), CypherResultMode.Set);
            IList<ApplicationUser> users = ((IRawGraphClient)client).ExecuteGetCypherResults<ApplicationUser>(query).ToList();

            return Ok(users);
        }

        [HttpGet(nameof(GetMedicalUser))]
        public async Task<IActionResult> GetMedicalUser(string username)
        { 
            var query = client.Cypher
                .Match("(user:ApplicationUser)-[r:WORKS_AT]->(med:MedicalOrganization)")
                .Return(() => new
                {
                    User = Return.As<ApplicationUser>("user"),
                    MedOrg = Return.As<MedicalOrganization>("med")
                });
            var res = query.Results.FirstOrDefault();

            ApplicationUserDTO applicationUserDTO = new ApplicationUserDTO
            {
                firstname = res.User.firstname,
                lastname = res.User.lastname,
                username = res.User.username,
                orgname = res.MedOrg.name,
                orglocation = res.MedOrg.location
            };
            return Ok(applicationUserDTO);
        }

        [HttpGet(nameof(GetTransportUser))]
        public async Task<IActionResult> GetTransportUser(string username)
        {
            var query = client.Cypher
                .Match("(user:ApplicationUser)-[r:WORKS_AT]->(transport:TransportCompany)")
                .Return(() => new
                {
                    User = Return.As<ApplicationUser>("user"),
                    Transport = Return.As<TransportCompany>("transport")
                });
            var res = query.Results.FirstOrDefault();

            ApplicationUserDTO applicationUserDTO = new ApplicationUserDTO
            {
                firstname = res.User.firstname,
                lastname = res.User.lastname,
                username = res.User.username,
                orgname = res.Transport.name,
                orglocation = res.Transport.location
            };
            return Ok(applicationUserDTO);
        }

        [HttpGet(nameof(GetLandfillUser))]
        public async Task<IActionResult> GetLandfillUser(string username)
        {
            var query = client.Cypher
                .Match("(user:ApplicationUser)-[r:WORKS_AT]->(landfill:LandfillOrganization)")
                .Return(() => new
                {
                    User = Return.As<ApplicationUser>("user"),
                    Landfill = Return.As<TransportCompany>("landfill")
                });
            var res = query.Results.FirstOrDefault();

            ApplicationUserDTO applicationUserDTO = new ApplicationUserDTO
            {
                firstname = res.User.firstname,
                lastname = res.User.lastname,
                username = res.User.username,
                orgname = res.Landfill.name,
                orglocation = res.Landfill.location
            };
            return Ok(applicationUserDTO);
        }

        [HttpPost(nameof(CreateUser))]
        public async Task<IActionResult> CreateUser([FromBody] ApplicationUser user)
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
            ApplicationUser applicationUser = ((IRawGraphClient)client).ExecuteGetCypherResults<ApplicationUser>(user).FirstOrDefault();
            if(applicationUser==null)
            {
                return BadRequest("Wrong username or password");
            }
            else
            {
                return Ok(applicationUser);
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

        [HttpPost(nameof(WorksAtMedical))]
        public void WorksAtMedical(string username, Guid organizationGuid)
        {
            var session = driver.AsyncSession();
            session.RunAsync("MATCH (o:MedicalOrganization),(u:ApplicationUser) WHERE o.guid = '" + organizationGuid + "' AND u.username = '" + username + "' CREATE(u) -[r: WORKS_AT]->(o) RETURN type(r)");
        }

        [HttpPut(nameof(PickUpPackage))]
        public async Task<IActionResult> PickUpPackage(string username, Guid barcode, int weight)
        {
            var pickup = new Neo4jClient.Cypher.CypherQuery("MATCH (u:ApplicationUser), (p:Package) WHERE u.username = '" + username + "' AND p.barcode = '" + barcode + "' SET p.pickedweight = '" + weight + "' CREATE (u)-[r: PICKED_UP]->(p) RETURN type(r) ", new Dictionary<string, object>(), CypherResultMode.Set);
            ((IRawGraphClient)client).ExecuteCypher(pickup);

            return Ok(pickup);
        }

        [HttpPut(nameof(StorePackage))]
        public async Task<IActionResult> StorePackage(string username, Guid barcode, int weight)
        {
            var store = new Neo4jClient.Cypher.CypherQuery("MATCH (u:ApplicationUser), (p:Package) WHERE u.username = '" + username + "' AND p.barcode = '" + barcode + "' SET p.storedweight = '" + weight + "' CREATE (u)-[r: STORED]->(p) RETURN type(r) ", new Dictionary<string, object>(), CypherResultMode.Set);
            ((IRawGraphClient)client).ExecuteCypher(store);

            return Ok(store);
        }
    }
}
