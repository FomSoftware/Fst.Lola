using System;
using System.Collections.Generic;
using System.Linq;
using FomMonitoringCore.SqlServer;
using UserManager.Framework.Common;
using UserManager.Service;

namespace UserManager.Gateway
{
    public class Users : IUsers
    {
        private readonly IFomMonitoringEntities _fomMonitoringEntities;
        private readonly ILoggedUserServices _loggedUserServices;

        public Users(IFomMonitoringEntities fomMonitoringEntities, ILoggedUserServices loggedUserServices)
        {
            _fomMonitoringEntities = fomMonitoringEntities;
            _loggedUserServices = loggedUserServices;
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
        public FomMonitoringCore.SqlServer.Users GetUserById(Guid Id)
        {
            try
            {
                return _fomMonitoringEntities.Set<FomMonitoringCore.SqlServer.Users>().Find(Id);
            }
            catch (Exception ex)
            {
                throw new Exception($"Errore: {ex.Message} (Error #111009)");
            }
        }



    }
}
