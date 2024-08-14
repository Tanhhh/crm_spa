
using Erp.BackOffice.Areas.Crm.Models;
using Erp.BackOffice.Crm.Models;
using Erp.BackOffice.Sale.Models;
//using Erp.Domain.Entities;
using System.Web.Mvc;

namespace Erp.BackOffice.Crm
{
    public class CrmAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Crm";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                 "Crm_Campaign",
                 "Campaign/{action}/{id}",
                 new { controller = "Campaign", action = "Index", id = UrlParameter.Optional }
             );

            context.MapRoute(
                 "Crm_Process",
                 "Process/{action}/{id}",
                 new { controller = "Process", action = "Index", id = UrlParameter.Optional }
             );

            context.MapRoute(
                "Crm_ProcessAction",
                "ProcessAction/{action}/{id}",
                new { controller = "ProcessAction", action = "Index", id = UrlParameter.Optional }
            );

            context.MapRoute(
            "Crm_Task",
            "Task/{action}/{id}",
            new { controller = "Task", action = "Index", id = UrlParameter.Optional }
            );

            context.MapRoute(
            "Crm_ProcessStage",
            "ProcessStage/{action}/{id}",
            new { controller = "ProcessStage", action = "Index", id = UrlParameter.Optional }
            );

            context.MapRoute(
            "Crm_ProcessStep",
            "ProcessStep/{action}/{id}",
            new { controller = "ProcessStep", action = "Index", id = UrlParameter.Optional }
            );
            context.MapRoute(
          "Crm_Report",
          "CrmReport/{action}/{id}",
          new { controller = "CrmReport", action = "Index", id = UrlParameter.Optional }
          );
            context.MapRoute(
            "Crm_Question",
            "Question/{action}/{id}",
            new { controller = "Question", action = "Index", id = UrlParameter.Optional }
            );

            context.MapRoute(
            "Crm_Answer",
            "Answer/{action}/{id}",
            new { controller = "Answer", action = "Index", id = UrlParameter.Optional }
            );

            context.MapRoute(
            "Crm_Vote",
            "Vote/{action}/{id}",
            new { controller = "Vote", action = "Index", id = UrlParameter.Optional }
            );

            context.MapRoute(
            "Crm_ProcessApplied",
            "ProcessApplied/{action}/{id}",
            new { controller = "ProcessApplied", action = "Index", id = UrlParameter.Optional }
            );

            context.MapRoute(
            "Crm_SMSLog",
            "SMSLog/{action}/{id}",
            new { controller = "SMSLog", action = "Index", id = UrlParameter.Optional }
            );

            context.MapRoute(
            "Crm_EmailLog",
            "EmailLog/{action}/{id}",
            new { controller = "EmailLog", action = "Index", id = UrlParameter.Optional }
            );
              context.MapRoute(
            "Crm_Image",
            "Image/{action}/{id}",
            new { controller = "Image", action = "Index", id = UrlParameter.Optional }
            );
             context.MapRoute(
             "Crm_KH_GIOITHIEU",
             "KH_GIOITHIEU/{action}/{id}",
             new { controller = "KH_GIOITHIEU", action = "Index", id = UrlParameter.Optional }
            );

            context.MapRoute(
            "Crm_CRM_KH_BANHANG",
            "CRM_KH_BANHANG/{action}/{id}",
            new { controller = "CRM_KH_BANHANG", action = "Index", id = UrlParameter.Optional }
           );

            context.MapRoute(
            "Crm_CRM_KH_BANHANG_DEMO",
            "CRM_KH_BANHANG_DEMO/{action}/{id}",
            new { controller = "CRM_KH_BANHANG_DEMO", action = "Index", id = UrlParameter.Optional }
           );

            context.MapRoute(
            "Crm_BH_DOANHSO",
            "BH_DOANHSO/{action}/{id}",
            new { controller = "BH_DOANHSO", action = "Index", id = UrlParameter.Optional }
            );

            context.MapRoute(
           "CALLog",
           "CALLog/{action}/{id}",
           new { controller = "CALLog", action = "Index", id = UrlParameter.Optional }
           );
            //<append_content_route_here>
            context.MapRoute("Crm_SystemParameters",
            "SystemParameters/{action}/{id}",
            new { controller = "SystemParameters", action = "Index", id = UrlParameter.Optional });

            context.MapRoute(
            "Crm_CrmTemplate",
            "CrmTemplate/{action}/{id}",
            new { controller = "CrmTemplate", action = "Index", id = UrlParameter.Optional }
            );
            context.MapRoute(
            "Crm_LeadMeeting",
            "LeadMeeting/{action}/{id}",
            new { controller = "LeadMeeting", action = "Index", id = UrlParameter.Optional }
            );
            context.MapRoute(
            "Crm_StatusLead",
            "StatusLead/{action}/{id}",
            new { controller = "StatusLead", action = "Index", id = UrlParameter.Optional }
            );
            context.MapRoute(
            "Crm_SettingDisplayLead",
            "SettingDisplayLead/{action}/{id}",
            new { controller = "SettingDisplayLead", action = "Index", id = UrlParameter.Optional }
            );
            context.MapRoute(
            "Crm_ImportExcel",
            "ImportExcel/{action}/{id}",
            new { controller = "ImportExcel", action = "SetupImport", id = UrlParameter.Optional }
            );
            context.MapRoute(
            "Crm_AssignedToU",
            "AssignedToU/{action}/{id}",
            new { controller = "AssignedToU", action = "Index", id = UrlParameter.Optional }
            );
            context.MapRoute(
            "Crm_CrmTemplateFile",
            "CrmTemplateFile/{action}/{id}",
            new { controller = "CrmTemplateFile", action = "Index", id = UrlParameter.Optional }
            );
            context.MapRoute(
            "Crm_ProcessDiagram",
            "ProcessDiagram/{action}/{id}",
            new { controller = "ProcessDiagram", action = "TableWork", id = UrlParameter.Optional }
            );
            context.MapRoute(
            "Crm_ActivitiesLead",
            "ActivitiesLead/{action}/{id}",
            new { controller = "ActivitiesLead", action = "ActivitiesLeadIndex", id = UrlParameter.Optional }
            );
            context.MapRoute(
             "Crm_CrmTargetAllocation",
             "CrmTargetAllocation/{action}/{id}",
             new { controller = "CrmTargetAllocation", action = "Index", id = UrlParameter.Optional }
             );
            context.MapRoute(
              "Crm_OrderTBDH",
              "OrderTBDH/{action}/{id}",
              new { controller = "OrderTBDH", action = "Index", id = UrlParameter.Optional }
              );
            context.MapRoute(
            "Crm_Appointment",
            "Appointment/{action}/{id}",
            new { controller = "Appointment", action = "Index", id = UrlParameter.Optional }
            );
            RegisterAutoMapperMap();
        }

        private static void RegisterAutoMapperMap()
        {
            AutoMapper.Mapper.CreateMap<Domain.Crm.Entities.Campaign, CampaignViewModel>();
            AutoMapper.Mapper.CreateMap<CampaignViewModel, Domain.Crm.Entities.Campaign>();

            AutoMapper.Mapper.CreateMap<Domain.Crm.Entities.Process, ProcessViewModel>();
            AutoMapper.Mapper.CreateMap<ProcessViewModel, Domain.Crm.Entities.Process>();
            AutoMapper.Mapper.CreateMap<Domain.Crm.Entities.ProcessAction, ProcessActionViewModel>();
            AutoMapper.Mapper.CreateMap<ProcessActionViewModel, Domain.Crm.Entities.ProcessAction>();

            AutoMapper.Mapper.CreateMap<Domain.Crm.Entities.Task, TaskViewModel>();
            AutoMapper.Mapper.CreateMap<TaskViewModel, Domain.Crm.Entities.Task>();
            AutoMapper.Mapper.CreateMap<TaskTemplateViewModel, Domain.Crm.Entities.Task>();
            AutoMapper.Mapper.CreateMap<Domain.Crm.Entities.vwTask, TaskViewModel>();
            AutoMapper.Mapper.CreateMap<TaskViewModel, Domain.Crm.Entities.vwTask>();

            AutoMapper.Mapper.CreateMap<Domain.Crm.Entities.ProcessStage, ProcessStageViewModel>();
            AutoMapper.Mapper.CreateMap<ProcessStageViewModel, Domain.Crm.Entities.ProcessStage>();

            AutoMapper.Mapper.CreateMap<Domain.Crm.Entities.ProcessStep, ProcessStepViewModel>();
            AutoMapper.Mapper.CreateMap<ProcessStepViewModel, Domain.Crm.Entities.ProcessStep>();

            AutoMapper.Mapper.CreateMap<Domain.Crm.Entities.Question, QuestionViewModel>();
            AutoMapper.Mapper.CreateMap<QuestionViewModel, Domain.Crm.Entities.Question>();

            AutoMapper.Mapper.CreateMap<Domain.Crm.Entities.Answer, AnswerViewModel>();
            AutoMapper.Mapper.CreateMap<AnswerViewModel, Domain.Crm.Entities.Answer>();

            AutoMapper.Mapper.CreateMap<Domain.Crm.Entities.ProcessApplied, ProcessAppliedViewModel>();
            AutoMapper.Mapper.CreateMap<ProcessAppliedViewModel, Domain.Crm.Entities.ProcessApplied>();

            AutoMapper.Mapper.CreateMap<Domain.Crm.Entities.SMSLog, SMSLogViewModel>();
            AutoMapper.Mapper.CreateMap<SMSLogViewModel, Domain.Crm.Entities.SMSLog>();

            AutoMapper.Mapper.CreateMap<Domain.Crm.Entities.vwSMSLog, SMSLogViewModel>();
            AutoMapper.Mapper.CreateMap<SMSLogViewModel, Domain.Crm.Entities.vwSMSLog>();

            AutoMapper.Mapper.CreateMap<Domain.Crm.Entities.EmailLog, EmailLogViewModel>();
            AutoMapper.Mapper.CreateMap<EmailLogViewModel, Domain.Crm.Entities.EmailLog>();

            AutoMapper.Mapper.CreateMap<Domain.Crm.Entities.vwEmailLog, EmailLogViewModel>();
            AutoMapper.Mapper.CreateMap<EmailLogViewModel, Domain.Crm.Entities.vwEmailLog>();

            AutoMapper.Mapper.CreateMap<Domain.Crm.Entities.vwVote2, vwVoteViewModel>();
            AutoMapper.Mapper.CreateMap<vwVoteViewModel, Domain.Crm.Entities.vwVote2>();
            //AutoMapper.Mapper.CreateMap<Domain.Crm.Entities.Image, ImageViewModel>();
            //AutoMapper.Mapper.CreateMap<ImageViewModel, Domain.Crm.Entities.Image>();

            AutoMapper.Mapper.CreateMap<Domain.Crm.Entities.CRM_KH_GIOITHIEU, CRM_KH_GIOITHIEUViewModel>();
            AutoMapper.Mapper.CreateMap<CRM_KH_GIOITHIEUViewModel, Domain.Crm.Entities.CRM_KH_GIOITHIEU>();
            AutoMapper.Mapper.CreateMap<Domain.Crm.Entities.vwKH_GIOITHIEU, CRM_KH_GIOITHIEUViewModel>();
            AutoMapper.Mapper.CreateMap<CRM_KH_GIOITHIEUViewModel, Domain.Crm.Entities.vwKH_GIOITHIEU>();

            AutoMapper.Mapper.CreateMap<Domain.Crm.Entities.CRM_KH_BANHANG, CRM_KH_BANHANGViewModel>();
            AutoMapper.Mapper.CreateMap<CRM_KH_BANHANGViewModel, Domain.Crm.Entities.CRM_KH_BANHANG>();
            AutoMapper.Mapper.CreateMap<Domain.Crm.Entities.vwCRM_KH_BANHANG, CRM_KH_BANHANGViewModel>();
            AutoMapper.Mapper.CreateMap<CRM_KH_BANHANGViewModel, Domain.Crm.Entities.vwCRM_KH_BANHANG>();
            AutoMapper.Mapper.CreateMap<Domain.Crm.Entities.CRM_KH_BANHANG_CTIET, CRM_KH_BANHANGViewModel>();
            AutoMapper.Mapper.CreateMap<CRM_KH_BANHANGViewModel, Domain.Crm.Entities.CRM_KH_BANHANG_CTIET>();
            AutoMapper.Mapper.CreateMap<Domain.Crm.Entities.vwCRM_KH_BANHANG_CTIET, CRM_KH_BANHANGViewModel>();
            AutoMapper.Mapper.CreateMap<CRM_KH_BANHANGViewModel, Domain.Crm.Entities.vwCRM_KH_BANHANG_CTIET>();
            AutoMapper.Mapper.CreateMap<Domain.Crm.Entities.vwCustomer_ProductInvoice, CRM_KH_BANHANGViewModel>();
            AutoMapper.Mapper.CreateMap<CRM_KH_BANHANGViewModel, Domain.Crm.Entities.vwCustomer_ProductInvoice>();
            AutoMapper.Mapper.CreateMap<Domain.Crm.Entities.CRM_Sale_ProductInvoice, ProductInvoiceViewModel>();
            AutoMapper.Mapper.CreateMap<ProductInvoiceViewModel, Domain.Crm.Entities.CRM_Sale_ProductInvoice>();
            AutoMapper.Mapper.CreateMap<Domain.Crm.Entities.vwCRM_Sale_ProductInvoice, ProductInvoiceViewModel>();
            AutoMapper.Mapper.CreateMap<ProductInvoiceViewModel, Domain.Crm.Entities.vwCRM_Sale_ProductInvoice>();

            AutoMapper.Mapper.CreateMap<Domain.Crm.Entities.vwCRM_Sale_ProductInvoice, Domain.Crm.Entities.CRM_Sale_ProductInvoice>();
            AutoMapper.Mapper.CreateMap<Domain.Crm.Entities.CRM_Sale_ProductInvoice, Domain.Crm.Entities.vwCRM_Sale_ProductInvoice>();
            AutoMapper.Mapper.CreateMap<Domain.Crm.Entities.vwCRM_Sale_ProductInvoicePlan, Domain.Crm.Entities.CRM_Sale_ProductInvoice>();
            AutoMapper.Mapper.CreateMap<Domain.Crm.Entities.CRM_Sale_ProductInvoice, Domain.Crm.Entities.vwCRM_Sale_ProductInvoicePlan>();


            AutoMapper.Mapper.CreateMap<Domain.Crm.Entities.vwCRM_Sale_ProductInvoicePlan, ProductInvoiceViewModel>();
            AutoMapper.Mapper.CreateMap<ProductInvoiceViewModel, Domain.Crm.Entities.vwCRM_Sale_ProductInvoicePlan>();

            AutoMapper.Mapper.CreateMap<Domain.Crm.Entities.CRM_BH_DOANHSO, CRM_BH_DOANHSOViewModel>();
            AutoMapper.Mapper.CreateMap<CRM_BH_DOANHSOViewModel, Domain.Crm.Entities.CRM_BH_DOANHSO>();
            AutoMapper.Mapper.CreateMap<Domain.Crm.Entities.vwCRM_BH_DOANHSO, CRM_BH_DOANHSOViewModel>();
            AutoMapper.Mapper.CreateMap<CRM_BH_DOANHSOViewModel, Domain.Crm.Entities.vwCRM_BH_DOANHSO>();
            AutoMapper.Mapper.CreateMap<Domain.Crm.Entities.CRM_BH_DOANHSO_CT, CRM_BH_DOANHSOViewModel>();
            AutoMapper.Mapper.CreateMap<CRM_BH_DOANHSOViewModel, Domain.Crm.Entities.CRM_BH_DOANHSO_CT>();
            AutoMapper.Mapper.CreateMap<Domain.Crm.Entities.vwCRM_BH_DOANHSO_CT, CRM_BH_DOANHSOViewModel>();
            AutoMapper.Mapper.CreateMap<CRM_BH_DOANHSOViewModel, Domain.Crm.Entities.vwCRM_BH_DOANHSO_CT>();


            AutoMapper.Mapper.CreateMap<Domain.Crm.Entities.CALLog, CALLogViewModel>();
            AutoMapper.Mapper.CreateMap<CALLogViewModel, Domain.Crm.Entities.CALLog>();

            


            AutoMapper.Mapper.CreateMap<Domain.Crm.Entities.ListNoti, ListNotiViewModel>();
            AutoMapper.Mapper.CreateMap<ListNotiViewModel, Domain.Crm.Entities.ListNoti>();


            AutoMapper.Mapper.CreateMap<Domain.Crm.Entities.Crm_Level, Crm_LevelViewModel>();
            AutoMapper.Mapper.CreateMap<Crm_LevelViewModel, Domain.Crm.Entities.Crm_Level>();

            AutoMapper.Mapper.CreateMap<Domain.Crm.Entities.Crm_Period, Crm_PeriodViewModel>();
            AutoMapper.Mapper.CreateMap<Crm_PeriodViewModel, Domain.Crm.Entities.Crm_Period>();
            AutoMapper.Mapper.CreateMap<Domain.Crm.Entities.Crm_Target, Crm_TargetViewModel>();
            AutoMapper.Mapper.CreateMap<Crm_TargetViewModel, Domain.Crm.Entities.Crm_Target>();
 
            AutoMapper.Mapper.CreateMap<Domain.Crm.Entities.CRM_KH_BANHANG_CTIET_DEMO, CRM_KH_BANHANG_CTIETViewModel>();
            AutoMapper.Mapper.CreateMap<CRM_KH_BANHANG_CTIETViewModel, Domain.Crm.Entities.CRM_KH_BANHANG_CTIET_DEMO>();
            AutoMapper.Mapper.CreateMap<Domain.Crm.Entities.CRM_KH_BANHANG_DEMO, CRM_KH_BANHANGViewModel>();
            AutoMapper.Mapper.CreateMap<CRM_KH_BANHANGViewModel, Domain.Crm.Entities.CRM_KH_BANHANG_DEMO>();
            //<append_content_mapper_here>
            AutoMapper.Mapper.CreateMap<Domain.Crm.Entities.LeadMeeting, LeadMeetingViewModel>();
            AutoMapper.Mapper.CreateMap<LeadMeetingViewModel, Domain.Crm.Entities.LeadMeeting>();
        }
    }
}
