using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stumana.DataAcess.Models
{
    public class District
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string ProvinceId { get; set; }

        // Navigation Properties
        public Province Province { get; set; }
        public ICollection<School> Schools { get; set; }
    }
}
