using System.Globalization;
using Erp.BackOffice.Sale.Models;
using Erp.BackOffice.Filters;
using Erp.Domain.Entities;
using Erp.Domain.Interfaces;
using Erp.Domain.Sale.Interfaces;
using Erp.Domain.Sale.Repositories;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Erp.Utilities;
using WebMatrix.WebData;
using Erp.BackOffice.Helpers;
using Erp.Domain.Sale.Entities;
using Erp.Domain.Sale.Interfaces;
using Erp.Domain.Staff.Interfaces;
using System.Transactions;
using System.Web;

namespace Erp.BackOffice.Sale.Controllers
{
    [Authorize]
    [InitializeSimpleMembership]
    [Erp.BackOffice.Helpers.NoCacheHelper]
    public class WarehouseController : Controller
    {
        private readonly IWarehouseRepository WarehouseRepository;
        private readonly IBranchRepository branchRepository;
        private readonly IObjectAttributeRepository ObjectAttributeRepository;
        private readonly IUserRepository userRepository;
        private readonly ITemplatePrintRepository templatePrintRepository;

        public WarehouseController(
            IWarehouseRepository _Warehouse
            , IBranchRepository _branch
            , IObjectAttributeRepository _ObjectAttribute
            , IUserRepository _user
            , ITemplatePrintRepository _templatePrint
            )
        {
            WarehouseRepository = _Warehouse;
            branchRepository = _branch;
            ObjectAttributeRepository = _ObjectAttribute;
            userRepository = _user;
            templatePrintRepository = _templatePrint;
        }

        #region Index

        public ViewResult Index(string txtSearch, string txtAddress)
        {
            IEnumerable<WarehouseViewModel> q = WarehouseRepository.GetAllWarehouse().AsEnumerable()
                .Select(item => new WarehouseViewModel
                {
                    Id = item.Id,
                    CreatedUserId = item.CreatedUserId,
                    CreatedDate = item.CreatedDate,
                    ModifiedUserId = item.ModifiedUserId,
                    ModifiedDate = item.ModifiedDate,
                    Name = item.Name,
                    Code = item.Code,
                    Address = item.Address,
                    BranchId = item.BranchId
                }).OrderByDescending(m => m.ModifiedDate).ToList();
            foreach (var i in q)
            {
                if (i.BranchId != null)
                {
                    var br = branchRepository.GetBranchById(i.BranchId.Value);
                    if (br != null && br.Name != null)
                    {
                        i.BranchName = br.Name;
                    }
                    else
                    {
                        i.BranchName = "No Title";
                    }
                }
                else
                {
                    continue;
                }
            }
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

            int? intBrandID = int.Parse(strBrandID);

            if (intBrandID != null && intBrandID.Value > 0)
            {
                q = q.Where(x => x.BranchId == intBrandID).ToList();
            }
            if (Helpers.Common.CurrentUser.BranchId!= null && Helpers.Common.CurrentUser.BranchId.Value>0)
            {
                q = q.Where(x => x.BranchId == Helpers.Common.CurrentUser.BranchId.Value).OrderByDescending(m => m.ModifiedDate);
            }

            if (string.IsNullOrEmpty(txtSearch) == false || string.IsNullOrEmpty(txtAddress) == false)
            {
                txtSearch = txtSearch == "" ? "~" : Erp.BackOffice.Helpers.Common.ChuyenThanhKhongDau(txtSearch);
                txtAddress = txtAddress == "" ? "~" : txtAddress.ToLower();
                q = q.Where(x => Erp.BackOffice.Helpers.Common.ChuyenThanhKhongDau(x.Name).Contains(txtSearch) || x.Code.ToLowerOrEmpty().Contains(txtSearch) || Erp.BackOffice.Helpers.Common.ChuyenThanhKhongDau(x.Address).Contains(txtAddress));
            }
            //if (!Erp.BackOffice.Filters.SecurityFilter.IsAdmin())
            //{
            //    //lấy tất cả kho mà user quản lý hiện ra
            //    // admin thì hiện tất cả
            //    q = q.Where(x => ("," + Helpers.Common.CurrentUser.DrugStore + ",").Contains("," + x.BranchId + ",") == true);
            //}
            ViewBag.SuccessMessage = TempData["SuccessMessage"];
            ViewBag.FailedMessage = TempData["FailedMessage"];
            ViewBag.AlertMessage = TempData["AlertMessage"];
            return View(q);
        }
        #endregion

