using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Erp.BackOffice.Sale.Models
{
    public class HoaHongNVViewModel
    {
        public int userid { get; set; }
        public int branchid { get; set; }
        public string userName { get; set; }
        public decimal? TongLuong { get; set; }
        //public decimal? TyleHH { get; set; }

        //target
        public decimal? orlaneTarget { get; set; }
        public decimal? oldorlaneTarget { get; set; }
        public decimal? neworlaneTarget { get; set; }
        public decimal? annayakeTarget { get; set; }
        public decimal? LennorGreylTarget { get; set; }
        public decimal? TotalTarget { get; set; }

        //Thực tế
        public decimal? orlane { get; set; }
        public decimal? oldorlane { get; set; }
        public decimal? neworlane { get; set; }
        public decimal? annayake { get; set; }
        public decimal? LennorGreyl { get; set; }
        public decimal? Total { get; set; }

        //tỷ lệ đạt
        public decimal? TyleOrlane { get; set; }
        public decimal? TyleoldOrlane { get; set; }
        public decimal? TylenewOrlane { get; set; }
        public decimal? TyleAnna { get; set; }
        public decimal? TyleLenor { get; set; }
        public decimal? TyleTotal { get; set; }

        //tiền
        public decimal? moneyOrlane { get; set; }
        public decimal? moneyOldOrlane { get; set; }
        public decimal? moneyNewOrlane { get; set; }
        public decimal? moneyAnna { get; set; }
        public decimal? moneyLenor { get; set; }

        //tỷ lệ Hoahong
        public decimal? HoahongOrlane { get; set; }
        public decimal? HoahongOldOrlane { get; set; }
        public decimal? HoahongNewOrlane { get; set; }
        public decimal? HoaHongAnna { get; set; }
        public decimal? HoahongLenor { get; set; }
    }

    public class HoaHongChiNhanhViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Tháng")]
        public int month { get; set; }
        [Display(Name = "Năm")]
        public int year { get; set; }
        public int BranchId { get; set; }
        public string BranchName { get; set; }

        [Display(Name = "DS cũ Orlane")]
        public decimal? OldOrlane { get; set; }

        [Display(Name = "DS mới Orlane")]
        public decimal? NewOrlane { get; set; }

        [Display(Name = "DS Annayake")]
        public decimal? Annayake { get; set; }

        [Display(Name = "DS Lennor Greyl")]
        public decimal? LennorGreyl { get; set; }
        public decimal? Orlane { get; set; }
        public decimal? Total { get; set; }
        public Nullable<bool> IsDeleted { get; set; }

        //
        public decimal? OldOrlane_HT { get; set; }


        public decimal? NewOrlane_HT { get; set; }


        public decimal? Annayake_HT { get; set; }


        public decimal? LennorGreyl_HT { get; set; }
        public decimal? Orlane_HT { get; set; }
        public decimal? Total_HT { get; set; }
    }

    public partial class DSHOTRO_JSON
    {
        public int branch { get; set; }
        public int month { get; set; }
        public int year { get; set; }
        public decimal? New { get; set; }
        public decimal? Old { get; set; }
        public decimal? anna { get; set; }
        public decimal? lennor { get; set; }
    }
}