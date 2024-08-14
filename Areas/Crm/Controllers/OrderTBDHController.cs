using System.Globalization;
using Erp.BackOffice.Crm.Models;
using Erp.BackOffice.Filters;
using Erp.Domain.Entities;
using Erp.Domain.Interfaces;
using Erp.Domain.Crm.Entities;
using Erp.Domain.Crm.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Erp.Utilities;
using WebMatrix.WebData;
using Erp.BackOffice.Helpers;
using Erp.BackOffice.Sale.Models;
using static System.Runtime.CompilerServices.RuntimeHelpers;
using Erp.Domain.Crm.Helper;
using DocumentFormat.OpenXml.Wordprocessing;
using Erp.Domain.Sale.Repositories;
using System.Text.RegularExpressions;
using DocumentFormat.OpenXml.EMMA;
using DocumentFormat.OpenXml.Office2010.Excel;
using System.Windows.Forms;
using DocumentFormat.OpenXml.Drawing.Charts;
using Newtonsoft.Json;
using Erp.BackOffice.Areas.Crm.Models;
using DocumentFormat.OpenXml.Spreadsheet;

namespace Erp.BackOffice.Crm.Controllers
{
    [Authorize]
    [InitializeSimpleMembership]
    [Erp.BackOffice.Helpers.NoCacheHelper]
    public class OrderTBDHController : Controller
    {
        private readonly IUserRepository userRepository;

        public OrderTBDHController(
            IUserRepository _user
            )
        {
            userRepository = _user;
        }

        #region Tuấn Anh - Xếp loại trung bình đơn hàng
        public ActionResult Index()
        {

            var data = SqlHelper.QuerySP<OrderTBDHCViewModel>("sp_Crm_GetOrderTBDH");
            return View(data);

        }

        [HttpPost]
        public ActionResult Create(OrderTBDHCViewModel model)
        {
            
                try
                {
                    var data = SqlHelper.QuerySP<OrderTBDHCViewModel>("sp_Crm_CreateOrderTBDH", new
                    {
                        @MinTotal = model.MinTotal,
                        @MaxTotal = model.MaxTotal,
                        @Name = model.Name,
                        @CreatedUserId = WebSecurity.CurrentUserId,
                    });

                return Json(new { success = true, message = "Dữ liệu đã được lưu thành công." });
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = "Đã xảy ra lỗi khi lưu dữ liệu.", error = ex.Message });
                }           
         }


        [HttpPost]
        public ActionResult Edit(OrderTBDHCViewModel model)
        {

            try
            {
                var data = SqlHelper.QuerySP<OrderTBDHCViewModel>("sp_Crm_UpdateOrderTBDH", new
                {
                    @Id = model.Id,
                    @MinTotal = model.MinTotal,
                    @MaxTotal = model.MaxTotal,
                    @Name = model.Name,
                    @ModifiedUserId = WebSecurity.CurrentUserId,
                });

                return Json(new { success = true, message = "Dữ liệu đã được cập nhật thành công." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Đã xảy ra lỗi khi cập nhật dữ liệu.", error = ex.Message });
            }
        }

        [HttpPost]
        public ActionResult Delete(string Ids)
        {
            try
            {
                // Phân tích chuỗi IDs thành một mảng các ID
                var idArray = Ids.Split(',');

                foreach (var id in idArray)
                {
                    var data = SqlHelper.QuerySP<OrderTBDHCViewModel>("sp_Crm_DeleteOrderTBDH", new
                    {
                        @Id = int.Parse(id)
                    });
                }

                return Json(new { success = true, message = "Dữ liệu đã được xóa thành công." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Đã xảy ra lỗi khi xóa dữ liệu.", error = ex.Message });
            }
        }




        #endregion


    }
}
