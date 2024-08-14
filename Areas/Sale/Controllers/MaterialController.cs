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
using System.Web;
using Excels = Microsoft.Office.Interop.Excel;
using ImportExeclModel = Erp.BackOffice.Sale.Models.ImportExeclModel;
using System.Transactions;
using Excel;
using System.Data;
using Erp.BackOffice.Account.Controllers;
using Erp.BackOffice.Account.Models;

namespace Erp.BackOffice.Sale.Controllers
{
    [Authorize]
    [InitializeSimpleMembership]
    [Erp.BackOffice.Helpers.NoCacheHelper]
    public class MaterialController : Controller
    {
        private readonly IMaterialOrServiceRepository materialRepository;
        private readonly IObjectAttributeRepository ObjectAttributeRepository;
        private readonly ISupplierRepository SupplierRepository;
        private readonly IUserRepository userRepository;
        private readonly ICategoryRepository categoryRepository;
        private readonly IInventoryMaterialRepository inventoryMaterialRepository;
        private readonly ITemplatePrintRepository templatePrintRepository;
        private readonly IMaterialInboundRepository MaterialInboundRepository;
        private readonly IWarehouseRepository WarehouseRepository;

        public MaterialController(
            IMaterialOrServiceRepository _Material
            , IObjectAttributeRepository _ObjectAttribute
            , ISupplierRepository _Supplier
            , IUserRepository _user
            , ICategoryRepository category
            ,IInventoryMaterialRepository inventoryMaterial
            , ITemplatePrintRepository _templatePrint
             , IMaterialInboundRepository _MaterialInbound
             , IWarehouseRepository _Warehouse
            )
        {
            materialRepository = _Material;
            ObjectAttributeRepository = _ObjectAttribute;
            SupplierRepository = _Supplier;
            userRepository = _user;
            categoryRepository = category;
            inventoryMaterialRepository = inventoryMaterial;
            templatePrintRepository = _templatePrint;
            MaterialInboundRepository = _MaterialInbound;
            WarehouseRepository = _Warehouse;
        }

        #region Index

