using System.Data.Entity.Infrastructure;
using System.Globalization;
using Erp.BackOffice.Administration.Models;
using Erp.BackOffice.App_GlobalResources;
using Erp.BackOffice.Filters;
using Erp.Domain.Entities;
using Erp.Domain.Interfaces;
using System.Linq;
using System.Web.Mvc;
using Erp.Utilities;
using System.Collections.Generic;
using System.Web;

namespace Erp.BackOffice.Administration.Controllers
{
    [Authorize]
    [InitializeSimpleMembership]
    [Erp.BackOffice.Helpers.NoCacheHelper]
    public class UserType_kdController : Controller
    {
        private readonly IUserType_kdRepository _userType_kdRepository;
        public UserType_kdController(IUserType_kdRepository userType)
        {
            _userType_kdRepository = userType;
        }

        #region Index Action
        public ViewResult Index(string txtSearch)
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

            var userTypes = _userType_kdRepository.GetvwUserTypes().Where(x=>x.BranchId == intBrandID);
            var model = new ListUsersType_kdModel { UserTypes_kd = userTypes };
            if (!string.IsNullOrEmpty(txtSearch))
            {
                model.UserTypes_kd = userTypes.Where(m => m.Name.ToUpper().Contains(txtSearch.ToUpper()));
            }
            ViewBag.SuccessMessage = TempData[Globals.SuccessMessageKey];
            ViewBag.FailedMessage = TempData[Globals.FailedMessageKey];
            return View(model);
        }
        #endregion

        #region Edit Action
        public ActionResult EditUserType(int userTypeId)
        {
            List<SelectListItem> lstScope = new List<SelectListItem>
            {
                        new SelectListItem(),
                        new SelectListItem() {Text = "Internal", Value="false"},
                        new SelectListItem() {Text = "External", Value="true"}
            };
            
            var editUserType = new EditUserTypeModel();
//v0
            var userType = _userType_kdRepository.GetUserTypeById(userTypeId);
            if (userType != null)
            {
                AutoMapper.Mapper.Map(userType, editUserType);
                
                ViewBag.Scope = new SelectList(lstScope, "Value", "Text", (editUserType.Scope.HasValue == true) ? ((editUserType.Scope == true) ? "true" : "false") : null);

                return View("EditUserType", editUserType);
            }

            if (Request.UrlReferrer != null)
                return Redirect(Request.UrlReferrer.AbsoluteUri);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult EditUserType(EditUserTypeModel model)
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
            var BranchId = intBrandID;
            List<SelectListItem> lstScope = new List<SelectListItem>
            {
                        new SelectListItem(),
                        new SelectListItem() {Text = "Internal", Value=false.ToString()},
                        new SelectListItem() {Text = "External", Value=true.ToString()}
            };
            ViewBag.Scope = new SelectList(lstScope, "Value", "Text");
            if (ModelState.IsValid)
            {
                if (Request["Submit"]=="Save")
                {
                    var userType_kd = _userType_kdRepository.GetUserTypeById(model.Id);
                    AutoMapper.Mapper.Map(model, userType_kd);
                    userType_kd.BranchId = BranchId.Value;
                    _userType_kdRepository.Save();
                    //ghi log 
                    Erp.BackOffice.Controllers.HomeController.WriteLog(model.Id, model.Name, "đã cập nhật nhóm người dùng", "UserType_kd/EditUserType?AreaName=Administration&userTypeId=", Helpers.Common.CurrentUser.BranchId.Value);

                    TempData[Globals.SuccessMessageKey] = Wording.UpdateSuccess;
                    return RedirectToAction("Index");
                }
                return RedirectToAction("Index");
            }
            TempData[Globals.FailedMessageKey] = Error.UpdateUnsuccess;
            if (Request.UrlReferrer != null)
                return Redirect(Request.UrlReferrer.AbsoluteUri);
            return RedirectToAction("Index");
        }
        #endregion

        #region Delete Action
        [HttpPost]
        public ActionResult DeleteUserType()
        {
            if (ModelState.IsValid)
            {
                int deleteUserTypeId = int.Parse(Request["DeleteUserTypeId"], CultureInfo.InvariantCulture);
                try
                {
                    _userType_kdRepository.DeleteUserType(deleteUserTypeId);
                    TempData[Globals.SuccessMessageKey] = Wording.DeleteSuccess;
                }
                catch(DbUpdateException)
                {
                    TempData[Globals.FailedMessageKey] = Error.DeletingError;
                }
                return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
            
        }

        [HttpPost]
        public ActionResult DeleteAll()
        {
            try
            {
                string idDeleteAll = Request["DeleteAll-checkbox"];
                string[] arrDeleteId = idDeleteAll.Split(',');
                for (int i = 0; i < arrDeleteId.Count(); i++)
                {
                    _userType_kdRepository.DeleteUserType(int.Parse(arrDeleteId[i], CultureInfo.InvariantCulture));
                }
                TempData[Globals.SuccessMessageKey] = Wording.DeleteSuccess;
                return RedirectToAction("Index");
            }
            catch(DbUpdateException)
            {
                TempData[Globals.FailedMessageKey] = Error.RelationError;
                return RedirectToAction("Index");
            }
        }
        #endregion

        #region Create Action
        public ActionResult AddUserType()
        {
   
            List<SelectListItem> lstScope = new List<SelectListItem>
            {
                        new SelectListItem(),
                        new SelectListItem() {Text = "Internal", Value="false"},
                        new SelectListItem() {Text = "External", Value="true"}
        };
            ViewBag.Scope = new SelectList(lstScope, "Value", "Text");
            return View();
        }

        [HttpPost]
        public ActionResult AddUserType(EditUserTypeModel model)
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
            List<SelectListItem> lstScope = new List<SelectListItem>
            {
                        new SelectListItem(),
                          new SelectListItem() {Text = "Internal", Value="false"},
                        new SelectListItem() {Text = "External", Value="true"}
            };
            ViewBag.Scope = new SelectList(lstScope, "Value", "Text");
           var BranchId = intBrandID;

            if (ModelState.IsValid)
            {
                if (Request["Submit"]=="Save")
                {
                    var userType_kd = new UserType_kd();
                    AutoMapper.Mapper.Map(model, userType_kd);
                    userType_kd.BranchId = BranchId.Value;
                   _userType_kdRepository.InsertUserType(userType_kd);
                    //ghi log 
                    Erp.BackOffice.Controllers.HomeController.WriteLog(model.Id, model.Name, "đã tạo nhóm người dùng", "UserType_kd/EditUserType?AreaName=Administration&userTypeId=", Helpers.Common.CurrentUser.BranchId.Value);

                    TempData[Globals.SuccessMessageKey] = Wording.InsertSuccess;
                    return RedirectToAction("Index");
                }
                if (Request["Cancel"]=="Cancel")
                {
                    return RedirectToAction("Index");
                }
                return RedirectToAction("AddUserType");
            }
            if (Request.UrlReferrer != null)
                return Redirect(Request.UrlReferrer.AbsoluteUri);
            return RedirectToAction("Index");
        }
        #endregion
    }
}
