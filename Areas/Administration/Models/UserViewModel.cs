using Erp.BackOffice.App_GlobalResources;
using Erp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Erp.BackOffice.Areas.Administration.Models
{
    public class UserViewModel
    {
        public int Id { get; set; }
        public int? UserTypeId { get; set; }
        public int? UserType_kd_id { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public int? LoginFailedCount { get; set; }
        public string UserTypeName { get; set; }
        public string UserTypeName_kd { get; set; }
        public UserStatus? Status { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string ListHouse { get; set; }
        public string PlayerId { get; set; }
        public string PlayerId_web { get; set; }
        public string PositionName { get; set; }
        public string FullNameManager { get; set; }
        public decimal? Discount { get; set; }
        public int? IdManager { get; set; }
        public int? BranchId { get; set; }
        public string BranchName { get; set; }
        


    }
}