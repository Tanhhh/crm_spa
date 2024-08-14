using Erp.BackOffice.App_GlobalResources;
using Erp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Erp.BackOffice.Sale.Models
{
    public class InquiryCardViewModel
    {
        public int Id { get; set; }

        [Display(Name = "IsDeleted")]
        public bool? IsDeleted { get; set; }

        [Display(Name = "CreatedUser", ResourceType = typeof(Wording))]
        public int? CreatedUserId { get; set; }
        public string CreatedUserName { get; set; }

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

        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Error))]
        [StringLength(100, ErrorMessageResourceType = typeof(Error), ErrorMessageResourceName = "StringError", ErrorMessage = null)]
        [Display(Name = "InquiryCode", ResourceType = typeof(Wording))]
        public string Code { get; set; }

        public string TargetModule { get; set; }
        public Nullable<int> TargetId { get; set; }

        //[Display(Name = "CreateStaffId", ResourceType = typeof(Wording))]
        //public Nullable<int> CreateStaffId { get; set; }
        //[Display(Name = "AdviseCard", ResourceType = typeof(Wording))]
        //public Nullable<int> AdviseCardId { get; set; }
        [Display(Name = "Customer", ResourceType = typeof(Wording))]
        public Nullable<int> CustomerId { get; set; }
        [Display(Name = "Product", ResourceType = typeof(Wording))]
        public Nullable<int> ProductId { get; set; }
        [Display(Name = "ExecutionTime", ResourceType = typeof(Wording))]
        public Nullable<int> TotalMinute { get; set; }
        [Display(Name = "WorkDay", ResourceType = typeof(Wording))]
        [Required(ErrorMessage="Bạn cần nhập thời gian")]
        public Nullable<System.DateTime> WorkDay { get; set; }
        [Display(Name = "Note", ResourceType = typeof(Wording))]
        public string Note { get; set; }
        [Display(Name = "ManagerStaff", ResourceType = typeof(Wording))]
        public Nullable<int> ManagerUserId { get; set; }
       

        [Display(Name = "InquiryType", ResourceType = typeof(Wording))]
        public string Type { get; set; }
        [Display(Name = "BranchName", ResourceType = typeof(Wording))]
        public int? BranchId { get; set; }
        [Display(Name = "BranchName", ResourceType = typeof(Wording))]
        public string BranchName { get; set; }
        [Display(Name = "BranchCode", ResourceType = typeof(Wording))]
        public string BranchCode { get; set; }
        [Display(Name = "CustomerName", ResourceType = typeof(Wording))]
        public string CustomerName { get; set; }
        [Display(Name = "CustomerCode", ResourceType = typeof(Wording))]
        public string CustomerCode { get; set; }
        [Display(Name = "CreateStaffCode", ResourceType = typeof(Wording))]
        public string CreateUserCode { get; set; }
        [Display(Name = "CreateStaffName", ResourceType = typeof(Wording))]
        public string CreateUserName { get; set; }
        [Display(Name = "Service", ResourceType = typeof(Wording))]
        public string ProductName { get; set; }
        [Display(Name = "ManagerStaff", ResourceType = typeof(Wording))]
        public string ManagerName { get; set; }
        [Display(Name = "ManagerStaffName", ResourceType = typeof(Wording))]
        public string ManagerStaffName { get; set; }
        [Display(Name = "ManagerCode", ResourceType = typeof(Wording))]
        public string ManagerCode { get; set; }
         [Display(Name = "ServiceCode", ResourceType = typeof(Wording))]
        public string ProductCode { get; set; }
        //[Display(Name = "AdviseCode", ResourceType = typeof(Wording))]
        public string TargetCode { get; set; }
        public string Status { get; set; }
        public string CustomerImage { get; set; }
        public List<InquiryCardDetailViewModel> DetailList { get; set; }
        public string EquimentGroup { get; set; }
        public List<ServiceDetailViewModel> DetailServiceList { get; set; }

        [Display(Name = "CounselorName", ResourceType = typeof(Wording))]
        public string CounselorName { get; set; }

        [Display(Name = "SkinscanUser", ResourceType = typeof(Wording))]
        public Nullable<int> SkinscanUserId { get; set; }
        [Display(Name = "SkinscanUser", ResourceType = typeof(Wording))]
        public string SkinscanUserName { get; set; }


        public int InquiryCardId { get; set; }
        //public string Code { get; set; }
        public string InquiryCardType { get; set; }
        public string Phone { get; set; }
        [Display(Name = "CustomerGroup", ResourceType = typeof(Wording))]
        public string CustomerGroup { get; set; }
        [Display(Name = "SkinLevel", ResourceType = typeof(Wording))]
        public string SkinSkinLevel { get; set; }
        [Display(Name = "HairlLevel", ResourceType = typeof(Wording))]
        public string HairlLevel { get; set; }
        [Display(Name = "GladLevel", ResourceType = typeof(Wording))]
        public string GladLevel { get; set; }
        public int ManagerStaffId { get; set; }
    }
}