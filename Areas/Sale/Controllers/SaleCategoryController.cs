using System.Globalization;
using Erp.BackOffice.Areas.Cms.Models;
using Erp.BackOffice.Filters;
using Erp.Domain.Entities;
using Erp.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Erp.Utilities;
using WebMatrix.WebData;
using Erp.BackOffice.Helpers;
using Erp.Domain.Repositories;
using System.Web;
using Erp.Domain.Sale.Interfaces;
using Erp.BackOffice.Sale.Models;

namespace Erp.BackOffice.Sale.Controllers
{
    [Authorize]
    [InitializeSimpleMembership]
    [Erp.BackOffice.Helpers.NoCacheHelper]
    public class SaleCategoryController : Controller
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IUserRepository _userRepository;
        private readonly ITemplatePrintRepository templatePrintRepository;
        public SaleCategoryController(ICategoryRepository category, IUserRepository user, ITemplatePrintRepository _templatePrint)
        {
            _categoryRepository = category;
            _userRepository = user;
            templatePrintRepository = _templatePrint;

        }
       

        #region List Category
        IEnumerable<CategoryViewModel> getCategories(string code)
        {
            IEnumerable<CategoryViewModel> listCategory = new List<CategoryViewModel>();
            var model = _categoryRepository.GetAllCategories()
                .Where(item => item.Code == code)
                .OrderBy(m => m.OrderNo).ToList();

            listCategory = AutoMapper.Mapper.Map(model, listCategory);

            return listCategory;
        }

        public ViewResult ProductCategory()
        {
            string code = "product";
            ViewBag.SuccessMessage = TempData[Globals.SuccessMessageKey];
            ViewBag.FailedMessage = TempData[Globals.FailedMessageKey];
            ViewBag.CategoryCode = code;
            ViewBag.ActionName = "ProductCategory";

            return View("Index", getCategories(code));
        }


        public ViewResult CustomerGroup()
        {
            string code = "CustomerGroup";
            ViewBag.SuccessMessage = TempData[Globals.SuccessMessageKey];
            ViewBag.FailedMessage = TempData[Globals.FailedMessageKey];
            ViewBag.CategoryCode = code;
            ViewBag.ActionName = "CustomerGroup";

            return View("Index", getCategories(code));
        }

        public ViewResult EconomicStatus()
        {
            string code = "EconomicStatus";
            ViewBag.SuccessMessage = TempData[Globals.SuccessMessageKey];
            ViewBag.FailedMessage = TempData[Globals.FailedMessageKey];
            ViewBag.CategoryCode = code;
            ViewBag.ActionName = "EconomicStatus";

            return View("Index", getCategories(code));
        }

        public ViewResult CustomerType()
        {
            string code = "CustomerType";
            ViewBag.SuccessMessage = TempData[Globals.SuccessMessageKey];
            ViewBag.FailedMessage = TempData[Globals.FailedMessageKey];
            ViewBag.CategoryCode = code;
            ViewBag.ActionName = "CustomerType";

            return View("Index", getCategories(code));
        }
        public ViewResult ProductUnit()
        {
            string code = "productUnit";
            ViewBag.SuccessMessage = TempData[Globals.SuccessMessageKey];
            ViewBag.FailedMessage = TempData[Globals.FailedMessageKey];
            ViewBag.CategoryCode = code;
            ViewBag.ActionName = "ProductUnit";

            return View("Index", getCategories(code));
        }

        public ViewResult ProductManufacturer()
        {
            string code = "manufacturerList";
            ViewBag.SuccessMessage = TempData[Globals.SuccessMessageKey];
            ViewBag.FailedMessage = TempData[Globals.FailedMessageKey];
            ViewBag.CategoryCode = code;
            ViewBag.ActionName = "ProductManufacturer";

            return View("Index", getCategories(code));
        }

