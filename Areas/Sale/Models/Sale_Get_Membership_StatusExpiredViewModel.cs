
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Erp.BackOffice.Sale.Models
{
    public class Sale_Get_Membership_StatusExpiredViewModel
    {
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string Code { get; set; }
        public string CustomerName { get; set; }
        public string CustomerCode { get; set; }
        public string ManagerName { get; set; }

    }
}