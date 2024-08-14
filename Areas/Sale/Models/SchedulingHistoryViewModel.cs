using Erp.BackOffice.App_GlobalResources;
using Erp.BackOffice.Areas.Administration.Models;
using Erp.BackOffice.Staff.Models;
using Erp.Domain.Entities;
using Erp.Domain.Staff.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Erp.BackOffice.Sale.Models
{
    public class SchedulingHistoryViewModel
    {
        public int Id { get; set; }

        [Display(Name = "IsDeleted")]
        public bool? IsDeleted { get; set; }

        [Display(Name = "CreatedUser", ResourceType = typeof(Wording))]
        public int? CreatedUserId { get; set; }

        [Display(Name = "CreatedDate", ResourceType = typeof(Wording))]
        public DateTime? CreatedDate { get; set; }

        [Display(Name = "ModifiedUser", ResourceType = typeof(Wording))]
        public int? ModifiedUserId { get; set; }
        public string ModifiedUserName { get; set; }

        [Display(Name = "ModifiedDate", ResourceType = typeof(Wording))]
        public DateTime? ModifiedDate { get; set; }

        [Display(Name = "AssignedUserId", ResourceType = typeof(Wording))]
        public int? AssignedUserId { get; set; }
        public string AssignedUserName { get; set; }
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Error))]
        [Display(Name = "InquiryCode", ResourceType = typeof(Wording))]
        public Nullable<int> InquiryCardId { get; set; }
        [Display(Name = "WorkDay", ResourceType = typeof(Wording))]
        public DateTime? WorkDay { get; set; }
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Error))]
        [Display(Name = "ExecutionTime", ResourceType = typeof(Wording))]
        public Nullable<int> TotalMinute { get; set; }

        [Display(Name = "EndDates", ResourceType = typeof(Wording))]
        public Nullable<System.DateTime> EndDate { get; set; }
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Error))]
        [Display(Name = "ExpectedEndDate", ResourceType = typeof(Wording))]
        public Nullable<System.DateTime> ExpectedEndDate { get; set; }
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Error))]
        [Display(Name = "Customer", ResourceType = typeof(Wording))]
        public Nullable<int> CustomerId { get; set; }
        [Display(Name = "Status", ResourceType = typeof(Wording))]
        public string Status { get; set; }
        [Display(Name = "SchedulingCode", ResourceType = typeof(Wording))]
        public string Code { get; set; }
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Error))]
        [Display(Name = "Room", ResourceType = typeof(Wording))]
        public Nullable<int> RoomId { get; set; }
        [Display(Name = "Bed", ResourceType = typeof(Wording))]
        [Required(ErrorMessage = "{0} is required.")]
        public Nullable<int> BedId { get; set; }
        [Display(Name = "Note", ResourceType = typeof(Wording))]
        public string Note { get; set; }
        [Display(Name = "BranchName", ResourceType = typeof(Wording))]
        public Nullable<int> BranchId { get; set; }

        [Display(Name = "BranchName", ResourceType = typeof(Wording))]
        public string BranchName { get; set; }
        [Display(Name = "BranchCode", ResourceType = typeof(Wording))]
        public string BranchCode { get; set; }
        [Display(Name = "CreateStaffName", ResourceType = typeof(Wording))]
        public string CreatedUserName { get; set; }
        [Display(Name = "CreateStaffCode", ResourceType = typeof(Wording))]
        public string CreatedUserCode { get; set; }
        [Display(Name = "InquiryCode", ResourceType = typeof(Wording))]
        public string InquiryCardCode { get; set; }
        [Display(Name = "InquiryType", ResourceType = typeof(Wording))]
        public string Type { get; set; }
        [Display(Name = "Service", ResourceType = typeof(Wording))]
        public Nullable<int> ProductId { get; set; }
        [Display(Name = "CustomerName", ResourceType = typeof(Wording))]
        public string CustomerName { get; set; }
        [Display(Name = "CustomerCode", ResourceType = typeof(Wording))]
        public string CustomerCode { get; set; }
        [Display(Name = "ServiceCode", ResourceType = typeof(Wording))]
        public string ProductCode { get; set; }
        [Display(Name = "Service", ResourceType = typeof(Wording))]
        public string ProductName { get; set; }
        [Display(Name = "RoomName", ResourceType = typeof(Wording))]
        public string RoomName { get; set; }
        [Display(Name = "FloorName", ResourceType = typeof(Wording))]
        public string FloorName { get; set; }
        public string CustomerImage { get; set; }

        [Display(Name = "Giường")]
        public string Name_Bed { get; set; }
        public string Name_Room { get; set; }
        public List<RoomViewModel> RoomList { get; set; }
    
        public List<InquiryCardViewModel> InquiryCardList { get; set; }
        public List<SchedulingHistoryViewModel> SchedulingList { get; set; }
        public List<UserOnlineViewModel> UserList { get; set; }
        public List<StaffMadeViewModel> StaffMadeList { get; set; }
        public List<ServiceScheduleViewModel> ServiceScheduleList { get; set; }
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Error))]
        [Display(Name = "ExpectedWorkDay", ResourceType = typeof(Wording))]
        public Nullable<System.DateTime> ExpectedWorkDay { get; set; }
        public string strEndDate { get; set; }
        public string startTime { get; set; }
        public string endTime { get; set; }
        public Nullable<int> executionTime { get; set; }
        public Nullable<int> moretime { get; set; }
        public string strexecutionTime { get; set; }
        public string strTotalMinute { get; set; }
        public string ColorStatus { get; set; }
        public string ColorBed { get; set; }
        public string strStatus { get; set; }
        public string CustomerImagePath { get; set; }
        public string NameNV { get; set; }
        public List<LogEquipmentViewModel> EquipmentList { get; set; }
        public string EquimentGroup { get; set; }
        public List<StaffEquipment> StaffEquipmentList { get; set; }
        public List<Category> StaffEquipmentGroup { get; set; }
        public string TargetModule { get; set; }
        public string ExpectFinishHour { get; set; }
    }
}