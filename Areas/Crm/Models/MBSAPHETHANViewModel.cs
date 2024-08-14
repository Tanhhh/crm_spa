using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Erp.BackOffice.Crm.Models
{
    public class MBSAPHETHANViewModel
    {
        public int Id { get; set; }
        public int? Quantity { get; set; }
        public int? CustomerId { get; set; }
        public string Phone { get; set; }
        public string CustomerCode { get; set; }
        public string ExpiryDate { get; set; }
        public string MemberShipStatus { get; set; }
        public int? ManagerStaffId { get; set; }
        public string CustomerName { get; set; }
        public string GHI_CHU { get; set; }
        public int? is_checkedMBSAPHETHAN { get; set; }
        public string ManagerStaffName { get; set; }
        public int? BranchId { get; set; }
    }
}