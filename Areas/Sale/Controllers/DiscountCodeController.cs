using System.Globalization;
using Erp.BackOffice.Sale.Models;
using Erp.BackOffice.Staff.Models;
using Erp.BackOffice.Filters;
using Erp.Domain.Entities;
using Erp.Domain.Interfaces;
using Erp.Domain.Sale.Entities;
using Erp.Domain.Sale.Interfaces;
using Erp.Domain.Staff.Entities;
using Erp.Domain.Staff.Interfaces;
using System;
using System.IO;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using Erp.Utilities;
using WebMatrix.WebData;
using Erp.BackOffice.Helpers;

using System.Transactions;
using System.Web.Script.Serialization;
using Erp.Domain.Account.Interfaces;
using Erp.Domain.Account.Helper;
using Erp.BackOffice.Account.Models;
using System.Web;
using Excel;
using System.Data;


namespace Erp.BackOffice.Sale.Controllers
{
    [Authorize]
    [InitializeSimpleMembership]
    [Helpers.NoCacheHelper]
    public class DiscountCodeController : Controller
    {
        private readonly ICommissionCusRepository CommissionCusRepository;
        private readonly IUserRepository userRepository;

        private readonly IProductOrServiceRepository productRepository;
        private readonly ICommisionCustomerRepository CommissionDetailRepository;
        private readonly IBranchRepository branchRepository;
        private readonly IDM_DEALHOTRepositories dm_dealhotRepository;
        private readonly IDM_BANNER_SLIDERRepositories dm_bannersliderRepository;
        private readonly IObjectAttributeRepository ObjectAttributeRepository;
        private readonly ILoyaltyPointRepository LoyaltyPoint;
        private readonly ICustomerRepository customerRepository;
        private readonly IDM_NHOMSANPHAMRepositories dM_NHOMSANPHAMRepositories;

        private readonly ITemplatePrintRepository templatePrintRepository;
        private readonly IDiscountCodeRepository discountCodeRepository;

        public DiscountCodeController(
            ICommissionCusRepository _CommissionCus
            , IUserRepository _user

             , IProductOrServiceRepository _Product
            , ICommisionCustomerRepository CommissionDetail
        , IBranchRepository branch
            , IDM_DEALHOTRepositories dm_dealhot
            , IDM_BANNER_SLIDERRepositories bannerslider
             , IObjectAttributeRepository _ObjectAttribute
            , ILoyaltyPointRepository _LoyaltyPoint
            , ICustomerRepository _customerRepository
            , IDM_NHOMSANPHAMRepositories _DM_NHOMSANPHAMRepositories

            , ITemplatePrintRepository templatePrint
            , IDiscountCodeRepository _discountCode
             )
        {
            CommissionCusRepository = _CommissionCus;

            userRepository = _user;
            productRepository = _Product;
            CommissionDetailRepository = CommissionDetail;
            branchRepository = branch;
            dm_dealhotRepository = dm_dealhot;
            dm_bannersliderRepository = bannerslider;
            LoyaltyPoint = _LoyaltyPoint;
            customerRepository = _customerRepository;
            dM_NHOMSANPHAMRepositories = _DM_NHOMSANPHAMRepositories;

            templatePrintRepository = templatePrint;
            discountCodeRepository = _discountCode;
        }

