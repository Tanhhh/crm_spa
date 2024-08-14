using Erp.BackOffice.App_GlobalResources;
using Erp.BackOffice.Areas.Cms.Models;
using Erp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.Design.Serialization;
using System.Web.Mvc;

namespace Erp.BackOffice.Sale.Models
{
	public class LeadReport : LeadModel
	{
		public decimal TotalMoney { get; set; }
	}
	public class WinLoseLeadReport
	{
		public string FullName { get; set; }
		public string UserCode { get; set; }
		public int SL { get; set; }
		public int SLWin { get; set; }
		public int SLLose { get; set; }
		public int SLCur { get; set; }
		public float TLSLWin { get; set; }
		public float TLSLLose { get; set; }
		public float TLSLCur { get; set; }
		public float TLGTWin { get; set; }
		public float TLGTLose { get; set; }
		public float TLGTCur { get; set; }
		public decimal GiaTri { get; set; }
		public decimal GiaTriWin { get; set; }
		public decimal GiaTriLose { get; set; }
		public decimal GiaTriCur { get; set; }
	}
	public class ReasonLeadReport
	{
		public string TheEnd { get; set; }
		public decimal TotalMoney { get; set; }
		public float TLDS { get; set; }
		public int SL { get; set; }
		public float TLSL { get; set; }
	}
	public class ImproveLeadReport
	{
		public int BranchId { get; set; }
		public string BranchName { get; set; }
		public int SL { get; set; }
		public float TLSL { get; set; }
	}
	public class StargetLeadReport
	{
		public decimal Starget { get; set; }
		public int TypeTarget { get; set; }
		public int UserId { get; set; }
	}
	public class TBDHALeadReport
	{
		public int BranchId { get; set; }
		public string Name { get; set; }
		public decimal SL { get; set; }
		public decimal TargetSL { get; set; }
        public decimal SLLM { get; set; }
        public decimal TargetSLLM { get; set; }
        public decimal SLNM { get; set; }
        public decimal TargetSLNM { get; set; }
        public decimal TBDHA { get; set; }
        public decimal DSA { get; set; }
        public decimal TargetDSA { get; set; }
        public decimal SLNL { get; set; }
        public decimal TargetSLNL { get; set; }
        public decimal TLSL { get; set; }
        public decimal TargetTLSL { get; set; }
        public string Date { get; set; }
        public string Topic { get; set; }
	}
	public class DtTblLeadReport
	{
		public string LeadName { get; set; }
		public string Deadline { get; set; }
		public string FullName { get; set; }
		public string BranchName { get; set; }
		public string F42 { get; set; }
	}
	public class DtCBTblLeadReport
    {
		public string LeadName { get; set; }
		public string Status { get; set; }
		public decimal Totalmoney { get; set; }
		public string FullName { get; set; }
		public string BranchName { get; set; }
		public string Mobile { get; set; }
	}
	public class AgeLeadReport
	{
		public string FullName { get; set; }
		public int SLWin { get; set; }
		public int SLLose { get; set; }
		public int AgeWin { get; set; }
		public int AgeLose { get; set; }
	}
	public class TargetLeadReport
	{
		public int TypeForecast { get; set; }
		public int mNext { get; set; }
		public int mNextNext { get; set; }
		public decimal TotalMoney { get; set; }
		public float TLTarget { get; set; }
	}
	public class TrendingLeadReport: ReasonLeadReport
	{
		public decimal SLWin { get; set; }
		public decimal GiaTriWin { get; set; }
		public float TLSLWin { get; set; }
	}
	public class LeadModel
	{
		public int Id { get; set; }
		[DisplayName("Nguồn Data")]
		public string Source { get; set; }
		[DisplayName("Loại Data")]
		public string TypeData { get; set; }
		[DisplayName("Topic")]
		public string Topic { get; set; }
		[DisplayName("Tên Lead")]
		public string LeadName { get; set; }
		[DisplayName("Tên")]
		public string Name { get; set; }
		[DisplayName("Mã Khách hàng")]
		public string CustomerCode { get; set; }
		[DisplayName("Điện thoại")]
		public string Mobile { get; set; }
		[DisplayName("Email")]
		public string Email { get; set; }
		[DisplayName("Địa chỉ")]
		public string Address { get; set; }
		[DisplayName("Mã số KC")]
		public string CodeKC { get; set; }
		[DisplayName("Tên công ty")]
		public string CompanyName { get; set; }
		[DisplayName("Điện thoại công ty")]
		public string MobileCompany { get; set; }
		[DisplayName("Email công ty")]
		public string EmailCompany { get; set; }
		[DisplayName("Số thuế")]
		public string Taxcode { get; set; }
		[DisplayName("Năm sinh")]
		public int? YearofBirth { get; set; }
		[DisplayName("NV Tiếp nhận")]
		public int? ReceptionStaffId { get; set; }
		[DisplayName("Đề nghị hủy")]
		public int? IsCancel { get; set; }
		[DisplayName("Trạng thái")]
		public int? StatusId { get; set; }

