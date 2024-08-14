using Erp.BackOffice.Crm.Models;
using Erp.BackOffice.Hubs;
using Erp.BackOffice.Sale.Models;
using Erp.Domain.Account.Helper;
using Erp.Domain.Interfaces;
using Hangfire;
using Microsoft.AspNet.SignalR;
using Newtonsoft.Json;
using PagedList;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using static Erp.BackOffice.Crm.Controllers.LeadMeetingController;
using WebMatrix.WebData;
using static Erp.BackOffice.Sale.Controllers.AdviseCardController;
using Erp.Domain.Repositories;
using System.Runtime.ConstrainedExecution;
using clr = System.Drawing.Color;
using System.Globalization;
using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Office2010.Excel;
using DotNetOpenAuth.Messaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Calendar = System.Globalization.Calendar;
using DocumentFormat.OpenXml.Vml.Office;

namespace Erp.BackOffice.Crm.Controllers
{
    public class AppointmentController : Controller
    {

        private readonly IUserRepository userRepository;
        public AppointmentController(
             IUserRepository _user
            )
        {
            userRepository = _user;
        }
        #region Chỉ số chốt hẹn
        public ActionResult Index()
        {
            var branchs = SqlHelper.QuerySP<BranchVModel>("sp_Staff_Branch_GetBranch");
            var BoPhan = SqlHelper.QuerySP<BoPhanVModel>("spSystem_UserType_kd_Select_By_BranchId");
            var user = userRepository.GetAllUsers();

            ViewBag.user = user.Where(x => x.UserType_kd_id != null);
            ViewBag.Branchs = branchs;
            ViewBag.BoPhan = BoPhan;
            return View();
        }

