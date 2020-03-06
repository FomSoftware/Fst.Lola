using Autofac;
using Autofac.Integration.Mvc;
using Autofac.Integration.WebApi;
using FomMonitoringBLL.ViewServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;

namespace FomMonitoring
{
    public static class IocConfig
    {

        public static void ConfigureDependencyInjection()
        {
            var builder = new ContainerBuilder();


            builder.RegisterApiControllers(typeof(MvcApplication).Assembly);

            builder.RegisterControllers(typeof(MvcApplication).Assembly);
            // Add your registrations
            FomMonitoringCore.Ioc.IocContainerBuilder.BuildCore(builder);

            builder.RegisterFilterProvider();
            builder.RegisterType<JobsViewService>().As<IJobsViewService>().InstancePerRequest();
            builder.RegisterType<EfficiencyViewService>().As<IEfficiencyViewService>().InstancePerRequest();
            builder.RegisterType<MachineViewService>().As<IMachineViewService>().InstancePerRequest();
            builder.RegisterType<MaintenanceViewService>().As<IMaintenanceViewService>().InstancePerRequest();
            builder.RegisterType<MessagesViewService>().As<IMessagesViewService>().InstancePerRequest();
            builder.RegisterType<PlantMessagesViewService>().As<IPlantMessagesViewService>().InstancePerRequest();
            builder.RegisterType<ProductivityViewService>().As<IProductivityViewService>().InstancePerRequest();
            builder.RegisterType<MachineViewService>().As<IMachineViewService>().InstancePerRequest();
            builder.RegisterType<ToolsViewService>().As<IToolsViewService>().InstancePerRequest();
            builder.RegisterType<PanelParametersViewService>().As<IPanelParametersViewService>().InstancePerRequest();
            builder.RegisterType<XToolsViewService>().As<IXToolsViewService>().InstancePerRequest();
            builder.RegisterType<PlantManagerViewService> ().As<IPlantManagerViewService> ().InstancePerRequest();
            builder.RegisterType<MesViewService>().As<IMesViewService>().InstancePerRequest();
            builder.RegisterType<NotificationViewService>().As<INotificationViewService>().InstancePerRequest();

            var container = builder.Build();

            // Set the dependency resolver for Web API.
            var webApiResolver = new AutofacWebApiDependencyResolver(container);
            GlobalConfiguration.Configuration.DependencyResolver = webApiResolver;

            // Set the dependency resolver for MVC.
            var mvcResolver = new AutofacDependencyResolver(container);
            DependencyResolver.SetResolver(mvcResolver);
        }

        
    }
}