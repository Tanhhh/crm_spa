using Erp.BackOffice.App_GlobalResources;
using Erp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Erp.BackOffice.Crm.Models
{
    public class AssignedToUUser
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public int UserType_kd_id { get; set; }
    }
}