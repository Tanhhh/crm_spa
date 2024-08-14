using Erp.BackOffice.App_GlobalResources;
using Erp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Erp.BackOffice.Sale.Models
{
    public class AdviseCardEmailTemplateViewModel
    {
        public string TileEmail { get; set; }
        public string ContentEmail { get; set; }
    }
}