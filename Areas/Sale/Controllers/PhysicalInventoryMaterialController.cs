
using Erp.BackOffice.Filters;
using Erp.BackOffice.Sale.Models;
using Erp.Domain.Interfaces;
using Erp.Domain.Sale.Entities;
using Erp.Domain.Sale.Interfaces;
using Erp.Utilities;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Globalization;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using WebMatrix.WebData;

namespace Erp.BackOffice.Sale.Controllers
{
    [Authorize]
    [InitializeSimpleMembership]
    [Erp.BackOffice.Helpers.NoCacheHelper]
    public class PhysicalInventoryMaterialController : Controller
    {
        private readonly IWarehouseRepository WarehouseRepository;
        private readonly IProductOrServiceRepository ProductRepository;
        private readonly IMaterialOrServiceRepository MaterialRepository;
        private readonly IInventoryMaterialRepository inventoryMaterialRepository;
        private readonly IMaterialInboundRepository MaterialInboundRepository;
        private readonly IMaterialOutboundRepository MaterialOutboundRepository;
        private readonly IPhysicalInventoryMaterialRepository PhysicalInventoryMaterialRepository;
        private readonly IUserRepository userRepository;
        private readonly IWarehouseLocationItemRepository warehouseLocationItemRepository;
        private readonly ICategoryRepository categoryRepository;
        private readonly ITemplatePrintRepository templatePrintRepository;

        public PhysicalInventoryMaterialController(
            IInventoryMaterialRepository _Inventory
            , IProductOrServiceRepository _Product
            , IMaterialOrServiceRepository _Material
            , IWarehouseRepository _Warehouse
            , IMaterialInboundRepository _MaterialInbound
            , IMaterialOutboundRepository _MaterialOutbound
            , IPhysicalInventoryMaterialRepository _PhysicalInventoryMaterial
            , IUserRepository _user
            , IWarehouseLocationItemRepository _WarehouseLocationItem
            , ICategoryRepository _CategoryRepository
            , ITemplatePrintRepository _templatePrint
            )
        {
            WarehouseRepository = _Warehouse;
            ProductRepository = _Product;
            MaterialRepository = _Material;
            inventoryMaterialRepository = _Inventory;
            MaterialInboundRepository = _MaterialInbound;
            MaterialOutboundRepository = _MaterialOutbound;
            userRepository = _user;
            PhysicalInventoryMaterialRepository = _PhysicalInventoryMaterial;
            warehouseLocationItemRepository = _WarehouseLocationItem;
            categoryRepository = _CategoryRepository;
            templatePrintRepository = _templatePrint;
        }

        public ActionResult Index(string txtSearch, string txtWarehouseName)
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


            IEnumerable<PhysicalInventoryMaterialViewModel> list = PhysicalInventoryMaterialRepository.GetAllvwPhysicalInventoryMaterial().Where(x => x.BranchId == intBrandID)
                .Select(item => new PhysicalInventoryMaterialViewModel
                {
                    Id = item.Id,
                    Code = item.Code,
                    CreatedDate = item.CreatedDate,
                    ModifiedDate = item.ModifiedDate,
                    Note = item.Note,
                    WarehouseId = item.WarehouseId,
                    WarehouseName = item.WarehouseName,
                    IsExchange = item.IsExchange,
                    BranchId = item.BranchId,
                    Status = item.Status
                }).OrderByDescending(x => x.CreatedDate);


            if (string.IsNullOrEmpty(txtSearch) == false || string.IsNullOrEmpty(txtWarehouseName) == false)
            {
                txtSearch = txtSearch == "" ? "~" : txtSearch.ToLower();
                txtWarehouseName = txtWarehouseName == "" ? "~" : Erp.BackOffice.Helpers.Common.ChuyenThanhKhongDau(txtWarehouseName);
                list = list.Where(x => x.Code.ToLower().Contains(txtSearch) || Erp.BackOffice.Helpers.Common.ChuyenThanhKhongDau(x.WarehouseName).Contains(txtWarehouseName));
            }

            return View(list);
        }

