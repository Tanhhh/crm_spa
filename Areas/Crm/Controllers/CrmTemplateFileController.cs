using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Services.Description;
using Erp.BackOffice.Crm.Models;
using Erp.BackOffice.Sale.Models;
using Erp.Domain.Account.Helper;
using WebMatrix.WebData;
using OfficeOpenXml;
using System.Text.RegularExpressions;
using PagedList;
using Quartz.Util;
using Microsoft.Ajax.Utilities;
using OfficeOpenXml.Style;
using OfficeOpenXml.DataValidation;
using Erp.BackOffice.Helpers;
using System.IO.Packaging;
using System.Net;
using System.Windows.Forms;
using DocumentFormat.OpenXml.Spreadsheet;
using OfficeOpenXml.Style.XmlAccess;
using DocumentFormat.OpenXml.Drawing.Charts;
using GemBox.Spreadsheet;

namespace Erp.BackOffice.Crm.Controllers
{
    public class CrmTemplateFileController : Controller
    {
        public ViewResult Index(string areasT)
        {
            ViewBag.DataSe = null;
            var dataFiled = SqlHelper.QuerySP<CrmTemplateFileViewModel>("spCrm_TemplateExcel_Index").ToList();
            var dataselect = new[] {
                                new DataSelectItem { Key = "BAOGIA", Value = "Báo Giá" },
                                new DataSelectItem { Key = "HOADON", Value = "Hóa Đơn" }
                            };
            ViewBag.Data = dataselect;
            if (areasT != null)
            {
                dataFiled = dataFiled.Where(x => x.Module == areasT).ToList();
                ViewBag.DataSe = areasT;
            }
            return View(dataFiled);
        }
        public ViewResult Create()
        {
            var dataselect = new[] {
                                new DataSelectItem { Key = "BAOGIA", Value = "Báo Giá" },
                                new DataSelectItem { Key = "HOADON", Value = "Hóa Đơn" }
                            };
            ViewBag.Data = dataselect;
            return View();
        }
        [HttpPost]
        public ActionResult CreateTF(HttpPostedFileBase pexcelFile, string pmodule)
        {
            if (pexcelFile == null || pexcelFile.ContentLength == 0)
            {
                return Json(new { Success = false });
            }

            try
            {
                string fileName = $"{pexcelFile.FileName.Substring(0, pexcelFile.FileName.LastIndexOf('.'))}_{GenerateRandomString()}.{pexcelFile.FileName.Substring(pexcelFile.FileName.LastIndexOf('.') + 1)}";
                var filePath = Path.Combine(Server.MapPath("~/Temp"), fileName);
                pexcelFile.SaveAs(filePath);
                var result = SqlHelper.QuerySP<int>("spCrm_TemplateExcel_Create", new
                {
                    @pModule = pmodule,
                    @pFileName = fileName,
                    @pCreatedUserId = WebSecurity.CurrentUserId
                });

                return Json(new { Success = true });
            }
            catch (Exception ex)
            {
                return Json(new { Success = false });
            }
        }
        [HttpGet]
        public ActionResult DownloadFileXLSX(string fileNames)
        {
            try
            {
                string filePath = Path.Combine(Server.MapPath("~/Temp"), fileNames);
                if (!System.IO.File.Exists(filePath))
                {
                    return new HttpStatusCodeResult(HttpStatusCode.NotFound);
                }
                byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);
                string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

                return File(fileBytes, contentType, fileNames);
            }
            catch (Exception ex)
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }
        }
        [HttpPost]
        public ActionResult DeleteTF(int Id)
        {
            try
            {
                var result = SqlHelper.QuerySP<int>("spCrm_TemplateExcel_Delete", new
                {
                    @pId = Id,
                    @pModifiedUserId = WebSecurity.CurrentUserId
                }).FirstOrDefault();
                return Json(new { Success = true });
            }
            catch (Exception ex)
            {
                return Json(new { Success = false });
            }
        }
        public static string GenerateRandomString()
        {
            string pool = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

            StringBuilder sb = new StringBuilder();

            Random rnd = new Random();
            for (int i = 0; i < 3; i++)
            {
                int index = rnd.Next(pool.Length);
                sb.Append(pool[index]);
            }
            return sb.ToString();
        }

        //in BG
        public ViewResult SelectQuotationForm(int id)
        {
            var dataFiled = SqlHelper.QuerySP<CrmTemplateFileViewModel>("spCrm_TemplateExcel_Index").ToList();
            dataFiled = dataFiled.Where(x => x.Module == "BAOGIA").ToList();
            ViewBag.DataSel = dataFiled;

            var datals = SqlHelper.QuerySP<LeadQuotationIndex>("spCrm_LeadQuotation_Index", new
            {
                @pLeadId = id
            });
            ViewBag.DataList = datals;
            ViewBag.LeadId = id;
            return View();
        }
        [HttpGet]
        public ActionResult PrintFileInTL(string fileName, string codeQuo, int isPartial)
        {
            var SystemPara = SqlHelper.QuerySP<SystemParametersViewModel>("spCrm_SystemParameters").ToList();
            var SysParaBG = SystemPara.Where(x => x.Module == "BAOGIA").ToList();
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            string filePath = Path.Combine(Server.MapPath("~/Temp"), fileName);

            if (!System.IO.File.Exists(filePath))
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound, "Không tìm thấy file");
            }
            using (ExcelPackage package = new ExcelPackage(new FileInfo(filePath)))
            {
                OfficeOpenXml.ExcelWorksheet worksheet = package.Workbook.Worksheets.FirstOrDefault();

                if (worksheet != null)
                {
                    int rowCount = worksheet.Dimension.Rows;
                    int colCount = worksheet.Dimension.Columns;

                    int row = 1;

                    while (row <= rowCount)
                    {
                        bool isMatchedRow = true;

                        for (int col = 1; col <= colCount; col++)
                        {
                            ExcelRangeBase cell = worksheet.Cells[row, col];
                            string cellValue = cell.Text;

                            Match match = Regex.Match(cellValue, @"##(.*?)##");

                            if (match.Success)
                            {
                                string extractedValue = match.Groups[1].Value;
                                if (checkIsList(SysParaBG, extractedValue))
                                {
                                    isMatchedRow = false;
                                    break;
                                }
                                else
                                if (extractedValue.Contains("BG_Code"))
                                {
                                    cellValue = Regex.Replace(cellValue, @"##(.*?)##", codeQuo);
                                    cell.Value = cellValue;
                                }
                                else
                                {
                                    var txtxt = getValueInQuota(SysParaBG, extractedValue, codeQuo, isPartial);
                                    cellValue = Regex.Replace(cellValue, @"##(.*?)##", txtxt);
                                    cell.Value = cellValue;
                                }
                            }
                        }

                        if (!isMatchedRow)
                        {
                            int n = 0;
                            for (int col = 1; col <= colCount; col++)
                            {
                                ExcelRangeBase cell = worksheet.Cells[row, col];
                                string cellValue = cell.Text;

                                Match match = Regex.Match(cellValue, @"##(.*?)##");
                                if (match.Success)
                                {
                                    string extractedValue = match.Groups[1].Value;
                                    n = getValueInQuotaDetail(SystemPara, extractedValue, codeQuo, -1, isPartial) != null ? Convert.ToInt32(getValueInQuotaDetail(SystemPara, extractedValue, codeQuo, -1, isPartial))  : 0;
                                    break;
                                }
                            }
                            if (n > 1)
                            {
                                int rowtemp = row;
                                for (int i = 1; i < n; i++)
                                {
                                    row++;
                                    rowCount++;
                                    worksheet.InsertRow(row, 1);
                                    worksheet.Cells[rowtemp, 1, rowtemp, colCount].Copy(worksheet.Cells[row, 1, row, colCount]);
                                    worksheet.Cells[rowtemp, 1, rowtemp, colCount].CopyStyles(worksheet.Cells[row, 1, row, colCount]);
                                    for (int col = 1; col <= colCount; col++)
                                    {
                                        ExcelRangeBase cell = worksheet.Cells[row, col];
                                        string cellValue = cell.Text;
                                        Match match = Regex.Match(cellValue, @"##(.*?)##");

                                        if (match.Success)
                                        {
                                            string extractedValue = match.Groups[1].Value;
                                            ExcelRangeBase newcell = worksheet.Cells[row, col];
                                            var txtxt = getValueInQuotaDetail(SystemPara, extractedValue, codeQuo, i, isPartial);
                                            newcell.Value = txtxt;
                                        }

                                    }
                                }
                                for (int col = 1; col <= colCount; col++)
                                {
                                    ExcelRangeBase cell = worksheet.Cells[rowtemp, col];
                                    string cellValue = cell.Text;

                                    Match match = Regex.Match(cellValue, @"##(.*?)##");

                                    if (match.Success)
                                    {
                                        string extractedValue = match.Groups[1].Value;
                                        var txtxt = getValueInQuotaDetail(SystemPara, extractedValue, codeQuo, 0, isPartial);
                                        cellValue = Regex.Replace(cellValue, @"##(.*?)##", txtxt);
                                        cell.Value = cellValue;
                                    }
                                }
                            }
                            else
                            {
                                for (int col = 1; col <= colCount; col++)
                                {
                                    ExcelRangeBase cell = worksheet.Cells[row, col];
                                    string cellValue = cell.Text;

                                    Match match = Regex.Match(cellValue, @"##(.*?)##");

                                    if (match.Success)
                                    {
                                        string extractedValue = match.Groups[1].Value;

                                        var txtxt = getValueInQuotaDetail(SystemPara, extractedValue, codeQuo, 0, isPartial);
                                        cellValue = Regex.Replace(cellValue, @"##(.*?)##", txtxt);
                                        cell.Value = cellValue;
                                    }

                                }
                            }

                        }

                        row++;
                    }
                    string fileNamedd = $"{fileName.Substring(0, fileName.LastIndexOf('.'))}_{codeQuo}.{fileName.Substring(fileName.LastIndexOf('.') + 1)}";
                    string filePaths = Path.Combine(Server.MapPath("~/Temp"), fileNamedd);
                    package.SaveAs(filePaths);
                    var data = new
                    {
                        fileName = fileNamedd
                    };           
                    return Json(data, JsonRequestBehavior.AllowGet);
                }
                return null;
            }
        }

        [HttpPost]
        public ActionResult ConvertPDF(string fileName, string codeQuo, int isPartial)
        {
            var printResult = PrintFileInTL(fileName, codeQuo, isPartial) as JsonResult;
            var printData = printResult.Data as dynamic;
            var fileNamedd = printData.fileName;
            string filePath = Path.Combine(Server.MapPath("~/Temp"), fileNamedd);
            SpreadsheetInfo.SetLicense("FREE-LIMITED-KEY");

            string pdfFile = Path.ChangeExtension(filePath, ".pdf");
            var workbook = ExcelFile.Load(filePath);
            workbook.Save(pdfFile);

            byte[] fileBytes = System.IO.File.ReadAllBytes(pdfFile);
            string pdfFileName = Path.GetFileName(pdfFile);

            RemoveFile(pdfFileName);

            return Json(new { FileData = fileBytes, FileName = pdfFileName });
        }
        [HttpPost]
        public ActionResult RemoveFile(string fileName)
        {
            try
            {
                string filePath = Path.Combine(Server.MapPath("~/Temp"), fileName);
                string pdfxlsx = Path.ChangeExtension(filePath, ".xlsx");
                System.IO.File.Delete(pdfxlsx);
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
                return Json(new { Success = true});
            }
            catch(Exception ex)
            {
                return Json(new { Success = false});
            }
            
        }
        [HttpPost]
        public ActionResult CheckFileExists(string fileName)
        {
            string filePath = Path.Combine(Server.MapPath("~/Temp"), fileName);
            if (System.IO.File.Exists(filePath))
            {
                return Json(new { Success = true });
            } else
            {
                return Json(new { Success = false });
            }
        }
        [HttpPost]
        public ActionResult UpdateLeadLogDownloadFile(int leadId, int isPartial)
        {
            try
            {
                SqlHelper.ExecuteSP("InsertLeadLogs", new
                {
                    @LeadId = leadId,
                    @Action = "OTHER",
                    @Logs = "In báo giá",
                    @isImportant = 0,
                    @IdAction = -1,
                    @UserId = WebSecurity.CurrentUserId,
                    @TypeLead = isPartial == 1 ? 0 : 1
                }) ;
                return Json(new { Success = true });
            }
            catch(Exception ex)
            {
                return Json(new { Success = false });
            }
        }
        private bool checkIsList(List<SystemParametersViewModel> sysPra, string pra)
        {
            return sysPra.FirstOrDefault(x => x.Name == pra)?.Note == "ListQuotation";
        }
        private string getValueInQuota(List<SystemParametersViewModel> sysPra, string pra, string codeQ, int isPartial)
        {
            var storedprocedure = sysPra.FirstOrDefault(x => x.Name == pra)?.Storedprocedure;
            var fiedName = sysPra.FirstOrDefault(x => x.Name == pra)?.FiedName;
            var helpersName = sysPra.FirstOrDefault(x => x.Name == pra)?.CSharp;
            if (helpersName != null)
            {
                if (storedprocedure != null)
                {
                    var result = SqlHelper.QuerySP(storedprocedure, new
                    {
                        @pColumnName = fiedName,
                        @pCode = codeQ,
                        @isPartial = isPartial
                    });

                    var value = result.FirstOrDefault()?.ValueByName?.ToString();
                    if (string.IsNullOrWhiteSpace(value))
                    {
                        return "";
                    }
                    else
                    {
                        var method = typeof(ConvertInBG).GetMethod(helpersName);
                        if (method != null)
                        {
                            if (helpersName == "ConvertToTXT")
                            {
                                if (value.EndsWith(".0"))
                                {
                                    value = value.Substring(0, value.Length - 2);
                                }
                                decimal numericValue;
                                if (decimal.TryParse(value, out numericValue))
                                {
                                    string vals = (string)method.Invoke(null, new object[] { numericValue });
                                    return vals;
                                }
                                else
                                {
                                    return "";
                                }
                            }
                            else
                            {
                                return "";
                            }

                        }
                        else
                        {
                            return "";
                        }
                    }
                }
                else
                {
                    return "";
                }

            }
            else
            if (storedprocedure != null)
            {
                var result = SqlHelper.QuerySP(storedprocedure, new
                {
                    @pColumnName = fiedName,
                    @pCode = codeQ,
                    @isPartial = isPartial
                });

                var value = result.FirstOrDefault()?.ValueByName?.ToString();
                if (string.IsNullOrWhiteSpace(value))
                {
                    return "";
                }
                else
                {
                    if (pra.Contains("Price") || pra.Contains("Amount"))
                    {
                        if (value.EndsWith(".0"))
                        {
                            value = value.Substring(0, value.Length - 2);
                        }
                        decimal numericValue;
                        if (decimal.TryParse(value, out numericValue))
                        {
                            return FormatNumber(numericValue);
                        }
                    }
                    return value;
                }
            }
            else
            {
                return "";
            }
        }

        private string getValueInQuotaDetail(List<SystemParametersViewModel> sysPra, string pra, string codeQ, int stt, int isPartial)
        {
            var storedprocedure = sysPra.FirstOrDefault(x => x.Name == pra)?.Storedprocedure;
            var fiedName = sysPra.FirstOrDefault(x => x.Name == pra)?.FiedName;

            if (storedprocedure != null)
            {
                var result = SqlHelper.QuerySP(storedprocedure, new
                {
                    @pColumnName = fiedName,
                    @pCode = codeQ,
                    @isPartial = isPartial
                }).ToArray();
                if (stt == -1)
                {
                    var value = result.Count();
                    if (value == 0)
                    {
                        return "";
                    }
                    else
                    {
                        return value.ToString();
                    }
                }
                else
                {
                    var value = result[stt]?.ValueByName?.ToString();
                    if (string.IsNullOrWhiteSpace(value))
                    {
                        return "";
                    }
                    else
                    {
                        if (pra.Contains("Price") || pra.Contains("Amount"))
                        {
                            if (value.EndsWith(".0"))
                            {
                                value = value.Substring(0, value.Length - 2);
                            }
                            decimal numericValue;
                            if (decimal.TryParse(value, out numericValue))
                            {
                                return FormatNumber(numericValue);
                            }
                        }
                        return value;
                    }
                }
            }
            else
            {
                return "";
            }
        }
        private string FormatNumber(decimal value)
        {
            return value.ToString("N0");
        }

    }
}
