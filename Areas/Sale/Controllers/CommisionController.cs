using System.Globalization;
using Erp.BackOffice.Sale.Models;
using Erp.BackOffice.Filters;
using Erp.Domain.Entities;
using Erp.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Erp.Utilities;
using WebMatrix.WebData;
using Erp.Domain.Sale.Interfaces;
using Erp.BackOffice.Helpers;
using Erp.Domain.Staff.Interfaces;
using Erp.Domain.Sale.Entities;
using Erp.BackOffice.App_GlobalResources;
using Erp.BackOffice.Areas.Staff.Models;
using Erp.BackOffice.Staff.Models;
using System.Web;

namespace Erp.BackOffice.Sale.Controllers
{
    [Authorize]
    [InitializeSimpleMembership]
    [Erp.BackOffice.Helpers.NoCacheHelper]
    public class CommisionController : Controller
    {
        private readonly ICommisionRepository commisionRepository;
        private readonly IBranchRepository branchRepository;
        private readonly IStaffsRepository staffsRepository;
        private readonly IUserRepository userRepository;
        private readonly IProductOrServiceRepository productRepository;
        private readonly IHOAHONG_NVKDRepository HOAHONG_NVKDRepository;
        private readonly ISale_TARGET_NVKDRepository Sale_TARGET_NVKDRepository;
        private readonly IProductInvoiceRepository productInvoiceRepository;
        private readonly IUserType_kdRepository UserType_kdRepository;
        private readonly ISale_DSHOTRO_CNRepository Sale_DSHOTRO_CNRepository;
        private readonly IBC_DOANHSO_NHANHANGRepository BC_DOANHSO_NHANHANGRepository;

        public CommisionController(
            ICommisionRepository _Commision
            , IBranchRepository _Branch
            , IUserRepository _user
            , IStaffsRepository _staffs
            , IProductOrServiceRepository _Product
             , IHOAHONG_NVKDRepository HOAHONG_NVKD
            , ISale_TARGET_NVKDRepository Sale_TARGET_NVKD
            , IProductInvoiceRepository ProductInvoice
            , IUserType_kdRepository _user_kd
            , ISale_DSHOTRO_CNRepository _dshotro
             , IBC_DOANHSO_NHANHANGRepository BC_DOANHSO_NHANHANG
            )
        {
            commisionRepository = _Commision;
            branchRepository = _Branch;
            userRepository = _user;
            staffsRepository = _staffs;
            productRepository = _Product;
            HOAHONG_NVKDRepository = HOAHONG_NVKD;
            Sale_TARGET_NVKDRepository = Sale_TARGET_NVKD;
            productInvoiceRepository = ProductInvoice;
            UserType_kdRepository = _user_kd;
            Sale_DSHOTRO_CNRepository = _dshotro;

            BC_DOANHSO_NHANHANGRepository = BC_DOANHSO_NHANHANG;
        }

        #region Index

        public ViewResult Index(string txtSearch)
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
           
            IQueryable<BranchViewModel> q = branchRepository.GetAllvwBranch()
                .Select(item => new BranchViewModel
                {
                    Id = item.Id,
                    Name = item.Name
                }).OrderBy(m => m.Name);

            ViewBag.SuccessMessage = TempData["SuccessMessage"];
            ViewBag.FailedMessage = TempData["FailedMessage"];
            ViewBag.AlertMessage = TempData["AlertMessage"];
            return View(q);
        }
        #endregion

        #region Edit
        public ActionResult Edit(int? Id)
        {
            var branch = branchRepository.GetBranchById(Id.Value);
            if (branch != null && branch.IsDeleted != true)
            {
                var model = new CommisionEditViewModel();
                model.BranchId = branch.Id;
                model.BranchName = branch.Name;

                model.ListStaff = staffsRepository.GetvwAllStaffs()
                  //  .Where(item => item.Sale_BranchId == branch.Id)
                    .Select(item => new StaffGeneralInfoViewModel
                    {
                        Id = item.Id,
                        Name = item.Name,
                        Code = item.Code,
                        //BranchId = item.Sale_BranchId
                    })
                    .OrderBy(item => item.Name)
                    .ToList();

                return View(model);
            }

            if (Request.UrlReferrer != null)
                return Redirect(Request.UrlReferrer.AbsoluteUri);
            return RedirectToAction("Index");
        }
        #endregion

