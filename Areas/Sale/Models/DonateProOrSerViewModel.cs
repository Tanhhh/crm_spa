using Erp.BackOffice.App_GlobalResources;
using Erp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Erp.BackOffice.Sale.Models
{
    public class DonateProOrSerViewModel
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

        public Nullable<int> ProductId { get; set; }

        //[Display(Name = "TargetId", ResourceType = typeof(Wording))]
        public Nullable<int> TargetId { get; set; }
        //[Display(Name = "TargetModule", ResourceType = typeof(Wording))]
        public string TargetModule { get; set; }
        //[Display(Name = "Quantity", ResourceType = typeof(Wording))]
        public Nullable<int> Quantity { get; set; }
        //[Display(Name = "ExpriryMonth", ResourceType = typeof(Wording))]
        public Nullable<int> ExpriryMonth { get; set; }
        //[Display(Name = "TotalQuantity", ResourceType = typeof(Wording))]
        public Nullable<int> TotalQuantity { get; set; }
        //[Display(Name = "RemainQuantity", ResourceType = typeof(Wording))]
        public Nullable<int> RemainQuantity { get; set; }

        public int? OrderNo { get; set; }
        //vw
        public string CategoryCode { get; set; }
        public string ProductName { get; set; }
        public string ProductCode { get; set; }
        public decimal? Price { get; set; }
        public string ProductType { get; set; }
        public int? ParentOrderNo { get; set; }
    }
}