using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Erp.BackOffice.Crm.Models
{
    public class vwKH_SAPHETSPViewModel
    {
        public int Id { get; set; }
        public int? CustomerId { get; set; }
        public string CustomerCode { get; set; }
        public string CompanyName { get; set; }
        public int? ProductInvoiceId { get; set; }
        public string ProductInvoiceCode { get; set; }
        public string ProductInvoiceStatus { get; set; }
        public string Ngay_xuatkho { get; set; }
        public int? Quantity { get; set; }
        public int? QuantityDayUsed { get; set; }
        public string SPSAPHETHAN { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public string ProductOutboundType { get; set; }
        public string Phone { get; set; }
        public int? is_checkedSAPHETSP { get; set; }
        public string GHI_CHU { get; set; }
        public bool isLock { get; set; }
        public int? ManagerStaffId { get; set; }
        public string ManagerStaffName { get; set; }
        public int? BranchId { get; set; }
    }
}