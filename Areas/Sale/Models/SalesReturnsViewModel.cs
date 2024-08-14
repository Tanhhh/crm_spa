using Erp.BackOffice.App_GlobalResources;
using Erp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Erp.BackOffice.Account.Models;
namespace Erp.BackOffice.Sale.Models
{
    public class SalesReturnsViewModel
    {
        public int Id { get; set; }

        public bool? IsDeleted { get; set; }

        public DateTime? CreatedDate { get; set; }

        public int? CreatedUserId { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public int? ModifiedUserId { get; set; }

        public int? AssignedUserId { get; set; }
        [Display(Name = "Code", ResourceType = typeof(Wording))]
        public string Code { get; set; }
        [Display(Name = "Customer", ResourceType = typeof(Wording))]
        public int? CustomerId { get; set; }
        [Display(Name = "TotalAmount", ResourceType = typeof(Wording))]
        public decimal? TotalAmount { get; set; }
        [Display(Name = "VAT", ResourceType = typeof(Wording))]
        public double? TaxFee { get; set; }
        [Display(Name = "Status", ResourceType = typeof(Wording))]
        public string Status { get; set; }
        [Display(Name = "Note", ResourceType = typeof(Wording))]
        public string Note { get; set; }

        public int? BranchId { get; set; }
        [Display(Name = "PaymentMethod", ResourceType = typeof(Wording))]
        public string PaymentMethod { get; set; }
        [Display(Name = "PaidAmount", ResourceType = typeof(Wording))]
        public decimal? PaidAmount { get; set; }
        [Display(Name = "ProductInvoiceOld", ResourceType = typeof(Wording))]
        public int? ProductInvoiceOldId { get; set; }
        [Display(Name = "ProductInvoiceNew", ResourceType = typeof(Wording))]
        public int? ProductInvoiceNewId { get; set; }
        [Display(Name = "ProductInbound", ResourceType = typeof(Wording))]
        public int? ProductInboundId { get; set; }
        [Display(Name = "RemainingAmount", ResourceType = typeof(Wording))]
        public decimal? RemainingAmount { get; set; }
        [Display(Name = "CustomerCode", ResourceType = typeof(Wording))]
        public string CustomerCode { get; set; }
        [Display(Name = "CustomerName", ResourceType = typeof(Wording))]
        public string CustomerName { get; set; }
        [Display(Name = "Address", ResourceType = typeof(Wording))]
        public string Address { get; set; }
        [Display(Name = "BranchName", ResourceType = typeof(Wording))]
        public string BranchName { get; set; }
        [Display(Name = "CreatedUserName", ResourceType = typeof(Wording))]
        public string CreatedUserName { get; set; }
        public string ProductInvoiceOldCode { get; set; }
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Error))]
        [Display(Name = "WarehouseDestination", ResourceType = typeof(Wording))]
        public Nullable<int> WarehouseDestinationId { get; set; }
        public List<SalesReturnsDetailViewModel> DetailList { get; set; }
        public PaymentViewModel paymentViewModel { get; set; }
        public ProductInboundViewModel ProductInboundViewModel { get; set; }
        public List<ProductInvoiceDetailViewModel> InvoiceList { get; set; }
        public List<SalesReturnsDetailViewModel> GroupProduct { get; set; }
        public bool IsArchive { get; set; }
        public List<TransactionLiabilitiesViewModel> ListTransactionLiabilities { get; set; }
        public ProductInvoiceViewModel InvoiceOld { get; set; }
        public ProductInvoiceViewModel InvoiceNew { get; set; }
        [Display(Name = "AmountPayment", ResourceType = typeof(Wording))]
        public decimal? AmountPayment { get; set; }
        [Display(Name = "AmountReceipt", ResourceType = typeof(Wording))]
        public decimal? AmountReceipt { get; set; }
        public bool? AllowEdit { get; set; }

        public int? ManagerStaffId { get; set; }
        [Display(Name = "ManagerStaff", ResourceType = typeof(Wording))]
        public string ManagerStaffName { get; set; }
        [Display(Name = "ManagerStaff", ResourceType = typeof(Wording))]
        public string ManagerUserName { get; set; }
    }
}