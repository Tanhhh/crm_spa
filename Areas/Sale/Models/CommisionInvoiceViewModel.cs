using Erp.BackOffice.App_GlobalResources;
using Erp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Erp.BackOffice.Sale.Models
{
    public class CommisionInvoiceViewModel
    {
        public int Id { get; set; }

        [Display(Name = "IsDeleted")]
        public bool? IsDeleted { get; set; }

        [Display(Name = "CreatedUser", ResourceType = typeof(Wording))]
        public int? CreatedUserId { get; set; }
        public string CreatedUserName { get; set; }

        [Display(Name = "CreatedDate", ResourceType = typeof(Wording))]
        public DateTime? CreatedDate { get; set; }

        [Display(Name = "ModifiedUser", ResourceType = typeof(Wording))]
        public int? ModifiedUserId { get; set; }
        public string ModifiedUserName { get; set; }

        [Display(Name = "ModifiedDate", ResourceType = typeof(Wording))]
        public DateTime? ModifiedDate { get; set; }

        [Display(Name = "AssignedUserId", ResourceType = typeof(Wording))]
        public int? AssignedUserId { get; set; }
        public string AssignedUserName { get; set; }

        public Nullable<int> CommissionCusId { get; set; }

        //[Display(Name = "StartSymbol", ResourceType = typeof(Wording))]
        public string StartSymbol { get; set; }
        //[Display(Name = "StartAmount", ResourceType = typeof(Wording))]
        public decimal? StartAmount { get; set; }
        //[Display(Name = "EndSymbol", ResourceType = typeof(Wording))]
        public string EndSymbol { get; set; }
        //[Display(Name = "EndAmount", ResourceType = typeof(Wording))]
        public decimal? EndAmount { get; set; }
        //[Display(Name = "Type", ResourceType = typeof(Wording))]
        public string Type { get; set; }
        //[Display(Name = "CommissionValue", ResourceType = typeof(Wording))]
        public decimal? CommissionValue { get; set; }
        //[Display(Name = "IsMoney", ResourceType = typeof(Wording))]
        public Nullable<bool> IsMoney { get; set; }
        //[Display(Name = "IsVIP", ResourceType = typeof(Wording))]
        public Nullable<bool> IsVIP { get; set; }
        //[Display(Name = "SalesPercent", ResourceType = typeof(Wording))]
        public Nullable<int> SalesPercent { get; set; }
        public int? OrderNo { get; set; }
        public int? Index { get; set; }
        public string Name { get; set; }
        public List<DonateProOrSerViewModel> DonateDetailList { get; set; }
    }
}