using Erp.BackOffice.Account.Models;
using Erp.BackOffice.App_GlobalResources;
using Erp.BackOffice.Sale.Models;
using Erp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Erp.BackOffice.Crm.Models
{
    public class CRM_KH_BANHANGViewModel
    {
        public int KH_BANHANG_ID { get; set; }

        public int? ID_ANNAYAKE { get; set; }
        public int? ID_DICHVU { get; set; }
        public int? ID_LEONORGREYL { get; set; }
        public int? ID_ORLANEPARIS { get; set; }
        public int? ID_CONGNGHECAO { get; set; }
        public int? BranchId { get; set; }

        public int THANG { get; set; }
        public int NAM { get; set; }
        public int? NGUOILAP_ID { get; set; }

        [Display(Name = "CountForBrand", ResourceType = typeof(Wording))]
        public string CountForBrand { get; set; }
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Error))]
        //[Display(Name = "TARGET_BRAND", ResourceType = typeof(Wording))]
        public decimal? TARGET_BRAND { get; set; }
        public decimal? LEONORGREYL { get; set; }
        public decimal? DICHVU { get; set; }
        public decimal? CONGNGHECAO { get; set; }
        public decimal? ORLANEPARIS { get; set; }

        public string GHI_CHU { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        public Nullable<int> CreatedUserId { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> ModifiedUserId { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }

        //public IEnumerable<CRM_KH_BANHANGViewModel> CRM_KH_BANHANGList { get; set; }
        public List<CRM_KH_BANHANGViewModel> CRM_KH_BANHANG_CTIETList { get; set; }
        public List<CRM_KH_BANHANG_CTIETViewModel> CRM_KH_BANHANG_CTIET { get; set; }
        public List<CRM_KH_BANHANG_CTIETViewModel> CRM_KH_BANHANGList { get; set; }
        public List<CustomerViewModel> CustomerList { get; set; }
        public List<vwKH_SAPHETSPViewModel> vwKH_SAPHETSP { get; set; }
        public List<KH_TUONGTACViewModel> KH_TUONGTACList { get; set; }
        public List<MBSAPHETHANViewModel> MBSAPHETHANList { get; set; }
        public List<KH_SapHetDVViewModel> vwKH_SAPHETDV { get; set; }
        public decimal KH_BANHANG_CTIET_ID { get; set; }

        public int KHACHHANG_ID { get; set; }
        public string NOIDUNG { get; set; }
        public int? TYLE_THANHCONG { get; set; }
        public int? TYLE_THANHCONG_REVIEW { get; set; }
        public int? CustomerId { get; set; }
        public string Name { get; set; }
        public string FullName { get; set; }

        public string CompanyName { get; set; }
        public string Code { get; set; }
        public string Phone{ get; set; }

        public int Id { get; set; }
        public int? ManagerStaffId { get; set; }
        public int? ProductInvoiceId { get; set; }
        public string ProductInvoiceCode { get; set; }
        public string KeThua { get; set; }
        public string Month { get; set; }
        public string year { get; set; }
        public int checkSPSHH { get; set; }
        public int checkLTT { get; set; }
        public int is_checked { get; set; }
        public int checkMB { get; set; }
        public int checkDVSHH { get; set; }
        public string checkTK { get; set; }
        public decimal TotalAmount { get; set; }
    }

    public class TargetViewModel
    {
        public int UserId { get; set; }
        public string LevelName { get; set; }
        public int Target { get; set; }
        public int Value { get; set; }
    }

    public class TongHopDealViewModel
    {
        public string Name { get; set; }
        public decimal? Total { get; set; }
    }

    public class TongHopDealNhanVienTheoNhanViewModel
    {
        public int? Level { get; set; }
        public int? Period { get; set; }
        public decimal? Target { get; set; }
    }
}