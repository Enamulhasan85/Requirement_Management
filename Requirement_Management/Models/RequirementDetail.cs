using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Requirement_Management.Models
{
    public enum Status
    {
        Submitted,
        Verified,
        Working,
        Done,
        Tested,
        Deployed,
        Canceled
    };

    public enum RequirementMode
    {
        Project,
        Task,
        Maintenance,
        Bug,
        Free_Service
    };

    public enum Priority
    {
        High,
        Mid,
        Low
    }

    public class RequirementDetail
    {
        public int Id { get; set; }
        public int? ReqProviderId { get; set; }
        public virtual RequirementProvider ReqProvider { get; set; }
        public string Requirement { get; set; }
        public string Description { get; set; }

        public int? SoftCategoryId { get; set; }
        public virtual SoftwareCategory SoftCategory { get; set; }

        public int? ReqTypeId { get; set; }
        public virtual RequirementType ReqType { get; set; }
        public int? ReqId { get; set; }
        public virtual Requirement Req { get; set; }

        public int? ProjectId { get; set; }
        public virtual Project Project { get; set; }
        public int? ProjectScheduleId { get; set; }
        public virtual ProjectSchedule ProjectSchedule { get; set; }

        public Status Status { get; set; }
        public RequirementMode ReqMode { get; set; }
        public Priority Priority { get; set; }
    }
}