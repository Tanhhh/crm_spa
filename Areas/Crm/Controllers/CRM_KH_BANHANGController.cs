using Erp.BackOffice.Filters;
using Erp.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using WebMatrix.WebData;
using Erp.Domain.Crm.Interfaces;
using Erp.BackOffice.Areas.Cms.Models;
using Erp.BackOffice.Crm.Models;
using Erp.Domain.Crm.Entities;
using System.Globalization;
using Erp.BackOffice.Sale.Models;
using Erp.BackOffice.Account.Models;
using Erp.Domain.Entities;
using Erp.Domain.Sale.Entities;
using Erp.Domain.Sale.Interfaces;
using System.Data.Entity.Infrastructure;
using System.Text;
using Erp.BackOffice.Helpers;
using System.Transactions;
using System.Text.RegularExpressions;
using Erp.Domain.Account.Interfaces;
using Erp.Utilities;
using System.Data.Entity.Validation;
using PagedList;
using PagedList.Mvc;
using Erp.BackOffice.Areas.Administration.Models;
using Erp.BackOffice.App_GlobalResources;
using Erp.BackOffice.Account.Controllers;
using Erp.Domain.Account.Helper;
using System.Data.Entity;
using System.Web;
using System.Data;
using System.Security.Cryptography;
using Erp.Domain.Staff.Entities;
using Erp.Domain.Staff.Interfaces;
using Erp.BackOffice.Staff.Models;

namespace Erp.BackOffice.Crm.Controllers
{
    [Authorize]
    [InitializeSimpleMembership]
    [Erp.BackOffice.Helpers.NoCacheHelper]
    public class CRM_KH_BANHANGController : Controller
    {
        private readonly ICRM_BH_DOANHSORepository CRM_BH_DOANHSORepository;
        private readonly ICRM_KH_GIOITHIEURepository CRM_KH_GIOITHIEURepository;
        private readonly ICategoryRepository categoryRepository;
        private readonly ICustomerRepository CustomerRepository;
        private readonly IUserRepository userRepository;
        private readonly ICRM_KH_BANHANGRepository CRM_KH_BANHANGRepository;
        private readonly ICRM_KH_BANHANG_CTIETRepository CRM_KH_BANHANG_CTIETRepository;
        private readonly IProductInvoiceRepository ProductInvoiceRepository;
        private readonly ITransactionLiabilitiesRepository transactionLiabilitiesRepository;
        private readonly IKH_TUONGTACRepository kH_TUONGTACRepository;
        private readonly ITemplatePrintRepository templatePrintRepository;
        private readonly ICrm_PeriodRepository Crm_PeriodRepository;
        private readonly ICrm_TargetRepository Crm_TargetRepository;
        private readonly ICrm_LevelRepository Crm_LevelRepository;
        private readonly IBranchRepository BranchRepository;
        private readonly ICrm_KhachHenBitrixRepository Crm_KhachHenBitrixRepository;
        public CRM_KH_BANHANGController(
            ICRM_BH_DOANHSORepository _CRM_BH_DOANHSO,
            ICRM_KH_BANHANGRepository _CRM_KH_BANHANG,
            ICategoryRepository _category,
            ICustomerRepository _Customer,
            IUserRepository _userRepository,
            ICRM_KH_BANHANG_CTIETRepository _CRM_KH_BANHANG_CTIET,
            IProductInvoiceRepository _ProductInvoiceRepository,
            ITransactionLiabilitiesRepository _transactionLiabilities,
            IKH_TUONGTACRepository _kH_TUONGTAC,
            ITemplatePrintRepository _templatePrintRepository,
             ICrm_PeriodRepository Crm_Period,
             ICrm_TargetRepository Crm_Target,
             ICrm_LevelRepository Crm_Level,
             IBranchRepository Branch,
             ICrm_KhachHenBitrixRepository KhachHenBitrixRepository
            )
        {
            CRM_BH_DOANHSORepository = _CRM_BH_DOANHSO;
            CRM_KH_BANHANGRepository = _CRM_KH_BANHANG;
            categoryRepository = _category;
            CustomerRepository = _Customer;
            userRepository = _userRepository;
            CRM_KH_BANHANG_CTIETRepository = _CRM_KH_BANHANG_CTIET;
            ProductInvoiceRepository = _ProductInvoiceRepository;
            transactionLiabilitiesRepository = _transactionLiabilities;
            kH_TUONGTACRepository = _kH_TUONGTAC;
            templatePrintRepository = _templatePrintRepository;
            Crm_PeriodRepository = Crm_Period;
            Crm_TargetRepository = Crm_Target;
            Crm_LevelRepository = Crm_Level;
            BranchRepository = Branch;
            Crm_KhachHenBitrixRepository = KhachHenBitrixRepository;
        }
        #region Index

        public ViewResult Index(int? branchId, int? Month, int? year, int? NGUOILAP, int? checkSPSHH, string IsTHucte)
        {
            //get cookie brachID 
            HttpRequestBase request = this.HttpContext.Request;
            var u = userRepository.GetUserById(WebSecurity.CurrentUserId);
            string strBrandID = "0";
            //ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
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
            branchId = intBrandID;
            int? GlobalCurentBranchId = Convert.ToInt32(Session["GlobalCurentBranchId"]);
            //branchId = (Erp.BackOffice.Helpers.Common.CurrentUser.BranchId == null ? GlobalCurentBranchId : Erp.BackOffice.Helpers.Common.CurrentUser.BranchId);
            year = year == null ? DateTime.Now.Year : year;
            Month = Month == null ? DateTime.Now.Month : Month;
            //NGUOILAP = NGUOILAP == null ? WebSecurity.CurrentUserId : NGUOILAP;
            var dataNhanvien = SqlHelper.QuerySP<ManagerStaff>("spgetNhanvienbacap", new
            {
                BranchId = (branchId == null ? 0 : branchId),
                UserId = WebSecurity.CurrentUserId,
            }).ToList();

            List<int> listNhanvien = new List<int>();


            for (int i = 0; i < dataNhanvien.Count; i++)
            {
                listNhanvien.Add(int.Parse(dataNhanvien[i].Id.ToString()));
            }

            ViewBag.KHsumANNA = 0;
            ViewBag.KHsumCNC = 0;
            ViewBag.KHsumLEO = 0;
            ViewBag.KHsumDV = 0;
            ViewBag.KHsumOP = 0;




            var q = CRM_KH_BANHANGRepository.GetAllvwCRM_KH_BANHANG();
            var user = userRepository.GetAllUsers().Select(item => new UserViewModel
            {
                Id = item.Id,
                UserName = item.UserName,
                FullName = item.FullName,
                Status = item.Status,
                BranchId = item.BranchId,
                UserTypeId = item.UserTypeId
            }).Where(x => x.Status == UserStatus.Active && x.UserTypeId == 2026).ToList();
            //if (branchId > 0)
            //{
            //    ViewBag.user2 = user.Where(x => x.BranchId == branchId).ToList();
            //}

            //&& listNhanvien.Contains(x.Id)
            var user2branch = user.Where(x => x.Id == WebSecurity.CurrentUserId).FirstOrDefault();

            if (branchId != null && branchId > 0)
            {
                q = q.Where(x => x.BranchId == branchId);
                user = user.Where(x => x.BranchId == branchId).ToList();
                if (user2branch != null && user2branch.BranchId == 0)
                {
                    user.Add(user2branch);
                }
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

            if (!Filters.SecurityFilter.IsAdmin() && u.IsLetan != true && (u.UserTypeId != 1027 || u.UserTypeId != 2053))
            {
                user = user.Where(x => x.Id != 0 && listNhanvien.Contains(x.Id)).ToList();
            }
            if (Month != null)
            {
                if (year != null)
                {
                    q = q.Where(x => x.THANG == Month && x.NAM == year);
                }
            }

            var origin = categoryRepository.GetAllCategories().Select(item => new CategoryViewModel
            {
                OrderNo = item.OrderNo,
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


            List<decimal?> khsum = new List<decimal?>();
            for (int i = 0; i < count; i++)
            {
                khsum.Add(0);
            }
            if (IsTHucte == "on")
            {
                var dataTongThucTe = SqlHelper.QuerySP<CRM_KH_BANHANGViewModel>("CrmListTongThucTe", new
                {
                    THANG = Month,
                    NAM = year,
                }).ToList();
                //ViewBag.BANHANG_CTIET_ProductInvoice = dataTongThucTe;
                dataTongThucTe = dataTongThucTe.Where(x => listNhanvien.Contains(x.NGUOILAP_ID.Value)).ToList();
                ViewBag.productInvoice = dataTongThucTe;
                TempData["Data_TongThucTe"] = dataTongThucTe;


                if (user != null)
                {
                    foreach (var s in dataTongThucTe)
                    {

                        if (s.TotalAmount > 0)
                        {
                            for (int i = 0; i < count; i++)
                            {
                                if ((Helpers.Common.ChuyenThanhKhongDau(s.CountForBrand).Replace(" ", "")).Equals(Helpers.Common.ChuyenThanhKhongDau(ten[i]).Replace(" ", "")))
                                {
                                    var sum = s.TotalAmount;
                                    khsum[i] += sum;
                                }
                            }
                        }
                    }
                }
            }
            List<CRM_KH_BANHANG_CTIETViewModel> CrmProductInvoice = SqlHelper.QuerySP<CRM_KH_BANHANG_CTIETViewModel>("TOIUUCSKH_CrmProductInvoice", new { @pTHANG =Month, @pNAM =year}).ToList();
            ViewBag.CrmProductInvoice = CrmProductInvoice;

            var model = q.Select(item => new CRM_KH_BANHANGViewModel
            {
                KH_BANHANG_ID = item.KH_BANHANG_ID,
                CreatedUserId = item.CreatedUserId,
                CreatedDate = item.CreatedDate,
                ModifiedUserId = item.ModifiedUserId,
                ModifiedDate = item.ModifiedDate,
                NAM = item.NAM,
                THANG = item.THANG,
                NGUOILAP_ID = item.NGUOILAP_ID,
                CountForBrand = item.CountForBrand,
                TARGET_BRAND = item.TARGET_BRAND,
                GHI_CHU = item.GHI_CHU,
                Name = item.Name,
                FullName = item.FullName,
                BranchId = item.BranchId
            }).OrderBy(x => x.FullName).ToList();

            List<int?> listUser = new List<int?>();
            foreach (var a in user)
            {
                listUser.Add(a.Id);
            }
            model = model.Where(x => listUser.Contains(x.NGUOILAP_ID)).ToList();
            //&& listNhanvien.Contains(x.NGUOILAP_ID.Value)

            //if (listNhanvien.Count() > 0)
            //{
            //    model = model.Where(x => listNhanvien.Contains(x.NGUOILAP_ID.Value)).ToList();
            //}
            ViewBag.TarsumANNA = 0;
            ViewBag.TarsumCNC = 0;
            ViewBag.TarsumLEO = 0;
            ViewBag.TarsumDV = 0;
            ViewBag.TarsumOP = 0;
            //

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
                                tarsum[i] += sum;
                            }
                        }
                    }
                }
            }
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

            List<decimal?> Kehoach = new List<decimal?>();
            //for (int i = 0; i < count; i++)
            //{
            //    //foreach (var item in user.OrderBy(x => x.UserName))
            //{ 
            foreach (var a in origin)
            {
                var list = CrmProductInvoice.Where(x => x.CountForBrand == a.Value).ToList();

                if (list.Count() > 0)
                {
                    //        < td hidden id = "KH-@(index1 - 1)-@a.Value.Replace(" ", "")" class="text-right">
                    //            <a>

                    decimal tb = (decimal)(list.Sum(b => b.TotalAmount));

                    Kehoach.Add(tb);
                    //            </a>
                    //        </td>
                    //    }
                    //    else
                    //    {
                    //        <td hidden id="KH-@(index1 - 1)-@a.Value.Replace(" ", "")" class="text-right"><a>0</a></td>
                    //    }
                    //}
                }
                else
                    Kehoach.Add(0);
            }

            for (int i = 0; i < tarsum.Count; i++)
            {
                if (Kehoach[i] >= tarsum[i] && Kehoach[i] > 0 && tarsum[i] > 0)
                    Kehoach[i] = 100;
                else
                {
                    if (Kehoach[i] < tarsum[i] && Kehoach[i] > 0 && tarsum[i] > 0)
                        Kehoach[i] = Kehoach[i] * 100 / tarsum[i];
                    else
                        Kehoach[i] = 0;
                }
            }
            //}
            //}
            //if (NGUOILAP != null && NGUOILAP > 0)
            //{
            //    model = model.Where(x => x.NGUOILAP_ID == NGUOILAP).ToList();
            //    user = user.Where(x => x.Id == NGUOILAP).ToList();
            //}
            ViewBag.khsum = khsum;
            ViewBag.tarsum = tarsum;
            ViewBag.tiendo = tiendo;
            ViewBag.user = user;
            ViewBag.kehoach = Kehoach;
            ViewBag.NGUOILAP = WebSecurity.CurrentUserId;
            ViewBag.SuccessMessage = TempData["SuccessMessage"];
            ViewBag.FailedMessage = TempData["FailedMessage"];
            ViewBag.AlertMessage = TempData["AlertMessage"];
            ViewBag.AdviseTypeError = TempData["AdviseTypeError"];

            TempData["Data_KHBanHang"] = model;
            TempData["Data_KHBanHang_User"] = user;
            TempData["Data_KHBanHang_origin"] = origin;
            TempData["Data_KeHoach"] = CrmProductInvoice;
            TempData["Data_Khsum"] = khsum;
            TempData["Data_Tarsum"] = tarsum;
            TempData["Data_TienDo"] = tiendo;

