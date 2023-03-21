using Requirement_Management.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Requirement_Management.ViewModels
{
    public class DetailsRepForEdit
    {
        public int RequirementId { get; set; }
        public int RequirementDetailId { get; set; }

        public int? ReqProviderId { get; set; }
        public virtual RequirementProvider ReqProvider { get; set; }
        public string Requirement { get; set; }
        public string Description { get; set; }

        public int? ReqTypeId { get; set; }
        public virtual RequirementType ReqType { get; set; }

        public int? ProjectId { get; set; }
        public int? ProjectScheduleId { get; set; }

        public int? CategoryId { get; set; }
        public int? eCategoryId { get; set; }
        public List<int> SoftwareId { get; set; }
        public RequirementMode ReqMode { get; set; }
        public Status Status { get; set; }
        public Priority Priority { get; set; }
    }

    public class DetailsRepForEditRecord
    {
        public int Id { get; set; }
        public int? JobHolderId { get; set; }
        public virtual JobHolder JobHolder { get; set; }
        public string JobHolderName { get; set; }

        public int RequirementDetailId { get; set; }
        public virtual RequirementDetail RequirementDetail { get; set; }

        public Status? CurrStatus { get; set; }
        public Status TargetStatus { get; set; }

        public DateTime AssignDate { get; set; }
        public DateTime DeadLine { get; set; }

        public decimal TargetWorkhours { get; set; }
        public decimal WorkhoursConsumed { get; set; }
        public CompType? CompType { get; set; }
        public DateTime? CompDate { get; set; }
        public string Note { get; set; }
        public string DevNote { get; set; }

        public decimal HourlyCost { get; set; }
    }
}