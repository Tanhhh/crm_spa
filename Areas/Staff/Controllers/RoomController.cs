using System.Globalization;
using Erp.BackOffice.Staff.Models;
using Erp.BackOffice.Filters;
using Erp.Domain.Entities;
using Erp.Domain.Interfaces;
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
using System.Web;

namespace Erp.BackOffice.Staff.Controllers
{
    [Authorize]
    [InitializeSimpleMembership]
    [Erp.BackOffice.Helpers.NoCacheHelper]
    public class RoomController : Controller
    {
        private readonly IRoomRepository RoomRepository;
        private readonly IUserRepository userRepository;
        private readonly IBedRepository BedRepository;

        public RoomController(
            IRoomRepository _Room
            , IUserRepository _user
            , IBedRepository _Bed
            )
        {
            RoomRepository = _Room;
            userRepository = _user;
            BedRepository = _Bed;

        }


        #region Index

        public ViewResult Index(string txtRoomName)
        {
            IEnumerable<RoomViewModel> q = RoomRepository.GetAllRoom()
                .AsEnumerable()
                .Select(item => new RoomViewModel
                {
                    Id = item.Id,
                    CreatedUserId = item.CreatedUserId,
                    //CreatedUserName = item.CreatedUserName,
                    CreatedDate = item.CreatedDate,
                    ModifiedUserId = item.ModifiedUserId,
                    //ModifiedUserName = item.ModifiedUserName,
                    ModifiedDate = item.ModifiedDate,
                    Name = item.Name,
                    Floor = item.Floor,
                    BranchId = item.BranchId
                }).OrderByDescending(m => m.ModifiedDate).ToList();
            if (string.IsNullOrEmpty(txtRoomName) == false)
            {
                //string a = Erp.BackOffice.Helpers.Common.ChuyenThanhKhongDau("phòng 105");
                txtRoomName = txtRoomName == "" ? "~" : Erp.BackOffice.Helpers.Common.ChuyenThanhKhongDau(txtRoomName);
                q = q.Where(x => Helpers.Common.ChuyenThanhKhongDau(x.Name).Contains(txtRoomName)).ToList();
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

            //get  CurrentUser.branchId

            if ((Helpers.Common.CurrentUser.BranchId != null) && (Helpers.Common.CurrentUser.BranchId > 0))
            {
                strBrandID = Helpers.Common.CurrentUser.BranchId.ToString();
            }

            int? intBrandID = int.Parse(strBrandID);

            if(intBrandID > 0)
            {
                q = q.Where(x => x.BranchId == intBrandID).ToList();
            }
            ViewBag.SuccessMessage = TempData["SuccessMessage"];
            ViewBag.FailedMessage = TempData["FailedMessage"];
            ViewBag.AlertMessage = TempData["AlertMessage"];
            return View(q);
        }
        #endregion

        #region Create
        public ViewResult Create()
        {
            var model = new RoomViewModel();

            return View(model);
        }

        [HttpPost]
        public ActionResult Create(RoomViewModel model)
        {
            if (ModelState.IsValid)
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

                var Room = new Room();
                AutoMapper.Mapper.Map(model, Room);
                Room.IsDeleted = false;
                Room.CreatedUserId = WebSecurity.CurrentUserId;
                Room.ModifiedUserId = WebSecurity.CurrentUserId;
                Room.AssignedUserId = WebSecurity.CurrentUserId;
                Room.CreatedDate = DateTime.Now;
                Room.ModifiedDate = DateTime.Now;
                if(Room.BranchId == null)
                {
                    Room.BranchId = intBrandID;
                }
                RoomRepository.InsertRoom(Room);

                TempData[Globals.SuccessMessageKey] = App_GlobalResources.Wording.InsertSuccess;
                return RedirectToAction("Index");
            }
            return View(model);
        }
        #endregion

