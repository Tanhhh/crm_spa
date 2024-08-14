using Erp.BackOffice.App_GlobalResources;
using Erp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Erp.BackOffice.Crm.Models
{
    public class CRM_KH_GIOITHIEUViewModel
    {
        public int KH_GIOITHIEU_ID { get; set; }
        public int? BranchId { get; set; }
        public int? KHACHHANG_ID { get; set; }
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Error))]
        [Display(Name = "LOAI_GIOITHIEU", ResourceType = typeof(Wording))]
        public string LOAI_GIOITHIEU { get; set; }
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Error))]
        [Display(Name = "TRANGTHAI_GIOITHIEU", ResourceType = typeof(Wording))]
        public string TRANGTHAI_GIOITHIEU { get; set; }
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Error))]
        public string NOIDUNG { get; set; }
        public int? TYLE_THANHCONG { get; set; }
        public bool? IsDeleted { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public int? CreatedUserId { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public int? ModifiedUserId { get; set; }
        [Display(Name = "CustomerName", ResourceType = typeof(Wording))]
        public string CustomerName { get; set; }
        [Display(Name = "Phone", ResourceType = typeof(Wording))]
        public string Phone { get; set; }
        public string CustomerCode { get; set; }
        [Display(Name = "Customer", ResourceType = typeof(Wording))]
        public int? CustomerId { get; set; }
        public string JavaScriptToRun { get; set; }
        //public IEnumerable<CRM_KH_GIOITHIEUViewModel> CRM_KH_GIOITHIEUList { get; set; }
        public string UserName { get; set; }
        public string ModifiedUserName { get; set; }
        public string CustomerId_DisplayText { get; set; }
        public string FullName { get; set; }
        public int? USerId { get; set; }
        public int? ManagerStaffId { get; set; }
        public string BranchName { get; set; }
        public string ManagerUserName { get; set; }
    }
}