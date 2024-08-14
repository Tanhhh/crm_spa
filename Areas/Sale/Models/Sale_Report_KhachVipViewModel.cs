
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Erp.BackOffice.Sale.Models
{
    public class Sale_Report_KhachVipViewModel
    {
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string Code { get; set; }
        public string CompanyName { get; set; }
        public string Status { get; set; }
        public int? Ratings { get; set; }

    }
}