using System.Globalization;
using Erp.BackOffice.Crm.Models;
using Erp.BackOffice.Filters;
using Erp.Domain.Entities;
using Erp.Domain.Interfaces;
using Erp.Domain.Crm.Entities;
using Erp.Domain.Crm.Interfaces;
using Erp.Domain.Account.Entities;
using Erp.Domain.Account.Interfaces;
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
using Erp.Domain.Sale.Interfaces;
using Erp.Domain.Account.Helper;
using Erp.BackOffice.Sale.Models;
using System.Web;
using OneSignal.CSharp.SDK;
using Push_Notification_OneSignal;
using OneSignal.CSharp.SDK.Resources;
using System.Transactions;
using Erp.BackOffice.Account.Models;

namespace Erp.BackOffice.Crm.Controllers
{
    [Authorize]
    [InitializeSimpleMembership]
    [Erp.BackOffice.Helpers.NoCacheHelper]
    public class TaskController : Controller
    {
        private readonly ITaskRepository TaskRepository;
        private readonly IUserRepository userRepository;
        private readonly IUserTypeRepository userTypeRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IServiceScheduleRepository serviceScheduleRepository;
        private readonly ISendNotifRepository SendNotifRepository;
        private readonly IListNotiRepository ListNotiRepository;
        private readonly ICustomerRepository CustomerRepository;
        private readonly ITemplatePrintRepository templatePrintRepository;

        public TaskController(
            ITaskRepository _Task
            , IUserRepository _user
              , IUserTypeRepository userType
            , ICategoryRepository category
            , IServiceScheduleRepository serviceSchedule
             , ISendNotifRepository _SendNotif
            , IListNotiRepository ListNoti
            ,ICustomerRepository Customer
            , ITemplatePrintRepository _templatePrint
            )
        {
            TaskRepository = _Task;
            userRepository = _user;
            userTypeRepository = userType;
            _categoryRepository = category;
            serviceScheduleRepository = serviceSchedule;
            SendNotifRepository = _SendNotif;
            ListNotiRepository = ListNoti;
            CustomerRepository = Customer;
            templatePrintRepository = _templatePrint;
        }

        #region Index

        public ViewResult Index(string status, string Priority, int? CreateId, int? AssignId)
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
            var start_date = Request["startDate"];
            var end_date = Request["endDate"];
            IQueryable<TaskViewModel> q = TaskRepository.GetAllvwTaskFull()
                .Where(x => x.Type == "task")
                .Select(item => new TaskViewModel
                {
                    Id = item.Id,
                    CreatedUserId = item.CreatedUserId,
                    //CreatedUserName = item.CreatedUserName,
                    CreatedDate = item.CreatedDate,
                    ModifiedUserId = item.ModifiedUserId,
                    //ModifiedUserName = item.ModifiedUserName,
                    ModifiedDate = item.ModifiedDate,
                    Subject = item.Subject,
                    Status = item.Status,
                    Priority = item.Priority,
                    ParentType = item.ParentType,
                    ParentId = item.ParentId,
                    AssignedUserId = item.AssignedUserId,
                    ProfileImage = item.ProfileImage,
                    FullName = item.FullName,
                    Type = item.Type,
                    Note = item.Note,
                    ReceiverImage = item.ReceiverImage,
                    ReceiverName = item.ReceiverName,
                    ReceiverUser = item.ReceiverUser,
                    StartDate = item.StartDate
                }).OrderByDescending(m => m.ModifiedDate);
            bool bIsSearch = false;
            if (!string.IsNullOrEmpty(start_date) && !string.IsNullOrEmpty(end_date))
            {
                DateTime start_d;
                if (DateTime.TryParseExact(start_date, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out start_d))
                {
                    DateTime end_d;
                    if (DateTime.TryParseExact(end_date, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out end_d))
                    {
                        end_d = end_d.AddHours(23).AddMinutes(59).AddSeconds(59);
                        q = q.Where(x => start_d <= x.CreatedDate && x.CreatedDate <= end_d);
                        bIsSearch = true;
                    }
                }
            }

            if (!string.IsNullOrEmpty(status))
            {
                q = q.Where(item => item.Status == status);
                bIsSearch = true;
            }
            if (!string.IsNullOrEmpty(Priority))
            {
                q = q.Where(item => item.Priority == Priority);
                bIsSearch = true;
            }
            if (CreateId != null && CreateId.Value > 0)
            {
                q = q.Where(item => item.CreatedUserId == CreateId);
                bIsSearch = true;
            }
            if (AssignId != null && AssignId.Value > 0)
            {
                q = q.Where(item => item.AssignedUserId == AssignId);
                bIsSearch = true;
            }
            //if (!Filters.SecurityFilter.IsAdmin() )
            //{
            //    q = q.Where(item => item.AssignedUserId == WebSecurity.CurrentUserId || item.CreatedUserId == WebSecurity.CurrentUserId);
            //}

            var dataNhanvien = SqlHelper.QuerySP<ManagerStaff>("spgetNhanvienbacap", new
            {
                BranchId = (intBrandID == null ? Convert.ToInt32(Session["GlobalCurentBranchId"]) : intBrandID),
                UserId = WebSecurity.CurrentUserId,
            }).ToList();

            List<int> listNhanvien = new List<int>();


