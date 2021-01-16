using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MedicallWaste.Models
{
    public class Package
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Weight { get; set; }
        public Organization Organization { get; set; }

    }
}
