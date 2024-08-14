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
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using System.Web.Script.Serialization;
using Newtonsoft.Json.Linq;
using System.Transactions;
using System.Net;
using Erp.Domain.Crm.Repositories;

namespace Erp.BackOffice.Crm.Controllers
{
    [Authorize]
    [InitializeSimpleMembership]
    [Erp.BackOffice.Helpers.NoCacheHelper]
    public class CALLogController : Controller
    {
        private readonly ISMSLogRepository SMSLogRepository;
        private readonly IUserRepository userRepository;
        private readonly ICALLogRepository CALLogRepository;

        public CALLogController(ISMSLogRepository _SMSLog, IUserRepository _user, ICALLogRepository _Callog)
        {
            SMSLogRepository = _SMSLog;
            userRepository = _user;
            CALLogRepository = _Callog;
        }


        #region Index
        public ViewResult Index(string startDate, string endDate, string txtSogoi, string txtSonhan, string txtANSWER)
        {
            IEnumerable<CALLogViewModel> q = CALLogRepository.GetAllCALLog()
                .Select(item => new CALLogViewModel
                {
                    Id = item.Id,
                    CreatedUserId = item.CreatedUserId,

                    CreatedDate = item.CreatedDate,
                    ModifiedUserId = item.ModifiedUserId,

                    ModifiedDate = item.ModifiedDate,
                    stt = item.stt,
                    keylog = item.keylog,
                    ngaygoi = item.ngaygoi,
                    sogoidien = item.sogoidien,
                    dauso = item.dauso,
                    sonhan = item.sonhan,
                    trangthai = item.trangthai,
                    tongthoigiangoi = item.tongthoigiangoi,
                    thoigianthucgoi = item.thoigianthucgoi,
                    linkfile = item.linkfile,
                    CallDate = item.CallDate

                }).OrderByDescending(m => m.ngaygoi).ToList();
            foreach (var i in q)
            {
                DateTime DateCall;
                DateTime.TryParseExact(i.CallDate, "dd/MM/yyyy", new CultureInfo("vi-VN"), DateTimeStyles.None, out DateCall);
                i.Date = DateCall;
            }
            ViewBag.SuccessMessage = TempData["SuccessMessage"];
            ViewBag.FailedMessage = TempData["FailedMessage"];
            ViewBag.AlertMessage = TempData["AlertMessage"];

            DateTime d_startDate, d_endDate;
            if (DateTime.TryParseExact(startDate, "dd/MM/yyyy", new CultureInfo("vi-VN"), DateTimeStyles.None, out d_startDate))
            {
                if (DateTime.TryParseExact(endDate, "dd/MM/yyyy", new CultureInfo("vi-VN"), DateTimeStyles.None, out d_endDate))
                {

                    d_endDate = d_endDate.AddHours(23).AddMinutes(59);
                    q = q.Where(x => x.Date >= d_startDate && x.Date <= d_endDate).ToList();
                }
            }
            if (!string.IsNullOrEmpty(txtSogoi))
            {
                q = q.Where(item => Erp.BackOffice.Helpers.Common.ChuyenThanhKhongDau(item.sogoidien).ToLower().Contains(Erp.BackOffice.Helpers.Common.ChuyenThanhKhongDau(txtSogoi).ToLower())).ToList();
            }
            if (!string.IsNullOrEmpty(txtSonhan))
            {
                q = q.Where(item => Erp.BackOffice.Helpers.Common.ChuyenThanhKhongDau(item.sonhan).ToLower().Contains(Erp.BackOffice.Helpers.Common.ChuyenThanhKhongDau(txtSonhan).ToLower())).ToList();
            }
            if (!string.IsNullOrEmpty(txtANSWER))
            {
                q = q.Where(item => item.trangthai == txtANSWER).ToList();
            }

            return View(q);
        }
        #endregion
        #region Delete
        [HttpPost]
        public ActionResult Delete()
        {
            try
            {
                string id = Request["Delete"];
                if (id != null)
                {
                    var item = CALLogRepository.GetCALLogById(int.Parse(id, CultureInfo.InvariantCulture));
                    if (item != null)
                    {
                        if (item.CreatedUserId != Helpers.Common.CurrentUser.Id && Helpers.Common.CurrentUser.UserTypeId != 1)
                        {
                            TempData["FailedMessage"] = "NotOwner";
                            return RedirectToAction("Index");
                        }

                        item.IsDeleted = true;
                        CALLogRepository.UpdateCALLog(item);
                    }
                }
                else
                {
                    string idDeleteAll = Request["DeleteId-checkbox"];
                    string[] arrDeleteId = idDeleteAll.Split(',');
                    for (int i = 0; i < arrDeleteId.Count(); i++)
                    {
                        var item = CALLogRepository.GetCALLogById(int.Parse(arrDeleteId[i], CultureInfo.InvariantCulture));
                        if (item != null)
                        {
                            if (item.CreatedUserId != Helpers.Common.CurrentUser.Id && Helpers.Common.CurrentUser.UserTypeId != 1)
                            {
                                TempData["FailedMessage"] = "NotOwner";
                                return RedirectToAction("Index");
                            }

                            item.IsDeleted = true;
                            CALLogRepository.UpdateCALLog(item);
                        }
                    }
                }
                
                TempData[Globals.SuccessMessageKey] = App_GlobalResources.Wording.DeleteSuccess;
                return Redirect("Index");
            }
            catch (DbUpdateException)
            {
                TempData[Globals.FailedMessageKey] = App_GlobalResources.Error.RelationError;
                return Redirect("Index");
            }
        }
        #endregion

