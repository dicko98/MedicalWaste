using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MedicallWaste.Models
{
    public class TransportCompany
    {
        public Guid guid { get; set; }
        public string name { get; set; }
        public string location { get; set; }
        public List<ApplicationUser> users { get; set; }
    }
}
