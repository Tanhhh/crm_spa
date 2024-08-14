using Erp.BackOffice.App_GlobalResources;
using Erp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Erp.BackOffice.Crm.Models
{
    public class SettingDisplayLeadViewModel
    {
        public int Id { get; set; }
        public int LeadSectionId { get; set; }
        public string NameLabel { get; set; }
        public bool? IsHiden { get; set; }
        public bool? IsHidenList { get; set; }
    }
}