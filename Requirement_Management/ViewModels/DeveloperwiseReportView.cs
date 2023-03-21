using Requirement_Management.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Requirement_Management.ViewModels
{
    public class DeveloperwiseReportView
    {
        public DeveloperwiseReportView()
        {
            From = DateTime.Today;
            To = DateTime.Today;
            DevReportTable = new List<DeveloperwiseReportViewTable>();
            SoftwareId = new List<int>();
        }

        public int Id { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public int? CompanyId { get; set; }
        public virtual ClientCompany Company { get; set; }
        public int? ProjectId { get; set; }
        public virtual Project Project { get; set; }
        public int? JobHolderId { get; set; }
        public virtual JobHolder JobHolder { get; set; }

        public int? ReqProviderId { get; set; }
        public virtual RequirementProvider ReqProvider { get; set; }
        public int? ReqTypeId { get; set; }
        public virtual RequirementType ReqType { get; set; }
        public Status? Status { get; set; }
        public RequirementMode? ReqMode { get; set; }
        public Priority? Priority { get; set; }
        public int? CategoryId { get; set; }
        public virtual SoftwareCategory Category { get; set; }
        public int? ProjectScheduleId { get; set; }
        public virtual ProjectSchedule ProjectSchedule { get; set; }

        public List<int> SoftwareId { get; set; }

        public List<DeveloperwiseReportViewTable> DevReportTable { get; set; }

    }

    public class DeveloperwiseReportViewTable
    {
        public int Id { get; set; }
        public int? JobHolderId { get; set; }
        public virtual JobHolder JobHolder { get; set; }
        public string JobHolderName { get; set; }
        public string ProjectName { get; set; }
        public decimal? TargetWorkhours { get; set; }
        public decimal? WorkhoursConsumed { get; set; }
        public decimal? PendingWorkhours { get; set; }
        public string Remarks { get; set; }
    }
}