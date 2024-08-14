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
using Erp.BackOffice.Areas.Cms.Models;
using Erp.Domain.Crm.Interfaces;
using Erp.BackOffice.Crm.Models;
using System.Transactions;
using System.Text.RegularExpressions;
using Erp.Domain.Account.Interfaces;
using System.Web;
using iTextSharp.text.pdf.qrcode;
using Org.BouncyCastle.Asn1.X509;
using Antlr.Runtime;
using Erp.Domain.Account.Helper;
using System.IO;
using fileio = System.IO.File;
using System.Web.UI;
using System.ComponentModel;
using System.Dynamic;
using System.Reflection;
using PagedList;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.Bibliography;
using Erp.BackOffice.Hubs;
using DocumentFormat.OpenXml.EMMA;
using DocumentFormat.OpenXml.Wordprocessing;
using iTextSharp.text;
using System.Web.Configuration;
using System.Threading.Tasks;
using System.Configuration;
using System.Net.Http;
using Newtonsoft.Json;
using Org.BouncyCastle.Asn1.Ocsp;
using DocumentFormat.OpenXml.Drawing.Diagrams;
using Telerik.Reporting.Drawing;
using static iTextSharp.text.pdf.codec.TiffWriter;
using DocumentFormat.OpenXml.Spreadsheet;
using System.Web.UI.WebControls;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Header;
using Hangfire;
using Microsoft.AspNet.SignalR;
using DocumentFormat.OpenXml.Vml.Office;
using static Erp.BackOffice.Crm.Controllers.LeadMeetingController;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;
using DocumentFormat.OpenXml.Math;
using System.Data;
using System.Net;
using Newtonsoft.Json.Linq;
using System.Web.Http;
using HttpPostAttribute = System.Web.Mvc.HttpPostAttribute;
using AllowAnonymousAttribute = System.Web.Mvc.AllowAnonymousAttribute;
using HttpGetAttribute = System.Web.Mvc.HttpGetAttribute;
using Erp.BackOffice.Account.Models;
using clr = System.Drawing.Color;
using Erp.Domain.Account.Entities;
using Erp.BackOffice.Areas.Crm.Models;
using Erp.Domain.Staff.Interfaces;

namespace Erp.BackOffice.Sale.Controllers
{
    [System.Web.Http.Authorize]
    [InitializeSimpleMembership]
    [Erp.BackOffice.Helpers.NoCacheHelper]
    public class AdviseCardController : Controller
    {
        private readonly IAdviseCardRepository AdviseCardRepository;
        private readonly IUserRepository userRepository;
        private readonly ICategoryRepository categoryRepository;
        private readonly IQuestionRepository questionRepository;
        private readonly IAnswerRepository answerRepository;
        private readonly IAdviseCardDetailRepository adviseCardDetailRepository;
        private readonly ITemplatePrintRepository templatePrintRepository;
        private readonly ICustomerRepository customerRepository;
        private readonly IProductOrServiceRepository productRepository;
        private readonly IBranchRepository branchRepository;
        private readonly IUserType_kdRepository userType_kdRepository;
        public AdviseCardController(
            IAdviseCardRepository _AdviseCard
            , IUserRepository _user
            , ICategoryRepository category
            , IQuestionRepository question
            , IAnswerRepository answer
            , IAdviseCardDetailRepository adviseCardDetail
                     , ITemplatePrintRepository templatePrint
            , ICustomerRepository customer
            , IProductOrServiceRepository _Product
            , IBranchRepository branch
            , IUserType_kdRepository userType_kd
            )
        {
            AdviseCardRepository = _AdviseCard;
            userRepository = _user;
            categoryRepository = category;
            questionRepository = question;
            answerRepository = answer;
            adviseCardDetailRepository = adviseCardDetail;
            templatePrintRepository = templatePrint;
            customerRepository = customer;
            productRepository = _Product;
            branchRepository = branch;
            userType_kdRepository = userType_kd;
        }

        // Constructor không tham số , giúp zns lập lịch
        public AdviseCardController()
        {
        }


        #region Index

        public ViewResult Index(string startDate, string endDate, string txtCode, string txtCusInfo, int? Status, int? BranchId, int? CounselorId, int? CreateUserId, string type)
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

            BranchId = int.Parse(strBrandID);

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

            if ((d_startDate == null) || (d_endDate == null))
            {
                return View();
            }

            IEnumerable<AdviseCardViewModel> q = SqlHelper.QuerySP<AdviseCardViewModel>("TOIUUCSKH_AllAdviseCard", new { @pd_startDate = d_startDate, @pd_endDate = d_endDate }).ToList();
            //AdviseCardRepository.GetvwAllAdviseCard().Where(x => x.CreatedDate >= d_startDate && x.CreatedDate <= d_endDate)
            //.Select(item => new AdviseCardViewModel
            //{
            //    Id = item.Id,
            //    CreatedUserId = item.CreatedUserId,
            //    //CreatedUserName = item.CreatedUserName,
            //    CreatedDate = item.CreatedDate,
            //    ModifiedUserId = item.ModifiedUserId,
            //    //ModifiedUserName = item.ModifiedUserName,
            //    ModifiedDate = item.ModifiedDate,
            //    Code = item.Code,
            //    Note = item.Note,
            //    //CustomerAddress=item.
            //    CustomerAddress = item.CustomerAddress,
            //    CustomerBirthday = item.CustomerBirthday,
            //    CreatedUserCode = item.CreatedUserCode,
            //    CounselorId = item.CounselorId,
            //    CreatedUserName = item.CreatedUserName,
            //    CounselorName = item.CounselorName,
            //    IsActived = item.IsActived,
            //    CustomerCode = item.CustomerCode,
            //    CustomerName = item.CustomerName,
            //    BranchId = item.BranchId,
            //    Type = item.Type,
            //    ManagerStaffName = item.ManagerStaffName
            //}).OrderByDescending(m => m.CreatedDate);

            if (!string.IsNullOrEmpty(txtCode))
            {
                txtCode = Helpers.Common.ChuyenThanhKhongDau(txtCode);
                q = q.Where(x => Helpers.Common.ChuyenThanhKhongDau(x.Code).Contains(txtCode)).ToList();
            }
            if (!string.IsNullOrEmpty(txtCusInfo))
            {

                q = q.Where(x => Helpers.Common.ChuyenThanhKhongDau(x.CustomerName).Contains(Helpers.Common.ChuyenThanhKhongDau(txtCusInfo)) || x.CustomerCode.Contains(txtCusInfo)).ToList();
            }
            //if (!string.IsNullOrEmpty(txtCusCode))
            //{            
            //        txtCusCode = Helpers.Common.ChuyenThanhKhongDau(txtCusCode);
            //        q = q.Where(x => Helpers.Common.ChuyenThanhKhongDau(x.Code).Contains(txtCusCode)).ToList();                         
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

            if (BranchId != null && BranchId.Value > 0)
            {
                q = q.Where(x => x.BranchId == BranchId).ToList();
            }

            //if (Helpers.Common.CurrentUser.BranchId != null)
            //{
            //    q = q.Where(x => x.BranchId == Helpers.Common.CurrentUser.BranchId.Value).ToList();
            //}

            if (CounselorId != null && CounselorId.Value > 0)
            {
                q = q.Where(x => x.CounselorId == CounselorId).ToList();
            }
            if (CreateUserId != null && CreateUserId.Value > 0)
            {
                q = q.Where(x => x.CreatedUserId == CreateUserId).ToList();
            }
            if (Status != null)
            {
                if (Status == 1)
                    q = q.Where(x => x.IsActived == true).ToList();
                else
                    q = q.Where(x => x.IsActived == false).ToList();
            }
            else
            {
                q = q.Where(x => x.IsActived == false).ToList();
            }
            ViewBag.SuccessMessage = TempData["SuccessMessage"];
            ViewBag.FailedMessage = TempData["FailedMessage"];
            ViewBag.AlertMessage = TempData["AlertMessage"];
            ViewBag.AdviseTypeError = TempData["AdviseTypeError"];
            return View(q);
        }
        #endregion

        #region Create
        public ViewResult Create(int? Id, AdviseCardViewModel model)
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
            //var AdviseCard = customerRepository.GetCustomerById(Id.Value);

