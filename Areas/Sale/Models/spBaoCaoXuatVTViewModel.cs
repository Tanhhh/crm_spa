using Erp.BackOffice.App_GlobalResources;
using Erp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Erp.BackOffice.Areas.Sale.Models
{
    public class spBaoCaoXuatVTViewModel
    {
        public Nullable<int> Quantity { get; set; }
        public Nullable<int> MaterialId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string LoCode { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public Nullable<int> WarehouseSourceId { get; set; }
        public string MaterialOutboundCode { get; set; }
    }
}