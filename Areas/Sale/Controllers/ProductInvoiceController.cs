using System.Globalization;
using Erp.BackOffice.Sale.Models;
using Erp.BackOffice.Account.Models;
using Erp.BackOffice.Filters;
using Erp.Domain.Sale.Entities;
using Erp.Domain.Interfaces;
using Erp.Domain.Sale.Interfaces;
using Erp.Domain.Sale.Repositories;
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
using Erp.BackOffice.App_GlobalResources;
using Erp.Domain.Staff.Interfaces;
using Erp.BackOffice.Account.Controllers;
using Erp.Domain.Account.Entities;
using System.Xml.Linq;
using Erp.BackOffice.Areas.Cms.Models;
using Erp.Domain.Entities;
using Newtonsoft.Json;
using GenCode128;
using System.Drawing;
using System.IO;
using Erp.Domain.Crm.Interfaces;
using Erp.Domain.Staff.Entities;
using System.Web;
using Erp.Domain.Repositories;
using Erp.Domain;
using Erp.Domain.Sale;
using System.Web.Script.Serialization;
using System.Data.Entity;
using System.Transactions;
using Erp.Domain.Account.Helper;
using iTextSharp.text.pdf.qrcode;

namespace Erp.BackOffice.Sale.Controllers
{
    [Authorize]
    [InitializeSimpleMembership]
    [Erp.BackOffice.Helpers.NoCacheHelper]
    public class ProductInvoiceController : Controller
    {
        private readonly IMembership_parentRepository Membership_parentRepository;
        private readonly ITransactionRepository transactionRepository;
        private readonly IReceiptRepository ReceiptRepository;
        private readonly IProductInvoiceRepository productInvoiceRepository;
        private readonly ICommisionRepository commisionRepository;
        private readonly IProductOrServiceRepository ProductRepository;
        private readonly ICustomerRepository customerRepository;
        private readonly IInventoryRepository inventoryRepository;
        private readonly IProductOutboundRepository productOutboundRepository;
        private readonly IStaffsRepository staffRepository;
        private readonly IUserRepository userRepository;
        private readonly ITemplatePrintRepository templatePrintRepository;
        private readonly ICategoryRepository categoryRepository;
        private readonly IWarehouseLocationItemRepository warehouseLocationItemRepository;
        private readonly ICommisionStaffRepository commisionStaffRepository;
        private readonly IServiceComboRepository servicecomboRepository;
        private readonly IUsingServiceLogRepository usingServiceLogRepository;
        private readonly ILogServiceRemminderRepository logServiceReminderRepository;
        private readonly IServiceReminderGroupRepository ServiceReminderGroupRepository;
        private readonly ITransactionLiabilitiesRepository transactionLiabilitiesRepository;
        private readonly IWarehouseRepository warehouseRepository;
        private readonly IUsingServiceLogDetailRepository usingServiceLogDetailRepository;
        private readonly ICommisionCustomerRepository commisionCustomerRepository;
        private readonly IServiceScheduleRepository serviceScheduleRepository;
        private readonly ITaskRepository taskRepository;
        private readonly IWorkSchedulesRepository WorkSchedulesRepository;
        private readonly IRegisterForOvertimeRepository RegisterForOvertimeRepository;
        private readonly IReceiptDetailRepository receiptDetailRepository;
        private readonly ICommissionCusRepository commissionCusRepository;
        private readonly IDonateProOrSerRepository donateProductOrServiceRepository;
        private readonly ICommisionInvoiceRepository commissionInvoiceRepository;
        private readonly ILogPromotionRepository logPromotionRepository;
        private readonly IMembershipRepository membershipRepository;
        public ProductInvoiceController(

            ITransactionRepository _transaction
            , IReceiptRepository _Receipt
            , IProductInvoiceRepository _ProductInvoice
            , ICommisionRepository _Commision
            , IProductOrServiceRepository _Product
            , ICustomerRepository _Customer
            , IInventoryRepository _Inventory
            , IProductOutboundRepository _ProductOutbound
            , IStaffsRepository _staff
            , IUserRepository _user
            , ITemplatePrintRepository _templatePrint
            , ICategoryRepository category
            , IWarehouseLocationItemRepository _WarehouseLocationItem
            , ICommisionStaffRepository _commisionStaff
            , IServiceComboRepository servicecombo
            , IUsingServiceLogRepository usingServiceLog
            , ILogServiceRemminderRepository logServiceReminder
            , IServiceReminderGroupRepository serviceReminderGroup
            , ITransactionLiabilitiesRepository _transactionLiabilities
            , IWarehouseRepository _Warehouse
            , IUsingServiceLogDetailRepository usingServiceLogDetail
            , ICommisionCustomerRepository _Commision_Customer
            , IServiceScheduleRepository schedule
            , ITaskRepository task
            , IWorkSchedulesRepository _WorkSchedules
            , IRegisterForOvertimeRepository _RegisterForOvertime
            , IReceiptDetailRepository receiptDetail
            , ICommissionCusRepository commissionCus
            , IDonateProOrSerRepository donateProductOrService
            , ICommisionInvoiceRepository commssionInvoice
            , ILogPromotionRepository logPromotion
            , IMembershipRepository membership
             , IMembership_parentRepository _Membership_parent
            )
        {
            transactionRepository = _transaction;
            ReceiptRepository = _Receipt;
            productInvoiceRepository = _ProductInvoice;
            commisionRepository = _Commision;
            ProductRepository = _Product;
            inventoryRepository = _Inventory;
            productOutboundRepository = _ProductOutbound;
            customerRepository = _Customer;
            staffRepository = _staff;
            userRepository = _user;
            templatePrintRepository = _templatePrint;
            categoryRepository = category;
            warehouseLocationItemRepository = _WarehouseLocationItem;
            commisionStaffRepository = _commisionStaff;
            usingServiceLogRepository = usingServiceLog;
            servicecomboRepository = servicecombo;
            logServiceReminderRepository = logServiceReminder;
            ServiceReminderGroupRepository = serviceReminderGroup;
            transactionLiabilitiesRepository = _transactionLiabilities;
            warehouseRepository = _Warehouse;
            usingServiceLogDetailRepository = usingServiceLogDetail;
            commisionCustomerRepository = _Commision_Customer;
            serviceScheduleRepository = schedule;
            taskRepository = task;
            WorkSchedulesRepository = _WorkSchedules;
            RegisterForOvertimeRepository = _RegisterForOvertime;
            receiptDetailRepository = receiptDetail;
            commissionCusRepository = commissionCus;
            donateProductOrServiceRepository = donateProductOrService;
            commissionInvoiceRepository = commssionInvoice;
            logPromotionRepository = logPromotion;
            membershipRepository = membership;
            Membership_parentRepository = _Membership_parent;
        }

