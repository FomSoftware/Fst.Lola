using System.Text;
using FomMonitoringCoreQueue.Connection;
using FomMonitoringCoreQueue.Dto;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace FomMonitoringCoreQueue.QueueProducer
{
    public class HistoryJobPieceBarProducer : IProducer<HistoryJobPieceBar>
    {
        private readonly IQueueConnection _queueConnection;
        public HistoryJobPieceBarProducer(IQueueConnection queueConnection)
        {
            _queueConnection = queueConnection;
        }

        public bool Send(HistoryJobPieceBar model)
        {
            var message = JsonConvert.SerializeObject(model);
            var body = Encoding.UTF8.GetBytes(message);
            var props = _queueConnection.ChannelHistoryJobPieceBar.CreateBasicProperties();
            props.Persistent = true;

            _queueConnection.ChannelHistoryJobPieceBar.BasicPublish("",
                "HistoryJobPieceBar",
                props,
                body);

            return true;
        }

    }
}
