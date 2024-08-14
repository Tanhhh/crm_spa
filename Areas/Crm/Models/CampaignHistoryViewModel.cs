using Erp.BackOffice.App_GlobalResources;
using Erp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Erp.BackOffice.Crm.Models
{
    public class CampaignHistoryViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Action { get; set; }
        public int TongTN { get; set; }
        public int TongKhach { get; set; }
        public int ThanhCong { get; set; }
        public int ThatBai { get; set; }
        public int ChuaGui { get; set; }
    }
}