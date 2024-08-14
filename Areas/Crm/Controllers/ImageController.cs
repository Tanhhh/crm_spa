using Erp.BackOffice.Crm.Models;
using Erp.BackOffice.Filters;
using Erp.Domain.Crm.Entities;
using Erp.Domain.Crm.Interfaces;
using Erp.Domain.Interfaces;
using Erp.Domain.Sale.Interfaces;
using Erp.Utilities;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using WebMatrix.WebData;
//hoa commet tesst
namespace Erp.BackOffice.Crm.Controllers
{
    [Authorize]
    [InitializeSimpleMembership]
    [Erp.BackOffice.Helpers.NoCacheHelper]
    public class ImageController : Controller
    {
        private readonly IImageRepository imageRepository;
        private readonly IUserRepository userRepository;
        private readonly IProductOrServiceRepository productRepository;

        public ImageController(
            IImageRepository _Image
            , IUserRepository _user
             , IProductOrServiceRepository _Product
            )
        {
            imageRepository = _Image;
            userRepository = _user;
            productRepository = _Product;
        }
        #region Index
        public ViewResult Index()
        {
            IQueryable<ImageViewModel> q = imageRepository.GetAllImage()
                .Select(item => new ImageViewModel
                {
                    Id = item.Id,
                    CreatedUserId = item.CreatedUserId,
                    CreatedDate = item.CreatedDate,
                    ModifiedUserId = item.ModifiedUserId,
                    ModifiedDate = item.ModifiedDate,
                    TargetId = item.TargetId,
                    Name = item.Name,
                    Images = item.Images,
                    TargetModule = item.TargetModule
                }).OrderByDescending(m => m.ModifiedDate);
            ViewBag.SuccessMessage = TempData["SuccessMessage"];
            ViewBag.FailedMessage = TempData["FailedMessage"];
            ViewBag.AlertMessage = TempData["AlertMessage"];
            return View(q);
        }
        #endregion

        #region Edit
        public ActionResult Edit(int? Id)
        {
            var _image = new Image();
            _image = imageRepository.GetImageById(Id.Value);
            if (_image != null && _image.IsDeleted != true)
            {
                var model = new ImageViewModel();
                AutoMapper.Mapper.CreateMap<Image, ImageViewModel>();
                AutoMapper.Mapper.Map(_image, model);

                //if (model.CreatedUserId != Helpers.Common.CurrentUser.Id && Helpers.Common.CurrentUser.UserTypeId != 1)
                //{
                //    TempData["FailedMessage"] = "NotOwner";
                //    return RedirectToAction("Index");
                //}

                return View(model);
            }
            if (Request.UrlReferrer != null)
                return Redirect(Request.UrlReferrer.AbsoluteUri);
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(ImageViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (Request["Submit"] == "Save")
                {
                    var _image = imageRepository.GetImageById(model.Id);
                    _image.ModifiedUserId = WebSecurity.CurrentUserId;
                    _image.ModifiedDate = DateTime.Now;
                    _image.Name = model.Name;
                    var path = Helpers.Common.GetSetting("upload_path_Image") + _image.TargetModule + "/";
                    var filepath = System.Web.HttpContext.Current.Server.MapPath("~" + path);
                    if (Request.Files["file-image"] != null)
                    {
                        var file = Request.Files["file-image"];
                        if (file.ContentLength > 0)
                        {
                            FileInfo fi = new FileInfo(Server.MapPath("~" + path) + _image.Images);
                            if (fi.Exists)
                            {
                                fi.Delete();
                            }
                            var FileName = model.Name.Replace(" ", "_");
                            var name = Erp.BackOffice.Helpers.Common.ChuyenThanhKhongDau(FileName).ToLower();
                            string image_name = name + "(" + _image.Id + ")" + "." + file.FileName.Split('.').Last();

                            bool isExists = System.IO.Directory.Exists(filepath);
                            if (!isExists)
                                System.IO.Directory.CreateDirectory(filepath);
                            file.SaveAs(filepath + image_name);
                            _image.Images = image_name;
                        }
                    }

                    imageRepository.UpdateImage(_image);

                    return RedirectToAction("_ClosePopup", "Home", new { area = "", FunctionCallback = "ClosePopupAndReloadPage" });
                }

                return View(model);
            }

            return View(model);

            //if (Request.UrlReferrer != null)
            //    return Redirect(Request.UrlReferrer.AbsoluteUri);
            //return RedirectToAction("Index");
        }

