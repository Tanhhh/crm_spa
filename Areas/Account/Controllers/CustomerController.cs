using Erp.BackOffice.Filters;
using Erp.BackOffice.Helpers;
using Erp.BackOffice.Account.Models;
using Erp.Domain.Interfaces;
using Erp.Domain.Account.Entities;
using Erp.Domain.Account.Interfaces;
using Erp.Utilities;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
//using Erp.Domain.Sale.Entities;
using Erp.Domain.Sale.Interfaces;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using WebMatrix.WebData;
//using Erp.BackOffice.Sale.Controllers;
using Erp.BackOffice.Sale.Models;
using System.Drawing;
using System.IO;
//using Erp.Domain.Crm.Interfaces;
using Erp.Domain.Staff.Entities;
using Erp.Domain.Staff.Interfaces;
//using Erp.BackOffice.Staff.Models;
//using Erp.BackOffice.Staff.Controllers;
//using Erp.Domain.Entities;
//using Erp.Domain.Interfaces;
//using System.Net.Http;
//using System.Net.Http.Headers;
//using Newtonsoft.Json.Linq;
using System.Transactions;
using Erp.Domain.Account.Helper;
using System.Web;
using Erp.Domain.Crm.Interfaces;
using Erp.Domain.Crm.Repositories;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
using Erp.Domain.Repositories;
using System.Diagnostics;
using DocumentFormat.OpenXml.Office2010.Excel;

namespace Erp.BackOffice.Account.Controllers
{
	[Authorize]
	[InitializeSimpleMembership]
	[Erp.BackOffice.Helpers.NoCacheHelper]
	public class CustomerController : Controller
	{
		private readonly ICustomerRepository CustomerRepository;
		private readonly ICustomerUserRepository CustomerUserRepository;
		private readonly IContactRepository ContactRepository;
		private readonly IUserRepository userRepository;
		private readonly IProductInvoiceRepository productInvoiceRepository;
		private readonly ILogVipRepository logVipRepository;
		private readonly ILocationRepository locationRepository;
		private readonly IUsingServiceLogRepository usingServiceLogRepository;
		private readonly ITransactionLiabilitiesRepository transactionLiabilitiesRepository;
		private readonly IDocumentFieldRepository DocumentFieldRepository;
		private readonly IDocumentAttributeRepository DocumentAttributeRepository;
		private readonly IUsingServiceLogDetailRepository usingServiceLogdetailRepository;
		private readonly IBranchRepository branchRepository;
		private readonly IUserTypeRepository user_typeRepository;
		private readonly ITemplatePrintRepository templatePrintRepository;
		private readonly IUserType_kdRepository _userType_kdRepository;
		private readonly IKH_TUONGTACRepository iKH_TUONGTACRepository;

		bool isGetAPI = false;
		static CustomerViewModel cus_crm_model = null;
		public CustomerController(
			ICustomerRepository _Customer
			, ICustomerUserRepository _CustomerUser
			, IContactRepository _Contact
			, IUserRepository _user
			, IProductInvoiceRepository productInvoice
			, ILogVipRepository logVip
			, ILocationRepository location
			, IUsingServiceLogRepository usingServiceLog
			, ITransactionLiabilitiesRepository _transactionLiabilities
			 , IDocumentFieldRepository _DocumentField
			, IDocumentAttributeRepository DocumentAttribute
			, IUsingServiceLogDetailRepository usingServiceLogDetail
						, IBranchRepository branch
					   , IUserTypeRepository user_type
			, ITemplatePrintRepository templatePrint
			, IUserType_kdRepository userTypeKD
			, IKH_TUONGTACRepository kH_TUONGTAC
			)
		{
			ContactRepository = _Contact;
			CustomerRepository = _Customer;
			CustomerUserRepository = _CustomerUser;
			userRepository = _user;
			productInvoiceRepository = productInvoice;
			logVipRepository = logVip;
			locationRepository = location;
			usingServiceLogRepository = usingServiceLog;
			transactionLiabilitiesRepository = _transactionLiabilities;
			DocumentFieldRepository = _DocumentField;
			DocumentAttributeRepository = DocumentAttribute;
			usingServiceLogdetailRepository = usingServiceLogDetail;
			branchRepository = branch;
			user_typeRepository = user_type;
			templatePrintRepository = templatePrint;
			_userType_kdRepository = userTypeKD;
			iKH_TUONGTACRepository = kH_TUONGTAC;
		}

        #region Index

        public ViewResult Index(string startDate, string endDate, string CardCode, string txtCusInfo, string Phone, string ProvinceId, string DistrictId, string WardId, string pLOAIKH, string nTHEODOI, int? SalerId, string NguonKhach, int? pageSize)
        {

			int totalCustomers;
            pLOAIKH = string.IsNullOrEmpty(pLOAIKH) ? "ISLEAD" : pLOAIKH;
			NguonKhach = string.IsNullOrEmpty(NguonKhach) ? "" : NguonKhach;
			//get cookie brachID 
			HttpRequestBase request = this.HttpContext.Request;
			string strBrandID = "0";
			if (request.Cookies["BRANCH_ID_SPA_CookieName"] != null)
			{
				strBrandID = request.Cookies["BRANCH_ID_SPA_CookieName"].Value;
				if (strBrandID == "")
				{
					strBrandID = "0";
				}
			}

			//get  CurrentUser.branchId

			if ((Helpers.Common.CurrentUser.BranchId != null) && (Helpers.Common.CurrentUser.BranchId > 0))
			{
				strBrandID = Helpers.Common.CurrentUser.BranchId.ToString();
			}

			int? intBrandID = int.Parse(strBrandID);
			DateTime d_startDate, d_endDate;
			bool whdate = false;
			bool a = DateTime.TryParseExact(startDate, "dd/MM/yyyy", new CultureInfo("vi-VN"), DateTimeStyles.None, out d_startDate);
			bool b = DateTime.TryParseExact(endDate, "dd/MM/yyyy", new CultureInfo("vi-VN"), DateTimeStyles.None, out d_endDate);
			if (a && b)
			{
				d_endDate = d_endDate.AddHours(23).AddMinutes(59);
				whdate = true;
			}


			if (pLOAIKH == "TATCA" || pLOAIKH == "ISLEAD")
			{
				IEnumerable<CustomerViewModel> q = SqlHelper.QuerySP<CustomerViewModel>("Get_AllCustomer", new
				{
					isLead = pLOAIKH == "TATCA" ? (int?)null : 1,
					isLock = nTHEODOI == "TDOI" ? 1 : 0,
					BranchId = intBrandID,
					tenNguonkhach = NguonKhach,
					startDate = whdate ? d_startDate : (DateTime?)null,
					endDate = whdate ? d_endDate : (DateTime?)null,
					txtCusInfo = txtCusInfo,
					Phone = Phone,
					ProvinceId = ProvinceId,
					DistrictId = DistrictId,
					WardId = WardId,
					SalerId = SalerId
				});
                totalCustomers = q.Count();
                ViewBag.TotalCustomers = totalCustomers;
                ViewBag.PageSize = pageSize;

                return View(q);
            }
            else
			{

				IEnumerable<CustomerViewModel> q = SqlHelper.QuerySP<CustomerViewModel>("spGetAllCustomerByType", new
				{
					islock = nTHEODOI == "TDOI" ? 1 : 0,
					BranchId = intBrandID,
					loaiKH = pLOAIKH == "KHCU" ? 0 : pLOAIKH == "KHMOI" ? 1 : 2,
					tenNguonkhach = NguonKhach,
					startDate = whdate ? d_startDate : (DateTime?)null,
					endDate = whdate ? d_endDate : (DateTime?)null,
					txtCusInfo = txtCusInfo,
					Phone = Phone,
					ProvinceId = ProvinceId,
					DistrictId = DistrictId,
					WardId = WardId,
					SalerId = SalerId
				});
                totalCustomers = q.Count();
                ViewBag.TotalCustomers = totalCustomers;
                ViewBag.PageSize = pageSize;

                return View(q);
            }
        }
		#region Xuất excel
		public ActionResult ExportExcel(string startDate, string endDate, string CardCode, string txtCusInfo, string Phone, string ProvinceId, string DistrictId, string WardId, string pLOAIKH, string nTHEODOI, int? SalerId, bool ExportExcel, string NguonKhach)
		{
			//get cookie brachID 
			HttpRequestBase request = this.HttpContext.Request;
			NguonKhach = string.IsNullOrEmpty(NguonKhach) ? "" : NguonKhach;
			string strBrandID = "0";
			if (request.Cookies["BRANCH_ID_SPA_CookieName"] != null)
			{
				strBrandID = request.Cookies["BRANCH_ID_SPA_CookieName"].Value;
				if (strBrandID == "")
				{
					strBrandID = "0";
				}
			}

			//get  CurrentUser.branchId

			if ((Helpers.Common.CurrentUser.BranchId != null) && (Helpers.Common.CurrentUser.BranchId > 0))
			{
				strBrandID = Helpers.Common.CurrentUser.BranchId.ToString();
			}

			int? intBrandID = int.Parse(strBrandID);

			var data = new List<CustomerViewModel>();

			if (pLOAIKH == "TATCA")
			{
				if (nTHEODOI == "TDOI")
				{

					IEnumerable<CustomerViewModel> q = SqlHelper.QuerySP<CustomerViewModel>("spGetAllCustomerToExport", new
					{
						islock = 1,
						BranchId = intBrandID,
						loaiKH = 3
					});
					if (NguonKhach != "")
					{
						q = q.Where(x => x.tenNguonkhach == NguonKhach).ToList();
					}
					q = FilterSearchIndex(q, startDate, endDate, CardCode, txtCusInfo, Phone, ProvinceId, DistrictId, WardId, SalerId).ToList();

					//return View(q);
					data = q.ToList();

				}
				else
				{
					IEnumerable<CustomerViewModel> q = SqlHelper.QuerySP<CustomerViewModel>("spGetAllCustomerToExport", new
					{
						islock = 0,
						BranchId = intBrandID,
						loaiKH = 3
					});
					if (NguonKhach != "")
					{
						q = q.Where(x => x.tenNguonkhach == NguonKhach).ToList();
					}
					q = FilterSearchIndex(q, startDate, endDate, CardCode, txtCusInfo, Phone, ProvinceId, DistrictId, WardId, SalerId).ToList();
					data = q.ToList();
					//return View(q);

				}
				//return View(q);
			}
			else if (pLOAIKH == "KHCU")
			{
				if (nTHEODOI == "TDOI")
				{

					IEnumerable<CustomerViewModel> q = SqlHelper.QuerySP<CustomerViewModel>("spGetAllCustomerToExport", new
					{
						islock = 1,
						BranchId = intBrandID,
						loaiKH = 0
					});
					if (NguonKhach != "")
					{
						q = q.Where(x => x.tenNguonkhach == NguonKhach).ToList();
					}
					q = FilterSearchIndex(q, startDate, endDate, CardCode, txtCusInfo, Phone, ProvinceId, DistrictId, WardId, SalerId).ToList();
					//return View(q);
					data = q.ToList();

				}
				else
				{
					IEnumerable<CustomerViewModel> q = SqlHelper.QuerySP<CustomerViewModel>("spGetAllCustomerToExport", new
					{
						islock = 0,
						BranchId = intBrandID,
						loaiKH = 0
					});
					if (NguonKhach != "")
					{
						q = q.Where(x => x.tenNguonkhach == NguonKhach).ToList();
					}
					q = FilterSearchIndex(q, startDate, endDate, CardCode, txtCusInfo, Phone, ProvinceId, DistrictId, WardId, SalerId).ToList();
					data = q.ToList();
					//return View(q);

				}
			}
			else if (pLOAIKH == "KHMOI")
			{
				if (nTHEODOI == "TDOI")
				{

					IEnumerable<CustomerViewModel> q = SqlHelper.QuerySP<CustomerViewModel>("spGetAllCustomerToExport", new
					{
						islock = 1,
						BranchId = intBrandID,
						loaiKH = 1
					});
					if (NguonKhach != "")
					{
						q = q.Where(x => x.tenNguonkhach == NguonKhach).ToList();
					}
					q = FilterSearchIndex(q, startDate, endDate, CardCode, txtCusInfo, Phone, ProvinceId, DistrictId, WardId, SalerId).ToList();
					data = q.ToList();
					//return View(q);


				}
				else
				{
					IEnumerable<CustomerViewModel> q = SqlHelper.QuerySP<CustomerViewModel>("spGetAllCustomerToExport", new
					{
						islock = 0,
						BranchId = intBrandID,
						loaiKH = 1
					});
					if (NguonKhach != "")
					{
						q = q.Where(x => x.tenNguonkhach == NguonKhach).ToList();
					}
					q = FilterSearchIndex(q, startDate, endDate, CardCode, txtCusInfo, Phone, ProvinceId, DistrictId, WardId, SalerId).ToList();
					data = q.ToList();
					//return View(q);

				}
			}
			else
			{
				if (nTHEODOI == "TDOI")
				{

					IEnumerable<CustomerViewModel> q = SqlHelper.QuerySP<CustomerViewModel>("spGetAllCustomerToExport", new
					{
						islock = 1,
						BranchId = intBrandID,
						loaiKH = 2
					});
					if (NguonKhach != "")
					{
						q = q.Where(x => x.tenNguonkhach == NguonKhach).ToList();
					}
					q = FilterSearchIndex(q, startDate, endDate, CardCode, txtCusInfo, Phone, ProvinceId, DistrictId, WardId, SalerId).ToList();
					data = q.ToList();
					//return View(q);


				}
				else
				{
					IEnumerable<CustomerViewModel> q = SqlHelper.QuerySP<CustomerViewModel>("spGetAllCustomerToExport", new
					{
						islock = 0,
						BranchId = intBrandID,
						loaiKH = 2
					});
					if (NguonKhach != "")
					{
						q = q.Where(x => x.tenNguonkhach == NguonKhach).ToList();
					}
					q = FilterSearchIndex(q, startDate, endDate, CardCode, txtCusInfo, Phone, ProvinceId, DistrictId, WardId, SalerId).ToList();
					data = q.ToList();
					//return View(q);

				}
			}
			//var data = q.ToList();
			var model = new TemplatePrintViewModel();
			var logo = Erp.BackOffice.Helpers.Common.GetSetting("LogoCompany");
			var company = Erp.BackOffice.Helpers.Common.GetSetting("companyName");
			var address = Erp.BackOffice.Helpers.Common.GetSetting("addresscompany");
			var phone = Erp.BackOffice.Helpers.Common.GetSetting("phonecompany");
			var fax = Erp.BackOffice.Helpers.Common.GetSetting("faxcompany");
			var bankcode = Erp.BackOffice.Helpers.Common.GetSetting("bankcode");
			var bankname = Erp.BackOffice.Helpers.Common.GetSetting("bankname");
			var ImgLogo = "<div class=\"logo\"><img src=" + logo + " height=\"73\" /></div>";
			var template = templatePrintRepository.GetAllTemplatePrint().Where(x => x.Code.Contains("PrintReport")).OrderByDescending(x => x.CreatedDate).FirstOrDefault();
			model.Content = template.Content;
			model.Content = model.Content.Replace("{DataTable}", buildHtml(data));
			model.Content = model.Content.Replace("{System.Logo}", ImgLogo);
			model.Content = model.Content.Replace("{System.CompanyName}", company);
			model.Content = model.Content.Replace("{System.AddressCompany}", address);
			model.Content = model.Content.Replace("{System.PhoneCompany}", phone);
			model.Content = model.Content.Replace("{System.Fax}", fax);
			model.Content = model.Content.Replace("{System.BankCodeCompany}", bankcode);
			model.Content = model.Content.Replace("{System.BankNameCompany}", bankname);
			model.Content = model.Content.Replace("{Title}", "Danh Sách Khách Hàng");
			if (ExportExcel)
			{
				Response.AppendHeader("content-disposition", "attachment;filename=" + "DanhSachKH_" + DateTime.Now.ToString("yyyyMMdd") + ".xls");
				Response.Charset = "";
				Response.Cache.SetCacheability(HttpCacheability.NoCache);
				Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
				Response.Write(model.Content);
				Response.End();
			}

			return View(model);
			//return View();
		}
		string buildHtml(List<CustomerViewModel> DetailList)
		{
			string detailLists = "<table border=\"1\" class=\"invoice-detail\"  >\r\n";
			detailLists += "<thead>\r\n";
			detailLists += "	<tr>";
			detailLists += "		<th>STT</th>";
			detailLists += "		<th>Mã KH</th>";
			detailLists += "		<th>Tên Khách Hàng</th>";
			detailLists += "		<th>SĐT</th>";
			detailLists += "		<th>Ngày sinh</th>";
			detailLists += "		<th>Địa Chỉ</th>";
			detailLists += "		<th>Nhân Viên</th>";
			detailLists += "		<th>Nhóm</th>";
			detailLists += "		<th>Nguồn khách</th>";
			detailLists += "		<th>Ngày bắt đầu GD</th>";
			detailLists += "		<th>Ngày kết thúc GD</th>";
			detailLists += "		<th>Tổng tiền GD</th>";
			detailLists += "		<th>Tổng nợ</th>";
			detailLists += "	</tr>";
			detailLists += "</thead>\r\n";
			detailLists += "<tbody>\r\n";
			var index = 1;
			foreach (var item in DetailList)
			{


				detailLists += "<tr border=\"1\" >\r\n"
				+ "<td class=\"text-center orderNo\">" + (index++) + "</td>\r\n"
				+ "<td class=\"text-center \">" + item.Code + "</td>\r\n"
				+ "<td class=\"text-center \">" + item.CompanyName + "</td>\r\n"
				+ "<td class=\"text-center \">" + item.Phone + "</td>\r\n"
				 + "<td class=\"text-center \">" + item.Birthday + "</td>\r\n"
				+ "<td class=\"text-center \">" + item.Address + "</td>\r\n"
				+ "<td class=\"text-center \">" + item.ManagerStaffName + "</td>\r\n";
				if (item.TenNhomHuong != null)
				{
					//var user = userRepository.GetUserById(item.ManagerStaffId.Value);
					//if (user != null)
					//{
					//    if (user.UserType_kd_id != null)
					//    {
					//        var TenNhom = _userType_kdRepository.GetUserTypeById(user.UserType_kd_id.Value);
					//        if (TenNhom != null)
					//        {
					//            detailLists += "<td class=\"text-center \">" + TenNhom.Name + "</td>\r\n";
					//        }
					//        else
					//        {
					//            detailLists += "<td class=\"text-center \">" + "</td>\r\n";
					//        }
					//    }
					//    else
					//    {
					//        detailLists += "<td class=\"text-center \">" + "</td>\r\n";
					//    }

					//}
					//else
					//{
					detailLists += "<td class=\"text-center \">" + item.TenNhomHuong + "</td>\r\n";
					//}
				}
				else
				{
					detailLists += "<td class=\"text-center \">" + "</td>\r\n";
				}
				detailLists += "<td class=\"text-center \">" + item.NguonKhach + "</td>\r\n";
				detailLists += "<td class=\"text-center \">" + item.NgayMuaDau + "</td>\r\n"
				+ "<td class=\"text-center \">" + item.NgayMuaCuoi + "</td>\r\n"
				+ "<td class=\"text-center \">" + CommonSatic.ToCurrencyStr(item.TotalAmount, null).Replace(".", ",") + "</td>\r\n"
				+ "<td class=\"text-center \">" + CommonSatic.ToCurrencyStr(item.TienNo, null).Replace(".", ",") + "</td>\r\n"
				+ "</tr>\r\n";
			}
			detailLists += "</tbody>\r\n";
			detailLists += "<tfoot style=\"font-weight:bold\">\r\n";
			detailLists += "</tfoot>\r\n</table>\r\n";




			return detailLists;
		}

