using FomMonitoringCore.Model;

namespace FomMonitoringCore.Service.API
{
    public interface IBasicManager
    {
        bool ValidateCredentials(LoginModel login);
    }
}