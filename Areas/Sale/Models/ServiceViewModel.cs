using Erp.BackOffice.App_GlobalResources;
using Erp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Erp.BackOffice.Sale.Models
{
    public class ServiceViewModel
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

        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Error))]
        [StringLength(300, ErrorMessageResourceType = typeof(Error), ErrorMessageResourceName = "StringError", ErrorMessage = null)]
        [Display(Name = "ServiceName", ResourceType = typeof(Wording))]
        public string Name { get; set; }

        [Display(Name = "ServiceCode", ResourceType = typeof(Wording))]
        public string Code { get; set; }
        [Display(Name = "Description", ResourceType = typeof(Wording))]
        public string Description { get; set; }
        [Display(Name = "Unit", ResourceType = typeof(Wording))]
        public string Unit { get; set; }
        [Display(Name = "Price", ResourceType = typeof(Wording))]
        public decimal? PriceOutbound { get; set; }

        [Display(Name = "ServiceCategory", ResourceType = typeof(Wording))]
        public string CategoryCode { get; set; }
        [Display(Name = "Barcode", ResourceType = typeof(Wording))]
        public string Barcode { get; set; }
        //[Display(Name = "Image_Name", ResourceType = typeof(Wording))]
        public string Image_Name { get; set; }
        [Display(Name = "IsCombo", ResourceType = typeof(Wording))]
        public bool? IsCombo { get; set; }
        [Display(Name = "TimeForService", ResourceType = typeof(Wording))]
        public int TimeForService { get; set; }
        //public List<ObjectAttributeValueViewModel> AttributeValueList { get; set; }
        //public List<ServiceComboViewModel> DetailList { get; set; }
        //public List<ServiceReminderGroupViewModel> ReminderList { get; set; }
        //public int? DiscountStaff { get; set; }
        //public bool? IsMoneyDiscount { get; set; }
        public List<ServiceDetailViewModel> DetailServiceList { get; set; }
        public List<ServiceStepsViewModel> DetailList { get; set; }
        [Display(Name = "Type", ResourceType = typeof(Wording))]
        public string Type { get; set; }

        [Display(Name = "ServiceProductGroup", ResourceType = typeof(Wording))]
        public string ProductGroup { get; set; }
        [Display(Name = "ServiceLinkId", ResourceType = typeof(Wording))]
        public int? ProductLinkId { get; set; }
        [Display(Name = "ServiceLinkName", ResourceType = typeof(Wording))]
        public string ProductLinkName { get; set; }
        [Display(Name = "ServiceLinkCode", ResourceType = typeof(Wording))]
        public string ProductLinkCode { get; set; }
        [Display(Name = "QuantityDayUsed", ResourceType = typeof(Wording))]
        public int? QuantityDayUsed { get; set; }
        [Display(Name = "QuantityDayNotify", ResourceType = typeof(Wording))]
        public int? QuantityDayNotify { get; set; }
        public int? ProductId { get; set; }
        public int Quantity { get; set; }
        public string EquimentGroup { get; set; }
        [Display(Name ="Origin",ResourceType =typeof(Wording))]
        public string Origin { get; set; }
        [Display(Name = "MinQuantityforSevice", ResourceType = typeof(Wording))]
        public int MinQuantityforSevice { get; set; }

        [Display(Name = "Khóa")]
        public bool? NotUse { get; set; }

        [Display(Name = "Cấp dịch vụ")]
        public int? NHOMSANPHAM_ID { get; set; }

        public string List_Image { get; set; }
        public bool? IS_ALOW_BAN_AM { get; set; }

        [Display(Name = "Url_slug")]
        public string URL_SLUG { get; set; }
        public string HDSD { get; set; }
        public string THANH_PHAN { get; set; }
        public string Description_brief { get; set; }
        public string NHOMSANPHAM_ID_LST { get; set; }
        [Display(Name = "Hiển thị")]
        public bool? is_display { get; set; }
        public string CONGDUNG { get; set; }

        [Display(Name = "Video")]
        public string Video { get; set; }
    }
}