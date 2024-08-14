using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Erp.BackOffice.Areas.Administration.Models
{
    public class UserHistoryViewModel
    {
        public int Id { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string UserName { get; set; }
        public int UserId { get; set; }
        public Nullable<int> BranchId { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        public string Action { get; set; }
        public string Content { get; set; }
        public long TargetId { get; set; }
        public string TargetCode { get; set; }
    }
}