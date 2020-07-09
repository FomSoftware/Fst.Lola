using System;
using System.Collections.Generic;
using System.Linq;
using FomMonitoringCore.SqlServer;
using UserManager.Framework.Common;

namespace UserManager.Gateway.Concrete
{
    public class Users : IUsers
    {
        private readonly IFomMonitoringEntities _fomMonitoringEntities;

        public Users(IFomMonitoringEntities fomMonitoringEntities)
        {
            _fomMonitoringEntities = fomMonitoringEntities;
        }


        public bool LoginUser(string username, string password, string domain, out string message, out FomMonitoringCore.SqlServer.Users user)
        {

            try
            {

                if (domain != "")
                    user = _fomMonitoringEntities.Set<FomMonitoringCore.SqlServer.Users>()
                        .Include("Roles_Users")
                        .Include("Roles_Users.Roles")
                        .SingleOrDefault(i => i.Username == username && i.Domain == domain);
                else
                    user = _fomMonitoringEntities.Set<FomMonitoringCore.SqlServer.Users>()
                        .Include("Roles_Users")
                        .Include("Roles_Users.Roles")
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

        /**
         * Esegue il login dell'utente partendo FomMonitoringCore.SqlServerla sola Username
         */
        public bool LoginUserWithoutPassword(string username, out string message, out FomMonitoringCore.SqlServer.Users user)
        {

            var userId = _fomMonitoringEntities.Set<FomMonitoringCore.SqlServer.Users>()
                .FirstOrDefault(u => u.Username == username)?.ID ?? Guid.Empty;

            
            return LoginUserWithoutPassword(userId, out message, out user);
        }

        /**
         * Esegue il login dell'utente partendo FomMonitoringCore.SqlServerla sola Username e FomMonitoringCore.SqlServer Dominio
         */
        public bool LoginUserWithoutPassword(string username, string domain, out string message, out FomMonitoringCore.SqlServer.Users user)
        {
            var userId = _fomMonitoringEntities.Set<FomMonitoringCore.SqlServer.Users>().FirstOrDefault(users =>
                         users.Username == username
                         && users.Domain == domain)?.ID ?? Guid.Empty;

            
            return LoginUserWithoutPassword(userId, out message, out user);
        }

        /**
         * Autentica l'utente partendo solamente FomMonitoringCore.SqlServerla UserID
         */
        public bool LoginUserWithoutPassword(Guid userId, out string message, out FomMonitoringCore.SqlServer.Users user)
        {
            try
            {

                user = _fomMonitoringEntities.Set<FomMonitoringCore.SqlServer.Users>().FirstOrDefault(i => i.ID == userId);

                if (null == user) { message = "User not found"; new Exception(message); return false; }
                if (user.Enabled == false) { message = "User is not allowed to have accessed to this application"; new Exception(message); return false; }

                //Cancella il record della richiesta
                _fomMonitoringEntities.SaveChanges();

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
        public bool ChangePassword(Guid userId, string newPassword, string oldPassword, out string message)
        {
            message = string.Empty;
            try
            {

                    var user = _fomMonitoringEntities.Set<FomMonitoringCore.SqlServer.Users>().First(i => i.ID == userId);

                    if (user.Password != oldPassword) { message = "The old password does not match with your password!"; return false; }

                    user.Password = newPassword;
                    user.ModifiedBy = userId;
                    user.ModifiedDate = DateTime.Now;
                    user.LastDateUpdatePassword = DateTime.Now;
                    _fomMonitoringEntities.SaveChanges();
                
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
        public FomMonitoringCore.SqlServer.Users GetUser(Guid userId)
        {
            try
            {

                return _fomMonitoringEntities.Set<FomMonitoringCore.SqlServer.Users>().FirstOrDefault(i => i.ID == userId);
                
            }
            catch (Exception ex)
            {
                throw new Exception($"Errore: {ex.Message} (Error #111009)");
            }
        }

        /**
        * Get users by Username
        */
        public FomMonitoringCore.SqlServer.Users GetUser(string username)
        {
            try
            {

                    return _fomMonitoringEntities.Set<FomMonitoringCore.SqlServer.Users>()
                            .Include("Roles_Users")
                            .Include("Roles_Users.Roles").FirstOrDefault(w => w.Username == username);
                
            }
            catch (Exception ex)
            {
                throw new Exception($"Errore: {ex.Message} (Error #111009)");
            }
        }

        /**
        * Get users by Username e Password
        */
        public FomMonitoringCore.SqlServer.Users GetUser(string username, string password)
        {
            try
            {

                    return _fomMonitoringEntities.Set<FomMonitoringCore.SqlServer.Users>()
                        .Include("Roles_Users")
                        .Include("Roles_Users.Roles").FirstOrDefault(w => w.Username == username && w.Password == password);
                
            }
            catch (Exception ex)
            {
                throw new Exception($"Errore: {ex.Message} (Error #111009)");
            }
        }

        /**
         * Get all users
         */
        public List<FomMonitoringCore.SqlServer.Users> GetAllUsers()
        {
            try
            {
                return _fomMonitoringEntities.Set<FomMonitoringCore.SqlServer.Users>().ToList();
                
            }
            catch (Exception ex)
            {
                throw new Exception($"Errore nel cambio password: {ex.Message} (Error #111009)");
            }
        }

        /**
         * Get all users
         */
        public List<FomMonitoringCore.SqlServer.Users> GetListOfNonDeletedUsers(FomMonitoringEntities fomMonitoringEntities)
        {
            try
            {
                return _fomMonitoringEntities.Set<FomMonitoringCore.SqlServer.Users>()
                    .Where(users => users.DeletedBy == null && users.DeletedDate == null).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception($"Errore nel cambio password: {ex.Message} (Error #111009)");
            }
        }

        /**
         * Get all users and check Role
         */
        public List<FomMonitoringCore.SqlServer.Users> GetAllUsers(Enumerators.UserRole userRole)
        {
            try
            {
                var usersRoleString = Enum.GetName(typeof(Enumerators.UserRole), (int)userRole);

                return _fomMonitoringEntities.Set<FomMonitoringCore.SqlServer.Users>()
                    .Where(users => users.Roles_Users.Select(x => x.Roles.Name).Contains(usersRoleString)).ToList();
                
                
            }
            catch (Exception ex)
            {
                throw new Exception($"Errore nel cambio password: {ex.Message} (Error #111009)");
            }
        }

        public List<FomMonitoringCore.SqlServer.Users> GetAllUsers(string userRoleName)
        {
            try
            {

                return _fomMonitoringEntities.Set<FomMonitoringCore.SqlServer.Users>().Where(users => users.Roles_Users.Select(x => x.Roles.Name).Contains(userRoleName)).ToList();

            }
            catch (Exception ex)
            {
                throw new Exception($"Errore {ex.Message} (Error #111009)");
            }
        }

        /**
         * Restituisce una stringa con la culture DOT.NET partendo FomMonitoringCore.SqlServerl'ID dell'utente
         */
        public string GetDotNetCultureFromUserId(Guid userId)
        {
            try
            {

                return _fomMonitoringEntities.Set<FomMonitoringCore.SqlServer.Users>().FirstOrDefault(users => users.ID == userId)?.Languages.DotNetCulture;
                
            }
            catch (Exception ex)
            {
                throw new Exception($"Errore recupero UserID culture: {ex.Message} (Error #111010)");
            }
        }

        public bool CheckIfUsernameAlreadyExist(string username)
        {
            try
            {
                    return _fomMonitoringEntities.Set<FomMonitoringCore.SqlServer.Users>().Any(w => string.Equals(w.Username, username, StringComparison.CurrentCultureIgnoreCase));
                
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
        public void CreateUser(FomMonitoringCore.SqlServer.Users user)
        {
            try
            {
                    //Verifico che la username non sia già presente
                    if (CheckIfUsernameAlreadyExist(user.Username))
                    {
                        // errore - nome utente esiste già
                        throw new Exception("Error username already exists: {0}");
                    }

                // Aggiungo l'utente a DB
                _fomMonitoringEntities.Set<FomMonitoringCore.SqlServer.Users>().Add(user);
                    _fomMonitoringEntities.SaveChanges();
                
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
        public void ModifyUser(FomMonitoringCore.SqlServer.Users user)
        {
            try
            {
                // Verifico che l'utente esista nel db
                if (!_fomMonitoringEntities.Set<FomMonitoringCore.SqlServer.Users>().Any(w => string.Equals(w.Username, user.Username, StringComparison.CurrentCultureIgnoreCase)))
                {
                    // Errore - utente non trovato
                    throw new Exception("Error user not found: {0}");
                }

                // Recupero lo user FomMonitoringCore.SqlServer db
                var userToModify = _fomMonitoringEntities.Set<FomMonitoringCore.SqlServer.Users>().First(w => string.Equals(w.Username, user.Username, StringComparison.CurrentCultureIgnoreCase));

                // Aggiorno l'utente con le nuove informazioni (solo alcuni campi vengono aggiornati)
                userToModify.FirstName = user.FirstName;
                userToModify.LastName = user.LastName;
                userToModify.Email = user.Email;
                userToModify.Domain = user.Domain;
                userToModify.DefaultHomePage = user.DefaultHomePage;
                userToModify.Status = user.Status;
                userToModify.LanguageID = user.LanguageID;

                // Persisto le modifiche nel DB
                _fomMonitoringEntities.SaveChanges();
                
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
        public void DeleteUser(Guid userId)
        {
            try
            {
                    // Verifico che l'utente esista nel db
                    if (!_fomMonitoringEntities.Set<FomMonitoringCore.SqlServer.Users>().Any(w => w.ID == userId))
                    {
                        // Errore - utente non trovato
                        throw new Exception("Error user not found: {0}");
                    }

                    // Recupero lo user FomMonitoringCore.SqlServer db
                    var userToDelete = _fomMonitoringEntities.Set<FomMonitoringCore.SqlServer.Users>().First(w => w.ID == userId);

                    // disabilito l'utente
                    userToDelete.Enabled = false;
                    Service.ILoggedUserServices iLogUser = new Service.Concrete.LoggedUserServices();
                    userToDelete.DeletedBy = iLogUser.GetLoggedUserID();
                    userToDelete.DeletedDate = DateTime.Now;

                    // Persisto le modifiche nel DB
                    _fomMonitoringEntities.SaveChanges();
                
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
        public void DeleteUser(FomMonitoringCore.SqlServer.Users user)
        {
            try
            {

                    // Verifico che l'utente esista nel db
                if (!_fomMonitoringEntities.Set<FomMonitoringCore.SqlServer.Users>().Any(w => string.Equals(w.Username, user.Username, StringComparison.CurrentCultureIgnoreCase)))
                {
                    // Errore - utente non trovato
                    throw new Exception("Error user not found: {0}");
                }

                // Recupero lo user FomMonitoringCore.SqlServer db
                var userToDelete = _fomMonitoringEntities.Set<FomMonitoringCore.SqlServer.Users>().First(w => string.Equals(w.Username, user.Username, StringComparison.CurrentCultureIgnoreCase));

                // disabilito l'utente
                userToDelete.Enabled = false;
                userToDelete.DeletedDate = DateTime.Now;

                // Persisto le modifiche nel DB
                _fomMonitoringEntities.SaveChanges();
                
            }
            catch (Exception ex)
            {
                throw new Exception($"Error during the delete of the user: {ex.Message}");
            }
        }

    }
}
