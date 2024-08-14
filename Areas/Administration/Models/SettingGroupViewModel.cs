using Erp.BackOffice.App_GlobalResources;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Erp.BackOffice.Areas.Administration.Models
{
    public class SettingGroupViewModel
    {
        public string Name { get; set; }
        public List<SettingViewModel> ListSetting { get; set; }
    }
}