using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Requirement_Management.Models
{
    public class Software
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int? Software_CategoryId { get; set; }
        public virtual SoftwareCategory Software_Category { get; set; }
    }
}