using System.Globalization;
using Erp.BackOffice.Sale.Models;
using Erp.BackOffice.Filters;
using Erp.Domain.Entities;
using Erp.Domain.Interfaces;
using Erp.Domain.Sale.Entities;
using Erp.Domain.Sale.Interfaces;
using Erp.Domain.Account.Interfaces;
using Erp.Domain.Account.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Erp.Utilities;
using WebMatrix.WebData;
using Erp.BackOffice.Helpers;
using System.Transactions;
using PagedList;
using PagedList.Mvc;
using Erp.Domain.Staff.Interfaces;
using Erp.BackOffice.Account.Controllers;
using Erp.BackOffice.Account.Models;
using System.Web;
using Erp.Domain.Helper;
using System.IO;
using System.Web.UI;

namespace Erp.BackOffice.Sale.Controllers
{
    [Authorize]
    [InitializeSimpleMembership]
    [Erp.BackOffice.Helpers.NoCacheHelper]
    public class MembershipController : Controller
    {
        private readonly IMembershipRepository MembershipRepository;
        private readonly IMembershipThugonRepository MembershipThugonRepository;
        private readonly IMembership_parentRepository Membership_parentRepository;
        private readonly IUserRepository userRepository;
        private readonly IProductInvoiceRepository productInvoiceRepository;
        private readonly ICustomerRepository customerRepository;
        private readonly ITemplatePrintRepository templatePrintRepository;
        private readonly IInquiryCardRepository InquiryCardRepository;
        private readonly ISchedulingHistoryRepository SchedulingHistoryRepository;
        private readonly IBedRepository bedRepository;
        private readonly ILogEquipmentRepository logEquipmentRepository;
        private readonly IStaffEquipmentRepository StaffEquipmentRepository;
        private readonly IStaffEquipmentRepository staffEquipmentRepository;
        private readonly IServiceDetailRepository serviceDetailRepository;
        private readonly IInventoryRepository InventoryRepository;
        private readonly IWarehouseRepository WarehouseRepository;
        private readonly IProductInboundRepository ProductInboundRepository;
        private readonly IWarehouseRepository warehouseRepository;
        private readonly IMaterialInboundRepository materialInboundRepository;
        private readonly IProductDetailRepository productDetailRepository;
        private readonly IProductOrServiceRepository productOrServiceRepository;
        public MembershipController(
            IMembershipRepository _Membership
            , IUserRepository _user
            , IProductInvoiceRepository productInvoice
            , IMembershipThugonRepository _MembershipThugon
            , IMembership_parentRepository _Membership_parent
            , ICustomerRepository _Customer
            , ITemplatePrintRepository _templatePrint
            , IInquiryCardRepository _InquiryCard
            , ISchedulingHistoryRepository _SchedulingHistory
             , IBedRepository bed
            , ILogEquipmentRepository logEquipment
            , IStaffEquipmentRepository staffEquipment
             , IServiceDetailRepository serviceDetail
            , IInventoryRepository _Inventory
            , IWarehouseRepository _Warehouse
            , IProductInboundRepository _ProductInbound
            , IWarehouseRepository warehouse
            , IMaterialInboundRepository _MaterialOutbound
            , IProductDetailRepository productDetail
            , IProductOrServiceRepository productOrService
            )
        {
            MembershipRepository = _Membership;
            MembershipThugonRepository = _MembershipThugon;
            Membership_parentRepository = _Membership_parent;
            userRepository = _user;
            productInvoiceRepository = productInvoice;
            customerRepository = _Customer;
            templatePrintRepository = _templatePrint;
            InquiryCardRepository = _InquiryCard;
            SchedulingHistoryRepository = _SchedulingHistory;
            bedRepository = bed;
            logEquipmentRepository = logEquipment;
            staffEquipmentRepository = staffEquipment;
            serviceDetailRepository = serviceDetail;
            InventoryRepository = _Inventory;
            WarehouseRepository = _Warehouse;
            ProductInboundRepository = _ProductInbound;
            warehouseRepository = warehouse;
            materialInboundRepository = _MaterialOutbound;
            productDetailRepository = productDetail;
            productOrServiceRepository = productOrService;
        }

        #region Index

        public ViewResult Index(int? page, string startDate, string endDate, string txtCode, string txtCusCode, int? BranchId, int? ManagerId, string productCode, string TargetCode, int? PhieuConTu, int? PhieuConDen)
        {
            page = page == null ? 1 : page;

            txtCode = txtCode != null ? txtCode : "";
            txtCusCode = txtCusCode != null ? txtCusCode : "";
            ManagerId = ManagerId != null ? ManagerId : 0;
            TargetCode = TargetCode != null ? TargetCode : "";
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
            //updatesudung();
            //updatetralai();
            if (startDate == null && endDate == null)
            {

                DateTime aDateTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                // Cộng thêm 1 tháng và trừ đi một ngày.
                DateTime retDateTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(1).AddDays(-1);



                startDate = aDateTime.ToString("dd/MM/yyyy");
                endDate = retDateTime.ToString("dd/MM/yyyy");
            }


            DateTime d_startDate, d_endDate;
            if (DateTime.TryParseExact(startDate, "dd/MM/yyyy", new CultureInfo("vi-VN"), DateTimeStyles.None, out d_startDate))
            {

            }
            if (DateTime.TryParseExact(endDate, "dd/MM/yyyy", new CultureInfo("vi-VN"), DateTimeStyles.None, out d_endDate))
            {
                d_endDate = d_endDate.AddHours(23).AddMinutes(59);
            }

            if ((d_startDate == null) || (d_endDate == null))
            {
                return View();
            }


            var count = SqlHelper.QuerySP<Membership_parentViewModel>("getMembershipParent", new
            {
                txtCusCode = txtCusCode,
                StartDate = d_startDate,
                EndDate = d_endDate,
                @BranchId = intBrandID,
                @skip = 0,
                @txtCode = txtCode,
                @ManagerId = ManagerId,
                @iscount = 1,
                @TargetCode = TargetCode,
                @txtproCode = productCode
            }).FirstOrDefault();

            //lấy số trang
            var AmountPage = count.Total / 30;
            var du = count.Total % 30;
            if (du > 0)
            {
                AmountPage = AmountPage + 1;
            }
            ViewBag.TongTrang = AmountPage;
            int skip = ((int)page - 1) * 30;
            //q = q.OrderByDescending(m => m.CreatedDate).Skip(skip).Take(30);

            var model = SqlHelper.QuerySP<Membership_parentViewModel>("getMembershipParent", new
            {
                txtCusCode = txtCusCode,
                StartDate = d_startDate,
                EndDate = d_endDate,
                @BranchId = intBrandID,
                @skip = skip,
                @txtCode = txtCode,
                @ManagerId = ManagerId,
                @iscount = 0,
                @TargetCode = TargetCode,
                @txtproCode = productCode
            }).AsEnumerable();

            try
            {
                Session["txtCusCode"] = txtCusCode;
                Session["d_startDate"] = d_startDate;
                Session["d_endDate"] = d_endDate;
                Session["skip"] = skip;
                Session["txtCode"] = txtCode;
                Session["ManagerId"] = ManagerId;
                Session["TargetCode"] = TargetCode;
                Session["intBrandID"] = intBrandID;
            }
            catch (Exception)
            {

            }


            

            //IEnumerable<Membership_parentViewModel> q1 = q.Select(x => new Membership_parentViewModel
            //{
            //    Id = x.Id,
            //    //CustomerId = x.CustomerId,
            //    Code = x.Code,
            //    CustomerId = x.CustomerId,
            //    CustomerCode = x.CustomerCode,
            //    CustomerName = x.CustomerName,
            //    CreatedDate = x.CreatedDate,
            //    CreatedUserId = x.CreatedUserId,
            //    ProductInvoiceId = x.ProductInvoiceId,
            //    Status = x.Status,
            //    QRCode = x.QRCode,
            //    Note = x.Note,
            //    SerialNumber = x.SerialNumber,
            //    ExpiryDate = x.ExpiryDate,
            //    BranchId = x.BranchId,
            //    isPrint = x.isPrint,
            //    NumberPrint = x.NumberPrint,
            //    is_extend = x.is_extend,
            //    ProductName = x.ProductName,
            //    Total = x.Total,
            //    solandadung = x.solandadung,
            //    solanconlai = (x.Total - x.solandadung),
            //    ManagerId = x.ManagerId,
            //    ManagerName = x.ManagerName,
            //    ChiefUserFullName = x.ChiefUserFullName,
            //    TargetCode = x.TargetCode,
            //    UserTypeName_kd = x.UserTypeName_kd,
            //    tienconno = x.tienconno,
            //    ProductInvoiceId_Return = x.ProductInvoiceId_Return

            //}).OrderByDescending(m => m.CreatedDate);


            //if (CreateUserId != null && CreateUserId.Value > 0)
            //{
            //   
            //}

            //if (PhieuConTu != null)
            //{
            //    if (PhieuConDen != null)
            //    {
            //        q = q.Where(x => x.solanconlai >= PhieuConTu && x.solanconlai <= PhieuConDen);
            //    }
            //}

            ViewBag.SuccessMessage = TempData["SuccessMessage"];
            ViewBag.FailedMessage = TempData["FailedMessage"];
            ViewBag.AlertMessage = TempData["AlertMessage"];


            return View(model);
        }

        string buildHtml(List<Membership_parentViewModel> data)
        {
            //Tạo table html chi tiết phiếu xuất
            string detailLists = "<table border=\"1\" class=\"invoice-detail\">\r\n";
            detailLists += "<thead>\r\n";
            detailLists += "	<tr>\r\n";
            detailLists += "		<th>STT</th>\r\n";
            detailLists += "		<th>Mã phiếu</th>\r\n";
            detailLists += "		<th>Ngày lập phiếu</th>\r\n";
            detailLists += "		<th>Tên khách hàng</ th>\r\n";
            detailLists += "		<th>Mã khách hàng</th>\r\n";
            detailLists += "		<th>Hàng</th>\r\n";
            detailLists += "		<th>Số lần CSD</th>\r\n";
            detailLists += "		<th>Số lần đã dùng</ th>\r\n";
            detailLists += "		<th>Số lần còn lại</th>\r\n";
            detailLists += "		<th>Nhân viên quản lý</th>\r\n";
            detailLists += "		<th>Nhóm</th>\r\n";
            detailLists += "		<th>Đơn hàng</th>\r\n";
            detailLists += "	</tr>\r\n";
            detailLists += "</thead>\r\n";
            detailLists += "<tbody>\r\n";
            var index = 1;

            foreach (var item in data)
            {
                detailLists += "<tr>\r\n"
                + "<td class=\"text-center orderNo\">" + (index++) + "</td>\r\n"
                + "<td class=\"text-left \">" + item.Code + "</td>\r\n"
                + "<td class=\"text-left \">" + item.CreatedDate + "</td>\r\n"
                + "<td class=\"text-left\">" + item.CustomerName + "</td>\r\n"
                + "<td class=\"text-left \">" + item.CustomerCode + "</td>\r\n"
                + "<td class=\"text-left \">" + item.ProductCode + "-" + item.ProductName + "</td>\r\n"
                + "<td class=\"text-left \">" + item.Total + "</td>\r\n"
                + "<td class=\"text-left \">" + item.solandadung + "</td>\r\n"
                + "<td class=\"text-left \">" + item.solanconlai + "</td>\r\n"
                + "<td class=\"text-left \">" + item.ManagerName + "</td>\r\n"
                + "<td class=\"text-left \">" + item.UserTypeName_kd + "</td>\r\n"
                + "<td class=\"text-left \">" + item.TargetCode + "</td>\r\n"
                + "</tr>\r\n";
            }
            detailLists += "</tbody>\r\n";
            detailLists += "<tfoot style=\"font-weight:bold\">\r\n";
            detailLists += "</tfoot>\r\n</table>\r\n";

            return detailLists;
        }

