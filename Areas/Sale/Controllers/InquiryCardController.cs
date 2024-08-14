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
using Newtonsoft.Json;
using Erp.Domain.Account.Interfaces;
using System.Transactions;
using Erp.BackOffice.Account.Controllers;
using Erp.BackOffice.Account.Models;
using System.Web;
using Erp.Domain.Account.Helper;
using System.IO;
using System.Web.UI;
//sdsds
namespace Erp.BackOffice.Sale.Controllers
{
    [Authorize]
    [InitializeSimpleMembership]
    [Erp.BackOffice.Helpers.NoCacheHelper]
    public class InquiryCardController : Controller
    {
        private readonly IInquiryCardRepository InquiryCardRepository;
        private readonly IUserRepository userRepository;
        private readonly IProductOrServiceRepository productRepository;
        private readonly IServiceStepsRepository serviceStepsRepository;
        private readonly IInquiryCardDetailRepository inquiryCardDetailRepository;
        private readonly ICustomerRepository customerRepository;
        private readonly IAdviseCardRepository adviseCardRepository;
        private readonly IMembershipRepository membershipRepository;
        private readonly IServiceDetailRepository serviceDetailRepository;
        private readonly IProductDetailRepository productDetailRepository;
        private readonly IWarehouseRepository warehouseRepository;
        private readonly IProductOutboundRepository productOutboundRepository;
        private readonly IMembershipRepository MembershipRepository;
        private readonly IMaterialOutboundRepository materialOutboundRepository;
        private readonly ITemplatePrintRepository templatePrintRepository;

        public InquiryCardController(
            IInquiryCardRepository _InquiryCard
            , IUserRepository _user
            , IProductOrServiceRepository product
            , IServiceStepsRepository serviceSteps
            , IInquiryCardDetailRepository inquiryCard
            , ICustomerRepository customer
            , IAdviseCardRepository adviseCard
            , IMembershipRepository membership
            , IServiceDetailRepository serviceDetail
            , IProductDetailRepository productDetail
            , IWarehouseRepository warehouse
            , IProductOutboundRepository productOutbound
            , IMembershipRepository _Membership
            , ITemplatePrintRepository templatePrint
                        , IMaterialOutboundRepository _MaterialOutbound
            )
        {
            InquiryCardRepository = _InquiryCard;
            userRepository = _user;
            productRepository = product;
            serviceStepsRepository = serviceSteps;
            inquiryCardDetailRepository = inquiryCard;
            customerRepository = customer;
            adviseCardRepository = adviseCard;
            membershipRepository = membership;
            serviceDetailRepository = serviceDetail;
            productDetailRepository = productDetail;
            warehouseRepository = warehouse;
            productOutboundRepository = productOutbound;
            MembershipRepository = _Membership;
            materialOutboundRepository = _MaterialOutbound;
            templatePrintRepository = templatePrint;
        }

        #region Index

        public ViewResult Index(string startDate, string endDate, string txtCode, string txtCusCode, string txtCusInfo,
            string txtCusName, int? Status, int? BranchId, int? ManagerId, int? CreateUserId, string type, string productCode, string TargetCode)
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
            BranchId = intBrandID;

