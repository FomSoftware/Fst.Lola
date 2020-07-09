using System;
using System.Collections.Generic;
namespace UserManager.Service
{
    public interface ILoggedUserServices
    {
        Guid GetLoggedUserID();

        FomMonitoringCore.SqlServer.Users GetLoggedUser();

        string GetLoggedUserDefualtHomePage();





    }
}
