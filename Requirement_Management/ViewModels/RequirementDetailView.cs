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
        public string Requirement { get; set; }
        public string Description { get; set; }
        public decimal Workdays { get; set; }
        public int? ReqTypeId { get; set; }
        public virtual RequirementType ReqType { get; set; }
        public string ReqTypeName { get; set; }
        public int? ReqId { get; set; }
        public virtual Requirement Req { get; set; }
        public Status Status { get; set; }
        public bool StarMarked { get; set; }

        public int? SoftCategoryId { get; set; }
        public virtual SoftwareCategory SoftCategory { get; set; }
        public string SoftCategoryName { get; set; }
        public string ReqSoftwareId { get; set; }
        public string ReqSoftwareName { get; set; }
    }
}