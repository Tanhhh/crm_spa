using Erp.BackOffice.Filters;
using Erp.BackOffice.Staff.Models;
using Erp.Domain.Interfaces;
using Erp.Domain.Staff.Interfaces;
using Erp.Domain.Staff.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebMatrix.WebData;
using Erp.Utilities;
using System.Globalization;
using System.Data.Entity.Infrastructure;
using Erp.BackOffice.Sale.Models;
using Erp.Domain.Sale.Interfaces;
using System.Web;

namespace Erp.BackOffice.Staff.Controllers
{
    [Authorize]
    [InitializeSimpleMembership]
    [Erp.BackOffice.Helpers.NoCacheHelper]
    public class StaffEquipmentController : Controller
    {
        private readonly IStaffEquipmentRepository StaffEquipmentRepository;
        private readonly IUserRepository userRepository;
        private readonly ITemplatePrintRepository templatePrintRepository;
        
        public StaffEquipmentController(IStaffEquipmentRepository _StaffEquipment, IUserRepository _user, ITemplatePrintRepository _templatePrint)
        {
            StaffEquipmentRepository = _StaffEquipment;
            userRepository = _user;
            templatePrintRepository = _templatePrint;
        }

        #region Index

        public ViewResult Index(int? BranchId, string code, string name, string group, string info)
        {
            //if (BranchId == null)
            //{
            //    BranchId= Erp.BackOffice.Helpers.Common.CurrentUser.BranchId;
            //}
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
            //IEnumerable<StaffEquipmentViewModel> q = StaffEquipmentRepository.GetAllStaffEquipment()
            //    .Select(item => new StaffEquipmentViewModel
            //    {
            //        Id = item.Id,
            //        CreatedUserId = item.CreatedUserId,
            //        //CreatedUserName = item.CreatedUserName,
            //        CreatedDate = item.CreatedDate,
            //        ModifiedUserId = item.ModifiedUserId,
            //        //ModifiedUserName = item.ModifiedUserName,
            //        ModifiedDate = item.ModifiedDate,
            //        Name = item.Name,
            //        BranchId=item.BranchId,
            //        Code=item.Code,
            //        InspectionDate=item.InspectionDate,
            //        Group=item.Group
            //    }).OrderByDescending(m => m.ModifiedDate);
            List<StaffEquipmentViewModel> model = new List<StaffEquipmentViewModel>();
            
            List<StaffEquipment> q = StaffEquipmentRepository.GetListAllStaffEquipment().Where(p=>p.BranchId== BranchId).ToList();
            if (q.Count() > 0)
            {
                foreach (var item in q)
                {
                    StaffEquipmentViewModel tmp = new StaffEquipmentViewModel();
                    tmp.Id = item.Id;
                    tmp.CreatedUserId = item.CreatedUserId;
                    
                    tmp.CreatedDate = item.CreatedDate;
                    tmp.ModifiedUserId = item.ModifiedUserId;
             
                    tmp.ModifiedDate = item.ModifiedDate;
                    tmp.Name = item.Name;
                    tmp.BranchId = item.BranchId;
                    tmp.Code = item.Code;
                    tmp.InspectionDate = item.InspectionDate;
                    tmp.Group = item.Group;
                    if (item.Status == true)
                    {
                        tmp.Status = "Đang sử dụng";
                    }
                    else
                    {
                        tmp.Status = "Chưa Sử Dụng";
                    }
                    model.Add(tmp);
                }
            }
            //if (!Erp.BackOffice.Filters.SecurityFilter.IsAdmin())
            //{
            //    model = model.Where(x => x.BranchId == Erp.BackOffice.Helpers.Common.CurrentUser.BranchId).ToList();
            //}
            if (BranchId != null && BranchId.Value > 0)
            {
                model = model.Where(x => x.BranchId == BranchId).ToList();
            }
            if (!string.IsNullOrEmpty(group))
            {
                model = model.Where(x => x.Group == group).ToList();
            }
            //if (!string.IsNullOrEmpty(name))
            //{
            //    name = Helpers.Common.ChuyenThanhKhongDau(name);
            //    model = model.Where(x => Helpers.Common.ChuyenThanhKhongDau(x.Name).Contains(name)).ToList();
            //}
            //if (!string.IsNullOrEmpty(code))
            //{
            //    code = Helpers.Common.ChuyenThanhKhongDau(code);
            //    model = model.Where(x => Helpers.Common.ChuyenThanhKhongDau(x.Code).Contains(code)).ToList();
            //}
            if (!string.IsNullOrEmpty(info))
            {
                model = model.Where(x => Helpers.Common.ChuyenThanhKhongDau(x.Name).Contains(Helpers.Common.ChuyenThanhKhongDau(info)) || Helpers.Common.ChuyenThanhKhongDau(x.Code).Contains(Helpers.Common.ChuyenThanhKhongDau(info))).ToList();
            }
            ViewBag.SuccessMessage = TempData["SuccessMessage"];
            ViewBag.FailedMessage = TempData["FailedMessage"];
            ViewBag.AlertMessage = TempData["AlertMessage"];
            return View(model);
        }

