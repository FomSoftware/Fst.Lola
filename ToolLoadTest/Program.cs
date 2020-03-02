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


            var jsons = context.Set<JsonData>().OrderByDescending(i => i.Id).ToList()
                .Select(o => new { o.Id, o.Json }).Where(o => o.Json.Contains("tool")).ToList();

            jsons = jsons.OrderBy(n => n.Id).ToList();

            foreach (var data in jsons)
            {
                forwarder.Forward(data.Json);
            }

            Debugger.Break();
        }
    }
}
