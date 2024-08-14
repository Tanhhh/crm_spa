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

namespace Erp.BackOffice.Account.Controllers
{
    [Authorize]
    [InitializeSimpleMembership]
    [Erp.BackOffice.Helpers.NoCacheHelper]
    public class PaymentController : Controller
    {
        private readonly IPaymentRepository paymentRepository;
        private readonly IUserRepository userRepository;
        private readonly ICustomerRepository customerRepository;
        private readonly ITemplatePrintRepository templatePrintRepository;
        private readonly ICategoryRepository categoryRepository;
        private readonly IPaymentDetailRepository paymentDetailRepository;
        public PaymentController(
            IPaymentRepository _Payment
            , IUserRepository _user
            , ICustomerRepository customer
            , ITemplatePrintRepository _templatePrint
            , ICategoryRepository category
            , IPaymentDetailRepository paymentDetail
            )
        {
            paymentRepository = _Payment;
            userRepository = _user;
            customerRepository = customer;
            templatePrintRepository = _templatePrint;
            categoryRepository = category;
            paymentDetailRepository = paymentDetail;
        }

        #region Index

        public ViewResult Index(int? SalerId, string Code, string ReceiverUserName)
        {
            var start = Request["start"];
            var end = Request["end"];
            var startDate = Request["startDate"];
            var endDate = Request["endDate"];
            IQueryable<PaymentViewModel> q = paymentRepository.GetAllvwPayment()
                .Select(item => new PaymentViewModel
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
                    TargetId = item.TargetId,
                    TargetName = item.TargetName,
                    Receiver = item.Receiver,
                    SalerId = item.SalerId,
                    SalerName = item.SalerName,
                    VoucherDate = item.VoucherDate,
                    ReceiverUserName = item.ReceiverUserName,
                    MaChungTuGoc = item.MaChungTuGoc,
                    LoaiChungTuGoc = item.LoaiChungTuGoc,
                    CancelReason = item.CancelReason,
                    IsDeleted = item.IsDeleted,
                    IsArchive = item.IsArchive
                }).OrderByDescending(m => m.CreatedDate);

            if (!string.IsNullOrEmpty(startDate) && !string.IsNullOrEmpty(endDate))
            {
                DateTime start_d;
                if (DateTime.TryParseExact(startDate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out start_d))
                {
                    DateTime end_d;
                    if (DateTime.TryParseExact(endDate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out end_d))
                    {
                        end_d = end_d.AddHours(23).AddMinutes(59).AddSeconds(59);
                        q = q.Where(x => start_d <= x.CreatedDate && x.CreatedDate <= end_d);
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
                        q = q.Where(x => start_d <= x.VoucherDate && x.VoucherDate <= end_d);
                    }
                }
            }
            //if (CustomerId != null && CustomerId.Value > 0)
            //{
            //    q = q.Where(x => x.CustomerId == CustomerId);
            //}
            if (SalerId != null && SalerId.Value > 0)
            {
                q = q.Where(x => x.SalerId == SalerId);
            }
            if (!string.IsNullOrEmpty(Code))
            {
                q = q.Where(x => x.Code.Contains(Code));
            }
            if (!string.IsNullOrEmpty(ReceiverUserName))
            {
                q = q.Where(x => x.ReceiverUserName.Contains(ReceiverUserName));
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
            var model = new PaymentViewModel();

            return View(model);
        }

