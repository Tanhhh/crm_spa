using Erp.BackOffice.App_GlobalResources;
using Erp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Erp.BackOffice.Sale.Models
{
    public class AdviseCardSendEmailViewModel
    {
        [Required(ErrorMessage = "Hãy nhập Email người gửi!")]
        [EmailAddress(ErrorMessage = "Hãy nhập Email có cấu trúc hợp lệ!")]
        public string Sender { get; set; }
        [Required(ErrorMessage = "Hãy nhập Email người nhận!")]
        [EmailAddress(ErrorMessage = "Hãy nhập Email có cấu trúc hợp lệ!")]
        public string Receiver { get; set; }
        [Required(ErrorMessage = "Hãy nhập tiêu đề Email!")]
        public string Title { get; set; }
        [Required(ErrorMessage = "Hãy nhập nội dung Email!")]
        public string Body { get; set; }
    }
}