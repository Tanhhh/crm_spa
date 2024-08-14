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
using Erp.BackOffice.Sale.Models;
using Erp.Domain.Sale.Interfaces;
using System.Transactions;
using System.Web;
using Erp.Domain.Account.Helper;
using Erp.Domain.Sale.Entities;

namespace Erp.BackOffice.Account.Controllers
{
    [Authorize]
    [InitializeSimpleMembership]
    [Erp.BackOffice.Helpers.NoCacheHelper]
    public class ReceiptController : Controller
    {
        private readonly IReceiptRepository ReceiptRepository;
        private readonly IUserRepository userRepository;
        private readonly ITransactionLiabilitiesRepository transactionRepository;
        private readonly IProcessPaymentRepository processPaymentRepository;
        //private readonly ICustomerRepository customerRepository;
        private readonly ITemplatePrintRepository templatePrintRepository;
        private readonly IReceiptDetailRepository ReceiptDetailRepository;
        private readonly ILogReceiptRepository logReceiptRepository;
        private readonly IProductInvoiceRepository productInvoiceRepository;
        public ReceiptController(
            IReceiptRepository _Receipt
            , IUserRepository _user
            , ITransactionLiabilitiesRepository _Transaction
            , IProcessPaymentRepository _ProcessPayment
            , ICustomerRepository _customer
            , ITemplatePrintRepository _templatePrint
            , IReceiptDetailRepository _ReceiptDetail
            , ILogReceiptRepository logReceipt
            , IProductInvoiceRepository _productInvoiceRepository
            )
        {
            ReceiptRepository = _Receipt;
            userRepository = _user;
            transactionRepository = _Transaction;
            processPaymentRepository = _ProcessPayment;
            templatePrintRepository = _templatePrint;
            ReceiptDetailRepository = _ReceiptDetail;
            logReceiptRepository = logReceipt;
            productInvoiceRepository = _productInvoiceRepository;
            //customerRepository = _customer;
        }



        #region Index

        public ViewResult Index(int? CustomerId, int? SalerId, string Code, string MaChungTuGoc, string CustomerInfo)
        {
            SalerId = SalerId != null ? SalerId : 0;
            MaChungTuGoc = MaChungTuGoc != null ? MaChungTuGoc : "";
            CustomerInfo = CustomerInfo != null ? CustomerInfo : "";
            Code = Code != null ? Code : "";
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

            var start = Request["start"];
            var end = Request["end"];
            var startDate = Request["startDate"];
            var endDate = Request["endDate"];
            //IQueryable<ReceiptViewModel> q = ReceiptRepository.GetAllReceipt()
            //    .Select(item => new ReceiptViewModel
            //    {
            //        Id = item.Id,
            //        CreatedUserId = item.CreatedUserId,
            //        //CreatedUserName = item.CreatedUserName,
            //        CreatedDate = item.CreatedDate,
            //        ModifiedUserId = item.ModifiedUserId,
            //        //ModifiedUserName = item.ModifiedUserName,
            //        ModifiedDate = item.ModifiedDate,
            //        Name = item.Name,
            //        Code = item.Code,
            //        Amount = item.Amount,
            //        Address = item.Address,
            //        Note = item.Note,
            //        Payer = item.Payer,
            //        SalerId = item.SalerId,
            //        CompanyName = item.CompanyName,
            //        VoucherDate = item.VoucherDate,
            //        SalerName = item.SalerName,
            //        CustomerId = item.CustomerId,
            //        IsArchive = item.IsArchive,
            //        LoaiChungTuGoc = item.LoaiChungTuGoc,
            //        MaChungTuGoc = item.MaChungTuGoc,
            //        CustomerCode = item.CustomerCode,
            //        NVQL = item.NVQL,
            //        BranchId = item.BranchId
            //    }).OrderByDescending(x => x.CreatedDate);
            //int a = q.Count();

            //if (intBrandID > 0)
            //{
            //    q = q.Where(x => x.BranchId == intBrandID);
            //}

            
            if (start == null && end == null)
            {
                DateTime aDateTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
                // Cộng thêm 1 tháng và trừ đi một ngày.
               // DateTime retDateTime = aDateTime.AddMonths(1).AddDays(-1);
                start = aDateTime.ToString("dd/MM/yyyy");
                end = aDateTime.ToString("dd/MM/yyyy");
            }
            DateTime start_d = DateTime.Now;
            DateTime end_d = DateTime.Now;
            if (!string.IsNullOrEmpty(start) && !string.IsNullOrEmpty(end))
            {
               
                if (DateTime.TryParseExact(start, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out start_d))
                {
                    
                    if (DateTime.TryParseExact(end, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out end_d))
                    {
                        end_d = end_d.AddHours(23).AddMinutes(59).AddSeconds(59);
                        //q = q.Where(x => start_d <= x.VoucherDate && x.VoucherDate <= end_d);
                    }
                }
            }
            //if (CustomerId != null && CustomerId.Value > 0)
            //{
            //    q = q.Where(x => x.CustomerId == CustomerId);
            //}
            //if (SalerId != null && SalerId.Value > 0)
            //{
            //    q = q.Where(x => x.SalerId == SalerId);
            //}
            //if (!string.IsNullOrEmpty(Code))
            //{
            //    q = q.Where(x => x.Code.Contains(Code));
            //}
            //if (!string.IsNullOrEmpty(MaChungTuGoc))
            //{
            //    q = q.Where(x => x.MaChungTuGoc == MaChungTuGoc);
            //}

            //if (!string.IsNullOrEmpty(CustomerCode))
            //{
            //    q = q.Where(x => x.CustomerCode == CustomerCode);
            //}

            //if (!string.IsNullOrEmpty(CompanyName))
            //{
            //    q = q.Where(x => x.CompanyName.Contains(CompanyName));
            //}

            //if (!string.IsNullOrEmpty(CustomerInfo))
            //{
            //    q = q.Where(x => x.CompanyName.Contains(CustomerInfo) || x.CustomerCode == CustomerInfo);
            //}


            var model = SqlHelper.QuerySP<ReceiptViewModel>("getListReceipt", new
            {
                @startDate = start_d,
                @endDate = end_d,
                @branchId =intBrandID,
                @SalerId = SalerId,
                @CustomerInfo = CustomerInfo,
                @Code = Code,
                @MaChungTuGoc = MaChungTuGoc
            }).ToList();


            if (!string.IsNullOrEmpty(startDate) && !string.IsNullOrEmpty(endDate))
            {
                DateTime start_d2;
                if (DateTime.TryParseExact(startDate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out start_d2))
                {
                    DateTime end_d2;
                    if (DateTime.TryParseExact(endDate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out end_d2))
                    {
                        end_d2 = end_d.AddHours(23).AddMinutes(59).AddSeconds(59);
                        model = model.Where(x => start_d2 <= x.CreatedDate && x.CreatedDate <= end_d2).ToList();
                    }
                }
            }
            //getListReceipt

            ViewBag.SuccessMessage = TempData["SuccessMessage"];
            ViewBag.FailedMessage = TempData["FailedMessage"];
            ViewBag.AlertMessage = TempData["AlertMessage"];

            ViewBag.tienchuanop = Erp.BackOffice.Helpers.Common.PhanCachHangNgan2(model.Where(x => x.IsArchive == false).Sum(x => x.Amount));
            ViewBag.tongtien = Erp.BackOffice.Helpers.Common.PhanCachHangNgan2(model.Sum(x => x.Amount));
            ViewBag.tiendanop = Erp.BackOffice.Helpers.Common.PhanCachHangNgan2(model.Where(x => x.IsArchive == true).Sum(x => x.Amount));

            return View(model);
        }