            for (int i = 0; i < dataNhanvien.Count; i++)
            {
                listNhanvien.Add(int.Parse(dataNhanvien[i].Id.ToString()));
            }
            //q = q.Where(item => listNhanvien.Contains(item.AssignedUserId.Value) || listNhanvien.Contains(item.CreatedUserId.Value));

            if (!Filters.SecurityFilter.IsAdmin() && (u.UserTypeId != 1027 || u.UserTypeId != 2053 || u.UserTypeId != 2041))
            {
                q = q.Where(item => listNhanvien.Contains(item.AssignedUserId.Value) || listNhanvien.Contains(item.CreatedUserId.Value));
            }
            if (Filters.SecurityFilter.IsAdmin() && (u.UserTypeId != 1027 || u.UserTypeId != 2053 || u.UserTypeId != 2041))
            {
                q = q.Where(item => listNhanvien.Contains(item.AssignedUserId.Value) || listNhanvien.Contains(item.CreatedUserId.Value));
            }
            ViewBag.Search = bIsSearch;
            ViewBag.SuccessMessage = TempData["SuccessMessage"];
            ViewBag.FailedMessage = TempData["FailedMessage"];
            ViewBag.AlertMessage = TempData["AlertMessage"];
            return View(q);
        }

        public ViewResult MyTasks()
        {
            IQueryable<TaskViewModel> q = TaskRepository.GetAllTask()
                .Where(item => item.AssignedUserId == WebSecurity.CurrentUserId
                && item.Status != "Deferred"
                && item.Status != "Completed")
                .Select(item => new TaskViewModel
                {
                    Id = item.Id,
                    CreatedUserId = item.CreatedUserId,
                    //CreatedUserName = item.CreatedUserName,
                    CreatedDate = item.CreatedDate,
                    ModifiedUserId = item.ModifiedUserId,
                    //ModifiedUserName = item.ModifiedUserName,
                    ModifiedDate = item.ModifiedDate,
                    Subject = item.Subject,
                    Status = item.Status
                }).OrderByDescending(m => m.ModifiedDate);
            return View(q);
        }
        #endregion

        #region CreateOfUser
        public ViewResult CreateOfUser(DateTime date)
        {
            var model = new TaskViewModel();
            model.StartDate = date;
            model.DueDate = date;
            return View(model);
        }

        [HttpPost]
        public ActionResult CreateOfUser(TaskViewModel model)
        {
            var urlRefer = Request["UrlReferrer"];
            if (ModelState.IsValid)
            {
                var Task = new Task();
                AutoMapper.Mapper.Map(model, Task);
                Task.IsDeleted = false;
                Task.CreatedUserId = WebSecurity.CurrentUserId;
                Task.ModifiedUserId = WebSecurity.CurrentUserId;
                Task.AssignedUserId = WebSecurity.CurrentUserId;
                Task.CreatedDate = DateTime.Now;
                Task.ModifiedDate = DateTime.Now;
                Task.Status = "pending";
                Task.ParentType = "Task";
                Task.Type = "task";
                TaskRepository.InsertTask(Task);
                Task.ParentId = Task.Id;
                TaskRepository.UpdateTask(Task);

                TempData[Globals.SuccessMessageKey] = App_GlobalResources.Wording.InsertSuccess;
                if (Request["IsPopup"] == "true" || Request["IsPopup"] == "True")
                {
                    TempData[Globals.SuccessMessageKey] = App_GlobalResources.Wording.InsertSuccess;
                    ViewBag.closePopup = "true";
                    model.Id = Task.Id;
                    ViewBag.urlRefer = urlRefer;
                    return View(model);
                }
                return Redirect(urlRefer);
            }
            return View(model);
        }
        #endregion

        #region Create
        public ViewResult Create(int? ParentId)
        //(string ParentType, int? ParentId, string Description, string Subject,DateTime start, DateTime end)
        {
            var model = new TaskViewModel();
            model.ListUser = userRepository.GetAllUsers().Where(x => x.Status == UserStatus.Active ).ToList();
            model.ListUserType = userTypeRepository.GetUserTypes().ToList();
            if (ParentId != null)
            {
                model.ParentId = ParentId;
            }
            else
            {
                model.ParentId = 0;
            }
            //model.ParentType = ParentType;
            //model.Description = Description;
            //model.Subject = Subject;
            ////var date = DateTime.Now;
            //model.StartDate = start;
            //model.DueDate = end;
            return View(model);
        }

