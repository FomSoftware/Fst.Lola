using Autofac;
using FomMonitoringCore.Framework.Common;
using FomMonitoringCore.Service;
using Mapster;
using System;
using System.Reflection;

namespace FomMonitoringCheckMaintenance
{
    class Program
    {
        static void Main(string[] args)
        {
            Inizialization();
            var builder = new ContainerBuilder();

            FomMonitoringCore.Ioc.IocContainerBuilder.BuildCore(builder, false);
            var container = builder.Build();
            var messageService = container.Resolve<IMessageService>();
            try
            {
                messageService.CheckMaintenance();
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
