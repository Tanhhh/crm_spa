using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Erp.Domain.Entities;
using System.Globalization;
using Erp.BackOffice.Helpers;
using System;
using Erp.BackOffice.Models;
using Erp.Domain.Interfaces;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;
using System.Web.Security;
using WebMatrix.WebData;
using Erp.Utilities;
using System.IO;
using System.Drawing.Text;
using System.Drawing.Drawing2D;
using Erp.BackOffice.App_GlobalResources;
using Erp.Domain.Staff.Interfaces;
using Erp.BackOffice.Staff.Models;
using Erp.Domain.Helper;
using Erp.Domain.Sale.Interfaces;
using Erp.BackOffice.Administration.Models;
using Erp.Domain.Crm.Interfaces;
using Erp.BackOffice.Crm.Models;
using Erp.BackOffice.Areas.Cms.Models;
using Erp.BackOffice.Areas.Administration.Models;
using Erp.BackOffice.Sale.Models;
using Erp.Domain.Crm.Repositories;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using DocumentFormat.OpenXml.EMMA;
using System.Diagnostics;
using Erp.BackOffice.Hubs;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using AutoMapper;
using System.Net.Http;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Drawing.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using System.Configuration;


namespace Erp.BackOffice.Controllers
{
    [Erp.BackOffice.Helpers.NoCacheHelper]
    public class HomeController : Controller
    {
        private readonly IPageMenuRepository _pageMenuRepository;
        private readonly IUserRepository _userRepository;
        private readonly ILoginLogRepository _loginlogRepository;
        private readonly IInternalNotificationsRepository internalNNotificationsRepository;
        private readonly IStaffsRepository staffRepository;
        private readonly ITaskRepository taskRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IUserTypeRepository userTypeRepository;
        private readonly IWarehouseRepository WarehouseRepository;
        private readonly IUserRepository userRepository;
        private readonly IChatFacebookRepository _chatFacebookRepository;
        public object ChatFacebookrRepository { get; private set; }


        public HomeController(
            IUserRepository userRepo
            , IPageMenuRepository pageMenuRepository
            , ILoginLogRepository loginlog
            , IInternalNotificationsRepository internalNotifications
            , IStaffsRepository staffs
            , ITaskRepository task
             , ICategoryRepository categoryRepository
            , IUserTypeRepository userType
            , IWarehouseRepository _Warehouse
            , IUserRepository _user
            , IChatFacebookRepository chatFacebook
            )
        {
            _pageMenuRepository = pageMenuRepository;
            _loginlogRepository = loginlog;
            _userRepository = userRepo;
            internalNNotificationsRepository = internalNotifications;
            staffRepository = staffs;
            taskRepository = task;
            _categoryRepository = categoryRepository;
            userTypeRepository = userType;
            WarehouseRepository = _Warehouse;
            userRepository = _user;
            _chatFacebookRepository = chatFacebook;
        }

        [Authorize]
        [OutputCache(CacheProfile = "Cache24h", VaryByParam = "*", Location = OutputCacheLocation.Client)]
        public ActionResult Index()
        {

            string home_page = Helpers.Common.GetSetting("home_page");
            var id = Erp.BackOffice.Helpers.Common.CurrentUser.UserTypeId;
            //var usertype = userTypeRepository.GetUserTypeById(id);
            if (id == 21)
            {
                var url = "/Customer/Client";
                return Redirect(url);
            }
            return Redirect(home_page);
        }

        [Authorize]
        [OutputCache(CacheProfile = "Cache24h", VaryByParam = "*", Location = OutputCacheLocation.Client)]
        public ActionResult Dashboard(int Id)
        {
            ViewBag.Id = Id;
            return View();
        }

        [Authorize]
        public ActionResult DashboardInventory()
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

            
            //
            var warehouseList = WarehouseRepository.GetvwAllWarehouse().AsEnumerable();
            warehouseList = warehouseList.Where(x => x.Categories != "VT").ToList();
            if(intBrandID > 0)
            {
                warehouseList = warehouseList.Where(x => x.BranchId == intBrandID).ToList();
            }
            var _warehouseList = warehouseList.Select(item => new SelectListItem
            {
                Text = item.Name + " (" + item.BranchName + ")",
                Value = item.Id.ToString()
            });
            ViewBag.warehouseList = _warehouseList;
            return View();
        }
        [Authorize]
        public ActionResult DashboardMaterialInventory()
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