        public ViewResult ProductGroup()
        {
            string code = "ProductGroup";
            ViewBag.SuccessMessage = TempData[Globals.SuccessMessageKey];
            ViewBag.FailedMessage = TempData[Globals.FailedMessageKey];
            ViewBag.CategoryCode = code;
            ViewBag.ActionName = "ProductGroup";

            return View("Index", getCategories(code));
        }

        public ViewResult TitleSalarySetting()
        {
            string code = "TitleSalarySetting";
            ViewBag.SuccessMessage = TempData[Globals.SuccessMessageKey];
            ViewBag.FailedMessage = TempData[Globals.FailedMessageKey];
            ViewBag.CategoryCode = code;
            ViewBag.ActionName = "TitleSalarySetting";

            return View("Index", getCategories(code));
        }

        public ViewResult FloorList()
        {
            string code = "FloorList";
            ViewBag.SuccessMessage = TempData[Globals.SuccessMessageKey];
            ViewBag.FailedMessage = TempData[Globals.FailedMessageKey];
            ViewBag.CategoryCode = code;
            ViewBag.ActionName = "FloorList";

            return View("Index", getCategories(code));
        }

        public ViewResult ServiceUnit()
        {
            string code = "serviceUnit";
            ViewBag.SuccessMessage = TempData[Globals.SuccessMessageKey];
            ViewBag.FailedMessage = TempData[Globals.FailedMessageKey];
            ViewBag.CategoryCode = code;
            ViewBag.ActionName = "ServiceUnit";

            return View("Index", getCategories(code));
        }
        public ViewResult TypeWarehouse()
        {
            string code = "Categories_product";
            ViewBag.SuccessMessage = TempData[Globals.SuccessMessageKey];
            ViewBag.FailedMessage = TempData[Globals.FailedMessageKey];
            ViewBag.CategoryCode = code;
            ViewBag.ActionName = "TypeWarehouse";

            return View("Index", getCategories(code));
        }

        public ViewResult EquimentGroup()
        {
            string code = "EquimentGroup";
            ViewBag.SuccessMessage = TempData[Globals.SuccessMessageKey];
            ViewBag.FailedMessage = TempData[Globals.FailedMessageKey];
            ViewBag.CategoryCode = code;
            ViewBag.ActionName = "EquimentGroup";

            return View("Index", getCategories(code));
        }

        public ViewResult Origin()
        {
            string code = "Origin";
            ViewBag.SuccessMessage = TempData[Globals.SuccessMessageKey];
            ViewBag.FailedMessage = TempData[Globals.FailedMessageKey];
            ViewBag.CategoryCode = code;
            ViewBag.ActionName = "Origin";

            return View("Index", getCategories(code));
        }
        #endregion

