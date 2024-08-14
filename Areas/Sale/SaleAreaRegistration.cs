using Erp.BackOffice.Sale.Models;
using Erp.Domain.Account.Entities;
using Erp.Domain.Sale.Entities;
using System.Web.Mvc;
using System.Web.UI;

namespace Erp.BackOffice.Sale
{
    public class SaleAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Sale";
            }
        }
        [OutputCache(Duration = 3600 * 24, Location = OutputCacheLocation.Server)]
        public override void RegisterArea(AreaRegistrationContext context)
        {

            context.MapRoute(
            "Sale_SaleCategory",
            "SaleCategory/{action}/{id}",
            new { controller = "SaleCategory", action = "Index", id = UrlParameter.Optional }
            );

            context.MapRoute(
            "Sale_Product",
            "Product/{action}/{id}",
            new { controller = "Product", action = "Index", id = UrlParameter.Optional }
            );
            context.MapRoute(
            "Sale_Supplier",
            "Supplier/{action}/{id}",
            new { controller = "Supplier", action = "Index", id = UrlParameter.Optional }
            );

            context.MapRoute(
            "Sale_Warehouse",
            "Warehouse/{action}/{id}",
            new { controller = "Warehouse", action = "Index", id = UrlParameter.Optional }
            );

            context.MapRoute(
            "Sale_PurchaseOrder",
            "PurchaseOrder/{action}/{id}",
            new { controller = "PurchaseOrder", action = "Index", id = UrlParameter.Optional }
            );

            context.MapRoute(
            "Sale_ProductInvoice",
            "ProductInvoice/{action}/{id}",
            new { controller = "ProductInvoice", action = "Index", id = UrlParameter.Optional }
            );

            context.MapRoute(
            "Sale_Inventory",
            "Inventory/{action}/{id}",
            new { controller = "Inventory", action = "Index", id = UrlParameter.Optional }
            );

            context.MapRoute(
            "Sale_ProductOutbound",
            "ProductOutbound/{action}/{id}",
            new { controller = "ProductOutbound", action = "Index", id = UrlParameter.Optional }
            );

            context.MapRoute(
            "Sale_ProductInBound",
            "ProductInBound/{action}/{id}",
            new { controller = "ProductInBound", action = "Index", id = UrlParameter.Optional }
            );
            context.MapRoute(
            "Sale_PhysicalInventory",
            "PhysicalInventory/{action}/{id}",
            new { controller = "PhysicalInventory", action = "Index", id = UrlParameter.Optional }
            );

            context.MapRoute(
            "Sales_Commision",
            "Commision/{action}/{id}",
            new { controller = "Commision", action = "Index", id = UrlParameter.Optional }
            );

            context.MapRoute(
            "Sale_Report",
            "SaleReport/{action}/{id}",
            new { controller = "SaleReport", action = "Index", id = UrlParameter.Optional }
            );

            context.MapRoute(
            "Sale_SalesReturns",
            "SalesReturns/{action}/{id}",
            new { controller = "SalesReturns", action = "Index", id = UrlParameter.Optional }
            );

            context.MapRoute(
            "Sale_CommisionStaff",
            "CommisionStaff/{action}/{id}",
            new { controller = "CommisionStaff", action = "Index", id = UrlParameter.Optional }
            );


            context.MapRoute(
            "Sale_TemplatePrint",
            "TemplatePrint/{action}/{id}",
            new { controller = "TemplatePrint", action = "Index", id = UrlParameter.Optional }
            );

            context.MapRoute(
            "Sale_Service",
            "Service/{action}/{id}",
            new { controller = "Service", action = "Index", id = UrlParameter.Optional }
            );


            context.MapRoute(
            "Sale_RequestInbound",
            "RequestInbound/{action}/{id}",
            new { controller = "RequestInbound", action = "Index", id = UrlParameter.Optional }
            );

            context.MapRoute(
            "Sale_ProductDamaged",
            "ProductDamaged/{action}/{id}",
            new { controller = "ProductDamaged", action = "Index", id = UrlParameter.Optional }
            );

            context.MapRoute(
            "Sale_Commision_Customer",
            "CommisionCustomer/{action}/{id}",
            new { controller = "CommisionCustomer", action = "Index", id = UrlParameter.Optional }
            );
            context.MapRoute(
            "Sale_ServiceSchedule",
            "ServiceSchedule/{action}/{id}",
            new { controller = "ServiceSchedule", action = "Index", id = UrlParameter.Optional }
            );

            context.MapRoute(
            "Sale_LogLoyaltyPoint",
            "LogLoyaltyPoint/{action}/{id}",
            new { controller = "LogLoyaltyPoint", action = "Index", id = UrlParameter.Optional }
            );

            context.MapRoute(
            "Sale_LoyaltyPoint",
            "LoyaltyPoint/{action}/{id}",
            new { controller = "LoyaltyPoint", action = "Index", id = UrlParameter.Optional }
            );

            context.MapRoute(
            "Sale_CommissionCus",
            "CommissionCus/{action}/{id}",
            new { controller = "CommissionCus", action = "Index", id = UrlParameter.Optional }
            );

            context.MapRoute(
            "Sale_LogVip",
            "LogVip/{action}/{id}",
            new { controller = "LogVip", action = "Index", id = UrlParameter.Optional }
            );
            context.MapRoute(
            "Sale_ProductPackage",
            "ProductPackage/{action}/{id}",
            new { controller = "ProductPackage", action = "Index", id = UrlParameter.Optional }
            );

            context.MapRoute(
            "Sale_Material",
            "Material/{action}/{id}",
            new { controller = "Material", action = "Index", id = UrlParameter.Optional }
            );

            context.MapRoute(
            "Sale_MaterialInbound",
            "MaterialInbound/{action}/{id}",
            new { controller = "MaterialInbound", action = "Index", id = UrlParameter.Optional }
            );

            context.MapRoute(
            "Sale_InventoryMaterial",
            "InventoryMaterial/{action}/{id}",
            new { controller = "InventoryMaterial", action = "Index", id = UrlParameter.Optional }
            );

            context.MapRoute(
            "Sale_PhysicalInventoryMaterial",
            "PhysicalInventoryMaterial/{action}/{id}",
            new { controller = "PhysicalInventoryMaterial", action = "Index", id = UrlParameter.Optional }
            );

            context.MapRoute(
            "Sale_AdviseCard",
            "AdviseCard/{action}/{id}",
            new { controller = "AdviseCard", action = "Index", id = UrlParameter.Optional }
            );
            context.MapRoute(
            "Sale_MaterialOutbound",
            "MaterialOutbound/{action}/{id}",
            new { controller = "MaterialOutbound", action = "Index", id = UrlParameter.Optional }
            );
            context.MapRoute(
            "Sale_InquiryCard",
            "InquiryCard/{action}/{id}",
            new { controller = "InquiryCard", action = "Index", id = UrlParameter.Optional }
            );
            context.MapRoute(
            "Sale_Membership",
            "Membership/{action}/{id}",
            new { controller = "Membership", action = "Index", id = UrlParameter.Optional }
            );

            context.MapRoute(
            "Sale_SchedulingHistory",
            "SchedulingHistory/{action}/{id}",
            new { controller = "SchedulingHistory", action = "Index", id = UrlParameter.Optional }
            );

            context.MapRoute(
            "Sale_ProductSample",
            "ProductSample/{action}/{id}",
            new { controller = "ProductSample", action = "Index", id = UrlParameter.Optional }
            );
            context.MapRoute(
            "Sale_Plan",
            "Plan/{action}/{id}",
            new { controller = "Plan", action = "Index", id = UrlParameter.Optional }
            );
            context.MapRoute(
            "Sale_RequestInboundMaterial",
            "RequestInboundMaterial/{action}/{id}",
            new { controller = "RequestInboundMaterial", action = "Index", id = UrlParameter.Optional }
            );

            context.MapRoute(
            "Sale_TARGET_NVKD",
            "Sale_TARGET_NVKD/{action}/{id}",
            new { controller = "Sale_TARGET_NVKD", action = "Index", id = UrlParameter.Optional }
            );

            context.MapRoute(
            "DM_TINTUC",
            "DM_TINTUC/{action}/{id}",
            new { controller = "DM_TINTUC", action = "Index", id = UrlParameter.Optional }
            );

            context.MapRoute(
            "DM_BAIVE_YHL",
            "DM_BAIVE_YHL/{action}/{id}",
            new { controller = "DM_BAIVE_YHL", action = "Index", id = UrlParameter.Optional }
            );

            context.MapRoute(
            "DM_CAMNHAN_KHANG",
            "DM_CAMNHAN_KHANG/{action}/{id}",
            new { controller = "DM_CAMNHAN_KHANG", action = "Index", id = UrlParameter.Optional }
            );

            context.MapRoute(
            "DM_TIN_KHUYENMAI",
            "DM_TIN_KHUYENMAI/{action}/{id}",
            new { controller = "DM_TIN_KHUYENMAI", action = "Index", id = UrlParameter.Optional }
            );

            context.MapRoute(
             "DM_NGHESY_TINDUNG",
             "DM_NGHESY_TINDUNG/{action}/{id}",
             new { controller = "DM_NGHESY_TINDUNG", action = "Index", id = UrlParameter.Optional }
             );

            context.MapRoute(
            "DiscountCode",
            "DiscountCode/{action}/{id}",
            new { controller = "DiscountCode", action = "Index", id = UrlParameter.Optional }
            );
            //<append_content_route_here>


            RegisterAutoMapperMap();
        }

        private static void RegisterAutoMapperMap()
        {
            AutoMapper.Mapper.CreateMap<Customer, vwCustomer>();
            AutoMapper.Mapper.CreateMap<vwCustomer, Customer>();

            AutoMapper.Mapper.CreateMap<ProductSample, ProductSampleViewModel>();
            AutoMapper.Mapper.CreateMap<ProductSampleViewModel, ProductSample>();
            AutoMapper.Mapper.CreateMap<vwProductSample, ProductSampleViewModel>();
            AutoMapper.Mapper.CreateMap<ProductSampleViewModel, vwProductSample>();
            AutoMapper.Mapper.CreateMap<ProductSampleDetail, ProductSampleDetailViewModel>();
            AutoMapper.Mapper.CreateMap<ProductSampleDetailViewModel, ProductSampleDetail>();
            AutoMapper.Mapper.CreateMap<vwProductSampleDetail, ProductSampleDetailViewModel>();
            AutoMapper.Mapper.CreateMap<ProductSampleDetailViewModel, vwProductSampleDetail>();

            AutoMapper.Mapper.CreateMap<vwMaterialOutbound, MaterialOutboundViewModel>();
            AutoMapper.Mapper.CreateMap<MaterialOutboundViewModel, vwMaterialOutbound>();

            AutoMapper.Mapper.CreateMap<MaterialOutboundViewModel, MaterialOutbound>();
            AutoMapper.Mapper.CreateMap<MaterialOutbound, MaterialOutboundViewModel>();

            AutoMapper.Mapper.CreateMap<MaterialOutbound, MaterialOutboundTransferViewModel>();
            AutoMapper.Mapper.CreateMap<MaterialOutboundTransferViewModel, MaterialOutbound>();

            AutoMapper.Mapper.CreateMap<InventoryMaterial, InventoryMaterialViewModel>();
            AutoMapper.Mapper.CreateMap<InventoryMaterialViewModel, InventoryMaterial>();

            AutoMapper.Mapper.CreateMap<vwMaterialInbound, MaterialInboundViewModel>();
            AutoMapper.Mapper.CreateMap<MaterialInboundViewModel, vwMaterialInbound>();
            AutoMapper.Mapper.CreateMap<MaterialInbound, MaterialInboundViewModel>();
            AutoMapper.Mapper.CreateMap<MaterialInboundViewModel, MaterialInbound>();


            AutoMapper.Mapper.CreateMap<Material, MaterialViewModel>();
            AutoMapper.Mapper.CreateMap<MaterialViewModel, Material>();
            AutoMapper.Mapper.CreateMap<vwMaterial, MaterialViewModel>();

            AutoMapper.Mapper.CreateMap<Domain.Sale.Entities.Product, ProductViewModel>();
            AutoMapper.Mapper.CreateMap<ProductViewModel, Domain.Sale.Entities.Product>();
            AutoMapper.Mapper.CreateMap<Domain.Sale.Entities.vwProduct, ProductViewModel>();
            AutoMapper.Mapper.CreateMap<ServiceViewModel, Domain.Sale.Entities.Product>();

            AutoMapper.Mapper.CreateMap<Domain.Sale.Entities.Supplier, SupplierViewModel>();
            AutoMapper.Mapper.CreateMap<SupplierViewModel, Domain.Sale.Entities.Supplier>();

            AutoMapper.Mapper.CreateMap<Domain.Sale.Entities.Warehouse, WarehouseViewModel>();
            AutoMapper.Mapper.CreateMap<WarehouseViewModel, Domain.Sale.Entities.Warehouse>();

            AutoMapper.Mapper.CreateMap<Domain.Sale.Entities.PurchaseOrder, PurchaseOrderViewModel>();
            AutoMapper.Mapper.CreateMap<PurchaseOrderViewModel, Domain.Sale.Entities.PurchaseOrder>();
            AutoMapper.Mapper.CreateMap<Domain.Sale.Entities.vwPurchaseOrder, PurchaseOrderViewModel>();

            AutoMapper.Mapper.CreateMap<Domain.Sale.Entities.ProductInvoice, ProductInvoiceViewModel>();
            AutoMapper.Mapper.CreateMap<ProductInvoiceViewModel, Domain.Sale.Entities.ProductInvoice>();
            AutoMapper.Mapper.CreateMap<Domain.Sale.Entities.vwProductInvoice, ProductInvoiceViewModel>();
            AutoMapper.Mapper.CreateMap<Domain.Sale.Entities.vwProductInvoice, ProductInvoice>();
            AutoMapper.Mapper.CreateMap<Domain.Sale.Entities.vwProductInvoiceDetail, ProductInvoiceDetailViewModel>();
            AutoMapper.Mapper.CreateMap<ProductInvoiceDetailViewModel, ProductOutboundDetailViewModel>();

            AutoMapper.Mapper.CreateMap<Domain.Sale.Entities.PurchaseOrderDetail, PurchaseOrderDetailViewModel>();
            AutoMapper.Mapper.CreateMap<PurchaseOrderDetailViewModel, Domain.Sale.Entities.PurchaseOrderDetail>();

            AutoMapper.Mapper.CreateMap<Domain.Sale.Entities.Inventory, InventoryViewModel>();
            AutoMapper.Mapper.CreateMap<InventoryViewModel, Domain.Sale.Entities.Inventory>();

            AutoMapper.Mapper.CreateMap<Domain.Sale.Entities.ProductOutbound, ProductOutboundViewModel>();
            AutoMapper.Mapper.CreateMap<ProductOutboundViewModel, Domain.Sale.Entities.ProductOutbound>();
            AutoMapper.Mapper.CreateMap<Domain.Sale.Entities.vwProductOutbound, ProductOutboundViewModel>();
            AutoMapper.Mapper.CreateMap<ProductOutboundViewModel, Domain.Sale.Entities.vwProductOutbound>();

            AutoMapper.Mapper.CreateMap<Domain.Sale.Entities.ProductOutbound, ProductOutboundTransferViewModel>();
            AutoMapper.Mapper.CreateMap<ProductOutboundTransferViewModel, Domain.Sale.Entities.ProductOutbound>();
            AutoMapper.Mapper.CreateMap<Domain.Sale.Entities.vwProductOutbound, ProductOutboundTransferViewModel>();
            AutoMapper.Mapper.CreateMap<ProductOutboundTransferViewModel, Domain.Sale.Entities.vwProductOutbound>();

            AutoMapper.Mapper.CreateMap<Domain.Sale.Entities.ProductInbound, ProductInboundViewModel>();
            AutoMapper.Mapper.CreateMap<ProductInboundViewModel, Domain.Sale.Entities.ProductInbound>();
            AutoMapper.Mapper.CreateMap<Domain.Sale.Entities.vwProductInbound, ProductInboundViewModel>();

            AutoMapper.Mapper.CreateMap<Domain.Sale.Entities.ProductInboundDetail, ProductInboundDetailViewModel>();
            AutoMapper.Mapper.CreateMap<ProductInboundDetailViewModel, Domain.Sale.Entities.ProductInboundDetail>();

            AutoMapper.Mapper.CreateMap<Domain.Sale.Entities.ProductOutboundDetail, ProductOutboundDetailViewModel>();
            AutoMapper.Mapper.CreateMap<ProductOutboundDetailViewModel, Domain.Sale.Entities.ProductOutboundDetail>();
            AutoMapper.Mapper.CreateMap<Domain.Sale.Entities.vwProductOutboundDetail, ProductOutboundDetailViewModel>();
            AutoMapper.Mapper.CreateMap<ProductOutboundDetailViewModel, Domain.Sale.Entities.vwProductOutboundDetail>();

            AutoMapper.Mapper.CreateMap<Domain.Sale.Entities.PhysicalInventory, PhysicalInventoryViewModel>();
            AutoMapper.Mapper.CreateMap<PhysicalInventoryViewModel, Domain.Sale.Entities.PhysicalInventory>();
            AutoMapper.Mapper.CreateMap<Domain.Sale.Entities.vwPhysicalInventory, PhysicalInventoryViewModel>();
            AutoMapper.Mapper.CreateMap<Domain.Sale.Entities.vwPhysicalInventory, PhysicalInventory>();

            AutoMapper.Mapper.CreateMap<Domain.Sale.Entities.PhysicalInventoryDetail, PhysicalInventoryDetailViewModel>();
            AutoMapper.Mapper.CreateMap<PhysicalInventoryDetailViewModel, Domain.Sale.Entities.PhysicalInventoryDetail>();

            AutoMapper.Mapper.CreateMap<Domain.Sale.Entities.ObjectAttribute, ObjectAttributeViewModel>();
            AutoMapper.Mapper.CreateMap<ObjectAttributeViewModel, Domain.Sale.Entities.ObjectAttribute>();

            AutoMapper.Mapper.CreateMap<Domain.Sale.Entities.ObjectAttributeValue, ObjectAttributeValueViewModel>();
            AutoMapper.Mapper.CreateMap<ObjectAttributeValueViewModel, Domain.Sale.Entities.ObjectAttributeValue>();

            AutoMapper.Mapper.CreateMap<Domain.Sale.Entities.Commision, CommisionViewModel>();
            AutoMapper.Mapper.CreateMap<CommisionViewModel, Domain.Sale.Entities.Commision>();

            AutoMapper.Mapper.CreateMap<Domain.Sale.Entities.vwCommision, CommisionViewModel>();
            AutoMapper.Mapper.CreateMap<CommisionViewModel, Domain.Sale.Entities.vwCommision>();

            AutoMapper.Mapper.CreateMap<Domain.Sale.Entities.WarehouseLocationItem, WarehouseLocationItemViewModel>();
            AutoMapper.Mapper.CreateMap<WarehouseLocationItemViewModel, Domain.Sale.Entities.WarehouseLocationItem>();

            AutoMapper.Mapper.CreateMap<Domain.Sale.Entities.WarehouseLocationItem, vwWarehouseLocationItem>();
            AutoMapper.Mapper.CreateMap<vwWarehouseLocationItem, Domain.Sale.Entities.WarehouseLocationItem>();

            AutoMapper.Mapper.CreateMap<Domain.Sale.Entities.SalesReturns, SalesReturnsViewModel>();
            AutoMapper.Mapper.CreateMap<SalesReturnsViewModel, Domain.Sale.Entities.SalesReturns>();
            AutoMapper.Mapper.CreateMap<Domain.Sale.Entities.vwSalesReturns, SalesReturnsViewModel>();
            AutoMapper.Mapper.CreateMap<SalesReturnsViewModel, Domain.Sale.Entities.vwSalesReturns>();
            AutoMapper.Mapper.CreateMap<SalesReturnsDetailViewModel, Domain.Sale.Entities.SalesReturnsDetail>();
            AutoMapper.Mapper.CreateMap<SalesReturnsDetail, SalesReturnsDetailViewModel>();
            AutoMapper.Mapper.CreateMap<SalesReturnsDetailViewModel, Domain.Sale.Entities.vwSalesReturnsDetail>();
            AutoMapper.Mapper.CreateMap<vwSalesReturnsDetail, SalesReturnsDetailViewModel>();

            AutoMapper.Mapper.CreateMap<Domain.Sale.Entities.Promotion, PromotionViewModel>();
            AutoMapper.Mapper.CreateMap<PromotionViewModel, Domain.Sale.Entities.Promotion>();

            AutoMapper.Mapper.CreateMap<Domain.Sale.Entities.PromotionDetail, PromotionDetailViewModel>();
            AutoMapper.Mapper.CreateMap<PromotionDetailViewModel, Domain.Sale.Entities.PromotionDetail>();

            AutoMapper.Mapper.CreateMap<Domain.Sale.Entities.vwProductInvoice, SalesReturnsViewModel>();
            AutoMapper.Mapper.CreateMap<SalesReturnsViewModel, Domain.Sale.Entities.vwProductInvoice>();

            AutoMapper.Mapper.CreateMap<Domain.Sale.Entities.CommisionStaff, CommisionStaffViewModel>();
            AutoMapper.Mapper.CreateMap<CommisionStaffViewModel, Domain.Sale.Entities.CommisionStaff>();

            AutoMapper.Mapper.CreateMap<Domain.Sale.Entities.TemplatePrint, TemplatePrintViewModel>();
            AutoMapper.Mapper.CreateMap<TemplatePrintViewModel, Domain.Sale.Entities.TemplatePrint>();


            AutoMapper.Mapper.CreateMap<Domain.Sale.Entities.vwService, ServiceViewModel>();
            AutoMapper.Mapper.CreateMap<ServiceViewModel, Domain.Sale.Entities.vwService>();

            AutoMapper.Mapper.CreateMap<Domain.Sale.Entities.UsingServiceLog, UsingServiceLogViewModel>();
            AutoMapper.Mapper.CreateMap<UsingServiceLogViewModel, Domain.Sale.Entities.UsingServiceLog>();
            AutoMapper.Mapper.CreateMap<Domain.Sale.Entities.vwUsingServiceLog, UsingServiceLogViewModel>();
            AutoMapper.Mapper.CreateMap<UsingServiceLogViewModel, Domain.Sale.Entities.vwUsingServiceLog>();

            AutoMapper.Mapper.CreateMap<Domain.Sale.Entities.ServiceCombo, ServiceComboViewModel>();
            AutoMapper.Mapper.CreateMap<ServiceComboViewModel, Domain.Sale.Entities.ServiceCombo>();
            AutoMapper.Mapper.CreateMap<Domain.Sale.Entities.vwServiceCombo, ServiceComboViewModel>();
            AutoMapper.Mapper.CreateMap<ServiceComboViewModel, Domain.Sale.Entities.vwServiceCombo>();

            AutoMapper.Mapper.CreateMap<Domain.Sale.Entities.RequestInbound, RequestInboundViewModel>();
            AutoMapper.Mapper.CreateMap<RequestInboundViewModel, Domain.Sale.Entities.RequestInbound>();
            AutoMapper.Mapper.CreateMap<Domain.Sale.Entities.vwRequestInbound, RequestInboundViewModel>();
            AutoMapper.Mapper.CreateMap<RequestInboundViewModel, Domain.Sale.Entities.vwRequestInbound>();

            AutoMapper.Mapper.CreateMap<Domain.Sale.Entities.RequestInboundDetail, RequestInboundDetailViewModel>();
            AutoMapper.Mapper.CreateMap<RequestInboundDetailViewModel, Domain.Sale.Entities.RequestInboundDetail>();
            AutoMapper.Mapper.CreateMap<Domain.Sale.Entities.vwRequestInboundDetail, RequestInboundDetailViewModel>();
            AutoMapper.Mapper.CreateMap<RequestInboundDetailViewModel, Domain.Sale.Entities.vwRequestInboundDetail>();

            AutoMapper.Mapper.CreateMap<Domain.Sale.Entities.UsingServiceLogDetail, UsingServiceLogDetailViewModel>();
            AutoMapper.Mapper.CreateMap<UsingServiceLogDetailViewModel, Domain.Sale.Entities.UsingServiceLogDetail>();
            AutoMapper.Mapper.CreateMap<Domain.Sale.Entities.vwUsingServiceLogDetail, UsingServiceLogDetailViewModel>();
            AutoMapper.Mapper.CreateMap<UsingServiceLogDetailViewModel, Domain.Sale.Entities.vwUsingServiceLogDetail>();

            AutoMapper.Mapper.CreateMap<Domain.Sale.Entities.ProductDamaged, ProductDamagedViewModel>();
            AutoMapper.Mapper.CreateMap<ProductDamagedViewModel, Domain.Sale.Entities.ProductDamaged>();

            AutoMapper.Mapper.CreateMap<Domain.Sale.Entities.ServiceReminder, ServiceReminderViewModel>();
            AutoMapper.Mapper.CreateMap<ServiceReminderViewModel, Domain.Sale.Entities.ServiceReminder>();

            AutoMapper.Mapper.CreateMap<Domain.Sale.Entities.ServiceReminderGroup, ServiceReminderGroupViewModel>();
            AutoMapper.Mapper.CreateMap<ServiceReminderGroupViewModel, Domain.Sale.Entities.ServiceReminderGroup>();

            AutoMapper.Mapper.CreateMap<Domain.Sale.Entities.LogServiceRemminder, LogServiceRemminderViewModel>();
            AutoMapper.Mapper.CreateMap<LogServiceRemminderViewModel, Domain.Sale.Entities.LogServiceRemminder>();

            AutoMapper.Mapper.CreateMap<Domain.Sale.Entities.CommisionCustomer, CommisionCustomerViewModel>();
            AutoMapper.Mapper.CreateMap<CommisionCustomerViewModel, Domain.Sale.Entities.CommisionCustomer>();

            AutoMapper.Mapper.CreateMap<Domain.Sale.Entities.vwCommisionCustomer, CommisionCustomerViewModel>();
            AutoMapper.Mapper.CreateMap<CommisionCustomerViewModel, Domain.Sale.Entities.vwCommisionCustomer>();

            AutoMapper.Mapper.CreateMap<Domain.Sale.Entities.ServiceSchedule, ServiceScheduleViewModel>();
            AutoMapper.Mapper.CreateMap<ServiceScheduleViewModel, Domain.Sale.Entities.ServiceSchedule>();
            AutoMapper.Mapper.CreateMap<Domain.Sale.Entities.vwServiceSchedule, ServiceScheduleViewModel>();
            AutoMapper.Mapper.CreateMap<ServiceScheduleViewModel, Domain.Sale.Entities.vwServiceSchedule>();

            AutoMapper.Mapper.CreateMap<Domain.Sale.Entities.LogLoyaltyPoint, LogLoyaltyPointViewModel>();
            AutoMapper.Mapper.CreateMap<LogLoyaltyPointViewModel, Domain.Sale.Entities.LogLoyaltyPoint>();
            AutoMapper.Mapper.CreateMap<Domain.Sale.Entities.vwLogLoyaltyPoint, LogLoyaltyPointViewModel>();
            AutoMapper.Mapper.CreateMap<LogLoyaltyPointViewModel, Domain.Sale.Entities.vwLogLoyaltyPoint>();

            AutoMapper.Mapper.CreateMap<Domain.Sale.Entities.LoyaltyPoint, LoyaltyPointViewModel>();
            AutoMapper.Mapper.CreateMap<LoyaltyPointViewModel, Domain.Sale.Entities.LoyaltyPoint>();

            AutoMapper.Mapper.CreateMap<Domain.Sale.Entities.CommissionCus, CommissionCusViewModel>();
            AutoMapper.Mapper.CreateMap<CommissionCusViewModel, Domain.Sale.Entities.CommissionCus>();

            AutoMapper.Mapper.CreateMap<Domain.Sale.Entities.TotalDiscountMoneyNT, TotalDiscountMoneyNTViewModel>();
            AutoMapper.Mapper.CreateMap<TotalDiscountMoneyNTViewModel, Domain.Sale.Entities.TotalDiscountMoneyNT>();
            AutoMapper.Mapper.CreateMap<Domain.Sale.Entities.vwTotalDiscountMoneyNT, TotalDiscountMoneyNTViewModel>();
            AutoMapper.Mapper.CreateMap<TotalDiscountMoneyNTViewModel, Domain.Sale.Entities.vwTotalDiscountMoneyNT>();

            AutoMapper.Mapper.CreateMap<Domain.Sale.Entities.LogVip, LogVipViewModel>();
            AutoMapper.Mapper.CreateMap<LogVipViewModel, Domain.Sale.Entities.LogVip>();
            AutoMapper.Mapper.CreateMap<Domain.Sale.Entities.vwLogVip,LogVipViewModel>();
            AutoMapper.Mapper.CreateMap<LogVipViewModel, Domain.Sale.Entities.vwLogVip>();

            AutoMapper.Mapper.CreateMap<Domain.Sale.Entities.AdviseCard, AdviseCardViewModel>();
            AutoMapper.Mapper.CreateMap<AdviseCardViewModel, Domain.Sale.Entities.AdviseCard>();
            AutoMapper.Mapper.CreateMap<Domain.Sale.Entities.vwAdviseCard, AdviseCardViewModel>();
            AutoMapper.Mapper.CreateMap<AdviseCardViewModel, Domain.Sale.Entities.vwAdviseCard>();
            AutoMapper.Mapper.CreateMap<Domain.Sale.Entities.AdviseCardDetail, AdviseCardDetailViewModel>();
            AutoMapper.Mapper.CreateMap<AdviseCardDetailViewModel, Domain.Sale.Entities.AdviseCardDetail>();

            AutoMapper.Mapper.CreateMap<Domain.Sale.Entities.InquiryCard, InquiryCardViewModel>();
            AutoMapper.Mapper.CreateMap<InquiryCardViewModel, Domain.Sale.Entities.InquiryCard>();
            AutoMapper.Mapper.CreateMap<Domain.Sale.Entities.vwInquiryCard, InquiryCardViewModel>();
            AutoMapper.Mapper.CreateMap<InquiryCardViewModel, Domain.Sale.Entities.vwInquiryCard>();
            AutoMapper.Mapper.CreateMap<Domain.Sale.Entities.InquiryCardDetail, InquiryCardViewModel>();
            AutoMapper.Mapper.CreateMap<InquiryCardViewModel, Domain.Sale.Entities.InquiryCardDetail>();

            AutoMapper.Mapper.CreateMap<Domain.Sale.Entities.Membership, MembershipViewModel>();
            AutoMapper.Mapper.CreateMap<MembershipViewModel, Domain.Sale.Entities.Membership>();
            AutoMapper.Mapper.CreateMap<Domain.Sale.Entities.vwMembership, MembershipViewModel>();
            AutoMapper.Mapper.CreateMap<MembershipViewModel, Domain.Sale.Entities.vwMembership>();

            AutoMapper.Mapper.CreateMap<Domain.Sale.Entities.Membership_parent, Membership_parentViewModel>();
            AutoMapper.Mapper.CreateMap<Membership_parentViewModel, Domain.Sale.Entities.Membership_parent>();
            AutoMapper.Mapper.CreateMap<Domain.Sale.Entities.vwMembership_parent, Membership_parentViewModel>();

            AutoMapper.Mapper.CreateMap<Domain.Sale.Entities.SchedulingHistory, SchedulingHistoryViewModel>();
            AutoMapper.Mapper.CreateMap<SchedulingHistoryViewModel, Domain.Sale.Entities.SchedulingHistory>();
            AutoMapper.Mapper.CreateMap<Domain.Sale.Entities.vwSchedulingHistory, SchedulingHistoryViewModel>();
            AutoMapper.Mapper.CreateMap<SchedulingHistoryViewModel, Domain.Sale.Entities.vwSchedulingHistory>();

            AutoMapper.Mapper.CreateMap<Domain.Sale.Entities.StaffMade, StaffMadeViewModel>();
            AutoMapper.Mapper.CreateMap<StaffMadeViewModel, Domain.Sale.Entities.StaffMade>();

            AutoMapper.Mapper.CreateMap<Domain.Sale.Entities.vwStaffMade, StaffMadeViewModel>();
            AutoMapper.Mapper.CreateMap<StaffMadeViewModel, Domain.Sale.Entities.vwStaffMade>();

            AutoMapper.Mapper.CreateMap<Domain.Sale.Entities.DonateProOrSer, DonateProOrSerViewModel>();
            AutoMapper.Mapper.CreateMap<DonateProOrSerViewModel, Domain.Sale.Entities.DonateProOrSer>();
            AutoMapper.Mapper.CreateMap<Domain.Sale.Entities.vwDonateProOrSer, DonateProOrSerViewModel>();
            AutoMapper.Mapper.CreateMap<DonateProOrSerViewModel, Domain.Sale.Entities.vwDonateProOrSer>();

            AutoMapper.Mapper.CreateMap<Domain.Sale.Entities.ProductDetail, ProductDetailViewModel>();
            AutoMapper.Mapper.CreateMap<DonateProOrSerViewModel, Domain.Sale.Entities.ProductDetail>();
            AutoMapper.Mapper.CreateMap<Domain.Sale.Entities.vwProductDetail, ProductDetailViewModel>();
            AutoMapper.Mapper.CreateMap<DonateProOrSerViewModel, Domain.Sale.Entities.vwProductDetail>();

            AutoMapper.Mapper.CreateMap<Domain.Sale.Entities.vwProductAndService, ProductViewModel>();
            AutoMapper.Mapper.CreateMap<ProductViewModel, Domain.Sale.Entities.vwProductAndService>();

            AutoMapper.Mapper.CreateMap<Domain.Sale.Entities.PhysicalInventoryMaterial, PhysicalInventoryMaterialViewModel>();
            AutoMapper.Mapper.CreateMap<PhysicalInventoryMaterialViewModel, Domain.Sale.Entities.PhysicalInventoryMaterial>();
            AutoMapper.Mapper.CreateMap<Domain.Sale.Entities.vwPhysicalInventoryMaterial, PhysicalInventoryMaterialViewModel>();
            AutoMapper.Mapper.CreateMap<Domain.Sale.Entities.vwPhysicalInventoryMaterial, PhysicalInventoryMaterial>();

            AutoMapper.Mapper.CreateMap<Domain.Sale.Entities.PhysicalInventoryMaterialDetail, PhysicalInventoryMaterialDetailViewModel>();
            AutoMapper.Mapper.CreateMap<PhysicalInventoryMaterialDetailViewModel, Domain.Sale.Entities.PhysicalInventoryMaterialDetail>();
            AutoMapper.Mapper.CreateMap<Domain.Sale.Entities.LogPromotion, LogPromotionViewModel>();
            AutoMapper.Mapper.CreateMap<LogPromotionViewModel, Domain.Sale.Entities.LogPromotion>();
            AutoMapper.Mapper.CreateMap<Domain.Sale.Entities.vwLogPromotion, LogPromotionViewModel>();
            AutoMapper.Mapper.CreateMap<LogPromotionViewModel, Domain.Sale.Entities.vwLogPromotion>();

            AutoMapper.Mapper.CreateMap<Domain.Crm.Entities.KH_TUONGTAC, KH_TUONGTACViewModel>();
            AutoMapper.Mapper.CreateMap<KH_TUONGTACViewModel, Domain.Crm.Entities.KH_TUONGTAC>();
            AutoMapper.Mapper.CreateMap<Domain.Crm.Entities.vwCRM_KH_TUONGTAC, KH_TUONGTACViewModel>();
            AutoMapper.Mapper.CreateMap<KH_TUONGTACViewModel, Domain.Crm.Entities.vwCRM_KH_TUONGTAC>();

            AutoMapper.Mapper.CreateMap<Domain.Sale.Entities.HOAHONG_NVKD, HOAHONG_NVKDViewModel>();
            AutoMapper.Mapper.CreateMap<HOAHONG_NVKDViewModel, Domain.Sale.Entities.HOAHONG_NVKD>();

            AutoMapper.Mapper.CreateMap<Domain.Sale.Entities.Sale_TARGET_NVKD, HoaHongChiNhanhViewModel>();
            AutoMapper.Mapper.CreateMap<HoaHongChiNhanhViewModel, Domain.Sale.Entities.Sale_TARGET_NVKD>();


            AutoMapper.Mapper.CreateMap<Domain.Sale.Entities.DM_NHOMSANPHAM, DM_NHOMSANPHAMViewModel>();
            AutoMapper.Mapper.CreateMap<DM_NHOMSANPHAMViewModel, Domain.Sale.Entities.DM_NHOMSANPHAM>();

            //AutoMapper.Mapper.CreateMap<Domain.Sale.Entities.DM_LOAISANPHAM, DM_LOAISANPHAMViewModel>();
            //AutoMapper.Mapper.CreateMap<DM_LOAISANPHAMViewModel, Domain.Sale.Entities.DM_LOAISANPHAM>();

            AutoMapper.Mapper.CreateMap<Domain.Sale.Entities.DM_BEST_SELLER, DM_BEST_SELLERViewModel>();
            AutoMapper.Mapper.CreateMap<DM_BEST_SELLERViewModel, Domain.Sale.Entities.DM_BEST_SELLER>();

            AutoMapper.Mapper.CreateMap<Domain.Sale.Entities.DM_DEALHOT, DM_DEALHOTViewModel>();
            AutoMapper.Mapper.CreateMap<DM_DEALHOTViewModel, Domain.Sale.Entities.DM_DEALHOT>();

            AutoMapper.Mapper.CreateMap<Domain.Sale.Entities.DM_BANNER_SLIDER, DM_BANNER_SLIDERViewModel>();
            AutoMapper.Mapper.CreateMap<DM_BANNER_SLIDERViewModel, Domain.Sale.Entities.DM_BANNER_SLIDER>();

            AutoMapper.Mapper.CreateMap<Domain.Sale.Entities.DM_TINTUC, DM_TINTUCViewModel>();
            AutoMapper.Mapper.CreateMap<DM_TINTUCViewModel, Domain.Sale.Entities.DM_TINTUC>();

            AutoMapper.Mapper.CreateMap<Domain.Sale.Entities.DM_BAIVE_YHL, DM_BAIVE_YHLViewModel>();
            AutoMapper.Mapper.CreateMap<DM_BAIVE_YHLViewModel, Domain.Sale.Entities.DM_BAIVE_YHL>();

            AutoMapper.Mapper.CreateMap<Domain.Sale.Entities.DM_NHOMTIN, DM_NHOMTINViewModel>();
            AutoMapper.Mapper.CreateMap<DM_NHOMTINViewModel, Domain.Sale.Entities.DM_NHOMTIN>();

            AutoMapper.Mapper.CreateMap<Domain.Sale.Entities.DM_CAMNHAN_KHANG, DM_CAMNHAN_KHANGViewModel>();
            AutoMapper.Mapper.CreateMap<DM_CAMNHAN_KHANGViewModel, Domain.Sale.Entities.DM_CAMNHAN_KHANG>();

            AutoMapper.Mapper.CreateMap<Domain.Sale.Entities.DM_LOAIBAI, DM_LOAIBAIViewModel>();
            AutoMapper.Mapper.CreateMap<DM_LOAIBAIViewModel, Domain.Sale.Entities.DM_LOAIBAI>();

            AutoMapper.Mapper.CreateMap<Domain.Sale.Entities.DM_TIN_KHUYENMAI, DM_TIN_KHUYENMAIViewModel>();
            AutoMapper.Mapper.CreateMap<DM_TIN_KHUYENMAIViewModel, Domain.Sale.Entities.DM_TIN_KHUYENMAI>();

            AutoMapper.Mapper.CreateMap<Domain.Sale.Entities.DM_BAOCHI, DM_BAOCHIViewModel>();
            AutoMapper.Mapper.CreateMap<DM_BAOCHIViewModel, Domain.Sale.Entities.DM_BAOCHI>();

            AutoMapper.Mapper.CreateMap<Domain.Sale.Entities.DiscountCode, DiscountCodeViewModel>();
            AutoMapper.Mapper.CreateMap<DiscountCodeViewModel, Domain.Sale.Entities.DiscountCode>();
            //AutoMapper.Mapper.CreateMap<Domain.Sale.Entities.DM_NGHESY_TINDUNG, DM_NGHESY_TINDUNGViewModel>();
            //AutoMapper.Mapper.CreateMap<DM_NGHESY_TINDUNGViewModel, Domain.Sale.Entities.DM_NGHESY_TINDUNG>();

            //<append_content_mapper_here>
        }
    }
}
