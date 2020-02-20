using System;
using System.Collections.Generic;
using UserManager.DAL;

namespace UserManager.Service.Concrete
{
    public class UsersGroupsServices : IUsersGroups
    {
        public List<DAL.Groups_Users> GetUsersGroups()
        {
            return DAL.Gateway.Concrete.UsersGroups.GetUsersGroups();
        }
        
        public List<DAL.Groups_Users> GetUsersGroups(Guid UserID)
        {
            return DAL.Gateway.Concrete.UsersGroups.GetUsersGroups(UserID);
        }

        public List<DAL.Groups_Users> GetUsersGroups(UserManagerEntities userManagerEntities, Guid UserID)
        {
            return DAL.Gateway.Concrete.UsersGroups.GetUsersGroups(userManagerEntities, UserID);
        }

        public void DeleteUserGroups(Guid ID)
        {
            DAL.Gateway.Concrete.UsersGroups.DeleteUserGroups(ID);
        }

    }
}
