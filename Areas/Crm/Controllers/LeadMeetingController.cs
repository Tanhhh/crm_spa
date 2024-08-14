using Erp.BackOffice.Crm.Models;
using Erp.BackOffice.Filters;
using Erp.BackOffice.Hubs;
using Erp.BackOffice.Models;
using Erp.BackOffice.Sale.Models;
using Erp.BackOffice.Staff.Models;
using Erp.Domain.Crm.Helper;
using Erp.Domain.Crm.Interfaces;
using Erp.Domain.Entities;
using Erp.Domain.Interfaces;
using Hangfire;
using Microsoft.AspNet.SignalR;
using Newtonsoft.Json;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using WebMatrix.WebData;

namespace Erp.BackOffice.Crm.Controllers
{
    [System.Web.Mvc.Authorize]
    [InitializeSimpleMembership]
    [Erp.BackOffice.Helpers.NoCacheHelper]
    public class LeadMeetingController : Controller
    {
        private readonly IUserRepository userRepository;

        private readonly ILeadMeetingRepository leadMeetingRepository;
        public LeadMeetingController(IUserRepository _user, ILeadMeetingRepository _leadMeeting)
        {
            userRepository = _user;
            leadMeetingRepository = _leadMeeting;

        }
        //DTO của lead
        public class Lead
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }

        public ViewResult Index(int leadId, int? actionId, int? LeadLogId, int? JobId, bool? isEdit)
        {
            // Đoạn code hiện tại của bạn
            IQueryable<User> userQuery = userRepository.GetAllUsers();
            // Tiếp tục xử lý với danh sách người dùng
            var usersProfileImage = userQuery
                                            .AsEnumerable()
                                            .Where(user => user.UserName != null)
                                            .Select(user => new User
                                            {
                                                Id = user.Id,
                                                UserName = user.FullName,
                                                ProfileImage = user.ProfileImage
                                            })
                                            .ToList();
            ViewBag.UsersList = usersProfileImage;
            ViewBag.LeadId = leadId;
            ViewBag.Numberofminutes_Warning = Erp.BackOffice.Helpers.Common.GetSetting("sophutbaotruocmeeting");
            ViewBag.CurrentUserId = WebSecurity.CurrentUserId;


            if (leadId > 0)
            {
                var leadInfo = SqlHelper.QuerySP<dynamic>("sp_Crm_LeadByID_Get", new
                {
                    pLeadId = leadId
                }).FirstOrDefault();
                var appear = SqlHelper.QuerySP<dynamic>("chkActionLeadLogs", new
                {
                    @action = "MEETING",
                    @leadid = leadId
                }).FirstOrDefault();
                if (leadInfo != null)
                {
                    ViewBag.NameLead = leadInfo.Name;
                    ViewBag.Location = leadInfo.F12;
                }
                if (appear != null && actionId == null)
                {
                    ViewBag.iskpi = appear.chk;
                }
            }
            if (actionId != null)
            {
                //sp_Crm_LeadMeating_ByID_Get new {@pIdAction = actionId}
                var model = SqlHelper.QuerySP<LeadMeetingViewModel>("sp_Crm_LeadMeating_ByID_Get", new
                {
                    @pIdAction = actionId
                }).FirstOrDefault();
                string dateAction = model.DateAction;

                string[] parts = dateAction.Split(' ');
                string datePart = parts[0];
                string[] dateParts = datePart.Split('/');

                string formattedDate = String.Format("{0}-{1}-{2}", dateParts[2], dateParts[1], dateParts[0]);
                model.DateAction = formattedDate;
                ViewBag.iskpi = 1;
                if (LeadLogId != null)
                {
                    ViewBag.LeadLogId = LeadLogId;
                    ViewBag.JobId = JobId;
                    ViewBag.IsEdit = isEdit;
                }
                return View(model);
            }
            ViewBag.IsEdit = null;
            ViewBag.LeadLogId = null;
            ViewBag.JobId = null;
            return View();
        }