            if (startDate == null && endDate == null)
            {
                DateTime aDateTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                // Cộng thêm 1 tháng và trừ đi một ngày.
                DateTime retDateTime = aDateTime.AddMonths(1).AddDays(-1);
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
            var q = SqlHelper.QuerySP<InquiryCardViewModel>("Get_ListInquiryCard", new
            {
                StartDate = d_startDate,
                EndDate = d_endDate,
                branchId = BranchId
            }).OrderByDescending(m => m.CreatedDate).ToList();

            //IEnumerable<InquiryCardViewModel> q = InquiryCardRepository.GetvwAllInquiryCard().Where(x => x.CreatedDate >= d_startDate && x.CreatedDate <= d_endDate)
            //    .Select(item => new InquiryCardViewModel
            //    {
            //        Id = item.Id,
            //        CreatedUserId = item.CreatedUserId,
            //        //CreatedUserName = item.CreatedUserName,
            //        CreatedDate = item.CreatedDate,
            //        ModifiedUserId = item.ModifiedUserId,
            //        //ModifiedUserName = item.ModifiedUserName,
            //        ModifiedDate = item.ModifiedDate,
            //        Code = item.Code,
            //        TargetCode = item.TargetCode,
            //        BranchId = item.BranchId,
            //        CreateUserName = item.CreateUserName,
            //        BranchName = item.BranchName,
            //        TargetModule = item.TargetModule,
            //        CreateUserCode = item.CreateUserCode,
            //        CustomerCode = item.CustomerCode,
            //        CustomerName = item.CustomerName,
            //        CustomerId = item.CustomerId,
            //        ManagerCode = item.ManagerCode,
            //        ManagerName = item.ManagerName,
            //        ProductId = item.ProductId,
            //        ProductName = item.ProductName,
            //        TargetId = item.TargetId,
            //        BranchCode = item.BranchCode,
            //        Note = item.Note,
            //        TotalMinute = item.TotalMinute,
            //        ManagerUserId = item.ManagerUserId,
            //        ProductCode = item.ProductCode,
            //        WorkDay = item.WorkDay,
            //        Type = item.Type,
            //        SkinscanUserId = item.SkinscanUserId,
            //        SkinscanUserName = item.SkinscanUserName
            //    }).OrderByDescending(m => m.CreatedDate).ToList();

            foreach (var item in q)
            {
                if (item.TargetModule == "AdviseCard")
                {
                    var info = adviseCardRepository.GetvwAdviseCardById(item.TargetId.Value);
                    if (info != null)
                    {
                        item.CounselorName = info.CounselorName;
                    } 
                }
            }

            if (!string.IsNullOrEmpty(txtCode))
            {
                txtCode = Helpers.Common.ChuyenThanhKhongDau(txtCode);
                q = q.Where(x => Helpers.Common.ChuyenThanhKhongDau(x.Code).Contains(txtCode)).ToList();
            }
            if (!string.IsNullOrEmpty(TargetCode))
            {
                TargetCode = Helpers.Common.ChuyenThanhKhongDau(TargetCode);
                q = q.Where(x => Helpers.Common.ChuyenThanhKhongDau(x.TargetCode).Contains(TargetCode)).ToList();
            }
            if (!string.IsNullOrEmpty(productCode))
            {
                productCode = Helpers.Common.ChuyenThanhKhongDau(productCode);
                q = q.Where(x => Helpers.Common.ChuyenThanhKhongDau(x.ProductCode).Contains(productCode)).ToList();
            }
            //if (!string.IsNullOrEmpty(txtCusCode))
            //{
            //    txtCusCode = Helpers.Common.ChuyenThanhKhongDau(txtCusCode);
            //    q = q.Where(x => Helpers.Common.ChuyenThanhKhongDau(x.CustomerCode).Contains(txtCusCode)).ToList();
            //}
            if (!string.IsNullOrEmpty(type))
            {
                q = q.Where(x => x.Type == type).ToList();
            }
            //if (!string.IsNullOrEmpty(txtCusName))
            //{
            //    txtCusName = Helpers.Common.ChuyenThanhKhongDau(txtCusName);
            //    q = q.Where(x => Helpers.Common.ChuyenThanhKhongDau(x.CustomerName).Contains(txtCusName)).ToList();
            //}

            // Gộp textbox
            if (!string.IsNullOrEmpty(txtCusInfo))
            {
                q = q.Where(x => Helpers.Common.ChuyenThanhKhongDau(x.CustomerName).Contains(Helpers.Common.ChuyenThanhKhongDau(txtCusInfo)) || x.CustomerCode.Contains(txtCusInfo)).ToList();
            }

            //if (BranchId != null && BranchId.Value > 0)
            //{
            //    q = q.Where(x => x.BranchId == BranchId).ToList();
            //}


            //if (Helpers.Common.CurrentUser.BranchId != null)
            //{
            //    q = q.Where(x => x.BranchId == Helpers.Common.CurrentUser.BranchId.Value).ToList();
            //}


            if (ManagerId != null && ManagerId.Value > 0)
            {
                q = q.Where(x => x.ManagerStaffId == ManagerId).ToList();
            }
            if (CreateUserId != null && CreateUserId.Value > 0)
            {
                q = q.Where(x => x.CreatedUserId == CreateUserId).ToList();
            }
            ViewBag.SuccessMessage = TempData["SuccessMessage"];
            ViewBag.FailedMessage = TempData["FailedMessage"];
            ViewBag.AlertMessage = TempData["AlertMessage"];
            return View(q);
        }
        #endregion
        #region export
        public List<InquiryCardViewModel> IndexExport(string startDate, string endDate, string txtCode, string txtCusCode, string txtCusInfo,
            string txtCusName, int? Status, int? BranchId, int? ManagerId, int? CreateUserId, string type, string productCode, string TargetCode)
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
                d_startDate = d_startDate.AddHours(23).AddMinutes(59);
            }
            if (DateTime.TryParseExact(endDate, "dd/MM/yyyy", new CultureInfo("vi-VN"), DateTimeStyles.None, out d_endDate))
            {
                d_endDate = d_endDate.AddHours(23).AddMinutes(59);
            }

            var q = InquiryCardRepository.GetvwAllInquiryCard().Where(x => x.CreatedDate >= d_startDate && x.CreatedDate <= d_endDate)
                .Select(item => new InquiryCardViewModel
                {
                    Id = item.Id,
                    CreatedUserId = item.CreatedUserId,
                    //CreatedUserName = item.CreatedUserName,
                    CreatedDate = item.CreatedDate,
                    ModifiedUserId = item.ModifiedUserId,
                    //ModifiedUserName = item.ModifiedUserName,
                    ModifiedDate = item.ModifiedDate,
                    Code = item.Code,
                    TargetCode = item.TargetCode,
                    BranchId = item.BranchId,
                    CreateUserName = item.CreateUserName,
                    BranchName = item.BranchName,
                    TargetModule = item.TargetModule,
                    CreateUserCode = item.CreateUserCode,
                    CustomerCode = item.CustomerCode,
                    CustomerName = item.CustomerName,
                    CustomerId = item.CustomerId,
                    ManagerCode = item.ManagerCode,
                    ManagerName = item.ManagerName,
                    ProductId = item.ProductId,
                    ProductName = item.ProductName,
                    TargetId = item.TargetId,
                    BranchCode = item.BranchCode,
                    Note = item.Note,
                    TotalMinute = item.TotalMinute,
                    ManagerUserId = item.ManagerUserId,
                    ProductCode = item.ProductCode,
                    WorkDay = item.WorkDay,
                    Type = item.Type,
                    SkinscanUserId = item.SkinscanUserId,
                    SkinscanUserName = item.SkinscanUserName
                }).OrderByDescending(m => m.CreatedDate).ToList();

            foreach (var item in q)
            {
                if (item.TargetModule == "AdviseCard")
                {
                    var info = adviseCardRepository.GetvwAdviseCardById(item.TargetId.Value);
                    item.CounselorName = info.CounselorName;
                }
            }

