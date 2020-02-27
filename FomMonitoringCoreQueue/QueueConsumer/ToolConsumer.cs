using System;
using FomMonitoringCoreQueue.Dto;
using FomMonitoringCoreQueue.Events;

namespace FomMonitoringCoreQueue.QueueConsumer
{
    public class ToolConsumer : IConsumer<Tool>
    {

        public event EventHandler<LoggerEventsQueue> Log;

        public void Init()
        {
        }
    }
}