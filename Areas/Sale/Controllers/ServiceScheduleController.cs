using System.Globalization;
using Erp.BackOffice.Sale.Models;
using Erp.BackOffice.Filters;
using Erp.Domain.Entities;
using Erp.Domain.Interfaces;
using Erp.Domain.Sale.Entities;
using Erp.Domain.Sale.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Erp.Utilities;
using WebMatrix.WebData;
using Erp.BackOffice.Helpers;
using Erp.BackOffice.Areas.Cms.Models;
using System.Web.Script.Serialization;
using System.Web;
using Erp.Domain.Account.Entities;
using Erp.Domain.Account.Interfaces;
using Erp.Domain.Helper;
using OneSignal.CSharp.SDK;
using Push_Notification_OneSignal;
using OneSignal.CSharp.SDK.Resources;

namespace Erp.BackOffice.Sale.Controllers
{
    [Authorize]
    [InitializeSimpleMembership]
    [Erp.BackOffice.Helpers.NoCacheHelper]
    public class ServiceScheduleController : Controller
    {
        private readonly IServiceScheduleRepository ServiceScheduleRepository;
        private readonly IUserRepository userRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly ICustomerRepository CustomerRepository;

        public ServiceScheduleController(
            IServiceScheduleRepository _ServiceSchedule
            , IUserRepository _user
            ,ICategoryRepository category,
            ICustomerRepository _Customer
            )
        {
            ServiceScheduleRepository = _ServiceSchedule;
            userRepository = _user;
            _categoryRepository = category;
            CustomerRepository = _Customer;
        }
        #region Index
        public ActionResult Index()
        {
            var a = _categoryRepository.GetListCategoryByCode("serviceSchedule_status");
            var category = _categoryRepository.GetCategoryByCode("serviceSchedule_status").Select(x => new CategoryViewModel
            {
                Code = x.Code,
                Description = x.Description,
                Id = x.Id,
                Name = x.Name,
                Value = x.Value,
                OrderNo = x.OrderNo
            }).OrderBy(x => x.OrderNo).ToList();
            ViewBag.Category = category;
            return View();
        }
        #endregion

        #region Create
        public ViewResult Create(int? CustomerId, DateTime date)
        {
            var model = new ServiceScheduleViewModel();
            model.StartDate = date;
            model.DueDate = date.AddHours(1);
            //var branchId = Helpers.Common.CurrentUser.BranchId.Value;
            model.BranchId = Helpers.Common.CurrentUser.BranchId;
            model.CustomerId = CustomerId;
            return View(model);
        }

        [HttpPost]
        public ActionResult Create(ServiceScheduleViewModel model,bool?IsPopup)
        {
            var urlRefer = Request["UrlReferrer"];
            if (ModelState.IsValid)
            {
                var serviceSchedule = new ServiceSchedule();
                AutoMapper.Mapper.Map(model, serviceSchedule);
                serviceSchedule.IsDeleted = false;
                serviceSchedule.CreatedUserId = WebSecurity.CurrentUserId;
                serviceSchedule.ModifiedUserId = WebSecurity.CurrentUserId;
                serviceSchedule.AssignedUserId = WebSecurity.CurrentUserId;
                serviceSchedule.CreatedDate = DateTime.Now;
                serviceSchedule.ModifiedDate = DateTime.Now;
                serviceSchedule.Status = "pending";
                serviceSchedule.Chanel = 1;
                ServiceScheduleRepository.InsertServiceSchedule(serviceSchedule);
                string prefix = Erp.BackOffice.Helpers.Common.GetSetting("prefixOrderNo_ServiceSchedule");
                serviceSchedule.Code = Erp.BackOffice.Helpers.Common.GetCode(prefix, serviceSchedule.Id);
                ServiceScheduleRepository.UpdateServiceSchedule(serviceSchedule);

                //ghi log 
                Erp.BackOffice.Controllers.HomeController.WriteLog(serviceSchedule.Id, serviceSchedule.Code, "đã tạo lịch hẹn", "ServiceSchedule/Detail/" + serviceSchedule.Id, Helpers.Common.CurrentUser.BranchId.Value);


                TempData[Globals.SuccessMessageKey] = App_GlobalResources.Wording.InsertSuccess;
                if (IsPopup == true)
                {
                    return RedirectToAction("_ClosePopup", "Home", new { area = "", FunctionCallback = "ClosePopupAndReloadPage" });
                }
                else
                {
                    TempData[Globals.SuccessMessageKey] = App_GlobalResources.Wording.UpdateSuccess;
                    return RedirectToAction("Index");
                }
            }
            return View(model);
        }
        #endregion

