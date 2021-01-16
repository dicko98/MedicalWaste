﻿using Microsoft.AspNetCore.Http;
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
        public async Task<IActionResult> GetPackage(string name)
        {

            var q1 = new Neo4jClient.Cypher.CypherQuery("MATCH (package:Package) where package.name = '" + name + "' RETURN package", new Dictionary<string, object>(), CypherResultMode.Set);
            IList<Package> records = ((IRawGraphClient)client).ExecuteGetCypherResults<Package>(q1).ToList();

            return Ok(records);
        }

        [HttpGet(nameof(GetAllPackages))]
        public async Task<IActionResult> GetAllPackages()
        {
            var q1 = new Neo4jClient.Cypher.CypherQuery("MATCH (package:Package) RETURN " +
                "package", new Dictionary<string, object>(), CypherResultMode.Set);
            IList<Package> records = ((IRawGraphClient)client).ExecuteGetCypherResults<Package>(q1).ToList();

            return Ok(records);
        }

        [HttpPost(nameof(CreatePackage))]
        public async Task<IActionResult> CreatePackage(string name, int weight, string username)
        {
            Guid barcode = Guid.NewGuid();
      
            var user = new Neo4jClient.Cypher.CypherQuery("MATCH (user:ApplicationUser) WHERE user.username = '" + username +"' RETURN user", new Dictionary<string, object>(), CypherResultMode.Set);
            IList<ApplicationUser> applicationUsers = ((IRawGraphClient)client).ExecuteGetCypherResults<ApplicationUser>(user).ToList();

            try
            {
                if (applicationUsers.Count == 0)
                {
                    var result = new Neo4jClient.Cypher.CypherQuery("CREATE (package:Package {barcode: '" + barcode + "', name: '" + name + "', weight: '" + weight + "'}), (user:ApplicationUser {username: '" + username + "'})", new Dictionary<string, object>(), CypherResultMode.Set);
                    ((IRawGraphClient)client).ExecuteCypher(result);

                    var relation = new Neo4jClient.Cypher.CypherQuery("MATCH (package:Package), (user:ApplicationUser) WHERE package.barcode = '" + barcode + "' AND user.username = '" + username + "' CREATE (package) -[r:MADE_BY]-> (user) RETURN TYPE(r)", new Dictionary<string, object>(), CypherResultMode.Set);
                    ((IRawGraphClient)client).ExecuteCypher(relation);
                    return Ok(result);
                }
                else if (applicationUsers.Count != 0)
                {
                    var result = new Neo4jClient.Cypher.CypherQuery("CREATE(package: Package { barcode: '" + barcode + "', name: '" + name + "', weight: '" + weight + "'}", new Dictionary<string, object>(), CypherResultMode.Set);
                    ((IRawGraphClient)client).ExecuteCypher(result);

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


        [HttpDelete(nameof(DeletePackage))]
        public void DeletePackage(Package package)
        {
            var session = driver.AsyncSession();
            session.RunAsync("MATCH (package:Package) WHERE package.barcode = '" + package.barcode + "' DELETE package");
        }

        [HttpDelete(nameof(DeleteConnectedPackage))]
        public void DeleteConnectedPackage(Package package)
        {
            session.RunAsync("OPTIONAL MATCH (package:Package)-[r]->() WHERE package.barcode = '" + package.barcode + "' DELETE r, package");
        }

    }
}
