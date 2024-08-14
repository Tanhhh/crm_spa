using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Erp.BackOffice.Areas.Sale.Models
{
    public class Sale_BaoCaoKH_PhatSinh
    {
        public string ManagerStaffName { get; set; }
        public int? ManagerStaffId { get; set; }
        public int NhomHuongDS { get; set; }
        public string NhomNVKD { get; set; }
        public int TongPhatSinhMoi { get; set; }
        public int TongMoiGd { get; set; }
        public int TongMoiGdMotLan { get; set; }
        public int TongTamNgung { get; set; }
        public int TongTamNgungS { get; set; }
        public int TongActive { get; set; }
        public int ToTal { get; set; }
        public int? BranchId { get; set; }
        public string BranchName { get; set; }
    }
}