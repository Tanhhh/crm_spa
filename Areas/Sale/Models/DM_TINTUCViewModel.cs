using Erp.BackOffice.App_GlobalResources;
using Erp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Erp.BackOffice.Sale.Models
{
    public class DM_TINTUCViewModel
    {
        public int TINTUC_ID { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        public Nullable<int> CreatedUserId { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> ModifiedUserId { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public Nullable<int> AssignedUserId { get; set; }

        [Display(Name = "Nhóm tin")]
        public int NHOMTIN_ID { get; set; }

        [Display(Name = "Title", ResourceType = typeof(Wording))]
        public string TIEUDE { get; set; }

        public string TOMTAT { get; set; }

        [Display(Name = "Content", ResourceType = typeof(Wording))]
        public string NOIDUNG { get; set; }

        public string ANH_DAIDIEN { get; set; }

        [Display(Name = "OrderNo", ResourceType = typeof(Wording))]
        public int STT { get; set; }

        public Nullable<int> IS_NOIBAT { get; set; }

        public int IS_SHOW { get; set; }

        public string TEN_NhomTin { get; set; }

        public string LIST_TAGS { get; set; }

        public string META_TITLE { get; set; }
        public string META_DESCRIPTION { get; set; }
        public string URL_SLUG { get; set; }

        public string MA_DVIQLY { get; set; } 

       
    }
}