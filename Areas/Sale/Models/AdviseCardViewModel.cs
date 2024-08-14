using Erp.BackOffice.App_GlobalResources;
using Erp.BackOffice.Areas.Cms.Models;
using Erp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Erp.BackOffice.Sale.Models
{
    public class AdviseCardViewModel
    {
        public int Id { get; set; }

        [Display(Name = "IsDeleted")]
        public bool? IsDeleted { get; set; }

        [Display(Name = "CreatedUser", ResourceType = typeof(Wording))]
        public int? CreatedUserId { get; set; }
        [Display(Name = "CreateStaffName", ResourceType = typeof(Wording))]
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

        [Display(Name = "Customer", ResourceType = typeof(Wording))]
        public Nullable<int> CustomerId { get; set; }
        [Display(Name = "CustomerName", ResourceType = typeof(Wording))]
        public string CustomerName { get; set; }
        [Display(Name = "CustomerCode", ResourceType = typeof(Wording))]
        public string CustomerCode { get; set; }
        [Display(Name = "AdviseCode", ResourceType = typeof(Wording))]

        public string Code { get; set; }
        [Display(Name = "CounselorId", ResourceType = typeof(Wording))]
        public Nullable<int> CounselorId { get; set; }
        [Display(Name = "Note", ResourceType = typeof(Wording))]
        public string Note { get; set; }
        public bool IsActived { get; set; }
        [Display(Name = "BranchName", ResourceType = typeof(Wording))]
        public int? BranchId { get; set; }

        [Display(Name = "BranchName", ResourceType = typeof(Wording))]
        public string BranchName { get; set; }
        [Display(Name="CustomerAddress",ResourceType=typeof(Wording))]
        public string CustomerAddress { get; set; }
        [Display(Name="CustomerBirthday",ResourceType=typeof(Wording))]
        public DateTime? CustomerBirthday { get; set; }

        [Display(Name = "CounselorName", ResourceType = typeof(Wording))]
        public string CounselorName { get; set; }
        [Display(Name = "BranchCode", ResourceType = typeof(Wording))]
        public string BranchCode { get; set; }

        [Display(Name = "CreateStaffCode", ResourceType = typeof(Wording))]
        public string CreatedUserCode { get; set; }

        [Display(Name = "CounselorCode", ResourceType = typeof(Wording))]
        public string CounselorCode { get; set; }
        [Display(Name = "AdviseType", ResourceType = typeof(Wording))]
        public string Type { get; set; }

        public List<CategoryViewModel> ListAdviseType { get; set; }
        public string JavaScriptToRun { get; set; }
        public string CustomerPhone { get; set; }
        public string ManagerStaffName { get; set; }
        
    }
}