        public ViewResult Index(string txtSearch, string txtCode, string CategoryCode, string ProductGroup)
        {
            IEnumerable<MaterialViewModel> q = materialRepository.GetAllvwMaterial().AsEnumerable()
                .Select(item => new MaterialViewModel
                {
                    Id = item.Id,
                    CreatedUserId = item.CreatedUserId,
                    CreatedDate = item.CreatedDate,
                    ModifiedUserId = item.ModifiedUserId,
                    ModifiedDate = item.ModifiedDate,
                    Name = item.Name,
                    Code = item.Code,
                    PriceInbound = item.PriceInbound,
                    PriceOutbound = item.PriceOutBound,
                    //Barcode = item.Barcode,
                    //Type = item.Type,
                    Unit = item.Unit,
                    CategoryCode = item.CategoryCode,
                    ProductGroup = item.ProductGroup
                }).OrderByDescending(m => m.Id);

            if (string.IsNullOrEmpty(txtSearch) == false || string.IsNullOrEmpty(txtCode) == false)
            {
                //txtSearch = txtSearch == "" ? "~" : Erp.BackOffice.Helpers.Common.ChuyenThanhKhongDau(txtSearch);
                txtCode = txtCode == "" ? "~" : txtCode.Trim().ToLower();
                q = q.Where(x => Erp.BackOffice.Helpers.Common.ChuyenThanhKhongDau(x.Name).Contains(Helpers.Common.ChuyenThanhKhongDau(txtCode)) || x.Code.ToLower().Contains(txtCode));
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

        #region Create
        public ViewResult Create()
        {
            var model = new MaterialViewModel();
            model.PriceInbound = 0;
            model.PriceOutbound = 0;
            //model.MinInventory = 0;
            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        //[ValidateAntiForgeryToken]
        public ActionResult Create(MaterialViewModel model)
        {
            if (ModelState.IsValid)
            {
                var Material = new Domain.Sale.Entities.Material();
                AutoMapper.Mapper.Map(model, Material);
                Material.IsDeleted = false;
                Material.CreatedUserId = WebSecurity.CurrentUserId;
                Material.ModifiedUserId = WebSecurity.CurrentUserId;
                Material.CreatedDate = DateTime.Now;
                Material.ModifiedDate = DateTime.Now;
                Material.ProductGroup = model.ProductGroup;
                //if (model.PriceInbound == null)
                //    Material.PriceInbound = 0;
                Material.Code = Material.Code.Trim();
                var path = System.Web.HttpContext.Current.Server.MapPath("~" + Helpers.Common.GetSetting("Material"));
                if (Request.Files["file-image"] != null)
                {
                    var file = Request.Files["file-image"];
                    if (file.ContentLength > 0)
                    {
                        string image_name = "Material_" + Helpers.Common.ChuyenThanhKhongDau(Regex.Replace(Material.Code, @"\s+", "_")) + "." + file.FileName.Split('.').Last();
                        bool isExists = System.IO.Directory.Exists(path);
                        if (!isExists)
                            System.IO.Directory.CreateDirectory(path);
                        file.SaveAs(path + image_name);
                        Material.Image_Name = image_name;
                    }
                }

                materialRepository.InsertMaterial(Material);

                //ghi log 
                Erp.BackOffice.Controllers.HomeController.WriteLog(Material.Id, Material.Code, "tạo vật tư", "Material/Edit/" + Material.Id, Helpers.Common.CurrentUser.BranchId.Value);


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
            var Material = materialRepository.GetAllMaterial()
                .Where(item => item.Code == code).FirstOrDefault();
            if (Material != null)
            {
                if (id == null || (id != null && Material.Id != id))
                    return Content("Trùng mã vật tư !");
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
        public ActionResult Edit(int? Id)
        {
            var Material = materialRepository.GetMaterialById(Id.Value);
            if (Material != null && Material.IsDeleted != true)
            {
                var model = new MaterialViewModel();
                AutoMapper.Mapper.Map(Material, model);
                string MaterialId = "," + model.Id + ",";
                var supplierList = SupplierRepository.GetAllSupplier().AsEnumerable().Where(item => ("," + item.ProductIdOfSupplier + ",").Contains(MaterialId) == true).ToList();
                ViewBag.supplierList = supplierList;

                return View(model);
            }

            if (Request.UrlReferrer != null)
                return Redirect(Request.UrlReferrer.AbsoluteUri);
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(MaterialViewModel model)
        {
            foreach (var modelKey in ModelState.Keys)
            {
                if (modelKey == "PriceInbound" || modelKey == "PriceOutbound")
                {
                    var index = ModelState.Keys.ToList().IndexOf(modelKey);
                    ModelState.Values.ElementAt(index).Errors.Clear();
                }
            }
            if (ModelState.IsValid)
            {
                var Material = materialRepository.GetMaterialById(model.Id);
                AutoMapper.Mapper.Map(model, Material);
                Material.ModifiedUserId = WebSecurity.CurrentUserId;
                Material.ModifiedDate = DateTime.Now;
                if (model.PriceInbound == null)
                    Material.PriceInbound = 0;
                var path = Helpers.Common.GetSetting("Material");
                var filepath = System.Web.HttpContext.Current.Server.MapPath("~" + path);
                if (Request.Files["file-image"] != null)
                {
                    var file = Request.Files["file-image"];
                    if (file.ContentLength > 0)
                    {
                        FileInfo fi = new FileInfo(Server.MapPath("~" + path) + Material.Image_Name);
                        if (fi.Exists)
                        {
                            fi.Delete();
                        }

                        string image_name = "Material_" + Helpers.Common.ChuyenThanhKhongDau(Regex.Replace(Material.Code, @"\s+", "_")) + "." + file.FileName.Split('.').Last();

                        bool isExists = System.IO.Directory.Exists(filepath);
                        if (!isExists)
                            System.IO.Directory.CreateDirectory(filepath);
                        file.SaveAs(filepath + image_name);
                        Material.Image_Name = image_name;
                    }
                }

                materialRepository.UpdateMaterial(Material);
                //ghi log 
                Erp.BackOffice.Controllers.HomeController.WriteLog(Material.Id, Material.Code, "đã cập nhật vật tư", "Material/Edit/" + Material.Id, Helpers.Common.CurrentUser.BranchId.Value);

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
                    var item = materialRepository.GetMaterialById(int.Parse(id, CultureInfo.InvariantCulture));
                    if (item != null)
                    {

                        item.IsDeleted = true;
                        materialRepository.UpdateMaterial(item);
                    }
                }
                else
                {
                     string idDeleteAll = Request["DeleteId-checkbox"];
                string[] arrDeleteId = idDeleteAll.Split(',');
                for (int i = 0; i < arrDeleteId.Count(); i++)
                {
                    var item = materialRepository.GetMaterialById(int.Parse(arrDeleteId[i], CultureInfo.InvariantCulture));
                    if (item != null)
                    {

                        item.IsDeleted = true;
                        materialRepository.UpdateMaterial(item);
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
            var list = materialRepository.GetAllMaterial().ToList();
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        #endregion

        [AllowAnonymous]
        public JsonResult GetListMaterial()
        {
            var pathimg = Helpers.Common.GetSetting("Material");
            var q = materialRepository.GetAllvwMaterial()
                .Select(item => new
                {
                    item.Id,
                    item.Name,
                    item.Code,
                    item.PriceOutBound,
                    item.ProductGroup,
                    item.Unit,
                    item.Image_Name,
                    item.CategoryCode
                })
                .OrderBy(item => item.Name)
                .ToList();
            return Json(q.Select(item => new { item.Id, Code = item.Code, Image = Helpers.Common.KiemTraTonTaiHinhAnhHoapd(item.Image_Name, "Material", "product", pathimg), Unit = item.Unit, Note = Erp.BackOffice.Helpers.Common.PhanCachHangNgan2(item.PriceOutBound)+"VNĐ/"+item.Unit, Price = item.PriceOutBound, Text = item.Name, Name = item.Name, Value = item.Id }), JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public JsonResult GetListMaterialInventory()
        {
            var branchId = Erp.BackOffice.Helpers.Common.CurrentUser.BranchId.Value;
            var productList = inventoryMaterialRepository.GetAllvwInventoryMaterialByBranchId(branchId).Where(x => x.Quantity > 0)
             .Select(item => new
             {
                 Id = item.MaterialId,
                 Name = item.MaterialName,
                 Code = item.MaterialCode,
                 PriceOutbound = item.MaterialPriceOutbound,
                 CategoryCode = item.CategoryCode,
                 Unit = item.MaterialUnit,
                 Image_Name = item.Image_Name,
                 QuantityTotalInventory = item.Quantity,
                 LoCode = item.LoCode,
                 ExpiryDate = item.ExpiryDate
             }).OrderBy(item => item.CategoryCode).ToList();
            var pathimg = Helpers.Common.GetSetting("Material");
            var json_data = productList.Select(item => new
            {
                item.Id,
                Code = item.Code,
                Image = Helpers.Common.KiemTraTonTaiHinhAnhHoapd(item.Image_Name, "Material", "product", pathimg),
                Unit = item.Unit,
                Price = item.PriceOutbound,
                Text = item.Code + " - " + item.Name + " (" + Helpers.Common.PhanCachHangNgan2(item.PriceOutbound) + "/" + item.Unit + ")",
                Name = item.Name,
                Value = item.Id,
                QuantityTotalInventory = item.QuantityTotalInventory,
                LoCode = item.LoCode,
                ExpiryDate = (item.ExpiryDate == null ? "" : item.ExpiryDate.Value.ToString("dd/MM/yyyy")),
                Note = "SL:" + item.QuantityTotalInventory + "  Lô:" + item.LoCode + "  HSD:" + (item.ExpiryDate.HasValue ? item.ExpiryDate.Value.ToString("dd/MM/yyyy") : "")
            });

            return Json(json_data, JsonRequestBehavior.AllowGet);
        }

        #region ExportExcel
        public List<MaterialViewModel> IndexExport(string txtCode, string CategoryCode, string ProductGroup, int? BranchId)
        {
            var q = materialRepository.GetAllMaterial().AsEnumerable()
                .Select(item => new MaterialViewModel
                {
                    Id = item.Id,
                    CreatedUserId = item.CreatedUserId,
                    CreatedDate = item.CreatedDate,
                    ModifiedUserId = item.ModifiedUserId,
                    ModifiedDate = item.ModifiedDate,
                    Name = item.Name,
                    Code = item.Code,
                    PriceInbound = item.PriceInbound,
                    PriceOutbound = item.PriceOutBound,
                    //Barcode = item.Barcode,
                    //Type = item.Type,
                    Unit = item.Unit,
                    CategoryCode = item.CategoryCode,
                    ProductGroup = item.ProductGroup
                }).OrderByDescending(m => m.Id).ToList();

            if (string.IsNullOrEmpty(txtCode) == false)
            {
                txtCode = txtCode == "" ? "~" : txtCode.ToLower();
                q = q.Where(x => Erp.BackOffice.Helpers.Common.ChuyenThanhKhongDau(x.Name).Contains(Helpers.Common.ChuyenThanhKhongDau(txtCode)) || x.Code.ToLower().Contains(txtCode)).ToList();
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
            model.Content = model.Content.Replace("{Title}", "Danh sách vật tư");
            if (ExportExcel)
            {
                Response.AppendHeader("content-disposition", "attachment;filename=" + "DS_Vattu" + DateTime.Now.ToString("yyyyMMdd") + ".xls");
                Response.Charset = "";
                Response.Cache.SetCacheability(HttpCacheability.NoCache);
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.Write(model.Content);
                Response.End();
            }
            return View(model);
        }

        string buildHtml(List<MaterialViewModel> data)
        {
            decimal? tong_tien = 0;
            //Tạo table html chi tiết phiếu xuất
            string detailLists = "<table border=\"1\" class=\"invoice-detail\">\r\n";
            detailLists += "<thead>\r\n";
            detailLists += "	<tr>\r\n";
            detailLists += "		<th>STT</th>\r\n";
            detailLists += "		<th>Mã Vật Tư</th>\r\n";
            detailLists += "		<th>Tên Vật Tư</th>\r\n";
            detailLists += "		<th>Giá Xuất</th>\r\n";
            detailLists += "		<th>Danh Mục</th>\r\n";
            detailLists += "		<th>Nhóm</th>\r\n";
            detailLists += "		<th>Ngày Tạo</th>\r\n";
            detailLists += "		<th>Ngày Cập Nhật</th>\r\n";
            detailLists += "	</tr>\r\n";
            detailLists += "</thead>\r\n";
            detailLists += "<tbody>\r\n";
            var index = 1;

            foreach (var item in data)
            {
                decimal? subtotal = item.PriceOutbound;
                tong_tien += subtotal;
                detailLists += "<tr>\r\n"
                + "<td class=\"text-center orderNo\">" + (index++) + "</td>\r\n"
                + "<td class=\"text-left \">" + item.Code + "</td>\r\n"
                + "<td class=\"text-left \">" + item.Name + "</td>\r\n"
                + "<td class=\"text-left\">" + Erp.BackOffice.Helpers.CommonSatic.ToCurrencyStr(item.PriceOutbound, null) + "</td>\r\n"
                + "<td class=\"text-left \">" + item.CategoryCode + "</td>\r\n"
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



        #region Xu ly file excel
        //Get action method
        [System.Web.Mvc.HttpGet]
        public ActionResult ImportFile()
        {
            var resultData = new List<ImportExeclModel>();
            return View(resultData);
        }
        [System.Web.Mvc.HttpPost]
        public ActionResult ImportFile(HttpPostedFileBase file)
        {
            var resultData = new List<ImportExeclModel>();
            var path = string.Empty;
            if (file != null && file.ContentLength > 0)
            {
                var fileName = DateTime.Now.Ticks + file.FileName;
                path = Path.Combine(Server.MapPath("~/fileuploads/"),
                Path.GetFileName(fileName));


                file.SaveAs(path);
                ViewBag.FileName = fileName;
            }

            resultData = ReadDataFileExcel(path);

            return View(resultData);
        }
        private List<ImportExeclModel> ReadDataFileExcel(string filePath)
        {
            var resultData = new List<ImportExeclModel>();
            //read file
            //resultData = ReadDataFromFileExcel(path);
            Excels.Application app = new Excels.Application();
            Excels.Workbook wb = app.Workbooks.Open(filePath);
            Excels.Worksheet ws = wb.ActiveSheet;
            Excels.Range range = ws.UsedRange;
            for (int row = 1; row <= range.Rows.Count; row++)
            {
                var product = new ImportExeclModel()
                {
                    

                    //LoaiHang = ((Excels.Range)range.Cells[row, 1]).Text,
                    ///NhomHang = ((Excels.Range)range.Cells[row, 2]).Text,
                    MaHang = ((Excels.Range)range.Cells[row, 1]).Text,
                    TenHangHoa = ((Excels.Range)range.Cells[row, 2]).Text,
                    //GiaBan = ((Excels.Range)range.Cells[row, 4]).Text,
                    GiaVon = ((Excels.Range)range.Cells[row, 3]).Text,
                    TonKho = ((Excels.Range)range.Cells[row, 4]).Text,
                    //KHDat = ((Excels.Range)range.Cells[row, 8]).Text,
                    //TonNhoNhat = ((Excels.Range)range.Cells[row, 6]).Text,
                    /// TonLonNhat = ((Excels.Range)range.Cells[row, 10]).Text,
                    DVT = ((Excels.Range)range.Cells[row, 5]).Text,
                    //ThuocTinh = ((Excels.Range)range.Cells[row, 8]).Text,
                   // MaHHLienQuan = ((Excels.Range)range.Cells[row, 7]).Text,
                    // TrongLuong = ((Excels.Range)range.Cells[row, 17]).Text,
                    //DangKinhDoanh = ((Excels.Range)range.Cells[row, 18]).Text,
                    //BanTrucTiep = ((Excels.Range)range.Cells[row, 19]).Text,

                };
                resultData.Add(product);
            }
            return resultData;
        }

        private List<ImportExeclModel> ReadDataFileExcel2(string filePath)
        {
            var resultData = new List<ImportExeclModel>();

            if (filePath != "")
            {
                using (FileStream stream = System.IO.File.Open(filePath, FileMode.Open, FileAccess.Read))
                {

                    //1. Reading from a binary Excel file ('97-2003 format; *.xls)
                    //IExcelDataReader excelReader = ExcelReaderFactory.CreateBinaryReader(stream);
                    //...
                    //2. Reading from a OpenXml Excel file (2007 format; *.xlsx)
                    using (var excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream))
                    {
                        //...

                        //3. DataSet - The result of each spreadsheet will be created in the result.Tables
                        //DataSet result = excelReader.AsDataSet();
                        //...
                        //4. DataSet - Create column names from first row
                        excelReader.IsFirstRowAsColumnNames = true;
                        DataSet result = excelReader.AsDataSet();

                        //5. Data Reader methods
                        while (excelReader.Read())
                        {
                            var donhang = new ImportExeclModel()
                            {
                                //LoaiHang = excelReader.GetString(0),
                                //NhomHang = excelReader.GetString(1),
                                //MaHang = excelReader.GetString(2),
                                //TenHangHoa = excelReader.GetString(3),
                                //GiaBan = excelReader.GetString(4),
                                //GiaVon = excelReader.GetString(5),
                                //TonKho = excelReader.GetString(6),
                                //KHDat = excelReader.GetString(7),
                                //TonNhoNhat = excelReader.GetString(8),
                                //TonLonNhat = excelReader.GetString(9),
                                //DVT = excelReader.GetString(10),
                                //ThuocTinh = excelReader.GetString(13),
                                //MaHHLienQuan = excelReader.GetString(14),
                                //TrongLuong = excelReader.GetString(16),
                                //DangKinhDoanh = excelReader.GetString(17),
                                //BanTrucTiep = excelReader.GetString(18),


                                //LoaiHang = excelReader.GetString(0),
                                // NhomHang = excelReader.GetString(1),
                                MaHang = excelReader.GetString(1),
                                TenHangHoa = excelReader.GetString(0),
                                GiaBan = excelReader.GetString(3),
                                GiaVon = excelReader.GetString(2),
                                TonKho = excelReader.GetString(4),
                                // KHDat = excelReader.GetString(7),
                                TonNhoNhat = excelReader.GetString(5),
                                //  TonLonNhat = excelReader.GetString(9),
                                // DVT = excelReader.GetString(10),
                                ThuocTinh = excelReader.GetString(6),
                                MaHHLienQuan = excelReader.GetString(5)
                                //TrongLuong = excelReader.GetString(16),
                                //DangKinhDoanh = excelReader.GetString(17),
                                //BanTrucTiep = excelReader.GetString(18),
                            };
                            resultData.Add(donhang);
                            //excelReader.GetInt32(0);
                        }

                        //6. Free resources (IExcelDataReader is IDisposable)
                        excelReader.Close();
                    }
                }
            }
            else
            {
                Response.Write("<script>alert('Chưa Chọn File Excel')</script>");
            }

            //readfile 

            //FileStream stream = File.Open(filePath, FileMode.Open, FileAccess.Read);


            return resultData;

        }

        /// <summary>
        /// Doc du lieu tu file excel vua upload insert vao db
        /// </summary>
        /// <param name="currentFile"></param>
        /// <returns></returns>
        [System.Web.Mvc.HttpPost]
        public ActionResult SaveFileExcel(string currentFile)
        {
            var path = Path.Combine(Server.MapPath("~/fileuploads/"),
            Path.GetFileName(currentFile));
            var dataExcels = ReadDataFileExcel(path);
            var listProductCodeDaco = new List<string>();
            var listproductid = new List<int>();


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

            // goi code insert vao db
            var warehouse = WarehouseRepository.GetAllWarehouse().Where(x => x.BranchId == intBrandID && x.Categories == "VT").FirstOrDefault();

            using (TransactionScope scope =
             new TransactionScope(TransactionScopeOption.Required,
                                   new System.TimeSpan(0, 30, 0)))
            {
                var ProductInbound = new MaterialInbound();
                ProductInbound.IsDeleted = false;
                ProductInbound.CreatedUserId = WebSecurity.CurrentUserId;
                ProductInbound.ModifiedUserId = WebSecurity.CurrentUserId;
                ProductInbound.CreatedDate = DateTime.Now;
                ProductInbound.ModifiedDate = DateTime.Now;
                ProductInbound.BranchId = intBrandID;
                ProductInbound.WarehouseDestinationId = warehouse.Id;
                List<MaterialInboundDetail> listNewCheckSameId = new List<MaterialInboundDetail>();
                int i = 0;
                foreach (var importExeclModel in dataExcels)
                {
                    //goi insert tung product vao bang Sale_Product
                    if (i > 0)
                    {
                        //Kiem tra trung ma sp truoc khi insert
                        var productExist = materialRepository.GetAllvwMaterial().Where(x=> x.Code == importExeclModel.MaHang).FirstOrDefault();

                        //Neu no null tuc la no chua co trong bang product
                        if (productExist == null)
                        {
                            var product = new Material();
                            product.IsDeleted = false;
                            product.Code = importExeclModel.MaHang;
                            product.Name = importExeclModel.TenHangHoa;
                            //product.Origin = importExeclModel.LoaiHang;
                            //product.Manufacturer = importExeclModel.LoaiHang;
                            //product.CategoryCode = importExeclModel.LoaiHang;
                            // product.ProductGroup = importExeclModel.NhomHang;
                            product.Unit = importExeclModel.DVT;
                            //product.ProductLinkId = 0;
                            product.Size = importExeclModel.ThuocTinh;
                            product.PriceInbound = Convert.ToDecimal(importExeclModel.GiaVon.Replace(",", string.Empty));
                            product.PriceOutBound = Convert.ToDecimal(importExeclModel.GiaVon.Replace(",", string.Empty));
                            product.MinInventory = 0;//Convert.ToInt32(importExeclModel.TonNhoNhat.Replace(".0", string.Empty));
                            product.CreatedDate = DateTime.Now;
                            //product.Type = "product";
                            // product.Origin = importExeclModel.MaHHLienQuan;
                            materialRepository.InsertMaterial(product);

                            int tonkho = 0;
                            if (importExeclModel.TonKho != null)
                            {
                                tonkho = Convert.ToInt32(importExeclModel.TonKho.Replace(".0", string.Empty).Replace(",", string.Empty));
                            }
                            //Tạo phiếu nhập kho 
                            listNewCheckSameId.Add(new Domain.Sale.Entities.MaterialInboundDetail
                            {
                                MaterialId = product.Id,
                                Quantity = tonkho,
                                Unit = product.Unit,
                                Price = product.PriceInbound,
                                IsDeleted = false,
                                CreatedUserId = WebSecurity.CurrentUserId,
                                ModifiedUserId = WebSecurity.CurrentUserId,
                                CreatedDate = DateTime.Now,
                                ModifiedDate = DateTime.Now,
                                //ExpiryDate = group.ExpiryDate,
                                // LoCode = group.LoCode
                            });
                        }
                        else
                        {
                            listProductCodeDaco.Add(importExeclModel.MaHang);
                            int tonkho = 0;
                            if (importExeclModel.TonKho != null)
                            {
                                tonkho = Convert.ToInt32(importExeclModel.TonKho.Replace(".0", string.Empty));
                            }
                            //listproductid.Add(productExist.Id);
                            listNewCheckSameId.Add(new Domain.Sale.Entities.MaterialInboundDetail
                            {
                                MaterialId = productExist.Id,
                                Quantity = tonkho,
                                Unit = productExist.Unit,
                                Price = productExist.PriceInbound,
                                IsDeleted = false,
                                CreatedUserId = WebSecurity.CurrentUserId,
                                ModifiedUserId = WebSecurity.CurrentUserId,
                                CreatedDate = DateTime.Now,
                                ModifiedDate = DateTime.Now,
                                //ExpiryDate = group.ExpiryDate,
                                // LoCode = group.LoCode
                            });
                        }
                    }
                    i++;

                }

                ProductInbound.TotalAmount = listNewCheckSameId.Sum(x => x.Price * (decimal)x.Quantity);
                // ProductInbound.Type = (order.Id == 0 ? "manual" : (order.SupplierId != null ? "external" : "internal"));
                if (listNewCheckSameId.Any())
                {

                    MaterialInboundRepository.InsertMaterialInbound(ProductInbound);
                    ProductInbound.Code = Erp.BackOffice.Helpers.Common.GetOrderNo("MaterialInbound");
                    //string prefix = Erp.BackOffice.Helpers.Common.GetSetting("prefixOrderNo_Inbound");
                    // ProductInbound.Code = Erp.BackOffice.Helpers.Common.GetCode(prefix, ProductInbound.Id);

                    MaterialInboundRepository.UpdateMaterialInbound(ProductInbound);

                    Erp.BackOffice.Helpers.Common.SetOrderNo("MaterialInbound");
                    //Thêm chi tiết phiếu nhập

                    foreach (var item in listNewCheckSameId)
                    {
                        item.MaterialInboundId = ProductInbound.Id;
                        MaterialInboundRepository.InsertMaterialInboundDetail(item);
                    }

                    //Thêm vào quản lý chứng từ
                    TransactionController.Create(new TransactionViewModel
                    {
                        TransactionModule = "MaterialInBound",
                        TransactionCode = ProductInbound.Code,
                        TransactionName = "Nhập kho vật tư"
                    });

                }
                scope.Complete();
            }
            if (listProductCodeDaco.Count > 0)
            {
                //ViewBag.ErrorMesseage = $"Ma du lieu them vao da ton tai:{string.Join(",", listProductCodeDaco)}";
                return View("ImportFile", dataExcels);
            }
            return RedirectToAction("Index");
        }


       

        #endregion
    }
}