        [HttpPost]
        public ActionResult LeadMeeting([System.Web.Http.FromBody] LeadMeetingViewModel model)
        {
            int status = model.Status == true ? 1 : 0;

            int important = model.Important == true ? 1 : 0;
            if (!string.IsNullOrEmpty(model.selectedId))
            {
                string[] strings = model.selectedId.Split(';');
                if (strings.Length > 0 && strings[0] != "" && model != null)
                {
                    foreach(var item in strings)
                    {
                        var idLeadMeeting = SqlHelper.QuerySP<int>("sp_Crm_LeadMeeting_insert", new
                        {
                            pLeadId =int.Parse( item),
                            pDateAction = model.DateAction,
                            pTimeAction = model.TimeAction,
                            pTimeExcute = model.TimeExcute,
                            pStatus = status,
                            pTitle = model.Title,
                            pIsKpi = model.isKpi,
                            pLocationEvent = model.LocationEvent,
                            pNote = model.Note,
                            pUserId = model.UserId,
                            pNumberofminutes_Warning = model.Numberofminutes_Warning,
                            pCreatedUserId = WebSecurity.CurrentUserId,
                            pTypeLead = (model.isPartial == 1 || model.isPartial == -1) ? 0 : 1

                        }).FirstOrDefault();
                        if (idLeadMeeting > 0)
                        {
                            var idLeadMeeting_LeadLogs = SqlHelper.QuerySP<int>("sp_Crm_LeadMeeting_LeadLogs_insert", new
                            {
                                pLeadId = int.Parse(item),
                                pAction = "MEETING",
                                pLogs = model.Title,
                                pisImportant = important,
                                pIdAction = idLeadMeeting,
                                pCreatedUserId = WebSecurity.CurrentUserId,
                                pTypeLead = (model.isPartial == 1 || model.isPartial == -1) ? 0 : 1,
                                pStatus = status

                            }).FirstOrDefault();
                            var uptBranchId = SqlHelper.ExecuteSP("UpdateBranchIdLead", new { @pLeadId = model.LeadId, @pBranchId = model.LocationEvent, @pUserId = WebSecurity.CurrentUserId });
                            var profileImage = SqlHelper.QuerySP<string>("sp_Crm_ImgOfLeadUser_Get", new
                            {
                                pLeadId = int.Parse(item),

                            }).FirstOrDefault();
                            // gọi signalR ErpHub
                            var context = GlobalHost.ConnectionManager.GetHubContext<ErpHub>();
                            // kiểm tra hàm sp_Crm_LeadMeeting_insert đúng chưa
                            if (idLeadMeeting_LeadLogs > 0)
                            {
                                // đặt lịch thông báo cuộc họp
                                DateTime dateAction = DateTime.Parse(model.DateAction);
                                TimeSpan timeAction = TimeSpan.Parse(model.TimeAction);
                                if (model.Numberofminutes_Warning != null)
                                {
                                    int warningMinutes = int.Parse(model.Numberofminutes_Warning.ToString());
                                    double totalMinutes = timeAction.TotalMinutes - warningMinutes;
                                    timeAction = TimeSpan.FromMinutes(totalMinutes);
                                }

                                // Tính toán thời gian kích hoạt công việc lập lịch
                                DateTime notificationTime = dateAction.Date + timeAction;


                                var message = "Cuộc họp: " + $"{model.Title} - {model.TimeAction} ";
                                // tạo lời nhắc cuộc họp
                                string idJob = null;
                                if (status == 1) { }
                                    //return Json(new { error = false, message = "Cuộc họp đã được tạo thành công!" });
                                else
                                {
                                    var customerId = SqlHelper.QuerySP<int?>("sp_Crm_CustomerIdByLeadId_Get", new
                                    {
                                        pLeadId = model.LeadId,

                                    }).FirstOrDefault();
                                    if (customerId.HasValue && customerId > 0)
                                        idJob = ScheduleMeetingNotification(notificationTime, model.Title, idLeadMeeting, message, model.UserId, WebSecurity.CurrentUserId, int.Parse(item), profileImage, model.isPartial, customerId);
                                    else
                                        idJob = ScheduleMeetingNotification(notificationTime, model.Title, idLeadMeeting, message, model.UserId, WebSecurity.CurrentUserId, int.Parse(item), profileImage, model.isPartial, -1);
                                }
                                if (idJob != null)
                                {
                                    // thêm idJob vào LeadLods
                                    var rsEditIdJob = SqlHelper.QuerySP<int>("spCrm_LeadLog_Edit_IdJob", new
                                    {
                                        @pId = idLeadMeeting_LeadLogs,
                                        @pIdJob = int.Parse(idJob),
                                        @pModifiedUserId = WebSecurity.CurrentUserId

                                    }).FirstOrDefault();
                                    if (rsEditIdJob > 0)
                                    {
                                        if (model.IsEdit == true)
                                        {
                                            SqlHelper.ExecuteSP("InsertLeadLogs", new
                                            {
                                                @LeadId = int.Parse(item),
                                                @Action = "OTHER",
                                                @Logs = "Chỉnh sửa kế hoạch đã lên",
                                                @isImportant = 0,
                                                @IdAction = -1,
                                                @UserId = WebSecurity.CurrentUserId,
                                                @TypeLead = (model.isPartial == 1 || model.isPartial == -1) ? 0 : 1
                                            });
                                        }
                                    }
                                    else
                                    {
                                    }
                                }
                                else
                                {
                                }

                            }
                            else
                            {
                            }
                        }
                        else
                        {

                        }
                    }
                    return Json(new { error = false, message = "Cuộc họp đã được tạo thành công!" });
                }
                else
                {
                    return Json(new { error = true, message = "Nhập thông tin bị lỗi! Vui lòng thử lại." });

                }
            }
            else if (model != null)
            {
                var idLeadMeeting = SqlHelper.QuerySP<int>("sp_Crm_LeadMeeting_insert", new
                {
                    pLeadId = model.LeadId,
                    pDateAction = model.DateAction,
                    pTimeAction = model.TimeAction,
                    pTimeExcute = model.TimeExcute,
                    pStatus = status,
                    pTitle = model.Title,
                    pIsKpi = model.isKpi,
                    pLocationEvent = model.LocationEvent,
                    pNote = model.Note,
                    pUserId = model.UserId,
                    pNumberofminutes_Warning = model.Numberofminutes_Warning,
                    pCreatedUserId = WebSecurity.CurrentUserId,
                    pTypeLead = (model.isPartial == 1 || model.isPartial == -1) ? 0 : 1

                }).FirstOrDefault();
                if (idLeadMeeting > 0)
                {
                    var idLeadMeeting_LeadLogs = SqlHelper.QuerySP<int>("sp_Crm_LeadMeeting_LeadLogs_insert", new
                    {
                        pLeadId = model.LeadId,
                        pAction = "MEETING",
                        pLogs = model.Title,
                        pisImportant = important,
                        pIdAction = idLeadMeeting,
                        pCreatedUserId = WebSecurity.CurrentUserId,
                        pTypeLead = (model.isPartial == 1 || model.isPartial == -1) ? 0 : 1,
                        pStatus = status

                    }).FirstOrDefault();
                    var uptBranchId = SqlHelper.ExecuteSP("UpdateBranchIdLead", new { @pLeadId = model.LeadId, @pBranchId = model.LocationEvent, @pUserId = WebSecurity.CurrentUserId });
                    var profileImage = SqlHelper.QuerySP<string>("sp_Crm_ImgOfLeadUser_Get", new
                    {
                        pLeadId = model.LeadId,

                    }).FirstOrDefault();
                    // gọi signalR ErpHub
                    var context = GlobalHost.ConnectionManager.GetHubContext<ErpHub>();
                    // kiểm tra hàm sp_Crm_LeadMeeting_insert đúng chưa
                    if (idLeadMeeting_LeadLogs > 0)
                    {
                        // gửi thông báo đến hàm f5LeadLogs
                        context.Clients.All.LeadLogsMeeting(model.LeadId);
                        context.Clients.All.f5LeadLogs(model.LeadId);
                        // đặt lịch thông báo cuộc họp
                        DateTime dateAction = DateTime.Parse(model.DateAction);
                        TimeSpan timeAction = TimeSpan.Parse(model.TimeAction);
                        if (model.Numberofminutes_Warning != null)
                        {
                            int warningMinutes = int.Parse(model.Numberofminutes_Warning.ToString());
                            double totalMinutes = timeAction.TotalMinutes - warningMinutes;
                            timeAction = TimeSpan.FromMinutes(totalMinutes);
                        }

                        // Tính toán thời gian kích hoạt công việc lập lịch
                        DateTime notificationTime = dateAction.Date + timeAction;


                        var message = "Cuộc họp: " + $"{model.Title} - {model.TimeAction} ";
                        // tạo lời nhắc cuộc họp
                        string idJob = null;
                        if (status == 1)
                            return Json(new { error = false, message = "Cuộc họp đã được tạo thành công!" });
                        else
                        {
                            var customerId = SqlHelper.QuerySP<int?>("sp_Crm_CustomerIdByLeadId_Get", new
                            {
                                pLeadId = model.LeadId,

                            }).FirstOrDefault();
                            if (customerId.HasValue && customerId > 0)
                                idJob = ScheduleMeetingNotification(notificationTime, model.Title, idLeadMeeting, message, model.UserId, WebSecurity.CurrentUserId, model.LeadId, profileImage, model.isPartial, customerId);
                            else
                                idJob = ScheduleMeetingNotification(notificationTime, model.Title, idLeadMeeting, message, model.UserId, WebSecurity.CurrentUserId, model.LeadId, profileImage, model.isPartial, -1);
                        }
                        if (idJob != null)
                        {
                            // thêm idJob vào LeadLods
                            var rsEditIdJob = SqlHelper.QuerySP<int>("spCrm_LeadLog_Edit_IdJob", new
                            {
                                @pId = idLeadMeeting_LeadLogs,
                                @pIdJob = int.Parse(idJob),
                                @pModifiedUserId = WebSecurity.CurrentUserId

                            }).FirstOrDefault();
                            if (rsEditIdJob > 0)
                            {
                                if (model.IsEdit == true)
                                {
                                    SqlHelper.ExecuteSP("InsertLeadLogs", new
                                    {
                                        @LeadId = model.LeadId,
                                        @Action = "OTHER",
                                        @Logs = "Chỉnh sửa kế hoạch đã lên",
                                        @isImportant = 0,
                                        @IdAction = -1,
                                        @UserId = WebSecurity.CurrentUserId,
                                        @TypeLead = (model.isPartial == 1 || model.isPartial == -1) ? 0 : 1
                                    });
                                }
                                return Json(new { error = false, message = "Cuộc họp đã được tạo thành công!" });
                            }
                            else
                            {
                                return Json(new { error = true, message = "Lỗi tạo cuộc họp, vui lòng thử lại!" });
                            }
                        }
                        else
                        {
                            return Json(new { error = true, message = "Lỗi tạo cuộc họp, vui lòng thử lại!" });
                        }


                    }
                    else
                    {
                        return Json(new { error = true, message = "Lỗi tạo cuộc họp, vui lòng thử lại!" });
                    }
                }
                else
                {
                    return Json(new { error = true, message = "Cuộc họp bị trùng! Vui lòng thử lại." });

                }
            }
            else
            {
                return Json(new { error = true, message = "Nhập thông tin bị lỗi! Vui lòng thử lại." });

            }

        }

        // hàm tạo lời nhắc Hangfire
        public static string ScheduleMeetingNotification(DateTime notificationTime, string title, int idLeadMeeting, string message, int userId, int CreatedUserId, int LeadId, string profileImage, int? isPartial, int? customerId)
        {
            try
            {
                return BackgroundJob.Schedule(() => SendMeetingNotification(message, userId, title, idLeadMeeting, notificationTime, CreatedUserId, LeadId, profileImage, isPartial, customerId), notificationTime);

            }
            catch (Exception ex)
            {
                return null;
            }
        }


        public static void SendMeetingNotification(string message, int userId, string title, int idLeadMeeting, DateTime notificationTime, int CreatedUserId, int LeadId, string profileImage, int? isPartial, int? customerId)
        {
            // Gửi thông báo sử dụng SignalR
            string clientIdUser;
            string clientIdUserCreate;
            var hubContext = GlobalHost.ConnectionManager.GetHubContext<MeetingHub>();

            if (userId == CreatedUserId)
            {
                var result = SqlHelper.QuerySP<string>("sp_Crm_ClientIdByUserId_GET", new
                {
                    pUserId = CreatedUserId
                }).FirstOrDefault();

                clientIdUser = result;
                var idTaskNew = SqlHelper.QuerySP<int>("sp_Crm_TaskNew_insert", new
                {
                    @pCreatedDate = notificationTime,
                    @pCreatedUserId = CreatedUserId,
                    @pSubject = title,
                    @pStatus = "success",
                    @pParentId = idLeadMeeting,
                    @pPriority = "high",
                    @pDescription = title,
                    @pType = "notifications",
                    @pAssignUserId = userId
                }).FirstOrDefault();
                hubContext.Clients.All.upadteListNotificationOfLeadTaskCall();
                hubContext.Clients.Client(clientIdUser).SendMeetingNotificationToUser(message, LeadId, profileImage, isPartial, idTaskNew, customerId);

            }
            else
            {

                var resultUser = SqlHelper.QuerySP<string>("sp_Crm_ClientIdByUserId_GET", new
                {
                    pUserId = userId
                }).FirstOrDefault();
                // lấy clientId User
                clientIdUser = resultUser;


                var resultUserCreate = SqlHelper.QuerySP<string>("sp_Crm_ClientIdByUserId_GET", new
                {
                    pUserId = CreatedUserId
                }).FirstOrDefault();
                // clientID của createUser
                clientIdUserCreate = resultUserCreate;
                if (clientIdUserCreate != null && clientIdUser != null)
                {

                    List<string> listclientId = new List<string>();
                    listclientId.Add(clientIdUser);
                    listclientId.Add(clientIdUserCreate);
                    var idTaskNew = SqlHelper.QuerySP<int>("sp_Crm_TaskNew_insert", new
                    {
                        @pCreatedDate = notificationTime,
                        @pCreatedUserId = CreatedUserId,
                        @pSubject = title,
                        @pStatus = "success",
                        @pParentId = idLeadMeeting,
                        @pPriority = "high",
                        @pDescription = title,
                        @pType = "notifications",
                        @pAssignUserId = userId
                    }).FirstOrDefault();
                    hubContext.Clients.All.upadteListNotificationOfLeadTaskCall();
                    hubContext.Clients.Clients(listclientId).SendMeetingNotificationToUser(message, LeadId, profileImage, isPartial, idTaskNew, customerId);
                }

            }

            //         if (clientIdUser != null)
            //{
            //	//hubContext.Clients.All.SendMeetingNotification(message);
            //	var idTaskNew = SqlHelper.QuerySP<int>("sp_Crm_TaskNew_insert", new
            //	{
            //		@pCreatedDate = notificationTime,
            //		@pCreatedUserId = CreatedUserId,
            //		@pSubject = title,
            //		@pStatus = "success",
            //		@pParentId = idLeadMeeting,
            //		@pPriority = "high",
            //		@pDescription = title,
            //		@pType = "notifications",
            //                 @pAssignUserId = userId
            //             }).FirstOrDefault();
            //	hubContext.Clients.All.upadteListNotificationOfLeadTaskCall();
            //         }
        }

