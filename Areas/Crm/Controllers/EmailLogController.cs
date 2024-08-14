using System.Globalization;
using Erp.BackOffice.Crm.Models;
using Erp.BackOffice.Filters;
using Erp.Domain.Entities;
using Erp.Domain.Interfaces;
using Erp.Domain.Crm.Entities;
using Erp.Domain.Crm.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Erp.Utilities;
using WebMatrix.WebData;
using Erp.BackOffice.Helpers;

namespace Erp.BackOffice.Crm.Controllers
{
    [Authorize]
    [InitializeSimpleMembership]
    [Erp.BackOffice.Helpers.NoCacheHelper]
    public class EmailLogController : Controller
    {
        private readonly IEmailLogRepository EmailLogRepository;
        private readonly IUserRepository userRepository;

        public EmailLogController(IEmailLogRepository _EmailLog, IUserRepository _user)
        {
            EmailLogRepository = _EmailLog;
            userRepository = _user;
        }

        #region Index
        public ViewResult Index(string txtCustomer, string txtCustomerInfo, string startDate, string endDate, string txtEmail, string TargetModule)
        {
            IEnumerable<EmailLogViewModel> q = EmailLogRepository.GetAllvwEmailLog()
                .Select(item => new EmailLogViewModel
                {
                    Id = item.Id,
                    CreatedUserId = item.CreatedUserId,
                    CreatedDate = item.CreatedDate,
                    ModifiedUserId = item.ModifiedUserId,
                    ModifiedDate = item.ModifiedDate,
                    //Campaign = item.Campaign,
                    Customer = item.Customer,
                    Body = item.Body,
                    SentDate = item.SentDate,
                    Status = item.Status,
                    CustomerID = item.CustomerID,
                    //CampaignID = item.CampaignID,
                    Email = item.Email,
                    TargetModule = item.TargetModule
                }).OrderByDescending(m => m.ModifiedDate);
            ViewBag.SuccessMessage = TempData["SuccessMessage"];
            ViewBag.FailedMessage = TempData["FailedMessage"];
            ViewBag.AlertMessage = TempData["AlertMessage"];

           
             
            if (!string.IsNullOrEmpty(txtCustomer))
            {
                q = q.Where(item => Erp.BackOffice.Helpers.Common.ChuyenThanhKhongDau(item.Customer).ToLower().Contains(Erp.BackOffice.Helpers.Common.ChuyenThanhKhongDau(txtCustomer).ToLower()));
            }
            if (!string.IsNullOrEmpty(txtEmail))
            {
                q = q.Where(item => item.Email == txtEmail);
            }
            if (!string.IsNullOrEmpty(txtCustomerInfo))
            {
                q = q.Where(item => Erp.BackOffice.Helpers.Common.ChuyenThanhKhongDau(item.Customer).ToLower().Contains(Erp.BackOffice.Helpers.Common.ChuyenThanhKhongDau(txtCustomerInfo).ToLower()) || item.Email == txtCustomerInfo);
            }
            DateTime d_startDate, d_endDate;
            if (DateTime.TryParseExact(startDate, "dd/MM/yyyy", new CultureInfo("vi-VN"), DateTimeStyles.None, out d_startDate))
            {
                if (DateTime.TryParseExact(endDate, "dd/MM/yyyy", new CultureInfo("vi-VN"), DateTimeStyles.None, out d_endDate))
                {
                    d_endDate = d_endDate.AddHours(23).AddMinutes(59);
                    q = q.Where(x => x.SentDate >= d_startDate && x.SentDate <= d_endDate);
                }
            }
            if (!string.IsNullOrEmpty(TargetModule))
            {
                q = q.Where(item => item.TargetModule == TargetModule);
            }
            return View(q);
        }
        #endregion

