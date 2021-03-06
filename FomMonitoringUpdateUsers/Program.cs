﻿using Autofac;
using FomMonitoringCore.Framework.Common;
using FomMonitoringCore.Framework.Config;
using FomMonitoringCore.Service;
using FomMonitoringCore.Service.APIClient;
using Mapster;
using System;
using System.Reflection;

namespace FomMonitoringUpdateUsers
{
    class Program
    {
        static int Main(string[] args)
        {
            int result = 1;
            Inizialization();

            var builder = new ContainerBuilder();

            FomMonitoringCore.Ioc.IocContainerBuilder.BuildCore(builder, false);
            var container = builder.Build();
            var jsonAPIClientService = container.Resolve<IJsonAPIClientService>();

            try
            {
                if (jsonAPIClientService.UpdateActiveCustomersAndMachines())
                    result--;
                else
                    throw new Exception("Errore durante l'aggiornamento dei clienti e delle macchine abilitate al servizio!");
            }
            catch (Exception ex)
            {
                string errMessage = string.Format(ex.GetStringLog(), string.Join(", ", args));
                LogService.WriteLog(errMessage, LogService.TypeLevel.Error, ex);
            }
            return result;
        }

        private static void Inizialization()
        {
            TypeAdapterConfig.GlobalSettings.Scan(Assembly.GetAssembly(typeof(MapsterConfig)));
        }

    }
}
