using System;
using System.Collections.Generic;
using System.Linq;
using FomMonitoringCore.SqlServer;

namespace UserManager.Gateway.Concrete
{
    public class UsersRoles
    {
        public static void DeleteUserRoles(Guid ID)
        {
            try
            {
                using (var userManagerEntities = new FomMonitoringEntities())
                {
                    
                    if (!userManagerEntities.Roles_Users.Where(r => r.ID == ID).Any())
                    {
                        
                        throw new Exception("Error not found: {0}");
                    }

                   
                    var rolesToDelete = userManagerEntities.Roles_Users.Where(r => r.ID == ID).FirstOrDefault();

                    userManagerEntities.Roles_Users.Remove(rolesToDelete);

                    // Persisto le modifiche nel DB
                    userManagerEntities.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error during the delete of the user: {ex.Message}");
            }
        }

        public static List<Roles_Users> GetUsersRoles(Guid UserID)
        {
            using (var userManagerEntities = new FomMonitoringEntities())
            {
                return GetUsersRoles(userManagerEntities, UserID);
            }
        }

        public static List<Roles_Users> GetUsersRoles()
        {
            using (var userManagerEntities = new FomMonitoringEntities())
            {
                return GetUsersRoles(userManagerEntities);
            }
        }

        public static List<Roles_Users> GetUsersRoles(FomMonitoringEntities userManagerEntities)
        {
            try
            {
                return (from r in userManagerEntities.Roles_Users
                        select r).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error while getting user roles: {ex.Message}");
            }
        }

        public static List<Roles_Users> GetUsersRoles(FomMonitoringEntities userManagerEntities, Guid UserID)
        {
            try
            {
                return (from r in userManagerEntities.Roles_Users
                        where r.UserID == UserID
                        select r).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error while getting user roles: {ex.Message}");
            }
        }
    }
}
