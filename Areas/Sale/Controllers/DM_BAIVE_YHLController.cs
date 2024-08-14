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
    public class DM_BAIVE_YHLController : Controller
    {
        private readonly IDM_BAIVE_YHLRepositories dm_baiveyhlRepository;
        private readonly IDM_LOAIBAIRepositories dm_loaibaiRepository;
        private readonly IDM_BAOCHIRepositories dm_baochiRepository;

        public DM_BAIVE_YHLController(
            IDM_BAIVE_YHLRepositories dm_baiveyhl
           , IDM_LOAIBAIRepositories dm_loaibai
           , IDM_BAOCHIRepositories dm_baochi)
        {
            dm_baiveyhlRepository = dm_baiveyhl;
            dm_loaibaiRepository = dm_loaibai;
            dm_baochiRepository = dm_baochi;
        }

        #region Index

        public ViewResult Index(string txtSearch)
        {
            //string pMA_DVIQLY = Helpers.Common.CurrentUser.MA_DVIQLY;

            var q = dm_baiveyhlRepository.GetAllDM_BAIVE_YHL().AsEnumerable()
            .Select(item => new DM_BAIVE_YHLViewModel
            {
                BAIVE_YHL_ID = item.BAIVE_YHL_ID,
                CreatedUserId = item.CreatedUserId,
                CreatedDate = item.CreatedDate,
                ModifiedUserId = item.ModifiedUserId,
                ModifiedDate = item.ModifiedDate,
                AssignedUserId = item.AssignedUserId,
                CODE_LOAIBAI = item.CODE_LOAIBAI,
                TIEUDE = item.TIEUDE,
                TOMTAT = item.TOMTAT,
                HINHANH = item.HINHANH,
                IS_SHOW = item.IS_SHOW,
                NOIDUNG = item.NOIDUNG,
                IsDeleted = item.IsDeleted

            }).OrderByDescending(m => m.ModifiedDate).ToList();

            foreach (var item in q)
            {              
                var nhomtin = dm_loaibaiRepository.GetDM_LOAIBAIByCODE_LOAIBAI( item.CODE_LOAIBAI);
                if (nhomtin != null)
                {
                    item.Ten_LoaiBai = nhomtin.TEN_LOAIBAI;
                }
            }

            q = q.ToList();
            if(string.IsNullOrEmpty(txtSearch) == false)
            {
                txtSearch = txtSearch == "" ? "~" : Erp.BackOffice.Helpers.Common.ChuyenThanhKhongDau(txtSearch);

                q = q.Where(x => Erp.BackOffice.Helpers.Common.ChuyenThanhKhongDau(x.TIEUDE).Contains(txtSearch)).ToList();
            }
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
            var loaibaiList = dm_loaibaiRepository.GetAllDM_LOAIBAI().AsEnumerable();

            var model = new DM_BAIVE_YHLViewModel();
            loaibaiList = loaibaiList.ToList();
            var _loaibaiList = loaibaiList.Select(item => new SelectListItem
            {
                Text = item.TEN_LOAIBAI,
                Value = item.CODE_LOAIBAI
            });
            ViewBag.loaibaiList = _loaibaiList;
            return View(model);
        }

        [System.Web.Mvc.HttpPost]
        [ValidateInput(false)]
        public ActionResult
            Create(DM_BAIVE_YHLViewModel model)
        {
            //string pMA_DVIQLY = Helpers.Common.CurrentUser.MA_DVIQLY;

            if (ModelState.IsValid)
            {
                using (var scope =  new TransactionScope(TransactionScopeOption.Required,new System.TimeSpan(0, 30, 0)))
                {


                    var dm_tintuc = new Domain.Sale.Entities.DM_BAIVE_YHL();
                    AutoMapper.Mapper.Map(model, dm_tintuc);
                    dm_tintuc.IsDeleted = false;
                    dm_tintuc.CreatedUserId = WebSecurity.CurrentUserId;
                    dm_tintuc.ModifiedUserId = WebSecurity.CurrentUserId;
                    dm_tintuc.CreatedDate = DateTime.Now;
                    dm_tintuc.ModifiedDate = DateTime.Now;
                    dm_tintuc.AssignedUserId = WebSecurity.CurrentUserId;
                    dm_tintuc.HINHANH = "";
                    dm_tintuc.IS_SHOW = Convert.ToInt32(Request["is-show-checkbox"]);
                    //dm_tintuc.MA_DVIQLY = pMA_DVIQLY;
                    dm_baiveyhlRepository.InsertDM_BAIVE_YHL(dm_tintuc);
                    
                    dm_tintuc = dm_baiveyhlRepository.GetDM_BAIVE_YHLByBAIVE_YHL_ID(dm_tintuc.BAIVE_YHL_ID);

                    //begin up hinh anh cho backend
                    var path = System.Web.HttpContext.Current.Server.MapPath("~" + Helpers.Common.GetSetting("tintuckhac-image-folder"));
                    if (Request.Files["file-image"] != null)
                    {
                        var file = Request.Files["file-image"];
                        if (file.ContentLength > 0)
                        {
                            string image_name = "tinkhac_" + Helpers.Common.ChuyenThanhKhongDau(Regex.Replace(dm_tintuc.BAIVE_YHL_ID.ToString(), @"\s+", "_")) +  file.FileName;
                            bool isExists = System.IO.Directory.Exists(path);
                            if (!isExists)
                                System.IO.Directory.CreateDirectory(path);
                            file.SaveAs(path + image_name);
                            dm_tintuc.HINHANH = image_name;
                        }
                    }

                    //end up hinh anh cho backend


                    //begin up hinh anh cho client
                    path = Helpers.Common.GetSetting("tintuckhac-image-folder-client");
                    string prjClient = Helpers.Common.GetSetting("prj-client");
                    string filepath = System.Web.HttpContext.Current.Server.MapPath("~");
                    DirectoryInfo di = new DirectoryInfo(filepath);
                    filepath = di.Parent.FullName + "\\" + prjClient + path.Replace("/", @"\");
                    if (Request.Files["file-image"] != null)
                    {
                        var file = Request.Files["file-image"];
                        if (file.ContentLength > 0)
                        {
                            FileInfo fi = new FileInfo(Server.MapPath("~" + path) + dm_tintuc.HINHANH);
                            if (fi.Exists)
                            {
                                fi.Delete();
                            }

                            string image_name = "tinkhac_" + Helpers.Common.ChuyenThanhKhongDau(Regex.Replace(dm_tintuc.BAIVE_YHL_ID.ToString(), @"\s+", "_")) + file.FileName;

                            bool isExists = System.IO.Directory.Exists(filepath);
                            if (!isExists)
                                System.IO.Directory.CreateDirectory(filepath);
                            file.SaveAs(filepath + image_name);
                            dm_tintuc.HINHANH = image_name;
                        }
                    }
                    //end up hinh anh cho client







                    dm_baiveyhlRepository.UpdateDM_BAIVE_YHL(dm_tintuc);
                    Erp.BackOffice.Controllers.HomeController.WriteLog(dm_tintuc.BAIVE_YHL_ID, dm_tintuc.CODE_LOAIBAI, "đã cập nhật bài viết", "DM_BAIVE_YHLView/Edit/" + dm_tintuc.BAIVE_YHL_ID, Helpers.Common.CurrentUser.BranchId.Value);
                    scope.Complete();

                    if (string.IsNullOrEmpty(Request["IsPopup"]) == false)
                    {
                        return RedirectToAction("Edit", new { Id = model.BAIVE_YHL_ID, IsPopup = Request["IsPopup"] });
                    }

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
           // string pMA_DVIQLY = Helpers.Common.CurrentUser.MA_DVIQLY;

            var loaibaiList = dm_loaibaiRepository.GetAllDM_LOAIBAI().AsEnumerable();
            var baiyhl = dm_baiveyhlRepository.GetDM_BAIVE_YHLByBAIVE_YHL_ID(Id);
            if (baiyhl != null && baiyhl.IsDeleted != true)
            {
                var model = new DM_BAIVE_YHLViewModel();
                AutoMapper.Mapper.Map(baiyhl, model);

                loaibaiList = loaibaiList.ToList();
                var _loaibaiList = loaibaiList.Select(item => new SelectListItem
                {
                    Text = item.TEN_LOAIBAI,
                    Value = item.CODE_LOAIBAI
                });
                ViewBag.loaibaiList = _loaibaiList;
                return View(model);
            }
            if (Request.UrlReferrer != null)
                return Redirect(Request.UrlReferrer.AbsoluteUri);
            return RedirectToAction("Index");
        }

        [System.Web.Mvc.HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(DM_BAIVE_YHLViewModel model)
        {
            //string pMA_DVIQLY = Helpers.Common.CurrentUser.MA_DVIQLY;

            if (ModelState.IsValid)
            {
                var dm_tintuc = dm_baiveyhlRepository.GetDM_BAIVE_YHLByBAIVE_YHL_ID( model.BAIVE_YHL_ID);
                dm_tintuc.ModifiedUserId = WebSecurity.CurrentUserId;
                dm_tintuc.ModifiedDate = DateTime.Now;
                dm_tintuc.AssignedUserId = WebSecurity.CurrentUserId;
                dm_tintuc.CODE_LOAIBAI = model.CODE_LOAIBAI;
                dm_tintuc.TIEUDE = model.TIEUDE;
                dm_tintuc.TOMTAT = model.TOMTAT;
                dm_tintuc.NOIDUNG = model.NOIDUNG;

                dm_tintuc.IS_SHOW = Convert.ToInt32(Request["is-show-checkbox"]);

                //begin up hinh anh cho backend
                var path = Helpers.Common.GetSetting("tintuckhac-image-folder");
                var filepath = System.Web.HttpContext.Current.Server.MapPath("~" + path);
                if (Request.Files["file-image"] != null)
                {
                    var file = Request.Files["file-image"];
                    if (file.ContentLength > 0)
                    {
                        FileInfo fi = new FileInfo(Server.MapPath("~" + path) + dm_tintuc.HINHANH);
                        if (fi.Exists)
                        {
                            fi.Delete();
                        }

                        string image_name = "tinkhac_" + Helpers.Common.ChuyenThanhKhongDau(Regex.Replace(dm_tintuc.BAIVE_YHL_ID.ToString(), @"\s+", "_")) +  file.FileName;

                        bool isExists = System.IO.Directory.Exists(filepath);
                        if (!isExists)
                            System.IO.Directory.CreateDirectory(filepath);
                        file.SaveAs(filepath + image_name);
                        dm_tintuc.HINHANH = image_name;
                    }
                }
                //end up hinh anh cho backend


                //begin up hinh anh cho client
                path = Helpers.Common.GetSetting("tintuckhac-image-folder-client");
                string prjClient = Helpers.Common.GetSetting("prj-client");
                filepath = System.Web.HttpContext.Current.Server.MapPath("~");
                DirectoryInfo di = new DirectoryInfo(filepath);
                filepath = di.Parent.FullName + "\\" + prjClient + path.Replace("/", @"\");
                if (Request.Files["file-image"] != null)
                {
                    var file = Request.Files["file-image"];
                    if (file.ContentLength > 0)
                    {
                        FileInfo fi = new FileInfo(Server.MapPath("~" + path) + dm_tintuc.HINHANH);
                        if (fi.Exists)
                        {
                            fi.Delete();
                        }

                        string image_name = "tinkhac_" + Helpers.Common.ChuyenThanhKhongDau(Regex.Replace(dm_tintuc.BAIVE_YHL_ID.ToString(), @"\s+", "_")) + file.FileName;

                        bool isExists = System.IO.Directory.Exists(filepath);
                        if (!isExists)
                            System.IO.Directory.CreateDirectory(filepath);
                        file.SaveAs(filepath + image_name);
                        dm_tintuc.HINHANH = image_name;
                    }
                }
                //end up hinh anh cho client


                dm_baiveyhlRepository.UpdateDM_BAIVE_YHL(dm_tintuc);

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
            //string pMA_DVIQLY = Helpers.Common.CurrentUser.MA_DVIQLY;

            try
            {
                string idDeleteAll = Request["DeleteId-checkbox"];
                string[] arrDeleteId = idDeleteAll.Split(',');
                for (int i = 0; i < arrDeleteId.Count(); i++)
                {
                    var item = dm_baiveyhlRepository.GetDM_BAIVE_YHLByBAIVE_YHL_ID( int.Parse(arrDeleteId[i], CultureInfo.InvariantCulture));
                    if (item != null)
                    {
                        dm_baiveyhlRepository.DeleteDM_BAIVE_YHL( item.BAIVE_YHL_ID);
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

        #region DM_BAOCHI
        public ViewResult Dm_BaoChi()
        {
            var model = new DM_BAOCHIViewModel();

            //string pMA_DVIQLY = Helpers.Common.CurrentUser.MA_DVIQLY;

            var list = dm_baochiRepository.GetAllDM_BAOCHI().AsEnumerable()
            .Select(item => new DM_BAOCHIViewModel
            {
                BAOCHI_ID = item.BAOCHI_ID,
                CreatedUserId = item.CreatedUserId,
                CreatedDate = item.CreatedDate,
                ModifiedUserId = item.ModifiedUserId,
                ModifiedDate = item.ModifiedDate,
                AssignedUserId = item.AssignedUserId,
                LINK_BAO = item.LINK_BAO,
                STT = item.STT,
                HINHANH = item.HINHANH,
                IS_SHOW = item.IS_SHOW,
                IsDeleted = item.IsDeleted
            }).OrderBy(x => x.STT).ToList();
            //list = AutoMapper.Mapper.Map(model, list);


            ViewBag.DM_BAOCHI = list;
            ViewBag.SuccessMessage = TempData[Globals.SuccessMessageKey];
            ViewBag.FailedMessage = TempData[Globals.FailedMessageKey];
            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult CreateBaoChi(DM_BAOCHIViewModel model)
        {
            //string pMA_DVIQLY = Helpers.Common.CurrentUser.MA_DVIQLY;

            if (ModelState.IsValid)
            {

                //**create**//
                if (model.BAOCHI_ID == 0)
                {
                    using (var scope =  new TransactionScope(TransactionScopeOption.Required,new System.TimeSpan(0, 30, 0)))
                    {

                        var dm_baochi = new Domain.Sale.Entities.DM_BAOCHI();
                        AutoMapper.Mapper.Map(model, dm_baochi);
                        dm_baochi.IsDeleted = false;
                        dm_baochi.CreatedUserId = WebSecurity.CurrentUserId;
                        dm_baochi.ModifiedUserId = WebSecurity.CurrentUserId;
                        dm_baochi.CreatedDate = DateTime.Now;
                        dm_baochi.ModifiedDate = DateTime.Now;
                        dm_baochi.AssignedUserId = WebSecurity.CurrentUserId;
                        dm_baochi.HINHANH = "";
                        dm_baochi.IS_SHOW = Convert.ToInt32(Request["is-show-checkbox"]);//???

                        var loaisanpham = dm_baochiRepository.GetDM_BAOCHIByBAOCHI_ID(model.BAOCHI_ID);

                        if (loaisanpham != null)
                        {
                            TempData[Globals.FailedMessageKey] = "Báo chí đã tồn tại";
                            return RedirectToAction("Dm_BaoChi");
                        }

                        dm_baochiRepository.InsertDM_BAOCHI(dm_baochi);
                        dm_baochi = dm_baochiRepository.GetDM_BAOCHIByBAOCHI_ID(dm_baochi.BAOCHI_ID);

                        //begin up hinh anh cho backend
                        var path = System.Web.HttpContext.Current.Server.MapPath("~" + Helpers.Common.GetSetting("baochi-image-folder"));
                        if (Request.Files["file-image"] != null)
                        {
                            var file = Request.Files["file-image"];
                            if (file.ContentLength > 0)
                            {
                                string image_name = "baochi_" + Helpers.Common.ChuyenThanhKhongDau(Regex.Replace(dm_baochi.BAOCHI_ID.ToString(), @"\s+", "_")) +file.FileName;
                                bool isExists = System.IO.Directory.Exists(path);
                                if (!isExists)
                                    System.IO.Directory.CreateDirectory(path);
                                file.SaveAs(path + image_name);
                                dm_baochi.HINHANH = image_name;
                            }
                        }
                        //end up hinh anh cho backend

                        //begin up hinh anh cho client
                        path = Helpers.Common.GetSetting("baochi-image-folder-client");
                        string prjClient = Helpers.Common.GetSetting("prj-client");
                        string filepath = System.Web.HttpContext.Current.Server.MapPath("~");
                        DirectoryInfo di = new DirectoryInfo(filepath);
                        filepath = di.Parent.FullName + "\\" + prjClient + path.Replace("/", @"\");
                        if (Request.Files["file-image"] != null)
                        {
                            var file = Request.Files["file-image"];
                            if (file.ContentLength > 0)
                            {
                                FileInfo fi = new FileInfo(Server.MapPath("~" + path) + dm_baochi.HINHANH);
                                if (fi.Exists)
                                {
                                    fi.Delete();
                                }

                                string image_name = "baochi_" + Helpers.Common.ChuyenThanhKhongDau(Regex.Replace(dm_baochi.BAOCHI_ID.ToString(), @"\s+", "_")) + file.FileName;

                                bool isExists = System.IO.Directory.Exists(filepath);
                                if (!isExists)
                                    System.IO.Directory.CreateDirectory(filepath);
                                file.SaveAs(filepath + image_name);
                                dm_baochi.HINHANH = image_name;
                            }
                        }
                        //end up hinh anh cho client






                        dm_baochiRepository.UpdateDM_BAOCHI(dm_baochi);
                        scope.Complete();

                        TempData[Globals.SuccessMessageKey] = App_GlobalResources.Wording.InsertSuccess;
                        return RedirectToAction("Dm_BaoChi");
                    }
                }
                else//**edit**//
                {
                    var dm_baochi = dm_baochiRepository.GetDM_BAOCHIByBAOCHI_ID(model.BAOCHI_ID);
                    dm_baochi.ModifiedUserId = WebSecurity.CurrentUserId;
                    dm_baochi.ModifiedDate = DateTime.Now;
                    dm_baochi.AssignedUserId = WebSecurity.CurrentUserId;
                    dm_baochi.LINK_BAO = model.LINK_BAO;

                    dm_baochi.STT = model.STT;
                    dm_baochi.IS_SHOW = Convert.ToInt32(Request["is-show-checkbox"]);


                    var oldItem = dm_baochiRepository.GetDM_BAOCHIByBAOCHI_ID( model.BAOCHI_ID);

                    //begin up hinh anh cho backend
                    var path = Helpers.Common.GetSetting("baochi-image-folder");
                    var filepath = System.Web.HttpContext.Current.Server.MapPath("~" + path);
                    if (Request.Files["file-image"] != null)
                    {
                        var file = Request.Files["file-image"];
                        if (file.ContentLength > 0)
                        {
                            FileInfo fi = new FileInfo(Server.MapPath("~" + path) + dm_baochi.HINHANH);
                            if (fi.Exists)
                            {
                                fi.Delete();
                            }

                            string image_name = "baochi_" + Helpers.Common.ChuyenThanhKhongDau(Regex.Replace(dm_baochi.BAOCHI_ID.ToString(), @"\s+", "_")) + file.FileName;

                            bool isExists = System.IO.Directory.Exists(filepath);
                            if (!isExists)
                                System.IO.Directory.CreateDirectory(filepath);
                            file.SaveAs(filepath + image_name);
                            dm_baochi.HINHANH = image_name;
                        }
                    }

                    //end up hinh anh cho backend



                    //begin up hinh anh cho client
                    path = Helpers.Common.GetSetting("baochi-image-folder-client");
                    string prjClient = Helpers.Common.GetSetting("prj-client");
                    filepath = System.Web.HttpContext.Current.Server.MapPath("~");
                    DirectoryInfo di = new DirectoryInfo(filepath);
                    filepath = di.Parent.FullName + "\\" + prjClient + path.Replace("/", @"\");
                    if (Request.Files["file-image"] != null)
                    {
                        var file = Request.Files["file-image"];
                        if (file.ContentLength > 0)
                        {
                            FileInfo fi = new FileInfo(Server.MapPath("~" + path) + dm_baochi.HINHANH);
                            if (fi.Exists)
                            {
                                fi.Delete();
                            }

                            string image_name = "baochi_" + Helpers.Common.ChuyenThanhKhongDau(Regex.Replace(dm_baochi.BAOCHI_ID.ToString(), @"\s+", "_")) + file.FileName;

                            bool isExists = System.IO.Directory.Exists(filepath);
                            if (!isExists)
                                System.IO.Directory.CreateDirectory(filepath);
                            file.SaveAs(filepath + image_name);
                            dm_baochi.HINHANH = image_name;
                        }
                    }
                    //end up hinh anh cho client









                    dm_baochiRepository.UpdateDM_BAOCHI(dm_baochi);

                    TempData[Globals.SuccessMessageKey] = App_GlobalResources.Wording.UpdateSuccess;
                    return RedirectToAction("Dm_BaoChi");

                }

            }
            return View();
        }

        [HttpPost]
        public ActionResult DeleteBaoChi()
        {
            //string pMA_DVIQLY = Helpers.Common.CurrentUser.MA_DVIQLY;

            int id = int.Parse(Request["Id"], CultureInfo.InvariantCulture);
            try
            {
                var item = dm_baochiRepository.GetDM_BAOCHIByBAOCHI_ID( id);
                if (item != null)
                {
                    dm_baochiRepository.DeleteDM_BAOCHI(item.BAOCHI_ID);
                }
                TempData[Globals.SuccessMessageKey] = App_GlobalResources.Wording.DeleteSuccess;
                return RedirectToAction("Dm_BaoChi");
            }
            catch (DbUpdateException)
            {
                TempData[Globals.FailedMessageKey] = App_GlobalResources.Error.RelationError;
                return RedirectToAction("Dm_BaoChi");
            }
        }
        #endregion
    }
}
