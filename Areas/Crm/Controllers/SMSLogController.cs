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
using Erp.BackOffice.Areas.Crm.Models;
using Erp.BackOffice.Account.Models;
using Erp.Domain.Account.Repositories;
using Erp.Domain.Account.Interfaces;
using Erp.BackOffice.Areas.Administration.Models;
using Erp.Domain.Account.Helper;
using Erp.BackOffice.Sale.Models;

namespace Erp.BackOffice.Crm.Controllers
{
    [Authorize]
    [InitializeSimpleMembership]
    [Erp.BackOffice.Helpers.NoCacheHelper]
    public class SMSLogController : Controller
    {
        private readonly ISMSLogRepository SMSLogRepository;
        private readonly IEmailLogRepository EMAILRepository;
        private readonly IUserRepository userRepository;
        private readonly ICustomerRepository CustomerRepository;
        private readonly CustomerViewModel tmp;

        public SMSLogController(ISMSLogRepository _SMSLog, IUserRepository _user, ICustomerRepository _Customer, IEmailLogRepository _EMAILLog)
        {
            SMSLogRepository = _SMSLog;
            userRepository = _user;
            CustomerRepository = _Customer;
            EMAILRepository = _EMAILLog;
        }

        #region Index
        public ViewResult Index(string txtCustomer, string startDate, string endDate, string txtPhone, string TargetModule)
        {
            IEnumerable<SMSLogViewModel> q = SMSLogRepository.GetAllvwSMSLog()
                .Select(item => new SMSLogViewModel
                {
                    Id = item.Id,
                    CreatedUserId = item.CreatedUserId,
                    //CreatedUserName = item.CreatedUserName,
                    CreatedDate = item.CreatedDate,
                    ModifiedUserId = item.ModifiedUserId,
                    //ModifiedUserName = item.ModifiedUserName,
                    ModifiedDate = item.ModifiedDate,
                    Customer = item.Customer,
                    Body = item.Body,
                    SentDate = item.SentDate,
                    Status = item.Status,
                    CustomerID = item.CustomerID,
                    Phone = item.Phone,
                    TargetModule = item.TargetModule
                }).OrderByDescending(m => m.ModifiedDate);
            ViewBag.SuccessMessage = TempData["SuccessMessage"];
            ViewBag.FailedMessage = TempData["FailedMessage"];
            ViewBag.AlertMessage = TempData["AlertMessage"];



            if (!string.IsNullOrEmpty(txtCustomer))
            {
                q = q.Where(item => Erp.BackOffice.Helpers.Common.ChuyenThanhKhongDau(item.Customer).ToLower().Contains(Erp.BackOffice.Helpers.Common.ChuyenThanhKhongDau(txtCustomer).ToLower()));
            }
            if (!string.IsNullOrEmpty(txtPhone))
            {
                q = q.Where(item => item.Phone == txtPhone);
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

        #region ListSMS
        public ViewResult ListSMS(string TargetModule, int? TargetID, bool? layout)
        {
            IEnumerable<SMSLogViewModel> q = SMSLogRepository.GetAllvwSMSLog()
                .Select(item => new SMSLogViewModel
                {
                    Id = item.Id,
                    CreatedUserId = item.CreatedUserId,
                    //CreatedUserName = item.CreatedUserName,
                    CreatedDate = item.CreatedDate,
                    ModifiedUserId = item.ModifiedUserId,
                    //ModifiedUserName = item.ModifiedUserName,
                    ModifiedDate = item.ModifiedDate,

                    Customer = item.Customer,
                    Body = item.Body,
                    SentDate = item.SentDate,
                    Status = item.Status,
                    CustomerID = item.CustomerID,
                    TargetID = item.TargetID,
                    TargetModule = item.TargetModule,
                    Phone = item.Phone
                }).OrderByDescending(m => m.ModifiedDate);
            ViewBag.SuccessMessage = TempData["SuccessMessage"];
            ViewBag.FailedMessage = TempData["FailedMessage"];
            ViewBag.AlertMessage = TempData["AlertMessage"];
            ViewBag.TargetModule = TargetModule;
            ViewBag.TargetID = TargetID;

            q = q.Where(item => item.TargetID == TargetID && item.TargetModule == TargetModule);
            ViewBag.layout = layout;
            return View(q);
        }
        #endregion

        #region Detail & SendSMS
        public ActionResult Detail(int? Id)
        {

            var SMSLog = SMSLogRepository.GetvwSMSLogById(Id.Value);
            if (SMSLog != null && SMSLog.IsDeleted != true)
            {
                var model = new SMSLogViewModel();
                AutoMapper.Mapper.Map(SMSLog, model);
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
        // detail and sendSMS
        [HttpPost]
        public ActionResult SendSMS(SMSLogViewModel model)
        {
            var urlRefer = Request["UrlReferrerDetail"];
            var smsLog = SMSLogRepository.GetSMSLogById(model.Id);//GetvwSMSLogById();
            if (smsLog != null && !string.IsNullOrEmpty(smsLog.Phone))
            {
                if ((smsLog.Body != null) && (smsLog.Body.Length > 0))
                {
                    Erp.BackOffice.Helpers.Common.SendSMSEX(smsLog.Phone, smsLog.Body, smsLog, SMSLogRepository);
                }
                if (Request["IsPopup"] == "true" || Request["IsPopup"] == "True")
                {
                    ViewBag.closePopup = "true";
                    ViewBag.urlRefer = urlRefer;
                    return RedirectToAction("Detail", "SMSLog", new { area = "Crm", Id = model.Id, closePopup = "true" });
                }
                return Redirect(urlRefer);
            }
            return RedirectToAction("Index");
        }
        #endregion

        #region Create
        public ViewResult Create(string TargetModule, int? TargetID)
        {
            var model = new SMSLogViewModel();
            model.TargetModule = TargetModule;
            model.TargetID = TargetID;
            return View(model);
        }
        public ViewResult searchsms(string TargetModule, int? TargetID, string CardCode, string txtCusName, string txtCode, string Phone, string ProvinceId, string DistrictId, string WardId, string search,string WhoCall)
        {
            var model = new SMSLogViewModel();

            var dataNhanvien = SqlHelper.QuerySP<ManagerStaff>("spgetNhanvienbacap", new
            {
                BranchId = Erp.BackOffice.Helpers.Common.CurrentUser.BranchId,
                UserId = WebSecurity.CurrentUserId,
            }).ToList();
            List<int> listNhanvien = new List<int>();


            for (int i = 0; i < dataNhanvien.Count; i++)
            {
                listNhanvien.Add(int.Parse(dataNhanvien[i].Id.ToString()));
            }
            listNhanvien.Add(WebSecurity.CurrentUserId);
            var user = userRepository.GetAllUsers().Select(item => new UserViewModel
            {
                Id = item.Id,
                UserName = item.UserName,
                FullName = item.FullName,
                Status = item.Status,

            }).Where(x => x.Status == UserStatus.Active && listNhanvien.Contains(x.Id)).ToList();
            ViewBag.User = user;

            IEnumerable<CustomerViewModel> q = CustomerRepository.GetAllvwCustomer().AsEnumerable().Where(x => x.isLock != true)
                //.Where(x => ("," + Helpers.Common.CurrentUser.DrugStore + ",").Contains("," + x.BranchId + ",") == true&&x.CustomerType=="Customer")
                .Where(x => x.BranchId == Helpers.Common.CurrentUser.BranchId)
                .Select(item => new CustomerViewModel
                {
                    Id = item.Id,
                    CreatedUserId = item.CreatedUserId,
                    //CreatedUserName = item.CreatedUserName,
                    CreatedDate = item.CreatedDate,
                    ModifiedUserId = item.ModifiedUserId,
                    //ModifiedUserName = item.ModifiedUserName,
                    ModifiedDate = item.ModifiedDate,
                    Code = item.Code,
                    CompanyName = item.CompanyName,
                    Phone = item.Phone,
                    Address = item.Address,
                    Note = item.Note,
                    CardCode = item.CardCode,
                    Image = item.Image,
                    Birthday = item.Birthday,
                    Email = item.Email,
                    Phoneghep = item.Phoneghep,
                    Gender = item.Gender,
                    IdCardDate = item.IdCardDate,
                    IdCardIssued = item.IdCardIssued,
                    IdCardNumber = item.IdCardNumber,
                    CardIssuedName = item.CardIssuedName,
                    BranchName = item.BranchName,
                    ManagerStaffName = item.ManagerStaffName,
                    ManagerUserName = item.ManagerUserName,
                    CustomerType = item.CustomerType,

                });

            bool hasSearch = false;

            if (!string.IsNullOrEmpty(CardCode))
            {
                q = q.Where(x => x.CardCode.Contains(CardCode));
                hasSearch = true;
            }

            if (!string.IsNullOrEmpty(txtCusName))
            {
                txtCusName = Helpers.Common.ChuyenThanhKhongDau(txtCusName);
                q = q.Where(x => Helpers.Common.ChuyenThanhKhongDau(x.CompanyName).Contains(txtCusName));
                hasSearch = true;
            }

            if (!string.IsNullOrEmpty(txtCode))
            {
                q = q.Where(x => x.Code.Contains(txtCode));
                hasSearch = true;
            }
            if (!string.IsNullOrEmpty(Phone))
            {
                q = q.Where(x => x.Phone != null && x.Phone.Contains(Phone));
                hasSearch = true;
            }
            if (!string.IsNullOrEmpty(ProvinceId))
            {
                q = q.Where(item => item.CityId == ProvinceId);
                hasSearch = true;
            }
            if (!string.IsNullOrEmpty(DistrictId))
            {
                q = q.Where(item => item.DistrictId == DistrictId);
                hasSearch = true;
            }
            if (!string.IsNullOrEmpty(WardId))
            {
                q = q.Where(item => item.WardId == WardId);
                hasSearch = true;
            }
            q = q.OrderByDescending(m => m.CreatedDate);
            //if(hasSearch)
            //{
            //    q = q.OrderByDescending(m => m.CompanyName);
            //    hasSearch = true;
            //}
            //else
            //{
            //    q = null;
            //}

            ViewBag.SuccessMessage = TempData["SuccessMessage"];
            ViewBag.FailedMessage = TempData["FailedMessage"];
            ViewBag.AlertMessage = TempData["AlertMessage"];
            ViewBag.AdviseTypeError = TempData["AdviseTypeError"];
            ViewBag.TargetModule = TargetModule;
            ViewBag.TargetID = TargetID;
            ViewBag.WhoCall = WhoCall;
            return View(q);
        }
        [HttpPost]
        public ViewResult searchsms(string TargetModule, int? TargetID, string CardCode, string txtCusName, string txtCode, string Phone, string ProvinceId, string DistrictId, string WardId)
        {
            var model = new SMSLogViewModel();

            var dataNhanvien = SqlHelper.QuerySP<ManagerStaff>("spgetNhanvienbacap", new
            {
                BranchId = Erp.BackOffice.Helpers.Common.CurrentUser.BranchId,
                UserId = WebSecurity.CurrentUserId,
            }).ToList();
            List<int> listNhanvien = new List<int>();


            for (int i = 0; i < dataNhanvien.Count; i++)
            {
                listNhanvien.Add(int.Parse(dataNhanvien[i].Id.ToString()));
            }
            listNhanvien.Add(WebSecurity.CurrentUserId);
            var user = userRepository.GetAllUsers().Select(item => new UserViewModel
            {
                Id = item.Id,
                UserName = item.UserName,
                FullName = item.FullName,
                Status = item.Status,

            }).Where(x => x.Status == UserStatus.Active && listNhanvien.Contains(x.Id)).ToList();
            ViewBag.User = user;
            IEnumerable<CustomerViewModel> q = CustomerRepository.GetAllvwCustomer().AsEnumerable().Where(x => x.isLock != true)
                //.Where(x => ("," + Helpers.Common.CurrentUser.DrugStore + ",").Contains("," + x.BranchId + ",") == true&&x.CustomerType=="Customer")
                .Where(x => x.BranchId == Helpers.Common.CurrentUser.BranchId)
                .Select(item => new CustomerViewModel
                {
                    Id = item.Id,
                    CreatedUserId = item.CreatedUserId,
                    //CreatedUserName = item.CreatedUserName,
                    CreatedDate = item.CreatedDate,
                    ModifiedUserId = item.ModifiedUserId,
                    //ModifiedUserName = item.ModifiedUserName,
                    ModifiedDate = item.ModifiedDate,
                    Code = item.Code,
                    CompanyName = item.CompanyName,
                    Phone = item.Phone,
                    Address = item.Address,
                    Note = item.Note,
                    CardCode = item.CardCode,
                    Image = item.Image,
                    Birthday = item.Birthday,
                    Email = item.Email,
                    Phoneghep = item.Phoneghep,
                    Gender = item.Gender,
                    IdCardDate = item.IdCardDate,
                    IdCardIssued = item.IdCardIssued,
                    IdCardNumber = item.IdCardNumber,
                    CardIssuedName = item.CardIssuedName,
                    BranchName = item.BranchName,
                    ManagerStaffName = item.ManagerStaffName,
                    ManagerUserName = item.ManagerUserName,
                    CustomerType = item.CustomerType,

                });

            bool hasSearch = false;

            if (!string.IsNullOrEmpty(CardCode))
            {
                q = q.Where(x => x.CardCode.Contains(CardCode));
                hasSearch = true;
            }

            if (!string.IsNullOrEmpty(txtCusName))
            {
                txtCusName = Helpers.Common.ChuyenThanhKhongDau(txtCusName);
                q = q.Where(x => Helpers.Common.ChuyenThanhKhongDau(x.CompanyName).Contains(txtCusName));
                hasSearch = true;
            }

            if (!string.IsNullOrEmpty(txtCode))
            {
                q = q.Where(x => x.Code.Contains(txtCode));
                hasSearch = true;
            }
            if (!string.IsNullOrEmpty(Phone))
            {
                q = q.Where(x => x.Phone != null && x.Phone.Contains(Phone));
                hasSearch = true;
            }
            if (!string.IsNullOrEmpty(ProvinceId))
            {
                q = q.Where(item => item.CityId == ProvinceId);
                hasSearch = true;
            }
            if (!string.IsNullOrEmpty(DistrictId))
            {
                q = q.Where(item => item.DistrictId == DistrictId);
                hasSearch = true;
            }
            if (!string.IsNullOrEmpty(WardId))
            {
                q = q.Where(item => item.WardId == WardId);
                hasSearch = true;
            }
            q = q.OrderByDescending(m => m.CreatedDate);
            //if(hasSearch)
            //{
            //    q = q.OrderByDescending(m => m.CompanyName);
            //    hasSearch = true;
            //}
            //else
            //{
            //    q = null;
            //}

            ViewBag.SuccessMessage = TempData["SuccessMessage"];
            ViewBag.FailedMessage = TempData["FailedMessage"];
            ViewBag.AlertMessage = TempData["AlertMessage"];
            ViewBag.AdviseTypeError = TempData["AdviseTypeError"];
            ViewBag.TargetModule = TargetModule;
            ViewBag.TargetID = TargetID;

            return View(q);
        }
        [HttpPost]
        public ActionResult Listsmsguicheacked(string TargetModule, int TargetID, bool? recallnokh, string WhoCall)
        {
            if (recallnokh.HasValue)
            {
                bool recallss = true;
                return RedirectToAction("Detail", "Campaign", new { id = TargetID, recall = recallss });
            }
            else
            {
                var dataNhanvien = SqlHelper.QuerySP<ManagerStaff>("spgetNhanvienbacap", new
                {
                    BranchId = Erp.BackOffice.Helpers.Common.CurrentUser.BranchId,
                    UserId = WebSecurity.CurrentUserId,
                }).ToList();
                List<int> listNhanvien = new List<int>();


                for (int i = 0; i < dataNhanvien.Count; i++)
                {
                    listNhanvien.Add(int.Parse(dataNhanvien[i].Id.ToString()));
                }
                listNhanvien.Add(WebSecurity.CurrentUserId);
                var user = userRepository.GetAllUsers().Select(item => new UserViewModel
                {
                    Id = item.Id,
                    UserName = item.UserName,
                    FullName = item.FullName,
                    Status = item.Status,

                }).Where(x => x.Status == UserStatus.Active && listNhanvien.Contains(x.Id)).ToList();
                ViewBag.User = user;

                List<CustomerViewModel> kq = new List<CustomerViewModel>();
                string listKHsms = Request["listKHsms-checkbox"];
                string[] listKHsmss = listKHsms.Split(',');
                for (int i = 0; i < listKHsmss.Count() - 1; i++)
                {
                    var item = CustomerRepository.GetCustomerById(int.Parse(listKHsmss[i]));
                    if (WhoCall == "SMS")
                    {
                        var SMSLog = new Domain.Crm.Entities.SMSLog();
                        if (item != null)
                        {
                            SMSLog.IsDeleted = false;
                            SMSLog.CreatedUserId = WebSecurity.CurrentUserId;
                            SMSLog.ModifiedUserId = WebSecurity.CurrentUserId;
                            SMSLog.CustomerID = item.Id;
                            SMSLog.TargetModule = TargetModule;
                            SMSLog.TargetID = TargetID;
                            SMSLog.CreatedDate = DateTime.Now;
                            SMSLog.ModifiedDate = DateTime.Now;
                            SMSLog.SentDate = DateTime.Now;
                            SMSLog.Status = "Chưa Gửi";
                            SMSLog.Phone = item.Phone;
                            SMSLogRepository.InsertSMSLog(SMSLog);



                            //CustomerViewModel tmp = new CustomerViewModel();
                            //tmp.Id = item.Id;
                            // tmp.CreatedUserId = item.CreatedUserId;
                            //CreatedUserName = item.CreatedUserName,
                            //tmp.CreatedDate = item.CreatedDate;
                            //tmp.ModifiedUserId = item.ModifiedUserId;
                            //ModifiedUserName = item.ModifiedUserName,
                            //tmp.ModifiedDate = item.ModifiedDate;
                            //tmp.Code = item.Code;
                            // tmp.CompanyName = item.CompanyName;
                            // tmp.Phone = item.Phone;
                            //tmp.Address = item.Address;
                            //tmp.Note = item.Note;
                            // tmp.CardCode = item.CardCode;
                            //tmp.Image = item.Image;
                            //tmp.Birthday = item.Birthday;
                            //tmp.Email = item.Email;
                            //tmp.Phoneghep = item.Phone;
                            //tmp.Gender = item.Gender;
                            //tmp.IdCardDate = item.IdCardDate;
                            // tmp.IdCardIssued = item.IdCardIssued;
                            //tmp.IdCardNumber = item.IdCardNumber;
                            // tmp.CardIssuedName = item.CardIssuedName;
                            //tmp.BranchName = item.BranchName;
                            //tmp.ManagerStaffName = item.ManagerStaffName;
                            //  tmp.ManagerUserName = item.ManagerUserName;
                            // tmp.CustomerType = item.CustomerType;


                            //kq.Add(tmp);

                        }
                    }
                    if(WhoCall == "EMAIL")
                    {
                        var EMAILLog = new Domain.Crm.Entities.EmailLog();
                        if (item != null)
                        {
                            EMAILLog.IsDeleted = false;
                            EMAILLog.CreatedUserId = WebSecurity.CurrentUserId;
                            EMAILLog.ModifiedUserId = WebSecurity.CurrentUserId;
                            EMAILLog.CustomerID = item.Id;
                            EMAILLog.TargetModule = TargetModule;
                            EMAILLog.TargetID = TargetID;
                            EMAILLog.CreatedDate = DateTime.Now;
                            EMAILLog.ModifiedDate = DateTime.Now;
                            EMAILLog.SentDate = DateTime.Now;
                            EMAILLog.Status = "Chưa Gửi";
                            EMAILLog.Email = item.Email;
                            EMAILRepository.InsertEmailLog(EMAILLog);




                        }
                    }

                }
                bool recalls = true;
                return RedirectToAction("Detail", "Campaign", new { id = TargetID, recall = recalls });
            }
        }
        [HttpPost]
        public ActionResult XulySMS(bool? gui, string TargetModule, int TargetID)
        {
            if (gui.HasValue && gui == true)
            {
                var urlRefer = Request["UrlReferrerDelete"];

                string strIdKH = Request["DeleteId-checkboxSMS"];
                if(strIdKH == null)
                {
                    bool recalls = true;
                    return RedirectToAction("Detail", "Campaign", new { id = TargetID, recall = recalls });
                }
                else
                {
                    return RedirectToAction("Edit", "SMSLog", new { id = TargetID, TargetModule, strIdKH });
                }
            }
            else
            {
                var urlRefer = Request["UrlReferrerDelete"];
                try
                {
                    string idDeleteAll = Request["DeleteId-checkboxSMS"];
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
                            var item = SMSLogRepository.GetSMSLogById(int.Parse(arrDeleteId[i], CultureInfo.InvariantCulture));
                            if (item != null)
                            {
                                if (item.CreatedUserId != Erp.BackOffice.Helpers.Common.CurrentUser.Id && Erp.BackOffice.Helpers.Common.CurrentUser.UserTypeId != 1)
                                {
                                    TempData["FailedMessage"] = "NotOwner";

                                    return RedirectToAction("Detail", "Campaign", new { id = TargetID, recall = recalls });
                                }
                                item.IsDeleted = true;
                                SMSLogRepository.UpdateSMSLog(item);
                            }
                        }
                        TempData[Globals.SuccessMessageKey] = App_GlobalResources.Wording.DeleteSuccess;

                        return RedirectToAction("Detail", "Campaign", new { id = TargetID, recall = recalls });
                        //return RedirectToAction("Delete", "SMSLog");
                    }

                }
                catch (DbUpdateException)
                {
                    TempData[Globals.FailedMessageKey] = App_GlobalResources.Error.RelationError;
                    return RedirectToAction("Index");
                }


            }

        }
        [HttpPost]
        public ActionResult Create(SMSLogViewModel model)
        {

            var urlRefer = Request["UrlReferrerCreate"];
            if (ModelState.IsValid)
            {
                var SMSLog = new Domain.Crm.Entities.SMSLog();
                AutoMapper.Mapper.Map(model, SMSLog);
                SMSLog.IsDeleted = false;
                SMSLog.CreatedUserId = WebSecurity.CurrentUserId;
                SMSLog.ModifiedUserId = WebSecurity.CurrentUserId;
                SMSLog.CreatedDate = DateTime.Now;
                SMSLog.ModifiedDate = DateTime.Now;
                SMSLog.SentDate = DateTime.Now;
                SMSLog.Status = "Đã gửi";
                SMSLogRepository.InsertSMSLog(SMSLog);
                TempData[Globals.SuccessMessageKey] = App_GlobalResources.Wording.InsertSuccess;
                //send sms

                Erp.BackOffice.Helpers.Common.SendSMSEX(SMSLog.Phone, SMSLog.Body, SMSLog, SMSLogRepository);

                if (Request["IsPopup"] == "true" || Request["IsPopup"] == "True")
                {
                    ViewBag.closePopup = "true";
                    model.Id = SMSLog.Id;
                    ViewBag.urlRefer = urlRefer;
                    return View(model);
                }
                return Redirect(urlRefer);
            }
            return RedirectToAction("Create");
        }
        #endregion

        #region Edit
        public ActionResult Edit(int? Id, string TargetModule, string strIdKH)
        {
            if (strIdKH != null)
            {
                string[] arrIDKH = strIdKH.Split(',');
                ViewBag.strIdKH = strIdKH;
            }
            else
            {
                ViewBag.strIdKH = Id;
            }

            ViewBag.Id = Id;
            ViewBag.TargetModule = TargetModule;
            var SMSLog = SMSLogRepository.GetSMSLogById(Id.Value);
            var model = new SMSLogViewModel();

            return View(model);
        }

        [HttpPost]
        public ActionResult Edit( int TargetID, string TargetModule, string strIdKH, string body, bool? quaylai)
        {
            if (TargetModule != "")
            {
                if (quaylai.HasValue)
                {
                    return RedirectToAction("Detail", "Campaign", new { id = TargetID });
                }

            }
            else
            {
                if (quaylai.HasValue)
                {
                    return RedirectToAction("Index", "SMSLog");
                }


            }
           
           


            string[] arrIDKH = strIdKH.Split(',');
            foreach (var item in arrIDKH)
            {

                int Id = int.Parse(item);
                var tmp = SMSLogRepository.GetSMSLogById(Id);
                tmp.Body = body;


                Erp.BackOffice.Helpers.Common.SendSMSEX(tmp.Phone, tmp.Body, tmp, SMSLogRepository);
            }
            //IEnumerable<SMSLogViewModel> q = SMSLogRepository.GetAllvwSMSLog()






            //if (Request["IsPopup"] == "true" || Request["IsPopup"] == "True")
            //{
            //    ViewBag.closePopup = "true";
            //    model.Id = SMSLog.Id;
            //    ViewBag.urlRefer = urlRefer;
            //    return View(model);
            //}
            //return Redirect(urlRefer);


            //var urlRefer = Request["UrlReferrerEdit"];
            //if (ModelState.IsValid)
            //{
            //    if (Request["Submit"] == "Save")
            //    {
            //        var SMSLog = SMSLogRepository.GetSMSLogById(model.Id);
            //        AutoMapper.Mapper.Map(model, SMSLog);
            //        SMSLog.ModifiedUserId = WebSecurity.CurrentUserId;
            //        SMSLog.ModifiedDate = DateTime.Now;
            //        SMSLog.SentDate = DateTime.Now;
            //        SMSLogRepository.InsertSMSLog(SMSLog);
            //        //send sms
            //        Erp.BackOffice.Helpers.Common.SendSMS(SMSLog.Phone, SMSLog.Body);
            //        TempData[Globals.SuccessMessageKey] = App_GlobalResources.Wording.UpdateSuccess;
            //        if (Request["IsPopup"] == "true" || Request["IsPopup"] == "True")
            //        {
            //            ViewBag.closePopup = "true";
            //            model.Id = SMSLog.Id;
            //            ViewBag.urlRefer = urlRefer;
            //            return View(model);
            //        }
            //        return Redirect(urlRefer);
            //    }

            if (TargetModule != "")
            {
                return RedirectToAction("Detail", "Campaign", new { id = TargetID });
                
            }
            else
            {
                return RedirectToAction("Index", "SMSLog");
                
            }
        }
        public ActionResult Editbodyone()
        {
            return View();
        }
        //[HttpPost]
        //public ActionResult Editbodyone()
        //{
        //    return View();
        //}

        #endregion

        #region Delete

        public ActionResult Delete()
        {
            //var urlRefer = Request["UrlReferrerDelete"];
            try
            {
                string idDeleteAll = Request["DeleteId-checkboxSMS"];
                string[] arrDeleteId = idDeleteAll.Split(',');
                for (int i = 0; i < arrDeleteId.Count(); i++)
                {
                    var item = SMSLogRepository.GetSMSLogById(int.Parse(arrDeleteId[i], CultureInfo.InvariantCulture));
                    if (item != null)
                    {
                        if (item.CreatedUserId != Erp.BackOffice.Helpers.Common.CurrentUser.Id && Erp.BackOffice.Helpers.Common.CurrentUser.UserTypeId != 1)
                        {
                            TempData["FailedMessage"] = "NotOwner";
                            return RedirectToAction("Index");
                        }
                        item.IsDeleted = true;
                        SMSLogRepository.UpdateSMSLog(item);
                    }
                }
                TempData[Globals.SuccessMessageKey] = App_GlobalResources.Wording.DeleteSuccess;
                return RedirectToAction("Index", "SMSLog");
            }
            catch (DbUpdateException)
            {
                TempData[Globals.FailedMessageKey] = App_GlobalResources.Error.RelationError;
                return RedirectToAction("Index");
            }
        }
        #endregion

        public static void SaveSMS(string phone, string body, int? customerID, int TargetID, string TargetModule, string subj)
        {
            Erp.Domain.Crm.Repositories.SMSLogRepository smsLogRepository = new Erp.Domain.Crm.Repositories.SMSLogRepository(new Domain.Crm.ErpCrmDbContext());
            var SMSLog = new Domain.Crm.Entities.SMSLog();
            SMSLog.IsDeleted = false;
            SMSLog.CreatedUserId = WebSecurity.CurrentUserId;
            SMSLog.ModifiedUserId = WebSecurity.CurrentUserId;
            SMSLog.CreatedDate = DateTime.Now;
            SMSLog.ModifiedDate = DateTime.Now;
            SMSLog.SentDate = DateTime.Now;
            SMSLog.Status = "Đã gửi";
            SMSLog.Body = body;
            SMSLog.Phone = phone;
            SMSLog.CustomerID = customerID.Value;
            SMSLog.TargetID = TargetID;
            SMSLog.TargetModule = TargetModule;

            smsLogRepository.InsertSMSLog(SMSLog);
        }
    }
}
