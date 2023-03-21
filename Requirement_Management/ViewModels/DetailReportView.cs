using Requirement_Management.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Requirement_Management.ViewModels
{
    public class DetailReportView
    {
        public int Id { get; set; }
        public int RequirementId { get; set; }
        public int? ReqProviderId { get; set; }
        public virtual RequirementProvider ReqProvider { get; set; }
        public string ReqProviderName { get; set; }
        public string Requirement { get; set; }
        public string Description { get; set; }
        public int? ReqTypeId { get; set; }
        public virtual RequirementType ReqType { get; set; }
        public string ReqTypeName { get; set; }
        public RequirementMode ReqMode { get; set; }
        public Status Status { get; set; }
        public Priority Priority { get; set; }

        public DateTime Date { get; set; }
        public int? CompanyId { get; set; }
        public virtual ClientCompany Company { get; set; }
        public string CompanyName { get; set; }
        public int? ProjectId { get; set; }
        public virtual Project Project { get; set; }
        public string ProjectName { get; set; }
        public int? ProjectScheduleId { get; set; }
        public virtual ProjectSchedule ProjectSchedule { get; set; }
        public string ScheduleName { get; set; }
        public int? SoftCategoryId { get; set; }
        public virtual RequirementProvider SoftCategory { get; set; }
        public string SoftCategoryName { get; set; }

        public string STRSoftwareId { get; set; }
        public string STRSoftwareName { get; set; }

        public decimal TotalTarWorkhoursinReq { get; set; }
        public decimal TotalConWorkhoursinReq { get; set; }
        public int? JobHolderId { get; set; }
        public virtual JobHolder JobHolder { get; set; }
        public String JobHolderName { get; set; }

        public decimal? HourlyCost { get; set; }

        public Status CurrStatus { get; set; }
        public Status TargetStatus { get; set; }
        public DateTime? AssignDate { get; set; }
        public DateTime? DeadLine { get; set; }
        public decimal? TargetWorkhours { get; set; }
        public decimal? WorkhoursConsumed { get; set; }
        public CompType? CompType { get; set; }
        public DateTime? CompDate { get; set; }
        public string Note { get; set; }
        public string DevNote { get; set; }
    }
}