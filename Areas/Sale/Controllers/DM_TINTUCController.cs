using System.Globalization;
using Erp.BackOffice.Sale.Models;
using Erp.BackOffice.Filters;
using Erp.Domain.Sale.Entities;
using Erp.Domain.Interfaces;
using Erp.Domain.Sale.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Erp.Utilities;
using WebMatrix.WebData;
using Erp.BackOffice.Helpers;
using System.Web.Script.Serialization;
using System.IO;
using System.Text.RegularExpressions;
using Erp.Domain.Entities;
using System.Transactions;

namespace Erp.BackOffice.Sale.Controllers
{
    [Authorize]
    [InitializeSimpleMembership]
    [Erp.BackOffice.Helpers.NoCacheHelper]
    public class DM_TINTUCController : Controller
    {
        private readonly IDM_NHOMTINRepositories dm_nhomtinRepository;
        private readonly IDM_TINTUCRepositories dm_tintucRepository;
        private readonly IDM_BAIVE_YHLRepositories dm_baiveyhlRepository;
        private readonly IDM_TINTUC_tagsRepositories dm_tintuc_tagRepository;
        private readonly IDM_TINTUC_tags_listRepositories dm_tintuc_taglistRepository;
        public DM_TINTUCController(
            IDM_NHOMTINRepositories dm_nhomtin
            , IDM_TINTUCRepositories dm_tintuc
            , IDM_BAIVE_YHLRepositories dm_baiveyhl
            , IDM_TINTUC_tagsRepositories dm_tag
            , IDM_TINTUC_tags_listRepositories dm_tag_list
            )
        {
            dm_nhomtinRepository = dm_nhomtin;
            dm_tintucRepository = dm_tintuc;
            dm_baiveyhlRepository = dm_baiveyhl;
            dm_tintuc_tagRepository = dm_tag;
            dm_tintuc_taglistRepository = dm_tag_list;

        }

        #region Index
        public ViewResult Index(string txtSearch, int? NHOMTIN_ID, SearchObjectAttributeViewModel SearchOjectAttr, int? AmountPage)
        {
            //string pMA_DVIQLY = Helpers.Common.CurrentUser.MA_DVIQLY;
            var q = dm_tintucRepository.GetAllDM_TINTUC().AsEnumerable()
            .Select(item => new DM_TINTUCViewModel
            {
                TINTUC_ID = item.TINTUC_ID,
                CreatedUserId = item.CreatedUserId,
                CreatedDate = item.CreatedDate,
                ModifiedUserId = item.ModifiedUserId,
                ModifiedDate = item.ModifiedDate,
                NHOMTIN_ID = item.NHOMTIN_ID,
                TIEUDE = item.TIEUDE,
                TOMTAT = item.TOMTAT,
                NOIDUNG = item.NOIDUNG,
                ANH_DAIDIEN = item.ANH_DAIDIEN,
                STT = item.STT,
                IS_NOIBAT = item.IS_NOIBAT,
                IS_SHOW = item.IS_SHOW
            }).OrderByDescending(m => m.ModifiedDate).ToList();

            HttpRequestBase request = this.HttpContext.Request;
            string strBrandID = "0";
            string pAmountPage = "45";
            if (request.Cookies["BRANCH_ID_CookieName"] != null)
            {
                strBrandID = request.Cookies["BRANCH_ID_CookieName"].Value;
                if (strBrandID == "")
                {
                    strBrandID = "0";
                }
            }


            //begin get cookie numberow
            if (request.Cookies["NUMBERROW_INVOICE_CookieName"] != null)
            {
                pAmountPage = request.Cookies["NUMBERROW_INVOICE_CookieName"].Value;
                if (pAmountPage == "")
                {
                    pAmountPage = "45";
                }
            }



            if (pAmountPage != null)
            {
                ViewBag.AmountPage = int.Parse(pAmountPage);

            }
            else

            {
                ViewBag.pAmountPage = 45;
            }
            if (NHOMTIN_ID != null && NHOMTIN_ID > 0)
            {
                q = q.Where(x => x.NHOMTIN_ID == NHOMTIN_ID).ToList();
            }
            if (string.IsNullOrEmpty(txtSearch) == false)
            {
                txtSearch = txtSearch == "" ? "~" : Erp.BackOffice.Helpers.Common.ChuyenThanhKhongDau(txtSearch);

                q = q.Where(x => Erp.BackOffice.Helpers.Common.ChuyenThanhKhongDau(x.TIEUDE).Contains(txtSearch)).ToList();
            }


            foreach (var item in q)
            {
                var nhomtin = dm_nhomtinRepository.GetDM_NHOMTINByNHOMTIN_ID(item.NHOMTIN_ID);
                if (nhomtin != null)
                {
                    item.TEN_NhomTin = nhomtin.TEN_LOAISANPHAM;
                }
            }

            q = q.ToList();
            var nhomtinList = dm_nhomtinRepository.GetAllDM_NHOMTIN().AsEnumerable();

            var model = new DM_TINTUCViewModel();
            nhomtinList = nhomtinList.ToList();
            var _nhomtinList = nhomtinList.Select(item => new SelectListItem
            {
                Text = item.TEN_LOAISANPHAM,
                Value = item.NHOMTIN_ID.ToString()
            });
            ViewBag.nhomtinList = _nhomtinList;
            ViewBag.SuccessMessage = TempData["SuccessMessage"];
            ViewBag.FailedMessage = TempData["FailedMessage"];
            ViewBag.AlertMessage = TempData["AlertMessage"];
            return View(q);
        }
        #endregion

