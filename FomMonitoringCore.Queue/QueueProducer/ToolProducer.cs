using System.Text;
using FomMonitoringCore.Queue.Connection;
using FomMonitoringCore.Queue.Dto;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace FomMonitoringCore.Queue.QueueProducer
{
    public class ToolProducer : IProducer<Tool>
    {
        private readonly IQueueConnection _queueConnection;
        public ToolProducer(IQueueConnection queueConnection)
        {
            _queueConnection = queueConnection;
        }
        public bool Send(Tool model)
        {
            var message = JsonConvert.SerializeObject(model);
            var body = Encoding.UTF8.GetBytes((string) message);
            var props = _queueConnection.ChannelTool.CreateBasicProperties();
            props.Persistent = true;

            _queueConnection.ChannelTool.BasicPublish("",
                "Tool",
                props,
                body);

            return true;
        }
    }
}