        #region Detail
        public ViewResult Detail(int BranchId, int StaffId, string txtProductCode,string txtCode, string txtSearch)
        {
            CommisionDetailViewModel model = new CommisionDetailViewModel();
            var listCommision = commisionRepository.GetAllCommision().Where(item => item.BranchId == BranchId && item.StaffId == StaffId).ToList();
            model.StaffName = staffsRepository.GetAllStaffs().Where(item => item.Id == StaffId).Select(item => item.Name).FirstOrDefault();
            model.StaffId = StaffId;
            model.DetailList = new List<CommisionViewModel>();

            var productList = productRepository.GetAllvwProductAndService().Select(item => new
            {
                item.Id,
                item.Code,
                item.Name,
                item.PriceOutbound
            }).ToList();
            if (!string.IsNullOrEmpty(txtCode))
            {
                productList = productList.Where(x => x.Code.Contains(txtCode)).ToList();
            }
            if (!string.IsNullOrEmpty(txtSearch))
            {
                productList = productList.Where(x => x.Name.Contains(txtSearch)).ToList();
            }
            foreach (var item in productList)
            {
                var commisionViewModel = new CommisionViewModel();
                commisionViewModel.ProductId = item.Id;
                commisionViewModel.Name = item.Code + " - " + item.Name;
                commisionViewModel.Price = item.PriceOutbound.Value;
                commisionViewModel.IsMoney = false;
                var commision = listCommision.Where(i => i.ProductId == item.Id).FirstOrDefault();
                if (commision != null)
                {
                    commisionViewModel.Id = commision.Id;
                    commisionViewModel.CommissionValue = commision.CommissionValue;
                    commisionViewModel.IsMoney = commision.IsMoney == null ? false : commision.IsMoney;
                }

                model.DetailList.Add(commisionViewModel);
            }

            ViewBag.SuccessMessage = TempData["SuccessMessage"];
            ViewBag.FailedMessage = TempData["FailedMessage"];
            ViewBag.AlertMessage = TempData["AlertMessage"];

            return View(model);
        }

        [HttpPost]
        public ActionResult Detail(CommisionDetailViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (Request["Submit"] == "Save")
                {
                    foreach (var item in model.DetailList)
                    {
                        if (item.CommissionValue < 0)
                            item.CommissionValue = 0;
                        else if ((item.IsMoney == null || item.IsMoney == false) && item.CommissionValue > 100)
                            item.CommissionValue = 100;
                        else if (item.CommissionValue > item.Price)
                            item.CommissionValue = item.Price;

                        if (item.Id > 0)
                        {
                            var commision = commisionRepository.GetCommisionById(item.Id);
                            commision.ModifiedUserId = WebSecurity.CurrentUserId;
                            commision.ModifiedDate = DateTime.Now;
                            commision.CommissionValue = item.CommissionValue;
                            commision.IsMoney = item.IsMoney;
                            commisionRepository.UpdateCommision(commision);
                        }
                        else
                        {
                            if (item.CommissionValue > 0)
                            {
                                var commision = new Commision();
                                commision.IsDeleted = false;
                                commision.CreatedUserId = WebSecurity.CurrentUserId;
                                commision.ModifiedUserId = WebSecurity.CurrentUserId;
                                commision.CreatedDate = DateTime.Now;
                                commision.ModifiedDate = DateTime.Now;
                                commision.BranchId = model.BranchId;
                                commision.StaffId = model.StaffId;
                                commision.ProductId = item.ProductId;
                                commision.CommissionValue = item.CommissionValue;
                                commision.IsMoney = item.IsMoney;
                                commisionRepository.InsertCommision(commision);
                            }
                        }
                    }

                    TempData[Globals.SuccessMessageKey] = App_GlobalResources.Wording.UpdateSuccess;
                    return RedirectToAction("Detail", new { BranchId = model.BranchId, StaffId = model.StaffId });
                }

                return View(model);
            }

