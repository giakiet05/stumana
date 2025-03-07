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
        public string SubjectOfferingId { get; set; }

        // Navigation Properties
        public ScoreType ScoreType { get; set; }
        public SubjectOffering SubjectOffering { get; set; }
    }
}
