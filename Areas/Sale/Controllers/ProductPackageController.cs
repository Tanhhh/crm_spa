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
using System.Web.Mvc;
using Erp.Utilities;
using WebMatrix.WebData;
using Erp.BackOffice.Helpers;
using System.Web.Script.Serialization;
using System.IO;
using System.Text.RegularExpressions;
using Erp.Domain.Entities;
using Erp.Domain.Sale.Repositories;
using System.Data.SqlClient;
using Erp.Domain.Helper;
using System.Web;

namespace Erp.BackOffice.Sale.Controllers
{
    [Authorize]
    [InitializeSimpleMembership]
    [Erp.BackOffice.Helpers.NoCacheHelper]
    public class ProductPackageController : Controller
    {
        private readonly IProductOrServiceRepository productRepository;
        private readonly IObjectAttributeRepository ObjectAttributeRepository;
        private readonly ISupplierRepository SupplierRepository;
        private readonly IUserRepository userRepository;
        private readonly ICategoryRepository categoryRepository;
        private readonly IProductDetailRepository productDetailRepository;
        private readonly IMaterialRepository materialRepository;
        private readonly ITemplatePrintRepository templatePrintRepository;
        public ProductPackageController(
            IMaterialRepository _Material
            , IProductDetailRepository _ProductDetail
            , IProductOrServiceRepository _Product
            , IObjectAttributeRepository _ObjectAttribute
            , ISupplierRepository _Supplier
            , IUserRepository _user
            , ICategoryRepository category
            , ITemplatePrintRepository _templatePrint
            )
        {
            materialRepository = _Material;
            productDetailRepository = _ProductDetail;
            productRepository = _Product;
            ObjectAttributeRepository = _ObjectAttribute;
            SupplierRepository = _Supplier;
            userRepository = _user;
            categoryRepository = category;
            templatePrintRepository = _templatePrint;
        }

        #region Index

        public ViewResult Index(string txtCode, string CategoryCode, string ProductGroup, SearchObjectAttributeViewModel SearchOjectAttr)
        {
            IEnumerable<ProductViewModel> q = productRepository.GetAllProduct().Where(x=>x.Type=="productpackage").AsEnumerable()
                .Select(item => new ProductViewModel
                {
                    Id = item.Id,
                    CreatedUserId = item.CreatedUserId,
                    CreatedDate = item.CreatedDate,
                    ModifiedUserId = item.ModifiedUserId,
                    ModifiedDate = item.ModifiedDate,
                    Name = item.Name,
                    Code = item.Code,
                    PriceInbound = item.PriceInbound,
                    PriceOutbound = item.PriceOutbound,
                    Barcode = item.Barcode,
                    Type = item.Type,
                    Unit = item.Unit,
                    CategoryCode = item.CategoryCode,
                    DiscountStaff = item.DiscountStaff,
                    IsMoneyDiscount = item.IsMoneyDiscount,
                    ProductGroup=item.ProductGroup,
                    Origin=item.Origin
                }).OrderByDescending(m => m.Id);

            //nếu có tìm kiếm nâng cao thì lọc trước
            if (SearchOjectAttr.ListField != null)
            {
                if (SearchOjectAttr.ListField.Count > 0)
                {
                    //lấy các đối tượng ObjectAttributeValue nào thỏa đk có AttributeId trong ListField và có giá trị tương ứng trong ListField
                    var listObjectAttrValue = ObjectAttributeRepository.GetAllObjectAttributeValue().AsEnumerable().Where(attr => SearchOjectAttr.ListField.Any(item => item.Id == attr.AttributeId && Erp.BackOffice.Helpers.Common.ChuyenThanhKhongDau(attr.Value).Contains(Erp.BackOffice.Helpers.Common.ChuyenThanhKhongDau(item.Value)))).ToList();

                    //tiếp theo tìm các sản phẩm có id bằng với ObjectId trong listObjectAttrValue vừa tìm được
                    q = q.Where(product => listObjectAttrValue.Any(item => item.ObjectId == product.Id));

                    ViewBag.ListOjectAttrSearch = new JavaScriptSerializer().Serialize(SearchOjectAttr.ListField.Select(x => new { Id = x.Id, Value = x.Value }));
                }
            }

            //if (string.IsNullOrEmpty(txtSearch) == false || string.IsNullOrEmpty(txtCode) == false)
            //{
            //    txtSearch = txtSearch == "" ? "~" : Erp.BackOffice.Helpers.Common.ChuyenThanhKhongDau(txtSearch);
            //    //txtCode = txtCode == "" ? "~" : txtCode.ToLower();
            //q = q.Where(x => Erp.BackOffice.Helpers.Common.ChuyenThanhKhongDau(x.Name).Contains(txtSearch)|| x.Code.ToLower().Contains(txtCode));
                
            //}
            if (string.IsNullOrEmpty(txtCode) == false)
            {
              
                txtCode = txtCode == "" ? "~" :Erp.BackOffice.Helpers.Common.ChuyenThanhKhongDau(txtCode).ToLower();
                q = q.Where(x => Helpers.Common.ChuyenThanhKhongDau(x.Name).ToLower().Contains(txtCode) || Helpers.Common.ChuyenThanhKhongDau(x.Code).ToLower().Contains(txtCode));
            }
          
            if (!string.IsNullOrEmpty(CategoryCode))
            {
                q = q.Where(x => x.CategoryCode == CategoryCode);
            }
            if (!string.IsNullOrEmpty(ProductGroup))
            {
                q = q.Where(x => x.ProductGroup == ProductGroup);
            }
            

            ViewBag.SuccessMessage = TempData["SuccessMessage"];
            ViewBag.FailedMessage = TempData["FailedMessage"];
            ViewBag.AlertMessage = TempData["AlertMessage"];
            return View(q);
        }
        #endregion

