using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
namespace Erp.BackOffice.Crm.Models
{
    public class HistoryCallLogViewModel
    {
        public int Id { get; set; }
        public string Event { get; set; }
        public string Direct { get; set; }
        public string Numberphone { get; set; }
        public string TimeCall { get; set; }
        public string Ext { get; set; }
        public string UserReceiver { get; set; }
        public string TimeExecute { get; set; }
        public string RecordingURL { get; set; }
    }
}