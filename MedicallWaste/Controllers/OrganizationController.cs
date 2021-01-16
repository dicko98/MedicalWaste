using MedicallWaste.Models;
using Microsoft.AspNetCore.Mvc;
using Neo4j.Driver;
using Neo4jClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MedicallWaste.Controllers
{
    [Produces("application/json")]
    [Route("[controller]")]
    [ApiController]
    public class OrganizationController : ControllerBase
    {
        private readonly IDriver driver;
        private readonly IGraphClient client;

        public OrganizationController(IDriver _driver, IGraphClient graphClient)
        {
            driver = _driver;
            client = graphClient;
        }

        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        [HttpPost(nameof(CreateOrganization))]
        public async Task<IActionResult> CreateOrganization(string name, string location)
        {
            var statementText = new StringBuilder();
            statementText.Append("CREATE (org:Organization {name: $name, location: $location})");
            var statementParameters = new Dictionary<string, object>
        {
            {"name", name },
            {"location", location}
        };

            var session = driver.AsyncSession();
            var result = await session.WriteTransactionAsync(tx => tx.RunAsync(statementText.ToString(), statementParameters));
            return StatusCode(201, "Organization has been created in the database");
        }

        [HttpPost(nameof(ConnectUser))]
        public void ConnectUser(string username, string organization)
        {
            var session = driver.AsyncSession();
            session.RunAsync("MATCH (o:Organization),(u:ApplicationUser) WHERE o.name = '" + organization + "' AND u.username = '" + username + "' CREATE(o) -[r: HAS_EMPLOYEE]->(u) RETURN type(r)");
           
            //return StatusCode(201, "User has been added to Organization");
        }

        [HttpDelete(nameof(DeleteOrganization))]
        public void DeleteOrganization(Organization organization)
        {

        }
    }
}