		#endregion
		IEnumerable<CustomerViewModel> FilterSearchIndex(IEnumerable<CustomerViewModel> q, string startDate, string endDate, string CardCode, string txtCusInfo, string Phone, string ProvinceId, string DistrictId, string WardId, int? SalerId)
		{
			bool hasSearch = false;
			//if (!string.IsNullOrEmpty(CardCode))
			//{
			//    q = q.Where(x => x.CardCode.Contains(CardCode));
			//    hasSearch = true;
			//}

			//if (!string.IsNullOrEmpty(txtCusName))
			//{
			//    txtCusName = Helpers.Common.ChuyenThanhKhongDau(txtCusName);
			//    q = q.Where(x => Helpers.Common.ChuyenThanhKhongDau(x.CompanyName).Contains(txtCusName));
			//    hasSearch = true;
			//}

			//if (!string.IsNullOrEmpty(txtCode))
			//{
			//    q = q.Where(x => x.Code.Contains(txtCode));
			//    hasSearch = true;
			//}

			if (!string.IsNullOrEmpty(txtCusInfo))
			{
				q = q.Where(x => Helpers.Common.ChuyenThanhKhongDau(x.CompanyName).Contains(Helpers.Common.ChuyenThanhKhongDau(txtCusInfo)) || x.Code.Contains(txtCusInfo));
				hasSearch = true;
			}

			if (!string.IsNullOrEmpty(Phone))
			{
				q = q.Where(x => x.Phone != null && x.Phone.Contains(Phone));
				hasSearch = true;
			}
			if (!string.IsNullOrEmpty(ProvinceId))
			{
				q = q.Where(item => item.CityId == ProvinceId);
				hasSearch = true;
			}
			if (!string.IsNullOrEmpty(DistrictId))
			{
				q = q.Where(item => item.DistrictId == DistrictId);
				hasSearch = true;
			}
			if (!string.IsNullOrEmpty(WardId))
			{
				q = q.Where(item => item.WardId == WardId);
				hasSearch = true;
			}

			if (SalerId != null && SalerId > 0)
			{
				q = q.Where(item => item.ManagerStaffId == SalerId);
				hasSearch = true;
			}

			DateTime d_startDate, d_endDate;
			if (DateTime.TryParseExact(startDate, "dd/MM/yyyy", new CultureInfo("vi-VN"), DateTimeStyles.None, out d_startDate))
			{
				if (DateTime.TryParseExact(endDate, "dd/MM/yyyy", new CultureInfo("vi-VN"), DateTimeStyles.None, out d_endDate))
				{
					d_endDate = d_endDate.AddHours(23).AddMinutes(59);
					q = q.Where(x => x.CreatedDate >= d_startDate && x.CreatedDate <= d_endDate).ToList();
				}
			}


			q = q.OrderByDescending(m => m.CreatedDate);

			//foreach (var item in q)
			//{
			//    if (item.NhomHuongDS != null)
			//    {
			//        var user_kd = _userType_kdRepository.GetUserTypeById(item.NhomHuongDS.Value);
			//        if (user_kd != null)
			//        {
			//            item.TenNhomHuong = user_kd.Name;
			//        }
			//    }
			//}
			//if(hasSearch)
			//{
			//    q = q.OrderByDescending(m => m.CompanyName);
			//    hasSearch = true;
			//}
			//else
			//{
			//    q = null;
			//}

			ViewBag.SuccessMessage = TempData["SuccessMessage"];
			ViewBag.FailedMessage = TempData["FailedMessage"];
			ViewBag.AlertMessage = TempData["AlertMessage"];
			ViewBag.AdviseTypeError = TempData["AdviseTypeError"];
			return q;
		}

		public ViewResult IndexSearch(string CardCode, string txtCusInfo, string Phone, string ProvinceId, string DistrictId, string WardId, string search, string Allcus, string startDate, string endDate)
		{
			if (search == "Search")
			{
				//get cookie brachID 
				HttpRequestBase request = this.HttpContext.Request;
				string strBrandID = "0";
				if (request.Cookies["BRANCH_ID_SPA_CookieName"] != null)
				{
					strBrandID = request.Cookies["BRANCH_ID_SPA_CookieName"].Value;
					if (strBrandID == "")
					{
						strBrandID = "0";
					}
				}

				//get  CurrentUser.branchId

				if ((Helpers.Common.CurrentUser.BranchId != null) && (Helpers.Common.CurrentUser.BranchId > 0))
				{
					strBrandID = Helpers.Common.CurrentUser.BranchId.ToString();
				}

				int? intBrandID = int.Parse(strBrandID);

				IEnumerable<CustomerViewModel> q = CustomerRepository.GetAllvwCustomer().AsEnumerable()
					//.Where(x => ("," + Helpers.Common.CurrentUser.DrugStore + ",").Contains("," + x.BranchId + ",") == true&&x.CustomerType=="Customer")
					//.Where(x => x.BranchId == Helpers.Common.CurrentUser.BranchId)
					.Select(item => new CustomerViewModel
					{
						Id = item.Id,
						CreatedUserId = item.CreatedUserId,
						//CreatedUserName = item.CreatedUserName,
						CreatedDate = item.CreatedDate,
						ModifiedUserId = item.ModifiedUserId,
						//ModifiedUserName = item.ModifiedUserName,
						ModifiedDate = item.ModifiedDate,
						Code = item.Code,
						CompanyName = item.CompanyName,
						Phone = item.Phone,
						Address = item.Address,
						Note = item.Note,
						CardCode = item.CardCode,
						Image = item.Image,
						Birthday = item.Birthday,
						Email = item.Email,
						Phoneghep = item.Phoneghep,
						Gender = item.Gender,
						IdCardDate = item.IdCardDate,
						IdCardIssued = item.IdCardIssued,
						IdCardNumber = item.IdCardNumber,
						CardIssuedName = item.CardIssuedName,
						BranchName = item.BranchName,
						ManagerStaffName = item.ManagerStaffName,
						ManagerUserName = item.ManagerUserName,
						CustomerType = item.CustomerType,
						BranchId = item.BranchId

					}).OrderByDescending(m => m.CreatedDate);

				bool hasSearch = false;
				if (Allcus != "on")
				{
					q = q.Where(x => x.BranchId == intBrandID);
					hasSearch = true;
				}
				// if (startDate != "")
				//{
				// q=q.Where()
				//  }
				DateTime d_startDate, d_endDate;
				if (DateTime.TryParseExact(startDate, "dd/MM/yyyy", new CultureInfo("vi-VN"), DateTimeStyles.None, out d_startDate))
				{
					if (DateTime.TryParseExact(endDate, "dd/MM/yyyy", new CultureInfo("vi-VN"), DateTimeStyles.None, out d_endDate))
					{
						d_endDate = d_endDate.AddHours(23).AddMinutes(59);
						q = q.Where(x => x.CreatedDate >= d_startDate && x.CreatedDate <= d_endDate).ToList();
					}
				}

				//if (!string.IsNullOrEmpty(CardCode))
				//{
				//    q = q.Where(x => x.CardCode != null && x.CardCode.Contains(CardCode)) || q.Where(x=>x.Code.Contains(txtCusInfo)
				//    hasSearch = true;
				//}

				//if (!string.IsNullOrEmpty(txtCusName))
				//{
				//    txtCusName = Helpers.Common.ChuyenThanhKhongDau(txtCusName);
				//    q = q.Where(x => Helpers.Common.ChuyenThanhKhongDau(x.CompanyName).Contains(txtCusName));
				//    hasSearch = true;
				//}

				//if (!string.IsNullOrEmpty(txtCode))
				//{
				//    q = q.Where(x => x.Code.Contains(txtCode));
				//    hasSearch = true;
				//}

				if (!string.IsNullOrEmpty(txtCusInfo))
				{
					q = q.Where(x => Helpers.Common.ChuyenThanhKhongDau(x.CompanyName).Contains(Helpers.Common.ChuyenThanhKhongDau(txtCusInfo.Trim())) || x.Code.Contains(txtCusInfo.Trim()));
					hasSearch = true;
				}

				if (!string.IsNullOrEmpty(Phone))
				{
					q = q.Where(x => x.Phoneghep.Contains(Phone.Trim()));
					hasSearch = true;
				}
				if (!string.IsNullOrEmpty(ProvinceId))
				{
					q = q.Where(item => item.CityId == ProvinceId);
					hasSearch = true;
				}
				if (!string.IsNullOrEmpty(DistrictId))
				{
					q = q.Where(item => item.DistrictId == DistrictId);
					hasSearch = true;
				}
				if (!string.IsNullOrEmpty(WardId))
				{
					q = q.Where(item => item.WardId == WardId);
					hasSearch = true;
				}
				//if(hasSearch)
				//{
				//    q = q.OrderByDescending(m => m.CompanyName);
				//    hasSearch = true;
				//}
				//else
				//{
				//    q = null;
				//}

				ViewBag.SuccessMessage = TempData["SuccessMessage"];
				ViewBag.FailedMessage = TempData["FailedMessage"];
				ViewBag.AlertMessage = TempData["AlertMessage"];
				ViewBag.AdviseTypeError = TempData["AdviseTypeError"];
				return View(q);
			}

			return View();
		}
		//public ActionResult ListSMS()
		//{

		//}
		#endregion


		//#region Edit
		//public ActionResult Edit(int? Id)
		//{
		//    var Customer = CustomerRepository.GetCustomerById(Id.Value);
		//    if (Customer != null && Customer.IsDeleted != true)
		//    {
		//        var model = new CustomerViewModel();
		//        AutoMapper.Mapper.Map(Customer, model);
		//        return View(model);
		//    }
		//    if (Request.UrlReferrer != null)
		//        return Redirect(Request.UrlReferrer.AbsoluteUri);
		//    return RedirectToAction("Index");
		//}

		//[HttpPost]
		//public ActionResult Edit(CustomerViewModel model)
		//{
		//    if (ModelState.IsValid)
		//    {
		//        if (Request["Submit"] == "Save")
		//        {
		//            var Customer = CustomerRepository.GetCustomerById(model.Id);
		//            AutoMapper.Mapper.Map(model, Customer);
		//            Customer.ModifiedUserId = WebSecurity.CurrentUserId;
		//            Customer.ModifiedDate = DateTime.Now;

		//            ////tạo đặc tính động cho sản phẩm nếu có danh sách đặc tính động truyền vào và đặc tính đó phải có giá trị
		//            //ObjectAttributeController.CreateOrUpdateForObject(Customer.Id, model.AttributeValueList);

		//            CustomerRepository.UpdateCustomer(Customer);

		//            TempData[Globals.SuccessMessageKey] = App_GlobalResources.Wording.UpdateSuccess;
		//            return RedirectToAction("Index");
		//        }

		//        return View(model);
		//    }
		//    return View(model);

		//    //if (Request.UrlReferrer != null)
		//    //    return Redirect(Request.UrlReferrer.AbsoluteUri);
		//    //return RedirectToAction("Index");
		//}

		//#endregion

		//#region Create
		//public ViewResult Create(string Phone)
		//{
		//    var model = new CustomerViewModel();
		//    model.ProvinceList = Helpers.SelectListHelper.GetSelectList_Location("0", null, App_GlobalResources.Wording.Empty);

		//    if (!string.IsNullOrEmpty(Phone))
		//    {
		//        model.Phone = Phone;
		//    }

		//    return View(model);
		//}

		//[HttpPost]
		//public ActionResult Create(CustomerViewModel model, bool IsPopup)
		//{
		//    if (ModelState.IsValid)
		//    {
		//        //Kiểm tra trùng tên số điện thoại thì không cho lưu
		//        var c = CustomerRepository.GetAllvwCustomer()
		//            .Where(item => item.Phone == model.Phone && item.CompanyName == model.CompanyName).FirstOrDefault();

		//        if (c != null)
		//        {
		//            TempData[Globals.FailedMessageKey] = "Khách hàng này đã tồn tại!";
		//            return RedirectToAction("Detail", new { Id = c.Id });
		//        }

