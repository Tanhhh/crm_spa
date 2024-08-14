using Erp.BackOffice.App_GlobalResources;
using Erp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Erp.BackOffice.Sale.Models
{
    public class GopLeadCheckViewModel
    {
        [DisplayName("Id chính")]
        public int MainId { get; set; }
        [DisplayName("Tên Lead")]
        public string LeadName { get; set; }
        [DisplayName("Số Điện Thoại")]
        public string Mobile { get; set; }
        [DisplayName("Mã Số Thuế")]
        public string TaxCode { get; set; }
        [DisplayName("Email")]
        public string Email { get; set; }
        [DisplayName("Trùng")]
        public int Count { get; set; }
    }   
}