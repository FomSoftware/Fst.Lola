using System;
using FomMonitoringCore.Service;
using FomMonitoringCoreQueue.Dto;
using FomMonitoringCoreQueue.Events;

namespace FomMonitoringCoreQueue.QueueConsumer
{
    public interface IConsumer<T> : IDisposable where T : BaseModel
    {
        event EventHandler<LoggerEventsQueue> Log;

        void Init();
    }
}