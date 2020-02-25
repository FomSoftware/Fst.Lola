using System.ServiceProcess;
using Autofac;
using Autofac.Core.Lifetime;
using FomMonitoringCoreQueue.Connection;
using FomMonitoringCoreQueue.Dto;
using FomMonitoringCoreQueue.ProcessData;
using FomMonitoringCoreQueue.QueueConsumer;
using FomMonitoringCoreQueue.QueueProducer;

namespace FomMonitoringQueueConsumerService
{
    public partial class LolaQueueConsumer : ServiceBase
    {
        private ILifetimeScope _scopeVariable;
        private ILifetimeScope _scopeInfo;
        private ILifetimeScope _scopeMessage;
        private ILifetimeScope _scopeHistoryJob;
        private ILifetimeScope _scopeSpindle;
        private ILifetimeScope _scopeTool;
        private ILifetimeScope _scopeState;

        public LolaQueueConsumer()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {

            var builder = new ContainerBuilder();
            
            FomMonitoringCoreQueue.Ioc.IocContainerBuilder.BuildCore(builder, false);

            var container = builder.Build();

            _scopeVariable = container.BeginLifetimeScope();
            _scopeInfo = container.BeginLifetimeScope();
            _scopeMessage = container.BeginLifetimeScope();
            _scopeHistoryJob = container.BeginLifetimeScope();
            _scopeSpindle = container.BeginLifetimeScope();
            _scopeTool = container.BeginLifetimeScope();
            _scopeState = container.BeginLifetimeScope();

            //var consumerVariable = _scopeVariable.Resolve<IConsumer<VariablesList>>();
            //consumerVariable.Init();

            //var consumerInfo = _scopeVariable.Resolve<IConsumer<Info>>();
            //consumerInfo.Init();

            var consumerState = _scopeVariable.Resolve<IConsumer<State>>();
            consumerState.Init();

            //var consumerHistoryJob = _scopeVariable.Resolve<IConsumer<HistoryJobPieceBar>>();
            //consumerHistoryJob.Init();

            //var consumerMessage = _scopeVariable.Resolve<IConsumer<Message>>();
            //consumerMessage.Init();

            //var consumerSpindle = _scopeVariable.Resolve<IConsumer<Spindle>>();
            //consumerSpindle.Init();

            //var consumerTool = _scopeVariable.Resolve<IConsumer<Tool>>();
            //consumerTool.Init();


        }

        protected override void OnStop()
        {
            _scopeVariable.Dispose();
            _scopeInfo.Dispose();
            _scopeMessage.Dispose();
            _scopeHistoryJob.Dispose();
            _scopeSpindle.Dispose();
            _scopeTool.Dispose();
            _scopeState.Dispose();
        }
    }
}
