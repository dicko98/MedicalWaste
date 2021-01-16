using MedicallWaste.Models;
using Microsoft.AspNetCore.Mvc;
using Neo4j.Driver;
using Neo4jClient;
using Neo4jClient.Cypher;
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
    public class MedicalOrganizationController : ControllerBase
    {
        private readonly IDriver driver;
        private readonly IGraphClient client;

        public MedicalOrganizationController(IDriver _driver, IGraphClient graphClient)
        {
            driver = _driver;
            client = graphClient;
        }

        [HttpGet(nameof(GetAllMedicalOrganizations))]
        public async Task<IActionResult> GetAllMedicalOrganizations()
        {

            var query = new Neo4jClient.Cypher.CypherQuery("MATCH(o: MedicalOrganization) RETURN o", new Dictionary<string, object>(), CypherResultMode.Set);
            IList<MedicalOrganization> organizations = ((IRawGraphClient)client).ExecuteGetCypherResults<MedicalOrganization>(query).ToList();

            return Ok(organizations);
        }

        [HttpGet(nameof(GetMedicalCompanyByLocation))]
        public async Task<IActionResult> GetMedicalCompanyByLocation(string location)
        {
            var query = new Neo4jClient.Cypher.CypherQuery("MATCH (o: MedicalOrganization) WHERE o.location = '" + location + "' RETURN o", new Dictionary<string, object>(), CypherResultMode.Set);
            IList<MedicalOrganization> organizations = ((IRawGraphClient)client).ExecuteGetCypherResults<MedicalOrganization>(query).ToList();

            return Ok(organizations);
        }

        [HttpPost(nameof(CreateMedicalOrganization))]
        public async Task<IActionResult> CreateMedicalOrganization(string name, string location)
        {
            Guid guid = Guid.NewGuid();

            var medical = new Neo4jClient.Cypher.CypherQuery("CREATE (org:MedicalOrganization {guid: '" + guid + "', name: '" + name + "', location: '" + location + "'})", new Dictionary<string, object>(), CypherResultMode.Set);
            ((IRawGraphClient)client).ExecuteCypher(medical);

            return Ok(medical);
        }

        [HttpPost(nameof(ConnectUser))]
        public void ConnectUser(string username, string organization)
        {
            var session = driver.AsyncSession();
            session.RunAsync("MATCH (o:MedicalOrganization),(u:ApplicationUser) WHERE o.name = '" + organization + "' AND u.username = '" + username + "' CREATE(o) -[r: HAS_EMPLOYEE]->(u) RETURN type(r)");
        }

        [HttpDelete(nameof(DeleteMedicalOrganization))]
        public void DeleteMedicalOrganization(MedicalOrganization organization)
        {
            var session = driver.AsyncSession();
            session.RunAsync("MATCH (org:MedicalOrganization) WHERE org.guid = '" + organization.guid + "' DELETE org");
        }
    }
}
