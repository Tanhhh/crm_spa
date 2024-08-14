using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Erp.BackOffice.Crm.Models
{
    public class ListNotiViewModel
    {
        public int Id { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        public Nullable<int> CreatedUserId { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> ModifiedUserId { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }

        public string Body { get; set; }
        public string Title { get; set; }

        public string ApplyFor { get; set; }
        public string ApplyUser { get; set; }
        public int? BranchId { get; set; }
        public bool? IsSend { get; set; }

        [Display(Name = "Nhóm hưởng DS")]
        public List<string> NhomDS { get; set; }

        [Display(Name = "Nhân viên quản lý")]
        public List<string> Nhanvien { get; set; }

        [Display(Name = "Khách hàng")]
        public List<string> KhachHang { get; set; }
    }
}