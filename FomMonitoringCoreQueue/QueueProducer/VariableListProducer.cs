using System.Text;
using FomMonitoringCoreQueue.Connection;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace FomMonitoringCoreQueue.QueueProducer
{
    public class VariableListProducer : IVariableListProducer
    {
        private readonly IQueueConnection _queueConnection;

        public VariableListProducer(IQueueConnection queueConnection)
        {
            _queueConnection = queueConnection;
        }


        public bool Send(Dto.VariablesList variablesList)
        {



            string message = JsonConvert.SerializeObject(variablesList);
            var body = Encoding.UTF8.GetBytes(message);

            _queueConnection.Channel.BasicPublish(exchange: "",
                routingKey: "VariableList",
                basicProperties: null,
                body: body);

            return true;
        }
    }
}