
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Erp.BackOffice.Sale.Models
{
    public class Sale_Get_CustomerViewModel
    {
        public int Id { get; set; }
        public int? KhCuMuonBo { get; set; }
        public int? KhCuThanPhien { get; set; }
        public int? KhMoiDenVaHuaQuayLai { get; set; }
        public int? KhMoiDenKinhTeYeu { get; set; }
        public int? KhLauNgayKhongTuongTac { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CompanyName { get; set; }
        public string DistrictName { get; set; }
        public string Phone { get; set; }
        public string Code { get; set; }
        public int? BranchId { get; set; }
        public string Address {get;set;}
        public string SkinLevel { get; set; }
        public string HairlLevel { get; set; }
        public string GladLevel { get; set; }
        public int? ManagerStaffId { get; set; }
        public string ManagerStaffName { get; set; }
        public string CustomerType { get; set; }
        public bool? IsLock { get; set; }
    }
}