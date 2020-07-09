using System;
using System.Collections.Generic;
using System.Linq;
using FomMonitoringCore.SqlServer;
using UserManager.Framework.Common;

namespace UserManager.Gateway.Concrete
{
    public static class Users
    {
        public const string EntitySetName = "Users";

        public static bool LoginUser(string username, string password, out string message, out FomMonitoringCore.SqlServer.Users user)
        {
            return LoginUser(username, password, "", out message, out user);
        }

        public static bool LoginUser(string username, string password, string domain, out string message, out FomMonitoringCore.SqlServer.Users user)
        {

                using (var fomMonitoringEntities = new FomMonitoringEntities())
                {
                    try
                    {
                        fomMonitoringEntities.Configuration.ProxyCreationEnabled = false;

                        if (domain != "")
                            user = fomMonitoringEntities.Users
                                .Include("Groups_Users")
                                .Include("Groups_Users.Groups")
                                .Include("Roles_Users")
                                .Include("Roles_Users.Roles")
                                //.Include("Languages")
                                .SingleOrDefault(i => i.Username == username && i.Domain == domain);
                        else
                            user = fomMonitoringEntities.Users
                                .Include("Groups_Users")
                                .Include("Groups_Users.Groups")
                                .Include("Roles_Users")
                                .Include("Roles_Users.Roles")
                                //.Include("Languages")
                                .SingleOrDefault(i => i.Username == username);

                        if (null == user) { message = "Username is wrong"; return false; }
                        if (user.Password != password) { message = "Password is wrong"; return false; }
                        if (user.Enabled == false) { message = "User is not enabled"; return false; }

                        message = "Login done successfully";

                        //Ritorna true per indicare che il login dell'utente è avvenuto correttamente
                        return true;
                    }
                    catch (Exception ex)
                    {
                        message =
                            $"Non è stato possibile effettuare il login dell'utente perchè è stata rilevata la seguente eccezione: {ex.Message}";
                        user = null;
                        return false;
                    }
                }

        }

        /**
         * Esegue il login dell'utente partendo FomMonitoringCore.SqlServerla sola Username
         */
        public static bool LoginUserWithoutPassword(string username, out string message, out FomMonitoringCore.SqlServer.Users user)
        {
            var userId = Guid.Empty;
            using (var fomMonitoringEntities = new FomMonitoringEntities())
            {
                userId = (from users in fomMonitoringEntities.Users
                          where users.Username == username
                          select users.ID).FirstOrDefault();

            }
            return LoginUserWithoutPassword(userId, out message, out user);
        }

        /**
         * Esegue il login dell'utente partendo FomMonitoringCore.SqlServerla sola Username e FomMonitoringCore.SqlServer Dominio
         */
        public static bool LoginUserWithoutPassword(string username, string domain, out string message, out FomMonitoringCore.SqlServer.Users user)
        {
            var userId = Guid.Empty;
            using (var fomMonitoringEntities = new FomMonitoringEntities())
            {
                userId = (from users in fomMonitoringEntities.Users
                          where users.Username == username
                          && users.Domain == domain
                          select users.ID).FirstOrDefault();

            }
            return LoginUserWithoutPassword(userId, out message, out user);
        }

