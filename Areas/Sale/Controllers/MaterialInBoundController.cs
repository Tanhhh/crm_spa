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
using Erp.BackOffice.App_GlobalResources;
using Erp.BackOffice.Account.Models;
using Erp.BackOffice.Account.Controllers;
using Erp.Domain.Account.Entities;
using Erp.Domain.Account.Interfaces;
using Erp.Domain.Sale.Repositories;
using Erp.Domain.Staff.Interfaces;
using GenCode128;
using System.Drawing;
using System.IO;
using System.Transactions;
using Erp.Domain.Account.Helper;
using Erp.BackOffice.Areas.Cms.Models;
using System.Web;
using ClosedXML.Excel;
using System.Data;

namespace Erp.BackOffice.Sale.Controllers
{
    [Authorize]
    [InitializeSimpleMembership]
    [Erp.BackOffice.Helpers.NoCacheHelper]
    public class MaterialInboundController : Controller
    {
        private readonly ITransactionRepository transactionRepository;
        private readonly IWarehouseRepository WarehouseRepository;
        private readonly IWarehouseLocationItemRepository WarehouseLocationItemRepository;
        private readonly IInventoryMaterialRepository InventoryMaterialRepository;

        private readonly IMaterialOrServiceRepository MaterialRepository;
        private readonly IMaterialInboundRepository MaterialInboundRepository;
        private readonly IMaterialOutboundRepository MaterialOutboundRepository;
        private readonly ISupplierRepository SupplierRepository;
        private readonly IUserRepository userRepository;
        private readonly IPaymentRepository paymentRepository;
        private readonly IQueryHelper QueryHelper;
        private readonly ITemplatePrintRepository templatePrintRepository;
        private readonly IRequestInboundRepository requestInboundRepository;
        private readonly IProductDamagedRepository productDamagesRepository;
        private readonly IBranchRepository branchRepository;
        private readonly ISalesReturnsRepository saleReturnRepository;
        private readonly ICategoryRepository categoryRepository;
        public MaterialInboundController(
               ITransactionRepository _transaction
            , IWarehouseRepository _Warehouse
            , IWarehouseLocationItemRepository _WarehouseLocationItem
            , IInventoryMaterialRepository _InventoryMaterial

            , IMaterialOrServiceRepository _Material
            , IMaterialInboundRepository _MaterialInbound
            , IMaterialOutboundRepository _MaterialOutbound
            , ISupplierRepository _Supplier
            , IUserRepository _user
            , IQueryHelper _QueryHelper
            , IPaymentRepository payment
            , ITemplatePrintRepository _templatePrint
            , IRequestInboundRepository requestInbound
            , IProductDamagedRepository productdamages
               , IBranchRepository branch
            , ISalesReturnsRepository saleReturn
            ,ICategoryRepository _category
            )
        {
            transactionRepository = _transaction;
            WarehouseRepository = _Warehouse;
            WarehouseLocationItemRepository = _WarehouseLocationItem;
            InventoryMaterialRepository = _InventoryMaterial;

            MaterialRepository = _Material;
            MaterialInboundRepository = _MaterialInbound;
            MaterialOutboundRepository = _MaterialOutbound;
            SupplierRepository = _Supplier;
            userRepository = _user;
            QueryHelper = _QueryHelper;
            paymentRepository = payment;
            templatePrintRepository = _templatePrint;
            requestInboundRepository = requestInbound;
            productDamagesRepository = productdamages;
            branchRepository = branch;
            saleReturnRepository = saleReturn;
            categoryRepository = _category;
        }

        #region Index
        public ViewResult Index(string txtCode, string txtMinAmount, string txtMaxAmount, int? warehouseDestinationId, string txtWarehouseSource, string startDate, string endDate, string txtMaterialCode, int? supplierId, string Status)
        {
            var warehouseList = WarehouseRepository.GetvwAllWarehouse().AsEnumerable();
            var user = userRepository.GetByvwUserName(Helpers.Common.CurrentUser.UserName);
            IEnumerable<MaterialInboundViewModel> q = MaterialInboundRepository.GetAllvwMaterialInboundFull()
                //.Where(x => x.BranchId == Helpers.Common.CurrentUser.BranchId)
                .Select(item => new MaterialInboundViewModel
                {
                    Id = item.Id,
                    CreatedUserId = item.CreatedUserId,
                    CreatedDate = item.CreatedDate,
                    ModifiedUserId = item.ModifiedUserId,
                    ModifiedDate = item.ModifiedDate,
                    Code = item.Code,
                    TotalAmount = item.TotalAmount,
                    WarehouseDestinationName = item.WarehouseDestinationName,
                    IsDone = item.IsDone,
                    //PurchaseOrderId = item.PurchaseOrderId,
                    //SupplierId = item.SupplierId,
                    //SupplierName = item.SupplierName,
                    WarehouseDestinationId = item.WarehouseDestinationId,
                    Note = item.Note,
                    IsArchive = item.IsArchive,
                    IsDeleted = item.IsDeleted,
                    BranchId = item.BranchId
                }).OrderByDescending(m => m.Id);

            //Tìm những phiếu nhập có chứa mã sp
            if (!string.IsNullOrEmpty(txtCode))
            {


                q = q.Where(x => x.Code.ToLowerOrEmpty().Contains(txtCode.Trim().ToLower())
                 //|| x.WarehouseDestinationId == warehouseDestinationId
                 );
            }
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
            

            if (intBrandID > 0)
            {
                q = q.Where(x => x.BranchId == intBrandID);
            }

            if (warehouseDestinationId != null && warehouseDestinationId.Value > 0)
            {
                q = q.Where(x => x.WarehouseDestinationId == warehouseDestinationId);
            }
            if (supplierId != null && supplierId.Value > 0)
            {
                q = q.Where(x => x.SupplierId == supplierId);
            }
            // lọc theo ngày
            DateTime d_startDate, d_endDate;
            if (DateTime.TryParseExact(startDate, "dd/MM/yyyy", new CultureInfo("vi-VN"), DateTimeStyles.None, out d_startDate))
            {
                if (DateTime.TryParseExact(endDate, "dd/MM/yyyy", new CultureInfo("vi-VN"), DateTimeStyles.None, out d_endDate))
                {
                    d_endDate = d_endDate.AddHours(23).AddMinutes(59);
                    q = q.Where(x => x.CreatedDate >= d_startDate && x.CreatedDate <= d_endDate);
                }
            }

            decimal minAmount;
            if (decimal.TryParse(txtMinAmount, out minAmount))
            {
                q = q.Where(x => x.TotalAmount >= minAmount);
            }

            decimal maxAmount;
            if (decimal.TryParse(txtMaxAmount, out maxAmount))
            {
                if (maxAmount > 0)
                {
                    q = q.Where(x => x.TotalAmount <= maxAmount);
                }
            }
            if (!string.IsNullOrEmpty(Status))
            {
                if (Status == "complete")
                {
                    q = q.Where(x => x.IsArchive == true);
                }
                else if (Status == "pending")
                {
                    q = q.Where(x => x.IsArchive == false);
                }
                else if (Status == "delete")
                {
                    q = q.Where(x => x.IsDeleted == true);
                }
            }

            warehouseList = warehouseList.Where(x => x.Categories == "VT" && x.BranchId == intBrandID).ToList();
            var _warehouseList = warehouseList.Select(item => new SelectListItem
            {
                Text = item.Name + " (" + item.BranchName + ")",
                Value = item.Id.ToString()
            });
            ViewBag.warehouseList = _warehouseList;
            ViewBag.SuccessMessage = TempData["SuccessMessage"];
            ViewBag.FailedMessage = TempData["FailedMessage"];
            ViewBag.AlertMessage = TempData["AlertMessage"];
            TempData["DataMaterialInBound"] = q.ToList();
            return View(q);
        }
        #endregion

        #region LoadMaterialItem
        public PartialViewResult LoadMaterialItem(int OrderNo, int MaterialId, string MaterialName, string Unit, int Quantity, decimal Price, string MaterialCode, string MaterialType, string LoCode, string ExpiryDate)
        {
            var model = new MaterialInboundDetailViewModel();
            model.OrderNo = OrderNo;
            model.MaterialId = MaterialId;
            model.MaterialName = MaterialName;
            model.Unit = Unit;
            model.Quantity = Quantity;
            model.Price = Price;
            model.MaterialCode = MaterialCode;
            model.MaterialType = MaterialType;
            model.LoCode = LoCode;
            if (!string.IsNullOrEmpty(ExpiryDate))
                model.ExpiryDate = Convert.ToDateTime(ExpiryDate);
            return PartialView(model);
        }
        #endregion

        #region Create
        public ActionResult Create()
        {
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
            //var categoryList = categoryRepository.GetAllCategories()
            //  .Select(item => new WarehouseViewModel
            //  {
            //      Code = item.Code,
            //      Value = item.Value
            //  }).Where(x=>x.Code== "Categories_product");
            //ViewBag.categoryList = categoryList;

            var categoryList = SqlHelper.QuerySP<CategoryViewModel>("GetAllDanhMucKho").Where(x => x.CategoryValue == "VT").ToList();
            if(intBrandID > 0)
            {
                categoryList = categoryList.Where(x => x.BranchId == intBrandID).ToList();
            }
            ViewBag.categoryList = categoryList;
            var model = new MaterialInboundViewModel();
            model.DetailList = new List<MaterialInboundDetailViewModel>();
            model.CreatedDate = DateTime.Now;
            model.CreatedUserName = Helpers.Common.CurrentUser.FullName;
            model.PaymentViewModel = new PaymentViewModel();
            model.NextPaymentDate = DateTime.Now.AddDays(30);
            model.Code = Erp.BackOffice.Helpers.Common.GetOrderNo("MaterialInbound");
            return View(model);
        }

