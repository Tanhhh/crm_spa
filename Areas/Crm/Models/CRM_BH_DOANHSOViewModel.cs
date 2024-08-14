using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Erp.BackOffice.Crm.Models;
using Erp.BackOffice.Areas.Crm.Models;
using Erp.BackOffice.Account.Models;
using Erp.BackOffice.Sale.Models;

namespace Erp.BackOffice.Crm.Models
{
    public class CRM_BH_DOANHSOViewModel
    {
        public int KH_BANHANG_DOANHSO_ID { get; set; }
        public int? ID_ANNAYAKE { get; set; }
        public int? ID_DICHVU { get; set; }
        public int? ID_LEONORGREYL { get; set; }
        public int? ID_ORLANEPARIS { get; set; }
        public int? ID_CONGNGHECAO { get; set; }
        public int? BranchId { get; set; }
        public int? THANG { get; set; }
        public int? NAM { get; set; }
        public int? NGUOILAP_ID { get; set; }
        public string CountForBrand { get; set; }
        public decimal? TARGET_BRAND { get; set; }
        public decimal? TARGET_DALAP { get; set; }
        public decimal? LEONORGREYL { get; set; }
        public decimal? DICHVU { get; set; }
        public decimal? CONGNGHECAO { get; set; }
        public decimal? ORLANEPARIS { get; set; }
        public string GHI_CHU { get; set; }
        public List<ProductInvoiceViewModel> ProductInvoiceList { get; set; }
        public List<ProductInvoiceViewModel> CRM_BH_DOANHSO_CTIETList { get; set; }
        public List<CRM_BH_DOANHSO_CTViewModel> CRM_BH_DOANHSO_CTList { get; set; }
        public List<CustomerViewModel> CustomerList { get; set; }
        public bool? IsDeleted { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public int? CreatedUserId { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public int? ModifiedUserId { get; set; }
        public string FullName { get; set; }
        public List<vwCRM_BH_DOANHSO_CTViewModel> ListDetail { set; get; }
        public int TYLE_THANHCONG { get; set; }
        public decimal? TONG_TIEN { get; set; }
        public decimal? DA_TRA { set; get; }
        public decimal? CON_NO { get; set; }

        public int TYLE_THANHCONG_REVIEW { get; set; }
        public decimal KH_BANHANG_DOANHSO_CTIET_ID { get; set; }
        public string BranchName { get; set; }
        public string UserName { get; set; }
        public string CompanyName { get; set; }
        public string Code { get; set; }
        public string Phone { get; set; }
        public string month { get; set; }
    }
}