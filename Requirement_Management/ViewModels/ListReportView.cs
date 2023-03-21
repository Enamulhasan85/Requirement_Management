using Requirement_Management.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Requirement_Management.ViewModels
{
    public class ListReportView
    {
        public ListReportView()
        {
            ListReport = new List<ProjectListReportView>();
        }

        public int? CompanyId { get; set; }
        public virtual ClientCompany Company { get; set; }
        public int? ProjectId { get; set; }
        public virtual Project Project { get; set; }
        public Mode? Mode { get; set; }
        public List<ProjectListReportView> ListReport { get; set; }
    }

    public class ProjectListReportView
    {
        public string Title { get; set; }
        public DateTime StartDate { get; set; }
        public int? CompanyId { get; set; }
        public virtual ClientCompany Company { get; set; }
        public string CompanyName { get; set; }
    }
}