        [AllowAnonymous]
        public ActionResult ExportExcel()
        {

            DateTime d_startDate, d_endDate;
            if (DateTime.TryParseExact(Session["d_startDate"].ToString().Substring(0,10), "dd/MM/yyyy", new CultureInfo("vi-VN"), DateTimeStyles.None, out d_startDate))
            {

            }
            if (DateTime.TryParseExact(Session["d_endDate"].ToString().Substring(0, 10), "dd/MM/yyyy", new CultureInfo("vi-VN"), DateTimeStyles.None, out d_endDate))
            {
                d_endDate = d_endDate.AddHours(23).AddMinutes(59);
            }
            int intBrandID = int.Parse(Session["intBrandID"].ToString());
            int skip = int.Parse(Session["skip"].ToString());
            var data = SqlHelper.QuerySP<Membership_parentViewModel>("getMembershipParentexcel", new
            {
                txtCusCode = Session["txtCusCode"].ToString(),
                StartDate = d_startDate,
                EndDate = d_endDate,
                @BranchId = intBrandID,
                @skip = skip,
                @txtCode = "",
                @ManagerId = "",
                @iscount = 1,
                @TargetCode = "",
                @txtproCode = ""
            }).ToList();

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
            model.Content = model.Content.Replace("{DataTable}", buildHtml(data));
            model.Content = model.Content.Replace("{System.Logo}", ImgLogo);
            model.Content = model.Content.Replace("{System.CompanyName}", company);
            model.Content = model.Content.Replace("{System.AddressCompany}", address);
            model.Content = model.Content.Replace("{System.PhoneCompany}", phone);
            model.Content = model.Content.Replace("{System.Fax}", fax);
            model.Content = model.Content.Replace("{System.BankCodeCompany}", bankcode);
            model.Content = model.Content.Replace("{System.BankNameCompany}", bankname);
            model.Content = model.Content.Replace("{PrintDate}", DateTime.Now.ToString("dd/MM/yyyy HH:mm"));
            model.Content = model.Content.Replace("{Title}", "Danh sách phiếu MBS");

            Response.AppendHeader("content-disposition", "attachment;filename=" + "DS_Phieumbs" + DateTime.Now.ToString("yyyyMMdd") + ".xls");
            Response.Charset = "";
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.ContentEncoding = System.Text.Encoding.UTF8;
            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);
            htw.WriteLine("<meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\">");
            Response.ContentType = "application/vnd.ms-excel";
            Response.Output.Write(sw.ToString());
            Response.Flush();

            Response.Write(model.Content);
            Response.End();

            return View();
        }
        public ViewResult MembershipIndex(Int64 IdParent, int ProductInvoiceId)
        {
            IEnumerable<MembershipViewModel> q = MembershipRepository.GetAllvwMembershipByIdParent(IdParent)
         .Select(item => new MembershipViewModel
         {
             Id = item.Id,
             CreatedUserId = item.CreatedUserId,
             //CreatedUserName = item.CreatedUserName,
             CreatedDate = item.CreatedDate,
             ModifiedUserId = item.ModifiedUserId,
             //ModifiedUserName = item.ModifiedUserName,
             ModifiedDate = item.ModifiedDate,
             AssignedUserId = item.AssignedUserId,
             BranchCode = item.BranchCode,
             BranchId = item.BranchId,
             BranchName = item.BranchName,
             Code = item.Code,
             CreatedUserCode = item.CreatedUserCode,
             CreatedUserName = item.CreatedUserName,
             CustomerCode = item.CustomerCode,
             CustomerId = item.CustomerId,
             CustomerName = item.CustomerName,
             ExpiryDate = item.ExpiryDate,
             ManagerName = item.ManagerName,
             ProductCode = item.ProductCode,
             ProductId = item.ProductId,
             ProductName = item.ProductName,
             Status = item.Status,
             TargetCode = item.TargetCode,
             TargetId = item.TargetId,
             TargetModule = item.TargetModule,
             Type = item.Type,
             CustomerImage = item.CustomerImage
         }).OrderByDescending(m => m.CreatedDate).ToList();
            //đếm số lần hoàn thành
            int dem = 0;
            //gán
            var k = q.ToList();
            foreach (var item in k.ToList())
            {
                if (item.Status == "complete")
                {
                    dem++;
                    k.Remove(item);
                    k.Add(item);

                }
            }

            //MCUONG.FIT--- 
            //lấy hóa đơn
            var tienno = productInvoiceRepository.GetAllvwProductInvoiceFull().Where(x => x.Id == ProductInvoiceId).FirstOrDefault();

            if (tienno != null)
            {
                var ishddetail = productInvoiceRepository.GetListAllInvoiceDetailsByInvoiceId(ProductInvoiceId);
                //lấy tổng tiền
                decimal sumT = 0;     //=  Helpers.Common.NVL_NUM_LONG_NEW(tienno.tienconno) + Helpers.Common.NVL_NUM_LONG_NEW(tienno.tiendathu);
                foreach (var item in ishddetail)
                {
                    sumT += item.Price.Value * item.Quantity.Value;
                }
                //lấy đơn giáì
                decimal dongia;
                if (k.Count() != 0)
                {

                    dongia = sumT / k.Count();
                }
                else
                    dongia = sumT;
                //tao biến cho phép sử dụng tiếp phiếp
                bool chophep = false;
                if (dem == 0)
                {
                    if (tienno.tiendathu >= dongia)
                    {
                        chophep = true;
                    }
                }
                else
                {
                    if ((dongia * dem) < tienno.tiendathu && tienno.tiendathu - (dongia * dem) >= dongia)
                    {
                        chophep = true;
                    }
                }


                if (tienno.Status == "delete")
                {
                    chophep = false;
                }
                //TH phiếu đang sử dụng
                var conlai = k.Where(x => x.Status == "pending").ToList();

                var tiencosudung = tienno.tiendathu - (k.Count() - conlai.Count()) * dongia;
                if (tiencosudung < dongia)
                {
                    chophep = false;
                }
                if (tienno.Status == "complete" && tienno.IsArchive == true)
                {
                    chophep = true;
                }

                ViewBag.choPhep = chophep;
                ViewBag.tienconno = tienno.tienconno;
            }
            // TH hoàn phiếu dc ng.khac chuyển
            if (ProductInvoiceId == 0)
            {
                ViewBag.choPhep = true;
            }



            ViewBag.SuccessMessage = TempData["SuccessMessage"];
            ViewBag.FailedMessage = TempData["FailedMessage"];
            ViewBag.AlertMessage = TempData["AlertMessage"];


            return View(k);

        }

        //public ViewResult IndexOld(int? page, string startDate, string endDate, string txtCode, string txtCusCode,
        //    string txtCusName, string Status, int? BranchId, int? ManagerId, int? CreateUserId, string type, string productCode, string TargetCode, string search)
        //{
        //    if (search == "Search")
        //    {
        //        IEnumerable<MembershipViewModel> q = MembershipRepository.GetvwAllMembership()
        //     .Select(item => new MembershipViewModel
        //     {
        //         Id = item.Id,
        //         CreatedUserId = item.CreatedUserId,
        //         //CreatedUserName = item.CreatedUserName,
        //         CreatedDate = item.CreatedDate,
        //         ModifiedUserId = item.ModifiedUserId,
        //         //ModifiedUserName = item.ModifiedUserName,
        //         ModifiedDate = item.ModifiedDate,
        //         AssignedUserId = item.AssignedUserId,
        //         BranchCode = item.BranchCode,
        //         BranchId = item.BranchId,
        //         BranchName = item.BranchName,
        //         Code = item.Code,
        //         CreatedUserCode = item.CreatedUserCode,
        //         CreatedUserName = item.CreatedUserName,
        //         CustomerCode = item.CustomerCode,
        //         CustomerId = item.CustomerId,
        //         CustomerName = item.CustomerName,
        //         ExpiryDate = item.ExpiryDate,
        //         ManagerName = item.ManagerName,
        //         ProductCode = item.ProductCode,
        //         ProductId = item.ProductId,
        //         ProductName = item.ProductName,
        //         Status = item.Status,
        //         TargetCode = item.TargetCode,
        //         TargetId = item.TargetId,
        //         TargetModule = item.TargetModule,
        //         Type = item.Type,
        //         CustomerImage = item.CustomerImage
        //     }).OrderByDescending(m => m.CreatedDate);


        //        IEnumerable<MembershipThuGonViewModel> k = MembershipThugonRepository.GetvwAllMembershipThugon()
        //       .Select(item => new MembershipThuGonViewModel
        //       {
        //           Id = item.Id,
        //           CustomerName = item.CustomerName,
        //           CustomerCode = item.CustomerCode,
        //           TargetId = item.TargetId,
        //           Type = item.Type,
        //           CreatedDate = item.CreatedDate,
        //           ProductInvoiceCode = item.ProductInvoiceCode,
        //           ProductName = item.ProductName,
        //           ManagerName = item.ManagerName,
        //           SOLUONG = item.SOLUONG,
        //           Quantity = item.Quantity,
        //           BranchId = item.BranchId,
        //           CreatedUserId = item.CreatedUserId,
        //           soluongconlai = item.soluongconlai,
        //           soluongdung = item.soluongdung,
        //           ProductInvoiceId = item.ProductInvoiceId,
        //           soluongtra = item.soluongtra,
        //           soluongchuyen = item.soluongchuyen
        //       }).OrderByDescending(m => m.CreatedDate);


        //        if (!string.IsNullOrEmpty(txtCode))
        //        {
        //            txtCode = Helpers.Common.ChuyenThanhKhongDau(txtCode);
        //            q = q.Where(x => Helpers.Common.ChuyenThanhKhongDau(x.Code).Contains(txtCode)).ToList();
        //        }
        //        if (!string.IsNullOrEmpty(TargetCode))
        //        {
        //            TargetCode = Helpers.Common.ChuyenThanhKhongDau(TargetCode);
        //            q = q.Where(x => Helpers.Common.ChuyenThanhKhongDau(x.TargetCode).Contains(TargetCode)).ToList();
        //            k = k.Where(x => Helpers.Common.ChuyenThanhKhongDau(x.ProductInvoiceCode).Contains(TargetCode)).ToList();
        //        }
        //        //ViewBag.Thugon = k;

        //        if (!string.IsNullOrEmpty(productCode))
        //        {
        //            productCode = Helpers.Common.ChuyenThanhKhongDau(productCode);
        //            q = q.Where(x => Helpers.Common.ChuyenThanhKhongDau(x.ProductCode).Contains(productCode)).ToList();
        //        }
        //        if (!string.IsNullOrEmpty(txtCusCode))
        //        {
        //            txtCusCode = Helpers.Common.ChuyenThanhKhongDau(txtCusCode);
        //            q = q.Where(x => Helpers.Common.ChuyenThanhKhongDau(x.CustomerCode).Contains(txtCusCode)).ToList();
        //        }
        //        if (!string.IsNullOrEmpty(type))
        //        {
        //            q = q.Where(x => x.Type == type).ToList();
        //        }
        //        if (!string.IsNullOrEmpty(txtCusName))
        //        {
        //            txtCusName = Helpers.Common.ChuyenThanhKhongDau(txtCusName);
        //            q = q.Where(x => Helpers.Common.ChuyenThanhKhongDau(x.CustomerName).Contains(txtCusName)).ToList();
        //        }
        //        DateTime d_startDate, d_endDate;
        //        if (DateTime.TryParseExact(startDate, "dd/MM/yyyy", new CultureInfo("vi-VN"), DateTimeStyles.None, out d_startDate))
        //        {
        //            if (DateTime.TryParseExact(endDate, "dd/MM/yyyy", new CultureInfo("vi-VN"), DateTimeStyles.None, out d_endDate))
        //            {
        //                d_endDate = d_endDate.AddHours(23).AddMinutes(59);
        //                q = q.Where(x => x.CreatedDate >= d_startDate && x.CreatedDate <= d_endDate).ToList();
        //            }
        //        }
        //        if (BranchId != null && BranchId.Value > 0)
        //        {
        //            q = q.Where(x => x.BranchId == BranchId).ToList();
        //        }
        //        if (ManagerId != null && ManagerId.Value > 0)
        //        {
        //            q = q.Where(x => x.ManagerId == ManagerId).ToList();
        //        }
        //        if (CreateUserId != null && CreateUserId.Value > 0)
        //        {l
        //            q = q.Where(x => x.CreatedUserId == CreateUserId).ToList();
        //        }
        //        if (!string.IsNullOrEmpty(Status))
        //        {
        //            q = q.Where(x => x.Status == Status).ToList();
        //        }
        //        if (Request["search"] == null)
        //        {
        //            q = q.Where(x => x.Status == "pending" && x.BranchId == Erp.BackOffice.Helpers.Common.CurrentUser.BranchId).ToList();
        //        }
        //        int pageSize = 20;
        //        int pageNumber = (page ?? 1);
        //        ViewBag.SuccessMessage = TempData["SuccessMessage"];
        //        ViewBag.FailedMessage = TempData["FailedMessage"];
        //        ViewBag.AlertMessage = TempData["AlertMessage"];
        //        ViewBag.Thugon = k.ToPagedList(pageNumber, pageSize);
        //        return View(q.ToPagedList(pageNumber, pageSize));
        //    }
        //    return View();

