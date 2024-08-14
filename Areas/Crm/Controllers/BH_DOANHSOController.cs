using Erp.BackOffice.Filters;
using Erp.BackOffice.Helpers;
using Erp.BackOffice.Account.Models;
using Erp.Domain.Interfaces;
using Erp.Domain.Account.Entities;
using Erp.Domain.Account.Interfaces;
using Erp.Utilities;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using Erp.Domain.Sale.Entities;
using Erp.Domain.Sale.Interfaces;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using WebMatrix.WebData;
using Erp.BackOffice.Sale.Controllers;
using Erp.BackOffice.Sale.Models;
using System.Drawing;
using System.IO;
using Erp.Domain.Crm.Interfaces;
using Erp.Domain.Staff.Entities;
using Erp.Domain.Staff.Interfaces;
using Erp.BackOffice.Staff.Models;
using Erp.BackOffice.Staff.Controllers;
using Erp.Domain.Entities;
using Erp.Domain.Interfaces;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json.Linq;
using System.Transactions;
using Erp.BackOffice.Crm.Models;
using Erp.BackOffice.Areas.Administration.Models;
using Erp.BackOffice.Areas.Crm.Models;
using Erp.Domain.Crm.Entities;
using Erp.Domain.Crm.Repositories;
using Erp.BackOffice.Areas.Cms.Models;
using PagedList;
using PagedList.Mvc;
using Erp.Domain.Account.Helper;
using System.Web;

namespace Erp.BackOffice.Crm.Controllers
{
    [Authorize]
    [InitializeSimpleMembership]
    [Erp.BackOffice.Helpers.NoCacheHelper]
    public class BH_DOANHSOController : Controller
    {
        private readonly ICRM_BH_DOANHSORepository CRM_BH_DOANHSORepository;
        private readonly IProductInvoiceRepository productInvoiRepository;
        private readonly ICategoryRepository categoryRepository;
        private readonly ICustomerRepository CustomerRepository;
        private readonly ICRM_BH_DOANHSO_CTRepository CRM_BH_DOANHSO_CTRepository;
        private readonly IKH_TUONGTACRepository kH_TUONGTACRepository;
        private readonly IUserRepository userRepository;
        private readonly ITemplatePrintRepository templatePrintRepository;
        public BH_DOANHSOController (
            ICRM_BH_DOANHSORepository _CRM_BH_DOANHSO,
            IProductInvoiceRepository _productInvoi,
            ICustomerRepository _Customer,
            ICategoryRepository _category,
            IUserRepository _userID,
            ICRM_BH_DOANHSO_CTRepository _CRM_BH_DOANHSO_CT,
            IKH_TUONGTACRepository _kH_TUONGTAC,
            ITemplatePrintRepository _templatePrintRepository
            )
        {
            CRM_BH_DOANHSORepository = _CRM_BH_DOANHSO;
            productInvoiRepository = _productInvoi;
            categoryRepository = _category;
            CustomerRepository = _Customer;
            userRepository = _userID;
            CRM_BH_DOANHSO_CTRepository = _CRM_BH_DOANHSO_CT;
            kH_TUONGTACRepository = _kH_TUONGTAC;
            templatePrintRepository = _templatePrintRepository;
        }
        #region Index
        public ViewResult Index(int? BranchId, int? month, int? year, int? NGUOILAP)
        {
            //get cookie brachID 
            HttpRequestBase request = this.HttpContext.Request;
            var u = userRepository.GetUserById(WebSecurity.CurrentUserId);
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
            int? GlobalCurentBranchId = Convert.ToInt32(Session["GlobalCurentBranchId"]);
            //branchId = (Erp.BackOffice.Helpers.Common.CurrentUser.BranchId == null ? GlobalCurentBranchId : Erp.BackOffice.Helpers.Common.CurrentUser.BranchId);
            year = year == null ? DateTime.Now.Year : year;
            month = month == null ? DateTime.Now.Month : month;
            //NGUOILAP = NGUOILAP == null ? WebSecurity.CurrentUserId : NGUOILAP;
            var dataNhanvien = SqlHelper.QuerySP<ManagerStaff>("spgetNhanvienbacap", new
            {
                BranchId = (intBrandID == null ? Convert.ToInt32(Session["GlobalCurentBranchId"]) : intBrandID),
                UserId = WebSecurity.CurrentUserId,
            }).ToList();

            List<int> listNhanvien = new List<int>();


            for (int i = 0; i < dataNhanvien.Count; i++)
            {
                listNhanvien.Add(int.Parse(dataNhanvien[i].Id.ToString()));
            }

            var q = CRM_BH_DOANHSORepository.GetAllvwCRM_BH_DOANHSO();
            var user = userRepository.GetAllUsers().Select(item => new UserViewModel
            {
                Id = item.Id,
                UserName = item.UserName,
                FullName = item.FullName,
                Status = item.Status,
                BranchId = item.BranchId,
                UserTypeId = item.UserTypeId
            }).Where(x => x.Status == UserStatus.Active && x.UserTypeId == 2026).ToList();
            
            
            //&& listNhanvien.Contains(x.Id)
            var user2branch = user.Where(x => x.Id == WebSecurity.CurrentUserId).FirstOrDefault();
            //if(listNhanvien.Count()>0)
            //{
            //    user = user.Where(x => listNhanvien.Contains(x.Id)).ToList();
            //}
            if (BranchId != null && BranchId > 0)
            {
                user = user.Where(x => x.BranchId == BranchId).ToList();
                if (user2branch != null && user2branch.BranchId == 0)
                {
                    user.Add(user2branch);
                }
                q = q.Where(x => x.BranchId == BranchId);
            }
            ViewBag.user2 = user;
            if (NGUOILAP != null && NGUOILAP > 0)
            {
                q = q.Where(x => x.NGUOILAP_ID == NGUOILAP);
                user = user.Where(x => x.Id == NGUOILAP).ToList();
            }
            if (NGUOILAP == null)
            {
                user = user.ToList();
            }
            //if (NGUOILAP == null)
            //{
            //    //q = q;
            //    user = user.ToList();
            //}
            //if (!Erp.BackOffice.Filters.SecurityFilter.IsAdmin())
            //{
            //    user = user.Where(x => x.Id == WebSecurity.CurrentUserId).ToList();
            //}
            
            if (!Filters.SecurityFilter.IsAdmin() && u.IsLetan != true && (u.UserTypeId != 1027 || u.UserTypeId != 2053))
            {
                user = user.Where(x => x.Id != 0 && listNhanvien.Contains(x.Id)).ToList();
            }
            if (month != null)
            {
                if (year != null)
                {
                    q = q.Where(x => x.THANG == month && x.NAM == year);
                }
            }
            var origin = categoryRepository.GetAllCategories().Select(item => new CategoryViewModel
            {
                Name = item.Name,
                Value = item.Value,
                Code = item.Code
            }).Where(item => item.Code == "Origin").ToList();
            ViewBag.Origin = origin;

            int count = origin.Count();
            List<string> ten = new List<string>();
            foreach (var item in origin)
            {
                ten.Add(item.Name);
            }

            var model = q.Select(item => new CRM_BH_DOANHSOViewModel
            {
                KH_BANHANG_DOANHSO_ID = item.KH_BANHANG_DOANHSO_ID,
                BranchId = item.BranchId,
                NGUOILAP_ID = item.NGUOILAP_ID,
                BranchName = item.BranchName,
                UserName = item.UserName,
                ModifiedDate = item.ModifiedDate,
                CreatedDate = item.CreatedDate,
                THANG = item.THANG,
                NAM = item.NAM,
                GHI_CHU = item.GHI_CHU,
                CountForBrand = item.CountForBrand,
                TARGET_BRAND = item.TARGET_BRAND,
                TARGET_DALAP = item.TARGET_DALAP
            }).OrderBy(x => x.UserName).ToList();
            List<int?> listUser = new List<int?>();
            foreach (var a in user)
            {
                listUser.Add(a.Id);
            }
            model = model.Where(x => listUser.Contains(x.NGUOILAP_ID)).ToList();
            ViewBag.TarsumANNA = 0;
            ViewBag.TarsumCNC = 0;
            ViewBag.TarsumLEO = 0;
            ViewBag.TarsumDV = 0;
            ViewBag.TarsumOP = 0;
            //
            ViewBag.KHsumANNA = 0;
            ViewBag.KHsumCNC = 0;
            ViewBag.KHsumLEO = 0;
            ViewBag.KHsumDV = 0;
            ViewBag.KHsumOP = 0;

            List<decimal?> tarsum = new List<decimal?>();


            for (int i = 0; i < count; i++)
            {
                tarsum.Add(0);
            }

            if (user != null)
            {
                foreach (var s in model)
                {


                    if (s.TARGET_BRAND > 0)
                    {
                        for (int i = 0; i < count; i++)
                        {
                            string test = Helpers.Common.ChuyenThanhKhongDau(ten[i]).Replace(" ", "");
                            if ((Helpers.Common.ChuyenThanhKhongDau(s.CountForBrand).Replace(" ", "")).Equals(Helpers.Common.ChuyenThanhKhongDau(ten[i]).Replace(" ", "")))
                            {
                                var sum = s.TARGET_BRAND;
                                tarsum[i] = tarsum[i] + sum;
                            }
                        }
                    }
                }
            }
            ViewBag.tarsum = tarsum;

            ViewBag.user = user;
            IEnumerable<CRM_BH_DOANHSO_CTViewModel> CRM_BH_DOANHSO_CT =
                SqlHelper.QuerySP<CRM_BH_DOANHSO_CTViewModel>("TOIUUCSKH_CRM_BH_DOANHSO_CT", new { @pTHANG = month, @pNAM = year });
                //CRM_BH_DOANHSORepository.GetAllvwCRM_BH_DOANHSO_CT().Where(x => x.THANG == month && x.NAM == year).ToList();
            CRM_BH_DOANHSO_CT = CRM_BH_DOANHSO_CT.Where(x => listUser.Contains(x.NGUOILAP_ID));
            //&& listNhanvien.Contains(x.NGUOILAP_ID.Value)
            ViewBag.CRM_BH_DOANHSO_CT = CRM_BH_DOANHSO_CT;
           
            List<decimal?> khsum = new List<decimal?>();
            for (int i = 0; i < count; i++)
            {
                khsum.Add(0);
            }
            if (user != null)
            {
                foreach (var s in CRM_BH_DOANHSO_CT)
                {

                    if (s.CON_NO > 0)
                    {
                        for (int i = 0; i < count; i++)
                        {
                            if ((Helpers.Common.ChuyenThanhKhongDau(s.CountForBrand).Replace(" ", "")).Equals(Helpers.Common.ChuyenThanhKhongDau(ten[i]).Replace(" ", "")))
                            {
                                var sum = s.CON_NO;
                                khsum[i] += sum;
                            }
                        }
                       
                    }
                }
            }
            ViewBag.khsum = khsum;
            List<string> tiendo = new List<string>();
            for (int i = 0; i < count; i++)
            {
                if (tarsum[i] != 0 && tarsum[i] != null)
                {
                    decimal td = (decimal)(khsum[i] * 100 / tarsum[i]);
                    tiendo.Add(Math.Round(td, 1).ToString().Replace(",", "."));
                }
                else
                {
                    tiendo.Add("0.0");
                }
            }
           

            ViewBag.tiendo = tiendo;

            ViewBag.NGUOILAP = WebSecurity.CurrentUserId;
            ViewBag.SuccessMessage = TempData["SuccessMessage"];
            ViewBag.FailedMessage = TempData["FailedMessage"];
            ViewBag.AlertMessage = TempData["AlertMessage"];

            TempData["Data_KHBanHang"] = model;
            TempData["Data_KHBanHang_User"] = user;
            TempData["Data_KHBanHang_origin"] = origin;
            TempData["Data_KeHoach"] = CRM_BH_DOANHSO_CT;
            TempData["Data_Khsum"] = khsum;
            TempData["Data_Tarsum"] = tarsum;
            TempData["Data_TienDo"] = tiendo;
            TempData["Month"] = month;
            TempData["Year"] = year;
            return View(model);

            
        }
        public ActionResult PrintKHDoanhSo(int? Year,int? Month, string txtCode, string txtMinAmount, string txtMaxAmount, string txtCusName, string startDate, string endDate, int? BranchId, string txtCustomerCode, string countForBrand, int? userTypeId, string GDNDT, bool ExportExcel = false)
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
            var data = TempData["Data_KHBanHang"] as List<CRM_BH_DOANHSOViewModel>;
            TempData.Keep();
            var dataUser = TempData["Data_KHBanHang_User"] as List<UserViewModel>;
            TempData.Keep();
            var dataOrigin = TempData["Data_KHBanHang_origin"] as List<CategoryViewModel>;
            TempData.Keep();
            var dataKeHoach = TempData["Data_KeHoach"] as List<CRM_BH_DOANHSO_CTViewModel>;
            TempData.Keep();
            var khsum = TempData["Data_Khsum"] as List<decimal?>; TempData.Keep();
            var tarsum = TempData["Data_Tarsum"] as List<decimal?>; TempData.Keep();
            var tiendo = TempData["Data_TienDo"] as List<string>; TempData.Keep();
            int? year = TempData["Year"] as int?; TempData.Keep();
            int? month = TempData["Month"] as int?; TempData.Keep();

