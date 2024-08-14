using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Erp.BackOffice.Areas.Crm.Models
{
    public class vwCRM_BH_DOANHSOViewModel
    {

        public int KH_BANHANG_DOANHSO_ID { get; set; }
        public int? BranchId { get; set; }
        public int? THANG { get; set; }
        public int? NAM { get; set; }
        public int? NGUOILAP_ID { get; set; }
        public string CountForBrand { get; set; }
        public decimal? TARGET_BRAND { get; set; }
        public decimal? TARGET_DALAP { get; set; }
        public string GHI_CHU { get; set; }
        public bool? IsDeleted { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public int? CreatedUserId { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public int? ModifiedUserId { get; set; }
        public string BranchName { get; set; }
        public string UserName { get; set; }
    }
}