		//        var Customer = new Domain.Account.Entities.Customer();
		//        AutoMapper.Mapper.Map(model, Customer);
		//        Customer.IsDeleted = false;
		//        Customer.CreatedUserId = WebSecurity.CurrentUserId;
		//        Customer.ModifiedUserId = WebSecurity.CurrentUserId;
		//        Customer.CreatedDate = DateTime.Now;
		//        Customer.ModifiedDate = DateTime.Now;
		//        Customer.CustomerType = "Customer";
		//        CustomerRepository.InsertCustomer(Customer);
		//        var prefix1 = Erp.BackOffice.Helpers.Common.GetSetting("prefixOrderNo_Customer");
		//        Customer.Code = Erp.BackOffice.Helpers.Common.GetCode(prefix1, Customer.Id);
		//        CustomerRepository.UpdateCustomer(Customer);

		//        //tạo liên hệ cho khách hàng
		//        if (string.IsNullOrEmpty(model.FirstName) == false && string.IsNullOrEmpty(model.LastName) == false)
		//        {
		//            var contact = new Domain.Account.Entities.Contact();
		//            AutoMapper.Mapper.Map(model, contact);
		//            contact.IsDeleted = false;
		//            contact.CreatedUserId = WebSecurity.CurrentUserId;
		//            contact.ModifiedUserId = WebSecurity.CurrentUserId;
		//            contact.CreatedDate = DateTime.Now;
		//            contact.ModifiedDate = DateTime.Now;
		//            contact.CustomerId = Customer.Id;

		//            ContactRepository.InsertContact(contact);
		//        }

		//        //tạo đặc tính động cho sản phẩm nếu có danh sách đặc tính động truyền vào và đặc tính đó phải có giá trị
		//        //   ObjectAttributeController.CreateOrUpdateForObject(Customer.Id, model.AttributeValueList);

		//        if (Request["IsPopup"] == "true")
		//        {
		//            ViewBag.closePopup = "close and append to page parent";
		//            model.FullName = model.LastName + " " + model.FirstName;
		//            model.Id = Customer.Id;
		//            return View(model);
		//        }
		//        if (IsPopup)
		//        {
		//            return RedirectToAction("_ClosePopup", "Home", new { area = "", FunctionCallback = "ClosePopupAndReloadPage(" + Customer.Id + ",'" + Customer.CompanyName + "')" });
		//        }
		//        TempData[Globals.SuccessMessageKey] = App_GlobalResources.Wording.InsertSuccess;
		//        return RedirectToAction("Detail", new { Id = Customer.Id });
		//    }
		//    return View(model);
		//}
		//#endregion

		#region Delete
		[HttpPost]
		public ActionResult Delete()
		{
			try
			{
				string idDeleteAll = Request["DeleteId-checkbox"];
				string[] arrDeleteId = idDeleteAll.Split(',');
				for (int i = 0; i < arrDeleteId.Count(); i++)
				{
					var item = CustomerRepository.GetCustomerById(int.Parse(arrDeleteId[i], CultureInfo.InvariantCulture));
					if (item != null)
					{
						//if (item.CreatedUserId != Erp.BackOffice.Helpers.Common.CurrentUser.Id && Erp.BackOffice.Helpers.Common.CurrentUser.UserTypeId != 1)
						//{
						//    TempData["FailedMessage"] = "NotOwner";
						//    return RedirectToAction("Index");
						//}

						item.IsDeleted = true;
						CustomerRepository.UpdateCustomer(item);
					}
				}
				TempData[Globals.SuccessMessageKey] = App_GlobalResources.Wording.DeleteSuccess;
				return RedirectToAction("Index");
			}
			catch (DbUpdateException)
			{
				TempData[Globals.FailedMessageKey] = App_GlobalResources.Error.RelationError;
				return RedirectToAction("Index");
			}
		}
		#endregion

		#region Detail
		public ActionResult Detail(int? Id, bool? IsNullLayOut, int? LeadId)
		{
			if (Id > 0)
			{
				var customer = SqlHelper.QuerySP<vwCustomer>("spGetCustomerById", new { Id = Id }).SingleOrDefault();
				int TagetMonth = Request["TargetMonth"] != null ? int.Parse(Request["TargetMonth"]) : -1;
				int TagetYear = Request["TargetYear"] != null ? int.Parse(Request["TargetYear"]) : -1;
				ViewBag.IsNullLayOut = IsNullLayOut;
				var model = new CustomerViewModel();
				if (customer != null && customer.IsDeleted != true)
				{
					AutoMapper.Mapper.Map(customer, model);
					var service = SqlHelper.QuerySP<ProductInvoiceViewModel>("spGetAllvwProductInvoiceByCustomer", new { Id = customer.Id, @month = TagetMonth != -1 ? TagetMonth : 0, @year = TagetMonth != -1 ? TagetMonth : 0 });

					var serviceLogVip = SqlHelper.QuerySP<LogVipViewModel>("spGetAllLogVip", new { Id = customer.Id });

					//lấy danh sách CustomerUser

					model.CustomerUserList = GetCustomerUserListByCustomerId(Id.Value);
					model.CustomerRecommendList = GetCustomerRecommendListByCustomerId(Id.Value);
					service = service.OrderByDescending(n => n.CreatedDate);
					ViewBag.service = service;
					ViewBag.serviceLogVip = serviceLogVip;

					//Lấy công nợ hiện tại của KH
					model.Liabilities = SqlHelper.QuerySP("GetvwAccount_Liabilities", new { @Code = customer.Code }).FirstOrDefault() != null ? SqlHelper.QuerySP("GetvwAccount_Liabilities", new { @Code = customer.Code }).FirstOrDefault().Remain : 0;
					//get cookie brachID 
					HttpRequestBase request = this.HttpContext.Request;
					string strBrandID = "0";
					if (request.Cookies["BRANCH_ID_SPA_CookieName"] != null)
					{
						strBrandID = request.Cookies["BRANCH_ID_SPA_CookieName"].Value;
						if (strBrandID == "")
						{
							strBrandID = "0";
						}
					}

					//get  CurrentUser.branchId

					if ((Helpers.Common.CurrentUser.BranchId != null) && (Helpers.Common.CurrentUser.BranchId > 0))
					{
						strBrandID = Helpers.Common.CurrentUser.BranchId.ToString();
					}

					int? intBrandID = int.Parse(strBrandID);
					IEnumerable<CustomerViewModel> q = SqlHelper.QuerySP<CustomerViewModel>("GetAllvwCustomerDetail", new { @BranchId = intBrandID });
					var checkIsLetan = SqlHelper.QuerySP<int>("checkUserIsLetan", new
					{
						@pId = WebSecurity.CurrentUserId
					}).ToList();
					var leaddt = SqlHelper.QuerySP<LeadModel>("LoadLeadIndex", new
					{
						@pId = WebSecurity.CurrentUserId,
						@pIsLeTan = (checkIsLetan.Count > 0) ? 1 : 0
					});
					var ldt = leaddt.Where(x => x.Id == LeadId).FirstOrDefault();
					if (ldt != null)
					{
						model.Email = ldt.Email;
						model.LeadId = ldt.Id;
						model.Address = ldt.Address;
						model.Phone = ldt.Mobile;
						model.LastName = ldt.Name;
					}
					if (LeadId != null)
					{
						SqlHelper.ExecuteSP("UPDATE_SALE_CUSTOMER", new { LEADID = LeadId, ID = Id, UserId = WebSecurity.CurrentUserId });
					}
					ViewBag.CustomerList = q;
					ViewBag.SuccessMessage = TempData["SuccessMessage"];
					ViewBag.FailedMessage = TempData["FailedMessage"];
					ViewBag.AlertMessage = TempData["AlertMessage"];

					return View(model);
				}

				TempData[Globals.FailedMessageKey] = "Không tìm thấy thông tin khách hàng. Vui lòng kiểm tra lại!";
				return View(model);
			}
			else
			{

				return RedirectToAction("CreateNT", new { Phone = cus_crm_model.Phone, CompanyName = cus_crm_model.CompanyName, FirstName = cus_crm_model.FirstName, LastName = cus_crm_model.LastName, FullName = cus_crm_model.FullName, cus_crm = cus_crm_model.cus_crm, LeadId = LeadId });
			}
		}
		[AllowAnonymous]
		[HttpGet]
		public ActionResult DsLeadCustomer(int Id)
		{
			var leaddt = SqlHelper.QuerySP<LeadModel>("DsLeadCustomer", new
			{
				@Id = Id
			});
			var statusCustomer = SqlHelper.QuerySP<StatusLeadModel>("Crm_StatusCustomer_Get", new
			{
			});

			var user = userRepository;
			ViewBag.user = user;
			ViewBag.statusCustomer = statusCustomer;
			return View(leaddt);
		}
		#endregion
		public ActionResult DetailSearch(int? Id, bool? IsNullLayOut)
		{
			if (Id > 0)
			{
				var customer = CustomerRepository.GetvwCustomerById(Id.Value);
				int TagetMonth = Request["TargetMonth"] != null ? int.Parse(Request["TargetMonth"]) : -1;
				int TagetYear = Request["TargetYear"] != null ? int.Parse(Request["TargetYear"]) : -1;
				ViewBag.IsNullLayOut = IsNullLayOut;
				var model = new CustomerViewModel();
				if (customer != null && customer.IsDeleted != true)
				{
					AutoMapper.Mapper.Map(customer, model);
					var service = productInvoiceRepository.GetAllvwProductInvoice().Where(x => x.CustomerId == customer.Id).AsEnumerable()
						.Select(x => new ProductInvoiceViewModel
						{
							Code = x.Code,
							CodeInvoiceRed = x.CodeInvoiceRed,
							Note = x.Note,
							Id = x.Id,
							TotalAmount = x.TotalAmount,
							CreatedDate = x.CreatedDate,
							StaffCreateName = x.StaffCreateName,
							BranchName = x.BranchName
						});

					var serviceLogVip = logVipRepository.GetAllLogVip().Where(x => x.CustomerId == customer.Id).AsEnumerable()
						.Select(x => new LogVipViewModel
						{
							//  Year = x.Year,
							Ratings = x.Ratings
						});

					if (TagetMonth != -1)
					{
						service = service.Where(n => n.CreatedDate.Value.Month == TagetMonth);
					}
					if (TagetYear != -1)
					{
						service = service.Where(n => n.CreatedDate.Value.Year == TagetYear);
					}

					ViewBag.service = service;
					ViewBag.serviceLogVip = serviceLogVip;

					//Lấy công nợ hiện tại của KH
					model.Liabilities = transactionLiabilitiesRepository.GetvwAccount_Liabilities()
						.Where(item => item.TargetModule == "Customer" && item.TargetCode == customer.Code)
						.Select(item => item.Remain).FirstOrDefault();

					ViewBag.SuccessMessage = TempData["SuccessMessage"];
					ViewBag.FailedMessage = TempData["FailedMessage"];
					ViewBag.AlertMessage = TempData["AlertMessage"];



					//begin hoapd lay lich hen KH
					//if (customer.cus_crm != null && customer.cus_crm.Length > 0)
					//{
					//    using (var client = new HttpClient())
					//    {
					//        // Initialize HTTP client
					//        client.BaseAddress = new Uri("https://intranet.trangha.com.vn/rest/", UriKind.Absolute);
					//        client.Timeout = TimeSpan.FromSeconds(50);
					//        client.DefaultRequestHeaders.Accept.Clear();
					//        //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("nhom1q5", "demo@123");
					//        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
					//        // Build session data to send
					//        //var values = new List<KeyValuePair<string, string>>();

					//        //values.Add(new KeyValuePair<string, string>("StartDate", "01/01/2019"));
					//        //values.Add(new KeyValuePair<string, string>("EndDate", "30/01/2019"));
					//        //values.Add(new KeyValuePair<string, string>("UserId", "7765"));


					//        client.DefaultRequestHeaders.Authorization =
					//       new AuthenticationHeaderValue(
					//           "Basic",
					//           Convert.ToBase64String(
					//               System.Text.ASCIIEncoding.ASCII.GetBytes(
					//                   string.Format("{0}:{1}", Erp.BackOffice.Helpers.Common.GetSetting("UserName_Bitrix"), Erp.BackOffice.Helpers.Common.GetSetting("PassWord_Bitrix")))));

					//        // Build session data to send
					//        //var values = new List<KeyValuePair<string, string>>();

					//        //values.Add(new KeyValuePair<string, string>("StartDate", "01/01/2019"));
					//        //values.Add(new KeyValuePair<string, string>("EndDate", "30/01/2019"));
					//        //values.Add(new KeyValuePair<string, string>("UserId", "7765"));

					//        // Send session data via POST using form-url-encoded content
					//        string pTocken = Erp.BackOffice.Helpers.Common.GetSetting("access_token");// "88af7a5c003534b80031ffbe0000020800000339f3b54e41e21b19dad6ec22f9722f39";
					//        using (var response = client.GetAsync("https://intranet.trangha.com.vn/oauth/token/?grant_type=refresh_token&client_id=local.5c52681b619942.47504244&client_secret=x60zSfvIjjF2r6WXLcnzDiJlhS5t82SjeaU7IWMSE2CZoBzLVR&refresh_token=" + pTocken + "&scope=granted_permission&redirect_uri=app_URL").Result)
					//        {
					//            string responseString = response.Content.ReadAsStringAsync().Result;
					//            Newtonsoft.Json.Linq.JObject json = Newtonsoft.Json.Linq.JObject.Parse(responseString);
					//            pTocken = json["access_token"].ToString();
					//            // Console.WriteLine(response.StatusCode);
					//        }

					//        // Send session data via POST using form-url-encoded content
					//        string pIDCus = customer.cus_crm;

					//        //begin lay lich hen dua vao khach hang

					//        using (var response = client.GetAsync(string.Format("https://intranet.trangha.com.vn/rest/crm.activity.list?FILTER[OWNER_ID]=" + pIDCus + "& auth=" + pTocken)).Result)
					//        {
					//            string responseString = response.Content.ReadAsStringAsync().Result;

					//            if (response.StatusCode.ToString() == "OK")
					//            {

					//                List<LogVipViewModelCrm> ListLogVipViewModelCrm = new List<LogVipViewModelCrm>();
					//                Newtonsoft.Json.Linq.JObject json = Newtonsoft.Json.Linq.JObject.Parse(responseString);
					//                JArray signInNames = (JArray)json.SelectToken("result");

					//                if (signInNames.Count > 0)
					//                {

					//                    for (int i = 0; i < signInNames.Count; i++)
					//                    {
					//                        var obj = ((JArray)json.SelectToken("result")).ToObject<List<JObject>>().ElementAt(i);
					//                        LogVipViewModelCrm cus = new LogVipViewModelCrm
					//                        {
					//                            Id = 0,
					//                            Chude = obj["SUBJECT"].ToString(),
					//                            Mota = obj["DESCRIPTION"].ToString(),
					//                            Thoigianhen = obj["START_TIME"].ToString() + "-" + obj["END_TIME"].ToString(),
					//                            Hoanthanh = obj["COMPLETED"].ToString()
					//                        };

					//                        ListLogVipViewModelCrm.Add(cus);
					//                    }

					//                    ViewBag.ListLogVipViewModelCrm = ListLogVipViewModelCrm;
					//                }

					//            }

					//            //end lay lich hen dua vao khach hang
					//        }
					//    }
					//}
					//else//truong hop khong phai la khach hang tao tu CRM
					//{
					//    //begin buoc 1 lay code
					//    if (model.Phone != null && model.Phone.Length > 0)
					//    {
					//        using (var client = new HttpClient())
					//        {
					//            // Initialize HTTP client
					//            client.BaseAddress = new Uri("https://intranet.trangha.com.vn/rest/", UriKind.Absolute);
					//            client.Timeout = TimeSpan.FromSeconds(50);
					//            client.DefaultRequestHeaders.Accept.Clear();
					//            //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("nhom1q5", "demo@123");
					//            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
					//            // Build session data to send
					//            //var values = new List<KeyValuePair<string, string>>();

					//            //values.Add(new KeyValuePair<string, string>("StartDate", "01/01/2019"));
					//            //values.Add(new KeyValuePair<string, string>("EndDate", "30/01/2019"));
					//            //values.Add(new KeyValuePair<string, string>("UserId", "7765"));

					//            // Send session data via POST using form-url-encoded content

					//            client.DefaultRequestHeaders.Authorization =
					//           new AuthenticationHeaderValue(
					//               "Basic",
					//               Convert.ToBase64String(
					//                   System.Text.ASCIIEncoding.ASCII.GetBytes(
					//                       string.Format("{0}:{1}", Erp.BackOffice.Helpers.Common.GetSetting("UserName_Bitrix"), Erp.BackOffice.Helpers.Common.GetSetting("PassWord_Bitrix")))));

					//            // Build session data to send
					//            //var values = new List<KeyValuePair<string, string>>();

					//            //values.Add(new KeyValuePair<string, string>("StartDate", "01/01/2019"));
					//            //values.Add(new KeyValuePair<string, string>("EndDate", "30/01/2019"));
					//            //values.Add(new KeyValuePair<string, string>("UserId", "7765"));

					//            // Send session data via POST using form-url-encoded content
					//            string pTocken = Erp.BackOffice.Helpers.Common.GetSetting("access_token");// "88af7a5c003534b80031ffbe0000020800000339f3b54e41e21b19dad6ec22f9722f39";
					//            using (var response = client.GetAsync("https://intranet.trangha.com.vn/oauth/token/?grant_type=refresh_token&client_id=local.5c52681b619942.47504244&client_secret=x60zSfvIjjF2r6WXLcnzDiJlhS5t82SjeaU7IWMSE2CZoBzLVR&refresh_token=" + pTocken + "&scope=granted_permission&redirect_uri=app_URL").Result)
					//            {
					//                string responseString = response.Content.ReadAsStringAsync().Result;
					//                Newtonsoft.Json.Linq.JObject json = Newtonsoft.Json.Linq.JObject.Parse(responseString);
					//                pTocken = json["access_token"].ToString();
					//                // Console.WriteLine(response.StatusCode);
					//            }

					//            string pIDCus = "";
					//            using (var response = client.GetAsync(string.Format("crm.contact.list?FILTER[PHONE]=" + model.Phone + "& auth=" + pTocken + "& select[]=PHONE&select[]=NAME&select[]=LAST_NAME&select[]=ID&select[]=ADDRESS")).Result)
					//            {
					//                string responseString = response.Content.ReadAsStringAsync().Result;

					//                if (response.StatusCode.ToString() == "OK")
					//                {


					//                    Newtonsoft.Json.Linq.JObject json = Newtonsoft.Json.Linq.JObject.Parse(responseString);
					//                    JArray signInNames = (JArray)json.SelectToken("result");

					//                    if (signInNames.Count > 0)
					//                    {
					//                        var obj = ((JArray)json.SelectToken("result")).ToObject<List<JObject>>().FirstOrDefault();

					//                        pIDCus = obj["ID"].ToString();

					//                        //begin lay lich hen dua vao khach hang

					//                        using (var response1 = client.GetAsync(string.Format("https://intranet.trangha.com.vn/rest/crm.activity.list?FILTER[OWNER_ID]=" + pIDCus + "& auth=" + pTocken)).Result)
					//                        {
					//                            string responseString1 = response1.Content.ReadAsStringAsync().Result;

					//                            if (response1.StatusCode.ToString() == "OK")
					//                            {

					//                                List<LogVipViewModelCrm> ListLogVipViewModelCrm = new List<LogVipViewModelCrm>();
					//                                Newtonsoft.Json.Linq.JObject json1 = Newtonsoft.Json.Linq.JObject.Parse(responseString1);
					//                                JArray signInNames1 = (JArray)json1.SelectToken("result");

					//                                if (signInNames1.Count > 0)
					//                                {

					//                                    for (int i = 0; i < signInNames1.Count; i++)
					//                                    {
					//                                        var obj1 = ((JArray)json1.SelectToken("result")).ToObject<List<JObject>>().ElementAt(i);
					//                                        LogVipViewModelCrm cus = new LogVipViewModelCrm
					//                                        {
					//                                            Id = 0,
					//                                            Chude = obj1["SUBJECT"].ToString(),
					//                                            Mota = obj1["DESCRIPTION"].ToString(),
					//                                            Thoigianhen = obj1["START_TIME"].ToString() + "-" + obj1["END_TIME"].ToString(),
					//                                            Hoanthanh = obj1["COMPLETED"].ToString()
					//                                        };

					//                                        ListLogVipViewModelCrm.Add(cus);
					//                                    }

					//                                    ViewBag.ListLogVipViewModelCrm = ListLogVipViewModelCrm;
					//                                }

					//                            }

					//                            //end lay lich hen dua vao khach hang

					//                        }

					//                    }

					//                }
					//            }




					//        }
					//    }

					//    //end buoc 1 lay code

					//}
					////end hoapd lay lich hen KH

					return View(model);
				}

				TempData[Globals.FailedMessageKey] = "Không tìm thấy thông tin khách hàng. Vui lòng kiểm tra lại!";
				return View(model);
			}
			else
			{

				return RedirectToAction("CreateNT", new { Phone = cus_crm_model.Phone, CompanyName = cus_crm_model.CompanyName, FirstName = cus_crm_model.FirstName, LastName = cus_crm_model.LastName, FullName = cus_crm_model.FullName, cus_crm = cus_crm_model.cus_crm });
			}
		}

