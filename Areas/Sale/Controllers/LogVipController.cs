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
using System.Net;
using Erp.Domain.Account.Helper;
using System.Transactions;
using System.Web;
using PagedList;
using iTextSharp.text.pdf.qrcode;
using Erp.Domain.Account.Interfaces;
//using Erp.Domain.Sale;

namespace Erp.BackOffice.Sale.Controllers
{
    [Authorize]
    [InitializeSimpleMembership]
    [Erp.BackOffice.Helpers.NoCacheHelper]
    public class LogVipController : Controller
    {
        private readonly ILogVipRepository LogVipRepository;
        private readonly IUserRepository userRepository;
        private readonly ICustomerRepository customerRepository;
        private readonly ITemplatePrintRepository templatePrintRepository;

        public LogVipController(
            ILogVipRepository _LogVip
            , IUserRepository _user
            , ICustomerRepository _cus
             , ITemplatePrintRepository templatePrint
            )
        {
            LogVipRepository = _LogVip;
            userRepository = _user;
            customerRepository = _cus;
            templatePrintRepository = templatePrint;
        }

        #region Index

        public ViewResult Index(string CustomerInfo, string status, int? year )
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

            IEnumerable<LogVipViewModel> q = SqlHelper.QuerySP<LogVipViewModel>("TOIUUCSKH_AllvwAllLogVip");
            //if (!string.IsNullOrEmpty(Code))
            //{
            //    q = q.Where(x => x.CustomerCode == Code).ToList();
            //}

            //if (!string.IsNullOrEmpty(TEN))
            //{
            //    TEN = Helpers.Common.ChuyenThanhKhongDau(TEN);
            //    q = q.Where(x => Helpers.Common.ChuyenThanhKhongDau(x.CustomerName).Contains(TEN)).ToList();
            //}
            if(intBrandID > 0)
            {
                q = q.Where(x => x.BranchId == intBrandID).ToList();
            }

            if (!string.IsNullOrEmpty(CustomerInfo))
            {
                q = q.Where(x => Helpers.Common.ChuyenThanhKhongDau(x.CustomerName).Contains(Helpers.Common.ChuyenThanhKhongDau(CustomerInfo)) || x.CustomerCode == CustomerInfo).ToList();
            }

            if (!string.IsNullOrEmpty(status))
            {
                if (status == "1")
                {
                    q = q.Where(x => x.Status == "Đang sử dụng").ToList();
                }
                else
                {
                    q = q.Where(x => x.Status == "Đã sử dụng").ToList();
                }

            }
            if(year > 0)
            {
                q = q.Where(x => x.Year == year).ToList();
            }
            ViewBag.SuccessMessage = TempData["SuccessMessage"];
            ViewBag.FailedMessage = TempData["FailedMessage"];
            ViewBag.AlertMessage = TempData["AlertMessage"];
            return View(q);
        }
        #endregion

        #region Create
        public ViewResult Create()
        {
            var model = new LogVipViewModel();

            return View(model);
        }

