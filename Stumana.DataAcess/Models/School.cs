using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stumana.DataAcess.Models
{
    public class School
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Rules { get; set; }
        public bool IsRegistered { get; set; }
        public string DistrictId { get; set; }

        // Navigation Properties
        public District District { get; set; }
    }
}