        #region Index
        public ViewResult Index(string txtSearch, string Ma, string txtSearch2)
        {
            //string pMA_DVIQLY = Helpers.Common.CurrentUser.MA_DVIQLY;
            IEnumerable<DiscountCodeViewModel> q = discountCodeRepository.GetAllDiscountCode()
                .Select(item => new DiscountCodeViewModel
                {
                    Id = item.Id,
                    CreatedUserId = item.CreatedUserId,

                    CreatedDate = item.CreatedDate,
                    ModifiedUserId = item.ModifiedUserId,

                    ModifiedDate = item.ModifiedDate,
                    Name = item.Name,
                    Code = item.Code,
                    //ApplyFor = item.ApplyFor,
                    EndDate = item.EndDate,
                    Note = item.Note,
                    StartDate = item.StartDate,
                    //Type = item.Type,
                    Status = item.Status,
                    Discount = item.Discount,
                    DiscountMoney = item.DiscountMoney

                }).OrderByDescending(x => x.CreatedDate).ToList();
            //if (Ma != null )
            //{
            //    q = q.Where(x => x.Code == Ma).ToList();
            //}
            if (string.IsNullOrEmpty(txtSearch) == false)
            {
                txtSearch = txtSearch == "" ? "~" : Erp.BackOffice.Helpers.Common.ChuyenThanhKhongDau(txtSearch);

                q = q.Where(x => Erp.BackOffice.Helpers.Common.ChuyenThanhKhongDau(x.Name).Contains(txtSearch)).ToList();
            }
            if (string.IsNullOrEmpty(txtSearch2) == false)
            {
                txtSearch2 = txtSearch2 == "" ? "~" : Erp.BackOffice.Helpers.Common.ChuyenThanhKhongDau(txtSearch2);
                q = q.Where(x => Erp.BackOffice.Helpers.Common.ChuyenThanhKhongDau(x.Code).Contains(txtSearch2)).ToList();
            }
            ViewBag.SuccessMessage = TempData[Globals.SuccessMessageKey];
            ViewBag.FailedMessage = TempData[Globals.FailedMessageKey];
            return View(q);

        }
        #region ExportExcel
        public List<DiscountCodeViewModel> IndexExport(string txtSearch, string Ma, string txtSearch2)
        {
            var q = discountCodeRepository.GetAllDiscountCode()
                .Select(item => new DiscountCodeViewModel
                {
                    Id = item.Id,
                    CreatedUserId = item.CreatedUserId,

                    CreatedDate = item.CreatedDate,
                    ModifiedUserId = item.ModifiedUserId,

                    ModifiedDate = item.ModifiedDate,
                    Name = item.Name,
                    Code = item.Code,
                    //ApplyFor = item.ApplyFor,
                    EndDate = item.EndDate,
                    Note = item.Note,
                    StartDate = item.StartDate,
                    //Type = item.Type,
                    Status = item.Status,
                    Discount = item.Discount,
                    DiscountMoney = item.DiscountMoney

                }).OrderByDescending(x => x.CreatedDate).ToList();

            if (string.IsNullOrEmpty(txtSearch) == false)
            {
                txtSearch = txtSearch == "" ? "~" : Erp.BackOffice.Helpers.Common.ChuyenThanhKhongDau(txtSearch);

                q = q.Where(x => Erp.BackOffice.Helpers.Common.ChuyenThanhKhongDau(x.Name).Contains(txtSearch)).ToList();
            }
            if (string.IsNullOrEmpty(txtSearch2) == false)
            {
                txtSearch2 = txtSearch2 == "" ? "~" : Erp.BackOffice.Helpers.Common.ChuyenThanhKhongDau(txtSearch2);
                q = q.Where(x => Erp.BackOffice.Helpers.Common.ChuyenThanhKhongDau(x.Code).Contains(txtSearch2)).ToList();
            }

            return q;
        }

        public ActionResult ExportExcel(string txtSearch, string Ma, string txtSearch2, bool ExportExcel = false)
        {
            var data = IndexExport(txtSearch, Ma, txtSearch2);

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
            model.Content = model.Content.Replace("{Title}", "Danh Sách Dịch Vụ");
            if (ExportExcel)
            {
                Response.AppendHeader("content-disposition", "attachment;filename=" + "DS_Dichvu" + DateTime.Now.ToString("yyyyMMdd") + ".xls");
                Response.Charset = "utf-8";
                Response.Cache.SetCacheability(HttpCacheability.NoCache);
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.Write(model.Content);
                Response.End();
            }
            return View(model);
        }

        string buildHtml(List<DiscountCodeViewModel> data)
        {
            //Tạo table html chi tiết phiếu xuất
            string detailLists = "<table border=\"1\" class=\"invoice-detail\">\r\n";
            detailLists += "<thead>\r\n";
            detailLists += "	<tr>\r\n";
            detailLists += "		<th>Tên</th>\r\n";
            detailLists += "		<th>Mã </th>\r\n";
            detailLists += "		<th>Ngày bắt đầu</th>\r\n";
            detailLists += "		<th>Ngày kết thúc</ th>\r\n";
            detailLists += "		<th>Ngày tạo</th>\r\n";
            detailLists += "		<th>Trạng thái</ th>\r\n";
            detailLists += "		<th>Ngày cập nhật</th>\r\n";
            detailLists += "	</tr>\r\n";
            detailLists += "</thead>\r\n";
            detailLists += "<tbody>\r\n";
            var index = 1;

            foreach (var item in data)
            {
                detailLists += "<tr>\r\n"
                + "<td class=\"text-center orderNo\">" + (index++) + "</td>\r\n"
                + "<td class=\"text-left \">" + item.Name + "</td>\r\n"
                + "<td class=\"text-left \">" + item.StartDate + "</td>\r\n"
                 + "<td class=\"text-left \">" + item.EndDate + "</td>\r\n"
                  + "<td class=\"text-left \">" + item.CreatedDate + "</td>\r\n"
                   + "<td class=\"text-left \">" + (item.Status == 1 ? "Đã kích hoạt" : "Chưa kích hoạt") + "</td>\r\n"
                    + "<td class=\"text-left \">" + item.ModifiedDate + "</td>\r\n"
                + "</tr>\r\n";
            }

            detailLists += "</tfoot>\r\n</table>\r\n";

            return detailLists;
        }
        #endregion

