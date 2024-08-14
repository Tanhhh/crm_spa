using System.Globalization;
using Erp.BackOffice.Sale.Models;
using Erp.BackOffice.Filters;
using Erp.Domain.Sale.Entities;
using Erp.Domain.Interfaces;
using Erp.Domain.Sale.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Erp.Utilities;
using WebMatrix.WebData;
using Erp.BackOffice.Helpers;
using System.Web.Script.Serialization;
using Erp.Domain.Sale.Repositories;
using System.Data.Entity;
using System.Transactions;
using System.Web;
using PagedList;
using PagedList.Mvc;

namespace Erp.BackOffice.Sale.Controllers
{
	[Authorize]
	[InitializeSimpleMembership]
	[Erp.BackOffice.Helpers.NoCacheHelper]
	public class InventoryController : Controller
	{
		private readonly IWarehouseRepository WarehouseRepository;
		private readonly IProductOrServiceRepository ProductRepository;
		private readonly IInventoryRepository InventoryRepository;
		private readonly IProductInboundRepository ProductInboundRepository;
		private readonly IProductOutboundRepository ProductOutboundRepository;
		private readonly IPhysicalInventoryRepository PhysicalInventoryRepository;
		private readonly IUserRepository userRepository;
		private readonly ITemplatePrintRepository templatePrintRepository;
		public InventoryController(
			IInventoryRepository _Inventory
			, IProductOrServiceRepository _Product
			, IWarehouseRepository _Warehouse
			, IProductInboundRepository _ProductInbound
			, IProductOutboundRepository _ProductOutbound
			, IPhysicalInventoryRepository _PhysicalInventory
			, IUserRepository _user
			, ITemplatePrintRepository _templatePrint
			)
		{
			WarehouseRepository = _Warehouse;
			ProductRepository = _Product;
			InventoryRepository = _Inventory;
			ProductInboundRepository = _ProductInbound;
			ProductOutboundRepository = _ProductOutbound;
			userRepository = _user;
			PhysicalInventoryRepository = _PhysicalInventory;
			templatePrintRepository = _templatePrint;
		}

		#region Index
		public ViewResult Index(string WarehouseId, string txtCode, string conHang, string category, string txtSearch, string manufacturer, int? page, int? BranchId, string origin)
		{
			WarehouseId = WarehouseId == null ? "" : WarehouseId;
			category = category == null ? "" : category;
			txtSearch = txtSearch == null ? "" : txtSearch;
			txtCode = txtCode == null ? "" : txtCode;
			manufacturer = manufacturer == null ? "" : manufacturer;
			BranchId = BranchId == null ? 0 : BranchId;
			origin = origin == null ? "" : origin;
			var user = userRepository.GetByvwUserName(Helpers.Common.CurrentUser.UserName);
			if (string.IsNullOrEmpty(conHang))
			{
				conHang = "1";
			}
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
			BranchId = intBrandID;
			if (!Erp.BackOffice.Filters.SecurityFilter.IsAdmin())
			{
				BranchId = Erp.BackOffice.Helpers.Common.CurrentUser.BranchId;
			}
			var listInventory = Domain.Helper.SqlHelper.QuerySP<InventoryViewModel>("spSale_Get_Inventory", new { WarehouseId = WarehouseId, HasQuantity = conHang, ProductCode = "", ProductName = "", CategoryCode = category, ProductGroup = "", BranchId = BranchId, LoCode = "", ProductId = "", ExpiryDate = "", Origin = origin });
			var listProduct = Domain.Helper.SqlHelper.QuerySP<InventoryViewModel>("spSale_Get_ListProductFromInventory", new { WarehouseId = WarehouseId, HasQuantity = conHang, ProductCode = "", ProductName = "", CategoryCode = category, ProductGroup = "", BranchId = BranchId, CityId = "", DistrictId = "", Origin = origin });

			var warehouseList = new List<WarehouseViewModel>();
			var warehouseList2 = new List<WarehouseViewModel>();

			warehouseList = WarehouseRepository.GetvwAllWarehouse()
				.Select(item => new WarehouseViewModel
				{
					Id = item.Id,
					Name = item.Name,
					Code = item.Code,
					Address = item.Address,
					Note = item.Note,
					BranchId = item.BranchId,
					BranchName = item.BranchName,
					Categories = item.Categories
				}).OrderBy(x => x.Name).ToList();

			warehouseList = warehouseList.Where(x => x.Categories != "VT" && (x.BranchId == BranchId || BranchId == 0)).ToList();
			foreach (var item in warehouseList)
			{
				warehouseList2.Add(item);
			}

			if (BranchId > 0)
			{
				warehouseList = warehouseList.Where(x => x.BranchId == BranchId || BranchId == 0).ToList();
				warehouseList2 = warehouseList2.Where(x => x.BranchId == BranchId || BranchId == 0).ToList();
			}

			//if (!string.IsNullOrEmpty(txtSearch))
			//{
			//    txtSearch = txtSearch == "" ? "~" : Erp.BackOffice.Helpers.Common.ChuyenThanhKhongDau(txtSearch);
			//    listInventory = listInventory.Where(x => Erp.BackOffice.Helpers.Common.ChuyenThanhKhongDau(x.ProductName).Contains(txtSearch)).ToList();
			//    listProduct  = listProduct.Where(x => Erp.BackOffice.Helpers.Common.ChuyenThanhKhongDau(x.ProductName).Contains(txtSearch)).ToList();
			//}
			if (!string.IsNullOrEmpty(txtCode))
			{
				txtCode = txtCode == "" ? "~" : Erp.BackOffice.Helpers.Common.ChuyenThanhKhongDau(txtCode);
				listInventory = listInventory.Where(x => Erp.BackOffice.Helpers.Common.ChuyenThanhKhongDau(x.ProductCode).Contains(Helpers.Common.ChuyenThanhKhongDau(txtCode)) || x.ProductName.ToLower().Contains(Helpers.Common.ChuyenThanhKhongDau(txtCode))).ToList();
				listProduct = listProduct.Where(x => Erp.BackOffice.Helpers.Common.ChuyenThanhKhongDau(x.ProductCode).Contains(Helpers.Common.ChuyenThanhKhongDau(txtCode)) || x.ProductName.ToLower().Contains(Helpers.Common.ChuyenThanhKhongDau(txtCode))).ToList();
			}

			//if (!string.IsNullOrEmpty(WarehouseId))
			//{
			//    warehouseList = warehouseList.Where(x => ("," + WarehouseId + ",").Contains("," + x.Id + ",") == true).ToList();
			//}
			if (!string.IsNullOrEmpty(WarehouseId))
			{
				listInventory = listInventory.Where(x => x.WarehouseId == Int32.Parse(WarehouseId)).ToList();
				warehouseList = warehouseList.Where(x => x.Id == Int32.Parse(WarehouseId)).ToList();
			}


			if (BranchId > 0)
			{
				listInventory = listInventory.Where(x => x.BranchId == BranchId).ToList();

			}



			if (!string.IsNullOrEmpty(manufacturer))
			{
				listInventory = listInventory.Where(x => x.Manufacturer == manufacturer).ToList();
				listProduct = listProduct.Where(x => x.Manufacturer == manufacturer).ToList();
			}
			page = page ?? 1;




			int pageNumber = (page ?? 1);

			ViewBag.inventoryList = listInventory.ToList();
			ViewBag.listProduct = listProduct.ToPagedList(pageNumber, 50);

			ViewBag.warehouseList = warehouseList.ToList();
			ViewBag.warehouseList2 = warehouseList2.ToList();
			ViewBag.SuccessMessage = TempData["SuccessMessage"];
			ViewBag.FailedMessage = TempData["FailedMessage"];
			ViewBag.AlertMessage = TempData["AlertMessage"];

			return View();
		}
		#endregion

