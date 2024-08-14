using System.Globalization;
using Erp.BackOffice.Account.Models;
using Erp.BackOffice.Filters;
using Erp.Domain.Entities;
using Erp.Domain.Interfaces;
using Erp.Domain.Account.Entities;
using Erp.Domain.Account.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Erp.Utilities;
using WebMatrix.WebData;
using Erp.BackOffice.Helpers;
using Erp.Domain.Account.Repositories;
using System.ComponentModel.DataAnnotations;
using Erp.BackOffice.App_GlobalResources;
using Erp.Domain.Sale.Interfaces;
using Erp.BackOffice.Sale.Models;
using Erp.Domain.Staff.Interfaces;
using System.Transactions;
using System.Web;
using System.Web.UI.WebControls;
using System.IO;
using System.Web.UI;
using System.Data;

namespace Erp.BackOffice.Account.Controllers
{
    [Authorize]
    [InitializeSimpleMembership]
    [Erp.BackOffice.Helpers.NoCacheHelper]
    public class TransactionLiabilitiesController : Controller
    {
        private readonly ITransactionLiabilitiesRepository transactionLiabilitiesRepository;
        private readonly IUserRepository userRepository;
        private readonly IProcessPaymentRepository processPaymentRepository;
        private readonly IReceiptRepository receiptRepository;
        private readonly IPaymentRepository paymentRepository;
        private readonly IProductInvoiceRepository productInvoiceRepository;
        private readonly ICustomerRepository customerRepository;
        private readonly IReceiptDetailRepository receiptDetailRepository;
        private readonly IPurchaseOrderRepository purchaseOrderRepository;
        private readonly ISupplierRepository supplierRepository;
        private readonly IProductOutboundRepository productOutboundRepository;
        private readonly IBranchRepository branchRepository;
        private readonly IWarehouseRepository warehouseRepository;
        private readonly ITemplatePrintRepository templatePrintRepository;
        public TransactionLiabilitiesController(
            ITransactionLiabilitiesRepository _Transaction
            , IUserRepository _user
            , IProcessPaymentRepository _ProcessPayment
            , IReceiptRepository _Receipt
            , IPaymentRepository _Payment
            , IProductInvoiceRepository _ProductInvoice
            , ICustomerRepository _Customer
            , IReceiptDetailRepository _ReceiptDetail
            , IPurchaseOrderRepository purchase
            , ISupplierRepository supplier
            , IProductOutboundRepository productOutbound
            , IBranchRepository branch
            , IWarehouseRepository warehouse
            , ITemplatePrintRepository _templatePrint
            )
        {
            transactionLiabilitiesRepository = _Transaction;
            userRepository = _user;
            processPaymentRepository = _ProcessPayment;
            receiptRepository = _Receipt;
            paymentRepository = _Payment;
            productInvoiceRepository = _ProductInvoice;
            customerRepository = _Customer;
            receiptDetailRepository = _ReceiptDetail;
            purchaseOrderRepository = purchase;
            supplierRepository = supplier;
            productOutboundRepository = productOutbound;
            branchRepository = branch;
            warehouseRepository = warehouse;
            templatePrintRepository = _templatePrint;
        }

        #region Index

        public ViewResult Index()
        {
            List<TransactionLiabilitiesViewModel> q = transactionLiabilitiesRepository.GetAllTransaction()
                .Select(item => new TransactionLiabilitiesViewModel
                {
                    Id = item.Id,
                    CreatedUserId = item.CreatedUserId,
                    CreatedDate = item.CreatedDate,
                    ModifiedUserId = item.ModifiedUserId,
                    ModifiedDate = item.ModifiedDate,
                    TransactionCode = item.TransactionCode,
                    TransactionModule = item.TransactionModule,
                    TransactionName = item.TransactionName,
                    Debit = item.Debit,
                    Credit = item.Credit
                }).OrderByDescending(m => m.CreatedDate).ToList();

            //foreach (var item in q)
            //{
            //    item.SubListTransaction = transactionRepository.GetAllTransaction()
            //    .Select(i => new TransactionLiabilitiesViewModel
            //    {
            //        Id = i.Id,
            //        CreatedDate = i.CreatedDate,
            //        TransactionCode = i.TransactionCode,
            //        TransactionModule = item.TransactionModule,
            //        TransactionName = item.TransactionName,
            //        Debit = item.Debit,
            //        Credit = item.Credit
            //    }).OrderByDescending(m => m.CreatedDate).ToList();
            //}

            ViewBag.SuccessMessage = TempData["SuccessMessage"];
            ViewBag.FailedMessage = TempData["FailedMessage"];
            ViewBag.AlertMessage = TempData["AlertMessage"];
            return View(q);
        }
        #endregion

        #region Liabilities
        public ActionResult LiabilitiesCustomer(string AllData, string TargetSearch)
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

            var startDate = Request["startDate"];
            var endDate = Request["endDate"];
            var model = transactionLiabilitiesRepository.GetvwAccount_Liabilities().Where(item => item.TargetModule == "Customer").ToList();
            decimal sum = 0;
            if (intBrandID > 0)
            {
                model = model.Where(x => x.BranchId == intBrandID).ToList();
            }
            //if (!string.IsNullOrEmpty(TargetCode))
            //{
            //    model = model.Where(item => item.TargetCode == TargetCode);
            //}
            if (!string.IsNullOrEmpty(TargetSearch))
            {
                TargetSearch = Helpers.Common.ChuyenThanhKhongDau(TargetSearch);
                //model = model.Where(item => item.TargetName.Contains(TargetSearch) || item.TargetCode.Contains(TargetSearch)).ToList();
                model = model.Where(item => Helpers.Common.ChuyenThanhKhongDau(item.TargetName).Contains(TargetSearch) || Helpers.Common.ChuyenThanhKhongDau(item.TargetCode).Contains(TargetSearch)).ToList();
            }

            if (!string.IsNullOrEmpty(startDate) && !string.IsNullOrEmpty(endDate))
            {
                DateTime start_d;
                if (DateTime.TryParseExact(startDate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out start_d))
                {
                    DateTime end_d;
                    if (DateTime.TryParseExact(endDate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out end_d))
                    {
                        end_d = end_d.AddHours(23).AddMinutes(59).AddSeconds(59);
                        model = model.Where(x => start_d <= x.CreatedDate && x.CreatedDate <= end_d).ToList();
                    }
                }
            }

            //if (!string.IsNullOrEmpty(TargetName))
            //{
            //    model = model.Where(item => item.TargetName.Contains(TargetName));
            //}

            if (AllData != "on")
            {
                model = model.Where(item => item.Remain > 0).ToList();
            }

            model = model.OrderBy(item => item.TargetName).ToList();
            foreach (var i in model)
            {
                sum = sum + i.Remain;
            }
            ViewBag.SumCongNo = sum;
            return View(model);
        }

        public ActionResult LiabilitiesSupplier(/*string TargetCode, string TargetName*/string txtSearch, string AllData)
        {
            var model = transactionLiabilitiesRepository.GetvwAccount_Liabilities().Where(item => item.TargetModule == "Supplier").AsEnumerable();

            //if (!string.IsNullOrEmpty(TargetCode))
            //{
            //    model = model.Where(item => item.TargetCode == TargetCode);
            //}

            //if (!string.IsNullOrEmpty(TargetName))
            //{
            //    model = model.Where(item => item.TargetName.Contains(TargetName));
            //}
            if (!string.IsNullOrEmpty(txtSearch))
            {
                model = model.Where(item => item.TargetCode == txtSearch || Helpers.Common.ChuyenThanhKhongDau(item.TargetName).ToLower().Contains(txtSearch));
            }
            if (AllData != "on")
            {
                model = model.Where(item => item.Remain > 0);
            }

            model = model.OrderBy(item => item.TargetName);
            return View(model);
        }

