
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Erp.BackOffice.Sale.Models
{
    public class Sale_BaoCao_KhachTuVanMaKhongMuaViewModel
    {
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string CompanyName { get; set; }
        public string Code { get; set; }
        public string AdviseCardCode { get; set; }
        public string Status { get; set; }
        public int? IsVip { get; set; }
        public string EconomicStatus { get; set; }

    }
}