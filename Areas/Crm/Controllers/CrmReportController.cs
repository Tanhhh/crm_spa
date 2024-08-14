using System.Globalization;
using Erp.BackOffice.Filters;
using Erp.Domain.Interfaces;
using Erp.Domain.Sale.Entities;
using Erp.Domain.Sale.Interfaces;
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
using Erp.BackOffice.Sale.Models;
using Erp.BackOffice.Helpers;
using Erp.Domain.Helper;
using Newtonsoft.Json;
using Erp.Domain.Account.Interfaces;
using Erp.BackOffice.Account.Models;
using System.Data;
using Erp.BackOffice.Crm.Models;
using Erp.BackOffice.Areas.Cms.Models;
using Erp.Domain.Crm.Repositories;
using System.Transactions;

namespace Erp.BackOffice.Crm.Controllers
{
    [Authorize]
    [InitializeSimpleMembership]
    [Erp.BackOffice.Helpers.NoCacheHelper]
    public class CrmReportController : Controller
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IUserRepository userRepository;
        private readonly ILogServiceRemminderRepository logServiceReminderRepository;
        private readonly ICustomerRepository customerRepository;
        private readonly IQueryHelper QueryHelper;
        private readonly ITaskRepository taskRepository;
        private readonly ICrm_LevelRepository Crm_LevelRepository;
        private readonly ICrm_PeriodRepository Crm_PeriodRepository;
        private readonly ICrm_TargetRepository Crm_TargetRepository;
        public CrmReportController(
             ILogServiceRemminderRepository logserviceReminder
            , IUserRepository _user
          , ITaskRepository task
            , IQueryHelper _QueryHelper
             , ICustomerRepository _Customer
               , ICategoryRepository category,
             ICrm_LevelRepository _Crm_Level, 
             ICrm_PeriodRepository _Crm_Period,
             ICrm_TargetRepository _Crm_Target
            )
        {
            logServiceReminderRepository = logserviceReminder;
            customerRepository = _Customer;
            QueryHelper = _QueryHelper;
            userRepository = _user;
            taskRepository = task;
            _categoryRepository = category;
           Crm_LevelRepository = _Crm_Level;
            Crm_PeriodRepository = _Crm_Period;
            Crm_TargetRepository = _Crm_Target;
        }

