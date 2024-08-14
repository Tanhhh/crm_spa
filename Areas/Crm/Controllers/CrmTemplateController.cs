using System.Globalization;
using Erp.BackOffice.Crm.Models;
using Erp.BackOffice.Filters;
using Erp.Domain.Entities;
using Erp.Domain.Interfaces;
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
using Erp.BackOffice.Helpers;
using Erp.BackOffice.Sale.Models;
using static System.Runtime.CompilerServices.RuntimeHelpers;
using Erp.Domain.Crm.Helper;
using DocumentFormat.OpenXml.Wordprocessing;
using Erp.Domain.Sale.Repositories;
using System.Text.RegularExpressions;
using DocumentFormat.OpenXml.EMMA;
using DocumentFormat.OpenXml.Office2010.Excel;
using System.Windows.Forms;
using DocumentFormat.OpenXml.Drawing.Charts;
using Newtonsoft.Json;
using Erp.BackOffice.Areas.Crm.Models;

namespace Erp.BackOffice.Crm.Controllers
{
    [Authorize]
    [InitializeSimpleMembership]
    [Erp.BackOffice.Helpers.NoCacheHelper]
    public class CrmTemplateController : Controller
    {
        private readonly IUserRepository userRepository;

        public CrmTemplateController(
            IUserRepository _user
            )
        {
            userRepository = _user;
        }

        #region Index
        public ActionResult Index(string txtSearch)
        {
            var model = SqlHelper.QuerySP<CrmTemplateViewModel>("spCrm_Template_Index", new
            {

            }).ToList();

            ViewBag.SuccessMessage = TempData["SuccessMessage"];
            ViewBag.FailedMessage = TempData["FailedMessage"];
            ViewBag.AlertMessage = TempData["AlertMessage"];
            return View(model);
        }
        #endregion


