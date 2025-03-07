using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stumana.DataAcess.Models
{
    public class Province
    {
        public string Id { get; set; }
        public string Name { get; set; }

        // Navigation Properties
        public ICollection<District> Districts { get; set; }
    }

}
