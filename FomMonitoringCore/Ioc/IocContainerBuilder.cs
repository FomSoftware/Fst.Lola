using Autofac;
using Autofac.Builder;
using FomMonitoringCore.DAL;
using FomMonitoringCore.DAL_SQLite;
using FomMonitoringCore.Framework.Common;
using FomMonitoringCore.Repository.SQL;
using FomMonitoringCore.Service;
using FomMonitoringCore.Service.API;
using FomMonitoringCore.Service.API.Concrete;
using FomMonitoringCore.Service.APIClient;
using FomMonitoringCore.Service.APIClient.Concrete;
using FomMonitoringCore.Service.DataMapping;
using FomMonitoringCore.Uow;
using System.Collections.Generic;
using FomMonitoringCore.DalMongoDb;

namespace FomMonitoringCore.Ioc
{
    public static class IocContainerBuilder
    {
        public static void BuildCore(ContainerBuilder builder, bool instancePerRequest = true, bool instancePerLifetimeScope = false)
        {

            var instancesFoRequest = new List<IRegistrationBuilder<object, object, object>>
            {
                builder.RegisterType<JsonAPIClientService>().As<IJsonAPIClientService>(),
                builder.RegisterType<JsonDataService>().As<IJsonDataService>(),
                builder.RegisterType<ContextService>().As<IContextService>(),
                builder.RegisterType<SpindleService>().As<ISpindleService>(),
                builder.RegisterType<JobService>().As<IJobService>(),
                builder.RegisterType<ParameterMachineService>().As<IParameterMachineService>(),
                builder.RegisterType<MachineService>().As<IMachineService>(),
                builder.RegisterType<XmlDataService>().As<IXmlDataService>(),
                builder.RegisterType<JwtManager>().As<IJwtManager>(),
                builder.RegisterType<MessageMachineRepository>().As<IMessageMachineRepository>(),
                builder.RegisterType<MachineModelRepository>().As<IMachineModelRepository>(),
                builder.RegisterType<MachineTypeRepository>().As<IMachineTypeRepository>(),
                builder.RegisterType<MachineRepository>().As<IMachineRepository>(),
                builder.RegisterType<PanelRepository>().As<IPanelRepository>(),
                builder.RegisterType<SpindleRepository>().As<ISpindleRepository>(),
                builder.RegisterType<HistoryMessageRepository>().As<IHistoryMessageRepository>(),
                builder.RegisterType<HistoryJobRepository>().As<IHistoryJobRepository>(),
                builder.RegisterType<ParameterMachineRepository>().As<IParameterMachineRepository>(),
                builder.RegisterType<ParameterMachineValueRepository>().As<IParameterMachineValueRepository>(),
                builder.RegisterType<MessagesIndexRepository>().As<IMessagesIndexRepository>(),

                builder.RegisterType<MessageLanguagesRepository>().As<IMessageLanguagesRepository>(),
                builder.RegisterType<HistoryStateRepository>().As<IHistoryStateRepository>(),
                builder.RegisterType<MessageTranslationRepository>().As<IMessageTranslationRepository>(),
                builder.RegisterType<MachineGroupRepository>().As<IMachineGroupRepository>(),
                builder.RegisterType<HistoryPieceRepository>().As<IHistoryPieceRepository>(),

                builder.RegisterType<SqLiteToSqlServerService>().As<ISqLiteToSqlServerService>(),
                builder.RegisterType<PieceService>().As<IPieceService>(),
                builder.RegisterType<LanguageService>().As<ILanguageService>(),
                builder.RegisterType<StateService>().As<IStateService>(),
                builder.RegisterType<MessageService>().As<IMessageService>(),
                builder.RegisterType<UnitOfWork>().As<IUnitOfWork>(),
                
                builder.RegisterType<JsonToSqLiteService>().As<IJsonToSqLiteService>(),
                builder.RegisterType<FST_FomMonitoringSQLiteEntities>().As<IFomMonitoringSQLiteEntities>(),
                builder.RegisterType<JsonVariantsToSqlServerService>().As<IJsonVariantsToSqlServerService>(),
                builder.RegisterType<BarService>().As<IBarService>(),
                builder.RegisterType<MesService>().As<IMesService>(),
                builder.RegisterType<PlantManagerService>().As<IPlantManagerService>(),
                builder.RegisterType<ToolService>().As<IToolService>(),
                builder.RegisterType<MongoDbContext>().As<IMongoDbContext>(),
                builder.RegisterGeneric(typeof(Repository.MongoDb.GenericRepository<>)).As(typeof(Repository.MongoDb.IGenericRepository<>))
            };

            var dbContext = builder.RegisterType<FST_FomMonitoringEntities>().As<IFomMonitoringEntities>();

            if (instancePerRequest)
            {
                foreach (var b in instancesFoRequest)
                {
                    b.InstancePerRequest();
                }

                dbContext.InstancePerRequest();
            }
            else
            {
                foreach (var b in instancesFoRequest)
                {
                    b.SingleInstance();
                }

                if (instancePerLifetimeScope)
                {
                    foreach (var b in instancesFoRequest)
                    {
                        b.InstancePerLifetimeScope();
                    }
                    dbContext.InstancePerLifetimeScope();
                }
                else
                {
                    foreach (var b in instancesFoRequest)
                    {
                        b.SingleInstance();
                    }
                    dbContext.SingleInstance();
                }
            }

            builder.RegisterType<SessionWebAttribute>().PropertiesAutowired().InstancePerRequest();

        }
    }
}
