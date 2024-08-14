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
using System.Transactions;
using Erp.Domain.Sale.Interfaces;
using Erp.Domain.Account.Helper;
using System.Web;
using Erp.BackOffice.Sale.Models;

namespace Erp.BackOffice.Account.Controllers
{
    [Authorize]
    [InitializeSimpleMembership]
    [Erp.BackOffice.Helpers.NoCacheHelper]
    public class LogReceiptController : Controller
    {
        private readonly ILogReceiptRepository LogReceiptRepository;
        private readonly IUserRepository userRepository;
        private readonly IReceiptRepository receiptRepository;
        private readonly ITemplatePrintRepository templatePrintRepository;
        public LogReceiptController(
            ILogReceiptRepository _LogReceipt
            , IUserRepository _user
            , IReceiptRepository receipt
            , ITemplatePrintRepository _templatePrint
            )
        {
            LogReceiptRepository = _LogReceipt;
            userRepository = _user;
            receiptRepository = receipt;
            templatePrintRepository = _templatePrint;
        }

        #region Index

        public ViewResult Index(string Code, string startDate, string endDate, string Status, int? BranchId, int? CreateUserId)
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

            IEnumerable<LogReceiptViewModel> q = LogReceiptRepository.GetAllLogReceipt()
                .Select(item => new LogReceiptViewModel
                {
                    Id = item.Id,
                    CreatedUserId = item.CreatedUserId,
                    //CreatedUserName = item.CreatedUserName,
                    CreatedDate = item.CreatedDate,
                    ModifiedUserId = item.ModifiedUserId,
                    //ModifiedUserName = item.ModifiedUserName,
                    ModifiedDate = item.ModifiedDate,
                    BranchId = item.BranchId,
                    Code = item.Code,
                    DateReceipt = item.DateReceipt,
                    Note = item.Note,
                    Status = item.Status,
                    TotalAmount = item.TotalAmount,
                    UserName = item.UserName,
                    Tongtienxacnhan = item.Tongtienxacnhan
                }).OrderByDescending(m => m.CreatedDate);
            if (!string.IsNullOrEmpty(Code))
            {
                Code = Helpers.Common.ChuyenThanhKhongDau(Code);
                q = q.Where(x => Helpers.Common.ChuyenThanhKhongDau(x.Code).Contains(Code)).ToList();
            }
            if (!string.IsNullOrEmpty(Status))
            {
                q = q.Where(x => x.Status == Status).ToList();
            }
            DateTime d_startDate, d_endDate;
            if (DateTime.TryParseExact(startDate, "dd/MM/yyyy", new CultureInfo("vi-VN"), DateTimeStyles.None, out d_startDate))
            {
                if (DateTime.TryParseExact(endDate, "dd/MM/yyyy", new CultureInfo("vi-VN"), DateTimeStyles.None, out d_endDate))
                {
                    d_endDate = d_endDate.AddHours(23).AddMinutes(59);
                    q = q.Where(x => x.DateReceipt >= d_startDate && x.DateReceipt <= d_endDate).ToList();
                }
            }
            if (BranchId != null && BranchId.Value > 0)
            {
                q = q.Where(x => x.BranchId == BranchId).ToList();
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

        #region Print
        public List<LogReceiptViewModel> IndexPrint(string Code, string startDate, string endDate, string Status, int? CreateUserId, int? BranchId)
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

            var q = LogReceiptRepository.GetAllLogReceipt()
            .Select(item => new LogReceiptViewModel
            {
                Id = item.Id,
                CreatedUserId = item.CreatedUserId,
                //CreatedUserName = item.CreatedUserName,
                CreatedDate = item.CreatedDate,
                ModifiedUserId = item.ModifiedUserId,
                //ModifiedUserName = item.ModifiedUserName,
                ModifiedDate = item.ModifiedDate,
                BranchId = item.BranchId,
                Code = item.Code,
                DateReceipt = item.DateReceipt,
                Note = item.Note,
                Status = item.Status,
                TotalAmount = item.TotalAmount,
                UserName = item.UserName,
                Tongtienxacnhan = item.Tongtienxacnhan
            }).OrderByDescending(m => m.CreatedDate).ToList();
            if (!string.IsNullOrEmpty(Code))
            {
                Code = Helpers.Common.ChuyenThanhKhongDau(Code);
                q = q.Where(x => Helpers.Common.ChuyenThanhKhongDau(x.Code).Contains(Code)).ToList();
            }
            if (!string.IsNullOrEmpty(Status))
            {
                q = q.Where(x => x.Status == Status).ToList();
            }
            DateTime d_startDate, d_endDate;
            if (DateTime.TryParseExact(startDate, "dd/MM/yyyy", new CultureInfo("vi-VN"), DateTimeStyles.None, out d_startDate))
            {
                if (DateTime.TryParseExact(endDate, "dd/MM/yyyy", new CultureInfo("vi-VN"), DateTimeStyles.None, out d_endDate))
                {
                    d_endDate = d_endDate.AddHours(23).AddMinutes(59);
                    q = q.Where(x => x.DateReceipt >= d_startDate && x.DateReceipt <= d_endDate).ToList();
                }
            }
            if (BranchId != null && BranchId.Value > 0)
            {
                q = q.Where(x => x.BranchId == BranchId).ToList();
            }
            if (CreateUserId != null && CreateUserId.Value > 0)
            {
                q = q.Where(x => x.CreatedUserId == CreateUserId).ToList();
            }
            return q;
        }

