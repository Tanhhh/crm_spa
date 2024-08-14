using Erp.BackOffice.App_GlobalResources;
using Erp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Erp.BackOffice.Crm.Models
{
    public class ChartTotalChangeOutBoundCallViewModel
    {
        public string Action { get; set; }
        public int SaiSo { get; set; }
        public int Nam { get; set; }
        public int TatMayNgang_KhoKhaiThac { get; set; }

        public int DeNghiHuy { get; set; }

        public int ChuaCoNhuCau { get; set; }

        public int KhongNgheMay { get; set; }

        public int SimKhoa { get; set; }

        public int GuiThongTinQuaZalo { get; set; }

        public int DatHenKhongTC { get; set; }
        public int NhacHenXa { get; set; }
        public int NhacHenGan { get; set; }
        public int DatHen { get; set; }
        public int GoiLaiSau { get; set; }
        public int TheoLai { get; set; }

    }

    public class ChartTotalChangeOutBoundCallLeadViewModel
    {
        public string LeadName { get; set; }

        public string Mobile { get; set; }

        public string StatusName { get; set; }

        public string Deadline { get; set; }

        public string ModifiedDate { get; set; }

        public string ResponsibleUserName { get; set; }

    }
}