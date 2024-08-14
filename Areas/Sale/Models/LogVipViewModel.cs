using Erp.BackOffice.App_GlobalResources;
using Erp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Erp.BackOffice.Sale.Models
{
    public class LogVipViewModel
    {
        public LogVipViewModel()
        {
            Year = DateTime.Now.Year;
            Ratings = "";
        }

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

        [Display(Name = "CustomerId", ResourceType = typeof(Wording))]
        public Nullable<int> CustomerId { get; set; }

        [Display(Name = "Status", ResourceType = typeof(Wording))]
        public string Status { get; set; }

        [Display(Name = "Ratings", ResourceType = typeof(Wording))]
        public string Ratings { get; set; }

        [Display(Name = "ApprovedUserId", ResourceType = typeof(Wording))]
        public Nullable<int> ApprovedUserId { get; set; }


        //[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString ="{0:dd/MM/yyyy}")]
        //[Display(Name = "Insert Start Date")]
        [Required(ErrorMessage = "You must specify the date of the event!")]
        [DataType(DataType.DateTime, ErrorMessage = "You must specify the date of the event")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}", ApplyFormatInEditMode = true)]
        [Display(Name = "ApprovedDate", ResourceType = typeof(Wording))]
        public Nullable<System.DateTime> ApprovedDate { get; set; }

        [Required(ErrorMessage = "Tổng tiền không được rỗng!")]
        [Display(Name = "TotalAmount", ResourceType = typeof(Wording))]
        public decimal? TotalAmount { get; set; }

        [Display(Name = "Year", ResourceType = typeof(Wording))]
        public Nullable<int> Year { get; set; }

        [Display(Name = "is_approved", ResourceType = typeof(Wording))]

        public bool is_approved { get; set; }

        public string Name { get; set; }
        [Display(Name = "Name", ResourceType = typeof(Wording))]
        public string LoyaltyPointName { get; set; }
        [Display(Name = "LoyaltyPointId", ResourceType = typeof(Wording))]
        public int? LoyaltyPointId { get; set; }
        public string CustomerName { get; set; }
        public string CustomerCode { get; set; }
        public string ApprovedUserName { get; set; }

        public int? solan { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string NhomNVKD { get; set; }
        public string ManagerStaffName { get; set; }
        public int? ManagerStaffId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public int? BranchId { get; set; }
    }
}