		#region SendSMS
		[HttpPost]
		public ContentResult SendSMS(int Id)
		{
			var customer = CustomerRepository.GetvwCustomerById(Id);
			if (customer != null && !string.IsNullOrEmpty(customer.Phone))
			{
				string User = "dgthcm";
				string Password = "147a@258";
				string CPCode = "DIGITAL_HCM";
				string RequestID = "1";
				string UserID = "84987292389";
				string ReceiverID = "84987292389";
				string ServiceID = "N.HuongTest";
				string CommandCode = "bulksms";
				string sContent = "test gui tin nhan ngoc huong spa";
				string ContentType = "0";



				return Content(null);
			}

			return Content("");
		}
		#endregion

		#region Client
		public ActionResult Client()
		{

			return View();
		}
		#endregion

		//#region ClientListProductInvoice
		//public ActionResult ClientListProductInvoice(string txtCode, string txtCusName)
		//{
		//    ////mở camera ở server.
		//    //Erp.BackOffice.Hubs.ErpHub.ShowOrHiddenCamera(true, "show");

		//    var q = productInvoiceRepository.GetAllvwProductInvoiceFull();
		//    var model = q.Select(item => new ProductInvoiceViewModel
		//    {
		//        Id = item.Id,
		//        IsDeleted = item.IsDeleted,
		//        CreatedUserId = item.CreatedUserId,
		//        CreatedDate = item.CreatedDate,
		//        ModifiedUserId = item.ModifiedUserId,
		//        ModifiedDate = item.ModifiedDate,
		//        Code = item.Code,
		//        CustomerCode = item.CustomerCode,
		//        CustomerName = item.CustomerName,
		//        ShipCityName = item.ShipCityName,
		//        TotalAmount = item.TotalAmount,
		//        Discount = item.Discount,
		//        TaxFee = item.TaxFee,
		//        CodeInvoiceRed = item.CodeInvoiceRed,
		//        Status = item.Status,
		//        IsArchive = item.IsArchive,
		//        ProductOutboundId = item.ProductOutboundId,
		//        ProductOutboundCode = item.ProductOutboundCode,
		//        Note = item.Note,
		//        CancelReason = item.CancelReason,
		//        CustomerId = item.CustomerId,
		//        BranchId=item.BranchId,
		//        BranchName=item.BranchName
		//    }).OrderByDescending(m => m.ModifiedDate).ToList();

		//    if (Helpers.Common.CurrentUser.BranchId != null && Helpers.Common.CurrentUser.BranchId.Value > 0)
		//    {
		//        model = model.Where(x => x.BranchId == Helpers.Common.CurrentUser.BranchId).ToList();
		//    }
		//    if (!string.IsNullOrEmpty(txtCode))
		//    {
		//        model = model.Where(x => Helpers.Common.ChuyenThanhKhongDau(x.Code).Contains(Helpers.Common.ChuyenThanhKhongDau(txtCode))).ToList();
		//    }

		//    if (!string.IsNullOrEmpty(txtCusName))
		//    {
		//        txtCusName = Helpers.Common.ChuyenThanhKhongDau(txtCusName);
		//        model = model.Where(x => Helpers.Common.ChuyenThanhKhongDau(x.CustomerName).Contains(txtCusName)).ToList();
		//    }
		//    model = model.Take(15).ToList();
		//    return View(model);
		//}
		//#endregion

		#region Camera
		public ActionResult Camera(int? Id, string ConnectionID)
		{
			if (Id != null)
			{
				var customer = CustomerRepository.GetvwCustomerById(Id.Value);
				CustomerViewModel model = null;
				if (customer != null && customer.IsDeleted != true)
				{
					model = new CustomerViewModel();
					AutoMapper.Mapper.Map(customer, model);
					ViewBag.SuccessMessage = TempData["SuccessMessage"];
					ViewBag.FailedMessage = TempData["FailedMessage"];
					ViewBag.AlertMessage = TempData["AlertMessage"];
					ViewBag.ConnectionID = ConnectionID;
					return View(model);
				}
			}

			return View();
		}
		#endregion

		#region TakePhoto
		public ActionResult TakePhoto(int Id, int UserId)
		{
			var customer = CustomerRepository.GetvwCustomerById(Id);
			CustomerViewModel model = null;
			//if (Erp.BackOffice.Helpers.Common.GetSetting("status_camera_server") == "hidden")
			//{
			//    TempData[Globals.FailedMessageKey] = "Hiện tại client đang chụp hình! Nên bạn không thể chụp hình. Vui lòng tắt chụp hình ở client";
			//    return View(model);
			//}
			ViewBag.UserId = UserId;
			if (customer != null && customer.IsDeleted != true)
			{
				model = new CustomerViewModel();
				AutoMapper.Mapper.Map(customer, model);

				ViewBag.SuccessMessage = TempData["SuccessMessage"];
				ViewBag.FailedMessage = TempData["FailedMessage"];
				ViewBag.AlertMessage = TempData["AlertMessage"];

				return View(model);
			}

			TempData[Globals.FailedMessageKey] = "Không tìm thấy thông tin khách hàng. Vui lòng kiểm tra lại!";
			return View(model);
		}

		[HttpPost]
		public ActionResult TakePhoto(CustomerViewModel model)
		{
			if (Request["Submit"] == "Save" && !string.IsNullOrEmpty(model.Image))
			{
				var Customer = CustomerRepository.GetCustomerById(model.Id);
				Customer.ModifiedUserId = WebSecurity.CurrentUserId;
				Customer.ModifiedDate = DateTime.Now;
				Customer.Image = Customer.Code + ".png";

				//Lưu hình
				string base64 = model.Image.Substring(model.Image.IndexOf(',') + 1);
				base64 = base64.Trim('\0');
				byte[] data = Convert.FromBase64String(base64);

				//convert to an image (or do whatever else you need to do)
				Image image;
				using (MemoryStream ms = new MemoryStream(data))
				{
					image = Image.FromStream(ms);
				}

				var customerImagePath = Helpers.Common.GetSetting("Customer");
				image.Save(Server.MapPath("~" + customerImagePath) + Customer.Image, System.Drawing.Imaging.ImageFormat.Png);

				CustomerRepository.UpdateCustomer(Customer);
				return RedirectToAction("_ClosePopup", "Home", new { area = "", FunctionCallback = "ClosePopupAndReloadPage" });
			}

			TempData[Globals.FailedMessageKey] = "Vui lòng chụp ảnh!";
			return RedirectToAction("TakePhoto", new { Id = model.Id });
		}
		#endregion

		#region Approval
		public ActionResult Approval()
		{
			var q = CustomerRepository.GetAllCustomer().ToList();
			for (int i = 0; i < q.Count(); i++)
			{
				if (!string.IsNullOrEmpty(q[i].CompanyName))
				{
					q[i].SearchFullName = Helpers.Common.ChuyenThanhKhongDau(q[i].CompanyName);
					CustomerRepository.UpdateCustomer(q[i]);
				}
			}
			return RedirectToAction("Index");

		}
		#endregion

		#region SavePoint
		public static void SavePoint(int Id, int Point)
		{

			Erp.Domain.Account.Repositories.CustomerRepository customerRepository = new Erp.Domain.Account.Repositories.CustomerRepository(new Domain.Account.ErpAccountDbContext());
			var Customer = customerRepository.GetCustomerById(Id);
			//var customerPoint = new Domain.Account.Entities.Customer();
			Customer.Point += Point;
			customerRepository.InsertCustomer(Customer);
		}

		#endregion

		#region PhotoCustomer
		public ActionResult PhotoCustomer(int Id, int UserId)
		{
			var usinglogdetail = usingServiceLogdetailRepository.GetvwUsingServiceLogDetailById(Id);
			var customer = CustomerRepository.GetvwCustomerById(usinglogdetail.CustomerId.Value);
			UsingServiceLogDetailViewModel model = null;
			if (customer != null && customer.IsDeleted != true)
			{
				model = new UsingServiceLogDetailViewModel();
				AutoMapper.Mapper.Map(usinglogdetail, model);
				var customerModel = new CustomerViewModel();
				AutoMapper.Mapper.Map(customer, customerModel);
				ViewBag.SuccessMessage = TempData["SuccessMessage"];
				ViewBag.FailedMessage = TempData["FailedMessage"];
				ViewBag.AlertMessage = TempData["AlertMessage"];
				ViewBag.Customer = customerModel;
				ViewBag.UserId = UserId;
				return View(model);
			}

			TempData[Globals.FailedMessageKey] = "Không tìm thấy thông tin khách hàng. Vui lòng kiểm tra lại!";
			return View(model);
		}

		[HttpPost]
		public ActionResult PhotoCustomer(UsingServiceLogDetailViewModel model)
		{
			if (Request["Submit"] == "Save" && model.DetailList.Count() > 0)
			{
				var usinglogdetail = usingServiceLogdetailRepository.GetvwUsingServiceLogDetailById(model.Id);
				var Customer = CustomerRepository.GetCustomerById(usinglogdetail.CustomerId.Value);
				var DocumentField = new DocumentField();
				var type = "";
				if (usinglogdetail.Type == "usedservice")
				{
					type = " sử dụng dịch vụ ngày " + usinglogdetail.CreatedDate.Value.ToString("dd/MM/yyyy HH:mm");
				}
				else
				{
					type = " tái khám ngày " + usinglogdetail.CreatedDate.Value.ToString("dd/MM/yyyy HH:mm");
				}
				DocumentField.IsDeleted = false;
				DocumentField.CreatedUserId = WebSecurity.CurrentUserId;
				DocumentField.ModifiedUserId = WebSecurity.CurrentUserId;
				DocumentField.AssignedUserId = WebSecurity.CurrentUserId;
				DocumentField.CreatedDate = DateTime.Now;
				DocumentField.ModifiedDate = DateTime.Now;
				DocumentField.Name = Customer.CompanyName + type;
				//  DocumentField.DocumentTypeId = DocumentTypeId;
				// DocumentField.IsSearch = "";
				DocumentField.Category = "UsingServiceLogDetail";
				DocumentField.CategoryId = usinglogdetail.Id;
				DocumentFieldRepository.InsertDocumentField(DocumentField);
				var prefix = Erp.BackOffice.Helpers.Common.GetSetting("prefixOrderNo_DocumentField");
				DocumentField.Code = Erp.BackOffice.Helpers.Common.GetCode(prefix, DocumentField.Id);
				DocumentFieldRepository.UpdateDocumentField(DocumentField);
				int dem = 0;
				foreach (var item in model.DetailList)
				{
					var DocumentAttribute = new DocumentAttribute();
					DocumentAttribute.IsDeleted = false;
					DocumentAttribute.CreatedUserId = WebSecurity.CurrentUserId;
					DocumentAttribute.ModifiedUserId = WebSecurity.CurrentUserId;
					DocumentAttribute.AssignedUserId = WebSecurity.CurrentUserId;
					DocumentAttribute.CreatedDate = DateTime.Now;
					DocumentAttribute.ModifiedDate = DateTime.Now;
					DocumentAttribute.OrderNo = dem++;
					DocumentAttribute.File = Customer.Code + "(" + DocumentField.Id + "_" + DocumentAttribute.OrderNo + ")" + ".png";
					DocumentAttribute.Size = "";
					DocumentAttribute.TypeFile = "png";
					DocumentAttribute.DocumentFieldId = DocumentField.Id;
					DocumentAttributeRepository.InsertDocumentAttribute(DocumentAttribute);
					//Lưu hình
					string base64 = item.File.Substring(item.File.IndexOf(',') + 1);
					base64 = base64.Trim('\0');
					byte[] data = Convert.FromBase64String(base64);

					//convert to an image (or do whatever else you need to do)
					Image image;
					using (MemoryStream ms = new MemoryStream(data))
					{
						image = Image.FromStream(ms);
					}

					var customerImagePath = Helpers.Common.GetSetting("UsingServiceLogDetail");
					image.Save(Server.MapPath("~" + customerImagePath) + DocumentAttribute.File, System.Drawing.Imaging.ImageFormat.Png);

				}

				return RedirectToAction("_ClosePopup", "Home", new { area = "", FunctionCallback = "" });
			}

			TempData[Globals.FailedMessageKey] = "Vui lòng chụp ảnh!";
			return RedirectToAction("PhotoCustomer", new { Id = model.Id });
		}
		#endregion

