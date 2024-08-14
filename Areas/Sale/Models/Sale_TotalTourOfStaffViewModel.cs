
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Erp.BackOffice.Sale.Models
{
    public class Sale_TotalTourOfStaffViewModel
    {
        public int IdNV { get; set; }
        public string FullName { get; set; }
        public int? Quantity { get; set; }
    }
}