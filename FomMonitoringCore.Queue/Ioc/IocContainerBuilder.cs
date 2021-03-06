﻿using Autofac;
using FomMonitoringCore.Queue.Connection;
using FomMonitoringCore.Queue.Dto;
using FomMonitoringCore.Queue.Forwarder;
using FomMonitoringCore.Queue.ProcessData;
using FomMonitoringCore.Queue.QueueConsumer;
using FomMonitoringCore.Queue.QueueProducer;
using State = FomMonitoringCore.Queue.Dto.State;

namespace FomMonitoringCore.Queue.Ioc
{
    public static class IocContainerBuilder
    {
        public static void BuildCore(ContainerBuilder builder, bool instancePerRequest = true, bool instancePerLifetimeScope = false)
        {

            FomMonitoringCore.Ioc.IocContainerBuilder.BuildCore(builder, instancePerRequest, instancePerLifetimeScope);
            builder.RegisterType<QueueConnection>().As<IQueueConnection>().SingleInstance();


            builder.RegisterType<ReForwarder>().As<IReForwarder>().InstancePerLifetimeScope();
            builder.RegisterType<VariableListProducer>().As<IProducer<VariablesList>>().InstancePerLifetimeScope();
            builder.RegisterType<InfoProducer>().As<IProducer<Info>>().InstancePerLifetimeScope();
            builder.RegisterType<HistoryJobPieceBarProducer>().As<IProducer<HistoryJobPieceBar>>().InstancePerLifetimeScope();
            builder.RegisterType<MessageProducer>().As<IProducer<Message>>().InstancePerLifetimeScope();
            builder.RegisterType<StateProducer>().As<IProducer<State>>().InstancePerLifetimeScope();
            builder.RegisterType<ToolProducer>().As<IProducer<Tool>>().InstancePerLifetimeScope();

            builder.RegisterType<VariableListConsumer>().As<IConsumer<VariablesList>>().InstancePerLifetimeScope();
            builder.RegisterType<InfoConsumer>().As<IConsumer<Info>>().InstancePerLifetimeScope();
            builder.RegisterType<HistoryJobPieceBarConsumer>().As<IConsumer<HistoryJobPieceBar>>().InstancePerLifetimeScope();
            builder.RegisterType<StateConsumer>().As<IConsumer<State>>().InstancePerLifetimeScope();
            builder.RegisterType<MessageConsumer>().As<IConsumer<Message>>().InstancePerLifetimeScope();
            builder.RegisterType<ToolConsumer>().As<IConsumer<Tool>>().InstancePerLifetimeScope();

            builder.RegisterType<VariableListProcessor>().As<IProcessor<VariablesList>>().InstancePerLifetimeScope();
            builder.RegisterType<InfoProcessor>().As<IProcessor<Info>>().InstancePerLifetimeScope();
            builder.RegisterType<HistoryJobPieceBarProcessor>().As<IProcessor<HistoryJobPieceBar>>().InstancePerLifetimeScope();
            builder.RegisterType<StateProcessor>().As<IProcessor<State>>().InstancePerLifetimeScope();
            builder.RegisterType<MessageProcessor>().As<IProcessor<Message>>().InstancePerLifetimeScope();
            builder.RegisterType<ToolProcessor>().As<IProcessor<Tool>>().InstancePerLifetimeScope();
            builder.RegisterType<QueueForwarder>().As<IQueueForwarder>().InstancePerLifetimeScope();

        }
    }
}
