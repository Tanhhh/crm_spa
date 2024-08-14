using Erp.BackOffice.Filters;
using Erp.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using WebMatrix.WebData;
using Erp.Domain.Crm.Interfaces;
using Erp.BackOffice.Areas.Cms.Models;
using Erp.Domain.Crm.Entities;
using System.Globalization;
using Erp.BackOffice.Sale.Models;
using Erp.Domain.Entities;
using Erp.Domain.Sale.Interfaces;
using System.Data.Entity.Infrastructure;
using System.Text.RegularExpressions;
using Erp.Domain.Account.Interfaces;
using Erp.Utilities;
using Erp.BackOffice.Areas.Administration.Models;
using Erp.BackOffice.Account.Models;
using PagedList;
using Erp.Domain.Crm.Helper;
using System.IO;
using System.Web;
using iTextSharp.text.pdf.qrcode;
using Erp.BackOffice.Helpers;
using ClosedXML.Excel;
using System.Data;
using Erp.Domain.Staff.Entities;
using Erp.Domain.Staff.Interfaces;
using Erp.BackOffice.Staff.Models;
using Newtonsoft.Json;
using System.Collections;
//sdfghuio
namespace Erp.BackOffice.Sale.Controllers
{
    [Authorize]
    [InitializeSimpleMembership]
    [Erp.BackOffice.Helpers.NoCacheHelper]
    public class PlanController : Controller
    {
        private readonly IPlanRepository PlanRepository;
        private readonly IKH_TUONGTACRepository KH_TUONGTACRepository;
        private readonly ICategoryRepository categoryRepository;
        private readonly IUserRepository userRepository;
        private readonly ICustomerRepository customerRepository;
        private readonly IProductInvoiceRepository productInvoiceRepository;
        private readonly IInquiryCardRepository InquiryCardRepository;
        private readonly IInquiryCardDetailRepository inquiryCardDetailRepository;
        private readonly ITemplatePrintRepository templatePrintRepository;
        private readonly IBranchRepository BranchRepository;

        public PlanController(
            IPlanRepository _Plan,
            IKH_TUONGTACRepository _kH_TUONGTACRepository,
            ICategoryRepository _category,
            IUserRepository _user,
            ICustomerRepository _customer,
            IProductInvoiceRepository _productInvoice,
            IInquiryCardRepository _InquiryCard,
            IInquiryCardDetailRepository inquiryCard,
            ITemplatePrintRepository _templatePrint,
            IBranchRepository _BranchRepository
        )
        {
            PlanRepository = _Plan;
            KH_TUONGTACRepository = _kH_TUONGTACRepository;
            categoryRepository = _category;
            userRepository = _user;
            customerRepository = _customer;
            productInvoiceRepository = _productInvoice;
            InquiryCardRepository = _InquiryCard;
            inquiryCardDetailRepository = inquiryCard;
            templatePrintRepository = _templatePrint;
            BranchRepository = _BranchRepository;
        }
        public IEnumerable<KH_TUONGTACViewModel> GETKH_TUONGTAC()
        {

            IEnumerable<KH_TUONGTACViewModel> q = KH_TUONGTACRepository.GetAllvwKH_TUONGTAC()
                  .Select(item => new KH_TUONGTACViewModel
                  {
                      KH_TUONGTAC_ID = item.KH_TUONGTAC_ID,
                      CustomerCode = item.CustomerCode,
                      CustomerName = item.CustomerName,
                      Phone = item.Phone,
                      NGAYLAP = item.NGAYLAP,
                      //THANG = item.THANG.Value,
                      //NAM = item.NAM.Value,
                      HINHTHUC_TUONGTAC = item.HINHTHUC_TUONGTAC,
                      LOAI_TUONGTAC = item.LOAI_TUONGTAC,
                      PHANLOAI_TUONGTAC = item.PHANLOAI_TUONGTAC,
                      TINHTRANG_TUONGTAC = item.TINHTRANG_TUONGTAC,
                      MUCDO_TUONGTAC = item.MUCDO_TUONGTAC,
                      GHI_CHU = item.GHI_CHU,
                      GIAIPHAP_TUONGTAC = item.GIAIPHAP_TUONGTAC,
                      NGAYTUONGTAC_TIEP = item.NGAYTUONGTAC_TIEP,
                      GIOTUONGTAC_TIEP = item.GIOTUONGTAC_TIEP,
                      MUCCANHBAO_TUONGTAC = item.MUCCANHBAO_TUONGTAC,
                      HINH_ANH = item.HINH_ANH,
                      BranchId = item.BranchId,
                      FullName = item.FullName,
                      ModifiedDate = item.ModifiedDate,
                      CreatedDate = item.CreatedDate,
                      //LICHTUONGTATIEP = item.NGAYTUONGTAC_TIEP + "/" + item.THANG.Value + "/" + item.NAM.Value + " " + item.GIOTUONGTAC_TIEP,
                      GIO_TUONGTAC = item.GIO_TUONGTAC,
                      NGUOILAP_ID = item.NGUOILAP_ID,
                      KHACHHANG_ID = item.KHACHHANG_ID,
                      KETQUA_SAUTUONGTAC = item.KETQUA_SAUTUONGTAC,
                      Ngay_Lap = item.EDATE
                  }).OrderByDescending(x => x.Ngay_Lap);

            return q;
        }
        public ViewResult PlanUseSkinCare(string CustomerCodeOrName, string ProductName, int? BranchId, string StartDate, string EndDate, string THANHTOAN, int? PhieuConTu, int? PhieuConDen, int? ManagerStaffId, int? page)
        {
            // 2 dòng tìm theo tên và mã
            // string CustomerName,
            // string CustomerCode
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
            IEnumerable<vwPlanuseSkinCareViewModel> q = null;

            q = PlanRepository.GetvwAllvwPlanuseSkinCare()
                 .Select(item => new vwPlanuseSkinCareViewModel
                 {
                     BranchId = item.BranchId,
                     CustomerCode = item.CustomerCode,
                     CustomerName = item.CustomerName,
                     Phone = item.Phone,
                     ProductInvoiceCode = item.ProductInvoiceCode,
                     ProductName = item.ProductName,
                     SOLUONG = item.SOLUONG,
                     soluongdung = item.soluongdung,
                     soluongtra = item.soluongtra,
                     soluongchuyen = item.soluongchuyen,
                     soluongconlai = item.soluongconlai,
                     Type = item.Type,
                     ModifiedDate = item.ModifiedDate,
                     GladLevel = item.GladLevel,
                     TargetId = item.TargetId,
                     CreatedDate = item.CreatedDate,
                     ProductInvoiceId = item.ProductInvoiceId,
                     THANGTOANHET = item.THANGTOANHET,
                     CustomerId = item.CustomerId,
                     ManagerStaffId = item.ManagerStaffId,
                     ManagerName = item.ManagerName
                 }).OrderByDescending(x => x.ModifiedDate).ToList();

            if (BranchId != null && BranchId > 0)
            {
                q = q.Where(x => x.BranchId == BranchId);
            }

            if (!string.IsNullOrEmpty(CustomerCodeOrName))
            {
                q = q.Where(x => Helpers.Common.ChuyenThanhKhongDau(x.CustomerCode).Contains(Helpers.Common.ChuyenThanhKhongDau(CustomerCodeOrName)) || x.CustomerName.Contains(CustomerCodeOrName));
            }

            if (!string.IsNullOrEmpty(ProductName))
            {
                q = q.Where(x => Helpers.Common.ChuyenThanhKhongDau(x.ProductName).Contains(Helpers.Common.ChuyenThanhKhongDau(ProductName)));
            }

            if (ManagerStaffId != null && ManagerStaffId > 0)
            {

                q = q.Where(x => x.ManagerStaffId == ManagerStaffId).ToList();
            }

            if (PhieuConTu != null)
            {
                if (PhieuConDen != null)
                {
                    q = q.Where(x => x.soluongconlai >= PhieuConTu && x.soluongconlai <= PhieuConDen);
                }
            }

            if (!string.IsNullOrEmpty(THANHTOAN))
            {
                THANHTOAN = Helpers.Common.ChuyenThanhKhongDau(THANHTOAN);
                if (THANHTOAN == "het")
                {
                    //THANHTOAN = null;
                    q = q.Where(x => Helpers.Common.ChuyenThanhKhongDau(x.THANGTOANHET).Contains(THANHTOAN)).ToList();
                }
                else
                {
                    q = q.Where(x => x.THANGTOANHET == null);
                }
            }
            //Lọc theo ngày

            if (!string.IsNullOrEmpty(StartDate))
            {
                var startDate = DateTime.ParseExact(StartDate, "dd/MM/yyyy", null).ToString("yyyy-MM-dd HH:mm:ss");
                DateTime d_startDate = Convert.ToDateTime(startDate);
                if (!string.IsNullOrEmpty(EndDate))
                {
                    var endDate = DateTime.ParseExact(EndDate, "dd/MM/yyyy", null).ToString("yyyy-MM-dd HH:mm:ss");
                    DateTime d_endDate = Convert.ToDateTime(endDate);
                    d_endDate = d_endDate.AddHours(23).AddMinutes(59);
                    q = q.Where(x => x.ModifiedDate >= d_startDate && x.ModifiedDate <= d_endDate);

                }
            }
            ///
            if (StartDate == null && EndDate == null)
            {
                DateTime aDateTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                // Cộng thêm 1 tháng và trừ đi một ngày.
                DateTime retDateTime = aDateTime.AddMonths(1).AddDays(-1);
                StartDate = aDateTime.ToString("dd/MM/yyyy");
                EndDate = retDateTime.ToString("dd/MM/yyyy");

                DateTime d_startDate1, d_endDate1;
                if (DateTime.TryParseExact(StartDate, "dd/MM/yyyy", new CultureInfo("vi-VN"), DateTimeStyles.None, out d_startDate1))
                {
                    if (DateTime.TryParseExact(EndDate, "dd/MM/yyyy", new CultureInfo("vi-VN"), DateTimeStyles.None, out d_endDate1))
                    {
                        d_endDate1 = d_endDate1.AddHours(23).AddMinutes(59);
                        //ViewBag.retDateTime = d_endDate;
                        //ViewBag.aDateTime = d_startDate;
                        q = q.Where(x => x.ModifiedDate >= d_startDate1 && x.ModifiedDate <= d_endDate1);
                    }
                }
            }

            IEnumerable<KH_TUONGTACViewModel> data = KH_TUONGTACRepository.GetAllvwKH_TUONGTAC()
                  .Select(item => new KH_TUONGTACViewModel
                  {

                      NGUOILAP_ID = item.NGUOILAP_ID,
                      KHACHHANG_ID = item.KHACHHANG_ID

                  });

            int pageNumber = (page ?? 1);
            int pageSize = 10;
            ViewBag.PageNumber = pageNumber;
            ViewBag.PageSize = pageSize;
            ViewBag.LICHSUTUONGTAC = data;
            ViewBag.SuccessMessage = TempData["SuccessMessage"];
            ViewBag.FailedMessage = TempData["FailedMessage"];
            ViewBag.AlertMessage = TempData["AlertMessage"];
            List<int?> ints = new List<int?>();
            foreach(var item in q) { 
                ints.Add(null);
            }
            IEnumerable<vwPlanuseSkinCareViewModel> pw=q.Skip((pageNumber-1)*pageSize).Take(pageSize);
            TempData["Data_PlanUseSkinCare"] = q;
            ViewBag.LSPlan = pw;
            return View(ints.ToPagedList(pageNumber,pageSize));

        }

        #region Create
        public ActionResult Create(string NGAYLAP, int? NGUOILAP_ID)
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

            if (DateTime.Parse(NGAYLAP) < (DateTime.Now.Date))
            {
                TempData["Seccess"] = "Ngày lập phải lớn hơn ngày hiện tại";
                return RedirectToAction("Index");
            }
            var DUOITOC = categoryRepository.GetAllCategories()
                .Select(item => new CategoryViewModel
                {
                    Value = item.Value,
                    Code = item.Code,
                    Name = item.Name,
                }).Where(x => x.Code == "DUOITOC");
            ViewBag.DUOITOC = DUOITOC;
            var DODAITOC = categoryRepository.GetAllCategories()
                .Select(item => new CategoryViewModel
                {
                    Value = item.Value,
                    Code = item.Code,
                    Name = item.Name,
                }).Where(x => x.Code == "DODAITOC");
            ViewBag.DODAITOC = DODAITOC;
            var DADAU = categoryRepository.GetAllCategories()
                .Select(item => new CategoryViewModel
                {
                    Value = item.Value,
                    Code = item.Code,
                    Name = item.Name,
                }).Where(x => x.Code == "DADAU");
            ViewBag.DADAU = DADAU;
            var MAMTOC = categoryRepository.GetAllCategories()
                .Select(item => new CategoryViewModel
                {
                    Value = item.Value,
                    Code = item.Code,
                    Name = item.Name,
                }).Where(x => x.Code == "MAMTOC");
            ViewBag.MAMTOC = MAMTOC;
            var THANTOC = categoryRepository.GetAllCategories()
                 .Select(item => new CategoryViewModel
                 {
                     Value = item.Value,
                     Code = item.Code,
                     Name = item.Name,
                 }).Where(x => x.Code == "THANTOC");
            ViewBag.THANTOC = THANTOC;

            var CO = categoryRepository.GetAllCategories()
             .Select(item => new CategoryViewModel
             {
                 Value = item.Value,
                 Code = item.Code,
                 Name = item.Name,
             }).Where(x => x.Code == "CO");
            ViewBag.CO = CO;
            var MAT = categoryRepository.GetAllCategories()
             .Select(item => new CategoryViewModel
             {
                 Value = item.Value,
                 Code = item.Code,
                 Name = item.Name,
             }).Where(x => x.Code == "MAT");
            ViewBag.MAT = MAT;
            var BODY = categoryRepository.GetAllCategories()
             .Select(item => new CategoryViewModel
             {
                 Value = item.Value,
                 Code = item.Code,
                 Name = item.Name,
             }).Where(x => x.Code == "BODY");
            ViewBag.BODY = BODY;
            var DAMAT = categoryRepository.GetAllCategories()
             .Select(item => new CategoryViewModel
             {
                 Value = item.Value,
                 Code = item.Code,
                 Name = item.Name,
             }).Where(x => x.Code == "DAMAT");
            ViewBag.DAMAT = DAMAT;

            var data = GETKH_TUONGTAC();
            if (NGUOILAP_ID != null)
            {
                data.Where(x => x.NGUOILAP_ID == NGUOILAP_ID);
            }
            ViewBag.KH_TUONGTAC = data;

            var dataNhanvien = SqlHelper.QuerySP<ManagerStaff>("spgetNhanvienbacap", new
            {
                BranchId = (intBrandID == null ? 0 : intBrandID),
                UserId = NGUOILAP_ID,
            }).ToList();

            List<int> listNhanvien = new List<int>();


            for (int i = 0; i < dataNhanvien.Count; i++)
            {
                listNhanvien.Add(int.Parse(dataNhanvien[i].Id.ToString()));
            }





            var CustomerList = customerRepository.GetAllCustomer()
               .Select(item => new CustomerViewModel
               {
                   Code = item.Code,
                   Id = item.Id,
                   CompanyName = item.CompanyName,
                   Image = item.Image,
                   ManagerStaffId = item.ManagerStaffId,
                   isLock = item.isLock,
                   BranchId = item.BranchId
               }).Where(x => x.BranchId == intBrandID && x.isLock != true).ToList();

