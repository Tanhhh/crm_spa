using Erp.BackOffice.App_GlobalResources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Erp.BackOffice.Sale.Models
{
    public class vwPlanuseSkinCareViewModel
    {
        public Int64 Id { get; set; }
        public int? CustomerId { get; set; }
        public int? BranchId { get; set; }
        public Int64? TargetId { get; set; }
        public string ProductInvoiceCode { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        [Display(Name = "CustomerName", ResourceType = typeof(Wording))]
        public string CustomerName { get; set; }
        [Display(Name = "CustomerCode", ResourceType = typeof(Wording))]
        public string CustomerCode { get; set; }
        [Display(Name = "ProductName", ResourceType = typeof(Wording))]
        public string ProductName { get; set; }
        public int? Quantity { get; set; }
        public string ManagerName { get; set; }
        public string Type { get; set; }
        public int? CreatedUserId { get; set; }
        public int? SOLUONG { get; set; }
        public int? soluongdung { get; set; }
        public int? soluongtra { get; set; }
        public int? soluongchuyen { get; set; }
        public int? soluongconlai { get; set; }
        [Display(Name = "Phone", ResourceType = typeof(Wording))]
        public string Phone { get; set; }
        public string GladLevel { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        [Display(Name = "Amount", ResourceType = typeof(Wording))]
        public decimal? Amount { get; set; }
        public int? ProductInvoiceId { get; set; }
        public int? ManagerStaffId { get; set; }
        public string THANGTOANHET { get; set; }
    }
}