        #region Print
        public List<WarehouseViewModel> IndexPrint(string txtSearch, string txtAddress)
        {
            var q = WarehouseRepository.GetAllWarehouse()
            .Select(item => new WarehouseViewModel
            {
                Id = item.Id,
                CreatedUserId = item.CreatedUserId,
                CreatedDate = item.CreatedDate,
                ModifiedUserId = item.ModifiedUserId,
                ModifiedDate = item.ModifiedDate,
                Name = item.Name,
                Code = item.Code,
                Address = item.Address,
                BranchId = item.BranchId
            }).OrderByDescending(m => m.ModifiedDate).ToList();
            foreach (var i in q)
            {
                if (i.BranchId != null)
                {
                    var br = branchRepository.GetBranchById(i.BranchId.Value);
                    //if tr0ng day
                    if ( br != null && i.BranchId > 0 )
                    {
                        i.BranchName = br.Name;
                    }
                    else
                    {
                        i.BranchName = "No Title";
                    }
                }
                else
                {
                    continue;
                }
            }
            if (Helpers.Common.CurrentUser.BranchId != null && Helpers.Common.CurrentUser.BranchId.Value > 0)
            {
                q = q.Where(x => x.BranchId == Helpers.Common.CurrentUser.BranchId.Value).OrderByDescending(m => m.ModifiedDate).ToList();
            }

            //Lọc theo từ khoá
            if (string.IsNullOrEmpty(txtSearch) == false || string.IsNullOrEmpty(txtAddress) == false)
            {
                txtSearch = txtSearch == "" ? "~" : Erp.BackOffice.Helpers.Common.ChuyenThanhKhongDau(txtSearch);
                txtAddress = txtAddress == "" ? "~" : txtAddress.ToLower();
                q = q.Where(x => Erp.BackOffice.Helpers.Common.ChuyenThanhKhongDau(x.Name).Contains(txtSearch) || x.Code.ToLowerOrEmpty().Contains(txtSearch) || Erp.BackOffice.Helpers.Common.ChuyenThanhKhongDau(x.Address).Contains(txtAddress)).ToList();
            }

            return q;
        }

        public ActionResult PrintWarehouse(string txtSearch, string txtAddress, bool ExportExcel = false)
        {
            var data = IndexPrint(txtSearch, txtAddress);

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
            model.Content = model.Content.Replace("{DataTable}", buildHtmlDanhSachNhaKho(data));
            model.Content = model.Content.Replace("{System.Logo}", ImgLogo);
            model.Content = model.Content.Replace("{System.CompanyName}", company);
            model.Content = model.Content.Replace("{System.AddressCompany}", address);
            model.Content = model.Content.Replace("{System.PhoneCompany}", phone);
            model.Content = model.Content.Replace("{System.Fax}", fax);
            model.Content = model.Content.Replace("{System.BankCodeCompany}", bankcode);
            model.Content = model.Content.Replace("{System.BankNameCompany}", bankname);
            model.Content = model.Content.Replace("{PrintDate}", DateTime.Now.ToString("dd/MM/yyyy HH:mm"));
            model.Content = model.Content.Replace("{Title}", "Danh sách nhà kho");
            if (ExportExcel)
            {
                Response.AppendHeader("content-disposition", "attachment;filename=" + "DS_Nhakho" + DateTime.Now.ToString("yyyyMMdd") + ".xls");
                Response.Charset = "";
                Response.Cache.SetCacheability(HttpCacheability.NoCache);
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.Write(model.Content);
                Response.End();
            }




            return View(model);
        }

        string buildHtmlDanhSachNhaKho(List<WarehouseViewModel> detailList)
        {
            string detailLists = "<table border=\"1\" class=\"invoice-detail\">\r\n";
            detailLists += "<thead>\r\n";
            detailLists += "	<tr>";
            detailLists += "		<th>STT</th>";
            detailLists += "		<th>Tên kho</th>";
            detailLists += "		<th>Chi nhánh</th>";
            detailLists += "		<th>Mã kho</th>";
            detailLists += "		<th>Địa chỉ</th>";
            detailLists += "		<th>Ngày tạo</th>";
            detailLists += "		<th>Ngày cập nhật</th>";
            detailLists += "	</tr>";
            detailLists += "</thead>\r\n";
            detailLists += "<tbody>\r\n";
            var index = 1;

            foreach (var item in detailList)
            {
                detailLists += "<tr>\r\n"
                + "<td class=\"text-center orderNo\">" + (index++) + "</td>\r\n"
                + "<td class=\"text-left \">" + item.Name + "</td>\r\n"
                + "<td class=\"text-left \">" + item.BranchName + "</td>\r\n"
                + "<td class=\"text-left \">" + item.Code + "</td>\r\n"
                + "<td class=\"text-left \">" + item.Address + "</td>\r\n"
                + "<td>" + (item.CreatedDate.HasValue ? item.CreatedDate.Value.ToString("dd/MM/yyyy HH:mm") : "") + "</td>"
                + "<td>" + (item.ModifiedDate.HasValue ? item.ModifiedDate.Value.ToString("dd/MM/yyyy HH:mm") : "") + "</td>"
                + "</tr>\r\n";
            }

            detailLists += "</tbody>\r\n";
            detailLists += "<tfoot style=\"font-weight:bold\">\r\n";
            detailLists += "</tfoot>\r\n</table>\r\n";
            return detailLists;
        }

