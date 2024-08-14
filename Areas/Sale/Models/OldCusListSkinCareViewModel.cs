using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Erp.BackOffice.Sale.Models
{
    public class OldCusListSkinCareViewModel
    {
        public int? NVQL_ID { get; set; }
        public string NhanVienQL { get; set; }
        public int? NhomHuongDS_Id { get; set; }
        public string NHomHuongDS { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public DateTime? NgayCSDGN { get; set; }
        public DateTime? NgayHen { get; set; }
        public DateTime? GioHen { get; set; }
        public string ChuongTrinh { get; set; }
        public int? BranchId { get; set; }
    }
}