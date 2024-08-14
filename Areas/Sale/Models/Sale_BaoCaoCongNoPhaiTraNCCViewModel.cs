
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Erp.BackOffice.Sale.Models
{
    public class Sale_BaoCaoCongNoPhaiTraNCCViewModel
    {
        public string SupplierCode { get; set; }
        public string SupplierName { get; set; }
        public decimal? CongNo { get; set; }

    }
}