        public ActionResult PrintLogReceipt(string Code, string startDate, string endDate, string Status, int? CreateUserId, int? BranchId, bool ExportExcel = false)
        {
            var data = IndexPrint(Code, startDate, endDate, Status, CreateUserId, BranchId);

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
            model.Content = model.Content.Replace("{DataTable}", buildHtmlDanhSachLichSuNopTien(data));
            model.Content = model.Content.Replace("{System.Logo}", ImgLogo);
            model.Content = model.Content.Replace("{System.CompanyName}", company);
            model.Content = model.Content.Replace("{System.AddressCompany}", address);
            model.Content = model.Content.Replace("{System.PhoneCompany}", phone);
            model.Content = model.Content.Replace("{System.Fax}", fax);
            model.Content = model.Content.Replace("{System.BankCodeCompany}", bankcode);
            model.Content = model.Content.Replace("{System.BankNameCompany}", bankname);
            model.Content = model.Content.Replace("{PrintDate}", DateTime.Now.ToString("dd/MM/yyyy HH:mm"));
            model.Content = model.Content.Replace("{Title}", "Danh sách lịch sử nộp tiền");
            if (ExportExcel)
            {
                Response.AppendHeader("content-disposition", "attachment;filename=" + "DS_Lichsunoptien" + DateTime.Now.ToString("yyyyMMdd") + ".xls");
                Response.Charset = "";
                Response.Cache.SetCacheability(HttpCacheability.NoCache);
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.Write(model.Content);
                Response.End();
            }
            return View(model);
        }

