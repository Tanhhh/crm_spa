using Erp.BackOffice.App_GlobalResources;
using Erp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Erp.BackOffice.Sale.Models
{
    public class MembershipThuGonViewModel
    {
        public Int64? Id { get; set; }
        public Nullable<int> TargetId { get; set; }

        [Display(Name = "CustomerName", ResourceType = typeof(Wording))]
        public string CustomerName { get; set; }

        [Display(Name = "CustomerCode", ResourceType = typeof(Wording))]
        public string CustomerCode { get; set; }

        [Display(Name = "CreatedDate", ResourceType = typeof(Wording))]
        public DateTime? CreatedDate { get; set; }

        [Display(Name = "InquiryType", ResourceType = typeof(Wording))]
        public string Type { get; set; }

        [Display(Name = "CreatedUser", ResourceType = typeof(Wording))]
        public int? CreatedUserId { get; set; }

        [Display(Name = "ProductInvoiceCode", ResourceType = typeof(Wording))]
        public string ProductInvoiceCode { get; set; }

        public string ProductName { get; set; }
        public string ManagerName { get; set; }
        public int? Quantity { get; set; }
        public int? SOLUONG { get; set; }
        public int? soluongdung { get; set; }
        public int? soluongconlai { get; set; }    
        public Nullable<int> BranchId { get; set; }
        [Display(Name = "Customer", ResourceType = typeof(Wording))]
        public int? CustomerId { get; set; }
        public int? ProductInvoiceId { get; set; }
        public int? soluongtra { get; set; }
        public int? soluongchuyen { get; set; }    

    }
}