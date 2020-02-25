using System;
using System.Configuration;
using RabbitMQ.Client;

namespace FomMonitoringCoreQueue.Connection
{
    public class QueueConnection : IDisposable, IQueueConnection
    {
        public IConnection Connection { get; set; }
        public IModel Channel { get; set; }

        public QueueConnection()
        {

            var rabbitHost = ConfigurationManager.AppSettings["RabbitMqHost"];
            var rabbitUsername = ConfigurationManager.AppSettings["RabbitMqUsername"];
            var rabbitPassword = ConfigurationManager.AppSettings["RabbitMqPassword"];

            var factory = new ConnectionFactory
            {
                HostName = rabbitHost,
                UserName = rabbitUsername,
                Password = rabbitPassword,
                AutomaticRecoveryEnabled = true
            };

            Connection = factory.CreateConnection();
            Channel = Connection.CreateModel();
            Channel.QueueDeclare("VariableList",
                true,
                false,
                false,
                null);
            Channel.QueueDeclare("Info",
                true,
                false,
                false,
                null);
            Channel.QueueDeclare("State",
                true,
                false,
                false,
                null);
        }

        public void Dispose()
        {
            Connection.Dispose();
            Channel.Dispose();
        }
    }
}
