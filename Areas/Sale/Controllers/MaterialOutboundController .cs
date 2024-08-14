using System.Globalization;
using Erp.BackOffice.Sale.Models;
using Erp.BackOffice.Filters;
using Erp.Domain.Sale.Entities;
using Erp.Domain.Interfaces;
using Erp.Domain.Sale.Interfaces;
using Erp.Domain.Sale.Repositories;
using Erp.Domain.Staff.Interfaces;
using Erp.Domain.Staff.Repositories;
using Erp.Domain.Account.Interfaces;
using Erp.Domain.Account.Repositories;
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
using Erp.BackOffice.App_GlobalResources;
using Erp.Domain.Entities;
using System.Xml.Linq;
using Erp.BackOffice.Account.Controllers;
using Erp.BackOffice.Account.Models;
using System.Drawing;
using GenCode128;
using System.IO;
using Erp.Domain.Helper;
using System.Transactions;
using Erp.BackOffice.Areas.Cms.Models;
using System.Web;
using System.Data;
using ClosedXML.Excel;

namespace Erp.BackOffice.Sale.Controllers
{
    [Authorize]
    [InitializeSimpleMembership]
    [Erp.BackOffice.Helpers.NoCacheHelper]
    public class MaterialOutboundController : Controller
    {
        private readonly IWarehouseRepository WarehouseRepository;
        private readonly IWarehouseLocationItemRepository warehouseLocationItemRepository;
        //private readonly IMaterialInvoiceRepository MaterialInvoiceRepository;
        private readonly IPurchaseOrderRepository PurchaseOrderRepository;
        private readonly IInventoryRepository inventoryRepository;
        private readonly IMaterialOrServiceRepository MaterialRepository;
        private readonly IMaterialOutboundRepository materialOutboundRepository;
        private readonly IMaterialInboundRepository MaterialInboundRepository;
        private readonly IStaffsRepository staffRepository;
        private readonly IUserRepository userRepository;
        private readonly ITemplatePrintRepository templatePrintRepository;
        private readonly IQueryHelper QueryHelper;
        private readonly ICustomerRepository customerRepository;
        private readonly ISettingRepository settingRepository;
        private readonly IRequestInboundRepository requestInboundRepository;
        private readonly IBranchRepository branchRepository;
        private readonly ITransactionLiabilitiesRepository transactionLiabilitiesRepository;
        public MaterialOutboundController(
             IWarehouseRepository _Warehouse
            , IWarehouseLocationItemRepository _WarehouseLocationItem
            , IInventoryRepository _Inventory

            , IPurchaseOrderRepository _PurchaseOrder
            , IMaterialOrServiceRepository _Material
            , IMaterialOutboundRepository _MaterialOutbound
            , IMaterialInboundRepository _MaterialInbound
            , IStaffsRepository _staff
            , IUserRepository _user
            , IQueryHelper _QueryHelper
            , ITemplatePrintRepository _templatePrint
            , ICustomerRepository customer
             , ISettingRepository _Setting
            , IRequestInboundRepository requestInbound
            , IBranchRepository branch
            )
        {
            WarehouseRepository = _Warehouse;
            warehouseLocationItemRepository = _WarehouseLocationItem;
            inventoryRepository = _Inventory;

            PurchaseOrderRepository = _PurchaseOrder;
            MaterialRepository = _Material;
            materialOutboundRepository = _MaterialOutbound;
            MaterialInboundRepository = _MaterialInbound;
            staffRepository = _staff;
            userRepository = _user;
            QueryHelper = _QueryHelper;
            templatePrintRepository = _templatePrint;
            customerRepository = customer;
            settingRepository = _Setting;
            requestInboundRepository = requestInbound;
            branchRepository = branch;
        }

        #region Index
        public ViewResult Index(string txtCode, string txtMinAmount, string txtMaxAmount, string txtWarehouseDestination, int? warehouseSourceId, string startDate, string endDate, string txtProductCode, string Status)
        {
            var warehouseList = WarehouseRepository.GetvwAllWarehouse().AsEnumerable();
            var user = userRepository.GetByvwUserName(Helpers.Common.CurrentUser.UserName);
            IEnumerable<MaterialOutboundViewModel> q = materialOutboundRepository.GetAllvwMaterialOutboundFull()
                //.Where(x => x.BranchId == Helpers.Common.CurrentUser.BranchId)
                .Select(item => new MaterialOutboundViewModel
                {
                    Id = item.Id,
                    CreatedUserId = item.CreatedUserId,
                    CreatedDate = item.CreatedDate,
                    ModifiedUserId = item.ModifiedUserId,
                    ModifiedDate = item.ModifiedDate,
                    Code = item.Code,
                    TotalAmount = item.TotalAmount,

                    WarehouseDestinationName = item.WarehouseDestinationName,
                    WarehouseSourceName = item.WarehouseSourceName,

                    Type = item.Type,
                    IsArchive = item.IsArchive,
                    WarehouseSourceId = item.WarehouseSourceId,

                    IsDeleted = item.IsDeleted,
                    BranchId = item.BranchId
                }).OrderByDescending(m => m.Id);

            //Tìm những phiếu xuất có chứa mã sp
            if (!string.IsNullOrEmpty(txtProductCode))
            {
                txtProductCode = txtProductCode.Trim();
                var materialListId = MaterialRepository.GetAllvwMaterial()
                    .Where(item => item.Code == txtProductCode).Select(item => item.Id).ToList();

                if (materialListId.Count > 0)
                {
                    List<int> listMaterialOutboundId = new List<int>();
                    foreach (var id in materialListId)
                    {
                        var list = materialOutboundRepository.GetAllvwMaterialOutboundDetailByMaterialId(id)
                            .Select(item => item.MaterialOutboundId.Value).Distinct().ToList();

                        listMaterialOutboundId.AddRange(list);
                    }

                    q = q.Where(item => listMaterialOutboundId.Contains(item.Id));
                }
            }

            if (!string.IsNullOrEmpty(txtCode))
            {
                q = q.Where(x => x.Code.ToLowerOrEmpty().Contains(txtCode.Trim().ToLower())
                    //|| x.WarehouseSourceId == warehouseSourceId
                    );
            }
            if (warehouseSourceId != null && warehouseSourceId.Value > 0)
            {
                q = q.Where(x => x.WarehouseSourceId == warehouseSourceId);
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


            //if (!Erp.BackOffice.Filters.SecurityFilter.IsAdmin() && !Erp.BackOffice.Filters.SecurityFilter.IsKeToan() && !Erp.BackOffice.Filters.SecurityFilter.IsQLKhoTong())
            //{
            //    q = q.Where(x => ("," + user.DrugStore + ",").Contains("," + x.BranchId + ",") == true);
            //}
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
            TempData["DataMaterialOutBound"] = q.ToList();
            return View(q);
        }
        #endregion

        #region Xuất Excel
        public ActionResult ExportMaterialOutBound(bool ExportExcel = false)
        {
            var data = TempData["DataMaterialOutBound"] as List<MaterialOutboundViewModel>;

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
            model.Content = model.Content.Replace("{DataTable}", buildHtmlMaterialOutBound(data));
            model.Content = model.Content.Replace("{System.Logo}", ImgLogo);
            model.Content = model.Content.Replace("{System.CompanyName}", company);
            model.Content = model.Content.Replace("{System.AddressCompany}", address);
            model.Content = model.Content.Replace("{System.PhoneCompany}", phone);
            model.Content = model.Content.Replace("{System.Fax}", fax);
            model.Content = model.Content.Replace("{System.BankCodeCompany}", bankcode);
            model.Content = model.Content.Replace("{System.BankNameCompany}", bankname);
            model.Content = model.Content.Replace("{PrintDate}", DateTime.Now.ToString("dd/MM/yyyy HH:mm"));
            model.Content = model.Content.Replace("{Title}", "Danh sách phiếu xuất kho");

            Response.AppendHeader("content-disposition", "attachment;filename=" + "DS_PhieuXuat" + DateTime.Now.ToString("dd_MM_yyyy") + ".xls");
            Response.Charset = "";
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.Write(model.Content);
            Response.End();

            return View(model);
        }

        string buildHtmlMaterialOutBound(List<MaterialOutboundViewModel> detailList)
        {
            string detailLists = "<table border=\"1\" class=\"invoice-detail\">";
            detailLists += "<thead>";
            detailLists += "	<tr>";
            detailLists += "		<th>STT</th>";
            detailLists += "		<th>Ngày tạo</th>";
            //detailLists += "		<th>Người tạo</th>";
            detailLists += "		<th>Loại phiếu</th>";
            detailLists += "		<th >Mã chứng từ</th>";
            detailLists += "		<th>Chứng từ gốc</th>";
            detailLists += "		<th>Tổng tiền</th>";
            detailLists += "		<th>Xuất từ kho</th>";
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
                + "<td>" + item.Type + "</td>"
                + "<td>" + item.Code + "</td>"
                + "<td>" + item.InvoiceCode + "</td>"
                + "<td>" + CommonSatic.ToCurrencyStr(item.TotalAmount, null) + "</td>"
                + "<td>" + item.WarehouseSourceName + "</td>"
                + "<td>" + item.ModifiedDate + "</td>"
                + "<td>" + isArchive + "</td>"
                + "</tr>";
            }

            detailLists += "</tbody>";
            detailLists += "<tfoot style=\"font-weight:bold\">";
            detailLists += "</tfoot></table>";
            return detailLists;
        }
        #endregion

