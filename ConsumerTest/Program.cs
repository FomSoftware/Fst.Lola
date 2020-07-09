using System.Reflection;
using Autofac;
using FomMonitoringCore.Queue.Dto;
using FomMonitoringCore.Queue.QueueConsumer;
using Mapster;

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
            Inizialization();
            var builder = new ContainerBuilder();

            FomMonitoringCore.Ioc.IocContainerBuilder.BuildCore(builder, false);
            FomMonitoringCore.Queue.Ioc.IocContainerBuilder.BuildCore(builder, false);

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

        private static void Inizialization()
        {
            TypeAdapterConfig.GlobalSettings.Scan(Assembly.GetAssembly(typeof(FomMonitoringCore.Framework.Config.MapsterConfig)));
        }
    }
}
