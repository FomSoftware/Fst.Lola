using System.Diagnostics;
using Autofac;
using FomMonitoringCore.SqlServer;
using FomMonitoringCore.Queue.Forwarder;

namespace ToolLoadTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var builder = new ContainerBuilder();

            FomMonitoringCore.Ioc.IocContainerBuilder.BuildCore(builder, false);
            FomMonitoringCore.Queue.Ioc.IocContainerBuilder.BuildCore(builder, false);

            var container = builder.Build();

            var context = container.Resolve<IFomMonitoringEntities>();
            var forwarder = container.Resolve<IQueueForwarder>();


            Debugger.Break();
        }
    }
}
