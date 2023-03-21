using Requirement_Management.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Requirement_Management.ViewModels
{
    public class ProjectView
    {
        public ProjectView()
        {
            ProSoftware = new List<ProjectSoftwareView>();
        }

        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime StartDate { get; set; }
        public int? CompanyId { get; set; }
        public virtual ClientCompany Company { get; set; }
        
        public int? CategoryId { get; set; }
        public virtual SoftwareCategory Category { get; set; }
        public int? SoftwareId { get; set; }
        public virtual Software Software { get; set; }
        public SoftwareStatus Status { get; set; }
        public List<ProjectSoftwareView> ProSoftware { get; set; }

        public RequirementMode ProjectMode { get; set; }
        public decimal TargetCost { get; set; }
        public DateTime ScheduleStartDate { get; set; }
        public DateTime? TargetDate { get; set; }
        public DateTime? EndDate { get; set; }
        public ScheduleStatus ScheduleStatus { get; set; }

        public List<Project> Project { get; set; }
    }
}