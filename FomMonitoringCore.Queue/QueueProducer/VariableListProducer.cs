﻿using System.Text;
using FomMonitoringCore.Queue.Connection;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace FomMonitoringCore.Queue.QueueProducer
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
            var body = Encoding.UTF8.GetBytes((string) message);
            var properties = _queueConnection.ChannelVariableList.CreateBasicProperties();
            properties.Persistent = true;
            _queueConnection.ChannelVariableList.BasicPublish("",
                "VariableList",
                properties,
                body);

            return true;
        }
    }
}