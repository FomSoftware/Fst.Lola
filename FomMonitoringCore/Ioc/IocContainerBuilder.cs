using Autofac;
using Autofac.Builder;
using FomMonitoringCore.SqlServer;
using FomMonitoringCore.Framework.Common;
using FomMonitoringCore.Service;
using FomMonitoringCore.Service.API;
using FomMonitoringCore.Service.API.Concrete;
using FomMonitoringCore.Service.APIClient;
using FomMonitoringCore.Service.APIClient.Concrete;
using FomMonitoringCore.Uow;
using System.Collections.Generic;
using FomMonitoringCore.SqlServer.Repository;
using FomMonitoringCore.Mongo;
using UserManager.Gateway;
using UserManager.Service;
using UserManager.Service.Concrete;
using AuditLogin = UserManager.Gateway.AuditLogin;
using Users = UserManager.Gateway.Users;

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

                 builder.RegisterType<PieceService>().As<IPieceService>(),
                builder.RegisterType<LanguageService>().As<ILanguageService>(),
                builder.RegisterType<StateService>().As<IStateService>(),
                builder.RegisterType<MessageService>().As<IMessageService>(),
                builder.RegisterType<UnitOfWork>().As<IUnitOfWork>(),

                builder.RegisterType<TimeZoneService>().As<ITimeZoneService>(),
                builder.RegisterType<BarService>().As<IBarService>(),
                builder.RegisterType<MesService>().As<IMesService>(),
                builder.RegisterType<PlantManagerService>().As<IPlantManagerService>(),
                builder.RegisterType<ToolService>().As<IToolService>(),
                builder.RegisterType<MongoDbContext>().As<IMongoDbContext>(),
                builder.RegisterType<BasicManager>().As<IBasicManager>(),
                builder.RegisterType<JsonAPIClientService>().As<IJsonAPIClientService>(),
                builder.RegisterType<UserManagerService>().As<IUserManagerService>(),
                builder.RegisterType<AccountService>().As<IAccountService>(),
                builder.RegisterType<LoginServices>().As<ILoginServices>(),
                builder.RegisterType<AuditLogin>().As<IAuditLogin>(),
                builder.RegisterType<UserServices>().As<IUserServices>(),
                builder.RegisterType<LoggedUserServices>().As<ILoggedUserServices>(),
                builder.RegisterType<Users>().As<IUsers>(),
                builder.RegisterGeneric(typeof(Mongo.Repository.GenericRepository<>)).As(typeof(Mongo.Repository.IGenericRepository<>))
            };

            var dbContext = builder.RegisterType<FomMonitoringEntities>().As<IFomMonitoringEntities>();

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