        public ActionResult Print(int Id)
        {
            var model = new TemplatePrintViewModel();
            //lấy logo công ty
            //var logo = Erp.BackOffice.Helpers.Common.GetSetting("LogoCompany");
            var company = Erp.BackOffice.Helpers.Common.GetSetting("companyName");
            var address = Erp.BackOffice.Helpers.Common.GetSetting("addresscompany");
            var phone = Erp.BackOffice.Helpers.Common.GetSetting("phonecompany");
            var fax = Erp.BackOffice.Helpers.Common.GetSetting("faxcompany");
            var bankcode = Erp.BackOffice.Helpers.Common.GetSetting("bankcode");
            var bankname = Erp.BackOffice.Helpers.Common.GetSetting("bankname");
            //var ImgLogo = "<div class=\"logo\"><img src=" + logo + " height=\"73\" /></div>";
            //lấy phiếu chi.
            var warehouse = WarehouseRepository.GetWarehouseById(Id);

            //lấy template phiếu xuất.
            var template = templatePrintRepository.GetAllTemplatePrint().Where(x => x.Code.Contains("Warehouse")).OrderByDescending(x => x.CreatedDate).FirstOrDefault();
            //truyền dữ liệu vào template.
            model.Content = template.Content;
            model.Content = model.Content.Replace("{CreatedUserId}", warehouse.CreatedUserId.ToString());
            model.Content = model.Content.Replace("{CreatedDate}", warehouse.CreatedDate.Value.ToString("dd/MM/yyyy HH:mm"));
            model.Content = model.Content.Replace("{ModifiedUserId}", warehouse.ModifiedUserId.ToString());
            model.Content = model.Content.Replace("{ModifiedDate}", warehouse.ModifiedDate.Value.ToString("dd/MM/yyyy HH:mm"));
            model.Content = model.Content.Replace("{Name}", warehouse.Name);
            model.Content = model.Content.Replace("Code}", warehouse.Code);
            model.Content = model.Content.Replace("{Address}", warehouse.Address);

            model.Content = model.Content.Replace("{System.CompanyName}", company);
            model.Content = model.Content.Replace("{System.AddressCompany}", address);
            model.Content = model.Content.Replace("{System.PhoneCompany}", phone);
            model.Content = model.Content.Replace("{System.Fax}", fax);
            model.Content = model.Content.Replace("{System.BankCodeCompany}", bankcode);
            model.Content = model.Content.Replace("{System.BankNameCompany}", bankname);
            return View(model);
        }
        #endregion

        #region Create
        public ViewResult Create()
        {
            var model = new WarehouseViewModel();
            model.IsSale = true;

            // ViewBag.branchList = branchRepository.GetAllBranch().Where(x=>x.ParentId!=null&&x.ParentId>0).AsEnumerable().Select(x => new SelectListItem { Value = x.Id + "", Text = x.Name });

            return View(model);
        }