        [HttpPost]
        public ActionResult SaveClientId(string clientId)
        {

            var rowsAffected = SqlHelper.QuerySP<int>("sp_Crm_ClientID_User_Update", new
            {
                pUserId = WebSecurity.CurrentUserId,
                pClientId = clientId
            }).FirstOrDefault();
            if (rowsAffected > 0)
            {
                return Json(new { success = true, message = "Update thành công" });
            }

            return Json(new { success = false, message = "Update thất bại" });

        }
        [HttpGet]
        public ActionResult UpdateStatusLeadLog(int isDeleted, int LeadLogId, int JobId)
        {
            try
            {
                BackgroundJob.Delete(JobId.ToString());
                var context = GlobalHost.ConnectionManager.GetHubContext<ErpHub>();
                var leadId = SqlHelper.QuerySP<int>("sp_Crm_UpdateIsDeletedLeadLogById_GET", new
                {
                    pLeadLogId = LeadLogId,
                    pStatus = isDeleted
                }).FirstOrDefault();
                if (leadId > 0)
                {
                    context.Clients.All.LeadLogsMeeting(leadId);
                    context.Clients.All.f5LeadLogs(leadId);
                    return Json(new { success = true, message = "Update thành công" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { success = false, message = "Update thất bại" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Update thất bại" }, JsonRequestBehavior.AllowGet);
            }



        }
        [HttpGet]
        public ActionResult DeleteJobHF(int LeadLogId, int JobId)
        {
            try
            {
                BackgroundJob.Delete(JobId.ToString());
                // Công việc đã được xóa thành công
                var context = GlobalHost.ConnectionManager.GetHubContext<ErpHub>();
                var leadId = SqlHelper.QuerySP<int>("spCrm_LeadLogs_Delete", new
                {
                    @pId = LeadLogId,
                    @pModifiedUserId = WebSecurity.CurrentUserId
                }).FirstOrDefault();
                if (leadId > 0)
                {
                    context.Clients.All.LeadLogsMeeting(leadId);
                    context.Clients.All.f5LeadLogs(leadId);
                    SqlHelper.ExecuteSP("InsertLeadLogs", new
                    {
                        @LeadId = leadId,
                        @Action = "OTHER",
                        @Logs = "Xóa việc đã lên kế hoạch",
                        @isImportant = 0,
                        @IdAction = -1,
                        @UserId = WebSecurity.CurrentUserId,
                        @TypeLead = 1
                    });
                    return Json(new { success = true, message = "Update thành công" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { success = false, message = "Update thất bại" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Update thất bại" }, JsonRequestBehavior.AllowGet);
            }
        }
        public ViewResult CallToView(int leadId, int? actionId, int? LeadLogId, int? JobId, bool? isEdit)
        {
            // Đoạn code hiện tại của bạn
            IQueryable<User> userQuery = userRepository.GetAllUsers();
            // Tiếp tục xử lý với danh sách người dùng
            var usersProfileImage = userQuery
                                            .AsEnumerable()
                                            .Where(user => user.UserName != null)
                                            .Select(user => new User
                                            {
                                                Id = user.Id,
                                                UserName = user.FullName,
                                                ProfileImage = user.ProfileImage
                                            })
                                            .ToList();
            ViewBag.UsersList = usersProfileImage;
            ViewBag.LeadId = leadId;
            ViewBag.Numberofminutes_Warning = Erp.BackOffice.Helpers.Common.GetSetting("sophutbaotruoccall");
            ViewBag.CurrentUserId = WebSecurity.CurrentUserId;


            if (leadId > 0)
            {
                var leadInfo = SqlHelper.QuerySP<dynamic>("sp_Crm_LeadByID_Get", new
                {
                    pLeadId = leadId
                }).FirstOrDefault();

                if (leadInfo != null)
                {
                    ViewBag.NameLead = leadInfo.Name;
                }
            }
            if (actionId != null)
            {
                //sp_Crm_LeadMeating_ByID_Get new {@pIdAction = actionId}
                var model = SqlHelper.QuerySP<LeadMeetingViewModel>("sp_Crm_LeadMeating_ByID_Get", new
                {
                    @pIdAction = actionId
                }).FirstOrDefault();
                string dateAction = model.DateAction;

                string[] parts = dateAction.Split(' ');
                string datePart = parts[0];
                string[] dateParts = datePart.Split('/');

                string formattedDate = String.Format("{0}-{1}-{2}", dateParts[2], dateParts[1], dateParts[0]);
                model.DateAction = formattedDate;
                if (LeadLogId != null)
                {
                    ViewBag.LeadLogId = LeadLogId;
                    ViewBag.JobId = JobId;
                    ViewBag.IsEdit = isEdit;
                }
                return View(model);
            }
            ViewBag.IsEdit = null;
            ViewBag.LeadLogId = null;
            ViewBag.JobId = null;
            return View();
        }
        [HttpPost]
        public ActionResult LeadCall([System.Web.Http.FromBody] LeadMeetingViewModel model)
        {
            int status = model.Status == true ? 1 : 0;

            int important = model.Important == true ? 1 : 0;
            if (!string.IsNullOrEmpty(model.selectedId))
            {
                string[] strings = model.selectedId.Split(';');
                if (strings.Length > 0 && strings[0] != "" && model != null)
                {
                    foreach (var item in strings)
                    {
                        var idLeadMeeting = SqlHelper.QuerySP<int>("sp_Crm_LeadCall_insert", new
                        {
                            pLeadId = int.Parse(item),
                            pDateAction = model.DateAction,
                            pTimeAction = model.TimeAction,
                            pTimeExcute = model.TimeExcute,
                            pStatus = status,
                            pTitle = model.Title,
                            pNote = model.Note,
                            pUserId = model.UserId,
                            pNumberofminutes_Warning = model.Numberofminutes_Warning,
                            pCreatedUserId = WebSecurity.CurrentUserId,
                            pTypeLead = (model.isPartial == 1 || model.isPartial == -1) ? 0 : 1

                        }).FirstOrDefault();
                        if (idLeadMeeting > 0)
                        {
                            var idLeadMeeting_LeadLogs = SqlHelper.QuerySP<int>("sp_Crm_LeadMeeting_LeadLogs_insert", new
                            {
                                pLeadId = int.Parse(item),
                                pAction = "CALL",
                                pLogs = model.Title,
                                pisImportant = important,
                                pIdAction = idLeadMeeting,
                                pTypeLead = (model.isPartial == 1 || model.isPartial == -1) ? 0 : 1,
                                pCreatedUserId = WebSecurity.CurrentUserId,
                                pStatus = status

                            }).FirstOrDefault();
                            var profileImage = SqlHelper.QuerySP<string>("sp_Crm_ImgOfLeadUser_Get", new
                            {
                                pLeadId = int.Parse(item),

                            }).FirstOrDefault();
                            // gọi signalR ErpHub
                            var context = GlobalHost.ConnectionManager.GetHubContext<ErpHub>();
                            // kiểm tra hàm sp_Crm_LeadMeeting_insert đúng chưa
                            if (idLeadMeeting_LeadLogs > 0)
                            {

                                // đặt lịch thông báo cuộc họp
                                DateTime dateAction = DateTime.Parse(model.DateAction);
                                TimeSpan timeAction = TimeSpan.Parse(model.TimeAction);
                                if (model.Numberofminutes_Warning != null)
                                {
                                    int warningMinutes = int.Parse(model.Numberofminutes_Warning.ToString());
                                    double totalMinutes = timeAction.TotalMinutes - warningMinutes;
                                    timeAction = TimeSpan.FromMinutes(totalMinutes);
                                }

                                // Tính toán thời gian kích hoạt công việc lập lịch
                                DateTime notificationTime = dateAction.Date + timeAction;


                                var message = "Cuộc gọi đi: " + $"{model.Title} - {model.TimeAction} ";
                                // tạo lời nhắc cuộc họp
                                string idJob = null;
                                if (status == 1)
                                    return Json(new { error = false, message = "Cuộc họp đã được tạo thành công!" });
                                else
                                {
                                    var customerId = SqlHelper.QuerySP<int?>("sp_Crm_CustomerIdByLeadId_Get", new
                                    {
                                        pLeadId = int.Parse(item),

                                    }).FirstOrDefault();
                                    if (customerId.HasValue && customerId > 0)
                                        idJob = ScheduleMeetingNotification(notificationTime, model.Title, idLeadMeeting, message, model.UserId, WebSecurity.CurrentUserId, int.Parse(item), profileImage, model.isPartial, customerId);
                                    else
                                        idJob = ScheduleMeetingNotification(notificationTime, model.Title, idLeadMeeting, message, model.UserId, WebSecurity.CurrentUserId, int.Parse(item), profileImage, model.isPartial, -1);
                                }
                                if (idJob != null)
                                {
                                    // thêm idJob vào LeadLods
                                    var rsEditIdJob = SqlHelper.QuerySP<int>("spCrm_LeadLog_Edit_IdJob", new
                                    {
                                        @pId = idLeadMeeting_LeadLogs,
                                        @pIdJob = int.Parse(idJob),
                                        @pModifiedUserId = WebSecurity.CurrentUserId

                                    }).FirstOrDefault();
                                    if (rsEditIdJob > 0)
                                    {
                                        if (model.IsEdit == true)
                                        {
                                            SqlHelper.ExecuteSP("InsertLeadLogs", new
                                            {
                                                @LeadId =int.Parse(item),
                                                @Action = "OTHER",
                                                @Logs = "Chỉnh sửa kế hoạch đã lên",
                                                @isImportant = 0,
                                                @IdAction = -1,
                                                @UserId = WebSecurity.CurrentUserId,
                                                @TypeLead = (model.isPartial == 1 || model.isPartial == -1) ? 0 : 1
                                            });
                                        }
                                    }
                                    else
                                    {
                                    }
                                }
                                else
                                {
                                }
                            }
                            else
                            {

                            }
                        }
                        else
                        {

                        }
                    }
                    return Json(new { error = false, message = "Cuộc họp đã được tạo thành công!" });
                }
                else
                {
                    return Json(new { error = true, message = "Nhập thông tin bị lỗi! Vui lòng thử lại." });

                }
            }
            else if (model != null)
            {
                var idLeadMeeting = SqlHelper.QuerySP<int>("sp_Crm_LeadCall_insert", new
                {
                    pLeadId = model.LeadId,
                    pDateAction = model.DateAction,
                    pTimeAction = model.TimeAction,
                    pTimeExcute = model.TimeExcute,
                    pStatus = status,
                    pTitle = model.Title,
                    pNote = model.Note,
                    pUserId = model.UserId,
                    pNumberofminutes_Warning = model.Numberofminutes_Warning,
                    pCreatedUserId = WebSecurity.CurrentUserId,
                    pTypeLead = (model.isPartial == 1 || model.isPartial == -1) ? 0 : 1

                }).FirstOrDefault();
                if (idLeadMeeting > 0)
                {
                    var idLeadMeeting_LeadLogs = SqlHelper.QuerySP<int>("sp_Crm_LeadMeeting_LeadLogs_insert", new
                    {
                        pLeadId = model.LeadId,
                        pAction = "CALL",
                        pLogs = model.Title,
                        pisImportant = important,
                        pIdAction = idLeadMeeting,
                        pTypeLead = (model.isPartial == 1 || model.isPartial == -1) ? 0 : 1,
                        pCreatedUserId = WebSecurity.CurrentUserId,
                        pStatus = status

                    }).FirstOrDefault();
                    var profileImage = SqlHelper.QuerySP<string>("sp_Crm_ImgOfLeadUser_Get", new
                    {
                        pLeadId = model.LeadId,

                    }).FirstOrDefault();
                    // gọi signalR ErpHub
                    var context = GlobalHost.ConnectionManager.GetHubContext<ErpHub>();
                    // kiểm tra hàm sp_Crm_LeadMeeting_insert đúng chưa
                    if (idLeadMeeting_LeadLogs > 0)
                    {
                        // gửi thông báo đến hàm f5LeadLogs
                        context.Clients.All.LeadLogsMeeting(model.LeadId);
                        context.Clients.All.f5LeadLogs(model.LeadId);

                        // đặt lịch thông báo cuộc họp
                        DateTime dateAction = DateTime.Parse(model.DateAction);
                        TimeSpan timeAction = TimeSpan.Parse(model.TimeAction);
                        if (model.Numberofminutes_Warning != null)
                        {
                            int warningMinutes = int.Parse(model.Numberofminutes_Warning.ToString());
                            double totalMinutes = timeAction.TotalMinutes - warningMinutes;
                            timeAction = TimeSpan.FromMinutes(totalMinutes);
                        }

                        // Tính toán thời gian kích hoạt công việc lập lịch
                        DateTime notificationTime = dateAction.Date + timeAction;


                        var message = "Cuộc gọi đi: " + $"{model.Title} - {model.TimeAction} ";
                        // tạo lời nhắc cuộc họp
                        string idJob = null;
                        if (status == 1)
                            return Json(new { error = false, message = "Cuộc họp đã được tạo thành công!" });
                        else
                        {
                            var customerId = SqlHelper.QuerySP<int?>("sp_Crm_CustomerIdByLeadId_Get", new
                            {
                                pLeadId = model.LeadId,

                            }).FirstOrDefault();
                            if (customerId.HasValue && customerId > 0)
                                idJob = ScheduleMeetingNotification(notificationTime, model.Title, idLeadMeeting, message, model.UserId, WebSecurity.CurrentUserId, model.LeadId, profileImage, model.isPartial, customerId);
                            else
                                idJob = ScheduleMeetingNotification(notificationTime, model.Title, idLeadMeeting, message, model.UserId, WebSecurity.CurrentUserId, model.LeadId, profileImage, model.isPartial, -1);
                        }
                        if (idJob != null)
                        {
                            // thêm idJob vào LeadLods
                            var rsEditIdJob = SqlHelper.QuerySP<int>("spCrm_LeadLog_Edit_IdJob", new
                            {
                                @pId = idLeadMeeting_LeadLogs,
                                @pIdJob = int.Parse(idJob),
                                @pModifiedUserId = WebSecurity.CurrentUserId

                            }).FirstOrDefault();
                            if (rsEditIdJob > 0)
                            {
                                if (model.IsEdit == true)
                                {
                                    SqlHelper.ExecuteSP("InsertLeadLogs", new
                                    {
                                        @LeadId = model.LeadId,
                                        @Action = "OTHER",
                                        @Logs = "Chỉnh sửa kế hoạch đã lên",
                                        @isImportant = 0,
                                        @IdAction = -1,
                                        @UserId = WebSecurity.CurrentUserId,
                                        @TypeLead = (model.isPartial == 1 || model.isPartial == -1) ? 0 : 1
                                    });
                                }
                                return Json(new { error = false, message = "cuộc gọi đã được tạo thành công!" });
                            }
                            else
                            {
                                return Json(new { error = true, message = "Lỗi tạo cuộc gọi đi, vui lòng thử lại!" });
                            }
                        }
                        else
                        {
                            return Json(new { error = true, message = "Lỗi tạo cuộc gọi đi, vui lòng thử lại!" });
                        }
                    }
                    else
                    {
                        return Json(new { error = true, message = "Lỗi tạo cuộc gọi đi, vui lòng thử lại!" });

                    }
                }
                else
                {
                    return Json(new { error = true, message = "Cuộc gọi đi bị trùng! Vui lòng thử lại." });

                }
            }
            else
            {
                return Json(new { error = true, message = "Lỗi nhập thông tin, vui lòng thử lại!" });

            }


        }

        public ViewResult TaskView(int leadId, int? actionId, int? LeadLogId, int? JobId, bool? isEdit)
        {
            // Đoạn code hiện tại của bạn
            IQueryable<User> userQuery = userRepository.GetAllUsers();
            // Tiếp tục xử lý với danh sách người dùng
            var usersProfileImage = userQuery
                                            .AsEnumerable()
                                            .Where(user => user.UserName != null)
                                            .Select(user => new User
                                            {
                                                Id = user.Id,
                                                UserName = user.FullName,
                                                ProfileImage = user.ProfileImage
                                            })
                                            .ToList();
            ViewBag.UsersList = usersProfileImage;
            ViewBag.LeadId = leadId;
            ViewBag.Numberofminutes_Warning = Erp.BackOffice.Helpers.Common.GetSetting("sophutbaotruoctask");
            ViewBag.CurrentUserId = WebSecurity.CurrentUserId;


            if (leadId > 0)
            {
                var leadInfo = SqlHelper.QuerySP<dynamic>("sp_Crm_LeadByID_Get", new
                {
                    pLeadId = leadId
                }).FirstOrDefault();

                if (leadInfo != null)
                {
                    ViewBag.NameLead = leadInfo.Name;
                }
            }
            if (actionId != null)
            {
                //sp_Crm_LeadMeating_ByID_Get new {@pIdAction = actionId}
                var model = SqlHelper.QuerySP<LeadMeetingViewModel>("sp_Crm_LeadMeating_ByID_Get", new
                {
                    @pIdAction = actionId
                }).FirstOrDefault();
                string dateAction = model.DateAction;

                string[] parts = dateAction.Split(' ');
                string datePart = parts[0];
                string[] dateParts = datePart.Split('/');

                string formattedDate = String.Format("{0}-{1}-{2}", dateParts[2], dateParts[1], dateParts[0]);
                model.DateAction = formattedDate;
                if (LeadLogId != null)
                {
                    ViewBag.LeadLogId = LeadLogId;
                    ViewBag.JobId = JobId;
                    ViewBag.IsEdit = isEdit;
                }
                return View(model);
            }
            ViewBag.IsEdit = null;
            ViewBag.LeadLogId = null;
            ViewBag.JobId = null;
            return View();
        }
        [HttpPost]
        public ActionResult LeadTask([System.Web.Http.FromBody] LeadMeetingViewModel model)
        {
            int status = model.Status == true ? 1 : 0;

            int important = model.Important == true ? 1 : 0;
            if (!string.IsNullOrEmpty(model.selectedId))
            {
                string[] strings = model.selectedId.Split(';');
                if (strings.Length > 0 && strings[0] != "" && model != null)
                {
                    foreach (var item in strings)
                    {
                        var idLeadMeeting = SqlHelper.QuerySP<int>("sp_Crm_LeadTask_insert", new
                        {
                            pLeadId = int.Parse(item),
                            pDateAction = model.DateAction,
                            pTimeAction = model.TimeAction,
                            pStatus = status,
                            pTitle = model.Title,
                            pNote = model.Note,
                            pUserId = model.UserId,
                            pNumberofminutes_Warning = model.Numberofminutes_Warning,
                            pCreatedUserId = WebSecurity.CurrentUserId,
                            pTypeLead = (model.isPartial == 1 || model.isPartial == -1) ? 0 : 1

                        }).FirstOrDefault();
                        if (idLeadMeeting > 0)
                        {
                            var idLeadMeeting_LeadLogs = SqlHelper.QuerySP<int>("sp_Crm_LeadMeeting_LeadLogs_insert", new
                            {
                                pLeadId = int.Parse(item),
                                pAction = "TASK",
                                pLogs = model.Title,
                                pisImportant = important,
                                pIdAction = idLeadMeeting,
                                pTypeLead = (model.isPartial == 1 || model.isPartial == -1) ? 0 : 1,
                                pCreatedUserId = WebSecurity.CurrentUserId,
                                pStatus = status


                            }).FirstOrDefault();
                            var profileImage = SqlHelper.QuerySP<string>("sp_Crm_ImgOfLeadUser_Get", new
                            {
                                pLeadId = int.Parse(item),

                            }).FirstOrDefault();
                            // gọi signalR ErpHub
                            var context = GlobalHost.ConnectionManager.GetHubContext<ErpHub>();
                            // kiểm tra hàm sp_Crm_LeadMeeting_insert đúng chưa
                            if (idLeadMeeting_LeadLogs > 0)
                            {
                                // đặt lịch thông báo cuộc họp
                                DateTime dateAction = DateTime.Parse(model.DateAction);
                                TimeSpan timeAction = TimeSpan.Parse(model.TimeAction);
                                if (model.Numberofminutes_Warning != null)
                                {
                                    int warningMinutes = int.Parse(model.Numberofminutes_Warning.ToString());
                                    double totalMinutes = timeAction.TotalMinutes - warningMinutes;
                                    timeAction = TimeSpan.FromMinutes(totalMinutes);
                                }

                                // Tính toán thời gian kích hoạt công việc lập lịch
                                DateTime notificationTime = dateAction.Date + timeAction;


                                var message = "Nhiệm vụ mới: " + $"{model.Title} - {model.TimeAction} ";
                                string idJob = null;

                                var customerId = SqlHelper.QuerySP<int?>("sp_Crm_CustomerIdByLeadId_Get", new
                                {
                                    pLeadId = int.Parse(item),

                                }).FirstOrDefault();
                                if (customerId.HasValue && customerId > 0)
                                    idJob = ScheduleMeetingNotification(notificationTime, model.Title, idLeadMeeting, message, model.UserId, WebSecurity.CurrentUserId, int.Parse(item), profileImage, model.isPartial, customerId);
                                else
                                    idJob = ScheduleMeetingNotification(notificationTime, model.Title, idLeadMeeting, message, model.UserId, WebSecurity.CurrentUserId, int.Parse(item), profileImage, model.isPartial, -1);
                                if (idJob != null)
                                {
                                    // thêm idJob vào LeadLods
                                    var rsEditIdJob = SqlHelper.QuerySP<int>("spCrm_LeadLog_Edit_IdJob", new
                                    {
                                        @pId = idLeadMeeting_LeadLogs,
                                        @pIdJob = int.Parse(idJob),
                                        @pModifiedUserId = WebSecurity.CurrentUserId

                                    }).FirstOrDefault();
                                    if (rsEditIdJob > 0)
                                    {
                                        if (model.IsEdit == true)
                                        {
                                            SqlHelper.ExecuteSP("InsertLeadLogs", new
                                            {
                                                @LeadId = int.Parse(item),
                                                @Action = "OTHER",
                                                @Logs = "Chỉnh sửa kế hoạch đã lên",
                                                @isImportant = 0,
                                                @IdAction = -1,
                                                @UserId = WebSecurity.CurrentUserId,
                                                @TypeLead = (model.isPartial == 1 || model.isPartial == -1) ? 0 : 1
                                            });
                                        }
                                    }
                                    else
                                    {
                                    }
                                }
                                else
                                {
                                }
                            }
                            else
                            {
                            }
                        }
                        else
                        {
                        }
                    }
                    return Json(new { error = false, message = "Cuộc họp đã được tạo thành công!" });
                }
                else
                {
                    return Json(new { error = true, message = "Nhập thông tin bị lỗi! Vui lòng thử lại." });

                }
            }
            else if (model != null)
            {
                var idLeadMeeting = SqlHelper.QuerySP<int>("sp_Crm_LeadTask_insert", new
                {
                    pLeadId = model.LeadId,
                    pDateAction = model.DateAction,
                    pTimeAction = model.TimeAction,
                    pStatus = status,
                    pTitle = model.Title,
                    pNote = model.Note,
                    pUserId = model.UserId,
                    pNumberofminutes_Warning = model.Numberofminutes_Warning,
                    pCreatedUserId = WebSecurity.CurrentUserId,
                    pTypeLead = (model.isPartial == 1 || model.isPartial == -1) ? 0 : 1

                }).FirstOrDefault();
                if (idLeadMeeting > 0)
                {
                    var idLeadMeeting_LeadLogs = SqlHelper.QuerySP<int>("sp_Crm_LeadMeeting_LeadLogs_insert", new
                    {
                        pLeadId = model.LeadId,
                        pAction = "TASK",
                        pLogs = model.Title,
                        pisImportant = important,
                        pIdAction = idLeadMeeting,
                        pTypeLead = (model.isPartial == 1 || model.isPartial == -1) ? 0 : 1,
                        pCreatedUserId = WebSecurity.CurrentUserId,
                        pStatus = status


                    }).FirstOrDefault();
                    var profileImage = SqlHelper.QuerySP<string>("sp_Crm_ImgOfLeadUser_Get", new
                    {
                        pLeadId = model.LeadId,

                    }).FirstOrDefault();
                    // gọi signalR ErpHub
                    var context = GlobalHost.ConnectionManager.GetHubContext<ErpHub>();
                    // kiểm tra hàm sp_Crm_LeadMeeting_insert đúng chưa
                    if (idLeadMeeting_LeadLogs > 0)
                    {
                        // gửi thông báo đến hàm f5LeadLogs
                        context.Clients.All.LeadLogsMeeting(model.LeadId);
                        context.Clients.All.f5LeadLogs(model.LeadId);

                        // đặt lịch thông báo cuộc họp
                        DateTime dateAction = DateTime.Parse(model.DateAction);
                        TimeSpan timeAction = TimeSpan.Parse(model.TimeAction);
                        if (model.Numberofminutes_Warning != null)
                        {
                            int warningMinutes = int.Parse(model.Numberofminutes_Warning.ToString());
                            double totalMinutes = timeAction.TotalMinutes - warningMinutes;
                            timeAction = TimeSpan.FromMinutes(totalMinutes);
                        }

                        // Tính toán thời gian kích hoạt công việc lập lịch
                        DateTime notificationTime = dateAction.Date + timeAction;


                        var message = "Nhiệm vụ mới: " + $"{model.Title} - {model.TimeAction} ";
                        string idJob = null;

                        var customerId = SqlHelper.QuerySP<int?>("sp_Crm_CustomerIdByLeadId_Get", new
                        {
                            pLeadId = model.LeadId,

                        }).FirstOrDefault();
                        if (customerId.HasValue && customerId > 0)
                            idJob = ScheduleMeetingNotification(notificationTime, model.Title, idLeadMeeting, message, model.UserId, WebSecurity.CurrentUserId, model.LeadId, profileImage, model.isPartial, customerId);
                        else
                            idJob = ScheduleMeetingNotification(notificationTime, model.Title, idLeadMeeting, message, model.UserId, WebSecurity.CurrentUserId, model.LeadId, profileImage, model.isPartial, -1);
                        if (idJob != null)
                        {
                            // thêm idJob vào LeadLods
                            var rsEditIdJob = SqlHelper.QuerySP<int>("spCrm_LeadLog_Edit_IdJob", new
                            {
                                @pId = idLeadMeeting_LeadLogs,
                                @pIdJob = int.Parse(idJob),
                                @pModifiedUserId = WebSecurity.CurrentUserId

                            }).FirstOrDefault();
                            if (rsEditIdJob > 0)
                            {
                                if (model.IsEdit == true)
                                {
                                    SqlHelper.ExecuteSP("InsertLeadLogs", new
                                    {
                                        @LeadId = model.LeadId,
                                        @Action = "OTHER",
                                        @Logs = "Chỉnh sửa kế hoạch đã lên",
                                        @isImportant = 0,
                                        @IdAction = -1,
                                        @UserId = WebSecurity.CurrentUserId,
                                        @TypeLead = (model.isPartial == 1 || model.isPartial == -1) ? 0 : 1
                                    });
                                }
                                return Json(new { error = false, message = "Nhiệm v đã được tạo thành công!" });
                            }
                            else
                            {
                                return Json(new { error = true, message = "Lỗi tạo nhiệm vụ, vui lòng thử lại!" });
                            }
                        }
                        else
                        {
                            return Json(new { error = true, message = "Lỗi tạo nhiệm vụ, vui lòng thử lại!" });
                        }
                    }
                    else
                    {
                        return Json(new { error = true, message = "Lỗi tạo nhiệm vụ, vui lòng thử lại!" });
                    }
                }
                else
                {
                    return Json(new { error = true, message = "Nhiệm vụ tạo bị trùng! Vui lòng thử lại." });
                }
            }
            else
            {
                return Json(new { error = true, message = "Lỗi nhập thông tin, vui lòng thử lại!" });
            }
        }

        public ViewResult HistoryCallView(int leadId)
        {
            IEnumerable<HistoryCallLogViewModel> listHistoryCallLog = null;

            if (leadId > 0)
            {
                listHistoryCallLog = SqlHelper.QuerySP<HistoryCallLogViewModel>("sp_Crm_HistoryCall_Get", new
                {
                    pLeadId = leadId
                });
            }

            return View(listHistoryCallLog);
        }

        [HttpPost]
        public ActionResult UpdateIsSendNotifications(int idTaskNew)
        {

            if (idTaskNew > 0)
            {
                var idTaskNews = SqlHelper.QuerySP<int>("sp_Crm_IsSendNotifications_Update", new
                {
                    pIdTask = idTaskNew
                }).FirstOrDefault();
                if (idTaskNews > 0)
                {
                    var hubContext = Microsoft.AspNet.SignalR.GlobalHost.ConnectionManager.GetHubContext<MeetingHub>();

                    hubContext.Clients.All.upadteListNotificationOfLeadTaskCall();
                    return Json(new { success = true });
                }
            }
            return Json(new { success = false, error = "Không tìm thấy item" });
        }
        [HttpPost]
        public ActionResult UpdateIsSendNotificationsByIdLeadMeeting(int idLeadMeeting)
        {

            if (idLeadMeeting > 0)
            {
                var idTaskNews = SqlHelper.QuerySP<int>("sp_Crm_IsSendNotificationsByIdLeadMeeting_Update", new
                {
                    pIdLeadMeeting = idLeadMeeting
                }).FirstOrDefault();
                if (idTaskNews > 0)
                {
                    var hubContext = Microsoft.AspNet.SignalR.GlobalHost.ConnectionManager.GetHubContext<MeetingHub>();

                    hubContext.Clients.All.upadteListNotificationOfLeadTaskCall();
                    return Json(new { success = true });
                }
            }
            return Json(new { success = false, error = "Không tìm thấy item" });
        }
        public ActionResult AllNotifications()
        {
            IEnumerable<TaskMeetingViewModel> listTaskNews = null;
            listTaskNews = SqlHelper.QuerySP<TaskMeetingViewModel>("sp_Crm_AllTaskNew_Get", new
            {
                pCurrentUser = WebSecurity.CurrentUserId
            });
            return View(listTaskNews);
        }
        public ViewResult ParameterCRM()
        {
            IEnumerable<SystemParameter> listSystemParameter = null;

            listSystemParameter = SqlHelper.QuerySP<SystemParameter>("sp_Crm_SystemParameter_Get", new
            {
            });

            return View(listSystemParameter);
        }
        public ActionResult ExportToExcelParameter()
        {
            // Lấy dữ liệu từ database hoặc bất kỳ nguồn nào khác
            var data = GetDataFromDatabase();

            // Tạo một package Excel
            ExcelPackage excelPackage = new ExcelPackage();
            var worksheet = excelPackage.Workbook.Worksheets.Add("Data");

            // Đặt tiêu đề cho các cột
            worksheet.Cells[1, 1].Value = "STT";
            worksheet.Cells[1, 2].Value = "Name";
            worksheet.Cells[1, 3].Value = "Module";

            // Đặt dữ liệu vào bảng Excel
            int stt = 1; // Biến đếm số thứ tự
            foreach (var parameter in data)
            {
                worksheet.Cells[stt + 1, 1].Value = stt; // STT
                worksheet.Cells[stt + 1, 2].Value = parameter.Name;
                worksheet.Cells[stt + 1, 3].Value = parameter.Module;
                stt++; // Tăng biến đếm sau mỗi lần lặp
            }

            // Xuất Excel sang MemoryStream
            MemoryStream memoryStream = new MemoryStream();
            excelPackage.SaveAs(memoryStream);

            // Đặt vị trí của con trỏ tới đầu MemoryStream
            memoryStream.Seek(0, SeekOrigin.Begin);

            // Trả về file Excel như một file tải về
            return File(memoryStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "ListParameter.xlsx");
        }
        private List<SystemParameter> GetDataFromDatabase()
        {
            List<SystemParameter> listSystemParameter = null;
            listSystemParameter = SqlHelper.QuerySP<SystemParameter>("sp_Crm_SystemParameter_Get", new
            {
            }).ToList();
            return listSystemParameter;
        }

        #region Nghĩa - LeadSection
        public ActionResult LeadSection()
        {
            List<SystemParameter> listSystemParameter = null;
            listSystemParameter = SqlHelper.QuerySP<SystemParameter>("sp_Crm_SystemParameter_Get", new
            {
            }).ToList();
            return View();
        }
        [HttpGet]
        public ActionResult LeadSectionLeft(int typeLead)
        {
            var listLeadSection = SqlHelper.QuerySP<LeadSectionModel>("sp_Crm_LeadSection_Get", new
            {
                ptypeLead = typeLead
            }).ToList();
            return View(listLeadSection);
        }
        [HttpGet]
        public ActionResult LeadSectionRight(int idLeadSection)
        {
            var listLeadSectionField = SqlHelper.QuerySP<LeadSection_FieldModel>("sp_Crm_LeadSectionField_Get", new
            {
                pIdLeadSection = idLeadSection
            }).ToList();
            return View(listLeadSectionField);
        }
        [HttpGet]
        public ActionResult GetCustomerByLeadId(int leadId)
        {
            var customerId = SqlHelper.QuerySP<int?>("sp_Crm_CustomerIdByLeadId_Get", new
            {
                pLeadId = leadId,
            }).FirstOrDefault();

            if (customerId.HasValue && customerId > 0)
            {
                // Trả về customerId nếu nó lớn hơn 0
                return Json(new { CustomerId = customerId.Value }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                // Nếu customerId là null hoặc <= 0, trả về -1
                return Json(new { CustomerId = -1 }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public ActionResult UpdateOrderSection(int currentId, int targetId, int currentOrderSection, int targetOrderSection)
        {
            if (currentId <= 0 || targetId <= 0 || currentOrderSection < 0 || targetOrderSection < 0)
            {
                return Json(new { success = false, message = "Lỗi sắp xếp" });
            }

            var orderLeadSectionUpdate = SqlHelper.QuerySP<int>("sp_Crm_OrderLeadSection_Update", new
            {
                pCurrentId = currentId,
                pTargetId = targetId,
                pCurrentOrderSection = currentOrderSection,
                pTargetOrderSection = targetOrderSection,
            }).FirstOrDefault();

            return Json(new { success = true, message = "Cập nhật thành công" });
        }
        [HttpPost]
        public ActionResult UpdateOrderField(int currentId, int targetId, int currentOrderSection, int targetOrderSection)
        {
            if (currentId <= 0 || targetId <= 0 || currentOrderSection < 0 || targetOrderSection < 0)
            {
                return Json(new { success = false, message = "Lỗi sắp xếp" });
            }

            var leadSectionId = SqlHelper.QuerySP<LeadSection_FieldModel>("sp_Crm_OrderFieldLeadSection_Update", new
            {
                pCurrentId = currentId,
                pTargetId = targetId,
                pCurrentOrderSection = currentOrderSection,
                pTargetOrderSection = targetOrderSection,
            }).FirstOrDefault();
            return Json(new { success = true, message = "Cập nhật thành công", IdLeadSection = leadSectionId.LeadSectionId });
        }
        [HttpPost]
        public ActionResult UpdateNameLeadSection(int idLeadSection, string newName)
        {
            if (idLeadSection <= 0 || newName == null)
            {
                return Json(new { success = false, message = "Lỗi gửi dữ liệu" });
            }

            var nameLeadSectionUpdate = SqlHelper.QuerySP<int>("sp_Crm_NameLeadSection_Update", new
            {
                pIdLeadSection = idLeadSection,
                pName = newName,

            }).FirstOrDefault();

            return Json(new { success = true, message = "Cập nhật thành công" });
        }
        [HttpPost]
        public ActionResult InsertLeadSection(string newName, int previousOrderSection, int typeLead)
        {
            if (previousOrderSection <= 0 || newName == null || typeLead < 0)
            {
                return Json(new { success = false, message = "Lỗi gửi dữ liệu" });
            }

            var nameLeadSectionUpdate = SqlHelper.QuerySP<int>("sp_Crm_LeadSection_Insert", new
            {
                pPreviousOrderSection = previousOrderSection,
                pName = newName,
                pTypeLead = typeLead

            }).FirstOrDefault();

            return Json(new { success = true, message = "Cập nhật thành công" });
        }
        [HttpPost]
        public ActionResult UpdateNewValueLeadSectionField(int currentRowId, string fieldName, string newValue)
        {
            if (currentRowId <= 0 || newValue == null || fieldName == null)
            {
                return Json(new { success = false, message = "Lỗi sắp xếp" });
            }

            var leadSectionId = SqlHelper.QuerySP<LeadSection_FieldModel>("sp_Crm_FieldLeadSectionByVaLue_Update", new
            {
                pCurrentId = currentRowId,
                pFieldName = fieldName,
                pNewValue = newValue
            }).FirstOrDefault();
            return Json(new { success = true, message = "Cập nhật thành công", IdLeadSection = leadSectionId.LeadSectionId });
        }
        [HttpPost]
        public ActionResult InsertLeadSectionField(int leadSectionId, int previousOrderSection, string fieldValues)
        {
            Dictionary<string, string> fieldValuesDict = JsonConvert.DeserializeObject<Dictionary<string, string>>(fieldValues);
            if (leadSectionId <= 0 || fieldValuesDict == null || previousOrderSection < 0)
            {
                return Json(new { success = false, message = "Lỗi dữ liệu" });
            }

            var insertLeadSectionField = SqlHelper.QuerySP<int>("sp_Crm_FieldLeadSectionByVaLue_Insert", new
            {
                pLeadSectionId = leadSectionId,
                pPreviousOrderSection = previousOrderSection,
                pNameLabel = fieldValuesDict["NameLabel"],
                pFieldName = fieldValuesDict["FieldName"],
                pTypeField = fieldValuesDict["TypeField"],
                pCodeList = fieldValuesDict["CodeList"],
                pIsHiden = fieldValuesDict["IsHiden"] == "1" ? 1 : 0,

            }).FirstOrDefault();
            return Json(new { success = true, message = "Thêm LeadSectionField thành công" });
        }
        [HttpGet]
        public ActionResult GetFColumnNamesInLeadSecTionField(int typeLead)
        {
            Type type = typeof(LeadModel);
            PropertyInfo[] properties = type.GetProperties();
            var fColumns = properties
                .Where(p => p.Name.StartsWith("F"))
                .Select(p => p.Name)
                .ToList();
            fColumns.Add("CountForBrand");
            var listFieldNames = SqlHelper.QuerySP<LeadSection_FieldModel>("[Crm_FieldNamesForTypeLead_Get]", new
            {
                pTypeLead = typeLead,
            }).Select(x => x.FieldName).ToList();

            var differentColumns = fColumns.Except(listFieldNames).ToList();
            return Json(differentColumns, JsonRequestBehavior.AllowGet);
        }
        #endregion
        [HttpGet]
        public ActionResult ChartOutboundCall()
        {
            var user = userRepository.GetAllUsers();
            ViewBag.user = user;

            return View();
        }
        public class OutboundCallData
        {
            public string FullName { get; set; }
            public int Completes { get; set; }
            public int UnCompletes { get; set; }
            public int TotalCalls { get; set; }
            public int Index { get; set; }
        }

        [HttpPost]
        public ActionResult GetQuantityOutboundCall(string dauThang, string cuoiThang, string leadOrContact)
        {
            int leadOrContactInt = int.Parse(leadOrContact);

            var outboundCallData = SqlHelper.QuerySP<OutboundCallData>("[sp_Crm_QuantityOutboundCall_Get]", new
            {
                pDauThang = dauThang,
                pCuoiThang = cuoiThang,
                pLeadOrContact = leadOrContactInt
            }).ToList();

            // Sắp xếp lại dữ liệu theo Index
            outboundCallData = outboundCallData.OrderBy(d => d.Index).ToList();

            // Tạo các mảng tương ứng
            var userNames = outboundCallData.Select(d => d.FullName).ToArray();
            var completes = outboundCallData.Select(d => d.Completes).ToArray();
            var unCompletes = outboundCallData.Select(d => d.UnCompletes).ToArray();
            var totalCalls = outboundCallData.Select(d => d.TotalCalls).ToArray();

            return Json(new
            {
                success = true,
                message = "success",
                data = new
                {
                    userNames,
                    completes,
                    unCompletes,
                    totalCalls
                }
            });
        }
        [HttpGet]
        public ActionResult ChartStatusLeadOutboundCall()
        {
            var user = userRepository.GetAllUsers();
            ViewBag.user = user;

            return View();
        }

        public class OutboundCallDataDone
        {
            public string StatusName { get; set; }

            public int TotalCalls { get; set; }
            public int Index { get; set; }
        }
        [HttpPost]
        public ActionResult GetQuantityChartStatusLeadOutboundCall(string dauThang, string cuoiThang, string leadOrContact)
        {
            int leadOrContactInt = int.Parse(leadOrContact);
            var outboundCallDoneData = SqlHelper.QuerySP<OutboundCallDataDone>("[sp_Crm_QuantityChartStatusLeadOutboundCall_Get]", new
            {
                pDauThang = dauThang,
                pCuoiThang = cuoiThang,
                pLeadOrContact = leadOrContactInt
            }).ToList();

            // Sắp xếp lại dữ liệu theo Index
            outboundCallDoneData = outboundCallDoneData.OrderBy(d => d.Index).ToList();

            // Tạo các mảng tương ứng
            var statusNames = outboundCallDoneData.Select(d => d.StatusName).ToArray();
            var totalCalls = outboundCallDoneData.Select(d => d.TotalCalls).ToArray();


            return Json(new
            {
                success = true,
                message = "success",
                data = new
                {
                    statusNames,
                    totalCalls
                }
            });
        }
        [HttpGet]
        public ActionResult ChartBranchByTopicLead()
        {
            var user = userRepository.GetAllUsers();
            ViewBag.user = user;

            return View();
        }
        public class BranchByTopicLead
        {
            public string Topic { get; set; }
            public string CNQ1 { get; set; }
            public string CNQ5 { get; set; }
            public string CNTB { get; set; }
            public int Tong { get; set; }
            public int Index { get; set; }
        }
        [HttpPost]
        public ActionResult GetChartBranchByTopicLead(string dauThang, string cuoiThang, string leadOrContact)
        {
            int leadOrContactInt = int.Parse(leadOrContact);
            var chartBranchByTopicLead = SqlHelper.QuerySP<BranchByTopicLead>("[sp_Crm_ChartBranchByTopicLead_Get]", new
            {
                pDauThang = dauThang,
                pCuoiThang = cuoiThang,
                pLeadOrContact = leadOrContactInt
            }).ToList();

            // Sắp xếp lại dữ liệu theo Index
            chartBranchByTopicLead = chartBranchByTopicLead.OrderBy(d => d.Index).ToList();

            // Tạo các mảng tương ứng
            var topics = chartBranchByTopicLead.Select(d => d.Topic).ToArray();

            var quantities = ParseListGetChartBranchByTopicLead(chartBranchByTopicLead);
            return Json(new
            {
                success = true,
                message = "success",
                data = new
                {
                    topics,
                    quantities
                }
            });
        }
        public int[,] ParseListGetChartBranchByTopicLead(List<BranchByTopicLead> chartBranchByTopicLead)
        {
            // Mảng mới để lưu trữ các giá trị từ các thuộc tính
            var quantities = new List<List<int>>();

            // Lặp qua danh sách các đối tượng ChartBranchByTopicLead
            foreach (var item in chartBranchByTopicLead)
            {
                // Tạo một mảng con mới chứa các giá trị từ các thuộc tính
                var sublist = new List<int>
                {
                    int.Parse(item.CNQ1),
                    int.Parse(item.CNQ5),
                    int.Parse(item.CNTB)
                };

                // Thêm mảng con vào mảng chính
                quantities.Add(sublist);
            }

            // Chuyển mảng List<List<int>> thành mảng 2 chiều
            int[,] branchCounts = new int[quantities.Count, 3];
            for (int i = 0; i < quantities.Count; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    branchCounts[i, j] = quantities[i][j];
                }
            }

            // Trả về mảng 2 chiều branchCounts
            return branchCounts;
        }
        [HttpGet]
        public ActionResult ChartDataByTopic()
        {
            var user = userRepository.GetAllUsers();
            ViewBag.user = user;
            var branch = SqlHelper.QuerySP<BranchViewModel>("[sp_Crm_Branch_Get]", new
            {

            }).ToList();
            ViewBag.Branch = branch;
            return View();
        }
        public class GetChartDataByTopicDTO
        {
            public long TotalMeeting { get; set; }
            public long TotalKhachLen { get; set; }
            public long TotalKhachMua { get; set; }
            public long DoanhSoAo { get; set; }
            public long KhachLen { get; set; }

            public long DoanhSoThuc { get; set; }

            public long TotalDoanhSoAo { get; set; }
            public string Topic { get; set; }
            public string TopicKhachLen { get; set; }

            public int Index { get; set; }


        }
        //TỚI ĐÂY đã thêm cho mỗi store một leadOrContactInt nhưng chưa xử lí store
        [HttpPost]
        public ActionResult GetChartDataByTopic(string dauThang, string cuoiThang, string branchId, string leadOrContact)
        {
            int branchIdInt = int.Parse(branchId);
            int leadOrContactInt = int.Parse(leadOrContact);
            var chartDataByTopicMeeting = SqlHelper.QuerySP<GetChartDataByTopicDTO>("[sp_Crm_ChartDataByTopic_Get]", new
            {
                pDauThang = dauThang,
                pCuoiThang = cuoiThang,
                pBranchId = branchIdInt,
                pLeadOrContact = leadOrContactInt
            }).FirstOrDefault();
            long totalMeeting = chartDataByTopicMeeting.TotalMeeting > 0 ? chartDataByTopicMeeting.TotalMeeting : 0;
            long totalKhachLen = chartDataByTopicMeeting.TotalKhachLen > 0 ? chartDataByTopicMeeting.TotalKhachLen : 0;
            long totalKhachMua = chartDataByTopicMeeting.TotalKhachMua > 0 ? chartDataByTopicMeeting.TotalKhachMua : 0;
            long doanhSoAo = chartDataByTopicMeeting.DoanhSoAo > 0 ? chartDataByTopicMeeting.DoanhSoAo : 0;
            long doanhSoThuc = chartDataByTopicMeeting.DoanhSoThuc > 0 ? chartDataByTopicMeeting.DoanhSoThuc : 0;


            var chartDataByTopicTotalDoanhSoAo = SqlHelper.QuerySP<GetChartDataByTopicDTO>("[sp_Crm_ChartDataByTopic_DoanhSoAo_Get]", new
            {
                pDauThang = dauThang,
                pCuoiThang = cuoiThang,
                pBranchId = branchId,
                pLeadOrContact = leadOrContactInt

            }).ToList();
            chartDataByTopicTotalDoanhSoAo = chartDataByTopicTotalDoanhSoAo.OrderBy(d => d.Index).ToList();
            var totalDoanhSoAo = chartDataByTopicTotalDoanhSoAo.Select(d => d.TotalDoanhSoAo).ToArray();
            var topic = chartDataByTopicTotalDoanhSoAo.Select(d => d.Topic).ToArray();

            var chartDataByTopicKhachLen = SqlHelper.QuerySP<GetChartDataByTopicDTO>("[sp_Crm_ChartDataByTopic_KhachLen_Get]", new
            {
                pDauThang = dauThang,
                pCuoiThang = cuoiThang,
                pBranchId = branchId,
                pLeadOrContact = leadOrContactInt

            }).ToList();
            var khachLen = chartDataByTopicKhachLen.Select(d => d.KhachLen).ToArray();
            var topicKhachLen = chartDataByTopicKhachLen.Select(d => d.TopicKhachLen).ToArray();


            return Json(new
            {
                success = true,
                message = "success",
                data = new
                {
                    totalMeeting,
                    totalKhachLen,
                    totalKhachMua,
                    doanhSoAo,
                    doanhSoThuc,
                    totalDoanhSoAo,
                    topic,
                    khachLen,
                    topicKhachLen
                }
            });
        }
        [HttpGet]
        public ActionResult ChartStatusLeadByTopic()
        {
            var user = userRepository.GetAllUsers();
            ViewBag.user = user;
            var branch = SqlHelper.QuerySP<BranchViewModel>("[sp_Crm_Branch_Get]", new
            {

            }).ToList();
            ViewBag.Branch = branch;
            var statusLead = SqlHelper.QuerySP<StatusViewModel>("[sp_Crm_Status_Get]", new
            {
                pType = 1
            }).ToList();
            ViewBag.StatusLead = statusLead;
            return View();
        }
        [HttpPost]
        public ActionResult StatusGet(int type)
        {


            var statusLead = SqlHelper.QuerySP<StatusViewModel>("[sp_Crm_Status_Get]", new
            {
                pType = type
            }).ToList();

            return Json(new
            {
                success = true,
                message = "success",
                data = new
                {
                    statusLead
                }
            });
        }
        public class ChartStatusLeadByTopicDTO
        {
            public long ToTalLead { get; set; }

            public string Topic { get; set; }

            public int Index { get; set; }


        }
        [HttpPost]
        public ActionResult GetChartStatusLeadByTopic(string dauThang, string cuoiThang, string branchId, string statusLead, string leadOrContact)
        {
            int branchIdInt = int.Parse(branchId);

            int statusLeadInt = int.Parse(statusLead);

            int leadOrContactInt = int.Parse(leadOrContact);

            var chartDataByTopicTotalDoanhSoAo = SqlHelper.QuerySP<ChartStatusLeadByTopicDTO>("[sp_Crm_ChartDataByTopic_Status_Get]", new
            {
                pDauThang = dauThang,
                pCuoiThang = cuoiThang,
                pBranchId = branchIdInt,
                pStatusLead = statusLeadInt,
                pLeadOrContact = leadOrContactInt
            }).ToList();
            chartDataByTopicTotalDoanhSoAo = chartDataByTopicTotalDoanhSoAo.OrderBy(d => d.Index).ToList();
            var toTalLead = chartDataByTopicTotalDoanhSoAo.Select(d => d.ToTalLead).ToArray();
            var topic = chartDataByTopicTotalDoanhSoAo.Select(d => d.Topic).ToArray();




            return Json(new
            {
                success = true,
                message = "success",
                data = new
                {

                    toTalLead,
                    topic,

                }
            });
        }
        [HttpGet]
        public ActionResult ChartTotalChangeOutBoundCall()
        {
            var user = userRepository.GetAllUsers();
            ViewBag.user = user;
            var branch = SqlHelper.QuerySP<BranchViewModel>("[sp_Crm_Branch_Get]", new
            {

            }).ToList();
            ViewBag.Branch = branch;
            return View();
        }
        [HttpPost]
        public ActionResult GetChartTotalChangeOutBoundCall(string dauThang, string cuoiThang, string branchId, string leadOrContact)
        {
            int branchIdInt = int.Parse(branchId);

            int leadOrContactInt = int.Parse(leadOrContact);

            var listDeNghiHuy = SqlHelper.QuerySP<ChartTotalChangeOutBoundCallViewModel>("[sp_Crm_ChartTotalChangeOutBoundCall_DeNghiHuy_Get]", new
            {
                pDauThang = dauThang,
                pCuoiThang = cuoiThang,
                pBranchId = branchIdInt,
                pLeadOrContact = leadOrContactInt
            }).FirstOrDefault();
            var nam = 0;
            var saiSo = 0;
            var tatMayNgang_KhoKhaiThac = 0;
            var deNghiHuy = 0;
            if (listDeNghiHuy != null)
            {
                nam = listDeNghiHuy.Nam;
                saiSo = listDeNghiHuy.SaiSo;
                tatMayNgang_KhoKhaiThac = listDeNghiHuy.TatMayNgang_KhoKhaiThac;
                deNghiHuy = listDeNghiHuy.DeNghiHuy;

            }
            var chuaCoNhuCau = 0;
            var simKhoa = 0;
            var guiThongTinQuaZalo = 0;
            var khongNgheMay = 0;
            var datHenKhongTC = 0;

            var listDatHenKhongTC = SqlHelper.QuerySP<ChartTotalChangeOutBoundCallViewModel>("[sp_Crm_ChartTotalChangeOutBoundCall_DatHenKhongTC_Get]", new
            {
                pDauThang = dauThang,
                pCuoiThang = cuoiThang,
                pBranchId = branchIdInt,
                pLeadOrContact = leadOrContactInt
            }).FirstOrDefault();
            if (listDatHenKhongTC != null)
            {
                chuaCoNhuCau = listDatHenKhongTC.ChuaCoNhuCau;
                simKhoa = listDatHenKhongTC.SimKhoa;
                guiThongTinQuaZalo = listDatHenKhongTC.GuiThongTinQuaZalo;
                khongNgheMay = listDatHenKhongTC.KhongNgheMay;
                datHenKhongTC = listDatHenKhongTC.DatHenKhongTC;

            }
            var nhacHenXa = 0;
            var nhacHenGan = 0;
            var datHen = 0;

            var listDatHen = SqlHelper.QuerySP<ChartTotalChangeOutBoundCallViewModel>("[sp_Crm_ChartTotalChangeOutBoundCall_DatHen_Get]", new
            {
                pDauThang = dauThang,
                pCuoiThang = cuoiThang,
                pBranchId = branchIdInt,
                pLeadOrContact = leadOrContactInt

            }).FirstOrDefault();
            if (listDatHen != null)
            {
                nhacHenXa = listDatHen.NhacHenXa;
                nhacHenGan = listDatHen.NhacHenGan;
                datHen = listDatHen.DatHen;
            }

            var goiLaiSau = 0;
            var theoLai = 0;
            var listTheoLai = SqlHelper.QuerySP<ChartTotalChangeOutBoundCallViewModel>("[sp_Crm_ChartTotalChangeOutBoundCall_TheoLai_Get]", new
            {
                pDauThang = dauThang,
                pCuoiThang = cuoiThang,
                pBranchId = branchIdInt,
                pLeadOrContact = leadOrContactInt

            }).FirstOrDefault();
            if (listTheoLai != null)
            {
                goiLaiSau = listTheoLai.GoiLaiSau;
                theoLai = listTheoLai.TheoLai;
            }


            var totalChangeOutBoundCallLead = SqlHelper.QuerySP<ChartTotalChangeOutBoundCallLeadViewModel>("[sp_Crm_ChartTotalChangeOutBoundCall_Lead_Get]", new
            {
                pDauThang = dauThang,
                pCuoiThang = cuoiThang,
                pBranchId = branchIdInt,
                pLeadOrContact = leadOrContactInt

            }).ToList();
            return Json(new
            {
                success = true,
                message = "success",
                data = new
                {
                    nam,
                    saiSo,
                    tatMayNgang_KhoKhaiThac,
                    chuaCoNhuCau,
                    deNghiHuy,
                    simKhoa,
                    guiThongTinQuaZalo,
                    khongNgheMay,
                    datHenKhongTC,
                    nhacHenXa,
                    nhacHenGan,
                    datHen,
                    goiLaiSau,
                    theoLai,
                    totalChangeOutBoundCallLead
                }
            });
        }
        [HttpPost]
        public ActionResult ExistedFieldValueLeadCheck(string fieldName)
        {
            var existedFieldValueLead = SqlHelper.QuerySP<dynamic>("[sp_Crm_ExistedFieldValueLead_Check]", new
            {
                pFieldName = fieldName
            }).FirstOrDefault();


            int existedField = existedFieldValueLead.ExistedFieldValueLead;
            return Json(new
            {
                success = true,
                message = "Thành công",
                data = new
                {
                    existedField
                }
            });
        }
    }
}
