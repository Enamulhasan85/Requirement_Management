using Requirement_Management.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Requirement_Management.ViewModels
{
    public class RequirementView
    {
        public RequirementView()
        {
            ReqDetail = new List<RequirementDetailView>();
            FilePath = new List<string>();
        }

        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime Date { get; set; }
        public DateTime EntryDate { get; set; }
        public int? CompanyId { get; set; }
        public virtual ClientCompany Company { get; set; }
        public string CompanyName { get; set; }
        public int? ReqProviderId { get; set; }
        public virtual RequirementProvider ReqProvider { get; set; }
        public string ReqProviderName { get; set; }

        public List<RequirementDetailView> ReqDetail { get; set; }
        public List<string> FilePath { get; set; }
    }
}