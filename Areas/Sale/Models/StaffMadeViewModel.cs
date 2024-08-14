using Erp.BackOffice.App_GlobalResources;
using Erp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Erp.BackOffice.Sale.Models
{
    public class StaffMadeViewModel
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

        //[Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Error))]
        //[StringLength(100, ErrorMessageResourceType = typeof(Error), ErrorMessageResourceName = "StringError", ErrorMessage = null)]
        //[Display(Name = "Name", ResourceType = typeof(Wording))]
        //public string Name { get; set; }

        //[Display(Name = "User", ResourceType = typeof(Wording))]
        public Nullable<int> UserId { get; set; }
        [Display(Name = "Status", ResourceType = typeof(Wording))]
        public string Status { get; set; }
        [Display(Name = "Note", ResourceType = typeof(Wording))]
        public string Note { get; set; }
        //[Display(Name = "SchedulingHistoryId", ResourceType = typeof(Wording))]
        public Nullable<int> SchedulingHistoryId { get; set; }
        public string FullName { get; set; }
        public string UserCode { get; set; }
        public string ProfileImage { get; set; }
        public Nullable<System.DateTime> WorkDay { get; set; }
        public Nullable<int> BranchId { get; set; }

        public string RoomName { get; set; }
        public string FloorName { get; set; }
        public string Type { get; set; }
        public string ProductName { get; set; }
        public string ProductCode { get; set; }
        public string SchedulingCode { get; set; }
        public string SchedulingStatus { get; set; }
        public string CustomerName { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerImage { get; set; }

        public Nullable<System.DateTime> ExpectedWorkDay { get; set; }
        public Nullable<int> TotalMinute { get; set; }
        public Nullable<System.DateTime> ExpectedEndDate { get; set; }
        public Nullable<System.DateTime> EndDate { get; set; }
    }
}