        #region Create
        public ActionResult Create()
        {
            //string pMA_DVIQLY = Helpers.Common.CurrentUser.MA_DVIQLY;
            var nhomtinList = dm_nhomtinRepository.GetAllDM_NHOMTIN().AsEnumerable();

            var model = new DM_TINTUCViewModel();
            nhomtinList = nhomtinList.ToList();
            var _nhomtinList = nhomtinList.Select(item => new SelectListItem
            {
                Text = item.TEN_LOAISANPHAM,
                Value = item.NHOMTIN_ID.ToString()
            });
            ViewBag.nhomtinList = _nhomtinList;
            return View(model);
        }

        [System.Web.Mvc.HttpPost]
        [ValidateInput(false)]
        public ActionResult Create(DM_TINTUCViewModel model)
        {
            //string pMA_DVIQLY = Helpers.Common.CurrentUser.MA_DVIQLY;
            //begin reset cache tin tuc
            Helpers.Common.ResetCacheYHL("tintucSpa_ViewPlayout,khuyenmai_ViewPlayout,tintucchitiet_ViewPlayout,nhomtintucSpa_ViewPlayout,FooterMainPartial_Mainkissy");
            //end reset cache tin tuc
            if (ModelState.IsValid)
            {

                using (var scope = new TransactionScope(TransactionScopeOption.Required, new System.TimeSpan(0, 30, 0)))
                {

                    var dm_tintuc = new Domain.Sale.Entities.DM_TINTUC();
                    var dm_tintuc_tag = new Domain.Sale.Entities.DM_TINTUC_tags();
                    AutoMapper.Mapper.Map(model, dm_tintuc);
                    dm_tintuc.MA_DVIQLY = "PE0800";
                    dm_tintuc.IsDeleted = false;
                    dm_tintuc.CreatedUserId = WebSecurity.CurrentUserId;
                    dm_tintuc.ModifiedUserId = WebSecurity.CurrentUserId;
                    dm_tintuc.CreatedDate = DateTime.Now;
                    dm_tintuc.ModifiedDate = DateTime.Now;
                    dm_tintuc.AssignedUserId = WebSecurity.CurrentUserId;
                    dm_tintuc.ANH_DAIDIEN = "";
                    dm_tintuc.IS_SHOW = Convert.ToInt32(Request["is-show-checkbox"]);
                    dm_tintuc.IS_NOIBAT = Convert.ToInt32(Request["is-noibat-checkbox"]);
                    dm_tintuc.META_TITLE = model.META_TITLE;
                    dm_tintuc.META_DESCRIPTION = model.META_DESCRIPTION;
                    dm_tintuc.NOIDUNG = model.NOIDUNG;
                    dm_tintuc.URL_SLUG = model.URL_SLUG;
                    dm_tintucRepository.InsertDM_TINTUC(dm_tintuc);
                    if (!string.IsNullOrEmpty(model.LIST_TAGS))
                    {
                        string[] tags = model.LIST_TAGS.Split(',');
                        foreach (var item in tags)
                        {
                            var tagId = Helpers.Common.ToUnsignString(item);
                            var existedTag = CheckTag(tagId);


                            //insert to to tag table
                            if (!existedTag)
                            {

                                dm_tintuc_taglistRepository.InsertDM_TINTUC_TagList(tagId, item);
                            }
                            dm_tintuc_tagRepository.InsertContentTag(dm_tintuc.TINTUC_ID, tagId);
                        }

                    }

                    dm_tintuc = dm_tintucRepository.GetDM_TINTUCByTINTUC_ID(dm_tintuc.TINTUC_ID);
                    //Xử lý tag
                    var Tag = dm_tintuc_tagRepository.GetAllDM_TINTUC_TagsByTINTUC_ID(model.TINTUC_ID);

                    //var taglist = dm_tintuc_taglistRepository.GetDM_TINTUC_tags_list_ByTagList_ID()
                    //begin up hinh anh cho backend
                    var path = System.Web.HttpContext.Current.Server.MapPath("~" + Helpers.Common.GetSetting("tintuc-image-folder"));
                    if (Request.Files["file-image"] != null)
                    {
                        var file = Request.Files["file-image"];
                        if (file.ContentLength > 0)
                        {
                            string image_name = "tintuc_" + Helpers.Common.ChuyenThanhKhongDau(Regex.Replace(dm_tintuc.TINTUC_ID.ToString(), @"\s+", "_")) + file.FileName;
                            bool isExists = System.IO.Directory.Exists(path);
                            if (!isExists)
                                System.IO.Directory.CreateDirectory(path);
                            file.SaveAs(path + image_name);
                            dm_tintuc.ANH_DAIDIEN = image_name;
                        }
                    }
                    //end up hinh anh cho backend

                    //begin up hinh anh cho client
                    path = Helpers.Common.GetSetting("tintuc-image-folder-client");
                    string prjClient = Helpers.Common.GetSetting("prj-client");
                    string filepath = System.Web.HttpContext.Current.Server.MapPath("~");
                    DirectoryInfo di = new DirectoryInfo(filepath);
                    filepath = di.Parent.FullName + "\\" + prjClient + path.Replace("/", @"\");
                    if (Request.Files["file-image"] != null)
                    {
                        var file = Request.Files["file-image"];
                        if (file.ContentLength > 0)
                        {
                            FileInfo fi = new FileInfo(Server.MapPath("~" + path) + dm_tintuc.ANH_DAIDIEN);
                            if (fi.Exists)
                            {
                                fi.Delete();
                            }

                            string image_name = "tintuc_" + Helpers.Common.ChuyenThanhKhongDau(Regex.Replace(dm_tintuc.TINTUC_ID.ToString(), @"\s+", "_")) + file.FileName;

                            bool isExists = System.IO.Directory.Exists(filepath);
                            if (!isExists)
                                System.IO.Directory.CreateDirectory(filepath);
                            file.SaveAs(filepath + image_name);
                            dm_tintuc.ANH_DAIDIEN = image_name;
                        }
                    }
                    //end up hinh anh cho client




                    dm_tintucRepository.UpdateDM_TINTUC(dm_tintuc);
                    Erp.BackOffice.Controllers.HomeController.WriteLog(dm_tintuc.NHOMTIN_ID, dm_tintuc.META_TITLE, "đã cập nhật tin tức ", "DM_TINTUC/Edit/" + dm_tintuc.NHOMTIN_ID, Helpers.Common.CurrentUser.BranchId.Value);
                    Helpers.Common.ResetCacheYHL("khuyenmai_ViewPlayout,tintucchitiet_ViewPlayout,TinNoiBatPartial_Mainkissy,");


                    if (string.IsNullOrEmpty(Request["IsPopup"]) == false)
                    {
                        return RedirectToAction("Edit", new { Id = model.TINTUC_ID, IsPopup = Request["IsPopup"] });
                    }

                    scope.Complete();

                    TempData[Globals.SuccessMessageKey] = App_GlobalResources.Wording.InsertSuccess;
                    return RedirectToAction("Index");
                }
            }
            return View(model);
        }
        #endregion

        #region Edit
        public ActionResult Edit(int Id)
        {
            //string pMA_DVIQLY = Helpers.Common.CurrentUser.MA_DVIQLY;
            var TagList = dm_tintuc_taglistRepository.GetAllDM_TINTUC_tags_list().AsEnumerable();

            var nhomtinList = dm_nhomtinRepository.GetAllDM_NHOMTIN().AsEnumerable();
            var TinTuc = dm_tintucRepository.GetDM_TINTUCByTINTUC_ID(Id);
            if (TinTuc != null && TinTuc.IsDeleted != true)
            {
                var model = new DM_TINTUCViewModel();
                var tag = new DM_TINTUC_tags();
                AutoMapper.Mapper.Map(TinTuc, model);




                nhomtinList = nhomtinList.ToList();
                var _nhomtinList = nhomtinList.Select(item => new SelectListItem
                {
                    Text = item.TEN_LOAISANPHAM,
                    Value = item.NHOMTIN_ID.ToString()
                });

                ViewBag.nhomtinList = _nhomtinList;

                //TagList = TagList.ToList();
                //var _tagList = TagList.Select(item => new SelectListItem
                //{
                //    Text = item.Name,
                //    Value = item.Id.ToString()
                //});
                //ViewBag.tagList = _tagList;
                return View(model);
            }
            if (Request.UrlReferrer != null)
                return Redirect(Request.UrlReferrer.AbsoluteUri);
            return RedirectToAction("Index");
        }



        [System.Web.Mvc.HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(DM_TINTUCViewModel model)
        {
            Helpers.Common.ResetCacheYHL("tintucSpa_ViewPlayout,khuyenmai_ViewPlayout,tintucchitiet_ViewPlayout,nhomtintucSpa_ViewPlayout,FooterMainPartial_Mainkissy");
            if (ModelState.IsValid)
            {

                using (var scope = new TransactionScope(TransactionScopeOption.Required, new System.TimeSpan(0, 30, 0)))
                {
                    try
                    {
                        var dm_tintuc = dm_tintucRepository.GetDM_TINTUCByTINTUC_ID(model.TINTUC_ID);
                        // dm_tintuc.MA_DVIQLY = pMA_DVIQLY;
                        dm_tintuc.ModifiedUserId = WebSecurity.CurrentUserId;
                        dm_tintuc.ModifiedDate = DateTime.Now;
                        dm_tintuc.AssignedUserId = WebSecurity.CurrentUserId;
                        dm_tintuc.NHOMTIN_ID = model.NHOMTIN_ID;
                        dm_tintuc.TIEUDE = model.TIEUDE;
                        dm_tintuc.TOMTAT = model.TOMTAT;
                        dm_tintuc.NOIDUNG = model.NOIDUNG;
                        dm_tintuc.STT = model.STT;
                        dm_tintuc.IS_SHOW = Convert.ToInt32(Request["is-show-checkbox"]);
                        dm_tintuc.IS_NOIBAT = Convert.ToInt32(Request["is-noibat-checkbox"]);
                        dm_tintuc.META_TITLE = model.META_TITLE;
                        dm_tintuc.META_DESCRIPTION = model.META_DESCRIPTION;
                        dm_tintuc.URL_SLUG = model.URL_SLUG;
                        if (model.LIST_TAGS != null)
                        {
                            dm_tintuc.LIST_TAGS = model.LIST_TAGS.Trim();
                        }

                        if (!string.IsNullOrEmpty(model.LIST_TAGS))
                        {
                            dm_tintuc_tagRepository.RemoveAllContentTag(model.TINTUC_ID);
                            string[] tags = model.LIST_TAGS.Trim().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(user => user.Trim().ToUpper()).Distinct().Aggregate((users, user) => users + "," + user).Split(',');

                            foreach (var item in tags)
                            {
                                var tagId = Helpers.Common.ToUnsignString(item);
                                var existedTag = CheckTag(tagId);

                                //insert to to tag table
                                if (!existedTag)
                                {
                                    dm_tintuc_taglistRepository.InsertDM_TINTUC_TagList(tagId, item);
                                }

                                //insert to content tag
                                //this.InsertContentTag(content.ID, tagId);
                                dm_tintuc_tagRepository.InsertContentTag(model.TINTUC_ID, tagId);

                            }

                        }
                        //begin up hinh anh cho backend
                        var path = Helpers.Common.GetSetting("tintuc-image-folder");
                        var filepath = System.Web.HttpContext.Current.Server.MapPath("~" + path);
                        if (Request.Files["file-image"] != null)
                        {
                            var file = Request.Files["file-image"];
                            if (file.ContentLength > 0)
                            {
                                FileInfo fi = new FileInfo(Server.MapPath("~" + path) + dm_tintuc.ANH_DAIDIEN);
                                if (fi.Exists)
                                {
                                    fi.Delete();
                                }

                                string image_name = "tintuc_" + Helpers.Common.ChuyenThanhKhongDau(Regex.Replace(dm_tintuc.TINTUC_ID.ToString(), @"\s+", "_")) + file.FileName;

                                bool isExists = System.IO.Directory.Exists(filepath);
                                if (!isExists)
                                    System.IO.Directory.CreateDirectory(filepath);
                                file.SaveAs(filepath + image_name);
                                dm_tintuc.ANH_DAIDIEN = image_name;
                            }
                        }
                        //end up hinh anh cho backend

                        //begin up hinh anh cho client
                        path = Helpers.Common.GetSetting("tintuc-image-folder-client");
                        string prjClient = Helpers.Common.GetSetting("prj-client");
                        filepath = System.Web.HttpContext.Current.Server.MapPath("~");
                        DirectoryInfo di = new DirectoryInfo(filepath);
                        filepath = di.Parent.FullName + "\\" + prjClient + path.Replace("/", @"\");
                        if (Request.Files["file-image"] != null)
                        {
                            var file = Request.Files["file-image"];
                            if (file.ContentLength > 0)
                            {
                                FileInfo fi = new FileInfo(Server.MapPath("~" + path) + dm_tintuc.ANH_DAIDIEN);
                                if (fi.Exists)
                                {
                                    fi.Delete();
                                }

                                string image_name = "tintuc_" + Helpers.Common.ChuyenThanhKhongDau(Regex.Replace(dm_tintuc.TINTUC_ID.ToString(), @"\s+", "_")) + file.FileName;

                                bool isExists = System.IO.Directory.Exists(filepath);
                                if (!isExists)
                                    System.IO.Directory.CreateDirectory(filepath);
                                file.SaveAs(filepath + image_name);
                                dm_tintuc.ANH_DAIDIEN = image_name;
                            }
                        }
                        //end up hinh anh cho client





                        dm_tintucRepository.UpdateDM_TINTUC(dm_tintuc);
                        Helpers.Common.ResetCacheYHL("khuyenmai_ViewPlayout,tintucchitiet_ViewPlayout,TinNoiBatPartial_Mainkissy");
                        scope.Complete();
                    }
                    catch (Exception ex)
                    {
                        return Content("Fail");
                    }
                }

                TempData[Globals.SuccessMessageKey] = App_GlobalResources.Wording.UpdateSuccess;
                return RedirectToAction("Index");
            }
            return View(model);
        }
        #endregion

        public ActionResult CheckSTTExsist(int? id, int stt)
        {
            // string pMA_DVIQLY = Helpers.Common.CurrentUser.MA_DVIQLY;
            var tintuc = dm_tintucRepository.GetAllDM_TINTUC()
                .Where(item => item.STT == stt).FirstOrDefault();
            if (tintuc != null)
            {
                if (id == null || (id != null && tintuc.TINTUC_ID != id))
                    return Content("Trùng số thứ tự!");
                else
                {
                    return Content("");
                }
            }
            else
            {
                return Content("");
            }
        }

        public bool CheckTag(string tagId)
        {
            // string pMA_DVIQLY = Helpers.Common.CurrentUser.MA_DVIQLY;
            var TagList = dm_tintuc_taglistRepository.GetAllDM_TINTUC_tags_list().AsEnumerable();
            return TagList.Count(x => x.Id == tagId) > 0;
        }

        #region Delete
        [HttpPost]
        public ActionResult Delete()
        {
            // string pMA_DVIQLY = Helpers.Common.CurrentUser.MA_DVIQLY;
            Helpers.Common.ResetCacheYHL("tintucSpa_ViewPlayout,khuyenmai_ViewPlayout,tintucchitiet_ViewPlayout,nhomtintucSpa_ViewPlayout,FooterMainPartial_Mainkissy");
            try
            {
                string idDeleteAll = Request["DeleteId-checkbox"];
                string[] arrDeleteId = idDeleteAll.Split(',');
                for (int i = 0; i < arrDeleteId.Count(); i++)
                {
                    var item = dm_tintucRepository.GetDM_TINTUCByTINTUC_ID(int.Parse(arrDeleteId[i], CultureInfo.InvariantCulture));
                    if (item != null)
                    {
                        dm_tintucRepository.DeleteDM_TINTUC(item.TINTUC_ID);
                    }
                }
                Helpers.Common.ResetCacheYHL("khuyenmai_ViewPlayout,tintucchitiet_ViewPlayout,TinNoiBatPartial_Mainkissy");
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

        #region DM_NHOMTIN
        public ViewResult Dm_NhomTin()
        {
            // string pMA_DVIQLY = Helpers.Common.CurrentUser.MA_DVIQLY;
            var model = new DM_NHOMTINViewModel();


            var list = dm_nhomtinRepository.GetAllDM_NHOMTIN().AsEnumerable()
            .Select(item => new DM_NHOMTINViewModel
            {
                NHOMTIN_ID = item.NHOMTIN_ID,
                CreatedUserId = item.CreatedUserId,
                CreatedDate = item.CreatedDate,
                ModifiedUserId = item.ModifiedUserId,
                ModifiedDate = item.ModifiedDate,
                AssignedUserId = item.AssignedUserId,
                TEN_LOAISANPHAM = item.TEN_LOAISANPHAM,
                STT = item.STT,
                ANH_DAIDIEN = item.ANH_DAIDIEN,
                IS_SHOW = item.IS_SHOW,
                IsDeleted = item.IsDeleted,
                META_TITLE = item.META_TITLE,
                META_DESCRIPTION = item.META_DESCRIPTION,
                URL_SLUG = item.URL_SLUG

            }).OrderBy(x => x.STT).ToList();
            //list = AutoMapper.Mapper.Map(model, list);


            ViewBag.DM_NHOMTIN = list;
            ViewBag.SuccessMessage = TempData[Globals.SuccessMessageKey];
            ViewBag.FailedMessage = TempData[Globals.FailedMessageKey];
            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult CreateNhomTin(DM_NHOMTINViewModel model)
        {
            //string pMA_DVIQLY = Helpers.Common.CurrentUser.MA_DVIQLY;
            if (ModelState.IsValid)
            {
                Helpers.Common.ResetCacheYHL("tintucSpa_ViewPlayout,khuyenmai_ViewPlayout,tintucchitiet_ViewPlayout,nhomtintucSpa_ViewPlayout,FooterMainPartial_Mainkissy");
                //**create**//
                if (model.NHOMTIN_ID == 0)
                {
                    using (var scope = new TransactionScope(TransactionScopeOption.Required, new System.TimeSpan(0, 30, 0)))
                    {

                        var dm_nhomtin = new Domain.Sale.Entities.DM_NHOMTIN();
                        AutoMapper.Mapper.Map(model, dm_nhomtin);
                        //dm_nhomtin.MA_DVIQLY = pMA_DVIQLY;
                        dm_nhomtin.IsDeleted = false;
                        dm_nhomtin.CreatedUserId = WebSecurity.CurrentUserId;
                        dm_nhomtin.ModifiedUserId = WebSecurity.CurrentUserId;
                        dm_nhomtin.CreatedDate = DateTime.Now;
                        dm_nhomtin.ModifiedDate = DateTime.Now;
                        dm_nhomtin.AssignedUserId = WebSecurity.CurrentUserId;
                        dm_nhomtin.META_TITLE = model.META_TITLE;
                        dm_nhomtin.META_DESCRIPTION = model.META_DESCRIPTION;
                        dm_nhomtin.URL_SLUG = model.URL_SLUG;
                        dm_nhomtin.IS_SHOW = Convert.ToInt32(Request["is-show-checkbox"]);//???

                        dm_nhomtin.ANH_DAIDIEN = "";
                        var loaisanpham = dm_nhomtinRepository.GetDM_NHOMTINByNHOMTIN_ID(model.NHOMTIN_ID);

                        if (loaisanpham != null)
                        {
                            TempData[Globals.FailedMessageKey] = "Nhóm tin đã tồn tại";
                            return RedirectToAction("Dm_Nhomtin");
                        }

                        dm_nhomtinRepository.InsertDM_NHOMTIN(dm_nhomtin);

                        dm_nhomtin = dm_nhomtinRepository.GetDM_NHOMTINByNHOMTIN_ID(dm_nhomtin.NHOMTIN_ID);

                        //begin up hinh anh cho backend
                        var path = System.Web.HttpContext.Current.Server.MapPath("~" + Helpers.Common.GetSetting("nhomtin-image-folder"));
                        if (Request.Files["file-image"] != null)
                        {
                            var file = Request.Files["file-image"];
                            if (file.ContentLength > 0)
                            {
                                string image_name = "nhomtin_" + Helpers.Common.ChuyenThanhKhongDau(Regex.Replace(dm_nhomtin.NHOMTIN_ID.ToString(), @"\s+", "_")) + file.FileName;
                                bool isExists = System.IO.Directory.Exists(path);
                                if (!isExists)
                                    System.IO.Directory.CreateDirectory(path);
                                file.SaveAs(path + image_name);
                                dm_nhomtin.ANH_DAIDIEN = image_name;
                            }
                        }
                        //end up hinh anh cho backend

                        //begin up hinh anh cho client
                        path = Helpers.Common.GetSetting("nhomtin-image-folder-client");
                        string prjClient = Helpers.Common.GetSetting("prj-client");
                        string filepath = System.Web.HttpContext.Current.Server.MapPath("~");
                        DirectoryInfo di = new DirectoryInfo(filepath);
                        filepath = di.Parent.FullName + "\\" + prjClient + path.Replace("/", @"\");
                        if (Request.Files["file-image"] != null)
                        {
                            var file = Request.Files["file-image"];
                            if (file.ContentLength > 0)
                            {
                                FileInfo fi = new FileInfo(Server.MapPath("~" + path) + dm_nhomtin.ANH_DAIDIEN);
                                if (fi.Exists)
                                {
                                    fi.Delete();
                                }

                                string image_name = "nhomtin_" + Helpers.Common.ChuyenThanhKhongDau(Regex.Replace(dm_nhomtin.NHOMTIN_ID.ToString(), @"\s+", "_")) + file.FileName;

                                bool isExists = System.IO.Directory.Exists(filepath);
                                if (!isExists)
                                    System.IO.Directory.CreateDirectory(filepath);
                                file.SaveAs(filepath + image_name);
                                dm_nhomtin.ANH_DAIDIEN = image_name;
                            }
                        }
                        //end up hinh anh cho client






                        dm_nhomtinRepository.UpdateDM_NHOMTIN(dm_nhomtin);

                        scope.Complete();

                        TempData[Globals.SuccessMessageKey] = App_GlobalResources.Wording.InsertSuccess;
                        return RedirectToAction("Dm_NhomTin");
                    }
                }
                else//**edit**//
                {
                    var dm_nhomtin = dm_nhomtinRepository.GetDM_NHOMTINByNHOMTIN_ID(model.NHOMTIN_ID);
                    dm_nhomtin.ModifiedUserId = WebSecurity.CurrentUserId;
                    dm_nhomtin.ModifiedDate = DateTime.Now;
                    dm_nhomtin.AssignedUserId = WebSecurity.CurrentUserId;
                    dm_nhomtin.TEN_LOAISANPHAM = model.TEN_LOAISANPHAM;
                    dm_nhomtin.STT = model.STT;
                    dm_nhomtin.IS_SHOW = Convert.ToInt32(Request["is-show-checkbox"]);
                    dm_nhomtin.META_TITLE = model.META_TITLE;
                    dm_nhomtin.META_DESCRIPTION = model.META_DESCRIPTION;
                    dm_nhomtin.URL_SLUG = model.URL_SLUG;

                    var oldItem = dm_nhomtinRepository.GetDM_NHOMTINByNHOMTIN_ID(model.NHOMTIN_ID);

                    //begin up hinh anh cho backend
                    var path = Helpers.Common.GetSetting("nhomtin-image-folder");
                    var filepath = System.Web.HttpContext.Current.Server.MapPath("~" + path);
                    if (Request.Files["file-image"] != null)
                    {
                        var file = Request.Files["file-image"];
                        if (file.ContentLength > 0)
                        {
                            FileInfo fi = new FileInfo(Server.MapPath("~" + path) + dm_nhomtin.ANH_DAIDIEN);
                            if (fi.Exists)
                            {
                                fi.Delete();
                            }

                            string image_name = "nhomtin_" + Helpers.Common.ChuyenThanhKhongDau(Regex.Replace(dm_nhomtin.NHOMTIN_ID.ToString(), @"\s+", "_")) + file.FileName;

                            bool isExists = System.IO.Directory.Exists(filepath);
                            if (!isExists)
                                System.IO.Directory.CreateDirectory(filepath);
                            file.SaveAs(filepath + image_name);
                            dm_nhomtin.ANH_DAIDIEN = image_name;
                        }
                    }

                    //end up hinh anh cho backend



                    //begin up hinh anh cho client
                    path = Helpers.Common.GetSetting("nhomtin-image-folder-client");
                    string prjClient = Helpers.Common.GetSetting("prj-client");
                    filepath = System.Web.HttpContext.Current.Server.MapPath("~");
                    DirectoryInfo di = new DirectoryInfo(filepath);
                    filepath = di.Parent.FullName + "\\" + prjClient + path.Replace("/", @"\");
                    if (Request.Files["file-image"] != null)
                    {
                        var file = Request.Files["file-image"];
                        if (file.ContentLength > 0)
                        {
                            FileInfo fi = new FileInfo(Server.MapPath("~" + path) + dm_nhomtin.ANH_DAIDIEN);
                            if (fi.Exists)
                            {
                                fi.Delete();
                            }

                            string image_name = "nhomtin_" + Helpers.Common.ChuyenThanhKhongDau(Regex.Replace(dm_nhomtin.NHOMTIN_ID.ToString(), @"\s+", "_")) + file.FileName;

                            bool isExists = System.IO.Directory.Exists(filepath);
                            if (!isExists)
                                System.IO.Directory.CreateDirectory(filepath);
                            file.SaveAs(filepath + image_name);
                            dm_nhomtin.ANH_DAIDIEN = image_name;
                        }
                    }
                    //end up hinh anh cho client









                    dm_nhomtinRepository.UpdateDM_NHOMTIN(dm_nhomtin);

                    TempData[Globals.SuccessMessageKey] = App_GlobalResources.Wording.UpdateSuccess;
                    return RedirectToAction("Dm_NhomTin");

                }

            }
            return View();
        }

        [HttpPost]
        public ActionResult DeleteNhomTin()
        {
            //string pMA_DVIQLY = Helpers.Common.CurrentUser.MA_DVIQLY;
            Helpers.Common.ResetCacheYHL("tintucSpa_ViewPlayout,khuyenmai_ViewPlayout,tintucchitiet_ViewPlayout,nhomtintucSpa_ViewPlayout,FooterMainPartial_Mainkissy");
            int id = int.Parse(Request["Id"], CultureInfo.InvariantCulture);
            try
            {
                var item = dm_nhomtinRepository.GetDM_NHOMTINByNHOMTIN_ID(id);
                if (item != null)
                {
                    dm_nhomtinRepository.DeleteDM_NHOMTIN(item.NHOMTIN_ID);
                }
                TempData[Globals.SuccessMessageKey] = App_GlobalResources.Wording.DeleteSuccess;
                return RedirectToAction("DM_NhomTin");
            }
            catch (DbUpdateException)
            {
                TempData[Globals.FailedMessageKey] = App_GlobalResources.Error.RelationError;
                return RedirectToAction("DM_NhomTin");
            }
        }

        #endregion

        //#region DM_BaiveYHL
        //public ViewResult DM_BaiVe_YHL()
        //{
        //    string pMA_DVIQLY = Helpers.Common.CurrentUser.MA_DVIQLY;
        //    var model = new DM_BAIVE_YHLViewModel();


        //    var list = dm_baiveyhlRepository.GetAllDM_BAIVE_YHL(pMA_DVIQLY).AsEnumerable()
        //    .Select(item => new DM_BAIVE_YHLViewModel
        //    {
        //        BAIVE_YHL_ID = item.BAIVE_YHL_ID,
        //        CreatedUserId = item.CreatedUserId,
        //        CreatedDate = item.CreatedDate,
        //        ModifiedUserId = item.ModifiedUserId,
        //        ModifiedDate = item.ModifiedDate,
        //        AssignedUserId = item.AssignedUserId,
        //        TIEUDE = item.TIEUDE,
        //        TOMTAT = item.TOMTAT,
        //        HINHANH = item.HINHANH,
        //        IS_SHOW = item.IS_SHOW,
        //        NOIDUNG = item.NOIDUNG,
        //        IsDeleted = item.IsDeleted
        //    }).OrderBy(x => x.BAIVE_YHL_ID).ToList();
        //    //list = AutoMapper.Mapper.Map(model, list);


        //    ViewBag.DM_BAIVE_YHL = list;
        //    ViewBag.SuccessMessage = TempData[Globals.SuccessMessageKey];
        //    ViewBag.FailedMessage = TempData[Globals.FailedMessageKey];
        //    return View(model);
        //}

        //[HttpPost]
        //[ValidateInput(false)]
        //public ActionResult CreateBaiveYHL(DM_BAIVE_YHLViewModel model)
        //{
        //    string pMA_DVIQLY = Helpers.Common.CurrentUser.MA_DVIQLY;
        //    if (ModelState.IsValid)
        //    {

        //        //**create**//
        //        if (model.BAIVE_YHL_ID == 0)
        //        {
        //            var dm_baiveyhl = new Domain.Sale.Entities.DM_BAIVE_YHL();
        //            AutoMapper.Mapper.Map(model, dm_baiveyhl);
        //            dm_baiveyhl.MA_DVIQLY = pMA_DVIQLY;
        //            dm_baiveyhl.IsDeleted = false;
        //            dm_baiveyhl.CreatedUserId = WebSecurity.CurrentUserId;
        //            dm_baiveyhl.ModifiedUserId = WebSecurity.CurrentUserId;
        //            dm_baiveyhl.CreatedDate = DateTime.Now;
        //            dm_baiveyhl.ModifiedDate = DateTime.Now;
        //            dm_baiveyhl.AssignedUserId = WebSecurity.CurrentUserId;
        //            //dm_baiveyhl.TIEUDE = model.TIEUDE;
        //            //dm_baiveyhl.TOMTAT = model.TOMTAT;
        //            //dm_baiveyhl.NOIDUNG = model.NOIDUNG;

        //            dm_baiveyhl.IS_SHOW = Convert.ToInt32(Request["is-show-checkbox"]);//???

        //            var loaisanpham = dm_baiveyhlRepository.GetDM_BAIVE_YHLByBAIVE_YHL_ID(pMA_DVIQLY,model.BAIVE_YHL_ID);

        //            if (loaisanpham != null)
        //            {
        //                TempData[Globals.FailedMessageKey] = "Bài đã tồn tại";
        //                return RedirectToAction("DM_BaiVe_YHL");
        //            }


        //            var path = System.Web.HttpContext.Current.Server.MapPath("~" + Helpers.Common.GetSetting(Erp.BackOffice.Helpers.Common.CurrentUser.MA_DVIQLY,"tintuc-image-folder"));
        //            if (Request.Files["file-image"] != null)
        //            {
        //                var file = Request.Files["file-image"];
        //                if (file.ContentLength > 0)
        //                {
        //                    string image_name = "baiyhl_" + Helpers.Common.ChuyenThanhKhongDau(Regex.Replace(dm_baiveyhl.BAIVE_YHL_ID.ToString(), @"\s+", "_")) + file.FileName;
        //                    bool isExists = System.IO.Directory.Exists(path);
        //                    if (!isExists)
        //                        System.IO.Directory.CreateDirectory(path);
        //                    file.SaveAs(path + image_name);
        //                    dm_baiveyhl.HINHANH = image_name;
        //                }
        //            }


        //            dm_baiveyhlRepository.InsertDM_BAIVE_YHL(dm_baiveyhl);

        //            TempData[Globals.SuccessMessageKey] = App_GlobalResources.Wording.InsertSuccess;
        //            return RedirectToAction("DM_BaiVe_YHL");
        //        }
        //        else//**edit**//
        //        {
        //            var dm_baiveyhl = dm_baiveyhlRepository.GetDM_BAIVE_YHLByBAIVE_YHL_ID(model.BAIVE_YHL_ID);
        //            dm_baiveyhl.ModifiedUserId = WebSecurity.CurrentUserId;
        //            dm_baiveyhl.ModifiedDate = DateTime.Now;
        //            dm_baiveyhl.AssignedUserId = WebSecurity.CurrentUserId;
        //            dm_baiveyhl.TIEUDE = model.TIEUDE;
        //            dm_baiveyhl.TOMTAT = model.TOMTAT;
        //            dm_baiveyhl.NOIDUNG = model.NOIDUNG;
        //            dm_baiveyhl.IS_SHOW = Convert.ToInt32(Request["is-show-checkbox"]);


        //            var oldItem = dm_baiveyhlRepository.GetDM_BAIVE_YHLByBAIVE_YHL_ID(model.BAIVE_YHL_ID);


        //            var path = Helpers.Common.GetSetting("tintuc-image-folder");
        //            var filepath = System.Web.HttpContext.Current.Server.MapPath("~" + path);
        //            if (Request.Files["file-image"] != null)
        //            {
        //                var file = Request.Files["file-image"];
        //                if (file.ContentLength > 0)
        //                {
        //                    FileInfo fi = new FileInfo(Server.MapPath("~" + path) + dm_baiveyhl.HINHANH);
        //                    if (fi.Exists)
        //                    {
        //                        fi.Delete();
        //                    }

        //                    string image_name = "baiyhl_" + Helpers.Common.ChuyenThanhKhongDau(Regex.Replace(dm_baiveyhl.BAIVE_YHL_ID.ToString(), @"\s+", "_")) +  file.FileName;

        //                    bool isExists = System.IO.Directory.Exists(filepath);
        //                    if (!isExists)
        //                        System.IO.Directory.CreateDirectory(filepath);
        //                    file.SaveAs(filepath + image_name);
        //                    dm_baiveyhl.HINHANH = image_name;
        //                }
        //            }


        //            dm_baiveyhlRepository.UpdateDM_BAIVE_YHL(dm_baiveyhl);

        //            TempData[Globals.SuccessMessageKey] = App_GlobalResources.Wording.UpdateSuccess;
        //            return RedirectToAction("DM_BaiVe_YHL");

        //        }

        //    }
        //    return View();
        //}

        //[HttpPost]
        //public ActionResult DeleteBaiveYHL()
        //{
        //    ///string pMA_DVIQLY = Helpers.Common.CurrentUser.MA_DVIQLY;
        //    int id = int.Parse(Request["Id"], CultureInfo.InvariantCulture);
        //    try
        //    {
        //        var item = dm_baiveyhlRepository.GetDM_BAIVE_YHLByBAIVE_YHL_ID(id);
        //        if (item != null)
        //        {
        //            dm_baiveyhlRepository.DeleteDM_BAIVE_YHL(item.BAIVE_YHL_ID);
        //        }
        //        TempData[Globals.SuccessMessageKey] = App_GlobalResources.Wording.DeleteSuccess;
        //        return RedirectToAction("DM_BaiVe_YHL");
        //    }
        //    catch (DbUpdateException)
        //    {
        //        TempData[Globals.FailedMessageKey] = App_GlobalResources.Error.RelationError;
        //        return RedirectToAction("DM_BaiVe_YHL");
        //    }
        //}
        //#endregion
    }
}