        public List<ReceiptViewModel> IndexPrint(string startDate, string endDate, string start, string end, int? CustomerId, int? SalerId, string Code, string MaChungTuGoc, string CustomerInfo)
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
            var q = ReceiptRepository.GetAllReceipt().Where(x => x.BranchId == intBrandID)
                .Select(item => new ReceiptViewModel
                {
                    Id = item.Id,
                    CreatedUserId = item.CreatedUserId,
                    //CreatedUserName = item.CreatedUserName,
                    CreatedDate = item.CreatedDate,
                    ModifiedUserId = item.ModifiedUserId,
                    //ModifiedUserName = item.ModifiedUserName,
                    ModifiedDate = item.ModifiedDate,
                    Name = item.Name,
                    Code = item.Code,
                    Amount = item.Amount,
                    Address = item.Address,
                    Note = item.Note,
                    Payer = item.Payer,
                    SalerId = item.SalerId,
                    CompanyName = item.CompanyName,
                    VoucherDate = item.VoucherDate,
                    SalerName = item.SalerName,
                    CustomerId = item.CustomerId,
                    IsArchive = item.IsArchive,
                    LoaiChungTuGoc = item.LoaiChungTuGoc,
                    MaChungTuGoc = item.MaChungTuGoc,
                    CustomerCode = item.CustomerCode,
                    PaymentMethod= item.PaymentMethod,
                    NVQL = item.NVQL
                }).OrderByDescending(x => x.CreatedDate).ToList();
            if (!string.IsNullOrEmpty(startDate) && !string.IsNullOrEmpty(endDate))
            {
                DateTime start_d;
                if (DateTime.TryParseExact(startDate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out start_d))
                {
                    DateTime end_d;
                    if (DateTime.TryParseExact(endDate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out end_d))
                    {
                        end_d = end_d.AddHours(23).AddMinutes(59).AddSeconds(59);
                        q = q.Where(x => start_d <= x.CreatedDate && x.CreatedDate <= end_d).ToList();
                    }
                }
            }
            if (!string.IsNullOrEmpty(start) && !string.IsNullOrEmpty(end))
            {
                DateTime start_d;
                if (DateTime.TryParseExact(start, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out start_d))
                {
                    DateTime end_d;
                    if (DateTime.TryParseExact(end, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out end_d))
                    {
                        end_d = end_d.AddHours(23).AddMinutes(59).AddSeconds(59);
                        end_d = end_d.AddMinutes(59);
                        q = q.Where(x => start_d <= x.VoucherDate && x.VoucherDate <= end_d).ToList();
                    }
                }
            }
            if (CustomerId != null && CustomerId.Value > 0)
            {
                q = q.Where(x => x.CustomerId == CustomerId).ToList();
            }
            if (SalerId != null && SalerId.Value > 0)
            {
                q = q.Where(x => x.SalerId == SalerId).ToList();
            }
            if (!string.IsNullOrEmpty(Code))
            {
                q = q.Where(x => x.Code.Contains(Code)).ToList();
            }
            if (!string.IsNullOrEmpty(MaChungTuGoc))
            {
                q = q.Where(x => x.MaChungTuGoc == MaChungTuGoc).ToList();
            }

