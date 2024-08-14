using Erp.BackOffice.App_GlobalResources;
using Erp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Erp.BackOffice.Crm.Models
{
    public class ImportExcelNameLabelViewModel
    {
        public string NameLabel { get; set; }
        public string FieldName { get; set; }
    }
}