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

        [HttpGet(nameof(GetTransportCompany))]
        public async Task<IActionResult> GetTransportCompany(string location)
        {

            var query = new Neo4jClient.Cypher.CypherQuery("MATCH (t:TransportCompany) where t.location = '" + location + "' RETURN t", new Dictionary<string, object>(), CypherResultMode.Set);
            IList<TransportCompany> organizations = ((IRawGraphClient)client).ExecuteGetCypherResults<TransportCompany>(query).ToList();

            return Ok(organizations);
        }

        [HttpPost(nameof(CreateTransportCompany))]
        public async Task<IActionResult> CreateTransportCompany(string name, string location)
        {
            Guid guid = Guid.NewGuid();

            var transport = new Neo4jClient.Cypher.CypherQuery("CREATE (t:TransportCompany {guid: '" + guid + "', name: '" + name + "', location: '" + location + "'})", new Dictionary<string, object>(), CypherResultMode.Set);
            ((IRawGraphClient)client).ExecuteCypher(transport);

            return Ok(transport);
        }

        [HttpPost(nameof(StorePackage))]
        public async Task<IActionResult> StorePackage(Guid transportGuid, Guid landfillGuid)
        {
            var store = new Neo4jClient.Cypher.CypherQuery("MATCH(t: TransportCompany),(l: LandfillOrganization) WHERE t.guid = '" + transportGuid + "' AND l.guid = '" + landfillGuid + "' CREATE (t)-[r: STORED_TO]->(l) RETURN type(r)", new Dictionary<string, object>(), CypherResultMode.Set);
            ((IRawGraphClient)client).ExecuteCypher(store);

            return Ok(store);
        }

        [HttpDelete(nameof(DeleteTransportCompany))]
        public void DeleteTransportCompany(TransportCompany transport)
        {
            var session = driver.AsyncSession();
            session.RunAsync("MATCH (t:TransportCompany) WHERE t.guid = '" + transport.guid + "' DELETE t");
        }

        [HttpDelete(nameof(DeleteConnectedTransportCompanies))]
        public void DeleteConnectedTransportCompanies(TransportCompany transport)
        {
            var session = driver.AsyncSession();
            session.RunAsync("OPTIONAL MATCH ()-[p]->(t:TransportCompany)->[s]-() WHERE t.guid = '" + transport.guid + "' DELETE p, s, t");
        }
    }
}
