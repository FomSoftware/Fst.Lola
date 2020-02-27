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
                Password = rabbitPassword
            };

            Connection = factory.CreateConnection();
            Channel = Connection.CreateModel();
            Channel.QueueDeclare("VariableList",
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);
            Channel.QueueDeclare("Info",
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);
            Channel.QueueDeclare("State",
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);
            Channel.QueueDeclare("Messages",
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);
            Channel.QueueDeclare("Tool",
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);
            Channel.QueueDeclare("HistoryJobPieceBar",
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
