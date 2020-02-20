using System;
using System.Collections.Generic;
using UserManager.DAL;

namespace UserManager.Service
{
  public  interface IUsersGroups
    {
        List<DAL.Groups_Users> GetUsersGroups(Guid UserID);

        List<DAL.Groups_Users> GetUsersGroups(UserManagerEntities userManagerEntities, Guid UserID);

        void DeleteUserGroups(Guid ID);

    }
}
