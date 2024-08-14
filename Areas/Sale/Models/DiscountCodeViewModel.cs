using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Erp.BackOffice.Sale.Models
{
    public class DiscountCodeViewModel
    {

        public int Id { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        public Nullable<int> CreatedUserId { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> ModifiedUserId { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        //public Nullable<int> AssignedUserId { get; set; }
        [Display(Name = "Tên")]
        public string Name { get; set; }

        [Display(Name = "Mã")]
        public string Code { get; set; }
        //public string Type { get; set; }
        public Nullable<System.DateTime> StartDate { get; set; }
        public Nullable<System.DateTime> EndDate { get; set; }
        //public string ApplyFor { get; set; }
        [Display(Name = "Ghi chú")]
        public string Note { get; set; }
        public int? Status { get; set; }
        public string MA_DVIQLY { get; set; }
        [Display(Name = "Giảm %")]
        public int? Discount { get; set; }

        [Display(Name = "Giảm tiền")]
        public decimal? DiscountMoney { get; set; }

        public string ApplyFor { get; set; }
        public string ApplyUser { get; set; }
    }
}