            //data = data.OrderByDescending(c => c.CustomerCode).ToList();
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
            model.Content = model.Content.Replace("{DataTable}", buildHtmlKHDoanhSo(data, dataUser, dataOrigin, dataKeHoach, khsum, tarsum, tiendo, Year, Month));
            model.Content = model.Content.Replace("{System.Logo}", ImgLogo);
            model.Content = model.Content.Replace("{System.CompanyName}", company);
            model.Content = model.Content.Replace("{System.AddressCompany}", address);
            model.Content = model.Content.Replace("{System.PhoneCompany}", phone);
            model.Content = model.Content.Replace("{System.Fax}", fax);
            model.Content = model.Content.Replace("{System.BankCodeCompany}", bankcode);
            model.Content = model.Content.Replace("{System.BankNameCompany}", bankname);
            model.Content = model.Content.Replace("{PrintDate}", DateTime.Now.ToString("dd/MM/yyyy HH:mm"));
            model.Content = model.Content.Replace("{Title}", "Kế hoạch chạy doanh số");
            if (ExportExcel)
            {
                Response.AppendHeader("content-disposition", "attachment;filename=" + "KH_ChayDoanhSO" + DateTime.Now.ToString("yyyyMMdd") + ".xls");
                Response.Charset = "";
                Response.Cache.SetCacheability(HttpCacheability.NoCache);
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.Write(model.Content);
                Response.End();
            }
            return View(model);
        }

