using System;
using RabbitMQ.Client;

namespace FomMonitoringCore.Queue.Connection
{
    public interface IQueueConnection : IDisposable
    {
        IConnection Connection { get; set; }
        IModel ChannelVariableList { get; set; }
        IModel ChannelInfo { get; set; }
        IModel ChannelState { get; set; }
        IModel ChannelMessages { get; set; }
        IModel ChannelTool { get; set; }
        IModel ChannelHistoryJobPieceBar { get; set; }
        void Reconnect();

    }
}