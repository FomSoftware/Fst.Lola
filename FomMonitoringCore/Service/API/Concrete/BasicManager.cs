using FomMonitoringCore.Model;

namespace FomMonitoringCore.Service.API.Concrete
{
    public class BasicManager : IBasicManager
    {
        public bool ValidateCredentials(LoginModel login)
        {
            bool result = true;
            return result;
        }
    }
}