        [HttpPost]
        public ActionResult Create(WarehouseViewModel model)
        {
            if (ModelState.IsValid)
            {
                using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.Snapshot }))
                {
                    try
                    {
                        var ListUserId = Request["ListUserId"];
                        var Warehouse = new Domain.Sale.Entities.Warehouse();
                        AutoMapper.Mapper.Map(model, Warehouse);
                        Warehouse.IsDeleted = false;
                        Warehouse.CreatedUserId = WebSecurity.CurrentUserId;
                        Warehouse.ModifiedUserId = WebSecurity.CurrentUserId;
                        Warehouse.CreatedDate = DateTime.Now;
                        Warehouse.ModifiedDate = DateTime.Now;
                        Warehouse.KeeperId = ListUserId;
                        Warehouse.Categories = Request["Categories"];
                        Warehouse.IsSale = model.IsSale;
                        //Warehouse.BranchId = Helpers.Common.CurrentUser.BranchId;
                        if(Warehouse.BranchId == null)
                        {
                            Warehouse.BranchId = 0;
                        }

                        //tạo đặc tính động cho kho hàng nếu có danh sách đặc tính động truyền vào và đặc tính đó phải có giá trị
                        ObjectAttributeController.CreateOrUpdateForObject(Warehouse.Id, model.AttributeValueList);

                        WarehouseRepository.InsertWarehouse(Warehouse);

                        //ghi log 
                        Erp.BackOffice.Controllers.HomeController.WriteLog(Warehouse.Id, Warehouse.Code, "đã tạo kho", "Warehouse/Edit/" + Warehouse.Id, Helpers.Common.CurrentUser.BranchId.Value);

                        scope.Complete();
                        if (string.IsNullOrEmpty(Request["IsPopup"]) == false)
                        {
                            ViewBag.closePopup = "close and append to page parent";
                            model.Id = Warehouse.Id;
                            return View(model);
                        }


                        TempData[Globals.SuccessMessageKey] = App_GlobalResources.Wording.InsertSuccess;
                        return RedirectToAction("Index");
                    }
                    catch (DbUpdateException)
                    {
                        return Content("Fail");
                    }
                }
            }
            return View(model);
        }
        #endregion

        #region Edit
        public ActionResult Edit(int? Id)
        {
            var Warehouse = WarehouseRepository.GetWarehouseById(Id.Value);
            if (Warehouse != null && Warehouse.IsDeleted != true)
            {
                var model = new WarehouseViewModel();
                AutoMapper.Mapper.Map(Warehouse, model);
                List<string> listCategories = new List<string>();
                if (!string.IsNullOrEmpty(Warehouse.Categories))
                {
                    listCategories = Warehouse.Categories.Split(',').ToList();
                }
                ViewBag.listCategories = listCategories;

                List<string> listKeeperID = new List<string>();
                if (!string.IsNullOrEmpty(Warehouse.KeeperId))
                {
                    listKeeperID = Warehouse.KeeperId.Split(',').ToList();
                }
                ViewBag.listKeeperID = listKeeperID;
                return View(model);
            }

            if (Request.UrlReferrer != null)
                return Redirect(Request.UrlReferrer.AbsoluteUri);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult Edit(WarehouseViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (Request["Submit"] == "Save")
                {
                    using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.Snapshot }))
                    {
                        try
                        {
                            var ListUserId = Request["ListUserId"];
                            var Warehouse = WarehouseRepository.GetWarehouseById(model.Id);
                            AutoMapper.Mapper.Map(model, Warehouse);
                            Warehouse.KeeperId = ListUserId;
                            Warehouse.Categories = Request["Categories"];
                            Warehouse.ModifiedUserId = WebSecurity.CurrentUserId;
                            Warehouse.ModifiedDate = DateTime.Now;
                            Warehouse.IsSale = model.IsSale;
                            //tạo đặc tính động cho kho hàng nếu có danh sách đặc tính động truyền vào và đặc tính đó phải có giá trị
                            ObjectAttributeController.CreateOrUpdateForObject(Warehouse.Id, model.AttributeValueList);

                            WarehouseRepository.UpdateWarehouse(Warehouse);
                            //ghi log 
                            Erp.BackOffice.Controllers.HomeController.WriteLog(Warehouse.Id, Warehouse.Code, "đã cập nhật kho", "Warehouse/Edit/" + Warehouse.Id, Helpers.Common.CurrentUser.BranchId.Value);

                            scope.Complete();
                            if (string.IsNullOrEmpty(Request["IsPopup"]) == false)
                            {
                                return RedirectToAction("Edit", new { Id = model.Id, IsPopup = Request["IsPopup"] });
                            }

                            TempData[Globals.SuccessMessageKey] = App_GlobalResources.Wording.UpdateSuccess;
                            return RedirectToAction("Index");
                        }
                        catch (DbUpdateException)
                        {
                            return Content("Fail");
                        }
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

        #region Delete
        [HttpPost]
        public ActionResult Delete()
        {
            try
            {
                string idDeleteAll = Request["DeleteId-checkbox"];
                string[] arrDeleteId = idDeleteAll.Split(',');
                for (int i = 0; i < arrDeleteId.Count(); i++)
                {
                    var item = WarehouseRepository.GetWarehouseById(int.Parse(arrDeleteId[i], CultureInfo.InvariantCulture));
                    if (item != null)
                    {
                        item.IsDeleted = true;
                        WarehouseRepository.UpdateWarehouse(item);
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

        #region
        public JsonResult GetListUserByBranchId(int? branchId)
        {
            if (branchId == null)
                return Json(new List<int>(), JsonRequestBehavior.AllowGet);

            var list = userRepository.GetAllUsers()
                .Select(x => new
                {
                    e = x.FullName,
                    a = x.Id,
                    b = x.UserTypeId,
                    c = x.UserName,
                    d = x.Status,
                    f = x.BranchId,
                });
            if (!Erp.BackOffice.Filters.SecurityFilter.IsAdmin())
            {
                if (branchId != null && branchId.Value > 0)
                {
                    list = list.Where(x => x.f == branchId);
                }
                else
                {
                    list = null;
                }
            }
            else
            {
                if (branchId != null && branchId.Value > 0)
                {
                    list = list.Where(x => x.f == branchId);
                }
            }
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}
