using FomMonitoringCore.Framework.Model;

namespace FomMonitoringCore.Service.API.Concrete
{
    public class BasicManager : IBasicManager
    {
        private readonly IAccountService _accountService;

        public BasicManager(IAccountService accountService)
        {
            _accountService = accountService;
        }

        public bool ValidateCredentials(LoginModel login)
        {
            return _accountService.LoginApi(login.Username, login.Password);
        }
    }
}
