using Erp.BackOffice.App_GlobalResources;
using Erp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Erp.BackOffice.Sale.Models
{
    public class SalesReturnsDetailViewModel
    {
        public int Id { get; set; }

        public bool? IsDeleted { get; set; }

        public DateTime? CreatedDate { get; set; }

        public int? CreatedUserId { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public int? ModifiedUserId { get; set; }

        public int? AssignedUserId { get; set; }

        public int? SalesReturnsId { get; set; }

        public int? ProductId { get; set; }

        public int? Quantity { get; set; }

        public int? ProductInvoiceDetailId { get; set; }

        public decimal? Price { get; set; }

        public decimal? Amount { get; set; }

        public string ProductName { get; set; }

        public string ProductCode { get; set; }

        public string CategoryCode { get; set; }

        public string ProductGroup { get; set; }

        public string Manufacturer { get; set; }

        public string UnitProduct { get; set; }
        public string LoCode { get; set; }
        public Nullable<System.DateTime> ExpiryDate { get; set; }
        public string Unit { get; set; }

        public int? BranchId { get; set; }
    }
}