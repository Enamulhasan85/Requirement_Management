using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Requirement_Management.Models
{
    public class RequirementProvider
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Contact { get; set; }
        public string Email { get; set; }
    }


    public class CompanyProvider
    {
        public int Id { get; set; }
        public int? CompanyId { get; set; }
        public virtual ClientCompany Company { get; set; }
        public int? ReqProviderId { get; set; }
        public virtual RequirementProvider ReqProvider { get; set; }
    }
}