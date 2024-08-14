using System.Globalization;
using Erp.BackOffice.Staff.Models;
using Erp.BackOffice.Filters;
using Erp.Domain.Entities;
using Erp.Domain.Interfaces;
using Erp.Domain.Staff.Entities;
using Erp.Domain.Staff.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Erp.Utilities;
using WebMatrix.WebData;
using Erp.BackOffice.Helpers;
using System.Web;

namespace Erp.BackOffice.Staff.Controllers
{
    [Authorize]
    [InitializeSimpleMembership]
    [Erp.BackOffice.Helpers.NoCacheHelper]
    public class CheckInOutController : Controller
    {
        private readonly ICheckInOutRepository CheckInOutRepository;
        private readonly IUserRepository userRepository;

        public CheckInOutController(
            ICheckInOutRepository _CheckInOut
            , IUserRepository _user
            )
        {
            CheckInOutRepository = _CheckInOut;
            userRepository = _user;
        }

        #region Index
        public ViewResult Index(/*string Name, string Code,*/string txtSearch, int? Branch, int? Department)
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

            Branch = int.Parse(strBrandID);

            var user = userRepository.GetUserById(WebSecurity.CurrentUserId);
            IEnumerable<CheckInOutViewModel> q = CheckInOutRepository.GetAllvwCheckInOut()
                .Select(item => new CheckInOutViewModel
                {
                    Id = item.Id,
                    CreatedDate = item.CreatedDate,
                    TimeDate = item.TimeDate,
                    TimeType = item.TimeType,
                    MachineNo = item.MachineNo,
                    UserId = item.UserId,
                    TimeStr = item.TimeStr,
                    BranchDepartmentId=item.BranchDepartmentId,
                    Sale_BranchId=item.Sale_BranchId,
                    Code=item.Code,
                    StaffId=item.StaffId,
                    Name=item.Name
                }).OrderByDescending(m => m.TimeStr);
            //if (!string.IsNullOrEmpty(Name))
            //{
            //    q = q.Where(x => Erp.BackOffice.Helpers.Common.ChuyenThanhKhongDau(x.Name).ToLower().Contains(Erp.BackOffice.Helpers.Common.ChuyenThanhKhongDau(Name).ToLower()));
            //}
            //if (!string.IsNullOrEmpty(Code))
            //{
            //    q = q.Where(x => Erp.BackOffice.Helpers.Common.ChuyenThanhKhongDau(x.Code).ToLower().Contains(Erp.BackOffice.Helpers.Common.ChuyenThanhKhongDau(Code).ToLower()));
            //}
            if (!string.IsNullOrEmpty(txtSearch))
            {
                int i = 0;
                //a = System.Convert.ToInt32(txtSearch);
                if (!Int32.TryParse(txtSearch, out i))
                {
                    q = q.Where(x => Erp.BackOffice.Helpers.Common.ChuyenThanhKhongDau(x.Name).ToLower().Contains(Erp.BackOffice.Helpers.Common.ChuyenThanhKhongDau(txtSearch).ToLower()));
                }
                else
                    q = q.Where(x => x.UserId == i);
            }
            if (Branch != null && Branch.Value > 0)
            {
                q = q.Where(x => x.Sale_BranchId==Branch);
            }
            else
            {
                if (Request["search"] == null)
                {
                    //if (user.BranchId != null && user.BranchId.Value > 0)
                    //{
                    //    q = q.Where(item => item.Sale_BranchId == user.BranchId);
                    //}
                }
            }
            if (Department != null && Department.Value > 0)
            {
                q = q.Where(x => x.BranchDepartmentId == Department);
            }
            ViewBag.SuccessMessage = TempData["SuccessMessage"];
            ViewBag.FailedMessage = TempData["FailedMessage"];
            ViewBag.AlertMessage = TempData["AlertMessage"];
            return View(q);
        }
        #endregion
        #region List
        public ViewResult List(int? StaffId)
        {
            var user = userRepository.GetUserById(WebSecurity.CurrentUserId);
            IEnumerable<CheckInOutViewModel> q = CheckInOutRepository.GetAllvwCheckInOut()
                .Select(item => new CheckInOutViewModel
                {
                    MachineNo = item.MachineNo,
                    UserId = item.UserId,
                    TimeStr = item.TimeStr,
                    BranchDepartmentId = item.BranchDepartmentId,
                    Sale_BranchId = item.Sale_BranchId,
                    Code = item.Code,
                    StaffId = item.StaffId,
                    Name = item.Name
                }).OrderByDescending(m => m.TimeStr);

            if (StaffId != null && StaffId.Value > 0)
            {
                q = q.Where(x => x.StaffId == StaffId.Value);
            }
            else
            {
                q = null;
            }
           
            ViewBag.SuccessMessage = TempData["SuccessMessage"];
            ViewBag.FailedMessage = TempData["FailedMessage"];
            ViewBag.AlertMessage = TempData["AlertMessage"];
            return View(q);
        }
        #endregion
        #region Create
        public ViewResult Create()
        {
            var model = new CheckInOutViewModel();
            return View(model);
        }
        [HttpPost]
        public ActionResult Create(CheckInOutViewModel model)
        {
            if (ModelState.IsValid)
            {
                var CheckInOut = new CheckInOut();
                AutoMapper.Mapper.Map(model, CheckInOut);
                CheckInOut.CreatedDate = DateTime.Now;
                CheckInOut.TimeStr = DateTime.Now;
                //CheckInOut.TimeDate = DateTime.Now;
                CheckInOutRepository.InsertCheckInOut(CheckInOut);

                TempData[Globals.SuccessMessageKey] = App_GlobalResources.Wording.InsertSuccess;
                return RedirectToAction("Index");
            }
            return View(model);
        }
        #endregion


        #region Edit
        public ActionResult Edit(int Id)
        {
            var CheckInOut = CheckInOutRepository.GetCheckInOutById(Id);
            if (CheckInOut != null)
            {
                var model = new CheckInOutViewModel();
                AutoMapper.Mapper.Map(CheckInOut, model);
                return View(model);
            }
            if (Request.UrlReferrer != null)
                return Redirect(Request.UrlReferrer.AbsoluteUri);
            return RedirectToAction("Index");
        }
        [HttpPost]
        public ActionResult Edit(CheckInOutViewModel model)
        {
            if (ModelState.IsValid)
            {
                    var CheckInOut = CheckInOutRepository.GetCheckInOutById(model.Id);
                    AutoMapper.Mapper.Map(model, CheckInOut);
                    CheckInOut.CreatedDate = DateTime.Now;
                    CheckInOut.TimeStr = DateTime.Now;
                    //CheckInOut.TimeDate = DateTime.Now;
                    CheckInOutRepository.UpdateCheckInOut(CheckInOut);

                    TempData[Globals.SuccessMessageKey] = App_GlobalResources.Wording.UpdateSuccess;
                    return RedirectToAction("Index");
            }
            return View(model);
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
                    var item = CheckInOutRepository.GetCheckInOutById(int.Parse(id, CultureInfo.InvariantCulture));
                    if (item != null)
                    {
                        CheckInOutRepository.DeleteCheckInOut(item.Id);
                    }
                }
                else
                {
                    string idDeleteAll = Request["DeleteId-checkbox"];
                    string[] arrDeleteId = idDeleteAll.Split(',');
                    for (int i = 0; i < arrDeleteId.Count(); i++)
                    {
                        var item = CheckInOutRepository.GetCheckInOutById(int.Parse(arrDeleteId[i], CultureInfo.InvariantCulture));
                        if (item != null)
                        {
                            CheckInOutRepository.DeleteCheckInOut(item.Id);
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