        [HttpPost]
        public ActionResult Create(PaymentViewModel model)
        {
            if (ModelState.IsValid)
            {
                var payment = new Payment();
                using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.Snapshot }))
                {
                    AutoMapper.Mapper.Map(model, payment);
                    payment.IsDeleted = false;
                    payment.IsArchive = false;
                    payment.CreatedUserId = WebSecurity.CurrentUserId;
                    payment.ModifiedUserId = WebSecurity.CurrentUserId;
                    payment.AssignedUserId = WebSecurity.CurrentUserId;
                    payment.CreatedDate = DateTime.Now;
                    payment.ModifiedDate = DateTime.Now;
                    var check = Request["group_choice"];
                    payment.TargetName = check;
                    Erp.Domain.Repositories.CategoryRepository categoryRepository = new Erp.Domain.Repositories.CategoryRepository(new Domain.ErpDbContext());
                    var item = categoryRepository.GetCategoryByName(payment.Name).Value;
                    payment.ShortName = item.ToString();
                    if (check == "Customer")
                    {
                        payment.LoaiChungTuGoc = "ProductInvoice";
                    }
                    paymentRepository.InsertPayment(payment);

                    // cập nhật lại mã
                    payment.Code = Erp.BackOffice.Helpers.Common.GetOrderNo("Payment", model.Code);
                    paymentRepository.UpdatePayment(payment);
                    Erp.BackOffice.Helpers.Common.SetOrderNo("Payment");

                    //var prefix = Erp.BackOffice.Helpers.Common.GetSetting("prefixOrderNo_PaymentOther");
                    //payment.Code = Erp.BackOffice.Helpers.Common.GetCode(prefix, payment.Id);
                    //paymentRepository.UpdatePayment(payment);

                    var paymentDetail = new PaymentDetail();
                    paymentDetail.IsDeleted = false;
                    paymentDetail.CreatedUserId = WebSecurity.CurrentUserId;
                    paymentDetail.ModifiedUserId = WebSecurity.CurrentUserId;
                    paymentDetail.AssignedUserId = WebSecurity.CurrentUserId;
                    paymentDetail.CreatedDate = DateTime.Now;
                    paymentDetail.ModifiedDate = DateTime.Now;
                    paymentDetail.Name = model.Name;
                    paymentDetail.Amount = model.Amount;
                    paymentDetail.PaymentId = payment.Id;
                    paymentDetail.MaChungTuGoc = model.MaChungTuGoc;
                    paymentDetail.LoaiChungTuGoc = "ProductInvoice";
                    paymentDetailRepository.InsertPaymentDetail(paymentDetail);
                    scope.Complete();
                
                }
                if (Request.IsAjaxRequest())
                {
                    return Content("success");
                }
                else
                {
                    TempData[Globals.SuccessMessageKey] = App_GlobalResources.Wording.InsertSuccess;
                    return RedirectToAction("Index");
                }
                
            }
            return View(model);
        }
        #endregion

        #region Edit
        public ActionResult Edit(int? Id)
        {
            var Payment = paymentRepository.GetvwPaymentById(Id.Value);
            if (Payment != null && Payment.IsDeleted != true)
            {
                var model = new PaymentViewModel();
                AutoMapper.Mapper.Map(Payment, model);

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
        public ActionResult Edit(PaymentViewModel model)
        {
            if (ModelState.IsValid)
            {
                //if (Request["Submit"] == "Save")
                //{
                    var Payment = paymentRepository.GetPaymentById(model.Id);
                    AutoMapper.Mapper.Map(model, Payment);
                    Payment.ModifiedUserId = WebSecurity.CurrentUserId;
                    Payment.ModifiedDate = DateTime.Now;
                    var check = Request["group_choice"];
                    Payment.TargetName = check;
                    Erp.Domain.Repositories.CategoryRepository categoryRepository = new Erp.Domain.Repositories.CategoryRepository(new Domain.ErpDbContext());
                    var item = categoryRepository.GetCategoryByName(Payment.Name).Value;
                    Payment.ShortName = item.ToString();
                    paymentRepository.UpdatePayment(Payment);

                    var paymentDetail = paymentDetailRepository.GetPaymentDetailByPaymentId(model.Id);
                    paymentDetail.ModifiedUserId = WebSecurity.CurrentUserId;
                    paymentDetail.ModifiedDate = DateTime.Now;
                    paymentDetail.Name = model.Name;
                    paymentDetail.Amount = model.Amount;
                    paymentDetailRepository.UpdatePaymentDetail(paymentDetail);

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
            var payment = new vwPayment();
            if (Id != null && Id.Value > 0)
            {
                payment = paymentRepository.GetvwPaymentById(Id.Value);
            }

            if (!string.IsNullOrEmpty(TransactionCode))
            {
                payment = paymentRepository.GetAllvwPaymentFull().Where(item => item.Code == TransactionCode).FirstOrDefault();
            }
            if (payment != null && payment.IsDeleted != true)
            {
                var model = new PaymentViewModel();
                AutoMapper.Mapper.Map(payment, model);

                //if (model.CreatedUserId != Helpers.Common.CurrentUser.Id && Helpers.Common.CurrentUser.UserTypeId != 1)
                //{
                //    TempData["FailedMessage"] = "NotOwner";
                //    return RedirectToAction("Index");
                //}                
                ViewBag.PaymentDetail = paymentDetailRepository.GetAllPaymentDetailByPaymentId(model.Id).ToList();
                return View(model);
            }
            if (Request.UrlReferrer != null)
                return Redirect(Request.UrlReferrer.AbsoluteUri);
            return RedirectToAction("Index");
        }
        #endregion

        #region Delete
        [HttpPost]
        //public ActionResult Delete(int Id, string CancelReason, bool IsPopup)
        //{
        //    var payment = paymentRepository.GetPaymentById(Id);
        //    if (payment != null)
        //    {
        //        payment.IsDeleted = true;
        //        payment.IsArchive = false;
        //        payment.CancelReason = CancelReason;
        //        payment.ModifiedUserId = WebSecurity.CurrentUserId;
        //        payment.ModifiedDate = DateTime.Now;
        //        paymentRepository.UpdatePayment(payment);
        //        var paymentDetail = paymentDetailRepository.GetAllPaymentDetailByPaymentId(Id).ToList();
        //        for (int i = 0; i < paymentDetail.Count(); i++)
        //        {
        //            paymentDetail[i].IsDeleted = true;
        //            paymentDetail[i].ModifiedUserId = WebSecurity.CurrentUserId;
        //            paymentDetail[i].ModifiedDate = DateTime.Now;
        //            paymentDetailRepository.UpdatePaymentDetail(paymentDetail[i]);
        //        }

        //    }

        //    TempData[Globals.SuccessMessageKey] = "Đã hủy chứng từ";

        //    if (IsPopup)
        //    {
        //        return RedirectToAction("Detail", new { Id = payment.Id, IsPopup = IsPopup });
        //    }
        //    return RedirectToAction("Detail", new { Id = payment.Id });
        //}
        public ActionResult Delete(int? id)
        {
            try
            {
                string idDele = Request["Delete"];
                if (idDele != null)
                {
                    var item = paymentRepository.GetPaymentById(int.Parse(idDele, CultureInfo.InvariantCulture));
                    if (item != null)
                    {
                        if (item.CreatedUserId != Erp.BackOffice.Helpers.Common.CurrentUser.Id && Erp.BackOffice.Helpers.Common.CurrentUser.UserTypeId != 1)
                        {
                            TempData["FailedMessage"] = "NotOwner";
                            return RedirectToAction("Index");
                        }

                        item.IsDeleted = true;
                        paymentRepository.UpdatePayment(item);
                    }
                }
                else
                {
                    string idDeleteAll = Request["DeleteId-checkbox"];
                    if (idDeleteAll == null)
                    {
                        idDeleteAll = id.ToString();
                    }
                    string[] arrDeleteId = idDeleteAll.Split(',');
                    for (int i = 0; i < arrDeleteId.Count(); i++)
                    {
                        var item = paymentRepository.GetPaymentById(int.Parse(arrDeleteId[i], CultureInfo.InvariantCulture));
                        if (item != null)
                        {
                            if (item.CreatedUserId != Erp.BackOffice.Helpers.Common.CurrentUser.Id && Erp.BackOffice.Helpers.Common.CurrentUser.UserTypeId != 1)
                            {
                                TempData["FailedMessage"] = "NotOwner";
                                return RedirectToAction("Index");
                            }

                            item.IsDeleted = true;
                            paymentRepository.UpdatePayment(item);
                        }
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

        #region Print
        public ActionResult Print(int? Id, int? TargetId, int? Month, int? Year)
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
            vwPayment payment = new vwPayment();
            if (Id != null)
            {
                payment = paymentRepository.GetvwPaymentById(Id.Value);
            }
            else
            {
                payment = paymentRepository.GetAllvwPayment().Where(x => x.TargetId == TargetId && x.TargetName == "Staffs" && x.Name == "Chi trả lương tháng " + Month + "/" + Year).FirstOrDefault();
            }
            //lấy template phiếu xuất.
            var template = templatePrintRepository.GetAllTemplatePrint().Where(x => x.Code.Contains("Payment")).OrderByDescending(x => x.CreatedDate).FirstOrDefault();
            //truyền dữ liệu vào template.
            model.Content = template.Content;
            model.Content = model.Content.Replace("{Code}", payment.Code);
            model.Content = model.Content.Replace("{Company}", payment.ReceiverUserName);
            model.Content = model.Content.Replace("{Customer}", payment.Receiver);
            model.Content = model.Content.Replace("{Address}", payment.Address);
            model.Content = model.Content.Replace("{Reason}", payment.Name);
            model.Content = model.Content.Replace("{Money}", payment.Amount.ToCurrencyStr(null));
            model.Content = model.Content.Replace("{VoucherDate}", payment.VoucherDate!=null?payment.VoucherDate.Value.ToShortDateString():"chưa có");

            model.Content = model.Content.Replace("{CreatedDate}", payment.CreatedDate.Value.ToString("dd/MM/yyyy HH:mm"));
            model.Content = model.Content.Replace("{SalerName}", payment.SalerName);
            model.Content = model.Content.Replace("{MoneyText}", Erp.BackOffice.Helpers.Common.ChuyenSoThanhChu_2(payment.Amount.ToString()));
            model.Content = model.Content.Replace("{Note}", payment.Note);

            model.Content = model.Content.Replace("{System.CompanyName}", company);
            model.Content = model.Content.Replace("{System.AddressCompany}", address);
            model.Content = model.Content.Replace("{System.PhoneCompany}", phone);
            model.Content = model.Content.Replace("{System.Fax}", fax);
            model.Content = model.Content.Replace("{System.BankCodeCompany}", bankcode);
            model.Content = model.Content.Replace("{System.BankNameCompany}", bankname);
            //Mid
            model.Content = model.Content.Replace("{DetailList}", buildHtmlDetailList(payment.Id, payment.Amount));
            return View(model);
        }

        public ActionResult PrintPayment(string startDate, string endDate, string start, string end, int? SalerId, string Code, string ReceiverUserName, bool ExportExcel = false)
        {
            var data = IndexPrint(startDate, endDate, start, end, SalerId, Code, ReceiverUserName);

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
            model.Content = model.Content.Replace("{DataTable}", buildHtmlDanhsachphieuchi(data));
            model.Content = model.Content.Replace("{System.Logo}", ImgLogo);
            model.Content = model.Content.Replace("{System.CompanyName}", company);
            model.Content = model.Content.Replace("{System.AddressCompany}", address);
            model.Content = model.Content.Replace("{System.PhoneCompany}", phone);
            model.Content = model.Content.Replace("{System.Fax}", fax);
            model.Content = model.Content.Replace("{System.BankCodeCompany}", bankcode);
            model.Content = model.Content.Replace("{System.BankNameCompany}", bankname);
            model.Content = model.Content.Replace("{PrintDate}", DateTime.Now.ToString("dd/MM/yyyy HH:mm"));
            model.Content = model.Content.Replace("{Title}", "Danh sách phiếu chi");
            if (ExportExcel)
            {
                Response.AppendHeader("content-disposition", "attachment;filename=" + "DS_Phieuchi" + DateTime.Now.ToString("yyyyMMdd") + ".xls");
                Response.Charset = "";
                Response.Cache.SetCacheability(HttpCacheability.NoCache);
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.Write(model.Content);
                Response.End();
            }
            return View(model);
        }

        string buildHtmlDanhsachphieuchi(List<PaymentViewModel> detailList)
        {
            string detailLists = "<table border=\"1\" class=\"invoice-detail\">\r\n";
            detailLists += "<thead>\r\n";
            detailLists += "	<tr>";
            detailLists += "		<th>STT</th>";
            detailLists += "		<th>Ngày ghi sổ</th>";
            detailLists += "		<th>Ngày chứng từ</th>";
            detailLists += "		<th>Mã chứng từ</th>";
            detailLists += "		<th>Chứng từ gốc</th>";
            detailLists += "		<th>Người nhận</th>";
            detailLists += "		<th>Lý do</th>";
            detailLists += "		<th>Diễn giải</th>";
            detailLists += "		<th>Số tiền</th>";
            detailLists += "		<th>Trạng thái ghi sổ</th>";
            detailLists += "	</tr>";
            detailLists += "</thead>\r\n";
            detailLists += "<tbody>\r\n";
            var index = 1;
            decimal total = 0;

            foreach (var item in detailList)
            {
                detailLists += "<tr>\r\n"
                + "<td class=\"text-center orderNo\">" + (index++) + "</td>\r\n"
                + "<td>" + (item.ModifiedDate.HasValue ? item.ModifiedDate.Value.ToString("dd/MM/yyyy HH:mm") : "") + "</td>"
                + "<td>" + (item.CreatedDate.HasValue ? item.CreatedDate.Value.ToString("dd/MM/yyyy HH:mm") : "") + "</td>"
                + "<td class=\"text-left \">" + item.Code + "</td>\r\n"
                + "<td class=\"text-left \">" + item.MaChungTuGoc + "</td>\r\n"
                + "<td class=\"text-left \">" + item.Receiver + "</td>\r\n"
                + "<td class=\"text-left \">" + item.Name + "</td>\r\n"
                + "<td class=\"text-left \">" + item.Note + "</td>\r\n"
                + "<td class=\"text-left \">" + CommonSatic.ToCurrencyStr(item.Amount, null).Replace(".", ",") + "</td>\r\n"
                + "<td class=\"text-left \">" + (item.IsArchive == true ? "Đã ghi sổ" : "Chưa ghi sổ") + "</td>\r\n"
                + "</tr>\r\n";
                total += item.Amount ?? 0;
            }

            detailLists += "<tr>\r\n"
               + "<td style=\"font-weight:bold \">Tổng cộng</td>"
               + "<td class=\"text-left \">" + "" + "</td>\r\n"
               + "<td class=\"text-left \">" + "" + "</td>\r\n"
               + "<td class=\"text-left \">" + "" + "</td>\r\n"
               + "<td class=\"text-left \">" + "" + "</td>\r\n"
               + "<td class=\"text-left \">" + "" + "</td>\r\n"
               + "<td class=\"text-left \">" + "" + "</td>\r\n"
               + "<td class=\"text-left \">" + "" + "</td>\r\n"
               + "<td class=\"text-left \" style=\"font-weight:bold \">" + CommonSatic.ToCurrencyStr(total, null).Replace(".", ",") + "</td>\r\n"
               + "</tr>\r\n";


            detailLists += "</tbody>\r\n";
            detailLists += "<tfoot style=\"font-weight:bold\">\r\n";
            detailLists += "</tfoot>\r\n</table>\r\n";
            return detailLists;
        }

        public List<PaymentViewModel> IndexPrint(string startDate, string endDate, string start, string end, int? SalerId, string Code, string ReceiverUserName)
        {
            var q = paymentRepository.GetAllPayment()
            .Select(item => new PaymentViewModel
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
                TargetId = item.TargetId,
                TargetName = item.TargetName,
                Receiver = item.Receiver,
                SalerId = item.SalerId,
                //SalerName = item.SalerName,
                VoucherDate = item.VoucherDate,
                //ReceiverUserName = item.ReceiverUserName,
                MaChungTuGoc = item.MaChungTuGoc,
                LoaiChungTuGoc = item.LoaiChungTuGoc,
                CancelReason = item.CancelReason,
                IsDeleted = item.IsDeleted,
                IsArchive = item.IsArchive
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
                        q = q.Where(x => start_d <= x.VoucherDate && x.VoucherDate <= end_d).ToList();
                    }
                }
            }
            //if (CustomerId != null && CustomerId.Value > 0)
            //{
            //    q = q.Where(x => x.CustomerId == CustomerId).ToList();
            //}
            if (SalerId != null && SalerId.Value > 0)
            {
                q = q.Where(x => x.SalerId == SalerId).ToList();
            }
            if (!string.IsNullOrEmpty(Code))
            {
                q = q.Where(x => x.Code == Code).ToList();
            }
            //if (!string.IsNullOrEmpty(ReceiverUserName))
            //{
            //    q = q.Where(x => x.ReceiverUserName == ReceiverUserName).ToList();
            //}
            return q;
        }

        #endregion
        string buildHtmlDetailList(int Id, decimal? Amount)
        {

            //Lấy danh sách sản phẩm xuất kho
            var paymentDetails = paymentDetailRepository.GetAllPaymentDetailByPaymentId(Id).AsEnumerable()
                    .Select(x => new PaymentDetailViewModel
                    {
                        Id = x.Id,
                        Amount = x.Amount.Value,
                        LoaiChungTuGoc = x.LoaiChungTuGoc,
                        MaChungTuGoc = x.MaChungTuGoc,
                        Name = x.Name,
                        PaymentId = x.PaymentId,
                        GroupName = x.GroupName
                    }).ToList();
            var group = paymentDetails.GroupBy(x => x.GroupName).OrderBy(x => x.Key).ToList();
            var index = 0;
            //Tạo table html chi tiết phiếu xuất
            string detailList = "<table class=\"invoice-detail\">\r\n";
            detailList += "<thead>\r\n";
            detailList += "	<tr>\r\n";
            detailList += "		<th style=\"width:40px\">STT</th>\r\n";
            detailList += "		<th>Tên</th>\r\n";
            detailList += "		<th>Số tiền</th>\r\n";
            detailList += "		<th>Mã chứng từ gốc</th>\r\n";
            detailList += "	</tr>\r\n";
            detailList += "</thead>\r\n";
            detailList += "<tbody><tbody>\r\n";
            foreach (var groups in group)
            {
                if (!string.IsNullOrEmpty(groups.Key))
                {
                    detailList += "<tr>\r\n"
                     + "<td class=\"text-left\" colspan=\"4\">" + groups.Key + "</td>\r\n"
                     + "</tr>\r\n";
                }
                foreach (var item in paymentDetails.Where(x => x.GroupName == groups.Key))
                {
                    index++;
                    detailList += "<tr>\r\n"
                     + "<td class=\"text-center\">" + index + "</td>\r\n"
                     + "<td class=\"text-left\">" + item.Name + "</td>\r\n"
                     + "<td class=\"text-right\">" + Erp.BackOffice.Helpers.CommonSatic.ToCurrencyStr(item.Amount, null) + "</td>\r\n"
                     + "<td class=\"text-left\">" + item.MaChungTuGoc + "</td>\r\n"

                     + "</tr>\r\n";
                }
            }
            detailList += "</tbody>\r\n";
            detailList += "<tfoot>\r\n";
            detailList += "<tr></td><td class=\"text-right\" colspan=\"2\">Tổng cộng</td><td class=\"text-right\">"
                         + Erp.BackOffice.Helpers.CommonSatic.ToCurrencyStr(Amount, null)
                         + "</td><td></td></tr>\r\n";
            detailList += "<tr><td colspan=\"4\" class=\"text-left\"><strong>Tiền bằng chữ: " + Erp.BackOffice.Helpers.Common.ChuyenSoThanhChu(Amount.ToString()) + "</strong></td></tr>\r\n";
            detailList += "</tfoot>\r\n</table>\r\n";

            return detailList;
        }


        #region UnArchive
        [HttpPost]
        public ActionResult UnArchive(int? Id, int? TargetId, int? TargetMonth, int? TargetYear)
        {
            //lấy phiếu chi.
            Payment payment = new Payment();
            if (Id != null)
            {
                payment = paymentRepository.GetPaymentById(Id.Value);
            }
            else
            {
                payment = paymentRepository.GetAllPayment().Where(x => x.TargetId == TargetId && x.TargetName == "Staffs" && x.Name == "Chi trả lương tháng " + TargetMonth + "/" + TargetYear).FirstOrDefault();
            }
            if (payment != null)
            {
                payment.IsArchive = false;
                payment.ModifiedDate = DateTime.Now;
                payment.ModifiedUserId = WebSecurity.CurrentUserId;
                payment.VoucherDate = null;
                paymentRepository.UpdatePayment(payment);
                return Content("success");
            }
            return Content("error");
        }
        #endregion

        #region Archive
        [HttpPost]
        public ActionResult Archive(int Id)
        {
            var receipt = paymentRepository.GetPaymentById(Id);
            if (receipt != null)
            {
                receipt.IsArchive = true;
                receipt.ModifiedUserId = WebSecurity.CurrentUserId;
                receipt.ModifiedDate = DateTime.Now;
                paymentRepository.UpdatePayment(receipt);

                TempData[Globals.SuccessMessageKey] = "Xét duyệt thành công!";
            }
            return RedirectToAction("Index");
        }
        #endregion

        #region List

        public ViewResult List(int? StaffId)
        {

            IQueryable<PaymentViewModel> q = paymentRepository.GetAllvwPayment()
                .Select(item => new PaymentViewModel
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
                    TargetId = item.TargetId,
                    TargetName = item.TargetName,
                    Receiver = item.Receiver,
                    SalerId = item.SalerId,
                    SalerName = item.SalerName,
                    VoucherDate = item.VoucherDate,
                    ReceiverUserName = item.ReceiverUserName,
                    MaChungTuGoc = item.MaChungTuGoc,
                    LoaiChungTuGoc = item.LoaiChungTuGoc,
                    CancelReason = item.CancelReason,
                    IsDeleted = item.IsDeleted,
                    IsArchive = item.IsArchive
                }).OrderByDescending(m => m.CreatedDate);

            if (StaffId != null && StaffId.Value > 0)
            {
                q = q.Where(x => x.TargetId == StaffId&&x.TargetName=="Staffs");
            }

            else
            {
                q = null;
            }
            ViewBag.SuccessMessage = TempData["SuccessMessage"];
            ViewBag.FailedMessage = TempData["FailedMessage"];
            ViewBag.AlertMessage = TempData["AlertMessage"];
            return View(q);
        }
        #endregion
    }
}
