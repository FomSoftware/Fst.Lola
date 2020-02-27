using System;
using Autofac;
using FomMonitoringCoreQueue.Dto;
using FomMonitoringCoreQueue.Events;

namespace FomMonitoringCoreQueue.QueueConsumer
{
    public interface IConsumerInitializer : IDisposable
    {
        event EventHandler<LoggerEventsQueue> MessageLogged;
    }

    public class ConsumerInitializer : IConsumerInitializer
    {
        private readonly ILifetimeScope _scopeVariable;
        private readonly ILifetimeScope _scopeInfo;
        private readonly ILifetimeScope _scopeMessage;
        private readonly ILifetimeScope _scopeHistoryJob;
        private readonly ILifetimeScope _scopeTool;
        private readonly ILifetimeScope _scopeState;


        public ConsumerInitializer()
        {
            var builder = new ContainerBuilder();
            
            Ioc.IocContainerBuilder.BuildCore(builder, false);

            var container = builder.Build();

            _scopeVariable = container.BeginLifetimeScope();
            _scopeInfo = container.BeginLifetimeScope();
            _scopeMessage = container.BeginLifetimeScope();
            _scopeHistoryJob = container.BeginLifetimeScope();
            _scopeTool = container.BeginLifetimeScope();
            _scopeState = container.BeginLifetimeScope();

            var consumerVariable = _scopeVariable.Resolve<IConsumer<VariablesList>>();
            consumerVariable.Log += WriteLog;
            consumerVariable.Init();

            var consumerInfo = _scopeInfo.Resolve<IConsumer<Info>>();
            consumerInfo.Log += WriteLog;
            consumerInfo.Init();

            var consumerState = _scopeState.Resolve<IConsumer<State>>();
            consumerState.Log += WriteLog;
            consumerState.Init();

            var consumerHistoryJob = _scopeHistoryJob.Resolve<IConsumer<HistoryJobPieceBar>>();
            consumerHistoryJob.Log += WriteLog;
            consumerHistoryJob.Init();

            var consumerMessage = _scopeMessage.Resolve<IConsumer<Message>>();
            consumerMessage.Log += WriteLog;
            consumerMessage.Init();


            var consumerTool = _scopeTool.Resolve<IConsumer<Tool>>();
            consumerTool.Log += WriteLog;
            consumerTool.Init();
        }

        public event EventHandler<LoggerEventsQueue> MessageLogged;

        protected void WriteLog(object sender, LoggerEventsQueue eventLog)
        {
            OnMessageLogged(eventLog); // raise event  
        }

        // .Net recommends - Event method should be protected, virtual, void and start with On<Eventname>  
        protected virtual void OnMessageLogged(LoggerEventsQueue eventLog)
        {
            MessageLogged?.Invoke(this, eventLog);
        }


        public void Dispose()
        {
            _scopeVariable?.Dispose();
            _scopeInfo?.Dispose();
            _scopeMessage?.Dispose();
            _scopeHistoryJob?.Dispose();
            _scopeTool?.Dispose();
            _scopeState?.Dispose();
        }
    }
}