        #endregion

        #region Detail
        public ActionResult Detail(int? TargetId, string TargetModule)
        {
            List<ImageViewModel> q = imageRepository.GetAllImage().Where(x => x.TargetId == TargetId && x.TargetModule == TargetModule)
             .Select(item => new ImageViewModel
             {
                 Id = item.Id,
                 CreatedUserId = item.CreatedUserId,
                 //CreatedUserName = item.CreatedUserName,
                 CreatedDate = item.CreatedDate,
                 ModifiedUserId = item.ModifiedUserId,
                 //ModifiedUserName = item.ModifiedUserName,
                 ModifiedDate = item.ModifiedDate,
                 Name = item.Name,
                 Description = item.Description,
                 Images = item.Images,
                 TargetId = item.TargetId,
                 TargetModule = item.TargetModule
             }).OrderByDescending(m => m.CreatedDate).ToList();
            return View(q);
        }
        #endregion

        #region Delete
        [HttpPost]
        public ActionResult Delete(int Id)
        {
            try
            {
                var item = imageRepository.GetImageById(Id);
                if (item != null)
                {
                    if (item != null)
                    {
                        using (var scope = new TransactionScope(TransactionScopeOption.Required, new System.TimeSpan(0, 30, 0)))
                        {
                            try
                            {
                                imageRepository.DeleteImageRs(Id);

                                IQueryable<ImageViewModel> q = imageRepository.GetAllImage().Where(x => x.TargetId == item.TargetId)
                                   .Select(item1 => new ImageViewModel
                                   {
                                       Name = item1.Images,
                                   });

                                string nameimgs = "";
                                string Hinhdaidien = "";
                                int iHinhdaidien = 0;
                                foreach (var item2 in q)
                                {
                                    nameimgs += item2.Name + ",";
                                    if (iHinhdaidien == 0)
                                    {
                                        Hinhdaidien = item2.Name;
                                    }
                                    iHinhdaidien++;

                                }
                                if (nameimgs.Length > 1)
                                {
                                    nameimgs = nameimgs.Substring(0, nameimgs.Length - 1);
                                }
                                var Product = productRepository.GetProductById(item.TargetId.Value);
                                if (Product != null)
                                {
                                    Product.Image_Name = Hinhdaidien;
                                    Product.List_Image = nameimgs;
                                    productRepository.UpdateProduct(Product);
                                }

                                scope.Complete();
                            }

                            catch (DbUpdateException)
                            {
                                return Content("error");
                            }
                        }
                        return Content("success");
                    }
                    return Content("success");
                }
                return Content("error");
            }
            catch (DbUpdateException)
            {
                TempData[Globals.FailedMessageKey] = App_GlobalResources.Error.RelationError;
                return Content("error");
            }
        }
        #endregion

        #region Create
        public ActionResult Create(int TargetId, string TargetModule)
        {
            var model = new ImageViewModel();
            if (TargetId > 0)
            {
                model.TargetId = TargetId;
                model.TargetModule = TargetModule;
            }
            Session["file"] = null;
            return View(model);
        }

        [HttpPost]
        public ActionResult Create(ImageViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.TargetModule == "Product")
                {
                    SaveUpload2(model.Name, model.TargetId.Value, model.TargetModule);
                }
                else
                {
                    ImageController.SaveUpload(model.Name, model.TargetId.Value, model.TargetModule);
                }

                return RedirectToAction("_ClosePopup", "Home", new { area = "", FunctionCallback = "ClosePopupAndReloadPage" });
            }

