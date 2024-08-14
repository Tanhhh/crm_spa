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
using System.Web;
using Erp.Domain.Sale.Interfaces;

namespace Erp.BackOffice.Account.Controllers
{
    [Authorize]
    [InitializeSimpleMembership]
    [Erp.BackOffice.Helpers.NoCacheHelper]
    public class AccountReportController : Controller
    {
        private readonly ITransactionLiabilitiesRepository transactionRepository;
        private readonly IUserRepository userRepository;
        private readonly IProcessPaymentRepository processPaymentRepository;
        private readonly IProductInvoiceRepository productInvoiceRepository;

        public AccountReportController(
            ITransactionLiabilitiesRepository _Transaction
            , IUserRepository _user
            , IProcessPaymentRepository _ProcessPayment
             , IProductInvoiceRepository _ProductInvoice
            )
        {
            transactionRepository = _Transaction;
            userRepository = _user;
            processPaymentRepository = _ProcessPayment;
            productInvoiceRepository = _ProductInvoice;
        }

        #region Summary
        public ViewResult Summary(int? year = null, int? month = null, int? day = null)
        {
            if (year == null)
            {
                year = DateTime.Now.Year;
                month = DateTime.Now.Month;
                day = DateTime.Now.Day;
            }

            var q = transactionRepository.GetAllTransaction()
                .Where(item => item.CreatedDate.Value.Year == year
                && item.CreatedDate.Value.Month == month
                && item.CreatedDate.Value.Day == day);

            var model = new AccountReportSumaryViewModel();

            //string TransactionType_ContractLease = TransactionController.TransactionType.ContractLease.ToString();
            //string TransactionType_ContractSell = TransactionController.TransactionType.ContractSell.ToString();
            //string TransactionType_ProductInvoice = TransactionController.TransactionType.ProductInvoice.ToString();
            //string TransactionType_ReceiptCustomer = TransactionController.TransactionType.ReceiptCustomer.ToString();
            //string TransactionType_ReceiptOhter = TransactionController.TransactionType.ReceiptOhter.ToString();
            //string TransactionType_PaymentCustomer = TransactionController.TransactionType.PaymentCustomer.ToString();
            //string TransactionType_PaymentOhter = TransactionController.TransactionType.PaymentOhter.ToString();

            //var DoanhSoTrongNgay = q.Where(item => item.ParentId == null
            //    && (item.TransactionType == TransactionType_ContractLease
            //    || item.TransactionType == TransactionType_ContractSell
            //    || item.TransactionType == TransactionType_ProductInvoice))
            //    .Sum(item => item.Total);

            //var ThuTrongNgay = q.Where(item => item.TransactionType == TransactionType_ReceiptCustomer
            //    || item.TransactionType == TransactionType_ReceiptOhter)
            //    .Sum(item => item.Payment);

            //var ChiTrongNgay = q.Where(item => item.TransactionType == TransactionType_PaymentOhter
            //    || item.TransactionType == TransactionType_PaymentCustomer)
            //    .Sum(item => item.Payment);

            //var q_thu_tien_mat = q.Where(item => item.PaymentMethod == App_GlobalResources.Wording.PaymentMethod_Cash
            //    && (item.TransactionType == TransactionType_ReceiptCustomer
            //    || item.TransactionType == TransactionType_ReceiptOhter))
            //    .Sum(item => item.Payment);

            //double thu_tien_mat = q_thu_tien_mat != null ? q_thu_tien_mat.Value : 0;

            //var q_chi_tien_mat = q.Where(item => item.PaymentMethod == App_GlobalResources.Wording.PaymentMethod_Cash
            //    && (item.TransactionType == TransactionType_PaymentOhter || item.TransactionType == TransactionType_PaymentCustomer))
            //    .Sum(item => item.Payment);

            //double chi_tien_mat = q_chi_tien_mat != null ? q_chi_tien_mat.Value : 0;

            //model.DoanhSo = DoanhSoTrongNgay != null ? DoanhSoTrongNgay.Value : 0;
            //model.Thu = ThuTrongNgay != null ? ThuTrongNgay.Value : 0;
            //model.Chi = ChiTrongNgay != null ? ChiTrongNgay.Value : 0;
            //model.TienMat = thu_tien_mat - chi_tien_mat;

            return View(model);
        }
        #endregion

        #region TrackLiabilities
        public ViewResult TrackLiabilities(string type)
        {
            AccountReportTrackLiabilitiesViewModel model = new AccountReportTrackLiabilitiesViewModel();
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

            var listProductInvoice = productInvoiceRepository.GetAllvwProductInvoiceFull_NVKD()
                    .Where(item =>item.BranchId == intBrandID.Value
                            && item.tienconno != null
                            && item.tienconno > 0
                            );

            if (listProductInvoice.Count() > 0)
            {
                model.CongNoPhaiThuKhachHang = listProductInvoice.Sum(item => item.tienconno.Value);
            }

            var cong_no_ncc = transactionRepository.GetAllTransaction()
                .Where(item => item.BranchId == intBrandID.Value && item.TargetModule == "Supplier");

            if (cong_no_ncc.Count() > 0)
            {
                model.CongNoPhaiTraNhaCungCap = cong_no_ncc.Sum(item => item.Debit - item.Credit);
            }
            ViewBag.Name = type;
            return View(model);
        }
        #endregion
    }
}
