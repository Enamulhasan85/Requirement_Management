using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Requirement_Management.Models
{
    public class JobHolder
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Contact { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string Designation { get; set; }
        public DateTime? JoiningDate { get; set; }
        public decimal HourlyCost { get; set; }
        public bool isActive { get; set; }
    }
}