        #region CreateFromRequest
        public ActionResult CreateFromRequest(int? WarehouseSourceId, int? RequestInboundId)
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

            var categoryList = SqlHelper.QuerySP<CategoryViewModel>("GetAllDanhMucKho").Where(x => x.CategoryValue == "VT").ToList();
            ViewBag.categoryList = categoryList;
            //string image_folder = Helpers.Common.GetSetting("product-image-folder");
            var warehouseList = WarehouseRepository.GetvwAllWarehouse().AsEnumerable();
            warehouseList = warehouseList.Where(x => x.Categories == "VT" ).ToList();
            if(intBrandID > 0)
            {
                warehouseList = warehouseList.Where(x => x.BranchId == intBrandID).ToList();
            }    
            var user = userRepository.GetByvwUserName(Helpers.Common.CurrentUser.UserName);
            if (!Erp.BackOffice.Filters.SecurityFilter.IsAdmin() && !Erp.BackOffice.Filters.SecurityFilter.IsKeToan() && Helpers.Common.CurrentUser.UserTypeId != 2039)
            {
                warehouseList = warehouseList.Where(x => ("," + user.WarehouseId + ",").Contains("," + x.Id + ",") == true);
            }
            var GetwarhouseID = requestInboundRepository.GetvwRequestInboundById(RequestInboundId.Value).WarehouseDestinationId.ToString();
            var _warehouseList = warehouseList.Select(item => new SelectListItem
            {
                Text = item.Name + " (" + item.BranchName + ")",
                Value = item.Id.ToString()
            });
            ViewBag.warehouseSourceList = _warehouseList.Where(x=>x.Value!=GetwarhouseID);
            ViewBag.warehouseDestinationList = warehouseList
                .Select(item => new SelectListItem
                {
                    Text = item.Name,
                    Value = item.Id.ToString()
                });

            MaterialOutboundViewModel model = new MaterialOutboundViewModel();
            model.requestInboundViewModel = new RequestInboundViewModel();
            model.CreatedUserName = Erp.BackOffice.Helpers.Common.CurrentUser.FullName;
            model.CreatedDate = DateTime.Now;
            model.TotalAmount = 0;
            //model.CreatedStaffId = CreatedStaffId;
            model.DetailList = new List<MaterialOutboundDetailViewModel>();


            if (WarehouseSourceId != null && WarehouseSourceId > 0)
            {
                model.WarehouseSourceId = WarehouseSourceId;
                var materialList = Domain.Helper.SqlHelper.QuerySP<InventoryMaterialViewModel>("spSale_Get_InventoryMaterial", new { WarehouseId = WarehouseSourceId, HasQuantity = "1", MaterialCode = "", MaterialName = "", CategoryCode = "", ProductGroup = "", BranchId = "0", LoCode = "", MaterialId = "", ExpiryDate = "" });
                ViewBag.materialList = materialList;
                if (RequestInboundId != null)
                {
                    //var RequestInboundView = new RequestInboundViewModel();
                    var RequestInbound = requestInboundRepository.GetvwRequestInboundById(RequestInboundId.Value);

                    if (RequestInbound == null)
                    {
                        TempData[Globals.FailedMessageKey] = Wording.NotfoundObject;
                        return RedirectToAction("Create");
                    }

                    model.WarehouseDestinationId = RequestInbound.WarehouseDestinationId;

                    var list_request_detail = requestInboundRepository.GetAllvwRequestInboundDetailsByInvoiceId(RequestInbound.Id).ToList();
                    int i = -1;
                    foreach (var item in list_request_detail)
                    {
                        var inventory = materialList.Where(x => x.MaterialId == item.ProductId).OrderBy(x => x.ExpiryDate);
                        double? quantity = 0;
                        foreach (var inv in inventory)
                        {
                            i++;
                            quantity += inv.Quantity;
                            if (item.Quantity >= quantity)
                            {
                                var add_item = new MaterialOutboundDetailViewModel();
                                add_item.MaterialId = item.ProductId;
                                add_item.Price = item.Price;
                                add_item.Quantity = inv.Quantity;
                                add_item.QuantityInInventory = 0;
                                add_item.MaterialName = item.MaterialName;
                                add_item.MaterialCode = item.MaterialCode;
                                add_item.Unit = item.Unit;
                                add_item.OrderNo = i;
                                add_item.LoCode = inv.LoCode;
                                add_item.ExpiryDate = inv.ExpiryDate;
                                model.DetailList.Add(add_item);
                                if (item.Quantity == quantity)
                                    break;
                            }
                            else
                            {
                                var sl_cu = quantity - inv.Quantity;
                                var sl_can = item.Quantity - sl_cu;
                                var add_item = new MaterialOutboundDetailViewModel();
                                add_item.MaterialId = item.ProductId;
                                add_item.Price = item.Price;
                                add_item.Quantity = sl_can;
                                add_item.QuantityInInventory = inv.Quantity - sl_can;
                                add_item.MaterialName = item.MaterialName;
                                add_item.MaterialCode = item.MaterialCode;
                                add_item.Unit = item.Unit;
                                add_item.OrderNo = i;
                                add_item.LoCode = inv.LoCode;
                                add_item.ExpiryDate = inv.ExpiryDate;
                                model.DetailList.Add(add_item);
                                break;
                            }

                        }
                    }

                    var q = requestInboundRepository.GetRequestInboundById(RequestInboundId.Value);
                    if (q.Status == "ApprovedASM")
                    {
                        q.Status = "ApprovedKT";
                        q.ModifiedUserId = WebSecurity.CurrentUserId;
                        requestInboundRepository.UpdateRequestInbound(q);
                        //run process
                        Crm.Controllers.ProcessController.Run("RequestInbound", "ApprovedKT", q.Id, q.CreatedUserId, null, q, q.BranchId.Value.ToString());
                    }
                }
            }

            return View(model);
        }