		#region ProfileImage
		public ActionResult ProfileImage(int? CustomerId, int? ProductInvoiceId)
		{
			//đóng camera ở server.
			Erp.BackOffice.Hubs.ErpHub.ShowOrHiddenCamera(false, "hidden");
			if (CustomerId != null && CustomerId.Value > 0)
			{
				var customer = CustomerRepository.GetvwCustomerById(CustomerId.Value);
				if (customer != null)
				{
					var model = new CustomerViewModel();
					AutoMapper.Mapper.Map(customer, model);
					if (string.IsNullOrEmpty(model.Image))//Đã có hình
					{
						model.Image_Path = Erp.BackOffice.Helpers.Common.KiemTraTonTaiHinhAnh(model.Image, "Customer", "user");
					}
					ViewBag.profile_image_width = Erp.BackOffice.Helpers.Common.GetSetting("profile_image_width");
					ViewBag.profile_image_height = Erp.BackOffice.Helpers.Common.GetSetting("profile_image_height");

					ViewBag.SuccessMessage = TempData[Globals.SuccessMessageKey];
					ViewBag.FailedMessage = TempData[Globals.FailedMessageKey];
					return View(model);
				}
				else
				{
					ViewBag.NewPage = true;
					ViewBag.SuccessMessage = "Chưa có thông tin";
					return View();
				}
			}
			else
			{
				ViewBag.NewPage = true;
				ViewBag.SuccessMessage = TempData[Globals.SuccessMessageKey];
				ViewBag.FailedMessage = TempData[Globals.FailedMessageKey];
				return View();
			}
		}

		[HttpPost]
		public ActionResult ProfileImage(CustomerViewModel model)
		{
			if (Request["Submit"] == "Save")
			{
				var ImagePath = Helpers.Common.GetSetting("Customer");
				var customer = CustomerRepository.GetCustomerById(model.Id);
				var name = customer.Code + ".jpg";
				//Lưu hình từ chụp hình
				if (!string.IsNullOrEmpty(model.Image_File))
				{
					string base64 = model.Image_File.Substring(model.Image_File.IndexOf(',') + 1);
					base64 = base64.Trim('\0');
					byte[] data = Convert.FromBase64String(base64);

					//convert to an image (or do whatever else you need to do)
					Image image;
					using (MemoryStream ms = new MemoryStream(data))
					{
						image = Image.FromStream(ms);
					}

					image.Save(Path.Combine(Server.MapPath("~" + ImagePath), name), System.Drawing.Imaging.ImageFormat.Jpeg);
					image.Dispose();
					//var customer = CustomerRepository.GetCustomerById(model.Id);
					customer.ModifiedUserId = WebSecurity.CurrentUserId;
					customer.ModifiedDate = DateTime.Now;
					customer.Image = name;
					customer.ModifiedDate = DateTime.Now;
					CustomerRepository.UpdateCustomer(customer);
					TempData[Globals.SuccessMessageKey] = App_GlobalResources.Wording.UpdateSuccess;
				}
				else
				{
					TempData[Globals.FailedMessageKey] = "Không có hình! Không thể lưu";
					return View(model);
				}
			}

			return RedirectToAction("CameraInvoice", "Customer", new { area = "Account", ProductInvoiceId = Request["ProductInvoiceId"] });
		}

		#endregion

		#region CameraUsingService
		//public ActionResult CameraUsingService(int Id)
		//{
		//    var usinglogdetail = usingServiceLogdetailRepository.GetvwUsingServiceLogDetailById(Id);
		//    UsingServiceLogDetailViewModel model = null;
		//    // xóa file đính kèm trong session để bắt đầu thêm mới
		//    Session["file"] = null;
		//    if (usinglogdetail != null && usinglogdetail.IsDeleted != true)
		//    {
		//        model = new UsingServiceLogDetailViewModel();
		//        AutoMapper.Mapper.Map(usinglogdetail, model);
		//        ViewBag.SuccessMessage = TempData["SuccessMessage"];
		//        ViewBag.FailedMessage = TempData["FailedMessage"];
		//        ViewBag.AlertMessage = TempData["AlertMessage"];
		//        return View(model);
		//    }

		//    TempData[Globals.FailedMessageKey] = "Không tìm thấy thông tin khách hàng sử dụng dịch vụ/tái khám. Vui lòng kiểm tra lại!";
		//    return View(model);
		//}

		//[HttpPost]
		//public ActionResult CameraUsingService(UsingServiceLogDetailViewModel model)
		//{
		//    if (Request["Submit"] == "Save")
		//    {
		//        var usinglogdetail = usingServiceLogdetailRepository.GetvwUsingServiceLogDetailById(model.Id);
		//        var type = "";
		//        if (usinglogdetail.Type == "usedservice")
		//        {
		//            type = " sử dụng dịch vụ ngày " + usinglogdetail.CreatedDate.Value.ToString("dd/MM/yyyy HH:mm");
		//        }
		//        else
		//        {
		//            type = " tái khám ngày " + usinglogdetail.CreatedDate.Value.ToString("dd/MM/yyyy HH:mm");
		//        }
		//        string FailedMessageKey = "";
		//        //lưu file
		//        DocumentFieldController.SaveUpload(usinglogdetail.CustomerName + type, "", usinglogdetail.Id, "UsingServiceLogDetail", FailedMessageKey,usinglogdetail.CustomerCode);
		//        if (!string.IsNullOrEmpty(FailedMessageKey))
		//        {
		//            TempData[Globals.FailedMessageKey] = FailedMessageKey;
		//        }
		//        //send alert
		//        var user=Erp.BackOffice.Helpers.Common.CurrentUser;
		//        var list_User=userRepository.GetUserbyUserType("Admin chi nhánh").Where(x=>x.BranchId==user.BranchId).ToList();
		//        for (int i = 0; i < list_User.Count(); i++)
		//        {
		//            string titile = "";
		//            if (usinglogdetail.Type == "usedservice")
		//            {
		//                titile="Chụp hình thành công khách hàng " + usinglogdetail.CustomerName + " sử dụng DV: " + usinglogdetail.ServiceName;
		//            }
		//            else
		//            { 
		//                titile="Chụp hình thành công khách hàng " + usinglogdetail.CustomerName + " tái khám DV: " + usinglogdetail.ServiceName;
		//            }
		//            Erp.BackOffice.Hubs.ErpHub.SendAlerts(list_User[i].Id, user.Id,titile);
		//        }

		//        return RedirectToAction("CameraUsingService", "Customer", new { area = "Account", Id =model.Id});
		//    }
		//    TempData[Globals.FailedMessageKey] = "Vui lòng chụp ảnh!";
		//    return RedirectToAction("CameraUsingService", "Customer", new { area = "Account", Id = model.Id });
		//}
		#endregion

		#region ClientListUsingService
		public ActionResult ClientListUsingService(string txtCode, string txtCusName)
		{
			var usinglogdetail = usingServiceLogdetailRepository.GetAllvwUsingServiceLogDetail();
			var model = usinglogdetail.Select(item => new UsingServiceLogDetailViewModel
			{
				Id = item.Id,
				IsDeleted = item.IsDeleted,
				CreatedUserId = item.CreatedUserId,
				CreatedDate = item.CreatedDate,
				ModifiedUserId = item.ModifiedUserId,
				ModifiedDate = item.ModifiedDate,
				Code = item.Code,
				CustomerCode = item.CustomerCode,
				CustomerName = item.CustomerName,
				Status = item.Status,
				CustomerId = item.CustomerId,
				CustomerImage = item.CustomerImage,
				IsVote = item.IsVote,
				Name = item.Name,
				ProductInvoiceCode = item.ProductInvoiceCode,
				ServiceName = item.ServiceName,
				UsingServiceId = item.UsingServiceId,
				Type = item.Type,
				BranchId = item.BranchId
			}).OrderByDescending(m => m.CreatedDate).ToList();

			//if (!string.IsNullOrEmpty(Helpers.Common.CurrentUser.DrugStore))
			//{
			//    model = model.Where(x => ("," + Helpers.Common.CurrentUser.DrugStore + ",").Contains("," + x.BranchId + ",") == true).ToList();
			//}
			if (!string.IsNullOrEmpty(txtCode))
			{
				model = model.Where(x => Helpers.Common.ChuyenThanhKhongDau(x.Code).Contains(Helpers.Common.ChuyenThanhKhongDau(txtCode))).ToList();
			}

			if (!string.IsNullOrEmpty(txtCusName))
			{
				txtCusName = Helpers.Common.ChuyenThanhKhongDau(txtCusName);
				model = model.Where(x => Helpers.Common.ChuyenThanhKhongDau(x.CustomerName).Contains(txtCusName)).ToList();
			}
			model = model.Take(15).ToList();
			return View(model);
		}
		#endregion

		#region CameraInvoice
		public ActionResult CameraInvoice(int ProductInvoiceId)
		{
			var productInvoice = productInvoiceRepository.GetvwProductInvoiceById(ProductInvoiceId);
			ProductInvoiceViewModel model = null;
			// xóa file đính kèm trong session để bắt đầu thêm mới
			Session["file"] = null;
			if (productInvoice != null && productInvoice.IsDeleted != true)
			{
				model = new ProductInvoiceViewModel();
				AutoMapper.Mapper.Map(productInvoice, model);
				ViewBag.SuccessMessage = TempData["SuccessMessage"];
				ViewBag.FailedMessage = TempData["FailedMessage"];
				ViewBag.AlertMessage = TempData["AlertMessage"];
				return View(model);
			}

			TempData[Globals.FailedMessageKey] = "Không tìm thấy thông tin hóa đơn. Vui lòng kiểm tra lại!";
			return View(model);
		}

		//[HttpPost]
		//public ActionResult CameraInvoice(ProductInvoiceViewModel model)
		//{
		//    if (Request["Submit"] == "Save")
		//    {
		//        string FailedMessageKey = "";
		//        //lưu file
		//        DocumentFieldController.SaveUpload(model.Code, "", model.Id, "ProductInvoice", FailedMessageKey, model.Code);
		//        if (!string.IsNullOrEmpty(FailedMessageKey))
		//        {
		//            TempData[Globals.FailedMessageKey] = FailedMessageKey;
		//        }
		//        //send alert
		//        var user = Erp.BackOffice.Helpers.Common.CurrentUser;
		//        var list_User = userRepository.GetUserbyUserType("Admin chi nhánh").Where(x => x.BranchId == user.BranchId).ToList();
		//        for (int i = 0; i < list_User.Count(); i++)
		//        {
		//            string titile = "Chụp hình thành công hóa đơn " + model.Code + " của khách " + model.CustomerName;
		//            Erp.BackOffice.Hubs.ErpHub.SendAlerts(list_User[i].Id, user.Id, titile);
		//        }
		//        TempData[Globals.SuccessMessageKey] = "Lưu hình hóa đơn thành công!";
		//        return RedirectToAction("CameraInvoice", "Customer", new { area = "Account", ProductInvoiceId = model.Id });
		//    }
		//    TempData[Globals.FailedMessageKey] = "Vui lòng chụp ảnh!";
		//    return RedirectToAction("CameraInvoice", "Customer", new { area = "Account", ProductInvoiceId = model.Id });
		//}
		#endregion

		#region ListUserDrugStore

		public ViewResult ListNT(string CardCode, string txtCusName, string txtCode, string Phone, string ProvinceId, string DistrictId, string WardId)
		{
			IEnumerable<CustomerNTViewModel> q = CustomerRepository.GetAllvwCustomer()
				 .Where(x => x.CustomerType == "DrugStore").AsEnumerable()
				//Where(x => x.BranchId == Helpers.Common.CurrentUser.BranchId)
				.Select(item => new CustomerNTViewModel
				{
					Id = item.Id,
					CreatedUserId = item.CreatedUserId,
					//CreatedUserName = item.CreatedUserName,
					CreatedDate = item.CreatedDate,
					ModifiedUserId = item.ModifiedUserId,
					//ModifiedUserName = item.ModifiedUserName,
					ModifiedDate = item.ModifiedDate,
					Code = item.Code,
					CompanyName = item.CompanyName,
					Phone = item.Phone,
					Address = item.Address,
					Note = item.Note,
					CardCode = item.CardCode,
					SearchFullName = item.SearchFullName,
					Image = item.Image,
					Birthday = item.Birthday,
					Gender = item.Gender,
					IdCardDate = item.IdCardDate,
					IdCardIssued = item.IdCardIssued,
					IdCardNumber = item.IdCardNumber,
					CardIssuedName = item.CardIssuedName,
					WardName = item.WardName,
					ProvinceName = item.ProvinceName,
					DistrictName = item.DistrictName,
					Mobile = item.Mobile,
					Email = item.Email,
					BranchId = item.BranchId,
					Image_File = item.Image,
					GenderName = item.GenderName,
					UserName = item.UserName

				});

			bool hasSearch = false;
			if (!Filters.SecurityFilter.IsAdmin())
			{
				//q=q.Where(x=>("," + Helpers.Common.CurrentUser.DrugStore + ",").Contains("," + x.BranchId + ",") == true );
				hasSearch = true;
			}
			if (!string.IsNullOrEmpty(CardCode))
			{
				q = q.Where(x => x.CardCode.Contains(CardCode));
				hasSearch = true;
			}

			if (!string.IsNullOrEmpty(txtCusName))
			{
				txtCusName = Helpers.Common.ChuyenThanhKhongDau(txtCusName);
				q = q.Where(x => Helpers.Common.ChuyenThanhKhongDau(x.FullName).Contains(txtCusName));
				hasSearch = true;
			}

			if (!string.IsNullOrEmpty(txtCode))
			{
				q = q.Where(x => x.Code.Contains(txtCode));
				hasSearch = true;
			}
			if (!string.IsNullOrEmpty(Phone))
			{
				q = q.Where(x => x.Phone.Contains(Phone));
				hasSearch = true;
			}
			if (!string.IsNullOrEmpty(ProvinceId))
			{
				q = q.Where(item => item.CityId == ProvinceId);
				hasSearch = true;
			}
			if (!string.IsNullOrEmpty(DistrictId))
			{
				q = q.Where(item => item.DistrictId == DistrictId);
				hasSearch = true;
			}
			if (!string.IsNullOrEmpty(WardId))
			{
				q = q.Where(item => item.WardId == WardId);
				hasSearch = true;
			}

			if (hasSearch)
			{
				q = q.OrderByDescending(m => m.CompanyName);
				hasSearch = true;
			}
			else
			{
				q = null;
			}

			ViewBag.SuccessMessage = TempData["SuccessMessage"];
			ViewBag.FailedMessage = TempData["FailedMessage"];
			ViewBag.AlertMessage = TempData["AlertMessage"];
			return View(q);
		}
		#endregion

