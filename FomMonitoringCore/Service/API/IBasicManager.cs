using FomMonitoringCore.Framework.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FomMonitoringCore.Service.API
{
    public interface IBasicManager
    {
        bool ValidateCredentials(LoginModel login);
    }
}
