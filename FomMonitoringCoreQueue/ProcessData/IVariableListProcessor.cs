namespace FomMonitoringCoreQueue.ProcessData
{
    public interface IVariableListProcessor
    {
        bool ProcessData(Dto.VariablesList data);
    }
}