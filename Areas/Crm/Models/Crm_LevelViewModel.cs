using Erp.BackOffice.App_GlobalResources;
using Erp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Erp.BackOffice.Crm.Models
{
    public class Crm_LevelViewModel
    {
        public int Id { get; set; }

        [Display(Name = "IsDeleted")]
        public bool? IsDeleted { get; set; }

        [Display(Name = "Tên")]
        public string Name { get; set; }

        [Display(Name = "Level")]
        public string Level { get; set; }

        [Display(Name = "Tối thiểu(%)")]
        public Nullable<int> StaredIndex { get; set; }

        [Display(Name = "Tối đa(%)")]
        public Nullable<int> EndIndex { get; set; }
    }
}