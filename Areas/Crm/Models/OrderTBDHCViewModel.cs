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
    public class OrderTBDHCViewModel
    {
        public int Id { get; set; }
        public decimal MinTotal { get; set; }
        public decimal MaxTotal { get; set; }
        public string Name { get; set; }
        public bool? IsDeleted { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? CreatedUserId { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int? ModifiedUserId { get; set; }
        public int? AssignedUserId { get; set; }
    }



}