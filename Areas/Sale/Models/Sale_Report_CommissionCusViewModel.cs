
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Erp.BackOffice.Sale.Models
{
    public class Sale_Report_CommissionCusViewModel
    {
        public string Name { get; set; }
        public string ProductName { get; set; }
        public string ProductCode { get; set; }
        public int? Quantity { get; set; }
        public int? ExpiryMonth { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

    }
}