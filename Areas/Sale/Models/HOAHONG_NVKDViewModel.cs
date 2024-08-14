using Erp.BackOffice.App_GlobalResources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Erp.Domain.Entities;
using System.Web.Mvc;


namespace Erp.BackOffice.Sale.Models
{
    public class HOAHONG_NVKDViewModel
    {
        public int Id { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        public Nullable<int> CreatedUserId { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> ModifiedUserId { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        [Display(Name ="STT")]
        public int STT { get; set; }

        [Display(Name = "Tỷ Lệ Đạt Target %")]
        public string TYLE_TARGET { get; set; }

        [Display(Name ="Từ %")]
        public decimal? MIN_TARGET { get; set; }

        [Display(Name = "Đến %")]
        public decimal? MAX_TARGET { get; set; }

        [Display(Name ="Tỷ lệ hoa hồng NVKD %")]
        public decimal? TYLE_HOAHONG { get; set; }

    }
}