        #region Edit Category
        public ActionResult Edit(int Id, string ActionName)
        {
            var category = _categoryRepository.GetCategoryById(Id);
            var model = new CategoryViewModel();
            if (category != null && category.IsDeleted != true)
            {
                AutoMapper.Mapper.Map(category, model);
                return View(model);
            }

            ViewBag.ActionName = ActionName;
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult Edit(CategoryViewModel model, string ActionName, bool? IsPopup)
        {
            if (ModelState.IsValid)
            {
                if (Request["Submit"] == "Save")
                {
                    var category = _categoryRepository.GetCategoryById(model.Id);
                    AutoMapper.Mapper.Map(model, category);
                    category.ModifiedUserId = WebSecurity.CurrentUserId;
                    category.ModifiedDate = DateTime.Now;
                    _categoryRepository.UpdateCategory(category);

                    //ghi log 
                    Erp.BackOffice.Controllers.HomeController.WriteLog(category.Id, model.Name, "đã cập nhật danh mục", "SaleCategory/Edit/" + category.Id, Helpers.Common.CurrentUser.BranchId.Value);

                    if (IsPopup==true)
                    {
                        return RedirectToAction("_ClosePopup", "Home", new { area = "", FunctionCallback = "ClosePopupAndReloadPage" });
                    }
                    else
                    {
                        TempData[Globals.SuccessMessageKey] = App_GlobalResources.Wording.InsertSuccess;
                        return RedirectToAction(ActionName, new { Code = category.Code });
                    }
                }
                return RedirectToAction("Edit", new { categoryId = model.Id });
            }
            if (Request.UrlReferrer != null)
                return Redirect(Request.UrlReferrer.AbsoluteUri);
            return RedirectToAction("Index");
        }

        #endregion

        #region Create Category
        public ActionResult Create(string ActionName, string Code)
        {
            var model = new CategoryViewModel { };
            var count = _categoryRepository.GetAllCategories().Where(x => x.Code == Code).ToList();
           
            model.Code = Code;

            model.OrderNo = count.Count + 1;
            ViewBag.ActionName = ActionName;

            return View(model);
        }

        [HttpPost]
        public ActionResult Create(CategoryViewModel model, string ActionName, bool? IsPopup)
        {
            if (ModelState.IsValid)
            {
                var category = new Category();
                AutoMapper.Mapper.Map(model, category);
                category.CreatedUserId = WebSecurity.CurrentUserId;
                category.ModifiedUserId = WebSecurity.CurrentUserId;
                category.CreatedDate = DateTime.Now;
                category.ModifiedDate = DateTime.Now;

                if (string.IsNullOrEmpty(category.Value) == true)
                    category.Value = Helpers.Common.ChuyenThanhKhongDau(category.Name).Replace(" ", "_");

                _categoryRepository.InsertCategory(category);

                //ghi log 
                Erp.BackOffice.Controllers.HomeController.WriteLog(category.Id, model.Name, "đã tạo danh mục mới", "SaleCategory/Edit/" + category.Id, Helpers.Common.CurrentUser.BranchId.Value);

                if (IsPopup==true)
                {
                    return RedirectToAction("_ClosePopup", "Home", new { area = "", FunctionCallback = "ClosePopupAndReloadPage" });
                }
                else
                {
                    TempData[Globals.SuccessMessageKey] = App_GlobalResources.Wording.InsertSuccess;
                    return RedirectToAction(ActionName, new { Code = category.Code });
                }
            }
            return RedirectToAction("Create");
        }
        #endregion

        #region Delete Category

           [HttpPost]
        public ActionResult Delete(int id)
        {
            try
            {
                //var deleteCategoryId = int.Parse(Request["DeleteCategoryId"], CultureInfo.InvariantCulture);
                _categoryRepository.DeleteCategory(id);
                //TempData[Globals.SuccessMessageKey] = App_GlobalResources.Wording.DeleteSuccess;
                return Content("Success");
            }
            catch (DbUpdateException)
            {
                //TempData[Globals.FailedMessageKey] = App_GlobalResources.Error.RelationError;
                return Content("Error");
            }
        }
        #endregion

        #region ExportExcel
        public List<CategoryViewModel> IndexExport(string ActionName)
        {
            string Code = "";

            switch(ActionName)
            {
                case "ProductCategory":
                    Code = "product";
                    break;
                case "CustomerGroup":
                    Code = "CustomerGroup";
                    break;
                case "EconomicStatus":
                    Code = "EconomicStatus";
                    break;
                case "CustomerType":
                    Code = "CustomerType";
                    break;
                case "ProductUnit":
                    Code = "ProductUnit";
                    break;
                case "ProductManufacturer":
                    Code = "manufacturerList";
                    break;
                case "ProductGroup":
                    Code = "ProductGroup";
                    break;
                case "TitleSalarySetting":
                    Code = "TitleSalarySetting";
                    break;
                case "FloorList":
                    Code = "FloorList";
                    break;
                case "ServiceUnit":
                    Code = "serviceUnit";
                    break;
                case "TypeWarehouse":
                    Code = "Categories_product";
                    break;
                case "EquimentGroup":
                    Code = "EquimentGroup";
                    break;
                case "Origin":
                    Code = "Origin";
                    break;
            }

            IEnumerable<CategoryViewModel>  listCategory = new List<CategoryViewModel>();
            var model = _categoryRepository.GetAllCategories()
                .Where(item => item.Code == Code)
                .OrderBy(m => m.OrderNo).ToList();

            listCategory = AutoMapper.Mapper.Map(model, listCategory);

            return listCategory.ToList();
        }

        public string getTitle(string ActionName)
        {
            switch (ActionName)
            {
                case "ProductManufacturer":
                    return "nhà sản xuất";
                case "Origin":
                    return "nhãn hàng";
                case "EquimentGroup":
                    return "nhóm trang thiết bị";
                case "TypeWarehouse":
                    return "loại kho";
                case "ServiceUnit":
                    return "đơn vị dịch vụ";
                case "ProductUnit":
                    return "đơn vị sản phẩm";
                case "ProductCategory":
                    return "danh mục SP/DV";
                case "ProductGroup":
                    return "nhóm SP/DV";
                case "EconomicStatus":
                    return "tình trạng kinh tế";
                case "CustomerType":
                    return "loại khách hàng";
                case "CustomerGroup":
                    return "nhóm VIP";
            }

            return "";
        }

        public string getFileName(string ActionName)
        {
            switch (ActionName)
            {
                case "ProductManufacturer":
                    return "NhaSanXuat";
                case "Origin":
                    return "NhanHang";
                case "EquimentGroup":
                    return "NhomTrangThietBi";
                case "TypeWarehouse":
                    return "LoaiKho";
                case "ServiceUnit":
                    return "DonViDichVu";
                case "ProductUnit":
                    return "DonViSanPham";
                case "ProductCategory":
                    return "DanhMucSPDV";
                case "ProductGroup":
                    return "NhomSPDV";
                case "EconomicStatus":
                    return "TinhTrangKinhTe";
                case "CustomerType":
                    return "LoaiKhachHang";
                case "CustomerGroup":
                    return "NhomVIP";
            }

            return "";
        }

        public ActionResult ExportExcel(string ActionName, int? BranchId, bool ExportExcel = false)
        {
            var data = IndexExport(ActionName);

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
            model.Content = model.Content.Replace("{Title}", "Danh sách " + getTitle(ActionName));
            if (ExportExcel)
            {
                Response.AppendHeader("content-disposition", "attachment;filename=" + "DS_" + getFileName(ActionName) + DateTime.Now.ToString("yyyyMMdd") + ".xls");
                Response.Charset = "";
                Response.Cache.SetCacheability(HttpCacheability.NoCache);
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.Write(model.Content);
                Response.End();
            }
            return View(model);
        }

        string buildHtml(List<CategoryViewModel> data)
        {
            //Tạo table html chi tiết phiếu xuất
            string detailLists = "<table border=\"1\" class=\"invoice-detail\">\r\n";
            detailLists += "<thead>\r\n";
            detailLists += "	<tr>\r\n";
            detailLists += "		<th>STT</th>\r\n";
            detailLists += "		<th>Tiêu đề</th>\r\n";
            detailLists += "		<th>Value</th>\r\n";
            detailLists += "	</tr>\r\n";
            detailLists += "</thead>\r\n";
            detailLists += "<tbody>\r\n";
            var index = 1;

            foreach (var item in data)
            {
                detailLists += "<tr>\r\n"
                + "<td class=\"text-center orderNo\">" + (index++) + "</td>\r\n"
                + "<td class=\"text-left \">" + item.Name + "</td>\r\n"
                + "<td class=\"text-left \">" + item.Value + "</td>\r\n"
                + "</tr>\r\n";
            }

            detailLists += "</tfoot>\r\n</table>\r\n";

            return detailLists;
        }
        #endregion

    }
}
