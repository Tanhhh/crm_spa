using Erp.BackOffice.App_GlobalResources;
using Erp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Erp.BackOffice.Sale.Models
{
    public class DM_BEST_SELLERViewModel
    {
        public int BEST_SELLER_ID { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        public Nullable<int> CreatedUserId { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> ModifiedUserId { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public Nullable<int> AssignedUserId { get; set; }

        public int Product_Id { get; set; }

        [Display(Name = "ProductOrServiceName", ResourceType = typeof(Wording))]
        public string ProductName { get; set; }   
     
        public int STT { get; set; }
        public int IS_SHOW { get; set; }

        public List<DM_BEST_SELLERViewModel> bestsellerList { get; set; }
        public string MA_DVIQLY { get; set; }
    }
}