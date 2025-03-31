using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stumana.DataAcess.Models
{
    public class PerformanceType
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public double MinScore { get; set; }
        public double MaxScore { get; set; }
        public string YearId { get; set; }
        public SchoolYear Year { get; set; }
    }
}
