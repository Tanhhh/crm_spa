using Erp.BackOffice.App_GlobalResources;
using Erp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Erp.BackOffice.Crm.Models
{
    public class Crm_PeriodViewModel
    {
        public int Id { get; set; }

        [Display(Name = "IsDeleted")]
        public bool? IsDeleted { get; set; }

        [Display(Name = "Tên")]
        public string Name { get; set; }

        [Display(Name = "Bắt đầu")]
        public Nullable<int> starDate { get; set; }

        [Display(Name = "Kết thúc")]
        public Nullable<int> endDate { get; set; }
    }
}