        public ActionResult Create()
        {
            var model = new PhysicalInventoryMaterialViewModel();
            var warehouseList = WarehouseRepository.GetvwAllWarehouse().AsEnumerable();
            warehouseList = warehouseList.Where(x => x.Categories == "VT").ToList();
            var _warehouseList = warehouseList.Select(item => new SelectListItem
            {
                Text = item.Name + " (" + item.BranchName + ")",
                Value = item.Id.ToString()
            });

            ViewBag.warehouseList = _warehouseList;
            return View(model);
        }

        [HttpPost]
        public ActionResult Create(PhysicalInventoryMaterialViewModel model)
        {
            if (ModelState.IsValid)
            {
                using (var scope = new TransactionScope(TransactionScopeOption.Required,
                               new System.TimeSpan(0, 30, 0)))
                {
                    try
                    {
                        PhysicalInventoryMaterial PhysicalInventoryMaterial = new Domain.Sale.Entities.PhysicalInventoryMaterial();
                        AutoMapper.Mapper.Map(model, PhysicalInventoryMaterial);
                        PhysicalInventoryMaterial.CreatedUserId = WebSecurity.CurrentUserId;
                        PhysicalInventoryMaterial.CreatedDate = DateTime.Now;
                        PhysicalInventoryMaterial.ModifiedUserId = WebSecurity.CurrentUserId;
                        PhysicalInventoryMaterial.ModifiedDate = DateTime.Now;
                        PhysicalInventoryMaterial.IsDeleted = false;
                        PhysicalInventoryMaterial.IsExchange = false;
                        var warehouse = WarehouseRepository.GetWarehouseById(model.WarehouseId.Value);
                        PhysicalInventoryMaterial.BranchId = warehouse.BranchId;

                        List<PhysicalInventoryMaterialDetail> PhysicalInventoryMaterialDetailList = new List<PhysicalInventoryMaterialDetail>();
                        if (model.DetailList != null)
                        {
                            var list_inventoryProduct = inventoryMaterialRepository.GetAllInventoryMaterial();
                            foreach (var item in model.DetailList)
                            {
                                var inventoryProduct = list_inventoryProduct.Where(x => x.WarehouseId == PhysicalInventoryMaterial.WarehouseId && x.MaterialId == item.MaterialId).ToList();
                                if (!string.IsNullOrEmpty(item.LoCode) && item.LoCode != "")
                                    inventoryProduct = inventoryProduct.Where(x => x.LoCode == item.LoCode).ToList();
                                else
                                    inventoryProduct = inventoryProduct.Where(x => x.LoCode == null || x.LoCode == "").ToList();

                                if (item.ExpiryDate == null)
                                    inventoryProduct = inventoryProduct.Where(x => x.ExpiryDate == null).ToList();
                                else
                                    inventoryProduct = inventoryProduct.Where(x => x.ExpiryDate == item.ExpiryDate).ToList();

                                double QuantityInInventory = 0;
                                if (inventoryProduct.Count() > 0)
                                {
                                    var ii = inventoryProduct.FirstOrDefault();
                                    QuantityInInventory = ii.Quantity.Value;
                                    ii.Quantity = item.QuantityRemaining;
                                    inventoryMaterialRepository.UpdateInventoryMaterial(ii);
                                }

                                PhysicalInventoryMaterialDetailList.Add(new PhysicalInventoryMaterialDetail
                                {
                                    CreatedDate = DateTime.Now,
                                    CreatedUserId = WebSecurity.CurrentUserId,
                                    IsDeleted = false,
                                    Note = item.Note,
                                    MaterialId = item.MaterialId,
                                    QuantityInInventory = QuantityInInventory,
                                    QuantityRemaining = item.QuantityRemaining,
                                    QuantityDiff = item.QuantityRemaining - QuantityInInventory,
                                    LoCode = item.LoCode,
                                    ExpiryDate = item.ExpiryDate
                                });
                            }
                        }

                        PhysicalInventoryMaterialRepository.InsertPhysicalInventoryMaterial(PhysicalInventoryMaterial, PhysicalInventoryMaterialDetailList);

                        //cập nhật lại mã kiểm kho
                        PhysicalInventoryMaterial.Code = Erp.BackOffice.Helpers.Common.GetOrderNo("PhysicalInventoryMaterial", model.Code);
                        PhysicalInventoryMaterialRepository.UpdatePhysicalInventoryMaterial(PhysicalInventoryMaterial);
                        Erp.BackOffice.Helpers.Common.SetOrderNo("PhysicalInventoryMaterial");
                        //ghi log 
                        Erp.BackOffice.Controllers.HomeController.WriteLog(PhysicalInventoryMaterial.Id, PhysicalInventoryMaterial.Code, "đã tạo phiếu kiểm kho vật tư", "PhysicalInventoryMaterial/Detail/" + PhysicalInventoryMaterial.Id, Helpers.Common.CurrentUser.BranchId.Value);

                        scope.Complete();

                    }
                    catch (DbUpdateException)
                    {
                        return Content("Fail");
                    }
                }
                TempData[Globals.SuccessMessageKey] = App_GlobalResources.Wording.InsertSuccess;
                return RedirectToAction("Index");
            }
            return View(model);
        }