            return View(model);
        }

        public ActionResult PrintKHBanHang(int? Year, string txtCode, string txtMinAmount, string txtMaxAmount, string txtCusName, string startDate, string endDate, int? BranchId, string txtCustomerCode, string countForBrand, int? userTypeId, string GDNDT, bool ExportExcel = false)
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
            var data = TempData["Data_KHBanHang"] as List<CRM_KH_BANHANGViewModel>;
            TempData.Keep();
            var dataUser = TempData["Data_KHBanHang_User"] as List<UserViewModel>;
            TempData.Keep();
            var dataOrigin = TempData["Data_KHBanHang_origin"] as List<CategoryViewModel>;
            TempData.Keep();
            var dataThucTe = TempData["Data_TongThucTe"] as List<CRM_KH_BANHANGViewModel>;
            TempData.Keep();
            var dataKeHoach = TempData["Data_KeHoach"] as List<CRM_KH_BANHANG_CTIETViewModel>;
            TempData.Keep();
            var khsum = TempData["Data_Khsum"] as List<decimal?>; TempData.Keep();
            var tarsum = TempData["Data_Tarsum"] as List<decimal?>; TempData.Keep();
            var tiendo = TempData["Data_TienDo"] as List<string>; TempData.Keep();
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
            model.Content = model.Content.Replace("{DataTable}", buildHtmlKHBanHang(data, dataUser, dataOrigin, dataThucTe, dataKeHoach, khsum, tarsum, tiendo));
            model.Content = model.Content.Replace("{System.Logo}", ImgLogo);
            model.Content = model.Content.Replace("{System.CompanyName}", company);
            model.Content = model.Content.Replace("{System.AddressCompany}", address);
            model.Content = model.Content.Replace("{System.PhoneCompany}", phone);
            model.Content = model.Content.Replace("{System.Fax}", fax);
            model.Content = model.Content.Replace("{System.BankCodeCompany}", bankcode);
            model.Content = model.Content.Replace("{System.BankNameCompany}", bankname);
            model.Content = model.Content.Replace("{PrintDate}", DateTime.Now.ToString("dd/MM/yyyy HH:mm"));
            model.Content = model.Content.Replace("{Title}", "Kế hoạch bán hàng");
            if (ExportExcel)
            {
                Response.AppendHeader("content-disposition", "attachment;filename=" + "KH_BanHang" + DateTime.Now.ToString("yyyyMMdd") + ".xls");
                Response.Charset = "";
                Response.Cache.SetCacheability(HttpCacheability.NoCache);
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.Write(model.Content);
                Response.End();
            }
            return View(model);
        }

        string buildHtmlKHBanHang(List<CRM_KH_BANHANGViewModel> detailList, List<UserViewModel> user, List<CategoryViewModel> origin, List<CRM_KH_BANHANGViewModel> thucTe, List<CRM_KH_BANHANG_CTIETViewModel> keHoach, List<decimal?> khsum, List<decimal?> tarsum, List<string> tienDo)
        {
            string detailLists = "<table border=\"1\" class=\"invoice-detail\">\r\n";
            detailLists += "<thead>\r\n";
            detailLists += "	<tr>";
            detailLists += "		<th rowspan=\"2\">STT</th>";
            //detailLists += "		<th>Trạng thái</th>";
            detailLists += "		<th rowspan=\"2\">Nhân viên lập</th>";
            // detailLists += "		<th>Người tạo</th>";
            detailLists += "		<th colspan=\"5\">Target</th>";
            detailLists += "		<th colspan=\"5\">Thực tế</th>";
            detailLists += "		<th colspan=\"5\">Kế hoạch(%)</th>";
            detailLists += "		<th colspan=\"5\">Tiến độ(%)</th>";
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

                    if (thucTe != null)
                    {
                        var listThucTe = thucTe.Where(x => x.CountForBrand == a.Value && x.NGUOILAP_ID == item.Id);

                        if (listThucTe != null && list.Count() > 0)
                        {

                            total = list.Sum(b => b.TotalAmount);
                            thucte.Add(CommonSatic.ToCurrencyStr(list.Sum(b => b.TotalAmount), null));

                        }
                        else
                        {
                            total = 0;
                            thucte.Add("0");
                        }
                    }
                    else
                    {
                        total = 0;
                        thucte.Add("0");
                    }

                    var listKH = keHoach.Where(x => x.CountForBrand == a.Value && x.NGUOILAP_ID == item.Id).ToList();

                    if (listKH.Count() > 0)
                    {
                        kehoach.Add(CommonSatic.ToCurrencyStr(listKH.Sum(b => b.TotalAmount), null));
                    }
                    else
                        kehoach.Add("0");
                    if (total > 0)
                    {
                        decimal td = (decimal)(total * 100 / target);
                        tiendo.Add(Math.Round(td, 1).ToString().Replace(",", "."));
                    }
                    else
                        tiendo.Add("0.0");
                }
                detailLists += "<tr>\r\n"
                + "<td class=\"text-center orderNo\">" + (index++) + "</td>\r\n"
                + "<td class=\"text-left \">" + item.FullName + " </td>\r\n"
                + "<td class=\"text-left \">" + sum[0] + " </td>\r\n"
                + "<td class=\"text-left \">" + sum[1] + " </td>\r\n"
                + "<td class=\"text-left \">" + sum[2] + " </td>\r\n"
                + "<td class=\"text-left \">" + sum[3] + " </td>\r\n"
                + "<td class=\"text-left \">" + sum[4] + " </td>\r\n"
                + "<td class=\"text-left \">" + thucte[0] + " </td>\r\n"
                + "<td class=\"text-left \">" + thucte[1] + " </td>\r\n"
                + "<td class=\"text-left \">" + thucte[2] + " </td>\r\n"
                + "<td class=\"text-left \">" + thucte[3] + " </td>\r\n"
                + "<td class=\"text-left \">" + thucte[4] + " </td>\r\n"
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
            detailLists += "<tr><td colspan=\"2\" class=\"text-right\">Tổng cộng</td>"
                         + "<td class=\"text-left \">" + CommonSatic.ToCurrencyStr(tarsum[0], null) + " </td>\r\n"
                         + "<td class=\"text-left \">" + CommonSatic.ToCurrencyStr(tarsum[1], null) + " </td>\r\n"
                         + "<td class=\"text-left \">" + CommonSatic.ToCurrencyStr(tarsum[2], null) + " </td>\r\n"
                         + "<td class=\"text-left \">" + CommonSatic.ToCurrencyStr(tarsum[3], null) + " </td>\r\n"
                         + "<td class=\"text-left \">" + CommonSatic.ToCurrencyStr(tarsum[4], null) + " </td>\r\n"
                         + "<td class=\"text-left \">" + CommonSatic.ToCurrencyStr(khsum[0], null) + " </td>\r\n"
                         + "<td class=\"text-left \">" + CommonSatic.ToCurrencyStr(khsum[1], null) + " </td>\r\n"
                         + "<td class=\"text-left \">" + CommonSatic.ToCurrencyStr(khsum[2], null) + " </td>\r\n"
                         + "<td class=\"text-left \">" + CommonSatic.ToCurrencyStr(khsum[3], null) + " </td>\r\n"
                         + "<td class=\"text-left \">" + CommonSatic.ToCurrencyStr(khsum[4], null) + " </td>\r\n"
                         + "<td colspan=\"5\" class=\"text-left \"> </td>\r\n"
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
        [AllowAnonymous]
        public ViewResult DetailPlan(int? CreateUserId, int? Month, int? Year, string CountForBrand)
        {
            //Lấy đơn hoàn thành trong tháng
            //var ProductInvoice = ProductInvoiceRepository.GetAllvwProductInvoice()
            //   .Where(x => x.CountForBrand == CountForBrand && x.Month == Month && x.Year == Year && x.IsArchive == true && x.Status == "complete" && x.ManagerStaffId == CreateUserId)
            //   .Select(item => new ProductInvoiceViewModel
            //   {
            //       Id = item.Id,
            //       ManagerStaffId = item.ManagerStaffId,
            //       CountForBrand = item.CountForBrand,
            //       TotalAmount = item.TotalAmount,
            //       Code = item.Code,
            //       CustomerId = item.CustomerId,
            //       CreatedDate = item.CreatedDate,
            //       Month = item.CreatedDate.Value.Month,
            //       Year = item.CreatedDate.Value.Year,
            //       IsArchive = item.IsArchive,
            //       TotalDebit = item.TotalDebit,
            //       TotalCredit = item.TotalCredit,
            //       Status = item.Status

            //   }).ToList();


            //Lấy 
            var model = new CRM_KH_BANHANGViewModel();

            model = SqlHelper.QuerySP<CRM_KH_BANHANGViewModel>("getInvoiceInKHBH", new
            {
                ManagerStaffId = CreateUserId,
                CountForBrand = CountForBrand,
                Month = Month,
                Year = Year,
                KHBANHANG_ID = 0,
                Check = 2
            }).FirstOrDefault();

            //model = CRM_KH_BANHANGRepository.GetAllvwCRM_KH_BANHANG()
            //    .Where(x => x.NGUOILAP_ID == CreateUserId && x.IsDeleted == false && x.THANG == Month && x.NAM == Year && x.CountForBrand == CountForBrand)
            //                .Select(item => new CRM_KH_BANHANGViewModel
            //                {
            //                    KH_BANHANG_ID = item.KH_BANHANG_ID,
            //                    BranchId = item.BranchId,
            //                    IsDeleted = item.IsDeleted,
            //                    CreatedDate = item.CreatedDate,
            //                    CreatedUserId = item.CreatedUserId,
            //                    ModifiedDate = item.ModifiedDate,
            //                    ModifiedUserId = item.ModifiedUserId,
            //                    NGUOILAP_ID = item.NGUOILAP_ID,
            //                    TARGET_BRAND = item.TARGET_BRAND,
            //                    GHI_CHU = item.GHI_CHU,
            //                    CountForBrand = item.CountForBrand,
            //                    THANG = item.THANG,
            //                    NAM = item.NAM,
            //                }).FirstOrDefault();
            if (model == null)
            {
                model = new CRM_KH_BANHANGViewModel();
                model.NGUOILAP_ID = CreateUserId;
                model.CountForBrand = CountForBrand;
                model.KH_BANHANG_ID = 0;
            }
            var user = userRepository.GetUserById(CreateUserId.Value);
            // model.NGUOILAP_ID = user.Id;
            model.FullName = user.FullName;

            model.CRM_KH_BANHANGList = CRM_KH_BANHANGRepository.GetAllvwCRM_KH_BANHANG_CTIET()
               .Where(x => x.KH_BANHANG_ID == model.KH_BANHANG_ID)
               .Select(x => new CRM_KH_BANHANG_CTIETViewModel
               {
                   KH_BANHANG_ID = x.KH_BANHANG_ID,
                   KH_BANHANG_CTIET_ID = x.KH_BANHANG_CTIET_ID,
                   Phone = x.Phone,
                   KHACHHANG_ID = x.KHACHHANG_ID,
                   CompanyName = x.CompanyName,
                   Code = x.Code,
                   TYLE_THANHCONG = x.TYLE_THANHCONG,
                   TYLE_THANHCONG_REVIEW = x.TYLE_THANHCONG_REVIEW,
                   GHI_CHU = x.GHI_CHU,
                   IsDeleted = x.IsDeleted,
                   THANG = x.THANG,
                   NAM = x.NAM,
                   CRM_Sale_ProductInvoiceCode = x.CRM_Sale_ProductInvoiceCode,
                   CountForBrand = x.CountForBrand,
                   TotalAmount = x.TotalAmount,
                   ProductInvoiceId = x.ProductInvoiceId,
                   NGUOILAP_ID = x.NGUOILAP_ID,
                   NOIDUNG = x.NOIDUNG,
                   IsApprove = x.IsApprove,
                   UserApprove = x.UserApprove
               }).ToList();

            var ProductInvoice = SqlHelper.QuerySP<ProductInvoiceViewModel>("getInvoiceInKHBH", new
            {
                ManagerStaffId = CreateUserId,
                CountForBrand = CountForBrand,
                Month = Month,
                Year = Year,
                KHBANHANG_ID = model.KH_BANHANG_ID,
                Check = 1
            }).ToList();

            ViewBag.ProductInvoice = ProductInvoice;

            ViewBag.vwCRM_KH_BANHANG = model.CRM_KH_BANHANGList;
            ViewBag.model = model;

            //var BANHANG_CTIET_ProductInvoice = SqlHelper.QuerySP<CRM_KH_BANHANGViewModel>("spCrm_BANHANG_CTIET_ProductInvoice", new
            //{
            //    Month=month,
            //    Year=year,
            //    ManagerStaffId=id
            //}).ToList();
            //var dataTongThucTe = SqlHelper.QuerySP<CRM_KH_BANHANGViewModel>("CrmListTongThucTeByNGUOILAP_ID", new
            //{
            //    THANG = Month,
            //    NAM = Year,
            //    NGUOILAP_ID = CreateUserId
            //}).Where(x => x.CountForBrand == CountForBrand).ToList();
            //ViewBag.BANHANG_CTIET_ProductInvoice = dataTongThucTe;


            return View(model);
        }

        public ActionResult Create(int id, int month, int year, string sortOrder)
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

            //var ProductInvoice = ProductInvoiceRepository.GetAllvwProductInvoice()
            //    .Where(x => x.Month == month && x.Year == year && x.IsArchive == true && x.Status == "complete" && x.ManagerStaffId == id)
            //    .Select(item => new ProductInvoiceViewModel
            //{
            //    Id = item.Id,
            //    ManagerStaffId = item.ManagerStaffId,
            //    CountForBrand = item.CountForBrand,
            //    TotalAmount = item.TotalAmount,
            //    Code = item.Code,
            //    CustomerId = item.CustomerId,
            //    CreatedDate = item.CreatedDate,
            //    Month = item.CreatedDate.Value.Month,
            //    Year = item.CreatedDate.Value.Year,
            //    IsArchive = item.IsArchive,
            //    TotalDebit = item.TotalDebit,
            //    TotalCredit = item.TotalCredit,
            //    Status = item.Status

            //}).ToList();
            //ViewBag.ProductInvoice = ProductInvoice;
            //List<int?> KHID = new List<int?>();
            //for(int i =0; i < ProductInvoice.Count;i++ )
            //{
            //    KHID.Add(ProductInvoice[i].CustomerId);
            //}

            var model = new CRM_KH_BANHANGViewModel();
            model.NGUOILAP_ID = id;
            //IEnumerable<CRM_KH_BANHANGViewModel> q = CRM_KH_BANHANGRepository.GetAllvwCRM_KH_BANHANG()
            //    .Where(x => x.NGUOILAP_ID == id && x.IsDeleted == false && x.THANG == month && x.NAM == year)
            //                .Select(item => new CRM_KH_BANHANGViewModel
            //                {
            //                    KH_BANHANG_ID = item.KH_BANHANG_ID,
            //                    BranchId = item.BranchId,
            //                    IsDeleted = item.IsDeleted,
            //                    CreatedDate = item.CreatedDate,
            //                    CreatedUserId = item.CreatedUserId,
            //                    ModifiedDate = item.ModifiedDate,
            //                    ModifiedUserId = item.ModifiedUserId,
            //                    NGUOILAP_ID = item.NGUOILAP_ID,
            //                    TARGET_BRAND = item.TARGET_BRAND,
            //                    GHI_CHU = item.GHI_CHU,
            //                    CountForBrand = item.CountForBrand,
            //                    THANG = item.THANG,
            //                    NAM = item.NAM,
            //                }).ToList();

            //foreach (var item in q)
            //{
            //    model.BranchId = item.BranchId;
            //    model.IsDeleted = item.IsDeleted;
            //    model.CreatedDate = item.CreatedDate;
            //    model.CreatedUserId = item.CreatedUserId;
            //    model.ModifiedDate = item.ModifiedDate;
            //    model.ModifiedUserId = item.ModifiedUserId;
            //    model.CountForBrand = item.CountForBrand;
            //    model.GHI_CHU = item.GHI_CHU;
            //    if (item.CountForBrand == "ANNAYAKE")
            //    {
            //        model.ID_ANNAYAKE = item.KH_BANHANG_ID;
            //        model.TARGET_BRAND = item.TARGET_BRAND;
            //        ViewBag.GHICHU_ANNAYAKE = item.GHI_CHU;
            //        ViewBag.ID_ANNAYAKE = item.KH_BANHANG_ID;
            //        ViewBag.ANNAYAKE = item.TARGET_BRAND;
            //    }

            //    if (item.CountForBrand.Replace(" ", "") == "ORLANEPARIS")
            //    {
            //        model.ID_ORLANEPARIS = item.KH_BANHANG_ID;
            //        ViewBag.GHICHU_ORLANEPARIS = item.GHI_CHU;
            //        ViewBag.ID_ORLANEPARIS = item.KH_BANHANG_ID;
            //        ViewBag.ORLANEPARIS = item.TARGET_BRAND;
            //    }

            //    if (item.CountForBrand.Replace(" ", "") == "LEONORGREYL")
            //    {
            //        model.ID_LEONORGREYL = item.KH_BANHANG_ID;
            //        ViewBag.GHICHU_LEONORGREYL = item.GHI_CHU;
            //        ViewBag.ID_LEONORGREYL = item.KH_BANHANG_ID;
            //        ViewBag.LEONORGREYL = item.TARGET_BRAND;
            //    }

            //    if (item.CountForBrand.Replace(" ", "") == "CONGNGHECAO")
            //    {
            //        model.ID_CONGNGHECAO = item.KH_BANHANG_ID;
            //        ViewBag.GHICHU_CNC = item.GHI_CHU;
            //        ViewBag.ID_CNC = item.KH_BANHANG_ID;
            //        ViewBag.CNC = item.TARGET_BRAND;
            //    }


            //    if (item.CountForBrand.Replace(" ", "") == "DICHVU")
            //    {
            //        model.ID_DICHVU = item.KH_BANHANG_ID;
            //        ViewBag.GHICHU_DV = item.GHI_CHU;
            //        ViewBag.ID_DV = item.KH_BANHANG_ID;
            //        ViewBag.DV = item.TARGET_BRAND;
            //    }





            //}


            //if (id != null)
            //{
            //    var user = userRepository.GetUserById(id);
            //    model.NGUOILAP_ID = user.Id;
            //    model.FullName = user.FullName;
            //}
            //else
            //{
            //    var user = userRepository.GetUserById(WebSecurity.CurrentUserId);
            //    model.NGUOILAP_ID = user.Id;
            //    model.FullName = user.FullName;
            //}



            //model.CRM_KH_BANHANGList = CRM_KH_BANHANGRepository.GetAllvwCRM_KH_BANHANG_CTIET()
            //    .Where(x => x.IsDeleted != true && x.THANG == month && x.NAM == year && x.NGUOILAP_ID == id)
            //    .Select(x => new CRM_KH_BANHANG_CTIETViewModel
            //{
            //    KH_BANHANG_ID = x.KH_BANHANG_ID,
            //    KH_BANHANG_CTIET_ID = x.KH_BANHANG_CTIET_ID,
            //    Phone = x.Phone,
            //    KHACHHANG_ID = x.KHACHHANG_ID,
            //    CompanyName = x.CompanyName,
            //    Code = x.Code,
            //    TYLE_THANHCONG = x.TYLE_THANHCONG,
            //    TYLE_THANHCONG_REVIEW = x.TYLE_THANHCONG_REVIEW,
            //    GHI_CHU = x.GHI_CHU,
            //    IsDeleted = x.IsDeleted,
            //    THANG = x.THANG,
            //    NAM = x.NAM,
            //    CRM_Sale_ProductInvoiceCode = x.CRM_Sale_ProductInvoiceCode,
            //    CountForBrand = x.CountForBrand,
            //    TotalAmount = x.TotalAmount,
            //    ProductInvoiceId = x.ProductInvoiceId,
            //    NGUOILAP_ID = x.NGUOILAP_ID,
            //    NOIDUNG = x.NOIDUNG,
            //}).ToList();
            //ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            //ViewBag.DateSortParm = sortOrder == "History" ? "history_desc" : "History";
            //switch (sortOrder)
            //{
            //    case "name_desc":
            //        model.CRM_KH_BANHANGList = model.CRM_KH_BANHANGList.OrderByDescending(s => s.CompanyName).ToList();
            //        break;
            //    case "history_desc":
            //        model.CRM_KH_BANHANGList = model.CRM_KH_BANHANGList.OrderByDescending(s => s.KHACHHANG_ID).ToList();
            //        break;
            //    default:
            //        model.CRM_KH_BANHANGList = model.CRM_KH_BANHANGList.OrderBy(s => s.CompanyName).ToList();
            //        break;
            //}
            //foreach (var item in model.CRM_KH_BANHANGList)
            //{
            //    model.KHACHHANG_ID = item.KHACHHANG_ID.Value;
            //}
            //ViewBag.vwCRM_KH_BANHANG = model.CRM_KH_BANHANGList;
            //ViewBag.model = model;

            ////var BANHANG_CTIET_ProductInvoice = SqlHelper.QuerySP<CRM_KH_BANHANGViewModel>("spCrm_BANHANG_CTIET_ProductInvoice", new
            ////{
            ////    Month=month,
            ////    Year=year,
            ////    ManagerStaffId=id
            ////}).ToList();
            //var dataTongThucTe = SqlHelper.QuerySP<CRM_KH_BANHANGViewModel>("CrmListTongThucTeByNGUOILAP_ID", new
            //{
            //    THANG = month,
            //    NAM = year,
            //    NGUOILAP_ID = id
            //}).ToList();
            //ViewBag.BANHANG_CTIET_ProductInvoice = dataTongThucTe;
            //IEnumerable<KH_TUONGTACViewModel> KH_TUONGTAC = kH_TUONGTACRepository.GetAllvwKH_TUONGTAC()
            //      .Select(item => new KH_TUONGTACViewModel
            //      {
            //          KH_TUONGTAC_ID = item.KH_TUONGTAC_ID,
            //          NGAYLAP = item.NGAYLAP,
            //          HINHTHUC_TUONGTAC = item.HINHTHUC_TUONGTAC,
            //          LOAI_TUONGTAC = item.LOAI_TUONGTAC,
            //          PHANLOAI_TUONGTAC = item.PHANLOAI_TUONGTAC,
            //          TINHTRANG_TUONGTAC = item.TINHTRANG_TUONGTAC,
            //          MUCDO_TUONGTAC = item.MUCDO_TUONGTAC,
            //          GHI_CHU = item.GHI_CHU,
            //          GIAIPHAP_TUONGTAC = item.GIAIPHAP_TUONGTAC,
            //          NGAYTUONGTAC_TIEP = item.NGAYTUONGTAC_TIEP,
            //          GIOTUONGTAC_TIEP = item.GIOTUONGTAC_TIEP,
            //          MUCCANHBAO_TUONGTAC = item.MUCCANHBAO_TUONGTAC,
            //          HINH_ANH = item.HINH_ANH,
            //          BranchId = item.BranchId,
            //          FullName = item.FullName,
            //          ModifiedDate = item.ModifiedDate,
            //          CreatedDate = item.CreatedDate,
            //          GIO_TUONGTAC = item.GIO_TUONGTAC,
            //          NGUOILAP_ID = item.NGUOILAP_ID,
            //          KHACHHANG_ID = item.KHACHHANG_ID,
            //          KETQUA_SAUTUONGTAC = item.KETQUA_SAUTUONGTAC

            //      });


            //ViewBag.LICHSUTUONGTAC = KH_TUONGTAC;
            var origin = categoryRepository.GetAllCategories().Select(item => new CategoryViewModel
            {
                Name = item.Name,
                Value = item.Value,
                Code = item.Code
            }).Where(item => item.Code == "Origin").ToList();
            ViewBag.Origin = origin;

            ViewBag.NGUOILAP = WebSecurity.CurrentUserId;
            ViewBag.NGUOILAP_ID = model.NGUOILAP_ID;
            ViewBag.SuccessMessage = TempData["SuccessMessage"];
            ViewBag.FailedMessage = TempData["FailedMessage"];
            ViewBag.AlertMessage = TempData["AlertMessage"];
            ViewBag.AdviseTypeError = TempData["AdviseTypeError"];


            return View(model);
        }


        [HttpPost]
        public ActionResult Create(CRM_KH_BANHANGViewModel model)
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.Snapshot }))
            {
                try
                {
                    if (model.ID_ANNAYAKE.HasValue)
                    {
                        var CRM_KH_BANHANG_ANNAYAKE = CRM_KH_BANHANGRepository.GetCRM_KH_BANHANGById(model.ID_ANNAYAKE.Value);
                        CRM_KH_BANHANG_ANNAYAKE.TARGET_BRAND = model.TARGET_BRAND;
                        CRM_KH_BANHANGRepository.UpdateCRM_KH_BANHANG(CRM_KH_BANHANG_ANNAYAKE);
                        model.NAM = CRM_KH_BANHANG_ANNAYAKE.NAM.Value;
                        model.THANG = CRM_KH_BANHANG_ANNAYAKE.THANG.Value;
                    }

                    if (model.ID_ORLANEPARIS.HasValue)
                    {
                        var CRM_KH_BANHANG_ORLANEPARIS = CRM_KH_BANHANGRepository.GetCRM_KH_BANHANGById(model.ID_ORLANEPARIS.Value);
                        CRM_KH_BANHANG_ORLANEPARIS.TARGET_BRAND = model.ORLANEPARIS;
                        CRM_KH_BANHANGRepository.UpdateCRM_KH_BANHANG(CRM_KH_BANHANG_ORLANEPARIS);
                        model.NAM = CRM_KH_BANHANG_ORLANEPARIS.NAM.Value;
                        model.THANG = CRM_KH_BANHANG_ORLANEPARIS.THANG.Value;
                    }
                    if (model.ID_LEONORGREYL.HasValue)
                    {
                        var CRM_KH_BANHANG_LEONORGREYL = CRM_KH_BANHANGRepository.GetCRM_KH_BANHANGById(model.ID_LEONORGREYL.Value);
                        CRM_KH_BANHANG_LEONORGREYL.TARGET_BRAND = model.LEONORGREYL;
                        CRM_KH_BANHANGRepository.UpdateCRM_KH_BANHANG(CRM_KH_BANHANG_LEONORGREYL);
                        model.NAM = CRM_KH_BANHANG_LEONORGREYL.NAM.Value;
                        model.THANG = CRM_KH_BANHANG_LEONORGREYL.THANG.Value;
                    }
                    if (model.ID_CONGNGHECAO.HasValue)
                    {
                        var CRM_KH_BANHANG_CONGNGHECAO = CRM_KH_BANHANGRepository.GetCRM_KH_BANHANGById(model.ID_CONGNGHECAO.Value);
                        CRM_KH_BANHANG_CONGNGHECAO.TARGET_BRAND = model.CONGNGHECAO;
                        CRM_KH_BANHANGRepository.UpdateCRM_KH_BANHANG(CRM_KH_BANHANG_CONGNGHECAO);
                        model.NAM = CRM_KH_BANHANG_CONGNGHECAO.NAM.Value;
                        model.THANG = CRM_KH_BANHANG_CONGNGHECAO.THANG.Value;
                    }
                    if (model.ID_DICHVU.HasValue)
                    {
                        var CRM_KH_BANHANG_DICHVU = CRM_KH_BANHANGRepository.GetCRM_KH_BANHANGById(model.ID_DICHVU.Value);
                        CRM_KH_BANHANG_DICHVU.TARGET_BRAND = model.DICHVU;
                        CRM_KH_BANHANGRepository.UpdateCRM_KH_BANHANG(CRM_KH_BANHANG_DICHVU);
                        model.NAM = CRM_KH_BANHANG_DICHVU.NAM.Value;
                        model.THANG = CRM_KH_BANHANG_DICHVU.THANG.Value;
                    }

                    ///bỏ vì update bằng ajax
                    //if (model.CRM_KH_BANHANGList != null)
                    //{
                    //    foreach (var item in model.CRM_KH_BANHANGList)
                    //    {
                    //        var CRM_KH_BANHANG_CT = CRM_KH_BANHANGRepository.GetCRM_KH_BANHANG_CTById(item.KH_BANHANG_CTIET_ID);
                    //        CRM_KH_BANHANG_CT.TYLE_THANHCONG = item.TYLE_THANHCONG;
                    //        CRM_KH_BANHANG_CT.TYLE_THANHCONG_REVIEW = item.TYLE_THANHCONG_REVIEW;
                    //        CRM_KH_BANHANG_CT.ModifiedDate = DateTime.Now;
                    //        CRM_KH_BANHANG_CT.ModifiedUserId = WebSecurity.CurrentUserId;
                    //        CRM_KH_BANHANGRepository.UpdateCRM_KH_BANHANG_CT(CRM_KH_BANHANG_CT);
                    //    }
                    //}
                    scope.Complete();
                    TempData[Globals.SuccessMessageKey] = App_GlobalResources.Wording.UpdateSuccess;
                    return RedirectToAction("Create", new { Id = model.Id, month = model.THANG, year = model.NAM });
                }
                catch (DbUpdateException ex)
                {
                    TempData[Globals.FailedMessageKey] = App_GlobalResources.Wording.Error;
                    return View(model);
                }
            }

        }

        #endregion


        #region Edit
        public ActionResult Edit(int id)
        {
            var CRM_KH_BANHANG = CRM_KH_BANHANGRepository.GetCRM_KH_BANHANGByUserId(id);
            if (CRM_KH_BANHANG != null && CRM_KH_BANHANG.IsDeleted != true)
            {
                var model = new CRM_KH_BANHANGViewModel();
                AutoMapper.Mapper.Map(CRM_KH_BANHANG, model);
                model.CRM_KH_BANHANG_CTIET = new List<CRM_KH_BANHANG_CTIETViewModel>();
                model.CRM_KH_BANHANG_CTIET = CRM_KH_BANHANG_CTIETRepository.GetAllvwCRM_KH_BANHANG_CTIET().Where(x => x.KH_BANHANG_ID == model.KH_BANHANG_ID)
                    .Select(x => new CRM_KH_BANHANG_CTIETViewModel
                    {
                        KH_BANHANG_ID = x.KH_BANHANG_ID,
                        KHACHHANG_ID = x.KHACHHANG_ID,
                        TYLE_THANHCONG = x.TYLE_THANHCONG,
                        TYLE_THANHCONG_REVIEW = x.TYLE_THANHCONG_REVIEW,
                        CompanyName = x.CompanyName,
                        Phone = x.Phone,
                        Code = x.Code,
                    }).ToList();

                return View(model);
            }
            return View();
        }

        [HttpPost]
        public ActionResult Edit(CRM_KH_BANHANGViewModel model, bool? IsPopup, int? id)
        {
            if (ModelState.IsValid)
            {

                var CRM_KH_BANHANG = CRM_KH_BANHANGRepository.GetCRM_KH_BANHANGById(model.KH_BANHANG_ID);
                AutoMapper.Mapper.Map(model, CRM_KH_BANHANG);
                CRM_KH_BANHANG.ModifiedUserId = WebSecurity.CurrentUserId;
                CRM_KH_BANHANG.ModifiedDate = DateTime.Now;
                CRM_KH_BANHANGRepository.UpdateCRM_KH_BANHANG(CRM_KH_BANHANG);

                if (model.CRM_KH_BANHANG_CTIETList.Any(x => x.Id > 0))
                {
                    //lưu danh sách thao tác thực hiện dịch vụ
                    foreach (var item in model.CRM_KH_BANHANG_CTIETList.Where(x => x.Id > 0))
                    {
                        var ins = new CRM_KH_BANHANG_CTIET();
                        ins.ModifiedUserId = WebSecurity.CurrentUserId;
                        ins.ModifiedDate = DateTime.Now;
                        ins.KHACHHANG_ID = item.Id;
                        ins.BranchId = Erp.BackOffice.Helpers.Common.CurrentUser.BranchId;
                        ins.KH_BANHANG_ID = CRM_KH_BANHANG.KH_BANHANG_ID;
                        CRM_KH_BANHANG_CTIETRepository.UpdateCRM_KH_BANHANG_CTIET(ins);

                    }
                }
                return View(model);
            }
            return View(model);
        }

        #endregion
        #region CreateProductInvoice
        public ActionResult CreateProductInvoice(int? CustomerId, string CustomerName, int? KH_BANHANG_CTIET_ID, string CountForBrand, int? Id)
        {
            //var crminvoice = new CRM_Sale_ProductInvoice();

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

            ProductInvoiceViewModel model = new ProductInvoiceViewModel();
            model.InvoiceList = new List<ProductInvoiceDetailViewModel>();
            if (Id != null)
            {
                var productInvoice = CRM_KH_BANHANGRepository.GetvwCRM_Sale_ProductInvoiceById(Id.Value);
                AutoMapper.Mapper.Map(productInvoice, model);
                //crminvoice = CRM_KH_BANHANGRepository.GetCRM_Sale_ProductInvoiceById(Id.Value);
                //AutoMapper.Mapper.Map(crminvoice, model);
                CountForBrand = model.CountForBrand;
                model.InvoiceList = CRM_KH_BANHANGRepository.GetAllvwInvoiceDetailsByInvoiceId(Id.Value).Select(item => new ProductInvoiceDetailViewModel
                {
                    IsDeleted = item.IsDeleted,
                    CreatedUserId = item.CreatedUserId,
                    ModifiedUserId = item.ModifiedUserId,
                    CreatedDate = item.CreatedDate,
                    ModifiedDate = item.ModifiedDate,
                    ExpiryDate = item.ExpiryDate,
                    LoCode = item.LoCode,
                    ProductId = item.ProductId.Value,
                    ProductName = item.ProductName,
                    ProductInvoiceId = item.ProductInvoiceId,
                    Price = item.Price,
                    Quantity = item.Quantity,
                    Unit = item.Unit,
                    Discount = item.Discount,
                    DiscountAmount = item.DiscountAmount,
                    Note = item.Note,
                    is_TANG = item.is_TANG,
                    CountForBrand = item.CountForBrand

                }).ToList();
            }
            else
            {
                var user = userRepository.GetByvwUserName(Helpers.Common.CurrentUser.UserName);
                model.BranchId = (intBrandID == null ? Convert.ToInt32(Session["GlobalCurentBranchId"]) : intBrandID);
                model.ReceiptViewModel = new ReceiptViewModel();
                model.NextPaymentDate_Temp = DateTime.Now.AddDays(30);
                model.ReceiptViewModel.Name = "Bán hàng - Thu tiền mặt";
                model.ReceiptViewModel.Amount = 0;
                model.CountForBrand = CountForBrand;
            }
            var productList = Domain.Helper.SqlHelper.QuerySP<InventoryViewModel>("spSale_Get_Inventory", new { WarehouseId = "", HasQuantity = "1", ProductCode = "", ProductName = "", CategoryCode = "", ProductGroup = "", BranchId = model.BranchId, LoCode = "", ProductId = "", ExpiryDate = "", Origin = "" });
            productList = productList.Where(x => x.IsSale != null && x.IsSale == true);
            if (KH_BANHANG_CTIET_ID != null)
            {
                model.KH_BANHANG_CTIET_ID = KH_BANHANG_CTIET_ID.Value;
            }
            //Thêm số lượng tồn kho cho chi tiết đơn hàng đã được thêm
            if (model.InvoiceList != null && model.InvoiceList.Count > 0)
            {
                foreach (var item in model.InvoiceList)
                {
                    var product = productList.Where(i => i.ProductId == item.ProductId && i.LoCode == item.LoCode && i.ExpiryDate == item.ExpiryDate).FirstOrDefault();
                    if (product != null)
                    {
                        item.QuantityInInventory = product.Quantity;
                        item.PriceTest = product.ProductPriceOutbound;
                    }
                    else
                    {
                        item.Id = 0;
                    }
                }
            }
            int taxfee = 0;
            int.TryParse(Helpers.Common.GetSetting("vat"), out taxfee);
            model.TaxFee = taxfee;

            var category = categoryRepository.GetAllCategories()
            .Select(item => new CategoryViewModel
            {
                Id = item.Id,
                Code = item.Code,
                Value = item.Value,
                Name = item.Name
            }).Where(x => x.Code == "Origin").ToList();
            ViewBag.category = category;

            return View(model);
        }
        [HttpPost]
        public ActionResult CreateProductInvoice(ProductInvoiceViewModel model, int KH_BANHANG_CTIET_ID)
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
                CRM_Sale_ProductInvoice productInvoice = null;
                using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.Snapshot }))
                {
                    try
                    {
                        if (model.Id == 0)
                        {
                            productInvoice = new CRM_Sale_ProductInvoice();
                            AutoMapper.Mapper.Map(model, productInvoice);
                            productInvoice.IsDeleted = false;
                            productInvoice.CreatedUserId = WebSecurity.CurrentUserId;
                            productInvoice.CreatedDate = DateTime.Now;
                            productInvoice.Status = Wording.OrderStatus_pending;
                            productInvoice.BranchId = (intBrandID == null ? Convert.ToInt32(Session["GlobalCurentBranchId"]) : intBrandID.Value);
                            productInvoice.IsArchive = false;
                            productInvoice.IsReturn = false;
                            productInvoice.Type = "invoice";
                            productInvoice.RemainingAmount = 0;
                            productInvoice.PaidAmount = model.TongTienSauVAT;
                            productInvoice.TotalAmount = model.TongTienSauVAT;
                            productInvoice.KH_BANHANG_CTIET_ID = KH_BANHANG_CTIET_ID;
                            productInvoice.PaymentMethod = SelectListHelper.GetSelectList_Category("FormPayment", null, "Name", null).FirstOrDefault().Value;
                            productInvoice.Discount = (double)(Helpers.Common.NVL_NUM_DECIMAL_NEW(model.Discount_DB) + Helpers.Common.NVL_NUM_DECIMAL_NEW(model.Discount_KM) + Helpers.Common.NVL_NUM_DECIMAL_NEW(model.Discount_VIP));

                            //hàm thêm mới
                            CRM_KH_BANHANGRepository.InsertProductInvoice(productInvoice, null);



                            //cập nhật lại mã hóa đơn
                            productInvoice.Code = Erp.BackOffice.Helpers.Common.GetOrderNo("CRMProductInvoice", model.Code);

                            //begin kiểm tra trùng mã đơn hàng
                            var c = CRM_KH_BANHANGRepository.GetAllvwCRM_Sale_ProductInvoice().
                                Where(item => item.Code == productInvoice.Code).FirstOrDefault();

                            if (c != null)
                            {
                                TempData[Globals.FailedMessageKey] = "Mã đơn hàng đã bị trùng. Vui lòng kiểm tra lại!";

                                //return RedirectToAction("Create");
                                return RedirectToAction("_ClosePopup", "Home", new { area = "", FunctionCallback = "ClosePopupAndReloadPage" });
                            }
                            //end kiểm tra trùng mã đơn hàng

                            //ghi log 
                            Erp.BackOffice.Controllers.HomeController.WriteLog(productInvoice.Id, productInvoice.Code, "đã lập đơn hàng dự kiến", "", Helpers.Common.CurrentUser.BranchId.Value);

                            Erp.BackOffice.Helpers.Common.SetOrderNo("CRMProductInvoice");
                            if (model.InvoiceList.Any(x => x.Id == 0))
                            {
                                //lưu danh sách thao tác thực hiện dịch vụ
                                foreach (var item in model.InvoiceList.Where(x => x.Id == 0 && x.ProductId > 0))
                                {
                                    var ins = new CRM_Sale_ProductInvoiceDetail();
                                    ins.IsDeleted = false;
                                    ins.CreatedUserId = WebSecurity.CurrentUserId;
                                    ins.ModifiedUserId = WebSecurity.CurrentUserId;
                                    ins.CreatedDate = DateTime.Now;
                                    ins.ModifiedDate = DateTime.Now;
                                    ins.ExpiryDate = item.ExpiryDate;
                                    ins.LoCode = item.LoCode;
                                    ins.ProductId = item.ProductId.Value;
                                    ins.ProductInvoiceId = productInvoice.Id;
                                    ins.Price = item.Price;
                                    ins.Quantity = item.Quantity;
                                    ins.Unit = item.Unit;
                                    ins.Discount = 0;
                                    ins.DiscountAmount = 0;
                                    ins.Note = item.Note;
                                    ins.is_TANG = item.is_TANG;
                                    //  ins.ProductType = item.ProductType;
                                    CRM_KH_BANHANGRepository.InsertProductInvoiceDetail(ins);
                                }
                            }
                            //Thêm vào quản lý chứng từ
                            TransactionController.Create(new TransactionViewModel
                            {
                                TransactionModule = "ProductInvoice",
                                TransactionCode = productInvoice.Code,
                                TransactionName = "Bán hàng"
                            });
                        }
                        else
                        {


                            productInvoice = new CRM_Sale_ProductInvoice();
                            AutoMapper.Mapper.Map(model, productInvoice);
                            productInvoice.Status = Wording.OrderStatus_pending;
                            productInvoice.BranchId = (intBrandID == null ? Convert.ToInt32(Session["GlobalCurentBranchId"]) : intBrandID.Value);
                            productInvoice.IsArchive = false;
                            productInvoice.IsReturn = false;
                            productInvoice.Type = "invoice";
                            productInvoice.RemainingAmount = 0;
                            productInvoice.PaidAmount = model.TongTienSauVAT;
                            productInvoice.TotalAmount = model.TongTienSauVAT;
                            productInvoice.KH_BANHANG_CTIET_ID = KH_BANHANG_CTIET_ID;
                            productInvoice.PaymentMethod = SelectListHelper.GetSelectList_Category("FormPayment", null, "Name", null).FirstOrDefault().Value;
                            productInvoice.Discount = (double)(Helpers.Common.NVL_NUM_DECIMAL_NEW(model.Discount_DB) + Helpers.Common.NVL_NUM_DECIMAL_NEW(model.Discount_KM) + Helpers.Common.NVL_NUM_DECIMAL_NEW(model.Discount_VIP));

                            //hàm thêm mới
                            CRM_KH_BANHANGRepository.UpdateCRMProductInvoice(productInvoice);

                            //ghi log 
                            //Erp.BackOffice.Controllers.HomeController.WriteLog(productInvoice.Id, productInvoice.Code, "đã cập đơn hàng dự kiến", "", Helpers.Common.CurrentUser.BranchId.Value);

                            var details = CRM_KH_BANHANGRepository.GetAllvwInvoiceDetailsByInvoiceId(model.Id).ToList();

                            foreach (var item in details)
                            {
                                CRM_KH_BANHANGRepository.DeleteDetailsInvoice(item.Id);
                            }
                            if (model.InvoiceList.Any(x => x.Id == 0))
                            {
                                //lưu danh sách thao tác thực hiện dịch vụ
                                foreach (var item in model.InvoiceList.Where(x => x.Id == 0 && x.ProductId > 0))
                                {
                                    var ins = new CRM_Sale_ProductInvoiceDetail();
                                    ins.IsDeleted = false;
                                    ins.CreatedUserId = WebSecurity.CurrentUserId;
                                    ins.ModifiedUserId = WebSecurity.CurrentUserId;
                                    ins.CreatedDate = DateTime.Now;
                                    ins.ModifiedDate = DateTime.Now;
                                    ins.ExpiryDate = item.ExpiryDate;
                                    ins.LoCode = item.LoCode;
                                    ins.ProductId = item.ProductId.Value;
                                    ins.ProductInvoiceId = productInvoice.Id;
                                    ins.Price = item.Price;
                                    ins.Quantity = item.Quantity;
                                    ins.Unit = item.Unit;
                                    ins.Discount = 0;
                                    ins.DiscountAmount = 0;
                                    ins.Note = item.Note;
                                    ins.is_TANG = item.is_TANG;
                                    //  ins.ProductType = item.ProductType;
                                    CRM_KH_BANHANGRepository.InsertProductInvoiceDetail(ins);
                                }
                            }
                        }
                        scope.Complete();
                    }
                    catch (DbUpdateException ex)
                    {
                        TempData[Globals.FailedMessageKey] = App_GlobalResources.Wording.Error;
                        return View(model);
                    }
                }
                return RedirectToAction("_ClosePopup", "Home", new { area = "", FunctionCallback = "ClosePopupAndReloadPage" });
            }
            return View(model);
        }
        #endregion
        #region DetailProductInvoice
        public ActionResult DetailProductInvoice(int? Id, string TransactionCode, string code)
        {
            var productInvoice = new vwCRM_Sale_ProductInvoice();

            if (Id != null && Id.Value > 0)
            {
                productInvoice = CRM_KH_BANHANGRepository.GetvwCRM_Sale_ProductInvoiceFullById(Id.Value);
            }

            if (!string.IsNullOrEmpty(TransactionCode))
            {
                productInvoice = CRM_KH_BANHANGRepository.GetvwCRM_Sale_ProductInvoiceByCode(TransactionCode);
            }
            if (!string.IsNullOrEmpty(code))
            {
                productInvoice = CRM_KH_BANHANGRepository.GetvwCRM_Sale_ProductInvoiceByCode(code);
            }
            if (productInvoice == null)
            {
                return RedirectToAction("Index");
            }

            var model = new ProductInvoiceViewModel();
            AutoMapper.Mapper.Map(productInvoice, model);

            model.AllowEdit = Helpers.Common.KiemTraNgaySuaChungTu
            (model.CreatedDate.Value)
            && (Filters.SecurityFilter.IsAdmin()
            || productInvoice.BranchId == Helpers.Common.CurrentUser.BranchId
            );
            //Lấy lịch sử giao dịch thanh toán
            var listTransaction = transactionLiabilitiesRepository.GetAllvwTransaction()
                        .Where(item => item.MaChungTuGoc == productInvoice.Code)
                        .OrderByDescending(item => item.CreatedDate)
                        .ToList();

            model.ListTransactionLiabilities = new List<TransactionLiabilitiesViewModel>();
            AutoMapper.Mapper.Map(listTransaction, model.ListTransactionLiabilities);
            //Lấy danh sách chi tiết đơn hàng
            model.InvoiceList = CRM_KH_BANHANGRepository.GetAllvwInvoiceDetailsByInvoiceId(productInvoice.Id).Select(x =>
                new ProductInvoiceDetailViewModel
                {
                    Id = x.Id,
                    Price = x.Price,
                    ProductId = x.ProductId,
                    Quantity = x.Quantity,
                    Unit = x.Unit,
                    ProductType = x.ProductType,

                    CategoryCode = x.CategoryCode,
                    ProductInvoiceCode = x.ProductInvoiceCode,
                    ProductName = x.ProductName,
                    ProductCode = x.ProductCode,
                    ProductInvoiceId = x.ProductInvoiceId,
                    ProductGroup = x.ProductGroup,
                    Amount = x.is_TANG == 1 ? 0 : x.Amount,
                    LoCode = x.LoCode,
                    ExpiryDate = x.ExpiryDate,
                    Discount = x.Discount,
                    DiscountAmount = x.DiscountAmount,
                    is_TANG = x.is_TANG
                }).OrderBy(x => x.Id).ToList();

            ViewBag.isAdmin = Erp.BackOffice.Helpers.Common.CurrentUser.UserTypeId == 1 ? true : false;

            ViewBag.SuccessMessage = TempData["SuccessMessage"];
            ViewBag.FailedMessage = TempData["FailedMessage"];
            ViewBag.AlertMessage = TempData["AlertMessage"];

            return View(model);
        }
        #endregion

        #region CreateKH_BANHANG
        public ActionResult CreateKH_BANHANG(CRM_KH_BANHANGViewModel model, int? id, string FullName, string CountForBrand, int? month, int? year)
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

            //model.CustomerList = CustomerRepository.GetAllCustomer().Select(x => new CustomerViewModel
            //{
            //    Id = x.Id,
            //    CompanyName = x.CompanyName,
            //    Code = x.Code,
            //    Phone = x.Phone,
            //    ManagerStaffId = x.ManagerStaffId,
            //    CreatedDate = x.CreatedDate,
            //    isLock = x.isLock,
            //}).Where(x => x.isLock != true && listNhanvien.Contains(x.ManagerStaffId.Value)).ToList();
            //model.CustomerList = model.CustomerList.Where(x => x.ManagerStaffId == id).ToList();

            model.CRM_KH_BANHANG_CTIET = CRM_KH_BANHANGRepository.GetAllvwCRM_KH_BANHANG_CTIET().Select(x => new CRM_KH_BANHANG_CTIETViewModel
            {
                KH_BANHANG_CTIET_ID = x.KH_BANHANG_CTIET_ID,
                BranchId = x.BranchId,
                KH_BANHANG_ID = x.KH_BANHANG_ID,
                THANG = x.THANG,
                NAM = x.NAM,
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
                Phone = x.Phone,
                CompanyName = x.CompanyName,
                Code = x.Code,
                CRM_Sale_ProductInvoiceCode = x.CRM_Sale_ProductInvoiceCode,
                CountForBrand = x.CountForBrand,
                TotalAmount = x.TotalAmount,
                ProductInvoiceId = x.ProductInvoiceId,
                NGUOILAP_ID = x.NGUOILAP_ID,
            }).Where(x => x.THANG == month && x.NAM == year && x.NGUOILAP_ID == id).ToList();
            if (id != null)
            {
                var user = userRepository.GetUserById(id.Value);
                model.NGUOILAP_ID = user.Id;
                model.FullName = user.FullName;
                ViewBag.user = user;
            }
            else
            {
                var user = userRepository.GetUserById(WebSecurity.CurrentUserId);
                model.NGUOILAP_ID = user.Id;
                model.FullName = user.FullName;
                ViewBag.user = user;
            }
            //var ProductInvoice = ProductInvoiceRepository.GetAllvwProductInvoice().Select(item => new ProductInvoiceViewModel
            //{
            //    Id = item.Id,
            //    ManagerStaffId = item.ManagerStaffId,
            //    CountForBrand = item.CountForBrand,
            //    TotalAmount = item.TotalAmount,
            //    Code = item.Code,
            //    CustomerId = item.CustomerId,
            //}).ToList();
            //ViewBag.ProductInvoice = ProductInvoice;
            return View(model);
        }

        [HttpPost]
        public ActionResult CreateKH_BANHANG(CRM_KH_BANHANGViewModel model)
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
            model.BranchId = intBrandID;

            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.Snapshot }))
            {
                try
                {
                    var kh_banhang = CRM_KH_BANHANGRepository.GetAllCRM_KH_BANHANG().Where(x => x.NAM == model.NAM &&
                    x.THANG == model.THANG && x.NGUOILAP_ID == model.NGUOILAP_ID && x.CountForBrand == model.CountForBrand && x.BranchId == model.BranchId).ToList();
                    if (kh_banhang.Count() > 0)
                    {
                        TempData[Globals.FailedMessageKey] = "Tháng " + model.THANG + " năm " + model.NAM + " đã có kế hoạch cho nhãn hàng " + model.CountForBrand;
                        return RedirectToAction("_ClosePopup", "Home", new { area = "", FunctionCallback = "ClosePopupAndReloadPage" });

                    }
                    else
                    {
                        var CRM_KH_BANHANG = new CRM_KH_BANHANG();
                        AutoMapper.Mapper.Map(model, CRM_KH_BANHANG);
                        CRM_KH_BANHANG.IsDeleted = false;
                        CRM_KH_BANHANG.CreatedUserId = WebSecurity.CurrentUserId;
                        CRM_KH_BANHANG.ModifiedUserId = WebSecurity.CurrentUserId;

                        CRM_KH_BANHANG.CreatedDate = DateTime.Now;
                        CRM_KH_BANHANG.ModifiedDate = DateTime.Now;
                        CRM_KH_BANHANG.NAM = model.NAM;
                        CRM_KH_BANHANG.THANG = model.THANG;
                        CRM_KH_BANHANG.BranchId = (intBrandID == null ? Convert.ToInt32(Session["GlobalCurentBranchId"]) : intBrandID);
                        CRM_KH_BANHANG.CountForBrand = model.CountForBrand;
                        CRM_KH_BANHANG.TARGET_BRAND = model.TARGET_BRAND;
                        CRM_KH_BANHANG.NGUOILAP_ID = model.NGUOILAP_ID;
                        CRM_KH_BANHANGRepository.InsertCRM_KH_BANHANG(CRM_KH_BANHANG);
                        //ghi log 
                        Erp.BackOffice.Controllers.HomeController.WriteLog(CRM_KH_BANHANG.KH_BANHANG_ID, "", "đã lập kế hoạch bán hàng (" + model.THANG + "/" + model.NAM + ")" + " cho nhãn hàng " + model.CountForBrand, "", Helpers.Common.CurrentUser.BranchId.Value);

                    }
                    scope.Complete();
                    TempData[Globals.SuccessMessageKey] = App_GlobalResources.Wording.UpdateSuccess;
                    return RedirectToAction("_ClosePopup", "Home", new { area = "", FunctionCallback = "ClosePopupAndReloadPage" });
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
        public ActionResult CreateKH(int? SalerId, int? Year, int? Month, int? KH_BANHANG_ID, string Phone, string CustomerName, string CustomerCode, string checkTK)
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

            Year = Year == null ? DateTime.Now.Year : Year;
            Month = Month == null ? DateTime.Now.Month : Month;
            Phone = Phone == null ? "" : Phone;
            CustomerCode = CustomerCode == null ? "" : CustomerCode;
            CustomerName = CustomerName == null ? "" : CustomerName;
            //var CRM_KH_BANHANGCT = CRM_KH_BANHANGRepository.GetAllvwCRM_KH_BANHANG_CTIET().Where(x => x.KH_BANHANG_ID == KH_BANHANG_ID && x.IsDeleted == false).ToList();
            var dataNhanvien = SqlHelper.QuerySP<ManagerStaff>("spgetNhanvienbacap", new
            {
                BranchId = (intBrandID == null ? 0 : intBrandID),
                UserId = WebSecurity.CurrentUserId,
            }).ToList();

            List<int> listNhanvien = new List<int>();

            bool hasSearch = false;
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

            var model = new CRM_KH_BANHANGViewModel();
            //foreach (var item1 in CRM_KH_BANHANGCT)
            //{
            if ((SalerId == null) || (SalerId == 0))
            {
                return View(model);
            }
            model.CustomerList = CustomerRepository.GetAllvwCustomer().Select(x => new CustomerViewModel
            {
                Id = x.Id,
                CompanyName = x.CompanyName,
                Code = x.Code,
                Phone = x.Phone,
                ManagerStaffId = x.ManagerStaffId,
                CreatedDate = x.CreatedDate,
                isLock = x.isLock,
                ManagerStaffName = x.ManagerStaffName,
                ManagerUserName = x.ManagerUserName,
                BranchId = x.BranchId
            }).Where(x => (x.BranchId == intBrandID) && (x.isLock == false || x.isLock == null) /*&& x.ManagerStaffId == SalerId*/).ToList();

            if (SalerId != null)
            {
                model.CustomerList = model.CustomerList.Where(x => x.ManagerStaffId == SalerId).ToList();
            }



            //}
            //if(listNhanvien.Count() >0)
            //{
            //    model.CustomerList.Where(x =>listNhanvien.Contains(x.ManagerStaffId.Value)).ToList();
            //}
            //model.CustomerList.RemoveAll(
            //                x =>
            //                CRM_KH_BANHANGCT.Any(
            //                k => k.KHACHHANG_ID == x.Id));

            model.vwKH_SAPHETSP = SqlHelper.QuerySP<vwKH_SAPHETSPViewModel>("sp_CRMKH_SAPHETSP", new
            {
                Phone = Phone,
                //ManagerStaffId = WebSecurity.CurrentUserId,
                CustomerCode = CustomerCode,
                CustomerName = CustomerName,
                Year = Year,
                Month = Month
            }).Where(x => x.BranchId == intBrandID).ToList();






            model.MBSAPHETHANList = SqlHelper.QuerySP<MBSAPHETHANViewModel>("spMBSAPHETHAN", new
            {
                Phone = Phone,
                ManagerStaffId = SalerId,
                CustomerCode = CustomerCode,
                CustomerName = CustomerName,
                Year = Year,
                Month = Month
            }).Where(x => x.BranchId == intBrandID).ToList();




            model.KH_TUONGTACList = SqlHelper.QuerySP<KH_TUONGTACViewModel>("spGET_KH_TUONGTAC", new
            {
                Phone = Phone,
                ManagerStaffId = SalerId,
                CustomerCode = CustomerCode,
                CustomerName = CustomerName,
                Year = Year,
                Month = Month
            }).Where(x => x.BranchId == intBrandID).ToList();






            model.vwKH_SAPHETDV = SqlHelper.QuerySP<KH_SapHetDVViewModel>("sp_CRMKH_SAPHETDV", new
            {
                Phone = Phone,
                ManagerStaffId = SalerId,
                CustomerCode = CustomerCode,
                CustomerName = CustomerName,
                Year = Year,
                Month = Month
            }).Where(x => x.BranchId == intBrandID).ToList();



            if (!Erp.BackOffice.Filters.SecurityFilter.IsAdmin())
            {
                model.vwKH_SAPHETDV = model.vwKH_SAPHETDV.Where(x => listNhanvien.Contains(x.ManagerStaffId.Value)).ToList();
                model.CustomerList = model.CustomerList.Where(x => listNhanvien.Contains(x.ManagerStaffId.Value)).ToList();
                model.vwKH_SAPHETSP = model.vwKH_SAPHETSP.Where(x => listNhanvien.Contains(x.ManagerStaffId.Value)).ToList();
                model.KH_TUONGTACList = model.KH_TUONGTACList.Where(x => listNhanvien.Contains(x.ManagerStaffId.Value)).ToList();
                model.MBSAPHETHANList = model.MBSAPHETHANList.Where(x => listNhanvien.Contains(x.ManagerStaffId.Value)).ToList();

            }




            //model.vwKH_SAPHETDV.RemoveAll(
            //       x =>
            //       CRM_KH_BANHANGCT.Any(
            //       k => k.KHACHHANG_ID == x.CustomerId));
            //model.KH_TUONGTACList.RemoveAll(
            //       x =>
            //       CRM_KH_BANHANGCT.Any(
            //       k => k.KHACHHANG_ID == x.KHACHHANG_ID));
            //model.MBSAPHETHANList.RemoveAll(
            //       x =>
            //       CRM_KH_BANHANGCT.Any(
            //       k => k.KHACHHANG_ID == x.CustomerId));
            //model.vwKH_SAPHETSP.RemoveAll(
            //       x =>
            //       CRM_KH_BANHANGCT.Any(
            //       k => k.KHACHHANG_ID == x.CustomerId));
            model.KH_TUONGTACList = model.KH_TUONGTACList.GroupBy(x => x.KHACHHANG_ID).Select(x => x.First()).OrderByDescending(x => x.CreatedDate).ToList();
            if (checkTK == "checkSPSHH")
            {
                model.checkTK = checkTK;
            }
            if (checkTK == "checkMB")
            {
                model.checkTK = checkTK;
            }
            if (checkTK == "checkDVSHH")
            {
                model.checkTK = checkTK;
            }
            //model.KH_TUONGTACList = kH_TUONGTACRepository.GetAllvwKH_TUONGTAC().Select(x => new KH_TUONGTACViewModel
            //    {
            //        KH_TUONGTAC_ID = x.KH_TUONGTAC_ID,
            //        CustomerCode = x.CustomerCode,
            //        KHACHHANG_ID = x.KHACHHANG_ID,
            //        Phone = x.Phone,
            //        NGUOILAP_ID = x.NGUOILAP_ID,
            //        CreatedDate = x.CreatedDate,
            //        NGAYTUONGTAC_TIEP = x.NGAYTUONGTAC_TIEP,
            //        GIOTUONGTAC_TIEP = x.GIOTUONGTAC_TIEP,
            //        LICHTUONGTATIEP = x.NGAYTUONGTAC_TIEP,
            //        //THANG= int.Parse(x.NGAYTUONGTAC_TIEP.Substring(0,2)),
            //        //NAM= int.Parse(x.NGAYTUONGTAC_TIEP.Substring(3,2)),
            //        CustomerName=x.CustomerName
            //    }).ToList();
            //model.KH_TUONGTACList.Where(x=>x.LICHTUONGTATIEP!=null && int.Parse(x.LICHTUONGTATIEP.Substring(3,2))==Month && int.Parse(x.LICHTUONGTATIEP.Substring(6,4))==Year);
            if (checkTK == "checkLTT")
            {
                model.checkTK = checkTK;
            }
            if (CustomerCode != null)
            {
                CustomerCode = Helpers.Common.ChuyenThanhKhongDau(CustomerCode);
                model.vwKH_SAPHETDV = model.vwKH_SAPHETDV.Where(x => Helpers.Common.ChuyenThanhKhongDau(x.CustomerCode).Contains(CustomerCode)).ToList();
                model.MBSAPHETHANList = model.MBSAPHETHANList.Where(x => Helpers.Common.ChuyenThanhKhongDau(x.CustomerCode).Contains(CustomerCode)).ToList();
                model.vwKH_SAPHETSP = model.vwKH_SAPHETSP.Where(x => Helpers.Common.ChuyenThanhKhongDau(x.CustomerCode).Contains(CustomerCode)).ToList();
                model.KH_TUONGTACList = model.KH_TUONGTACList.Where(x => Helpers.Common.ChuyenThanhKhongDau(x.CustomerCode).Contains(CustomerCode)).ToList();
                model.CustomerList = model.CustomerList.Where(x => Helpers.Common.ChuyenThanhKhongDau(x.Code).Contains(CustomerCode)).ToList();
                hasSearch = true;
            }
            if (CustomerName != null)
            {
                CustomerName = Helpers.Common.ChuyenThanhKhongDau(CustomerName);
                model.vwKH_SAPHETDV = model.vwKH_SAPHETDV.Where(x => Helpers.Common.ChuyenThanhKhongDau(x.CompanyName).Contains(CustomerName)).ToList();
                model.MBSAPHETHANList = model.MBSAPHETHANList.Where(x => Helpers.Common.ChuyenThanhKhongDau(x.CustomerName).Contains(CustomerName)).ToList();
                model.vwKH_SAPHETSP = model.vwKH_SAPHETSP.Where(x => Helpers.Common.ChuyenThanhKhongDau(x.CompanyName).Contains(CustomerName)).ToList();
                model.KH_TUONGTACList = model.KH_TUONGTACList.Where(x => Helpers.Common.ChuyenThanhKhongDau(x.CustomerName).Contains(CustomerName)).ToList();
                model.CustomerList = model.CustomerList.Where(x => Helpers.Common.ChuyenThanhKhongDau(x.CompanyName).Contains(CustomerName)).ToList();
                hasSearch = true;
            }
            if (Phone != null)
            {
                model.vwKH_SAPHETDV = model.vwKH_SAPHETDV.Where(x => Helpers.Common.ChuyenThanhKhongDau(x.Phone).Contains(Phone)).ToList();
                model.MBSAPHETHANList = model.MBSAPHETHANList.Where(x => Helpers.Common.ChuyenThanhKhongDau(x.Phone).Contains(Phone)).ToList();
                model.CustomerList = model.CustomerList.Where(x => Helpers.Common.ChuyenThanhKhongDau(x.Phone).Contains(Phone)).ToList();
                model.KH_TUONGTACList = model.KH_TUONGTACList.Where(x => Helpers.Common.ChuyenThanhKhongDau(x.Phone).Contains(Phone)).ToList();
                hasSearch = true;
            }
            //var ProductInvoice = ProductInvoiceRepository.GetAllvwProductInvoice().Select(item => new ProductInvoiceViewModel
            //{
            //    Id = item.Id,
            //    ManagerStaffId = item.ManagerStaffId,
            //    CountForBrand = item.CountForBrand,
            //    TotalAmount = item.TotalAmount,
            //    Code = item.Code,
            //    CustomerId = item.CustomerId,
            //}).ToList();

            //IEnumerable<CustomerViewModel> q = SqlHelper.QuerySP<CustomerViewModel>("spGetAllCustomerByType", new
            //{
            //    islock = 0,
            //    BranchId = intBrandID,
            //    loaiKH = 2
            //});


            //List<int?> KHID = new List<int?>();
            //foreach(var item in q)
            //{
            //    KHID.Add(item.Id);
            //}
            model.CustomerList = model.CustomerList.ToList();


            return View(model);
        }
        [HttpPost]
        public ActionResult CreateKH(CRM_KH_BANHANGViewModel model)
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


            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.Snapshot }))
            {
                try
                {
                    var q = CRM_KH_BANHANGRepository.GetCRM_KH_BANHANGById(model.KH_BANHANG_ID);
                    if (model.CustomerList != null)
                    {
                        foreach (var item in model.CustomerList)
                        {
                            // var CRM_KH_BANHANGCT = CRM_KH_BANHANGRepository.GetAllvwCRM_KH_BANHANG_CTIET().Where(x => x.KH_BANHANG_ID == model.KH_BANHANG_ID && x.KHACHHANG_ID == item.Id && x.IsDeleted == false);
                            if (item.is_checked != null)
                            {
                                //if (CRM_KH_BANHANGCT.Count() == 0)
                                //{
                                var ins = new CRM_KH_BANHANG_CTIET();
                                ins.IsDeleted = false;
                                ins.CreatedUserId = WebSecurity.CurrentUserId;
                                ins.ModifiedUserId = WebSecurity.CurrentUserId;
                                ins.CreatedDate = DateTime.Now;
                                ins.ModifiedDate = DateTime.Now;
                                ins.THANG = q.THANG;
                                ins.NAM = q.NAM;
                                ins.KHACHHANG_ID = item.Id;
                                ins.BranchId = (intBrandID == null ? Convert.ToInt32(Session["GlobalCurentBranchId"]) : intBrandID);
                                ins.KH_BANHANG_ID = model.KH_BANHANG_ID;
                                ins.GHI_CHU = item.GHI_CHU;
                                ins.TYLE_THANHCONG = 0;
                                ins.TYLE_THANHCONG_REVIEW = 0;
                                ins.IsApprove = 0;
                                CRM_KH_BANHANG_CTIETRepository.InsertCRM_KH_BANHANG_CTIET(ins);
                                // }
                            }
                        }
                    }
                    if (model.KH_TUONGTACList != null)
                    {
                        foreach (var item in model.KH_TUONGTACList)
                        {
                            // var CRM_KH_BANHANGCT = CRM_KH_BANHANGRepository.GetAllvwCRM_KH_BANHANG_CTIET().Where(x => x.KH_BANHANG_ID == model.KH_BANHANG_ID && x.KHACHHANG_ID == item.CustomerId && x.IsDeleted == false);
                            if (item.is_checkedKH_TUONGTAC != null)
                            {
                                //if (CRM_KH_BANHANGCT.Count() == 0)
                                //{
                                var ins = new CRM_KH_BANHANG_CTIET();
                                ins.IsDeleted = false;
                                ins.CreatedUserId = WebSecurity.CurrentUserId;
                                ins.ModifiedUserId = WebSecurity.CurrentUserId;
                                ins.CreatedDate = DateTime.Now;
                                ins.ModifiedDate = DateTime.Now;
                                ins.THANG = q.THANG;
                                ins.NAM = q.NAM;
                                ins.KHACHHANG_ID = item.KHACHHANG_ID;
                                ins.BranchId = (intBrandID == null ? Convert.ToInt32(Session["GlobalCurentBranchId"]) : intBrandID);
                                ins.KH_BANHANG_ID = model.KH_BANHANG_ID;
                                ins.GHI_CHU = item.GHI_CHU;
                                ins.TYLE_THANHCONG = 0;
                                ins.TYLE_THANHCONG_REVIEW = 0;
                                ins.NOIDUNG = item.NGAYTUONGTAC_TIEP;
                                ins.IsApprove = 0;
                                CRM_KH_BANHANG_CTIETRepository.InsertCRM_KH_BANHANG_CTIET(ins);
                                //}
                            }
                        }
                    }
                    if (model.vwKH_SAPHETSP != null)
                    {
                        foreach (var item in model.vwKH_SAPHETSP)
                        {
                            //var CRM_KH_BANHANGCT = CRM_KH_BANHANGRepository.GetAllvwCRM_KH_BANHANG_CTIET().Where(x => x.KH_BANHANG_ID == model.KH_BANHANG_ID && x.KHACHHANG_ID == item.CustomerId && x.IsDeleted == false);
                            if (item.is_checkedSAPHETSP != null)
                            {
                                //if (CRM_KH_BANHANGCT.Count() == 0)
                                //{
                                var ins = new CRM_KH_BANHANG_CTIET();
                                ins.IsDeleted = false;
                                ins.CreatedUserId = WebSecurity.CurrentUserId;
                                ins.ModifiedUserId = WebSecurity.CurrentUserId;
                                ins.CreatedDate = DateTime.Now;
                                ins.ModifiedDate = DateTime.Now;
                                ins.THANG = q.THANG;
                                ins.NAM = q.NAM;
                                ins.KHACHHANG_ID = item.CustomerId;
                                ins.BranchId = (intBrandID == null ? Convert.ToInt32(Session["GlobalCurentBranchId"]) : intBrandID);
                                ins.KH_BANHANG_ID = model.KH_BANHANG_ID;
                                ins.GHI_CHU = item.GHI_CHU;
                                ins.TYLE_THANHCONG = 0;
                                ins.TYLE_THANHCONG_REVIEW = 0;
                                ins.NOIDUNG = item.SPSAPHETHAN;
                                ins.IsApprove = 0;
                                CRM_KH_BANHANG_CTIETRepository.InsertCRM_KH_BANHANG_CTIET(ins);
                                // }
                            }
                        }
                    }
                    if (model.MBSAPHETHANList != null)
                    {
                        foreach (var item in model.MBSAPHETHANList)
                        {
                            //var CRM_KH_BANHANGCT = CRM_KH_BANHANGRepository.GetAllvwCRM_KH_BANHANG_CTIET().Where(x => x.KH_BANHANG_ID == model.KH_BANHANG_ID && x.KHACHHANG_ID == item.CustomerId && x.IsDeleted == false);
                            if (item.is_checkedMBSAPHETHAN != null)
                            {
                                //if (CRM_KH_BANHANGCT.Count() == 0)
                                //{
                                var ins = new CRM_KH_BANHANG_CTIET();
                                ins.IsDeleted = false;
                                ins.CreatedUserId = WebSecurity.CurrentUserId;
                                ins.ModifiedUserId = WebSecurity.CurrentUserId;
                                ins.CreatedDate = DateTime.Now;
                                ins.ModifiedDate = DateTime.Now;
                                ins.THANG = q.THANG;
                                ins.NAM = q.NAM;
                                ins.KHACHHANG_ID = item.CustomerId;
                                ins.BranchId = (intBrandID == null ? Convert.ToInt32(Session["GlobalCurentBranchId"]) : intBrandID);
                                ins.KH_BANHANG_ID = model.KH_BANHANG_ID;
                                ins.GHI_CHU = item.GHI_CHU;
                                ins.TYLE_THANHCONG = 0;
                                ins.TYLE_THANHCONG_REVIEW = 0;
                                ins.NOIDUNG = item.ExpiryDate;
                                ins.IsApprove = 0;
                                CRM_KH_BANHANG_CTIETRepository.InsertCRM_KH_BANHANG_CTIET(ins);
                                // }
                            }
                        }
                    }
                    if (model.vwKH_SAPHETDV != null)
                    {
                        foreach (var item in model.vwKH_SAPHETDV)
                        {
                            //var CRM_KH_BANHANGCT = CRM_KH_BANHANGRepository.GetAllvwCRM_KH_BANHANG_CTIET().Where(x => x.KH_BANHANG_ID == model.KH_BANHANG_ID && x.KHACHHANG_ID == item.CustomerId && x.IsDeleted == false);
                            if (item.is_checkedKH_SAPHETDV != null)
                            {
                                //if (CRM_KH_BANHANGCT.Count() == 0)
                                //{
                                var ins = new CRM_KH_BANHANG_CTIET();
                                ins.IsDeleted = false;
                                ins.CreatedUserId = WebSecurity.CurrentUserId;
                                ins.ModifiedUserId = WebSecurity.CurrentUserId;
                                ins.CreatedDate = DateTime.Now;
                                ins.ModifiedDate = DateTime.Now;
                                ins.THANG = q.THANG;
                                ins.NAM = q.NAM;
                                ins.KHACHHANG_ID = item.CustomerId;
                                ins.BranchId = (intBrandID == null ? Convert.ToInt32(Session["GlobalCurentBranchId"]) : intBrandID);
                                ins.KH_BANHANG_ID = model.KH_BANHANG_ID;
                                ins.GHI_CHU = item.GHI_CHU;
                                ins.TYLE_THANHCONG = 0;
                                ins.TYLE_THANHCONG_REVIEW = 0;
                                ins.NOIDUNG = item.SPSAPHETHAN;
                                ins.IsApprove = 0;
                                CRM_KH_BANHANG_CTIETRepository.InsertCRM_KH_BANHANG_CTIET(ins);
                                //}
                            }
                        }
                    }
                    scope.Complete();
                    TempData[Globals.SuccessMessageKey] = App_GlobalResources.Wording.UpdateSuccess;
                    return RedirectToAction("_ClosePopup", "Home", new { area = "", FunctionCallback = "ClosePopupAndReloadPage" });
                }
                catch (Exception ex)
                {
                    return View(model);
                }
            }

        }
        #endregion
        #region KETHUA
        [AllowAnonymous]
        [HttpPost]
        public ActionResult KETHUA(CRM_KH_BANHANGViewModel model, int? KH_BANHANG_ID, string CountForBrand, int? NGUOILAP_ID)
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

            DateTime date = DateTime.Parse(model.Month);
            int monthnow = date.Month;
            int yearnow = date.Year;
            date = date.AddMonths(-1);
            //string THANGNAM = date.ToString("M/yyyy");
            //int THANG = int.Parse(THANGNAM.Substring(0, 1));
            //int NAM = int.Parse(THANGNAM.Substring(2, 4));
            //var model = new CRM_KH_BANHANGViewModel();
            int THANG = date.Month;
            int NAM = date.Year;
            IEnumerable<CRM_KH_BANHANGViewModel> q = CRM_KH_BANHANGRepository.GetAllCRM_KH_BANHANG().Select(x => new CRM_KH_BANHANGViewModel
            {
                KH_BANHANG_ID = x.KH_BANHANG_ID,
                BranchId = x.BranchId,
                THANG = x.THANG.Value,
                NAM = x.NAM.Value,
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
            //var CRM_KH_BANHANGS =new CRM_KH_BANHANG_CTIETViewModel();
            IEnumerable<CRM_KH_BANHANG_CTIETViewModel> CRM_KH_BANHANGS = CRM_KH_BANHANGRepository.GetAllvwCRM_KH_BANHANG_CTIET().Select(x => new CRM_KH_BANHANG_CTIETViewModel
            {
                //KH_BANHANG_ID = x.KH_BANHANG_ID,
                KH_BANHANG_CTIET_ID = x.KH_BANHANG_CTIET_ID,
                Phone = x.Phone,
                KHACHHANG_ID = x.KHACHHANG_ID,
                CompanyName = x.CompanyName,
                Code = x.Code,
                TYLE_THANHCONG = x.TYLE_THANHCONG,
                TYLE_THANHCONG_REVIEW = x.TYLE_THANHCONG_REVIEW,
                GHI_CHU = x.GHI_CHU,
                IsDeleted = x.IsDeleted,
                THANG = x.THANG,
                NAM = x.NAM,
                CRM_Sale_ProductInvoiceCode = x.CRM_Sale_ProductInvoiceCode,
                CountForBrand = x.CountForBrand,
                TotalAmount = x.TotalAmount,
                ProductInvoiceId = x.ProductInvoiceId,
                NGUOILAP_ID = x.NGUOILAP_ID
            }).Where(x => x.THANG == THANG && x.NAM == NAM && x.CountForBrand == model.CountForBrand && (x.TYLE_THANHCONG < 100 || x.TYLE_THANHCONG == null) && x.NGUOILAP_ID == model.NGUOILAP_ID).ToList();
            if (model.KH_BANHANG_ID > 0)
            {
                var CRM_KH_BANHANG = CRM_KH_BANHANGRepository.GetCRM_KH_BANHANGById(model.KH_BANHANG_ID);
                //var CRM_KH_BANHANGCT = CRM_KH_BANHANGRepository.GetCRM_KH_BANHANGByKHBANHANG_ID(KH_BANHANG_ID.Value);
                foreach (var item in CRM_KH_BANHANGS)
                {
                    IEnumerable<CRM_KH_BANHANG_CTIETViewModel> CRM_KH_BANHANGCT = CRM_KH_BANHANGRepository.GetAllvwCRM_KH_BANHANG_CTIET().Select(x => new CRM_KH_BANHANG_CTIETViewModel
                    {
                        KH_BANHANG_CTIET_ID = x.KH_BANHANG_CTIET_ID,
                        Phone = x.Phone,
                        KHACHHANG_ID = x.KHACHHANG_ID,
                        CompanyName = x.CompanyName,
                        Code = x.Code,
                        TYLE_THANHCONG = x.TYLE_THANHCONG,
                        TYLE_THANHCONG_REVIEW = x.TYLE_THANHCONG_REVIEW,
                        GHI_CHU = x.GHI_CHU,
                        IsDeleted = x.IsDeleted,
                        THANG = x.THANG,
                        NAM = x.NAM,
                        CRM_Sale_ProductInvoiceCode = x.CRM_Sale_ProductInvoiceCode,
                        CountForBrand = x.CountForBrand,
                        TotalAmount = x.TotalAmount,
                        ProductInvoiceId = x.ProductInvoiceId,
                        KH_BANHANG_ID = x.KH_BANHANG_ID
                    }).Where(x => x.CountForBrand == model.CountForBrand && x.KH_BANHANG_ID == model.KH_BANHANG_ID && x.KHACHHANG_ID == item.KHACHHANG_ID).ToList();
                    var ins = new CRM_KH_BANHANG_CTIET();
                    if (CRM_KH_BANHANGCT.Count() == 0)
                    {
                        ins.IsDeleted = false;
                        ins.CreatedUserId = WebSecurity.CurrentUserId;
                        ins.ModifiedUserId = WebSecurity.CurrentUserId;
                        ins.CreatedDate = DateTime.Now;
                        ins.ModifiedDate = DateTime.Now;
                        ins.THANG = CRM_KH_BANHANG.THANG;
                        ins.NAM = CRM_KH_BANHANG.NAM;
                        ins.KHACHHANG_ID = item.KHACHHANG_ID;
                        ins.BranchId = (intBrandID == null ? Convert.ToInt32(Session["GlobalCurentBranchId"]) : intBrandID);
                        ins.KH_BANHANG_ID = model.KH_BANHANG_ID;
                        ins.GHI_CHU = item.GHI_CHU;
                        ins.TYLE_THANHCONG = item.TYLE_THANHCONG;
                        ins.TYLE_THANHCONG_REVIEW = 0;
                        //ins.NOIDUNG = "Kế thừa từ tháng trước";
                        CRM_KH_BANHANG_CTIETRepository.InsertCRM_KH_BANHANG_CTIET(ins);

                        //ke thừa don hang tu tháng truoc
                        var oldInvoice = CRM_KH_BANHANGRepository.GetAllvwCRM_Sale_ProductInvoice().Where(x => x.KH_BANHANG_CTIET_ID == item.KH_BANHANG_CTIET_ID).FirstOrDefault();
                        if (oldInvoice != null)
                        {
                            CRM_Sale_ProductInvoice productInvoice = new CRM_Sale_ProductInvoice();
                            //AutoMapper.Mapper.Map(model, productInvoice);
                            productInvoice.IsDeleted = false;
                            productInvoice.CreatedUserId = WebSecurity.CurrentUserId;
                            productInvoice.CreatedDate = DateTime.Now;
                            productInvoice.Status = Wording.OrderStatus_pending;
                            productInvoice.BranchId = (intBrandID == null ? Convert.ToInt32(Session["GlobalCurentBranchId"]) : intBrandID.Value);
                            productInvoice.IsArchive = false;
                            productInvoice.IsReturn = false;
                            productInvoice.Type = "invoice";
                            productInvoice.RemainingAmount = 0;
                            productInvoice.PaidAmount = oldInvoice.PaidAmount;
                            productInvoice.TotalAmount = oldInvoice.TotalAmount;
                            productInvoice.KH_BANHANG_CTIET_ID = ins.KH_BANHANG_CTIET_ID;
                            productInvoice.PaymentMethod = oldInvoice.PaymentMethod;
                            productInvoice.Discount = oldInvoice.Discount;
                            productInvoice.CustomerId = oldInvoice.CustomerId;
                            //hàm thêm mới
                            CRM_KH_BANHANGRepository.InsertProductInvoice(productInvoice, null);



                            //cập nhật lại mã hóa đơn
                            productInvoice.Code = Erp.BackOffice.Helpers.Common.GetOrderNo("CRMProductInvoice", model.Code);

                            //begin kiểm tra trùng mã đơn hàng
                            var c = CRM_KH_BANHANGRepository.GetAllvwCRM_Sale_ProductInvoice().
                                Where(x => x.Code == productInvoice.Code).FirstOrDefault();

                            if (c != null)
                            {
                                TempData[Globals.FailedMessageKey] = "Mã đơn hàng đã bị trùng. Vui lòng kiểm tra lại!";

                                //return RedirectToAction("Create");
                                return RedirectToAction("_ClosePopup", "Home", new { area = "", FunctionCallback = "ClosePopupAndReloadPage" });
                            }
                            //end kiểm tra trùng mã đơn hàng

                            //ghi log 
                            //Erp.BackOffice.Controllers.HomeController.WriteLog(productInvoice.Id, productInvoice.Code, "đã lập đơn hàng dự kiến", "", Helpers.Common.CurrentUser.BranchId.Value);

                            Erp.BackOffice.Helpers.Common.SetOrderNo("CRMProductInvoice");

                            var oldDetail = CRM_KH_BANHANGRepository.GetAllInvoiceDetailsByInvoiceId(oldInvoice.Id).ToList();


                            //lưu danh sách thao tác thực hiện dịch vụ
                            foreach (var item2 in oldDetail.Where(x => x.ProductId > 0))
                            {
                                var ins2 = new CRM_Sale_ProductInvoiceDetail();
                                ins2.IsDeleted = false;
                                ins2.CreatedUserId = WebSecurity.CurrentUserId;
                                ins2.ModifiedUserId = WebSecurity.CurrentUserId;
                                ins2.CreatedDate = DateTime.Now;
                                ins2.ModifiedDate = DateTime.Now;
                                ins2.ExpiryDate = item2.ExpiryDate;
                                ins2.LoCode = item2.LoCode;
                                ins2.ProductId = item2.ProductId;
                                ins2.ProductInvoiceId = productInvoice.Id;
                                ins2.Price = item2.Price;
                                ins2.Quantity = item2.Quantity;
                                ins2.Unit = item2.Unit;
                                ins2.Discount = 0;
                                ins2.DiscountAmount = 0;
                                ins2.Note = item2.Note;
                                ins2.is_TANG = item2.is_TANG;
                                //  ins.ProductType = item.ProductType;
                                CRM_KH_BANHANGRepository.InsertProductInvoiceDetail(ins2);
                            }

                            //Thêm vào quản lý chứng từ
                            TransactionController.Create(new TransactionViewModel
                            {
                                TransactionModule = "ProductInvoice",
                                TransactionCode = productInvoice.Code,
                                TransactionName = "Bán hàng"
                            });
                        }
                    }
                }
                TempData[Globals.SuccessMessageKey] = App_GlobalResources.Wording.UpdateSuccess;
                return RedirectToAction("Index", "CRM_KH_BANHANG");
            }
            else
            {
                using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.Snapshot }))
                {
                    try
                    {
                        var CRM_KH_BANHANG = new CRM_KH_BANHANG();
                        foreach (var item in q)
                        {
                            CRM_KH_BANHANG.IsDeleted = false;
                            CRM_KH_BANHANG.CreatedUserId = WebSecurity.CurrentUserId;
                            CRM_KH_BANHANG.ModifiedUserId = WebSecurity.CurrentUserId;
                            CRM_KH_BANHANG.CreatedDate = DateTime.Now;
                            CRM_KH_BANHANG.ModifiedDate = DateTime.Now;
                            CRM_KH_BANHANG.THANG = monthnow;
                            CRM_KH_BANHANG.NAM = yearnow;
                            CRM_KH_BANHANG.BranchId = (intBrandID == null ? Convert.ToInt32(Session["GlobalCurentBranchId"]) : intBrandID);
                            CRM_KH_BANHANG.CountForBrand = model.CountForBrand;
                            CRM_KH_BANHANG.TARGET_BRAND = item.TARGET_BRAND;
                            CRM_KH_BANHANG.NGUOILAP_ID = model.NGUOILAP_ID;
                            CRM_KH_BANHANGRepository.InsertCRM_KH_BANHANG(CRM_KH_BANHANG);
                        }

                        foreach (var item in CRM_KH_BANHANGS)
                        {
                            var ins = new CRM_KH_BANHANG_CTIET();
                            ins.IsDeleted = false;
                            ins.CreatedUserId = WebSecurity.CurrentUserId;
                            ins.ModifiedUserId = WebSecurity.CurrentUserId;
                            ins.CreatedDate = DateTime.Now;
                            ins.ModifiedDate = DateTime.Now;
                            ins.THANG = CRM_KH_BANHANG.THANG;
                            ins.NAM = CRM_KH_BANHANG.NAM;
                            ins.KHACHHANG_ID = item.KHACHHANG_ID;
                            ins.BranchId = (intBrandID == null ? Convert.ToInt32(Session["GlobalCurentBranchId"]) : intBrandID);
                            ins.KH_BANHANG_ID = CRM_KH_BANHANG.KH_BANHANG_ID;
                            ins.GHI_CHU = item.GHI_CHU;
                            ins.TYLE_THANHCONG = item.TYLE_THANHCONG;
                            ins.TYLE_THANHCONG_REVIEW = 0;
                            //ins.NOIDUNG = "Kế thừa từ tháng trước";
                            CRM_KH_BANHANG_CTIETRepository.InsertCRM_KH_BANHANG_CTIET(ins);

                            //ke thừa don hang tu tháng truoc
                            var oldInvoice = CRM_KH_BANHANGRepository.GetAllvwCRM_Sale_ProductInvoice().Where(x => x.KH_BANHANG_CTIET_ID == item.KH_BANHANG_CTIET_ID).FirstOrDefault();
                            if (oldInvoice != null)
                            {
                                CRM_Sale_ProductInvoice productInvoice = new CRM_Sale_ProductInvoice();
                                //AutoMapper.Mapper.Map(model, productInvoice);
                                productInvoice.IsDeleted = false;
                                productInvoice.CreatedUserId = WebSecurity.CurrentUserId;
                                productInvoice.CreatedDate = DateTime.Now;
                                productInvoice.Status = Wording.OrderStatus_pending;
                                productInvoice.BranchId = (intBrandID == null ? Convert.ToInt32(Session["GlobalCurentBranchId"]) : intBrandID.Value);
                                productInvoice.IsArchive = false;
                                productInvoice.IsReturn = false;
                                productInvoice.Type = "invoice";
                                productInvoice.RemainingAmount = 0;
                                productInvoice.PaidAmount = oldInvoice.PaidAmount;
                                productInvoice.TotalAmount = oldInvoice.TotalAmount;
                                productInvoice.KH_BANHANG_CTIET_ID = ins.KH_BANHANG_CTIET_ID;
                                productInvoice.PaymentMethod = oldInvoice.PaymentMethod;
                                productInvoice.Discount = oldInvoice.Discount;
                                productInvoice.CustomerId = oldInvoice.CustomerId;
                                //hàm thêm mới
                                CRM_KH_BANHANGRepository.InsertProductInvoice(productInvoice, null);



                                //cập nhật lại mã hóa đơn
                                productInvoice.Code = Erp.BackOffice.Helpers.Common.GetOrderNo("CRMProductInvoice", model.Code);

                                //begin kiểm tra trùng mã đơn hàng
                                var c = CRM_KH_BANHANGRepository.GetAllvwCRM_Sale_ProductInvoice().
                                    Where(x => x.Code == productInvoice.Code).FirstOrDefault();

                                if (c != null)
                                {
                                    TempData[Globals.FailedMessageKey] = "Mã đơn hàng đã bị trùng. Vui lòng kiểm tra lại!";

                                    //return RedirectToAction("Create");
                                    return RedirectToAction("_ClosePopup", "Home", new { area = "", FunctionCallback = "ClosePopupAndReloadPage" });
                                }
                                //end kiểm tra trùng mã đơn hàng

                                //ghi log 
                                //Erp.BackOffice.Controllers.HomeController.WriteLog(productInvoice.Id, productInvoice.Code, "đã lập đơn hàng dự kiến", "", Helpers.Common.CurrentUser.BranchId.Value);

                                Erp.BackOffice.Helpers.Common.SetOrderNo("CRMProductInvoice");

                                var oldDetail = CRM_KH_BANHANGRepository.GetAllInvoiceDetailsByInvoiceId(oldInvoice.Id).ToList();


                                //lưu danh sách thao tác thực hiện dịch vụ
                                foreach (var item2 in oldDetail.Where(x => x.ProductId > 0))
                                {
                                    var ins2 = new CRM_Sale_ProductInvoiceDetail();
                                    ins2.IsDeleted = false;
                                    ins2.CreatedUserId = WebSecurity.CurrentUserId;
                                    ins2.ModifiedUserId = WebSecurity.CurrentUserId;
                                    ins2.CreatedDate = DateTime.Now;
                                    ins2.ModifiedDate = DateTime.Now;
                                    ins2.ExpiryDate = item2.ExpiryDate;
                                    ins2.LoCode = item2.LoCode;
                                    ins2.ProductId = item2.ProductId;
                                    ins2.ProductInvoiceId = productInvoice.Id;
                                    ins2.Price = item2.Price;
                                    ins2.Quantity = item2.Quantity;
                                    ins2.Unit = item2.Unit;
                                    ins2.Discount = 0;
                                    ins2.DiscountAmount = 0;
                                    ins2.Note = item2.Note;
                                    ins2.is_TANG = item2.is_TANG;
                                    //  ins.ProductType = item.ProductType;
                                    CRM_KH_BANHANGRepository.InsertProductInvoiceDetail(ins2);
                                }

                                //Thêm vào quản lý chứng từ
                                TransactionController.Create(new TransactionViewModel
                                {
                                    TransactionModule = "ProductInvoice",
                                    TransactionCode = productInvoice.Code,
                                    TransactionName = "Bán hàng"
                                });
                            }
                        }
                        scope.Complete();
                        TempData[Globals.SuccessMessageKey] = App_GlobalResources.Wording.UpdateSuccess;
                        return RedirectToAction("Index", "CRM_KH_BANHANG");
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
        public JsonResult Delete(CRM_KH_BANHANG_CTIETViewModel model)
        {
            try
            {
                var item = CRM_KH_BANHANGRepository.GetCRM_KH_BANHANG_CTById(model.KH_BANHANG_CTIET_ID);

                if (item != null)
                {
                    item.IsDeleted = true;
                    CRM_KH_BANHANGRepository.UpdateCRM_KH_BANHANG_CT(item);
                }

                if (model.ProductInvoiceId != null)
                {
                    var item1 = CRM_KH_BANHANGRepository.GetCRM_Sale_ProductInvoiceById(model.ProductInvoiceId.Value);
                    //var item2 = CRM_KH_BANHANGRepository.Get
                    if (item1 != null)
                    {
                        item1.IsDeleted = true;
                        CRM_KH_BANHANGRepository.UpdateCRMProductInvoice(item1);
                    }
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

        #region Print

        public ActionResult Print_KH_BH(int id, string month, string year)
        {

            var model = new TemplatePrintViewModel();
            var model2 = new TemplatePrintViewModel();
            var model3 = new TemplatePrintViewModel();
            var model4 = new TemplatePrintViewModel();
            //lấy logo công ty
            var logo = Erp.BackOffice.Helpers.Common.GetSetting("LogoCompany");
            var company = Erp.BackOffice.Helpers.Common.GetSetting("companyName");
            var address = Erp.BackOffice.Helpers.Common.GetSetting("addresscompany");
            var phone = Erp.BackOffice.Helpers.Common.GetSetting("phonecompany");
            var fax = Erp.BackOffice.Helpers.Common.GetSetting("faxcompany");
            var bankcode = Erp.BackOffice.Helpers.Common.GetSetting("bankcode");
            var bankname = Erp.BackOffice.Helpers.Common.GetSetting("bankname");
            var ImgLogo = "<div class=\"logo\"><img src=" + logo + " height=\"73\" /></div>";

            //lấy template phiếu xuất.
            //var template = templatePrintRepository.GetAllTemplatePrint().Where(x => x.Code.Contains("Print_kh_bh")).OrderByDescending(x => x.CreatedDate).FirstOrDefault();
            //truyền dữ liệu vào template.
            //model.Content = template.Content;
            //if (month != null && year != null)
            //    model.Content = model.Content.Replace("{DataTable}", buildHtmlDetailList_PrintBCTK(id, Int32.Parse(month), Int32.Parse(year), "ANNAYAKE"));
            //model.Content = model.Content.Replace("{System.Logo}", ImgLogo);
            //model.Content = model.Content.Replace("{System.CompanyName}", company);
            //model.Content = model.Content.Replace("{System.AddressCompany}", address);
            //model.Content = model.Content.Replace("{System.PhoneCompany}", phone);
            //model.Content = model.Content.Replace("{System.Fax}", fax);
            //model.Content = model.Content.Replace("{System.BankCodeCompany}", bankcode);
            //model.Content = model.Content.Replace("{System.BankNameCompany}", bankname);
            //model.Content = model.Content.Replace("{PrintDate}", DateTime.Now.ToString("dd/MM/yyyy HH:mm"));

            //model2.Content = template.Content;
            //if (month != null && year != null)
            //    model2.Content = model2.Content.Replace("{DataTable}", buildHtmlDetailList_PrintBCTK(id, Int32.Parse(month), Int32.Parse(year), "ORLANEPARIS"));
            //model2.Content = model2.Content.Replace("{System.Logo}", ImgLogo);
            //model2.Content = model2.Content.Replace("{System.CompanyName}", company);
            //model2.Content = model2.Content.Replace("{System.AddressCompany}", address);
            //model2.Content = model2.Content.Replace("{System.PhoneCompany}", phone);
            //model2.Content = model2.Content.Replace("{System.Fax}", fax);
            //model2.Content = model2.Content.Replace("{System.BankCodeCompany}", bankcode);
            //model2.Content = model2.Content.Replace("{System.BankNameCompany}", bankname);
            //model2.Content = model2.Content.Replace("{PrintDate}", DateTime.Now.ToString("dd/MM/yyyy HH:mm"));

            //model3.Content = template.Content;
            //if (month != null && year != null)
            //    model3.Content = model3.Content.Replace("{DataTable}", buildHtmlDetailList_PrintBCTK(id, Int32.Parse(month), Int32.Parse(year), "CONGNGHECAO"));
            //model3.Content = model3.Content.Replace("{System.Logo}", ImgLogo);
            //model3.Content = model3.Content.Replace("{System.CompanyName}", company);
            //model3.Content = model3.Content.Replace("{System.AddressCompany}", address);
            //model3.Content = model3.Content.Replace("{System.PhoneCompany}", phone);
            //model3.Content = model3.Content.Replace("{System.Fax}", fax);
            //model3.Content = model3.Content.Replace("{System.BankCodeCompany}", bankcode);
            //model3.Content = model3.Content.Replace("{System.BankNameCompany}", bankname);
            //model3.Content = model3.Content.Replace("{PrintDate}", DateTime.Now.ToString("dd/MM/yyyy HH:mm"));

            //model4.Content = template.Content;
            //if (month != null && year != null)
            //    model4.Content = model.Content.Replace("{DataTable}", buildHtmlDetailList_PrintBCTK(id, Int32.Parse(month), Int32.Parse(year), "DICHVU"));
            //model4.Content = model4.Content.Replace("{System.Logo}", ImgLogo);
            //model4.Content = model4.Content.Replace("{System.CompanyName}", company);
            //model4.Content = model4.Content.Replace("{System.AddressCompany}", address);
            //model4.Content = model4.Content.Replace("{System.PhoneCompany}", phone);
            //model4.Content = model4.Content.Replace("{System.Fax}", fax);
            //model4.Content = model4.Content.Replace("{System.BankCodeCompany}", bankcode);
            //model4.Content = model4.Content.Replace("{System.BankNameCompany}", bankname);
            //model4.Content = model4.Content.Replace("{PrintDate}", DateTime.Now.ToString("dd/MM/yyyy HH:mm"));


            DataTable dt1 = new DataTable("ANNAYAKE");
            for (int i = 0; i < 11; i++)
                dt1.Columns.Add();
            dt1.Rows.Add(company);
            dt1.Rows.Add("Địa chỉ :" + address);
            dt1.Rows.Add("Điện thoại : " + phone + " - " + "fax: " + fax);
            dt1.Rows.Add("STK :" + bankcode + " - " + "Ngân hàng :" + bankname);
            dt1.Rows.Add("KẾ HOẠCH BÁN HÀNG");
            buildHtmlDetailList_PrintBCKHBH(dt1, id, Int32.Parse(month), Int32.Parse(year), "ANNAYAKE");

            DataTable dt2 = new DataTable("ORLANE PARIS");
            for (int i = 0; i < 11; i++)
                dt2.Columns.Add();
            dt2.Rows.Add(company);
            dt2.Rows.Add("Địa chỉ :" + address);
            dt2.Rows.Add("Điện thoại : " + phone + " - " + "fax: " + fax);
            dt2.Rows.Add("STK :" + bankcode + " - " + "Ngân hàng :" + bankname);
            dt2.Rows.Add("KẾ HOẠCH BÁN HÀNG");
            buildHtmlDetailList_PrintBCKHBH(dt2, id, Int32.Parse(month), Int32.Parse(year), "ORLANEPARIS");

            DataTable dt3 = new DataTable("LEONORGREYL");
            for (int i = 0; i < 11; i++)
                dt3.Columns.Add();
            dt3.Rows.Add(company);
            dt3.Rows.Add("Địa chỉ :" + address);
            dt3.Rows.Add("Điện thoại : " + phone + " - " + "fax: " + fax);
            dt3.Rows.Add("STK :" + bankcode + " - " + "Ngân hàng :" + bankname);
            dt3.Rows.Add("KẾ HOẠCH BÁN HÀNG");
            buildHtmlDetailList_PrintBCKHBH(dt3, id, Int32.Parse(month), Int32.Parse(year), "LEONORGREYL");

            DataTable dt4 = new DataTable("CONG NGHE CAO");
            for (int i = 0; i < 11; i++)
                dt4.Columns.Add();
            dt4.Rows.Add(company);
            dt4.Rows.Add("Địa chỉ :" + address);
            dt4.Rows.Add("Điện thoại : " + phone + " - " + "fax: " + fax);
            dt4.Rows.Add("STK :" + bankcode + " - " + "Ngân hàng :" + bankname);
            dt4.Rows.Add("KẾ HOẠCH BÁN HÀNG");
            buildHtmlDetailList_PrintBCKHBH(dt4, id, Int32.Parse(month), Int32.Parse(year), "CONGNGHECAO");

            DataTable dt5 = new DataTable("DICH VU");
            for (int i = 0; i < 11; i++)
                dt5.Columns.Add();
            dt5.Rows.Add(company);
            dt5.Rows.Add("Địa chỉ :" + address);
            dt5.Rows.Add("Điện thoại : " + phone + " - " + "fax: " + fax);
            dt5.Rows.Add("STK :" + bankcode + " - " + "Ngân hàng :" + bankname);
            dt5.Rows.Add("KẾ HOẠCH BÁN HÀNG");
            buildHtmlDetailList_PrintBCKHBH(dt5, id, Int32.Parse(month), Int32.Parse(year), "DICHVU");

            var ds = new DataSet();
            ds.Tables.Add(dt1);
            ds.Tables.Add(dt2);
            ds.Tables.Add(dt3);
            ds.Tables.Add(dt4);
            ds.Tables.Add(dt5);
            ExcelHelper.ToExcel(ds, "KeHoachBanHang" + DateTime.Now.ToString("yyyyMMdd") + ".xls", Response);

            //if (ExportExcel)
            //{
            //Response.AppendHeader("content-disposition", "attachment;filename=" + "Kế hoạch bán hàng" + DateTime.Now.ToString("yyyyMMdd") + ".xls");
            //Response.Charset = "";
            //Response.Cache.SetCacheability(HttpCacheability.NoCache);
            //Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            //Response.Write(model.Content);
            //Response.Flush();
            //Response.Close();
            //Response.End();
            //}


            return View(model);
        }
        void buildHtmlDetailList_PrintBCKHBH(DataTable dt, int id, int month, int year, string brand)
        {
            var model = new CRM_KH_BANHANGViewModel();
            IEnumerable<CRM_KH_BANHANGViewModel> q = CRM_KH_BANHANGRepository.GetAllvwCRM_KH_BANHANG()
                .Select(item => new CRM_KH_BANHANGViewModel
                {
                    KH_BANHANG_ID = item.KH_BANHANG_ID,
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
                }).Where(x => x.NGUOILAP_ID == id && x.THANG == month && x.NAM == year).ToList();
            foreach (var item in q)
            {
                model.BranchId = item.BranchId;
                model.IsDeleted = item.IsDeleted;
                model.CreatedDate = item.CreatedDate;
                model.CreatedUserId = item.CreatedUserId;
                model.ModifiedDate = item.ModifiedDate;
                model.ModifiedUserId = item.ModifiedUserId;
                model.CountForBrand = item.CountForBrand;
                model.GHI_CHU = item.GHI_CHU;
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
            model.CRM_KH_BANHANGList = CRM_KH_BANHANGRepository.GetAllvwCRM_KH_BANHANG_CTIET().Select(x => new CRM_KH_BANHANG_CTIETViewModel
            {
                KH_BANHANG_ID = x.KH_BANHANG_ID,
                KH_BANHANG_CTIET_ID = x.KH_BANHANG_CTIET_ID,
                Phone = x.Phone,
                KHACHHANG_ID = x.KHACHHANG_ID,
                CompanyName = x.CompanyName,
                Code = x.Code,
                TYLE_THANHCONG = x.TYLE_THANHCONG,
                TYLE_THANHCONG_REVIEW = x.TYLE_THANHCONG_REVIEW,
                GHI_CHU = x.GHI_CHU,
                IsDeleted = x.IsDeleted,
                THANG = x.THANG,
                NAM = x.NAM,
                CRM_Sale_ProductInvoiceCode = x.CRM_Sale_ProductInvoiceCode,
                CountForBrand = x.CountForBrand,
                TotalAmount = x.TotalAmount,
                ProductInvoiceId = x.ProductInvoiceId,
                NGUOILAP_ID = x.NGUOILAP_ID,
                NOIDUNG = x.NOIDUNG,
            }).Where(x => x.IsDeleted == false && x.THANG == month && x.NAM == year && x.NGUOILAP_ID == id).ToList();

            //Tạo table html chi tiết phiếu xuất
            //string detailLists = "";
            //detailLists += "<thead>\r\n";
            //detailLists += "	<tr>\r\n";
            //detailLists += "		<th>STT</th>\r\n";
            //detailLists += "		<th>Tên khách hàng</th>\r\n";
            //detailLists += "		<th>Mã khách hàng</th>\r\n";
            //detailLists += "		<th>Điện thoại</th>\r\n";
            //detailLists += "		<th>Thông tin lưu ý</th>\r\n";
            //detailLists += "		<th>Tỷ lệ thành công(%)</th>\r\n";
            //detailLists += "		<th>Ghi chú</th>\r\n";
            //detailLists += "		<th>Đơn hàng PS</th>\r\n";
            //detailLists += "		<th>Đánh giá lại</th>\r\n";
            //detailLists += "		<th>Lịch sử tương tác</th>\r\n";
            //detailLists += "		<th>Đơn hàng dự kiến</th>\r\n";
            //detailLists += "	</tr>\r\n";
            //detailLists += "</thead>\r\n";

            dt.Rows.Add("STT", "Tên khách hàng", "Mã khách hàng", "Điện thoại", "Thông tin lưu ý", "Tỷ lệ thành công(%)", "Ghi chú", "Đơn hàng PS", "Đánh giá lại", "Lịch sử tương tác", "Đơn hàng dự kiến");

            var index = 1;


            foreach (var item in model.CRM_KH_BANHANGList)
            {
                if (item.CountForBrand.Replace(" ", "") == brand)
                {
                    dt.Rows.Add((index++), item.CompanyName, item.KHACHHANG_ID, item.Phone, item.NOIDUNG, item.TYLE_THANHCONG, item.GHI_CHU, " ", item.TYLE_THANHCONG_REVIEW, " ", item.TotalAmount);
                    //detailLists += "<tr>\r\n"
                    //+ "<td class=\"text-center orderNo\">" + (index++) + "</td>\r\n"
                    //+ "<td class=\"text-left \">" + item.CompanyName + "</td>\r\n"
                    //+ "<td class=\"text-left \">" + item.KHACHHANG_ID + "</td>\r\n"
                    //+ "<td class=\"text-left \">" + item.Phone + "</td>\r\n"
                    //+ "<td class=\"text-left \">" + item.NOIDUNG + "</td>\r\n"
                    //+ "<td class=\"text-left \">" + item.TYLE_THANHCONG + "</td>\r\n"
                    //+ "<td class=\"text-right\">" + item.GHI_CHU + "</td>\r\n"
                    //+ "<td class=\"text-right\">" +  " "+ "</td>\r\n"
                    //+ "<td class=\"text-center\">" + item.TYLE_THANHCONG_REVIEW + "</td>\r\n"
                    //+ "<td class=\"text-center\">" + item.TotalAmount + "</td>\r\n"
                    //+ "</tr>\r\n";
                }
            }
        }
        #endregion

        #region Cập nhật tỉ lệ thành công
        [AllowAnonymous]
        [HttpPost]
        public JsonResult UpdateRate(int id, int khid, int tyle, int tylerw)
        {
            var CRM_KH_BANHANG_CT = CRM_KH_BANHANGRepository.GetCRM_KH_BANHANG_CTById(id);
            CRM_KH_BANHANG_CT.TYLE_THANHCONG = tyle;
            CRM_KH_BANHANG_CT.TYLE_THANHCONG_REVIEW = tylerw;
            CRM_KH_BANHANG_CT.ModifiedDate = DateTime.Now;
            CRM_KH_BANHANG_CT.ModifiedUserId = WebSecurity.CurrentUserId;
            CRM_KH_BANHANGRepository.UpdateCRM_KH_BANHANG_CT(CRM_KH_BANHANG_CT);
            return Json(1);
        }

        [AllowAnonymous]
        [HttpPost]
        public JsonResult UpdateTarget(int id, decimal? target, string Note)
        {
            var CRM_KH_BANHANG = CRM_KH_BANHANGRepository.GetCRM_KH_BANHANGById(id);
            //CRM_KH_BANHANG_CT.TYLE_THANHCONG = tyle;
            //CRM_KH_BANHANG_CT.TYLE_THANHCONG_REVIEW = tylerw;
            CRM_KH_BANHANG.GHI_CHU = Note;
            CRM_KH_BANHANG.TARGET_BRAND = target;
            CRM_KH_BANHANG.ModifiedDate = DateTime.Now;
            CRM_KH_BANHANG.ModifiedUserId = WebSecurity.CurrentUserId;
            CRM_KH_BANHANGRepository.UpdateCRM_KH_BANHANG(CRM_KH_BANHANG);
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
                    var kh_banhang = CRM_KH_BANHANGRepository.GetAllCRM_KH_BANHANG().Where(x => x.NAM == year &&
                    x.THANG == month && x.NGUOILAP_ID == id && x.BranchId == intBrandID).ToList();
                    if (kh_banhang.Count > 0)
                    {
                        foreach (var item in kh_banhang)
                        {
                            var chitiet = CRM_KH_BANHANGRepository.GetAllvwCRM_KH_BANHANG_CTIET().Where(x => x.KH_BANHANG_ID == item.KH_BANHANG_ID).ToList();
                            if (chitiet.Count > 0)
                            {
                                foreach (var item2 in chitiet)
                                {
                                    CRM_KH_BANHANG_CTIETRepository.DeleteCRM_KH_BANHANG_CTIETRs(item2.KH_BANHANG_CTIET_ID);
                                }
                            }
                            CRM_KH_BANHANGRepository.DeleteCRM_KH_BANHANGRs(item.KH_BANHANG_ID);
                        }
                    }


                }

                scope.Complete();
            }
            TempData[Globals.SuccessMessageKey] = App_GlobalResources.Wording.UpdateSuccess;
            return RedirectToAction("Index");
        }
        #endregion

        #region Chuyen Ke Hoach tu thang nay qua tháng khác

        [HttpPost]
        public ActionResult ChuyenKHBH(int? KHID, int? Thang, int? Nam)
        {
            //var a = Request["KHID"];
            var Chitiet = CRM_KH_BANHANGRepository.GetCRM_KH_BANHANG_CTById(KHID.Value);
            var KHcu = CRM_KH_BANHANGRepository.GetCRM_KH_BANHANGById(Chitiet.KH_BANHANG_ID.Value);

            //Tim kH cua thang dc chuyen qua
            var KHmoi = CRM_KH_BANHANGRepository.GetAllCRM_KH_BANHANG()
                .Where(x => x.NGUOILAP_ID == KHcu.NGUOILAP_ID && x.CountForBrand == KHcu.CountForBrand && x.NAM == Nam && x.THANG == Thang).FirstOrDefault();

            if (KHmoi != null)
            {
                using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.Snapshot }))
                {
                    try
                    {

                        var ins = new CRM_KH_BANHANG_CTIET();
                        ins.IsDeleted = false;
                        ins.CreatedUserId = WebSecurity.CurrentUserId;
                        ins.ModifiedUserId = WebSecurity.CurrentUserId;
                        ins.CreatedDate = DateTime.Now;
                        ins.ModifiedDate = DateTime.Now;
                        ins.THANG = Thang;
                        ins.NAM = Nam;
                        ins.KHACHHANG_ID = Chitiet.KHACHHANG_ID;
                        ins.BranchId = Chitiet.BranchId;
                        ins.KH_BANHANG_ID = KHmoi.KH_BANHANG_ID;
                        ins.GHI_CHU = Chitiet.GHI_CHU;
                        ins.TYLE_THANHCONG = Chitiet.TYLE_THANHCONG;
                        ins.TYLE_THANHCONG_REVIEW = 0;
                        //ins.NOIDUNG = "Kế thừa từ tháng trước";
                        CRM_KH_BANHANG_CTIETRepository.InsertCRM_KH_BANHANG_CTIET(ins);

                        //ke thừa don hang tu tháng truoc
                        var oldInvoice = CRM_KH_BANHANGRepository.GetAllvwCRM_Sale_ProductInvoice().Where(x => x.KH_BANHANG_CTIET_ID == Chitiet.KH_BANHANG_CTIET_ID).FirstOrDefault();
                        if (oldInvoice != null)
                        {
                            CRM_Sale_ProductInvoice productInvoice = new CRM_Sale_ProductInvoice();
                            //AutoMapper.Mapper.Map(model, productInvoice);
                            productInvoice.IsDeleted = false;
                            productInvoice.CreatedUserId = WebSecurity.CurrentUserId;
                            productInvoice.CreatedDate = DateTime.Now;
                            productInvoice.Status = Wording.OrderStatus_pending;
                            productInvoice.BranchId = oldInvoice.BranchId;
                            productInvoice.IsArchive = false;
                            productInvoice.IsReturn = false;
                            productInvoice.Type = "invoice";
                            productInvoice.RemainingAmount = 0;
                            productInvoice.PaidAmount = oldInvoice.PaidAmount;
                            productInvoice.TotalAmount = oldInvoice.TotalAmount;
                            productInvoice.KH_BANHANG_CTIET_ID = ins.KH_BANHANG_CTIET_ID;
                            productInvoice.PaymentMethod = oldInvoice.PaymentMethod;
                            productInvoice.Discount = oldInvoice.Discount;
                            productInvoice.CustomerId = oldInvoice.CustomerId;
                            productInvoice.CountForBrand = oldInvoice.CountForBrand;
                            //hàm thêm mới
                            CRM_KH_BANHANGRepository.InsertProductInvoice(productInvoice, null);



                            //cập nhật lại mã hóa đơn
                            productInvoice.Code = Erp.BackOffice.Helpers.Common.GetOrderNo("CRMProductInvoice", null);

                            //begin kiểm tra trùng mã đơn hàng
                            var c = CRM_KH_BANHANGRepository.GetAllvwCRM_Sale_ProductInvoice().
                                Where(x => x.Code == productInvoice.Code).FirstOrDefault();

                            if (c != null)
                            {
                                TempData[Globals.FailedMessageKey] = "Mã đơn hàng đã bị trùng. Vui lòng kiểm tra lại!";

                                //return RedirectToAction("Create");
                                return RedirectToAction("_ClosePopup", "Home", new { area = "", FunctionCallback = "ClosePopupAndReloadPage" });
                            }
                            //end kiểm tra trùng mã đơn hàng

                            //ghi log 
                            //Erp.BackOffice.Controllers.HomeController.WriteLog(productInvoice.Id, productInvoice.Code, "đã lập đơn hàng dự kiến", "", Helpers.Common.CurrentUser.BranchId.Value);

                            Erp.BackOffice.Helpers.Common.SetOrderNo("CRMProductInvoice");

                            var oldDetail = CRM_KH_BANHANGRepository.GetAllInvoiceDetailsByInvoiceId(oldInvoice.Id).ToList();


                            //lưu danh sách thao tác thực hiện dịch vụ
                            foreach (var item2 in oldDetail.Where(x => x.ProductId > 0))
                            {
                                var ins2 = new CRM_Sale_ProductInvoiceDetail();
                                ins2.IsDeleted = false;
                                ins2.CreatedUserId = WebSecurity.CurrentUserId;
                                ins2.ModifiedUserId = WebSecurity.CurrentUserId;
                                ins2.CreatedDate = DateTime.Now;
                                ins2.ModifiedDate = DateTime.Now;
                                ins2.ExpiryDate = item2.ExpiryDate;
                                ins2.LoCode = item2.LoCode;
                                ins2.ProductId = item2.ProductId;
                                ins2.ProductInvoiceId = productInvoice.Id;
                                ins2.Price = item2.Price;
                                ins2.Quantity = item2.Quantity;
                                ins2.Unit = item2.Unit;
                                ins2.Discount = 0;
                                ins2.DiscountAmount = 0;
                                ins2.Note = item2.Note;
                                ins2.is_TANG = item2.is_TANG;
                                //  ins.ProductType = item.ProductType;
                                CRM_KH_BANHANGRepository.InsertProductInvoiceDetail(ins2);
                            }

                            //Thêm vào quản lý chứng từ
                            TransactionController.Create(new TransactionViewModel
                            {
                                TransactionModule = "ProductInvoice",
                                TransactionCode = productInvoice.Code,
                                TransactionName = "Bán hàng"
                            });
                        }
                        scope.Complete();
                    }
                    catch (Exception ex)
                    {

                    }
                }
            }
            else
            {
                return Json(0);
            }
            return Json(1);
        }
        #endregion


        #region AddPromotionCRM
        public ActionResult AddPromotionCRM(int Id)
        {

            ProductInvoiceViewModel model = new ProductInvoiceViewModel();
            model.InvoiceList = new List<ProductInvoiceDetailViewModel>();
            //model.commission = new CommissionCusViewModel();
            //model.commission.DetailList = new List<CommisionCustomerViewModel>();
            //model.commission.InvoiceDetailList = new List<CommisionInvoiceViewModel>();
            //model.LogCommission = new List<LogPromotionViewModel>();
            if (Id > 0)
            {
                var productInvoice = CRM_KH_BANHANGRepository.GetvwCRM_Sale_ProductInvoiceById(Id);
                AutoMapper.Mapper.Map(productInvoice, model);
                decimal DC = (decimal)model.Discount;
                model.DiscountAmount = model.TotalAmount * DC / (100 - (decimal)model.Discount);
                ///chi tiet đơn hàng
                model.InvoiceList = CRM_KH_BANHANGRepository.GetAllvwInvoiceDetailsByInvoiceId(productInvoice.Id).Select(x => new ProductInvoiceDetailViewModel
                {
                    Id = x.Id,
                    Amount = x.Amount,
                    CategoryCode = x.CategoryCode,
                    ExpiryDate = x.ExpiryDate,
                    Unit = x.Unit,
                    LoCode = x.LoCode,
                    Price = x.Price,
                    ProductCode = x.ProductCode,
                    ProductGroup = x.ProductGroup,
                    ProductId = x.ProductId,
                    ProductName = x.ProductName,
                    Quantity = x.Quantity,
                    ProductType = x.ProductType,
                    DiscountAmount = x.DiscountAmount,
                    Discount = x.Discount
                }).ToList();

            }
            return View(model);
        }



        [HttpPost]
        public ActionResult AddPromotionCRM(ProductInvoiceViewModel model)
        {
            var productInvoice = new CRM_Sale_ProductInvoice();// productInvoice = null;

            if (model.Id > 0)
            {

                var productInvoice1 = CRM_KH_BANHANGRepository.GetvwCRM_Sale_ProductInvoiceById(model.Id);
                AutoMapper.Mapper.Map(productInvoice1, productInvoice);
            }



            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.Snapshot }))
            {
                try
                {
                    if (productInvoice != null)
                    {

                        var _listdata = CRM_KH_BANHANGRepository.GetAllInvoiceDetailsByInvoiceId(productInvoice.Id).ToList();
                        if (model.InvoiceList.Any(x => x.Id > 0))
                        {
                            var update = _listdata.Where(id1 => model.InvoiceList.ToList().Any(id2 => id2.Id == id1.Id)).ToList();
                            //lưu danh sách thao tác thực hiện dịch vụ
                            foreach (var item in model.InvoiceList)
                            {
                                var _update = update.FirstOrDefault(x => x.Id == item.Id);
                                _update.Discount = item.Discount;
                                _update.DiscountAmount = item.DiscountAmount;

                                CRM_KH_BANHANGRepository.UpdateDetailsInvoice(_update);
                            }
                        }


                        //begin hoapd tinh lai hoa don 
                        _listdata = CRM_KH_BANHANGRepository.GetAllInvoiceDetailsByInvoiceId(productInvoice.Id).ToList();
                        decimal Tongtien = 0;
                        foreach (var item in _listdata)
                        {
                            if (item.is_TANG != 1)
                            {
                                Tongtien = Tongtien + Math.Round(Helpers.Common.NVL_NUM_DECIMAL_NEW(item.Quantity * item.Price)) - Helpers.Common.NVL_NUM_DECIMAL_NEW(item.DiscountAmount);
                            }
                        }
                        decimal TongTyleGiamgia = Helpers.Common.NVL_NUM_DECIMAL_NEW(productInvoice.Discount_DB) + Helpers.Common.NVL_NUM_DECIMAL_NEW(productInvoice.Discount_KM) + Helpers.Common.NVL_NUM_DECIMAL_NEW(productInvoice.Discount_VIP);
                        decimal TongtienGiam = Math.Round((Tongtien / 100) * TongTyleGiamgia);
                        Tongtien = Tongtien - TongtienGiam;
                        Tongtien = Math.Round(Tongtien + (Helpers.Common.NVL_NUM_DECIMAL_NEW(productInvoice.TaxFee) * (Tongtien / 100)));

                        if ((productInvoice != null) && (productInvoice.Id > 0))
                        {
                            //Nếu đã ghi sổ rồi thì không được sửa

                            int productInvoiceID = productInvoice.Id;

                            productInvoice.Type = "invoice";
                            productInvoice.IsReturn = false;
                            productInvoice.Id = productInvoiceID;
                            productInvoice.ModifiedUserId = WebSecurity.CurrentUserId;
                            productInvoice.ModifiedDate = DateTime.Now;
                            productInvoice.RemainingAmount = Tongtien;
                            //productInvoice.DiscountAmount = TongtienGiam;
                            productInvoice.Discount = (double)TongTyleGiamgia;
                            productInvoice.PaidAmount = 0;
                            productInvoice.TotalAmount = Tongtien;
                            CRM_KH_BANHANGRepository.UpdateCRMProductInvoice(productInvoice);
                        }

                    }

                    scope.Complete();
                }
                catch (DbUpdateException)
                {
                    TempData[Globals.FailedMessageKey] = App_GlobalResources.Wording.Error;
                    return View(model);
                }
            }
            // return RedirectToAction("DetailProductInvoice", new { Id = productInvoice.Id });
            return RedirectToAction("_ClosePopup", "Home", new { area = "", FunctionCallback = "ClosePopupAndReloadPage" });
        }

        #endregion


        #region Theo doi tien do ke hoach
        public ActionResult FollowPlan(int? NGUOILAP_ID, string CountForBrand)
        {

            var origin = categoryRepository.GetAllCategories().Select(item => new CategoryViewModel
            {
                Name = item.Name,
                Value = item.Value,
                Code = item.Code
            }).Where(item => item.Code == "Origin").ToList();
            ViewBag.Origin = origin;
            CountForBrand = CountForBrand == null ? origin.FirstOrDefault().Value : CountForBrand;
            //NGUOILAP_ID = 14791;
            //get cookie brachID 
            HttpRequestBase request = this.HttpContext.Request;
            var u = userRepository.GetUserById(WebSecurity.CurrentUserId);
            string strBrandID = "0";
            //ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
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
            //branchId = intBrandID;
            int? GlobalCurentBranchId = Convert.ToInt32(Session["GlobalCurentBranchId"]);
            //branchId = (Erp.BackOffice.Helpers.Common.CurrentUser.BranchId == null ? GlobalCurentBranchId : Erp.BackOffice.Helpers.Common.CurrentUser.BranchId);

            //NGUOILAP = NGUOILAP == null ? WebSecurity.CurrentUserId : NGUOILAP;
            var dataNhanvien = SqlHelper.QuerySP<ManagerStaff>("spgetNhanvienbacap", new
            {
                BranchId = intBrandID,
                UserId = WebSecurity.CurrentUserId,
            }).ToList();

            List<int> listNhanvien = new List<int>();


            for (int i = 0; i < dataNhanvien.Count; i++)
            {
                listNhanvien.Add(int.Parse(dataNhanvien[i].Id.ToString()));
            }





            // var q = CRM_KH_BANHANGRepository.GetAllvwCRM_KH_BANHANG();
            var user = userRepository.GetAllUsers().Select(item => new UserViewModel
            {
                Id = item.Id,
                UserName = item.UserName,
                FullName = item.FullName,
                Status = item.Status,
                BranchId = item.BranchId,
                UserTypeId = item.UserTypeId
            }).Where(x => x.Status == UserStatus.Active && x.UserTypeId == 2026).ToList();
            //if (branchId > 0)
            //{
            //    ViewBag.user2 = user.Where(x => x.BranchId == branchId).ToList();
            //}

            //&& listNhanvien.Contains(x.Id)
            var user2branch = user.Where(x => x.Id == WebSecurity.CurrentUserId).FirstOrDefault();

            if (intBrandID != null && intBrandID > 0)
            {
                // q = q.Where(x => x.BranchId == intBrandID);
                user = user.Where(x => x.BranchId == intBrandID).ToList();
                if (user2branch != null && user2branch.BranchId == 0)
                {
                    user.Add(user2branch);
                }
            }
            ViewBag.user2 = user;
            if (NGUOILAP_ID != null && NGUOILAP_ID > 0)
            {
                // q = q.Where(x => x.NGUOILAP_ID == NGUOILAP_ID);
                user = user.Where(x => x.Id == NGUOILAP_ID).ToList();
            }
            if (NGUOILAP_ID == null)
            {
                user = user.ToList();
            }

            if (!Filters.SecurityFilter.IsAdmin() && u.IsLetan != true && (u.UserTypeId != 1027 || u.UserTypeId != 2053))
            {
                user = user.Where(x => x.Id != 0 && listNhanvien.Contains(x.Id)).ToList();
            }


            var L = new List<TargetViewModel>();
            var date = DateTime.Now.Day;
            var Period = Crm_PeriodRepository.GetAllCrm_Period().Where(x => x.starDate <= date && x.endDate >= date).FirstOrDefault();
            var Target = Crm_TargetRepository.GetAllCrm_Target().Where(x => x.IdPeriod == Period.Id).ToList();
            var Level = Crm_LevelRepository.GetAllCrm_Level().Select(item => new Crm_LevelViewModel
            {
                Id = item.Id,
                Name = item.Name,
                StaredIndex = item.StaredIndex,
                EndIndex = item.EndIndex

            }).ToList();
            foreach (var itemuser in user)
            {


                //Lấy ke hoach ban hang
                var KHBHCT = CRM_KH_BANHANGRepository.GetAllvwCRM_KH_BANHANG_CTIET()
                    .Where(x => (x.NAM == DateTime.Now.Year) && (x.THANG == DateTime.Now.Month) && x.NGUOILAP_ID == itemuser.Id  && x.CountForBrand == CountForBrand).ToList();
                var Total = KHBHCT.Count();
                //var L = new List<TargetViewModel>();
                foreach (var item in Level)
                {
                    var target2 = Target.Where(x => x.IdLevel == item.Id).FirstOrDefault();
                    if (target2 != null)
                    {
                        var target = target2.Target;
                        var Li = KHBHCT.Where(x => x.TYLE_THANHCONG >= item.StaredIndex && x.TYLE_THANHCONG <= item.EndIndex).Count();
                        double per = 0;
                        if (Total > 0)
                        {
                            per = Math.Round((float)Li / (float)Total * 100);
                        }
                        var real = new TargetViewModel();
                        real.UserId = itemuser.Id;
                        real.LevelName = item.Name;
                        real.Value = Li;//(int)per;
                        if (per > target)
                        {
                            real.Target = 1;
                        }
                        else
                        {
                            real.Target = 0;
                        }
                        L.Add(real);
                    }
                }
            }
            //var L1 = KHBHCT.Where(x => x.TYLE_THANHCONG)
            ViewBag.Level = Level;
            ViewBag.user = user;
            return View(L);
        }


        public ActionResult FollowPlanBranch(string CountForBrand)
        {
            var origin = categoryRepository.GetAllCategories().Select(item => new CategoryViewModel
            {
                Name = item.Name,
                Value = item.Value,
                Code = item.Code
            }).Where(item => item.Code == "Origin").ToList();
            ViewBag.Origin = origin;
            CountForBrand = CountForBrand == null ? origin.FirstOrDefault().Value : CountForBrand;

            //get cookie brachID 
            HttpRequestBase request = this.HttpContext.Request;
            var u = userRepository.GetUserById(WebSecurity.CurrentUserId);
            string strBrandID = "0";
            //ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
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

            var L = new List<TargetViewModel>();
            var date = DateTime.Now.Day;
            var Period = Crm_PeriodRepository.GetAllCrm_Period().Where(x => x.starDate <= date && x.endDate >= date).FirstOrDefault();
            var Target = Crm_TargetRepository.GetAllCrm_Target().Where(x => x.IdPeriod == Period.Id).ToList();
            var Level = Crm_LevelRepository.GetAllCrm_Level().Select(item => new Crm_LevelViewModel
            {
                Id = item.Id,
                Name = item.Name,
                StaredIndex = item.StaredIndex,
                EndIndex = item.EndIndex

            }).ToList();
            ViewBag.Level = Level;

            var BranchList = BranchRepository.GetAllBranch().Select(x => new BranchViewModel
            {
                Id = x.Id,
                Name = x.Name

            }).ToList();

            if (intBrandID > 0)
            {
                BranchList = BranchList.Where(x => x.Id == intBrandID).ToList();
            }
            ViewBag.Branch = BranchList;
            foreach (var branch in BranchList)
            {
                //Lấy ke hoach ban hang
                var KHBHCT = CRM_KH_BANHANGRepository.GetAllvwCRM_KH_BANHANG_CTIET()
                    .Where(x => (x.NAM == DateTime.Now.Year) && (x.THANG == DateTime.Now.Month) && x.BranchId == branch.Id && x.CountForBrand == CountForBrand).ToList();
                var Total = KHBHCT.Count();
                //var L = new List<TargetViewModel>();
                foreach (var item in Level)
                {
                    var target2 = Target.Where(x => x.IdLevel == item.Id).FirstOrDefault();
                    if (target2 != null)
                    {
                        var target = target2.Target;
                        var Li = KHBHCT.Where(x => x.TYLE_THANHCONG >= item.StaredIndex && x.TYLE_THANHCONG <= item.EndIndex).Count();
                        float per = 0;
                        if (Total > 0)
                        {
                            per = (float)Li / (float)Total * 100;
                        }
                        var real = new TargetViewModel();
                        real.UserId = branch.Id;
                        real.LevelName = item.Name;
                        real.Value = Li;//(int)per;
                        if (per > target)
                        {
                            real.Target = 1;
                        }
                        else
                        {
                            real.Target = 0;
                        }
                        L.Add(real);
                    }
                }
            }

            return View(L);
        }
        #endregion
        #region Tong hop theo doi deal cua nhân vien
        
        public ActionResult TongHopDealNhanVien(int? month, int? year, int? NguoiLap)
        {
            month = month != null ? month : DateTime.Now.Month;
            year = year != null ? year : DateTime.Now.Year;

            //get cookie brachID 
            HttpRequestBase request = this.HttpContext.Request;
            var u = userRepository.GetUserById(WebSecurity.CurrentUserId);
            string strBrandID = "0";
            //ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
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
            var user = userRepository.GetAllUsers().Select(item => new UserViewModel
            {
                Id = item.Id,
                UserName = item.UserName,
                FullName = item.FullName,
                Status = item.Status,
                BranchId = item.BranchId,
                UserTypeId = item.UserTypeId
            }).Where(x => x.Status == UserStatus.Active && x.UserTypeId == 2026).ToList();
            if(intBrandID > 0)
            {
                user = user.Where(x => x.BranchId == intBrandID).ToList();
            }
            if (u.UserTypeId == 2026)
            {
                user = user.Where(x => x.Id == u.Id).ToList();
            }
            NguoiLap = NguoiLap != null ? NguoiLap : 0;
            ViewBag.NguoiLap = NguoiLap;
            ViewBag.user = user;
            //getDoanhSoTheoLevel
            var model = new List<TongHopDealViewModel>();
            //Nhãn Anna
            var Anna = SqlHelper.QuerySP<TongHopDealViewModel>("getDoanhSoTheoLevel", new
            {
                NguoiLap = NguoiLap,
                Thang = month,
                Nam = year,
                NhanHang = "ANNAYAKE",
                BranchId = intBrandID

            }).ToList();
            ViewBag.Anna = Anna;
           // model.Add(Anna);
            //Nhãn leo 
            var Leo = SqlHelper.QuerySP<TongHopDealViewModel>("getDoanhSoTheoLevel", new
            {
                NguoiLap = NguoiLap,
                Thang = month,
                Nam = year,
                NhanHang = "LEONOR GREYL",
                BranchId = intBrandID

            }).ToList();
            ViewBag.Leo = Leo;
           // model.Add(Leo);
            //nhãn orlane
            //Nhãn leo 
            var orlane = SqlHelper.QuerySP<TongHopDealViewModel>("getDoanhSoTheoLevel", new
            {
                NguoiLap = NguoiLap,
                Thang = month,
                Nam = year,
                NhanHang = "ORLANE PARIS,DICHVU,CONGNGHECAO",
                BranchId = intBrandID

            }).ToList();
            ViewBag.Orlane = orlane;

            var Level = Crm_LevelRepository.GetAllCrm_Level().Select(item => new Crm_LevelViewModel
            {
                Id = item.Id,
                Name = item.Name,
                StaredIndex = item.StaredIndex,
                EndIndex = item.EndIndex

            }).ToList();
            ViewBag.Level = Level;

            var thucte = SqlHelper.QuerySP<KHCDS_KHBH_ViewModel>("getThucteKHBH", new
            {
                @NguoiLap_Id = NguoiLap,
                @KHCDS = 0,
                @Month = month,
                @Year = year,
                @BranchId = intBrandID,
                @isKH = 1
            }).FirstOrDefault();
            ViewBag.ThucTeKHBH = thucte;

            // model.Add(orlane);
            return View();
        }
        #endregion

        #region theo dõi deal -1 va 100


        public ActionResult FollowDeal(int? month, int? year, int? ManagerStaffId)
        {
            if(ManagerStaffId == null)
            {
                ManagerStaffId = 0;
            }
            if (year == null && month == null)
            {
                year = int.Parse(DateTime.Now.Year.ToString());
                month = int.Parse(DateTime.Now.Month.ToString());
            }

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

            var listdate = new List<DateTime>();
            listdate = Enumerable.Range(1, DateTime.DaysInMonth((int)year, (int)month))  // Days: 1, 2 ... 31 etc.
                    .Select(day => new DateTime((int)year, (int)month, day)) // Map each day to a date
                    .ToList();
            int skipdate = 0;
            int getdate = 7;
            int n = 0;
            var model = new List<FollowDealViewModel>();
            var model2 = new List<FollowDealViewModel>();
            var model3 = new List<FollowDealViewModel>();
            var model4 = new List<FollowDealViewModel>();
            if (listdate.Count() > 0)
            {
                for (int i = 0; i < 4; i++)
                {
                    n++;
                    IEnumerable<DateTime> listdatee = null;
                    if (n < 4)
                    {
                        listdatee = listdate.Skip(skipdate).Take(getdate);
                    }
                    else
                    {
                        listdatee = listdate.Skip(skipdate);
                    }
                    skipdate = skipdate + 7;
                    var firstDate = listdatee.FirstOrDefault();
                    var lastDate = listdatee.Last();

                    //getFollowDeal
                   var item = SqlHelper.QuerySP<FollowDealViewModel>("getFollowDeal", new
                    {
                        branchId = intBrandID,
                        month = month,
                        year = year,
                        EndDate = lastDate,
                        NguoiLap = ManagerStaffId

                   }).ToList();

                    var orlane = item.Where(x => x.CountForBrand == "ORLANE PARIS" || x.CountForBrand == "CONGNGHECAO" || x.CountForBrand == "DICHVU").ToList();
                    var anna = item.Where(x => x.CountForBrand == "ANNAYAKE").FirstOrDefault();
                    var leo = item.Where(x => x.CountForBrand == "LEONOR GREYL").FirstOrDefault();

                    if (n == 1)
                    {
                         //Tính Orlane
                        var modelitem = new FollowDealViewModel();
                        modelitem.CountForBrand = "ORLANE PARIS";
                        modelitem.PhanTramDealTB =Math.Round( ((float)orlane.Sum(x => x.SLDeatbai) / (float)orlane.Sum(x => x.TongDeal)) * 100);
                        modelitem.PhanTramDealTC = Math.Round((((float)orlane.Sum(x => x.SLTC) / (float)orlane.Sum(x => x.TongDeal))) * 100);
                        modelitem.PhanTramDSTB = Math.Round((((float)orlane.Sum(x => x.DoanhSoBai) / (float)orlane.Sum(x => x.TongDS))) * 100);
                        modelitem.PhanTramDSTC = Math.Round((((float)orlane.Sum(x => x.DoanhSoTC) / (float)orlane.Sum(x => x.TongDS))) * 100);
                        model.Add(modelitem);
                        //Tính Anna
                        modelitem = new FollowDealViewModel();
                        modelitem.CountForBrand = "ANNAYAKE";
                        modelitem.PhanTramDealTB = Math.Round((((float)anna.SLDeatbai / (float)anna.TongDeal)) * 100);
                        modelitem.PhanTramDealTC = Math.Round((((float)anna.SLTC / (float)anna.TongDeal)) * 100);
                        modelitem.PhanTramDSTB = Math.Round((((float)anna.DoanhSoBai / (float)anna.TongDS)) * 100);
                        modelitem.PhanTramDSTC = Math.Round((((float)anna.DoanhSoTC / (float)anna.TongDS)) * 100);
                        model.Add(modelitem);
                        //Tính leo
                        modelitem = new FollowDealViewModel();
                        modelitem.CountForBrand = "LEONOR GREYL";
                        modelitem.PhanTramDealTB = Math.Round((((float)leo.SLDeatbai / (float)leo.TongDeal)) * 100);
                        modelitem.PhanTramDealTC = Math.Round((((float)leo.SLTC / (float)leo.TongDeal)) * 100);
                        modelitem.PhanTramDSTB = Math.Round((((float)leo.DoanhSoBai / (float)leo.TongDS)) * 100);
                        modelitem.PhanTramDSTC = Math.Round((((float)leo.DoanhSoTC / (float)leo.TongDS)) * 100);
                        model.Add(modelitem);
                    }
                    if(n == 2)
                    {
                        //Tính Orlane
                        var modelitem = new FollowDealViewModel();
                        modelitem.CountForBrand = "ORLANE PARIS";
                        modelitem.PhanTramDealTB = Math.Round(((float)orlane.Sum(x => x.SLDeatbai) / (float)orlane.Sum(x => x.TongDeal)) * 100);
                        modelitem.PhanTramDealTC = Math.Round((((float)orlane.Sum(x => x.SLTC) / (float)orlane.Sum(x => x.TongDeal))) * 100);
                        modelitem.PhanTramDSTB = Math.Round((((float)orlane.Sum(x => x.DoanhSoBai) / (float)orlane.Sum(x => x.TongDS))) * 100);
                        modelitem.PhanTramDSTC = Math.Round((((float)orlane.Sum(x => x.DoanhSoTC) / (float)orlane.Sum(x => x.TongDS))) * 100);
                        model2.Add(modelitem);
                        //Tính Anna
                        modelitem = new FollowDealViewModel();
                        modelitem.CountForBrand = "ANNAYAKE";
                        modelitem.PhanTramDealTB = Math.Round((((float)anna.SLDeatbai / (float)anna.TongDeal)) * 100);
                        modelitem.PhanTramDealTC = Math.Round((((float)anna.SLTC / (float)anna.TongDeal)) * 100);
                        modelitem.PhanTramDSTB = Math.Round((((float)anna.DoanhSoBai / (float)anna.TongDS)) * 100);
                        modelitem.PhanTramDSTC = Math.Round((((float)anna.DoanhSoTC / (float)anna.TongDS)) * 100);
                        model2.Add(modelitem);
                        //Tính leo
                        modelitem = new FollowDealViewModel();
                        modelitem.CountForBrand = "LEONOR GREYL";
                        modelitem.PhanTramDealTB = Math.Round((((float)leo.SLDeatbai / (float)leo.TongDeal)) * 100);
                        modelitem.PhanTramDealTC = Math.Round((((float)leo.SLTC / (float)leo.TongDeal)) * 100);
                        modelitem.PhanTramDSTB = Math.Round((((float)leo.DoanhSoBai / (float)leo.TongDS)) * 100);
                        modelitem.PhanTramDSTC = Math.Round((((float)leo.DoanhSoTC / (float)leo.TongDS)) * 100);
                        model2.Add(modelitem);
                    }
                    if(n == 3)
                    {
                        //Tính Orlane
                        var modelitem = new FollowDealViewModel();
                        modelitem.CountForBrand = "ORLANE PARIS";
                        modelitem.PhanTramDealTB = Math.Round(((float)orlane.Sum(x => x.SLDeatbai) / (float)orlane.Sum(x => x.TongDeal)) * 100);
                        modelitem.PhanTramDealTC = Math.Round((((float)orlane.Sum(x => x.SLTC) / (float)orlane.Sum(x => x.TongDeal))) * 100);
                        modelitem.PhanTramDSTB = Math.Round((((float)orlane.Sum(x => x.DoanhSoBai) / (float)orlane.Sum(x => x.TongDS))) * 100);
                        modelitem.PhanTramDSTC = Math.Round((((float)orlane.Sum(x => x.DoanhSoTC) / (float)orlane.Sum(x => x.TongDS))) * 100);
                        model3.Add(modelitem);
                        //Tính Anna
                        modelitem = new FollowDealViewModel();
                        modelitem.CountForBrand = "ANNAYAKE";
                        modelitem.PhanTramDealTB = Math.Round((((float)anna.SLDeatbai / (float)anna.TongDeal)) * 100);
                        modelitem.PhanTramDealTC = Math.Round((((float)anna.SLTC / (float)anna.TongDeal)) * 100);
                        modelitem.PhanTramDSTB = Math.Round((((float)anna.DoanhSoBai / (float)anna.TongDS)) * 100);
                        modelitem.PhanTramDSTC = Math.Round((((float)anna.DoanhSoTC / (float)anna.TongDS)) * 100);
                        model3.Add(modelitem);
                        //Tính leo
                        modelitem = new FollowDealViewModel();
                        modelitem.CountForBrand = "LEONOR GREYL";
                        modelitem.PhanTramDealTB = Math.Round((((float)leo.SLDeatbai / (float)leo.TongDeal)) * 100);
                        modelitem.PhanTramDealTC = Math.Round((((float)leo.SLTC / (float)leo.TongDeal)) * 100);
                        modelitem.PhanTramDSTB = Math.Round((((float)leo.DoanhSoBai / (float)leo.TongDS)) * 100);
                        modelitem.PhanTramDSTC = Math.Round((((float)leo.DoanhSoTC / (float)leo.TongDS)) * 100);
                        model3.Add(modelitem);
                    }
                    if(n == 4)
                    {
                        //Tính Orlane
                        var modelitem = new FollowDealViewModel();
                        modelitem.CountForBrand = "ORLANE PARIS";
                        modelitem.PhanTramDealTB = Math.Round(((float)orlane.Sum(x => x.SLDeatbai) / (float)orlane.Sum(x => x.TongDeal)) * 100);
                        modelitem.PhanTramDealTC = Math.Round((((float)orlane.Sum(x => x.SLTC) / (float)orlane.Sum(x => x.TongDeal))) * 100);
                        modelitem.PhanTramDSTB = Math.Round((((float)orlane.Sum(x => x.DoanhSoBai) / (float)orlane.Sum(x => x.TongDS))) * 100);
                        modelitem.PhanTramDSTC = Math.Round((((float)orlane.Sum(x => x.DoanhSoTC) / (float)orlane.Sum(x => x.TongDS))) * 100);
                        model4.Add(modelitem);
                        //Tính Anna
                        modelitem = new FollowDealViewModel();
                        modelitem.CountForBrand = "ANNAYAKE";
                        modelitem.PhanTramDealTB = Math.Round((((float)anna.SLDeatbai / (float)anna.TongDeal)) * 100);
                        modelitem.PhanTramDealTC = Math.Round((((float)anna.SLTC / (float)anna.TongDeal)) * 100);
                        modelitem.PhanTramDSTB = Math.Round((((float)anna.DoanhSoBai / (float)anna.TongDS)) * 100);
                        modelitem.PhanTramDSTC = Math.Round((((float)anna.DoanhSoTC / (float)anna.TongDS)) * 100);
                        model4.Add(modelitem);
                        //Tính leo
                        modelitem = new FollowDealViewModel();
                        modelitem.CountForBrand = "LEONOR GREYL";
                        modelitem.PhanTramDealTB = Math.Round((((float)leo.SLDeatbai / (float)leo.TongDeal)) * 100);
                        modelitem.PhanTramDealTC = Math.Round((((float)leo.SLTC / (float)leo.TongDeal)) * 100);
                        modelitem.PhanTramDSTB = Math.Round((((float)leo.DoanhSoBai / (float)leo.TongDS)) * 100);
                        modelitem.PhanTramDSTC = Math.Round((((float)leo.DoanhSoTC / (float)leo.TongDS)) * 100);
                        model4.Add(modelitem);
                    }

                }
            }

            ViewBag.model = model;
            ViewBag.model2 = model2;
            ViewBag.model3 = model3;
            ViewBag.model4 = model4;
            return View();
        }


        #endregion

        #region Duyệt deal bổ sung
       
        public ActionResult ApproveDeal(string txtCode,int? month, int? year, int? NguoiLap, string CountForBrand, int? Approve)
        {
            Approve = Approve == null ? 0 : Approve;
            month = month != null ? month : DateTime.Now.Month;
            year = year != null ? year : DateTime.Now.Year;
            ViewBag.Approve = Approve;

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

            var user = userRepository.GetAllUsers().Select(item => new UserViewModel
            {
                Id = item.Id,
                UserName = item.UserName,
                FullName = item.FullName,
                Status = item.Status,
                BranchId = item.BranchId,
                UserTypeId = item.UserTypeId
            }).Where(x => x.Status == UserStatus.Active && x.UserTypeId == 2026).ToList();
            //if (branchId > 0)
            //{
            //    ViewBag.user2 = user.Where(x => x.BranchId == branchId).ToList();
            //}

            //&& listNhanvien.Contains(x.Id)
            var user2branch = user.Where(x => x.Id == WebSecurity.CurrentUserId).FirstOrDefault();

            var model = CRM_KH_BANHANGRepository.GetAllvwCRM_KH_BANHANG_CTIET()
               .Where(x => x.NAM == year && x.THANG == month);

            if (intBrandID != null && intBrandID > 0)
            {
                model = model.Where(x => x.BranchId == intBrandID);
                user = user.Where(x => x.BranchId == intBrandID).ToList();
                if (user2branch != null && user2branch.BranchId == 0)
                {
                    user.Add(user2branch);
                }
            }
            ViewBag.user = user;
            
            if (Approve == 1)
            {
                model = model.Where(x => x.NAM == year && x.THANG == month && x.IsApprove ==1);
            }
            else if(Approve == 2)
            {
                model = model.Where(x => x.NAM == year && x.THANG == month && x.IsApprove == 2);
            }
            else if (Approve == -1)
            {
                model = model.Where(x => x.NAM == year && x.THANG == month && x.IsApprove == -1);
            }
            else
            {
                model = model.Where(x => x.NAM == year && x.THANG == month && x.IsApprove != 1 && x.IsApprove != 2 && x.IsApprove != -1);
            }

                var CRM_KH_BANHANGList = model
               .Select(x => new CRM_KH_BANHANG_CTIETViewModel
               {
                   KH_BANHANG_ID = x.KH_BANHANG_ID,
                   KH_BANHANG_CTIET_ID = x.KH_BANHANG_CTIET_ID,
                   Phone = x.Phone,
                   KHACHHANG_ID = x.KHACHHANG_ID,
                   CompanyName = x.CompanyName,
                   Code = x.Code,
                   TYLE_THANHCONG = x.TYLE_THANHCONG,
                   TYLE_THANHCONG_REVIEW = x.TYLE_THANHCONG_REVIEW,
                   GHI_CHU = x.GHI_CHU,
                   IsDeleted = x.IsDeleted,
                   THANG = x.THANG,
                   NAM = x.NAM,
                   CRM_Sale_ProductInvoiceCode = x.CRM_Sale_ProductInvoiceCode,
                   CountForBrand = x.CountForBrand,
                   TotalAmount = x.TotalAmount,
                   ProductInvoiceId = x.ProductInvoiceId,
                   NGUOILAP_ID = x.NGUOILAP_ID,
                   BranchId = x.BranchId,
                   NOIDUNG = x.NOIDUNG,
                   IsApprove = x.IsApprove,
                   UserApprove = x.UserApprove,
                   UserApproveName = x.UserApproveName
               }).ToList();
            if(NguoiLap> 0)
            {
                CRM_KH_BANHANGList = CRM_KH_BANHANGList.Where(x => x.NGUOILAP_ID == NguoiLap).ToList();
            }
            if (!String.IsNullOrEmpty(CountForBrand))
            {
                CRM_KH_BANHANGList = CRM_KH_BANHANGList.Where(x => x.CountForBrand == CountForBrand).ToList();
            }
            if (!String.IsNullOrEmpty(txtCode))
            {
                txtCode = Helpers.Common.ChuyenThanhKhongDau(txtCode);
                CRM_KH_BANHANGList = CRM_KH_BANHANGList.Where(x => Helpers.Common.ChuyenThanhKhongDau(x.Code) == txtCode || Helpers.Common.ChuyenThanhKhongDau(x.CompanyName) == txtCode).ToList();
            }

            var origin = categoryRepository.GetAllCategories().Select(item => new CategoryViewModel
            {
                OrderNo = item.OrderNo,
                Name = item.Name,
                Value = item.Value,
                Code = item.Code
            }).Where(item => item.Code == "Origin").ToList();
            ViewBag.Origin = origin;
            ViewBag.SuccessMessage = TempData["SuccessMessage"];
            ViewBag.FailedMessage = TempData["FailedMessage"];
            return View(CRM_KH_BANHANGList);
        }

        [HttpPost]
        public ActionResult ApproveDeal()
        {
           
            string idDeleteAll = Request["DeleteId-checkbox"];
            if (idDeleteAll != null)
            {
                string[] arrDeleteId = idDeleteAll.Split(',');
                for (int i = 0; i < arrDeleteId.Count(); i++)
                {
                    var item = CRM_KH_BANHANGRepository.GetCRM_KH_BANHANG_CTById(int.Parse(arrDeleteId[i], CultureInfo.InvariantCulture));
                    if (item != null)
                    {
                       
                        if (item.IsApprove == 1)
                        {
                            item.IsApprove = 2;
                        }
                        else if(item.IsApprove != 1 && item.IsApprove != 2)
                        {
                            item.UserApprove = WebSecurity.CurrentUserId;
                            item.IsApprove = 1;
                        }
                        CRM_KH_BANHANGRepository.UpdateCRM_KH_BANHANG_CT(item);
                    }
                }
            }
            TempData["SuccessMessage"] = "Xét duyệt thành công !";
            return RedirectToAction("ApproveDeal");
        }

        [HttpPost]
        public ActionResult AbortDeal(int KHID, string Note)
        {
            var item = CRM_KH_BANHANGRepository.GetCRM_KH_BANHANG_CTById(KHID);
            if (item != null)
            {

               
                item.UserApprove = WebSecurity.CurrentUserId;
                item.IsApprove = -1;
                item.GHI_CHU = Note;
                CRM_KH_BANHANGRepository.UpdateCRM_KH_BANHANG_CT(item);
            }
            TempData["SuccessMessage"] = "Xét duyệt thành công !";
            return Json(1);//RedirectToAction("ApproveDeal");
        }
        #endregion

        #region SL DEAL THEO TARGET DỰA VÀO SỐ DEAL TỪ L
        public ActionResult TongHopDealNhanVienTheoNhan (int? month, int? year, int? NguoiLap, string CountForBrand)
        {
            month = month != null ? month : DateTime.Now.Month;
            year = year != null ? year : DateTime.Now.Year;
            CountForBrand = CountForBrand != null ? CountForBrand : "ANNAYAKE";
            //get cookie brachID 
            HttpRequestBase request = this.HttpContext.Request;
            var u = userRepository.GetUserById(WebSecurity.CurrentUserId);
            string strBrandID = "0";
            //ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
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
            var user = userRepository.GetAllUsers().Select(item => new UserViewModel
            {
                Id = item.Id,
                UserName = item.UserName,
                FullName = item.FullName,
                Status = item.Status,
                BranchId = item.BranchId,
                UserTypeId = item.UserTypeId
            }).Where(x => x.Status == UserStatus.Active && x.UserTypeId == 2026).ToList();
            if (intBrandID > 0)
            {
                user = user.Where(x => x.BranchId == intBrandID).ToList();
            }
            if (u.UserTypeId == 2026)
            {
                user = user.Where(x => x.Id == u.Id).ToList();
            }
            NguoiLap = NguoiLap != null ? NguoiLap : 0;
            ViewBag.NguoiLap = NguoiLap;
            ViewBag.user = user;


            //Lấy P và L
            var Target = Crm_TargetRepository.GetAllCrm_Target().ToList();
            var Period = Crm_PeriodRepository.GetAllCrm_Period().Select(x => new Crm_PeriodViewModel
            {
                Id = x.Id,
                Name = x.Name,
                starDate = x.starDate,
                endDate = x.endDate

            }).ToList();
            ViewBag.Period = Period;

            var Level = Crm_LevelRepository.GetAllCrm_Level().Select(item => new Crm_LevelViewModel
            {
                Id = item.Id,
                Name = item.Name,
                StaredIndex = item.StaredIndex,
                EndIndex = item.EndIndex

            }).ToList();
            ViewBag.Level = Level;

            //DS theo level
            var Anna = SqlHelper.QuerySP<TongHopDealViewModel>("getDoanhSoTheoLevel", new
            {
                NguoiLap = NguoiLap,
                Thang = month,
                Nam = year,
                NhanHang = CountForBrand,
                BranchId = intBrandID

            }).ToList();

            //lấy khbh- khcds
            var listBrand = new List<string>();
            listBrand = CountForBrand.Split(',').ToList();
            var KHBHCT = CRM_KH_BANHANGRepository.GetAllvwCRM_KH_BANHANG_CTIET()
                    .Where(x => (x.BranchId == intBrandID || intBrandID == 0) && listBrand.Contains(x.CountForBrand) &&( x.NAM == year) && (x.THANG == month) && (x.NGUOILAP_ID == NguoiLap || NguoiLap ==0)).ToList();

            var KHCDSCT = CRM_BH_DOANHSORepository.GetAllvwCRM_BH_DOANHSO_CT()
                   .Where(x => (x.BranchId == intBrandID || intBrandID == 0) && listBrand.Contains(x.CountForBrand) && (x.NAM == year) && (x.THANG == month) && (x.NGUOILAP_ID == NguoiLap || NguoiLap == 0)).ToList();
            //tổng deal
            var Total = KHBHCT.Where(x => x.TYLE_THANHCONG > 1 && x.TYLE_THANHCONG < 100).Count() + KHCDSCT.Where(x => x.TYLE_THANHCONG > 1 && x.TYLE_THANHCONG < 100).Count();
            //Khai báo các List
            var targetondealLevel = new List<TongHopDealNhanVienTheoNhanViewModel>();
            var targetonDeal = new List<TongHopDealNhanVienTheoNhanViewModel>();
            var chenhlechDeal = new List<TongHopDealNhanVienTheoNhanViewModel>();
            var revenueonTarget = new List<TongHopDealNhanVienTheoNhanViewModel>();
            //
            foreach (var itemLevel in Level)
            {
                if (itemLevel.Name != "L1" && itemLevel.Name != "L6")
                {
                    foreach (var itemPeriod in Period)
                    {
                        var itemTarget = Target.Where(x => x.IdLevel == itemLevel.Id && x.IdPeriod == itemPeriod.Id).FirstOrDefault();
                        var itemKHBHCT = KHBHCT.Where(x => x.TYLE_THANHCONG >= itemLevel.StaredIndex && x.TYLE_THANHCONG <= itemLevel.EndIndex).ToList().Count();
                        var itemKHCDSCT = KHCDSCT.Where(x => x.TYLE_THANHCONG >= itemLevel.StaredIndex && x.TYLE_THANHCONG <= itemLevel.EndIndex).ToList().Count();

                        var deal1 = new TongHopDealNhanVienTheoNhanViewModel();
                        deal1.Level = itemLevel.Id;
                        deal1.Period = itemPeriod.Id;
                        //SL DEAL THEO TARGET DỰA VÀO SỐ DEAL TỪ L
                        var xxx =Math.Round((decimal)((itemKHBHCT + itemKHCDSCT) * itemTarget.Target / 100));
                        deal1.Target = xxx;
                        //var deal1 = new TongHopDealNhanVienTheoNhanViewModel();
                        //deal1 = deal;
                        targetondealLevel.Add(deal1);

                        //SL DEAL THEO TARGET TRÊN TỔNG DEAL	
                        var deal2 = new TongHopDealNhanVienTheoNhanViewModel();
                        deal2.Level = itemLevel.Id;
                        deal2.Period = itemPeriod.Id;
                        var yyy =Math.Round((decimal)(Total * itemTarget.Target / 100));
                        deal2.Target = yyy;
                        //var deal2 = new TongHopDealNhanVienTheoNhanViewModel();
                        //deal2 = deal;
                        targetonDeal.Add(deal2);

                        //CHÊNH LỆCH SL  DEAL SO TARGET	
                        var deal3 = new TongHopDealNhanVienTheoNhanViewModel();
                        deal3.Level = itemLevel.Id;
                        deal3.Period = itemPeriod.Id;
                        var zzz = (itemKHBHCT + itemKHCDSCT) - yyy;
                        deal3.Target = zzz;
                        //var deal3 = new TongHopDealNhanVienTheoNhanViewModel();deal3 = deal;
                        chenhlechDeal.Add(deal3);

                        //DS ĐẠT THEO TARGET % KH	
                        var deal4 = new TongHopDealNhanVienTheoNhanViewModel();
                        deal4.Level = itemLevel.Id;
                        deal4.Period = itemPeriod.Id;
                        var www = Anna.Where(x => x.Name == itemLevel.Name).FirstOrDefault();
                        var vvv = (www.Total * itemTarget.Target) / 100;
                        deal4.Target = vvv;
                        //var deal4 = new TongHopDealNhanVienTheoNhanViewModel();deal4 = deal;
                        revenueonTarget.Add(deal4);
                    }
                }
            }
            ViewBag.targetonDeal = targetonDeal;
            ViewBag.chenhlechDeal = chenhlechDeal;
            ViewBag.revenueonTarget = revenueonTarget;
            return View(targetondealLevel);
        }

        //public class 
        #endregion


        #region Cập nhật tay khách hẹn từ bitrix
        public ActionResult CreateKhachBitrix(string startDate, string endDate, int? NguoiLap)
        {
            //get cookie brachID 
            HttpRequestBase request = this.HttpContext.Request;
            var u = userRepository.GetUserById(WebSecurity.CurrentUserId);
            string strBrandID = "0";
            //ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
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
            var user = userRepository.GetAllUsers().Select(item => new UserViewModel
            {
                Id = item.Id,
                UserName = item.UserName,
                FullName = item.FullName,
                Status = item.Status,
                BranchId = item.BranchId,
                UserTypeId = item.UserTypeId
            }).Where(x => x.Status == UserStatus.Active && x.UserTypeId == 2026).ToList();
            if (intBrandID > 0)
            {
                user = user.Where(x => x.BranchId == intBrandID).ToList();
            }
            if (u.UserTypeId == 2026)
            {
                user = user.Where(x => x.Id == u.Id).ToList();
            }
            NguoiLap = NguoiLap != null ? NguoiLap : 0;
            ViewBag.NguoiLap = NguoiLap;
            
           

            if (startDate == null && endDate == null)
            {
                DateTime aDateTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                // Cộng thêm 1 tháng và trừ đi một ngày.
                DateTime retDateTime = aDateTime.AddMonths(1).AddDays(-1);
                startDate = aDateTime.ToString("dd/MM/yyyy");
                endDate = retDateTime.ToString("dd/MM/yyyy");
            }
            var model = new List<Crm_KhachHenBitrixModel>();

            //Lọc theo ngày
            DateTime d_startDate, d_endDate;
            if (DateTime.TryParseExact(startDate, "dd/MM/yyyy", new CultureInfo("vi-VN"), DateTimeStyles.None, out d_startDate))
            {
                if (DateTime.TryParseExact(endDate, "dd/MM/yyyy", new CultureInfo("vi-VN"), DateTimeStyles.None, out d_endDate))
                {
                    d_endDate = d_endDate.AddHours(23).AddMinutes(59);
                    ViewBag.retDateTime = d_endDate;
                    ViewBag.aDateTime = d_startDate;
                    model = SqlHelper.QuerySP<Crm_KhachHenBitrixModel>("getSoluongKhachBitrix", new
                    {
                        StartDate = d_startDate,
                        EndDate = d_endDate
                       
                    }).ToList();
                }
            }
            if (NguoiLap > 0)
            {
                model = model.Where(x => x.UserId == NguoiLap).ToList();
                user = user.Where(x => x.Id == NguoiLap).ToList();
            }
            ViewBag.user = user;


            return View(model);
        }

        [AllowAnonymous]
        [HttpPost]
        public JsonResult UpdateBitrix(int? user, string date, int? amount)
        {

            DateTime d_startDate;
            if (DateTime.TryParseExact(date, "dd/MM/yyyy", new CultureInfo("vi-VN"), DateTimeStyles.None, out d_startDate))
            {

            }
            //Kiểm tra ngày đó có du lieu hay chua
            var check = Crm_KhachHenBitrixRepository.GetAllCrm_KhachHenBitrixByUser(user.Value).Where(x => x.Date == d_startDate).FirstOrDefault();
            if(check != null)
            {
                check.Amount = amount;
                check.ModifiedUserId = WebSecurity.CurrentUserId;
                check.ModifiedDate = DateTime.Now;
                Crm_KhachHenBitrixRepository.UpdateCrm_KhachHenBitrix(check);
            }
            else
            {
                var nBitrix = new Crm_KhachHenBitrix();
                nBitrix.CreatedDate = DateTime.Now;
                nBitrix.CreatedUserId = WebSecurity.CurrentUserId;
                nBitrix.UserId = user;
                nBitrix.Date = d_startDate;
                nBitrix.Amount = amount;
                Crm_KhachHenBitrixRepository.InsertCrm_KhachHenBitrix(nBitrix);
            }
            return Json(0);
        }
        #endregion
    }
}
