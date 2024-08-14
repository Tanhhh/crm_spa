using System.Globalization;
using Erp.BackOffice.Sale.Models;
using Erp.BackOffice.Filters;
using Erp.Domain.Entities;
using Erp.Domain.Interfaces;
using Erp.Domain.Sale.Entities;
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
using System.Web.Script.Serialization;
using System.IO;
using System.Text.RegularExpressions;
using Erp.Domain.Account.Interfaces;
using Erp.BackOffice.Account.Models;
using System.Web;

namespace Erp.BackOffice.Sale.Controllers
{
    [Authorize]
    [InitializeSimpleMembership]
    [Erp.BackOffice.Helpers.NoCacheHelper]
    public class ServiceController : Controller
    {
        private readonly IProductOrServiceRepository ServiceRepository;
        private readonly IUserRepository userRepository;
        private readonly IObjectAttributeRepository ObjectAttributeRepository;
        private readonly IServiceComboRepository serviceComboRepository;
        private readonly IUsingServiceLogRepository usingServiceLogRepository;
        private readonly IProductInvoiceRepository productInvoiceRepository;
        private readonly ICustomerRepository CustomerRepository;
        private readonly IUsingServiceLogDetailRepository usingServiceLogDetailRepository;
        private readonly IServiceReminderGroupRepository serviceReminderGroupRepository;
        private readonly IServiceReminderRepository serviceReminderRepository;
        private readonly IServiceStepsRepository serviceStepRepository;
        private readonly IServiceDetailRepository serviceDetailRepository;
        private readonly ITemplatePrintRepository templatePrintRepository;
        private readonly IProductInSkinCareRepository ProductInSkinCareRepository;

        public ServiceController(
            IProductOrServiceRepository _Service
            , IUserRepository _user
            , IObjectAttributeRepository _ObjectAttribute
            , IServiceComboRepository serviceCombo
           , IUsingServiceLogRepository usingServiceLog
             , IProductInvoiceRepository productInvoice
            , ICustomerRepository _Customer
            , IUsingServiceLogDetailRepository usingServiceLogDetail
            , IServiceReminderRepository serviceReminder
            , IServiceReminderGroupRepository serviceReminderGroup
            , IServiceStepsRepository serviceStep
            , IServiceDetailRepository serviceDetail
            , ITemplatePrintRepository _templatePrint
            , IProductInSkinCareRepository _IProductInSkinCare
            )
        {
            ServiceRepository = _Service;
            userRepository = _user;
            ObjectAttributeRepository = _ObjectAttribute;
            serviceComboRepository = serviceCombo;
            usingServiceLogRepository = usingServiceLog;
            productInvoiceRepository = productInvoice;
            CustomerRepository = _Customer;
            usingServiceLogDetailRepository = usingServiceLogDetail;
            serviceReminderGroupRepository = serviceReminderGroup;
            serviceReminderRepository = serviceReminder;
            serviceStepRepository = serviceStep;
            serviceDetailRepository = serviceDetail;
            templatePrintRepository = _templatePrint;
            ProductInSkinCareRepository = _IProductInSkinCare;
        }

        #region Index

