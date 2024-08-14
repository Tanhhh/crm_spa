using Erp.BackOffice.App_GlobalResources;
using Erp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Erp.BackOffice.Crm.Models
{
    public class CRM_KH_BANHANG_CTIETViewModel
    {
        public int KH_BANHANG_CTIET_ID { get; set; }
        public int? BranchId { get; set; }
        public int? KH_BANHANG_ID { get; set; }

        public int? THANG { get; set; }
        public int? NAM { get; set; }
        public int? KHACHHANG_ID { get; set; }
        public string NOIDUNG { get; set; }

        public int? TYLE_THANHCONG { get; set; }

        public int? TYLE_THANHCONG_REVIEW { get; set; }
        public string GHI_CHU { get; set; }

        public Nullable<bool> IsDeleted { get; set; }
        public Nullable<int> CreatedUserId { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> ModifiedUserId { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public string Phone { get; set; }
        public string CompanyName { get; set; }
        public string Code { get; set; }
        public string CRM_Sale_ProductInvoiceCode { get; set; }
        public string CountForBrand { get; set; }
        public decimal? TotalAmount { get; set; }
        public int? ProductInvoiceId { get; set; }
        public int? NGUOILAP_ID { get; set; }
        public int? is_checked { get; set; }
        //public List<CRM_KH_BANHANG_CTIETViewModel> CRM_KH_BANHANG_CTIETList { get; set; }

        public int? IsApprove { get; set; }
        public int? UserApprove { get; set; }
        public string UserApproveName { get; set; }
    }

    public class FollowDealViewModel
    {
        public string CountForBrand { get; set; }
        public int? TongDeal { get; set; }
        public decimal? TongDS { get; set; }
        public int? SLDeatbai { get; set; }
        public decimal? DoanhSoBai { get; set; }
        public int? SLTC { get; set; }
        public decimal? DoanhSoTC { get; set; }

        public double? PhanTramDSTC { get; set; }
        public double? PhanTramDSTB { get; set; }
        public double? PhanTramDealTB { get; set; }
        public double? PhanTramDealTC { get; set; }
    }
}