        #region ListEmail
        public ViewResult ListEmail(string TargetModule, int? TargetID, bool? layout)
        {
            IEnumerable<EmailLogViewModel> q = EmailLogRepository.GetAllvwEmailLog()
                .Select(item => new EmailLogViewModel
                {
                    Id = item.Id,
                    CreatedUserId = item.CreatedUserId,
                    CreatedDate = item.CreatedDate,
                    ModifiedUserId = item.ModifiedUserId,
                    ModifiedDate = item.ModifiedDate,
                    //Campaign = item.Campaign,
                    Customer = item.Customer,
                    Body = item.Body,
                    SentDate = item.SentDate,
                    Status = item.Status,
                    CustomerID = item.CustomerID,
                    //CampaignID = item.CampaignID,
                    TargetID = item.TargetID,
                    TargetModule = item.TargetModule,
                    Email = item.Email
                }).OrderByDescending(m => m.ModifiedDate);
            ViewBag.SuccessMessage = TempData["SuccessMessage"];
            ViewBag.FailedMessage = TempData["FailedMessage"];
            ViewBag.AlertMessage = TempData["AlertMessage"];
            ViewBag.TargetModule = TargetModule;
            ViewBag.TargetID = TargetID;
            //if (Campaign != null && Campaign.Value > 0)
            //{
            //    q = q.Where(item => item.CampaignID == Campaign);
            //}

            q = q.Where(item => item.TargetID == TargetID && item.TargetModule == TargetModule);
            
            ViewBag.layout = layout;
            return View(q);
        }
        [HttpPost]
        public ActionResult XulyEMAIL(bool? gui, string TargetModule, int TargetID)
        {
            if (gui.HasValue && gui == true)
            {
                var urlRefer = Request["UrlReferrerDelete"];

                string strIdKH = Request["DeleteId-checkbox-Email"];
                if (strIdKH == null)
                {
                    bool recalls = true;
                    return RedirectToAction("Detail", "Campaign", new { id = TargetID, recall = recalls });
                }
                else
                {
                    return RedirectToAction("Edit", "EmailLog", new { TargetID, TargetModule, strIdKH });
                }
            }
            else
            {
                var urlRefer = Request["UrlReferrerDelete"];
                try
                {
                    string idDeleteAll = Request["DeleteId-checkbox-Email"];
                    if (idDeleteAll == null)
                    {

                        return RedirectToAction("Detail", "Campaign", new { id = TargetID });
                    }
                    else
                    {
                        string[] arrDeleteId = idDeleteAll.Split(',');
                        bool recalls = true;
                        for (int i = 0; i < arrDeleteId.Count(); i++)
                        {
                            var item = EmailLogRepository.GetEmailLogById(int.Parse(arrDeleteId[i], CultureInfo.InvariantCulture));
                            if (item != null)
                            {
                                if (item.CreatedUserId != Erp.BackOffice.Helpers.Common.CurrentUser.Id && Erp.BackOffice.Helpers.Common.CurrentUser.UserTypeId != 1)
                                {
                                    TempData["FailedMessage"] = "NotOwner";

                                    return RedirectToAction("Detail", "Campaign", new { id = TargetID, recall = recalls });
                                }
                                item.IsDeleted = true;
                                EmailLogRepository.UpdateEmailLog(item);
                            }
                        }
                        TempData[Globals.SuccessMessageKey] = App_GlobalResources.Wording.DeleteSuccess;

                        return RedirectToAction("Detail", "Campaign", new { id = TargetID, recall = recalls });

                    }

                }
                catch (DbUpdateException)
                {
                    TempData[Globals.FailedMessageKey] = App_GlobalResources.Error.RelationError;
                    return RedirectToAction("Index");
                }


            }
        }
        #endregion

