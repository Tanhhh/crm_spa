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
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Erp.Utilities;
using WebMatrix.WebData;
using Erp.BackOffice.Helpers;
using Newtonsoft.Json;
using System.Web;
using Erp.Domain.Helper;

namespace Erp.BackOffice.Sale.Controllers
{
    [Authorize]
    [InitializeSimpleMembership]
    [Erp.BackOffice.Helpers.NoCacheHelper]
    public class CommissionCusController : Controller
    {
        private readonly ICommissionCusRepository CommissionCusRepository;
        private readonly IUserRepository userRepository;
        private readonly IProductOrServiceRepository productRepository;
        private readonly ICommisionCustomerRepository CommissionDetailRepository;
        private readonly IBranchRepository branchRepository;
        private readonly ICommisionInvoiceRepository commissionInvoiceRepository;
        private readonly IDonateProOrSerRepository donateProductOrServiceRepository;
        private readonly ILogPromotionRepository logPromotionRepository;
        private readonly ITemplatePrintRepository templatePrintRepository;
        private readonly IProductInvoiceRepository invoiceRepository;

        public CommissionCusController(
            ICommissionCusRepository _CommissionCus
            , IUserRepository _user
             , IProductOrServiceRepository _Product
            , ICommisionCustomerRepository CommissionDetail
            , IBranchRepository branch
            , ICommisionInvoiceRepository commissionInvoice
            , IDonateProOrSerRepository donateProductOrService
             , ILogPromotionRepository logPromotion
             , ITemplatePrintRepository templatePrint
            , IProductInvoiceRepository invoice
            )
        {
            CommissionCusRepository = _CommissionCus;
            userRepository = _user;
            productRepository = _Product;
            CommissionDetailRepository = CommissionDetail;
            branchRepository = branch;
            commissionInvoiceRepository = commissionInvoice;
            donateProductOrServiceRepository = donateProductOrService;
            logPromotionRepository = logPromotion;
            templatePrintRepository = templatePrint;
            invoiceRepository = invoice;
        }
        #region List Category
        //IEnumerable<CommissionCusViewModel> getCommission(string ApplyFor)
        //{
        //    IEnumerable<CommissionCusViewModel> listCategory = new List<CommissionCusViewModel>();
        //    var model = CommissionCusRepository.GetAllCommissionCus()
        //        .Where(item => item.ApplyFor == ApplyFor)
        //        .OrderByDescending(m => m.CreatedDate).ToList();

        //    listCategory = AutoMapper.Mapper.Map(model, listCategory);

        //    return listCategory;
        //}

        //public ViewResult CommissionDrugStore()
        //{
        //    string ApplyFor = "DrugStore";
        //    ViewBag.SuccessMessage = TempData[Globals.SuccessMessageKey];
        //    ViewBag.FailedMessage = TempData[Globals.FailedMessageKey];
        //    ViewBag.ApplyFor = ApplyFor;
        //    ViewBag.ActionName = "CommissionDrugStore";

        //    return View("Index", getCommission(ApplyFor));
        //}

        //public ViewResult CommissionCustomer()
        //{
        //    string ApplyFor = "Customer";
        //    ViewBag.SuccessMessage = TempData[Globals.SuccessMessageKey];
        //    ViewBag.FailedMessage = TempData[Globals.FailedMessageKey];
        //    ViewBag.ApplyFor = ApplyFor;
        //    ViewBag.ActionName = "CommissionCustomer";

        //    return View("Index", getCommission(ApplyFor));
        //}

        public ViewResult Index(string txtSearch, string Type)
        {
            var user = userRepository.GetByvwUserName(Helpers.Common.CurrentUser.UserName);
            IEnumerable<CommissionCusViewModel> q = CommissionCusRepository.GetAllCommissionCus()
                .Select(item => new CommissionCusViewModel
                {
                    Id = item.Id,
                    CreatedUserId = item.CreatedUserId,
                    //CreatedUserName = item.CreatedUserName,
                    CreatedDate = item.CreatedDate,
                    ModifiedUserId = item.ModifiedUserId,
                    //ModifiedUserName = item.ModifiedUserName,
                    ModifiedDate = item.ModifiedDate,
                    Name = item.Name,
                    ApplyFor = item.ApplyFor,
                    EndDate = item.EndDate,
                    Note = item.Note,
                    StartDate = item.StartDate,
                    Type = item.Type,
                    TyleHuong = item.TyleHuong,

                }).OrderByDescending(m => m.ModifiedDate);

            if (string.IsNullOrEmpty(txtSearch) == false/* || string.IsNullOrEmpty(Type) == false*/)
            {
                txtSearch = txtSearch == "" ? "~" : Helpers.Common.ChuyenThanhKhongDau(txtSearch);
                //Type = Type == "" ? "~" : Type.ToLower();
                //txtPhone = txtPhone == "" ? "~" : txtPhone.ToLower();
                //txtEmail = txtEmail == "" ? "~" : txtEmail.ToLower();
                q = q.Where(x => Helpers.Common.ChuyenThanhKhongDau(x.Name).Contains(txtSearch)/* || x.Type.ToLowerOrEmpty().Contains(Type)*/);
            }
            //if (!Filters.SecurityFilter.IsAdmin() && !Filters.SecurityFilter.IsKeToan())
            //{
            //    q = q.Where(item => ("," + user.DrugStore + ",").Contains("," + item.ApplyFor + ",") == true);
            //}
            ViewBag.SuccessMessage = TempData["SuccessMessage"];
            ViewBag.FailedMessage = TempData["FailedMessage"];
            ViewBag.AlertMessage = TempData["AlertMessage"];
            return View(q);
        }
        #endregion
        string buildHtml(List<CommissionCusViewModel> data)
        {
            //Tạo table html chi tiết phiếu xuất
            string detailLists = "<table border=\"1\" class=\"invoice-detail\">\r\n";
            detailLists += "<thead>\r\n";
            detailLists += "	<tr>\r\n";
            detailLists += "		<th>Tên chương trình khuyến mãi</th>\r\n";
            detailLists += "		<th>Ngày bắt đầu</th>\r\n";
            detailLists += "		<th>Ngày kết thúc</th>\r\n";
            detailLists += "		<th>Ngày tạo</th>\r\n";
            detailLists += "		<th>Ngày cập nhập</th>\r\n";
            detailLists += "		<th>Hưởng doanh thu (%)</th>\r\n";
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
                + "<td class=\"text-left \">" + item.ModifiedDate + "</td>\r\n"
                + "<td class=\"text-left \">" + item.TyleHuong + "</td>\r\n"
                + "</tr>\r\n";
            }

            detailLists += "</tfoot>\r\n</table>\r\n";

