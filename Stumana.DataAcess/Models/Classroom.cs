﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stumana.DataAcess.Models
{
    public class Classroom
    {
        public string Id { get; set; }
        public string? Name { get; set; }
        public string GradeId { get; set; }
        public string YearId { get; set; }

        // Navigation Props
        public Grade Grade { get; set; }
        public SchoolYear Year { get; set; }
    }
}
