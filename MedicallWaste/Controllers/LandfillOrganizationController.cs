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

        [HttpGet(nameof(GetLandfillOrganization))]
        public async Task<IActionResult> GetLandfillOrganization(string location)
        {
            //string fullLocation = ".*" + location + ".*";

            //Dictionary<string, object> queryDict = new Dictionary<string, object>();
            //queryDict.Add("fullLocation", fullLocation);

            var query = new Neo4jClient.Cypher.CypherQuery("MATCH (l:LandfillOrganization) WHERE l.location =~ '.*" + location + ".*' RETURN l", new Dictionary<string, object>(), CypherResultMode.Set);
            IList<LandfillOrganization> organizations = ((IRawGraphClient)client).ExecuteGetCypherResults<LandfillOrganization>(query).ToList();

            return Ok(organizations);
        }

        [HttpPost(nameof(CreateLandfillOrganization))]
        public async Task<IActionResult> CreateLandfillOrganization(string name, string location)
        {
            Guid guid = Guid.NewGuid();

            var landfill = new Neo4jClient.Cypher.CypherQuery("CREATE (org:LandfillOrganization {guid: '" + guid + "', name: '" + name + "', location: '" + location + "'})", new Dictionary<string, object>(), CypherResultMode.Set);
            ((IRawGraphClient)client).ExecuteCypher(landfill);

            return Ok(landfill);
        }

        [HttpPatch(nameof(ChangeName))]
        public void ChangeName(LandfillOrganization organization)
        {
            var session = driver.AsyncSession();
            session.RunAsync("MATCH (org:LandfillOrganization) WHERE org.guid = '" + organization.guid + "' SET org.name = " + organization.name + " ");
        }

        [HttpDelete(nameof(DeleteConnectedLandfillOrganization))]
        public void DeleteConnectedLandfillOrganization(string organizationGuid)
        {
            var session = driver.AsyncSession();
            session.RunAsync("OPTIONAL MATCH ()-[p]->(l:LandfillOrganization) WHERE l.guid = '" + organizationGuid + "' DELETE p, l");

            session.RunAsync("MATCH (l:LandfillOrganization) WHERE l.guid = '" + organizationGuid + "' DELETE l");
        }
    }
}
