using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
 namespace Erp.BackOffice.Sale.Models
{
    public class Sale_BaoCaoSapHetHang
    {
        public string Name { get; set; }
        public string ProductGroup { get; set; }
        public string MinInventory { get; set; }
        public int Quantity { get; set; }
        public int ChenhLech { get; set; }
    }
}