using FomMonitoringCore.Framework.Common;

namespace FomMonitoringCore.Service.APIClient
{
    public interface IJsonAPIClientService
    {
        string GetJsonData(string method);
        bool ElaborateUpdateUsersJsonData(string json);
        enLoginResult ElaborateLoginJsonData(string json);
    }
}
