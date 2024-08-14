using Erp.BackOffice.App_GlobalResources;
using Erp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Erp.BackOffice.Sale.Models
{
    public class LogPromotionViewModel
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


        public int? AssignedUserId { get; set; }
        public string AssignedUserName { get; set; }


        public string Name { get; set; }

        public string ProductInvoiceCode { get; set; }

        public int ProductId { get; set; }
      
        public string CommissionCusCode { get; set; }

        public Nullable<int> TargetID { get; set; }

        public string TargetModule { get; set; }

        public string Type { get; set; }

        public Nullable<int> CustomerId { get; set; }

        public Nullable<int> BranchId { get; set; }

        public string BranchName { get; set; }

        public string Code { get; set; }

        public decimal? CommissionValue { get; set; }

        public Nullable<bool> IsMoney { get; set; }

        public Nullable<int> GiftProductId { get; set; }

        public Nullable<int> DonateProOrSerId { get; set; }
        public Nullable<int> ProductQuantity { get; set; }
        public string ProductSymbolQuantity { get; set; }
        public string StartSymbol { get; set; }
        public string EndSymbol { get; set; }
        public string MaChungTuLienQuan { get; set; }
        public decimal? EndAmount { get; set; }
        public decimal? StartAmount { get; set; }
        public Nullable<System.DateTime> ExpiryDate { get; set; }
        public string Category { get; set; }
        public string Note { get; set; }
        //vw
        public string ProductName { get; set; }
        public string GiftProductName { get; set; }
        public string GiftProductOrigin { get; set; }
        public string ProductOrigin { get; set; }
        //model
        public bool? CheckSave { get; set; }
        public int count { get; set; }
        public int? CommissionCusId { get; set; }
        public decimal? TyleHuong { get; set; }

        public decimal? TotalAmount { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public DateTime? BuyDate { get; set; }

        public string ManagerStaffName { get; set; }
        public string NhomNVKD { get; set; }

    }
}