            // var model = new AdviseCardViewModel();
            //var AdviseCard = AdviseCardRepository.GetvwAdviseCardById(Id.Value);
            if (Id == null)
            {
                //var model = new AdviseCardViewModel();
                model.Code = Erp.BackOffice.Helpers.Common.GetOrderNo("AdviseCard");
                model.BranchId = intBrandID;
                return View(model);
            }
            else
            {
                var AdviseCard = customerRepository.GetCustomerById(Id.Value);

                var Service = new AdviseCard();
                AutoMapper.Mapper.Map(model, Service);
                //AdviseCard.IsDeleted = false;
                //Service.CreatedUserId = WebSecurity.CurrentUserId;
                //Service.ModifiedUserId = WebSecurity.CurrentUserId;
                //Service.AssignedUserId = WebSecurity.CurrentUserId;
                Service.IsDeleted = false;
                //Service.CreatedDate = DateTime.Now;
                //Service.ModifiedDate = DateTime.Now;
                //Service.IsActived = false;
                model.CustomerName = AdviseCard.CompanyName;
                model.CustomerId = AdviseCard.Id;
                Service.BranchId = AdviseCard.BranchId;
                model.Code = Erp.BackOffice.Helpers.Common.GetOrderNo("AdviseCard");

                AdviseCardRepository.InsertAdviseCard(Service);
                AdviseCardRepository.UpdateAdviseCard(Service);
                Erp.BackOffice.Helpers.Common.SetOrderNo("AdviseCard");

                //Service.c
                //Service.
                return View(model);

            }
            return View();

        }

        [HttpPost]
        public ActionResult Create(AdviseCardViewModel model, bool? IsPopup)
        {
            var vwAdviseCard = AdviseCardRepository.GetvwAllAdviseCard();
            var q = vwAdviseCard.Select(item => new AdviseCardViewModel
            {
                Id = item.Id,
                CreatedUserId = item.CreatedUserId,
                CreatedDate = item.CreatedDate,
                ModifiedUserId = item.ModifiedUserId,
                //ModifiedUserName = item.ModifiedUserName,
                ModifiedDate = item.ModifiedDate,
                Code = item.Code,
                Note = item.Note,
                CreatedUserCode = item.CreatedUserCode,
                CounselorId = item.CounselorId,
                CreatedUserName = item.CreatedUserName,
                CounselorName = item.CounselorName,
                IsActived = item.IsActived,
                CustomerId = item.CustomerId,
                CustomerCode = item.CustomerCode,
                CustomerName = item.CustomerName,
                BranchId = item.BranchId,
                Type = item.Type
            }).ToList();

            //1 khách đc tạo nhìu phiếu
            //q = q.Where(x => x.CustomerId == model.CustomerId && x.BranchId== model.BranchId && x.Type == model.Type).ToList();

            //foreach (var item in q)
            //{
            //    if (item.Type == model.Type && item.BranchId == model.BranchId)
            //    {
            //        if (IsPopup == true)
            //        {
            //            TempData["AdviseTypeError"] = "Đã tồn tại phiếu tư vấn có cùng loại tư vấn";
            //            return RedirectToAction("_ClosePopup", "Home", new { area = "", FunctionCallback = "ClosePopupAndReloadPage" });
            //        }
            //        else
            //        {
            //            TempData["AdviseTypeError"] = "Đã tồn tại phiếu tư vấn có cùng loại tư vấn";
            //            return RedirectToAction("_ClosePopup", "Home", new { area = "", FunctionCallback = "ClosePopupAndReloadPage" });
            //        }
            //    }
            //} 
            if (ModelState.IsValid)
            {
                var AdviseCard = new AdviseCard();
                AutoMapper.Mapper.Map(model, AdviseCard);
                AdviseCard.IsDeleted = false;
                AdviseCard.CreatedUserId = WebSecurity.CurrentUserId;
                AdviseCard.ModifiedUserId = WebSecurity.CurrentUserId;
                AdviseCard.AssignedUserId = WebSecurity.CurrentUserId;
                AdviseCard.CreatedDate = DateTime.Now;
                AdviseCard.ModifiedDate = DateTime.Now;
                AdviseCard.IsActived = false;
                //AdviseCard.BranchId = Erp.BackOffice.Helpers.Common.CurrentUser.BranchId;
                AdviseCardRepository.InsertAdviseCard(AdviseCard);
                //cập nhật lại mã phiếu yêu cầu
                AdviseCard.Code = Erp.BackOffice.Helpers.Common.GetOrderNo("AdviseCard", model.Code);
                AdviseCardRepository.UpdateAdviseCard(AdviseCard);

                //ghi log 
                Erp.BackOffice.Controllers.HomeController.WriteLog(AdviseCard.Id, AdviseCard.Code, "đã tạo phiếu tư vấn", "AdviseCard/Detail/" + AdviseCard.Id, Helpers.Common.CurrentUser.BranchId.Value);

                Erp.BackOffice.Helpers.Common.SetOrderNo("AdviseCard");
                ///
                if (IsPopup == true)
                {
                    return RedirectToAction("_ClosePopup", "Home", new { area = "", FunctionCallback = "ClosePopupAndReloadPage" });
                }
                else
                {
                    TempData[Globals.SuccessMessageKey] = App_GlobalResources.Wording.InsertSuccess;
                    //gọi JavaScriptToRun để chạy code Jquery trong View
                    model.JavaScriptToRun = "CloseCurrentPopup(" + AdviseCard.Id + ")";
                    //return RedirectToAction("Detail", "AdviseCard", new { Id = AdviseCard.Id });
                    return View(model);
                }


            }


            return View(model);
        }
        #endregion
        #region export
        public List<AdviseCardViewModel> IndexExport(string startDate, string endDate, string txtCode, string txtCusInfo, int? Status, int? BranchId, int? CounselorId, int? CreateUserId, string type)
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

            var q = AdviseCardRepository.GetvwAllAdviseCard().Where(x => x.CreatedDate >= d_startDate && x.CreatedDate <= d_endDate)
                 .Select(item => new AdviseCardViewModel
                 {
                     Id = item.Id,
                     CreatedUserId = item.CreatedUserId,
                     //CreatedUserName = item.CreatedUserName,
                     CreatedDate = item.CreatedDate,
                     ModifiedUserId = item.ModifiedUserId,
                     //ModifiedUserName = item.ModifiedUserName,
                     ModifiedDate = item.ModifiedDate,
                     Code = item.Code,
                     Note = item.Note,
                     //CustomerAddress=item.
                     CustomerAddress = item.CustomerAddress,
                     CustomerBirthday = item.CustomerBirthday,
                     CreatedUserCode = item.CreatedUserCode,
                     CounselorId = item.CounselorId,
                     CreatedUserName = item.CreatedUserName,
                     CounselorName = item.CounselorName,
                     IsActived = item.IsActived,
                     CustomerCode = item.CustomerCode,
                     CustomerName = item.CustomerName,
                     BranchId = item.BranchId,
                     Type = item.Type,
                     ManagerStaffName = item.ManagerStaffName
                 }).OrderByDescending(m => m.CreatedDate).ToList();

            if (!string.IsNullOrEmpty(txtCode))
            {
                txtCode = Helpers.Common.ChuyenThanhKhongDau(txtCode);
                q = q.Where(x => Helpers.Common.ChuyenThanhKhongDau(x.Code).Contains(txtCode)).ToList();
            }
            if (!string.IsNullOrEmpty(txtCusInfo))
            {

                q = q.Where(x => Helpers.Common.ChuyenThanhKhongDau(x.CustomerName).Contains(Helpers.Common.ChuyenThanhKhongDau(txtCusInfo)) || x.CustomerCode.Contains(txtCusInfo)).ToList();
            }
            //if (!string.IsNullOrEmpty(txtCusCode))
            //{            
            //        txtCusCode = Helpers.Common.ChuyenThanhKhongDau(txtCusCode);
            //        q = q.Where(x => Helpers.Common.ChuyenThanhKhongDau(x.Code).Contains(txtCusCode)).ToList();                         
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

            if (BranchId != null && BranchId.Value > 0)
            {
                q = q.Where(x => x.BranchId == BranchId).ToList();
            }

            //if (Helpers.Common.CurrentUser.BranchId != null)
            //{
            //    q = q.Where(x => x.BranchId == Helpers.Common.CurrentUser.BranchId.Value).ToList();
            //}

            if (CounselorId != null && CounselorId.Value > 0)
            {
                q = q.Where(x => x.CounselorId == CounselorId).ToList();
            }
            if (CreateUserId != null && CreateUserId.Value > 0)
            {
                q = q.Where(x => x.CreatedUserId == CreateUserId).ToList();
            }
            if (Status != null)
            {
                if (Status == 1)
                    q = q.Where(x => x.IsActived == true).ToList();
                else
                    q = q.Where(x => x.IsActived == false).ToList();
            }
            else
            {
                q = q.Where(x => x.IsActived == false).ToList();
            }
            return q;
        }
        public ActionResult ExportExcel(string startDate, string endDate, string txtCode, string txtCusInfo, int? Status, int? BranchId, int? CounselorId, int? CreateUserId, string type, bool ExportExcel = false)
        {
            var data = IndexExport(startDate, endDate, txtCode, txtCusInfo, Status, BranchId, CounselorId, CreateUserId, type);
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

        string buildHtml(List<AdviseCardViewModel> data)
        {
            //Tạo table html chi tiết phiếu xuất
            string detailLists = "<table border=\"1\" class=\"invoice-detail\">\r\n";
            detailLists += "<thead>\r\n";
            detailLists += "	<tr>\r\n";
            detailLists += "		<th>STT</th>\r\n";
            detailLists += "		<th>Loại tư vấn</th>\r\n";
            detailLists += "		<th>Mã phiếu tư vấn</th>\r\n";
            detailLists += "		<th>Mã KH</th>\r\n";
            detailLists += "		<th>Tên khách hàng</ th>\r\n";
            detailLists += "		<th>Địa chỉ</th>\r\n";
            detailLists += "		<th>Ngày sinh</th>\r\n";
            detailLists += "		<th>Nhân viên tư vấn</th>\r\n";
            detailLists += "		<th>Nhân viên lập phiếu</th>\r\n";
            detailLists += "		<th>Nhân viên quản lý</th>\r\n";
            detailLists += "		<th>Ngày tạo</th>\r\n";
            detailLists += "		<th>Ngày cập nhật</th>\r\n";
            detailLists += "		<th>Lập phiếu YC</th>\r\n";
            detailLists += "	</tr>\r\n";
            detailLists += "</thead>\r\n";
            detailLists += "<tbody>\r\n";
            var index = 1;

            foreach (var item in data)
            {
                detailLists += "<tr>\r\n"
                + "<td class=\"text-center orderNo\">" + (index++) + "</td>\r\n"
                + "<td class=\"text-left \">" + item.ListAdviseType + "</td>\r\n"
                + "<td class=\"text-left \">" + item.CounselorCode + "</td>\r\n"
                + "<td class=\"text-left\">" + item.CustomerCode + "</td>\r\n"
                + "<td class=\"text-left \">" + item.CustomerName + "</td>\r\n"
                + "<td class=\"text-left \">" + item.CustomerAddress + "</td>\r\n"
                + "<td class=\"text-left \">" + item.CustomerBirthday + "</td>\r\n"
                + "<td class=\"text-left \">" + item.CounselorName + "</td>\r\n"
                + "<td class=\"text-left \">" + item.CreatedUserName + "</td>\r\n"
                 + "<td class=\"text-left \">" + item.ManagerStaffName + "</td>\r\n"
                + "<td class=\"text-left \">" + item.CreatedDate + "</td>\r\n"
                + "<td class=\"text-left \">" + item.ModifiedDate + "</td>\r\n"
                + "<td class=\"text-left \">" + (item.IsActived == true ? "đã lập phiếu" : "chưa lập phiếu") + "</td>\r\n"
                //+ "<td>" + (item.ModifiedDate.HasValue ? item.ModifiedDate.Value.ToString("dd/MM/yyyy HH:mm") : "") + "</td>"
                + "</tr>\r\n";
            }
            detailLists += "</tbody>\r\n";
            detailLists += "<tfoot style=\"font-weight:bold\">\r\n";
            detailLists += "</tfoot>\r\n</table>\r\n";

            return detailLists;
        }

        #endregion

        #region Edit
        public ActionResult Edit(int? Id)
        {
            var AdviseCard = AdviseCardRepository.GetvwAdviseCardById(Id.Value);
            if (AdviseCard != null && AdviseCard.IsDeleted != true)
            {
                var model = new AdviseCardViewModel();
                AutoMapper.Mapper.Map(AdviseCard, model);

                //if (model.CreatedUserId != Helpers.Common.CurrentUser.Id && Helpers.Common.CurrentUser.UserTypeId != 1)
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
        public ActionResult Edit(AdviseCardViewModel model, bool? IsPopup)
        {
            if (ModelState.IsValid)
            {
                if (Request["Submit"] == "Save")
                {
                    var AdviseCard = AdviseCardRepository.GetAdviseCardById(model.Id);
                    AutoMapper.Mapper.Map(model, AdviseCard);
                    AdviseCard.ModifiedUserId = WebSecurity.CurrentUserId;
                    AdviseCard.ModifiedDate = DateTime.Now;
                    AdviseCardRepository.UpdateAdviseCard(AdviseCard);
                    //ghi log 
                    Erp.BackOffice.Controllers.HomeController.WriteLog(AdviseCard.Id, AdviseCard.Code, "đã cập nhật phiếu tư vấn", "AdviseCard/Detail/" + AdviseCard.Id, Helpers.Common.CurrentUser.BranchId.Value);

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
        public ActionResult Detail(int? Id)
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

            var AdviseCard = AdviseCardRepository.GetvwAdviseCardById(Id.Value);
            var _category = categoryRepository.GetAllCategories();

            var _list_question = questionRepository.GetAllQuestion().Where(x => x.IsActivated == true)
                              .Select(item1 => new QuestionViewModel
                              {
                                  Id = item1.Id,
                                  Name = item1.Name,
                                  Type = item1.Type,
                                  Category = item1.Category,
                                  OrderNo = item1.OrderNo,
                                  Content = item1.Content
                              }).OrderByDescending(m => m.OrderNo);

            var _answer = answerRepository.GetAllAnswer().Where(x => x.IsActivated == true).Select(item1 => new AnswerViewModel
            {
                Id = item1.Id,
                Content = item1.Content,
                QuestionId = item1.QuestionId,
                OrderNo = item1.OrderNo
            }).OrderByDescending(m => m.OrderNo);

            if (AdviseCard != null && AdviseCard.IsDeleted != true)
            {
                var model = new AdviseCardViewModel();
                AutoMapper.Mapper.Map(AdviseCard, model);

                model.ListAdviseType = new List<CategoryViewModel>();

                model.ListAdviseType = _category.Where(x => x.Code == AdviseCard.Type).Select(x => new CategoryViewModel
                {
                    Code = x.Code,
                    Id = x.Id,
                    Name = x.Name,
                    OrderNo = x.OrderNo,
                    Value = x.Value,
                    ParentId = x.ParentId,
                    Level = 1
                }).OrderBy(x => x.OrderNo).ToList();

                foreach (var item in model.ListAdviseType)
                {
                    item.ListQuestion = _list_question.Where(x => x.Category == item.Value).OrderBy(x => x.OrderNo).ToList();
                    foreach (var item1 in item.ListQuestion)
                    {
                        item1.DetailList = _answer.Where(x => x.QuestionId == item1.Id).OrderBy(x => x.OrderNo).ToList();
                    }

                    var aa = _category.Where(x => x.Code == item.Value).Select(x => new CategoryViewModel
                    {
                        Code = x.Code,
                        Id = x.Id,
                        Name = x.Name,
                        OrderNo = x.OrderNo,
                        Value = x.Value,
                        ParentId = x.ParentId,
                        Level = 2
                    }).OrderBy(x => x.OrderNo).ToList();

                    if (aa.Count() > 0)
                    {
                        foreach (var q in aa)
                        {
                            q.ListQuestion = _list_question.Where(x => x.Category == q.Value).OrderBy(x => x.OrderNo).ToList();
                            foreach (var p in q.ListQuestion)
                            {
                                p.DetailList = _answer.Where(x => x.QuestionId == p.Id).OrderBy(x => x.OrderNo).ToList();
                            }
                        }
                        model.ListAdviseType = model.ListAdviseType.Union(aa).ToList();
                    }
                }
                TempData["model_AdviseCard"] = model;
                var productList = SqlHelper.QuerySP<ProductViewModel>("spGetProductAndService", new
                {
                    BranchId = intBrandID,
                    WarehouseId = 0,
                    ProductGroup = "",
                    CategoryCode = "",
                    Manufacturer = "",
                    CityId = "",
                    DistrictId = "",
                    Origin = ""

                }).Where(item => item.Type != "productpackage").ToList();
                // var productList = productRepository.GetAllProduct().Where(item => item.Type != "productpackage" && item.NotUse != true )
                //.Select(item => new ProductViewModel
                //{
                //    Code = item.Code,
                //    Barcode = item.Barcode,
                //    Name = item.Name,
                //    Id = item.Id,
                //    CategoryCode = item.CategoryCode,
                //    PriceInbound = item.PriceOutbound,
                //    Unit = item.Unit,
                //    Image_Name = item.Image_Name
                //});

                ViewBag.productList = productList;
                TempData["AdviseCardId"] = Id;
                ViewBag.AdviseCardId = Id;
                //TempData["Question"] = model.ListAdviseType.ListQuestion;
                return View(model);
            }



            if (Request.UrlReferrer != null)
                return Redirect(Request.UrlReferrer.AbsoluteUri);
            return RedirectToAction("Index");
        }

        [AllowAnonymous]
        public PartialViewResult LoadAdviseCard()
        {
            int? AdviseCardId = (int?)TempData["AdviseCardId"];
            TempData.Keep();
            var model = adviseCardDetailRepository.GetAllAdviseCardDetail();
            model = model.Where(x => x.TargetModule == "Product" && x.AdviseCardId == AdviseCardId);

            ViewBag.AdviseCardId = AdviseCardId;
            return PartialView(model);
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
                for (int i = 0; i < arrDeleteId.Count(); i++)
                {
                    var item = AdviseCardRepository.GetAdviseCardById(int.Parse(arrDeleteId[i], CultureInfo.InvariantCulture));
                    if (item != null)
                    {
                        var list = adviseCardDetailRepository.GetAllAdviseCardDetailById(item.Id).ToList();
                        foreach (var q in list)
                        {
                            adviseCardDetailRepository.DeleteAdviseCardDetailRs(q.Id);
                        }
                        AdviseCardRepository.DeleteAdviseCardRs(item.Id);
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

        [HttpPost]
        public ActionResult SaveItem(int AdviseCardId, int QuestionId, int TargetId, string TargetModule, string content, string Note, bool IsChecked)
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.Snapshot }))
            {

                try
                {
                    if (TargetId != null)
                    {
                        var update = adviseCardDetailRepository.GetAdviseCardDetailById(AdviseCardId, QuestionId, TargetId);
                        if (update == null)
                        {
                            var add = new AdviseCardDetail();
                            add.IsDeleted = false;
                            add.CreatedUserId = WebSecurity.CurrentUserId;
                            add.ModifiedUserId = WebSecurity.CurrentUserId;
                            add.CreatedDate = DateTime.Now;
                            add.ModifiedDate = DateTime.Now;
                            add.Content = content;
                            add.Note = Note;
                            add.AdviseCardId = AdviseCardId;
                            add.QuestionId = QuestionId;
                            add.TargetId = TargetId;
                            add.TargetModule = TargetModule;
                            adviseCardDetailRepository.InsertAdviseCardDetail(add);
                        }
                        else
                        {
                            if (IsChecked)
                            {
                                update.ModifiedUserId = WebSecurity.CurrentUserId;
                                update.ModifiedDate = DateTime.Now;
                                update.Content = content;
                                update.Note = Note;
                                update.AdviseCardId = AdviseCardId;
                                update.QuestionId = QuestionId;
                                update.TargetId = TargetId;
                                update.TargetModule = TargetModule;
                                adviseCardDetailRepository.UpdateAdviseCardDetail(update);
                            }
                            else
                            {
                                adviseCardDetailRepository.DeleteAdviseCardDetail(update.Id);
                            }
                        }
                        scope.Complete();
                    }
                }
                catch (DbUpdateException)
                {
                    return Content("error");
                }
            }
            return Content("success");
        }
        [HttpPost]
        public ActionResult SaveItemProductService(int AdviseCardId, int TargetId, string TargetModule, bool IsChecked)
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.Snapshot }))
            {

                try
                {
                    int QuestionId = 0;
                    var data = Erp.BackOffice.Helpers.SelectListHelper.GetAllProductAndService().Where(x => x.Id == TargetId).ToList();
                    string type = data.FirstOrDefault().Type;

                    var _list_question = questionRepository.GetAllQuestion().Where(x => x.IsActivated == true)
                             .Select(item1 => new QuestionViewModel
                             {
                                 Id = item1.Id,
                                 Name = item1.Name,
                                 Type = item1.Type,
                                 Category = item1.Category,
                                 OrderNo = item1.OrderNo,
                                 Content = item1.Content
                             }).OrderByDescending(m => m.OrderNo);

                    if (type == "product")
                    {
                        QuestionId = _list_question.Where(x => x.Name == "SẢN PHẨM").FirstOrDefault().Id;
                    }
                    else
                        QuestionId = _list_question.Where(x => x.Name == "DỊCH VỤ").FirstOrDefault().Id;
                    var update = adviseCardDetailRepository.GetAdviseCardDetailById(AdviseCardId, QuestionId, TargetId);
                    if (update == null)
                    {
                        var add = new AdviseCardDetail();
                        add.IsDeleted = false;
                        add.CreatedUserId = WebSecurity.CurrentUserId;
                        add.ModifiedUserId = WebSecurity.CurrentUserId;
                        add.CreatedDate = DateTime.Now;
                        add.ModifiedDate = DateTime.Now;
                        add.AdviseCardId = AdviseCardId;
                        add.QuestionId = QuestionId;
                        add.TargetId = TargetId;
                        add.TargetModule = TargetModule;
                        adviseCardDetailRepository.InsertAdviseCardDetail(add);
                    }
                    else
                    {
                        if (IsChecked)
                        {
                            update.ModifiedUserId = WebSecurity.CurrentUserId;
                            update.ModifiedDate = DateTime.Now;

                            update.AdviseCardId = AdviseCardId;
                            update.QuestionId = QuestionId;
                            update.TargetId = TargetId;
                            update.TargetModule = TargetModule;
                            adviseCardDetailRepository.UpdateAdviseCardDetail(update);
                        }
                        else
                        {
                            adviseCardDetailRepository.DeleteAdviseCardDetail(update.Id);
                        }
                    }
                    scope.Complete();
                }
                catch (DbUpdateException)
                {
                    return Content("error");
                }
            }
            return Content("success");
        }
        [HttpPost]
        public ActionResult Save(string Note)
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.Snapshot }))
            {

                try
                {
                    var data = TempData["model_AdviseCard"] as AdviseCardViewModel;
                    var up = AdviseCardRepository.GetAdviseCardById(data.Id);
                    up.Note = Note;
                    AdviseCardRepository.UpdateAdviseCard(up);
                    scope.Complete();
                }
                catch (DbUpdateException)
                {
                    return Content("error");
                }
            }
            return Content("success");
        }

        #region  - LoadAdviseCardDetailById -
        public JsonResult LoadAdviseCardDetailById(int AdviseCardId)
        {
            var list = adviseCardDetailRepository.GetAllAdviseCardDetailById(AdviseCardId).Select(x => new
            {
                Id = x.Id,
                TargetId = x.TargetId,
                Content = x.Content,
                Note = x.Note,
                QuestionId = x.QuestionId,
                TargetModule = x.TargetModule
            }).ToList();

            return Json(list, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Print
        public ActionResult Print(int? Id)
        {
            var AdviseCard = AdviseCardRepository.GetvwAdviseCardById(Id.Value);
            var _category = categoryRepository.GetAllCategories();

            var _list_question = questionRepository.GetAllQuestion().Where(x => x.IsActivated == true)
                              .Select(item1 => new QuestionViewModel
                              {
                                  Id = item1.Id,
                                  Name = item1.Name,
                                  Type = item1.Type,
                                  Category = item1.Category,
                                  OrderNo = item1.OrderNo,
                                  Content = item1.Content
                              }).OrderByDescending(m => m.OrderNo);

            var _answer = answerRepository.GetAllAnswer().Where(x => x.IsActivated == true).Select(item1 => new AnswerViewModel
            {
                Id = item1.Id,
                Content = item1.Content,
                QuestionId = item1.QuestionId,
                OrderNo = item1.OrderNo
            }).OrderByDescending(m => m.OrderNo);

            if (AdviseCard != null && AdviseCard.IsDeleted != true)
            {
                var model = new AdviseCardViewModel();
                AutoMapper.Mapper.Map(AdviseCard, model);
                model.ListAdviseType = new List<CategoryViewModel>();
                model.ListAdviseType = _category.Where(x => x.Code == AdviseCard.Type).Select(x => new CategoryViewModel
                {
                    Code = x.Code,
                    Id = x.Id,
                    Name = x.Name,
                    OrderNo = x.OrderNo,
                    Value = x.Value,
                    ParentId = x.ParentId,
                    Level = 1
                }).OrderBy(x => x.OrderNo).ToList();

                foreach (var item in model.ListAdviseType)
                {
                    item.ListQuestion = _list_question.Where(x => x.Category == item.Value).OrderBy(x => x.OrderNo).ToList();
                    foreach (var item1 in item.ListQuestion)
                    {
                        item1.DetailList = _answer.Where(x => x.QuestionId == item1.Id).OrderBy(x => x.OrderNo).ToList();
                    }

                    var aa = _category.Where(x => x.Code == item.Value).Select(x => new CategoryViewModel
                    {
                        Code = x.Code,
                        Id = x.Id,
                        Name = x.Name,
                        OrderNo = x.OrderNo,
                        Value = x.Value,
                        ParentId = x.ParentId,
                        Level = 2
                    }).OrderBy(x => x.OrderNo).ToList();

                    if (aa.Count() > 0)
                    {
                        foreach (var q in aa)
                        {
                            q.ListQuestion = _list_question.Where(x => x.Category == q.Value).OrderBy(x => x.OrderNo).ToList();
                            foreach (var p in q.ListQuestion)
                            {
                                p.DetailList = _answer.Where(x => x.QuestionId == p.Id).OrderBy(x => x.OrderNo).ToList();
                            }
                        }
                        model.ListAdviseType = model.ListAdviseType.Union(aa).ToList();
                    }
                }
                var list = adviseCardDetailRepository.GetAllAdviseCardDetailById(model.Id).Select(x => new AdviseCardDetailViewModel
                {
                    Id = x.Id,
                    TargetId = x.TargetId,
                    Content = x.Content,
                    Note = x.Note,
                    QuestionId = x.QuestionId,
                    TargetModule = x.TargetModule
                }).ToList();
                var _model = new TemplatePrintViewModel();
                //lấy logo công ty
                var logo = Erp.BackOffice.Helpers.Common.GetSetting("LogoCompany");
                var company = Erp.BackOffice.Helpers.Common.GetSetting("companyName");
                var address = Erp.BackOffice.Helpers.Common.GetSetting("addresscompany");
                var phone = Erp.BackOffice.Helpers.Common.GetSetting("phonecompany");
                var fax = Erp.BackOffice.Helpers.Common.GetSetting("faxcompany");
                var bankcode = Erp.BackOffice.Helpers.Common.GetSetting("bankcode");
                var bankname = Erp.BackOffice.Helpers.Common.GetSetting("bankname");
                var ImgLogo = "<div class=\"logo\"><img src=" + logo + " height=\"63\" /></div>";
                //lấy template phiếu xuất.
                var template = templatePrintRepository.GetAllTemplatePrint().Where(x => x.Code.Contains("AdviseCard")).OrderByDescending(x => x.CreatedDate).FirstOrDefault();
                //truyền dữ liệu vào template.
                _model.Content = template.Content;

                _model.Content = _model.Content.Replace("{DataTable}", buildHtml1(model, list));
                _model.Content = _model.Content.Replace("{System.Logo}", ImgLogo);
                _model.Content = _model.Content.Replace("{System.CompanyName}", company);
                _model.Content = _model.Content.Replace("{System.AddressCompany}", address);
                _model.Content = _model.Content.Replace("{System.PhoneCompany}", phone);
                _model.Content = _model.Content.Replace("{System.Fax}", fax);
                _model.Content = _model.Content.Replace("{System.BankCodeCompany}", bankcode);
                _model.Content = _model.Content.Replace("{System.BankNameCompany}", bankname);
                _model.Content = _model.Content.Replace("{PrintDate}", DateTime.Now.ToString("dd/MM/yyyy HH:mm"));
                _model.Content = _model.Content.Replace("{CustomerCode}", model.CustomerCode);
                _model.Content = _model.Content.Replace("{CustomerName}", model.CustomerName);
                _model.Content = _model.Content.Replace("{Note}", model.Note);
                _model.Content = _model.Content.Replace("{CreateStaffName}", model.CreatedUserName);
                _model.Content = _model.Content.Replace("{CounselorName}", model.CounselorName);
                //Bổ sung
                _model.Content = _model.Content.Replace("{Birthday}", model.CustomerBirthday.Value.ToShortDateString());
                _model.Content = _model.Content.Replace("{sdt}", model.CustomerPhone);
                _model.Content = _model.Content.Replace("{Address}", model.CustomerAddress);
                _model.Content = _model.Content.Replace("{Title}", model.Type == "SkinScan" ? "PHIẾU CHĂM SÓC DA" : "PHIẾU CHĂM SÓC TÓC");

                return View(_model);
            }
            if (Request.UrlReferrer != null)
                return Redirect(Request.UrlReferrer.AbsoluteUri);
            return RedirectToAction("Index");
        }
        #endregion

        string buildHtml1(AdviseCardViewModel model, List<AdviseCardDetailViewModel> detail)
        {

            string detailLists = "";

            foreach (var item in model.ListAdviseType.Where(x => x.Level == 1))
            {
                detailLists += "<p><strong>" + item.Name + "</strong></p>\r\n";
                if (item.Value == "SkinScan1")
                {
                    detailLists += buildHtml_Partial1(item, detail);
                }
                if (item.Value == "SkinScan2" || item.Value == "SkinScan6" || item.Value == "CheckingHair1" || item.Value == "CheckingHair2" || item.Value == "CheckingHair3" || item.Value == "CheckingHair5")
                {
                    detailLists += buildHtml_Partial2(item, detail);
                }
                if (item.Value == "SkinScan3")
                {
                    detailLists += buildHtml_Partial3(item, detail);
                }
                if (item.Value == "SkinScan4")
                {
                    detailLists += buildHtml_Partial4(item, detail);
                }
                if (item.Value == "SkinScan5")
                {
                    detailLists += buildHtml_Partial6(item, detail);
                }
                if (item.Value == "CheckingHair4")
                {
                    var list = model.ListAdviseType.Where(x => x.Level == 2 && x.Code == item.Value).ToList();
                    detailLists += buildHtml_Partial5(list, detail);
                }
                else
                {
                    if (model.ListAdviseType.Any(x => x.Level == 2 && x.Code == item.Value))
                    {
                        foreach (var ii in model.ListAdviseType.Where(x => x.Level == 2 && x.Code == item.Value))
                        {
                            detailLists += "<p>" + item.Name + "</p>\r\n";
                            detailLists += buildHtml_Partial2(item, detail);
                        }
                    }
                }
            }
            return detailLists;
        }
        string buildHtml_Partial1(CategoryViewModel item, List<AdviseCardDetailViewModel> detail)
        {
            string detailLists = "";
            detailLists += "<p><table class=\"invoice-detail\">\r\n";
            detailLists += "<thead>\r\n";
            detailLists += "	<tr>\r\n";
            detailLists += "		<th>Nội dung</th>\r\n";
            detailLists += "		<th>Chi tiết</th>\r\n";
            detailLists += "		<th>Ghi chú</th>\r\n";
            detailLists += "	</tr>\r\n";
            detailLists += "</thead>\r\n";
            detailLists += "<tbody>\r\n";
            for (int i = 0; i < item.ListQuestion.Count(); i++)
            {
                // dữ liệu của khách hàng
                var data = detail.FirstOrDefault(x => x.QuestionId == item.ListQuestion[i].Id && x.TargetId == 0 && x.TargetModule == "Answer");
                //
                detailLists += "<tr class=\"text-left\"><td>" + item.ListQuestion[i].Name + "</td>";
                detailLists += "<td class=\"text-left\">" + (data == null ? "" : data.Content) + "</td><td class=\"text-left\">";
                detailLists += (data == null ? "" : data.Note) + "</td></tr>";
            }
            detailLists += "</tbody>\r\n</table>\r\n</p>";
            return detailLists;
        }
        string buildHtml_Partial2(CategoryViewModel item, List<AdviseCardDetailViewModel> detail)
        {
            string detailLists = "";
            foreach (var question in item.ListQuestion)
            {
                detailLists += "<p>" + question.Name + "\r\n";
                if (question.Type == "input")
                {
                    // dữ liệu của khách hàng
                    var data = detail.FirstOrDefault(x => x.QuestionId == question.Id && x.TargetId == 0 && x.TargetModule == "Answer");
                    //
                    detailLists += (data == null ? "" : data.Content) + "</p>\r\n";
                }
                if (question.Type == "check")
                {
                    foreach (var answer in question.DetailList)
                    {
                        // dữ liệu của khách hàng
                        var data = detail.FirstOrDefault(x => x.QuestionId == question.Id && x.TargetId == answer.Id && x.TargetModule == "Answer");
                        //
                        detailLists += "<input type=\"checkbox\" " + (data == null ? "" : "checked") + "/>" + answer.Content + "\r\n";
                    }
                    detailLists += "</p>\r\n";
                }
                if (question.Type == "all")
                {
                    foreach (var answer in question.DetailList)
                    {
                        // dữ liệu của khách hàng
                        var data = detail.FirstOrDefault(x => x.QuestionId == question.Id && x.TargetId == answer.Id && x.TargetModule == "Answer");
                        //
                        detailLists += "<input type=\"checkbox\" " + (data == null ? "" : "checked") + "/>" + answer.Content + "\r\n";
                        detailLists += "Content_" + question.Id + "_" + answer.Id + "\r\n";
                    }
                    detailLists += "</p>\r\n";
                }
            }
            return detailLists;
        }
        string buildHtml_Partial3(CategoryViewModel item, List<AdviseCardDetailViewModel> detail)
        {
            string detailLists = "";
            detailLists += "<p><table class=\"invoice-detail\">\r\n";
            detailLists += "<thead>\r\n";
            detailLists += "	<tr>\r\n";
            detailLists += "		<th></th>\r\n";
            detailLists += "		<th>Trạng thái</th>\r\n";
            detailLists += "		<th>Chi tiết</th>\r\n";
            detailLists += "		<th>Ghi chú</th>\r\n";
            detailLists += "	</tr>\r\n";
            detailLists += "</thead>\r\n";
            detailLists += "<tbody>\r\n";
            for (int i = 0; i < item.ListQuestion.Count(); i++)
            {
                detailLists += "<tr class=\"text-left\"><td>" + item.ListQuestion[i].OrderNo + "</td>";
                detailLists += "<td class=\"text-left\">" + item.ListQuestion[i].Name + "</td>";
                detailLists += "<td class=\"text-left\">";
                foreach (var answer in item.ListQuestion[i].DetailList)
                {
                    // dữ liệu của khách hàng
                    var data = detail.FirstOrDefault(x => x.QuestionId == item.ListQuestion[i].Id && x.TargetId == answer.Id && x.TargetModule == "Answer");
                    //
                    detailLists += "<input type=\"checkbox\" " + (data == null ? "" : "checked") + "/>" + answer.Content + "\r\n";
                }
                detailLists += "</td><td>";
                // dữ liệu của khách hàng
                var _data = detail.FirstOrDefault(x => x.QuestionId == item.ListQuestion[i].Id && x.TargetId == 0 && x.TargetModule == "Answer");
                //
                detailLists += (_data == null ? "" : _data.Content) + "</td></tr>";
            }
            detailLists += "</tbody>\r\n</table>\r\n</p>";
            return detailLists;
        }
        string buildHtml_Partial4(CategoryViewModel item, List<AdviseCardDetailViewModel> detail)
        {
            string detailLists = "";
            detailLists += "<p><table class=\"invoice-detail\">\r\n";
            detailLists += "<thead>\r\n";
            detailLists += "	<tr>\r\n";
            detailLists += "		<th></th>\r\n";
            detailLists += "		<th>Chi tiết</th>\r\n";
            detailLists += "		<th>Ghi chú</th>\r\n";
            detailLists += "	</tr>\r\n";
            detailLists += "</thead>\r\n";
            detailLists += "<tbody>\r\n";
            for (int i = 0; i < item.ListQuestion.Count(); i++)
            {
                detailLists += "<tr><td class=\"text-left\">" + item.ListQuestion[i].Name + "</td>";
                detailLists += "<td class=\"text-left\">" + item.ListQuestion[i].Content.Replace("\n", "<br>") + "</td>";
                detailLists += "<td class=\"text-left\">";
                foreach (var answer in item.ListQuestion[i].DetailList)
                {
                    // dữ liệu của khách hàng
                    var data = detail.FirstOrDefault(x => x.QuestionId == item.ListQuestion[i].Id && x.TargetId == answer.Id && x.TargetModule == "Answer");
                    //
                    detailLists += "<input type=\"checkbox\" " + (data == null ? "" : "checked") + "/>" + answer.Content + "\r\n";
                }
                detailLists += "</td></tr>";
            }
            detailLists += "</tbody>\r\n</table>\r\n</p>";
            return detailLists;
        }
        string buildHtml_Partial5(List<CategoryViewModel> Model, List<AdviseCardDetailViewModel> detail)
        {
            string detailLists = "";
            detailLists += "<p><table class=\"invoice-detail\">\r\n";
            detailLists += "<thead>\r\n";
            detailLists += "	<tr>\r\n";
            detailLists += "<th></th>\r\n";
            foreach (var ii in Model)
            {
                detailLists += "<th>" + ii.Name + "</th>\r\n";
            }
            detailLists += "	</tr>\r\n";
            detailLists += "</thead>\r\n";
            detailLists += "<tbody>\r\n";
            detailLists += "<tr><td>KẾT LUẬN</td>";
            foreach (var ii in Model)
            {
                foreach (var item in ii.ListQuestion.Where(x => x.Category == ii.Value && x.Name == "KẾT LUẬN"))
                {
                    // dữ liệu của khách hàng
                    var data = detail.FirstOrDefault(x => x.QuestionId == item.Id && x.TargetId == 0 && x.TargetModule == "Answer");
                    //
                    detailLists += "<td class=\"text-left\">" + (data == null ? "" : data.Content) + "</td>";
                }
            }
            detailLists += "</tr>";
            detailLists += "<tr><td>GIẢI PHÁP</td>";
            foreach (var ii in Model)
            {
                foreach (var item in ii.ListQuestion.Where(x => x.Category == ii.Value && x.Name == "GIẢI PHÁP"))
                {
                    // dữ liệu của khách hàng
                    var data = detail.FirstOrDefault(x => x.QuestionId == item.Id && x.TargetId == 0 && x.TargetModule == "Answer");
                    //
                    detailLists += "<td class=\"text-left\">" + (data == null ? "" : data.Content) + "</td>";
                }
            }
            detailLists += "</tr>";
            detailLists += "<tr><td></td>";
            foreach (var ii in Model)
            {
                detailLists += "<td class=\"text-left\">";
                foreach (var item in ii.ListQuestion.Where(x => x.Category == ii.Value && x.Name == "Sản phẩm"))
                {

                    detailLists += "<p>" + item.Name + "</p>\r\n";
                    if (item.Type == "input")
                    {
                        // dữ liệu của khách hàng
                        var data = detail.FirstOrDefault(x => x.QuestionId == item.Id && x.TargetId == 0 && x.TargetModule == "Answer");
                        //
                        detailLists += "<p>" + (data == null ? "" : data.Content) + "</p>\r\n";
                    }
                    if (item.Type == "check")
                    {
                        foreach (var answer in item.DetailList)
                        {
                            // dữ liệu của khách hàng
                            var data = detail.FirstOrDefault(x => x.QuestionId == item.Id && x.TargetId == answer.Id && x.TargetModule == "Answer");
                            //
                            detailLists += "<input type=\"checkbox\" " + (data == null ? "" : "checked") + "/>" + answer.Content + "\r\n";
                        }
                    }
                    if (item.Type == "all")
                    {
                        foreach (var answer in item.DetailList)
                        {
                            // dữ liệu của khách hàng
                            var data = detail.FirstOrDefault(x => x.QuestionId == item.Id && x.TargetId == answer.Id && x.TargetModule == "Answer");
                            //
                            detailLists += "<input type=\"checkbox\" " + (data == null ? "" : "checked") + "/>" + answer.Content + "\r\n";
                            detailLists += "<p>" + (data == null ? "" : data.Content) + "</p>\r\n";
                        }
                    }
                }
                detailLists += "</td>";
            }
            detailLists += "</tr>";
            detailLists += "</tbody>\r\n</table>\r\n</p>";
            return detailLists;
        }

        string buildHtml_Partial6(CategoryViewModel item, List<AdviseCardDetailViewModel> detail)
        {
            var data = Erp.BackOffice.Helpers.SelectListHelper.GetAllProductAndService();
            string detailLists = "";
            detailLists += "<p><table class=\"invoice-detail\">\r\n";
            detailLists += "<thead>\r\n";
            detailLists += "	<tr>\r\n";
            detailLists += "<th></th>\r\n";
            foreach (var ii in item.ListQuestion)
            {
                detailLists += "<th>" + ii.Name + "</th>\r\n";
            }
            detailLists += "	</tr>\r\n";
            detailLists += "</thead>\r\n";
            detailLists += "<tbody>\r\n";

            foreach (var ii in data.GroupBy(x => x.CategoryCode))
            {
                detailLists += "<tr><td>" + ii.Key + "</td>";
                foreach (var question in item.ListQuestion)
                {
                    detailLists += "<td>";
                    if (question.Name == "SẢN PHẨM")
                    {
                        foreach (var q in data.Where(x => x.CategoryCode == ii.Key && x.Type == "product"))
                        {
                            // dữ liệu của khách hàng
                            var _data = detail.FirstOrDefault(x => x.QuestionId == question.Id && x.TargetId == q.Id && x.TargetModule == "Product");
                            //
                            if (_data != null)
                            {
                                detailLists += "<input type=\"checkbox\" " + (_data == null ? "" : "checked") + "/>" + q.Name + "\r\n";
                            }
                        }
                    }
                    if (question.Name == "DỊCH VỤ")
                    {
                        foreach (var q in data.Where(x => x.CategoryCode == ii.Key && x.Type == "service"))
                        {
                            // dữ liệu của khách hàng
                            var _data = detail.FirstOrDefault(x => x.QuestionId == question.Id && x.TargetId == q.Id && x.TargetModule == "Product");
                            //
                            if (_data != null)
                            {
                                detailLists += "<input type=\"checkbox\" " + (_data == null ? "" : "checked") + "/>" + q.Name + "\r\n";
                            }
                        }
                    }
                    detailLists += "</td>";
                }
                detailLists += "</tr>";
            }
            detailLists += "</tr>";
            detailLists += "</tbody>\r\n</table>\r\n</p>";
            return detailLists;
        }

        #region Detail
        [AllowAnonymous]
        public ActionResult DetailApp(int? Id)
        {
            var AdviseCard = AdviseCardRepository.GetvwAdviseCardById(Id.Value);
            var _category = categoryRepository.GetAllCategories();

            var _list_question = questionRepository.GetAllQuestion().Where(x => x.IsActivated == true)
                              .Select(item1 => new QuestionViewModel
                              {
                                  Id = item1.Id,
                                  Name = item1.Name,
                                  Type = item1.Type,
                                  Category = item1.Category,
                                  OrderNo = item1.OrderNo,
                                  Content = item1.Content
                              }).OrderByDescending(m => m.OrderNo);

            var _answer = answerRepository.GetAllAnswer().Where(x => x.IsActivated == true).Select(item1 => new AnswerViewModel
            {
                Id = item1.Id,
                Content = item1.Content,
                QuestionId = item1.QuestionId,
                OrderNo = item1.OrderNo
            }).OrderByDescending(m => m.OrderNo);

            if (AdviseCard != null && AdviseCard.IsDeleted != true)
            {
                var model = new AdviseCardViewModel();
                AutoMapper.Mapper.Map(AdviseCard, model);
                model.ListAdviseType = new List<CategoryViewModel>();
                model.ListAdviseType = _category.Where(x => x.Code == AdviseCard.Type).Select(x => new CategoryViewModel
                {
                    Code = x.Code,
                    Id = x.Id,
                    Name = x.Name,
                    OrderNo = x.OrderNo,
                    Value = x.Value,
                    ParentId = x.ParentId,
                    Level = 1
                }).OrderBy(x => x.OrderNo).ToList();

                foreach (var item in model.ListAdviseType)
                {
                    item.ListQuestion = _list_question.Where(x => x.Category == item.Value).OrderBy(x => x.OrderNo).ToList();
                    foreach (var item1 in item.ListQuestion)
                    {
                        item1.DetailList = _answer.Where(x => x.QuestionId == item1.Id).OrderBy(x => x.OrderNo).ToList();
                    }

                    var aa = _category.Where(x => x.Code == item.Value).Select(x => new CategoryViewModel
                    {
                        Code = x.Code,
                        Id = x.Id,
                        Name = x.Name,
                        OrderNo = x.OrderNo,
                        Value = x.Value,
                        ParentId = x.ParentId,
                        Level = 2
                    }).OrderBy(x => x.OrderNo).ToList();

                    if (aa.Count() > 0)
                    {
                        foreach (var q in aa)
                        {
                            q.ListQuestion = _list_question.Where(x => x.Category == q.Value).OrderBy(x => x.OrderNo).ToList();
                            foreach (var p in q.ListQuestion)
                            {
                                p.DetailList = _answer.Where(x => x.QuestionId == p.Id).OrderBy(x => x.OrderNo).ToList();
                            }
                        }
                        model.ListAdviseType = model.ListAdviseType.Union(aa).ToList();
                    }
                }
                return View(model);
            }



            if (Request.UrlReferrer != null)
                return Redirect(Request.UrlReferrer.AbsoluteUri);
            return RedirectToAction("Index");
        }
        #endregion

        #region GetInfoAdviseCard
        [AllowAnonymous]
        public JsonResult GetInfoAdviseCard(int id)
        {

            var q = AdviseCardRepository.GetvwAdviseCardById(id);
            var model = new AdviseCardViewModel();
            AutoMapper.Mapper.Map(q, model);
            return Json(model, JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region CSKH - Lead - Nghi
        [HttpGet]
        public ActionResult LeadReportIndex()
        {
            var user = userRepository.GetAllUsers();
            ViewBag.user = user;

            return View();
        }
        [HttpGet]
        [AllowAnonymousAttribute]
        public JsonResult GetLeadReport(int? idBranch, string strFromDate, string strToDate)
        {
            List<string> labels = new List<string>();
            List<string> backgroundColor = new List<string>();
            List<int> data = new List<int>();
            List<decimal> dataValue = new List<decimal>();
            List<string> dataRate = new List<string>();
            var lsstatus = SqlHelper.QuerySP<StatusLeadModel>("GetAllStatus");
            var leads = SqlHelper.QuerySP<LeadReport>("LoadLeadReport", new
            {
                @fromDate = strFromDate,
                @toDate = strToDate,
                @Id = idBranch ?? 0,
            });
            var totallsstatus = leads?.Sum(x => x.TotalMoney) ?? 0;
            if (leads != null && leads.Count() > 0)
            {
                foreach (var item in lsstatus)
                {
                    var lead = leads?.Where(x => x.StatusId == item.Id);
                    var totalmoney = lead?.Sum(x => x.TotalMoney) ?? 0;
                    var count = lead?.Count() ?? 0;
                    labels.Add(item.Name);
                    backgroundColor.Add(item.ColorStatus);
                    dataValue.Add(totalmoney);
                    data.Add(count);
                    dataRate.Add((totallsstatus > 0 ? (float)totalmoney / (float)totallsstatus * 100 : 0).ToString("0.00") + "%");

                }
            }
            return Json(new { labels, backgroundColor, data, dataValue, dataRate, totallsstatus, datasum = data.Sum() }, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult WinLoseLeadReport()
        {
            var user = userRepository.GetAllUsers();
            ViewBag.user = user;

            return View();
        }
        [HttpGet]
        [AllowAnonymousAttribute]
        public JsonResult GetWinLoseLeadReport(int? idBranch, string strFromDate, string strToDate)
        {
            var leads = SqlHelper.QuerySP<WinLoseLeadReport>("GetWinLoseLeadReport", new
            {
                @fromDate = strFromDate,
                @toDate = strToDate,
                @Id = idBranch ?? 0,
            });
            return Json(leads, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult ImproveLeadReport()
        {
            var branches = branchRepository.GetAllBranch();
            var bps = userType_kdRepository.GetUserTypes();
            ViewBag.branches = branches;
            ViewBag.bps = bps;
            return View();
        }
        [HttpGet]
        [AllowAnonymousAttribute]
        public JsonResult GetImproveLeadReport(int? idBranch, string strFromDate, string strToDate, int? idsource)
        {
            List<string> labels = new List<string>();
            List<string> backgroundColor = new List<string>();
            List<float> data = new List<float>();
            List<float> dataValue = new List<float>();
            List<dynamic> extraid = new List<dynamic>();
            var leads = SqlHelper.QuerySP<ImproveLeadReport>("GetImproveLeadReport", new
            {
                @fromDate = strFromDate,
                @toDate = strToDate,
                @Id = idBranch ?? 0,
                idsource = idsource
            });
            if (leads != null && leads.Count() > 0)
            {
                foreach (var item in leads)
                {
                    labels.Add(item.BranchName + $"(SL: {item.SL})");
                    data.Add(item.TLSL);
                    extraid.Add(new { id = item.BranchId });
                }
                foreach (var item in typeof(clr).GetProperties().Skip(1))
                {
                    backgroundColor.Add(item.Name);
                    if (backgroundColor.Count == labels.Count)
                        break;
                }
            }
            return Json(new { labels, backgroundColor, data, dataValue, leads, extraid }, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        [AllowAnonymousAttribute]
        public JsonResult GetBPLeadReport(int? idBranch, string strFromDate, string strToDate, int? idsource)
        {
            List<string> labels = new List<string>();
            List<string> backgroundColor = new List<string>();
            List<float> data = new List<float>();
            List<float> dataValue = new List<float>();
            List<dynamic> extraid = new List<dynamic>();
            var leads = SqlHelper.QuerySP<ImproveLeadReport>("GetBPLeadReport", new
            {
                @fromDate = strFromDate,
                @toDate = strToDate,
                @Id = idBranch ?? 0,
                idsource = idsource
            });
            if (leads != null && leads.Count() > 0)
            {
                foreach (var item in leads)
                {
                    labels.Add(item.BranchName + $"(SL: {item.SL})");
                    data.Add(item.TLSL);
                    extraid.Add(new { id = item.BranchId });
                }
                foreach (var item in typeof(clr).GetProperties().Skip(1))
                {
                    backgroundColor.Add(item.Name);
                    if (backgroundColor.Count == labels.Count)
                        break;
                }
            }
            return Json(new { labels, backgroundColor, data, dataValue, leads, extraid }, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        [AllowAnonymousAttribute]
        public JsonResult GetUsrLeadReport(int? idBranch, string strFromDate, string strToDate, int? idsource)
        {
            List<string> labels = new List<string>();
            List<string> backgroundColor = new List<string>();
            List<float> data = new List<float>();
            List<float> dataValue = new List<float>();
            List<dynamic> extraid = new List<dynamic>();
            var leads = SqlHelper.QuerySP<ImproveLeadReport>("GetUsrLeadReport", new
            {
                @fromDate = strFromDate,
                @toDate = strToDate,
                @Id = idBranch ?? 0,
                idsource = idsource
            });
            if (leads != null && leads.Count() > 0)
            {
                foreach (var item in leads)
                {
                    labels.Add(item.BranchName + $"(SL: {item.SL})");
                    data.Add(item.SL);
                    extraid.Add(new { id = item.BranchId });
                }
                foreach (var item in typeof(clr).GetProperties().Skip(1))
                {
                    backgroundColor.Add(item.Name);
                    if (backgroundColor.Count == labels.Count)
                        break;
                }
            }
            return Json(new { labels, backgroundColor, data, dataValue, leads, extraid }, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        [AllowAnonymousAttribute]
        public JsonResult GetDtTblLeadReport(int? idBranch, string strFromDate, string strToDate, int type, int? idcha)
        {
            var leads = SqlHelper.QuerySP<DtTblLeadReport>("GetDtTblLeadReport", new
            {
                @fromDate = strFromDate,
                @toDate = strToDate,
                @Id = idBranch ?? 0,
                @type = type,
                @idcha = idcha ?? 0
            });
            return Json(leads, JsonRequestBehavior.AllowGet);
        }
        #region CusBuy
        [HttpGet]
        public ActionResult CusBuyLeadReport()
        {
            var branches = branchRepository.GetAllBranch();
            var bps = userType_kdRepository.GetUserTypes();
            ViewBag.branches = branches;
            ViewBag.bps = bps;
            return View();
        }
        [HttpGet]
        [AllowAnonymousAttribute]
        public JsonResult GetCusBuyLeadReport(int? idBranch, string strFromDate, string strToDate, int? idsource)
        {
            List<string> labels = new List<string>();
            List<string> backgroundColor = new List<string>();
            List<float> data = new List<float>();
            List<float> dataValue = new List<float>();
            List<dynamic> extraid = new List<dynamic>();
            var leads = SqlHelper.QuerySP<ImproveLeadReport>("GetCusBuyLeadReport", new
            {
                @fromDate = strFromDate,
                @toDate = strToDate,
                @Id = idBranch ?? 0,
                @idsource = idsource
            });
            if (leads != null && leads.Count() > 0)
            {
                foreach (var item in leads)
                {
                    labels.Add(item.BranchName + $"(SL: {item.SL})");
                    data.Add(item.TLSL);
                    extraid.Add(new { id = item.BranchId });
                }
                foreach (var item in typeof(clr).GetProperties().Skip(1))
                {
                    backgroundColor.Add(item.Name);
                    if (backgroundColor.Count == labels.Count)
                        break;
                }
            }
            return Json(new { labels, backgroundColor, data, dataValue, leads, extraid }, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        [AllowAnonymousAttribute]
        public JsonResult GetBPCBLeadReport(int? idBranch, string strFromDate, string strToDate, int? idsource)
        {
            List<string> labels = new List<string>();
            List<string> backgroundColor = new List<string>();
            List<float> data = new List<float>();
            List<float> dataValue = new List<float>();
            List<dynamic> extraid = new List<dynamic>();
            var leads = SqlHelper.QuerySP<ImproveLeadReport>("GetBPCBLeadReport", new
            {
                @fromDate = strFromDate,
                @toDate = strToDate,
                @Id = idBranch ?? 0,
                idsource = idsource
            });
            if (leads != null && leads.Count() > 0)
            {
                foreach (var item in leads)
                {
                    labels.Add(item.BranchName + $"(SL: {item.SL})");
                    data.Add(item.TLSL);
                    extraid.Add(new { id = item.BranchId });
                }
                foreach (var item in typeof(clr).GetProperties().Skip(1))
                {
                    backgroundColor.Add(item.Name);
                    if (backgroundColor.Count == labels.Count)
                        break;
                }
            }
            return Json(new { labels, backgroundColor, data, dataValue, leads, extraid }, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        [AllowAnonymousAttribute]
        public JsonResult GetUsrCBLeadReport(int? idBranch, string strFromDate, string strToDate, int? idsource)
        {
            List<string> labels = new List<string>();
            List<string> backgroundColor = new List<string>();
            List<float> data = new List<float>();
            List<float> dataValue = new List<float>();
            List<dynamic> extraid = new List<dynamic>();
            var leads = SqlHelper.QuerySP<ImproveLeadReport>("GetUsrCBLeadReport", new
            {
                @fromDate = strFromDate,
                @toDate = strToDate,
                @Id = idBranch ?? 0,
                idsource = idsource
            });
            if (leads != null && leads.Count() > 0)
            {
                foreach (var item in leads)
                {
                    labels.Add(item.BranchName + $"(SL: {item.SL})");
                    data.Add(item.TLSL);
                    extraid.Add(new { id = item.BranchId });
                }
                foreach (var item in typeof(clr).GetProperties().Skip(1))
                {
                    backgroundColor.Add(item.Name);
                    if (backgroundColor.Count == labels.Count)
                        break;
                }
            }
            return Json(new { labels, backgroundColor, data, dataValue, leads, extraid }, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        [AllowAnonymousAttribute]
        public JsonResult GetDtCBTblLeadReport(int? idBranch, string strFromDate, string strToDate, int type, int? idcha)
        {
            var leads = SqlHelper.QuerySP<DtCBTblLeadReport>("GetDtCBTblLeadReport", new
            {
                @fromDate = strFromDate,
                @toDate = strToDate,
                @Id = idBranch ?? 0,
                @type = type,
                @idcha = idcha ?? 0
            });
            return Json(leads, JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region TBDHA
        [HttpGet]
        public ActionResult TBDHALeadReport()
        {
            var branches = branchRepository.GetAllBranch();
            var bps = userType_kdRepository.GetUserTypes();
            ViewBag.branches = branches;
            ViewBag.bps = bps;
            return View();
        }
        [HttpGet]
        [AllowAnonymousAttribute]
        public JsonResult GetTBDHALeadReport(int? idBranch, string strFromDate, string strToDate, int? datechart, int? idsource)
        {
            List<string> backgroundColor = new List<string>();
            List<dynamic> extraid = new List<dynamic>();
            var leads = SqlHelper.QuerySP<TBDHALeadReport>("GetTBDHALeadReport", new
            {
                @fromDate = strFromDate,
                @toDate = strToDate,
                @Id = idBranch ?? 0,
                @datechart = datechart ?? 0,
                @idsource = idsource
            });
            var labels = leads.GroupBy(p => p.Date,
(key, g) => new { Date = key });
            var data = leads.GroupBy(p => new { p.BranchId, p.Name }, p => p.SL,
(key, g) => new { key, SL = g.ToList() });
            foreach (var item in data)
            {
                extraid.Add(new { id = item.key.BranchId });
            }
            foreach (var item in typeof(clr).GetProperties().Skip(1))
            {
                backgroundColor.Add(item.Name);
                if (backgroundColor.Count == data.Count())
                    break;
            }
            return Json(new { labels = labels.Select(x => x.Date), backgroundColor, data, extraid }, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        [AllowAnonymousAttribute]
        public JsonResult GetBPTBDHALeadReport(int? idBranch, string strFromDate, string strToDate, int? datechart, int? idsource)
        {
            List<string> backgroundColor = new List<string>();
            List<dynamic> extraid = new List<dynamic>();
            var leads = SqlHelper.QuerySP<TBDHALeadReport>("GetBPTBDHALeadReport", new
            {
                @fromDate = strFromDate,
                @toDate = strToDate,
                @Id = idBranch ?? 0,
                @datechart = datechart ?? 0,
                @idsource = idsource
            });
            var labels = leads.GroupBy(p => p.Date,
(key, g) => new { Date = key });
            var data = leads.GroupBy(p => new { p.BranchId, p.Name }, p => p.SL,
(key, g) => new { key, SL = g.ToList() });
            foreach (var item in data)
            {
                extraid.Add(new { id = item.key.BranchId });
            }
            foreach (var item in typeof(clr).GetProperties().Skip(1))
            {
                backgroundColor.Add(item.Name);
                if (backgroundColor.Count == data.Count())
                    break;
            }
            return Json(new { labels = labels.Select(x => x.Date), backgroundColor, data, extraid }, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        [AllowAnonymousAttribute]
        public JsonResult GetUsrTBDHALeadReport(int? idBranch, string strFromDate, string strToDate, int? datechart, int? idsource)
        {
            List<string> backgroundColor = new List<string>();
            List<dynamic> extraid = new List<dynamic>();
            var leads = SqlHelper.QuerySP<TBDHALeadReport>("GetUsrTBDHALeadReport", new
            {
                @fromDate = strFromDate,
                @toDate = strToDate,
                @Id = idBranch ?? 0,
                @datechart = datechart ?? 0,
                @idsource = idsource
            });
            var labels = leads.GroupBy(p => p.Date,
(key, g) => new { Date = key });
            var data = leads.GroupBy(p => new { p.BranchId, p.Name }, p => p.SL,
(key, g) => new { key, SL = g.ToList() });
            foreach (var item in data)
            {
                extraid.Add(new { id = item.key.BranchId });
            }
            foreach (var item in typeof(clr).GetProperties().Skip(1))
            {
                backgroundColor.Add(item.Name);
                if (backgroundColor.Count == data.Count())
                    break;
            }
            return Json(new { labels = labels.Select(x => x.Date), backgroundColor, data, extraid }, JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region Interact
        [HttpGet]
        public ActionResult InteractLeadReport()
        {
            var branches = branchRepository.GetAllBranch();
            var bps = userType_kdRepository.GetUserTypes();
            ViewBag.branches = branches;
            ViewBag.bps = bps;
            return View();
        }
        [HttpGet]
        public ActionResult TblTBDHLeadReport()
        {
            var branches = branchRepository.GetAllBranch();
            ViewBag.branches = branches;
            return View();
        }
        [HttpGet]
        public ActionResult TargetFlLeadReport()
        {
            var branches = branchRepository.GetAllBranch();
            var bps = userType_kdRepository.GetUserTypes();
            ViewBag.branches = branches;
            ViewBag.bps = bps;
            return View();
        }
        [HttpGet]
        public ActionResult TopicDescLeadReport()
        {
            var branches = branchRepository.GetAllBranch();
            var bps = userType_kdRepository.GetUserTypes();
            ViewBag.branches = branches;
            ViewBag.bps = bps;
            return View();
        }
        [HttpGet]
        [AllowAnonymousAttribute]
        public JsonResult GetInteractLeadReport(int? idBranch, string strFromDate, string strToDate, int? type, int? idsource)
        {
            List<string> backgroundColor = new List<string>();
            List<dynamic> extraid = new List<dynamic>();
            var leads = SqlHelper.QuerySP<TBDHALeadReport>("GetInteractLeadReport", new
            {
                @fromDate = strFromDate,
                @toDate = strToDate,
                @Id = idBranch ?? 0,
                type = type ?? 1,
                @idsource = idsource
            });
            var labels = leads.GroupBy(p => p.Date,
(key, g) => new { Date = key });
            var leadstbl = SqlHelper.QuerySP<TBDHALeadReport>("GetInteractLeadReport", new
            {
                @fromDate = strFromDate,
                @toDate = strToDate,
                @Id = 0,
                type = 2,
                @idsource = idsource
            });
            var tbl = leadstbl.GroupBy(p => new { p.BranchId, p.Name }, p => new { p.SL, p.TLSL, p.SLNM, p.SLNL, p.SLLM, p.DSA, p.TBDHA },
(key, g) => new { Id = key.BranchId, Name = key.Name, SL = g.Sum(x => x.SL), TLSL = g.Sum(x => x.TLSL), S = g.Sum(x => x.SL) + g.Sum(x => x.TLSL), SLNM = g.Sum(x => x.SLNM), SLNL = g.Sum(x => x.SLNL), SLLM = g.Sum(x => x.SLLM), DSA = g.Sum(x => x.DSA) });
            var tbldesc = tbl.OrderByDescending(x => x.S).Take(3);
            var hendesc = tbl.OrderByDescending(x => x.SLLM).Take(3);
            var lendesc = tbl.OrderByDescending(x => x.SLNL).Take(3);
            var muadesc = tbl.OrderByDescending(x => x.SLNM).Take(3);
            var dsdesc = tbl.OrderByDescending(x => x.DSA).Take(3);
            var dsasc = tbl.OrderBy(x => x.DSA).Take(3);
            var muaasc = tbl.OrderBy(x => x.SLNM).Take(3);
            var henasc = tbl.OrderBy(x => x.SLLM).Take(3);
            var lenasc = tbl.OrderBy(x => x.SLNL).Take(3);
            var tblasc = tbl.OrderBy(x => x.S).Take(3);
            var data = leads.GroupBy(p => new { p.BranchId, p.Name }, p => new { p.SL, p.TLSL, p.SLNM, p.SLNL, p.SLLM, p.DSA, p.TBDHA },
(key, g) => new { key, SL = g.ToList() });
            foreach (var item in data)
            {
                extraid.Add(new { id = item.key.BranchId });
            }
            foreach (var item in typeof(clr).GetProperties().Skip(1))
            {
                backgroundColor.Add(item.Name);
                if (backgroundColor.Count == data.Count())
                    break;
            }
            return Json(new { lenasc, lendesc, muaasc, muadesc, dsasc, dsdesc, hendesc, henasc, labels = labels.Select(x => x.Date), backgroundColor, data, extraid, tbldesc, tblasc }, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        [AllowAnonymousAttribute]
        public JsonResult GetTblTBDHLeadReport(int? idBranch, string strFromDate, string strToDate, int? type, int? idsource)
        {
            List<string> backgroundColor = new List<string>();
            List<dynamic> extraid = new List<dynamic>();
            var leads = SqlHelper.QuerySP<TBDHALeadReport>("GetTblTBDHLeadReport", new
            {
                @fromDate = strFromDate,
                @toDate = strToDate,
                @Id = idBranch ?? 0,
                @idsource = idsource
            });

            return Json(leads, JsonRequestBehavior.AllowGet);
        }
        
        [HttpPost]
        [AllowAnonymousAttribute]
        public JsonResult UpdatePin(string leadid,string id,int type)
        {
            int chk;
            int chkleadid;
            if(int.TryParse(id, out chk) && int.TryParse(leadid, out chkleadid) )
            {
                var leads = SqlHelper.ExecuteSQL("Update crm_leadlogs set isPing=@isPing WHERE Id=@Id", new { isPing = type, @Id= chk });
                var context = Microsoft.AspNet.SignalR.GlobalHost.ConnectionManager.GetHubContext<ErpHub>();
                context.Clients.All.f5LeadLogs(chkleadid);
                context.Clients.All.f5PinCmt(chkleadid);
                return leads>0?Json(1): Json(0);
       
            }
            else
            {
                return Json(0);

            }

        }

        [HttpGet]
        [AllowAnonymousAttribute]
        public JsonResult GetTargetFlLeadReport(int? idBranch, string strFromDate, string strToDate, int? type, int? idsource, int? branchid)
        {
            List<string> backgroundColor = new List<string>();
            List<dynamic> extraid = new List<dynamic>();
            var leads = SqlHelper.QuerySP<TBDHALeadReport>("GetTargetFlLeadReport", new
            {
                @fromDate = strFromDate,
                @toDate = strToDate,
                @Id = idBranch ?? 0,
                type = type ?? 1,
                idsource = idsource
            });
            var ATarget = SqlHelper.QueryMultipleSP<TBDHALeadReport, TBDHALeadReport>("GetATargetLeadReport", new
            {
                @fromDate = strFromDate,
                @toDate = strToDate,
                @Id = idBranch ?? 0,
                type = type ?? 1,
                idsource = idsource
            });
            var Starget = SqlHelper.QuerySP<StargetLeadReport>("GetTargetSmLeadReport", new
            {
                @fromDate = strFromDate,
                @toDate = strToDate,
                @branchid = branchid
            });
            foreach (var item in leads)
            {
                extraid.Add(new { id = item.BranchId });
                var tgtsl = Starget.Where(x => x.TypeTarget == 1 && x.UserId == item.BranchId).FirstOrDefault();
                var tgttlsl = Starget.Where(x => x.TypeTarget == 2 && x.UserId == item.BranchId).FirstOrDefault();
                var tgtsllm = Starget.Where(x => x.TypeTarget == 3 && x.UserId == item.BranchId).FirstOrDefault();
                var tgtslnl = Starget.Where(x => x.TypeTarget == 4 && x.UserId == item.BranchId).FirstOrDefault();
                var tgtslnm = Starget.Where(x => x.TypeTarget == 5 && x.UserId == item.BranchId).FirstOrDefault();
                var tgtdsa = Starget.Where(x => x.TypeTarget == 5 && x.UserId == item.BranchId).FirstOrDefault();
                item.TargetSL = tgtsl?.Starget ?? 0;
                item.TargetTLSL = tgttlsl?.Starget ?? 0;
                item.TargetSLLM = tgtsllm?.Starget ?? 0;
                item.TargetSLNL = tgtslnl?.Starget ?? 0;
                item.TargetSLNM = tgtslnm?.Starget ?? 0;
                item.TargetDSA = tgtdsa?.Starget ?? 0;
            }
            foreach (var item in typeof(clr).GetProperties().Skip(1))
            {
                backgroundColor.Add(item.Name);
                if (backgroundColor.Count == leads.Count())
                    break;
            }
            return Json(new { labels = leads.Select(x => x.Name), backgroundColor, leads, extraid, ATarget }, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        [AllowAnonymousAttribute]
        public JsonResult GetTopicDescLeadReport(int? idBranch, string strFromDate, string strToDate, int? idsource)
        {
            var leads = SqlHelper.QuerySP<dynamic>("GetTopicDescLeadReport", new
            {
                @fromDate = strFromDate,
                @toDate = strToDate,
                @Id = idBranch ?? 0,
                idsource = idsource ?? 2
            });
            return Json(leads, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [AllowAnonymousAttribute]
        public JsonResult GetUserTypekdByBranch(int? idBranch)
        {
            var UserTypekd = userType_kdRepository.GetUserTypes().Where(x => x.BranchId == idBranch || idBranch == null || idBranch == 0);
            return Json(UserTypekd, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        [AllowAnonymousAttribute]
        public JsonResult GetUserByUserTypekd(int? idBranch)
        {
            var user = userRepository.GetAllUsers().Where(x => x.UserType_kd_id == idBranch || idBranch == null || idBranch == 0);
            return Json(user, JsonRequestBehavior.AllowGet);
        }
        #endregion
        [HttpGet]
        public ActionResult TrendingLeadReport()
        {
            var user = userRepository.GetAllUsers();
            ViewBag.user = user;

            return View();
        }
        [HttpGet]
        [AllowAnonymousAttribute]
        public JsonResult GetTrendingLeadReport(int? idBranch, string strFromDate, string strToDate)
        {
            var leads = SqlHelper.QuerySP<TrendingLeadReport>("GetTrendingLeadReport", new
            {
                @fromDate = strFromDate,
                @toDate = strToDate,
                @Id = idBranch ?? 0,
            });
            return Json(leads, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult ReasonLeadReport()
        {
            var user = userRepository.GetAllUsers();
            ViewBag.user = user;

            return View();
        }
        [HttpGet]
        [AllowAnonymousAttribute]
        public JsonResult GetReasonLeadReport(int? idBranch, string strFromDate, string strToDate)
        {
            List<string> labels = new List<string>();
            List<string> backgroundColor = new List<string>();
            List<float> data = new List<float>();
            List<float> dataValue = new List<float>();
            var leads = SqlHelper.QuerySP<ReasonLeadReport>("GetReasonLeadReport", new
            {
                @fromDate = strFromDate,
                @toDate = strToDate,
                @Id = idBranch ?? 0,
            });
            if (leads != null && leads.Count() > 0)
            {
                foreach (var item in leads)
                {
                    labels.Add(item.TheEnd);
                    data.Add(item.TLSL);
                    dataValue.Add(item.TLDS);
                }
                foreach (var item in typeof(clr).GetProperties().Skip(1))
                {
                    backgroundColor.Add(item.Name);
                    if (backgroundColor.Count == labels.Count - 1)
                        break;
                }
            }
            return Json(new { labels, backgroundColor, data, dataValue, leads }, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult AgeLeadReport()
        {
            var user = userRepository.GetAllUsers();
            ViewBag.user = user;

            return View();
        }
        [HttpGet]
        [AllowAnonymousAttribute]
        public JsonResult GetAgeLeadReport(int? idBranch, string strFromDate, string strToDate)
        {
            List<string> labels = new List<string>();
            List<float> data = new List<float>();
            List<float> dataValue = new List<float>();
            var leads = SqlHelper.QuerySP<AgeLeadReport>("GetAgeLeadReport", new
            {
                @fromDate = strFromDate,
                @toDate = strToDate,
                @Id = idBranch ?? 0,
            });
            if (leads != null && leads.Count() > 0)
            {
                foreach (var item in leads)
                {
                    labels.Add(item.FullName);
                    data.Add(item.AgeWin);
                    dataValue.Add(item.AgeLose);
                }
            }
            return Json(new { labels, data, dataValue, leads }, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult THKHLeadReport()
        {
            var user = userRepository.GetAllUsers();
            ViewBag.user = user;

            return View();
        }
        [HttpGet]
        [AllowAnonymousAttribute]
        public JsonResult GetTHKHLeadReport(int? idBranch, string strFromDate, string strToDate)
        {
            List<string> labels = new List<string>();
            List<int> data = new List<int>();
            List<decimal> dataValue = new List<decimal>();
            List<string> dataRate = new List<string>();
            List<string> dataRateSL = new List<string>();
            List<string> backgroundColor = new List<string>();
            var lsstatus = SqlHelper.QuerySP<StatusLeadModel>("GetAllStatus");
            var leads = SqlHelper.QuerySP<LeadReport>("LoadLeadReport", new
            {
                @fromDate = strFromDate,
                @toDate = strToDate,
                @Id = idBranch ?? 0,
            });
            var totallsstatus = leads?.Sum(x => x.TotalMoney) ?? 0;
            if (leads != null && leads.Count() > 0)
            {
                foreach (var item in lsstatus.Where(x => x.EndStatus != false))
                {
                    var lead = leads?.Where(x => x.StatusId == item.Id);
                    var totalmoney = lead?.Sum(x => x.TotalMoney) ?? 0;
                    var count = lead?.Count() ?? 0;
                    labels.Add(item.Name);
                    dataValue.Add(totalmoney);
                    data.Add(count);
                    dataRate.Add((totallsstatus > 0 ? (float)totalmoney / (float)totallsstatus * 100 : 0).ToString("0.00") + "%");
                    dataRateSL.Add(((float)count / (float)leads.Count() * 100).ToString("0.00") + "%");
                    backgroundColor.Add(item.ColorStatus);
                }

            }
            return Json(new { labels, data, dataValue, dataRate, totallsstatus, datasum = leads.Count(), dataRateSL, backgroundColor }, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult TargetLeadReport()
        {
            var user = userRepository.GetAllUsers();
            ViewBag.user = user;

            return View();
        }
        [HttpGet]
        [AllowAnonymousAttribute]
        public JsonResult GetTargetLeadReport(int? idBranch, int nam, int thang, string CountForBrand)
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
            var leads = SqlHelper.QuerySP<TargetLeadReport>("GetTargetLeadReport", new
            {
                nam = nam,
                THANG = thang,
                BranchId = intBrandID,
                @Id = idBranch ?? 0,
                CountForBrand = CountForBrand
            });
            return Json(leads, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult TypeDataLeadReport()
        {
            var user = userRepository.GetAllUsers();
            ViewBag.user = user;

            return View();
        }
        [HttpGet]
        [AllowAnonymousAttribute]
        public JsonResult GetTypeDataLeadReport(int? idBranch, string strFromDate, string strToDate)
        {
            List<string> labels = new List<string>();
            List<string> backgroundColor = new List<string>();
            List<float> data = new List<float>();
            List<float> dataValue = new List<float>();
            var leads = SqlHelper.QuerySP<ReasonLeadReport>("GetTypeDataLeadReport", new
            {
                @fromDate = strFromDate,
                @toDate = strToDate,
                @Id = idBranch ?? 0,
            });
            foreach (var item in leads)
            {
                labels.Add(item.TheEnd);
                data.Add(item.TLSL);
                dataValue.Add(item.TLDS);
            }
            foreach (var item in typeof(clr).GetProperties().Skip(1))
            {
                backgroundColor.Add(item.Name);
                if (backgroundColor.Count == labels.Count - 1)
                    break;
            }
            return Json(new { labels, backgroundColor, data, dataValue, leads }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult LeadIndex()
        {
            return View();
        }
        public ActionResult LeadCusIndex()
        {
            ViewBag.ldcusview = -1;
            return View("LeadIndex");
        }
        public ActionResult LeadRuleIndex()
        {

            return View();
        }
        [AllowAnonymous]
        public ActionResult LeadPartialView(IEnumerable<LeadModel> model, IPagedList pagelist, string columnsort, int? sortdir, int? ispartial = null)
        {
            model = model != null ? model : new List<LeadModel>();
            var section = SqlHelper.QueryMultipleSP<LeadSectionModel, LeadSection_FieldModel>("AddNewModal");
            var user = userRepository;
            var lsstatus = SqlHelper.QuerySP<StatusLeadModel>("GetAllStatus", new { @ispartial = ispartial });
            var category = categoryRepository;
            if (columnsort != null)
            {
                ViewBag.ColumnSort = columnsort;
                ViewBag.SortDir = sortdir;
            }
            ViewBag.pagelist = pagelist;
            ViewBag.category = category;
            ViewData["section"] = section;
            ViewBag.user = user;
            ViewBag.lsstatus = lsstatus;
            ViewBag.ispartial = ispartial;
            return PartialView(model);
        }
        [HttpGet]
        public PartialViewResult AddNewModal(int type, int ldcusview = 0, int Id = 0, string Email = null, string Address = null, string Phone = null, string LastName = null) // =1 add, =0 search
        {
            var category = categoryRepository;
            var user = userRepository;
            var section = SqlHelper.QueryMultipleSP<LeadSectionModel, LeadSection_FieldModel>("AddNewModal", new { @isPartial = (Id > 0 || ldcusview == -1) ? 1 : 0 });
            var lsstatus = SqlHelper.QuerySP<StatusLeadModel>("GetAllStatus", new { @isPartial = (Id > 0 || ldcusview == -1) ? 1 : 0 });
            ViewBag.lsstatus = lsstatus;
            ViewBag.category = category;
            ViewBag.ldcusview = ldcusview;
            ViewBag.CustomerInfo = new Customer { Id = Id, Email = Email, Address = Address, Phone = Phone, LastName = LastName };
            ViewBag.user = user;
            SectionCusModel sectionCusModel = new SectionCusModel();
            sectionCusModel.Tuple = section;
            return type == 1 ? PartialView("AddLeadModal", sectionCusModel) : PartialView("SearchPanelLead", section);
        }
        [AllowAnonymous]
        public PartialViewResult LeadRuleModal(int? typeLead)
        {
            var TyleRule = SqlHelper.QuerySP<TyleRuleModel>("GetTyleRule");
            ViewBag.TyleRule = TyleRule;
            var section = SqlHelper.QueryMultipleSP<LeadSectionModel, LeadSection_FieldModel>("AddNewModal");
            ViewBag.section = section;
            var lsstatus = SqlHelper.QuerySP<StatusLeadModel>("GetAllStatus", new
            {
                @isPartial = typeLead == 1 ? 0 : 1
            });
            ViewBag.lsstatus = lsstatus;
            ViewBag.FromMail = ConfigurationManager.AppSettings["EmailSender"].ToString();
            return PartialView();
        }
        [AllowAnonymous]
        public PartialViewResult LeadRulePartial(int? typeLead)
        {
            var Rule = SqlHelper.QuerySP<RuleModel>("GetAllRule", new
            {
                @isPartial = typeLead == 1 ? 0 : 1
            });
            ViewBag.Rule = Rule;
            var lsstatus = SqlHelper.QuerySP<StatusLeadModel>("GetAllStatus", new
            {
                @isPartial = typeLead == 1 ? 0 : 1
            });
            ViewBag.lsstatus = lsstatus;
            var ruledt = SqlHelper.QuerySP<RuleDetailModel>("CheckRulesLead");
            ViewBag.ruledt = ruledt;
            var section = SqlHelper.QueryMultipleSP<LeadSectionModel, LeadSection_FieldModel>("AddNewModal");
            ViewBag.section = section;
            return PartialView();
        }
        [AllowAnonymous]
        public JsonResult GetTempeleByType(int type)
        {
            if (type == 1)
            {
                return Json(SqlHelper.QuerySP<Sale_GetList_TemplateSMS>("spCrm_Template_SelectTemplateSMS"), JsonRequestBehavior.AllowGet);

            }
            else if (type == 2)
            {
                return Json(SqlHelper.QuerySP<Sale_GetList_TemplateSMS>("spCrm_Template_SelectTemplateZNSZalo", new
                {
                    @pTypeLead = 1
                }), JsonRequestBehavior.AllowGet);

            }
            else
            {
                return Json(SqlHelper.QuerySP<AdviseCardEmailTemplateViewModel>("spCrm_Template_SelectAllEmail"), JsonRequestBehavior.AllowGet);
            }

        }
        [AllowAnonymous]
        public JsonResult GetListByCode(string code)
        {

            return Json(categoryRepository.GetListCategoryByCode(code), JsonRequestBehavior.AllowGet);

        }
        [AllowAnonymous]
        public JsonResult GetReceptionStaffId()
        {

            return Json(userRepository.GetAllUsers(), JsonRequestBehavior.AllowGet);

        }
        [AllowAnonymous]
        public JsonResult f5RuleDetail(int id)
        {
            var ruledt = SqlHelper.QuerySP<RuleDetailModel>("CheckRulesLead");
            var f5RuleDetail = ruledt.Where(x => x.RuleId == id);
            var tuple = SqlHelper.QueryMultipleSP<LeadSectionModel, LeadSection_FieldModel>("AddNewModal");
            foreach (var item in f5RuleDetail)
            {
                LeadSection_FieldModel model = tuple.Item2.Where(x => x.FieldName == item.FieldName).FirstOrDefault();
                string LabelName = model != null ? model.NameLabel : "";
                if (LabelName != "")
                {
                    item.LabelName = LabelName;

                }
                else
                {
                    var properties = typeof(LeadModel).GetProperties();
                    var prop = properties.Where(x => x.Name == item.FieldName).FirstOrDefault();
                    var display = prop != null ? prop.GetCustomAttributes(typeof(DisplayNameAttribute), true).FirstOrDefault() as DisplayNameAttribute : new DisplayNameAttribute();
                    string displayname = display != null ? display.DisplayName : "";
                    item.LabelName = displayname;
                }
            }
            return Json(f5RuleDetail, JsonRequestBehavior.AllowGet);
        }
        [AllowAnonymous]
        [HttpPost]
        [ValidateInput(false)]
        public JsonResult SaveLeadRule(RuleModel ruleModel)
        {
            int id = 0;

            try
            {
                id = SqlHelper.QuerySP<int>("SaveLeadRule", new
                {
                    @TypeRuleId = ruleModel.TypeRuleId,
                    @ContentRule = ruleModel.ContentRule,
                    @TileEmail = ruleModel.TileEmail,
                    @ContentEmail = ruleModel.ContentEmail,
                    @Note = ruleModel.Note,
                    @StatusFrom = ruleModel.StatusFrom,
                    @StatusTo = ruleModel.StatusTo,
                    @FieldName = ruleModel.ruleDetails.FieldName,
                    @Logic = ruleModel.ruleDetails.Logic,
                    @AndOr = ruleModel.ruleDetails.AndOr,
                    @ContentRuleDetail = ruleModel.ruleDetails.ContentRule,
                    @UserId = WebSecurity.CurrentUserId,
                    @Id = ruleModel.Id,
                    @IdDetail = ruleModel.ruleDetails.Id,
                    @TypeLead = ruleModel.TypeLead
                }).FirstOrDefault();


                return Json(ruleModel.Id > 0 ? ruleModel.Id : id);
            }
            catch
            {
                return Json(ruleModel.Id > 0 ? ruleModel.Id : id);

            }

        }
        [AllowAnonymous]
        [HttpPost]
        public JsonResult deleteRuleDetail(int Id)
        {
            int id = 0;

            id = SqlHelper.QuerySP<int>("deleteRuleDetail", new
            {
                @Id = Id
            }).FirstOrDefault();


            return Json(id);
        }
        [AllowAnonymous]
        [HttpPost]
        public JsonResult deleteLeadRule(int Id)
        {
            try
            {
                int id = 0;
                id = SqlHelper.QuerySP<int>("deleteRuleDetailByRuleId", new
                {
                    @RuleId = Id
                }).FirstOrDefault();
                id = SqlHelper.QuerySP<int>("deleteRule", new
                {
                    @Id = Id
                }).FirstOrDefault();

                return Json(id);
            }
            catch (Exception ex)
            {
                return Json(0);
            }

        }
        public class UpdateStatusFromModel
        {
            public int StatusId { get; set; }
            public int RuleId { get; set; }
        }
        [AllowAnonymous]
        [HttpPost]
        public JsonResult updateStatusFrom(UpdateStatusFromModel[] updates)
        {
            var Rule = SqlHelper.QuerySP<RuleModel>("GetAllRule");
            var chk = new UpdateStatusFromModel();
            if (updates != null)
            {
                foreach (var item in updates)
                {
                    if (Rule.Where(x => x.StatusFrom == item.StatusId && x.Id == item.RuleId).Count() <= 0)
                    {
                        chk = item;
                    }

                }
            }

            var kq = SqlHelper.ExecuteSP("updateStatusFrom", new { @Id = chk.RuleId, @StatusFrom = chk.StatusId, @UserId = WebSecurity.CurrentUserId });

            return Json(kq);
        }
        [AllowAnonymous]
        [HttpPost]
        public JsonResult SaveLeadLogSMS(int id, int? val, string name)
        {
            try
            {
                var Rule = SqlHelper.QuerySP<RuleModel>("GetAllRule");
                var checkIsLetan = SqlHelper.QuerySP<int>("checkUserIsLetan", new
                {
                    @pId = WebSecurity.CurrentUserId
                }).ToList();
                var leads = SqlHelper.QuerySP<LeadModel>("LoadLeadIndex", new
                {
                    @pId = WebSecurity.CurrentUserId,
                    @pIsLeTan = (checkIsLetan.Count > 0) ? 1 : 0
                });
                var lead = leads.Where(x => x.Id == id).FirstOrDefault();
                lead = lead != null ? lead : new LeadModel();
                var rulesms = Rule.Where(x => x.StatusFrom == lead.StatusId);
                var TyleRule = SqlHelper.QuerySP<TyleRuleModel>("GetTyleRule");
                var rulessms = rulesms.Where(x => x.TypeRuleId == TyleRule.Where(y => y.TypeRule == "SENDSMS").FirstOrDefault()?.Id);
                //var ruleszl = rulesms.Where(x => x.TypeRuleId == TyleRule.Where(y => y.TypeRule == "SENDZALO").FirstOrDefault()?.Id);
                if (rulessms != null)
                {
                    rulesms = ReplaceCtSMS(rulesms, id, true);
                    typeof(LeadModel).GetProperty(name).SetValue(lead, val.ToString());
                    InsertLeadLogsSMS(id, rulessms, "PREVIEWSMS", 1, lead);
                }
                //if (ruleszl != null)
                //{
                //    InsertLeadLogsSMS(id, ruleszl, "PREVIEWZL", 1, lead);
                //}
                return Json(1);
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }

        }
        public RuleModel InsertLeadLogsSMS(int Id, IEnumerable<RuleModel> rule, string action, int chk, LeadModel leadModel)
        {
            var ruledt = SqlHelper.QuerySP<RuleDetailModel>("CheckRulesLead");
            foreach (var rulee in rule)
            {
                var chkdk = ruledt.Where(x => x.RuleId == rulee.Id);
                bool? result = null;
                List<bool> orr = new List<bool>();
                foreach (var item in chkdk)
                {
                    string objchk = typeof(LeadModel).GetProperty(item.FieldName).GetValue(leadModel) != null ? typeof(LeadModel).GetProperty(item.FieldName).GetValue(leadModel).ToString() : "";
                    //(new OperatorRule(item.Logic, objchk)) + item.ContentRule;
                    if (item.AndOr == null || item.AndOr == 0)
                    {
                        orr.Add((new OperatorRule(item.Logic, objchk)) + item.ContentRule);
                    }
                    else
                    {
                        bool? orb = null;
                        orr.ForEach(x =>
                        {
                            orb = orb.HasValue ? (bool)orb || x : x;
                        });
                        result = result.HasValue ? orb != null ? ((bool)result || (bool)orb) && (new OperatorRule(item.Logic, objchk)) + item.ContentRule : (bool)result && (new OperatorRule(item.Logic, objchk)) + item.ContentRule : (bool)orb && (new OperatorRule(item.Logic, objchk)) + item.ContentRule;
                        orr.Clear();
                    }
                }
                if (chkdk.Count() == 1)
                {
                    result = orr.FirstOrDefault();
                }
                if (result == true && chk == 1)
                {
                    int a = SqlHelper.ExecuteSP("InsertLeadLogs", new
                    {
                        @LeadId = Id
              ,
                        @Action = action
              ,
                        @Logs = rulee.ContentRule
              ,
                        @isImportant = 0
              ,
                        @IdAction = -1
              ,
                        @UserId = WebSecurity.CurrentUserId,
                        @TypeLead = 1
                    });
                    var context = Microsoft.AspNet.SignalR.GlobalHost.ConnectionManager.GetHubContext<ErpHub>();
                    context.Clients.All.f5LeadLogs(Id);
                    return rulee;
                }
                else if (result == true)
                {
                    return rulee;
                }
                else
                {
                    return new RuleModel();
                }
            }
            return new RuleModel();
        }
        public object HandleLogicRule(IEnumerable<RuleModel> rule, LeadModel leadModel)
        {
            var ruledt = SqlHelper.QuerySP<RuleDetailModel>("CheckRulesLead");
            foreach (var rulee in rule)
            {
                var chkdk = ruledt.Where(x => x.RuleId == rulee.Id);
                bool? result = null;
                List<bool> orr = new List<bool>();
                foreach (var item in chkdk)
                {
                    string objchk = typeof(LeadModel).GetProperty(item.FieldName).GetValue(leadModel) != null ? typeof(LeadModel).GetProperty(item.FieldName).GetValue(leadModel).ToString() : "";
                    //(new OperatorRule(item.Logic, objchk)) + item.ContentRule;
                    if (item.AndOr == null || item.AndOr == 0)
                    {
                        orr.Add((new OperatorRule(item.Logic, objchk)) + item.ContentRule);
                    }
                    else
                    {
                        bool? orb = null;
                        orr.ForEach(x =>
                        {
                            orb = orb.HasValue ? (bool)orb || x : x;
                        });
                        result = result.HasValue ? orb != null ? ((bool)result || (bool)orb) && (new OperatorRule(item.Logic, objchk)) + item.ContentRule : (bool)result && (new OperatorRule(item.Logic, objchk)) + item.ContentRule : (bool)orb && (new OperatorRule(item.Logic, objchk)) + item.ContentRule;
                        orr.Clear();
                    }
                }
                if (result == null && orr.Count() > 0)
                {
                    bool? orb = null;
                    orr.ForEach(x =>
                    {
                        orb = orb.HasValue ? (bool)orb || x : x;
                    });
                    result = (bool)orb;
                    orr.Clear();
                }
                var lsstatus = SqlHelper.QuerySP<StatusLeadModel>("GetAllStatus");
                var statusdt = lsstatus.Where(x => x.Id == rulee.StatusTo).FirstOrDefault();
                if (result == true && statusdt != null && (statusdt.EndStatus == true || statusdt.EndStatus == false))
                {
                    return new { statusdt.Id, endstatus = statusdt.EndStatus };
                }
                if (result == true)
                {
                    SqlHelper.ExecuteSP("InsertLeadLogs", new
                    {
                        @LeadId = leadModel.Id
    ,
                        @Action = "CHANGESTATUS"
    ,
                        @Logs = rulee.StatusFrom.ToString() + ',' + rulee.StatusTo
    ,
                        @isImportant = 0
    ,
                        @IdAction = rulee.Id
    ,
                        @UserId = WebSecurity.CurrentUserId,
                        @TypeLead = 1
                    });
                    var context = Microsoft.AspNet.SignalR.GlobalHost.ConnectionManager.GetHubContext<ErpHub>();
                    context.Clients.All.f5LeadLogs(leadModel.Id);
                    leadModel.StatusId = rulee.StatusTo;
                    return 0;
                }
            }
            return -1;

        }

        public class OperatorRule
        {
            private readonly string Logic;
            private readonly string Value;
            public OperatorRule(string _Logic, string _Value)
            {
                Logic = _Logic;
                Value = _Value;
            }
            public static bool operator +(OperatorRule left, string right)
            {
                bool result = false;
                switch (left.Logic)
                {
                    case "=": result = left.Value == right; break;
                    case "!=": result = left.Value != right; break;
                    case ">": result = decimal.Parse(left.Value) > decimal.Parse(right); break;
                    case "<": result = decimal.Parse(left.Value) < decimal.Parse(right); break;
                    case "<=": result = decimal.Parse(left.Value) <= decimal.Parse(right); break;
                    case ">=": result = decimal.Parse(left.Value) >= decimal.Parse(right); break;
                    case "like": result = left.Value.Contains(right); break;
                    default:
                        break;
                }
                return result;
            }
        }
        [AllowAnonymousAttribute]
        [HttpGetAttribute]
        public JsonResult GetCusByPhone(string phone)
        {
            try
            {
                var codekh = SqlHelper.QuerySP<Customer>("GetCusByPhone", new { @pPhone = phone }).FirstOrDefault();
                if (codekh != null)
                    return Json(new { success = true, codekh = codekh.Code }, JsonRequestBehavior.AllowGet);
                else
                    return Json(new { success = false, codekh = "" }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(new { success = false, codekh = "" }, JsonRequestBehavior.AllowGet);

            }

        }
        [HttpGet]
        public PartialViewResult DetailLeadModal(int Id, int? isPartial, int? CusId) //isPartial=1 là partial cus, =2 là xem lead, =-1 la tab co hoi,=0 la lead
        {
            var category = categoryRepository;
            var user = userRepository;
            var section = SqlHelper.QueryMultipleSP<LeadSectionModel, LeadSection_FieldModel>("AddNewModal", new { isPartial = isPartial });
            List<LeadSection_FieldModel> lsSection = new List<LeadSection_FieldModel>();
            lsSection.AddRange(section.Item2);
            var properties = typeof(LeadModel).GetProperties();


            ViewBag.category = category;
            ViewBag.user = user;
            var leadc = SqlHelper.QueryMultipleSP<LeadModel, Customer>("LoadLeadIndexById", new
            {
                @pId = Id
            });
            var lead = leadc.Item1.FirstOrDefault();
            if (lead != null)
            {
                foreach (var item in properties)
                {
                    if (lsSection.Any(x => x.FieldName == item.Name))
                    {
                        var value = item.GetValue(lead);

                        if (value != null)
                        {
                            string edit = value.ToString();

                            foreach (var s in lsSection)
                            {
                                if (s.FieldName == item.Name && (s.TypeField == "Date" || s.TypeField == "Datetime"))
                                {
                                    if (s.TypeField.IndexOf("Datetime", StringComparison.OrdinalIgnoreCase) >= 0)
                                    {
                                        DateTime dateTimeValue;
                                        if (DateTime.TryParse(edit, out dateTimeValue))
                                        {
                                            edit = dateTimeValue.ToString("yyyy-MM-ddTHH:mm");
                                        }
                                    }

                                    if (s.TypeField == "Date")
                                    {
                                        DateTime dateValue;
                                        if (DateTime.TryParse(edit, out dateValue))
                                        {
                                            edit = dateValue.ToString("yyyy-MM-dd");
                                        }
                                    }
                                }
                            }

                            item.SetValue(lead, edit);
                        }
                    }
                }
            }
            ViewBag.lead = lead != null ? lead : new LeadModel();
            //Check Email config
            string port = WebConfigurationManager.AppSettings["Port"];
            string ssl = WebConfigurationManager.AppSettings["SSL"];
            string smtp = WebConfigurationManager.AppSettings["SMTP"];
            string smtpUserName = WebConfigurationManager.AppSettings["SMTPUserName"];
            string smtpPassword = WebConfigurationManager.AppSettings["SMTPPassword"];
            if (port == null || ssl == null || smtp == null || smtpUserName == null || smtpPassword == null)
            {
                string errorMessageEmail = "Bạn chưa cài đặt thông số gửi mail, vui lòng cài đặt!";
                ViewBag.errorMessageEmail = errorMessageEmail;
            }
            //Check Api Phone Call Config
            string ServiceName = ConfigurationManager.AppSettings["PhoneCallAPI:ServiceName"];
            string AuthUser = ConfigurationManager.AppSettings["PhoneCallAPI:AuthUser"];
            string AuthKey = ConfigurationManager.AppSettings["PhoneCallAPI:AuthKey"];
            string Prefix = ConfigurationManager.AppSettings["PhoneCallAPI:Prefix"];
            string Url = ConfigurationManager.AppSettings["PhoneCallAPI:Url"];

            if (ServiceName == null || AuthUser == null || AuthKey == null || Prefix == null || Url == null)
            {
                string errorMessagePhoneCall = "Bạn chưa cài đặt thông số để gọi điện thoại, vui lòng cài đặt!";
                ViewBag.errorMessagePhoneCall = errorMessagePhoneCall;
            }
            //Check Api SMS Call Config
            string requestlink = ConfigurationManager.AppSettings["SendSMS:Link"];
            string secretKey = ConfigurationManager.AppSettings["SendSMS:SecretKey"];
            string apiKey = ConfigurationManager.AppSettings["SendSMS:ApiKey"];
            string brandName = ConfigurationManager.AppSettings["SendSMS:Brandname"];

            if (apiKey == null || secretKey == null || brandName == null || requestlink == null)
            {
                string errorMessageSendSMS = "NotOK";
                ViewBag.errorMessageSendSMS = errorMessageSendSMS;
            }
            else
            {
                ViewBag.errorMessageSendSMS = "";
            }
            var DauSoCloudFone = SqlHelper.QuerySP<string>("spSystem_User_SelectDauSoCloudFoneById", new
            {
                pId = WebSecurity.CurrentUserId
            }).FirstOrDefault();
            ViewBag.DauSoCloudFone = DauSoCloudFone;
            //Tìm người chịu trách nhiệm của Lead
            int personInChargeID = lead != null ? (lead.AssignedUserId != null ? lead.AssignedUserId.Value : lead.CreatedUserId.Value) : 13467;
            var personInCharge = SqlHelper.QuerySP<User>("spSale_AdviseCard_GetUser_ById", new
            {
                pId = personInChargeID
            }).FirstOrDefault();
            ViewBag.personInCharge = personInCharge;
            ViewBag.isPartial = isPartial;
            ViewBag.CusId = CusId ?? leadc.Item2.FirstOrDefault().Id;
            ViewBag.CodeKH = leadc.Item2.FirstOrDefault()?.Code;
            SectionCusModel sectionCusModel = new SectionCusModel();
            sectionCusModel.Tuple = section;
            return PartialView("AddNewModal", sectionCusModel);
        }
        [HttpPost]
        public PartialViewResult UpdateStatusLead(int Id, int status, int? isPartial = 0) //-2 chưa mua, -3 khách mua, -1 get all
        {
            LeadModel leadModel = new LeadModel();
            var lsstatus = SqlHelper.QuerySP<StatusLeadModel>("GetAllStatus", new { @isPartial = isPartial });
            var omst = lsstatus.Where(x => x.EndStatus == false).FirstOrDefault();
            var kmst = lsstatus.Where(x => x.EndStatus == true).FirstOrDefault();
            var checkIsLetan = SqlHelper.QuerySP<int>("checkUserIsLetan", new
            {
                @pId = WebSecurity.CurrentUserId
            }).ToList();
            var data = SqlHelper.QuerySP<LeadModel>("LoadLeadIndex", new
            {
                @pId = WebSecurity.CurrentUserId,
                @pIsLeTan = (checkIsLetan.Count > 0) ? 1 : 0
            });
            var lead = data.Where(x => x.Id == Id).FirstOrDefault();
            if (status == -2)
            {
                status = omst != null ? omst.Id : 0;


            }
            else if (status == -3)
            {
                status = kmst != null ? kmst.Id : 0;
            }
            status = lead != null && omst != null && kmst != null && lead.StatusId != omst.Id && lead.StatusId != kmst.Id ? status : 0;
            string query = $"UPDATE Crm_Lead SET StatusId={status},ModifiedDate=@ModifiedDate,ModifiedUserId=@ModifiedUserId where Id={Id}";

            ViewBag.lead = lead != null ? lead : new LeadModel();
            leadModel.ModifiedDate = DateTime.Now;
            leadModel.ModifiedUserId = WebSecurity.CurrentUserId;
            ViewBag.lsstatus = lsstatus;
            // Thêm thuộc tính cho đối tượng ExpandoObject

            int kq = lead != null && status != lead.StatusId && status != -1 && status != 0 ? SqlHelper.ExecuteSQL(query, leadModel) : 2;
            if (kq == 1)
            {
                int a = SqlHelper.ExecuteSP("InsertLeadLogs", new
                {
                    @LeadId = Id
    ,
                    @Action = "CHANGESTATUS"
    ,
                    @Logs = lead.StatusId.ToString() + ',' + status
    ,
                    @isImportant = 0
    ,
                    @IdAction = -1
    ,
                    @UserId = WebSecurity.CurrentUserId,
                    @TypeLead = 1
                });
                var context = Microsoft.AspNet.SignalR.GlobalHost.ConnectionManager.GetHubContext<ErpHub>();
                context.Clients.All.f5LeadLogs(Id);
                lead.StatusId = status;
            }
            ViewBag.kq = kq;
            var chkend = customerRepository.GetAllCustomer().Where(x => x.LeadId == Id).Count() > 0 ? 1 : 0;
            ViewBag.chkend = chkend;
            return PartialView();
        }

        [HttpPost]
        [AllowAnonymousAttribute]
        public JsonResult UptStatusMultiLead(string selectedId, int statusid, int? ispartial) //-2 chưa mua, -3 khách mua, -1 get all
        {
            LeadModel leadModel = new LeadModel();
            var lsstatus = SqlHelper.QuerySP<StatusLeadModel>("GetAllStatus", new { @isPartial = ispartial });
            var omst = lsstatus.Where(x => x.EndStatus == false).FirstOrDefault();
            var kmst = lsstatus.Where(x => x.EndStatus == true).FirstOrDefault();
            var checkIsLetan = SqlHelper.QuerySP<int>("checkUserIsLetan", new
            {
                @pId = WebSecurity.CurrentUserId
            }).ToList();
            var data = SqlHelper.QuerySP<LeadModel>("LoadLeadIndex", new
            {
                @pId = WebSecurity.CurrentUserId,
                @pIsLeTan = (checkIsLetan.Count > 0) ? 1 : 0
            });
            if (statusid == -2)
            {
                statusid = omst != null ? omst.Id : 0;


            }
            else if (statusid == -3)
            {
                statusid = kmst != null ? kmst.Id : 0;
            }
            string[] strings = selectedId.Split(';');
            if (strings.Length > 0 && strings[0] != "")
            {
                foreach (string s in strings)
                {
                    var lead = data.Where(x => x.Id == int.Parse(s)).FirstOrDefault();
                    statusid = lead != null && omst != null && kmst != null && lead.StatusId != omst.Id && lead.StatusId != kmst.Id ? statusid : 0;
                    string query = $"UPDATE Crm_Lead SET StatusId={statusid},ModifiedDate=@ModifiedDate,ModifiedUserId=@ModifiedUserId where Id={int.Parse(s)}";

                    leadModel.ModifiedDate = DateTime.Now;
                    leadModel.ModifiedUserId = WebSecurity.CurrentUserId;
                    int kq = lead != null && statusid != lead.StatusId && statusid != -1 && statusid != 0 ? SqlHelper.ExecuteSQL(query, leadModel) : 2;
                    if (kq == 1)
                    {
                        int a = SqlHelper.ExecuteSP("InsertLeadLogs", new
                        {
                            @LeadId = int.Parse(s)
            ,
                            @Action = "CHANGESTATUS"
            ,
                            @Logs = lead.StatusId.ToString() + ',' + statusid
            ,
                            @isImportant = 0
            ,
                            @IdAction = -1
            ,
                            @UserId = WebSecurity.CurrentUserId,
                            @TypeLead = 1
                        });
                    }
                }

            }
            return Json(new { success = true });
        }
        [AllowAnonymous]
        [HttpPost]
        public JsonResult DeleteLead(int[] Id)
        {
            try
            {
                foreach (int i in Id)
                {
                    SqlHelper.ExecuteSP("DeleteLead", new
                    {
                        @pId = i,
                        @pModifiedUserId = WebSecurity.CurrentUserId
                    });
                }
            }
            catch
            {
                return Json(0);
            }
            return Json(1);
        }
        [HttpGet]
        public JsonResult CheckExistLeadId(int LeadId)
        {
            var chkend = customerRepository.GetAllCustomer().Where(x => x.LeadId == LeadId).FirstOrDefault();
            return Json(chkend != null ? chkend.Id : 0, JsonRequestBehavior.AllowGet);
        }
        [AllowAnonymous]
        [HttpGet]
        public ActionResult LeadLogsView(int Id, int? isPartial)
        {
            var data = SqlHelper.QuerySP<LeadLogsModel>("GetLeadLogs", new { Id = Id, isPartial = isPartial });
            ViewBag.status = SqlHelper.QuerySP<StatusLeadModel>("GetAllStatus", new { isPartial = 3 });
            ViewBag.LeadId = Id;
            return PartialView(data);
        }
        //leadlogs của LeadMeeting
        [AllowAnonymous]
        [HttpGet]
        public ActionResult LeadLogsMeetingView(int LeadId, int? isPartial)
        {
            var data = SqlHelper.QuerySP<LeadLogsModel>("sp_Crm_LeadLogsByLeadId_GET", new { LeadId = LeadId, isPartial = isPartial });
    
            ViewBag.LeadId = LeadId;
            return PartialView(data);
        }
        //leadlogs của LeadMeeting
        [AllowAnonymous]
        [HttpGet]
        public ActionResult PinCmtView(int Id, int? isPartial)
        {
            var data = SqlHelper.QuerySP<LeadLogsModel>("sp_Crm_PinCmtByLeadId_GET", new { LeadId = Id, isPartial = isPartial });
            ViewBag.LeadId = Id;
            return PartialView(data);
        }
        [HttpPost]
        public JsonResult SendCmtLead([System.Web.Http.FromBody] string message, int id, int? isPartial, HttpPostedFileBase file)
        {
            var upload = Server.MapPath("/Uploads/Lead/Cmt/");
            if (!fileio.Exists(upload))
                Directory.CreateDirectory(upload);
            if (file != null && file.ContentLength > 0)
            {
                string filename = upload + file.FileName;
                if (fileio.Exists(filename))
                    fileio.Delete(filename);
                file.SaveAs(filename);
            }
            var kq = SqlHelper.QuerySP("Insert_LogChat", new
            {
                @createdDate = DateTime.Now,
                @createdUserId = WebSecurity.CurrentUserId,
                @modifiedDate = DateTime.Now,
                @modifiedUserId = WebSecurity.CurrentUserId,
                @username = WebSecurity.CurrentUserName,
                @content = message,
                @idExcute = 0,
                @maDviqly = "PE0800"
            }).ToList().FirstOrDefault();
            if (kq != null && kq.ChatId > 0)
            {
                SqlHelper.ExecuteSP("InsertLeadLogs", new
                {
                    @LeadId = id
    ,
                    @Action = "COMMENT"
    ,
                    @TypeLead = isPartial == 1 ? 0 : 1,
                    @Logs = ""
    ,
                    @filepath = file?.FileName
    ,
                    @isImportant = 0
    ,
                    @IdAction = kq.ChatId
    ,
                    @UserId = WebSecurity.CurrentUserId
                });
                var context = Microsoft.AspNet.SignalR.GlobalHost.ConnectionManager.GetHubContext<ErpHub>();
                context.Clients.All.f5LeadLogs(id);
                return Json(1);

            }
            return Json(0);
        }
        [HttpPost]
        public async Task<ActionResult> SaveNewLead(LeadModel leadModel, int? CusId = null)
        {
            try
            {
                string parameters = "";
                string columns = "";
                string updatesql = "";
                string query = "";
                int chkendstatus = 0;
                bool? endstatus = null;
                var properties = typeof(LeadModel).GetProperties();
                if (leadModel.Id > 0)
                {
                    var IsLetan = SqlHelper.QuerySP<int>("checkUserIsLetan", new
                    {
                        @pId = WebSecurity.CurrentUserId
                    }).ToList();
                    var data = SqlHelper.QuerySP<LeadModel>("LoadLeadIndex", new
                    {
                        @pId = WebSecurity.CurrentUserId,
                        @pIsLeTan = (IsLetan.Count > 0) ? 1 : 0
                    });
                    var lead = data.Where(x => x.Id == leadModel.Id).FirstOrDefault();
                    var section = SqlHelper.QueryMultipleSP<LeadSectionModel, LeadSection_FieldModel>("AddNewModal", new { @isPartial = CusId > 0 ? 1 : 0 });
                    List<LeadSection_FieldModel> lsSection = new List<LeadSection_FieldModel>();
                    lsSection.AddRange(section.Item2);

                    foreach (var item in properties)
                    {
                        if (lsSection.Any(x => x.FieldName == item.Name))
                        {
                            var value = item.GetValue(leadModel);

                            if (value != null)
                            {
                                string edit = value.ToString();

                                foreach (var s in lsSection)
                                {
                                    if (s.FieldName == item.Name && (s.TypeField == "Date" || s.TypeField == "Datetime"))
                                    {
                                        if (s.TypeField.IndexOf("Datetime", StringComparison.OrdinalIgnoreCase) >= 0)
                                        {
                                            DateTime dateTimeValue;
                                            if (DateTime.TryParse(edit, out dateTimeValue))
                                            {
                                                edit = dateTimeValue.ToString("dd/MM/yyyy HH:mm");
                                            }
                                        }

                                        if (s.TypeField == "Date")
                                        {
                                            DateTime dateValue;
                                            if (DateTime.TryParse(edit, out dateValue))
                                            {
                                                edit = dateValue.ToString("dd/MM/yyyy");
                                            }
                                        }
                                    }
                                }

                                item.SetValue(leadModel, edit);
                            }
                        }
                    }
                    if (leadModel.IsCancel == 1 && leadModel.F48 == null)
                        return Json(-1);
                    if (leadModel.IsCancel == null)
                    {
                        lsSection.AddRange(section.Item2.Where(x => x.LeadSectionId != 2));
                        foreach (var item in properties)
                        {
                            if (lsSection.Any(x => x.FieldName == item.Name))
                            {
                                string edit = item.GetValue(leadModel) != null ? item.GetValue(leadModel).ToString() : "";
                                string curr = item.GetValue(lead) != null ? item.GetValue(lead).ToString() : "";

                                if (edit != curr)
                                {
                                    return Json(0);
                                }
                            }
                        }
                    }
                    leadModel.StatusId = lead.StatusId;
                    string[] dontupt = new string[] { "Id", "CreatedDate", "CreatedUserId", "AssignedUserId", "CustomerCode" };
                    var prop = properties.Where(x => !dontupt.Contains(x.Name));
                    updatesql = string.Join(", ", prop.Select(i => i.Name + $"=@{i.Name}"));
                    var ruledt = SqlHelper.QuerySP<RuleDetailModel>("CheckRulesLead");
                    var Rule = SqlHelper.QuerySP<RuleModel>("GetAllRule", new { isPartial = CusId > 0 ? 1 : 0 });
                    var rulesms = Rule.Where(x => x.StatusFrom == lead.StatusId);
                    var TyleRule = SqlHelper.QuerySP<TyleRuleModel>("GetTyleRule");
                    var rulessms = rulesms.Where(x => x.TypeRuleId == TyleRule.Where(y => y.TypeRule == "SENDSMS").FirstOrDefault()?.Id);
                    var ruleszl = rulesms.Where(x => x.TypeRuleId == TyleRule.Where(y => y.TypeRule == "SENDZALO").FirstOrDefault()?.Id);
                    var rulesmail = rulesms.Where(x => x.TypeRuleId == TyleRule.Where(y => y.TypeRule == "SENDEMAIL").FirstOrDefault()?.Id);
                    var rulestatus = rulesms.Where(x => x.TypeRuleId == TyleRule.Where(y => y.TypeRule == "CHANGESTATUS").FirstOrDefault()?.Id);
                    if (rulestatus != null)
                    {
                        var obj = HandleLogicRule(rulestatus, leadModel);
                        chkendstatus = int.Parse(obj.GetType().GetProperty("Id")?.GetValue(obj).ToString() ?? "0");
                        endstatus = bool.Parse(obj.GetType().GetProperty("endstatus")?.GetValue(obj).ToString() ?? "False");


                    }
                    if (rulessms != null)
                    {
                        string sentence = "";
                        RuleModel chk = InsertLeadLogsSMS(0, rulessms, "", 0, leadModel);
                        if (chk.Id > 0)
                        {
                            sentence = String.Copy(chk.ContentRule);

                            // Trích xuất các giá trị trong cặp ngoặc nhọn {{}}
                            List<string> extractedValues = ExtractValuesFromBrackets(sentence);
                            var SystemPara = SqlHelper.QuerySP<SystemParametersViewModel>("spCrm_SystemParameters").ToList();

                            foreach (var ite in extractedValues)
                            {
                                var storedprocedure = SystemPara.FirstOrDefault(x => x.Name == ite)?.Storedprocedure;
                                var fiedName = SystemPara.FirstOrDefault(x => x.Name == ite)?.FiedName;
                                if (storedprocedure != null)
                                {
                                    var resultSMS = SqlHelper.QuerySP(storedprocedure, new
                                    {
                                        @pColumnName = fiedName,
                                        @pLeadId = leadModel.Id
                                    });
                                    // Thay thế giá trị result vào sentence
                                    sentence = sentence.Replace($"{{{{{ite}}}}}", resultSMS.First().ValueByName);
                                }
                            }
                            if (leadModel.Mobile != null && leadModel.Mobile != "")
                            {
                                try
                                {

                                    var rs = (await SendApiSMS(sentence, leadModel.Mobile)) as JsonResult;
                                    dynamic result = rs.Data as dynamic;
                                    if (result.Success == true && result.CodeResult == "100")
                                    {
                                        CreateSMSLeadLogs(leadModel.Id, sentence, 1, leadModel.Mobile, "100");
                                    }
                                }
                                catch (Exception ex)
                                {
                                }

                            }

                        }
                    }
                    //if (ruleszl != null && leadModel.F17 == "1")
                    //{
                    //    RuleModel chk = InsertLeadLogsSMS(0, ruleszl, "", 0, leadModel);
                    //    if (chk.Id > 0)
                    //    {
                    //        //send(ruleszl.ContentRule, leadModel.Mobile);
                    //    }
                    //}
                    if (rulesmail != null)
                    {
                        RuleModel chk = InsertLeadLogsSMS(0, rulesmail, "", 0, leadModel);
                        if (chk.Id > 0 && leadModel.Email != null && leadModel.Email != "")
                        {
                            try
                            {
                                AdviseCardSendEmailViewModel adviseCardSendEmailViewModel = new AdviseCardSendEmailViewModel() { Body = chk.ContentEmail, Receiver = leadModel.Email, Sender = ConfigurationManager.AppSettings["EmailSender"].ToString(), Title = chk.TileEmail };
                                SendEmail(adviseCardSendEmailViewModel, leadModel.Id);
                            }
                            catch (Exception ex)
                            {
                            }

                        }
                    }
                    query = $"UPDATE Crm_Lead SET {updatesql},TotalMoney=@F44,BranchId=@F12 where Id={leadModel.Id} ;IF @@ROWCOUNT > 0 SELECT {leadModel.Id}; ELSE SELECT 0;";
                }
                else
                {
                    var prop = properties.Where(x => x.Name != "Id" && x.Name != "CustomerCode");
                    columns = string.Join(", ", prop.Select(i => i.Name));
                    parameters = string.Join(", ", prop.Select(i => $"@{i.Name}"));
                    if (CusId > 0)
                    {
                        columns += ", CustomerId";
                        parameters += $", {CusId}";
                    }
                    query = $"DECLARE @NewLeadId INT; INSERT INTO Crm_Lead ({columns},BranchId) VALUES ({parameters},@F12);SET @NewLeadId = SCOPE_IDENTITY();SELECT @NewLeadId AS NewLeadId;";
                    // Tạo một đối tượng ExpandoObject
                    leadModel.CreatedDate = DateTime.Now;
                    leadModel.CreatedUserId = WebSecurity.CurrentUserId;
                    leadModel.AssignedUserId = WebSecurity.CurrentUserId;
                }
                leadModel.ModifiedDate = DateTime.Now;
                leadModel.ModifiedUserId = WebSecurity.CurrentUserId;
                // Thêm thuộc tính cho đối tượng ExpandoObject
                //begin hoapd them get trang thai ban dau
                int pStatusId = SqlHelper.QuerySP<int>("getTrangthaibandaucualead", new { @ispartial = CusId > 0 ? 1 : 0 }).FirstOrDefault();
                leadModel.StatusId = leadModel.Id > 0 ? leadModel.StatusId : pStatusId;
                //end hoapd them get trang thai ban dau

                //hoapd them de su ly may truong so
                if (leadModel.F44 != null & leadModel.F44 != "")
                {
                    leadModel.F44 = leadModel.F44.Replace(",", "").Replace(".", "");
                }
                if (leadModel.F45 != null & leadModel.F45 != "")
                {
                    leadModel.F45 = leadModel.F45.Replace(",", "");
                }
                if (leadModel.F46 != null & leadModel.F46 != "")
                {
                    leadModel.F46 = leadModel.F46.Replace(",", "");
                }
              int kq = SqlHelper.QuerySQL<int>(query, leadModel).FirstOrDefault();
                if (kq > 0)
                {
                    if (query.Contains("INSERT INTO Crm_Lead"))
                    {
                      SqlHelper.ExecuteSP("InsertLeadLogs", new
                        {
                            @LeadId = kq
    ,
                            @Action = "LEADCREATE"
    ,
                            @Logs = ""
    ,
                            @isImportant = 0
    ,
                            @IdAction = 0
    ,
                            @UserId = WebSecurity.CurrentUserId,
                            @TypeLead = 1
                        });
                    }
                    else
                    {
                        SqlHelper.ExecuteSP("InsertLeadLogs", new
                        {
                            @LeadId = kq
    ,
                            @Action = "OTHER"
    ,
                            @Logs = "Sửa thông tin chung của Lead"
    ,
                            @isImportant = 0
    ,
                            @IdAction = -1
    ,
                            @UserId = WebSecurity.CurrentUserId,
                            @TypeLead = 1
                        });
                        var context = Microsoft.AspNet.SignalR.GlobalHost.ConnectionManager.GetHubContext<ErpHub>();
                        context.Clients.All.f5LeadLogs(kq);
                    }

                }
                if (chkendstatus > 0)
                    return Json(new { chkendstatus, endstatus = endstatus });
                // Truy cập và sử dụng thuộc tính được thêm vào
                //connection.Execute(query, (object)dynamicObject);
                var checkIsLetan = SqlHelper.QuerySP<int>("checkUserIsLetan", new
                {
                    @pId = WebSecurity.CurrentUserId
                }).ToList();
                var data1 = SqlHelper.QuerySP<LeadModel>("LoadLeadIndex", new
                {
                    @pId = WebSecurity.CurrentUserId,
                    @pIsLeTan = (checkIsLetan.Count > 0) ? 1 : 0,
                    @pIsLead = CusId > 0 ? -1 : 0
                });
                int pageSize = Request.Cookies["pageSize"] != null ? int.Parse(Request.Cookies["pageSize"].Value) : 15;

                IPagedList pagedList = new PagedList<LeadModel>(data1, 1, pageSize);

                var result1 = LeadPartialView(data1.Skip((1 - 1) * pageSize).Take(pageSize), pagedList, null, -1, CusId > 0 ? -1 : 0);
                ControllerContext.RouteData.Values["action"] = "LeadPartialView";
                Response.Headers.Add("id", kq.ToString());
                return result1;
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }
        [AllowAnonymous]
        [HttpPost]
        public async Task<JsonResult> QuickEditLead(QuickEditLeadModel model, long[] valid, int ldcusview = 0)
        {
            try
            {
                string parameters = "";
                string columns = "";
                string updatesql = "";
                string updatesql1 = "";
                string query = "";
                var properties = typeof(LeadModel).GetProperties();
                foreach (var leadModel in model.LeadModel)
                {
                    if (leadModel.Id > 0 && valid.Contains(leadModel.Id))
                    {
                        var checkIsLetan = SqlHelper.QuerySP<int>("checkUserIsLetan", new
                        {
                            @pId = WebSecurity.CurrentUserId
                        }).ToList();
                        var data = SqlHelper.QuerySP<LeadModel>("LoadLeadIndex", new
                        {
                            @pId = WebSecurity.CurrentUserId,
                            @pIsLeTan = (checkIsLetan.Count > 0) ? 1 : 0
                        });
                        var lead = data.Where(x => x.Id == leadModel.Id).FirstOrDefault();

                        leadModel.StatusId = lead.StatusId;

                        string[] doupt = new string[] { "F2", "F3", "IsCancel", "ReceptionStaffId", "StatusId", "ModifiedDate", "ModifiedUserId" };
                        var prop = properties.Where(x => doupt.Contains(x.Name));
                        updatesql = string.Join(", ", prop.Select(i => i.Name + $"=@{i.Name}"));

                        updatesql1 = string.Join(", ", properties.Where(x => x.Name.ToLower() != "id" && x.Name.ToLower() != "createddate" && x.Name.ToLower() != "assigneduserid" && x.Name.ToLower() != "createduserid").Select(i => i.Name + $"=@{i.Name}")); //Khang - update all columns
                        var section = SqlHelper.QueryMultipleSP<LeadSectionModel, LeadSection_FieldModel>("AddNewModal", new { @isPartial = ldcusview == -1 || ldcusview == 1 ? 1 : 0 });
                        List<LeadSection_FieldModel> lsSection = new List<LeadSection_FieldModel>();
                        lsSection.AddRange(section.Item2);
                        foreach (var item in properties)
                        {
                            if (lsSection.Any(x => x.FieldName == item.Name))
                            {
                                var value = item.GetValue(leadModel);

                                if (value != null)
                                {
                                    string edit = value.ToString();

                                    foreach (var s in lsSection)
                                    {
                                        if (s.FieldName == item.Name && (s.TypeField == "Date" || s.TypeField == "Datetime"))
                                        {
                                            if (s.TypeField.IndexOf("Datetime", StringComparison.OrdinalIgnoreCase) >= 0)
                                            {
                                                DateTime dateTimeValue;
                                                if (DateTime.TryParse(edit, out dateTimeValue))
                                                {
                                                    edit = dateTimeValue.ToString("dd/MM/yyyy HH:mm");
                                                }
                                            }

                                            if (s.TypeField == "Date")
                                            {
                                                DateTime dateValue;
                                                if (DateTime.TryParse(edit, out dateValue))
                                                {
                                                    edit = dateValue.ToString("dd/MM/yyyy");
                                                }
                                            }
                                        }
                                    }

                                    item.SetValue(leadModel, edit);
                                }
                            }
                        }
                        var ruledt = SqlHelper.QuerySP<RuleDetailModel>("CheckRulesLead");
                        var Rule = SqlHelper.QuerySP<RuleModel>("GetAllRule", new { isPartial = ldcusview == -1 ? 1 : 0 });
                        var rulesms = Rule.Where(x => x.StatusFrom == lead.StatusId);
                        var TyleRule = SqlHelper.QuerySP<TyleRuleModel>("GetTyleRule");
                        var rulessms = rulesms.Where(x => x.TypeRuleId == TyleRule.Where(y => y.TypeRule == "SENDSMS").FirstOrDefault()?.Id);
                        var ruleszl = rulesms.Where(x => x.TypeRuleId == TyleRule.Where(y => y.TypeRule == "SENDZALO").FirstOrDefault()?.Id);
                        var rulesmail = rulesms.Where(x => x.TypeRuleId == TyleRule.Where(y => y.TypeRule == "SENDEMAIL").FirstOrDefault()?.Id);

                        var rulestatus = rulesms.Where(x => x.TypeRuleId == TyleRule.Where(y => y.TypeRule == "CHANGESTATUS").FirstOrDefault()?.Id);
                        if (rulestatus != null)
                        {
                            var obj = HandleLogicRule(rulestatus, leadModel);
                        }
                        if (rulessms != null)
                        {
                            string sentence = "";
                            RuleModel chk = InsertLeadLogsSMS(0, rulessms, "", 0, leadModel);
                            if (chk.Id > 0)
                            {
                                sentence = String.Copy(chk.ContentRule);

                                // Trích xuất các giá trị trong cặp ngoặc nhọn {{}}
                                List<string> extractedValues = ExtractValuesFromBrackets(sentence);
                                var SystemPara = SqlHelper.QuerySP<SystemParametersViewModel>("spCrm_SystemParameters").ToList();

                                foreach (var ite in extractedValues)
                                {
                                    var storedprocedure = SystemPara.FirstOrDefault(x => x.Name == ite)?.Storedprocedure;
                                    var fiedName = SystemPara.FirstOrDefault(x => x.Name == ite)?.FiedName;
                                    if (storedprocedure != null)
                                    {
                                        var resultSMS = SqlHelper.QuerySP(storedprocedure, new
                                        {
                                            @pColumnName = fiedName,
                                            @pLeadId = leadModel.Id
                                        });
                                        // Thay thế giá trị result vào sentence
                                        sentence = sentence.Replace($"{{{{{ite}}}}}", resultSMS.First().ValueByName);
                                    }
                                }
                                if (leadModel.Mobile != null && leadModel.Mobile != "")
                                {
                                    try
                                    {

                                        var rs = (await SendApiSMS(sentence, leadModel.Mobile)) as JsonResult;
                                        dynamic result = rs.Data as dynamic;
                                        if (result.Success == true && result.CodeResult == "100")
                                        {
                                            CreateSMSLeadLogs(leadModel.Id, sentence, 1, leadModel.Mobile, "100");
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                    }

                                }

                            }
                        }
                        if (rulesmail != null)
                        {
                            RuleModel chk = InsertLeadLogsSMS(0, rulesmail, "", 0, leadModel);
                            if (chk.Id > 0 && leadModel.Email != null && leadModel.Email != "")
                            {
                                try
                                {
                                    AdviseCardSendEmailViewModel adviseCardSendEmailViewModel = new AdviseCardSendEmailViewModel() { Body = chk.ContentEmail, Receiver = leadModel.Email, Sender = ConfigurationManager.AppSettings["EmailSender"].ToString(), Title = chk.TileEmail };
                                    SendEmail(adviseCardSendEmailViewModel, leadModel.Id);
                                }
                                catch (Exception ex)
                                {
                                }

                            }
                        }

                        //query = $"UPDATE Crm_Lead SET {updatesql1} where Id={leadModel.Id}";
                        query = $"UPDATE Crm_Lead SET {updatesql1.Replace("CustomerCode=@CustomerCode, ", "")} where Id={leadModel.Id}";
                        if (leadModel.CustomerCode != null)
                        {
                            var update = SqlHelper.QuerySP<int>("[sp_Crm_CodeCustomer_Update]", new
                            {
                                pLeadId = leadModel.Id,
                                pCustomerCode = leadModel.CustomerCode
                            }).FirstOrDefault();
                        }
                        leadModel.ModifiedDate = DateTime.Now;
                        leadModel.ModifiedUserId = WebSecurity.CurrentUserId;
                        // Thêm thuộc tính cho đối tượng ExpandoObject

                        if (ModelState.IsValid)
                        {

                        }
                        SqlHelper.ExecuteSQL(query, leadModel); //Khang addd - update all column
                                                                // Truy cập và sử dụng thuộc tính được thêm vào
                                                                //connection.Execute(query, (object)dynamicObject);
                    }

                }
            }
            catch
            {
                return Json(0);
            }

            return Json(1);
        }
        [HttpPost]
        public ActionResult SearchLead(LeadSearchModel leadModel, int pageNumber, int pageSize, string columnsort, int? sortdir, int ldcusview = 0)
        {
            var checkIsLetan = SqlHelper.QuerySP<int>("checkUserIsLetan", new
            {
                @pId = WebSecurity.CurrentUserId
            }).ToList();
            int? chkacti = null;
            string chkcreated = null;
            string chkmodified = null;
            if (leadModel.keyValues != null && leadModel.keyValues.Length > 0)
            {
                foreach (var item in leadModel.keyValues)
                {
                    if (item.Key == "Activities")
                    {
                        chkacti = string.IsNullOrEmpty(item.Value) ? (int?)null : item.Value == "1" ? 1 : 0;
                    }
                    if (item.Key == "ModifiedBy")
                    {
                        chkmodified = item.Value;
                    }
                    if (item.Key == "CreatedBy")
                    {
                        chkcreated = item.Value;
                    }

                }
            }
            var model = SqlHelper.QuerySP<LeadModel>("LoadLeadIndex", new
            {
                @pId = WebSecurity.CurrentUserId,
                @pIsLeTan = (checkIsLetan.Count > 0) ? 1 : 0,
                @pIsLead = ldcusview,
                @chkacti = chkacti
            });
            int ipageNumber = (pageNumber != null ? (int)pageNumber : 1);
            if (leadModel.keyValues != null && leadModel.keyValues.Length > 0)
            {
                foreach (var item in leadModel.keyValues)
                {
                    if (item.Key.Split(',').Length > 1 && item.Key.Split(',')[1] == "Start" && !string.IsNullOrEmpty(item.Value))
                    {
                        model = model.Where(x => typeof(LeadModel).GetProperty(item.Key.Split(',')[0]).GetValue(x) != null && typeof(LeadModel).GetProperty(item.Key.Split(',')[0]).GetValue(x).ToString() != "" ? DateTime.Parse(typeof(LeadModel).GetProperty(item.Key.Split(',')[0]).GetValue(x).ToString()) >= DateTime.Parse(item.Value) : false);
                    }
                    else if (item.Key.Split(',').Length > 1 && item.Key.Split(',')[1] == "End" && !string.IsNullOrEmpty(item.Value))
                    {
                        model = model.Where(x => typeof(LeadModel).GetProperty(item.Key.Split(',')[0]).GetValue(x) != null && typeof(LeadModel).GetProperty(item.Key.Split(',')[0]).GetValue(x).ToString() != "" ? DateTime.Parse(typeof(LeadModel).GetProperty(item.Key.Split(',')[0]).GetValue(x).ToString()) <= DateTime.Parse(item.Value) : false);

                    }
                    else if (item.Key == "CreatedByFrom" && string.IsNullOrEmpty(chkcreated) && !string.IsNullOrEmpty(item.Value))
                    {
                        model = model.Where(x => x.CreatedDate >= DateTime.Parse(item.Value));
                    }
                    else if (item.Key == "CreatedByTo" && string.IsNullOrEmpty(chkcreated) && !string.IsNullOrEmpty(item.Value))
                    {
                        model = model.Where(x => x.CreatedDate <= DateTime.Parse(item.Value));

                    }
                    else if (item.Key == "ModifiedByFrom" && string.IsNullOrEmpty(chkmodified) && !string.IsNullOrEmpty(item.Value))
                    {
                        model = model.Where(x => x.ModifiedDate >= DateTime.Parse(item.Value));

                    }
                    else if (item.Key == "ModifiedByTo" && string.IsNullOrEmpty(chkmodified) && !string.IsNullOrEmpty(item.Value))
                    {
                        model = model.Where(x => x.ModifiedDate <= DateTime.Parse(item.Value));

                    }
                    else if (item.Key == "CreatedBy" && !string.IsNullOrEmpty(chkcreated) && !string.IsNullOrEmpty(item.Value))
                    {
                        var daynow = DateTime.Now;
                        int dayinw = (int)daynow.DayOfWeek;
                        DateTime mond = daynow.Date.AddDays(-dayinw + 1);
                        switch (item.Value)
                        {
                            case "0":
                                model = model.Where(x => x.CreatedDate != null ? x.CreatedDate.Value.Date == daynow.Date.AddDays(-1) : false);
                                break;
                            case "1":
                                model = model.Where(x => x.CreatedDate != null ? x.CreatedDate.Value.Date == daynow.Date : false);
                                break;
                            case "2":
                                model = model.Where(x => x.CreatedDate != null ? x.CreatedDate.Value.Date >= mond &&
                                x.CreatedDate.Value.Date <= mond.AddDays(6).AddHours(23).AddMinutes(59) : false);
                                break;
                            default:
                                break;
                        }

                    }
                    else if (item.Key == "ModifiedBy" && !string.IsNullOrEmpty(chkmodified) && !string.IsNullOrEmpty(item.Value))
                    {
                        var daynow = DateTime.Now;
                        int dayinw = (int)daynow.DayOfWeek;
                        DateTime mond = daynow.Date.AddDays(-dayinw + 1);
                        switch (item.Value)
                        {
                            case "0":
                                model = model.Where(x => x.ModifiedDate != null ? x.ModifiedDate.Value.Date == daynow.Date.AddDays(-1) : false);
                                break;
                            case "1":
                                model = model.Where(x => x.ModifiedDate != null ? x.ModifiedDate.Value.Date == daynow.Date : false);
                                break;
                            case "2":
                                model = model.Where(x => x.ModifiedDate != null ? x.ModifiedDate.Value.Date >= mond &&
                                x.ModifiedDate.Value.Date <= mond.AddDays(6).AddHours(23).AddMinutes(59) : false);
                                break;
                            default:
                                break;
                        }
                    }
                    else if (item.ValueArr != null && item.ValueArr.Length > 0) //Helpers.Common.ChuyenThanhKhongDau(q[i].CompanyName);
                    {
                        //thanh
                        if (item.Key == "AssignedUserId")
                        {
                            model = model.Where(x =>
                            {
                                var itemKeyValue = typeof(LeadModel).GetProperty(item.Key).GetValue(x);
                                var createdUserIdValue = typeof(LeadModel).GetProperty("CreatedUserId").GetValue(x);

                                return (itemKeyValue != null ? item.ValueArr.Any(y => itemKeyValue.ToString().Contains(y)) : (createdUserIdValue != null ? item.ValueArr.Any(y => createdUserIdValue.ToString().Contains(y)) : false)
                                );
                            });
                        }
                        else if (item.Key == "StatusId")

                        {
                            model = model.Where(x =>
                            {
                                var propertyValue = typeof(LeadModel).GetProperty(item.Key).GetValue(x);
                                if (propertyValue != null)
                                {
                                    return item.ValueArr.Any(y => propertyValue.ToString().Equals(y));
                                }
                                return false;
                            });
                        }

                        else
                        {
                            model = model.Where(x => typeof(LeadModel).GetProperty(item.Key)?.GetValue(x) != null ? item.ValueArr.Any(y => typeof(LeadModel).GetProperty(item.Key).GetValue(x).ToString().Contains(y)) : false);
                        }
                    }
                }
            }
            var props = typeof(LeadModel);
            try
            {
                foreach (var item in props.GetProperties())
                {
                    if (item.PropertyType == typeof(String) && item.GetValue(leadModel) != null && item.GetValue(leadModel).ToString() != "")
                    {
                        model = model.Where(x => item.GetValue(x) != null ? Helpers.Common.ChuyenThanhKhongDau(item.GetValue(x).ToString()).ToLower().Trim().Contains(Helpers.Common.ChuyenThanhKhongDau(item.GetValue(leadModel).ToString()).ToLower().Trim()) : false);

                    }
                    else if (item.PropertyType == typeof(Int32?) && item.GetValue(leadModel) != null)
                    {
                        model = model.Where(x => item.GetValue(x) != null ? item.GetValue(x).ToString() == item.GetValue(leadModel).ToString() : false);
                    }
                }

            }
            catch (Exception ex)
            {

            }
            IPagedList pagedList = new PagedList<LeadModel>(model, pageNumber, pageSize);
            model = model.Skip((ipageNumber - 1) * pageSize).Take(pageSize);
            if (columnsort != null)
            {
                if (sortdir == 1)
                {
                    model = model.OrderByDescending(x => typeof(LeadModel).GetProperty(columnsort).GetValue(x)).ToList();
                }
                else
                {
                    model = model.OrderBy(x => typeof(LeadModel).GetProperty(columnsort).GetValue(x)).ToList();
                }

                var result = LeadPartialView(model, pagedList, columnsort, sortdir, ldcusview);
                ControllerContext.RouteData.Values["action"] = "LeadPartialView";
                return result;
            }
            else
            {
                var result = LeadPartialView(model, pagedList, null, -1, ldcusview);
                ControllerContext.RouteData.Values["action"] = "LeadPartialView";
                return result;
            }
        }
        public class CurrentEditLeadnModel
        {
            public int leadId { get; set; }
            public int stt { get; set; }
        }
        [AllowAnonymous]
        [HttpPost]
        public ActionResult CurrentEditLead(string requestData, int? ispartial)
        {
            List<CurrentEditLeadnModel> requestDataList = JsonConvert.DeserializeObject<List<CurrentEditLeadnModel>>(requestData);
            var checkIsLetan = SqlHelper.QuerySP<int>("checkUserIsLetan", new
            {
                @pId = WebSecurity.CurrentUserId
            }).ToList();
            var modelrs = SqlHelper.QuerySP<LeadModel>("LoadLeadIndex", new
            {
                @pId = WebSecurity.CurrentUserId,
                @pIsLeTan = (checkIsLetan.Count > 0) ? 1 : 0
            });
            var branch = Erp.Domain.Account.Helper.SqlHelper.QuerySP<Erp.Domain.Staff.Entities.Branch>("GetAllBranch");

            var section = SqlHelper.QueryMultipleSP<LeadSectionModel, LeadSection_FieldModel>("AddNewModal");
            var users = userRepository.GetAllUsers();
            var lsstatus = SqlHelper.QuerySP<StatusLeadModel>("GetAllStatus");
            var categorys = categoryRepository.GetAllCategories();
            var properties = typeof(LeadModel).GetProperties();
            int outchk;
            string[] dont = new string[] { "IsDeleted", "CreatedDate", "CreatedUserId", "ModifiedDate", "ModifiedUserId", "AssignedUserId", "UserIdZalo", "CountForBrand" , "CustomerCode" };
            if (ispartial == -1)
                dont.Append("CustomerCode");
            var prop = properties.Where(x => !x.Name.StartsWith("F") && !int.TryParse(x.Name.Remove(0, 1).ToString(), out outchk) && x.Name != "Id" && !dont.Contains(x.Name));
            List<string> resultv = new List<string>();
            foreach (var requ in requestDataList)
            {
                var modefn = modelrs.FirstOrDefault(x => x.Id == requ.leadId);
                string trele = "";
                string tdele = "";
                foreach (var item1 in prop)
                {
                    object dsplay = item1.GetValue(modefn);
                    string dsplay1 = dsplay != null ? dsplay.ToString().Trim() : "";
                    string dsplay2 = dsplay1 == "1" ? "Có" : dsplay1 == "0" ? "Không" : dsplay1;
                    if (item1.Name == "ReceptionStaffId")
                    {
                        string inputele;
                        List<SelectListItem> lstcus = new List<SelectListItem>();
                        foreach (var item2 in users)
                        {
                            SelectListItem selectListItem = new SelectListItem() { Text = item2.FullName, Value = item2.Id.ToString() };
                            lstcus.Add(selectListItem);
                        }
                        string optionele = "";
                        foreach (var item2 in lstcus)
                        {
                            if (modefn != null && item1.GetValue(modefn) != null && item2.Value == item1.GetValue(modefn).ToString())
                            {
                                optionele += "<option selected value=" + @item2.Value + ">" + @item2.Text + "</option>";
                            }
                            else
                            {
                                optionele += "<option value='" + @item2.Value + "'>" + @item2.Text + "</option>";
                            }
                        }
                        inputele = "<select class='edit-iput'  class=\"js-example-basic-singleq\" name='LeadModel[" + (requ.stt - 1).ToString() + "]." + item1.Name + "'><option " + (modefn != null && item1.GetValue(modefn) != null ? "" : "selected") + " value=''></option>" + optionele + "</select>";
                        tdele += "<td>" + inputele + "</td>";
                    }
                    else if (item1.Name == "IsCancel")
                    {
                        string kq = item1.GetValue(modefn) != null ? item1.GetValue(modefn).ToString() : "";
                        tdele += "<td><select class='edit-iput' name='LeadModel[" + (requ.stt - 1) + "]." + @item1.Name + "'><option value='' " + (kq == "" ? "selected" : "") + "></option><option value='0' " + (kq == "0" ? "selected" : "") + ">Không</option><option value='1' " + (kq == "1" ? "selected" : "") + ">Có</option></select></td>";
                    }
                    else if (item1.Name == "StatusId")
                    {
                        string inputele;
                        string optionele = "";
                        int idtemp = !string.IsNullOrEmpty(dsplay1) ? int.Parse(dsplay1) : 0;
                        var statusval = lsstatus.Where(x => x.Id == idtemp).FirstOrDefault();
                        foreach (var status in lsstatus)
                        {
                            if (status.Name == (statusval != null ? statusval.Name : ""))
                            {
                                optionele += "<option selected value='" + status.Id + "'>" + status.Name + "</option>";
                            }
                            else
                            {
                                optionele += "<option value='" + status.Id + "'>" + status.Name + "</option>";
                            }
                        }
                        inputele = "<select class='edit-iput' class=\"js-example-basic-singleq\" name='LeadModel[" + (requ.stt - 1).ToString() + "]." + @item1.Name + "'>" + optionele + "</select>";
                        tdele += "<td>" + inputele + "</td>";
                    }
                    else if (item1.Name == "YearofBirth")
                    {
                        string inputele;
                        inputele = "<input class='edit-iput' type='number' min = '1900' max='2100'  value = '" + dsplay1 + "'" + "name='LeadModel[" + (requ.stt - 1).ToString() + "]." + item1.Name + "'/>";
                        tdele += "<td>" + inputele + "</td>";
                    }
                    else if (item1.Name == "Mobile")
                    {
                        string inputele;
                        inputele = "<input class='edit-iput' type='tel'  value = '" + dsplay1 + "'" + "name='LeadModel[" + (requ.stt - 1).ToString() + "]." + item1.Name + "'/>";
                        tdele += "<td>" + inputele + "</td>";
                    }
                    else if (item1.Name == "LeadName")
                    {
                        string inputele;
                        inputele = "<input class='edit-iput' type='text' value = '" + dsplay1 + "'" + "name='LeadModel[" + (requ.stt - 1).ToString() + "]." + item1.Name + "'/>";
                        tdele += "<td style='' name='Link'>" + inputele + "</td>";
                    }
                    else if (item1.Name == "Source")
                    {
                        string inputele;
                        inputele = "<input class='edit-iput' type='text' value = '" + dsplay1 + "'" + "name='LeadModel[" + (requ.stt - 1).ToString() + "]." + item1.Name + "'/>";
                        tdele += "<td class=\"sub\" style=\"left: 54px; position: sticky; z-index: 1; background-color: rgb(255, 255, 255); color: rgb(0, 0, 0);\">" + inputele + "</td>";
                    }
                    else
                    {
                        string inputele;
                        inputele = "<input class='edit-iput' type='text' value = '" + dsplay1 + "'" + "name='LeadModel[" + (requ.stt - 1).ToString() + "]." + item1.Name + "'/>";
                        tdele += "<td>" + inputele + "</td>";
                    }
                }
                foreach (var item2 in section.Item1)
                {
                    var itemField = section.Item2.Where(x => x.LeadSectionId == item2.Id);
                    foreach (var item13 in itemField)
                    {
                        if (item13.IsHiden != true)
                        {
                            if (typeof(LeadModel).GetProperty(item13.FieldName) == null)
                                continue;
                            object dsplay = typeof(LeadModel).GetProperty(item13.FieldName).GetValue(modefn);
                            string dsplay1 = dsplay != null ? dsplay.ToString().Trim() : "";
                            string dsplay2 = dsplay1 == "1" ? "Có" : dsplay1 == "0" ? "Không" : dsplay1;
                            if (true)
                            {
                                string inputele = "";

                                switch (item13.TypeField)
                                {
                                    case "Date":
                                        var fieldValue = typeof(LeadModel).GetProperty(item13.FieldName).GetValue(modefn);
                                        var dateValue = fieldValue != null && DateTime.TryParse(fieldValue.ToString(), out var date) ? date.ToString("yyyy-MM-dd") : "";
                                        tdele += "<td><input class='edit-iput' value='" + dateValue + "' type='date' name='LeadModel[" + (requ.stt - 1) + "]." + item13.FieldName + "'/></td>";
                                        break;
                                    case "Datetime":
                                        var fieldValues = typeof(LeadModel).GetProperty(item13.FieldName).GetValue(modefn);
                                        var dateValues = fieldValues != null && DateTime.TryParse(fieldValues.ToString(), out var datetime) ? datetime.ToString("yyyy-MM-ddThh:mm:ss") : "";
                                        tdele += "<td><input class='edit-iput' value='" + dateValues + "' type='datetime-local' name='LeadModel[" + (requ.stt - 1) + "]." + item13.FieldName + "'/></td>";
                                        break;
                                    case "List":
                                        var itemList = categorys.Where(x => x.Code == item13.CodeList).ToList();
                                        string optionele = "";
                                        if (item13.FieldName == "F12")
                                        {

                                            foreach (var item23 in branch)
                                            {
                                                if (modefn != null && typeof(LeadModel).GetProperty(item13.FieldName).GetValue(modefn) != null && item23.Id.ToString() == typeof(LeadModel).GetProperty(item13.FieldName).GetValue(modefn).ToString())
                                                {
                                                    optionele += "<option selected value='" + item23.Id.ToString() + "'>" + item23.Name + "</option>";

                                                }
                                                else
                                                {
                                                    optionele += "<option value='" + item23.Id.ToString() + "'>" + item23.Name + "</option>";

                                                }

                                            }
                                        }
                                        else
                                        {
                                            foreach (var item23 in itemList)
                                            {
                                                if (typeof(LeadModel).GetProperty(item13.FieldName).GetValue(modefn) != null && item23.Value == typeof(LeadModel).GetProperty(item13.FieldName).GetValue(modefn).ToString())
                                                {
                                                    optionele += "<option selected value='" + item23.Value + "'>" + item23.Name + "</option>";
                                                }
                                                else
                                                {
                                                    optionele += "<option value='" + item23.Value + "'>" + item23.Name + "</option>";
                                                }
                                            }
                                        }
                                     
                                        inputele += "<select class='edit-iput' class='js-example-basic-singleq' name='LeadModel[" + (requ.stt - 1) + "]." + item13.FieldName + "'> <option " + (typeof(LeadModel).GetProperty(item13.FieldName).GetValue(modefn) != null ? "" : "selected") + " value =''></option>" + optionele + "</select>";
                                        tdele += "<td>" + inputele + "</td>";
                                        break;
                                    case "Number":
                                        tdele += "<td><input class='edit-iput' value='" + dsplay1 + "' type='number' oninput='javascript: if (this.value.length > this.maxLength) this.value = this.value.slice(0, this.maxLength);' maxlength='12' name='LeadModel[" + (requ.stt - 1) + "]." + item13.FieldName + "'/></td>";
                                        break;
                                    default:
                                        tdele += "<td><input value='" + dsplay1 + "' class='edit-iput' type='text' name='LeadModel[" + (requ.stt - 1) + "]." + item13.FieldName + "'/></td>";
                                        break;
                                }
                            }
                        }
                    }
                }
                trele = "<tr><td hidden><input data-id='1' name=LeadModel[" + (requ.stt - 1) + "].Id value='" + modefn.Id + "'/>" + modefn.Id + "</td><td class=\"sub\" style=\"left: 0px; position: sticky; z-index: 1; background-color: rgb(255, 255, 255); color: rgb(0, 0, 0);\"><input type=\"checkbox\" class=\"checkbox child\" checked role ='1'/></td><td class=\"sub\" style=\"left: 23px; position: sticky; z-index: 1; background-color: rgb(255, 255, 255); color: rgb(0, 0, 0);\">" + (requ.stt) + "</td>" + tdele + "</tr>";
                resultv.Add(trele);
            }
            return Json(resultv);
        }


        //Kanban lead - thanh
        public class UpdateStatusLeadKanbanModel
        {
            public int StatusId { get; set; }
            public int LeadId { get; set; }
        }
        public class DataPurchase
        {
            public string amount { get; set; }
            public string endDate { get; set; }
            public string brand { get; set; }
            public string reason { get; set; }
        }
        [AllowAnonymous]
        [HttpPost]
        public ActionResult UpdateStatusLeadKanban(string datalKb, string dataPurchase)
        {
            List<UpdateStatusLeadKanbanModel> dataListKb = JsonConvert.DeserializeObject<List<UpdateStatusLeadKanbanModel>>(datalKb);

            var checkIsLetan = SqlHelper.QuerySP<int>("checkUserIsLetan", new
            {
                @pId = WebSecurity.CurrentUserId
            }).ToList();
            var data = SqlHelper.QuerySP<LeadModel>("LoadLeadIndex", new
            {
                @pId = WebSecurity.CurrentUserId,
                @pIsLeTan = (checkIsLetan.Count > 0) ? 1 : 0
            });

            var dtUp = new UpdateStatusLeadKanbanModel();
            if (dataListKb.Count > 0)
            {
                foreach (var item in dataListKb)
                {
                    if (data.Where(x => x.StatusId == item.StatusId && x.Id == item.LeadId).Count() <= 0)
                    {
                        dtUp = item;
                    }

                }
            }
            LeadModel leadModel = new LeadModel();
            string query = $"UPDATE Crm_Lead SET StatusId={dtUp.StatusId},ModifiedDate=@ModifiedDate,ModifiedUserId=@ModifiedUserId where Id={dtUp.LeadId}";

            var lead = data.Where(x => x.Id == dtUp.LeadId).FirstOrDefault();
            var lsstatus = SqlHelper.QuerySP<StatusLeadModel>("GetAllStatus");
            ViewBag.lead = lead != null ? lead : new LeadModel();
            leadModel.ModifiedDate = DateTime.Now;
            leadModel.ModifiedUserId = WebSecurity.CurrentUserId;
            ViewBag.lsstatus = lsstatus;
            int kq = 0;

            if (lead != null)
                kq = dtUp.StatusId != lead.StatusId && dtUp.StatusId != -1 ? SqlHelper.ExecuteSQL(query, leadModel) : 2;
            if (kq == 1)
            {
                if (dtUp.StatusId == 27 || dtUp.StatusId == 28)
                {
                    DataPurchase listDataPurchase = JsonConvert.DeserializeObject<DataPurchase>(dataPurchase);
                    string F42 = listDataPurchase.endDate;
                    DateTime dateValue;
                    if (DateTime.TryParse(listDataPurchase.endDate, out dateValue))
                    {
                        F42 = dateValue.ToString("dd/MM/yyyy");
                    }
                    var customerPurchase = SqlHelper.QuerySP<int>("sp_Crm_CustomerPurchase_Update", new
                    {
                        // pStatusId = statusId,
                        pTotalMoney = listDataPurchase.amount,
                        pTimeEnd = listDataPurchase.endDate,
                        pLeadId = dtUp.LeadId,
                        pTheEnd = listDataPurchase.reason,
                        pCountForBrand = listDataPurchase.brand,
                        pF44 = listDataPurchase.amount == "NaN" ? null : listDataPurchase.amount,
                        pF42 = F42
                    }).FirstOrDefault();
                }
                int a = SqlHelper.ExecuteSP("InsertLeadLogs", new
                {
                    @LeadId = dtUp.LeadId,
                    @Action = "CHANGESTATUS",
                    @Logs = lead.StatusId.ToString() + ',' + dtUp.StatusId,
                    @isImportant = 0,
                    @IdAction = -1,
                    @UserId = WebSecurity.CurrentUserId,
                    @TypeLead = 1
                });
                lead.StatusId = dtUp.StatusId;

                return Json(true);
            }
            var context = Microsoft.AspNet.SignalR.GlobalHost.ConnectionManager.GetHubContext<ErpHub>();
            context.Clients.All.f5LeadLogs(dtUp.LeadId);
            return Json(false);
        }
        [AllowAnonymous]
        [HttpPost]
        public ActionResult F5LeadLogsKanBan(string datalKb)
        {
            List<UpdateStatusLeadKanbanModel> dataListKb = JsonConvert.DeserializeObject<List<UpdateStatusLeadKanbanModel>>(datalKb);

            var checkIsLetan = SqlHelper.QuerySP<int>("checkUserIsLetan", new
            {
                @pId = WebSecurity.CurrentUserId
            }).ToList();
            var data = SqlHelper.QuerySP<LeadModel>("LoadLeadIndex", new
            {
                @pId = WebSecurity.CurrentUserId,
                @pIsLeTan = (checkIsLetan.Count > 0) ? 1 : 0
            });

            var dtUp = new UpdateStatusLeadKanbanModel();
            if (dataListKb.Count > 0)
            {
                foreach (var item in dataListKb)
                {
                    if (data.Where(x => x.StatusId == item.StatusId && x.Id == item.LeadId).Count() <= 0)
                    {
                        dtUp = item;
                    }

                }
            }
            var context = Microsoft.AspNet.SignalR.GlobalHost.ConnectionManager.GetHubContext<ErpHub>();
            context.Clients.All.f5LeadLogs(dtUp.LeadId);
            return Json(true);
        }
        public PartialViewResult LeadKanbanPartial(IEnumerable<LeadModel> model, int? ldcusview)
        {
            model = model != null ? model : new List<LeadModel>();
            var lsstatus = SqlHelper.QuerySP<StatusLeadModel>("GetAllStatusByUserId", new
            {
                @isAdminCRM = WebSecurity.CurrentUserId,
                @pLdcusview = ldcusview
            });
            foreach (var item in lsstatus)
            {
                item.LeadCount = model.Where(x => x.StatusId == item.Id).Count();
                //item.TotalPrice = model.Where(x => x.StatusId == item.Id && x.F44 != null).Sum(x => Convert.ToInt32(x.F44));
                item.TotalPrice = model.Where(x => x.StatusId == item.Id && x.F44 != null).Sum(x => Convert.ToDecimal(x.F44));
            }
            ViewBag.lsstatus = lsstatus;
            var section = SqlHelper.QueryMultipleSP<LeadSectionModel, LeadSection_FieldModel>("AddNewModal");
            ViewBag.section = section;
            return PartialView(model);
        }
        [AllowAnonymous]
        [HttpPost]
        public ActionResult SearchLeadKanban(LeadSearchModel leadModel, int? ldcusview = null)
        {
            var checkIsLetan = SqlHelper.QuerySP<int>("checkUserIsLetan", new
            {
                @pId = WebSecurity.CurrentUserId
            }).ToList();
            var leaddt = SqlHelper.QuerySP<LeadModel>("LoadLeadIndex", new
            {
                @pId = WebSecurity.CurrentUserId,
                @pIsLeTan = (checkIsLetan.Count > 0) ? 1 : 0,
                @pIsLead = ldcusview,
            });

            if (leadModel.keyValues != null && leadModel.keyValues.Length > 0)
            {
                foreach (var item in leadModel.keyValues)
                {
                    if (item.Key.Split(',').Length > 1 && item.Key.Split(',')[1] == "Start" && !string.IsNullOrEmpty(item.Value))
                    {
                        leaddt = leaddt.Where(x => typeof(LeadModel).GetProperty(item.Key.Split(',')[0]).GetValue(x) != null && typeof(LeadModel).GetProperty(item.Key.Split(',')[0]).GetValue(x).ToString() != "" ? DateTime.Parse(typeof(LeadModel).GetProperty(item.Key.Split(',')[0]).GetValue(x).ToString()) >= DateTime.Parse(item.Value) : false);
                    }
                    else if (item.Key.Split(',').Length > 1 && item.Key.Split(',')[1] == "End" && !string.IsNullOrEmpty(item.Value))
                    {
                        leaddt = leaddt.Where(x => typeof(LeadModel).GetProperty(item.Key.Split(',')[0]).GetValue(x) != null && typeof(LeadModel).GetProperty(item.Key.Split(',')[0]).GetValue(x).ToString() != "" ? DateTime.Parse(typeof(LeadModel).GetProperty(item.Key.Split(',')[0]).GetValue(x).ToString()) <= DateTime.Parse(item.Value) : false);
                    }
                    else if (item.ValueArr != null && item.ValueArr.Length > 0) //Helpers.Common.ChuyenThanhKhongDau(q[i].CompanyName);
                    {
                        //thanh
                        if (item.Key == "AssignedUserId")
                        {
                            leaddt = leaddt.Where(x =>
                            {
                                var itemKeyValue = typeof(LeadModel).GetProperty(item.Key).GetValue(x);
                                var createdUserIdValue = typeof(LeadModel).GetProperty("CreatedUserId").GetValue(x);

                                return (itemKeyValue != null ? item.ValueArr.Any(y => itemKeyValue.ToString().Contains(y)) : (createdUserIdValue != null ? item.ValueArr.Any(y => createdUserIdValue.ToString().Contains(y)) : false)
                                );
                            });
                        }
                        else
                        {
                            leaddt = leaddt.Where(x => typeof(LeadModel).GetProperty(item.Key).GetValue(x) != null ? item.ValueArr.Any(y => typeof(LeadModel).GetProperty(item.Key).GetValue(x).ToString().Contains(y)) : false);
                        }
                    }
                }
            }
            var props = typeof(LeadModel);
            foreach (var item in props.GetProperties())
            {
                if (item.PropertyType == typeof(String) && item.GetValue(leadModel) != null && item.GetValue(leadModel).ToString() != "")
                {
                    leaddt = leaddt.Where(x => item.GetValue(x) != null ? Helpers.Common.ChuyenThanhKhongDau(item.GetValue(x).ToString()).ToLower().Trim().Contains(Helpers.Common.ChuyenThanhKhongDau(item.GetValue(leadModel).ToString()).ToLower().Trim()) : false);
                }
                else if (item.PropertyType == typeof(Int32?) && item.GetValue(leadModel) != null)
                {
                    leaddt = leaddt.Where(x => item.GetValue(x) != null ? item.GetValue(x).ToString() == item.GetValue(leadModel).ToString() : false);
                }
            }
            //var data = LeadKanbanPartial(leaddt);
            var data = LeadKanbanPartial(leaddt, ldcusview);

            ControllerContext.RouteData.Values["action"] = "LeadKanbanPartial";
            return data;
        }


        //tableadProduct - thanh
        [HttpGet]
        public ActionResult TabLeadProduct(int id, int? isPartial)
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

            var datals = SqlHelper.QuerySP<LeadProduct>("spCrm_LeadProduct_Index", new
            {
                @pLeadId = id
            });

            var category = categoryRepository.GetAllCategories()
            .Select(item => new CategoryViewModel
            {
                Id = item.Id,
                Code = item.Code,
                Value = item.Value,
                Name = item.Name
            }).Where(x => x.Code == "Origin").ToList();
            ViewBag.category = category;
            ViewBag.BrandId = intBrandID;
            ViewBag.LeadId = id;
            ViewBag.DataList = datals;
            ViewBag.IsPartial = isPartial;
            return PartialView();
        }
        [AllowAnonymous]
        [HttpPost]
        public ActionResult CreateLeadProduct(List<LeadProductCreate> datalist, int? isPartial)
        {
            try
            {
                // Xử lý các mục trong invoiceItems
                foreach (var item in datalist)
                {
                    var result = SqlHelper.QuerySP("spCrm_LeadProduct_Create", new
                    {
                        @pLeadId = item.LeadId,
                        @pProductId = item.ProductId,
                        @pQuantity = item.Quantity,
                        @pPrice = item.Price,
                        @pTotalAmount = item.TotalAmount,
                        @pNote = item.Note,
                        @CreatedUserId = WebSecurity.CurrentUserId,
                        @pIsTang = item.IsTang

                    }).FirstOrDefault();
                }
                SqlHelper.ExecuteSP("InsertLeadLogs", new
                {
                    @LeadId = datalist.First().LeadId,
                    @Action = "OTHER",
                    @Logs = "Thêm mới sản phẩm",
                    @isImportant = 0,
                    @IdAction = -1,
                    @UserId = WebSecurity.CurrentUserId,
                    @TypeLead = isPartial != null ? 0 : 1
                });
                //var context = Microsoft.AspNet.SignalR.GlobalHost.ConnectionManager.GetHubContext<ErpHub>();
                //context.Clients.All.f5LeadLogs(datalist.First().LeadId);
                return Json(new { success = true, message = "Thành công!" });
            }
            catch
            {
                return Json(new { success = false, message = "Thất bại!" });
            }
        }
        [AllowAnonymous]
        [HttpPost]
        public ActionResult EditLeadProduct(List<LeadProductCreate> datalist, int leadId, int? isPartial)
        {
            try
            {
                // Xử lý các mục trong invoiceItems
                foreach (var item in datalist)
                {
                    var result = SqlHelper.QuerySP("spCrm_LeadProduct_Edit", new
                    {
                        @pId = item.LeadId,
                        @pQuantity = item.Quantity,
                        @pTotalAmount = item.TotalAmount,
                        @pNote = item.Note,
                        @ModifiedUserId = WebSecurity.CurrentUserId,
                        @pIsTang = item.IsTang

                    }).FirstOrDefault();
                }
                SqlHelper.ExecuteSP("InsertLeadLogs", new
                {
                    @LeadId = leadId,
                    @Action = "OTHER",
                    @Logs = "Chỉnh sửa sản phẩm",
                    @isImportant = 0,
                    @IdAction = -1,
                    @UserId = WebSecurity.CurrentUserId,
                    @TypeLead = isPartial != null ? 0 : 1
                });
                //var context = Microsoft.AspNet.SignalR.GlobalHost.ConnectionManager.GetHubContext<ErpHub>();
                //context.Clients.All.f5LeadLogs(datalist.First().LeadId);
                return Json(new { success = true, message = "Thành công!" });
            }
            catch
            {
                return Json(new { success = false, message = "Thất bại!" });
            }

        }
        [AllowAnonymous]
        [HttpPost]
        public ActionResult DeleteLeadProduct(List<int> datalist, int? isPartial)
        {
            try
            {
                int leadid = 0;
                // Xử lý các mục trong invoiceItems
                foreach (var item in datalist)
                {
                    var result = SqlHelper.QuerySP<int>("spCrm_LeadProduct_Delete", new
                    {
                        @pId = item,
                        @ModifiedUserId = WebSecurity.CurrentUserId
                    }).FirstOrDefault();
                    leadid = result;
                }
                if (leadid != 0)
                {
                    SqlHelper.ExecuteSP("InsertLeadLogs", new
                    {
                        @LeadId = leadid,
                        @Action = "OTHER",
                        @Logs = "Xóa sản phẩm",
                        @isImportant = 0,
                        @IdAction = -1,
                        @UserId = WebSecurity.CurrentUserId,
                        @TypeLead = isPartial != null ? 0 : 1
                    });
                    var context = Microsoft.AspNet.SignalR.GlobalHost.ConnectionManager.GetHubContext<ErpHub>();
                    context.Clients.All.f5LeadLogs(leadid);
                }
                return Json(new { success = true, message = "Thành công!" });
            }
            catch
            {
                return Json(new { success = false, message = "Thất bại!" });
            }
        }
        //tableadQuota
        [HttpGet]
        public ActionResult TabLeadQuotation(int id, int? isPartial)
        {

            var datals = SqlHelper.QuerySP<LeadQuotationIndex>("spCrm_LeadQuotation_Index", new
            {
                @pLeadId = id
            });
            ViewBag.IsPartial = isPartial;
            ViewBag.DataList = datals;
            ViewBag.LeadId = id;

            return PartialView();
        }
        public ViewResult TabLeadQuotationAddNew(int id, int? isPartial)
        {
            var model = SqlHelper.QuerySP<LeadQuotationPopupIndex>("sp_Crm_Lead_getTT_byId", new
            {
                @pId = id,
                @isPartial = isPartial
            }).FirstOrDefault();
            var datals = SqlHelper.QuerySP<LeadProduct>("spCrm_LeadProduct_Index", new
            {
                @pLeadId = id
            });
            var categorySt = categoryRepository.GetAllCategories()
            .Select(item => new CategoryViewModel
            {
                Id = item.Id,
                Code = item.Code,
                Value = item.Value,
                Name = item.Name
            }).Where(x => x.Code == "TINHTRANGBAOGIA").ToList();

            var categoryOr = categoryRepository.GetAllCategories()
            .Select(item => new CategoryViewModel
            {
                Id = item.Id,
                Code = item.Code,
                Value = item.Value,
                Name = item.Name
            }).Where(x => x.Code == "Origin").ToList();
            var user = userRepository;


            ViewBag.user = user;
            ViewBag.categorySt = categorySt;
            ViewBag.categoryOr = categoryOr;
            ViewBag.LeadId = id;
            ViewBag.IsPartial = isPartial;
            ViewBag.DataList = datals;
            ViewBag.DataModel = model;
            return View();
        }
        [AllowAnonymous]
        [HttpPost]
        public ActionResult CreateLeadQuotation(LeadQuotationPopupCreate datalist, int? isPartial)
        {
            try
            {
                var result = SqlHelper.QuerySP<int>("spCrm_LeadQuotation_Create", new
                {
                    @pLeadId = datalist.LeadId,
                    @pCode = datalist.Code,
                    @pTotalAmount = datalist.TotalAmount,
                    @pTaxCode = datalist.TaxCode,
                    @pStatus = datalist.Status,
                    @pAddress = datalist.Address,
                    @pReceptionStaffId = datalist.ReceptionStaffId,
                    @pUntilDateValue = datalist.UntilDate,
                    @CreatedUserId = WebSecurity.CurrentUserId

                }).FirstOrDefault();
                if (result != 0)
                {
                    foreach (var item in datalist.LeadProductLQ)
                    {
                        var results = SqlHelper.QuerySP("spCrm_LeadQuotationDetail_Create", new
                        {
                            @pQuotationId = result,
                            @pProductId = item.ProductId,
                            @pPrice = item.Price,
                            @pQuantity = item.Quantity,
                            @pis_TANG = item.IsTang != true ? 0 : 1,
                            @pNote = item.Note,
                            @CreatedUserId = WebSecurity.CurrentUserId
                        }).FirstOrDefault();
                    }

                    SqlHelper.ExecuteSP("InsertLeadLogs", new
                    {
                        @LeadId = datalist.LeadId,
                        @Action = "OTHER",
                        @Logs = "Thêm mới báo giá",
                        @isImportant = 0,
                        @IdAction = -1,
                        @UserId = WebSecurity.CurrentUserId,
                        @TypeLead = isPartial != null ? 0 : 1
                    });
                    var context = Microsoft.AspNet.SignalR.GlobalHost.ConnectionManager.GetHubContext<ErpHub>();
                    context.Clients.All.f5LeadLogs(datalist.LeadId);
                    return Json(new { success = true, data = result, message = "Thành công!" });
                }
                return Json(new { success = false, data = "", message = "Thất bại!" });
            }
            catch
            {
                return Json(new { success = false, data = "", message = "Thất bại!" });
            }
        }
        public ViewResult TabLeadQuotationDetail(int id, int? isPartial)
        {
            var model = SqlHelper.QuerySP<LeadQuotationPopupEdit>("spCrm_LeadQuotation_SELECT", new
            {
                @pId = id,
                @isPartial = isPartial
            }).FirstOrDefault();
            var modeldt = SqlHelper.QuerySP<LeadProductInQuotationEdit>("spCrm_LeadQuotation_SELECT_Detail", new
            {
                @pQuotationId = model.Id
            }).ToArray();
            model.LeadProductLQ = modeldt;
            var categorySt = categoryRepository.GetAllCategories()
            .Select(item => new CategoryViewModel
            {
                Id = item.Id,
                Code = item.Code,
                Value = item.Value,
                Name = item.Name
            }).Where(x => x.Code == "TINHTRANGBAOGIA").ToList();

            var categoryOr = categoryRepository.GetAllCategories()
            .Select(item => new CategoryViewModel
            {
                Id = item.Id,
                Code = item.Code,
                Value = item.Value,
                Name = item.Name
            }).Where(x => x.Code == "Origin").ToList();
            var user = userRepository;
            var dataFiled = SqlHelper.QuerySP<CrmTemplateFileViewModel>("spCrm_TemplateExcel_Index").ToList();
            dataFiled = dataFiled.Where(x => x.Module == "BAOGIA").ToList();

            ViewBag.DataSel = dataFiled;

            ViewBag.user = user;
            ViewBag.categorySt = categorySt;
            ViewBag.categoryOr = categoryOr;
            ViewBag.LeadId = model.LeadId;
            ViewBag.DataModel = model;
            ViewBag.IsPartial = isPartial;
            return View();
        }
        [AllowAnonymous]
        [HttpPost]
        public ActionResult EditLeadQuotationDetail(LeadQuotationPopupEditCr datalist, int? isPartial)
        {
            try
            {
                var result = SqlHelper.QuerySP<int>("spCrm_LeadQuotation_Edit", new
                {
                    @pId = datalist.data.Id,
                    @pTotalAmount = datalist.data.TotalAmount,
                    @pTaxCode = datalist.data.TaxCode,
                    @pStatus = datalist.data.Status,
                    @pAddress = datalist.data.Address,
                    @pReceptionStaffId = datalist.data.ReceptionStaffId,
                    @pUntilDateValue = datalist.data.UntilDateValue,
                    @pModifiedUserId = WebSecurity.CurrentUserId

                }).FirstOrDefault();
                if (datalist.LeadProductNewLQ != null)
                {
                    foreach (var item in datalist.LeadProductNewLQ)
                    {
                        var results = SqlHelper.QuerySP("spCrm_LeadQuotationDetail_Create", new
                        {
                            @pQuotationId = datalist.data.Id,
                            @pProductId = item.ProductId,
                            @pPrice = item.Price,
                            @pQuantity = item.Quantity,
                            @pis_TANG = item.IsTang != true ? 0 : 1,
                            @pNote = item.Note,
                            @CreatedUserId = WebSecurity.CurrentUserId
                        }).FirstOrDefault();
                    }
                }
                foreach (var item in datalist.data.LeadProductLQ)
                {
                    var results = SqlHelper.QuerySP("spCrm_LeadQuotationDetail_Edit", new
                    {
                        @pId = item.Id,
                        @pQuotationId = datalist.data.Id,
                        @pProductId = item.ProductId,
                        @pPrice = item.Price,
                        @pQuantity = item.Quantity,
                        @pis_TANG = item.is_TANG != true ? 0 : 1,
                        @pNote = item.Note,
                        @pModifiedUserId = WebSecurity.CurrentUserId
                    }).FirstOrDefault();
                }
                SqlHelper.ExecuteSP("InsertLeadLogs", new
                {
                    @LeadId = datalist.data.LeadId,
                    @Action = "OTHER",
                    @Logs = "Chỉnh sửa báo giá",
                    @isImportant = 0,
                    @IdAction = -1,
                    @UserId = WebSecurity.CurrentUserId,
                    @TypeLead = isPartial != null ? 0 : 1
                });
                //var context = Microsoft.AspNet.SignalR.GlobalHost.ConnectionManager.GetHubContext<ErpHub>();
                //context.Clients.All.f5LeadLogs(datalt.data.LeadId);
                return Json(new { success = true, message = "Thành công!" });
            }
            catch
            {
                return Json(new { success = false, message = "Thất bại!" });
            }
        }
        [AllowAnonymous]
        [HttpPost]
        public ActionResult CheckCodeQuotation(string codec)
        {
            var results = SqlHelper.QuerySP("spCrm_LeadQuotation_CheckCode", new
            {
                @pCode = codec
            }).FirstOrDefault();
            if (results != null)
            {
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }
        [AllowAnonymous]
        [HttpPost]
        public ActionResult DeleteLeadQuotationDetail(int Id, int? isPartial)
        {
            try
            {
                var result = SqlHelper.QuerySP("spCrm_LeadQuotationDetail_Delete", new
                {
                    @pId = Id,
                    @ModifiedUserId = WebSecurity.CurrentUserId
                }).FirstOrDefault();
                return Json(new { success = true, message = "Thành công!" });
            }
            catch
            {
                return Json(new { success = false, message = "Thất bại!" });
            }
        }
        //tab LeadHistory

        [AllowAnonymous]
        [HttpGet]
        public ActionResult TabLeadHistory(int Id, int? isPartial)
        {
            var data = SqlHelper.QuerySP<LeadLogsModel>("GetLeadLogsAllById", new { @pLeadId = Id, @isPartial = isPartial });
            List<ItemLeadLog> ItemLeadLogList = new List<ItemLeadLog>
            {
                new ItemLeadLog { Key = "MEETING", Value = "Tạo cuộc họp" },
                new ItemLeadLog { Key = "CALL", Value = "Tạo cuộc gọi đi" },
                new ItemLeadLog { Key = "TASK", Value = "Tạo nhiệm vụ" },
                new ItemLeadLog { Key = "CHANGESTATUS", Value = "Trạng thái đã thay đổi" },
                new ItemLeadLog { Key = "LEADCREATE", Value = "Tạo mới Lead" },
                new ItemLeadLog { Key = "LEADCREATEFACEBOOK", Value = "Tạo mới Lead từ Facebook" },
                new ItemLeadLog { Key = "LEADCREATEZALO", Value = "Tạo mới Lead từ Zalo" },
                new ItemLeadLog { Key = "COMMENT", Value = "Bình luận" },
                new ItemLeadLog { Key = "SENDSMS", Value = "Gửi SMS" },
                new ItemLeadLog { Key = "SENDEMAIL", Value = "Gửi Email" },
                new ItemLeadLog { Key = "SENDZALO", Value = "Gửi Zalo" },
                new ItemLeadLog { Key = "PREVIEWSMS", Value = "Xem trước nội dung gửi SMS" },
                new ItemLeadLog { Key = "PREVIEWZL", Value = "Xem trước nội dung gửi Zalo" },
                new ItemLeadLog { Key = "OTHER", Value = "Hành động khác" }
            };
            var users = userRepository;
            ViewBag.Users = users;
            ViewBag.SortedList = data.OrderByDescending(x => x.CreatedDate).ToList();
            ViewBag.status = SqlHelper.QuerySP<StatusLeadModel>("GetAllStatus");
            ViewBag.ItemLeadLog = ItemLeadLogList;
            return PartialView();
        }
        #endregion
        #region SendSMS - Thanh
        [HttpGet]
        public ActionResult GetSMSList(int id, bool typeLead)
        {
            //var checkIsLetan = SqlHelper.QuerySP<int>("checkUserIsLetan", new
            //{
            //	@pId = WebSecurity.CurrentUserId
            //}).ToList();
            //var datalead = SqlHelper.QuerySP<LeadModel>("LoadLeadIndex", new
            //{
            //	@pId = WebSecurity.CurrentUserId,
            //	@pIsLeTan = (checkIsLetan.Count > 0) ? 1 : 0
            //});
            var data = SqlHelper.QuerySP<Sale_GetList_TemplateSMS>("spCrm_Template_SelectTemplateSMS", new
            {
                @pTypeLead = typeLead
            }).ToList();
            if (id != 0)
            {
                //var lead = datalead.Where(x => x.Id == id).FirstOrDefault();

                // Tìm và thay thế các giá trị trong danh sách data
                List<string> replacedData = new List<string>();
                foreach (var item in data)
                {
                    string sentence = item.ContentRule;

                    // Trích xuất các giá trị trong cặp ngoặc nhọn {{}}
                    List<string> extractedValues = ExtractValuesFromBrackets(sentence);
                    var SystemPara = SqlHelper.QuerySP<SystemParametersViewModel>("spCrm_SystemParameters").ToList();

                    foreach (var ite in extractedValues)
                    {
                        var storedprocedure = SystemPara.FirstOrDefault(x => x.Name == ite)?.Storedprocedure;
                        var fiedName = SystemPara.FirstOrDefault(x => x.Name == ite)?.FiedName;
                        if (storedprocedure != null)
                        {

                            if (typeLead == true)
                            {
                                var result = SqlHelper.QuerySP(storedprocedure, new
                                {
                                    @pColumnName = fiedName,
                                    @pLeadId = id
                                });
                                // Thay thế giá trị result vào sentence
                                if (result != null && result.Count() > 0)
                                {
                                    sentence = sentence.Replace($"{{{{{ite}}}}}", result.First().ValueByName);
                                }
                            }
                            else
                            {
                                var result = SqlHelper.QuerySP(storedprocedure, new
                                {
                                    @pColumnName = fiedName,
                                    @pCustomerId = id
                                });
                                // Thay thế giá trị result vào sentence
                                if (result != null && result.Count() > 0)
                                {
                                    sentence = sentence.Replace($"{{{{{ite}}}}}", result.First().ValueByName);
                                }
                            }
                        }
                    }
                    item.ContentRule = sentence;
                }
            }

            return Json(data, JsonRequestBehavior.AllowGet);
        }
        // Hàm trích xuất giá trị từ cặp ngoặc nhọn {{}}
        private List<string> ExtractValuesFromBrackets(string sentence)
        {
            List<string> values = new List<string>();
            var regex = new Regex(@"{{(.*?)}}");
            var matches = regex.Matches(sentence);
            foreach (Match match in matches)
            {
                string value = match.Value.Trim(new char[] { '{', '}' });
                values.Add(value);
            }
            return values;
        }
        //Hàm thay đổi thông tin trong list sms
        private IEnumerable<RuleModel> ReplaceCtSMS(IEnumerable<RuleModel> rule, int Id, bool? typeLead = false)
        {
            List<string> replacedData = new List<string>();
            foreach (var rulee in rule)
            {
                if (rulee.ContentRule != null)
                {
                    List<string> extractedValues = ExtractValuesFromBrackets(rulee.ContentRule);
                    var SystemPara = SqlHelper.QuerySP<SystemParametersViewModel>("spCrm_SystemParameters").ToList();

                    foreach (var ite in extractedValues)
                    {
                        var storedprocedure = SystemPara.FirstOrDefault(x => x.Name == ite)?.Storedprocedure;
                        var fiedName = SystemPara.FirstOrDefault(x => x.Name == ite)?.FiedName;
                        if (storedprocedure != null)
                        {
                            if (typeLead == true)
                            {
                                var result = SqlHelper.QuerySP(storedprocedure, new
                                {
                                    @pColumnName = fiedName,
                                    @pLeadId = Id
                                });
                                // Thay thế giá trị result vào sentence
                                rulee.ContentRule = rulee.ContentRule.Replace($"{{{{{ite}}}}}", result.First().ValueByName);
                            }
                            else
                            {
                                var result = SqlHelper.QuerySP(storedprocedure, new
                                {
                                    @pColumnName = fiedName,
                                    @pCustomerId = Id
                                });
                                // Thay thế giá trị result vào sentence
                                rulee.ContentRule = rulee.ContentRule.Replace($"{{{{{ite}}}}}", result.First().ValueByName);
                            }
                        }
                    }
                }

            }
            return rule;
        }
        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult> SendApiSMS(string contentSMS, string phone)
        {

            string requestlink = ConfigurationManager.AppSettings["SendSMS:Link"];
            string secretKey = ConfigurationManager.AppSettings["SendSMS:SecretKey"];
            string apiKey = ConfigurationManager.AppSettings["SendSMS:ApiKey"];
            string brandName = ConfigurationManager.AppSettings["SendSMS:Brandname"];
            string encodedContent = HttpUtility.UrlEncode(contentSMS);
            string requestUrl = requestlink + "&ApiKey={ApiKey}&SecretKey={SecretKey}&Brandname={Brandname}&Content={Content}&Phone={Phone}";
            requestUrl = requestUrl.Replace("{ApiKey}", apiKey)
                                   .Replace("{SecretKey}", secretKey)
                                   .Replace("{Brandname}", brandName)
                                   .Replace("{Content}", encodedContent)
                                   .Replace("{Phone}", phone);

            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(requestUrl);

                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    dynamic jsonObject = JsonConvert.DeserializeObject(responseBody);
                    string codeResult = jsonObject.CodeResult;
                    return Json(new { Success = true, CodeResult = codeResult });
                }
                else
                {
                    return Json(new { Success = false, CodeResult = "99" });
                }
            }
        }
        [AllowAnonymous]
        [HttpPost]
        public ActionResult CreateSMSLeadLogs(long Id, string Content, int Status, string Mobile, string codeResult, int? isPartial = null)
        {
            var result = SqlHelper.ExecuteSP("spSale_LeadLogs_Create_SendSMS_UpdateErr", new
            {
                @pLeadId = Id,
                @pAction = "SENDSMS",
                @pLogs = Content,
                @pStatus = Status,
                @pMobile = Mobile,
                @CreatedUserId = WebSecurity.CurrentUserId,
                @pTypeLead = isPartial == 1 ? 0 : 1,
                @pCampaignId = 0,
                @pcodeResult = codeResult
            });
            var context = Microsoft.AspNet.SignalR.GlobalHost.ConnectionManager.GetHubContext<ErpHub>();
            context.Clients.All.f5LeadLogs(Id);
            if (result != 0)
            {
                return Json(new { Success = true });
            }
            return Json(new { Success = false });
        }
        public void getLeadByMobile(string CID)
        {
            var result = SqlHelper.QuerySP<LeadByMobileModel>("sp_Crm_LeadByMobile_Get", new
            {
                @pMobile = CID
            }).FirstOrDefault();

            var curentUId = WebSecurity.CurrentUserId;
            string clientIdUser;
            string message = result.Name + ',' + result.Mobile;
            var hubContext = GlobalHost.ConnectionManager.GetHubContext<CallInComingHub>();
            var rsClientId = SqlHelper.QuerySP<string>("sp_Crm_ClientIdByUserId_GET", new
            {
                pUserId = result.AssignedUserId
            }).FirstOrDefault();
            if (rsClientId != null)
            {
                clientIdUser = rsClientId;
                hubContext.Clients.Client(clientIdUser).CallInComingNotificationToUser(message, result.Id);
            }
            else
            {
                var rssClientId = SqlHelper.QuerySP<string>("sp_Crm_ClientIdByUserId_GET", new
                {
                    pUserId = result.CreatedUserId
                }).FirstOrDefault();
                if (rssClientId != null)
                {
                    clientIdUser = rsClientId;
                    hubContext.Clients.Client(clientIdUser).CallInComingNotificationToUser(message, result.Id);
                }
            }
        }

        #endregion

        #region Tuấn Anh - ZNS Zalo
        [HttpGet]
        public ActionResult GetZNSList(int id, bool typeLead)
        {
            //var checkIsLetan = SqlHelper.QuerySP<int>("checkUserIsLetan", new
            //{
            //	@pId = WebSecurity.CurrentUserId
            //}).ToList();
            //var datalead = SqlHelper.QuerySP<LeadModel>("LoadLeadIndex", new
            //{
            //	@pId = WebSecurity.CurrentUserId,
            //	@pIsLeTan = (checkIsLetan.Count > 0) ? 1 : 0
            //});
            //var lead = datalead.FirstOrDefault(x => x.Id == id);
            var data = SqlHelper.QuerySP<Sale_GetList_TemplateSMS>("spCrm_Template_SelectTemplateZNSZalo", new
            {
                @pTypeLead = typeLead
            }).ToList();

            // Tạo danh sách lưu trữ lỗi
            List<string> errors = new List<string>();
            List<string> realValues = new List<string>();

            // Tìm và thay thế các giá trị trong danh sách data
            foreach (var item in data)
            {
                string sentence = item.ContentRule;

                // Trích xuất các giá trị trong cặp ngoặc nhọn {{}}
                List<string> extractedValues = ExtractValuesFromBrackets(sentence);
                var SystemPara = SqlHelper.QuerySP<SystemParametersViewModel>("spCrm_SystemParameters").ToList();

                // Tạo danh sách lưu trữ lỗi cho mỗi option
                List<string> optionErrors = new List<string>();
                List<string> optionRealValue = new List<string>();

                foreach (var ite in extractedValues)
                {
                    var storedprocedure = SystemPara.FirstOrDefault(x => x.Name == ite)?.Storedprocedure;
                    var fiedName = SystemPara.FirstOrDefault(x => x.Name == ite)?.FiedName;

                    if (storedprocedure != null)
                    {
                        if (typeLead == true)
                        {
                            var result = SqlHelper.QuerySP(storedprocedure, new
                            {
                                @pColumnName = fiedName,
                                @pLeadId = id
                            });

                            var value = result.FirstOrDefault()?.ValueByName;
                            // Thay thế giá trị vào ContentRule
                            sentence = sentence.Replace($"{{{{{ite}}}}}", value);
                            optionRealValue.Add(value);

                            // Kiểm tra xem giá trị có rỗng không
                            if (string.IsNullOrWhiteSpace(value))
                            {
                                optionErrors.Add($"Thiếu thông tin của '{ite}'.");
                            }
                        }
                        else
                        {
                            var result = SqlHelper.QuerySP(storedprocedure, new
                            {
                                @pColumnName = fiedName,
                                @pCustomerId = id
                            });

                            var value = result.FirstOrDefault()?.ValueByName;
                            // Thay thế giá trị vào ContentRule
                            sentence = sentence.Replace($"{{{{{ite}}}}}", value);
                            optionRealValue.Add(value);

                            // Kiểm tra xem giá trị có rỗng không
                            if (string.IsNullOrWhiteSpace(value))
                            {
                                optionErrors.Add($"Thiếu thông tin của '{ite}'.");
                            }
                        }
                    }
                }
                item.ContentRule = sentence;
                if (optionRealValue.Any())
                {
                    item.realValue = string.Join(",", optionRealValue);
                    realValues.AddRange(optionRealValue);
                }
                // Gắn thông báo lỗi vào dữ liệu tương ứng nếu có lỗi
                if (optionErrors.Any())
                {

                    item.error = string.Join("<br>", optionErrors);
                    errors.AddRange(optionErrors);
                }
            }

            // Nếu có lỗi, trả về danh sách lỗi kèm theo dữ liệu
            if (errors.Any())
            {
                return Json(new { errors = errors, data = data, realValues = realValues }, JsonRequestBehavior.AllowGet);
            }

            // Nếu không có lỗi, trả về dữ liệu
            return Json(new { data = data, realValues = realValues }, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public ActionResult GetZNSTemplate(int id)
        {
            var checkIsLetan = SqlHelper.QuerySP<int>("checkUserIsLetan", new
            {
                @pId = WebSecurity.CurrentUserId
            }).ToList();
            var datalead = SqlHelper.QuerySP<LeadModel>("LoadLeadIndex", new
            {
                @pId = WebSecurity.CurrentUserId,
                @pIsLeTan = (checkIsLetan.Count > 0) ? 1 : 0
            });
            var data = SqlHelper.QuerySP<Sale_GetList_TemplateSMS>("spCrm_Template_SelectTemplateZNSZaloById", new { Id = id }).ToList();

            // Tạo danh sách lưu trữ fieldZns
            List<string> fieldZnsList = new List<string>();

            // Tìm và thay thế các giá trị trong danh sách data
            foreach (var item in data)
            {
                string sentence = item.ContentRule;

                // Trích xuất các giá trị trong cặp ngoặc nhọn {{}}
                List<string> extractedValues = ExtractValuesFromBrackets(sentence);
                var SystemPara = SqlHelper.QuerySP<SystemParametersViewModel>("spCrm_SystemParameters").ToList();

                foreach (var ite in extractedValues)
                {
                    var storedprocedure = SystemPara.FirstOrDefault(x => x.Name == ite)?.Storedprocedure;
                    var fieldZns = SystemPara.FirstOrDefault(x => x.Name == ite)?.FieldZns;

                    // Thay thế giá trị fieldZns vào sentence
                    sentence = sentence.Replace($"{{{{{ite}}}}}", fieldZns);
                    fieldZnsList.Add(fieldZns); // Thêm fieldZns vào danh sách

                }
                item.ContentRule = sentence;
            }

            // Tạo chuỗi danh sách fieldZns được nối bởi dấu phẩy
            string fieldZnsString = string.Join(",", fieldZnsList);

            return Json(new { data = data, fieldZnsList = fieldZnsString }, JsonRequestBehavior.AllowGet);
        }




        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult> SendZNS(string phoneNumber, string templateId, Dictionary<string, object> templateData)
        {
            try
            {
                // Lấy access token mới nhất
                var accessTokenData = SqlHelper.QuerySP<dynamic>("sp_Crm_GetLatestAccessToken").FirstOrDefault();
                if (accessTokenData == null)
                {
                    // Trả về mã lỗi và thông báo lỗi cho client
                    return Content("{\"error\": 1, \"message\": \"Chưa kết nối với Zalo OA.\"}", "application/json");
                }

                // URL endpoint
                string url = "https://business.openapi.zalo.me/message/template";

                // Dữ liệu cần gửi
                var requestData = new
                {
                    phone = phoneNumber,
                    template_id = templateId,
                    template_data = templateData,
                };

                // Chuyển đổi dữ liệu thành chuỗi JSON
                var jsonRequestData = JsonConvert.SerializeObject(requestData);

                // Tạo đối tượng HttpClient để gửi request
                using (HttpClient client = new HttpClient())
                {
                    // Thêm header Content-Type
                    client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json");

                    // Thêm header Access Token
                    client.DefaultRequestHeaders.TryAddWithoutValidation("access_token", accessTokenData.Token);

                    // Gửi request POST
                    var response = await client.PostAsync(url, new StringContent(jsonRequestData, Encoding.UTF8, "application/json"));

                    // Đọc và trả về response từ API
                    string responseContent = await response.Content.ReadAsStringAsync();

                    if (response.IsSuccessStatusCode)
                    {
                        // Trả về kết quả thành công từ API cho client
                        JObject responseObject = JObject.Parse(responseContent);
                        int errorCode = responseObject.Value<int>("error");
                        if (errorCode != 0)
                        {
                            return Content($"{{ \"success\": false, \"error\": {errorCode}, \"message\": \"{responseObject.Value<string>("message")}\" }}", "application/json");
                        }
                        else
                        {
                            return Content(responseContent, "application/json");
                        }
                    }
                    else
                    {
                        // Lỗi không thành công khi gửi request
                        return Content("{\"error\": -2, \"message\": \"Gửi ZNS thất bại.\"}", "application/json");
                    }
                }
            }
            catch (Exception ex)
            {
                // Xử lý lỗi nếu có
                return Content($"{{ \"error\": \"{ex.Message}\" }}", "application/json");
            }
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult ScheduleZNS(string phoneNumber, string templateId, Dictionary<string, object> templateData, DateTime scheduleTime)
        {
            try
            {
                BackgroundJob.Schedule(() => SendZNS(phoneNumber, templateId, templateData), scheduleTime);

                // Trả về thành công
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                // Xử lý lỗi nếu có
                return Json(new { success = false, error = ex.Message });
            }
        }



        [AllowAnonymous]
        [HttpPost]
        public ActionResult CreateZNSLeadLogs(string Id, string Content, int Status, string Mobile, int CampaignId, string ErrorCode, int isPartial)
        {
            var result = SqlHelper.ExecuteSP("spSale_LeadLogs_Create_SendSMS_UpdateErr", new
            {
                @pLeadId = Id,
                @pAction = "SENDZALO",
                @pLogs = Content,
                @pStatus = Status,
                @pMobile = Mobile,
                @CreatedUserId = WebSecurity.CurrentUserId,
                @pTypeLead = isPartial,
                @pCampaignId = CampaignId,
                @pcodeResult = ErrorCode
            }); ;
            var context = Microsoft.AspNet.SignalR.GlobalHost.ConnectionManager.GetHubContext<ErpHub>();
            context.Clients.All.f5LeadLogs(Id);
            return Json(new
            {
                success = true,
            });
        }

        [AllowAnonymous]
        [HttpGet]
        public ActionResult GetZNSListHangfire()
        {
            var znsMessages = SqlHelper.QuerySP<Sale_GetList_TemplateSMS>("spCrm_Template_SelectTemplateZNSZalo", new
            {
                @pTypeLead = 1
            }).ToList();
            var campaign = SqlHelper.QuerySP<AdviseCardCampaignViewModel>("spCrm_Campaign_Select").ToList();
            var data = new
            {
                Campaign = campaign,
                znsMessages = znsMessages,
            };
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public ActionResult GetZNSListForMultipleUsers(List<int> userIds, string znsContent, bool? typeLead = true)
        {
            Dictionary<string, List<Sale_GetList_TemplateSMS>> userData = new Dictionary<string, List<Sale_GetList_TemplateSMS>>();
            Dictionary<string, List<string>> userRealValues = new Dictionary<string, List<string>>();

            foreach (int userId in userIds)
            {
                var leadName = SqlHelper.QuerySP<string>("sp_Crm_GetLeadNameById", new
                {
                    @LeadId = userId
                }).FirstOrDefault();
                var leadPhone = SqlHelper.QuerySP<string>("sp_Crm_GetMobileByIdLead", new
                {
                    @LeadId = userId
                }).FirstOrDefault();

                List<string> realValues = new List<string>();

                // Thay thế giá trị vào mẫu template
                string sentence = znsContent;
                List<string> extractedValues = ExtractValuesFromBrackets(sentence);
                var SystemPara = SqlHelper.QuerySP<SystemParametersViewModel>("spCrm_SystemParameters").ToList();

                foreach (var ite in extractedValues)
                {
                    var storedprocedure = SystemPara.FirstOrDefault(x => x.Name == ite)?.Storedprocedure;
                    var fiedName = SystemPara.FirstOrDefault(x => x.Name == ite)?.FiedName;

                    if (storedprocedure != null)
                    {
                        if (typeLead == true)
                        {
                            var result = SqlHelper.QuerySP(storedprocedure, new
                            {
                                @pColumnName = fiedName,
                                @pLeadId = userId
                            });

                            var value = result.FirstOrDefault()?.ValueByName;
                            sentence = sentence.Replace($"{{{{{ite}}}}}", value);
                            realValues.Add(value);
                            if (string.IsNullOrWhiteSpace(value))
                            {
                                // Nếu giá trị trống, không thêm vào danh sách realValues
                                continue;
                            }
                        }
                        else
                        {
                            var result = SqlHelper.QuerySP(storedprocedure, new
                            {
                                @pColumnName = fiedName,
                                @pCustomerId = userId
                            });

                            var value = result.FirstOrDefault()?.ValueByName;
                            sentence = sentence.Replace($"{{{{{ite}}}}}", value);
                            realValues.Add(value);
                            if (string.IsNullOrWhiteSpace(value))
                            {
                                // Nếu giá trị trống, không thêm vào danh sách realValues
                                continue;
                            }
                        }
                    }
                }

                // Thêm thông tin của người dùng và mẫu template vào userData
                var userTemplates = new List<Sale_GetList_TemplateSMS>();
                // Tạo một mẫu template mới và gán giá trị đã thay thế
                var userTemplate = new Sale_GetList_TemplateSMS { ContentRule = sentence, LeadPhone = leadPhone, LeadName = leadName };
                userTemplates.Add(userTemplate);

                userData.Add(userId.ToString(), userTemplates);
                userRealValues.Add(userId.ToString(), realValues);
            }

            // Trả về thông tin user và danh sách mẫu đã thay thế đầy đủ thông tin
            return Json(new { success = true, userData = userData, userRealValues = userRealValues }, JsonRequestBehavior.AllowGet);
        }



        #endregion
        #region Khang - Send Email
        [AllowAnonymous]
        [HttpPost]
        public ActionResult SendEmail(AdviseCardSendEmailViewModel model, int Id, int? isPartial = null)
        {
            if (ModelState.IsValid)
            {
                string port = WebConfigurationManager.AppSettings["Port"];
                string ssl = WebConfigurationManager.AppSettings["SSL"];
                string smtp = WebConfigurationManager.AppSettings["SMTP"];
                string smtpUserName = WebConfigurationManager.AppSettings["SMTPUserName"];
                string smtpPassword = WebConfigurationManager.AppSettings["SMTPPassword"];
                bool status = SendMailNotGmail.SendEmail(port, ssl, smtp, model.Sender, smtpUserName, smtpPassword, model.Receiver, model.Title, model.Body);
                var result = SqlHelper.ExecuteSP("spSale_LeadLogs_Create_SendSMS", new
                {
                    @pLeadId = Id,
                    @pAction = "SENDEMAIL",
                    @pLogs = model.Body,
                    @pStatus = status ? 1 : 0,
                    @pMobile = model.Receiver,
                    @CreatedUserId = WebSecurity.CurrentUserId,
                    @pTypeLead = isPartial == 1 ? 0 : 1
                });
                var context = Microsoft.AspNet.SignalR.GlobalHost.ConnectionManager.GetHubContext<ErpHub>();
                context.Clients.All.f5LeadLogs(Id);
                return Json(new
                {
                    success = true,
                });
            }
            return Json(new
            {
                success = false,
                errors = ModelState.Values.SelectMany(x => x.Errors)
                                          .Select(x => x.ErrorMessage)
            });
        }

        #region Tuấn Anh - SendEmail với File
        [AllowAnonymous]
        [HttpPost]
        public ActionResult SendEmailWithFiles(AdviseCardSendEmailViewModel model, int Id, int? isPartial = null)
        {
            if (ModelState.IsValid)
            {
                string port = WebConfigurationManager.AppSettings["Port"];
                string smtpUserName = WebConfigurationManager.AppSettings["SMTPUserName"];
                string smtpPassword = WebConfigurationManager.AppSettings["SMTPPassword"];
                string smtpHost = WebConfigurationManager.AppSettings["SMTP"];
                string enableSSL = WebConfigurationManager.AppSettings["SSL"];
                bool status = false;

                try
                {
                    List<(string fileName, string filePath)> attachments = new List<(string fileName, string filePath)>();

                    foreach (string fileKey in Request.Files)
                    {
                        HttpPostedFileBase file = Request.Files[fileKey];
                        var originalFileName = Path.GetFileName(file.FileName);
                        var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(originalFileName);
                        var fileExtension = Path.GetExtension(originalFileName);

                        var tempDirectory = Server.MapPath("~/UploadedFile");

                        // Kiểm tra và tạo thư mục nếu cần
                        if (!Directory.Exists(tempDirectory))
                        {
                            Directory.CreateDirectory(tempDirectory);
                        }

                        // Tạo tên file duy nhất bằng cách kết hợp tên file gốc với một timestamp
                        var uniqueFileName = $"{fileNameWithoutExtension}_{DateTime.Now.Ticks}{fileExtension}";

                        var tempPath = Path.Combine(tempDirectory, uniqueFileName);
                        file.SaveAs(tempPath);
                        attachments.Add((originalFileName, tempPath));
                    }

                    model.Body = HttpUtility.UrlDecode(model.Body);

                    // Gọi hàm SendEmailAndFiles để gửi email với file đính kèm
                    status = SendMailNotGmail.SendEmailAndFiles(port, enableSSL, smtpHost, model.Sender, smtpUserName, smtpPassword,
                                                                   model.Receiver, model.Title, model.Body, attachments);


                    if (status == true)
                    {
                        var result = SqlHelper.ExecuteSP("spSale_LeadLogs_Create_SendSMS", new
                        {
                            @pLeadId = Id,
                            @pAction = "SENDEMAIL",
                            @pLogs = model.Body,
                            @pStatus = status ? 1 : 0,
                            @pMobile = model.Receiver,
                            @CreatedUserId = WebSecurity.CurrentUserId,
                            @pTypeLead = isPartial == 1 ? 0 : 1
                        });

                        var context = Microsoft.AspNet.SignalR.GlobalHost.ConnectionManager.GetHubContext<ErpHub>();
                        context.Clients.All.f5LeadLogs(Id);


                        return Json(new
                        {
                            success = true,
                        });
                    }
                    else
                    {
                        return Json(new
                        {
                            success = false,
                            errors = "Error sending email"
                        });
                    }
                }
                catch (Exception ex)
                {
                    return Json(new
                    {
                        success = false,
                        errors = "Error sending email: " + ex.Message
                    });
                }
            }

            return Json(new
            {
                success = false,
                errors = ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage)
            });
        }



        #endregion


        [HttpGet]
        public ActionResult ShowEmailModal(int id, bool typeLead)
        {
            string emailSender = WebConfigurationManager.AppSettings["EmailSender"];
            if (emailSender == null)
            {
                emailSender = SqlHelper.QuerySP<string>("spSystem_User_SelectEmailById", new
                {
                    pId = WebSecurity.CurrentUserId
                }).FirstOrDefault();
            }
            var emailTemplate = SqlHelper.QuerySP<AdviseCardEmailTemplateViewModel>("spCrm_Template_SelectAllEmail", new
            {
                @pTypeLead = typeLead
            }).ToList();
            var campaign = SqlHelper.QuerySP<AdviseCardCampaignViewModel>("spCrm_Campaign_Select").ToList();
            var emailFooter = SqlHelper.QuerySP<CrmEmailFooterViewModel>("spCrm_EmailFooter_Select", new
            {
                @pCreatedUserId = WebSecurity.CurrentUserId
            }).ToList();

            //var checkIsLetan = SqlHelper.QuerySP<int>("checkUserIsLetan", new
            //{
            //	@pId = WebSecurity.CurrentUserId
            //}).ToList();
            //var datalead = SqlHelper.QuerySP<LeadModel>("LoadLeadIndex", new
            //{
            //	@pId = WebSecurity.CurrentUserId,
            //	@pIsLeTan = (checkIsLetan.Count > 0) ? 1 : 0
            //});

            if (id != 0)
            {
                //var lead = datalead.Where(x => x.Id == id).FirstOrDefault();

                // Tìm và thay thế các giá trị trong danh sách data
                List<string> replacedData = new List<string>();
                foreach (var item in emailTemplate)
                {
                    string sentence = item.ContentEmail;

                    // Trích xuất các giá trị trong cặp ngoặc nhọn {{}}
                    List<string> extractedValues = ExtractValuesFromBrackets(sentence);
                    var SystemPara = SqlHelper.QuerySP<SystemParametersViewModel>("spCrm_SystemParameters").ToList();

                    foreach (var ite in extractedValues)
                    {
                        var storedprocedure = SystemPara.FirstOrDefault(x => x.Name == ite)?.Storedprocedure;
                        var fiedName = SystemPara.FirstOrDefault(x => x.Name == ite)?.FiedName;
                        if (storedprocedure != null)
                        {
                            if (typeLead == true)
                            {
                                var result = SqlHelper.QuerySP(storedprocedure, new
                                {
                                    @pColumnName = fiedName,
                                    @pLeadId = id
                                });
                                if (result != null && result.Count() > 0)
                                {
                                    sentence = sentence.Replace($"{{{{{ite}}}}}", result.First().ValueByName);
                                }
                            }
                            else
                            {
                                var result = SqlHelper.QuerySP(storedprocedure, new
                                {
                                    @pColumnName = fiedName,
                                    @pCustomerId = id
                                });
                                if (result != null && result.Count() > 0)
                                {
                                    sentence = sentence.Replace($"{{{{{ite}}}}}", result.First().ValueByName);
                                }
                            }

                        }
                    }
                    item.ContentEmail = sentence;
                }
            }

            var data = new
            {
                EmailSender = emailSender,
                EmailTemplate = emailTemplate,
                Campaign = campaign,
                EmailFooter = emailFooter,
            };
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public async Task<ActionResult> SendApiPhoneCall(string Ext, string PhoneName, string PhoneNumber, string Id, int? isPartial)
        {
            string pattern1 = @"^\+84";
            string pattern2 = @"^84";
            Regex regex1 = new Regex(pattern1);
            Regex regex2 = new Regex(pattern2);
            if (regex1.IsMatch(PhoneNumber))
            { // +84
                PhoneNumber = "0" + PhoneNumber.Substring(3);
            }
            if (regex2.IsMatch(PhoneNumber))
            { // 84
                PhoneNumber = "0" + PhoneNumber.Substring(2);
            }

            string ServiceName = ConfigurationManager.AppSettings["PhoneCallAPI:ServiceName"];
            string AuthUser = ConfigurationManager.AppSettings["PhoneCallAPI:AuthUser"];
            string AuthKey = ConfigurationManager.AppSettings["PhoneCallAPI:AuthKey"];
            string Prefix = ConfigurationManager.AppSettings["PhoneCallAPI:Prefix"];
            string Url = ConfigurationManager.AppSettings["PhoneCallAPI:Url"];

            var requestData = new
            {
                ServiceName = ServiceName,
                AuthUser = AuthUser,
                AuthKey = AuthKey,
                Prefix = Prefix,
                Ext = Ext,
                PhoneName = PhoneName,
                PhoneNumber = PhoneNumber,
                KeySearch = ""
            };
            var requestDataJson = JsonConvert.SerializeObject(requestData);
            var requestContent = new StringContent(requestDataJson, Encoding.UTF8, "application/json");

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    HttpResponseMessage response = await client.PostAsync(Url, requestContent);

                    if (response.IsSuccessStatusCode)
                    {
                        string responseBody = await response.Content.ReadAsStringAsync();
                        dynamic jsonObject = JsonConvert.DeserializeObject(responseBody);
                        string result = jsonObject.result;
                        if (result == "success")
                        {
                            var resultQuery = SqlHelper.ExecuteSP("spSale_LeadLogs_Create_SendSMS", new
                            {
                                @pLeadId = Id,
                                @pAction = "CALL",
                                @pLogs = "",
                                @pStatus = 1,
                                @pMobile = PhoneNumber,
                                @CreatedUserId = WebSecurity.CurrentUserId,
                                @pTypeLead = isPartial == 1 ? 0 : 1
                            });
                            return Json(new { Success = true });
                        }
                        else //config issue
                        {
                            return Json(new { Success = false, ErrorMessage = "Gọi thất bại! Vui lòng kiểm tra cài đặt" });
                        }
                    }
                    else //api server side issue
                    {
                        return Json(new { Success = false, ErrorMessage = "Gọi thất bại! Lỗi phía tổng đài!" });
                    }
                }
            }
            catch (HttpRequestException ex) //network issue
            {
                return Json(new { Success = false, result = "null", ErrorMessage = "Lỗi mạng, vui lòng kiểm tra!" });
            }
        }
        [HttpPost]
        [AllowAnonymous]
        public void CloudFoneWebHookHandle()
        {
            string FileContent = "";
            using (StreamReader sr = new StreamReader(Request.InputStream))
            {
                FileContent = sr.ReadToEnd();
            }
            dynamic data = JsonConvert.DeserializeObject(FileContent);
            string pEvent = data.Event;
            string pLID = data.LID;
            string pDir = data.Dir;
            string pCID = data.CID;
            string pTimeCall = data.TimeCall;
            string pCh = data.Ch;
            string pTimeBillSec = data.TimeBillSec;
            string pBillSecBy = data.BillSecBy;
            string pIVR = data.IVR;
            string pQueue = data.Queue;
            string pTimeDial = data.TimeDial;
            string pExt = data.Ext;
            string pUid = data.Uid;
            string pTimeStopDial = data.TimeStopDial;
            string pDisposition = data.Disposition;
            string pTimeAnswer = data.TimeAnswer;
            string pChDest = data.ChDest;
            string pTimeEnd = data.TimeEnd;
            string precordingfileURL = data.recordingfileURL;



            switch (pEvent)
            {
                case "register":
                    SqlHelper.ExecuteSP("spSale_CloudFoneWebHook_Register_INSERT", new
                    {
                        @pEvent = pEvent,
                        @pLID = pLID,
                        @pDir = pDir,
                        @pCID = pCID,
                        @pTimeCall = pTimeCall,
                        @pCh = pCh,
                        @pCreatedUserId = WebSecurity.CurrentUserId
                    });
                    if (pDir == "Inbound")
                    {
                        getLeadByMobile(pCID);
                    }
                    break;
                case "show":
                    SqlHelper.ExecuteSP("spSale_CloudFoneWebHook_Show_INSERT", new
                    {
                        @pEvent = pEvent,
                        @pLID = pLID,
                        @pDir = pDir,
                        @pCID = pCID,
                        @pTimeCall = pTimeCall,
                        @pCh = pCh,
                        @pTimeBillSec = pTimeBillSec,
                        @pBillSecBy = pBillSecBy,
                        @pIVR = pIVR,
                        @pQueue = pQueue,
                        @pTimeDial = pTimeDial,
                        @pExt = pExt,
                        @pUid = pUid,
                        @pCreatedUserId = WebSecurity.CurrentUserId
                    });
                    break;
                case "answer":
                    SqlHelper.ExecuteSP("spSale_CloudFoneWebHook_Answer_INSERT", new
                    {
                        @pEvent = pEvent,
                        @pLID = pLID,
                        @pDir = pDir,
                        @pCID = pCID,
                        @pTimeCall = pTimeCall,
                        @pCh = pCh,
                        @pTimeBillSec = pTimeBillSec,
                        @pBillSecBy = pBillSecBy,
                        @pIVR = pIVR,
                        @pQueue = pQueue,
                        @pTimeAnswer = pTimeAnswer,
                        @pChDest = pChDest,
                        @pExt = pExt,
                        @pUid = pUid,
                        @pCreatedUserId = WebSecurity.CurrentUserId
                    });
                    break;
                case "hide":
                    SqlHelper.ExecuteSP("spSale_CloudFoneWebHook_Hide_INSERT", new
                    {
                        @pEvent = pEvent,
                        @pLID = pLID,
                        @pDir = pDir,
                        @pCID = pCID,
                        @pTimeCall = pTimeCall,
                        @pCh = pCh,
                        @pTimeBillSec = pTimeBillSec,
                        @pBillSecBy = pBillSecBy,
                        @pIVR = pIVR,
                        @pQueue = pQueue,
                        @pTimeDial = pTimeDial,
                        @pTimeStopDial = pTimeStopDial,
                        @pExt = pExt,
                        @pUid = pUid,
                        @pDisposition = pDisposition,
                        @pCreatedUserId = WebSecurity.CurrentUserId
                    });
                    break;
                case "finish":
                    // code block
                    SqlHelper.ExecuteSP("spSale_CloudFoneWebHook_Finish_INSERT", new
                    {
                        @pEvent = pEvent,
                        @pLID = pLID,
                        @pDir = pDir,
                        @pCID = pCID,
                        @pTimeCall = pTimeCall,
                        @pCh = pCh,
                        @pTimeBillSec = pTimeBillSec,
                        @pBillSecBy = pBillSecBy,
                        @pIVR = pIVR,
                        @pQueue = pQueue,
                        @pTimeAnswer = pTimeAnswer,
                        @pChDest = pChDest,
                        @pExt = pExt,
                        @pUid = pUid,
                        @pTimeEnd = pTimeEnd,
                        @precordingfileURL = precordingfileURL,
                        @pCreatedUserId = WebSecurity.CurrentUserId
                    });
                    break;
            }






        }
        [AllowAnonymous]
        public ActionResult ShowEmailModalHangfire()
        {
            string errorMessageEmail = "";
            string emailSender = WebConfigurationManager.AppSettings["EmailSender"];
            if (emailSender == null)
            {
                emailSender = SqlHelper.QuerySP<string>("spSystem_User_SelectEmailById", new
                {
                    pId = WebSecurity.CurrentUserId
                }).FirstOrDefault();
            }
            var emailTemplate = SqlHelper.QuerySP<AdviseCardEmailTemplateViewModel>("spCrm_Template_SelectAllEmail", new
            {
                @pTypeLead = true
            }).ToList();
            var emailFooter = SqlHelper.QuerySP<CrmEmailFooterViewModel>("spCrm_EmailFooter_Select", new
            {
                @pCreatedUserId = WebSecurity.CurrentUserId
            }).ToList();
            var campaign = SqlHelper.QuerySP<AdviseCardCampaignViewModel>("spCrm_Campaign_Select").ToList();
            string port = WebConfigurationManager.AppSettings["Port"];
            string ssl = WebConfigurationManager.AppSettings["SSL"];
            string smtp = WebConfigurationManager.AppSettings["SMTP"];
            string smtpUserName = WebConfigurationManager.AppSettings["SMTPUserName"];
            string smtpPassword = WebConfigurationManager.AppSettings["SMTPPassword"];

            if (port == null || ssl == null || smtp == null || smtpUserName == null || smtpPassword == null)
            {
                errorMessageEmail = "Bạn chưa cài đặt thông số gửi mail, vui lòng cài đặt!";
            }

            var data = new
            {
                EmailSender = emailSender,
                EmailTemplate = emailTemplate,
                Campaign = campaign,
                errorMessageEmail = errorMessageEmail,
                EmailFooter = emailFooter,

            };
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        [AllowAnonymous]
        [HttpPost]
        public ActionResult SendEmailHangfire(string EmailSender, string EmailTitle, string EmailBody, string EmailReceiver, string EmailId, string DateAction, string TimeAction, int? CampaignId, string CampaignName)
        {


            string[] emails = EmailReceiver.Split(';');
            string[] ids = EmailId.Split(';');

            try
            {
                for (int i = 0; i < emails.Length; i++)
                {
                    string sentence = EmailBody;
                    // Trích xuất các giá trị trong cặp ngoặc nhọn {{}}
                    List<string> extractedValues = ExtractValuesFromBrackets(sentence);
                    var SystemPara = SqlHelper.QuerySP<SystemParametersViewModel>("spCrm_SystemParameters").ToList();

                    foreach (var ite in extractedValues)
                    {
                        var storedprocedure = SystemPara.FirstOrDefault(x => x.Name == ite)?.Storedprocedure;
                        var fiedName = SystemPara.FirstOrDefault(x => x.Name == ite)?.FiedName;
                        if (storedprocedure != null)
                        {
                            var result = SqlHelper.QuerySP(storedprocedure, new
                            {
                                @pColumnName = fiedName,
                                @pLeadId = ids[i]
                            });
                            // Thay thế giá trị result vào sentence
                            sentence = sentence.Replace($"{{{{{ite}}}}}", result.First().ValueByName);
                        }
                    }

                    AdviseCardSendEmailHangfireViewModel model = new AdviseCardSendEmailHangfireViewModel()
                    {
                        Sender = EmailSender,
                        Receiver = emails[i],
                        Title = EmailTitle,
                        Body = sentence,
                        CampaignId = (CampaignId.HasValue ? CampaignId.Value : 0),
                        CampaignName = CampaignName,
                    };
                    var logId = SqlHelper.QuerySP<int>("spSale_LeadLogs_CreateHangfire_ReturnLogId", new
                    {
                        @pLeadId = ids[i],
                        @pAction = "SENDEMAIL",
                        @pLogs = model.Body,
                        @pMobile = model.Receiver,
                        @CreatedUserId = WebSecurity.CurrentUserId,
                        @pCampaignId = model.CampaignId,
                        @pTypeLead = 1
                    }).FirstOrDefault();

                    ExecuteSendEmailHangfire(model, ids[i], WebSecurity.CurrentUserId, logId, DateAction, TimeAction);


                    //else
                    //{
                    //    // Nếu không, lập lịch
                    //    var jobId = BackgroundJob.Schedule(() => ExecuteSendEmailHangfire(model, ids[i], WebSecurity.CurrentUserId, logId), notificationTime);
                    //}

                }
                var data = new
                {
                    success = true
                };
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                var data = new
                {
                    success = false
                };
                return Json(data);
            }



        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult SendEmailHangfireCustomer(string EmailSender, string EmailTitle, string EmailBody, string EmailReceiver, string EmailId, string DateAction, string TimeAction, int? CampaignId, string CampaignName)
        {


            string[] emails = EmailReceiver.Split(';');
            string[] ids = EmailId.Split(';');

            try
            {
                for (int i = 0; i < emails.Length; i++)
                {
                    //string sentence = EmailBody;
                    //// Trích xuất các giá trị trong cặp ngoặc nhọn {{}}
                    //List<string> extractedValues = ExtractValuesFromBrackets(sentence);
                    //var SystemPara = SqlHelper.QuerySP<SystemParametersViewModel>("spCrm_SystemParameters").ToList();

                    //foreach (var ite in extractedValues)
                    //{
                    //    var storedprocedure = SystemPara.FirstOrDefault(x => x.Name == ite)?.Storedprocedure;
                    //    var fiedName = SystemPara.FirstOrDefault(x => x.Name == ite)?.FiedName;
                    //    if (storedprocedure != null)
                    //    {
                    //        var result = SqlHelper.QuerySP(storedprocedure, new
                    //        {
                    //            @pColumnName = fiedName,
                    //            @pLeadId = ids[i]
                    //        });
                    //        // Thay thế giá trị result vào sentence
                    //        sentence = sentence.Replace($"{{{{{ite}}}}}", result.First().ValueByName);
                    //    }
                    //}


                    AdviseCardSendEmailHangfireViewModel model = new AdviseCardSendEmailHangfireViewModel()
                    {
                        Sender = EmailSender,
                        Receiver = emails[i],
                        Title = EmailTitle,
                        Body = EmailBody,
                        CampaignId = (CampaignId.HasValue ? CampaignId.Value : 0),
                        CampaignName = CampaignName,
                    };
                    var logId = SqlHelper.QuerySP<int>("spSale_LeadLogs_CreateHangfire_ReturnLogId", new
                    {
                        @pLeadId = ids[i],
                        @pAction = "SENDEMAIL",
                        @pLogs = model.Body,
                        @pMobile = model.Receiver,
                        @CreatedUserId = WebSecurity.CurrentUserId,
                        @pCampaignId = model.CampaignId,
                        @pTypeLead = 0
                    }).FirstOrDefault();

                    ExecuteSendEmailHangfire(model, ids[i], WebSecurity.CurrentUserId, logId, DateAction, TimeAction);


                    //else
                    //{
                    //    // Nếu không, lập lịch
                    //    var jobId = BackgroundJob.Schedule(() => ExecuteSendEmailHangfire(model, ids[i], WebSecurity.CurrentUserId, logId), notificationTime);
                    //}

                }
                var data = new
                {
                    success = true
                };
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                var data = new
                {
                    success = false
                };
                return Json(data);
            }



        }
        public static void ExecuteSendEmailHangfire(AdviseCardSendEmailViewModel model, string id, int createdUserId, int logId, string DateAction, string TimeAction)
        {
            string port = WebConfigurationManager.AppSettings["Port"];
            string ssl = WebConfigurationManager.AppSettings["SSL"];
            string smtp = WebConfigurationManager.AppSettings["SMTP"];
            string smtpUserName = WebConfigurationManager.AppSettings["SMTPUserName"];
            string smtpPassword = WebConfigurationManager.AppSettings["SMTPPassword"];
            string server = Path.Combine(HttpRuntime.AppDomainAppPath);
            DateTime actionDateTime = DateTime.Parse(DateAction).Date + TimeSpan.Parse(TimeAction);
            DateTime dateAction = DateTime.Parse(DateAction);
            TimeSpan timeAction = TimeSpan.Parse(TimeAction);
            DateTime notificationTime = dateAction.Date + timeAction;

            // Lấy ngày/tháng/năm hiện tại
            DateTime currentDate = DateTime.Now.Date;

            // Lấy giờ/phút hiện tại
            TimeSpan currentTime = DateTime.Now.TimeOfDay;

            // Kiểm tra nếu DateAction và TimeAction bằng với ngày và giờ hiện tại (không cần tính đến giây)
            if (actionDateTime.Date == currentDate && actionDateTime.TimeOfDay.Hours == currentTime.Hours && actionDateTime.TimeOfDay.Minutes == currentTime.Minutes)
            {
                SendMailNotGmail.SendEmailHangFire(port, ssl, smtp, model.Sender, smtpUserName, smtpPassword, model.Receiver, model.Title, model.Body, server);
            }
            else
            {
                var jobId = BackgroundJob.Schedule(() => SendMailNotGmail.SendEmailHangFire(port, ssl, smtp, model.Sender, smtpUserName, smtpPassword, model.Receiver, model.Title, model.Body, server), notificationTime);

            }
            //var result = SqlHelper.ExecuteSP("spSale_LeadLogs_Create_SendSMS", new
            //{
            //    @pLeadId = id,
            //    @pAction = "SENDEMAIL",
            //    @pLogs = model.Body,
            //    @pStatus = 1,
            //    @pMobile = model.Receiver,
            //    @CreatedUserId = createdUserId,
            //    @pTypeLead = 1
            //});
            var result = SqlHelper.ExecuteSP("spSale_LeadLogs_UpdateHangfire_UsingLogId", new
            {
                @plogId = logId,
                @pStatus = 1,
                @pErrorMessage = ""
            });
        }
        [AllowAnonymous]
        [HttpPost]
        public ActionResult SendEmailWithFilesHangFireCustomer(string EmailSender, string EmailTitle, string EmailBody, string EmailReceiver, string EmailId, string DateAction, string TimeAction, int? CampaignId, string CampaignName)
        {
            string[] emails = EmailReceiver.Split(';');
            string[] ids = EmailId.Split(';');

            EmailBody = HttpUtility.UrlDecode(EmailBody);

            try
            {
                for (int i = 0; i < emails.Length; i++)
                {
                    //string sentence = EmailBody;
                    //// Trích xuất các giá trị trong cặp ngoặc nhọn {{}}
                    //List<string> extractedValues = ExtractValuesFromBrackets(sentence);
                    //var SystemPara = SqlHelper.QuerySP<SystemParametersViewModel>("spCrm_SystemParameters").ToList();

                    //foreach (var ite in extractedValues)
                    //{
                    //    var storedprocedure = SystemPara.FirstOrDefault(x => x.Name == ite)?.Storedprocedure;
                    //    var fiedName = SystemPara.FirstOrDefault(x => x.Name == ite)?.FiedName;
                    //    if (storedprocedure != null)
                    //    {
                    //        var result = SqlHelper.QuerySP(storedprocedure, new
                    //        {
                    //            @pColumnName = fiedName,
                    //            @pLeadId = ids[i]
                    //        });
                    //        // Thay thế giá trị result vào sentence
                    //        sentence = sentence.Replace($"{{{{{ite}}}}}", result.First().ValueByName);
                    //    }
                    //}
                    List<(string fileName, string filePath)> attachments = new List<(string fileName, string filePath)>();

                    foreach (string fileKey in Request.Files)
                    {
                        HttpPostedFileBase file = Request.Files[fileKey];
                        var originalFileName = Path.GetFileName(file.FileName);
                        var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(originalFileName);
                        var fileExtension = Path.GetExtension(originalFileName);

                        var tempDirectory = Server.MapPath("~/UploadedFile");

                        // Kiểm tra và tạo thư mục nếu cần
                        if (!Directory.Exists(tempDirectory))
                        {
                            Directory.CreateDirectory(tempDirectory);
                        }

                        // Tạo tên file duy nhất bằng cách kết hợp tên file gốc với một timestamp
                        var uniqueFileName = $"{fileNameWithoutExtension}_{DateTime.Now.Ticks}{fileExtension}";

                        var tempPath = Path.Combine(tempDirectory, uniqueFileName);
                        file.SaveAs(tempPath);
                        attachments.Add((originalFileName, tempPath));
                    }

                    AdviseCardSendEmailHangfireViewModel model = new AdviseCardSendEmailHangfireViewModel()
                    {
                        Sender = EmailSender,
                        Receiver = emails[i],
                        Title = EmailTitle,
                        Body = EmailBody,
                        CampaignId = (CampaignId.HasValue ? CampaignId.Value : 0),
                        CampaignName = CampaignName,
                    };
                    var logId = SqlHelper.QuerySP<int>("spSale_LeadLogs_CreateHangfire_ReturnLogId", new
                    {
                        @pLeadId = ids[i],
                        @pAction = "SENDEMAIL",
                        @pLogs = model.Body,
                        @pMobile = model.Receiver,
                        @CreatedUserId = WebSecurity.CurrentUserId,
                        @pCampaignId = model.CampaignId,
                        @pTypeLead = 0
                    }).FirstOrDefault();
                    ExecuteSendEmailWithFilesHangfire(model, ids[i], WebSecurity.CurrentUserId, logId, DateAction, TimeAction, attachments);

                }


                var data = new
                {
                    success = true
                };
                return Json(data, JsonRequestBehavior.AllowGet);

            }


            catch (Exception ex)
            {
                var data = new
                {
                    success = false
                };
                return Json(data);
            }

        }


        [AllowAnonymous]
        [HttpPost]
        public ActionResult SendEmailWithFilesHangFire(string EmailSender, string EmailTitle, string EmailBody, string EmailReceiver, string EmailId, string DateAction, string TimeAction, int? CampaignId, string CampaignName)
        {
            string[] emails = EmailReceiver.Split(';');
            string[] ids = EmailId.Split(';');

            EmailBody = HttpUtility.UrlDecode(EmailBody);

            try
            {
                for (int i = 0; i < emails.Length; i++)
                {
                    string sentence = EmailBody;
                    // Trích xuất các giá trị trong cặp ngoặc nhọn {{}}
                    List<string> extractedValues = ExtractValuesFromBrackets(sentence);
                    var SystemPara = SqlHelper.QuerySP<SystemParametersViewModel>("spCrm_SystemParameters").ToList();

                    foreach (var ite in extractedValues)
                    {
                        var storedprocedure = SystemPara.FirstOrDefault(x => x.Name == ite)?.Storedprocedure;
                        var fiedName = SystemPara.FirstOrDefault(x => x.Name == ite)?.FiedName;
                        if (storedprocedure != null)
                        {
                            var result = SqlHelper.QuerySP(storedprocedure, new
                            {
                                @pColumnName = fiedName,
                                @pLeadId = ids[i]
                            });
                            // Thay thế giá trị result vào sentence
                            sentence = sentence.Replace($"{{{{{ite}}}}}", result.First().ValueByName);
                        }
                    }
                    List<(string fileName, string filePath)> attachments = new List<(string fileName, string filePath)>();

                    foreach (string fileKey in Request.Files)
                    {
                        HttpPostedFileBase file = Request.Files[fileKey];
                        var originalFileName = Path.GetFileName(file.FileName);
                        var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(originalFileName);
                        var fileExtension = Path.GetExtension(originalFileName);

                        var tempDirectory = Server.MapPath("~/UploadedFile");

                        // Kiểm tra và tạo thư mục nếu cần
                        if (!Directory.Exists(tempDirectory))
                        {
                            Directory.CreateDirectory(tempDirectory);
                        }

                        // Tạo tên file duy nhất bằng cách kết hợp tên file gốc với một timestamp
                        var uniqueFileName = $"{fileNameWithoutExtension}_{DateTime.Now.Ticks}{fileExtension}";

                        var tempPath = Path.Combine(tempDirectory, uniqueFileName);
                        file.SaveAs(tempPath);
                        attachments.Add((originalFileName, tempPath));
                    }

                    AdviseCardSendEmailHangfireViewModel model = new AdviseCardSendEmailHangfireViewModel()
                    {
                        Sender = EmailSender,
                        Receiver = emails[i],
                        Title = EmailTitle,
                        Body = sentence,
                        CampaignId = (CampaignId.HasValue ? CampaignId.Value : 0),
                        CampaignName = CampaignName,
                    };
                    var logId = SqlHelper.QuerySP<int>("spSale_LeadLogs_CreateHangfire_ReturnLogId", new
                    {
                        @pLeadId = ids[i],
                        @pAction = "SENDEMAIL",
                        @pLogs = model.Body,
                        @pMobile = model.Receiver,
                        @CreatedUserId = WebSecurity.CurrentUserId,
                        @pCampaignId = model.CampaignId,
                        @pTypeLead = 1,
                    }).FirstOrDefault();
                    ExecuteSendEmailWithFilesHangfire(model, ids[i], WebSecurity.CurrentUserId, logId, DateAction, TimeAction, attachments);

                }


                var data = new
                {
                    success = true
                };
                return Json(data, JsonRequestBehavior.AllowGet);

            }


            catch (Exception ex)
            {
                var data = new
                {
                    success = false
                };
                return Json(data);
            }

        }

        public static void ExecuteSendEmailWithFilesHangfire(AdviseCardSendEmailViewModel model, string id, int createdUserId, int logId, string DateAction, string TimeAction, List<(string fileName, string filePath)> attachments)
        {
            string port = WebConfigurationManager.AppSettings["Port"];
            string ssl = WebConfigurationManager.AppSettings["SSL"];
            string smtp = WebConfigurationManager.AppSettings["SMTP"];
            string smtpUserName = WebConfigurationManager.AppSettings["SMTPUserName"];
            string smtpPassword = WebConfigurationManager.AppSettings["SMTPPassword"];
            string server = Path.Combine(HttpRuntime.AppDomainAppPath);
            DateTime actionDateTime = DateTime.Parse(DateAction).Date + TimeSpan.Parse(TimeAction);
            DateTime dateAction = DateTime.Parse(DateAction);
            TimeSpan timeAction = TimeSpan.Parse(TimeAction);
            DateTime notificationTime = dateAction.Date + timeAction;

            // Lấy ngày/tháng/năm hiện tại
            DateTime currentDate = DateTime.Now.Date;

            // Lấy giờ/phút hiện tại
            TimeSpan currentTime = DateTime.Now.TimeOfDay;

            // Kiểm tra nếu DateAction và TimeAction bằng với ngày và giờ hiện tại (không cần tính đến giây)
            if (actionDateTime.Date == currentDate && actionDateTime.TimeOfDay.Hours == currentTime.Hours && actionDateTime.TimeOfDay.Minutes == currentTime.Minutes)
            {
                SendMailNotGmail.SendEmailAndFilesHangFire(port, ssl, smtp, model.Sender, smtpUserName, smtpPassword, model.Receiver, model.Title, model.Body, attachments, server);
            }
            else
            {
                var jobId = BackgroundJob.Schedule(() => SendMailNotGmail.SendEmailAndFilesHangFire(port, ssl, smtp, model.Sender, smtpUserName, smtpPassword, model.Receiver, model.Title, model.Body, attachments, server), notificationTime);

            }
            //var result = SqlHelper.ExecuteSP("spSale_LeadLogs_Create_SendSMS", new
            //{
            //    @pLeadId = id,
            //    @pAction = "SENDEMAIL",
            //    @pLogs = model.Body,
            //    @pStatus = 1,
            //    @pMobile = model.Receiver,
            //    @CreatedUserId = createdUserId,
            //    @pTypeLead = 1
            //});
            var result = SqlHelper.ExecuteSP("spSale_LeadLogs_UpdateHangfire_UsingLogId", new
            {
                @plogId = logId,
                @pStatus = 1,
                @pErrorMessage = ""
            });
        }
        [AllowAnonymous]
        [HttpGet]
        public ActionResult GetSMSListHangfire()
        {
            string errorMessageSendSMS = "";
            //Check Api SMS Call Config
            string requestlink = ConfigurationManager.AppSettings["SendSMS:Link"];
            string secretKey = ConfigurationManager.AppSettings["SendSMS:SecretKey"];
            string apiKey = ConfigurationManager.AppSettings["SendSMS:ApiKey"];
            string brandName = ConfigurationManager.AppSettings["SendSMS:Brandname"];

            if (apiKey == null || secretKey == null || brandName == null || requestlink == null)
            {
                errorMessageSendSMS = "Thiếu thông số cài đặt để gửi SMS! Vui lòng kiểm tra!";
            }
            var campaign = SqlHelper.QuerySP<AdviseCardCampaignViewModel>("spCrm_Campaign_Select").ToList();
            var smsMessages = SqlHelper.QuerySP<Sale_GetList_TemplateSMS>("spCrm_Template_SelectTemplateSMS", new
            {
                @pTypeLead = true
            }).ToList();
            var data = new
            {
                smsMessages = smsMessages,
                Campaign = campaign,
                errorMessageEmail = errorMessageSendSMS,
            };
            return Json(data, JsonRequestBehavior.AllowGet);
        }


        public class SmsHangfireResponse
        {
            public string Id { get; set; }
            public string Code { get; set; }
        }
        public static async Task ExecuteSendSMSHangfireAsync(string contentSMS, string phone, string id, int createdUserId, int CampaignId, int logResult)
        {
            var status = false;
            string codeResult = "";
            string requestlink = ConfigurationManager.AppSettings["SendSMS:Link"];
            string secretKey = ConfigurationManager.AppSettings["SendSMS:SecretKey"];
            string apiKey = ConfigurationManager.AppSettings["SendSMS:ApiKey"];
            string brandName = ConfigurationManager.AppSettings["SendSMS:Brandname"];
            string encodedContent = HttpUtility.UrlEncode(contentSMS);
            string requestUrl = requestlink + "&ApiKey={ApiKey}&SecretKey={SecretKey}&Brandname={Brandname}&Content={Content}&Phone={Phone}";
            requestUrl = requestUrl.Replace("{ApiKey}", apiKey)
                                   .Replace("{SecretKey}", secretKey)
                                   .Replace("{Brandname}", brandName)
                                   .Replace("{Content}", encodedContent)
                                   .Replace("{Phone}", phone);

            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(requestUrl);
                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    dynamic jsonObject = JsonConvert.DeserializeObject(responseBody);
                    codeResult = jsonObject.CodeResult;
                    if (codeResult == "100")
                    {
                        status = true;
                    }
                    else
                    {
                        status = false;
                    }

                }
                else
                {
                    status = false;
                }
            }
            //var result = SqlHelper.ExecuteSP("spSale_LeadLogs_Create_SendSMS", new
            //{
            //    @pLeadId = id,
            //    @pAction = "SMS",
            //    @pLogs = contentSMS,
            //    @pStatus = status,
            //    @pMobile = phone,
            //    @CreatedUserId = createdUserId,
            //    @pTypeLead = 1
            //});
            var result = SqlHelper.ExecuteSP("spSale_LeadLogs_UpdateHangfire_UsingLogId", new
            {
                @plogId = logResult,
                @pStatus = status,
                @pErrorMessage = codeResult
            });
        }
        [AllowAnonymous]
        [HttpPost]
        public ActionResult SendApiSMSHangfire(string ContentSMS, string Phones, string Ids, string DateAction, string TimeAction, int? CampaignId, string CampaignName) //all parameters are validated in views
        {

            DateTime dateAction = DateTime.Parse(DateAction);
            TimeSpan timeAction = TimeSpan.Parse(TimeAction);
            DateTime notificationTime = dateAction.Date + timeAction;

            string[] phones = Phones.Split(';');
            string[] ids = Ids.Split(';');

            List<SmsHangfireResponse> listResponses = new List<SmsHangfireResponse>();

            try
            {
                for (int i = 0; i < phones.Length; i++)
                {
                    string sentence = ContentSMS;
                    // Trích xuất các giá trị trong cặp ngoặc nhọn {{}}
                    List<string> extractedValues = ExtractValuesFromBrackets(sentence);
                    var SystemPara = SqlHelper.QuerySP<SystemParametersViewModel>("spCrm_SystemParameters").ToList();

                    foreach (var ite in extractedValues)
                    {
                        var storedprocedure = SystemPara.FirstOrDefault(x => x.Name == ite)?.Storedprocedure;
                        var fiedName = SystemPara.FirstOrDefault(x => x.Name == ite)?.FiedName;
                        if (storedprocedure != null)
                        {
                            var result = SqlHelper.QuerySP(storedprocedure, new
                            {
                                @pColumnName = fiedName,
                                @pLeadId = ids[i]
                            });
                            // Thay thế giá trị result vào sentence
                            sentence = sentence.Replace($"{{{{{ite}}}}}", result.First().ValueByName);
                        }
                    }
                    var logResult = SqlHelper.QuerySP<int>("spSale_LeadLogs_CreateHangfire_ReturnLogId", new
                    {
                        @pLeadId = ids[i],
                        @pAction = "SMS",
                        @pLogs = sentence,
                        @pMobile = phones[i],
                        @CreatedUserId = WebSecurity.CurrentUserId,
                        @pCampaignId = (CampaignId.HasValue ? CampaignId.Value : 0),
                        @pTypeLead = 1
                    }).FirstOrDefault();
                    var jobId = BackgroundJob.Schedule(() => ExecuteSendSMSHangfireAsync(sentence, phones[i], ids[i], WebSecurity.CurrentUserId, (CampaignId.HasValue ? CampaignId.Value : 0), logResult), notificationTime);
                }
                var data = new
                {
                    success = true
                };
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                var data = new
                {
                    success = false
                };
                return Json(data);
            }
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult SendApiSMSHangfireCustomer(string ContentSMS, string Phones, string Ids, string DateAction, string TimeAction, int? CampaignId, string CampaignName) //all parameters are validated in views
        {

            DateTime dateAction = DateTime.Parse(DateAction);
            TimeSpan timeAction = TimeSpan.Parse(TimeAction);
            DateTime notificationTime = dateAction.Date + timeAction;

            string[] phones = Phones.Split(';');
            string[] ids = Ids.Split(';');

            List<SmsHangfireResponse> listResponses = new List<SmsHangfireResponse>();

            try
            {
                for (int i = 0; i < phones.Length; i++)
                {
                    //string sentence = ContentSMS;
                    //// Trích xuất các giá trị trong cặp ngoặc nhọn {{}}
                    //List<string> extractedValues = ExtractValuesFromBrackets(sentence);
                    //var SystemPara = SqlHelper.QuerySP<SystemParametersViewModel>("spCrm_SystemParameters").ToList();

                    //foreach (var ite in extractedValues)
                    //{
                    //    var storedprocedure = SystemPara.FirstOrDefault(x => x.Name == ite)?.Storedprocedure;
                    //    var fiedName = SystemPara.FirstOrDefault(x => x.Name == ite)?.FiedName;
                    //    if (storedprocedure != null)
                    //    {
                    //        var result = SqlHelper.QuerySP(storedprocedure, new
                    //        {
                    //            @pColumnName = fiedName,
                    //            @pLeadId = ids[i]
                    //        });
                    //        // Thay thế giá trị result vào sentence
                    //        sentence = sentence.Replace($"{{{{{ite}}}}}", result.First().ValueByName);
                    //    }
                    //}
                    var logResult = SqlHelper.QuerySP<int>("spSale_LeadLogs_CreateHangfire_ReturnLogId", new
                    {
                        @pLeadId = ids[i],
                        @pAction = "SMS",
                        @pLogs = ContentSMS,
                        @pMobile = phones[i],
                        @CreatedUserId = WebSecurity.CurrentUserId,
                        @pCampaignId = (CampaignId.HasValue ? CampaignId.Value : 0),
                        @pTypeLead = 0
                    }).FirstOrDefault();
                    var jobId = BackgroundJob.Schedule(() => ExecuteSendSMSHangfireAsync(ContentSMS, phones[i], ids[i], WebSecurity.CurrentUserId, (CampaignId.HasValue ? CampaignId.Value : 0), logResult), notificationTime);
                }
                var data = new
                {
                    success = true
                };
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                var data = new
                {
                    success = false
                };
                return Json(data);
            }
        }
        [AllowAnonymous]
        [HttpPost]
        public PartialViewResult ShowGopLeadCheck(bool leadName, bool mobile, bool taxCode, bool email, [FromBody] string leadNameIn, [FromBody] string mobileIn, [FromBody] string taxCodeIn, [FromBody] string emailIn, [FromBody] int? ldleadview = null)
        {

            var model = SqlHelper.QuerySP<GopLeadCheckViewModel>("spSale_LeadGroupBy_LeadName_Mobile_TaxCode", new
            {
                pleadName = leadName,
                pmobile = mobile,
                ptaxCode = taxCode,
                pemail = email,
                pLeadNameInClause = leadNameIn,
                pMobileInClause = mobileIn,
                pTaxCodeInClause = taxCodeIn,
                pEmailInClause = emailIn,
                ldleadview = ldleadview
            });
            string[] selection = new string[] { (leadName ? "LeadName" : ""), (mobile ? "Mobile" : ""), (taxCode ? "TaxCode" : ""), (email ? "Email" : ""), "MainId", "Count" };
            ViewBag.selection = selection;
            return PartialView("ShowGopLeadCheck", model);
        }
        [AllowAnonymous]
        [HttpGet]
        public PartialViewResult ShowGopLeadDetail(string leadName, string mobile, string taxCode, string email, string id, string stt)
        {

            var model = SqlHelper.QuerySP<LeadModel>("spSale_Lead_Select", new
            {
                pleadName = leadName,
                pmobile = mobile,
                ptaxCode = taxCode,
                pemail = email
            }).ToList();

            model = model != null ? model : new List<LeadModel>();
            var section = SqlHelper.QueryMultipleSP<LeadSectionModel, LeadSection_FieldModel>("AddNewModal");
            var user = userRepository;
            var lsstatus = SqlHelper.QuerySP<StatusLeadModel>("GetAllStatus");
            var category = categoryRepository;
            ViewBag.category = category;
            ViewData["section"] = section;
            ViewBag.user = user;
            ViewBag.lsstatus = lsstatus;
            ViewBag.mainId = long.Parse(id);
            ViewBag.sttSelection = Int32.Parse(stt);
            return PartialView("ShowGopLeadDetail", model);
        }
        [AllowAnonymous]
        [HttpPost]
        public ActionResult GopLeadHandle([FromBody] List<GopLeadCheckViewModel> model)
        {
            try
            {
                foreach (GopLeadCheckViewModel item in model)
                {
                    int a = SqlHelper.ExecuteSP("spSale_Lead_Combine", new
                    {
                        @pleadName = item.LeadName,
                        @pmobile = item.Mobile,
                        @ptaxCode = item.TaxCode,
                        @pemail = item.Email,
                        @pmainId = item.MainId
                    });
                }
                var data = new
                {
                    success = true,
                };
                return Json(data);
            }
            catch (Exception ex)
            {
                var data = new
                {
                    success = false,
                };
                return Json(data);
            }
        }
        [AllowAnonymous]
        public PartialViewResult QuickSearchLeadModal(string search) //isPartial=1 là partial, =2 là popup trong detail customer
        {
            var category = categoryRepository;
            var user = userRepository;
            var section = SqlHelper.QueryMultipleSP<LeadSectionModel, LeadSection_FieldModel>("AddNewModal");
            ViewBag.category = category;
            ViewBag.user = user;
            var lead = SqlHelper.QuerySP<LeadModel>("LoadLeadIndexBySearch", new
            {
                @pCurrentUserId = WebSecurity.CurrentUserId,
                @pSearch = search
            }).FirstOrDefault();
            if (lead == null)
            {
                return null;
            }
            ViewBag.lead = lead != null ? lead : new LeadModel();
            //Check Email config
            string port = WebConfigurationManager.AppSettings["Port"];
            string ssl = WebConfigurationManager.AppSettings["SSL"];
            string smtp = WebConfigurationManager.AppSettings["SMTP"];
            string smtpUserName = WebConfigurationManager.AppSettings["SMTPUserName"];
            string smtpPassword = WebConfigurationManager.AppSettings["SMTPPassword"];
            if (port == null || ssl == null || smtp == null || smtpUserName == null || smtpPassword == null)
            {
                string errorMessageEmail = "Bạn chưa cài đặt thông số gửi mail, vui lòng cài đặt!";
                ViewBag.errorMessageEmail = errorMessageEmail;
            }
            //Check Api Phone Call Config
            string ServiceName = ConfigurationManager.AppSettings["PhoneCallAPI:ServiceName"];
            string AuthUser = ConfigurationManager.AppSettings["PhoneCallAPI:AuthUser"];
            string AuthKey = ConfigurationManager.AppSettings["PhoneCallAPI:AuthKey"];
            string Prefix = ConfigurationManager.AppSettings["PhoneCallAPI:Prefix"];
            string Url = ConfigurationManager.AppSettings["PhoneCallAPI:Url"];

            if (ServiceName == null || AuthUser == null || AuthKey == null || Prefix == null || Url == null)
            {
                string errorMessagePhoneCall = "Bạn chưa cài đặt thông số để gọi điện thoại, vui lòng cài đặt!";
                ViewBag.errorMessagePhoneCall = errorMessagePhoneCall;
            }
            //Check Api SMS Call Config
            string requestlink = ConfigurationManager.AppSettings["SendSMS:Link"];
            string secretKey = ConfigurationManager.AppSettings["SendSMS:SecretKey"];
            string apiKey = ConfigurationManager.AppSettings["SendSMS:ApiKey"];
            string brandName = ConfigurationManager.AppSettings["SendSMS:Brandname"];

            if (apiKey == null || secretKey == null || brandName == null || requestlink == null)
            {
                string errorMessageSendSMS = "NotOK";
                ViewBag.errorMessageSendSMS = errorMessageSendSMS;
            }
            else
            {
                ViewBag.errorMessageSendSMS = "";
            }
            var DauSoCloudFone = SqlHelper.QuerySP<string>("spSystem_User_SelectDauSoCloudFoneById", new
            {
                pId = WebSecurity.CurrentUserId
            }).FirstOrDefault();
            ViewBag.DauSoCloudFone = DauSoCloudFone;
            //Tìm người chịu trách nhiệm của Lead
            int personInChargeID = lead != null ? (lead.AssignedUserId != null ? lead.AssignedUserId.Value : lead.CreatedUserId.Value) : 13467;
            var personInCharge = SqlHelper.QuerySP<User>("spSale_AdviseCard_GetUser_ById", new
            {
                pId = personInChargeID
            }).FirstOrDefault();
            ViewBag.personInCharge = personInCharge;
            SectionCusModel sectionCusModel = new SectionCusModel();
            sectionCusModel.Tuple = section;

            return PartialView("QuickSearchLeadModal", sectionCusModel);
        }
        #endregion

        #region Nghĩa - Phục hồi Lead
        public ActionResult LeadRecoverView()
        {
            return View();
        }
        [AllowAnonymous]
        [HttpPost]
        public ActionResult SearchLeadRecover(LeadSearchModel leadModel, int pageNumber, int pageSize)
        {
            var checkIsLetan = SqlHelper.QuerySP<int>("checkUserIsLetan", new
            {
                @pId = WebSecurity.CurrentUserId
            }).ToList();
            var model = SqlHelper.QuerySP<LeadModel>("LoadLeadCoverIndex", new
            {
                @pId = WebSecurity.CurrentUserId,
                @pIsLeTan = (checkIsLetan.Count > 0) ? 1 : 0
            });
            int ipageNumber = (pageNumber != null ? (int)pageNumber : 1);
            if (leadModel.keyValues != null && leadModel.keyValues.Length > 0)
            {
                foreach (var item in leadModel.keyValues)
                {
                    if (item.Key.Split(',').Length > 1 && item.Key.Split(',')[1] == "Start" && !string.IsNullOrEmpty(item.Value))
                    {
                        model = model.Where(x => typeof(LeadModel).GetProperty(item.Key.Split(',')[0]).GetValue(x) != null && typeof(LeadModel).GetProperty(item.Key.Split(',')[0]).GetValue(x).ToString() != "" ? DateTime.Parse(typeof(LeadModel).GetProperty(item.Key.Split(',')[0]).GetValue(x).ToString()) >= DateTime.Parse(item.Value) : false);
                    }
                    else if (item.Key.Split(',').Length > 1 && item.Key.Split(',')[1] == "End" && !string.IsNullOrEmpty(item.Value))
                    {
                        model = model.Where(x => typeof(LeadModel).GetProperty(item.Key.Split(',')[0]).GetValue(x) != null && typeof(LeadModel).GetProperty(item.Key.Split(',')[0]).GetValue(x).ToString() != "" ? DateTime.Parse(typeof(LeadModel).GetProperty(item.Key.Split(',')[0]).GetValue(x).ToString()) <= DateTime.Parse(item.Value) : false);

                    }
                    else if (item.ValueArr != null && item.ValueArr.Length > 0) //Helpers.Common.ChuyenThanhKhongDau(q[i].CompanyName);
                    {
                        //thanh
                        if (item.Key == "AssignedUserId")
                        {
                            model = model.Where(x =>
                            {
                                var itemKeyValue = typeof(LeadModel).GetProperty(item.Key).GetValue(x);
                                var createdUserIdValue = typeof(LeadModel).GetProperty("CreatedUserId").GetValue(x);

                                return (itemKeyValue != null ? item.ValueArr.Any(y => itemKeyValue.ToString().Contains(y)) : (createdUserIdValue != null ? item.ValueArr.Any(y => createdUserIdValue.ToString().Contains(y)) : false)
                                );
                            });
                        }
                        else if (item.Key == "StatusId")

                        {
                            model = model.Where(x =>
                            {
                                var propertyValue = typeof(LeadModel).GetProperty(item.Key).GetValue(x);
                                if (propertyValue != null)
                                {
                                    return item.ValueArr.Any(y => propertyValue.ToString().Equals(y));
                                }
                                return false;
                            });
                        }
                        else
                        {
                            model = model.Where(x => typeof(LeadModel).GetProperty(item.Key).GetValue(x) != null ? item.ValueArr.Any(y => typeof(LeadModel).GetProperty(item.Key).GetValue(x).ToString().Contains(y)) : false);
                        }
                    }
                }
            }
            var props = typeof(LeadModel);
            foreach (var item in props.GetProperties())
            {
                if (item.PropertyType == typeof(String) && item.GetValue(leadModel) != null && item.GetValue(leadModel).ToString() != "")
                {
                    model = model.Where(x => item.GetValue(x) != null ? Helpers.Common.ChuyenThanhKhongDau(item.GetValue(x).ToString()).ToLower().Trim().Contains(Helpers.Common.ChuyenThanhKhongDau(item.GetValue(leadModel).ToString()).ToLower().Trim()) : false);

                }
                else if (item.PropertyType == typeof(Int32?) && item.GetValue(leadModel) != null)
                {
                    model = model.Where(x => item.GetValue(x) != null ? item.GetValue(x).ToString() == item.GetValue(leadModel).ToString() : false);
                }
            }
            IPagedList pagedList = new PagedList<LeadModel>(model, pageNumber, pageSize);

            var result = LeadPartialView(model.Skip((ipageNumber - 1) * pageSize).Take(pageSize), pagedList, null, -1);
            ControllerContext.RouteData.Values["action"] = "LeadPartialView";
            return result;
        }
        [AllowAnonymous]
        [HttpPost]
        public JsonResult RecoverLead(int[] Id)
        {
            try
            {
                foreach (int i in Id)
                {
                    SqlHelper.ExecuteSP("RecoverLead", new
                    {
                        @pId = i,
                        @pModifiedUserId = WebSecurity.CurrentUserId
                    });
                }
            }
            catch
            {
                return Json(0);
            }
            return Json(1);
        }
        [AllowAnonymous]
        [HttpPost]
        public JsonResult RecoverCustomer(int[] Id)
        {
            try
            {
                foreach (int i in Id)
                {
                    SqlHelper.ExecuteSP("RecoverCustomer", new
                    {
                        @pId = i,
                        @pModifiedUserId = WebSecurity.CurrentUserId
                    });
                }
            }
            catch
            {
                return Json(0);
            }
            return Json(1);
        }
        [AllowAnonymous]
        [HttpPost]
        public JsonResult DeleteAllLeadInRecover(int[] Id)
        {
            try
            {
                foreach (int i in Id)
                {
                    SqlHelper.ExecuteSP("DeleteAllLeadInRecover", new
                    {
                        @pId = i,
                    });
                }
            }
            catch
            {
                return Json(0);
            }
            return Json(1);
        }
        [AllowAnonymous]
        [HttpGet]
        public ActionResult SearchCustomerRecover()
        {
            var checkIsLetan = SqlHelper.QuerySP<int>("checkUserIsLetan", new
            {
                @pId = WebSecurity.CurrentUserId
            }).ToList();
            var customerData = SqlHelper.QuerySP<CustomerViewModel>("LoadCustumerIsDeleted", new
            {
                @pId = WebSecurity.CurrentUserId,
                @pIsLeTan = (checkIsLetan.Count > 0) ? 1 : 0
            });
            ViewBag.CustomerData = customerData;

            return PartialView("CustomerPartialView");
        }
        [AllowAnonymous]
        [HttpPost]
        public JsonResult DeleteAllCustomerRecover(int[] Id)
        {
            try
            {
                foreach (int i in Id)
                {
                    SqlHelper.ExecuteSP("DeleteAllCustomerRecover", new
                    {
                        @pId = i,
                    });
                }
            }
            catch
            {
                return Json(0);
            }
            return Json(1);
        }
        #endregion

        #region Nghĩa - Update Chưa mua - Đã mua

        [AllowAnonymous]
        [HttpGet]
        public JsonResult KetThucLeadCategoryGet()
        {
            try
            {
                var ketThucLeadCategory = SqlHelper.QuerySP("Crm_KetThucLeadCategory_Get", new { }).ToList();
                var jsonDataList = new List<object>();

                foreach (var result in ketThucLeadCategory)
                {
                    var jsonData = new
                    {
                        Name = result.Name,
                        Value = result.Value,
                        Code = result.Code
                    };

                    jsonDataList.Add(jsonData);
                }

                // Trả về dữ liệu JSON
                return Json(jsonDataList, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                // Xử lý nếu có lỗi xảy ra
                Console.WriteLine("Error: " + ex.Message);
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }
        [AllowAnonymous]
        [HttpGet]
        public JsonResult OriginCategoryGet()
        {
            try
            {
                var originCategoryGet = SqlHelper.QuerySP("Crm_OriginCategory_Get", new { }).ToList();
                var jsonDataList = new List<object>();

                foreach (var result in originCategoryGet)
                {
                    var jsonData = new
                    {
                        Name = result.Name,
                        Value = result.Value,
                        Code = result.Code
                    };

                    jsonDataList.Add(jsonData);
                }
                // Trả về dữ liệu JSON
                return Json(jsonDataList, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                // Xử lý nếu có lỗi xảy ra
                Console.WriteLine("Error: " + ex.Message);
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }
        [AllowAnonymous]
        [HttpGet]
        public JsonResult EndDateAndToTalMoneyGet(int leadId)
        {
            try
            {
                var endDateAndToTalMoneyGet = SqlHelper.QuerySP("[Crm_EndDateAndToTalMoney_Get]", new { @pLeadId = leadId }).FirstOrDefault();
                var jsonDataList = new List<object>();
                DateTime dateValue;
                if (DateTime.TryParse(endDateAndToTalMoneyGet.F42, out dateValue))
                {
                    endDateAndToTalMoneyGet.F42 = dateValue.ToString("yyyy-MM-dd");
                }

                var jsonData = new
                {
                    TotalMoney = endDateAndToTalMoneyGet.TotalMoney,
                    NgayKetThuc = endDateAndToTalMoneyGet.F42,
                };

                // Trả về dữ liệu JSON
                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                // Xử lý nếu có lỗi xảy ra
                Console.WriteLine("Error: " + ex.Message);
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }
        [AllowAnonymous]
        [HttpPost]
        public ActionResult SaveDataCustomerPurchase(FormCollection formData)
        {
            // Lấy dữ liệu từ formData
            string amount = formData["amount"];
            string endDate = formData["endDate"];
            string reason = formData["reason"];
            string brand = formData["brand"];
            string F42 = formData["endDate"];
            DateTime dateValue;
            if (DateTime.TryParse(formData["endDate"], out dateValue))
            {
                F42 = dateValue.ToString("dd/MM/yyyy");
            }
            int? leadId = Convert.ToInt32(formData["leadId"]);
            //int statusId = Convert.ToInt32(formData["statusId"]);
            try
            {
                var customerPurchase = SqlHelper.QuerySP<int>("sp_Crm_CustomerPurchase_Update", new
                {
                    // pStatusId = statusId,
                    pTotalMoney = amount == "NaN" ? null : amount,
                    pTimeEnd = endDate,
                    pLeadId = leadId,
                    pTheEnd = reason,
                    pCountForBrand = brand,
                    pF44 = amount == "NaN" ? null : amount,
                    pF42 = F42
                }).FirstOrDefault();
                return Json(new { success = true, message = "Dữ liệu đã được lưu thành công." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Đã xảy ra lỗi khi lưu dữ liệu: " + ex.Message });
            }
        }

        [AllowAnonymous]
        [HttpPost]
        public JsonResult SaveForEndStatus(FormCollection formData)
        {
            // Lấy dữ liệu từ formData
            string amount = formData["amount"];
            string endDate = formData["endDate"];
            string reason = formData["reason"];
            string brand = formData["brand"];
            string F42 = formData["endDate"];
            DateTime dateValue;
            if (DateTime.TryParse(formData["endDate"], out dateValue))
            {
                F42 = dateValue.ToString("dd/MM/yyyy");
            }
            int leadId = Convert.ToInt32(formData["leadId"]);
            try
            {
                var customerPurchase = SqlHelper.QuerySP<int>("sp_Crm_CustomerPurchase_Update", new
                {
                    pTotalMoney = amount == "NaN" ? null : amount,
                    pTimeEnd = endDate,
                    pLeadId = leadId,
                    pTheEnd = reason,
                    pCountForBrand = brand,
                    pF44 = amount == "NaN" ? null : amount,
                    pF42 = F42
                }).FirstOrDefault();
                return Json(new { success = true, message = "Dữ liệu đã được lưu thành công." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Đã xảy ra lỗi khi lưu dữ liệu: " + ex.Message });
            }
        }

        #endregion
        [HttpPost]
        public PartialViewResult UpdateStatusLeadForCheckRight()
        {
            return PartialView();
        }
        [AllowAnonymous]
        [HttpPost]
        public JsonResult DeleteLeadById(int id)
        {
            try
            {
                {
                    SqlHelper.ExecuteSP("DeleteLead", new
                    {
                        @pId = id,
                        @pModifiedUserId = WebSecurity.CurrentUserId
                    });
                }
            }
            catch
            {
                return Json(0);
            }
            return Json(1);
        }
        [HttpPost]
        [AllowAnonymous]
        public JsonResult GetCountList(LeadSearchModel leadModel, int ldcusview = 0)
        {
            var checkIsLetan = SqlHelper.QuerySP<int>("checkUserIsLetan", new
            {
                @pId = WebSecurity.CurrentUserId
            }).ToList();
            int? chkacti = null;
            string chkcreated = null;
            string chkmodified = null;
            if (leadModel.keyValues != null && leadModel.keyValues.Length > 0)
            {
                foreach (var item in leadModel.keyValues)
                {
                    if (item.Key == "Activities")
                    {
                        chkacti = string.IsNullOrEmpty(item.Value) ? (int?)null : item.Value == "1" ? 1 : 0;
                    }
                    if (item.Key == "ModifiedBy")
                    {
                        chkmodified = item.Value;
                    }
                    if (item.Key == "CreatedBy")
                    {
                        chkcreated = item.Value;
                    }

                }
            }
            var model = SqlHelper.QuerySP<LeadModel>("LoadLeadIndex", new
            {
                @pId = WebSecurity.CurrentUserId,
                @pIsLeTan = (checkIsLetan.Count > 0) ? 1 : 0,
                @pIsLead = ldcusview,
                @chkacti = chkacti
            });
            if (leadModel.keyValues != null && leadModel.keyValues.Length > 0)
            {
                foreach (var item in leadModel.keyValues)
                {
                    if (item.Key.Split(',').Length > 1 && item.Key.Split(',')[1] == "Start" && !string.IsNullOrEmpty(item.Value))
                    {
                        model = model.Where(x => typeof(LeadModel).GetProperty(item.Key.Split(',')[0]).GetValue(x) != null && typeof(LeadModel).GetProperty(item.Key.Split(',')[0]).GetValue(x).ToString() != "" ? DateTime.Parse(typeof(LeadModel).GetProperty(item.Key.Split(',')[0]).GetValue(x).ToString()) >= DateTime.Parse(item.Value) : false);
                    }
                    else if (item.Key.Split(',').Length > 1 && item.Key.Split(',')[1] == "End" && !string.IsNullOrEmpty(item.Value))
                    {
                        model = model.Where(x => typeof(LeadModel).GetProperty(item.Key.Split(',')[0]).GetValue(x) != null && typeof(LeadModel).GetProperty(item.Key.Split(',')[0]).GetValue(x).ToString() != "" ? DateTime.Parse(typeof(LeadModel).GetProperty(item.Key.Split(',')[0]).GetValue(x).ToString()) <= DateTime.Parse(item.Value) : false);

                    }
                    else if (item.Key == "CreatedByFrom" && string.IsNullOrEmpty(chkcreated) && !string.IsNullOrEmpty(item.Value))
                    {
                        model = model.Where(x => x.CreatedDate >= DateTime.Parse(item.Value));
                    }
                    else if (item.Key == "CreatedByTo" && string.IsNullOrEmpty(chkcreated) && !string.IsNullOrEmpty(item.Value))
                    {
                        model = model.Where(x => x.CreatedDate <= DateTime.Parse(item.Value));

                    }
                    else if (item.Key == "ModifiedByFrom" && string.IsNullOrEmpty(chkmodified) && !string.IsNullOrEmpty(item.Value))
                    {
                        model = model.Where(x => x.ModifiedDate >= DateTime.Parse(item.Value));

                    }
                    else if (item.Key == "ModifiedByTo" && string.IsNullOrEmpty(chkmodified) && !string.IsNullOrEmpty(item.Value))
                    {
                        model = model.Where(x => x.ModifiedDate <= DateTime.Parse(item.Value));

                    }
                    else if (item.Key == "CreatedBy" && !string.IsNullOrEmpty(chkcreated) && !string.IsNullOrEmpty(item.Value))
                    {
                        var daynow = DateTime.Now;
                        int dayinw = (int)daynow.DayOfWeek;
                        DateTime mond = daynow.Date.AddDays(-dayinw + 1);
                        switch (item.Value)
                        {
                            case "0":
                                model = model.Where(x => x.CreatedDate != null ? x.CreatedDate.Value.Date == daynow.Date.AddDays(-1) : false);
                                break;
                            case "1":
                                model = model.Where(x => x.CreatedDate != null ? x.CreatedDate.Value.Date == daynow.Date : false);
                                break;
                            case "2":
                                model = model.Where(x => x.CreatedDate != null ? x.CreatedDate.Value.Date >= mond &&
                                x.CreatedDate.Value.Date <= mond.AddDays(6).AddHours(23).AddMinutes(59) : false);
                                break;
                            default:
                                break;
                        }

                    }
                    else if (item.Key == "ModifiedBy" && !string.IsNullOrEmpty(chkmodified) && !string.IsNullOrEmpty(item.Value))
                    {
                        var daynow = DateTime.Now;
                        int dayinw = (int)daynow.DayOfWeek;
                        DateTime mond = daynow.Date.AddDays(-dayinw + 1);
                        switch (item.Value)
                        {
                            case "0":
                                model = model.Where(x => x.ModifiedDate != null ? x.ModifiedDate.Value.Date == daynow.Date.AddDays(-1) : false);
                                break;
                            case "1":
                                model = model.Where(x => x.ModifiedDate != null ? x.ModifiedDate.Value.Date == daynow.Date : false);
                                break;
                            case "2":
                                model = model.Where(x => x.ModifiedDate != null ? x.ModifiedDate.Value.Date >= mond &&
                                x.ModifiedDate.Value.Date <= mond.AddDays(6).AddHours(23).AddMinutes(59) : false);
                                break;
                            default:
                                break;
                        }
                    }
                    else if (item.ValueArr != null && item.ValueArr.Length > 0) //Helpers.Common.ChuyenThanhKhongDau(q[i].CompanyName);
                    {
                        //thanh
                        if (item.Key == "AssignedUserId")
                        {
                            model = model.Where(x =>
                            {
                                var itemKeyValue = typeof(LeadModel).GetProperty(item.Key).GetValue(x);
                                var createdUserIdValue = typeof(LeadModel).GetProperty("CreatedUserId").GetValue(x);

                                return (itemKeyValue != null ? item.ValueArr.Any(y => itemKeyValue.ToString().Contains(y)) : (createdUserIdValue != null ? item.ValueArr.Any(y => createdUserIdValue.ToString().Contains(y)) : false)
                                );
                            });
                        }
                        else if (item.Key == "StatusId")

                        {
                            model = model.Where(x =>
                            {
                                var propertyValue = typeof(LeadModel).GetProperty(item.Key).GetValue(x);
                                if (propertyValue != null)
                                {
                                    return item.ValueArr.Any(y => propertyValue.ToString().Equals(y));
                                }
                                return false;
                            });
                        }

                        else
                        {
                            model = model.Where(x => typeof(LeadModel).GetProperty(item.Key)?.GetValue(x) != null ? item.ValueArr.Any(y => typeof(LeadModel).GetProperty(item.Key).GetValue(x).ToString().Contains(y)) : false);
                        }
                    }
                }
            }
            var props = typeof(LeadModel);
            try
            {
                foreach (var item in props.GetProperties())
                {
                    if (item.PropertyType == typeof(String) && item.GetValue(leadModel) != null && item.GetValue(leadModel).ToString() != "")
                    {
                        model = model.Where(x => item.GetValue(x) != null ? Helpers.Common.ChuyenThanhKhongDau(item.GetValue(x).ToString()).ToLower().Trim().Contains(Helpers.Common.ChuyenThanhKhongDau(item.GetValue(leadModel).ToString()).ToLower().Trim()) : false);

                    }
                    else if (item.PropertyType == typeof(Int32?) && item.GetValue(leadModel) != null)
                    {
                        model = model.Where(x => item.GetValue(x) != null ? item.GetValue(x).ToString() == item.GetValue(leadModel).ToString() : false);
                    }
                }

            }
            catch (Exception ex)
            {

            }

            int totalCount = model.Count(); // Lấy tổng số bản ghi

            // Trả về JSON chứa tổng số bản ghi
            return Json(new { Count = totalCount });
        }

        [AllowAnonymous]
        [HttpPost]
        public JsonResult CheckExistedMobileLead(string mobile)
        {
            try
            {
                var existedMobileLead = SqlHelper.QuerySP<int>("[sp_Crm_ExistedMobileLead_Check]", new
                {
                    pMobile = mobile
                }).FirstOrDefault();
                if (existedMobileLead > 0 || string.IsNullOrEmpty(mobile))
                    return Json(new { success = true, message = "Chưa tồn tại số điện thoại này." });
                else
                    return Json(new { success = false, message = "Đã tồn tại số điện thoại này." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Đã xảy ra lỗi khi lưu dữ liệu: " + ex.Message });
            }
        }
    }

}
