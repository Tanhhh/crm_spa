
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Erp.BackOffice.Sale.Models
{
    public class Sale_BaoCaoNhapXuatTonVatTu_TuanViewModel
    {

        public string CategoryCode { get; set; }
        public string MaterialCode { get; set; }
        public string MaterialName { get; set; }
        public string MaterialUnit { get; set; }
        public Nullable<int> MaterialMinInventory { get; set; }
        public Nullable<int> First_InboundQuantity { get; set; }
        public Nullable<int> First_OutboundQuantity { get; set; }
        public Nullable<int> First_Remain { get; set; }
        public Nullable<int> Last_InboundQuantity { get; set; }
        public Nullable<int> Last_OutboundQuantity { get; set; }
        public Nullable<int> Remain { get; set; }
        public string LoCode { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public string WarehouseName { get; set; }
        public Nullable<int> WarehouseId { get; set; }

    }
}