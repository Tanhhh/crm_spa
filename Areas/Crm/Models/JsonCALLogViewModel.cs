using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Erp.BackOffice.Crm.Models
{
    public class JsonCALLogViewModel
    {

       
        public int stt { get; set; }
        public string key { get; set; }
        public string ngayGoi { get; set; }
        public string soGoiDen { get; set; }
        public string dauSo { get; set; }
        public string soNhan { get; set; }
        public string trangThai { get; set; }
        public string tongThoiGianGoi { get; set; }
        public string thoiGianThucGoi { get; set; }
        public string linkFile { get; set; }
       

    }
    public class ListJsonCALLogViewModel
    {
        public string result;
        public string message;
        public int total;
        public List<JsonCALLogViewModel> data {get; set;}
    }
  
}