using Erp.BackOffice.App_GlobalResources;
using Erp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Erp.BackOffice.Account.Models
{
    public class CustomerUserViewModel
    {
        public int Id { get; set; }


        [Display(Name = "IsDeleted")]
        public bool? IsDeleted { get; set; }

        [Display(Name = "CreatedDate", ResourceType = typeof(Wording))]
        public DateTime? CreatedDate { get; set; }

        [Display(Name = "CreatedUser", ResourceType = typeof(Wording))]
        public int? CreatedUserId { get; set; }

        [Display(Name = "ModifiedDate", ResourceType = typeof(Wording))]
        public DateTime? ModifiedDate { get; set; }

        [Display(Name = "ModifiedUser", ResourceType = typeof(Wording))]
        public int? ModifiedUserId { get; set; }

        [Display(Name = "AssignedUserId", ResourceType = typeof(Wording))]
        public int? AssignedUserId { get; set; }

        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Error))]
        [StringLength(100, ErrorMessageResourceType = typeof(Error), ErrorMessageResourceName = "StringError", ErrorMessage = null)]
        [Display(Name = "Name", ResourceType = typeof(Wording))]
        public string Name { get; set; }

        [Display(Name = "Description", ResourceType = typeof(Wording))]
        public string Description { get; set; }

        [Display(Name = "StartDate", ResourceType = typeof(Wording))]
        public Nullable<System.DateTime> StartDate { get; set; }

        [Display(Name = "EndDate", ResourceType = typeof(Wording))]
        public Nullable<System.DateTime> EndDate { get; set; }

        [Display(Name = "Customer", ResourceType = typeof(Wording))]
        public Nullable<int> CustomerId { get; set; }

        [Display(Name = "FullName", ResourceType = typeof(Wording))]
        public string FullName { get; set; }

        public int? Index { get; set; }
        public int? OrderNo { get; set; }

        

    }
}