
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Erp.BackOffice.Sale.Models
{
    public class Sale_DetailTourOfStaffViewModel
    {
        public int? IdNV { get; set; }
        public int? TotalMinute { get; set; }
        public string SumTotalMinute { get; set; }
        public string TotalDayHourMinute { get; set; }
        public string NameNV { get; set; }
        public int Id { get; set; }
        public int? BranchId { get; set; }
        public int? ProductId { get; set; }
        public string ProductName { get; set; }
        public Nullable<System.DateTime> WorkDay { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<System.DateTime> EndDate { get; set; }
        public string getPeriodOfTime { get; set; }
        public string getCreatedDate { get; set; }
        public int? days { get; set; }
        public int? hours { get; set; }
        public int? mins { get; set; }

    }
}