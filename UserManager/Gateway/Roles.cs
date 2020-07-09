using System;
using System.Collections.Generic;
using System.Linq;
using FomMonitoringCore.SqlServer;
using UserManager.Service;
using UserManager.Service.Concrete;

namespace UserManager.Gateway
{
    public class Roles : IRoles
    {
        private readonly IFomMonitoringEntities _fomMonitoringEntities;
        private readonly ILoggedUserServices _loggedUserServices;

        public Roles(IFomMonitoringEntities fomMonitoringEntities, ILoggedUserServices loggedUserServices)
        {
            _fomMonitoringEntities = fomMonitoringEntities;
            _loggedUserServices = loggedUserServices;
        }

        public List<FomMonitoringCore.SqlServer.Roles> GetListOfNonDeletedRoles()
        {
            try
            {
                return _fomMonitoringEntities.Set<FomMonitoringCore.SqlServer.Roles>()
                    .Where(r => r.DeletedBy == null && r.DeletedDate == null).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception($"Errore: {ex.Message} (Error #111009)");
            }
        }

        public void DeleteRole(Guid roleId)
        {
            try
            {
                // Verifico che il ruolo esista nel db
                if (!_fomMonitoringEntities.Set<FomMonitoringCore.SqlServer.Roles>().Any(r => r.ID == roleId))
                    // Errore - ruolo non trovato
                    throw new Exception("Error user not found: {0}");

                // Recupero lo Ruolo dal db
                var rolesToDelete = _fomMonitoringEntities.Set<FomMonitoringCore.SqlServer.Roles>()
                    .FirstOrDefault(r => r.ID == roleId);

                if (rolesToDelete != null)
                    _fomMonitoringEntities.Set<FomMonitoringCore.SqlServer.Roles>().Remove(rolesToDelete);


                // Persisto le modifiche nel DB
                _fomMonitoringEntities.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error during the delete of the user: {ex.Message}");
            }
        }

        public bool CheckIfRoleAlreadyExist(string name)
        {
            try
            {
                return _fomMonitoringEntities.Set<FomMonitoringCore.SqlServer.Roles>()
                    .Any(r => r.Name.ToLower() == name.ToLower());
            }
            catch (Exception ex)
            {
                throw new Exception($"Error: {ex.Message}");
            }
        }


        /// <summary>
        ///     Modifica alcune proprietà dell'utente
        /// </summary>
        /// <param name="role"></param>
        public void ModifyRole(FomMonitoringCore.SqlServer.Roles role)
        {
            try
            {
                // Verifico che l'utente esista nel db
                if (!_fomMonitoringEntities.Set<FomMonitoringCore.SqlServer.Roles>().Any(r =>
                        string.Equals(r.Name, role.Name, StringComparison.CurrentCultureIgnoreCase)))
                    // Errore - utente non trovato
                    throw new Exception("Error user not found: {0}");

                // Recupero lo user dal db
                var roleToModify = _fomMonitoringEntities.Set<FomMonitoringCore.SqlServer.Roles>()
                    .FirstOrDefault(r => r.Name.ToLower() == role.Name.ToLower());

                // Aggiorno l'utente con le nuove informazioni (solo alcuni campi vengono aggiornati)
                if (roleToModify != null)
                {
                    roleToModify.Name = role.Name;
                    roleToModify.Description = role.Description;
                    roleToModify.Enabled = role.Enabled;
                }

                // Persisto le modifiche nel DB
                _fomMonitoringEntities.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error during the update of the user: {ex.Message}");
            }
        }

        public FomMonitoringCore.SqlServer.Roles GetRoles(Guid rolesId)
        {
            try
            {
                return _fomMonitoringEntities.Set<FomMonitoringCore.SqlServer.Roles>()
                    .FirstOrDefault(roles => roles.ID == rolesId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Errore nel cambio password: {ex.Message} (Error #111009)");
            }
        }

        public List<FomMonitoringCore.SqlServer.Roles> GetRoles()
        {
            try
            {
                return _fomMonitoringEntities.Set<FomMonitoringCore.SqlServer.Roles>().ToList();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error while getting user roles: {ex.Message}");
            }
        }


        public List<FomMonitoringCore.SqlServer.Roles> GetRolesFromUserId(Guid userId)
        {
            try
            {
                return _fomMonitoringEntities.Set<Roles_Users>().Where(r => r.UserID == userId).Select(r => r.Roles)
                    .ToList();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error while getting user roles: {ex.Message}");
            }
        }

        public void DeleteRoles(Guid userId)
        {
            try
            {
                // Verifico che Ruolo esista nel db
                if (!_fomMonitoringEntities.Set<FomMonitoringCore.SqlServer.Roles>().Any(w => w.ID == userId))
                    // Errore - utente non trovato
                    throw new Exception("Error user not found: {0}");

                // Recupero lo user dal db
                var rolesToDelete = _fomMonitoringEntities.Set<FomMonitoringCore.SqlServer.Roles>()
                    .FirstOrDefault(w => w.ID == userId);

                // disabilito l'utente
                if (rolesToDelete != null)
                {
                    rolesToDelete.Enabled = false;
                    rolesToDelete.DeletedBy = _loggedUserServices.GetLoggedUserID();
                    rolesToDelete.DeletedDate = DateTime.Now;
                }

                // Persisto le modifiche nel DB
                _fomMonitoringEntities.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error during the delete of the user: {ex.Message}");
            }
        }

        public void AddRolesToUser(Guid userId, List<Guid> rolesId)
        {
            try
            {
                // Controllo che l'utente ed i ruoli passati esistano nel database
                var user = _fomMonitoringEntities.Set<FomMonitoringCore.SqlServer.Users>()
                    .FirstOrDefault(w => w.ID == userId);
                var roles = _fomMonitoringEntities.Set<FomMonitoringCore.SqlServer.Roles>()
                    .Where(w => rolesId.Contains(w.ID)).ToList();

                if (user == null)
                    return;

                // Recupero la lista dei ruoli legati all'utente
                var userRoles = _fomMonitoringEntities.Set<Roles_Users>()
                    .Where(rl => rl.UserID == userId).Select(n => n.Roles).ToList();


                // Verifico se il ruolo che voglio aggiungere non è già presente tra i ruoli legati all'utente
                // e in caso negativo lo aggiungo
                foreach (var role in roles)
                {
                    if (userRoles.Contains(role))
                        continue;

                    var newUserRole = _fomMonitoringEntities.Set<Roles_Users>().Create();
                    newUserRole.ID = Guid.NewGuid();
                    newUserRole.UserID = userId;
                    newUserRole.RoleID = role.ID;
                    _fomMonitoringEntities.Set<Roles_Users>().Add(newUserRole);
                }

                _fomMonitoringEntities.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error during the role assignment to the user: {ex.Message}");
            }
        }

        public void DeleteRolesFromUser(Guid userId)
        {
            try
            {
                if (!_fomMonitoringEntities.Set<Roles_Users>().Any(r => r.UserID == userId))
                    throw new Exception("Error not found: {0}");


                var rolesToDelete = _fomMonitoringEntities.Set<Roles_Users>().Where(r => r.UserID == userId).ToList();

                foreach (var role in rolesToDelete) _fomMonitoringEntities.Set<Roles_Users>().Remove(role);

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