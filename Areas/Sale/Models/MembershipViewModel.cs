using Erp.BackOffice.App_GlobalResources;
using Erp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Erp.BackOffice.Sale.Models
{
    public class MembershipViewModel
    {
        public Int64 Id { get; set; }

        [Display(Name = "IsDeleted")]
        public bool? IsDeleted { get; set; }


        [Display(Name = "Is_Extend")]
        public Nullable<int> Is_extend { get; set; }



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
        public Nullable<int> CustomerId { get; set; }
        [Display(Name = "Service", ResourceType = typeof(Wording))]
        public Nullable<int> ProductId { get; set; }
        [Display(Name = "Status", ResourceType = typeof(Wording))]
        public string Status { get; set; }
        //[Display(Name = "TargetId", ResourceType = typeof(Wording))]
        public Nullable<Int64> TargetId { get; set; }
        //[Display(Name = "TargetModule", ResourceType = typeof(Wording))]
        public string TargetModule { get; set; }
        [Display(Name = "MembershipCode", ResourceType = typeof(Wording))]
        public string Code { get; set; }
        [Display(Name = "MaChungTu", ResourceType = typeof(Wording))]
        public string TargetCode { get; set; }
        [Display(Name = "ExpiryDate", ResourceType = typeof(Wording))]
        public Nullable<System.DateTime> ExpiryDate { get; set; }

        [Display(Name = "ExpiryDateOld", ResourceType = typeof(Wording))]
        public Nullable<System.DateTime> ExpiryDateOld { get; set; }

        [Display(Name = "BranchName", ResourceType = typeof(Wording))]
        public Nullable<int> BranchId { get; set; }
        [Display(Name = "BranchName", ResourceType = typeof(Wording))]
        public string BranchName { get; set; }
        [Display(Name = "BranchCode", ResourceType = typeof(Wording))]
        public string BranchCode { get; set; }
        [Display(Name = "CreateStaffName", ResourceType = typeof(Wording))]
        public string CreatedUserName { get; set; }
        [Display(Name = "CreateStaffCode", ResourceType = typeof(Wording))]
        public string CreatedUserCode { get; set; }
        [Display(Name = "ManagerStaff", ResourceType = typeof(Wording))]
        public string ManagerName { get; set; }
        [Display(Name = "ManagerStaff", ResourceType = typeof(Wording))]
        public Nullable<int> ManagerId { get; set; }

        [Display(Name = "InquiryType", ResourceType = typeof(Wording))]
        public string Type { get; set; }
        [Display(Name = "CustomerName", ResourceType = typeof(Wording))]
        public string CustomerName { get; set; }
        [Display(Name = "CustomerCode", ResourceType = typeof(Wording))]
        public string CustomerCode { get; set; }
        [Display(Name = "ServiceCode", ResourceType = typeof(Wording))]
        public string ProductCode { get; set; }
        [Display(Name = "Service", ResourceType = typeof(Wording))]
        public string ProductName { get; set; }
        public string CustomerImage { get; set; }


        public string idcu { get; set; }
        public Nullable<int> solandadung { get; set; }
        public Nullable<int> solantratra { get; set; }
        public Nullable<int> TongLanCSD { get; set; }
        public Nullable<int> ProductInvoiceId_Return { get; set; }

    }

    public class MemberReceipt
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
}