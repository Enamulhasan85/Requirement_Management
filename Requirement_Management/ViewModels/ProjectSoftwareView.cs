using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Requirement_Management.Models;

namespace Requirement_Management.ViewModels
{
    public class ProjectSoftwareView
    {
        public int Id { get; set; }
        public int? SoftwareId { get; set; }
        public virtual Software Software { get; set; }
        public string SoftwareName { get; set; }
        public SoftwareStatus Status { get; set; }
    }
}