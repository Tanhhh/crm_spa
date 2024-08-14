using Erp.BackOffice.App_GlobalResources;
using Erp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Erp.BackOffice.Sale.Models
{
    public class ProductSampleDetailViewModel
    {
        public ProductSampleDetailViewModel()
        {

        }
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
        [Display(Name = "CustomerId", ResourceType = typeof(Wording))]
        public Nullable<int> CustomerId { get; set; }
        [Display(Name = "Status", ResourceType = typeof(Wording))]
        public string Status { get; set; }
        [Display(Name = "ProductSampleId", ResourceType = typeof(Wording))]
        public Nullable<int> ProductSampleId { get; set; }
        [Display(Name = "Note", ResourceType = typeof(Wording))]
        public string Note { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string CustomerCode { get; set; }
        public string ProductSampleCode { get; set; }
        public Nullable<int> ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductCode { get; set; }
        public string CustomerImage { get; set; }
        public string ProductImage { get; set; }
        public Nullable<int> ProductLinkId { get; set; }

        public List<ProductInvoiceDetailViewModel> InvoiceList { get; set; }
    }
}