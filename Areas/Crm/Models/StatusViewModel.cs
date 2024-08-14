using Erp.BackOffice.App_GlobalResources;
using Erp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Erp.BackOffice.Crm.Models
{
    public class StatusViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal? SuccessRate { get; set; }
        public string ColorStatus { get; set; }
        public int? OrderStatus { get; set; }
        public int? NotMoveable { get; set; }
        public bool? EndStatus { get; set; }
        public int? Type { get; set; }
        public int? TypeForecast { get; set; }
    }
}