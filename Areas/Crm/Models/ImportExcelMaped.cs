using Erp.BackOffice.App_GlobalResources;
using Erp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Erp.BackOffice.Crm.Models
{
    public class ImportExcelMaped
    {
        public string ColumnInTable { get; set; }
        public string ColumnInFile { get; set; }
        public int fieldID { get; set; }
    }
    public class ImportExeclCheck
    {
        public string Key { get; set; }
        public bool Value { get; set; }
    }
    public class CheckTrung
    {
        public string Key { get; set; }
        public int Value { get; set; }
    }
}