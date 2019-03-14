using CommonCore.Service;
using FomMonitoringCore.Framework.Common;
using FomMonitoringCore.Framework.Config;
using FomMonitoringCore.Service.APIClient;
using FomMonitoringCore.Service.APIClient.Concrete;
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

            try
            {
                IJsonAPIClientService jsonAPIClientService = new JsonAPIClientService();
                string method = ApplicationSettingService.GetWebConfigKey("GetCustomers");
                string customersJson = jsonAPIClientService.GetJsonData(method);
                if (!string.IsNullOrEmpty(customersJson) && jsonAPIClientService.ElaborateUpdateUsersJsonData(customersJson))
                {
                    result--;
                }
                else if (string.IsNullOrEmpty(customersJson))
                {
                    throw new NullReferenceException("JSON vuoto!");
                }
                else
                {
                    throw new FormatException("Formato JSON errato!");
                }
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
