using FomMonitoringCore.DAL;
using FomMonitoringCoreQueue.Forwarder;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;

namespace ToolLoadTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var builder = new ContainerBuilder();

            FomMonitoringCore.Ioc.IocContainerBuilder.BuildCore(builder, false);
            FomMonitoringCoreQueue.Ioc.IocContainerBuilder.BuildCore(builder, false);

            var container = builder.Build();

            var context = container.Resolve<IFomMonitoringEntities>();
            var forwarder = container.Resolve<IQueueForwarder>();


            Debugger.Break();
        }
    }
}
