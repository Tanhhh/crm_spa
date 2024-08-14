using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
namespace Erp.BackOffice.Hubs
{
    public class CallInComingHub : Hub
    {
		public async Task CallInComingNotification(string message,int idUs, string clientId)
		{
			await Clients.Client(clientId).CallInComingNotificationToUser(message, idUs);
		}

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