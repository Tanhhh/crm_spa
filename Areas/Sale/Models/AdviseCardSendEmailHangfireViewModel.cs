using Erp.BackOffice.App_GlobalResources;
using Erp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Erp.BackOffice.Sale.Models
{
    public class AdviseCardSendEmailHangfireViewModel : AdviseCardSendEmailViewModel //
    {
        public int CampaignId { get; set; }
        public string CampaignName { get; set; }
    }
}