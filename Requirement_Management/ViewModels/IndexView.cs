using Requirement_Management.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Requirement_Management.ViewModels
{
    public enum Query
    {
        EntryDate,
        ReqDate
    };

    public class IndexView
    {
        public IndexView()
        {
            From = DateTime.Today;
            To = DateTime.Today;
            Req = new List<Requirement>();
        }

        public int? Id { get; set; }
        public DateTime From { get; set; }
        public DateTime Date { get; set; }
        public DateTime To { get; set; }
        public int? CompanyId { get; set; }
        public virtual ClientCompany Company { get; set; }
        public int? ReqProviderId { get; set; }
        public virtual RequirementProvider ReqProvider { get; set; }
        public Query Query { get; set; }

        public List<Requirement> Req { get; set; }
    }
}