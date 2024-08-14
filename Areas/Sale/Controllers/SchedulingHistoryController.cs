using System.Globalization;
using Erp.BackOffice.Sale.Models;
using Erp.BackOffice.Filters;
using Erp.Domain.Entities;
using Erp.Domain.Interfaces;
//using Erp.Domain.Sale.Entities;
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
using Erp.Domain.Staff.Interfaces;
using Erp.BackOffice.Staff.Models;
using Erp.BackOffice.Areas.Administration.Models;
using Erp.Domain.Staff.Entities;
using System.Net.Http;
using System.Net.Http.Headers;
//using OneSignal.CSharp.SDK;
//using Push_Notification_OneSignal;
//using OneSignal.CSharp.SDK.Resources;
using System.Net.Http;
using System.Net.Http.Headers;
using Erp.Domain.Sale.Entities;
using OneSignal.CSharp.SDK;
using Push_Notification_OneSignal;
using OneSignal.CSharp.SDK.Resources;
using Microsoft.Owin.Security.Infrastructure;
using System.Transactions;
using System.Net;
using Erp.Domain.Repositories;
using System.Web;
using Erp.Domain.Account.Helper;

namespace Erp.BackOffice.Sale.Controllers
{
    [Authorize]
    [InitializeSimpleMembership]
    [Erp.BackOffice.Helpers.NoCacheHelper]
    public class SchedulingHistoryController : Controller
    {
        private readonly IStaffEquipmentRepository StaffEquipmentRepository;
        private readonly ISchedulingHistoryRepository SchedulingHistoryRepository;
        private readonly IUserRepository userRepository;
        private readonly IInquiryCardRepository inquiryCardRepository;
        private readonly IRoomRepository roomRepository;
        private readonly IBedRepository bedRepository;
        private readonly IStaffMadeRepository staffMadeRepository;
        private readonly ICheckInOutRepository checkInOutRepository;
        private readonly IServiceScheduleRepository serviceScheduleRepository;
        private readonly IStaffEquipmentRepository staffEquipmentRepository;
        private readonly ILogEquipmentRepository logEquipmentRepository;
        private readonly IMembershipRepository MembershipRepository;
        private readonly ITemplatePrintRepository templatePrintRepository;

        public SchedulingHistoryController(
            ISchedulingHistoryRepository _SchedulingHistory
            , IUserRepository _user
            , IInquiryCardRepository inquiryCard
            , IRoomRepository room
            , IBedRepository bed
            , IStaffMadeRepository staffMade
            , ICheckInOutRepository checkInOut
            , IServiceScheduleRepository serviceSchedule
            , IStaffEquipmentRepository staffEquipment
            , ILogEquipmentRepository logEquipment
            , IMembershipRepository membership
            , IStaffEquipmentRepository _StaffEquipment
              , ITemplatePrintRepository templatePrint
            )
        {
            SchedulingHistoryRepository = _SchedulingHistory;
            userRepository = _user;
            inquiryCardRepository = inquiryCard;
            roomRepository = room;
            bedRepository = bed;
            staffMadeRepository = staffMade;
            checkInOutRepository = checkInOut;
            serviceScheduleRepository = serviceSchedule;
            staffEquipmentRepository = staffEquipment;
            logEquipmentRepository = logEquipment;
            MembershipRepository = membership;
            StaffEquipmentRepository = _StaffEquipment;
            templatePrintRepository = templatePrint;
        }

        #region Index

        public ViewResult Index()
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

            var model = new SchedulingHistoryViewModel();
            model.BranchId = 0;
            if(intBrandID > 0)
            {
                model.BranchId = intBrandID;
            }
            var time_now = DateTime.Now.ToShortDateString();
            //danh sách phiếu yêu cầu chưa xử lý trong ngày
            model.InquiryCardList = new List<InquiryCardViewModel>();
            //model.InquiryCardList = inquiryCardRepository.GetvwAllInquiryCard().AsEnumerable().Where(x => x.BranchId == model.BranchId && x.WorkDay.Value.ToShortDateString() == time_now && (x.Status == "pending")).Select(x => new InquiryCardViewModel
            //{
            //    Code = x.Code,
            //    CustomerCode = x.CustomerCode,
            //    CustomerName = x.CustomerName,
            //    CustomerId = x.CustomerId,
            //    Id = x.Id,
            //    ProductName = x.ProductName,
            //    ProductCode = x.ProductCode,
            //    ProductId = x.ProductId,
            //    WorkDay = x.WorkDay,
            //    Note = x.Note,
            //    TotalMinute = x.TotalMinute,
            //    Status = x.Status,
            //    CustomerImage = x.CustomerImage,
            //    TargetModule = x.TargetModule
            //}).OrderBy(x => x.WorkDay).ToList();


            model.InquiryCardList = SqlHelper.QuerySP<InquiryCardViewModel>("getallSale_InquiryCard", new
            {
                pWorkDay = time_now,
                pBranchId = model.BranchId,
                pStatus = "pending"
            }).ToList();


            //danh sách phòng của chi nhánh
            model.RoomList = new List<RoomViewModel>();
            List<Bed> ListBed = new List<Bed>();
            ListBed = bedRepository.GetAllBed();
            model.RoomList = SqlHelper.QuerySP<RoomViewModel>("TOIUUCSKH_AllRoom", new { @pBranchId =model.BranchId}).ToList();

			foreach (var item in model.RoomList)
            {
                List<Bed> listbedbyidRoom = new List<Bed>();
                listbedbyidRoom = bedRepository.GetAllBedbyIdRoom(item.Id);
                item.bedList = listbedbyidRoom;
            }

            //danh sách phiếu xếp lịch trong ngày đang đợi thực hiện dịch vụ.
            model.SchedulingList = new List<SchedulingHistoryViewModel>();
            //model.SchedulingList = SchedulingHistoryRepository.GetvwAllSchedulingHistory().AsEnumerable().Where(x => x.BranchId == model.BranchId && x.ExpectedWorkDay.Value.ToShortDateString() == time_now && x.Status == "pending").Select(x => new SchedulingHistoryViewModel
            //{
            //    NameNV = x.NameNV,
            //    Code = x.Code,
            //    CustomerCode = x.CustomerCode,
            //    CustomerName = x.CustomerName,
            //    CustomerId = x.CustomerId,
            //    ExpectedEndDate = x.ExpectedEndDate,
            //    Id = x.Id,
            //    InquiryCardCode = x.InquiryCardCode,
            //    InquiryCardId = x.InquiryCardId,
            //    Note = x.Note,
            //    ProductCode = x.ProductCode,
            //    ProductId = x.ProductId,
            //    ProductName = x.ProductName,
            //    RoomId = x.RoomId,
            //    BedId = x.BedId,
            //    Status = x.Status,
            //    WorkDay = x.WorkDay,
            //    TotalMinute = x.TotalMinute,
            //    Type = x.Type,
            //    RoomName = x.RoomName,
            //    FloorName = x.FloorName,
            //    CustomerImage = x.CustomerImage,
            //    ExpectedWorkDay = x.ExpectedWorkDay
            //}).OrderBy(x => x.ExpectedWorkDay).ToList();


            model.SchedulingList = SqlHelper.QuerySP<SchedulingHistoryViewModel>("getallvwSale_SchedulingHistory", new
            {
                pWorkDay = time_now,
                pBranchId = model.BranchId,
                pStatus = "pending"
            }).ToList();



            //var staffMade = staffMadeRepository.GetvwAllStaffMade().AsEnumerable().Where(x => x.ExpectedWorkDay != null && x.BranchId == model.BranchId && x.ExpectedWorkDay.Value.ToShortDateString() == time_now).ToList();

            var staffMade = SqlHelper.QuerySP<vwStaffMade>("getallvwSale_StaffMade", new
            {
                pWorkDay = time_now,
                pBranchId = model.BranchId
            }).ToList();



            foreach (var item in model.SchedulingList)
            {
                item.StaffMadeList = new List<StaffMadeViewModel>();
                item.StaffMadeList = staffMade.Where(x => x.SchedulingHistoryId == item.Id).Select(x => new StaffMadeViewModel
                {
                    FullName = x.FullName,
                    UserCode = x.UserCode,
                    UserId = x.UserId,
                    Status = x.Status
                }).ToList();
            }
            //danh sách khách đặt hẹn
            UserRepository userRepository = new UserRepository(new Domain.ErpDbContext());
            model.ServiceScheduleList = new List<ServiceScheduleViewModel>();
            //var ServiceScheduleLists = serviceScheduleRepository.GetListAllvwServiceSchedule().Where(x => x.BranchId == model.BranchId && x.StartDate.Value.ToShortDateString() == time_now).ToList();
            var ServiceScheduleLists = SqlHelper.QuerySP<vwServiceSchedule>("getallvwSale_ServiceSchedule", new
            {
                pWorkDay = time_now,
                pBranchId = model.BranchId
            }).ToList();

            foreach (var item in ServiceScheduleLists)
            {
                var tmp = new ServiceScheduleViewModel();
                tmp.Id = item.Id;
                tmp.Code = item.Code;
                tmp.CustomerCode = item.CustomerCode;
                tmp.CustomerId = item.CustomerId;
                tmp.CustomerName = item.CustomerName;
                tmp.CustomerImage = item.CustomerImage;
                tmp.DueDate = item.DueDate;
                tmp.Note = item.Note;
                tmp.ServiceCode = item.ServiceCode;
                tmp.ServiceId = item.ServiceId;
                tmp.ServiceName = item.ServiceName;
                tmp.StartDate = item.StartDate;
                tmp.Status = item.Status;
                tmp.AssignedUserId = item.AssignedUserId;
                tmp.Chanel = item.Chanel;
                var a = userRepository.GetUserById(item.AssignedUserId.Value);
                tmp.NameAssignedUser = a.FullName;
                model.ServiceScheduleList.Add(tmp);
            }
            ViewBag.SuccessMessage = TempData["SuccessMessage"];
            ViewBag.FailedMessage = TempData["FailedMessage"];
            ViewBag.AlertMessage = TempData["AlertMessage"];
            return View(model);
        }
        #endregion