            //if (!string.IsNullOrEmpty(CustomerCode))
            //{
            //    q = q.Where(x => x.CustomerCode == CustomerCode).ToList();
            //}

            //if (!string.IsNullOrEmpty(CompanyName))
            //{
            //    q = q.Where(x => x.CompanyName.Contains(CompanyName)).ToList();
            //}

            if (!string.IsNullOrEmpty(CustomerInfo))
            {
                q = q.Where(x => x.CompanyName.Contains(CustomerInfo) || x.CustomerCode == CustomerInfo).ToList();
            }

            return q;
        }


        public ViewResult IndexThemphieuthu(LogReceiptViewModel Model, int? CustomerId, int? SalerId, string Code, string MaChungTuGoc, string CustomerCode, string CompanyName)
        {
            var start = Request["start"];
            var end = Request["end"];
            var startDate = Request["startDate"];
            var endDate = Request["endDate"];
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

            IQueryable<ReceiptViewModel> q = ReceiptRepository.GetAllReceipt()
            .Where(x => x.BranchId == intBrandID && (x.LogReceiptId == null || x.LogReceiptId == 0) && (x.IsArchive == false))
                .Select(item => new ReceiptViewModel
                {
                    Id = item.Id,
                    CreatedUserId = item.CreatedUserId,
                    //CreatedUserName = item.CreatedUserName,
                    CreatedDate = item.CreatedDate,
                    ModifiedUserId = item.ModifiedUserId,
                    //ModifiedUserName = item.ModifiedUserName,
                    ModifiedDate = item.ModifiedDate,
                    Name = item.Name,
                    Code = item.Code,
                    Amount = item.Amount,
                    Address = item.Address,
                    Note = item.Note,
                    Payer = item.Payer,
                    SalerId = item.SalerId,
                    CompanyName = item.CompanyName,
                    VoucherDate = item.VoucherDate,
                    SalerName = item.SalerName,
                    CustomerId = item.CustomerId,
                    IsArchive = item.IsArchive,
                    LoaiChungTuGoc = item.LoaiChungTuGoc,
                    MaChungTuGoc = item.MaChungTuGoc,
                    CustomerCode = item.CustomerCode
                }).OrderByDescending(x => x.CreatedDate);





            if (!string.IsNullOrEmpty(start) && !string.IsNullOrEmpty(end))
            {
                DateTime start_d;
                if (DateTime.TryParseExact(start, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out start_d))
                {
                    DateTime end_d;
                    if (DateTime.TryParseExact(end, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out end_d))
                    {
                        end_d = end_d.AddHours(23).AddMinutes(59).AddSeconds(59);
                        end_d.AddMinutes(59);
                        q = q.Where(x => start_d <= x.CreatedDate && x.CreatedDate <= end_d);
                    }
                }
            }






            if (CustomerId != null && CustomerId.Value > 0)
            {
                q = q.Where(x => x.CustomerId == CustomerId);
            }
            if (SalerId != null && SalerId.Value > 0)
            {
                q = q.Where(x => x.SalerId == SalerId);
            }
            if (!string.IsNullOrEmpty(Code))
            {
                q = q.Where(x => x.Code.Contains(Code));
            }
            if (!string.IsNullOrEmpty(MaChungTuGoc))
            {
                q = q.Where(x => x.MaChungTuGoc == MaChungTuGoc);
            }

            if (!string.IsNullOrEmpty(CustomerCode))
            {
                q = q.Where(x => x.CustomerCode == CustomerCode);
            }

            if (!string.IsNullOrEmpty(CompanyName))
            {
                q = q.Where(x => x.CompanyName.Contains(CompanyName));
            }

            ViewBag.SuccessMessage = TempData["SuccessMessage"];
            ViewBag.FailedMessage = TempData["FailedMessage"];
            ViewBag.AlertMessage = TempData["AlertMessage"];
            return View(q);
        }

        private int isInserOrAddnewNoptien(int noptienid, string plisphieuthuid)
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