        #region Create
        [HttpGet]
        public ActionResult Create()
        {
            var model = new CrmTemplateViewModel();
            var dropdownData = SqlHelper.QuerySP<string>("spCrm_SystemParameters_Select_Name", new
            {
            });
            ViewBag.dropdownData = dropdownData;
            return View(model);
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create(CrmTemplateViewModel model)
        {
            if (ModelState.IsValid)
            {
                SqlHelper.QuerySP<CrmTemplateViewModel>("spCrm_Template_Create", new
                {

                    pTypeTemplate = model.TypeTemplate,
                    pContentRule = model.ContentRule,
                    pTileEmail = model.TileEmail,
                    pContentEmail = model.ContentEmail,
                    pNote = model.Note,
                    pCreatedUserId = WebSecurity.CurrentUserId,
                    pTypeLead = model.TypeLead,
                    pZnsId = model.ZNSId
                });
                






                TempData[Globals.SuccessMessageKey] = App_GlobalResources.Wording.InsertSuccess;
                return Json(new { success = true, 
                                  redirectToUrl = Url.Action("Index", "CrmTemplate") });
            }
            // return View(model);
            return Json(new
            {
                success = false,
                errors = string.Join("; ", ModelState.Values
                                             .SelectMany(x => x.Errors)
                                             .Select(x => x.ErrorMessage))
        });
        }
        #endregion

        [HttpGet]
        #region Edit
        public ActionResult Edit(int? Id)
        {
            var model = SqlHelper.QuerySP<CrmTemplateViewModel>("spCrm_Template_SelectById", new
            {
                pID = Id,
            }).FirstOrDefault(s => s.Id == Id);

            var dropdownData = SqlHelper.QuerySP<string>("spCrm_SystemParameters_Select_Name", new
            {
            });
            ViewBag.dropdownData = dropdownData;
            if (model != null && model.IsDeleted != true)
            {
                //if (model.CreatedUserId != Helpers.Common.CurrentUser.Id && Helpers.Common.CurrentUser.UserTypeId != 1)
                //{
                //    TempData["FailedMessage"] = "NotOwner";
                //    return RedirectToAction("Index");
                //}
                return View(model);
            }
            //if (Request.UrlReferrer != null)
            //    return Redirect(Request.UrlReferrer.AbsoluteUri);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult Edit(CrmTemplateViewModel model)
        {
            if (ModelState.IsValid)
            {
                SqlHelper.QuerySP<CrmTemplateViewModel>("spCrm_Template_Edit", new
                {
                    pId = model.Id,
                    pTypeTemplate = model.TypeTemplate,
                    pContentRule = model.ContentRule,
                    pTileEmail = model.TileEmail,
                    pContentEmail = model.ContentEmail,
                    pNote = model.Note,
                    pModifiedUserId = WebSecurity.CurrentUserId,
                    pTypeLead = model.TypeLead,
                    pZnsId = model.ZNSId
                });
                TempData[Globals.SuccessMessageKey] = App_GlobalResources.Wording.UpdateSuccess;
                return Json(new
                {
                    success = true,
                    redirectToUrl = Url.Action("Index", "CrmTemplate")
                });

            }
            return Json(new
            {
                success = false,
                errors = string.Join("; ", ModelState.Values
                                                         .SelectMany(x => x.Errors)
                                                         .Select(x => x.ErrorMessage))
            });
        }

        #endregion

        #region Detail
        public ActionResult Detail(int? Id)
        {
            var model = SqlHelper.QuerySP<CrmTemplateViewModel>("spCrm_Template_SelectById", new
            {
                pID = Id,
            }).FirstOrDefault(s => s.Id == Id);

            if (model != null && model.IsDeleted != true)
            {
                //if (model.CreatedUserId != Helpers.Common.CurrentUser.Id && Helpers.Common.CurrentUser.UserTypeId != 1)
                //{
                //    TempData["FailedMessage"] = "NotOwner";
                //    return RedirectToAction("Index");
                //}
                return View(model);
            }
            //if (Request.UrlReferrer != null)
            //    return Redirect(Request.UrlReferrer.AbsoluteUri);
            return RedirectToAction("Index");
        }
        #endregion

        #region Delete
        [HttpPost]
        public ActionResult Delete()
        {
                
            try
            {
                string Id = Request["Delete"];
                if (Id != null)
                {
                        //if (item.CreatedUserId != Helpers.Common.CurrentUser.Id && Helpers.Common.CurrentUser.UserTypeId != 1)
                        //{
                        //    TempData["FailedMessage"] = "NotOwner";
                        //    return RedirectToAction("Index");
                        //}

                        SqlHelper.QuerySP<CrmTemplateViewModel>("spCrm_Template_Delete", new
                        {
                            pIDs = Id,
                            //pIsDeleted = true,
                            //pTypeTemplate = "",
                            //pContentRule = "",
                            //pTileEmail = "",
                            //pContentEmail = "",
                            //pNote = "",
                            pModifiedUserId = WebSecurity.CurrentUserId,
                            //pAssignedUserId = "",
                        });
                }
                else
                {
                    string idDeleteAll = Request["DeleteId-checkbox"];
                    SqlHelper.QuerySP<CrmTemplateViewModel>("spCrm_Template_Delete", new
                    {
                        pIDs = idDeleteAll,
                        pModifiedUserId = WebSecurity.CurrentUserId,
                    });
                }

                TempData[Globals.SuccessMessageKey] = App_GlobalResources.Wording.DeleteSuccess;
                return Json(new
                {
                    redirectToUrl = Url.Action("Index", "CrmTemplate")
                });
            }
            catch (DbUpdateException)
            {
                TempData[Globals.FailedMessageKey] = App_GlobalResources.Error.RelationError;
                return Json(new
                {
                    redirectToUrl = Url.Action("Index", "CrmTemplate")
                });
            }
        }
        #endregion

        #region CRUD Email Footer - Tuấn Anh
        public ActionResult IndexFooterEmail(string txtSearch)
        {
            var model = SqlHelper.QuerySP<CrmEmailFooterViewModel>("spCrm_EmailFooter_Index", new
            {
                @pCreatedUserId = WebSecurity.CurrentUserId
            }).ToList();

            ViewBag.SuccessMessage = TempData["SuccessMessage"];
            ViewBag.FailedMessage = TempData["FailedMessage"];
            ViewBag.AlertMessage = TempData["AlertMessage"];
            return View(model);
        }
        [HttpGet]
        public ActionResult CreateFooterEmail()
        {
            var model = new CrmEmailFooterViewModel();
            return View(model);
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult CreateFooterEmail(CrmEmailFooterViewModel model)
        {
            if (ModelState.IsValid)
            {
                SqlHelper.QuerySP<CrmEmailFooterViewModel>("spCrm_EmailFooter_Create", new
                {
                    @Logs = model.Logs,
                    @CreatedUserId = WebSecurity.CurrentUserId,
                });

                TempData[Globals.SuccessMessageKey] = App_GlobalResources.Wording.InsertSuccess;
                return RedirectToAction("IndexFooterEmail", "CrmTemplate");
            }

            // If ModelState is not valid, return the view with errors
            return View(model);
        }

        [HttpGet]
        public ActionResult EditFooterEmail(int? Id)
        {
            var model = SqlHelper.QuerySP<CrmEmailFooterViewModel>("spCrm_EmailFooter_SelectById", new
            {
                pID = Id,
            }).FirstOrDefault(s => s.Id == Id);

            if (model != null && model.IsDeleted != true)
            {

                return View(model);
            }

            return RedirectToAction("IndexFooterEmail");
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult EditFooterEmail(CrmEmailFooterViewModel model)
        {
            if (ModelState.IsValid)
            {
                SqlHelper.QuerySP<CrmEmailFooterViewModel>("spCrm_EmailFooter_Edit", new
                {
                    @Id = model.Id,
                    @Logs = model.Logs,
                    @pModifiedUserId = WebSecurity.CurrentUserId,
                });
                TempData[Globals.SuccessMessageKey] = App_GlobalResources.Wording.UpdateSuccess;
                return RedirectToAction("IndexFooterEmail", "CrmTemplate");
            }

            // If ModelState is not valid, return the view with errors
            return View(model);
        }

        [HttpPost]
        public ActionResult DeleteFooterEmail(List<string> selectedIds)
        {
            try
            {
                if (selectedIds != null && selectedIds.Count > 0)
                {
                    foreach (var id in selectedIds)
                    {
                        // Kiểm tra tính hợp lệ của id
                        if (!string.IsNullOrEmpty(id))
                        {
                            SqlHelper.QuerySP<CrmEmailFooterViewModel>("spCrm_EmailFooter_Delete", new
                            {
                                @Id = id,
                                @ModifiedUserId = WebSecurity.CurrentUserId,
                            });
                        }
                    }

                    TempData[Globals.SuccessMessageKey] = App_GlobalResources.Wording.DeleteSuccess;
                }
                else
                {
                    TempData[Globals.FailedMessageKey] = "No item selected for deletion.";
                }
            }
            catch (Exception ex)
            {
                TempData[Globals.FailedMessageKey] = ex.Message;
            }

            return RedirectToAction("IndexFooterEmail", "CrmTemplate");
        }


        public ActionResult DetailFooterEmail(int? Id)
        {
            var model = SqlHelper.QuerySP<CrmEmailFooterViewModel>("spCrm_EmailFooter_SelectById", new
            {
                pID = Id,
            }).FirstOrDefault(s => s.Id == Id);

            if (model != null && model.IsDeleted != true)
            {

                return View(model);
            }

            return RedirectToAction("IndexFooterEmail");
        }
        #endregion

    }
}