        #region Index
        public ViewResult Index(int? saleChanel, string txtCode, string txtMinAmount, string txtMaxAmount, string txtCusName, string startDate, string endDate, int? BranchId, string Status, string txtCustomerCode, string txtCusInfo, string txtProductName, int? SalerId, string IS_ONLINE)
        {
            txtCusInfo = txtCusInfo != null ? txtCusInfo : "";
            SalerId = SalerId != null ? SalerId : 0;
            txtCusInfo = txtCusInfo != null ? txtCusInfo : "";
            txtCode = txtCode != null ? txtCode : "";


            if (startDate == null && endDate == null)
            {

                DateTime aDateTime = DateTime.Now;
                // Cộng thêm 1 tháng và trừ đi một ngày.
                DateTime retDateTime = DateTime.Now;



                startDate = aDateTime.ToString("dd/MM/yyyy");
                endDate = retDateTime.ToString("dd/MM/yyyy");
            }

            DateTime d_startDate, d_endDate;
            if (DateTime.TryParseExact(startDate, "dd/MM/yyyy", new CultureInfo("vi-VN"), DateTimeStyles.None, out d_startDate))
            {

            }
            if (DateTime.TryParseExact(endDate, "dd/MM/yyyy", new CultureInfo("vi-VN"), DateTimeStyles.None, out d_endDate))
            {
                d_endDate = d_endDate.AddHours(23).AddMinutes(59);
            }

            if ((d_startDate == null) || (d_endDate == null))
            {
                return View();
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

            var model = SqlHelper.QuerySP<ProductInvoiceViewModel>("getProductInvoice", new
            {

                StartDate = d_startDate,
                EndDate = d_endDate,
                BranchId = intBrandID,
                txtCusInfo = txtCusInfo,
                ManagerStaffId = SalerId,
                txtCode = txtCode

            }).OrderByDescending(m => m.CreatedDate).ToList();

           


            if (saleChanel > 0)
            {
                if (saleChanel == 1)
                {
                    model = model.Where(x => x.SaleChanel == saleChanel || x.SaleChanel == null).ToList();
                }
                else
                {
                    model = model.Where(x => x.SaleChanel == saleChanel).ToList();
                }
            }

            if (!string.IsNullOrEmpty(Status))
            {
                if (Status == "delete")
                {
                    model = model.Where(x => x.Status == "delete").ToList();
                }
                if (Status == "complete")
                {
                    model = model.Where(x => x.Status == "complete").ToList();
                }
                if (Status == "pending")
                {
                    model = model.Where(x => x.Status == "pending").ToList();
                }
                if (Status == "Đặt cọc")
                {
                    model = model.Where(x => x.Status == "Đặt cọc").ToList();
                }

                if (Status == "Đặt cọc và hoàn thành")
                {
                    model = model.Where(x => x.Status == "Đặt cọc" || x.Status == "complete").ToList();
                }
            }

            //if ((SalerId != null) && (SalerId > 0))
            //{
            //    model = model.Where(x => x.ManagerStaffId == SalerId).ToList();
            //}
            if (IS_ONLINE == "on")
            {
                model = model.Where(x => x.IS_ONLINE == true).ToList();
            }


            decimal minAmount;
            if (decimal.TryParse(txtMinAmount, out minAmount))
            {
                model = model.Where(x => x.TotalAmount >= minAmount).ToList();
            }

            decimal maxAmount;
            if (decimal.TryParse(txtMaxAmount, out maxAmount))
            {
                if (maxAmount > 0)
                {
                    model = model.Where(x => x.TotalAmount <= maxAmount).ToList();
                }
            }
            //if (BranchId != null && BranchId.Value > 0)
            //{
            //    model = model.Where(x => x.BranchId == BranchId).ToList();
            //}

            if (!string.IsNullOrEmpty(txtProductName))
            {
                txtProductName = Helpers.Common.ChuyenThanhKhongDau(txtProductName);
                var productListId = ProductRepository.GetAllvwProductAndService()
                    .Where(item => item.Code == txtProductName || item.Name.Contains(txtProductName)).Select(item => item.Id).ToList();

                if (productListId.Count > 0)
                {
                    List<int> listProductInboundId = new List<int>();
                    foreach (var id in productListId)
                    {
                        var list = productInvoiceRepository.GetAllvwInvoiceDetails().Where(x => x.ProductName.Contains(txtProductName) || x.ProductCode == txtProductName)
                            .Select(item => item.ProductInvoiceId.Value).Distinct().ToList();

                        listProductInboundId.AddRange(list);
                    }

                    model = model.Where(item => listProductInboundId.Contains(item.Id)).ToList();
                }
            }

            //model = model.OrderByDescending(c => c.CreatedDate).ToList();

            ViewBag.tongtien = model.Sum(x => x.TotalAmount);
            //ViewBag.tienconno = model.Sum(x => x.TongConNo);
            ViewBag.tienconno = model.Sum(x => x.tienconno);
            //ViewBag.tongthu = model.Sum(x => x.TotalDebit);
            ViewBag.tongthu = model.Sum(x => x.TotalDebit);
            //ViewBag.dathanhtoan = model.Sum(x => x.TotalCredit);
            ViewBag.dathanhtoan = model.Sum(x => x.tiendathu);
            ViewBag.SuccessMessage = TempData["SuccessMessage"];
            ViewBag.FailedMessage = TempData["FailedMessage"];
            ViewBag.AlertMessage = TempData["AlertMessage"];

            return View(model);
        }


        public ViewResult IndexSearch(string txtCode, string txtMinAmount, string txtMaxAmount, string txtCusName, string startDate, string endDate, int? BranchId, string Status, string txtCustomerCode)
        {
            if (Request["checktrang"] == "Receipt")
            {
                Status = "Đặt cọc";
            }
            else
            {
                if (Status == null)
                {
                    Status = "complete";
                }
            }

            if (startDate == null && endDate == null)
            {
                DateTime aDateTime = new DateTime(DateTime.Now.Year, 1, 1);
                // Cộng thêm 1 tháng và trừ đi một ngày.
                DateTime retDateTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(1).AddDays(-1);
                startDate = aDateTime.ToString("dd/MM/yyyy");
                endDate = retDateTime.ToString("dd/MM/yyyy");
            }

            DateTime d_startDate, d_endDate;
            if (DateTime.TryParseExact(startDate, "dd/MM/yyyy", new CultureInfo("vi-VN"), DateTimeStyles.None, out d_startDate))
            {

            }
            if (DateTime.TryParseExact(endDate, "dd/MM/yyyy", new CultureInfo("vi-VN"), DateTimeStyles.None, out d_endDate))
            {
                d_endDate = d_endDate.AddHours(23).AddMinutes(59).AddSeconds(59);
            }

            if ((d_startDate == null) || (d_endDate == null))
            {
                return View();
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

            //var user = userRepository.GetByvwUserName(Helpers.Common.CurrentUser.UserName);
            /// var q = productInvoiceRepository.GetAllvwProductInvoiceFull().Where(x => x.CreatedDate >= d_startDate && x.CreatedDate <= d_endDate && x.Status == Status && x.BranchId == intBrandID);
            //.Where(x => x.BranchId == Helpers.Common.CurrentUser.BranchId);
            //var model = q.Select(item => new ProductInvoiceViewModel
            //{
            //    Id = item.Id,
            //    IsDeleted = item.IsDeleted,
            //    CreatedUserId = item.CreatedUserId,
            //    CreatedDate = item.CreatedDate,
            //    ModifiedUserId = item.ModifiedUserId,
            //    ModifiedDate = item.ModifiedDate,
            //    Code = item.Code,
            //    CustomerCode = item.CustomerCode,
            //    CustomerName = item.CustomerName,
            //    ShipCityName = item.ShipCityName,
            //    TotalAmount = item.TotalAmount,
            //    //FixedDiscount = item.FixedDiscount,
            //    TaxFee = item.TaxFee,
            //    CodeInvoiceRed = item.CodeInvoiceRed,
            //    Status = item.Status,
            //    IsArchive = item.IsArchive,
            //    //ProductOutboundId = item.ProductOutboundId,
            //    //ProductOutboundCode = item.ProductOutboundCode,
            //    Note = item.Note,
            //    CancelReason = item.CancelReason,
            //    BranchId = item.BranchId,
            //    CustomerId = item.CustomerId,
            //    BranchName = item.BranchName,
            //    ManagerStaffId = item.ManagerStaffId,
            //    ManagerStaffName = item.ManagerStaffName,
            //    ManagerUserName = item.ManagerUserName,
            //    RemainingAmount = item.RemainingAmount,
            //    //
            //    UserTypeName = item.UserTypeName,
            //    tienconno = item.tienconno

            //}).OrderByDescending(m => m.Id).ToList();
            if (string.IsNullOrEmpty(txtCode))
            {
                txtCode = "";
            }

            var model = SqlHelper.QuerySP<ProductInvoiceViewModel>("getProductInvoiceForSearch", new
            {
                @StartDate = d_startDate,
                @EndDate = d_endDate,
                @BranchId = intBrandID,
                @Code = txtCode

            }).ToList();

            //if (!string.IsNullOrEmpty(txtCode))
            //{
            //    model = model.Where(x => x.Code.ToLowerOrEmpty().Contains(txtCode.Trim().ToLower())
            //        //|| x.WarehouseSourceId == warehouseSourceId
            //        ).ToList();
            //}

            if (!string.IsNullOrEmpty(txtCusName))
            {
                txtCusName = Helpers.Common.ChuyenThanhKhongDau(txtCusName);
                model = model.Where(x => Helpers.Common.ChuyenThanhKhongDau(x.CustomerName).Contains(txtCusName)
                || x.CustomerCode.ToLowerOrEmpty().Contains(txtCusName.Trim().ToLower())).ToList();
            }

            //if (intBrandID > 0)
            //{
            //    model = model.Where(x => x.BranchId == intBrandID).ToList();
            //}

            if (!string.IsNullOrEmpty(Status))
            {
                if (Status == "delete")
                {
                    model = model.Where(x => x.IsDeleted == true).ToList();
                }
                if (Status == "complete")
                {
                    model = model.Where(x => x.Status == "complete").ToList();
                }
                if (Status == "pending")
                {
                    model = model.Where(x => x.Status == "pending").ToList();
                }
                if (Status == "Đặt cọc" || Status == "dc")
                {
                    model = model.Where(x => x.Status == "Đặt cọc").ToList();
                    //var productListId = ProductRepository.GetAllvwProductAndService()
                    //    .Where(item => item.Code == "ĐC").Select(item => item.Id).ToList();

                    //if (productListId.Count > 0)
                    //{
                    //    List<int> listProductInboundId = new List<int>();
                    //    foreach (var id in productListId)
                    //    {
                    //        var list = productInvoiceRepository.GetAllvwInvoiceDetails().Where(x => x.ProductCode == "ĐC")
                    //            .Select(item => item.ProductInvoiceId.Value).Distinct().ToList();

                    //        listProductInboundId.AddRange(list);
                    //    }

                    //    model = model.Where(item => listProductInboundId.Contains(item.Id)).ToList();
                    //}
                }
            }

            decimal minAmount;
            if (decimal.TryParse(txtMinAmount, out minAmount))
            {
                model = model.Where(x => x.TotalAmount >= minAmount).ToList();
            }

            decimal maxAmount;
            if (decimal.TryParse(txtMaxAmount, out maxAmount))
            {
                if (maxAmount > 0)
                {
                    model = model.Where(x => x.TotalAmount <= maxAmount).ToList();
                }
            }
            //if (BranchId != null && BranchId.Value > 0)
            //{
            //    model = model.Where(x => x.BranchId == BranchId).ToList();
            //}
            //{IsPopup=true&module_list=Payment&jsCallback=selectItemCustomer&TargetId=1152}
            if (Request["IsPopup"] == "true" || Request["IsPopup"] == "True")
            {
                if (Request["module_list"] == "Payment" && Request["TargetId"] != "")
                {
                    var TargetId = Request["TargetId"];
                    model = model.Where(x => x.CustomerId == Int32.Parse(TargetId)).ToList();
                }

                if (Request["module_list"] == "Receipt" && Request["TargetId"] != "")
                {
                    var TargetId = Request["TargetId"];
                    model = model.Where(x => x.CustomerId == Int32.Parse(TargetId)).ToList();
                }
            }

            //if (!string.IsNullOrEmpty(txtCustomerCode))
            //{
            //    txtCustomerCode = Helpers.Common.ChuyenThanhKhongDau(txtCustomerCode);
            //    model = model.Where(x => Helpers.Common.ChuyenThanhKhongDau(x.CustomerCode).Contains(txtCustomerCode)).ToList();
            //}

            ViewBag.SuccessMessage = TempData["SuccessMessage"];
            ViewBag.FailedMessage = TempData["FailedMessage"];
            ViewBag.AlertMessage = TempData["AlertMessage"];

            return View(model);
        }

        public ViewResult IndexSearch_return(string txtCode, string txtMinAmount, string txtMaxAmount, string txtCusName, string startDate, string endDate, int? BranchId, string Status, string txtCustomerCode, string MainCode)
        {

            if (string.IsNullOrEmpty(MainCode))
            {
                MainCode = "";
            }
            if (startDate == null && endDate == null)
            {
                DateTime aDateTime = new DateTime(DateTime.Now.Year, 1, 1);
                // Cộng thêm 1 tháng và trừ đi một ngày.
                DateTime retDateTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(1).AddDays(-1);

                startDate = aDateTime.ToString("dd/MM/yyyy");
                endDate = retDateTime.ToString("dd/MM/yyyy");
            }

            DateTime d_startDate, d_endDate;
            if (DateTime.TryParseExact(startDate, "dd/MM/yyyy", new CultureInfo("vi-VN"), DateTimeStyles.None, out d_startDate))
            {

            }
            if (DateTime.TryParseExact(endDate, "dd/MM/yyyy", new CultureInfo("vi-VN"), DateTimeStyles.None, out d_endDate))
            {
                d_endDate = d_endDate.AddHours(23).AddMinutes(59).AddSeconds(59);
            }

            if ((d_startDate == null) || (d_endDate == null))
            {
                return View();
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

            //var user = userRepository.GetByvwUserName(Helpers.Common.CurrentUser.UserName);
            var model = SqlHelper.QuerySP<ProductInvoiceViewModel>("spSale_AllProductInVoiceWithOutMoneyMove", new
            {
                StartDate = d_startDate,
                EndDate = d_endDate,
                ProductInvoiceId = MainCode,
                pCustomerCode= txtCustomerCode,
                BranchId = intBrandID
            }).OrderByDescending(m => m.Id).ToList();

            /*productInvoiceRepository.GetAllvwProductInvoiceFull().Where(x => x.CreatedDate >= d_startDate && x.CreatedDate <= d_endDate);*/

            //.Where(x => x.BranchId == Helpers.Common.CurrentUser.BranchId);
            //var model = q.Select(item => new ProductInvoiceViewModel
            //{
            //    Id = item.Id,
            //    IsDeleted = item.IsDeleted,
            //    CreatedUserId = item.CreatedUserId,
            //    CreatedDate = item.CreatedDate,
            //    ModifiedUserId = item.ModifiedUserId,
            //    ModifiedDate = item.ModifiedDate,
            //    Code = item.Code,
            //    CustomerCode = item.CustomerCode,
            //    CustomerName = item.CustomerName,
            //    ShipCityName = item.ShipCityName,
            //    TotalAmount = item.TotalAmount,
            //    //FixedDiscount = item.FixedDiscount,
            //    TaxFee = item.TaxFee,
            //    CodeInvoiceRed = item.CodeInvoiceRed,
            //    Status = item.Status,
            //    IsArchive = item.IsArchive,
            //    //ProductOutboundId = item.ProductOutboundId,
            //    //ProductOutboundCode = item.ProductOutboundCode,
            //    Note = item.Note,
            //    CancelReason = item.CancelReason,
            //    BranchId = item.BranchId,
            //    CustomerId = item.CustomerId,
            //    BranchName = item.BranchName,
            //    ManagerStaffId = item.ManagerStaffId,
            //    ManagerStaffName = item.ManagerStaffName,
            //    ManagerUserName = item.ManagerUserName,
            //    //
            //    UserTypeName = item.UserTypeName,
            //    tiendathu = item.tiendathu

            //}).OrderByDescending(m => m.Id).ToList();


            if (!string.IsNullOrEmpty(txtCode))
            {
                model = model.Where(x => x.Code.ToLowerOrEmpty().Contains(txtCode.Trim().ToLower())
                    //|| x.WarehouseSourceId == warehouseSourceId
                    ).ToList();
            }

            if (!string.IsNullOrEmpty(txtCusName))
            {
                txtCusName = Helpers.Common.ChuyenThanhKhongDau(txtCusName);
                model = model.Where(x => Helpers.Common.ChuyenThanhKhongDau(x.CustomerName).Contains(txtCusName)).ToList();
            }

            //if (Helpers.Common.CurrentUser.BranchId != null)
            //{
            //    model = model.Where(x => x.BranchId == Helpers.Common.CurrentUser.BranchId.Value).ToList();
            //}

            if (!string.IsNullOrEmpty(Status))
            {
                if (Status == "delete")
                {
                    model = model.Where(x => x.IsDeleted == true).ToList();
                }
                if (Status == "complete")
                {
                    model = model.Where(x => x.Status == "complete").ToList();
                }
                if (Status == "pending")
                {
                    model = model.Where(x => x.Status == "pending").ToList();
                }
                if (Status == "dc")
                {
                    var productListId = ProductRepository.GetAllvwProductAndService()
                        .Where(item => item.Code == "ĐC").Select(item => item.Id).ToList();

                    if (productListId.Count > 0)
                    {
                        List<int> listProductInboundId = new List<int>();
                        foreach (var id in productListId)
                        {
                            var list = productInvoiceRepository.GetAllvwInvoiceDetails().Where(x => x.ProductCode == "ĐC")
                                .Select(item => item.ProductInvoiceId.Value).Distinct().ToList();

                            listProductInboundId.AddRange(list);
                        }

                        model = model.Where(item => listProductInboundId.Contains(item.Id)).ToList();
                    }
                }
            }


            decimal minAmount;
            if (decimal.TryParse(txtMinAmount, out minAmount))
            {
                model = model.Where(x => x.TotalAmount >= minAmount).ToList();
            }

            decimal maxAmount;
            if (decimal.TryParse(txtMaxAmount, out maxAmount))
            {
                if (maxAmount > 0)
                {
                    model = model.Where(x => x.TotalAmount <= maxAmount).ToList();
                }
            }
            //if (BranchId != null && BranchId.Value > 0)
            //{
            //    model = model.Where(x => x.BranchId == BranchId).ToList();
            //}
            //{IsPopup=true&module_list=Payment&jsCallback=selectItemCustomer&TargetId=1152}
            if (Request["IsPopup"] == "true" || Request["IsPopup"] == "True")
            {
                if (Request["module_list"] == "Payment" && Request["TargetId"] != "")
                {
                    var TargetId = Request["TargetId"];
                    model = model.Where(x => x.CustomerId == Int32.Parse(TargetId)).ToList();
                }

                if (Request["module_list"] == "Receipt" && Request["TargetId"] != "")
                {
                    var TargetId = Request["TargetId"];
                    model = model.Where(x => x.CustomerId == Int32.Parse(TargetId)).ToList();
                }
            }

            //if (!string.IsNullOrEmpty(txtCustomerCode))
            //{
            //    txtCustomerCode = Helpers.Common.ChuyenThanhKhongDau(txtCustomerCode);
            //    model = model.Where(x => Helpers.Common.ChuyenThanhKhongDau(x.CustomerCode).Contains(txtCustomerCode)).ToList();
            //}

            //foreach (var item in model)
            //{
            //    if (item.Status.Contains("Đặt cọc"))
            //    {
            //        if (item.PaidAmount != null && item.PaidAmount > 0)
            //        {
            //            item.SaleReturnTotalAmount = item.PaidAmount;
            //        }
            //        else if (item.tiendathu != null && item.tiendathu > 0)
            //        {
            //            item.SaleReturnTotalAmount = item.tiendathu;
            //        }
            //        else
            //        {
            //            item.SaleReturnTotalAmount = item.SaleReturnTotalAmount;
            //        }

            //    }
            //}

            ViewBag.SuccessMessage = TempData["SuccessMessage"];
            ViewBag.FailedMessage = TempData["FailedMessage"];
            ViewBag.AlertMessage = TempData["AlertMessage"];
            if (model != null && model.Count > 0)
                foreach (var item in model)
                {
                    if (Helpers.Common.NVL_NUM_LONG_NEW(item.Tiencuadonhangtra) > 0)
                    {
                        item.SaleReturnTotalAmount = item.Tiencuadonhangtra;
                    }else
                    {
                        item.SaleReturnTotalAmount = item.tiendathu;
                    }
                }

            return View(model);
        }
        #endregion

        #region Print
        //--------------------------------------------------------------------------------------------------------//
        public List<ProductInvoiceViewModel> IndexPrint(string txtCode, string txtCusInfo, string txtProductName, string Status, int? SalerId, string startDate, string endDate, string txtMinAmount, string txtMaxAmount, int? BranchId)
        {
            if (startDate == null && endDate == null)
            {
                DateTime aDateTime = new DateTime(DateTime.Now.Year, 1, 1);
                // Cộng thêm 1 tháng và trừ đi một ngày.
                DateTime retDateTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(1).AddDays(-1);

                startDate = aDateTime.ToString("dd/MM/yyyy");
                endDate = retDateTime.ToString("dd/MM/yyyy");
            }

            DateTime d_startDate, d_endDate;
            if (DateTime.TryParseExact(startDate, "dd/MM/yyyy", new CultureInfo("vi-VN"), DateTimeStyles.None, out d_startDate))
            {

            }
            if (DateTime.TryParseExact(endDate, "dd/MM/yyyy", new CultureInfo("vi-VN"), DateTimeStyles.None, out d_endDate))
            {
                d_endDate = d_endDate.AddHours(23).AddMinutes(59);
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
            var user = userRepository.GetByvwUserName(Helpers.Common.CurrentUser.UserName);
            var pp = productInvoiceRepository.GetAllvwProductInvoiceFulls().Where(x => x.CreatedDate >= d_startDate && x.CreatedDate <= d_endDate);
            //var q = productInvoiceRepository.GetAllvwProductInvoiceFull().Where(x => x.CreatedDate >= d_startDate && x.CreatedDate <= d_endDate);
            //.Where(x => x.BranchId == Helpers.Common.CurrentUser.BranchId);
            var model = pp.Select(item => new ProductInvoiceViewModel
            {
                Id = item.Id,
                IsDeleted = item.IsDeleted,
                CreatedUserId = item.CreatedUserId,
                CreatedDate = item.CreatedDate,
                ModifiedUserId = item.ModifiedUserId,
                ModifiedDate = item.ModifiedDate,
                Code = item.Code,
                CustomerCode = item.CustomerCode,
                CustomerName = item.CustomerName,
                ShipCityName = item.ShipCityName,
                TotalAmount = item.TotalAmount,
                //FixedDiscount = item.FixedDiscount,
                TaxFee = item.TaxFee,
                CodeInvoiceRed = item.CodeInvoiceRed,
                Status = item.Status,
                IsArchive = item.IsArchive,
                //ProductOutboundId = item.ProductOutboundId,
                //ProductOutboundCode = item.ProductOutboundCode,
                PaidAmount = item.PaidAmount,
                RemainingAmount = item.RemainingAmount,
                Note = item.Note,
                CancelReason = item.CancelReason,
                BranchId = item.BranchId,
                CustomerId = item.CustomerId,
                BranchName = item.BranchName,
                ManagerStaffId = item.ManagerStaffId,
                ManagerStaffName = item.ManagerStaffName,
                ManagerUserName = item.ManagerUserName,
                CountForBrand = item.CountForBrand,
                TotalDebit = item.TotalDebit,
                TotalCredit = item.TotalCredit,
                TongConNo = (item.TotalDebit - item.TotalCredit),
                UserTypeName = item.UserTypeName,
                CreatedUserName = item.StaffCreateName,
                PaymentMethod = item.PaymentMethod,
                DoanhThu = item.DoanhThu

            }).ToList();

            if (!string.IsNullOrEmpty(txtCode))
            {
                model = model.Where(x => x.Code.ToLowerOrEmpty().Contains(txtCode.Trim().ToLower())
                    //|| x.WarehouseSourceId == warehouseSourceId
                    ).ToList();
            }

            if (!string.IsNullOrEmpty(txtCusInfo))
            {
                model = model.Where(x => Helpers.Common.ChuyenThanhKhongDau(x.CustomerName).Contains(Helpers.Common.ChuyenThanhKhongDau(txtCusInfo)) || x.CustomerCode.Contains(txtCusInfo)).ToList();
            }

            //if (Helpers.Common.CurrentUser.BranchId != null)
            //{
            //    model = model.Where(x => x.BranchId == Helpers.Common.CurrentUser.BranchId.Value).ToList();
            //}
            if (intBrandID > 0)
            {
                model = model.Where(x => x.BranchId == intBrandID).ToList();
            }

            if (!string.IsNullOrEmpty(Status))
            {
                if (Status == "delete")
                {
                    model = model.Where(x => x.IsDeleted == true).ToList();
                }
                if (Status == "complete")
                {
                    model = model.Where(x => x.Status == "complete").ToList();
                }
                if (Status == "pending")
                {
                    model = model.Where(x => x.Status == "pending").ToList();
                }
                if (Status == "Đặt cọc")
                {
                    model = model.Where(x => x.Status == "Đặt cọc").ToList();
                }

                if (Status == "Đặt cọc và hoàn thành")
                {
                    model = model.Where(x => x.Status == "Đặt cọc" || x.Status == "complete").ToList();
                }
            }

            if ((SalerId != null) && (SalerId > 0))
            {
                model = model.Where(x => x.ManagerStaffId == SalerId).ToList();
            }

            decimal minAmount;
            if (decimal.TryParse(txtMinAmount, out minAmount))
            {
                model = model.Where(x => x.TotalAmount >= minAmount).ToList();
            }

            decimal maxAmount;
            if (decimal.TryParse(txtMaxAmount, out maxAmount))
            {
                if (maxAmount > 0)
                {
                    model = model.Where(x => x.TotalAmount <= maxAmount).ToList();
                }
            }

            //if (BranchId != null && BranchId.Value > 0)
            //{
            //    model = model.Where(x => x.BranchId == BranchId).ToList();
            //}

            if (!string.IsNullOrEmpty(txtProductName))
            {
                txtProductName = Helpers.Common.ChuyenThanhKhongDau(txtProductName);
                var productListId = ProductRepository.GetAllvwProductAndService()
                    .Where(item => item.Code == txtProductName || item.Name.Contains(txtProductName)).Select(item => item.Id).ToList();

                if (productListId.Count > 0)
                {
                    List<int> listProductInboundId = new List<int>();
                    foreach (var id in productListId)
                    {
                        var list = productInvoiceRepository.GetAllvwInvoiceDetails().Where(x => x.ProductName.Contains(txtProductName) || x.ProductCode == txtProductName)
                            .Select(item => item.ProductInvoiceId.Value).Distinct().ToList();

                        listProductInboundId.AddRange(list);
                    }

                    model = model.Where(item => listProductInboundId.Contains(item.Id)).ToList();
                }
            }

            model = model.OrderByDescending(c => c.CreatedDate).ToList();

            return model;
        }

        public ActionResult PrintProductInvoice(string txtCode, string txtCusInfo, string txtProductName, string Status, int? SalerId, string startDate, string endDate, string txtMinAmount, string txtMaxAmount, int? BranchId, bool ExportExcel = false)
        {
            var data = IndexPrint(txtCode, txtCusInfo, txtProductName, Status, SalerId, startDate, endDate, txtMinAmount, txtMaxAmount, BranchId);

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
            model.Content = model.Content.Replace("{DataTable}", buildHtmlDanhSachDonHang(data));
            model.Content = model.Content.Replace("{System.Logo}", ImgLogo);
            model.Content = model.Content.Replace("{System.CompanyName}", company);
            model.Content = model.Content.Replace("{System.AddressCompany}", address);
            model.Content = model.Content.Replace("{System.PhoneCompany}", phone);
            model.Content = model.Content.Replace("{System.Fax}", fax);
            model.Content = model.Content.Replace("{System.BankCodeCompany}", bankcode);
            model.Content = model.Content.Replace("{System.BankNameCompany}", bankname);
            model.Content = model.Content.Replace("{PrintDate}", DateTime.Now.ToString("dd/MM/yyyy HH:mm"));
            model.Content = model.Content.Replace("{Title}", "Danh sách đơn hàng");
            if (ExportExcel)
            {
                Response.AppendHeader("content-disposition", "attachment;filename=" + "DS_Donhang" + DateTime.Now.ToString("yyyyMMdd") + ".xls");
                Response.Charset = "";
                Response.Cache.SetCacheability(HttpCacheability.NoCache);
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.Write(model.Content);
                Response.End();
            }
            return View(model);
        }

        string buildHtmlDanhSachDonHang(List<ProductInvoiceViewModel> detailList)
        {
            string detailLists = "<table border=\"1\" class=\"invoice-detail\">\r\n";
            detailLists += "<thead>\r\n";
            detailLists += "	<tr>";
            detailLists += "		<th>STT</th>";
            detailLists += "		<th>Trạng thái</th>";
            detailLists += "		<th>Ghi chú</th>";
            detailLists += "		<th>Ngày tạo</th>";
            detailLists += "		<th>Người tạo</th>";
            detailLists += "		<th>Mã đơn hàng</th>";
            detailLists += "		<th>Tổng tiền</th>";
            detailLists += "		<th>Tổng thu</th>";
            detailLists += "		<th>Đã thanh toán</th>";
            detailLists += "		<th>HTTT</th>";
            detailLists += "		<th>Còn nợ</th>";
            detailLists += "		<th>Tên Khách hàng</th>";
            detailLists += "		<th>Mã Khách hàng</th>";
            detailLists += "		<th>Nhân viên QL</th>";
            detailLists += "		<th>Nhóm hưởng DS</th>";
            detailLists += "		<th>CT xuất kho</th>";
            detailLists += "		<th>Chi nhánh</th>";
            detailLists += "		<th>Ngày cập nhật</th>";
            detailLists += "		<th>TT ghi sổ</th>";
            detailLists += "		<th>Tính cho</th>";
            detailLists += "	</tr>";
            detailLists += "</thead>\r\n";
            detailLists += "<tbody>\r\n";
            var index = 1;
            decimal totalTotalAmount = 0;
            decimal totalDoanhThu = 0;
            decimal totalPaidAmount = 0;
            decimal totalRemainingAmount = 0;

            foreach (var item in detailList)
            {
                detailLists += "<tr>\r\n"
                + "<td class=\"text-center orderNo\">" + (index++) + "</td>\r\n"
                + "<td class=\"text-left \">" + (item.Status == "pending" ? "Khởi tạo" : item.Status == "delete" ? "Huỷ" : item.Status == "complete" ? "Hoàn thành" : item.Status) + "</td>\r\n"
                + "<td class=\"text-left \">" + item.Note + "</td>\r\n"
                + "<td>" + (item.CreatedDate.HasValue ? item.CreatedDate.Value.ToString("dd/MM/yyyy HH:mm") : "") + "</td>"
                + "<td class=\"text-left \">" + item.CreatedUserName + "</td>\r\n"
                + "<td class=\"text-left \">" + item.Code + "</td>\r\n"
                + "<td class=\"text-left \">" + CommonSatic.ToCurrencyStr(item.TotalAmount, null).Replace(".", ",") + "</td>\r\n"
                + "<td class=\"text-left \">" + CommonSatic.ToCurrencyStr(item.DoanhThu, null).Replace(".", ",") + "</td>\r\n"
                + "<td class=\"text-left \">" + CommonSatic.ToCurrencyStr(item.PaidAmount, null).Replace(".", ",") + "</td>\r\n"
                + "<td class=\"text-left \">" + item.PaymentMethod + "</td>\r\n"
                + "<td class=\"text-left \">" + CommonSatic.ToCurrencyStr(item.RemainingAmount, null).Replace(".", ",") + "</td>\r\n"
                + "<td class=\"text-left \">" + item.CustomerName + "</td>\r\n"
                + "<td class=\"text-left \">" + item.CustomerCode + "</td>\r\n"
                + "<td class=\"text-left \">" + item.ManagerUserName + "</td>\r\n"
                + "<td class=\"text-left \">" + item.UserTypeName + "</td>\r\n"
                + "<td class=\"text-left \">" + item.ProductOutboundId + "</td>\r\n"
                + "<td class=\"text-left \">" + item.BranchName + "</td>\r\n"
                + "<td>" + (item.ModifiedDate.HasValue ? item.ModifiedDate.Value.ToString("dd/MM/yyyy HH:mm") : "") + "</td>"
                + "<td class=\"text-left \">" + (item.IsArchive ? "Đã ghi sổ" : "Chưa ghi sổ") + "</td>\r\n"
                + "<td class=\"text-left \">" + item.CountForBrand + "</td>\r\n"
                + "</tr>\r\n";
                totalTotalAmount += item.TotalAmount;
                totalDoanhThu += item.DoanhThu ?? 0;
                totalPaidAmount += item.PaidAmount ?? 0;
                totalRemainingAmount += item.RemainingAmount ?? 0;
            }

            detailLists += "<tr>\r\n"
               + "<td style=\"font-weight:bold \">Tổng cộng</td>"
               + "<td class=\"text-left \">" + "" + "</td>\r\n"
               + "<td class=\"text-left \">" + "" + "</td>\r\n"
               + "<td class=\"text-left \">" + "" + "</td>\r\n"
               + "<td class=\"text-left \">" + "" + "</td>\r\n"
               + "<td class=\"text-left \">" + "" + "</td>\r\n"
               + "<td class=\"text-left \" style=\"font-weight:bold \">" + CommonSatic.ToCurrencyStr(totalTotalAmount, null).Replace(".", ",") + "</td>\r\n"
               + "<td class=\"text-left \" style=\"font-weight:bold \">" + CommonSatic.ToCurrencyStr(totalDoanhThu, null).Replace(".", ",") + "</td>\r\n"
               + "<td class=\"text-left \" style=\"font-weight:bold \">" + CommonSatic.ToCurrencyStr(totalPaidAmount, null).Replace(".", ",") + "</td>\r\n"
               + "<td class=\"text-left \">" + "" + "</td>\r\n"
               + "<td class=\"text-left \" style=\"font-weight:bold \">" + CommonSatic.ToCurrencyStr(totalRemainingAmount, null).Replace(".", ",") + "</td>\r\n"
               + "<td class=\"text-left \">" + "" + "</td>\r\n"
               + "<td class=\"text-left \">" + "" + "</td>\r\n"
               + "<td class=\"text-left \">" + "" + "</td>\r\n"
               + "<td class=\"text-left \">" + "" + "</td>\r\n"
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

        //--------------------------------------------------------------------------------------------------------//

        public ActionResult Print(int Id, bool ExportExcel = false)
        {
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
            //lấy hóa đơn.
            var productInvoice = productInvoiceRepository.GetvwProductInvoiceById(Id);
            //lấy thông tin khách hàng
            var customer = customerRepository.GetvwCustomerByCode(productInvoice.CustomerCode);

            List<ProductInvoiceDetailViewModel> detailList = new List<ProductInvoiceDetailViewModel>();
            if (productInvoice != null && productInvoice.IsDeleted != true)
            {
                //lấy danh sách sản phẩm xuất kho
                detailList = productInvoiceRepository.GetAllvwInvoiceDetailsByInvoiceId(Id)
                        .Select(x => new ProductInvoiceDetailViewModel
                        {
                            Id = x.Id,
                            Price = x.Price,
                            ProductId = x.ProductId,
                            ProductName = x.ProductName,
                            ProductCode = x.ProductCode,
                            Quantity = x.Quantity,
                            Unit = x.Unit,
                            ProductGroup = x.ProductGroup,
                            ProductType = x.ProductType,
                            CategoryCode = x.CategoryCode,
                            LoCode = x.LoCode,
                            ExpiryDate = x.ExpiryDate
                        }).ToList();
            }

            //lấy template phiếu xuất.
            var template = templatePrintRepository.GetAllTemplatePrint().Where(x => x.Code.Contains("ProductInvoice")).OrderByDescending(x => x.CreatedDate).FirstOrDefault();
            //truyền dữ liệu vào template.
            model.Content = template.Content;
            model.Content = model.Content.Replace("{Code}", productInvoice.Code);
            model.Content = model.Content.Replace("{CreateDate}", productInvoice.CreatedDate.Value.ToString("dd/MM/yyyy HH:mm"));
            model.Content = model.Content.Replace("{Hours}", productInvoice.CreatedDate.Value.ToString("HH:mm"));
            model.Content = model.Content.Replace("{CustomerName}", customer.LastName + " " + customer.FirstName + "(" + customer.Code + ")");
            model.Content = model.Content.Replace("{NhomQL}", productInvoice.NhomNVKD);
            model.Content = model.Content.Replace("{CustomerPhone}", customer.Phone);
            model.Content = model.Content.Replace("{CompanyName}", customer.CompanyName + " (" + customer.Code + ")");
            model.Content = model.Content.Replace("{StaffCreateName}", productInvoice.StaffCreateName);
            if (!string.IsNullOrEmpty(customer.Address))
            {
                model.Content = model.Content.Replace("{Address}", customer.Address + ", ");
            }
            else
            {
                model.Content = model.Content.Replace("{Address}", "");
            }
            if (!string.IsNullOrEmpty(customer.DistrictName))
            {
                model.Content = model.Content.Replace("{District}", customer.DistrictName + ", ");
            }
            else
            {
                model.Content = model.Content.Replace("{District}", "");
            }
            if (!string.IsNullOrEmpty(customer.WardName))
            {
                model.Content = model.Content.Replace("{Ward}", customer.WardName + ", ");
            }
            else
            {
                model.Content = model.Content.Replace("{Ward}", "");
            }
            if (!string.IsNullOrEmpty(customer.ProvinceName))
            {
                model.Content = model.Content.Replace("{Province}", customer.ProvinceName);
            }
            else
            {
                model.Content = model.Content.Replace("{Province}", "");
            }

            model.Content = model.Content.Replace("{Note}", productInvoice.Note);
            if (!string.IsNullOrEmpty(productInvoice.CodeInvoiceRed))
            {
                model.Content = model.Content.Replace("{InvoiceCode}", productInvoice.Code + " - " + productInvoice.CodeInvoiceRed);
            }
            else
            {
                model.Content = model.Content.Replace("{InvoiceCode}", productInvoice.Code);
            }
            model.Content = model.Content.Replace("{PaymentMethod}", productInvoice.PaymentMethod);
            model.Content = model.Content.Replace("{MoneyText}", Erp.BackOffice.Helpers.Common.ChuyenSoThanhChu_2(productInvoice.tienconno.ToString()));

            model.Content = model.Content.Replace("{DataTable}", buildHtmlDetailList(detailList, productInvoice));
            model.Content = model.Content.Replace("{System.Logo}", ImgLogo);
            model.Content = model.Content.Replace("{System.CompanyName}", company);
            model.Content = model.Content.Replace("{System.AddressCompany}", address);
            model.Content = model.Content.Replace("{System.PhoneCompany}", phone);
            model.Content = model.Content.Replace("{System.Fax}", fax);
            model.Content = model.Content.Replace("{System.BankCodeCompany}", bankcode);
            model.Content = model.Content.Replace("{System.BankNameCompany}", bankname);
            model.Content = model.Content.Replace("{PrintDate}", DateTime.Now.ToString("dd/MM/yyyy HH:mm"));
            //Tạo barcode
            Image barcode = Code128Rendering.MakeBarcodeImage(productInvoice.Code, 1, true);
            model.Content = model.Content.Replace("{BarcodeImgSource}", ImageToBase64(barcode, System.Drawing.Imaging.ImageFormat.Png));

            if (ExportExcel)
            {
                Response.AppendHeader("content-disposition", "attachment;filename=" + productInvoice.CreatedDate.Value.ToString("yyyyMMdd") + productInvoice.Code + ".xls");
                Response.Charset = "";
                Response.Cache.SetCacheability(HttpCacheability.NoCache);
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.Write(model.Content);
                Response.End();
            }
            return View(model);
        }

        string buildHtmlDetailList(List<ProductInvoiceDetailViewModel> detailList, vwProductInvoice model)
        {
            decimal? tong_tien = 0;
            //int da_thanh_toan = 0;
            //int con_lai = 0;
            //var productInvoice = productInvoiceRepository.GetvwProductInvoiceById(Id.Value);

            //Tạo table html chi tiết phiếu xuất
            string detailLists = "<table class=\"invoice-detail\">\r\n";
            detailLists += "<thead>\r\n";
            detailLists += "	<tr>\r\n";
            detailLists += "		<th>STT</th>\r\n";
            detailLists += "		<th>Mã SP</th>\r\n";
            detailLists += "		<th>Tên SP</th>\r\n";
            detailLists += "		<th>SL</th>\r\n";
            detailLists += "		<th>Đơn giá</th>\r\n";
            detailLists += "		<th>Thành tiền</th>\r\n";
            detailLists += "	</tr>\r\n";
            detailLists += "</thead>\r\n";
            detailLists += "<tbody>\r\n";
            var index = 1;

            foreach (var item in detailList)
            {
                decimal? subTotal = item.Quantity * item.Price.Value;

                tong_tien += subTotal;
                detailLists += "<tr>\r\n"
                + "<td class=\"text-center orderNo\">" + (index++) + "</td>\r\n"
                + "<td class=\"text-left code_product\">" + item.ProductCode + "</td>\r\n"
                + "<td class=\"text-left \">" + item.ProductName + "</td>\r\n"
                + "<td class=\"text-right orderNo\">" + Erp.BackOffice.Helpers.Common.PhanCachHangNgan2(item.Quantity) + "</td>\r\n"
                + "<td class=\"text-right code_product\">" + Erp.BackOffice.Helpers.CommonSatic.ToCurrencyStr(item.Price, null) + "</td>\r\n"
                + "<td class=\"text-right chiet_khau\">" + Erp.BackOffice.Helpers.CommonSatic.ToCurrencyStr(subTotal, null) + "</td>\r\n"
                + "</tr>\r\n";
            }
            detailLists += "</tbody>\r\n";
            detailLists += "<tfoot style=\"font-weight:bold\">\r\n";
            detailLists += "<tr><td colspan=\"5\" class=\"text-right\">Tổng cộng</td><td class=\"text-right\">"
                         + Erp.BackOffice.Helpers.CommonSatic.ToCurrencyStr(tong_tien, null)
                         + "</td></tr>\r\n";
            if (model.TaxFee > 0)
            {
                var vat = tong_tien * Convert.ToDecimal(model.TaxFee) / 100;
                var tong = tong_tien + vat;
                detailLists += "<tr><td colspan=\"5\" class=\"text-right\">VAT (" + model.TaxFee + "%)</td><td class=\"text-right\">"
                        + Erp.BackOffice.Helpers.CommonSatic.ToCurrencyStr(vat, null)
                        + "</td></tr>\r\n";
                detailLists += "<tr><td colspan=\"5\" class=\"text-right\">Tổng tiền</td><td class=\"text-right\">"
                    + Erp.BackOffice.Helpers.CommonSatic.ToCurrencyStr(tong, null)
                    + "</td></tr>\r\n";
            }


            detailLists += "<tr><td colspan=\"5\" class=\"text-right\">Chiết khấu</td><td class=\"text-right\">"
                        + (model.Discount > 0 ? Erp.BackOffice.Helpers.CommonSatic.ToCurrencyStr((decimal?)model.Discount, null) : "0")
                        + "</td></tr>\r\n";
            detailLists += "<tr><td colspan=\"5\" class=\"text-right\">Còn lại</td><td class=\"text-right\">"
                        + (model.TotalAmount > 0 ? Erp.BackOffice.Helpers.CommonSatic.ToCurrencyStr(model.TotalAmount, null) : "0")
                        + "</td></tr>\r\n";

            detailLists += "<tr><td colspan=\"5\" class=\"text-right\">Đã thanh toán</td><td class=\"text-right\">"
                       + (model.TotalAmount > 0 ? Erp.BackOffice.Helpers.CommonSatic.ToCurrencyStr(model.tiendathu, null) : "0")
                       + "</td></tr>\r\n";

            detailLists += "<tr><td colspan=\"5\" class=\"text-right\">Còn nợ</td><td class=\"text-right\">"
                     + (model.TotalAmount > 0 ? Erp.BackOffice.Helpers.CommonSatic.ToCurrencyStr(model.tienconno, null) : "0")
                     + "</td></tr>\r\n";

            detailLists += "</tfoot>\r\n</table>\r\n";

            return detailLists;
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
        public ActionResult Delete(int Id, string CancelReason)
        {
            var q = Membership_parentRepository.GetAllMembership_parent().Where(item => item.ProductInvoiceId == Id).FirstOrDefault();
            if (q != null)
            {
                List<Membership_parent> updateMers = Membership_parentRepository.GetListMembership_parentById(q.Id);
                foreach (var item in updateMers)
                {
                    item.IsDeleted = true;
                    Membership_parentRepository.UpdateMembership_parent(item);
                }


            }


            var productInvoice = productInvoiceRepository.GetProductInvoiceById(Id);
            if (productInvoice != null)
            {
                //Kiểm tra phân quyền Chi nhánh
                //if (!(Filters.SecurityFilter.IsAdmin() || ("," + Helpers.Common.CurrentUser.DrugStore + ",").Contains("," + productInvoice.BranchId + ",") == true))
                //{
                //    return Content("Mẫu tin không tồn tại! Không có quyền truy cập!");
                //}

                productInvoice.ModifiedUserId = WebSecurity.CurrentUserId;
                productInvoice.ModifiedDate = DateTime.Now;
                productInvoice.IsDeleted = true;
                productInvoice.IsArchive = false;
                productInvoice.CancelReason = CancelReason;
                productInvoice.Status = Wording.OrderStatus_deleted;
                productInvoiceRepository.UpdateProductInvoice(productInvoice);


                #region isDelete receipt
                var receipt = ReceiptRepository.GetAllReceipts()
                    .Where(item => item.MaChungTuGoc == productInvoice.Code).ToList();
                var receipt_detail = receiptDetailRepository.GetAllReceiptDetail().Where(x => x.MaChungTuGoc == productInvoice.Code).ToList();
                if (receipt_detail.Count() > 0)
                {
                    // isDelete chi tiết phiếu thu
                    for (int i = 0; i < receipt_detail.Count(); i++)
                    {
                        receiptDetailRepository.DeleteReceiptDetailRs(receipt_detail[i].Id);
                    }
                }
                if (receipt.Count() > 0)
                {
                    // isDelete phiếu thu
                    for (int i = 0; i < receipt.Count(); i++)
                    {
                        ReceiptRepository.DeleteReceiptRs(receipt[i].Id);
                    }
                }
                #endregion

                return RedirectToAction("Detail", new { Id = productInvoice.Id });
            }

            return RedirectToAction("Index");
        }
        #endregion

        #region Detail

        public ActionResult DetailByChart(string single, int? day, int? month, int? year, string CityId, string DistrictId, string branchId, int? quarter, int? week)
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

            var user = userRepository.GetByvwUserName(Helpers.Common.CurrentUser.UserName);
            single = single == null ? "month" : single;
            year = year == null ? DateTime.Now.Year : year;
            month = month == null ? DateTime.Now.Month : month;
            quarter = quarter == null ? 1 : quarter;
            CityId = string.IsNullOrEmpty(CityId) ? "" : CityId;
            DistrictId = string.IsNullOrEmpty(DistrictId) ? "" : DistrictId;
            Calendar calendar = CultureInfo.InvariantCulture.Calendar;
            var weekdefault = calendar.GetWeekOfYear(DateTime.Now, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
            week = week == null ? weekdefault : week;
            branchId = branchId == null ? "" : branchId.ToString();
            branchId = strBrandID;
            DateTime StartDate = DateTime.Now;
            DateTime EndDate = DateTime.Now;

            ViewBag.DateRangeText = Helpers.Common.ConvertToDateRange(ref StartDate, ref EndDate, single, year.Value, month.Value, quarter.Value, ref week);


            var q = productInvoiceRepository.GetAllvwProductInvoice().AsEnumerable().Where(x => x.IsArchive == true);
            //if (!Filters.SecurityFilter.IsAdmin() && !Filters.SecurityFilter.IsKeToan() && string.IsNullOrEmpty(branchId))
            //{
            //    q = q.Where(x => ("," + user.DrugStore + ",").Contains("," + x.BranchId + ",") == true);
            //}
            if (!string.IsNullOrEmpty(CityId))
            {
                q = q.Where(item => !string.IsNullOrEmpty(item.CityId) && item.CityId == CityId);
            }
            if (!string.IsNullOrEmpty(DistrictId))
            {
                q = q.Where(item => !string.IsNullOrEmpty(item.DistrictId) && item.DistrictId == DistrictId);
            }
            if (!string.IsNullOrEmpty(branchId))
            {
                q = q.Where(item => !string.IsNullOrEmpty(branchId) && ("," + branchId + ",").Contains("," + item.BranchId + ",") == true);
            }
            if (year != null)
            {
                q = q.Where(n => n.Year == year);
            }
            if (month != null)
            {
                q = q.Where(n => n.Month == month);
            }
            if (day != null)
            {
                q = q.Where(n => n.Day == day);
            }

            q = q.Where(x => x.IsArchive && x.CreatedDate > StartDate && x.CreatedDate < EndDate);

            var model = q.Select(item => new ProductInvoiceViewModel
            {
                Id = item.Id,
                IsDeleted = item.IsDeleted,
                CreatedUserId = item.CreatedUserId,
                CreatedDate = item.CreatedDate,
                ModifiedUserId = item.ModifiedUserId,
                ModifiedDate = item.ModifiedDate,
                Code = item.Code,
                CustomerCode = item.CustomerCode,
                CustomerName = item.CustomerName,
                ShipCityName = item.ShipCityName,
                TotalAmount = item.TotalAmount,
                Discount = item.Discount,
                TaxFee = item.TaxFee,
                CodeInvoiceRed = item.CodeInvoiceRed,
                Status = item.Status,
                IsArchive = item.IsArchive,
                //ProductOutboundId = item.ProductOutboundId,
                //ProductOutboundCode = item.ProductOutboundCode,
                Note = item.Note,
                CancelReason = item.CancelReason,
                PaidAmount = item.PaidAmount,
                RemainingAmount = item.RemainingAmount,
                BranchName = item.BranchName
            }).OrderByDescending(m => m.CreatedDate);

            return View(model);
        }

        public ActionResult Detail(int? Id, string TransactionCode, string code)
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



            var productInvoice = new vwProductInvoice();

            if (Id != null && Id.Value > 0)
            {
                productInvoice = productInvoiceRepository.GetvwProductInvoiceFullById(Id.Value);
            }

            if (!string.IsNullOrEmpty(TransactionCode))
            {
                productInvoice = productInvoiceRepository.GetvwProductInvoiceByCode(intBrandID.Value, TransactionCode);
            }
            if (!string.IsNullOrEmpty(code))
            {
                productInvoice = productInvoiceRepository.GetvwProductInvoiceByCode(intBrandID.Value, code);
            }
            if (productInvoice == null)
            {
                //return JavaScript("alert('Đơn hàng không tồn tại! Vui lòng kiểm tra lại.')");
                return RedirectToAction("Index", new { test = "1" });
            }

            var model = new ProductInvoiceViewModel();
            AutoMapper.Mapper.Map(productInvoice, model);

            model.ReceiptViewModel = new ReceiptViewModel();
            model.NextPaymentDate_Temp = DateTime.Now.AddDays(30);
            model.ReceiptViewModel.Name = "Bán hàng - Thu tiền mặt";
            model.ReceiptViewModel.Amount = productInvoice.TotalAmount;


            //Lấy thông tin kiểm tra cho phép sửa chứng từ này hay không
            model.AllowEdit = Helpers.Common.KiemTraNgaySuaChungTu
                (model.CreatedDate.Value)
                && (Filters.SecurityFilter.IsAdmin()
                || productInvoice.BranchId == intBrandID
                );

            //Lấy lịch sử giao dịch thanh toán
            var listTransaction = transactionLiabilitiesRepository.GetAllvwTransaction()
                        .Where(item => item.BranchId == intBrandID.Value && item.MaChungTuGoc == productInvoice.Code)
                        .OrderByDescending(item => item.CreatedDate)
                        .ToList();
            model.ListTransactionLiabilities = new List<TransactionLiabilitiesViewModel>();
            AutoMapper.Mapper.Map(listTransaction, model.ListTransactionLiabilities);
            //Lấy danh sách chi tiết đơn hàng
            model.InvoiceList = productInvoiceRepository.GetAllvwInvoiceDetailsByInvoiceId(productInvoice.Id).Select(x =>
                new ProductInvoiceDetailViewModel
                {
                    Id = x.Id,
                    Price = x.Price,
                    ProductId = x.ProductId,
                    Quantity = x.Quantity,
                    Unit = x.Unit,
                    ProductType = x.ProductType,

                    CategoryCode = x.CategoryCode,
                    ProductInvoiceCode = x.ProductInvoiceCode,
                    ProductName = x.ProductName,
                    ProductCode = x.ProductCode,
                    ProductInvoiceId = x.ProductInvoiceId,
                    ProductGroup = x.ProductGroup,
                    Amount = x.is_TANG == 1 ? 0 : x.Amount,
                    LoCode = x.LoCode,
                    ExpiryDate = x.ExpiryDate,
                    Discount = x.Discount,
                    DiscountAmount = x.DiscountAmount,
                    is_TANG = x.is_TANG,
                    Tiensaugiam = x.Tiensaugiam


                }).OrderBy(x => x.Id).ToList();
            //Lấy thông tin phiếu xuất kho
            //if (productInvoice.ProductOutboundId != null && productInvoice.ProductOutboundId > 0)
            //{
            //    var productOutbound = productOutboundRepository.GetvwProductOutboundById(productInvoice.ProductOutboundId.Value);
            //    model.ProductOutboundViewModel = new ProductOutboundViewModel();
            //    AutoMapper.Mapper.Map(productOutbound, model.ProductOutboundViewModel);
            //}

            ViewBag.isAdmin = Erp.BackOffice.Helpers.Common.CurrentUser.UserTypeId == 1 ? true : false;
            ViewBag.mbs = model.InvoiceList.Where(x => x.ProductType == "service");
            // model.ModifiedUserName = userRepository.GetUserById(model.ModifiedUserId.Value).FullName;
            model.LogCommission = new List<LogPromotionViewModel>();
            var data = logPromotionRepository.GetvwAllLogPromotion().Where(x => x.ProductInvoiceCode == model.Code).ToList();
            if (data.Any())
                AutoMapper.Mapper.Map(data, model.LogCommission);
            model.LogCommission.ForEach(x => x.CheckSave = true);
            ViewBag.SuccessMessage = TempData["SuccessMessage"];
            ViewBag.FailedMessage = TempData["FailedMessage"];
            ViewBag.AlertMessage = TempData["AlertMessage"];

            return View(model);
        }
        #endregion

        #region UnArchive
        [HttpPost]
        public ActionResult UnArchive(int Id, bool IsPopup)
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

            if (Request["Submit"] == "Save")
            {
                var productInvoice = productInvoiceRepository.GetProductInvoiceById(Id);


                var productInvoicexacnhanthutien = productInvoiceRepository.GetvwProductInvoiceById(Id);
                if (productInvoicexacnhanthutien != null && productInvoicexacnhanthutien.daxacnhanthutien > 0)
                {
                    TempData[Globals.FailedMessageKey] = "Phiếu thu đã được xác nhận không thể bỏ ghi sổ được !";
                    return RedirectToAction("Detail", new { Id = Id, IsPopup = IsPopup });
                }


                // Kiểm tra phân quyền Chi nhánh
                if (!(Filters.SecurityFilter.IsAdmin() || productInvoice.BranchId == intBrandID))
                {
                    return Content("Mẫu tin không tồn tại! Không có quyền truy cập!");
                }

                //Kiểm tra cho phép sửa chứng từ này hay không
                if (Helpers.Common.KiemTraNgaySuaChungTu(productInvoice.CreatedDate.Value) == false)
                {
                    TempData[Globals.FailedMessageKey] = "Quá hạn sửa chứng từ!";
                }
                else
                {
                    using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.Snapshot }))
                    {
                        try
                        {
                            #region isDelete receipt
                            var receipt = ReceiptRepository.GetAllReceipts()
                                .Where(item => item.MaChungTuGoc == productInvoice.Code).ToList();
                            var receipt_detail = receiptDetailRepository.GetAllReceiptDetail().Where(x => x.MaChungTuGoc == productInvoice.Code).ToList();
                            if (receipt_detail.Count() > 0)
                            {
                                // isDelete chi tiết phiếu thu
                                for (int i = 0; i < receipt_detail.Count(); i++)
                                {
                                    receiptDetailRepository.DeleteReceiptDetailRs(receipt_detail[i].Id);
                                }
                            }
                            if (receipt.Count() > 0)
                            {
                                // isDelete phiếu thu
                                for (int i = 0; i < receipt.Count(); i++)
                                {
                                    ReceiptRepository.DeleteReceiptRs(receipt[i].Id);
                                }
                            }
                            #endregion

                            #region isDelete listTransaction
                            //isDelete lịch sử giao dịch
                            var listTransaction = transactionLiabilitiesRepository.GetAllTransaction()
                            .Where(item => item.BranchId == intBrandID && item.MaChungTuGoc == productInvoice.Code)
                            .Select(item => item.Id)
                            .ToList();

                            var listTransactionCode = transactionLiabilitiesRepository.GetAllTransaction()
                           .Where(item => item.BranchId == intBrandID && item.MaChungTuGoc == productInvoice.Code)
                           .Select(item => item.TransactionCode)
                           .ToList();

                            foreach (var item in listTransaction)
                            {
                                transactionLiabilitiesRepository.DeleteTransaction(item);
                            }
                            #endregion



                            #region  hoapd them de xoa quan he giao dich isDelete listRelationship
                            //isDelete lịch sử giao dịch
                            var listTransactionRelationship = transactionRepository.GetAllTransactionRelationship()
                            .Where(item => item.BranchId == intBrandID && item.TransactionB == productInvoice.Code)
                            .Select(item => new TransactionRelationshipViewModel
                            {
                                Id = item.Id,
                                TransactionA = item.TransactionA
                            })
                            .ToList();

                            foreach (var item in listTransactionRelationship)
                            {

                                bool has = listTransactionCode.Any(cus => listTransactionCode.Contains(item.TransactionA));

                                if (has == true)
                                {
                                    transactionRepository.DeleteTransactionRelationship(item.Id);
                                }
                            }
                            #endregion

                            #region bỏ ghi sổ ProductOutbound
                            var productOutbound = productOutboundRepository.GetAllProductOutbound().Where(x => x.InvoiceId == productInvoice.Id).ToList();
                            foreach (var item in productOutbound)
                            {

                                string check = "";
                                //var detail = productOutboundRepository.GetAllvwProductOutboundDetailByOutboundId(item.Id).ToList();

                                var detail = productOutboundRepository.GetAllvwProductOutboundDetailByOutboundId(item.Id)
                               .Select(item1 => new ProductTam
                               {
                                   ProductName = item1.ProductCode + " - " + item1.ProductName,
                                   ProductId = item1.ProductId.Value,
                                   Quantity = item1.Quantity.Value,
                                   LoCode = item1.LoCode,
                                   ExpiryDate = item1.ExpiryDate
                               }).ToList();




                                //begin hoapd su ly san pham trung nhau thi cong so luong va chi de  lai 1 row
                                List<ProductTam> lstdetailList = new List<ProductTam>();
                                foreach (ProductTam item1 in detail)
                                {
                                    var bmCheck = lstdetailList.Where(x => x.ProductId == item1.ProductId && x.LoCode == item1.LoCode && x.ExpiryDate == item1.ExpiryDate);
                                    if ((bmCheck == null) || (bmCheck.Count() == 0))
                                    {
                                        lstdetailList.Add(item1);
                                    }
                                    else
                                    {
                                        bmCheck.ElementAt(0).Quantity = bmCheck.ElementAt(0).Quantity + item1.Quantity;
                                    }

                                }

                                //end hoapd su ly san pham trung nhau thi cong so luong va chi de  lai 1 row



                                foreach (var item2 in lstdetailList)
                                {
                                    var error = InventoryController.Check(item2.ProductName, item2.ProductId, item2.LoCode, item2.ExpiryDate, item.WarehouseSourceId.Value, item2.Quantity, 0);
                                    check += error;
                                }
                                if (string.IsNullOrEmpty(check))
                                {
                                    //Khi đã hợp lệ thì mới update
                                    foreach (var i in lstdetailList)
                                    {
                                        InventoryController.Update(i.ProductName, i.ProductId, i.LoCode, i.ExpiryDate, item.WarehouseSourceId.Value, i.Quantity, 0);
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

                            productInvoice.PaidAmount = 0;
                            productInvoice.DoanhThu = 0;
                            productInvoice.RemainingAmount = productInvoice.TotalAmount;
                            productInvoice.NextPaymentDate = null;
                            productInvoice.ModifiedUserId = WebSecurity.CurrentUserId;
                            productInvoice.ModifiedDate = DateTime.Now;
                            productInvoice.IsArchive = false;
                            productInvoice.Status = Wording.OrderStatus_inprogress;
                            productInvoiceRepository.UpdateProductInvoice(productInvoice);

                            //ghi log 
                            Erp.BackOffice.Controllers.HomeController.WriteLog(productInvoice.Id, productInvoice.Code, "đã bỏ ghi sổ đơn hàng", "ProductInvoice/Detail/" + productInvoice.Id, Helpers.Common.CurrentUser.BranchId.Value);

                            TempData[Globals.SuccessMessageKey] = "Đã bỏ ghi sổ";

                            #region isDelete membership
                            var mbs = membershipRepository.GetAllMembership()
                                .Where(item => item.TargetModule == "ProductInvoiceDetail" && item.TargetCode == productInvoice.Code).ToList();
                            if (mbs.Count() > 0)
                            {
                                // isDelete MBS
                                //for (int i = 0; i < mbs.Count(); i++)
                                //{
                                //    membershipRepository.DeleteMembershipRs(mbs[i].Id);
                                //}
                            }
                            #endregion
                            #region Xoa chuyển tiền nếu có
                            var moneymove = productInvoiceRepository.GetMMByNewId(productInvoice.Id);
                            if (moneymove != null)
                            {
                                var oldInvoice = productInvoiceRepository.GetProductInvoiceById(moneymove.FromProductInvoiceId);
                                if (oldInvoice.IsArchive != true)
                                {
                                    oldInvoice.Status = App_GlobalResources.Wording.Status_deposit;
                                    oldInvoice.Note = "";
                                }
                                else
                                {
                                    oldInvoice.Status = "complete";
                                    oldInvoice.Note = "";
                                }
                                productInvoiceRepository.UpdateProductInvoice(oldInvoice);
                                productInvoiceRepository.DeleteMoneyMove(moneymove.Id);
                            }

                            #endregion
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

        string bill(int Id)
        {
            var model = new TemplatePrintViewModel();
            //lấy logo công ty
            var logo = Erp.BackOffice.Helpers.Common.GetSetting("LogoCompany");
            var company = Erp.BackOffice.Helpers.Common.GetSetting("companyName");
            var address = Erp.BackOffice.Helpers.Common.GetSetting("addresscompany");
            var phone = Erp.BackOffice.Helpers.Common.GetSetting("phonecompany");
            var fax = Erp.BackOffice.Helpers.Common.GetSetting("faxcompany");
            var bankcode = Erp.BackOffice.Helpers.Common.GetSetting("bankcode");
            var bankname = Erp.BackOffice.Helpers.Common.GetSetting("bankname");
            var ImgLogo = "<div class=\"logo\"><img src=http://ngochuong.osales.vn/" + logo + " height=\"73\" /></div>";
            //lấy hóa đơn.
            var productInvoice = productInvoiceRepository.GetvwProductInvoiceById(Id);
            //lấy thông tin khách hàng
            var customer = customerRepository.GetvwCustomerByCode(productInvoice.CustomerCode);

            List<ProductInvoiceDetailViewModel> detailList = new List<ProductInvoiceDetailViewModel>();
            if (productInvoice != null && productInvoice.IsDeleted != true)
            {
                //lấy danh sách sản phẩm xuất kho
                detailList = productInvoiceRepository.GetAllvwInvoiceDetailsByInvoiceId(Id)
                        .Select(x => new ProductInvoiceDetailViewModel
                        {
                            Id = x.Id,
                            Price = x.Price,
                            ProductId = x.ProductId,
                            ProductName = x.ProductName,
                            ProductCode = x.ProductCode,
                            Quantity = x.Quantity,
                            Unit = x.Unit,
                            ProductGroup = x.ProductGroup,
                            ProductType = x.ProductType,
                            CategoryCode = x.CategoryCode
                        }).ToList();
            }
            //lấy template phiếu xuất.
            var template = templatePrintRepository.GetAllTemplatePrint().Where(x => x.Code.Contains("ProductInvoice")).OrderByDescending(x => x.CreatedDate).FirstOrDefault();
            //truyền dữ liệu vào template.
            model.Content = template.Content;
            model.Content = model.Content.Replace("{Code}", productInvoice.Code);
            model.Content = model.Content.Replace("{CreateDate}", productInvoice.CreatedDate.Value.ToString("dd/MM/yyyy HH:mm"));
            model.Content = model.Content.Replace("{Hours}", productInvoice.CreatedDate.Value.ToString("HH:mm"));
            model.Content = model.Content.Replace("{CustomerName}", customer.LastName + " " + customer.FirstName);
            model.Content = model.Content.Replace("{CustomerPhone}", customer.Phone);
            model.Content = model.Content.Replace("{CompanyName}", customer.CompanyName);
            model.Content = model.Content.Replace("{StaffCreateName}", productInvoice.StaffCreateName);
            if (!string.IsNullOrEmpty(customer.Address))
            {
                model.Content = model.Content.Replace("{Address}", customer.Address + ", ");
            }
            else
            {
                model.Content = model.Content.Replace("{Address}", "");
            }
            if (!string.IsNullOrEmpty(customer.DistrictName))
            {
                model.Content = model.Content.Replace("{District}", customer.DistrictName + ", ");
            }
            else
            {
                model.Content = model.Content.Replace("{District}", "");
            }
            if (!string.IsNullOrEmpty(customer.WardName))
            {
                model.Content = model.Content.Replace("{Ward}", customer.WardName + ", ");
            }
            else
            {
                model.Content = model.Content.Replace("{Ward}", "");
            }
            if (!string.IsNullOrEmpty(customer.ProvinceName))
            {
                model.Content = model.Content.Replace("{Province}", customer.ProvinceName);
            }
            else
            {
                model.Content = model.Content.Replace("{Province}", "");
            }

            model.Content = model.Content.Replace("{Note}", productInvoice.Note);
            if (!string.IsNullOrEmpty(productInvoice.CodeInvoiceRed))
            {
                model.Content = model.Content.Replace("{InvoiceCode}", productInvoice.Code + " - " + productInvoice.CodeInvoiceRed);
            }
            else
            {
                model.Content = model.Content.Replace("{InvoiceCode}", productInvoice.Code);
            }
            model.Content = model.Content.Replace("{PaymentMethod}", productInvoice.PaymentMethod);
            model.Content = model.Content.Replace("{MoneyText}", Erp.BackOffice.Helpers.Common.ChuyenSoThanhChu_2(Convert.ToInt32(productInvoice.TotalAmount)));

            model.Content = model.Content.Replace("{DataTable}", buildHtmlDetailList(detailList, productInvoice));
            model.Content = model.Content.Replace("{System.Logo}", ImgLogo);
            model.Content = model.Content.Replace("{System.CompanyName}", company);
            model.Content = model.Content.Replace("{System.AddressCompany}", address);
            model.Content = model.Content.Replace("{System.PhoneCompany}", phone);
            model.Content = model.Content.Replace("{System.Fax}", fax);
            model.Content = model.Content.Replace("{System.BankCodeCompany}", bankcode);
            model.Content = model.Content.Replace("{System.BankNameCompany}", bankname);
            //model.Content = model.Content.Replace("{Reminder}", NoteReminder);
            model.Content = model.Content.Replace("{PrintDate}", DateTime.Now.ToString("dd/MM/yyyy HH:mm"));
            //Tạo barcode
            Image barcode = Code128Rendering.MakeBarcodeImage(productInvoice.Code, 1, true);
            model.Content = model.Content.Replace("{BarcodeImgSource}", ImageToBase64(barcode, System.Drawing.Imaging.ImageFormat.Png));

            return model.Content;
        }

        #region Success
        [HttpPost]
        public ActionResult Success(ProductInvoiceViewModel model, string GHICHU)
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
            var productInvoice1 = productInvoiceRepository.GetvwProductInvoiceById(model.Id);
            if ((Convert.ToDecimal(model.ReceiptViewModel.Amount) - productInvoice1.tienconno) > 0)
            {

                TempData[Globals.FailedMessageKey] = "Số tiền vượt quá khoản nợ " + productInvoice1.tienconno;
                return RedirectToAction("Detail", new { Id = productInvoice1.Id });
            }

            var productInvoice = productInvoiceRepository.GetProductInvoiceById(model.Id);

            using (var scope = new TransactionScope(
                           TransactionScopeOption.Required,
                           new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted, Timeout = new TimeSpan(1, 0, 0) })) // 1 hour or wathever, will not affect anything
            {
                try
                {
                    var invoice_detail_list = productInvoiceRepository.GetAllvwInvoiceDetailsByInvoiceId(productInvoice.Id).ToList();
                    ProductOutboundViewModel productOutboundViewModel = new ProductOutboundViewModel();
                    var warehouseDefault = warehouseRepository.GetAllWarehouse().Where(x => x.BranchId == productInvoice.BranchId && x.IsSale == true).FirstOrDefault();
                    //model.ReceiptViewModel = new ReceiptViewModel();
                    model.NextPaymentDate_Temp = DateTime.Now;
                    model.ReceiptViewModel.Amount = model.ReceiptViewModel.Amount;
                    model.ReceiptViewModel.Name = string.Empty;
                    model.ReceiptViewModel.PaymentMethod = model.ReceiptViewModel.PaymentMethod;

                    if (warehouseDefault == null)
                    {
                        TempData[Globals.FailedMessageKey] += "<br/>Chi nhánh này không tìm thấy kho xuất bán! Vui lòng kiểm tra lại!";
                        return RedirectToAction("Detail", new { Id = model.Id });
                    }
                    string check = "";
                    foreach (var item in invoice_detail_list)
                    {
                        var error = InventoryController.Check(item.ProductName, item.ProductId.Value, item.LoCode, item.ExpiryDate, warehouseDefault.Id, 0, item.Quantity.Value);
                        check += error;
                        // item.Note = GHICHU;
                    }
                    if (!string.IsNullOrEmpty(check))
                    {
                        TempData[Globals.FailedMessageKey] = App_GlobalResources.Wording.ArchiveFail + check;
                        return RedirectToAction("Detail", new { Id = model.Id });
                    }
                    #region  phiếu xuất ok
                    var product_outbound = productOutboundRepository.GetAllProductOutboundFull().Where(x => x.InvoiceId == productInvoice.Id).ToList();

                    //insert mới
                    if (product_outbound.Count() <= 0)
                    {
                        #region  thêm mới phiếu xuất

                        //Nếu trong đơn hàng có sản phẩm thì xuất kho
                        if (warehouseDefault != null)
                        {
                            productOutboundViewModel.InvoiceId = productInvoice.Id;
                            productOutboundViewModel.InvoiceCode = productInvoice.Code;
                            productOutboundViewModel.WarehouseSourceId = warehouseDefault.Id;
                            productOutboundViewModel.Note = "Xuất kho cho đơn hàng " + productInvoice.Code;
                            var DetailList = invoice_detail_list.Select(x =>
                                  new ProductInvoiceDetailViewModel
                                  {
                                      Id = x.Id,
                                      Price = x.Price,
                                      ProductId = x.ProductId,
                                      Quantity = x.Quantity,
                                      Unit = x.Unit,
                                      ProductType = x.ProductType,
                                      CategoryCode = x.CategoryCode,
                                      ProductInvoiceCode = x.ProductInvoiceCode,
                                      ProductName = x.ProductName,
                                      ProductCode = x.ProductCode,
                                      ProductInvoiceId = x.ProductInvoiceId,
                                      ProductGroup = x.ProductGroup,
                                      LoCode = x.LoCode,
                                      ExpiryDate = x.ExpiryDate,
                                      Amount = x.Amount,
                                      Type = x.Type,
                                      Note = x.Note

                                  }).ToList();
                            //Lấy dữ liệu cho chi tiết
                            productOutboundViewModel.DetailList = new List<ProductOutboundDetailViewModel>();
                            AutoMapper.Mapper.Map(DetailList, productOutboundViewModel.DetailList);

                            var productOutbound = ProductOutboundController.AutoCreateProductOutboundFromProductInvoice(productOutboundRepository, productOutboundViewModel, productInvoice.Code);
                            //ProductOutboundController.Archive_mobile(productOutbound, model.CreatedUserId.GetValueOrDefault());
                            //Ghi sổ chứng từ phiếu xuất
                            ProductOutboundController.Archive(productOutbound, TempData);
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
                            var insert_list = invoice_detail_list.ToList();
                            //insert mới
                            for (int iii = 0; iii < insert_list.Count(); iii++)
                            {
                                ProductOutboundDetail productOutboundDetail = new ProductOutboundDetail();
                                productOutboundDetail.Price = insert_list[iii].Price;
                                productOutboundDetail.ProductId = insert_list[iii].ProductId;
                                productOutboundDetail.Quantity = insert_list[iii].Quantity;
                                productOutboundDetail.Unit = insert_list[iii].Unit;
                                productOutboundDetail.LoCode = insert_list[iii].LoCode;
                                productOutboundDetail.ExpiryDate = insert_list[iii].ExpiryDate;
                                productOutboundDetail.IsDeleted = false;
                                productOutboundDetail.CreatedUserId = WebSecurity.CurrentUserId;
                                productOutboundDetail.ModifiedUserId = WebSecurity.CurrentUserId;
                                productOutboundDetail.CreatedDate = DateTime.Now;
                                productOutboundDetail.ModifiedDate = DateTime.Now;
                                productOutboundDetail.ProductOutboundId = product_outbound[i].Id;
                                productOutboundRepository.InsertProductOutboundDetail(productOutboundDetail);

                            }
                            product_outbound[i].TotalAmount = invoice_detail_list.Sum(x => (x.Price * x.Quantity));

                            //Ghi sổ chứng từ phiếu xuất
                            //kiểm tra nếu phiếu xuất đã ghi sổ rồi thì phải bỏ ghi sổ
                            if (product_outbound[i].IsArchive == true)
                            {
                                ProductOutboundController.UnArchiveAnd_no_Delete(product_outbound[i]);
                            }

                            ProductOutboundController.Archive(product_outbound[i], TempData);
                        }
                        #endregion
                    }
                    #endregion



                    if (!productInvoice.IsArchive)
                    {

                        #region neu chua thu co thi tao giao dich phai thu KH
                        //Lấy mã KH
                        var customer = customerRepository.GetCustomerById(productInvoice.CustomerId.Value);
                        var transaction_Liablities = transactionLiabilitiesRepository.GetAllTransaction().Where(x => x.BranchId == intBrandID && x.MaChungTuGoc == productInvoice.Code && x.LoaiChungTuGoc == "ProductInvoice").ToList();
                        if (transaction_Liablities.Count() == 0)
                        {

                            #region thêm lịch sử bán hàng
                            //Ghi Nợ TK 131 - Phải thu của khách hàng (tổng giá thanh toán)
                            Erp.BackOffice.Account.Controllers.TransactionLiabilitiesController.Create(
                                    intBrandID,
                                    productInvoice.Code,
                                    "ProductInvoice",
                                    "Phải thu của khách hàng cho đơn hàng " + productInvoice.Code,
                                    customer.Code,
                                    "Customer",
                                    productInvoice.TotalAmount,
                                    0,
                                    productInvoice.Code,
                                    "ProductInvoice",
                                    null,
                                    model.NextPaymentDate_Temp,
                                    null);
                            #endregion
                        }
                        #endregion






                        #region  Cập nhật thông tin thanh toán cho đơn hàng
                        //Cập nhật thông tin thanh toán cho đơn hàng
                        productInvoice.PaymentMethod = model.ReceiptViewModel.PaymentMethod;
                        productInvoice.PaidAmount += Convert.ToDecimal(model.ReceiptViewModel.Amount);
                        productInvoice.DoanhThu = productInvoice.TotalAmount;
                        productInvoice.RemainingAmount = productInvoice.TotalAmount - productInvoice.PaidAmount;
                        productInvoice.NextPaymentDate = model.NextPaymentDate_Temp;
                        //productInvoice.Note = model.Note;
                        productInvoice.ModifiedDate = DateTime.Now;
                        productInvoice.ModifiedUserId = WebSecurity.CurrentUserId;
                        productInvoiceRepository.UpdateProductInvoice(productInvoice);



                        var remain = productInvoice.TotalAmount - Convert.ToDecimal(model.ReceiptViewModel.Amount.Value);
                        if (remain > 0)
                        {
                        }
                        else
                        {
                            productInvoice.NextPaymentDate = null;
                            model.NextPaymentDate_Temp = null;
                        }
                        #endregion

                        #region phiếu thu
                        //Khách thanh toán ngay
                        if (model.ReceiptViewModel.Amount > 0)
                        {
                            #region thêm mới phiếu thu
                            //Lập phiếu thu
                            var receipt_inser = new Receipt();
                            AutoMapper.Mapper.Map(model.ReceiptViewModel, receipt_inser);
                            receipt_inser.IsDeleted = false;
                            receipt_inser.CreatedUserId = WebSecurity.CurrentUserId;
                            receipt_inser.ModifiedUserId = WebSecurity.CurrentUserId;
                            receipt_inser.AssignedUserId = WebSecurity.CurrentUserId;
                            receipt_inser.CreatedDate = DateTime.Now;
                            receipt_inser.ModifiedDate = DateTime.Now;
                            receipt_inser.VoucherDate = DateTime.Now;
                            receipt_inser.CustomerId = customer.Id;
                            receipt_inser.IsArchive = false;
                            receipt_inser.Payer = customer.LastName + " " + customer.FirstName;
                            receipt_inser.PaymentMethod = productInvoice.PaymentMethod;
                            receipt_inser.Address = customer.Address;
                            receipt_inser.MaChungTuGoc = productInvoice.Code;
                            receipt_inser.LoaiChungTuGoc = "ProductInvoice";
                            receipt_inser.Note = "Thu tiền khách hàng cho đơn hàng " + productInvoice.Code;
                            receipt_inser.BranchId = intBrandID.Value;

                            if (receipt_inser.Amount > productInvoice.TotalAmount)
                                receipt_inser.Amount = productInvoice.TotalAmount;

                            ReceiptRepository.InsertReceipt(receipt_inser);
                            receipt_inser.Code = Erp.BackOffice.Helpers.Common.GetOrderNo("Receipt");
                            ReceiptRepository.UpdateReceipt(receipt_inser);
                            Erp.BackOffice.Helpers.Common.SetOrderNo("Receipt");


                            //Thêm vào quản lý chứng từ
                            TransactionController.Create(new TransactionViewModel
                            {
                                TransactionModule = "Receipt",
                                TransactionCode = receipt_inser.Code,
                                TransactionName = "Thu tiền khách hàng cho đơn hàng " + productInvoice.Code
                            });

                            // Thêm chứng từ liên quan
                            TransactionController.CreateRelationship(new TransactionRelationshipViewModel
                            {
                                TransactionA = receipt_inser.Code,
                                TransactionB = productInvoice.Code
                            });

                            //Ghi Có TK 131 - Phải thu của khách hàng.
                            Erp.BackOffice.Account.Controllers.TransactionLiabilitiesController.Create(
                                intBrandID,
                                receipt_inser.Code,
                                "Receipt",
                                "Thu tiền khách hàng cho đơn hàng " + productInvoice.Code,
                                customer.Code,
                                "Customer",
                                0,
                                Convert.ToDecimal(model.ReceiptViewModel.Amount),
                                productInvoice.Code,
                                "ProductInvoice",
                                model.ReceiptViewModel.PaymentMethod,
                                null,
                                GHICHU);

                            #endregion
                        }
                        #endregion

                        #region cập nhật đơn bán hàng
                        //Cập nhật đơn hàng
                        productInvoice.ModifiedUserId = WebSecurity.CurrentUserId;
                        productInvoice.ModifiedDate = DateTime.Now;
                        productInvoice.IsArchive = true;
                        productInvoice.Status = Wording.OrderStatus_complete;
                        productInvoiceRepository.UpdateProductInvoice(productInvoice);

                        //ghi log 
                        Erp.BackOffice.Controllers.HomeController.WriteLog(productInvoice.Id, productInvoice.Code, "đã ghi sổ đơn hàng", "ProductInvoice/Detail/" + productInvoice.Id, Helpers.Common.CurrentUser.BranchId.Value);

                        TempData[Globals.SuccessMessageKey] = App_GlobalResources.Wording.ArchiveSuccess;

                        TempData[Globals.SuccessMessageKey] += "<br/>Đơn hàng này đã được xuất kho! Vui lòng kiểm tra lại chứng từ xuất kho để tránh sai sót dữ liệu!";
                        #endregion
                        //#region tạo phiếu membership
                        //if (invoice_detail_list.Where(x => x.ProductType == "service").Any())
                        //{
                        //    var insert_Service = invoice_detail_list.Where(x => x.ProductType == "service").ToList();
                        //    foreach (var item in insert_Service)
                        //    {
                        //        for (int i = 0; i < item.Quantity; i++)
                        //        {
                        //            var ins = new Erp.Domain.Sale.Entities.Membership();
                        //            ins.IsDeleted = false;
                        //            ins.CreatedUserId = WebSecurity.CurrentUserId;
                        //            ins.ModifiedUserId = WebSecurity.CurrentUserId;
                        //            ins.AssignedUserId = WebSecurity.CurrentUserId;
                        //            ins.CreatedDate = DateTime.Now;
                        //            ins.ModifiedDate = DateTime.Now;
                        //            ins.Status = "pending";
                        //            ins.BranchId = productInvoice.BranchId;
                        //            ins.CustomerId = productInvoice.CustomerId;
                        //            ins.ExpiryDate = item.ExpiryDate;
                        //            ins.ProductId = item.ProductId;
                        //            ins.TargetId = item.Id;
                        //            ins.TargetModule = "ProductInvoiceDetail";
                        //            ins.TargetCode = productInvoice.Code;
                        //            //begin hòa bổ sung vào để nhận biết đâu là dịch vụ tóc, đâu là dịch vụ da
                        //            ins.Type = item.CategoryCode;
                        //            //end hòa bổ sung vào để nhận biết đâu là dịch vụ tóc, đâu là dịch vụ da
                        //            membershipRepository.InsertMembership(ins);
                        //            ins.Code = Erp.BackOffice.Helpers.Common.GetOrderNo("Membership");
                        //            membershipRepository.UpdateMembership(ins);
                        //            Erp.BackOffice.Helpers.Common.SetOrderNo("Membership");
                        //        }
                        //    }
                        //}
                        //#endregion
                    }
                    scope.Complete();
                }
                catch (Exception ex)
                {
                    return Content("Fail");
                }
            }
            return RedirectToAction("Detail", new { Id = model.Id });
        }
        #endregion

        #region UpdateAll
        //   [HttpPost]
        //public ActionResult UpdateAll(string url)
        //{
        //     DateTime aDateTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
        //     // Cộng thêm 1 tháng và trừ đi một ngày.
        //    DateTime retDateTime = aDateTime.AddDays(18);
        //    var product_invoice = productInvoiceRepository.GetAllProductInvoice().Where(x => x.IsArchive == true && x.CreatedDate >= aDateTime && x.CreatedDate <= retDateTime).ToList();
        //    foreach (var item in product_invoice)
        //    {
        //          CommisionStaffController.CreateCommission(item.Id);
        //    }   
        //    TempData[Globals.SuccessMessageKey] = App_GlobalResources.Wording.UpdateSuccess;
        //    return Redirect(url);
        //}
        #endregion

        #region CreateNT

        public ActionResult Create(int? Id)
        {

            if (Erp.BackOffice.Helpers.Common.CurrentUser.BranchId == null)
            {
                TempData[Globals.FailedMessageKey] = "Bạn không có quyền tạo đơn hàng!";

                return RedirectToAction("Index");
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

            ProductInvoiceViewModel model = new ProductInvoiceViewModel();
            model.InvoiceList = new List<ProductInvoiceDetailViewModel>();
            var user = userRepository.GetByvwUserName(Helpers.Common.CurrentUser.UserName);
            model.BranchId = intBrandID;
            if (Id.HasValue && Id > 0)
            {
                var productInvoice = productInvoiceRepository.GetvwProductInvoiceById(Id.Value);
                AutoMapper.Mapper.Map(productInvoice, model);

                model.InvoiceList = productInvoiceRepository.GetAllvwInvoiceDetailsByInvoiceId(productInvoice.Id).Select(x => new ProductInvoiceDetailViewModel
                {
                    Id = x.Id,
                    Amount = x.Amount,
                    BranchId = x.BranchId,
                    CategoryCode = x.CategoryCode,
                    ExpiryDate = x.ExpiryDate,
                    Unit = x.Unit,
                    LoCode = x.LoCode,
                    Price = x.Price,
                    ProductCode = x.ProductCode,
                    ProductGroup = x.ProductGroup,
                    ProductId = x.ProductId,
                    ProductName = x.ProductName,
                    Quantity = x.Quantity,
                    Type = x.Type,
                    Origin = x.Origin,
                    Note = x.Note,
                    is_TANG = x.is_TANG,
                    Origin_Price = 0,
                }).ToList();
                //var product = ProductRepository.GetvwProductById()
                for (int i = 0; i < model.InvoiceList.Count(); i++)
                {
                    var product = ProductRepository.GetvwProductAndServiceById(model.InvoiceList[0].ProductId.Value);
                    model.InvoiceList[0].Origin_Price = product.PriceOutbound;
                }
                ViewBag.FailedMessage = TempData["FailedMessage"];
            }
            else
            {
                model.Id = 0;
            }
            model.ReceiptViewModel = new ReceiptViewModel();
            model.NextPaymentDate_Temp = DateTime.Now.AddDays(30);
            model.ReceiptViewModel.Name = "Bán hàng - Thu tiền mặt";
            model.ReceiptViewModel.Amount = 0;
            var productList = Domain.Helper.SqlHelper.QuerySP<InventoryViewModel>("spSale_Get_Inventory", new { WarehouseId = "", HasQuantity = "1", ProductCode = "", ProductName = "", CategoryCode = "", ProductGroup = "", BranchId = model.BranchId, LoCode = "", ProductId = "", ExpiryDate = "", Origin = "" });
            productList = productList.Where(x => x.IsSale != null && x.IsSale == true);

            //Thêm số lượng tồn kho cho chi tiết đơn hàng đã được thêm
            //if (model.InvoiceList != null && model.InvoiceList.Count > 0)
            //{
            //    foreach (var item in model.InvoiceList)
            //    {
            //        var product = productList.Where(i => i.ProductId == item.ProductId).FirstOrDefault();
            //        if (product != null)
            //        {
            //            item.QuantityInInventory = product.Quantity;
            //            item.PriceTest = product.ProductPriceOutbound;
            //        }
            //        else
            //        {
            //            item.Id = 0;
            //        }
            //    }
            //}
            int taxfee = 0;
            int.TryParse(Helpers.Common.GetSetting("vat"), out taxfee);
            model.TaxFee = taxfee;
            if (Helpers.Common.NVL_NUM_NT_NEW(model.Id) == 0)
            {
                model.IsBonusSales = true;
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

            return View(model);
        }

        [HttpPost]
        public ActionResult Create(ProductInvoiceViewModel model)
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
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors);
            }

            if (ModelState.IsValid)
            {
                ProductInvoice productInvoice = null;

                if (Erp.BackOffice.Helpers.Common.CurrentUser.BranchId == null)
                {
                    TempData[Globals.FailedMessageKey] = "Bạn không có quyền tạo đơn hàng!";

                    //return RedirectToAction("Create");
                    return RedirectToAction("Create", new { Id = model.Id });
                }


                if (model.Id > 0)
                {
                    productInvoice = productInvoiceRepository.GetProductInvoiceById(model.Id);
                }
                using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.Snapshot }))
                {
                    try
                    {
                        if (productInvoice != null)
                        {
                            //Nếu đã ghi sổ rồi thì không được sửa
                            if (productInvoice.IsArchive)
                            {
                                return RedirectToAction("Detail", new { Id = productInvoice.Id });
                            }
                            AutoMapper.Mapper.Map(model, productInvoice);
                            productInvoice.Type = "invoice";
                            productInvoice.TotalAmount = model.TongTienSauVAT;
                            productInvoice.IsReturn = false;
                            productInvoice.ModifiedUserId = WebSecurity.CurrentUserId;
                            productInvoice.ModifiedDate = DateTime.Now;
                            productInvoice.RemainingAmount = 0;
                            productInvoice.PaidAmount = model.TongTienSauVAT;
                            productInvoice.TotalAmount = model.TongTienSauVAT;
                            productInvoice.ProductInvoiceId_Return = model.ProductInvoiceOldId;
                            productInvoice.Discount_VIP = model.Discount_VIP;
                            productInvoice.BranchId = intBrandID.Value;
                            productInvoiceRepository.UpdateProductInvoice(productInvoice);

                            //ghi log 
                            Erp.BackOffice.Controllers.HomeController.WriteLog(productInvoice.Id, productInvoice.Code, "đã cập nhật đơn hàng", "ProductInvoice/Detail/" + productInvoice.Id, Helpers.Common.CurrentUser.BranchId.Value);

                            //xóa membership cũ, nếu có
                            var mbsp = Membership_parentRepository.GetAllMembership_parentByInvoiceId(productInvoice.Id).ToList();
                            if (mbsp != null)
                            {
                                foreach (var item in mbsp)
                                {
                                    var mbs = membershipRepository.GetAllMembershipByIdParent(item.Id).ToList();
                                    if (mbs != null)
                                    {
                                        foreach (var i in mbs)
                                        {
                                            i.IsDeleted = true;
                                            membershipRepository.UpdateMembership(i);
                                        }
                                    }
                                    item.IsDeleted = true;
                                    Membership_parentRepository.UpdateMembership_parent(item);
                                }
                            }
                        }
                        else
                        {
                            productInvoice = new ProductInvoice();
                            AutoMapper.Mapper.Map(model, productInvoice);
                            productInvoice.IsDeleted = false;
                            productInvoice.CreatedUserId = WebSecurity.CurrentUserId;
                            productInvoice.CreatedDate = DateTime.Now;
                            productInvoice.Status = Wording.OrderStatus_pending;
                            productInvoice.BranchId = intBrandID.Value;
                            productInvoice.IsArchive = false;
                            productInvoice.IsReturn = false;
                            productInvoice.Type = "invoice";
                            productInvoice.RemainingAmount = 0;
                            productInvoice.PaidAmount = model.TongTienSauVAT;
                            productInvoice.TotalAmount = model.TongTienSauVAT;
                            productInvoice.ProductInvoiceId_Return = model.ProductInvoiceOldId;
                            productInvoice.PaymentMethod = model.PaymentMethod;
                            productInvoice.SaleChanel = 1;
                            //hàm thêm mới
                            productInvoiceRepository.InsertProductInvoice(productInvoice, null);


                            //cập nhật lại mã hóa đơn
                            productInvoice.Code = Erp.BackOffice.Helpers.Common.GetOrderNo("ProductInvoice", model.Code);

                            //begin kiểm tra trùng mã đơn hàng
                            var c = productInvoiceRepository.GetAllvwProductInvoice().
                                Where(item => item.Code == productInvoice.Code).FirstOrDefault();

                            if (c != null)
                            {
                                TempData[Globals.FailedMessageKey] = "Mã đơn hàng đã bị trùng. Vui lòng kiểm tra lại!";

                                //return RedirectToAction("Create");
                                return RedirectToAction("Create", new { Id = c.Id });
                            }
                            //end kiểm tra trùng mã đơn hàng


                            productInvoiceRepository.UpdateProductInvoice(productInvoice);
                            Erp.BackOffice.Helpers.Common.SetOrderNo("ProductInvoice");

                            //ghi log 
                            Erp.BackOffice.Controllers.HomeController.WriteLog(productInvoice.Id, productInvoice.Code, "đã tạo đơn hàng", "ProductInvoice/Detail/" + productInvoice.Id, Helpers.Common.CurrentUser.BranchId.Value);

                        }
                        var _listdata = productInvoiceRepository.GetAllInvoiceDetailsByInvoiceId(productInvoice.Id).ToList();
                        if (model.InvoiceList.Any(x => x.Id == 0))
                        {
                            //lưu danh sách thao tác thực hiện dịch vụ
                            foreach (var item in model.InvoiceList.Where(x => x.Id == 0 && x.ProductId > 0))
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
                                ins.ProductInvoiceId = productInvoice.Id;
                                ins.Price = item.Price;
                                ins.Quantity = item.Quantity;
                                ins.Unit = item.Unit;
                                ins.Discount = 0;
                                ins.DiscountAmount = 0;
                                ins.Note = item.Note;
                                ins.is_TANG = item.is_TANG;
                                ins.BranchId = intBrandID.Value;
                                //  ins.ProductType = item.ProductType;
                                productInvoiceRepository.InsertProductInvoiceDetail(ins);
                            }
                        }
                        var _delete = _listdata.Where(id1 => !model.InvoiceList.ToList().Any(id2 => id2.Id == id1.Id)).ToList();
                        foreach (var item in _delete)
                        {
                            productInvoiceRepository.DeleteProductInvoiceDetail(item.Id);
                        }
                        if (model.InvoiceList.Any(x => x.Id > 0))
                        {
                            var update = _listdata.Where(id1 => model.InvoiceList.ToList().Any(id2 => id2.Id == id1.Id)).ToList();
                            //lưu danh sách thao tác thực hiện dịch vụ
                            foreach (var item in model.InvoiceList.Where(x => x.Id > 0 && x.ProductId > 0))
                            {
                                var _update = update.FirstOrDefault(x => x.Id == item.Id);
                                _update.ExpiryDate = item.ExpiryDate;
                                _update.LoCode = item.LoCode;
                                _update.ProductId = item.ProductId.Value;
                                _update.Price = item.Price;
                                _update.Quantity = item.Quantity;
                                _update.Unit = item.Unit;
                                _update.is_TANG = item.is_TANG;
                                productInvoiceRepository.UpdateProductInvoiceDetail(_update);
                            }
                        }


                        //begin hoapd tinh lai hoa don 
                        _listdata = productInvoiceRepository.GetAllInvoiceDetailsByInvoiceId(productInvoice.Id).ToList();
                        decimal Tongtien = 0;
                        foreach (var item in _listdata)
                        {
                            if (item.is_TANG != 1)
                            {
                                Tongtien = Tongtien + Math.Round(Helpers.Common.NVL_NUM_DECIMAL_NEW(item.Quantity * item.Price)) - Helpers.Common.NVL_NUM_DECIMAL_NEW(item.DiscountAmount);
                            }
                        }
                        decimal TongTyleGiamgia = Helpers.Common.NVL_NUM_DECIMAL_NEW(model.Discount_DB) + Helpers.Common.NVL_NUM_DECIMAL_NEW(model.Discount_KM) + Helpers.Common.NVL_NUM_DECIMAL_NEW(model.Discount_VIP);
                        decimal TongtienGiam = Math.Round((Tongtien / 100) * TongTyleGiamgia);
                        Tongtien = Tongtien - TongtienGiam;
                        Tongtien = Math.Round(Tongtien + (Helpers.Common.NVL_NUM_DECIMAL_NEW(model.TaxFee) * (Tongtien / 100)));

                        if ((productInvoice != null) && (productInvoice.Id > 0))
                        {
                            //Nếu đã ghi sổ rồi thì không được sửa
                            productInvoice = productInvoiceRepository.GetProductInvoiceById(productInvoice.Id);
                            if (productInvoice.IsArchive)
                            {
                                return RedirectToAction("Detail", new { Id = productInvoice.Id });
                            }

                            int productInvoiceID = productInvoice.Id;

                            productInvoice.Type = "invoice";
                            productInvoice.IsReturn = false;
                            productInvoice.Id = productInvoiceID;
                            productInvoice.ModifiedUserId = WebSecurity.CurrentUserId;
                            productInvoice.ModifiedDate = DateTime.Now;
                            productInvoice.RemainingAmount = Tongtien;
                            productInvoice.DiscountAmount = TongtienGiam;
                            productInvoice.Discount = (double)TongTyleGiamgia;
                            productInvoice.PaidAmount = 0;
                            productInvoice.DoanhThu = 0;
                            productInvoice.TotalAmount = Tongtien;
                            productInvoiceRepository.UpdateProductInvoice(productInvoice);
                        }

                        //begin neu da co phai thu khach hang thi phai cap nhat lai so tien phai thu
                        var transaction_Liablities = transactionLiabilitiesRepository.GetAllTransaction().Where(x => x.BranchId == intBrandID.Value && x.MaChungTuGoc == productInvoice.Code && x.LoaiChungTuGoc == "ProductInvoice" && x.TransactionModule == "ProductInvoice" && x.TargetModule == "Customer").ToList();
                        if (transaction_Liablities.Count() > 0)
                        {
                            transaction_Liablities[0].Debit = Tongtien;
                            transactionLiabilitiesRepository.UpdateTransaction(transaction_Liablities[0]);
                        }
                        //end neu da co phai thu khach hang thi phai cap nhat lai so tien phai thu



                        //end hoapd tinh lai hoa don

                        //Thêm vào quản lý chứng từ
                        TransactionController.Create(new TransactionViewModel
                        {
                            TransactionModule = "ProductInvoice",
                            TransactionCode = productInvoice.Code,
                            TransactionName = "Bán hàng"
                        });
                        scope.Complete();
                    }
                    catch (DbUpdateException)
                    {
                        TempData[Globals.FailedMessageKey] = App_GlobalResources.Wording.Error;
                        return View(model);
                    }
                }
                //if (productInvoice.Discount_VIP == null)
                //    productInvoice.Discount_VIP = 0;
                return RedirectToAction("Detail", new { Id = productInvoice.Id });
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

            return View(model);
        }
        #endregion


        #region GetLoyaltyPoint
        [HttpGet]
        public JsonResult GetLoyaltyPoint(int id)
        {
            //Sửa sum thành max - cong
            string sql = string.Format(@"select COALESCE((select max(y.PlusPoint)
                                        from [dbo].[Sale_LogVip] x  join  [dbo].[Sale_LoyaltyPoint] y on x.LoyaltyPointId = y.Id 
                                        where x.CustomerId = {0} and x.is_approved = 1 
                                        GROUP BY x.CustomerId),0) as VIP", id);
            var json_data = Domain.Helper.SqlHelper.QuerySQL(sql).FirstOrDefault();
            return Json(json_data, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region AutoCreateProductInvoice

        public static void AutoCreateProductInvoice(IProductInvoiceRepository productInvoiceRepository, ref ProductInvoiceViewModel model, List<ProductInvoiceDetailViewModel> DetailList)
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
                    //  ins.ProductType = item.ProductType;
                    productInvoiceRepository.InsertProductInvoiceDetail(ins);
                }
            }
            var _delete = _listdata.Where(id1 => !DetailList.ToList().Any(id2 => id2.Id == id1.Id)).ToList();
            foreach (var item in _delete)
            {
                productInvoiceRepository.DeleteProductInvoiceDetail(item.Id);
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
        }
        #endregion


        #region GetListUser

        public JsonResult GetListUser()
        {
            var branchId = Erp.BackOffice.Helpers.Common.CurrentUser.BranchId.Value;
            var user = userRepository.GetAllvwUsers().Where(x => x.BranchId == branchId)
             .Select(item => new
             {
                 item.Id,
                 item.FullName,
                 item.UserCode,
                 item.ProfileImage,
                 item.UserTypeName
             }).OrderBy(item => item.FullName).ToList();

            var pathimg = Helpers.Common.GetSetting("User");
            var json_data = user.Select(item => new
            {
                item.Id,
                Code = item.UserCode,
                Image = Helpers.Common.KiemTraTonTaiHinhAnhHoapd(item.ProfileImage, "User", "user",pathimg),
                Name = item.FullName,
                Text = item.FullName,
                Note = item.UserCode,
                Value = item.Id
            });
            return Json(json_data, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region GetDetailJsonByInvoiceId
        [AllowAnonymous]
        public JsonResult GetDetailJsonByInvoiceId(int Id)
        {
            var user = productInvoiceRepository.GetAllvwInvoiceDetailsByInvoiceId(Id)
             .Select(item => new
             {
                 item.Id,
                 item.Amount,
                 item.ExpiryDate,
                 item.LoCode,
                 item.ProductId,
                 item.Unit,
                 item.Quantity,
                 item.Price,
                 item.ProductCode,
                 item.ProductName,
                 item.ProductType,
                 item.ProductImage,
                 item.Origin,
                 item.CategoryCode,
                 item.Note,
                 item.Tiensaugiam,
                 item.CountForBrand

             }).OrderBy(item => item.Id).ToList();

            var json_data = user.Select(item => new
            {
                item.Id,
                Code = item.ProductCode,
                Image = item.ProductImage,
                Name = item.ProductName,
                Text = item.ProductName,
                Note = item.Note,
                Value = item.Id,
                Quantity = item.Quantity,
                ExpiryDate = item.ExpiryDate,
                LoCode = item.LoCode,
                Unit = item.Unit,
                ProductId = item.ProductId,
                Amount = item.Tiensaugiam,
                Price = item.Tiensaugiam / item.Quantity,
                Type = item.ProductType,
                Categories = "KB",
                Origin = item.Origin,
                QuantityTotalInventory = 0,
                OriginalPrice = item.Price,
                Brand = item.CountForBrand
            });
            return Json(json_data, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region AddPromotion

        public ActionResult AddPromotion(int Id)
        {

            ProductInvoiceViewModel model = new ProductInvoiceViewModel();
            model.InvoiceList = new List<ProductInvoiceDetailViewModel>();
            model.commission = new CommissionCusViewModel();
            model.commission.DetailList = new List<CommisionCustomerViewModel>();
            model.commission.InvoiceDetailList = new List<CommisionInvoiceViewModel>();
            model.LogCommission = new List<LogPromotionViewModel>();
            if (Id > 0)
            {
                var productInvoice = productInvoiceRepository.GetvwProductInvoiceById(Id);
                AutoMapper.Mapper.Map(productInvoice, model);
                ///chi tiet đơn hàng
                model.InvoiceList = productInvoiceRepository.GetAllvwInvoiceDetailsByInvoiceId(productInvoice.Id).Select(x => new ProductInvoiceDetailViewModel
                {
                    Id = x.Id,
                    Amount = x.Amount,
                    CategoryCode = x.CategoryCode,
                    ExpiryDate = x.ExpiryDate,
                    Unit = x.Unit,
                    LoCode = x.LoCode,
                    Price = x.Price,
                    ProductCode = x.ProductCode,
                    ProductGroup = x.ProductGroup,
                    ProductId = x.ProductId,
                    ProductName = x.ProductName,
                    Quantity = x.Quantity,
                    ProductType = x.ProductType,
                    DiscountAmount = x.DiscountAmount,
                    Discount = x.Discount
                }).ToList();
                //khuyến mãi
                var commision = commissionCusRepository.GetAllCommissionCus().ToList();
                //kt xem đơn hàng có nằm trong chương trình khuyến mãi nào ko

                var dataKM = commision.Where(item => ("," + item.ApplyFor + ",").Contains("," + model.BranchId + ",") == true && item.StartDate <= UrlCommon.EndOfDay(DateTime.Now) && item.EndDate >= UrlCommon.StartOfDay(DateTime.Now)).ToList();
                if ((dataKM != null) && (dataKM.Count > 0))
                {
                    foreach (var _data in dataKM)
                    {
                        if (_data.Id == 4171)
                        {
                            int a = 0;
                        }
                        AutoMapper.Mapper.Map(_data, model.commission);
                        #region
                        var detail = commisionCustomerRepository.GetvwAllCommisionCustomer().Where(x => x.CommissionCusId == model.commission.Id).ToList();
                        detail = detail.Where(id1 => model.InvoiceList.Any(id2 => id2.ProductId == id1.ProductId)).ToList();
                        var commission_product = AddPromotionProduct(model.InvoiceList, detail);
                        //giảm giá sản phẩm
                        model.commission.DetailList = commission_product.Select(x => new CommisionCustomerViewModel
                        {
                            ProductId = x.ProductId.Value,
                            Id = x.Id,
                            IsMoney = x.IsMoney,
                            Type = x.Type,
                            CommissionValue = x.CommissionValue,
                            CommissionCusId = x.CommissionCusId.Value,
                            Symbol = x.Symbol,
                            Quantity = x.Quantity,
                            Price = x.Price,
                            ProductCode = x.ProductCode,
                            ProductName = x.ProductName,
                            ExpiryMonth = x.ExpiryMonth
                        }).ToList();
                        var data = donateProductOrServiceRepository.GetvwAllDonateProOrSer();
                        for (int i = 0; i < model.commission.DetailList.Count(); i++)
                        {
                            model.commission.DetailList[i].Index = i;
                            //mua sản phẩm tặng sản phẩm
                            if (model.commission.DetailList[i].Type == "donate")
                            {
                                model.commission.DetailList[i].DonateDetailList = new List<DonateProOrSerViewModel>();

                                var data_donate = data.AsEnumerable().Where(x => x.TargetId == model.commission.DetailList[i].Id && x.TargetModule == "CommisionCustomer").ToList();
                                if (data_donate.Any())
                                {
                                    model.commission.DetailList[i].DonateDetailList = data_donate
                                        .Select(x => new DonateProOrSerViewModel
                                        {
                                            Id = x.Id,
                                            CategoryCode = x.CategoryCode,
                                            ExpriryMonth = x.ExpriryMonth,
                                            Price = x.Price,
                                            ParentOrderNo = i,
                                            ProductCode = x.ProductCode,
                                            ProductId = x.ProductId,
                                            ProductName = x.ProductName,
                                            ProductType = x.ProductType,
                                            Quantity = x.Quantity,
                                            TargetId = x.TargetId,
                                            TargetModule = x.TargetModule,
                                            TotalQuantity = x.TotalQuantity
                                        }).ToList();
                                }
                            }
                        }
                        #endregion
                        //giảm giá tổng hóa đơn
                        ///
                        #region
                        var invoice_detail = commissionInvoiceRepository.GetAllCommisionInvoice().Where(x => x.CommissionCusId == model.commission.Id).ToList();
                        var commission_invoice = AddPromotionInvoice(invoice_detail, productInvoice);
                        model.commission.InvoiceDetailList = commission_invoice.Select(x => new CommisionInvoiceViewModel
                        {
                            EndAmount = x.EndAmount,
                            EndSymbol = x.EndSymbol,
                            Id = x.Id,
                            IsMoney = x.IsMoney,
                            IsVIP = x.IsVIP,
                            SalesPercent = x.SalesPercent,
                            StartAmount = x.StartAmount,
                            StartSymbol = x.StartSymbol,
                            Type = x.Type,
                            CommissionCusId = x.CommissionCusId,
                            CommissionValue = x.CommissionValue,
                            Name = x.Name
                        }).ToList();
                        //mua hóa đơn tang75 sản phẩm
                        for (int i = 0; i < model.commission.InvoiceDetailList.Count(); i++)
                        {
                            model.commission.InvoiceDetailList[i].Index = i;
                            model.commission.InvoiceDetailList[i].DonateDetailList = new List<DonateProOrSerViewModel>();
                            var data_invoice = data.AsEnumerable().Where(x => x.TargetId == model.commission.InvoiceDetailList[i].Id && x.TargetModule == "CommisionInvoice").ToList();
                            if (data_invoice.Any())
                            {
                                model.commission.InvoiceDetailList[i].DonateDetailList = data_invoice
                                    .Select(x => new DonateProOrSerViewModel
                                    {
                                        Id = x.Id,
                                        CategoryCode = x.CategoryCode,
                                        ExpriryMonth = x.ExpriryMonth,
                                        Price = x.Price,
                                        ParentOrderNo = i,
                                        ProductCode = x.ProductCode,
                                        ProductId = x.ProductId,
                                        ProductName = x.ProductName,
                                        ProductType = x.ProductType,
                                        Quantity = x.Quantity,
                                        TargetId = x.TargetId,
                                        TargetModule = x.TargetModule,
                                        TotalQuantity = x.TotalQuantity
                                    }).ToList();
                            }
                        }
                        #endregion
                        //gắn chương trình khuyến mãi trong cài đặt đã lấy ra gắn vào LogPromotionViewModel
                        //nguyên tắt gắn là cứ 1 sản phẩm tặng kèm thì sinh ra 1 dòng LogPromotionViewModel
                        #region AddPromotion Product
                        if (model.commission.DetailList.Any())
                        {
                            foreach (var item in model.commission.DetailList)
                            {

                                if (item.Type == "donate")
                                {
                                    foreach (var donate in item.DonateDetailList)
                                    {
                                        var aa = new LogPromotionViewModel();
                                        aa.Name = "Chương trình khuyến mãi mua sản phẩm tặng sản phẩm";
                                        aa.GiftProductId = donate.ProductId;
                                        aa.ProductInvoiceCode = model.Code;
                                        aa.ProductId = item.ProductId;
                                        aa.CommissionCusCode = model.commission.Name;
                                        aa.CommissionCusId = model.commission.Id;
                                        aa.CustomerId = model.CustomerId;
                                        aa.BranchId = model.BranchId;
                                        aa.DonateProOrSerId = donate.Id;
                                        aa.ProductQuantity = item.Quantity;
                                        aa.ProductSymbolQuantity = item.Symbol;
                                        aa.Type = "product_donate";
                                        aa.ProductName = item.ProductName;
                                        aa.GiftProductName = donate.ProductName;
                                        aa.TargetID = model.InvoiceList.FirstOrDefault(x => x.ProductId == aa.ProductId).Id;
                                        aa.TargetModule = "ProductInvoiceDetail";
                                        aa.Category = "auto";
                                        model.LogCommission.Add(aa);
                                    }

                                }
                                else
                                {
                                    var aa = new LogPromotionViewModel();
                                    aa.Name = "Chương trình khuyến mãi mua sản phẩm giảm giá";
                                    aa.ProductInvoiceCode = model.Code;
                                    aa.ProductId = item.ProductId;
                                    aa.CommissionCusCode = model.commission.Name;
                                    aa.CommissionCusId = model.commission.Id;
                                    aa.CustomerId = model.CustomerId;
                                    aa.BranchId = model.BranchId;
                                    aa.ProductQuantity = item.Quantity;
                                    aa.ProductSymbolQuantity = item.Symbol;
                                    aa.Type = "product_discount";
                                    aa.IsMoney = item.IsMoney;
                                    aa.CommissionValue = item.CommissionValue;
                                    aa.ProductName = item.ProductName;
                                    aa.TargetID = model.InvoiceList.FirstOrDefault(x => x.ProductId == aa.ProductId).Id;
                                    aa.TargetModule = "ProductInvoiceDetail";
                                    aa.Category = "auto";
                                    model.LogCommission.Add(aa);

                                }

                            }
                        }

                        #endregion

                        ////////////

                        #region AddPromotion Invoice
                        if (model.commission.InvoiceDetailList.Any())
                        {
                            foreach (var item in model.commission.InvoiceDetailList)
                            {

                                if (item.Type == "donate")
                                {
                                    foreach (var donate in item.DonateDetailList)
                                    {
                                        var aa = new LogPromotionViewModel();
                                        aa.Name = "Chương trình khuyến mãi tặng sản phẩm theo giá trị hóa đơn";
                                        aa.GiftProductId = donate.ProductId;
                                        aa.ProductInvoiceCode = model.Code;
                                        aa.StartSymbol = item.StartSymbol;
                                        aa.CommissionCusCode = model.commission.Name;
                                        aa.CommissionCusId = model.commission.Id;
                                        aa.CustomerId = model.CustomerId;
                                        aa.BranchId = model.BranchId;
                                        aa.DonateProOrSerId = donate.Id;
                                        aa.EndSymbol = item.EndSymbol;
                                        aa.EndAmount = item.EndAmount;
                                        aa.StartAmount = item.StartAmount;
                                        aa.Type = "invoice_donate";
                                        aa.GiftProductName = donate.ProductName;
                                        aa.TargetID = productInvoice.Id;
                                        aa.TargetModule = "ProductInvoice";
                                        aa.Category = "auto";
                                        model.LogCommission.Add(aa);
                                    }

                                }
                                else if (item.Type == "discountdif")
                                {

                                    var aa = new LogPromotionViewModel();
                                    aa.Name = item.Name;
                                    //aa.Name = "Chương trình khuyến mãi khác";
                                    aa.ProductInvoiceCode = model.Code;
                                    aa.StartSymbol = item.StartSymbol;
                                    aa.EndSymbol = item.EndSymbol;
                                    aa.EndAmount = item.EndAmount;
                                    aa.StartAmount = item.StartAmount;
                                    aa.CommissionCusCode = model.commission.Name;
                                    aa.CommissionCusId = model.commission.Id;
                                    aa.CustomerId = model.CustomerId;
                                    aa.BranchId = model.BranchId;
                                    aa.Type = "invoice_discountdif";
                                    aa.IsMoney = item.IsMoney;
                                    aa.CommissionValue = item.CommissionValue;
                                    aa.TargetID = productInvoice.Id;
                                    aa.TargetModule = "ProductInvoice";
                                    aa.Category = "auto";
                                    model.LogCommission.Add(aa);


                                }
                                else
                                {
                                    var aa = new LogPromotionViewModel();
                                    aa.Name = "Chương trình khuyến mãi giảm giá theo giá trị hóa đơn";
                                    aa.ProductInvoiceCode = model.Code;
                                    aa.StartSymbol = item.StartSymbol;
                                    aa.EndSymbol = item.EndSymbol;
                                    aa.EndAmount = item.EndAmount;
                                    aa.StartAmount = item.StartAmount;
                                    aa.CommissionCusCode = model.commission.Name;
                                    aa.CommissionCusId = model.commission.Id;
                                    aa.CustomerId = model.CustomerId;
                                    aa.BranchId = model.BranchId;
                                    aa.Type = "invoice_discount";
                                    aa.IsMoney = item.IsMoney;
                                    aa.CommissionValue = item.CommissionValue;
                                    aa.TargetID = productInvoice.Id;
                                    aa.TargetModule = "ProductInvoice";
                                    aa.Category = "auto";
                                    model.LogCommission.Add(aa);

                                }

                            }
                        }
                        #endregion

                    }
                }
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult AddPromotion(ProductInvoiceViewModel model)
        {
            ProductInvoice productInvoice = null;

            if (model.Id > 0)
            {
                productInvoice = productInvoiceRepository.GetProductInvoiceById(model.Id);
            }

            if (model.LogCommission == null)
            {
                TempData[Globals.FailedMessageKey] += "<br/>Bạn chưa chọn chương trình khuyến mãi! Vui lòng kiểm tra lại!";
                return RedirectToAction("Detail", new { Id = model.Id });
            }


            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.Snapshot }))
            {
                try
                {
                    if (productInvoice != null)
                    {

                        var _listdata = productInvoiceRepository.GetAllInvoiceDetailsByInvoiceId(productInvoice.Id).ToList();
                        if (model.InvoiceList.Any(x => x.Id > 0))
                        {
                            var update = _listdata.Where(id1 => model.InvoiceList.ToList().Any(id2 => id2.Id == id1.Id)).ToList();
                            //lưu danh sách thao tác thực hiện dịch vụ
                            foreach (var item in model.InvoiceList)
                            {
                                var _update = update.FirstOrDefault(x => x.Id == item.Id);
                                _update.Discount = item.Discount;
                                _update.DiscountAmount = item.DiscountAmount;

                                productInvoiceRepository.UpdateProductInvoiceDetail(_update);
                            }
                        }


                        //begin hoapd tinh lai hoa don 
                        _listdata = productInvoiceRepository.GetAllInvoiceDetailsByInvoiceId(productInvoice.Id).ToList();
                        decimal Tongtien = 0;
                        foreach (var item in _listdata)
                        {
                            if (item.is_TANG != 1)
                            {
                                Tongtien = Tongtien + Math.Round(Helpers.Common.NVL_NUM_DECIMAL_NEW(item.Quantity * item.Price)) - Helpers.Common.NVL_NUM_DECIMAL_NEW(item.DiscountAmount);
                            }
                        }
                        decimal TongTyleGiamgia = Helpers.Common.NVL_NUM_DECIMAL_NEW(productInvoice.Discount_DB) + Helpers.Common.NVL_NUM_DECIMAL_NEW(productInvoice.Discount_KM) + Helpers.Common.NVL_NUM_DECIMAL_NEW(productInvoice.Discount_VIP);
                        decimal TongtienGiam = Math.Round((Tongtien / 100) * TongTyleGiamgia);
                        Tongtien = Tongtien - TongtienGiam;
                        Tongtien = Math.Round(Tongtien + (Helpers.Common.NVL_NUM_DECIMAL_NEW(productInvoice.TaxFee) * (Tongtien / 100)));

                        if ((productInvoice != null) && (productInvoice.Id > 0))
                        {
                            //Nếu đã ghi sổ rồi thì không được sửa
                            productInvoice = productInvoiceRepository.GetProductInvoiceById(productInvoice.Id);
                            if (productInvoice.IsArchive)
                            {
                                return RedirectToAction("Detail", new { Id = productInvoice.Id });
                            }

                            int productInvoiceID = productInvoice.Id;

                            productInvoice.Type = "invoice";
                            productInvoice.IsReturn = false;
                            productInvoice.Id = productInvoiceID;
                            productInvoice.ModifiedUserId = WebSecurity.CurrentUserId;
                            productInvoice.ModifiedDate = DateTime.Now;
                            productInvoice.RemainingAmount = Tongtien;
                            productInvoice.DiscountAmount = TongtienGiam;
                            productInvoice.Discount = (double)TongTyleGiamgia;
                            productInvoice.PaidAmount = 0;
                            productInvoice.TotalAmount = Tongtien;
                            productInvoiceRepository.UpdateProductInvoice(productInvoice);

                            //ghi log 
                            Erp.BackOffice.Controllers.HomeController.WriteLog(productInvoice.Id, productInvoice.Code, "đã thêm khuyến mãi cho đơn hàng", "ProductInvoice/Detail/" + productInvoice.Id, Helpers.Common.CurrentUser.BranchId.Value);

                        }


                        //begin neu da co phai thu khach hang thi phai cap nhat lai so tien phai thu
                        var transaction_Liablities = transactionLiabilitiesRepository.GetAllTransaction().Where(x => x.BranchId == Helpers.Common.CurrentUser.BranchId.Value && x.MaChungTuGoc == productInvoice.Code && x.LoaiChungTuGoc == "ProductInvoice" && x.TransactionModule == "ProductInvoice" && x.TargetModule == "Customer").ToList();
                        if (transaction_Liablities.Count() > 0)
                        {
                            transaction_Liablities[0].Debit = Tongtien;
                            transactionLiabilitiesRepository.UpdateTransaction(transaction_Liablities[0]);
                        }
                        //end neu da co phai thu khach hang thi phai cap nhat lai so tien phai thu

                        //end hoapd tinh lai hoa don






                    }


                    //Xóa ct km cũ
                    var data = logPromotionRepository.GetAllLogPromotion().Where(x => x.ProductInvoiceCode == model.Code).ToList();
                    foreach (var item in data)
                    {
                        item.IsDeleted = true;
                        logPromotionRepository.UpdateLogPromotion(item);
                    }

                    foreach (var item in model.LogCommission.Where(x => x.CheckSave.HasValue))
                    {
                        var insr = new LogPromotion();

                        AutoMapper.Mapper.Map(item, insr);

                        //if (item.Type == "invoice_discountdif")
                        //{
                        //    insr.Name = "Chương trình khuyến mãi khác";
                        //}
                        insr.ModifiedUserId = WebSecurity.CurrentUserId;
                        insr.ModifiedDate = DateTime.Now;
                        insr.CreatedUserId = WebSecurity.CurrentUserId;
                        insr.CreatedDate = DateTime.Now;
                        insr.IsDeleted = false;
                        logPromotionRepository.InsertLogPromotion(insr);
                        insr.Code = Erp.BackOffice.Helpers.Common.GetOrderNo("LogPromotion");
                        logPromotionRepository.UpdateLogPromotion(insr);
                        Erp.BackOffice.Helpers.Common.SetOrderNo("LogPromotion");
                    }

                    scope.Complete();
                }
                catch (DbUpdateException)
                {
                    TempData[Globals.FailedMessageKey] = App_GlobalResources.Wording.Error;
                    return View(model);
                }
            }
            return RedirectToAction("Detail", new { Id = productInvoice.Id });
        }

        #region AddPromotionProduct dựa theo số lượng sản phẩm trong hóa đơn.. kiểm tra điều kiện khuyến mãi.. lấy ra danh sách khuyến mãi cái đặt thỏa yêu cầu
        public List<vwCommisionCustomer> AddPromotionProduct(List<ProductInvoiceDetailViewModel> InvoiceList, List<vwCommisionCustomer> detail)
        {
            var list1 = detail.Where(x => x.Symbol == ">").ToList();
            var list2 = detail.Where(x => x.Symbol == ">=").ToList();
            var list3 = detail.Where(x => x.Symbol == "=").ToList();
            var list4 = detail.Where(x => x.Symbol == "<").ToList();
            var list5 = detail.Where(x => x.Symbol == "<=").ToList();
            var commission_product = new List<vwCommisionCustomer>();

            if (list1.Any())
            {
                var aa = list1.Where(id1 => InvoiceList.Any(id2 => id2.Quantity > id1.Quantity && id2.ProductId == id1.ProductId)).ToList();
                if (aa.Any())
                    commission_product.AddRange(aa);
            }
            if (list2.Any())
            {
                var aa1 = list2.Where(id1 => InvoiceList.Any(id2 => id2.Quantity >= id1.Quantity && id2.ProductId == id1.ProductId)).ToList();
                if (aa1.Any())
                    commission_product.AddRange(aa1);
            }
            if (list3.Any())
            {
                var aa2 = list3.Where(id1 => InvoiceList.Any(id2 => id2.Quantity == id1.Quantity && id2.ProductId == id1.ProductId)).ToList();
                if (aa2.Any())
                    commission_product.AddRange(aa2);
            }
            if (list4.Any())
            {
                var aa3 = list4.Where(id1 => InvoiceList.Any(id2 => id2.Quantity < id1.Quantity && id2.ProductId == id1.ProductId)).ToList();
                if (aa3.Any())
                    commission_product.AddRange(aa3);
            }
            if (list5.Any())
            {
                var aa4 = list5.Where(id1 => InvoiceList.Any(id2 => id2.Quantity <= id1.Quantity && id2.ProductId == id1.ProductId)).ToList();
                if (aa4.Any())
                    commission_product.AddRange(aa4);
            }

            return commission_product;
        }

        #endregion

        #region AddPromotionInvoice dựa theo tổng tiền trong hóa đơn.. kiểm tra điều kiện khuyến mãi.. lấy ra danh sách khuyến mãi cái đặt thỏa yêu cầu
        public List<CommisionInvoice> AddPromotionInvoice(List<CommisionInvoice> invoice_detail, vwProductInvoice productInvoice)
        {

            var list31 = invoice_detail.Where(x => x.StartSymbol == "=" && x.EndSymbol == "<" && x.StartAmount != null && x.EndAmount != null).ToList();
            var list32 = invoice_detail.Where(x => x.StartSymbol == "=" && x.EndSymbol == "<=" && x.StartAmount != null && x.EndAmount != null).ToList();
            var list35 = invoice_detail.Where(x => x.StartSymbol == "=" && x.EndSymbol == "=" && x.StartAmount != null && x.EndAmount != null).ToList();

            var list41 = invoice_detail.Where(x => x.StartSymbol == "<" && x.EndSymbol == "<" && x.StartAmount != null && x.EndAmount != null).ToList();
            var list42 = invoice_detail.Where(x => x.StartSymbol == "<" && x.EndSymbol == "<=" && x.StartAmount != null && x.EndAmount != null).ToList();
            var list45 = invoice_detail.Where(x => x.StartSymbol == "<" && x.EndSymbol == "=" && x.StartAmount != null && x.EndAmount != null).ToList();

            var list51 = invoice_detail.Where(x => x.StartSymbol == "<=" && x.EndSymbol == "<" && x.StartAmount != null && x.EndAmount != null).ToList();
            var list52 = invoice_detail.Where(x => x.StartSymbol == "<=" && x.EndSymbol == "<=" && x.StartAmount != null && x.EndAmount != null).ToList();
            var list55 = invoice_detail.Where(x => x.StartSymbol == "<=" && x.EndSymbol == "=" && x.StartAmount != null && x.EndAmount != null).ToList();

            var list21 = invoice_detail.Where(x => x.StartSymbol == "=" && x.StartAmount != null && x.EndAmount == null).ToList();
            var list22 = invoice_detail.Where(x => x.StartSymbol == "<" && x.StartAmount != null && x.EndAmount == null).ToList();
            var list23 = invoice_detail.Where(x => x.StartSymbol == "<=" && x.StartAmount != null && x.EndAmount == null).ToList();

            var list11 = invoice_detail.Where(x => x.EndSymbol == "=" && x.StartAmount == null && x.EndAmount != null).ToList();
            var list12 = invoice_detail.Where(x => x.EndSymbol == "<" && x.StartAmount == null && x.EndAmount != null).ToList();
            var list13 = invoice_detail.Where(x => x.EndSymbol == "<=" && x.StartAmount == null && x.EndAmount != null).ToList();

            //
            var listdiffer = invoice_detail.Where(x => x.Type == "discountdif").ToList();
            //
            var commission_invoice = new List<CommisionInvoice>();

            #region check ==
            if (list31.Any())
            {
                var aa = list31.Where(x => x.StartAmount == productInvoice.TotalAmount && productInvoice.TotalAmount < x.EndAmount).ToList();
                if (aa.Any())
                    commission_invoice.AddRange(aa);
            }

            if (list32.Any())
            {
                var aa = list32.Where(x => x.StartAmount == productInvoice.TotalAmount && productInvoice.TotalAmount <= x.EndAmount).ToList();
                if (aa.Any())
                    commission_invoice.AddRange(aa);
            }
            if (list35.Any())
            {
                var aa = list35.Where(x => x.StartAmount == productInvoice.TotalAmount && productInvoice.TotalAmount == x.EndAmount).ToList();
                if (aa.Any())
                    commission_invoice.AddRange(aa);
            }
            if (listdiffer.Any())
            {
                commission_invoice.AddRange(listdiffer);
            }
            #endregion

            #region check <
            if (list41.Any())
            {
                var aa = list41.Where(x => x.StartAmount < productInvoice.TotalAmount && productInvoice.TotalAmount < x.EndAmount).ToList();
                if (aa.Any())
                    commission_invoice.AddRange(aa);
            }

            if (list42.Any())
            {
                var aa = list42.Where(x => x.StartAmount < productInvoice.TotalAmount && productInvoice.TotalAmount <= x.EndAmount).ToList();
                if (aa.Any())
                    commission_invoice.AddRange(aa);
            }

            if (list45.Any())
            {
                var aa = list45.Where(x => x.StartAmount < productInvoice.TotalAmount && productInvoice.TotalAmount == x.EndAmount).ToList();
                if (aa.Any())
                    commission_invoice.AddRange(aa);
            }
            #endregion

            #region check <=
            if (list51.Any())
            {
                var aa = list51.Where(x => x.StartAmount <= productInvoice.TotalAmount && productInvoice.TotalAmount < x.EndAmount).ToList();
                if (aa.Any())
                    commission_invoice.AddRange(aa);
            }

            if (list52.Any())
            {
                var aa = list52.Where(x => x.StartAmount <= productInvoice.TotalAmount && productInvoice.TotalAmount <= x.EndAmount).ToList();
                if (aa.Any())
                    commission_invoice.AddRange(aa);
            }

            if (list55.Any())
            {
                var aa = list55.Where(x => x.StartAmount <= productInvoice.TotalAmount && productInvoice.TotalAmount == x.EndAmount).ToList();
                if (aa.Any())
                    commission_invoice.AddRange(aa);
            }
            #endregion

            #region check EndAmount null
            if (list21.Any())
            {
                var aa = list21.Where(x => x.StartAmount == productInvoice.TotalAmount).ToList();
                if (aa.Any())
                    commission_invoice.AddRange(aa);
            }

            if (list22.Any())
            {
                var aa = list22.Where(x => x.StartAmount < productInvoice.TotalAmount).ToList();
                if (aa.Any())
                    commission_invoice.AddRange(aa);
            }

            if (list23.Any())
            {
                var aa = list23.Where(x => x.StartAmount <= productInvoice.TotalAmount).ToList();
                if (aa.Any())
                    commission_invoice.AddRange(aa);
            }
            #endregion

            #region check StartAmount null
            if (list11.Any())
            {
                var aa = list11.Where(x => productInvoice.TotalAmount == x.EndAmount).ToList();
                if (aa.Any())
                    commission_invoice.AddRange(aa);
            }

            if (list12.Any())
            {
                var aa = list12.Where(x => productInvoice.TotalAmount < x.EndAmount).ToList();
                if (aa.Any())
                    commission_invoice.AddRange(aa);
            }

            if (list13.Any())
            {
                var aa = list13.Where(x => productInvoice.TotalAmount <= x.EndAmount).ToList();
                if (aa.Any())
                    commission_invoice.AddRange(aa);
            }
            #endregion

            return commission_invoice;
        }
        #endregion
        #endregion

        #region Archive Đặt cọc, xuất kho cho đơn hàng

        [HttpPost]
        public ActionResult Archive(ProductInvoiceViewModel model, string GHICHU)
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
                    var productInvoice = productInvoiceRepository.GetProductInvoiceById(model.Id);
                    var invoice_detail_list = productInvoiceRepository.GetAllvwInvoiceDetailsByInvoiceId(productInvoice.Id).ToList();

                    if (model.ReceiptViewModel.Amount > productInvoice.RemainingAmount)
                    {
                        TempData[Globals.FailedMessageKey] = "Tiền đặt cọc không được lớn hơn tiền nợ là " + CommonSatic.ToCurrencyStr(productInvoice.RemainingAmount, null); ;
                        return RedirectToAction("Detail", new { Id = model.Id });
                    }

                    if (productInvoice.Status != "Đặt cọc")
                    {


                        ProductOutboundViewModel productOutboundViewModel = new ProductOutboundViewModel();
                        var warehouseDefault = warehouseRepository.GetAllWarehouse().Where(x => x.IsSale == true && x.BranchId == intBrandID).FirstOrDefault();

                        if (warehouseDefault == null)
                        {
                            TempData[Globals.FailedMessageKey] += "<br/>Không tìm thấy kho xuất bán! Vui lòng kiểm tra lại!";
                            return RedirectToAction("Detail", new { Id = model.Id });
                        }
                        string check = "";
                        foreach (var item in invoice_detail_list)
                        {
                            var error = InventoryController.Check(item.ProductName, item.ProductId.Value, item.LoCode, item.ExpiryDate, warehouseDefault.Id, 0, item.Quantity.Value);
                            check += error;
                        }
                        if (!string.IsNullOrEmpty(check))
                        {
                            TempData[Globals.FailedMessageKey] = App_GlobalResources.Wording.ArchiveFail + check;
                            return RedirectToAction("Detail", new { Id = model.Id });
                        }
                        // -- tiendev--trao đổi lại với chị Phương, xuất kho
                        //#region  phiếu xuất ok
                        //var product_outbound = productOutboundRepository.GetAllProductOutboundFull().Where(x => x.InvoiceId == productInvoice.Id).ToList();

                        ////insert mới
                        //if (product_outbound.Count() <= 0)
                        //{
                        //    #region  thêm mới phiếu xuất

                        //    //Nếu trong đơn hàng có sản phẩm thì xuất kho
                        //    if (warehouseDefault != null)
                        //    {
                        //        productOutboundViewModel.InvoiceId = productInvoice.Id;
                        //        productOutboundViewModel.InvoiceCode = productInvoice.Code;
                        //        productOutboundViewModel.WarehouseSourceId = warehouseDefault.Id;
                        //        productOutboundViewModel.Note = "Xuất kho cho đơn hàng " + productInvoice.Code;
                        //        var DetailList = invoice_detail_list.Where(x => x.ProductType == "product").Select(x =>
                        //              new ProductInvoiceDetailViewModel
                        //              {
                        //                  Id = x.Id,
                        //                  Price = x.Price,
                        //                  ProductId = x.ProductId,
                        //                  Quantity = x.Quantity,
                        //                  Unit = x.Unit,
                        //                  ProductType = x.ProductType,
                        //                  CategoryCode = x.CategoryCode,
                        //                  ProductInvoiceCode = x.ProductInvoiceCode,
                        //                  ProductName = x.ProductName,
                        //                  ProductCode = x.ProductCode,
                        //                  ProductInvoiceId = x.ProductInvoiceId,
                        //                  ProductGroup = x.ProductGroup,
                        //                  LoCode = x.LoCode,
                        //                  ExpiryDate = x.ExpiryDate,
                        //                  Amount = x.Amount,
                        //                  Discount = x.Discount,
                        //                  DiscountAmount = x.DiscountAmount
                        //              }).ToList();
                        //        //Lấy dữ liệu cho chi tiết
                        //        productOutboundViewModel.DetailList = new List<ProductOutboundDetailViewModel>();
                        //        AutoMapper.Mapper.Map(DetailList, productOutboundViewModel.DetailList);

                        //        var productOutbound = ProductOutboundController.AutoCreateProductOutboundFromProductInvoice(productOutboundRepository, productOutboundViewModel, productInvoice.Code);

                        //        //Ghi sổ chứng từ phiếu xuất
                        //        // ProductOutboundController.Archive(productOutbound, TempData);
                        //    }
                        //    #endregion
                        //}
                        //else
                        //{
                        //    #region   cập nhật phiếu xuất kho
                        //    //xóa chi tiết phiếu xuất, insert chi tiết mới
                        //    //cập nhật lại tổng tiền, trạng thái phiếu xuất
                        //    for (int i = 0; i < product_outbound.Count(); i++)
                        //    {
                        //        var outbound_detail = productOutboundRepository.GetAllProductOutboundDetailByOutboundId(product_outbound[i].Id).ToList();
                        //        //xóa
                        //        for (int ii = 0; ii < outbound_detail.Count(); ii++)
                        //        {
                        //            productOutboundRepository.DeleteProductOutboundDetail(outbound_detail[ii].Id);
                        //        }
                        //        var insert_ = invoice_detail_list.Where(x => x.ProductType == "product").ToList();
                        //        //insert mới
                        //        for (int iii = 0; iii < insert_.Count(); iii++)
                        //        {
                        //            ProductOutboundDetail productOutboundDetail = new ProductOutboundDetail();
                        //            productOutboundDetail.Price = insert_[iii].Price;
                        //            productOutboundDetail.ProductId = insert_[iii].ProductId;
                        //            productOutboundDetail.Quantity = insert_[iii].Quantity;
                        //            productOutboundDetail.Unit = insert_[iii].Unit;
                        //            productOutboundDetail.LoCode = insert_[iii].LoCode;
                        //            productOutboundDetail.ExpiryDate = insert_[iii].ExpiryDate;
                        //            productOutboundDetail.IsDeleted = false;
                        //            productOutboundDetail.CreatedUserId = WebSecurity.CurrentUserId;
                        //            productOutboundDetail.ModifiedUserId = WebSecurity.CurrentUserId;
                        //            productOutboundDetail.CreatedDate = DateTime.Now;
                        //            productOutboundDetail.ModifiedDate = DateTime.Now;
                        //            productOutboundDetail.ProductOutboundId = product_outbound[i].Id;
                        //            productOutboundRepository.InsertProductOutboundDetail(productOutboundDetail);

                        //        }
                        //        product_outbound[i].TotalAmount = invoice_detail_list.Where(x => x.ProductType == "product").Sum(x => (x.Price * x.Quantity));

                        //        //Ghi sổ chứng từ phiếu xuất
                        //        //ProductOutboundController.Archive(product_outbound[i], TempData);
                        //    }
                        //    #endregion
                        //}
                        //#endregion

                        #region  xóa hết lịch sử giao dịch
                        //xóa lịch sử giao dịch có liên quan đến đơn hàng, gồm: 1 dòng giao dịch bán hàng, 1 dòng thu tiền.
                        var transaction_Liablities = transactionLiabilitiesRepository.GetAllTransaction().Where(x => x.BranchId == intBrandID && x.MaChungTuGoc == productInvoice.Code && x.LoaiChungTuGoc == "ProductInvoice").ToList();
                        if (transaction_Liablities.Count() > 0)
                        {
                            for (int i = 0; i < transaction_Liablities.Count(); i++)
                            {
                                transactionLiabilitiesRepository.DeleteTransaction(transaction_Liablities[i].Id);
                            }
                        }
                        #endregion

                        if (!productInvoice.IsArchive)
                        {
                            #region  Cập nhật thông tin thanh toán cho đơn hàng
                            //Cập nhật thông tin thanh toán cho đơn hàng
                            productInvoice.PaymentMethod = model.ReceiptViewModel.PaymentMethod;
                            productInvoice.PaidAmount += Convert.ToDecimal(model.ReceiptViewModel.Amount);
                            productInvoice.DoanhThu = productInvoice.PaidAmount;
                            productInvoice.RemainingAmount = productInvoice.TotalAmount - productInvoice.PaidAmount;
                            productInvoice.NextPaymentDate = model.NextPaymentDate_Temp;

                            productInvoice.ModifiedDate = DateTime.Now;
                            productInvoice.ModifiedUserId = WebSecurity.CurrentUserId;
                            productInvoiceRepository.UpdateProductInvoice(productInvoice);

                            //Lấy mã KH
                            var customer = customerRepository.GetCustomerById(productInvoice.CustomerId.Value);

                            var remain = productInvoice.TotalAmount - Convert.ToDecimal(model.ReceiptViewModel.Amount.Value);
                            if (remain > 0)
                            {
                            }
                            else
                            {
                                productInvoice.NextPaymentDate = null;
                                model.NextPaymentDate_Temp = null;
                            }
                            #endregion

                            #region thêm lịch sử bán hàng
                            //Ghi Nợ TK 131 - Phải thu của khách hàng (tổng giá thanh toán)
                            Erp.BackOffice.Account.Controllers.TransactionLiabilitiesController.Create(
                                    intBrandID,
                                    productInvoice.Code,
                                    "ProductInvoice",
                                    "Phải thu của khách hàng cho đơn hàng " + productInvoice.Code,
                                    customer.Code,
                                    "Customer",
                                    productInvoice.TotalAmount,
                                    0,
                                    productInvoice.Code,
                                    "ProductInvoice",
                                    null,
                                    model.NextPaymentDate_Temp,
                                    null);
                            #endregion

                            #region phiếu thu
                            //Khách thanh toán ngay
                            if (model.ReceiptViewModel.Amount > 0)
                            {
                                #region xóa phiếu thu cũ (nếu có)
                                var receipt = ReceiptRepository.GetAllReceiptFull()
                                    .Where(item => item.MaChungTuGoc == productInvoice.Code).ToList();
                                var receipt_detail = receiptDetailRepository.GetAllReceiptDetailFull().ToList();
                                receipt_detail = receipt_detail.Where(x => x.MaChungTuGoc == productInvoice.Code).ToList();
                                if (receipt_detail.Count() > 0)
                                {
                                    // isDelete chi tiết phiếu thu
                                    for (int i = 0; i < receipt_detail.Count(); i++)
                                    {
                                        receiptDetailRepository.DeleteReceiptDetail(receipt_detail[i].Id);
                                    }
                                }
                                #endregion
                                if (receipt.Count() > 0)
                                {
                                    #region cập nhật phiếu thu cũ
                                    // isDelete phiếu thu
                                    var receipts = receipt.FirstOrDefault();
                                    receipts.IsDeleted = false;
                                    receipts.Payer = customer.LastName + " " + customer.FirstName;
                                    receipts.PaymentMethod = productInvoice.PaymentMethod;
                                    receipts.ModifiedDate = DateTime.Now;
                                    receipts.VoucherDate = DateTime.Now;
                                    receipts.Amount = model.ReceiptViewModel.Amount;
                                    if (receipts.Amount > productInvoice.TotalAmount)
                                        receipts.Amount = productInvoice.TotalAmount;

                                    ReceiptRepository.UpdateReceipt(receipts);

                                    //ghi log 
                                    Erp.BackOffice.Controllers.HomeController.WriteLog(receipts.Id, receipts.Code, "đã cập nhật phiếu thu", "Receipt/Detail/" + receipts.Id, Helpers.Common.CurrentUser.BranchId.Value);

                                    // Thêm vào quản lý chứng từ
                                    TransactionController.Create(new TransactionViewModel
                                    {
                                        TransactionModule = "Receipt",
                                        TransactionCode = receipts.Code,
                                        TransactionName = "Thu tiền đặt cọc khách hàng cho đơn hàng " + productInvoice.Code
                                    });

                                    // Thêm chứng từ liên quan
                                    TransactionController.CreateRelationship(new TransactionRelationshipViewModel
                                    {
                                        TransactionA = receipts.Code,
                                        TransactionB = productInvoice.Code
                                    });

                                    //Ghi Có TK 131 - Phải thu của khách hàng.
                                    Erp.BackOffice.Account.Controllers.TransactionLiabilitiesController.Create(
                                        intBrandID,
                                        receipts.Code,
                                        "Receipt",
                                        "Thu tiền đặt cọc khách hàng cho đơn hàng " + productInvoice.Code,
                                        customer.Code,
                                        "Customer",
                                        0,
                                        Convert.ToDecimal(model.ReceiptViewModel.Amount),
                                        productInvoice.Code,
                                        "ProductInvoice",
                                        model.ReceiptViewModel.PaymentMethod,
                                        null,
                                        GHICHU);
                                    #endregion
                                }
                                else
                                {
                                    #region thêm mới phiếu thu
                                    //Lập phiếu thu
                                    var receipt_inser = new Receipt();
                                    AutoMapper.Mapper.Map(model.ReceiptViewModel, receipt_inser);
                                    receipt_inser.IsDeleted = false;
                                    receipt_inser.CreatedUserId = WebSecurity.CurrentUserId;
                                    receipt_inser.ModifiedUserId = WebSecurity.CurrentUserId;
                                    receipt_inser.AssignedUserId = WebSecurity.CurrentUserId;
                                    receipt_inser.CreatedDate = DateTime.Now;
                                    receipt_inser.ModifiedDate = DateTime.Now;
                                    receipt_inser.VoucherDate = DateTime.Now;
                                    receipt_inser.CustomerId = customer.Id;
                                    receipt_inser.Payer = customer.LastName + " " + customer.FirstName;
                                    receipt_inser.PaymentMethod = productInvoice.PaymentMethod;
                                    receipt_inser.Address = customer.Address;
                                    receipt_inser.MaChungTuGoc = productInvoice.Code;
                                    receipt_inser.LoaiChungTuGoc = "ProductInvoice";
                                    receipt_inser.Note = "Thu tiền đặt cọc khách hàng cho đơn hàng " + productInvoice.Code;
                                    receipt_inser.BranchId = intBrandID.Value;

                                    if (receipt_inser.Amount > productInvoice.TotalAmount)
                                        receipt_inser.Amount = productInvoice.TotalAmount;

                                    ReceiptRepository.InsertReceipt(receipt_inser);
                                    receipt_inser.Code = Erp.BackOffice.Helpers.Common.GetOrderNo("Receipt");
                                    ReceiptRepository.UpdateReceipt(receipt_inser);
                                    Erp.BackOffice.Helpers.Common.SetOrderNo("Receipt");

                                    //ghi log 
                                    Erp.BackOffice.Controllers.HomeController.WriteLog(receipt_inser.Id, receipt_inser.Code, "đã tạo phiếu thu", "Receipt/Detail/" + receipt_inser.Id, Helpers.Common.CurrentUser.BranchId.Value);

                                    //Thêm vào quản lý chứng từ
                                    TransactionController.Create(new TransactionViewModel
                                    {
                                        TransactionModule = "Receipt",
                                        TransactionCode = receipt_inser.Code,
                                        TransactionName = "Thu tiền đặt cọc khách hàng cho đơn hàng " + productInvoice.Code
                                    });

                                    //Thêm chứng từ liên quan
                                    TransactionController.CreateRelationship(new TransactionRelationshipViewModel
                                    {
                                        TransactionA = receipt_inser.Code,
                                        TransactionB = productInvoice.Code
                                    });

                                    //Ghi Có TK 131 - Phải thu của khách hàng.
                                    Erp.BackOffice.Account.Controllers.TransactionLiabilitiesController.Create(
                                        intBrandID,
                                        receipt_inser.Code,
                                        "Receipt",
                                        "Thu tiền đặt cọc khách hàng cho đơn hàng " + productInvoice.Code,
                                        customer.Code,
                                        "Customer",
                                        0,
                                        Convert.ToDecimal(model.ReceiptViewModel.Amount),
                                        productInvoice.Code,
                                        "ProductInvoice",
                                        model.ReceiptViewModel.PaymentMethod,
                                        null,
                                        GHICHU);

                                    #endregion
                                }
                            }
                            #endregion

                            #region cập nhật đơn bán hàng
                            //Cập nhật đơn hàng
                            productInvoice.ModifiedUserId = WebSecurity.CurrentUserId;
                            productInvoice.ModifiedDate = DateTime.Now;
                            productInvoice.IsArchive = false;
                            productInvoice.Status = App_GlobalResources.Wording.Status_deposit;
                            productInvoiceRepository.UpdateProductInvoice(productInvoice);


                            //ghi log 
                            Erp.BackOffice.Controllers.HomeController.WriteLog(productInvoice.Id, productInvoice.Code, "đã thu tiền cọc cho đơn hàng", "ProductInvoice/Detail/" + productInvoice.Id, Helpers.Common.CurrentUser.BranchId.Value);

                            TempData[Globals.SuccessMessageKey] = App_GlobalResources.Wording.DepositSuccess;

                            TempData[Globals.SuccessMessageKey] += "<br/>Đơn hàng này đã được xuất kho ở dạng treo ! Vui lòng kiểm tra lại chứng từ xuất kho để tránh sai sót dữ liệu!";
                            #endregion

                            //#region tạo phiếu membership
                            //if (invoice_detail_list.Where(x => x.ProductType == "service").Any())
                            //{
                            //    var insert_Service = invoice_detail_list.Where(x => x.ProductType == "service").ToList();
                            //    foreach (var item in insert_Service)
                            //    {
                            //        for (int i = 0; i < item.Quantity; i++)
                            //        {
                            //            var ins = new Erp.Domain.Sale.Entities.Membership();
                            //            ins.IsDeleted = false;
                            //            ins.CreatedUserId = WebSecurity.CurrentUserId;
                            //            ins.ModifiedUserId = WebSecurity.CurrentUserId;
                            //            ins.AssignedUserId = WebSecurity.CurrentUserId;
                            //            ins.CreatedDate = DateTime.Now;
                            //            ins.ModifiedDate = DateTime.Now;
                            //            ins.Status = "pending";
                            //            ins.BranchId = productInvoice.BranchId;
                            //            ins.CustomerId = productInvoice.CustomerId;
                            //            ins.ExpiryDate = item.ExpiryDate;
                            //            ins.ProductId = item.ProductId;
                            //            ins.TargetId = item.Id;
                            //            ins.TargetModule = "ProductInvoiceDetail";
                            //            ins.TargetCode = productInvoice.Code;
                            //            membershipRepository.InsertMembership(ins);
                            //            ins.Code = Erp.BackOffice.Helpers.Common.GetOrderNo("Membership");
                            //            membershipRepository.UpdateMembership(ins);
                            //            Erp.BackOffice.Helpers.Common.SetOrderNo("Membership");
                            //        }
                            //    }
                            //}
                            //#endregion
                        }
                        scope.Complete();
                    }
                    else
                    {
                        TempData[Globals.SuccessMessageKey] += "<br/>Đơn hàng đã được đặt cọc trước đó, không thể tiếp tục đăt cọc nữa!";
                    }
                }
                catch (DbUpdateException)
                {
                    return Content("Fail");
                }
            }
            return RedirectToAction("Detail", new { Id = model.Id });
        }
        #endregion

        #region Chuyển tiền
        [HttpPost]
        public ActionResult MoneyMove(ProductInvoiceViewModel model)
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

            var productInvoice = productInvoiceRepository.GetProductInvoiceById(model.Id);
            var id = Request["ProductInvoiceOldId"];
            if (Erp.BackOffice.Helpers.Common.CurrentUser.BranchId == null)
            {
                TempData[Globals.FailedMessageKey] = "Bạn không quyền nhận tiền chuyển cho đơn hàng, phải đăng nhập vào tài khoản chi nhánh!";

                //return RedirectToAction("Create");
                return RedirectToAction("Detail", new { Id = model.Id });
            }

            vwProductInvoice vwOldProductInvoice = new vwProductInvoice();
            vwProductInvoice productInvoiceOLD = new vwProductInvoice();
            //begin kiểm tra số tiền nhận không được lớn hơn giá trị đơn hàng cũ
            if (model.ProductInvoiceOldCode != null)
            {
                vwOldProductInvoice = productInvoiceRepository.GetvwProductInvoiceByCode(Erp.BackOffice.Helpers.Common.CurrentUser.BranchId.Value, model.ProductInvoiceOldCode);
                model.ProductInvoiceOldId = vwOldProductInvoice.Id;

            }
            if (model.ProductInvoiceOldId > 0)
            {
                productInvoiceOLD = productInvoiceRepository.GetvwProductInvoiceFullById(model.ProductInvoiceOldId.Value);

                if (model.TotalAmountMoneyMove > productInvoiceOLD.TotalCredit) //model.TotalAmountMoneyMove > productInvoiceOLD.PaidAmount || productInvoiceOLD.tiendathu 
                {
                    TempData[Globals.FailedMessageKey] = "Số tiền chuyển vượt quá số tiền đã thu trong đơn hàng cũ, xin vui lòng kiểm tra lại !";

                    return RedirectToAction("Detail", new { Id = model.Id });
                }
                //if(productInvoiceOLD.TotalAmount > productInvoice.TotalAmount)
                //{
                //    TempData[Globals.FailedMessageKey] = "Đơn hàng cũ phải có giá trị nhỏ hơn đơn hàng mới";

                //    return RedirectToAction("Detail", new { Id = model.Id });
                //}

                //kiểm tra số tiền chuyển > nợ
                if (model.TotalAmountMoneyMove > model.tienconno) //model.TotalAmountMoneyMove > model.RemainingAmount ||
                {
                    TempData[Globals.FailedMessageKey] = "Số tiền chuyển vượt quá số tiền nợ của đơn mới: " + CommonSatic.ToCurrencyStr(model.RemainingAmount, null);

                    return RedirectToAction("Detail", new { Id = model.Id });
                }
            }

            //end kiểm tra số tiền nhận không được lớn hơn giá trị đơn hàng cũ và không vượt quá số tiền nợ




            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.Snapshot }))
            {
                try
                {

                    //begin su ly nhu nhan dat coc


                    if (productInvoice.Status == "pending" || productInvoice.Status.Contains("Đặt cọc"))
                    {

                        var invoice_detail_list = productInvoiceRepository.GetAllvwInvoiceDetailsByInvoiceId(productInvoice.Id).ToList();
                        ProductOutboundViewModel productOutboundViewModel = new ProductOutboundViewModel();
                        var warehouseDefault = warehouseRepository.GetAllWarehouse().Where(x => x.IsSale == true && x.BranchId == intBrandID).FirstOrDefault();

                        if (warehouseDefault == null)
                        {
                            TempData[Globals.FailedMessageKey] += "<br/>Không tìm thấy kho xuất bán! Vui lòng kiểm tra lại!";
                            return RedirectToAction("Detail", new { Id = model.Id });
                        }
                        string check = "";
                        foreach (var item in invoice_detail_list)
                        {
                            var error = InventoryController.Check(item.ProductName, item.ProductId.Value, item.LoCode, item.ExpiryDate, warehouseDefault.Id, 0, item.Quantity.Value);
                            check += error;
                        }
                        if (!string.IsNullOrEmpty(check))
                        {
                            TempData[Globals.FailedMessageKey] = App_GlobalResources.Wording.ArchiveFail + check;
                            return RedirectToAction("Detail", new { Id = model.Id });
                        }

                        //#region  phiếu xuất ok
                        //var product_outbound = productOutboundRepository.GetAllProductOutboundFull().Where(x => x.InvoiceId == productInvoice.Id).ToList();

                        ////insert mới
                        //if (product_outbound.Count() <= 0)
                        //{
                        //    #region  thêm mới phiếu xuất

                        //    //Nếu trong đơn hàng có sản phẩm thì xuất kho
                        //    if (warehouseDefault != null)
                        //    {
                        //        productOutboundViewModel.InvoiceId = productInvoice.Id;
                        //        productOutboundViewModel.InvoiceCode = productInvoice.Code;
                        //        productOutboundViewModel.WarehouseSourceId = warehouseDefault.Id;
                        //        productOutboundViewModel.Note = "Xuất kho cho đơn hàng " + productInvoice.Code;
                        //        var DetailList = invoice_detail_list.Where(x => x.ProductType == "product").Select(x =>
                        //              new ProductInvoiceDetailViewModel
                        //              {
                        //                  Id = x.Id,
                        //                  Price = x.Price,
                        //                  ProductId = x.ProductId,
                        //                  Quantity = x.Quantity,
                        //                  Unit = x.Unit,
                        //                  ProductType = x.ProductType,
                        //                  CategoryCode = x.CategoryCode,
                        //                  ProductInvoiceCode = x.ProductInvoiceCode,
                        //                  ProductName = x.ProductName,
                        //                  ProductCode = x.ProductCode,
                        //                  ProductInvoiceId = x.ProductInvoiceId,
                        //                  ProductGroup = x.ProductGroup,
                        //                  LoCode = x.LoCode,
                        //                  ExpiryDate = x.ExpiryDate,
                        //                  Amount = x.Amount,
                        //                  Discount = x.Discount,
                        //                  DiscountAmount = x.DiscountAmount
                        //              }).ToList();
                        //        //Lấy dữ liệu cho chi tiết
                        //        productOutboundViewModel.DetailList = new List<ProductOutboundDetailViewModel>();
                        //        AutoMapper.Mapper.Map(DetailList, productOutboundViewModel.DetailList);

                        //        var productOutbound = ProductOutboundController.AutoCreateProductOutboundFromProductInvoice(productOutboundRepository, productOutboundViewModel, productInvoice.Code);

                        //        //Ghi sổ chứng từ phiếu xuất
                        //        // ProductOutboundController.Archive(productOutbound, TempData);
                        //    }
                        //    #endregion
                        //}
                        //else
                        //{
                        //    #region   cập nhật phiếu xuất kho
                        //    //xóa chi tiết phiếu xuất, insert chi tiết mới
                        //    //cập nhật lại tổng tiền, trạng thái phiếu xuất
                        //    for (int i = 0; i < product_outbound.Count(); i++)
                        //    {
                        //        var outbound_detail = productOutboundRepository.GetAllProductOutboundDetailByOutboundId(product_outbound[i].Id).ToList();
                        //        //xóa
                        //        for (int ii = 0; ii < outbound_detail.Count(); ii++)
                        //        {
                        //            productOutboundRepository.DeleteProductOutboundDetail(outbound_detail[ii].Id);
                        //        }
                        //        var insert_ = invoice_detail_list.Where(x => x.ProductType == "product").ToList();
                        //        //insert mới
                        //        for (int iii = 0; iii < insert_.Count(); iii++)
                        //        {
                        //            ProductOutboundDetail productOutboundDetail = new ProductOutboundDetail();
                        //            productOutboundDetail.Price = insert_[iii].Price;
                        //            productOutboundDetail.ProductId = insert_[iii].ProductId;
                        //            productOutboundDetail.Quantity = insert_[iii].Quantity;
                        //            productOutboundDetail.Unit = insert_[iii].Unit;
                        //            productOutboundDetail.LoCode = insert_[iii].LoCode;
                        //            productOutboundDetail.ExpiryDate = insert_[iii].ExpiryDate;
                        //            productOutboundDetail.IsDeleted = false;
                        //            productOutboundDetail.CreatedUserId = WebSecurity.CurrentUserId;
                        //            productOutboundDetail.ModifiedUserId = WebSecurity.CurrentUserId;
                        //            productOutboundDetail.CreatedDate = DateTime.Now;
                        //            productOutboundDetail.ModifiedDate = DateTime.Now;
                        //            productOutboundDetail.ProductOutboundId = product_outbound[i].Id;
                        //            productOutboundRepository.InsertProductOutboundDetail(productOutboundDetail);

                        //        }
                        //        product_outbound[i].TotalAmount = invoice_detail_list.Where(x => x.ProductType == "product").Sum(x => (x.Price * x.Quantity));

                        //        //Ghi sổ chứng từ phiếu xuất
                        //        //ProductOutboundController.Archive(product_outbound[i], TempData);
                        //    }
                        //    #endregion
                        //}
                        //#endregion

                        #region  xóa hết lịch sử giao dịch
                        //xóa lịch sử giao dịch có liên quan đến đơn hàng, gồm: 1 dòng giao dịch bán hàng, 1 dòng thu tiền.
                        var transaction_Liablities = transactionLiabilitiesRepository.GetAllTransaction().Where(x => x.BranchId == Helpers.Common.CurrentUser.BranchId.Value && x.MaChungTuGoc == productInvoice.Code && x.LoaiChungTuGoc == "ProductInvoice").ToList();
                        if (transaction_Liablities.Count() > 0)
                        {
                            for (int i = 0; i < transaction_Liablities.Count(); i++)
                            {
                                transactionLiabilitiesRepository.DeleteTransaction(transaction_Liablities[i].Id);
                            }
                        }
                        #endregion

                        if (!productInvoice.IsArchive)
                        {
                            #region  Cập nhật thông tin thanh toán cho đơn hàng
                            //Cập nhật thông tin thanh toán cho đơn hàng
                            productInvoice.PaymentMethod = model.ReceiptViewModel.PaymentMethod;
                            productInvoice.PaidAmount += Convert.ToDecimal(model.TotalAmountMoneyMove);
                            productInvoice.DoanhThu = productInvoice.PaidAmount;
                            productInvoice.RemainingAmount = Math.Abs(productInvoice.TotalAmount - productInvoice.PaidAmount);
                            productInvoice.NextPaymentDate = model.NextPaymentDate_Temp;
                            productInvoice.ModifiedDate = DateTime.Now;
                            productInvoice.ModifiedUserId = WebSecurity.CurrentUserId;

                            productInvoiceRepository.UpdateProductInvoice(productInvoice);

                            //Lấy mã KH
                            var customer = customerRepository.GetCustomerById(productInvoice.CustomerId.Value);


                            #endregion

                            #region thêm lịch sử bán hàng
                            //Ghi Nợ TK 131 - Phải thu của khách hàng (tổng giá thanh toán)
                            Erp.BackOffice.Account.Controllers.TransactionLiabilitiesController.Create(
                                    intBrandID,
                                    productInvoice.Code,
                                    "ProductInvoice",
                                    "Phải thu của khách hàng cho đơn hàng " + productInvoice.Code,
                                    customer.Code,
                                    "Customer",
                                    productInvoice.TotalAmount,
                                    0,
                                    productInvoice.Code,
                                    "ProductInvoice",
                                    null,
                                    model.NextPaymentDate_Temp,
                                    null);
                            #endregion



                            #region cập nhật đơn bán hàng
                            //Cập nhật đơn hàng
                            productInvoice.ModifiedUserId = WebSecurity.CurrentUserId;
                            productInvoice.ModifiedDate = DateTime.Now;
                            productInvoice.IsArchive = false;
                            productInvoice.Status = App_GlobalResources.Wording.Status_deposit;
                            productInvoiceRepository.UpdateProductInvoice(productInvoice);

                            #endregion


                        }

                    }
                    //end su ly nhu nhan dat coc


                    //asas


                    // cập nhật tiền trong đơn bị chuyển
                    // chưa xử lý được thao tác update nhiều lần cùng 1 bảng trong 1 transaction nên phải dùng stored
                    productInvoiceOLD.DoanhThu -= Convert.ToDecimal(model.TotalAmountMoneyMove);
                    //đơn cọc và đơn hoàn thành chuyển hết đều có doanh thu bằng 0

                    bool IsDelete = productInvoiceOLD.DoanhThu == 0 ? true : false;

                    string Note;
                    if (productInvoiceOLD.Status.Contains("complete"))
                    {
                        Note = productInvoiceOLD.Note + "<br>" + "<b>Chuyển cho: </b>" + productInvoice.Code;
                    }
                    else
                    {
                        Note = "Chuyển cho " + productInvoice.Code;
                    }

                    // Cập nhật cột Status, DoanhThu
                    var value_test = SqlHelper.QuerySP<ProductInvoiceViewModel>("UpdateProductInvoiceAfterTransfer", new
                    {
                        TienDaTra = model.TotalAmountMoneyMove,//productInvoiceOLD.tiendathu,
                        OldCode = productInvoiceOLD.Code,
                        Note = Note,
                        IsDelete = IsDelete,
                        BranchId = productInvoiceOLD.BranchId
                    }).ToList();




                    var MoneyMove = new MoneyMove();
                    //AutoMapper.Mapper.Map(model, MoneyMove);
                    MoneyMove.IsDeleted = false;
                    MoneyMove.CreatedUserId = WebSecurity.CurrentUserId;
                    if (model.NextPaymentDate_Temphoapd != null)
                    {
                        MoneyMove.CreatedDate = model.NextPaymentDate_Temphoapd;
                        MoneyMove.ModifiedDate = model.NextPaymentDate_Temphoapd;
                    }
                    else
                    {
                        MoneyMove.CreatedDate = DateTime.Now;
                        MoneyMove.ModifiedDate = DateTime.Now;
                    }
                    MoneyMove.Note = model.NoteMoneyMove;
                    MoneyMove.FromProductInvoiceId = model.ProductInvoiceOldId;
                    MoneyMove.ToProductInvoiceId = model.Id;
                    MoneyMove.TotalAmount = model.TotalAmountMoneyMove;
                    MoneyMove.IsArchive = true;
                    MoneyMove.BranchId = intBrandID;
                    productInvoiceRepository.InsertMoneyMove(MoneyMove);
                    scope.Complete();

                }
                catch (DbUpdateException ex)
                {
                    return Content(ex.Message);
                }
            }
            //using (var scope2 = new TransactionScope(TransactionScopeOption.Required))
            //{
            //    try
            //    {
            //        //cập nhật tiền trong đơn bị chuyển
            //        vwOldProductInvoice.PaidAmount -= Convert.ToDecimal(model.ReceiptViewModel.Amount);
            //        vwOldProductInvoice.RemainingAmount = vwOldProductInvoice.TotalAmount - vwOldProductInvoice.PaidAmount;
            //        ProductInvoice OldProductInvoice = new ProductInvoice();
            //        AutoMapper.Mapper.Map(vwOldProductInvoice, OldProductInvoice);
            //        productInvoiceRepository.UpdateProductInvoice(OldProductInvoice);
            //    }
            //    catch (DbUpdateException ex)
            //    {
            //        return Content(ex.Message);
            //    }
            //}

