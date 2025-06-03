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
        public string Conduct { get; set; } // Hạnh kiểm
        public int Absence { get; set; } //Số ngày nghỉ
        public string Comment { get; set; } //Nhận xét
        public string StudentId { get; set; }
        public string ClassroomId { get; set; }

        // Navigation Properties
        public Student Student { get; set; }
        public Classroom Classroom { get; set; }
    }
}
