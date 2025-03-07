using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stumana.DataAcess.Models
{
    [Table("Teachers")]
    public class Teacher : User
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MajorId { get; set; }

        // Navigation Props
        public Major Major { get; set; }
    }
}
