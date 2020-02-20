﻿using System;
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
            var body = Encoding.UTF8.GetBytes(message);
            var props = _queueConnection.Channel.CreateBasicProperties();
            props.ContentType = "text/plain";
            props.DeliveryMode = 2;

            _queueConnection.Channel.BasicPublish("MessageExchange",
                "Message",
                props,
                body);

            return true;
        }
    }
}
