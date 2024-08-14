using Erp.BackOffice.App_GlobalResources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;

namespace Erp.BackOffice.Crm.Models
{
    public class LeadMeetingViewModel
    {
        public int Id { get; set; }
        public int LeadId { get; set; }
        public string selectedId { get; set; }
        public int? isPartial { get; set; }
        public string Action { get; set; }

        [Display(Name = "Date of Action")]
        public string DateAction { get; set; }

        [Display(Name = "Time of Action")]
        public string TimeAction { get; set; }

        [Display(Name = "Time to Execute")]
        public int TimeExcute { get; set; }

        public string Title { get; set; }

        [Display(Name = "Event Location")]
        public string LocationEvent { get; set; }

        public string Note { get; set; }
        public bool isKpi { get; set; }
        public int UserId { get; set; }

        [Display(Name = "Number of Minutes Warning")]
        public int? Numberofminutes_Warning { get; set; }

        public bool Status { get; set; }
        public bool Important { get; set; }
        public bool? IsDeleted { get; set; }

        [Display(Name = "Created Date")]
        public DateTime? CreatedDate { get; set; }

        [Display(Name = "Created User ID")]
        public int? CreatedUserId { get; set; }

        [Display(Name = "Modified Date")]
        public DateTime? ModifiedDate { get; set; }

        [Display(Name = "Modified User ID")]
        public int? ModifiedUserId { get; set; }

        [Display(Name = "Assigned User ID")]
        public int? AssignedUserId { get; set; }
        public bool? IsEdit { get; set; }

    }

}
