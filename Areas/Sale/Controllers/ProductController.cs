using Erp.BackOffice.Filters;
using Erp.BackOffice.Sale.Models;
using Erp.Domain.Helper;
using Erp.Domain.Interfaces;
using Erp.Domain.Sale.Entities;
using Erp.Domain.Sale.Interfaces;
using Erp.Utilities;
using Excel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Infrastructure;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using WebMatrix.WebData;
using Excels = Microsoft.Office.Interop.Excel;
using ImportExeclModel = Erp.BackOffice.Sale.Models.ImportExeclModel;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Transactions;
using Erp.BackOffice.Account.Controllers;
using Erp.BackOffice.Account.Models;

namespace Erp.BackOffice.Sale.Controllers
{
    [System.Web.Mvc.Authorize]
    [InitializeSimpleMembership]
    [Erp.BackOffice.Helpers.NoCacheHelper]
    public class ProductController : Controller
    {
        private readonly IProductOrServiceRepository productRepository;
        private readonly IObjectAttributeRepository ObjectAttributeRepository;
        private readonly ISupplierRepository SupplierRepository;
        private readonly IUserRepository userRepository;
        private readonly ICategoryRepository categoryRepository;
        private readonly IProductDetailRepository productDetailRepository;
        private readonly IMaterialRepository materialRepository;
        private readonly IInventoryRepository inventoryRepository;
        private readonly IWarehouseRepository warehouseRepository;
        private readonly ITemplatePrintRepository templatePrintRepository;
        private readonly IProductInboundRepository ProductInboundRepository;
        private readonly IProductOutboundRepository productOutboundRepository;
        private readonly IDM_NHOMSANPHAMRepositories dm_nhomsanphamRepository;
        private readonly IProductInSkinCareRepository ProductInSkinCareRepository;

        public ProductController(
            IMaterialRepository _Material
            , IProductDetailRepository _ProductDetail
            , IProductOrServiceRepository _Product
            , IObjectAttributeRepository _ObjectAttribute
            , ISupplierRepository _Supplier
            , IUserRepository _user
            , ICategoryRepository category
            , IInventoryRepository inventory
            , IWarehouseRepository warehouse
            , ITemplatePrintRepository _templatePrint
            , IProductInboundRepository _ProductInbound
            , IProductOutboundRepository _ProductOutbound
             , IDM_NHOMSANPHAMRepositories dm_nhomsanpham
             , IProductInSkinCareRepository _IProductInSkinCare
            )
        {
            materialRepository = _Material;
            productDetailRepository = _ProductDetail;
            productRepository = _Product;
            ObjectAttributeRepository = _ObjectAttribute;
            SupplierRepository = _Supplier;
            userRepository = _user;
            categoryRepository = category;
            inventoryRepository = inventory;
            warehouseRepository = warehouse;
            templatePrintRepository = _templatePrint;
            ProductInboundRepository = _ProductInbound;
            productOutboundRepository = _ProductOutbound;
            dm_nhomsanphamRepository = dm_nhomsanpham;
            ProductInSkinCareRepository = _IProductInSkinCare;
        }


        #region Index

        public ViewResult Index(string isDisplay,string txtSearch, string txtCode, string CategoryCode, string ProductGroup, SearchObjectAttributeViewModel SearchOjectAttr)
        {
            IEnumerable<ProductViewModel> q = productRepository.GetAllvwProduct().Where(x => x.Type == "product").AsEnumerable()
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
                    ProductGroup = item.ProductGroup,
                    DiscountStaff = item.DiscountStaff,
                    IsMoneyDiscount = item.IsMoneyDiscount,
                    Origin = item.Origin,
                    NotUse = item.NotUse,
                    is_display = item.is_display
                }).OrderByDescending(m => m.Id);

            //var group = q.GroupBy(x => x.CategoryCode).ToList();
            //int dem = 1;
            //foreach (var item in group)
            //{
            //    Category category = new Category();
            //    category.IsDeleted = false;
            //    category.CreatedUserId = WebSecurity.CurrentUserId;
            //    category.ModifiedUserId = Web Security.CurrentUserId;
            //    category.CreatedDate = DateTime.Now;
            //    category.ModifiedDate = DateTime.Now;
            //    category.Value = item.Key;
            //    category.Name = item.Key;
            //    category.Code = "product";
            //    category.OrderNo = dem;
            //    categoryRepository.InsertCategory(category);
            //    dem++;
            //}
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
            if(isDisplay == "on")
            {
                q = q.Where(x => x.is_display == true);
            }

            if (string.IsNullOrEmpty(txtCode) == false)
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
            //decimal minPriceInbound;
            //if (decimal.TryParse(txtMinPrice, out minPriceInbound))
            //{
            //    q = q.Where(x => x.PriceInbound >= minPriceInbound);
            //}

            //decimal maxPriceInbound;
            //if (decimal.TryParse(txtMaxPrice, out maxPriceInbound))
            //{
            //    q = q.Where(x => x.PriceInbound <= maxPriceInbound);
            //}


            ViewBag.SuccessMessage = TempData["SuccessMessage"];
            ViewBag.FailedMessage = TempData["FailedMessage"];
            ViewBag.AlertMessage = TempData["AlertMessage"];
            return View(q);
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

            resultData = ReadDataFileExcel2(path);

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
                    //NhomHang = ((Excels.Range)range.Cells[row, 2]).Text,
                    //MaHang = ((Excels.Range)range.Cells[row, 3]).Text,
                    //TenHangHoa = ((Excels.Range)range.Cells[row, 4]).Text,
                    //GiaBan = ((Excels.Range)range.Cells[row, 5]).Text,
                    //GiaVon = ((Excels.Range)range.Cells[row, 6]).Text,
                    //TonKho = ((Excels.Range)range.Cells[row, 7]).Text,
                    //KHDat = ((Excels.Range)range.Cells[row, 8]).Text,
                    //TonNhoNhat = ((Excels.Range)range.Cells[row, 9]).Text,
                    //TonLonNhat = ((Excels.Range)range.Cells[row, 10]).Text,
                    //DVT = ((Excels.Range)range.Cells[row, 11]).Text,
                    //ThuocTinh = ((Excels.Range)range.Cells[row, 14]).Text,
                    //MaHHLienQuan = ((Excels.Range)range.Cells[row, 15]).Text,
                    //TrongLuong = ((Excels.Range)range.Cells[row, 17]).Text,
                    //DangKinhDoanh = ((Excels.Range)range.Cells[row, 18]).Text,
                    //BanTrucTiep = ((Excels.Range)range.Cells[row, 19]).Text,

                    //LoaiHang = ((Excels.Range)range.Cells[row, 1]).Text,
                    ///NhomHang = ((Excels.Range)range.Cells[row, 2]).Text,
                    MaHang = ((Excels.Range)range.Cells[row, 2]).Text,
                    TenHangHoa = ((Excels.Range)range.Cells[row, 1]).Text,
                    GiaBan = ((Excels.Range)range.Cells[row, 4]).Text,
                    GiaVon = ((Excels.Range)range.Cells[row, 3]).Text,
                    TonKho = ((Excels.Range)range.Cells[row, 5]).Text,
                    //KHDat = ((Excels.Range)range.Cells[row, 8]).Text,
                    TonNhoNhat = ((Excels.Range)range.Cells[row, 6]).Text,
                   /// TonLonNhat = ((Excels.Range)range.Cells[row, 10]).Text,
                   // DVT = ((Excels.Range)range.Cells[row, 11]).Text,
                    ThuocTinh = ((Excels.Range)range.Cells[row, 8]).Text,
                    MaHHLienQuan = ((Excels.Range)range.Cells[row, 7]).Text,
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
            var dataExcels = ReadDataFileExcel2(path);
            var listProductCodeDaco = new List<string>();
            var listproductid = new List<int>();
            // goi code insert vao db
            var warehouse = warehouseRepository.GetAllWarehouse().Where(x => x.BranchId == 0).FirstOrDefault();

