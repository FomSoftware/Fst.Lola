using System;
using System.Collections.Generic;
using System.Linq;
using UserManager.Framework.Common;
using UserManager.Gateway;

namespace UserManager.Service.Concrete
{
    public class LoggedUserServices : ILoggedUserServices
    {

        #region Logged User


        public FomMonitoringCore.SqlServer.Users GetLoggedUser()
        {
            var user = SessionsVariables.GetLoggedUser();
            return user;
        }



        #endregion

    }
}