        #region List

        public ViewResult List(string startDate, string endDate, string txtCode, string txtCusCode,string txtDichvu,
            string txtCusName, string Status, int? BranchId)
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

            IEnumerable<SchedulingHistoryViewModel> q = SchedulingHistoryRepository.GetvwAllSchedulingHistory()
                .Select(item => new SchedulingHistoryViewModel
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
                    CreatedUserCode = item.CreatedUserCode,
                    CreatedUserName = item.CreatedUserName,
                    CustomerCode = item.CustomerCode,
                    CustomerName = item.CustomerName,
                    BranchId = item.BranchId,
                    Type = item.Type,
                    WorkDay = item.WorkDay,
                    CustomerId = item.CustomerId,
                    CustomerImage = item.CustomerImage,
                    RoomName = item.RoomName,
                    InquiryCardCode = item.InquiryCardCode,
                    InquiryCardId = item.InquiryCardId,
                    ProductName = item.ProductName,
                    ProductCode = item.ProductCode,
                    Status = item.Status,
                    strexecutionTime = item.TimeExecution,
                    NameNV = item.NameNV,
                    startTime = item.startTime,
                    endTime = item.endTime,
                    TargetModule = item.TargetModule,
                    //strTotalMinute = ""+item.TotalMinute
                    TotalMinute =   item.TotalMinute,
                    ExpectedEndDate = item.ExpectedEndDate,
                    
                   
                    
                }).OrderByDescending(m => m.CreatedDate).ToList();

            
            
            foreach(var a in q)
            {
                var id = a.Id;
                var hour = int.Parse(a.TotalMinute.ToString()) / 60; ;
                var minute = double.Parse(a.TotalMinute.ToString()) % 60;

                if(a.startTime != null)
                {
                    var expectedhour = int.Parse(a.startTime.Split(':')[0]) + hour;
                    var expedtedminute = int.Parse( a.startTime.Split(':')[1] )+ minute;

                    if(expectedhour >23)
                    {
                        expectedhour = 00;
                        expedtedminute = expedtedminute - 60;
                    }
                    else if( expedtedminute >= 60 )
                    {
                        expectedhour++;
                        expedtedminute = expedtedminute - 60;
                    }
                    a.ExpectFinishHour = expectedhour + ":" + expedtedminute;
                }
                
            }

            if (!string.IsNullOrEmpty(txtCode))
            {
                txtCode = Helpers.Common.ChuyenThanhKhongDau(txtCode);
                q = q.Where(x => Helpers.Common.ChuyenThanhKhongDau(x.Code).Contains(txtCode)).ToList();
            }
            if (!string.IsNullOrEmpty(txtCusCode))
            {
                txtCusCode = Helpers.Common.ChuyenThanhKhongDau(txtCusCode);
                q = q.Where(x => Helpers.Common.ChuyenThanhKhongDau(x.CustomerCode).Contains(txtCusCode)).ToList();
            }
            //if (!string.IsNullOrEmpty(type))
            //{
            //    q = q.Where(x => x.Type == type).ToList();
            //}
            if (!string.IsNullOrEmpty(txtCusName))
            {
                txtCusName = Helpers.Common.ChuyenThanhKhongDau(txtCusName);
                //q = q.Where(x => Helpers.Common.ChuyenThanhKhongDau(x.CustomerCode).Contains(txtCusName)).ToList();

             //   txtCusName = Helpers.Common.ChuyenThanhKhongDau(txtCusName);
                q = q.Where(x => Helpers.Common.ChuyenThanhKhongDau(x.CustomerCode).Contains(txtCusName) || Helpers.Common.ChuyenThanhKhongDau(x.CustomerName).Replace(" ", "").Contains((Helpers.Common.ChuyenThanhKhongDau(txtCusName)).Replace(" ", ""))).ToList();
          
            }
            if (!string.IsNullOrEmpty(txtDichvu))
            {
                txtDichvu = txtDichvu == "" ? "~" : Erp.BackOffice.Helpers.Common.ChuyenThanhKhongDau(txtDichvu).Trim();

                q = q.Where(x => Erp.BackOffice.Helpers.Common.ChuyenThanhKhongDau(x.ProductName).Contains(txtDichvu)).ToList();
            }

            DateTime d_startDate, d_endDate;
            if (DateTime.TryParseExact(startDate, "dd/MM/yyyy", new CultureInfo("vi-VN"), DateTimeStyles.None, out d_startDate))
            {
                if (DateTime.TryParseExact(endDate, "dd/MM/yyyy", new CultureInfo("vi-VN"), DateTimeStyles.None, out d_endDate))
                {
                    d_endDate = d_endDate.AddHours(23).AddMinutes(59);
                    q = q.Where(x => x.CreatedDate >= d_startDate && x.CreatedDate <= d_endDate).ToList();
                }

            }
            if (BranchId != null && BranchId.Value > 0)
            {
                q = q.Where(x => x.BranchId == BranchId).ToList();
            }

            if (!string.IsNullOrEmpty(Status))
            {
                q = q.Where(x => x.Status == Status).ToList();
            }

            ViewBag.SuccessMessage = TempData["SuccessMessage"];
            ViewBag.FailedMessage = TempData["FailedMessage"];
            ViewBag.AlertMessage = TempData["AlertMessage"];
            try
            {
                var q1 = q.ToList();
            }
            catch (Exception ex)
            {

                throw ex;
            }

            if (txtCusName != null || startDate != null || txtCusCode != null || Status != null || txtCode != null)
            {
                return View(q.OrderBy(a => a.CustomerName).ToList());
            }
            else
            {
                return View(q.ToList());
            }

        }
        #endregion

        #region Create
        public ViewResult Create(int InquiryCardId)
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

            var model = new SchedulingHistoryViewModel();
            model.Code = Erp.BackOffice.Helpers.Common.GetOrderNo("SchedulingHistory");
            model.BranchId = intBrandID;
            var time_now = DateTime.Now.ToShortDateString();
            //map dữ liệu từ phiếu yêu cầu vào model
            var info = inquiryCardRepository.GetvwInquiryCardById(InquiryCardId);
            if (info != null)
            {
                model.CustomerId = info.CustomerId;
                model.TotalMinute = info.TotalMinute;
                model.ExpectedWorkDay = info.WorkDay;
                model.ProductId = info.ProductId;
                model.ProductCode = info.ProductCode;
                model.CustomerCode = info.CustomerCode;
                model.CustomerName = info.CustomerCode + "(" + info.CustomerName + ")";
                model.ProductName = info.ProductName;
                model.InquiryCardCode = info.Code;
                model.InquiryCardId = info.Id;

                model.ExpectedEndDate = info.WorkDay.Value.AddMinutes(model.TotalMinute.Value);
            }

            //lấy list user để chọn
            var _data_user = userRepository.GetAllvwUsers().Where(x => x.BranchId == model.BranchId && x.UserTypeCode != "Admin").ToList();
            //lấy dữ liệu ra vào của chi nhánh
            var aa = checkInOutRepository.GetAllvwCheckInOut().Where(x => x.Sale_BranchId == model.BranchId).ToList();
            var check_in = aa.Where(x => x.TimeType == "In" && x.TimeDate.Value.ToShortDateString() == time_now).ToList();
            var check_out = aa.Where(x => x.TimeType == "Out" && x.TimeDate.Value.ToShortDateString() == time_now).ToList();
            //nếu nhân viên có check in mà ko có check out thì lấy ra.. xem như là nhân viên đó đang làm
            var _online = check_in.Where(id1 => !check_out.Any(id2 => id2.UserId == id1.UserId)).ToList();



            //lay nhan vien dang cham soc chua xong
            var staffMadeCS = staffMadeRepository.GetvwAllStaffMade().AsEnumerable().Where(x => x.BranchId == model.BranchId && x.ExpectedWorkDay.Value.ToShortDateString() == time_now && x.SchedulingStatus != "complete"
                ).ToList();

            if (staffMadeCS.Count() > 0)
            {
                _online = _online.Where(id1 => !staffMadeCS.Any(id2 => id2.UserId == id1.UserId)).ToList();
            }




            model.UserList = new List<UserOnlineViewModel>();
            model.UserList = _data_user.Where(id1 => _online.Any(id2 => id2.UserId == id1.Id)).Select(x => new UserOnlineViewModel
            {
                Id = x.Id,
                ProfileImage = x.ProfileImage,
                FullName = x.FullName,
                UserCode = x.UserCode,
                UserTypeId = x.UserTypeId,
                UserTypeName = x.UserTypeName
            }).ToList();
            //đếm số lượt mà nhân viên đã làm trong ngày
            var staffMade = staffMadeRepository.GetvwAllStaffMade().AsEnumerable().Where(x => x.WorkDay != null && x.BranchId == model.BranchId && x.WorkDay.Value.ToShortDateString() == time_now)
                .GroupBy(x => x.UserId).Select(x => new
                {
                    UserId = x.Key,
                    count = x.Count()
                }).ToList();
            //gắn số lượt nhân viên đã làm vào danh sách user đang online
            foreach (var item in model.UserList)
            {
                if (staffMade.Any(x => x.UserId == item.Id))
                {
                    item.Count_Scheduling = staffMade.FirstOrDefault(x => x.UserId == item.Id).count;
                }
            }
            //lấy StaffEquipmentList
            var StaffEquipmentLists = staffEquipmentRepository.GetListAllStaffEquipment().Where(u => u.BranchId == model.BranchId && u.Status == true).ToList();
            model.StaffEquipmentList = StaffEquipmentLists;
            //

            return View(model);
        }

        [HttpPost]
        public ActionResult Create(SchedulingHistoryViewModel model, bool? IsPopup, int user_check)
        {

            Bed updateTrangthai = new Bed();
            updateTrangthai = bedRepository.GetBedById(model.BedId.Value);
            updateTrangthai.Trang_Thai = true;
            bedRepository.UpdateBed(updateTrangthai);
            Room isroom = new Room();
            isroom = roomRepository.GetRoomById(updateTrangthai.Room_Id);


            var SchedulingHistory = new SchedulingHistory();
            AutoMapper.Mapper.Map(model, SchedulingHistory);
            SchedulingHistory.IsDeleted = false;
            SchedulingHistory.CreatedUserId = WebSecurity.CurrentUserId;
            SchedulingHistory.ModifiedUserId = WebSecurity.CurrentUserId;
            SchedulingHistory.AssignedUserId = WebSecurity.CurrentUserId;
            SchedulingHistory.CreatedDate = DateTime.Now;
            SchedulingHistory.IdNV = user_check;
            SchedulingHistory.ModifiedDate = DateTime.Now;
            SchedulingHistory.Status = "pending";
            SchedulingHistory.RoomId = updateTrangthai.Room_Id;
            SchedulingHistory.Name_Bed = updateTrangthai.Name_Bed;
            SchedulingHistory.Name_Room = isroom.Name;
            SchedulingHistory.NameNV = "";

            SchedulingHistoryRepository.InsertSchedulingHistory(SchedulingHistory);
            SchedulingHistory.Code = Erp.BackOffice.Helpers.Common.GetOrderNo("SchedulingHistory", model.Code);




            Erp.BackOffice.Helpers.Common.SetOrderNo("SchedulingHistory");
            //lưu danh sách nhân viên thực hiện
            List<string> ListUser = new List<string>();
            if (Request["user_check"] != null)
            {
                ListUser = Request["user_check"].Split(',').ToList();
                for (int i = 0; i < ListUser.Count(); i++)
                {
                    var add = new StaffMade();
                    add.UserId = Convert.ToInt32(ListUser[i].ToString());
                    add.IsDeleted = false;
                    add.CreatedUserId = WebSecurity.CurrentUserId;
                    add.ModifiedUserId = WebSecurity.CurrentUserId;
                    add.AssignedUserId = WebSecurity.CurrentUserId;
                    add.CreatedDate = DateTime.Now;
                    add.ModifiedDate = DateTime.Now;
                    add.SchedulingHistoryId = SchedulingHistory.Id;
                    add.Status = "pending";
                    staffMadeRepository.InsertStaffMade(add);
                    //lay ten nhan vien
                    int idnv = Convert.ToInt32(ListUser[i].ToString());
                    var NV = userRepository.GetUserById(idnv);
                    if (i == 0)
                    {
                        SchedulingHistory.NameNV = SchedulingHistory.NameNV + NV.FullName;
                    }
                    else
                    {
                        SchedulingHistory.NameNV = SchedulingHistory.NameNV + " + " + NV.FullName;
                    }

                }
            }
            SchedulingHistoryRepository.UpdateSchedulingHistory(SchedulingHistory);
            //ghi log 
            Erp.BackOffice.Controllers.HomeController.WriteLog(SchedulingHistory.Id, SchedulingHistory.Code, "đã tạo phiếu xếp lịch", "SchedulingHistory/Detail/" + SchedulingHistory.Id, Helpers.Common.CurrentUser.BranchId.Value);


            //cập nhật lại phiếu yêu cầu là đã xếp lịch
            if (SchedulingHistory.InquiryCardId != null)
            {
                var _update = inquiryCardRepository.GetInquiryCardById(SchedulingHistory.InquiryCardId.Value);
                if (_update != null)
                {
                    _update.Status = "has_scheduled";
                    inquiryCardRepository.UpdateInquiryCard(_update);
                }
            }
            //lưu danh sách trang thiet16bi5 thực hiện dịch vụ
            List<LogEquipmentViewModel> LogListEquipment = new List<LogEquipmentViewModel>();
            if (Request["StaffMade_check"] != null)
            {

                LogEquipmentViewModel logtmp = new LogEquipmentViewModel();
                List<string> ListStaffMade = new List<string>();
                ListStaffMade = Request["StaffMade_check"].Split(',').ToList();
                foreach (var item in ListStaffMade)
                {
                    var Equipment = StaffEquipmentRepository.GetStaffEquipmentById(int.Parse(item));
                    var add = new Domain.Sale.Entities.LogEquipment();
                    add.IsDeleted = false;
                    add.CreatedUserId = WebSecurity.CurrentUserId;
                    add.ModifiedUserId = WebSecurity.CurrentUserId;
                    add.CreatedDate = DateTime.Now;
                    add.ModifiedDate = DateTime.Now;
                    add.EquipmentId = Equipment.Id;
                    add.Status = "pending";
                    add.SchedulingHistoryId = SchedulingHistory.Id;
                    logEquipmentRepository.InsertLogEquipment(add);
                    Equipment.StatusStaffMade = true;
                    StaffEquipmentRepository.UpdateEquipment(Equipment);

                }
            }

            //Thông báo cho người dùng

            Crm.Controllers.ProcessController.Run("SchedulingHistory", "Create", SchedulingHistory.Id, SchedulingHistory.ModifiedUserId, null, SchedulingHistory);



            if (IsPopup == true)
            {
                return RedirectToAction("_ClosePopup", "Home", new { area = "", FunctionCallback = "ClosePopupAndReloadPage" });
            }
            else
            {
                TempData[Globals.SuccessMessageKey] = App_GlobalResources.Wording.InsertSuccess;
                return RedirectToAction("Index");
            }

            // return View(model);
        }
        #endregion

        #region Edit
        public ActionResult Edit(int? Id)
        {
            var SchedulingHistory = SchedulingHistoryRepository.GetvwSchedulingHistoryById(Id.Value);
            var time_now = DateTime.Now.ToShortDateString();
            if (SchedulingHistory != null && SchedulingHistory.IsDeleted != true)
            {
                var model = new SchedulingHistoryViewModel();
                AutoMapper.Mapper.Map(SchedulingHistory, model);
                model.StaffMadeList = new List<StaffMadeViewModel>();
                //model.StaffMadeList = staffMadeRepository.GetvwAllStaffMade().Where(x => x.SchedulingHistoryId == Id).Select(x => new StaffMadeViewModel
                //{
                //    UserId = x.UserId
                //}).ToList();


                //lấy list user để chọn
                var _data_user = userRepository.GetAllvwUsers().Where(x => x.BranchId == model.BranchId && x.UserTypeCode != "Admin").ToList();
                //lấy dữ liệu ra vào của chi nhánh
                var aa = checkInOutRepository.GetAllvwCheckInOut().Where(x => x.Sale_BranchId == model.BranchId).ToList();
                var check_in = aa.Where(x => x.TimeType == "In" && x.TimeDate.Value.ToShortDateString() == time_now).ToList();
                var check_out = aa.Where(x => x.TimeType == "Out" && x.TimeDate.Value.ToShortDateString() == time_now).ToList();
                //nếu nhân viên có check in mà ko có check out thì lấy ra.. xem như là nhân viên đó đang làm
                var _online = check_in.Where(id1 => !check_out.Any(id2 => id2.UserId == id1.UserId)).ToList();
                //lay nhan vien dang cham soc chua xong
                var staffMadeCS = staffMadeRepository.GetvwAllStaffMade().AsEnumerable().Where(x => x.BranchId == model.BranchId && x.ExpectedWorkDay.Value.ToShortDateString() == time_now && x.SchedulingStatus != "complete"
                    ).ToList();

                if (staffMadeCS.Count() > 0)
                {
                    _online = _online.Where(id1 => !staffMadeCS.Any(id2 => id2.UserId == id1.UserId && id2.Status != "pending")).ToList();
                }

                model.UserList = new List<UserOnlineViewModel>();
                model.UserList = _data_user.Where(id1 => _online.Any(id2 => id2.UserId == id1.Id)).Select(x => new UserOnlineViewModel
                {
                    Id = x.Id,
                    ProfileImage = x.ProfileImage,
                    FullName = x.FullName,
                    UserCode = x.UserCode,
                    UserTypeId = x.UserTypeId,
                    UserTypeName = x.UserTypeName
                }).ToList();
                //đếm số lượt mà nhân viên đã làm trong ngày
                var staffMade = staffMadeRepository.GetvwAllStaffMade().AsEnumerable().Where(x => x.WorkDay != null && x.BranchId == model.BranchId && x.WorkDay.Value.ToShortDateString() == time_now)
                    .GroupBy(x => x.UserId).Select(x => new
                    {
                        UserId = x.Key,
                        count = x.Count()
                    }).ToList();
                //gắn số lượt nhân viên đã làm vào danh sách user đang online
                foreach (var item in model.UserList)
                {
                    if (staffMade.Any(x => x.UserId == item.Id))
                    {
                        item.Count_Scheduling = staffMade.FirstOrDefault(x => x.UserId == item.Id).count;
                    }
                }
                if (model.StaffMadeList.Any())
                {
                    model.UserList = model.UserList.Where(id1 => !model.StaffMadeList.Any(id2 => id2.UserId == id1.Id)).ToList();
                }
                //lấy danh sách trang thiết bị
                model.EquipmentList = new List<LogEquipmentViewModel>();
                model.EquipmentList = logEquipmentRepository.GetvwAllLogEquipment().Where(x => x.SchedulingHistoryId == SchedulingHistory.Id).Select(x => new LogEquipmentViewModel
                {
                    Id = x.Id,
                    EquipmentId = x.EquipmentId,
                    InspectionDate = x.InspectionDate,
                    Note = x.Note,
                    StaffEquipmentCode = x.StaffEquipmentCode,
                    SchedulingHistoryId = x.SchedulingHistoryId,
                    StaffEquipmentName = x.StaffEquipmentName,
                    Status = x.Status
                }).ToList();
                return View(model);
            }
            if (Request.UrlReferrer != null)
                return Redirect(Request.UrlReferrer.AbsoluteUri);
            return RedirectToAction("Index");
        }

        private long ConvertToTimestamp(DateTime value)
        {
            TimeZoneInfo NYTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
            DateTime NyTime = TimeZoneInfo.ConvertTime(value, NYTimeZone);
            TimeZone localZone = TimeZone.CurrentTimeZone;
            System.Globalization.DaylightTime dst = localZone.GetDaylightChanges(NyTime.Year);
            NyTime = NyTime.AddHours(-1);
            DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, 0).ToLocalTime();
            TimeSpan span = (NyTime - epoch);
            return (long)Convert.ToDouble(span.TotalSeconds);
        }

        [HttpPost]
        public ActionResult EditNoti(int? Id)
        {
            try
            {




                //#region goi API VHCAL



                //string uri = "https://acd-api.vht.com.vn/rest/softphones/login";

                //var client = new HttpClient();
                //client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                //client.DefaultRequestHeaders.Add("AppPlatform", "Web");
                //client.DefaultRequestHeaders.Add("AppName", "vcall");
                //client.DefaultRequestHeaders.Add("AppVersion", "1.0");
                //var values = new Dictionary<string, string>()
                //    {
                //        {"username", "VPBX_CTTTH_319"},
                //        {"password", "Trangha123"},
                //    };
                //var content = new FormUrlEncodedContent(values);
                //using (var response = client.PostAsync(uri, content).Result)
                //{
                //    string responseString = response.Content.ReadAsStringAsync().Result;
                //    Newtonsoft.Json.Linq.JObject json = Newtonsoft.Json.Linq.JObject.Parse(responseString);
                //    Console.WriteLine(response.StatusCode);
                //}


                ////begin hàm lấy thông tin cuộc gọi là get

                //uri = "https://acd-api.vht.com.vn/rest/cdrs";
                //client.DefaultRequestHeaders.Authorization =
                //    new AuthenticationHeaderValue(
                //        "Basic",
                //    Convert.ToBase64String(
                //        System.Text.ASCIIEncoding.ASCII.GetBytes(
                //            string.Format("{0}:{1}", "d5eeb9e6d80dfdeedb82f7d9b6806f00", "271645f369ad983266fe0b251d826257"))));


                ////string strurl= "https://acd-api.vht.com.vn/rest/cdrs?page=100&limit=40&sort_by=cdr_id&sort_type=ASC&state=200&direction=3&extension=1055&from_number=0899179004&to_number=090&date_started=" + ConvertToTimestamp(DateTime.Now.AddDays(-90)) + "&date_ended=" + ConvertToTimestamp(DateTime.Now.AddDays(20));

                //string strurl = "https://acd-api.vht.com.vn/rest/cdrs?page=10&limit=40&sort_by=acd_cdr_id&sort_type=ASC";


                //using (var response = client.GetAsync(string.Format(strurl)).Result)
                //{
                //    string responseString = response.Content.ReadAsStringAsync().Result;
                //    Newtonsoft.Json.Linq.JObject json = Newtonsoft.Json.Linq.JObject.Parse(responseString);
                //    Console.WriteLine(response.StatusCode);
                //}


                //return Content("Không có nhân viên đề gửi thông báo");





                ////response.EnsureSuccessStatusCode();






                ////using (var client = new HttpClient())
                ////{
                ////    // Initialize HTTP client
                ////    client.BaseAddress = new Uri("https://acd-api.vht.com.vn/rest/softphones/login", UriKind.Absolute);
                ////    client.Timeout = TimeSpan.FromSeconds(50);
                ////    client.DefaultRequestHeaders.Accept.Clear();
                ////    //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Integration", "bmcEpOm0=");
                ////    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                ////    // Build session data to send
                ////    //var values = new List<KeyValuePair<string, string>>();

                ////    //values.Add(new KeyValuePair<string, string>("StartDate", "01/01/2019"));
                ////    //values.Add(new KeyValuePair<string, string>("EndDate", "30/01/2019"));
                ////    //values.Add(new KeyValuePair<string, string>("UserId", "7765"));

                ////    // Send session data via POST using form-url-encoded content

                ////    using (var response = client.GetAsync(string.Format("crm.contact.list?FILTER[PHONE]=0947909294&auth=32a6525c003534b80031ffbe000002080000035b69e3441686bd63adb1a249df67447d&select[]=PHONE&select[]=NAME&select[]=LAST_NAME")).Result)
                ////    {
                ////        string responseString = response.Content.ReadAsStringAsync().Result;
                ////        Newtonsoft.Json.Linq.JObject json = Newtonsoft.Json.Linq.JObject.Parse(responseString);
                ////        Console.WriteLine(response.StatusCode);
                ////    }

                ////}
                ////return Content("Không có nhân viên đề gửi thông báo");

                //#endregion



















                //#region goi API


                //using (var client = new HttpClient())
                //{
                //    // Initialize HTTP client
                //    client.BaseAddress = new Uri("https://intranet.trangha.com.vn/rest/", UriKind.Absolute);
                //    client.Timeout = TimeSpan.FromSeconds(50);
                //    client.DefaultRequestHeaders.Accept.Clear();
                //    //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Integration", "bmcEpOm0=");
                //    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                //    // Build session data to send
                //    //var values = new List<KeyValuePair<string, string>>();

                //    //values.Add(new KeyValuePair<string, string>("StartDate", "01/01/2019"));
                //    //values.Add(new KeyValuePair<string, string>("EndDate", "30/01/2019"));
                //    //values.Add(new KeyValuePair<string, string>("UserId", "7765"));

                //    // Send session data via POST using form-url-encoded content


                //    client.DefaultRequestHeaders.Authorization =
                //        new AuthenticationHeaderValue(
                //            "Basic",
                //            Convert.ToBase64String(
                //                System.Text.ASCIIEncoding.ASCII.GetBytes(
                //                    string.Format("{0}:{1}", "nhom1q5", "demo@123"))));


                //    //client. = new NetworkCredential("DDSServices", "jCole2011");

                //    using (var response = client.GetAsync("https://intranet.trangha.com.vn/oauth/token/?grant_type=refresh_token&client_id=local.5c52681b619942.47504244&client_secret=x60zSfvIjjF2r6WXLcnzDiJlhS5t82SjeaU7IWMSE2CZoBzLVR&refresh_token=69aba65c003534b80031ffbe000002080000033f2e12f22f2c9f0ff37708aabb25bf09&scope=granted_permission&redirect_uri=app_URL").Result)
                //    {
                //        string responseString = response.Content.ReadAsStringAsync().Result;
                //        Newtonsoft.Json.Linq.JObject json = Newtonsoft.Json.Linq.JObject.Parse(responseString);
                //        Console.WriteLine(response.StatusCode);
                //    }

                //}
                //return Content("Không có nhân viên đề gửi thông báo");

                //#endregion


                #region Gui thong bao 1


                var staff_made = staffMadeRepository.GetAllStaffMade().Where(x => x.SchedulingHistoryId == Id && x.Status == "pending").ToList();
                var user = userRepository.GetAllvwUsers().ToList();
                var user_online = user.Where(id1 => staff_made.Any(id2 => id2.UserId == id1.Id)).ToList();

                Dictionary<string, string> dictionary = new Dictionary<string, string>();
                var IncludePlayerIds = new List<string>();
                string pPlayerIds = "";
                if ((user_online != null) && (user_online.Count > 0))
                {
                    foreach (var item in user_online)
                    {
                        if ((item.PlayerId != null) && (item.PlayerId.Length > 0))
                        {
                            IncludePlayerIds.Add(item.PlayerId);
                        }
                    }
                }
                if (Erp.BackOffice.Helpers.Common.CurrentUser.PlayerId_web.Trim() != "" || Erp.BackOffice.Helpers.Common.CurrentUser.PlayerId_web.Trim() != "Chưa có")
                {
                    //IncludePlayerIds.Add(Erp.BackOffice.Helpers.Common.CurrentUser.PlayerId_web);
                }
                //IncludePlayerIds.Add("93f16a25-c9c5-42d9-a6d5-6b1fd8d075ce");


                if (IncludePlayerIds.Count > 0)
                {


                    var client = new OneSignalClient("ZTA4ZGMyOTctZjU0ZS00Y2ExLWI4MTEtYWY5YzNiYjg0OWU0"); // Nhập API Serect Key

                    var options = new NotificationCreateOptions_new();


                    options.AppId = new Guid("872eae43-21c1-44f7-80c9-ab594b66bbb6"); // Nhập My AppID của bạn
                    //options.IncludedSegments = new List<string> { "All" };
                    options.DeliverToChromeWeb = true;
                    options.DeliverToAndroid = true;
                    options.DeliverToIos = true;
                    options.DeliverToFirefox = true;

                    options.Headings.Add(LanguageCodes.English, "Thông báo khách hàng mới");
                    options.Contents.Add(LanguageCodes.English, "Bạn nhận được một yêu cầu chăm sóc da từ lễ tân, vui lòng vào xác nhận!");


                    //options.Url = txtLaunchURL.Text;
                    //options.ChromeWebImage = txtImageChrome.Text;
                    dictionary.Add("IdXeplich", Id.ToString());
                    options.Data = dictionary;
                    options.IncludePlayerIds = IncludePlayerIds;
                    var result = client.Notifications.Create(options);
                    if (result.Recipients < 0)
                    {
                        return Json(0);
                    } 
                    return Json(1); 
                        //Content("Gửi thành công thông báo cho nhân viên liên quan");
                }
                else
                {
                    return Json(0); 
                        //Content("Không có nhân viên đề gửi thông báo");
                }

                return Json(0); 
                    //Content("Không có nhân viên đề gửi thông báo");
                #endregion

            }
            catch (DbUpdateException)
            {
                return Content("error");
            }
        }



        [HttpPost]
        public ActionResult Edit(SchedulingHistoryViewModel model, bool? IsPopup)
        {
            Bed updateTrangthai = new Bed();
            updateTrangthai = bedRepository.GetBedById(model.BedId.Value);
            //Xóa nhân viên đã chọn
            var StaffMadeList = staffMadeRepository.GetAllStaffMade().Where(x => x.SchedulingHistoryId == model.Id).ToList();
            foreach (var i in StaffMadeList)
            {
                i.IsDeleted = true;
                staffMadeRepository.UpdateStaffMade(i);
            }

            updateTrangthai.Trang_Thai = true;
            bedRepository.UpdateBed(updateTrangthai);
            Room isroom = new Room();
            isroom = roomRepository.GetRoomById(updateTrangthai.Room_Id);

            //Lấy id phòng
            model.RoomId = updateTrangthai.Room_Id;
            //var errors = ModelState.Values.SelectMany(v => v.Errors);
            //if (ModelState.IsValid)
            //{
            if (Request["Submit"] == "Save")
            {
                var SchedulingHistory = SchedulingHistoryRepository.GetSchedulingHistoryById(model.Id);
                AutoMapper.Mapper.Map(model, SchedulingHistory);
                SchedulingHistory.ModifiedUserId = WebSecurity.CurrentUserId;
                SchedulingHistory.ModifiedDate = DateTime.Now;

                SchedulingHistory.RoomId = updateTrangthai.Room_Id;
                SchedulingHistory.Name_Bed = updateTrangthai.Name_Bed;
                SchedulingHistory.Name_Room = isroom.Name;
                SchedulingHistory.NameNV = "";
                //string starttime = SchedulingHistory.WorkDay.ToString();
                //string[] timestarts = starttime.Split(' ');
                //SchedulingHistory.startTime = timestarts[1];

                //danh sách nhân viên thực hiện
                List<string> ListUser = new List<string>();
                if (Request["user_check"] != null)
                {
                    ListUser = Request["user_check"].Split(',').ToList();
                    for (int i = 0; i < ListUser.Count(); i++)
                    {
                        var add = new StaffMade();
                        add.UserId = Convert.ToInt32(ListUser[i].ToString());
                        add.IsDeleted = false;
                        add.CreatedUserId = WebSecurity.CurrentUserId;
                        add.ModifiedUserId = WebSecurity.CurrentUserId;
                        add.AssignedUserId = WebSecurity.CurrentUserId;
                        add.CreatedDate = DateTime.Now;
                        add.ModifiedDate = DateTime.Now;
                        add.SchedulingHistoryId = SchedulingHistory.Id;
                        add.Status = "pending";
                        staffMadeRepository.InsertStaffMade(add);

                        //lay ten nhan vien
                        int idnv = Convert.ToInt32(ListUser[i].ToString());
                        var NV = userRepository.GetUserById(idnv);
                        if (i == 0)
                        {
                            SchedulingHistory.NameNV = SchedulingHistory.NameNV + NV.FullName;
                        }
                        else
                        {
                            SchedulingHistory.NameNV = SchedulingHistory.NameNV + " + " + NV.FullName;
                        }

                    }
                }
                SchedulingHistoryRepository.UpdateSchedulingHistory(SchedulingHistory);
                //ghi log 
                Erp.BackOffice.Controllers.HomeController.WriteLog(SchedulingHistory.Id, SchedulingHistory.Code, "đã cập nhật phiếu xếp lịch", "SchedulingHistory/Detail/" + SchedulingHistory.Id, Helpers.Common.CurrentUser.BranchId.Value);

                // danh sách trang thiết bị thực hiện
                var _listdata = logEquipmentRepository.GetAllLogEquipment().Where(x => x.SchedulingHistoryId == SchedulingHistory.Id).ToList();
                if (model.EquipmentList.Any(x => x.Id == 0))
                {
                    //lưu danh sách thao tác thực hiện dịch vụ
                    foreach (var item in model.EquipmentList.Where(x => x.Id == 0 && x.EquipmentId > 0))
                    {
                        var add = new Domain.Sale.Entities.LogEquipment();
                        add.IsDeleted = false;
                        add.CreatedUserId = WebSecurity.CurrentUserId;
                        add.ModifiedUserId = WebSecurity.CurrentUserId;
                        add.CreatedDate = DateTime.Now;
                        add.ModifiedDate = DateTime.Now;
                        add.EquipmentId = item.EquipmentId;
                        add.Status = "pending";
                        add.SchedulingHistoryId = SchedulingHistory.Id;
                        logEquipmentRepository.InsertLogEquipment(add);

                    }
                }
                var _delete = _listdata.Where(id1 => !model.EquipmentList.ToList().Any(id2 => id2.Id == id1.Id)).ToList();
                if (_delete.Any())
                {
                    foreach (var item in _delete)
                    {
                        logEquipmentRepository.DeleteLogEquipment(item.Id);
                    }
                }
                if (model.EquipmentList.Any(x => x.Id > 0))
                {
                    var update = _listdata.Where(id1 => model.EquipmentList.Where(x => x.Id > 0 && x.EquipmentId > 0).ToList().Any(id2 => id2.Id == id1.Id)).ToList();
                    if (update.Any())
                    {
                        //lưu danh sách thao tác thực hiện dịch vụ
                        foreach (var item in model.EquipmentList.Where(x => x.Id > 0 && x.EquipmentId > 0))
                        {
                            var _update = update.FirstOrDefault(x => x.Id == item.Id);
                            _update.EquipmentId = item.EquipmentId;
                            //  _update.Note = item.Note;
                            logEquipmentRepository.UpdateLogEquipment(_update);
                        }
                    }
                }
                if (IsPopup == true)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData[Globals.SuccessMessageKey] = App_GlobalResources.Wording.UpdateSuccess;
                    return RedirectToAction("Index");
                }
            }

            return View(model);
            //}

            //return View(model);

            //if (Request.UrlReferrer != null)
            //    return Redirect(Request.UrlReferrer.AbsoluteUri);
            //return RedirectToAction("Index");
        }

        #endregion

        #region Detail
        public ActionResult Detail(int? Id)
        {
            var SchedulingHistory = SchedulingHistoryRepository.GetvwSchedulingHistoryById(Id.Value);
            if (SchedulingHistory != null && SchedulingHistory.IsDeleted != true)
            {
                var model = new SchedulingHistoryViewModel();
                AutoMapper.Mapper.Map(SchedulingHistory, model);
                model.StaffMadeList = new List<StaffMadeViewModel>();
                model.StaffMadeList = staffMadeRepository.GetvwAllStaffMade().Where(x => x.SchedulingHistoryId == Id).Select(x => new StaffMadeViewModel
                {
                    Id = x.Id,
                    Note = x.Note,
                    UserId = x.UserId,
                    Status = x.Status,
                    SchedulingHistoryId = x.SchedulingHistoryId,
                    ProfileImage = x.ProfileImage,
                    UserCode = x.UserCode,
                    FullName = x.FullName,
                    ModifiedDate = x.ModifiedDate
                }).ToList();
                model.EquipmentList = new List<LogEquipmentViewModel>();
                model.EquipmentList = logEquipmentRepository.GetvwAllLogEquipment().Where(x => x.SchedulingHistoryId == Id).Select(x => new LogEquipmentViewModel
                {
                    Id = x.Id,
                    EquipmentId = x.EquipmentId,
                    InspectionDate = x.InspectionDate,
                    Note = x.Note,
                    StaffEquipmentCode = x.StaffEquipmentCode,
                    SchedulingHistoryId = x.SchedulingHistoryId,
                    StaffEquipmentName = x.StaffEquipmentName,
                    Status = x.Status,
                    EquimentGroup = x.EquimentGroup
                }).ToList();
                return View(model);
            }
            if (Request.UrlReferrer != null)
                return Redirect(Request.UrlReferrer.AbsoluteUri);
            ViewBag.AlertMessage2 = TempData["AlertMessage2"];
            return RedirectToAction("Index");
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
                    var item = SchedulingHistoryRepository.GetSchedulingHistoryById(int.Parse(id, CultureInfo.InvariantCulture));
                    if (item != null)
                    {
                        //if (item.CreatedUserId != Helpers.Common.CurrentUser.Id && Helpers.Common.CurrentUser.UserTypeId != 1)
                        //{
                        //    TempData["FailedMessage"] = "NotOwner";
                        //    return RedirectToAction("Index");
                        //}

                        item.IsDeleted = true;
                        SchedulingHistoryRepository.UpdateSchedulingHistory(item);

                        //ghi log 
                        Erp.BackOffice.Controllers.HomeController.WriteLog(item.Id, item.Code, "đã xóa phiếu xếp lịch", "SchedulingHistory/Detail/" + item.Id, Helpers.Common.CurrentUser.BranchId.Value);

                    }
                }
                else
                {
                    string idDeleteAll = Request["DeleteId-checkbox"];
                    string[] arrDeleteId = idDeleteAll.Split(',');
                    for (int i = 0; i < arrDeleteId.Count(); i++)
                    {
                        var item = SchedulingHistoryRepository.GetSchedulingHistoryById(int.Parse(arrDeleteId[i], CultureInfo.InvariantCulture));
                        if (item != null)
                        {
                            //if (item.CreatedUserId != Helpers.Common.CurrentUser.Id && Helpers.Common.CurrentUser.UserTypeId != 1)
                            //{
                            //    TempData["FailedMessage"] = "NotOwner";
                            //    return RedirectToAction("Index");
                            //}

                            item.IsDeleted = true;
                            SchedulingHistoryRepository.UpdateSchedulingHistory(item);
                            //ghi log 
                            Erp.BackOffice.Controllers.HomeController.WriteLog(item.Id, item.Code, "đã xóa phiếu xếp lịch", "SchedulingHistory/Detail/" + item.Id, Helpers.Common.CurrentUser.BranchId.Value);

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

        [HttpPost]
        public JsonResult DeleteById(string Id)
        {
            try
            {
                var item = SchedulingHistoryRepository.GetSchedulingHistoryById(int.Parse(Id, CultureInfo.InvariantCulture));

                if (item != null)
                {
                    if (item.BedId.HasValue)
                    {
                        Bed updateTrangthai = new Bed();
                        updateTrangthai = bedRepository.GetBedById(item.BedId.Value);
                        updateTrangthai.Trang_Thai = false;
                        bedRepository.UpdateBed(updateTrangthai);
                    }

                    //if (item.CreatedUserId != Helpers.Common.CurrentUser.Id && Helpers.Common.CurrentUser.UserTypeId != 1)
                    //{
                    //    TempData["FailedMessage"] = "NotOwner";
                    //    return RedirectToAction("Index");
                    //}
                    item.ModifiedUserId = Helpers.Common.CurrentUser.UserTypeId;
                    item.IsDeleted = true;
                    SchedulingHistoryRepository.UpdateSchedulingHistory(item);
                }
                TempData[Globals.SuccessMessageKey] = App_GlobalResources.Wording.DeleteSuccess;
                return Json(item, JsonRequestBehavior.AllowGet);
            }
            catch (DbUpdateException)
            {
                TempData[Globals.FailedMessageKey] = App_GlobalResources.Error.RelationError;
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion


        #region Cancel  lễ tân duyệt nhân viên không tham gia,
        public ActionResult Cancel(int? Id)
        {
            var data = staffMadeRepository.GetvwStaffMadeById(Id.Value);
            if (data != null && data.IsDeleted != true)
            {
                var model = new StaffMadeViewModel();
                AutoMapper.Mapper.Map(data, model);

                return View(model);
            }
            if (Request.UrlReferrer != null)
                return Redirect(Request.UrlReferrer.AbsoluteUri);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult Cancel(StaffMadeViewModel model, bool? IsPopup)
        {
            if (ModelState.IsValid)
            {
                if (Request["Submit"] == "Save")
                {
                    var data = staffMadeRepository.GetStaffMadeById(model.Id);
                    //  AutoMapper.Mapper.Map(model, data);
                    data.Note = model.Note;
                    data.ModifiedUserId = WebSecurity.CurrentUserId;
                    data.ModifiedDate = DateTime.Now;
                    data.Status = "cancel";
                    staffMadeRepository.UpdateStaffMade(data);

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

        #region Approval nhân viên tham gia thực hiện
        [HttpPost]
        public ActionResult Approval(int id)
        {
            var time_now = DateTime.Now.ToShortDateString();
            try
            {
                var item = staffMadeRepository.GetStaffMadeById(id);
                var check = checkInOutRepository.GetAllvwCheckInOut().Where(x => x.UserId == item.UserId).ToList();
                // kiểm tra có tan ca hay chưa (trong ngày hôm nay)
                var checkout = check.Where(x => x.TimeType == "Out" && x.TimeDate.Value.ToShortDateString() == time_now).ToList();
                if (item != null && checkout.Count == 0)
                {
                    item.Status = "accept";
                    item.ModifiedUserId = WebSecurity.CurrentUserId;
                    item.ModifiedDate = DateTime.Now;
                    staffMadeRepository.UpdateStaffMade(item);
                    return Content("success");
                }
                else
                {
                    return JavaScript("alert('Nhân viên đã tan ca! Xác nhận không thành công.')");
                }

            }
            catch (DbUpdateException)
            {
                return Content("error");
            }
        }
        #endregion

        #region  ChangeStatus
        [HttpPost]
        public JsonResult ChangeStatus(int id, string status, string note, string moretime)
        {

            string tennv = "null";

            if (moretime != null && moretime != "" && moretime != "undefined")
            {
                var SchedulingHistory2 = SchedulingHistoryRepository.GetSchedulingHistoryById(id);
                SchedulingHistory2.moretime = int.Parse(moretime);
                SchedulingHistoryRepository.UpdateSchedulingHistory(SchedulingHistory2);

            }
            var SchedulingHistory = SchedulingHistoryRepository.GetSchedulingHistoryById(id);
            if (SchedulingHistory.IdNV.HasValue)
            {
                int idnv = SchedulingHistory.IdNV.Value;
                var NV = userRepository.GetUserById(idnv);
                tennv = NV.FullName;




            }
            var model = new SchedulingHistoryViewModel();

            var pathimg = Helpers.Common.GetSetting("Customer");
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.Snapshot }))
            {

                if (SchedulingHistory != null)
                {
                    SchedulingHistory.Status = status;
                    if (status == "inprogress")
                    {
                        if (moretime != null && moretime != "" && moretime != "undefined")
                        {

                        }
                        else
                        {
                            SchedulingHistory.WorkDay = DateTime.Now;
                            string starttime = SchedulingHistory.WorkDay.ToString();
                            string[] timestarts = starttime.Split(' ');
                            SchedulingHistory.startTime = timestarts[1];
                        }


                    }

                    if (SchedulingHistory.startTime ==null)
                    {
                        SchedulingHistory.WorkDay = DateTime.Now;
                        string starttime = SchedulingHistory.WorkDay.ToString();
                        string[] timestarts = starttime.Split(' ');
                        SchedulingHistory.startTime = timestarts[1];
                    }

                    if (status == "complete")
                    {
                        //cap nhat lai trang thai thiet bi
                        var listEquipment = logEquipmentRepository.GetListAllLogEquipment().Where(x => x.SchedulingHistoryId == id);
                        foreach (var item in listEquipment)
                        {
                            var equipment = staffEquipmentRepository.GetStaffEquipmentById(item.EquipmentId.Value);
                            equipment.StatusStaffMade = false;
                            staffEquipmentRepository.UpdateEquipment(equipment);
                        }
                        //cap nhat lai trang thai giuong
                        Bed thisbed = bedRepository.GetBedById(SchedulingHistory.BedId.Value);
                        thisbed.Trang_Thai = false;
                        bedRepository.UpdateBed(thisbed);
                        SchedulingHistory.EndDate = DateTime.Now;
                        string endtime = SchedulingHistory.EndDate.ToString();
                        string[] timeends = endtime.Split(' ');
                        SchedulingHistory.endTime = timeends[1];
                        //mcuong.fit----
                        //tính tg nhân viên thực hiện----
                        //lấy h,m,s lúc start và end
                        string[] hmsStarts = SchedulingHistory.startTime.Split(':');
                        string[] hmsEnds = SchedulingHistory.endTime.Split(':');
                        //ep kiểu
                        int hstart = int.Parse(hmsStarts[0]);
                        int mstart = int.Parse(hmsStarts[1]);
                        int sstart = int.Parse(hmsStarts[2]);
                        int hend = int.Parse(hmsEnds[0]);
                        int mend = int.Parse(hmsEnds[1]);
                        int send = int.Parse(hmsEnds[2]);
                        //tính toán tg thực hiện
                        int mExecution = 0;
                        if (hend >= hstart)
                        {
                            mExecution = (hend - hstart) * 60 + (mend - mstart);
                        }
                        else
                        {
                            hend = hend + 12;
                            mExecution = (hend - hstart) * 60 + (mend - mstart);
                        }
                        SchedulingHistory.TimeExecution = mExecution.ToString();
                    }
                    SchedulingHistory.Note = SchedulingHistory.Note + note;
                    SchedulingHistoryRepository.UpdateSchedulingHistory(SchedulingHistory);

                    ///lấy ra để hiển thị ở ô room
                    var data = SchedulingHistoryRepository.GetvwSchedulingHistoryById(id);
                    AutoMapper.Mapper.Map(data, model);
                    model.NameNV = data.NameNV;
                    model.Name_Bed = data.Name_Bed;
                    model.Name_Room = data.Name_Room;
                    model.CustomerName = data.CustomerCode + "(" + data.CustomerName + ")";
                    //begin hoapd them vao de updae membership
                    if (status == "complete")
                    {
                        if (data.TargetModule == "Membership")
                        {
                            var info_old = MembershipRepository.GetMembershipById(data.TargetId.Value);
                            if (info_old != null)
                            {
                                info_old.Status = "complete";
                                info_old.ModifiedUserId = WebSecurity.CurrentUserId;
                                info_old.ModifiedDate = DateTime.Now;
                                MembershipRepository.UpdateMembership(info_old);
                            }
                        }
                    }

                    //end hoapd them vao de update membership


                    if (status == "inprogress")
                    {
                        if (moretime != null && moretime != "" && moretime != "undefined")
                        {

                            model.strEndDate = SchedulingHistory.WorkDay.Value.AddMinutes((SchedulingHistory.TotalMinute + SchedulingHistory.moretime).Value).ToString("yyyy/MM/dd HH:mm:ss");
                        }
                        else
                        {
                            model.strEndDate = SchedulingHistory.WorkDay.Value.AddMinutes(SchedulingHistory.TotalMinute.Value).ToString("yyyy/MM/dd HH:mm:ss");
                        }

                    }
                    else
                    {
                        model.strEndDate = SchedulingHistory.EndDate.Value.ToString("yyyy/MM/dd HH:mm:ss");
                    }
                    switch (model.Status)
                    {
                        case "pending":
                            {
                                model.ColorStatus = "label-info";
                                model.strStatus = Erp.BackOffice.App_GlobalResources.Wording.pending;
                                break;
                            }
                        case "complete":
                            {
                                model.ColorStatus = "label-success";
                                model.strStatus = Erp.BackOffice.App_GlobalResources.Wording.Status_Completed;
                                break;
                            }

                        case "expired":
                            {
                                model.ColorStatus = "label-danger";
                                model.strStatus = Erp.BackOffice.App_GlobalResources.Wording.expired;
                                break;
                            }
                        case "inprogress":
                            {
                                if (moretime != null && moretime != "" && moretime != "undefined")
                                {
                                    model.ColorStatus = "label-danger";
                                    model.strStatus = Erp.BackOffice.App_GlobalResources.Wording.inprogresss;
                                    break;

                                }
                                else
                                {
                                    model.ColorStatus = "label-warning";
                                    model.strStatus = Erp.BackOffice.App_GlobalResources.Wording.inprogresss;
                                    break;
                                }
                            }
                            model.CustomerImagePath = Erp.BackOffice.Helpers.Common.KiemTraTonTaiHinhAnhHoapd(model.CustomerImage, "Customer", "user",pathimg);
                    }
                    scope.Complete();
                    return Json(model, JsonRequestBehavior.AllowGet);
                }

            }
            return Json(model, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region  LoadData
        [HttpPost]
        public JsonResult LoadData()
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

            var time_now = DateTime.Now;
            var model = new List<SchedulingHistoryViewModel>();
            model =
                SqlHelper.QuerySP<SchedulingHistoryViewModel>("TOIUUCSKH_AllSchedulingHistory", new
                {
                    @pBranchId = intBrandID,
                    @ptimenow = time_now,
                    @ptimenowstr = time_now.ToString("dd/MM/yyyy")
                }).ToList();
                //SchedulingHistoryRepository.GetvwAllSchedulingHistory().AsEnumerable().Where(x =>
                //x.BranchId == intBrandID
                //&& x.Status == "inprogress"
                //&& x.WorkDay != null && x.WorkDay < time_now
                //&& x.WorkDaystr != "" && x.WorkDaystr == time_now.ToString("dd/MM/yyyy")
                ////&& x.WorkDay.Value.AddMinutes(x.TotalMinute.Value) >= time_now
                //).Select(x => new SchedulingHistoryViewModel
                //{
                //    Name_Room = x.Name_Room,
                //    Name_Bed = x.Name_Bed,
                //    NameNV = x.NameNV,
                //    CustomerImage = x.CustomerImage,
                //    CustomerName = x.CustomerCode + "(" + x.CustomerName + ")",
                //    WorkDay = x.WorkDay,
                //    EndDate = x.EndDate,
                //    TotalMinute = x.TotalMinute,
                //    Id = x.Id,
                //    RoomId = x.RoomId,
                //    BedId = x.BedId,
                //    Status = x.Status,
                //    ExpectedWorkDay = x.ExpectedWorkDay,
                //    ExpectedEndDate = x.ExpectedEndDate,
                //    moretime = x.moretime
                //}).ToList();

			if (model != null)
            {
                var pathimg = Helpers.Common.GetSetting("Customer");
                foreach (var item in model)
                {
                    switch (item.Status)
                    {
                        case "pending":
                            {
                                item.ColorStatus = "label-info";
                                item.strStatus = Erp.BackOffice.App_GlobalResources.Wording.pending;
                                break;
                            }
                        case "complete":
                            {
                                item.ColorStatus = "label-success";
                                item.strStatus = Erp.BackOffice.App_GlobalResources.Wording.Status_Completed;
                                break;
                            }

                        case "expired":
                            {
                                item.ColorStatus = "label-danger";
                                item.strStatus = Erp.BackOffice.App_GlobalResources.Wording.expired;
                                break;
                            }
                        case "inprogress":
                            {
                                if (item.moretime != null)
                                {
                                    item.ColorBed = "color:red";
                                    item.ColorStatus = "label-danger";
                                    item.strStatus = Erp.BackOffice.App_GlobalResources.Wording.inprogresss;
                                    break;

                                }
                                else
                                {
                                    item.ColorBed = "color:red";
                                    item.ColorStatus = "label-warning";
                                    item.strStatus = Erp.BackOffice.App_GlobalResources.Wording.inprogresss;
                                    break;
                                }

                            }
                    }
                    if (item.Status == "inprogress")
                    {
                        TimeSpan interval = DateTime.Now.Subtract(item.WorkDay.Value);
                        var _time = (item.TotalMinute.Value * 60) - interval.TotalSeconds;
                        if (item.moretime != null || item.moretime > 0)
                        {
                            _time = ((item.TotalMinute.Value * 60) + (item.moretime.Value * 60)) - interval.TotalSeconds;
                        }

                        var day = DateTime.Now.AddSeconds(_time);
                        item.strEndDate = day.ToString("yyyy/MM/dd HH:mm:ss");
                    }
                    else
                    {
                        item.strEndDate = item.ExpectedEndDate.Value.ToString("yyyy/MM/dd HH:mm:ss");
                    }
                    item.CustomerImagePath = Erp.BackOffice.Helpers.Common.KiemTraTonTaiHinhAnhHoapd(item.CustomerImage, "Customer", "user",pathimg);
                }
                //nếu không có giường nào đang hoat động reset lại tất cả các giường,thiết bị thành trống
                var BranchId = intBrandID;
                var time_nows = DateTime.Now.ToShortDateString();
                var InquiryCardLists =
                    SqlHelper.QuerySP<vwInquiryCard>("TOIUUCSKH_AllInquiryCard").ToList();
                    /*inquiryCardRepository.GetvwAllInquiryCard().Where(x =>  x.WorkDay.Value.ToShortDateString() == time_nows && (x.Status == "pending")).ToList();*/
                var SchedulingLists = SqlHelper.QuerySP<vwSchedulingHistory>("TOIUUCSKH_SchedulingLists").ToList();/* SchedulingHistoryRepository.GetvwAllSchedulingHistory().AsEnumerable().Where(x =>  x.ExpectedWorkDay.Value.ToShortDateString() == time_nows && x.Status == "pending").ToList();*/
				//x.BranchId == BranchId && bo bat theo chi nhanh
				if (model.Count == 0 && SchedulingLists.Count == 0 && InquiryCardLists.Count == 0)
                {
                    List<Bed> arr = bedRepository.GetAllBed().Where(x => x.Trang_Thai == true).ToList();
                    foreach (var item in arr)
                    {
                        item.Trang_Thai = false;
                        bedRepository.UpdateBed(item);
                    }
                    var StaffEquipmentLists = staffEquipmentRepository.GetListAllStaffEquipment();
                    foreach (var k in StaffEquipmentLists)
                    {
                        k.StatusStaffMade = false;
                        staffEquipmentRepository.UpdateEquipment(k);
                    }
                }
                return Json(model, JsonRequestBehavior.AllowGet);
            }


            return Json(model, JsonRequestBehavior.AllowGet);
        }
        #endregion

        [AllowAnonymous]
        public JsonResult GetListStaffEquipment(int id)
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

            var branchid = intBrandID;
            var inquiryCard = inquiryCardRepository.GetvwInquiryCardById(id);
            if (inquiryCard != null)
            {
                var q = staffEquipmentRepository.GetAllStaffEquipment().AsEnumerable().Where(item => item.BranchId == branchid && ("," + inquiryCard.EquimentGroup + ",").Contains("," + item.Group + ",") == true)
                    .Select(item => new
                    {
                        item.Id,
                        item.Name,
                        item.Code,
                        item.InspectionDate,
                        item.Group
                    })
                    .OrderBy(item => item.Name)
                    .ToList();
                return Json(q.Select(item => new { item.Id, Code = item.Code, Text = item.Code + " - " + item.Name, Name = item.Name, Note = (item.InspectionDate.HasValue ? item.InspectionDate.Value.ToString("dd/MM/yyyy") : ""), Value = item.Id }), JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }

        #region  CheckInOut
        [HttpPost]
        public JsonResult CheckInOut(int id, string status)
        {
            try
            {
                var CheckInOut = new CheckInOut();
                CheckInOut.CreatedDate = DateTime.Now;
                CheckInOut.TimeStr = DateTime.Now;
                CheckInOut.TimeDate = DateTime.Now;
                CheckInOut.UserId = id;
                CheckInOut.TimeType = status;
                checkInOutRepository.InsertCheckInOut(CheckInOut);
                var aa = new { strTime = CheckInOut.TimeDate.Value.ToString("HH:mm"), status = "success" };
                return Json(aa, JsonRequestBehavior.AllowGet);
            }
            catch (DbUpdateException)
            {
                return Json(status = "error", JsonRequestBehavior.AllowGet);
            }

        }
        #endregion

        [AllowAnonymous]
        public JsonResult ChangeStatusStaffMade(int BranchId, string Type, string Note, string Status, int Id)
        {
            Erp.BackOffice.Hubs.ErpHub.ChangeStatusStaffMade(BranchId, Type, Note, Status, Id);
            return Json(null, JsonRequestBehavior.AllowGet);
        }


        #region Export
        public ActionResult ExportExcel(string startDate, string endDate, string txtCode, string txtCusCode,
            string txtCusName, string Status, int? BranchId)
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

            IEnumerable<SchedulingHistoryViewModel> q = SchedulingHistoryRepository.GetvwAllSchedulingHistory()
                .Select(item => new SchedulingHistoryViewModel
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
                    CreatedUserCode = item.CreatedUserCode,
                    CreatedUserName = item.CreatedUserName,
                    CustomerCode = item.CustomerCode,
                    CustomerName = item.CustomerName,
                    BranchId = item.BranchId,
                    Type = item.Type,
                    WorkDay = item.WorkDay,
                    CustomerId = item.CustomerId,
                    CustomerImage = item.CustomerImage,
                    RoomName = item.RoomName,
                    InquiryCardCode = item.InquiryCardCode,
                    InquiryCardId = item.InquiryCardId,
                    ProductName = item.ProductName,
                    ProductCode = item.ProductCode,
                    Status = item.Status,
                    strexecutionTime = item.TimeExecution,
                    NameNV = item.NameNV,
                    startTime = item.startTime,
                    endTime = item.endTime,
                    TargetModule = item.TargetModule
                    //strTotalMinute = ""+item.TotalMinute
                }).OrderByDescending(m => m.CreatedDate);

            if (!string.IsNullOrEmpty(txtCode))
            {
                txtCode = Helpers.Common.ChuyenThanhKhongDau(txtCode);
                q = q.Where(x => Helpers.Common.ChuyenThanhKhongDau(x.Code).Contains(txtCode)).ToList();
            }
            if (!string.IsNullOrEmpty(txtCusCode))
            {
                txtCusCode = Helpers.Common.ChuyenThanhKhongDau(txtCusCode);
                q = q.Where(x => Helpers.Common.ChuyenThanhKhongDau(x.CustomerCode).Contains(txtCusCode)).ToList();
            }
            //if (!string.IsNullOrEmpty(type))
            //{
            //    q = q.Where(x => x.Type == type).ToList();
            //}
            if (!string.IsNullOrEmpty(txtCusName))
            {
                txtCusName = Helpers.Common.ChuyenThanhKhongDau(txtCusName);
                q = q.Where(x => Helpers.Common.ChuyenThanhKhongDau(x.CustomerCode).Contains(txtCusName) || Helpers.Common.ChuyenThanhKhongDau(x.CustomerName).Contains(txtCusName)).ToList();
          
            } 
            DateTime d_startDate, d_endDate;
            if (DateTime.TryParseExact(startDate, "dd/MM/yyyy", new CultureInfo("vi-VN"), DateTimeStyles.None, out d_startDate))
            {
                if (DateTime.TryParseExact(endDate, "dd/MM/yyyy", new CultureInfo("vi-VN"), DateTimeStyles.None, out d_endDate))
                {
                    d_endDate = d_endDate.AddHours(23).AddMinutes(59);
                    q = q.Where(x => x.CreatedDate >= d_startDate && x.CreatedDate <= d_endDate).ToList();
                }
            }
            if (BranchId != null && BranchId.Value > 0)
            {
                q = q.Where(x => x.BranchId == BranchId).ToList();
            }

            if (!string.IsNullOrEmpty(Status))
            {
                q = q.Where(x => x.Status == Status).ToList();
            }


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
            model.Content = model.Content.Replace("{DataTable}", buildHtml(q));
            model.Content = model.Content.Replace("{System.Logo}", ImgLogo);
            model.Content = model.Content.Replace("{System.CompanyName}", company);
            model.Content = model.Content.Replace("{System.AddressCompany}", address);
            model.Content = model.Content.Replace("{System.PhoneCompany}", phone);
            model.Content = model.Content.Replace("{System.Fax}", fax);
            model.Content = model.Content.Replace("{System.BankCodeCompany}", bankcode);
            model.Content = model.Content.Replace("{System.BankNameCompany}", bankname);
            model.Content = model.Content.Replace("{PrintDate}", DateTime.Now.ToString("dd/MM/yyyy HH:mm"));
            model.Content = model.Content.Replace("{Title}", "Lịch sử xếp lịch chăm sóc");

            Response.AppendHeader("content-disposition", "attachment;filename=" + "LichSuXepLich" + DateTime.Now.ToString("yyyyMMdd") + ".xls");
            Response.Charset = "";
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.Write(model.Content);
            Response.End();


            return View(model);
        }


        string buildHtml(IEnumerable<SchedulingHistoryViewModel> data)
        {
            //Tạo table html chi tiết phiếu xuất
            string detailLists = "<table border=\"1\" class=\"invoice-detail\">\r\n";
            detailLists += "<thead>\r\n";
            detailLists += "	<tr>\r\n";
            detailLists += "		<th>STT</th>\r\n";
            detailLists += "		<th>Trạng thái</th>\r\n";
            detailLists += "		<th>Tạo từ</th>\r\n";
            detailLists += "		<th>Mã phiếu tư vấn</th>\r\n";
            detailLists += "		<th>Mã khách hàng</th>\r\n";
            detailLists += "		<th>Tên khách hàng</th>\r\n";
            detailLists += "		<th>KTV Thực Hiện chăm sóc</th>\r\n";
            detailLists += "		<th>TG Thực Hiện Thực Tế</th>\r\n";
            detailLists += "		<th>TG vào Liệu Trình</th>\r\n";
            detailLists += "		<th>TG Kết thúc Liệu Trình</th>\r\n";
            detailLists += "		<th>Dịch vụ</th>\r\n";
            detailLists += "		<th>Phiếu yêu cầu</th>\r\n";
            detailLists += "		<th>Ngày thực hiện</th>\r\n";
            detailLists += "		<th>Ngày tạo</th>\r\n";
            detailLists += "	</tr>\r\n";
            detailLists += "</thead>\r\n";
            detailLists += "<tbody>\r\n";
            var index = 1;

            foreach (var item in data)
            {
                detailLists += "<tr>\r\n"
                + "<td class=\"text-center orderNo\">" + (index++) + "</td>\r\n";
               
                switch (item.Status)
                {
                    case "pending":
                        detailLists += "<td class=\"text-left \">Khởi tạo</td>\r\n";
                        break;
                    case "complete":
                        detailLists += "<td class=\"text-left \">Hoàn thành</td>\r\n";
                        break;
                    case "expired":
                        detailLists += "<td class=\"text-left \">Hết thời gian</td>\r\n";
                        break;
                    case "inprogress":
                        detailLists += "<td class=\"text-left \">Đang thực hiện</td>\r\n";
                        break;
                }


                detailLists += "<td class=\"text-left \">" + item.TargetModule + "</td>\r\n"
                            + "<td class=\"text-left \">" + item.Code + "</td>\r\n"
                            + "<td class=\"text-left \">" + item.CustomerCode + "</td>\r\n"
                            //+ "<td class=\"text-left \">" + CommonSatic.ToCurrencyStr(item.TotalAmount, null).Replace(".", ",") + "</td>\r\n"
                            //+ "<td class=\"text-left \">" + CommonSatic.ToCurrencyStr(item.TotalCredit, null).Replace(".", ",") + "</td>\r\n"
                            //+ "<td class=\"text-left \">" + CommonSatic.ToCurrencyStr(item.TongConNo, null).Replace(".", ",") + "</td>\r\n"
                            + "<td class=\"text-left \">" + item.CustomerName + "</td>\r\n"
                            //+ "<td class=\"text-left \">" + CommonSatic.ToCurrencyStr((decimal)(item.TyleHuong) * item.TotalAmount / 100, null).Replace(".", ",") + "</td>\r\n"
                            + "<td class=\"text-left \">" + item.NameNV + "</td>\r\n"
                            + "<td class=\"text-left \">" + item.strexecutionTime + "</td>\r\n"
                            + "<td class=\"text-left \">" + item.startTime + "</td>\r\n"
                            + "<td class=\"text-left \">" + item.endTime + "</td>\r\n"
                            + "<td class=\"text-left \">" + item.ProductName + "</td>\r\n"
                            + "<td class=\"text-left \">" + item.InquiryCardCode + "</td>\r\n"
                            + "<td class=\"text-left \">" + item.WorkDay + "</td>\r\n"

                            + "<td class=\"text-left \">" + item.CreatedDate + "</td>\r\n"

                            + "</tr>\r\n";
            }
            detailLists += "</tbody>\r\n";
            detailLists += "<tfoot style=\"font-weight:bold\">\r\n";
            detailLists += "</tfoot>\r\n</table>\r\n";

            return detailLists;
        }
        #endregion
    }
}