        #endregion

        #region ExportExcel
        public List<StaffEquipmentViewModel> IndexExport(string info, string group, int? BranchId)
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

            List<StaffEquipmentViewModel> model = new List<StaffEquipmentViewModel>();

            var q = StaffEquipmentRepository.GetListAllStaffEquipment().Where(p => p.BranchId == BranchId).ToList();

            if (q.Count() > 0)
            {
                foreach (var item in q)
                {
                    StaffEquipmentViewModel tmp = new StaffEquipmentViewModel();
                    tmp.Id = item.Id;
                    tmp.CreatedUserId = item.CreatedUserId;

                    tmp.CreatedDate = item.CreatedDate;
                    tmp.ModifiedUserId = item.ModifiedUserId;

                    tmp.ModifiedDate = item.ModifiedDate;
                    tmp.Name = item.Name;
                    tmp.BranchId = item.BranchId;
                    tmp.Code = item.Code;
                    tmp.InspectionDate = item.InspectionDate;
                    tmp.Group = item.Group;
                    if (item.Status == true)
                    {
                        tmp.Status = "Đang sử dụng";
                    }
                    else
                    {
                        tmp.Status = "Chưa Sử Dụng";
                    }
                    model.Add(tmp);
                }
            }

            if (BranchId != null && BranchId.Value > 0)
            {
                model = model.Where(x => x.BranchId == BranchId).ToList();
            }

            if (!string.IsNullOrEmpty(group))
            {
                model = model.Where(x => x.Group == group).ToList();

            }

            if (!string.IsNullOrEmpty(info))
            {
                model = model.Where(x => Helpers.Common.ChuyenThanhKhongDau(x.Name).Contains(Helpers.Common.ChuyenThanhKhongDau(info)) || Helpers.Common.ChuyenThanhKhongDau(x.Code).Contains(Helpers.Common.ChuyenThanhKhongDau(info))).ToList();
            }

            return model;
        }

        public ActionResult ExportExcel(string info, string group, int? BranchId, bool ExportExcel = false)
        {
            var data = IndexExport(info, group, BranchId);

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
            model.Content = model.Content.Replace("{Title}", "Danh sách trang thiết bị");
            if (ExportExcel)
            {
                Response.AppendHeader("content-disposition", "attachment;filename=" + "DS_Trangthietbi" + DateTime.Now.ToString("yyyyMMdd") + ".xls");
                Response.Charset = "";
                Response.Cache.SetCacheability(HttpCacheability.NoCache);
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.Write(model.Content);
                Response.End();
            }
            return View(model);
        }

        string buildHtml(List<StaffEquipmentViewModel> data)
        {
            //Tạo table html chi tiết phiếu xuất
            string detailLists = "<table border=\"1\" class=\"invoice-detail\">\r\n";
            detailLists += "<thead>\r\n";
            detailLists += "	<tr>\r\n";
            detailLists += "		<th>STT</th>\r\n";
            detailLists += "		<th>Mã Trang Thiết Bị</th>\r\n";
            detailLists += "		<th>Tên Trang Thiết Bị</th>\r\n";
            detailLists += "		<th>Trạng Thái</th>\r\n";
            detailLists += "		<th>Ngày Kiểm Tra</th>\r\n";
            detailLists += "		<th>Nhóm</th>\r\n";
            detailLists += "		<th>Ngày Tạo</th>\r\n";
            detailLists += "		<th>Ngày Cập Nhật</th>\r\n";
            detailLists += "	</tr>\r\n";
            detailLists += "</thead>\r\n";
            detailLists += "<tbody>\r\n";
            var index = 1;

            foreach (var item in data)
            {
                detailLists += "<tr>\r\n"
                + "<td class=\"text-center orderNo\">" + (index++) + "</td>\r\n"
                + "<td class=\"text-left \">" + item.Code + "</td>\r\n"
                + "<td class=\"text-left \">" + item.Name + "</td>\r\n"
                + "<td class=\"text-left \">" + item.Status + "</td>\r\n"
                + "<td>" + (item.InspectionDate.HasValue ? item.InspectionDate.Value.ToString("dd/MM/yyyy HH:mm") : "") + "</td>"
                + "<td class=\"text-left \">" + Erp.BackOffice.Helpers.Common.GetCategoryByValueCodeOrId("value", item.Group, "EquimentGroup").Name + "</td>\r\n"
                + "<td>" + (item.CreatedDate.HasValue ? item.CreatedDate.Value.ToString("dd/MM/yyyy HH:mm") : "") + "</td>"
                + "<td>" + (item.ModifiedDate.HasValue ? item.ModifiedDate.Value.ToString("dd/MM/yyyy HH:mm") : "") + "</td>"
                + "</tr>\r\n";
            }
            detailLists += "</tbody>\r\n";
            detailLists += "<tfoot>\r\n";
            detailLists += "</tfoot>\r\n</table>\r\n";


            return detailLists;
        }
        #endregion

