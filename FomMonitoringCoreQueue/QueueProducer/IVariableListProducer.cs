namespace FomMonitoringCoreQueue.QueueProducer
{
    public interface IVariableListProducer
    {
        bool Send(Dto.VariablesList variablesList);
    }
}