        #region Edit

        public ActionResult Edit(int? Id)
        {
            var serviceSchedule = ServiceScheduleRepository.GetvwServiceScheduleById(Id.Value);
            var customerImagePath = Helpers.Common.GetSetting("Customer");
            var filepath = System.Web.HttpContext.Current.Server.MapPath("~" +customerImagePath);
            if (serviceSchedule != null && serviceSchedule.IsDeleted != true)
            {
                var model = new ServiceScheduleViewModel();
                AutoMapper.Mapper.Map(serviceSchedule, model);
                //kiem tra hinh co ton tai hay khong
                if (!string.IsNullOrEmpty(model.CustomerImage))
                {
                    model.CustomerImagePath = customerImagePath + model.CustomerImage;
                    if (!System.IO.File.Exists(filepath + model.CustomerImage))
                    {
                        model.CustomerImagePath = "/assets/img/no-avatar.png";
                    }
                    else
                    {
                        model.CustomerImagePath = customerImagePath + model.CustomerImage;
                    }
                }
                else
                    if (string.IsNullOrEmpty(model.CustomerImage))//Đã có hình
                    {
                        model.CustomerImagePath = "/assets/img/no-avatar.png";
                    }

                return View(model);
            }
            if (Request.UrlReferrer != null)
                return Redirect(Request.UrlReferrer.AbsoluteUri);
            return RedirectToAction("Index");
        }
        [HttpPost]
        public ActionResult Edit(ServiceScheduleViewModel model, bool? IsPopup)
        {
            string APIKEY = Helpers.Common.GetSetting("OneSignalKey");
            string APPID = Helpers.Common.GetSetting("OneSignalApp");

            var urlRefer = Request["UrlReferrer"];
            var serviceSchedule = ServiceScheduleRepository.GetServiceScheduleById(model.Id);
            AutoMapper.Mapper.Map(model, serviceSchedule);
            serviceSchedule.ModifiedUserId = WebSecurity.CurrentUserId;
            serviceSchedule.ModifiedDate = DateTime.Now;
            ServiceScheduleRepository.UpdateServiceSchedule(serviceSchedule);

            if(serviceSchedule.Status == "accept")
            {
                var cus = CustomerRepository.GetCustomerById(serviceSchedule.CustomerId.Value);
                if (!String.IsNullOrEmpty(cus.PlayId))
                {
                    Dictionary<string, string> dictionary = new Dictionary<string, string>();
                    var IncludePlayerIds = new List<string>();
                    IncludePlayerIds.Add(cus.PlayId);
                    var client = new OneSignalClient(APIKEY); // Nhập API Serect Key

                    var options = new NotificationCreateOptions_new();


                    options.AppId = new Guid(APPID); // Nhập My AppID của bạn
                    options.IncludePlayerIds = IncludePlayerIds;
                    //options.IncludedSegments = new List<string> { "All" };
                    options.DeliverToChromeWeb = true;
                    options.DeliverToAndroid = true;
                    options.DeliverToIos = true;
                    options.DeliverToFirefox = true;
                   // dictionary.Add("IdNoti", Noti.Id.ToString());
                    options.Data = dictionary;
                    options.Headings.Add(LanguageCodes.English, "Thông báo");
                    options.Contents.Add(LanguageCodes.English, "Lịch hẹn của bạn đã được tiếp nhận.");


                    //options.Url = txtLaunchURL.Text;
                    //options.ChromeWebImage = txtImageChrome.Text;

                    //options.IncludePlayerIds = IncludePlayerIds;
                    var result = client.Notifications.Create(options);
                }
            }
            if (serviceSchedule.Status == "cancel")
            {
                var cus = CustomerRepository.GetCustomerById(serviceSchedule.CustomerId.Value);
                if (!String.IsNullOrEmpty(cus.PlayId))
                {
                    Dictionary<string, string> dictionary = new Dictionary<string, string>();
                    var IncludePlayerIds = new List<string>();
                    IncludePlayerIds.Add(cus.PlayId);
                    var client = new OneSignalClient(APIKEY); // Nhập API Serect Key

                    var options = new NotificationCreateOptions_new();


                    options.AppId = new Guid(APPID); // Nhập My AppID của bạn
                    options.IncludePlayerIds = IncludePlayerIds;
                    //options.IncludedSegments = new List<string> { "All" };
                    options.DeliverToChromeWeb = true;
                    options.DeliverToAndroid = true;
                    options.DeliverToIos = true;
                    options.DeliverToFirefox = true;
                    // dictionary.Add("IdNoti", Noti.Id.ToString());
                    options.Data = dictionary;
                    options.Headings.Add(LanguageCodes.English, "Thông báo");
                    options.Contents.Add(LanguageCodes.English, "Lịch hẹn của bạn đã được hủy.");


                    //options.Url = txtLaunchURL.Text;
                    //options.ChromeWebImage = txtImageChrome.Text;

                    //options.IncludePlayerIds = IncludePlayerIds;
                    var result = client.Notifications.Create(options);
                }
            }


            //ghi log 
            Erp.BackOffice.Controllers.HomeController.WriteLog(serviceSchedule.Id, serviceSchedule.Code, "đã cập nhật lịch hẹn", "ServiceSchedule/Detail/" + serviceSchedule.Id, Helpers.Common.CurrentUser.BranchId.Value);

            if (IsPopup == true)
            {
                return RedirectToAction("_ClosePopup", "Home", new { area = "", FunctionCallback = "ClosePopupAndReloadPage" });
            }
            else
            {
                TempData[Globals.SuccessMessageKey] = App_GlobalResources.Wording.UpdateSuccess;
                return RedirectToAction("Index");
            }
        }
        #endregion

