using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stumana.DataAcess.Models
{
    public class Asset
    {
        public string Id { get; set; }
        public int MinAge { get; set; }
        public int MaxAge { get; set; }
        public double ScoreToPass {  get; set; }
        public string YearId { get; set; }

        public SchoolYear Year { get; set; }
    }
}
