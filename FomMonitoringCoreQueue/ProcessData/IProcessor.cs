using System;

namespace FomMonitoringCoreQueue.ProcessData
{
    public interface IProcessor<in T> : IDisposable where T: Dto.BaseModel
    {
        bool ProcessData(T data);
    }
}