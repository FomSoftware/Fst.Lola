using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserManager.DAL.Gateway.Concrete
{
    public class UsersGroups
    {

        public static void DeleteUserGroups(Guid ID)
        {
            try
            {
                using (UserManagerEntities userManagerEntities = new UserManagerEntities())
                {

                    if (!userManagerEntities.Groups_Users.Where(r => r.ID == ID).Any())
                    {

                        throw new Exception("Error not found: {0}");
                    }


                    DAL.Groups_Users groupsToDelete = userManagerEntities.Groups_Users.Where(r => r.ID == ID).FirstOrDefault();

                    userManagerEntities.Groups_Users.Remove(groupsToDelete);

                    // Persisto le modifiche nel DB
                    userManagerEntities.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(String.Format("Error during the delete of the user: {0}", ex.Message));
            }
        }

        public static List<DAL.Groups_Users> GetUsersGroups(Guid UserID)
        {
            using (UserManagerEntities userManagerEntities = new UserManagerEntities())
            {
                return GetUsersGroups(userManagerEntities, UserID);
            }
        }

        public static List<DAL.Groups_Users> GetUsersGroups()
        {
            using (UserManagerEntities userManagerEntities = new UserManagerEntities())
            {
                return GetUsersGroups(userManagerEntities);
            }
        }

        public static List<DAL.Groups_Users> GetUsersGroups(UserManagerEntities userManagerEntities)
        {
            try
            {
                return (from r in userManagerEntities.Groups_Users
                        select r).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(String.Format("Error while getting user groups: {0}", ex.Message));
            }
        }

        public static List<DAL.Groups_Users> GetUsersGroups(UserManagerEntities userManagerEntities, Guid UserID)
        {
            try
            {
                return (from r in userManagerEntities.Groups_Users
                        where r.UserID == UserID
                        select r).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(String.Format("Error while getting user groups: {0}", ex.Message));
            }
        }
    }

}
