using Erp.BackOffice.App_GlobalResources;
using Erp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Erp.BackOffice.Sale.Models
{
    public class MaterialInboundDetailViewModel
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
        [Display(Name = "MaterialInboundId", ResourceType = typeof(Wording))]
        public Nullable<int> MaterialInboundId { get; set; }
        [Display(Name = "MaterialName", ResourceType = typeof(Wording))]
        public Nullable<int> MaterialId { get; set; }
        [Display(Name = "MaterialName", ResourceType = typeof(Wording))]
        public string MaterialName { get; set; }
        public string MaterialCode { get; set; }

        [Display(Name = "Quantity", ResourceType = typeof(Wording))]
        public Nullable<double> Quantity { get; set; }
        [Display(Name = "Price", ResourceType = typeof(Wording))]
        public decimal? Price { get; set; }
        [Display(Name = "Unit", ResourceType = typeof(Wording))]
        public string Unit { get; set; }
        public string MaterialBarCode { get; set; }
        public string MaterialType { get; set; }
        public int OrderNo { get; set; }
        [Display(Name = "LoCode", ResourceType = typeof(Wording))]
        public string LoCode { get; set; }
        [Display(Name = "ExpiryDateItem", ResourceType = typeof(Wording))]
        public Nullable<System.DateTime> ExpiryDate { get; set; }
        public int QuantityUsed { get; set; }
        //nhập kho từ phiếu xuất.
        [Display(Name = "Quantity", ResourceType = typeof(Wording))]
        public double? QuantityTest { get; set; }


        public Nullable<int> MaterialMinInventory { get; set; }
    }
}