using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Hangfire.Dashboard;
namespace Erp.BackOffice.App_Start
{
    public class HangfireAuthorizationFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize(DashboardContext context)
        {
            // Thực hiện xác thực người dùng, có thể kiểm tra roles hoặc claims

            return true; // Trả về true nếu muốn cho phép truy cập
        }
    }
}