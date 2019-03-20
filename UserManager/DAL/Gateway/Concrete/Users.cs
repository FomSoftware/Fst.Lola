using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UserManager.Framework.Common;
using UserManager.Service.Concrete;

namespace UserManager.DAL.Gateway.Concrete
{
    public static class Users
    {
        public const string EntitySetName = "Users";

        public static bool LoginUser(string Username, string Password, out string Message, out DAL.Users User)
        {
            return LoginUser(Username, Password, "", out Message, out User);
        }

        public static bool LoginUser(string Username, string Password, string Domain, out string Message, out DAL.Users User)
        {
            try
            {
                UserManagerEntities userManagerEntities = new UserManagerEntities();
                userManagerEntities.Configuration.ProxyCreationEnabled = false;

                if (Domain != "")
                    User = userManagerEntities.Users
                        .Include("Groups_Users")
                        .Include("Groups_Users.Groups")
                        .Include("Roles_Users")
                        .Include("Roles_Users.Roles")
                        .SingleOrDefault(i => i.Username == Username && i.Domain == Domain);
                else
                    User = userManagerEntities.Users
                        .Include("Groups_Users")
                        .Include("Groups_Users.Groups")
                        .Include("Roles_Users")
                        .Include("Roles_Users.Roles")
                        .SingleOrDefault(i => i.Username == Username);

                if (null == User) { Message = "Username is wrong"; return false; }
                if (User.Password != Password) { Message = "Password is wrong"; return false; }
                if (User.Enabled == false) { Message = "User is not enabled"; return false; }

                Message = "Login done successfully";

                //Ritorna true per indicare che il login dell'utente è avvenuto correttamente
                return true;
            }
            catch (Exception ex)
            {
                Message = String.Format("Non è stato possibile effettuare il login dell'utente perchè è stata rilevata la seguente eccezione: {0}", ex.Message);
                User = null;
                return false;
            }
        }

        /**
         * Esegue il login dell'utente partendo dalla sola Username
         */
        public static bool LoginUserWithoutPassword(string Username, out string Message, out DAL.Users User)
        {
            Guid UserID = Guid.Empty;
            using (UserManagerEntities userManagerEntities = new UserManagerEntities())
            {
                UserID = (from users in userManagerEntities.Users
                          where users.Username == Username
                          select users.ID).FirstOrDefault();

            }
            return LoginUserWithoutPassword(UserID, out Message, out User);
        }

        /**
         * Esegue il login dell'utente partendo dalla sola Username e dal Dominio
         */
        public static bool LoginUserWithoutPassword(string Username, string Domain, out string Message, out DAL.Users User)
        {
            Guid UserID = Guid.Empty;
            using (UserManagerEntities userManagerEntities = new UserManagerEntities())
            {
                UserID = (from users in userManagerEntities.Users
                          where users.Username == Username
                          && users.Domain == Domain
                          select users.ID).FirstOrDefault();

            }
            return LoginUserWithoutPassword(UserID, out Message, out User);
        }

