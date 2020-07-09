using System.Diagnostics;
using System.Reflection;
using Autofac;
using FomMonitoringCore.Queue.Forwarder;
using FomMonitoringCore.SqlServer;
using Mapster;

namespace StateLoadTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Inizialization();
            var builder = new ContainerBuilder();

            FomMonitoringCore.Ioc.IocContainerBuilder.BuildCore(builder, false);
            FomMonitoringCore.Queue.Ioc.IocContainerBuilder.BuildCore(builder, false);

            var container = builder.Build();

            var context = container.Resolve<IFomMonitoringEntities>();
            var forwarder = container.Resolve<IQueueForwarder>();

            Debugger.Break();

        }
        private static void Inizialization()
        {
            TypeAdapterConfig.GlobalSettings.Scan(Assembly.GetAssembly(typeof(FomMonitoringCore.Framework.Config.MapsterConfig)));
        }
    }
}