        public ActionResult Detail(int? Id)
        {
            var PhysicalInventory = PhysicalInventoryMaterialRepository.GetAllvwPhysicalInventoryMaterial()
                .Where(item => item.Id == Id).FirstOrDefault();
            if (PhysicalInventory != null)
            {
                var model = new PhysicalInventoryMaterialViewModel();
                AutoMapper.Mapper.Map(PhysicalInventory, model);

                var detailList = PhysicalInventoryMaterialRepository.GetAllvwPhysicalInventoryMaterialDetail(Id.Value);
                model.DetailList = detailList.Select(item => new PhysicalInventoryMaterialDetailViewModel
                {
                    Note = item.Note,
                    MaterialId = item.MaterialId,
                    MaterialCode = item.MaterialCode,
                    MaterialName = item.MaterialName,
                    PhysicalInventoryMaterialId = item.PhysicalInventoryMaterialId,
                    QuantityInInventory = item.QuantityInInventory,
                    QuantityRemaining = item.QuantityRemaining,
                    QuantityDiff = item.QuantityDiff,
                    CategoryCode = item.CategoryCode,
                    ExpiryDate = item.ExpiryDate,
                    LoCode = item.LoCode
                })
                .OrderBy(item => item.CategoryCode)
                .ThenBy(item => item.MaterialCode)
                .ToList();

                return View(model);
            }

            if (Request.UrlReferrer != null)
                return Redirect(Request.UrlReferrer.AbsoluteUri);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult Delete(int? id)
        {
            using (var scope = new TransactionScope(TransactionScopeOption.Required,
                               new System.TimeSpan(0, 30, 0)))
            {
                try
                {
                    string idDeleteAll = Request["DeleteId-checkbox"];
                    string[] arrDeleteId = idDeleteAll.Split(',');
                    for (int i = 0; i < arrDeleteId.Count(); i++)
                    {
                        var item = PhysicalInventoryMaterialRepository.GetPhysicalInventoryMaterialById(int.Parse(arrDeleteId[i], CultureInfo.InvariantCulture));
                        if (item != null)
                        {
                            var detailList = PhysicalInventoryMaterialRepository.GetAllPhysicalInventoryMaterialDetail(item.Id).ToList();
                            var list_inventoryProduct = inventoryMaterialRepository.GetAllInventoryMaterial();
                            foreach (var detail in detailList)
                            {
                                //lấy inventory của sản phẩm trong kho
                                var inventoryProduct = list_inventoryProduct.Where(x => x.WarehouseId == item.WarehouseId && x.MaterialId == detail.MaterialId).ToList();
                                if (!string.IsNullOrEmpty(detail.LoCode))
                                    inventoryProduct = inventoryProduct.Where(x => x.LoCode == detail.LoCode).ToList();
                                else
                                    inventoryProduct = inventoryProduct.Where(x => x.LoCode == null).ToList();

                                if (detail.ExpiryDate == null)
                                    inventoryProduct = inventoryProduct.Where(x => x.ExpiryDate == null).ToList();
                                else
                                    inventoryProduct = inventoryProduct.Where(x => x.ExpiryDate == detail.ExpiryDate).ToList();

                                double? quantity_remaining_difference = detail.QuantityInInventory - detail.QuantityRemaining;
                                if (inventoryProduct.Count() > 0)
                                {
                                    var ii = inventoryProduct.FirstOrDefault();
                                    ii.Quantity += quantity_remaining_difference;
                                    ii.ModifiedDate = DateTime.Now;
                                    ii.ModifiedUserId = WebSecurity.CurrentUserId;
                                    inventoryMaterialRepository.UpdateInventoryMaterial(ii);
                                }
                                //cập nhật SL inevntory theo sự chênh lệch SL lúc ban đầu được ghi nhận và SL cập nhật

                                //cập nhật tình trạng chi tiết kiểm kho thành xóa
                                detail.IsDeleted = true;
                                PhysicalInventoryMaterialRepository.UpdatePhysicalInventoryMaterialDetail(detail);
                            }

                            item.IsDeleted = true;
                            PhysicalInventoryMaterialRepository.UpdatePhysicalInventoryMaterial(item);
                        }
                    }
                    scope.Complete();

                }
                catch (DbUpdateException)
                {
                    TempData[Globals.FailedMessageKey] = App_GlobalResources.Error.RelationError;
                    return RedirectToAction("Index");
                }
            }
            TempData[Globals.SuccessMessageKey] = App_GlobalResources.Wording.DeleteSuccess;
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult Exchange(int Id)
        {

            var PhysicalInventory = PhysicalInventoryMaterialRepository.GetvwPhysicalInventoryMaterialById(Id);
            var model = new PhysicalInventoryMaterial();
            if (PhysicalInventory == null)
            {
                TempData[Globals.FailedMessageKey] = App_GlobalResources.Wording.NotfoundObject;
                return RedirectToAction("Index");
            }
            using (var scope = new TransactionScope(TransactionScopeOption.Required,
                               new System.TimeSpan(0, 30, 0)))
            {
                try
                {
                    AutoMapper.Mapper.Map(PhysicalInventory, model);
                    List<MaterialOutboundDetail> outboundDetails = new List<MaterialOutboundDetail>();
                    List<MaterialInboundDetail> inboundDetails = new List<MaterialInboundDetail>();
                    var outbounds = MaterialOutboundRepository.GetAllMaterialOutbound().Where(item => item.Code == PhysicalInventory.MaterialOutboundCode);
                    if (outbounds.Count() > 0)
                    {
                        //Xóa chi tiết xuất cũ
                        var outboundDetails_old = MaterialOutboundRepository.GetAllMaterialOutboundDetailByOutboundId(outbounds.FirstOrDefault().Id).Select(item => item.Id).ToList();
                        foreach (var item in outboundDetails_old)
                        {
                            MaterialOutboundRepository.DeleteMaterialOutboundDetail(item);
                        }
                        MaterialOutboundRepository.DeleteMaterialOutbound(outbounds.FirstOrDefault().Id);
                    }
                    var inbounds = MaterialInboundRepository.GetAllMaterialInbound().Where(item => item.Code == PhysicalInventory.MaterialInboundCode);
                    if (inbounds.Count() > 0)
                    {

                        //Xóa chi tiết nhập cũ
                        var inboundDetails_old = MaterialInboundRepository.GetAllMaterialInboundDetailByInboundId(inbounds.FirstOrDefault().Id).Select(item => item.Id).ToList();
                        foreach (var item in inboundDetails_old)
                        {
                            MaterialInboundRepository.DeleteMaterialInboundDetail(item);
                        }
                        MaterialInboundRepository.DeleteMaterialInbound(inbounds.FirstOrDefault().Id);
                    }

                    var listDetail = PhysicalInventoryMaterialRepository.GetAllPhysicalInventoryMaterialDetail(Id).Where(x => x.QuantityInInventory != x.QuantityRemaining).ToList();
                    foreach (var item in listDetail)
                    {
                        //var product = ProductRepository.GetProductById(item.MaterialId);
                        var material = MaterialRepository.GetMaterialById(item.MaterialId);
                        if (item.QuantityDiff < 0) //Chênh lệch dương thì thuộc về xuất
                        {
                            outboundDetails.Add(
                                new MaterialOutboundDetail
                                {
                                    IsDeleted = false,
                                    CreatedDate = DateTime.Now,
                                    CreatedUserId = Helpers.Common.CurrentUser.Id,
                                    ModifiedDate = DateTime.Now,
                                    ModifiedUserId = Helpers.Common.CurrentUser.Id,
                                    Price = material.PriceOutBound,
                                    MaterialId = material.Id,
                                    Quantity = Math.Abs(item.QuantityDiff),
                                    ExpiryDate = item.ExpiryDate,
                                    LoCode = item.LoCode
                                }
                            );

                        }
                        else if (item.QuantityDiff > 0) //Chênh lệch âm thì thuộc về nhập
                        {
                            inboundDetails.Add(
                                new MaterialInboundDetail
                                {
                                    IsDeleted = false,
                                    CreatedDate = DateTime.Now,
                                    CreatedUserId = Helpers.Common.CurrentUser.Id,
                                    ModifiedDate = DateTime.Now,
                                    ModifiedUserId = Helpers.Common.CurrentUser.Id,
                                    Price = material.PriceInbound,
                                    MaterialId = material.Id,
                                    Quantity = Math.Abs(item.QuantityDiff),
                                    LoCode = item.LoCode,
                                    ExpiryDate = item.ExpiryDate
                                }
                            );
                        }
                    }

                    if (outboundDetails.Count != 0)
                    {
                        var outbound = new MaterialOutbound
                        {
                            IsDeleted = false,
                            CreatedDate = DateTime.Now,
                            CreatedUserId = Helpers.Common.CurrentUser.Id,
                            ModifiedDate = DateTime.Now,
                            ModifiedUserId = Helpers.Common.CurrentUser.Id,
                            BranchId = model.BranchId,
                            IsDone = true,
                            Type = "PhysicalInventory",
                            TotalAmount = outboundDetails.Sum(x => (decimal)x.Quantity * x.Price),
                            WarehouseSourceId = PhysicalInventory.WarehouseId,
                            PhysicalInventoryId = PhysicalInventory.Id,
                            Note = "Xuất kho kiểm kê vật tư"
                        };

                        MaterialOutboundRepository.InsertMaterialOutbound(outbound);

                        foreach (var item in outboundDetails)
                        {
                            item.MaterialOutboundId = outbound.Id;
                            item.IsDeleted = false;
                            item.CreatedUserId = WebSecurity.CurrentUserId;
                            item.ModifiedUserId = WebSecurity.CurrentUserId;
                            item.CreatedDate = DateTime.Now;
                            item.ModifiedDate = DateTime.Now;
                            MaterialOutboundRepository.InsertMaterialOutboundDetail(item);
                        }

                        //cập nhật lại mã xuất kho
                        outbound.Code = Erp.BackOffice.Helpers.Common.GetOrderNo("MaterialOutbound", model.Code);
                        MaterialOutboundRepository.UpdateMaterialOutbound(outbound);
                        Erp.BackOffice.Helpers.Common.SetOrderNo("MaterialOutbound");
                        foreach (var item in listDetail.Where(x => x.QuantityDiff < 0))
                        {
                            item.ModifiedDate = DateTime.Now;
                            item.ModifiedUserId = Helpers.Common.CurrentUser.Id;
                            item.ReferenceVoucher = outbound.Code;
                            PhysicalInventoryMaterialRepository.UpdatePhysicalInventoryMaterialDetail(item);
                        }
                    }

                    if (inboundDetails.Count != 0)
                    {
                        var inbound = new MaterialInbound
                        {
                            IsDeleted = false,
                            CreatedDate = DateTime.Now,
                            CreatedUserId = Helpers.Common.CurrentUser.Id,
                            ModifiedDate = DateTime.Now,
                            ModifiedUserId = Helpers.Common.CurrentUser.Id,
                            BranchId = model.BranchId,
                            IsDone = true,
                            Type = "PhysicalInventory",
                            TotalAmount = inboundDetails.Sum(x => (decimal)x.Quantity * x.Price),
                            WarehouseDestinationId = PhysicalInventory.WarehouseId,
                            PhysicalInventoryId = PhysicalInventory.Id,
                            Note = "Nhập kho kiểm kê vật tư"
                        };

                        MaterialInboundRepository.InsertMaterialInbound(inbound);

                        //Thêm chi tiết phiếu nhập
                        foreach (var item in inboundDetails)
                        {
                            item.MaterialInboundId = inbound.Id;
                            item.IsDeleted = false;
                            item.CreatedUserId = WebSecurity.CurrentUserId;
                            item.ModifiedUserId = WebSecurity.CurrentUserId;
                            item.CreatedDate = DateTime.Now;
                            item.ModifiedDate = DateTime.Now;
                            MaterialInboundRepository.InsertMaterialInboundDetail(item);
                        }

                        //cập nhật lại mã xuất kho
                        inbound.Code = Erp.BackOffice.Helpers.Common.GetOrderNo("MaterialInbound", model.Code);
                        MaterialInboundRepository.UpdateMaterialInbound(inbound);
                        Erp.BackOffice.Helpers.Common.SetOrderNo("MaterialInbound");
                        foreach (var item in listDetail.Where(x => x.QuantityDiff > 0))
                        {
                            item.ModifiedDate = DateTime.Now;
                            item.ModifiedUserId = Helpers.Common.CurrentUser.Id;
                            item.ReferenceVoucher = inbound.Code;
                            PhysicalInventoryMaterialRepository.UpdatePhysicalInventoryMaterialDetail(item);
                        }
                    }

                    model.IsExchange = true;
                    model.ModifiedDate = DateTime.Now;
                    model.ModifiedUserId = Helpers.Common.CurrentUser.Id;
                    PhysicalInventoryMaterialRepository.UpdatePhysicalInventoryMaterial(model);
                    scope.Complete();
                }
                catch (DbUpdateException)
                {
                    return Content("Fail");
                }
            }
            TempData[Globals.SuccessMessageKey] = App_GlobalResources.Wording.Success;
            return RedirectToAction("Detail", new { Id = PhysicalInventory.Id });
        }


        public ActionResult Print(int? Id)
        {
            var PhysicalInventory = PhysicalInventoryMaterialRepository.GetAllvwPhysicalInventoryMaterial()
                .Where(item => item.Id == Id).FirstOrDefault();

            //lấy logo công ty
            var logo = Erp.BackOffice.Helpers.Common.GetSetting("LogoCompany");
            var company = Erp.BackOffice.Helpers.Common.GetSetting("companyName");
            var address = Erp.BackOffice.Helpers.Common.GetSetting("addresscompany");
            var phone = Erp.BackOffice.Helpers.Common.GetSetting("phonecompany");
            var fax = Erp.BackOffice.Helpers.Common.GetSetting("faxcompany");
            var bankcode = Erp.BackOffice.Helpers.Common.GetSetting("bankcode");
            var bankname = Erp.BackOffice.Helpers.Common.GetSetting("bankname");
            var ImgLogo = "<div class=\"logo\"><img src=" + logo + " height=\"73\" /></div>";

            if (PhysicalInventory != null && PhysicalInventory.IsDeleted != true)
            {
                var model = new TemplatePrintViewModel();

                //lấy danh sách sản phẩm 
                var user = userRepository.GetUserById(PhysicalInventory.CreatedUserId.Value);
                var detailList = PhysicalInventoryMaterialRepository.GetAllvwPhysicalInventoryMaterialDetail(Id.Value).AsEnumerable()
                    .Select(item => new PhysicalInventoryMaterialDetailViewModel
                    {
                        Note = item.Note,
                        MaterialId = item.MaterialId,
                        MaterialCode = item.MaterialCode,
                        MaterialName = item.MaterialName,
                        PhysicalInventoryMaterialId = item.PhysicalInventoryMaterialId,
                        QuantityInInventory = item.QuantityInInventory,
                        QuantityRemaining = item.QuantityRemaining,
                        QuantityDiff = item.QuantityDiff,
                        CategoryCode = item.CategoryCode,
                        ExpiryDate = item.ExpiryDate,
                        LoCode = item.LoCode
                    })
                .OrderBy(item => item.CategoryCode)
                .ThenBy(item => item.MaterialCode)
                .ToList();
                var groupList = detailList.GroupBy(x => new { x.CategoryCode }, (key, group) => new
                {
                    CategoryCode = key.CategoryCode,
                    ProductList = group.ToList()
                });
                var ListRow = "";
                int n = 1;
                foreach (var item in groupList)
                {
                    var Row = "<tr style=\"background:#eee\"><td class=\"text-left\" colspan=\"7\"><b>" + item.CategoryCode + "</b></td></tr>";
                    foreach (var g in item.ProductList)
                    {

                        Row += "<tr>"
                           + "<td class=\"text-center\">" + n++ + "</td>"
                           + "<td class=\"text-left\">" + g.MaterialCode + "-" + g.MaterialName + "</td>"
                            + "<td>" + g.LoCode + "</td>"
                            + "<td>" + (g.ExpiryDate == null ? "" : g.ExpiryDate.Value.ToString("dd/MM/yyyy")) + "</td>"
                           + "<td>" + Erp.BackOffice.Helpers.Common.PhanCachHangNgan2(g.QuantityInInventory) + "</td>"
                           + "<td>" + Erp.BackOffice.Helpers.Common.PhanCachHangNgan2(g.QuantityRemaining) + "</td>"
                           + "<td>" + Erp.BackOffice.Helpers.Common.PhanCachHangNgan2(g.QuantityDiff) + "</td></tr>";
                    }

                    ListRow += Row;
                }



                var style = "<style>.invoice-detail{ width:100%;margin-top: 10px;border-spacing: 0px;}"
                    + ".invoice-detail th{border: 1px solid #000;border-right: none;padding: 5px;}"
                    + " .invoice-detail tr th:last-child {border-right: 1px solid #000;}"
                    + " .invoice-detail td{padding: 5px 5px; border-bottom: 1px solid #000; border-left: 1px solid #000; height: 15px;font-size: 12px;}"
                    + " .invoice-detail tr td:last-child {border-right: 1px solid #000;}"
                    + ".invoice-detail tbody tr:last-child td {border-bottom: 1px solid #000;}"
                    + " .invoice-detail tfoot td{font-weight:bold;border-bottom: 1px solid #000;}"
                    + " .invoice-detail tfoot tr:first-child td{border-top: 1px solid #000;}"
                    + ".text-center{text-align:center;}"
                    + ".text-right{text-align:right;}"
                    + ".text-left{text-align:left;}"
                    + " .logo{ width: 100px;float: left; margin: 0 20px;height: 100px;line-height: 100px;}"
                    + ".logo img{ width:100%;vertical-align: middle;}"
                    + "</style>";

                var table = style + "<table class=\"invoice-detail\"><thead><tr> <th>STT</th> <th>Tên sản phẩm</th><th>Số Lô</th><th>Hạn dùng</th><th>SL hệ thống</th><th>SL thực tế</th><th>SL chênh lệch</th></tr></thead><tbody>"
                             + ListRow
                             + "</tbody></table>";

                //lấy template phiếu xuất.
                var template = templatePrintRepository.GetAllTemplatePrint().Where(x => x.Code.Contains("PhysicalInventoryMaterial")).OrderByDescending(x => x.CreatedDate).FirstOrDefault();
                model.Content = template.Content;
                model.Content = model.Content.Replace("{Note}", PhysicalInventory.Note);
                model.Content = model.Content.Replace("{WarehouseName}", PhysicalInventory.WarehouseName);
                model.Content = model.Content.Replace("{MaterialInboundCode}", PhysicalInventory.MaterialInboundCode);
                model.Content = model.Content.Replace("{MaterialOutboundCode}", PhysicalInventory.MaterialOutboundCode);
                model.Content = model.Content.Replace("{Code}", PhysicalInventory.Code);
                model.Content = model.Content.Replace("{Day}", PhysicalInventory.CreatedDate.Value.Day.ToString());
                model.Content = model.Content.Replace("{Month}", PhysicalInventory.CreatedDate.Value.Month.ToString());
                model.Content = model.Content.Replace("{Year}", PhysicalInventory.CreatedDate.Value.Year.ToString());
                model.Content = model.Content.Replace("{DataTable}", table);
                model.Content = model.Content.Replace("{Logo}", ImgLogo);
                model.Content = model.Content.Replace("{System.CompanyName}", company);
                model.Content = model.Content.Replace("{System.AddressCompany}", address);
                model.Content = model.Content.Replace("{System.PhoneCompany}", phone);
                model.Content = model.Content.Replace("{System.Fax}", fax);
                model.Content = model.Content.Replace("{System.BankCodeCompany}", bankcode);
                model.Content = model.Content.Replace("{System.BankNameCompany}", bankname);
                return View(model);
            }

            if (Request.UrlReferrer != null)
                return Redirect(Request.UrlReferrer.AbsoluteUri);
            return RedirectToAction("Index");
        }

    }
}