        #region ExportExcel
        public List<ProductViewModel> IndexExport(string txtCode, string CategoryCode, string ProductGroup, int? BranchId)
        {
            var q = productRepository.GetAllProduct().Where(x => x.Type == "productpackage").AsEnumerable()
                .Select(item => new ProductViewModel
                {
                    Id = item.Id,
                    CreatedUserId = item.CreatedUserId,
                    CreatedDate = item.CreatedDate,
                    ModifiedUserId = item.ModifiedUserId,
                    ModifiedDate = item.ModifiedDate,
                    Name = item.Name,
                    Code = item.Code,
                    PriceInbound = item.PriceInbound,
                    PriceOutbound = item.PriceOutbound,
                    Barcode = item.Barcode,
                    Type = item.Type,
                    Unit = item.Unit,
                    CategoryCode = item.CategoryCode,
                    DiscountStaff = item.DiscountStaff,
                    IsMoneyDiscount = item.IsMoneyDiscount,
                    ProductGroup = item.ProductGroup,
                    Origin = item.Origin
                }).OrderByDescending(m => m.Id).ToList();

            if (string.IsNullOrEmpty(txtCode) == false)
            {

                txtCode = txtCode == "" ? "~" : Erp.BackOffice.Helpers.Common.ChuyenThanhKhongDau(txtCode).ToLower();
                q = q.Where(x => Helpers.Common.ChuyenThanhKhongDau(x.Name).ToLower().Contains(txtCode) || Helpers.Common.ChuyenThanhKhongDau(x.Code).ToLower().Contains(txtCode)).ToList();
            }

            if (!string.IsNullOrEmpty(CategoryCode))
            {
                q = q.Where(x => x.CategoryCode == CategoryCode).ToList();
            }
            if (!string.IsNullOrEmpty(ProductGroup))
            {
                q = q.Where(x => x.ProductGroup == ProductGroup).ToList();
            }

            return q;
        }

