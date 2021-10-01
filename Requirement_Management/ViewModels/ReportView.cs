using Requirement_Management.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Requirement_Management.ViewModels
{
    public class ReportView
    {
        public ReportView()
        {
            ReqDetail = new List<DetailReportView>();
        }

        public int Id { get; set; }
        public int? ReqTypeId { get; set; }
        public virtual RequirementType ReqType { get; set; }
        public Status Status { get; set; }
        public bool StarMarked { get; set; }


        public DateTime From { get; set; }
        public DateTime Date { get; set; }
        public DateTime To { get; set; }
        public int? CompanyId { get; set; }
        public virtual ClientCompany Company { get; set; }
        public int? ReqProviderId { get; set; }
        public virtual RequirementProvider ReqProvider { get; set; }

        public List<int> SoftwareId { get; set; }

        public List<DetailReportView> ReqDetail { get; set; }
    }
}