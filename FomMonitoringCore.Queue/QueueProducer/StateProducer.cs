using System.Text;
using FomMonitoringCore.Queue.Connection;
using FomMonitoringCore.Queue.Dto;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace FomMonitoringCore.Queue.QueueProducer
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
            var body = Encoding.UTF8.GetBytes((string) message);
            var props = _queueConnection.ChannelHistoryJobPieceBar.CreateBasicProperties();
            props.Persistent = true;
            _queueConnection.ChannelState.BasicPublish("",
                "State",
                props, body);

            return true;
        }
    }
}
