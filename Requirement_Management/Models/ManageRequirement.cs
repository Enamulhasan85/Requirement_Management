using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Requirement_Management.Models
{
    public enum CompType
    {
        Done,
        Transfered,
        Partially_Transfered,
        Removed
    };

    public class ManageRequirement
    {
        public int Id { get; set; }

        public int? JobHolderId { get; set; }
        public virtual JobHolder JobHolder { get;set;}

        public int RequirementDetailId { get; set; }
        public virtual RequirementDetail RequirementDetail { get; set; }

        public Status CurrStatus { get; set; }
        public Status TargetStatus { get; set; }

        public DateTime AssignDate { get; set; }
        public DateTime DeadLine { get; set; }
        public decimal TargetWorkhours { get; set; }
        public decimal HourlyCost { get; set; }
        public decimal WorkhoursConsumed { get; set; }
        public CompType? CompType { get; set; }
        public DateTime? CompDate { get; set; }
        public string Note { get; set; }
        public string DevNote { get; set; }
    }
}