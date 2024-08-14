using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Erp.BackOffice.Sale.Models
{
    public class CancelledTransferredInvoiceViewModel
    {
        public int Id { get; set; } 
        public int FromProductInvoiceId { get; set; }
        public int ToProductInvoiceId { get; set; }
        public string Code { get; set; }
        public int CustomerId { get; set; }

        public string CustomerName { get; set; }

        public decimal? TotalAmount { get; set; }

        public decimal? TransferredMoney { get; set; }
        public string Status { get; set; }

        public string CountForBrand { get; set; }
    }
}