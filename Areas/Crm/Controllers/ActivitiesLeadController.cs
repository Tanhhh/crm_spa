using Erp.BackOffice.Crm.Models;
using Erp.BackOffice.Hubs;
using Erp.BackOffice.Sale.Models;
using Erp.Domain.Account.Helper;
using Erp.Domain.Interfaces;
using Hangfire;
using Microsoft.AspNet.SignalR;
using Newtonsoft.Json;
using PagedList;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using static Erp.BackOffice.Crm.Controllers.LeadMeetingController;
using WebMatrix.WebData;
using static Erp.BackOffice.Sale.Controllers.AdviseCardController;
using Erp.Domain.Repositories;

namespace Erp.BackOffice.Crm.Controllers
{
    public class ActivitiesLeadController : Controller
    {

        private readonly IUserRepository userRepository;
        public ActivitiesLeadController(
             IUserRepository _user
            )
        {
            userRepository = _user;
        }
        public ActionResult ActivitiesLeadIndex()
        {
            var user = userRepository;
            ViewBag.user = user;

            return View();
        }
        [AllowAnonymous]
        public ActionResult ActivitiesLeadPartialView(IEnumerable<ActivitiesLeadLogsModel> model, IPagedList pagelist, string columnsort, int? sortdir)
        {
            model = model != null ? model : new List<ActivitiesLeadLogsModel>();

            var user = userRepository;
            ViewBag.user = user;
            ViewBag.ColumnSort = columnsort;
            ViewBag.SortDir = sortdir;
            ViewBag.PageList = pagelist;
            ViewBag.DataList = model;

            return PartialView();
        }
        [HttpPost]
        public ActionResult SearchActivitiesLead(string datars,bool isSearch, int pageNumber, int pageSize, string columnsort, int? sortdir)
        {
            var model = SqlHelper.QuerySP<ActivitiesPlusModel>("GetLeadLogsActivities", new
            {
                @uId = WebSecurity.CurrentUserId
            });
            int ipageNumber = (pageNumber != null ? (int)pageNumber : 1);
            if (isSearch)
            {
                ActivitieseadSearchModel datacv = JsonConvert.DeserializeObject<ActivitieseadSearchModel>(datars);
                model = FilterModel(model, datacv);
            }
            IPagedList pagedList = new PagedList<ActivitiesLeadLogsModel>(model, pageNumber, pageSize);
            model = model.Skip((ipageNumber - 1) * pageSize).Take(pageSize);
            if (columnsort != null && columnsort != "undefined")
            {
                if (sortdir == 1)
                {
                    model = model.OrderByDescending(x => typeof(ActivitiesLeadLogsModel).GetProperty(columnsort).GetValue(x)).ToList();
                }
                else
                {
                    model = model.OrderBy(x => typeof(ActivitiesLeadLogsModel).GetProperty(columnsort).GetValue(x)).ToList();
                }

                var result = ActivitiesLeadPartialView(model, pagedList, columnsort, sortdir);
                ControllerContext.RouteData.Values["action"] = "ActivitiesLeadPartialView";
                return result;
            }
            else
            {
                var result = ActivitiesLeadPartialView(model, pagedList, null, -1);
                ControllerContext.RouteData.Values["action"] = "ActivitiesLeadPartialView";
                return result;
            }
        }
        private IEnumerable<ActivitiesPlusModel> FilterModel(IEnumerable<ActivitiesPlusModel> model, ActivitieseadSearchModel datars)
        {
          
            if(!string.IsNullOrWhiteSpace(datars.contentKey))
            {
                model = model.Where(x => x.Content != null ? Helpers.Common.ChuyenThanhKhongDau(x.Content.ToString().ToLower().Trim()).Contains(Helpers.Common.ChuyenThanhKhongDau(datars.contentKey.ToString()).ToLower().Trim()) : false);
            }
            if(!string.IsNullOrWhiteSpace(datars.leadNameKey))
            {
                model = model.Where(x => x.LeadName != null ? Helpers.Common.ChuyenThanhKhongDau(x.LeadName.ToString().ToLower().Trim()).Contains(Helpers.Common.ChuyenThanhKhongDau(datars.leadNameKey.ToString()).ToLower().Trim()) : false);
            }
            if (!string.IsNullOrWhiteSpace(datars.createdDateStart))
            {
                model = model.Where(x => x.CreatedDate != null ? x.CreatedDate.Date >= DateTime.Parse(datars.createdDateStart).Date : false);
            }
            if (!string.IsNullOrWhiteSpace(datars.createdDateEnd))
            {
                model = model.Where(x => x.CreatedDate != null ? x.CreatedDate.Date <= DateTime.Parse(datars.createdDateEnd).Date : false);
            }
            if (!string.IsNullOrWhiteSpace(datars.combinedDateTimeStart))
            {
                model = model.Where(x => x.CombinedDateTime != null ? x.CombinedDateTime.Date >= DateTime.Parse(datars.combinedDateTimeStart).Date : false);
            }
            if (!string.IsNullOrWhiteSpace(datars.combinedDateTimeEnd))
            {
                model = model.Where(x => x.CombinedDateTime != null ? x.CombinedDateTime.Date <= DateTime.Parse(datars.combinedDateTimeEnd).Date : false);
            }
            if ( datars.HDValueArr==1)
            {
                model = model.Where(x => x.CombinedDateTime != null ? x.CombinedDateTime > DateTime.Now : false);
            }
            if(datars.actionValueArr != null && datars.actionValueArr.Length > 0)
            {
                model = model.Where(x => x.Action != null ? datars.actionValueArr.Any(y => x.Action.ToString().Contains(y)) : false);
            }
            if (datars.statusValueArr != null && datars.statusValueArr.Length > 0)
            {
                model = model.Where(x => x.Status != null  ? datars.statusValueArr.Any(y =>(y == "1" ? true : false) == x.Status) : false);
            }
            if (datars.typeValueArr != null && datars.typeValueArr.Length > 0)
            {
                model = model.Where(x => x.TypeLead != null ? datars.typeValueArr.Any(y => int.Parse(y) == x.TypeLead) : false);
            }
            if (datars.UsrValueArr != null && datars.UsrValueArr.Length > 0)
            {
                model = model.Where(x => x.AssignedUserId != null ? datars.typeValueArr.Any(y => int.Parse(y) == x.AssignedUserId) : false);
            }
            return model;
        }

        [HttpPost]
        public ActionResult UpdateStatusLeadLogs(string requestData)
        {
            try
            {
                List<int> requestDataList = JsonConvert.DeserializeObject<List<int>>(requestData);
                foreach (var item in requestDataList)
                {
                    var jobid = SqlHelper.QuerySP<int?>("sp_Crm_UpdateStatusLeadLogReturnIdJob", new
                    {
                        pId = item
                    }).FirstOrDefault();
                    if (jobid.ToString() != "")
                    {
                        BackgroundJob.Delete(jobid.ToString());
                    }

                }
                return Json(new { success = true, message = "Update thành công" }, JsonRequestBehavior.AllowGet);
            } catch(Exception ex)
            {
                return Json(new { success = false, message = "Update thất bại" }, JsonRequestBehavior.AllowGet);
            }
        }
    }
        
}