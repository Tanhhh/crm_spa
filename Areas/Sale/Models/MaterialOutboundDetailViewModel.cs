using Erp.BackOffice.App_GlobalResources;
using Erp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Erp.BackOffice.Sale.Models
{
    public class MaterialOutboundDetailViewModel
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

        //[Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Error))]
        //[StringLength(100, ErrorMessageResourceType = typeof(Error), ErrorMessageResourceName = "StringError", ErrorMessage = null)]

        [Display(Name = "MaterialOutboundId", ResourceType = typeof(Wording))]
        public Nullable<int> MaterialOutboundId { get; set; }
        [Display(Name = "MaterialId", ResourceType = typeof(Wording))]
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
        [Display(Name = "Discount", ResourceType = typeof(Wording))]
        public int? DisCount { get; set; }
        [Display(Name = "DisCountAmount", ResourceType = typeof(Wording))]
        public int? DisCountAmount { get; set; }
        [Display(Name = "LoCode", ResourceType = typeof(Wording))]
        public string LoCode { get; set; }
        [Display(Name = "ExpiryDateItem", ResourceType = typeof(Wording))]
        public DateTime? ExpiryDate { get; set; }
        public int OrderNo { get; set; }
        public int? WarehouseDestinationId { get; set; }
        public int? WarehouseSourceId { get; set; }
        public string MaterialType { get; set; }
        public double? QuantityInInventory { get; set; }

        public List<WarehouseLocationItemViewModel> ListWarehouseLocationItemViewModel { get; set; }
    }
}