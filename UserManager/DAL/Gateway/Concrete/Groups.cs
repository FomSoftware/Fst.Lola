using System;
using System.Collections.Generic;
using System.Linq;
using UserManager.Service.Concrete;

namespace UserManager.DAL.Gateway.Concrete
{
    public static class Groups
    {
        public static List<DAL.Groups> GetListOfNonDeletedGroups(UserManagerEntities userManagerEntities)
        {
            try
            {
                return (from groups in userManagerEntities.Groups
                        where groups.DeletedBy == null && groups.DeletedDate == null
                        select groups).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(String.Format("Errore: {0} (Error #111009)", ex.Message));
            }
        }

        public static void DeleteGroup(Guid GroupID)
        {
            try
            {
                using (UserManagerEntities userManagerEntities = new UserManagerEntities())
                {
                    // Verifico che il ruolo esista nel db
                    if (!userManagerEntities.Groups.Where(g => g.ID == GroupID).Any())
                    {
                        // Errore - ruolo non trovato
                        throw new Exception("Error user not found: {0}");
                    }

                    // Recupero lo Ruolo dal db
                    DAL.Groups groupsToDelete = userManagerEntities.Groups.Where(g => g.ID == GroupID).FirstOrDefault();

                    userManagerEntities.Groups.Remove(groupsToDelete);


                    // Persisto le modifiche nel DB
                    userManagerEntities.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(String.Format("Error during the delete of the Group: {0}", ex.Message));
            }
        }

        public static bool CheckIfGroupAlreadyExist(string Name)
        {
            try
            {
                using (UserManagerEntities userManagerEntities = new UserManagerEntities())
                {
                    return userManagerEntities.Groups.Where(g => g.Name.ToLower() == Name.ToLower()).Any();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(String.Format("Error: {0}", ex.Message));
            }
        }

        /// <summary>
        /// Modifica alcune proprietà dell'group
        /// </summary>
        /// <param name="User"></param>
        public static void ModifyGroup(DAL.Groups Group)
        {
            try
            {
                using (UserManagerEntities userManagerEntities = new UserManagerEntities())
                {
                    // Verifico che il grupo esista nel db
                    if (!userManagerEntities.Groups.Where(g => g.Name.ToLower() == Group.Name.ToLower()).Any())
                    {
                        // Errore - group non trovato
                        throw new Exception("Error user not found: {0}");
                    }

                    // Recupero lo user dal db
                    DAL.Groups groupToModify = userManagerEntities.Groups.Where(g => g.Name.ToLower() == Group.Name.ToLower()).FirstOrDefault();

                    // Aggiorno l'utente con le nuove informazioni (solo alcuni campi vengono aggiornati)
                    groupToModify.Name = Group.Name;
                    groupToModify.Description = Group.Description;
                    groupToModify.Enabled = Group.Enabled;

                    // Persisto le modifiche nel DB
                    userManagerEntities.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(String.Format("Error during the update of the group: {0}", ex.Message));
            }
        }



        public static DAL.Groups GetGroups(Guid GroupsId)
        {
            try
            {
                using (UserManagerEntities userManagerEntities = new UserManagerEntities())
                {
                    return (from groups in userManagerEntities.Groups
                            where groups.ID == GroupsId
                            select groups).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(String.Format("Errore nel cambio password: {0} (Error #111009)", ex.Message));
            }
        }

        /**
         * restituisce un lista di tutti i gruppi
         */
        public static List<DAL.Groups> GetGroups()
        {
            try
            {
                using (UserManagerEntities userManagerEntities = new UserManagerEntities())
                {
                    return (from gr in userManagerEntities.Groups
                            select gr).ToList();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(String.Format("Error while getting user groups: {0}", ex.Message));
            }

        }

        public static List<DAL.Groups> GetGroupsFromUserID(Guid UserID)
        {
            try
            {
                using (UserManagerEntities userManagerEntities = new UserManagerEntities())
                {
                    return (from gr in userManagerEntities.Groups_Users
                            where gr.UserID == UserID
                            select gr.Groups).ToList();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(String.Format("Error while getting user groups: {0}", ex.Message));
            }
        }

        public static void AddGroupsToUser(Guid UserID, List<Guid> GroupsID)
        {
            try
            {
                using (UserManagerEntities userManagerEntities = new UserManagerEntities())
                {
                    // Controllo che l'utente ed il gruppo passati esistano nel database
                    DAL.Users user = userManagerEntities.Users.Where(w => w.ID == UserID).FirstOrDefault();
                    List<DAL.Groups> groups = userManagerEntities.Groups.Where(w => GroupsID.Contains(w.ID)).ToList();

                    if (user != null && groups != null)
                    {
                        // Recupero la lista dei gruppi legati all'utente
                        List<DAL.Groups> userGroups = (from gr in userManagerEntities.Groups_Users
                                                       where gr.UserID == UserID
                                                       select gr.Groups).ToList();

                        // Verifico se il gruppo che voglio aggiungere non è già presente tra i gruppi legati all'utente
                        // e in caso negativo lo aggiungo
                        foreach (DAL.Groups group in groups)
                        {
                            if (!userGroups.Contains(group))
                            {
                                DAL.Groups_Users newUserGroup = userManagerEntities.Groups_Users.Create();
                                newUserGroup.ID = Guid.NewGuid();
                                newUserGroup.UserID = UserID;
                                newUserGroup.GroupID = group.ID;
                                userManagerEntities.Groups_Users.Add(newUserGroup);
                            }
                        }
                        userManagerEntities.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(String.Format("Error during the group assignment to the user: {0}", ex.Message));
            }
        }

        public static void DeleteGroupsFromUser(Guid UserID)
        {
            try
            {
                using (UserManagerEntities userManagerEntities = new UserManagerEntities())
                {

                    if (!userManagerEntities.Groups_Users.Where(r => r.UserID == UserID).Any())
                    {

                        throw new Exception("Error not found: {0}");
                    }


                    List<DAL.Groups_Users> groupsToDelete = userManagerEntities.Groups_Users.Where(r => r.UserID == UserID).ToList();

                    foreach (var group in groupsToDelete)
                    {
                        userManagerEntities.Groups_Users.Remove(group);
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
