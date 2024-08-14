using System.Globalization;
using Erp.BackOffice.Sale.Models;
using Erp.BackOffice.Filters;
using Erp.Domain.Entities;
using Erp.Domain.Interfaces;
using Erp.Domain.Sale.Entities;
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
using Erp.Domain.Account.Interfaces;
using Erp.Domain.Account.Entities;
using Erp.BackOffice.Account.Controllers;
using Erp.BackOffice.Account.Models;
using System.Web;
using System.Transactions;
using Erp.BackOffice.Areas.Cms.Models;
using Erp.Domain.Sale.Repositories;
using DocumentFormat.OpenXml.Drawing.Diagrams;

namespace Erp.BackOffice.Sale.Controllers
{
    [Authorize]
    [InitializeSimpleMembership]
    [Erp.BackOffice.Helpers.NoCacheHelper]
    public class SalesReturnsController : Controller
    {
        private readonly ITransactionLiabilitiesRepository transactionRepository;
        private readonly ITransactionRepository transactionRepository2;
        private readonly ISalesReturnsRepository SalesReturnsRepository;
        private readonly IUserRepository userRepository;
        private readonly IProductInvoiceRepository productInvoiceRepository;
        private readonly IProductOrServiceRepository productRepository;
        private readonly ICustomerRepository customerRepository;
        private readonly IPaymentRepository paymentRepository;
        private readonly ICategoryRepository categoryRepository;
        private readonly IProductInboundRepository productInboundRepository;
        private readonly IWarehouseLocationItemRepository warehouseLocationItemRepository;
        private readonly IWarehouseRepository WarehouseRepository;
        private readonly ITemplatePrintRepository templatePrintRepository;
        private readonly ITransactionLiabilitiesRepository transactionLiabilitiesRepository;
        private readonly IReceiptRepository ReceiptRepository;
        private readonly IPaymentDetailRepository paymentDetailRepository;
        private readonly IProductOutboundRepository productOutboundRepository;
        private readonly IWarehouseRepository warehouseRepository;
        private readonly IMembershipRepository membershipRepository;
        private readonly IMembership_parentRepository Membership_parentRepository;
        private readonly ICustomerRepository CustomerRepository;
        public SalesReturnsController(
            ITransactionLiabilitiesRepository _transaction
            , ITransactionRepository _transactionRepository2
            , ISalesReturnsRepository _SalesReturns
            , IUserRepository _user
            , IProductInvoiceRepository _ProductInvoice
            , IProductOrServiceRepository product
            , ICustomerRepository customer
            , IPaymentRepository payment
            , ICategoryRepository category
            , IProductInboundRepository _ProductInbound
            , IWarehouseLocationItemRepository _WarehouseLocationItem
            , IWarehouseRepository _Warehouse
            , ITemplatePrintRepository _templatePrint
            , ITransactionLiabilitiesRepository transactionLiabilities
            , IReceiptRepository Receipt
            , IPaymentDetailRepository paymentDetail
            , IProductOutboundRepository productOutbound
            , IWarehouseRepository warehouse
            , IMembershipRepository membership
            , IMembership_parentRepository _Membership_parent
            , ICustomerRepository _Customer
            )
        {
            SalesReturnsRepository = _SalesReturns;
            userRepository = _user;
            productInvoiceRepository = _ProductInvoice;
            productRepository = product;
            customerRepository = customer;
            transactionRepository = _transaction;
            transactionRepository2 = _transactionRepository2;
            paymentRepository = payment;
            categoryRepository = category;
            productInboundRepository = _ProductInbound;
            warehouseLocationItemRepository = _WarehouseLocationItem;
            WarehouseRepository = _Warehouse;
            templatePrintRepository = _templatePrint;
            transactionLiabilitiesRepository = transactionLiabilities;
            ReceiptRepository = Receipt;
            paymentDetailRepository = paymentDetail;
            productOutboundRepository = productOutbound;
            warehouseRepository = warehouse;
            membershipRepository = membership;
            Membership_parentRepository = _Membership_parent;
            CustomerRepository = _Customer;
        }

        #region Index

        public ViewResult Index(string txtCode, string txtCusName, string startDate, string endDate ,string Status, int? SalerId)
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

            IEnumerable<SalesReturnsViewModel> q = SalesReturnsRepository.GetAllvwSalesReturns()
                //.Where(x => x.BranchId == Helpers.Common.CurrentUser.BranchId)
                .AsEnumerable()
                .Select(item => new SalesReturnsViewModel
                {
                    Id = item.Id,
                    CreatedUserId = item.CreatedUserId,
                    CreatedDate = item.CreatedDate,
                    ModifiedUserId = item.ModifiedUserId,
                    ModifiedDate = item.ModifiedDate,
                    Code = item.Code,
                    CustomerCode = item.CustomerCode,
                    CustomerName = item.CustomerName,
                    TotalAmount = item.TotalAmount,
                    TaxFee = item.TaxFee,
                    BranchId = item.BranchId,
                    CustomerId = item.CustomerId,
                    PaymentMethod = item.PaymentMethod,
                    Status = item.Status,
                    BranchName = item.BranchName,
                    IsArchive = item.IsArchive
                }).OrderByDescending(m => m.CreatedDate).ToList();

            foreach (var item2 in q)
            {
                if (item2.CustomerId != null)
                {
                    var Man = CustomerRepository.GetvwCustomerById(item2.CustomerId.Value);
                    item2.ManagerStaffId = Man.ManagerStaffId;
                    item2.ManagerStaffName = Man.ManagerStaffName;
                    item2.ManagerUserName = Man.ManagerUserName;
                }
                else
                {
                    item2.ManagerStaffId = 0;
                    item2.ManagerStaffName = "null";
                    item2.ManagerUserName = "null";
                }
                
            }

            if(intBrandID > 0)
            {
                q = q.Where(x => x.BranchId == intBrandID);
            }
            if (string.IsNullOrEmpty(txtCode) == false || string.IsNullOrEmpty(txtCusName) == false)
            {
                txtCode = txtCode == "" ? "~" : txtCode.ToLower();

                txtCusName = txtCusName == "" ? "~" : Erp.BackOffice.Helpers.Common.ChuyenThanhKhongDau(txtCusName);
                q = q.Where(x => x.Code.ToLowerOrEmpty().Contains(txtCode)
                    || Erp.BackOffice.Helpers.Common.ChuyenThanhKhongDau(x.CustomerName).Contains(txtCusName));
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
            // tìm kiếm theo status
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
            if ((SalerId != null) && (SalerId > 0))
            {
                q = q.Where(x => x.ManagerStaffId == SalerId);
            }
            ViewBag.SuccessMessage = TempData["SuccessMessage"];
            ViewBag.FailedMessage = TempData["FailedMessage"];
            ViewBag.AlertMessage = TempData["AlertMessage"];
            return View(q);

            
        }
        #endregion


        #region Detail

        public ViewResult DetailChart(int? month, int? year)
        {
            IEnumerable<SalesReturnsViewModel> q = SalesReturnsRepository.GetAllvwSalesReturns()
                .Where(x => x.BranchId == Helpers.Common.CurrentUser.BranchId)
                .AsEnumerable()
                .Select(item => new SalesReturnsViewModel
                {
                    Id = item.Id,
                    CreatedUserId = item.CreatedUserId,
                    CreatedDate = item.CreatedDate,
                    ModifiedUserId = item.ModifiedUserId,
                    ModifiedDate = item.ModifiedDate,
                    Code = item.Code,
                    CustomerCode = item.CustomerCode,
                    CustomerName = item.CustomerName,
                    TotalAmount = item.TotalAmount,
                    //  Discount = item.Discount,
                    TaxFee = item.TaxFee,
                    BranchId = item.BranchId,
                    CustomerId = item.CustomerId,
                    PaymentMethod = item.PaymentMethod,
                    Status = item.Status
                }).OrderByDescending(m => m.ModifiedDate);


            if (month != null)
            {
                q = q.Where(x => x.CreatedDate.Value.Month == month);
            }

            if (year != null)
            {
                q = q.Where(x => x.CreatedDate.Value.Year == year);
            }

            return View(q);
        }

        public ActionResult Detail(int? Id, string TransactionCode)
        {

            var saleReturn = new vwSalesReturns();
            if (Id != null)
                saleReturn = SalesReturnsRepository.GetvwSalesReturnsById(Id.Value);
            if (!string.IsNullOrEmpty(TransactionCode))
                saleReturn = SalesReturnsRepository.GetvwSalesReturnsByTransactionCode(TransactionCode);
            if (saleReturn != null && saleReturn.IsDeleted != true)
            {
                var model = new SalesReturnsViewModel();
                model.InvoiceOld = new ProductInvoiceViewModel();
                model.InvoiceOld.InvoiceList = new List<ProductInvoiceDetailViewModel>();

                model.InvoiceNew = new ProductInvoiceViewModel();

                model.InvoiceNew.InvoiceList = new List<ProductInvoiceDetailViewModel>();

                AutoMapper.Mapper.Map(saleReturn, model);
                //debug
                var InvoiceList = productInvoiceRepository.GetAllvwInvoiceDetails().Where(x => x.ProductInvoiceId == model.ProductInvoiceOldId.Value).ToList();
                AutoMapper.Mapper.Map(InvoiceList, model.InvoiceNew.InvoiceList);
                //
                if (model.ProductInvoiceOldId != null && model.ProductInvoiceOldId.Value > 0)
                {
                    var data = productInvoiceRepository.GetvwProductInvoiceById(model.ProductInvoiceOldId.Value);
                    AutoMapper.Mapper.Map(data, model.InvoiceOld);
                    var detail = productInvoiceRepository.GetAllvwInvoiceDetailsByInvoiceId(model.ProductInvoiceOldId.Value).ToList();
                    AutoMapper.Mapper.Map(detail, model.InvoiceOld.InvoiceList);
                }
                if (model.ProductInvoiceNewId != null && model.ProductInvoiceNewId.Value > 0)
                {
                    var data = productInvoiceRepository.GetvwProductInvoiceById(model.ProductInvoiceNewId.Value);
                    AutoMapper.Mapper.Map(data, model.InvoiceNew);
                    var detail = productInvoiceRepository.GetAllvwInvoiceDetailsByInvoiceId(model.ProductInvoiceNewId.Value).ToList();
                    AutoMapper.Mapper.Map(detail, model.InvoiceNew.InvoiceList);
                }
                //Lấy danh sách invoice detail
                var detailList = SalesReturnsRepository.GetvwAllReturnsDetailsByReturnId(saleReturn.Id).ToList();
                model.DetailList = new List<SalesReturnsDetailViewModel>();
                AutoMapper.Mapper.Map(detailList, model.DetailList);


                model.GroupProduct = model.DetailList.GroupBy(x => new { x.CategoryCode }, (key, group) => new SalesReturnsDetailViewModel
                {
                    CategoryCode = key.CategoryCode,
                    ProductId = group.FirstOrDefault().ProductId,
                    Id = group.FirstOrDefault().Id
                }).ToList();

                //Lấy thông tin kiểm tra cho phép sửa chứng từ này hay không
                model.AllowEdit = Helpers.Common.KiemTraNgaySuaChungTu
                    (model.CreatedDate.Value)
                    && (Filters.SecurityFilter.IsAdmin()
                    || saleReturn.BranchId == Helpers.Common.CurrentUser.BranchId
                    );
                //Lấy lịch sử giao dịch thanh toán
                var listTransaction = transactionLiabilitiesRepository.GetAllvwTransaction()
                            .Where(item => item.BranchId == Helpers.Common.CurrentUser.BranchId.Value && item.MaChungTuGoc == saleReturn.Code)
                            .OrderByDescending(item => item.CreatedDate)
                            .ToList();

                model.ListTransactionLiabilities = new List<TransactionLiabilitiesViewModel>();
                AutoMapper.Mapper.Map(listTransaction, model.ListTransactionLiabilities);

                ViewBag.SuccessMessage = TempData["SuccessMessage"];
                ViewBag.FailedMessage = TempData["FailedMessage"];
                ViewBag.AlertMessage = TempData["AlertMessage"];

                return View(model);
            }

            if (Request.UrlReferrer != null)
                return Redirect(Request.UrlReferrer.AbsoluteUri);
            return RedirectToAction("Index");
        }
        #endregion

