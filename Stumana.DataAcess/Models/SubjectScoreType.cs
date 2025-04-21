using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stumana.DataAcess.Models
{
    public class SubjectScoreType
    {
        public string Id { get; set; }
        public string ScoreTypeId { get; set; }
        public string SubjectId { get; set; }
        public int Amount { get; set; } // Số lượng điểm loại này của môn

        // Navigation Properties
        public ScoreType ScoreType { get; set; }
        public Subject Subject { get; set; }
    }
}
