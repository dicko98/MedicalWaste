﻿using MedicallWaste.Models;
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
    public class TransportCompanyController : ControllerBase
    {
        private readonly IDriver driver;
        private readonly IGraphClient client;

        public TransportCompanyController(IDriver _driver, IGraphClient graphClient)
        {
            driver = _driver;
            client = graphClient;
        }

        [HttpGet(nameof(GetAllTransportCompanies))]
        public async Task<IActionResult> GetAllTransportCompanies()
        {

            var query = new Neo4jClient.Cypher.CypherQuery("MATCH(t: TransportCompany) RETURN t", new Dictionary<string, object>(), CypherResultMode.Set);
            IList<TransportCompany> organizations = ((IRawGraphClient)client).ExecuteGetCypherResults<TransportCompany>(query).ToList();

            return Ok(organizations);
        }

        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        [HttpPost(nameof(CreateTransportCompany))]
        public async Task<IActionResult> CreateTransportCompany(string name, string location)
        {
            Guid guid = Guid.NewGuid();

            var transport = new Neo4jClient.Cypher.CypherQuery("CREATE (t:TransportCompany {guid: '" + guid + "', name: '" + name + "', location: '" + location + "'})", new Dictionary<string, object>(), CypherResultMode.Set);
            ((IRawGraphClient)client).ExecuteCypher(transport);

            return Ok(transport);
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
