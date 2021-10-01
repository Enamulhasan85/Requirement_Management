using Requirement_Management.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Requirement_Management.ViewModels
{
    public class DetailReportView
    {
        public int Id { get; set; }
        public string Requirement { get; set; }
        public string Description { get; set; }
        public int? ReqTypeId { get; set; }
        public virtual RequirementType ReqType { get; set; }
        public string ReqTypeName { get; set; }
        public Status Status { get; set; }
        public bool StarMarked { get; set; }

        public DateTime Date { get; set; }
        public int? CompanyId { get; set; }
        public virtual ClientCompany Company { get; set; }
        public string CompanyName { get; set; }
        public int? ReqProviderId { get; set; }
        public virtual RequirementProvider ReqProvider { get; set; }
        public string ReqProviderName { get; set; }

        public string STRSoftwareId { get; set; }
        public string STRSoftwareName { get; set; }
    }
}