        #region Detail
        public ActionResult Detail(int? Id)
        {
            var ServiceSchedule = ServiceScheduleRepository.GetvwServiceScheduleById(Id.Value);
            var customerImagePath = Helpers.Common.GetSetting("Customer");
            var filepath = System.Web.HttpContext.Current.Server.MapPath("~" + customerImagePath);
            if (ServiceSchedule != null && ServiceSchedule.IsDeleted != true)
            {
                var model = new ServiceScheduleViewModel();
                AutoMapper.Mapper.Map(ServiceSchedule, model);
                //kiem tra hinh co ton tai hay khong
                if (!string.IsNullOrEmpty(model.CustomerImage))
                {
                    model.CustomerImagePath = customerImagePath + model.CustomerImage;
                    if (!System.IO.File.Exists(filepath + model.CustomerImage))
                    {
                        model.CustomerImagePath = "/assets/img/no-avatar.png";
                    }
                    else
                    {
                        model.CustomerImagePath = customerImagePath + model.CustomerImage;
                    }
                }
                else
                    if (string.IsNullOrEmpty(model.CustomerImage))//Đã có hình
                    {
                        model.CustomerImagePath = "/assets/img/no-avatar.png";
                    }
                //if (model.CreatedUserId != Helpers.Common.CurrentUser.Id && Helpers.Common.CurrentUser.UserTypeId != 1)
                //{
                //    TempData["FailedMessage"] = "NotOwner";
                //    return RedirectToAction("Index");
                //}                

                return View(model);
            }
            if (Request.UrlReferrer != null)
                return Redirect(Request.UrlReferrer.AbsoluteUri);
            return RedirectToAction("Index");
        }
        #endregion

        //#region Delete
        //[HttpPost]
        //public ActionResult Delete()
        //{
        //    try
        //    {
        //        string idDeleteAll = Request["DeleteId-checkbox"];
        //        string[] arrDeleteId = idDeleteAll.Split(',');
        //        for (int i = 0; i < arrDeleteId.Count(); i++)
        //        {
        //            var item = ServiceScheduleRepository.GetServiceScheduleById(int.Parse(arrDeleteId[i], CultureInfo.InvariantCulture));
        //            if(item != null)
        //            {
        //                if (item.CreatedUserId != Helpers.Common.CurrentUser.Id && Helpers.Common.CurrentUser.UserTypeId != 1)
        //                {
        //                    TempData["FailedMessage"] = "NotOwner";
        //                    return RedirectToAction("Index");
        //                }

        //                item.IsDeleted = true;
        //                ServiceScheduleRepository.UpdateServiceSchedule(item);
        //            }
        //        }
        //        TempData[Globals.SuccessMessageKey] = App_GlobalResources.Wording.DeleteSuccess;
        //        return RedirectToAction("Index");
        //    }
        //    catch (DbUpdateException)
        //    {
        //        TempData[Globals.FailedMessageKey] = App_GlobalResources.Error.RelationError;
        //        return RedirectToAction("Index");
        //    }
        //}
        //#endregion

