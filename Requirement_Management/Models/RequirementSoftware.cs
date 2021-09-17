using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Requirement_Management.Models
{
    public class RequirementSoftware
    {
        public int Id { get; set; }
        public int? SoftwareId { get; set; }
        public virtual Software Software { get; set; }
        public int? RequirementDetailId { get; set; }
        public virtual RequirementDetail RequirementDetail { get; set; }
    }
}