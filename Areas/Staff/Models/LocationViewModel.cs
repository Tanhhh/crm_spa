using Erp.BackOffice.App_GlobalResources;
using Erp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Erp.BackOffice.Staff.Models
{
    public class LocationViewModel
    {
        [StringLength(10, ErrorMessageResourceType = typeof(Error), ErrorMessageResourceName = "StringError", ErrorMessage = null)]
        public string DistrictId { get; set; }
        [StringLength(255, ErrorMessageResourceType = typeof(Error), ErrorMessageResourceName = "StringError", ErrorMessage = null)]
        public string Name { get; set; }
        [StringLength(10, ErrorMessageResourceType = typeof(Error), ErrorMessageResourceName = "StringError", ErrorMessage = null)]
        public string ProvinceId { get; set; }
        [StringLength(255, ErrorMessageResourceType = typeof(Error), ErrorMessageResourceName = "StringError", ErrorMessage = null)]
        public string Type { get; set; }
        [StringLength(10, ErrorMessageResourceType = typeof(Error), ErrorMessageResourceName = "StringError", ErrorMessage = null)]
        public string WardId { get; set; }
        [StringLength(255, ErrorMessageResourceType = typeof(Error), ErrorMessageResourceName = "StringError", ErrorMessage = null)]
        public string Location { get; set; }
        public string Group { get; set; }
        public string Setting { get; set; }
        public int PHISHIP { get; set; }
        public string MA_DVIQLY { get; set; }
        public int STT { get; set; }
    }
}