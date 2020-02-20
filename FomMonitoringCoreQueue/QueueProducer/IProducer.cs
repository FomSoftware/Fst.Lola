

namespace FomMonitoringCoreQueue.QueueProducer
{
    public interface IProducer<in T> where T : Dto.BaseModel
    {
        bool Send(T model);
    }

}