        #region Delete
        [HttpPost]
        public ActionResult Delete(int? id)
        {
            try
            {
                string APIKEY = Helpers.Common.GetSetting("OneSignalKey");
                string APPID = Helpers.Common.GetSetting("OneSignalApp");

                var item = ServiceScheduleRepository.GetServiceScheduleById(id.Value);
                if (item != null)
                {
                    var cus = CustomerRepository.GetCustomerById(item.CustomerId.Value);
                    if (!String.IsNullOrEmpty(cus.PlayId))
                    {
                        Dictionary<string, string> dictionary = new Dictionary<string, string>();
                        var IncludePlayerIds = new List<string>();
                        IncludePlayerIds.Add(cus.PlayId);
                        var client = new OneSignalClient(APIKEY); // Nhập API Serect Key

                        var options = new NotificationCreateOptions_new();


                        options.AppId = new Guid(APPID); // Nhập My AppID của bạn
                        options.IncludePlayerIds = IncludePlayerIds;
                        //options.IncludedSegments = new List<string> { "All" };
                        options.DeliverToChromeWeb = true;
                        options.DeliverToAndroid = true;
                        options.DeliverToIos = true;
                        options.DeliverToFirefox = true;
                        // dictionary.Add("IdNoti", Noti.Id.ToString());
                        options.Data = dictionary;
                        options.Headings.Add(LanguageCodes.English, "Thông báo");
                        options.Contents.Add(LanguageCodes.English, "Lịch hẹn của bạn đã được hủy.");


                        //options.Url = txtLaunchURL.Text;
                        //options.ChromeWebImage = txtImageChrome.Text;

                        //options.IncludePlayerIds = IncludePlayerIds;
                        var result = client.Notifications.Create(options);
                    }
                    ServiceScheduleRepository.DeleteServiceSchedule(item.Id);
                }
                return Content("success");
            }
            catch (DbUpdateException)
            {
                return Content("error");
            }
        }


        [HttpPost]
        public ActionResult Changstatus(int? id)
        {
            try
            {
                string APIKEY = Helpers.Common.GetSetting("OneSignalKey");
                string APPID = Helpers.Common.GetSetting("OneSignalApp");

                var item = ServiceScheduleRepository.GetServiceScheduleById(id.Value);
                if (item != null)
                {
                    item.Status = "accept";
                    ServiceScheduleRepository.UpdateServiceSchedule(item);
                    var cus = CustomerRepository.GetCustomerById(item.CustomerId.Value);
                    if (!String.IsNullOrEmpty(cus.PlayId))
                    {
                        Dictionary<string, string> dictionary = new Dictionary<string, string>();
                        var IncludePlayerIds = new List<string>();
                        IncludePlayerIds.Add(cus.PlayId);
                        var client = new OneSignalClient(APIKEY); // Nhập API Serect Key

                        var options = new NotificationCreateOptions_new();


                        options.AppId = new Guid(APPID); // Nhập My AppID của bạn
                        options.IncludePlayerIds = IncludePlayerIds;
                        //options.IncludedSegments = new List<string> { "All" };
                        options.DeliverToChromeWeb = true;
                        options.DeliverToAndroid = true;
                        options.DeliverToIos = true;
                        options.DeliverToFirefox = true;
                        // dictionary.Add("IdNoti", Noti.Id.ToString());
                        options.Data = dictionary;
                        options.Headings.Add(LanguageCodes.English, "Thông báo");
                        options.Contents.Add(LanguageCodes.English, "Lịch hẹn của bạn đã được tiếp nhận.");


                        //options.Url = txtLaunchURL.Text;
                        //options.ChromeWebImage = txtImageChrome.Text;

                        //options.IncludePlayerIds = IncludePlayerIds;
                        var result = client.Notifications.Create(options);
                    }
                }
                return Content("success");
            }
            catch (DbUpdateException)
            {
                return Content("error");
            }
        }
        #endregion

        #region Calendar
        public ViewResult Calendar(string status_check, int? branchId, int? day, int? month, int? year)
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

            var dataNhanvien = SqlHelper.QuerySP<ManagerStaff>("spgetNhanvienbacap", new
            {
                BranchId = (intBrandID == null ? 0 : intBrandID),
                UserId = WebSecurity.CurrentUserId,
            }).ToList();

            List<int> listNhanvien = new List<int>();


            for (int i = 0; i < dataNhanvien.Count; i++)
            {
                listNhanvien.Add(int.Parse(dataNhanvien[i].Id.ToString()));
            }


