using Erp.BackOffice.Account.Models;
using Erp.BackOffice.App_GlobalResources;
using Erp.BackOffice.Sale.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Erp.BackOffice.Areas.Sale.Models
{
    public class KH_DangCoGDViewModel
    {
        public int Id { get; set; }

        [Display(Name = "IsDeleted")]
        public bool? IsDeleted { get; set; }

        [Display(Name = "CreatedUser", ResourceType = typeof(Wording))]
        public int? CreatedUserId { get; set; }
        public string CreatedUserName { get; set; }

        [Display(Name = "CreatedDate", ResourceType = typeof(Wording))]
        public DateTime? CreatedDate { get; set; }

        [Display(Name = "ModifiedUser", ResourceType = typeof(Wording))]
        public int? ModifiedUserId { get; set; }
        public string ModifiedUserName { get; set; }

        [Display(Name = "ModifiedDate", ResourceType = typeof(Wording))]
        public DateTime? ModifiedDate { get; set; }

        [Display(Name = "Code", ResourceType = typeof(Wording))]
        public string Code { get; set; }

        [StringLength(100, ErrorMessageResourceType = typeof(Error), ErrorMessageResourceName = "StringError", ErrorMessage = null)]
        [Display(Name = "FirstName", ResourceType = typeof(Wording))]
        public string FirstName { get; set; }

        [StringLength(100, ErrorMessageResourceType = typeof(Error), ErrorMessageResourceName = "StringError", ErrorMessage = null)]
        [Display(Name = "LastName", ResourceType = typeof(Wording))]
        public string LastName { get; set; }

        [Display(Name = "FullName", ResourceType = typeof(Wording))]
        public string FullName { get; set; }

        [Display(Name = "Birthday", ResourceType = typeof(Wording))]
        public Nullable<System.DateTime> Birthday { get; set; }

        [Display(Name = "Gender", ResourceType = typeof(Wording))]
        public Nullable<bool> Gender { get; set; }

        [Display(Name = "Note", ResourceType = typeof(Wording))]
        public string Note { get; set; }

        [Display(Name = "Address", ResourceType = typeof(Wording))]
        public string Address { get; set; }

        [Display(Name = "WardName", ResourceType = typeof(Wording))]
        public string WardId { get; set; }

        [Display(Name = "DistrictName", ResourceType = typeof(Wording))]
        public string DistrictId { get; set; }

        [Display(Name = "CityName", ResourceType = typeof(Wording))]
        public string CityId { get; set; }

        //[Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Error))]
        [Display(Name = "Phone", ResourceType = typeof(Wording))]
        public string Phone { get; set; }

        [Display(Name = "Mobile", ResourceType = typeof(Wording))]
        public string Mobile { get; set; }

        [Display(Name = "Email", ResourceType = typeof(Wording))]
        public string Email { get; set; }

        //[Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Error))]
        [Display(Name = "CustomerName", ResourceType = typeof(Wording))]
        public string CompanyName { get; set; }

        [Display(Name = "BonusScore", ResourceType = typeof(Wording))]
        public int? Point { get; set; }

        [Display(Name = "Province", ResourceType = typeof(Wording))]
        public string ProvinceName { get; set; }

        [Display(Name = "District", ResourceType = typeof(Wording))]
        public string DistrictName { get; set; }

        [Display(Name = "Ward", ResourceType = typeof(Wording))]
        public string WardName { get; set; }

        [Display(Name = "GenderName", ResourceType = typeof(Wording))]
        public string GenderName { get; set; }

        public List<ObjectAttributeValueViewModel> AttributeValueList { get; set; }

        [Display(Name = "CardCodeCustomer", ResourceType = typeof(Wording))]
        public string CardCode { get; set; }

        public string SearchFullName { get; set; }

        [Display(Name = "ProfileImage", ResourceType = typeof(Wording))]
        public string Image { get; set; }
        public string Image_Path { get; set; }
        public string Image_File { get; set; }
        [Display(Name = "IdCardNumber", ResourceType = typeof(Wording))]
        public string IdCardNumber { get; set; }

        [Display(Name = "IdCardDate", ResourceType = typeof(Wording))]
        public Nullable<System.DateTime> IdCardDate { get; set; }

        [Display(Name = "IdCardIssued", ResourceType = typeof(Wording))]
        public string IdCardIssued { get; set; }

        [Display(Name = "IdCardIssued", ResourceType = typeof(Wording))]
        public string CardIssuedName { get; set; }
        public decimal Liabilities { get; set; }
        public IEnumerable<SelectListItem> ProvinceList { get; set; }

        [Display(Name = "CustomerGroup", ResourceType = typeof(Wording))]
        public string CustomerGroup { get; set; }

        [Display(Name = "CustomerType", ResourceType = typeof(Wording))]
        public string CustomerType { get; set; }

        [Display(Name = "EconomicStatus", ResourceType = typeof(Wording))]
        public string EconomicStatus { get; set; }


        [Display(Name = "BranchName", ResourceType = typeof(Wording))]
        public int? BranchId { get; set; }

        [Display(Name = "IsBonusSales", ResourceType = typeof(Wording))]
        public bool? IsBonusSales { get; set; }

        [Display(Name = "KhCuMuonBo", ResourceType = typeof(Wording))]
        public bool KhCuMuonBo { get; set; }
        [Display(Name = "KhCuThanPhien", ResourceType = typeof(Wording))]
        public bool KhCuThanPhien { get; set; }
        [Display(Name = "KhLauNgayKhongTuongTac", ResourceType = typeof(Wording))]
        public bool KhLauNgayKhongTuongTac { get; set; }
        [Display(Name = "KhMoiDenVaHuaQuaiLai", ResourceType = typeof(Wording))]
        public bool KhMoiDenVaHuaQuaiLai { get; set; }
        [Display(Name = "KhMoiDenVaKinhTeYeu", ResourceType = typeof(Wording))]
        public bool KhMoiDenVaKinhTeYeu { get; set; }
        [Display(Name = "ManagerStaff", ResourceType = typeof(Wording))]
        public int? ManagerStaffId { get; set; }
        public string BranchName { get; set; }
        public string ManagerStaffName { get; set; }

        public string ManagerUserName { get; set; }
        [Display(Name = "SkinLevel", ResourceType = typeof(Wording))]
        public string SkinSkinLevel { get; set; }
        [Display(Name = "HairlLevel", ResourceType = typeof(Wording))]
        public string HairlLevel { get; set; }
        [Display(Name = "GladLevel", ResourceType = typeof(Wording))]
        public string GladLevel { get; set; }

        public string cus_crm { get; set; }
        public int? PlusPoint { get; set; }
        public string JavaScriptToRun { get; set; }
        //public string CustomerGroup { get; set; }

        [Display(Name = "StartDate", ResourceType = typeof(Wording))]
        public Nullable<System.DateTime> StartDate { get; set; }
        public Nullable<System.DateTime> _StartDate { get; set; }

        public List<CustomerUserViewModel> CustomerUserList { get; set; }

        public List<CustomerRecommendViewModel> CustomerRecommendList { get; set; }

        public int? TYLE_THANHCONG { get; set; }
        public int? TYLE_THANHCONG_REVIEW { get; set; }

        public string GHI_CHU { get; set; }
        public int? is_checked { get; set; }
        [Display(Name = "Ngừng theo dõi")]
        public bool? isLock { get; set; }
        [Display(Name = "IdCustomer_Gioithieu", ResourceType = typeof(Wording))]

        public int IdCustomer_Gioithieu { get; set; }
        public string Phoneghep { get; set; }

        public string NgayMuaDau { get; set; }
        public string NgayMuaCuoi { get; set; }
        public decimal? TotalAmount { get; set; }
        public decimal? TienNo { get; set; }

        [Display(Name = "Nhóm hưởng DS")]
        public int? NhomHuongDS { get; set; }
        public string TenNhomHuong { get; set; }
        public DateTime? NgayGDGN { get; set; }
        public int SoNgayGD { get; set; }
        public int SoLanGD { get; set; }
        public string LoyaltyPointName { get; set; }

        public decimal? tongtiendatra { get; set; }
    }
}