        //}
        #endregion

        #region Create
        public ViewResult Create(int? TargetId, string TargetModule)
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

            var model = new MembershipViewModel();
            model.Code = Erp.BackOffice.Helpers.Common.GetOrderNo("Membership");
            model.BranchId = intBrandID;
            model.TargetId = TargetId;
            model.TargetModule = TargetModule;
            if (TargetId != null)
            {
                if (model.TargetModule == "ProductInvoiceDetail")
                {
                    var info = productInvoiceRepository.GetvwProductInvoiceDetailById(model.TargetId.Value);
                    model.TargetCode = info.ProductInvoiceCode;
                    model.Type = info.Type;
                    model.ProductId = info.ProductId;
                    model.CustomerId = info.CustomerId;
                    model.ExpiryDate = info.ExpiryDate;
                }
                if (model.TargetModule == "Membership")
                {
                    var info = MembershipRepository.GetvwMembershipById(model.TargetId.Value);
                    model.TargetCode = info.Code;
                    model.Type = info.Type;
                    model.ProductId = info.ProductId;
                    model.ExpiryDate = info.ExpiryDate;
                }
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult Create(MembershipViewModel model, bool? IsPopup, string kiemtracheck)
        {
            IsPopup = true;
            if (ModelState.IsValid)
            {
                using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.Snapshot }))
                {
                    var Membershipold = MembershipRepository.GetMembershipById(model.TargetId.Value);
                    Membershipold.Status = "tranfer";
                    MembershipRepository.UpdateMembership(Membershipold);
                    var Membershipnew = new Erp.Domain.Sale.Entities.Membership();
                    //AutoMapper.Mapper.Map(Membershipnew, Membershipold);
                    Membershipnew.Status = "receive";
                    Membershipnew.CustomerId = model.CustomerId;
                    Membershipnew.ProductId = model.ProductId;

                    //var MembershipParent = Membership_parentRepository.GetListMembership_parentByIdcustomer(model.CustomerId.Value).ToList();
                    //long idparent = MembershipParent[0].Id;
                    //DateTime timetmp;
                    //timetmp = MembershipParent[0].CreatedDate.Value;
                    //foreach (var item in MembershipParent)
                    //{
                    //    if (timetmp <= item.CreatedDate.Value)
                    //    {
                    //        timetmp = item.CreatedDate.Value;
                    //        idparent = item.Id;
                    //    }

                    //}
                    //
                    var Cus = customerRepository.GetCustomerById(model.CustomerId.Value);
                    var MembershipoldParent = Membership_parentRepository.GetMembership_parentById(Membershipold.IdParent);
                    //cong -- begin tạo mới MBS parent
                    var ins = new Membership_parent();
                    ins.IsDeleted = false;
                    ins.CreatedUserId = WebSecurity.CurrentUserId;
                    ins.ModifiedUserId = WebSecurity.CurrentUserId;
                    ins.AssignedUserId = WebSecurity.CurrentUserId;
                    ins.CreatedDate = DateTime.Now;
                    ins.ModifiedDate = DateTime.Now;
                    ins.CustomerId = model.CustomerId.Value;
                    ins.ProductInvoiceId = 0;
                    ins.Status = "pending";
                    ins.Code = MembershipoldParent.Code + "/" + Cus.Code;
                    ins.QRCode = MembershipoldParent.QRCode + "/" + Cus.Code;
                    ins.Note = "Được chuyển";
                    ins.SerialNumber = MembershipoldParent.SerialNumber;
                    ins.ExpiryDate = MembershipoldParent.ExpiryDate;
                    ins.Total = 1;
                    //ins.isPrint = 
                    //ins.NumberPrint = item.NumberPrint;
                    ins.BranchId = model.BranchId.Value;
                    ins.is_extend = 0; //?????
                    var checkMSP = Membership_parentRepository.GetListMembership_parentByIdcustomer(model.CustomerId.Value).Where(x => x.Code == ins.Code).ToList();
                    if (checkMSP.Count == 0)
                    {
                        Membership_parentRepository.InsertMembership_parent(ins);
                        Membershipnew.IdParent = ins.Id;
                    }
                    else
                    {
                        long idparent = checkMSP[0].Id;
                        Membershipnew.IdParent = idparent;
                        var isn2 = Membership_parentRepository.GetMembership_parentById(idparent);
                        isn2.Total = isn2.Total + 1;
                        Membership_parentRepository.UpdateMembership_parent(isn2);
                    }


                    //var mebershipparent = MembershipRepository.GetAllMembershipByIdParent(idparent).First();
                    Membershipnew.IsDeleted = false;
                    Membershipnew.CreatedUserId = WebSecurity.CurrentUserId;
                    Membershipnew.ModifiedUserId = WebSecurity.CurrentUserId;
                    Membershipnew.AssignedUserId = WebSecurity.CurrentUserId;
                    Membershipnew.CreatedDate = DateTime.Now;
                    Membershipnew.ModifiedDate = DateTime.Now;
                    Membershipnew.CustomerId = model.CustomerId;
                    Membershipnew.ProductId = model.ProductId;
                    Membershipnew.Status = "receive";

                    Membershipnew.TargetModule = "ProductInvoiceDetail";
                    Membershipnew.TargetId = Membershipold.TargetId;
                    Membershipnew.TargetCode = Membershipold.Code;
                    Membershipnew.BranchId = model.BranchId;
                    Membershipnew.Type = "SkinScan";
                    Membershipnew.ExpiryDate = model.ExpiryDate;
                    Membershipnew.Is_extend = 0;
                    Membershipnew.TongLanCSD = 1;


                    Membershipnew.Code = Erp.BackOffice.Helpers.Common.GetOrderNo("Membership", model.Code);
                    //MembershipRepository.UpdateMembership(Membershipnew);
                    MembershipRepository.InsertMembership(Membershipnew);
                    Erp.BackOffice.Helpers.Common.SetOrderNo("Membership");

                    //end
                    //var mebershipparent = MembershipRepository.GetAllMembershipByIdParent(idparent).First();
                    //Membershipnew.IsDeleted = false;
                    //Membershipnew.CreatedUserId = WebSecurity.CurrentUserId;
                    //Membershipnew.ModifiedUserId = WebSecurity.CurrentUserId;
                    //Membershipnew.AssignedUserId = WebSecurity.CurrentUserId;
                    //Membershipnew.CreatedDate = DateTime.Now;
                    //Membershipnew.ModifiedDate = DateTime.Now;
                    //Membershipnew.CustomerId = model.CustomerId;
                    //Membershipnew.ProductId = model.ProductId;
                    //Membershipnew.Status = "receive";
                    //Membershipnew.IdParent = ins.Id;
                    //Membershipnew.TargetModule = "ProductInvoiceDetail";
                    //Membershipnew.TargetId = mebershipparent.TargetId;
                    //Membershipnew.TargetCode = mebershipparent.Code;
                    //Membershipnew.BranchId = model.BranchId;
                    //Membershipnew.Type = "SkinScan";
                    //Membershipnew.ExpiryDate = model.ExpiryDate;
                    //Membershipnew.Is_extend = 0;
                    //Membershipnew.TongLanCSD = mebershipparent.TongLanCSD;


                    //Membershipnew.Code = Erp.BackOffice.Helpers.Common.GetOrderNo("Membership", model.Code);
                    ////MembershipRepository.UpdateMembership(Membershipnew);
                    //MembershipRepository.InsertMembership(Membershipnew);
                    //Erp.BackOffice.Helpers.Common.SetOrderNo("Membership");
                    ////
                    //var Membership = new Erp.Domain.Sale.Entities.Membership();

                    //AutoMapper.Mapper.Map(model, Membership);
                    //Membership.IsDeleted = false;
                    //Membership.CreatedUserId = WebSecurity.CurrentUserId;
                    //Membership.ModifiedUserId = WebSecurity.CurrentUserId;
                    //Membership.AssignedUserId = WebSecurity.CurrentUserId;
                    //Membership.CreatedDate = DateTime.Now;
                    //Membership.ModifiedDate = DateTime.Now;
                    //Membership.Status = "pending";
                    //MembershipRepository.InsertMembership(Membership);
                    //Membership.Code = Erp.BackOffice.Helpers.Common.GetOrderNo("Membership", model.Code);

                    //MembershipRepository.UpdateMembership(Membership);

                    //long targetID = 0;
                    //int customerID_OLD = 0;

                    //Erp.BackOffice.Helpers.Common.SetOrderNo("Membership");
                    //if (Membership.TargetModule == "Membership")
                    //{
                    //    var info_old = MembershipRepository.GetMembershipById(Membership.TargetId.Value);
                    //    if (info_old != null)
                    //    {
                    //        targetID = info_old.TargetId.Value;
                    //        customerID_OLD = info_old.CustomerId.Value;
                    //        info_old.Status = "tranfer";
                    //        info_old.ModifiedUserId = WebSecurity.CurrentUserId;
                    //        info_old.ModifiedDate = DateTime.Now;
                    //        MembershipRepository.UpdateMembership(info_old);
                    //    }
                    //}
                    //string str = kiemtracheck;
                    //if (kiemtracheck == "check")
                    //{
                    //    IEnumerable<MembershipViewModel> q = MembershipRepository.GetvwAllMembership().Where(x => x.BranchId == Erp.BackOffice.Helpers.Common.CurrentUser.BranchId && x.TargetModule == "ProductInvoiceDetail" && x.TargetId == targetID && x.CustomerId == customerID_OLD && x.Status == "pending")
                    //     .Select(item => new MembershipViewModel
                    //     {
                    //         Id = item.Id,
                    //         BranchId = item.BranchId,
                    //         Status = item.Status,
                    //         TargetCode = item.TargetCode,
                    //         TargetId = item.TargetId,
                    //         Code = item.Code
                    //     }).ToList();


                    //    //end  neu ma check thi thay doi het theo tagetid

                    //    foreach (var item in q)
                    //    {

                    //        //begin tao moi phieu MBS
                    //        Membership = new Erp.Domain.Sale.Entities.Membership();
                    //        AutoMapper.Mapper.Map(model, Membership);
                    //        Membership.IsDeleted = false;
                    //        Membership.CreatedUserId = WebSecurity.CurrentUserId;
                    //        Membership.ModifiedUserId = WebSecurity.CurrentUserId;
                    //        Membership.AssignedUserId = WebSecurity.CurrentUserId;
                    //        Membership.CreatedDate = DateTime.Now;
                    //        Membership.ModifiedDate = DateTime.Now;
                    //        Membership.TargetId = item.Id;
                    //        Membership.TargetCode = item.Code;
                    //        Membership.Status = "pending";
                    //        MembershipRepository.InsertMembership(Membership);
                    //        Membership.Code = Erp.BackOffice.Helpers.Common.GetOrderNo("Membership", model.Code);
                    //        MembershipRepository.UpdateMembership(Membership);

                    //        //end tao moi phieu MBS

                    //        //begin update lai MBD cu
                    //        if (Membership.TargetModule == "Membership")
                    //        {
                    //            var info_old = MembershipRepository.GetMembershipById(item.Id);
                    //            if (info_old != null)
                    //            {
                    //                info_old.Status = "tranfer";
                    //                info_old.ModifiedUserId = WebSecurity.CurrentUserId;
                    //                info_old.ModifiedDate = DateTime.Now;
                    //                MembershipRepository.UpdateMembership(info_old);
                    //            }
                    //        }
                    //        //end update lai MBS cu



                    //    }

                    //}

                    scope.Complete();

                }

