using System.Collections.Generic;
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
using FomMonitoringCoreQueue.Connection;
using FomMonitoringCoreQueue.Dto;
using FomMonitoringCoreQueue.ProcessData;
using FomMonitoringCoreQueue.QueueConsumer;
using FomMonitoringCoreQueue.QueueProducer;
using Spindle = FomMonitoringCoreQueue.Dto.Spindle;
using State = FomMonitoringCoreQueue.Dto.State;

namespace FomMonitoringCoreQueue.Ioc
{
    public static class IocContainerBuilder
    {
        public static void BuildCore(ContainerBuilder builder, bool instancePerRequest = true)
        {

            FomMonitoringCore.Ioc.IocContainerBuilder.BuildCore(builder, false, true);
            builder.RegisterType<QueueConnection>().As<IQueueConnection>().SingleInstance();


            builder.RegisterType<VariableListProducer>().As<IProducer<VariablesList>>().SingleInstance();
            builder.RegisterType<InfoProducer>().As<IProducer<Info>>().SingleInstance();
            builder.RegisterType<HistoryJobPieceBarProducer>().As<IProducer<HistoryJobPieceBar>>().SingleInstance();
            builder.RegisterType<MessageProducer>().As<IProducer<Message>>().SingleInstance();
            builder.RegisterType<StateProducer>().As<IProducer<State>>().SingleInstance();
            builder.RegisterType<ToolProducer>().As<IProducer<Tool>>().SingleInstance();
            builder.RegisterType<SpindleProducer>().As<IProducer<Spindle>>().SingleInstance();

            builder.RegisterType<VariableListConsumer>().As<IConsumer<VariablesList>>().SingleInstance();
            builder.RegisterType<InfoConsumer>().As<IConsumer<Info>>().SingleInstance();
            builder.RegisterType<HistoryJobPieceBarConsumer>().As<IConsumer<HistoryJobPieceBar>>().SingleInstance();
            builder.RegisterType<StateConsumer>().As<IConsumer<State>>().SingleInstance();
            builder.RegisterType<MessageConsumer>().As<IConsumer<Message>>().SingleInstance();
            builder.RegisterType<SpindleConsumer>().As<IConsumer<Spindle>>().SingleInstance();
            builder.RegisterType<ToolConsumer>().As<IConsumer<Tool>>().SingleInstance();

            builder.RegisterType<VariableListProcessor>().As<IProcessor<VariablesList>>().SingleInstance();
            builder.RegisterType<InfoProcessor>().As<IProcessor<Info>>().SingleInstance();
            builder.RegisterType<HistoryJobPieceBarProcessor>().As<IProcessor<HistoryJobPieceBar>>().SingleInstance();
            builder.RegisterType<StateProcessor>().As<IProcessor<State>>().SingleInstance();
            builder.RegisterType<MessageProcessor>().As<IProcessor<Message>>().SingleInstance();
            builder.RegisterType<SpindleProcessor>().As<IProcessor<Spindle>>().SingleInstance();
            builder.RegisterType<ToolProcessor>().As<IProcessor<Tool>>().SingleInstance();

        }
    }
}
