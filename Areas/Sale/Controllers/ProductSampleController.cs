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
using Erp.Domain.Staff.Entities;
using Erp.Domain.Account.Entities;
using Erp.Domain.Staff.Repositories;
using Erp.Domain.Staff.Interfaces;
using Erp.Domain.Account.Interfaces;
using Erp.BackOffice.Account.Models;
using Erp.BackOffice.App_GlobalResources;
using System.Transactions;

namespace Erp.BackOffice.Sale.Controllers
{
    [Authorize]
    [InitializeSimpleMembership]
    [Erp.BackOffice.Helpers.NoCacheHelper]
    public class ProductSampleController : Controller
    {
        private readonly IProductSampleRepository ProductSampleRepository;
        private readonly IProductSampleDetailRepository ProductSampleDetailRepository;
        private readonly IUserRepository userRepository;
        // private readonly IStaffsRepository staffsRepository;
        private readonly ICustomerRepository customersRepository;
        private readonly IProductOrServiceRepository productOrServiceRepository;
        //private readonly IUserRepository UserRepository;
        private readonly IProductInvoiceRepository productInvoiceRepository;
        public ProductSampleController(
            IProductSampleRepository _ProductSample
            , IUserRepository _user,
            ICustomerRepository _customesRepository,
            IProductOrServiceRepository _pOrServiceRepository,
            IProductSampleDetailRepository _ProductSampleDetailRepository,
            IProductInvoiceRepository invoice
            )
        {
            ProductSampleRepository = _ProductSample;
            userRepository = _user;
            customersRepository = _customesRepository;
            productOrServiceRepository = _pOrServiceRepository;
            ProductSampleDetailRepository = _ProductSampleDetailRepository;
            // UserRepository = _UserRepository;
            productInvoiceRepository = invoice;
        }

        #region Index

