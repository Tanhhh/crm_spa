
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Erp.BackOffice.Sale.Models
{
    public class spSale_Get_TrangThietBiViewModel
    {
        public Nullable<DateTime> CreatedDate { get; set; }
        public string EquipmentName { get; set; }
        public string Status { get; set; }
        public Nullable<int> TotalMinute { get; set; }
        public Nullable<DateTime> WorkDay { get; set; }
        public string Code { get; set; }
        public string Note { get; set; }
        public DateTime? EndDate { get; set; }
        public string RoomName { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public int? BranchId { get; set; }

    }
}