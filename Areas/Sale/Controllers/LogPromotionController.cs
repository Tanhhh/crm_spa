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

namespace Erp.BackOffice.Sale.Controllers
{
    [Authorize]
    [InitializeSimpleMembership]
    [Erp.BackOffice.Helpers.NoCacheHelper]
    public class LogPromotionController : Controller
    {
        private readonly ILogPromotionRepository LogPromotionRepository;
        private readonly IUserRepository userRepository;

        public LogPromotionController(
            ILogPromotionRepository _LogPromotion
            , IUserRepository _user
            )
        {
            LogPromotionRepository = _LogPromotion;
            userRepository = _user;
        }

        #region Index

        public ViewResult Index(string txtSearch)
        {

            IQueryable<LogPromotionViewModel> q = LogPromotionRepository.GetAllLogPromotion()
                .Select(item => new LogPromotionViewModel
                {
                    Id = item.Id,
                    CreatedUserId = item.CreatedUserId,
                    //CreatedUserName = item.CreatedUserName,
                    CreatedDate = item.CreatedDate,
                    ModifiedUserId = item.ModifiedUserId,
                    //ModifiedUserName = item.ModifiedUserName,
                    ModifiedDate = item.ModifiedDate,
                    Name = item.Name,
                }).OrderByDescending(m => m.ModifiedDate);

            ViewBag.SuccessMessage = TempData["SuccessMessage"];
            ViewBag.FailedMessage = TempData["FailedMessage"];
            ViewBag.AlertMessage = TempData["AlertMessage"];
            return View(q);
        }
        #endregion

        #region Create
        public ViewResult Create()
        {
            var model = new LogPromotionViewModel();

            return View(model);
        }

        [HttpPost]
        public ActionResult Create(LogPromotionViewModel model)
        {
            if (ModelState.IsValid)
            {
                var LogPromotion = new LogPromotion();
                AutoMapper.Mapper.Map(model, LogPromotion);
                LogPromotion.IsDeleted = false;
                LogPromotion.CreatedUserId = WebSecurity.CurrentUserId;
                LogPromotion.ModifiedUserId = WebSecurity.CurrentUserId;
                LogPromotion.AssignedUserId = WebSecurity.CurrentUserId;
                LogPromotion.CreatedDate = DateTime.Now;
                LogPromotion.ModifiedDate = DateTime.Now;
                LogPromotionRepository.InsertLogPromotion(LogPromotion);

                TempData[Globals.SuccessMessageKey] = App_GlobalResources.Wording.InsertSuccess;
                return RedirectToAction("Index");
            }
            return View(model);
        }
        #endregion

        #region Edit
        public ActionResult Edit(int? Id)
        {
            var LogPromotion = LogPromotionRepository.GetLogPromotionById(Id.Value);
            if (LogPromotion != null && LogPromotion.IsDeleted != true)
            {
                var model = new LogPromotionViewModel();
                AutoMapper.Mapper.Map(LogPromotion, model);
                
                if (model.CreatedUserId != Helpers.Common.CurrentUser.Id && Helpers.Common.CurrentUser.UserTypeId != 1)
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
        public ActionResult Edit(LogPromotionViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (Request["Submit"] == "Save")
                {
                    var LogPromotion = LogPromotionRepository.GetLogPromotionById(model.Id);
                    AutoMapper.Mapper.Map(model, LogPromotion);
                    LogPromotion.ModifiedUserId = WebSecurity.CurrentUserId;
                    LogPromotion.ModifiedDate = DateTime.Now;
                    LogPromotionRepository.UpdateLogPromotion(LogPromotion);

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

        #endregion

        #region Detail
        public ActionResult Detail(int? Id)
        {
            var LogPromotion = LogPromotionRepository.GetLogPromotionById(Id.Value);
            if (LogPromotion != null && LogPromotion.IsDeleted != true)
            {
                var model = new LogPromotionViewModel();
                AutoMapper.Mapper.Map(LogPromotion, model);
                
                if (model.CreatedUserId != Helpers.Common.CurrentUser.Id && Helpers.Common.CurrentUser.UserTypeId != 1)
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
                    var item = LogPromotionRepository.GetLogPromotionById(int.Parse(arrDeleteId[i], CultureInfo.InvariantCulture));
                    if(item != null)
                    {
                        if (item.CreatedUserId != Helpers.Common.CurrentUser.Id && Helpers.Common.CurrentUser.UserTypeId != 1)
                        {
                            TempData["FailedMessage"] = "NotOwner";
                            return RedirectToAction("Index");
                        }

                        item.IsDeleted = true;
                        LogPromotionRepository.UpdateLogPromotion(item);
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