        string buildHtmlKHDoanhSo(List<CRM_BH_DOANHSOViewModel> detailList, List<UserViewModel> user, List<CategoryViewModel> origin, List<CRM_BH_DOANHSO_CTViewModel> keHoach, List<decimal?> khsum, List<decimal?> tarsum, List<string> tienDo, int? Year, int? Month)
        {
            string detailLists = "<table border=\"1\" class=\"invoice-detail\">\r\n";
            detailLists += "<thead>\r\n";
            detailLists += "	<tr>";
            detailLists += "		<th rowspan=\"2\">STT</th>";
            //detailLists += "		<th>Trạng thái</th>";
            detailLists += "		<th rowspan=\"2\">Nhân viên lập</th>";
            detailLists += "		<th rowspan=\"2\">Tháng/Năm</th>";
            // detailLists += "		<th>Người tạo</th>";
            detailLists += "		<th colspan=\"5\">Target</th>";
            detailLists += "		<th colspan=\"5\">Kế hoạch</th>";
            detailLists += "		<th colspan=\"5\">Đạt(%)</th>";
            detailLists += "	</tr>";

            detailLists += "	<tr>";
            detailLists += "		<th>ANNAYAKE</th>";
            detailLists += "		<th>CÔNG NGHỆ CAO</th>";
            detailLists += "		<th>LEONOR GREYL</th>";
            detailLists += "		<th>DỊCH VỤ</th>";
            detailLists += "		<th>ORLANE PARIS</th>";

            detailLists += "		<th>ANNAYAKE</th>";
            detailLists += "		<th>CÔNG NGHỆ CAO</th>";
            detailLists += "		<th>LEONOR GREYL</th>";
            detailLists += "		<th>DỊCH VỤ</th>";
            detailLists += "		<th>ORLANE PARIS</th>";

            detailLists += "		<th>ANNAYAKE</th>";
            detailLists += "		<th>CÔNG NGHỆ CAO</th>";
            detailLists += "		<th>LEONOR GREYL</th>";
            detailLists += "		<th>DỊCH VỤ</th>";
            detailLists += "		<th>ORLANE PARIS</th>";
            detailLists += "	</tr>";

            detailLists += "</thead>\r\n";
            detailLists += "<tbody>\r\n";
            var index = 1;
            foreach (var item in user.OrderBy(x => x.UserName))
            {
                List<string> sum = new List<string>();
                List<string> thucte = new List<string>();
                List<string> kehoach = new List<string>();
                List<string> tiendo = new List<string>();
                decimal? target;
                decimal? total;
                foreach (var a in origin)
                {
                    var list = detailList.Where(x => x.CountForBrand == a.Value && x.NGUOILAP_ID == item.Id);
                    if (list.Count() > 0)
                    {
                        target = list.Sum(b => b.TARGET_BRAND);
                        sum.Add(CommonSatic.ToCurrencyStr(list.Sum(b => b.TARGET_BRAND), null));
                    }
                    else
                    {
                        target = 0;
                        sum.Add("0");
                    }

                    var listKH = keHoach.Where(x => x.CountForBrand == a.Value && x.NGUOILAP_ID == item.Id).ToList();

                    if (listKH.Count() > 0)
                    {
                        total = listKH.Sum(b => b.CON_NO);
                        kehoach.Add(CommonSatic.ToCurrencyStr(listKH.Sum(b => b.CON_NO), null));
                    }
                    else
                    {
                        total = 0;
                        kehoach.Add("0");
                    }    
                       
                    if (total > 0)
                    {
                        decimal td = (decimal)(total * 100 / target);
                        CommonSatic.ToCurrencyStr(td, null);
                        tiendo.Add(CommonSatic.ToCurrencyStr(td, null));
                    }
                    else
                        tiendo.Add("0.0");
                }
                detailLists += "<tr>\r\n"
                + "<td class=\"text-center orderNo\">" + (index++) + "</td>\r\n"
                + "<td class=\"text-left \">" + item.FullName + " </td>\r\n"
                + "<td class=\"text-left \">" + Month+"/"+Year + " </td>\r\n"
                + "<td class=\"text-left \">" + sum[0] + " </td>\r\n"
                + "<td class=\"text-left \">" + sum[1] + " </td>\r\n"
                + "<td class=\"text-left \">" + sum[2] + " </td>\r\n"
                + "<td class=\"text-left \">" + sum[3] + " </td>\r\n"
                + "<td class=\"text-left \">" + sum[4] + " </td>\r\n"
                + "<td class=\"text-left \">" + kehoach[0] + " </td>\r\n"
                + "<td class=\"text-left \">" + kehoach[1] + " </td>\r\n"
                + "<td class=\"text-left \">" + kehoach[2] + " </td>\r\n"
                + "<td class=\"text-left \">" + kehoach[3] + " </td>\r\n"
                + "<td class=\"text-left \">" + kehoach[4] + " </td>\r\n"
                + "<td class=\"text-left \">" + tiendo[0] + " </td>\r\n"
                + "<td class=\"text-left \">" + tiendo[1] + " </td>\r\n"
                + "<td class=\"text-left \">" + tiendo[2] + " </td>\r\n"
                + "<td class=\"text-left \">" + tiendo[3] + " </td>\r\n"
                + "<td class=\"text-left \">" + tiendo[4] + " </td>\r\n"
                + "</tr>\r\n";
            };
            detailLists += "</tbody>\r\n";
            detailLists += "<tfoot style=\"font-weight:bold\">\r\n";
            // detailLists += "<tr>";
            detailLists += "<tr><td colspan=\"3\" class=\"text-right\">Tổng cộng</td>"
                         + "<td class=\"text-left \">" + CommonSatic.ToCurrencyStr(tarsum[0],null) + " </td>\r\n"
                         + "<td class=\"text-left \">" + CommonSatic.ToCurrencyStr(tarsum[1],null) + " </td>\r\n"
                         + "<td class=\"text-left \">" + CommonSatic.ToCurrencyStr(tarsum[2],null) + " </td>\r\n"
                         + "<td class=\"text-left \">" + CommonSatic.ToCurrencyStr(tarsum[3],null) + " </td>\r\n"
                         + "<td class=\"text-left \">" + CommonSatic.ToCurrencyStr(tarsum[4],null) + " </td>\r\n"
                         + "<td class=\"text-left \">" + CommonSatic.ToCurrencyStr(khsum[0],null) + " </td>\r\n"
                         + "<td class=\"text-left \">" + CommonSatic.ToCurrencyStr(khsum[1], null) + " </td>\r\n"
                         + "<td class=\"text-left \">" + CommonSatic.ToCurrencyStr(khsum[2], null) + " </td>\r\n"
                         + "<td class=\"text-left \">" + CommonSatic.ToCurrencyStr(khsum[3], null) + " </td>\r\n"
                         + "<td class=\"text-left \">" + CommonSatic.ToCurrencyStr(khsum[4], null) + " </td>\r\n"
                         + "<td class=\"text-left \">" + tienDo[0] + " </td>\r\n"
                         + "<td class=\"text-left \">" + tienDo[1] + " </td>\r\n"
                         + "<td class=\"text-left \">" + tienDo[2] + " </td>\r\n"
                         + "<td class=\"text-left \">" + tienDo[3] + " </td>\r\n"
                         + "<td class=\"text-left \">" + tienDo[4] + " </td>\r\n"
                         + "</tr>";
            detailLists += "</tfoot>\r\n</table>\r\n";
            return detailLists;
        }
        #endregion
        #region Create
        public ViewResult Create(CRM_BH_DOANHSOViewModel model, int? id, string FullName, int? month, int? year)
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

            var dataNhanvien = SqlHelper.QuerySP<ManagerStaff>("spgetNhanvienbacap", new
            {
                BranchId = (intBrandID == null ? 0 : intBrandID),
                UserId = WebSecurity.CurrentUserId,
            }).ToList();

            List<int> listNhanvien = new List<int>();


