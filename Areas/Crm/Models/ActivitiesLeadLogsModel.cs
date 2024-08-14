using Erp.BackOffice.App_GlobalResources;
using Erp.BackOffice.Sale.Models;
using Erp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Erp.BackOffice.Crm.Models
{
    public class ActivitiesPlusModel: ActivitiesLeadLogsModel
    {
        public int AssignedUserId { get; set; }
    }
    public class ActivitiesLeadLogsModel
    {
        [DisplayName("ID")]
        public int Id { get; set; }
        [DisplayName("LeadId")]
        public int LeadId { get; set; }
        [DisplayName("Nguồn dữ liệu")]
        public int TypeLead { get; set; }
        [DisplayName("Tên hoạt động")]
        public string Action { get; set; }
        [DisplayName("Người chịu trách nhiệm")]
        public string AssignedUserName { get; set; }
        [DisplayName("Tên Lead")]
        public string LeadName { get; set; }
        [DisplayName("Khởi tạo lúc")]
        public DateTime CreatedDate { get; set; }
        [DisplayName("Trạng thái")]
        public bool Status { get; set; }
        [DisplayName("Bắt đầu")]
        public DateTime CreatedDates { get; set; }
        [DisplayName("Kết thúc")]
        public DateTime CombinedDateTime { get; set; }
        [DisplayName("Mô tả")]
        public string Content { get; set; }

    }
    public class ActivitieseadSearchModel
    {
        public string[] actionValueArr { get; set; }
        public string leadNameKey { get; set; }
        public string[] statusValueArr { get; set; }
        public string createdDateStart { get; set; }
        public string createdDateEnd { get; set; }
        public string combinedDateTimeStart { get; set; }
        public string combinedDateTimeEnd { get; set; }
        public string contentKey { get; set; }
        public string[] typeValueArr { get; set; }
        public int? HDValueArr { get; set; }
        public int[] UsrValueArr { get; set; }
    }
}