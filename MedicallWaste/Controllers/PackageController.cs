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
        public async Task<List<IRecord>> GetPackage(string name)
        {
            var session = driver.AsyncSession();
            var cursor = await session.RunAsync("MATCH (package:Package) WHERE package.name = '" + name + "' RETURN package");
            var packages = await cursor.ToListAsync();

            return packages;
        }
        [HttpGet(nameof(GetAllPackages))]
        public async Task<List<IRecord>> GetAllPackages()
        {   
            var session = driver.AsyncSession();
            var cursor = await session.RunAsync("MATCH (package:Package) RETURN package");
            //var cursor = await session.RunAsync("MATCH (n1)-[r]->(n2) RETURN r, n1, n2 LIMIT 25");
            var packages = await cursor.ToListAsync();

            return packages;
        }

        [HttpPost(nameof(CreatePackage))]
        public async Task<IActionResult> CreatePackage(string name, int weight)
        {
            var statementText = new StringBuilder();
            statementText.Append("CREATE (package:Package {name: $name, weight: $weight})");
            var statementParameters = new Dictionary<string, object>
        {
            {"name", name },
            {"weight", weight}
        };

            var session = driver.AsyncSession();
            var result = await session.WriteTransactionAsync(tx => tx.RunAsync(statementText.ToString(), statementParameters));
            return StatusCode(201, "Package has been created in the database");
        }


        [HttpPatch(nameof(ChangeWeight))]
        public void ChangeWeight(Package package)
        {
            var session = driver.AsyncSession();
            session.RunAsync("MATCH (package:Package) WHERE package.name = '"+package.Name+"' SET package.weight = "+package.Weight+" ");
        }


        [HttpDelete(nameof(DeletePackage))]
        public void DeletePackage(Package package)
        {
            var session = driver.AsyncSession();
            session.RunAsync("MATCH (package:Package) WHERE package.name = '" + package.Name + "' DELETE package");       
        }
    }
}
