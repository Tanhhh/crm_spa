using Erp.BackOffice.App_GlobalResources;
using Erp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Erp.BackOffice.Sale.Models
{
    public class DM_LOAIBAIViewModel
    {
        public int LOAIBAI_ID { get; set; }
        public string CODE_LOAIBAI { get; set; }
        public string TEN_LOAIBAI { get; set; }
        public int STT { get; set; }
        public int IS_SHOW { get; set; }


        public Nullable<bool> IsDeleted { get; set; }
        public Nullable<int> CreatedUserId { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> ModifiedUserId { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public Nullable<int> AssignedUserId { get; set; }
        public string MA_DVIQLY { get; set; }
    }
}