using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Neo4j.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Neo4jClient;
using Neo4jClient.Cypher;
using MedicallWaste.Models;
using Neo4jMapper;

namespace MedicallWaste.Controllers
{
    [Produces("application/json")]
    [ApiController]
    [Route("[controller]")]
    public class PackageController : ControllerBase
    {
        private readonly IDriver driver;
        private readonly IGraphClient client;

        public PackageController(IDriver _driver, IGraphClient graphClient)
        {
            driver = _driver;
            client = graphClient;
        }
        [HttpGet(nameof(GetPackage))]
        public async Task<IActionResult> GetPackage(Guid barcode)
        {

            var query = new Neo4jClient.Cypher.CypherQuery("MATCH (package:Package) where package.barcode = '" + barcode + "' RETURN package", new Dictionary<string, object>(), CypherResultMode.Set);
            IList<Package> packages = ((IRawGraphClient)client).ExecuteGetCypherResults<Package>(query).ToList();

            return Ok(packages);
        }

        [HttpGet(nameof(GetMedicalTrack))]
        public async Task<IActionResult> GetMedicalTrack(string username)
        {
            if (username == "undefined")
                return Ok(null);
            var query = client.Cypher
                .Match("(user:ApplicationUser)-[w:WORKS_AT]->(med:MedicalOrganization) WHERE user.username='" + username + "'")
                .Return(() => new
                {
                    User = Return.As<ApplicationUser>("user"),
                    MedOrg = Return.As<MedicalOrganization>("med")
                });
            var res = query.Results.FirstOrDefault();

            var query2 = client.Cypher
                .Match("(user:ApplicationUser)-[w:WORKS_AT]->(med:MedicalOrganization) where med.guid='" + res.MedOrg.guid + "'")
                .Return(() => new
                {
                    User = Return.As<ApplicationUser>("user")
                });

            var users = query2.Results.ToList();
           
            IList<Package> packages = new List<Package>();
            foreach(var u in users)
            {
                var query3 = client.Cypher
               .Match("(u:ApplicationUser)-[m:MADE]->(p:Package) where u.username='"+u.User.username+"'")
               .Return(() => new
               {
                   Package = Return.As<Package>("p")
               });
                var pa = query3.Results.ToList();
                foreach (var p in pa)
                    packages.Add(p.Package);
            }

            foreach(var p in packages)
            {
                var query4 = client.Cypher
               .Match("(t:TransportCompany)<-[w:WORKS_AT]-(u:ApplicationUser)-[up:PICKED_UP]->(p:Package) where p.barcode='"+p.barcode+"'")
               .Return(() => new
               {
                   Transport = Return.As<TransportCompany>("t")
               });
                 
                if (query4.Results.ToList().Count > 0)
                    p.transportcompany = query4.Results.FirstOrDefault().Transport;

                var query5 = client.Cypher
               .Match("(l:LandfillOrganization)<-[w:WORKS_AT]-(u:ApplicationUser)-[s:STORED]->(p:Package) where p.barcode='"+p.barcode+"'")
               .Return(() => new
               {
                   Landfill = Return.As<LandfillOrganization>("l")
               });
                
                if(query5.Results.ToList().Count > 0)
                    p.landfillorganization = query5.Results.FirstOrDefault().Landfill;

            }

            return Ok(packages);
        }


        [HttpGet(nameof(GetAllPackages))]
        public async Task<IActionResult> GetAllPackages()
        {
            var query = new Neo4jClient.Cypher.CypherQuery("MATCH (package:Package) RETURN package", new Dictionary<string, object>(), CypherResultMode.Set);
            IList<Package> packages = ((IRawGraphClient)client).ExecuteGetCypherResults<Package>(query).ToList();

            return Ok(packages);
        }

        [HttpPost(nameof(CreatePackage))]
        public async Task<IActionResult> CreatePackage(string name, int weight, string username)
        {
            Guid barcode = Guid.NewGuid();
      
            var user = new Neo4jClient.Cypher.CypherQuery("MATCH (user:ApplicationUser) WHERE user.username = '" + username +"' RETURN user", new Dictionary<string, object>(), CypherResultMode.Set);
            IList<ApplicationUser> applicationUsers = ((IRawGraphClient)client).ExecuteGetCypherResults<ApplicationUser>(user).ToList();

            DateTime createDate = DateTime.UtcNow;
            try
            {
                if (applicationUsers.Count == 0)
                {
                    var result = new Neo4jClient.Cypher.CypherQuery("CREATE (package:Package {barcode: '" + barcode + "', name: '" + name + "', weight: '" + weight + "', datecreated: '" + createDate + "'}), (user:ApplicationUser {username: '" + username + "'})", new Dictionary<string, object>(), CypherResultMode.Set);
                    ((IRawGraphClient)client).ExecuteCypher(result);

                    var relation = new Neo4jClient.Cypher.CypherQuery("MATCH (package:Package), (user:ApplicationUser) WHERE package.barcode = '" + barcode + "' AND user.username = '" + username + "' CREATE (user) -[r:MADE {date: package.datecreated}]-> (package) RETURN TYPE(r), r.date", new Dictionary<string, object>(), CypherResultMode.Set);
                    ((IRawGraphClient)client).ExecuteCypher(relation);
                    return Ok(result);
                }
                else if (applicationUsers.Count != 0)
                {
                    var result = new Neo4jClient.Cypher.CypherQuery("CREATE(package:Package {barcode: '" + barcode + "', name: '" + name + "', weight: '" + weight + "', datecreated: '" + createDate + "'})", new Dictionary<string, object>(), CypherResultMode.Set);
                    ((IRawGraphClient)client).ExecuteCypher(result);

                    var relation = new Neo4jClient.Cypher.CypherQuery("MATCH (package:Package), (user:ApplicationUser) WHERE package.barcode = '" + barcode + "' AND user.username = '" + username + "' CREATE (user) -[r:MADE {date: package.datecreated}]-> (package) RETURN TYPE(r), r.date", new Dictionary<string, object>(), CypherResultMode.Set);
                    ((IRawGraphClient)client).ExecuteCypher(relation);

                    return Ok(result);
                }
                else
                    return BadRequest();
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPatch(nameof(ChangeWeight))]
        public void ChangeWeight(Package package)
        {
            var session = driver.AsyncSession();
            session.RunAsync("MATCH (package:Package) WHERE package.name = '"+package.name+"' SET package.weight = "+package.weight+" ");
        }

        [HttpDelete(nameof(DeleteConnectedPackage))]
        public void DeleteConnectedPackage(Package package)
        {
            var session = driver.AsyncSession();
            session.RunAsync("OPTIONAL MATCH ()-[r]->(package:Package) WHERE package.barcode = '" + package.barcode + "' DELETE r, package");
            session.RunAsync("MATCH (package:Package) WHERE package.barcode = '" + package.barcode + "' DELETE package");
        }
    }
}
