using Autofac;
using FomMonitoringCore.Framework.Common;
using FomMonitoringCore.Service;
using Mapster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FomMonitoringCheckOffline
{
    class Program
    {
        static void Main(string[] args)
        {
            Inizialization();
            var builder = new ContainerBuilder();

            FomMonitoringCore.Ioc.IocContainerBuilder.BuildCore(builder, false);
            var container = builder.Build();
            var mesService = container.Resolve<IMesService>();
            try
            {
                mesService.CheckOfflineMachines();
            }
            catch (Exception ex)
            {
                string errMessage = string.Format(ex.GetStringLog(), string.Join(", ", args));
                LogService.WriteLog(errMessage, LogService.TypeLevel.Error, ex);
            }

        }

        private static void Inizialization()
        {
            TypeAdapterConfig.GlobalSettings.Scan(Assembly.GetAssembly(typeof(FomMonitoringCore.Framework.Config.MapsterConfig)));
        }

    }
}
