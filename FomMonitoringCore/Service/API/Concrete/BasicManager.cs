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
            return AccountService.LoginApi(login.Username, login.Password);
        }
    }
}
