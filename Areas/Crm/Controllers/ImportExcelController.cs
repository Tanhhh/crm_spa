using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.Spreadsheet;
using Erp.BackOffice.Crm.Models;
using Erp.BackOffice.Sale.Controllers;
using Erp.BackOffice.Sale.Models;
using Erp.Domain.Account.Entities;
using Erp.Domain.Account.Helper;
using Erp.Domain.Account.Interfaces;
using Erp.Domain.Entities;
using Erp.Domain.Interfaces;
using Erp.Domain.Repositories;
using Microsoft.Office.Core;
using Newtonsoft.Json;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.IO;
using System.IO.Packaging;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Xml;
using WebMatrix.WebData;
using static DotNetOpenAuth.OpenId.Extensions.AttributeExchange.WellKnownAttributes.Contact;
using Erp.BackOffice.Filters;

namespace Erp.BackOffice.Crm.Controllers
{
	public class MyData
	{
		public Dictionary<string, List<object>> ColumnData { get; set; }
		public Dictionary<string, string> ColumnHeaders { get; set; }
	}
	public class ImportExcelController : Controller
	{
		private static MyData dataExcel;
		private static int firstRow;
		private static int lastRow;
		private static List<ImportExcelMaped> dataListEx;
		private readonly IUserRepository userRepository;
		private readonly ICustomerRepository customerRepository;
		public ImportExcelController(IUserRepository _user, ICustomerRepository _customer)
		{
			userRepository = _user;
			customerRepository = _customer;
		}
		public ViewResult SetupImport(int? ldleadview=null)
		{
			var dataNameLabel = SqlHelper.QuerySP<ImportExcelNameLabelViewModel>("spCrm_ImportExcel_Select_Name").ToList();
			ViewBag.dataNameLabel = dataNameLabel;

			var displayNames = new List<string>();
			var properties = typeof(LeadModel).GetProperties();
			string[] dont = new string[] { "IsDeleted", "CreatedDate", "CreatedUserId", "ModifiedDate", "ModifiedUserId", "AssignedUserId", "CustomerCode" };
			var prop = properties.Where(x => x.Name != "Id" && !dont.Contains(x.Name));
			foreach (var item1 in prop)
			{
				var DisplayName = item1.Name;
				displayNames.Add(DisplayName);
			}
			ViewBag.datadDsplayName = displayNames;
			ViewBag.ldleadview = ldleadview;
			//
			var listTemplateName = SqlHelper.QuerySP<string>("spCrm_MapTemplateExcel_Select_TempalteName").ToList();
			ViewBag.listTemplateName = listTemplateName;
			return View();
		}
		[HttpPost]
		[AllowAnonymous]
		public ActionResult ImportData(HttpPostedFileBase pexcelFile, string psheetname, int pfirstRow, int plastRow)
		{
			try
			{

				if (pexcelFile == null || pexcelFile.ContentLength == 0)
				{
					return RedirectToAction("Index");
				}
				firstRow = pfirstRow;
				lastRow = plastRow;
				// Đường dẫn lưu trữ tạm thời của tệp Excel
				var filePath = Path.Combine(Server.MapPath("~/Temp"), pexcelFile.FileName);
				pexcelFile.SaveAs(filePath);
				ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
				using (var package = new ExcelPackage(new FileInfo(filePath)))
				{
					var worksheet = package.Workbook.Worksheets.FirstOrDefault(sheet => sheet.Name == psheetname);
					if (worksheet != null)
					{
						var columnData = new Dictionary<string, List<object>>();
						var columnHeaders = new Dictionary<string, string>();
						var dataRange = worksheet.Dimension;
						int rows = dataRange.Rows;
						int columns = dataRange.Columns;

						for (int col = 1; col <= columns; col++)
						{
							string columnName = GetColumnName(col);
							columnHeaders[columnName] = worksheet.Cells[1, col].Value?.ToString();
							Type columnType = worksheet.Cells[2, col].Value?.GetType();
							if (pfirstRow == 1)
							{
								pfirstRow++;
							}
							if (columnType == typeof(bool))
							{
								var boolData = new List<bool>();
								for (int row = pfirstRow; row <= plastRow; row++)
								{
									var cellValue = worksheet.Cells[row, col].Value;
									if (cellValue != null && cellValue is bool boolValue)
									{
										boolData.Add(boolValue);
									}
									else
									{
										boolData.Add(false);
									}
								}
								columnData[columnName] = boolData.Cast<object>().ToList();
							}
							else if (columnType == typeof(DateTime))
							{
								var dateTimeData = new List<DateTime>();
								for (int row = pfirstRow; row <= plastRow; row++)
								{
									var cellValue = worksheet.Cells[row, col].Value;
									if (cellValue != null && cellValue is DateTime dateTimeValue)
									{
										dateTimeData.Add(dateTimeValue);
									}
									else
									{
										dateTimeData.Add(DateTime.MinValue);
									}
								}
								columnData[columnName] = dateTimeData.Cast<object>().ToList();
							}
							else
							{
								var stringData = new List<string>();
								for (int row = pfirstRow; row <= plastRow; row++)
								{
									var rs = worksheet.Cells[row, col].Value?.ToString();
                                    if (!string.IsNullOrEmpty(rs) && rs.Length <= 10)
									{
                                        var checkphone = FormatNumber.AdjustPhoneNumber(rs);
										if(checkphone != null)
										{
                                            stringData.Add(checkphone);
                                        } else
										{
                                            stringData.Add(rs);
                                        }
                                    } else
									{
                                        stringData.Add(rs);
                                    }
									
								}
								columnData[columnName] = stringData.Cast<object>().ToList();
							}
						}
						System.IO.File.Delete(filePath);

						dataExcel = new MyData
						{
							ColumnData = columnData,
							ColumnHeaders = columnHeaders
						};


						//return Json(result, JsonRequestBehavior.AllowGet);

						var resultData = dataExcel;
						var jsonResult = Json(resultData, JsonRequestBehavior.AllowGet);
						jsonResult.MaxJsonLength = int.MaxValue;
						return jsonResult;

					}
					else
					{
                        System.IO.File.Delete(filePath);
                    }
				}
				return Json(false);
			}
			catch (Exception ex)
			{
				Erp.BackOffice.Helpers.Common.SendErrorToText("vao day 4" + ex.Message);
				throw;
			}
		}
		[System.Web.Mvc.HttpPost]
		public ActionResult ExcuteData(string data, string datacheck, int? ldleadview=null )
		{
			dataListEx = JsonConvert.DeserializeObject<List<ImportExcelMaped>>(data);
			var dataListCheck = JsonConvert.DeserializeObject<List<ImportExeclCheck>>(datacheck);
			try
			{
				List<CheckTrung> checktrunglist = new List<CheckTrung>();
				List<object> firstColumnValues = dataExcel.ColumnData.Values.FirstOrDefault();
				int numberOfValues = firstColumnValues?.Count ?? 0;
				checktrunglist = CheckDataTrung(dataListCheck, numberOfValues);
				for (int i = 0; i < numberOfValues; i++)
				{

					var isExistall = checktrunglist.Count()>0 ? checktrunglist.Any(x => x.Value == i) : false;
                    if (!isExistall) {
						int? cusid = null;

						if (ldleadview == -1)
						{
							var keyVal = dataListEx.Where(x => x.ColumnInTable == "Mobile").FirstOrDefault()?.ColumnInFile;
							var dsval = dataExcel.ColumnData.Where(x => x.Key == keyVal).FirstOrDefault().Value;
							string phone = dsval?[i].ToString();

							if (!string.IsNullOrEmpty(phone))
							{
								var codekh = SqlHelper.QuerySP<Customer>("GetCusByPhone", new { @pPhone = phone }).FirstOrDefault();
								if (codekh != null)
								{
									var cus = customerRepository.GetvwCustomerByCode(codekh.Code);
									cusid = cus.Id;
								}
								else
									continue;
							}
							else
								continue;
						}
						var dataResult = SqlHelper.ExecuteSP("spCrm_Lead_Insert_ImportExcel", new
						{
							@pSource = GetDataFile("Source", i),
							@pTypeData = GetDataFile("TypeData", i),
							@pTopic = GetDataFile("Topic", i),
							@pLeadName = GetDataFile("LeadName", i),
							@pName = GetDataFile("Name", i),
							@pMobile = GetDataFile("Mobile", i),
							@pEmail = GetDataFile("Email", i),
							@pAddress = GetDataFile("Address", i),
							@pCodeKC = GetDataFile("CodeKC", i),
							@pYearofBirth = GetDataFile("YearofBirth", i),
							@pReceptionStaffId = GetDataFile("ReceptionStaffId", i),
							@pIsCancel = GetDataFile("IsCancel", i),
							@pCompanyName = GetDataFile("CompanyName", i),
							@pMobileCompany = GetDataFile("MobileCompany", i),
							@pEmailCompany = GetDataFile("EmailCompany", i),
							@pTaxcode = GetDataFile("Taxcode", i),
							@pStatusId = GetDataFile("StatusId", i),
							@pCustomerId = cusid,
							@pF1 = GetDataFile("F1", i),
							@pF2 = GetDataFile("F2", i),
							@pF3 = GetDataFile("F3", i),
							@pF4 = GetDataFile("F4", i),
							@pF5 = GetDataFile("F5", i),
							@pF6 = GetDataFile("F6", i),
							@pF7 = GetDataFile("F7", i),
							@pF8 = GetDataFile("F8", i),
							@pF9 = GetDataFile("F9", i),
							@pF10 = GetDataFile("F10", i),
							@pF11 = GetDataFile("F11", i),
							@pF12 = GetDataFile("F12", i),
							@pF13 = GetDataFile("F13", i),
							@pF14 = GetDataFile("F14", i),
							@pF15 = GetDataFile("F15", i),
							@pF16 = GetDataFile("F16", i),
							@pF17 = GetDataFile("F17", i),
							@pF18 = GetDataFile("F18", i),
							@pF19 = GetDataFile("F19", i),
							@pF20 = GetDataFile("F20", i),
							@pF21 = GetDataFile("F21", i),
							@pF22 = GetDataFile("F22", i),
							@pF23 = GetDataFile("F23", i),
							@pF24 = GetDataFile("F24", i),
							@pF25 = GetDataFile("F25", i),
							@pF26 = GetDataFile("F26", i),
							@pF27 = GetDataFile("F27", i),
							@pF28 = GetDataFile("F28", i),
							@pF29 = GetDataFile("F29", i),
							@pF30 = GetDataFile("F30", i),
							@pF31 = GetDataFile("F31", i),
							@pF32 = GetDataFile("F32", i),
							@pF33 = GetDataFile("F33", i),
							@pF34 = GetDataFile("F34", i),
							@pF35 = GetDataFile("F35", i),
							@pF36 = GetDataFile("F36", i),
							@pF37 = GetDataFile("F37", i),
							@pF38 = GetDataFile("F38", i),
							@pF39 = GetDataFile("F39", i),
							@pF40 = GetDataFile("F40", i),
							@pF41 = GetDataFile("F41", i),
							@pF42 = GetDataFile("F42", i),
							@pF43 = GetDataFile("F43", i),
							@pF44 = GetDataFile("F44", i),
							@pF45 = GetDataFile("F45", i),
							@pF46 = GetDataFile("F46", i),
							@pF47 = GetDataFile("F47", i),
							@pF48 = GetDataFile("F48", i),
							@pF49 = GetDataFile("F49", i),
							@pF50 = GetDataFile("F50", i),
							@pF51 = GetDataFile("F51", i),
							@pF52 = GetDataFile("F52", i),
							@pF53 = GetDataFile("F53", i),
							@pF54 = GetDataFile("F54", i),
							@pF55 = GetDataFile("F55", i),
							@pF56 = GetDataFile("F56", i),
							@pF57 = GetDataFile("F57", i),
							@pF58 = GetDataFile("F58", i),
							@pF59 = GetDataFile("F59", i),
							@pF60 = GetDataFile("F60", i),
							@pF61 = GetDataFile("F61", i),
							@pF62 = GetDataFile("F62", i),
							@pF63 = GetDataFile("F63", i),
							@pF64 = GetDataFile("F64", i),
							@pF65 = GetDataFile("F65", i),
							@pF66 = GetDataFile("F66", i),
							@pF67 = GetDataFile("F67", i),
							@pF68 = GetDataFile("F68", i),
							@pF69 = GetDataFile("F69", i),
							@pF70 = GetDataFile("F70", i),
							@pF71 = GetDataFile("F71", i),
							@pF72 = GetDataFile("F72", i),
							@pF73 = GetDataFile("F73", i),
							@pF74 = GetDataFile("F74", i),
							@pF75 = GetDataFile("F75", i),
							@pF76 = GetDataFile("F76", i),
							@pF77 = GetDataFile("F77", i),
							@pF78 = GetDataFile("F78", i),
							@pF79 = GetDataFile("F79", i),
							@pF80 = GetDataFile("F80", i),
							@pF81 = GetDataFile("F81", i),
							@pF82 = GetDataFile("F82", i),
							@pF83 = GetDataFile("F83", i),
							@pF84 = GetDataFile("F84", i),
							@pF85 = GetDataFile("F85", i),
							@pF86 = GetDataFile("F86", i),
							@pF87 = GetDataFile("F87", i),
							@pF88 = GetDataFile("F88", i),
							@pF89 = GetDataFile("F89", i),
							@pF90 = GetDataFile("F90", i),
							@pF91 = GetDataFile("F91", i),
							@pF92 = GetDataFile("F92", i),
							@pF93 = GetDataFile("F93", i),
							@pF94 = GetDataFile("F94", i),
							@pF95 = GetDataFile("F95", i),
							@pF96 = GetDataFile("F96", i),
							@pF97 = GetDataFile("F97", i),
							@pF98 = GetDataFile("F98", i),
							@pF99 = GetDataFile("F99", i),
							@pCreatedUserId = WebSecurity.CurrentUserId
						});
						
					}
				}
				var fileName = "";
				if (checktrunglist.Count() > 0)
                {
					//checktrunglist = MergeKey(checktrunglist, numberOfValues);
					fileName = ExportExcelTrung(checktrunglist, numberOfValues);
				}
				var slitem = numberOfValues - checktrunglist.Count();
				return Json(new { Success = true , FileName = fileName ,SLItem = slitem });
			}
			catch (Exception e)
			{
				return Json(new { Success = false });
			}


		}
		private string ExportExcelTrung(List<CheckTrung> checkTrungs, int rowCOunt)
		{
			Dictionary<string, List<object>> ColumnData = dataExcel.ColumnData;
			Dictionary<string, string> ColumnHeader = dataExcel.ColumnHeaders;
			ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
			using (var package = new ExcelPackage())
			{
				// Tạo một worksheet mới
				var worksheet = package.Workbook.Worksheets.Add("Sheet1");

				// Đặt tên các cột
				int stt = 1;
				var colCount = ColumnHeader.Count();
				worksheet.Cells[1, 1].Value = "Loại trùng";
				foreach (var column in ColumnHeader)
				{
					stt++;
					worksheet.Cells[1, stt].Value = column.Value == null ? "" : column.Value;
				}
				
				stt = 0;
				int sttnew = 2;
				for (int i =0; i< rowCOunt; i++)
                {
					var isExist = checkTrungs.FirstOrDefault(x => x.Value == i);
					if (isExist == null)
					{
						stt++;
						continue;		
					}
					else
					{
						worksheet.Cells[sttnew, 1].Value = isExist.Key;
						for (int j = 2; j < colCount + 2; j++)
						{
							string key = ColumnHeader.Keys.ElementAt(j - 2);
							worksheet.Cells[sttnew, j].Value = ColumnData[key][stt] == null ? "" : ColumnData[key][stt];
						}
						stt++;
						sttnew++;
					}
				}
				// Thiết lập định dạng cột
				for (int columnIndex = 1; columnIndex <= worksheet.Dimension.Columns; columnIndex++)
				{
					worksheet.Column(columnIndex).AutoFit();
				}
				string fileName = "ContentDuplicate.xlsx";
				string filePath = Path.Combine(Server.MapPath("~/Temp"), fileName);

				if (System.IO.File.Exists(filePath))
				{
					System.IO.File.Delete(filePath);
				}
				package.SaveAs(filePath);
				return fileName;
			}
		}
		[System.Web.Mvc.HttpPost]
		public ActionResult SaveTemplateMap(string nameTem)
		{
			try
			{
				foreach (var datatem in dataListEx)
				{
					var dataResult = SqlHelper.ExecuteSP("spCrm_MapTemplateExcel_Insert", new
					{
						@pFieldId = datatem.fieldID,
						@pFieldExcel = datatem.ColumnInFile,
						@pTempalteName = nameTem,
						@pCreatedUserId = WebSecurity.CurrentUserId
					});
				}
				return Json(new { Success = true });
			}
			catch (Exception e)
			{
				return Json(new { Success = false });
			}
		}
		[System.Web.Mvc.HttpPost]
		public ActionResult GetTemplateMap(string nameTem)
		{
			var dataResult = SqlHelper.QuerySP<ImportExcelTemplate>("spCrm_MapTemplateExcel_Select_Field", new
			{
				@pTempalteName = nameTem
			});
			return Json(dataResult, JsonRequestBehavior.AllowGet);
		}
		private string GetDataFile(string para, int stRow)
		{
			var dataRs = dataListEx.FirstOrDefault(x => x.ColumnInTable == para)?.ColumnInFile;
			Dictionary<string, List<object>> ColumnData = dataExcel.ColumnData;

			if (dataRs != null)
			{
				if (ColumnData[dataRs][stRow] != null)
				{
					object value = ColumnData[dataRs][stRow];
					Type valueType = value.GetType();

					if (valueType == typeof(string))
					{
						string stringValue = (string)value;
						return stringValue;
					}
					else if (valueType == typeof(DateTime))
					{
						DateTime dateTimeValue = (DateTime)value;
						return dateTimeValue.ToString();
					}
					else if (valueType == typeof(int))
					{
						int intValue = (int)value;
						return intValue.ToString();
					}
					else if (valueType == typeof(bool))
					{
						bool boolValue = (bool)value;
						return boolValue.ToString();
					}
				}
			}
			return null;
		}
		private List<CheckTrung> CheckDataTrung(List<ImportExeclCheck> listcheck, int stRowCount)
		{
			Dictionary<string, List<object>> ColumnData = dataExcel.ColumnData;
			List<CheckTrung> rt = new List<CheckTrung>();
			var countCheck = listcheck.Count(x => x.Value == true);
			for (int i= 0; i< stRowCount; i++)
            {
				var colLeadName = listcheck.FirstOrDefault(x=>x.Key == "LeadName").Value == true ? dataListEx.FirstOrDefault(x => x.ColumnInTable == "LeadName")?.ColumnInFile : null;
				var colMobile = listcheck.FirstOrDefault(x => x.Key == "Mobile").Value == true ? dataListEx.FirstOrDefault(x => x.ColumnInTable == "Mobile")?.ColumnInFile : null;
				var colTaxcode = listcheck.FirstOrDefault(x => x.Key == "Taxcode").Value == true ? dataListEx.FirstOrDefault(x => x.ColumnInTable == "Taxcode")?.ColumnInFile : null;
				var colEmail = listcheck.FirstOrDefault(x => x.Key == "Email").Value == true ? dataListEx.FirstOrDefault(x => x.ColumnInTable == "Email")?.ColumnInFile : null;

				var valueLeadName = colLeadName != null ? ColumnData[colLeadName][i].ToString() : null;
				var valueMobile = colMobile != null ? ColumnData[colMobile][i].ToString() : null;
				var valueTaxcode = colTaxcode != null ? ColumnData[colTaxcode][i].ToString() : null;
				var valueEmail = colEmail != null ? ColumnData[colEmail][i].ToString() : null;

				var dataResult = SqlHelper.QuerySP("sp_Crm_ImportExcel_Check_Trung", new
				{
					@pValueLeadName = valueLeadName,
					@pValueMobile = valueMobile,
					@pValueTaxcode = valueTaxcode,
					@pValueEmail = valueEmail
				});
				if (dataResult.Count() == countCheck)
				{
					var keytemp = "";
					foreach (var item1 in dataResult)
                    {
						keytemp += item1.ColumnName + ",";
					}
					if (keytemp != "")
					{
						rt.Add(new CheckTrung
						{
							Key = keytemp.Substring(0, keytemp.Length - 1),
							Value = i
						});
					}
				}
			}
			var sortedrt = rt.OrderBy(ct => ct.Value).ToList();
			return sortedrt;
		}
		private string GetColumnName(int columnNumber)
		{
			int dividend = columnNumber;
			string columnName = string.Empty;

			while (dividend > 0)
			{
				int modulo = (dividend - 1) % 26;
				columnName = Convert.ToChar(65 + modulo) + columnName;
				dividend = (dividend - modulo) / 26;
			}

			return columnName;
		}
		[System.Web.Http.HttpGet]
		public ActionResult ExportExIndex(string listid)
		{
			var user = userRepository.GetAllUsers();

			var lsstatus = SqlHelper.QuerySP<StatusLeadModel>("GetAllStatus");
			List<string> idList = new List<string>();
			if (!string.IsNullOrEmpty(listid))
			{
				string[] idArray = listid.Split(',');
				idList = idArray.ToList();
			}

			int number;
			var section = SqlHelper.QueryMultipleSP<LeadSectionModel, LeadSection_FieldModel>("AddNewModal");
			var properties = typeof(LeadModel).GetProperties();
			string[] dont = new string[] { "IsDeleted", "CreatedDate", "CreatedUserId", "ModifiedDate", "ModifiedUserId", "AssignedUserId" };
			var prop = properties.Where(x => x.Name != "Id" && !dont.Contains(x.Name));

			List<LeadModel> modelList = new List<LeadModel>();

			for (int i = 0; i < idList.Count; i++)
			{
				var model = SqlHelper.QuerySP<LeadModel>("LoadLeadById", new
				{
					@pId = idList[i]
				}).ToList();

				if (model.Count > 0)
				{
					modelList.Add(model[0]);
				}
			}
			if (modelList.Count > 0)
			{
				ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
				using (var package = new ExcelPackage())
				{
					// Tạo một worksheet mới
					var worksheet = package.Workbook.Worksheets.Add("Content-Lead");

					// Đặt tên các cột
					int stt = 1;
					foreach (var item in prop)
					{
						if (item.Name.StartsWith("F") && int.TryParse(item.Name.Substring(1), out number))
						{
							var itemField = section.Item2.FirstOrDefault(x => x.FieldName == item.Name);
							if (itemField != null && itemField.IsHiden != true)
							{
								worksheet.Cells[1, stt].Value = itemField.NameLabel;

								for (int i = 0; i < modelList.Count; i++)
								{
									var nameiss = item.Name;
									var value = modelList[i].GetType().GetProperty(item.Name)?.GetValue(modelList[i]);
									worksheet.Cells[i + 2, stt].Value = value;
								}

								stt++;
							}
						}
						else
						{
							var display = item.GetCustomAttributes(typeof(DisplayNameAttribute), true).FirstOrDefault() as DisplayNameAttribute;
							string displayname = display != null ? display.DisplayName : "";
							worksheet.Cells[1, stt].Value = displayname;
							for (int i = 0; i < modelList.Count; i++)
							{
								object value = null;
								var nameiss = item.Name;
								if (nameiss == "ReceptionStaffId")
								{
									var receptionStaffIdObj = modelList[i].GetType().GetProperty(item.Name)?.GetValue(modelList[i]);
									int receptionStaffId;
									if (receptionStaffIdObj != null && int.TryParse(receptionStaffIdObj.ToString(), out receptionStaffId))
									{
										value = user.Where(x => x.Id == receptionStaffId).FirstOrDefault().FullName;
									}
								}
								else if (nameiss == "StatusId")
								{
									var StatusIdObj = modelList[i].GetType().GetProperty(item.Name)?.GetValue(modelList[i]);
									int StatusId;
									if (StatusIdObj != null && int.TryParse(StatusIdObj.ToString(), out StatusId))
									{
										value = lsstatus.Where(x => x.Id == StatusId).FirstOrDefault().Name;
									}
								}
								else
								{
									value = modelList[i].GetType().GetProperty(item.Name)?.GetValue(modelList[i]);
								}
								worksheet.Cells[i + 2, stt].Value = value;
							}
							stt++;
						}

					}
					// Thiết lập định dạng cột
					for (int columnIndex = 1; columnIndex <= worksheet.Dimension.Columns; columnIndex++)
					{
						worksheet.Column(columnIndex).AutoFit();
					}
					string date = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
					string excelName = "Lead-Content-" + date + ".xlsx";
					string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

					// Trả về FileResult để tải xuống file Excel
					return File(package.GetAsByteArray(), contentType, excelName);

				}
			}
			else
			{
				return null;
			}
		}
	}
}