        /**
         * Autentica l'utente partendo solamente dalla UserID
         */
        public static bool LoginUserWithoutPassword(Guid UserID, out string Message, out DAL.Users User)
        {
            try
            {
                UserManagerEntities userManagerEntities = new UserManagerEntities();

                User = userManagerEntities.Users.FirstOrDefault(i => i.ID == UserID);

                if (null == User) { Message = "User not found"; new Exception(Message); return false; }
                if (User.Enabled == false) { Message = "User is not allowed to have accessed to this application"; new Exception(Message); return false; }

                //Cancella il record della richiesta
                userManagerEntities.SaveChanges();

                Message = "Login done successfully";
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error: {0} Error (#111010)", ex.Message));
            }
        }

        /**
         * Esegue il login dell'utente utilizzando il guid di una richiesta effettuata in un altra pagina
         */
        public static bool LoginUserWithRedirectAccessRequestID(Guid RequestID, out string Message, out DAL.Users User)
        {
            try
            {
                UserManagerEntities userManagerEntities = new UserManagerEntities();

                Guid UserID = RedirectAccessRequests.GetUserIDFromRequestID(RequestID, ref userManagerEntities);

                User = userManagerEntities.Users.FirstOrDefault(i => i.ID == UserID);

                if (null == User) { Message = "User not found"; new Exception(Message); return false; }
                if (User.Enabled == false) { Message = "User is not allowed to have accessed to this application"; new Exception(Message); return false; }

                //Cancella il record della richiesta
                userManagerEntities.SaveChanges();

                Message = "Login done successfully";
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error: {0} Error (#111008)", ex.Message));
            }
        }

        /**
         * Cambia la password dell'utente
         */
        public static bool ChangePassword(Guid UserID, string NewPassword, string OldPassword, out string Message)
        {
            Message = String.Empty;
            try
            {
                using (UserManagerEntities userManagerEntities = new UserManagerEntities())
                {
                    DAL.Users User = userManagerEntities.Users.First(i => i.ID == UserID);

                    if (User.Password != OldPassword) { Message = "The old password does not match with your password!"; return false; }

                    User.Password = NewPassword;
                    User.ModifiedBy = UserID;
                    User.ModifiedDate = DateTime.Now;

                    userManagerEntities.SaveChanges();
                }
                return true;
            }
            catch (Exception ex)
            {
                Message = String.Format("Error: {0} Error (#111009)", ex.Message);
                return false;
                throw;
            }
        }

        /**
         * Get users by GuidID
         */
        public static DAL.Users GetUser(Guid UserId)
        {
            try
            {
                using (UserManagerEntities userManagerEntities = new UserManagerEntities())
                {
                    return (from users in userManagerEntities.Users
                            where users.ID == UserId
                            select users).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(String.Format("Errore: {0} (Error #111009)", ex.Message));
            }
        }

        /**
        * Get users by Username
        */
        public static DAL.Users GetUser(string username)
        {
            try
            {
                using (UserManagerEntities userManagerEntities = new UserManagerEntities())
                {
                    return userManagerEntities.Users
                            .Include("Roles_Users")
                            .Include("Roles_Users.Roles")
                            .Where(w => w.Username == username).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(String.Format("Errore: {0} (Error #111009)", ex.Message));
            }
        }

        /**
        * Get users by Username e Password
        */
        public static DAL.Users GetUser(string username, string password)
        {
            try
            {
                using (UserManagerEntities userManagerEntities = new UserManagerEntities())
                {
                    return (from users in userManagerEntities.Users
                            where users.Username == username && users.Password == password
                            select users).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(String.Format("Errore: {0} (Error #111009)", ex.Message));
            }
        }

        /**
         * Get all users
         */
        public static List<DAL.Users> GetAllUsers()
        {
            try
            {
                using (UserManagerEntities userManagerEntities = new UserManagerEntities())
                {
                    return (from users in userManagerEntities.Users
                            select users).ToList();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(String.Format("Errore nel cambio password: {0} (Error #111009)", ex.Message));
            }
        }

        /**
         * Get all users
         */
        public static List<DAL.Users> GetListOfNonDeletedUsers(UserManagerEntities userManagerEntities)
        {
            try
            {
                return (from users in userManagerEntities.Users
                        where users.DeletedBy == null && users.DeletedDate == null
                        select users).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(String.Format("Errore nel cambio password: {0} (Error #111009)", ex.Message));
            }
        }

        /**
         * Get all users and check Role
         */
        public static List<DAL.Users> GetAllUsers(Enumerators.UserRole UserRole)
        {
            try
            {
                string UsersRoleString = Enum.GetName(typeof(Framework.Common.Enumerators.UserRole), (int)UserRole);

                using (UserManagerEntities userManagerEntities = new UserManagerEntities())
                {
                    return (from users in userManagerEntities.Users
                            where users.Roles_Users.Select(x => x.Roles.Name).Contains(UsersRoleString)
                            select users).ToList();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(String.Format("Errore nel cambio password: {0} (Error #111009)", ex.Message));
            }
        }

        public static List<DAL.Users> GetAllUsers(string UserRoleName)
        {
            try
            {
                using (UserManagerEntities userManagerEntities = new UserManagerEntities())
                {
                    return (from users in userManagerEntities.Users
                            where users.Roles_Users.Select(x => x.Roles.Name).Contains(UserRoleName)
                            select users).ToList();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(String.Format("Errore {0} (Error #111009)", ex.Message));
            }
        }

        /**
         * Restituisce una stringa con la culture DOT.NET partendo dall'ID dell'utente
         */
        public static string GetDotNetCultureFromUserID(Guid UserID)
        {
            try
            {
                using (UserManagerEntities userManagerEntities = new UserManagerEntities())
                {
                    return (from users in userManagerEntities.Users
                            where users.ID == UserID
                            select users.Languages.DotNetCulture).Distinct().FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(String.Format("Errore recupero UserID culture: {0} (Error #111010)", ex.Message));
            }
        }

        public static bool CheckIfUsernameAlreadyExist(string Username)
        {
            try
            {
                using (UserManagerEntities userManagerEntities = new UserManagerEntities())
                {
                    return userManagerEntities.Users.Where(w => w.Username.ToLower() == Username.ToLower()).Any();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(String.Format("Error during the creation of new user: {0}", ex.Message));
            }
        }

        /// <summary>
        /// Crea un nuovo utente nel database
        /// </summary>
        /// <param name="User"></param>
        public static void CreateUser(DAL.Users User)
        {
            try
            {
                using (UserManagerEntities userManagerEntities = new UserManagerEntities())
                {
                    //Verifico che la username non sia già presente
                    if (CheckIfUsernameAlreadyExist(User.Username))
                    {
                        // errore - nome utente esiste già
                        throw new Exception("Error username already exists: {0}");
                    }

                    // Aggiungo l'utente a DB
                    userManagerEntities.Users.Add(User);
                    userManagerEntities.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(String.Format("Error during the creation of new user: {0}", ex.Message));
            }
        }

        /// <summary>
        /// Modifica alcune proprietà dell'utente
        /// </summary>
        /// <param name="User"></param>
        public static void ModifyUser(DAL.Users User)
        {
            try
            {
                using (UserManagerEntities userManagerEntities = new UserManagerEntities())
                {
                    // Verifico che l'utente esista nel db
                    if (!userManagerEntities.Users.Where(w => w.Username.ToLower() == User.Username.ToLower()).Any())
                    {
                        // Errore - utente non trovato
                        throw new Exception("Error user not found: {0}");
                    }

                    // Recupero lo user dal db
                    DAL.Users userToModify = userManagerEntities.Users.Where(w => w.Username.ToLower() == User.Username.ToLower()).FirstOrDefault();

                    // Aggiorno l'utente con le nuove informazioni (solo alcuni campi vengono aggiornati)
                    userToModify.FirstName = User.FirstName;
                    userToModify.LastName = User.LastName;
                    userToModify.Email = User.Email;
                    userToModify.Domain = User.Domain;
                    userToModify.DefaultHomePage = User.DefaultHomePage;
                    userToModify.Status = User.Status;
                    userToModify.LanguageID = User.LanguageID;

                    // Persisto le modifiche nel DB
                    userManagerEntities.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(String.Format("Error during the update of the user: {0}", ex.Message));
            }
        }
        /// <summary>
        /// Disabilita un utente
        /// </summary>
        /// <param name="User"></param>
        public static void DeleteUser(Guid UserID)
        {
            try
            {
                using (UserManagerEntities userManagerEntities = new UserManagerEntities())
                {
                    // Verifico che l'utente esista nel db
                    if (!userManagerEntities.Users.Where(w => w.ID == UserID).Any())
                    {
                        // Errore - utente non trovato
                        throw new Exception("Error user not found: {0}");
                    }

                    // Recupero lo user dal db
                    DAL.Users userToDelete = userManagerEntities.Users.Where(w => w.ID == UserID).FirstOrDefault();

                    // disabilito l'utente
                    userToDelete.Enabled = false;
                    Service.ILoggedUserServices iLogUser = new Service.Concrete.LoggedUserServices();
                    userToDelete.DeletedBy = iLogUser.GetLoggedUserID();
                    userToDelete.DeletedDate = DateTime.Now;

                    // Persisto le modifiche nel DB
                    userManagerEntities.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(String.Format("Error during the delete of the user: {0}", ex.Message));
            }
        }



        /// <summary>
        /// Disabilita un utente
        /// </summary>
        /// <param name="User"></param>
        public static void DeleteUser(DAL.Users User)
        {
            try
            {
                using (UserManagerEntities userManagerEntities = new UserManagerEntities())
                {
                    // Verifico che l'utente esista nel db
                    if (!userManagerEntities.Users.Where(w => w.Username.ToLower() == User.Username.ToLower()).Any())
                    {
                        // Errore - utente non trovato
                        throw new Exception("Error user not found: {0}");
                    }

                    // Recupero lo user dal db
                    DAL.Users userToDelete = userManagerEntities.Users.Where(w => w.Username.ToLower() == User.Username.ToLower()).FirstOrDefault();

                    // disabilito l'utente
                    userToDelete.Enabled = false;
                    userToDelete.DeletedDate = DateTime.Now;

                    // Persisto le modifiche nel DB
                    userManagerEntities.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(String.Format("Error during the delete of the user: {0}", ex.Message));
            }
        }

    }
}
