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
        public double ScoreToPass { get; set; } //Điểm qua môn
        public string YearId { get; set; }
        public string GradeId { get; set; }

        // Navigation Props
        public SchoolYear Year { get; set; }
        public Grade Grade { get; set; }
    }
}
