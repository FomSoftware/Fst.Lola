using RabbitMQ.Client;

namespace FomMonitoringCoreQueue.Connection
{
    public interface IQueueConnection
    {
        IConnection Connection { get; set; }
        IModel Channel { get; set; }
    }
}