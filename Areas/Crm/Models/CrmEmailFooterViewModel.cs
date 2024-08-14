using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Erp.BackOffice.Areas.Crm.Models
{
    public class CrmEmailFooterViewModel
    {

        public int Id { get; set; }

        [Required(ErrorMessage = "Hãy nhập nội dung tin nhắn!")]
        [Display(Name = "Nội dung Footer")]
        public string Logs { get; set; }

        
        [Display(Name = "IsDeleted")]
        public bool IsDeleted { get; set; }

        [Display(Name = "CreatedDate")]
        public DateTime? CreatedDate { get; set; }

        [Display(Name = "CreatedUserId")]
        public int CreatedUserId { get; set; }

        [Display(Name = "ModifiedDate")]
        public DateTime? ModifiedDate { get; set; }

        [Display(Name = "ModifiedUserId")]
        public int ModifiedUserId { get; set; }

        //[Required]
        [Display(Name = "AssignedUserId")]
        public int AssignedUserId { get; set; }
    
   
    }
}