        public ActionResult LiabilitiesDrugStore(string TargetCode, string TargetName, string AllData)
        {
            var model = transactionLiabilitiesRepository.GetvwAccount_Liabilities().Where(item => item.TargetModule == "DrugStore");

            if (!string.IsNullOrEmpty(TargetCode))
            {
                model = model.Where(item => item.TargetCode == TargetCode);
            }

            if (!string.IsNullOrEmpty(TargetName))
            {
                model = model.Where(item => item.TargetName.Contains(TargetName));
            }

            if (AllData != "on")
            {
                model = model.Where(item => item.Remain > 0);
            }

            model = model.OrderBy(item => item.TargetName);
            return View(model);
        }

        public ViewResult LiabilitiesDetail(string TargetModule, string TargetCode, string TargetName, string TransactionCode)
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

            var model = new ResolveLiabilitiesViewModel();
            if (TargetModule == "Customer")
            {
                model.TransactionName = "Bán hàng";
                model.NextPaymentDate = DateTime.Now.AddMonths(1);
                model.TargetModule = TargetModule;
                model.TargetName = TargetName;
                model.Amount = 0;

                //Lấy danh sách chứng từ bán hàng của KH này
                var listProductInvoice = productInvoiceRepository.GetAllvwProductInvoiceFull_NVKD()
                    .Where(item => (item.IsArchive && item.CustomerCode == TargetCode && item.Status == "complete") || (item.CustomerCode == TargetCode && item.Status == "Đặt cọc"))
                    .Select(item => new ProductInvoiceViewModel
                    {
                        Id = item.Id,
                        Code = item.Code,
                        CreatedDate = item.CreatedDate,
                        TotalAmount = item.TotalAmount,
                        PaidAmount = item.tiendathu,
                        RemainingAmount = item.tienconno,
                        Note = item.Note,
                        NextPaymentDate = item.NextPaymentDate
                    }).OrderByDescending(x => x.CreatedDate)
                    .ToList();

            

                //int n = 0;
                //foreach (var item in listProductInvoice)
                //{
                //    item.OrderNo = n;
                //    n++;
                //}
                model.ListProductInvoice = new List<ProductInvoiceViewModel>();
                AutoMapper.Mapper.Map(listProductInvoice, model.ListProductInvoice);
            }
            else
                if (TargetModule == "DrugStore")
            {
                model.TransactionName = "Xuất hàng cho Chi nhánh";
                model.NextPaymentDate = DateTime.Now.AddMonths(1);
                model.TargetModule = TargetModule;
                model.TargetName = TargetName;
                model.Amount = 0;

                //Lấy danh sách chứng từ bán hàng của KH này
                var listProductInvoice = transactionLiabilitiesRepository.GetAllvwTransaction()
                    .Where(item => item.TargetModule == "DrugStore" && item.TargetCode == TargetCode && item.TransactionModule == "ProductOutbound")
                    .Select(item => new ProductInvoiceViewModel
                    {
                        Id = item.Id,
                        Code = item.TransactionCode,
                        CreatedDate = item.CreatedDate,
                        TotalAmount = item.Debit,
                            //  PaidAmount = item.PaidAmount,
                            //  RemainingAmount = item.Debit,
                            Note = item.Note,
                        NextPaymentDate = item.NextPaymentDate
                    }).OrderByDescending(x => x.CreatedDate)
                    .ToList();

                foreach (var item in listProductInvoice)
                {
                    item.ListTransactionLiabilities = new List<TransactionLiabilitiesViewModel>();

                    var q = transactionLiabilitiesRepository.GetAllvwTransaction()
                    .Where(x => x.MaChungTuGoc == item.Code && x.BranchId == intBrandID)
                    .OrderByDescending(x => x.CreatedDate)
                    .ToList();

                    item.PaidAmount = q.Sum(i => i.Credit);
                    item.RemainingAmount = item.TotalAmount - item.PaidAmount;

                    AutoMapper.Mapper.Map(q, item.ListTransactionLiabilities);
                }

                //int n = 0;
                //foreach (var item in listProductInvoice)
                //{
                //    item.OrderNo = n;
                //    n++;
                //}
                model.ListProductInvoice = new List<ProductInvoiceViewModel>();
                AutoMapper.Mapper.Map(listProductInvoice, model.ListProductInvoice);
            }
            else
            {
                model.TransactionName = "Mua hàng";
                model.NextPaymentDate = DateTime.Now.AddMonths(1);
                model.TargetModule = TargetModule;
                model.TargetName = TargetName;
                model.Amount = 0;

                //Lấy danh sách chứng từ mua hàng từ nhà cung cấp này
                var listPurchaseOrder = purchaseOrderRepository.GetAllvwPurchaseOrder()
                    .Where(item => item.IsArchive.Value && item.SupplierCode == TargetCode)
                    .Select(item => new PurchaseOrderViewModel
                    {
                        Id = item.Id,
                        Code = item.Code,
                        CreatedDate = item.CreatedDate,
                        TotalAmount = item.TotalAmount,
                        PaidAmount = item.PaidAmount,
                        RemainingAmount = item.RemainingAmount,
                        Note = item.Note,
                        NextPaymentDate = item.NextPaymentDate
                    }).OrderByDescending(x => x.CreatedDate)
                    .ToList();

                foreach (var item in listPurchaseOrder)
                {
                    item.ListTransactionLiabilities = new List<TransactionLiabilitiesViewModel>();

                    var q = transactionLiabilitiesRepository.GetAllvwTransaction()
                    .Where(x => x.MaChungTuGoc == item.Code && x.BranchId == intBrandID)
                    .OrderByDescending(x => x.CreatedDate)
                    .ToList();

                    item.PaidAmount = q.Sum(i => i.Credit);
                    item.RemainingAmount = item.TotalAmount - item.PaidAmount;

                    AutoMapper.Mapper.Map(q, item.ListTransactionLiabilities);
                }

                //int n = 0;
                //foreach (var item in listProductInvoice)
                //{
                //    item.OrderNo = n;
                //    n++;
                //}
                model.ListPurchaseOrder = new List<PurchaseOrderViewModel>();
                AutoMapper.Mapper.Map(listPurchaseOrder, model.ListPurchaseOrder);
            }

            ViewBag.SuccessMessage = TempData["SuccessMessage"];
            ViewBag.FailedMessage = TempData["FailedMessage"];
            ViewBag.AlertMessage = TempData["AlertMessage"];

