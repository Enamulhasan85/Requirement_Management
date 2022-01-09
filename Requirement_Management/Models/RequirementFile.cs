using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Requirement_Management.Models
{
    public class RequirementFile
    {
        public int Id { get; set; }
        public string Filename { get; set; }
        public int? ReqId { get; set; }
        public virtual Requirement Req { get; set; }
    }
}