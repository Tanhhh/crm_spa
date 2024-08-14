using Erp.BackOffice.App_GlobalResources;
using Erp.BackOffice.Areas.Cms.Models;
using Erp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Erp.BackOffice.Sale.Models
{
    public class SystemParametersViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Note { get; set; }
        public string Storedprocedure { get; set; }
        public string FiedName { get; set; }
        public string FieldZns { get; set; }
        public string Module { get;set; }
        public bool? IsDeleted { get; set; }
        public bool? isDefault { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? CreatedUserId { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int? ModifiedUserId { get; set; }
        public int? AssignedUserId { get; set; }
        public string CSharp { get;set;}

    }
}