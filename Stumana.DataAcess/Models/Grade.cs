using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stumana.DataAcess.Models
{
    public class Grade
    {
        public string Id { get; set; }
        public string Name { get; set; } // Tên khối
        public int Level { get; set; } // Thứ tự của khối
    
    }
}
