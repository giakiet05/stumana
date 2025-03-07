using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stumana.DataAcess.Models
{
    public class SubjectOffering
    {
        public string Id { get; set; }
        public string SubjectId { get; set; }
        public string SchoolYearId { get; set; }

        // Navigation Properties
        public Subject Subject { get; set; }
        public SchoolYear SchoolYear { get; set; }
    }
}
