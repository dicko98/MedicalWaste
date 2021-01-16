using AspNetCore.Identity.Neo4j;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MedicallWaste.Models
{
    public class ApplicationUser : Neo4jIdentityUser
    {
        public Organization Organization { get; set; }
        public List<Package> Packages { get; set; }
        public Transporter Transporter { get; set; }
    }
}
