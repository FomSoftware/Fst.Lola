using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using FomMonitoringCore.Queue.ProcessData;
using FomMonitoringCore.SqlServer;
using Mapster;

namespace HistoricizingTest
{
    class Program
    {
        private static ILifetimeScope _scopeHistoryJob;

        static void Main(string[] args)
        {
            Inizialization();
            var builder = new ContainerBuilder();
            FomMonitoringCore.Ioc.IocContainerBuilder.BuildCore(builder, false);
            var container = builder.Build();
            _scopeHistoryJob = container.BeginLifetimeScope();
            var context = container.Resolve<IFomMonitoringEntities>();
            //var pp = new StateProcessor(_scopeHistoryJob);
            //pp.HistoricizingStates(context, 1523);
            var pp = new MessageProcessor(_scopeHistoryJob);
            pp.HistoricizingMessages(context, 1523);
        }

        private static void Inizialization()
        {
            TypeAdapterConfig.GlobalSettings.Scan(Assembly.GetAssembly(typeof(FomMonitoringCore.Framework.Config.MapsterConfig)));
        }
    }
}
