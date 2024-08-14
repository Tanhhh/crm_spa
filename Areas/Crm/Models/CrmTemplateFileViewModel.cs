using Erp.BackOffice.App_GlobalResources;
using Erp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Erp.BackOffice.Crm.Models
{
    public class CrmTemplateFileViewModel
    {
        public int Id { get; set; }
        public string Module { get; set; }
        public string FileName { get; set; }
    }
    public class DataSelectItem
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }
}