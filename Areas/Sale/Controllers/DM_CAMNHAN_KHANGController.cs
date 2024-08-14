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
    public class DM_CAMNHAN_KHANGController : Controller
    {
        private readonly IDM_CAMNHAN_KHANGRepositories dm_camnhan_khangRepository;
        private readonly IDM_BANNER_SLIDERRepositories dm_bannersliderRepository;
        public DM_CAMNHAN_KHANGController(IDM_CAMNHAN_KHANGRepositories dm_camnhan_khang,
             IDM_BANNER_SLIDERRepositories bannerslider
            )
        {
            dm_camnhan_khangRepository = dm_camnhan_khang;
            dm_bannersliderRepository = bannerslider;
        }


        public ActionResult CheckSTTExsist(int? id, int stt)
        {
            //string pMA_DVIQLY = Helpers.Common.CurrentUser.MA_DVIQLY;
            var tintuc = dm_camnhan_khangRepository.GetAllDM_CAMNHAN_KHANG()
                .Where(item => item.STT == stt).FirstOrDefault();
            if (tintuc != null)
            {
                if (id == null || (id != null && tintuc.CAMNHAN_KHANG_ID != id))
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


        #region Index
        public ViewResult Index(string txtSearch)
        {
            //string pMA_DVIQLY = Helpers.Common.CurrentUser.MA_DVIQLY;
            var q = dm_camnhan_khangRepository.GetAllDM_CAMNHAN_KHANG().AsEnumerable()
           .Select(item => new DM_CAMNHAN_KHANGViewModel
           {
               CAMNHAN_KHANG_ID = item.CAMNHAN_KHANG_ID,
               CreatedUserId = item.CreatedUserId,
               CreatedDate = item.CreatedDate,
               ModifiedUserId = item.ModifiedUserId,
               ModifiedDate = item.ModifiedDate,
               IsDeleted = item.IsDeleted,
               AssignedUserId = item.AssignedUserId,
               TIEUDE = item.TIEUDE,
               LINK_VIDEO = item.LINK_VIDEO,
               STT = item.STT,
               IS_SHOW = item.IS_SHOW


           }).OrderBy(m => m.STT).ToList();

            if (string.IsNullOrEmpty(txtSearch) == false)
            {
                txtSearch = txtSearch == "" ? "~" : Erp.BackOffice.Helpers.Common.ChuyenThanhKhongDau(txtSearch);

                q = q.Where(x => Erp.BackOffice.Helpers.Common.ChuyenThanhKhongDau(x.TIEUDE).Contains(txtSearch)).ToList();
            }
            return View(q);
        }
        #endregion

        #region Create
        public ActionResult Create()
        {
            //string pMA_DVIQLY = Helpers.Common.CurrentUser.MA_DVIQLY;
            var camnhanlist = dm_camnhan_khangRepository.GetAllDM_CAMNHAN_KHANG().AsEnumerable();
            var model = new DM_CAMNHAN_KHANGViewModel();
            camnhanlist = camnhanlist.ToList();
            var _camnhanlist = camnhanlist.Select(item => new SelectListItem
                {
                    Text = item.TIEUDE,
                    Value = item.CAMNHAN_KHANG_ID.ToString()

                });
            ViewBag.camnhalist = _camnhanlist;
            return View(model);
        }
        [System.Web.Mvc.HttpPost]
        [ValidateInput(false)]
        public ActionResult Create(DM_CAMNHAN_KHANGViewModel model)
        {
            //begin reset cache cam nhan khach hang
            Helpers.Common.ResetCacheYHL("SliderPartial_Mainkissy,camnhankhachhangSpa_ViewPlayout");
            //end reset cache cam nhan khach hang
            if (ModelState.IsValid)
            {
                var dm_tintuc = new Domain.Sale.Entities.DM_CAMNHAN_KHANG();
                AutoMapper.Mapper.Map(model, dm_tintuc);
                dm_tintuc.IsDeleted = false;
                dm_tintuc.CreatedUserId = WebSecurity.CurrentUserId;
                dm_tintuc.ModifiedUserId = WebSecurity.CurrentUserId;
                dm_tintuc.CreatedDate = DateTime.Now;
                dm_tintuc.ModifiedDate = DateTime.Now;
                dm_tintuc.AssignedUserId = WebSecurity.CurrentUserId;
               // dm_tintuc.MA_DVIQLY = pMA_DVIQLY;
                dm_tintuc.IS_SHOW = Convert.ToInt32(Request["is-show-checkbox"]);
                dm_tintuc.LINK_VIDEO = model.LINK_VIDEO;
                dm_tintuc.TIEUDE = model.TIEUDE;
                dm_tintuc.STT = model.STT;



                dm_camnhan_khangRepository.InsertDM_CAMNHAN_KHANG(dm_tintuc);

                if (string.IsNullOrEmpty(Request["IsPopup"]) == false)
                {
                    return RedirectToAction("Edit", new { Id = model.CAMNHAN_KHANG_ID, IsPopup = Request["IsPopup"] });
                }

                TempData[Globals.SuccessMessageKey] = App_GlobalResources.Wording.InsertSuccess;
                return RedirectToAction("Index");
            }
            return View(model);
        }
        #endregion

        #region Edit
        public ActionResult Edit(int id)
        {
          //  string pMA_DVIQLY = Helpers.Common.CurrentUser.MA_DVIQLY;
            var nhomtinList = dm_camnhan_khangRepository.GetAllDM_CAMNHAN_KHANG().AsEnumerable();
            var TinTuc = dm_camnhan_khangRepository.GetDM_CAMNHAN_KHANGByCAMNHAN_KHANG_ID(id);
            if (TinTuc != null && TinTuc.IsDeleted != true)
            {
                var model = new DM_CAMNHAN_KHANGViewModel();
                AutoMapper.Mapper.Map(TinTuc, model);

                nhomtinList = nhomtinList.ToList();
                var _nhomtinList = nhomtinList.Select(item => new SelectListItem
                {
                    Text = item.TIEUDE,
                    Value = item.CAMNHAN_KHANG_ID.ToString()
                });
                ViewBag.nhomtinList = _nhomtinList;
                return View(model);
            }
            if (Request.UrlReferrer != null)
                return Redirect(Request.UrlReferrer.AbsoluteUri);
            return RedirectToAction("Index");
        }
        [System.Web.Mvc.HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(DM_CAMNHAN_KHANGViewModel model)
        {
            // string pMA_DVIQLY = Helpers.Common.CurrentUser.MA_DVIQLY;
            //begin reset cache cam nhan khach hang
            Helpers.Common.ResetCacheYHL("SliderPartial_Mainkissy,camnhankhachhangSpa_ViewPlayout");
            //end reset cache cam nhan khach hang
            if (ModelState.IsValid)
            {
                var dm_camnhan = dm_camnhan_khangRepository.GetDM_CAMNHAN_KHANGByCAMNHAN_KHANG_ID(model.CAMNHAN_KHANG_ID);
                dm_camnhan.ModifiedUserId = WebSecurity.CurrentUserId;
                dm_camnhan.ModifiedDate = DateTime.Now;
                dm_camnhan.AssignedUserId = WebSecurity.CurrentUserId;
                dm_camnhan.TIEUDE = model.TIEUDE;
                dm_camnhan.STT = model.STT;
                dm_camnhan.IS_SHOW = Convert.ToInt32(Request["is-show-checkbox"]);
                dm_camnhan.LINK_VIDEO = model.LINK_VIDEO;

              //  dm_camnhan.MA_DVIQLY = pMA_DVIQLY;

                dm_camnhan_khangRepository.UpdateDM_CAMNHAN_KHANG(dm_camnhan);
                int CamNhan_KHANGID = dm_camnhan.CAMNHAN_KHANG_ID;
                //ghi log 
                Erp.BackOffice.Controllers.HomeController.WriteLog(dm_camnhan.CAMNHAN_KHANG_ID, dm_camnhan.MA_DVIQLY, "đã cập nhật Banner Slider", "DM_CAMNHAN_KHANG/Edit/" + dm_camnhan.CAMNHAN_KHANG_ID, Helpers.Common.CurrentUser.BranchId.Value);

                TempData[Globals.SuccessMessageKey] = App_GlobalResources.Wording.UpdateSuccess;
                return RedirectToAction("Index");
            }

            return View(model);
        }
        #endregion

        #region Delete
        [System.Web.Mvc.HttpPost]
        public ActionResult Delete()
        {
            //string pMA_DVIQLY = Helpers.Common.CurrentUser.MA_DVIQLY;
            //begin reset cache cam nhan khach hang
            Helpers.Common.ResetCacheYHL("SliderPartial_Mainkissy,camnhankhachhangSpa_ViewPlayout");

            //end reset cache cam nhan khach hang

            try
            {
                string idDeleteAll = Request["DeleteId-checkbox"];
                string[] arrDeleteId = idDeleteAll.Split(',');
                for (int i = 0; i < arrDeleteId.Count(); i++)
                {
                    var item = dm_camnhan_khangRepository.GetDM_CAMNHAN_KHANGByCAMNHAN_KHANG_ID(int.Parse(arrDeleteId[i], CultureInfo.InvariantCulture));
                    if (item != null)
                    {
                        dm_camnhan_khangRepository.DeleteDM_CAMNHAN_KHANG(item.CAMNHAN_KHANG_ID);
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

            /*int id = int.Parse(Request["Id"], CultureInfo.InvariantCulture);
            try
            {
                var item = dm_camnhan_khangRepository.GetDM_CAMNHAN_KHANGByCAMNHAN_KHANG_ID(id);
                if (item != null)
                {
                    dm_camnhan_khangRepository.DeleteDM_CAMNHAN_KHANG(item.CAMNHAN_KHANG_ID);
                }
                TempData[Globals.SuccessMessageKey] = App_GlobalResources.Wording.DeleteSuccess;
                return RedirectToAction("Index");
            }
            catch (DbUpdateException)
            {
                TempData[Globals.FailedMessageKey] = App_GlobalResources.Error.RelationError;
                return RedirectToAction("Index");
            }*/
        }
        #endregion



        #region DM_BannerSlider
        public ViewResult DM_BannerSlider()
        {
            
            var model = new DM_BANNER_SLIDERViewModel();


            var list = dm_bannersliderRepository.GetAllDM_BANNER_SLIDER().AsEnumerable()
            .Select(item => new DM_BANNER_SLIDERViewModel
            {
                BANNER_SLIDER_ID = item.BANNER_SLIDER_ID,
                CreatedUserId = item.CreatedUserId,
                CreatedDate = item.CreatedDate,
                ModifiedUserId = item.ModifiedUserId,
                ModifiedDate = item.ModifiedDate,
                AssignedUserId = item.AssignedUserId,
                LINK = item.LINK,
                STT = item.STT,
                ANH_DAIDIEN = item.ANH_DAIDIEN,
                IS_SHOW = item.IS_SHOW,
                IS_MOBILE = item.IS_MOBILE,
                IsDeleted = item.IsDeleted,
               // MA_DVIQLY = pMA_DVIQLY,
                IsBrand = item.IsBrand,
                IsNgheSi = item.IsNgheSi,
                IsKH=item.IsKH,
                isDichVu=item.isDichVu,
                isSanPham=item.isSanPham,
                isLogoMobile=item.isLogoMobile,
                isFooterWeb=item.isFooterWeb
              //  IsKH = item.IsKH
            }).OrderBy(x => x.STT).ToList();
            //list = AutoMapper.Mapper.Map(model, list);


            ViewBag.DM_BANNER_SLIDER = list;
            ViewBag.SuccessMessage = TempData[Globals.SuccessMessageKey];
            ViewBag.FailedMessage = TempData[Globals.FailedMessageKey];
            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult CreateBannerSlider(DM_BANNER_SLIDERViewModel model)
        {
            //string pMA_DVIQLY = Helpers.Common.CurrentUser.MA_DVIQLY;
           
            if (ModelState.IsValid)
            {

                //**create**//
                if (model.BANNER_SLIDER_ID == 0)
                {
                    using (var scope = new TransactionScope(TransactionScopeOption.Required, new System.TimeSpan(0, 30, 0)))
                    {


                        var dm_bannerslider = new Domain.Sale.Entities.DM_BANNER_SLIDER();
                        AutoMapper.Mapper.Map(model, dm_bannerslider);
                       // dm_bannerslider.MA_DVIQLY = pMA_DVIQLY;
                        dm_bannerslider.IsDeleted = false;
                        dm_bannerslider.CreatedUserId = WebSecurity.CurrentUserId;
                        dm_bannerslider.ModifiedUserId = WebSecurity.CurrentUserId;
                        dm_bannerslider.CreatedDate = DateTime.Now;
                        dm_bannerslider.ModifiedDate = DateTime.Now;
                        dm_bannerslider.AssignedUserId = WebSecurity.CurrentUserId;

                        dm_bannerslider.IS_SHOW = Convert.ToInt32(Request["is-show-checkbox"]);//???
                        dm_bannerslider.IS_MOBILE = Convert.ToInt32(Request["is-mobile-checkbox"]);// check show img mobile
                        dm_bannerslider.IsBrand = Convert.ToBoolean(Request["is-brand-checkbox"]);// check thuong hieu
                        dm_bannerslider.IsNgheSi = Convert.ToInt32(Request["is-nghesi-checkbox"]);
                        dm_bannerslider.IsKH = Convert.ToBoolean(Request["is-KH-checkbox"]);
                        dm_bannerslider.isDichVu = Convert.ToInt32(Request["is-DV-checkbox"]);
                        dm_bannerslider.isSanPham = Convert.ToInt32(Request["is-SP-checkbox"]);
                        dm_bannerslider.isLogoMobile = Convert.ToInt32(Request["is-LGMB-checkbox"]);
                        dm_bannerslider.isFooterWeb = Convert.ToInt32(Request["is-FooterWEB-checkbox"]);
                        // dm_bannerslider.IsKH = Convert.ToBoolean(Request["is-KH-checkbox"]);
                        var loaisanpham = dm_bannersliderRepository.GetDM_BANNER_SLIDERByBANNER_SLIDER_ID( model.BANNER_SLIDER_ID);

                        if (loaisanpham != null)
                        {
                            TempData[Globals.FailedMessageKey] = "Banner đã tồn tại";
                            return RedirectToAction("DM_BannerSlider");
                        }

                        dm_bannerslider.ANH_DAIDIEN = "";
                        dm_bannersliderRepository.InsertDM_BANNER_SLIDER(dm_bannerslider);
                        dm_bannerslider = dm_bannersliderRepository.GetDM_BANNER_SLIDERByBANNER_SLIDER_ID( dm_bannerslider.BANNER_SLIDER_ID);

                        //begin up hinh anh cho backend
                        var path = System.Web.HttpContext.Current.Server.MapPath("~" + Helpers.Common.GetSetting( "banner-image-folder"));
                        if (Request.Files["file-image"] != null)
                        {
                            var file = Request.Files["file-image"];
                            if (file.ContentLength > 0)
                            {
                                string image_name = "banner_" + Helpers.Common.ChuyenThanhKhongDau(Regex.Replace(dm_bannerslider.BANNER_SLIDER_ID.ToString(), @"\s+", "_")) + file.FileName;
                                bool isExists = System.IO.Directory.Exists(path);
                                if (!isExists)
                                    System.IO.Directory.CreateDirectory(path);
                                file.SaveAs(path + image_name);
                                dm_bannerslider.ANH_DAIDIEN = image_name;
                            }
                        }
                        //end up hinh anh cho backend

                        //begin up hinh anh cho client
                        path = Helpers.Common.GetSetting("banner-image-folder-client");
                        string prjClient = Helpers.Common.GetSetting("prj-client");
                        string filepath = System.Web.HttpContext.Current.Server.MapPath("~");
                        DirectoryInfo di = new DirectoryInfo(filepath);
                        filepath = di.Parent.FullName + "\\" + prjClient + path.Replace("/", @"\");
                        if (Request.Files["file-image"] != null)
                        {
                            var file = Request.Files["file-image"];
                            if (file.ContentLength > 0)
                            {
                                FileInfo fi = new FileInfo(Server.MapPath("~" + path) + dm_bannerslider.ANH_DAIDIEN);
                                if (fi.Exists)
                                {
                                    fi.Delete();
                                }

                                string image_name = "banner_" + Helpers.Common.ChuyenThanhKhongDau(Regex.Replace(dm_bannerslider.BANNER_SLIDER_ID.ToString(), @"\s+", "_")) + file.FileName;

                                bool isExists = System.IO.Directory.Exists(filepath);
                                if (!isExists)
                                    System.IO.Directory.CreateDirectory(filepath);
                                file.SaveAs(filepath + image_name);
                                dm_bannerslider.ANH_DAIDIEN = image_name;
                            }
                        }
                        //end up hinh anh cho client



                        dm_bannersliderRepository.UpdateDM_BANNER_SLIDER(dm_bannerslider);
                          Erp.BackOffice.Controllers.HomeController.WriteLog(dm_bannerslider.BANNER_SLIDER_ID, dm_bannerslider.LINK, "đã cập nhật Banner Slider", "DM_BannerSlider/Edit/" + dm_bannerslider.BANNER_SLIDER_ID, Helpers.Common.CurrentUser.BranchId.Value);
                        scope.Complete();
                        Helpers.Common.ResetCacheYHL("SliderPartial_Mainkissy,camnhankhachhangSpa_ViewPlayout");
                        // Helpers.Common.ResetCacheYHL("SliderPartial_Mainkissy");
                        //ghi log 
                      
                        TempData[Globals.SuccessMessageKey] = App_GlobalResources.Wording.InsertSuccess;
                        return RedirectToAction("DM_BannerSlider");
                    }
                }
                else//**edit**//
                {
                    var dm_bannerslider = dm_bannersliderRepository.GetDM_BANNER_SLIDERByBANNER_SLIDER_ID(model.BANNER_SLIDER_ID);
                    dm_bannerslider.ModifiedUserId = WebSecurity.CurrentUserId;
                    dm_bannerslider.ModifiedDate = DateTime.Now;
                    dm_bannerslider.AssignedUserId = WebSecurity.CurrentUserId;
                    dm_bannerslider.LINK = model.LINK;

                    dm_bannerslider.STT = model.STT;
                    dm_bannerslider.IS_SHOW = Convert.ToInt32(Request["is-show-checkbox"]);
                    dm_bannerslider.IS_MOBILE = Convert.ToInt32(Request["is-mobile-checkbox"]);
                    dm_bannerslider.IsBrand = Convert.ToBoolean(Request["is-brand-checkbox"]);// check thuong hieu
                    dm_bannerslider.IsNgheSi = Convert.ToInt32(Request["is-nghesi-checkbox"]);
                    dm_bannerslider.IsKH = Convert.ToBoolean(Request["is-KH-checkbox"]);
                    dm_bannerslider.isDichVu = Convert.ToInt32(Request["is-DV-checkbox"]);
                    dm_bannerslider.isSanPham = Convert.ToInt32(Request["is-SP-checkbox"]);
                    dm_bannerslider.isLogoMobile = Convert.ToInt32(Request["is-LGMB-checkbox"]);
                    dm_bannerslider.isFooterWeb = Convert.ToInt32(Request["is-FooterWEB-checkbox"]);
                    var oldItem = dm_bannersliderRepository.GetDM_BANNER_SLIDERByBANNER_SLIDER_ID( model.BANNER_SLIDER_ID);

                    //begin up hinh anh cho backend
                    var path = Helpers.Common.GetSetting("banner-image-folder");
                    var filepath = System.Web.HttpContext.Current.Server.MapPath("~" + path);
                    if (Request.Files["file-image"] != null)
                    {
                        var file = Request.Files["file-image"];
                        if (file.ContentLength > 0)
                        {
                            FileInfo fi = new FileInfo(Server.MapPath("~" + path) + dm_bannerslider.ANH_DAIDIEN);
                            if (fi.Exists)
                            {
                                fi.Delete();
                            }

                            string image_name = "banner_" + Helpers.Common.ChuyenThanhKhongDau(Regex.Replace(dm_bannerslider.BANNER_SLIDER_ID.ToString(), @"\s+", "_")) + file.FileName;

                            bool isExists = System.IO.Directory.Exists(filepath);
                            if (!isExists)
                                System.IO.Directory.CreateDirectory(filepath);
                            file.SaveAs(filepath + image_name);
                            dm_bannerslider.ANH_DAIDIEN = image_name;
                        }
                    }

                    //end up hinh anh cho backend



                    //begin up hinh anh cho client
                    path = Helpers.Common.GetSetting( "banner-image-folder-client");
                    string prjClient = Helpers.Common.GetSetting("prj-client");
                    filepath = System.Web.HttpContext.Current.Server.MapPath("~");
                    DirectoryInfo di = new DirectoryInfo(filepath);
                    filepath = di.Parent.FullName + "\\" + prjClient + path.Replace("/", @"\");
                    if (Request.Files["file-image"] != null)
                    {
                        var file = Request.Files["file-image"];
                        if (file.ContentLength > 0)
                        {
                            FileInfo fi = new FileInfo(Server.MapPath("~" + path) + dm_bannerslider.ANH_DAIDIEN);
                            if (fi.Exists)
                            {
                                fi.Delete();
                            }

                            string image_name = "banner_" + Helpers.Common.ChuyenThanhKhongDau(Regex.Replace(dm_bannerslider.BANNER_SLIDER_ID.ToString(), @"\s+", "_")) + file.FileName;

                            bool isExists = System.IO.Directory.Exists(filepath);
                            if (!isExists)
                                System.IO.Directory.CreateDirectory(filepath);
                            file.SaveAs(filepath + image_name);
                            dm_bannerslider.ANH_DAIDIEN = image_name;
                        }
                    }
                    //end up hinh anh cho client









                    dm_bannersliderRepository.UpdateDM_BANNER_SLIDER(dm_bannerslider);
                    Erp.BackOffice.Controllers.HomeController.WriteLog(dm_bannerslider.BANNER_SLIDER_ID, dm_bannerslider.LINK, "đã cập nhật Banner Slider", "DM_BannerSlider/Edit/" + dm_bannerslider.BANNER_SLIDER_ID, Helpers.Common.CurrentUser.BranchId.Value);
                    Helpers.Common.ResetCacheYHL("SliderPartial_Mainkissy,camnhankhachhangSpa_ViewPlayout");
                    //       Helpers.Common.ResetCacheYHL("SliderPartial_Mainkissy");
                    //ghi log 
                   
                    TempData[Globals.SuccessMessageKey] = App_GlobalResources.Wording.UpdateSuccess;
                    return RedirectToAction("DM_BannerSlider");

                }

            }
            return View();
        }

        [HttpPost]
        public ActionResult DeleteBannerSlider()
        {
            //string pMA_DVIQLY = Helpers.Common.CurrentUser.MA_DVIQLY;
            int id = int.Parse(Request["Id"], CultureInfo.InvariantCulture);
            try
            {
                var item = dm_bannersliderRepository.GetDM_BANNER_SLIDERByBANNER_SLIDER_ID(id);
                if (item != null)
                {
                    dm_bannersliderRepository.DeleteDM_BANNER_SLIDER(item.BANNER_SLIDER_ID);
                }
                TempData[Globals.SuccessMessageKey] = App_GlobalResources.Wording.DeleteSuccess;
                return RedirectToAction("DM_BannerSlider");
            }
            catch (DbUpdateException)
            {
                TempData[Globals.FailedMessageKey] = App_GlobalResources.Error.RelationError;
                return RedirectToAction("DM_BannerSlider");
            }
        }
        #endregion
    }


}
