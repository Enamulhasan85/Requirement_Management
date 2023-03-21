using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Requirement_Management.ViewModels
{
    public class CustomDropDownList
    {

    }

    public class CustomProjectScheduleDropDownList
    {
        public CustomProjectScheduleDropDownList(int Id, string date, string mode)
        {
            this.Id = Id;
            this.Name = mode + " - " + date;
        }

        public int Id { get; set; }
        public string Name { get; set; }

    }
}