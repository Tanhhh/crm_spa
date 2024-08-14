using Erp.BackOffice.Account.Models;
using Erp.BackOffice.App_GlobalResources;
using Erp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Erp.BackOffice.Sale.Models
{
    public class ProductInvoiceViewModel
    {
        public ProductInvoiceViewModel()
        {
            Discount = 0;
            TaxFee = 0;
            TotalAmount = 0;
            DiscountAmount = 0;
        }

        public int Id { get; set; }

        [Display(Name = "IsDeleted")]
        public bool? IsDeleted { get; set; }

        [Display(Name = "CreatedUser", ResourceType = typeof(Wording))]
        public int? CreatedUserId { get; set; }
        [Display(Name = "CreatedUserName", ResourceType = typeof(Wording))]
        public string CreatedUserName { get; set; }

        [Display(Name = "CreatedDate", ResourceType = typeof(Wording))]
        public DateTime? CreatedDate { get; set; }

        [Display(Name = "ModifiedUser", ResourceType = typeof(Wording))]
        public int? ModifiedUserId { get; set; }
        public string ModifiedUserName { get; set; }

        [Display(Name = "ModifiedDate", ResourceType = typeof(Wording))]
        public DateTime? ModifiedDate { get; set; }

        [Display(Name = "Code", ResourceType = typeof(Wording))]
        public string Code { get; set; }

        [Display(Name = "TotalAmount", ResourceType = typeof(Wording))]
        public decimal TotalAmount { get; set; }

        [Display(Name = "TaxFee", ResourceType = typeof(Wording))]
        public double? TaxFee { get; set; }

        [Display(Name = "TongTienSauVAT", ResourceType = typeof(Wording))]
        public decimal TongTienSauVAT { get; set; }

        [Display(Name = "Discount", ResourceType = typeof(Wording))]
        public double? Discount { get; set; }


        [Display(Name = "CustomerNameF2", ResourceType = typeof(Wording))]
        public int? CustomerId { get; set; }

        [Display(Name = "ShipName", ResourceType = typeof(Wording))]
        public string ShipName { get; set; }
        [Display(Name = "ShipPhone", ResourceType = typeof(Wording))]
        public string ShipPhone { get; set; }

        [Display(Name = "SoNha_TenDuong", ResourceType = typeof(Wording))]
        public string ShipAddress { get; set; }

        [Display(Name = "WardName", ResourceType = typeof(Wording))]
        public string ShipWardId { get; set; }

        [Display(Name = "DistrictName", ResourceType = typeof(Wording))]
        public string ShipDistrictId { get; set; }

        [Display(Name = "CityName", ResourceType = typeof(Wording))]
        public string ShipCityId { get; set; }

        [Display(Name = "Status", ResourceType = typeof(Wording))]
        public string Status { get; set; }

        [Display(Name = "Note", ResourceType = typeof(Wording))]
        public string Note { get; set; }

        [Display(Name = "PaymentNow", ResourceType = typeof(Wording))]
        public bool IsArchive { get; set; }
        [Display(Name = "BranchName", ResourceType = typeof(Wording))]
        public int? BranchId { get; set; }

        [Display(Name = "PaymentMethod", ResourceType = typeof(Wording))]
        public string PaymentMethod { get; set; }

        [Display(Name = "PaidAmount", ResourceType = typeof(Wording))]
        public decimal? PaidAmount { get; set; }

        [Display(Name = "RemainingAmount", ResourceType = typeof(Wording))]
        public decimal? RemainingAmount { get; set; }

        [Display(Name = "LyDoHuyChungTu", ResourceType = typeof(Wording))]
        public string CancelReason { get; set; }

        [Display(Name = "BarCode", ResourceType = typeof(Wording))]
        public string BarCode { get; set; }

        [Display(Name = "CustomerCode", ResourceType = typeof(Wording))]
        public string CustomerCode { get; set; }
        [Display(Name = "CustomerPhone", ResourceType = typeof(Wording))]
        public string CustomerPhone { get; set; }

        [Display(Name = "CustomerName", ResourceType = typeof(Wording))]
        public string CustomerName { get; set; }

        [Display(Name = "WardName", ResourceType = typeof(Wording))]
        public string ShipWardName { get; set; }

        [Display(Name = "DistrictName", ResourceType = typeof(Wording))]
        public string ShipDistrictName { get; set; }

        [Display(Name = "CityName", ResourceType = typeof(Wording))]
        public string ShipCityName { get; set; }

        [Display(Name = "BranchName", ResourceType = typeof(Wording))]
        public string BranchName { get; set; }

        [Display(Name = "ProductOutboundCode", ResourceType = typeof(Wording))]
        public string ProductOutboundCode { get; set; }

        public int? ProductOutboundId { get; set; }

        public bool? IsReturn { get; set; }

        public List<ProductInvoiceDetailViewModel> InvoiceList { get; set; }
        public List<ProductInvoiceDetailViewModel> GroupProduct { get; set; }
        public List<Membership_parentViewModel> Membership_parentList { get; set; }
        public ReceiptViewModel ReceiptViewModel { get; set; }

        [Display(Name = "NextPaymentDate", ResourceType = typeof(Wording))]
        public DateTime? NextPaymentDate { get; set; }

        [Display(Name = "NextPaymentDate", ResourceType = typeof(Wording))]
        public DateTime? NextPaymentDate_Temp { get; set; }

        [Display(Name = "NextPaymentDatehoapd", ResourceType = typeof(Wording))]
        public DateTime? NextPaymentDate_Temphoapd { get; set; }
        

        [Display(Name = "CodeInvoiceRed", ResourceType = typeof(Wording))]
        public string CodeInvoiceRed { get; set; }
        [Display(Name = "DiscountAmount", ResourceType = typeof(Wording))]
        public decimal? DiscountAmount { get; set; }

        [Display(Name = "StaffCreateName", ResourceType = typeof(Wording))]
        public string StaffCreateName { get; set; }

        public bool? AllowEdit { get; set; }
        public ProductOutboundViewModel ProductOutboundViewModel { get; set; }
        public List<TransactionLiabilitiesViewModel> ListTransactionLiabilities { get; set; }
        public List<ProcessPaymentViewModel> ListProcessPayment { get; set; }
        public int? QuantityCodeSaleReturns { get; set; }

        public int? Month { get; set; }
        public int? Year { get; set; }
        public int? Day { get; set; }
        public int? WeekOfYear { get; set; }
        public string UserTypeCode { get; set; }

        public string strCreatedDate { get; set; }

        public string DistrictId { get; set; }
        public string CityId { get; set; }
        public string ProvinceName { get; set; }
        public string DistrictName { get; set; }

        [Display(Name = "IsBonusSales", ResourceType = typeof(Wording))]
        public bool? IsBonusSales { get; set; }

        public int? SaleChanel { get; set; } //kênh bán

        public string Type { get; set; }
        public decimal? DoanhThu { get; set; }
        public int? ManagerStaffId { get; set; }
        [Display(Name = "ManagerStaff", ResourceType = typeof(Wording))]
        public string ManagerStaffName { get; set; }
        [Display(Name = "ManagerStaff", ResourceType = typeof(Wording))]
        public string ManagerUserName { get; set; }
        public CommissionCusViewModel commission { get; set; }
        public List<LogPromotionViewModel> LogCommission { get; set; }
        public int? is_TANG { get; set; }
        [Range(0, 100, ErrorMessageResourceName = "RequiredValid", ErrorMessageResourceType = typeof(Error))]
        [Display(Name = "Discount_VIP", ResourceType = typeof(Wording))]
        public Decimal? Discount_VIP { get; set; }
        [Range(0, 100, ErrorMessageResourceName = "RequiredValid", ErrorMessageResourceType = typeof(Error))]
        [Display(Name = "Discount_KM", ResourceType = typeof(Wording))]
        public Decimal? Discount_KM { get; set; }
        [Range(0, 100, ErrorMessageResourceName = "RequiredValid", ErrorMessageResourceType = typeof(Error))]
        [Display(Name = "Discount_DB", ResourceType = typeof(Wording))]
        public Decimal? Discount_DB { get; set; }
        [Display(Name = "Total_Discount", ResourceType = typeof(Wording))]
        public int? Total_Discount { get; set; }
        [Range(0, 100, ErrorMessageResourceName = "RequiredValid", ErrorMessageResourceType = typeof(Error))]
        [Display(Name = "PlusPoint", ResourceType = typeof(Wording))]
        public int? PlusPoint { get; set; }
        [Display(Name = "CountForBrand", ResourceType = typeof(Wording))]
        public string CountForBrand { get; set; }
        public int? NAM { get; set; }
        public string Phone { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public decimal? tienmuahang { get; set; }

        public decimal? tientralai { get; set; }
        public decimal? tongmua { get; set; }

        public string TenHang { get; set; }
        public string loai { get; set; }
        public int IdHang { get; set; }
        public int Idxephangcu { get; set; }
        public decimal? tiendathu { get; set; }
        public decimal? tienconno { get; set; }
        public decimal? TotalAmountMoneyMove { get; set; }
        public string NoteMoneyMove { get; set; }
        [Display(Name = "ProductInvoiceOld", ResourceType = typeof(Wording))]
        public int? ProductInvoiceOldId { get; set; }
        [Display(Name = "ProductInvoiceOldCode", ResourceType = typeof(Wording))]
        public string ProductInvoiceOldCode { get; set; }
        public decimal? TotalDebit { get; set; }
        public decimal? TotalCredit { get; set; }
        public decimal? TongConNo { get; set; }
        public int? TYLE_THANHCONG { get; set; }
        public int? TYLE_THANHCONG_REVIEW { get; set; }
        public string GHI_CHU { get; set; }
        public int KH_BANHANG_CTIET_ID { get; set; }
        public int? is_checked { get;set;}
        public int? coMBS { get; set; }
        public string UserTypeName { get; set; }
        public int? UserTypeId { get; set; }
        public string GDDauTienThanhToanHet { get; set; }
        public string GDNgayDauTienThanhToanHet { get; set; }

        public string SPHangHoa { get; set; }
        public string SPDichvu { get; set; }
        public string Hangduoctang { get; set; }

        public DateTime? VoucherDate { get; set; }
        public int? CommissionCusId { get; set; }
        [Display(Name = "TyleHuong", ResourceType = typeof(Wording))]
        public double? TyleHuong { get; set; }

        public int SaleReturnId { get; set; }
        public string SaleReturnCode { get; set; }
        public decimal? SaleReturnTotalAmount { get; set; }

        public string Address { get; set; } 
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string NhomNVKD { get; set; }


        public DateTime? NgayCSGN { get; set; }
        public DateTime? NgayGDGN { get; set; }
        public DateTime? NgayGDDT { get; set; }
        public string ProductName { get; set; }
        public int? SoPhieuCon { get; set; }


        public bool? IS_ONLINE { get; set; }
        public int? PHISHIP { get; set; }

        public int? tuoi { get; set; }
        public decimal total { get; set; }
        public decimal tienthu { get; set; }
        public decimal tienno { get; set; }

        public decimal? Tiencuadonhangtra { get; set; }
        //public string tenhang { get; set; }
        //public DateTime? nam { get; set; }
    }
}