		#region CreateNT
		public ViewResult CreateNT(string Phone, string Code, string CompanyName, string FirstName, string LastName, string FullName, string Address, string cus_crm, int? LeadId)
		{
			//get cookie brachID 
			HttpRequestBase request = this.HttpContext.Request;
			string strBrandID = "0";
			if (request.Cookies["BRANCH_ID_SPA_CookieName"] != null)
			{
				strBrandID = request.Cookies["BRANCH_ID_SPA_CookieName"].Value;
				if (strBrandID == "")
				{
					strBrandID = "0";
				}
			}

			//get  CurrentUser.branchId

			if ((Helpers.Common.CurrentUser.BranchId != null) && (Helpers.Common.CurrentUser.BranchId > 0))
			{
				strBrandID = Helpers.Common.CurrentUser.BranchId.ToString();
			}

			int? intBrandID = int.Parse(strBrandID);

			var model = new CustomerViewModel();
			model.Phone = Phone;
			model.FirstName = FirstName;
			model.LastName = LastName;
			model.CompanyName = CompanyName;
			model.FullName = FullName;
			model.Address = Address;
			model.cus_crm = cus_crm;
			model.BranchId = intBrandID;
			var checkIsLetan = SqlHelper.QuerySP<int>("checkUserIsLetan", new
			{
				@pId = WebSecurity.CurrentUserId
			}).ToList();
			var leaddt = SqlHelper.QuerySP<LeadModel>("LoadLeadIndex", new
			{
				@pId = WebSecurity.CurrentUserId,
				@pIsLeTan = (checkIsLetan.Count > 0) ? 1 : 0
			});
			var ldt = leaddt.Where(x => x.Id == LeadId).FirstOrDefault();
			if (ldt != null)
			{
				model.Email = ldt.Email;
				model.LeadId = ldt.Id;
				model.Address = ldt.Address;
				model.Phone = ldt.Mobile;
				model.LastName = ldt.Name;
				model.FirstName = ldt.LeadName;
				model.ManagerStaffId = ldt.ReceptionStaffId;

			}
			return View(model);
		}

		[HttpPost]
		public ActionResult CreateNT(CustomerViewModel model)
		{
			if (ModelState.IsValid)
			{
				//  Kiểm tra trùng tên số điện thoại thì không cho lưu
				var c = CustomerRepository.GetAllvwCustomer()
					.Where(item => item.Phone == model.Phone).FirstOrDefault();

				if (c != null)
				{
					if ((c != null) && (c.isLock == true) && model.Phone != null)
					{
						TempData[Globals.FailedMessageKey] = "Khách hàng này đã bị trùng số điện thoại với khách hàng ngừng hoạt động.";
						ViewBag.FailedMessage = TempData["FailedMessage"];


						return View(model);
						//return RedirectToAction("CreateNT", new { Id = c.Id });
					}
					else if (model.Phone != null)
					{
						TempData[Globals.FailedMessageKey] = "Khách hàng này đã bị trùng số điện thoại với khách hàng khác, vui lòng kiểm tra lại!";
						ViewBag.FailedMessage = TempData["FailedMessage"];

						return View(model);
						//return RedirectToAction("CreateNT", new { Id = c.Id });
					}
				}


				var cus = new Customer();

				using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.Snapshot }))
				{



					AutoMapper.Mapper.Map(model, cus);
					cus.IsDeleted = false;
					cus.CreatedUserId = WebSecurity.CurrentUserId;
					cus.ModifiedUserId = WebSecurity.CurrentUserId;
					cus.CreatedDate = DateTime.Now;
					cus.ModifiedDate = DateTime.Now;
					cus.CustomerGroup = model.CustomerGroup;
					cus.LeadId = model.LeadId;
					cus.isLead = cus.LeadId > 0 ? true : false;
					cus.CompanyName = model.FirstName + " " + model.LastName;

					//lấy mã.
					CustomerRepository.InsertCustomer(cus);

					//begin kiem tra trung ma khach hang
					cus.Code = Erp.BackOffice.Helpers.Common.GetOrderNo("Customer", model.Code);
					c = CustomerRepository.GetAllvwCustomer()
				   .Where(item => item.Code == cus.Code).FirstOrDefault();

					if (c != null)
					{
						TempData[Globals.FailedMessageKey] = "Khách hàng này đã bị trùng mã khách hàng với khách hàng khác, vui lòng kiểm tra lại!";
						ViewBag.FailedMessage = TempData["FailedMessage"];
						return View(model);
					}

					//end kiem tra trung ma khach hang

					CustomerRepository.UpdateCustomer(cus);





					Erp.BackOffice.Helpers.Common.SetOrderNo("Customer");

					//ghi log 
					Erp.BackOffice.Controllers.HomeController.WriteLog(cus.Id, cus.Code, "đã tạo khách hàng", "Customer/Detail/" + cus.Id, Helpers.Common.CurrentUser.BranchId.Value);

					scope.Complete();

				}
				var userDN = userRepository.GetUserById(WebSecurity.CurrentUserId);
				var path = System.Web.HttpContext.Current.Server.MapPath("~" + Helpers.Common.GetSetting("Customer"));
				if (Request.Files["file-image"] != null)
				{
					var file = Request.Files["file-image"];
					if (file.ContentLength > 0)
					{
						string image_name = cus.Code + "." + file.FileName.Split('.').Last();
						bool isExists = System.IO.Directory.Exists(path);
						if (!isExists)
							System.IO.Directory.CreateDirectory(path);
						file.SaveAs(path + image_name);
						cus.Image = image_name;
						CustomerRepository.UpdateCustomer(cus);
					}

				}

