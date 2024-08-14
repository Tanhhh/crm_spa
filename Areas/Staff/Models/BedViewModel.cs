using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Erp.BackOffice.Areas.Staff.Models
{
    public class BedViewModel
    {
        public int Bed_ID { get; set; }
        public int Room_Id { get; set; }
        public bool IsDeleted { get; set; }
        public string Name_Bed { get; set; }
        public bool Trang_Thai { get; set; }
        public string GHI_CHU { get; set; }
       

    }
}