		#region Detail
		public ActionResult Detail(int? Id, int? WarehouseId, string LoCode, int? day, int? month, int? year)
		{
			var Product = ProductRepository.GetvwProductAndServiceById(Id.Value);
			if (Product != null && Product.IsDeleted != true)
			{
				var model = new ProductViewModel();
				AutoMapper.Mapper.Map(Product, model);

				var inboundDetails = ProductInboundRepository.GetAllvwProductInboundDetailByProductId(Id.Value).AsEnumerable()
					.Where(item => item.IsArchive && item.LoCode == LoCode);
				var outboundDetails = ProductOutboundRepository.GetAllvwProductOutboundDetailByProductId(Id.Value).AsEnumerable()
					 .Where(item => item.IsArchive && item.LoCode == LoCode);

				if (WarehouseId != null && WarehouseId > 0)
				{
					inboundDetails = inboundDetails.Where(x => x.WarehouseDestinationId == WarehouseId);
					outboundDetails = outboundDetails.Where(x => x.WarehouseSourceId == WarehouseId);
				}
				if (day != null && month != null && year != null)
				{
					inboundDetails = inboundDetails.Where(x => x.ExpiryDate != null && x.ExpiryDate.Value.Day == day && x.ExpiryDate.Value.Month == month && x.ExpiryDate.Value.Year == year);
					outboundDetails = outboundDetails.Where(x => x.ExpiryDate != null && x.ExpiryDate.Value.Day == day && x.ExpiryDate.Value.Month == month && x.ExpiryDate.Value.Year == year);
				}
				else
				{
					inboundDetails = inboundDetails.Where(x => x.ExpiryDate == null);
					outboundDetails = outboundDetails.Where(x => x.ExpiryDate == null);
				}
				inboundDetails = inboundDetails.OrderBy(x => x.ProductInboundId);
				outboundDetails = outboundDetails.OrderBy(x => x.ProductOutboundId);
				ViewBag.inboundDetails = inboundDetails;
				ViewBag.outboundDetails = outboundDetails;

				return View(model);
			}

			if (Request.UrlReferrer != null)
				return Redirect(Request.UrlReferrer.AbsoluteUri);
			return RedirectToAction("Index");
		}

		#endregion

		#region Json
		public JsonResult GetListProductJsonByWarehouseId(int? warehouseId)
		{
			if (warehouseId == null)
				return Json(new List<int>(), JsonRequestBehavior.AllowGet);

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


			var list = Erp.Domain.Helper.SqlHelper.QuerySP<InventoryViewModel>("spSale_Get_Inventory", new { WarehouseId = warehouseId, HasQuantity = "1", ProductCode = "", ProductName = "", CategoryCode = "", ProductGroup = "", BranchId = intBrandID, LoCode = "", ProductId = "", ExpiryDate = "", Origin = "" }).OrderBy(x => x.ProductName).ToList();
			foreach (var item in list)
			{
				item.strExpiryDate = item.ExpiryDate == null ? "" : item.ExpiryDate.Value.ToString("dd/MM/yyyy");
				item.LoCode = item.LoCode == null ? "" : item.LoCode;
			}

			return Json(list, JsonRequestBehavior.AllowGet);
		}
		#endregion

		#region Check or Update
		public static string Check(string productName, int productId, string LoCode, DateTime? ExpiryDate, int warehouseId, int quantityIn, int quantityOut)
		{

			return Update(productName, productId, LoCode, ExpiryDate, warehouseId, quantityIn, quantityOut, false);

		}


		public static string Check_notran(string productName, int productId, string LoCode, DateTime? ExpiryDate, int warehouseId, int quantityIn, int quantityOut)
		{

			return Update_notrans(productName, productId, LoCode, ExpiryDate, warehouseId, quantityIn, quantityOut, false);

		}

