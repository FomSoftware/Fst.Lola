using System;
using System.Collections.Generic;
using System.Linq;
using UserManager.DAL;
using UserManager.Framework.Common;
using RedirectAccessRequests = UserManager.DAL.Gateway.Concrete.RedirectAccessRequests;

namespace UserManager.Service.Concrete
{
    public class LoggedUserServices : ILoggedUserServices
    {
        #region Logged User

        public Guid GetLoggedUserID()
        {
            var user = GetLoggedUser();
            return null == user ? Guid.Empty : user.ID;
        }

        public string GetLoggedUserName()
        {
            var user = GetLoggedUser();
            return null == user ? string.Empty : user.Username;
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

        public Users GetLoggedUser()
        {
            var user = SessionsVariables.GetLoggedUser();
            return user;
        }

        //-------- ROLE SECTION ---------

        public bool CheckUserRole(int idRole)
        {
            IRolesService rol = new RoleService();
            return rol.CheckUserRoles(GetLoggedUserID(), idRole);
        }

        public bool CheckUserRole(Roles role)
        {
            IRolesService rol = new RoleService();
            return rol.CheckUserRoles(GetLoggedUserID(), role);
        }

        public List<Roles> GetLoggedUserRoles()
        {
            IRolesService rol = new RoleService();
            return rol.GetUserRoles(GetLoggedUserID());
        }

        public string GetLoggedUserRolesString()
        {
            IRolesService rol = new RoleService();
            return String.Join(",", rol.GetUserRoles(GetLoggedUserID()).Select(s => s.Name).ToList());
        }

        //--------END ROLE SECTION ---------

        //-------- GROUP SECTION -----------

        public List<Groups> GetLoggedUserGroups()
        {
            return GroupService.GetUserGroups(GetLoggedUserID());
        }

        //------- END GROUP SECTION --------

        public Guid SetUserRedirectAccessRequestsAndGetGuidRequest()
        {
            var user = GetLoggedUser();
            var requestId = Guid.NewGuid();
            if (null == user) { throw new Exception("Error: User is null! Error (#111006)"); }
            RedirectAccessRequests.InsertRedirectAccessRequests(requestId, user.ID);
            return requestId;
        }

        #endregion

    }
}
