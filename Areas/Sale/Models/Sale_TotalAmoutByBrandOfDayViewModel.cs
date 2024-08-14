
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Erp.BackOffice.Sale.Models
{
    public class Sale_TotalAmoutByBrandOfDayViewModel
    {
        public int? CustomerId { get; set; }

        public string CountForBrand { get; set; }

        public decimal? TotalAmount { get; set; }

        public decimal? PaidAmount { get; set; }

        public decimal? DoanhThu { get; set; }

        public decimal? RemainingAmount { get; set; }
    }
}