        public ActionResult ChangeStatus(int userId)
        {
            //string pMA_DVIQLY = Helpers.Common.CurrentUser.MA_DVIQLY;
            if (ModelState.IsValid)
            {
                var user = discountCodeRepository.GetDiscountCodeById(userId);
                if (user.Status == 1)
                {
                    user.Status = 2;
                    discountCodeRepository.UpdateDiscountCode(user);
                    return RedirectToAction("/Index");
                }
                if (user.Status == 2)
                {
                    user.Status = 1;
                    discountCodeRepository.UpdateDiscountCode(user);
                    return RedirectToAction("/Index");
                }


            }
            return RedirectToAction("/Index");
        }
        #endregion

        #region Create
        public ActionResult Create(int? Id)
        {
            List<int> Vip = new List<int>();
            List<int> Cus = new List<int>();
            var model = new DiscountCodeViewModel();
            //string pMA_DVIQLY = Helpers.Common.CurrentUser.MA_DVIQLY;

            if (Id > 0)
            {
                var discountmodel = discountCodeRepository.GetDiscountCodeById(Id.Value);
                if (discountmodel != null)
                {
                    AutoMapper.Mapper.Map(discountmodel, model);
                    if (model.ApplyFor == "2")
                    {
                        Vip = model.ApplyUser.Split(',').Select(Int32.Parse).ToList();
                    }
                    if (model.ApplyFor == "3")
                    {
                        Cus = model.ApplyUser.Split(',').Select(Int32.Parse).ToList();
                    }
                }
            }
            //
            var loyaltypointList = LoyaltyPoint.GetAllLoyaltyPoint().Select(item => new LoyaltyPointViewModel
            {
                Id = item.Id,
                IsDeleted = item.IsDeleted,
                CreatedDate = item.CreatedDate,
                CreatedUserId = item.CreatedUserId,
                Name = item.Name,
                MinMoney = item.MinMoney,
                PlusPoint = item.PlusPoint,
                //MA_DVIQLY = pMA_DVIQLY
            }).ToList();
            //
            HttpRequestBase request = this.HttpContext.Request;
            string strBrandID = "0";
            if (request.Cookies["BRANCH_ID_CookieName"] != null)
            {
                strBrandID = request.Cookies["BRANCH_ID_CookieName"].Value;
                if (strBrandID == "")
                {
                    strBrandID = "0";
                }
            }



            if ((Helpers.Common.CurrentUser.BranchId != null) && (Helpers.Common.CurrentUser.BranchId > 0))
            {
                strBrandID = Helpers.Common.CurrentUser.BranchId.ToString();
            }

            int? intBrandID = int.Parse(strBrandID);

            //spGetAllCustomerByBranch trong stored hiện tại đã lọc theo tất cả chi nhanh
            var CustomerList = SqlHelper.QuerySP<CustomerViewModel>("spGetAllCustomerByBranch", new
            {
                //pMA_DVIQLY = pMA_DVIQLY,
                BranchId = intBrandID.Value
            }).ToList();

            foreach (var item in CustomerList)
            {
                item.FullName = item.FirstName + " " + item.LastName;
                item.Mobile = item.Mobile;
            }
            ViewBag.Vip = Vip;
            ViewBag.Cus = Cus;
            ViewBag.loyaltypointList = loyaltypointList;
            ViewBag.CustomerList = CustomerList;
            return View(model);
        }

