using System.Text;
using FomMonitoringCoreQueue.Connection;
using FomMonitoringCoreQueue.Dto;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace FomMonitoringCoreQueue.QueueProducer
{
    public class InfoProducer : IProducer<Info>
    {
        private readonly IQueueConnection _queueConnection;
        public InfoProducer(IQueueConnection queueConnection)
        {
            _queueConnection = queueConnection;
        }

        public bool Send(Info model)
        {
            var message = JsonConvert.SerializeObject(model);
            var body = Encoding.UTF8.GetBytes(message);
            var props = _queueConnection.Channel.CreateBasicProperties();
            props.Persistent = true;

            _queueConnection.Channel.BasicPublish("",
                "Info",
                props,
                body);

            return true;
        }
    }
}
