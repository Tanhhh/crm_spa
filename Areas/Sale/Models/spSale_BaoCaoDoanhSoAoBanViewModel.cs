
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Erp.BackOffice.Sale.Models
{
    public class spSale_BaoCaoDoanhSoAoBanViewModel
    {
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string ProductInvoiceCode { get; set; }
        public string BranchName { get; set; }
        public string CustomerName { get; set; }
        public int? TotalAmount { get; set; }
        public int? Id { get; set; }
        public int? CustomerId { get; set; }
        public string UserCode { get; set; }
        public string UserName { get; set; }
        public int? IsCNC { get; set; }

    }
}