            using (TransactionScope scope =
             new TransactionScope(TransactionScopeOption.Required,
                                   new System.TimeSpan(0, 30, 0)))
            {
                var ProductInbound = new ProductInbound();
                ProductInbound.IsDeleted = false;
                ProductInbound.CreatedUserId = WebSecurity.CurrentUserId;
                ProductInbound.ModifiedUserId = WebSecurity.CurrentUserId;
                ProductInbound.CreatedDate = DateTime.Now;
                ProductInbound.ModifiedDate = DateTime.Now;
                ProductInbound.BranchId = 0;
                ProductInbound.WarehouseDestinationId = warehouse.Id;
                List<ProductInboundDetail> listNewCheckSameId = new List<ProductInboundDetail>();
                int i = 0;
                foreach (var importExeclModel in dataExcels)
                {
                    //goi insert tung product vao bang Sale_Product
                    if (i > 0)
                    {
                        //Kiem tra trung ma sp truoc khi insert
                        var productExist = productRepository.GetProductByCode(importExeclModel.MaHang);

                        //Neu no null tuc la no chua co trong bang product
                        if (productExist == null)
                        {
                            var product = new Product();
                            product.IsDeleted = false;
                            product.Code = importExeclModel.MaHang;
                            product.Name = importExeclModel.TenHangHoa;
                            //product.Origin = importExeclModel.LoaiHang;
                            //product.Manufacturer = importExeclModel.LoaiHang;
                            //product.CategoryCode = importExeclModel.LoaiHang;
                            // product.ProductGroup = importExeclModel.NhomHang;
                            // product.Unit = importExeclModel.DVT;
                            product.ProductLinkId = 0;
                            product.Size = importExeclModel.ThuocTinh;
                            product.PriceInbound = Convert.ToDecimal(importExeclModel.GiaVon.Replace(",", string.Empty));
                            product.PriceOutbound = Convert.ToDecimal(importExeclModel.GiaBan.Replace(",", string.Empty));
                            product.MinInventory = Convert.ToInt32(importExeclModel.TonNhoNhat.Replace(".0", string.Empty));
                            product.CreatedDate = DateTime.Now;
                            product.Type = "product";
                            product.Origin = importExeclModel.MaHHLienQuan;
                            productRepository.InsertProduct(product);

                            int tonkho = 0;
                            if (importExeclModel.TonKho != null)
                            {
                                tonkho = Convert.ToInt32(importExeclModel.TonKho.Replace(".0", string.Empty).Replace(",", string.Empty));
                            }
                            //Tạo phiếu nhập kho 
                            listNewCheckSameId.Add(new Domain.Sale.Entities.ProductInboundDetail
                            {
                                ProductId = product.Id,
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
                            listNewCheckSameId.Add(new Domain.Sale.Entities.ProductInboundDetail
                            {
                                ProductId = productExist.Id,
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

                ProductInbound.TotalAmount = listNewCheckSameId.Sum(x => x.Price * x.Quantity);
                // ProductInbound.Type = (order.Id == 0 ? "manual" : (order.SupplierId != null ? "external" : "internal"));
                if (listNewCheckSameId.Any())
                {

                    ProductInboundRepository.InsertProductInbound(ProductInbound);
                   ProductInbound.Code = Erp.BackOffice.Helpers.Common.GetOrderNo("ProductInbound");
                    //string prefix = Erp.BackOffice.Helpers.Common.GetSetting("prefixOrderNo_Inbound");
                   // ProductInbound.Code = Erp.BackOffice.Helpers.Common.GetCode(prefix, ProductInbound.Id);

                    ProductInboundRepository.UpdateProductInbound(ProductInbound);

                    Erp.BackOffice.Helpers.Common.SetOrderNo("ProductInbound");
                    //Thêm chi tiết phiếu nhập

                    foreach (var item in listNewCheckSameId)
                    {
                        item.ProductInboundId = ProductInbound.Id;
                        ProductInboundRepository.InsertProductInboundDetail(item);
                    }

                    //Thêm vào quản lý chứng từ
                    TransactionController.Create(new TransactionViewModel
                    {
                        TransactionModule = "ProductInbound",
                        TransactionCode = ProductInbound.Code,
                        TransactionName = "Nhập kho"
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


        //[System.Web.Mvc.HttpPost]
        //public ActionResult ImportFile(HttpPostedFileBase file, string submitBtn, string currentFile)
        //{
        //    var resultData = new List<ImportExeclModel>();
        //    var path = string.Empty;
        //    if (file != null && file.ContentLength > 0)
        //    {
        //        var fileName = DateTime.Now.Ticks + file.FileName;
        //        path = Path.Combine(Server.MapPath("~/fileuploads/"),
        //            Path.GetFileName(fileName));
        //        file.SaveAs(path);
        //        ViewBag.FileName = fileName;
        //    }
        //    else
        //    {
        //        if (string.IsNullOrEmpty(currentFile) == false)
        //        {
        //            path = Path.Combine(Server.MapPath("~/fileuploads/"),
        //                Path.GetFileName(currentFile));
        //        }
        //    }

        //    //read file
        //    try
        //    {
        //        //resultData = ReadDataFromFileExcel(path);
        //        Excels.Application app = new Excels.Application();
        //        Excels.Workbook wb = app.Workbooks.Open(path);
        //        Excels.Worksheet ws = wb.ActiveSheet;
        //        Excels.Range range = ws.UsedRange;
        //        for (int row = 1; row <= range.Rows.Count; row++)
        //        {
        //            var product = new ImportExeclModel()
        //            {

        //                LoaiHang = ((Excels.Range)range.Cells[row, 1]).Text,
        //                NhomHang = ((Excels.Range)range.Cells[row, 2]).Text,
        //                MaHang = ((Excels.Range)range.Cells[row, 3]).Text,
        //                TenHangHoa = ((Excels.Range)range.Cells[row, 4]).Text,
        //                GiaBan = ((Excels.Range)range.Cells[row, 5]).Text,
        //                GiaVon = ((Excels.Range)range.Cells[row, 6]).Text,
        //                TonKho = ((Excels.Range)range.Cells[row, 7]).Text,
        //                KHDat = ((Excels.Range)range.Cells[row, 8]).Text,
        //                TonNhoNhat = ((Excels.Range)range.Cells[row, 9]).Text,
        //                TonLonNhat = ((Excels.Range)range.Cells[row, 10]).Text,
        //                DVT = ((Excels.Range)range.Cells[row, 11]).Text,
        //                ThuocTinh = ((Excels.Range)range.Cells[row, 14]).Text,
        //                MaHHLienQuan = ((Excels.Range)range.Cells[row, 15]).Text,
        //                TrongLuong = ((Excels.Range)range.Cells[row, 17]).Text,
        //                DangKinhDoanh = ((Excels.Range)range.Cells[row, 18]).Text,
        //                BanTrucTiep = ((Excels.Range)range.Cells[row, 19]).Text,

        //            };
        //            resultData.Add(product);
        //        }
        //    }
        //    
        //    }






        //public List<ProductViewModel> ReadDataFromFileExcel(string filePath)
        //{
        //    var resultData = new List<ProductViewModel>();
        //    //Viet code doc file excel roi Add vao resultData
        //    Excel.Application app = new Excel.Application();
        //    Excel.Workbook wb = app.Workbooks.Open(path);
        //    Excel.Worksheet ws = wb.ActiveSheet;
        //    Excel.Range range = ws.UsedRange;
        //    List<Product> ListProduct = new List<Product>();
        //    for (int row = 1; row <= range.Rows.Count; row++)
        //    {
        //        Product p = new Product();
        //        p.LoaiHang = ((Excel.Range)range.Cells[row, 1]).Text;
        //        p.TenHang = ((Excel.Range)range.Cells[row, 4]).Text;
        //        p.GiaBan = ((Excel.Range)range.Cells[row, 5]).Text;
        //        ListProduct.Add(p);
        //    }

        //    return resultData;
        //}

        //[HttpPost]
        //public ActionResult Import(HttpPostedFileBase excelfile)
        //{
        //    if (excelfile == null || excelfile.ContentLength == 0)
        //    {
        //        ViewBag.Error = "Chọn file Excel";
        //        return View("Index");
        //    }
        //    else
        //    {
        //        if (excelfile.FileName.EndsWith("xls") || excelfile.FileName.EndsWith("xlsx"))
        //        {
        //            string path = Server.MapPath("~/UpLoadfile/" + excelfile.FileName);
        //            if (System.IO.File.Exists(path))
        //                System.IO.File.Delete(path);
        //            excelfile.SaveAs(path);
        //            //đọ file
        //            Excel.Application app = new Excel.Application();
        //            Excel.Workbook wb = app.Workbooks.Open(path);
        //            Excel.Worksheet ws = wb.ActiveSheet;
        //            Excel.Range range = ws.UsedRange;
        //            List<Product> ListProduct = new List<Product>();
        //            for (int row = 1; row <= range.Rows.Count; row++)
        //            {
        //                Product p = new Product();
        //                p.LoaiHang = ((Excel.Range)range.Cells[row, 1]).Text;
        //                p.TenHang = ((Excel.Range)range.Cells[row, 4]).Text;
        //                p.GiaBan = ((Excel.Range)range.Cells[row, 5]).Text;
        //                ListProduct.Add(p);
        //            }

        //            ViewBag.ListProduct = ListProduct;
        //            return View("Success");
        //        }
        //        else
        //        {
        //            ViewBag.Error = "Định dạng file không đúng <br/>";
        //            return View("Index");
        //        }
        //    }

        //}


        #endregion
        // xuất excel chưa xong
        public List<ProductViewModel> IndexExport(string txtCode, string CategoryCode, string ProductGroup, int? BranchId)
        {
            var q = productRepository.GetAllProduct().Where(x => x.Type == "product").AsEnumerable()
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
                    ProductGroup = item.ProductGroup,
                    DiscountStaff = item.DiscountStaff,
                    IsMoneyDiscount = item.IsMoneyDiscount,
                    Origin = item.Origin,
                    NotUse = item.NotUse
                }).OrderByDescending(m => m.Id).ToList();

            //var group = q.GroupBy(x => x.CategoryCode).ToList();
            //int dem = 1;
            //foreach (var item in group)
            //{
            //    Category category = new Category();
            //    category.IsDeleted = false;
            //    category.CreatedUserId = WebSecurity.CurrentUserId;
            //    category.ModifiedUserId = Web Security.CurrentUserId;
            //    category.CreatedDate = DateTime.Now;
            //    category.ModifiedDate = DateTime.Now;
            //    category.Value = item.Key;
            //    category.Name = item.Key;
            //    category.Code = "product";
            //    category.OrderNo = dem;
            //    categoryRepository.InsertCategory(category);
            //    dem++;
            //}

            if (string.IsNullOrEmpty(txtCode) == false)
            {
                //txtSearch = txtSearch == "" ? "~" : Erp.BackOffice.Helpers.Common.ChuyenThanhKhongDau(txtSearch);
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
        /// <summary>
        /// </summary>
        /// <param name="txtCode"></param>
        /// <param name="CategoryCode"></param>
        /// <param name="ProductGroup"></param>
        /// <param name="BranchId"></param>
        /// <param name="ExportExcel"></param>
        /// <returns></returns>
        /// 
        // chỉnh sửa tiền danh mục, xuất tổng tiền dưới góc
        #region export
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
            model.Content = model.Content.Replace("{Title}", "Danh sách sản phẩm");
            if (ExportExcel)
            {
                Response.AppendHeader("content-disposition", "attachment;filename=" + "DS_Sanpham" + DateTime.Now.ToString("yyyyMMdd") + ".xls");
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
            detailLists += "		<th>Mã Sản Phẩm</th>\r\n";
            detailLists += "		<th>Tên Sản Phẩm</th>\r\n";
            detailLists += "		<th>Giá Xuất</th>\r\n";
            detailLists += "		<th>Danh Mục</th>\r\n";
            detailLists += "		<th>Nhóm</th>\r\n";
            detailLists += "		<th>Nhãn Hàng</th>\r\n";
            detailLists += "		<th>Ngày Tạo</th>\r\n";
            detailLists += "		<th>Khóa</th>\r\n";
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
                + "<td class=\"text-left \">" + item.Origin.ToString() + "</td>\r\n"
                + "<td class=\"text-left \">" + item.CreatedDate + "</td>\r\n"
                + "<td class=\"text-left \">" + (item.NotUse == true ? "Đã Khóa": "" )+ "</td>\r\n"
                //+ "<td>" + (item.ModifiedDate.HasValue ? item.ModifiedDate.Value.ToString("dd/MM/yyyy HH:mm") : "") + "</td>"
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
        [System.Web.Mvc.HttpPost]
        [ValidateInput(false)]
        //[ValidateAntiForgeryToken]
        public ActionResult Create(ProductViewModel model)
        {
            Helpers.Common.ResetCacheYHL("MainPartial_Mainkissy,lineService_GroupDetails,lineproduct_GroupDetails,SearchSpa_GroupDetails,alllsproduct_GroupDetails,catelistproduct_GroupDetails");
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
                Product.NHOMSANPHAM_ID_LST = Product.NHOMSANPHAM_ID.ToString();              
                Product.IS_ALOW_BAN_AM = true;
                string k = model.Description_brief;
                if (model.Description_brief != null)
                {
                    Product.Meta_Description_brief = Regex.Replace(k.Replace("&nbsp;", " "), "<.*?>", String.Empty);//
                }
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
                Erp.BackOffice.Controllers.HomeController.WriteLog(Product.Id, Product.Code, "đã tạo sản phẩm", "Product/Edit/" + Product.Id, Helpers.Common.CurrentUser.BranchId.Value);

                int ProductId = Product.Id;
                int Quantity = model.Quantity;

                //tạo đặc tính động cho sản phẩm nếu có danh sách đặc tính động truyền vào và đặc tính đó phải có giá trị
                ObjectAttributeController.CreateOrUpdateForObject(Product.Id, model.AttributeValueList);

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
            var Product = productRepository.GetProductById(Id);
            if (Product != null && Product.IsDeleted != true)
            {
                var model = new ProductViewModel();
                AutoMapper.Mapper.Map(Product, model);
                string productId = "," + model.Id + ",";
                var supplierList = SupplierRepository.GetAllSupplier().AsEnumerable().Where(item => ("," + item.ProductIdOfSupplier + ",").Contains(productId) == true).ToList();
                ViewBag.supplierList = supplierList;

                var productSkincare = ProductInSkinCareRepository.GetAllProductInSkinCareByProductId(model.Id).Select(
                   item => new ProductInSkinCareViewModel
                   {
                       Id = item.Id,
                       Name = item.Name,
                       ProductId = item.ProductId,
                       ProductIdMem = item.ProductIdMem,
                       IsDeleted = item.IsDeleted,
                       OrderNo = item.OrderNo
                   }
                   ).ToList();


                if (productSkincare != null)
                {
                    foreach (var i in productSkincare)
                    {
                        var pro = productRepository.GetProductById(i.ProductIdMem);
                        if (pro != null)
                        {
                            i.ProductName = pro.Name;
                            i.ProductCode = pro.Code;
                        }
                    }
                }
                ViewBag.productSkincare = productSkincare;

                return View(model);
            }
            if (Request.UrlReferrer != null)
                return Redirect(Request.UrlReferrer.AbsoluteUri);

            return RedirectToAction("Index");
        }

        [System.Web.Mvc.HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(ProductViewModel model)
        {
            //foreach (var modelKey in ModelState.Keys)
            //{
            //    if (modelKey == "PriceInbound" || modelKey == "PriceOutbound")
            //    {
            //        var index = ModelState.Keys.ToList().IndexOf(modelKey);
            //        ModelState.Values.ElementAt(index).Errors.Clear();
            //    }
            //}
            Helpers.Common.ResetCacheYHL("MainPartial_Mainkissy,lineService_GroupDetails,lineproduct_GroupDetails,SearchSpa_GroupDetails,alllsproduct_GroupDetails,catelistproduct_GroupDetails");
            if (ModelState.IsValid)
            {
                var Product = productRepository.GetProductById(model.Id);
                AutoMapper.Mapper.Map(model, Product);
                Product.ModifiedUserId = WebSecurity.CurrentUserId;
                Product.ModifiedDate = DateTime.Now;
                Product.NHOMSANPHAM_ID_LST = Product.NHOMSANPHAM_ID.ToString();
                Product.IS_ALOW_BAN_AM = true;
                string k = model.Description_brief;
                if (model.Description_brief != null)
                {
                    Product.Meta_Description_brief = Regex.Replace(k.Replace("&nbsp;", " "), "<.*?>", String.Empty);//
                }
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


                //tạo hoặc cập nhật đặc tính động cho sản phẩm nếu có danh sách đặc tính động truyền vào và đặc tính đó phải có giá trị
                ObjectAttributeController.CreateOrUpdateForObject(Product.Id, model.AttributeValueList);

                productRepository.UpdateProduct(Product);
                int ProductId = Product.Id;
                int Quantity = model.Quantity;

                //ghi log 
                Erp.BackOffice.Controllers.HomeController.WriteLog(Product.Id, Product.Code, "đã cập nhật sản phẩm", "Product/Edit/" + Product.Id, Helpers.Common.CurrentUser.BranchId.Value);

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
        [System.Web.Mvc.HttpPost]
        public ActionResult Delete()
        {
            Helpers.Common.ResetCacheYHL("MainPartial_Mainkissy,lineService_GroupDetails,lineproduct_GroupDetails,SearchSpa_GroupDetails,alllsproduct_GroupDetails,catelistproduct_GroupDetails");
            try
            {
                string id = Request["Delete"];
                string idDeleteAll = Request["DeleteId-checkbox"];
                string isNotUse = Request["checkUse"];
                if (isNotUse != null && isNotUse != "")
                {
                    string[] arrDeleteId = isNotUse.Split(',');
                    for (int i = 0; i < arrDeleteId.Count(); i++)
                    {
                        var item = productRepository.GetProductById(int.Parse(arrDeleteId[i], CultureInfo.InvariantCulture));
                        if (item != null)
                        {
                            if(item.NotUse != true)
                            {
                                item.NotUse = true;
                            }
                            else
                            {
                                item.NotUse = false;
                            }
                            
                            productRepository.UpdateProduct(item);
                        }
                    }
                }
                else
                {
                    if (id != null)
                    {
                        var item = productRepository.GetProductById(int.Parse(id, CultureInfo.InvariantCulture));
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
                    else
                    {
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

        #region GetListProductInventoryAndService
        [AllowAnonymous]
        public JsonResult GetListProductInventoryAndService()
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
            var branchId = intBrandID;
            //var branchId = (Erp.BackOffice.Helpers.Common.CurrentUser.BranchId == null ? Convert.ToInt32(Session["GlobalCurentBranchId"]) : Erp.BackOffice.Helpers.Common.CurrentUser.BranchId.Value);
            var productList = inventoryRepository.GetAllvwInventoryByBranchId(branchId.Value).Where(x => x.Quantity > 0 && x.IsSale == true)
             .Select(item => new ProductViewModel
             {
                 Id = item.ProductId,
                 Name = item.ProductName,
                 Type = "product",
                 Code = item.ProductCode,
                 PriceOutbound = item.ProductPriceOutbound,
                 CategoryCode = item.CategoryCode,
                 Unit = item.ProductUnit,
                 Image_Name = item.Image_Name,
                 QuantityTotalInventory = item.Quantity,
                 LoCode = item.LoCode,
                 ExpiryDate = item.ExpiryDate,
                 Origin = item.Origin,
                 Categories = item.Categories

             }).OrderBy(item => item.CategoryCode).ToList();
            //var product = new List<ProductViewModel>();
            ////xử lý sp khóa
            //foreach(var item in productList)
            //{
            //    var pro = productRepository.GetProductById(item.Id);
            //    if(pro != null && pro.NotUse != true)
            //    {
            //        product.Add(item);
            //    }
            //}
            //productList = product;

            var q = productRepository.GetAllvwService()
                .Select(item => new ProductViewModel
                {
                    Id = item.Id,
                    Name = item.Name,
                    Type = item.Type,
                    Code = item.Code,
                    PriceOutbound = item.PriceOutbound,
                    CategoryCode = item.CategoryCode,
                    Unit = item.Unit,
                    Image_Name = item.Image_Name,
                    QuantityTotalInventory = 0,
                    LoCode = null,
                    ExpiryDate = null,
                    Origin = item.Origin,
                    NotUse = item.NotUse
                }).Where(x=> x.NotUse != true)
                .OrderBy(item => item.Name)
                .ToList();
            var data = productList.Union(q).ToList();
            var pathimg = Helpers.Common.GetSetting("product-image-folder");
            var json_data = data.Select(item => new
            {
                item.Id,
                Origin = item.Origin,
                Code = item.Code,
                Image = Helpers.Common.KiemTraTonTaiHinhAnhHoapd(item.Image_Name, "product-image-folder", "product",pathimg),
                Type = item.Type,
                Unit = item.Unit,
                Price = item.PriceOutbound,
                Text = item.Code + " - " + item.Name + " (" + Helpers.Common.PhanCachHangNgan2(item.PriceOutbound) + "/" + item.Unit + ")",
                Name = item.Name,
                Value = item.Id,
                QuantityTotalInventory = item.QuantityTotalInventory,
                LoCode = item.LoCode,
                Categories = (item.Categories == null ? "" : item.Categories),
                ExpiryDate = (item.ExpiryDate == null ? "" : item.ExpiryDate.Value.ToString("dd/MM/yyyy")),
                Note = (item.Type == "product" ? " _SL:" + item.QuantityTotalInventory + "  Lô:" + item.LoCode + "  HSD:" + (item.ExpiryDate.HasValue ? item.ExpiryDate.Value.ToString("dd/MM/yyyy") : "") : ""),
            });
            return Json(json_data, JsonRequestBehavior.AllowGet);
        }


        //Lấy sản phẩm cho kHBH
        [AllowAnonymous]
        public JsonResult GetListProductInventoryAndServiceCRM()
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
            var branchId = intBrandID;
            //var branchId = (Erp.BackOffice.Helpers.Common.CurrentUser.BranchId == null ? Convert.ToInt32(Session["GlobalCurentBranchId"]) : Erp.BackOffice.Helpers.Common.CurrentUser.BranchId.Value);
            var productList = productRepository.GetAllvwProduct().Where(x => x.NotUse != true && x.Type == "product")
             .Select(item => new ProductViewModel
             {
                 Id = item.Id,
                 Name = item.Name,
                 Type = item.Type,
                 Code = item.Code,
                 PriceOutbound = item.PriceOutbound,
                 CategoryCode = item.CategoryCode,
                 Unit = item.Unit,
                 Image_Name = item.Image_Name,
                 QuantityTotalInventory = item.QuantityTotalInventory,
                 LoCode = "",
                 ExpiryDate = null,
                 Origin = item.Origin,
                 Categories = ""

             }).OrderBy(item => item.CategoryCode).ToList();
            //var product = new List<ProductViewModel>();
            ////xử lý sp khóa
            //foreach(var item in productList)
            //{
            //    var pro = productRepository.GetProductById(item.Id);
            //    if(pro != null && pro.NotUse != true)
            //    {
            //        product.Add(item);
            //    }
            //}
            //productList = product;

            var q = productRepository.GetAllvwService()
                .Select(item => new ProductViewModel
                {
                    Id = item.Id,
                    Name = item.Name,
                    Type = item.Type,
                    Code = item.Code,
                    PriceOutbound = item.PriceOutbound,
                    CategoryCode = item.CategoryCode,
                    Unit = item.Unit,
                    Image_Name = item.Image_Name,
                    QuantityTotalInventory = 0,
                    LoCode = null,
                    ExpiryDate = null,
                    Origin = item.Origin,
                    NotUse = item.NotUse
                }).Where(x => x.NotUse != true)
                .OrderBy(item => item.Name)
                .ToList();
            var data = productList.Union(q).ToList();
            var pathimg = Helpers.Common.GetSetting("Customer");
            var json_data = data.Select(item => new
            {
                item.Id,
                Origin = item.Origin,
                Code = item.Code,
                Image = Helpers.Common.KiemTraTonTaiHinhAnhHoapd(item.Image_Name, "product-image-folder", "product", pathimg),
                Type = item.Type,
                Unit = item.Unit,
                Price = item.PriceOutbound,
                Text = item.Code + " - " + item.Name + " (" + Helpers.Common.PhanCachHangNgan2(item.PriceOutbound) + "/" + item.Unit + ")",
                Name = item.Name,
                Value = item.Id,
                QuantityTotalInventory = item.QuantityTotalInventory,
                LoCode = item.LoCode,
                Categories = (item.Categories == null ? "" : item.Categories),
                ExpiryDate = (item.ExpiryDate == null ? "" : item.ExpiryDate.Value.ToString("dd/MM/yyyy")),
                Note = (item.Type == "product" ? " _SL:" + item.QuantityTotalInventory + "  Lô:" + item.LoCode + "  HSD:" + (item.ExpiryDate.HasValue ? item.ExpiryDate.Value.ToString("dd/MM/yyyy") : "") : ""),
            });
            return Json(json_data, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region GetListProductPackage
        [System.Web.Mvc.AllowAnonymous]
        public JsonResult GetListProductPackage()
        {

            var q = productRepository.GetAllProduct().Where(x => x.Type == "productpackage")
                .Select(item => new ProductViewModel
                {
                    Id = item.Id,
                    Name = item.Name,
                    Type = item.Type,
                    Code = item.Code,
                    PriceOutbound = item.PriceOutbound,
                    CategoryCode = item.CategoryCode,
                    Unit = item.Unit,
                    Image_Name = item.Image_Name,
                    QuantityTotalInventory = 0,
                    Origin = item.Origin,
                    LoCode = null,
                    ExpiryDate = null
                })
                .OrderBy(item => item.Name)
                .ToList();
            var pathimg = Helpers.Common.GetSetting("product-image-folder");
            var json_data = q.Select(item => new
            {
                item.Id,
                Code = item.Code,
                Image = Helpers.Common.KiemTraTonTaiHinhAnhHoapd(item.Image_Name, "product-image-folder", "product", pathimg),
                Type = item.Type,
                Price = item.PriceOutbound,
                Text = item.Name,
                Name = item.Name,
                Value = item.Id,
                Note = item.CategoryCode
            });
            return Json(json_data, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region GetListProductInventoryAndServiceKM
        [System.Web.Mvc.AllowAnonymous]
        public JsonResult GetListProductInventoryAndServiceKM()
        {
            var branchId = Erp.BackOffice.Helpers.Common.CurrentUser.BranchId.Value;
            var warehouseList = warehouseRepository.GetvwAllWarehouse().ToList();
            var WarehouseId = warehouseList.FirstOrDefault(x => x.BranchId == branchId && ("," + x.Categories + ",").Contains(",KKM,"));
            var listInventory = Domain.Helper.SqlHelper.QuerySP<InventoryViewModel>("spSale_Get_Inventory", new { WarehouseId = WarehouseId.Id, HasQuantity = '1', ProductCode = "", ProductName = "", CategoryCode = "", ProductGroup = "", BranchId = branchId, LoCode = "", ProductId = "", ExpiryDate = "" });
            var productList = listInventory.Select(item => new ProductViewModel
            {
                Id = item.ProductId.Value,
                Name = item.ProductName,
                Type = "product",
                Code = item.ProductCode,
                PriceOutbound = item.ProductPriceOutbound,
                CategoryCode = item.CategoryCode,
                Unit = item.ProductUnit,
                Image_Name = item.Image_Name,
                QuantityTotalInventory = item.Quantity,
                LoCode = item.LoCode,
                ExpiryDate = item.ExpiryDate,
                Origin = item.Origin
            }).OrderBy(item => item.CategoryCode).ToList();

            var q = productRepository.GetAllvwService()
                .Select(item => new ProductViewModel
                {
                    Id = item.Id,
                    Name = item.Name,
                    Type = item.Type,
                    Code = item.Code,
                    PriceOutbound = item.PriceOutbound,
                    CategoryCode = item.CategoryCode,
                    Unit = item.Unit,
                    Image_Name = item.Image_Name,
                    QuantityTotalInventory = 0,
                    LoCode = null,
                    ExpiryDate = null,
                    Origin = item.Origin
                })
                .OrderBy(item => item.Name)
                .ToList();
            var data = productList.Union(q).ToList();
            var pathimg = Helpers.Common.GetSetting("product-image-folder");
            var json_data = data.Select(item => new
            {
                item.Id,
                Code = item.Code,
                Image = Helpers.Common.KiemTraTonTaiHinhAnhHoapd(item.Image_Name, "product-image-folder", "product",pathimg),
                Type = item.Type,
                Unit = item.Unit,
                Price = item.PriceOutbound,
                Text = item.Code + " - " + item.Name + " (" + Helpers.Common.PhanCachHangNgan2(item.PriceOutbound) + "/" + item.Unit + ")",
                Name = item.Name,
                Value = item.Id,
                QuantityTotalInventory = item.QuantityTotalInventory,
                LoCode = item.LoCode,
                ExpiryDate = (item.ExpiryDate == null ? "" : item.ExpiryDate.Value.ToString("dd/MM/yyyy")),
                Origin = item.Origin,
                Note = item.Origin
            });
            return Json(json_data, JsonRequestBehavior.AllowGet);
        }
        #endregion



        #region In Mã Vạch
        public ActionResult PrintBarCode(int? Id, string source)
        {




            var model = new ProductInboundViewModel();
            var ProductInbound = new vwProductInbound();
            var ProductOutbound = new vwProductOutbound();
            model.DetailList = new List<ProductInboundDetailViewModel>();
            var i = 0;
            if (Id != null && Id > 0 && source == "in")
            {
                ProductInbound = ProductInboundRepository.GetvwProductInboundFullById(Id.Value);
                var Details = ProductInboundRepository.GetAllvwProductInboundDetailByInboundId(ProductInbound.Id)
                       .Select(x => new ProductInboundDetailViewModel
                       {
                           Id = x.Id,
                           Price = x.Price,
                           ProductId = x.ProductId,
                           ProductInboundId = x.ProductInboundId,
                           Quantity = x.Quantity,
                           ProductCode = "",
                           ExpiryDate = x.ExpiryDate,
                           LoCode = x.LoCode,
                           ProductDamagedId = x.ProductDamagedId,
                           Reason = x.Reason,
                           NumberAmount = x.NumberAmount,
                           Status = x.Status
                       }).OrderBy(x => x.Id).ToList();
                foreach (var item in Details)
                {
                    i++;
                    var pro = productRepository.GetProductById(item.ProductId.Value);
                    item.ProductCode = pro.Code;
                    item.ProductName = pro.Name;
                    item.Price = pro.PriceOutbound;
                    item.OrderNo = i;
                }
                model.DetailList = Details;
            }
            if (Id != null && Id > 0 && source == "out")
            {
                ProductOutbound = productOutboundRepository.GetvwProductOutboundFullById(Id.Value);
                var Details = productOutboundRepository.GetAllvwProductOutboundDetailByOutboundId(ProductOutbound.Id)
                       .Select(x => new ProductInboundDetailViewModel
                       {
                           Id = x.Id,
                           Price = x.Price,
                           ProductId = x.ProductId,
                           ProductInboundId = x.ProductOutboundId,
                           Quantity = x.Quantity,
                           ProductCode = x.ProductCode,
                           ExpiryDate = x.ExpiryDate,
                           LoCode = x.LoCode

                       }).OrderBy(x => x.Id).ToList();
                foreach (var item in Details)
                {
                    i++;
                    var pro = productRepository.GetProductById(item.ProductId.Value);
                    item.ProductCode = pro.Code;
                    item.ProductName = pro.Name;
                    item.Price = pro.PriceOutbound;
                    item.OrderNo = i;
                }
                model.DetailList = Details;
            }

            var productList = productRepository.GetAllvwProduct()
           .Select(item => new ProductViewModel
           {
               Code = item.Code,
               Barcode = item.Barcode,
               Name = item.Name,
               Id = item.Id,
               CategoryCode = item.CategoryCode,
               PriceInbound = item.PriceOutbound,
               Unit = item.Unit,
               Image_Name = item.Image_Name,
               Origin = item.Origin
           });
            ViewBag.productList = productList;
            var product = productList.ToList();
            return View(model);
        }


        #region LoadProductItem
        public PartialViewResult LoadProductItem(int OrderNo, int ProductId, string ProductName, string Unit, int Quantity, decimal Price, string ProductCode, string ProductType, string LoCode, string ExpiryDate)
        {
            var model = new ProductInboundDetailViewModel();
            model.ProductName = ProductName;
            model.OrderNo = OrderNo;
            model.ProductId = ProductId;
            model.Unit = Unit;
            model.Quantity = Quantity;
            model.Price = Price;
            model.ProductCode = ProductCode;
            model.ProductType = ProductType;
            model.LoCode = LoCode;
            var pro2 = productRepository.GetAllProduct().ToList();
            var pro = productRepository.GetAllProduct().FirstOrDefault(x => x.Code == ProductCode);
            foreach (var item in pro2)
            {
                if (pro.Id == item.Id)
                {
                    if (model.ProductName == null)
                    {
                        model.OrderNo = OrderNo - 1;

                    }

                    //if (Price != pro.PriceInbound)
                    //{
                    //    model.Price = pro.PriceInbound;
                    //}
                    model.ProductId = pro.Id;
                    model.ProductName = pro.Name;
                    model.ProductType = pro.Type;
                    model.Unit = pro.Unit;
                    if (!string.IsNullOrEmpty(ExpiryDate))
                        model.ExpiryDate = Convert.ToDateTime(ExpiryDate);
                }
                else
                {
                    continue;
                }
            }





            return PartialView(model);
        }
        #endregion


        public ActionResult PrintBarCode2(int? Id)
        {
            var ProductInbound = new vwProductInbound();
            if (Id != null)
                ProductInbound = ProductInboundRepository.GetvwProductInboundFullById(Id.Value);

            var model = new ProductInboundViewModel();
            model.DetailList = new List<ProductInboundDetailViewModel>();
            var Details = ProductInboundRepository.GetAllvwProductInboundDetailByInboundId(ProductInbound.Id)
                        .Select(x => new ProductInboundDetailViewModel
                        {
                            Id = x.Id,
                            Price = x.Price,
                            ProductId = x.ProductId,
                            ProductInboundId = x.ProductInboundId,
                            Quantity = x.Quantity,
                            ProductCode = "",
                            ExpiryDate = x.ExpiryDate,
                            LoCode = x.LoCode,
                            ProductDamagedId = x.ProductDamagedId,
                            Reason = x.Reason,
                            NumberAmount = x.NumberAmount,
                            Status = x.Status
                        }).OrderBy(x => x.Id).ToList();
            model.DetailList = Details;
            var productList = productRepository.GetAllvwProduct()
               .Select(item => new ProductViewModel
               {
                   Code = item.Code,
                   Barcode = item.Barcode,
                   Name = item.Name,
                   Id = item.Id,
                   CategoryCode = item.CategoryCode,
                   PriceInbound = item.PriceOutbound,
                   Unit = item.Unit,
                   Image_Name = item.Image_Name,
                   Origin = item.Origin
               });
            ViewBag.productList = productList;
            return View(model);
        }




        [HttpPost]
        [ValidateInput(false)]
        public JsonResult Inmavachpdf(List<sanphamexcel> listsp)
        {

            try
            {
                string fileName = @"BarcodePdf" + DateTime.Now.Ticks + ".pdf";
                string strReportName = Path.Combine(Server.MapPath("~/fileuploads"), fileName);
                if (listsp != null && listsp.Count > 0)
                {

                    //in ma vach
                    // begin phan tham so
                    float myWidth = float.Parse(Erp.BackOffice.Helpers.Common.GetSetting("myWidthBARCODE"));
                    float myHeight = float.Parse(Erp.BackOffice.Helpers.Common.GetSetting("myHeightBARCODE"));
                    float newwithImage = float.Parse(Erp.BackOffice.Helpers.Common.GetSetting("newwithImageBARCODE"));
                    float intFontdongia = float.Parse(Erp.BackOffice.Helpers.Common.GetSetting("Fontdongia"));
                    float intFontTensanpham = float.Parse(Erp.BackOffice.Helpers.Common.GetSetting("FontTensanpham"));
                    float BarHeight = float.Parse(Erp.BackOffice.Helpers.Common.GetSetting("BarHeight"));

                    float marginleftPage = float.Parse(Erp.BackOffice.Helpers.Common.GetSetting("marginleftPage"));
                    float marginrightPage = float.Parse(Erp.BackOffice.Helpers.Common.GetSetting("marginrightPage"));
                    float margintopPage = float.Parse(Erp.BackOffice.Helpers.Common.GetSetting("margintopPage"));
                    float marginbottonPage = float.Parse(Erp.BackOffice.Helpers.Common.GetSetting("marginbottonPage"));


                    float SpacingBeforeDONGIA = float.Parse(Erp.BackOffice.Helpers.Common.GetSetting("SpacingBeforeDONGIA"));
                    float SpacingBeforeTEN = float.Parse(Erp.BackOffice.Helpers.Common.GetSetting("SpacingBeforeTEN"));

                    float SpacingAfterDONGIA = float.Parse(Erp.BackOffice.Helpers.Common.GetSetting("SpacingAfterDONGIA"));
                    float SpacingAfterTEN = float.Parse(Erp.BackOffice.Helpers.Common.GetSetting("SpacingAfterTEN"));



                    float IndentationLeftDONGIA = float.Parse(Erp.BackOffice.Helpers.Common.GetSetting("IndentationLeftDONGIA"));
                    float IndentationrightDONGIA = float.Parse(Erp.BackOffice.Helpers.Common.GetSetting("IndentationrightDONGIA"));

                    float IndentationLeftTEN = float.Parse(Erp.BackOffice.Helpers.Common.GetSetting("IndentationLeftTEN"));
                    float IndentationrightTEN = float.Parse(Erp.BackOffice.Helpers.Common.GetSetting("IndentationrightTEN"));




                    iTextSharp.text.Font fonttensanpham = new iTextSharp.text.Font(BaseFont.CreateFont(Server.MapPath("~/fileuploads") + "\\times.ttf", "Identity-H", embedded: false));
                    fonttensanpham.SetColor(0, 0, 0);
                    fonttensanpham.Size = intFontTensanpham;
                    Font boldFontdongia = new Font(Font.FontFamily.TIMES_ROMAN, intFontdongia, Font.BOLD);
                    float FixedHeightcell = float.Parse(Erp.BackOffice.Helpers.Common.GetSetting("FixedHeightcellBARCODE"));
                    float ox = float.Parse(Erp.BackOffice.Helpers.Common.GetSetting("oxBARCODE"));
                    float oy = float.Parse(Erp.BackOffice.Helpers.Common.GetSetting("oyBARCODE"));

                    // end phan tham so

                    var pgSize = new iTextSharp.text.Rectangle(myWidth, myHeight);
                    var pdfToCreate = new Document(pgSize, marginleftPage, marginrightPage, margintopPage, marginbottonPage);


                    // Create a new PdfWrite object, writing the output to a MemoryStream
                    var outputStream = new MemoryStream();

                    var pdfWriter = PdfWriter.GetInstance(pdfToCreate, new FileStream(strReportName, FileMode.Create));
                    PdfContentByte cb = new PdfContentByte(pdfWriter);
                    // Open the Document for writing
                    pdfToCreate.Open();
                    Barcode128 code128 = new Barcode128();
                    PdfPTable BarCodeTable = null;
                    int i = 0;
                    int i3 = 0;
                    int iitem = 0;
                    foreach (var item in listsp)
                    {
                        int SoLuong = int.Parse(item.SoLuong.Replace(".", ""));
                        for (int isoluon = 0; isoluon < SoLuong; isoluon++)
                        {


                            if (i % 3 == 0)
                            {
                                BarCodeTable = new PdfPTable(3);
                                BarCodeTable.ExtendLastRow = true;
                                // Create barcode
                                i3 = i;
                            }
                            code128.CodeType = Barcode.CODE128_UCC;
                            code128.Code = item.MaSanPham;
                            //code128.Extended = true;
                            code128.TextAlignment = Element.ALIGN_CENTER;
                            //code128.StartStopText = true;
                            //code128.ChecksumText = true;
                            //code128.GenerateChecksum = true;
                            code128.BarHeight = BarHeight;


                            // Generate barcode image
                            iTextSharp.text.Image image128 = code128.CreateImageWithBarcode(cb, null, null);

                            image128.ScaleAbsoluteWidth(newwithImage);

                            PdfPCell cell = new PdfPCell();
                            cell.FixedHeight = FixedHeightcell;
                            cell.MinimumHeight = FixedHeightcell;

                            //cell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                            //cell.VerticalAlignment = PdfPCell.ALIGN_CENTER;
                            Paragraph p = new Paragraph();
                            //cell.Border = 0;
                            //cell.BorderColor = BaseColor.WHITE;

                            p.Add(new Chunk(image128, ox, oy));
                            //p.Add(new Phrase(100,item.Ten));
                            //cell.Column= ct;

                            cell.AddElement(p);

                            //Paragraph p1 = new Paragraph();
                            ////cell.Border = 0;
                            ////cell.BorderColor = BaseColor.WHITE;
                            //p1.Add(new Phrase(" ", boldFontdongia));

                            ////cell.Column= ct;
                            //cell.AddElement(p1);


                            Paragraph p11 = new Paragraph();
                            //cell.Border = 0;
                            //cell.BorderColor = BaseColor.WHITE;
                            p11.SpacingBefore = SpacingBeforeDONGIA;
                            p11.SpacingAfter = SpacingAfterDONGIA;
                            p11.Add(new Phrase(item.DonGia, boldFontdongia));
                            p11.Alignment = Element.ALIGN_CENTER;
                            p11.IndentationLeft = IndentationLeftDONGIA;
                            p11.IndentationRight = IndentationrightDONGIA;
                            cell.AddElement(p11);




                            Paragraph p2 = new Paragraph();
                            //cell.Border = 0;
                            //cell.BorderColor = BaseColor.WHITE;
                            p2.SpacingBefore = SpacingBeforeTEN;
                            p2.SpacingAfter = SpacingAfterTEN;
                            p2.Add(new Phrase(item.Ten, fonttensanpham));
                            p2.Alignment = Element.ALIGN_CENTER;
                            p2.IndentationLeft = IndentationLeftTEN;
                            p2.IndentationRight = IndentationrightTEN;

                            //cell.Column= ct;
                            cell.AddElement(p2);
                            cell.Border = Rectangle.NO_BORDER;
                            BarCodeTable.AddCell(cell);


                            // Add image to table cell

                            // Add table to document
                            if (((i - i3 + 1) == 3) || ((isoluon == (SoLuong - 1) && iitem == listsp.Count - 1)))
                            {
                                if (((i - i3 + 1) != 3) && ((isoluon == (SoLuong - 1) && iitem == listsp.Count - 1)))
                                {
                                    for (int i1 = 0; i1 < (3 - ((i + 1) % 3)); i1++)
                                    {
                                        PdfPCell cell1 = new PdfPCell();
                                        cell1.Border = Rectangle.NO_BORDER;
                                        BarCodeTable.AddCell(cell1);
                                    }
                                }

                                pdfToCreate.Add(BarCodeTable);
                                BarCodeTable.CompleteRow();
                                pdfToCreate.NewPage();

                            }
                            i++;
                        }
                        iitem++;
                    }
                    pdfToCreate.Close();
                    TempData[Globals.SuccessMessageKey] = App_GlobalResources.Wording.UpdateSuccess;
                }
                //FeedGoogle("FeedGoogle");

                return Json(new { fileName = fileName, errorMessage = "" });
            }
            catch (Exception ex)
            {

                return Json(0);
            }


        }


        [HttpGet]
        public ActionResult Download(string file)
        {
            var path = Path.Combine(Server.MapPath("~/fileuploads"));
            var filename = Path.Combine(path, file);
            File(filename, "application/pdf");
            Response.AddHeader("Content-Disposition", "inline; filename=" + file);
            return File(filename, "application/pdf");
        }
        #endregion



        #region DM_NhomSanPham
        public ViewResult DM_NhomSanPham()
        {
            ///string pMA_DVIQLY = Helpers.Common.CurrentUser.MA_DVIQLY;
            var model = new DM_NHOMSANPHAMViewModel();

            //IEnumerable<DM_NHOMSANPHAMViewModel> list = new List<DM_NHOMSANPHAMViewModel>();
            var list = dm_nhomsanphamRepository.GetAllDM_NHOMSANPHAM().AsEnumerable()
            .Select(item => new DM_NHOMSANPHAMViewModel
            {
                NHOMSANPHAM_ID = item.NHOMSANPHAM_ID,
                CreatedUserId = item.CreatedUserId,
                CreatedDate = item.CreatedDate,
                ModifiedUserId = item.ModifiedUserId,
                ModifiedDate = item.ModifiedDate,
                AssignedUserId = item.AssignedUserId,
                TEN_NHOMSANPHAM = item.TEN_NHOMSANPHAM,
                CAP_NHOMSANPHAM = item.CAP_NHOMSANPHAM,
                STT = item.STT,
                NHOM_CHA = item.NHOM_CHA,
                BANNER = item.BANNER,
                IS_SHOW = item.IS_SHOW,
                META_TITLE = item.META_TITLE,
                META_DESCRIPTION = item.META_DESCRIPTION,
                URL_SLUG = item.URL_SLUG,
                Decription = item.Decription,
                IsProduct = item.IsProduct,
                Video = item.Video
            
            }).OrderBy(x => x.STT).ToList();
            //list = AutoMapper.Mapper.Map(model, list);


            ViewBag.DM_NHOMSANPHAM = list;
            ViewBag.SuccessMessage = TempData[Globals.SuccessMessageKey];
            ViewBag.FailedMessage = TempData[Globals.FailedMessageKey];
            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult CreateNhomSanPham(DM_NHOMSANPHAMViewModel model)
        {
            //string pMA_DVIQLY = Helpers.Common.CurrentUser.MA_DVIQLY;
            if (model.STT != null && model.TEN_NHOMSANPHAM != null)
            {

                //**create**//
                if (model.NHOMSANPHAM_ID == 0)
                {
                    using (var scope = new TransactionScope(TransactionScopeOption.Required, new System.TimeSpan(0, 30, 0)))
                    {
                        #region Create
                        var dm_nhomsanpham = new Domain.Sale.Entities.DM_NHOMSANPHAM();

                        AutoMapper.Mapper.Map(model, dm_nhomsanpham);
                       // dm_nhomsanpham.MA_DVIQLY = pMA_DVIQLY;
                        dm_nhomsanpham.IsDeleted = false;
                        dm_nhomsanpham.CreatedUserId = WebSecurity.CurrentUserId;
                        dm_nhomsanpham.ModifiedUserId = WebSecurity.CurrentUserId;
                        dm_nhomsanpham.CreatedDate = DateTime.Now;
                        dm_nhomsanpham.ModifiedDate = DateTime.Now;
                        dm_nhomsanpham.AssignedUserId = WebSecurity.CurrentUserId;
                        dm_nhomsanpham.Video = model.Video;
                        dm_nhomsanpham.IS_SHOW = Convert.ToInt32(Request["is-show-checkbox"]);//???
                        dm_nhomsanpham.IsProduct = model.IsProduct;
                        if (!String.IsNullOrEmpty(model.Decription))
                        {
                            dm_nhomsanpham.Decription = model.Decription.Trim();
                        }
                        var nhomsanpham = dm_nhomsanphamRepository.GetDM_NHOMSANPHAMByTEN_NHOMSANPHAM( model.TEN_NHOMSANPHAM);

                        if (nhomsanpham != null)
                        {
                            TempData[Globals.FailedMessageKey] = "Tên nhóm hàng đã tồn tại";
                            return RedirectToAction("DM_NhomSanPham");
                        }

                        if (model.NHOM_CHA == null || model.NHOM_CHA == 0)
                        {
                            dm_nhomsanpham.CAP_NHOMSANPHAM = 1;
                            dm_nhomsanpham.URL_SLUG = to_slug(dm_nhomsanpham.TEN_NHOMSANPHAM);
                        }
                        else
                        {
                            if (model.CAP_NHOMSANPHAM < 3)
                            {
                                var nhomcha = dm_nhomsanphamRepository.GetDM_NHOMSANPHAMByNHOMSANPHAM_ID( model.NHOM_CHA.Value);
                                dm_nhomsanpham.CAP_NHOMSANPHAM = nhomcha.CAP_NHOMSANPHAM + 1;
                                dm_nhomsanpham.URL_SLUG = to_slug(dm_nhomsanpham.TEN_NHOMSANPHAM) + "-" + nhomcha.URL_SLUG;
                            }
                            else
                            {
                                TempData[Globals.FailedMessageKey] = "Nhóm sản phẩm bạn chọn thuộc cấp 3";
                                return RedirectToAction("DM_NhomSanPham");
                            }

                        }

                        dm_nhomsanpham.BANNER = "";
                        dm_nhomsanphamRepository.InsertDM_NHOMSANPHAM(dm_nhomsanpham);
                        dm_nhomsanpham = dm_nhomsanphamRepository.GetDM_NHOMSANPHAMByNHOMSANPHAM_ID( dm_nhomsanpham.NHOMSANPHAM_ID);

                        //begin up hinh anh cho backend
                        var path = System.Web.HttpContext.Current.Server.MapPath("~" + Helpers.Common.GetSetting("productgroups-image-folder"));
                        if (Request.Files["file-image"] != null)
                        {
                            var file = Request.Files["file-image"];
                            if (file.ContentLength > 0)
                            {
                                string image_name = "productgroups_" + Helpers.Common.ChuyenThanhKhongDau(Regex.Replace(dm_nhomsanpham.NHOMSANPHAM_ID.ToString(), @"\s+", "_")) + file.FileName;
                                bool isExists = System.IO.Directory.Exists(path);
                                if (!isExists)
                                    System.IO.Directory.CreateDirectory(path);
                                file.SaveAs(path + image_name);
                                dm_nhomsanpham.BANNER = image_name;
                            }
                        }
                        //end up hinh anh cho backend

                        //begin up hinh anh cho client
                        path = Helpers.Common.GetSetting( "productgroups-image-folder-client");
                        string prjClient = Helpers.Common.GetSetting( "prj-client");
                        string filepath = System.Web.HttpContext.Current.Server.MapPath("~");
                        DirectoryInfo di = new DirectoryInfo(filepath);
                        filepath = di.Parent.FullName + "\\" + prjClient + path.Replace("/", @"\");
                        if (Request.Files["file-image"] != null)
                        {
                            var file = Request.Files["file-image"];
                            if (file.ContentLength > 0)
                            {
                                //FileInfo fi = new FileInfo(Server.MapPath("~" + path) + dm_nhomsanpham.BANNER);
                                //if (fi.Exists)
                                //{
                                //    fi.Delete();
                                //}

                                string image_name = "productgroups_" + Helpers.Common.ChuyenThanhKhongDau(Regex.Replace(dm_nhomsanpham.NHOMSANPHAM_ID.ToString(), @"\s+", "_")) + file.FileName;

                                bool isExists = System.IO.Directory.Exists(filepath);
                                if (!isExists)
                                    System.IO.Directory.CreateDirectory(filepath);
                                file.SaveAs(filepath + image_name);
                                dm_nhomsanpham.BANNER = image_name;
                            }
                        }
                        //end up hinh anh cho client

                        dm_nhomsanphamRepository.UpdateDM_NHOMSANPHAM(dm_nhomsanpham);
                        Erp.BackOffice.Controllers.HomeController.WriteLog(dm_nhomsanpham.NHOMSANPHAM_ID, dm_nhomsanpham.TEN_NHOMSANPHAM, "đã cập nhật sản phẩm", "DM_BannerSlider/Edit/" + dm_nhomsanpham.NHOMSANPHAM_ID, Helpers.Common.CurrentUser.BranchId.Value);
                        scope.Complete();
                        Helpers.Common.ResetCacheYHL("SearchSpa_GroupDetails,alllsproduct_GroupDetails,lineService_GroupDetails,lineproduct_GroupDetails,MainPartial_Mainkissy");

                        TempData[Globals.SuccessMessageKey] = App_GlobalResources.Wording.InsertSuccess;
                        return RedirectToAction("DM_NhomSanPham");
                        #endregion
                    }
                }
                else//**edit**//
                {
                    #region edit
                    var dm_nhomsanpham = dm_nhomsanphamRepository.GetDM_NHOMSANPHAMByNHOMSANPHAM_ID( model.NHOMSANPHAM_ID);
                    dm_nhomsanpham.ModifiedUserId = WebSecurity.CurrentUserId;
                    dm_nhomsanpham.ModifiedDate = DateTime.Now;
                    dm_nhomsanpham.AssignedUserId = WebSecurity.CurrentUserId;
                    dm_nhomsanpham.TEN_NHOMSANPHAM = model.TEN_NHOMSANPHAM;
                    dm_nhomsanpham.NHOM_CHA = model.NHOM_CHA;
                    dm_nhomsanpham.STT = model.STT;
                    dm_nhomsanpham.IS_SHOW = Convert.ToInt32(Request["is-show-checkbox"]);
                    dm_nhomsanpham.META_TITLE = model.META_TITLE;
                    dm_nhomsanpham.META_DESCRIPTION = model.META_DESCRIPTION;
                    dm_nhomsanpham.URL_SLUG = model.URL_SLUG;
                    dm_nhomsanpham.Video = model.Video;
                    dm_nhomsanpham.IsProduct = model.IsProduct;
                    if (!String.IsNullOrEmpty(model.Decription))
                    {
                        dm_nhomsanpham.Decription = model.Decription.Trim();
                    }
                    var oldItem = dm_nhomsanphamRepository.GetDM_NHOMSANPHAMByNHOMSANPHAM_ID( model.NHOMSANPHAM_ID);
                    if (oldItem.TEN_NHOMSANPHAM != model.TEN_NHOMSANPHAM)
                    {
                        var checkTen = dm_nhomsanphamRepository.GetDM_NHOMSANPHAMByTEN_NHOMSANPHAM(model.TEN_NHOMSANPHAM);
                        if (checkTen != null)
                        {
                            TempData[Globals.FailedMessageKey] = "Tên nhóm hàng đã tồn tại";
                            return RedirectToAction("DM_NhomSanPham");
                        }
                    }

                    if (model.NHOM_CHA == null || model.NHOM_CHA == 0)
                    {
                        dm_nhomsanpham.CAP_NHOMSANPHAM = 1;
                        dm_nhomsanpham.URL_SLUG = to_slug(dm_nhomsanpham.TEN_NHOMSANPHAM);
                    }
                    else
                    {
                        if (model.CAP_NHOMSANPHAM < 3)
                        {
                            if (model.NHOMSANPHAM_ID != model.NHOM_CHA)
                            {
                                var nhomcha = dm_nhomsanphamRepository.GetDM_NHOMSANPHAMByNHOMSANPHAM_ID( model.NHOM_CHA.Value);
                                dm_nhomsanpham.CAP_NHOMSANPHAM = nhomcha.CAP_NHOMSANPHAM + 1;
                                dm_nhomsanpham.URL_SLUG = to_slug(dm_nhomsanpham.TEN_NHOMSANPHAM) + "-" + nhomcha.URL_SLUG;
                            }
                            else
                            {
                                TempData[Globals.FailedMessageKey] = "Nhóm sản phẩm không thể nằm trong chính nó";
                                return RedirectToAction("DM_NhomSanPham");
                            }
                        }
                        else
                        {
                            TempData[Globals.FailedMessageKey] = "Nhóm sản phẩm chỉ có 3 cấp";
                            return RedirectToAction("DM_NhomSanPham");
                        }

                    }

                    var path = Helpers.Common.GetSetting("productgroups-image-folder");
                    var filepath = System.Web.HttpContext.Current.Server.MapPath("~" + path);
                    if (Request.Files["file-image"] != null)
                    {
                        var file = Request.Files["file-image"];
                        if (file.ContentLength > 0)
                        {
                            //FileInfo fi = new FileInfo(Server.MapPath("~" + path) + dm_nhomsanpham.BANNER);
                            //if (fi.Exists)
                            //{
                            //    fi.Delete();
                            //}

                            string image_name = "productgroups_" + Helpers.Common.ChuyenThanhKhongDau(Regex.Replace(dm_nhomsanpham.NHOMSANPHAM_ID.ToString(), @"\s+", "_")) + file.FileName;

                            bool isExists = System.IO.Directory.Exists(filepath);
                            if (!isExists)
                                System.IO.Directory.CreateDirectory(filepath);
                            file.SaveAs(filepath + image_name);
                            dm_nhomsanpham.BANNER = image_name;
                        }
                    }







                    //begin up hinh anh cho client
                    path = Helpers.Common.GetSetting( "productgroups-image-folder-client");
                    string prjClient = Helpers.Common.GetSetting( "prj-client");
                    filepath = System.Web.HttpContext.Current.Server.MapPath("~");
                    DirectoryInfo di = new DirectoryInfo(filepath);
                    filepath = di.Parent.FullName + "\\" + prjClient + path.Replace("/", @"\");
                    if (Request.Files["file-image"] != null)
                    {
                        var file = Request.Files["file-image"];
                        if (file.ContentLength > 0)
                        {
                            //FileInfo fi = new FileInfo(Server.MapPath("~" + path) + dm_nhomsanpham.BANNER);
                            //if (fi.Exists)
                            //{
                            //    fi.Delete();
                            //}

                            string image_name = "productgroups_" + Helpers.Common.ChuyenThanhKhongDau(Regex.Replace(dm_nhomsanpham.NHOMSANPHAM_ID.ToString(), @"\s+", "_")) + file.FileName;

                            bool isExists = System.IO.Directory.Exists(filepath);
                            if (!isExists)
                                System.IO.Directory.CreateDirectory(filepath);
                            file.SaveAs(filepath + image_name);
                            dm_nhomsanpham.BANNER = image_name;
                        }
                    }
                    //end up hinh anh cho client


                    dm_nhomsanphamRepository.UpdateDM_NHOMSANPHAM(dm_nhomsanpham);
                    Erp.BackOffice.Controllers.HomeController.WriteLog(dm_nhomsanpham.NHOMSANPHAM_ID, dm_nhomsanpham.TEN_NHOMSANPHAM, "đã cập nhật sản phẩm", "DM_BannerSlider/Edit/" + dm_nhomsanpham.NHOMSANPHAM_ID, Helpers.Common.CurrentUser.BranchId.Value);
                    TempData[Globals.SuccessMessageKey] = App_GlobalResources.Wording.UpdateSuccess;
                    return RedirectToAction("DM_NhomSanPham");
                    #endregion
                }
                Helpers.Common.ResetCacheYHL("SearchSpa_GroupDetails,alllsproduct_GroupDetails,lineService_GroupDetails,lineproduct_GroupDetails,MainPartial_Mainkissy");
            }
            TempData[Globals.FailedMessageKey] = "Có lỗi khi tạo mới";
            return RedirectToAction("DM_NhomSanPham");
        }

        [HttpPost]
        public ActionResult DeleteNhomSanPham()
        {
           // string pMA_DVIQLY = Helpers.Common.CurrentUser.MA_DVIQLY;
            int id = int.Parse(Request["Id"], CultureInfo.InvariantCulture);
            dm_nhomsanphamRepository.DeleteDM_NHOMSANPHAM( id);

            var subList = dm_nhomsanphamRepository.GetAllDM_NHOMSANPHAMByNHOM_CHA(id).ToList();

            foreach (var item in subList)
            {
                var subList2 = dm_nhomsanphamRepository.GetAllDM_NHOMSANPHAMByNHOM_CHA( item.NHOMSANPHAM_ID).ToList();
                foreach (var item2 in subList2)
                {
                    dm_nhomsanphamRepository.DeleteDM_NHOMSANPHAM(item2.NHOMSANPHAM_ID);
                }
                dm_nhomsanphamRepository.DeleteDM_NHOMSANPHAM( item.NHOMSANPHAM_ID);
            }
            Helpers.Common.ResetCacheYHL("SearchSpa_GroupDetails,alllsproduct_GroupDetails,lineService_GroupDetails,lineproduct_GroupDetails,MainPartial_Mainkissy");


            TempData["AlertMessage"] = App_GlobalResources.Wording.DeleteSuccess;
            return RedirectToAction("DM_NhomSanPham");
        }


        public JsonResult getDecription(int Id)
        {
            var dm_nhomsanpham = dm_nhomsanphamRepository.GetDM_NHOMSANPHAMByNHOMSANPHAM_ID(Id);
            return Json(dm_nhomsanpham.Decription, JsonRequestBehavior.AllowGet);
        }
        #endregion



        #region LoadProductItem
        public JsonResult SearchProductJson()
        {
            //string pMA_DVIQLY = Helpers.Common.CurrentUser.MA_DVIQLY;


            var productList = productRepository.GetAllProduct().Where(x => x.NotUse != true && x.Type =="product")
             .Select(item => new ProductViewModel
             {

                 Code = item.Code,

                 Name = item.Name,
                 //ProductId_KiotViet = item.ProductId_KiotViet,
                 Id = item.Id,
                 //IS_NGUNG_KD = item.IS_NGUNG_KD

             });  //.Where(x => x.IS_NGUNG_KD == false);
            var pro = productList.ToList();

            return Json(productList.ToList().Select(i => new { id = i.Id, text = i.Code + " - " + i.Name, value = i.Id, product_id = i.Id, code = i.Code, ProductName = i.Name }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult SearchServiceJson()
        {
            //string pMA_DVIQLY = Helpers.Common.CurrentUser.MA_DVIQLY;


            var productList = productRepository.GetAllProduct().Where(x => x.NotUse != true && x.Type == "service")
             .Select(item => new ProductViewModel
             {

                 Code = item.Code,

                 Name = item.Name,
                 //ProductId_KiotViet = item.ProductId_KiotViet,
                 Id = item.Id,
                 //IS_NGUNG_KD = item.IS_NGUNG_KD

             });  //.Where(x => x.IS_NGUNG_KD == false);
            var pro = productList.ToList();

            return Json(productList.ToList().Select(i => new { id = i.Id, text = i.Code + " - " + i.Name, value = i.Id, product_id = i.Id, code = i.Code, ProductName = i.Name }), JsonRequestBehavior.AllowGet);
        }

        #region LoadProductItem
        public PartialViewResult LoadProductItem2(int OrderNo, int ProductId, string ProductName, string ProductCode, int ProductIdCha)
        {
            var productskincare = new ProductInSkinCare();
            productskincare.IsDeleted = false;
            productskincare.Name = "";
            productskincare.OrderNo = OrderNo + 1;
            productskincare.ProductId = ProductIdCha;
            productskincare.ProductIdMem = ProductId;
            ProductInSkinCareRepository.InsertProductInSkinCare(productskincare);


            // string pMA_DVIQLY = Helpers.Common.CurrentUser.MA_DVIQLY;
            var model = new ProductInSkinCareViewModel();
            model.ProductName = ProductName;
            model.OrderNo = productskincare.OrderNo;
            model.ProductCode = ProductCode;
            model.ProductIdMem = productskincare.ProductIdMem;
            model.Id = productskincare.Id;
            model.Name = productskincare.Name;

            return PartialView(model);
        }
        #endregion


        [HttpPost]
        public JsonResult SaveUpload(int Id, string Name)
        {
            var pro = ProductInSkinCareRepository.GetProductInSkinCareById(Id);
            if (pro != null)
            {
                pro.Name = Name;
                ProductInSkinCareRepository.UpdateProductInSkinCare(pro);
                return Json("success");
            }
            else
            {
                return Json("error");
            }

        }

        [HttpPost]
        public JsonResult DeleteProductInSkinCare(int Id)
        {
            ///string pMA_DVIQLY = Helpers.Common.CurrentUser.MA_DVIQLY;
            string json = "";
            var pro = ProductInSkinCareRepository.GetProductInSkinCareById(Id);
            var ProductIdCha = pro.ProductId;
            if (pro != null)
            {
                var getpro = ProductInSkinCareRepository.GetAllProductInSkinCareByProductId(pro.ProductId).Where(x => x.OrderNo > pro.OrderNo).ToList();
                foreach (var item in getpro)
                {
                    item.OrderNo = item.OrderNo - 1;
                    ProductInSkinCareRepository.UpdateProductInSkinCare(item);
                }

                ProductInSkinCareRepository.DeleteProductInSkinCare(pro.Id);

                var productSkincare = ProductInSkinCareRepository.GetAllProductInSkinCareByProductId(ProductIdCha).Select(
                    item => new ProductInSkinCareViewModel
                    {
                        Id = item.Id,
                        Name = item.Name,
                        ProductId = item.ProductId,
                        ProductIdMem = item.ProductIdMem,
                        IsDeleted = item.IsDeleted,
                        OrderNo = item.OrderNo
                    }
                    ).ToList();


                if (productSkincare != null)
                {
                    foreach (var i in productSkincare)
                    {
                        var _pro = productRepository.GetProductById(i.ProductIdMem);
                        if (_pro != null)
                        {
                            i.ProductName = _pro.Name;
                            i.ProductCode = _pro.Code;
                        }
                    }
                }
                foreach (var i in productSkincare)
                {
                    json += "<tr role=\"role_(" + i.OrderNo + ")\" id=\"product_item_" + i.OrderNo + "\" data-id=\"" + i.OrderNo + "\">";

                    json += "<td class=\"text-center\"> <span>" + i.OrderNo + "</span> </td>";

                    json += "<td class=\"detail_item_id\">";
                    json += "<input id=\"DetailList_" + i.OrderNo + "__Id\" name=\"DetailList[" + i.OrderNo + "].Id\" type=\"hidden\" value=\"" + i.Id + "\">";

                    json += "<input id=\"DetailList_" + i.OrderNo + "__ProductId\" name=\"DetailList[" + i.OrderNo + "].ProductId\" type=\"hidden\" value=\"" + i.ProductId + "\">";

                    json += "<input id=\"DetailList_" + i.OrderNo + "__ProductIdMem\" name=\"DetailList[" + i.OrderNo + "].ProductIdMem\" type=\"hidden\" value=\"" + i.ProductIdMem + "\">";
                    json += i.ProductCode + "-" + i.ProductName;
                    json += "</td>";

                    json += "<td class=\"has-error\">";
                    json += " <input type=\"text\" style=\"width:100%\" value=\" " + i.Name + "\" class=\"noidung\" name=\"DetailList[" + i.OrderNo + "].Name\" id=\"DetailList_" + i.OrderNo + "_Name\" autocomplete=\"off\" aria-invalid=\"false\">";
                    json += "</td>";

                    json += "<td class=\"text-center\"><a class=\"btn-delete-item\"><i class=\"ace-icon fa fa-trash red bigger-120\" style=\"cursor:pointer\"></i></a></td>";

                    json += "</tr>";
                }

                return Json(json);
            }
            return Json("error");
        }
        #endregion



        public static string to_slug(string str)
        {
            var regex = new Regex("[^a-zA-Z_0-9 ]");
            str = str.Trim();
            str = Helpers.Common.ChuyenThanhKhongDau(str);
            str = regex.Replace(str, "");
            str = str.Replace(" ", "-");
            // return
            return str;
        }

       
    }
}
