using Newtonsoft.Json;
using OneSignal.CSharp.SDK.Resources.Notifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Push_Notification_OneSignal
{
    class NotificationCreateOptions_new: NotificationCreateOptions
    {
        [JsonProperty("chrome_web_image")]
        public string ChromeWebImage { get; set; }
    }
}