            return View();
        }

        #region SaveUpload
        public static string SaveUpload(string Name, int TargetId, string TargetModule)
        {
            //insert và upload documentField, documentAttribute.
            List<HttpPostedFileBase> listFile = new List<HttpPostedFileBase>();
            if (System.Web.HttpContext.Current.Session["file"] != null)
                listFile = (List<HttpPostedFileBase>)System.Web.HttpContext.Current.Session["file"];
            Erp.Domain.Crm.Repositories.ImageRepository ImageRepository = new Erp.Domain.Crm.Repositories.ImageRepository(new Domain.Crm.ErpCrmDbContext());
            if (listFile.Count > 0)
            {
                foreach (var item in listFile)
                {
                    var type = item.FileName.Split('.').Last();
                    var FileName = Name.Replace(" ", "_");
                    var name = Erp.BackOffice.Helpers.Common.ChuyenThanhKhongDau(FileName).ToLower();
                    var _image = new Image();
                    _image.IsDeleted = false;
                    _image.CreatedDate = DateTime.Now;
                    _image.ModifiedDate = DateTime.Now;
                    _image.Name = Name;
                    _image.TargetId = TargetId;
                    _image.TargetModule = TargetModule;
                    //REProductImage.Description = "";
                    ImageRepository.InsertImage(_image);
                    _image.Images = name + "(" + _image.Id + ")" + "." + type;
                    ImageRepository.UpdateImage(_image);
                    var path = System.Web.HttpContext.Current.Server.MapPath("~" + Erp.BackOffice.Helpers.Common.GetSetting("upload_path_Image") + _image.TargetModule + "/");
                    bool isExists = System.IO.Directory.Exists(path);
                    if (!isExists)
                        System.IO.Directory.CreateDirectory(path);
                    item.SaveAs(path + _image.Images);
                }

                return "Vui lòng chọn tập tin!";
            }

            return "Vui lòng chọn tập tin!";
        }

        public string SaveUpload2(string Name, int TargetId, string TargetModule)
        {

            //insert và upload documentField, documentAttribute.
            List<HttpPostedFileBase> listFile = new List<HttpPostedFileBase>();
            if (System.Web.HttpContext.Current.Session["file"] != null)
                listFile = (List<HttpPostedFileBase>)System.Web.HttpContext.Current.Session["file"];

            //begin code lay tu product

            //begin up hinh anh cho backend
            var path = Helpers.Common.GetSetting("product-image-folder");
            var path2 = System.Web.HttpContext.Current.Server.MapPath("~" + Helpers.Common.GetSetting("product-image-folder"));
            var filepath = System.Web.HttpContext.Current.Server.MapPath("~" + path);
            Erp.Domain.Crm.Repositories.ImageRepository ImageRepository = new Erp.Domain.Crm.Repositories.ImageRepository(new Domain.Crm.ErpCrmDbContext());
            string nameimgs = "";
            string nameimg_main = "";
            //begin lay anh cu
            IQueryable<ImageViewModel> q = imageRepository.GetAllImage().Where(x => x.TargetId == TargetId)
                             .Select(item1 => new ImageViewModel
                             {
                                 Name = item1.Images,
                             });

            foreach (var item2 in q)
            {
                nameimgs += item2.Name + ",";
            }

            nameimg_main = "";
            int Hinhdaidien = 0;
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.Snapshot }))
            {
                try
                {
                    if (listFile.Count > 0)
                    {
                        foreach (var item in listFile)
                        {
                            var type = item.FileName.Split('.').Last();
                            var FileName = item.FileName.Replace(" ", "_");
                            var name = Erp.BackOffice.Helpers.Common.ChuyenThanhKhongDau(FileName).ToLower();


                            string image_name = "product_" + Helpers.Common.ChuyenThanhKhongDau(Regex.Replace(TargetId.ToString(), @"\s+", "_")) + "_" + name;
                            bool isExists = System.IO.Directory.Exists(path2);
                            if (!isExists)
                                System.IO.Directory.CreateDirectory(path2);
                            nameimgs += image_name + ",";
                            item.SaveAs(path2 + image_name);

                            //begin hoapd insert vao table chi tiet
                            var _image = new Image();
                            _image.IsDeleted = false;
                            _image.CreatedDate = DateTime.Now;
                            _image.ModifiedDate = DateTime.Now;
                            _image.Name = Name;
                            _image.TargetId = TargetId;
                            _image.TargetModule = TargetModule;

                            //REProductImage.Description = "";
                            ImageRepository.InsertImage(_image);
                            _image.Images = image_name;
                            ImageRepository.UpdateImage(_image);
                            //end hoapd insert vao table chi tiet
                            if (Hinhdaidien == 0)
                            {
                                nameimg_main = image_name;
                            }
                            Hinhdaidien++;

                        }


                        //begin up hinh anh cho client
                        path = Helpers.Common.GetSetting("product-image-folder-client");
                        string prjClient = Helpers.Common.GetSetting("prj-client");
                        //string prjClient = "KISSY_CLIENT";
                        filepath = System.Web.HttpContext.Current.Server.MapPath("~");
                        DirectoryInfo di = new DirectoryInfo(filepath);
                        if (prjClient.Trim() == "")
                        {
                            filepath = di.Parent.FullName + path.Replace("/", @"\");
                        }
                        else
                        {
                            filepath = di.Parent.FullName + "\\" + prjClient + path.Replace("/", @"\");
                        }



                        foreach (var item in listFile)
                        {
                            var type = item.FileName.Split('.').Last();
                            var FileName = item.FileName.Replace(" ", "_");
                            var name = Erp.BackOffice.Helpers.Common.ChuyenThanhKhongDau(FileName).ToLower();


                            string image_name = "product_" + Helpers.Common.ChuyenThanhKhongDau(Regex.Replace(TargetId.ToString(), @"\s+", "_")) + "_" + name;
                            bool isExists = System.IO.Directory.Exists(filepath);
                            if (!isExists)
                                System.IO.Directory.CreateDirectory(filepath);
                            item.SaveAs(filepath + image_name);


                        }

                        //end up hinh anh cho client

                        //end code lay tu product
                        if (listFile.Count > 0)
                        {
                            if (nameimgs.Length > 1)
                            {
                                nameimgs = nameimgs.Substring(0, nameimgs.Length - 1);
                            }
                            var Product = productRepository.GetProductById(TargetId);
                            if (nameimg_main != "")
                            {
                                Product.Image_Name = nameimg_main;
                                //Product.MA_DVIQLY = pMA_DVIQLY;
                            }
                            Product.List_Image = nameimgs;
                            productRepository.UpdateProduct(Product);
                        }
                    }
                    scope.Complete();
                }
                catch (DbUpdateException)
                {
                    TempData[Globals.FailedMessageKey] = App_GlobalResources.Wording.Error;
                }
                return "Vui lòng chọn tập tin!";
            }

            return "Vui lòng chọn tập tin!";
        }
        #endregion

        #region Upload
        public ActionResult Upload()
        {
            bool isSavedSuccessfully = true;
            string fName = "";
            foreach (string fileName in Request.Files)
            {
                HttpPostedFileBase file = Request.Files[fileName];
                //Save file content goes here
                fName = file.FileName;
                if (file != null && file.ContentLength > 0)
                {
                    List<HttpPostedFileBase> listFile = new List<HttpPostedFileBase>();
                    if (Session["file"] != null)
                    {
                        listFile = (List<HttpPostedFileBase>)Session["file"];
                    }

                    listFile.Add(file);

                    Session["file"] = listFile;

                }

            }

            if (isSavedSuccessfully)
            {
                return Json(new { Message = fName });
            }
            else
            {
                return Json(new { Message = "Error in saving file" });
            }
        }
        #endregion
        #endregion
    }
}
