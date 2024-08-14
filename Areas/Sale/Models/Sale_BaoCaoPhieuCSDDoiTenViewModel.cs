
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Erp.BackOffice.Sale.Models
{
    public class Sale_BaoCaoPhieuCSDDoiTenViewModel
    {
        public DateTime? CreatedDate { get; set; }
        public string Code { get; set; }
        public string CompanyName { get; set; }
        public string ProductName { get; set; }
        public DateTime? ExpiryDate { get; set; }

    }
}