        public ActionResult ExportExcel(string txtCode, string CategoryCode, string ProductGroup, int? BranchId, bool ExportExcel = false)
        {
            var data = IndexExport(txtCode, CategoryCode, ProductGroup, BranchId);

            var model = new TemplatePrintViewModel();
            var logo = Erp.BackOffice.Helpers.Common.GetSetting("LogoCompany");
            var company = Erp.BackOffice.Helpers.Common.GetSetting("companyName");
            var address = Erp.BackOffice.Helpers.Common.GetSetting("addresscompany");
            var phone = Erp.BackOffice.Helpers.Common.GetSetting("phonecompany");
            var fax = Erp.BackOffice.Helpers.Common.GetSetting("faxcompany");
            var bankcode = Erp.BackOffice.Helpers.Common.GetSetting("bankcode");
            var bankname = Erp.BackOffice.Helpers.Common.GetSetting("bankname");
            var ImgLogo = "<div class=\"logo\"><img src=" + logo + " height=\"73\" /></div>";
            var template = templatePrintRepository.GetAllTemplatePrint().Where(x => x.Code.Contains("PrintReport")).OrderByDescending(x => x.CreatedDate).FirstOrDefault();
            model.Content = template.Content;
            model.Content = model.Content.Replace("{DataTable}", buildHtml(data));
            model.Content = model.Content.Replace("{System.Logo}", ImgLogo);
            model.Content = model.Content.Replace("{System.CompanyName}", company);
            model.Content = model.Content.Replace("{System.AddressCompany}", address);
            model.Content = model.Content.Replace("{System.PhoneCompany}", phone);
            model.Content = model.Content.Replace("{System.Fax}", fax);
            model.Content = model.Content.Replace("{System.BankCodeCompany}", bankcode);
            model.Content = model.Content.Replace("{System.BankNameCompany}", bankname);
            model.Content = model.Content.Replace("{PrintDate}", DateTime.Now.ToString("dd/MM/yyyy HH:mm"));
            model.Content = model.Content.Replace("{Title}", "Danh sách túi liệu trình");
            if (ExportExcel)
            {
                Response.AppendHeader("content-disposition", "attachment;filename=" + "DS_Tuilieutrinh" + DateTime.Now.ToString("yyyyMMdd") + ".xls");
                Response.Charset = "";
                Response.Cache.SetCacheability(HttpCacheability.NoCache);
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.Write(model.Content);
                Response.End();
            }
            return View(model);
        }

        string buildHtml(List<ProductViewModel> data)
        {
            decimal? tong_tien = 0;
            //Tạo table html chi tiết phiếu xuất
            string detailLists = "<table border=\"1\" class=\"invoice-detail\">\r\n";
            detailLists += "<thead>\r\n";
            detailLists += "	<tr>\r\n";
            detailLists += "		<th>STT</th>\r\n";
            detailLists += "		<th>Mã Túi Liệu Trình</th>\r\n";
            detailLists += "		<th>Tên Túi Liệu Trình</th>\r\n";
            detailLists += "		<th>Giá Xuất</th>\r\n";
            detailLists += "		<th>Danh Mục</th>\r\n";
            detailLists += "		<th>Nhãn Hàng</th>\r\n";
            detailLists += "		<th>Nhóm</th>\r\n";
            detailLists += "		<th>Ngày Tạo</th>\r\n";
            detailLists += "		<th>Ngày Cập Nhật</th>\r\n";
            detailLists += "	</tr>\r\n";
            detailLists += "</thead>\r\n";
            detailLists += "<tbody>\r\n";
            var index = 1;

            foreach (var item in data)
            {
                tong_tien += (item.PriceOutbound == null ? 0 : item.PriceOutbound);

                detailLists += "<tr>\r\n"
                + "<td class=\"text-center orderNo\">" + (index++) + "</td>\r\n"
                + "<td class=\"text-left \">" + item.Code + "</td>\r\n"
                + "<td class=\"text-left \">" + item.Name + "</td>\r\n"
                + "<td class=\"text-left\">" + Erp.BackOffice.Helpers.CommonSatic.ToCurrencyStr(item.PriceOutbound, null) + "</td>\r\n"
                + "<td class=\"text-left \">" + item.CategoryCode + "</td>\r\n"
                + "<td class=\"text-left \">" + item.Origin + "</td>\r\n"
                + "<td class=\"text-left \">" + item.ProductGroup + "</td>\r\n"
                + "<td>" + (item.CreatedDate.HasValue ? item.CreatedDate.Value.ToString("dd/MM/yyyy HH:mm") : "") + "</td>"
                + "<td>" + (item.ModifiedDate.HasValue ? item.ModifiedDate.Value.ToString("dd/MM/yyyy HH:mm") : "") + "</td>"
                + "</tr>\r\n";
            }
            detailLists += "</tbody>\r\n";
            detailLists += "<tfoot style=\"font-weight:bold\">\r\n";
            detailLists += "<tr \"  style=\"font-weight:bold\"><td colspan=\"3\" class=\"text-right\">Tổng cộng</td><td class=\"text-right\">"
                        + Erp.BackOffice.Helpers.CommonSatic.ToCurrencyStr(tong_tien, null)
                        + "</td></tr>\r\n";

            detailLists += "</tfoot>\r\n</table>\r\n";


            return detailLists;
        }
        #endregion