        string buildHtmlDanhSachLichSuNopTien(List<LogReceiptViewModel> detailList)
        {
            string detailLists = "<table border=\"1\" class=\"invoice-detail\">\r\n";
            detailLists += "<thead>\r\n";
            detailLists += "	<tr>";
            detailLists += "		<th>STT</th>";
            detailLists += "		<th>Mã nộp tiền</th>";
            detailLists += "		<th>Ngày nộp tiền</th>";
            detailLists += "		<th>Người nộp</th>";
            detailLists += "		<th>Số tiền nộp</th>";
            detailLists += "		<th>Số tiền đã xác nhận</th>";
            detailLists += "		<th>Trạng thái</th>";
            detailLists += "		<th>Ghi chú</th>";
            detailLists += "	</tr>";
            detailLists += "</thead>\r\n";
            detailLists += "<tbody>\r\n";
            var index = 1;
            decimal total = 0;
            decimal total1 = 0;

            foreach (var item in detailList)
            {
                detailLists += "<tr>\r\n"
                + "<td class=\"text-center orderNo\">" + (index++) + "</td>\r\n"
                + "<td class=\"text-left \">" + item.Code + "</td>\r\n"
                + "<td>" + (item.DateReceipt.HasValue ? item.DateReceipt.Value.ToString("dd/MM/yyyy HH:mm") : "") + "</td>"
                + "<td class=\"text-left \">" + item.UserName + "</td>\r\n"
                + "<td class=\"text-left \">" + CommonSatic.ToCurrencyStr(item.TotalAmount, null).Replace(".", ",") + "</td>\r\n"
                + "<td class=\"text-left \">" + CommonSatic.ToCurrencyStr(item.Tongtienxacnhan, null).Replace(".", ",") + "</td>\r\n"
                + "<td class=\"text-left \">" + (item.Status == "pending" ? "Chưa xác nhận" : item.Status == "inprogress" ? "Chưa hoàn thành" : "Hoàn thành") + "</td>\r\n"
                + "<td class=\"text-left \">" + item.Note + "</td>\r\n"              
                + "</tr>\r\n";
                total += item.TotalAmount ?? 0;
                total1 += item.Tongtienxacnhan ?? 0;
            }

            detailLists += "<tr>\r\n"
               + "<td style=\"font-weight:bold \">Tổng cộng</td>"
               + "<td class=\"text-left \">" + "" + "</td>\r\n"
               + "<td class=\"text-left \">" + "" + "</td>\r\n"
               + "<td class=\"text-left \">" + "" + "</td>\r\n"
               + "<td class=\"text-left \" style=\"font-weight:bold \">" + CommonSatic.ToCurrencyStr(total, null).Replace(".", ",") + "</td>\r\n"
               + "<td class=\"text-left \" style=\"font-weight:bold \">" + CommonSatic.ToCurrencyStr(total1, null).Replace(".", ",") + "</td>\r\n"
               + "<td class=\"text-left \">" + "" + "</td>\r\n"
               + "<td class=\"text-left \">" + "" + "</td>\r\n"
               + "</tr>\r\n";


            detailLists += "</tbody>\r\n";
            detailLists += "<tfoot style=\"font-weight:bold\">\r\n";
            detailLists += "</tfoot>\r\n</table>\r\n";
            return detailLists;
        }
        #endregion

        #region Create
        public ViewResult Create(int? Id)
        {
            var model = new LogReceiptViewModel();
            model.DetailList = new List<ReceiptViewModel>();
            model.DateReceipt = DateTime.Now;
            if (Id != null)
            {
                var LogReceipt = LogReceiptRepository.GetLogReceiptById(Id.Value);
                if (LogReceipt != null && LogReceipt.IsDeleted != true)
                {
                    AutoMapper.Mapper.Map(LogReceipt, model);

                    model.DetailList = receiptRepository.GetAllReceipt().Where(x => x.LogReceiptId != null && x.LogReceiptId == Id).Select(x => new
                    ReceiptViewModel
                    {
                        Id = x.Id,
                        Amount = x.Amount,
                        CompanyName = x.CompanyName,
                        Code = x.Code,
                        LoaiChungTuGoc = x.LoaiChungTuGoc,
                        MaChungTuGoc = x.MaChungTuGoc,
                        VoucherDate = x.VoucherDate,
                        Name = x.Name,
                        SalerName = x.SalerName,
                        CreatedDate = x.CreatedDate,
                        LogReceiptId = x.LogReceiptId
                    }).ToList();
                    return View(model);
                }
            }
            return View(model);
        }