        #region ServiceReminder
        public ViewResult ServiceReminder(string status_check)
        {
            //danh sách tái khám đến hạn
            //var quantityDate = Dormitory.BackOffice.Helpers.Common.GetSetting("quantity_Reminder_Date");
            //var date = DateTime.Now.AddDays(Convert.ToInt32(quantityDate));
            List<string> ListCheck = new List<string>();
            if (!string.IsNullOrEmpty(status_check))
            {
                ListCheck = status_check.Split(',').ToList();
            }
            else
            {
                ListCheck = "pending,inprogress".Split(',').ToList();
            }
            List<TaskViewModel> q = new List<TaskViewModel>();
            var user = userRepository.GetUserById(WebSecurity.CurrentUserId);
            IEnumerable<TaskViewModel> model = taskRepository.GetAllvwTask().Where(x => x.Type == "task" && x.AssignedUserId == user.Id)
                .Select(item => new TaskViewModel
                {
                    Id = item.Id,
                    CreatedUserId = item.CreatedUserId,
                    CreatedDate = item.CreatedDate,
                    Description = item.Description,
                    DueDate = item.DueDate,
                    IsDeleted = item.IsDeleted,
                    Note = item.Note,
                    ParentId = item.ParentId,
                    ParentType = item.ParentType,
                    Priority = item.Priority,
                    StartDate = item.StartDate,
                    Status = item.Status,
                    Subject = item.Subject
                }).ToList();
            //q = q.Where(x => DateTime.Now <= x.DueDate && x.DueDate <= date);

            for (int i = 0; i < ListCheck.Count(); i++)
            {
                var a = model.Where(x => x.Status == ListCheck[i].ToString());
                q = q.Union(a).ToList();
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
            return View(q);
        }
        #endregion


        #region LEVEL
        public ViewResult Index()
        {
            IEnumerable<Crm_LevelViewModel> q = Crm_LevelRepository.GetAllCrm_Level()
                .Select(item => new Crm_LevelViewModel
                {
                    Id = item.Id,
                    IsDeleted = item.IsDeleted,
                    Name = item.Name,
                    Level = item.Level,
                    StaredIndex = item.StaredIndex,
                    EndIndex = item.EndIndex
                });
            var model = q.ToList();
            ViewBag.SuccessMessage = TempData["SuccessMessage"];
            ViewBag.FailedMessage = TempData["FailedMessage"];
            ViewBag.AlertMessage = TempData["AlertMessage"];
            return View(model);
        }

        public ViewResult CreateCrm_Level()
        {
            var model = new Crm_LevelViewModel();

            //var stt = Crm_LevelRepository.GetAllCrm_Level().ToList();

            return View(model);
        }

        [HttpPost]
        public ActionResult CreateCrm_Level( Crm_LevelViewModel item)
        {
            var LoyaltyPoint = new Crm_Level();
            AutoMapper.Mapper.Map(item, LoyaltyPoint);
            LoyaltyPoint.IsDeleted = false;
            //.AssignedUserId = WebSecurity.CurrentUserId;

            Crm_LevelRepository.InsertCrm_Level(LoyaltyPoint);

            
            TempData[Globals.SuccessMessageKey] = App_GlobalResources.Wording.InsertSuccess;
            return RedirectToAction("Index");
        }

        public ActionResult EditCrm_Level(int? Id)
        {
            var LoyaltyPoint = Crm_LevelRepository.GetCrm_LevelById(Id.Value);
            if (LoyaltyPoint != null && LoyaltyPoint.IsDeleted != true)
            {
                var model = new Crm_LevelViewModel();
                AutoMapper.Mapper.Map(LoyaltyPoint, model);

                if (model.Id != Helpers.Common.CurrentUser.Id && Helpers.Common.CurrentUser.UserTypeId != 1)
                {
                    TempData["FailedMessage"] = "NotOwner";
                    return RedirectToAction("Index");
                }

                return View(model);
            }
            if (Request.UrlReferrer != null)
                return Redirect(Request.UrlReferrer.AbsoluteUri);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult EditCrm_Level(Crm_LevelViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (Request["Submit"] == "Save")
                {
                    var LoyaltyPoint = Crm_LevelRepository.GetCrm_LevelById(model.Id);
                    LoyaltyPoint.Name = model.Name;
                    LoyaltyPoint.Level = model.Level;
                    LoyaltyPoint.StaredIndex = model.StaredIndex;
                    LoyaltyPoint.EndIndex = model.EndIndex;

                    Crm_LevelRepository.UpdateCrm_Level(LoyaltyPoint);

                    TempData[Globals.SuccessMessageKey] = App_GlobalResources.Wording.UpdateSuccess;
                    return RedirectToAction("Index");
                }



                return View(model);
            }

            return View(model);

            //if (Request.UrlReferrer != null)
            //    return Redirect(Request.UrlReferrer.AbsoluteUri);
            //return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult DeleteCrm_Level()
        {
           
            try
            {
                string id = Request["Delete"];
                if (id != null)
                {
                    var item = Crm_LevelRepository.GetCrm_LevelById(int.Parse(id, CultureInfo.InvariantCulture));
                    if (item != null)
                    {
                        if (item.Id != Helpers.Common.CurrentUser.Id && Helpers.Common.CurrentUser.UserTypeId != 1)
                        {
                            TempData["FailedMessage"] = "NotOwner";
                            return RedirectToAction("Index");
                        }
                        var tar = Crm_TargetRepository.GetAllCrm_Target().ToList();
                        //List<Crm_TargetViewModel> listTar = new List<Crm_TargetViewModel>();
                        //AutoMapper.Mapper.Map(tar, listTar); 
                        foreach(var t in tar)
                        {
                            if(t.IdLevel == item.Id)
                            {
                                TempData["FailedMessage"] = "Giá trị đang được sử dụng!";
                                return RedirectToAction("Index");
                            }
                        }                        
                        item.IsDeleted = true;
                        Crm_LevelRepository.UpdateCrm_Level(item);
                    }
                }
                else
                {
                    string idDeleteAll = Request["DeleteId-checkbox"];
                    string[] arrDeleteId = idDeleteAll.Split(',');
                    for (int i = 0; i < arrDeleteId.Count(); i++)
                    {
                        var item = Crm_LevelRepository.GetCrm_LevelById(int.Parse(arrDeleteId[i], CultureInfo.InvariantCulture));
                        if (item != null)
                        {
                            if (item.Id != Helpers.Common.CurrentUser.Id && Helpers.Common.CurrentUser.UserTypeId != 1)
                            {
                                TempData["FailedMessage"] = "NotOwner";
                                return RedirectToAction("Index");
                            }
                            var tar = Crm_TargetRepository.GetAllCrm_Target().ToList();
                            //List<Crm_TargetViewModel> listTar = new List<Crm_TargetViewModel>();
                            //AutoMapper.Mapper.Map(tar, listTar); 
                            foreach (var t in tar)
                            {
                                if (t.IdLevel == item.Id)
                                {
                                    TempData["FailedMessage"] = "Giá trị đang được sử dụng!";
                                    return RedirectToAction("Index");
                                }
                            }
                            item.IsDeleted = true;
                            Crm_LevelRepository.UpdateCrm_Level(item);
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

        #region Period
        public ViewResult IndexPeriod()
        {
            IEnumerable<Crm_PeriodViewModel> q = Crm_PeriodRepository.GetAllCrm_Period()
                .Select(item => new Crm_PeriodViewModel
                {
                    Id = item.Id,
                    IsDeleted = item.IsDeleted,
                    Name = item.Name,
                    starDate = item.starDate,
                    endDate = item.endDate
                });
            var model = q.ToList();
            ViewBag.SuccessMessage = TempData["SuccessMessage"];
            ViewBag.FailedMessage = TempData["FailedMessage"];
            ViewBag.AlertMessage = TempData["AlertMessage"];
            return View(model);
        }

        public ViewResult CreateCrm_Period()
        {
            var model = new Crm_PeriodViewModel();

            //var stt = Crm_LevelRepository.GetAllCrm_Level().ToList();

            return View(model);
        }

        [HttpPost]
        public ActionResult CreateCrm_Period(Crm_PeriodViewModel item)
        {
            var LoyaltyPoint = new Crm_Period();
            AutoMapper.Mapper.Map(item, LoyaltyPoint);
            LoyaltyPoint.IsDeleted = false;
            //.AssignedUserId = WebSecurity.CurrentUserId;

            Crm_PeriodRepository.InsertCrm_Period(LoyaltyPoint);


            TempData[Globals.SuccessMessageKey] = App_GlobalResources.Wording.InsertSuccess;
            return RedirectToAction("IndexPeriod");
        }

        public ActionResult EditCrm_Period(int? Id)
        {
            var LoyaltyPoint = Crm_PeriodRepository.GetCrm_PeriodById(Id.Value);
            if (LoyaltyPoint != null && LoyaltyPoint.IsDeleted != true)
            {
                var model = new Crm_PeriodViewModel();
                AutoMapper.Mapper.Map(LoyaltyPoint, model);

                if (model.Id != Helpers.Common.CurrentUser.Id && Helpers.Common.CurrentUser.UserTypeId != 1)
                {
                    TempData["FailedMessage"] = "NotOwner";
                    return RedirectToAction("IndexPeriod");
                }

                return View(model);
            }
            if (Request.UrlReferrer != null)
                return Redirect(Request.UrlReferrer.AbsoluteUri);
            return RedirectToAction("IndexPeriod");
        }

        [HttpPost]
        public ActionResult EditCrm_Period(Crm_PeriodViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (Request["Submit"] == "Save")
                {
                    var LoyaltyPoint = Crm_PeriodRepository.GetCrm_PeriodById(model.Id);
                    LoyaltyPoint.Name = model.Name;
                    LoyaltyPoint.starDate = model.starDate;
                    LoyaltyPoint.endDate = model.endDate;

                    Crm_PeriodRepository.UpdateCrm_Period(LoyaltyPoint);

                    TempData[Globals.SuccessMessageKey] = App_GlobalResources.Wording.UpdateSuccess;
                    return RedirectToAction("IndexPeriod");
                }



                return View(model);
            }

            return View(model);

            //if (Request.UrlReferrer != null)
            //    return Redirect(Request.UrlReferrer.AbsoluteUri);
            //return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult DeleteCrm_Period()
        {
            try
            {
                string id = Request["Delete"];
                if (id != null)
                {
                    var item = Crm_PeriodRepository.GetCrm_PeriodById(int.Parse(id, CultureInfo.InvariantCulture));
                    if (item != null)
                    {
                        if (item.Id != Helpers.Common.CurrentUser.Id && Helpers.Common.CurrentUser.UserTypeId != 1)
                        {
                            TempData["FailedMessage"] = "NotOwner";
                            return RedirectToAction("IndexPeriod");
                        }
                        var tar = Crm_TargetRepository.GetAllCrm_Target().ToList();
                        foreach(var t in tar)
                        {
                            if(t.IdPeriod == item.Id)
                            {
                                TempData["FailedMessage"] = " Giá trị đang được sử dụng!";
                                return RedirectToAction("IndexPeriod");
                            }
                        }
                        item.IsDeleted = true;
                        Crm_PeriodRepository.UpdateCrm_Period(item);
                    }
                }
                else
                {
                    string idDeleteAll = Request["DeleteId-checkbox"];
                    string[] arrDeleteId = idDeleteAll.Split(',');
                    for (int i = 0; i < arrDeleteId.Count(); i++)
                    {
                        var item = Crm_PeriodRepository.GetCrm_PeriodById(int.Parse(arrDeleteId[i], CultureInfo.InvariantCulture));
                        if (item != null)
                        {
                            if (item.Id != Helpers.Common.CurrentUser.Id && Helpers.Common.CurrentUser.UserTypeId != 1)
                            {
                                TempData["FailedMessage"] = "NotOwner";
                                return RedirectToAction("IndexPeriod");
                            }
                            var tar = Crm_TargetRepository.GetAllCrm_Target().ToList();
                            foreach (var t in tar)
                            {
                                if (t.IdPeriod == item.Id)
                                {
                                    TempData["FailedMessage"] = " Giá trị đang được sử dụng!";
                                    return RedirectToAction("IndexPeriod");
                                }
                            }
                            item.IsDeleted = true;
                            Crm_PeriodRepository.UpdateCrm_Period(item);
                        }
                    }
                }

                TempData[Globals.SuccessMessageKey] = App_GlobalResources.Wording.DeleteSuccess;
                return RedirectToAction("IndexPeriod");
            }
            catch (DbUpdateException)
            {
                TempData[Globals.FailedMessageKey] = App_GlobalResources.Error.RelationError;
                return RedirectToAction("IndexPeriod");
            }
        }
        #endregion

        #region Target
        public ViewResult IndexTarget()
        {
            IEnumerable<Crm_LevelViewModel> L = Crm_LevelRepository.GetAllCrm_Level()
               .Select(item => new Crm_LevelViewModel
               {
                   Id = item.Id,
                   IsDeleted = item.IsDeleted,
                   Name = item.Name,
                   Level = item.Level,
                   StaredIndex = item.StaredIndex,
                   EndIndex = item.EndIndex
               });
            var level = L.ToList();

            IEnumerable<Crm_PeriodViewModel> P = Crm_PeriodRepository.GetAllCrm_Period()
                .Select(item => new Crm_PeriodViewModel
                {
                    Id = item.Id,
                    IsDeleted = item.IsDeleted,
                    Name = item.Name,
                    starDate = item.starDate,
                    endDate = item.endDate
                });
            var period = P.ToList();

            IEnumerable<Crm_TargetViewModel> T1 = Crm_TargetRepository.GetAllCrm_Target()
                .Select(item => new Crm_TargetViewModel
                {
                    Id = item.Id,
                    IsDeleted = item.IsDeleted,
                    Level = item.IdLevel,
                    Period = item.IdPeriod,
                    Target = item.Target
                });
            var target1 = T1.ToList();
            

            ViewBag.SuccessMessage = TempData["SuccessMessage"];
            ViewBag.FailedMessage = TempData["FailedMessage"];
            ViewBag.AlertMessage = TempData["AlertMessage"];
            ViewBag.Level = level;
            ViewBag.Period = period;
            ViewBag.Target = target1;
            return View();
        }

        public ActionResult DeleteTarget(int id)
        {
            var point = Crm_TargetRepository.GetCrm_TargetById(id);
            point.IsDeleted = true;
            Crm_TargetRepository.UpdateCrm_Target(point);
            return RedirectToAction("IndexTarget");
        }

        [HttpPost]
        public ActionResult CreateOrEditTarget(Crm_TargetViewModel tar)
        {
            if(tar.Id > 0)
            {
                tar.IsDeleted = false;
                var LoyaltyPoint = Crm_TargetRepository.GetCrm_TargetById(tar.Id);
                //AutoMapper.Mapper.Map(LoyaltyPoint, mod);
                LoyaltyPoint.Target = tar.Target;
                Crm_TargetRepository.UpdateCrm_Target(LoyaltyPoint);
                return RedirectToAction("IndexTarget");
            }
            else
            {
                //kiem tra xem item da co chua
                var target = Crm_TargetRepository.GetAllCrm_Target().ToList();
                foreach (var t in target)
                {
                    if (t.IdLevel == tar.Level & t.IdPeriod == tar.Period)
                    {
                        TempData["isCreateTarget"] = "-1";
                        return RedirectToAction("IndexTarget");
                    }
                }

                var LoyaltyPoint = new Crm_Target();
                LoyaltyPoint.Target = tar.Target;
                LoyaltyPoint.IdLevel = tar.Level;
                LoyaltyPoint.IdPeriod = tar.Period;
                LoyaltyPoint.IsDeleted = false;
                Crm_TargetRepository.InsertCrm_Target(LoyaltyPoint);
                TempData["isCreateTarget"] = "1";
                return RedirectToAction("IndexTarget");
            }
            return RedirectToAction("IndexTarget");
        }
        #endregion
    }
}