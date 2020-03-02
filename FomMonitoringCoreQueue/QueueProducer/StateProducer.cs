using System.Text;
using FomMonitoringCoreQueue.Connection;
using FomMonitoringCoreQueue.Dto;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace FomMonitoringCoreQueue.QueueProducer
{
    public class StateProducer : IProducer<State>
    {
        private readonly IQueueConnection _queueConnection;
        public StateProducer(IQueueConnection queueConnection)
        {
            _queueConnection = queueConnection;
            
        }
        public bool Send(State model)
        {
            var message = JsonConvert.SerializeObject(model);
            var body = Encoding.UTF8.GetBytes(message);

            _queueConnection.ChannelState.BasicPublish("",
                "State",
                null, body);

            return true;
        }
    }
}