        #region Create

        public ActionResult Create(int? Id)
        {

            SalesReturnsViewModel model = new SalesReturnsViewModel();
            model.InvoiceNew = new ProductInvoiceViewModel();
            int taxfee = 0;
            int.TryParse(Helpers.Common.GetSetting("vat"), out taxfee);
            model.InvoiceNew.TaxFee = taxfee;
            model.DetailList = new List<SalesReturnsDetailViewModel>();
            model.InvoiceList = new List<ProductInvoiceDetailViewModel>();
            if (Id != null)
            {
                model.DetailList = SalesReturnsRepository.GetvwAllReturnsDetailsByReturnId(Id.Value)
                            .Select(x => new SalesReturnsDetailViewModel
                            {
                                Id = x.Id,
                                Price = x.Price,
                                ProductId = x.ProductId,
                                ProductName = x.ProductName,
                                ProductCode = x.ProductCode,
                                Quantity = x.Quantity,
                                UnitProduct = x.UnitProduct,
                                Amount = x.Amount,
                                CategoryCode = x.CategoryCode,
                                ProductInvoiceDetailId = x.ProductInvoiceDetailId,
                                SalesReturnsId = x.SalesReturnsId
                            }).ToList();

            }
            var category = categoryRepository.GetAllCategories()
            .Select(item => new CategoryViewModel
            {
                Id = item.Id,
                Code = item.Code,
                Value = item.Value,
                Name = item.Name
            }).Where(x => x.Code == "Origin").ToList();
            ViewBag.category = category;
            ViewBag.FailedMessage = TempData["FailedMessage"];
            return View(model);
        }


