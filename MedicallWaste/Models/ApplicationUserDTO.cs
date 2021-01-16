using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MedicallWaste.Models
{
    public class ApplicationUserDTO
    {
        public string firstname { get; set; }
        public string lastname { get; set; }
        public string username { get; set; }
        public string orgname { get; set; }
        public string orglocation { get; set; }
    }
}
