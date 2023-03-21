using Requirement_Management.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Requirement_Management.ViewModels
{
    public enum ReqStatus
    {
        Submitted,
        Pending,
        Done
    };

    public class StatuswiseRequirementReportView
    {
        public StatuswiseRequirementReportView()
        {
            From = DateTime.Today;
            To = DateTime.Today;
            ReqReport = new List<ListOfStatuswiseRequirementReportView>();
        }

        public int Id { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public int? CompanyId { get; set; }
        public virtual ClientCompany Company { get; set; }
        public Mode? Mode { get; set; }
        public int? ProjectId { get; set; }
        public virtual Project Project { get; set; }
        public int? CategoryId { get; set; }
        public virtual SoftwareCategory Category { get; set; }

        public string CompanyName { get; set; }
        public string Date { get; set; }
        public ReqStatus? ReqStatus { get; set; }

        public List<ListOfStatuswiseRequirementReportView> ReqReport { get; set; }
    }

    public class ListOfStatuswiseRequirementReportView
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }

        public int? CompanyId { get; set; }
        public virtual ClientCompany Company { get; set; }
        public string CompanyName { get; set; }
        public int? ProjectId { get; set; }
        public virtual Project Project { get; set; }
        public string ProjectName { get; set; }

        public int? ReqProviderId { get; set; }
        public virtual RequirementProvider ReqProvider { get; set; }
        public string ProviderName { get; set; }

        public string Requirement { get; set; }

        public int? ReqTypeId { get; set; }
        public virtual RequirementType ReqType { get; set; }
        public string ReqTypeName { get; set; }

        public Status Status { get; set; }
        public Priority Priority { get; set; }
        public decimal? TargetWorkhours { get; set; }
        public decimal? WorkhoursConsumed { get; set; }
        public decimal? PendingWorkhours { get; set; }
    }
}