				ViewBag.SuccessMessage = App_GlobalResources.Wording.InsertSuccess;
				if (Request["IsPopup"] == "true" || Request["IsPopup"] == "True")
				{
					ViewBag.closePopup = "close and append to page parent";
					model.FullName = model.LastName + " " + model.FirstName;
					model.Id = cus.Id;
					return View(model);
				}
				return RedirectToAction("Detail", new { Id = cus.Id });

			}
			return View(model);
		}
		#endregion

		#region EditNT
		public ActionResult EditNT(int? Id)
		{
			//get cookie brachID 
			HttpRequestBase request = this.HttpContext.Request;
			string strBrandID = "0";
			if (request.Cookies["BRANCH_ID_SPA_CookieName"] != null)
			{
				strBrandID = request.Cookies["BRANCH_ID_SPA_CookieName"].Value;
				if (strBrandID == "")
				{
					strBrandID = "0";
				}
			}

			//get  CurrentUser.branchId

			if ((Helpers.Common.CurrentUser.BranchId != null) && (Helpers.Common.CurrentUser.BranchId > 0))
			{
				strBrandID = Helpers.Common.CurrentUser.BranchId.ToString();
			}

			int? intBrandID = int.Parse(strBrandID);

			IEnumerable<CustomerViewModel> q = CustomerRepository.GetAllvwCustomer().AsEnumerable().Where(x => x.BranchId == intBrandID && (x.isLock == false || x.isLock == null))
			.Select(item => new CustomerViewModel
			{
				Id = item.Id,
				Code = item.Code,
				CompanyName = item.CompanyName,
				BranchId = item.BranchId
			});
			ViewBag.CustomerList = q;
			var Staffs = CustomerRepository.GetvwCustomerById(Id.Value);
			if (Staffs != null && Staffs.IsDeleted != true)
			{
				var model = new CustomerViewModel();
				AutoMapper.Mapper.Map(Staffs, model);

				//lấy danh sách CustomerUser               
				model.CustomerUserList = GetCustomerUserListByCustomerId(Id.Value);
				model.CustomerRecommendList = GetCustomerRecommendListByCustomerId(Id.Value);

				//////
				ViewBag.SuccessMessage = TempData["SuccessMessage"];
				ViewBag.FailedMessage = TempData["FailedMessage"];
				return View(model);
			}

			if (Request.UrlReferrer != null)
				return Redirect(Request.UrlReferrer.AbsoluteUri);
			return RedirectToAction("Index");
		}
		[HttpPost]
		public ActionResult EditNT(CustomerViewModel model)
		{


			var Customer = new Customer();
			if (!ModelState.IsValid)
			{
				var errors = ModelState.Values.SelectMany(v => v.Errors);
			}
			if (ModelState.IsValid)
			{
				if (Request["Submit"] == "Save")
				{
					var cus = CustomerRepository.GetCustomerById(model.Id);
					if (model.isLock == true)
					{
						if (model.isLock != cus.isLock)
						{
							model.ModifiedIsLock = DateTime.Now;
						}

					}
					AutoMapper.Mapper.Map(model, cus);
					cus.ModifiedUserId = WebSecurity.CurrentUserId;
					cus.ModifiedDate = DateTime.Now;
					cus.CompanyName = model.FirstName + " " + model.LastName;
					cus.EconomicStatus = model.EconomicStatus;
					cus.CustomerGroup = model.CustomerGroup;
					cus.LeadId = model.LeadId;
					var path = Helpers.Common.GetSetting("Customer");

					var filepath = System.Web.HttpContext.Current.Server.MapPath("~" + path);
					if (Request.Files["file-image"] != null)
					{
						var file = Request.Files["file-image"];
						if (file.ContentLength > 0)
						{
							FileInfo fi = new FileInfo(Server.MapPath("~" + path) + cus.Image);
							if (fi.Exists)
							{
								fi.Delete();
							}

							string image_name = cus.Code + "." + file.FileName.Split('.').Last();

							bool isExists = System.IO.Directory.Exists(filepath);
							if (!isExists)
								System.IO.Directory.CreateDirectory(filepath);
							file.SaveAs(filepath + image_name);
							cus.Image = image_name;
						}
					}
					int? NguoiLapOld = 0;
					if (cus != null && cus.ManagerStaffId != model.ManagerStaffId)
					{
						NguoiLapOld = cus.ManagerStaffId;
						var data = SqlHelper.QuerySP<TransferCustomerViewModel>("Transfer_CustomerGroups", new
						{
							CustomerID = cus.Id,
							NguoiLapIdNew = model.ManagerStaffId,

						}).ToList();
						//IEnumerable<SelectListItem> OriginList = Erp.BackOffice.Helpers.SelectListHelper.GetSelectList_Category("Origin", null, null);
						//if (OriginList.Count() > 0)
						//{
						//    foreach (var itemBrand in OriginList)
						//    {
						//        var data = SqlHelper.QuerySP<TransferCustomerViewModel>("Transfer_CustomerGroups", new
						//        {
						//            CustomerID = cus.Id,
						//            NguoiLapIdNew = model.ManagerStaffId,
						//            NameForBrand = itemBrand.Value

						//        }).ToList();
						//    }
						//}

					}
					var checkKHKT = iKH_TUONGTACRepository.GetallKH_TuongTacByManager(cus.Id, NguoiLapOld);
					if (checkKHKT.Count() > 0)
					{
						foreach (var itemkt in checkKHKT)
						{
							itemkt.NGUOILAP_ID = model.ManagerStaffId;
							iKH_TUONGTACRepository.UpdateKH_TUONGTAC(itemkt);
						}
					}
					if (model.isLock == true)
					{
						if (cus.ModifiedIsLock == null)
						{
							cus.ModifiedIsLock = DateTime.Now;
						}
					}

					CustomerRepository.UpdateCustomer(cus);


					//ghi log 
					Erp.BackOffice.Controllers.HomeController.WriteLog(cus.Id, cus.Code, "đã cập nhật khách hàng", "Customer/Detail/" + cus.Id, Helpers.Common.CurrentUser.BranchId.Value);

					if (Request["IsPopup"] == "true" || Request["IsPopup"] == "True")
					{
						return RedirectToAction("_ClosePopup", "Home", new { area = "", FunctionCallback = "ClosePopupAndReloadPage" });
					}
					else
					{
						TempData[Globals.SuccessMessageKey] = App_GlobalResources.Wording.UpdateSuccess;
						//gọi JavaScriptToRun để chạy code Jquery trong View
						model.JavaScriptToRun = "CloseCurrentPopup(" + cus.Id + ")";

						//lấy danh sách CustomerUser
						model.CustomerUserList = GetCustomerUserListByCustomerId(model.Id);
						model.CustomerRecommendList = GetCustomerRecommendListByCustomerId(model.Id);

						return RedirectToAction("Detail", new { Id = model.Id });


					}
					//return RedirectToAction("Detail", "Customer", new { area = "Account", Id = cus.Id, IsLayout = false });
					//return RedirectToAction("Detail", "Customer", new { area = "", FunctionCallback = "ClosePopupAndReloadPage" });

				}


				////lấy value của button name=Delete đang request
				string idDelete = Request["Delete"];
				if (idDelete != null && Request["Delete"] == idDelete)
				{
					DeleteCustomerUser(idDelete, model.Id);

					return RedirectToAction("Detail", new { Id = model.Id });

				}

				return RedirectToAction("Detail", new { Id = model.Id });
			}
			if (Request.UrlReferrer != null)
				return Redirect(Request.UrlReferrer.AbsoluteUri);
			return RedirectToAction("Detail", new { Id = model.Id });
		}
		[HttpPost]
		public PartialViewResult SaveTab4(CustomerViewModel model)
		{

			//truyền dữ liệu sang InsertCusUser
			InsertCusUser(model.ManagerStaffId, model.Id, model.StartDate);
			// NguoiLapOld = cus.ManagerStaffId;
			SqlHelper.QuerySP<TransferCustomerViewModel>("Transfer_CustomerGroups", new
			{
				CustomerID = model.Id,
				NguoiLapIdNew = model.ManagerStaffId,

			}).ToList();
			var data = GetCustomerUserListByCustomerId(model.Id);
			ViewBag.CustomerUserList = data;
			return PartialView("GridPartial", 0);
		}
		[HttpPost]
		public PartialViewResult SaveTab5(CustomerViewModel model)
		{
			InsertCustomerRecommend(model.IdCustomer_Gioithieu, model.Id, model._StartDate);
			var data = GetCustomerRecommendListByCustomerId(model.Id);
			ViewBag.CustomerRecommendList = data;
			return PartialView("GridPartial", 1);
		}
		[HttpPost]
		public ActionResult SaveTab1(CustomerViewModel model)
		{
			var path = Helpers.Common.GetSetting("Customer");

			var filepath = System.Web.HttpContext.Current.Server.MapPath("~" + path);
			if (Request.Files["file-image"] != null)
			{
				var file = Request.Files["file-image"];
				if (file.ContentLength > 0)
				{
					FileInfo fi = new FileInfo(Server.MapPath("~" + path) + model.Image);
					if (fi.Exists)
					{
						fi.Delete();
					}

					string image_name = model.Code + "." + file.FileName.Split('.').Last();

					bool isExists = System.IO.Directory.Exists(filepath);
					if (!isExists)
						System.IO.Directory.CreateDirectory(filepath);
					file.SaveAs(filepath + image_name);
					model.Image = image_name;
				}
			}
			int kq = SqlHelper.ExecuteSP("SaveTab1", new
			{
				BranchId = model.BranchId,
				tenNguonkhach = model.tenNguonkhach,
				FirstName = model.FirstName,
				LastName = model.LastName,
				NhomHuongDS = model.NhomHuongDS,
				Birthday = model.Birthday,
				Gender = model.Gender,
				Note = model.Note,
				cus_crm = model.cus_crm,
				Image = model.Image,
				Id = model.Id,
				isLead = model.isLead == true ? 1 : 0,
				@UserId = WebSecurity.CurrentUserId

			});
			//ghi log 
			if (kq == 1)
				Erp.BackOffice.Controllers.HomeController.WriteLog(model.Id, model.Code, "đã cập nhật khách hàng", "Customer/Detail/" + model.Id, Helpers.Common.CurrentUser.BranchId.Value);

			return Json(kq);
		}
		[HttpPost]
		public ActionResult SaveTab2(CustomerViewModel model)
		{

			int kq = SqlHelper.ExecuteSP("SaveTab2", new
			{
				CityId = model.CityId,
				DistrictId = model.DistrictId,
				WardId = model.WardId,
				Address = model.Address,
				Phone = model.Phone,
				Mobile = model.Mobile,
				Email = model.Email,
				Id = model.Id,
				isLead = model.isLead == true ? 1 : 0,
				@UserId = WebSecurity.CurrentUserId

			});
			//ghi log 
			if (kq == 1)
				Erp.BackOffice.Controllers.HomeController.WriteLog(model.Id, model.Code, "đã cập nhật khách hàng", "Customer/Detail/" + model.Id, Helpers.Common.CurrentUser.BranchId.Value);

			return Json(kq);
		}
		[HttpPost]
		public ActionResult SaveTab3(CustomerViewModel model)
		{

			int kq = SqlHelper.ExecuteSP("SaveTab3", new
			{
				CustomerGroup = model.CustomerGroup,
				CustomerType = model.CustomerType,
				EconomicStatus = model.EconomicStatus,
				SkinSkinLevel = model.SkinSkinLevel,
				HairlLevel = model.HairlLevel,
				GladLevel = model.GladLevel,
				KhCuMuonBo = model.KhCuMuonBo,
				KhCuThanPhien = model.KhCuThanPhien,
				KhLauNgayKhongTuongTac = model.KhLauNgayKhongTuongTac,
				KhMoiDenVaHuaQuaiLai = model.KhMoiDenVaHuaQuaiLai,
				KhMoiDenVaKinhTeYeu = model.KhMoiDenVaKinhTeYeu,
				IsBonusSales = model.IsBonusSales,
				isLock = model.isLock,
				Id = model.Id,
				isLead = model.isLead == true ? 1 : 0,
				@UserId = WebSecurity.CurrentUserId
			});
			//ghi log 
			if (kq == 1)
				Erp.BackOffice.Controllers.HomeController.WriteLog(model.Id, model.Code, "đã cập nhật khách hàng", "Customer/Detail/" + model.Id, Helpers.Common.CurrentUser.BranchId.Value);

			return Json(kq);
		}
		#endregion

		private bool CheckUsernameExists(string username)
		{
			var user = userRepository.GetByUserName(username);

			return user != null;
		}

		public ActionResult DetailBasicFull(int? Id)
		{
			var student = CustomerRepository.GetvwCustomerById(Id.Value);
			if (student != null)
			{
				var model = new CustomerNTViewModel();
				AutoMapper.Mapper.Map(student, model);
				return View(model);
			}

			if (Request.UrlReferrer != null)
				return Redirect(Request.UrlReferrer.AbsoluteUri);
			return RedirectToAction("Index");
		}

		public ActionResult Contact() { return View(); }

		#region InsertCusUser
		private void InsertCusUser(int? Id, int CustomerId, DateTime? StartDate)
		{
			var customer_user = CustomerUserRepository.GetAllCustomerUserByCustomerId(CustomerId).ToList();
			var customer = CustomerRepository.GetCustomerById(CustomerId);

			//lấy CustomerUser được thêm gần đây nhất nằm cuối danh sách customer_user 
			DateTime startDate = Convert.ToDateTime(StartDate);
			DateTime LastEndDate = startDate.AddDays(-1);
			var LastCustomerUser = new CustomerUser();

			if (customer_user.Count != 0)
			{   //Update End Date của CustomerUser nằm cuối list customer_user
				LastCustomerUser = customer_user.Last();
				if (StartDate > LastCustomerUser.StartDate)
				{
					if (LastCustomerUser.EndDate == null)
					{

						LastCustomerUser.EndDate = LastEndDate;
						CustomerUserRepository.UpdateCustomerUser(LastCustomerUser);
					}

					//tạo mới CustomerUser
					CreateCusUser(Id, CustomerId, StartDate);

					customer.ManagerStaffId = Id;
					CustomerRepository.UpdateCustomer(customer);
				}
				else
				{
					//CustomerUserRepository.GetCustomerUserById(customer_user.Last().Id);
					TempData[Globals.FailedMessageKey] = "Ngày bắt đầu không được nhỏ hơn hay bằng ngày bắt đầu mới nhất trong danh sách";
				}
			}
			else
			{
				//Tạo mới CustomerUser
				CreateCusUser(Id, CustomerId, StartDate);

				customer.ManagerStaffId = Id;
				CustomerRepository.UpdateCustomer(customer);

			}

		}
		#endregion

		private void CreateCusUser(int? Id, int CustomerId, DateTime? StartDate)
		{
			#region CreateCusUser
			var _insert_customeruser = new CustomerUser();
			_insert_customeruser.IsDeleted = false;
			_insert_customeruser.CreatedUserId = WebSecurity.CurrentUserId;
			_insert_customeruser.ModifiedUserId = WebSecurity.CurrentUserId;
			_insert_customeruser.AssignedUserId = Id;
			_insert_customeruser.CreatedDate = DateTime.Now;
			_insert_customeruser.ModifiedDate = DateTime.Now;
			//_insert_customeruser.Name = item.Name;
			//_insert_customeruser.Description = item.Description;
			_insert_customeruser.StartDate = StartDate;
			//_insert_customeruser.EndDate = item.EndDate;
			_insert_customeruser.CustomerId = CustomerId;
			CustomerUserRepository.InsertCustomerUser(_insert_customeruser);
			TempData[Globals.SuccessMessageKey] = App_GlobalResources.Wording.InsertSuccess;
			#endregion
		}

		#region DeleteCustomerUser
		private void DeleteCustomerUser(string idDelete, int CustomerId)
		{
			try
			{
				//string idDelete = Request["Delete"];

				var item = CustomerUserRepository.GetCustomerUserById(int.Parse(idDelete, CultureInfo.InvariantCulture));
				if (item != null)
				{
					CustomerUserRepository.DeleteCustomerUser(item.Id);
				}
				//Lấy list mới sau khi Delete và update Customer
				var customer_user = CustomerUserRepository.GetAllCustomerUserByCustomerId(CustomerId).ToList();

				if (customer_user.Count > 0)
				{
					var LastCustomerUser = customer_user.Last();

					if (CustomerId == LastCustomerUser.CustomerId)
					{
						var customer = CustomerRepository.GetCustomerById(CustomerId);
						customer.ManagerStaffId = LastCustomerUser.AssignedUserId;
						CustomerRepository.UpdateCustomer(customer);
					}
				}
				else
				{
					var customer = CustomerRepository.GetCustomerById(CustomerId);
					customer.ManagerStaffId = null;
					CustomerRepository.UpdateCustomer(customer);
				}

				TempData[Globals.SuccessMessageKey] = App_GlobalResources.Wording.DeleteSuccess;
			}
			catch (DbUpdateException)
			{
				TempData[Globals.FailedMessageKey] = App_GlobalResources.Error.RelationError;
			}
		}
		#endregion

		private List<CustomerUserViewModel> GetCustomerUserListByCustomerId(int? Id)
		{
			#region GetCustomerUserListByCustomerId
			var vwcustomer_user = SqlHelper.QuerySP<CustomerUserViewModel>("GetAllvwCustomerUserByCustomerId", new { @CustomerId = Id.Value }).ToList();
			var cusUser = vwcustomer_user;

			return cusUser;

			#endregion
		}



		#region InsertCustomerRecommend
		private void InsertCustomerRecommend(int CustomerId_new, int CustomerId, DateTime? _StartDate)
		{
			var customer_user = CustomerRepository.GetAllCustomerRecommendByCustomerId(CustomerId).ToList();
			var customer = CustomerRepository.GetCustomerById(CustomerId);

			//lấy CustomerUser được thêm gần đây nhất nằm cuối danh sách customer_user 
			DateTime startDate = Convert.ToDateTime(_StartDate);
			DateTime LastEndDate = startDate.AddDays(-1);
			var LastCustomerUser = new CustomerRecommend();

			if (customer_user.Count != 0)
			{   //Update End Date của CustomerUser nằm cuối list customer_user
				LastCustomerUser = customer_user.Last();
				if (_StartDate > LastCustomerUser.StartDate)
				{

					CustomerRepository.UpdateCustomerRecommend(LastCustomerUser);


					//tạo mới CustomerUser
					CreateCustomerRecommend(CustomerId_new, CustomerId, _StartDate);

					customer.IdCustomer_Gioithieu = CustomerId_new;
					CustomerRepository.UpdateCustomer(customer);
				}
				else
				{
					//CustomerUserRepository.GetCustomerUserById(customer_user.Last().Id);
					TempData[Globals.FailedMessageKey] = "Ngày bắt đầu không được nhỏ hơn hay bằng ngày bắt đầu mới nhất trong danh sách";
				}
			}
			else
			{
				//Tạo mới CustomerUser
				CreateCustomerRecommend(CustomerId_new, CustomerId, _StartDate);

				//customer.ManagerStaffId = Id;
				CustomerRepository.UpdateCustomer(customer);

			}

		}
		#endregion

		private void CreateCustomerRecommend(int CustomerId_new, int CustomerId, DateTime? _StartDate)
		{
			#region CreateCusUser
			var _insert_customeruser = new CustomerRecommend();
			_insert_customeruser.IsDeleted = false;
			_insert_customeruser.CreatedUserId = WebSecurity.CurrentUserId;
			_insert_customeruser.ModifiedUserId = WebSecurity.CurrentUserId;
			//_insert_customeruser.AssignedUserId = WebSecurity.CurrentUserId;
			_insert_customeruser.CreatedDate = DateTime.Now;
			_insert_customeruser.ModifiedDate = DateTime.Now;
			//_insert_customeruser.Name = item.Name;
			//_insert_customeruser.Description = item.Description;
			_insert_customeruser.StartDate = _StartDate;
			//_insert_customeruser.EndDate = item.EndDate;
			_insert_customeruser.CustomerId = CustomerId;
			_insert_customeruser.CustomerId_new = CustomerId_new;

			CustomerRepository.InsertCustomerRecommend(_insert_customeruser);
			TempData[Globals.SuccessMessageKey] = App_GlobalResources.Wording.InsertSuccess;
			#endregion
		}



		private List<CustomerRecommendViewModel> GetCustomerRecommendListByCustomerId(int? Id)
		{
			#region GetCustomerUserListByCustomerId
			var vwcustomer_user = SqlHelper.QuerySP<CustomerRecommendViewModel>("GetCustomerRecommendListByCustomerId", new { @CustomerId = Id.Value }).ToList();

			//var cus=CustomerRepository.get
			var cusUser = vwcustomer_user;
			return cusUser;

			#endregion
		}



		#region lấy ds khách hàng quản lí của nhân viên
		public ActionResult GetCusOfManager(int? id)
		{
			var user = userRepository.GetvwUserById(id.Value);
			var cus = CustomerRepository.GetAllvwCustomer().Where(x => x.ManagerStaffId == id.Value).Select(item => new CustomerViewModel
			{
				Id = item.Id,
				CreatedUserId = item.CreatedUserId,
				CreatedDate = item.CreatedDate,
				ModifiedUserId = item.ModifiedUserId,
				ModifiedDate = item.ModifiedDate,
				Code = item.Code,
				CompanyName = item.CompanyName,
				Phone = item.Phone,
				Address = item.Address,
				Note = item.Note,
				CardCode = item.CardCode,

				Birthday = item.Birthday,
				Email = item.Email,
				Phoneghep = item.Phoneghep,
				Gender = item.Gender,
				IdCardDate = item.IdCardDate,
				IdCardIssued = item.IdCardIssued,
				IdCardNumber = item.IdCardNumber,
				CardIssuedName = item.CardIssuedName,
				BranchName = item.BranchName,
				ManagerStaffName = item.ManagerStaffName,
				ManagerUserName = item.ManagerUserName,
				CustomerType = item.CustomerType,
				isLock = item.isLock,
				BranchId = item.BranchId
			}).ToList();

			ViewBag.UserName = user.FullName;

			return View(cus);
		}


		#endregion


		#region Chuyển người quản lí
		public ActionResult TransferManager(int? SalerId, int? NhomHuongDS, string txtCusInfo)
		{
			//get cookie brachID 
			HttpRequestBase request = this.HttpContext.Request;
			string strBrandID = "0";
			if (request.Cookies["BRANCH_ID_SPA_CookieName"] != null)
			{
				strBrandID = request.Cookies["BRANCH_ID_SPA_CookieName"].Value;
				if (strBrandID == "")
				{
					strBrandID = "0";
				}
			}

			//get  CurrentUser.branchId

			if ((Helpers.Common.CurrentUser.BranchId != null) && (Helpers.Common.CurrentUser.BranchId > 0))
			{
				strBrandID = Helpers.Common.CurrentUser.BranchId.ToString();
			}

			int? intBrandID = int.Parse(strBrandID);

			IEnumerable<CustomerViewModel> q = CustomerRepository.GetAllvwCustomer().AsEnumerable()
				   .Select(item => new CustomerViewModel
				   {
					   Id = item.Id,
					   CreatedUserId = item.CreatedUserId,
					   CreatedDate = item.CreatedDate,
					   ModifiedUserId = item.ModifiedUserId,
					   ModifiedDate = item.ModifiedDate,
					   Code = item.Code,
					   CompanyName = item.CompanyName,
					   Phone = item.Phone,
					   Address = item.Address,
					   Note = item.Note,
					   CardCode = item.CardCode,
					   Image = item.Image,
					   Birthday = item.Birthday,
					   Email = item.Email,
					   Phoneghep = item.Phoneghep,
					   Gender = item.Gender,
					   IdCardDate = item.IdCardDate,
					   IdCardIssued = item.IdCardIssued,
					   IdCardNumber = item.IdCardNumber,
					   CardIssuedName = item.CardIssuedName,
					   BranchName = item.BranchName,
					   ManagerStaffName = item.ManagerStaffName,
					   ManagerUserName = item.ManagerUserName,
					   CustomerType = item.CustomerType,
					   isLock = item.isLock,
					   BranchId = item.BranchId,
					   NhomHuongDS = item.NhomHuongDS,
					   ManagerStaffId = item.ManagerStaffId
				   }).ToList();

			if (intBrandID > 0)
			{
				q = q.Where(x => x.BranchId == intBrandID).ToList();
			}
			if (SalerId > 0 && SalerId != null)
			{
				q = q.Where(x => x.ManagerStaffId == SalerId).ToList();
			}
			if (!string.IsNullOrEmpty(txtCusInfo))
			{
				q = q.Where(x => Helpers.Common.ChuyenThanhKhongDau(x.CompanyName).Contains(Helpers.Common.ChuyenThanhKhongDau(txtCusInfo)) || x.Code.Contains(txtCusInfo)).ToList();

			}
			return View(q);
		}


		[HttpPost]
		public ActionResult TransferManager(int? SalerId2, int? GroupSaler2)
		{
			using (var scope = new TransactionScope(TransactionScopeOption.Required,
							   new System.TimeSpan(0, 30, 0)))
			{
				try
				{
					string SalerId = Request["SalerId2"];
					if (SalerId != null && SalerId != "")
					{
						SalerId2 = int.Parse(SalerId, CultureInfo.InvariantCulture);
					}
					string GroupSaler = Request["GroupSaler"];
					if (GroupSaler != null && GroupSaler != "")
					{
						GroupSaler2 = int.Parse(GroupSaler, CultureInfo.InvariantCulture);
					}
					string idDeleteAll = Request["DeleteId-checkbox"];
					//string gdall = Request["selectgd"];
					string[] arrDeleteId = idDeleteAll.Split(',');
					for (int i = 0; i < arrDeleteId.Count(); i++)
					{
						var cus = CustomerRepository.GetCustomerById(int.Parse(arrDeleteId[i], CultureInfo.InvariantCulture));

						if (cus != null)
						{
							//IEnumerable<SelectListItem> OriginList = Erp.BackOffice.Helpers.SelectListHelper.GetSelectList_Category("Origin", null, null);
							//if (OriginList.Count() > 0)
							//{
							//    foreach (var itemBrand in OriginList)
							//    {
							//        var data = SqlHelper.QuerySP<TransferCustomerViewModel>("Transfer_CustomerGroups", new
							//        {
							//            CustomerID = cus.Id,
							//            NguoiLapIdNew = int.Parse(SalerId),
							//            NameForBrand = itemBrand.Value

							//        }).ToList();
							//    }
							//}
							var data = SqlHelper.QuerySP<TransferCustomerViewModel>("Transfer_CustomerGroups", new
							{
								CustomerID = cus.Id,
								NguoiLapIdNew = int.Parse(SalerId),

							}).ToList();
						}

						if (SalerId2 != null)
						{
							cus.ManagerStaffId = SalerId2;
							CustomerRepository.UpdateCustomer(cus);
							InsertCusUser(SalerId2, int.Parse(arrDeleteId[i], CultureInfo.InvariantCulture), DateTime.Now);
						}
						if (GroupSaler2 != null)
						{
							cus.NhomHuongDS = GroupSaler2;
							CustomerRepository.UpdateCustomer(cus);
						}
					}
					scope.Complete();

					return RedirectToAction("_ClosePopup", "Home", new { area = "", FunctionCallback = "ClosePopupAndReloadPage" });
				}
				catch (DbUpdateException)
				{
					TempData[Globals.FailedMessageKey] = App_GlobalResources.Error.RelationError;
					return RedirectToAction("Index");
				}
			}
		}
		#endregion



		[HttpPost]
		[ValidateInput(false)]
		public JsonResult Create_POS(CustomerViewModel model)
		{
			try
			{
				//begin hoapd copy code insert
				//get cookie brachID 
				//string pMA_DVIQLY = Helpers.Common.CurrentUser.MA_DVIQLY;
				HttpRequestBase request = this.HttpContext.Request;
				string strBrandID = "0";
				if (request.Cookies["BRANCH_ID_CookieName"] != null)
				{
					strBrandID = request.Cookies["BRANCH_ID_CookieName"].Value;
					if (strBrandID == "")
					{
						strBrandID = "0";
					}
				}

				//get  CurrentUser.branchId

				if ((Helpers.Common.CurrentUser.BranchId != null) && (Helpers.Common.CurrentUser.BranchId > 0))
				{
					strBrandID = Helpers.Common.CurrentUser.BranchId.ToString();
				}

				int? intBrandID = int.Parse(strBrandID);

				//Kiểm tra trùng tên số điện thoại thì không cho lưu
				//var cc = CustomerRepository.GetAllvwCustomer().ToList();
				//var c = cc.Where(item => item.Code.ToLower().Trim() == model.Code.ToLower().Trim()).FirstOrDefault();
				var cus = CustomerRepository.GetCustomerByPhone(model.Mobile.Trim());
				if (cus != null)
				{
					return Json(0);
				}

				var Customer = new Domain.Account.Entities.Customer();
				AutoMapper.Mapper.Map(model, Customer);
				Customer.IsDeleted = false;
				Customer.CreatedUserId = WebSecurity.CurrentUserId;
				Customer.ModifiedUserId = WebSecurity.CurrentUserId;
				Customer.CreatedDate = DateTime.Now;
				Customer.ModifiedDate = DateTime.Now;
				Customer.CustomerType = "Customer";
				Customer.BranchId = intBrandID;
				Customer.Phone = model.Mobile;
				Customer.CompanyName = model.FirstName + " " + model.LastName;

				Customer.Code = Erp.BackOffice.Helpers.Common.GetOrderNo("Customer", model.Code);
				//  var customer = CustomerRepository.GetAllCustomer(pMA_DVIQLY)
				//.Where(item => ((item.Mobile == model.Mobile) || (item.Code == Customer.Code))).FirstOrDefault();
				//  if (customer != null)
				//  {
				//      TempData[Globals.FailedMessageKey] = "Khách hàng trùng số điện thoại ";
				//      ViewBag.FailedMessage = TempData[Globals.FailedMessageKey];
				//      return Json(0);
				//  }



				CustomerRepository.InsertCustomer(Customer);


				//var prefix1 = Erp.BackOffice.Helpers.Common.GetSetting(Erp.BackOffice.Helpers.Common.CurrentUser.MA_DVIQLY,"prefixOrderNo_Customer");
				//Customer.Code = Erp.BackOffice.Helpers.Common.GetCode(prefix1, Customer.Id);
				// CustomerRepository.UpdateCustomer(Customer);

				Erp.BackOffice.Helpers.Common.SetOrderNo("Customer");


				//tạo liên hệ cho khách hàng
				if (string.IsNullOrEmpty(model.FirstName) == false && string.IsNullOrEmpty(model.LastName) == false)
				{
					var contact = new Domain.Account.Entities.Contact();
					AutoMapper.Mapper.Map(model, contact);
					//ontact.MA_DVIQLY = pMA_DVIQLY;
					contact.IsDeleted = false;
					contact.CreatedUserId = WebSecurity.CurrentUserId;
					contact.ModifiedUserId = WebSecurity.CurrentUserId;
					contact.CreatedDate = DateTime.Now;
					contact.ModifiedDate = DateTime.Now;
					contact.CustomerId = Customer.Id;

					ContactRepository.InsertContact(contact);
				}
				return Json(Customer.Id);
			}
			catch (Exception ex)
			{

				return Json(0);
			};
		}
		[HttpPost]
		public JsonResult check_FBID(string FacebookId)
		{
			try
			{
				// string pMA_DVIQLY = Helpers.Common.CurrentUser.MA_DVIQLY;
				var fb = CustomerRepository.GetCustomerByFacebookId(FacebookId);
				if (fb != null)
				{
					var pro = productInvoiceRepository.GetAllProductInvoice().Where(x => x.CustomerId == fb.Id).FirstOrDefault();
					if (pro != null)
					{
						fb.PlayId = " (Đã mua)";
						return Json(fb);
					}
					else
					{
						fb.PlayId = " (Chưa mua)";
						return Json(fb);
					}

				}
				//Kiểm tra trùng Facebook ID
				else
				{
					return Json(0);
				}

			}
			catch (Exception ex)
			{
				return Json(-1);
			};

		}
		[HttpGet]
		[AllowAnonymous]
		public JsonResult GetInfoCustomer(string idc)
		{
			try
			{
				int id = 0;
				bool tryp=int.TryParse(idc, out id);
				var cus = CustomerRepository.GetCustomerById(tryp?id:0);
				if (cus != null)
				{
					return Json(new Customer()
					{
						LastName = cus.LastName,
						Address = cus.Address,
						Email = cus.Email,
						Phone = cus.Phone,
						Id=cus.Id,
                        Code = cus.Code
                    }, JsonRequestBehavior.AllowGet);
				}
				else
				{
					var cusc = CustomerRepository.GetvwCustomerByCode(idc);
					if (cusc != null)
					{
						return Json(new vwCustomer()
						{
							LastName = cusc.LastName,
							Address = cusc.Address,
							Email = cusc.Email,
							Phone = cusc.Phone,
							Id = cusc.Id,
							Code = cusc.Code
						}, JsonRequestBehavior.AllowGet);
					} 
					else
					{
						return Json(0, JsonRequestBehavior.AllowGet);
					}
				}

			}
			catch (Exception ex)
			{
				return Json(0, JsonRequestBehavior.AllowGet);
			};

		}

		[HttpPost]
		[ValidateInput(false)]
		public JsonResult LinkedCus(int Id, string fbId, int check)
		{

			var cus = CustomerRepository.GetCustomerById(Id);
			if (cus != null)
			{
				if (String.IsNullOrEmpty(cus.FacebookId) || (!String.IsNullOrEmpty(cus.FacebookId) && check == 1))
				{
					cus.FacebookId = fbId;
					CustomerRepository.UpdateCustomer(cus);
					return Json(1);
				}
				else if ((!String.IsNullOrEmpty(cus.FacebookId) && check == 0))
				{
					return Json(2);
				}
				else
				{
					return Json(0);
				}
			}
			return Json(0);
		}
		public ActionResult LichSuHoaDon()
		{
			return View();
		}
		public ActionResult LichSuXepHang()
		{
			return View();
		}
		public ActionResult NhanVienQuanLy()
		{
			return View();
		}
		public ActionResult KhachHangGioiThieu()
		{
			return View();
		}
		public ActionResult ThongTinKhachHang()
		{
			return View();
		}
		public ActionResult ThonTinLienHe()
		{
			return View();
		}
		public ActionResult DanhGiaKhachHang()
		{
			return View();
		}

		#region Lấy khách từ Bitrix
		public ActionResult KhachHenBitrix()
		{
			string CurrentURL = System.Web.HttpContext.Current.Request.Url.AbsoluteUri;
			string path = System.Web.HttpContext.Current.Request.Url.AbsolutePath;
			CurrentURL = Erp.BackOffice.Helpers.Common.GetSetting("AppURL");
			//CurrentURL = CurrentURL + "/Customer/LayKhachHenBitrix";
			return Redirect(new Uri("https://intranet.trangha.com.vn/oauth/authorize?client_id=local.5c52681b619942.47504244&response_type=code&redirect_uri=" + CurrentURL).ToString());
		}



		public ActionResult LayKhachHenBitrix(string code)
		{
			SettingRepository settingRepository = new SettingRepository(new Domain.ErpDbContext());
			var url = "https://intranet.trangha.com.vn/oauth/token/?grant_type=authorization_code&client_id=local.5c52681b619942.47504244&client_secret=x60zSfvIjjF2r6WXLcnzDiJlhS5t82SjeaU7IWMSE2CZoBzLVR&code=" + code + "&scope=application_permissions&redirect_uri=application_URL";
			// return Redirect(new Uri(url).ToString());
			var json2 = new Newtonsoft.Json.Linq.JObject();
			string pTocken = Erp.BackOffice.Helpers.Common.GetSetting("access_token");
			try
			{
				using (var client2 = new HttpClient())
				{
					client2.BaseAddress = new Uri("https://intranet.trangha.com.vn/", UriKind.Absolute);
					client2.Timeout = TimeSpan.FromSeconds(50);
					client2.DefaultRequestHeaders.Accept.Clear();
					client2.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
					client2.DefaultRequestHeaders.Authorization =
				   new AuthenticationHeaderValue(
					   "Basic",
					   Convert.ToBase64String(
						   System.Text.ASCIIEncoding.ASCII.GetBytes(
							   string.Format("{0}:{1}", Erp.BackOffice.Helpers.Common.GetSetting("UserName_Bitrix"), Erp.BackOffice.Helpers.Common.GetSetting("PassWord_Bitrix")))));

					using (var response2 = client2.GetAsync(url).Result)
					{
						string responseString = response2.Content.ReadAsStringAsync().Result;
						Newtonsoft.Json.Linq.JObject json = Newtonsoft.Json.Linq.JObject.Parse(responseString);
						//json2 = json;
						pTocken = json["access_token"].ToString();
						var stringToken = settingRepository.GetSettingByKey("access_token");
						if (stringToken != null)
						{
							settingRepository.Update(stringToken);
						}

					}
					//
				}
				string startDate = null;
				string endDate = null;
				if (startDate == null && endDate == null)
				{

					DateTime aDateTime = DateTime.Now;
					// Cộng thêm 1 tháng và trừ đi một ngày.
					DateTime retDateTime = DateTime.Now.AddDays(1);



					startDate = aDateTime.ToString("MM/dd/yyyy");
					endDate = retDateTime.ToString("MM/dd/yyyy");
				}


				//UF_CRM_1548498231264

				var url2 = "https://intranet.trangha.com.vn/rest/crm.lead.list?FILTER[>UF_CRM_1542960755869]=" + startDate + "&FILTER[<UF_CRM_1542960755869]=" + endDate + "&auth=" + pTocken + "&select[]=ADDRESS&select[]=PHONE&select[]=TITLE&select[]=UF_CRM_1542960755869";
				using (var client2 = new HttpClient())
				{
					client2.BaseAddress = new Uri("https://intranet.trangha.com.vn/", UriKind.Absolute);
					client2.Timeout = TimeSpan.FromSeconds(50);
					client2.DefaultRequestHeaders.Accept.Clear();
					client2.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
					client2.DefaultRequestHeaders.Authorization =
				   new AuthenticationHeaderValue(
					   "Basic",
					   Convert.ToBase64String(
						   System.Text.ASCIIEncoding.ASCII.GetBytes(
							   string.Format("{0}:{1}", Erp.BackOffice.Helpers.Common.GetSetting("UserName_Bitrix"), Erp.BackOffice.Helpers.Common.GetSetting("PassWord_Bitrix")))));

					using (var response2 = client2.GetAsync(url2).Result)
					{
						string responseString = response2.Content.ReadAsStringAsync().Result;
						Newtonsoft.Json.Linq.JObject json = Newtonsoft.Json.Linq.JObject.Parse(responseString);
						json2 = json;

						// Console.WriteLine(response.StatusCode);
					}
					var result = json2["result"].ToString();
					var total = json2["total"].ToString();
					var rootObject = new JavaScriptSerializer().Deserialize<List<KhachhenModel>>(result);
					foreach (var item in rootObject)
					{
						var cus = CustomerRepository.GetCustomerByPhone(item.PHONE[0].VALUE.Trim());
						if (cus != null)
						{
							item.HasCreate = 1;
						}
					}
					TempData["KhachhenBitrix"] = rootObject;
					return View(rootObject);
				}
			}
			catch (Exception ex)
			{
				return RedirectToAction("KhachHenBitrix");
			}


		}

		[HttpPost]
		public ActionResult LuuKhachHen()
		{
			HttpRequestBase request = this.HttpContext.Request;
			string strBrandID = "0";
			if (request.Cookies["BRANCH_ID_CookieName"] != null)
			{
				strBrandID = request.Cookies["BRANCH_ID_CookieName"].Value;
				if (strBrandID == "")
				{
					strBrandID = "0";
				}
			}

			//get  CurrentUser.branchId

			if ((Helpers.Common.CurrentUser.BranchId != null) && (Helpers.Common.CurrentUser.BranchId > 0))
			{
				strBrandID = Helpers.Common.CurrentUser.BranchId.ToString();
			}

			int? intBrandID = int.Parse(strBrandID);

			var Data = TempData["KhachhenBitrix"] as List<KhachhenModel>;
			string idDeleteAll = Request["DeleteId-checkbox"];
			if (idDeleteAll != null)
			{
				string[] arrDeleteId = idDeleteAll.Split(',');
				for (int i = 0; i < arrDeleteId.Count(); i++)
				{
					var data = Data.Where(x => x.ID == int.Parse(arrDeleteId[i], CultureInfo.InvariantCulture)).FirstOrDefault();
					if (data != null)
					{
						var cus = CustomerRepository.GetCustomerByPhone(data.PHONE[0].VALUE.Trim());
						if (cus == null)
						{


							var Customer = new Domain.Account.Entities.Customer();
							//AutoMapper.Mapper.Map(model, Customer);
							Customer.IsDeleted = false;
							Customer.CreatedUserId = WebSecurity.CurrentUserId;
							Customer.ModifiedUserId = WebSecurity.CurrentUserId;
							Customer.CreatedDate = DateTime.Now;
							Customer.ModifiedDate = DateTime.Now;
							Customer.CustomerType = "";
							Customer.BranchId = intBrandID;
							Customer.Phone = data.PHONE[0].VALUE.Trim();
							Customer.CompanyName = data.TITLE;
							Customer.Address = data.ADDRESS;
							Customer.Code = Erp.BackOffice.Helpers.Common.GetOrderNo("Customer", null);



							CustomerRepository.InsertCustomer(Customer);
							Erp.BackOffice.Helpers.Common.SetOrderNo("Customer");
							//ghi log 
							Erp.BackOffice.Controllers.HomeController.WriteLog(Customer.Id, Customer.Code, "đã tạo khách hàng", "Customer/Detail/" + Customer.Id, Helpers.Common.CurrentUser.BranchId.Value);

						}
					}
				}
			}
			return RedirectToAction("KhachHenBitrix");
		}

	}


	#endregion
}



