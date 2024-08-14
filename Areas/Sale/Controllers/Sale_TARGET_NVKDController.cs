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
using Erp.BackOffice.Areas.Cms.Models;
using Erp.Domain.Crm.Interfaces;
using Erp.BackOffice.Crm.Models;
using System.Transactions;
using System.Text.RegularExpressions;
using Erp.Domain.Account.Interfaces;
using Erp.BackOffice.Staff.Models;
using Erp.Domain.Staff.Interfaces;
//using Erp.Domain.Account.Repositories;

namespace Erp.BackOffice.Sale.Controllers
{
    [Authorize]
    [InitializeSimpleMembership]
    [Erp.BackOffice.Helpers.NoCacheHelper]
    public class Sale_TARGET_NVKDController : Controller
    {
        private readonly ISale_TARGET_NVKDRepository Sale_TARGET_NVKDRepository;
        private readonly IBranchRepository branchRepository;
        public Sale_TARGET_NVKDController(
            ISale_TARGET_NVKDRepository _Sale_TARGET_NVKD
            , IBranchRepository branch
            )
        {
            Sale_TARGET_NVKDRepository = _Sale_TARGET_NVKD;
            branchRepository = branch;
        }


        #region Index
        public ActionResult Index(int? month, int? year, int? BranchId)
        {
            var q = Sale_TARGET_NVKDRepository.GetAllSale_TARGET_NVKD();
            var branch_list = branchRepository.GetAllBranch().Select(item => new BranchViewModel
            {
                Id = item.Id,
                Name = item.Name,
                Code = item.Code,
                DistrictId = item.DistrictId,
                CityId = item.CityId
            }).ToList();

            var model = q.Select(item => new HoaHongChiNhanhViewModel
            {
                Id = item.Id,
                month = item.month,
                year = item.year,
                BranchId = item.BranchId,
                OldOrlane = item.OldOrlane,
                NewOrlane = item.NewOrlane,
                Annayake = item.Annayake,
                LennorGreyl = item.LennorGreyl,
                Orlane = item.NewOrlane + item.OldOrlane,
                Total = item.NewOrlane + item.OldOrlane + item.Annayake + item.LennorGreyl
            }).ToList();

            foreach(var i in model)
            {
                foreach (var j in branch_list)
                {
                    if (i.BranchId == j.Id)
                    {
                        i.BranchName = j.Name;
                    }
                }

            }
            if(month != null)
            {
                model = model.Where(x => x.month == month).ToList();

            }
            if (year != null)
            {
                model = model.Where(x => x.year == year).ToList();

            }
            if (BranchId != null)
            {
                model = model.Where(x => x.BranchId == BranchId).ToList();

            }

            return View(model);
        }

        #endregion

        #region Create
        public ActionResult Create()
        {
            var branch_list = branchRepository.GetAllBranch().Select(item => new BranchViewModel
            {
                Id = item.Id,
                Name = item.Name,
                Code = item.Code,
                DistrictId = item.DistrictId,
                CityId = item.CityId
            }).ToList();
            var model = new HoaHongChiNhanhViewModel();
            model.month = DateTime.Now.Month;
            model.year = DateTime.Now.Year;
            ViewBag.BranchList = branch_list;
            return View(model);

            
        }

        [HttpPost]
        public ActionResult Create(HoaHongChiNhanhViewModel model)
        {
            if (ModelState.IsValid)
            {
                using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.Snapshot }))
                {
                    var Check = Sale_TARGET_NVKDRepository.GetSale_TARGET_NVKDByMonthYearBranch(model.month, model.year, model.BranchId);
                    if (Check == null)
                    {
                        var LoyaltyPoint = new Sale_TARGET_NVKD();
                        AutoMapper.Mapper.Map(model, LoyaltyPoint);
                        LoyaltyPoint.BranchId = model.BranchId;

                        LoyaltyPoint.IsDeleted = false;

                        Sale_TARGET_NVKDRepository.InsertSale_TARGET_NVKD(LoyaltyPoint);
                        //ghi log 
                        Erp.BackOffice.Controllers.HomeController.WriteLog(LoyaltyPoint.Id, model.month + "/" + model.year, "đã tạo target NVKD", "Sale_TARGET_NVKD/Edit/" + LoyaltyPoint.Id, Helpers.Common.CurrentUser.BranchId.Value);

                        scope.Complete();
                    }
                    else
                    {
                        return RedirectToAction("Index");
                    }
                }
                    TempData[Globals.SuccessMessageKey] = App_GlobalResources.Wording.InsertSuccess;
                //Sale_TARGET_NVKDRepository.InsertSale_TARGET_NVKD(model);
                return RedirectToAction("Index");
            }
            return View(model);
        }
        #endregion


        #region Edit
        public ActionResult Edit(int Id)
        {
            var LoyaltyPoint = Sale_TARGET_NVKDRepository.GetSale_TARGET_NVKDById(Id);
            if (LoyaltyPoint != null && LoyaltyPoint.IsDeleted != true)
            {
                var model = new HoaHongChiNhanhViewModel();
                AutoMapper.Mapper.Map(LoyaltyPoint, model);

               

                return View(model);
            }
            if (Request.UrlReferrer != null)
                return Redirect(Request.UrlReferrer.AbsoluteUri);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult Edit(HoaHongChiNhanhViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (Request["Submit"] == "Save")
                {
                    var LoyaltyPoint = Sale_TARGET_NVKDRepository.GetSale_TARGET_NVKDById(model.Id);
                    AutoMapper.Mapper.Map(model, LoyaltyPoint);
                   
                    Sale_TARGET_NVKDRepository.UpdateSale_TARGET_NVKD(LoyaltyPoint);
                    //ghi log 
                    Erp.BackOffice.Controllers.HomeController.WriteLog(LoyaltyPoint.Id, model.month + "/" + model.year, "đã cập nhật target NVKD", "Sale_TARGET_NVKD/Edit/" + LoyaltyPoint.Id, Helpers.Common.CurrentUser.BranchId.Value);

                    TempData[Globals.SuccessMessageKey] = App_GlobalResources.Wording.UpdateSuccess;
                    return RedirectToAction("Index");
                }


            }
                return View(model);
               
         
        }
        #endregion
        
    }
}