        #region Detail
        public ActionResult Detail(int? Id)
        {
            var EmailLog = EmailLogRepository.GetvwEmailLogById(Id.Value);
            if (EmailLog != null && EmailLog.IsDeleted != true)
            {
                var model = new EmailLogViewModel();
                AutoMapper.Mapper.Map(EmailLog, model);
                if (model.CreatedUserId != Erp.BackOffice.Helpers.Common.CurrentUser.Id && Erp.BackOffice.Helpers.Common.CurrentUser.UserTypeId != 1)
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
        #region Send Email
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult SendEmail(EmailLogViewModel model)
        {
            var urlRefer = Request["UrlReferrerDetail"];
            var emailLog = EmailLogRepository.GetEmailLogById(model.Id);
            if (emailLog != null && !string.IsNullOrEmpty(emailLog.Email))
            {
               
                string sentTo = emailLog.Email;
                string subject = emailLog.SubjectEmail;
                string body = emailLog.Body;
                //string cc = null;
                //string bcc = null;
                //string displayName = emailLog.Campaign;
                //string filePath = null;
                //string fileNameDisplayHasExtention = null;
                Erp.BackOffice.Helpers.Common.SendEmailEX(sentTo, emailLog.TargetModule, body, emailLog, EmailLogRepository);
                if (Request["IsPopup"] == "true" || Request["IsPopup"] == "True")
                {
                    return RedirectToAction("Detail", "EmailLog", new { area = "Crm", Id = model.Id, closePopup = "true" });
                }
                return Redirect(urlRefer);
            }
            return RedirectToAction("Index");
        }

        //public string EmailTemplate(EmailLogViewModel model)
        //{

        //} 
        #endregion

        #region Create
        public ViewResult Create( string TargetModule, int? TargetID)
        {
            var model = new EmailLogViewModel();
            model.TargetModule = TargetModule;
            model.TargetID = TargetID;
            
            return View(model);
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create(EmailLogViewModel model)
        {
            var urlRefer = Request["UrlReferrer"];
            if (ModelState.IsValid)
            {
                var EmailLog = new Domain.Crm.Entities.EmailLog();
                AutoMapper.Mapper.Map(model, EmailLog);
                EmailLog.IsDeleted = false;
                EmailLog.CreatedUserId = WebSecurity.CurrentUserId;
                EmailLog.ModifiedUserId = WebSecurity.CurrentUserId;
                EmailLog.CreatedDate = DateTime.Now;
                EmailLog.ModifiedDate = DateTime.Now;
                EmailLog.SentDate = DateTime.Now;
                EmailLog.Status = "Đã gửi";
                EmailLog.SubjectEmail = Erp.BackOffice.Helpers.Common.GetSetting("companyName") + " - " + EmailLog.TargetModule;
                EmailLogRepository.InsertEmailLog(EmailLog);
                
                TempData[Globals.SuccessMessageKey] = App_GlobalResources.Wording.InsertSuccess;
                
                Erp.BackOffice.Helpers.Common.SendEmailEX(EmailLog.Email, EmailLog.TargetModule, EmailLog.Body, EmailLog, EmailLogRepository);
                if (Request["IsPopup"] == "true" || Request["IsPopup"] == "True")
                {
                    ViewBag.closePopup = "true";
                    model.Id = EmailLog.Id;
                    ViewBag.urlRefer = urlRefer;
                    return View(model);
                }
                return Redirect(urlRefer);
            }
            return RedirectToAction("Create");
        }
        #endregion

        #region Edit
        public ActionResult Edit(int? TargetID, string TargetModule, string strIdKH)
        {
            if (strIdKH != null)
            {
                string[] arrIDKH = strIdKH.Split(',');
                ViewBag.strIdKH = strIdKH;
            }
            else
            {
                ViewBag.strIdKH = strIdKH;
            }

            ViewBag.TargetID = TargetID;
            ViewBag.TargetModule = TargetModule;
            if (TargetID != null)
            {
                var SMSLog = EmailLogRepository.GetEmailLogById(TargetID.Value);
            }
            var model = new EmailLogViewModel();
            return View(model);
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(int? TargetID, string TargetModule, string strIdKH, string body, bool? quaylai)
        {
            if (quaylai.HasValue && quaylai != null)
            {
                return RedirectToAction("Detail", "Campaign", new { id = TargetID });
            }

            var Status = Request["Status"];

            string[] arrIDKH = strIdKH.Split(',');
            foreach (var item in arrIDKH)
            {

                int Id = int.Parse(item);
                var tmp = EmailLogRepository.GetEmailLogById(Id);
                tmp.Body = body;
                tmp.Status = Status;
                EmailLogRepository.UpdateEmailLog(tmp);

                Erp.BackOffice.Helpers.Common.SendEmailEX(tmp.Email, TargetModule, tmp.Body, tmp, EmailLogRepository);

            }
            return RedirectToAction("Detail", "Campaign", new { id = TargetID });
        }
        #endregion

        #region Delete
        [HttpPost]
        public ActionResult Delete()
        {
            var urlRefer = Request["UrlReferrerDelete"];
            try
            {
                string id = Request["Delete"];
                string idDeleteAll = Request["DeleteId-checkbox-Email"];
                if (id != null)
                {
                    var item = EmailLogRepository.GetEmailLogById(int.Parse(id, CultureInfo.InvariantCulture));
                    if (item != null)
                    {
                        if (item.CreatedUserId != Erp.BackOffice.Helpers.Common.CurrentUser.Id && Erp.BackOffice.Helpers.Common.CurrentUser.UserTypeId != 1)
                        {
                            TempData["FailedMessage"] = "NotOwner";
                            return RedirectToAction("Index");
                        }
                        item.IsDeleted = true;
                        EmailLogRepository.UpdateEmailLog(item);
                    }
                }
                else
                {
                    
                    string[] arrDeleteId = idDeleteAll.Split(',');
                    for (int i = 0; i < arrDeleteId.Count(); i++)
                    {
                        var item = EmailLogRepository.GetEmailLogById(int.Parse(arrDeleteId[i], CultureInfo.InvariantCulture));
                        if (item != null)
                        {
                            if (item.CreatedUserId != Erp.BackOffice.Helpers.Common.CurrentUser.Id && Erp.BackOffice.Helpers.Common.CurrentUser.UserTypeId != 1)
                            {
                                TempData["FailedMessage"] = "NotOwner";
                                return RedirectToAction("Index");
                            }
                            item.IsDeleted = true;
                            EmailLogRepository.UpdateEmailLog(item);
                        }
                    }
                }
                
                TempData[Globals.SuccessMessageKey] = App_GlobalResources.Wording.DeleteSuccess;
                return Redirect("Index");
            }
            catch (DbUpdateException)
            {
                TempData[Globals.FailedMessageKey] = App_GlobalResources.Error.RelationError;
                return RedirectToAction("Index");
            }
        }
        #endregion

        public static void SaveEmail(string email,string body, int? customerID, int TargetID, string TargetModule, string subj)
        {
            Erp.Domain.Crm.Repositories.EmailLogReponsitory emailLogRepository = new Erp.Domain.Crm.Repositories.EmailLogReponsitory(new Domain.Crm.ErpCrmDbContext());
            var EmailLog = new Domain.Crm.Entities.EmailLog();
            EmailLog.IsDeleted = false;
            EmailLog.CreatedUserId = WebSecurity.CurrentUserId;
            EmailLog.ModifiedUserId = WebSecurity.CurrentUserId;
            EmailLog.CreatedDate = DateTime.Now;
            EmailLog.ModifiedDate = DateTime.Now;
            EmailLog.SentDate = DateTime.Now;
            EmailLog.Status = "Đã gửi";
            EmailLog.Email = email;
            EmailLog.Body = body;
            EmailLog.CustomerID = customerID.Value;
            EmailLog.TargetID = TargetID;
            EmailLog.TargetModule = TargetModule;
            EmailLog.SubjectEmail = Erp.BackOffice.Helpers.Common.GetSetting("companyName") + " - " + subj;

            emailLogRepository.InsertEmailLog(EmailLog);
        }

    }
}
