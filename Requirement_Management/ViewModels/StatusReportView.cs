using Requirement_Management.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Requirement_Management.ViewModels
{
    public class StatusReportView
    {
        public StatusReportView()
        {
            ProjectDetail = new List<ProjectReportView>();
        }

        public int Id { get; set; }
        public DateTime? From { get; set; }
        public string Fromdate { get; set; }
        public DateTime? To { get; set; }
        public string Todate { get; set; }
        public int? CompanyId { get; set; }
        public virtual ClientCompany Company { get; set; }
        public int? ProjectId { get; set; }
        public virtual Project Project { get; set; }

        public int? CategoryId { get; set; }
        public virtual SoftwareCategory Category { get; set; }

        public List<ProjectReportView> ProjectDetail { get; set; }

    }

    public class ProjectReportView
    {
        public int Id { get; set; }
        public int? ProjectId { get; set; }
        public virtual Project Project { get; set; }
        public string ProjectName { get; set; }
        public decimal? TargetWorkhours { get; set; }
        public decimal? WorkhoursConsumed { get; set; }
        public decimal? PendingWorkhours { get; set; }
        public string Remarks { get; set; }
    }
}