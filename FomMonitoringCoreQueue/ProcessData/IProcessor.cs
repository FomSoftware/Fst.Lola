namespace FomMonitoringCoreQueue.ProcessData
{
    public interface IProcessor<in T> where T: Dto.BaseModel
    {
        bool ProcessData(T data);
    }
}