        #region Create
        public ViewResult Create()
        {
            var model = new StaffEquipmentViewModel();

            return View(model);
        }

        [HttpPost]
        public ActionResult Create(StaffEquipmentViewModel model)
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
                var Equipment = new StaffEquipment();
                AutoMapper.Mapper.Map(model, Equipment);
                Equipment.IsDeleted = false;
                Equipment.CreatedUserId = WebSecurity.CurrentUserId;
                Equipment.ModifiedUserId = WebSecurity.CurrentUserId;
                Equipment.AssignedUserId = WebSecurity.CurrentUserId;
                Equipment.BranchId = intBrandID;
                Equipment.CreatedDate = DateTime.Now;
                Equipment.ModifiedDate = DateTime.Now;
                StaffEquipmentRepository.InsertEquipment(Equipment);

                //ghi log 
                Erp.BackOffice.Controllers.HomeController.WriteLog(Equipment.Id, Equipment.Code, "đã tạo thiết bị", "StaffEquipment/Edit/" + Equipment.Id, Helpers.Common.CurrentUser.BranchId.Value);

                TempData[Globals.SuccessMessageKey] = App_GlobalResources.Wording.InsertSuccess;
                return RedirectToAction("Index");
            }
            return View(model);
        }
        #endregion

        #region Edit
        public ActionResult Edit(int? Id)
        {
            var Equipment = StaffEquipmentRepository.GetStaffEquipmentById(Id.Value);
            if (Equipment != null && Equipment.IsDeleted != true)
            {
                var model = new StaffEquipmentViewModel();
                AutoMapper.Mapper.Map(Equipment, model);

                //if (model.CreatedUserId != Erp.BackOffice.Helpers.Common.CurrentUser.Id && Erp.BackOffice.Helpers.Common.CurrentUser.UserTypeId != 1)
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

        [HttpPost]
        public ActionResult Edit(StaffEquipmentViewModel model)
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
                if (Request["Submit"] == "Save")
                {
                    var Equipment = StaffEquipmentRepository.GetStaffEquipmentById(model.Id);
                    AutoMapper.Mapper.Map(model, Equipment);
                    Equipment.ModifiedUserId = WebSecurity.CurrentUserId;
                    Equipment.ModifiedDate = DateTime.Now;
                    Equipment.BranchId = intBrandID;
                    StaffEquipmentRepository.UpdateEquipment(Equipment);


                    //ghi log 
                    Erp.BackOffice.Controllers.HomeController.WriteLog(Equipment.Id, Equipment.Code, "đã cập nhật thiết bị", "StaffEquipment/Edit/" + Equipment.Id, Helpers.Common.CurrentUser.BranchId.Value);

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

        #region Delete
        [HttpPost]
        public ActionResult Delete()
        {
            try
            {
                string id = Request["Delete"];
                if (id != null)
                {
                    var item = StaffEquipmentRepository.GetStaffEquipmentById(int.Parse(id, CultureInfo.InvariantCulture));
                    if (item != null)
                    {
                        StaffEquipmentRepository.DeleteEquipmentRs(item.Id);
                    }
                }
                else
                {
                    string idDeleteAll = Request["DeleteId-checkbox"];
                    string[] arrDeleteId = idDeleteAll.Split(',');
                    for (int i = 0; i < arrDeleteId.Count(); i++)
                    {
                        var item = StaffEquipmentRepository.GetStaffEquipmentById(int.Parse(arrDeleteId[i], CultureInfo.InvariantCulture));
                        if (item != null)
                        {
                            StaffEquipmentRepository.DeleteEquipmentRs(item.Id);
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