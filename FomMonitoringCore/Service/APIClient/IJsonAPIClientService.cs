using FomMonitoringCore.Framework.Common;

namespace FomMonitoringCore.Service.APIClient
{
    public interface IJsonAPIClientService
    {
        enLoginResult ValidateCredentialsViaRemoteApi(string username, string password);

        bool UpdateActiveCustomersAndMachines();
    }
}
