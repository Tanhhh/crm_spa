using Erp.BackOffice.App_GlobalResources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;

namespace Erp.BackOffice.Crm.Models
{
    public class AppointmentViewModel
    {
        public int Id { get; set; }
        public int LeadId { get; set; }
        public string Action { get; set; }

        [Display(Name = "Date of Action")]
        public string DateAction { get; set; }

        [Display(Name = "Time of Action")]
        public string TimeAction { get; set; }

        [Display(Name = "Time to Execute")]
        public int? TimeExcute { get; set; }

        public string Title { get; set; }

        [Display(Name = "Event Location")]
        public string LocationEvent { get; set; }

        public string Note { get; set; }
        public int? UserId { get; set; }

        [Display(Name = "Number of Minutes Warning")]
        public int? Numberofminutes_Warning { get; set; }

        public int? Status { get; set; }
        public bool? TypeLead { get; set; }
        public int BranchId { get; set; }
        public bool? isKpi { get; set; }
        public bool? IsDeleted { get; set; }

        [Display(Name = "Created Date")]
        public DateTime? CreatedDate { get; set; }

        [Display(Name = "Created User ID")]
        public int? CreatedUserId { get; set; }

        [Display(Name = "Modified Date")]
        public DateTime? ModifiedDate { get; set; }

        [Display(Name = "Modified User ID")]
        public int? ModifiedUserId { get; set; }

        [Display(Name = "Assigned User ID")]
        public int? AssignedUserId { get; set; }
        public string LeadName { get; set; }
        public string AssignedUserName { get; set; }
        public string BranchName { get; set; }
        public string CombinedDateTime { get; set; }
        public int? BoPhanId { get; set; }
    }
    public class BranchVModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
    }
    public class BoPhanVModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public int BranchId { get; set; }
    }
    public class SalesVirtualReportViewModel
    {
        public int Id { get; set; }
        public string LeadName { get; set; }
        public string Topic { get; set; }
        public string DoanhSoAo { get; set; }
        public string Mobile { get; set; }
        public string AssignedUserName { get; set; }
        public int? AssignedUserId { get; set; }
        public int? BoPhanId { get; set; }
        public int? BranchId { get; set; }
        public string BranchName { get; set; }
    }
    public class TargetOptionModel
    {
        public int Value { get; set; }
        public string Content { get; set; }
    }
    public class EmployeePerRsReportModel
    {
        public int BranchId { get; set; }
        public int BoPhanId { get; set; }
        public string BoPhanName { get; set; }
        public int AssignedUserId { get; set; }
        public string AssignedUserName { get; set; }
        public string TotalTarget { get; set; }
        public decimal TotalTargetNewOfDay { get; set; }
        public double percentTotalTarget { get; set; }
        public double percentDat { get; set; }
        public double percentChuaDat { get; set; }
    }
    public class TargetNewOfDayeModel
    {
        public int TargetofDay_Id { get; set; }
        public int Target_Id { get; set; }
        public int BranchId { get; set; }
        public int THANG { get; set; }
        public int NAM { get; set; }
        public int UserId { get; set; }
        public int TypeTarget { get; set; }
        public decimal Day1 { get; set; }
        public decimal Day2 { get; set; }
        public decimal Day3 { get; set; }
        public decimal Day4 { get; set; }
        public decimal Day5 { get; set; }
        public decimal Day6 { get; set; }
        public decimal Day7 { get; set; }
        public decimal Day8 { get; set; }
        public decimal Day9 { get; set; }
        public decimal Day10 { get; set; }
        public decimal Day11 { get; set; }
        public decimal Day12 { get; set; }
        public decimal Day13 { get; set; }
        public decimal Day14 { get; set; }
        public decimal Day15 { get; set; }
        public decimal Day16 { get; set; }
        public decimal Day17 { get; set; }
        public decimal Day18 { get; set; }
        public decimal Day19 { get; set; }
        public decimal Day20 { get; set; }
        public decimal Day21 { get; set; }
        public decimal Day22 { get; set; }
        public decimal Day23 { get; set; }
        public decimal Day24 { get; set; }
        public decimal Day25 { get; set; }
        public decimal Day26 { get; set; }
        public decimal Day27 { get; set; }
        public decimal Day28 { get; set; }
        public decimal Day29 { get; set; }
        public decimal Day30 { get; set; }
        public decimal Day31 { get; set; }
        public string Note { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedDate { get; set; }
        public int CreatedUserId { get; set; }
        public DateTime ModifiedDate { get; set; }
        public int ModifiedUserId { get; set; }
    }
    public class EmployeeTitleReportModel
    {
        public int BranchId { get; set; }
        public int BoPhanId { get; set; }
        public string BoPhanName { get; set; }
        public int AssignedUserId { get; set; }
        public string AssignedUserName { get; set; }
        public string TotalCall { get; set; }
        public string TotalPlay { get; set; }
        public string TotalHen { get; set; }
        public string TotalLen { get; set; }
        public string TotalMua { get; set; }
        public string TotalDSAo { get; set; }
    }
    public class EmployeeTitleTableReportModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double PercentCall { get; set; }
        public double PercentPlay { get; set; }
        public double PercentHen { get; set; }
        public double PercentLen { get; set; }
        public double PercentMua { get; set; }
        public double PercentDSAo { get; set; }
    }
    public class DateRange
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public int StartDay { get; set; }
        public int EndDay { get; set; }
    }
    public class EmployeeLenMuaReportModel
    {
        public int BranchId { get; set; }
        public int BoPhanId { get; set; }
        public string BoPhanName { get; set; }
        public int AssignedUserId { get; set; }
        public string AssignedUserName { get; set; }
        public string TotalLen { get; set; }
        public string TotalMua { get; set; }
        public string TotalDSAo { get; set; }
    }
    public class RatingTopicReportModel
    {
        public int BranchId { get; set; }
        public string BranchName { get; set; }
        public string Topic { get; set; }
        public string Total { get; set; }
    }
    public class DashboardReportModel : EmployeeTitleReportModel
    {
        public string TotalSecond { get; set; }

    }
    public class DhRpDetailModel
    {
        public int Key { get; set; }
        public string Name { get; set; }
        public string SumValue { get; set; }
        public double PercentDat { get; set; }
        public double PercentChuaDat { get; set; }
        public double PercentChange { get; set; }
        
    }
    public class TargetNewModel
    {
        public int? BranchId { get; set; }
        public int UserId { get; set; }
        public int TypeTarget { get; set; }
        public decimal TargetTotal { get; set; }
    }
    public class PercentBranchModel
    {
        public int BranchId { get; set; }
        public int TypeTarget { get; set; }
        public double PercentDat { get; set; }
    }
}