        public ViewResult Index(string isDisplay, string txtSearch, string txtCode, string CategoryCode, string ProductGroupService, SearchObjectAttributeViewModel SearchOjectAttr)
        {
            IEnumerable<ServiceViewModel> q = ServiceRepository.GetAllvwService().AsEnumerable()
                .Select(item => new ServiceViewModel
                {
                    Id = item.Id,
                    CreatedUserId = item.CreatedUserId,
                    //CreatedUserName = item.CreatedUserName,
                    CreatedDate = item.CreatedDate,
                    ModifiedUserId = item.ModifiedUserId,
                    //ModifiedUserName = item.ModifiedUserName,
                    ModifiedDate = item.ModifiedDate,
                    CategoryCode = item.CategoryCode,
                    Code = item.Code,
                    Image_Name = item.Image_Name,
                    Name = item.Name,
                    PriceOutbound = item.PriceOutbound,
                    Unit = item.Unit,
                    Barcode = item.Barcode,
                    IsCombo = item.IsCombo,
                    ProductGroup = item.ProductGroup,

                    Origin = item.Origin,
                    NotUse = item.NotUse,
                    is_display = item.is_display
                    //DiscountStaff = item.DiscountStaff,
                    //IsMoneyDiscount = item.IsMoneyDiscount
                }).OrderByDescending(m => m.ModifiedDate);
            if (SearchOjectAttr.ListField != null)
            {
                if (SearchOjectAttr.ListField.Count > 0)
                {
                    //lấy các đối tượng ObjectAttributeValue nào thỏa đk có AttributeId trong ListField và có giá trị tương ứng trong ListField
                    var listObjectAttrValue = ObjectAttributeRepository.GetAllObjectAttributeValue().AsEnumerable().Where(attr => SearchOjectAttr.ListField.Any(item => item.Id == attr.AttributeId && Erp.BackOffice.Helpers.Common.ChuyenThanhKhongDau(attr.Value).Contains(Erp.BackOffice.Helpers.Common.ChuyenThanhKhongDau(item.Value)))).ToList();

                    //tiếp theo tìm các sản phẩm có id bằng với ObjectId trong listObjectAttrValue vừa tìm được
                    q = q.Where(product => listObjectAttrValue.Any(item => item.ObjectId == product.Id));

                    ViewBag.ListOjectAttrSearch = new JavaScriptSerializer().Serialize(SearchOjectAttr.ListField.Select(x => new { Id = x.Id, Value = x.Value }));
                }
            }

            if (string.IsNullOrEmpty(txtSearch) == false)
            {
                txtSearch = txtSearch == "" ? "~" : Erp.BackOffice.Helpers.Common.ChuyenThanhKhongDau(txtSearch);
                q = q.Where(x => Erp.BackOffice.Helpers.Common.ChuyenThanhKhongDau(x.Name).Contains(txtSearch));
            }
            //GỘP 2 mã và tên
            if (string.IsNullOrEmpty(txtCode) == false || string.IsNullOrEmpty(txtSearch) == false)
            {
                txtCode = txtCode == "" ? "~" : txtCode.Trim().ToLower();
                //  q = q.Where(x => x.Code.ToLower().Contains(txtCode));
                q = q.Where(x => Erp.BackOffice.Helpers.Common.ChuyenThanhKhongDau(x.Name).Contains(Helpers.Common.ChuyenThanhKhongDau(txtCode)) || x.Code.ToLower().Contains(txtCode));

            }
            if (isDisplay == "on")
            {
                q = q.Where(x => x.is_display == true);
            }

            if (!string.IsNullOrEmpty(CategoryCode))
            {
                q = q.Where(x => x.CategoryCode == CategoryCode);
            }
            if (!string.IsNullOrEmpty(ProductGroupService))
            {
                q = q.Where(x => x.ProductGroup == ProductGroupService);
            }

            ViewBag.SuccessMessage = TempData["SuccessMessage"];
            ViewBag.FailedMessage = TempData["FailedMessage"];
            ViewBag.AlertMessage = TempData["AlertMessage"];
            return View(q);
        }
        #endregion

        #region ExportExcel
        public List<ServiceViewModel> IndexExport(string txtCode, string CategoryCode, string ProductGroupService, int? BranchId)
        {
            var q = ServiceRepository.GetAllvwService()
                .Select(item => new ServiceViewModel
                {
                    Id = item.Id,
                    CreatedUserId = item.CreatedUserId,
                    //CreatedUserName = item.CreatedUserName,
                    CreatedDate = item.CreatedDate,
                    ModifiedUserId = item.ModifiedUserId,
                    //ModifiedUserName = item.ModifiedUserName,
                    ModifiedDate = item.ModifiedDate,
                    CategoryCode = item.CategoryCode,
                    Code = item.Code,
                    Image_Name = item.Image_Name,
                    Name = item.Name,
                    PriceOutbound = item.PriceOutbound,
                    Unit = item.Unit,
                    Barcode = item.Barcode,
                    IsCombo = item.IsCombo,
                    ProductGroup = item.ProductGroup,
                    Origin = item.Origin,
                    NotUse = item.NotUse
                    //DiscountStaff = item.DiscountStaff,
                    //IsMoneyDiscount = item.IsMoneyDiscount
                }).OrderByDescending(m => m.ModifiedDate).ToList();

            if (string.IsNullOrEmpty(txtCode) == false)
            {
                txtCode = txtCode == "" ? "~" : txtCode.ToLower();
                //  q = q.Where(x => x.Code.ToLower().Contains(txtCode));
                q = q.Where(x => Erp.BackOffice.Helpers.Common.ChuyenThanhKhongDau(x.Name).Contains(Helpers.Common.ChuyenThanhKhongDau(txtCode)) || x.Code.ToLower().Contains(txtCode)).ToList();

            }

            if (!string.IsNullOrEmpty(CategoryCode))
            {
                q = q.Where(x => x.CategoryCode == CategoryCode).ToList();
            }

            if (!string.IsNullOrEmpty(ProductGroupService))
            {
                q = q.Where(x => x.ProductGroup == ProductGroupService).ToList();
            }

            return q;
        }