        [HttpPost]
        public ActionResult Create(SalesReturnsViewModel model)
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

            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.Snapshot }))
            {
                try
                {
                    if (ModelState.IsValid)
                    {
                        if (model.DetailList.Count > 0)
                        {
                            SalesReturns sale_return = null;

                            if (model.Id > 0)
                            {
                                sale_return = SalesReturnsRepository.GetSalesReturnsById(model.Id);

                            }
                            if (sale_return != null)
                            {
                                //Nếu đã ghi sổ rồi thì không được sửa
                                if (sale_return.IsArchive)
                                {
                                    return RedirectToAction("Detail", new { Id = sale_return.Id });
                                }
                                //productInvoice.Type = check;
                                AutoMapper.Mapper.Map(model, sale_return);
                                SalesReturnsRepository.UpdateSalesReturns(sale_return);

                                //ghi log 
                                Erp.BackOffice.Controllers.HomeController.WriteLog(sale_return.Id, sale_return.Code, "đã cập nhật hàng bán trả", "SalesReturns/Detail/" + sale_return.Id, Helpers.Common.CurrentUser.BranchId.Value);

                            }
                            else
                            {
                                #region Tạo phiếu hàng bán trả lại
                                sale_return = new Domain.Sale.Entities.SalesReturns();
                                AutoMapper.Mapper.Map(model, sale_return);
                                //
                                //var sum = model.DetailList.Sum(x => x.Price * x.Quantity);
                                //sale_return.TotalAmount = sum;
                                //sale_return.AmountPayment = sum;
                                //var invoice_old = productInvoiceRepository.GetProductInvoiceById(model.ProductInvoiceOldId.Value);
                                // sale_return.TotalAmount = invoice_old.TotalAmount;
                                // sale_return.AmountPayment = invoice_old.TotalAmount;
                                //
                                sale_return.IsDeleted = false;
                                sale_return.IsArchive = false;
                                sale_return.CreatedUserId = WebSecurity.CurrentUserId;
                                sale_return.ModifiedUserId = WebSecurity.CurrentUserId;
                                sale_return.CreatedDate = DateTime.Now;
                                sale_return.ModifiedDate = DateTime.Now;
                                sale_return.Status = Wording.OrderStatus_pending;
                                sale_return.BranchId = intBrandID;
                                //thêm 
                                SalesReturnsRepository.InsertSalesReturn(sale_return);
                                //cập nhật lại mã hóa đơn
                                sale_return.Code = Erp.BackOffice.Helpers.Common.GetOrderNo("SalesReturns", model.Code);
                                SalesReturnsRepository.UpdateSalesReturns(sale_return);
                                Erp.BackOffice.Helpers.Common.SetOrderNo("SalesReturns");
                                #endregion

                                #region update lại đơn cũ
                                //đơn hàng cũ sau khi trả 1 phần hàng                            
                                var _updateProductInvoice = productInvoiceRepository.GetProductInvoiceById(model.ProductInvoiceOldId);
                                if (_updateProductInvoice.TotalAmount >= sale_return.AmountPayment.Value && _updateProductInvoice.Status == "complete")
                                {
                                    //Tiền từ hàng trả chỉ bị trừ khi chuyển tiền
                                    //_updateProductInvoice.DoanhThu = _updateProductInvoice.TotalAmount - sale_return.AmountPayment.Value;
                                    _updateProductInvoice.RemainingAmount = 0;
                                    _updateProductInvoice.ProductInvoiceId_Return = sale_return.Id;
                                    _updateProductInvoice.Note = "<b>Đơn trả: </b>" + sale_return.Code;
                                    productInvoiceRepository.UpdateProductInvoice(_updateProductInvoice);
                                }
                                #endregion
                                //update lại membership nếu có 
                                var membershiparenpupdate = Membership_parentRepository.GetAllMembership_parentByInvoiceId(model.ProductInvoiceOldId.Value);
                                if (membershiparenpupdate != null)
                                {

                                    List<long> parentID = new List<long>();
                                    foreach(var a in membershiparenpupdate)
                                    {
                                        parentID.Add(a.Id);
                                    }    

                                    foreach (var item in model.DetailList.Where(x => x.Id == 0 && x.ProductId > 0))
                                    {
                                        
                                        var membershpupdate = membershipRepository.GetAllMembership().Where(x => x.Status == "pending" && parentID.Contains(x.IdParent)).ToList();
                                        if (membershpupdate != null && membershpupdate.Count() > 0)
                                        {
                                            if (membershpupdate.Count() >= item.Quantity.Value)
                                            {
                                                for (int i = 0; i < item.Quantity.Value; i++)
                                                {
                                                    membershpupdate[i].Status = "TraHang";
                                                    membershipRepository.UpdateMembership(membershpupdate[i]);
                                                }

                                            }
                                        }
                                    }
                                    



                                }

                            }
                            var _listdata = SalesReturnsRepository.GetAllReturnsDetailsByReturnId(sale_return.Id).ToList();
                            if (model.DetailList.Any(x => x.Id == 0))
                            {
                                //lưu danh sách thao tác thực hiện dịch vụ
                                foreach (var item in model.DetailList.Where(x => x.Id == 0 && x.ProductId > 0))
                                {
                                    var ins = new SalesReturnsDetail();
                                    ins.IsDeleted = false;
                                    ins.CreatedUserId = WebSecurity.CurrentUserId;
                                    ins.ModifiedUserId = WebSecurity.CurrentUserId;
                                    ins.AssignedUserId = WebSecurity.CurrentUserId;
                                    ins.CreatedDate = DateTime.Now;
                                    ins.ModifiedDate = DateTime.Now;
                                    ins.ExpiryDate = item.ExpiryDate;
                                    ins.LoCode = item.LoCode;
                                    ins.ProductId = item.ProductId;
                                    ins.SalesReturnsId = sale_return.Id;
                                    ins.Price = item.Price;
                                    ins.Quantity = item.Quantity;
                                    ins.Unit = item.Unit;
                                    //
                                    //ins.Amount = item.Amount;
                                    ins.Amount = (item.Quantity * item.Price);
                                    ins.ProductInvoiceDetailId = item.ProductInvoiceDetailId;
                                    SalesReturnsRepository.InsertSalesReturnsDetail(ins);
                                }

                            }

                            var _delete = _listdata.Where(id1 => !model.DetailList.ToList().Any(id2 => id2.Id == id1.Id)).ToList();
                            if (_delete.Any())
                            {
                                foreach (var item in _delete)
                                {
                                    SalesReturnsRepository.DeleteSalesReturnsDetail(item.Id);
                                }
                            }
                            if (model.DetailList.Any(x => x.Id > 0))
                            {
                                var update = _listdata.Where(id1 => model.DetailList.ToList().Any(id2 => id2.Id == id1.Id)).ToList();
                                //lưu danh sách thao tác thực hiện dịch vụ
                                foreach (var item in model.DetailList.Where(x => x.Id > 0 && x.ProductId > 0))
                                {
                                    var _update = update.FirstOrDefault(x => x.Id == item.Id);
                                    _update.ExpiryDate = item.ExpiryDate;
                                    _update.LoCode = item.LoCode;
                                    _update.ProductId = item.ProductId;
                                    _update.SalesReturnsId = sale_return.Id;
                                    _update.Price = item.Price;
                                    _update.Quantity = item.Quantity;
                                    _update.Unit = item.Unit;
                                    _update.Amount = item.Amount;
                                    _update.ProductInvoiceDetailId = item.ProductInvoiceDetailId;
                                    SalesReturnsRepository.UpdateSalesReturnsDetail(_update);
                                }
                            }

                            // Tra MemberShip
                            //var productInvoidce = productInvoiceRepository.GetProductInvoiceById(model.ProductInvoiceOldId);
                            //IEnumerable<MembershipViewModel> q = membershipRepository.GetvwAllMembership().Where(x => x.BranchId==Helpers.Common.CurrentUser.BranchId.Value)
                            // .Select(item => new MembershipViewModel
                            // {
                            //     Id = item.Id,
                            //     BranchId = item.BranchId,
                            //     Status = item.Status,
                            //     TargetCode = item.TargetCode,
                            //     TargetId = item.TargetId,
                            //     Code = item.Code,
                            //     IsDeleted = item.IsDeleted,
                            //     CreatedDate = item.CreatedDate,
                            //     CreatedUserId = item.CreatedUserId,
                            //     ModifiedDate = item.ModifiedDate,
                            //     ModifiedUserId = item.ModifiedUserId,
                            //     AssignedUserId = item.AssignedUserId,
                            //     CustomerId = item.CustomerId,
                            //     ProductId = item.ProductId,
                            //     TargetModule = item.TargetModule,
                            //     ExpiryDate = item.ExpiryDate,
                            //     Type = item.Type
                            // }).ToList();


                            //var Membership = new Erp.Domain.Sale.Entities.Membership();

                            ////begin hoapd cap nhat lai membership tra
                            //if (model.DetailList.Count > 0)
                            //{
                            //    foreach (var item in model.DetailList.Where(x =>x.ProductId > 0))
                            //    {
                            //      var listMBS=  q.Where(x => x.Status == "pending" && x.CustomerId == productInvoidce.CustomerId && x.TargetCode == productInvoidce.Code && x.TargetModule== "ProductInvoiceDetail" && x.TargetId== item.ProductInvoiceDetailId);
                            //      if (listMBS.Count()>0)
                            //        {
                            //            int Solantra= listMBS.Count()<= item.Quantity? listMBS.Count():  Helpers.Common.NVL_NUM_NT_NEW(item.Quantity);

                            //            int idem=0;
                            //            foreach (var itemMBS in listMBS)
                            //            {
                            //                itemMBS.Status = "TraHang";
                            //                itemMBS.ModifiedUserId = WebSecurity.CurrentUserId;
                            //                itemMBS.ModifiedDate = DateTime.Now;
                            //                //membershipRepository.UpdateMembership(itemMBS);
                            //            }


                            //        }
                            //    }
                            //}
                            ////end hoapd cap nhat lai membership tra


                            //if (model.InvoiceList.Any(x => x.ProductId > 0))
                            //{
                            //    model.InvoiceNew.BranchId = sale_return.BranchId;
                            //    model.InvoiceNew.CustomerId = sale_return.CustomerId;
                            //    model.InvoiceNew.IsArchive = false;
                            //    model.InvoiceNew.DoanhThu = model.AmountReceipt;
                            //    model.InvoiceNew.TotalAmount = model.InvoiceNew.TongTienSauVAT;
                            //    sale_return.ProductInvoiceNewId = AutoCreateProductInvoice(productInvoiceRepository, model.InvoiceNew, model.InvoiceList).Id;
                            //}
                            SalesReturnsRepository.UpdateSalesReturns(sale_return);

                            //ghi log 
                            Erp.BackOffice.Controllers.HomeController.WriteLog(sale_return.Id, sale_return.Code, "đã tạo hàng bán trả", "SalesReturns/Detail/" + sale_return.Id, Helpers.Common.CurrentUser.BranchId.Value);

                            scope.Complete();
                            return RedirectToAction("Detail", new { Id = sale_return.Id });
                        }
                    }
                }
                catch (DbUpdateException ex)
                {
                    TempData[Globals.FailedMessageKey] = App_GlobalResources.Wording.Error;
                    return View(model);
                }
            }
            return View(model);
        }

        //#region CreateDetailReturnAndDetailInbound
        //public void CreateDetailReturnAndDetailInbound(int ProductId, int? DisCount, int Quantity, int? quantity_return, decimal Price,
        //                         int ProductInvoiceId, int ProductInvoiceDetailId, int SaleReturnId, int ProductInboundId, string SaleReturnCode,
        //                         string ProductInvoiceCode, int? WarehouseDestinationId, string PaymentMethod, string customerCode, List<string> list,
        //                         List<ProductInvoiceDetailViewModel> listinvoice, List<ProductInboundDetail> list_inbound, bool IsReturn)
        //{
        //    //đánh đấu vào hóa đơn là có trả hàng hóa
        //    var invoiceDetail = productInvoiceRepository.GetProductInvoiceDetailById(ProductInvoiceDetailId);
        //    invoiceDetail.IsReturn = true;
        //    invoiceDetail.QuantitySaleReturn = quantity_return;
        //    productInvoiceRepository.UpdateProductInvoiceDetail(invoiceDetail);
        //    decimal total_Amount = Convert.ToDecimal((Quantity * invoiceDetail.Price));
        //    decimal discount = (total_Amount * (DisCount.HasValue ? DisCount.Value : 0)) / 100;
        //    total_Amount = total_Amount - discount;
        //    var SalesReturnsDetail = new SalesReturnsDetail();
        //    //hàng bán trả lại
        //    SalesReturnsDetail.IsDeleted = false;
        //    SalesReturnsDetail.CreatedUserId = WebSecurity.CurrentUserId;
        //    SalesReturnsDetail.ModifiedUserId = WebSecurity.CurrentUserId;
        //    SalesReturnsDetail.CreatedDate = DateTime.Now;
        //    SalesReturnsDetail.ModifiedDate = DateTime.Now;
        //    SalesReturnsDetail.ProductInvoiceDetailId = ProductInvoiceDetailId;
        //    SalesReturnsDetail.ProductInvoiceId = ProductInvoiceId;
        //    SalesReturnsDetail.SalesReturnsId = SaleReturnId;
        //    SalesReturnsDetail.Quantity = Quantity;
        //    SalesReturnsDetail.ProductId = ProductId;
        //    SalesReturnsDetail.DisCount = DisCount;
        //    SalesReturnsDetail.DisCountAmount = Convert.ToInt32(discount);
        //    SalesReturnsDetail.IsReturn = IsReturn;
        //    SalesReturnsRepository.InsertSalesReturnsDetail(SalesReturnsDetail);
        //    TempData[Globals.SuccessMessageKey] = App_GlobalResources.Wording.InsertSuccess;

        //    //phiếu nhập
        //    var productInboundDetail = new ProductInboundDetail();
        //    productInboundDetail.IsDeleted = false;
        //    productInboundDetail.CreatedUserId = WebSecurity.CurrentUserId;
        //    productInboundDetail.ModifiedUserId = WebSecurity.CurrentUserId;
        //    productInboundDetail.CreatedDate = DateTime.Now;
        //    productInboundDetail.ModifiedDate = DateTime.Now;
        //    productInboundDetail.ProductInboundId = ProductInboundId;
        //    productInboundDetail.ProductId = SalesReturnsDetail.ProductId;
        //    productInboundDetail.Quantity = SalesReturnsDetail.Quantity;
        //    productInboundDetail.Price = Price;
        //    list_inbound.Add(productInboundDetail);



        //    //Thêm vào quản lý chứng từ
        //    TransactionController.Create(new TransactionViewModel
        //    {
        //        TransactionModule = "SalesReturns",
        //        TransactionCode = SaleReturnCode,
        //        TransactionName = "Hàng bán trả lại"
        //    });

        //    //lưu lại mã hóa đơn để kiểm tra, trùng thì ko lưu chứng từ liên quan
        //    if (list.Where(x => x.ToString() == ProductInvoiceCode.ToString()).Count() <= 0)
        //    {
        //        list.Add(ProductInvoiceCode);
        //        //Thêm chứng từ liên quan
        //        TransactionController.CreateRelationship(new TransactionRelationshipViewModel
        //        {
        //            TransactionA = SaleReturnCode,
        //            TransactionB = ProductInvoiceCode
        //        });
        //    }


        //    //dựa vào mã hóa đơn nếu trùng mã hóa đơn thì lấy ra model trùng đang lưu số tiền cập nhật lại, không trùng mã thì thêm vào bình thường
        //    var listproduct = listinvoice.Where(x => x.ProductInvoiceCode == ProductInvoiceCode);
        //    //list lưu lại số tiền của sản phẩm để tính giảm trừ công nợ của từng hóa đơn

        //    var model = new ProductInvoiceDetailViewModel();
        //    model.ProductInvoiceCode = ProductInvoiceCode;
        //    if (listproduct.Count() > 0)
        //    {
        //        model.Amount = total_Amount + listproduct.FirstOrDefault().Amount;
        //        listinvoice.Remove(listproduct.FirstOrDefault());
        //    }
        //    else
        //    {
        //        model.Amount = total_Amount;
        //    }

        //    listinvoice.Add(model);

        //}
        //#endregion

        #endregion

        #region SearchProductInvoice
        [AllowAnonymous]
        public PartialViewResult SearchProductInvoice(int id)
        {
            var model = new ProductInvoiceViewModel();
            model.Id = id;
            return PartialView(model);
        }
        #endregion

        #region Print
        //public ActionResult Print(int Id, bool ExportExcel = false)
        //{
        //    var model = new TemplatePrintViewModel();
        //    lấy logo công ty
        //    var logo = Erp.BackOffice.Helpers.Common.GetSetting("LogoCompany");
        //    var company = Erp.BackOffice.Helpers.Common.GetSetting("companyName");
        //    var address = Erp.BackOffice.Helpers.Common.GetSetting("addresscompany");
        //    var phone = Erp.BackOffice.Helpers.Common.GetSetting("phonecompany");
        //    var fax = Erp.BackOffice.Helpers.Common.GetSetting("faxcompany");
        //    var bankcode = Erp.BackOffice.Helpers.Common.GetSetting("bankcode");
        //    var bankname = Erp.BackOffice.Helpers.Common.GetSetting("bankname");
        //    var ImgLogo = "<div class=\"logo\"><img src=" + logo + " height=\"73\" /></div>";
        //    lấy hóa đơn.
        //    var salesReturns = SalesReturnsRepository.GetvwSalesReturnsById(Id);
        //    lấy thông tin khách hàng
        //    var customer = customerRepository.GetvwCustomerByCode(salesReturns.CustomerCode);
        //    lấy người lập phiếu xuất kho
        //    var user = userRepository.GetUserById(salesReturns.SalerId.Value);
        //    List<SalesReturnsDetailViewModel> detailList = new List<SalesReturnsDetailViewModel>();
        //    if (salesReturns != null && salesReturns.IsDeleted != true)
        //    {
        //        lấy danh sách sản phẩm xuất kho
        //        detailList = SalesReturnsRepository.GetAllReturnsDetailsByReturnId(Id)
        //                .Select(x => new SalesReturnsDetailViewModel
        //                {
        //                    Id = x.Id,
        //                    Price = x.Price,
        //                    ProductId = x.ProductId,
        //                    ProductName = x.ProductName,
        //                    ProductCode = x.ProductCode,
        //                    Quantity = x.Quantity,
        //                    UnitProduct = x.UnitProduct,
        //                    DisCount = x.DisCount.HasValue ? x.DisCount.Value : 0,
        //                    DisCountAmount = x.DisCountAmount.HasValue ? x.DisCountAmount : 0,
        //                    ProductGroup = x.ProductGroup,
        //                    ProductInvoiceCode = x.ProductInvoiceCode,
        //                    ProductInvoiceDate = x.ProductInvoiceDate,
        //                    ProductInvoiceDetailId = x.ProductInvoiceDetailId,
        //                    ProductInvoiceId = x.ProductInvoiceId,
        //                    SalesReturnsId = x.SalesReturnsId,
        //                    SalesReturnDate = x.SalesReturnDate,
        //                    SalerInvoiceName = x.SalerInvoiceName
        //                }).ToList();
        //    }

        //    tạo dòng của table html danh sách sản phẩm.
        //    var ListRow = "";
        //    int tong_tien = 0;
        //    int da_thanh_toan = 0;
        //    int con_lai = 0;
        //    var groupProduct = detailList.GroupBy(x => new { x.ProductGroup }, (key, group) => new ProductInvoiceDetailViewModel
        //    {
        //        ProductGroup = key.ProductGroup,
        //        ProductId = group.FirstOrDefault().ProductId,
        //        Id = group.FirstOrDefault().Id
        //    }).ToList();
        //    var Rows = "";
        //    var ProductGroupName = new Category();
        //    foreach (var i in groupProduct)
        //    {
        //        var count = detailList.Where(x => x.ProductGroup == i.ProductGroup).ToList();
        //        var chiet_khau1 = count.Sum(x => x.DisCountAmount.HasValue ? x.DisCountAmount.Value : 0);
        //        decimal? subTotal1 = count.Sum(x => (x.Quantity) * (x.Price));
        //        var thanh_tien1 = subTotal1 - chiet_khau1;
        //        if (!string.IsNullOrEmpty(i.ProductGroup))
        //        {
        //            ProductGroupName = categoryRepository.GetCategoryByCode("ProductGroup").Where(x => x.Value == i.ProductGroup).FirstOrDefault();

        //            Rows = "<tr style=\"background:#eee;font-weight:bold\"><td colspan=\"6\" class=\"text-left\">" + (i.ProductGroup == null ? "" : i.ProductGroup) + ": " + (ProductGroupName.Name == null ? "" : ProductGroupName.Name) + "</td><td class=\"text-right\">"
        //                 + Erp.BackOffice.Helpers.Common.PhanCachHangNgan2(count.Sum(x => x.Quantity))
        //                 + "</td><td colspan=\"2\" class=\"text-right\"></td><td class=\"text-right\">"
        //                 + Erp.BackOffice.Helpers.Common.PhanCachHangNgan2(thanh_tien1)
        //                 + "</td></tr>";
        //        }
        //        ListRow += Rows;
        //        int index = 1;
        //        foreach (var item in detailList.Where(x => x.ProductGroup == i.ProductGroup))
        //        {
        //            decimal? subTotal = item.Quantity * item.Price.Value;
        //            var chiet_khau = item.DisCountAmount.HasValue ? item.DisCountAmount.Value : 0;
        //            var thanh_tien = subTotal - chiet_khau;

        //            var Row = "<tr>"
        //             + "<td class=\"text-center\">" + (index++) + "</td>"
        //             + "<td class=\"text-right\">" + item.ProductCode + "</td>"
        //             + "<td class=\"text-left\">" + item.ProductName + "</td>"
        //             + "<td class=\"text-left\">" + item.ProductInvoiceCode + " (" + item.ProductInvoiceDate.Value.ToString("dd/MM/yyyy HH:mm") + ")" + "</td>"
        //              + "<td class=\"text-center\">" + (item.LoCode == null ? "" : item.LoCode) + "</td>"
        //                + "<td class=\"text-center\">" + (item.ExpiryDate == null ? "" : item.ExpiryDate.Value.ToShortDateString()) + "</td>"
        //             + "<td class=\"text-center\">" + item.UnitProduct + "</td>"
        //             + "<td class=\"text-right\">" + item.Quantity.Value + "</td>"
        //             + "<td class=\"text-right\">" + Erp.BackOffice.Helpers.Common.PhanCachHangNgan2(item.Price) + "</td>"
        //             + "<td class=\"text-right\">" + (item.DisCount.HasValue ? item.DisCount.Value : 0) + "</td>"
        //             + "<td class=\"text-right\">" + Erp.BackOffice.Helpers.Common.PhanCachHangNgan2(chiet_khau) + "</td>"
        //             + "<td class=\"text-right\">" + Erp.BackOffice.Helpers.Common.PhanCachHangNgan2(thanh_tien) + "</td></tr>";
        //            ListRow += Row;
        //        }
        //    }

        //    khởi tạo table html.                
        //    var table = "<table class=\"invoice-detail\"><thead><tr> <th>STT</th> <th>Mã hàng</th><th>Tên mặt hàng</th><th>Hóa đơn BH</th><th>ĐVT</th><th>Số lượng</th><th>Đơn giá</th><th>% CK</th><th>Trị giá chiết khấu</th><th>Thành tiền</th></tr></thead><tbody>"
        //                 + ListRow
        //                 + "<tr><td></td><td></td><td></td><td></td><td></td><td></td><td></td><td></td><td></td><td></td><td></td></tr>"
        //                 + "<tr><td></td><td></td><td></td><td></td><td></td><td></td><td></td><td></td><td></td><td></td><td></td></tr>"
        //                 + "</tbody><tfoot>"
        //                 + "<tr><td colspan=\"6\" class=\"text-right\"></td><td class=\"text-right\">"
        //                 + Erp.BackOffice.Helpers.Common.PhanCachHangNgan2(detailList.Sum(x => x.Quantity))
        //                 + "</td><td colspan=\"2\" class=\"text-right\">Tổng cộng</td><td class=\"text-right\">"
        //                 + Erp.BackOffice.Helpers.Common.PhanCachHangNgan2(salesReturns.TotalAmount.Value)
        //                 + "</td></tr>"
        //                   + "<tr><td colspan=\"10\" class=\"text-right\">VAT (" + vat + "%)</td><td class=\"text-right\">"
        //                 + Erp.BackOffice.Helpers.Common.PhanCachHangNgan2(totalVAT)
        //                 + "</td></tr>"
        //                  + "<tr><td colspan=\"10\" class=\"text-right\">Tổng tiền phải thanh toán</td><td class=\"text-right\">"
        //                 + Erp.BackOffice.Helpers.Common.PhanCachHangNgan2(total)
        //                 + "</td></tr>"

        //                 + "</tfoot></table>";

        //    lấy template phiếu xuất.
        //    var template = templatePrintRepository.GetAllTemplatePrint().Where(x => x.Code == "SalesReturns").OrderByDescending(x => x.CreatedDate).FirstOrDefault();
        //    truyền dữ liệu vào template.
        //    model.Content = template.Content;
        //    model.Content = model.Content.Replace("{InvoiceCode}", salesReturns.Code);
        //    model.Content = model.Content.Replace("{Day}", salesReturns.CreatedDate.Value.Day.ToString());
        //    model.Content = model.Content.Replace("{Month}", salesReturns.CreatedDate.Value.Month.ToString());
        //    model.Content = model.Content.Replace("{Year}", salesReturns.CreatedDate.Value.Year.ToString());
        //    model.Content = model.Content.Replace("{CustomerName}", customer.LastName + " " + customer.FirstName);
        //    model.Content = model.Content.Replace("{CustomerPhone}", customer.Phone);
        //    model.Content = model.Content.Replace("{CompanyName}", customer.CompanyName);

        //    if (!string.IsNullOrEmpty(customer.Address))
        //    {
        //        model.Content = model.Content.Replace("{Address}", customer.Address + ", ");
        //    }
        //    else
        //    {
        //        model.Content = model.Content.Replace("{Address}", "");
        //    }
        //    if (!string.IsNullOrEmpty(customer.DistrictName))
        //    {
        //        model.Content = model.Content.Replace("{District}", customer.DistrictName + ", ");
        //    }
        //    else
        //    {
        //        model.Content = model.Content.Replace("{District}", "");
        //    }
        //    if (!string.IsNullOrEmpty(customer.WardName))
        //    {
        //        model.Content = model.Content.Replace("{Ward}", customer.WardName + ", ");
        //    }
        //    else
        //    {
        //        model.Content = model.Content.Replace("{Ward}", "");
        //    }
        //    if (!string.IsNullOrEmpty(customer.ProvinceName))
        //    {
        //        model.Content = model.Content.Replace("{Province}", customer.ProvinceName);
        //    }
        //    else
        //    {
        //        model.Content = model.Content.Replace("{Province}", "");
        //    }

        //    model.Content = model.Content.Replace("{Note}", salesReturns.Note);
        //      model.Content = model.Content.Replace("{InvoiceCode}", productInvoice.Code);
        //    model.Content = model.Content.Replace("{SaleName}", user.FullName);
        //      model.Content = model.Content.Replace("{CodeInvoiceRed}", productInvoice.CodeInvoiceRed);
        //    model.Content = model.Content.Replace("{PaymentMethod}", salesReturns.PaymentMethod);
        //    model.Content = model.Content.Replace("{MoneyText}", Erp.BackOffice.Helpers.Common.ChuyenSoThanhChu_2(Convert.ToInt32(salesReturns.TotalAmount.Value)));

        //    model.Content = model.Content.Replace("{DataTable}", table);
        //    model.Content = model.Content.Replace("{System.Logo}", ImgLogo);
        //    model.Content = model.Content.Replace("{System.CompanyName}", company);
        //    model.Content = model.Content.Replace("{System.AddressCompany}", address);
        //    model.Content = model.Content.Replace("{System.PhoneCompany}", phone);
        //    model.Content = model.Content.Replace("{System.Fax}", fax);
        //    model.Content = model.Content.Replace("{System.BankCodeCompany}", bankcode);
        //    model.Content = model.Content.Replace("{System.BankNameCompany}", bankname);

        //    if (ExportExcel)
        //    {
        //        Response.AppendHeader("content-disposition", "attachment;filename=" + salesReturns.CreatedDate.Value.ToString("yyyyMMdd") + salesReturns.Code + ".xls");
        //        Response.Charset = "";
        //        Response.Cache.SetCacheability(HttpCacheability.NoCache);
        //        Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        //        Response.Write(model.Content);
        //        Response.End();
        //    }

        //    return View(model);
        //}
        #endregion

        public ProductInvoice AutoCreateProductInvoice(IProductInvoiceRepository productInvoiceRepository, ProductInvoiceViewModel model, List<ProductInvoiceDetailViewModel> DetailList)
        {
            ProductInvoice invoice = null;
            if (model.Id > 0)
            {
                invoice = productInvoiceRepository.GetProductInvoiceById(model.Id);
            }
            if (invoice != null)
            {
                AutoMapper.Mapper.Map(model, invoice);
                productInvoiceRepository.UpdateProductInvoice(invoice);
            }
            else
            {
                #region Tạo phiếu hàng bán trả lại
                invoice = new Domain.Sale.Entities.ProductInvoice();
                AutoMapper.Mapper.Map(model, invoice);
                invoice.IsDeleted = false;
                invoice.IsArchive = false;
                invoice.CreatedUserId = WebSecurity.CurrentUserId;
                invoice.ModifiedUserId = WebSecurity.CurrentUserId;
                invoice.CreatedDate = DateTime.Now;
                invoice.ModifiedDate = DateTime.Now;
                invoice.Status = Wording.OrderStatus_pending;
                invoice.BranchId = Helpers.Common.CurrentUser.BranchId.Value;
                invoice.Type = "invoice_return";
                invoice.Status = "pending";
                //thêm 
                productInvoiceRepository.InsertProductInvoice(invoice, null);
                //cập nhật lại mã hóa đơn
                invoice.Code = Erp.BackOffice.Helpers.Common.GetOrderNo("ProductInvoice", model.Code);
                productInvoiceRepository.UpdateProductInvoice(invoice);
                Erp.BackOffice.Helpers.Common.SetOrderNo("ProductInvoice");
                #endregion
            }
            var _listdata = productInvoiceRepository.GetAllInvoiceDetailsByInvoiceId(invoice.Id).ToList();
            if (DetailList.Any(x => x.Id == 0))
            {
                //lưu danh sách thao tác thực hiện dịch vụ
                foreach (var item in DetailList.Where(x => x.Id == 0 && x.ProductId > 0))
                {
                    var ins = new ProductInvoiceDetail();
                    ins.IsDeleted = false;
                    ins.CreatedUserId = WebSecurity.CurrentUserId;
                    ins.ModifiedUserId = WebSecurity.CurrentUserId;
                    ins.CreatedDate = DateTime.Now;
                    ins.ModifiedDate = DateTime.Now;
                    ins.ExpiryDate = item.ExpiryDate;
                    ins.LoCode = item.LoCode;
                    ins.ProductId = item.ProductId.Value;
                    ins.ProductInvoiceId = invoice.Id;
                    ins.Price = item.Price;
                    ins.Quantity = item.Quantity;
                    ins.Unit = item.Unit;
                    // ins.ProductType = item.ProductType;
                    productInvoiceRepository.InsertProductInvoiceDetail(ins);
                }
            }
            var _delete = _listdata.Where(id1 => !DetailList.ToList().Any(id2 => id2.Id == id1.Id)).ToList();
            if (_delete.Any())
            {
                foreach (var item in _delete)
                {
                    productInvoiceRepository.DeleteProductInvoiceDetail(item.Id);
                }
            }
            if (DetailList.Any(x => x.Id > 0))
            {
                var update = _listdata.Where(id1 => DetailList.ToList().Any(id2 => id2.Id == id1.Id)).ToList();
                //lưu danh sách thao tác thực hiện dịch vụ
                foreach (var item in DetailList.Where(x => x.Id > 0 && x.ProductId > 0))
                {
                    var _update = update.FirstOrDefault(x => x.Id == item.Id);

                    _update.ExpiryDate = item.ExpiryDate;
                    _update.LoCode = item.LoCode;
                    _update.ProductId = item.ProductId.Value;
                    _update.Price = item.Price;
                    _update.Quantity = item.Quantity;
                    _update.Unit = item.Unit;
                    productInvoiceRepository.UpdateProductInvoiceDetail(_update);
                }
            }

            //Thêm vào quản lý chứng từ
            TransactionController.Create(new TransactionViewModel
            {
                TransactionModule = "ProductInvoice",
                TransactionCode = invoice.Code,
                TransactionName = "Bán hàng"
            });
            return invoice;
        }

        #region Success
        [HttpPost]
        public ActionResult Success(SalesReturnsViewModel model)
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.Snapshot }))
            {
                try
                {
                    var PaymentMethod = SelectListHelper.GetSelectList_Category("FormPayment", null, "Name", null).FirstOrDefault().Value;

                    var sale_return = SalesReturnsRepository.GetSalesReturnsById(model.Id);
                    var invoice_detail_list = SalesReturnsRepository.GetvwAllReturnsDetailsByReturnId(sale_return.Id).ToList();
                    var invoice_old = productInvoiceRepository.GetvwProductInvoiceById(sale_return.ProductInvoiceOldId.Value);
                    
                    var q = Membership_parentRepository.GetAllMembership_parent().Where(item => item.ProductInvoiceId == invoice_old.Id).FirstOrDefault();
                    
                    //Xóa membership hàng trả 
                    //if (q != null)
                    //{
                    //    var membershpupdate = membershipRepository.GetListAllMembershipByIdParent(q.Id).Where(x => x.Status == "pending").ToList();
                    //    if (membershpupdate == null || membershpupdate.Count() == 0)
                    //    {
                    //        List<Membership_parent> updateMers = Membership_parentRepository.GetListMembership_parentById(q.Id);
                    //        foreach (var item in updateMers)
                    //        {
                    //            item.IsDeleted = true;
                    //            Membership_parentRepository.UpdateMembership_parent(item);
                    //        }


                    //    }
                    //}
                    var invoice_new = new ProductInvoice();

                    //debug //cập nhật ghi chú đơn hàng bán
                    //var invoice_old2 = productInvoiceRepository.GetProductInvoiceById(invoice_old.Id);
                    //invoice_old2.Note = "Đơn hàng đã trả - ";
                    //productInvoiceRepository.UpdateProductInvoice(invoice_old2);

                    //
                    var invoice_detail_new = new List<vwProductInvoiceDetail>();
                    if (sale_return.ProductInvoiceNewId != null)
                    {
                        invoice_new = productInvoiceRepository.GetProductInvoiceById(sale_return.ProductInvoiceNewId.Value);
                        invoice_detail_new = productInvoiceRepository.GetAllvwInvoiceDetailsByInvoiceId(sale_return.ProductInvoiceNewId.Value).Where(x => x.ProductType == "product").ToList();
                    }
                    ProductInbound ins_inbound = new ProductInbound();
                    ProductOutbound ins_outbound = new ProductOutbound();

                    string check = "";
                    //foreach (var item in invoice_detail_list)
                    //{
                    //    var error = InventoryController.Check(item.ProductName, item.ProductId.Value, item.LoCode, item.ExpiryDate, sale_return.WarehouseDestinationId.Value,item.Quantity.Value, 0 );
                    //    check += error;
                    //}
                    if (!string.IsNullOrEmpty(check))
                    {
                        TempData[Globals.FailedMessageKey] = App_GlobalResources.Wording.ArchiveFail + check;
                        return RedirectToAction("Detail", new { Id = model.Id });
                    }
                    #region  phiếu nhập ok
                    var product_inbound = productInboundRepository.GetAllProductInbound().Where(x => x.SalesReturnsId == sale_return.Id).ToList();

                    var product_outbound = productOutboundRepository.GetAllProductOutbound().Where(x => x.InvoiceId == sale_return.ProductInvoiceNewId).ToList();
                    #endregion
                    if (!sale_return.IsArchive)
                    {
                        //if (sale_return.AmountReceipt > 0)
                        //{
                        //    #region phiếu thu
                        //    //Khách thanh toán ngay
                        //    if (sale_return.AmountReceipt > 0)
                        //    {
                        //        var customer = customerRepository.GetvwCustomerById(sale_return.CustomerId.Value);
                        //        #region xóa phiếu thu cũ (nếu có)
                        //        var receipt = ReceiptRepository.GetAllReceiptFull()
                        //            .Where(item => item.BranchId == Helpers.Common.CurrentUser.BranchId.Value && item.MaChungTuGoc == sale_return.Code).ToList();
                        //        #endregion
                        //        if (receipt.Count() > 0)
                        //        {
                        //            #region cập nhật phiếu thu cũ
                        //            // isDelete phiếu thu
                        //            var receipts = receipt.FirstOrDefault();
                        //            receipts.IsDeleted = false;
                        //            receipts.Payer = customer.LastName + " " + customer.FirstName;
                        //            receipts.PaymentMethod = invoice_new.PaymentMethod;
                        //            receipts.ModifiedDate = DateTime.Now;
                        //            receipts.VoucherDate = DateTime.Now;
                        //            receipts.Amount = sale_return.AmountReceipt;

                        //            ReceiptRepository.UpdateReceipt(receipts);

                        //            //Ghi Có TK 131 - Phải thu của khách hàng.
                        //            Erp.BackOffice.Account.Controllers.TransactionLiabilitiesController.Create(
                        //                receipts.Code,
                        //                "Receipt",
                        //                "Thu tiền khách hàng",
                        //                customer.Code,
                        //                "Customer",
                        //                0,
                        //                Convert.ToDecimal(sale_return.AmountReceipt),
                        //                invoice_new.Code,
                        //                "ProductInvoice",
                        //                invoice_new.PaymentMethod,
                        //                null,
                        //                null);
                        //            #endregion
                        //        }
                        //        else
                        //        {
                        //            #region thêm mới phiếu thu
                        //            //Lập phiếu thu
                        //            var receipt_inser = new Receipt();
                        //            receipt_inser.IsDeleted = false;
                        //            receipt_inser.CreatedUserId = WebSecurity.CurrentUserId;
                        //            receipt_inser.ModifiedUserId = WebSecurity.CurrentUserId;
                        //            receipt_inser.AssignedUserId = WebSecurity.CurrentUserId;
                        //            receipt_inser.CreatedDate = DateTime.Now;
                        //            receipt_inser.ModifiedDate = DateTime.Now;
                        //            receipt_inser.VoucherDate = DateTime.Now;
                        //            receipt_inser.CustomerId = customer.Id;
                        //            receipt_inser.Payer = customer.LastName + " " + customer.FirstName;
                        //            receipt_inser.PaymentMethod = invoice_new.PaymentMethod;
                        //            receipt_inser.Address = customer.Address;
                        //            receipt_inser.MaChungTuGoc = invoice_new.Code;
                        //            receipt_inser.LoaiChungTuGoc = "ProductInvoice";
                        //            receipt_inser.Note = receipt_inser.Name;
                        //            receipt_inser.BranchId = Helpers.Common.CurrentUser.BranchId.Value;
                        //            receipt_inser.Amount = sale_return.AmountReceipt;

                        //            ReceiptRepository.InsertReceipt(receipt_inser);
                        //            receipt_inser.Code = Erp.BackOffice.Helpers.Common.GetOrderNo("Receipt", model.Code);
                        //            ReceiptRepository.UpdateReceipt(receipt_inser);
                        //            Erp.BackOffice.Helpers.Common.SetOrderNo("Receipt");

                        //            //Thêm vào quản lý chứng từ
                        //            TransactionController.Create(new TransactionViewModel
                        //            {
                        //                TransactionModule = "Receipt",
                        //                TransactionCode = receipt_inser.Code,
                        //                TransactionName = "Trả tiền cho đơn hàng bán trả lại"
                        //            });

                        //            //Thêm chứng từ liên quan
                        //            TransactionController.CreateRelationship(new TransactionRelationshipViewModel
                        //            {
                        //                TransactionA = receipt_inser.Code,
                        //                TransactionB = invoice_new.Code
                        //            });
                        //            TransactionController.CreateRelationship(new TransactionRelationshipViewModel
                        //            {
                        //                TransactionA = receipt_inser.Code,
                        //                TransactionB = sale_return.Code
                        //            });
                        //            //Ghi Có TK 131 - Phải thu của khách hàng.
                        //            Erp.BackOffice.Account.Controllers.TransactionLiabilitiesController.Create(
                        //                receipt_inser.Code,
                        //                "Receipt",
                        //                "Thu tiền khách hàng",
                        //                customer.Code,
                        //                "Customer",
                        //                0,
                        //                Convert.ToDecimal(sale_return.AmountReceipt),
                        //                invoice_new.Code,
                        //                "ProductInvoice",
                        //                invoice_new.PaymentMethod,
                        //                null,
                        //                null);

                        //            #endregion
                        //        }
                        //    }
                        //    #endregion
                        //}
                        //if (sale_return.AmountPayment > 0)
                        //{
                        //    #region phiếu chi
                        //    //Khách thanh toán ngay
                        //    if (sale_return.AmountPayment > 0)
                        //    {
                        //        var customer = customerRepository.GetvwCustomerById(sale_return.CustomerId.Value);
                        //        #region xóa phiếu chi cũ (nếu có)
                        //        var _payment = paymentRepository.GetAllPayment()
                        //            .Where(item => item.BranchId == Helpers.Common.CurrentUser.BranchId.Value && item.MaChungTuGoc == sale_return.Code).ToList();
                        //        foreach (var item in _payment)
                        //        {
                        //            paymentRepository.DeletePaymentRs(item.Id);
                        //        }
                        //        #endregion

                        //        //Khách thanh toán ngay
                        //        if (sale_return.AmountPayment > 0)
                        //        {
                        //            //Lập phiếu thu
                        //            var payment = new Payment();
                        //            payment.IsDeleted = false;
                        //            payment.CreatedUserId = WebSecurity.CurrentUserId;
                        //            payment.ModifiedUserId = WebSecurity.CurrentUserId;
                        //            payment.AssignedUserId = WebSecurity.CurrentUserId;
                        //            payment.CreatedDate = DateTime.Now;
                        //            payment.ModifiedDate = DateTime.Now;
                        //            payment.VoucherDate = DateTime.Now;
                        //            payment.TargetId = customer.Id;
                        //            payment.TargetName = "Customer";
                        //            payment.Receiver = customer.CompanyName;
                        //            payment.PaymentMethod = sale_return.PaymentMethod;
                        //            payment.BranchId = Helpers.Common.CurrentUser.BranchId.Value;
                        //            payment.Address = customer.Address;
                        //            payment.MaChungTuGoc = sale_return.Code;
                        //            payment.LoaiChungTuGoc = "SalesReturns";
                        //            payment.Note = payment.Name;
                        //            payment.Amount = sale_return.AmountPayment;

                        //            paymentRepository.InsertPayment(payment);

                        //            payment.Code = Erp.BackOffice.Helpers.Common.GetOrderNo("Payment", model.Code);
                        //            paymentRepository.UpdatePayment(payment);
                        //            Erp.BackOffice.Helpers.Common.SetOrderNo("Payment");

                        //            //Lập chi tiết phiếu thu

                        //            var paymentDetail = new PaymentDetail();
                        //            paymentDetail.IsDeleted = false;
                        //            paymentDetail.CreatedUserId = WebSecurity.CurrentUserId;
                        //            paymentDetail.ModifiedUserId = WebSecurity.CurrentUserId;
                        //            paymentDetail.AssignedUserId = WebSecurity.CurrentUserId;
                        //            paymentDetail.CreatedDate = DateTime.Now;
                        //            paymentDetail.ModifiedDate = DateTime.Now;
                        //            paymentDetail.Name = "Trả tiền cho đơn hàng bán trả lại";
                        //            paymentDetail.Amount = payment.Amount;
                        //            paymentDetail.PaymentId = payment.Id;
                        //            paymentDetail.MaChungTuGoc = sale_return.Code;
                        //            paymentDetail.LoaiChungTuGoc = "SalesReturns";

                        //            paymentDetailRepository.InsertPaymentDetail(paymentDetail);
                        //            //Thêm vào quản lý chứng từ
                        //            TransactionController.Create(new TransactionViewModel
                        //            {
                        //                TransactionModule = "Payment",
                        //                TransactionCode = payment.Code,
                        //                TransactionName = "Trả tiền cho đơn hàng bán trả lại"
                        //            });

                        //            //Thêm chứng từ liên quan
                        //            TransactionController.CreateRelationship(new TransactionRelationshipViewModel
                        //            {
                        //                TransactionA = payment.Code,
                        //                TransactionB = sale_return.Code
                        //            });

                        //            //Ghi Có TK 131 - Phải thu của khách hàng.
                        //            Erp.BackOffice.Account.Controllers.TransactionLiabilitiesController.Create(
                        //                payment.Code,
                        //                "Payment",
                        //                "Trả tiền cho đơn hàng bán trả lại",
                        //                customer.Code,
                        //                "Customer",
                        //                0,
                        //                Convert.ToDecimal(sale_return.AmountPayment),
                        //                sale_return.Code,
                        //                "SalesReturns",
                        //                sale_return.PaymentMethod,
                        //                null,
                        //                null);
                        //        }
                        //    }
                        //    #endregion
                        //}
                        //insert mới
                        if (product_inbound.Count() <= 0)
                        {
                            #region  thêm mới phiếu nhập

                            //Nếu trong đơn hàng có sản phẩm thì xuất kho
                            if (sale_return.WarehouseDestinationId != null)
                            {
                                ins_inbound.IsDeleted = false;
                                ins_inbound.CreatedUserId = WebSecurity.CurrentUserId;
                                ins_inbound.ModifiedUserId = WebSecurity.CurrentUserId;
                                ins_inbound.CreatedDate = DateTime.Now;
                                ins_inbound.ModifiedDate = DateTime.Now;
                                ins_inbound.BranchId = sale_return.BranchId;
                                ins_inbound.SalesReturnsId = sale_return.Id;
                                ins_inbound.WarehouseDestinationId = sale_return.WarehouseDestinationId;
                                ins_inbound.Note = "Nhập kho cho đơn hàng trả lại " + sale_return.Code;
                                ins_inbound.IsArchive = false;
                                ins_inbound.Type = "SalesReturns";
                                ins_inbound.TotalAmount = invoice_detail_list.Sum(x => x.Price * x.Quantity);
                                productInboundRepository.InsertProductInbound(ins_inbound);

                                //Cập nhật lại mã xuất kho
                                ins_inbound.Code = Erp.BackOffice.Helpers.Common.GetOrderNo("ProductInbound");
                                productInboundRepository.UpdateProductInbound(ins_inbound);
                                Erp.BackOffice.Helpers.Common.SetOrderNo("ProductInbound");
                                foreach (var item in invoice_detail_list)
                                {
                                    ProductInboundDetail ins_detail_inbound = new ProductInboundDetail();
                                    ins_detail_inbound.IsDeleted = false;
                                    ins_detail_inbound.CreatedUserId = WebSecurity.CurrentUserId;
                                    ins_detail_inbound.ModifiedUserId = WebSecurity.CurrentUserId;
                                    ins_detail_inbound.CreatedDate = DateTime.Now;
                                    ins_detail_inbound.ModifiedDate = DateTime.Now;
                                    ins_detail_inbound.ProductInboundId = ins_inbound.Id;
                                    ins_detail_inbound.ExpiryDate = item.ExpiryDate;
                                    ins_detail_inbound.LoCode = item.LoCode;
                                    ins_detail_inbound.ProductId = item.ProductId;
                                    ins_detail_inbound.Price = item.Price;
                                    ins_detail_inbound.Quantity = item.Quantity;
                                    ins_detail_inbound.Unit = item.Unit;
                                    productInboundRepository.InsertProductInboundDetail(ins_detail_inbound);
                                }

                                //Ghi sổ chứng từ phiếu xuất
                                ProductInboundController.Archive(ins_inbound);

                                //Ghi sổ chứng từ phiếu 
                                TransactionController.Create(new TransactionViewModel
                                {
                                    TransactionModule = "ProductInbound",
                                    TransactionCode = ins_inbound.Code,
                                    TransactionName = "Nhập kho hàng bán trả lại"
                                });

                                //Thêm chứng từ liên quan
                                TransactionController.CreateRelationship(new TransactionRelationshipViewModel
                                {
                                    TransactionA = ins_inbound.Code,
                                    TransactionB = sale_return.Code
                                });
                            }
                            #endregion
                        }
                        else
                        {
                            ins_inbound = product_inbound.FirstOrDefault();
                            //Ghi sổ chứng từ phiếu xuất
                            ProductInboundController.Archive(ins_inbound);
                        }

                        //insert mới
                        if (product_outbound.Count() <= 0)
                        {
                            #region  phiếu xuất ok
                            //insert mới
                            if (product_outbound.Count() <= 0)
                            {
                                #region  thêm mới phiếu xuất
                                var warehouseDefault = warehouseRepository.GetAllWarehouse().Where(x => x.BranchId == sale_return.BranchId && x.IsSale == true).FirstOrDefault();
                                //Nếu trong đơn hàng có sản phẩm thì xuất kho
                                if (warehouseDefault != null)
                                {
                                    ins_outbound.InvoiceId = sale_return.ProductInvoiceNewId;

                                    ins_outbound.WarehouseSourceId = warehouseDefault.Id;
                                    ins_outbound.Note = "Xuất kho cho đơn hàng " + invoice_new.Code;
                                    ins_outbound.IsDeleted = false;
                                    ins_outbound.CreatedUserId = WebSecurity.CurrentUserId;
                                    ins_outbound.ModifiedUserId = WebSecurity.CurrentUserId;
                                    ins_outbound.CreatedDate = DateTime.Now;
                                    ins_outbound.ModifiedDate = DateTime.Now;
                                    ins_outbound.BranchId = sale_return.BranchId;
                                    ins_outbound.TotalAmount = invoice_detail_new.Sum(x => x.Price * x.Quantity);
                                    ins_outbound.Note = "Xuất kho cho đơn hàng" + invoice_new.Code;
                                    ins_outbound.IsArchive = false;
                                    ins_outbound.Type = "invoice";
                                    productOutboundRepository.InsertProductOutbound(ins_outbound);

                                    //Cập nhật lại mã xuất kho
                                    ins_outbound.Code = Erp.BackOffice.Helpers.Common.GetOrderNo("ProductOutbound");
                                    productOutboundRepository.UpdateProductOutbound(ins_outbound);
                                    Erp.BackOffice.Helpers.Common.SetOrderNo("ProductOutbound");

                                    foreach (var item in invoice_detail_new)
                                    {
                                        ProductOutboundDetail ins_detail_outbound = new ProductOutboundDetail();
                                        ins_detail_outbound.IsDeleted = false;
                                        ins_detail_outbound.CreatedUserId = WebSecurity.CurrentUserId;
                                        ins_detail_outbound.ModifiedUserId = WebSecurity.CurrentUserId;
                                        ins_detail_outbound.CreatedDate = DateTime.Now;
                                        ins_detail_outbound.ModifiedDate = DateTime.Now;
                                        ins_detail_outbound.ProductOutboundId = ins_outbound.Id;
                                        ins_detail_outbound.ExpiryDate = item.ExpiryDate;
                                        ins_detail_outbound.LoCode = item.LoCode;
                                        ins_detail_outbound.ProductId = item.ProductId;
                                        ins_detail_outbound.Price = item.Price;
                                        ins_detail_outbound.Quantity = item.Quantity;
                                        ins_detail_outbound.Unit = item.Unit;
                                        productOutboundRepository.InsertProductOutboundDetail(ins_detail_outbound);
                                    }
                                    //Ghi sổ chứng từ phiếu xuất
                                    ProductOutboundController.Archive(ins_outbound, TempData);
                                    //Ghi sổ chứng từ phiếu 
                                    TransactionController.Create(new TransactionViewModel
                                    {
                                        TransactionModule = "ProductOutbound",
                                        TransactionCode = ins_outbound.Code,
                                        TransactionName = "Xuất kho bán hàng"
                                    });

                                    //Thêm chứng từ liên quan
                                    TransactionController.CreateRelationship(new TransactionRelationshipViewModel
                                    {
                                        TransactionA = ins_outbound.Code,
                                        TransactionB = invoice_new.Code
                                    });
                                }
                                #endregion
                            }
                            else
                            {
                                #region   cập nhật phiếu xuất kho
                                //xóa chi tiết phiếu xuất, insert chi tiết mới
                                //cập nhật lại tổng tiền, trạng thái phiếu xuất
                                for (int i = 0; i < product_outbound.Count(); i++)
                                {
                                    var outbound_detail = productOutboundRepository.GetAllProductOutboundDetailByOutboundId(product_outbound[i].Id).ToList();
                                    //xóa
                                    for (int ii = 0; ii < outbound_detail.Count(); ii++)
                                    {
                                        productOutboundRepository.DeleteProductOutboundDetail(outbound_detail[ii].Id);
                                    }
                                    //insert mới
                                    for (int iii = 0; iii < invoice_detail_list.Count(); iii++)
                                    {
                                        ProductOutboundDetail productOutboundDetail = new ProductOutboundDetail();
                                        productOutboundDetail.Price = invoice_detail_list[iii].Price;
                                        productOutboundDetail.ProductId = invoice_detail_list[iii].ProductId;
                                        productOutboundDetail.Quantity = invoice_detail_list[iii].Quantity;
                                        productOutboundDetail.Unit = invoice_detail_list[iii].Unit;
                                        productOutboundDetail.LoCode = invoice_detail_list[iii].LoCode;
                                        productOutboundDetail.ExpiryDate = invoice_detail_list[iii].ExpiryDate;
                                        productOutboundDetail.IsDeleted = false;
                                        productOutboundDetail.CreatedUserId = WebSecurity.CurrentUserId;
                                        productOutboundDetail.ModifiedUserId = WebSecurity.CurrentUserId;
                                        productOutboundDetail.CreatedDate = DateTime.Now;
                                        productOutboundDetail.ModifiedDate = DateTime.Now;
                                        productOutboundDetail.ProductOutboundId = product_outbound[i].Id;
                                        productOutboundRepository.InsertProductOutboundDetail(productOutboundDetail);

                                    }
                                    product_outbound[i].TotalAmount = invoice_detail_list.Sum(x => (x.Price * x.Quantity));
                                    ProductOutboundController.Archive(product_outbound[i], TempData);

                                }
                                #endregion
                            }
                            #endregion
                        }
                    }
                    sale_return.PaymentMethod = PaymentMethod;
                    sale_return.PaidAmount = sale_return.AmountReceipt;
                    sale_return.RemainingAmount = 0;
                    sale_return.Status = Wording.OrderStatus_complete;
                    sale_return.IsArchive = true;
                    sale_return.ModifiedDate = DateTime.Now;
                    sale_return.ProductInboundId = ins_inbound.Id;
                    sale_return.ModifiedUserId = WebSecurity.CurrentUserId;
                    SalesReturnsRepository.UpdateSalesReturns(sale_return);
                    //ghi log 
                    Erp.BackOffice.Controllers.HomeController.WriteLog(sale_return.Id, sale_return.Code, "đã ghi sổ hàng bán trả", "SalesReturns/Detail/" + sale_return.Id, Helpers.Common.CurrentUser.BranchId.Value);

                    if ((invoice_new != null) && (invoice_detail_new != null && invoice_detail_new.Count > 0))
                    {
                        invoice_new.PaymentMethod = PaymentMethod;
                        invoice_new.PaidAmount = sale_return.AmountReceipt.Value;
                        invoice_new.RemainingAmount = 0;
                        invoice_new.NextPaymentDate = null;
                        invoice_new.IsArchive = true;
                        invoice_new.Status = Wording.OrderStatus_complete;
                        invoice_new.ModifiedDate = DateTime.Now;
                        invoice_new.ModifiedUserId = WebSecurity.CurrentUserId;
                        productInvoiceRepository.UpdateProductInvoice(invoice_new);
                    }
                    scope.Complete();
                }

                catch (DbUpdateException ex)
                {
                    return Content("Fail");
                }
            }
            return RedirectToAction("Detail", new { Id = model.Id });
        }
        #endregion
        #region Delete
        [HttpPost]
        public ActionResult Delete(int Id, string Note)
        {
            var sale_return = SalesReturnsRepository.GetSalesReturnsById(Id);
            if (sale_return != null)
            {

                sale_return.ModifiedUserId = WebSecurity.CurrentUserId;
                sale_return.ModifiedDate = DateTime.Now;
                sale_return.IsDeleted = true;
                sale_return.IsArchive = false;
                sale_return.Note = Note;
                sale_return.Status = Wording.OrderStatus_deleted;
                SalesReturnsRepository.UpdateSalesReturns(sale_return);
                TempData["SuccessMessage"] = "Hủy đơn hàng thành công";
                return RedirectToAction("Index");
                //return RedirectToAction("Detail", new { Id = sale_return.Id });
            }

            return RedirectToAction("Index");
        }
        #endregion

        #region UnArchive
        [HttpPost]
        public ActionResult UnArchive(int Id, bool IsPopup)
        {
            if (Request["Submit"] == "Save")
            {

                var sale_return = SalesReturnsRepository.GetSalesReturnsById(Id);
                var invoice_detail_list = SalesReturnsRepository.GetvwAllReturnsDetailsByReturnId(sale_return.Id).ToList();
                var invoice_old = productInvoiceRepository.GetvwProductInvoiceById(sale_return.ProductInvoiceOldId.Value);
                var invoice_new = productInvoiceRepository.GetProductInvoiceById(Helpers.Common.NVL_NUM_NT_NEW(sale_return.ProductInvoiceNewId));
                var invoice_detail_new = productInvoiceRepository.GetAllvwInvoiceDetailsByInvoiceId(Helpers.Common.NVL_NUM_NT_NEW(sale_return.ProductInvoiceNewId)).Where(x => x.ProductType == "product").ToList();

                //cập nhật ghi chú đơn hàng bán
                var invoice_old2 = productInvoiceRepository.GetProductInvoiceById(invoice_old.Id);
                invoice_old2.Note = "";
                productInvoiceRepository.UpdateProductInvoice(invoice_old2);
                // Kiểm tra phân quyền Chi nhánh
                if (!(Filters.SecurityFilter.IsAdmin() || sale_return.BranchId == Helpers.Common.CurrentUser.BranchId))
                {
                    return Content("Mẫu tin không tồn tại! Không có quyền truy cập!");
                }

                //Kiểm tra cho phép sửa chứng từ này hay không
                if (Helpers.Common.KiemTraNgaySuaChungTu(sale_return.CreatedDate.Value) == false)
                {
                    TempData[Globals.FailedMessageKey] = "Quá hạn sửa chứng từ!";
                }
                else
                {
                    using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.Snapshot }))
                    {
                        try
                        {
                            //#region isDelete payment
                            //var payment = paymentRepository.GetAllPayment()
                            //    .Where(item => item.BranchId == Helpers.Common.CurrentUser.BranchId.Value && item.LoaiChungTuGoc == "SalesReturns" && item.MaChungTuGoc == sale_return.Code).ToList();
                            //var payment_detail = paymentDetailRepository.GetAllPaymentDetail().Where(x =>x.MaChungTuGoc == sale_return.Code).ToList();
                            //if (payment_detail.Count() > 0)
                            //{
                            //    // isDelete chi tiết phiếu thu
                            //    for (int i = 0; i < payment_detail.Count(); i++)
                            //    {
                            //        paymentDetailRepository.DeletePaymentDetail(payment_detail[i].Id);
                            //    }
                            //}
                            //if (payment.Count() > 0)
                            //{
                            //    // isDelete phiếu thu
                            //    for (int i = 0; i < payment.Count(); i++)
                            //    {
                            //        paymentRepository.DeletePayment(payment[i].Id);
                            //    }
                            //}
                            //var listTransaction = transactionLiabilitiesRepository.GetAllTransaction()
                            //  .Where(item => item.BranchId == Helpers.Common.CurrentUser.BranchId.Value && item.LoaiChungTuGoc == "SalesReturns" && (item.MaChungTuGoc == sale_return.Code))
                            //  .Select(item => item.Id)
                            //  .ToList();


                            //foreach (var item in listTransaction)
                            //{
                            //    transactionLiabilitiesRepository.DeleteTransactionRs(item);
                            //}


                            //var listTransactionRelationship = transactionRepository2.GetAllTransactionRelationship()
                            //.Where(item => item.BranchId == Helpers.Common.CurrentUser.BranchId.Value && item.TransactionB == sale_return.Code).Select(item => item.Id)
                            //  .ToList(); ;
                            //foreach (var item in listTransactionRelationship)
                            //{
                            //    transactionRepository2.DeleteTransactionRelationshipRs(item);
                            //}

                            //#endregion

                            //#region isDelete receipt
                            //if (invoice_new != null)
                            //{
                            //    var receipt = ReceiptRepository.GetAllReceipts()
                            //        .Where(item => item.BranchId == Helpers.Common.CurrentUser.BranchId.Value && item.MaChungTuGoc == invoice_new.Code).ToList();
                            //    //var receipt_detail = receiptDetailRepository.GetAllReceiptDetail().Where(x => x.MaChungTuGoc == invoice_new.Code).ToList();
                            //    //if (receipt_detail.Count() > 0)
                            //    //{
                            //    //    // isDelete chi tiết phiếu thu
                            //    //    for (int i = 0; i < receipt_detail.Count(); i++)
                            //    //    {
                            //    //        receiptDetailRepository.DeleteReceiptDetailRs(receipt_detail[i].Id);
                            //    //    }
                            //    //}
                            //    if (receipt.Count() > 0)
                            //    {
                            //        // isDelete phiếu thu
                            //        for (int i = 0; i < receipt.Count(); i++)
                            //        {
                            //            ReceiptRepository.DeleteReceiptRs(receipt[i].Id);
                            //        }
                            //    }

                            //    #endregion

                            //    #region isDelete listTransaction
                            //    //isDelete lịch sử giao dịch
                            //    listTransaction = transactionLiabilitiesRepository.GetAllTransaction()
                            //    .Where(item => item.BranchId == Helpers.Common.CurrentUser.BranchId.Value && (item.MaChungTuGoc == invoice_new.Code || item.MaChungTuGoc == sale_return.Code))
                            //    .Select(item => item.Id)
                            //    .ToList();


                            //    foreach (var item in listTransaction)
                            //    {
                            //        transactionLiabilitiesRepository.DeleteTransactionRs(item);
                            //    }
                            //}
                            //#endregion

                            #region bỏ ghi sổ ProductOutbound
                            if (invoice_new != null)
                            {
                                var productOutbound = productOutboundRepository.GetAllProductOutbound().Where(x => x.InvoiceId == invoice_new.Id).ToList();
                                foreach (var item in productOutbound)
                                {

                                    string check = "";
                                    var detail = productOutboundRepository.GetAllvwProductOutboundDetailByOutboundId(item.Id).ToList();
                                    foreach (var item2 in detail)
                                    {
                                        var error = InventoryController.Check(item2.ProductName, item2.ProductId.Value, item2.LoCode, item2.ExpiryDate, item.WarehouseSourceId.Value, item2.Quantity.Value, 0);
                                        check += error;
                                    }
                                    if (string.IsNullOrEmpty(check))
                                    {
                                        //Khi đã hợp lệ thì mới update
                                        foreach (var i in detail)
                                        {
                                            InventoryController.Update(i.ProductName, i.ProductId.Value, i.LoCode, i.ExpiryDate, item.WarehouseSourceId.Value, i.Quantity.Value, 0);
                                        }

                                        item.IsArchive = false;
                                        productOutboundRepository.UpdateProductOutbound(item);
                                        TempData[Globals.SuccessMessageKey] = "Đã bỏ ghi sổ";
                                    }
                                    else
                                    {
                                        TempData[Globals.FailedMessageKey] = App_GlobalResources.Wording.ArchiveFail + check;
                                    }
                                    //productOutboundRepository.DeleteProductOutboundRs(item.Id);

                                }
                                #endregion
                            }

                            #region bỏ ghi sổ productInbound
                            var productInbound = productInboundRepository.GetProductInboundById(Helpers.Common.NVL_NUM_NT_NEW(sale_return.ProductInboundId));

                            //Cập nhật lại tồn kho cho những sp trong phiếu nhập này
                            var detailList = productInboundRepository.GetAllvwProductInboundDetailByInboundId(Helpers.Common.NVL_NUM_NT_NEW(sale_return.ProductInboundId))
                                .Select(item => new
                                {
                                    ProductName = item.ProductCode + " - " + item.ProductName,
                                    ProductId = item.ProductId.Value,
                                    Quantity = item.Quantity.Value,
                                    LoCode = item.LoCode,
                                    ExpiryDate = item.ExpiryDate
                                }).ToList();

                            //Kiểm tra dữ liệu sau khi bỏ ghi sổ có hợp lệ không
                            string check1 = "";
                            foreach (var item in detailList.Where(x => x.Quantity > 0))
                            {
                                var error = InventoryController.Check(item.ProductName, item.ProductId, item.LoCode, item.ExpiryDate, productInbound.WarehouseDestinationId.Value, 0, item.Quantity);
                                check1 += error;
                            }

                            if (string.IsNullOrEmpty(check1))
                            {
                                //Khi đã hợp lệ thì mới update
                                foreach (var item in detailList.Where(x => x.Quantity > 0))
                                {
                                    InventoryController.Update(item.ProductName, item.ProductId, item.LoCode, item.ExpiryDate, productInbound.WarehouseDestinationId.Value, 0, item.Quantity);
                                }

                                if (productInbound != null)
                                {
                                    productInbound.IsArchive = false;
                                    productInboundRepository.UpdateProductInbound(productInbound);
                                }
                                TempData[Globals.SuccessMessageKey] = "Đã bỏ ghi sổ";
                            }
                            else
                            {
                                TempData[Globals.FailedMessageKey] = App_GlobalResources.Wording.ArchiveFail + check1;
                            }
                            #endregion

                            sale_return.PaidAmount = 0;
                            sale_return.RemainingAmount = sale_return.AmountReceipt;

                            sale_return.ModifiedUserId = WebSecurity.CurrentUserId;
                            sale_return.ModifiedDate = DateTime.Now;
                            sale_return.IsArchive = false;
                            sale_return.Status = Wording.OrderStatus_inprogress;
                            SalesReturnsRepository.UpdateSalesReturns(sale_return);

                            if (invoice_new != null)
                            {
                                invoice_new.PaymentMethod = null;
                                invoice_new.PaidAmount = 0;
                                invoice_new.RemainingAmount = sale_return.AmountReceipt.Value;
                                invoice_new.NextPaymentDate = null;
                                invoice_new.IsArchive = false;
                                invoice_new.Status = Wording.OrderStatus_inprogress;
                                invoice_new.ModifiedDate = DateTime.Now;
                                invoice_new.ModifiedUserId = WebSecurity.CurrentUserId;
                                productInvoiceRepository.UpdateProductInvoice(invoice_new);
                            }

                            TempData[Globals.SuccessMessageKey] = "Đã bỏ ghi sổ";

                            scope.Complete();
                        }
                        catch (DbUpdateException)
                        {
                            return Content("Fail");
                        }
                    }
                }
            }

            return RedirectToAction("Detail", new { Id = Id, IsPopup = IsPopup });
        }
        #endregion

        #region ExportExcel
        public List<SalesReturnsViewModel> IndexPrint(string startDate, string endDate, string txtCode, string txtCusName, int? SalerId, string Status)
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

            var q = SalesReturnsRepository.GetAllvwSalesReturns()
                //.Where(x => x.BranchId == Helpers.Common.CurrentUser.BranchId)
                .AsEnumerable()
                .Select(item => new SalesReturnsViewModel
                {
                    Id = item.Id,
                    CreatedUserId = item.CreatedUserId,
                    CreatedDate = item.CreatedDate,
                    ModifiedUserId = item.ModifiedUserId,
                    ModifiedDate = item.ModifiedDate,
                    Code = item.Code,
                    CustomerCode = item.CustomerCode,
                    CustomerName = item.CustomerName,
                    TotalAmount = item.TotalAmount,
                    TaxFee = item.TaxFee,
                    BranchId = item.BranchId,
                    CustomerId = item.CustomerId,
                    PaymentMethod = item.PaymentMethod,
                    Status = item.Status,
                    BranchName = item.BranchName,
                    IsArchive = item.IsArchive
                }).OrderByDescending(m => m.CreatedDate).ToList();

            foreach (var item2 in q)
            {
                if (item2.CustomerId != null)
                {
                    var Man = CustomerRepository.GetvwCustomerById(item2.CustomerId.Value);
                    item2.ManagerStaffId = Man.ManagerStaffId;
                    item2.ManagerStaffName = Man.ManagerStaffName;
                    item2.ManagerUserName = Man.ManagerUserName;
                }
                else
                {
                    item2.ManagerStaffId = 0;
                    item2.ManagerStaffName = "null";
                    item2.ManagerUserName = "null";
                }

            }

            if (intBrandID > 0)
            {
                q = q.Where(x => x.BranchId == intBrandID).ToList();
            }
            if (string.IsNullOrEmpty(txtCode) == false || string.IsNullOrEmpty(txtCusName) == false)
            {
                txtCode = txtCode == "" ? "~" : txtCode.ToLower();

                txtCusName = txtCusName == "" ? "~" : Erp.BackOffice.Helpers.Common.ChuyenThanhKhongDau(txtCusName);
                q = q.Where(x => x.Code.ToLowerOrEmpty().Contains(txtCode)
                    || Erp.BackOffice.Helpers.Common.ChuyenThanhKhongDau(x.CustomerName).Contains(txtCusName)).ToList();
            }


            // lọc theo ngày
            DateTime d_startDate, d_endDate;
            if (DateTime.TryParseExact(startDate, "dd/MM/yyyy", new CultureInfo("vi-VN"), DateTimeStyles.None, out d_startDate))
            {
                if (DateTime.TryParseExact(endDate, "dd/MM/yyyy", new CultureInfo("vi-VN"), DateTimeStyles.None, out d_endDate))
                {
                    d_endDate = d_endDate.AddHours(23).AddMinutes(59);
                    q = q.Where(x => x.CreatedDate >= d_startDate && x.CreatedDate <= d_endDate).ToList();
                }
            }
            // tìm kiếm theo status
            if (!string.IsNullOrEmpty(Status))
            {
                if (Status == "complete")
                {
                    q = q.Where(x => x.IsArchive == true).ToList();
                }
                else if (Status == "pending")
                {
                    q = q.Where(x => x.IsArchive == false).ToList();
                }
                else if (Status == "delete")
                {
                    q = q.Where(x => x.IsDeleted == true).ToList();
                }
            }
            if ((SalerId != null) && (SalerId > 0))
            {
                q = q.Where(x => x.ManagerStaffId == SalerId).ToList();
            }

            return q;
        }

        public ActionResult PrintSalesReturns (string startDate, string endDate, string txtCode, string txtCusName, int? SalerId, string Status, bool ExportExcel = false)
        {
            var data = IndexPrint(startDate, endDate, txtCode, txtCusName, SalerId, Status);

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
            model.Content = model.Content.Replace("{DataTable}", buildHtmlDanhsachhangbantralai(data));
            model.Content = model.Content.Replace("{System.Logo}", ImgLogo);
            model.Content = model.Content.Replace("{System.CompanyName}", company);
            model.Content = model.Content.Replace("{System.AddressCompany}", address);
            model.Content = model.Content.Replace("{System.PhoneCompany}", phone);
            model.Content = model.Content.Replace("{System.Fax}", fax);
            model.Content = model.Content.Replace("{System.BankCodeCompany}", bankcode);
            model.Content = model.Content.Replace("{System.BankNameCompany}", bankname);
            model.Content = model.Content.Replace("{PrintDate}", DateTime.Now.ToString("dd/MM/yyyy HH:mm"));
            model.Content = model.Content.Replace("{Title}", "Danh sách hàng bán trả lại");
            if (ExportExcel)
            {
                Response.AppendHeader("content-disposition", "attachment;filename=" + "DS_Hangbantralai" + DateTime.Now.ToString("yyyyMMdd") + ".xls");
                Response.Charset = "";
                Response.Cache.SetCacheability(HttpCacheability.NoCache);
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.Write(model.Content);
                Response.End();
            }
            return View(model);
        }

        string buildHtmlDanhsachhangbantralai(List<SalesReturnsViewModel> detailList)
        {
            string detailLists = "<table border=\"1\" class=\"invoice-detail\">\r\n";
            detailLists += "<thead>\r\n";
            detailLists += "	<tr>";
            detailLists += "		<th>STT</th>";
            detailLists += "		<th>Trạng thái</th>";
            detailLists += "		<th>Ngày tạo</th>";
            detailLists += "		<th>Mã đơn hàng</th>";
            detailLists += "		<th>Tổng tiền</th>";
            detailLists += "		<th>Khách hàng</th>";
            detailLists += "		<th>Mã KH</th>";
            detailLists += "		<th>Nhân viên quản lý</th>";
            detailLists += "		<th>Ngày cập nhật</th>";
            detailLists += "		<th>Trạng thái ghi sổ</th>";
            detailLists += "	</tr>";
            detailLists += "</thead>\r\n";
            detailLists += "<tbody>\r\n";
            var index = 1;
            decimal total = 0;

            foreach (var item in detailList)
            {
                total += item.TotalAmount ?? 0;
                string status =
                    item.Status == "pending" ? Wording.OrderStatusDisplay_pending :
                    item.Status == "inprogress" ? Wording.OrderStatusDisplay_inprogress :
                    item.Status == "shipping" ? Wording.OrderStatusDisplay_shipping :
                    item.Status == "complete" ? Wording.OrderStatusDisplay_complete :
                    item.Status == "delete" ? Wording.OrderStatusDisplay_delete : "";

                detailLists += "<tr>\r\n"
                + "<td class=\"text-center orderNo\">" + (index++) + "</td>\r\n"
                + "<td class=\"text-left \">" + status + "</td>\r\n"
                + "<td>" + (item.CreatedDate.HasValue ? item.CreatedDate.Value.ToString("dd/MM/yyyy HH:mm") : "") + "</td>"
                + "<td class=\"text-left \">" + item.Code + "</td>\r\n"
                 + "<td class=\"text-right \">" + CommonSatic.ToCurrencyStr(item.TotalAmount, null).Replace(".", ",") + "</td>\r\n"
                + "<td class=\"text-left \">" + item.CustomerName + "</td>\r\n"
                + "<td class=\"text-left \">" + item.CustomerCode + "</td>\r\n"
                + "<td class=\"text-left \">" + item.ManagerStaffName + "</td>\r\n"             
                + "<td>" + (item.ModifiedDate.HasValue ? item.ModifiedDate.Value.ToString("dd/MM/yyyy HH:mm") : "") + "</td>"
                + "<td class=\"text-left \">" + (item.IsArchive == true ? "Đã ghi sổ" : "Chưa ghi sổ") + "</td>\r\n"
                + "</tr>\r\n";
            }

            detailLists += "<tr>\r\n"
               + "<td style=\"font-weight:bold \">Tổng cộng</td>"
               + "<td class=\"text-left \">" + "" + "</td>\r\n"
               + "<td class=\"text-left \">" + "" + "</td>\r\n"
               + "<td class=\"text-left \">" + "" + "</td>\r\n"
               + "<td class=\"text-right \" style=\"font-weight:bold \">" + CommonSatic.ToCurrencyStr(total, null).Replace(".", ",") + "</td>\r\n"
               + "<td class=\"text-left \">" + "" + "</td>\r\n"
               + "<td class=\"text-left \">" + "" + "</td>\r\n"
               + "<td class=\"text-left \">" + "" + "</td>\r\n"
               + "<td class=\"text-left \">" + "" + "</td>\r\n"
               + "<td class=\"text-left \">" + "" + "</td>\r\n"
               + "</tr>\r\n";


            detailLists += "</tbody>\r\n";
            detailLists += "<tfoot style=\"font-weight:bold\">\r\n";
            detailLists += "</tfoot>\r\n</table>\r\n";
            return detailLists;
        }
        #endregion
    }
}
