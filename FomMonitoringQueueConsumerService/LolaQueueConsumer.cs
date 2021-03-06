﻿using System.ServiceProcess;
using Autofac;
using Autofac.Core.Lifetime;
using FomMonitoringCoreQueue.Connection;
using FomMonitoringCoreQueue.Dto;
using FomMonitoringCoreQueue.Forwarder;
using FomMonitoringCoreQueue.ProcessData;
using FomMonitoringCoreQueue.QueueConsumer;
using FomMonitoringCoreQueue.QueueProducer;

namespace FomMonitoringQueueConsumerService
{
    public partial class LolaQueueConsumer : ServiceBase
    {

        public LolaQueueConsumer()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {

            var builder = new ContainerBuilder();

            FomMonitoringCore.Ioc.IocContainerBuilder.BuildCore(builder, false);
            builder.RegisterType<QueueConnection>().As<IQueueConnection>().SingleInstance();


            builder.RegisterType<VariableListProducer>().As<IProducer<VariablesList>>().SingleInstance();
            builder.RegisterType<InfoProducer>().As<IProducer<Info>>().SingleInstance();
            builder.RegisterType<HistoryJobPieceBarProducer>().As<IProducer<HistoryJobPieceBar>>().SingleInstance();
            builder.RegisterType<MessageProducer>().As<IProducer<Message>>().SingleInstance();
            builder.RegisterType<StateProducer>().As<IProducer<State>>().SingleInstance();
            builder.RegisterType<ToolProducer>().As<IProducer<Tool>>().SingleInstance();

            builder.RegisterType<VariableListConsumer>().As<IConsumer<VariablesList>>().SingleInstance();
            builder.RegisterType<InfoConsumer>().As<IConsumer<Info>>().SingleInstance();
            builder.RegisterType<HistoryJobPieceBarConsumer>().As<IConsumer<HistoryJobPieceBar>>().SingleInstance();
            builder.RegisterType<StateConsumer>().As<IConsumer<State>>().SingleInstance();
            builder.RegisterType<MessageConsumer>().As<IConsumer<Message>>().SingleInstance();
            builder.RegisterType<ToolConsumer>().As<IConsumer<Tool>>().SingleInstance();

            builder.RegisterType<VariableListProcessor>().As<IProcessor<VariablesList>>().SingleInstance();
            builder.RegisterType<InfoProcessor>().As<IProcessor<Info>>().SingleInstance();
            builder.RegisterType<HistoryJobPieceBarProcessor>().As<IProcessor<HistoryJobPieceBar>>().SingleInstance();
            builder.RegisterType<StateProcessor>().As<IProcessor<State>>().SingleInstance();
            builder.RegisterType<MessageProcessor>().As<IProcessor<Message>>().SingleInstance();
            builder.RegisterType<ToolProcessor>().As<IProcessor<Tool>>().SingleInstance();
            builder.RegisterType<QueueForwarder>().As<IQueueForwarder>().SingleInstance();
            var container = builder.Build();
            

            var consumerVariable = container.Resolve<IConsumer<VariablesList>>();
            consumerVariable.Init();

            var consumerInfo = container.Resolve<IConsumer<Info>>();
            consumerInfo.Init();

            var consumerState = container.Resolve<IConsumer<State>>();
            consumerState.Init();

            var consumerHistoryJob = container.Resolve<IConsumer<HistoryJobPieceBar>>();
            consumerHistoryJob.Init();

            var consumerMessage = container.Resolve<IConsumer<Message>>();
            consumerMessage.Init();


            var consumerTool = container.Resolve<IConsumer<Tool>>();
            consumerTool.Init();


        }

        protected override void OnStop()
        {
        }
    }
}
