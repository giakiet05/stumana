using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stumana.DataAcess.Models
{
    public class StudentAssignment
    {
        public string Id { get; set; }
        public int Semester { get; set; }
        public string Conduct { get; set; }
        public int ExcusedAbsence { get; set; }
        public int UnexcusedAbsence { get; set; }
        public string StudentId { get; set; }
        public string ClassroomOfferingId { get; set; }

        // Navigation Properties
        public Student Student { get; set; }
        public ClassroomOffering ClassroomOffering { get; set; }
    }
}
