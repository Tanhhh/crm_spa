using Erp.BackOffice.App_GlobalResources;
using Erp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Erp.BackOffice.Sale.Models
{
    public class DM_BANNER_SLIDERViewModel
    {
        public int BANNER_SLIDER_ID { get; set; }
        public string ANH_DAIDIEN { get; set; }
        public int STT { get; set; }
        public string LINK { get; set; }
        public int IS_SHOW { get; set; }
        public int IS_MOBILE { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        public Nullable<int> CreatedUserId { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> ModifiedUserId { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public Nullable<int> AssignedUserId { get; set; }
        public string MA_DVIQLY { get; set; }
        public bool? IsBrand { get; set; }
        public int? IsNgheSi { get; set; }
        public bool? IsKH { get; set; }
        public int?isDichVu { get; set; }
        public int?isSanPham { get; set; }
        public int? isLogoMobile { get; set; }
        public int? isFooterWeb { get; set; }
    }
}