            if (!string.IsNullOrEmpty(txtCode))
            {
                txtCode = Helpers.Common.ChuyenThanhKhongDau(txtCode);
                q = q.Where(x => Helpers.Common.ChuyenThanhKhongDau(x.Code).Contains(txtCode)).ToList();
            }
            if (!string.IsNullOrEmpty(TargetCode))
            {
                TargetCode = Helpers.Common.ChuyenThanhKhongDau(TargetCode);
                q = q.Where(x => Helpers.Common.ChuyenThanhKhongDau(x.TargetCode).Contains(TargetCode)).ToList();
            }
            if (!string.IsNullOrEmpty(productCode))
            {
                productCode = Helpers.Common.ChuyenThanhKhongDau(productCode);
                q = q.Where(x => Helpers.Common.ChuyenThanhKhongDau(x.ProductCode).Contains(productCode)).ToList();
            }
            //if (!string.IsNullOrEmpty(txtCusCode))
            //{
            //    txtCusCode = Helpers.Common.ChuyenThanhKhongDau(txtCusCode);
            //    q = q.Where(x => Helpers.Common.ChuyenThanhKhongDau(x.CustomerCode).Contains(txtCusCode)).ToList();
            //}
            if (!string.IsNullOrEmpty(type))
            {
                q = q.Where(x => x.Type == type).ToList();
            }
            //if (!string.IsNullOrEmpty(txtCusName))
            //{
            //    txtCusName = Helpers.Common.ChuyenThanhKhongDau(txtCusName);
            //    q = q.Where(x => Helpers.Common.ChuyenThanhKhongDau(x.CustomerName).Contains(txtCusName)).ToList();
            //}

            // Gộp textbox
            if (!string.IsNullOrEmpty(txtCusInfo))
            {
                q = q.Where(x => Helpers.Common.ChuyenThanhKhongDau(x.CustomerName).Contains(Helpers.Common.ChuyenThanhKhongDau(txtCusInfo)) || x.CustomerCode.Contains(txtCusInfo)).ToList();
            }

            //if (BranchId != null && BranchId.Value > 0)
            //{
            //    q = q.Where(x => x.BranchId == BranchId).ToList();
            //}


            //if (Helpers.Common.CurrentUser.BranchId != null)
            //{
            //    q = q.Where(x => x.BranchId == Helpers.Common.CurrentUser.BranchId.Value).ToList();
            //}


