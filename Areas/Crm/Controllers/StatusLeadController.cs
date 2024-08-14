using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using System.IO;
using System.Web.Mvc;
using Erp.BackOffice.Crm.Models;
using Erp.Domain.Account.Helper;
using WebMatrix.WebData;

namespace Erp.BackOffice.Crm.Controllers
{
    public class StatusLeadController : Controller
    {
        public ViewResult Index(int? type)
        {
            var dataStatus = SqlHelper.QuerySP<StatusViewModel>("spCrm_StatusLead_Index").ToList();

            ViewBag.SuccessMessage = TempData["SuccessMessage"];
            ViewBag.FailedMessage = TempData["FailedMessage"];
            ViewBag.AlertMessage = TempData["AlertMessage"];
            if (type != null)
            {
                dataStatus = dataStatus.Where(x => x.Type == type).ToList();
                ViewBag.DataType = type;
            } else
            {
                dataStatus = dataStatus.Where(x => x.Type == 1).ToList();
                ViewBag.DataType = 1;
            }
            return View(dataStatus);
        }

        #region Create
        public ViewResult Create(string ActionName, int? lastOrderStatus, int? typeoption)
        {
            ViewBag.ActionName = ActionName;
            ViewBag.lastOrderStatus = lastOrderStatus != null ? lastOrderStatus : 0;
            ViewBag.DataType = typeoption;
            return View();
        }
        [System.Web.Mvc.HttpPost]
        //[ValidateInput(false)]
        public ActionResult Create(string name, decimal? successRate, string colorStatus, int orderStatus, Boolean? EndStatus, int type, int? typeForcast, int notMoveable)
        {
            var result = SqlHelper.ExecuteSP("spCrm_StatusLead_Create", new
            {
                pName = name,
                pSuccessRate = successRate,
                pColorStatus = colorStatus,
                pOrderStatus = orderStatus,
                pEndStatus = EndStatus,
                pType = type,
                CreatedUserId = WebSecurity.CurrentUserId,
                ModifiedUserId = WebSecurity.CurrentUserId,
                ptypeForcast = typeForcast,
                pNotMoveable = notMoveable
            });
            if(result != 0)
            {
                return Json(new { Success = true });
            }
            return Json(new { Success = false });
        }
        #endregion

        #region Edit
        public ViewResult EditStatus(string ActionName, int id)
        {
            var dataStatus = SqlHelper.QuerySP<StatusViewModel>("spCrm_StatusLead_SelectById", new { pId = id}).ToList();
            StatusViewModel editStatus = dataStatus.First();
            ViewBag.ActionName = ActionName;
            return View(editStatus);
        }
        [System.Web.Mvc.HttpPost]
        //[ValidateInput(false)]
        public ActionResult EditStatus(int id,string name, decimal? successRate, string colorStatus, int orderStatus, Boolean? EndStatus, int type, int typeForcast, int notMoveable)
        {
            var result = SqlHelper.ExecuteSP("spCrm_StatusLead_Edit", new
            {
                pid = id,
                pName = name,
                pSuccessRate = successRate,
                pColorStatus = colorStatus,
                pOrderStatus = orderStatus,
                pEndStatus = EndStatus,
                pType = type,
                ModifiedUserId = WebSecurity.CurrentUserId,
                ptypeForcast = typeForcast,
                pNotMoveable = notMoveable
            });
            if (result != 0)
            {
                return Json(new { Success = true });
            }
            return Json(new { Success = false });
        }
        #endregion

        #region Delete
        [System.Web.Mvc.HttpPost]
        public ActionResult Delete(int id)
        {
            var result = SqlHelper.ExecuteSP("spCrm_StatusLead_Delete", new
            {
                pId = id
            });
            if (result != 0)
            {
                return Json(new { Success = true });
            }
            return Json(new { Success = false });
        }
        #endregion
    }
}
