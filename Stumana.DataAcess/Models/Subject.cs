using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stumana.DataAcess.Models
{
    public class Subject
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string MajorId { get; set; }

        // Navigation Props
        public Major Major { get; set; }
    }
}
