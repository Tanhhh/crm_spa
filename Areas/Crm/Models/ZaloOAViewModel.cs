using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Erp.BackOffice.Areas.Crm.Models
{
    public class ZaloOAViewModel
    {

        public int Id { get; set; }
        public string ZaloOAId { get; set; }
        public bool Status { get; set; }
        public string Note { get; set; }
        public string Name { get; set; }
        public string Avatar { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        public Nullable<int> CreatedUserId { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> ModifiedUserId { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public Nullable<int> AssignedUserId { get; set; }

    }
}
