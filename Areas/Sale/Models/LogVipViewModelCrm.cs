using Erp.BackOffice.App_GlobalResources;
using Erp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Erp.BackOffice.Sale.Models
{
    public class LogVipViewModelCrm
    {
        public LogVipViewModelCrm()
        {
        }

        public int Id { get; set; }


        public string Chude { get; set; }
        public string Mota { get; set; }
        public string Thoigianhen { get; set; }
        public string Hoanthanh { get; set; }



    }

    public class SearchLogVipViewModel
    {
        public int Id { get; set; }

        public int? PlusPoint { get; set; }

        [Display(Name = "Code", ResourceType = typeof(Wording))]
        public string Code { get; set; }

        [Display(Name = "TotalAmount", ResourceType = typeof(Wording))]
        public decimal TotalAmount { get; set; }

       
        

       

     
        [Display(Name = "BranchName", ResourceType = typeof(Wording))]
        public int? BranchId { get; set; }

      

        [Display(Name = "CustomerCode", ResourceType = typeof(Wording))]
        public string CustomerCode { get; set; }
        [Display(Name = "CustomerPhone", ResourceType = typeof(Wording))]
        public string CustomerPhone { get; set; }

        [Display(Name = "CustomerName", ResourceType = typeof(Wording))]
        public string CustomerName { get; set; }


        public int? CustomerId { get; set; }

       
       

      
       

        public int? Month { get; set; }
        public int? Year { get; set; }
        public int? Day { get; set; }
     
      


      

        public string Type { get; set; }
        public decimal? DoanhThu { get; set; }
      
        
        public int? NAM { get; set; }
        public string Phone { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public decimal? tienmuahang { get; set; }

        public decimal? tientralai { get; set; }
        public decimal? tongmua { get; set; }

        public string TenHang { get; set; }
        public string loai { get; set; }
        public int IdHang { get; set; }
        public int Idxephangcu { get; set; }
        public decimal? tiendathu { get; set; }
        public decimal? tienconno { get; set; }
        
       

        public string Address { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string NhomNVKD { get; set; }


        




     
    }
}