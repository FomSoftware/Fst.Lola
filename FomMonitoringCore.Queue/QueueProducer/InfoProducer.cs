﻿using System.Text;
using FomMonitoringCore.Queue.Connection;
using FomMonitoringCore.Queue.Dto;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace FomMonitoringCore.Queue.QueueProducer
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
            var props = _queueConnection.ChannelInfo.CreateBasicProperties();
            props.Persistent = true;

            _queueConnection.ChannelInfo.BasicPublish("",
                "Info",
                props,
                body);

            return true;
        }
    }
}