        [HttpPost]
        public ActionResult Create(DiscountCodeViewModel model)
        {
            //string pMA_DVIQLY = Helpers.Common.CurrentUser.MA_DVIQLY;
            var ApplyFor = Request["single"];
            var ApplyForVip = Request["ApplyForVip"];
            var ApplyForCustomer = Request["ForCustomer_hiden"];
            var App = Request["ApplyFor"];
            if (ApplyForVip != "")
            {
                ApplyForVip = ApplyForVip.Substring(1);
            }

            if (ApplyForCustomer != "")
            {
                ApplyForCustomer = ApplyForCustomer.Substring(1);
            }

            if (ModelState.IsValid)
            {
                using (var scope = new TransactionScope(TransactionScopeOption.Required, new System.TimeSpan(0, 30, 0)))
                {
                    var DiscountCode = new DiscountCode();
                    if (model.Id > 0)
                    {
                        DiscountCode = discountCodeRepository.GetDiscountCodeById(model.Id);
                        AutoMapper.Mapper.Map(model, DiscountCode);
                        // DiscountCode.MA_DVIQLY = pMA_DVIQLY;
                        DiscountCode.ApplyFor = ApplyFor;
                        DiscountCode.Status = 1;
                        DiscountCode.IsDeleted = false;
                        DiscountCode.ModifiedDate = DateTime.Now;
                        DiscountCode.ModifiedUserId = WebSecurity.CurrentUserId;
                        //ApplyFor = 1 (Áp dụng cho tất cả KH)
                        //Áp dụng cho nhóm khách
                        if (ApplyFor == "2")
                        {
                            DiscountCode.ApplyUser = ApplyForVip;
                        }
                        //Ap dung cho khách hàng lua chon
                        if (ApplyFor == "3")
                        {
                            DiscountCode.ApplyUser = ApplyForCustomer;
                        }
                        discountCodeRepository.UpdateDiscountCode(DiscountCode);
                        Erp.BackOffice.Controllers.HomeController.WriteLog(DiscountCode.Id, DiscountCode.Code, "đã cập nhật mã giảm giá ", "DiscountCode/Edit/" + DiscountCode.Id, Helpers.Common.CurrentUser.BranchId.Value);
                    }
                    else
                    {
                        var dis = discountCodeRepository.GetAllDiscountCode().Where(x => x.Status == 1 && x.Code == model.Code).FirstOrDefault();
                        if (dis != null)
                        {
                            TempData[Globals.FailedMessageKey] = "Mã đang được sử dụng !";
                            return RedirectToAction("_ClosePopup", "Home", new { area = "", FunctionCallback = "ClosePopupAndReloadPage" });
                        }
                        AutoMapper.Mapper.Map(model, DiscountCode);
                        //DiscountCode.MA_DVIQLY = pMA_DVIQLY;
                        DiscountCode.ApplyFor = ApplyFor;
                        DiscountCode.Status = 1;
                        DiscountCode.IsDeleted = false;
                        DiscountCode.CreatedDate = DateTime.Now;
                        DiscountCode.CreatedUserId = WebSecurity.CurrentUserId;

                        //ApplyFor = 1 (Áp dụng cho tất cả KH)
                        //Áp dụng cho nhóm khách
                        if (ApplyFor == "2")
                        {
                            DiscountCode.ApplyUser = ApplyForVip;
                        }
                        //Ap dung cho khách hàng lua chon
                        if (ApplyFor == "3")
                        {
                            DiscountCode.ApplyUser = ApplyForCustomer;
                        }
                        discountCodeRepository.InsertDiscountCode(DiscountCode);
                        Erp.BackOffice.Controllers.HomeController.WriteLog(DiscountCode.Id, DiscountCode.Code, "đã cập nhật mã giảm giá ", "DiscountCode/Edit/" + DiscountCode.Id, Helpers.Common.CurrentUser.BranchId.Value);
                    }
                    scope.Complete();
                }
            }

            //  return RedirectToAction("Index");
            TempData[Globals.SuccessMessageKey] = "Tạo mới thành công !";
            return RedirectToAction("_ClosePopup", "Home", new { area = "", FunctionCallback = "ClosePopupAndReloadPage" });
        }
        #endregion



        #region Delete
        [HttpPost]
        public ActionResult Delete()
        {
            try
            {
                //  string pMA_DVIQLY = Helpers.Common.CurrentUser.MA_DVIQLY;
                string idDeleteAll = Request["DeleteId-checkbox"];
                string[] arrDeleteId = idDeleteAll.Split(',');
                using (var scope = new TransactionScope(TransactionScopeOption.Required, new System.TimeSpan(0, 30, 0)))
                {
                    try
                    {
                        for (int i = 0; i < arrDeleteId.Count(); i++)
                        {
                            var item = discountCodeRepository.GetDiscountCodeById(int.Parse(arrDeleteId[i], CultureInfo.InvariantCulture));
                            if (item != null)
                            {

                                if (item.Status == 1 || item.Status == 2)
                                {
                                    TempData["FailedMessage"] = "Không thể xóa chương trình đã kích hoạt hoặc đang kích hoạt";
                                    return RedirectToAction("/Index");
                                }

                                if (item.CreatedUserId != Helpers.Common.CurrentUser.Id && Helpers.Common.CurrentUser.UserTypeId != 1)
                                {
                                    TempData["FailedMessage"] = "NotOwner";
                                    return RedirectToAction("/Index");
                                }

                                item.IsDeleted = true;
                                discountCodeRepository.UpdateDiscountCode(item);


                            }
                        }
                        TempData[Globals.SuccessMessageKey] = App_GlobalResources.Wording.DeleteSuccess;
                        scope.Complete();
                    }
                    catch (Exception ex)
                    {
                        return Content("Fail");
                    }
                }
                return RedirectToAction("/Index");
            }
            catch (Exception ex)
            {
                TempData[Globals.FailedMessageKey] = App_GlobalResources.Error.RelationError;
                return RedirectToAction("/Index");
            }
        }
        #endregion
    }
}