		public static string Check_mobile(string productName, int productId, string LoCode, DateTime? ExpiryDate, int warehouseId, int quantityIn, int quantityOut, int pCurrentUserId)
		{
			return Update_mobile(productName, productId, LoCode, ExpiryDate, warehouseId, quantityIn, quantityOut, pCurrentUserId, false);
		}

		public static string Update(string productName, int productId, string LoCode, DateTime? ExpiryDate, int warehouseId, int quantityIn, int quantityOut, bool isArchive = true)
		{
			string error = "";
			using (var scope = new TransactionScope(TransactionScopeOption.Required,
							   new System.TimeSpan(0, 30, 0)))
			{
				try
				{
					ProductInboundRepository productInboundRepository = new ProductInboundRepository(new Domain.Sale.ErpSaleDbContext());
					ProductOutboundRepository productOutboundRepository = new Domain.Sale.Repositories.ProductOutboundRepository(new Domain.Sale.ErpSaleDbContext());
					InventoryRepository inventoryRepository = new Domain.Sale.Repositories.InventoryRepository(new Domain.Sale.ErpSaleDbContext());
					LoCode = LoCode == null ? "" : LoCode;
					var d_ExpiryDate = (ExpiryDate != null ? DateTime.ParseExact(ExpiryDate.Value.ToString("dd/MM/yyyy"), "dd/MM/yyyy", null).ToString("yyyy-MM-dd HH:mm:ss") : "");
					var inbound = Domain.Helper.SqlHelper.QuerySP<vwProductInboundDetail>("spSale_GetInboundDetail", new
					{
						BranchId = Helpers.Common.CurrentUser.BranchId,
						ProductId = productId,
						LoCode = LoCode,
						ExpiryDate = d_ExpiryDate,
						WarehouseDestinationId = warehouseId
					}).ToList();

					var outbound = Domain.Helper.SqlHelper.QuerySP<vwProductOutboundDetail>("spSale_GetOutboundDetail", new
					{
						BranchId = Helpers.Common.CurrentUser.BranchId,
						ProductId = productId,
						LoCode = LoCode,
						ExpiryDate = d_ExpiryDate,
						WarehouseSourceId = warehouseId
					});
					var inventoryCurrent_List = Erp.Domain.Helper.SqlHelper.QuerySP<Inventory>("spSale_Get_Inventory",
							 new
							 {
								 WarehouseId = warehouseId,
								 HasQuantity = "1",
								 ProductCode = "",
								 ProductName = "",
								 CategoryCode = "",
								 ProductGroup = "",
								 BranchId = Helpers.Common.CurrentUser.BranchId,
								 LoCode = LoCode,
								 ProductId = productId,
								 ExpiryDate = ExpiryDate,
								 Origin = ""
							 }).ToList();
					if (string.IsNullOrEmpty(LoCode) || LoCode == "")
					{
						inbound = inbound.Where(x => string.IsNullOrEmpty(x.LoCode) || x.LoCode == "").ToList();
						outbound = outbound.Where(x => string.IsNullOrEmpty(x.LoCode) || x.LoCode == "").ToList();
						inventoryCurrent_List = inventoryCurrent_List.Where(x => string.IsNullOrEmpty(x.LoCode)).ToList();
					}
					else
					{
						inbound = inbound.Where(x => x.LoCode == LoCode).ToList();
						outbound = outbound.Where(x => x.LoCode == LoCode).ToList();
						inventoryCurrent_List = inventoryCurrent_List.Where(x => x.LoCode == LoCode).ToList();
					}
					if (ExpiryDate == null)
					{
						inbound = inbound.Where(x => x.ExpiryDate == null).ToList();
						outbound = outbound.Where(x => x.ExpiryDate == null).ToList();
						inventoryCurrent_List = inventoryCurrent_List.Where(x => x.ExpiryDate == null).ToList();
					}
					else
					{
						inbound = inbound.Where(x => x.ExpiryDate == ExpiryDate).ToList();
						outbound = outbound.Where(x => x.ExpiryDate == ExpiryDate).ToList();
						inventoryCurrent_List = inventoryCurrent_List.Where(x => x.ExpiryDate == ExpiryDate).ToList();
					}
					var qty_inbound = inbound.Sum(x => x.Quantity);
					var qty_outbound = outbound.Sum(x => x.Quantity);
					var inventory = (inbound.Count() > 0 ? qty_inbound : 0) - (outbound.Count() > 0 ? qty_outbound : 0) + quantityIn - quantityOut;


					for (int i = 0; i < inventoryCurrent_List.Count(); i++)
					{
						if (i > 0)
						{
							var id = inventoryCurrent_List[i].Id;
							inventoryRepository.DeleteInventory(id);
						}
					}
					//Sau khi thay đổi dữ liệu phải đảm bảo tồn kho >= 0
					if (isArchive)
					{
						if (inventoryCurrent_List.Count() > 0)
						{
							if (inventoryCurrent_List[0].Quantity != inventory)
							{
								inventoryCurrent_List[0].Quantity = inventory;
								if (Helpers.Common.CurrentUser.BranchId != null)
								{
									inventoryCurrent_List[0].BranchId = Helpers.Common.CurrentUser.BranchId;
								}
								inventoryRepository.UpdateInventory(inventoryCurrent_List[0]);
							}
						}
						else
						{
							var insert = new Inventory();
							insert.IsDeleted = false;
							insert.CreatedUserId = WebSecurity.CurrentUserId;
							insert.ModifiedUserId = WebSecurity.CurrentUserId;
							insert.CreatedDate = DateTime.Now;
							insert.ModifiedDate = DateTime.Now;
							insert.WarehouseId = warehouseId;
							insert.ProductId = productId;
							insert.Quantity = inventory;
							insert.LoCode = LoCode;
							insert.ExpiryDate = ExpiryDate;
							if (Helpers.Common.CurrentUser.BranchId != null)
							{
								insert.BranchId = Helpers.Common.CurrentUser.BranchId;
							}
							inventoryRepository.InsertInventory(insert);
						}
					}

					scope.Complete();
				}
				catch (DbUpdateException)
				{
				}
			}
			return error;
		}