            LogReceipt sample = null;
            if (noptienid > 0)
            {
                sample = logReceiptRepository.GetLogReceiptById(noptienid);
            }
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.Snapshot }))
            {
                try
                {

                    var _listdata = ReceiptRepository.GetAllReceipts().Where(x => x.BranchId == intBrandID).ToList();
                    var _update = _listdata.Where(id1 => plisphieuthuid.Split(',').ToList().Any(id2 => int.Parse(id2) == id1.Id)).ToList();

                    decimal? pTotalAmount = 0;
                    if (noptienid > 0)
                    {
                        var oldData = _listdata.Where(x => x.LogReceiptId != null && x.LogReceiptId == sample.Id).ToList();
                        foreach (var item in oldData)
                        {
                            pTotalAmount = pTotalAmount + item.Amount;
                        }
                    }

                    foreach (var item in _update)
                    {
                        pTotalAmount = pTotalAmount + item.Amount;
                    }

                    if (sample != null)
                    {
                        //sample.Note = model.Note;
                        sample.TotalAmount = pTotalAmount;
                        sample.BranchId = intBrandID;
                        sample.ModifiedUserId = WebSecurity.CurrentUserId;
                        sample.ModifiedDate = DateTime.Now;
                        logReceiptRepository.UpdateLogReceipt(sample);
                    }
                    else
                    {
                        sample = new LogReceipt();
                        sample.IsDeleted = false;
                        sample.TotalAmount = pTotalAmount;
                        sample.CreatedUserId = WebSecurity.CurrentUserId;
                        sample.ModifiedUserId = WebSecurity.CurrentUserId;
                        sample.AssignedUserId = WebSecurity.CurrentUserId;
                        sample.CreatedDate = DateTime.Now;
                        sample.ModifiedDate = DateTime.Now;
                        sample.DateReceipt = DateTime.Now;
                        sample.BranchId = intBrandID;
                        sample.Status = "pending";
                        sample.Code = Erp.BackOffice.Helpers.Common.GetOrderNo("LogReceipt", "NT");
                        Erp.BackOffice.Helpers.Common.SetOrderNo("LogReceipt");
                        logReceiptRepository.InsertLogReceipt(sample);
                    }

                    if (_update.Any())
                    {
                        foreach (var item in _update)
                        {
                            item.LogReceiptId = sample.Id;
                            ReceiptRepository.UpdateReceipt(item);
                        }
                    }
                    scope.Complete();
                    return sample.Id;
                }
                catch (DbUpdateException)
                {
                    TempData[Globals.FailedMessageKey] = App_GlobalResources.Wording.Error;
                    return 0;
                }
            }
        }
        [HttpPost]
        public ActionResult IndexThemphieuthu(LogReceiptViewModel Model, string noptienId)
        {
            string idDeleteAll = Request["DeleteId-checkbox"];
            int phieunoptien = isInserOrAddnewNoptien(int.Parse(noptienId), idDeleteAll);
            if (noptienId != "0")
            {
                return RedirectToAction("IndexThemphieuthu", new { IsPopup = false, Test = "abc", noptienId = phieunoptien });
            }
            else
            {
                return RedirectToAction("IndexThemphieuthu", new { IsPopup = false, Test = "abc", noptienId = "0" });
            }
        }

        #endregion

        #region Create
        public ViewResult Create()
        {
            var model = new ReceiptViewModel();

            return View(model);
        }

        [HttpPost]
        public ActionResult Create(ReceiptViewModel model)
        {
            if (ModelState.IsValid)
            {
                var receipt = new Receipt();
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

                    AutoMapper.Mapper.Map(model, receipt);
                    receipt.IsDeleted = false;
                    receipt.BranchId = intBrandID.Value;
                    receipt.CreatedUserId = WebSecurity.CurrentUserId;
                    receipt.ModifiedUserId = WebSecurity.CurrentUserId;
                    receipt.AssignedUserId = WebSecurity.CurrentUserId;
                    receipt.CreatedDate = DateTime.Now;
                    receipt.ModifiedDate = DateTime.Now;
                    //receipt.VoucherDate = DateTime.Now;
                    receipt.IsArchive = false;
                    receipt.LoaiChungTuGoc = "ProductInvoice";
                    ReceiptRepository.InsertReceipt(receipt);

                    // bắt đầu kiểm tra phiếu thu bị trùng
                    // lấy mã 
                    receipt.Code = Erp.BackOffice.Helpers.Common.GetOrderNo("Receipt", model.Code);

                    var c = ReceiptRepository.GetAllvwReceiptFull().
                        Where(item => item.Code == receipt.Code).FirstOrDefault();

                    if (c != null)
                    {
                        TempData[Globals.FailedMessageKey] = "phiếu thu này đã bị trùng, vui lòng kiểm tra lại!";
                        return RedirectToAction("Detail", new { Id = c.Id });
                    }
                    // kết thúc kiểm tra phiếu thu bị trùng
                    ReceiptRepository.UpdateReceipt(receipt);
                    Erp.BackOffice.Helpers.Common.SetOrderNo("Receipt");

                    //ghi log 
                    Erp.BackOffice.Controllers.HomeController.WriteLog(receipt.Id, receipt.Code, "đã tạo phiếu thu", "Receipt/Detail/" + receipt.Id, Helpers.Common.CurrentUser.BranchId.Value);


                    var vwproductInvoice = productInvoiceRepository.GetvwProductInvoiceByCode(receipt.BranchId, receipt.MaChungTuGoc);
                    if (receipt.Amount <= vwproductInvoice.tienconno)
                    {
                        var receiptDetail = new ReceiptDetail();
                        receiptDetail.IsDeleted = false;
                        receiptDetail.CreatedUserId = WebSecurity.CurrentUserId;
                        receiptDetail.ModifiedUserId = WebSecurity.CurrentUserId;
                        receiptDetail.AssignedUserId = WebSecurity.CurrentUserId;
                        receiptDetail.CreatedDate = DateTime.Now;
                        receiptDetail.ModifiedDate = DateTime.Now;
                        receiptDetail.Name = model.Name;
                        receiptDetail.Amount = model.Amount;
                        receiptDetail.ReceiptId = receipt.Id;
                        receiptDetail.MaChungTuGoc = receipt.MaChungTuGoc;
                        receiptDetail.LoaiChungTuGoc = "ProductInvoice";
                        ReceiptDetailRepository.InsertReceiptDetail(receiptDetail);


                        if (vwproductInvoice.Status.Contains("Đặt cọc") || vwproductInvoice.Status.Contains("pending"))
                        {
                            #region xử lý chứng từ

                            // Thêm vào quản lý chứng từ
                            TransactionController.Create(new TransactionViewModel
                            {
                                TransactionModule = "Receipt",
                                TransactionCode = receipt.Code,
                                TransactionName = "Thu tiền cho đơn hàng " + vwproductInvoice.Code
                            });

                            // Thêm chứng từ liên quan
                            TransactionController.CreateRelationship(new TransactionRelationshipViewModel
                            {
                                TransactionA = receipt.Code,
                                TransactionB = receipt.MaChungTuGoc
                            });

                            Erp.BackOffice.Account.Controllers.TransactionLiabilitiesController.Create(
                                              intBrandID,
                                              receipt.Code,
                                              "Receipt",
                                              "Thu tiền đặt cọc khách hàng cho đơn hàng " + vwproductInvoice.Code,
                                              vwproductInvoice.CustomerCode,
                                              "Customer",
                                              0,
                                              Convert.ToDecimal(receipt.Amount),
                                              vwproductInvoice.Code,
                                              "ProductInvoice",
                                              receipt.PaymentMethod,
                                              null,
                                              null);



                            vwproductInvoice.PaymentMethod = receipt.PaymentMethod;
                            vwproductInvoice.PaidAmount += Convert.ToDecimal(receipt.Amount);
                            vwproductInvoice.DoanhThu = vwproductInvoice.PaidAmount;
                            vwproductInvoice.RemainingAmount = vwproductInvoice.TotalAmount - vwproductInvoice.PaidAmount;

                            if (vwproductInvoice.RemainingAmount == 0)
                            {
                                vwproductInvoice.NextPaymentDate = null;
                                //vwproductInvoice.Status = "complete";
                            }
                            else
                            {
                                vwproductInvoice.NextPaymentDate = vwproductInvoice.NextPaymentDate.HasValue ? vwproductInvoice.NextPaymentDate.Value.AddMonths(1) : model.VoucherDate.Value.AddMonths(1);
                                vwproductInvoice.Status = "Đặt cọc";
                            }
                            vwproductInvoice.ModifiedDate = DateTime.Now;
                            vwproductInvoice.ModifiedUserId = WebSecurity.CurrentUserId;
                            ProductInvoice productinvoice = new ProductInvoice();
                            AutoMapper.Mapper.Map(vwproductInvoice, productinvoice);
                            productInvoiceRepository.UpdateProductInvoice(productinvoice);
                        }
                        #endregion
                        TempData[Globals.SuccessMessageKey] = App_GlobalResources.Wording.InsertSuccess;
                    }
                    else
                    {
                        TempData[Globals.FailedMessageKey] = "Số tiền phiếu thu " + receipt.Amount + " đã vượt quá số tiền nợ là " + vwproductInvoice.tienconno;
                        return RedirectToAction("Index");
                    }
                    scope.Complete();
                }
                if (Request.IsAjaxRequest())
                {
                    return Content("success");
                }
                else
                {
                    return RedirectToAction("Index");
                }
            }
            return View(model);
        }




        #endregion

        #region Edit
        public ActionResult Edit(int? Id)
        {
            var Receipt = ReceiptRepository.GetvwReceiptById(Id.Value);
            if (Receipt != null && Receipt.IsDeleted != true)
            {
                var model = new ReceiptViewModel();
                AutoMapper.Mapper.Map(Receipt, model);

                //if (model.CreatedUserId != Erp.BackOffice.Helpers.Common.CurrentUser.Id && Erp.BackOffice.Helpers.Common.CurrentUser.UserTypeId != 1)
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
        public ActionResult Edit(ReceiptViewModel model)
        {
            if (ModelState.IsValid)
            {
                //if (Request["Submit"] == "Save")
                //{
                var Receipt = ReceiptRepository.GetReceiptById(model.Id);
                AutoMapper.Mapper.Map(model, Receipt);
                Receipt.ModifiedUserId = WebSecurity.CurrentUserId;
                Receipt.ModifiedDate = DateTime.Now;
                ReceiptRepository.UpdateReceipt(Receipt);

                //var receiptDetail = ReceiptDetailRepository.GetReceiptDetailByReceiptId(model.Id);
                //receiptDetail.ModifiedUserId = WebSecurity.CurrentUserId;
                //receiptDetail.ModifiedDate = DateTime.Now;
                //receiptDetail.Name = model.Name;
                //receiptDetail.Amount = model.Amount;
                //ReceiptDetailRepository.UpdateReceiptDetail(receiptDetail);
                TempData[Globals.SuccessMessageKey] = App_GlobalResources.Wording.UpdateSuccess;
                return RedirectToAction("Index");
                //}

                //return View(model);
            }

            return View(model);

            //if (Request.UrlReferrer != null)
            //    return Redirect(Request.UrlReferrer.AbsoluteUri);
            //return RedirectToAction("Index");
        }





        #endregion

        #region Detail
        public ActionResult Detail(int? Id, string TransactionCode)
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

            var receipt = new vwReceipt();
            if (Id != null && Id.Value > 0)
            {
                receipt = ReceiptRepository.GetvwReceiptById(Id.Value);
            }

            if (!string.IsNullOrEmpty(TransactionCode))
            {
                receipt = ReceiptRepository.GetAllvwReceiptFull().Where(item => item.BranchId == intBrandID && item.Code == TransactionCode).FirstOrDefault();
            }

            if (receipt != null)
            {
                var model = new ReceiptViewModel();
                AutoMapper.Mapper.Map(receipt, model);

                // model.ModifiedUserName = userRepository.GetUserById(model.ModifiedUserId.Value).FullName;

                ViewBag.SuccessMessage = TempData["SuccessMessage"];
                ViewBag.FailedMessage = TempData["FailedMessage"];
                ViewBag.AlertMessage = TempData["AlertMessage"];

                return View(model);
            }

            return RedirectToAction("Index");
        }




        #endregion

        #region Delete
        [HttpPost]
        public ActionResult Delete(int Id, string CancelReason, bool IsPopup = false)
        {
            var receipt = ReceiptRepository.GetReceiptById(Id);
            if (receipt != null)
            {
                receipt.IsDeleted = true;
                receipt.IsArchive = false;
                receipt.CancelReason = CancelReason;
                receipt.ModifiedUserId = WebSecurity.CurrentUserId;
                receipt.ModifiedDate = DateTime.Now;
                ReceiptRepository.UpdateReceipt(receipt);
                //var receiptDetail = ReceiptDetailRepository.GetReceiptDetailByReceiptId(Id);
                //receiptDetail.IsDeleted = true;
                //receiptDetail.ModifiedUserId = WebSecurity.CurrentUserId;
                //receiptDetail.ModifiedDate = DateTime.Now;
                //ReceiptDetailRepository.UpdateReceiptDetail(receiptDetail);
            }

            TempData[Globals.SuccessMessageKey] = "Đã hủy chứng từ";

            if (IsPopup)
            {
                return RedirectToAction("Detail", new { Id = receipt.Id, IsPopup = IsPopup });
            }
            return RedirectToAction("Detail", new { Id = receipt.Id });
        }




        #endregion

        #region Print
        public ActionResult Print(int Id)
        {
            var model = new TemplatePrintViewModel();
            //lấy logo công ty
            //var logo = Erp.BackOffice.Helpers.Common.GetSetting("LogoCompany");
            var company = Erp.BackOffice.Helpers.Common.GetSetting("companyName");
            var address = Erp.BackOffice.Helpers.Common.GetSetting("addresscompany");
            var phone = Erp.BackOffice.Helpers.Common.GetSetting("phonecompany");
            var fax = Erp.BackOffice.Helpers.Common.GetSetting("faxcompany");
            var bankcode = Erp.BackOffice.Helpers.Common.GetSetting("bankcode");
            var bankname = Erp.BackOffice.Helpers.Common.GetSetting("bankname");
            //var ImgLogo = "<div class=\"logo\"><img src=" + logo + " height=\"73\" /></div>";
            //lấy phiếu chi.
            var receipt = ReceiptRepository.GetvwReceiptById(Id);

            //lấy template phiếu xuất.
            var template = templatePrintRepository.GetAllTemplatePrint().Where(x => x.Code.Contains("Receipt")).OrderByDescending(x => x.CreatedDate).FirstOrDefault();
            //truyền dữ liệu vào template.
            model.Content = template.Content;
            model.Content = model.Content.Replace("{Code}", receipt.Code);
            model.Content = model.Content.Replace("{Company}", receipt.CompanyName);
            model.Content = model.Content.Replace("{Customer}", receipt.Payer);
            model.Content = model.Content.Replace("{Address}", receipt.Address);
            model.Content = model.Content.Replace("{Reason}", receipt.Name);
            model.Content = model.Content.Replace("{Money}", Erp.BackOffice.Helpers.CommonSatic.ToCurrencyStr(receipt.Amount, null));
            model.Content = model.Content.Replace("{VoucherDate}", receipt.VoucherDate.Value.ToShortDateString());

            model.Content = model.Content.Replace("{CreatedDate}", receipt.CreatedDate.Value.ToString("dd/MM/yyyy HH:mm"));
            model.Content = model.Content.Replace("{SalerName}", receipt.SalerName);
            model.Content = model.Content.Replace("{MoneyText}", Erp.BackOffice.Helpers.Common.ChuyenSoThanhChu(receipt.Amount.ToString()));
            model.Content = model.Content.Replace("{Note}", receipt.Note);

            model.Content = model.Content.Replace("{System.CompanyName}", company);
            model.Content = model.Content.Replace("{System.AddressCompany}", address);
            model.Content = model.Content.Replace("{System.PhoneCompany}", phone);
            model.Content = model.Content.Replace("{System.Fax}", fax);
            model.Content = model.Content.Replace("{System.BankCodeCompany}", bankcode);
            model.Content = model.Content.Replace("{System.BankNameCompany}", bankname);
            return View(model);
        }



        #endregion


        public ActionResult PrintReceipt(string startDate, string endDate, string start, string end, int? CustomerId, int? SalerId, string Code, string MaChungTuGoc, string CustomerInfo, string tienchuanop, string tiendanop, bool ExportExcel = false)
        {
            var data = IndexPrint(startDate, endDate, start, end, CustomerId, SalerId, Code, MaChungTuGoc, CustomerInfo);

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
            model.Content = model.Content.Replace("{DataTable}", buildHtmlDanhsachphieuthu(data, tienchuanop, tiendanop));
            model.Content = model.Content.Replace("{System.Logo}", ImgLogo);
            model.Content = model.Content.Replace("{System.CompanyName}", company);
            model.Content = model.Content.Replace("{System.AddressCompany}", address);
            model.Content = model.Content.Replace("{System.PhoneCompany}", phone);
            model.Content = model.Content.Replace("{System.Fax}", fax);
            model.Content = model.Content.Replace("{System.BankCodeCompany}", bankcode);
            model.Content = model.Content.Replace("{System.BankNameCompany}", bankname);
            model.Content = model.Content.Replace("{PrintDate}", DateTime.Now.ToString("dd/MM/yyyy HH:mm"));
            model.Content = model.Content.Replace("{Title}", "Danh sách phiếu thu");
            if (ExportExcel)
            {
                Response.AppendHeader("content-disposition", "attachment;filename=" + "DS_Phieuthu" + DateTime.Now.ToString("yyyyMMdd") + ".xls");
                Response.Charset = "";
                Response.Cache.SetCacheability(HttpCacheability.NoCache);
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.Write(model.Content);
                Response.End();
            }




            return View(model);
        }


        string buildHtmlDanhsachphieuthu(List<ReceiptViewModel> detailList, string tienchuanop, string tiendanop)
        {
            string detailLists = "<table border=\"1\" class=\"invoice-detail\">\r\n";
            detailLists += "<thead>\r\n";
            detailLists += "	<tr>";
            detailLists += "		<th>STT</th>";
            detailLists += "		<th>Ngày ghi sổ</th>";
            detailLists += "		<th>Ngày chứng từ</th>";
            detailLists += "		<th>Mã chứng từ</th>";
            detailLists += "		<th>Chứng từ gốc</th>";
            detailLists += "		<th>Tên Khách hàng</th>";
            detailLists += "		<th>Mã khách hàng</th>";
            detailLists += "		<th>Số tiền</th>";
            detailLists += "		<th>HTTT</th>";
            detailLists += "		<th>Nhân viên QL KH</th>";
            detailLists += "		<th>Lý do</th>";
            detailLists += "		<th>Ghi chú</th>";

            detailLists += "		<th>Trạng thái ghi sổ</th>";
            detailLists += "	</tr>";
            detailLists += "</thead>\r\n";
            detailLists += "<tbody>\r\n";
            var index = 1;

            foreach (var item in detailList)
            {
                detailLists += "<tr>\r\n"
                + "<td class=\"text-center orderNo\">" + (index++) + "</td>\r\n"
                + "<td>" + (item.ModifiedDate.HasValue ? item.ModifiedDate.Value.ToString("dd/MM/yyyy HH:mm") : "") + "</td>"
                + "<td>" + (item.CreatedDate.HasValue ? item.CreatedDate.Value.ToString("dd/MM/yyyy HH:mm") : "") + "</td>"
                + "<td class=\"text-left \">" + item.Code + "</td>\r\n"
                + "<td class=\"text-left \">" + item.MaChungTuGoc + "</td>\r\n"
                + "<td class=\"text-left \">" + item.CompanyName + "</td>\r\n"
                + "<td class=\"text-left \">" + item.CustomerCode + "</td>\r\n"
                 + "<td class=\"text-left \">" + CommonSatic.ToCurrencyStr(item.Amount, null).Replace(".", ",") + "</td>\r\n"
                + "<td class=\"text-left \">" + item.PaymentMethod + "</td>\r\n"
                + "<td class=\"text-left \">" + item.NVQL + "</td>\r\n"
                + "<td class=\"text-left \">" + item.Name + "</td>\r\n"
                + "<td class=\"text-left \">" + item.Note + "</td>\r\n"

                + "<td class=\"text-left \">" + (item.IsArchive == true ? "Đã ghi sổ" : "Chưa ghi sổ") + "</td>\r\n"
                + "</tr>\r\n";
            }

            detailLists += "<tr>\r\n"
               + "<td>Tổng cộng</td>"
               + "<td>Tiền chưa nộp</td>"
               + "<td class=\"text-left \">" + tienchuanop + "</td>\r\n"
               + "<td class=\"text-left \">" + "" + "</td>\r\n"
               + "<td class=\"text-left \">" + "" + "</td>\r\n"
               + "<td class=\"text-left \">" + "Tiền đã nộp" + "</td>\r\n"
               + "<td class=\"text-left \">" + tiendanop + "</td>\r\n"
               + "</tr>\r\n";


            detailLists += "</tbody>\r\n";
            detailLists += "<tfoot style=\"font-weight:bold\">\r\n";
            detailLists += "</tfoot>\r\n</table>\r\n";
            return detailLists;
        }

        #region Archive
        [HttpPost]
        public ActionResult Archive(int Id)
        {
            var receipt = ReceiptRepository.GetReceiptById(Id);
            if (receipt != null)
            {
                receipt.IsArchive = true;
                receipt.ModifiedUserId = WebSecurity.CurrentUserId;
                receipt.ModifiedDate = DateTime.Now;
                ReceiptRepository.UpdateReceipt(receipt);
                if (receipt.LogReceiptId != null && receipt.LogReceiptId.Value > 0)
                {
                    var data = ReceiptRepository.GetAllReceipt().Where(x => x.LogReceiptId != null && x.LogReceiptId.Value == receipt.LogReceiptId && x.IsArchive == false).ToList();
                    var ins = logReceiptRepository.GetLogReceiptById(receipt.LogReceiptId.Value);
                    if (ins != null)
                    {
                        if (!data.Any())
                        {
                            ins.Status = "complete";
                        }
                        else
                        {
                            ins.Status = "inprogress";
                        }
                        ins.ModifiedDate = DateTime.Now;
                        ins.ModifiedUserId = WebSecurity.CurrentUserId;
                        logReceiptRepository.UpdateLogReceipt(ins);
                    }
                }
                TempData[Globals.SuccessMessageKey] = "Xét duyệt thành công!";
            }
            else
            {
                TempData[Globals.FailedMessageKey] = "Không tìm thấy phiếu thu";
            }
            return View(receipt);
        }

        [HttpPost]
        public JsonResult ArchiveAll(List<int> ListId)
        {
            foreach (var i in ListId)
            {
                var receipt = ReceiptRepository.GetReceiptById(i);
                if (receipt != null && receipt.IsArchive == false)
                {
                    receipt.IsArchive = true;
                    receipt.ModifiedUserId = WebSecurity.CurrentUserId;
                    receipt.ModifiedDate = DateTime.Now;
                    ReceiptRepository.UpdateReceipt(receipt);
                    if (receipt.LogReceiptId != null && receipt.LogReceiptId.Value > 0)
                    {
                        var data = ReceiptRepository.GetAllReceipt().Where(x => x.LogReceiptId != null && x.LogReceiptId.Value == receipt.LogReceiptId && x.IsArchive == false).ToList();
                        var ins = logReceiptRepository.GetLogReceiptById(receipt.LogReceiptId.Value);
                        if (ins != null)
                        {
                            if (!data.Any())
                            {
                                ins.Status = "complete";
                            }
                            else
                            {
                                ins.Status = "inprogress";
                            }
                            ins.ModifiedDate = DateTime.Now;
                            ins.ModifiedUserId = WebSecurity.CurrentUserId;
                            logReceiptRepository.UpdateLogReceipt(ins);
                        }
                    }
                    TempData[Globals.SuccessMessageKey] = "Xét duyệt thành công!";

                }
                else
                {
                    TempData[Globals.FailedMessageKey] = "Không tìm thấy phiếu thu";
                }
            }
            return Json(1);
        }

        #endregion

        #region Xoa phieu thu
        [HttpPost]
        public ActionResult DeleteReceipt(int? Id)
        {
            var Rpt = ReceiptRepository.GetReceiptById(Id.Value);
            if(Rpt != null)
            {

            }
            return View();
        }
        #endregion
    }
}