            var warehouseList = WarehouseRepository.GetvwAllWarehouse().AsEnumerable();
            warehouseList = warehouseList.Where(x => x.Categories == "VT").ToList();
            if(intBrandID > 0)
            {
                warehouseList = warehouseList.Where(x => x.BranchId == intBrandID).ToList();
            }
            var _warehouseList = warehouseList.Select(item => new SelectListItem
            {
                Text = item.Name + " (" + item.BranchName + ")",
                Value = item.Id.ToString()
            });
            ViewBag.warehouseList = _warehouseList;
            return View();
        }
        [Authorize]
        [OutputCache(CacheProfile = "Cache24h", VaryByParam = "*", Location = OutputCacheLocation.Client)]
        public ActionResult DashboardStaff()
        {
            return View();
        }
        [Authorize]
        [OutputCache(CacheProfile = "Cache24h", VaryByParam = "*", Location = OutputCacheLocation.Client)]
        public ActionResult DashboardSalary()
        {
            return View();
        }
        [Authorize]
        [OutputCache(CacheProfile = "Cache24h", VaryByParam = "*", Location = OutputCacheLocation.Client)]
        public ActionResult DashboardSale()
        {
            
            return View();
        }
        [Authorize]
        [OutputCache(CacheProfile = "Cache24h", VaryByParam = "*", Location = OutputCacheLocation.Client)]
        public ActionResult DashboardCrm()
        {
            var category = _categoryRepository.GetCategoryByCode("task_status").Select(x => new CategoryViewModel
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
        [Authorize]
        [OutputCache(CacheProfile = "Cache24h", VaryByParam = "*", Location = OutputCacheLocation.Client)]
        public ActionResult DashboardReport()
        {
            return View();
        }
        [Authorize]
        [OutputCache(CacheProfile = "Cache24h", VaryByParam = "*", Location = OutputCacheLocation.Client)]
        public ActionResult DashboardTransaction()
        {
            return View();
        }

        [Authorize]
        [OutputCache(CacheProfile = "Cache24h", VaryByParam = "*", Location = OutputCacheLocation.Client)]
        public ActionResult TrackRequest()
        {
            var model = ControllerContext.HttpContext.Application["ListRequest"] as List<RequestInfo>;
            return View(model.OrderByDescending(item => item.FirstDate).ToList());
        }
        [Authorize]
        [OutputCache(CacheProfile = "Cache24h", VaryByParam = "*", Location = OutputCacheLocation.Client)]
        public ActionResult Breadcrumb(List<Erp.BackOffice.Areas.Administration.Models.PageMenuViewModel> DataMenuItem, string controllerName, string actionName, string areaName)
        {
            var model = new List<BreadcrumbViewModel>();

            var currentUrl = ("/" + controllerName + "/" + actionName).ToLower();
            var pageMenuItem = DataMenuItem.Where(x => (x.Url != null && x.Url.ToLower() == currentUrl) || (x.PageUrl != null && x.PageUrl.ToLower() == currentUrl)).FirstOrDefault();

            if (pageMenuItem == null)
            {
                currentUrl = ("/" + controllerName + "/index").ToLower();
                pageMenuItem = DataMenuItem.Where(x => x.PageUrl != null && x.PageUrl.ToLower() == currentUrl).FirstOrDefault();
            }

            while (pageMenuItem != null)
            {
                var breadCrumb = new BreadcrumbViewModel
                {
                    Area = pageMenuItem.AreaName,
                    ParrentId = pageMenuItem.ParentId,
                    Name = pageMenuItem.Name,
                    Url = pageMenuItem.PageUrl
                };

                model.Insert(0, breadCrumb);

                if (pageMenuItem.ParentId != null)
                {
                    pageMenuItem = DataMenuItem.Where(x => x.Id == pageMenuItem.ParentId.Value).FirstOrDefault();
                }
                else
                {
                    pageMenuItem = null;
                }
            }


            return PartialView("_BreadcrumbPartial", model);

            //var page = _pageMenuRepository.GetPageByAction(areaName, controllerName, actionName);

            //if (page != null)
            //{
            //    var model = new BreadcrumbViewModel
            //    {
            //        Area = page.AreaName,
            //        Controller = page.ControllerName,
            //        Action = page.ActionName,
            //        ParrentId = page.ParentId,
            //        Name = _pageMenuRepository.GetPageName(page.Id, Session["CurrentLanguage"].ToString()),
            //        Url = page.Url
            //    };
            //    return PartialView("_BreadcrumbPartial", model);
            //}

            //return PartialView("_BreadcrumbPartial", null);
        }
        [OutputCache(CacheProfile = "Cache24h", VaryByParam = "*", Location = OutputCacheLocation.Client)]
        public ActionResult BreadcrumbParentPage(int parrentId)
        {
            var page = _pageMenuRepository.GetPageById(parrentId);

            if (page != null)
            {
                var model = new BreadcrumbViewModel
                {
                    Area = page.AreaName,
                    Controller = page.ControllerName,
                    Action = page.ActionName,
                    ParrentId = page.ParentId,
                    Name = _pageMenuRepository.GetPageName(page.Id, Session["CurrentLanguage"].ToString())
                };
                return PartialView("_BreadcrumbParrentPagePartial", model);
            }

            return PartialView("_BreadcrumbParrentPagePartial", null);
        }

        [AllowAnonymous]
        public ActionResult _ClosePopup()
        {
            return View();
        }

        [Authorize]
        public ActionResult Notifications()
        {

            var user = _userRepository.GetUserById(WebSecurity.CurrentUserId);
            //xóa thông báo quá hạn.
            DeleteNotifications(user.Id);
            //lấy danh sách thông báo của user hiện lên.
            var pathimg = Helpers.Common.GetSetting("Customer");
            var q = taskRepository.GetAllvwTask().Take(50).Where(x => x.ModifiedUserId == user.Id && x.Type == "notifications").AsEnumerable()
                .Select(x => new TaskViewModel
                {
                    AssignedUserId = x.AssignedUserId,
                    CreatedDate = x.CreatedDate,
                    CreatedUserId = x.CreatedUserId,
                    FullName = x.FullName,
                    ProfileImage = Erp.BackOffice.Helpers.Common.KiemTraTonTaiHinhAnhHoapd(x.ProfileImage, "Staff", "user", pathimg),
                    Id = x.Id,
                    IsDeleted = x.IsDeleted,
                    ParentId = x.ParentId,
                    ParentType = x.ParentType,
                    Subject = x.Subject,
                    UserName = x.UserName,
                    ModifiedUserId = x.ModifiedUserId
                })?.Take(50).OrderByDescending(x => x.CreatedDate).ToList();
            return View(q);
        }

        #region xóa thông báo quá hạn
        public static void DeleteNotifications(int? userID)
        {
            //hoapd rem lai do chay qua lau
            ////chỉ xóa những tin nhắn quá hạn mà đã xem.
            //Erp.Domain.Crm.Repositories.TaskRepository taskRepository = new Erp.Domain.Crm.Repositories.TaskRepository(new Domain.Crm.ErpCrmDbContext());
            //var quantityDate = Helpers.Common.GetSetting("quantity_notifications_date");
            //var date = DateTime.Now.AddDays(Convert.ToInt32(quantityDate));
            //var notifications = taskRepository.GetAllTaskFull().Where(x => x.AssignedUserId == userID && x.Type == "notifications").ToList();
            //notifications = notifications.Where(x => x.CreatedDate <= date).ToList();
            //for (int i = 0; i < notifications.Count(); i++)
            //{
            //    taskRepository.DeleteTask(notifications[i].Id);
            //}

        }
        #endregion

        public ActionResult InteractiveChart()
        {

            var dataNhanvien = SqlHelper.QuerySP<ManagerStaff>("spgetNhanvienbacap", new
            {
                BranchId = Erp.BackOffice.Helpers.Common.CurrentUser.BranchId,
                UserId = WebSecurity.CurrentUserId,
            }).ToList();

            List<int> listNhanvien = new List<int>();


            for (int i = 0; i < dataNhanvien.Count; i++)
            {
                listNhanvien.Add(int.Parse(dataNhanvien[i].Id.ToString()));
            }


            var user = userRepository.GetAllUsers().Select(item => new UserViewModel
            {
                Id = item.Id,
                UserName = item.UserName,
                FullName = item.FullName,
                Status = item.Status,
                BranchId = item.BranchId,
                UserTypeId = item.UserTypeId
            }).Where(x => x.Status == UserStatus.Active && listNhanvien.Contains(x.Id) && x.UserTypeId != 2054 && x.UserTypeId != 2039).ToList();


            ViewBag.user = user;
            ViewBag.batbuoc = "Bắt buộc nhập";


            return View();

        }

        public List<InteractiveChartViewModel> GetList_InteractiveChart(string StartDate, string EndDate, string CityId, string DistrictId, int? branchId, int? NGUOILAP, string HINHTHUC_TUONGTAC)
        {
            // sau nay nếu có bổ xung ???
            CityId = string.IsNullOrEmpty(CityId) ? "" : CityId;
            DistrictId = string.IsNullOrEmpty(DistrictId) ? "" : DistrictId;
            branchId = branchId == null ? 0 : branchId;
            NGUOILAP = NGUOILAP == null ? 0 : NGUOILAP;
            //if (!Filters.SecurityFilter.IsAdmin() && branchId == 0)
            //{
            //    branchId = Helpers.Common.CurrentUser.BranchId;
            //}
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
            branchId = intBrandID;

            var d_startDate = DateTime.ParseExact(StartDate, "dd/MM/yyyy", null).ToString("yyyy-MM-dd HH:mm:ss");
            var d_endDate = DateTime.ParseExact(EndDate, "dd/MM/yyyy", null).ToString("yyyy-MM-dd HH:mm:ss");
            HINHTHUC_TUONGTAC = string.IsNullOrEmpty(HINHTHUC_TUONGTAC) ? "" : HINHTHUC_TUONGTAC;
            
            var data = HINHTHUC_TUONGTAC == "GOIDIEN" ? SqlHelper.QuerySP<InteractiveChartViewModel>("spGetList_InteractiveChart_phone", new
            {
                StartDate = d_startDate,
                EndDate = d_endDate,
                HINHTHUC_TUONGTAC = HINHTHUC_TUONGTAC,
                NGUOILAP = NGUOILAP,
                branchId = branchId
            }).ToList() : SqlHelper.QuerySP<InteractiveChartViewModel>("spGetList_InteractiveChart", new
            {
                StartDate = d_startDate,
                EndDate = d_endDate,
                HINHTHUC_TUONGTAC = HINHTHUC_TUONGTAC,
                NGUOILAP = NGUOILAP,
                branchId = branchId
            }).ToList();

             
            ViewBag.StartDate = StartDate;
            ViewBag.EndDate = EndDate;

            return data;
        }

        public PartialViewResult _GetList_InteractiveChart(string StartDate, string EndDate, string CityId, string DistrictId, int? branchId, int? NGUOILAP, string HINHTHUC_TUONGTAC)
        {
            ViewBag.hinhthuctuongtac = HINHTHUC_TUONGTAC;
            var data = GetList_InteractiveChart(StartDate, EndDate, CityId, DistrictId, branchId, NGUOILAP, HINHTHUC_TUONGTAC);
            return PartialView(data);
        }
        [HttpPost]
        public ActionResult _Set_GlobalCurentBranchId(int? branchId)
        {

            Session["GlobalCurentBranchId"] = branchId;


            return View();
        }


        public static bool WriteLog(long TargetId, string TargetCode, string Content, string Action, int BranchId)
        {
            Erp.Domain.Repositories.UserHistoryRepository UserHistoryRepository = new Erp.Domain.Repositories.UserHistoryRepository(new Domain.ErpDbContext());
            var UserId = Helpers.Common.CurrentUser.Id;
            var His = new UserHistory();
            His.CreatedDate = DateTime.Now;
            His.TargetId = TargetId;
            His.TargetCode = TargetCode;
           
            His.IsDeleted = false;
            His.Action = Action;
            His.BranchId = BranchId;
            His.UserId = UserId;
            His.UserName = Helpers.Common.CurrentUser.UserName;
            His.Content = His.UserName +" " +Content + " "+ TargetCode;
            UserHistoryRepository.InsertUserHistory(His);
            return true;
        }


        //public ActionResult FacebookLogin()
        //{
        //    return View();
        //}

        //public ActionResult ManageFB(string id)
        //{
        //    ViewBag.id = id;
        //    return View();
        //}
        //[HttpPost]
        //[AllowAnonymous]
        //public JsonResult GetPageSession(string model)
        //{
        //    var listPage = new JavaScriptSerializer().Deserialize<List<FaceBookPageViewModel>>(model);
        //    Session["FanPageId"] = listPage;
        //    return Json(new
        //    {
        //        status = true
        //    });
        //}

        //[HttpPost]
        //[AllowAnonymous]
        //public JsonResult GetFanpage()
        //{
        //    var listPage = new List<FaceBookPageViewModel>();
        //    listPage = Session["FanPageId"] as List<FaceBookPageViewModel>;
        //    return Json(listPage);
        //}


        //[HttpPost]
        //[AllowAnonymous]
        //public JsonResult Create_Chat(string FacebookId)
        //{
        //    try
        //    {
        //        //begin hoapd copy code insert
        //        // string pMA_DVIQLY = Helpers.Common.CurrentUser.MA_DVIQLY;


        //        //Kiểm tra trùng Facebook thì không cho lưu


        //        var chat = _chatFacebookRepository.GetChatFacebookByFacebookId(FacebookId).ToList();
        //        foreach (var i in chat)
        //        {
        //            if ((i.chatDate == System.DateTime.Today) && (i.FacebookId == FacebookId))
        //            {
        //                return Json(0);
        //            }
        //        }



        //        var ChatFacebook = new Domain.Sale.Entities.ChatFacebook();

        //        //AutoMapper.Mapper.Map(ChatFacebook);
        //        ChatFacebook.IsDeleted = false;
        //        ChatFacebook.CreatedUserId = WebSecurity.CurrentUserId;
        //        ChatFacebook.ModifiedUserId = WebSecurity.CurrentUserId;
        //        ChatFacebook.CreatedDate = DateTime.Now;
        //        ChatFacebook.ModifiedDate = DateTime.Now;
        //        ChatFacebook.chatDate = DateTime.Today;
        //        //  ChatFacebook.MA_DVIQLY = pMA_DVIQLY;
        //        ChatFacebook.FacebookId = FacebookId;
        //        _chatFacebookRepository.InsertChatFacebook(ChatFacebook);
        //        Erp.BackOffice.Helpers.Common.SetOrderNo("ChatFacebook");
        //        return Json(ChatFacebook);
        //    }
        //    catch (Exception ex)
        //    {

        //        return Json(0);
        //    };
        //}

        [Authorize]
        public ActionResult NotificationsOfLeadmeeting()
        {
            var listTaskNews = SqlHelper.QuerySP<TaskMeetingViewModel>("sp_Crm_SuccessTaskNew_Get", new
            {
                pCurrentUser = WebSecurity.CurrentUserId
            });
            return View(listTaskNews);
        }
        [AllowAnonymous]
        public ActionResult NotificationsLogOfLeadMeeting()
        {
            var listTaskNews = SqlHelper.QuerySP<TaskMeetingViewModel>("sp_Crm_SuccessTaskNew_Get", new
            {
                pCurrentUser = WebSecurity.CurrentUserId
            });
            return PartialView(listTaskNews);
        }
        [AllowAnonymous]
        public ActionResult NotificationsLogs()
        {
            var listTaskNews = SqlHelper.QuerySP<TaskMeetingViewModel>("sp_Crm_SuccessTaskNewZalo_Get");

            return PartialView(listTaskNews);

        }

        [AllowAnonymous]
        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public ActionResult FacebookWebhook()
        {
            // Xác định mã xác nhận token từ Facebook
            var verifyToken = "ConFirm_WebHook_Facebook";

            if (Request.HttpMethod == "GET")
            {
                // Kiểm tra xem yêu cầu từ Facebook có phải là yêu cầu xác nhận webhook không
                if (Request.QueryString["hub.mode"] == "subscribe" &&
                    Request.QueryString["hub.verify_token"] == verifyToken)
                {
                    // Trả về mã xác nhận để Facebook xác nhận đăng ký webhook
                    return Content(Request.QueryString["hub.challenge"], "text/plain");
                }
                else
                {
                    // Nếu không trùng khớp với mã xác nhận token hoặc hub.mode không phải là "subscribe"
                    // hoặc có bất kỳ lỗi nào khác, trả về mã lỗi 403
                    return new HttpStatusCodeResult(403);
                }
            }
            else if (Request.HttpMethod == "POST")
            {
                // Đọc dữ liệu từ body của yêu cầu
                string jsonData = string.Empty;
                using (var reader = new StreamReader(Request.InputStream))
                {
                    jsonData = reader.ReadToEnd();
                }
                if (!string.IsNullOrEmpty(jsonData))
                {
                    try
                    {
                        // Phân tích dữ liệu JSON
                        JObject jsonObject = JObject.Parse(jsonData);
                        string item = jsonObject["entry"][0]["changes"][0]["value"]["item"].ToString();
                        //var expiredToken = SqlHelper.QuerySP<DateTime>("sp_Crm_ExpiredTokenZalo_Get", new
                        //{
                        //}).FirstOrDefault();
                        //&& expiredToken > DateTime.Now
                        if (!string.IsNullOrEmpty(item) && item.Contains("comment"))
                        {
                            string entryId = jsonObject["entry"][0]["id"].ToString();
                            string fromId = jsonObject["entry"][0]["changes"][0]["value"]["from"]["id"].ToString();
                            string fromName = jsonObject["entry"][0]["changes"][0]["value"]["from"]["name"].ToString();
                            string permalinkUrl = jsonObject["entry"][0]["changes"][0]["value"]["post"]["permalink_url"].ToString();
                            string message = jsonObject["entry"][0]["changes"][0]["value"]["message"].ToString();
                            string postId = jsonObject["entry"][0]["changes"][0]["value"]["post_id"].ToString();
                            string commentId = jsonObject["entry"][0]["changes"][0]["value"]["comment_id"].ToString();
                            var resultOfInsert = SqlHelper.QuerySP<dynamic>("sp_Crm_InsertFaceBookCustomerInfoLeadAndLeadLogs", new
                            {
                                pUserIdFacebook = fromId,
                                pName = fromName,
                                pMobile = "",
                                pCity = "",
                                pDistrict = "",
                                pAddress = "",
                                pMgsComment = message,
                                pPermalinkUrl = permalinkUrl
                            }).FirstOrDefault();

                            if (resultOfInsert.leadId > 0)
                            {

                                var hubContext = Microsoft.AspNet.SignalR.GlobalHost.ConnectionManager.GetHubContext<MeetingHub>();
                                message = fromName + ": " + message;
                                var listUserIdhaveCareFacebook = SqlHelper.QuerySP<string>("[Crm_UsersCareFacebook_Get]", new
                                {
                                }).ToList();
                                hubContext.Clients.Clients(listUserIdhaveCareFacebook).notificationOfFacebook(resultOfInsert.leadId, message, permalinkUrl, resultOfInsert.taskNewId);

                                //hubContext.Clients.All.notificationOfFacebook(resultOfInsert.leadId, message, permalinkUrl, resultOfInsert.taskNewId);
                                hubContext.Clients.All.upadteListNotificationOfLeadTaskCall();
                            }
                            return new HttpStatusCodeResult(200);
                        }
                        return new HttpStatusCodeResult(500);
                    }
                    catch (Exception ex)
                    {
                        return new HttpStatusCodeResult(500);
                    }
                }
                return new HttpStatusCodeResult(400);
            }
            else
            {
                return new HttpStatusCodeResult(405);
            }
        }
        [AllowAnonymous]
        public ActionResult LoginFacebook()
        {

            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> DeleteWebhooks()
        {
            try
            {
                // Lấy dữ liệu fanpage từ localStorage
                var storedFanpageInfo = HttpContext.Request.Form["fanpageInfoMap"];
                if (!string.IsNullOrEmpty(storedFanpageInfo))
                {
                    // Chuyển đổi chuỗi JSON thành một đối tượng Dictionary<string, Dictionary<string, string>>
                    var fanpageInfoMap = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, string>>>(storedFanpageInfo);

                    // Lặp qua thông tin fanpage và thực hiện xóa webhook cho từng fanpage
                    foreach (var fanpage in fanpageInfoMap)
                    {
                        // Truy cập giá trị id và accessToken từ đối tượng fanpage.Value
                        var id = fanpage.Value["id"];
                        var accessToken = fanpage.Value["accessToken"];

                        var subscribedFields = new List<string>
                        {
                            "feed",
                            "messages",
                            "message_reactions",
                            "messaging_customer_information",
                            "message_deliveries",
                            "message_echoes",
                            "message_edits",
                            "message_reads",
                            "messaging_optins",
                            "messaging_policy_enforcement",
                            "messaging_postbacks"
                        };

                        var webhookUrl = $"https://graph.facebook.com/{id}/subscribed_apps?subscribed_fields={string.Join(",", subscribedFields)}&access_token={accessToken}";
                        var setExpiredAccessTokenFacebook = SqlHelper.QuerySP<int>("sp_Crm_AccessTokenFacebook_Update", new
                        {
                        }).FirstOrDefault();
                        using (var client = new HttpClient())
                        {
                            var response = await client.DeleteAsync(webhookUrl);
                            if (!response.IsSuccessStatusCode && setExpiredAccessTokenFacebook > 1)
                            {
                                // Trả về lỗi khi không thể xóa webhook cho fanpage
                                return Json(new { error = true, message = "Xóa webhook bị lỗi." });
                            }
                        }
                    }

                }
                else
                {
                    // Trả về thông báo khi không có dữ liệu fanpage trong localStorage
                    return Json(new { error = true, message = "Không có dữ liệu Fanpage " });
                }
                return Json(new { error = false, message = "Xóa thành công." });
            }

            catch (Exception ex)
            {
                // Trả về lỗi chung nếu có bất kỳ lỗi nào xảy ra
                return Json(new { error = true, message = "Lỗi server." });
            }
        }
        [HttpPost]
        [AllowAnonymous]
        public ActionResult InsertAccessTokenFacebook(string accessToken)
        {
            // Định dạng chuỗi ngày giờ
            string dateFormat = "yyyy-MM-dd HH:mm:ss";
            DateTime expireDateTime;
            //if (DateTime.TryParseExact(expireAt, dateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out expireDateTime))
            if(accessToken != null)
            {

                var responseInsertAccessTokenFacebook = SqlHelper.QuerySP<int>("sp_Crm_InsertAccessTokenFacebook", new
                {
                    Token = accessToken,
                }).FirstOrDefault();


                return Json(new { error = false, message = "Insert AccessToken thành công." });
            }
            else
            {

                return Json(new { error = true, message = "Định dạng ngày giờ không hợp lệ." });
            }
        }
        [HttpGet]
        [AllowAnonymous]
        public ActionResult GetExpiredAccessTokenFacebook()
        {
            //var expiredToken = SqlHelper.QuerySP<DateTime>("sp_Crm_ExpiredTokenZalo_Get", new { }).FirstOrDefault();
            //if (expiredToken > DateTime.Now)
            //{
            //    var token = SqlHelper.QuerySP<string>("sp_Crm_TokenFacebook_Get", new { }).FirstOrDefault();
            //    return Json(new { error = false, message = "Còn hạn", token = token }, JsonRequestBehavior.AllowGet);
            //}
            //else
            //{
            //    return Json(new { error = true, message = "Hết hạn" }, JsonRequestBehavior.AllowGet);
            //}

            var result = SqlHelper.QuerySP("sp_Crm_TokenFacebook_Get", new { }).FirstOrDefault();

            if (result != null)
            {
                // Lấy trạng thái của token từ kết quả
                bool isActive = Convert.ToBoolean(result.IsActive);

                string message;
                if (isActive)
                {
                    message = "Còn hạn";
                }
                else
                {
                    message = "Hết hạn";
                }

                // Tạo một Dictionary để lưu trữ thông tin token và trạng thái
                var tokenInfo = new Dictionary<string, object>
                {
                    { "token", result.Token },
                    { "isActive", isActive }
                };

                // Trả về thông tin token và trạng thái dưới dạng JSON
                return Json(new { error = false, message = message, tokenInfo = tokenInfo }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { error = true, message = "Không tìm thấy token" }, JsonRequestBehavior.AllowGet);
            }


        }
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> ConvertToLongLivedTokenFacebook(string shortAccessToken)
        {
            string client_id = ConfigurationManager.AppSettings["Facebook:AppId"];
            string client_secret = ConfigurationManager.AppSettings["Facebook:ClientSecret"];
            try
            {
                // Thực hiện request để lấy long-lived access token từ short-lived access token
                using (var httpClient = new HttpClient())
                {
                    
                    // Gửi yêu cầu đến Graph API để chuyển đổi short-lived access token thành long-lived access token
                    var response = await httpClient.GetAsync($"https://graph.facebook.com/v19.0/oauth/access_token?grant_type=fb_exchange_token&client_id={client_id}&client_secret={client_secret}&fb_exchange_token={shortAccessToken}");

                    // Xác nhận kết quả
                    response.EnsureSuccessStatusCode();

                    // Đọc và parse response
                    var responseData = await response.Content.ReadAsAsync<dynamic>();

                    // Lấy long-lived access token từ response
                    var longLivedAccessToken = responseData.access_token;
                    InsertAccessTokenFacebook2(longLivedAccessToken.ToString());
                    return Content(longLivedAccessToken.ToString());
                }
            }
            catch (Exception ex)
            {
                // Xử lý lỗi nếu có
                return Content($"Error converting to long-lived token: {ex.Message}");
            }
        }

        public void InsertAccessTokenFacebook2(string accessToken)
        {
            if (accessToken != null)
            {

                var responseInsertAccessTokenFacebook = SqlHelper.QuerySP<int>("sp_Crm_InsertAccessTokenFacebook", new
                {
                    Token = accessToken,
                }).FirstOrDefault();


            }
        }
        [HttpGet]
        [AllowAnonymous]
        public ActionResult UpdateActiveAcessToken()
        {
            var result = SqlHelper.QuerySP<int>("sp_Crm_ActiveAccessTokenFacebook_Updadte", new { }).FirstOrDefault();

            return Json(new { error = true, message = "Upadte thành công Active AcessToken" }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        [AllowAnonymous]
        public ActionResult UpdateIsDeletedTask(int idTask)
        {
            var hubContext = Microsoft.AspNet.SignalR.GlobalHost.ConnectionManager.GetHubContext<MeetingHub>();

            if (idTask > 0)
            {
                var result = SqlHelper.QuerySP<int>("sp_Crm_UpdateIsDeletedTask_Updadte", new
                {
                    pIdTask = idTask
                }).FirstOrDefault();
                if(result > 0)
                {
                    hubContext.Clients.All.upadteListNotificationOfLeadTaskCall();
                    return Json(new { error = false, message = "Upadate thành công" });
                }
            }
            return Json(new { error = true, message = "Upadate không thành công" });
        }
        [Authorize]
        public ActionResult QuickLeadSearch()
        {
            return View();
        }
        
    }
}