            var user = userRepository.GetUserById(WebSecurity.CurrentUserId);
            List<string> ListCheck = new List<string>();
            if (!string.IsNullOrEmpty(status_check))
            {
                ListCheck = status_check.Split(',').ToList();
            }
            else
            {
                ListCheck = "pending,inprogress".Split(',').ToList();
            }
            List<ServiceScheduleViewModel> q = new List<ServiceScheduleViewModel>();
            IEnumerable<ServiceScheduleViewModel> model = ServiceScheduleRepository.GetAllvwServiceSchedule()
                .Select(i => new ServiceScheduleViewModel
                {
                        BranchId=i.BranchId,
                        BranchName=i.BranchName,
                        Code=i.Code,
                        Status=i.Status,
                        StartDate=i.StartDate,
                        DueDate=i.DueDate,
                        CustomerId=i.CustomerId,                      
                        Id=i.Id,
                        CustomerCode=i.CustomerCode,
                        CustomerName=i.CustomerName,
                        Note=i.Note,
                        ServiceName=i.ServiceName,
                        CustomerImage=i.CustomerImage,
                        Chanel = i.Chanel
                }).OrderByDescending(x => x.StartDate).ToList();
            for (int i = 0; i < ListCheck.Count(); i++)
            {
                var a = model.Where(x => x.Status == ListCheck[i].ToString());
                q = q.Union(a).ToList();
            }
            //if (!string.IsNullOrEmpty(Code))
            //{
            //    q = q.Where(item => item.Code == Code).ToList();
            //}
            //if (branchId != null && branchId.Value > 0)
            //{
            //    q = q.Where(item => item.BranchId == branchId).ToList();
            //}
            //if (CustomerId != null && CustomerId.Value > 0)
            //{
            //    q = q.Where(item => item.CustomerId == CustomerId).ToList();
            //}
            //if (ServiceId != null && ServiceId.Value > 0)
            //{
            //    q = q.Where(item => item.ServiceId == ServiceId).ToList();
            //}
            DateTime aDateTime = new DateTime(year.Value, month.Value, day.Value);

            DateTime retDateTime = aDateTime.AddHours(23).AddMinutes(59);

            q = q.Where(x => aDateTime <= x.StartDate.Value && x.StartDate.Value <= retDateTime).OrderBy(x=>x.StartDate).ToList();
            var category = _categoryRepository.GetCategoryByCode("serviceSchedule_status").Select(x => new CategoryViewModel
            {
                Code = x.Code,
                Description = x.Description,
                Id = x.Id,
                Name = x.Name,
                Value = x.Value,
                OrderNo = x.OrderNo
            }).ToList();
            var aa = category.Where(id1 => ListCheck.Any(id2 => id2 == id1.Value)).ToList();
            ViewBag.Category = aa.OrderBy(x => x.OrderNo).ToList();
            //var customerImagePath = Helpers.Common.GetSetting("Customer");
            //var filepath = System.Web.HttpContext.Current.Server.MapPath("~" + customerImagePath);
            List<ServiceScheduleViewModel> lcus = new List<ServiceScheduleViewModel>();
            var pathimg = Helpers.Common.GetSetting("Customer");
            foreach (var item in q)
            {
                var Cus = CustomerRepository.GetvwCustomerById(item.CustomerId.Value);
                
                item.CustomerImagePath = Erp.BackOffice.Helpers.Common.KiemTraTonTaiHinhAnhHoapd(item.CustomerImage, "Customer", "user",pathimg);
                item.NhomHuongId = Cus.NhomHuongDS;
                item.NhomHuong = Cus.NhomNVKD;
                item.NVQLID = Cus.ManagerStaffId;
                //if (!Filters.SecurityFilter.IsAdmin())
                //{
                //    if (Cus.ManagerStaffId == WebSecurity.CurrentUserId)
                //    {
                //        lcus.Add(item);
                //    }
                //}
                //else
                //{
                //    lcus.Add(item);
                //}
            }
            //q = lcus;
            if (user.IsLetan == true || Filters.SecurityFilter.IsAdmin())
            {
                q = q.ToList();
            }
            else
            {
                q = q.Where(x => listNhanvien.Contains(x.NVQLID.Value)).ToList();
            }

            if(intBrandID > 0)
            {
                q = q.Where(x => x.BranchId == intBrandID).ToList();
            }
            var dataEvent = q.Select(e => new { 
                title = e.CustomerCode,
                start = e.StartDate.Value.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                end = e.DueDate.Value.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                allDay = false,
                className = (e.Status == "pending" ? "label-info" : (e.Status == "inprogress" ? "label-warning" : (e.Status == "completed" ? "label-success" : "label-danger"))),
                url = string.Format("/ServiceSchedule/{0}/?Id={1}", (e.Status == "completed" ? "Detail" : "Edit"), e.Id)
            }).ToList();

            ViewBag.dataEvent = new JavaScriptSerializer().Serialize(dataEvent);
            ViewBag.aDateTime = aDateTime.ToString("yyyy-MM-dd");
            return View(q);
        }
        #endregion

       
    }
}