            TempData[Globals.SuccessMessageKey] += "Nhận tiền chuyển thành công";
            return RedirectToAction("Detail", new { Id = model.Id });
        }
        #endregion

        public ActionResult SettingPromotion(string Type, int ProductInvoiceId)
        {
            var model = new LogPromotionViewModel();
            model.Type = Type;
            model.Category = "handwork";
            model.TargetID = ProductInvoiceId;
            return View(model);
        }

        public PartialViewResult PartialItemPromotion(LogPromotionViewModel data)
        {

            return PartialView(data);
        }



        [HttpPost]
        [ValidateInput(false)]
        public ActionResult CreatePOSInDathang(List<ProductInvoiceDetailViewModel> listsp, string customerId, string DiscountTab, string Hinhthuctt, decimal? MoneyPayTab, decimal ATMPayTab, decimal TransferPayTab, string IsDisAll, string DiscountAll, string NoteHDtab, bool NotPrint, string tyLeVIP, int? MoneyShip, string CountForBrand)
        {
            // string pMA_DVIQLY = Helpers.Common.CurrentUser.MA_DVIQLY;

            int idHD = 0;
            try
            {

                if (listsp != null && listsp.Count > 0)
                {




                    ProductInvoiceViewModel model = new ProductInvoiceViewModel();
                    model.CountForBrand = CountForBrand;
                    model.InvoiceList = new List<ProductInvoiceDetailViewModel>();

                    AutoMapper.Mapper.Map(listsp, model.InvoiceList);
                    model.ReceiptViewModel = new ReceiptViewModel();
                    model.NextPaymentDate_Temp = DateTime.Now.AddDays(30);
                    // model.MA_DVIQLY = pMA_DVIQLY;
                    model.ReceiptViewModel.Name = "Bán hàng - Thu tiền mặt";
                    model.ReceiptViewModel.Amount = 0;
                    if (customerId == "")
                    {
                        customerId = "0";
                    }
                    model.CustomerId = int.Parse(customerId);

                    //begin hoapd copy code insert
                    //get cookie brachID 
                    HttpRequestBase request = this.HttpContext.Request;
                    string strBrandID = "0";
                    if (request.Cookies["BRANCH_ID_CookieName"] != null)
                    {
                        strBrandID = request.Cookies["BRANCH_ID_CookieName"].Value;
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


                    /// var json = new JavaScriptSerializer().Serialize(model);
                    if (model.InvoiceList.Count != 0)
                    {
                        var customerthis = customerRepository.GetCustomerById(int.Parse(customerId));

                        ProductInvoice productInvoice = null;

                        //    string output = JsonConvert.SerializeObject(model);

                        if (model.Id > 0)
                        {
                            productInvoice = productInvoiceRepository.GetProductInvoiceById(model.Id);
                            //Kiểm tra phân quyền Chi nhánh
                            //if ((Filters.SecurityFilter.IsTrinhDuocVien() || Filters.SecurityFilter.IsAdminSystemManager() || Filters.SecurityFilter.IsStaffDrugStore() || Filters.SecurityFilter.IsAdminDrugStore()) && ("," + Helpers.Common.CurrentUser.DrugStore + ",").Contains("," + productInvoice.BranchId + ",") == false)
                            //{
                            //    return Content("Mẫu tin không tồn tại! Không có quyền truy cập!");
                            //}
                        }
                        using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.Snapshot }))
                        {
                            try
                            {
                                if (productInvoice != null)
                                {
                                    //Nếu đã ghi sổ rồi thì không được sửa
                                    if (productInvoice.IsArchive)
                                    {
                                        return Json(0);
                                    }

                                    AutoMapper.Mapper.Map(model, productInvoice);
                                }
                                else
                                {
                                    productInvoice = new ProductInvoice();

                                    AutoMapper.Mapper.Map(model, productInvoice);
                                    if (customerId == null || customerId == "" || customerId == "0")
                                    {
                                        //var customernull = customerRepository.GetCustomerById(0);
                                        //productInvoice.Cu = customernull.FirstName;
                                        //productInvoice.CustomerId = customernull.Id;
                                        //productInvoice.ShipPhone = customernull.Phone;
                                        //productInvoice.TaxFee = customernull.TaxCode;
                                        //productInvoice.ShipAddress = customernull.Address;
                                        // productInvoice.BankName = customernull.BankName;
                                        // productInvoice.MA_DVIQLY = pMA_DVIQLY;
                                    }
                                    else
                                    {
                                        productInvoice.CustomerId = int.Parse(customerId);
                                    }

                                    if ((productInvoice.CustomerId == null) || (productInvoice.CustomerId == 0))
                                    {
                                        //var customernull = customerRepository.GetCustomerByCode(pMA_DVIQLY, "KHACHVANGLAI");
                                        //productInvoice.CustomerName = customernull.FirstName;
                                        //productInvoice.CustomerId = customernull.Id;
                                        //productInvoice.CustomerPhone = customernull.Phone;
                                        //productInvoice.TaxCode = customernull.TaxCode;
                                        //productInvoice.Address = customernull.Address;
                                        //productInvoice.BankName = customernull.BankName;
                                        //productInvoice.MA_DVIQLY = pMA_DVIQLY;

                                    }
                                    // productInvoice.TotalAmount = 0;

                                    foreach (var k in model.InvoiceList)
                                    {
                                        productInvoice.TotalAmount += k.Quantity.Value * k.Price.Value;
                                    }
                                    //3408 khách vãng lai

                                    //if (productInvoice.CustomerName != "KHACHVANGLAI")
                                    //{
                                    //    productInvoice.CustomerId = customerthis.Id;
                                    //    productInvoice.CustomerName = customerthis.LastName + " " + customerthis.FirstName;
                                    //    productInvoice.CustomerPhone = customerthis.Phone;
                                    //    productInvoice.TaxCode = customerthis.TaxCode;
                                    //    productInvoice.Address = customerthis.Address;
                                    //    productInvoice.BankName = customerthis.BankName;
                                    //    productInvoice.MA_DVIQLY = pMA_DVIQLY;
                                    //}

                                    productInvoice.IsDeleted = false;
                                    productInvoice.CreatedUserId = WebSecurity.CurrentUserId;
                                    productInvoice.CreatedDate = DateTime.Now;
                                    productInvoice.Status = Wording.OrderStatus_pending;
                                    productInvoice.BranchId = model.BranchId.HasValue ? model.BranchId.Value : 1;
                                    productInvoice.IsArchive = false;
                                    productInvoice.IsReturn = false;
                                    productInvoice.SaleChanel = 4;
                                    //productInvoice.MA_DVIQLY = pMA_DVIQLY;
                                    if (MoneyPayTab != null && MoneyPayTab != 0)
                                    {
                                        //productInvoice.MoneyPay = MoneyPayTab;
                                    }
                                    else
                                    {
                                        // productInvoice.MoneyPay = 0;
                                    }
                                    if (NoteHDtab.Length > 0 && NoteHDtab != null)
                                    {
                                        productInvoice.Note = NoteHDtab;
                                    }
                                    else
                                    {
                                        productInvoice.Note = "";
                                    }
                                    if (ATMPayTab != null && ATMPayTab != 0)
                                    {
                                        // productInvoice.ATMPay = ATMPayTab;
                                    }
                                    else
                                    {
                                        // productInvoice.ATMPay = 0;
                                    }
                                    if (TransferPayTab != null && TransferPayTab != 0)
                                    {
                                        //productInvoice.TransferPay = TransferPayTab;
                                    }
                                    else
                                    {
                                        // productInvoice.TransferPay = 0;
                                    }
                                    productInvoice.RemainingAmount = model.TotalAmount;
                                    productInvoice.PaidAmount = 0;
                                    productInvoice.BranchId = intBrandID.Value;
                                    //  productInvoice.MA_DVIQLY = pMA_DVIQLY;
                                    productInvoice.PaymentMethod = SelectListHelper.GetSelectList_Category("FormPayment", null, "Name", null).FirstOrDefault().Value;
                                }
                                //Duyệt qua danh sách sản phẩm mới xử lý tình huống user chọn 2 sản phầm cùng id
                                List<ProductInvoiceDetail> listNewCheckSameId = new List<ProductInvoiceDetail>();
                                model.InvoiceList.RemoveAll(x => x.Quantity <= 0);
                                decimal pTongtienhoadon = 0;
                                decimal pTongtiengiamgia = 0;
                                foreach (var group in model.InvoiceList)
                                {
                                    var product = ProductRepository.GetProductById(group.ProductId.Value);
                                    ProductInvoiceDetail tmp = new ProductInvoiceDetail();
                                    tmp.ProductId = product.Id;
                                    //tmp.ProductType = product.Type;
                                    tmp.Quantity = group.Quantity;
                                    tmp.Unit = product.Unit;
                                    tmp.Price = group.Price;
                                    tmp.Discount = 0;
                                    tmp.DiscountAmount = 0;

                                    tmp.IsDeleted = false;
                                    tmp.CreatedUserId = WebSecurity.CurrentUserId;
                                    tmp.ModifiedUserId = WebSecurity.CurrentUserId;
                                    tmp.CreatedDate = DateTime.Now;
                                    tmp.ModifiedDate = DateTime.Now;

                                    tmp.LoCode = group.LoCode;
                                    tmp.ExpiryDate = group.ExpiryDate;

                                    listNewCheckSameId.Add(tmp);
                                }


                                //productInvoice.TotalAmount = pTongtienhoadon;



                                // productInvoice.IrregularDiscount = pTongtiengiamgia;

                                if (DiscountTab != null && DiscountTab != "" && DiscountTab.Length > 0)
                                {
                                    char[] whitespace = new char[] { '-', '\t' };
                                    string[] arrdis = DiscountTab.Split(whitespace);
                                    if (arrdis[1] == "%")
                                    {
                                        //productInvoice.DiscountTabBill = int.Parse(arrdis[0]);
                                        //productInvoice.DiscountTabBillAmount = Convert.ToInt32(productInvoice.TotalAmount * (int.Parse(arrdis[0])) / 100);
                                        //productInvoice.IrregularDiscount = Helpers.Common.NVL_NUM_LONG_NEW(productInvoice.IrregularDiscount) + productInvoice.DiscountTabBillAmount.Value;
                                        //productInvoice.PromotionId = Convert.ToInt32(arrdis[3]);
                                        //productInvoice.PromotionDetailId = Convert.ToInt32(arrdis[2]);
                                        //productInvoice.PromotionValue = Convert.ToInt32(arrdis[0]);
                                        //productInvoice.IsMoney = false;
                                        //productInvoice.TotalAmount = productInvoice.TotalAmount - productInvoice.DiscountTabBillAmount.Value;
                                        //productInvoice.MA_DVIQLY = pMA_DVIQLY;

                                    }
                                    else
                                    {
                                        //productInvoice.DiscountTabBillAmount = decimal.Parse(arrdis[0]);
                                        //productInvoice.IrregularDiscount = Helpers.Common.NVL_NUM_LONG_NEW(productInvoice.IrregularDiscount) + productInvoice.DiscountTabBillAmount.Value;
                                        //productInvoice.PromotionId = Convert.ToInt32(arrdis[3]);
                                        //productInvoice.PromotionDetailId = Convert.ToInt32(arrdis[2]);
                                        //productInvoice.PromotionValue = Convert.ToInt32(arrdis[0]);
                                        //productInvoice.IsMoney = true;
                                        //productInvoice.TotalAmount = productInvoice.TotalAmount - productInvoice.DiscountTabBillAmount.Value;
                                        //productInvoice.MA_DVIQLY = pMA_DVIQLY;
                                    }

                                }
                                if (DiscountAll != null && DiscountAll != "" && DiscountAll.Length > 0)
                                {
                                    DiscountAll = DiscountAll.Replace(".", "");
                                    //productInvoice.DisCountAllTab = decimal.Parse(DiscountAll);
                                    if (IsDisAll == "1")
                                    {
                                        decimal tiengiam3 = ((productInvoice.TotalAmount * int.Parse(DiscountAll)) / 100);
                                        productInvoice.TotalAmount = productInvoice.TotalAmount - tiengiam3;
                                        //productInvoice.isDisCountAllTab = true;
                                        //productInvoice.IrregularDiscount = Helpers.Common.NVL_NUM_LONG_NEW(productInvoice.IrregularDiscount) + tiengiam3;
                                    }
                                    else
                                    {
                                        productInvoice.TotalAmount = productInvoice.TotalAmount - decimal.Parse(DiscountAll);
                                        //productInvoice.IrregularDiscount = Helpers.Common.NVL_NUM_LONG_NEW(productInvoice.IrregularDiscount) + decimal.Parse(DiscountAll);
                                        //productInvoice.isDisCountAllTab = false;
                                    }
                                }
                                productInvoice.IsReturn = false;
                                productInvoice.ModifiedUserId = WebSecurity.CurrentUserId;
                                productInvoice.ModifiedDate = DateTime.Now;
                                productInvoice.PaidAmount = 0;
                                productInvoice.TotalAmount = Math.Round(productInvoice.TotalAmount, 0, MidpointRounding.ToEven);
                                productInvoice.RemainingAmount = productInvoice.TotalAmount;




                                //kiểm tra xem khách hàng có phải là khách hàng VIP không, nếu Vip thì giảm giá
                                //var logvip = logVipRepository.GetlistvwAllLogVip(pMA_DVIQLY).Where(x => x.is_approved == true && x.Status == "Đang sử dụng" && x.CustomerId == productInvoice.CustomerId).FirstOrDefault();
                                //if (logvip != null)
                                //{
                                //    if (logvip.PlusPoint != null && logvip.PlusPoint > 0)
                                //    {
                                //        decimal tiengiam3 = Math.Round(((productInvoice.TotalAmount * logvip.PlusPoint.Value) / 100), 0, MidpointRounding.ToEven);
                                //        productInvoice.TotalAmount = productInvoice.TotalAmount - tiengiam3;
                                //        productInvoice.isDisCountAllTab = true;
                                //        productInvoice.VIP = logvip.PlusPoint.Value;
                                //        productInvoice.IrregularDiscount = Helpers.Common.NVL_NUM_LONG_NEW(productInvoice.IrregularDiscount) + tiengiam3;
                                //    }
                                //}

                                //End tra xem khách hàng có phải là khách hàng VIP không, nếu Vip thì giảm giá
                                //if (Helpers.Common.NVL_NUM_LONG_NEW(productInvoice.TransferPay) == 0 && Helpers.Common.NVL_NUM_LONG_NEW(productInvoice.ATMPay) == 0)
                                //{
                                //    //productInvoice.MoneyPay = productInvoice.TotalAmount;
                                //}
                                //tính lại tổng tiền
                                //hàm edit 
                                if (model.Id > 0)
                                {
                                    //productInvoice.MA_DVIQLY = pMA_DVIQLY;
                                    productInvoiceRepository.UpdateProductInvoice(productInvoice);
                                    var listDetail = productInvoiceRepository.GetAllInvoiceDetailsByInvoiceId(productInvoice.Id).ToList();

                                    //xóa danh sách dữ liệu cũ dưới database gồm product invoice, productInvoiceDetail, UsingServiceLog,UsingServiceLogDetail,ServiceReminder
                                    productInvoiceRepository.DeleteProductInvoiceDetail(listDetail);

                                    //thêm mới toàn bộ database
                                    foreach (var item in listNewCheckSameId)
                                    {
                                        item.ProductInvoiceId = productInvoice.Id;
                                        //item.MA_DVIQLY = pMA_DVIQLY;
                                        productInvoiceRepository.InsertProductInvoiceDetail(item);
                                    }

                                    //Thêm vào quản lý chứng từ
                                    TransactionController.Create(new TransactionViewModel
                                    {
                                        TransactionModule = "ProductInvoice",
                                        TransactionCode = productInvoice.Code,
                                        TransactionName = "Bán hàng",
                                        // MA_DVIQLY = pMA_DVIQLY
                                    });
                                }
                                else
                                {
                                    //hàm thêm mới
                                    //productInvoice.MA_DVIQLY = pMA_DVIQLY;
                                    //productInvoice.PHISHIP = MoneyShip;
                                    productInvoiceRepository.InsertProductInvoice(productInvoice, listNewCheckSameId);

                                    //cập nhật lại mã hóa đơn
                                    //string prefix = Erp.BackOffice.Helpers.Common.GetSetting(Erp.BackOffice.Helpers.Common.CurrentUser.MA_DVIQLY,"prefixOrderNo_Invoice");
                                    //productInvoice.Code = Erp.BackOffice.Helpers.Common.GetCode(prefix, productInvoice.Id);
                                    //productInvoiceRepository.UpdateProductInvoice(productInvoice);

                                    //hoa sua lai 
                                    productInvoice.Code = Erp.BackOffice.Helpers.Common.GetOrderNo("ProductInvoice", model.Code);

                                    //begin kiem tra trung ma don hang

                                    var donhangtrunga = productInvoiceRepository.GetvwProductInvoiceByCode(intBrandID.Value, productInvoice.Code);
                                    if (donhangtrunga != null)
                                    {
                                        TempData[Globals.FailedMessageKey] = App_GlobalResources.Wording.Error;
                                        return Json(0);
                                    }

                                    //end kiem tra trung ma don hang



                                    productInvoiceRepository.UpdateProductInvoice(productInvoice);
                                    Erp.BackOffice.Helpers.Common.SetOrderNo("ProductInvoice");

                                    //ghi log 
                                    Erp.BackOffice.Controllers.HomeController.WriteLog(productInvoice.Id, productInvoice.Code, "đã tạo đơn hàng", "ProductInvoice/Detail/" + productInvoice.Id, Helpers.Common.CurrentUser.BranchId.Value);

                                    //thêm lịch sử ghi chú - cong
                                    //var Note = new NoteProductInvoice();
                                    //Note.ProductInvoiceId = productInvoice.Id;
                                    //Note.Note = productInvoice.Note;
                                    //Note.CreatedDate = productInvoice.CreatedDate;
                                    //Note.CreatedUserId = productInvoice.CreatedUserId;
                                    //Note.MA_DVIQLY = pMA_DVIQLY;
                                    //NoteProductInvoiceRepository.InsertNoteProductInvoice(Note);


                                    //end thêm lịch sử ghi chú

                                    //Thêm vào quản lý chứng từ
                                    TransactionController.Create(new TransactionViewModel
                                    {
                                        TransactionModule = "ProductInvoice",
                                        TransactionCode = productInvoice.Code,
                                        TransactionName = "Bán hàng",
                                        // MA_DVIQLY = pMA_DVIQLY
                                    });
                                    idHD = productInvoice.Id;
                                }

                                //if (ghiso(productInvoice.Id, intBrandID, Hinhthuctt) == false)
                                //{
                                //    TempData[Globals.FailedMessageKey] = App_GlobalResources.Wording.Error;
                                //    return Json(0);
                                //}

                                ViewBag.isnull = "";
                                if (MoneyShip > 0)
                                {
                                    productInvoice.TotalAmount = productInvoice.TotalAmount + MoneyShip.Value;
                                }
                                //if ((Helpers.Common.NVL_NUM_LONG_NEW(productInvoice.TotalAmount) < 0)
                                //    || (Helpers.Common.NVL_NUM_LONG_NEW(productInvoice.TotalAmount) != (Helpers.Common.NVL_NUM_LONG_NEW(productInvoice.MoneyPay) + Helpers.Common.NVL_NUM_LONG_NEW(productInvoice.TransferPay) + Helpers.Common.NVL_NUM_LONG_NEW(productInvoice.ATMPay)))
                                //    )
                                //{
                                //    TempData[Globals.FailedMessageKey] = App_GlobalResources.Wording.Error;
                                //    return Json(0);
                                //}


                                scope.Complete();

                            }
                            catch (DbUpdateException)
                            {
                                TempData[Globals.FailedMessageKey] = App_GlobalResources.Wording.Error;
                                return Json(0);
                            }
                        }
                    }
                    //end hoapd copy code insert

                }
                if (NotPrint == true)
                {
                    return Json(1);
                }
                else
                {
                    return Json(idHD);
                }

            }
            catch (Exception ex)
            {

                return Json(0);
            }



        }


    }
}
