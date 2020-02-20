using FomMonitoringCoreQueue.Dto;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FomMonitoringCoreQueue.Connection;
using RabbitMQ.Client;

namespace FomMonitoringCoreQueue.QueueProducer
{
    public class PieceProducer : IProducer<Piece>
    {
        private readonly IQueueConnection _queueConnection;
        public PieceProducer(IQueueConnection queueConnection)
        {
            _queueConnection = queueConnection;
        }
        public bool Send(Piece model)
        {
            var message = JsonConvert.SerializeObject(model);
            var body = Encoding.UTF8.GetBytes(message);
            var props = _queueConnection.Channel.CreateBasicProperties();
            props.ContentType = "text/plain";
            props.DeliveryMode = 2;

            _queueConnection.Channel.BasicPublish("PieceExchange",
                "Piece",
                props,
                body);

            return true;
        }
    }
}
