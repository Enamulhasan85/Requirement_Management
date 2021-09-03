using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Requirement_Management.Models
{

    public class Requirement
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime Date { get; set; }
        public DateTime EntryDate { get; set; }
        public int? CompanyId { get; set; }
        public virtual ClientCompany Company { get; set; }
        public int? ReqProviderId { get; set; }
        public virtual RequirementProvider ReqProvider { get; set; }
    }
}