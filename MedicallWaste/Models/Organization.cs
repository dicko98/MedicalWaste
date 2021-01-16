using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MedicallWaste.Models
{
    public class Organization
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
        public List<ApplicationUser> Users { get; set; }
    }
}
