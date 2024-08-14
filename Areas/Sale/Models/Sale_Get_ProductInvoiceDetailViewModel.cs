
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Erp.BackOffice.Sale.Models
{
    public class Sale_Get_ProductInvoiceDetailViewModel
    {
        public string CustomerName { get; set; }
public string CustomerCode { get; set; }
public int? Quantity { get; set; }
public string ProductCode { get; set; }
public string ProductName { get; set; }

    }
}