using FomMonitoringCore.Framework.Common;
using FomMonitoringCore.Model;
using System.Linq;

namespace FomMonitoringCore.Service.API.Concrete
{
    public class BasicManager : IBasicManager
    {
        public bool ValidateCredentials(LoginModel login)
        {
            return AccountService.GetUser(login.Username, login.Password).Roles.Any(a => a == enRole.UserApi);
        }
    }
}