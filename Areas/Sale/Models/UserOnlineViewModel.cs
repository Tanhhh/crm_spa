using Erp.BackOffice.App_GlobalResources;
using Erp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Erp.BackOffice.Sale.Models
{
    public class UserOnlineViewModel
    {
        public int Id { get; set; }

        public string ProfileImage { get; set; }
        public string FullName { get; set; }
        public string UserCode { get; set; }
        public int? UserTypeId { get; set; }
        public int? Count_Scheduling { get; set; }
        public string UserTypeName { get; set; }
        public string Status { get; set; }
        public DateTime? InTime { get; set; }
        public DateTime? OutTime { get; set; }
    }
}