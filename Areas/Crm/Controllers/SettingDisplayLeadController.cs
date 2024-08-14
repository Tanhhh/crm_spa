using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Erp.BackOffice.Crm.Models;
using Erp.Domain.Account.Helper;

namespace Erp.BackOffice.Crm.Controllers
{
    public class SettingDisplayLeadController : Controller
    {
        public ViewResult Index(int? areaId)
        {
            var dataFiled = SqlHelper.QuerySP<SettingDisplayLeadViewModel>("spCrm_SettingDisplayLead_Index").ToList();
            var data = SqlHelper.QuerySP<LeadSectionViewModel>("spCrm_LeadSection_Index").ToList();
            ViewBag.SuccessMessage = TempData["SuccessMessage"];
            ViewBag.FailedMessage = TempData["FailedMessage"];
            ViewBag.AlertMessage = TempData["AlertMessage"];
            ViewBag.Data = data;
            if (areaId != null)
            {
                dataFiled = dataFiled.Where(x => x.LeadSectionId == areaId).ToList();
                ViewBag.DataSe = areaId;
            }
            return View(dataFiled);
        }
        [HttpPost]
        public ActionResult UpIsHidens(int id, bool isHiden)
        {
            var result = SqlHelper.ExecuteSP("spCrm_LeadSection_Field_Edit_IsHiden", new { pId = id, pIsHiden = isHiden });
            if (result != 0)
            {
                return Json(new { Success = true });
            }
            return Json(new { Success = false });
        }
    }
}
