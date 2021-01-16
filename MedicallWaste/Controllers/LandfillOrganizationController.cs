using MedicallWaste.Models;
using Microsoft.AspNetCore.Mvc;
using Neo4j.Driver;
using Neo4jClient;
using Neo4jClient.Cypher;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MedicallWaste.Controllers
{
    [Produces("application/json")]
    [Route("[controller]")]
    [ApiController]
    public class LandfillOrganizationController : ControllerBase
    {
        private readonly IDriver driver;
        private readonly IGraphClient client;

        public LandfillOrganizationController(IDriver _driver, IGraphClient graphClient)
        {
            driver = _driver;
            client = graphClient;
        }

        [HttpGet(nameof(GetAllLandfillOrganizations))]
        public async Task<IActionResult> GetAllLandfillOrganizations()
        {

            var query = new Neo4jClient.Cypher.CypherQuery("MATCH(l: LandfillOrganization) RETURN l", new Dictionary<string, object>(), CypherResultMode.Set);
            IList<LandfillOrganization> organizations = ((IRawGraphClient)client).ExecuteGetCypherResults<LandfillOrganization>(query).ToList();

            return Ok(organizations);
        }

        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        [HttpPost(nameof(CreateLandfillOrganization))]
        public async Task<IActionResult> CreateLandfillOrganization(string name, string location)
        {
            Guid guid = Guid.NewGuid();

            var landfill = new Neo4jClient.Cypher.CypherQuery("CREATE (org:LandfillOrganization {guid: '" + guid + "', name: '" + name + "', location: '" + location + "'})", new Dictionary<string, object>(), CypherResultMode.Set);
            ((IRawGraphClient)client).ExecuteCypher(landfill);

            return Ok(landfill);
        }

        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
