using Erp.BackOffice.App_GlobalResources;
using Erp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Erp.BackOffice.Account.Models;

namespace Erp.BackOffice.Sale.Models
{
    public class MaterialInboundViewModel
    {
        public int Id { get; set; }

        [Display(Name = "IsDeleted")]
        public bool? IsDeleted { get; set; }

        [Display(Name = "CreatedUser", ResourceType = typeof(Wording))]
        public int? CreatedUserId { get; set; }

        [Display(Name = "ReceiverName", ResourceType = typeof(Wording))]
        public string CreatedUserName { get; set; }

        [Display(Name = "NgayChungTu", ResourceType = typeof(Wording))]
        public DateTime? CreatedDate { get; set; }

        [Display(Name = "ModifiedUser", ResourceType = typeof(Wording))]
        public int? ModifiedUserId { get; set; }
        public string ModifiedUserName { get; set; }

        [Display(Name = "ModifiedDate", ResourceType = typeof(Wording))]
        public DateTime? ModifiedDate { get; set; }

        [Display(Name = "MaChungTu", ResourceType = typeof(Wording))]
        public string Code { get; set; }

        [Display(Name = "Note", ResourceType = typeof(Wording))]
        public string Note { get; set; }

        [Display(Name = "WarehouseSource", ResourceType = typeof(Wording))]
        public Nullable<int> WarehouseSourceId { get; set; }

        [Display(Name = "WarehouseSource", ResourceType = typeof(Wording))]
        public string WarehouseSourceName { get; set; }

        [Display(Name = "WarehouseKeeperId", ResourceType = typeof(Wording))]
        public Nullable<int> WarehouseKeeperId { get; set; }

        [Display(Name = "Type", ResourceType = typeof(Wording))]
        public string Type { get; set; }

        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Error))]
        [Display(Name = "WarehouseDestination", ResourceType = typeof(Wording))]
        public Nullable<int> WarehouseDestinationId { get; set; }

        [Display(Name = "WarehouseDestination", ResourceType = typeof(Wording))]
        public string WarehouseDestinationName { get; set; }

        [Display(Name = "PurchaseOrderCode", ResourceType = typeof(Wording))]
        public int? PurchaseOrderId { get; set; }

        public int? MaterialOutboundId { get; set; }

        [Display(Name = "PurchaseOrderCode", ResourceType = typeof(Wording))]
        public string PurchaseOrderCode { get; set; }

        [Display(Name = "TotalAmount", ResourceType = typeof(Wording))]
        public decimal? TotalAmount { get; set; }

        public bool? IsDone { get; set; }

      

        [Display(Name = "ShipCompany", ResourceType = typeof(Wording))]
        public Nullable<int> ShipSupplierId { get; set; }

        [Display(Name = "Supplier", ResourceType = typeof(Wording))]
        public Nullable<int> SupplierId { get; set; }

        [Display(Name = "Supplier", ResourceType = typeof(Wording))]
        public string SupplierName { get; set; }

        public List<MaterialInboundDetailViewModel> DetailList { get; set; }
        public PaymentViewModel PaymentViewModel { get; set; }
        public List<WarehouseLocationItemViewModel> LocationItemList { get; set; }
        [Display(Name = "NextPaymentDate", ResourceType = typeof(Wording))]
        public DateTime? NextPaymentDate { get; set; }
        [Display(Name = "PaymentNow", ResourceType = typeof(Wording))]
        public bool IsPayment { get; set; }

        //[Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Error))]
        //[Display(Name = "MaterialItemCount", ResourceType = typeof(Wording))]
        //public Nullable<int> MaterialItemCount { get; set; }
        public bool IsArchive { get; set; }
        public bool AllowEdit { get; set; }
        //[Display(Name = "CreatedStaffName", ResourceType = typeof(Wording))]
        //public int? CreatedStaffId { get; set; }
        //[Display(Name = "CreatedStaffName", ResourceType = typeof(Wording))]
        //public string CreatedStaffName { get; set; }

        [Display(Name = "BranchName", ResourceType = typeof(Wording))]
        public Nullable<int> BranchId { get; set; }

        public string DistrictId { get; set; }
        public string CityId { get; set; }
        public string ProvinceName { get; set; }
        public string DistrictName { get; set; }
        public string PhysicalInventoryId { get; set; }
    }
}