using System;
using System.Collections.Generic;
using System.Linq;

namespace UserManager.DAL.Gateway.Concrete
{
    public static class Roles
    {
        public static List<DAL.Roles> GetListOfNonDeletedRoles(UserManagerEntities userManagerEntities)
        {
            try
            {
                return (from roles in userManagerEntities.Roles
                        where roles.DeletedBy == null && roles.DeletedDate == null
                        select roles).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(String.Format("Errore: {0} (Error #111009)", ex.Message));
            }
        }

        public static void DeleteRole(Guid RoleID)
        {
            try
            {
                using (UserManagerEntities userManagerEntities = new UserManagerEntities())
                {
                    // Verifico che il ruolo esista nel db
                    if (!userManagerEntities.Roles.Where(r => r.ID == RoleID).Any())
                    {
                        // Errore - ruolo non trovato
                        throw new Exception("Error user not found: {0}");
                    }

                    // Recupero lo Ruolo dal db
                    DAL.Roles rolesToDelete = userManagerEntities.Roles.Where(r => r.ID == RoleID).FirstOrDefault();

                    userManagerEntities.Roles.Remove(rolesToDelete);


                    // Persisto le modifiche nel DB
                    userManagerEntities.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(String.Format("Error during the delete of the user: {0}", ex.Message));
            }
        }

        public static bool CheckIfRoleAlreadyExist(string Name)
        {
            try
            {
                using (UserManagerEntities userManagerEntities = new UserManagerEntities())
                {
                    return userManagerEntities.Roles.Where(r => r.Name.ToLower() == Name.ToLower()).Any();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(String.Format("Error: {0}", ex.Message));
            }
        }


        /// <summary>
        /// Modifica alcune proprietà dell'utente
        /// </summary>
        /// <param name="User"></param>
        public static void ModifyRole(DAL.Roles Role)
        {
            try
            {
                using (UserManagerEntities userManagerEntities = new UserManagerEntities())
                {
                    // Verifico che l'utente esista nel db
                    if (!userManagerEntities.Roles.Where(r => r.Name.ToLower() == Role.Name.ToLower()).Any())
                    {
                        // Errore - utente non trovato
                        throw new Exception("Error user not found: {0}");
                    }

                    // Recupero lo user dal db
                    DAL.Roles roleToModify = userManagerEntities.Roles.Where(r => r.Name.ToLower() == Role.Name.ToLower()).FirstOrDefault();

                    // Aggiorno l'utente con le nuove informazioni (solo alcuni campi vengono aggiornati)
                    roleToModify.Name = Role.Name;
                    roleToModify.Description = Role.Description;
                    roleToModify.Enabled = Role.Enabled;

                    // Persisto le modifiche nel DB
                    userManagerEntities.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(String.Format("Error during the update of the user: {0}", ex.Message));
            }
        }

        public static DAL.Roles GetRoles(Guid RolesId)
        {
            try
            {
                using (UserManagerEntities userManagerEntities = new UserManagerEntities())
                {
                    return (from roles in userManagerEntities.Roles
                            where roles.ID == RolesId
                            select roles).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(String.Format("Errore nel cambio password: {0} (Error #111009)", ex.Message));
            }
        }

        public static List<DAL.Roles> GetRoles()
        {
            try
            {
                using (UserManagerEntities userManagerEntities = new UserManagerEntities())
                {
                    return (from r in userManagerEntities.Roles
                            select r).ToList();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(String.Format("Error while getting user roles: {0}", ex.Message));
            }

        }

        public static List<DAL.Roles> GetRolesFromUserID(Guid UserID)
        {
            using (UserManagerEntities userManagerEntities = new UserManagerEntities())
            {
                return GetRolesFromUserID(userManagerEntities, UserID);
            }
        }

        public static List<DAL.Roles> GetRolesFromUserID(UserManagerEntities userManagerEntities, Guid UserID)
        {
            try
            {
                return (from r in userManagerEntities.Roles_Users
                        where r.UserID == UserID
                        select r.Roles).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(String.Format("Error while getting user roles: {0}", ex.Message));
            }
        }

        public static void DeleteRoles(Guid UserID)
        {
            try
            {
                using (UserManagerEntities userManagerEntities = new UserManagerEntities())
                {
                    // Verifico che Ruolo esista nel db
                    if (!userManagerEntities.Roles.Where(w => w.ID == UserID).Any())
                    {
                        // Errore - utente non trovato
                        throw new Exception("Error user not found: {0}");
                    }

                    // Recupero lo user dal db
                    DAL.Roles rolesToDelete = userManagerEntities.Roles.Where(w => w.ID == UserID).FirstOrDefault();

                    // disabilito l'utente
                    rolesToDelete.Enabled = false;
                    Service.ILoggedUserServices iLogUser = new Service.Concrete.LoggedUserServices();
                    rolesToDelete.DeletedBy = iLogUser.GetLoggedUserID();
                    rolesToDelete.DeletedDate = DateTime.Now;

                    // Persisto le modifiche nel DB
                    userManagerEntities.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(String.Format("Error during the delete of the user: {0}", ex.Message));
            }
        }

        public static void AddRolesToUser(Guid UserID, List<Guid> RolesID)
        {
            try
            {
                using (UserManagerEntities userManagerEntities = new UserManagerEntities())
                {
                    // Controllo che l'utente ed i ruoli passati esistano nel database
                    DAL.Users user = userManagerEntities.Users.Where(w => w.ID == UserID).FirstOrDefault();
                    List<DAL.Roles> roles = userManagerEntities.Roles.Where(w => RolesID.Contains(w.ID)).ToList();

                    if (user != null && roles != null)
                    {
                        // Recupero la lista dei ruoli legati all'utente
                        List<DAL.Roles> userRoles = (from rl in userManagerEntities.Roles_Users
                                                     where rl.UserID == UserID
                                                     select rl.Roles).ToList();

                        // Verifico se il ruolo che voglio aggiungere non è già presente tra i ruoli legati all'utente
                        // e in caso negativo lo aggiungo
                        foreach (DAL.Roles role in roles)
                        {
                            if (!userRoles.Contains(role))
                            {
                                DAL.Roles_Users newUserRole = userManagerEntities.Roles_Users.Create();
                                newUserRole.ID = Guid.NewGuid();
                                newUserRole.UserID = UserID;
                                newUserRole.RoleID = role.ID;
                                userManagerEntities.Roles_Users.Add(newUserRole);
                            }
                        }
                        userManagerEntities.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(String.Format("Error during the role assignment to the user: {0}", ex.Message));
            }
        }

        public static void DeleteRolesFromUser(Guid UserID)
        {
            try
            {
                using (UserManagerEntities userManagerEntities = new UserManagerEntities())
                {

                    if (!userManagerEntities.Roles_Users.Where(r => r.UserID == UserID).Any())
                    {

                        throw new Exception("Error not found: {0}");
                    }


                    List<DAL.Roles_Users> rolesToDelete = userManagerEntities.Roles_Users.Where(r => r.UserID == UserID).ToList();

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
                throw new Exception(String.Format("Error during the delete of the user: {0}", ex.Message));
            }
        }
    }
}