		public static string Update_notrans(string productName, int productId, string LoCode, DateTime? ExpiryDate, int warehouseId, int quantityIn, int quantityOut, bool isArchive = true)
		{
			try
			{




				string error = "";

				ProductInboundRepository productInboundRepository = new ProductInboundRepository(new Domain.Sale.ErpSaleDbContext());
				ProductOutboundRepository productOutboundRepository = new Domain.Sale.Repositories.ProductOutboundRepository(new Domain.Sale.ErpSaleDbContext());
				InventoryRepository inventoryRepository = new Domain.Sale.Repositories.InventoryRepository(new Domain.Sale.ErpSaleDbContext());
				LoCode = LoCode == null ? "" : LoCode;
				var d_ExpiryDate = (ExpiryDate != null ? DateTime.ParseExact(ExpiryDate.Value.ToString("dd/MM/yyyy"), "dd/MM/yyyy", null).ToString("yyyy-MM-dd HH:mm:ss") : "");
				var inbound = Domain.Helper.SqlHelper.QuerySP<vwProductInboundDetail>("spSale_GetInboundDetail", new
				{
					BranchId = Helpers.Common.CurrentUser.BranchId.Value,
					ProductId = productId,
					LoCode = LoCode,
					ExpiryDate = d_ExpiryDate,
					WarehouseDestinationId = warehouseId
				}).ToList();

				var outbound = Domain.Helper.SqlHelper.QuerySP<vwProductOutboundDetail>("spSale_GetOutboundDetail", new
				{
					BranchId = Helpers.Common.CurrentUser.BranchId.Value,
					ProductId = productId,
					LoCode = LoCode,
					ExpiryDate = d_ExpiryDate,
					WarehouseSourceId = warehouseId
				});
				var inventoryCurrent_List = Domain.Helper.SqlHelper.QuerySP<Inventory>("spSale_Get_Inventory",
						 new
						 {
							 WarehouseId = warehouseId,
							 HasQuantity = "1",
							 ProductCode = "",
							 ProductName = "",
							 CategoryCode = "",
							 ProductGroup = "",
							 BranchId = Helpers.Common.CurrentUser.BranchId.Value,
							 LoCode = LoCode,
							 ProductId = productId,
							 ExpiryDate = ExpiryDate,
							 Origin = ""
						 }).ToList();
				if (string.IsNullOrEmpty(LoCode) || LoCode == "")
				{
					inbound = inbound.Where(x => string.IsNullOrEmpty(x.LoCode) || x.LoCode == "").ToList();
					outbound = outbound.Where(x => string.IsNullOrEmpty(x.LoCode) || x.LoCode == "").ToList();
					inventoryCurrent_List = inventoryCurrent_List.Where(x => string.IsNullOrEmpty(x.LoCode)).ToList();
				}
				else
				{
					inbound = inbound.Where(x => x.LoCode == LoCode).ToList();
					outbound = outbound.Where(x => x.LoCode == LoCode).ToList();
					inventoryCurrent_List = inventoryCurrent_List.Where(x => x.LoCode == LoCode).ToList();
				}
				if (ExpiryDate == null)
				{
					inbound = inbound.Where(x => x.ExpiryDate == null).ToList();
					outbound = outbound.Where(x => x.ExpiryDate == null).ToList();
					inventoryCurrent_List = inventoryCurrent_List.Where(x => x.ExpiryDate == null).ToList();
				}
				else
				{
					inbound = inbound.Where(x => x.ExpiryDate == ExpiryDate).ToList();
					outbound = outbound.Where(x => x.ExpiryDate == ExpiryDate).ToList();
					inventoryCurrent_List = inventoryCurrent_List.Where(x => x.ExpiryDate == ExpiryDate).ToList();
				}
				var qty_inbound = inbound.Sum(x => x.Quantity);
				var qty_outbound = outbound.Sum(x => x.Quantity);
				var inventory = (inbound.Count() > 0 ? qty_inbound : 0) - (outbound.Count() > 0 ? qty_outbound : 0) + quantityIn - quantityOut;


				for (int i = 0; i < inventoryCurrent_List.Count(); i++)
				{
					if (i > 0)
					{
						var id = inventoryCurrent_List[i].Id;
						inventoryRepository.DeleteInventory(id);
					}
				}
				//Sau khi thay đổi dữ liệu phải đảm bảo tồn kho >= 0
				if (inventory >= 0)//inventory >= 0)
				{
					if (isArchive)
					{
						if (inventoryCurrent_List.Count() > 0)
						{
							if (inventoryCurrent_List[0].Quantity != inventory)
							{
								inventoryCurrent_List[0].Quantity = inventory;
								if (Helpers.Common.CurrentUser.BranchId != null)
								{
									inventoryCurrent_List[0].BranchId = Helpers.Common.CurrentUser.BranchId.Value;
								}
								inventoryRepository.UpdateInventory(inventoryCurrent_List[0]);
							}
						}
						else
						{
							var insert = new Inventory();
							insert.IsDeleted = false;
							insert.CreatedUserId = WebSecurity.CurrentUserId;
							insert.ModifiedUserId = WebSecurity.CurrentUserId;
							insert.CreatedDate = DateTime.Now;
							insert.ModifiedDate = DateTime.Now;
							insert.WarehouseId = warehouseId;
							insert.ProductId = productId;
							insert.Quantity = inventory;
							insert.LoCode = LoCode;
							insert.ExpiryDate = ExpiryDate;
							if (Helpers.Common.CurrentUser.BranchId != null)
							{
								insert.BranchId = Helpers.Common.CurrentUser.BranchId.Value;
							}
							inventoryRepository.InsertInventory(insert);
						}
					}
				}
				else
				{
					error += string.Format("<br/>Dữ liệu tồn kho của sản phẩm \"{0}\" là {1}: không hợp lệ!", productName, inventory);

				}
				return error;
			}
			catch (Exception ex)
			{

				return ex.Message;
			}
		}