            for (int i = 0; i < dataNhanvien.Count; i++)
            {
                listNhanvien.Add(int.Parse(dataNhanvien[i].Id.ToString()));
            }
            model.CRM_BH_DOANHSO_CTList = CRM_BH_DOANHSORepository.GetAllvwCRM_BH_DOANHSO_CT().Select(x => new CRM_BH_DOANHSO_CTViewModel
            {
                KH_BANHANG_DOANHSO_CTIET_ID = x.KH_BANHANG_DOANHSO_CTIET_ID,
                BranchId = x.BranchId,
                KH_BANHANG_DOANHSO_ID = x.KH_BANHANG_DOANHSO_ID,
                THANG = x.THANG,
                NAM = x.NAM,
                MA_DONHANG = x.MA_DONHANG,
                NGAY_MUA = x.NGAY_MUA,
                TONG_TIEN = x.TONG_TIEN,
                DA_TRA = x.DA_TRA,
                CON_NO = x.CON_NO,
                KHACHHANG_ID = x.KHACHHANG_ID,
                NOIDUNG = x.NOIDUNG,
                TYLE_THANHCONG = x.TYLE_THANHCONG,
                TYLE_THANHCONG_REVIEW = x.TYLE_THANHCONG_REVIEW,
                GHI_CHU = x.GHI_CHU,
                IsDeleted = x.IsDeleted,
                CreatedDate = x.CreatedDate,
                CustomerName = x.CustomerName,
                CustomerCode = x.CustomerCode,
                Phone = x.Phone,
                CountForBrand = x.CountForBrand,
                NGUOILAP_ID = x.NGUOILAP_ID
            }).Where(x => x.BranchId == intBrandID && x.THANG == month && x.NAM == year && x.NGUOILAP_ID == id).ToList();
            //model.CRM_BH_DOANHSO_CTIETList = productInvoiRepository.GetAllvwProductInvoice().Select(x => new ProductInvoiceViewModel
            //{
            //    Id = x.Id,
            //    CustomerId = x.CustomerId,
            //    Code = x.Code,
            //    CustomerPhone = x.CustomerPhone,
            //    TotalCredit = x.TotalCredit,
            //    TotalDebit = x.TotalDebit,
            //    TongConNo = x.TotalDebit - x.TotalCredit,
            //    BranchId = x.BranchId,
            //    CreatedUserName = x.CustomerName,
            //    ManagerStaffId = x.ManagerStaffId,
            //    CreatedDate = x.CreatedDate,
            //    CustomerName=x.CustomerName,
            //    CustomerCode=x.CustomerCode,
            //    Phone=x.CustomerPhone,
            //}).Where(x=>listNhanvien.Contains(x.ManagerStaffId.Value)).ToList();
            if (id != null)
            {
                var user = userRepository.GetUserById(id.Value);
                model.NGUOILAP_ID = user.Id;
                model.FullName = user.FullName;
            }
            else
            {
                var user = userRepository.GetUserById(WebSecurity.CurrentUserId);
                model.NGUOILAP_ID = user.Id;
                model.FullName = user.FullName;
            }
            ViewBag.FailedMessage = TempData["FailedMessage"];
            return View(model);
        }

        [HttpPost]
        public ActionResult Create(CRM_BH_DOANHSOViewModel model)
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

            if (ModelState.IsValid)
            {
                using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.Snapshot }))
                {
                    try
                    {

                        //if (CRM_KH_BANHANG != null)
                        //{
                        var BranchId = (intBrandID == null ? Convert.ToInt32(Session["GlobalCurentBranchId"]) : intBrandID);
                        // kiểm tra đã có kế hoạch doanh số trong tháng chưa

                        var list = CRM_BH_DOANHSORepository.GetlistAllvwCRM_BH_DOANHSO().Where(x => x.BranchId == BranchId && x.CountForBrand == model.CountForBrand && x.NGUOILAP_ID == model.NGUOILAP_ID && x.THANG == model.THANG && x.NAM == model.NAM).ToList();
                        if (list.Count() == 0)
                        {
                            var CRM_BH_DOANHSO = new CRM_BH_DOANHSO();
                            AutoMapper.Mapper.Map(model, CRM_BH_DOANHSO);
                            CRM_BH_DOANHSO.IsDeleted = false;
                            CRM_BH_DOANHSO.CreatedUserId = WebSecurity.CurrentUserId;
                            CRM_BH_DOANHSO.ModifiedUserId = WebSecurity.CurrentUserId;

                            CRM_BH_DOANHSO.CreatedDate = DateTime.Now;
                            CRM_BH_DOANHSO.ModifiedDate = DateTime.Now;
                            CRM_BH_DOANHSO.NAM = model.NAM;
                            CRM_BH_DOANHSO.THANG = model.THANG;
                            CRM_BH_DOANHSO.BranchId = BranchId;
                            //CRM_KH_BANHANG_CTIET.KHACHHANG_ID = model.CustomerId;
                            CRM_BH_DOANHSO.CountForBrand = model.CountForBrand;
                            CRM_BH_DOANHSO.TARGET_BRAND = model.TARGET_BRAND;
                            CRM_BH_DOANHSO.NGUOILAP_ID = model.NGUOILAP_ID;
                            CRM_BH_DOANHSORepository.InserCRM_BH_DOANHSO(CRM_BH_DOANHSO);
                            //if (model.CRM_BH_DOANHSO_CTIETList.Any())
                            //{
                            //    //lưu danh sách thao tác thực hiện dịch vụ
                            //    foreach (var item in model.CRM_BH_DOANHSO_CTIETList)
                            //    {
                            //        if (item.is_checked != null)
                            //        {
                            //            var ins = new CRM_BH_DOANHSO_CT();
                            //            var productinvoice = new ProductInvoice();
                            //            ins.IsDeleted = false;
                            //            ins.CreatedUserId = WebSecurity.CurrentUserId;
                            //            ins.ModifiedUserId = WebSecurity.CurrentUserId;
                            //            ins.CreatedDate = DateTime.Now;
                            //            ins.ModifiedDate = DateTime.Now;
                            //            ins.THANG = model.THANG;
                            //            ins.NAM = model.NAM;
                            //            ins.KHACHHANG_ID = item.CustomerId;
                            //            ins.MA_DONHANG = item.Code;
                            //            ins.TONG_TIEN = item.TotalDebit;
                            //            ins.DA_TRA = item.TotalCredit;
                            //            ins.CON_NO = item.TongConNo;
                            //            ins.BranchId = Erp.BackOffice.Helpers.Common.CurrentUser.BranchId;
                            //            ins.GHI_CHU = item.GHI_CHU;
                            //            ins.TYLE_THANHCONG = item.TYLE_THANHCONG;
                            //            ins.TYLE_THANHCONG_REVIEW = item.TYLE_THANHCONG_REVIEW;
                            //            ins.KH_BANHANG_DOANHSO_ID = CRM_BH_DOANHSO.KH_BANHANG_DOANHSO_ID;
                            //            ins.NGAY_MUA = item.CreatedDate.ToString();
                            //            CRM_BH_DOANHSO_CTRepository.InsertCRM_BH_DOANHSO_CTIET(ins);
                            //        }
                            //    }
                            //}
                            //ghi log 
                            Erp.BackOffice.Controllers.HomeController.WriteLog(CRM_BH_DOANHSO.KH_BANHANG_DOANHSO_ID, "", "đã lập kế hoạch chạy doanh số (" + model.THANG +"/" + model.NAM + ")" + " cho nhãn hàng "+ model.CountForBrand , "", Helpers.Common.CurrentUser.BranchId.Value);

                            scope.Complete();
                            TempData[Globals.SuccessMessageKey] = App_GlobalResources.Wording.InsertSuccess;
                            return RedirectToAction("_ClosePopup", "Home", new { area = "", FunctionCallback = "ClosePopupAndReloadPage" });
                        }
                        else
                        {
                            scope.Complete();
                            TempData[Globals.FailedMessageKey] = "Tháng " + model.THANG + " đã có kế hoạch nhãn hàng " + model.CountForBrand;
                            return RedirectToAction("_ClosePopup", "Home", new { area = "", FunctionCallback = "ClosePopupAndReloadPage" });
                        }
                    }
                    catch (DbUpdateException ex)
                    {
                        TempData[Globals.FailedMessageKey] = App_GlobalResources.Wording.Error;

                        return View(model);
                    }
                }

            }


            return View(model);
        }
        #endregion
        #region Edit
        public ActionResult Edit(int id, int? month, int? year, string sortOrder)
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


            var dataNhanvien = SqlHelper.QuerySP<ManagerStaff>("spgetNhanvienbacap", new
            {
                BranchId = (intBrandID == null ? Convert.ToInt32(Session["GlobalCurentBranchId"]) : intBrandID),
                UserId = WebSecurity.CurrentUserId,
            }).ToList();

            List<int> listNhanvien = new List<int>();


            for (int i = 0; i < dataNhanvien.Count; i++)
            {
                listNhanvien.Add(int.Parse(dataNhanvien[i].Id.ToString()));
            }
            var user2 = userRepository.GetAllUsers().Select(item => new UserViewModel
            {
                Id = item.Id,
                UserName = item.UserName,
                FullName = item.FullName,
                Status = item.Status,
                BranchId = item.BranchId,
                IdManager = item.IdManager
            }).Where(x => x.Status == UserStatus.Active && listNhanvien.Contains(x.Id)).ToList();
            ViewBag.user = user2;


            var model = new CRM_BH_DOANHSOViewModel();
            IEnumerable<CRM_BH_DOANHSOViewModel> q = CRM_BH_DOANHSORepository.GetAllvwCRM_BH_DOANHSO()
                            .Select(item => new CRM_BH_DOANHSOViewModel
                            {
                                KH_BANHANG_DOANHSO_ID = item.KH_BANHANG_DOANHSO_ID,
                                BranchId = item.BranchId,
                                IsDeleted = item.IsDeleted,
                                CreatedDate = item.CreatedDate,
                                CreatedUserId = item.CreatedUserId,
                                ModifiedDate = item.ModifiedDate,
                                ModifiedUserId = item.ModifiedUserId,
                                NGUOILAP_ID = item.NGUOILAP_ID,
                                TARGET_BRAND = item.TARGET_BRAND,
                                GHI_CHU = item.GHI_CHU,
                                CountForBrand = item.CountForBrand,
                                THANG = item.THANG,
                                NAM = item.NAM,
                            }).Where(x => x.NGUOILAP_ID == id && x.IsDeleted == false && x.THANG == month && x.NAM == year).ToList();
            foreach (var item in q)
            {
                model.KH_BANHANG_DOANHSO_ID = item.KH_BANHANG_DOANHSO_ID;
                model.BranchId = item.BranchId;
                model.IsDeleted = item.IsDeleted;
                model.CreatedDate = item.CreatedDate;
                model.CreatedUserId = item.CreatedUserId;
                model.ModifiedDate = item.ModifiedDate;
                model.ModifiedUserId = item.ModifiedUserId;
                model.CountForBrand = item.CountForBrand;
                model.GHI_CHU = item.GHI_CHU;


                if (item.CountForBrand == "ANNAYAKE")
                {
                    model.ID_ANNAYAKE = item.KH_BANHANG_DOANHSO_ID;
                    model.TARGET_BRAND = item.TARGET_BRAND;
                    ViewBag.GHICHU_ANNAYAKE = item.GHI_CHU;
                    ViewBag.ID_ANNAYAKE = item.KH_BANHANG_DOANHSO_ID;
                    ViewBag.ANNAYAKE = item.TARGET_BRAND;
                }

                if (item.CountForBrand.Replace(" ", "") == "ORLANEPARIS")
                {
                    model.ID_ORLANEPARIS = item.KH_BANHANG_DOANHSO_ID;
                    ViewBag.GHICHU_ORLANEPARIS = item.GHI_CHU;
                    ViewBag.ID_ORLANEPARIS = item.KH_BANHANG_DOANHSO_ID;
                    ViewBag.ORLANEPARIS = item.TARGET_BRAND;
                }

                if (item.CountForBrand.Replace(" ", "") == "LEONORGREYL")
                {
                    model.ID_LEONORGREYL = item.KH_BANHANG_DOANHSO_ID;
                    ViewBag.GHICHU_LEONORGREYL = item.GHI_CHU;
                    ViewBag.ID_LEONORGREYL = item.KH_BANHANG_DOANHSO_ID;
                    ViewBag.LEONORGREYL = item.TARGET_BRAND;
                }

                if (item.CountForBrand.Replace(" ", "") == "CONGNGHECAO")
                {
                    model.ID_CONGNGHECAO = item.KH_BANHANG_DOANHSO_ID;
                    ViewBag.GHICHU_CNC = item.GHI_CHU;
                    ViewBag.ID_CNC = item.KH_BANHANG_DOANHSO_ID;
                    ViewBag.CNC = item.TARGET_BRAND;
                }


                if (item.CountForBrand.Replace(" ", "") == "DICHVU")
                {
                    model.ID_DICHVU = item.KH_BANHANG_DOANHSO_ID;
                    ViewBag.GHICHU_DV = item.GHI_CHU;
                    ViewBag.ID_DV = item.KH_BANHANG_DOANHSO_ID;
                    ViewBag.DV = item.TARGET_BRAND;
                }

            }

            if (id != null)
            {
                var user = userRepository.GetUserById(id);
                model.NGUOILAP_ID = user.Id;
                model.FullName = user.FullName;
            }
            else
            {
                var user = userRepository.GetUserById(WebSecurity.CurrentUserId);
                model.NGUOILAP_ID = user.Id;
                model.FullName = user.FullName;
            }
            model.CRM_BH_DOANHSO_CTList = SqlHelper.QuerySP<CRM_BH_DOANHSO_CTViewModel>("spCRM_KH_BANHANG_DOANHSO_CTIET", new
            {
            }).Where(x => x.IsDeleted == false && x.THANG == month && x.NAM == year && x.NGUOILAP_ID == model.NGUOILAP_ID).ToList();
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.DateSortParm = sortOrder == "History" ? "history_desc" : "History";
            switch (sortOrder)
            {
                case "name_desc":
                    model.CRM_BH_DOANHSO_CTList = model.CRM_BH_DOANHSO_CTList.OrderByDescending(s => s.CustomerName).ToList();
                    break;
                case "history_desc":
                    model.CRM_BH_DOANHSO_CTList = model.CRM_BH_DOANHSO_CTList.OrderByDescending(s => s.KHACHHANG_ID).ToList();
                    break;
                default:
                    model.CRM_BH_DOANHSO_CTList = model.CRM_BH_DOANHSO_CTList.OrderBy(s => s.CustomerName).ToList();
                    break;
            }

            //IEnumerable<KH_TUONGTACViewModel> KH_TUONGTAC = kH_TUONGTACRepository.GetAllvwKH_TUONGTAC()
            //  .Select(item => new KH_TUONGTACViewModel
            //  {
            //      KH_TUONGTAC_ID = item.KH_TUONGTAC_ID,
            //      NGAYLAP = item.NGAYLAP,
            //      HINHTHUC_TUONGTAC = item.HINHTHUC_TUONGTAC,
            //      LOAI_TUONGTAC = item.LOAI_TUONGTAC,
            //      PHANLOAI_TUONGTAC = item.PHANLOAI_TUONGTAC,
            //      TINHTRANG_TUONGTAC = item.TINHTRANG_TUONGTAC,
            //      MUCDO_TUONGTAC = item.MUCDO_TUONGTAC,
            //      GHI_CHU = item.GHI_CHU,
            //      GIAIPHAP_TUONGTAC = item.GIAIPHAP_TUONGTAC,
            //      NGAYTUONGTAC_TIEP = item.NGAYTUONGTAC_TIEP,
            //      GIOTUONGTAC_TIEP = item.GIOTUONGTAC_TIEP,
            //      MUCCANHBAO_TUONGTAC = item.MUCCANHBAO_TUONGTAC,
            //      HINH_ANH = item.HINH_ANH,
            //      BranchId = item.BranchId,
            //      FullName = item.FullName,
            //      ModifiedDate = item.ModifiedDate,
            //      CreatedDate = item.CreatedDate,
            //      GIO_TUONGTAC = item.GIO_TUONGTAC,
            //      NGUOILAP_ID = item.NGUOILAP_ID,
            //      KHACHHANG_ID = item.KHACHHANG_ID,
            //      KETQUA_SAUTUONGTAC = item.KETQUA_SAUTUONGTAC

            //  });
            //ViewBag.LICHSUTUONGTAC = KH_TUONGTAC;

            //foreach (var item in model.CRM_BH_DOANHSO_CTList)
            //{
            //    var invoice = productInvoiRepository.GetvwProductInvoiceByCode(item.BranchId.Value, item.MA_DONHANG);
            //    if (invoice != null)
            //    {
            //        item.ProductInvoiceId = invoice.Id;
            //    }
            //}
            ViewBag.vwCRM_BH_DOANHSO_CT = model.CRM_BH_DOANHSO_CTList;
            var origin = categoryRepository.GetAllCategories().Select(item => new CategoryViewModel
            {
                Name = item.Name,
                Value = item.Value.Replace(" ", ""),
                Code = item.Code
            }).Where(item => item.Code == "Origin");
            ViewBag.Origin = origin;
            model.THANG = month;
            model.NAM = year;
            return View(model);
        }
        [HttpPost]
        public ActionResult Edit(CRM_BH_DOANHSOViewModel model)
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.Snapshot }))
            {
                try
                {
                    if (model.ID_ANNAYAKE.HasValue)
                    {
                        var CRM_KH_BANHANG_ANNAYAKE = CRM_BH_DOANHSORepository.GetCRM_BH_DOANHSOById(model.ID_ANNAYAKE.Value);
                        CRM_KH_BANHANG_ANNAYAKE.TARGET_BRAND = model.TARGET_BRAND;
                        CRM_BH_DOANHSORepository.UpdateCRM_BH_DOANHSO(CRM_KH_BANHANG_ANNAYAKE);
                    }
                    if (model.ID_ORLANEPARIS.HasValue)
                    {
                        var CRM_KH_BANHANG_ORLANEPARIS = CRM_BH_DOANHSORepository.GetCRM_BH_DOANHSOById(model.ID_ORLANEPARIS.Value);
                        CRM_KH_BANHANG_ORLANEPARIS.TARGET_BRAND = model.ORLANEPARIS;
                        CRM_BH_DOANHSORepository.UpdateCRM_BH_DOANHSO(CRM_KH_BANHANG_ORLANEPARIS);
                    }
                    if (model.ID_LEONORGREYL.HasValue)
                    {
                        var CRM_KH_BANHANG_LEONORGREYL = CRM_BH_DOANHSORepository.GetCRM_BH_DOANHSOById(model.ID_LEONORGREYL.Value);
                        CRM_KH_BANHANG_LEONORGREYL.TARGET_BRAND = model.LEONORGREYL;
                        CRM_BH_DOANHSORepository.UpdateCRM_BH_DOANHSO(CRM_KH_BANHANG_LEONORGREYL);
                    }
                    if (model.ID_CONGNGHECAO.HasValue)
                    {
                        var CRM_KH_BANHANG_CONGNGHECAO = CRM_BH_DOANHSORepository.GetCRM_BH_DOANHSOById(model.ID_CONGNGHECAO.Value);
                        CRM_KH_BANHANG_CONGNGHECAO.TARGET_BRAND = model.CONGNGHECAO;
                        CRM_BH_DOANHSORepository.UpdateCRM_BH_DOANHSO(CRM_KH_BANHANG_CONGNGHECAO);
                    }
                    if (model.ID_DICHVU.HasValue)
                    {
                        var CRM_KH_BANHANG_DICHVU = CRM_BH_DOANHSORepository.GetCRM_BH_DOANHSOById(model.ID_DICHVU.Value);
                        CRM_KH_BANHANG_DICHVU.TARGET_BRAND = model.DICHVU;
                        CRM_BH_DOANHSORepository.UpdateCRM_BH_DOANHSO(CRM_KH_BANHANG_DICHVU);
                    }


                    if (model.CRM_BH_DOANHSO_CTList != null)
                    {
                        foreach (var item in model.CRM_BH_DOANHSO_CTList)
                        {
                            var CRM_BH_DOANHSO = CRM_BH_DOANHSORepository.GetCRM_BH_DOANHSO_CTById(Helpers.Common.NVL_NUM_NT_NEW(Erp.BackOffice.Helpers.Common.CurrentUser.BranchId), item.KH_BANHANG_DOANHSO_CTIET_ID);
                            //AutoMapper.Mapper.Map(item, CRM_KH_BANHANG_CT);
                            CRM_BH_DOANHSO.TYLE_THANHCONG = item.TYLE_THANHCONG;
                            CRM_BH_DOANHSO.TYLE_THANHCONG_REVIEW = item.TYLE_THANHCONG_REVIEW;
                            CRM_BH_DOANHSO.ModifiedDate = DateTime.Now;
                            CRM_BH_DOANHSO.ModifiedUserId = WebSecurity.CurrentUserId;
                            CRM_BH_DOANHSORepository.UpdateCRM_BH_DOANHSO_CT(CRM_BH_DOANHSO);
                        }
                        //scope.Complete();
                        //TempData[Globals.SuccessMessageKey] = App_GlobalResources.Wording.UpdateSuccess;
                        //return RedirectToAction("Edit", new { Id = CRM_KH_BANHANG_ANNAYAKE.NGUOILAP_ID, month = CRM_KH_BANHANG_ANNAYAKE.THANG, year = CRM_KH_BANHANG_ANNAYAKE.NAM });
                    }

                    scope.Complete();
                    TempData[Globals.SuccessMessageKey] = App_GlobalResources.Wording.UpdateSuccess;
                    return RedirectToAction("Edit", new { Id = model.NGUOILAP_ID, month = model.THANG, year = model.NAM });
                    //return RedirectToAction("Index");
                }
                catch (DbUpdateException ex)
                {
                    TempData[Globals.FailedMessageKey] = App_GlobalResources.Wording.Error;
                    return View(model);
                }
            }
        }
        #endregion
        #region CreateKH
        public ActionResult CreateKH(int? SalerId, int? KH_BANHANG_DOANHSO_ID, string CustomerCode, string CustomerName, string Phone, string MA_DONHANG, string startDate, string endDate, string item_origin)
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

            var dataNhanvien = SqlHelper.QuerySP<ManagerStaff>("spgetNhanvienbacap", new
            {
                BranchId = (intBrandID == null ? 0 : intBrandID),
                UserId = WebSecurity.CurrentUserId,
            }).ToList();

           
            List<int> listNhanvien = new List<int>();

            var category = categoryRepository.GetAllCategories()
          .Select(item => new CategoryViewModel
          {
              Id = item.Id,
              Code = item.Code,
              Value = item.Value,
              Name = item.Name
          }).Where(x => x.Code == "Origin").ToList();
            ViewBag.category = category;

            for (int i = 0; i < dataNhanvien.Count; i++)
            {
                listNhanvien.Add(int.Parse(dataNhanvien[i].Id.ToString()));
            }
            var user2 = userRepository.GetAllUsers().Select(item => new UserViewModel
            {
                Id = item.Id,
                UserName = item.UserName,
                FullName = item.FullName,
                Status = item.Status,
                BranchId = item.BranchId,
                IdManager = item.IdManager
            }).Where(x => x.Status == UserStatus.Active && listNhanvien.Contains(x.Id)).ToList();
            ViewBag.user = user2;

            var model = new CRM_BH_DOANHSOViewModel();
            if (KH_BANHANG_DOANHSO_ID != null)
            {
                model.KH_BANHANG_DOANHSO_ID = KH_BANHANG_DOANHSO_ID.Value;
            }


            //if ((SalerId == null) || (SalerId == 0))
            //{
            //    return View(model);
            //}

            DateTime d_startDate, d_endDate;
            if (DateTime.TryParseExact(startDate, "dd/MM/yyyy", new CultureInfo("vi-VN"), DateTimeStyles.None, out d_startDate))
            {

            }
            if (DateTime.TryParseExact(endDate, "dd/MM/yyyy", new CultureInfo("vi-VN"), DateTimeStyles.None, out d_endDate))
            {
                d_endDate = d_endDate.AddHours(23).AddMinutes(59);
            }


            //model.CRM_BH_DOANHSO_CTIETList = productInvoiRepository.GetAllvwProductInvoice().Select(x => new ProductInvoiceViewModel
            //{
            //    Id = x.Id,
            //    CustomerId = x.CustomerId,
            //    Code = x.Code,
            //    CustomerPhone = x.CustomerPhone,
            //    TotalCredit = x.TotalCredit,
            //    TotalDebit = x.TotalDebit,
            //    TongConNo = x.TotalDebit - x.TotalCredit,
            //    BranchId = x.BranchId,
            //    CreatedUserName = x.CustomerName,
            //    ManagerStaffId = x.ManagerStaffId,
            //    ManagerStaffName = x.ManagerStaffName,
            //    CreatedDate = x.CreatedDate,
            //    CustomerName = x.CustomerName,
            //    CustomerCode = x.CustomerCode,
            //    Phone = x.CustomerPhone,
            //    tienconno = x.tienconno,
            //    CountForBrand = x.CountForBrand,
            //    Status = x.Status,
            //     IsArchive = x.IsArchive
            //}).Where(x => (x.BranchId == intBrandID) && (x.TotalDebit - x.TotalCredit) >0 &&  x.Status != "delete" && x.IsArchive != true/*&& (x.tienconno > 0) && x.ManagerStaffId == SalerId*/).ToList();

            model.CRM_BH_DOANHSO_CTIETList = SqlHelper.QuerySP<ProductInvoiceViewModel>("getInvoiceThuNo", new
            {
                BranchId = intBrandID
               
            }).ToList();

            if (SalerId != null)
            {
                model.CRM_BH_DOANHSO_CTIETList = model.CRM_BH_DOANHSO_CTIETList.Where(x => x.ManagerStaffId == SalerId).ToList();

            }

            if (item_origin != null && item_origin != "")
            {
                model.CRM_BH_DOANHSO_CTIETList = model.CRM_BH_DOANHSO_CTIETList.Where(x => x.CountForBrand == item_origin).ToList();
            }

            if ((startDate != "") && (endDate != ""))
            {

                model.CRM_BH_DOANHSO_CTIETList = model.CRM_BH_DOANHSO_CTIETList.Where(x => (x.CreatedDate >= d_startDate && x.CreatedDate <= d_endDate)).ToList();

            }

            //if (!Erp.BackOffice.Filters.SecurityFilter.IsAdmin())
            //{
            //    model.CRM_BH_DOANHSO_CTIETList = model.CRM_BH_DOANHSO_CTIETList.Where(x => listNhanvien.Contains(x.ManagerStaffId.Value)).ToList();
            //}


            bool hasSearch = false;
            if ((CustomerCode != null) && (CustomerCode != ""))
            {
                model.CRM_BH_DOANHSO_CTIETList = model.CRM_BH_DOANHSO_CTIETList.Where(x => Helpers.Common.ChuyenThanhKhongDau(x.CustomerCode).Contains(CustomerCode)).ToList();
                hasSearch = true;
            }
            if ((CustomerName != null) && (CustomerName != ""))
            {
                CustomerName = Helpers.Common.ChuyenThanhKhongDau(CustomerName);
                model.CRM_BH_DOANHSO_CTIETList = model.CRM_BH_DOANHSO_CTIETList.Where(x => Helpers.Common.ChuyenThanhKhongDau(x.CustomerName).Contains(CustomerName)).ToList();
                hasSearch = true;
            }
            if ((Phone != null) && (Phone != ""))
            {
                model.CRM_BH_DOANHSO_CTIETList = model.CRM_BH_DOANHSO_CTIETList.Where(x => Helpers.Common.ChuyenThanhKhongDau(x.Phone).Contains(Phone)).ToList();
                hasSearch = true;
            }
            if ((MA_DONHANG != null) && (MA_DONHANG != ""))
            {
                MA_DONHANG = Helpers.Common.ChuyenThanhKhongDau(MA_DONHANG);
                model.CRM_BH_DOANHSO_CTIETList = model.CRM_BH_DOANHSO_CTIETList.Where(x => Helpers.Common.ChuyenThanhKhongDau(x.Code).Contains(MA_DONHANG)).ToList();
                hasSearch = true;
            }
            var CRM_BH_DOANHSO_CT = CRM_BH_DOANHSORepository.GetAllvwCRM_BH_DOANHSO_CT().Where(x => x.KH_BANHANG_DOANHSO_ID == KH_BANHANG_DOANHSO_ID && x.IsDeleted == false).ToList();
            //model.CRM_BH_DOANHSO_CTIETList.RemoveAll(
            //                x =>
            //                CRM_BH_DOANHSO_CT.Any(
            //                k => k.KHACHHANG_ID == x.CustomerId));
            model.CRM_BH_DOANHSO_CTIETList.RemoveAll(
                            x => CRM_BH_DOANHSO_CT.Any(
                            k => k.MA_DONHANG == x.Code));
            var CRM_BH_DOANHSO_CT2 = new List<ProductInvoiceViewModel>();
            foreach (var item in model.CRM_BH_DOANHSO_CTIETList)
            {
                var cus = CustomerRepository.GetvwCustomerById(item.CustomerId.Value);
                if(cus.isLock == true)
                {
                    CRM_BH_DOANHSO_CT2.Add(item);
                }
            }
            model.CRM_BH_DOANHSO_CTIETList.RemoveAll(
                           x => CRM_BH_DOANHSO_CT2.Any(
                           k => k.Code == x.Code));


            return View(model);
        }
        [HttpPost]
        public ActionResult CreateKH(CRM_BH_DOANHSOViewModel model)
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
            try
            {
                var q = CRM_BH_DOANHSORepository.GetCRM_BH_DOANHSOById(model.KH_BANHANG_DOANHSO_ID);
                if (model.CRM_BH_DOANHSO_CTIETList.Any())
                {
                    foreach (var item in model.CRM_BH_DOANHSO_CTIETList)
                    {
                        if (item.is_checked != null)
                        {
                            var ins = new CRM_BH_DOANHSO_CT();
                            var productinvoice = new ProductInvoice();
                            ins.IsDeleted = false;
                            ins.CreatedUserId = WebSecurity.CurrentUserId;
                            ins.ModifiedUserId = WebSecurity.CurrentUserId;
                            ins.CreatedDate = DateTime.Now;
                            ins.ModifiedDate = DateTime.Now;
                            ins.THANG = q.THANG;
                            ins.NAM = q.NAM;
                            ins.KHACHHANG_ID = item.CustomerId;
                            ins.MA_DONHANG = item.Code;
                            ins.TONG_TIEN = item.TotalDebit;
                            ins.DA_TRA = item.TotalCredit;
                            ins.CON_NO = item.TongConNo;
                            ins.BranchId = (intBrandID == null ? Convert.ToInt32(Session["GlobalCurentBranchId"]) : intBrandID);
                            ins.GHI_CHU = item.GHI_CHU;
                            ins.TYLE_THANHCONG = 0;
                            ins.TYLE_THANHCONG_REVIEW = 0;
                            ins.KH_BANHANG_DOANHSO_ID = model.KH_BANHANG_DOANHSO_ID;
                            ins.NGAY_MUA = item.CreatedDate.ToString();
                            CRM_BH_DOANHSO_CTRepository.InsertCRM_BH_DOANHSO_CTIET(ins);
                        }
                    }
                }
                TempData[Globals.SuccessMessageKey] = App_GlobalResources.Wording.InsertSuccess;
                return RedirectToAction("_ClosePopup", "Home", new { area = "", FunctionCallback = "ClosePopupAndReloadPage" });
            }
            catch (Exception ex)
            {
                return View(model);
            }

        }
        #endregion
        #region KETHUA
        [AllowAnonymous]
        [HttpPost]
        public ActionResult KETHUA(CRM_BH_DOANHSOViewModel model, int? KH_BANHANG_DOANHSO_ID, string CountForBrand, int? NGUOILAP_ID)
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

            DateTime date = DateTime.Parse(model.month);
            date = date.AddMonths(-1);
            //string THANGNAM = date.ToString("MM/yyyy");
            //int THANG = int.Parse(THANGNAM.Substring(0, 2));
            //int NAM = int.Parse(THANGNAM.Substring(2,5));
            int THANG = date.Month;
            int NAM = date.Year;
            IEnumerable<CRM_BH_DOANHSOViewModel> q = CRM_BH_DOANHSORepository.GetAllCRM_BH_DOANHSO().Select(x => new CRM_BH_DOANHSOViewModel
            {
                KH_BANHANG_DOANHSO_ID = x.KH_BANHANG_DOANHSO_ID,
                BranchId = x.BranchId,
                THANG = x.THANG,
                NAM = x.NAM,
                NGUOILAP_ID = x.NGUOILAP_ID,
                CountForBrand = x.CountForBrand,
                TARGET_BRAND = x.TARGET_BRAND,
                GHI_CHU = x.GHI_CHU,
                IsDeleted = x.IsDeleted,
                CreatedDate = x.CreatedDate,
                CreatedUserId = x.CreatedUserId,
                ModifiedDate = x.ModifiedDate,
                ModifiedUserId = x.ModifiedUserId
            }).Where(x => x.THANG == THANG && x.NAM == NAM && x.CountForBrand == model.CountForBrand && x.NGUOILAP_ID == model.NGUOILAP_ID).ToList();
            model.CRM_BH_DOANHSO_CTList = CRM_BH_DOANHSORepository.GetAllvwCRM_BH_DOANHSO_CT().Select(x => new CRM_BH_DOANHSO_CTViewModel
            {
                KH_BANHANG_DOANHSO_CTIET_ID = x.KH_BANHANG_DOANHSO_CTIET_ID,
                BranchId = x.BranchId,
                KH_BANHANG_DOANHSO_ID = x.KH_BANHANG_DOANHSO_ID,
                THANG = x.THANG,
                NAM = x.NAM,
                MA_DONHANG = x.MA_DONHANG,
                NGAY_MUA = x.NGAY_MUA,
                TONG_TIEN = x.TONG_TIEN,
                DA_TRA = x.DA_TRA,
                CON_NO = x.CON_NO,
                KHACHHANG_ID = x.KHACHHANG_ID,
                NOIDUNG = x.NOIDUNG,
                TYLE_THANHCONG = x.TYLE_THANHCONG,
                TYLE_THANHCONG_REVIEW = x.TYLE_THANHCONG_REVIEW,
                GHI_CHU = x.GHI_CHU,
                IsDeleted = x.IsDeleted,
                CreatedDate = x.CreatedDate,
                CreatedUserId = x.CreatedUserId,
                ModifiedDate = x.ModifiedDate,
                ModifiedUserId = x.ModifiedUserId,
                CustomerName = x.CustomerName,
                CustomerCode = x.CustomerCode,
                Phone = x.Phone,
                CountForBrand = x.CountForBrand,
                NGUOILAP_ID = x.NGUOILAP_ID

            }).Where(x => x.THANG == THANG && x.NAM == NAM && x.CountForBrand == model.CountForBrand && x.TYLE_THANHCONG < 100 && x.TYLE_THANHCONG_REVIEW > -1 && x.NGUOILAP_ID == model.NGUOILAP_ID ).ToList();

            if (KH_BANHANG_DOANHSO_ID != null)
            {
                var KH_BANHANG_DOANHSO = CRM_BH_DOANHSORepository.GetCRM_BH_DOANHSOById(KH_BANHANG_DOANHSO_ID.Value);
                foreach (var item in model.CRM_BH_DOANHSO_CTList)
                {
                    IEnumerable<CRM_BH_DOANHSO_CTViewModel> CRM_BH_DOANHSO_CT = CRM_BH_DOANHSORepository.GetAllvwCRM_BH_DOANHSO_CT().Select(x => new CRM_BH_DOANHSO_CTViewModel
                    {
                        KH_BANHANG_DOANHSO_CTIET_ID = x.KH_BANHANG_DOANHSO_CTIET_ID,
                        BranchId = x.BranchId,
                        KH_BANHANG_DOANHSO_ID = x.KH_BANHANG_DOANHSO_ID,
                        THANG = x.THANG,
                        NAM = x.NAM,
                        MA_DONHANG = x.MA_DONHANG,
                        NGAY_MUA = x.NGAY_MUA,
                        TONG_TIEN = x.TONG_TIEN,
                        DA_TRA = x.DA_TRA,
                        CON_NO = x.CON_NO,
                        KHACHHANG_ID = x.KHACHHANG_ID,
                        NOIDUNG = x.NOIDUNG,
                        TYLE_THANHCONG = x.TYLE_THANHCONG,
                        TYLE_THANHCONG_REVIEW = x.TYLE_THANHCONG_REVIEW,
                        GHI_CHU = x.GHI_CHU,
                        IsDeleted = x.IsDeleted,
                        CreatedDate = x.CreatedDate,
                        CreatedUserId = x.CreatedUserId,
                        ModifiedDate = x.ModifiedDate,
                        ModifiedUserId = x.ModifiedUserId,
                        CustomerName = x.CustomerName,
                        CustomerCode = x.CustomerCode,
                        Phone = x.Phone,
                        CountForBrand = x.CountForBrand,
                    }).Where(x => x.CountForBrand == model.CountForBrand && x.KH_BANHANG_DOANHSO_ID == KH_BANHANG_DOANHSO_ID && x.MA_DONHANG == item.MA_DONHANG /*x.KHACHHANG_ID == item.KHACHHANG_ID*/).ToList();
                    //sửa lại vì kế thừa theo đơn hàng, ko phải khách hàng

                    if (CRM_BH_DOANHSO_CT.Count() == 0)
                    {
                        var ins = new CRM_BH_DOANHSO_CT();
                        ins.IsDeleted = false;
                        ins.CreatedUserId = WebSecurity.CurrentUserId;
                        ins.ModifiedUserId = WebSecurity.CurrentUserId;
                        ins.CreatedDate = DateTime.Now;
                        ins.ModifiedDate = DateTime.Now;
                        ins.THANG = KH_BANHANG_DOANHSO.THANG;
                        ins.NAM = KH_BANHANG_DOANHSO.NAM;
                        ins.KHACHHANG_ID = item.KHACHHANG_ID;
                        ins.BranchId = intBrandID;
                        ins.KH_BANHANG_DOANHSO_ID = KH_BANHANG_DOANHSO_ID;
                        ins.GHI_CHU = item.GHI_CHU;
                        ins.TYLE_THANHCONG = item.TYLE_THANHCONG;
                        ins.TYLE_THANHCONG_REVIEW = item.TYLE_THANHCONG_REVIEW;
                        ins.MA_DONHANG = item.MA_DONHANG;
                        ins.NGAY_MUA = item.NGAY_MUA;
                        ins.TONG_TIEN = item.TONG_TIEN;
                        ins.DA_TRA = item.DA_TRA;
                        ins.CON_NO = item.CON_NO;
                        CRM_BH_DOANHSORepository.InsertCRM_BH_DOANHSO_CT(ins);
                    }
                }
                TempData[Globals.SuccessMessageKey] = App_GlobalResources.Wording.UpdateSuccess;
                return RedirectToAction("Create");
            }
            else
            {
                using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.Snapshot }))
                {
                    try
                    {
                        var CRM_BH_DOANHSO = new CRM_BH_DOANHSO();
                        foreach (var item in q)
                        {
                            CRM_BH_DOANHSO.IsDeleted = false;
                            CRM_BH_DOANHSO.CreatedUserId = WebSecurity.CurrentUserId;
                            CRM_BH_DOANHSO.ModifiedUserId = WebSecurity.CurrentUserId;
                            CRM_BH_DOANHSO.CreatedDate = DateTime.Now;
                            CRM_BH_DOANHSO.ModifiedDate = DateTime.Now;
                            CRM_BH_DOANHSO.THANG = int.Parse(model.month.Substring(0, 1));
                            CRM_BH_DOANHSO.NAM = int.Parse(model.month.Substring(2, 4));
                            CRM_BH_DOANHSO.BranchId = intBrandID;
                            CRM_BH_DOANHSO.CountForBrand = model.CountForBrand;
                            CRM_BH_DOANHSO.TARGET_BRAND = item.TARGET_BRAND;
                            CRM_BH_DOANHSO.NGUOILAP_ID = model.NGUOILAP_ID;
                            CRM_BH_DOANHSORepository.InserCRM_BH_DOANHSO(CRM_BH_DOANHSO);
                        }

                        foreach (var item in model.CRM_BH_DOANHSO_CTList)
                        {
                            var ins = new CRM_BH_DOANHSO_CT();
                            ins.IsDeleted = false;
                            ins.CreatedUserId = WebSecurity.CurrentUserId;
                            ins.ModifiedUserId = WebSecurity.CurrentUserId;
                            ins.CreatedDate = DateTime.Now;
                            ins.ModifiedDate = DateTime.Now;
                            ins.THANG = CRM_BH_DOANHSO.THANG;
                            ins.NAM = CRM_BH_DOANHSO.NAM;
                            ins.KHACHHANG_ID = item.KHACHHANG_ID;
                            ins.MA_DONHANG = item.MA_DONHANG;
                            ins.TONG_TIEN = item.TONG_TIEN;
                            ins.DA_TRA = item.DA_TRA;
                            ins.CON_NO = item.CON_NO;
                            ins.BranchId = intBrandID;
                            ins.GHI_CHU = item.GHI_CHU;
                            ins.TYLE_THANHCONG = item.TYLE_THANHCONG;
                            ins.TYLE_THANHCONG_REVIEW = item.TYLE_THANHCONG_REVIEW;
                            ins.KH_BANHANG_DOANHSO_ID = CRM_BH_DOANHSO.KH_BANHANG_DOANHSO_ID;
                            ins.NGAY_MUA = item.CreatedDate.ToString();
                            CRM_BH_DOANHSO_CTRepository.InsertCRM_BH_DOANHSO_CTIET(ins);
                        }
                        scope.Complete();
                        TempData[Globals.SuccessMessageKey] = App_GlobalResources.Wording.UpdateSuccess;
                        return RedirectToAction("Index");
                    }
                    catch (Exception ex) { }
                }
            }

            TempData[Globals.SuccessMessageKey] = App_GlobalResources.Wording.UpdateSuccess;
            return RedirectToAction("Index");
        }
        #endregion
        #region Delete
        [AllowAnonymous]
        [HttpPost]
        public JsonResult Delete(int KH_BANHANG_DOANHSO_CTIET_ID)
        {
            try
            {
                var item = CRM_BH_DOANHSORepository.GetCRM_BH_DOANHSO_CTById(Helpers.Common.NVL_NUM_NT_NEW(Erp.BackOffice.Helpers.Common.CurrentUser.BranchId), KH_BANHANG_DOANHSO_CTIET_ID);
                if (item != null)
                {
                    item.IsDeleted = true;
                    CRM_BH_DOANHSORepository.UpdateCRM_BH_DOANHSO_CT(item);
                }
                TempData[Globals.SuccessMessageKey] = App_GlobalResources.Wording.DeleteSuccess;
                return Json(true);
            }
            catch (DbUpdateException)
            {
                TempData[Globals.FailedMessageKey] = App_GlobalResources.Error.RelationError;
                return Json(false);
            }
        }
        #endregion
        #region Cập nhật tỉ lệ thành công
        [AllowAnonymous]
        [HttpPost]
        public JsonResult UpdateRate(int id, int khid, int tyle, int tylerw)
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

            var CRM_BH_DOANHSO = CRM_BH_DOANHSORepository.GetCRM_BH_DOANHSO_CTById(Helpers.Common.NVL_NUM_NT_NEW(intBrandID), id);
            //AutoMapper.Mapper.Map(item, CRM_KH_BANHANG_CT);
            CRM_BH_DOANHSO.TYLE_THANHCONG = tyle;
            CRM_BH_DOANHSO.TYLE_THANHCONG_REVIEW = tylerw;
            CRM_BH_DOANHSO.ModifiedDate = DateTime.Now;
            CRM_BH_DOANHSO.ModifiedUserId = WebSecurity.CurrentUserId;
            CRM_BH_DOANHSORepository.UpdateCRM_BH_DOANHSO_CT(CRM_BH_DOANHSO);
            return Json(1);
        }
        #endregion
        #region XoaKH
        [HttpPost]
        public ActionResult DeleteKH(int? month, int? year)
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
            string idDeleteAll = Request["DeleteId-checkbox"];
            //string gdall = Request["selectgd"];
            string[] arrDeleteId = idDeleteAll.Split(',');

            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.Snapshot }))
            {
                for (int i = 0; i < arrDeleteId.Count(); i++)
                {
                    var id = int.Parse(arrDeleteId[i], CultureInfo.InvariantCulture);
                    var kh_banhang = CRM_BH_DOANHSORepository.GetAllCRM_BH_DOANHSO().Where(x => x.NAM == year &&
                    x.THANG == month && x.NGUOILAP_ID == id && x.BranchId == intBrandID).ToList();
                    if (kh_banhang.Count > 0)
                    {
                        foreach (var item in kh_banhang)
                        {
                            var chitiet = CRM_BH_DOANHSORepository.GetAllvwCRM_BH_DOANHSO_CT().Where(x => x.KH_BANHANG_DOANHSO_ID == item.KH_BANHANG_DOANHSO_ID).ToList();
                            if (chitiet.Count > 0)
                            {
                                foreach (var item2 in chitiet)
                                {
                                    CRM_BH_DOANHSORepository.DeleteCRM_BH_DOANHSO_CTRs(item2.KH_BANHANG_DOANHSO_CTIET_ID,intBrandID.Value);
                                }
                            }
                            CRM_BH_DOANHSORepository.DeleteCRM_BH_DOANHSORs(item.KH_BANHANG_DOANHSO_ID);
                        }
                    }


                }

                scope.Complete();
            }
            TempData[Globals.SuccessMessageKey] = App_GlobalResources.Wording.UpdateSuccess;
            return RedirectToAction("Index");
        }
        #endregion
    }





}
