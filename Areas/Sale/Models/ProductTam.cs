using System;

namespace Erp.BackOffice.Sale.Models
{
    public class ProductTam
    {

        public string ProductName { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }

        public string LoCode { get; set; }
        public Nullable<System.DateTime> ExpiryDate { get; set; }



    }
}