        #region  LayLog

        private string Filter(string s)
        {
            var chars = new[] { '*', '/', '#' };
            var filteredChars = s.ToArray();
            return new string(filteredChars
                     .Where(ch => !chars.Contains(ch))
                     .Select(ch => ch == ' ' ? '-' : ch).ToArray());
        }

        public string RemoveSpecialChars(string str)
        {
            // Create  a string array and add the special characters you want to remove
            //You can include / exclude more special characters based on your needs
            string[] chars = new string[] { ",", ".", "/", "!", "@", "#", "$", "%", "^", "&", "*", "'", "\"", ";", "_", "(", ")", ":", "|", "[", "]" };
            //Iterate the number of times based on the String array length.
            //1.gif
            return str;
        }
        public ActionResult LayLog(string startDate, string endDate)
        {



            DateTime start_d;
            DateTime end_d = DateTime.Now;
            if (DateTime.TryParseExact(startDate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out start_d))
            {

                if (DateTime.TryParseExact(endDate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out end_d))
                {
                    end_d = end_d.AddHours(23).AddMinutes(59).AddSeconds(59);
                }
            }

            string sstart_d = start_d.ToString("yyyy-MM-dd");
            string send_d = end_d.ToString("yyyy-MM-dd");

            string uri = "https://api.cloudfone.vn/api/CloudFone/GetCallHistory";

            var client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("AppPlatform", "Web");
            client.DefaultRequestHeaders.Add("AppName", "vcall");
            client.DefaultRequestHeaders.Add("AppVersion", "1.0");
            var values = new Dictionary<string, string>()
                {
                    {"ServiceName",Helpers.Common.GetSetting("ServiceName")},
                    {"AuthUser", Helpers.Common.GetSetting("AuthUser")},
                    {"AuthKey", Helpers.Common.GetSetting("AuthKey")},
                    {"TypeGet", "0"},
                    {"DateStart",sstart_d},
                    {"DateEnd",send_d},
                    {"CallNum",""},
                    {"ReceiveNum",""},
                    {"Key",""},
                    {"PageIndex","1"},
                    {"PageSize","200"},

                };
            var content = new FormUrlEncodedContent(values);
            int PageIndex = 1;
            int PageCount = 1;
            //begin xac dinh so trang
            using (var response = client.PostAsync(uri, content).Result)
            {
                string responseString = response.Content.ReadAsStringAsync().Result;
                Newtonsoft.Json.Linq.JObject json = Newtonsoft.Json.Linq.JObject.Parse(responseString);
                Console.WriteLine(response.StatusCode);
                if (response.StatusCode.ToString() == "OK")
                {
                    var rootObject = new JavaScriptSerializer().Deserialize<ListJsonCALLogViewModel>(json.ToString());
                    PageCount = rootObject.total / 200;
                    if (PageCount * 200 < rootObject.total)
                    {
                        PageCount = PageCount + 1;
                    }


                }
            }
            //end xac sinh so trang

            for (int ir = 1; ir <= PageCount; ir++)
            {

                values = new Dictionary<string, string>()
                {
                    {"ServiceName",Helpers.Common.GetSetting("ServiceName")},
                    {"AuthUser", Helpers.Common.GetSetting("AuthUser")},
                    {"AuthKey", Helpers.Common.GetSetting("AuthKey")},
                    {"TypeGet", "0"},
                    {"DateStart",sstart_d},
                    {"DateEnd",send_d},
                    {"CallNum",""},
                    {"ReceiveNum",""},
                    {"Key",""},
                    {"PageIndex",ir.ToString()},
                    {"PageSize","200"},

                };
                content = new FormUrlEncodedContent(values);

                using (var response = client.PostAsync(uri, content).Result)
                {
                    string responseString = response.Content.ReadAsStringAsync().Result;
                    Newtonsoft.Json.Linq.JObject json = Newtonsoft.Json.Linq.JObject.Parse(responseString);
                    Console.WriteLine(response.StatusCode);
                    if (response.StatusCode.ToString() == "OK")
                    {


                        //ListJsonCALLogViewModel obj = JsonConvert.DeserializeObject<ListJsonCALLogViewModel>(json.ToString());

                        var rootObject = new JavaScriptSerializer().Deserialize<ListJsonCALLogViewModel>(json.ToString());
                        using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.Snapshot }))
                        {
                            foreach (var i in rootObject.data)
                            {

                                var calllog1 = CALLogRepository.GetCALLogByKeyLog(WebUtility.HtmlDecode(i.key.ToLower().Replace("Diđộng\n".ToLower(), "")));
                                if (calllog1 == null)
                                {
                                    var Callog = new CALLog();
                                    Callog.IsDeleted = false;
                                    Callog.CreatedDate = DateTime.Now;
                                    Callog.CreatedUserId = WebSecurity.CurrentUserId;
                                    Callog.stt = i.stt;
                                    Callog.dauso = WebUtility.HtmlDecode(i.dauSo);
                                    Callog.keylog = WebUtility.HtmlDecode(i.key.ToLower().Replace("Diđộng\n".ToLower(), ""));
                                    Callog.sogoidien = WebUtility.HtmlDecode(i.soGoiDen);
                                    Callog.sonhan = WebUtility.HtmlDecode(i.soNhan.ToLower().Replace("Diđộng\n".ToLower(), ""));
                                    Callog.thoigianthucgoi = i.thoiGianThucGoi;
                                    Callog.tongthoigiangoi = i.tongThoiGianGoi;
                                    Callog.trangthai = i.trangThai;
                                    Callog.linkfile = i.linkFile;
                                    Callog.ngaygoi = i.ngayGoi;
                                    Callog.CallDate = DateTimeOffset.Parse(i.ngayGoi).UtcDateTime.ToString("dd/MM/yyyy");
                                    CALLogRepository.InsertCALLog(Callog);
                                }

                            }
                            scope.Complete();
                        }

                    }
                }
            }
            return RedirectToAction("Index");
        }
        public static void LayLogEditByLANH(string startDate, string endDate)
        {

           
            DateTime start_d;
            DateTime end_d = DateTime.Now;
            if (DateTime.TryParseExact(startDate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out start_d))
            {

                if (DateTime.TryParseExact(endDate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out end_d))
                {
                    end_d = end_d.AddHours(23).AddMinutes(59).AddSeconds(59);
                }
            }

            string sstart_d = start_d.ToString("yyyy-MM-dd");
            string send_d = end_d.ToString("yyyy-MM-dd HHmmss");

            string uri = "https://api.cloudfone.vn/api/CloudFone/GetCallHistory";

            var client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("AppPlatform", "Web");
            client.DefaultRequestHeaders.Add("AppName", "vcall");
            client.DefaultRequestHeaders.Add("AppVersion", "1.0");
            var values = new Dictionary<string, string>()
                {
                    {"ServiceName",Helpers.Common.GetSetting("ServiceName")},
                    {"AuthUser", Helpers.Common.GetSetting("AuthUser")},
                    {"AuthKey", Helpers.Common.GetSetting("AuthKey")},
                    {"TypeGet", "0"},
                    {"DateStart",sstart_d},
                    {"DateEnd",send_d},
                    {"CallNum",""},
                    {"ReceiveNum",""},
                    {"Key",""},
                    {"PageIndex","1"},
                    {"PageSize","200"},

                };
            var content = new FormUrlEncodedContent(values);
            int PageIndex = 1;
            int PageCount = 1;
            //begin xac dinh so trang
            using (var response = client.PostAsync(uri, content).Result)
            {
                string responseString = response.Content.ReadAsStringAsync().Result;
                Newtonsoft.Json.Linq.JObject json = Newtonsoft.Json.Linq.JObject.Parse(responseString);
                Console.WriteLine(response.StatusCode);
                if (response.StatusCode.ToString() == "OK")
                {
                    var rootObject = new JavaScriptSerializer().Deserialize<ListJsonCALLogViewModel>(json.ToString());
                    PageCount = rootObject.total / 200;
                    if (PageCount * 200 < rootObject.total)
                    {
                        PageCount = PageCount + 1;
                    }


                }
            }
            //end xac sinh so trang

            for (int ir = 1; ir <= PageCount; ir++)
            {

                values = new Dictionary<string, string>()
                {
                    {"ServiceName",Helpers.Common.GetSetting("ServiceName")},
                    {"AuthUser", Helpers.Common.GetSetting("AuthUser")},
                    {"AuthKey", Helpers.Common.GetSetting("AuthKey")},
                    {"TypeGet", "0"},
                    {"DateStart",sstart_d},
                    {"DateEnd",send_d},
                    {"CallNum",""},
                    {"ReceiveNum",""},
                    {"Key",""},
                    {"PageIndex",ir.ToString()},
                    {"PageSize","200"},

                };
                content = new FormUrlEncodedContent(values);

                using (var response = client.PostAsync(uri, content).Result)
                {
                    string responseString = response.Content.ReadAsStringAsync().Result;
                    Newtonsoft.Json.Linq.JObject json = Newtonsoft.Json.Linq.JObject.Parse(responseString);
                    Console.WriteLine(response.StatusCode);
                    if (response.StatusCode.ToString() == "OK")
                    {


                        //ListJsonCALLogViewModel obj = JsonConvert.DeserializeObject<ListJsonCALLogViewModel>(json.ToString());

                        var rootObject = new JavaScriptSerializer().Deserialize<ListJsonCALLogViewModel>(json.ToString());
                        using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.Snapshot }))
                        {
                            foreach (var i in rootObject.data)
                            {
                                CALLogRepository CALLogRepository = new CALLogRepository(new Erp.Domain.Crm.ErpCrmDbContext());
                             
                                  var calllog1 = CALLogRepository.GetCALLogByKeyLog(WebUtility.HtmlDecode(i.key.ToLower().Replace("Diđộng\n".ToLower(), "")));
                                if (calllog1 == null)
                                {
                                    var Callog = new CALLog();
                                    Callog.IsDeleted = false;
                                    Callog.CreatedDate = DateTime.Now;
                                    Callog.CreatedUserId =2460;
                                    Callog.stt = i.stt;
                                    Callog.dauso = WebUtility.HtmlDecode(i.dauSo);
                                    Callog.keylog = WebUtility.HtmlDecode(i.key.ToLower().Replace("Diđộng\n".ToLower(), ""));
                                    Callog.sogoidien = WebUtility.HtmlDecode(i.soGoiDen);
                                    Callog.sonhan = WebUtility.HtmlDecode(i.soNhan.ToLower().Replace("Diđộng\n".ToLower(), ""));
                                    Callog.thoigianthucgoi = i.thoiGianThucGoi;
                                    Callog.tongthoigiangoi = i.tongThoiGianGoi;
                                    Callog.trangthai = i.trangThai;
                                    Callog.linkfile = i.linkFile;
                                    Callog.ngaygoi = i.ngayGoi;
                                    Callog.CallDate = DateTimeOffset.Parse(i.ngayGoi).UtcDateTime.ToString("dd/MM/yyyy");
                                    CALLogRepository.InsertCALLog(Callog);
                                }

                            }
                            scope.Complete();
                        }

                    }
                }
            }
           
        }
        #endregion
    }

}
