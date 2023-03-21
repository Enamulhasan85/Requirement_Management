using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Requirement_Management.Models
{
    public enum ScheduleStatus
    {
        Active,
        InActive
    };

    public class ProjectSchedule
    {
        public int Id { get; set; }
        public int? ProjectId { get; set; }
        public virtual Project Project { get; set; }
        public RequirementMode ProjectMode { get; set; }
        public decimal TargetCost { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? TargetDate { get; set; }
        public DateTime? EndDate { get; set; }
        public ScheduleStatus Status { get; set; }
    }
}