		public static string Update_mobile(string productName, int productId, string LoCode, DateTime? ExpiryDate, int warehouseId, int quantityIn, int quantityOut, int pCurrentUserId, bool isArchive = true)
		{
			string error = "";
			using (var scope = new TransactionScope(TransactionScopeOption.Required,
							   new System.TimeSpan(0, 30, 0)))
			{
				try
				{

					ProductInboundRepository productInboundRepository = new ProductInboundRepository(new Domain.Sale.ErpSaleDbContext());
					ProductOutboundRepository productOutboundRepository = new Domain.Sale.Repositories.ProductOutboundRepository(new Domain.Sale.ErpSaleDbContext());
					InventoryRepository inventoryRepository = new Domain.Sale.Repositories.InventoryRepository(new Domain.Sale.ErpSaleDbContext());
					LoCode = LoCode == null ? "" : LoCode;
					var d_ExpiryDate = (ExpiryDate != null ? DateTime.ParseExact(ExpiryDate.Value.ToString("dd/MM/yyyy"), "dd/MM/yyyy", null).ToString("yyyy-MM-dd HH:mm:ss") : "");

					//lấy tất cả các phiếu nhập
					var inbound = Domain.Helper.SqlHelper.QuerySP<vwProductInboundDetail>("spSale_GetInboundDetail", new
					{
						BranchId = Helpers.Common.CurrentUser.BranchId.Value,
						ProductId = productId,
						LoCode = LoCode,
						ExpiryDate = d_ExpiryDate,
						WarehouseDestinationId = warehouseId
					}).ToList();

					//lất tất cả các phiếu xuất
					var outbound = Domain.Helper.SqlHelper.QuerySP<vwProductOutboundDetail>("spSale_GetOutboundDetail", new
					{
						BranchId = Helpers.Common.CurrentUser.BranchId.Value,
						ProductId = productId,
						LoCode = LoCode,
						ExpiryDate = d_ExpiryDate,
						WarehouseSourceId = warehouseId
					});

					//lấy tồn kho hiện 
					var inventoryCurrent_List = Erp.Domain.Helper.SqlHelper.QuerySP<Inventory>("spSale_Get_Inventory",
						new
						{
							WarehouseId = warehouseId,
							HasQuantity = "1",
							ProductCode = "",
							ProductName = "",
							CategoryCode = "",
							ProductGroup = "",
							BranchId = Helpers.Common.CurrentUser.BranchId.Value,
							LoCode = LoCode,
							ProductId = productId,
							ExpiryDate = ExpiryDate
						}).ToList();


					//lấy các sản phẩm nhập, xuất, tồn kho theo lô
					if (string.IsNullOrEmpty(LoCode) || LoCode == "")
					{
						inbound = inbound.Where(x => string.IsNullOrEmpty(x.LoCode) || x.LoCode == "").ToList();
						outbound = outbound.Where(x => string.IsNullOrEmpty(x.LoCode) || x.LoCode == "").ToList();
						inventoryCurrent_List = inventoryCurrent_List.Where(x => string.IsNullOrEmpty(x.LoCode)).ToList();
					}
					else
					{
						inbound = inbound.Where(x => x.LoCode == LoCode).ToList();
						outbound = outbound.Where(x => x.LoCode == LoCode).ToList();
						inventoryCurrent_List = inventoryCurrent_List.Where(x => x.LoCode == LoCode).ToList();
					}

					//lấy các sản phẩm nhập, xuất, tồn kho theo lô và ngày hết hạn
					if (ExpiryDate == null)
					{
						inbound = inbound.Where(x => x.ExpiryDate == null).ToList();
						outbound = outbound.Where(x => x.ExpiryDate == null).ToList();
						inventoryCurrent_List = inventoryCurrent_List.Where(x => x.ExpiryDate == null).ToList();
					}
					else
					{
						inbound = inbound.Where(x => x.ExpiryDate == ExpiryDate).ToList();
						outbound = outbound.Where(x => x.ExpiryDate == ExpiryDate).ToList();
						inventoryCurrent_List = inventoryCurrent_List.Where(x => x.ExpiryDate == ExpiryDate).ToList();
					}


					var qty_inbound = inbound.Sum(x => x.Quantity);
					var qty_outbound = outbound.Sum(x => x.Quantity);
					//tính lại tồn kho dựa vào nhập và xuất
					var inventory = (inbound.Count() > 0 ? qty_inbound : 0) - (outbound.Count() > 0 ? qty_outbound : 0) + quantityIn - quantityOut;


					//begin xóa các dòng double của sản phẩm và lô của sản phẩm đó, chỉ giữ lại 1 dòng đầu tiên
					for (int i = 0; i < inventoryCurrent_List.Count(); i++)
					{
						if (i > 0)
						{
							var id = inventoryCurrent_List[i].Id;
							inventoryRepository.DeleteInventory(id);
						}
					}
					//end xóa các dòng double của sản phẩm và lô của sản phẩm đó, chỉ giữ lại 1 dòng đầu tiên

					//Sau khi thay đổi dữ liệu phải đảm bảo tồn kho >= 0
					if (true)//inventory >= 0)
					{
						if (isArchive)
						{
							if (inventoryCurrent_List.Count() > 0)
							{
								if (inventoryCurrent_List[0].Quantity != inventory)
								{
									inventoryCurrent_List[0].Quantity = inventory;
									if (Helpers.Common.CurrentUser.BranchId != null)
									{
										inventoryCurrent_List[0].BranchId = Helpers.Common.CurrentUser.BranchId.Value;
									}
									inventoryRepository.UpdateInventory(inventoryCurrent_List[0]);
								}
							}
							else
							{
								var insert = new Inventory();
								insert.IsDeleted = false;
								insert.CreatedUserId = pCurrentUserId;
								insert.ModifiedUserId = pCurrentUserId;
								insert.CreatedDate = DateTime.Now;
								insert.ModifiedDate = DateTime.Now;
								insert.WarehouseId = warehouseId;
								insert.ProductId = productId;
								insert.Quantity = inventory;
								insert.LoCode = LoCode;
								insert.ExpiryDate = ExpiryDate;
								if (Helpers.Common.CurrentUser.BranchId != null)
								{
									insert.BranchId = Helpers.Common.CurrentUser.BranchId.Value;
								}
								inventoryRepository.InsertInventory(insert);
							}
						}
					}
					else
					{
						error += string.Format("<br/>Dữ liệu tồn kho của sản phẩm \"{0}\" là {1}: không hợp lệ!", productName, inventory);
					}
					scope.Complete();
					return error;

				}
				catch (Exception ex)
				{

					return ex.Message;
				}
			}

		}
		#endregion

