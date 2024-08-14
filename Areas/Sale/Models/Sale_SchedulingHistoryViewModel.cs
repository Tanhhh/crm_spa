
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Erp.BackOffice.Sale.Models
{
    public class Sale_SchedulingHistoryViewModel
    {
        public int? ProductId { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public int? Quantity { get; set; }
        public string ProductGroup { get; set; }
        public string CategoryCode { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CustomerName { get; set; }
        public string CustomerCode { get; set; }
        public string RoomName { get; set; }
    }
}