using DocumentFormat.OpenXml.Drawing.Charts;
using Erp.BackOffice.Areas.Crm.Models;
using Erp.BackOffice.Crm.Models;
using Erp.BackOffice.Filters;
using Erp.BackOffice.Hubs;
using Erp.Domain.Crm.Entities;
using Erp.Domain.Crm.Helper;
using Erp.Domain.Crm.Interfaces;
using Erp.Domain.Entities;
using Erp.Domain.Interfaces;
using Erp.Domain.Repositories;
using Erp.Utilities;
using Microsoft.AspNet.SignalR;
using Newtonsoft.Json.Linq;
using System;
using System.Configuration;
using System.Data.Entity.Infrastructure;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using WebMatrix.WebData;

namespace Erp.BackOffice.Crm.Controllers
{
    [System.Web.Http.Authorize]
    [InitializeSimpleMembership]
    [Erp.BackOffice.Helpers.NoCacheHelper]
    public class AnswerController : Controller
    {
        private readonly IAnswerRepository AnswerRepository;
        private readonly IUserRepository userRepository;
        private static string _appId = ConfigurationManager.AppSettings["ZaloOA:AppId"];
        private static string _secretKey = ConfigurationManager.AppSettings["ZaloOA:SecretKey"];

        public AnswerController()
        {
        }
        public AnswerController(
            IAnswerRepository _Answer
            , IUserRepository _user
            )
        {
            AnswerRepository = _Answer;
            userRepository = _user;
        }
        #region Index

        public ViewResult Index(string txtSearch)
        {

            IQueryable<AnswerViewModel> q = AnswerRepository.GetAllAnswer()
                .Select(item => new AnswerViewModel
                {
                    Id = item.Id,
                    CreatedUserId = item.CreatedUserId,
                    //CreatedUserName = item.CreatedUserName,
                    CreatedDate = item.CreatedDate,
                    ModifiedUserId = item.ModifiedUserId,
                    //ModifiedUserName = item.ModifiedUserName,
                    ModifiedDate = item.ModifiedDate,
                }).OrderByDescending(m => m.ModifiedDate);

            ViewBag.SuccessMessage = TempData["SuccessMessage"];
            ViewBag.FailedMessage = TempData["FailedMessage"];
            ViewBag.AlertMessage = TempData["AlertMessage"];
            return View(q);
        }
        #endregion

        #region Create
        public ViewResult Create()
        {
            var model = new AnswerViewModel();

            return View(model);
        }

        [HttpPost]
        public ActionResult Create(AnswerViewModel model)
        {
            if (ModelState.IsValid)
            {
                var Answer = new Answer();
                AutoMapper.Mapper.Map(model, Answer);
                Answer.IsDeleted = false;
                Answer.CreatedUserId = WebSecurity.CurrentUserId;
                Answer.ModifiedUserId = WebSecurity.CurrentUserId;
                Answer.AssignedUserId = WebSecurity.CurrentUserId;
                Answer.CreatedDate = DateTime.Now;
                Answer.ModifiedDate = DateTime.Now;
                AnswerRepository.InsertAnswer(Answer);

                TempData[Globals.SuccessMessageKey] = App_GlobalResources.Wording.InsertSuccess;
                return RedirectToAction("Index");
            }
            return View(model);
        }
        #endregion

