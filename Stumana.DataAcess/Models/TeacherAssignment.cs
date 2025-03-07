using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stumana.DataAcess.Models
{
    public class TeacherAssignment
    {
        public string Id { get; set; }
        public string Weekday { get; set; }
        public int Period { get; set; }
        public string TeacherId { get; set; }
        public string SubjectOfferingId { get; set; }
        public string ClassroomOfferingId { get; set; }

        // Navigation Properties
        public Teacher Teacher { get; set; }
        public SubjectOffering SubjectOffering { get; set; }
        public ClassroomOffering ClassroomOffering { get; set; }
    }

}
