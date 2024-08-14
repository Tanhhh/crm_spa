
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Erp.BackOffice.Sale.Models
{
    public class Sale_Report_TuiLieuTrinhViewModel
    {
        public DateTime? CreatedDate { get; set; }
        public string ProductName { get; set; }
        public string ProductCode { get; set; }
        public string ServiceCode { get; set; }
        public string ServiceName { get; set; }
        public int? Quantity { get; set; }

    }
    public class Sale_Report_SanPhamBanTheoThang
    {
        public string Origin { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string SoLuongBanTheoThoiGian { get; set; }
        public decimal? DoanhSo { get; set; }


    }
    public class Sale_Report_CustomerbuyProduct
    {
        public string Code { get; set; }
        public string CompanyName { get; set; }
        public string Phone { get; set; }
        public string Mobile { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public string ManagerStaffName { get; set; }
        public string NhomNVKD { get; set; }

    }
    public class Sale_Report_ProductInVentoryComparedquota
    {
        public string Origin { get; set; }
        public string Name { get; set; }
        public int MinInventory { get; set; }
        public int? Quantity { get; set; }
        public int? SoLuongChenhLech { get; set; }
    }
}