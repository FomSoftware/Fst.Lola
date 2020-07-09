

namespace FomMonitoringCore.Queue.QueueProducer
{
    public interface IProducer<in T> where T : Dto.BaseModel
    {
        bool Send(T model);
    }

}
