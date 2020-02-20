using FomMonitoringCore.Framework.Model;

namespace FomMonitoringCore.Service.API.Concrete
{
    public class BasicManager : IBasicManager
    {
        public bool ValidateCredentials(LoginModel login)
        {
            return AccountService.LoginApi(login.Username, login.Password);
        }
    }
}
