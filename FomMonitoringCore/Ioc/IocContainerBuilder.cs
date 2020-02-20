using Autofac;
using Autofac.Builder;
using FomMonitoringCore.DAL;
using FomMonitoringCore.DAL_SQLite;
using FomMonitoringCore.Framework.Common;
using FomMonitoringCore.Repository;
using FomMonitoringCore.Repository.SQL;
using FomMonitoringCore.Service;
using FomMonitoringCore.Service.API;
using FomMonitoringCore.Service.API.Concrete;
using FomMonitoringCore.Service.APIClient;
using FomMonitoringCore.Service.APIClient.Concrete;
using FomMonitoringCore.Service.DataMapping;
using FomMonitoringCore.Uow;
using System.Collections.Generic;

namespace FomMonitoringCore.Ioc
{
    public static class IocContainerBuilder
    {
        public static void BuildCore(ContainerBuilder builder, bool instancePerRequest = true)
        {

            var instancesFoRequest = new List<IRegistrationBuilder<object, object, object>>();


            instancesFoRequest.Add(builder.RegisterType<JsonAPIClientService>().As<IJsonAPIClientService>());
            instancesFoRequest.Add(builder.RegisterType<JsonDataService>().As<IJsonDataService>());
            instancesFoRequest.Add(builder.RegisterType<ContextService>().As<IContextService>());
            instancesFoRequest.Add(builder.RegisterType<SpindleService>().As<ISpindleService>());
            instancesFoRequest.Add(builder.RegisterType<JobService>().As<IJobService>());
            instancesFoRequest.Add(builder.RegisterType<ParameterMachineService>().As<IParameterMachineService>());
            instancesFoRequest.Add(builder.RegisterType<MachineService>().As<IMachineService>());
            instancesFoRequest.Add(builder.RegisterType<XmlDataService>().As<IXmlDataService>());
            instancesFoRequest.Add(builder.RegisterType<JwtManager>().As<IJwtManager>());
            instancesFoRequest.Add(builder.RegisterType<MessageMachineRepository>().As<IMessageMachineRepository>());
            instancesFoRequest.Add(builder.RegisterType<MachineModelRepository>().As<IMachineModelRepository>());
            instancesFoRequest.Add(builder.RegisterType<MachineTypeRepository>().As<IMachineTypeRepository>());
            instancesFoRequest.Add(builder.RegisterType<MachineRepository>().As<IMachineRepository>());
            instancesFoRequest.Add(builder.RegisterType<PanelRepository>().As<IPanelRepository>());
            instancesFoRequest.Add(builder.RegisterType<SpindleRepository>().As<ISpindleRepository>());
            instancesFoRequest.Add(builder.RegisterType<HistoryMessageRepository>().As<IHistoryMessageRepository>());
            instancesFoRequest.Add(builder.RegisterType<HistoryJobRepository>().As<IHistoryJobRepository>());
            instancesFoRequest.Add(builder.RegisterType<ParameterMachineRepository>().As<IParameterMachineRepository>());
            instancesFoRequest.Add(builder.RegisterType<ParameterMachineValueRepository>().As<IParameterMachineValueRepository>());
            instancesFoRequest.Add(builder.RegisterType<MessagesIndexRepository>().As<IMessagesIndexRepository>());

            instancesFoRequest.Add(builder.RegisterType<MessageLanguagesRepository>().As<IMessageLanguagesRepository>());
            instancesFoRequest.Add(builder.RegisterType<HistoryStateRepository>().As<IHistoryStateRepository>());
            instancesFoRequest.Add(builder.RegisterType<MessageTranslationRepository>().As<IMessageTranslationRepository>());
            instancesFoRequest.Add(builder.RegisterType<MachineGroupRepository>().As<IMachineGroupRepository>());
            instancesFoRequest.Add(builder.RegisterType<HistoryPieceRepository>().As<IHistoryPieceRepository>());

            instancesFoRequest.Add(builder.RegisterType<SqLiteToSqlServerService>().As<ISqLiteToSqlServerService>());
            instancesFoRequest.Add(builder.RegisterType<PieceService>().As<IPieceService>());
            instancesFoRequest.Add(builder.RegisterType<LanguageService>().As<ILanguageService>());
            instancesFoRequest.Add(builder.RegisterType<StateService>().As<IStateService>());
            instancesFoRequest.Add(builder.RegisterType<MessageService>().As<IMessageService>());
            instancesFoRequest.Add(builder.RegisterType<UnitOfWork>().As<IUnitOfWork>());

            instancesFoRequest.Add(builder.RegisterType<FST_FomMonitoringEntities>().As<IFomMonitoringEntities>());
            instancesFoRequest.Add(builder.RegisterType<JsonToSqLiteService>().As<IJsonToSqLiteService>());
            instancesFoRequest.Add(builder.RegisterType<FST_FomMonitoringSQLiteEntities>().As<IFomMonitoringSQLiteEntities>());
            instancesFoRequest.Add(builder.RegisterType<JsonVariantsToSqlServerService>().As<IJsonVariantsToSqlServerService>());
            instancesFoRequest.Add(builder.RegisterType<BarService>().As<IBarService>());
            instancesFoRequest.Add(builder.RegisterType<MesService>().As<IMesService>());
            instancesFoRequest.Add(builder.RegisterType<PlantManagerService>().As<IPlantManagerService>());
            instancesFoRequest.Add(builder.RegisterType<ToolService>().As<IToolService>());

            if (instancePerRequest)
            {
                foreach (var b in instancesFoRequest)
                {
                    b.InstancePerRequest();
                }
            }
            else
            {
                foreach (var b in instancesFoRequest)
                {
                    b.SingleInstance();
                }
            }

            builder.RegisterType<SessionWebAttribute>().PropertiesAutowired().InstancePerRequest();

        }
    }
}
