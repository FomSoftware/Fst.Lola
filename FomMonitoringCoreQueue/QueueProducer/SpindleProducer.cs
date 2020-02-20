using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FomMonitoringCoreQueue.Connection;
using FomMonitoringCoreQueue.Dto;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace FomMonitoringCoreQueue.QueueProducer
{
    public class SpindleProducer : IProducer<Spindle>
    {
        private readonly IQueueConnection _queueConnection;

        public SpindleProducer(IQueueConnection queueConnection)
        {
            _queueConnection = queueConnection;
        }

        public bool Send(Spindle model)
        {
            var message = JsonConvert.SerializeObject(model);
            var body = Encoding.UTF8.GetBytes(message);
            var props = _queueConnection.Channel.CreateBasicProperties();
            props.ContentType = "text/plain";
            props.DeliveryMode = 2;

            _queueConnection.Channel.BasicPublish("SpindleExchange",
                "Spindle",
                props,
                body);

            return true;
        }
    }
}
