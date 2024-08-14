
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Erp.BackOffice.Sale.Models
{
    public class Sale_ProductSampleDetailViewModel
    {
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string CompanyName { get; set; }
        public string CustomerCode { get; set; }
        public string ProductSampleCode { get; set; }
        public string Status { get; set; }
        public string ProductName { get; set; }
        public string ProductCode { get; set; }
        public int? Id { get; set; }
        public int? CustomerId { get; set; }
        public int? ProductSampleId { get; set; }
    }
}