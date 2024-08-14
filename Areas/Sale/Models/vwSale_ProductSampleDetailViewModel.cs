using Erp.BackOffice.App_GlobalResources;
using Erp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Erp.BackOffice.Sale.Models
{
    public class vwSale_ProductSampleDetailViewModel
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
        [StringLength(100, ErrorMessageResourceType = typeof(Error), ErrorMessageResourceName = "StringError", ErrorMessage = null)]
        [Display(Name = "Name", ResourceType = typeof(Wording))]
        public string Name { get; set; }

        [Display(Name = "CustomerId", ResourceType = typeof(Wording))]
        public Nullable<int> CustomerId { get; set; }
        [Display(Name = "Status", ResourceType = typeof(Wording))]
        public string Status { get; set; }
        [Display(Name = "Note", ResourceType = typeof(Wording))]
        public string Note { get; set; }
        [Display(Name = "CustomerName", ResourceType = typeof(Wording))]
        public string CustomerName { get; set; }
        [Display(Name = "CustomerCode", ResourceType = typeof(Wording))]
        public string CustomerCode { get; set; }
        [Display(Name = "ProductSampleId", ResourceType = typeof(Wording))]
        public Nullable<int> ProductSampleId { get; set; }
        [Display(Name = "ProductSampleCode", ResourceType = typeof(Wording))]
        public string ProductSampleCode { get; set; }
        [Display(Name = "ProductId", ResourceType = typeof(Wording))]
        public Nullable<int> ProductId { get; set; }
        [Display(Name = "ProductName", ResourceType = typeof(Wording))]
        public string ProductName { get; set; }
        [Display(Name = "ProductCode", ResourceType = typeof(Wording))]
        public string ProductCode { get; set; }
        [Display(Name = "CustomerImage", ResourceType = typeof(Wording))]
        public string CustomerImage { get; set; }
        [Display(Name = "ProductImage", ResourceType = typeof(Wording))]
        public string ProductImage { get; set; }
        [Display(Name = "ProductLinkId", ResourceType = typeof(Wording))]
        public Nullable<int> ProductLinkId { get; set; }
        public List<ProductSampleDetailViewModel> DetailList { get; set; }

    }
}