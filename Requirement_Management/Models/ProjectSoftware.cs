using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Requirement_Management.Models
{
    public enum SoftwareStatus
    {
        Active,
        InActive
    };

    public class ProjectSoftware
    {
        public int Id { get; set; }
        public int? ProjectId { get; set; }
        public virtual Project Project { get; set; }
        public int? SoftwareId { get; set; }
        public virtual Software Software { get; set; }
        public SoftwareStatus Status { get; set; }
    }
}