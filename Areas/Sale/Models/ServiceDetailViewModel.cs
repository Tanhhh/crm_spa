using Erp.BackOffice.App_GlobalResources;
using Erp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Erp.BackOffice.Sale.Models
{
    public class ServiceDetailViewModel
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



        [Display(Name = "ProductId", ResourceType = typeof(Wording))]
        public Nullable<int> ProductId { get; set; }

        //  [Display(Name = "ServiceId", ResourceType = typeof(Wording))]
        public Nullable<int> ServiceId { get; set; }
     //   [Display(Name = "Quantity", ResourceType = typeof(Wording))]
        public Nullable<int> Quantity { get; set; }
        public string ProductName { get; set; }
        public string ProductCode { get; set; }
        public List<ProductDetailViewModel> ProductDetailList { get; set; }
    }
}