                if (IsPopup == true)
                {
                    return RedirectToAction("_ClosePopup", "Home", new { area = "", FunctionCallback = "ClosePopupAndReloadPage" });
                }
                else
                {
                    TempData[Globals.SuccessMessageKey] = App_GlobalResources.Wording.InsertSuccess;
                    return RedirectToAction("Index");
                }
            }
            return View(model);
        }
        #endregion

        #region Edit
        // Lấy thuộc tính checked
        public ActionResult Edit(int? Id)
        {

            var Membership = MembershipRepository.GetvwMembershipById(Id.Value);
            if (Membership != null && Membership.IsDeleted != true)
            {
                var model = new MembershipViewModel();
                AutoMapper.Mapper.Map(Membership, model);

                return View(model);
            }
            if (Request.UrlReferrer != null)
                return Redirect(Request.UrlReferrer.AbsoluteUri);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult Edit(MembershipViewModel model, bool? IsPopup, string kiemtracheck)
        {
            IsPopup = true;
            if (ModelState.IsValid)
            {
                if (Request["Submit"] == "Save")
                {
                    using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.Snapshot }))
                    {
                        var Membership = MembershipRepository.GetMembershipById(model.Id);
                        AutoMapper.Mapper.Map(model, Membership);
                        Membership.ModifiedUserId = WebSecurity.CurrentUserId;
                        Membership.ModifiedDate = DateTime.Now;
                        MembershipRepository.UpdateMembership(Membership);

                        //begin neu ma check thi thay doi het theo tagetid
                        string str = kiemtracheck;
                        if (kiemtracheck == "check")
                        {
                            IEnumerable<MembershipViewModel> q = MembershipRepository.GetvwAllMembership().Where(x => x.BranchId == Erp.BackOffice.Helpers.Common.CurrentUser.BranchId && x.TargetModule == "ProductInvoiceDetail" && x.TargetId == model.TargetId && x.CustomerId == model.CustomerId && x.Status == "pending")
                             .Select(item => new MembershipViewModel
                             {
                                 Id = item.Id,
                                 BranchId = item.BranchId,
                                 Status = item.Status,
                                 TargetCode = item.TargetCode,
                                 TargetId = item.TargetId,
                             }).ToList();


                            //end  neu ma check thi thay doi het theo tagetid

                            foreach (var item in q)
                            {
                                Membership = MembershipRepository.GetMembershipById(item.Id);
                                Membership.ModifiedUserId = WebSecurity.CurrentUserId;
                                Membership.ModifiedDate = DateTime.Now;
                                Membership.Is_extend = 1;
                                Membership.ExpiryDateOld = Membership.ExpiryDate;
                                Membership.ExpiryDate = model.ExpiryDate;

                                MembershipRepository.UpdateMembership(Membership);
                            }

                        }


                        scope.Complete();
                    }

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
        [HttpPost]
        public ActionResult UpdateTargetCodeALL()
        {
            var Membership = MembershipRepository.GetvwAllMembership().ToList();

            return View(Membership);

        }
        [AllowAnonymous]
        public ActionResult Action123()
        {
            updatesudung();
            updatetralai();
            return View();
        }


        #region Detail
        public ActionResult Detail(int? Id)
        {
            var Membership = MembershipRepository.GetvwMembershipById(Id.Value);
            if (Membership != null && Membership.IsDeleted != true)
            {
                var model = new MembershipViewModel();
                AutoMapper.Mapper.Map(Membership, model);

                //if (model.CreatedUserId != Helpers.Common.CurrentUser.Id && Helpers.Common.CurrentUser.UserTypeId != 1)
                //{
                //    TempData["FailedMessage"] = "NotOwner";
                //    return RedirectToAction("Index");
                //}                

                return View(model);
            }
            if (Request.UrlReferrer != null)
                return Redirect(Request.UrlReferrer.AbsoluteUri);
            return RedirectToAction("Index");
        }
        #endregion

        #region Update su dung va tra lai

        public void updatesudung()
        {
            try
            {
                //begin cap nhat so lan su dung

                IEnumerable<MembershipViewModel> q = MembershipRepository.GetvwAllMembership().Where(x => x.BranchId == Erp.BackOffice.Helpers.Common.CurrentUser.BranchId)
                                        .Select(item => new MembershipViewModel
                                        {
                                            Id = item.Id,
                                            BranchId = item.BranchId,
                                            Status = item.Status,
                                            TargetCode = item.TargetCode,
                                            TargetId = item.TargetId,
                                            Code = item.Code,
                                            idcu = item.idcu,
                                            solandadung = item.solandadung,
                                            solantratra = item.solantratra,
                                            TongLanCSD = item.TongLanCSD
                                        }).ToList();


                //end  neu ma check thi thay doi het theo tagetid
                string pTargetCode = "";
                foreach (var item in q)
                {
                    if ((pTargetCode != item.idcu) && (item.TongLanCSD > 0))
                    {
                        pTargetCode = item.idcu;

                        IEnumerable<MembershipViewModel> q1 = q.Where(x => x.idcu == item.idcu && x.Status != "pending");
                        if (q1.Count() == 0)
                        {
                            IEnumerable<MembershipViewModel> q2 = q.Where(x => x.idcu == item.idcu);


                            if (q2.Count() > 0)
                            {
                                int? psolandadung = q2.ElementAt(0).solandadung;
                                if ((psolandadung != null) && (psolandadung > 0))
                                {
                                    int pSolancapnhat = 0;
                                    foreach (var item1 in q2)
                                    {
                                        if (pSolancapnhat < psolandadung)
                                        {
                                            var info_old = MembershipRepository.GetMembershipById(item1.Id);
                                            if (info_old != null)
                                            {
                                                info_old.Status = "complete";
                                                MembershipRepository.UpdateMembership(info_old);
                                            }
                                            pSolancapnhat = pSolancapnhat + 1;
                                        }
                                        else
                                        {
                                            break;
                                        }
                                    }
                                }
                            }



                        }

                    }


                }
                TempData[Globals.SuccessMessageKey] = App_GlobalResources.Wording.DeleteSuccess;


                //end cap nhat so lan su dung








                ////begin cap nhat so lan tra
                //IEnumerable<MembershipViewModel> q = MembershipRepository.GetvwAllMembership().Where(x => x.BranchId == Erp.BackOffice.Helpers.Common.CurrentUser.BranchId)
                //                           .Select(item => new MembershipViewModel
                //                           {
                //                               Id = item.Id,
                //                               BranchId = item.BranchId,
                //                               Status = item.Status,
                //                               TargetCode = item.TargetCode,
                //                               TargetId = item.TargetId,
                //                               Code = item.Code,
                //                               idcu = item.idcu,
                //                               solandadung = item.solandadung,
                //                               solantratra = item.solantratra,
                //                               TongLanCSD = item.TongLanCSD
                //                           }).ToList();


                ////end  neu ma check thi thay doi het theo tagetid
                //string pTargetCode = "";
                //foreach (var item in q)
                //{
                //    if ((pTargetCode != item.idcu) && (item.TongLanCSD > 0))
                //    {
                //        pTargetCode = item.idcu;
                //        //if (pTargetCode == "5C114D1E-84B9-4982-8DCF-198539E764A6")
                //        //{
                //        IEnumerable<MembershipViewModel> q1 = q.Where(x => x.idcu == item.idcu && x.Status == "pending");
                //        if (q1.Count() > 0)
                //        {
                //            if (q1.Count() > 0)
                //            {
                //                int? psolantratra = q1.ElementAt(0).solantratra;
                //                if ((psolantratra != null) && (psolantratra > 0))
                //                {
                //                    int pSolancapnhat = 0;
                //                    foreach (var item1 in q1)
                //                    {
                //                        if (pSolancapnhat < psolantratra)
                //                        {
                //                            var info_old = MembershipRepository.GetMembershipById(item1.Id);
                //                            if (info_old != null)
                //                            {
                //                                info_old.Status = "TraHang";
                //                                MembershipRepository.UpdateMembership(info_old);
                //                            }
                //                            pSolancapnhat = pSolancapnhat + 1;
                //                        }
                //                        else
                //                        {
                //                            break;
                //                        }
                //                    }
                //                }
                //            }

                //        }
                //        //}
                //    }
                //}
                //TempData[Globals.SuccessMessageKey] = App_GlobalResources.Wording.DeleteSuccess;


                //end cap nhat so lan tra











            }
            catch (Exception ex)
            {
                TempData[Globals.FailedMessageKey] = App_GlobalResources.Error.RelationError;

            }
        }




        public void updatetralai()
        {
            try
            {





                //begin cap nhat so lan tra
                IEnumerable<MembershipViewModel> q = MembershipRepository.GetvwAllMembership().Where(x => x.BranchId == Erp.BackOffice.Helpers.Common.CurrentUser.BranchId)
                                           .Select(item => new MembershipViewModel
                                           {
                                               Id = item.Id,
                                               BranchId = item.BranchId,
                                               Status = item.Status,
                                               TargetCode = item.TargetCode,
                                               TargetId = item.TargetId,
                                               Code = item.Code,
                                               idcu = item.idcu,
                                               solandadung = item.solandadung,
                                               solantratra = item.solantratra,
                                               TongLanCSD = item.TongLanCSD
                                           }).ToList();


                //end  neu ma check thi thay doi het theo tagetid
                string pTargetCode = "";
                foreach (var item in q)
                {
                    if ((pTargetCode != item.idcu) && (item.TongLanCSD > 0))
                    {
                        pTargetCode = item.idcu;
                        //if (pTargetCode == "5C114D1E-84B9-4982-8DCF-198539E764A6")
                        //{
                        IEnumerable<MembershipViewModel> q1 = q.Where(x => x.idcu == item.idcu && x.Status == "pending");
                        if (q1.Count() > 0)
                        {
                            if (q1.Count() > 0)
                            {
                                int? psolantratra = q1.ElementAt(0).solantratra;
                                if ((psolantratra != null) && (psolantratra > 0))
                                {
                                    int pSolancapnhat = 0;
                                    foreach (var item1 in q1)
                                    {
                                        if (pSolancapnhat < psolantratra)
                                        {
                                            var info_old = MembershipRepository.GetMembershipById(item1.Id);
                                            if (info_old != null)
                                            {
                                                info_old.Status = "TraHang";
                                                MembershipRepository.UpdateMembership(info_old);
                                            }
                                            pSolancapnhat = pSolancapnhat + 1;
                                        }
                                        else
                                        {
                                            break;
                                        }
                                    }
                                }
                            }

                        }
                        //}
                    }
                }
                TempData[Globals.SuccessMessageKey] = App_GlobalResources.Wording.DeleteSuccess;














            }
            catch (Exception ex)
            {
                TempData[Globals.FailedMessageKey] = App_GlobalResources.Error.RelationError;

            }
        }

        #endregion
        #region Delete
        [HttpPost]
        public ActionResult Delete(int id)
        {
            try
            {

                var item = MembershipRepository.GetMembershipById(id);
                if (item != null)
                {
                    item.IsDeleted = true;
                    MembershipRepository.UpdateMembership(item);
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
        #region Revert

        public ActionResult Revert(int id, bool? IsPopup)
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

                var item = MembershipRepository.GetMembershipById(id);
                var DetailServiceList = serviceDetailRepository.GetvwAllServiceDetail().Where(x => x.ServiceId == item.ProductId)
                               .Select(x => new ProductViewModel
                               {
                                   Id = x.ProductId.Value,
                                   Quantity = x.Quantity.Value,
                                   Code = x.ProductCode,
                                   Name = x.ProductName
                               }).ToList();

                var warehouseDefault = WarehouseRepository.GetAllWarehouse().Where(x => (",GLT,").Contains(x.Categories) && x.BranchId == intBrandID).First();
                string check = "";


                InquiryCard item2 = InquiryCardRepository.GetAllInquiryCard().Where(a => a.TargetId == item.Id).FirstOrDefault();
                SchedulingHistory item3 = SchedulingHistoryRepository.GetAllSchedulingHistory().Where(a => a.InquiryCardId.Value == item2.Id).FirstOrDefault();
                if (item != null && item2 != null || item != null && item3 != null)
                {
                    if (item2 != null)
                    {

                        InquiryCardRepository.DeleteInquiryCard(item2.Id);

                    }
                    if (item3 != null)
                    {
                        var bed = bedRepository.GetBedById(item3.BedId.Value);
                        bed.Trang_Thai = false;
                        bedRepository.UpdateBed(bed);
                        var listEquipment = logEquipmentRepository.GetListAllLogEquipment().Where(x => x.SchedulingHistoryId == item3.Id);
                        foreach (var item4 in listEquipment)
                        {
                            var equipment = staffEquipmentRepository.GetStaffEquipmentById(item4.EquipmentId.Value);
                            equipment.StatusStaffMade = false;
                            staffEquipmentRepository.UpdateEquipment(equipment);
                        }
                        SchedulingHistoryRepository.DeleteSchedulingHistory(item3.Id);


                    }

                    item.Status = "pending";
                    MembershipRepository.UpdateMembership(item);

                    //ghi log 
                    Erp.BackOffice.Controllers.HomeController.WriteLog(item.IdParent, item.Code, "đã hoàn phiếu MBS", "Membership/MembershipIndex/" + item.IdParent, Helpers.Common.CurrentUser.BranchId.Value);

                    if (warehouseDefault != null)
                    {

                        //foreach (var ite in DetailServiceList)
                        //{


                        //    var error = InventoryController.Update(ite.Name, ite.Id, ite.LoCode, ite.ExpiryDate, warehouseDefault.Id, ite.Quantity, 0);
                        //    check += error;
                        //    //Inventory inventorys = new Inventory();
                        //    //inventorys.WarehouseId = warehouseDefault.Id;

                        //    //inventorys.BranchId = Helpers.Common.CurrentUser.BranchId;
                        //    //inventorys.LoCode = k.LoCode;
                        //    //inventorys.ProductId = k.Id;
                        //    //inventorys.ExpiryDate = k.ExpiryDate;          
                        //    //InventoryRepository.InsertInventory(inventorys);



                        //}
                        if (DetailServiceList.Any())
                        {
                            InsertProductInbound(DetailServiceList, warehouseDefault, item2);
                        }
                    }
                }
                TempData[Globals.SuccessMessageKey] = App_GlobalResources.Wording.DeleteSuccess;
                if (IsPopup == true)
                {
                    return RedirectToAction("_ClosePopup", "Home", new { area = "", FunctionCallback = "ClosePopupAndReloadPage" });
                }
                return RedirectToAction("Index");
            }
            catch (DbUpdateException)
            {
                TempData[Globals.FailedMessageKey] = App_GlobalResources.Error.RelationError;
                return RedirectToAction("Index");
            }
        }
        #endregion

        #region GetInfoMemberShip
        [AllowAnonymous]
        public JsonResult GetInfoMemberShip(int id)
        {

            var q = MembershipRepository.GetvwMembershipById(id);
            var model = new MembershipViewModel();
            AutoMapper.Mapper.Map(q, model);
            return Json(model, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region CreateFromProductInvoiceDetail
        public ViewResult CreateFromProductInvoiceDetail(int Id)
        {
            ProductInvoiceViewModel model = new ProductInvoiceViewModel();
            model.Membership_parentList = new List<Membership_parentViewModel>();
            if (Id > 0)
            {
                var productInvoice = productInvoiceRepository.GetvwProductInvoiceById(Id);
                AutoMapper.Mapper.Map(productInvoice, model);

                model.Membership_parentList = Membership_parentRepository.GetAllvwMembership_parentByInvoiceId(productInvoice.Id).Select(x => new Membership_parentViewModel
                {
                    Id = x.Id,
                    //
                    CustomerId = x.CustomerId,
                    Code = x.Code,
                    ProductInvoiceId = x.ProductInvoiceId,
                    Status = x.Status,
                    QRCode = x.QRCode,
                    Note = x.Note,
                    SerialNumber = x.SerialNumber,
                    ExpiryDate = x.ExpiryDate,
                    isPrint = x.isPrint,
                    NumberPrint = x.NumberPrint,
                    is_extend = x.is_extend,
                    ProductInvoiceDetaiId = x.ProductInvoiceDetailId,
                    ProductCode = x.ProductCode,
                    ProductName = x.ProductName,
                    ProductId = x.ProductId,
                    Quantity = x.Total,

                    //
                    ProductType = x.ProductType

                }).ToList();

                var InvoiceList = productInvoiceRepository.GetAllvwInvoiceDetailsByInvoiceId(productInvoice.Id).Select(x => new Membership_parentViewModel
                {
                    ProductInvoiceDetaiId = x.Id,
                    ProductInvoiceId = x.ProductInvoiceId.Value,
                    ProductCode = x.ProductCode,
                    ProductName = x.ProductName,
                    ProductId = x.ProductId,
                    Quantity = x.Quantity,
                    ProductType = x.ProductType,
                    CustomerCode = x.CustomerCode,
                    CustomerId = x.CustomerId,
                    SOHOADON = x.SOHOADON,
                    ProductGroup = x.ProductGroup

                }).Where(x => x.ProductType == "service").ToList();
                int? cusId = InvoiceList.First().CustomerId;

                var count = Membership_parentRepository.GetAllMembership_parent().Where(x => x.CustomerId == cusId).Count() + 1;

                InvoiceList = InvoiceList.Where(id1 => !model.Membership_parentList.ToList().Any(id2 => id2.ProductInvoiceDetaiId == id1.ProductInvoiceDetaiId)).Select(x => new Membership_parentViewModel
                {
                    ProductInvoiceDetaiId = x.ProductInvoiceDetaiId,
                    ProductInvoiceId = x.ProductInvoiceId,
                    ProductCode = x.ProductCode,
                    ProductName = x.ProductName,
                    ProductId = x.ProductId,
                    Quantity = x.Quantity,
                    ProductType = x.ProductType,
                    SOHOADON = x.SOHOADON,
                    CustomerCode = x.CustomerCode,
                    ProductGroup = x.ProductGroup,
                    Code = x.CustomerCode + x.ProductGroup + x.SOHOADON,
                    QRCode = x.CustomerCode + x.ProductGroup + x.SOHOADON,
                    NumberPrint = 0
                }).ToList();

                //var q = model.Membership_parentList.Where(x => x.Id == 0 && x.ProductId > 0).ToList();
                if ((InvoiceList != null) && (InvoiceList.Count > 0))
                {
                    int? soHD = InvoiceList[0].SOHOADON;
                    for (int i = 0; i < InvoiceList.Count; i++)
                    {
                        soHD = checkCode(InvoiceList[i].CustomerCode, InvoiceList[i].ProductGroup, InvoiceList[i].SOHOADON, i);
                        InvoiceList[i].Code = InvoiceList[i].CustomerCode + InvoiceList[i].ProductGroup + soHD + "_" + (i + 1).ToString();
                        InvoiceList[i].QRCode = InvoiceList[i].CustomerCode + InvoiceList[i].ProductGroup + soHD + "_" + (i + 1).ToString();

                    }
                }

                if (model.Membership_parentList.Count() == 0)
                {
                    foreach (var item in InvoiceList)
                    {
                        model.Membership_parentList.Add(item);
                    }
                }
                ViewBag.FailedMessage = TempData["FailedMessage"];
                ViewBag.SuccessMessage = TempData["SuccessMessage"];
                //model.Membership_parentList = model.Membership_parentList.Where(x => x.ProductType == "service").ToList();
            }

            return View(model);
        }
        public int? checkCode(string cusCode, string grCode, int? soHD, int i)
        {
            string code = cusCode + grCode + soHD + "_" + (i + 1).ToString();
            var cCode = Membership_parentRepository.GetAllMembership_parent().Where(x => x.Code == code && x.Code != null).FirstOrDefault();
            if (cCode != null)
            {
                soHD++;
                return checkCode(cusCode, grCode, soHD, i);
            }
            else
                return soHD;

        }
        [HttpPost]
        public ActionResult CreateFromProductInvoiceDetail(ProductInvoiceViewModel model, bool? IsPopup)
        {
            if (ModelState.IsValid)
            {
                ProductInvoice productInvoice = null;

                if (model.Id > 0)
                {
                    productInvoice = productInvoiceRepository.GetProductInvoiceById(model.Id);
                }
                using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.Snapshot }))
                {
                    try
                    {   //insert Membership parent
                        if (model.Membership_parentList.Any(x => x.Id == 0))
                        {
                            //lưu danh sách thao tác thực hiện dịch vụ
                            var q = model.Membership_parentList.Where(x => x.Id == 0 && x.ProductId > 0).ToList();
                            foreach (var item in q)
                            {
                                //kiểm tra Serial Number
                                if ((item.ExpiryDate == null) || (item.ExpiryDate.ToString() == ""))
                                {
                                    TempData[Globals.FailedMessageKey] = "Ngày hết hạn MBS không được để trống!";
                                    return RedirectToAction("CreateFromProductInvoiceDetail", new { Id = model.Id });
                                }

                                //kiểm tra Code
                                var cCode = Membership_parentRepository.GetAllMembership_parent().Where(x => x.Code == item.Code && x.Code != null).FirstOrDefault();
                                if (cCode != null)
                                {
                                    TempData[Globals.FailedMessageKey] = "Mã MBS đã bị trùng. Vui lòng kiểm tra lại!";
                                    return RedirectToAction("CreateFromProductInvoiceDetail", new { Id = model.Id });
                                }
                                //kiểm tra QRCode
                                var cQR = Membership_parentRepository.GetAllMembership_parent().Where(x => x.QRCode == item.QRCode && x.QRCode != null).FirstOrDefault();
                                if (cQR != null)
                                {
                                    TempData[Globals.FailedMessageKey] = "QRCode đã bị trùng. Vui lòng kiểm tra lại!";
                                    return RedirectToAction("CreateFromProductInvoiceDetail", new { Id = model.Id });
                                }
                                //kiểm tra Serial Number
                                if ((item.SerialNumber != null) && (item.SerialNumber != ""))
                                {
                                    var cSerial = Membership_parentRepository.GetAllMembership_parent().Where(x => x.SerialNumber == item.SerialNumber).FirstOrDefault();
                                    if (cSerial != null)
                                    {
                                        TempData[Globals.FailedMessageKey] = "Số Serial đã bị trùng. Vui lòng kiểm tra lại!";
                                        return RedirectToAction("CreateFromProductInvoiceDetail", new { Id = model.Id });
                                    }
                                }
                                var ins = new Membership_parent();
                                ins.IsDeleted = false;
                                ins.CreatedUserId = WebSecurity.CurrentUserId;
                                ins.ModifiedUserId = WebSecurity.CurrentUserId;
                                ins.AssignedUserId = WebSecurity.CurrentUserId;
                                ins.CreatedDate = DateTime.Now;
                                ins.ModifiedDate = DateTime.Now;
                                ins.CustomerId = productInvoice.CustomerId.Value;
                                ins.ProductInvoiceId = productInvoice.Id;
                                ins.Status = "pending";
                                ins.Code = item.Code;
                                ins.QRCode = item.QRCode;
                                ins.Note = item.Note;
                                ins.SerialNumber = item.SerialNumber;
                                ins.ExpiryDate = item.ExpiryDate;
                                ins.Total = item.Quantity.Value;
                                //ins.isPrint = 
                                ins.NumberPrint = item.NumberPrint;
                                ins.BranchId = productInvoice.BranchId;
                                ins.is_extend = 0; //?????

                                Membership_parentRepository.InsertMembership_parent(ins);

                                //ghi log 
                                Erp.BackOffice.Controllers.HomeController.WriteLog(ins.Id, ins.Code, "đã tạo phiếu MBS", "Membership/MembershipIndex/" + ins.Id, Helpers.Common.CurrentUser.BranchId.Value);

                            }
                        }
                        if (model.Membership_parentList.Any(x => x.Id > 0)) //update Membership theo Id
                        {


                            var q = model.Membership_parentList.Where(x => x.Id > 0 && x.ProductId > 0).ToList();
                            foreach (var item in q)
                            {
                                //kiểm tra Giá trị có bị thay đổi không, nếu có kiểm tra Code,QRCode,SerialNumber
                                var oldItem = Membership_parentRepository.GetMembership_parentById(item.Id);

                                //kiểm tra Code
                                if (oldItem.Code != item.Code)
                                {
                                    var cCode = Membership_parentRepository.GetAllMembership_parent().Where(x => x.Code == item.Code).FirstOrDefault();
                                    if (cCode != null)
                                    {
                                        TempData[Globals.FailedMessageKey] = "Mã MBS đã bị trùng. Vui lòng kiểm tra lại!";
                                        return RedirectToAction("CreateFromProductInvoiceDetail", new { Id = model.Id });
                                    }
                                }
                                //kiểm tra QRCode
                                if (oldItem.QRCode != item.QRCode)
                                {
                                    var cQR = Membership_parentRepository.GetAllMembership_parent().Where(x => x.QRCode == item.QRCode).FirstOrDefault();
                                    if (cQR != null)
                                    {
                                        TempData[Globals.FailedMessageKey] = "QRCode đã bị trùng. Vui lòng kiểm tra lại!";
                                        return RedirectToAction("CreateFromProductInvoiceDetail", new { Id = model.Id });
                                    }
                                }
                                //kiểm tra Serial Number
                                if (oldItem.SerialNumber != item.SerialNumber)
                                {
                                    var cSerial = Membership_parentRepository.GetAllMembership_parent().Where(x => x.SerialNumber == item.SerialNumber).FirstOrDefault();
                                    if (cSerial != null)
                                    {
                                        TempData[Globals.FailedMessageKey] = "Số Serial đã bị trùng. Vui lòng kiểm tra lại!";
                                        return RedirectToAction("CreateFromProductInvoiceDetail", new { Id = model.Id });
                                    }
                                }

                                var _update = Membership_parentRepository.GetMembership_parentById(item.Id);
                                _update.ModifiedUserId = WebSecurity.CurrentUserId;
                                _update.ModifiedDate = DateTime.Now;
                                _update.Code = item.Code;
                                _update.QRCode = item.QRCode;
                                _update.SerialNumber = item.SerialNumber;
                                _update.ExpiryDateOld = _update.ExpiryDate;
                                _update.ExpiryDate = item.ExpiryDate;
                                _update.Note = item.Note;
                                _update.NumberPrint = item.NumberPrint;
                                _update.is_extend = 1;
                                //_update.isPrint = ;
                                Membership_parentRepository.UpdateMembership_parent(_update);

                                //ghi log 
                                Erp.BackOffice.Controllers.HomeController.WriteLog(_update.Id, _update.Code, "đã cập nhật phiếu MBS", "Membership/MembershipIndex/" + _update.Id, Helpers.Common.CurrentUser.BranchId.Value);

                            }
                        }

                        //insert Membership List
                        var _membership_parent = Membership_parentRepository.GetAllMembership_parentByInvoiceId(model.Id).ToList();

                        foreach (var item in _membership_parent)
                        {
                            var _membership = MembershipRepository.GetAllMembershipByIdParent(item.Id).ToList();
                            if (_membership.Count == 0)
                            {
                                //lấy membership_parent item trong model với Code = Code của membership_parent vừa tạo trong database

                                var msp = model.Membership_parentList.Find(x => x.Code == item.Code);
                                //Tạo mới Membership
                                for (int i = 0; i < item.Total; i++)
                                {
                                    var Membership = new Erp.Domain.Sale.Entities.Membership();
                                    Membership.IsDeleted = false;
                                    Membership.CreatedUserId = WebSecurity.CurrentUserId;
                                    Membership.ModifiedUserId = WebSecurity.CurrentUserId;
                                    Membership.AssignedUserId = WebSecurity.CurrentUserId;
                                    Membership.CreatedDate = DateTime.Now;
                                    Membership.ModifiedDate = DateTime.Now;
                                    Membership.CustomerId = item.CustomerId;
                                    Membership.ProductId = msp.ProductId;
                                    Membership.Status = "pending";
                                    Membership.IdParent = item.Id;
                                    Membership.TargetModule = "ProductInvoiceDetail";
                                    Membership.TargetId = msp.ProductInvoiceDetaiId;
                                    Membership.TargetCode = productInvoice.Code;
                                    Membership.BranchId = productInvoice.BranchId;
                                    Membership.Type = "SkinScan";
                                    Membership.ExpiryDate = item.ExpiryDate;
                                    Membership.Is_extend = 0;
                                    Membership.TongLanCSD = item.Total;
                                    MembershipRepository.InsertMembership(Membership);

                                    Membership.Code = Erp.BackOffice.Helpers.Common.GetOrderNo("Membership", model.Code);
                                    MembershipRepository.UpdateMembership(Membership);
                                    Erp.BackOffice.Helpers.Common.SetOrderNo("Membership");
                                }
                            }
                            else
                            {
                                // cập nhật Membership
                                foreach (var itemMS in _membership)
                                {
                                    var Membership = itemMS;
                                    Membership.ModifiedUserId = WebSecurity.CurrentUserId;
                                    Membership.ModifiedDate = DateTime.Now;
                                    Membership.Is_extend = 1;
                                    Membership.ExpiryDateOld = Membership.ExpiryDate;
                                    Membership.ExpiryDate = item.ExpiryDate;
                                    Membership.TongLanCSD = item.Total;

                                    MembershipRepository.UpdateMembership(Membership);

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
                if (IsPopup == true)
                {
                    return RedirectToAction("_ClosePopup", "Home", new { area = "", FunctionCallback = "ClosePopupAndReloadPage" });
                }
                else
                {
                    TempData[Globals.SuccessMessageKey] = App_GlobalResources.Wording.InsertSuccess;
                    return RedirectToAction("_ClosePopup", "Home", new { area = "", FunctionCallback = "ClosePopupAndReloadPage" });
                }

            }
            return View(model);
        }
        #endregion

        #region Delete Membership_parent

        public ActionResult Delete_parent(long id)
        {
            try
            {
                var item = Membership_parentRepository.GetMembership_parentById(id);
                var ProductInvoiceId = 0;
                if (item != null)
                {
                    ProductInvoiceId = item.ProductInvoiceId;
                    //item.IsDeleted = true;
                    //Membership_parentRepository.UpdateMembership_parent(item);

                    //xóa Membership
                    var _deleteMS = MembershipRepository.GetAllMembershipByIdParent(item.Id).ToList();
                    foreach (var itemMS in _deleteMS)
                    {
                        MembershipRepository.DeleteMembership(itemMS.Id);
                    }
                    Membership_parentRepository.DeleteMembership_parent(item.Id);
                }
                TempData[Globals.SuccessMessageKey] = App_GlobalResources.Wording.DeleteSuccess;
                return RedirectToAction("CreateFromProductInvoiceDetail", new { Id = ProductInvoiceId });
            }
            catch (DbUpdateException)
            {
                TempData[Globals.FailedMessageKey] = App_GlobalResources.Error.RelationError;
                return RedirectToAction("Index", "ProductInvoice");
            }
        }
        #endregion

        #region PrintMBS
        public ActionResult PrintAllMBS(string idPrintAll)
        {
            var modelBiaNgoai = new TemplatePrintViewModel();
            var logo = Erp.BackOffice.Helpers.Common.GetSetting("LogoCompany");
            var company = Erp.BackOffice.Helpers.Common.GetSetting("companyName");
            var address = Erp.BackOffice.Helpers.Common.GetSetting("addresscompany");
            var phone = Erp.BackOffice.Helpers.Common.GetSetting("phonecompany");
            var fax = Erp.BackOffice.Helpers.Common.GetSetting("faxcompany");
            var bankcode = Erp.BackOffice.Helpers.Common.GetSetting("bankcode");
            var bankname = Erp.BackOffice.Helpers.Common.GetSetting("bankname");
            var ImgLogo = "<div class=\"logo\"><img src=" + logo + " height=\"73\" /></div>";

            //string idPrintAll = Request["PrintId-checkbox"];
            string[] arrPrintId = idPrintAll.Split(',');
            List<string> arrList = arrPrintId.ToList();

            //lấy template phiếu MBS.
            var templateBiaNgoai = templatePrintRepository.GetAllTemplatePrint().Where(x => x.Code.Contains("BiaNgoaiMembership")).OrderByDescending(x => x.CreatedDate).FirstOrDefault();
            modelBiaNgoai.Content = templateBiaNgoai.Content;

            modelBiaNgoai.Content = modelBiaNgoai.Content.Replace("{System.AddressCompany}", address);
            modelBiaNgoai.Content = modelBiaNgoai.Content.Replace("{System.PhoneCompany}", phone);

            //tạo modelList chứa danh sách model bìa trong, bìa ngoài tương tự nhau nên chỉ load 1 cái
            var modelList = new List<TemplatePrintViewModel>();
            var templateBiaTrong = templatePrintRepository.GetAllTemplatePrint().Where(x => x.Code.Contains("PrintMatTrongMS")).OrderByDescending(x => x.CreatedDate).FirstOrDefault();
            var templateBiaTrong2 = templatePrintRepository.GetAllTemplatePrint().Where(x => x.Code.Contains("MatTrongMBS2")).OrderByDescending(x => x.CreatedDate).FirstOrDefault();

            foreach (var item in arrList)
            {
                var membership_parent = Membership_parentRepository.GetvwMembership_parentById(Int64.Parse(item));


                var tienno = productInvoiceRepository.GetAllvwProductInvoiceFull().Where(x => x.Id == membership_parent.ProductInvoiceId).FirstOrDefault();
                if (tienno != null && tienno.tiendathu == null)
                {
                    tienno.tiendathu = 0;
                }

                //begin tính số lần được phép chăm sóc
                int psolanduocphepsudung = membership_parent.Total;
                if (tienno != null)
                {
                    if (tienno.tienconno > 0)
                    {
                        decimal? pdongia = (tienno.tienconno + tienno.tiendathu) / membership_parent.Total;
                        psolanduocphepsudung = decimal.ToInt16((tienno.tiendathu / pdongia).Value);

                    }

                }

                //end tính số lần được phép chăm sóc


                var membership = MembershipRepository.GetAllvwMembershipByIdParent(membership_parent.Id);
                var customer = customerRepository.GetCustomerById(membership_parent.CustomerId);

                var mbsfirst = membership.FirstOrDefault(x => x.CustomerId == membership_parent.CustomerId);
                //truyền dữ liệu vào template.
                //truyền dữ liệu bìa trong vào list bìa trong
                var modelBiatrong = new TemplatePrintViewModel();

                if (membership_parent.Total <= 10)
                {
                    modelBiatrong.Content = templateBiaTrong.Content;
                }
                else
                {
                    modelBiatrong.Content = templateBiaTrong2.Content;
                }

                modelBiatrong.Content = modelBiatrong.Content.Replace("{maKH}", mbsfirst.CustomerCode);
                modelBiatrong.Content = modelBiatrong.Content.Replace("{tenKH}", mbsfirst.CustomerName);
                modelBiatrong.Content = modelBiatrong.Content.Replace("{tenNV}", mbsfirst.ManagerName);
                modelBiatrong.Content = modelBiatrong.Content.Replace("{Nhom}", membership_parent.UserTypeName_kd);
                modelBiatrong.Content = modelBiatrong.Content.Replace("{Tel}", mbsfirst.CustomerPhone);
                modelBiatrong.Content = modelBiatrong.Content.Replace("{SoSeri}", membership_parent.SerialNumber);
                modelBiatrong.Content = modelBiatrong.Content.Replace("{chuongtrinh}", membership_parent.ProductCode);
                modelBiatrong.Content = modelBiatrong.Content.Replace("{membership}", membership_parent.Code);
                modelBiatrong.Content = modelBiatrong.Content.Replace("{dd}", mbsfirst.CreatedDate.Value.Day.ToString());
                modelBiatrong.Content = modelBiatrong.Content.Replace("{mm}", mbsfirst.CreatedDate.Value.Month.ToString());
                modelBiatrong.Content = modelBiatrong.Content.Replace("{yyyy}", mbsfirst.CreatedDate.Value.Year.ToString());
                var solan = membership_parent.Total;//số lần sử dụng 
                                                    //membership.status = pending la chua su dung: de trang
                                                    //membership.status = complete la da su dung : ghi "v" hoac "da su dung"
                                                    //membership.status =... : khong su dung

                int orderNo = 0;
                foreach (var mbsitem in membership)
                {
                    orderNo++;
                    if (mbsitem.Status == "complete")
                    {
                        //lấy ngày thang năm da sử dụng
                        string a = mbsitem.ModifiedDate.ToString();
                        if (!string.IsNullOrEmpty(a))
                        {
                            string[] aa = a.Split(' ');
                            string[] aaa = aa[0].Split('/');
                            string[] yyy = aaa[2].Split();
                            string c = yyy[0].Substring(yyy[0].Length - 2, 2);
                            //end lấy ngày tháng nắm đã sử dụng
                            modelBiatrong.Content = modelBiatrong.Content.Replace("{" + orderNo + "}", "***Đã sử dụng***");
                            modelBiatrong.Content = modelBiatrong.Content.Replace("{ddd" + orderNo + "}", aaa[0]);
                            modelBiatrong.Content = modelBiatrong.Content.Replace("{mmm" + orderNo + "}", aaa[1]);
                            modelBiatrong.Content = modelBiatrong.Content.Replace("{yyy" + orderNo + "}", c + "(" + aa[1] + " " + aa[2] + ")");
                        }
                    }
                    if (mbsitem.Status == "TraHang")
                    {
                        modelBiatrong.Content = modelBiatrong.Content.Replace("{" + orderNo + "}", "-----Trả Hàng-------");
                        modelBiatrong.Content = modelBiatrong.Content.Replace("{ddd" + orderNo + "}", "&nbsp; &nbsp;");
                        modelBiatrong.Content = modelBiatrong.Content.Replace("{mmm" + orderNo + "}", "&nbsp; &nbsp;");
                        modelBiatrong.Content = modelBiatrong.Content.Replace("{yyy" + orderNo + "}", "&nbsp; &nbsp;");
                    }
                    else if ((mbsitem.Status == "pending" || mbsitem.Status == "inprocess") && (orderNo <= psolanduocphepsudung))
                    {
                        modelBiatrong.Content = modelBiatrong.Content.Replace("{" + orderNo + "}", "---------------");
                        modelBiatrong.Content = modelBiatrong.Content.Replace("{ddd" + orderNo + "}", "&nbsp; &nbsp;");
                        modelBiatrong.Content = modelBiatrong.Content.Replace("{mmm" + orderNo + "}", "&nbsp; &nbsp;");
                        modelBiatrong.Content = modelBiatrong.Content.Replace("{yyy" + orderNo + "}", "&nbsp; &nbsp;");
                    }
                    else if ((mbsitem.Status == "pending") && (psolanduocphepsudung < solan))
                    {
                        if (orderNo > psolanduocphepsudung)
                        {
                            modelBiatrong.Content = modelBiatrong.Content.Replace("{" + orderNo + "}", "CTT");
                            modelBiatrong.Content = modelBiatrong.Content.Replace("{ddd" + orderNo + "}", "&nbsp; &nbsp;");
                            modelBiatrong.Content = modelBiatrong.Content.Replace("{mmm" + orderNo + "}", "&nbsp; &nbsp;");
                            modelBiatrong.Content = modelBiatrong.Content.Replace("{yyy" + orderNo + "}", "&nbsp; &nbsp;");

                        }

                    }
                }

                //nếu sử dụng 3 lần thì từ 4 đến 10 đổi thành không sử dụng
                for (int khongsudung = orderNo + 1; khongsudung <= 20; khongsudung++)
                {
                    modelBiatrong.Content = modelBiatrong.Content.Replace("{" + khongsudung + "}", "Không Sử Dụng");
                    modelBiatrong.Content = modelBiatrong.Content.Replace("{ddd" + khongsudung + "}", "&nbsp; &nbsp;");
                    modelBiatrong.Content = modelBiatrong.Content.Replace("{mmm" + khongsudung + "}", "&nbsp; &nbsp;");
                    modelBiatrong.Content = modelBiatrong.Content.Replace("{yyy" + khongsudung + "}", "&nbsp; &nbsp;");
                }


                modelList.Add(modelBiatrong);
            }


            ViewBag.BiaTrong = modelList;
            return View(modelBiaNgoai);
        }

        #endregion

        #region Lập phiếu nhập lúc hoàn phiếu
        public void InsertProductInbound(List<ProductViewModel> DetailServiceList, Warehouse warehouseDefault, InquiryCard inquiryCard)
        {
            try
            {
                //nhập kho hàng
                var productInbound = new Domain.Sale.Entities.ProductInbound();

                productInbound.IsDeleted = false;
                productInbound.CreatedUserId = WebSecurity.CurrentUserId;
                productInbound.ModifiedUserId = WebSecurity.CurrentUserId;
                productInbound.CreatedDate = DateTime.Now;
                productInbound.ModifiedDate = DateTime.Now;
                productInbound.Type = "inquiry_card";

                productInbound.BranchId = warehouseDefault.BranchId;
                productInbound.WarehouseDestinationId = warehouseDefault.Id;
                productInbound.TotalAmount = DetailServiceList.Sum(x => x.PriceOutbound * x.Quantity);
                productInbound.IsArchive = false;
                productInbound.Note = "Nhập kho hoàn phiếu MBS";
                ProductInboundRepository.InsertProductInbound(productInbound);

                //nhập kho vật tư
                var warehouseMaterial = warehouseRepository.GetAllWarehouse().Where(x => ((",VT,").Contains(x.Categories)) && (x.BranchId == warehouseDefault.BranchId)).FirstOrDefault();

                var check = "service";
                var MaterialInbound = new Domain.Sale.Entities.MaterialInbound();
                MaterialInbound.IsDeleted = false;
                MaterialInbound.CreatedUserId = WebSecurity.CurrentUserId;
                MaterialInbound.ModifiedUserId = WebSecurity.CurrentUserId;
                MaterialInbound.CreatedDate = DateTime.Now;
                MaterialInbound.ModifiedDate = DateTime.Now;
                MaterialInbound.Type = check;
                if (check == "service")
                {
                    MaterialInbound.WarehouseDestinationId = null;
                    ///Xuất kho vật tư
                    ///
                }
                // var branch = warehouseRepository.GetWarehouseById(MaterialOutbound.WarehouseSourceId.Value);
                MaterialInbound.BranchId = warehouseMaterial.BranchId;
                MaterialInbound.IsDone = false;
                MaterialInbound.TotalAmount = 0;//outboundDetails.Sum(item => item.Quantity * item.Price);
                MaterialInbound.WarehouseDestinationId = warehouseMaterial.Id;
                MaterialInbound.Note = "Nhập kho vật tư cho dịch vụ";
                materialInboundRepository.InsertMaterialInbound(MaterialInbound);
                var MaterialList = new List<ProductDetailViewModel>();

                //Cập nhật lại mã xuất kho
                productInbound.Code = Erp.BackOffice.Helpers.Common.GetOrderNo("ProductInbound");
                ProductInboundRepository.UpdateProductInbound(productInbound);
                Erp.BackOffice.Helpers.Common.SetOrderNo("ProductInbound");
                // chi tiết phiếu xuất
                if (DetailServiceList.Any())
                {
                    foreach (var item in DetailServiceList)
                    {
                        ProductInboundDetail ins_detail = new ProductInboundDetail();
                        ins_detail.IsDeleted = false;
                        ins_detail.CreatedUserId = WebSecurity.CurrentUserId;
                        ins_detail.ModifiedUserId = WebSecurity.CurrentUserId;
                        ins_detail.CreatedDate = DateTime.Now;
                        ins_detail.ModifiedDate = DateTime.Now;
                        ins_detail.ProductInboundId = productInbound.Id;
                        ins_detail.Price = item.PriceOutbound;
                        ins_detail.Quantity = item.Quantity;
                        ins_detail.ProductId = item.Id;
                        ins_detail.Unit = item.Unit;
                        ProductInboundRepository.InsertProductInboundDetail(ins_detail);

                        var ProductDetailList = new List<ProductDetailViewModel>();
                        ProductDetailList = productDetailRepository.GetvwAllProductDetail().Where(x => x.ProductId == item.Id)
                       .Select(x => new ProductDetailViewModel
                       {
                           MaterialName = x.MaterialName,
                           Id = x.Id,
                           ProductId = x.ProductId,
                           MaterialCode = x.MaterialCode,
                           MaterialId = x.MaterialId,
                           Quantity = x.Quantity
                       }).ToList();
                        if (ProductDetailList.Any())
                        {
                            foreach (var i in ProductDetailList)
                            {
                                MaterialList.Add(i);
                            }
                        }
                    }
                    //Chi tiết nhập vật tư
                    if (MaterialList.Any())
                    {
                        foreach (var item in MaterialList)
                        {

                            MaterialInboundDetail materialInboundDetail = new MaterialInboundDetail();
                            materialInboundDetail.MaterialId = item.MaterialId;
                            materialInboundDetail.Price = 0;
                            materialInboundDetail.Quantity = item.Quantity;
                            materialInboundDetail.Unit = "";
                            // materialOutboundDetail.LoCode = "";//item.LoCode;
                            // materialOutboundDetail.ExpiryDate = "";// item.ExpiryDate;
                            materialInboundDetail.IsDeleted = false;
                            materialInboundDetail.CreatedUserId = WebSecurity.CurrentUserId;
                            materialInboundDetail.ModifiedUserId = WebSecurity.CurrentUserId;
                            materialInboundDetail.CreatedDate = DateTime.Now;
                            materialInboundDetail.ModifiedDate = DateTime.Now;
                            materialInboundDetail.MaterialInboundId = MaterialInbound.Id;
                            materialInboundRepository.InsertMaterialInboundDetail(materialInboundDetail);

                        }
                    }
                }
                MaterialInbound.Code = Erp.BackOffice.Helpers.Common.GetOrderNo("MaterialInbound");
                materialInboundRepository.UpdateMaterialInbound(MaterialInbound);
                Erp.BackOffice.Helpers.Common.SetOrderNo("MaterialInbound");
                ProductInboundController.Archive(productInbound);
                MaterialInboundController.Archive(MaterialInbound);
                TransactionController.Create(new TransactionViewModel
                {
                    TransactionModule = "ProductInbound",
                    TransactionCode = productInbound.Code,
                    TransactionName = "Nhập kho - hoàn phiếu MBS"
                });
                TransactionController.Create(new TransactionViewModel
                {
                    TransactionModule = "InquiryCard",
                    TransactionCode = inquiryCard.Code,
                    TransactionName = "Hoàn phiếu yêu cầu"
                });

                TransactionController.Create(new TransactionViewModel
                {
                    TransactionModule = "MaterialOutbound",
                    TransactionCode = MaterialInbound.Code,
                    TransactionName = "Nhập kho vật tư cho dịch vụ"
                });

                //Thêm chứng từ liên quan
                TransactionController.CreateRelationship(new TransactionRelationshipViewModel
                {
                    TransactionA = productInbound.Code,
                    TransactionB = MaterialInbound.Code
                });
            }
            catch (Exception)
            {

                throw;
            }

        }
        #region quy đổi liệu trình
        [HttpGet]
        public ActionResult ExchangeAllMBS(int? TargetId, string TargetModule)
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

            var model = new MembershipViewModel();
            model.Code = Erp.BackOffice.Helpers.Common.GetOrderNo("Membership");
            model.BranchId = intBrandID;
            model.TargetId = TargetId;
            model.TargetModule = TargetModule;
            if (TargetId != null)
            {
                if (model.TargetModule == "ProductInvoiceDetail")
                {
                    var info = productInvoiceRepository.GetvwProductInvoiceDetailById(model.TargetId.Value);
                    model.TargetCode = info.ProductInvoiceCode;
                    model.Type = info.Type;
                    model.ProductId = info.ProductId;
                    model.CustomerId = info.CustomerId;
                    model.ExpiryDate = info.ExpiryDate;
                }
                if (model.TargetModule == "Membership")
                {
                    var info = MembershipRepository.GetvwMembershipById(model.TargetId.Value);
                    model.TargetCode = info.Code;
                    model.Type = info.Type;
                    model.ProductId = info.ProductId;
                    model.ExpiryDate = info.ExpiryDate;
                }
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult ExchangeAllMBS(MembershipViewModel model, bool? IsPopup)
        {
            IsPopup = true;
            if (ModelState.IsValid)
            {
                using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.Snapshot }))
                {
                    var Membershipold = MembershipRepository.GetMembershipById(model.TargetId.Value);
                    Membershipold.Status = "tranfer";
                    MembershipRepository.UpdateMembership(Membershipold);
                    var Membershipnew = new Erp.Domain.Sale.Entities.Membership();
                    //AutoMapper.Mapper.Map(Membershipnew, Membershipold);
                    Membershipnew.Status = "receive";
                    Membershipnew.CustomerId = model.CustomerId;
                    Membershipnew.ProductId = model.ProductId;
                    var Cus = customerRepository.GetCustomerById(Membershipold.CustomerId.Value);
                    var MembershipoldParent = Membership_parentRepository.GetMembership_parentById(Membershipold.IdParent);

                    //cong -- begin tạo mới MBS parent
                    var ins = new Membership_parent();
                    ins.IsDeleted = false;
                    ins.CreatedUserId = WebSecurity.CurrentUserId;
                    ins.ModifiedUserId = WebSecurity.CurrentUserId;
                    ins.AssignedUserId = WebSecurity.CurrentUserId;
                    ins.CreatedDate = DateTime.Now;
                    ins.ModifiedDate = DateTime.Now;
                    ins.CustomerId = Membershipold.CustomerId.Value;
                    ins.ProductInvoiceId = 0;
                    ins.Status = "pending";
                    ins.Code = MembershipoldParent.Code + "/" + Cus.Code;
                    ins.QRCode = MembershipoldParent.QRCode + "/" + Cus.Code;
                    ins.Note = "Được chuyển";
                    ins.SerialNumber = MembershipoldParent.SerialNumber;
                    ins.ExpiryDate = MembershipoldParent.ExpiryDate;
                    ins.Total = 1;
                    //ins.isPrint = 
                    //ins.NumberPrint = item.NumberPrint;
                    ins.BranchId = model.BranchId.Value;
                    ins.is_extend = 0; //?????
                    var checkMSP = Membership_parentRepository.GetListMembership_parentByIdcustomer(Membershipold.CustomerId.Value).Where(x => x.Code == ins.Code).ToList();
                    if (checkMSP.Count == 0)
                    {
                        Membership_parentRepository.InsertMembership_parent(ins);
                        Membershipnew.IdParent = ins.Id;
                    }
                    else
                    {
                        long idparent = checkMSP[0].Id;
                        Membershipnew.IdParent = idparent;
                        var isn2 = Membership_parentRepository.GetMembership_parentById(idparent);
                        isn2.Total = isn2.Total + 1;
                        Membership_parentRepository.UpdateMembership_parent(isn2);
                    }


                    //var mebershipparent = MembershipRepository.GetAllMembershipByIdParent(idparent).First();
                    Membershipnew.IsDeleted = false;
                    Membershipnew.CreatedUserId = WebSecurity.CurrentUserId;
                    Membershipnew.ModifiedUserId = WebSecurity.CurrentUserId;
                    Membershipnew.AssignedUserId = WebSecurity.CurrentUserId;
                    Membershipnew.CreatedDate = DateTime.Now;
                    Membershipnew.ModifiedDate = DateTime.Now;
                    Membershipnew.CustomerId = Membershipold.CustomerId;
                    Membershipnew.ProductId = model.ProductId;
                    Membershipnew.Status = "receive";

                    Membershipnew.TargetModule = "ProductInvoiceDetail";
                    Membershipnew.TargetId = Membershipold.TargetId;
                    Membershipnew.TargetCode = Membershipold.Code;
                    Membershipnew.BranchId = model.BranchId;
                    Membershipnew.Type = "SkinScan";
                    Membershipnew.ExpiryDate = model.ExpiryDate;
                    Membershipnew.Is_extend = 0;
                    Membershipnew.TongLanCSD = 1;


                    Membershipnew.Code = Erp.BackOffice.Helpers.Common.GetOrderNo("Membership", model.Code);
                    //MembershipRepository.UpdateMembership(Membershipnew);
                    MembershipRepository.InsertMembership(Membershipnew);
                    Erp.BackOffice.Helpers.Common.SetOrderNo("Membership");


                    scope.Complete();

                }

                if (IsPopup == true)
                {
                    return RedirectToAction("_ClosePopup", "Home", new { area = "", FunctionCallback = "ClosePopupAndReloadPage" });
                }
                else
                {
                    TempData[Globals.SuccessMessageKey] = App_GlobalResources.Wording.InsertSuccess;
                    return RedirectToAction("Index");
                }
            }
            return View(model);
        }
        #endregion

        #endregion



        #region Đổi MBS - cong
        public ActionResult ChangeMBS()
        {

            return View();
        }

        [HttpPost]
        public JsonResult ChangeMBS(int CustomerId, List<int> SerViceOld, List<MemberReceipt> SerViceNew)
        {
            try
            {
                //  Block of code to try

                var Cus = customerRepository.GetCustomerById(CustomerId);
                // phiếu cũ
                foreach (var i in SerViceOld)
                {
                    var Membershipold = MembershipRepository.GetMembershipById(i);
                    Membershipold.Status = "tranfer";
                    MembershipRepository.UpdateMembership(Membershipold);
                }



                //cong -- begin tạo mới MBS parent
                var ins = new Membership_parent();
                ins.IsDeleted = false;
                ins.CreatedUserId = WebSecurity.CurrentUserId;
                ins.ModifiedUserId = WebSecurity.CurrentUserId;
                ins.AssignedUserId = WebSecurity.CurrentUserId;
                ins.CreatedDate = DateTime.Now;
                ins.ModifiedDate = DateTime.Now;
                ins.CustomerId = CustomerId;
                ins.ProductInvoiceId = 0;
                ins.Status = "pending";
                ins.Code = Cus.Code + "/" + "CHUYEN" + DateTime.Now;
                ins.QRCode = Cus.Code + "/" + "CHUYEN" + DateTime.Now;
                ins.Note = "Được chuyển";
                ins.SerialNumber = Cus.Code + "/" + "CHUYEN" + DateTime.Now;
                //ins.ExpiryDate = //MembershipoldParent.ExpiryDate;
                ins.Total = SerViceNew.Sum(x => x.Quantity);
                //ins.isPrint = 
                //ins.NumberPrint = item.NumberPrint;
                ins.BranchId = Cus.BranchId.Value;
                ins.is_extend = 0; //?????
                var checkMSP = Membership_parentRepository.GetListMembership_parentByIdcustomer(CustomerId).Where(x => x.Code == ins.Code).ToList();
                if (checkMSP.Count == 0)
                {
                    Membership_parentRepository.InsertMembership_parent(ins);
                    //Membershipnew.IdParent = ins.Id;
                }

                //ghi log 
                Erp.BackOffice.Controllers.HomeController.WriteLog(ins.Id, ins.Code, "đã chuyển đổi MBS", "Membership/MembershipIndex/" + ins.Id, Helpers.Common.CurrentUser.BranchId.Value);

                //else
                //{
                //    long idparent = checkMSP[0].Id;
                //   // Membershipnew.IdParent = idparent;
                //    var isn2 = Membership_parentRepository.GetMembership_parentById(idparent);
                //    isn2.Total = isn2.Total + 1;
                //    Membership_parentRepository.UpdateMembership_parent(isn2);
                //}

                foreach (var item in SerViceNew)
                {
                    for (int i = 0; i < item.Quantity; i++)
                    {
                        var Membership = new Erp.Domain.Sale.Entities.Membership();
                        Membership.IsDeleted = false;
                        Membership.CreatedUserId = WebSecurity.CurrentUserId;
                        Membership.ModifiedUserId = WebSecurity.CurrentUserId;
                        Membership.AssignedUserId = WebSecurity.CurrentUserId;
                        Membership.CreatedDate = DateTime.Now;
                        Membership.ModifiedDate = DateTime.Now;
                        Membership.CustomerId = CustomerId;
                        Membership.ProductId = item.ProductId;
                        Membership.Status = "receive";
                        Membership.IdParent = ins.Id;
                        Membership.TargetModule = "ProductInvoiceDetail";
                        Membership.TargetId = 0;
                        Membership.TargetCode = "DUOCCHUYEN";
                        Membership.BranchId = Cus.BranchId;//productInvoice.BranchId;
                        Membership.Type = "SkinScan";
                        // Membership.ExpiryDate = ;
                        Membership.Is_extend = 0;
                        Membership.TongLanCSD = item.Quantity;
                        MembershipRepository.InsertMembership(Membership);

                        Membership.Code = Erp.BackOffice.Helpers.Common.GetOrderNo("Membership");
                        MembershipRepository.UpdateMembership(Membership);
                        Erp.BackOffice.Helpers.Common.SetOrderNo("Membership");
                    }
                }

                return Json(1);
            }
            catch (Exception e)
            {
                return Json(e.Message);//  Block of code to handle errors
            }
        }


        public JsonResult getCustomer()
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

            var q = customerRepository.GetAllCustomer().Where(x => x.BranchId == intBrandID && x.isLock != true).ToList();
            return Json(q.Select(i => new { id = i.Id, text = i.CompanyName + '-' + i.Code, value = i.Id, Code = i.Code, Name = i.CompanyName }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetMembershipByCus(int CustomerId)
        {
            var q = MembershipRepository.GetvwAllMembership().Where(x => x.CustomerId == CustomerId && x.Status == "pending").ToList();
            return Json(q.Select(i => new { id = i.Id, text = i.Code + '-' + i.ProductName, value = i.Id, Code = i.Code, productId = i.ProductId, productCode = i.ProductCode, productName = i.ProductName }), JsonRequestBehavior.AllowGet);
        }


        public JsonResult getService()
        {
            var q = productOrServiceRepository.GetAllvwService().Where(i => i.NotUse == false || i.NotUse == null).ToList();
            return Json(q.Select(i => new { id = i.Id, text = i.Code + "-" + i.Name, value = i.Id, code = i.Code, name = i.Name }), JsonRequestBehavior.AllowGet);
        }


        public ActionResult LoadServiceItem(int OrderNo, int ProductId, int Quantity, string ProductCode, string ProductName)
        {
            var model = new ProductInboundDetailViewModel();
            model.OrderNo = OrderNo;
            model.ProductId = ProductId;
            model.ProductCode = ProductCode;
            model.Quantity = Quantity;
            model.ProductName = ProductName;
            return View(model);
        }
        #endregion
    }



}