        [HttpPost]
        public ActionResult Create(TaskViewModel model)
        {

            model.ListUser = userRepository.GetAllUsers().ToList();
            model.ListUserType = userTypeRepository.GetUserTypes().ToList();
            var urlRefer = Request["UrlReferrer"];
            if (ModelState.IsValid)
            {
                List<string> ListUser = new List<string>();
                if (Request["user_check"] != null)
                {
                    ListUser = Request["user_check"].Split(',').ToList();
                    int tmp = Convert.ToInt32(ListUser.First().ToString());
                    var Schedule = serviceScheduleRepository.GetServiceScheduleById(model.ParentId.Value);
                    if(Schedule != null)
                    {
                        Schedule.AssignedUserId = tmp;
                        serviceScheduleRepository.UpdateServiceSchedule(Schedule);
                    }
                  
                    for (int i = 0; i < ListUser.Count(); i++)
                    {
                        var Task = new Task();
                        AutoMapper.Mapper.Map(model, Task);
                        Task.IsDeleted = false;
                        Task.CreatedUserId = WebSecurity.CurrentUserId;
                        Task.ModifiedUserId = WebSecurity.CurrentUserId;
                        Task.AssignedUserId = Convert.ToInt32(ListUser[i].ToString());
                        Task.CreatedDate = DateTime.Now;
                        Task.ModifiedDate = DateTime.Now;
                        Task.Status = "pending";
                        if (Task.ParentType == null)
                        {
                            Task.ParentType = "Task";
                        }
                        else
                        {
                            Task.ParentType = model.ParentType;
                        }
                        Task.Type = "Task";
                        TaskRepository.InsertTask(Task);
                        if (Task.ParentType == "Task")
                        {
                            Task.ParentId = model.ParentId;
                        }
                        else
                        {
                            Task.ParentId = model.ParentId;
                        }
                        TaskRepository.UpdateTask(Task);

                        //gửi notifications cho người được phân quyền.
                        var task = TaskRepository.GetTaskById(Task.Id);
                        ProcessController.Run("Task"
                            , "Create"
                            , Task.Id
                            , Task.AssignedUserId
                            , null
                            , task);
                    }
                    if (Request["IsPopup"] == "true" || Request["IsPopup"] == "True" || Request["IsPopup"] == null)
                    {
                        ViewBag.closePopup = "true";
                        // model.Id = Task.Id;
                        ViewBag.urlRefer = urlRefer;

                        return View(model);
                    }
                    ViewBag.closePopup = "true";
                    // model.Id = Task.Id;
                    ViewBag.urlRefer = urlRefer;
                    return Redirect(urlRefer);
                }
                else
                {
                    TempData[Globals.FailedMessageKey] = App_GlobalResources.Wording.NoUserCheckTask;
                    return View(model);
                }

            }
            return View(model);
        }
        #endregion

