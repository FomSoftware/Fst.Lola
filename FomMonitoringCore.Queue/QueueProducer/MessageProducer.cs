using System.Text;
using FomMonitoringCore.Queue.Connection;
using FomMonitoringCore.Queue.Dto;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace FomMonitoringCore.Queue.QueueProducer
{
    public class MessageProducer : IProducer<Message>
    {
        private readonly IQueueConnection _queueConnection;
        public MessageProducer(IQueueConnection queueConnection)
        {
            _queueConnection = queueConnection;
        }
        public bool Send(Message model)
        {
            var message = JsonConvert.SerializeObject(model);
            var body = Encoding.UTF8.GetBytes((string) message);
            var props = _queueConnection.ChannelMessages.CreateBasicProperties();
            props.Persistent = true;

            _queueConnection.ChannelMessages.BasicPublish("",
                "Messages",
                props,
                body);

            return true;
        }
    }
}
