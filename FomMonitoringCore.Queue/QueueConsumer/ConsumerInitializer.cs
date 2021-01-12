using System;
using Autofac;
using FomMonitoringCore.Queue.Connection;
using FomMonitoringCore.Queue.Dto;
using FomMonitoringCore.Queue.Events;
using FomMonitoringCore.Queue.Forwarder;

namespace FomMonitoringCore.Queue.QueueConsumer
{
    public interface IConsumerInitializer : IDisposable
    {
        event EventHandler<LoggerEventsQueue> MessageLogged;
        void Reconnect();
    }

    public class ConsumerInitializer : IConsumerInitializer
    {
        private ILifetimeScope _scopeVariable;
        private ILifetimeScope _scopeInfo;
        private ILifetimeScope _scopeMessage;
        private ILifetimeScope _scopeHistoryJob;
        private ILifetimeScope _scopeTool;
        private ILifetimeScope _scopeState;
        private IContainer CurrentContainer { get; set; }

        private IQueueConnection Connection;
        private ILifetimeScope _scopeUnknown;

        public ConsumerInitializer()
        {
            Init();
        }

        private void Init()
        {
            var builder = new ContainerBuilder();

            Ioc.IocContainerBuilder.BuildCore(builder, false, true);

            CurrentContainer = builder.Build();
            Connection = CurrentContainer.Resolve<IQueueConnection>();
            _scopeVariable = CurrentContainer.BeginLifetimeScope();
            _scopeInfo = CurrentContainer.BeginLifetimeScope();
            _scopeMessage = CurrentContainer.BeginLifetimeScope();
            _scopeHistoryJob = CurrentContainer.BeginLifetimeScope();
            _scopeTool = CurrentContainer.BeginLifetimeScope();
            _scopeState = CurrentContainer.BeginLifetimeScope();
            _scopeUnknown = CurrentContainer.BeginLifetimeScope();

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


        public void Reconnect()
        {
            Dispose();
            Init();
        }

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
            _scopeUnknown?.Dispose();
            CurrentContainer?.Dispose();
            CurrentContainer = null;

        }

        public void ElaborateUnknown()
        {
            _scopeUnknown.Resolve<IReForwarder>().ReForward();
        }
    }
}