        [HttpPost]
        public ActionResult CreateFromRequest(MaterialOutboundViewModel model)
        {
            if (ModelState.IsValid)
            {
                //duyệt qua danh sách sản phẩm mới xử lý tình huống user chọn 2 sản phầm cùng id
                List<MaterialOutboundDetail> outboundDetails = new List<MaterialOutboundDetail>();
                model.DetailList.RemoveAll(x => x.Quantity <= 0);
                foreach (var group in model.DetailList)
                {
                    //var product = ProductRepository.GetProductById(group.Key.Value);

                    outboundDetails.Add(new MaterialOutboundDetail
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
                        LoCode = group.LoCode,
                        ExpiryDate = group.ExpiryDate
                    });
                }
                using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.Snapshot }))
                {
                    try
                    {
                        //thêm mới phiếu xuất kho theo đơn xuất kho ở trên
                        var materialOutbound = new Domain.Sale.Entities.MaterialOutbound();
                        AutoMapper.Mapper.Map(model, materialOutbound);
                        materialOutbound.IsDeleted = false;
                        materialOutbound.CreatedUserId = WebSecurity.CurrentUserId;
                        materialOutbound.ModifiedUserId = WebSecurity.CurrentUserId;
                        materialOutbound.CreatedDate = DateTime.Now;
                        materialOutbound.ModifiedDate = DateTime.Now;
                        if (model.RequestInboundId != null)
                        {
                            materialOutbound.Type = "internal";
                        }
                        var branch = WarehouseRepository.GetWarehouseById(materialOutbound.WarehouseSourceId.Value);
                        materialOutbound.BranchId = branch.BranchId;

                        materialOutbound.IsDone = false;
                        materialOutbound.TotalAmount = outboundDetails.Sum(item => (decimal)item.Quantity * item.Price);

                        materialOutboundRepository.InsertMaterialOutbound(materialOutbound);

                        foreach (var item in outboundDetails)
                        {
                            MaterialOutboundDetail materialOutboundDetail = new MaterialOutboundDetail();
                            materialOutboundDetail.MaterialId = item.MaterialId;
                            materialOutboundDetail.Price = item.Price;
                            materialOutboundDetail.Quantity = item.Quantity;
                            materialOutboundDetail.Unit = item.Unit;
                            materialOutboundDetail.LoCode = item.LoCode;
                            materialOutboundDetail.ExpiryDate = item.ExpiryDate;
                            materialOutboundDetail.IsDeleted = false;
                            materialOutboundDetail.CreatedUserId = WebSecurity.CurrentUserId;
                            materialOutboundDetail.ModifiedUserId = WebSecurity.CurrentUserId;
                            materialOutboundDetail.CreatedDate = DateTime.Now;
                            materialOutboundDetail.ModifiedDate = DateTime.Now;
                            materialOutboundDetail.MaterialOutboundId = materialOutbound.Id;
                            materialOutboundRepository.InsertMaterialOutboundDetail(materialOutboundDetail);

                        }
                        if (model.RequestInboundId != null)
                        {
                            var order = requestInboundRepository.GetRequestInboundById(model.RequestInboundId.Value);
                            order.OutboundId = materialOutbound.Id;
                            requestInboundRepository.UpdateRequestInbound(order);
                        }
                        if (model.RequestInboundId != null)
                        {
                            var order = requestInboundRepository.GetRequestInboundById(model.RequestInboundId.Value);
                            order.Status = "shipping";
                            order.ModifiedDate = DateTime.Now;
                            order.WarehouseSourceId = materialOutbound.WarehouseSourceId;
                            order.ModifiedUserId = WebSecurity.CurrentUserId;
                            order.ShipName = model.requestInboundViewModel.ShipName;
                            order.ShipPhone = model.requestInboundViewModel.ShipPhone;
                            requestInboundRepository.UpdateRequestInbound(order);
                            //Thêm vào quản lý chứng từ
                            TransactionController.Create(new TransactionViewModel
                            {
                                TransactionModule = "MaterialOutbound",
                                TransactionCode = materialOutbound.Code,
                                TransactionName = "Xuất kho"
                            });
                            //gửi notifications cho người được phân quyền.
                            Crm.Controllers.ProcessController.Run("RequestInbound", "shipping", order.Id, order.CreatedUserId, null, order, order.BranchId.Value.ToString());
                        }
                        //cập nhật lại mã xuất kho
                        materialOutbound.Code = Erp.BackOffice.Helpers.Common.GetOrderNo("materialOutbound", model.Code);
                        materialOutboundRepository.UpdateMaterialOutbound(materialOutbound);
                        Erp.BackOffice.Helpers.Common.SetOrderNo("materialOutbound");

                        //string prefixOutbound = Helpers.Common.GetSetting("prefixOrderNo_Outbound");
                        //materialOutbound.Code = Helpers.Common.GetCode(prefixOutbound, materialOutbound.Id);
                        //materialOutboundRepository.UpdateMaterialOutbound(materialOutbound);
                        //Thêm vào quản lý chứng từ
                        TransactionController.Create(new TransactionViewModel
                        {
                            TransactionModule = "MaterialOutbound",
                            TransactionCode = materialOutbound.Code,
                            TransactionName = "Xuất kho"
                        });
                        scope.Complete();
                    }
                    catch (DbUpdateException)
                    {
                        TempData[Globals.FailedMessageKey] = App_GlobalResources.Wording.Error;
                        return Content("Fail");
                    }
                }
                TempData[Globals.SuccessMessageKey] = App_GlobalResources.Wording.InsertSuccess;
                if (Request["IsPopup"] == "true" || Request["IsPopup"] == "True")
                {
                    return RedirectToAction("_ClosePopup", "Home", new { area = "", FunctionCallback = "ClosePopupAndReloadPage" });
                }
                //  return RedirectToAction("Index");
            }

            // phần xử lý cho việc valid sai
            var warehouseList = WarehouseRepository.GetAllWarehouse().AsEnumerable();

            ViewBag.warehouseSourceList = warehouseList.Where(x => ("," + Helpers.Common.CurrentUser.WarehouseId + ",").Contains("," + x.Id + ",") == true)
                   .Select(item => new SelectListItem
                   {
                       Text = item.Name,
                       Value = item.Id.ToString()
                   });

            ViewBag.warehouseDestinationList = warehouseList
                .Select(item => new SelectListItem
                {
                    Text = item.Name,
                    Value = item.Id.ToString()
                });

            return View(model);
        }
        #endregion

        #region Create
        public ActionResult Create(int? WarehouseSourceId)
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

            var categoryList = SqlHelper.QuerySP<CategoryViewModel>("GetAllDanhMucKho").Where(x => x.CategoryValue == "VT").ToList();
            if (intBrandID > 0)
            {
                categoryList = categoryList.Where(x => x.BranchId == intBrandID).ToList();
            }
            ViewBag.categoryList = categoryList;

            var categoryListXuatPhucVu = SqlHelper.QuerySP<CategoryViewModel>("GetAllDanhMucKho").Where(x => x.CategoryValue == "VT" && x.Id != WarehouseSourceId).ToList();
            ViewBag.categoryListXuatPhucVu = categoryListXuatPhucVu;

            MaterialOutboundTransferViewModel model = new MaterialOutboundTransferViewModel();
            model.TotalAmount = 0;
            model.Code = Erp.BackOffice.Helpers.Common.GetOrderNo("MaterialOutbound");
            model.WarehouseSourceId = WarehouseSourceId;
            //var materialList = Domain.Helper.SqlHelper.QuerySP<InventoryMaterialViewModel>("spSale_Get_InventoryMaterial", new { WarehouseId = WarehouseSourceId, HasQuantity = "1", MaterialCode = "", MaterialName = "", CategoryCode = "", ProductGroup = "", BranchId = "0", LoCode = "", MaterialId = "", ExpiryDate = "" });
            //ViewBag.materialList = materialList;
            return View(model);
        }

        [HttpPost]
        public ActionResult Create(MaterialOutboundTransferViewModel model)
        {
            if (ModelState.IsValid)
            {
                var MaterialOutbound = new Domain.Sale.Entities.MaterialOutbound();
                var check = Request["group_choice"];
                //duyệt qua danh sách sản phẩm mới xử lý tình huống user chọn 2 sản phầm cùng id
                List<MaterialOutboundDetail> outboundDetails = new List<MaterialOutboundDetail>();
                model.DetailList.RemoveAll(x => x.Quantity <= 0);
                using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.Snapshot }))
                {
                    try
                    {
                        foreach (var group in model.DetailList.Where(x => x.MaterialId > 0))
                        {
                            outboundDetails.Add(new MaterialOutboundDetail
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
                                LoCode = group.LoCode,
                                ExpiryDate = group.ExpiryDate
                            });
                        }

                        //thêm mới phiếu xuất kho theo đơn xuất kho ở trên

                        AutoMapper.Mapper.Map(model, MaterialOutbound);
                        MaterialOutbound.IsDeleted = false;
                        MaterialOutbound.CreatedUserId = WebSecurity.CurrentUserId;
                        MaterialOutbound.ModifiedUserId = WebSecurity.CurrentUserId;
                        MaterialOutbound.CreatedDate = DateTime.Now;
                        MaterialOutbound.ModifiedDate = DateTime.Now;
                        MaterialOutbound.Type = check;
                        if (check == "service")
                        {
                            MaterialOutbound.WarehouseDestinationId = null;
                        }
                        var branch = WarehouseRepository.GetWarehouseById(MaterialOutbound.WarehouseSourceId.Value);
                        MaterialOutbound.BranchId = branch.BranchId;
                        MaterialOutbound.IsDone = false;
                        MaterialOutbound.TotalAmount = outboundDetails.Sum(item => (decimal)item.Quantity * item.Price);
                        MaterialOutbound.WarehouseDestinationId = model.WarehouseDestinationId;
                        materialOutboundRepository.InsertMaterialOutbound(MaterialOutbound);

                        foreach (var item in outboundDetails)
                        {
                            MaterialOutboundDetail materialOutboundDetail = new MaterialOutboundDetail();
                            materialOutboundDetail.MaterialId = item.MaterialId;
                            materialOutboundDetail.Price = item.Price;
                            materialOutboundDetail.Quantity = item.Quantity;
                            materialOutboundDetail.Unit = item.Unit;
                            materialOutboundDetail.LoCode = item.LoCode;
                            materialOutboundDetail.ExpiryDate = item.ExpiryDate;
                            materialOutboundDetail.IsDeleted = false;
                            materialOutboundDetail.CreatedUserId = WebSecurity.CurrentUserId;
                            materialOutboundDetail.ModifiedUserId = WebSecurity.CurrentUserId;
                            materialOutboundDetail.CreatedDate = DateTime.Now;
                            materialOutboundDetail.ModifiedDate = DateTime.Now;
                            materialOutboundDetail.MaterialOutboundId = MaterialOutbound.Id;
                            materialOutboundRepository.InsertMaterialOutboundDetail(materialOutboundDetail);

                        }

                        //cập nhật lại mã xuất kho
                        MaterialOutbound.Code = Erp.BackOffice.Helpers.Common.GetOrderNo("MaterialOutbound", model.Code);
                        materialOutboundRepository.UpdateMaterialOutbound(MaterialOutbound);
                        Erp.BackOffice.Helpers.Common.SetOrderNo("MaterialOutbound");


                        //ghi log 
                        Erp.BackOffice.Controllers.HomeController.WriteLog(MaterialOutbound.Id, MaterialOutbound.Code, "đã tạo phiếu xuất vật tư", "MaterialOutbound/Detail/" + MaterialOutbound.Id, Helpers.Common.CurrentUser.BranchId.Value);

                        //Thêm vào quản lý chứng từ
                        TransactionController.Create(new TransactionViewModel
                        {
                            TransactionModule = "MaterialOutbound",
                            TransactionCode = MaterialOutbound.Code,
                            TransactionName = "Xuất kho vật tư"
                        });
                        if (MaterialOutbound.WarehouseDestinationId != null && MaterialOutbound.WarehouseDestinationId.Value > 0)
                        {
                            Erp.BackOffice.Sale.Controllers.MaterialInboundController.CreateFromMaterialOutbound(MaterialOutbound, outboundDetails);
                        }
                        scope.Complete();
                    }
                    catch (DbUpdateException)
                    {
                        TempData[Globals.FailedMessageKey] = App_GlobalResources.Wording.Error;
                        return View(model);
                    }
                }
                TempData[Globals.SuccessMessageKey] = App_GlobalResources.Wording.InsertSuccess;
                return RedirectToAction("Detail", "MaterialOutbound", new { area = "Sale", Id = MaterialOutbound.Id });
            }
            return View(model);
        }
        #endregion
        public PartialViewResult LoadProductItem(int OrderNo, int MaterialId, string MaterialName, string Unit, int Quantity, decimal Price, string MaterialCode, string MaterialType, int QuantityInventory, int WarehouseId, string LoCode, string ExpiryDate)
        {
            var model = new MaterialOutboundDetailViewModel();
            model.OrderNo = OrderNo;
            model.MaterialId = MaterialId;
            model.MaterialName = MaterialName;
            model.Unit = Unit;
            model.Quantity = Quantity;
            model.Price = Price;
            model.MaterialCode = MaterialCode;
            model.MaterialType = MaterialType;
            model.QuantityInInventory = QuantityInventory;
            model.LoCode = LoCode;
            if (!string.IsNullOrEmpty(ExpiryDate))
                model.ExpiryDate = Convert.ToDateTime(ExpiryDate);
            model.ListWarehouseLocationItemViewModel = new List<WarehouseLocationItemViewModel>();

            var listLocationItemExits = warehouseLocationItemRepository.GetAllWarehouseLocationItem()
                .Where(q => q.ProductId == MaterialId && q.WarehouseId == WarehouseId && q.IsOut == false)
                .OrderBy(x => x.ExpiryDate)
                .Take(Quantity)
                .ToList();

            AutoMapper.Mapper.Map(listLocationItemExits, model.ListWarehouseLocationItemViewModel);

            return PartialView(model);
        }


        #region Json
        public JsonResult EditInline(int? Id, string fieldName, string value)
        {
            Dictionary<string, object> field_value = new Dictionary<string, object>();
            field_value.Add(fieldName, value);
            field_value.Add("ModifiedDate", DateTime.Now.ToString("yyyy-MM-dd H:mm:ss"));
            field_value.Add("ModifiedUserId", WebSecurity.CurrentUserId);

            var flag = QueryHelper.UpdateFields("Sale_MaterialOutbound", field_value, Id.Value);
            if (flag == true)
                return Json(new { status = "success", id = Id, fieldName = fieldName, value = value }, JsonRequestBehavior.AllowGet);

            return Json(new { status = "error", id = Id, fieldName = fieldName, value = value }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetWarehouseLocationItem(int? warehouseId, int? productId, string serialNumber)
        {
            if (warehouseId == null || productId == null || string.IsNullOrEmpty(serialNumber) == true)
                return Json(new WarehouseLocationItemViewModel(), JsonRequestBehavior.AllowGet);

            var item = warehouseLocationItemRepository.GetWarehouseLocationItemBySerialNumber(warehouseId.Value, productId.Value, serialNumber);
            return Json(item, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Print
        public ActionResult Print(int? Id)
        {
            //Lấy thông tin phiếu xuất kho
            var MaterialOutbound = materialOutboundRepository.GetvwMaterialOutboundById(Id.Value);

            if (MaterialOutbound != null)
            {

                //Lấy template và replace dữ liệu vào phiếu xuất.
                TemplatePrint template = null;
                template = templatePrintRepository.GetAllTemplatePrint().Where(x => x.Code.Contains("MateiralOutbound")).OrderByDescending(x => x.CreatedDate).FirstOrDefault();
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

                string day = MaterialOutbound.CreatedDate.Value.Day.ToString();
                string month = MaterialOutbound.CreatedDate.Value.Month.ToString();
                string year = MaterialOutbound.CreatedDate.Value.Year.ToString();
                string customerName = "";
                string customerPhone = "";
                string companyName = "";
                string fullAddress = "";
                string note = MaterialOutbound.Note;
                string code = MaterialOutbound.Code;
                string saleName = "";
                string VAT_InvoiceCode = "";
                string paymentMethod = "";

                if (MaterialOutbound.Type == "internal")
                {
                    output = output.Replace("{WarehouseSourceName}", MaterialOutbound.WarehouseSourceName);
                    output = output.Replace("{WarehouseDestinationName}", MaterialOutbound.WarehouseDestinationName);
                    output = output.Replace("{MaterialOutboundDate}", MaterialOutbound.CreatedDate.Value.ToString("dd/MM/yyyy HH:mm"));
                }
                else
                {
                    output = output.Replace("{CustomerName}", customerName);
                    output = output.Replace("{CustomerPhone}", customerPhone);
                    output = output.Replace("{Day}", day);
                    output = output.Replace("{Month}", month);
                    output = output.Replace("{Year}", year);
                    output = output.Replace("{CompanyName}", companyName);
                    output = output.Replace("{FullAddress}", fullAddress);
                    output = output.Replace("{SaleName}", saleName);
                    output = output.Replace("{VAT_InvoiceCode}", VAT_InvoiceCode);
                    output = output.Replace("{PaymentMethod}", paymentMethod);
                }
                //Tạo barcode
                Image barcode = Code128Rendering.MakeBarcodeImage(MaterialOutbound.Code, 1, true);
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
            var outboundDetails = materialOutboundRepository.GetAllvwMaterialOutboundDetailByOutboundId(Id).AsEnumerable()
                    .Select(x => new MaterialOutboundDetailViewModel
                    {
                        Id = x.Id,
                        Price = x.Price,
                        MaterialId = x.MaterialId,
                        MaterialName = x.MaterialName,
                        MaterialCode = x.MaterialCode,
                        MaterialOutboundId = x.MaterialOutboundId,
                        Quantity = x.Quantity,
                        Unit = x.Unit,
                        LoCode = x.LoCode,
                        ExpiryDate = x.ExpiryDate
                    }).ToList();

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

            foreach (var item in outboundDetails)
            {
                decimal thanh_tien = (decimal)item.Quantity.Value * item.Price.Value;
                tong_tien += thanh_tien;

                detailList += "<tr>\r\n"
                 + "<td class=\"text-center\">" + (outboundDetails.ToList().IndexOf(item) + 1) + "</td>\r\n"
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
                         + Erp.BackOffice.Helpers.Common.PhanCachHangNgan2(outboundDetails.Sum(x => x.Quantity))
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

        #region Config
        public ViewResult Config()
        {
            var q = settingRepository.GetAlls()
                .Where(item => item.Code == "settingprint")
                .OrderBy(item => item.Note).ToList();

            List<Erp.BackOffice.Areas.Administration.Models.SettingViewModel> model = new List<Areas.Administration.Models.SettingViewModel>();
            AutoMapper.Mapper.Map(q, model);

            ViewBag.SuccessMessage = TempData["SuccessMessage"];
            ViewBag.FailedMessage = TempData["FailedMessage"];
            ViewBag.AlertMessage = TempData["AlertMessage"];
            return View(model);
        }
        #endregion

        //#region CreateFromInvoice
        //public ActionResult CreateFromInvoice(int? InvoiceId, int? WarehouseSourceId)
        //{
        //    var model = new MaterialOutboundViewModel();
        //    model.CreatedDate = DateTime.Now;
        //    model.DetailList = new List<MaterialOutboundDetailViewModel>();
        //    model.InvoiceId = InvoiceId;
        //    model.WarehouseSourceId = WarehouseSourceId;

        //    model.SelectList_WarehouseSource = WarehouseRepository.GetAllWarehouse()
        //        .Where(x => ("," + Helpers.Common.CurrentUser.WarehouseId + ",").Contains("," + x.Id + ",") == true)
        //        .Select(item => new
        //        {
        //            Id = item.Id,
        //            Name = item.Name
        //        }).ToList()
        //        .Select(item => new SelectListItem
        //        {
        //            Text = item.Name,
        //            Value = item.Id + ""
        //        }).AsEnumerable();

        //    //Load phiếu xuất theo đơn hàng & Kho hàng
        //    if (InvoiceId != null)
        //    {
        //        var materialInvoice = materialInvoiceRepository.GetvwMaterialInvoiceById(InvoiceId.Value);
        //        if (materialInvoice != null && WarehouseSourceId != null)
        //        {
        //            //Kiểm tra xem nếu có xuất kho ghi sổ rồi thì return
        //            var productOutbound = materialOutboundRepository.GetAllMaterialOutbound()
        //                    .Where(item => item.InvoiceId == materialInvoice.Id).FirstOrDefault();

        //            if (productOutbound != null && productOutbound.IsArchive)
        //                return Content("Đã xuất kho cho đơn hàng!");

        //            model.InvoiceCode = materialInvoice.Code;
        //            model.CreatedStaffId = materialInvoice.StaffCreateId;
        //            model.CreatedStaffName = materialInvoice.StaffCreateName;
        //            //Lấy danh sách chi tiết đơn hàng
        //            var listProductInvoiceDetail = materialInvoiceRepository.GetAllvwInvoiceDetailsByInvoiceId(materialInvoice.Id).Where(x => x.MaterialType == "material")
        //                .Select(item => new
        //                {
        //                    MaterialId = item.MaterialId,
        //                    Price = item.Price,
        //                    MaterialCode = item.MaterialCode,
        //                    MaterialName = item.MaterialName,
        //                    Quantity = item.Quantity
        //                }).ToList();

        //            //Tạo danh sách chi tiết phiếu xuất tương ứng                    
        //            foreach (var item in listMaterialInvoiceDetail)
        //            {
        //                var materialOutboundDetailViewModel = model.DetailList.Where(i => i.MaterialId == item.MaterialId).FirstOrDefault();
        //                if (materialOutboundDetailViewModel == null)
        //                {
        //                    materialOutboundDetailViewModel = new MaterialOutboundDetailViewModel();
        //                    materialOutboundDetailViewModel.MaterialId = item.ProductId;
        //                    materialOutboundDetailViewModel.MaterialCode = item.ProductCode;
        //                    materialOutboundDetailViewModel.MaterialName = item.ProductName;
        //                    materialOutboundDetailViewModel.Quantity = item.Quantity;
        //                    materialOutboundDetailViewModel.Price = item.Price;
        //                    model.DetailList.Add(materialOutboundDetailViewModel);
        //                }
        //                else
        //                {
        //                    materialOutboundDetailViewModel.Quantity += item.Quantity;
        //                }
        //            }

        //            //Lấy lô/date. Mặc định lấy theo phương pháp nhập trước xuất trước
        //            foreach (var item in model.DetailList)
        //            {
        //                item.ListWarehouseLocationItemViewModel = new List<WarehouseLocationItemViewModel>();
        //                var listLocationItemExits = warehouseLocationItemRepository.GetAllWarehouseLocationItem()
        //                    .Where(q => q.MaterialId == item.MaterialId && q.WarehouseId == WarehouseSourceId && q.IsOut == false)
        //                    .OrderBy(x => x.ExpiryDate)
        //                    .Take(item.Quantity.Value);

        //                AutoMapper.Mapper.Map(listLocationItemExits, item.ListWarehouseLocationItemViewModel);
        //            }

        //            model.TotalAmount = model.DetailList.Sum(item => item.Quantity * item.Price);
        //        }
        //    }

        //    ViewBag.SuccessMessage = TempData["SuccessMessage"];
        //    ViewBag.FailedMessage = TempData["FailedMessage"];
        //    ViewBag.AlertMessage = TempData["AlertMessage"];

        //    return View(model);
        //}

        //[HttpPost]
        //public ActionResult CreateFromInvoice(MaterialOutboundViewModel model)
        //{
        //    ProductOutbound productOutbound = null;
        //    if (model.InvoiceId != null)
        //    {
        //        materialOutbound = materialOutboundRepository.GetAllMaterialOutbound()
        //            .Where(x =>
        //        //("," + Helpers.Common.CurrentUser.DrugStore + ",").Contains("," + x.BranchId + ",") == true 
        //        //&&
        //        x.InvoiceId == model.InvoiceId).FirstOrDefault();

        //        if (materialOutbound != null && materialOutbound.IsArchive)
        //            return Content("Phiếu xuất kho cho đơn hàng này đã được ghi sổ!");
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        //Lấy danh sách inventory các sản phẩm đang có trong phiếu xuất của kho nguồn nếu có
        //        var list_InventoryWHSource = inventoryRepository.GetAllInventoryByWarehouseId(model.WarehouseSourceId.Value).ToList();

        //        //Duyệt qua hết chi tiết phiếu xuất, kiểm tra số lượng tồn kho
        //        foreach (var item in model.DetailList)
        //        {
        //            //Kiểm tra tình trạng đảm bảo SP và SL của SP trong kho đáp ứng cho đơn hàng. Xảy ra trong tình huống client có tình can thiệp làm sai các thông số đầu vào
        //            var material = list_InventoryWHSource.Where(x => x.MaterialId == item.MaterialId).FirstOrDefault();
        //            if (material == null || material.Quantity < item.Quantity)
        //            {
        //                TempData[Globals.FailedMessageKey] = item.MaterialCode + " - " + item.MaterialName + ": không đáp ứng đủ số lượng";
        //                if (model.InvoiceId != null)
        //                    return RedirectToAction("CreateFromInvoice", new { InvoiceId = model.InvoiceId, WarehouseSourceId = model.WarehouseSourceId });
        //                else
        //                    return RedirectToAction("CreateFromInvoice");
        //            }
        //        }

        //        //Nếu đã tạo phiếu xuất rồi thì cập nhật
        //        if (materialOutbound != null)//Cập nhật
        //        {
        //            materialOutbound.CreatedStaffId = model.CreatedStaffId;
        //            materialOutbound.ModifiedUserId = WebSecurity.CurrentUserId;
        //            materialOutbound.ModifiedDate = DateTime.Now;
        //            materialOutbound.TotalAmount = model.DetailList.Sum(x => x.Price * x.Quantity);
        //            materialOutboundRepository.UpdateMaterialOutbound(materialOutbound);

        //            //Xóa chi tiết cũ và thêm chi tiết mới
        //            var listMaterialOutboundDetail_old = materialOutboundRepository.GetAllMaterialOutboundDetailByOutboundId(materialOutbound.Id)
        //                .Select(item => item.Id).ToList();

        //            foreach (var id in listMaterialOutboundDetail_old)
        //            {
        //                materialOutboundRepository.DeleteMaterialOutboundDetail(id);
        //            }

        //            foreach (var item in model.DetailList)
        //            {
        //                MaterialOutboundDetail materialOutboundDetail = new MaterialOutboundDetail();
        //                AutoMapper.Mapper.Map(item, materialOutboundDetail);
        //                materialOutboundDetail.MaterialOutboundId = materialOutbound.Id;
        //                materialOutboundDetail.IsDeleted = false;
        //                materialOutboundDetail.CreatedUserId = WebSecurity.CurrentUserId;
        //                materialOutboundDetail.ModifiedUserId = WebSecurity.CurrentUserId;
        //                materialOutboundDetail.CreatedDate = DateTime.Now;
        //                materialOutboundDetail.ModifiedDate = DateTime.Now;
        //                materialOutboundRepository.InsertMaterialOutboundDetail(materialOutboundDetail);
        //            }

        //        }
        //        else//Thêm mới
        //        {
        //            CreateFromInvoice(materialOutboundRepository, model, model.InvoiceCode);
        //        }
        //        //Chọn lô/date cho mỗi chi tiết phiếu xuất
        //        UpdateLote(materialOutboundRepository, warehouseLocationItemRepository, materialOutbound.Id, model.LocationItemList);


        //        //Ghi sổ chứng từ
        //        Archive(materialOutbound, TempData);

        //        return RedirectToAction("Detail", new { Id = materialOutbound.Id });
        //    }

        //    return RedirectToAction("Index", new { Id = materialOutbound.Id });
        //}


        //public static MaterialOutbound CreateFromInvoice(IMaterialOutboundRepository materialOutboundRepository,
        //    MaterialOutboundViewModel model,
        //    string materialInvoiceCode)
        //{
        //    Erp.Domain.Sale.Repositories.WarehouseLocationItemRepository warehouseLocationItemRepository = new Erp.Domain.Sale.Repositories.WarehouseLocationItemRepository(new Domain.Sale.ErpSaleDbContext());
        //    Erp.Domain.Sale.Repositories.WarehouseRepository warehouseRepository = new Erp.Domain.Sale.Repositories.WarehouseRepository(new Domain.Sale.ErpSaleDbContext());
        //    var materialOutbound = new Domain.Sale.Entities.MaterialOutbound();
        //    AutoMapper.Mapper.Map(model, materialOutbound);
        //    materialOutbound.IsDeleted = false;
        //    materialOutbound.CreatedUserId = WebSecurity.CurrentUserId;
        //    materialOutbound.ModifiedUserId = WebSecurity.CurrentUserId;
        //    materialOutbound.CreatedDate = DateTime.Now;
        //    materialOutbound.ModifiedDate = DateTime.Now;
        //    materialOutbound.Type = "invoice";
        //    var branch = warehouseRepository.GetWarehouseById(materialOutbound.WarehouseSourceId.Value);
        //    materialOutbound.BranchId = branch.BranchId;
        //    materialOutbound.TotalAmount = model.DetailList.Sum(x => x.Price * x.Quantity);
        //    materialOutboundRepository.InsertMaterialOutbound(materialOutbound);

        //    //Cập nhật lại mã xuất kho
        //    materialOutbound.Code = Erp.BackOffice.Helpers.Common.GetOrderNo("materialOutbound", model.Code);
        //    materialOutboundRepository.UpdateMaterialOutbound(materialOutbound);
        //    Erp.BackOffice.Helpers.Common.SetOrderNo("materialOutbound");

        //    string prefix = Erp.BackOffice.Helpers.Common.GetSetting("prefixOrderNo_Outbound");
        //    materialOutbound.Code = Erp.BackOffice.Helpers.Common.GetCode(prefix, materialOutbound.Id);
        //    materialOutboundRepository.UpdateMaterialOutbound(materialOutbound);
        //    // chi tiết phiếu xuất
        //    foreach (var item in model.DetailList)
        //    {
        //        MaterialOutboundDetail materialOutboundDetail = new MaterialOutboundDetail();
        //        AutoMapper.Mapper.Map(item, materialOutboundDetail);
        //        materialOutboundDetail.IsDeleted = false;
        //        materialOutboundDetail.CreatedUserId = WebSecurity.CurrentUserId;
        //        materialOutboundDetail.ModifiedUserId = WebSecurity.CurrentUserId;
        //        materialOutboundDetail.CreatedDate = DateTime.Now;
        //        materialOutboundDetail.ModifiedDate = DateTime.Now;
        //        materialOutboundDetail.MaterialOutboundId = materialOutbound.Id;
        //        materialOutboundRepository.InsertMaterialOutboundDetail(materialOutboundDetail);

        //    }
        //    //cập nhật vị trí các sản phẩm đã xuất kho
        //    //
        //    //Thêm vào quản lý chứng từ
        //    TransactionController.Create(new TransactionViewModel
        //    {
        //        TransactionModule = "MaterialOutbound",
        //        TransactionCode = materialOutbound.Code,
        //        TransactionName = "Xuất kho bán hàng"
        //    });

        //    //Thêm chứng từ liên quan
        //    TransactionController.CreateRelationship(new TransactionRelationshipViewModel
        //    {
        //        TransactionA = materialOutbound.Code,
        //        TransactionB = materialInvoiceCode
        //    });

        //    return materialOutbound;
        //}


        //public static MaterialOutbound AutoCreateMaterialOutboundFromMaterialInvoice(IMaterialOutboundRepository materialOutboundRepository,
        // MaterialOutboundViewModel model,
        // string materialInvoiceCode)
        //{
        //    Erp.Domain.Sale.Repositories.WarehouseRepository warehouseRepository = new Erp.Domain.Sale.Repositories.WarehouseRepository(new Domain.Sale.ErpSaleDbContext());
        //    //Erp.Domain.Sale.Repositories.WarehouseLocationItemRepository warehouseLocationItemRepository = new Erp.Domain.Sale.Repositories.WarehouseLocationItemRepository(new Domain.Sale.ErpSaleDbContext());
        //    //AutoMapper.Mapper.CreateMap<Domain.Sale.Entities.vwWarehouseLocationItem, Domain.Sale.Entities.WarehouseLocationItem>();


        //    var materialOutbound = new Domain.Sale.Entities.MaterialOutbound();
        //    AutoMapper.Mapper.Map(model, materialOutbound);
        //    materialOutbound.IsDeleted = false;
        //    materialOutbound.CreatedUserId = WebSecurity.CurrentUserId;
        //    materialOutbound.ModifiedUserId = WebSecurity.CurrentUserId;
        //    materialOutbound.CreatedDate = DateTime.Now;
        //    materialOutbound.ModifiedDate = DateTime.Now;
        //    materialOutbound.Type = "invoice";
        //    var branch = warehouseRepository.GetWarehouseById(materialOutbound.WarehouseSourceId.Value);
        //    materialOutbound.BranchId = branch.BranchId;
        //    materialOutbound.TotalAmount = model.DetailList.Sum(x => x.Price * x.Quantity);
        //    materialOutboundRepository.InsertMaterialOutbound(materialOutbound);

        //    //Cập nhật lại mã xuất kho
        //    materialOutbound.Code = Erp.BackOffice.Helpers.Common.GetOrderNo("materialOutbound", model.Code);
        //    materialOutboundRepository.UpdateMaterialOutbound(materialOutbound);
        //    Erp.BackOffice.Helpers.Common.SetOrderNo("materialOutbound");

        //    string prefix = Erp.BackOffice.Helpers.Common.GetSetting("prefixOrderNo_Outbound");
        //    materialOutbound.Code = Erp.BackOffice.Helpers.Common.GetCode(prefix, materialOutbound.Id);
        //    materialOutboundRepository.UpdateMaterialOutbound(materialOutbound);
        //    // chi tiết phiếu xuất
        //    foreach (var item in model.DetailList)
        //    {
        //        MaterialOutboundDetail materialOutboundDetail = new MaterialOutboundDetail();
        //        AutoMapper.Mapper.Map(item, materialOutboundDetail);
        //        materialOutboundDetail.IsDeleted = false;
        //        materialOutboundDetail.CreatedUserId = WebSecurity.CurrentUserId;
        //        materialOutboundDetail.ModifiedUserId = WebSecurity.CurrentUserId;
        //        materialOutboundDetail.CreatedDate = DateTime.Now;
        //        materialOutboundDetail.ModifiedDate = DateTime.Now;
        //        materialOutboundDetail.MaterialOutboundId = materialOutbound.Id;
        //        materialOutboundRepository.InsertMaterialOutboundDetail(materialOutboundDetail);
        //        //var list=new List<WarehouseLocationItem>();
        //        //    var listLocationItemExits = warehouseLocationItemRepository.GetAllvwWarehouseLocationItem()
        //        //        .Where(q => q.materialId == item.materialId && q.BranchId == materialOutbound.BranchId&&q.IsSale==true && q.IsOut == false&&q.materialOutboundId==null&&q.materialOutboundDetailId==null)
        //        //        .OrderBy(x => x.ExpiryDate)
        //        //        .Take(item.Quantity.Value).ToList();
        //        //    AutoMapper.Mapper.Map(listLocationItemExits, list);
        //        //    for (int i = 0; i < list.Count(); i++)
        //        //    {
        //        //        list[i].materialOutboundId = materialOutbound.Id;
        //        //        list[i].materialOutboundDetailId = materialOutboundDetail.Id;
        //        //        list[i].ModifiedDate = DateTime.Now;
        //        //        list[i].ModifiedUserId = WebSecurity.CurrentUserId;
        //        //        warehouseLocationItemRepository.UpdateWarehouseLocationItem(list[i]);
        //        //    }
        //    }
        //    //cập nhật vị trí các sản phẩm đã xuất kho
        //    //
        //    //Thêm vào quản lý chứng từ
        //    TransactionController.Create(new TransactionViewModel
        //    {
        //        TransactionModule = "MaterialOutbound",
        //        TransactionCode = materialOutbound.Code,
        //        TransactionName = "Xuất kho bán hàng"
        //    });

        //    //Thêm chứng từ liên quan
        //    TransactionController.CreateRelationship(new TransactionRelationshipViewModel
        //    {
        //        TransactionA = materialOutbound.Code,
        //        TransactionB = materialInvoiceCode
        //    });

        //    return materialOutbound;
        //}

        //public static void UpdateLote(IMaterialOutboundRepository materialOutboundRepository,
        //    IWarehouseLocationItemRepository warehouseLocationItemRepository,
        //    int materialOutboundId,
        //    List<WarehouseLocationItemViewModel> LocationItemList)
        //{

        //    if (LocationItemList != null)
        //    {
        //        var listMaterialOutboundDetail = materialOutboundRepository.GetAllMaterialOutboundDetailByOutboundId(materialOutboundId)
        //            .Select(item => new { item.Id, item.MaterialId }).ToList();

        //        LocationItemList = LocationItemList.Where(x => x.Id != null && x.Id != 0).ToList();

        //        if (LocationItemList.Count != 0)
        //        {
        //            foreach (var item in LocationItemList)
        //            {
        //                var MaterialOutboundDetail = listMaterialOutboundDetail.Where(x => x.MaterialId == item.MaterialId).FirstOrDefault();
        //                var LocationItem = warehouseLocationItemRepository.GetWarehouseLocationItemById(item.Id.Value);
        //                if (LocationItem != null)
        //                {
        //                    LocationItem.MaterialOutboundId = materialOutboundId;
        //                    LocationItem.MaterialOutboundDetailId = MaterialOutboundDetail != null ? MaterialOutboundDetail.Id : 0;
        //                    LocationItem.ModifiedDate = DateTime.Now;
        //                    LocationItem.ModifiedUserId = WebSecurity.CurrentUserId;
        //                    warehouseLocationItemRepository.UpdateWarehouseLocationItem(LocationItem);
        //                }
        //            }
        //        }
        //    }
        //}

        public PartialViewResult LoadMaterialItem(int OrderNo, int MaterialId, string MaterialName, string Unit, int Quantity, string MaterialCode, string MaterialType, int QuantityInventory, int WarehouseId, string LoCode, string ExpiryDate)
        {
            var model = new MaterialOutboundDetailViewModel();
            model.OrderNo = OrderNo;
            model.MaterialId = MaterialId;
            model.MaterialName = MaterialName;
            model.Unit = Unit;
            model.Quantity = Quantity;
            model.MaterialCode = MaterialCode;
            model.MaterialType = MaterialType;
            model.QuantityInInventory = QuantityInventory;
            model.LoCode = LoCode;
            if (!string.IsNullOrEmpty(ExpiryDate))
                model.ExpiryDate = Convert.ToDateTime(ExpiryDate);
            model.ListWarehouseLocationItemViewModel = new List<WarehouseLocationItemViewModel>();

            //var listLocationItemExits = warehouseLocationItemRepository.GetAllWarehouseLocationItem()
            //    .Where(q => q. == MaterialId && q.WarehouseId == WarehouseId && q.IsOut == false)
            //    .OrderBy(x => x.ExpiryDate)
            //    .Take(Quantity)
            //    .ToList();

            //AutoMapper.Mapper.Map(listLocationItemExits, model.ListWarehouseLocationItemViewModel);

            return PartialView(model);
        }


        #region Detail
        public ActionResult Detail(int? Id, string TransactionCode)
        {
            var MaterialOutbound = new vwMaterialOutbound();
            if (Id != null)
                MaterialOutbound = materialOutboundRepository.GetvwMaterialOutboundFullById(Id.Value);

            if (!string.IsNullOrEmpty(TransactionCode))
                MaterialOutbound = materialOutboundRepository.GetAllvwMaterialOutboundFull().Where(item => item.Code == TransactionCode).FirstOrDefault();

            if (MaterialOutbound != null)
            {
                var model = new MaterialOutboundViewModel();
                AutoMapper.Mapper.Map(MaterialOutbound, model);

                //Kiểm tra cho phép sửa chứng từ này hay không
                model.AllowEdit = Helpers.Common.KiemTraNgaySuaChungTu(model.CreatedDate.Value);

                var wh_source = WarehouseRepository.GetWarehouseById(model.WarehouseSourceId.Value);
                if (wh_source != null)
                {
                    model.WarehouseSourceName = wh_source.Name;
                }
                //Lấy hóa đơn để hiện thị

                if (model.WarehouseDestinationId != null)
                {
                    var wh_destination = WarehouseRepository.GetWarehouseById(model.WarehouseDestinationId.Value);
                    if (wh_destination != null)
                        model.WarehouseDestinationName = wh_destination.Name;
                    else
                        model.WarehouseDestinationName = "Khác";
                }

                var outboundDetails = materialOutboundRepository.GetAllvwMaterialOutboundDetailByOutboundId(model.Id).AsEnumerable()
                    .Select(x => new MaterialOutboundDetailViewModel
                    {
                        MaterialId = x.MaterialId,
                        Price = x.Price,
                        Quantity = x.Quantity,
                        Unit = x.Unit,
                        MaterialName = x.MaterialName,
                        MaterialCode = x.MaterialCode,
                        LoCode = x.LoCode,
                        ExpiryDate = x.ExpiryDate,
                        Id = x.Id
                    }).OrderBy(x => x.Id).ToList();

                model.DetailList = outboundDetails;

                var username = userRepository.GetUserById(model.CreatedUserId.Value);
                if (username != null)
                {
                    model.CreatedUserName = username.FullName;
                }

                ViewBag.SuccessMessage = TempData["SuccessMessage"];
                ViewBag.FailedMessage = TempData["FailedMessage"];

                return View(model);
            }

            if (Request.UrlReferrer != null)
                return Redirect(Request.UrlReferrer.AbsoluteUri);
            return RedirectToAction("Index");
        }
        #endregion

        #region Archive
        [HttpPost]
        public ActionResult Archive(int Id, bool? IsPopup)
        {
            if (Request["Submit"] == "Save")
            {
                var materialOutbound = materialOutboundRepository.GetMaterialOutboundById(Id);

                //Kiểm tra cho phép sửa chứng từ này hay không
                if (Helpers.Common.KiemTraNgaySuaChungTu(materialOutbound.CreatedDate.Value) == false)
                {
                    return RedirectToAction("Detail", new { Id = materialOutbound.Id });
                }
                using (var scope = new TransactionScope(TransactionScopeOption.Required,new System.TimeSpan(0,30, 0)))
                {
                    try
                    {
                        Archive(materialOutbound, TempData);

                        //ghi log 
                        Erp.BackOffice.Controllers.HomeController.WriteLog(materialOutbound.Id, materialOutbound.Code, "đã ghi sổ phiếu xuất vật tư", "MaterialOutbound/Detail/" + materialOutbound.Id, Helpers.Common.CurrentUser.BranchId.Value);

                        scope.Complete();
                    }
                    catch (DbUpdateException)
                    {
                        TempData[Globals.FailedMessageKey] = App_GlobalResources.Wording.Error;
                        return RedirectToAction("Detail", new { Id = Id });
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

        public static void Archive(MaterialOutbound materialOutbound,
            TempDataDictionary TempData)
        {
            MaterialOutboundRepository materialOutboundRepository = new Domain.Sale.Repositories.MaterialOutboundRepository(new Domain.Sale.ErpSaleDbContext());
            //Cập nhật lại tồn kho cho những sp trong phiếu này
            var detailList = materialOutboundRepository.GetAllvwMaterialOutboundDetailByOutboundId(materialOutbound.Id)
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
            foreach (var item in detailList)
            {
                var error = InventoryMaterialController.Check(item.MaterialName, item.MaterialId, item.LoCode, item.ExpiryDate, materialOutbound.WarehouseSourceId.Value, 0, item.Quantity);
                check += error;
            }

            if (string.IsNullOrEmpty(check))
            {
                //Khi đã hợp lệ thì mới update
                foreach (var item in detailList)
                {
                    InventoryMaterialController.Update(item.MaterialName, item.MaterialId, item.LoCode, item.ExpiryDate, materialOutbound.WarehouseSourceId.Value, 0, item.Quantity);
                }

                materialOutbound.IsArchive = true;
                materialOutboundRepository.UpdateMaterialOutbound(materialOutbound);

                TempData[Globals.SuccessMessageKey] = App_GlobalResources.Wording.ArchiveSuccess;
            }
            else
            {
                TempData[Globals.FailedMessageKey] = App_GlobalResources.Wording.ArchiveFail + check;
            }
        }


        public static void Archive(MaterialOutbound materialOutbound)
        {
            MaterialOutboundRepository materialOutboundRepository = new Domain.Sale.Repositories.MaterialOutboundRepository(new Domain.Sale.ErpSaleDbContext());
            //Cập nhật lại tồn kho cho những sp trong phiếu này
            var detailList = materialOutboundRepository.GetAllvwMaterialOutboundDetailByOutboundId(materialOutbound.Id)
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
            foreach (var item in detailList)
            {
                var error = InventoryMaterialController.Check(item.MaterialName, item.MaterialId, item.LoCode, item.ExpiryDate, materialOutbound.WarehouseSourceId.Value, 0, item.Quantity);
                check += error;
            }

            if (string.IsNullOrEmpty(check))
            {
                //Khi đã hợp lệ thì mới update
                foreach (var item in detailList)
                {
                    InventoryMaterialController.Update(item.MaterialName, item.MaterialId, item.LoCode, item.ExpiryDate, materialOutbound.WarehouseSourceId.Value, 0, item.Quantity);
                }

                materialOutbound.IsArchive = true;
                materialOutboundRepository.UpdateMaterialOutbound(materialOutbound);

            }
        }

        #endregion

        #region UnArchive
        [HttpPost]
        public ActionResult UnArchive(int Id, bool? IsPopup)
        {
            if (Request["Submit"] == "Save")
            {
                var materialOutbound = materialOutboundRepository.GetMaterialOutboundById(Id);

                //Kiểm tra cho phép sửa chứng từ này hay không
                if (Helpers.Common.KiemTraNgaySuaChungTu(materialOutbound.CreatedDate.Value) == false)
                {
                    return RedirectToAction("Detail", new { Id = materialOutbound.Id });
                }

                //Cập nhật lại tồn kho cho những sp trong phiếu này
                var detailList = materialOutboundRepository.GetAllvwMaterialOutboundDetailByOutboundId(materialOutbound.Id)
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
                foreach (var item in detailList)
                {
                    var error = InventoryMaterialController.Check(item.MaterialName, item.MaterialId, item.LoCode, item.ExpiryDate, materialOutbound.WarehouseSourceId.Value, item.Quantity, 0);
                    check += error;
                }

                if (string.IsNullOrEmpty(check))
                {
                    //Khi đã hợp lệ thì mới update
                    foreach (var item in detailList)
                    {
                        InventoryMaterialController.Update(item.MaterialName, item.MaterialId, item.LoCode, item.ExpiryDate, materialOutbound.WarehouseSourceId.Value, item.Quantity, 0);
                    }

                    materialOutbound.IsArchive = false;
                    materialOutboundRepository.UpdateMaterialOutbound(materialOutbound);
                    //ghi log 
                    Erp.BackOffice.Controllers.HomeController.WriteLog(materialOutbound.Id, materialOutbound.Code, "đã bỏ ghi sổ phiếu xuất vật tư", "MaterialOutbound/Detail/" + materialOutbound.Id, Helpers.Common.CurrentUser.BranchId.Value);

                    TempData[Globals.SuccessMessageKey] = "Đã bỏ ghi sổ";
                }
                else
                {
                    TempData[Globals.FailedMessageKey] = App_GlobalResources.Wording.ArchiveFail + check;
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

        public static void UnArchiveAndDelete(MaterialOutbound productOutbound)
        {
            MaterialOutboundRepository productOutboundRepository = new Domain.Sale.Repositories.MaterialOutboundRepository(new Domain.Sale.ErpSaleDbContext());

            //Cập nhật lại tồn kho cho những sp trong phiếu này
            var detailList = productOutboundRepository.GetAllvwMaterialOutboundDetailByOutboundId(productOutbound.Id)
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
            foreach (var item in detailList)
            {
                var error = InventoryMaterialController.Check(item.MaterialName, item.MaterialId, item.LoCode, item.ExpiryDate, productOutbound.WarehouseSourceId.Value, item.Quantity, 0);
                check += error;
            }

            if (string.IsNullOrEmpty(check))
            {
                //Khi đã hợp lệ thì mới update
                foreach (var item in detailList)
                {
                    InventoryMaterialController.Update(item.MaterialName, item.MaterialId, item.LoCode, item.ExpiryDate, productOutbound.WarehouseSourceId.Value, item.Quantity, 0);
                }

                productOutbound.IsArchive = false;
                productOutbound.IsDeleted = true;
                productOutboundRepository.UpdateMaterialOutbound(productOutbound);
                //TempData[Globals.SuccessMessageKey] = "Đã bỏ ghi sổ";
            }
            else
            {
                //TempData[Globals.FailedMessageKey] = App_GlobalResources.Wording.ArchiveFail + check;
            }
        }
        #endregion

        #region Edit
        public ActionResult Edit(int? Id, string TransactionCode)
        {
            var materialOutbound = new vwMaterialOutbound();
            if (Id != null)
                materialOutbound = materialOutboundRepository.GetvwMaterialOutboundById(Id.Value);
            if (!string.IsNullOrEmpty(TransactionCode))
                materialOutbound = materialOutboundRepository.GetvwMaterialOutboundByTransactionCode(TransactionCode);

            if (materialOutbound != null && materialOutbound.IsDeleted != true)
            {
                //Nếu đã ghi sổ rồi thì không được sửa
                if (materialOutbound.IsArchive)
                {
                    return RedirectToAction("Detail", new { Id = materialOutbound.Id });
                }

                var model = new MaterialOutboundViewModel();
                model.DetailList = new List<MaterialOutboundDetailViewModel>();
                AutoMapper.Mapper.Map(materialOutbound, model);

                // lấy danh sách detail
                model.DetailList = materialOutboundRepository.GetAllvwMaterialOutboundDetailByOutboundId(materialOutbound.Id)
                    .Select(x => new MaterialOutboundDetailViewModel
                    {
                        Id = x.Id,
                        Price = x.Price,
                        MaterialId = x.MaterialId,
                        MaterialOutboundId = x.MaterialOutboundId,
                        Quantity = x.Quantity,
                        MaterialCode = x.MaterialCode,
                        MaterialName = x.MaterialName,
                        ExpiryDate = x.ExpiryDate,
                        LoCode = x.LoCode,
                        Unit = x.Unit
                    }).ToList();
                model.CreatedUserName = userRepository.GetUserById(model.CreatedUserId.Value).FullName;
                return View(model);
            }

            return View();
        }

        [HttpPost]
        public ActionResult Edit(MaterialOutboundViewModel model)
        {

            var materialOutbound = materialOutboundRepository.GetMaterialOutboundById(model.Id);
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.Snapshot }))
            {
                try
                {
                    var check = Request["group_choice"];
                    var _listdata = materialOutboundRepository.GetAllMaterialOutboundDetailByOutboundId(materialOutbound.Id).ToList();
                    if (model.DetailList.Any(x => x.Id == 0))
                    {
                        //lưu danh sách thao tác thực hiện dịch vụ
                        foreach (var item in model.DetailList.Where(x => x.Id == 0 && x.MaterialId > 0))
                        {
                            var ins = new MaterialOutboundDetail();
                            ins.IsDeleted = false;
                            ins.CreatedUserId = WebSecurity.CurrentUserId;
                            ins.ModifiedUserId = WebSecurity.CurrentUserId;
                            ins.CreatedDate = DateTime.Now;
                            ins.ModifiedDate = DateTime.Now;
                            ins.ExpiryDate = item.ExpiryDate;
                            ins.LoCode = item.LoCode;
                            ins.MaterialId = item.MaterialId;
                            ins.MaterialOutboundId = materialOutbound.Id;
                            ins.Price = item.Price;
                            ins.Quantity = item.Quantity;
                            ins.Unit = item.Unit;
                            materialOutboundRepository.InsertMaterialOutboundDetail(ins);
                        }
                    }
                    var _delete = _listdata.Where(id1 => !model.DetailList.ToList().Any(id2 => id2.Id == id1.Id)).ToList();
                    foreach (var item in _delete)
                    {
                        materialOutboundRepository.DeleteMaterialOutboundDetail(item.Id);
                    }
                    if (model.DetailList.Any(x => x.Id > 0))
                    {
                        var update = _listdata.Where(id1 => model.DetailList.ToList().Any(id2 => id2.Id == id1.Id)).ToList();
                        //lưu danh sách thao tác thực hiện dịch vụ
                        foreach (var item in model.DetailList.Where(x => x.Id > 0 && x.MaterialId > 0))
                        {
                            var _update = update.FirstOrDefault(x => x.Id == item.Id);
                            _update.ExpiryDate = item.ExpiryDate;
                            _update.LoCode = item.LoCode;
                            _update.MaterialId = item.MaterialId;
                            _update.MaterialOutboundId = materialOutbound.Id;
                            _update.Price = item.Price;
                            _update.Quantity = item.Quantity;
                            _update.Unit = item.Unit;
                            materialOutboundRepository.UpdateMaterialOutboundDetail(_update);
                        }
                    }
                    materialOutbound.Type = check;
                    if (check == "service")
                    {
                        materialOutbound.WarehouseDestinationId = null;
                    }
                    materialOutbound.Note = model.Note;
                    materialOutbound.TotalAmount = model.TotalAmount;
                    materialOutbound.ModifiedUserId = Helpers.Common.CurrentUser.Id;
                    materialOutbound.ModifiedDate = DateTime.Now;
                    materialOutboundRepository.UpdateMaterialOutbound(materialOutbound);

                    //ghi log 
                    Erp.BackOffice.Controllers.HomeController.WriteLog(materialOutbound.Id, materialOutbound.Code, "đã cập nhật phiếu xuất vật tư", "MaterialOutbound/Detail/" + materialOutbound.Id, Helpers.Common.CurrentUser.BranchId.Value);

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
        #region Delete
        [HttpPost]
        public ActionResult Delete(int Id, string Note)
        {
            var materialOutbound = materialOutboundRepository.GetMaterialOutboundById(Id);
            if (materialOutbound != null)
            {
                //Kiểm tra phân quyền Chi nhánh
                //if (!(Filters.SecurityFilter.IsAdmin() || ("," + Helpers.Common.CurrentUser.DrugStore + ",").Contains("," + productInbound.BranchId + ",") == true))
                //{
                //    return Content("Mẫu tin không tồn tại! Không có quyền truy cập!");
                //}

                materialOutbound.ModifiedUserId = WebSecurity.CurrentUserId;
                materialOutbound.ModifiedDate = DateTime.Now;
                materialOutbound.IsDeleted = true;
                materialOutbound.IsArchive = false;
                materialOutbound.Note = Note;
                materialOutboundRepository.UpdateMaterialOutbound(materialOutbound);
                //ghi log 
                Erp.BackOffice.Controllers.HomeController.WriteLog(materialOutbound.Id, materialOutbound.Code, "đã hủy phiếu xuất vật tư", "MaterialOutbound/Detail/" + materialOutbound.Id, Helpers.Common.CurrentUser.BranchId.Value);

                return RedirectToAction("Detail", new { Id = materialOutbound.Id });
            }

            return RedirectToAction("Index");
        }
        #endregion

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

                    a += "</td><td>" +
                "<input type = \"hidden\" name=\"DetailList[" + item.OrderNo + "].Unit\" value=\"" + item.Unit + "\" class=\"item_unit\">" +
                "<input class=\"item_quantity numberinput2 text-right\" type=\"text\" id=\"DetailList_" + item.OrderNo + "__Quantity\" name=\"DetailList[" + item.OrderNo + "].Quantity\" value=\"" + item.Quantity + "\" style=\"width:100%\"> ";

                    a += "</td><td>" +

                    "<input class=\"item_price numberinput2 text-right\" type=\"text\" id=\"DetailList_" + item.OrderNo + "__Price\" name=\"DetailList[" + item.OrderNo + "].Price\" value=\"" + item.Price + "\" style=\"width:100%\"> ";

                    a += "<td align=\"right\"><span class=\"item_total\" align=\"right\">" + Helpers.CommonSatic.ToCurrencyStr((decimal)item.Quantity * item.Price, null) + "</span></td>";
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
        public DataTable getData()
        {
            //Creating DataTable  
            DataTable dt = new DataTable();
            //Setiing Table Name  
            dt.TableName = "MaterialOutBoundData";
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

            
            DataTable dt = getData();
            //Name of File  
            string fileName = "ExcelMauXuat.xlsx";
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
            
        }
        #endregion
    }
}