        [HttpGet]
        [AllowAnonymousAttribute]
        public JsonResult GetAppointmentKeyReport(string strFromDate, string strToDate, int isLead)
        {
            List<string> labels = new List<string>();
            List<string> backgroundColor = new List<string>();
            List<double> percentages = new List<double>();
            List<int> quantities = new List<int>();
            var datals = SqlHelper.QuerySP<AppointmentViewModel>("sp_Crm_LeadMeating_Appointment_Select", new
            {
                @fromDate = strFromDate,
                @toDate = strToDate,
                @pisLead = isLead
            });
            var branch = SqlHelper.QuerySP<BranchVModel>("sp_Staff_Branch_GetBranch");
            var countAll = datals.Count();
            foreach (var britem in branch)
            {
                var rss = datals?.Where(x => x.BranchId == britem.Id);
                if (rss != null)
                {
                    var count = rss.Count();
                    if (countAll > 0)
                    {
                        double percentage = ((count * 100.0) / countAll);
                        percentage = Math.Round(percentage, 2);
                        labels.Add(britem.Name);
                        quantities.Add(count);
                        percentages.Add(percentage);
                    }
                }
                else
                {
                    labels.Add(britem.Name);
                    quantities.Add(0);
                    percentages.Add(0.0);
                }

            }

            foreach (var item in typeof(clr).GetProperties().Skip(1))
            {
                backgroundColor.Add(item.Name);
                if (backgroundColor.Count == labels.Count)
                    break;
            }
            return Json(new { labels, backgroundColor, data = percentages, quantities, datals }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Doanh số ảo
        public ActionResult SalesVirtualReportIndex()
        {
            var branchs = SqlHelper.QuerySP<BranchVModel>("sp_Staff_Branch_GetBranch");
            var BoPhan = SqlHelper.QuerySP<BoPhanVModel>("spSystem_UserType_kd_Select_By_BranchId");
            var user = userRepository.GetAllUsers();

            ViewBag.user = user.Where(x => x.UserType_kd_id != null);
            ViewBag.Branchs = branchs;
            ViewBag.BoPhan = BoPhan;
            return View();
        }
        [HttpGet]
        [AllowAnonymousAttribute]
        public JsonResult GetSalesVirtualReport(string strFromDate, string strToDate, int isLead)
        {
            List<string> labels = new List<string>();
            List<string> backgroundColor = new List<string>();
            List<double> percentages = new List<double>();
            List<long> sums = new List<long>();
            var datals = SqlHelper.QuerySP<SalesVirtualReportViewModel>("sp_Crm_LeadMeating_SalesVirtualReport_Select", new
            {
                @fromDate = strFromDate,
                @toDate = strToDate,
                @pisLead = isLead
            });
            var branch = SqlHelper.QuerySP<BranchVModel>("sp_Staff_Branch_GetBranch");
            long sumAll = datals.Sum(x => long.Parse(x.DoanhSoAo));
            foreach (var britem in branch)
            {
                var rss = datals?.Where(x => x.BranchId == britem.Id);
                if (rss != null)
                {
                    long sum = rss.Sum(x => long.Parse(x.DoanhSoAo));
                    double percentage = ((sum * 100.0) / sumAll);
                    percentage = Math.Round(percentage, 2);
                    labels.Add(britem.Name);
                    sums.Add(sum);
                    percentages.Add(percentage);
                }
                else
                {
                    labels.Add(britem.Name);
                    sums.Add(0);
                    percentages.Add(0.0);
                }

            }

            labels.Add("Tổng");
            percentages.Add(100.0);
            sums.Add(sumAll);
            foreach (var item in typeof(clr).GetProperties().Skip(1))
            {
                backgroundColor.Add(item.Name);
                if (backgroundColor.Count == labels.Count - 1)
                    break;
            }
            return Json(new { labels, backgroundColor, data = percentages, sums, datals }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Kết quả làm việc của nhân viên
        public ActionResult EmployeePerRsReportIndex()
        {
            var branchs = SqlHelper.QuerySP<BranchVModel>("sp_Staff_Branch_GetBranch");
            var BoPhan = SqlHelper.QuerySP<BoPhanVModel>("spSystem_UserType_kd_Select_By_BranchId");
            var user = userRepository.GetAllUsers();
            var Options = new List<TargetOptionModel>
        {
            new TargetOptionModel { Value = 1, Content = "Call" },
            new TargetOptionModel { Value = 2, Content = "Play" },
            new TargetOptionModel { Value = 3, Content = "Hẹn" },
            new TargetOptionModel { Value = 4, Content = "Lên" },
            new TargetOptionModel { Value = 5, Content = "Mua" },
            new TargetOptionModel { Value = 6, Content = "Doanh số ảo" }
        };
            ViewBag.user = user.Where(x => x.UserType_kd_id != null);
            ViewBag.Branchs = branchs;
            ViewBag.BoPhan = BoPhan;
            ViewBag.Options = Options;
            return View();
        }
        [HttpGet]
        [AllowAnonymousAttribute]
        public JsonResult GetEmployeePerRsReport(string strFromDate, string strToDate, int branchId, int boPhanId, int userId, int targetId,int isLead)
        {
            DateTime fromDate = DateTime.ParseExact(strFromDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            int fromDay = fromDate.Day;
            int fromMonth = fromDate.Month;
            int fromYear = fromDate.Year;
            DateTime toDate = DateTime.ParseExact(strToDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            int toDay = toDate.Day;
            int toMonth = toDate.Month;
            int toYear = toDate.Year;

            if (fromYear != toYear && fromMonth < toMonth)
            {
                return Json(new { Success = false }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                List<string> labels = new List<string>();
                List<string> totaltg = new List<string>();
                List<double> percentTotal = new List<double>();
                List<string> backgroundColor = new List<string>();
                var users = userRepository.GetAllUsers();
                users = users.Where(x => x.UserType_kd_id != null);

                var BoPhan = SqlHelper.QuerySP<BoPhanVModel>("spSystem_UserType_kd_Select_By_BranchId");
                BoPhan = BoPhan.Where(x => x.BranchId == branchId);

                List<TargetNewOfDayeModel> targetNewOfDayeList = new List<TargetNewOfDayeModel>();


                var datasp = SqlHelper.QuerySP<EmployeePerRsReportModel>("sp_EmployeePerRsReport_Select", new
                {
                    @fromDate = strFromDate,
                    @toDate = strToDate,
                    @pBranchId = branchId,
                    @pBoPhanId = boPhanId != -1 ? boPhanId : (int?)null,
                    @pUserId = userId != -1 ? userId : (int?)null,
                    @pTarget = targetId,
                    @pisLead = isLead
                });
                datasp = datasp.Where(x =>
                {
                    int totalTarget;
                    return int.TryParse(x.TotalTarget, out totalTarget) && totalTarget != 0;
                });
                long sumTotalAll = datasp.Sum(x => long.Parse(x.TotalTarget));
                if (userId != -1)
                {
                    var datatg = SqlHelper.QuerySP<TargetNewOfDayeModel>("sp_Crm_TargetNewOfDay_Select", new
                    {
                        @pYear = fromYear,
                        @pMonth = toMonth,
                        @pBranchId = branchId,
                        @pUserId = userId,
                        @pTarget = targetId
                    });
                    targetNewOfDayeList.AddRange(datatg);
                }
                else
                {
                    users = users.Where(x => boPhanId != -1 ? x.UserType_kd_id == boPhanId : true);
                    foreach (var item in users)
                    {
                        var datatg = SqlHelper.QuerySP<TargetNewOfDayeModel>("sp_Crm_TargetNewOfDay_Select", new
                        {
                            @pYear = fromYear,
                            @pMonth = toMonth,
                            @pBranchId = branchId,
                            @pUserId = item.Id,
                            @pTarget = targetId
                        });
                        targetNewOfDayeList.AddRange(datatg);

                    }
                }
                foreach (var item1 in datasp)
                {
                    decimal totalTargetNewOfDay = 0;
                    for (int i = fromDay; i <= toDay; i++)
                    {
                        string daytg = "Day" + i.ToString();
                        if (targetNewOfDayeList.Count() > 0)
                        {
                            var property = targetNewOfDayeList.FirstOrDefault(x => x.UserId == item1.AssignedUserId)?.GetType().GetProperty(daytg);

                            if (property != null)
                            {
                                totalTargetNewOfDay += (decimal)property.GetValue(targetNewOfDayeList.FirstOrDefault(x => x.UserId == item1.AssignedUserId));
                            }
                            else
                            {
                                totalTargetNewOfDay += 0;
                            }
                        }
                        else
                        {
                            totalTargetNewOfDay += 0;
                        }
                    }
                    if (totalTargetNewOfDay > 0)
                    {
                        item1.TotalTargetNewOfDay = totalTargetNewOfDay;
                        double percenDat = Math.Round(((double.Parse(item1.TotalTarget) * 100.0) / (double)totalTargetNewOfDay), 2);
                        double percenChuaDat = 100 - percenDat;
                        item1.percentDat = percenDat;
                        item1.percentChuaDat = percenChuaDat;
                    }
                    else
                    {
                        item1.percentDat = 0;
                        item1.percentChuaDat = 100;
                    }

                    item1.percentTotalTarget = Math.Round(((double.Parse(item1.TotalTarget) * 100.0) / sumTotalAll), 2);
                    labels.Add(item1.AssignedUserName);
                    totaltg.Add(item1.TotalTarget);
                    percentTotal.Add(item1.percentTotalTarget);

                }

                foreach (var item in typeof(clr).GetProperties().Skip(1))
                {
                    backgroundColor.Add(item.Name);
                    if (backgroundColor.Count == labels.Count)
                        break;
                }
                return Json(new { Success = true, labels, totaltg, datasp, percentTotal, backgroundColor }, JsonRequestBehavior.AllowGet);
            }

        }
        #endregion

        #region Chỉ số nhân viên
        public ActionResult EmployeeTitelsReportIndex()
        {
            var branchs = SqlHelper.QuerySP<BranchVModel>("sp_Staff_Branch_GetBranch");
            var BoPhan = SqlHelper.QuerySP<BoPhanVModel>("spSystem_UserType_kd_Select_By_BranchId");
            var user = userRepository.GetAllUsers();
            var Options = new List<TargetOptionModel>
        {
            new TargetOptionModel { Value = 1, Content = "Call" },
            new TargetOptionModel { Value = 2, Content = "Play" },
            new TargetOptionModel { Value = 3, Content = "Hẹn" },
            new TargetOptionModel { Value = 4, Content = "Lên" },
            new TargetOptionModel { Value = 5, Content = "Mua" },
            new TargetOptionModel { Value = 6, Content = "Doanh số ảo" }
        };
            ViewBag.user = user.Where(x => x.UserType_kd_id != null);
            ViewBag.Branchs = branchs;
            ViewBag.BoPhan = BoPhan;
            ViewBag.Options = Options;
            return View();
        }
        [HttpGet]
        [AllowAnonymousAttribute]
        public JsonResult GetEmployeeTitelsReport(string strFromDate, string strToDate, int branchId, int boPhanId, int userId, int isLead)
        {

            List<DateRange> dateRange = CalculateDateRanges(strFromDate, strToDate);
            List<string> labels = new List<string>();
            List<string> dataCall = new List<string>();
            List<string> dataPlay = new List<string>();

            var users = userRepository.GetAllUsers();
            users = users.Where(x => x.UserType_kd_id != null);

            List<TargetNewOfDayeModel> targetNewOfDayeList = new List<TargetNewOfDayeModel>();
            List<EmployeeTitleTableReportModel> dataTableReport = new List<EmployeeTitleTableReportModel>();
            List<TargetOptionModel> datasHen = new List<TargetOptionModel>();
            var datasp = SqlHelper.QuerySP<EmployeeTitleReportModel>("sp_EmployeeTitlesReport_Select", new
            {
                @fromDate = strFromDate,
                @toDate = strToDate,
                @pBranchId = branchId != -1 ? branchId : (int?)null,
                @pBoPhanId = boPhanId != -1 ? boPhanId : (int?)null,
                @pUserId = userId != -1 ? userId : (int?)null,
                @pisLead = isLead
            });
            if (userId != -1)
            {
                foreach (var itemd in dateRange)
                {
                    var datatg = SqlHelper.QuerySP<TargetNewOfDayeModel>("sp_Crm_TargetNewOfDay_Select", new
                    {
                        @pYear = itemd.Year,
                        @pMonth = itemd.Month,
                        @pBranchId = branchId != -1 ? branchId : (int?)null,
                        @pUserId = userId
                    });
                    targetNewOfDayeList.AddRange(datatg);
                }

            }
            else
            {
                users = users.Where(x => boPhanId != -1 ? x.UserType_kd_id == boPhanId : true);
                foreach (var item in users)
                {
                    foreach (var itemd in dateRange)
                    {
                        var datatg = SqlHelper.QuerySP<TargetNewOfDayeModel>("sp_Crm_TargetNewOfDay_Select", new
                        {
                            @pYear = itemd.Year,
                            @pMonth = itemd.Month,
                            @pBranchId = branchId != -1 ? branchId : (int?)null,
                            @pUserId = item.Id
                        });
                        targetNewOfDayeList.AddRange(datatg);
                    }
                }

            }
            foreach (var item1 in datasp)
            {
                decimal totalTargetCall = 0;
                decimal totalTargetPlay = 0;
                decimal totalTargetHen = 0;
                decimal totalTargetLen = 0;
                decimal totalTargetMua = 0;
                decimal totalTargetDSAo = 0;
                if (targetNewOfDayeList.Count() > 0)
                {
                    foreach (var itemd in dateRange)
                    {
                        for (int i = itemd.StartDay; i <= itemd.EndDay; i++)
                        {
                            string daytg = "Day" + i.ToString();
                            var propertyCall = targetNewOfDayeList.FirstOrDefault(x => x.UserId == item1.AssignedUserId && x.NAM == itemd.Year && x.THANG == itemd.Month && x.TypeTarget == 1)?.GetType().GetProperty(daytg);
                            totalTargetCall += propertyCall != null ? (decimal)propertyCall.GetValue(targetNewOfDayeList.FirstOrDefault(x => x.UserId == item1.AssignedUserId && x.NAM == itemd.Year && x.THANG == itemd.Month && x.TypeTarget == 1)) : 0;
                            var propertyPlay = targetNewOfDayeList.FirstOrDefault(x => x.UserId == item1.AssignedUserId && x.NAM == itemd.Year && x.THANG == itemd.Month && x.TypeTarget == 2)?.GetType().GetProperty(daytg);
                            totalTargetPlay += propertyPlay != null ? (decimal)propertyPlay.GetValue(targetNewOfDayeList.FirstOrDefault(x => x.UserId == item1.AssignedUserId && x.NAM == itemd.Year && x.THANG == itemd.Month && x.TypeTarget == 2)) : 0;
                            var propertyHen = targetNewOfDayeList.FirstOrDefault(x => x.UserId == item1.AssignedUserId && x.NAM == itemd.Year && x.THANG == itemd.Month && x.TypeTarget == 3)?.GetType().GetProperty(daytg);
                            totalTargetHen += propertyHen != null ? (decimal)propertyHen.GetValue(targetNewOfDayeList.FirstOrDefault(x => x.UserId == item1.AssignedUserId && x.NAM == itemd.Year && x.THANG == itemd.Month && x.TypeTarget == 3)) : 0;
                            var propertyLen = targetNewOfDayeList.FirstOrDefault(x => x.UserId == item1.AssignedUserId && x.NAM == itemd.Year && x.THANG == itemd.Month && x.TypeTarget == 4)?.GetType().GetProperty(daytg);
                            totalTargetLen += propertyLen != null ? (decimal)propertyLen.GetValue(targetNewOfDayeList.FirstOrDefault(x => x.UserId == item1.AssignedUserId && x.NAM == itemd.Year && x.THANG == itemd.Month && x.TypeTarget == 4)) : 0;
                            var propertyMua = targetNewOfDayeList.FirstOrDefault(x => x.UserId == item1.AssignedUserId && x.NAM == itemd.Year && x.THANG == itemd.Month && x.TypeTarget == 5)?.GetType().GetProperty(daytg);
                            totalTargetMua += propertyMua != null ? (decimal)propertyMua.GetValue(targetNewOfDayeList.FirstOrDefault(x => x.UserId == item1.AssignedUserId && x.NAM == itemd.Year && x.THANG == itemd.Month && x.TypeTarget == 5)) : 0;
                            var propertyDSAo = targetNewOfDayeList.FirstOrDefault(x => x.UserId == item1.AssignedUserId && x.NAM == itemd.Year && x.THANG == itemd.Month && x.TypeTarget == 6)?.GetType().GetProperty(daytg);
                            totalTargetDSAo += propertyDSAo != null ? (decimal)propertyDSAo.GetValue(targetNewOfDayeList.FirstOrDefault(x => x.UserId == item1.AssignedUserId && x.NAM == itemd.Year && x.THANG == itemd.Month && x.TypeTarget == 6)) : 0;
                        }
                    }
                }
                else
                {
                    totalTargetCall += 0;
                    totalTargetPlay += 0;
                    totalTargetHen += 0;
                    totalTargetLen += 0;
                    totalTargetMua += 0;
                    totalTargetDSAo += 0;
                }
                double percentcall = totalTargetCall != 0 ? Math.Round(((double.Parse(item1.TotalCall != null ? item1.TotalCall : "0") * 100.0) / (double)totalTargetCall), 2) : 0;
                double percentplay = totalTargetPlay != 0 ? Math.Round(((double.Parse(item1.TotalPlay != null ? item1.TotalPlay : "0") * 100.0) / (double)totalTargetPlay), 2) : 0;
                double percenthen = totalTargetHen != 0 ? Math.Round(((double.Parse(item1.TotalHen != null ? item1.TotalHen : "0") * 100.0) / (double)totalTargetHen), 2) : 0;
                double percentlen = totalTargetLen != 0 ? Math.Round(((double.Parse(item1.TotalLen != null ? item1.TotalLen : "0") * 100.0) / (double)totalTargetLen), 2) : 0;
                double percentmua = totalTargetMua != 0 ? Math.Round(((double.Parse(item1.TotalMua != null ? item1.TotalMua : "0") * 100.0) / (double)totalTargetMua), 2) : 0;
                double percentdsao = totalTargetDSAo != 0 ? Math.Round(((double.Parse(item1.TotalDSAo != null ? item1.TotalDSAo : "0") * 100.0) / (double)totalTargetDSAo), 2) : 0;

                if (percentcall == 0 && percentplay == 0 && percenthen == 0 && percentlen == 0 && percentmua == 0 && percentdsao == 0)
                {

                }
                else
                {
                    EmployeeTitleTableReportModel datatb = new EmployeeTitleTableReportModel
                    {
                        Id = item1.AssignedUserId,
                        Name = item1.AssignedUserName,
                        PercentCall = percentcall,
                        PercentPlay = percentplay,
                        PercentHen = percenthen,
                        PercentLen = percentlen,
                        PercentMua = percentmua,
                        PercentDSAo = percentdsao
                    };
                    dataTableReport.Add(datatb);
                }
                if (item1.TotalHen != "0")
                {
                    TargetOptionModel dtHen = new TargetOptionModel
                    {
                        Value = int.Parse(item1.TotalHen),
                        Content = item1.AssignedUserName
                    };
                    datasHen.Add(dtHen);
                }
                if (item1.TotalCall != "0")
                {
                    labels.Add(item1.AssignedUserName);
                    dataCall.Add(item1.TotalCall);
                    dataPlay.Add(item1.TotalPlay);
                }
            }
            datasHen.Sort(new TargetOptionModelComparer());
            return Json(new { labels, dataCall, dataPlay, datasHen, dataTableReport }, JsonRequestBehavior.AllowGet);
        }
        public List<DateRange> CalculateDateRanges(string strFromDate, string strToDate)
        {
            List<DateRange> dateRanges = new List<DateRange>();

            DateTime fromDate = DateTime.ParseExact(strFromDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            DateTime toDate = DateTime.ParseExact(strToDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);

            DateTime currentMonth = new DateTime(fromDate.Year, fromDate.Month, fromDate.Day);
            while (currentMonth <= toDate)
            {
                int startDay = currentMonth.Month == fromDate.Month && currentMonth.Year == fromDate.Year ? fromDate.Day : 1;
                int endDay = currentMonth.Month == toDate.Month && currentMonth.Year == toDate.Year ? toDate.Day : DateTime.DaysInMonth(currentMonth.Year, currentMonth.Month);

                dateRanges.Add(new DateRange
                {
                    Year = currentMonth.Year,
                    Month = currentMonth.Month,
                    StartDay = startDay,
                    EndDay = endDay
                });

                currentMonth = currentMonth.AddMonths(1);
                currentMonth = new DateTime(currentMonth.Year, currentMonth.Month, 1);
            }

            return dateRanges;
        }
        public class TargetOptionModelComparer : IComparer<TargetOptionModel>
        {
            public int Compare(TargetOptionModel x, TargetOptionModel y)
            {
                return x.Value.CompareTo(y.Value);
            }
        }
        #endregion

        #region Lượng lên và mua theo nhân viên
        public ActionResult EmployeeLenMuaReportIndex()
        {
            var branchs = SqlHelper.QuerySP<BranchVModel>("sp_Staff_Branch_GetBranch");
            var BoPhan = SqlHelper.QuerySP<BoPhanVModel>("spSystem_UserType_kd_Select_By_BranchId");
            var user = userRepository.GetAllUsers();
            ViewBag.user = user.Where(x => x.UserType_kd_id != null);
            ViewBag.Branchs = branchs;
            ViewBag.BoPhan = BoPhan;
            return View();
        }
        [HttpGet]
        [AllowAnonymousAttribute]
        public JsonResult GetEmployeeLenMuaReport(string strFromDate, string strToDate, int branchId, int boPhanId, int userId, int isLead)
        {
            List<string> labels1 = new List<string>();
            List<string> labels2 = new List<string>();
            List<string> dataLen = new List<string>();
            List<string> dataMua = new List<string>();
            List<double> percenDS = new List<double>();
            List<string> dataDS = new List<string>();
            var datasp = SqlHelper.QuerySP<EmployeeLenMuaReportModel>("sp_EmployeeLenMuaReport_Select", new
            {
                @fromDate = strFromDate,
                @toDate = strToDate,
                @pBranchId = branchId != -1 ? branchId : (int?)null,
                @pBoPhanId = boPhanId != -1 ? boPhanId : (int?)null,
                @pUserId = userId != -1 ? userId : (int?)null,
                @pisLead = isLead
            });
            long sumAll = datasp.Sum(x => long.Parse(x.TotalDSAo ?? "0"));
            foreach (var item in datasp)
            {
                if (item.TotalLen != "0" || item.TotalMua != "0")
                {
                    labels1.Add(item.AssignedUserName);
                    dataLen.Add(item.TotalLen);
                    dataMua.Add(item.TotalMua);
                }
                if (item.TotalDSAo != null && item.TotalDSAo != "0")
                {
                    double percends = sumAll != 0 ? Math.Round(double.Parse(item.TotalDSAo) * 100.0 / sumAll, 2) : 0;
                    percenDS.Add(percends);
                    dataDS.Add(item.TotalDSAo);
                    labels2.Add(item.AssignedUserName);
                }
            }
            List<string> backgroundColor = new List<string>();
            foreach (var item in typeof(clr).GetProperties().Skip(1))
            {
                backgroundColor.Add(item.Name);
                if (backgroundColor.Count == labels2.Count)
                    break;
            }
            return Json(new { labels1, dataLen, dataMua, percenDS, dataDS, backgroundColor, labels2 }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Đánh giá Topic
        public ActionResult RatingTopicReportIndex()
        {
            var branchs = SqlHelper.QuerySP<BranchVModel>("sp_Staff_Branch_GetBranch");
            var BoPhan = SqlHelper.QuerySP<BoPhanVModel>("spSystem_UserType_kd_Select_By_BranchId");
            var user = userRepository.GetAllUsers();
            ViewBag.user = user.Where(x => x.UserType_kd_id != null);
            ViewBag.Branchs = branchs;
            ViewBag.BoPhan = BoPhan;
            return View();
        }
        [HttpGet]
        [AllowAnonymousAttribute]
        public JsonResult GetRatingTopicReport(int year, int month,int branch,int isLead)
        {
            var branchs = SqlHelper.QuerySP<BranchVModel>("sp_Staff_Branch_GetBranch");
            if(branch != -1)
            {
                branchs = branchs.Where(x => x.Id == branch);
            }
            var dataDSAo = SqlHelper.QuerySP<RatingTopicReportModel>("sp_RatingTopicReport_Select_DSAo", new
            {
                @month = month,
                @year = year,
                @branch = branch,
                @pisLead = isLead
            });
            foreach (var itemdsa in dataDSAo)
            {
                decimal numericValue;
                if (decimal.TryParse(itemdsa.Total, out numericValue))
                {
                    itemdsa.Total = FormatNumber(numericValue);
                }
            }
            var dataHen = SqlHelper.QuerySP<RatingTopicReportModel>("sp_RatingTopicReport_Select_LHen", new
            {
                @month = month,
                @year = year,
                @branch = branch,
                @pisLead = isLead,
                @maxTopics = 3
            });
            var dataHenLen = SqlHelper.QuerySP<RatingTopicReportModel>("sp_RatingTopicReport_Select_LHen", new
            {
                @month = month,
                @year = year,
                @branch = branch,
                @pisLead = isLead
            });
            var dataLen = SqlHelper.QuerySP<RatingTopicReportModel>("sp_RatingTopicReport_Select", new
            {
                @month = month,
                @year = year,
                @branch = branch,
                @pisLead = isLead
            });
            List<RatingTopicReportModel> dataPerent = new List<RatingTopicReportModel>();
            int fla = 0;
            foreach (var itbran in branchs)
            {
                var henLentemp = dataHenLen.Where(x => x.BranchId == itbran.Id);
                foreach (var item in henLentemp)
                {
                    if (item.Total != "0" && item.Total != null)
                    {
                        RatingTopicReportModel itemtmp = new RatingTopicReportModel();
                        var item1 = dataLen.FirstOrDefault(x => x.Topic == item.Topic && x.BranchId == item.BranchId);
                        if (item1 != null)
                        {
                            double percentcall = Math.Round(((double.Parse(item1.Total != null ? item1.Total : "0") * 100.0) / double.Parse(item.Total)), 2);
                            itemtmp.Total = percentcall.ToString() + "%";
                            itemtmp.Topic = item.Topic;
                            itemtmp.BranchId = item.BranchId;
                            itemtmp.BranchName = item.BranchName;
                            dataPerent.Add(itemtmp);
                            fla++;
                        }
                    }
                    if (fla == 3)
                    {
                        break;
                    }
                }
            }

            return Json(new { Success = true, dataDSAo, dataHen, dataPerent }, JsonRequestBehavior.AllowGet);
        }
        private string FormatNumber(decimal value)
        {
            return value.ToString("N0");
        }
        #endregion

        #region DashboardReport
        public ActionResult DashboardReportIndex()
        {
            var branchs = SqlHelper.QuerySP<BranchVModel>("sp_Staff_Branch_GetBranch");
            ViewBag.Branchs = branchs;
            return View();
        }
        [HttpGet]
        [AllowAnonymousAttribute]
        public JsonResult GetResultDashboardRp(string single, int? year, int? month, int? quarter, int? week, int branchId, int isLead)
         {
            single = single == null ? "month" : single;
            year = year == null ? DateTime.Now.Year : year;
            month = month == null ? DateTime.Now.Month : month;
            quarter = quarter == null ? 1 : quarter;
            Calendar calendar = CultureInfo.InvariantCulture.Calendar;
            var weekdefault = calendar.GetWeekOfYear(DateTime.Now, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
            week = week == null ? weekdefault : week;
            DateTime StartDate = DateTime.Now;
            DateTime EndDate = DateTime.Now;
            
            ViewBag.DateRangeText = Helpers.Common.ConvertToDateRange(ref StartDate, ref EndDate, single, year.Value, month.Value, quarter.Value, ref week);

            var datars = SqlHelper.QuerySP<DashboardReportModel>("sp_DashboardReport_Select", new
            {
                @fromDate = StartDate,
                @toDate = EndDate,
                @pBranchId = branchId != -1 ? branchId : (int?)null,
                @pisLead = isLead
            });

            DateTime StartDatebf = DateTime.Now;
            DateTime EndDatebf = DateTime.Now;
            int? weekOfYear = week == null ? calendar.GetWeekOfYear(new DateTime(StartDate.Year, StartDate.Month, 1), CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday) : (int)week;
            if (weekOfYear == 1)
            {
                weekOfYear = calendar.GetWeekOfYear(new DateTime(StartDate.Year - 1, 12, 31), CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
                year = year - 1;
            }
            else
            {
                weekOfYear = weekOfYear - 1;
            }
            int? monthOfYear = month == null ? DateTime.Now.Month : (int)month;
            if (monthOfYear == 1)
            {
                monthOfYear = 12;
                year = year - 1;
            }
            else
            {
                monthOfYear = monthOfYear - 1;
            }
            int? quarterOfYear = quarter == null ? 1 : (int)quarter;
            if (quarterOfYear == 1)
            {
                quarterOfYear = 4;
                year = year - 1;
            }
            else
            {
                quarterOfYear = quarterOfYear - 1;
            }
            ViewBag.DateRangeTestbf = Helpers.Common.ConvertToDateRange(ref StartDatebf, ref EndDatebf, single, year.Value, monthOfYear.Value, quarterOfYear.Value, ref weekOfYear);
            var databf = SqlHelper.QuerySP<DashboardReportModel>("sp_DashboardReport_Select", new
            {
                @fromDate = StartDatebf,
                @toDate = EndDatebf,
                @pBranchId = branchId != -1 ? branchId : (int?)null,
                @pisLead = isLead
            });

            var branchs = SqlHelper.QuerySP<BranchVModel>("sp_Staff_Branch_GetBranch");

            
            List<TargetNewModel> targetNewList = new List<TargetNewModel>();
            foreach (var item in datars)
            {
                decimal totalTargetCall = 0;
                decimal totalTargetPlay = 0;
                decimal totalTargetHen = 0;
                decimal totalTargetLen = 0;
                decimal totalTargetMua = 0;
                decimal totalTargetDSAo = 0;
                if (single != "week")
                {
                    List<TargetNewModel> targetTempList = new List<TargetNewModel>();
                    for (int i = StartDate.Month; i <= EndDate.Month; i++)
                    {
                        var datatg = SqlHelper.QuerySP<TargetNewModel>("sp_Crm_TargetNew_Select", new
                        {
                            @pYear = StartDate.Year,
                            @pMonth = i,
                            @pBranchId = branchId != -1 ? branchId : (int?)null,
                            @pUserId = item.AssignedUserId
                        });
                        targetTempList.AddRange(datatg);
                    }
                    if (targetTempList.Count() > 0)
                    {
                        foreach (var item2 in targetTempList)
                        {
                            if (item2.TargetTotal != null && item2.TypeTarget == 1)
                            {
                                totalTargetCall += item2.TargetTotal;
                            }
                            else if (item2.TargetTotal != null && item2.TypeTarget == 2)
                            {
                                totalTargetPlay += item2.TargetTotal;
                            }
                            if (item2.TargetTotal != null && item2.TypeTarget == 3)
                            {
                                totalTargetHen += item2.TargetTotal;
                            }
                            else if (item2.TargetTotal != null && item2.TypeTarget == 4)
                            {
                                totalTargetLen += item2.TargetTotal;
                            }
                            if (item2.TargetTotal != null && item2.TypeTarget == 5)
                            {
                                totalTargetMua += item2.TargetTotal;
                            }
                            else if (item2.TargetTotal != null && item2.TypeTarget == 6)
                            {
                                totalTargetDSAo += item2.TargetTotal;
                            }
                        }
                        AddTargetNew(targetNewList, branchId != -1 ? branchId : (int?)null, item.AssignedUserId, 1, totalTargetCall);
                        AddTargetNew(targetNewList, branchId != -1 ? branchId : (int?)null, item.AssignedUserId, 2, totalTargetPlay);
                        AddTargetNew(targetNewList, branchId != -1 ? branchId : (int?)null, item.AssignedUserId, 3, totalTargetHen);
                        AddTargetNew(targetNewList, branchId != -1 ? branchId : (int?)null, item.AssignedUserId, 4, totalTargetLen);
                        AddTargetNew(targetNewList, branchId != -1 ? branchId : (int?)null, item.AssignedUserId, 5, totalTargetMua);
                        AddTargetNew(targetNewList, branchId != -1 ? branchId : (int?)null, item.AssignedUserId, 6, totalTargetDSAo);
                    }
                }
                else
                {
                    var datatg = SqlHelper.QuerySP<TargetNewOfDayeModel>("sp_Crm_TargetNewOfDay_Select", new
                    {
                        @pYear = StartDate.Year,
                        @pMonth = StartDate.Month,
                        @pBranchId = branchId != -1 ? branchId : (int?)null,
                        @pUserId = item.AssignedUserId
                    });

                    if (datatg.Count() > 0)
                    {
                        for (int i = StartDate.Day; i <= EndDate.Day; i++)
                        {
                            string daytg = "Day" + i.ToString();
                            var propertyCall = datatg.FirstOrDefault(x => x.TypeTarget == 1)?.GetType().GetProperty(daytg);
                            totalTargetCall += propertyCall != null ? (decimal)propertyCall.GetValue(datatg.FirstOrDefault(x => x.TypeTarget == 1)) : 0;
                            var propertyPlay = datatg.FirstOrDefault(x => x.TypeTarget == 2)?.GetType().GetProperty(daytg);
                            totalTargetPlay += propertyPlay != null ? (decimal)propertyPlay.GetValue(datatg.FirstOrDefault(x => x.TypeTarget == 2)) : 0;
                            var propertyHen = datatg.FirstOrDefault(x => x.TypeTarget == 3)?.GetType().GetProperty(daytg);
                            totalTargetHen += propertyHen != null ? (decimal)propertyHen.GetValue(datatg.FirstOrDefault(x => x.TypeTarget == 3)) : 0;
                            var propertyLen = datatg.FirstOrDefault(x => x.TypeTarget == 4)?.GetType().GetProperty(daytg);
                            totalTargetLen += propertyLen != null ? (decimal)propertyLen.GetValue(datatg.FirstOrDefault(x => x.TypeTarget == 4)) : 0;
                            var propertyMua = datatg.FirstOrDefault(x => x.TypeTarget == 5)?.GetType().GetProperty(daytg);
                            totalTargetMua += propertyMua != null ? (decimal)propertyMua.GetValue(datatg.FirstOrDefault(x => x.TypeTarget == 5)) : 0;
                            var propertyDSAo = datatg.FirstOrDefault(x => x.TypeTarget == 6)?.GetType().GetProperty(daytg);
                            totalTargetDSAo += propertyDSAo != null ? (decimal)propertyDSAo.GetValue(datatg.FirstOrDefault(x => x.TypeTarget == 6)) : 0;
                        }
                        AddTargetNew(targetNewList, branchId != -1 ? branchId : (int?)null, item.AssignedUserId, 1, totalTargetCall);
                        AddTargetNew(targetNewList, branchId != -1 ? branchId : (int?)null, item.AssignedUserId, 2, totalTargetPlay);
                        AddTargetNew(targetNewList, branchId != -1 ? branchId : (int?)null, item.AssignedUserId, 3, totalTargetHen);
                        AddTargetNew(targetNewList, branchId != -1 ? branchId : (int?)null, item.AssignedUserId, 4, totalTargetLen);
                        AddTargetNew(targetNewList, branchId != -1 ? branchId : (int?)null, item.AssignedUserId, 5, totalTargetMua);
                        AddTargetNew(targetNewList, branchId != -1 ? branchId : (int?)null, item.AssignedUserId, 6, totalTargetDSAo);

                    }
                }
            }
            decimal sumtargetCall = targetNewList.Where(x => x.TypeTarget == 1).Sum(x => x.TargetTotal);
            decimal sumuserCall = datars.Sum(x => decimal.Parse(x.TotalCall ?? "0"));
            double percentdatCall = sumtargetCall == 0 ? 0 : Math.Round((double)sumuserCall * 100 / (double)sumtargetCall, 2);
            double percenChuaDatCall = 100 - percentdatCall;
            decimal sumuserCallBf = databf.Sum(x => decimal.Parse(x.TotalCall ?? "0"));
            double percentCallBf = sumuserCallBf == 0 ? 0 : Math.Round(((double)sumuserCall - (double)sumuserCallBf) / (double)sumuserCallBf * 100, 2);

            decimal sumtargetPlay = targetNewList.Where(x => x.TypeTarget == 2).Sum(x => x.TargetTotal);
            decimal sumuserPlay = datars.Sum(x => decimal.Parse(x.TotalPlay ?? "0"));
            double percentdatPlay = sumtargetPlay == 0 ? 0 : Math.Round((double)sumuserPlay * 100 / (double)sumtargetPlay, 2);
            double percenChuaDatPlay = 100 - percentdatPlay;
            decimal sumuserPlayBf = databf.Sum(x => decimal.Parse(x.TotalPlay ?? "0"));
            double percentPlayBf = sumuserPlayBf == 0 ? 0 : Math.Round(((double)sumuserPlay - (double)sumuserPlayBf) / (double)sumuserPlayBf * 100, 2);

            decimal sumtargetHen = targetNewList.Where(x => x.TypeTarget == 3).Sum(x => x.TargetTotal);
            decimal sumuserHen = datars.Sum(x => decimal.Parse(x.TotalHen ?? "0"));
            double percentdatHen = sumtargetHen == 0 ? 0 : Math.Round((double)sumuserHen * 100 / (double)sumtargetHen, 2);
            double percenChuaDatHen = 100 - percentdatHen;
            decimal sumuserHenBf = databf.Sum(x => decimal.Parse(x.TotalHen ?? "0"));
            double percentHenBf = sumuserHenBf == 0 ? 0 : Math.Round(((double)sumuserHen - (double)sumuserHenBf) / (double)sumuserHenBf * 100, 2);

            decimal sumtargetLen = targetNewList.Where(x => x.TypeTarget == 4).Sum(x => x.TargetTotal);
            decimal sumuserLen = datars.Sum(x => decimal.Parse(x.TotalLen ?? "0"));
            double percentdatLen = sumtargetLen == 0 ? 0 : Math.Round((double)sumuserLen * 100 / (double)sumtargetLen, 2);
            double percenChuaDatLen = 100 - percentdatLen;
            decimal sumuserLenBf = databf.Sum(x => decimal.Parse(x.TotalLen ?? "0"));
            double percentLenBf = sumuserLenBf == 0 ? 0 : Math.Round(((double)sumuserLen - (double)sumuserLenBf) / (double)sumuserLenBf * 100, 2);

            decimal sumtargetMua = targetNewList.Where(x => x.TypeTarget == 5).Sum(x => x.TargetTotal);
            decimal sumuserMua = datars.Sum(x => decimal.Parse(x.TotalMua ?? "0"));
            double percentdatMua = sumtargetMua == 0 ? 0 : Math.Round((double)sumuserMua * 100 / (double)sumtargetMua, 2);
            double percenChuaDatMua = 100 - percentdatMua;
            decimal sumuserMuaBf = databf.Sum(x => decimal.Parse(x.TotalMua ?? "0"));
            double percentMuaBf = sumuserMuaBf == 0 ? 0 : Math.Round(((double)sumuserMua - (double)sumuserMuaBf) / (double)sumuserMuaBf * 100, 2);

            decimal sumtargetDSAo = targetNewList.Where(x => x.TypeTarget == 6).Sum(x => x.TargetTotal);
            decimal sumuserDSAo = datars.Sum(x => decimal.Parse(x.TotalDSAo ?? "0"));
            double percentdatDSAo = sumtargetDSAo == 0 ? 0 : Math.Round((double)sumuserDSAo * 100 / (double)sumtargetDSAo, 2);
            double percenChuaDatDSAo = 100 - percentdatDSAo;
            decimal sumuserDSAoBf = databf.Sum(x => decimal.Parse(x.TotalDSAo ?? "0"));
            double percentDSAoBf = sumuserDSAoBf == 0 ? 0 : Math.Round(((double)sumuserDSAo - (double)sumuserDSAoBf) / (double)sumuserDSAoBf * 100, 2);

            List<DhRpDetailModel> datadt = new List<DhRpDetailModel>();
            AddDetailModel(datadt, 1, "Call", percentdatCall, percenChuaDatCall, percentCallBf, datars, x => x.TotalCall);
            AddDetailModel(datadt, 2, "Play", percentdatPlay, percenChuaDatPlay, percentPlayBf, datars, x => x.TotalPlay);
            AddDetailModel(datadt, 3, "Hẹn", percentdatHen, percenChuaDatHen, percentHenBf, datars, x => x.TotalHen);
            AddDetailModel(datadt, 4, "Lên", percentdatLen, percenChuaDatLen, percentLenBf, datars, x => x.TotalLen);
            AddDetailModel(datadt, 5, "Mua", percentdatMua, percenChuaDatMua, percentMuaBf, datars, x => x.TotalMua);
            AddDetailModel(datadt, 6, "Doanh số ảo", percentdatDSAo, percenChuaDatDSAo, percentDSAoBf, datars, x => x.TotalDSAo);

            long sumCallSeconds = datars.Sum(x => long.Parse(x.TotalSecond ?? "0"));
            double sumCallMinutes = Math.Round((double)sumCallSeconds / 60, 2);
            long sumCallSecondsBefore = databf.Sum(x => long.Parse(x.TotalSecond ?? "0"));
            double sumCallMinutesBefore = Math.Round((double)sumCallSecondsBefore / 60, 2);
            double percentCallMinutesChange = sumCallMinutesBefore == 0 ? 0 : Math.Round(((sumCallMinutes - sumCallMinutesBefore) / sumCallMinutesBefore * 100) , 2);
            List<PercentBranchModel> dataPrbr = new List<PercentBranchModel>();
            foreach (var item3 in branchs)
            {
                decimal sumbrHen = datars.Where(x => x.BranchId == item3.Id).Sum(x => decimal.Parse(x.TotalHen ?? "0"));
                double percentbrHen = sumuserHen == 0 ? 0 : Math.Round((double)sumbrHen * 100 / (double)sumuserHen, 2);
                decimal sumbrLen = datars.Where(x => x.BranchId == item3.Id).Sum(x => decimal.Parse(x.TotalLen ?? "0"));
                double percentbrLen = sumuserLen == 0 ? 0 : Math.Round((double)sumbrLen * 100 / (double)sumuserLen, 2);
                decimal sumbrMua = datars.Where(x => x.BranchId == item3.Id).Sum(x => decimal.Parse(x.TotalMua ?? "0"));
                double percentbrMua = sumuserMua == 0 ? 0 : Math.Round((double)sumbrMua * 100 / (double)sumuserMua, 2);
                decimal sumbrDSAo = datars.Where(x => x.BranchId == item3.Id).Sum(x => decimal.Parse(x.TotalDSAo ?? "0"));
                double percentbrDSAo = sumuserDSAo == 0 ? 0 : Math.Round((double)sumbrDSAo * 100 / (double)sumuserDSAo, 2);
                AddPrencntBrNew(dataPrbr, item3.Id, 3, percentbrHen);
                AddPrencntBrNew(dataPrbr, item3.Id, 4, percentbrLen);
                AddPrencntBrNew(dataPrbr, item3.Id, 5, percentbrMua);
                AddPrencntBrNew(dataPrbr, item3.Id, 6, percentbrDSAo);
            }


            return Json(new { Success = true, datars, sumCallMinutes, percentCallMinutesChange, datadt, targetNewList, dataPrbr }, JsonRequestBehavior.AllowGet);

        }
        public void AddDetailModel(List<DhRpDetailModel> models, int key, string name, double dat, double chuadat, double tanggiam, IEnumerable<DashboardReportModel> datars, Func<DashboardReportModel, string> valueSelector)
        {
            var model = new DhRpDetailModel
            {
                Key = key,
                Name = name,
                SumValue = datars.Sum(x => long.Parse(valueSelector(x) ?? "0")).ToString(),
                PercentDat = dat,
                PercentChuaDat = chuadat,
                PercentChange = tanggiam
            };
            models.Add(model);
        }
        public void AddTargetNew(List<TargetNewModel> models, int? BranchId, int UserId, int TypeTarget, decimal TargetTotal)
        {
            var model = new TargetNewModel
            {
                BranchId = BranchId,
                UserId = UserId,
                TypeTarget = TypeTarget,
                TargetTotal = TargetTotal
            };
            models.Add(model);
        }
        public void AddPrencntBrNew(List<PercentBranchModel> models, int BranchId, int TypeTarget, double PercentDat)
        {
            var model = new PercentBranchModel
            {
                BranchId = BranchId,
                TypeTarget = TypeTarget,
                PercentDat = PercentDat
            };
            models.Add(model);
        }

        #endregion

    }
}