using Erp.BackOffice.App_GlobalResources;
using Erp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Erp.BackOffice.Sale.Models
{
    public class ChartItem
    {
        public int? managerid { get; set; }
        public string group { get; set; }
        public string label { get; set; }
        public string label2 { get; set; }
        public object data { get; set; }
        public object data2 { get; set; }
        public string id { get; set; }
        public string Code { get; set; } 
        public int? Id { get; set; }
    }

    public class ActiveCustomerViewModel
    {
        //getCustomerActiveByDay @StartDate @EndDate @BranchId
        public int? DauKy { get; set; }
        public int? CuoiKy { get; set; }
        public int? Tang { get; set; }
        public int? Giam { get; set; }
    }
    public class ListMonthYearActiveBrandViewModel
    {
        public int? month { get; set; }
        public int? year { get; set; }
    }
}