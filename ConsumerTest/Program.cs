using FomMonitoringCoreQueue.ProcessData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using FomMonitoringCoreQueue.Connection;
using FomMonitoringCoreQueue.Dto;
using FomMonitoringCoreQueue.Forwarder;
using FomMonitoringCoreQueue.QueueConsumer;
using FomMonitoringCoreQueue.QueueProducer;

namespace ConsumerTest
{
    class Program
    {
        

        private static ILifetimeScope _scopeVariable;
        private static ILifetimeScope _scopeInfo;
        private static ILifetimeScope _scopeMessage;
        private static ILifetimeScope _scopeHistoryJob;
        private static ILifetimeScope _scopeTool;
        private static ILifetimeScope _scopeState;
        

        static void Main(string[] args)
        {
            var builder = new ContainerBuilder();

            FomMonitoringCore.Ioc.IocContainerBuilder.BuildCore(builder, false);
            FomMonitoringCoreQueue.Ioc.IocContainerBuilder.BuildCore(builder, false);

            var container = builder.Build();

            _scopeVariable = container.BeginLifetimeScope();
            _scopeInfo = container.BeginLifetimeScope();
            _scopeMessage = container.BeginLifetimeScope();
            _scopeHistoryJob = container.BeginLifetimeScope();
            _scopeTool = container.BeginLifetimeScope();
            _scopeState = container.BeginLifetimeScope();

            var consumerVariable = _scopeVariable.Resolve<IConsumer<VariablesList>>();
            consumerVariable.Init();

            var consumerInfo = _scopeInfo.Resolve<IConsumer<Info>>();
            consumerInfo.Init();

            var consumerState = _scopeState.Resolve<IConsumer<State>>();
            consumerState.Init();

            var consumerHistoryJob = _scopeHistoryJob.Resolve<IConsumer<HistoryJobPieceBar>>();
            consumerHistoryJob.Init();

            var consumerMessage = _scopeMessage.Resolve<IConsumer<Message>>();
            consumerMessage.Init();


            var consumerTool = _scopeTool.Resolve<IConsumer<Tool>>();
            consumerTool.Init();
        }
    }
}
