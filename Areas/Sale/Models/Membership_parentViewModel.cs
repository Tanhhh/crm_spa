using Erp.BackOffice.App_GlobalResources;
using Erp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Erp.BackOffice.Sale.Models
{
    public class Membership_parentViewModel
    {
        public Int64 Id { get; set; }

        [Display(Name = "IsDeleted")]
        public bool? IsDeleted { get; set; }


        [Display(Name = "Is_Extend")]
        public Nullable<int> is_extend { get; set; }

        [Display(Name = "CreatedUser", ResourceType = typeof(Wording))]
        public int? CreatedUserId { get; set; }

        [Display(Name = "CreatedDate", ResourceType = typeof(Wording))]
        public DateTime? CreatedDate { get; set; }

        [Display(Name = "ModifiedUser", ResourceType = typeof(Wording))]
        public int? ModifiedUserId { get; set; }
        public string ModifiedUserName { get; set; }

        [Display(Name = "ModifiedDate", ResourceType = typeof(Wording))]
        public DateTime? ModifiedDate { get; set; }

        [Display(Name = "AssignedUserId", ResourceType = typeof(Wording))]
        public int? AssignedUserId { get; set; }
        public string AssignedUserName { get; set; }

        [Display(Name = "Customer", ResourceType = typeof(Wording))]
        public int? CustomerId { get; set; }

        [Display(Name = "ProductInvoice", ResourceType = typeof(Wording))]
        public int ProductInvoiceId { get; set; }

        [Display(Name = "Status", ResourceType = typeof(Wording))]
        public string Status { get; set; }

        [Display(Name = "MembershipCode", ResourceType = typeof(Wording))]
        public string Code { get; set; }

        public string QRCode { get; set; }

        [Display(Name = "Note", ResourceType = typeof(Wording))]
        public string Note { get; set; }

        [Display(Name = "SN", ResourceType = typeof(Wording))]
        public string SerialNumber { get; set; }

        [Display(Name = "ExpiryDate", ResourceType = typeof(Wording))]
        public Nullable<System.DateTime> ExpiryDate { get; set; }

        public int Total { get; set; }

        public bool isPrint { get; set; }

        public Nullable<int> NumberPrint { get; set; }

        [Display(Name = "BranchName", ResourceType = typeof(Wording))]
        public int BranchId { get; set; }

        public Nullable<System.DateTime> ExpiryDateOld { get; set; }

        public string idcu { get; set; }

        public long ProductInvoiceDetaiId { get; set; }

        [Display(Name = "ProductId", ResourceType = typeof(Wording))]
        public Nullable<int> ProductId { get; set; }

        [Display(Name = "ProductId", ResourceType = typeof(Wording))]
        public string ProductName { get; set; }

        [Display(Name = "ProductCode", ResourceType = typeof(Wording))]
        public string ProductCode { get; set; }
        
        public string ProductType { get; set; }

        [Display(Name = "Quantity", ResourceType = typeof(Wording))]
        public Nullable<int> Quantity { get; set; }

        public Nullable<int> solandadung { get; set; }
        public Nullable<int> solanconlai { get; set; }
        public Nullable<int> ProductInvoiceId_Return { get; set; }

        [Display(Name = "CustomerName", ResourceType = typeof(Wording))]
        public string CustomerName { get; set; }
        [Display(Name = "CustomerCode", ResourceType = typeof(Wording))]
        public string CustomerCode { get; set; }
        [Display(Name = "ManagerStaff", ResourceType = typeof(Wording))]
        public string ManagerName { get; set; }
        [Display(Name = "ManagerStaff", ResourceType = typeof(Wording))]
        public Nullable<int> ManagerId { get; set; }

        [Display(Name = "ProductInvoiceCode", ResourceType = typeof(Wording))]
        public string TargetCode { get; set; }

        [Display(Name = "Leader", ResourceType = typeof(Wording))]
        public Nullable<int> ChiefUserId { get; set; }
        [Display(Name = "Leader", ResourceType = typeof(Wording))]
        public string ChiefUserFullName { get; set; }
        [Display(Name = "UserType", ResourceType = typeof(Wording))]
        public string UserTypeName_kd { get; set; }
        public decimal? tienconno { get; set; }
        public string ProductGroup { get; set; }

        public int? SOHOADON { get; set; }
        
    }
}