            return View(model);
        }

        [HttpPost]
        public ActionResult ResolveLiabilities(List<ResolveLiabilitiesViewModel> model, string PaymentMethod, decimal? Amount, string TargetCode, string TargetModule)
        {
            //if (ModelState.IsValid)
            //{
            if (TargetModule == "Customer")
            {



                using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.Snapshot }))
                {
                    var customer = customerRepository.GetvwCustomerByCode(TargetCode);
                    var receipt = new Receipt();
                    receipt.IsDeleted = false;
                    receipt.CreatedUserId = WebSecurity.CurrentUserId;
                    receipt.ModifiedUserId = WebSecurity.CurrentUserId;
                    receipt.AssignedUserId = WebSecurity.CurrentUserId;
                    receipt.CreatedDate = DateTime.Now;
                    receipt.ModifiedDate = DateTime.Now;
                    receipt.VoucherDate = DateTime.Now;
                    receipt.BranchId = Helpers.Common.CurrentUser.BranchId.HasValue ? Helpers.Common.CurrentUser.BranchId.Value : 0;
                    receipt.Name = "Thu tiền khách hàng";

                    receipt.CustomerId = customer.Id;
                    receipt.Address = customer.Address;
                    receipt.Amount = Amount;
                    receipt.PaymentMethod = PaymentMethod;
                    receiptRepository.InsertReceipt(receipt);

                    var prefix = Erp.BackOffice.Helpers.Common.GetSetting("prefixOrderNo_ReceiptCustomer");
                    receipt.Code = Erp.BackOffice.Helpers.Common.GetCode(prefix, receipt.Id);
                    receiptRepository.UpdateReceipt(receipt);
                    foreach (var item in model)
                    {
                        if (item.Amount != null)
                        {
                            ResolveLiabilities(item, PaymentMethod, receipt.Code, receipt.Id);
                        }
                    }
                    scope.Complete();
                }
            }
            else
                if (TargetModule == "DrugStore")
            {

                using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.Snapshot }))
                {
                    var branch = branchRepository.GetvwBranchByCode(TargetCode);
                    var receipt = new Receipt();
                    receipt.IsDeleted = false;
                    receipt.CreatedUserId = WebSecurity.CurrentUserId;
                    receipt.ModifiedUserId = WebSecurity.CurrentUserId;
                    receipt.AssignedUserId = WebSecurity.CurrentUserId;
                    receipt.CreatedDate = DateTime.Now;
                    receipt.ModifiedDate = DateTime.Now;
                    receipt.VoucherDate = DateTime.Now;

                    receipt.Name = "Thu tiền Chi nhánh";
                    receipt.BranchId = branch.Id;
                    receipt.CustomerId = branch.Id;
                    receipt.Address = branch.Address;

                    receipt.Amount = Amount;
                    receipt.PaymentMethod = PaymentMethod;
                    receiptRepository.InsertReceipt(receipt);

                    var prefix = Erp.BackOffice.Helpers.Common.GetSetting("prefixOrderNo_ReceiptCustomer");
                    receipt.Code = Erp.BackOffice.Helpers.Common.GetCode(prefix, receipt.Id);
                    receiptRepository.UpdateReceipt(receipt);
                    foreach (var item in model)
                    {
                        if (item.Amount != null)
                        {
                            ResolveLiabilities(item, PaymentMethod, receipt.Code, receipt.Id);
                        }
                    }
                    scope.Complete();
                }
            }
            else
            {
                using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.Snapshot }))
                {
                    var supplier = supplierRepository.GetAllSupplier().Where(x => x.Code == TargetCode).FirstOrDefault();
                    var payment = new Payment();
                    payment.IsDeleted = false;
                    payment.CreatedUserId = WebSecurity.CurrentUserId;
                    payment.ModifiedUserId = WebSecurity.CurrentUserId;
                    payment.AssignedUserId = WebSecurity.CurrentUserId;
                    payment.CreatedDate = DateTime.Now;
                    payment.ModifiedDate = DateTime.Now;
                    payment.VoucherDate = DateTime.Now;

                    payment.Name = "Trả tiền cho nhà cung cấp";

                    payment.TargetId = supplier.Id;
                    payment.TargetName = "Supplier";
                    payment.Address = supplier.Address;
                    payment.Amount = Amount;
                    payment.PaymentMethod = PaymentMethod;
                    paymentRepository.InsertPayment(payment);

                    var prefix = Erp.BackOffice.Helpers.Common.GetSetting("prefixOrderNo_PaymentSupplier");
                    payment.Code = Erp.BackOffice.Helpers.Common.GetCode(prefix, payment.Id);
                    paymentRepository.UpdatePayment(payment);
                    foreach (var item in model)
                    {
                        if (item.Amount != null)
                        {
                            ResolveLiabilities(item, PaymentMethod, payment.Code, payment.Id);
                        }
                    }
                    scope.Complete();
                }
            }

            return Content("success");
            //}

            //return Content("error");
        }

        public void ResolveLiabilities(ResolveLiabilitiesViewModel model, string PaymentMethod, string receiptCode, int receiptId)
        {

            if (model.TargetModule == "Customer")
            {
                //Lấy đơn hàng và cập nhật số tiền đã trả
                var productInvoice = productInvoiceRepository.GetAllProductInvoice()
                    .Where(item => item.IsDeleted == false && item.Code == model.MaChungTuGoc && item.BranchId == Helpers.Common.CurrentUser.BranchId).FirstOrDefault();
                productInvoice.PaidAmount += Convert.ToDecimal(model.Amount);
                productInvoice.RemainingAmount = productInvoice.TotalAmount - productInvoice.PaidAmount;
                productInvoiceRepository.UpdateProductInvoice(productInvoice);

                //Lấy thông tin KH
                var customer = customerRepository.GetCustomerById(productInvoice.CustomerId.Value);

                //Lập chi tiết phiếu thu

                var receiptDetail = new ReceiptDetail();
                receiptDetail.IsDeleted = false;
                receiptDetail.CreatedUserId = WebSecurity.CurrentUserId;
                receiptDetail.ModifiedUserId = WebSecurity.CurrentUserId;
                receiptDetail.AssignedUserId = WebSecurity.CurrentUserId;
                receiptDetail.CreatedDate = DateTime.Now;
                receiptDetail.ModifiedDate = DateTime.Now;

                receiptDetail.Name = "Thu tiền khách hàng";
                receiptDetail.Amount = model.Amount;
                receiptDetail.ReceiptId = receiptId;
                receiptDetail.MaChungTuGoc = productInvoice.Code;
                receiptDetail.LoaiChungTuGoc = "ProductInvoice";

                receiptDetailRepository.InsertReceiptDetail(receiptDetail);
                //Thêm vào quản lý chứng từ
                TransactionController.Create(new TransactionViewModel
                {
                    TransactionModule = "Receipt",
                    TransactionCode = receiptCode,
                    TransactionName = "Thu tiền khách hàng"
                });

                //Thêm chứng từ liên quan
                TransactionController.CreateRelationship(new TransactionRelationshipViewModel
                {
                    TransactionA = receiptCode,
                    TransactionB = productInvoice.Code
                });

                //Lấy lịch sử giao dịch thanh toán
                var q = transactionLiabilitiesRepository.GetAllvwTransaction()
                            .Where(item => item.MaChungTuGoc == productInvoice.Code && item.BranchId == Helpers.Common.CurrentUser.BranchId).ToList();
                decimal du_no = 0;
                if (q.Count > 0)
                {
                    du_no = q.Sum(item => item.Debit - item.Credit);
                }


                bool da_thanh_toan_du = false;
                if (Convert.ToDecimal(model.Amount) == du_no)
                {
                    da_thanh_toan_du = true;
                }

                //Ghi Có TK 131 - Phải thu của khách hàng.
                Erp.BackOffice.Account.Controllers.TransactionLiabilitiesController.Create(
                    productInvoice.BranchId,
                    receiptCode,
                    "Receipt",
                    "Thu tiền khách hàng",
                    model.TargetCode,
                    model.TargetModule,
                    0,
                    Convert.ToDecimal(model.Amount),
                    model.MaChungTuGoc,
                    model.LoaiChungTuGoc,
                    model.PaymentMethod = PaymentMethod,
                    da_thanh_toan_du ? null : model.NextPaymentDate,
                    model.Note);

                //Cập nhật ngày hẹn trả cho đơn hàng này
                productInvoice.NextPaymentDate = da_thanh_toan_du ? null : model.NextPaymentDate;
                productInvoiceRepository.UpdateProductInvoice(productInvoice);
            }
            else
                if (model.TargetModule == "DrugStore")
            {
                ////Lấy đơn hàng và cập nhật số tiền đã trả
                var productOutbound = productOutboundRepository.GetAllvwProductOutbound()
                    .Where(item => item.IsArchive && item.IsDeleted == false && item.Code == model.MaChungTuGoc).FirstOrDefault();
                //productInvoice.PaidAmount += Convert.ToDecimal(model.Amount);
                //productInvoice.RemainingAmount = productInvoice.TotalAmount - productInvoice.PaidAmount;
                //productInvoiceRepository.UpdateProductInvoice(productInvoice);

                //Lấy thông tin KH
                //var wh = warehouseRepository.GetWarehouseById(productOutbound.WarehouseDestinationId.Value);
                //var branch = branchRepository.GetBranchById(wh.BranchId.Value);

                //Lập chi tiết phiếu thu

                var receiptDetail = new ReceiptDetail();
                receiptDetail.IsDeleted = false;
                receiptDetail.CreatedUserId = WebSecurity.CurrentUserId;
                receiptDetail.ModifiedUserId = WebSecurity.CurrentUserId;
                receiptDetail.AssignedUserId = WebSecurity.CurrentUserId;
                receiptDetail.CreatedDate = DateTime.Now;
                receiptDetail.ModifiedDate = DateTime.Now;

                receiptDetail.Name = "Thu tiền khách hàng";
                receiptDetail.Amount = model.Amount;
                receiptDetail.ReceiptId = receiptId;
                receiptDetail.MaChungTuGoc = productOutbound.Code;
                receiptDetail.LoaiChungTuGoc = "ProductOutbound";

                receiptDetailRepository.InsertReceiptDetail(receiptDetail);
                //Thêm vào quản lý chứng từ
                TransactionController.Create(new TransactionViewModel
                {
                    TransactionModule = "Receipt",
                    TransactionCode = receiptCode,
                    TransactionName = "Thu tiền Chi nhánh"
                });

                //Thêm chứng từ liên quan
                TransactionController.CreateRelationship(new TransactionRelationshipViewModel
                {
                    TransactionA = receiptCode,
                    TransactionB = productOutbound.Code
                });

                //Lấy lịch sử giao dịch thanh toán
                var q = transactionLiabilitiesRepository.GetAllvwTransaction()
                            .Where(item => item.MaChungTuGoc == productOutbound.Code && item.BranchId == Helpers.Common.CurrentUser.BranchId).ToList();
                decimal du_no = 0;
                if (q.Count > 0)
                {
                    du_no = q.Sum(item => item.Debit - item.Credit);
                }


                bool da_thanh_toan_du = false;
                if (Convert.ToDecimal(model.Amount) == du_no)
                {
                    da_thanh_toan_du = true;
                }

                //Ghi Có TK 131 - Phải thu của khách hàng.
                Erp.BackOffice.Account.Controllers.TransactionLiabilitiesController.Create(
                    productOutbound.BranchId,
                    receiptCode,
                    "Receipt",
                    "Thu tiền Chi nhánh",
                    model.TargetCode,
                    model.TargetModule,
                    0,
                    Convert.ToDecimal(model.Amount),
                    model.MaChungTuGoc,
                    model.LoaiChungTuGoc,
                    model.PaymentMethod = PaymentMethod,
                    da_thanh_toan_du ? null : model.NextPaymentDate,
                    model.Note);

                ////Cập nhật ngày hẹn trả cho đơn hàng này
                //productInvoice.NextPaymentDate = da_thanh_toan_du ? null : model.NextPaymentDate;
                //productInvoiceRepository.UpdateProductInvoice(productInvoice);
            }
            else
            {
                //Lấy đơn hàng và cập nhật số tiền đã trả
                var purchaseOrder = purchaseOrderRepository.GetAllPurchaseOrder()
                    .Where(item => item.IsArchive.Value && item.IsDeleted == false && item.Code == model.MaChungTuGoc).FirstOrDefault();
                purchaseOrder.PaidAmount += Convert.ToDecimal(model.Amount);
                purchaseOrder.RemainingAmount = purchaseOrder.TotalAmount - purchaseOrder.PaidAmount;
                purchaseOrderRepository.UpdatePurchaseOrder(purchaseOrder);

                //Lấy thông tin KH
                var supplier = supplierRepository.GetSupplierById(purchaseOrder.SupplierId.Value);
                var payment = new Payment();
                payment.IsDeleted = false;
                payment.CreatedUserId = WebSecurity.CurrentUserId;
                payment.ModifiedUserId = WebSecurity.CurrentUserId;
                payment.AssignedUserId = WebSecurity.CurrentUserId;
                payment.CreatedDate = DateTime.Now;
                payment.ModifiedDate = DateTime.Now;
                payment.MaChungTuGoc = model.MaChungTuGoc;
                payment.LoaiChungTuGoc = model.LoaiChungTuGoc;

                paymentRepository.InsertPayment(payment);

                var prefix = Erp.BackOffice.Helpers.Common.GetSetting("prefixOrderNo_PaymentSupplier");
                payment.Code = Erp.BackOffice.Helpers.Common.GetCode(prefix, payment.Id);
                paymentRepository.UpdatePayment(payment);

                //Thêm vào quản lý chứng từ
                TransactionController.Create(new TransactionViewModel
                {
                    TransactionModule = "Payment",
                    TransactionCode = payment.Code,
                    TransactionName = "Chi tiền nhà cung cấp"
                });

                //Thêm chứng từ liên quan
                TransactionController.CreateRelationship(new TransactionRelationshipViewModel
                {
                    TransactionA = payment.Code,
                    TransactionB = purchaseOrder.Code
                });
                //Lấy lịch sử giao dịch thanh toán
                var q = transactionLiabilitiesRepository.GetAllvwTransaction()
                            .Where(item => item.MaChungTuGoc == purchaseOrder.Code && item.BranchId == Helpers.Common.CurrentUser.BranchId).ToList();
                decimal du_no = 0;
                if (q.Count > 0)
                {
                    du_no = q.Sum(item => item.Debit - item.Credit);
                }


                bool da_thanh_toan_du = false;
                if (Convert.ToDecimal(model.Amount) == du_no)
                {
                    da_thanh_toan_du = true;
                }
                //Ghi Có TK ??? - Phải trả nhà cung cấp
                Erp.BackOffice.Account.Controllers.TransactionLiabilitiesController.Create(
                    null,
                    payment.Code,
                    "Payment",
                    "Chi tiền nhà cung cấp",
                    model.TargetCode,
                    model.TargetModule,
                    0,
                    Convert.ToDecimal(model.Amount),
                    model.MaChungTuGoc,
                    model.LoaiChungTuGoc,
                    model.PaymentMethod = PaymentMethod,
                    model.NextPaymentDate,
                    model.Note);
                //Cập nhật ngày hẹn trả cho đơn hàng này
                purchaseOrder.NextPaymentDate = da_thanh_toan_du ? null : model.NextPaymentDate;
                purchaseOrderRepository.UpdatePurchaseOrder(purchaseOrder);
            }

            //Update Process Payment
            if (model.ProcessPaymentId > 0)
            {
                var processPayment = processPaymentRepository.GetProcessPaymentById(model.ProcessPaymentId.Value);
                processPayment.ModifiedUserId = WebSecurity.CurrentUserId;
                processPayment.ModifiedDate = DateTime.Now;
                processPayment.Status = "Đã thanh toán";

                processPaymentRepository.UpdateProcessPayment(processPayment);
            }
        }

        public ViewResult TrackLiabilities(int? year, int? month, string type)
        {
            if (year == null)
            {
                year = DateTime.Now.Year;
                month = DateTime.Now.Month;
            }
            if (type.Contains("Customer") == true)
            {
                #region listProductInvoice
                //Lấy danh sách chứng từ công nợ
                var listProductInvoice = productInvoiceRepository.GetAllvwProductInvoiceFull_NVKD()
                    .Where(item =>item.Year == year
                            && item.tienconno != null
                            && item.tienconno > 0
                            )
                    .Select(item => new ProductInvoiceViewModel
                    {
                        Id = item.Id,
                        Code = item.Code,
                        CreatedDate = item.CreatedDate,
                        TotalAmount = item.TotalAmount,
                        PaidAmount = item.tiendathu,
                        RemainingAmount = item.tienconno,
                        Note = item.Note,
                        CustomerName = item.CustomerName,
                        tienconno= item.tienconno,
                        CustomerCode = item.CustomerCode,
                        NextPaymentDate = item.NextPaymentDate
                    }).OrderBy(x => x.NextPaymentDate).ToList();

             
                ViewBag.Year = year;
                ViewBag.Month = month;
                ViewBag.Type = type;
                ViewBag.SuccessMessage = TempData["SuccessMessage"];
                ViewBag.FailedMessage = TempData["FailedMessage"];
                ViewBag.AlertMessage = TempData["AlertMessage"];
                return View("TrackLiabilities", listProductInvoice);
                #endregion
            }
            else
            {
                #region listPurchaseOrder
                //Lấy danh sách chứng từ công nợ
                var listPurchaseOrder = purchaseOrderRepository.GetAllvwPurchaseOrder()
                    .Where(item => item.NextPaymentDate != null
                            && item.NextPaymentDate.Value.Year == year
                            && item.NextPaymentDate.Value.Month == month)
                    .Select(item => new PurchaseOrderViewModel
                    {
                        Id = item.Id,
                        Code = item.Code,
                        CreatedDate = item.CreatedDate,
                        TotalAmount = item.TotalAmount,
                        PaidAmount = item.PaidAmount,
                        RemainingAmount = item.RemainingAmount,
                        Note = item.Note,
                        SupplierName = item.SupplierName,
                        //Phone = item.Phone,
                        SupplierCode = item.SupplierCode,
                        NextPaymentDate = item.NextPaymentDate
                    }).OrderBy(x => x.NextPaymentDate).ToList();

                foreach (var item in listPurchaseOrder)
                {
                    //item.ListTransactionLiabilities = new List<TransactionLiabilitiesViewModel>();

                    var q = transactionLiabilitiesRepository.GetAllvwTransaction()
                    .Where(x => x.MaChungTuGoc == item.Code && x.BranchId == Helpers.Common.CurrentUser.BranchId)
                    .OrderByDescending(x => x.CreatedDate)
                    .ToList();

                    item.PaidAmount = q.Sum(i => i.Credit);
                    item.RemainingAmount = item.TotalAmount - item.PaidAmount;

                    //AutoMapper.Mapper.Map(q, item.ListTransactionLiabilities);
                }
                ViewBag.Year = year;
                ViewBag.Month = month;
                ViewBag.Type = type;
                ViewBag.SuccessMessage = TempData["SuccessMessage"];
                ViewBag.FailedMessage = TempData["FailedMessage"];
                ViewBag.AlertMessage = TempData["AlertMessage"];
                return View("TrackLiabilitiesSupplier", listPurchaseOrder);
                #endregion
            }
        }

        public ViewResult TrackLiabilitiesExpire(int? DateRange, string type)
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
            if (DateRange == null)
            {
                DateRange = 30;
                if (type.Contains("Customer") == true)
                {
                    #region listProductInvoice
                    //Lấy danh sách chứng từ công nợ
                    var listProductInvoice = productInvoiceRepository.GetAllvwProductInvoiceFull_NVKD()
                        .Where(item => ((item.NextPaymentDate != null
                            && item.NextPaymentDate < DateTime.Now)
                            ||
                            ((item.Year.Value== DateTime.Now.Year) && (item.Month.Value <= (DateTime.Now.Month-1)))
                            ||
                            (item.Year.Value < DateTime.Now.Year)
                            )
                            && item.tienconno!=null
                            && item.tienconno>0 && item.BranchId== intBrandID.Value)
                        .Select(item => new ProductInvoiceViewModel
                        {
                            Id = item.Id,
                            Code = item.Code,
                            CreatedDate = item.CreatedDate,
                            TotalAmount = item.TotalAmount,
                            PaidAmount = item.tiendathu,
                            RemainingAmount = item.tienconno,
                            Note = item.Note,
                            CustomerName = item.CustomerName,
                            tienconno= item.tienconno,
                            CustomerCode = item.CustomerCode,
                            NextPaymentDate = item.NextPaymentDate
                        }).OrderBy(x => x.CreatedDate).ToList();

                    

                    return View("TrackLiabilitiesExpire", listProductInvoice);
                    #endregion
                }
                else
                {
                    #region listPurchaseOrder
                    //Lấy danh sách chứng từ công nợ
                    var listPurchaseOrder = purchaseOrderRepository.GetAllvwPurchaseOrder()
                        .Where(item => item.NextPaymentDate != null
                            && item.NextPaymentDate < DateTime.Now)
                        .Select(item => new PurchaseOrderViewModel
                        {
                            Id = item.Id,
                            Code = item.Code,
                            CreatedDate = item.CreatedDate,
                            TotalAmount = item.TotalAmount,
                            PaidAmount = item.PaidAmount,
                            RemainingAmount = item.RemainingAmount,
                            Note = item.Note,
                            SupplierName = item.SupplierName,
                            //Phone = item.Phone,
                            SupplierCode = item.SupplierCode,
                            NextPaymentDate = item.NextPaymentDate
                        }).OrderBy(x => x.NextPaymentDate).ToList();

                    foreach (var item in listPurchaseOrder)
                    {
                        //item.ListTransactionLiabilities = new List<TransactionLiabilitiesViewModel>();

                        var q = transactionLiabilitiesRepository.GetAllvwTransaction()
                        .Where(x => x.MaChungTuGoc == item.Code && x.BranchId == Helpers.Common.CurrentUser.BranchId)
                        .OrderByDescending(x => x.CreatedDate)
                        .ToList();

                        item.PaidAmount = q.Sum(i => i.Credit);
                        item.RemainingAmount = item.TotalAmount - item.PaidAmount;

                        //AutoMapper.Mapper.Map(q, item.ListTransactionLiabilities);
                    }

                    return View("TrackLiabilitiesExpireSupplier", listPurchaseOrder);
                    #endregion
                }
            }
            else
            {
                var date = DateTime.Now.AddDays(DateRange.Value);

                if (type.Contains("Customer") == true)
                {
                    #region listProductInvoice
                    //Lấy danh sách chứng từ công nợ
                    var listProductInvoice = productInvoiceRepository.GetAllvwProductInvoiceFull_NVKD()
                        .Where(item => ((item.NextPaymentDate != null
                            && item.NextPaymentDate < DateTime.Now)
                            ||
                            ((item.Year.Value == DateTime.Now.Year) && (item.Month.Value == (DateTime.Now.Month)))
                            )
                            && item.tienconno != null
                            && item.tienconno > 0 && item.BranchId == intBrandID.Value)
                        .Select(item => new ProductInvoiceViewModel
                        {
                            Id = item.Id,
                            Code = item.Code,
                            CreatedDate = item.CreatedDate,
                            TotalAmount = item.TotalAmount,
                            PaidAmount = item.tiendathu,
                            RemainingAmount = item.tienconno,
                            Note = item.Note,
                            CustomerName = item.CustomerName,
                            tienconno=item.tienconno,
                            CustomerCode = item.CustomerCode,
                            NextPaymentDate = item.NextPaymentDate
                        }).OrderBy(x => x.NextPaymentDate).ToList();

                    return View("TrackLiabilitiesExpire", listProductInvoice);
                    #endregion
                }
                else
                {
                    #region listProductInvoice
                    //Lấy danh sách chứng từ công nợ
                    var listPurchaseOrder = purchaseOrderRepository.GetAllvwPurchaseOrder()
                        .Where(item => item.NextPaymentDate != null
                            && item.NextPaymentDate > DateTime.Now
                            && item.NextPaymentDate < date)
                        .Select(item => new PurchaseOrderViewModel
                        {
                            Id = item.Id,
                            Code = item.Code,
                            CreatedDate = item.CreatedDate,
                            TotalAmount = item.TotalAmount,
                            PaidAmount = item.PaidAmount,
                            RemainingAmount = item.RemainingAmount,
                            Note = item.Note,
                            SupplierName = item.SupplierName,
                            //Phone = item.Phone,
                            SupplierCode = item.SupplierCode,
                            NextPaymentDate = item.NextPaymentDate
                        }).OrderBy(x => x.NextPaymentDate).ToList();

                    foreach (var item in listPurchaseOrder)
                    {
                        //item.ListTransactionLiabilities = new List<TransactionLiabilitiesViewModel>();

                        var q = transactionLiabilitiesRepository.GetAllvwTransaction()
                        .Where(x => x.MaChungTuGoc == item.Code && x.BranchId == Helpers.Common.CurrentUser.BranchId)
                        .OrderByDescending(x => x.CreatedDate)
                        .ToList();

                        item.PaidAmount = q.Sum(i => i.Credit);
                        item.RemainingAmount = item.TotalAmount - item.PaidAmount;

                        //AutoMapper.Mapper.Map(q, item.ListTransactionLiabilities);
                    }

                    return View("TrackLiabilitiesExpireSupplier", listPurchaseOrder);
                    #endregion
                }
            }
        }

        public ViewResult TrackLiabilitiesSupplier(int? year, int? month)
        {
            if (year == null)
            {
                year = DateTime.Now.Year;
                month = DateTime.Now.Month;
            }

            //Lấy danh sách chứng từ công nợ
            var listPurchaseOrder = purchaseOrderRepository.GetAllvwPurchaseOrder()
                .Where(item => item.NextPaymentDate != null
                        && item.NextPaymentDate.Value.Year == year
                        && item.NextPaymentDate.Value.Month == month)
                .Select(item => new PurchaseOrderViewModel
                {
                    Id = item.Id,
                    Code = item.Code,
                    CreatedDate = item.CreatedDate,
                    TotalAmount = item.TotalAmount,
                    PaidAmount = item.PaidAmount,
                    RemainingAmount = item.RemainingAmount,
                    Note = item.Note,
                    SupplierName = item.SupplierName,
                    //Phone = item.Phone,
                    SupplierCode = item.SupplierCode,
                    NextPaymentDate = item.NextPaymentDate
                }).OrderBy(x => x.NextPaymentDate).ToList();

            foreach (var item in listPurchaseOrder)
            {
                //item.ListTransactionLiabilities = new List<TransactionLiabilitiesViewModel>();

                var q = transactionLiabilitiesRepository.GetAllvwTransaction()
                .Where(x => x.MaChungTuGoc == item.Code && x.BranchId == Helpers.Common.CurrentUser.BranchId)
                .OrderByDescending(x => x.CreatedDate)
                .ToList();

                item.PaidAmount = q.Sum(i => i.Credit);
                item.RemainingAmount = item.TotalAmount - item.PaidAmount;

                //AutoMapper.Mapper.Map(q, item.ListTransactionLiabilities);
            }

            ViewBag.Year = year;
            ViewBag.Month = month;
            ViewBag.SuccessMessage = TempData["SuccessMessage"];
            ViewBag.FailedMessage = TempData["FailedMessage"];
            ViewBag.AlertMessage = TempData["AlertMessage"];

            return View(listPurchaseOrder);
        }

        public ViewResult TrackLiabilitiesExpireSupplier(int? DateRange)
        {
            if (DateRange == null)
            {
                //Lấy danh sách chứng từ công nợ
                var listPurchaseOrder = purchaseOrderRepository.GetAllvwPurchaseOrder()
                    .Where(item => item.NextPaymentDate != null
                        && item.NextPaymentDate < DateTime.Now)
                    .Select(item => new PurchaseOrderViewModel
                    {
                        Id = item.Id,
                        Code = item.Code,
                        CreatedDate = item.CreatedDate,
                        TotalAmount = item.TotalAmount,
                        PaidAmount = item.PaidAmount,
                        RemainingAmount = item.RemainingAmount,
                        Note = item.Note,
                        SupplierName = item.SupplierName,
                        //Phone = item.Phone,
                        SupplierCode = item.SupplierCode,
                        NextPaymentDate = item.NextPaymentDate
                    }).OrderBy(x => x.NextPaymentDate).ToList();

                foreach (var item in listPurchaseOrder)
                {
                    //item.ListTransactionLiabilities = new List<TransactionLiabilitiesViewModel>();

                    var q = transactionLiabilitiesRepository.GetAllvwTransaction()
                    .Where(x => x.MaChungTuGoc == item.Code && x.BranchId == Helpers.Common.CurrentUser.BranchId)
                    .OrderByDescending(x => x.CreatedDate)
                    .ToList();

                    item.PaidAmount = q.Sum(i => i.Credit);
                    item.RemainingAmount = item.TotalAmount - item.PaidAmount;

                    //AutoMapper.Mapper.Map(q, item.ListTransactionLiabilities);
                }

                return View(listPurchaseOrder);
            }
            else
            {
                var date = DateTime.Now.AddDays(DateRange.Value);

                //Lấy danh sách chứng từ công nợ
                var listPurchaseOrder = purchaseOrderRepository.GetAllvwPurchaseOrder()
                    .Where(item => item.NextPaymentDate != null
                        && item.NextPaymentDate > DateTime.Now
                        && item.NextPaymentDate < date)
                    .Select(item => new PurchaseOrderViewModel
                    {
                        Id = item.Id,
                        Code = item.Code,
                        CreatedDate = item.CreatedDate,
                        TotalAmount = item.TotalAmount,
                        PaidAmount = item.PaidAmount,
                        RemainingAmount = item.RemainingAmount,
                        Note = item.Note,
                        SupplierName = item.SupplierName,
                        //Phone = item.Phone,
                        SupplierCode = item.SupplierCode,
                        NextPaymentDate = item.NextPaymentDate
                    }).OrderBy(x => x.NextPaymentDate).ToList();

                foreach (var item in listPurchaseOrder)
                {
                    //item.ListTransactionLiabilities = new List<TransactionLiabilitiesViewModel>();

                    var q = transactionLiabilitiesRepository.GetAllvwTransaction()
                    .Where(x => x.MaChungTuGoc == item.Code && x.BranchId == Helpers.Common.CurrentUser.BranchId)
                    .OrderByDescending(x => x.CreatedDate)
                    .ToList();

                    item.PaidAmount = q.Sum(i => i.Credit);
                    item.RemainingAmount = item.TotalAmount - item.PaidAmount;

                    //AutoMapper.Mapper.Map(q, item.ListTransactionLiabilities);
                }

                return View(listPurchaseOrder);
            }
        }

        public ViewResult TrackLiabilitiesCustomer(int? year, int? month)
        {
            if (year == null)
            {
                year = DateTime.Now.Year;
                month = DateTime.Now.Month;
            }

            //Lấy danh sách chứng từ công nợ
            var listProductInvoice = productInvoiceRepository.GetAllvwProductInvoiceFull_NVKD()
                .Where(item => item.NextPaymentDate != null
                        && item.NextPaymentDate.Value.Year == year
                        && item.NextPaymentDate.Value.Month == month)
                .Select(item => new ProductInvoiceViewModel
                {
                    Id = item.Id,
                    Code = item.Code,
                    CreatedDate = item.CreatedDate,
                    TotalAmount = item.TotalAmount,
                    PaidAmount = item.tiendathu,
                    RemainingAmount = item.tienconno,
                    Note = item.Note,
                    CustomerName = item.CustomerName,

                    CustomerCode = item.CustomerCode,
                    NextPaymentDate = item.NextPaymentDate
                }).OrderBy(x => x.NextPaymentDate).ToList();

         

            ViewBag.Year = year;
            ViewBag.Month = month;
            ViewBag.SuccessMessage = TempData["SuccessMessage"];
            ViewBag.FailedMessage = TempData["FailedMessage"];
            ViewBag.AlertMessage = TempData["AlertMessage"];

            return View(listProductInvoice);
        }

        public ViewResult TrackLiabilitiesExpireCustomer(int? DateRange)
        {
            if (DateRange == null)
            {
                //Lấy danh sách chứng từ công nợ
                var listProductInvoice = productInvoiceRepository.GetAllvwProductInvoiceFull_NVKD()
                    .Where(item => item.NextPaymentDate != null
                        && item.NextPaymentDate < DateTime.Now)
                    .Select(item => new ProductInvoiceViewModel
                    {
                        Id = item.Id,
                        Code = item.Code,
                        CreatedDate = item.CreatedDate,
                        TotalAmount = item.TotalAmount,
                        PaidAmount = item.tiendathu,
                        RemainingAmount = item.tienconno,
                        Note = item.Note,
                        CustomerName = item.CustomerName,

                        CustomerCode = item.CustomerCode,
                        NextPaymentDate = item.NextPaymentDate
                    }).OrderBy(x => x.NextPaymentDate).ToList();

             

                return View(listProductInvoice);
            }
            else
            {
                var date = DateTime.Now.AddDays(DateRange.Value);

                //Lấy danh sách chứng từ công nợ
                var listProductInvoice = productInvoiceRepository.GetAllvwProductInvoiceFull_NVKD()
                    .Where(item => item.NextPaymentDate != null
                        && item.NextPaymentDate > DateTime.Now
                        && item.NextPaymentDate < date)
                    .Select(item => new ProductInvoiceViewModel
                    {
                        Id = item.Id,
                        Code = item.Code,
                        CreatedDate = item.CreatedDate,
                        TotalAmount = item.TotalAmount,
                        PaidAmount = item.tiendathu,
                        RemainingAmount = item.tienconno,
                        Note = item.Note,
                        CustomerName = item.CustomerName,

                        CustomerCode = item.CustomerCode,
                        NextPaymentDate = item.NextPaymentDate
                    }).OrderBy(x => x.NextPaymentDate).ToList();

              

                return View(listProductInvoice);
            }
        }
        #endregion

        #region Create
        public static TransactionLiabilities Create(int? BranchId, string TransactionCode, string TransactionModule, string TransactionName, string TargetCode, string TargetModule, decimal Debit, decimal Credit, string MaChungTuGoc, string LoaiChungTuGoc, string PaymentMethod, DateTime? NextPaymentDate, string Note)
        {


            TransactionLiabilitiesRepository transactionRepository = new TransactionLiabilitiesRepository(new Domain.Account.ErpAccountDbContext());

            var transaction = new TransactionLiabilities();
            transaction.IsDeleted = false;
            transaction.CreatedUserId = WebSecurity.CurrentUserId;
            transaction.ModifiedUserId = WebSecurity.CurrentUserId;
            transaction.AssignedUserId = WebSecurity.CurrentUserId;
            transaction.CreatedDate = DateTime.Now;
            transaction.ModifiedDate = DateTime.Now;

            transaction.TransactionCode = TransactionCode;
            transaction.TransactionModule = TransactionModule;
            transaction.TransactionName = TransactionName;
            transaction.TargetCode = TargetCode;
            transaction.TargetModule = TargetModule;
            transaction.Debit = Debit;
            transaction.Credit = Credit;
            transaction.PaymentMethod = PaymentMethod;
            transaction.NextPaymentDate = NextPaymentDate;
            transaction.MaChungTuGoc = MaChungTuGoc;
            transaction.LoaiChungTuGoc = LoaiChungTuGoc;
            transaction.Note = Note;
            if (BranchId != null)
            {
                transaction.BranchId = BranchId;
            }
            else
            {
                transaction.BranchId = Helpers.Common.CurrentUser.BranchId.Value;
            }
            //Helpers.Common.CurrentUser.BranchId.Value;
            transactionRepository.InsertTransaction(transaction);
            return transaction;
        }



        public static TransactionLiabilities Create_mobile(string TransactionCode, string TransactionModule, string TransactionName, string TargetCode, string TargetModule, decimal Debit, decimal Credit, string MaChungTuGoc, string LoaiChungTuGoc, string PaymentMethod, DateTime? NextPaymentDate, string Note, int CurrentUserId)
        {
            TransactionLiabilitiesRepository transactionRepository = new TransactionLiabilitiesRepository(new Domain.Account.ErpAccountDbContext());

            var transaction = new TransactionLiabilities();
            transaction.IsDeleted = false;
            transaction.CreatedUserId = CurrentUserId;
            transaction.ModifiedUserId = CurrentUserId;
            transaction.AssignedUserId = CurrentUserId;
            transaction.CreatedDate = DateTime.Now;
            transaction.ModifiedDate = DateTime.Now;

            transaction.TransactionCode = TransactionCode;
            transaction.TransactionModule = TransactionModule;
            transaction.TransactionName = TransactionName;
            transaction.TargetCode = TargetCode;
            transaction.TargetModule = TargetModule;
            transaction.Debit = Debit;
            transaction.Credit = Credit;
            transaction.PaymentMethod = PaymentMethod;
            transaction.NextPaymentDate = NextPaymentDate;
            transaction.MaChungTuGoc = MaChungTuGoc;
            transaction.LoaiChungTuGoc = LoaiChungTuGoc;
            transaction.Note = Note;

            transactionRepository.InsertTransaction(transaction);
            return transaction;
        }
        #endregion

        #region XuatEcxel

        //[HttpPost]
        public ActionResult XuatExcel(string startDate, string endDate, string TargetSearch/*targetCode, string targetName*/, string AllData)
        {

            var model1 = transactionLiabilitiesRepository.GetvwAccount_Liabilities().Where(item => item.TargetModule == "Customer" && item.BranchId == Helpers.Common.CurrentUser.BranchId).ToList();

            //if (!string.IsNullOrEmpty(targetCode))
            //{
            //    model1 = model1.Where(item => item.TargetCode == targetCode);
            //}

            if (!string.IsNullOrEmpty(startDate) && !string.IsNullOrEmpty(endDate))
            {
                DateTime start_d;
                if (DateTime.TryParseExact(startDate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out start_d))
                {
                    DateTime end_d;
                    if (DateTime.TryParseExact(endDate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out end_d))
                    {
                        end_d = end_d.AddHours(23).AddMinutes(59).AddSeconds(59);
                        model1 = model1.Where(x => start_d <= x.CreatedDate && x.CreatedDate <= end_d).ToList();
                    }
                }
            }
            //haha
            if (!string.IsNullOrEmpty(TargetSearch))
            {
                //model1 = model1.Where(item => item.TargetName.Contains(TargetSearch));
                model1 = model1.Where(item => Helpers.Common.ChuyenThanhKhongDau(item.TargetName).Contains(TargetSearch) || Helpers.Common.ChuyenThanhKhongDau(item.TargetCode).Contains(TargetSearch)).ToList();
            }

            if (AllData != "on")
            {
                model1 = model1.Where(item => item.Remain > 0).ToList();
            }



            var model = new TemplatePrintViewModel();
            //lấy logo công ty
            var logo = Erp.BackOffice.Helpers.Common.GetSetting("LogoCompany");
            var company = Erp.BackOffice.Helpers.Common.GetSetting("companyName");
            var address = Erp.BackOffice.Helpers.Common.GetSetting("addresscompany");
            var phone = Erp.BackOffice.Helpers.Common.GetSetting("phonecompany");
            var fax = Erp.BackOffice.Helpers.Common.GetSetting("faxcompany");
            var bankcode = Erp.BackOffice.Helpers.Common.GetSetting("bankcode");
            var bankname = Erp.BackOffice.Helpers.Common.GetSetting("bankname");
            var ImgLogo = "<div class=\"logo\"><img src=" + logo + " height=\"73\" /></div>";


            var BranchId = Helpers.Common.CurrentUser.BranchId;

            // ListCN.RemoveAt(ListCN.Count() - 1);

            var data = model1.ToList();

            var template = templatePrintRepository.GetAllTemplatePrint().Where(x => x.Code.Contains("PrintReport")).OrderByDescending(x => x.CreatedDate).FirstOrDefault();
            //truyền dữ liệu vào template.
            model.Content = template.Content;
            model.Content = model.Content.Replace("{Title}", "Báo Cáo Công Nợ");
            model.Content = model.Content.Replace("{DataTable}", buildHtmlDetailList_PrintBCTK(data));
            model.Content = model.Content.Replace("{System.Logo}", ImgLogo);
            model.Content = model.Content.Replace("{System.CompanyName}", company);
            model.Content = model.Content.Replace("{System.AddressCompany}", address);
            model.Content = model.Content.Replace("{System.PhoneCompany}", phone);
            model.Content = model.Content.Replace("{System.Fax}", fax);
            model.Content = model.Content.Replace("{System.BankCodeCompany}", bankcode);
            model.Content = model.Content.Replace("{System.BankNameCompany}", bankname);
            model.Content = model.Content.Replace("{PrintDate}", DateTime.Now.ToString("dd/MM/yyyy HH:mm"));


            Response.AppendHeader("content-disposition", "attachment;filename=" + "BaoCaoCongNo" + DateTime.Now.ToString("yyyyMMdd") + ".xls");
            Response.Charset = "";
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.Write(model.Content);
            Response.Flush();
            Response.Close();
            Response.End();
            return RedirectToAction("LiabilitiesCustomer"); ;



        }

        string buildHtmlDetailList_PrintBCTK(List<vwAccount_Liabilities> detailList)
        {
            //Tạo table html chi tiết phiếu xuất
            string detailLists = "<table border=\"1\" class=\"invoice-detail\">\r\n";
            detailLists += "<thead>\r\n";
            detailLists += "	<tr>\r\n";
            detailLists += "		<th>#</th>\r\n";
            detailLists += "		<th>Mã Khách Hàng</th>\r\n";
            detailLists += "		<th>Tên Khách Hàng</th>\r\n";

            detailLists += "		<th>Ngày Tạo</th>\r\n";
            detailLists += "		<th>Công Nợ Hiện Tại</th>\r\n";

            detailLists += "	</tr>\r\n";
            detailLists += "</thead>\r\n";
            // detailLists += "<tbody>\r\n";
            var index = 1;

            foreach (var item in detailList)
            {
                detailLists += "<tr>\r\n"
                + "<td class=\"text-center orderNo\">" + (index++) + "</td>\r\n"
                + "<td class=\"text-left code_product\">" + item.TargetCode + "</td>\r\n"
                + "<td class=\"text-left \">" + item.TargetName + "</td>\r\n"
                + "<td class=\"text-right\">" + (item.CreatedDate.ToString("dd-MM-yyyy")) + "</td>\r\n"
                + "<td class=\"text-right orderNo\">" + Erp.BackOffice.Helpers.Common.PhanCachHangNgan2(item.Remain).Replace(".", ",") + "</td>\r\n"
                + "</tr>\r\n";
            }
            //detailLists += "</tbody>\r\n";
            detailLists += "<tfoot style=\"font-weight:bold\">\r\n";
            detailLists += "<tr><td colspan=\"4\" class=\"text-right\">Tổng cộng</td><td class=\"text-right\">"
                         + Erp.BackOffice.Helpers.CommonSatic.ToCurrencyStr(detailList.Sum(x => x.Remain), null)
                         + "</tr>\r\n";
            detailLists += "</tfoot>\r\n</table>\r\n";

            return detailLists;
        }

        #endregion
        //#region Edit
        //public static TransactionLiabilities Edit(string TransactionCode, string TransactionModule, string TransactionName, string TargetCode, string TargetModule, decimal Debit, decimal Credit, string MaChungTuGoc, string LoaiChungTuGoc, string PaymentMethod, DateTime? NextPaymentDate, string Note)
        //{
        //    TransactionLiabilitiesRepository transactionRepository = new TransactionLiabilitiesRepository(new Domain.Account.ErpAccountDbContext());
        //    var
        //    var transaction = new TransactionLiabilities();
        //    transaction.IsDeleted = false;
        //    transaction.CreatedUserId = WebSecurity.CurrentUserId;
        //    transaction.ModifiedUserId = WebSecurity.CurrentUserId;
        //    transaction.AssignedUserId = WebSecurity.CurrentUserId;
        //    transaction.CreatedDate = DateTime.Now;
        //    transaction.ModifiedDate = DateTime.Now;

        //    transaction.TransactionCode = TransactionCode;
        //    transaction.TransactionModule = TransactionModule;
        //    transaction.TransactionName = TransactionName;
        //    transaction.TargetCode = TargetCode;
        //    transaction.TargetModule = TargetModule;
        //    transaction.Debit = Debit;
        //    transaction.Credit = Credit;
        //    transaction.PaymentMethod = PaymentMethod;
        //    transaction.NextPaymentDate = NextPaymentDate;
        //    transaction.MaChungTuGoc = MaChungTuGoc;
        //    transaction.LoaiChungTuGoc = LoaiChungTuGoc;
        //    transaction.Note = Note;

        //    transactionRepository.InsertTransaction(transaction);
        //    return transaction;
        //}
        //#endregion
    }
}
