
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Erp.BackOffice.Sale.Models
{
    public class Sale_TotalCustomerOfDayViewModel
    {
        public int? CustomerId { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public int? BranchId { get; set; }
        public string CustomerPhone { get; set; }
        public int soluong { get; set; }
        public int totalOldCustomer { get; set; }
        public int totalNewCustomer { get; set; }
        public int ManagerStaffId { get; set; }
        public string FullName { get; set; }

        public decimal? aocu;
        public decimal? aomoi;
        public decimal? thuccu;
        public decimal? thucmoi;
        public decimal? tienconno;
        public decimal? tientralai;

        //ảo cũ theo hãng
        public decimal TongAoCuANNAYAKE = 0;
        public decimal TongAoCuLEONORGREYL = 0;
        public decimal TongAoCuORLANEPARIS = 0;
        public decimal TongAoCuCONGNGHECAO = 0;
        public decimal TongAoCuDICHVU = 0;
        //ảo mới theo hãng
        public decimal TongAoMoiANNAYAKE = 0;
        public decimal TongAoMoiLEONORGREYL = 0;
        public decimal TongAoMoiORLANEPARIS = 0;
        public decimal TongAoMoiCONGNGHECAO = 0;
        public decimal TongAoMoiDICHVU = 0;
        //thực cũ theo hãng
        public decimal TongThucCuANNAYAKE = 0;
        public decimal TongThucCuLEONORGREYL = 0;
        public decimal TongThucCuORLANEPARIS = 0;
        public decimal TongThucCuCONGNGHECAO = 0;
        public decimal TongThucCuDICHVU = 0;
        //thực mới theo hãng
        public decimal TongThucMoiANNAYAKE = 0;
        public decimal TongThucMoiLEONORGREYL = 0;
        public decimal TongThucMoiORLANEPARIS = 0;
        public decimal TongThucMoiCONGNGHECAO = 0;
        public decimal TongThucMoiDICHVU = 0;

        //tính tổng
        public decimal TongAoCu = 0;
        public decimal TongThucCu = 0;
        public decimal TongAoMoi = 0;
        public decimal TongThucMoi = 0;
        public decimal TongThanhToan = 0;
        public decimal TongDoanhThu = 0;
        public decimal TongDoanhSo = 0;
        public decimal TongNoCu = 0;
        public decimal TongNoMoi = 0;
        public decimal TongNo = 0;
        public decimal TienThuNo = 0;

        //Số lượng đơn hàng theo hãng
        public int countANNAYAKE_Cu = 0;
        public int countORLANEPARIS_Cu = 0;
        public int countDICHVU_Cu = 0;
        public int countLEONORGREYL_Cu = 0;
        public int countCONGNGHECAO_Cu = 0;

        public int countANNAYAKE_Moi = 0;
        public int countORLANEPARIS_Moi = 0;
        public int countDICHVU_Moi = 0;
        public int countLEONORGREYL_Moi = 0;
        public int countCONGNGHECAO_Moi = 0;
        public int TongDonHuyTrongNgay = 0;

        public List<Sale_TotalCustomerOfDayViewModel> ListOldCustomer;
        public List<Sale_TotalCustomerOfDayViewModel> ListNewCustomer;
    }
}