        /**
         * Autentica l'utente partendo solamente FomMonitoringCore.SqlServerla UserID
         */
        public static bool LoginUserWithoutPassword(Guid userId, out string message, out FomMonitoringCore.SqlServer.Users user)
        {
            try
            {
                var fomMonitoringEntities = new FomMonitoringEntities();

                user = fomMonitoringEntities.Users.FirstOrDefault(i => i.ID == userId);

                if (null == user) { message = "User not found"; new Exception(message); return false; }
                if (user.Enabled == false) { message = "User is not allowed to have accessed to this application"; new Exception(message); return false; }

                //Cancella il record della richiesta
                fomMonitoringEntities.SaveChanges();

                message = "Login done successfully";
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error: {ex.Message} Error (#111010)");
            }
        }


        /**
         * Cambia la password dell'utente
         */
        public static bool ChangePassword(Guid userId, string newPassword, string oldPassword, out string message)
        {
            message = string.Empty;
            try
            {
                using (var fomMonitoringEntities = new FomMonitoringEntities())
                {
                    var user = fomMonitoringEntities.Users.First(i => i.ID == userId);

                    if (user.Password != oldPassword) { message = "The old password does not match with your password!"; return false; }

                    user.Password = newPassword;
                    user.ModifiedBy = userId;
                    user.ModifiedDate = DateTime.Now;
                    user.LastDateUpdatePassword = DateTime.Now;
                    fomMonitoringEntities.SaveChanges();
                }
                return true;
            }
            catch (Exception ex)
            {
                message = $"Error: {ex.Message} Error (#111009)";
                return false;
                throw;
            }
        }

        /**
         * Get users by GuidID
         */
        public static FomMonitoringCore.SqlServer.Users GetUser(Guid userId)
        {
            try
            {
                using (var fomMonitoringEntities = new FomMonitoringEntities())
                {
                    return (from users in fomMonitoringEntities.Users
                            where users.ID == userId
                            select users).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Errore: {ex.Message} (Error #111009)");
            }
        }

        /**
        * Get users by Username
        */
        public static FomMonitoringCore.SqlServer.Users GetUser(string username)
        {
            try
            {
                using (var fomMonitoringEntities = new FomMonitoringEntities())
                {
                    return fomMonitoringEntities.Users
                            .Include("Roles_Users")
                            .Include("Roles_Users.Roles").FirstOrDefault(w => w.Username == username);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Errore: {ex.Message} (Error #111009)");
            }
        }

        /**
        * Get users by Username e Password
        */
        public static FomMonitoringCore.SqlServer.Users GetUser(string username, string password)
        {
            try
            {
                using (var fomMonitoringEntities = new FomMonitoringEntities())
                {
                    return (from users in fomMonitoringEntities.Users
                            where users.Username == username && users.Password == password
                            select users).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Errore: {ex.Message} (Error #111009)");
            }
        }

        /**
         * Get all users
         */
        public static List<FomMonitoringCore.SqlServer.Users> GetAllUsers()
        {
            try
            {
                using (var fomMonitoringEntities = new FomMonitoringEntities())
                {
                    return (from users in fomMonitoringEntities.Users
                            select users).ToList();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Errore nel cambio password: {ex.Message} (Error #111009)");
            }
        }

        /**
         * Get all users
         */
        public static List<FomMonitoringCore.SqlServer.Users> GetListOfNonDeletedUsers(FomMonitoringEntities fomMonitoringEntities)
        {
            try
            {
                return (from users in fomMonitoringEntities.Users
                        where users.DeletedBy == null && users.DeletedDate == null
                        select users).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception($"Errore nel cambio password: {ex.Message} (Error #111009)");
            }
        }

        /**
         * Get all users and check Role
         */
        public static List<FomMonitoringCore.SqlServer.Users> GetAllUsers(Enumerators.UserRole userRole)
        {
            try
            {
                var usersRoleString = Enum.GetName(typeof(Enumerators.UserRole), (int)userRole);

                using (var fomMonitoringEntities = new FomMonitoringEntities())
                {
                    return (from users in fomMonitoringEntities.Users
                            where users.Roles_Users.Select(x => x.Roles.Name).Contains(usersRoleString)
                            select users).ToList();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Errore nel cambio password: {ex.Message} (Error #111009)");
            }
        }

        public static List<FomMonitoringCore.SqlServer.Users> GetAllUsers(string userRoleName)
        {
            try
            {
                using (var fomMonitoringEntities = new FomMonitoringEntities())
                {
                    return (from users in fomMonitoringEntities.Users
                            where users.Roles_Users.Select(x => x.Roles.Name).Contains(userRoleName)
                            select users).ToList();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Errore {ex.Message} (Error #111009)");
            }
        }

        /**
         * Restituisce una stringa con la culture DOT.NET partendo FomMonitoringCore.SqlServerl'ID dell'utente
         */
        public static string GetDotNetCultureFromUserId(Guid userId)
        {
            try
            {
                using (var fomMonitoringEntities = new FomMonitoringEntities())
                {
                    return (from users in fomMonitoringEntities.Users
                            where users.ID == userId
                            select users.Languages.DotNetCulture).Distinct().FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Errore recupero UserID culture: {ex.Message} (Error #111010)");
            }
        }

        public static bool CheckIfUsernameAlreadyExist(string username)
        {
            try
            {
                using (var fomMonitoringEntities = new FomMonitoringEntities())
                {
                    return fomMonitoringEntities.Users.Any(w => w.Username.ToLower() == username.ToLower());
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error during the creation of new user: {ex.Message}");
            }
        }

        /// <summary>
        /// Crea un nuovo utente nel database
        /// </summary>
        /// <param name="user"></param>
        public static void CreateUser(FomMonitoringCore.SqlServer.Users user)
        {
            try
            {
                using (var fomMonitoringEntities = new FomMonitoringEntities())
                {
                    //Verifico che la username non sia già presente
                    if (CheckIfUsernameAlreadyExist(user.Username))
                    {
                        // errore - nome utente esiste già
                        throw new Exception("Error username already exists: {0}");
                    }

                    // Aggiungo l'utente a DB
                    fomMonitoringEntities.Users.Add(user);
                    fomMonitoringEntities.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error during the creation of new user: {ex.Message}");
            }
        }

        /// <summary>
        /// Modifica alcune proprietà dell'utente
        /// </summary>
        /// <param name="user"></param>
        public static void ModifyUser(FomMonitoringCore.SqlServer.Users user)
        {
            try
            {
                using (var fomMonitoringEntities = new FomMonitoringEntities())
                {
                    // Verifico che l'utente esista nel db
                    if (!fomMonitoringEntities.Users.Any(w => w.Username.ToLower() == user.Username.ToLower()))
                    {
                        // Errore - utente non trovato
                        throw new Exception("Error user not found: {0}");
                    }

                    // Recupero lo user FomMonitoringCore.SqlServer db
                    var userToModify = fomMonitoringEntities.Users.FirstOrDefault(w => w.Username.ToLower() == user.Username.ToLower());

                    // Aggiorno l'utente con le nuove informazioni (solo alcuni campi vengono aggiornati)
                    userToModify.FirstName = user.FirstName;
                    userToModify.LastName = user.LastName;
                    userToModify.Email = user.Email;
                    userToModify.Domain = user.Domain;
                    userToModify.DefaultHomePage = user.DefaultHomePage;
                    userToModify.Status = user.Status;
                    userToModify.LanguageID = user.LanguageID;

                    // Persisto le modifiche nel DB
                    fomMonitoringEntities.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error during the update of the user: {ex.Message}");
            }
        }

        /// <summary>
        /// Disabilita un utente
        /// </summary>
        /// <param name="userId"></param>
        public static void DeleteUser(Guid userId)
        {
            try
            {
                using (var fomMonitoringEntities = new FomMonitoringEntities())
                {
                    // Verifico che l'utente esista nel db
                    if (!fomMonitoringEntities.Users.Any(w => w.ID == userId))
                    {
                        // Errore - utente non trovato
                        throw new Exception("Error user not found: {0}");
                    }

                    // Recupero lo user FomMonitoringCore.SqlServer db
                    var userToDelete = fomMonitoringEntities.Users.FirstOrDefault(w => w.ID == userId);

                    // disabilito l'utente
                    userToDelete.Enabled = false;
                    Service.ILoggedUserServices iLogUser = new Service.Concrete.LoggedUserServices();
                    userToDelete.DeletedBy = iLogUser.GetLoggedUserID();
                    userToDelete.DeletedDate = DateTime.Now;

                    // Persisto le modifiche nel DB
                    fomMonitoringEntities.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error during the delete of the user: {ex.Message}");
            }
        }



        /// <summary>
        /// Disabilita un utente
        /// </summary>
        /// <param name="user"></param>
        public static void DeleteUser(FomMonitoringCore.SqlServer.Users user)
        {
            try
            {
                using (var fomMonitoringEntities = new FomMonitoringEntities())
                {
                    // Verifico che l'utente esista nel db
                    if (!fomMonitoringEntities.Users.Any(w => w.Username.ToLower() == user.Username.ToLower()))
                    {
                        // Errore - utente non trovato
                        throw new Exception("Error user not found: {0}");
                    }

                    // Recupero lo user FomMonitoringCore.SqlServer db
                    var userToDelete = fomMonitoringEntities.Users.FirstOrDefault(w => w.Username.ToLower() == user.Username.ToLower());

                    // disabilito l'utente
                    userToDelete.Enabled = false;
                    userToDelete.DeletedDate = DateTime.Now;

                    // Persisto le modifiche nel DB
                    fomMonitoringEntities.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error during the delete of the user: {ex.Message}");
            }
        }

    }
}