		public string F1 { get; set; }
		public string F2 { get; set; }
		public string F3 { get; set; }
		public string F4 { get; set; }
		public string F5 { get; set; }
		public string F6 { get; set; }
		public string F7 { get; set; }
		public string F8 { get; set; }
		public string F9 { get; set; }
		public string F10 { get; set; }
		public string F11 { get; set; }
		public string F12 { get; set; }
		public string F13 { get; set; }
		public string F14 { get; set; }
		public string F15 { get; set; }
		public string F16 { get; set; }
		public string F17 { get; set; }
		public string F18 { get; set; }
		public string F19 { get; set; }
		public string F20 { get; set; }
		public string F21 { get; set; }
		public string F22 { get; set; }
		public string F23 { get; set; }
		public string F24 { get; set; }
		public string F25 { get; set; }
		public string F26 { get; set; }
		public string F27 { get; set; }
		public string F28 { get; set; }
		public string F29 { get; set; }
		public string F30 { get; set; }
		public string F31 { get; set; }
		public string F32 { get; set; }
		public string F33 { get; set; }
		public string F34 { get; set; }
		public string F35 { get; set; }
		public string F36 { get; set; }
		public string F37 { get; set; }
		public string F38 { get; set; }
		public string F39 { get; set; }
		public string F40 { get; set; }
		public string F41 { get; set; }
		public string F42 { get; set; }
		public string F43 { get; set; }
		public string F44 { get; set; }
		public string F45 { get; set; }
		public string F46 { get; set; }
		public string F47 { get; set; }
		public string F48 { get; set; }
		public string F49 { get; set; }
		public string F50 { get; set; }
		public string F51 { get; set; }
		public string F52 { get; set; }
		public string F53 { get; set; }
		public string F54 { get; set; }
		public string F55 { get; set; }
		public string F56 { get; set; }
		public string F57 { get; set; }
		public string F58 { get; set; }
		public string F59 { get; set; }
		public string F60 { get; set; }
		public string F61 { get; set; }
		public string F62 { get; set; }
		public string F63 { get; set; }
		public string F64 { get; set; }
		public string F65 { get; set; }
		public string F66 { get; set; }
		public string F67 { get; set; }
		public string F68 { get; set; }
		public string F69 { get; set; }
		public string F70 { get; set; }
		public string F71 { get; set; }
		public string F72 { get; set; }
		public string F73 { get; set; }
		public string F74 { get; set; }
		public string F75 { get; set; }
		public string F76 { get; set; }
		public string F77 { get; set; }
		public string F78 { get; set; }
		public string F79 { get; set; }
		public string F80 { get; set; }
		public string F81 { get; set; }
		public string F82 { get; set; }
		public string F83 { get; set; }
		public string F84 { get; set; }
		public string F85 { get; set; }
		public string F86 { get; set; }
		public string F87 { get; set; }
		public string F88 { get; set; }
		public string F89 { get; set; }
		public string F90 { get; set; }
		public string F91 { get; set; }
		public string F92 { get; set; }
		public string F93 { get; set; }
		public string F94 { get; set; }
		public string F95 { get; set; }
		public string F96 { get; set; }
		public string F97 { get; set; }
		public string F98 { get; set; }
		public string F99 { get; set; }
		public bool? IsDeleted { get; set; }
		public DateTime? CreatedDate { get; set; }
		public int? CreatedUserId { get; set; }
        [DisplayName("Ngày sửa")]
        public DateTime? ModifiedDate { get; set; }
		public int? ModifiedUserId { get; set; }
		public int? AssignedUserId { get; set; }
		public string UserIdZalo { get; set; }
        public string CountForBrand { get; set; }
    }
	public class LeadSectionModel
	{
		public string Name { get; set; }
		public int OrderSection { get; set; }
		public int StatusId { get; set; }
		public int Id { get; set; }
		public int isDefault { get; set; }

	}
	public class LeadSection_FieldModel
	{
		public int Id { get; set; }
		public int LeadSectionId { get; set; }
		public string NameLabel { get; set; }
		public string FieldName { get; set; }
		public int OrderField { get; set; }
		public string TypeField { get; set; }
		public string CodeList { get; set; }
		public bool? IsHiden { get; set; }
		public bool? IsHidenList { get; set; }


	}

