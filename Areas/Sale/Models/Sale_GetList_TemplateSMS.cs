using Erp.BackOffice.App_GlobalResources;
using Erp.BackOffice.Areas.Cms.Models;
using Erp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Erp.BackOffice.Sale.Models
{
    public class Sale_GetList_TemplateSMS
    {
        public int Id { get; set; }
        public string ContentRule { get; set; }
        public string ZNSId { get; set; }
        public string error { get; set; }
        public string realValue { get; set; }
        public string LeadPhone { get; set; }
        public string LeadName { get; set; }

    }
}