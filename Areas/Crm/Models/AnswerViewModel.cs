using Erp.BackOffice.App_GlobalResources;
using Erp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Erp.BackOffice.Crm.Models
{
    public class AnswerViewModel
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

        [Display(Name = "QuestionId", ResourceType = typeof(Wording))]
        public Nullable<int> QuestionId { get; set; }
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Error))]
        [Display(Name = "Content", ResourceType = typeof(Wording))]
        public string Content { get; set; }
        [Display(Name = "IsActivated", ResourceType = typeof(Wording))]
        public Nullable<bool> IsActivated { get; set; }

        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Error))]
        [Display(Name = "OrderNo", ResourceType = typeof(Wording))]
        public Nullable<int> OrderNo { get; set; }
    }
}