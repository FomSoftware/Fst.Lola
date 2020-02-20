using Autofac;
using FomMonitoringCore.Framework.Common;
using FomMonitoringCore.Framework.Model;
using FomMonitoringCore.Service;
using FomMonitoringCore.Service.DataMapping;
using Mapster;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using FomMonitoringCore.DAL;
using FomMonitoringCore.Uow;

namespace FomMonitoringDatabaseMapping
{
    class Program
    {
        static int Main(string[] args)
        {
            int result = 0;
            Inizialization();

            var builder = new ContainerBuilder();

            FomMonitoringCore.Ioc.IocContainerBuilder.BuildCore(builder, false);
            var container = builder.Build();
            var sQLiteToSQLServerService = container.Resolve<ISqLiteToSqlServerService>();
            var jsonToSQLiteService = container.Resolve<IJsonToSqLiteService>();
            var jsonVariantsToSQLServerService = container.Resolve<IJsonVariantsToSqlServerService>();
            var context = container.Resolve<IFomMonitoringEntities>();
            var unitOfWork = container.Resolve<IUnitOfWork>();
            try
            {
                List<JsonDataModel> jsonDataModels = jsonToSQLiteService.GetAllJsonDataNotElaborated();
                result = jsonDataModels.Count;
                foreach (JsonDataModel jsonDataModel in jsonDataModels)
                {
                    var timer = new Stopwatch();
                    

                        timer.Start();
                        try
                        {
                            unitOfWork.StartTransaction(context);
                            if (!jsonDataModel.IsCumulative)
                            {
                                if (!jsonToSQLiteService.MappingJsonDetailsToSqLite(jsonDataModel))
                                    throw new Exception("JSON to SQLite Error:");

                                if (!sQLiteToSQLServerService.MappingSqLiteDetailsToSqlServer())
                                    throw new Exception("SQLite to SQLServer Error:");
                            }

                            if (jsonDataModel.IsCumulative)
                            {
                                if (!jsonToSQLiteService.MappingJsonHistoryToSqLite(jsonDataModel))
                                    throw new Exception("JSON to SQLite Error:");

                                if (!sQLiteToSQLServerService.MappingSqLiteHistoryToSqlServer())
                                    throw new Exception("SQLite to SQLServer Error:");
                            }

                            if (!jsonVariantsToSQLServerService.MappingJsonParametersToSqlServer(jsonDataModel))
                                throw new Exception("JSON variables reading Error:");


                            if (jsonToSQLiteService.SaveElaboration(jsonDataModel.Id, true))
                            {
                                context.SaveChanges();
                                unitOfWork.CommitTransaction();
                            }
                            else
                            {
                                unitOfWork.RollbackTransaction();
                                throw new Exception($"Save elaboration failed: jsonId {jsonDataModel.Id}");
                            }

                            result--;
                        }
                        catch (Exception ex)
                        {
                            unitOfWork.RollbackTransaction();
                            jsonToSQLiteService.SaveElaboration(jsonDataModel.Id, false);
                            string errMessage = string.Format(ex.GetStringLog(), string.Join(", ", args),
                                jsonDataModel.Id);
                            LogService.WriteLog(errMessage, LogService.TypeLevel.Error, ex);
                        }

                        timer.Stop();
                     
                        LogService.WriteLog($"Json {jsonDataModel.Id} elaborato in {timer.Elapsed.TotalSeconds} secondi", LogService.TypeLevel.Info);

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
