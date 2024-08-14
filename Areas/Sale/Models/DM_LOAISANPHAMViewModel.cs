using Erp.BackOffice.App_GlobalResources;
using Erp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
namespace Erp.BackOffice.Sale.Models
{
    public class DM_LOAISANPHAMViewModel
    {
        public int LOAISANPHAM_ID { get; set; }
        public int STT { get; set; }
        public string TEN_LOAISANPHAM { get; set; }
        public int IS_SHOWMAIN { get; set; }
        public Nullable<int> LOAISANPHAM_IDCHA { get; set; }
        public string ANH_DAIDIEN { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        public Nullable<int> CreatedUserId { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> ModifiedUserId { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public Nullable<int> AssignedUserId { get; set; }

        public string META_TITLE { get; set; }
        public string META_DESCRIPTION { get; set; }
        public string URL_SLUG { get; set; }

        public string MA_DVIQLY { get; set; }
    }
}