            return detailLists;
        }
        public ActionResult ExportExcel(string Name, string Type, bool ExportExcel = false)
        {
            var data = IndexExport(Name, Type);

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
            model.Content = model.Content.Replace("{Title}", "Danh sách");
            if (ExportExcel)
            {
                Response.AppendHeader("content-disposition", "attachment;filename=" + "DS_" + DateTime.Now.ToString("yyyyMMdd") + ".xls");
                Response.Charset = "";
                Response.Cache.SetCacheability(HttpCacheability.NoCache);
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.Write(model.Content);
                Response.End();
            }
            return View(model);
        }
        public List<CommissionCusViewModel> IndexExport(string txtSearch, string Type)
        {
            var q = CommissionCusRepository.GetAllCommissionCus().AsEnumerable()
                 .Select(item => new CommissionCusViewModel
                 {
                     Id = item.Id,
                     CreatedUserId = item.CreatedUserId,
                     //CreatedUserName = item.CreatedUserName,
                     CreatedDate = item.CreatedDate,
                     ModifiedUserId = item.ModifiedUserId,
                     //ModifiedUserName = item.ModifiedUserName,
                     ModifiedDate = item.ModifiedDate,
                     Name = item.Name,
                     ApplyFor = item.ApplyFor,
                     EndDate = item.EndDate,
                     Note = item.Note,
                     StartDate = item.StartDate,
                     Type = item.Type,
                     TyleHuong = item.TyleHuong,

                 }).OrderByDescending(m => m.ModifiedDate).ToList();

            if (string.IsNullOrEmpty(txtSearch) == false)
            {
                txtSearch = txtSearch == "" ? "~" : Helpers.Common.ChuyenThanhKhongDau(txtSearch);

                q = q.Where(x => Helpers.Common.ChuyenThanhKhongDau(x.Name).Contains(txtSearch)).ToList();
            }

            return q;
        }

        #region Create

        public ViewResult Create(int? Id)
        {

            var model = new CommissionCusViewModel();
            model.DetailList = new List<CommisionCustomerViewModel>();
            model.InvoiceDetailList = new List<CommisionInvoiceViewModel>();
            var departmentList = branchRepository.GetAllBranch().Where(x => x.ParentId == null)
               .Select(item => new BranchViewModel
               {
                   Name = item.Name,
                   Id = item.Id,
                   ParentId = item.ParentId
               }).ToList();
            ViewBag.departmentList = departmentList;
            if (Id != null && Id > 0)
            {
                var CommissionCus = CommissionCusRepository.GetCommissionCusById(Id.Value);
                AutoMapper.Mapper.Map(CommissionCus, model);
                ///
                var detail = CommissionDetailRepository.GetvwAllCommisionCustomer().Where(x => x.CommissionCusId == Id).ToList();
                model.DetailList = detail.Select(x => new CommisionCustomerViewModel
                {
                    ProductId = x.ProductId.Value,
                    Id = x.Id,
                    IsMoney = x.IsMoney,
                    Type = x.Type,
                    CommissionValue = x.CommissionValue,
                    CommissionCusId = x.CommissionCusId.Value,
                    Symbol = x.Symbol,
                    Quantity = x.Quantity,
                    Price = x.Price,
                    ProductCode = x.ProductCode,
                    ProductName = x.ProductName,
                    ExpiryMonth = x.ExpiryMonth,


                }).ToList();
                var data = donateProductOrServiceRepository.GetvwAllDonateProOrSer();
                for (int i = 0; i < model.DetailList.Count(); i++)
                {
                    model.DetailList[i].Index = i;
                    if (model.DetailList[i].Type == "donate")
                    {
                        model.DetailList[i].DonateDetailList = new List<DonateProOrSerViewModel>();

                        var data_donate = data.AsEnumerable().Where(x => x.TargetId == model.DetailList[i].Id && x.TargetModule == "CommisionCustomer").ToList();
                        if (data_donate.Any())
                        {
                            model.DetailList[i].DonateDetailList = data_donate
                                .Select(x => new DonateProOrSerViewModel
                                {
                                    Id = x.Id,
                                    CategoryCode = x.CategoryCode,
                                    ExpriryMonth = x.ExpriryMonth,
                                    Price = x.Price,
                                    ParentOrderNo = i,
                                    ProductCode = x.ProductCode,
                                    ProductId = x.ProductId,
                                    ProductName = x.ProductName,
                                    ProductType = x.ProductType,
                                    Quantity = x.Quantity,
                                    TargetId = x.TargetId,
                                    TargetModule = x.TargetModule,
                                    TotalQuantity = x.TotalQuantity
                                }).ToList();
                        }
                    }
                }
                ///
                var invoice_detail = commissionInvoiceRepository.GetAllCommisionInvoice().Where(x => x.CommissionCusId == Id).ToList();
                model.InvoiceDetailList = invoice_detail.Select(x => new CommisionInvoiceViewModel
                {
                    EndAmount = x.EndAmount,
                    EndSymbol = x.EndSymbol,
                    Id = x.Id,
                    IsMoney = x.IsMoney,
                    IsVIP = x.IsVIP,
                    SalesPercent = x.SalesPercent,
                    StartAmount = x.StartAmount,
                    StartSymbol = x.StartSymbol,
                    Type = x.Type,
                    CommissionCusId = x.CommissionCusId,
                    CommissionValue = x.CommissionValue,
                    Name = x.Name
                }).ToList();

                for (int i = 0; i < model.InvoiceDetailList.Count(); i++)
                {
                    model.InvoiceDetailList[i].Index = i;
                    model.InvoiceDetailList[i].DonateDetailList = new List<DonateProOrSerViewModel>();
                    var data_invoice = data.AsEnumerable().Where(x => x.TargetId == model.InvoiceDetailList[i].Id && x.TargetModule == "CommisionInvoice").ToList();
                    if (data_invoice.Any())
                    {
                        model.InvoiceDetailList[i].DonateDetailList = data_invoice
                            .Select(x => new DonateProOrSerViewModel
                            {
                                Id = x.Id,
                                CategoryCode = x.CategoryCode,
                                ExpriryMonth = x.ExpriryMonth,
                                Price = x.Price,
                                ParentOrderNo = i,
                                ProductCode = x.ProductCode,
                                ProductId = x.ProductId,
                                ProductName = x.ProductName,
                                ProductType = x.ProductType,
                                Quantity = x.Quantity,
                                TargetId = x.TargetId,
                                TargetModule = x.TargetModule,
                                TotalQuantity = x.TotalQuantity
                            }).ToList();
                    }
                }
            }
            var productList = productRepository.GetAllvwProductAndService().Where(item => item.Type != "productpackage" && item.IS_NGUNG_KD != true)
               .Select(item => new ProductViewModel
               {
                   Code = item.Code,
                   Barcode = item.Barcode,
                   Name = item.Name,
                   Id = item.Id,
                   CategoryCode = item.CategoryCode,
                   PriceInbound = item.PriceOutbound,
                   Unit = item.Unit,
                   Image_Name = item.Image_Name
               });
            ViewBag.productList = productList;
            return View(model);
        }