	public class KeyValueModel
	{
		public string Key { get; set; }
		public string Value { get; set; }
		public string[] ValueArr { get; set; }
	}
	public class StatusLeadModel
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public decimal SuccessRate { get; set; }
		public string ColorStatus { get; set; }
		public string TypeofStatus { get; set; }
		public int OrderStatus { get; set; }
		public Nullable<bool> EndStatus { get; set; }
		public int Type { get; set; }
		public bool? NotMoveable { get; set; }
		public bool IsDeleted { get; set; }
		public DateTime CreatedDate { get; set; }
		public int CreatedUserId { get; set; }
		public DateTime ModifiedDate { get; set; }
		public int ModifiedUserId { get; set; }
		public int AssignedUserId { get; set; }
		public int LeadCount { get; set; }
		public decimal TotalPrice { get; set; }

	}
	public class SectionCusModel
	{
		public Tuple<IEnumerable<LeadSectionModel>, IEnumerable<LeadSection_FieldModel>> Tuple { get; set; }
		[Display(Name = "CustomerId", ResourceType = typeof(Wording))]
		public int CustomerId { get; set; }
		public string CustomerName { get; set; }
	}
	public class RuleDetailModel
	{
		public int Id { get; set; }
		public int RuleId { get; set; }
		public string FieldName { get; set; } = "";
		public string LabelName { get; set; } = "";
		public string Logic { get; set; } = "";
		public string ContentRule { get; set; } = "";
		public int? StatusFrom { get; set; }
		public int? StatusTo { get; set; }
		public string StatusToName { get; set; }
		public string ContentRuleName { get; set; }
		public int? AndOr { get; set; }
		public string Note { get; set; }
		public bool IsDeleted { get; set; }
		public DateTime CreatedDate { get; set; }
		public int CreatedUserId { get; set; }
		public DateTime ModifiedDate { get; set; }
		public int ModifiedUserId { get; set; }
		public int AssignedUserId { get; set; }

	}
	public class LeadLogsModel
	{
		public int Id { get; set; }
		public string Action { get; set; }
		public int IdAction { get; set; }
		public string Username { get; set; }
		public string Content { get; set; }
		public string Source { get; set; }
		public string Logs { get; set; }
		public string filepath { get; set; }
		public string Mobile { get; set; }
		public bool Status { get; set; }
		public string StatusFromName { get; set; }
		public string StatusToName { get; set; }
		public int IdJob { get; set; }
		public string StateJob { get; set; }
		public bool IsDeleted { get; set; }
		public string Error { get; set; }

        public DateTime CreatedDate { get; set; }
		public int CreatedUserId { get; set; }
		public DateTime ModifiedDate { get; set; }
		public int ModifiedUserId { get; set; }
		public int AssignedUserId { get; set; }

	}
	public class LeadSearchModel : LeadModel {
		public KeyValueModel[] keyValues { get; set; }
	}
	public class QuickEditLeadModel
	{
		public LeadModel[] LeadModel { get; set; }
	}
	public class TyleRuleModel
	{
		public int Id { get; set; }
		public string TypeRule { get; set; }
		public string Note { get; set; }
		public bool IsDeleted { get; set; }
		public DateTime CreatedDate { get; set; }
		public int CreatedUserId { get; set; }
		public DateTime ModifiedDate { get; set; }
		public int ModifiedUserId { get; set; }
		public int AssignedUserId { get; set; }

	}
	public class RuleModel
	{
		public int Id { get; set; }
		public int TypeRuleId { get; set; }
		public int TypeLead { get; set; }
		public string ContentRule { get; set; }
		public string TileEmail { get; set; }
		public string ContentEmail { get; set; }
		public string Note { get; set; }
		public int? StatusFrom { get; set; }
		public int? StatusTo { get; set; }
		public bool IsDeleted { get; set; }
		public DateTime CreatedDate { get; set; }
		public int CreatedUserId { get; set; }
		public DateTime ModifiedDate { get; set; }
		public int ModifiedUserId { get; set; }
		public int AssignedUserId { get; set; }
		public RuleDetailModel ruleDetails { get; set; }
	}

	public class LeadProduct
	{
		public int Id { get; set; }
		public int ProductId { get; set; }
		public string Name { get; set; }
		public string Origin { get; set; }
		public string Unit { get; set; }
		public decimal Quantity { get; set; }
		public decimal Price { get; set; }
		public decimal TotalAmount { get; set; }
		public string Note { get; set; }
		public bool is_TANG { get; set; }
	}
	public class LeadProductCreate
	{
		public int LeadId { get; set; }
		public int ProductId { get; set; }
		public decimal Quantity { get; set; }
		public decimal Price { get; set; }
		public decimal TotalAmount { get; set; }
		public string Note { get; set; }
		public bool IsTang { get; set; }
	}
	public class LeadQuotationIndex
	{
		public int Id { get; set; }
		public string Code { get; set; }
		public DateTime CreatedDate { get; set; }
		public decimal TotalAmount { get; set; }
		public string Status { get; set; }
		public string Note { get; set; }
		public string ReceptionStaffName { get; set; }
		public DateTime UntilDateValue { get; set; }
	}
	public class LeadQuotationPopupIndex
	{
		public string LeadName { get; set; }
		public string Taxcode { get; set; }
		public string Address { get; set; }
		public int ReceptionStaffId { get; set; }
		public string ReceptionStaffName { get; set; }
	}
	public class LeadQuotationPopupCreate
	{
		public int LeadId { get; set; }
		public string Code { get; set; }
		public DateTime CreatedDate { get; set; }
		public DateTime UntilDate { get; set; }
		public string Status { get; set; }
		public string LeadName { get; set; }
		public string TaxCode { get; set; }
		public string Address { get; set; }
		public decimal TotalAmount { get; set; }
		public int ReceptionStaffId { get; set; }
		public LeadProductCreate[] LeadProductLQ { get; set; }
	}
	public class LeadProductInQuotationEdit
	{
		public int Id { get; set; }
		public int ProductId { get; set; }
		public string Name { get; set; }
		public decimal Quantity { get; set; }
		public string Origin { get; set; }
		public decimal Price { get; set; }
		public string Note { get; set; }
		public bool is_TANG { get; set; }
	}
	public class LeadQuotationPopupEdit
	{
		public int Id { get; set; }
		public int LeadId { get; set; }
		public string Code { get; set; }
        public string Email { get; set; }
        public DateTime CreatedDate { get; set; }
		public DateTime UntilDateValue { get; set; }
		public string Status { get; set; }
		public string LeadName { get; set; }
		public string TaxCode { get; set; }
		public string Address { get; set; }
		public decimal TotalAmount { get; set; }
		public int ReceptionStaffId { get; set; }
		public LeadProductInQuotationEdit[] LeadProductLQ { get; set; }
	}
	public class LeadQuotationPopupEditCr
	{ 
		public LeadQuotationPopupEdit data { get; set; }
        public LeadProductCreate[] LeadProductNewLQ { get; set; }
    }
    public class ItemLeadLog
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }
}