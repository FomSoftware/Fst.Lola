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
        
        public Guid GetLoggedUserID()
        {
            var user = GetLoggedUser();
            return user?.ID ?? Guid.Empty;
        }


        public string GetLoggedUserDefualtHomePage()
        {
            var user = GetLoggedUser();
            if (null == user)
                return string.Empty;

            var userHomePage = user.DefaultHomePage;

            if (!string.IsNullOrEmpty(userHomePage)) 
                return userHomePage;

            var firstOrDefault = user.Roles_Users.FirstOrDefault();
            if (firstOrDefault != null) 
                userHomePage = firstOrDefault.Roles.HomePage;

            return string.IsNullOrEmpty(userHomePage) 
                ? string.Empty 
                : userHomePage;
        }

        public FomMonitoringCore.SqlServer.Users GetLoggedUser()
        {
            var user = SessionsVariables.GetLoggedUser();
            return user;
        }



        #endregion

    }
}