            //if (!Erp.BackOffice.Filters.SecurityFilter.IsAdmin())
            //{

            CustomerList = CustomerList.Where(x => (x.ManagerStaffId != null && listNhanvien.Contains(x.ManagerStaffId.Value))).ToList();
            // }


            if (CustomerList.Count == 0)
            {
                return Content("<script>alert('Nhân viên chưa có khách hàng để quản lý!')</script>");
            }







            //CustomerList= CustomerList.Where(x => x.isLock != true);


            ViewBag.NGUOI_LAP = NGUOILAP_ID;
            ViewBag.CustomerList = CustomerList;
            ViewBag.SuccessMessage = TempData["SuccessMessage"];
            ViewBag.FailedMessage = TempData["FailedMessage"];
            ViewBag.AlertMessage = TempData["AlertMessage"];
            return View();
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create(KH_TUONGTACViewModel model, bool? IsPopup)
        {
            var NGUOI_LAP = Request["NGUOILAP_ID"];
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
                if (ModelState.IsValid)
                {

                    var KH_TUONGTAC = new KH_TUONGTAC();
                    AutoMapper.Mapper.Map(model, KH_TUONGTAC);
                    KH_TUONGTAC.IsDeleted = false;
                    KH_TUONGTAC.CreatedUserId = WebSecurity.CurrentUserId;
                    KH_TUONGTAC.CreatedDate = DateTime.Now;
                    KH_TUONGTAC.ModifiedDate = DateTime.Now;
                    if (model.NGUOILAP_ID != null)
                    {
                        KH_TUONGTAC.NGUOILAP_ID = model.NGUOILAP_ID;
                    }
                    else if (NGUOI_LAP != null)
                    {
                        KH_TUONGTAC.NGUOILAP_ID = Convert.ToInt32(NGUOI_LAP);
                    }
                    else
                    {
                        KH_TUONGTAC.NGUOILAP_ID = WebSecurity.CurrentUserId;
                    }
                    KH_TUONGTAC.GIO_TUONGTAC = model.GIO_TUONGTAC;
                    KH_TUONGTAC.NGAYLAP = model.NGAYLAP;
                    //KH_TUONGTAC.THANG = int.Parse(model.NGAYTUONGTAC_TIEP.Substring(3, 2));
                    //KH_TUONGTAC.NAM = int.Parse(model.NGAYTUONGTAC_TIEP.Substring(6, 4));
                    KH_TUONGTAC.THANG = int.Parse(model.NGAYLAP.Substring(3, 2));//DateTime.Now.Month;
                    KH_TUONGTAC.NAM = int.Parse(model.NGAYLAP.Substring(6, 4));//DateTime.Now.Year;

                    if (KH_TUONGTAC.NGAYTUONGTAC_TIEP != null)
                    {
                        KH_TUONGTAC.NGAYTUONGTAC_TIEP = model.NGAYTUONGTAC_TIEP.Substring(0, 10);
                        KH_TUONGTAC.GIOTUONGTAC_TIEP = model.NGAYTUONGTAC_TIEP.Substring(10, 6);

                    }
                    else
                    {
                        KH_TUONGTAC.NGAYTUONGTAC_TIEP = null;
                        KH_TUONGTAC.GIOTUONGTAC_TIEP = null;
                    }
                    KH_TUONGTAC.BranchId = (intBrandID == null ? Convert.ToInt32(Session["GlobalCurentBranchId"]) : intBrandID);


                    KH_TUONGTACRepository.InsertKH_TUONGTAC(KH_TUONGTAC);

                    var path = System.Web.HttpContext.Current.Server.MapPath("~" + Helpers.Common.GetSetting("Plan"));
                    if (Request.Files["file-image"] != null)
                    {
                        var file = Request.Files["file-image"];
                        if (file.ContentLength > 0)
                        {
                            string image_name = KH_TUONGTAC.KH_TUONGTAC_ID + "." + file.FileName.Split('.').Last();
                            bool isExists = System.IO.Directory.Exists(path);
                            if (!isExists)
                                System.IO.Directory.CreateDirectory(path);
                            file.SaveAs(path + image_name);
                            KH_TUONGTAC.HINH_ANH = image_name;
                            KH_TUONGTACRepository.UpdateKH_TUONGTAC(KH_TUONGTAC);
                        }

                    }
                    //ghi log 
                    Erp.BackOffice.Controllers.HomeController.WriteLog(KH_TUONGTAC.KH_TUONGTAC_ID, "", "đã lập kế hoạch tương tác cho ngày " + KH_TUONGTAC.NGAYLAP, "", Helpers.Common.CurrentUser.BranchId.Value);
                    //Lập kế hoạch lun cho ngày tiếp theo
                    //if(KH_TUONGTAC.NGAYTUONGTAC_TIEP != null)
                    //{
                    //    var KH_TUONGTAC2 = new KH_TUONGTAC();
                    //    AutoMapper.Mapper.Map(model, KH_TUONGTAC2);
                    //    KH_TUONGTAC2.IsDeleted = false;
                    //    KH_TUONGTAC2.CreatedUserId = WebSecurity.CurrentUserId;
                    //    KH_TUONGTAC2.CreatedDate = DateTime.Now;
                    //    KH_TUONGTAC2.ModifiedDate = DateTime.Now;
                    //    if (model.NGUOILAP_ID != null)
                    //    {
                    //        KH_TUONGTAC2.NGUOILAP_ID = model.NGUOILAP_ID;
                    //    }
                    //    else if (NGUOI_LAP != null)
                    //    {
                    //        KH_TUONGTAC2.NGUOILAP_ID = Convert.ToInt32(NGUOI_LAP);
                    //    }
                    //    else
                    //    {
                    //        KH_TUONGTAC2.NGUOILAP_ID = WebSecurity.CurrentUserId;
                    //    }
                    //    KH_TUONGTAC2.GIO_TUONGTAC = null;
                    //    KH_TUONGTAC2.NGAYLAP = model.NGAYTUONGTAC_TIEP.Substring(0, 10);
                    //    KH_TUONGTAC2.KHACHHANG_ID = model.KHACHHANG_ID;
                    //    KH_TUONGTAC2.THANG = int.Parse(model.NGAYTUONGTAC_TIEP.Substring(3, 2));//DateTime.Now.Month;
                    //    KH_TUONGTAC2.NAM = int.Parse(model.NGAYTUONGTAC_TIEP.Substring(6, 4));
                    //    KH_TUONGTAC2.BranchId = (intBrandID == null ? Convert.ToInt32(Session["GlobalCurentBranchId"]) : intBrandID);
                    //    KH_TUONGTAC2.NGAYTUONGTAC_TIEP = null;
                    //    KH_TUONGTAC2.GIOTUONGTAC_TIEP = null;
                    //    KH_TUONGTAC2.GHI_CHU = model.GHI_CHU + " (Lấy trong lịch tương tác tiếp theo)";
                    //    KH_TUONGTACRepository.InsertKH_TUONGTAC(KH_TUONGTAC2);
                    //}
                    //
                    if (IsPopup == true)
                    {
                        TempData[Globals.SuccessMessageKey] = App_GlobalResources.Wording.InsertSuccess;

                        return RedirectToAction("_ClosePopup", "Home", new { area = "", FunctionCallback = "ClosePopupAndReloadPage" });
                    }
                    else
                    {
                        TempData[Globals.SuccessMessageKey] = App_GlobalResources.Wording.InsertSuccess;
                        return RedirectToAction("/Index");
                    }
                    //TempData[Globals.SuccessMessageKey] = App_GlobalResources.Wording.InsertSuccess;
                    //return RedirectToAction("Index");

                }
            }
            catch (Exception ex)
            {
            }
            return RedirectToAction("/Create");
        }
        #endregion

        #region Delete
        [HttpPost]
        public ActionResult Delete(int? NGUOILAP_ID, string NGAYLAP, string FullName)
        {
            string date = Request["NGAYKH_hidden"];
            string name = Request["FullName_hidden"];
            string name_id = Request["NGUOILAP_ID_hidden"];
            try
            {
                string idDeleteAll = Request["DeleteId-checkbox"];
                string[] arrDeleteId = idDeleteAll.Split(',');
                for (int i = 0; i < arrDeleteId.Count(); i++)
                {
                    var item = KH_TUONGTACRepository.GetKH_TUONGTACById(int.Parse(arrDeleteId[i], CultureInfo.InvariantCulture));
                    if (item != null)
                    {
                        item.IsDeleted = true;
                        KH_TUONGTACRepository.UpdateKH_TUONGTAC(item);
                    }
                }
                TempData[Globals.SuccessMessageKey] = App_GlobalResources.Wording.DeleteSuccess;
                return RedirectToAction("Detail", new { NGAYLAP = date, NGUOILAP_ID = name_id, FullName = name });
            }
            catch (DbUpdateException)
            {
                TempData[Globals.FailedMessageKey] = App_GlobalResources.Error.RelationError;
                return RedirectToAction("Detail", new { NGAYLAP = date, NGUOILAP_ID = name_id, FullName = name });
            }
        }
        #endregion

        #region Detail
        public ActionResult Detail(int? NGUOILAP_ID, string NGAYLAP, string search, string FullName)
        {
            var model = GETListKH_TUONGTAC(NGUOILAP_ID, NGAYLAP, search, FullName);
            model = model.OrderBy(x => x.GIO_TUONGTAC).ToList();
            ViewBag.SuccessMessage = TempData["SuccessMessage"];
            ViewBag.FailedMessage = TempData["FailedMessage"];
            ViewBag.AlertMessage = TempData["AlertMessage"];
            return View(model);
        }

        public List<KH_TUONGTACViewModel> GETListKH_TUONGTAC(int? NGUOILAP_ID, string NGAYLAP, string search, string FullName)
        {
            //if (!string.IsNullOrEmpty(search))
            //{
            //    DateTime ngaylap = Convert.ToDateTime(NGAYLAP);
            //    NGAYLAP = ngaylap.AddDays(-1).ToString("dd/MM/yyyy");

            //}
            NGUOILAP_ID = NGUOILAP_ID == null ? 0 : NGUOILAP_ID;
            NGAYLAP = NGAYLAP == null ? "" : NGAYLAP;
            FullName = FullName == null ? "" : FullName;
            var q = KH_TUONGTACRepository.GetAllvwKH_TUONGTAC().Where(x => x.NGAYLAP == NGAYLAP);
            var model = q.OrderByDescending(m => m.CustomerCode).ThenByDescending(m => m.CreatedDate).Select(item => new KH_TUONGTACViewModel
            {
                KH_TUONGTAC_ID = item.KH_TUONGTAC_ID,
                NGAYLAP = item.NGAYLAP,
                THANG = item.THANG,
                NAM = item.NAM,
                HINHTHUC_TUONGTAC = item.HINHTHUC_TUONGTAC,
                LOAI_TUONGTAC = item.LOAI_TUONGTAC,
                PHANLOAI_TUONGTAC = item.PHANLOAI_TUONGTAC,
                TINHTRANG_TUONGTAC = item.TINHTRANG_TUONGTAC,
                MUCDO_TUONGTAC = item.MUCDO_TUONGTAC,
                GHI_CHU = item.GHI_CHU,
                GIAIPHAP_TUONGTAC = item.GIAIPHAP_TUONGTAC,
                NGAYTUONGTAC_TIEP = item.NGAYTUONGTAC_TIEP,
                GIOTUONGTAC_TIEP = item.GIOTUONGTAC_TIEP,
                MUCCANHBAO_TUONGTAC = item.MUCCANHBAO_TUONGTAC,
                HINH_ANH = item.HINH_ANH,
                BranchId = item.BranchId,
                FullName = item.FullName,
                ModifiedDate = item.ModifiedDate,
                CreatedDate = item.CreatedDate,
                GIO_TUONGTAC = item.GIO_TUONGTAC,
                //LICHTUONGTATIEP = item.NGAYTUONGTAC_TIEP + "/" + item.THANG.Value + "/" + item.NAM.Value + " " + item.GIOTUONGTAC_TIEP,
                NGUOILAP_ID = item.NGUOILAP_ID,
                Phone = item.Phone,
                CustomerName = item.CustomerName,
                CustomerCode = item.CustomerCode,
                CustomerId = item.KHACHHANG_ID,
                KETQUA_SAUTUONGTAC = item.KETQUA_SAUTUONGTAC,
            }).ToList();

            bool hasSearch = false;
			hasSearch = true;
			//if (!string.IsNullOrEmpty(NGAYLAP))
   //         {
   //             NGAYLAP = Helpers.Common.ChuyenThanhKhongDau(NGAYLAP);
   //             model = model.Where(x => Helpers.Common.ChuyenThanhKhongDau(x.NGAYLAP).Contains(NGAYLAP)).ToList();
   //             hasSearch = true;
   //         }
            //if (!string.IsNullOrEmpty(NGAYKH))
            //{
            //    NGAYKH = Helpers.Common.ChuyenThanhKhongDau(NGAYKH);
            //    model = model.Where(x => Helpers.Common.ChuyenThanhKhongDau(x.NGAYLAP).Contains(NGAYKH)).ToList();
            //    hasSearch = true;
            //}
            if (NGUOILAP_ID != null && NGUOILAP_ID.Value > 0)
            {
                model = model.Where(x => x.NGUOILAP_ID == NGUOILAP_ID).ToList();
            }
            ViewBag.SuccessMessage = TempData["SuccessMessage"];
            ViewBag.FailedMessage = TempData["FailedMessage"];
            ViewBag.AlertMessage = TempData["AlertMessage"];
            return model;
        }
        public PartialViewResult GetGETListKH_TUONGTAC(int? NGUOILAP_ID, string NGAYLAP, string search, string FullName)
        {
            var model = GETListKH_TUONGTAC(NGUOILAP_ID, NGAYLAP, search, FullName);
            return PartialView(model);
        }
        #endregion
        #region Edit
        public ActionResult Edit(int? NGUOILAP_ID, int? KH_TUONGTAC_ID, string CustomerName, string NGAYLAP, int KHACHHANG_ID)
        {

            var DUOITOC = categoryRepository.GetAllCategories()
                .Select(item => new CategoryViewModel
                {
                    Value = item.Value,
                    Code = item.Code,
                    Name = item.Name,
                }).Where(x => x.Code == "DUOITOC");
            ViewBag.DUOITOC = DUOITOC;
            var DODAITOC = categoryRepository.GetAllCategories()
                .Select(item => new CategoryViewModel
                {
                    Value = item.Value,
                    Code = item.Code,
                    Name = item.Name,
                }).Where(x => x.Code == "DODAITOC");
            ViewBag.DODAITOC = DODAITOC;
            var DADAU = categoryRepository.GetAllCategories()
                .Select(item => new CategoryViewModel
                {
                    Value = item.Value,
                    Code = item.Code,
                    Name = item.Name,
                }).Where(x => x.Code == "DADAU");
            ViewBag.DADAU = DADAU;
            var MAMTOC = categoryRepository.GetAllCategories()
                .Select(item => new CategoryViewModel
                {
                    Value = item.Value,
                    Code = item.Code,
                    Name = item.Name,
                }).Where(x => x.Code == "MAMTOC");
            ViewBag.MAMTOC = MAMTOC;
            var THANTOC = categoryRepository.GetAllCategories()
                 .Select(item => new CategoryViewModel
                 {
                     Value = item.Value,
                     Code = item.Code,
                     Name = item.Name,
                 }).Where(x => x.Code == "THANTOC");
            ViewBag.THANTOC = THANTOC;

            var CO = categoryRepository.GetAllCategories()
             .Select(item => new CategoryViewModel
             {
                 Value = item.Value,
                 Code = item.Code,
                 Name = item.Name,
             }).Where(x => x.Code == "CO");
            ViewBag.CO = CO;
            var MAT = categoryRepository.GetAllCategories()
             .Select(item => new CategoryViewModel
             {
                 Value = item.Value,
                 Code = item.Code,
                 Name = item.Name,
             }).Where(x => x.Code == "MAT");
            ViewBag.MAT = MAT;
            var BODY = categoryRepository.GetAllCategories()
             .Select(item => new CategoryViewModel
             {
                 Value = item.Value,
                 Code = item.Code,
                 Name = item.Name,
             }).Where(x => x.Code == "BODY");
            ViewBag.BODY = BODY;
            var DAMAT = categoryRepository.GetAllCategories()
             .Select(item => new CategoryViewModel
             {
                 Value = item.Value,
                 Code = item.Code,
                 Name = item.Name,
             }).Where(x => x.Code == "DAMAT");
            ViewBag.DAMAT = DAMAT;
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
            var data = GETKH_TUONGTAC().Where(x => x.KHACHHANG_ID == KHACHHANG_ID);
            ViewBag.KH_TUONGTAC = data;

            var CustomerList = customerRepository.GetAllCustomer()
               .Select(item => new CustomerViewModel
               {
                   Code = item.Code,
                   Id = item.Id,
                   CompanyName = item.CompanyName,
                   Image = item.Image,
                   ManagerStaffId = item.ManagerStaffId
               }).Where(x => listNhanvien.Contains(x.ManagerStaffId.Value));

            //if (!Erp.BackOffice.Filters.SecurityFilter.IsAdmin())
            //{
            //    CustomerList = CustomerList.Where(x => x.ManagerStaffId == WebSecurity.CurrentUserId);
            //}
            ViewBag.CustomerList = CustomerList;
            ViewBag.NGUOILAP = NGUOILAP_ID;

            var KH_TUONGTAC = KH_TUONGTACRepository.GetKH_TUONGTACById(KH_TUONGTAC_ID.Value);
            if (KH_TUONGTAC != null && KH_TUONGTAC.IsDeleted != true)
            {
                var KH_TUONGTAC1 = new KH_TUONGTAC();

                var model = new KH_TUONGTACViewModel();
                AutoMapper.Mapper.Map(KH_TUONGTAC, model);
                //KH_TUONGTAC.NGAYTUONGTAC_TIEP= model.NGAYTUONGTAC_TIEP;
                //KH_TUONGTAC.GIOTUONGTAC_TIEP = model.GIOTUONGTAC_TIEP;
                KH_TUONGTAC1.NGAYTUONGTAC_TIEP = model.NGAYTUONGTAC_TIEP;
                KH_TUONGTAC1.GIOTUONGTAC_TIEP = model.GIOTUONGTAC_TIEP;
                // 20/4/2019 

                return View(model);
            }


            ViewBag.SuccessMessage = TempData["SuccessMessage"];
            ViewBag.FailedMessage = TempData["FailedMessage"];
            ViewBag.AlertMessage = TempData["AlertMessage"];
            return View();
        }