        #region Create
        public ViewResult Create()
        {
            var model = new ProductViewModel();
            model.PriceInbound = 0;
            model.PriceOutbound = 0;
            model.MinInventory = 0;
            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        //[ValidateAntiForgeryToken]
        public ActionResult Create(ProductViewModel model)
        {
            if (ModelState.IsValid)
            {
                var Product = new Domain.Sale.Entities.Product();
                var ProductDetail = new Domain.Sale.Entities.ProductDetail();
                AutoMapper.Mapper.Map(model, Product);
                Product.IsDeleted = false;
                Product.CreatedUserId = WebSecurity.CurrentUserId;
                Product.ModifiedUserId = WebSecurity.CurrentUserId;
                Product.CreatedDate = DateTime.Now;
                Product.ModifiedDate = DateTime.Now;

                if (model.PriceInbound == null)
                    Product.PriceInbound = 0;
                Product.Code = Product.Code.Trim();
                var path = System.Web.HttpContext.Current.Server.MapPath("~" + Helpers.Common.GetSetting("product-image-folder"));
                if (Request.Files["file-image"] != null)
                {
                    var file = Request.Files["file-image"];
                    if (file.ContentLength > 0)
                    {
                        string image_name = "product_" + Helpers.Common.ChuyenThanhKhongDau(Regex.Replace(Product.Code, @"\s+", "_")) + "." + file.FileName.Split('.').Last();
                        bool isExists = System.IO.Directory.Exists(path);
                        if (!isExists)
                            System.IO.Directory.CreateDirectory(path);
                        file.SaveAs(path + image_name);
                        Product.Image_Name = image_name;
                    }
                }

                productRepository.InsertProduct(Product);
                //ghi log 
                Erp.BackOffice.Controllers.HomeController.WriteLog(Product.Id, Product.Code, "đã tạo túi liệu trình", "ProductPackage/Edit/" + Product.Id, Helpers.Common.CurrentUser.BranchId.Value);

                //////////////
                var _listdata = productDetailRepository.GetAllProductDetail().Where(x => x.ProductId == Product.Id).ToList();
                if (model.ProductDetailList.Any(x => x.Id == 0))
                {
                    //lưu danh sách thao tác thực hiện dịch vụ
                    foreach (var item in model.ProductDetailList.Where(x => x.Id == 0&&!string.IsNullOrEmpty(x.MaterialName)))
                    {
                        var add = new Domain.Sale.Entities.ProductDetail();
                        add.IsDeleted = false;
                        add.CreatedUserId = WebSecurity.CurrentUserId;
                        add.ModifiedUserId = WebSecurity.CurrentUserId;
                        add.CreatedDate = DateTime.Now;
                        add.ModifiedDate = DateTime.Now;
                        add.MaterialId = item.MaterialId.Value;
                        add.Quantity = item.Quantity.Value;
                        add.ProductId = Product.Id;
                        productDetailRepository.InsertProductDetail(add);
                    }
                }
                var _delete = _listdata.Where(id1 => !model.ProductDetailList.ToList().Any(id2 => id2.Id == id1.Id)).ToList();
                if (_delete.Any())
                {
                    foreach (var item in _delete)
                    {
                        productDetailRepository.DeleteProductDetail(item.Id);
                    }
                }
                if (model.ProductDetailList.Any(x => x.Id > 0))
                {
                    var update = _listdata.Where(id1 => model.ProductDetailList.ToList().Any(id2 => id2.Id == id1.Id)).ToList();
                    //lưu danh sách thao tác thực hiện dịch vụ
                    foreach (var item in model.ProductDetailList.Where(x => x.Id > 0 && !string.IsNullOrEmpty(x.MaterialName)))
                    {
                        var _update = update.FirstOrDefault(x => x.Id == item.Id);
                        _update.MaterialId = item.MaterialId.Value;
                        _update.Quantity = item.Quantity == null ? 0 : item.Quantity.Value;
                        _update.ProductId = Product.Id;
                        productDetailRepository.UpdateProductDetail(_update);
                    }
                }
                ////////////
                if (string.IsNullOrEmpty(Request["IsPopup"]) == false)
                {
                    return RedirectToAction("Edit", new { Id = model.Id, IsPopup = Request["IsPopup"] });
                }

                TempData[Globals.SuccessMessageKey] = App_GlobalResources.Wording.InsertSuccess;

                return RedirectToAction("Index");
            }

            string errors = string.Empty;
            foreach (var modalState in ModelState.Values)
            {
                errors += modalState.Value + ": ";
                foreach (var error in modalState.Errors)
                {
                    errors += error.ErrorMessage;
                }
            }

            ViewBag.errors = errors;

            return View(model);
        }

        public ActionResult CheckCodeExsist(int? id, string code)
        {
            code = code.Trim();
            var product = productRepository.GetAllProduct()
                .Where(item => item.Code == code).FirstOrDefault();
            if (product != null)
            {
                if (id == null || (id != null && product.Id != id))
                    return Content("Trùng mã sản phẩm!");
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
        #endregion

        #region Edit
        public ActionResult Edit(int Id)
        {
            var Product = productRepository.GetvwProductAndServiceById(Id);
            if (Product != null && Product.IsDeleted != true)
            {
                var model = new ProductViewModel();
                AutoMapper.Mapper.Map(Product, model);

                string productId = "," + model.Id + ",";
                var supplierList = SupplierRepository.GetAllSupplier().AsEnumerable().Where(item => ("," + item.ProductIdOfSupplier + ",").Contains(productId) == true).ToList();
                ViewBag.supplierList = supplierList;
                //
                model.ProductDetailList = new List<ProductDetailViewModel>();
                model.ProductDetailList = productDetailRepository.GetvwAllProductDetail().Where(x => x.ProductId == Product.Id)
               .Select(x => new ProductDetailViewModel
               {
                   MaterialName = x.MaterialName,
                   Id = x.Id,
                   ProductId = x.ProductId,
                   MaterialCode=x.MaterialCode,
                   MaterialId=x.MaterialId,
                   Quantity=x.Quantity
               }).ToList();
                return View(model);
            }
            if (Request.UrlReferrer != null)
                return Redirect(Request.UrlReferrer.AbsoluteUri);

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(ProductViewModel model)
        {
            if (ModelState.IsValid)
            {
                var Product = productRepository.GetProductById(model.Id);
                AutoMapper.Mapper.Map(model, Product);
                Product.ModifiedUserId = WebSecurity.CurrentUserId;
                Product.ModifiedDate = DateTime.Now;
                if (model.PriceInbound == null)
                    Product.PriceInbound = 0;
                var path = Helpers.Common.GetSetting("product-image-folder");
                var filepath = System.Web.HttpContext.Current.Server.MapPath("~" + path);
                if (Request.Files["file-image"] != null)
                {
                    var file = Request.Files["file-image"];
                    if (file.ContentLength > 0)
                    {
                        FileInfo fi = new FileInfo(Server.MapPath("~" + path) + Product.Image_Name);
                        if (fi.Exists)
                        {
                            fi.Delete();
                        }

                        string image_name = "product_" + Helpers.Common.ChuyenThanhKhongDau(Regex.Replace(Product.Code, @"\s+", "_")) + "." + file.FileName.Split('.').Last();

                        bool isExists = System.IO.Directory.Exists(filepath);
                        if (!isExists)
                            System.IO.Directory.CreateDirectory(filepath);
                        file.SaveAs(filepath + image_name);
                        Product.Image_Name = image_name;
                    }
                }
                productRepository.UpdateProduct(Product);
                //ghi log 
                Erp.BackOffice.Controllers.HomeController.WriteLog(Product.Id, Product.Code, "đã cập nhật túi liệu trình", "ProductPackage/Edit/" + Product.Id, Helpers.Common.CurrentUser.BranchId.Value);

                var _listdata = productDetailRepository.GetAllProductDetail().Where(x => x.ProductId == Product.Id).ToList();
                if (model.ProductDetailList.Any(x => x.Id == 0))
                {
                    //lưu danh sách thao tác thực hiện dịch vụ
                    foreach (var item in model.ProductDetailList.Where(x => x.Id == 0 && !string.IsNullOrEmpty(x.MaterialName)))
                    {
                        var add = new Domain.Sale.Entities.ProductDetail();
                        add.IsDeleted = false;
                        add.CreatedUserId = WebSecurity.CurrentUserId;
                        add.ModifiedUserId = WebSecurity.CurrentUserId;
                        add.CreatedDate = DateTime.Now;
                        add.ModifiedDate = DateTime.Now;
                        add.MaterialId = item.MaterialId.Value;
                        add.Quantity = item.Quantity.Value;
                        add.ProductId = Product.Id;
                        productDetailRepository.InsertProductDetail(add);
                    }
                }
                var _delete = _listdata.Where(id1 => !model.ProductDetailList.ToList().Any(id2 => id2.Id == id1.Id)).ToList();
                if (_delete.Any())
                {
                    foreach (var item in _delete)
                    {
                        productDetailRepository.DeleteProductDetail(item.Id);
                    }
                }
                if (model.ProductDetailList.Any(x => x.Id > 0))
                {
                    var update = _listdata.Where(id1 => model.ProductDetailList.ToList().Any(id2 => id2.Id == id1.Id)).ToList();
                    //lưu danh sách thao tác thực hiện dịch vụ
                    foreach (var item in model.ProductDetailList.Where(x => x.Id > 0 && !string.IsNullOrEmpty(x.MaterialName)))
                    {
                        var _update = update.FirstOrDefault(x => x.Id == item.Id);
                        _update.MaterialId = item.MaterialId.Value;
                        _update.Quantity = item.Quantity == null ? 0 : item.Quantity.Value;
                        _update.ProductId = Product.Id;
                        productDetailRepository.UpdateProductDetail(_update);
                    }
                }
                /////////
                if (string.IsNullOrEmpty(Request["IsPopup"]) == false)
                {
                    return RedirectToAction("Edit", new { Id = model.Id, IsPopup = Request["IsPopup"] });
                }

                TempData[Globals.SuccessMessageKey] = App_GlobalResources.Wording.UpdateSuccess;
                return RedirectToAction("Index");
            }

            string errors = string.Empty;
            foreach (var modalState in ModelState.Values)
            {
                errors += modalState.Value + ": ";
                foreach (var error in modalState.Errors)
                {
                    errors += error.ErrorMessage;
                }
            }

            ViewBag.errors = errors;

            return View(model);

            //if (Request.UrlReferrer != null)
            //    return Redirect(Request.UrlReferrer.AbsoluteUri);
            //return RedirectToAction("Index");
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
                    var item = productRepository.GetProductById(int.Parse(id, CultureInfo.InvariantCulture));
                    if (item != null)
                    {
                        

                        item.IsDeleted = true;
                        productRepository.UpdateProduct(item);
                    }
                }
                else
                {
                    string idDeleteAll = Request["DeleteId-checkbox"];
                    string[] arrDeleteId = idDeleteAll.Split(',');
                    for (int i = 0; i < arrDeleteId.Count(); i++)
                    {
                        var item = productRepository.GetProductById(int.Parse(arrDeleteId[i], CultureInfo.InvariantCulture));
                        if (item != null)
                        {
                            //if (item.CreatedUserId != Erp.BackOffice.Helpers.Common.CurrentUser.Id && Erp.BackOffice.Helpers.Common.CurrentUser.UserTypeId != 1)
                            //{
                            //    TempData["FailedMessage"] = "NotOwner";
                            //    return RedirectToAction("Index");
                            //}

                            item.IsDeleted = true;
                            productRepository.UpdateProduct(item);
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

        #region  - Json -
        public JsonResult GetListJson()
        {
            var list = productRepository.GetAllProduct().ToList();
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}
