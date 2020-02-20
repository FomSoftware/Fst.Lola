using Autofac;
using Autofac.Integration.WebApi;
using FomMonitoringCore.Ioc;
using System.Web.Http;

namespace FomMonitoringApi
{
    public static class IocConfig
    {
        public static void ConfigureDependencyInjection()
        {
            var builder = new ContainerBuilder();
            var config = GlobalConfiguration.Configuration;
            builder.RegisterApiControllers(typeof(WebApiApplication).Assembly);
            IocContainerBuilder.BuildCore(builder);

            builder.RegisterWebApiFilterProvider(config);
            builder.RegisterWebApiModelBinderProvider();
            IContainer container = builder.Build();
            config.DependencyResolver = new AutofacWebApiDependencyResolver(container);
        }
        
    }
}