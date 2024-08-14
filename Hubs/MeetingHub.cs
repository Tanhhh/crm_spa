using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
namespace Erp.BackOffice.Hubs
{
    public class MeetingHub : Hub
    {
		public async Task SendMeetingNotification(string message)
		{
			await Clients.All.SendAsync("ReceiveMeetingNotification", message);
		}
		//public async Task SendMeetingNotificationToUser(string message, string clientId)
		//{
		//	await Clients.Client(clientId).SendMeetingNotificationToUser(message);
		//}

		public override Task OnConnected()
		{
			//lấy list user đang đăng nhập
			// Lấy trạng thái kết nối từ Session hoặc cơ sở dữ liệu

			if (Helpers.Common.CurrentUser != null)
			{
				
			}
			return base.OnConnected();
		}
		public override Task OnReconnected()
		{

			return base.OnReconnected();
		}


		
	}
}