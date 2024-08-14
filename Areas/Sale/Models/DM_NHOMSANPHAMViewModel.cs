using Erp.BackOffice.App_GlobalResources;
using Erp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Erp.BackOffice.Sale.Models
{
    public class DM_NHOMSANPHAMViewModel
    {
        public int NHOMSANPHAM_ID { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        public Nullable<int> CreatedUserId { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> ModifiedUserId { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public Nullable<int> AssignedUserId { get; set; }

        public string TEN_NHOMSANPHAM { get; set; }
        public int CAP_NHOMSANPHAM { get; set; }
        public int STT { get; set; }
        public Nullable<int> NHOM_CHA { get; set; }
        public string BANNER { get; set; }
        public Nullable<int> IS_SHOW { get; set; }
        public string META_TITLE { get; set; }
        public string META_DESCRIPTION { get; set; }
        public string URL_SLUG { get; set; }
        public string MA_DVIQLY { get; set; }

        [Display(Name = "Là cấp sản phẩm")]
        public bool? IsProduct { get; set; }
        public string Decription { get; set; }
        public string Video { get; set; }
    }
}