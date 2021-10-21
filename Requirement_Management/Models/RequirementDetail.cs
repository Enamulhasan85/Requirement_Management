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
        Deployed
    };

    public class RequirementDetail
    {
        public int Id { get; set; }
        public string Requirement { get; set; }
        public string Description { get; set; }
        public decimal Workdays { get; set; }
        public int? SoftCategoryId { get; set; }
        public virtual SoftwareCategory SoftCategory { get; set; }
        public int? ReqTypeId { get; set; }
        public virtual RequirementType ReqType { get; set; }
        public int? ReqId { get; set; }
        public virtual Requirement Req { get; set; }
        public Status Status { get; set; }
        public bool StarMarked { get; set; }
    }
}