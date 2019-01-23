using CommonCore.Service;
using FomMonitoringCore.Framework.Common;
using FomMonitoringCore.Framework.Model;
using FomMonitoringCore.Service.DataMapping;
using Mapster;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace FomMonitoringDatabaseMapping
{
    class Program
    {
        static int Main(string[] args)
        {
            int result = 0;
            Inizialization();

            try
            {
                List<JsonDataModel> jsonDataModels = JsonToSQLiteService.GetAllJsonDataNotElaborated();
                result = jsonDataModels.Count;
                foreach (JsonDataModel jsonDataModel in jsonDataModels)
                {
                    try
                    {
                        if (!jsonDataModel.IsCumulative)
                        {
                            if (!JsonToSQLiteService.MappingJsonDetailsToSQLite(jsonDataModel))
                            {
                                throw new Exception("JSON to SQLite Error:");
                            }
                            if (!SQLiteToSQLServerService.MappingSQLiteDetailsToSQLServer())
                            {
                                throw new Exception("SQLite to SQLServer Error:");
                            }
                        }
                        if (jsonDataModel.IsCumulative)
                        {
                            if (!JsonToSQLiteService.MappingJsonHistoryToSQLite(jsonDataModel))
                            {
                                throw new Exception("JSON to SQLite Error:");
                            }
                            if (!SQLiteToSQLServerService.MappingSQLiteHistoryToSQLServer())
                            {
                                throw new Exception("SQLite to SQLServer Error:");
                            }
                        }
                        JsonToSQLiteService.SaveElaboration(jsonDataModel.Id, true);
                        result--;
                    }
                    catch (Exception ex)
                    {
                        JsonToSQLiteService.SaveElaboration(jsonDataModel.Id, false);
                        string errMessage = string.Format(ex.GetStringLog(), string.Join(", ", args), jsonDataModel.Id);
                        LogService.WriteLog(errMessage, LogService.TypeLevel.Error, ex);
                    }
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
            TypeAdapterConfig.GlobalSettings.Scan(Assembly.GetAssembly(typeof(FomMonitoringCore.Framework.Config.MapsterConfig)));
        }
    }
}
