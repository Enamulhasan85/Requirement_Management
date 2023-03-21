using Requirement_Management.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Requirement_Management.ViewModels
{
    public class RequirementDetailView
    {
        public int Id { get; set; }
        public int? ReqProviderId { get; set; }
        public virtual RequirementProvider ReqProvider { get; set; }
        public string ReqProviderName { get; set; }
        public string Requirement { get; set; }
        public string Description { get; set; }
        public int? ReqTypeId { get; set; }
        public virtual RequirementType ReqType { get; set; }
        public string ReqTypeName { get; set; }
        public int? ReqId { get; set; }
        public virtual Requirement Req { get; set; }
        public Status Status { get; set; }
        public RequirementMode ReqMode { get; set; }
        public Priority Priority { get; set; }

        public int? ProjectId { get; set; }
        public virtual Project Project { get; set; }
        public string ProjectName { get; set; }
        public int? ProjectScheduleId { get; set; }
        public virtual ProjectSchedule ProjectSchedule { get; set; }
        public string ScheduleName { get; set; }

        public int? SoftCategoryId { get; set; }
        public virtual SoftwareCategory SoftCategory { get; set; }
        public string SoftCategoryName { get; set; }
        public string ReqSoftwareId { get; set; }
        public string ReqSoftwareName { get; set; }
    }
}