        public ActionResult ExportExcel(string txtCode, string CategoryCode, string ProductGroupService, int? BranchId, bool ExportExcel = false)
        {
            var data = IndexExport(txtCode, CategoryCode, ProductGroupService, BranchId);

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

        string buildHtml(List<ServiceViewModel> data)
        {
            decimal? tong_tien = 0;
            //Tạo table html chi tiết phiếu xuất
            string detailLists = "<table border=\"1\" class=\"invoice-detail\">\r\n";
            detailLists += "<thead>\r\n";
            detailLists += "	<tr>\r\n";
            detailLists += "		<th>STT</th>\r\n";
            detailLists += "		<th>Mã Dịch Vụ</th>\r\n";
            detailLists += "		<th>Tên Dịch Vụ</th>\r\n";
            detailLists += "		<th>Giá Xuất</th>\r\n";
            detailLists += "		<th>Nhãn Hàng</th>\r\n";
            detailLists += "		<th>Danh Mục</th>\r\n";
            detailLists += "		<th>Nhóm</th>\r\n";
            detailLists += "		<th>Ngày Tạo</th>\r\n";
            detailLists += "		<th>Khóa</th>\r\n";
            detailLists += "	</tr>\r\n";
            detailLists += "</thead>\r\n";
            detailLists += "<tbody>\r\n";
            var index = 1;

            foreach (var item in data)
            {
                tong_tien += (item.PriceOutbound == null ? 0 : item.PriceOutbound);

                detailLists += "<tr>\r\n"
                + "<td class=\"text-center orderNo\">" + (index++) + "</td>\r\n"
                + "<td class=\"text-left \">" + item.Code + "</td>\r\n"
                + "<td class=\"text-left \">" + item.Name + "</td>\r\n"
                + "<td class=\"text-left\">" + item?.PriceOutbound + "</td>\r\n";
                if (item.Origin == "DICHVU")
                {
                    detailLists += "<td class=\"text-left \">" + "Dịch vụ" /*item.Origin*/ + "</td>\r\n";
                }
                else
                {
                    detailLists += "<td class=\"text-left \">" + "Công nghệ cao" /*item.Origin*/ + "</td>\r\n";
                }
                detailLists += "<td class=\"text-left \">" + item.CategoryCode + "</td>\r\n"
                + "<td class=\"text-left \">" + item.ProductGroup + "</td>\r\n"
                + "<td>" + (item.CreatedDate.HasValue ? item.CreatedDate.Value.ToString("dd/MM/yyyy HH:mm") : "") + "</td>"
                + "<td>" + (item.NotUse == true ? "Đã Khóa" : "") + "</td>"
                + "</tr>\r\n";
            }
            detailLists += "</tbody>\r\n";
            detailLists += "<tfoot style=\"font-weight:bold\">\r\n";
            detailLists += "<tr \"  style=\"font-weight:bold\"><td colspan=\"3\" class=\"text-right\">Tổng cộng</td><td class=\"text-right\">"
                        + Erp.BackOffice.Helpers.CommonSatic.ToCurrencyStr(tong_tien, null)
                        + "</td></tr>\r\n";

            detailLists += "</tfoot>\r\n</table>\r\n";


            return detailLists;
        }
        #endregion

        #region LoadServiceItem
        public PartialViewResult LoadServiceItem(int OrderNo, int serviceId, string serviceName, int Quantity, string serviceCode)
        {
            var model = new ServiceComboViewModel();
            model.OrderNo = OrderNo;
            model.ServiceId = serviceId;
            model.Name = serviceName;
            model.Quantity = Quantity;
            model.Code = serviceCode;
            model.Id = 0;
            return PartialView(model);
        }
        #endregion

        #region LoadServiceItem
        public PartialViewResult LoadReminderItem(int OrderNo, int reminderId, string reminderName)
        {
            var model = new ServiceReminderGroupViewModel();
            model.OrderNo = OrderNo;
            model.ServiceReminderId = reminderId;
            model.Name = reminderName;
            model.Id = 0;
            return PartialView(model);
        }
        #endregion

        #region Create
        public ViewResult Create()
        {
            var model = new ServiceViewModel();
            model.PriceOutbound = 0;
            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create(ServiceViewModel model)
        {
            Helpers.Common.ResetCacheYHL("MainPartial_Mainkissy,lineService_GroupDetails,lineproduct_GroupDetails,SearchSpa_GroupDetails,alllsproduct_GroupDetails,catelistproduct_GroupDetails");
            var EquimentGroup = Request["EquimentGroup"];
            if (ModelState.IsValid)
            {
                var Service = new Product();
                Service.IsDeleted = false;
                Service.CreatedUserId = WebSecurity.CurrentUserId;
                Service.ModifiedUserId = WebSecurity.CurrentUserId;
                Service.CreatedDate = DateTime.Now;
                Service.ModifiedDate = DateTime.Now;
                Service.Barcode = model.Barcode;
                Service.CategoryCode = model.CategoryCode;
                Service.Description = model.Description;
                //Service.IsCombo = model.IsCombo;
                Service.EquimentGroup = EquimentGroup;
                Service.Name = model.Name;
                Service.PriceOutbound = model.PriceOutbound;
                Service.Type = "service";
                Service.Unit = model.Unit;
                Service.Code = model.Code.Trim();
                Service.QuantityDayUsed = model.QuantityDayUsed;
                Service.TimeForService = model.TimeForService;
                Service.QuantityDayNotify = model.QuantityDayNotify;
                Service.ProductLinkId = model.ProductLinkId;
                Service.Origin = model.Origin;
                Service.MinQuantityforSevice = model.MinQuantityforSevice;
                Service.MinInventory = 0;
                Service.IS_ALOW_BAN_AM = true;
                Service.NHOMSANPHAM_ID = model.NHOMSANPHAM_ID;
                Service.NHOMSANPHAM_ID_LST = model.NHOMSANPHAM_ID.ToString();
                Service.THANH_PHAN = model.THANH_PHAN;
                Service.CONGDUNG = model.CONGDUNG;
                Service.Description_brief = model.Description_brief;
                string k = model.Description_brief;
                if (model.Description_brief != null)
                {
                    Service.Meta_Description_brief = Regex.Replace(k.Replace("&nbsp;", " "), "<.*?>", String.Empty);//
                }
                Service.HDSD = model.HDSD;
                Service.URL_SLUG = model.URL_SLUG;
                var path = System.Web.HttpContext.Current.Server.MapPath("~" + Helpers.Common.GetSetting("product-image-folder"));
                if (Request.Files["file-image"] != null)
                {
                    var file = Request.Files["file-image"];
                    if (file.ContentLength > 0)
                    {
                        string image_name = "service_" + Helpers.Common.ChuyenThanhKhongDau(Regex.Replace(Service.Name, @"\s+", "_")) + "." + file.FileName.Split('.').Last();
                        bool isExists = System.IO.Directory.Exists(path);
                        if (!isExists)
                            System.IO.Directory.CreateDirectory(path);
                        file.SaveAs(path + image_name);
                        Service.Image_Name = image_name;
                    }
                }
                ServiceRepository.InsertService(Service);

                //ghi log 
                Erp.BackOffice.Controllers.HomeController.WriteLog(Service.Id, Service.Code, "đã tạo dịch vụ", "Service/Edit/" + Service.Id, Helpers.Common.CurrentUser.BranchId.Value);

                //lưu danh sách thao tác thực hiện dịch vụ
                foreach (var item in model.DetailList.Where(x => !string.IsNullOrEmpty(x.Name)))
                {
                    var action = new Domain.Sale.Entities.ServiceSteps();
                    action.IsDeleted = false;
                    action.CreatedUserId = WebSecurity.CurrentUserId;
                    action.ModifiedUserId = WebSecurity.CurrentUserId;
                    action.CreatedDate = DateTime.Now;
                    action.ModifiedDate = DateTime.Now;
                    action.Name = item.Name;
                    action.TotalMinute = item.TotalMinute == null ? 0 : item.TotalMinute;
                    action.Note = item.Note;
                    action.ProductId = Service.Id;
                    serviceStepRepository.InsertServiceSteps(action);
                }

                //lưu danh sách túi liệu trình
                foreach (var item in model.DetailServiceList.Where(x => x.ProductId > 0))
                {
                    var servicedetail = new Domain.Sale.Entities.ServiceDetail();
                    servicedetail.IsDeleted = false;
                    servicedetail.CreatedUserId = WebSecurity.CurrentUserId;
                    servicedetail.ModifiedUserId = WebSecurity.CurrentUserId;
                    servicedetail.CreatedDate = DateTime.Now;
                    servicedetail.ModifiedDate = DateTime.Now;
                    servicedetail.ProductId = item.ProductId;
                    servicedetail.Quantity = item.Quantity;
                    servicedetail.ServiceId = Service.Id;
                    serviceDetailRepository.InsertServiceDetail(servicedetail);
                }

                if (string.IsNullOrEmpty(Request["IsPopup"]) == false)
                {
                    return RedirectToAction("Edit", new { Id = model.Id, IsPopup = Request["IsPopup"] });
                }
                TempData[Globals.SuccessMessageKey] = App_GlobalResources.Wording.InsertSuccess;
                return RedirectToAction("Index");
            }
            return View(model);
        }
        #endregion

        #region CheckCodeExsist
        public ActionResult CheckCodeExsist(int? id, string code)
        {
            code = code.Trim();
            var product = ServiceRepository.GetAllvwService()
                .Where(item => item.Code == code).FirstOrDefault();
            if (product != null)
            {
                if (id == null || (id != null && product.Id != id))
                    return Content("Trùng mã dịch vụ!");
                else
                {
                    return Content("");
                }
            }
            else
            {
                return Content("");
            }
        }
        #endregion

        #region Edit
        public ActionResult Edit(int? Id)
        {
            var Service = ServiceRepository.GetvwServiceById(Id.Value);
            if (Service != null && Service.IsDeleted != true)
            {
                var model = new ServiceViewModel();
                AutoMapper.Mapper.Map(Service, model);
                model.DetailList = serviceStepRepository.GetAllServiceSteps().Where(x => x.ProductId == Service.Id)
                   .Select(x => new ServiceStepsViewModel
                   {
                       Name = x.Name,
                       Id = x.Id,
                       Note = x.Note,
                       TotalMinute = x.TotalMinute,
                       ProductId = x.ProductId
                   }).ToList();
                model.DetailServiceList = serviceDetailRepository.GetvwAllServiceDetail().Where(x => x.ServiceId == Service.Id)
                  .Select(x => new ServiceDetailViewModel
                  {
                      Id = x.Id,
                      ProductId = x.ProductId,
                      Quantity = x.Quantity,
                      ServiceId = x.ServiceId,
                      ProductCode = x.ProductCode,
                      ProductName = x.ProductName
                  }).ToList();

                var productSkincare = ProductInSkinCareRepository.GetAllProductInSkinCareByProductId(model.Id).Select(
                   item => new ProductInSkinCareViewModel
                   {
                       Id = item.Id,
                       Name = item.Name,
                       ProductId = item.ProductId,
                       ProductIdMem = item.ProductIdMem,
                       IsDeleted = item.IsDeleted,
                       OrderNo = item.OrderNo
                   }
                   ).ToList();


                if (productSkincare != null)
                {
                    foreach (var i in productSkincare)
                    {
                        var pro = ServiceRepository.GetProductById(i.ProductIdMem);
                        if (pro != null)
                        {
                            i.ProductName = pro.Name;
                            i.ProductCode = pro.Code;
                        }
                    }
                }
                ViewBag.productSkincare = productSkincare;

                return View(model);
            }
            if (Request.UrlReferrer != null)
                return Redirect(Request.UrlReferrer.AbsoluteUri);
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(ServiceViewModel model)
        {
            Helpers.Common.ResetCacheYHL("MainPartial_Mainkissy,lineService_GroupDetails,lineproduct_GroupDetails,SearchSpa_GroupDetails,alllsproduct_GroupDetails,catelistproduct_GroupDetails");
            if (ModelState.IsValid)
            {
                var Service = ServiceRepository.GetProductById(model.Id);
                AutoMapper.Mapper.Map(model, Service);
                Service.ModifiedUserId = WebSecurity.CurrentUserId;
                Service.ModifiedDate = DateTime.Now;
                Service.Type = "service";
                Service.IS_ALOW_BAN_AM = true;
                Service.NHOMSANPHAM_ID_LST = model.NHOMSANPHAM_ID.ToString();
                string k = model.Description_brief;
                if (model.Description_brief != null)
                {
                    Service.Meta_Description_brief = Regex.Replace(k.Replace("&nbsp;", " "), "<.*?>", String.Empty);//
                }
                var EquimentGroup = Request["EquimentGroup"];
                Service.EquimentGroup = EquimentGroup;
                var path = Helpers.Common.GetSetting("product-image-folder");
                var filepath = System.Web.HttpContext.Current.Server.MapPath("~" + path);
                if (Request.Files["file-image"] != null)
                {
                    var file = Request.Files["file-image"];
                    if (file.ContentLength > 0)
                    {
                        FileInfo fi = new FileInfo(Server.MapPath("~" + path) + Service.Image_Name);
                        if (fi.Exists)
                        {
                            fi.Delete();
                        }

                        string image_name = "service_" + Helpers.Common.ChuyenThanhKhongDau(Regex.Replace(Service.Name, @"\s+", "_")) + "." + file.FileName.Split('.').Last();
                        bool isExists = System.IO.Directory.Exists(filepath);
                        if (!isExists)
                            System.IO.Directory.CreateDirectory(filepath);
                        file.SaveAs(filepath + image_name);
                        Service.Image_Name = image_name;
                    }
                }

                ServiceRepository.UpdateService(Service);

                //ghi log 
                Erp.BackOffice.Controllers.HomeController.WriteLog(Service.Id, Service.Code, "đã cập nhật dịch vụ", "Service/Edit/" + Service.Id, Helpers.Common.CurrentUser.BranchId.Value);

                var _listdata = serviceStepRepository.GetAllServiceSteps().Where(x => x.ProductId == Service.Id).ToList();
                var _listdatadetail = serviceDetailRepository.GetAllServiceDetail().Where(x => x.ServiceId == Service.Id).ToList();
                if (model.DetailList.Any(x => x.Id == 0))
                {
                    //lưu danh sách thao tác thực hiện dịch vụ
                    foreach (var item in model.DetailList.Where(x => x.Id == 0 && !string.IsNullOrEmpty(x.Name)))
                    {
                        var action = new Domain.Sale.Entities.ServiceSteps();
                        action.IsDeleted = false;
                        action.CreatedUserId = WebSecurity.CurrentUserId;
                        action.ModifiedUserId = WebSecurity.CurrentUserId;
                        action.CreatedDate = DateTime.Now;
                        action.ModifiedDate = DateTime.Now;
                        action.Name = item.Name;
                        action.TotalMinute = item.TotalMinute == null ? 0 : item.TotalMinute;
                        action.Note = item.Note;
                        action.ProductId = Service.Id;
                        serviceStepRepository.InsertServiceSteps(action);
                    }
                }
                var _delete = _listdata.Where(id1 => !model.DetailList.ToList().Any(id2 => id2.Id == id1.Id)).ToList();
                foreach (var item in _delete)
                {
                    serviceStepRepository.DeleteServiceSteps(item.Id);
                }
                if (model.DetailList.Any(x => x.Id > 0))
                {
                    var update = _listdata.Where(id1 => model.DetailList.ToList().Any(id2 => id2.Id == id1.Id)).ToList();
                    //lưu danh sách thao tác thực hiện dịch vụ
                    foreach (var item in model.DetailList.Where(x => x.Id > 0 && x.Name != null))
                    {
                        var _update = update.FirstOrDefault(x => x.Id == item.Id);
                        _update.Name = item.Name;
                        _update.TotalMinute = item.TotalMinute == null ? 0 : item.TotalMinute;
                        _update.Note = item.Note;
                        _update.ProductId = Service.Id;
                        serviceStepRepository.UpdateServiceSteps(_update);
                    }
                }
                var _deletedetailservice = _listdatadetail.Where(id1 => !model.DetailServiceList.ToList().Any(id2 => id2.Id == id1.Id)).ToList();
                foreach (var item in _deletedetailservice)
                {
                    serviceDetailRepository.DeleteServiceDetail(item.Id);
                }
                if (model.DetailServiceList.Any(x => x.Id == 0))
                {
                    foreach (var item in model.DetailServiceList.Where(x => x.Id == 0 && x.ProductId > 0))
                    {
                        var servicedetail = new Domain.Sale.Entities.ServiceDetail();
                        servicedetail.IsDeleted = false;
                        servicedetail.CreatedUserId = WebSecurity.CurrentUserId;
                        servicedetail.ModifiedUserId = WebSecurity.CurrentUserId;
                        servicedetail.CreatedDate = DateTime.Now;
                        servicedetail.ModifiedDate = DateTime.Now;
                        servicedetail.ProductId = item.ProductId;
                        servicedetail.Quantity = item.Quantity;
                        servicedetail.ServiceId = Service.Id;
                        serviceDetailRepository.InsertServiceDetail(servicedetail);
                    }
                }
                if (model.DetailServiceList.Any(x => x.Id > 0))
                {
                    var update = _listdatadetail.Where(id1 => model.DetailServiceList.ToList().Any(id2 => id2.Id == id1.Id)).ToList();
                    foreach (var item in model.DetailServiceList.Where(x => x.Id > 0 && x.ProductId != null))
                    {
                        var _update = update.FirstOrDefault(x => x.Id == item.Id);
                        _update.Quantity = item.Quantity; ;
                        _update.ProductId = item.ProductId;
                        serviceDetailRepository.UpdateServiceDetail(_update);
                    }
                }
                if (string.IsNullOrEmpty(Request["IsPopup"]) == false)
                {
                    return RedirectToAction("Edit", new { Id = model.Id, IsPopup = Request["IsPopup"] });
                }
                TempData[Globals.SuccessMessageKey] = App_GlobalResources.Wording.UpdateSuccess;
                return RedirectToAction("Index");
            }

            return View(model);

            //if (Request.UrlReferrer != null)
            //    return Redirect(Request.UrlReferrer.AbsoluteUri);
            //return RedirectToAction("Index");
        }

        #endregion

        #region Delete
        [HttpPost]
        public ActionResult Delete()
        {
            Helpers.Common.ResetCacheYHL("MainPartial_Mainkissy,lineService_GroupDetails,lineproduct_GroupDetails,SearchSpa_GroupDetails,alllsproduct_GroupDetails,catelistproduct_GroupDetails");
            try
            {
                string id = Request["Delete"];
                string isNotUse = Request["checkUse"];
                if (isNotUse != null && isNotUse != "")
                {
                    string[] arrDeleteId = isNotUse.Split(',');
                    for (int i = 0; i < arrDeleteId.Count(); i++)
                    {
                        var item = ServiceRepository.GetProductById(int.Parse(arrDeleteId[i], CultureInfo.InvariantCulture));
                        if (item != null)
                        {
                            if (item.NotUse != true)
                            {
                                item.NotUse = true;
                            }
                            else
                            {
                                item.NotUse = false;
                            }

                            ServiceRepository.UpdateProduct(item);
                        }
                    }
                }
                else
                {

                    if (id != null)
                    {
                        var item = ServiceRepository.GetProductById(int.Parse(id, CultureInfo.InvariantCulture));
                        if (item != null)
                        {


                            item.IsDeleted = true;
                            ServiceRepository.UpdateService(item);
                        }
                    }
                    else
                    {
                        string idDeleteAll = Request["DeleteId-checkbox"];
                        string[] arrDeleteId = idDeleteAll.Split(',');
                        for (int i = 0; i < arrDeleteId.Count(); i++)
                        {
                            var item = ServiceRepository.GetProductById(int.Parse(arrDeleteId[i], CultureInfo.InvariantCulture));
                            if (item != null)
                            {
                                //if (item.CreatedUserId != Helpers.Common.CurrentUser.Id && Helpers.Common.CurrentUser.UserTypeId != 1)
                                //{
                                //    TempData["FailedMessage"] = "NotOwner";
                                //    return RedirectToAction("Index");
                                //}

                                item.IsDeleted = true;
                                ServiceRepository.UpdateService(item);
                            }
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

        #region UsingServiceLog
        public ViewResult UsingServiceLog(int? UsingServiceId)
        {
            var usingServiceLogDetail = usingServiceLogDetailRepository.GetAllvwUsingServiceLogDetail().Where(x => x.UsingServiceId.Value == UsingServiceId).ToList();
            List<UsingServiceLogDetailViewModel> model = usingServiceLogDetail.Select(i => new UsingServiceLogDetailViewModel
            {
                Id = i.Id,
                Code = i.Code,
                Name = i.Name,
                StaffId = i.StaffId,
                UsingServiceId = i.UsingServiceId,
                CreatedDate = i.CreatedDate,
                CreatedUserId = i.CreatedUserId,
                Status = i.Status,
                Type = i.Type,
                CustomerId = i.CustomerId,
                ProductInvoiceId = i.ProductInvoiceId,
                ServiceName = i.ServiceName,
                IsVote = i.IsVote
            }).OrderByDescending(x => x.CreatedDate).ToList();
            return View(model);
        }
        [HttpPost]
        public ActionResult Update(int StaffId, string check, string Status, string type)
        {
            string[] arrUsingServiceLogId = check.Split(',');
            for (int i = 0; i < arrUsingServiceLogId.Count(); i++)
            {
                if (arrUsingServiceLogId[i] != "")
                {
                    var log = new UsingServiceLogDetail();
                    log.StaffId = StaffId;
                    log.CreatedDate = DateTime.Now;
                    log.CreatedUserId = WebSecurity.CurrentUserId;
                    log.ModifiedDate = DateTime.Now;
                    log.ModifiedUserId = WebSecurity.CurrentUserId;
                    log.AssignedUserId = WebSecurity.CurrentUserId;
                    log.IsDeleted = false;
                    log.Status = Status;
                    log.Type = type;
                    log.UsingServiceId = int.Parse(arrUsingServiceLogId[i], CultureInfo.InvariantCulture);

                    var usingLog = usingServiceLogRepository.GetUsingServiceLogById(int.Parse(arrUsingServiceLogId[i], CultureInfo.InvariantCulture));
                    if (type == "usedservice")
                    {
                        if (usingLog.QuantityUsed < usingLog.Quantity)
                        {
                            usingLog.QuantityUsed = usingLog.QuantityUsed + 1;
                            usingServiceLogRepository.UpdateUsingServiceLog(usingLog);
                            usingServiceLogDetailRepository.InsertUsingServiceLogDetail(log);
                        }
                        else
                        {
                            return Content("error");
                        }

                    }
                    else
                    {
                        usingServiceLogDetailRepository.InsertUsingServiceLogDetail(log);
                    }

                }
            }
            return Content("success");
        }
        #endregion

        #region UsingService
        public ActionResult UsingService(int? InvoiceId, bool? IsNullLayOut)
        {
            IEnumerable<UsingServiceLogViewModel> usingservice = usingServiceLogRepository.GetAllvwUsingServiceLog().Where(x => x.ProductInvoiceId == InvoiceId)
                .Select(x => new UsingServiceLogViewModel
                {
                    CategoryCode = x.CategoryCode,
                    CustomerId = x.CustomerId,
                    Id = x.Id,
                    IsCombo = x.IsCombo,
                    ItemCategory = x.ItemCategory,
                    ItemCode = x.ItemCode,
                    ItemName = x.ItemName,
                    ProductCode = x.ProductCode,
                    ProductInvoiceCode = x.ProductInvoiceCode,
                    ProductInvoiceDate = x.ProductInvoiceDate,
                    ProductInvoiceId = x.ProductInvoiceId,
                    Quantity = x.Quantity,
                    QuantityUsed = x.QuantityUsed,
                    ProductName = x.ProductName,
                    ServiceComboId = x.ServiceComboId,
                    ServiceId = x.ServiceId,
                    ServiceInvoiceDetailId = x.ServiceInvoiceDetailId
                }).ToList();
            ViewBag.IsNullLayOut = IsNullLayOut;
            return View(usingservice);
        }
        #endregion

        #region UpdateNote
        //[HttpPost]
        public ActionResult UpdateUsingServiceLogDetail(int? Id, string status)
        {
            var usingservice = usingServiceLogDetailRepository.GetUsingServiceLogDetailById(Id.Value);
            if (usingservice != null)
            {
                usingservice.Status = status;
                usingservice.ModifiedDate = DateTime.Now;
                usingservice.ModifiedUserId = WebSecurity.CurrentUserId;
                usingServiceLogDetailRepository.UpdateUsingServiceLogDetail(usingservice);
                return Content("success");
            }
            else
            {
                return Content("error");
            }
        }
        #endregion
    }
}
