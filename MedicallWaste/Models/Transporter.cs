using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MedicallWaste.Models
{
    public class Transporter
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Organization Organization { get; set; }
    }
}