            return View(model);
        }
        #endregion

        #region HoaHong_NVKD
        public ActionResult BranchCommision(string Month, string Year, string BranchId)
        {
            Month = Month == null ? DateTime.Now.Month.ToString() : Month;
            Year = Year == null ? DateTime.Now.Year.ToString() : Year;
            var month = Convert.ToInt32(Month);
            var year = Convert.ToInt32(Year);
            int? branchId = 0;
            if (BranchId != null && BranchId != "") {
                branchId = Convert.ToInt32(BranchId);
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
            branchId = intBrandID;
            var target = Sale_TARGET_NVKDRepository.GetAllSale_TARGET_NVKD().Where(x => x.month == month  && x.year == year);


            
            var listProduct = productInvoiceRepository.GetAllvwProductInvoiceFull_NVKD().Where(x => x.Month == month && x.Year == year);
            //var ListTargetName = new List<string>();
            //ListTargetName.Add("TỔNG");
            //ListTargetName.Add("TỔNG ORLANE");
            //ListTargetName.Add("DS MỚI ORLANE");
            //ListTargetName.Add("DS CŨ ORLANE");
            //ListTargetName.Add("DS ANNAYAKE");
            //ListTargetName.Add("DS LEONOR GREYL");

            var branch_list = branchRepository.GetAllBranch().Select(item => new BranchViewModel
            {
                Id = item.Id,
                Name = item.Name,
                Code = item.Code,
                DistrictId = item.DistrictId,
                CityId = item.CityId
            }).ToList();

            if (branchId > 0)
            {
                target = target.Where(x => x.BranchId == branchId);
                branch_list = branch_list.Where(x => x.Id == branchId).ToList();
                listProduct = listProduct.Where(x => x.BranchId == branchId);
            }
            //
            
            var total = listProduct.Select(item => new ProductInvoiceViewModel
            {
                Id = item.Id,
               
                Code = item.Code,
                CustomerCode = item.CustomerCode,
                CustomerName = item.CustomerName,
                TotalAmount = item.TotalAmount,
               
                TaxFee = item.TaxFee,
                CodeInvoiceRed = item.CodeInvoiceRed,
                Status = item.Status,
                IsArchive = item.IsArchive,
                Note = item.Note,
                CancelReason = item.CancelReason,
                BranchId = item.BranchId,
                CustomerId = item.CustomerId,
                BranchName = item.BranchName,
                ManagerStaffId = item.ManagerStaffId,
                ManagerStaffName = item.ManagerStaffName,
                ManagerUserName = item.ManagerUserName,
                CountForBrand = item.CountForBrand,
                TotalDebit = item.TotalDebit,
                TotalCredit = item.TotalCredit,
                TongConNo = (item.TotalDebit - item.TotalCredit),
                UserTypeName = item.UserTypeName,
                Month = item.Month,
                Year = item.Year,
                GDDauTienThanhToanHet = item.GDDauTienThanhToanHet,
                GDNgayDauTienThanhToanHet = item.GDNgayDauTienThanhToanHet,
                SPHangHoa = item.SPHangHoa,
                SPDichvu = item.SPDichvu,
                Hangduoctang = item.Hangduoctang,
               
                VoucherDate = item.VoucherDate,
                TyleHuong = item.TyleHuong
            }).ToList();

            var orlane = total.Where(x => x.CountForBrand == "ORLANE PARIS" || x.CountForBrand == "DICHVU" || x.CountForBrand == "CONGNGHECAO").ToList();

            var oldorlane = orlane.Where(x => x.GDNgayDauTienThanhToanHet == "N").ToList();
            var neworlane = orlane.Where(x => x.GDNgayDauTienThanhToanHet == "Y").ToList();

            var annayake = total.Where(x => x.CountForBrand == "ANNAYAKE").ToList();

            var LennorGreyl = total.Where(x => x.CountForBrand == "LEONOR GREYL").ToList();

            // //lay DS ho tro
            //var Dshotro = Sale_DSHOTRO_CNRepository.GetAllSale_DSHOTRO_CN();
            //
            //Kiểm tra user
            var user = userRepository.GetUserById(Helpers.Common.CurrentUser.Id);

            var model = target.Select(item => new HoaHongChiNhanhViewModel
            {
                month = item.month,
                year = item.year,
                BranchId = item.BranchId,
                OldOrlane = item.OldOrlane,
                NewOrlane = item.NewOrlane,
                Annayake = item.Annayake,
                LennorGreyl = item.LennorGreyl,
                Orlane = item.NewOrlane + item.OldOrlane,
                Total = item.NewOrlane + item.OldOrlane + item.Annayake + item.LennorGreyl,

                
            }).ToList();

            if (model.Count() == 0)
            {
                return RedirectToAction("Index", "Sale_TARGET_NVKD", new { FailedMessage = "Chi nhánh chưa có target!" });
            }

            ////Kiểm tra phân quyền user
            if (user.UserTypeId != 1 && user.UserTypeId != 17)
            {
                model = model.Where(x => x.BranchId == user.BranchId).ToList();

            }

            foreach (var i in model)
            {
                var Dshotro = Sale_DSHOTRO_CNRepository.GetSale_DSHOTRO_CNByMonthYearBranch(i.month, i.year,i.BranchId);
                if (Dshotro != null)
                {
                    i.NewOrlane_HT = Dshotro.NewOrlane;
                    i.OldOrlane_HT = Dshotro.OldOrlane;
                    i.Annayake_HT = Dshotro.Annayake;
                    i.LennorGreyl_HT = Dshotro.LennorGreyl;
                    i.Total_HT = Dshotro.NewOrlane + Dshotro.OldOrlane + Dshotro.Annayake + Dshotro.LennorGreyl;
                }
                else
                {
                    i.NewOrlane_HT = 0;
                    i.OldOrlane_HT = 0;
                    i.Annayake_HT = 0;
                    i.LennorGreyl_HT = 0;
                    i.Total_HT = 0;
                }
             }

            ViewBag.branchList = branch_list;
            ViewBag.TotalList = total;
            ViewBag.Orlane = orlane;
            ViewBag.Annayake = annayake;
            ViewBag.LennorGreyl = LennorGreyl;
            ViewBag.OldOrlane = oldorlane;
            ViewBag.NewOrlane = neworlane;
            //ViewBag.ListTargetName = ListTargetName;

            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public JsonResult BranchCommision(List<DSHOTRO_JSON> model)
        {
            foreach(var i in model)
            {
                var ds = new Sale_DSHOTRO_CN();
                var ds_ex = Sale_DSHOTRO_CNRepository.GetSale_DSHOTRO_CNByMonthYearBranch(i.month, i.year, i.branch);
                if(ds_ex != null)
                {
                    ds_ex.Annayake = i.anna;
                    ds_ex.NewOrlane = i.New;
                    ds_ex.OldOrlane = i.Old;
                    ds_ex.LennorGreyl = i.lennor;
                    Sale_DSHOTRO_CNRepository.UpdateSale_DSHOTRO_CN(ds_ex);
                }
                else
                {
                    ds.BranchId = i.branch;
                    ds.Month = i.month;
                    ds.Year = i.year;
                    ds.Annayake = i.anna;
                    ds.NewOrlane = i.New;
                    ds.OldOrlane = i.Old;
                    ds.LennorGreyl = i.lennor;
                    Sale_DSHOTRO_CNRepository.InsertSale_DSHOTRO_CN(ds);
                }
            }
            return Json(1);
        }


        public ActionResult StaffCommision(int month, int year, int branchId, int? userTypeId)
        {
            //Kiểm tra user
            var user = userRepository.GetUserById(Helpers.Common.CurrentUser.Id);
           
            //
            var listHoaHong = HOAHONG_NVKDRepository.GetAllHOAHONG_NVKD().ToList();
            var Target = Sale_TARGET_NVKDRepository.GetSale_TARGET_NVKDByMonthYearBranch(month, year, branchId);
           
            if(Target == null)
            {
                return RedirectToAction("StaffCommision", new { month = DateTime.Now.Month , year = DateTime.Now.Year, branchId = Helpers.Common.CurrentUser.BranchId });
            }

            var listProduct = GETDonHangTheoNhanHang2(month,year, branchId);
                //productInvoiceRepository.GetAllvwProductInvoiceFull_NVKD().Where(x => x.Month == month && x.Year == year & x.BranchId == branchId);
            //var q = userRepository.GetAllUsers().Where(x => x.Status == UserStatus.Active && x.BranchId == branchId).ToList();
            //var countuser = q.Count();
            ////User
            var q = UserType_kdRepository.GetUserTypes().Where(x => x.BranchId == branchId);
            ///
            var orlane = listProduct.Where(x => x.CountForBrand == "ORLANE PARIS" || x.CountForBrand == "DICHVU" || x.CountForBrand == "CONGNGHECAO").ToList();

            var oldorlane = orlane.Where(x => x.GDNgayDauTienThanhToanHet == "N").ToList();
            var neworlane = orlane.Where(x => x.GDNgayDauTienThanhToanHet == "Y").ToList();

            var annayake = listProduct.Where(x => x.CountForBrand == "ANNAYAKE").ToList();

            var LennorGreyl = listProduct.Where(x => x.CountForBrand == "LEONOR GREYL").ToList();

            //Target 1 NVKD
            var soNhom = 8;
            var oldOrlaneTarget = Math.Round((decimal)(Target.OldOrlane / soNhom));
            var newOrlaneTarget = Math.Round((decimal)(Target.NewOrlane / soNhom));
            var annaTarget = Math.Round((decimal)(Target.Annayake / soNhom));
            var LennorTarget = Math.Round((decimal)(Target.LennorGreyl / soNhom));
            var OrlaneTarget = oldOrlaneTarget + newOrlaneTarget;
            //
            var model = q.Select(item => new HoaHongNVViewModel()
            {
                userName = item.Name,
                userid = item.Id,
                branchid = item.BranchId,
                TotalTarget = OrlaneTarget + LennorTarget + annaTarget,
                orlaneTarget = OrlaneTarget,
                oldorlaneTarget = oldOrlaneTarget,
                neworlaneTarget = newOrlaneTarget,
                annayakeTarget = annaTarget,
                LennorGreylTarget = LennorTarget
            }).ToList();
            //
            if(userTypeId > 0)
            {
                model = model.Where(x=> x.userid == userTypeId).ToList();
            }
            if (user.UserTypeId != 1 && user.UserTypeId != 17)
            {
                model = model.Where(x => x.userid == user.UserType_kd_id).ToList();

            }
            foreach (var i in model)
            {
               //var orlanemoney = orlane.Where(x => x.ManagerStaffId == i.userid).Sum(x => x.TotalAmount);
               var Oldorlanemoney = oldorlane.Where(x => x.UserTypeId == i.userid).Sum(x => x.TotalAmount);
               var Neworlanemoney = neworlane.Where(x => x.UserTypeId == i.userid).Sum(x => x.TotalAmount);
               var annayakemoney = annayake.Where(x => x.UserTypeId == i.userid).Sum(x => x.TotalAmount);
               var LennorGreylmoney = LennorGreyl.Where(x => x.UserTypeId == i.userid).Sum(x => x.TotalAmount);
               var Totalmoney = Oldorlanemoney + Neworlanemoney + annayakemoney + LennorGreylmoney;
                //Doanh thu thực 1 NV
                i.orlane = Oldorlanemoney + Neworlanemoney;
                i.oldorlane = Oldorlanemoney;
                i.neworlane = Neworlanemoney;
                i.annayake = annayakemoney;
                i.LennorGreyl = LennorGreylmoney;
                i.Total = Totalmoney;

                //tính tỷ lệ đạt
                i.TyleTotal = Math.Round((decimal)(i.Total / i.TotalTarget * 100));
                i.TyleOrlane = Math.Round((decimal)(i.orlane / i.orlaneTarget * 100));
                i.TyleoldOrlane = Math.Round((decimal)(i.oldorlane / i.oldorlaneTarget * 100));
                i.TylenewOrlane = Math.Round((decimal)(i.neworlane / i.neworlaneTarget * 100));
                i.TyleAnna = Math.Round((decimal)(i.annayake / i.annayakeTarget * 100));
                i.TyleLenor = Math.Round((decimal)(i.LennorGreyl / i.LennorGreylTarget * 100));

                //Tính mức hoa hồng
                var tyledatYC = 75;
                
                foreach (var j in listHoaHong)
                {
                    ///Trên 75%
                    if (i.TyleoldOrlane > tyledatYC && i.TylenewOrlane > tyledatYC && i.TyleAnna > tyledatYC && i.TyleLenor > tyledatYC && i.TyleOrlane > tyledatYC)
                    {
                        ///Tất cả trên 100%
                        if (i.TyleoldOrlane > 100 && i.TylenewOrlane > 100 && i.TyleAnna > 100 && i.TyleLenor > 100 && i.TyleOrlane > 100)
                        {
                            
                            if(i.TylenewOrlane >= j.MIN_TARGET && i.TylenewOrlane <= j.MAX_TARGET)
                            {
                                i.HoahongOldOrlane = j.TYLE_HOAHONG;

                            }
                            if (i.TyleoldOrlane >= j.MIN_TARGET && i.TyleoldOrlane <= j.MAX_TARGET)
                            {
                                i.HoahongNewOrlane = j.TYLE_HOAHONG;

                            }
                            if (i.TyleAnna >= j.MIN_TARGET && i.TyleAnna <= j.MAX_TARGET)
                            {
                                i.HoaHongAnna = j.TYLE_HOAHONG;
                            }
                            if (i.TyleLenor >= j.MIN_TARGET && i.TyleLenor <= j.MAX_TARGET)
                            {
                                i.HoahongLenor = j.TYLE_HOAHONG;
                            }
                        }
                        // tính độc lập từng nhãn
                        else
                        {
                            if (i.TylenewOrlane >= j.MIN_TARGET && i.TylenewOrlane <= j.MAX_TARGET && i.TylenewOrlane < 115)
                            {
                                i.HoahongNewOrlane = j.TYLE_HOAHONG;
                            }
                            if(i.TylenewOrlane >= 115)
                            {
                                i.HoahongNewOrlane = 3;
                            }
                            if (i.TyleoldOrlane >= j.MIN_TARGET && i.TyleoldOrlane <= j.MAX_TARGET && i.TyleoldOrlane < 115)
                            {
                                i.HoahongOldOrlane = j.TYLE_HOAHONG;
                            }
                            if(i.TyleoldOrlane >= 115)
                            {
                                i.HoahongOldOrlane = 3;
                            }
                            if (i.TyleAnna >= j.MIN_TARGET && i.TyleAnna <= j.MAX_TARGET && i.TyleAnna < 115)
                            {
                                i.HoaHongAnna = j.TYLE_HOAHONG;
                            }
                            if(i.TyleAnna >= 115)
                            {
                                i.HoaHongAnna = 3;
                            }
                            if (i.TyleLenor >= j.MIN_TARGET && i.TyleLenor <= j.MAX_TARGET && i.TyleLenor < 115)
                            {
                                i.HoahongLenor = j.TYLE_HOAHONG;
                            }
                            if (i.TyleLenor >= 115)
                            {
                                i.HoahongLenor = 3;
                            }
                        }
                    }
                    ///Dưới 75%
                    else
                    {
                        var min1 = Math.Min((decimal)i.TylenewOrlane,(decimal)i.TyleoldOrlane);
                        var min2 = Math.Min((decimal)i.TyleAnna, (decimal)i.TyleLenor);
                        var MIN = Math.Min(min1, min2);
                        if(MIN >= j.MIN_TARGET && MIN <= j.MAX_TARGET)
                        {
                            i.HoahongNewOrlane = j.TYLE_HOAHONG;
                            i.HoahongOldOrlane = j.TYLE_HOAHONG;
                            i.HoaHongAnna = j.TYLE_HOAHONG;
                            i.HoahongLenor = j.TYLE_HOAHONG;
                        }
                        if(MIN == 0)
                        {
                            i.HoahongNewOrlane = 0;
                            i.HoahongOldOrlane = 0;
                            i.HoaHongAnna = 0;
                            i.HoahongLenor = 0;
                        }
                      
                    }
                }
                //Tính tiền lương
                i.moneyNewOrlane = Math.Round((decimal)(i.neworlane * i.HoahongNewOrlane / 100));
                i.moneyOldOrlane = Math.Round((decimal)(i.oldorlane * i.HoahongOldOrlane / 100));
                i.moneyAnna = Math.Round((decimal)(i.annayake * i.HoaHongAnna / 100));
                i.moneyLenor = Math.Round((decimal)(i.LennorGreyl * i.HoahongLenor / 100));
                i.TongLuong = i.moneyNewOrlane + i.moneyOldOrlane + i.moneyAnna + i.moneyLenor;
            }
            //var ListTargetName = new List<string>();
            //ListTargetName.Add("TỔNG");
            //ListTargetName.Add("TỔNG ORLANE");
            //ListTargetName.Add("DS MỚI ORLANE");
            //ListTargetName.Add("DS CŨ ORLANE");
            //ListTargetName.Add("DS ANNAYAKE");

            //ListTargetName.Add("DS LEONOR GREYL");
            //ViewBag.ListTargetName = ListTargetName;

            ViewBag.TotalTarget = OrlaneTarget + LennorTarget + annaTarget;
            ViewBag.OrlaneTarget = OrlaneTarget;
            ViewBag.oldOrlaneTarget = oldOrlaneTarget;
            ViewBag.newOrlaneTarget = newOrlaneTarget;
            ViewBag.LennorTarget = LennorTarget;
            ViewBag.annaTarget = annaTarget;

            return View(model);
        }

        #endregion
        public List<ProductInvoiceViewModel> GETDonHangTheoNhanHang2(int month, int year, int branchId)
        {
            var q = productInvoiceRepository.GetAllvwProductInvoiceFull_NVKD().Where(x => x.Month == month && x.Year == year & x.BranchId == branchId); ;
            var model = q.Select(item => new ProductInvoiceViewModel
            {
                Id = item.Id,
                IsDeleted = item.IsDeleted,
                CreatedUserId = item.CreatedUserId,
                CreatedDate = item.CreatedDate,
                ModifiedUserId = item.ModifiedUserId,
                ModifiedDate = item.ModifiedDate,
                Code = item.Code,
                CustomerCode = item.CustomerCode,
                CustomerName = item.CustomerName,
                TotalAmount = item.TotalAmount,
                //FixedDiscount = item.FixedDiscount,
                TaxFee = item.TaxFee,
                CodeInvoiceRed = item.CodeInvoiceRed,
                Status = item.Status,
                IsArchive = item.IsArchive,
                Note = item.Note,
                CancelReason = item.CancelReason,
                BranchId = item.BranchId,
                CustomerId = item.CustomerId,
                BranchName = item.BranchName,
                ManagerStaffId = item.ManagerStaffId,
                ManagerStaffName = item.ManagerStaffName,
                ManagerUserName = item.ManagerUserName,
                CountForBrand = item.CountForBrand,
                TotalDebit = item.TotalDebit,
                TotalCredit = item.TotalCredit,
                TongConNo = (item.TotalDebit - item.TotalCredit),
                UserTypeName = item.UserTypeName,
                UserTypeId = item.UserTypeId,
                Month = item.Month,
                Year = item.Year,
                GDDauTienThanhToanHet = item.GDDauTienThanhToanHet,
                GDNgayDauTienThanhToanHet = item.GDNgayDauTienThanhToanHet,
                SPHangHoa = item.SPHangHoa,
                SPDichvu = item.SPDichvu,
                Hangduoctang = item.Hangduoctang,
                Discount_VIP = item.Discount_VIP,
                Discount_KM = item.Discount_KM,
                Discount_DB = item.Discount_DB,
                VoucherDate = item.VoucherDate,
                TyleHuong = item.TyleHuong
            }).ToList();
            
            var modelKM50 = model.Where(x => (Helpers.Common.NVL_NUM_DECIMAL_NEW(x.Discount_DB) + Helpers.Common.NVL_NUM_DECIMAL_NEW(x.Discount_KM)) == 50).ToList();
            foreach (var item in modelKM50)
            {
                //if (item.CustomerCode == "37023")
                //{
                //    int a = 1;
                //}
                if (item.GDNgayDauTienThanhToanHet == "Y")
                {
                    //b2. tim tat cac cac don hang cua khach hang nay mua sau ngay tren va sap xep theo thu tu giam dan
                    var modelKM50_KH = model.Where(x => x.CustomerId == item.CustomerId && x.CreatedDate >= item.CreatedDate && x.Id != item.Id).OrderBy(c => c.CreatedDate).ToList();
                    if ((modelKM50_KH != null) && (modelKM50_KH.Count() > 0))
                    {
                        var modelKM50_KH_UPDATE = modelKM50_KH.Where(x => x.VoucherDate.ToString().Substring(0, 10) == modelKM50_KH[0].VoucherDate.ToString().Substring(0, 10)).OrderBy(c => c.CreatedDate).ToList();

                        if (modelKM50_KH_UPDATE != null && modelKM50_KH_UPDATE.Count > 0)
                        {
                            foreach (var item1 in modelKM50_KH_UPDATE)
                            {
                                var modelKM50_SETY = model.Where(x => x.Id == item1.Id).FirstOrDefault();
                                modelKM50_SETY.GDNgayDauTienThanhToanHet = "Y";
                                //các đơn chuyển từ đơn mới này cũng là mới luôn
                                var check = productInvoiceRepository.GetMMByOldId(modelKM50_SETY.Id);
                                if (check != null)
                                {
                                    var modelNext = model.SingleOrDefault(x => x.Id == check.ToProductInvoiceId);

                                    if (modelNext != null)
                                    {
                                        modelNext.GDNgayDauTienThanhToanHet = "Y";
                                    }
                                }
                                //
                            }
                        }



                    }

                }
            }

            foreach (var item in model)
            {
                if (item.TyleHuong == null)
                {
                    item.TyleHuong = 100;
                }

                //Tim kiem don da chot
                var itemdaco = BC_DOANHSO_NHANHANGRepository.GetBC_DOANHSO_NHANHANGByMaDH(item.Code, branchId);

                if (itemdaco != null)
                {
                    item.Status = "dachot";
                    var Idx = productInvoiceRepository.GetvwProductInvoiceByCode(branchId, item.Code);
                    if (Idx != null)
                    {
                        item.Id = Idx.Id;
                        item.Note = Idx.Note;
                    }
                }

                //Kiểm tra đơn hàng quá 3 tháng hủy chuyển - nếu là mới thì đơn dc chuyển sẽ là cũ
                //var monthSl = DateTime.Now.Year * 12 + DateTime.Now.Month - (item.Year * 12 + item.Month);

                ////Kiem tra don hang co huy chuyen hay khong 
                //var checkYN = productInvoiceRepository.GetMMByOldId(item.Id);
                //if (checkYN != null)
                //{
                //    var modelNext = model.SingleOrDefault(x => x.Id == checkYN.ToProductInvoiceId);
                //    var productinvoice = productInvoiceRepository.GetProductInvoiceById(item.Id);
                //    if (modelNext != null && item.GDNgayDauTienThanhToanHet == "Y" && checkYN.TotalAmount == productinvoice.TotalAmount && monthSl <= 3)
                //    {
                //        modelNext.GDNgayDauTienThanhToanHet = "Y";
                //    }
                //    else if (modelNext != null && item.GDNgayDauTienThanhToanHet == "Y" && checkYN.TotalAmount != productinvoice.TotalAmount)
                //    {
                //        modelNext.GDNgayDauTienThanhToanHet = "N";
                //    }
                //    if (modelNext != null && item.GDNgayDauTienThanhToanHet == "Y" && monthSl > 3)
                //    {
                //        modelNext.GDNgayDauTienThanhToanHet = "N";
                //    }
                //}
                //kiểm tra đơn đc chuyển
                var checkNY = productInvoiceRepository.GetMMByNewId(item.Id);
                if (checkNY != null)
                {
                    //don cu:
                    var modelNext = productInvoiceRepository.GetProductInvoiceById(checkNY.FromProductInvoiceId); //model.SingleOrDefault(x => x.Id == checkNY.FromProductInvoiceId);
                    var date1 = modelNext.CreatedDate;
                    date1 = date1.Value.AddMonths(+3);
                    var productinvoice = productInvoiceRepository.GetProductInvoiceById(item.Id);

                    if (modelNext != null && checkNY.TotalAmount == productinvoice.TotalAmount && item.CreatedDate < date1)
                    {
                        item.GDNgayDauTienThanhToanHet = "Y";
                    }
                    else if (modelNext != null && checkNY.TotalAmount != productinvoice.TotalAmount)
                    {
                        item.GDNgayDauTienThanhToanHet = "N";
                    }
                    if (modelNext != null && item.GDNgayDauTienThanhToanHet == "Y" && item.CreatedDate > date1)
                    {
                        item.GDNgayDauTienThanhToanHet = "N";
                    }
                }

                //Tim kiem don da chot
                //var itemdaco = BC_DOANHSO_NHANHANGRepository.GetBC_DOANHSO_NHANHANGByMaDH(item.Code, BranchId);

                if (itemdaco != null)
                {
                    //item.Status = "dachot";
                    //var Idx = productInvoiceRepository.GetvwProductInvoiceByCode(BranchId.Value, item.Code);
                    //item.Id = Idx.Id;
                    //item.Note = Idx.Note;

                    var checkDonchot = productInvoiceRepository.GetMMByOldId(item.Id);
                    if (checkDonchot != null)
                    {
                        var modelNext_1 = model.SingleOrDefault(x => x.Id == checkDonchot.ToProductInvoiceId);
                        var productinvoice_1 = productInvoiceRepository.GetProductInvoiceById(item.Id);
                        if (modelNext_1 != null && item.GDNgayDauTienThanhToanHet == "Y" && (item.CountForBrand == "ORLANE PARIS" || item.CountForBrand == "DICHVU" ||
                            item.CountForBrand == "CONGNGHECAO"))
                        {
                            if (modelNext_1.CountForBrand == "ORLANE PARIS" || modelNext_1.CountForBrand == "DICHVU" || modelNext_1.CountForBrand == "CONGNGHECAO")
                            {
                                modelNext_1.GDNgayDauTienThanhToanHet = "Y";
                            }
                        }

                        if (modelNext_1 != null && item.GDNgayDauTienThanhToanHet == "Y" && (item.CountForBrand == "ANNAYAKE" || item.CountForBrand == "LEONOR GREYL"))
                        {
                            if (modelNext_1.CountForBrand == "ORLANE PARIS" || modelNext_1.CountForBrand == "DICHVU" || modelNext_1.CountForBrand == "CONGNGHECAO")
                            {
                                modelNext_1.GDNgayDauTienThanhToanHet = "N";
                            }
                        }

                    }
                }

            }
            return model;
        }
        
    }
}
