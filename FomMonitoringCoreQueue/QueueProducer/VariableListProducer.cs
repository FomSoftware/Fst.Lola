﻿using System.Text;
using FomMonitoringCoreQueue.Connection;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace FomMonitoringCoreQueue.QueueProducer
{
    public class VariableListProducer : IProducer<Dto.VariablesList>
    {
        private readonly IQueueConnection _queueConnection;

        public VariableListProducer(IQueueConnection queueConnection)
        {
            _queueConnection = queueConnection;
        }


        public bool Send(Dto.VariablesList variablesList)
        {
            var message = JsonConvert.SerializeObject(variablesList);
            var body = Encoding.UTF8.GetBytes(message);
            var props = _queueConnection.Channel.CreateBasicProperties();
            props.ContentType = "text/plain";
            props.DeliveryMode = 2;

            _queueConnection.Channel.BasicPublish("VariableListExchange",
                "VariableList",
                props,
                body);

            return true;
        }
    }
}