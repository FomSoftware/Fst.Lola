using System;

namespace FomMonitoringCore.Queue.ProcessData
{
    public interface IProcessor<in T> : IDisposable where T: Dto.BaseModel
    {
        bool ProcessData(T data);
    }
}