        [HttpPost]
        public ActionResult Create(CommissionCusViewModel model)
        {
            var urlRefer = Request["UrlReferrer"];
            var check = Request["group_choice"];
            var ApplyFor = Request["ApplyFor"];
            if (ModelState.IsValid)
            {
                CommissionCus commissioncus = null;
                if (model.Id > 0)
                {
                    commissioncus = CommissionCusRepository.GetCommissionCusById(model.Id);
                }
                if (commissioncus != null)
                {
                    AutoMapper.Mapper.Map(model, commissioncus);
                    commissioncus.Type = check;
                    commissioncus.ApplyFor = ApplyFor;
                    commissioncus.StartDate = model.StartDate;
                    commissioncus.TyleHuong = model.TyleHuong;

                    if (model.EndDate.HasValue)
                        commissioncus.EndDate = model.EndDate.Value.AddHours(23).AddMinutes(59);
                    commissioncus.ModifiedUserId = WebSecurity.CurrentUserId;
                    commissioncus.ModifiedDate = DateTime.Now;
                }
                else
                {
                    commissioncus = new CommissionCus();
                    AutoMapper.Mapper.Map(model, commissioncus);
                    commissioncus.IsDeleted = false;
                    commissioncus.CreatedUserId = WebSecurity.CurrentUserId;
                    commissioncus.ModifiedUserId = WebSecurity.CurrentUserId;
                    commissioncus.AssignedUserId = WebSecurity.CurrentUserId;
                    commissioncus.CreatedDate = DateTime.Now;
                    commissioncus.ModifiedDate = DateTime.Now;
                    commissioncus.Type = check;


                    commissioncus.ApplyFor = ApplyFor;
                    commissioncus.StartDate = model.StartDate;
                    if (model.EndDate.HasValue)
                        commissioncus.EndDate = model.EndDate.Value.AddHours(23).AddMinutes(59);
                }
                //hàm edit 
                if (model.Id > 0)
                {
                    CommissionCusRepository.UpdateCommissionCus(commissioncus);
                    //add 
                    UpdateCommisionCustomer(model, commissioncus);
                    //
                    UpdateCommisionInvoice(model, commissioncus);
                    //ghi log 
                    Erp.BackOffice.Controllers.HomeController.WriteLog(model.Id, model.Name, "đã cập nhật CTKM", "CommissionCus/Detail/" + model.Id, Helpers.Common.CurrentUser.BranchId.Value);

                }
                else
                {
                    CommissionCusRepository.InsertCommissionCus(commissioncus);
                    //
                    CreateCommisionCustomer(model, commissioncus);
                    //
                    CreateCommisionInvoice(model, commissioncus);
                    //ghi log 
                    Erp.BackOffice.Controllers.HomeController.WriteLog(commissioncus.Id, model.Name, "đã tạo CTKM", "CommissionCus/Detail/" + commissioncus.Id, Helpers.Common.CurrentUser.BranchId.Value);


                }
                if (model.Id > 0)
                {
                    TempData[Globals.SuccessMessageKey] = App_GlobalResources.Wording.UpdateSuccess;
                }
                else
                {
                    TempData[Globals.SuccessMessageKey] = App_GlobalResources.Wording.InsertSuccess;
                }
                if (Request["IsPopup"] == "true" || Request["IsPopup"] == "True")
                {
                    return RedirectToAction("_ClosePopup", "Home", new { area = "", FunctionCallback = "ClosePopupAndReloadPage" });
                }
                //return Redirect(urlRefer);
                return RedirectToAction("Index");
            }
            return View(model);
        }
        #endregion

