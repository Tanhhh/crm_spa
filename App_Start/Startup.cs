using Owin;
using Microsoft.Owin;
using System.Net;
using Hangfire;
using Microsoft.AspNet.SignalR;
using Hangfire.Common;
using Hangfire.Server;
using Erp.BackOffice.Crm.Controllers;
[assembly: OwinStartup(typeof(Erp.BackOffice.App_Start.Startup))]
namespace Erp.BackOffice.App_Start
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // Any connection or hub wire up and configuration should go here
            app.MapSignalR();
            //app.MapSignalR("/meetingHub", new HubConfiguration());
            //app.MapSignalR("/zaloHub", new HubConfiguration());

            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            ////thêm bảng dữ liệu của Hangfire 
            GlobalConfiguration.Configuration.UseSqlServerStorage("ErpDbContext");

            ////Cấu hình hangfire




            app.UseHangfireDashboard("/hangfire", new DashboardOptions
            {
                Authorization = new[] { new HangfireAuthorizationFilter() }
            });
            app.UseHangfireServer();
            var tokenService = new AnswerController();

            // Lập lịch cho công việc lấy accesstoken tự động - 8h 1 lần
            BackgroundJob.Enqueue(() => tokenService.ScheduleRefreshTokens());          
            RecurringJob.AddOrUpdate("RefreshTokens", () => tokenService.ScheduleRefreshTokens(), Cron.Hourly(8));


        }
    }
}