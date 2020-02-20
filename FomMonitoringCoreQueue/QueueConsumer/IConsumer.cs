using FomMonitoringCoreQueue.Dto;

namespace FomMonitoringCoreQueue.QueueConsumer
{
    public interface IConsumer<T> where T : BaseModel
    {
        void Init();
    }
}