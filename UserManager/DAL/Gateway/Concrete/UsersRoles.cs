using System;
using System.Collections.Generic;
using System.Linq;

namespace UserManager.DAL.Gateway.Concrete
{
    public class UsersRoles
    {
        public static void DeleteUserRoles(Guid ID)
        {
            try
            {
                using (UserManagerEntities userManagerEntities = new UserManagerEntities())
                {
                    
                    if (!userManagerEntities.Roles_Users.Where(r => r.ID == ID).Any())
                    {
                        
                        throw new Exception("Error not found: {0}");
                    }

                   
                    DAL.Roles_Users rolesToDelete = userManagerEntities.Roles_Users.Where(r => r.ID == ID).FirstOrDefault();

                    userManagerEntities.Roles_Users.Remove(rolesToDelete);

                    // Persisto le modifiche nel DB
                    userManagerEntities.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(String.Format("Error during the delete of the user: {0}", ex.Message));
            }
        }

        public static List<DAL.Roles_Users> GetUsersRoles(Guid UserID)
        {
            using (UserManagerEntities userManagerEntities = new UserManagerEntities())
            {
                return GetUsersRoles(userManagerEntities, UserID);
            }
        }

        public static List<DAL.Roles_Users> GetUsersRoles()
        {
            using (UserManagerEntities userManagerEntities = new UserManagerEntities())
            {
                return GetUsersRoles(userManagerEntities);
            }
        }

        public static List<DAL.Roles_Users> GetUsersRoles(UserManagerEntities userManagerEntities)
        {
            try
            {
                return (from r in userManagerEntities.Roles_Users
                        select r).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(String.Format("Error while getting user roles: {0}", ex.Message));
            }
        }

        public static List<DAL.Roles_Users> GetUsersRoles(UserManagerEntities userManagerEntities, Guid UserID)
        {
            try
            {
                return (from r in userManagerEntities.Roles_Users
                        where r.UserID == UserID
                        select r).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(String.Format("Error while getting user roles: {0}", ex.Message));
            }
        }
    }
}