        #region Edit
        public ActionResult Edit(int? Id)
        {
            var Task = TaskRepository.GetTaskById(Id.Value);
            if (Task != null && Task.IsDeleted != true)
            {
                var model = new TaskViewModel();
                AutoMapper.Mapper.Map(Task, model);

                return View(model);
            }
            if (Request.UrlReferrer != null)
                return Redirect(Request.UrlReferrer.AbsoluteUri);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult Edit(TaskViewModel model)
        {
            var urlRefer = Request["UrlReferrer"];
            if (ModelState.IsValid)
            {
                if (Request["Submit"] == "Save")
                {
                    var Task = TaskRepository.GetTaskById(model.Id);
                    AutoMapper.Mapper.Map(model, Task);
                    Task.ModifiedUserId = WebSecurity.CurrentUserId;
                    Task.ModifiedDate = DateTime.Now;
                    TaskRepository.UpdateTask(Task);

                    TempData[Globals.SuccessMessageKey] = App_GlobalResources.Wording.UpdateSuccess;
                    if (Request["IsPopup"] == "true" || Request["IsPopup"] == "True")
                    {
                        TempData[Globals.SuccessMessageKey] = App_GlobalResources.Wording.InsertSuccess;
                        ViewBag.closePopup = "true";
                        model.Id = Task.Id;
                        ViewBag.urlRefer = urlRefer;
                        return View(model);
                    }
                    return Redirect(urlRefer);
                }

                return View(model);
            }

            return View(model);

            //if (Request.UrlReferrer != null)
            //    return Redirect(Request.UrlReferrer.AbsoluteUri);
            //return RedirectToAction("Index");
        }

        #endregion

        #region Detail
        public ActionResult Detail(int? Id)
        {
            var Task = TaskRepository.GetvwTaskById(Id.Value);
            if (Task != null && Task.IsDeleted != true)
            {
                var model = new TaskViewModel();
                AutoMapper.Mapper.Map(Task, model);

                //if (model.CreatedUserId != Erp.BackOffice.Helpers.Common.CurrentUser.Id && Erp.BackOffice.Helpers.Common.CurrentUser.UserTypeId != 1)
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

        #region Delete
        [HttpPost]
        public ActionResult Delete()
        {
            try
            {
                string idDeleteAll = Request["DeleteId-checkbox"];
                string[] arrDeleteId = idDeleteAll.Split(',');
                for (int i = 0; i < arrDeleteId.Count(); i++)
                {
                    var item = TaskRepository.GetTaskById(int.Parse(arrDeleteId[i], CultureInfo.InvariantCulture));
                    if (item != null)
                    {
                        if (item.CreatedUserId != Erp.BackOffice.Helpers.Common.CurrentUser.Id && Erp.BackOffice.Helpers.Common.CurrentUser.UserTypeId != 1)
                        {
                            TempData["FailedMessage"] = "NotOwner";
                            return RedirectToAction("Index");
                        }

                        item.IsDeleted = true;
                        TaskRepository.UpdateTask(item);
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

        #region CheckSeen
        //[HttpPost]
        public ActionResult CheckSeen(int? Id)
        {
            var notifications = TaskRepository.GetTaskById(Id.Value);
            if (notifications != null)
            {
                if (notifications.AssignedUserId == notifications.ModifiedUserId)
                {
                    notifications.AssignedUserId = 0;
                    TaskRepository.UpdateTask(notifications);
                    return Content("notseen");
                }
                else
                {
                    notifications.AssignedUserId = WebSecurity.CurrentUserId;
                    TaskRepository.UpdateTask(notifications);
                    return Content("success");
                }
            }
            else
            {
                return Content("error");
            }
        }
        #endregion

        #region CheckAllSeen
        //[HttpPost]
        public ActionResult CheckAllSeen()
        {
            var notifications = TaskRepository.GetAllTask().Where(x => x.ModifiedUserId == WebSecurity.CurrentUserId).ToList();
            for (int i = 0; i < notifications.Count(); i++)
            {
                notifications[i].AssignedUserId = WebSecurity.CurrentUserId;
                TaskRepository.UpdateTask(notifications[i]);
            }
            return Content("success");
        }
        #endregion

        #region CheckDisable
        //[HttpPost]
        public ActionResult CheckDisable(int? Id)
        {
            var notifications = TaskRepository.GetTaskFullById(Id.Value);
            //chia làm 2 trường hợp. 1 TH chưa xem thì chuyển thành đã xem và ẩn thông báo đi
            //TH2 đã xem rồi thì chỉ ẩn thông báo thôi.
            if (notifications != null)
            {
                if (notifications.AssignedUserId.Value <= 0)
                {
                    notifications.IsDeleted = true;
                    notifications.AssignedUserId = WebSecurity.CurrentUserId;
                    TaskRepository.UpdateTask(notifications);
                    return Content("notseen");
                }
                else
                {
                    notifications.IsDeleted = true;
                    TaskRepository.UpdateTask(notifications);
                    return Content("seen");
                }
            }
            else
            {
                return Content("error");
            }
        }
        #endregion

        #region UpdateTask

        public ActionResult UpdateTask(int? Id)
        {
            var task = TaskRepository.GetvwTaskById(Id.Value);
            if (task != null && task.IsDeleted != true)
            {
                var model = new TaskViewModel();
                AutoMapper.Mapper.Map(task, model);
                return View(model);
            }
            if (Request.UrlReferrer != null)
                return Redirect(Request.UrlReferrer.AbsoluteUri);
            return RedirectToAction("Index");
        }
        [HttpPost]
        public ActionResult UpdateTask(TaskViewModel model)
        {
            var urlRefer = Request["UrlReferrer"];
            var Task = TaskRepository.GetTaskById(model.Id);
            //   AutoMapper.Mapper.Map(model, Task);
            Task.Subject = model.Subject;
            Task.Description = model.Description;
            Task.Priority = model.Priority;
            Task.Note = model.Note;
            Task.StartDate = model.StartDate;
            Task.DueDate = model.DueDate;
            Task.Status = model.Status;
            Task.ModifiedUserId = WebSecurity.CurrentUserId;
            Task.ModifiedDate = DateTime.Now;
            TaskRepository.UpdateTask(Task);

            TempData[Globals.SuccessMessageKey] = App_GlobalResources.Wording.UpdateSuccess;
            if (Request["IsPopup"] == "true" || Request["IsPopup"] == "True")
            {
                ViewBag.closePopup = "true";
                model.Id = Task.Id;
                ViewBag.urlRefer = urlRefer;
                return View(model);
            }
            return Redirect(urlRefer);
        }
        #endregion

        public ActionResult LogNotifications()
        {

            IEnumerable<TaskMeetingViewModel> listTaskNews = null;
            listTaskNews = SqlHelper.QuerySP<TaskMeetingViewModel>("sp_Crm_SuccessTaskNewZalo_Get");

            return PartialView(listTaskNews);
        }

        #region Calendar
        public ViewResult Calendar(string status_check, int? month, int? year)
        {
            var user = userRepository.GetUserById(WebSecurity.CurrentUserId);
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
            List<string> ListCheck = new List<string>();
            if (!string.IsNullOrEmpty(status_check))
            {
                ListCheck = status_check.Split(',').ToList();
            }
            else
            {
                ListCheck = "pending,inprogress".Split(',').ToList();
            }
            DateTime aDateTime = new DateTime(year.Value, month.Value, 1);
            // Cộng thêm 1 tháng và trừ đi một ngày.
            DateTime retDateTime = aDateTime.AddMonths(1).AddDays(-1);
            //List<TaskViewModel> q = new List<TaskViewModel>();
            IEnumerable<TaskViewModel> q = TaskRepository.GetAllvwTask()
                .Where(item => item.Type == "task" && item.CreatedDate >=aDateTime && item.CreatedDate <= retDateTime && ListCheck.Contains(item.Status))
                .Select(i => new TaskViewModel
                {
                    Description = i.Description,
                    CreatedDate = i.CreatedDate,
                    DueDate = i.DueDate,
                    FullName = i.FullName,
                    Id = i.Id,
                    ModifiedDate = i.ModifiedDate,
                    Note = i.Note,
                    ParentId = i.ParentId,
                    ParentType = i.ParentType,
                    Priority = i.Priority,
                    StartDate = i.StartDate,
                    Status = i.Status,
                    CreatedUserId = i.CreatedUserId,
                    ProfileImage = i.ProfileImage,
                    Subject = i.Subject,
                    Type = i.Type,
                    UserName = i.UserName,
                    ContactId = i.ContactId,
                    AssignedUserId = i.AssignedUserId
                }).OrderByDescending(x => x.CreatedDate).ToList();
            //for (int i = 0; i < ListCheck.Count(); i++)
            //{
            //    var a = model.Where(x => x.Status == ListCheck[i].ToString());
            //    q = q.Union(a).ToList();
            //}
            //&& item.AssignedUserId == user.Id
            //IQueryable<TaskViewModel> q = TaskRepository.GetAllvwTaskFull()
            //    .Where(x => x.Type == "task")
            //    .Select(item => new TaskViewModel
            //    {
            //        Id = item.Id,
            //        CreatedUserId = item.CreatedUserId,
            //        //CreatedUserName = item.CreatedUserName,
            //        CreatedDate = item.CreatedDate,
            //        ModifiedUserId = item.ModifiedUserId,
            //        //ModifiedUserName = item.ModifiedUserName,
            //        ModifiedDate = item.ModifiedDate,
            //        Subject = item.Subject,
            //        Status = item.Status,
            //        Priority = item.Priority,
            //        ParentType = item.ParentType,
            //        ParentId = item.ParentId,
            //        AssignedUserId = item.AssignedUserId,
            //        ProfileImage = item.ProfileImage,
            //        FullName = item.FullName,
            //        Type = item.Type,
            //        Note = item.Note,
            //        ReceiverImage = item.ReceiverImage,
            //        ReceiverName = item.ReceiverName,
            //        ReceiverUser = item.ReceiverUser,
            //        StartDate = item.StartDate
            //    }).Where(x => ListCheck.Contains(x.Status)).OrderByDescending(m => m.ModifiedDate);

            //q = q.Where(x => aDateTime <= x.CreatedDate && x.CreatedDate <= retDateTime);
            //q = q.Where(x => aDateTime <= x.StartDate.Value && x.StartDate.Value <= retDateTime).OrderBy(x => x.StartDate).ToList();
            var dataNhanvien = SqlHelper.QuerySP<ManagerStaff>("spgetNhanvienbacap", new
            {
                BranchId = (intBrandID == null ? Convert.ToInt32(Session["GlobalCurentBranchId"]) : intBrandID),
                UserId = WebSecurity.CurrentUserId,
            }).ToList();

            List<int?> listNhanvien = new List<int?>();


            for (int i = 0; i < dataNhanvien.Count; i++)
            {
                listNhanvien.Add(int.Parse(dataNhanvien[i].Id.ToString()));
            }
            //q = q.Where(item => listNhanvien.Contains(item.AssignedUserId.Value) || listNhanvien.Contains(item.CreatedUserId.Value));

            if (!Filters.SecurityFilter.IsAdmin() && (user.UserTypeId != 1027 || user.UserTypeId != 2053 || user.UserTypeId != 2041))
            {
                q = q.Where(item => listNhanvien.Contains(item.AssignedUserId) || listNhanvien.Contains(item.CreatedUserId)).ToList();
            }
            var category = _categoryRepository.GetCategoryByCode("task_status").Select(x => new CategoryViewModel
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
            if(q != null )
            {
                var dataEvent = q.Select(e => new
                {
                    title = e.Subject,
                    start = e.StartDate.Value.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                    end = e.DueDate.Value.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                    allDay = false,
                    className = (e.Status == "pending" ? "label-info" : (e.Status == "inprogress" ? "label-warning" : (e.Status == "completed" ? "label-success" : "label-danger"))),
                    url = string.Format("/Task/{0}/?Id={1}", (e.Status == "completed" ? "Detail" : "UpdateTask"), e.Id)
                }).ToList();

                ViewBag.dataEvent = new JavaScriptSerializer().Serialize(dataEvent);
            }
            
            ViewBag.aDateTime = aDateTime.ToString("yyyy-MM-dd");

            return View(q);
        }
        #endregion

        #region DeleteTask
        [HttpPost]
        public ActionResult DeleteTask(int? id)
        {
            try
            {
                var item = TaskRepository.GetTaskById(id.Value);
                if (item != null)
                {
                    TaskRepository.DeleteTask(item.Id);
                }
                return Content("success");
            }
            catch (DbUpdateException)
            {
                return Content("error");
            }
        }
        #endregion



        public ActionResult SendNotif()
        {
            return View();
        }

        [HttpPost]
        public ActionResult SendNotif(string body, string title)
        {
            //string pMA_DVIQLY = Helpers.Common.CurrentUser.MA_DVIQLY;
            string APIKEY = Helpers.Common.GetSetting("OneSignalKey");
            string APPID = Helpers.Common.GetSetting("OneSignalApp");


            var Noti = new SendNotif();
            Noti.CreatedDate = DateTime.Now;
            Noti.Body = body;
            Noti.Title = title;
            Noti.IsBySeen = false;

            SendNotifRepository.InsertSendNotif(Noti);

            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            //var IncludePlayerIds = new List<string>();

            var client = new OneSignalClient(APIKEY); // Nhập API Serect Key

            var options = new NotificationCreateOptions_new();


            options.AppId = new Guid(APPID); // Nhập My AppID của bạn
            options.IncludedSegments = new List<string> { "All" };
            options.DeliverToChromeWeb = true;
            options.DeliverToAndroid = true;
            options.DeliverToIos = true;
            options.DeliverToFirefox = true;
            dictionary.Add("IdNoti", Noti.Id.ToString());
            options.Data = dictionary;
            options.Headings.Add(LanguageCodes.English, title);
            options.Contents.Add(LanguageCodes.English, body);


            //options.Url = txtLaunchURL.Text;
            //options.ChromeWebImage = txtImageChrome.Text;

            //options.IncludePlayerIds = IncludePlayerIds;
            var result = client.Notifications.Create(options);
            Erp.BackOffice.Controllers.HomeController.WriteLog(Noti.Id, Noti.Title, "đã gửi thông báo ", "ListNoti/Edit/" + Noti.Id, Helpers.Common.CurrentUser.BranchId.Value);
            ViewBag.SuccessMessage = "Đã gửi thành công!";
            return View();

        }


        public ActionResult CreateNoti(int? Id,bool? IsPopup) {

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

            var model = new ListNotiViewModel();
            model.BranchId = intBrandID;
            if (Id != null)
            {
                var ListNoti = ListNotiRepository.GetListNotiById(Id.Value);
                AutoMapper.Mapper.Map(ListNoti, model);
                if (!String.IsNullOrEmpty(ListNoti.ApplyUser))
                {
                    var IdUser = ListNoti.ApplyUser.Split(',');
                    

                    if (ListNoti.ApplyFor == "2")
                    {
                        model.NhomDS = IdUser.ToList();
                    }
                    else if (ListNoti.ApplyFor == "3")
                    {
                        model.Nhanvien = IdUser.ToList();
                    }
                    else if (ListNoti.ApplyFor == "4")
                    {
                        model.KhachHang = IdUser.ToList();
                    }
                }

            }
            

            

            return View(model);
        }

       
        [HttpPost]
        public ActionResult CreateNoti(ListNotiViewModel model)
        {
            var ApplyFor = Request["single"] != "" ? Request["single"] : "1";
            string Ispopup = Request["IsPopup"];
            if (ModelState.IsValid)
            {
                using (var scope = new TransactionScope(TransactionScopeOption.Required, new System.TimeSpan(0, 30, 0)))
                {
                    if(model.Id> 0)
                    {
                        var Noti = ListNotiRepository.GetListNotiById(model.Id);
                        // Noti.IsDeleted = false;
                        // Noti.IsSend = false;
                        //Noti.CreatedDate = DateTime.Now;
                        // Noti.CreatedUserId = WebSecurity.CurrentUserId;
                        Noti.ModifiedDate = DateTime.Now;
                        Noti.ModifiedUserId = WebSecurity.CurrentUserId;
                        Noti.Body = model.Body;
                        Noti.BranchId = model.BranchId;
                        Noti.Title = model.Title;
                        Noti.ApplyFor = ApplyFor;
                        if (Noti.ApplyFor == "2")
                        {
                            Noti.ApplyUser = string.Join(",", model.NhomDS.ToArray());
                        }
                        if (Noti.ApplyFor == "3")
                        {
                            Noti.ApplyUser = string.Join(",", model.Nhanvien.ToArray());
                        }
                        if (Noti.ApplyFor == "4")
                        {
                            Noti.ApplyUser = string.Join(",", model.KhachHang.ToArray());
                        }
                        ListNotiRepository.UpdateListNoti(Noti);
                        Erp.BackOffice.Controllers.HomeController.WriteLog(Noti.Id, Noti.Title, "đã gửi thông báo ", "ListNoti/Edit/" + Noti.Id, Helpers.Common.CurrentUser.BranchId.Value);
                        TempData[Globals.SuccessMessageKey] = "Đã cập nhật thành công!";
                    }
                    else
                    {
                        var Noti = new ListNoti();
                        Noti.IsDeleted = false;
                        Noti.IsSend = false;
                        Noti.CreatedDate = DateTime.Now;
                        Noti.CreatedUserId = WebSecurity.CurrentUserId;
                        Noti.Body = model.Body;
                        Noti.BranchId = model.BranchId;
                        Noti.Title = model.Title;
                        Noti.ApplyFor = ApplyFor;
                        if (Noti.ApplyFor == "2")
                        {
                            Noti.ApplyUser = string.Join(",", model.NhomDS.ToArray());
                        }
                        if (Noti.ApplyFor == "3")
                        {
                            Noti.ApplyUser = string.Join(",", model.Nhanvien.ToArray());
                        }
                        if (Noti.ApplyFor == "4")
                        {
                            Noti.ApplyUser = string.Join(",", model.KhachHang.ToArray());
                        }
                        ListNotiRepository.InsertListNoti(Noti);
                        Erp.BackOffice.Controllers.HomeController.WriteLog(Noti.Id, Noti.Title, "đã gửi thông báo ", "ListNoti/Edit/" + Noti.Id, Helpers.Common.CurrentUser.BranchId.Value);
                        TempData[Globals.SuccessMessageKey] = "Đã tạo thành công!";
                    }
                   
                    scope.Complete();
                }
            }
            if(Ispopup == "True")
            {
                return RedirectToAction("_ClosePopup", "Home", new { area = "", FunctionCallback = "ClosePopupAndReloadPage()" });
            }
            return RedirectToAction("NotiIndex");
        }


        public ViewResult NotiIndex(string txtSearch,int? Status1)
        {
            IEnumerable<ListNotiViewModel> q = ListNotiRepository.GetAllListNoti()
               .Select(item => new ListNotiViewModel
               {
                   Id = item.Id,
                   CreatedUserId = item.CreatedUserId,

                   CreatedDate = item.CreatedDate,
                   ModifiedUserId = item.ModifiedUserId,

                   ModifiedDate = item.ModifiedDate,
                  BranchId = item.BranchId,
                  Body = item.Body,
                  Title = item.Title,
                  ApplyFor = item.ApplyFor,
                  ApplyUser =item.ApplyUser,
                  IsSend = item.IsSend
               }).OrderByDescending(x => x.CreatedDate).ToList();
            if (string.IsNullOrEmpty(txtSearch) == false)
            {
                txtSearch = txtSearch == "" ? "~" : Erp.BackOffice.Helpers.Common.ChuyenThanhKhongDau(txtSearch);

                q = q.Where(x => Erp.BackOffice.Helpers.Common.ChuyenThanhKhongDau(x.Title).Contains(txtSearch)).ToList();
            }
            if(Status1 ==2 )
            {
                q = q.Where(x => x.IsSend == true);
            }
            if (Status1 == 1)
            {
                q = q.Where(x => x.IsSend == false);
            }


            ViewBag.SuccessMessage = TempData[Globals.SuccessMessageKey];
            ViewBag.FailedMessage = TempData[Globals.FailedMessageKey];
            return View(q);
            
        }
        public List<ListNotiViewModel> IndexExport(string txtSearch, int? Status1)
        {
            var q = ListNotiRepository.GetAllListNoti().AsEnumerable()
                 .Select(item => new ListNotiViewModel
                 {
                     Id = item.Id,
                     CreatedUserId = item.CreatedUserId,

                     CreatedDate = item.CreatedDate,
                     ModifiedUserId = item.ModifiedUserId,

                     ModifiedDate = item.ModifiedDate,
                     BranchId = item.BranchId,
                     Body = item.Body,
                     Title = item.Title,
                     ApplyFor = item.ApplyFor,
                     ApplyUser = item.ApplyUser,
                     IsSend = item.IsSend
                 }).OrderByDescending(m => m.CreatedDate).ToList();

            //var group = q.GroupBy(x => x.CategoryCode).ToList();
            //int dem = 1;
            //foreach (var item in group)
            //{
            //    Category category = new Category();
            //    category.IsDeleted = false;
            //    category.CreatedUserId = WebSecurity.CurrentUserId;
            //    category.ModifiedUserId = Web Security.CurrentUserId;
            //    category.CreatedDate = DateTime.Now;
            //    category.ModifiedDate = DateTime.Now;
            //    category.Value = item.Key;
            //    category.Name = item.Key;
            //    category.Code = "product";
            //    category.OrderNo = dem;
            //    categoryRepository.InsertCategory(category);
            //    dem++;
            //}

            if (string.IsNullOrEmpty(txtSearch) == false)
            {
                txtSearch = txtSearch == "" ? "~" : txtSearch.Trim();

                q = q.Where(x => Erp.BackOffice.Helpers.Common.ChuyenThanhKhongDau(x.Title).Contains(txtSearch)).ToList();
            }
            if (Status1 == 2)
            {
                q = q.Where(x => x.IsSend== true).ToList();

            }
            if (Status1 == 1)
            {
                q = q.Where(x => x.IsSend == false).ToList();
            }
            return q;
        }
        public ActionResult ExportExcel(string Title,int? Status1, bool ExportExcel = false)
        {
            var data = IndexExport(Title, Status1);

            var model = new TemplatePrintViewModel();
            var logo = Erp.BackOffice.Helpers.Common.GetSetting("LogoCompany");
            var company = Erp.BackOffice.Helpers.Common.GetSetting("companyName");
            var address = Erp.BackOffice.Helpers.Common.GetSetting("addresscompany");
            var phone = Erp.BackOffice.Helpers.Common.GetSetting("phonecompany");
            var fax = Erp.BackOffice.Helpers.Common.GetSetting("faxcompany");
            var bankcode = Erp.BackOffice.Helpers.Common.GetSetting("bankcode");
            var bankname = Erp.BackOffice.Helpers.Common.GetSetting("bankname");
            var ImgLogo = "<div class=\"logo\"><img src=" + logo + " height=\"73\" /></div>";
            var template = templatePrintRepository.GetAllTemplatePrint().Where(x => x.Code.Contains("PrintReport")).OrderByDescending(x => x.CreatedDate).FirstOrDefault();
            model.Content = template.Content;
            model.Content = model.Content.Replace("{DataTable}", buildHtml(data));
            model.Content = model.Content.Replace("{System.Logo}", ImgLogo);
            model.Content = model.Content.Replace("{System.CompanyName}", company);
            model.Content = model.Content.Replace("{System.AddressCompany}", address);
            model.Content = model.Content.Replace("{System.PhoneCompany}", phone);
            model.Content = model.Content.Replace("{System.Fax}", fax);
            model.Content = model.Content.Replace("{System.BankCodeCompany}", bankcode);
            model.Content = model.Content.Replace("{System.BankNameCompany}", bankname);
            model.Content = model.Content.Replace("{PrintDate}", DateTime.Now.ToString("dd/MM/yyyy HH:mm"));
            model.Content = model.Content.Replace("{Title}", "Danh sách");
            if (ExportExcel)
            {
                Response.AppendHeader("content-disposition", "attachment;filename=" + "DS_" + DateTime.Now.ToString("yyyyMMdd") + ".xls");
                Response.Charset = "";
                Response.Cache.SetCacheability(HttpCacheability.NoCache);
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.Write(model.Content);
                Response.End();
            }
            return View(model);
        }

        string buildHtml(List<ListNotiViewModel> data)
        {
            //Tạo table html chi tiết phiếu xuất
            string detailLists = "<table border=\"1\" class=\"invoice-detail\">\r\n";
            detailLists += "<thead>\r\n";
            detailLists += "	<tr>\r\n";
            detailLists += "		<th>Tiêu đề</th>\r\n";
            detailLists += "		<th>Nội dung</th>\r\n";
            detailLists += "		<th>Ngày tạo</th>\r\n";
            detailLists += "		<th>Trạng thái</th>\r\n";
            detailLists += "		<th>Ngày cập nhập</th>\r\n";
            detailLists += "	</tr>\r\n";
            detailLists += "</thead>\r\n";
            detailLists += "<tbody>\r\n";
            var index = 1;

            foreach (var item in data)
            {
                detailLists += "<tr>\r\n"
                + "<td class=\"text-center orderNo\">" + (index++) + "</td>\r\n"
                + "<td class=\"text-left \">" + item.Title + "</td>\r\n"
                + "<td class=\"text-left \">" + item.Body + "</td>\r\n"
                + "<td class=\"text-left \">" + item.CreatedDate + "</td>\r\n"
                + "<td class=\"text-left \">" + (item.IsSend == true ? "Đã gửi" :"Chưa gửi") + "</td>\r\n"
                + "<td class=\"text-left \">" + item.ModifiedDate + "</td>\r\n"
                + "</tr>\r\n";
            }

            detailLists += "</tfoot>\r\n</table>\r\n";

            return detailLists;
        }
        public ActionResult Send(int? Id)
        {
            var IncludePlayerIds = new List<string>();
            var ListNoti = ListNotiRepository.GetListNotiById(Id.Value);
            string[] IdUser = null;
            if (ListNoti.ApplyFor == "1")
            {

            }
            else if(ListNoti.ApplyFor == "2")
            {
                IdUser = ListNoti.ApplyUser.Split(',');
                foreach(var item in IdUser)
                {
                    var ID = int.Parse(item, CultureInfo.InvariantCulture);
                    var cus = CustomerRepository.GetAllCustomer().Where(x => x.NhomHuongDS == ID && !String.IsNullOrEmpty(x.PlayId)).ToList();
                    if(cus.Count > 0)
                    {
                        foreach(var i in cus)
                        {
                            IncludePlayerIds.Add(i.PlayId);
                        }
                    }
                }
                    

            }
            else if(ListNoti.ApplyFor == "3")
            {
                IdUser = ListNoti.ApplyUser.Split(',');
                foreach (var item in IdUser)
                {
                    var ID = int.Parse(item, CultureInfo.InvariantCulture);
                    var cus = CustomerRepository.GetAllCustomer().Where(x => x.ManagerStaffId == ID  && !String.IsNullOrEmpty(x.PlayId)).ToList();
                    if (cus.Count > 0)
                    {
                        foreach (var i in cus)
                        {
                            IncludePlayerIds.Add(i.PlayId);
                        }
                    }
                }
            }
            else
            {
                IdUser = ListNoti.ApplyUser.Split(',');
                foreach (var item in IdUser)
                {
                    var ID = int.Parse(item, CultureInfo.InvariantCulture);
                    var cus = CustomerRepository.GetCustomerById(ID);

                    IncludePlayerIds.Add(cus.PlayId);


                }
            }

            string APIKEY = Helpers.Common.GetSetting("OneSignalKey");
            string APPID = Helpers.Common.GetSetting("OneSignalApp");


            var Noti = new SendNotif();
            Noti.CreatedDate = DateTime.Now;
            Noti.Body = ListNoti.Body;
            Noti.Title = ListNoti.Title;
            Noti.IsBySeen = false;

            SendNotifRepository.InsertSendNotif(Noti);

            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            

            var client = new OneSignalClient(APIKEY); // Nhập API Serect Key

            var options = new NotificationCreateOptions_new();


            options.AppId = new Guid(APPID); // Nhập My AppID của bạn
            if(ListNoti.ApplyFor == "1")
            {
                options.IncludedSegments = new List<string> { "All" };
            }
            else
            {
                options.IncludePlayerIds = IncludePlayerIds;
            }
            //options.IncludedSegments = new List<string> { "All" };
            options.DeliverToChromeWeb = true;
            options.DeliverToAndroid = true;
            options.DeliverToIos = true;
            options.DeliverToFirefox = true;
            dictionary.Add("IdNoti", Noti.Id.ToString());
            options.Data = dictionary;
            options.Headings.Add(LanguageCodes.English, ListNoti.Title);
            options.Contents.Add(LanguageCodes.English, ListNoti.Body);


            //options.Url = txtLaunchURL.Text;
            //options.ChromeWebImage = txtImageChrome.Text;

            //options.IncludePlayerIds = IncludePlayerIds;
            var result = client.Notifications.Create(options);
            ListNoti.IsSend = true;
            ListNotiRepository.UpdateListNoti(ListNoti);

            TempData[Globals.SuccessMessageKey] = "Đã gửi thành công!";
            return RedirectToAction("NotiIndex");
        }
    }
}