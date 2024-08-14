using Erp.BackOffice.App_GlobalResources;
using Erp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Erp.BackOffice.Crm.Models
{
    public class Crm_TargetViewModel
    {
        public int Id { get; set; }

        [Display(Name = "IsDeleted")]
        public bool? IsDeleted { get; set; }

        [Display(Name = "Level")]
        public Nullable<int> Level { get; set; }

        [Display(Name = "Period")]
        public Nullable<int> Period { get; set; }

        [Display(Name = "Target")]
        public Nullable<int> Target { get; set; }
    }
}