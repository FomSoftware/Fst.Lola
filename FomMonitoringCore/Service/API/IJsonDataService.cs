namespace FomMonitoringCore.Service.API
{
    public interface IJsonDataService
    {
        bool ElaborateJsonData(string json);
        bool ResetMachineData(string json);
    }
}
