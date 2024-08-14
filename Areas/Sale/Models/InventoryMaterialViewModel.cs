using Erp.BackOffice.App_GlobalResources;
using Erp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Erp.BackOffice.Sale.Models
{
    public class InventoryMaterialViewModel
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

        public int MaterialId { get; set; }

        public string ProductGroup { get; set; }

        public string CategoryCode { get; set; }

        public string MaterialName { get; set; }

        public string MaterialCode { get; set; }

        public string MaterialBarcode { get; set; }

        public string MaterialUnit { get; set; }

        public int? MaterialMinInventory { get; set; }

        public int? WarehouseId { get; set; }

        public string WarehouseName { get; set; }

        public int? BranchId { get; set; }

        public string BranchName { get; set; }

        public double Quantity { get; set; }

        public string LoCode { get; set; }

        public DateTime? ExpiryDate { get; set; }

        public string Image_Name { get; set; }

        public int? CBTK { get; set; }

        public bool? IsSale { get; set; }

        public string MaterialManufacturer { get; set; }

        public decimal? MaterialPriceInbound { get; set; }

        public decimal? MaterialPriceOutbound { get; set; }

        public int? day { get; set; }
        public int? month { get; set; }
        public int? year { get; set; }

        public string strExpiryDate { get; set; }
        public string DistrictId { get; set; }
        public string CityId { get; set; }
        public string ProvinceName { get; set; }
        public string DistrictName { get; set; }
        public string Manufacturer { get; set; }
    }
}