        public ViewResult Index(string Code, string startDate, string endDate, string Status, int? BranchId, int? CreateUserId)
        {

            IEnumerable<ProductSampleViewModel> q = ProductSampleRepository.GetvwAllProductSample()
                .Select(item => new ProductSampleViewModel
                {
                    Id = item.Id,
                    Status = item.Status,
                    ProductId = item.ProductId,
                    Code = item.Code,
                    Note = item.Note,
                    CreatedUserId = item.CreatedUserId,
                    CreatedDate = item.CreatedDate,
                    ModifiedUserId = item.ModifiedUserId,
                    ModifiedDate = item.ModifiedDate,
                    BranchId=item.BranchId,
                    ProductCode=item.ProductCode,
                    ProductName=item.ProductName

                }).OrderByDescending(m => m.CreatedDate);
            if (!string.IsNullOrEmpty(Code))
            {
                Code = Helpers.Common.ChuyenThanhKhongDau(Code);
                q = q.Where(x => Helpers.Common.ChuyenThanhKhongDau(x.Code).Contains(Code)).ToList();
            }
            if (!string.IsNullOrEmpty(Status))
            {
                q = q.Where(x => x.Status == Status).ToList();
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
            if (CreateUserId != null && CreateUserId.Value > 0)
            {
                q = q.Where(x => x.CreatedUserId == CreateUserId).ToList();
            }
            ViewBag.SuccessMessage = TempData["SuccessMessage"];
            ViewBag.FailedMessage = TempData["FailedMessage"];
            ViewBag.AlertMessage = TempData["AlertMessage"];
            return View(q);
        }
        #endregion

        #region Create
        public ViewResult Create(int? Id)
        {
            var model = new ProductSampleViewModel();
            model.DetailList = new List<ProductSampleDetailViewModel>();
            if (Id != null && Id.Value > 0)
            {
                var data = ProductSampleRepository.GetvwProductSampleById(Id.Value);
                if (data != null && data.IsDeleted != true)
                {
                    AutoMapper.Mapper.Map(data, model);
                    model.DetailList = ProductSampleDetailRepository.GetvwAllProductSampleDetail().Where(x => x.ProductSampleId == Id)
                        .Select(x => new ProductSampleDetailViewModel { 
                        Id=x.Id,
                        CustomerCode=x.CustomerCode,
                        CustomerId=x.CustomerId,
                        CustomerImage=x.CustomerImage,
                        FirstName=x.FirstName,
                        LastName=x.LastName,
                        Note=x.Note,
                        ProductLinkId=x.ProductLinkId,
                        ProductSampleId=x.ProductSampleId
                        }).ToList();
                    return View(model);
                }
            }
            return View(model);
        }

        #region GetListCusJson
        [AllowAnonymous]
        public JsonResult GetListCusJson()
        {
            var pathimg = Helpers.Common.GetSetting("customer-image-folder");
            var q = customersRepository.GetAllvwCustomer().Where(x => x.CreatedUserId == Helpers.Common.CurrentUser.Id).Select(x => new
            {
                x.Id,
                x.Code,
                x.FirstName,
                x.LastName,
                x.Image
            }).OrderBy(x => x.LastName).ToList();
            return Json(q.Select(x => new { x.Id, Code = x.Code, Image = Helpers.Common.KiemTraTonTaiHinhAnhHoapd(x.Image, "customer-image-folder", "customerERR", pathimg), Name = x.FirstName + " " + x.LastName, Text = x.FirstName + " " + x.LastName + "(" + x.Code + ")" }), JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region GetListProductZeroPrice
        [AllowAnonymous]
        public JsonResult GetListProductZeroPrice()
        {
            var pathimg = Helpers.Common.GetSetting("product-image-folder");
            var q = productOrServiceRepository.GetAllvwProductAndService().Where(x => x.PriceOutbound == 0).Select(item => new
            {
                item.Id,
                item.Name,
                item.Type,
                item.Code,
                item.PriceOutbound,
                item.ProductGroup,
                item.Unit,
                item.Image_Name
            }).OrderBy(x => x.Name).ToList();
            return Json(q.Select(item => new { item.Id, Code = item.Code, Image = Helpers.Common.KiemTraTonTaiHinhAnhHoapd(item.Image_Name, "product-image-folder", "product",pathimg), Type = item.Type, Unit = item.Unit, Price = item.PriceOutbound, Text = item.Code + " - " + item.Name ,Note=(item.Type=="product"?"SP":"DV"), Name = item.Name, Value = item.Id }), JsonRequestBehavior.AllowGet);
        }
        #endregion

        [HttpPost]
        public ActionResult Create(ProductSampleViewModel model)
        {
            ProductSample sample = null;

                  using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.Snapshot }))
            {
                try
                {
                    if (ModelState.IsValid)
                    {
                      
                        if (model.Id > 0)
                        {
                            sample = ProductSampleRepository.GetProductSampleById(model.Id);
                        }
                        if (sample != null)
                        {
                            sample.Note = model.Note;
                            sample.ProductId = model.ProductId;
                            sample.ModifiedUserId = WebSecurity.CurrentUserId;
                            sample.ModifiedDate = DateTime.Now;
                            ProductSampleRepository.UpdateProductSample(sample);
                        }
                        else
                        {
                            sample = new ProductSample();
                            AutoMapper.Mapper.Map(model, sample);
                            sample.IsDeleted = false;
                            sample.CreatedUserId = WebSecurity.CurrentUserId;
                            sample.ModifiedUserId = WebSecurity.CurrentUserId;
                            sample.AssignedUserId = WebSecurity.CurrentUserId;
                            sample.CreatedDate = DateTime.Now;
                            sample.ModifiedDate = DateTime.Now;
                            sample.BranchId = Erp.BackOffice.Helpers.Common.CurrentUser.BranchId;
                            sample.Status = "pending";
                            sample.Code = Erp.BackOffice.Helpers.Common.GetOrderNo("ProductSample", model.Code);
                            Erp.BackOffice.Helpers.Common.SetOrderNo("ProductSample");
                            ProductSampleRepository.InsertProductSample(sample);
                        }
                        var _listdata = ProductSampleDetailRepository.GetAllProductSampleDetail().Where(x => x.ProductSampleId == sample.Id).ToList();
                        if (model.DetailList.Any(x => x.Id == 0))
                        {
                            //lưu danh sách thao tác thực hiện dịch vụ
                            foreach (var item in model.DetailList.Where(x => x.Id == 0 && x.CustomerId > 0))
                            {
                                var ins = new ProductSampleDetail();
                                AutoMapper.Mapper.Map(item, ins);
                                ins.IsDeleted = false;
                                ins.CreatedUserId = WebSecurity.CurrentUserId;
                                ins.ModifiedUserId = WebSecurity.CurrentUserId;
                                ins.AssignedUserId = WebSecurity.CurrentUserId;
                                ins.CreatedDate = DateTime.Now;
                                ins.ModifiedDate = DateTime.Now;
                                ins.Status = Wording.ProductSampleDetailStatus_Make;
                                ins.ProductSampleId = sample.Id;
                                ProductSampleDetailRepository.InsertProductSampleDetail(ins);
                            }
                        }
                        var _delete = _listdata.Where(id1 => !model.DetailList.ToList().Any(id2 => id2.Id == id1.Id)).ToList();
                        if (_delete.Any())
                        {
                            foreach (var item in _delete)
                            {
                                ProductSampleDetailRepository.DeleteProductSampleDetail(item.Id);
                            }
                        }
                        if (model.DetailList.Any(x => x.Id > 0))
                        {
                            var update = _listdata.Where(id1 => model.DetailList.ToList().Any(id2 => id2.Id == id1.Id)).ToList();
                            //lưu danh sách thao tác thực hiện dịch vụ
                            foreach (var item in model.DetailList.Where(x => x.Id > 0 && x.CustomerId > 0))
                            {
                                var _update = update.FirstOrDefault(x => x.Id == item.Id);
                                _update.Note = item.Note;
                                _update.ModifiedUserId = WebSecurity.CurrentUserId;
                                _update.ModifiedDate = DateTime.Now;
                                ProductSampleDetailRepository.UpdateProductSampleDetail(_update);
                            }
                        }
                        
                        scope.Complete();
                    }
                }
                catch (DbUpdateException)
                {
                    TempData[Globals.FailedMessageKey] = App_GlobalResources.Wording.Error;
                    return View(model);
                }

            }
                  return RedirectToAction("Detail", new { Id = sample.Id });
        }
        #endregion