            if (ManagerId != null && ManagerId.Value > 0)
            {
                q = q.Where(x => x.ManagerStaffId == ManagerId).ToList();
            }
            if (CreateUserId != null && CreateUserId.Value > 0)
            {
                q = q.Where(x => x.CreatedUserId == CreateUserId).ToList();
            }
            return q;
        }
        public ActionResult ExportExcel(string startDate, string endDate, string txtCode, string txtCusCode, string txtCusInfo,
            string txtCusName, int? Status, int? BranchId, int? ManagerId, int? CreateUserId, string type, string productCode, string TargetCode, bool ExportExcel = false)
        {
            var data = IndexExport(startDate, endDate, txtCode,txtCusCode,txtCusName, txtCusInfo, Status, BranchId,ManagerId, CreateUserId, type, productCode, TargetCode);
            //var data = TempData["DataAdviseCardInBound"] as List<AdviseCardViewModel>;
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
            model.Content = model.Content.Replace("{Title}", "Danh sách phiếu tư vấn");

            Response.AppendHeader("content-disposition", "attachment;filename=" + "DS_Phieutuvan" + DateTime.Now.ToString("yyyyMMdd") + ".xls");
            Response.Charset = "";
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.ContentEncoding = System.Text.Encoding.UTF8;
            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);
            htw.WriteLine("<meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\">");
            Response.ContentType = "application/vnd.ms-excel";
            Response.Output.Write(sw.ToString());
            Response.Flush();

            Response.Write(model.Content);
            Response.End();

            return View();
        }

        string buildHtml(List<InquiryCardViewModel> data)
        {
            //Tạo table html chi tiết phiếu xuất
            string detailLists = "<table border=\"1\" class=\"invoice-detail\">\r\n";
            detailLists += "<thead>\r\n";
            detailLists += "	<tr>\r\n";
            detailLists += "		<th>STT</th>\r\n";
            detailLists += "		<th>Loại yêu cầu</th>\r\n";
            detailLists += "		<th>Phiếu yêu cầu</th>\r\n";
            detailLists += "		<th>Phiếu tư vấn/MBS</th>\r\n";
            detailLists += "		<th>Mã KH</ th>\r\n";
            detailLists += "		<th>Tên khách hàng</th>\r\n";
            detailLists += "		<th>Nhân viên quản lý</th>\r\n";
            detailLists += "		<th>Nhân viên tư vấn</th>\r\n";
            detailLists += "		<th>Nhân viên soi da</ th>\r\n";
            detailLists += "		<th>Nhân viên lập phiếu</th>\r\n";
            detailLists += "		<th>Ngày thực hiện</th>\r\n";
            detailLists += "		<th>Ngày tạo</th>\r\n";
            detailLists += "		<th>Ngày cập nhật</th>\r\n";
            detailLists += "	</tr>\r\n";
            detailLists += "</thead>\r\n";
            detailLists += "<tbody>\r\n";
            var index = 1;

            foreach (var item in data)
            {
                detailLists += "<tr>\r\n"
                + "<td class=\"text-center orderNo\">" + (index++) + "</td>\r\n"
                + "<td class=\"text-left \">" + item.Type + "</td>\r\n"
                + "<td class=\"text-left \">" + item.TargetModule + "</td>\r\n"
                + "<td class=\"text-left\">" + item.TargetCode + "</td>\r\n"
                + "<td class=\"text-left \">" + item.CustomerName + "</td>\r\n"
                + "<td class=\"text-left \">" + item.CustomerName + "</td>\r\n"
                + "<td class=\"text-left \">" + item.ManagerStaffName + "</td>\r\n"
                + "<td class=\"text-left \">" + item.ManagerName + "</td>\r\n"
                + "<td class=\"text-left \">" + item.ManagerStaffId + "</td>\r\n"
                + "<td class=\"text-left \">" + item.CreatedUserName + "</td>\r\n"
                + "<td class=\"text-left \">" + item.WorkDay + "</td>\r\n"
                + "<td class=\"text-left \">" + item.CreatedDate + "</td>\r\n"
                + "<td class=\"text-left \">" + item.ModifiedDate + "</td>\r\n"
                //+ "<td>" + (item.ModifiedDate.HasValue ? item.ModifiedDate.Value.ToString("dd/MM/yyyy HH:mm") : "") + "</td>"
                + "</tr>\r\n";
            }
            detailLists += "</tbody>\r\n";
            detailLists += "<tfoot style=\"font-weight:bold\">\r\n";
            detailLists += "</tfoot>\r\n</table>\r\n";

            return detailLists;
        }

        #endregion
        #region Create
        public ViewResult Create(int? TargetId, string TargetModule)
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
            //BranchId = intBrandID;
            var model = new InquiryCardViewModel();
            model.Code = Erp.BackOffice.Helpers.Common.GetOrderNo("InquiryCard");
            model.BranchId = intBrandID;
            model.TargetId = TargetId;
            model.TargetModule = TargetModule == null ? "Membership" : TargetModule;
            model.WorkDay = DateTime.Now;
            if (TargetId != null)
            {
                if (model.TargetModule == "AdviseCard")
                {
                    var info = adviseCardRepository.GetvwAdviseCardById(model.TargetId.Value);
                    model.TargetCode = info.Code;
                    model.Type = info.Type;
                    model.CustomerId = info.CustomerId;
                    model.CustomerCode = info.CustomerCode;
                    model.CustomerName = info.CustomerName;
                }
                if (model.TargetModule == "Membership")
                {
                    var info = membershipRepository.GetvwMembershipById(model.TargetId.Value);
                    model.TargetCode = info.Code;
                    model.Type = info.Type;
                    model.CustomerId = info.CustomerId;
                    model.CustomerCode = info.CustomerCode;
                    model.CustomerName = info.CustomerName;
                    model.ProductName = info.ProductName;
                    model.ProductId = info.ProductId;
                    model.DetailList = new List<InquiryCardDetailViewModel>();
                    model.DetailList = serviceStepsRepository.GetAllServiceSteps().Where(x => x.ProductId == info.ProductId).Select(x => new InquiryCardDetailViewModel
                    {
                        Id = 0,
                        Name = x.Name,
                        Note = x.Note,
                        TotalMinute = x.TotalMinute,
                        IsActived = true
                    }).ToList();
                    model.TotalMinute = model.DetailList.Sum(x => x.TotalMinute);
                }
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult Create(InquiryCardViewModel model, bool? IsPopup)
        {
          

            if (ModelState.IsValid)
            {
                var Membership = MembershipRepository.GetvwMembershipById(model.TargetId.Value);

                if (Membership != null && model.TargetModule == "Membership" && Membership.Status == "inprocess")
                {
                    TempData["FailedMessage"] = "Phiếu đã được sử dụng";
                    return RedirectToAction("_ClosePopup", "Home", new { area = "", FunctionCallback = "ClosePopupAndReloadPage" });
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

                using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.Snapshot }))
                {
                    try
                    {
                        var InquiryCard = new InquiryCard();
                        AutoMapper.Mapper.Map(model, InquiryCard);
                        InquiryCard.IsDeleted = false;

                        InquiryCard.CreatedUserId = WebSecurity.CurrentUserId;
                        InquiryCard.ModifiedUserId = WebSecurity.CurrentUserId;
                        InquiryCard.AssignedUserId = WebSecurity.CurrentUserId;
                        InquiryCard.CreatedDate = DateTime.Now;
                        InquiryCard.ModifiedDate = DateTime.Now;
                        InquiryCard.BranchId = Helpers.Common.NVL_NUM_NT_NEW(intBrandID);
                        var cus = customerRepository.GetCustomerById(InquiryCard.CustomerId.Value);
                        InquiryCard.ManagerUserId = cus.ManagerStaffId;
                        InquiryCard.Status = "pending";
                        InquiryCardRepository.InsertInquiryCard(InquiryCard);

                        if (model.TargetModule == "AdviseCard")
                        {
                            var _advise = adviseCardRepository.GetAdviseCardById(InquiryCard.TargetId.Value);
                            if (_advise != null)
                            {
                                //
                                InquiryCard.Type = _advise.Type;
                                //
                                _advise.IsActived = true;
                                _advise.ModifiedDate = DateTime.Now;
                                _advise.ModifiedUserId = WebSecurity.CurrentUserId;
                                adviseCardRepository.UpdateAdviseCard(_advise);
                            }
                        }
                        if (model.TargetModule == "Membership")
                        {
                            var _membership = membershipRepository.GetMembershipById(InquiryCard.TargetId.Value);
                            if (_membership != null)
                            {
                                //
                                InquiryCard.Type = _membership.Type;
                                //
                                _membership.Status = "inprocess";
                                _membership.ModifiedDate = DateTime.Now;
                                _membership.ModifiedUserId = WebSecurity.CurrentUserId;
                                membershipRepository.UpdateMembership(_membership);
                            }
                        }
                        InquiryCard.Code = Erp.BackOffice.Helpers.Common.GetOrderNo("InquiryCard", model.Code);
                        InquiryCardRepository.UpdateInquiryCard(InquiryCard);
                        Erp.BackOffice.Helpers.Common.SetOrderNo("InquiryCard");

                        //ghi log 
                        Erp.BackOffice.Controllers.HomeController.WriteLog(InquiryCard.Id, InquiryCard.Code, "đã tạo phiếu yêu cầu", "InquiryCard/Detail/" + InquiryCard.Id, Helpers.Common.CurrentUser.BranchId.Value);

                        //xuất kho sử dụng dịch vụ
                        var DetailServiceList = serviceDetailRepository.GetvwAllServiceDetail().Where(x => x.ServiceId == InquiryCard.ProductId)
                               .Select(x => new ProductViewModel
                               {
                                   Id = x.ProductId.Value,
                                   Quantity = x.Quantity.Value,
                                   Code = x.ProductCode,
                                   Name = x.ProductName
                               }).ToList();

                        foreach (var item in DetailServiceList)
                        {
                            var product = productRepository.GetProductById(item.Id);
                            if (product != null)
                            {
                                item.PriceOutbound = product.PriceOutbound.HasValue ? product.PriceOutbound.Value : 0;
                                item.Unit = product.Unit;
                            }
                        }
                        var warehouseDefault = warehouseRepository.GetAllWarehouse().Where(x => ((",GLT,").Contains(x.Categories)) && (x.BranchId == intBrandID)).FirstOrDefault();
                        if (warehouseDefault == null)
                        {
                            TempData[Globals.FailedMessageKey] = "Không tìm thấy kho gói liệu trình";
                            return RedirectToAction("Detail", new { Id = InquiryCard.Id });
                        }
                        string check = "";
                        foreach (var item in DetailServiceList)
                        {
                            var error = InventoryController.Check(item.Name, item.Id, item.LoCode, item.ExpiryDate, warehouseDefault.Id, 0, item.Quantity);
                            check += error;
                        }
                        if (!string.IsNullOrEmpty(check))
                        {
                            TempData["Seccess"] = App_GlobalResources.Wording.ArchiveFail + check;
                            return RedirectToAction("Create", model);

                        }

                        InsertProductOutbound(DetailServiceList, warehouseDefault, InquiryCard);

                        if (model.DetailList != null)
                        {

                            // lưu các bước làm dịch vụ
                            if (model.DetailList.Any())
                            {
                                foreach (var item in model.DetailList)
                                {
                                    var ins = new InquiryCardDetail();
                                    ins.IsDeleted = false;
                                    ins.CreatedUserId = WebSecurity.CurrentUserId;
                                    ins.ModifiedUserId = WebSecurity.CurrentUserId;
                                    ins.AssignedUserId = WebSecurity.CurrentUserId;
                                    ins.CreatedDate = DateTime.Now;
                                    ins.ModifiedDate = DateTime.Now;
                                    ins.IsActived = item.IsActived;
                                    ins.Name = item.Name;
                                    ins.Note = item.Note;
                                    ins.TotalMinute = item.TotalMinute;
                                    ins.InquiryCardId = InquiryCard.Id;
                                    inquiryCardDetailRepository.InsertInquiryCardDetail(ins);
                                }
                            }

                            Crm.Controllers.ProcessController.Run("InquiryCard", "Create", InquiryCard.Id, InquiryCard.ModifiedUserId, null, InquiryCard);
                            scope.Complete();
                        }
                    
                        if (IsPopup == true)
                        {
                            return RedirectToAction("_ClosePopup", "Home", new { area = "", FunctionCallback = "ClosePopupAndReloadPage" });
                        }
                        else
                        {
                            TempData[Globals.SuccessMessageKey] = App_GlobalResources.Wording.InsertSuccess;
                            return RedirectToAction("Detail", new { Id = InquiryCard.Id });
                        }
                    }
                    catch (DbUpdateException)
                    {
                        return Content("Fail");
                    }
                }

            }

            //ViewBag.FailedMessage = TempData["FailedMessage"];

            return View(model);
        }
        #endregion

        #region Edit
        public ActionResult Edit(int? Id)
        {
            var InquiryCard = InquiryCardRepository.GetvwInquiryCardById(Id.Value);
            if (InquiryCard != null && InquiryCard.IsDeleted != true)
            {
                var model = new InquiryCardViewModel();
                AutoMapper.Mapper.Map(InquiryCard, model);
                model.DetailList = new List<InquiryCardDetailViewModel>();
                model.DetailList = inquiryCardDetailRepository.GetAllInquiryCardDetail().Where(x => x.InquiryCardId == model.Id)
                    .Select(x => new InquiryCardDetailViewModel
                    {
                        Id = x.Id,
                        InquiryCardId = x.InquiryCardId,
                        IsActived = x.IsActived.Value,
                        Name = x.Name,
                        Note = x.Note,
                        TotalMinute = x.TotalMinute
                    }).ToList();

                return View(model);
            }
            if (Request.UrlReferrer != null)
                return Redirect(Request.UrlReferrer.AbsoluteUri);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult Edit(InquiryCardViewModel model, bool? IsPopup)
        {
            if (ModelState.IsValid)
            {
                if (Request["Submit"] == "Save")
                {
                    var InquiryCard = InquiryCardRepository.GetInquiryCardById(model.Id);
                    AutoMapper.Mapper.Map(model, InquiryCard);
                    InquiryCard.ModifiedUserId = WebSecurity.CurrentUserId;
                    InquiryCard.ModifiedDate = DateTime.Now;
                    var cus = customerRepository.GetCustomerById(InquiryCard.CustomerId.Value);
                    InquiryCard.ManagerUserId = cus.ManagerStaffId;
                    InquiryCardRepository.UpdateInquiryCard(InquiryCard);

                    //ghi log 
                    Erp.BackOffice.Controllers.HomeController.WriteLog(InquiryCard.Id, InquiryCard.Code, "đã cập nhật phiếu yêu cầu", "InquiryCard/Detail/" + InquiryCard.Id, Helpers.Common.CurrentUser.BranchId.Value);

                    var _advise = adviseCardRepository.GetAdviseCardById(InquiryCard.TargetId.Value);
                    if (_advise != null)
                    {
                        _advise.IsActived = true;
                        adviseCardRepository.UpdateAdviseCard(_advise);
                    }
                    var _listdata = inquiryCardDetailRepository.GetAllInquiryCardDetail().Where(x => x.InquiryCardId == InquiryCard.Id).ToList();
                    if (model.DetailList.Any(x => x.Id == 0))
                    {
                        //lưu danh sách thao tác thực hiện dịch vụ
                        foreach (var item in model.DetailList.Where(x => x.Id == 0 && x.Name != null))
                        {
                            var ins = new InquiryCardDetail();
                            ins.IsDeleted = false;
                            ins.CreatedUserId = WebSecurity.CurrentUserId;
                            ins.ModifiedUserId = WebSecurity.CurrentUserId;
                            ins.AssignedUserId = WebSecurity.CurrentUserId;
                            ins.CreatedDate = DateTime.Now;
                            ins.ModifiedDate = DateTime.Now;
                            ins.IsActived = item.IsActived;
                            ins.Name = item.Name;
                            ins.Note = item.Note;
                            ins.TotalMinute = item.TotalMinute;
                            ins.InquiryCardId = InquiryCard.Id;
                            inquiryCardDetailRepository.InsertInquiryCardDetail(ins);
                        }
                    }
                    var _delete = _listdata.Where(id1 => !model.DetailList.ToList().Any(id2 => id2.Id == id1.Id)).ToList();
                    foreach (var item in _delete)
                    {
                        inquiryCardDetailRepository.DeleteInquiryCardDetail(item.Id);
                    }
                    if (model.DetailList.Any(x => x.Id > 0))
                    {
                        var update = _listdata.Where(id1 => model.DetailList.ToList().Any(id2 => id2.Id == id1.Id)).ToList();
                        //lưu danh sách thao tác thực hiện dịch vụ
                        foreach (var item in model.DetailList.Where(x => x.Id > 0 && x.Name != null))
                        {
                            var _update = update.FirstOrDefault(x => x.Id == item.Id);
                            _update.Name = item.Name;
                            _update.TotalMinute = item.TotalMinute == null ? 0 : item.TotalMinute;
                            _update.Note = item.Note;
                            _update.IsActived = item.IsActived;
                            inquiryCardDetailRepository.UpdateInquiryCardDetail(_update);
                        }
                    }


                    if (IsPopup == true)
                    {
                        return RedirectToAction("_ClosePopup", "Home", new { area = "", FunctionCallback = "ClosePopupAndReloadPage" });
                    }
                    else
                    {
                        TempData[Globals.SuccessMessageKey] = App_GlobalResources.Wording.UpdateSuccess;
                        return RedirectToAction("Index");
                    }
                }

                return View(model);
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
            var InquiryCard = new vwInquiryCard();

            if (Id != null && Id.Value > 0)
            {
                InquiryCard = InquiryCardRepository.GetvwInquiryCardById(Id.Value);
            }

            if (!string.IsNullOrEmpty(TransactionCode))
            {
                InquiryCard = InquiryCardRepository.GetvwInquiryCardByCode(TransactionCode);
            }

            if (InquiryCard == null)
            {
                return RedirectToAction("Index");
            }

            if (InquiryCard != null && InquiryCard.IsDeleted != true)
            {
                var model = new InquiryCardViewModel();
                model.DetailList = new List<InquiryCardDetailViewModel>();
                AutoMapper.Mapper.Map(InquiryCard, model);

                if (model.TargetModule == "AdviseCard")
                {
                    var info = adviseCardRepository.GetvwAdviseCardById(model.TargetId.Value);
                    model.CounselorName = info.CounselorName;
                }

                model.DetailList = inquiryCardDetailRepository.GetAllInquiryCardDetail().Where(x => x.InquiryCardId == model.Id)
                    .Select(x => new InquiryCardDetailViewModel
                    {
                        Id = x.Id,
                        InquiryCardId = x.InquiryCardId,
                        IsActived = x.IsActived.Value,
                        Name = x.Name,
                        Note = x.Note,
                        TotalMinute = x.TotalMinute
                    }).ToList();
                model.DetailServiceList = new List<ServiceDetailViewModel>();
                model.DetailServiceList = serviceDetailRepository.GetvwAllServiceDetail().Where(x => x.ServiceId == InquiryCard.ProductId)
                   .Select(x => new ServiceDetailViewModel
                   {
                       Id = x.Id,
                       ProductId = x.ProductId,
                       Quantity = x.Quantity,
                       ServiceId = x.ServiceId,
                       ProductCode = x.ProductCode,
                       ProductName = x.ProductName
                   }).ToList();
                if (model.DetailServiceList.Any())
                {
                    foreach (var item in model.DetailServiceList)
                    {
                        item.ProductDetailList = new List<ProductDetailViewModel>();
                        item.ProductDetailList = productDetailRepository.GetvwAllProductDetail().Where(x => x.ProductId == item.ProductId)
                        .Select(x => new ProductDetailViewModel
                        {
                            MaterialName = x.MaterialName,
                            Id = x.Id,
                            ProductId = x.ProductId,
                            MaterialCode = x.MaterialCode,
                            MaterialId = x.MaterialId,
                            Quantity = x.Quantity
                        }).ToList();
                    }
                }


                return View(model);
            }
            if (Request.UrlReferrer != null)
                return Redirect(Request.UrlReferrer.AbsoluteUri);
            ViewBag.FailedMessage = TempData["FailedMessage"];
            return RedirectToAction("Index");
        }
        #endregion

        #region Delete
        [HttpPost]
        public ActionResult Delete()
        {
            try
            {
                string idDeleteAll = Request["Delete"];
                string[] arrDeleteId = idDeleteAll.Split(',');

                using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.Snapshot }))
                {
                    try
                    {
                        for (int i = 0; i < arrDeleteId.Count(); i++)
                        {
                            var item = InquiryCardRepository.GetInquiryCardById(int.Parse(arrDeleteId[i], CultureInfo.InvariantCulture));
                            if (item != null)
                            {
                                //if (item.CreatedUserId != Helpers.Common.CurrentUser.Id && Helpers.Common.CurrentUser.UserTypeId != 1)
                                //{
                                //    TempData["FailedMessage"] = "NotOwner";
                                //    return RedirectToAction("Index");
                                //}
                                var _listdata = inquiryCardDetailRepository.GetAllInquiryCardDetail().Where(x => x.InquiryCardId == item.Id).ToList();
                                foreach (var q in _listdata)
                                {
                                    inquiryCardDetailRepository.DeleteInquiryCardDetailRs(q.Id);
                                }
                                InquiryCardRepository.DeleteInquiryCardRs(item.Id);
                                //begin khoi phuc trang thai cho MBS
                                if (item.TargetModule == "Membership")
                                {
                                    var itemMBS = MembershipRepository.GetMembershipById(item.TargetId.Value);
                                    if (itemMBS != null)
                                    {
                                        itemMBS.Status = "pending";
                                        itemMBS.ModifiedUserId = Helpers.Common.CurrentUser.Id;
                                        MembershipRepository.UpdateMembership(itemMBS);
                                    }
                                }
                                //end khoi phuc trang thai cho MBS

                            }
                        }
                        scope.Complete();
                    }
                    catch (DbUpdateException)
                    {
                        return Content("Fail");
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

        #region LoadDetail

        public PartialViewResult LoadDetail(int? ProductId)
        {
            var model = new InquiryCardViewModel();
            model.DetailList = new List<InquiryCardDetailViewModel>();
            model.DetailList = serviceStepsRepository.GetAllServiceSteps().Where(x => x.ProductId == ProductId).Select(x => new InquiryCardDetailViewModel
            {
                Id = x.Id,
                Name = x.Name,
                Note = x.Note,
                TotalMinute = x.TotalMinute,
                IsActived = true
            }).ToList();
            return PartialView(model);
        }


        public PartialViewResult LoadProduct(string ProductName, int ProductId, string Node, int TotalMinute)
        {
            var model = new InquiryCardViewModel();
            model.ProductId = ProductId;
            model.ProductName = ProductName;
            model.Note = Node;
            model.TotalMinute = TotalMinute;
            return PartialView(model);
        }
        #endregion

        [AllowAnonymous]
        public JsonResult GetListJsonAll()
        {
            var pathimg = Helpers.Common.GetSetting("product-image-folder");
            var q = productRepository.GetAllvwService()
                .Select(item => new
                {
                    item.Id,
                    item.Name,
                    item.Type,
                    item.Code,
                    item.PriceOutbound,
                    item.CategoryCode,
                    item.Unit,
                    item.Image_Name,
                    item.TimeForService,
                    item.ProductGroup
                })
                .OrderBy(item => item.Name)
                .ToList();
            return Json(q.Select(item => new { item.Id, Code = item.Code, Image = Helpers.Common.KiemTraTonTaiHinhAnhHoapd(item.Image_Name, "product-image-folder", "product", pathimg), Type = item.Type, Unit = item.Unit, Price = item.PriceOutbound, Text = item.Code + " - " + item.Name + " (" + Helpers.Common.PhanCachHangNgan2(item.PriceOutbound) + "/" + item.Unit + ")", Name = item.Name, Value = item.Id, item.TimeForService, Categories = item.ProductGroup }), JsonRequestBehavior.AllowGet);
        }

        public void InsertProductOutbound(List<ProductViewModel> DetailServiceList, Warehouse warehouseDefault, InquiryCard inquiryCard)
        {
            using (var scope = new TransactionScope(TransactionScopeOption.Required, new System.TimeSpan(0, 30, 0)))
            {
                

                try
                {

                    //Xuất kho dịch vụ
                    var productOutbound = new Domain.Sale.Entities.ProductOutbound();

                    productOutbound.IsDeleted = false;
                    productOutbound.CreatedUserId = WebSecurity.CurrentUserId;
                    productOutbound.ModifiedUserId = WebSecurity.CurrentUserId;
                    productOutbound.CreatedDate = DateTime.Now;
                    productOutbound.ModifiedDate = DateTime.Now;
                    productOutbound.Type = "inquiry_card";

                    productOutbound.BranchId = warehouseDefault.BranchId;
                    productOutbound.WarehouseSourceId = warehouseDefault.Id;
                    productOutbound.TotalAmount = DetailServiceList.Sum(x => x.PriceOutbound * x.Quantity);
                    productOutbound.IsArchive = false;
                    productOutbound.Note = "Xuất gói liệu trình làm dịch vụ";
                    productOutboundRepository.InsertProductOutbound(productOutbound);

                    //Xuất kho vật tư
                    var warehouseMaterial = warehouseRepository.GetAllWarehouse().Where(x => ((",VT,").Contains(x.Categories)) && (x.BranchId == warehouseDefault.BranchId)).FirstOrDefault();

                    var check = "service";
                    var MaterialOutbound = new Domain.Sale.Entities.MaterialOutbound();
                    MaterialOutbound.IsDeleted = false;
                    MaterialOutbound.CreatedUserId = WebSecurity.CurrentUserId;
                    MaterialOutbound.ModifiedUserId = WebSecurity.CurrentUserId;
                    MaterialOutbound.CreatedDate = DateTime.Now;
                    MaterialOutbound.ModifiedDate = DateTime.Now;
                    MaterialOutbound.Type = check;
                    if (check == "service")
                    {
                        MaterialOutbound.WarehouseDestinationId = null;
                        ///Xuất kho vật tư
                        ///
                    }
                    // var branch = warehouseRepository.GetWarehouseById(MaterialOutbound.WarehouseSourceId.Value);
                    MaterialOutbound.BranchId = warehouseMaterial.BranchId;
                    MaterialOutbound.IsDone = false;
                    MaterialOutbound.TotalAmount = 0;//outboundDetails.Sum(item => item.Quantity * item.Price);
                    MaterialOutbound.WarehouseSourceId = warehouseMaterial.Id;
                    MaterialOutbound.Note = "Xuất cho dịch vụ";
                    materialOutboundRepository.InsertMaterialOutbound(MaterialOutbound);
                    var MaterialList = new List<ProductDetailViewModel>();


                    //Cập nhật lại mã xuất kho dịch vụ
                    productOutbound.Code = Erp.BackOffice.Helpers.Common.GetOrderNo("ProductOutbound");
                    productOutboundRepository.UpdateProductOutbound(productOutbound);
                    Erp.BackOffice.Helpers.Common.SetOrderNo("ProductOutbound");
                    // chi tiết phiếu xuất
                    if (DetailServiceList.Any())
                    {
                        foreach (var item in DetailServiceList)
                        {
                            ProductOutboundDetail ins_detail = new ProductOutboundDetail();
                            ins_detail.IsDeleted = false;
                            ins_detail.CreatedUserId = WebSecurity.CurrentUserId;
                            ins_detail.ModifiedUserId = WebSecurity.CurrentUserId;
                            ins_detail.CreatedDate = DateTime.Now;
                            ins_detail.ModifiedDate = DateTime.Now;
                            ins_detail.ProductOutboundId = productOutbound.Id;
                            ins_detail.Price = item.PriceOutbound;
                            ins_detail.Quantity = item.Quantity;
                            ins_detail.ProductId = item.Id;
                            ins_detail.Unit = item.Unit;
                            productOutboundRepository.InsertProductOutboundDetail(ins_detail);
                            ///Xuất kho vật tư

                            var ProductDetailList = new List<ProductDetailViewModel>();
                            ProductDetailList = productDetailRepository.GetvwAllProductDetail().Where(x => x.ProductId == item.Id)
                           .Select(x => new ProductDetailViewModel
                           {
                               MaterialName = x.MaterialName,
                               Id = x.Id,
                               ProductId = x.ProductId,
                               MaterialCode = x.MaterialCode,
                               MaterialId = x.MaterialId,
                               Quantity = x.Quantity
                           }).ToList();
                            if (ProductDetailList.Any())
                            {
                                foreach (var i in ProductDetailList)
                                {
                                    MaterialList.Add(i);
                                }
                            }
                            ///
                        }
                        //Chi tiết xuất vật tư
                        if (MaterialList.Any())
                        {
                            foreach (var item in MaterialList)
                            {

                                MaterialOutboundDetail materialOutboundDetail = new MaterialOutboundDetail();
                                materialOutboundDetail.MaterialId = item.MaterialId;
                                materialOutboundDetail.Price = 0;
                                materialOutboundDetail.Quantity = item.Quantity;
                                materialOutboundDetail.Unit = "";
                                // materialOutboundDetail.LoCode = "";//item.LoCode;
                                // materialOutboundDetail.ExpiryDate = "";// item.ExpiryDate;
                                materialOutboundDetail.IsDeleted = false;
                                materialOutboundDetail.CreatedUserId = WebSecurity.CurrentUserId;
                                materialOutboundDetail.ModifiedUserId = WebSecurity.CurrentUserId;
                                materialOutboundDetail.CreatedDate = DateTime.Now;
                                materialOutboundDetail.ModifiedDate = DateTime.Now;
                                materialOutboundDetail.MaterialOutboundId = MaterialOutbound.Id;
                                materialOutboundRepository.InsertMaterialOutboundDetail(materialOutboundDetail);

                            }
                        }
                    }
                    //cập nhật lại mã xuất kho
                    MaterialOutbound.Code = Erp.BackOffice.Helpers.Common.GetOrderNo("MaterialOutbound");
                    materialOutboundRepository.UpdateMaterialOutbound(MaterialOutbound);
                    Erp.BackOffice.Helpers.Common.SetOrderNo("MaterialOutbound");
                    //Thêm vào quản lý chứng từ
                    TransactionController.Create(new TransactionViewModel
                    {
                        TransactionModule = "MaterialOutbound",
                        TransactionCode = MaterialOutbound.Code,
                        TransactionName = "Xuất kho vật tư cho dịch vụ"
                    });
                    TransactionController.CreateRelationship(new TransactionRelationshipViewModel
                    {
                        TransactionA = productOutbound.Code,
                        TransactionB = MaterialOutbound.Code
                    });
                    //if (MaterialOutbound.WarehouseDestinationId != null && MaterialOutbound.WarehouseDestinationId.Value > 0)
                    //{
                    //  //  Erp.BackOffice.Sale.Controllers.MaterialInboundController.CreateFromMaterialOutbound(MaterialOutbound, outboundDetails);
                    //}
                    ProductOutboundController.Archive(productOutbound, TempData);
                    MaterialOutboundController.Archive(MaterialOutbound, TempData);
                    TransactionController.Create(new TransactionViewModel
                    {
                        TransactionModule = "ProductOutbound",
                        TransactionCode = productOutbound.Code,
                        TransactionName = "Xuất kho sử dụng dịch vụ"
                    });
                    TransactionController.Create(new TransactionViewModel
                    {
                        TransactionModule = "InquiryCard",
                        TransactionCode = inquiryCard.Code,
                        TransactionName = "Phiếu yêu cầu làm dịch vụ"
                    });
                    //Thêm chứng từ liên quan
                    TransactionController.CreateRelationship(new TransactionRelationshipViewModel
                    {
                        TransactionA = productOutbound.Code,
                        TransactionB = inquiryCard.Code
                    });
                    scope.Complete();
                  
                }
                catch (Exception)
                {
                    throw;
                }
            }

        }
    }
}