        [HttpPost]
        public ActionResult Create(MaterialInboundViewModel model)
        {
            MaterialInbound materialInbound = null;

            if (ModelState.IsValid)
            {
                var MaterialInbound = new Domain.Sale.Entities.MaterialInbound();
                using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.Snapshot }))
                {
                    try
                    {
                        AutoMapper.Mapper.Map(model, MaterialInbound);
                        MaterialInbound.IsDeleted = false;
                        MaterialInbound.CreatedUserId = WebSecurity.CurrentUserId;
                        MaterialInbound.ModifiedUserId = WebSecurity.CurrentUserId;
                        MaterialInbound.CreatedDate = DateTime.Now;
                        MaterialInbound.ModifiedDate = DateTime.Now;
                        var branch = WarehouseRepository.GetWarehouseById(MaterialInbound.WarehouseDestinationId.Value);
                        MaterialInbound.BranchId = branch.BranchId;

                        //duyệt qua danh sách sản phẩm mới xử lý tình huống user chọn 2 sản phầm cùng id
                        List<Domain.Sale.Entities.MaterialInboundDetail> listNewCheckSameId = new List<Domain.Sale.Entities.MaterialInboundDetail>();
                        foreach (var group in model.DetailList.Where(x=>x.MaterialId>0))
                        {
                            listNewCheckSameId.Add(new Domain.Sale.Entities.MaterialInboundDetail
                            {
                                MaterialId = group.MaterialId,
                                Quantity = group.Quantity,
                                Unit = group.Unit,
                                Price = group.Price,
                                IsDeleted = false,
                                CreatedUserId = WebSecurity.CurrentUserId,
                                ModifiedUserId = WebSecurity.CurrentUserId,
                                CreatedDate = DateTime.Now,
                                ModifiedDate = DateTime.Now,
                                ExpiryDate = group.ExpiryDate,
                                LoCode = group.LoCode
                            });
                        }

                        MaterialInbound.TotalAmount = listNewCheckSameId.Sum(x => x.Price * (decimal)x.Quantity);

                        ////lấy thông tin đơn đặt hàng nếu có chọn          
                        MaterialInbound.Type = "internal";
                        //thêm mới phiếu nhập và chi tiết phiếu nhập
                        MaterialInboundRepository.InsertMaterialInbound(MaterialInbound);
                        //cập nhật thông tin đơn nhập hàng nếu có chọn      
                        //cập nhật lại mã nhập kho

                        MaterialInbound.Code = Erp.BackOffice.Helpers.Common.GetOrderNo("MaterialInbound", model.Code);
                        MaterialInboundRepository.UpdateMaterialInbound(MaterialInbound);
                        Erp.BackOffice.Helpers.Common.SetOrderNo("MaterialInbound");

                        //ghi log 
                        Erp.BackOffice.Controllers.HomeController.WriteLog(MaterialInbound.Id, MaterialInbound.Code, "đã tạo phiếu nhập vật tư", "MaterialInbound/Detail/" + MaterialInbound.Id, Helpers.Common.CurrentUser.BranchId.Value);

                        //Thêm chi tiết phiếu nhập
                        foreach (var item in listNewCheckSameId)
                        {
                            item.MaterialInboundId = MaterialInbound.Id;
                            MaterialInboundRepository.InsertMaterialInboundDetail(item);
                        }

                        //Thêm vào quản lý chứng từ
                        TransactionController.Create(new TransactionViewModel
                        {
                            TransactionModule = "MaterialInBound",
                            TransactionCode = MaterialInbound.Code,
                            TransactionName = "Nhập kho vật tư"
                        });
                        scope.Complete();
                    }
                    catch (DbUpdateException)
                    {
                        TempData[Globals.FailedMessageKey] = App_GlobalResources.Wording.Error;
                        return View(model);
                    }
                }
                TempData[Globals.SuccessMessageKey] = App_GlobalResources.Wording.InsertSuccess;
                return RedirectToAction("Index", new { Id = MaterialInbound.Id });
            }
            return View(model);
        }
        #endregion

        #region Eidt
        public ActionResult Edit(int? Id, string TransactionCode)
        {
            var MaterialInbound = new vwMaterialInbound();
            if (Id != null)
                MaterialInbound = MaterialInboundRepository.GetvwMaterialInboundById(Id.Value);
            if (!string.IsNullOrEmpty(TransactionCode))
                MaterialInbound = MaterialInboundRepository.GetvwMaterialInboundByTransactionCode(TransactionCode);

            if (MaterialInbound != null && MaterialInbound.IsDeleted != true)
            {
                //Nếu đã ghi sổ rồi thì không được sửa
                if (MaterialInbound.IsArchive)
                {
                    return RedirectToAction("Detail", new { Id = MaterialInbound.Id });
                }

                var model = new MaterialInboundViewModel();
                model.DetailList = new List<MaterialInboundDetailViewModel>();
                AutoMapper.Mapper.Map(MaterialInbound, model);

                // lấy danh sách detail
                model.DetailList = MaterialInboundRepository.GetAllvwMaterialInboundDetailByInboundId(MaterialInbound.Id)
                    .Select(x => new MaterialInboundDetailViewModel
                    {
                        Id = x.Id,
                        Price = x.Price,
                        MaterialId = x.MaterialId,
                        MaterialInboundId = x.MaterialInboundId,
                        Quantity = x.Quantity,
                        MaterialCode = x.MaterialCode,
                        MaterialName = x.MaterialName,
                        ExpiryDate = x.ExpiryDate,
                        LoCode = x.LoCode,
                        Unit=x.Unit
                    }).ToList();
                model.CreatedUserName = userRepository.GetUserById(model.CreatedUserId.Value).FullName;
                return View(model);
            }

            return View();
        }

        [HttpPost]
        public ActionResult Edit(MaterialInboundViewModel model)
        {

            var materialInbound = MaterialInboundRepository.GetMaterialInboundById(model.Id);
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.Snapshot }))
            {
                try
                {

                    var _listdata = MaterialInboundRepository.GetAllMaterialInboundDetailByInboundId(materialInbound.Id).ToList();
                    if (model.DetailList.Any(x => x.Id == 0))
                    {
                        //lưu danh sách thao tác thực hiện dịch vụ
                        foreach (var item in model.DetailList.Where(x => x.Id == 0 && x.MaterialId>0))
                        {
                            var ins = new MaterialInboundDetail();
                            ins.IsDeleted = false;
                            ins.CreatedUserId = WebSecurity.CurrentUserId;
                            ins.ModifiedUserId = WebSecurity.CurrentUserId;
                            ins.AssignedUserId = WebSecurity.CurrentUserId;
                            ins.CreatedDate = DateTime.Now;
                            ins.ModifiedDate = DateTime.Now;
                            ins.ExpiryDate = item.ExpiryDate;
                            ins.LoCode = item.LoCode;
                            ins.MaterialId = item.MaterialId;
                            ins.MaterialInboundId = materialInbound.Id;
                            ins.Price = item.Price;
                            ins.Quantity = item.Quantity;
                            ins.Unit = item.Unit;
                            MaterialInboundRepository.InsertMaterialInboundDetail(ins);
                        }
                    }
                    var _delete = _listdata.Where(id1 => !model.DetailList.ToList().Any(id2 => id2.Id == id1.Id)).ToList();
                    if (_delete.Any())
                    {
                        foreach (var item in _delete)
                        {
                            MaterialInboundRepository.DeleteMaterialInboundDetail(item.Id);
                        }
                    }
                    if (model.DetailList.Any(x => x.Id > 0))
                    {
                        var update = _listdata.Where(id1 => model.DetailList.ToList().Any(id2 => id2.Id == id1.Id)).ToList();
                        //lưu danh sách thao tác thực hiện dịch vụ
                        foreach (var item in model.DetailList.Where(x => x.Id > 0 && x.MaterialId>0))
                        {
                            var _update = update.FirstOrDefault(x => x.Id == item.Id);
                            _update.ExpiryDate = item.ExpiryDate;
                            _update.LoCode = item.LoCode;
                            _update.MaterialId = item.MaterialId;
                            _update.MaterialInboundId = materialInbound.Id;
                            _update.Price = item.Price;
                            _update.Quantity = item.Quantity;
                            _update.Unit = item.Unit;
                            MaterialInboundRepository.UpdateMaterialInboundDetail(_update);
                        }
                    }
                    materialInbound.Note = model.Note;
                    materialInbound.TotalAmount = model.TotalAmount;
                    materialInbound.ModifiedUserId = Helpers.Common.CurrentUser.Id;
                    materialInbound.ModifiedDate =DateTime.Now;
                    MaterialInboundRepository.UpdateMaterialInbound(materialInbound);
                    //ghi log 
                    Erp.BackOffice.Controllers.HomeController.WriteLog(materialInbound.Id, materialInbound.Code, "đã cập nhật phiếu nhập vật tư", "MaterialInbound/Detail/" + materialInbound.Id, Helpers.Common.CurrentUser.BranchId.Value);

                    scope.Complete();
                }
                catch (DbUpdateException)
                {
                    TempData[Globals.FailedMessageKey] = App_GlobalResources.Wording.Error;
                    return View(model);
                }
            }
            return RedirectToAction("Detail", new { Id = model.Id });
        }
        #endregion

        #region Detail
        public ActionResult Detail(int? Id, string TransactionCode)
        {
            var MaterialInbound = new vwMaterialInbound();
            if (Id != null)
                MaterialInbound = MaterialInboundRepository.GetvwMaterialInboundFullById(Id.Value);
            if (!string.IsNullOrEmpty(TransactionCode))
                MaterialInbound = MaterialInboundRepository.GetvwMaterialInboundByTransactionCode(TransactionCode);

            if (MaterialInbound != null)
            {
                var model = new MaterialInboundViewModel();

                AutoMapper.Mapper.Map(MaterialInbound, model);
                //Kiểm tra cho phép sửa chứng từ này hay không
                model.AllowEdit = Helpers.Common.KiemTraNgaySuaChungTu(model.CreatedDate.Value);

                // lấy danh sách detail
                var Details = MaterialInboundRepository.GetAllvwMaterialInboundDetailByInboundId(MaterialInbound.Id)
                    .Select(x => new MaterialInboundDetailViewModel
                    {
                        Id = x.Id,
                        Price = x.Price,
                        MaterialId = x.MaterialId,
                        MaterialInboundId = x.MaterialInboundId,
                        Quantity = x.Quantity,
                        MaterialCode = "",
                        ExpiryDate = x.ExpiryDate,
                        LoCode = x.LoCode,
                    }).OrderBy(x => x.Id).ToList();
                model.DetailList = Details;

                foreach (var item in Details)
                {
                    var material = MaterialRepository.GetMaterialById(item.MaterialId.Value);
                    item.MaterialName = material != null ? material.Name : "";
                    item.MaterialCode = material != null ? material.Code : "";
                    //var usedQuantity = WarehouseLocationItemRepository.GetAllLocationItem().Where(x => x.ProductId == item.MaterialId && x.ProductInboundId == item.ProductInboundId && x.IsOut == true).ToList().Count();
                    //item.QuantityUsed = usedQuantity;
                }

                var username = userRepository.GetUserById(model.CreatedUserId.Value);
                if (username != null)
                {
                    model.CreatedUserName = username.FullName;
                }
                ViewBag.SuccessMessage = TempData["SuccessMessage"];
                ViewBag.FailedMessage = TempData["FailedMessage"];
                return View(model);
            }

            return View();
        }
        #endregion

        #region Archive
        [HttpPost]
        public ActionResult Archive(int Id, bool? IsPopup)
        {
            if (Request["Submit"] == "Save")
            {
                var materialInbound = MaterialInboundRepository.GetMaterialInboundById(Id);

                //Kiểm tra cho phép sửa chứng từ này hay không
                if (Helpers.Common.KiemTraNgaySuaChungTu(materialInbound.CreatedDate.Value) == false)
                {
                    return RedirectToAction("Detail", new { Id = materialInbound.Id });
                }
                using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.Snapshot }))
                {
                    try
                    {
                        Archive(materialInbound);
                        TempData[Globals.SuccessMessageKey] = App_GlobalResources.Wording.ArchiveSuccess;
                        //ghi log 
                        Erp.BackOffice.Controllers.HomeController.WriteLog(materialInbound.Id, materialInbound.Code, "đã ghi sổ phiếu nhập vật tư", "MaterialInbound/Detail/" + materialInbound.Id, Helpers.Common.CurrentUser.BranchId.Value);


                        var request = requestInboundRepository.GetAllRequestInbound().Where(x => x.InboundId == materialInbound.Id);
                        if (request.Count() > 0)
                        {
                            var item = request.FirstOrDefault();

                            //cập nhật lại yêu cầu nhập kho
                            item.Status = "inbound_complete";
                            requestInboundRepository.UpdateRequestInbound(item);
                            var qq = MaterialOutboundRepository.GetMaterialOutboundById(item.OutboundId.Value);
                            //gửi notifications cho người được phân quyền.
                            Crm.Controllers.ProcessController.Run("RequestInbound", "inbound_complete", item.Id, qq.CreatedUserId, null, item);
                        }
                        scope.Complete();
                    }
                    catch (DbUpdateException)
                    {
                        TempData[Globals.FailedMessageKey] = App_GlobalResources.Wording.Error;
                    }
                }
            }
            if (IsPopup != null && IsPopup == true)
            {
                return RedirectToAction("_ClosePopup", "Home", new { area = "", FunctionCallback = "ClosePopupAndReloadPage" });
            }
            else
            {
                return RedirectToAction("Detail", new { Id = Id });
            }

        }

        public static void Archive(MaterialInbound materialInbound)
        {
            MaterialInboundRepository MaterialInboundRepository = new Domain.Sale.Repositories.MaterialInboundRepository(new Domain.Sale.ErpSaleDbContext());
            //Cập nhật lại tồn kho cho những sp trong phiếu nhập này
            var detailList = MaterialInboundRepository.GetAllvwMaterialInboundDetailByInboundId(materialInbound.Id)
                .Select(item => new
                {
                    MaterialName = item.MaterialCode + " - " + item.MaterialName,
                    MaterialId = item.MaterialId.Value,
                    Quantity = item.Quantity.Value,
                    LoCode = item.LoCode,
                    ExpiryDate = item.ExpiryDate
                }).ToList();

            //Kiểm tra dữ liệu sau khi bỏ ghi sổ có hợp lệ không
            string check = "";
            foreach (var item in detailList.Where(x => x.Quantity >= 0))
            {
                var error = InventoryMaterialController.Check(item.MaterialName, item.MaterialId, item.LoCode, item.ExpiryDate, materialInbound.WarehouseDestinationId.Value, item.Quantity, 0);
                check += error;
            }

            if (string.IsNullOrEmpty(check))
            {
                //Khi đã hợp lệ thì mới update
                foreach (var item in detailList.Where(x => x.Quantity >= 0))
                {
                    InventoryMaterialController.Update(item.MaterialName, item.MaterialId, item.LoCode, item.ExpiryDate, materialInbound.WarehouseDestinationId.Value, item.Quantity, 0);
                }

                materialInbound.IsArchive = true;
                MaterialInboundRepository.UpdateMaterialInbound(materialInbound);
            }
            //else
            //{
            //    TempData[Globals.FailedMessageKey] = App_GlobalResources.Wording.ArchiveFail + check;
            //}
        }
        #endregion

        #region UnArchive
        [HttpPost]
        public ActionResult UnArchive(int Id, bool? IsPopup)
        {
            if (Request["Submit"] == "Save")
            {
                var materialInbound = MaterialInboundRepository.GetMaterialInboundById(Id);

                //Kiểm tra cho phép sửa chứng từ này hay không
                if (Helpers.Common.KiemTraNgaySuaChungTu(materialInbound.CreatedDate.Value) == false)
                {
                    return RedirectToAction("Detail", new { Id = materialInbound.Id });
                }
                using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.Snapshot }))
                {
                    try
                    {
                        //Cập nhật lại tồn kho cho những sp trong phiếu nhập này
                        var detailList = MaterialInboundRepository.GetAllvwMaterialInboundDetailByInboundId(materialInbound.Id)
                            .Select(item => new
                            {
                                MaterialName = item.MaterialCode + " - " + item.MaterialName,
                                MaterialId = item.MaterialId.Value,
                                Quantity = item.Quantity.Value,
                                LoCode = item.LoCode,
                                ExpiryDate = item.ExpiryDate
                            }).ToList();

                        //Kiểm tra dữ liệu sau khi bỏ ghi sổ có hợp lệ không
                        string check = "";
                        foreach (var item in detailList.Where(x => x.Quantity > 0))
                        {
                            var error = InventoryMaterialController.Check(item.MaterialName, item.MaterialId, item.LoCode, item.ExpiryDate, materialInbound.WarehouseDestinationId.Value, 0, item.Quantity);
                            check += error;
                        }

                        if (string.IsNullOrEmpty(check))
                        {
                            //Khi đã hợp lệ thì mới update
                            foreach (var item in detailList.Where(x => x.Quantity > 0))
                            {
                                InventoryMaterialController.Update(item.MaterialName, item.MaterialId, item.LoCode, item.ExpiryDate, materialInbound.WarehouseDestinationId.Value, 0, item.Quantity);
                            }

                            materialInbound.IsArchive = false;
                            MaterialInboundRepository.UpdateMaterialInbound(materialInbound);
                            TempData[Globals.SuccessMessageKey] = "Đã bỏ ghi sổ";
                            //ghi log 
                            Erp.BackOffice.Controllers.HomeController.WriteLog(materialInbound.Id, materialInbound.Code, "đã bỏ ghi sổ phiếu nhập vật tư", "MaterialInbound/Detail/" + materialInbound.Id, Helpers.Common.CurrentUser.BranchId.Value);

                        }
                        else
                        {
                            TempData[Globals.FailedMessageKey] = App_GlobalResources.Wording.ArchiveFail + check;
                        }
                        scope.Complete();
                    }
                    catch (DbUpdateException)
                    {
                        return Content("Fail");
                    }
                }
            }
            if (IsPopup != null && IsPopup == true)
            {
                return RedirectToAction("_ClosePopup", "Home", new { area = "", FunctionCallback = "ClosePopupAndReloadPage" });
            }
            else
            {
                return RedirectToAction("Detail", new { Id = Id });
            }
        }

        public static void UnArchiveAndDelete(MaterialInbound materialInbound)
        {
            MaterialInboundRepository MaterialInboundRepository = new Domain.Sale.Repositories.MaterialInboundRepository(new Domain.Sale.ErpSaleDbContext());
            //Cập nhật lại tồn kho cho những sp trong phiếu nhập này
            var detailList = MaterialInboundRepository.GetAllvwMaterialInboundDetailByInboundId(materialInbound.Id)
                .Select(item => new
                {
                    MaterialName = item.MaterialCode + " - " + item.MaterialName,
                    MaterialId = item.MaterialId.Value,
                    Quantity = item.Quantity.Value,
                    LoCode = item.LoCode,
                    ExpiryDate = item.ExpiryDate
                }).ToList();

            //Kiểm tra dữ liệu sau khi bỏ ghi sổ có hợp lệ không
            string check = "";
            foreach (var item in detailList.Where(x => x.Quantity > 0))
            {
                var error = InventoryMaterialController.Check(item.MaterialName, item.MaterialId, item.LoCode, item.ExpiryDate, materialInbound.WarehouseDestinationId.Value, 0, item.Quantity);
                check += error;
            }

            if (string.IsNullOrEmpty(check))
            {
                //Khi đã hợp lệ thì mới update
                foreach (var item in detailList.Where(x => x.Quantity > 0))
                {
                    InventoryMaterialController.Update(item.MaterialName, item.MaterialId, item.LoCode, item.ExpiryDate, materialInbound.WarehouseDestinationId.Value, 0, item.Quantity);
                }

                materialInbound.IsArchive = false;
                materialInbound.IsDeleted = true;
                MaterialInboundRepository.UpdateMaterialInbound(materialInbound);
                //TempData[Globals.SuccessMessageKey] = "Đã bỏ ghi sổ";
            }
            else
            {
                //TempData[Globals.FailedMessageKey] = App_GlobalResources.Wording.ArchiveFail + check;
            }
        }
        #endregion

        //#region UpdateLocationItem
        //public ActionResult UpdateLocationItem(int Id)
        //{
        //    MaterialInboundViewModel model = new MaterialInboundViewModel();
        //    var item = MaterialInboundRepository.GetAllvwMaterialInbound().Where(x => x.Id == Id).FirstOrDefault();
        //    if (item != null)
        //    {
        //        AutoMapper.Mapper.Map(item, model);
        //        model.CreatedUserName = userRepository.GetUserById(model.CreatedUserId.Value).FullName;

        //        var detailList = MaterialInboundRepository.GetAllvwMaterialInboundDetailByInboundId(item.Id);
        //        model.DetailList = detailList.Select(x => new MaterialInboundDetailViewModel
        //        {
        //            Id = x.Id,
        //            Price = x.Price,
        //            MaterialId = x.MaterialId,
        //            MaterialInboundId = x.MaterialInboundId,
        //            MaterialName = x.MaterialName,
        //            Quantity = x.Quantity,
        //            Unit = x.Unit,
        //            MaterialCode = x.MaterialCode
        //        }).ToList();

        //        //foreach (var i in model.DetailList)
        //        //{
        //        //    i.ListWarehouseLocationItemViewModel = new List<WarehouseLocationItemViewModel>();
        //        //    var listLocationItemExits = WarehouseLocationItemRepository.GetAllWarehouseLocationItem().Where(q => q.MaterialInboundDetailId == i.Id).ToList();
        //        //    AutoMapper.Mapper.Map(listLocationItemExits, i.ListWarehouseLocationItemViewModel);
        //        //}                
        //    }

        //    ViewBag.FailedMessage = TempData["FailedMessage"];
        //    ViewBag.AlertMessage = TempData["AlertMessage"];

        //    if (model.DetailList.Count == 0)
        //    {
        //        TempData[Globals.FailedMessageKey] = "Danh sách sản phẩm trong phiếu nhập [" + model.Code + "] đã được nhập vị trí đầy đủ.";
        //        return RedirectToAction("Index");
        //    }

        //    return View(model);
        //}

        //[HttpPost]
        //public ActionResult UpdateLocationItem(InboundLocationItemsViewModel model)
        //{
        //    var list = model.LocationItemList
        //        .Where(item => string.IsNullOrEmpty(item.Shelf)
        //            || string.IsNullOrEmpty(item.Floor)
        //            || item.ExpiryDate == null
        //        );
        //    foreach (var item in model.LocationItemList)
        //    {
        //        var warehouseLocationItem = WarehouseLocationItemRepository.GetWarehouseLocationItemById(item.Id.Value);
        //        warehouseLocationItem.Shelf = item.Shelf;
        //        warehouseLocationItem.Floor = item.Floor;
        //        warehouseLocationItem.ExpiryDate = item.ExpiryDate;
        //        warehouseLocationItem.LoCode = item.LoCode;
        //        if (!string.IsNullOrEmpty(item.LoCode) && item.ExpiryDate != null)
        //        {
        //            warehouseLocationItem.SN = item.ProductCode + item.LoCode + item.ExpiryDate.Value.ToString("yyyyMMdd") + warehouseLocationItem.Id;
        //        }
        //        WarehouseLocationItemRepository.UpdateWarehouseLocationItem(warehouseLocationItem);
        //    }

        //    WarehouseLocationItemRepository.InsertWarehouseLocationItem(list);

        //    TempData[Globals.SuccessMessageKey] = "Thêm thành công " + list.Count + " sản phẩm có dữ liệu vị trí.";
        //    return Redirect("/MaterialInbound/Detail/" + model.MaterialInboundId);

        //    TempData[Globals.FailedMessageKey] = "Bạn chưa điền dữ liệu vị trí cho tất cả dòng.";
        //    return RedirectToAction("UpdateLocationItem", new { Id = model.MaterialInboundId });
        //}
        //#endregion

        //#region Print
        //public ActionResult Print(int? Id)
        //{
        //    var MaterialInbound = MaterialInboundRepository.GetvwMaterialInboundById(Id.Value);
        //    var logo = Erp.BackOffice.Helpers.Common.GetSetting("LogoCompany");
        //    var ImgLogo = "<div class=\"logo\"><img src=" + logo + " /></div>";
        //    var company = Erp.BackOffice.Helpers.Common.GetSetting("companyName");
        //    var address = Erp.BackOffice.Helpers.Common.GetSetting("addresscompany");
        //    var phone = Erp.BackOffice.Helpers.Common.GetSetting("phonecompany");
        //    var fax = Erp.BackOffice.Helpers.Common.GetSetting("faxcompany");
        //    var bankcode = Erp.BackOffice.Helpers.Common.GetSetting("bankcode");
        //    var bankname = Erp.BackOffice.Helpers.Common.GetSetting("bankname");
        //    if (MaterialInbound != null && MaterialInbound.IsDeleted != true)
        //    {
        //        var model = new TemplatePrintViewModel();
        //        var user = userRepository.GetUserById(MaterialInbound.CreatedUserId.Value);
        //        var inboundDetails = MaterialInboundRepository.GetAllMaterialInboundDetailByInboundId(Id.Value).AsEnumerable()
        //                .Select(x => new MaterialInboundDetailViewModel
        //                {
        //                    Id = x.Id,
        //                    Price = x.Price,
        //                    MaterialId = x.MaterialId,
        //                    MaterialName = MaterialRepository.GetMaterialById(x.MaterialId.Value).Name,
        //                    MaterialCode = MaterialRepository.GetMaterialById(x.MaterialId.Value).Code,
        //                    MaterialInboundId = x.MaterialInboundId,
        //                    Quantity = x.Quantity,
        //                    Unit = x.Unit

        //                }).ToList();
        //        var ListRow = "";
        //        foreach (var item in inboundDetails)
        //        {
        //            decimal? subTotal = item.Quantity * item.Price.Value;
        //            var Row = "<tr>"
        //             + "<td class=\"text-center\">" + (inboundDetails.ToList().IndexOf(item) + 1) + "</td>"
        //             + "<td>" + item.MaterialCode + "</td>"
        //             + "<td>" + item.MaterialName + "</td>"
        //             + "<td class=\"text-center\">" + item.Unit + "</td>"
        //             + "<td class=\"text-right\">" + item.Quantity + "</td>"
        //             + "<td class=\"text-right\">" + Erp.BackOffice.Helpers.Common.PhanCachHangNgan2(item.Price) + "</td>"
        //             + "<td class=\"text-right\">" + Erp.BackOffice.Helpers.Common.PhanCachHangNgan2(subTotal) + "</td></tr>";
        //            ListRow += Row;
        //        }
        //        var style = "<style>.invoice-detail{ width:100%;margin-top: 10px;border-spacing: 0px;}"
        //            + ".invoice-detail th{border: 1px solid #000;border-right: none;padding: 5px;}"
        //            + " .invoice-detail tr th:last-child {border-right: 1px solid #000;}"
        //            + " .invoice-detail td{padding: 5px 5px; border-bottom: 1px solid #000; border-left: 1px solid #000; height: 15px;font-size: 12px;}"
        //            + " .invoice-detail tr td:last-child {border-right: 1px solid #000;}"
        //            + ".invoice-detail tbody tr:last-child td {border-bottom: 1px solid #000;}"
        //            + " .invoice-detail tfoot td{font-weight:bold;border-bottom: 1px solid #000;}"
        //            + " .invoice-detail tfoot tr:first-child td{border-top: 1px solid #000;}"
        //            + ".text-center{text-align:center;}"
        //            + ".text-right{text-align:right;}"
        //            + " .logo{ width: 100px;float: left; margin: 0 0px;height: 100px;line-height: 100px;}"
        //            + ".logo img{ width:100%;vertical-align: middle;}"
        //            + "</style>";
        //        var table = style + "<table class=\"invoice-detail\"><thead><tr> <th>STT</th> <th>Mã hàng</th><th>Tên mặt hàng</th><th>ĐVT</th><th>Số lượng</th><th>Đơn giá</th><th>Thành tiền</th></tr></thead><tbody>"
        //                     + ListRow
        //                     + "</tbody> <tfoot><tr><td colspan=\"6\" class=\"text-center\">Tổng cộng</td><td class=\"text-right\">"
        //                     + Erp.BackOffice.Helpers.Common.PhanCachHangNgan2(MaterialInbound.TotalAmount.Value)
        //                     + "</td></tr></tfoot></table>";
        //        var template = templatePrintRepository.GetAllTemplatePrint().Where(x => x.Code.Contains("MaterialInbound")).OrderByDescending(x => x.CreatedDate).FirstOrDefault();
        //        model.Content = template.Content;
        //        model.Content = model.Content.Replace("{Code}", MaterialInbound.Code);
        //        model.Content = model.Content.Replace("{Day}", MaterialInbound.CreatedDate.Value.Day.ToString());
        //        model.Content = model.Content.Replace("{Month}", MaterialInbound.CreatedDate.Value.Month.ToString());
        //        model.Content = model.Content.Replace("{Year}", MaterialInbound.CreatedDate.Value.Year.ToString());
        //        model.Content = model.Content.Replace("{DataTable}", table);
        //        model.Content = model.Content.Replace("{Logo}", ImgLogo);
        //        model.Content = model.Content.Replace("{System.CompanyName}", company);
        //        model.Content = model.Content.Replace("{System.AddressCompany}", address);
        //        model.Content = model.Content.Replace("{System.PhoneCompany}", phone);
        //        model.Content = model.Content.Replace("{System.Fax}", fax);
        //        model.Content = model.Content.Replace("{System.BankCodeCompany}", bankcode);
        //        model.Content = model.Content.Replace("{System.BankNameCompany}", bankname);
        //        return View(model);
        //    }

        //    if (Request.UrlReferrer != null)
        //        return Redirect(Request.UrlReferrer.AbsoluteUri);
        //    return RedirectToAction("Index");
        //}
        //#endregion

        //#region UpdateData
        //public ActionResult UpdateData()
        //{
        //    var materialInboundDetail = MaterialInboundRepository.GetAllvwMaterialInboundDetail();
        //    foreach (var item in materialInboundDetail)
        //    {
        //        var locationItem = WarehouseLocationItemRepository.GetAllLocationItem().Where(x => x.ProductId == item.ProductId && x.ProductInboundDetailId == item.Id).ToList();

        //        for (int i = 0; i < locationItem.Count(); i++)
        //        {
        //            locationItem[i].ExpiryDate = item.ExpiryDate;
        //            locationItem[i].LoCode = item.LoCode;
        //            if (!string.IsNullOrEmpty(item.LoCode) && item.ExpiryDate.HasValue)
        //            {
        //                locationItem[i].SN = item.ProductCode + item.LoCode + item.ExpiryDate.Value.ToString("yyyyMMdd") + locationItem[i].Id;
        //            }
        //            WarehouseLocationItemRepository.UpdateWarehouseLocationItem(locationItem[i]);
        //        }
        //    }
        //    return RedirectToAction("Index");
        //}
        //#endregion

        #region CreateFromOutbound
        public ActionResult CreateFromOutbound(int? Id)
        {
            var MaterialInbound = new vwMaterialOutbound();
            var Request = new vwRequestInbound();
            if (Id != null)
                Request = requestInboundRepository.GetvwRequestInboundById(Id.Value);
            MaterialInbound = MaterialOutboundRepository.GetvwMaterialOutboundById(Request.OutboundId.Value);
            if (MaterialInbound != null && MaterialInbound.IsDeleted != true)
            {

                var model = new MaterialInboundViewModel();
                //AutoMapper.Mapper.Map(ProductInbound, model);
                // lấy danh sách detail
                var Details = MaterialOutboundRepository.GetAllvwMaterialOutboundDetailByOutboundId(MaterialInbound.Id)
                    .Select(x => new MaterialInboundDetailViewModel
                    {
                        Id = 0,
                        Price = x.Price,
                        MaterialId = x.MaterialId,
                        //ProductInboundId = x.ProductInboundId,
                        Quantity = x.Quantity,
                        MaterialCode = x.MaterialCode,
                        MaterialName = x.MaterialName,
                        Unit = x.Unit,
                        ExpiryDate = x.ExpiryDate,
                        LoCode = x.LoCode,
                        QuantityTest = x.Quantity
                    }).ToList();
                model.Code = Erp.BackOffice.Helpers.Common.GetOrderNo("MaterialInBound");
                model.DetailList = Details;
                model.WarehouseDestinationId = MaterialInbound.WarehouseDestinationId;
                model.CreatedDate = DateTime.Now;
                model.CreatedUserName = Helpers.Common.CurrentUser.FullName;
                model.WarehouseDestinationName = Request.WarehouseDestinationName;
                model.MaterialOutboundId = Request.OutboundId;
                model.BranchId = Request.BranchId;
                ViewBag.RequestInbound = Id;
                return View(model);
            }

            return View();
        }

        [HttpPost]
        public ActionResult CreateFromOutbound(MaterialInboundViewModel model)
        {
            var urlRefer = Request["UrlReferrer"];
            if (ModelState.IsValid)
            {
                model.DetailList.RemoveAll(x => x.Quantity <= 0);
                var MaterialInbound = new Domain.Sale.Entities.MaterialInbound();
                using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.Snapshot }))
                {
                    try
                    {
                        AutoMapper.Mapper.Map(model, MaterialInbound);
                        MaterialInbound.IsDeleted = false;
                        MaterialInbound.CreatedUserId = WebSecurity.CurrentUserId;
                        MaterialInbound.ModifiedUserId = WebSecurity.CurrentUserId;
                        MaterialInbound.CreatedDate = DateTime.Now;
                        MaterialInbound.ModifiedDate = DateTime.Now;
                        MaterialInbound.BranchId = model.BranchId;
                        MaterialInbound.TotalAmount = model.DetailList.Sum(x => x.Price * (decimal)x.Quantity);
                        MaterialInbound.MaterialOutboundId = model.MaterialOutboundId;

                        MaterialInbound.Type = "internal";
                        //thêm mới phiếu nhập và chi tiết phiếu nhập
                        MaterialInboundRepository.InsertMaterialInbound(MaterialInbound);

                        //cập nhật lại mã nhập kho
                        MaterialInbound.Code = Erp.BackOffice.Helpers.Common.GetOrderNo("MaterialInbound", model.Code);
                        MaterialInboundRepository.UpdateMaterialInbound(MaterialInbound);
                        Erp.BackOffice.Helpers.Common.SetOrderNo("MaterialInbound");
                        int number_error = 0;

                        //Thêm chi tiết phiếu nhập
                        foreach (var item in model.DetailList)
                        {
                            var materialInboundDetail = new MaterialInboundDetail();
                            materialInboundDetail.IsDeleted = false;
                            materialInboundDetail.CreatedUserId = WebSecurity.CurrentUserId;
                            materialInboundDetail.ModifiedUserId = WebSecurity.CurrentUserId;
                            materialInboundDetail.CreatedDate = DateTime.Now;
                            materialInboundDetail.ModifiedDate = DateTime.Now;
                            materialInboundDetail.MaterialInboundId = MaterialInbound.Id;
                            materialInboundDetail.MaterialId = item.MaterialId;
                            materialInboundDetail.Quantity = item.Quantity;
                            materialInboundDetail.Unit = item.Unit;
                            materialInboundDetail.ExpiryDate = item.ExpiryDate;
                            materialInboundDetail.LoCode = item.LoCode;
                            materialInboundDetail.Price = item.Price;
                            MaterialInboundRepository.InsertMaterialInboundDetail(materialInboundDetail);




                        }

                        //Cập nhật lại phiếu xuất
                        if (model.MaterialOutboundId != null && model.MaterialOutboundId > 0)
                        {
                            var materialOutbound = MaterialOutboundRepository.GetMaterialOutboundById(model.MaterialOutboundId.Value);
                            materialOutbound.IsDone = true;
                        }
                        //cập nhật lại yêu cầu nhập kho
                        var requestInbound = Request["RequestInbound"];
                        var request = requestInboundRepository.GetRequestInboundById(Convert.ToInt32(requestInbound));
                        request.InboundId = MaterialInbound.Id;
                        if (number_error > 0)
                        {
                            request.Error = true;
                            request.ErrorQuantity = number_error;
                        }
                        requestInboundRepository.UpdateRequestInbound(request);
                        //Thêm vào quản lý chứng từ
                        TransactionController.Create(new TransactionViewModel
                        {
                            TransactionModule = "MaterialInbound",
                            TransactionCode = MaterialInbound.Code,
                            TransactionName = "Nhập kho"
                        });
                        scope.Complete();
                    }
                    catch (DbUpdateException)
                    {
                        return Content("Fail");
                    }
                }
                TempData[Globals.SuccessMessageKey] = App_GlobalResources.Wording.InsertSuccess;
                //return RedirectToAction("Index", new { Id = ProductInbound.Id });
                if (Request["IsPopup"] == "true" || Request["IsPopup"] == "True")
                {
                    ViewBag.closePopup = "true";
                    ViewBag.urlRefer = urlRefer;
                    return View(model);
                }
                return Redirect(urlRefer);
            }
            return View(model);
        }
        #endregion

        //#region Approval chấp thuận chênh lệch giữa phiếu xuất và phiếu nhập.
        //public ActionResult Approval(int Id, int? ProductInboundId)
        //{
        //    // nếu trạng thái của quyết định điều chuyển là chờ duyệt thì chuyển thành đã duyệt
        //    // ngược lại thì chuyển thành Đã chuyển và cập nhật lại dữ liệu nhân viên trong bảng staff
        //    var q = productDamagesRepository.GetProductDamagedById(Id);
        //    q.Status = "success";

        //    productDamagesRepository.UpdateProductDamaged(q);
        //    var requestinbound = requestInboundRepository.GetAllRequestInbound().Where(x => x.InboundId == ProductInboundId).FirstOrDefault();
        //    requestinbound.ErrorQuantity = requestinbound.ErrorQuantity - 1;
        //    requestInboundRepository.UpdateRequestInbound(requestinbound);
        //    //gửi notifications cho người được phân quyền.
        //    Crm.Controllers.ProcessController.Run("ProductInbound", "Approval", q.Id, q.CreatedUserId, null, q, requestinbound.BranchId.Value.ToString());

        //    return RedirectToAction("Detail", "ProductInbound", new { area = "Sale", Id = ProductInboundId });
        //}
        //#endregion

        #region Print
        public ActionResult Print(int? Id)
        {
            //Lấy thông tin phiếu xuất kho
            var MaterialInbound = MaterialInboundRepository.GetvwMaterialInboundById(Id.Value);

            if (MaterialInbound != null)
            {

                //Lấy template và replace dữ liệu vào phiếu xuất.
                TemplatePrint template = null;

                template = templatePrintRepository.GetAllTemplatePrint().Where(x => x.Code.Contains("MaterialInbound")).OrderByDescending(x => x.CreatedDate).FirstOrDefault();
                if (template == null)
                    return Content("No template!");

                string output = template.Content;

                //lấy logo công ty
                var logo = Erp.BackOffice.Helpers.Common.GetSetting("LogoCompany");
                var company = Erp.BackOffice.Helpers.Common.GetSetting("companyName");
                var address = Erp.BackOffice.Helpers.Common.GetSetting("addresscompany");
                var phone = Erp.BackOffice.Helpers.Common.GetSetting("phonecompany");
                var fax = Erp.BackOffice.Helpers.Common.GetSetting("faxcompany");
                var bankcode = Erp.BackOffice.Helpers.Common.GetSetting("bankcode");
                var bankname = Erp.BackOffice.Helpers.Common.GetSetting("bankname");
                var ImgLogo = "<div class=\"logo\"><img src=" + logo + " height=\"63\" /></div>";

                output = output.Replace("{System.Logo}", ImgLogo);
                output = output.Replace("{System.CompanyName}", company);
                output = output.Replace("{System.AddressCompany}", address);
                output = output.Replace("{System.PhoneCompany}", phone);
                output = output.Replace("{System.Fax}", fax);
                output = output.Replace("{System.BankCodeCompany}", bankcode);
                output = output.Replace("{System.BankNameCompany}", bankname);

                string note = MaterialInbound.Note;
                string code = MaterialInbound.Code;
                //manual nhập từ kho
                //external nhập từ đơn đặt hàng
                //SalesReturns nhập từ đơn hàng trả lại
                if (MaterialInbound.Type == "internal")//nhập từ phiếu xuất
                {
                    if (MaterialInbound.MaterialOutboundId != null && MaterialInbound.MaterialOutboundId.Value > 0)
                    {
                        var materialOutbound = MaterialOutboundRepository.GetvwMaterialOutboundById(MaterialInbound.MaterialOutboundId.Value);
                        output = output.Replace("{WarehouseSourceName}", "phiếu xuất: " + materialOutbound.Code);
                    }
                }
                output = output.Replace("{WarehouseDestinationName}", MaterialInbound.WarehouseDestinationName);
                output = output.Replace("{MaterialInboundDate}", MaterialInbound.CreatedDate.Value.ToString("dd/MM/yyyy HH:mm"));
                //Tạo barcode
                Image barcode = Code128Rendering.MakeBarcodeImage(MaterialInbound.Code, 1, true);
                output = output.Replace("{BarcodeImgSource}", ImageToBase64(barcode, System.Drawing.Imaging.ImageFormat.Png));

                output = output.Replace("{Note}", note);
                output = output.Replace("{Code}", code);

                //Mid
                output = output.Replace("{DetailList}", buildHtmlDetailList(Id.Value));

                //Footer
                ViewBag.PrintContent = output;
                return View();
            }

            if (Request.UrlReferrer != null)
                return Redirect(Request.UrlReferrer.AbsoluteUri);
            return RedirectToAction("Index");
        }

        string buildHtmlDetailList(int Id)
        {
            decimal tong_tien = 0;
            //Lấy danh sách sản phẩm xuất kho
            var inboundDetails = MaterialInboundRepository.GetAllvwMaterialInboundDetailByInboundId(Id).AsEnumerable()
                    .Select(x => new MaterialInboundDetailViewModel
                    {
                        Id = x.Id,
                        Price = x.Price,
                        MaterialId = x.MaterialId,
                        MaterialName = x.MaterialName,
                        MaterialCode = x.MaterialCode,
                        MaterialInboundId = x.MaterialInboundId,
                        Quantity = x.Quantity,
                        Unit = x.Unit,
                        LoCode = x.LoCode,
                        ExpiryDate = x.ExpiryDate
                    }).ToList();

            //Điền lô/date
            //Tạo table html chi tiết phiếu xuất
            string detailList = "<table class=\"invoice-detail\">\r\n";
            detailList += "<thead>\r\n";
            detailList += "	<tr>\r\n";
            detailList += "		<th>STT</th>\r\n";
            detailList += "		<th>Mã vật tư</th>\r\n";
            detailList += "		<th>Tên vật tư</th>\r\n";
            detailList += "		<th>L&ocirc; sản xuất</th>\r\n";
            detailList += "		<th>Hạn d&ugrave;ng</th>\r\n";
            detailList += "		<th>ĐVT</th>\r\n";
            detailList += "		<th>Số lượng</th>\r\n";
            detailList += "		<th>Đơn gi&aacute;</th>\r\n";
            detailList += "		<th>Th&agrave;nh tiền</th>\r\n";
            detailList += "	</tr>\r\n";
            detailList += "</thead>\r\n";
            detailList += "<tbody><tbody>\r\n";

            foreach (var item in inboundDetails)
            {
                decimal thanh_tien = (decimal)item.Quantity.Value * item.Price.Value;
                tong_tien += thanh_tien;

                detailList += "<tr>\r\n"
                 + "<td class=\"text-center\">" + (inboundDetails.ToList().IndexOf(item) + 1) + "</td>\r\n"
                 + "<td class=\"text-left\">" + item.MaterialCode + "</td>\r\n"
                 + "<td class=\"text-left\">" + item.MaterialName + "</td>\r\n"
                 + "<td class=\"text-right\">" + item.LoCode + "</td>\r\n"
                 + "<td class=\"text-right\">" + (item.ExpiryDate.HasValue ? item.ExpiryDate.Value.ToString("dd-MM-yyyy") : "") + "</td>\r\n"
                 + "<td class=\"text-center\">" + item.Unit + "</td>\r\n"
                 + "<td class=\"text-right\">" + Erp.BackOffice.Helpers.Common.PhanCachHangNgan2(item.Quantity) + "</td>\r\n"
                 + "<td class=\"text-right\">" + Erp.BackOffice.Helpers.CommonSatic.ToCurrencyStr(item.Price, null) + "</td>\r\n"
                 + "<td class=\"text-right\">" + Erp.BackOffice.Helpers.CommonSatic.ToCurrencyStr(thanh_tien, null) + "</td>\r\n"
                 + "</tr>\r\n";
            }

            detailList += "</tbody>\r\n";
            detailList += "<tfoot>\r\n";
            detailList += "<tr><td colspan=\"6\" class=\"text-right\"></td><td class=\"text-right\">"
                         + Erp.BackOffice.Helpers.Common.PhanCachHangNgan2(inboundDetails.Sum(x => x.Quantity))
                         + "</td><td class=\"text-right\">Tổng cộng</td><td class=\"text-right\">"
                         + Erp.BackOffice.Helpers.CommonSatic.ToCurrencyStr(tong_tien, null)
                         + "</td></tr>\r\n";
            detailList += "<tr><td colspan=\"9\"><strong>Tiền bằng chữ: " + Erp.BackOffice.Helpers.Common.ChuyenSoThanhChu_2(tong_tien.ToString()) + "</strong></td></tr>\r\n";
            detailList += "</tfoot>\r\n</table>\r\n";

            return detailList;
        }
        public string ImageToBase64(Image image, System.Drawing.Imaging.ImageFormat format)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                // Convert Image to byte[]
                image.Save(ms, format);
                byte[] imageBytes = ms.ToArray();

                // Convert byte[] to Base64 String
                string base64String = Convert.ToBase64String(imageBytes);
                return base64String;
            }
        }
        #endregion

        #region Delete
        [HttpPost]
        public ActionResult Delete(int Id, string Note)
        {
            var materialInbound = MaterialInboundRepository.GetMaterialInboundById(Id);
            if (materialInbound != null)
            {
                //Kiểm tra phân quyền Chi nhánh
                //if (!(Filters.SecurityFilter.IsAdmin() || ("," + Helpers.Common.CurrentUser.DrugStore + ",").Contains("," + productInbound.BranchId + ",") == true))
                //{
                //    return Content("Mẫu tin không tồn tại! Không có quyền truy cập!");
                //}

                materialInbound.ModifiedUserId = WebSecurity.CurrentUserId;
                materialInbound.ModifiedDate = DateTime.Now;
                materialInbound.IsDeleted = true;
                materialInbound.IsArchive = false;
                materialInbound.Note = Note;
                MaterialInboundRepository.UpdateMaterialInbound(materialInbound);
                //ghi log 
                Erp.BackOffice.Controllers.HomeController.WriteLog(materialInbound.Id, materialInbound.Code, "đã hủy phiếu nhập vật tư", "MaterialInbound/Detail/" + materialInbound.Id, Helpers.Common.CurrentUser.BranchId.Value);

                return RedirectToAction("Detail", new { Id = materialInbound.Id });
            }

            return RedirectToAction("Index");
        }
        #endregion

        public static void CreateFromMaterialOutbound(MaterialOutbound outbound, List<MaterialOutboundDetail> ListDetail)
        {

            Erp.Domain.Sale.Repositories.MaterialInboundRepository materialInboundRepository = new Erp.Domain.Sale.Repositories.MaterialInboundRepository(new Domain.Sale.ErpSaleDbContext());
            var materialInbound = new Domain.Sale.Entities.MaterialInbound();
            materialInbound.IsDeleted = false;
            materialInbound.CreatedUserId = WebSecurity.CurrentUserId;
            materialInbound.ModifiedUserId = WebSecurity.CurrentUserId;
            materialInbound.CreatedDate = DateTime.Now;
            materialInbound.ModifiedDate = DateTime.Now;
            materialInbound.BranchId = outbound.BranchId;
            materialInbound.TotalAmount = outbound.TotalAmount;
            materialInbound.IsArchive = false;
            materialInbound.MaterialOutboundId = outbound.Id;
            materialInbound.WarehouseDestinationId = outbound.WarehouseDestinationId;
            materialInbound.Type = "internal";
            materialInboundRepository.InsertMaterialInbound(materialInbound);

            //Cập nhật lại mã xuất kho
            materialInbound.Code = Erp.BackOffice.Helpers.Common.GetOrderNo("MaterialInbound", null);
            materialInboundRepository.UpdateMaterialInbound(materialInbound);
            Erp.BackOffice.Helpers.Common.SetOrderNo("MaterialInbound");

            //string prefix = Erp.BackOffice.Helpers.Common.GetSetting("prefixOrderNo_Inbound");
            //materialInbound.Code = Erp.BackOffice.Helpers.Common.GetCode(prefix, materialInbound.Id);
            //materialInboundRepository.UpdateMaterialInbound(materialInbound);
            // chi tiết phiếu xuất
            foreach (var item in ListDetail)
            {
                MaterialInboundDetail inboundDetail = new MaterialInboundDetail();
                inboundDetail.IsDeleted = false;
                inboundDetail.CreatedUserId = WebSecurity.CurrentUserId;
                inboundDetail.ModifiedUserId = WebSecurity.CurrentUserId;
                inboundDetail.CreatedDate = DateTime.Now;
                inboundDetail.ModifiedDate = DateTime.Now;
                inboundDetail.MaterialInboundId = materialInbound.Id;
                inboundDetail.ExpiryDate = item.ExpiryDate;
                inboundDetail.LoCode = item.LoCode;
                inboundDetail.Price = item.Price;
                inboundDetail.MaterialId = item.MaterialId;
                inboundDetail.Quantity = item.Quantity;
                inboundDetail.Unit = item.Unit;
                materialInboundRepository.InsertMaterialInboundDetail(inboundDetail);
            }
            //cập nhật vị trí các sản phẩm đã xuất kho
            //
            //Thêm vào quản lý chứng từ
            TransactionController.Create(new TransactionViewModel
            {
                TransactionModule = "MaterialInbound",
                TransactionCode = materialInbound.Code,
                TransactionName = "Nhập kho"
            });

            //Thêm chứng từ liên quan
            TransactionController.CreateRelationship(new TransactionRelationshipViewModel
            {
                TransactionA = materialInbound.Code,
                TransactionB = outbound.Code
            });
        }

        #region NhapExcel
        [HttpPost]
        public JsonResult ImportExcel(List<sanphamexcel> listsp)
        {
            var modellist = new List<MaterialInboundDetailViewModel>();
            var list1 = new List<MaterialInboundDetailViewModel>();
            var list2 = new List<MaterialInboundDetailViewModel>();


            MaterialInboundViewModel model = new MaterialInboundViewModel();
            AutoMapper.Mapper.Map(modellist, model.DetailList);
            model.TotalAmount = 0;

            int WarehouseSourceId = 0;
            if (listsp.Count > 0)
            {
                WarehouseSourceId = Helpers.Common.NVL_NUM_NT_NEW(listsp[0].WarehouseId);
            }

            model.WarehouseKeeperId = WarehouseSourceId;

            int m = 1;
            foreach (var item in listsp)
            {
                var inbound = new MaterialInboundDetailViewModel();
                var mat = MaterialRepository.GetAllMaterial().Where(x => x.Code == item.MaSanPham).SingleOrDefault();
                if (mat != null)
                {
                    inbound.MaterialId = mat.Id;
                    inbound.OrderNo = Helpers.Common.NVL_NUM_NT_NEW(item.STT);
                    inbound.Price = Helpers.Common.NVL_NUM_DECIMAL_NEW(item.DonGia.Replace(",", "").Replace(".", ""));
                    inbound.MaterialCode = mat.Code;
                    inbound.MaterialName = mat.Name;
                    inbound.Quantity = Convert.ToInt32(item.SoLuong.Replace(",", "").Replace(".", ""));
                    list1.Add(inbound);
                }
                else
                {
                    inbound.MaterialId = 0;
                    inbound.OrderNo = Helpers.Common.NVL_NUM_NT_NEW(m);
                    inbound.Price = Helpers.Common.NVL_NUM_DECIMAL_NEW(0);
                    inbound.MaterialCode = item.MaSanPham;
                    inbound.MaterialName = "không tồn tại!";
                    inbound.Quantity = Convert.ToInt32(item.SoLuong.Replace(",", "").Replace(".", ""));
                    list2.Add(inbound);
                    m++;
                }
            }
            modellist = list2;

            int n = list2.Count() + 1;
            foreach (var i in list1)
            {
                i.OrderNo = n;
                modellist.Add(i);
                n++;
            }
            model.DetailList = modellist;
            string json = "";

            if (model.DetailList != null && model.DetailList.Count > 0)
            {
                var itemxoa = model.DetailList[0];

                foreach (var item in model.DetailList)
                {
                    string a = "<tr class=\"detail_item\" role=\"" + item.OrderNo + "\">\r\n";
                    if (item.MaterialId == 0)
                    {
                        a = "<tr style=\"background-color: yellow;\" class=\"detail_item\" role=\" " + item.OrderNo + "\" id=\"product_item_" + item.OrderNo + "\" data-id=\" " + item.OrderNo + "\">\r\n";
                    }
                    a += "<td class=\"text-center\">";
                    a += item.OrderNo + "</td>";

                    a += "<td>" +
                "<input class=\"item_id\" id = \"DetailList_" + item.OrderNo + "__MaterialId\" name=\"DetailList[" + item.OrderNo + "].MaterialId\" type=\"hidden\" value=\"" + item.MaterialId + "\"> " +
                "<input class=\"item_material_id\" id = \"DetailList_" + item.OrderNo + "__MaterialId\" name=\"DetailList[" + item.OrderNo + "].MaterialId\" type=\"hidden\" value=\"" + item.MaterialId + "\">" +
                "<span class=\"item_material_name\">" + item.MaterialName + "</span>";

                    a += "</td><td class=\"detail_locode\">" +


               "<input class=\"item_locode valid_null\" id = \"DetailList_" + item.OrderNo + "__LoCode\" name=\"DetailList[" + item.OrderNo + "].LoCode\"  value=\"" + item.LoCode + "\" style=\"width:80px\">" +
                "<input class=\"input-mask-date item_expiry_date valid_null\" type =\"text\" id = \"DetailList_" + item.OrderNo + "_ExpiryDate\" name=\"DetailList[" + item.OrderNo + "].ExpiryDate\" value=\"" + item.ExpiryDate + "\" style=\"width:80px;\"> ";


                    a += "</td><td>" +
                "<input type = \"hidden\" name=\"DetailList[" + item.OrderNo + "].Unit\" value=\"" + item.Unit + "\" class=\"item_unit\">" +
                "<input class=\"item_quantity numberinput2 text-right\" type=\"text\" id=\"DetailList_"+item.OrderNo+"__Quantity\" name=\"DetailList["+item.OrderNo+"].Quantity\" value=\""+item.Quantity+"\" style=\"width:100%\"> ";

                    a += "</td><td>" +

                    "<input class=\"item_price numberinput2 text-right\" type=\"text\" id=\"DetailList_" + item.OrderNo + "__Price\" name=\"DetailList[" + item.OrderNo + "].Price\" value=\"" + item.Price + "\" style=\"width:100%\"> ";

                    a += "<td align=\"right\"><span class=\"item_total\" align=\"right\">"+ Helpers.CommonSatic.ToCurrencyStr((decimal)item.Quantity * item.Price, null) + "</span></td>";
                    a += "</td><td class=\"text-center\">" +
                        "<a class=\"btn-delete-item\">" +
                            "<i class=\"ace-icon fa fa-trash red bigger-120\" style=\"cursor:pointer\"></i> </a></td></tr>";

                    json += a;
                }
                //them 1 dong
                string a1 = "<tr class=\"detail_item\" role=\"" + itemxoa.OrderNo + "\">\r\n";
                if (itemxoa.MaterialId == 0)
                {
                    a1 = "<tr style=\"background-color: yellow;\" class=\"detail_item\" role=\" " + itemxoa.OrderNo + "\" id=\"product_item_" + itemxoa.OrderNo + "\" data-id=\" " + itemxoa.OrderNo + "\">\r\n";
                }
                a1 += "<td class=\"text-center\">";
                a1 += itemxoa.OrderNo + "</td>";

                a1 += "<td>" +
            "<input class=\"item_id\" id = \"DetailList_" + itemxoa.OrderNo + "__MaterialId\" name=\"DetailList[" + itemxoa.OrderNo + "].MaterialId\" type=\"hidden\" value=\"" + itemxoa.MaterialId + "\"> " +
            "<input class=\"item_material_id\" id = \"DetailList_" + itemxoa.OrderNo + "__MaterialId\" name=\"DetailList[" + itemxoa.OrderNo + "].MaterialId\" type=\"hidden\" value=\"" + itemxoa.MaterialId + "\">" +
            "<span class=\"item_material_name\">" + itemxoa.MaterialName + "</span>";

                a1 += "</td><td class=\"detail_locode\">" +


           "<input class=\"item_locode valid_null\" id = \"DetailList_" + itemxoa.OrderNo + "__LoCode\" name=\"DetailList[" + itemxoa.OrderNo + "].LoCode\"  value=\"" + itemxoa.LoCode + "\" style=\"width:80px\">" +
            "<input class=\"input-mask-date item_expiry_date valid_null\" type =\"text\" id = \"DetailList_" + itemxoa.OrderNo + "_ExpiryDate\" name=\"DetailList[" + itemxoa.OrderNo + "].ExpiryDate\" value=\"" + itemxoa.ExpiryDate + "\" style=\"width:80px;\"> ";


                a1 += "</td><td>" +
            "<input type = \"hidden\" name=\"DetailList[" + itemxoa.OrderNo + "].Unit\" value=\"" + itemxoa.Unit + "\" class=\"item_unit\">" +
            "<input class=\"item_quantity numberinput2 text-right\" type=\"text\" id=\"DetailList_" + itemxoa.OrderNo + "__Quantity\" name=\"DetailList[" + itemxoa.OrderNo + "].Quantity\" value=\"" + itemxoa.Quantity + "\" style=\"width:100%\"> ";

                a1 += "</td><td>" +

                "<input class=\"item_price numberinput2 text-right\" type=\"text\" id=\"DetailList_" + itemxoa.OrderNo + "__Price\" name=\"DetailList[" + itemxoa.OrderNo + "].Price\" value=\"" + itemxoa.Price + "\" style=\"width:100%\"> ";

                a1 += "<td align=\"right\"><span class=\"item_total\" align=\"right\">" + Helpers.CommonSatic.ToCurrencyStr((decimal)itemxoa.Quantity * itemxoa.Price, null) + "</span></td>";
                a1 += "</td><td class=\"text-center\">" +
                    "<a1 class=\"btn-delete-item\">" +
                        "<i class=\"ace-icon fa fa-trash red bigger-120\" style=\"cursor:pointer\"></i> </a1></td></tr>";

                json += a1;

            }
            return Json(json);
        }

        public ActionResult ExportMaterialInBound(bool ExportExcel = false)
        {
            var data = TempData["DataMaterialInBound"] as List<MaterialInboundViewModel>;

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
            model.Content = model.Content.Replace("{DataTable}", buildHtmlMaterialInBound(data));
            model.Content = model.Content.Replace("{System.Logo}", ImgLogo);
            model.Content = model.Content.Replace("{System.CompanyName}", company);
            model.Content = model.Content.Replace("{System.AddressCompany}", address);
            model.Content = model.Content.Replace("{System.PhoneCompany}", phone);
            model.Content = model.Content.Replace("{System.Fax}", fax);
            model.Content = model.Content.Replace("{System.BankCodeCompany}", bankcode);
            model.Content = model.Content.Replace("{System.BankNameCompany}", bankname);
            model.Content = model.Content.Replace("{PrintDate}", DateTime.Now.ToString("dd/MM/yyyy HH:mm"));
            model.Content = model.Content.Replace("{Title}", "Danh sách phiếu nhập vật tư");

            Response.AppendHeader("content-disposition", "attachment;filename=" + "DS_PhieuNhap" + DateTime.Now.ToString("dd_MM_yyyy") + ".xls");
            Response.Charset = "";
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.Write(model.Content);
            Response.End();

            return View(model);
        }

        string buildHtmlMaterialInBound(List<MaterialInboundViewModel> detailList)
        {
            string detailLists = "<table border=\"1\" class=\"invoice-detail\">";
            detailLists += "<thead>";
            detailLists += "	<tr>";
            detailLists += "		<th>STT</th>";
            detailLists += "		<th>Ngày tạo</th>";
            //detailLists += "		<th>Người tạo</th>";
            detailLists += "		<th >Mã</th>";
            detailLists += "		<th>Ghi chú</th>";
            detailLists += "		<th>Tổng tiền</th>";
            detailLists += "		<th>Nhập vào kho</th>";
            detailLists += "		<th>Ngày cập nhật</th>";
            detailLists += "		<th>Thông tin ghi sổ</th>";
            detailLists += "</thead>";
            detailLists += "<tbody>";
            var index = 1;
            string isArchive = "";
            foreach (var item in detailList)
            {
                if (item.IsArchive == true)
                    isArchive = "Đã ghi sổ";
                else
                    isArchive = "Chưa ghi sổ";
                detailLists += "<tr>"
                + "<td class=\"text-center\">" + (index++) + "</td>"
                + "<td>" + item.CreatedDate + "</td>"
                //+ "<td>" + item.CreatedUserName + "</td>"
                + "<td>" + item.Code + "</td>"
                + "<td>" + item.Note + "</td>"
                + "<td>" + CommonSatic.ToCurrencyStr(item.TotalAmount, null) + "</td>"
                + "<td>" + item.WarehouseDestinationName + "</td>"
                + "<td>" + item.ModifiedDate + "</td>"
                + "<td>" + isArchive + "</td>"
                + "</tr>";
            }

            detailLists += "</tbody>";
            detailLists += "<tfoot style=\"font-weight:bold\">";
            detailLists += "</tfoot></table>";
            return detailLists;
        }

        public DataTable getData()
        {
            //Creating DataTable  
            DataTable dt = new DataTable();
            //Setiing Table Name  
            dt.TableName = "ProductInBoundData";
            //Add Columns  
            dt.Columns.Add("STT", typeof(int));
            dt.Columns.Add("MaSanPham", typeof(string));
            dt.Columns.Add("SoLuong", typeof(int));
            dt.Columns.Add("DonGia", typeof(decimal));
            //Add Rows in DataTable  
            dt.Rows.Add(1, "BG01", 15, 300000);
            dt.Rows.Add(2, "BG02", 20, 200000);
            dt.Rows.Add(3, "CLM", 20, 400000);
            dt.Rows.Add(3, "HIIFU 3D", 30, 10000);
            dt.AcceptChanges();
            return dt;
        }
        public ActionResult PrintExample()
        {

            //var model = new ImportHangKM();
            ////Encoding encoding = Encoding.UTF8;

            //var template = templatePrintRepository.GetAllTemplatePrint().Where(x => x.Code.Contains("importExInBound")).OrderByDescending(x => x.CreatedDate).FirstOrDefault();
            ////Response.ContentEncoding = Encoding.Unicode;
            //using (XLWorkbook wb = new XLWorkbook())
            //{
            //    wb.Worksheets.Add("qwe");

            //    Response.Clear();
            //    Response.Buffer = true;
            //    Response.Charset = "";
            //    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            //    Response.AddHeader("content-disposition", "attachment;filename=GridView.xlsx");
            //    using (MemoryStream MyMemoryStream = new MemoryStream())
            //    {
            //        wb.SaveAs(MyMemoryStream);
            //        MyMemoryStream.WriteTo(Response.OutputStream);
            //        Response.Flush();
            //        Response.End();
            //    }
            //}
            DataTable dt = getData();
            //Name of File  
            string fileName = "ExcelMauNhap.xlsx";
            using (XLWorkbook wb = new XLWorkbook())
            {
                //Add DataTable in worksheet  
                wb.Worksheets.Add(dt);
                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    //Return xlsx Excel File  
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
                }
            }
            //model.Content = template.Content;
            //Response.BinaryWrite(excel)
            //Response.AddHeader("content-disposition", "attachment; filename=" + "MauExcel" + DateTime.Now.ToString("yyyyMMdd") +".xls");
            //Response.Charset = "";
            //Response.Cache.SetCacheability(HttpCacheability.NoCache);
            //Response.ContentType = "application/ms-excel";

            //Response.Write(model.Content);
            //Response.End();
            //return View(model);
        }
        #endregion
    }
}
