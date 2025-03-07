using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stumana.DataAcess.Models
{
    public class ScoreType
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public double Coefficient { get; set; }
        public string SchoolId { get; set; }

        //Navigation Properties
        public School School { get; set; }
    }
}
