
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Erp.BackOffice.Sale.Models
{
    public class Sale_BaoCaoNhapXuatTonKhoVatTuViewModel
    {
        public string MaterialCode { get; set; }
        public string MaterialName { get; set; }
        public string MaterialUnit { get; set; }
        public string LoCode { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public int? First_Remain { get; set; }
        public int? PriceInbound { get; set; }
        public int? PriceOutbound { get; set; }
        public int? Last_InboundQuantity { get; set; }
        public int? Last_OutboundQuantity { get; set; }
        public int? Remain { get; set; }
        public string WarehouseName { get; set; }
    }
}