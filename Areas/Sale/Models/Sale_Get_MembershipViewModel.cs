
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Erp.BackOffice.Sale.Models
{
    public class Sale_Get_MembershipViewModel
    {
        public string Code { get; set; }
        public int? CustomerId { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public string TargetCode { get; set; }
        public int TargetId { get; set; }
        public string TargetModule { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public int ProductId { get; set; }
        public string StatusExpiryDate { get; set; }
        public int? ManagerId { get; set; }
        public string ManagerName { get; set; }
    }
}