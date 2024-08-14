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
    public class CrmTargetAllocationController : Controller
    {
        private readonly IUserRepository userRepository;

        public CrmTargetAllocationController(
            IUserRepository _user
            )
        {
            userRepository = _user;
        }

        #region Tuấn Anh - Target các chỉ số
        public ActionResult Index()
        {
            var dropdownData = SqlHelper.QuerySP<CrmTargetAllocationViewModel>("sp_Crm_GetGroupsAndMembersTarget");
            var targetNew = SqlHelper.QuerySP<CrmTargetAllocationViewModel>("sp_Crm_GetTargetNew");
            var targetOfDay = SqlHelper.QuerySP<CrmTargetAllocationViewModel>("sp_Crm_GetTargetNewOfDay");

            var userLoginId = WebSecurity.CurrentUserId;
            var userLoginPosition = dropdownData
                .Where(x => x.UserId == userLoginId)
                .Select(x => x.StaffPositionName)
                .FirstOrDefault();

            // Kiểm tra vị trí người dùng
            var isEmployee = userLoginPosition == "Nhân viên" ? 1 : 0;

            // Tạo danh sách chi nhánh từ dữ liệu trả về
            var branches = dropdownData
                .GroupBy(x => new { x.BranchId, x.BranchName })
                .Select(g => new SelectListItem { Value = g.Key.BranchId.ToString(), Text = g.Key.BranchName })
                .ToList();

            var groups = dropdownData
                .GroupBy(x => new { x.GroupId, x.GroupName })
                .Select(g => new SelectListItem { Value = g.Key.GroupId.ToString(), Text = g.Key.GroupName })
                .ToList();

            // Tạo danh sách người dùng từ dữ liệu trả về
            var usersByGroup = dropdownData
                .GroupBy(x => x.GroupId)
                .ToDictionary(
                    x => x.Key.ToString(),
                    x => x.Select(u => new SelectListItem { Value = u.UserId.ToString(), Text = u.UserName }).ToList()
                );

            // Lấy thông tin vị trí người dùng
            var userPositions = dropdownData
                .ToDictionary(x => x.UserId, x => x.StaffPositionName);

            ViewBag.Branches = branches;
            ViewBag.Groups = groups;
            ViewBag.UsersByGroup = usersByGroup;
            ViewBag.UserLoginId = userLoginId;
            ViewBag.TargetNew = targetNew;
            ViewBag.TargetOfDay = targetOfDay;
            ViewBag.IsEmployee = isEmployee; 

            return View();
        }


        [HttpPost]
        public ActionResult SaveTargetNew(CrmTargetAllocationViewModel model)
        {
            try
            {
                // Kiểm tra và xử lý giá trị TargetTotal trước khi truyền vào stored procedure

                var target = SqlHelper.QuerySP<CrmTargetAllocationViewModel>("sp_Crm_InsertTargetNew", new
                {
                    @BranchId = model.BranchId,
                    @UserId = model.UserId,
                    @TypeTarget = model.TypeTarget,
                    @TargetTotal = model.TargetTotal,
                    @TargetTotal2 = model.TargetTotal2,
                    @CreatedUserId = WebSecurity.CurrentUserId,
                    @MonthYear = model.MonthYear
                });

                return Json(new { success = true, message = "Dữ liệu đã được lưu thành công." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Đã xảy ra lỗi khi lưu dữ liệu.", error = ex.Message });
            }
        }

        [HttpPost]
        public ActionResult CheckTargetNewDay(CrmTargetAllocationViewModel model)
        {
            try
            {
                var target = SqlHelper.QuerySP<CrmTargetAllocationViewModel>("sp_Crm_CheckTargetNewDay", new
                {
                    @BranchId = model.BranchId,
                    @UserId = model.UserId,
                    @TypeTarget = model.TypeTarget,
                    @MonthYear = model.MonthYear
                }).FirstOrDefault();

                if (target != null)
                {
                    return Json(new { success = true, targetData = target, message = "Target đã được tìm thấy." });
                }
                else
                {
                    return Json(new { success = false, message = "Không có dữ liệu được trả về từ cơ sở dữ liệu." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Đã xảy ra lỗi khi lưu dữ liệu.", error = ex.Message });
            }
        }

        [HttpPost]
        public ActionResult CheckTargetNew(CrmTargetAllocationViewModel model)
        {
            try
            {
                var target = SqlHelper.QuerySP<CrmTargetAllocationViewModel>("sp_Crm_CheckTargetNew", new
                {
                    @BranchId = model.BranchId,
                    @UserId = model.UserId,
                    @TypeTarget = model.TypeTarget,
                    @MonthYear = model.MonthYear
                }).FirstOrDefault();

                if (target != null)
                {
                    return Json(new { success = true, targetData = target, message = "Target đã được tìm thấy." });
                }
                else
                {
                    return Json(new { success = false, message = "Không có dữ liệu được trả về từ cơ sở dữ liệu." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Đã xảy ra lỗi khi lưu dữ liệu.", error = ex.Message });
            }
        }



        [HttpPost]
        public ActionResult CheckTargetNewUsers(CrmTargetAllocationViewModel model)
        {
            try
            {
                var dropdownData = SqlHelper.QuerySP<CrmTargetAllocationViewModel>("sp_Crm_GetGroupsAndMembersTarget");
            
                var userLoginId = WebSecurity.CurrentUserId;
                var userLoginPosition = dropdownData
                    .Where(x => x.UserId == userLoginId)
                    .Select(x => x.StaffPositionName)
                    .FirstOrDefault();

                // Kiểm tra vị trí người dùng
                var isEmployee = userLoginPosition == "Nhân viên" ? 1 : 0;
                var targets = SqlHelper.QuerySP<dynamic>("sp_Crm_CheckTargetNewUsers", new
                {
                    @MonthYear = model.MonthYear,
                    @BranchId = model.BranchId,
                    @TypeTarget = model.TypeTarget
                });

                if (targets.Any())
                {
                    var targetData = targets.Select(t => new
                    {
                        BranchId = t.BranchId,
                        UserId = t.UserId,
                        TypeTarget = t.TypeTarget,
                        TargetTotal = t.TargetTotal,
                        TargetTotal2 = t.TargetTotal2,
                        UserName = t.UserName
                    });

                    return Json(new { success = true, targetData = targetData, message = "Target đã được tìm thấy." });
                }
                else
                {
                    return Json(new { success = false, message = "Không có dữ liệu được trả về từ cơ sở dữ liệu." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Đã xảy ra lỗi khi lưu dữ liệu.", error = ex.Message });
            }
        }


        [HttpPost]
        public ActionResult DeleteTargetNew(CrmTargetAllocationViewModel model)
        {
            try
            {
                var target = SqlHelper.QuerySP<CrmTargetAllocationViewModel>("sp_Crm_DeleteTarget", new
                {
                    @BranchId = model.BranchId,
                    @UserId = model.UserId,
                    @TypeTarget = model.TypeTarget,
                    @MonthYear = model.MonthYear
                });

                return Json(new { success = true, message = "Dữ liệu đã được xóa thành công." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Đã xảy ra lỗi khi xóa dữ liệu.", error = ex.Message });
            }
        }

        [HttpPost]
        public ActionResult DeleteTargetNewOfDay(CrmTargetAllocationViewModel model)
        {
            try
            {
                var target = SqlHelper.QuerySP<CrmTargetAllocationViewModel>("sp_Crm_DeleteTargetOfDay", new
                {
                    @BranchId = model.BranchId,
                    @UserId = model.UserId,
                    @TypeTarget = model.TypeTarget,
                    @MonthYear = model.MonthYear
                });

                return Json(new { success = true, message = "Dữ liệu đã được xóa thành công." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Đã xảy ra lỗi khi xóa dữ liệu.", error = ex.Message });
            }
        }
        [HttpPost]
        public ActionResult SaveTargetNewOfDay(CrmTargetAllocationViewModel model)
        {
            try
            {
                var result = SqlHelper.QuerySP<CrmTargetAllocationViewModel>("sp_GetTargetId", new
                {
                    @BranchId = model.BranchId,
                    @UserId = model.UserId,
                    @TypeTarget = model.TypeTarget,
                    @MonthYear = model.MonthYear,
                    @TargetTotal = model.TargetTotal,
                    @TargetTotal2 = model.TargetTotal2,

                });

                // Lấy targetId từ kết quả truy vấn
                var targetId = result.FirstOrDefault()?.TargetId;
                var target = SqlHelper.QuerySP<CrmTargetAllocationViewModel>("sp_Crm_InsertTargetNewOfDay", new
                {
                    @TargetTotal = model.TargetTotal,
                    @TargetTotal2 = model.TargetTotal2,
                    @Target_Id = targetId,
                    @BranchId = model.BranchId,
                    @UserId = model.UserId,
                    @TypeTarget = model.TypeTarget,
                    @CreatedUserId = WebSecurity.CurrentUserId,
                    @MonthYear = model.MonthYear,
                    @Day1 = model.TargetDay1,
                    @Day2 = model.TargetDay2,
                    @Day3 = model.TargetDay3,
                    @Day4 = model.TargetDay4,
                    @Day5 = model.TargetDay5,
                    @Day6 = model.TargetDay6,
                    @Day7 = model.TargetDay7,
                    @Day8 = model.TargetDay8,
                    @Day9 = model.TargetDay9,
                    @Day10 = model.TargetDay10,
                    @Day11 = model.TargetDay11,
                    @Day12 = model.TargetDay12,
                    @Day13 = model.TargetDay13,
                    @Day14 = model.TargetDay14,
                    @Day15 = model.TargetDay15,
                    @Day16 = model.TargetDay16,
                    @Day17 = model.TargetDay17,
                    @Day18 = model.TargetDay18,
                    @Day19 = model.TargetDay19,
                    @Day20 = model.TargetDay20,
                    @Day21 = model.TargetDay21,
                    @Day22 = model.TargetDay22,
                    @Day23 = model.TargetDay23,
                    @Day24 = model.TargetDay24,
                    @Day25 = model.TargetDay25,
                    @Day26 = model.TargetDay26,
                    @Day27 = model.TargetDay27,
                    @Day28 = model.TargetDay28,
                    @Day29 = model.TargetDay29,
                    @Day30 = model.TargetDay30,
                    @Day31 = model.TargetDay31,
                    @Day1_2 = model.TargetDay1_2,
                    @Day2_2 = model.TargetDay2_2,
                    @Day3_2 = model.TargetDay3_2,
                    @Day4_2 = model.TargetDay4_2,
                    @Day5_2 = model.TargetDay5_2,
                    @Day6_2 = model.TargetDay6_2,
                    @Day7_2 = model.TargetDay7_2,
                    @Day8_2 = model.TargetDay8_2,
                    @Day9_2 = model.TargetDay9_2,
                    @Day10_2 = model.TargetDay10_2,
                    @Day11_2 = model.TargetDay11_2,
                    @Day12_2 = model.TargetDay12_2,
                    @Day13_2 = model.TargetDay13_2,
                    @Day14_2 = model.TargetDay14_2,
                    @Day15_2 = model.TargetDay15_2,
                    @Day16_2 = model.TargetDay16_2,
                    @Day17_2 = model.TargetDay17_2,
                    @Day18_2 = model.TargetDay18_2,
                    @Day19_2 = model.TargetDay19_2,
                    @Day20_2 = model.TargetDay20_2,
                    @Day21_2 = model.TargetDay21_2,
                    @Day22_2 = model.TargetDay22_2,
                    @Day23_2 = model.TargetDay23_2,
                    @Day24_2 = model.TargetDay24_2,
                    @Day25_2 = model.TargetDay25_2,
                    @Day26_2 = model.TargetDay26_2,
                    @Day27_2 = model.TargetDay27_2,
                    @Day28_2 = model.TargetDay28_2,
                    @Day29_2 = model.TargetDay29_2,
                    @Day30_2 = model.TargetDay30_2,
                    @Day31_2 = model.TargetDay31_2,

                });

                return Json(new { success = true, message = "Dữ liệu đã được lưu thành công." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Đã xảy ra lỗi khi lưu dữ liệu.", error = ex.Message });
            }
        }




        #endregion


    }
}