        [HttpPost]
        public ActionResult Create(LogReceiptViewModel model)
        {
            var a = 0;
            var b = 0;
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
            if (model.Id > 0)
            {
                sample = LogReceiptRepository.GetLogReceiptById(model.Id);
            }
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.Snapshot }))
            {
                
                try
                {

                    if (sample != null)
                    {
                        sample.Note = model.Note;
                        sample.TotalAmount = model.TotalAmount;
                        sample.BranchId = intBrandID;
                        sample.ModifiedUserId = WebSecurity.CurrentUserId;
                        sample.ModifiedDate = DateTime.Now;
                        LogReceiptRepository.UpdateLogReceipt(sample);
                    }
                    else
                    {
                        sample = new LogReceipt();
                        AutoMapper.Mapper.Map(model, sample);
                        sample.IsDeleted = false;
                        sample.CreatedUserId = WebSecurity.CurrentUserId;
                        sample.ModifiedUserId = WebSecurity.CurrentUserId;
                        sample.AssignedUserId = WebSecurity.CurrentUserId;
                        sample.CreatedDate = DateTime.Now;
                        sample.ModifiedDate = DateTime.Now;
                        sample.BranchId = intBrandID;
                        sample.Status = "pending";
                        sample.Code = Erp.BackOffice.Helpers.Common.GetOrderNo("LogReceipt", model.Code);
                        Erp.BackOffice.Helpers.Common.SetOrderNo("LogReceipt");
                        LogReceiptRepository.InsertLogReceipt(sample);
                    }
                    var _listdata = receiptRepository.GetAllReceipts().ToList();
                    var _remove = _listdata.Where(x => x.LogReceiptId != null && x.LogReceiptId == sample.Id).ToList();
                    var _update = _listdata.Where(id1 => model.DetailList.ToList().Any(id2 => id2.Id == id1.Id)).ToList();
                    a = _remove.Count();
                    b = _update.Count();
                    if (_remove.Any())
                    {
                        foreach (var item in _remove)
                        {
                            item.LogReceiptId = null;
                            //item.IsArchive = false;
                            receiptRepository.UpdateReceipt(item);
                        }
                    }
                    if (_update.Any())
                    {
                        foreach (var item in _update)
                        {
                            item.LogReceiptId = sample.Id;
                            receiptRepository.UpdateReceipt(item);

                        }
                    }
                        scope.Complete();
                }
             
                catch (DbUpdateException)
                {
                    TempData[Globals.FailedMessageKey] = App_GlobalResources.Wording.Error;
                    return View(model);
                }
            }
            if (a == b)
            {
                return RedirectToAction("Detail", new { Id = sample.Id });
            }

            return RedirectToAction("Create", new { Id = sample.Id });
        }
        #endregion

        #region Detail
        public ActionResult Detail(int? Id)
        {
            var LogReceipt = LogReceiptRepository.GetLogReceiptById(Id.Value);
            if (LogReceipt != null && LogReceipt.IsDeleted != true)
            {
                var model = new LogReceiptViewModel();
                AutoMapper.Mapper.Map(LogReceipt, model);
                model.DetailList = new List<ReceiptViewModel>();
                model.DetailList = receiptRepository.GetAllReceipt().Where(x => x.LogReceiptId != null && x.LogReceiptId == Id).Select(x => new
                ReceiptViewModel
                {
                    Id = x.Id,
                    Amount = x.Amount,
                    CompanyName = x.CompanyName,
                    Code = x.Code,
                    LoaiChungTuGoc = x.LoaiChungTuGoc,
                    MaChungTuGoc = x.MaChungTuGoc,
                    VoucherDate = x.VoucherDate,
                    Name = x.Name,
                    SalerName = x.SalerName,
                    IsArchive = x.IsArchive,
                    LogReceiptId = x.LogReceiptId
                }).ToList();
                return View(model);
            }
            if (Request.UrlReferrer != null)
                return Redirect(Request.UrlReferrer.AbsoluteUri);
            return RedirectToAction("Index");
        }
        #endregion

        #region Delete
        [HttpPost]
        public ActionResult Delete()
        {
            try
            {
                using (var scope = new TransactionScope(TransactionScopeOption.Required, new System.TimeSpan(0, 30, 0)))
                {
                    var _listdata = receiptRepository.GetAllReceipts().ToList();
                    string idDeleteAll = Request["Delete"];
                    string[] arrDeleteId = idDeleteAll.Split(',');
                    for (int i = 0; i < arrDeleteId.Count(); i++)
                    {
                        var item = LogReceiptRepository.GetLogReceiptById(int.Parse(arrDeleteId[i], CultureInfo.InvariantCulture));
                        if (item != null)
                        {
                            item.IsDeleted = true;
                            LogReceiptRepository.UpdateLogReceipt(item);
                        }
                        var _remove = _listdata.Where(x => x.LogReceiptId != null && x.LogReceiptId == item.Id).ToList();

                        foreach (var item1 in _remove)
                        {
                            item1.LogReceiptId = null;
                            item1.IsArchive = false;
                            receiptRepository.UpdateReceipt(item1);
                        }


                    }
                    TempData[Globals.SuccessMessageKey] = App_GlobalResources.Wording.DeleteSuccess;
                    scope.Complete();
                }

                return RedirectToAction("Index");
            }
            catch (DbUpdateException)
            {
                TempData[Globals.FailedMessageKey] = App_GlobalResources.Error.RelationError;
                return RedirectToAction("Index");
            }
        }
        #endregion
    }
}
