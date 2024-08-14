using Erp.BackOffice.Filters;
using Erp.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using WebMatrix.WebData;
using Erp.Domain.Crm.Interfaces;
using Erp.BackOffice.Areas.Cms.Models;
using Erp.BackOffice.Crm.Models;
using Erp.Domain.Crm.Entities;
using System.Globalization;
using Erp.BackOffice.Sale.Models;
using Erp.Domain.Entities;
using Erp.Domain.Sale.Entities;
using Erp.Domain.Sale.Interfaces;
using System.Data.Entity.Infrastructure;
using System.Text;
using Erp.BackOffice.Helpers;
using System.Transactions;
using System.Text.RegularExpressions;
using Erp.Domain.Account.Interfaces;
using Erp.Utilities;
using System.Data.Entity.Validation;
using Erp.Domain.Account.Helper;
using Erp.BackOffice.Areas.Administration.Models;
using System.Web;

namespace Erp.BackOffice.Crm.Controllers
{
    [Authorize]
    [InitializeSimpleMembership]
    [Erp.BackOffice.Helpers.NoCacheHelper]
    public class KH_GIOITHIEUController : Controller
    {
        private readonly ICRM_KH_GIOITHIEURepository CRM_KH_GIOITHIEURepository;
        private readonly ICategoryRepository categoryRepository;
        private readonly ICustomerRepository CustomerRepository;
        private readonly IUserRepository userRepository;
        public KH_GIOITHIEUController(
            ICRM_KH_GIOITHIEURepository _CRM_KH_GIOITHIEU,
            ICategoryRepository _category,
            ICustomerRepository _Customer,
            IUserRepository _userRepository
            )
        {
            CRM_KH_GIOITHIEURepository = _CRM_KH_GIOITHIEU;
            categoryRepository = _category;
            CustomerRepository = _Customer;
            userRepository = _userRepository;
        }
        public IEnumerable<CRM_KH_GIOITHIEUViewModel> GETCRM_KH_GIOITHIEU(int? ManagerStaffId, string TRANGTHAI, int? txtGioiThieu, string UserName, int? BranchId)
        {

            var u = userRepository.GetUserById(WebSecurity.CurrentUserId);
            //get cookie brachID 
            HttpRequestBase request = this.HttpContext.Request;
            string strBrandID = "0";
            if (request.Cookies["BRANCH_ID_SPA_CookieName"] != null)
            {
                strBrandID = request.Cookies["BRANCH_ID_SPA_CookieName"].Value;
                if (strBrandID == "")
                {
                    strBrandID = "0";
                }
            }

            //get  CurrentUser.branchId

            if ((Helpers.Common.CurrentUser.BranchId != null) && (Helpers.Common.CurrentUser.BranchId > 0))
            {
                strBrandID = Helpers.Common.CurrentUser.BranchId.ToString();
            }

            int? intBrandID = int.Parse(strBrandID);
            BranchId = intBrandID;
            var dataNhanvien = SqlHelper.QuerySP<ManagerStaff>("spgetNhanvienbacap", new
            {
                BranchId = BranchId,
                UserId = WebSecurity.CurrentUserId,
            }).ToList();

            List<int> listNhanvien = new List<int>();


            for (int i = 0; i < dataNhanvien.Count; i++)
            {
                listNhanvien.Add(int.Parse(dataNhanvien[i].Id.ToString()));
            }
            listNhanvien.Add(WebSecurity.CurrentUserId);
            bool hasSearch = false;
            IEnumerable<CRM_KH_GIOITHIEUViewModel> q = CRM_KH_GIOITHIEURepository.GetAllvwKH_GIOITHIEU().AsEnumerable()
           .Select(item => new CRM_KH_GIOITHIEUViewModel
           {
               KH_GIOITHIEU_ID = item.KH_GIOITHIEU_ID,
               CreatedUserId = item.CreatedUserId,
               CustomerName = item.CustomerName,
               CreatedDate = item.CreatedDate,
               ModifiedUserId = item.ModifiedUserId,
               ModifiedDate = item.ModifiedDate,
               Phone = item.Phone,
               CustomerCode = item.CustomerCode,
               TYLE_THANHCONG = item.TYLE_THANHCONG,
               NOIDUNG = item.NOIDUNG,
               TRANGTHAI_GIOITHIEU = item.TRANGTHAI_GIOITHIEU, /*(item.TRANGTHAI_GIOITHIEU == "daxuly" ? "Đã xử lý" : "Đang chờ duyệt"),*/
               LOAI_GIOITHIEU = (item.LOAI_GIOITHIEU == "mobile" ? "Điện thoại" : "Trực tiếp"),
               UserName = item.UserName,
               FullName = item.FullName,
               ModifiedUserName = item.ModifiedUserName,
               BranchName = item.BranchName,
               BranchId = item.BranchId,
               USerId = item.UserId,
               KHACHHANG_ID = item.KHACHHANG_ID,
               ManagerStaffId = item.ManagerStaffId,
               ManagerUserName = item.ManagerUserName
           }).OrderBy(x => x.CreatedDate).ToList(); //.Where(x => listNhanvien.Contains(x.ManagerStaffId.Value) || x.CreatedUserId == WebSecurity.CurrentUserId)
            //lọc khách trùng
            //q = q.GroupBy(x => x.KHACHHANG_ID).Select(x => x.First()).OrderByDescending(x => x.CreatedDate).ToList();


            if (!Filters.SecurityFilter.IsAdmin() && u.IsLetan != true && (u.UserTypeId != 1027 || u.UserTypeId != 2053))
            {
                q = q.Where(x => x.ManagerStaffId != null && listNhanvien.Contains(x.ManagerStaffId.Value)).ToList();
            }

            if (!string.IsNullOrEmpty(TRANGTHAI))
            {
                TRANGTHAI = Helpers.Common.ChuyenThanhKhongDau(TRANGTHAI);
                q = q.Where(x => Helpers.Common.ChuyenThanhKhongDau(x.TRANGTHAI_GIOITHIEU).Contains(TRANGTHAI));
                hasSearch = true;
            }
            if (txtGioiThieu != null && txtGioiThieu.Value > 0)
            {
                q = q.Where(x => x.TYLE_THANHCONG == txtGioiThieu).ToList();
            }
            if (BranchId != null && BranchId > 0)
            {

                q = q.Where(x => x.BranchId == BranchId).ToList(); //&& x.USerId==WebSecurity.CurrentUserId
            }
            //if (!string.IsNullOrEmpty(UserName))
            //{
            //    UserName = Helpers.Common.ChuyenThanhKhongDau(UserName);
            //    q = q.Where(x => Helpers.Common.ChuyenThanhKhongDau(x.UserName).Contains(UserName.Replace(" ", "")));
            //    hasSearch = true;
            //}
            if (ManagerStaffId != null)
            {
                q = q.Where(x => x.ManagerStaffId == ManagerStaffId);
            }
            int UserId = WebSecurity.CurrentUserId;
            var username = userRepository.GetUserById(UserId);
            ViewBag.UserName = username.FullName;
            var TRANGTHAIGIOITHIEU = categoryRepository.GetAllCategories()
                .Select(item => new CategoryViewModel
                {
                    Id = item.Id,
                    Code = item.Code,
                    Value = item.Value,
                    Name = item.Name
                }).Where(x => x.Code == "TRANGTHAIGIOITHIEU").ToList();
            ViewBag.TRANGTHAIGIOITHIEU = TRANGTHAIGIOITHIEU;
            ViewBag.SuccessMessage = TempData["SuccessMessage"];
            ViewBag.FailedMessage = TempData["FailedMessage"];
            ViewBag.AlertMessage = TempData["AlertMessage"];
            return q;
        }
        #region Index
        public ViewResult Index(int? TU, int? DEN, string ModifiedUserName, string CustomerCode, string CustomerName, int? ManagerStaffId, string TRANGTHAI, int? txtGioiThieu, string UserName, int? BranchId, string CustomerInfo ,string StartDate, string EndDate)
        {
            //get cookie brachID 
            HttpRequestBase request = this.HttpContext.Request;
            string strBrandID = "0";
            if (request.Cookies["BRANCH_ID_SPA_CookieName"] != null)
            {
                strBrandID = request.Cookies["BRANCH_ID_SPA_CookieName"].Value;
                if (strBrandID == "")
                {
                    strBrandID = "0";
                }
            }

            //get  CurrentUser.branchId

            if ((Helpers.Common.CurrentUser.BranchId != null) && (Helpers.Common.CurrentUser.BranchId > 0))
            {
                strBrandID = Helpers.Common.CurrentUser.BranchId.ToString();
            }

            int? intBrandID = int.Parse(strBrandID);
            BranchId = intBrandID;
            var dataNhanvien = SqlHelper.QuerySP<ManagerStaff>("spgetNhanvienbacap", new
            {
                BranchId = intBrandID,
                UserId = WebSecurity.CurrentUserId,
            }).ToList();

            List<int> listNhanvien = new List<int>();


            for (int i = 0; i < dataNhanvien.Count; i++)
            {
                listNhanvien.Add(int.Parse(dataNhanvien[i].Id.ToString()));
            }
            listNhanvien.Add(WebSecurity.CurrentUserId);
            var user = userRepository.GetAllUsers().Select(item => new UserViewModel
            {
                Id = item.Id,
                UserName = item.UserName,
                FullName = item.FullName,
                Status = item.Status,

            }).Where(x => x.Status == UserStatus.Active && listNhanvien.Contains(x.Id)).ToList();
            ViewBag.User = user;
            bool hasSearch = false;
            var data = GETCRM_KH_GIOITHIEU(ManagerStaffId, TRANGTHAI, txtGioiThieu, UserName, BranchId);
            //begin tim theo ngay
            if (!string.IsNullOrEmpty(StartDate))
            {
                var startDate = DateTime.ParseExact(StartDate, "dd/MM/yyyy", null).ToString("yyyy-MM-dd HH:mm:ss");
                DateTime d_startDate = Convert.ToDateTime(startDate);
                if (!string.IsNullOrEmpty(EndDate))
                {
                    var endDate = DateTime.ParseExact(EndDate, "dd/MM/yyyy", null).ToString("yyyy-MM-dd HH:mm:ss");
                    DateTime d_endDate = Convert.ToDateTime(endDate);
                    d_endDate = d_endDate.AddHours(23).AddMinutes(59);
                    data = data.Where(x => x.CreatedDate >= d_startDate && x.CreatedDate <= d_endDate);

                }
            }
            //end tim theo ngay

            if (!string.IsNullOrEmpty(CustomerInfo))
            {
                data = data.Where(x => Helpers.Common.ChuyenThanhKhongDau(x.CustomerName).Contains(Helpers.Common.ChuyenThanhKhongDau(CustomerInfo)) || x.CustomerCode.Contains(CustomerInfo)).ToList();
                hasSearch = true;
            }

            if (!string.IsNullOrEmpty(ModifiedUserName))
            {
                ModifiedUserName = Helpers.Common.ChuyenThanhKhongDau(ModifiedUserName);
                data = data.Where(x => Helpers.Common.ChuyenThanhKhongDau(x.ModifiedUserName).Contains(ModifiedUserName));
                hasSearch = true;
            }
            if (TU != null && DEN != null)
            {
                data = data.Where(x => x.TYLE_THANHCONG >= TU && x.TYLE_THANHCONG <= DEN);
            }
            if (TU == null && DEN != null)
            {
                data = data.Where(x => x.TYLE_THANHCONG == DEN);
            }
            if (TU != null && DEN == null)
            {
                data = data.Where(x => x.TYLE_THANHCONG == TU);
            }

            //data = data.GroupBy(x => x.KHACHHANG_ID).Where(x => !x.Skip(1).Any()).Select(x => x.First());
            data = data.GroupBy(x => x.KHACHHANG_ID).Select(x => x.LastOrDefault()).OrderByDescending(x => x.CreatedDate);
            return View(data);
        }
        #endregion
        #region Create
        public ActionResult Create(int? KH_GIOITHIEU_ID, int? KHACHHANG_ID)
        {
            if (KH_GIOITHIEU_ID == null)
            {
                var LOAIGIOITHIEU = categoryRepository.GetAllCategories()
                .Select(item => new CategoryViewModel
                {
                    Id = item.Id,
                    Code = item.Code,
                    Value = item.Value,
                    Name = item.Name
                }).Where(x => x.Code == "LOAIGIOITHIEU").ToList();
                ViewBag.LOAIGIOITHIEU = LOAIGIOITHIEU;

                var TRANGTHAIGIOITHIEU = categoryRepository.GetAllCategories()
               .Select(item => new CategoryViewModel
               {
                   Id = item.Id,
                   Code = item.Code,
                   Value = item.Value,
                   Name = item.Name
               }).Where(x => x.Code == "TRANGTHAIGIOITHIEU").ToList();
                ViewBag.TRANGTHAIGIOITHIEU = TRANGTHAIGIOITHIEU;
                ViewBag.Username = WebSecurity.CurrentUserName;
                return View();
            }
            else
            {
                var KH_GIOITHIEUList = GETCRM_KH_GIOITHIEU(null, null, null, null, null).Where(x => x.KHACHHANG_ID == KHACHHANG_ID).OrderByDescending(x => x.KH_GIOITHIEU_ID);
                ViewBag.KH_GIOITHIEU = KH_GIOITHIEUList;
                var KH_GIOITHIEU = CRM_KH_GIOITHIEURepository.GetvwKH_GIOITHIEUById(KH_GIOITHIEU_ID.Value);
                if (KH_GIOITHIEU != null && KH_GIOITHIEU.IsDeleted != true)
                {
                    var model = new CRM_KH_GIOITHIEUViewModel();
                    AutoMapper.Mapper.Map(KH_GIOITHIEU, model);
                    //model.CustomerId_DisplayText = KH_GIOITHIEU.CustomerName;
                    model.CustomerId = KH_GIOITHIEU.KHACHHANG_ID;
                    var LOAIGIOITHIEU = categoryRepository.GetAllCategories()
                    .Select(item => new CategoryViewModel
                    {
                        Id = item.Id,
                        Code = item.Code,
                        Value = item.Value,
                        Name = item.Name
                    }).Where(x => x.Code == "LOAIGIOITHIEU").ToList();
                    ViewBag.LOAIGIOITHIEU = LOAIGIOITHIEU;

                    var TRANGTHAIGIOITHIEU = categoryRepository.GetAllCategories()
                   .Select(item => new CategoryViewModel
                   {
                       Id = item.Id,
                       Code = item.Code,
                       Value = item.Value,
                       Name = item.Name
                   }).Where(x => x.Code == "TRANGTHAIGIOITHIEU").ToList();
                    ViewBag.TRANGTHAIGIOITHIEU = TRANGTHAIGIOITHIEU;
                    return View(model);
                }
            }
            return View();
        }
        [HttpPost]
        public ActionResult Create(CRM_KH_GIOITHIEUViewModel model)
        {
            // get cookie brachID
            HttpRequestBase request = this.HttpContext.Request;
            string strBrandID = "0";
            if (request.Cookies["BRANCH_ID_SPA_CookieName"] != null)
            {
                strBrandID = request.Cookies["BRANCH_ID_SPA_CookieName"].Value;
                if (strBrandID == "")
                {
                    strBrandID = "0";
                }
            }

            //get  CurrentUser.branchId

            if ((Helpers.Common.CurrentUser.BranchId != null) && (Helpers.Common.CurrentUser.BranchId > 0))
            {
                strBrandID = Helpers.Common.CurrentUser.BranchId.ToString();
            }

            int? intBrandID = int.Parse(strBrandID);

            try
            {
                if (ModelState.IsValid)
                {
                    //var c = CRM_KH_GIOITHIEURepository.GetAllvwKH_GIOITHIEU()
                    //.Where(item => item.Phone == model.Phone).FirstOrDefault();

                    //if (c != null)
                    //{
                    //    TempData[Globals.FailedMessageKey] = "Khách hàng này đã tồn tại!";
                    //    return RedirectToAction("_ClosePopup", "Home", new { area = "", FunctionCallback = "ClosePopupAndReloadPage" });
                    //    //return RedirectToAction("Index", "KH_GIOITHIEU");
                    //}

                    if (model.KH_GIOITHIEU_ID == null || model.KH_GIOITHIEU_ID == 0)
                    {
                        var CRM_KH_GIOITHIEU = new CRM_KH_GIOITHIEU();
                        AutoMapper.Mapper.Map(model, CRM_KH_GIOITHIEU);
                        CRM_KH_GIOITHIEU.IsDeleted = false;
                        CRM_KH_GIOITHIEU.CreatedUserId = WebSecurity.CurrentUserId;
                        CRM_KH_GIOITHIEU.ModifiedUserId = WebSecurity.CurrentUserId;
                        CRM_KH_GIOITHIEU.CreatedDate = DateTime.Now;
                        CRM_KH_GIOITHIEU.ModifiedDate = DateTime.Now;
                        CRM_KH_GIOITHIEU.KHACHHANG_ID = model.CustomerId;
                        CRM_KH_GIOITHIEU.BranchId = intBrandID;
                        CRM_KH_GIOITHIEURepository.InsertCRM_KH_GIOITHIEU(CRM_KH_GIOITHIEU);

                        TempData[Globals.SuccessMessageKey] = App_GlobalResources.Wording.InsertSuccess;
                        return RedirectToAction("_ClosePopup", "Home", new { area = "", FunctionCallback = "ClosePopupAndReloadPage" });
                    }
                    else if (model.KH_GIOITHIEU_ID != null)
                    {
                        var KH_GIOITHIEU = CRM_KH_GIOITHIEURepository.GetCRM_KH_GIOITHIEUById(model.KH_GIOITHIEU_ID);
                        AutoMapper.Mapper.Map(model, KH_GIOITHIEU);
                        KH_GIOITHIEU.ModifiedUserId = WebSecurity.CurrentUserId;
                        KH_GIOITHIEU.ModifiedDate = DateTime.Now;
                        //KH_GIOITHIEU.KHACHHANG_ID = model.CustomerId;
                        KH_GIOITHIEU.BranchId = intBrandID;
                        CRM_KH_GIOITHIEURepository.UpdateCRM_KH_GIOITHIEU(KH_GIOITHIEU);
                        TempData[Globals.SuccessMessageKey] = App_GlobalResources.Wording.UpdateSuccess;
                        return RedirectToAction("_ClosePopup", "Home", new { area = "", FunctionCallback = "ClosePopupAndReloadPage" });
                    }
                    return View(model);
                }
            }
            catch (Exception ex)
            {

            }
            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(CRM_KH_GIOITHIEUViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (Request["Submit"] == "Save")
                {
                    var cus = CustomerRepository.GetCustomerById(model.KH_GIOITHIEU_ID);
                    AutoMapper.Mapper.Map(model, cus);
                    cus.ModifiedUserId = WebSecurity.CurrentUserId;
                    cus.ModifiedDate = DateTime.Now;
                    CustomerRepository.UpdateCustomer(cus);
                    if (Request["IsPopup"] == "true" || Request["IsPopup"] == "True")
                    {
                        return RedirectToAction("_ClosePopup", "Home", new { area = "", FunctionCallback = "ClosePopupAndReloadPage" });
                    }
                }
                return View(model);
            }
            return RedirectToAction("Index");
        }
        #endregion
        #region Deltail
        public ActionResult Detail()
        {
            return View();
        }
        #endregion
        #region Delete
        [HttpPost]
        public ActionResult Delete()
        {
            try
            {
                string id = Request["Delete"];
                if (id != null)
                {
                    var item = CRM_KH_GIOITHIEURepository.GetCRM_KH_GIOITHIEUById(int.Parse(id, CultureInfo.InvariantCulture));
                    if (item != null)
                    {
                        CRM_KH_GIOITHIEURepository.DeleteCRM_KH_GIOITHIEURs(item.KH_GIOITHIEU_ID);
                    }

                }
                else
                {
                    string idDeleteAll = Request["DeleteId-checkbox"];
                    string[] arrDeleteId = idDeleteAll.Split(',');
                    for (int i = 0; i < arrDeleteId.Count(); i++)
                    {
                        var item = CRM_KH_GIOITHIEURepository.GetCRM_KH_GIOITHIEUById(int.Parse(arrDeleteId[i], CultureInfo.InvariantCulture));
                        if (item != null)
                        {
                            CRM_KH_GIOITHIEURepository.DeleteCRM_KH_GIOITHIEURs(item.KH_GIOITHIEU_ID);
                        }
                    }
                }

                TempData[Globals.SuccessMessageKey] = App_GlobalResources.Wording.DeleteSuccess;
                return RedirectToAction("Index");
            }
            catch (DbUpdateException)
            {
                TempData[Globals.FailedMessageKey] = App_GlobalResources.Error.RelationError;
                return RedirectToAction("Index");
            }
        }
        #endregion
    }
}
