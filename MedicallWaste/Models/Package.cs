using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MedicallWaste.Models
{
    public class Package
    {
        public Guid barcode { get; set; }
        public string name { get; set; }
        public int weight { get; set; }
        public int pickedweight { get; set; }
        public int storedweight { get; set; }
        public DateTime datecreated { get; set; }
        public MedicalOrganization medorganization { get; set; }
        public LandfillOrganization landfillorganization { get; set; }
        public TransportCompany transportcompany { get; set; }
        public ApplicationUser medicaluser { get; set; }
        public ApplicationUser transportuser { get; set; }
        public ApplicationUser deponyuser { get; set; }
    }
}
