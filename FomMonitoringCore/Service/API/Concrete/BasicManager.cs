using FomMonitoringCore.Framework.Common;
using FomMonitoringCore.Framework.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FomMonitoringCore.Service.API.Concrete
{
    public class BasicManager : IBasicManager
    {
        public bool ValidateCredentials(LoginModel login)
        {
            var user = AccountService.LoginApi(login.Username, login.Password);
            return (user == null) ? false : user.Role == enRole.UserApi;
        }
    }
}
