
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Erp.BackOffice.Sale.Models
{
    public class Sale_Report_SaleReturnsViewModel
    {
        public string Status { get; set; }
        public string Code { get; set; }
        public int? TotalAmount { get; set; }
        public int? Quantity { get; set; }
        public string CustomerName { get; set; }
        public string CustomerCode { get; set; }
        public int? SalesReturnsId { get; set; }
    }
}