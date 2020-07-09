using System;
using System.Collections.Generic;
using System.Linq;
using FomMonitoringCore.SqlServer;

namespace UserManager.Gateway.Concrete
{
    public static class Roles
    {
        public static List<FomMonitoringCore.SqlServer.Roles> GetListOfNonDeletedRoles(FomMonitoringEntities userManagerEntities)
        {
            try
            {
                return (from roles in userManagerEntities.Roles
                        where roles.DeletedBy == null && roles.DeletedDate == null
                        select roles).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception($"Errore: {ex.Message} (Error #111009)");
            }
        }

        public static void DeleteRole(Guid roleId)
        {
            try
            {
                using (var userManagerEntities = new FomMonitoringEntities())
                {
                    // Verifico che il ruolo esista nel db
                    if (!userManagerEntities.Roles.Any(r => r.ID == roleId))
                    {
                        // Errore - ruolo non trovato
                        throw new Exception("Error user not found: {0}");
                    }

                    // Recupero lo Ruolo dal db
                    var rolesToDelete = userManagerEntities.Roles.FirstOrDefault(r => r.ID == roleId);

                    if (rolesToDelete != null) userManagerEntities.Roles.Remove(rolesToDelete);


                    // Persisto le modifiche nel DB
                    userManagerEntities.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error during the delete of the user: {ex.Message}");
            }
        }

        public static bool CheckIfRoleAlreadyExist(string name)
        {
            try
            {
                using (var userManagerEntities = new FomMonitoringEntities())
                {
                    return userManagerEntities.Roles.Any(r => r.Name.ToLower() == name.ToLower());
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error: {ex.Message}");
            }
        }


        /// <summary>
        /// Modifica alcune proprietà dell'utente
        /// </summary>
        /// <param name="role"></param>
        public static void ModifyRole(FomMonitoringCore.SqlServer.Roles role)
        {
            try
            {
                using (var userManagerEntities = new FomMonitoringEntities())
                {
                    // Verifico che l'utente esista nel db
                    if (!userManagerEntities.Roles.Any(r => string.Equals(r.Name, role.Name, StringComparison.CurrentCultureIgnoreCase)))
                    {
                        // Errore - utente non trovato
                        throw new Exception("Error user not found: {0}");
                    }

                    // Recupero lo user dal db
                    var roleToModify = userManagerEntities.Roles.FirstOrDefault(r => r.Name.ToLower() == role.Name.ToLower());

                    // Aggiorno l'utente con le nuove informazioni (solo alcuni campi vengono aggiornati)
                    if (roleToModify != null)
                    {
                        roleToModify.Name = role.Name;
                        roleToModify.Description = role.Description;
                        roleToModify.Enabled = role.Enabled;
                    }

                    // Persisto le modifiche nel DB
                    userManagerEntities.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error during the update of the user: {ex.Message}");
            }
        }

        public static FomMonitoringCore.SqlServer.Roles GetRoles(Guid rolesId)
        {
            try
            {
                using (var userManagerEntities = new FomMonitoringEntities())
                {
                    return (from roles in userManagerEntities.Roles
                            where roles.ID == rolesId
                            select roles).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Errore nel cambio password: {ex.Message} (Error #111009)");
            }
        }

        public static List<FomMonitoringCore.SqlServer.Roles> GetRoles()
        {
            try
            {
                using (var userManagerEntities = new FomMonitoringEntities())
                {
                    return (from r in userManagerEntities.Roles
                            select r).ToList();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error while getting user roles: {ex.Message}");
            }

        }

        public static List<FomMonitoringCore.SqlServer.Roles> GetRolesFromUserId(Guid userId)
        {
            using (var userManagerEntities = new FomMonitoringEntities())
            {
                return GetRolesFromUserId(userManagerEntities, userId);
            }
        }

        public static List<FomMonitoringCore.SqlServer.Roles> GetRolesFromUserId(FomMonitoringEntities userManagerEntities, Guid userId)
        {
            try
            {
                return (from r in userManagerEntities.Roles_Users
                        where r.UserID == userId
                        select r.Roles).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error while getting user roles: {ex.Message}");
            }
        }

        public static void DeleteRoles(Guid userId)
        {
            try
            {
                using (var userManagerEntities = new FomMonitoringEntities())
                {
                    // Verifico che Ruolo esista nel db
                    if (!userManagerEntities.Roles.Any(w => w.ID == userId))
                    {
                        // Errore - utente non trovato
                        throw new Exception("Error user not found: {0}");
                    }

                    // Recupero lo user dal db
                    var rolesToDelete = userManagerEntities.Roles.FirstOrDefault(w => w.ID == userId);

                    // disabilito l'utente
                    if (rolesToDelete != null)
                    {
                        rolesToDelete.Enabled = false;
                        Service.ILoggedUserServices iLogUser = new Service.Concrete.LoggedUserServices();
                        rolesToDelete.DeletedBy = iLogUser.GetLoggedUserID();
                        rolesToDelete.DeletedDate = DateTime.Now;
                    }

                    // Persisto le modifiche nel DB
                    userManagerEntities.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error during the delete of the user: {ex.Message}");
            }
        }

        public static void AddRolesToUser(Guid userId, List<Guid> rolesId)
        {
            try
            {
                using (var userManagerEntities = new FomMonitoringEntities())
                {
                    // Controllo che l'utente ed i ruoli passati esistano nel database
                    var user = userManagerEntities.Users.FirstOrDefault(w => w.ID == userId);
                    var roles = userManagerEntities.Roles.Where(w => rolesId.Contains(w.ID)).ToList();

                    if (user == null)
                        return;

                    // Recupero la lista dei ruoli legati all'utente
                    var userRoles = (from rl in userManagerEntities.Roles_Users
                        where rl.UserID == userId
                        select rl.Roles).ToList();

                    // Verifico se il ruolo che voglio aggiungere non è già presente tra i ruoli legati all'utente
                    // e in caso negativo lo aggiungo
                    foreach (var role in roles)
                    {
                        if (userRoles.Contains(role))
                            continue;

                        var newUserRole = userManagerEntities.Roles_Users.Create();
                        newUserRole.ID = Guid.NewGuid();
                        newUserRole.UserID = userId;
                        newUserRole.RoleID = role.ID;
                        userManagerEntities.Roles_Users.Add(newUserRole);
                    }
                    userManagerEntities.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error during the role assignment to the user: {ex.Message}");
            }
        }

        public static void DeleteRolesFromUser(Guid userId)
        {
            try
            {
                using (var userManagerEntities = new FomMonitoringEntities())
                {

                    if (!userManagerEntities.Roles_Users.Any(r => r.UserID == userId))
                    {

                        throw new Exception("Error not found: {0}");
                    }


                    var rolesToDelete = userManagerEntities.Roles_Users.Where(r => r.UserID == userId).ToList();

                    foreach (var role in rolesToDelete)
                    {
                        userManagerEntities.Roles_Users.Remove(role);
                    }

                    // Persisto le modifiche nel DB
                    userManagerEntities.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error during the delete of the user: {ex.Message}");
            }
        }
    }
}
