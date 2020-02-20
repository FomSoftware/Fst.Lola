using System;
using RabbitMQ.Client;

namespace FomMonitoringCoreQueue.Connection
{
    public class QueueConnection : IDisposable, IQueueConnection
    {
        public IConnection Connection { get; set; }
        public IModel Channel { get; set; }

        public QueueConnection()
        {
            var factory = new ConnectionFactory() { HostName = "10.104.1.170", UserName = "lola", Password = "lola" };
            Connection = factory.CreateConnection();
            Channel = Connection.CreateModel();
            Channel.QueueDeclare(queue: "VariableList",
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);
        }

        public void Dispose()
        {
            Connection.Dispose();
            Channel.Dispose();
        }
    }
}
