using Erp.BackOffice.App_GlobalResources;
using Erp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Erp.BackOffice.Sale.Models
{
    public class ProductInvoiceDetailViewModel
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

        [Display(Name = "ProductOrderId", ResourceType = typeof(Wording))]
        public Nullable<int> ProductInvoiceId { get; set; }

        [Display(Name = "ProductId", ResourceType = typeof(Wording))]
        public Nullable<int> ProductId { get; set; }

        [Display(Name = "ProductId", ResourceType = typeof(Wording))]
        public string ProductName { get; set; }

        [Display(Name = "ProductCode", ResourceType = typeof(Wording))]
        public string ProductCode { get; set; }

        [Display(Name = "Price", ResourceType = typeof(Wording))]
        public decimal? Price { get; set; }
        [Display(Name = "Price", ResourceType = typeof(Wording))]
        public decimal? PriceTest { get; set; }
        [Display(Name = "Quantity", ResourceType = typeof(Wording))]
        public Nullable<int> Quantity { get; set; }

        public int? QuantityInInventory { get; set; }

        public string Unit { get; set; }

        [Display(Name = "CategoryCode", ResourceType = typeof(Wording))]
        public string CategoryCode { get; set; }
        public string ProductType { get; set; }

        [Display(Name = "ProductCode", ResourceType = typeof(Wording))]
        public string ProductInvoiceCode { get; set; }
        [Display(Name = "ProductGroup", ResourceType = typeof(Wording))]
        public string ProductGroup { get; set; }
        public string ProductGroupName { get; set; }
        [Display(Name = "LoCode", ResourceType = typeof(Wording))]
        public string LoCode { get; set; }
        [Display(Name = "ExpiryDateItem", ResourceType = typeof(Wording))]
        public DateTime? ExpiryDate { get; set; }
        public string strExpiryDate { get; set; }
        public bool? IsCombo { get; set; }
        public System.DateTime ProductInvoiceDate { get; set; }

        public decimal? Amount { get; set; }
        [Display(Name = "Description", ResourceType = typeof(Wording))]
        public string Description { get; set; }

        public int BranchId { get; set; }
        public string BranchName { get; set; }

        public string Type { get; set; }
        [Display(Name = "Phone", ResourceType = typeof(Wording))]
        public string CustomerPhone { get; set; }
        [Display(Name = "Customer", ResourceType = typeof(Wording))]
        public string CustomerName { get; set; }
        public string CustomerCode { get; set; }
        public int? CustomerId { get; set; }
        public Nullable<int> Day { get; set; }
        public Nullable<int> Month { get; set; }
        public Nullable<int> Year { get; set; }
        public string DistrictId { get; set; }
        public string CityId { get; set; }
        public string ProvinceName { get; set; }
        public string DistrictName { get; set; }
        public string ProductImage { get; set; }
        public decimal? Discount { get; set; }
        public decimal? DiscountAmount { get; set; }
        [Display(Name = "Origin", ResourceType = typeof(Wording))]
        public string Origin { get; set; }
        public string Name { get; set; }
        [Display(Name = "Status", ResourceType = typeof(Wording))]
        public string Status { get; set; }
        public int? ProductOutboundId { get; set; }
        [Display(Name = "ProductOutboundCode", ResourceType = typeof(Wording))]
        public string ProductOutboundCode { get; set; }
        public bool IsArchive { get; set; }
        public string Code { get; set; }
        public string Note { get; set; }
        public int? is_TANG { get; set; }

        public string CountForBrand { get; set; }

        public int? Discount_VIP { get; set; }
        public int? Discount_KM { get; set; }
        public int? Discount_DB { get; set; }
        public decimal? Tiensaugiam { get; set; }
        public string NhomNVKD { get; set; }
        public string ManagerStaffName { get; set; }
        public int ManagerStaffId { get; set; }

        public decimal? Origin_Price { get; set; }
    }
}