        #region Detail
        public ActionResult Detail(int Id)
        {
            var ProductSample = ProductSampleRepository.GetvwProductSampleById(Id);
            if (ProductSample != null)
            {
                var model = new ProductSampleViewModel();
                model.DetailList = new List<ProductSampleDetailViewModel>();
                var invoice = productInvoiceRepository.GetAllvwInvoiceDetails();
                AutoMapper.Mapper.Map(ProductSample, model);

                var detailList = ProductSampleDetailRepository.GetvwAllProductSampleDetail().Where(x=>x.ProductSampleId==Id).ToList();//lấy tất cả danh sách ProductSampleDetail theo ProductSampleId
                AutoMapper.Mapper.Map(detailList, model.DetailList);
                if (model.DetailList.Any())
                {
                    foreach (var item in model.DetailList)
                    {
                        item.InvoiceList = new List<ProductInvoiceDetailViewModel>();
                        item.InvoiceList = invoice.Where(x => x.ProductId == item.ProductLinkId && x.ProductInvoiceDate > item.CreatedDate&&x.CustomerId==item.CustomerId).Select(x => new ProductInvoiceDetailViewModel
                        {
                            Id=x.Id,
                            ProductId=x.ProductId,
                            ProductImage=x.ProductImage,
                            ProductInvoiceCode=x.ProductInvoiceCode,
                            ProductInvoiceDate=x.ProductInvoiceDate,
                            LoCode=x.LoCode,
                            ExpiryDate=x.ExpiryDate,
                            Price=x.Price,
                            Quantity=x.Quantity,
                            Amount=x.Amount,
                            ProductName=x.ProductName,
                            ProductCode=x.ProductCode,
                            Unit=x.Unit
                        }).ToList();
                    }

                }
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
                if(id!= null){
                    var item = ProductSampleRepository.GetProductSampleById(int.Parse(id, CultureInfo.InvariantCulture));
                    if (item != null)
                    {
                        item.IsDeleted = true;
                        ProductSampleRepository.UpdateProductSample(item);
                    }
                }
                else
                {
                    string idDeleteAll = Request["DeleteId-checkbox"];
                    string[] arrDeleteId = idDeleteAll.Split(',');
                    for (int i = 0; i < arrDeleteId.Count(); i++)
                    {
                        var item = ProductSampleRepository.GetProductSampleById(int.Parse(arrDeleteId[i], CultureInfo.InvariantCulture));
                        if (item != null)
                        {


                            item.IsDeleted = true;
                            ProductSampleRepository.UpdateProductSample(item);
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
    }
}
