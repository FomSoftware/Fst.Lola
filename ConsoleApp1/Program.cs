using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using FomMonitoringCore.DAL;
using FomMonitoringCoreQueue.Forwarder;
using Mapster;

namespace ToolsLoadTest
{
    class Program
    {
        private static void Inizialization()
        {
            TypeAdapterConfig.GlobalSettings.Scan(Assembly.GetAssembly(typeof(FomMonitoringCore.Framework.Config.MapsterConfig)));
        }

        static void Main(string[] args)
        {
            Inizialization();

            var builder = new ContainerBuilder();

            FomMonitoringCore.Ioc.IocContainerBuilder.BuildCore(builder, false);
            FomMonitoringCoreQueue.Ioc.IocContainerBuilder.BuildCore(builder, false);

            var container = builder.Build();

            var context = container.Resolve<IFomMonitoringEntities>();
            var forwarder = container.Resolve<IQueueForwarder>();

            var jsons = context.Set<JsonData>().Where(j => j.Json.Contains("tool")).OrderByDescending(i => i.Id).Take(10).ToList()
                .Select(o => o.Json).ToList();

            foreach (var data in jsons)
            {
                forwarder.Forward(data);
            }

            Debugger.Break();
        }

    }
}
