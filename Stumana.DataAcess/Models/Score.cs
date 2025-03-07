using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stumana.DataAcess.Models
{
    public class Score
    {
        public string Id { get; set; }
        public double Value { get; set; }
        public int RecordNo { get; set; } = 1;
        public string StudentAssignmentId { get; set; }
        public string SubjectScoreTypeId { get; set; }

        // Navigation Properties
        public StudentAssignment StudentAssignment { get; set; }
        public SubjectScoreType SubjectScoreType { get; set; }
    }
}
