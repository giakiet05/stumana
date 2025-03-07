using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stumana.DataAcess.Models
{
    public class ClassroomOffering
    {
        public string Id { get; set; }
        public string ClassroomId { get; set; }
        public string SchoolYearId { get; set; }

        // Navigation Properties
        public Classroom Classroom { get; set; }
        public SchoolYear SchoolYear { get; set; }
    }
}
