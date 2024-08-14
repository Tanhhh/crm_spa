using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Erp.BackOffice.Areas.Crm.Models
{
    public class vwCRM_BH_DOANHSO_CTViewModel
    {
      
        public int KH_BANHANG_DOANHSO_CTIET_ID { get; set; }
        public int? BranchId { get; set; }
        public decimal? KH_BANHANG_DOANHSO_ID { get; set; }
        public int? THANG { get; set; }
        public int? NAM { get; set; }
        public int? KHACHHANG_ID { get; set; }
        public string NOIDUNG { get; set; }
        public int? TYLE_THANHCONG { get; set; }
        public string MA_DONHANG { get; set; }
        public string NGAY_MUA { get; set; }
        public decimal? TONG_TIEN { get; set; }
        public decimal? DA_TRA { set; get; }
        public decimal? CON_NO { get; set; }
        public int? TYLE_THANHCONG_REVIEW { get; set; }
        public string GHI_CHU { get; set; }
        public bool? IsDeleted { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public int CreatedUserId { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public int? ModifiedUserId { get; set; }
        public string BranchName { get; set; }
        public string CustomerName { get; set; }
        public string CustomerCode { get; set; }
        public string Phone { get; set; }
    }
}