using Erp.BackOffice.App_GlobalResources;
using Erp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Erp.BackOffice.Crm.Models
{
    public class Crm_SendPlanViewModel
    {
        public int Id { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        [Display(Name = "Ngày tạo")]
        public Nullable<DateTime> CreatedDate { get; set; }
        [Display(Name = "Ngày sửa")]
        public Nullable<DateTime> ModifiedDate { get; set; }

        [Display(Name = "Trạng thái")]
        public Nullable<int> IsApprove { get; set; }
        public int NGUOILAP_ID { get; set; }
        [Display(Name = "Người lập")]
        public string Name { get; set; }
        [Display(Name = "Tháng")]
        public int Month { get; set; }
        [Display(Name = "Năm")]
        public int Year { get; set; }

        [Display(Name = "Ghi chú")]
        public string Note { get; set; }

        public int? ModifiedUserId { get; set; }
        public int? CreatedUserId { get; set; }

        public int? BranchId { get; set; }

        [Display(Name = "Người duyệt")]
        public string NguoiDuyet { get; set; }
    }
}