        #region Edit
        public ActionResult Edit(int? Id)
        {

            var Room = RoomRepository.GetRoomById(Id.Value);
            if (Room != null && Room.IsDeleted != true)
            {

                var model = new RoomViewModel();
                AutoMapper.Mapper.Map(Room, model);

                if (model.CreatedUserId != Helpers.Common.CurrentUser.Id && Helpers.Common.CurrentUser.UserTypeId != 1)
                {
                    TempData["FailedMessage"] = "NotOwner";
                    return RedirectToAction("Index");
                }
                if (Id.HasValue)
                {
                    ViewBag.IdRoom = Id;
                }
                else
                {
                    ViewBag.IdRoom = 0;
                }

                return View(model);
            }
            if (Request.UrlReferrer != null)
            {
                return Redirect(Request.UrlReferrer.AbsoluteUri);
            }
            else
            {
                return RedirectToAction("Index");
            }

        }

        [HttpPost]
        public ActionResult Edit(RoomViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (Request["Submit"] == "Save")
                {
                    var Room = RoomRepository.GetRoomById(model.Id);
                    AutoMapper.Mapper.Map(model, Room);
                    Room.ModifiedUserId = WebSecurity.CurrentUserId;
                    Room.ModifiedDate = DateTime.Now;
                    RoomRepository.UpdateRoom(Room);

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
            var Room = RoomRepository.GetRoomById(Id.Value);
            if (Room != null && Room.IsDeleted != true)
            {
                var model = new RoomViewModel();
                AutoMapper.Mapper.Map(Room, model);

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
        #endregion

        #region Delete
        [HttpPost]
        public ActionResult Delete()
        {
            try
            {
                string id = Request["Delete"];
                string idDeleteAll = Request["DeleteId-checkbox"];
                
                if (id != null)
                {
                    var item = RoomRepository.GetRoomById(int.Parse(id, CultureInfo.InvariantCulture));
                    if (item != null)
                    {
                        if (item.CreatedUserId != Helpers.Common.CurrentUser.Id && Helpers.Common.CurrentUser.UserTypeId != 1)
                        {
                            TempData["FailedMessage"] = "NotOwner";
                            return RedirectToAction("Index");
                        }

                        item.IsDeleted = true;
                        RoomRepository.UpdateRoom(item);
                    }
                }
                else
                {
                   
                        string[] arrDeleteId = idDeleteAll.Split(',');
                        for (int i = 0; i < arrDeleteId.Count(); i++)
                        {
                            var item = RoomRepository.GetRoomById(int.Parse(arrDeleteId[i], CultureInfo.InvariantCulture));
                            if (item != null)
                            {
                                if (item.CreatedUserId != Helpers.Common.CurrentUser.Id && Helpers.Common.CurrentUser.UserTypeId != 1)
                                {
                                    TempData["FailedMessage"] = "NotOwner";
                                    return RedirectToAction("Index");
                                }

                                item.IsDeleted = true;
                                RoomRepository.UpdateRoom(item);
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
       

        public ActionResult BedIndex(int Id)
        {


            Room thisroom = RoomRepository.GetRoomById(Id);
            List<Bed> q = BedRepository.GetAllBedbyIdRoom(Id);

            ViewBag.IdRoom = Id;


            //ViewBag.SuccessMessage = TempData["SuccessMessage"];
            //ViewBag.FailedMessage = TempData["FailedMessage"];
            //ViewBag.AlertMessage = TempData["AlertMessage"];
            return View(q);
        }
        [HttpPost]
        public ActionResult DeleteBed()
        {


            try
            {
                string idDeleteAll = Request["DeleteId-checkbox"];
                string[] arrDeleteId = idDeleteAll.Split(',');
                for (int i = 0; i < arrDeleteId.Count(); i++)
                {
                    var item = BedRepository.GetBedById(int.Parse(arrDeleteId[i]));
                    if (item != null)
                    {


                        item.IsDeleted = true;
                        BedRepository.UpdateBed(item);
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
            return RedirectToAction("Index");
        }

        public ActionResult CreateBed(int Id)
        {
           
                var model = new Bed();
                model.Room_Id = Id;



                ViewBag.IdRoom = Id;

                return View(model);
          

        }
        [HttpPost]
        public ActionResult CreateBed(Bed bed)
        {

            
            if (ModelState.IsValid)
            {

                bed.IsDeleted = false;
                bed.Trang_Thai = false;
                BedRepository.InsertBed(bed);
              

                TempData[Globals.SuccessMessageKey] = App_GlobalResources.Wording.InsertSuccess;
                return RedirectToAction("Index");
            }

            return View();

          
        }


        #endregion
    }
}