        public ActionResult EditView(int? NGUOILAP_ID)
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
            var data = GETKH_TUONGTAC();
            ViewBag.KH_TUONGTAC = data;


            ViewBag.SuccessMessage = TempData["SuccessMessage"];
            ViewBag.FailedMessage = TempData["FailedMessage"];
            ViewBag.AlertMessage = TempData["AlertMessage"];
            return View();
        }
        [HttpPost]
        public ActionResult Edit(KH_TUONGTACViewModel model)
        {
            var Now = DateTime.Now.Date;
            var NgayLap = Convert.ToDateTime(model.NGAYLAP);


            if (NgayLap < Now)
            {
                TempData[Globals.FailedMessageKey] = "Ngày cập nhật lớn hơn ngày lập nha !";
                return RedirectToAction("_ClosePopup", "Home", new { area = "", FunctionCallback = "ClosePopupAndReloadPage" });
            }
            var NGUOI_LAP = Request["NGUOILAP_ID"];
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
                var KH_TUONGTAC = KH_TUONGTACRepository.GetKH_TUONGTACById(model.KH_TUONGTAC_ID);
                AutoMapper.Mapper.Map(model, KH_TUONGTAC);
                KH_TUONGTAC.ModifiedUserId = WebSecurity.CurrentUserId;
                if (model.NGUOILAP_ID != null)
                {
                    KH_TUONGTAC.NGUOILAP_ID = model.NGUOILAP_ID;
                }
                else if (NGUOI_LAP != null)
                {
                    KH_TUONGTAC.NGUOILAP_ID = Convert.ToInt32(NGUOI_LAP);
                }
                else
                {
                    KH_TUONGTAC.NGUOILAP_ID = WebSecurity.CurrentUserId;
                }

                KH_TUONGTAC.ModifiedDate = DateTime.Now;
                KH_TUONGTAC.BranchId = (intBrandID == null ? Convert.ToInt32(Session["GlobalCurentBranchId"]) : intBrandID);
                KH_TUONGTAC.THANG = int.Parse(model.NGAYLAP.Substring(3, 2));//DateTime.Now.Month;
                KH_TUONGTAC.NAM = int.Parse(model.NGAYLAP.Substring(6, 4));//DateTime.Now.Year;
                //string pNgay = model.NGAYTUONGTAC_TIEP.Substring(0, 10);
                //KH_TUONGTAC.NGAYTUONGTAC_TIEP = pNgay;
                KH_TUONGTAC.NGAYTUONGTAC_TIEP = model.NGAYTUONGTAC_TIEP;

                KH_TUONGTAC.GIOTUONGTAC_TIEP = model.GIOTUONGTAC_TIEP;
                var path = Helpers.Common.GetSetting("Plan");
                var filepath = System.Web.HttpContext.Current.Server.MapPath("~" + path);
                if (Request.Files["file-image"] != null)
                {
                    var file = Request.Files["file-image"];
                    if (file.ContentLength > 0)
                    {
                        FileInfo fi = new FileInfo(Server.MapPath("~" + path) + KH_TUONGTAC.HINH_ANH);
                        if (fi.Exists)
                        {
                            fi.Delete();
                        }

                        string image_name = KH_TUONGTAC.KH_TUONGTAC_ID + "." + file.FileName.Split('.').Last();

                        bool isExists = System.IO.Directory.Exists(filepath);
                        if (!isExists)
                            System.IO.Directory.CreateDirectory(filepath);
                        file.SaveAs(filepath + image_name);
                        KH_TUONGTAC.HINH_ANH = image_name;
                    }
                }
                KH_TUONGTACRepository.UpdateKH_TUONGTAC(KH_TUONGTAC);
                //ghi log 
                Erp.BackOffice.Controllers.HomeController.WriteLog(KH_TUONGTAC.KH_TUONGTAC_ID, "", "đã cập nhật kế hoạch tương tác cho ngày " + KH_TUONGTAC.NGAYLAP, "", Helpers.Common.CurrentUser.BranchId.Value);


                //int month = int.Parse(NGAYLAP.Substring(3, 2));
                //int year = int.Parse(NGAYLAP.Substring(6, 4));

                //var dataNhanvien = SqlHelper.QuerySP<ManagerStaff>("spCopykehoachdukien", new
                //{
                //    BranchId = (intBrandID == null ? Convert.ToInt32(Session["GlobalCurentBranchId"]) : intBrandID),
                //    UserId = NGUOILAP_ID,
                //    Pngaylap = NGAYLAP,
                //    Month = month,
                //    year = year
                //}).ToList();

                TempData[Globals.SuccessMessageKey] = App_GlobalResources.Wording.UpdateSuccess;
                return RedirectToAction("_ClosePopup", "Home", new { area = "", FunctionCallback = "ClosePopupAndReloadPage" });
            }
            return View(model);
        }
        #endregion

        public ViewResult Index(int? Month, int? year, int? NGUOILAP, int? BranchId)
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
            //NGUOILAP = NGUOILAP == null ? WebSecurity.CurrentUserId : NGUOILAP;
            Month = Month == null ? DateTime.Now.Month : Month;
            year = year == null ? DateTime.Now.Year : year;
            var q = KH_TUONGTACRepository.GetAllvwKH_TUONGTAC().Where(x => x.IsDeleted != true);

            DateTime d_startDate, d_endDate;

            if (Month == null && year == null)
            {
                d_startDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                d_endDate = d_startDate.AddMonths(1).AddDays(-1);
                ViewBag.retDateTime = d_endDate;
                ViewBag.aDateTime = d_startDate;
            }

            if (Month != null)
            {
                if (year != null)
                {
                    d_startDate = new DateTime(year.Value, Month.Value, 1);
                    d_endDate = d_startDate.AddMonths(1).AddDays(-1);
                    ViewBag.retDateTime = d_endDate;
                    ViewBag.aDateTime = d_startDate;
                    q = q.Where(x => x.THANG == Month && x.NAM == year);
                }
            }


            //begin hoapd loc ra nhan vien ma minh quan ly
            var dataNhanvien = SqlHelper.QuerySP<ManagerStaff>("spgetNhanvienbacap", new
            {
                BranchId = (BranchId == null ? 0 : BranchId),
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


            var customer = KH_TUONGTACRepository.GetAllKH_TUONGTAC().Select(item => new KH_TUONGTACViewModel
            {
                KH_TUONGTAC_ID = item.KH_TUONGTAC_ID,
                THANG = item.THANG,
                NAM = item.NAM,
                NGUOILAP_ID = item.NGUOILAP_ID,
                KHACHHANG_ID = item.KHACHHANG_ID,
                KETQUA_SAUTUONGTAC = item.KETQUA_SAUTUONGTAC,
                NGAYLAP = item.NGAYLAP,
            }).Where(x => x.THANG == Month && x.NAM == year && listNhanvien.Contains(x.NGUOILAP_ID.Value)).ToList();
            //ViewBag.Customer = customer;

            //dùng cho tài khoản dùng chung
            var user2branch = user.Where(x => x.Id == WebSecurity.CurrentUserId).FirstOrDefault();
            if (BranchId != null && BranchId > 0)
            {
                q = q.Where(x => x.BranchId == BranchId);
                user = user.Where(x => x.BranchId == BranchId).ToList();
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
            var model = q.Select(item => new KH_TUONGTACViewModel
            {
                KH_TUONGTAC_ID = item.KH_TUONGTAC_ID,
                NGAYLAP = item.NGAYLAP,
                HINHTHUC_TUONGTAC = item.HINHTHUC_TUONGTAC,
                LOAI_TUONGTAC = item.LOAI_TUONGTAC,
                PHANLOAI_TUONGTAC = item.PHANLOAI_TUONGTAC,
                TINHTRANG_TUONGTAC = item.TINHTRANG_TUONGTAC,
                MUCDO_TUONGTAC = item.MUCDO_TUONGTAC,
                GHI_CHU = item.GHI_CHU,
                GIAIPHAP_TUONGTAC = item.GIAIPHAP_TUONGTAC,
                NGAYTUONGTAC_TIEP = item.NGAYTUONGTAC_TIEP,
                GIOTUONGTAC_TIEP = item.GIOTUONGTAC_TIEP,
                MUCCANHBAO_TUONGTAC = item.MUCCANHBAO_TUONGTAC,
                HINH_ANH = item.HINH_ANH,
                BranchId = item.BranchId,
                FullName = item.FullName,
                GIO_TUONGTAC = item.GIO_TUONGTAC,
                NGUOILAP_ID = item.NGUOILAP_ID,

                //THANG = item.THANG,
                //NAM = item.NAM,
                ModifiedDate = item.ModifiedDate,
                CreatedDate = item.CreatedDate,
                //LICHTUONGTATIEP = item.NGAYTUONGTAC_TIEP + "/" + item.THANG + "/" + item.NAM + " " + item.GIOTUONGTAC_TIEP,
            }).ToList();
            ViewBag.Customer = customer;

            ViewBag.user = user;
            ViewBag.SuccessMessage = TempData["SuccessMessage"];
            ViewBag.FailedMessage = TempData["FailedMessage"];
            ViewBag.AlertMessage = TempData["AlertMessage"];
            return View(model);
        }

        #region ExportExcel
        public List<vwPlanuseSkinCareViewModel> IndexExportPlanUseSkinCare(string CustomerCodeOrName, string ProductName, int? BranchId, string StartDate, string EndDate, string THANHTOAN, int? PhieuConTu, int? PhieuConDen, int? ManagerStaffId)
        {
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
            //if (ManagerStaffId == null || ManagerStaffId == 0)
            //{
            //   // ManagerStaffId = WebSecurity.CurrentUserId;
            //    return View(q);
            //}
            var q = PlanRepository.GetvwAllvwPlanuseSkinCare()
                 .Select(item => new vwPlanuseSkinCareViewModel
                 {
                     BranchId = item.BranchId,
                     CustomerCode = item.CustomerCode,
                     CustomerName = item.CustomerName,
                     Phone = item.Phone,
                     ProductInvoiceCode = item.ProductInvoiceCode,
                     ProductName = item.ProductName,
                     SOLUONG = item.SOLUONG,
                     soluongdung = item.soluongdung,
                     soluongtra = item.soluongtra,
                     soluongchuyen = item.soluongchuyen,
                     soluongconlai = item.soluongconlai,
                     Type = item.Type,
                     ModifiedDate = item.ModifiedDate,
                     GladLevel = item.GladLevel,
                     TargetId = item.TargetId,
                     CreatedDate = item.CreatedDate,
                     ProductInvoiceId = item.ProductInvoiceId,
                     THANGTOANHET = item.THANGTOANHET,
                     CustomerId = item.CustomerId,
                     ManagerStaffId = item.ManagerStaffId,
                     ManagerName = item.ManagerName
                 }).ToList();

            if (BranchId != null && BranchId > 0)
            {
                q = q.Where(x => x.BranchId == BranchId).ToList();
            }

            if (!string.IsNullOrEmpty(CustomerCodeOrName))
            {
                q = q.Where(x => Helpers.Common.ChuyenThanhKhongDau(x.CustomerCode).Contains(Helpers.Common.ChuyenThanhKhongDau(CustomerCodeOrName)) || x.CustomerName.Contains(CustomerCodeOrName)).ToList();
            }
            //if (!string.IsNullOrEmpty(CustomerName))
            //{
            //    //CustomerName = Helpers.Common.ChuyenThanhKhongDau(CustomerName);
            //    //q = q.Where(x => Helpers.Common.ChuyenThanhKhongDau(x.CustomerName).Contains(CustomerName));
            //}
            //if (!string.IsNullOrEmpty(CustomerCode))
            //{
            //    q = q.Where(x => x.CustomerCode.Contains(CustomerCode));
            //}

            if (!string.IsNullOrEmpty(ProductName))
            {
                q = q.Where(x => Helpers.Common.ChuyenThanhKhongDau(x.ProductName).Contains(Helpers.Common.ChuyenThanhKhongDau(ProductName))).ToList();
            }

            if (ManagerStaffId != null && ManagerStaffId > 0)
            {

                q = q.Where(x => x.ManagerStaffId == ManagerStaffId).ToList();
            }

            if (PhieuConTu != null)
            {
                if (PhieuConDen != null)
                {
                    q = q.Where(x => x.soluongconlai >= PhieuConTu && x.soluongconlai <= PhieuConDen).ToList();
                }
            }

            if (!string.IsNullOrEmpty(THANHTOAN))
            {
                THANHTOAN = Helpers.Common.ChuyenThanhKhongDau(THANHTOAN);
                if (THANHTOAN == "het")
                {
                    //THANHTOAN = null;
                    q = q.Where(x => Helpers.Common.ChuyenThanhKhongDau(x.THANGTOANHET).Contains(THANHTOAN)).ToList();
                }
                else
                {
                    q = q.Where(x => x.THANGTOANHET == null).ToList();
                }
            }
            //Lọc theo ngày

            if (!string.IsNullOrEmpty(StartDate))
            {
                var startDate = DateTime.ParseExact(StartDate, "dd/MM/yyyy", null).ToString("yyyy-MM-dd HH:mm:ss");
                DateTime d_startDate = Convert.ToDateTime(startDate);
                if (!string.IsNullOrEmpty(EndDate))
                {
                    var endDate = DateTime.ParseExact(EndDate, "dd/MM/yyyy", null).ToString("yyyy-MM-dd HH:mm:ss");
                    DateTime d_endDate = Convert.ToDateTime(endDate);
                    d_endDate = d_endDate.AddHours(23).AddMinutes(59);
                    q = q.Where(x => x.ModifiedDate >= d_startDate && x.ModifiedDate <= d_endDate).ToList();

                }
            }
            ///
            if (StartDate == null && EndDate == null)
            {
                DateTime aDateTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                // Cộng thêm 1 tháng và trừ đi một ngày.
                DateTime retDateTime = aDateTime.AddMonths(1).AddDays(-1);
                StartDate = aDateTime.ToString("dd/MM/yyyy");
                EndDate = retDateTime.ToString("dd/MM/yyyy");

                DateTime d_startDate1, d_endDate1;
                if (DateTime.TryParseExact(StartDate, "dd/MM/yyyy", new CultureInfo("vi-VN"), DateTimeStyles.None, out d_startDate1))
                {
                    if (DateTime.TryParseExact(EndDate, "dd/MM/yyyy", new CultureInfo("vi-VN"), DateTimeStyles.None, out d_endDate1))
                    {
                        d_endDate1 = d_endDate1.AddHours(23).AddMinutes(59);
                        //ViewBag.retDateTime = d_endDate;
                        //ViewBag.aDateTime = d_startDate;
                        q = q.Where(x => x.ModifiedDate >= d_startDate1 && x.ModifiedDate <= d_endDate1).ToList();
                    }
                }
            }

            return q;
        }

        public ActionResult ExportPlanUseSkinCare(string CustomerCodeOrName, string ProductName, int? BranchId, string StartDate, string EndDate, string THANHTOAN, int? PhieuConTu, int? PhieuConDen, int? ManagerStaffId, bool ExportExcel = false)
        {
            var data = TempData["Data_PlanUseSkinCare"] as List<vwPlanuseSkinCareViewModel>;
            TempData.Keep();
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
            model.Content = model.Content.Replace("{DataTable}", buildHtmlPlanUseSkinCare(data));
            model.Content = model.Content.Replace("{System.Logo}", ImgLogo);
            model.Content = model.Content.Replace("{System.CompanyName}", company);
            model.Content = model.Content.Replace("{System.AddressCompany}", address);
            model.Content = model.Content.Replace("{System.PhoneCompany}", phone);
            model.Content = model.Content.Replace("{System.Fax}", fax);
            model.Content = model.Content.Replace("{System.BankCodeCompany}", bankcode);
            model.Content = model.Content.Replace("{System.BankNameCompany}", bankname);
            model.Content = model.Content.Replace("{PrintDate}", DateTime.Now.ToString("dd/MM/yyyy HH:mm"));
            model.Content = model.Content.Replace("{Title}", "Kế hoạch sử dụng phiếu CSD");

            Response.AppendHeader("content-disposition", "attachment;filename=" + "DS_KHSDPhieuCSD" + DateTime.Now.ToString("dd_MM_yyyy") + ".xls");
            Response.Charset = "";
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.Write(model.Content);
            Response.End();

            return View(model);
        }

        string buildHtmlPlanUseSkinCare(List<vwPlanuseSkinCareViewModel> detailList)
        {
            string detailLists = "<table border=\"1\" class=\"invoice-detail\">\r\n";
            detailLists += "<thead>\r\n";
            detailLists += "	<tr>";
            detailLists += "		<th rowspan=\"2\">STT</th>";
            detailLists += "		<th rowspan=\"2\">Mã khách hàng</th>";
            detailLists += "		<th rowspan=\"2\">Tên khách hàng</th>";
            detailLists += "		<th rowspan=\"2\">Người quản lý</th>";
            detailLists += "		<th rowspan=\"2\">Điện thoại</th>";
            detailLists += "		<th colspan=\"9\">Đơn hàng</th>";
            detailLists += "		<th rowspan=\"2\">Thởi điểm chăm sóc gần nhất</th>";
            detailLists += "		<th rowspan=\"2\">Mức độ hài lòng</th>";
            detailLists += "	</tr>";

            detailLists += "	<tr>";
            detailLists += "		<th>Đơn hàng</th>";
            detailLists += "		<th>Tên dịch vụ</th>";
            detailLists += "		<th>Số lượng tổng</th>";
            detailLists += "		<th>Đã sử dụng</th>";
            detailLists += "		<th>Số lượng trả</th>";
            detailLists += "		<th>Số lượng chuyển</th>";
            detailLists += "		<th>Còn lại</th>";
            detailLists += "		<th>Thanh toán hết</th>";
            detailLists += "		<th>Loại CS</th>";
            detailLists += "	</tr>";

            detailLists += "</thead>\r\n";
            detailLists += "<tbody>\r\n";
            var index = 1;

            foreach (var item in detailList)
            {
                detailLists += "<tr>\r\n"
                + "<td class=\"text-center\">" + (index++) + "</td>\r\n"
                + "<td>" + item.CustomerCode + "</td>"
                + "<td>" + item.CustomerName + "</td>"
                + "<td>" + item.ManagerName + "</td>\r\n"
                + "<td>" + item.Phone + "</td>\r\n"
                + "<td style=\"text-align:center\">" + item.ProductInvoiceCode + "<br>" + item.CreatedDate.Value.ToString("dd/MM/yyyy") + "</td>\r\n"
                + "<td>" + item.ProductName + "</td>\r\n"
                + "<td>" + item.SOLUONG + "</td>\r\n"
                + "<td>" + item.soluongdung + "</td>\r\n"
                + "<td>" + item.soluongtra + "</td>\r\n"
                + "<td>" + item.soluongchuyen + "</td>\r\n"
                + "<td>" + item.soluongconlai + "</td>\r\n"
                + "<td>" + (item.THANGTOANHET == "Het" ? "Hết" : item.THANGTOANHET == "null" ? "Chưa có đơn hàng trong phiếu thu" : "Chưa") + "</td>\r\n"
                + "<td>" + (item.Type == "SkinScan" ? "CSD" : item.Type == "CheckingHair" ? "CST" : "") + "</td>\r\n"
                + "<td>" + (item.ModifiedDate == null ? "" : item.ModifiedDate.Value.ToString("dd/MM/yyyy HH:mm")) + "</td>\r\n"
                + "<td>" + item.GladLevel + "</td>\r\n"
                + "</tr>";
            }

            detailLists += "</tbody>\r\n";
            detailLists += "<tfoot style=\"font-weight:bold\">\r\n";
            detailLists += "</tfoot>\r\n</table>\r\n";
            return detailLists;
        }
        #endregion


        #region Ke Thua

        [HttpPost]
        public JsonResult ViewKeThua(string NGAYLAP, int NGUOILAP_ID)
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
            int month = int.Parse(NGAYLAP.Substring(3, 2));
            int year = int.Parse(NGAYLAP.Substring(6, 4));

            //var dataNhanvien = SqlHelper.QuerySP<ManagerStaff>("spCopykehoachdukien", new
            //{
            //    BranchId = (intBrandID == null ? Convert.ToInt32(Session["GlobalCurentBranchId"]) : intBrandID),
            //    UserId = NGUOILAP_ID,
            //    Pngaylap = NGAYLAP,
            //    Month = month,
            //    year = year
            //}).ToList();
            var Plan = KH_TUONGTACRepository.GetAllKH_TUONGTAC().Where(x => x.NGUOILAP_ID == NGUOILAP_ID && x.NGAYTUONGTAC_TIEP == NGAYLAP).ToList();

            if (Plan.Count > 0)
            {
                foreach (var item in Plan)
                {
                    var checkPlan = KH_TUONGTACRepository.GetAllKH_TUONGTAC().Where(x => x.NGAYLAP == NGAYLAP && x.KHACHHANG_ID == item.KHACHHANG_ID && x.LOAI_TUONGTAC == item.LOAI_TUONGTAC).FirstOrDefault();

                    if (checkPlan == null)
                    {

                        var KH_TUONGTAC2 = new KH_TUONGTAC();
                        // AutoMapper.Mapper.Map(model, KH_TUONGTAC2);
                        KH_TUONGTAC2.IsDeleted = false;
                        KH_TUONGTAC2.CreatedUserId = WebSecurity.CurrentUserId;
                        KH_TUONGTAC2.CreatedDate = DateTime.Now;
                        KH_TUONGTAC2.ModifiedDate = DateTime.Now;
                        KH_TUONGTAC2.LOAI_TUONGTAC = item.LOAI_TUONGTAC;
                        KH_TUONGTAC2.HINHTHUC_TUONGTAC = item.HINHTHUC_TUONGTAC;
                        KH_TUONGTAC2.LOAI_TUONGTAC = item.LOAI_TUONGTAC;
                        KH_TUONGTAC2.NGUOILAP_ID = item.NGUOILAP_ID;
                        KH_TUONGTAC2.BranchId = item.BranchId;
                        KH_TUONGTAC2.GIO_TUONGTAC = null;
                        KH_TUONGTAC2.NGAYLAP = item.NGAYTUONGTAC_TIEP.Substring(0, 10);
                        KH_TUONGTAC2.KHACHHANG_ID = item.KHACHHANG_ID;
                        KH_TUONGTAC2.THANG = int.Parse(item.NGAYTUONGTAC_TIEP.Substring(3, 2));//DateTime.Now.Month;
                        KH_TUONGTAC2.NAM = int.Parse(item.NGAYTUONGTAC_TIEP.Substring(6, 4));
                        // KH_TUONGTAC2.BranchId = (intBrandID == null ? Convert.ToInt32(Session["GlobalCurentBranchId"]) : intBrandID);
                        KH_TUONGTAC2.NGAYTUONGTAC_TIEP = null;
                        KH_TUONGTAC2.GIOTUONGTAC_TIEP = null;
                        KH_TUONGTAC2.GHI_CHU = item.GHI_CHU + " (Lấy trong lịch tương tác tiếp theo)";
                        KH_TUONGTACRepository.InsertKH_TUONGTAC(KH_TUONGTAC2);
                    }
                }

            }
            return Json(1);
            ///return RedirectToAction("Index", new { Month = int.Parse(NGAYLAP.Substring(3, 2)), year = int.Parse(NGAYLAP.Substring(6, 4)), NGUOILAP = WebSecurity.CurrentUserId, BranchId = intBrandID });

        }










        //[HttpPost]
        //[AllowAnonymous]
        //public ActionResult ViewKeThua(KH_TUONGTACViewModel model)
        //{
        //    try
        //    {
        //        if (ModelState.IsValid)
        //        {
        //            DateTime date = DateTime.Parse(model.NGAYLAP);
        //            foreach (var item in model.KH_TUONGTAC)
        //            {
        //                if (item.is_checked == 1)
        //                {
        //                    var KH_TUONGTAC = new KH_TUONGTAC();
        //                    //AutoMapper.Mapper.Map(model.KH_TUONGTAC, KH_TUONGTAC);
        //                    KH_TUONGTAC.IsDeleted = false;
        //                    KH_TUONGTAC.CreatedUserId = WebSecurity.CurrentUserId;
        //                    KH_TUONGTAC.CreatedDate = DateTime.Now;
        //                    KH_TUONGTAC.ModifiedDate = DateTime.Now;
        //                    KH_TUONGTAC.NGUOILAP_ID = WebSecurity.CurrentUserId;
        //                    KH_TUONGTAC.GIO_TUONGTAC = item.GIO_TUONGTAC;
        //                    KH_TUONGTAC.NGAYLAP = date.ToString("dd/MM/yyyy");
        //                    KH_TUONGTAC.THANG = int.Parse(item.LICHTUONGTATIEP.Substring(3, 1));
        //                    KH_TUONGTAC.NAM = int.Parse(item.LICHTUONGTATIEP.Substring(5, 4));
        //                    KH_TUONGTAC.NGAYTUONGTAC_TIEP = item.LICHTUONGTATIEP.Substring(0, 2);
        //                    KH_TUONGTAC.GIOTUONGTAC_TIEP = item.LICHTUONGTATIEP.Substring(10, 4);
        //                    KH_TUONGTAC.BranchId = Erp.BackOffice.Helpers.Common.CurrentUser.BranchId;
        //                    KH_TUONGTAC.KHACHHANG_ID = item.CustomerId;
        //                    KH_TUONGTAC.HINHTHUC_TUONGTAC = item.HINHTHUC_TUONGTAC;
        //                    KH_TUONGTAC.GIO_TUONGTAC = item.GIO_TUONGTAC;
        //                    KH_TUONGTAC.LOAI_TUONGTAC = item.LOAI_TUONGTAC;
        //                    KH_TUONGTAC.PHANLOAI_TUONGTAC = item.PHANLOAI_TUONGTAC;
        //                    KH_TUONGTAC.TINHTRANG_TUONGTAC = item.TINHTRANG_TUONGTAC;
        //                    KH_TUONGTAC.MUCDO_TUONGTAC = item.MUCDO_TUONGTAC;
        //                    KH_TUONGTAC.GIAIPHAP_TUONGTAC = item.GIAIPHAP_TUONGTAC;
        //                    KH_TUONGTAC.MUCCANHBAO_TUONGTAC = item.MUCCANHBAO_TUONGTAC;
        //                    KH_TUONGTAC.GHI_CHU = item.GHI_CHU;
        //                    KH_TUONGTAC.HINH_ANH = item.HINH_ANH;

        //                    var path = System.Web.HttpContext.Current.Server.MapPath("~" + Helpers.Common.GetSetting("product-image-folder"));
        //                    if (Request.Files["file-image"] != null)
        //                    {
        //                        var file = Request.Files["file-image"];
        //                        if (file.ContentLength > 0)
        //                        {
        //                            string image_name = "KH_TUONGTAC_" + Helpers.Common.ChuyenThanhKhongDau(Regex.Replace(KH_TUONGTAC.TINHTRANG_TUONGTAC, @"\s+", "_")) + "." + file.FileName.Split('.').Last();
        //                            bool isExists = System.IO.Directory.Exists(path);
        //                            if (!isExists)
        //                                System.IO.Directory.CreateDirectory(path);
        //                            file.SaveAs(path + image_name);
        //                            KH_TUONGTAC.HINH_ANH = image_name;
        //                        }
        //                    }
        //                    KH_TUONGTACRepository.InsertKH_TUONGTAC(KH_TUONGTAC);

        //                    TempData[Globals.SuccessMessageKey] = App_GlobalResources.Wording.InsertSuccess;
        //                    return RedirectToAction("_ClosePopup", "Home", new { area = "", FunctionCallback = "ClosePopupAndReloadPage" });
        //                }
        //            }

        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //    }

        //    return View();
        //}
        #endregion
        #region LichSuTuongTac
        public ViewResult LichSuTuongTac(int? NGUOILAP_ID, int? KH_TUONGTAC_ID, string CustomerName, string NGAYLAP, int KHACHHANG_ID)
        {
            var data = GETKH_TUONGTAC().Where(x => x.KHACHHANG_ID == KHACHHANG_ID);
            ViewBag.KH_TUONGTAC = data;
            return View(data.FirstOrDefault());
        }
        #endregion
        #region ThongKeKH
        public ActionResult ThongKeKH(int? branchId, string StartDate, string EndDate)
        {
            branchId = branchId == null ? 0 : branchId;

            EndDate = string.IsNullOrEmpty(EndDate) ? "" : EndDate;
            StartDate = string.IsNullOrEmpty(StartDate) ? "" : StartDate;

            DateTime aDateTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            // Cộng thêm 1 tháng và trừ đi một ngày.
            DateTime retDateTime = aDateTime.AddMonths(1).AddDays(-1).AddHours(23).AddMinutes(59);

            var d_startDate = (!string.IsNullOrEmpty(StartDate) == true ? DateTime.ParseExact(StartDate, "dd/MM/yyyy", null).ToString("yyyy-MM-dd HH:mm:ss") : aDateTime.ToString("yyyy-MM-dd HH:mm:ss"));
            var d_endDate = (!string.IsNullOrEmpty(EndDate) == true ? DateTime.ParseExact(EndDate, "dd/MM/yyyy", null).AddHours(23).AddMinutes(59).ToString("yyyy-MM-dd HH:mm:ss") : retDateTime.ToString("yyyy-MM-dd HH:mm:ss"));
            var data = SqlHelper.QuerySP<InquiryCardViewModel>("spSale_ThongKeKH", new
            {
                branchId = branchId,
                StartDate = d_startDate,
                EndDate = d_endDate,
            }).ToList();
            if (!Filters.SecurityFilter.IsAdmin() && branchId == 0)
            {
                branchId = Helpers.Common.CurrentUser.BranchId;
            }
            data = data.GroupBy(x => x.CustomerId).Select(x => x.First()).OrderByDescending(x => x.CreatedDate).ToList();
            return View(data);
        }
        #endregion

        public ViewResult KH_TuongTac(string StartDate, string EndDate, int? userID)
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

            if (StartDate == null && EndDate == null)
            {
                DateTime aDateTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                // Cộng thêm 1 tháng và trừ đi một ngày.
                DateTime retDateTime = aDateTime.AddMonths(1).AddDays(-1);
                StartDate = aDateTime.ToString("dd/MM/yyyy");
                EndDate = retDateTime.ToString("dd/MM/yyyy");
            }


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

            var user = userRepository.GetAllUsers().Select(item => new UserViewModel
            {
                Id = item.Id,
                UserName = item.UserName,
                FullName = item.FullName,
                Status = item.Status,
                BranchId = item.BranchId
            }).Where(x => x.Status == UserStatus.Active).ToList();

            ViewBag.user = user.Where(x => listNhanvien.Contains(x.Id));



            var use = SqlHelper.QuerySP<KH_TUONGTACViewModel>("TOIUUCSKH_AllvwCRM_TK_TUONGTAC").ToList();

            use = use.Where(x => listNhanvien.Contains(x.NGUOILAP_ID.Value)).ToList();

            foreach (var item in use)
            {
                item.Ngay_Lap = Convert.ToDateTime(item.NGAYLAP);
            }
            use = use.OrderByDescending(x => x.Ngay_Lap).ToList();

            DateTime d_startDate, d_endDate;
            if (DateTime.TryParseExact(StartDate, "dd/MM/yyyy", new CultureInfo("vi-VN"), DateTimeStyles.None, out d_startDate))
            {
                if (DateTime.TryParseExact(EndDate, "dd/MM/yyyy", new CultureInfo("vi-VN"), DateTimeStyles.None, out d_endDate))
                {
                    d_endDate = d_endDate.AddHours(23).AddMinutes(59);
                    use = use.Where(x => Convert.ToDateTime(x.NGAYLAP) >= d_startDate && Convert.ToDateTime(x.NGAYLAP) <= d_endDate).ToList();
                }
            }

            if (userID != null)
            {
                use = use.Where(x => x.NGUOILAP_ID == userID).ToList();
            }

            return View(use);


        }

        #region Kiểm tra kế hoạch
        public ViewResult CheckPlan(string startDate, string endDate, int? branchId, string txtCusInfo, int? SalerId, string HINHTHUC_TUONGTAC)
        {

            startDate = string.IsNullOrEmpty(startDate) ? "" : startDate;
            endDate = string.IsNullOrEmpty(endDate) ? "" : endDate;


            DateTime aDateTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);

            // Cộng thêm 1 tháng và trừ đi một ngày.
            DateTime retDateTime = aDateTime.AddMonths(1).AddDays(-1).AddHours(23).AddMinutes(59);

            var d_startDate = (!string.IsNullOrEmpty(startDate) == true ? DateTime.ParseExact(startDate, "dd/MM/yyyy", null).ToString("yyyy-MM-dd HH:mm:ss") : aDateTime.ToString("yyyy-MM-dd HH:mm:ss"));
            var d_endDate = (!string.IsNullOrEmpty(endDate) == true ? DateTime.ParseExact(endDate, "dd/MM/yyyy", null).AddHours(23).AddMinutes(59).ToString("yyyy-MM-dd HH:mm:ss") : retDateTime.ToString("yyyy-MM-dd HH:mm:ss"));
            //get cookie brachID ]\
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

            branchId = intBrandID;

            ///lọc theo nhân viên
            var u = userRepository.GetUserById(WebSecurity.CurrentUserId);
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

            var user = userRepository.GetAllUsers().Select(item => new UserViewModel
            {
                Id = item.Id,
                UserName = item.UserName,
                FullName = item.FullName,
                Status = item.Status,
                BranchId = item.BranchId,
                IdManager = item.IdManager,
                UserTypeId = item.UserTypeId
            }).Where(x => x.Status == UserStatus.Active && listNhanvien.Contains(x.Id)).ToList();
            ViewBag.user2 = user;
            //

            var model = SqlHelper.QuerySP<CheckPlanViewModel>("CheckCusHavePlan", new
            {
                StartDate = d_startDate,
                EndDate = d_endDate,
                branchId = branchId

            }).ToList();


            if (!Filters.SecurityFilter.IsAdmin() && u.IsLetan != true && (u.UserTypeId != 1027 || u.UserTypeId != 2053))
            {
                model = model.Where(x => x.ManagerStaffId != null && listNhanvien.Contains(x.ManagerStaffId.Value)).ToList();
            }

            if (!string.IsNullOrEmpty(txtCusInfo))
            {
                model = model.Where(x => Helpers.Common.ChuyenThanhKhongDau(x.Name).Contains(Helpers.Common.ChuyenThanhKhongDau(txtCusInfo)) || x.Code.Contains(txtCusInfo)).ToList();
            }
            if ((SalerId != null) && (SalerId > 0))
            {
                model = model.Where(x => x.ManagerStaffId == SalerId).ToList();

            }
            if (!string.IsNullOrEmpty(HINHTHUC_TUONGTAC))
            {
                if (HINHTHUC_TUONGTAC == "Chưa có KH")
                {
                    model = model.Where(x => x.HINHTHUC_TUONGTAC == null).ToList();

                }
                else
                {
                    model = model.Where(x => Helpers.Common.ChuyenThanhKhongDau(x.HINHTHUC_TUONGTAC).Contains(Helpers.Common.ChuyenThanhKhongDau(HINHTHUC_TUONGTAC))).ToList();
                }
            }
            TempData["Data_CheckPlan"] = model;
            return View(model);

            //return View();
        }
        public ActionResult PrintCheckPlan(int? Month, int? Year, int? branchId, string txtCusInfo, int? SalerId, string Brand, string HINHTHUC_TUONGTAC, bool ExportExcel = false)
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
            branchId = intBrandID;
            var data = TempData["Data_CheckPlan"] as List<CheckPlanViewModel>;
            TempData.Keep();
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
            model.Content = model.Content.Replace("{DataTable}", buildHtmlCheckPlan(data));
            model.Content = model.Content.Replace("{System.Logo}", ImgLogo);
            model.Content = model.Content.Replace("{System.CompanyName}", company);
            model.Content = model.Content.Replace("{System.AddressCompany}", address);
            model.Content = model.Content.Replace("{System.PhoneCompany}", phone);
            model.Content = model.Content.Replace("{System.Fax}", fax);
            model.Content = model.Content.Replace("{System.BankCodeCompany}", bankcode);
            model.Content = model.Content.Replace("{System.BankNameCompany}", bankname);
            model.Content = model.Content.Replace("{PrintDate}", DateTime.Now.ToString("dd/MM/yyyy HH:mm"));
            model.Content = model.Content.Replace("{Title}", "Kiểm tra kế hoạch");
            if (ExportExcel)
            {
                Response.AppendHeader("content-disposition", "attachment;filename=" + "KiemTraKeHoach" + DateTime.Now.ToString("yyyyMMdd") + ".xls");
                Response.Charset = "";
                Response.Cache.SetCacheability(HttpCacheability.NoCache);
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.Write(model.Content);
                Response.End();
            }
            return View(model);
        }
        string buildHtmlCheckPlan(List<CheckPlanViewModel> detailList)
        {
            string detailLists = "<table border=\"1\" class=\"invoice - detail\">";
            detailLists += "<thead>";
            detailLists += "	<tr>";
            detailLists += "		<th>STT</th>";
            detailLists += "		<th>Mã KH</th>";
            detailLists += "		<th>Tên KH</th>";
            detailLists += "		<th>Điện thoại</th>";
            detailLists += "        <th>Hình thức TT</th>";
            detailLists += "        <th>Ngày</th>";
            detailLists += "        <th>Giờ</th>";
            detailLists += "        <th>KHBH</th>";
            detailLists += "		<th>Thực tế BH</th>";
            detailLists += "		<th>Tỷ lệ BH</th>";
            detailLists += "		<th>KHCDS</th>";
            detailLists += "		<th>Thực tế KHCDS</th>";
            detailLists += "		<th>Tỷ lệ DS</th>";
            detailLists += "		<th>NVQL</th>";
            detailLists += "	</tr>";
            detailLists += "</thead>";
            detailLists += "<tbody>";
            var index = 1;
            decimal totalBH = 0;
            decimal totalDS = 0;
            decimal totalTargetBH = 0;
            decimal totalTargetDS = 0;
            foreach (var item in detailList)
            {
                detailLists += "<tr>"
                + "<td>" + (index++) + "</td>"
                + "<td>" + item.Code + "</td>"
                + "<td>" + item.Name + "</td>"
                + "<td>" + item.Name + "</td>"
                + "<td>" + item.Phone + "</td>"
                + "<td>" + item.HINHTHUC_TUONGTAC + "</td>"
                + "<td>" + item.NGAYLAP + "</td>"
                + "<td>" + item.GIO_TUONGTAC + "</td>"
                + "<td>" + item.KHBH + "</td>"
                + "<td>" + item.ThucTeBH + "</td>"
                + "<td>" + item.BHTL + "</td>"
                + "<td>" + item.KHCDS + "</td>"
                + "<td>" + item.ThucTeDS + "</td>"
                + "<td>" + item.DSTL + "</td>"
                + "<td>" + item.ManagerStaffName + "</td>"
                + "</tr>";
                totalBH += item.TotalBH ?? 0;
                totalDS += item.TotalDS ?? 0;
                totalTargetBH += item.TARGET_BRAND ?? 0;
                totalTargetDS += item.TARGETDS ?? 0;
            }
            detailLists += "</tbody>\r\n";
            detailLists += "<tfoot style=\"font-weight:bold\">\r\n";
            detailLists += "</tfoot>\r\n</table>\r\n";
            return detailLists;
        }
        #endregion

        #region Thống kê kế hoạch
        public ViewResult DetailCusPlan(string startDate, string endDate, int? branchId, string txtCusInfo, int? SalerId, string Brand, string HINHTHUC_TUONGTAC, string tlbh, string tlds)
        {

            var category = categoryRepository.GetAllCategories()
           .Select(item => new CategoryViewModel
           {
               Id = item.Id,
               Code = item.Code,
               Value = item.Value,
               Name = item.Name
           }).Where(x => x.Code == "Origin").ToList();
            ViewBag.category = category;

            startDate = string.IsNullOrEmpty(startDate) ? "" : startDate;
            endDate = string.IsNullOrEmpty(endDate) ? "" : endDate;

            DateTime aDateTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);

            // Cộng thêm 1 tháng và trừ đi một ngày.
            DateTime retDateTime = aDateTime.AddMonths(1).AddDays(-1).AddHours(23).AddMinutes(59);

            var d_startDate = (!string.IsNullOrEmpty(startDate) == true ? DateTime.ParseExact(startDate, "dd/MM/yyyy", null).ToString("yyyy-MM-dd HH:mm:ss") : aDateTime.ToString("yyyy-MM-dd HH:mm:ss"));
            var d_endDate = (!string.IsNullOrEmpty(endDate) == true ? DateTime.ParseExact(endDate, "dd/MM/yyyy", null).AddHours(23).AddMinutes(59).ToString("yyyy-MM-dd HH:mm:ss") : retDateTime.ToString("yyyy-MM-dd HH:mm:ss"));
            //get cookie brachID ]\
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

            branchId = intBrandID;

            ///lọc theo nhân viên
            var u = userRepository.GetUserById(WebSecurity.CurrentUserId);
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

            var user = userRepository.GetAllUsers().Select(item => new UserViewModel
            {
                Id = item.Id,
                UserName = item.UserName,
                FullName = item.FullName,
                Status = item.Status,
                BranchId = item.BranchId,
                IdManager = item.IdManager,
                UserTypeId = item.UserTypeId
            }).Where(x => x.Status == UserStatus.Active && listNhanvien.Contains(x.Id)).ToList();
            ViewBag.user2 = user;
            //

            var model = SqlHelper.QuerySP<CheckPlanViewModel>("DetailCusPlan", new
            {
                StartDate = d_startDate,
                EndDate = d_endDate,
                branchId = branchId
            }).ToList();

            if (!Filters.SecurityFilter.IsAdmin() && u.IsLetan != true && (u.UserTypeId != 1027 || u.UserTypeId != 2053))
            {
                model = model.Where(x => x.ManagerStaffId != null && listNhanvien.Contains(x.ManagerStaffId.Value)).ToList();
            }

            if (!string.IsNullOrEmpty(txtCusInfo))
            {
                model = model.Where(x => Helpers.Common.ChuyenThanhKhongDau(x.Name).Contains(Helpers.Common.ChuyenThanhKhongDau(txtCusInfo)) || x.Code.Contains(txtCusInfo)).ToList();
            }
            if ((SalerId != null) && (SalerId > 0))
            {
                model = model.Where(x => x.ManagerStaffId == SalerId).ToList();

            }
            if (!string.IsNullOrEmpty(Brand))
            {
                model = model.Where(x => Helpers.Common.ChuyenThanhKhongDau(x.CountForBrand).Contains(Helpers.Common.ChuyenThanhKhongDau(Brand))).ToList();
            }
            if (!string.IsNullOrEmpty(HINHTHUC_TUONGTAC))
            {
                model = model.Where(x => Helpers.Common.ChuyenThanhKhongDau(x.HINHTHUC_TUONGTAC).Contains(Helpers.Common.ChuyenThanhKhongDau(HINHTHUC_TUONGTAC))).ToList();
            }

            if (tlbh != null && tlbh != "")
            {
                model = model.Where(x => x.BHTL == Int32.Parse(tlbh)).ToList();
            }
            if (tlds != null && tlds != "")
            {
                model = model.Where(x => x.DSTL == Int32.Parse(tlds)).ToList();
            }
            List<int> TLBH = new List<int>();
            TLBH.Add(-1);
            for (int i = 0; i <= 10; i++)
            {
                TLBH.Add(i * 10);
            }
            ViewBag.TLBH = TLBH;
            List<int> TLDS = new List<int>();
            TLDS.Add(-1);
            for (int i = 0; i <= 10; i++)
            {
                TLDS.Add(i * 10);
            }
            ViewBag.TLDS = TLDS;
            ViewBag.TONGKHBH = model.Sum(x => x.TotalBH);
            ViewBag.TONGKHDS = model.Sum(x => x.TotalDS);
            TempData["Data_DetailCusPlan"] = model;
            return View(model);

            //return View();
        }
        public ActionResult PrintDetailCusPlan(string txtCode, string txtCusInfo, string txtProductName, string Status, int? SalerId, string startDate, string endDate, string txtMinAmount, string txtMaxAmount, int? BranchId, bool ExportExcel = false)
        {
            var data = TempData["Data_DetailCusPlan"] as List<CheckPlanViewModel>;
            TempData.Keep();
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
            model.Content = model.Content.Replace("{DataTable}", buildHtmlDetailCusPlan(data));
            model.Content = model.Content.Replace("{System.Logo}", ImgLogo);
            model.Content = model.Content.Replace("{System.CompanyName}", company);
            model.Content = model.Content.Replace("{System.AddressCompany}", address);
            model.Content = model.Content.Replace("{System.PhoneCompany}", phone);
            model.Content = model.Content.Replace("{System.Fax}", fax);
            model.Content = model.Content.Replace("{System.BankCodeCompany}", bankcode);
            model.Content = model.Content.Replace("{System.BankNameCompany}", bankname);
            model.Content = model.Content.Replace("{PrintDate}", DateTime.Now.ToString("dd/MM/yyyy HH:mm"));
            model.Content = model.Content.Replace("{Title}", "Chi tiết kế hoạch khách hàng");
            if (ExportExcel)
            {
                Response.AppendHeader("content-disposition", "attachment;filename=" + "ChiTiet_KHKH" + DateTime.Now.ToString("yyyyMMdd") + ".xls");
                Response.Charset = "";
                Response.Cache.SetCacheability(HttpCacheability.NoCache);
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.Write(model.Content);
                Response.End();
            }
            return View(model);
        }

        string buildHtmlDetailCusPlan(List<CheckPlanViewModel> detailList)
        {
            string detailLists = "<table border=\"1\" class=\"invoice-detail\">\r\n";
            detailLists += "<thead>\r\n";
            detailLists += "	<tr>";
            detailLists += "		<th>STT</th>";
            detailLists += "		<th>Mã KH</th>";
            detailLists += "		<th>Tên KH</th>";
            detailLists += "		<th>Điện thoại</th>";
            detailLists += "		<th>Hình thức TT</th>";
            detailLists += "		<th>Ngày</th>";
            detailLists += "		<th>Giờ</th>";
            detailLists += "		<th>Nhãn hàng</th>";
            detailLists += "		<th>KHBH</th>";
            detailLists += "		<th>Tỷ lệ BH</th>";
            detailLists += "		<th>Ngày lập</th>";
            detailLists += "		<th>Ngày chỉnh sửa</th>";
            detailLists += "		<th>Người duyệt KHBH</th>";
            detailLists += "		<th>KHCDS</th>";
            detailLists += "		<th>Tỷ lệ DS</th>";
            detailLists += "		<th>NVQL</th>";
            detailLists += "	</tr>";
            detailLists += "</thead>\r\n";
            detailLists += "<tbody>\r\n";
            var index = 1;
            decimal totalBH = 0;
            decimal totalDS = 0;


            foreach (var item in detailList)
            {
                detailLists += "<tr>\r\n"
                + "<td class=\"text-center orderNo\">" + (index++) + "</td>\r\n"
                + "<td class=\"text-left \">" + item.Code + "</td>\r\n"
                + "<td class=\"text-left \">" + item.Name + "</td>\r\n"
                + "<td>" + item.Phone + "</td>"
                + "<td class=\"text-left \">" + item.HINHTHUC_TUONGTAC + "</td>\r\n"
                + "<td class=\"text-left \">" + item.NGAYLAP + "</td>\r\n"
                + "<td class=\"text-left \">" + item.GIO_TUONGTAC + "</td>\r\n"
                + "<td class=\"text-left \">" + item.CountForBrand + "</td>\r\n"
                + "<td class=\"text-left \">" + CommonSatic.ToCurrencyStr(item.TotalBH, null).Replace(".", ",") + "</td>\r\n"
                + "<td class=\"text-left \">" + item.BHTL + "</td>\r\n"
                 + "<td class=\"text-left \">" + item.CreatedDate + "</td>\r\n"
                  + "<td class=\"text-left \">" + item.ModifiedDate + "</td>\r\n"
                  + "<td class=\"text-left \">" + item.UserApproveName + "</td>\r\n"
                + "<td class=\"text-left \">" + CommonSatic.ToCurrencyStr(item.TotalDS, null).Replace(".", ",") + "</td>\r\n"
                + "<td class=\"text-left \">" + item.DSTL + "</td>\r\n"
                + "<td class=\"text-left \">" + item.ManagerStaffName + "</td>\r\n"
                + "</tr>\r\n";
                totalBH += item.TotalBH ?? 0;
                totalDS += item.TotalDS ?? 0;

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
               + "<td class=\"text-left \" style=\"font-weight:bold \">" + CommonSatic.ToCurrencyStr(totalBH, null).Replace(".", ",") + "</td>\r\n"
               + "<td class=\"text-left \">" + "" + "</td>\r\n"
               + "<td class=\"text-left \" style=\"font-weight:bold \">" + CommonSatic.ToCurrencyStr(totalDS, null).Replace(".", ",") + "</td>\r\n"
               + "<td class=\"text-left \">" + "" + "</td>\r\n"
               + "<td class=\"text-left \">" + "" + "</td>\r\n"
               + "</tr>\r\n";

            detailLists += "</tbody>\r\n";
            detailLists += "<tfoot style=\"font-weight:bold\">\r\n";
            detailLists += "</tfoot>\r\n</table>\r\n";
            return detailLists;
        }
        #endregion


        #region Tổng hợp kế hoạch
        public ViewResult GeneralCusPlan(int? Month, int? Year, int? branchId, string txtCusInfo, int? SalerId, string Brand, string HINHTHUC_TUONGTAC, string tlbh, string tlds, int? UserApprove)
        {
            var model = GETGeneralCusPlan(Month, Year, branchId, txtCusInfo, SalerId, Brand, HINHTHUC_TUONGTAC, tlbh, tlds, UserApprove);
            TempData["DataGeneralCusPlan"] = model;
            return View(model);
        }
        public List<CheckPlanViewModel> GETGeneralCusPlan(int? Month, int? Year, int? branchId, string txtCusInfo, int? SalerId, string Brand, string HINHTHUC_TUONGTAC, string tlbh, string tlds, int? UserApprove)
        {

            var category = categoryRepository.GetAllCategories()
            .Select(item => new CategoryViewModel
            {
                Id = item.Id,
                Code = item.Code,
                Value = item.Value,
                Name = item.Name
            }).Where(x => x.Code == "Origin").ToList();
            ViewBag.category = category;

            Month = Month == null ? DateTime.Now.Month : Month;
            Year = Year == null ? DateTime.Now.Year : Year;

            //get cookie brachID ]\
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

            branchId = intBrandID;

            ///lọc theo nhân viên
            var u = userRepository.GetUserById(WebSecurity.CurrentUserId);
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

            var user = userRepository.GetAllUsers().Select(item => new UserViewModel
            {
                Id = item.Id,
                UserName = item.UserName,
                FullName = item.FullName,
                Status = item.Status,
                BranchId = item.BranchId,
                IdManager = item.IdManager,
                UserTypeId = item.UserTypeId
            }).Where(x => x.Status == UserStatus.Active && listNhanvien.Contains(x.Id)).ToList();
            ViewBag.user2 = user;
            //
            //var ProductInvoice = productInvoiceRepository.GetAllvwProductInvoice().Select(item => new ProductInvoiceViewModel
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

            //}).Where(x => x.Month == Month && x.Year == Year && x.IsArchive == true && x.Status == "complete").ToList();
            //ViewBag.ProductInvoice = ProductInvoice;

            var model = SqlHelper.QuerySP<CheckPlanViewModel>("TotalCusPlan", new
            {
                Month = Month,
                Year = Year,
                branchId = branchId
            }).ToList();

            if (!Filters.SecurityFilter.IsAdmin() && u.IsLetan != true && (u.UserTypeId != 1027 || u.UserTypeId != 2053))
            {
                model = model.Where(x => x.ManagerStaffId != null && listNhanvien.Contains(x.ManagerStaffId.Value)).ToList();
            }

            if (!string.IsNullOrEmpty(txtCusInfo))
            {
                model = model.Where(x => Helpers.Common.ChuyenThanhKhongDau(x.Name).Contains(Helpers.Common.ChuyenThanhKhongDau(txtCusInfo)) || x.Code.Contains(txtCusInfo)).ToList();
            }
            if ((SalerId != null) && (SalerId > 0))
            {
                model = model.Where(x => x.ManagerStaffId == SalerId).ToList();

            }
            if ((UserApprove != null) && (UserApprove > 0))
            {
                model = model.Where(x => x.UserApprove == UserApprove).ToList();

            }
            if (!string.IsNullOrEmpty(Brand))
            {
                model = model.Where(x => Helpers.Common.ChuyenThanhKhongDau(x.CountForBrand).Contains(Helpers.Common.ChuyenThanhKhongDau(Brand))).ToList();
            }
            if (!string.IsNullOrEmpty(HINHTHUC_TUONGTAC))
            {
                model = model.Where(x => Helpers.Common.ChuyenThanhKhongDau(x.HINHTHUC_TUONGTAC).Contains(Helpers.Common.ChuyenThanhKhongDau(HINHTHUC_TUONGTAC))).ToList();
            }
            if (tlbh != null && tlbh != "")
            {
                model = model.Where(x => x.BHTL == Int32.Parse(tlbh)).ToList();
            }
            if (tlds != null && tlds != "")
            {
                model = model.Where(x => x.DSTL == Int32.Parse(tlds)).ToList();
            }

            //Kiểm tra trường hợp cùng kh và cùng tỉ lệ
            var modeltrung = SqlHelper.QuerySP<CheckPlanViewModel>("checkTrungKHBH", new
            {
                Month = Month,
                Year = Year,
                branchId = branchId
            }).ToList();

            int sodem = 0;
            foreach (var a in model)
            {
                foreach (var i in modeltrung)
                {
                    if (a.Id == i.Id)
                    {
                        if (sodem > 0)
                        {
                            a.ThucTeBH = 0;
                        }
                        sodem++;

                    }
                }

            }
            List<int> TLBH = new List<int>();
            TLBH.Add(-1);
            for (int i = 0; i <= 10; i++)
            {
                TLBH.Add(i * 10);
            }
            ViewBag.TLBH = TLBH;
            List<int> TLDS = new List<int>();
            TLDS.Add(-1);
            for (int i = 0; i <= 10; i++)
            {
                TLDS.Add(i * 10);
            }
            ViewBag.TLDS = TLDS;
            ViewBag.TONGKHBH = model.Sum(x => x.TotalBH);
            ViewBag.TONGKHDS = model.Sum(x => x.TotalDS);
            ViewBag.TONGTHUCKHBH = model.Sum(x => x.ThucTeBH);
            ViewBag.TONGTHUCKHDS = model.Sum(x => x.ThucTeDS);
            return model;
        }
        public ActionResult PrintGeneralCusPlan(int? Month, int? Year, int? branchId, string txtCusInfo, int? SalerId, string Brand, string HINHTHUC_TUONGTAC, bool ExportExcel = false)
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
            branchId = intBrandID;
            var data = TempData["DataGeneralCusPlan"] as List<CheckPlanViewModel>;
            TempData.Keep();
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
            model.Content = model.Content.Replace("{DataTable}", buildHtmlGeneralCusPlan(data));
            model.Content = model.Content.Replace("{System.Logo}", ImgLogo);
            model.Content = model.Content.Replace("{System.CompanyName}", company);
            model.Content = model.Content.Replace("{System.AddressCompany}", address);
            model.Content = model.Content.Replace("{System.PhoneCompany}", phone);
            model.Content = model.Content.Replace("{System.Fax}", fax);
            model.Content = model.Content.Replace("{System.BankCodeCompany}", bankcode);
            model.Content = model.Content.Replace("{System.BankNameCompany}", bankname);
            model.Content = model.Content.Replace("{PrintDate}", DateTime.Now.ToString("dd/MM/yyyy HH:mm"));
            model.Content = model.Content.Replace("{Title}", "Tổng hợp kế hoạch");
            if (ExportExcel)
            {
                Response.AppendHeader("content-disposition", "attachment;filename=" + "TongHopKeHoach" + DateTime.Now.ToString("yyyyMMdd") + ".xls");
                Response.Charset = "";
                Response.Cache.SetCacheability(HttpCacheability.NoCache);
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.Write(model.Content);
                Response.End();
            }
            return View(model);
        }
        string buildHtmlGeneralCusPlan(List<CheckPlanViewModel> detailList)
        {
            string detailLists = "<table border=\"1\" class=\"invoice - detail\">";
            detailLists += "<thead>";
            detailLists += "	<tr>";
            detailLists += "		<th>NVQL</th>";
            detailLists += "		<th>Mã KH</th>";
            detailLists += "		<th>Tên KH</th>";
            detailLists += "		<th>Điện thoại</th>";
            detailLists += "        <th>Nhãn hàng</th>";
            detailLists += "        <th>TARGET KHBH</th>";
            detailLists += "        <th>KHBH</th>";
            detailLists += "        <th>Thực tế KHBH</th>";
            detailLists += "		<th>Tỷ lệ BH</th>";
            detailLists += "		<th>Người duyệt KHBH</th>";
            detailLists += "		<th>Ngày tạo KHBH</th>";
            detailLists += "		<th>Ngày cập nhật KHBH</th>";
            detailLists += "		<th>TARGET KHCDS</th>";
            detailLists += "		<th>KHCDS</th>";
            detailLists += "		<th>Thực tế KHCDS</th>";
            detailLists += "		<th>Tỷ lệ DS</th>";
            detailLists += "		<th>Kế hoạch TT</th>";
            detailLists += "	</tr>";
            detailLists += "</thead>";
            detailLists += "<tbody>";
            var index = 1;
            decimal totalBH = 0;
            decimal totalDS = 0;
            decimal totalTargetBH = 0;
            decimal thucteBH = 0;
            decimal totalTargetDS = 0;
            decimal thucteDS = 0;
            foreach (var item in detailList)
            {
                detailLists += "<tr>"
                + "<td>" + item.ManagerStaffName + "</td>"
                + "<td>" + item.Code + "</td>"
                + "<td>" + item.Name + "</td>"
                + "<td>" + item.Phone + "</td>"
                + "<td>" + item.CountForBrand + "</td>"
                + "<td>" + item.TARGET_BRAND + "</td>"
                + "<td>" + item.TotalBH + "</td>"
                + "<td>" + item.ThucTeBH + "</td>"
                + "<td>" + item.BHTL + "</td>"
                 + "<td>" + item.UserApproveName + "</td>"
                  + "<td>" + item.CreatedDate + "</td>"
                   + "<td>" + item.ModifiedDate + "</td>"
                + "<td>" + item.TARGETDS + "</td>"
                + "<td>" + item.TotalDS + "</td>"
                + "<td>" + item.ThucTeDS + "</td>"
                + "<td>" + item.DSTL + "</td>"
                + "<td>" + item.KHTT + "</td>"
                + "</tr>";
                totalBH += item.TotalBH ?? 0;
                totalDS += item.TotalDS ?? 0;
                totalTargetBH += item.TARGET_BRAND ?? 0;
                totalTargetDS += item.TARGETDS ?? 0;
                thucteBH += item.ThucTeBH ?? 0;
                thucteDS += item.ThucTeDS ?? 0;
            }
            detailLists += "<tr>\r\n"
              + "<td style=\"font-weight:bold \">Tổng cộng</td>"
              + "<td class=\"text-left \">" + "" + "</td>\r\n"
              + "<td class=\"text-left \">" + "" + "</td>\r\n"
              + "<td class=\"text-left \">" + "" + "</td>\r\n"
              + "<td class=\"text-left \">" + "" + "</td>\r\n"
              + "<td class=\"text-left \" style=\"font-weight:bold \">" + CommonSatic.ToCurrencyStr(totalTargetBH, null).Replace(".", ",") + "</td>\r\n"
              + "<td class=\"text-left \" style=\"font-weight:bold \">" + CommonSatic.ToCurrencyStr(totalBH, null).Replace(".", ",") + "</td>\r\n"
              + "<td class=\"text-left \" style=\"font-weight:bold \">" + CommonSatic.ToCurrencyStr(thucteBH, null).Replace(".", ",") + "</td>\r\n"
              + "<td class=\"text-left \">" + "" + "</td>\r\n"
              + "<td class=\"text-left \" style=\"font-weight:bold \">" + CommonSatic.ToCurrencyStr(totalTargetDS, null).Replace(".", ",") + "</td>\r\n"
              + "<td class=\"text-left \" style=\"font-weight:bold \">" + CommonSatic.ToCurrencyStr(totalDS, null).Replace(".", ",") + "</td>\r\n"
              + "<td class=\"text-left \" style=\"font-weight:bold \">" + CommonSatic.ToCurrencyStr(thucteDS, null).Replace(".", ",") + "</td>\r\n"
              + "<td class=\"text-left \">" + "" + "</td>\r\n"
              + "<td class=\"text-left \">" + "" + "</td>\r\n"

              + "</tr>\r\n";

            detailLists += "</tbody>\r\n";
            detailLists += "<tfoot style=\"font-weight:bold\">\r\n";
            detailLists += "</tfoot>\r\n</table>\r\n";
            return detailLists;
        }
        #endregion


        #region thống kê sl ca complain mun, nám, nhạy cảm, da đầu gàu....
        public ActionResult CountComplain(string startDate, string endDate, int? UserId)
        {
            UserId = UserId == null ? 0 : UserId;
            startDate = string.IsNullOrEmpty(startDate) ? "" : startDate;
            endDate = string.IsNullOrEmpty(endDate) ? "" : endDate;

            DateTime aDateTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);

            // Cộng thêm 1 tháng và trừ đi một ngày.
            DateTime retDateTime = aDateTime.AddMonths(1).AddDays(-1).AddHours(23).AddMinutes(59);

            var d_startDate = (!string.IsNullOrEmpty(startDate) == true ? DateTime.ParseExact(startDate, "dd/MM/yyyy", null).ToString("yyyy-MM-dd HH:mm:ss") : aDateTime.ToString("yyyy-MM-dd HH:mm:ss"));
            var d_endDate = (!string.IsNullOrEmpty(endDate) == true ? DateTime.ParseExact(endDate, "dd/MM/yyyy", null).AddHours(23).AddMinutes(59).ToString("yyyy-MM-dd HH:mm:ss") : retDateTime.ToString("yyyy-MM-dd HH:mm:ss"));



            //get cookie brachID ]\
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


            //getListComplain
            var model = SqlHelper.QuerySP<ComplainViewModel>("getListComplain", new
            {
                UserId = UserId,
                StartDate = d_startDate,
                EndDate = d_endDate,
                BranchId = intBrandID
            }).ToList();

            TempData["ListComplain"] = model;
            return View(model);
        }

        public ActionResult ExportListComplain()
        {
            var data = TempData["ListComplain"] as List<ComplainViewModel>;

            //Creating DataTable  
            DataTable dt = new DataTable();
            dt.TableName = "Thông kê số lượng complain";
            //Add Columns  
            dt.Columns.Add("COMPLAIN", typeof(string));
            dt.Columns.Add("SỐ LƯỢNG", typeof(int));
            dt.Columns.Add("MỨC ĐỘ", typeof(string));
            dt.Columns.Add("KẾT QUẢ", typeof(string));
            dt.Columns.Add("ĐỘ HÀI LÒNG", typeof(string));

            //Add Rows in DataTable  
            foreach (var item in data)
            {

                dt.Rows.Add(item.LOAI_TUONGTAC, item.TONG, item.MUCDO_TUONGTAC, item.KETQUA_SAUTUONGTAC, item.DOHAILONG);

            }

            string fileName = "ComplainList" + DateTime.Now + ".xlsx";
            using (XLWorkbook wb = new XLWorkbook())
            {
                //Add DataTable in worksheet  
                wb.Worksheets.Add(dt);
                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    //Return xlsx Excel File  
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
                }
            }
            // return View();
        }
        #endregion

        #region Danh sách khách complain 
        public ActionResult ListCusComplain(string startDate, string endDate, int? UserId, string KETQUA_SAUTUONGTAC)
        {
            //getListComplainCustomer
            UserId = UserId == null ? 0 : UserId;
            startDate = string.IsNullOrEmpty(startDate) ? "" : startDate;
            endDate = string.IsNullOrEmpty(endDate) ? "" : endDate;

            DateTime aDateTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);

            // Cộng thêm 1 tháng và trừ đi một ngày.
            DateTime retDateTime = aDateTime.AddMonths(1).AddDays(-1).AddHours(23).AddMinutes(59);

            var d_startDate = (!string.IsNullOrEmpty(startDate) == true ? DateTime.ParseExact(startDate, "dd/MM/yyyy", null).ToString("yyyy-MM-dd HH:mm:ss") : aDateTime.ToString("yyyy-MM-dd HH:mm:ss"));
            var d_endDate = (!string.IsNullOrEmpty(endDate) == true ? DateTime.ParseExact(endDate, "dd/MM/yyyy", null).AddHours(23).AddMinutes(59).ToString("yyyy-MM-dd HH:mm:ss") : retDateTime.ToString("yyyy-MM-dd HH:mm:ss"));



            //get cookie brachID ]\
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
            if (KETQUA_SAUTUONGTAC == null)
            {
                KETQUA_SAUTUONGTAC = "";
            }

            //getListComplain
            var model = SqlHelper.QuerySP<ComplainCusViewModel>("getListComplainCustomer", new
            {
                UserId = UserId,
                StartDate = d_startDate,
                EndDate = d_endDate,
                BranchId = intBrandID,
                KETQUA_SAUTUONGTAC = KETQUA_SAUTUONGTAC
            }).ToList();
            TempData["ListCusComplain"] = model;
            return View(model);
        }

        public ActionResult ExportListCusComplain()
        {
            var data = TempData["ListCusComplain"] as List<ComplainCusViewModel>;

            //Creating DataTable  
            DataTable dt = new DataTable();
            dt.TableName = "Thông kê số lượng KH complain";
            //Add Columns  
            dt.Columns.Add("NGÀY", typeof(string));
            dt.Columns.Add("KHÁCH HÀNG", typeof(string));
            dt.Columns.Add("MÃ SỐ", typeof(string));
            dt.Columns.Add("QUẢN LÝ", typeof(string));
            dt.Columns.Add("COMPLAIN", typeof(string));
            dt.Columns.Add("SỐ LƯỢNG", typeof(int));
            dt.Columns.Add("MỨC ĐỘ", typeof(string));
            dt.Columns.Add("KẾT QUẢ", typeof(string));
            dt.Columns.Add("ĐỘ HÀI LÒNG", typeof(string));

            //Add Rows in DataTable  
            foreach (var item in data)
            {

                dt.Rows.Add(item.NGAYLAP, item.CompanyName, item.Code, item.ManagerStaff, item.LOAI_TUONGTAC, item.TONG, item.MUCDO_TUONGTAC, item.KETQUA_SAUTUONGTAC, item.DOHAILONG);

            }

            string fileName = "ComplainCustomerList" + DateTime.Now + ".xlsx";
            using (XLWorkbook wb = new XLWorkbook())
            {
                //Add DataTable in worksheet  
                wb.Worksheets.Add(dt);
                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    //Return xlsx Excel File  
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
                }
            }
            // return View();
        }
        #endregion


        #region SO SÁNH CHỈ SỐ CÁC NHÓM
        public ActionResult CompareIndex(string startDate, string endDate, int? UserId, int? OldCustomer)
        {
            OldCustomer = OldCustomer == null ? 2 : OldCustomer;
            //getManagerIndex
            UserId = UserId == null ? 0 : UserId;
            startDate = string.IsNullOrEmpty(startDate) ? "" : startDate;
            endDate = string.IsNullOrEmpty(endDate) ? "" : endDate;

            DateTime aDateTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);

            // Cộng thêm 1 tháng và trừ đi một ngày.
            DateTime retDateTime = aDateTime.AddMonths(1).AddDays(-1).AddHours(23).AddMinutes(59);

            var d_startDate = (!string.IsNullOrEmpty(startDate) == true ? DateTime.ParseExact(startDate, "dd/MM/yyyy", null).ToString("yyyy-MM-dd HH:mm:ss") : aDateTime.ToString("yyyy-MM-dd HH:mm:ss"));
            var d_endDate = (!string.IsNullOrEmpty(endDate) == true ? DateTime.ParseExact(endDate, "dd/MM/yyyy", null).AddHours(23).AddMinutes(59).ToString("yyyy-MM-dd HH:mm:ss") : retDateTime.ToString("yyyy-MM-dd HH:mm:ss"));



            //get cookie brachID ]\
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

            if (intBrandID != null && intBrandID > 0)
            {

                user = user.Where(x => x.BranchId == intBrandID).ToList();

            }
            if (UserId > 0)
            {
                user = user.Where(x => x.Id == UserId).ToList();
            }
            //Lọc theo tk
            var u = userRepository.GetUserById(WebSecurity.CurrentUserId);
            //var dataNhanvien = SqlHelper.QuerySP<ManagerStaff>("spgetNhanvienbacap", new
            //{
            //    BranchId = (intBrandID == null ? Convert.ToInt32(Session["GlobalCurentBranchId"]) : intBrandID),
            //    UserId = WebSecurity.CurrentUserId,
            //}).ToList();

            //List<int> listNhanvien = new List<int>();


            //for (int i = 0; i < dataNhanvien.Count; i++)
            //{
            //    listNhanvien.Add(int.Parse(dataNhanvien[i].Id.ToString()));
            //}
            if (u.UserTypeId == 2026)
            {
                user = user.Where(x => x.Id == u.Id).ToList();
            }

            var listmodel = new List<IndexViewModel>();
            foreach (var item in user)
            {
                if (OldCustomer == 1)
                {
                    var model = SqlHelper.QuerySP<IndexViewModel>("getManagerOldIndex", new
                    {
                        ManagerId = item.Id,
                        StartDate = d_startDate,
                        EndDate = d_endDate,
                        BranchId = 0

                    }).FirstOrDefault();

                    model.NguoiLap = item.Id;
                    listmodel.Add(model);
                }
                else
                {
                    var model = SqlHelper.QuerySP<IndexViewModel>("getManagerIndex", new
                    {
                        ManagerId = item.Id,
                        StartDate = d_startDate,
                        EndDate = d_endDate,
                        BranchId = 0

                    }).FirstOrDefault();

                    model.NguoiLap = item.Id;
                    listmodel.Add(model);
                }

            }
            ViewBag.user = user;
            ViewBag.OldCustomer = OldCustomer;
            TempData["OldCustomer"] = OldCustomer.ToString();
            TempData["CompareIndex"] = listmodel;
            TempData["CompareIndexUser"] = user;
            return View(listmodel);
        }

        public ActionResult ExportCompareIndex()
        {
            var OldCustomer = TempData["OldCustomer"] as string;

            var data = TempData["CompareIndex"] as List<IndexViewModel>;

            var user = TempData["CompareIndexUser"] as List<UserViewModel>;
            //Creating DataTable  
            DataTable dt = new DataTable();
            DataRow dtRow;
            dt.TableName = "So sánh chỉ số NV";
            //Add Columns  
            dt.Columns.Add(" ", typeof(string));
            if (user != null)
            {
                foreach (var item in user)
                {
                    dt.Columns.Add(item.FullName, typeof(string));
                }
            }
            if (OldCustomer == "1")
            {
                //dong 1
                dtRow = dt.NewRow();
                dtRow[" "] = "SL TƯƠNG TÁC CŨ";
                if (user != null)
                {
                    foreach (var item in user)
                    {
                        var data2 = data.Where(x => x.NguoiLap == item.Id).FirstOrDefault();
                        dtRow[item.FullName] = data2.TT;
                    }
                }
                dt.Rows.Add(dtRow);

                //dòng 2
                dtRow = dt.NewRow();
                dtRow[" "] = "SỐ LƯỢNG KHÁCH CŨ HẸN";

                foreach (var item in user)
                {
                    var data2 = data.Where(x => x.NguoiLap == item.Id).FirstOrDefault();
                    dtRow[item.FullName] = data2.Hen;
                }
                dt.Rows.Add(dtRow);

                //dòng 3
                dtRow = dt.NewRow();
                dtRow[" "] = "SỐ LƯỢNG KHÁCH CŨ LÊN";

                foreach (var item in user)
                {
                    var data2 = data.Where(x => x.NguoiLap == item.Id).FirstOrDefault();
                    dtRow[item.FullName] = data2.Len;
                }
                dt.Rows.Add(dtRow);

                //dong 4
                dtRow = dt.NewRow();
                dtRow[" "] = "SỐ LƯỢNG KHÁCH CŨ MUA";

                foreach (var item in user)
                {
                    var data2 = data.Where(x => x.NguoiLap == item.Id).FirstOrDefault();
                    dtRow[item.FullName] = data2.Mua;
                }
                dt.Rows.Add(dtRow);



                //dòng 6
                dtRow = dt.NewRow();
                dtRow[" "] = "TỶ LỆ LÊN TRÊN HẸN";

                foreach (var item in user)
                {
                    var data2 = data.Where(x => x.NguoiLap == item.Id).FirstOrDefault();
                    dtRow[item.FullName] = data2.Mua;
                }
                dt.Rows.Add(dtRow);

                //dòng 7
                dtRow = dt.NewRow();
                dtRow[" "] = "TỶ LỆ MUA TRÊN LÊN CŨ";

                foreach (var item in user)
                {
                    var data2 = data.Where(x => x.NguoiLap == item.Id).FirstOrDefault();
                    dtRow[item.FullName] = data2.Mua;
                }
                dt.Rows.Add(dtRow);

                //dòng 8
                dtRow = dt.NewRow();
                dtRow[" "] = "DOANH SỐ ẢO CŨ";

                foreach (var item in user)
                {
                    var data2 = data.Where(x => x.NguoiLap == item.Id).FirstOrDefault();
                    dtRow[item.FullName] = Helpers.Common.PhanCachHangNgan2(data2.Ao);
                }

                dt.Rows.Add(dtRow);

                //dòng 8
                dtRow = dt.NewRow();
                dtRow[" "] = "DOANH SỐ THỰC CŨ";

                foreach (var item in user)
                {
                    var data2 = data.Where(x => x.NguoiLap == item.Id).FirstOrDefault();
                    dtRow[item.FullName] = Helpers.Common.PhanCachHangNgan2(data2.Thuc);
                }

                dt.Rows.Add(dtRow);

                //dòng 9
                dtRow = dt.NewRow();
                dtRow[" "] = "TB HĐ ẢO CŨ";

                foreach (var item in user)
                {
                    var data2 = data.Where(x => x.NguoiLap == item.Id).FirstOrDefault();
                    var Tb = data2.Ao / data2.Mua;
                    dtRow[item.FullName] = Helpers.Common.PhanCachHangNgan2((decimal?)Tb);
                }

                dt.Rows.Add(dtRow);

                //dòng 10
                dtRow = dt.NewRow();
                dtRow[" "] = "TB HĐ THỰC CŨ";

                foreach (var item in user)
                {
                    var data2 = data.Where(x => x.NguoiLap == item.Id).FirstOrDefault();
                    var Tb = data2.Thuc / data2.Mua;
                    dtRow[item.FullName] = Helpers.Common.PhanCachHangNgan2((decimal?)Tb);
                }

                dt.Rows.Add(dtRow);

                //dòng 11
                dtRow = dt.NewRow();
                dtRow[" "] = "TỔNG ORLANE ẢO";

                foreach (var item in user)
                {
                    var data2 = data.Where(x => x.NguoiLap == item.Id).FirstOrDefault();

                    dtRow[item.FullName] = Helpers.Common.PhanCachHangNgan2(data2.AoOrlane);
                }

                dt.Rows.Add(dtRow);

                //dòng 12
                dtRow = dt.NewRow();
                dtRow[" "] = "TỔNG ORLANE THỰC";

                foreach (var item in user)
                {
                    var data2 = data.Where(x => x.NguoiLap == item.Id).FirstOrDefault();

                    dtRow[item.FullName] = Helpers.Common.PhanCachHangNgan2(data2.ThucOrlane);
                }

                dt.Rows.Add(dtRow);

                //dòng 13
                dtRow = dt.NewRow();
                dtRow[" "] = "DS ẢO ANNAYAKE";

                foreach (var item in user)
                {
                    var data2 = data.Where(x => x.NguoiLap == item.Id).FirstOrDefault();

                    dtRow[item.FullName] = Helpers.Common.PhanCachHangNgan2(data2.AoAnna);
                }

                dt.Rows.Add(dtRow);

                //dòng 14
                dtRow = dt.NewRow();
                dtRow[" "] = "DS THỰC ANNAYAKE";

                foreach (var item in user)
                {
                    var data2 = data.Where(x => x.NguoiLap == item.Id).FirstOrDefault();

                    dtRow[item.FullName] = Helpers.Common.PhanCachHangNgan2(data2.ThucAnna);
                }

                dt.Rows.Add(dtRow);

                //dòng 15
                dtRow = dt.NewRow();
                dtRow[" "] = "DS ẢO LEONOR GREYL";

                foreach (var item in user)
                {
                    var data2 = data.Where(x => x.NguoiLap == item.Id).FirstOrDefault();

                    dtRow[item.FullName] = Helpers.Common.PhanCachHangNgan2(data2.AoLeo);
                }

                dt.Rows.Add(dtRow);
                //dòng 15
                dtRow = dt.NewRow();
                dtRow[" "] = "DS THỰC LEONOR GREYL";

                foreach (var item in user)
                {
                    var data2 = data.Where(x => x.NguoiLap == item.Id).FirstOrDefault();

                    dtRow[item.FullName] = Helpers.Common.PhanCachHangNgan2(data2.ThucLeo);
                }

                dt.Rows.Add(dtRow);
            }
            else
            {
                //dong 1
                dtRow = dt.NewRow();
                dtRow[" "] = "SL TƯƠNG TÁC MỚI";
                if (user != null)
                {
                    foreach (var item in user)
                    {
                        var data2 = data.Where(x => x.NguoiLap == item.Id).FirstOrDefault();
                        dtRow[item.FullName] = data2?.TT;
                    }
                }
                dt.Rows.Add(dtRow);

                //dòng 2
                dtRow = dt.NewRow();
                dtRow[" "] = "SỐ LƯỢNG KHÁCH MỚI HẸN";
                if (user != null)
                {
                    foreach (var item in user)
                    {
                        var data2 = data.Where(x => x.NguoiLap == item.Id).FirstOrDefault();
                        dtRow[item.FullName] = data2?.Hen;
                    }
                }
                dt.Rows.Add(dtRow);

                //dòng 3
                dtRow = dt.NewRow();
                dtRow[" "] = "SỐ LƯỢNG KHÁCH MỚI LÊN";
                if (user != null)
                {
                    foreach (var item in user)
                    {
                        var data2 = data.Where(x => x.NguoiLap == item.Id).FirstOrDefault();
                        dtRow[item.FullName] = data2?.Len;
                    }
                }
                dt.Rows.Add(dtRow);

                //dong 4
                dtRow = dt.NewRow();
                dtRow[" "] = "SỐ LƯỢNG KHÁCH MỚI MUA";
                if (user != null)
                {
                    foreach (var item in user)
                    {
                        var data2 = data.Where(x => x.NguoiLap == item.Id).FirstOrDefault();
                        dtRow[item.FullName] = data2?.Mua;
                    }
                }
                dt.Rows.Add(dtRow);



                //dòng 6
                dtRow = dt.NewRow();
                dtRow[" "] = "TỶ LỆ LÊN TRÊN HẸN";
                if (user != null)
                {
                    foreach (var item in user)
                    {
                        var data2 = data.Where(x => x.NguoiLap == item.Id).FirstOrDefault();
                        dtRow[item.FullName] = data2?.Mua;
                    }
                }
                dt.Rows.Add(dtRow);

                //dòng 7
                dtRow = dt.NewRow();
                dtRow[" "] = "TỶ LỆ MUA TRÊN LÊN MỚI";
                if (user != null)
                {
                    foreach (var item in user)
                    {
                        var data2 = data.Where(x => x.NguoiLap == item.Id).FirstOrDefault();
                        dtRow[item.FullName] = data2?.Mua;
                    }
                }
                dt.Rows.Add(dtRow);

                //dòng 8
                dtRow = dt.NewRow();
                dtRow[" "] = "DOANH SỐ ẢO MỚI";
                if (user != null)
                {
                    foreach (var item in user)
                    {
                        var data2 = data.Where(x => x.NguoiLap == item.Id).FirstOrDefault();
                        dtRow[item.FullName] = Helpers.Common.PhanCachHangNgan2(data2?.Ao);
                    }
                }

                dt.Rows.Add(dtRow);

                //dòng 8
                dtRow = dt.NewRow();
                dtRow[" "] = "DOANH SỐ THỰC MỚI";
                if (user != null)
                {
                    foreach (var item in user)
                    {
                        var data2 = data.Where(x => x.NguoiLap == item.Id).FirstOrDefault();
                        dtRow[item.FullName] = Helpers.Common.PhanCachHangNgan2(data2?.Thuc);
                    }
                }
                dt.Rows.Add(dtRow);

                //dòng 9
                dtRow = dt.NewRow();
                dtRow[" "] = "TB HĐ ẢO MỚI";
                if (user != null)
                {
                    foreach (var item in user)
                    {
                        var data2 = data.Where(x => x.NguoiLap == item.Id).FirstOrDefault();
                        var Tb = data2?.Ao / data2?.Mua;
                        dtRow[item.FullName] = Helpers.Common.PhanCachHangNgan2((decimal?)Tb);
                    }
                }
                dt.Rows.Add(dtRow);

                //dòng 10
                dtRow = dt.NewRow();
                dtRow[" "] = "TB HĐ THỰC MỚI";
                if (user != null)
                {
                    foreach (var item in user)
                    {
                        var data2 = data.Where(x => x.NguoiLap == item.Id).FirstOrDefault();
                        var Tb = data2?.Thuc / data2?.Mua;
                        dtRow[item.FullName] = Helpers.Common.PhanCachHangNgan2((decimal?)Tb);
                    }
                }

                dt.Rows.Add(dtRow);
            }

            string fileName = "SoSanhChiso" + DateTime.Now + ".xlsx";
            using (XLWorkbook wb = new XLWorkbook())
            {
                //Add DataTable in worksheet  
                wb.Worksheets.Add(dt);
                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    //Return xlsx Excel File  
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
                }
            }
        }

        [AllowAnonymous]
        public ActionResult BieuDoChiSoTuan(string startDate, string endDate, int? UserId, int? OldCustomer)
        {
            OldCustomer = OldCustomer == null ? 2 : OldCustomer;
            //getManagerIndex
            UserId = UserId == null ? 0 : UserId;
            startDate = string.IsNullOrEmpty(startDate) ? "" : startDate;
            endDate = string.IsNullOrEmpty(endDate) ? "" : endDate;

            DateTime aDateTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);

            // Cộng thêm 1 tháng và trừ đi một ngày.
            DateTime retDateTime = aDateTime.AddMonths(1).AddDays(-1).AddHours(23).AddMinutes(59);

            var d_startDate = (!string.IsNullOrEmpty(startDate) == true ? DateTime.ParseExact(startDate, "dd/MM/yyyy", null).ToString("yyyy-MM-dd HH:mm:ss") : aDateTime.ToString("yyyy-MM-dd HH:mm:ss"));
            var d_endDate = (!string.IsNullOrEmpty(endDate) == true ? DateTime.ParseExact(endDate, "dd/MM/yyyy", null).AddHours(23).AddMinutes(59).ToString("yyyy-MM-dd HH:mm:ss") : retDateTime.ToString("yyyy-MM-dd HH:mm:ss"));



            //get cookie brachID ]\
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

            if (intBrandID != null && intBrandID > 0)
            {

                user = user.Where(x => x.BranchId == intBrandID).ToList();

            }
            if (UserId > 0)
            {
                user = user.Where(x => x.Id == UserId).ToList();
            }

            var listmodel = new List<IndexViewModel>();
            foreach (var item in user)
            {
                if (OldCustomer == 1)
                {
                    var model = SqlHelper.QuerySP<IndexViewModel>("getManagerOldIndex", new
                    {
                        ManagerId = item.Id,
                        StartDate = d_startDate,
                        EndDate = d_endDate,
                        BranchId = 0

                    }).FirstOrDefault();

                    model.AoFloat = model.Ao != null ? (double)model.Ao / 1000000 : 0;
                    model.ThucFloat = model.Thuc != null ? (double)model.Thuc / 1000000 : 0;
                    model.NguoiLap = item.Id;
                    model.TenNguoiLap = item.FullName;
                    listmodel.Add(model);
                }
                else
                {
                    var model = SqlHelper.QuerySP<IndexViewModel>("getManagerIndex", new
                    {
                        ManagerId = item.Id,
                        StartDate = d_startDate,
                        EndDate = d_endDate,
                        BranchId = 0

                    }).FirstOrDefault();
                    model.AoFloat = model.Ao != null ? (double)model.Ao / 1000000 : 0;
                    model.ThucFloat = model.Thuc != null ? (double)model.Thuc / 1000000 : 0;
                    model.NguoiLap = item.Id;
                    model.TenNguoiLap = item.FullName;
                    listmodel.Add(model);
                }

            }
            ViewBag.Listmodel = JsonConvert.SerializeObject(listmodel);

            return View();
        }

        public ActionResult CompareIndexBranch(string startDate, string endDate)
        {

            startDate = string.IsNullOrEmpty(startDate) ? "" : startDate;
            endDate = string.IsNullOrEmpty(endDate) ? "" : endDate;

            DateTime aDateTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);

            // Cộng thêm 1 tháng và trừ đi một ngày.
            DateTime retDateTime = aDateTime.AddMonths(1).AddDays(-1).AddHours(23).AddMinutes(59);

            var d_startDate = (!string.IsNullOrEmpty(startDate) == true ? DateTime.ParseExact(startDate, "dd/MM/yyyy", null).ToString("yyyy-MM-dd HH:mm:ss") : aDateTime.ToString("yyyy-MM-dd HH:mm:ss"));
            var d_endDate = (!string.IsNullOrEmpty(endDate) == true ? DateTime.ParseExact(endDate, "dd/MM/yyyy", null).AddHours(23).AddMinutes(59).ToString("yyyy-MM-dd HH:mm:ss") : retDateTime.ToString("yyyy-MM-dd HH:mm:ss"));



            //get cookie brachID ]\
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

            if (intBrandID != null && intBrandID > 0)
            {

                user = user.Where(x => x.BranchId == intBrandID).ToList();

            }

            TempData["CompareIndexUser"] = user;

            var branchList = BranchRepository.GetAllBranch().Select(x => new BranchViewModel
            {

                Id = x.Id,
                Name = x.Name
            }).ToList();

            if (intBrandID > 0)
            {
                branchList = branchList.Where(x => x.Id == intBrandID).ToList();
            }
            var listnewmodel = new List<IndexViewModel>();
            var listoldmodel = new List<IndexViewModel>();

            foreach (var item in branchList)
            {
                var model = SqlHelper.QuerySP<IndexViewModel>("getManagerOldIndex", new
                {
                    ManagerId = 0,
                    StartDate = d_startDate,
                    EndDate = d_endDate,
                    BranchId = item.Id

                }).FirstOrDefault();
                listoldmodel.Add(model);

                var model2 = SqlHelper.QuerySP<IndexViewModel>("getManagerIndex", new
                {
                    ManagerId = 0,
                    StartDate = d_startDate,
                    EndDate = d_endDate,
                    BranchId = item.Id

                }).FirstOrDefault();
                listnewmodel.Add(model2);
            }

            ViewBag.Branch = branchList;
            ViewBag.oldmodel = listoldmodel;
            TempData["CompareIndex"] = listnewmodel;
            return View(listnewmodel);
        }
        #endregion
    }
}