        [HttpPost]
        public ActionResult Create(LogVipViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var cus = customerRepository.GetvwCustomerByCode(model.CustomerCode);
                    model.CustomerId = cus.Id;
                    var LogVip = new LogVip();
                    AutoMapper.Mapper.Map(model, LogVip);
                    LogVip.IsDeleted = false;
                    LogVip.CreatedUserId = WebSecurity.CurrentUserId;
                    LogVip.ModifiedUserId = WebSecurity.CurrentUserId;
                    LogVip.AssignedUserId = WebSecurity.CurrentUserId;
                    LogVip.CreatedDate = DateTime.Now;
                    LogVip.ModifiedDate = DateTime.Now;
                    
                    LogVip.TotalAmount = model.TotalAmount;
                    //LogVip.CustomerName = model.CustomerName;
                    var check = Request["group_choice"];
                    LogVip.Status = check;
                    LogVip.is_approved = model.is_approved;
                    LogVip.ApprovedUserId = WebSecurity.CurrentUserId;


                    LogVipRepository.InsertLogVip(LogVip);
                    TempData[Globals.SuccessMessageKey] = App_GlobalResources.Wording.InsertSuccess;
                    return RedirectToAction("Index");
                }
                return View(model);
            }
            catch (DbUpdateException)
            {
                TempData[Globals.FailedMessageKey] = App_GlobalResources.Error.RelationError;
                return RedirectToAction("Index");
            }

        }
        #endregion

        #region Edit
        public ActionResult Edit(int? Id)
        {
            var LogVip = LogVipRepository.GetvwLogVipById(Id.Value);
            if (LogVip != null && LogVip.IsDeleted != true)
            {
                var model = new LogVipViewModel();
                AutoMapper.Mapper.Map(LogVip, model);
                LogVip.CustomerId = model.CustomerId;
                var check = Request["group_choice"];
                LogVip.Status = check;
                LogVip.Ratings = model.Ratings;
                //LogVip.ApprovedDate = model.ApprovedDate.ToString("dd/MM/yyyy");
                LogVip.TotalAmount = model.TotalAmount;

                if (model.CreatedUserId != Helpers.Common.CurrentUser.Id && Helpers.Common.CurrentUser.UserTypeId != 1)
                {
                    TempData["FailedMessage"] = "NotOwner";
                    return RedirectToAction("Index");
                }

                return View(model);
            }
            if (Request.UrlReferrer != null)
                return Redirect(Request.UrlReferrer.AbsoluteUri);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult Edit(LogVipViewModel model)
        {
            if (ModelState.IsValid)
            {
                //if (Request["Submit"] == "Save")
                //{
                var LogVip = LogVipRepository.GetLogVipById(model.Id);
                AutoMapper.Mapper.Map(model, LogVip);
                LogVip.ModifiedUserId = WebSecurity.CurrentUserId;
                LogVip.ModifiedDate = DateTime.Now;
                var check = Request["group_choice"];
                LogVip.Status = check;
                LogVip.is_approved = model.is_approved;
                LogVip.ApprovedUserId = WebSecurity.CurrentUserId;
                LogVipRepository.UpdateLogVip(LogVip);
                TempData[Globals.SuccessMessageKey] = App_GlobalResources.Wording.UpdateSuccess;
                return RedirectToAction("Index");
                //}

                //return RedirectToAction("Index");
            }

            return View(model);

            //if (Request.UrlReferrer != null)
            //    return Redirect(Request.UrlReferrer.AbsoluteUri);
            //return RedirectToAction("Index");
        }

        #endregion

        #region Detail
        public ActionResult Detail(int Id)
        {
            var LogVip = LogVipRepository.GetLogVipById(Id);
            if (LogVip != null && LogVip.IsDeleted != true)
            {
                var model = new LogVipViewModel();
                AutoMapper.Mapper.Map(LogVip, model);

                if (model.CreatedUserId != Helpers.Common.CurrentUser.Id && Helpers.Common.CurrentUser.UserTypeId != 1)
                {
                    TempData["FailedMessage"] = "NotOwner";
                    return RedirectToAction("Index");
                }

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
                string idDeleteAll = Request["DeleteId-checkbox"];
                string[] arrDeleteId = idDeleteAll.Split(',');
                for (int i = 0; i < arrDeleteId.Count(); i++)
                {
                    var item = LogVipRepository.GetLogVipById(int.Parse(arrDeleteId[i], CultureInfo.InvariantCulture));
                    if (item != null)
                    {
                        if (item.CreatedUserId != Helpers.Common.CurrentUser.Id && Helpers.Common.CurrentUser.UserTypeId != 1)
                        {
                            TempData["FailedMessage"] = "NotOwner";
                            return RedirectToAction("Index");
                        }
                        item.IsDeleted = true;
                        LogVipRepository.UpdateLogVip(item);
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


        #region ChangeStatus
        public ActionResult ChangeStatus(int Id)
        {
            var logvip = LogVipRepository.GetLogVipById(Id);
            if (logvip != null)
            {
                var year = logvip.Year;
                if (year == DateTime.Now.Year)
                    logvip.Status = "Đang sử dụng";
                else
                    logvip.Status = "Đã sử dụng";
                logvip.ModifiedUserId = WebSecurity.CurrentUserId;
                logvip.ModifiedDate = DateTime.Now;
                LogVipRepository.UpdateLogVip(logvip);

                TempData[Globals.SuccessMessageKey] = "Thay đổi thành công!";
            }
            return RedirectToAction("Index");
        }
        #endregion
        #region Rank
        public ActionResult Rank(int Id)
        {
            var logvip = LogVipRepository.GetvwLogVipById(Id);
            if (logvip != null)
            {
                var rank = logvip.TotalAmount;
                if (rank >= 1000000 && rank <= 10000000)
                    logvip.LoyaltyPointName = "Hạng bạc";
                else
                  if (rank >= 10000000 && rank <= 20000000)
                    logvip.LoyaltyPointName = "Hạng vàng";
                else
                         if (rank >= 20000000 && rank <= 40000000)
                    logvip.LoyaltyPointName = "Hạng kim cương";
                else
                            if (rank >= 40000000 && rank <= 100000000)
                    logvip.LoyaltyPointName = "Hạng Platinum";
                else
                                if (rank == 1000000)
                    logvip.LoyaltyPointName = "Hạng đồng";
                else
                                     if (rank >= 10000000 && rank <= 50000000)
                    logvip.LoyaltyPointName = "Hạng cao thủ";

                logvip.ModifiedUserId = WebSecurity.CurrentUserId;
                logvip.ModifiedDate = DateTime.Now;
                //LogVipRepository.UpdateLogVip(logvip);

                TempData[Globals.SuccessMessageKey] = "Thay đổi thành công!";
            }
            return RedirectToAction("Index");
        }
        #endregion



        #region Search
        public ActionResult Search(int? year, int? branchId,int? page)
        {

            branchId = branchId == null ? 0 : branchId;
            year = year == null ? DateTime.Now.Year : year;
            //if (!Filters.SecurityFilter.IsAdmin() && branchId == 0)
            //{
            //    branchId = Helpers.Common.CurrentUser.BranchId;
            //}
            page = page ?? 1;
            int pageNumber = (page ?? 1);
            
            var data = SqlHelper.QuerySP<SearchLogVipViewModel>("spGetlenhang", new
            {
                branchId = branchId,
                year = year
            });

            ViewBag.data = data.ToPagedList(pageNumber, 50);
            return View(data);
        }
        #endregion
        #region Change
        [HttpPost]
        public ActionResult Change(int? branchId, int? year)
        {
            branchId = branchId == null ? 0 : branchId;
            year = year == null ? DateTime.Now.Year : year;
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
            string idDeleteAll = Request["DeleteId-checkbox"];
            string[] arrDeleteId = null;
            arrDeleteId = idDeleteAll.Split(',');
            var LogVip = new LogVip();// LogVipRepository.GetLogVipById(Id);

            if (arrDeleteId != null)
            {
                for (int i = 0; i < arrDeleteId.Count(); i++)
                {
                    int CusId = int.Parse(arrDeleteId[i], CultureInfo.InvariantCulture);
                    var data = SqlHelper.QuerySP<SearchLogVipViewModel>("spGetlenhang", new
                    {
                        branchId = branchId,
                        year = year
                    }).FirstOrDefault(x => x.CustomerId == CusId);
                    //Chuyển hạng của khách ở năm trước thành đã sử dụng
                    var trans = SqlHelper.QuerySP<SearchLogVipViewModel>("deleteHangvip", new
                    {
                        customerId = CusId,
                        year = year
                    }).FirstOrDefault();


                    //var data = LogVipRepository.GetLogVipById(int.Parse(arrDeleteId[i]));
                    if (data.loai == "moi")
                    {
                        var model = new LogVipViewModel();
                        LogVip = new LogVip();
                        //AutoMapper.Mapper.Map(ProductModel, LogVip);
                        LogVip.IsDeleted = false;
                        LogVip.CreatedUserId = WebSecurity.CurrentUserId;
                        LogVip.ModifiedUserId = WebSecurity.CurrentUserId;
                        LogVip.AssignedUserId = WebSecurity.CurrentUserId;
                        LogVip.CreatedDate = DateTime.Now;
                        LogVip.ModifiedDate = DateTime.Now;
                        //LogVip.Year = DateTime.Now.Year;
                        LogVip.TotalAmount = data.tongmua;
                        //LogVip.CustomerName = model.CustomerName;
                        var check = Request["group_choice"];
                        LogVip.Status = "Đang sử dụng";
                        LogVip.CustomerId = data.CustomerId;
                        LogVip.Year = data.NAM;
                        LogVip.ApprovedDate = DateTime.Now.Date;
                        LogVip.ApprovedUserId = WebSecurity.CurrentUserId;
                        LogVip.LoyaltyPointId = data.IdHang;
                        LogVip.is_approved = true;
                        LogVipRepository.InsertLogVip(LogVip);

                    }
                    else
                        if (data.loai == "lenhang")
                            {

                            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.Snapshot }))
                            {
                                LogVip = new LogVip();
                                var model = new LogVipViewModel();
                                //LogVip = LogVipRepository.GetLogVipById(CustomerId);
                                AutoMapper.Mapper.Map(model, LogVip);
                                LogVip.IsDeleted = false;
                                LogVip.CreatedUserId = WebSecurity.CurrentUserId;
                                LogVip.ModifiedUserId = WebSecurity.CurrentUserId;
                                LogVip.AssignedUserId = WebSecurity.CurrentUserId;
                                LogVip.CreatedDate = DateTime.Now;
                                LogVip.ModifiedDate = DateTime.Now;
                                LogVip.Year = DateTime.Now.Year;
                                LogVip.TotalAmount = data.tongmua;
                                var check = Request["group_choice"];
                                LogVip.Status = "Đang sử dụng";
                                LogVip.CustomerId = data.CustomerId;
                                LogVip.Year = data.NAM;
                                LogVip.LoyaltyPointId = data.IdHang;
                                LogVip.is_approved = true;
                                LogVip.ApprovedDate = DateTime.Now.Date;

                                LogVip.ApprovedUserId = WebSecurity.CurrentUserId;

                                LogVipRepository.InsertLogVip(LogVip);
                                LogVip = LogVipRepository.GetLogVipById(data.Idxephangcu);
                                //LogVip.ApprovedDate = model.ApprovedDate;
                                if (LogVip != null)
                                {
                                    LogVip.ApprovedDate = DateTime.Now.Date;
                                    LogVip.Status = "Đã sử dụng";
                                    LogVip.ModifiedUserId = WebSecurity.CurrentUserId;
                                    LogVip.ModifiedDate = DateTime.Now;
                                    LogVipRepository.UpdateLogVip(LogVip);
                                }
                                else
                                {
                                    TempData[Globals.SuccessMessageKey] = App_GlobalResources.Wording.UpdateSuccess;                              
                                }


                                  scope.Complete();
                            }
                        TempData[Globals.SuccessMessageKey] = App_GlobalResources.Wording.UpdateSuccess;
                        

                    }

                    //logvip.ModifiedUserId = WebSecurity.CurrentUserId;
                    //    logvip.ModifiedDate = DateTime.Now;
                    //    LogVipRepository.UpdateLogVip(logvip);

                    TempData[Globals.SuccessMessageKey] = "Thay đổi thành công!";

                  
                }
            }
            return RedirectToAction("Index");

        }
        #endregion

        #region exportexcel
        public ActionResult Export(string CustomerInfo, string status)
        {
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
            model.Content = model.Content.Replace("{DataTable}", buildHtml(CustomerInfo, status));
            model.Content = model.Content.Replace("{System.Logo}", ImgLogo);
            model.Content = model.Content.Replace("{System.CompanyName}", company);
            model.Content = model.Content.Replace("{System.AddressCompany}", address);
            model.Content = model.Content.Replace("{System.PhoneCompany}", phone);
            model.Content = model.Content.Replace("{System.Fax}", fax);
            model.Content = model.Content.Replace("{System.BankCodeCompany}", bankcode);
            model.Content = model.Content.Replace("{System.BankNameCompany}", bankname);
            model.Content = model.Content.Replace("{PrintDate}", DateTime.Now.ToString("dd/MM/yyyy HH:mm"));
            model.Content = model.Content.Replace("{Title}", "Danh sách khách VIP");

            Response.AppendHeader("content-disposition", "attachment;filename=" + "Danhsachkhachvip" + DateTime.Now.ToString("yyyyMMdd") + ".xls");
            Response.Charset = "";
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.Write(model.Content);
            Response.End();

            return View();
        }
        string buildHtml(string CustomerInfo, string status)
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

            IEnumerable<LogVipViewModel> q = LogVipRepository.GetvwAllLogVip()
                .Select(item => new LogVipViewModel
                {
                    Id = item.Id,
                    CreatedDate = item.CreatedDate,
                    CreatedUserId = item.CreatedUserId,
                    CustomerId = item.CustomerId,
                    ModifiedUserId = item.ModifiedUserId,
                    ModifiedDate = item.ModifiedDate,
                    ApprovedUserId = item.ApprovedUserId,
                    ApprovedUserName = item.ApprovedUserName,
                    ApprovedDate = item.ApprovedDate,
                    CustomerCode = item.CustomerCode,
                    TotalAmount = item.TotalAmount,
                    Status = item.Status,
                    Year = item.Year,
                    LoyaltyPointId = item.LoyaltyPointId,
                    CustomerName = item.CustomerName,
                    is_approved = item.is_approved,
                    LoyaltyPointName = item.LoyaltyPointName,
                    BranchId = item.BranchId
                }).OrderByDescending(m => m.ModifiedDate).ToList();
            //if (!string.IsNullOrEmpty(Code))
            //{
            //    q = q.Where(x => x.CustomerCode == Code).ToList();
            //}
            //if (!string.IsNullOrEmpty(TEN))
            //{
            //    TEN = Helpers.Common.ChuyenThanhKhongDau(TEN);
            //    q = q.Where(x => Helpers.Common.ChuyenThanhKhongDau(x.CustomerName).Contains(TEN)).ToList();

            //}
            if (intBrandID > 0)
            {
                q = q.Where(x => x.BranchId == intBrandID).ToList();
            }

            if (!string.IsNullOrEmpty(CustomerInfo))
            {
                q = q.Where(x => Helpers.Common.ChuyenThanhKhongDau(x.CustomerName).Contains(Helpers.Common.ChuyenThanhKhongDau(CustomerInfo)) || x.CustomerCode == CustomerInfo).ToList();
            }

            if (!string.IsNullOrEmpty(status))
            {
                if (status == "1")
                {
                    q = q.Where(x => x.Status == "Đang sử dụng").ToList();
                }
                else
                {
                    q = q.Where(x => x.Status == "Đã sử dụng").ToList();
                }

            }

            string detailLists = "<table border=\"1\" class=\"invoice-detail\">\r\n";
            detailLists += "<thead>\r\n";
            detailLists += "	<tr>";
            detailLists += "		<th>Mã khách hàng</th>";
            detailLists += "		<th>Tên khách hàng</th>";
            detailLists += "		<th>Năm xếp hạng</th>";
            detailLists += "		<th>Xếp hạng VIP</th>";
            detailLists += "		<th>Người duyệt</th>";
            detailLists += "		<th>Ngày duyệt</th>";
            detailLists += "		<th>Tổng tiền</th>";
            detailLists += "		<th>Trạng thái</th>";
            detailLists += "		<th>Trạng thái duyệt</th>";
            detailLists += "	</tr>";
            detailLists += "</thead>\r\n";
            detailLists += "<tbody>\r\n";

            foreach (var item in q)
            {
                detailLists += "<tr>\r\n"

                + "<td class=\"text-left \">" + item.CustomerCode + "</td>\r\n"
                + "<td class=\"text-left \">" + item.CustomerName + "</td>\r\n"
                + "<td class=\"text-left \">" + item.Year + "</td>\r\n"
                + "<td class=\"text-left \">" + item.LoyaltyPointName + "</td>\r\n"
                + "<td class=\"text-left \">" + item.ApprovedUserName + "</td>\r\n"
                + "<td class=\"text-left \">" + item.ApprovedDate + "</td>\r\n"
                + "<td class=\"text-left \">" + CommonSatic.ToCurrencyStr(item.TotalAmount, null).Replace(".", ",") + "</td>\r\n"
                + "<td class=\"text-left \">" + item.Status + "</td>\r\n";
                
                if (item.is_approved == true) {
                    detailLists += "<td class=\"text-left \">Đã duyệt</td>\r\n";
                }
                else
                {
                    detailLists += "<td class=\"text-left \">Chưa duyệt</td>\r\n";
                }
                detailLists += "</tr>\r\n";
            }
            detailLists += "</tbody>\r\n";
            detailLists += "<tfoot style=\"font-weight:bold\">\r\n";
            detailLists += "</tfoot>\r\n</table>\r\n";
            return detailLists;
        }
        #endregion
    }


}