        #region Edit
        public ActionResult Edit(int? Id)
        {
            var Answer = AnswerRepository.GetAnswerById(Id.Value);
            if (Answer != null && Answer.IsDeleted != true)
            {
                var model = new AnswerViewModel();
                AutoMapper.Mapper.Map(Answer, model);

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
        public ActionResult Edit(AnswerViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (Request["Submit"] == "Save")
                {
                    var Answer = AnswerRepository.GetAnswerById(model.Id);
                    AutoMapper.Mapper.Map(model, Answer);
                    Answer.ModifiedUserId = WebSecurity.CurrentUserId;
                    Answer.ModifiedDate = DateTime.Now;
                    AnswerRepository.UpdateAnswer(Answer);

                    TempData[Globals.SuccessMessageKey] = App_GlobalResources.Wording.UpdateSuccess;
                    return RedirectToAction("Index");
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
            var Answer = AnswerRepository.GetAnswerById(Id.Value);
            if (Answer != null && Answer.IsDeleted != true)
            {
                var model = new AnswerViewModel();
                AutoMapper.Mapper.Map(Answer, model);

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
                    var item = AnswerRepository.GetAnswerById(int.Parse(arrDeleteId[i], CultureInfo.InvariantCulture));
                    if (item != null)
                    {
                        if (item.CreatedUserId != Helpers.Common.CurrentUser.Id && Helpers.Common.CurrentUser.UserTypeId != 1)
                        {
                            TempData["FailedMessage"] = "NotOwner";
                            return RedirectToAction("Index");
                        }

                        item.IsDeleted = true;
                        AnswerRepository.UpdateAnswer(item);
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

        #region Tuấn Anh - ZaloOA

        #region GetAccessTokenUsingAuthorizationCode
        //Hàm lấy access token
        private (string accessToken, DateTime expireAt, string refreshToken) GetAccessTokenUsingAuthorizationCode(string authorizationCode)
        {
            string accessTokenUrl = "https://oauth.zaloapp.com/v4/oa/access_token";
            string grantType = "authorization_code";

            using (WebClient client = new WebClient())
            {
                client.Headers.Add("Secret_key", _secretKey);

                var requestData = new System.Collections.Specialized.NameValueCollection();
                requestData["app_id"] = _appId;
                requestData["code"] = authorizationCode;
                requestData["grant_type"] = grantType;

                byte[] responseBytes = client.UploadValues(accessTokenUrl, "POST", requestData);
                string response = System.Text.Encoding.UTF8.GetString(responseBytes);

                JObject jsonResponse = JObject.Parse(response);
                if (jsonResponse["access_token"] != null)
                {
                    string accessToken = jsonResponse["access_token"].ToString();
                    // Lấy thời gian hết hạn của access_token
                    DateTime expireAt = DateTime.UtcNow.AddSeconds(int.Parse(jsonResponse["expires_in"].ToString()));
                    string refreshToken = jsonResponse["refresh_token"]?.ToString();
                    // Trả về cặp giá trị (accessToken, expireAt, refreshToken)
                    return (accessToken, expireAt, refreshToken);
                }
                else
                {
                    throw new Exception("Access token not found in response.");
                }
            }
        }



        #endregion

        #region GetAccessTokenByRefreshToken
        private (string accessToken, string refreshToken, DateTime expireAt) GetTokensUsingRefreshToken(string refreshTokenDB)
        {
            string accessTokenUrl = "https://oauth.zaloapp.com/v4/oa/access_token";
            string grantType = "refresh_token";

            using (WebClient client = new WebClient())
            {
                client.Headers.Add("Secret_key", _secretKey);

                var requestData = new System.Collections.Specialized.NameValueCollection();
                requestData["app_id"] = _appId;
                requestData["refresh_token"] = refreshTokenDB;
                requestData["grant_type"] = grantType;

                byte[] responseBytes = client.UploadValues(accessTokenUrl, "POST", requestData);
                string response = System.Text.Encoding.UTF8.GetString(responseBytes);

                JObject jsonResponse = JObject.Parse(response);
                if (jsonResponse["access_token"] != null && jsonResponse["refresh_token"] != null)
                {
                    string accessToken = jsonResponse["access_token"].ToString();
                    string refreshToken = jsonResponse["refresh_token"].ToString();
                    // Lấy thời gian hết hạn của access_token
                    DateTime expireAt = DateTime.UtcNow.AddSeconds(int.Parse(jsonResponse["expires_in"].ToString()));
                    // Trả về cặp giá trị (accessToken, refreshToken, expireAt)
                    return (accessToken, refreshToken, expireAt);
                }
                else
                {
                    return (null, null, DateTime.Now); ;
                    //throw new Exception("Access token or refresh token not found in response.");
                }
            }
        }

        #region ScheduleRefreshTokens - Lập lịch tự động lấy accesstoken zalo
        public void ScheduleRefreshTokens()
        {
            var result = SqlHelper.QuerySP<dynamic>("sp_Crm_GetLatestRefreshToken&ExpireAt&AccessToken").FirstOrDefault();

            if (result != null)
            {
                string refreshToken = result.RefreshToken?.ToString();
                string accessToken = result.Token?.ToString();  

                DateTime expireAt = (DateTime)result.ExpireAt;

                // Trừ đi 1 tiếng từ thời gian hết hạn hiện tại
                DateTime expireAtMinusOneHour = expireAt.AddHours(-1);
                TimeZoneInfo vnTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");

                // Chuyển đổi thời gian hiện tại từ múi giờ UTC sang múi giờ Việt Nam
                DateTime currentTimeVn = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vnTimeZone);
                // Kiểm tra xem thời gian hết hạn đã đến gần trong vòng 1 tiếng hay không
                if (expireAtMinusOneHour <= currentTimeVn)
                {
                    var (newAccessToken, newRefreshToken, newExpireAt) = GetTokensUsingRefreshToken(refreshToken);
                    InsertAccessToken(newAccessToken, newExpireAt, newRefreshToken);
                    var data = SqlHelper.QuerySP<string>("sp_Crm_UpdateAccessTokenStatus", new
                    {
                        pAccessToken = accessToken
                    }).ToList();
                }
            }
        }

        #endregion





        #endregion
        #region InsertAccessToken
        private void InsertAccessToken(string accessToken, DateTime expireAt, string refreshToken)
        {
            if (accessToken != null)
            {
                var data = SqlHelper.QuerySP<string>("sp_Crm_InsertAccessTokenZalo", new
                {
                    Token = accessToken,
                    ExpireAt = expireAt,
                    Refresh = refreshToken
                }).ToList();
            }


        }
        #endregion

        #region Callback
        public ActionResult Callback(string code)
        {
            // Lấy access token mới nhất từ cơ sở dữ liệu
            var accessTokenData = SqlHelper.QuerySP<dynamic>("sp_Crm_GetLatestAccessToken").FirstOrDefault();

            // Kiểm tra xem access token có tồn tại và isActive không
            if (accessTokenData != null)
            {
                string accessTokenFromDatabase = accessTokenData.Token.ToString();

                // Lấy thông tin Zalo OA và hiển thị trên View
                var oaInfo = GetZaloOAInfo(accessTokenFromDatabase);
                if (oaInfo != null)
                {
                    ViewBag.AccessToken = accessTokenFromDatabase;
                    ViewBag.OAInfo = oaInfo;

                    return View();
                }
            }

            else
            {
                var (accessToken, expireAt,refreshToken) = GetAccessTokenUsingAuthorizationCode(code);

                // Lưu access token mới vào cơ sở dữ liệu
                InsertAccessToken(accessToken, expireAt, refreshToken);

                if (!string.IsNullOrEmpty(accessToken))
                {
                    // Lấy thông tin Zalo OA
                    var oaInfo = GetZaloOAInfo(accessToken);

                    if (oaInfo == null)
                    {
                        // Nếu không lấy được thông tin Zalo OA, đánh dấu access token là không hoạt động
                        var data = SqlHelper.QuerySP<string>("sp_Crm_UpdateAccessTokenStatus", new
                        {
                            pAccessToken = accessToken
                        }).ToList();
                    }
                    else
                    {
                        ViewBag.AccessToken = accessToken;
                        ViewBag.OAInfo = oaInfo;


                        // Lưu thông tin khi kết nối Zalo OA vào cơ sở dữ liệu
                        int userId = WebSecurity.CurrentUserId;
                        ConnectZaloOA(oaInfo.ZaloOAId, userId);

                        // Lưu ZaloOAId vào Session
                        Session["ZaloOAId"] = oaInfo.ZaloOAId.ToString();
                    }
                }
            }

            return View();
        }
        #endregion






        #region GetZaloOAInfo
        //Hàm lấy thông tin Zalo OA
        private ZaloOAViewModel GetZaloOAInfo(string accessToken)
        {
            if (string.IsNullOrEmpty(accessToken))
            {
                return null;
            }

            string apiUrl = "https://openapi.zalo.me/v2.0/oa/getoa";

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(apiUrl);
            request.Method = "GET";
            request.Headers.Add("access_token", accessToken);

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    throw new Exception($"Error: {response.StatusCode}");
                }

                using (var streamReader = new System.IO.StreamReader(response.GetResponseStream()))
                {
                    string responseJson = streamReader.ReadToEnd();

                    JObject jsonResponse = JObject.Parse(responseJson);
                    if (jsonResponse != null && jsonResponse["data"] != null)
                    {
                        string name = jsonResponse["data"]["name"]?.ToString();
                        string avatar = jsonResponse["data"]["avatar"]?.ToString();
                        string id = jsonResponse["data"]["oa_id"]?.ToString();

                        ZaloOAViewModel oaInfo = new ZaloOAViewModel(); // Tạo một đối tượng ZaloOAInfo
                        oaInfo.Name = name;
                        oaInfo.Avatar = avatar;
                        oaInfo.ZaloOAId = id;
                        return oaInfo;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
        }
        #endregion

        #region GetCustomerProfile.
        [AllowAnonymous]
        public ZaloCustomerViewModel GetCustomerInfoFromZalo(string accessToken, string userId)
        {
            if (string.IsNullOrEmpty(accessToken) || string.IsNullOrEmpty(userId))
            {
                return null;
            }

            string apiUrl = $"https://openapi.zalo.me/v3.0/oa/user/detail?data={{\"user_id\":\"{userId}\"}}";

            // Tạo yêu cầu HTTP GET
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(apiUrl);
            request.Method = "GET";
            request.Headers.Add("access_token", accessToken);

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    throw new Exception($"Error: {response.StatusCode}");
                }

                using (var streamReader = new System.IO.StreamReader(response.GetResponseStream()))
                {
                    string responseJson = streamReader.ReadToEnd();

                    JObject jsonResponse = JObject.Parse(responseJson);
                    if (jsonResponse != null && jsonResponse["data"] != null)
                    {
                        var infoData = jsonResponse["data"];
                        string id = infoData["user_id"]?.ToString();
                        string name = infoData["display_name"]?.ToString();

                        string address = null;
                        string city = null;
                        string district = null;
                        string phone = null;
                        if (infoData["shared_info"] != null)
                        {
                            address = infoData["shared_info"]?["address"]?.ToString();
                            city = infoData["shared_info"]?["city"]?.ToString();
                            district = infoData["shared_info"]?["district"]?.ToString();
                            phone = infoData["shared_info"]?["phone"]?.ToString();
                        }

                        //Tạo đối tượng ZaloCustomerViewModel từ thông tin lấy được
                        ZaloCustomerViewModel customer = new ZaloCustomerViewModel
                        {
                            UserIdZalo = id,
                            Name = name,
                            Mobile = phone,
                            Address = address,
                            City = city,
                            District = district
                        };

                        return customer;
                    }
                }
            }

            return null;
        }
        #endregion



        #region Disconnect
        public ActionResult Disconnect()
        {
            string zaloOAId = Session["ZaloOAId"]?.ToString();
            int userId = WebSecurity.CurrentUserId;

            // Xóa thông tin Zalo OA khỏi cơ sở dữ liệu
            DisconnectZaloOA(zaloOAId, userId);
            var accessTokenData = SqlHelper.QuerySP<dynamic>("sp_Crm_GetLatestAccessToken").FirstOrDefault();

                var data = SqlHelper.QuerySP<string>("sp_Crm_UpdateAccessTokenStatus", new
            {
                pAccessToken = accessTokenData.Token
            }).ToList();
            return RedirectToAction("IndexZaloOA");
        }

        #endregion


        #region IndexZaloOA
        public ActionResult IndexZaloOA()
        {
            //Cập nhật trạng thái hoạt động của xaccess_token
            SqlHelper.ExecuteSP("sp_Crm_UpdateStatusAccessTokens");
            var accessTokenData = SqlHelper.QuerySP<dynamic>("sp_Crm_GetLatestAccessToken").FirstOrDefault();
            if (accessTokenData != null)
                return RedirectToAction("Callback");
            else
                return View();

        }
        #endregion


        #region GetAuthorizationCode
        //Hàm lấy Authorize code , xin cấp quyền
        public ActionResult GetAuthorizationCode()
        {
            string callbackUrl = Url.Action("Callback", "Answer", null, Request.Url.Scheme);
            string authorizeUrl = GetAuthorizationUrl(callbackUrl);
            return Redirect(authorizeUrl);
        }
        #endregion


        #region GetAuthorizationUrl
        //Hàm lấy URL để xin quyền truy cập
        private string GetAuthorizationUrl(string callbackUrl)
        {
            return $"https://oauth.zaloapp.com/v4/oa/permission?app_id={_appId}&redirect_uri={callbackUrl}";
        }
        #endregion

        #region SaveUserInfoToDatabase
        public int SaveUserInfoToDatabase(string accessToken, string userId,string link,string message)
        {
            var customerInfo = GetCustomerInfoFromZalo(accessToken, userId);
            if (customerInfo != null)
            {
                var data = SqlHelper.QuerySP<dynamic>("sp_Crm_InsertZaloCustomerInfo", new
                {
                    pUserIdZalo = customerInfo.UserIdZalo,
                    pName = customerInfo.Name,
                    pMobile = customerInfo.Mobile,
                    pCity = customerInfo.City,
                    pDistrict = customerInfo.District,
                    pAddress = customerInfo.Address,
                    pLink = link,
                    pMessage = message
                }).FirstOrDefault();
                if (data.leadId > 0)
                    return 1;
            }
            return 0;
        }
        #endregion


        #region Webhook
        [AllowAnonymous]
        [HttpPost]
        public ActionResult Webhook()
        
        {
            // Đọc dữ liệu từ yêu cầu Webhook
            string jsonData = new StreamReader(Request.InputStream).ReadToEnd();

            // Xử lý dữ liệu Webhook từ Zalo
            JObject webhookData = JObject.Parse(jsonData);

            // Kiểm tra xem sự kiện có phải là user_send_text không
            if (webhookData["event_name"]?.ToString() == "user_send_text")
            {

                // Lấy thông tin tin nhắn từ dữ liệu Webhook
                var messageData = webhookData["message"];
                string messageContent = messageData["text"].ToString();
                var senderData = webhookData["sender"];
                string customerID = senderData["id"].ToString();

                Session["CustomerMessage"] = messageContent;

                DateTime sentTime = DateTime.Now;
                string formattedTime = sentTime.ToString("HH:mm:ss dd/MM/yyyy");

                var accessTokenData = SqlHelper.QuerySP<dynamic>("sp_Crm_GetLatestAccessToken").FirstOrDefault();

                //var userIdZaloData = SqlHelper.QuerySP<int>("sp_Crm_CheckIDZaloFromLead", new { CustomerID = customerID }).FirstOrDefault();


                if (accessTokenData != null)
                {
                    var customerInfo = GetCustomerInfoFromZalo(accessTokenData.Token.ToString(), customerID);
                    if (customerInfo != null)
                    {
                        var oaId = SqlHelper.QuerySP<string>("sp_Crm_GetZaloOaIdNew").FirstOrDefault();
                        string link = "https://oa.zalo.me/chatv2?uid=" + customerID + "&oaid=" + oaId;
                        int leadId = SaveUserInfoToDatabase(accessTokenData.Token.ToString(), customerID, link, messageContent);
                        if(leadId > 0)
                        {
                            var listUserIdhaveCareZalo = SqlHelper.QuerySP<string>("[Crm_UsersCareZalo_Get]", new
                            {
                            }).ToList();
                            var hubContext = GlobalHost.ConnectionManager.GetHubContext<ZaloHub>();
                            hubContext.Clients.Clients(listUserIdhaveCareZalo).SendZaloNotification(messageContent, customerID, formattedTime);
                        }    
                    }
                }

            
                return new HttpStatusCodeResult(HttpStatusCode.OK);
            }
            else
            {
                // Trả về mã lỗi nếu sự kiện không phải là user_send_text
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Invalid event type.");
            }
        }





        #endregion
        #region GetLeadIdByUserIdZalo
        [HttpGet]
        [AllowAnonymous]
        public ActionResult GetLeadIdByUserIdZalo(string userIdZalo)
        {
            try
            {
                // Sử dụng SqlHelper để gọi stored procedure và lấy Id
                var leadId = SqlHelper.QuerySP<string>("sp_Crm_GetLeadIdByUserIdZalo", new { UserIdZalo = userIdZalo }).FirstOrDefault();

                // Trả về Id dưới dạng JSON
                return Json(leadId, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                // Xử lý lỗi nếu có
                return Json(new { error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region GetZaloOaId
        [HttpGet]
        [AllowAnonymous]
        public ActionResult GetZaloOaId()
        {
            try
            {
                // Sử dụng SqlHelper để gọi stored procedure và lấy ZaloOAId
                var oaId = SqlHelper.QuerySP<string>("sp_Crm_GetZaloOaIdNew").FirstOrDefault();

                // Trả về ZaloOAId dưới dạng JSON
                return Json(oaId, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                // Xử lý lỗi nếu có
                return Json(new { error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion
        #region ConnectZaloOA
        // Lưu thông tin khi kết nối Zalo OA
        public void ConnectZaloOA(string zaloOAId, int userId)
        {

            var data = SqlHelper.QuerySP<ZaloOAViewModel>("sp_Crm_ZaloOA_Connect", new
            {
                pZaloOAId = zaloOAId,
                pNote = "",
                pCreatedUserId = userId,
                pAssignedUserId = userId
            }).ToList();
        }

        #endregion

        #region DisconnectZaloOA
        // Lưu thông tin khi ngắt kết nối Zalo OA
        public void DisconnectZaloOA(string zaloOAId, int userId)
        {
            var data = SqlHelper.QuerySP<ZaloOAViewModel>("sp_Crm_ZaloOA_Disconnect", new
            {
                pZaloOAId = zaloOAId,
                pModifiedUserId = userId,
            }).ToList();


        }



        #endregion

        #endregion
    }
}