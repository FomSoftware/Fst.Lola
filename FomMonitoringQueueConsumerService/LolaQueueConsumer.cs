using System.ServiceProcess;
using Autofac;
using FomMonitoringCoreQueue.Connection;
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
            builder.RegisterType<VariableListConsumer>().As<IVariableListConsumer>().SingleInstance();
            builder.RegisterType<VariableListProcessor>().As<IVariableListProcessor>().SingleInstance();
            builder.RegisterType<VariableListProducer>().As<IVariableListProducer>().SingleInstance();
            var container = builder.Build();
            var consumer = container.Resolve<IVariableListConsumer>();
            consumer.Init();
        }

        protected override void OnStop()
        {
        }
    }
}
