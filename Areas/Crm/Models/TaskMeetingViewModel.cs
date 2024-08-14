using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Erp.BackOffice.Crm.Models
{
    public class TaskMeetingViewModel
    {
        public int Id { get; set; }
        public int LeadId { get; set; }
        public string LeadName { get; set; }
        public string Action { get; set; }
        public string MeetingTitle { get; set; }
        public string MeetingLocation { get; set; }
        public string Source { get; set; }
        public int TypeLead { get; set; }
        public int MeetingTime { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int IsSendNotifications { get; set; }

    }

}