		#region UpdateAll
		public ActionResult UpdateAll(string url)
		{


			string rs = "";
			using (var scope = new TransactionScope(TransactionScopeOption.Required,
							   new System.TimeSpan(0, 30, 0)))
			{
				try
				{

					//beign hoapd su ly cac so lieu trong ton kho bi dub
					var inventoryList_dele = InventoryRepository.GetAllvwInventory().Where(x => x.IdInventory > 0).ToList();
					foreach (var item in inventoryList_dele)
					{
						if (item.ProductId == 17320)
						{
							int a = 1;
						}
						var inventoryCurrent_List = inventoryList_dele.Where(x => x.ProductId == item.ProductId && x.WarehouseId == item.WarehouseId).ToList();
						if (inventoryCurrent_List.Count() > 1)
						{
							for (int i = 0; i < inventoryCurrent_List.Count(); i++)
							{
								if (i > 0)
								{
									var id = inventoryCurrent_List[i].IdInventory;
									InventoryRepository.DeleteInventory(id.Value);
								}
							}
						}
					}


					//End hoapd su ly cac so lieu trong ton kho bi dub


					var inventoryList = InventoryRepository.GetAllInventory().ToList();
					var inbound1 = ProductInboundRepository.GetAllvwProductInboundDetail().ToList();
					var outbound1 = ProductOutboundRepository.GetAllvwProductOutboundDetail().ToList();


					foreach (var item in inventoryList)
					{
						var warehouseId = item.WarehouseId.Value;
						var productId = item.ProductId.Value;
						if (productId == 16877)
						{
							int a = 1;
						}

						var inbound = inbound1.ToList();
						var outbound = outbound1.ToList();

						if (string.IsNullOrEmpty(item.LoCode) || item.LoCode == "")
						{
							inbound = inbound.Where(x => string.IsNullOrEmpty(x.LoCode) || x.LoCode == "").ToList();
							outbound = outbound.Where(x => string.IsNullOrEmpty(x.LoCode) || x.LoCode == "").ToList();
						}
						else
						{
							inbound = inbound.Where(x => x.LoCode == item.LoCode).ToList();
							outbound = outbound.Where(x => x.LoCode == item.LoCode).ToList();
						}

						if (item.ExpiryDate == null)
						{
							inbound = inbound.Where(x => x.ExpiryDate == null).ToList();
							outbound = outbound.Where(x => x.ExpiryDate == null).ToList();
						}
						else
						{
							inbound = inbound.Where(x => x.ExpiryDate == item.ExpiryDate).ToList();
							outbound = outbound.Where(x => x.ExpiryDate == item.ExpiryDate).ToList();
						}


						inbound = inbound.Where(x => x.IsArchive == true && x.ProductId == productId && x.WarehouseDestinationId == warehouseId).ToList();

						var _inbound = inbound.Where(x => x.IsArchive
							  && x.ProductId == productId
							  && x.WarehouseDestinationId == warehouseId
							  ).ToList().Sum(x => x.Quantity);

						var _outbound = outbound.Where(x => x.IsArchive
							&& x.ProductId == productId
							&& x.WarehouseSourceId == warehouseId
							).ToList().Sum(x => x.Quantity);
						var inventory = (_inbound == null ? 0 : _inbound) - (_outbound == null ? 0 : _outbound);





						if (item.Quantity != inventory)
						{
							rs += "<br/>" + item.ProductId + " | " + item.Quantity + " => " + inventory;
							item.Quantity = inventory;
							InventoryRepository.UpdateInventory(item);
						}

					}
					scope.Complete();
				}
				catch (DbUpdateException)
				{
					TempData[Globals.FailedMessageKey] = App_GlobalResources.Wording.Error;
					return Redirect(url);
				}
			}
			TempData[Globals.SuccessMessageKey] = App_GlobalResources.Wording.UpdateSuccess + rs;
			return Redirect(url);
		}
		#endregion

