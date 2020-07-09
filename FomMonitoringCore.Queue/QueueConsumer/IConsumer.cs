using System;
using FomMonitoringCore.Queue.Dto;
using FomMonitoringCore.Queue.Events;

namespace FomMonitoringCore.Queue.QueueConsumer
{
    public interface IConsumer<T> : IDisposable where T : BaseModel
    {
        event EventHandler<LoggerEventsQueue> Log;

        void Init();
    }
}