        #region Edit
        public ActionResult Edit(int? Id)
        {
            var CommissionCus = CommissionCusRepository.GetCommissionCusById(Id.Value);
            if (CommissionCus != null && CommissionCus.IsDeleted != true)
            {
                var model = new CommissionCusViewModel();
                AutoMapper.Mapper.Map(CommissionCus, model);

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
        public ActionResult Edit(CommissionCusViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (Request["Submit"] == "Save")
                {
                    var CommissionCus = CommissionCusRepository.GetCommissionCusById(model.Id);
                    AutoMapper.Mapper.Map(model, CommissionCus);
                    CommissionCus.ModifiedUserId = WebSecurity.CurrentUserId;
                    CommissionCus.ModifiedDate = DateTime.Now;
                    CommissionCusRepository.UpdateCommissionCus(CommissionCus);

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
            var CommissionCus = CommissionCusRepository.GetCommissionCusById(Id.Value);
            if (CommissionCus != null && CommissionCus.IsDeleted != true)
            {
                var model = new CommissionCusViewModel();
                AutoMapper.Mapper.Map(CommissionCus, model);
                model.DetailList = new List<CommisionCustomerViewModel>();
                model.ProductList = new List<ProductViewModel>();
                model.InvoiceDetailList = new List<CommisionInvoiceViewModel>();
                //lấy danh sách chi tiết chiết khấu sản phẩm
                var detail = CommissionDetailRepository.GetvwAllCommisionCustomer().Where(x => x.CommissionCusId == Id).ToList();
                model.DetailList = detail.Select(x => new CommisionCustomerViewModel
                {
                    ProductId = x.ProductId.Value,
                    Id = x.Id,
                    IsMoney = x.IsMoney,
                    Type = x.Type,
                    CommissionValue = x.CommissionValue,
                    CommissionCusId = x.CommissionCusId.Value,
                    Symbol = x.Symbol,
                    Quantity = x.Quantity,
                    Price = x.Price,
                    ProductCode = x.ProductCode,
                    ProductName = x.ProductName,
                    ExpiryMonth = x.ExpiryMonth
                }).ToList();
                /// lấy danh sách tặng sản phẩm/dịch vụ theo sản phẩm
                var data = donateProductOrServiceRepository.GetvwAllDonateProOrSer();
                for (int i = 0; i < model.DetailList.Count(); i++)
                {
                    model.DetailList[i].Index = i;
                    if (model.DetailList[i].Type == "donate")
                    {
                        model.DetailList[i].DonateDetailList = new List<DonateProOrSerViewModel>();

                        var data_donate = data.AsEnumerable().Where(x => x.TargetId == model.DetailList[i].Id && x.TargetModule == "CommisionCustomer").ToList();
                        if (data_donate.Any())
                        {
                            model.DetailList[i].DonateDetailList = data_donate
                                .Select(x => new DonateProOrSerViewModel
                                {
                                    Id = x.Id,
                                    CategoryCode = x.CategoryCode,
                                    ExpriryMonth = x.ExpriryMonth,
                                    Price = x.Price,
                                    ParentOrderNo = i,
                                    ProductCode = x.ProductCode,
                                    ProductId = x.ProductId,
                                    ProductName = x.ProductName,
                                    ProductType = x.ProductType,
                                    Quantity = x.Quantity,
                                    TargetId = x.TargetId,
                                    TargetModule = x.TargetModule,
                                    TotalQuantity = x.TotalQuantity
                                }).ToList();
                        }
                    }
                }
                /// lấy danh sách chi tiết hóa đơn
                var invoice_detail = commissionInvoiceRepository.GetAllCommisionInvoice().Where(x => x.CommissionCusId == Id).ToList();
                model.InvoiceDetailList = invoice_detail.Select(x => new CommisionInvoiceViewModel
                {
                    EndAmount = x.EndAmount,
                    EndSymbol = x.EndSymbol,
                    Id = x.Id,
                    IsMoney = x.IsMoney,
                    IsVIP = x.IsVIP,
                    SalesPercent = x.SalesPercent,
                    StartAmount = x.StartAmount,
                    StartSymbol = x.StartSymbol,
                    Type = x.Type,
                    CommissionCusId = x.CommissionCusId,
                    CommissionValue = x.CommissionValue,
                    Name = x.Name
                }).ToList();
                /// lấy danh sách tặng sản phẩm/dịch vụ theo hóa đơn
                for (int i = 0; i < model.InvoiceDetailList.Count(); i++)
                {
                    model.InvoiceDetailList[i].Index = i;
                    model.InvoiceDetailList[i].DonateDetailList = new List<DonateProOrSerViewModel>();
                    var data_invoice = data.AsEnumerable().Where(x => x.TargetId == model.InvoiceDetailList[i].Id && x.TargetModule == "CommisionInvoice").ToList();
                    if (data_invoice.Any())
                    {
                        model.InvoiceDetailList[i].DonateDetailList = data_invoice
                            .Select(x => new DonateProOrSerViewModel
                            {
                                Id = x.Id,
                                CategoryCode = x.CategoryCode,
                                ExpriryMonth = x.ExpriryMonth,
                                Price = x.Price,
                                ParentOrderNo = i,
                                ProductCode = x.ProductCode,
                                ProductId = x.ProductId,
                                ProductName = x.ProductName,
                                ProductType = x.ProductType,
                                Quantity = x.Quantity,
                                TargetId = x.TargetId,
                                TargetModule = x.TargetModule,
                                TotalQuantity = x.TotalQuantity
                            }).ToList();
                    }
                }
                //lấy danh sách sản phẩm thuộc nhóm đã chọn
                var product = productRepository.GetAllProduct();
                model.ProductList = product.Select(x => new ProductViewModel
                {
                    Id = x.Id,
                    Code = x.Code,
                    Name = x.Name,
                    PriceOutbound = x.PriceOutbound
                }).ToList();
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
                string id = Request["Delete"];
                if (id != null)
                {
                    var item = CommissionCusRepository.GetCommissionCusById(int.Parse(id, CultureInfo.InvariantCulture));
                    if (item != null)
                    {
                        if (item.CreatedUserId != Helpers.Common.CurrentUser.Id && Helpers.Common.CurrentUser.UserTypeId != 1)
                        {
                            TempData["FailedMessage"] = "NotOwner";
                            return RedirectToAction("Index");
                        }

                        item.IsDeleted = true;
                        CommissionCusRepository.UpdateCommissionCus(item);
                    }
                }
                else
                {
                    string idDeleteAll = Request["DeleteId-checkbox"];
                    string[] arrDeleteId = idDeleteAll.Split(',');
                    for (int i = 0; i < arrDeleteId.Count(); i++)
                    {
                        var item = CommissionCusRepository.GetCommissionCusById(int.Parse(arrDeleteId[i], CultureInfo.InvariantCulture));
                        if (item != null)
                        {
                            if (item.CreatedUserId != Helpers.Common.CurrentUser.Id && Helpers.Common.CurrentUser.UserTypeId != 1)
                            {
                                TempData["FailedMessage"] = "NotOwner";
                                return RedirectToAction("Index");
                            }

                            item.IsDeleted = true;
                            CommissionCusRepository.UpdateCommissionCus(item);
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
        #endregion
        public ActionResult Delete_parent(int id)
        {
            try
            {
                var item = CommissionDetailRepository.GetCommisionCustomerById(id);
                var ProductInvoiceId = 0;
                if (item != null)
                {
                    CommissionDetailRepository.DeleteCommisionCustomer(item.Id);
                }
                TempData[Globals.SuccessMessageKey] = App_GlobalResources.Wording.DeleteSuccess;
                return RedirectToAction("Detail", new { Id = id });
            }
            catch (DbUpdateException)
            {
                TempData[Globals.FailedMessageKey] = App_GlobalResources.Error.RelationError;
                return RedirectToAction("Index");
            }
        }
        public ActionResult Delete_parent1(int id)
        {
            try
            {
                var item = commissionInvoiceRepository.GetCommisionInvoiceById(id);
                var ProductInvoiceId = 0;
                if (item != null)
                {
                    commissionInvoiceRepository.DeleteCommisionInvoice(item.Id);
                }
                TempData[Globals.SuccessMessageKey] = App_GlobalResources.Wording.DeleteSuccess;
                return RedirectToAction("Detail", new { Id = id });
            }
            catch (DbUpdateException)
            {
                TempData[Globals.FailedMessageKey] = App_GlobalResources.Error.RelationError;
                return RedirectToAction("Index");
            }
        }

        [AllowAnonymous]
        public JsonResult GetListJsonAll()
        {
            var pathimg = Helpers.Common.GetSetting("product-image-folder");
            var q = productRepository.GetAllvwProductAndService().Where(item => item.Type != "productpackage")
                .Select(item => new
                {
                    item.Id,
                    item.Name,
                    item.Type,
                    item.Code,
                    item.PriceOutbound,
                    item.ProductGroup,
                    item.Unit,
                    item.Image_Name
                })
                .OrderBy(item => item.Name)
                .ToList();
            return Json(q.Select(item => new { item.Id, Code = item.Code, Image = Helpers.Common.KiemTraTonTaiHinhAnhHoapd(item.Image_Name, "product-image-folder", "product", pathimg), Type = item.Type, Unit = item.Unit, Price = item.PriceOutbound, Text = item.Code + " - " + item.Name + " (" + Helpers.Common.PhanCachHangNgan2(item.PriceOutbound) + "/" + item.Unit + ")", Name = item.Name, Value = item.Id }), JsonRequestBehavior.AllowGet);
        }

        #region LoadProductDonate

        public PartialViewResult LoadProductDonate(int OrderNo, int Index, string ProductCode, string ProductName, int ProductId, decimal Price)
        {
            var model = new CommisionCustomerViewModel();
            model.ProductId = ProductId;
            model.ProductCode = ProductCode;
            model.ProductName = ProductName;
            model.Price = Price;
            model.CommissionValue = 0;
            model.IsMoney = false;
            model.OrderNo = OrderNo;
            model.Index = Index;
            model.DonateDetailList = new List<DonateProOrSerViewModel>();

            var productList = productRepository.GetAllvwProductAndService().Where(item => item.Type != "productpackage")
               .Select(item => new ProductViewModel
               {
                   Code = item.Code,
                   Barcode = item.Barcode,
                   Name = item.Name,
                   Id = item.Id,
                   CategoryCode = item.CategoryCode,
                   PriceInbound = item.PriceOutbound,
                   Unit = item.Unit,
                   Image_Name = item.Image_Name
               });
            ViewBag.productList = productList;

            return PartialView(model);
        }
        #endregion

        #region LoadProduct

        public PartialViewResult LoadProduct(int OrderNo, int Index, string ProductCode, string ProductName, int ProductId, decimal Price)
        {
            var model = new CommisionCustomerViewModel();
            model.ProductId = ProductId;
            model.ProductCode = ProductCode;
            model.ProductName = ProductName;
            model.Price = Price;
            model.CommissionValue = 0;
            model.IsMoney = false;
            model.OrderNo = OrderNo;
            model.Index = Index;
            model.DonateDetailList = new List<DonateProOrSerViewModel>();
            return PartialView(model);
        }
        #endregion

        #region LoadInvoiceDonate

        public PartialViewResult LoadInvoiceDonate(int OrderNo, int Index)
        {
            var model = new CommisionInvoiceViewModel();
            model.CommissionValue = 0;
            model.IsMoney = false;
            model.OrderNo = OrderNo;
            model.Index = Index;
            model.DonateDetailList = new List<DonateProOrSerViewModel>();

            var productList = productRepository.GetAllvwProductAndService().Where(item => item.Type != "productpackage" && item.IS_NGUNG_KD != true)
              .Select(item => new ProductViewModel
              {
                  Code = item.Code,
                  Barcode = item.Barcode,
                  Name = item.Name,
                  Id = item.Id,
                  CategoryCode = item.CategoryCode,
                  PriceInbound = item.PriceOutbound,
                  Unit = item.Unit,
                  Image_Name = item.Image_Name
              });
            ViewBag.productList = productList;
            return PartialView(model);
        }
        #endregion

        #region LoadInvoice

        public PartialViewResult LoadInvoice(int OrderNo, int Index)
        {
            var model = new CommisionInvoiceViewModel();
            model.CommissionValue = 0;
            model.IsMoney = false;
            model.OrderNo = OrderNo;
            model.Index = Index;
            model.DonateDetailList = new List<DonateProOrSerViewModel>();
            return PartialView(model);
        }

        public PartialViewResult LoadInvoiceDif(int OrderNo, int Index)
        {
            var model = new CommisionInvoiceViewModel();
            model.CommissionValue = 0;
            model.IsMoney = false;
            model.OrderNo = OrderNo;
            model.Index = Index;
            model.DonateDetailList = new List<DonateProOrSerViewModel>();
            return PartialView(model);
        }
        #endregion

        #region LoadDonateItembyProduct

        public PartialViewResult LoadDonateItembyProduct(int OrderNo, string ProductCode, string ProductName, int ProductId, decimal Price, int ParentOrderNo)
        {
            var model = new DonateProOrSerViewModel();
            model.ProductId = ProductId;
            model.ProductCode = ProductCode;
            model.ProductName = ProductName;
            model.Price = Price;
            model.OrderNo = OrderNo;
            model.ParentOrderNo = ParentOrderNo;
            model.TargetModule = "CommisionCustomer";
            return PartialView(model);
        }
        #endregion

        #region LoadDonateItembyInvoice

        public PartialViewResult LoadDonateItembyInvoice(int OrderNo, string ProductCode, string ProductName, int ProductId, decimal Price, int ParentOrderNo)
        {
            var model = new DonateProOrSerViewModel();
            model.ProductId = ProductId;
            model.ProductCode = ProductCode;
            model.ProductName = ProductName;
            model.Price = Price;
            model.OrderNo = OrderNo;
            model.ParentOrderNo = ParentOrderNo;
            model.TargetModule = "CommisionInvoice";
            return PartialView(model);
        }
        #endregion

        void UpdateCommisionCustomer(CommissionCusViewModel model, CommissionCus commissioncus)
        {
            #region CommisionCustomer
            var detail = CommissionDetailRepository.GetAllCommisionCustomer().Where(x => x.CommissionCusId == model.Id).ToList();
            //
            if (detail == null || detail.Count() == 0)
            {
                CreateCommisionCustomer(model, commissioncus);
            }


            // 
            //
            var _delete = detail.Where(id1 => !model.DetailList.ToList().Any(id2 => id2.Id == id1.Id)).ToList();
            if (_delete.Any())
            {
                foreach (var item in _delete)
                {
                    CommissionDetailRepository.DeleteCommisionCustomer(item.Id);
                    DeleteDonateProOrSer("CommisionCustomer", item.Id);
                }
            }
            //
            if (model.DetailList != null)
            {
                if (model.DetailList.Any(x => x.Id > 0))
                {
                    var update = detail.Where(id1 => model.DetailList.ToList().Any(id2 => id2.Id == id1.Id)).ToList();
                    //lưu danh sách thao tác thực hiện dịch vụ
                    foreach (var item in model.DetailList.Where(x => x.Id >= 0))
                    {
                        var _update = update.FirstOrDefault(x => x.Id == item.Id);
                        if (item.Id == 0 || item.Id == null)
                        {
                            var commision = new CommisionCustomer();
                            commision.IsDeleted = false;
                            commision.CreatedUserId = WebSecurity.CurrentUserId;
                            commision.ModifiedUserId = WebSecurity.CurrentUserId;
                            commision.CreatedDate = DateTime.Now;
                            commision.ModifiedDate = DateTime.Now;
                            commision.CommissionCusId = commissioncus.Id;
                            commision.ProductId = item.ProductId;
                            commision.CommissionValue = item.CommissionValue;
                            commision.Quantity = item.Quantity;
                            commision.Symbol = item.Symbol;
                            commision.Type = item.Type;
                            commision.ExpiryMonth = item.ExpiryMonth;
                            commision.IsMoney = (item.IsMoney == null || item.IsMoney == false) ? false : true;
                            CommissionDetailRepository.InsertCommisionCustomer(commision);
                            //
                            CreateDonateProOrSer(item.DonateDetailList, commision.Id);
                        }
                        else
                        {
                            _update.CommissionValue = item.CommissionValue;
                            _update.IsMoney = (item.IsMoney == null || item.IsMoney == false) ? false : true;
                            _update.Symbol = item.Symbol;
                            _update.Quantity = item.Quantity;
                            _update.ExpiryMonth = item.ExpiryMonth;
                            CommissionDetailRepository.UpdateCommisionCustomer(_update);
                            if (item.DonateDetailList == null)
                            {
                                item.DonateDetailList = new List<DonateProOrSerViewModel>();
                            }
                            UpdateDonateProOrSer(item.DonateDetailList, _update.Id, "CommisionCustomer");
                        }


                    }
                }
            }
            #endregion
        }

        void UpdateCommisionInvoice(CommissionCusViewModel model, CommissionCus commissioncus)
        {
            #region commissionInvoice
            var detail_invoice = commissionInvoiceRepository.GetAllCommisionInvoice().Where(x => x.CommissionCusId == model.Id).ToList();
            //
            CreateCommisionInvoice(model, commissioncus);
            //
            var _delete_invoice = detail_invoice.Where(id1 => !model.InvoiceDetailList.ToList().Any(id2 => id2.Id == id1.Id)).ToList();
            if (_delete_invoice.Any())
            {
                foreach (var item in _delete_invoice)
                {
                    commissionInvoiceRepository.DeleteCommisionInvoice(item.Id);
                    DeleteDonateProOrSer("CommisionInvoice", item.Id);
                }
            }
            if (model.InvoiceDetailList != null)
            {
                if (model.InvoiceDetailList.Any(x => x.Id > 0))
                {
                    var update_invoice = detail_invoice.Where(id1 => model.InvoiceDetailList.ToList().Any(id2 => id2.Id == id1.Id)).ToList();
                    //lưu danh sách thao tác thực hiện dịch vụ
                    foreach (var item in model.InvoiceDetailList.Where(x => x.Id > 0))
                    {
                        var _update_invoice = update_invoice.FirstOrDefault(x => x.Id == item.Id);
                        _update_invoice.CommissionValue = item.CommissionValue;
                        _update_invoice.IsMoney = (item.IsMoney == null || item.IsMoney == false) ? false : true;
                        _update_invoice.IsVIP = item.IsVIP;
                        _update_invoice.StartSymbol = item.StartSymbol;
                        _update_invoice.StartAmount = item.StartAmount;
                        _update_invoice.EndSymbol = item.EndSymbol;
                        _update_invoice.EndAmount = item.EndAmount;
                        _update_invoice.SalesPercent = item.SalesPercent;
                        _update_invoice.Type = item.Type;
                        _update_invoice.Name = item.Name;
                        commissionInvoiceRepository.UpdateCommisionInvoice(_update_invoice);
                        if (item.DonateDetailList == null)
                        {
                            item.DonateDetailList = new List<DonateProOrSerViewModel>();
                        }
                        UpdateDonateProOrSer(item.DonateDetailList, _update_invoice.Id, "CommisionInvoice");
                    }
                }

            }
            #endregion
        }

        void UpdateDonateProOrSer(List<DonateProOrSerViewModel> DonateDetailList, int TargetId, string TargetModule)
        {
            #region UpdateDonateProOrSer
            var detail = donateProductOrServiceRepository.GetAllDonateProOrSer().Where(x => x.TargetId == TargetId && x.TargetModule == TargetModule).ToList();
            //
            CreateDonateProOrSer(DonateDetailList, TargetId);
            //
            var _delete = detail.Where(id1 => !DonateDetailList.ToList().Any(id2 => id2.Id == id1.Id)).ToList();
            foreach (var ii in _delete)
            {
                donateProductOrServiceRepository.DeleteDonateProOrSer(ii.Id);
            }
            //
            if (DonateDetailList.Any(x => x.Id > 0))
            {
                var update = detail.Where(id1 => DonateDetailList.ToList().Any(id2 => id2.Id == id1.Id)).ToList();
                //lưu danh sách thao tác thực hiện dịch vụ
                foreach (var ii in DonateDetailList.Where(x => x.Id > 0))
                {
                    var _update = update.FirstOrDefault(x => x.Id == ii.Id);
                    _update.TotalQuantity = ii.TotalQuantity;
                    _update.Quantity = ii.Quantity;
                    _update.ExpriryMonth = ii.ExpriryMonth;
                    _update.RemainQuantity = ii.RemainQuantity;
                    donateProductOrServiceRepository.UpdateDonateProOrSer(_update);
                }
            }
            #endregion
        }
        //add commision
        void CreateCommisionCustomer(CommissionCusViewModel model, CommissionCus commissioncus)
        {
            if (model.DetailList != null)
            {
                if (model.DetailList.Any(x => x.Id == 0))
                {
                    foreach (var item in model.DetailList)
                    {

                        var commision = new CommisionCustomer();
                        commision.IsDeleted = false;
                        commision.CreatedUserId = WebSecurity.CurrentUserId;
                        commision.ModifiedUserId = WebSecurity.CurrentUserId;
                        commision.CreatedDate = DateTime.Now;
                        commision.ModifiedDate = DateTime.Now;
                        commision.CommissionCusId = commissioncus.Id;
                        commision.ProductId = item.ProductId;
                        commision.CommissionValue = item.CommissionValue;
                        commision.Quantity = item.Quantity;
                        commision.Symbol = item.Symbol;
                        commision.Type = item.Type;
                        commision.ExpiryMonth = item.ExpiryMonth;
                        commision.IsMoney = (item.IsMoney == null || item.IsMoney == false) ? false : true;
                        CommissionDetailRepository.InsertCommisionCustomer(commision);
                        //
                        CreateDonateProOrSer(item.DonateDetailList, commision.Id);
                    }
                }
            }
        }

        void CreateCommisionInvoice(CommissionCusViewModel model, CommissionCus commissioncus)
        {
            if (model.InvoiceDetailList != null)
            {
                if (model.InvoiceDetailList.Any(x => x.Id == 0))
                {
                    foreach (var item in model.InvoiceDetailList.Where(x => x.Id == 0))
                    {
                        var commision_invoice = new CommisionInvoice();
                        commision_invoice.IsDeleted = false;
                        commision_invoice.CreatedUserId = WebSecurity.CurrentUserId;
                        commision_invoice.ModifiedUserId = WebSecurity.CurrentUserId;
                        commision_invoice.CreatedDate = DateTime.Now;
                        commision_invoice.ModifiedDate = DateTime.Now;
                        commision_invoice.CommissionCusId = commissioncus.Id;
                        commision_invoice.CommissionValue = item.CommissionValue;
                        commision_invoice.IsMoney = (item.IsMoney == null || item.IsMoney == false) ? false : true;
                        commision_invoice.Type = item.Type;
                        commision_invoice.EndAmount = item.EndAmount;
                        commision_invoice.EndSymbol = item.EndSymbol;
                        commision_invoice.IsVIP = item.IsVIP;
                        commision_invoice.Name = item.Name;
                        commision_invoice.SalesPercent = item.SalesPercent;
                        commision_invoice.StartAmount = item.StartAmount;
                        commision_invoice.StartSymbol = item.StartSymbol;
                        commissionInvoiceRepository.InsertCommisionInvoice(commision_invoice);
                        //
                        CreateDonateProOrSer(item.DonateDetailList, commision_invoice.Id);
                    }
                }
            }
        }

        void CreateDonateProOrSer(List<DonateProOrSerViewModel> DonateDetailList, int? TargetId)
        {
            #region DonateProOrSer
            if (DonateDetailList != null)
            {
                if (DonateDetailList.Any(x => x.Id == 0))
                {
                    foreach (var ii in DonateDetailList.Where(x => x.Id == 0))
                    {
                        var donate = new DonateProOrSer();
                        donate.IsDeleted = false;
                        donate.CreatedUserId = WebSecurity.CurrentUserId;
                        donate.ModifiedUserId = WebSecurity.CurrentUserId;
                        donate.CreatedDate = DateTime.Now;
                        donate.ModifiedDate = DateTime.Now;
                        donate.TargetId = TargetId;
                        donate.ProductId = ii.ProductId;
                        donate.TargetModule = ii.TargetModule;
                        donate.TotalQuantity = ii.TotalQuantity;
                        donate.Quantity = ii.Quantity;
                        donate.ExpriryMonth = ii.ExpriryMonth;
                        donate.RemainQuantity = ii.RemainQuantity;
                        donateProductOrServiceRepository.InsertDonateProOrSer(donate);
                    }
                }
            }

            #endregion
        }

        void DeleteDonateProOrSer(string TargetModule, int? TargetId)
        {
            var detail = donateProductOrServiceRepository.GetAllDonateProOrSer().Where(x => x.TargetId == TargetId && x.TargetModule == TargetModule).ToList();
            if (detail.Any())
            {
                foreach (var ii in detail)
                {
                    donateProductOrServiceRepository.DeleteDonateProOrSer(ii.Id);
                }
            }
        }


        #region BaoCaoKM
        public ViewResult BaoCaoKM(string Name, string StartDate, string EndDate, string BuyDate, string BuyDate2)
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

            //Lấy danh sách ctkm
            IEnumerable<CommissionCusViewModel> q = CommissionCusRepository.GetAllCommissionCus()
                .Select(item => new CommissionCusViewModel
                {
                    Id = item.Id,
                    CreatedUserId = item.CreatedUserId,
                    //CreatedUserName = item.CreatedUserName,
                    CreatedDate = item.CreatedDate,
                    ModifiedUserId = item.ModifiedUserId,
                    //ModifiedUserName = item.ModifiedUserName,
                    ModifiedDate = item.ModifiedDate,
                    Name = item.Name,
                    ApplyFor = item.ApplyFor,
                    EndDate = item.EndDate,
                    Note = item.Note,
                    StartDate = item.StartDate,
                    Type = item.Type,
                    TyleHuong = item.TyleHuong,

                }).OrderByDescending(m => m.ModifiedDate).ToList();
            if (!string.IsNullOrEmpty(Name))
            {
                Name = Helpers.Common.ChuyenThanhKhongDau(Name);
                q = q.Where(x => Helpers.Common.ChuyenThanhKhongDau(x.Name).Contains(Name)).ToList();
            }
            //Lọc theo ngày
            DateTime d_startDate, d_endDate;
            if (DateTime.TryParseExact(StartDate, "dd/MM/yyyy", new CultureInfo("vi-VN"), DateTimeStyles.None, out d_startDate))
            {
                if (DateTime.TryParseExact(EndDate, "dd/MM/yyyy", new CultureInfo("vi-VN"), DateTimeStyles.None, out d_endDate))
                {
                    d_endDate = d_endDate.AddHours(23).AddMinutes(59);
                    q = q.Where(x => x.StartDate >= d_startDate && x.EndDate <= d_endDate).ToList();
                }
            }

            //Lấy đơn hàng có ctkm
            //DateTime Buydate;
            //DateTime Buydate2;
            var data = logPromotionRepository.GetvwAllLogPromotion().ToList();
            //var logPromotion = new List<LogPromotionViewModel>();
            //var Branch = branchRepository.GetAllBranch().ToList();

            //if (DateTime.TryParseExact(BuyDate, "dd/MM/yyyy", new CultureInfo("vi-VN"), DateTimeStyles.None, out Buydate))
            //{
            //    if (DateTime.TryParseExact(BuyDate2, "dd/MM/yyyy", new CultureInfo("vi-VN"), DateTimeStyles.None, out Buydate2))
            //    {
            //        Buydate2 = Buydate2.AddHours(23).AddMinutes(59);
            //        data = data.Where(x => x.BuyDate >= Buydate && x.BuyDate <= Buydate2).ToList();
            //    }
            //}
            //if (intBrandID > 0)
            //{
            //    q = q.Where(x => x.ApplyFor != null && x.ApplyFor.Contains(intBrandID.ToString())).ToList();
            //    data = data.Where(x => x.BranchId == intBrandID).ToList();
            //}
            //var data2 = new List<vwLogPromotion>();
            ////tim don huy chuyen
            //foreach (var item in data)
            //{
            //    var donhang = invoiceRepository.GetvwProductInvoiceByCode(item.BranchId.Value, item.ProductInvoiceCode);
            //    if (donhang != null && donhang.Status != "delete")
            //    {

            //        data2.Add(item);
            //    }
            //}
            //data = data2;

            //AutoMapper.Mapper.Map(data, logPromotion);
            //foreach (var item in logPromotion)
            //{
            //    foreach (var i in Branch)
            //    {
            //        if (item.BranchId == i.Id)
            //        {
            //            item.BranchName = i.Name;
            //        }
            //    }
            //}

            //ViewBag.logPromotion = logPromotion;
            return View(q);
        }

        [AllowAnonymous]
        public ActionResult PartialBaoCaoKM(int Id)
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
            var data = SqlHelper.QuerySP<LogPromotionViewModel>("getLogPromotion", new
            {
                @intBrandID = intBrandID,
                @CommissionCusId = Id,
                @StartDate = "",
                @EndDate = ""

            }).ToList();
            return View(data);
        }

        public ActionResult Print(string Name, bool ExportExcel, string StartDate, string EndDate, string BuyDate, string BuyDate2)
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
            //Lấy danh sách ctkm
            IEnumerable<CommissionCusViewModel> q = CommissionCusRepository.GetAllCommissionCus()
                .Select(item => new CommissionCusViewModel
                {
                    Id = item.Id,
                    CreatedUserId = item.CreatedUserId,
                    //CreatedUserName = item.CreatedUserName,
                    CreatedDate = item.CreatedDate,
                    ModifiedUserId = item.ModifiedUserId,
                    //ModifiedUserName = item.ModifiedUserName,
                    ModifiedDate = item.ModifiedDate,
                    Name = item.Name,
                    ApplyFor = item.ApplyFor,
                    EndDate = item.EndDate,
                    Note = item.Note,
                    StartDate = item.StartDate,
                    Type = item.Type,
                    TyleHuong = item.TyleHuong,

                }).OrderByDescending(m => m.ModifiedDate).ToList();
            if (!string.IsNullOrEmpty(Name))
            {
                Name = Helpers.Common.ChuyenThanhKhongDau(Name);
                q = q.Where(x => Helpers.Common.ChuyenThanhKhongDau(x.Name).Contains(Name)).ToList();
            }
            //Lọc theo ngày
            DateTime d_startDate, d_endDate;
            if (!string.IsNullOrEmpty(StartDate) && !string.IsNullOrEmpty(EndDate))
            {
                if (DateTime.TryParseExact(StartDate, "dd/MM/yyyy", new CultureInfo("vi-VN"), DateTimeStyles.None, out d_startDate))
                {
                    if (DateTime.TryParseExact(EndDate, "dd/MM/yyyy", new CultureInfo("vi-VN"), DateTimeStyles.None, out d_endDate))
                    {
                        d_endDate = d_endDate.AddHours(23).AddMinutes(59);
                        q = q.Where(x => x.StartDate >= d_startDate && x.EndDate <= d_endDate).ToList();
                    }
                }
            }

            var data = q.ToList();
            //if (intBrandID > 0)
            //{
            //    data = data.Where(x => x.ApplyFor == strBrandID).ToList();
            //}

            //var logpromotion = logPromotionRepository.GetvwAllLogPromotion().ToList();

            var logpromotion = SqlHelper.QuerySP<LogPromotionViewModel>("getvwSale_LogPromotion", new
            {
                @intBrandID = intBrandID
            }).ToList();


            var log_Promotion = new List<LogPromotionViewModel>();
            DateTime Buydate;
            DateTime Buydate2;

            //if (DateTime.TryParseExact(BuyDate, "dd/MM/yyyy", new CultureInfo("vi-VN"), DateTimeStyles.None, out Buydate))
            //{
            //    var Buydate2 = Buydate.AddHours(23).AddMinutes(59);
            //    logpromotion = logpromotion.Where(x => x.BuyDate >= Buydate && x.BuyDate <= Buydate2).ToList();
            //}

            if (DateTime.TryParseExact(BuyDate, "dd/MM/yyyy", new CultureInfo("vi-VN"), DateTimeStyles.None, out Buydate))
            {
                if (DateTime.TryParseExact(BuyDate, "dd/MM/yyyy", new CultureInfo("vi-VN"), DateTimeStyles.None, out Buydate2))
                {
                    Buydate2 = Buydate2.AddHours(23).AddMinutes(59);
                    logpromotion = logpromotion.Where(x => x.BuyDate >= Buydate && x.BuyDate <= Buydate2).ToList();
                }
            }

            if (intBrandID > 0)
            {
                logpromotion = logpromotion.Where(x => x.BranchId == intBrandID).ToList();
            }

            var data2 = new List<vwLogPromotion>();
          
            AutoMapper.Mapper.Map(logpromotion, log_Promotion);

            var Branch = branchRepository.GetAllBranch().ToList();
            foreach (var j in log_Promotion)
            {
                foreach (var i in Branch)
                {
                    if (j.BranchId == i.Id)
                    {
                        j.BranchName = i.Name;
                    }
                }
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
            model.Content = model.Content.Replace("{DataTable}", buildHtml(data, log_Promotion));
            model.Content = model.Content.Replace("{System.Logo}", ImgLogo);
            model.Content = model.Content.Replace("{System.CompanyName}", company);
            model.Content = model.Content.Replace("{System.AddressCompany}", address);
            model.Content = model.Content.Replace("{System.PhoneCompany}", phone);
            model.Content = model.Content.Replace("{System.Fax}", fax);
            model.Content = model.Content.Replace("{System.BankCodeCompany}", bankcode);
            model.Content = model.Content.Replace("{System.BankNameCompany}", bankname);
            model.Content = model.Content.Replace("{Title}", "Báo cáo CTKM");


            Response.AppendHeader("content-disposition", "attachment;filename=" + "CTKM_" + DateTime.Now.ToString("yyyyMMdd") + ".xls");
            Response.Charset = "";
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.ContentType = "application/vnd.ms-excel";
            Response.Write(model.Content);
            Response.End();

            return View(model);
        }

        string buildHtml(List<CommissionCusViewModel> detailList, List<LogPromotionViewModel> listPromotion)
        {
            string detailLists = "<table class=\"invoice-detail\"  >\r\n";
            detailLists += "<thead>\r\n";
            detailLists += "	<tr>";
            detailLists += "		<th>STT</th>";
            detailLists += "		<th>Chương trình khuyến mãi</th>";
            detailLists += "		<th>Ngày bắt đầu</th>";
            detailLists += "		<th>Ngày kết thúc</th>";
            detailLists += "		<th>Hưởng doanh thu</th>";

            detailLists += "	</tr>";
            detailLists += "</thead>\r\n";
            detailLists += "<tbody>\r\n";
            var index = 1;
            var Branch = branchRepository.GetAllBranch().ToList();
            foreach (var item in detailList)
            {
                //Lấy đơn hàng có ctkm
                //DateTime Buydate;
                //var data = logPromotionRepository.GetvwAllLogPromotion().ToList();
                //var logPromotion = new List<LogPromotionViewModel>();
                //if (DateTime.TryParseExact(BuyDate, "dd/MM/yyyy", new CultureInfo("vi-VN"), DateTimeStyles.None, out Buydate))
                //{
                //    var Buydate2 = Buydate.AddHours(23).AddMinutes(59);
                //    data = data.Where(x => x.BuyDate >= Buydate && x.BuyDate <= Buydate2).ToList();
                //}
                //if (BranchId > 0)
                //{
                //    data = data.Where(x => x.BranchId == BranchId).ToList();
                //}
                //var data2 = new List<vwLogPromotion>();
                ////tim don huy chuyen
                //foreach (var i in data)
                //{
                //    var donhang = invoiceRepository.GetvwProductInvoiceByCode(i.BranchId.Value, i.ProductInvoiceCode);
                //    if (donhang != null && donhang.Status != "delete")
                //    {
                //        data2.Add(i);
                //    }
                //}
                //data = data2;
                //AutoMapper.Mapper.Map(data, logPromotion);

                //logPromotion = logPromotion.Where(x => x.CommissionCusId == item.Id).ToList();
                //foreach (var j in logPromotion)
                //{
                //    foreach (var i in Branch)
                //    {
                //        if (j.BranchId == i.Id)
                //        {
                //            j.BranchName = i.Name;
                //        }
                //    }
                //}

                var logPromotion = listPromotion.Where(x => x.CommissionCusId == item.Id).ToList();

                detailLists += "<tr border=\"1\" style=\"background-color:yellow\">\r\n"
               + "<td class=\"text-center orderNo\">" + (index++) + "</td>\r\n"
                + "<td class=\"text-center \">" + item.Name + "</td>\r\n"
                + "<td class=\"text-center \">" + (item.StartDate.HasValue ? item.StartDate.Value.ToString("dd/MM/yyyy") : "") + "</td>\r\n"
                + "<td class=\"text-center \">" + (item.EndDate.HasValue ? item.EndDate.Value.ToString("dd/MM/yyyy") : "") + "</td>\r\n"
                + "<td class=\"text-center \">" + item.TyleHuong + "</td>\r\n"
               + "</tr>\r\n"
               + "<tr>\r\n"
               + "<td colspan=\"8\">\r\n"
               + "<div>\r\n"
                + "<table class=\"detail-productinvoice\" border=\"1\">\r\n";


                detailLists += "<thead>\r\n";
                detailLists += "	<tr>";
                detailLists += "		<th>Chi nhánh</th>";
                detailLists += "		<th>Ngày mua</th>";
                detailLists += "		<th>Mã đơn hàng</th>";
                detailLists += "		<th>Khách hàng</th>";
                detailLists += "		<th>Mã khách hàng</th>";
                detailLists += "		<th>Loại khuyến mãi</th>";
                detailLists += "		<th>Giảm giá</th>";
                detailLists += "		<th>Hàng tặng</th>";
                detailLists += "		<th>Tổng tiền</th>";
                detailLists += "	</tr>";
                detailLists += "</thead>\r\n";
                detailLists += "<tbody>\r\n";
                foreach (var i in logPromotion)
                {
                    detailLists += "<tr>\r\n"
                 + "<td class=\"text-left \">" + i.BranchName + "</td>\r\n"
                + "<td class=\"text-left \">" + i.BuyDate.Value.ToString("dd/MM/yyyy") + "</td>\r\n"
                + "<td class=\"text-left \">" + i.ProductInvoiceCode + "</td>\r\n"
                + "<td class=\"text-left \">" + i.CustomerName + "</td>\r\n"
                 + "<td class=\"text-left \">" + i.CustomerCode + "</td>\r\n"
                + "<td class=\"text-left \">" + i.Name + "</td>\r\n"
                + "<td class=\"text-left \">" + CommonSatic.ToCurrencyStr(i.CommissionValue, null).Replace(".", ","); if (i.CommissionValue <= 100) { detailLists += "%"; } else { detailLists += " VND"; };
                    detailLists += "</td>\r\n" + "<td class=\"text-left \">" + i.GiftProductName + "</td>\r\n"
                      + "<td class=\"text-left \">" + CommonSatic.ToCurrencyStr(i.TotalAmount, null).Replace(".", ",") + "</td>\r\n"
                      + "</tr>\r\n";

                }
                detailLists += "</tbody>\r\n";
                detailLists += "<tfoot style=\"font-weight:bold\">\r\n";
                detailLists += "</tfoot>\r\n</table>\r\n"
                + "</div>\r\n"
                + "<td>\r\n";
                detailLists += "</tr>\r\n";
            }

            detailLists += "</tbody>\r\n";
            detailLists += "<tfoot style=\"font-weight:bold\">\r\n";
            detailLists += "</tfoot>\r\n</table>\r\n";
            return detailLists;
        }
        #endregion
    }
}