		#region ExportExcel
		public String buildHtml(string txtCode, string manufacturer, string category, string conHang, string WarehouseId, string origin, int? BranchId)
		{
			WarehouseId = WarehouseId == null ? "" : WarehouseId;
			category = category == null ? "" : category;
			txtCode = txtCode == null ? "" : txtCode;
			manufacturer = manufacturer == null ? "" : manufacturer;
			BranchId = BranchId == null ? 0 : BranchId;
			origin = origin == null ? "" : origin;
			var user = userRepository.GetByvwUserName(Helpers.Common.CurrentUser.UserName);
			if (string.IsNullOrEmpty(conHang))
			{
				conHang = "1";
			}
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
			BranchId = intBrandID;
			if (!Erp.BackOffice.Filters.SecurityFilter.IsAdmin())
			{
				BranchId = Erp.BackOffice.Helpers.Common.CurrentUser.BranchId;
			}
			var listInventory = Domain.Helper.SqlHelper.QuerySP<InventoryViewModel>("spSale_Get_Inventory", new { WarehouseId = WarehouseId, HasQuantity = conHang, ProductCode = "", ProductName = "", CategoryCode = category, ProductGroup = "", BranchId = BranchId, LoCode = "", ProductId = "", ExpiryDate = "", Origin = origin });
			var listProduct = Domain.Helper.SqlHelper.QuerySP<InventoryViewModel>("spSale_Get_ListProductFromInventory", new { WarehouseId = WarehouseId, HasQuantity = conHang, ProductCode = "", ProductName = "", CategoryCode = category, ProductGroup = "", BranchId = BranchId, CityId = "", DistrictId = "", Origin = origin });

			var warehouseList = new List<WarehouseViewModel>();
			var warehouseList2 = new List<WarehouseViewModel>();

			warehouseList = WarehouseRepository.GetvwAllWarehouse()
				.Select(item => new WarehouseViewModel
				{
					Id = item.Id,
					Name = item.Name,
					Code = item.Code,
					Address = item.Address,
					Note = item.Note,
					BranchId = item.BranchId,
					BranchName = item.BranchName,
					Categories = item.Categories
				}).OrderBy(x => x.Name).ToList();

			warehouseList = warehouseList.Where(x => x.Categories != "VT" && (x.BranchId == BranchId || BranchId == 0)).ToList();
			foreach (var item in warehouseList)
			{
				warehouseList2.Add(item);
			}

			if (BranchId > 0)
			{
				warehouseList = warehouseList.Where(x => x.BranchId == BranchId || BranchId == 0).ToList();
				warehouseList2 = warehouseList2.Where(x => x.BranchId == BranchId || BranchId == 0).ToList();
			}

			if (!string.IsNullOrEmpty(txtCode))
			{
				txtCode = txtCode == "" ? "~" : Erp.BackOffice.Helpers.Common.ChuyenThanhKhongDau(txtCode);
				listInventory = listInventory.Where(x => Erp.BackOffice.Helpers.Common.ChuyenThanhKhongDau(x.ProductCode).Contains(Helpers.Common.ChuyenThanhKhongDau(txtCode)) || x.ProductName.ToLower().Contains(Helpers.Common.ChuyenThanhKhongDau(txtCode))).ToList();
				listProduct = listProduct.Where(x => Erp.BackOffice.Helpers.Common.ChuyenThanhKhongDau(x.ProductCode).Contains(Helpers.Common.ChuyenThanhKhongDau(txtCode)) || x.ProductName.ToLower().Contains(Helpers.Common.ChuyenThanhKhongDau(txtCode))).ToList();
			}

			if (!string.IsNullOrEmpty(WarehouseId))
			{
				listInventory = listInventory.Where(x => x.WarehouseId == Int32.Parse(WarehouseId)).ToList();
				warehouseList = warehouseList.Where(x => x.Id == Int32.Parse(WarehouseId)).ToList();
			}

			if (BranchId > 0)
			{
				listInventory = listInventory.Where(x => x.BranchId == BranchId).ToList();

			}

			if (!string.IsNullOrEmpty(manufacturer))
			{
				listInventory = listInventory.Where(x => x.Manufacturer == manufacturer).ToList();
				listProduct = listProduct.Where(x => x.Manufacturer == manufacturer).ToList();
			}


			//var a = listProduct.Where(x => x.ProductId == 16960);
			//Tạo table html chi tiết phiếu xuất
			string detailLists = "<table border=\"1\" class=\"invoice-detail\">\r\n";
			detailLists += "<thead>\r\n";
			detailLists += "	<tr>\r\n";
			detailLists += "		<th>STT</th>\r\n";
			detailLists += "		<th>Mã</th>\r\n";
			detailLists += "		<th>Tên Hàng</th>\r\n";
			detailLists += "		<th>Nhãn Hàng</th>\r\n";
			foreach (var item in warehouseList)
			{
				detailLists += "		<th>" + item.Name + "</th>\r\n";
			}

			detailLists += "		<th>Tổng Số</th>\r\n";
			detailLists += "		<th>Giá nhập</th>\r\n";
			detailLists += "		<th>Giá xuất</th>\r\n";
			detailLists += "		<th>Danh mục</th>\r\n";
			detailLists += "		<th>Nhóm sản phẩm</th>\r\n";
			detailLists += "	</tr>\r\n";
			detailLists += "</thead>\r\n";
			detailLists += "<tbody>\r\n";
			var _index = 1;

			foreach (var item in listProduct.GroupBy(x => x.ProductId))
			{
				detailLists += "<tr>\r\n" //+ (_index++) + ". "
					+ "<td>" + (_index++) + "</td>\r\n"
				+ "<td>" + item.FirstOrDefault().ProductCode + "</td>\r\n"
				+ "<td>" + item.FirstOrDefault().ProductName + "</td>\r\n"
				+ "<td>" + item.FirstOrDefault().Origin + "</td>\r\n";

				foreach (var ii in warehouseList)
				{
					var productOfWH = listInventory.Where(x => x.ProductId == item.Key && x.WarehouseId == ii.Id).ToList();
					if (productOfWH.Count() != 0)
					{
						int? productQtyOfWH = productOfWH.Sum(x => x.Quantity);
						detailLists += "<td class=\"text-right\" style=\"font-weight:bold\">" + Erp.BackOffice.Helpers.Common.PhanCachHangNgan2(productQtyOfWH).Replace(".", ",") + "</td>\r\n";
					}
					else
					{
						detailLists += "<td class=\"text-right\" style=\"font-weight:bold\">" + 0 + "</td>\r\n";
					}
				}
				detailLists += "<td class=\"text-right\" style=\"font-weight:bold\">" + Erp.BackOffice.Helpers.Common.PhanCachHangNgan2(listInventory.Where(x => x.ProductId == item.Key).Sum(x => x.Quantity)).Replace(".", ",") + "</td>\r\n";
				detailLists += "<td class=\"text-right\" style=\"font-weight:bold\">" + Erp.BackOffice.Helpers.Common.PhanCachHangNgan2(listInventory.Where(x => x.ProductId == item.Key).FirstOrDefault().ProductPriceInbound).Replace(".", ",") + "</td>\r\n";
				detailLists += "<td class=\"text-right\" style=\"font-weight:bold\">" + Erp.BackOffice.Helpers.Common.PhanCachHangNgan2(listInventory.Where(x => x.ProductId == item.Key).FirstOrDefault().ProductPriceOutbound).Replace(".", ",") + "</td>\r\n";

					detailLists += "<td class=\"text-right\" style=\"font-weight:bold\">" + listInventory.Where(x => x.ProductId == item.Key).FirstOrDefault().CategoryCode + "</td>\r\n";
				detailLists += "<td class=\"text-right\" style=\"font-weight:bold\">" + listInventory.Where(x => x.ProductId == item.Key).FirstOrDefault().ProductGroup + "</td>\r\n"

				+"</tr>\r\n";
				//  detailLists += "<td class=\"text-right\" style=\"font-weight:bold\">" + Erp.BackOffice.Helpers.Common.PhanCachHangNgan2(item.FirstOrDefault().ProductPriceInbound).Replace(".", ",") + "</td>\r\n"
				// + "</tr>\r\n";
				//  detailLists += "<td class=\"text-right\" style=\"font-weight:bold\">" + Erp.BackOffice.Helpers.Common.PhanCachHangNgan2(item.FirstOrDefault().ProductPriceOutbound).Replace(".", ",") + "</td>\r\n"
				//+ "</tr>\r\n";

				int index = 1;
				//chưa dùng tới
				//foreach (var product in listProduct.Where(x => x.ProductId == item.Key))
				//{
				//    detailLists += "<tr>\r\n"
				//    + "<td>" + _index + "." + (index++) + "</td>\r\n"
				//    + "<td>" + product.LoCode + "</td>\r\n"
				//    + "<td>" + (product.ExpiryDate.HasValue ? product.ExpiryDate.Value.ToString("dd/MM/yyyy") : "") + "</td>\r\n";
				//    foreach (var ii in warehouseList)
				//    {
				//        var productOfWH = listInventory.Where(x => x.ProductId == product.ProductId && x.LoCode == product.LoCode && x.day == product.day && x.month == product.month && x.year == product.year && x.WarehouseId == ii.Id).ToList();
				//        if (productOfWH.Count() != 0)
				//        {
				//            int? productQtyOfWH = productOfWH.Sum(x => x.Quantity);
				//            detailLists += "<td class=\"text-right\" style=\"font-weight:bold\">" + Erp.BackOffice.Helpers.Common.PhanCachHangNgan2(productQtyOfWH).Replace(".", ",") + "</td>\r\n";
				//        }
				//        else
				//        {
				//            detailLists += "<td class=\"text-right\" style=\"font-weight:bold\">" + 0 + "</td>\r\n";
				//        }
				//    }
				//    detailLists += "<td class=\"text-right\" style=\"font-weight:bold\">" + Erp.BackOffice.Helpers.Common.PhanCachHangNgan2(listInventory.Where(x => x.ProductId == product.ProductId && x.LoCode == product.LoCode && x.day == product.day && x.month == product.month && x.year == product.year).Sum(x => x.Quantity)).Replace(".", ",") + "</td>\r\n"
				//    + "</tr>\r\n";
				//}
			}
			detailLists += "</tbody>\r\n";
			detailLists += "<tfoot>\r\n"
			+ "<tr>\r\n"
			+ "<td colspan=\"3\"></td>";
			foreach (var ii in warehouseList)
			{
				detailLists += "<td class=\"text-right\" style=\"font-weight:bold\">" + Erp.BackOffice.Helpers.Common.PhanCachHangNgan2(listInventory.Where(x => x.WarehouseId == ii.Id).Sum(x => x.Quantity)).Replace(".", ",") + "</td>\r\n";
			}
			detailLists += "<td class=\"text-right\" style=\"font-weight:bold\">" + Erp.BackOffice.Helpers.Common.PhanCachHangNgan2(listInventory.Sum(x => x.Quantity)).Replace(".", ",") + "</td>\r\n";
			detailLists += "</tr>\r\n"
			+ "</tfoot>\r\n</table>\r\n";


			return detailLists;
		}

