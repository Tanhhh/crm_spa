
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Erp.BackOffice.Sale.Models
{
    public class Sale_BaoCaoDoanhSoThucBanViewModel
    {
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string ProductInvoiceCode { get; set; }
        public string BranchName { get; set; }
        public string CustomerName { get; set; }
        public int? TotalAmount { get; set; }
        public int? Id { get; set; }
        public int? CustomerId { get; set; }

    }
}