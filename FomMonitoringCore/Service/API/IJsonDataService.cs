namespace FomMonitoringCore.Service.API
{
    public interface IJsonDataService
    {
        bool AddJsonData(string json, bool isCumulative);
        bool ElaborateJsonData(string json);
        bool ResetMachineData(string json);
    }
}
