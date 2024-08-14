
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Erp.BackOffice.Sale.Models
{
    public class Sale_BaoCaoCongNoTongHopNCCViewModel
    {
        public string MaChungTuGoc { get; set; }
        public string SupplierCode { get; set; }
        public string SupplierName { get; set; }
        public decimal? TotalAmount { get; set; }
        public decimal? cnt { get; set; }
        public DateTime? NextPaymentDate { get; set; }
        public int? daytra { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}