using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Erp.BackOffice.Sale.Models
{
    public class CheckPlanViewModel
    {
        public int Id { get; set; }
        public int? BranchId { get; set; }
        public int? THANG { get; set; }
        public int? NAM { get; set; }
        public string NGAYLAP { get; set; }
        public string Code { get; set; }

        public string Name { get; set; }
        public int? ManagerStaffId { get; set; }
        public string ManagerStaffName { get; set; }
        public string HINHTHUC_TUONGTAC { get; set; }
        public string GIO_TUONGTAC { get; set; }
       public int? NhomNVKDId { get; set; }
        public string NhomNVKD { get; set; }
        public string Phone { get; set; }
        public string KHBH { get; set; }
        public string KHCDS { get; set; }


        public string CountForBrand { get; set; }
        public decimal? TotalBH { get; set; }
        public decimal? TotalDS { get; set; }
        public int? BHTL { get; set; }
        public int? DSTL { get; set; }

        public string UserApproveName { get; set; }
        public int? UserApprove { get; set; }

        public string KHTT { get; set; }
        public decimal? TARGET_BRAND { get; set; }
        public decimal? TARGETDS { get; set; }

        public DateTime? NgayCSGN { get; set; }
        public decimal? ThucTeBH { get; set; }
        public decimal? ThucTeDS { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}