		public ActionResult ExportExcel(string txtCode, string manufacturer, string category, string conHang, string WarehouseId, string origin, int? BranchId, bool ExportExcel = false)
		{
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
			model.Content = model.Content.Replace("{DataTable}", buildHtml(txtCode, manufacturer, category, conHang, WarehouseId, origin, BranchId));
			model.Content = model.Content.Replace("{System.Logo}", ImgLogo);
			model.Content = model.Content.Replace("{System.CompanyName}", company);
			model.Content = model.Content.Replace("{System.AddressCompany}", address);
			model.Content = model.Content.Replace("{System.PhoneCompany}", phone);
			model.Content = model.Content.Replace("{System.Fax}", fax);
			model.Content = model.Content.Replace("{System.BankCodeCompany}", bankcode);
			model.Content = model.Content.Replace("{System.BankNameCompany}", bankname);
			model.Content = model.Content.Replace("{PrintDate}", DateTime.Now.ToString("dd/MM/yyyy HH:mm"));
			model.Content = model.Content.Replace("{Title}", "Tồn kho sản phẩm");
			if (ExportExcel)
			{
				Response.AppendHeader("content-disposition", "attachment;filename=" + "DS_Tonkhosanpham" + DateTime.Now.ToString("yyyyMMdd") + ".xls");
				Response.Charset = "";
				Response.Cache.SetCacheability(HttpCacheability.NoCache);
				Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
				Response.Write(model.Content);
				Response.End();
			}
			return View(model);
		}
		#endregion
	}
}
