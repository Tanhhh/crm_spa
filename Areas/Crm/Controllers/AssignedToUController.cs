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
    public class AssignedToUController : Controller
    {
        public ViewResult Index(int? total, string listData)
        {
            var typeAccList = SqlHelper.QuerySP<AssignedToUUserType>("spCrm_AssignedToU_Select_UserType").ToList();
            var accList = SqlHelper.QuerySP<AssignedToUUser>("spCrm_AssignedToU_Select_User_ByType").ToList();
            List<string> idList = new List<string>();
            if (!string.IsNullOrEmpty(listData))
            {
                string[] idArray = listData.Split(',');
                idList = idArray.ToList();
            }
            ViewBag.TypeAccList = typeAccList;
            ViewBag.AccList = accList;
            ViewBag.Total = total;
            ViewBag.ListDataId = idList;
            return View();
        }
        [HttpPost]
        public ActionResult AsignedData(string ListIdLead, string ListNd)
        {
            List<IdNd> dataLisstNd = JsonConvert.DeserializeObject<List<IdNd>>(ListNd);
            List<string> idList = new List<string>();
            if (!string.IsNullOrEmpty(ListIdLead))
            {
                string[] idArray = ListIdLead.Split(',');
                idList = idArray.ToList();
            }
            try
            {
                int stt = 0;
                foreach (var item in dataLisstNd)
                {
                    for (int i = 0; i < item.sl; i++)
                    {
                        var result = SqlHelper.QuerySP<int>("spCrm_AssignedTou_Edit_AssignedUserId", new
                        {
                            @pId = idList[stt],
                            @pModifiedUserId = WebSecurity.CurrentUserId,
                            @pAssignedUserId = item.id
                        });
                        stt++;
                    }
                }
                return Json(new { Success = true });
            }
            catch (Exception e)
            {
                return Json(new { Success = false });
            }
        }
        public ViewResult ChangeAssIndex(int? total, string listData)
        {
            var typeAccList = SqlHelper.QuerySP<AssignedToUUserType>("spCrm_AssignedToU_Select_UserType").ToList();
            var accList = SqlHelper.QuerySP<AssignedToUUser>("spCrm_AssignedToU_Select_User_ByType").ToList();
            List<string> idList = new List<string>();
            if (!string.IsNullOrEmpty(listData))
            {
                string[] idArray = listData.Split(',');
                idList = idArray.ToList();
            }
            ViewBag.TypeAccList = typeAccList;
            ViewBag.AccList = accList;
            ViewBag.Total = total;
            ViewBag.ListDataId = idList;
            return View();
        }
        [HttpPost]
        public ActionResult ChangeAssData(string ListIdLead, string ListNd)
        {
            List<IdNd> dataLisstNd = JsonConvert.DeserializeObject<List<IdNd>>(ListNd);
            List<string> idList = new List<string>();
            if (!string.IsNullOrEmpty(ListIdLead))
            {
                string[] idArray = ListIdLead.Split(',');
                idList = idArray.ToList();
            }
            try
            {
                int stt = 0;
                foreach (var item in dataLisstNd)
                {
                    for (int i = 0; i < item.sl; i++)
                    {
                        var result = SqlHelper.QuerySP<int>("spCrm_LeadMeeting_Edit_UserId", new
                        {
                            @pId = idList[stt],
                            @pModifiedUserId = WebSecurity.CurrentUserId,
                            @pAssignedUserId = item.id
                        });
                        stt++;
                    }
                }
                return Json(new { Success = true });
            }
            catch (Exception e)
            {
                return Json(new { Success = false });
            }
        }
    }
    public class IdNd
    {
        public int id { get; set; }
        public int sl { get; set; }
    }
}
