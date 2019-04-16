using FomMonitoringCore.DAL;
using FomMonitoringCore.Framework.Common;
using FomMonitoringCore.Framework.Model;
using FomMonitoringResources;
using Mapster;
using System;
using System.Collections.Generic;
using System.Linq;
using UserManager.DAL;
using UserManager.Service.Concrete;

namespace FomMonitoringCore.Service
{
    public class UserManagerService
    {
        /// <summary>
        /// Ritorna il singolo utente con le sue info
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static UserModel GetUser(Guid userId)
        {
            UserModel result = null;

            try
            {
                using (UserManagerEntities entUM = new UserManagerEntities())
                {
                    entUM.Configuration.LazyLoadingEnabled = false;

                    // Recupero l'utente dallo user manager
                    var user = entUM.Users
                        .Include("Roles_Users")
                        .Include("Roles_Users.Roles")
                        .Include("Languages").FirstOrDefault(f => f.ID == userId);
                    if (user == null) return result;

                    result = user.Adapt<Users, UserModel>();


                    // Recupero le sue macchine ed il customer associato
                    using (FST_FomMonitoringEntities ent = new FST_FomMonitoringEntities())
                    {
                        ent.Configuration.LazyLoadingEnabled = false;
                        var customerName = ent.UserCustomerMapping.Where(f => f.UserId == userId).Select(s => s.CustomerName).FirstOrDefault();
                        result.CustomerName = customerName;

                        var userMachines = ent.UserMachineMapping.Include("Machine").Where(w => w.UserId == userId).Select(s => s.Machine).ToList();
                        result.Machines = userMachines.Adapt<List<Machine>, List<MachineInfoModel>>();
                    }
                }
            }
            catch (Exception ex)
            {
                string errMessage = string.Format(ex.GetStringLog(), userId.ToString());
                LogService.WriteLog(errMessage, LogService.TypeLevel.Error, ex);
            }

            return result;
        }

        /// <summary>
        /// Ritorna il singolo utente con le sue info
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public static UserModel GetUser(string username)
        {
            UserModel result = null;

            try
            {
                using (UserManagerEntities entUM = new UserManagerEntities())
                {
                    var user = entUM.Users.FirstOrDefault(f => f.Username == username);
                    if (user == null) return result;
                    result = user.Adapt<Users, UserModel>();
                }
            }
            catch (Exception ex)
            {
                string errMessage = string.Format(ex.GetStringLog(), username);
                LogService.WriteLog(errMessage, LogService.TypeLevel.Error, ex);
            }

            return result;
        }

        /// <summary>
        /// Ritorna la lista degli utenti con le relative info
        /// </summary>
        /// <returns>Lista customer</returns>
        public static List<UserModel> GetUsers(string customerName)
        {
            List<UserModel> result = null;

            try
            {
                using (FST_FomMonitoringEntities ent = new FST_FomMonitoringEntities())
                {
                    ent.Configuration.LazyLoadingEnabled = false;

                    // Recupero la lista degli utenti associati al cliente
                    List<Guid> customerUsers = null;
                    if (customerName != null)
                    {
                        customerUsers = ent.UserCustomerMapping.Where(w => w.CustomerName == customerName).Select(s => s.UserId).Distinct().ToList();
                        if (customerUsers.Count == 0) return result;
                    }

                    using (UserManagerEntities entUM = new UserManagerEntities())
                    {
                        entUM.Configuration.LazyLoadingEnabled = false;

                        // Recupero i dati base degli utenti
                        var userQuery = entUM.Users
                            .Include("Roles_Users")
                            .Include("Roles_Users.Roles")
                            .Include("Languages").AsQueryable()
                            .Where(w => !w.Roles_Users.Any(a => a.Roles.IdRole == (int)enRole.Administrator
                                    || a.Roles.IdRole == (int)enRole.Customer || a.Roles.IdRole == (int)enRole.UserApi)).AsQueryable();

                        if (customerName != null) userQuery = userQuery.Where(w => customerUsers.Contains(w.ID));

                        var users = userQuery.ToList();
                        result = users.Adapt<List<Users>, List<UserModel>>();
                    }

                    // Associo il cliente all'utente
                    if (customerName != null)
                    {
                        result.ForEach(fe => fe.CustomerName = customerName);
                    }
                    else
                    {
                        var userCustomer = ent.UserCustomerMapping.ToList();
                        result.ForEach(fe => fe.CustomerName = userCustomer.Where(w => w.UserId == fe.ID).Select(s => s.CustomerName).FirstOrDefault());
                    }

                    // Recupero le macchine associate ad ogni utente
                    var machineQuery = ent.UserMachineMapping.Include("Machine").AsQueryable();
                    if (customerName != null) machineQuery = machineQuery.Where(w => customerUsers.Contains(w.UserId));

                    var userMachines = machineQuery.ToList();
                    result.ForEach(fe => fe.Machines = userMachines.Where(w => w.UserId == fe.ID).Select(s => s.Machine).ToList().Adapt<List<Machine>, List<MachineInfoModel>>());
                }
            }
            catch (Exception ex)
            {
                string errMessage = string.Format(ex.GetStringLog(), customerName);
                LogService.WriteLog(errMessage, LogService.TypeLevel.Error, ex);
            }

            return result;
        }

        /// <summary>
        /// Ritorna la lista dei clienti
        /// </summary>
        /// <returns>Lista customer</returns>
        public static List<string> GetCustomerNames()
        {
            List<string> result = null;

            try
            {
                using (FST_FomMonitoringEntities ent = new FST_FomMonitoringEntities())
                {
                    ent.Configuration.LazyLoadingEnabled = false;
                    result = ent.UserCustomerMapping.Select(s => s.CustomerName).Distinct().ToList();
                }
            }
            catch (Exception ex)
            {
                string errMessage = ex.GetStringLog();
                LogService.WriteLog(errMessage, LogService.TypeLevel.Error, ex);
            }

            return result;
        }

        /// <summary>
        /// Ritorna la lista dei ruoli
        /// </summary>
        /// <returns></returns>
        public static List<RoleModel> GetRoles()
        {
            List<RoleModel> result = null;

            try
            {
                using (UserManagerEntities entUM = new UserManagerEntities())
                {
                    entUM.Configuration.LazyLoadingEnabled = false;

                    var roles = entUM.Roles.ToList();
                    result = roles.Adapt<List<RoleModel>>();
                }
            }
            catch (Exception ex)
            {
                string errMessage = ex.GetStringLog();
                LogService.WriteLog(errMessage, LogService.TypeLevel.Error, ex);
            }

            return result;
        }

        /// <summary>
        /// Ritorna la lista delle macchine associate al cliente
        /// </summary>
        /// <param name="customerName"></param>
        /// <returns></returns>
        public static List<MachineInfoModel> GetCustomerMachines(string customerName)
        {
            List<MachineInfoModel> result = null;

            try
            {
                using (FST_FomMonitoringEntities ent = new FST_FomMonitoringEntities())
                {
                    ent.Configuration.LazyLoadingEnabled = false;

                    // Recupero la lista degli utenti associati al cliente
                    List<Guid> users = ent.UserCustomerMapping.Where(w => w.CustomerName == customerName).Select(s => s.UserId).Distinct().ToList();
                    if (users.Count == 0) return result;

                    // Recupero l'utente con ruolo cliente
                    Guid customerUser;
                    using (UserManagerEntities entUM = new UserManagerEntities())
                    {
                        entUM.Configuration.LazyLoadingEnabled = false;

                        // Recupero l'utente con ruolo cliente
                        customerUser = entUM.Users
                            .Include("Roles_Users")
                            .Include("Roles_Users.Roles")
                            .Where(w => users.Contains(w.ID) && w.Roles_Users.Any(a => a.Roles.IdRole == (int)enRole.Customer))
                            .Select(s => s.ID).FirstOrDefault();
                    }

                    // Recupero la lista delle macchine associate al cliente
                    var machines = ent.UserMachineMapping.Include("Machine").Where(w => w.UserId == customerUser).Select(s => s.Machine).ToList();
                    result = machines.Adapt<List<Machine>, List<MachineInfoModel>>();
                }
            }
            catch (Exception ex)
            {
                string errMessage = ex.GetStringLog();
                LogService.WriteLog(errMessage, LogService.TypeLevel.Error, ex);
            }

            return result;
        }

        /// <summary>
        /// Restituisce la lista delle lingue
        /// </summary>
        /// <returns></returns>
        public static List<Languages> GetLanguages()
        {
            List<Languages> result = null;

            try
            {
                using (UserManagerEntities entUM = new UserManagerEntities())
                {
                    entUM.Configuration.LazyLoadingEnabled = false;
                    result = entUM.Languages.ToList();
                }
            }
            catch (Exception ex)
            {
                string errMessage = ex.GetStringLog();
                LogService.WriteLog(errMessage, LogService.TypeLevel.Error, ex);
            }

            return result;
        }

        /// <summary>
        /// Aggiunge un nuovo utente
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public static Guid CreateUser(UserModel user)
        {
            try
            {
                using (UserManagerEntities entUM = new UserManagerEntities())
                {

                    LoginServices ls = new LoginServices();

                    var usernameUser = entUM.Users.Where(w => w.Username == user.Username).FirstOrDefault();
                    if (usernameUser != null)
                        throw new InvalidOperationException(Resource.UsernameExists);

                    // creo il nuovo utente
                    var defaultPassword = ApplicationSettingService.GetWebConfigKey("DefaultPassword");
                    var addUser = new Users
                    {
                        ID = Guid.NewGuid(),                       
                        Username = user.Username,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Email = user.Email,
                        LanguageID = user.Language?.ID,
                        Enabled = user.Enabled,
                        Password = ls.EncryptPassword(defaultPassword)
                    };

                    // user language
                    if (addUser.LanguageID == null)
                    {
                        var defaultLanguage = ApplicationSettingService.GetWebConfigKey("DefaultLanguage");
                        addUser.LanguageID = Guid.Parse(defaultLanguage);
                    }

                    entUM.Users.Add(addUser);
                    entUM.SaveChanges();

                    // Add user role
                    var newRole = entUM.Roles.FirstOrDefault(f => f.IdRole == (int)user.Role);
                    if (newRole != null)
                        addUser.Roles_Users.Add(new Roles_Users() { ID = Guid.NewGuid(), UserID = addUser.ID, RoleID = newRole.ID });

                    using (FST_FomMonitoringEntities ent = new FST_FomMonitoringEntities())
                    {
                        // Add user customer
                        //fabiana: lo user appena inserito è quello del db UM per cui non ha valorizzato il campo CustomerName ma solo il campo username
                        ent.UserCustomerMapping.Add(new UserCustomerMapping() { UserId = addUser.ID, CustomerName = user.Username });

                        // Add user machines
                        //fabiana: commentato perchè qui user.Machines è sempre null
                       /* foreach (var machine in user.Machines)
                            ent.UserMachineMapping.Add(new UserMachineMapping() { UserId = addUser.ID, MachineId = machine.Id, ExpirationDate = DateTime.Now });
                            */
                        ent.SaveChanges();
                    }

                    entUM.SaveChanges();
                    return addUser.ID;
                }
            }
            //catch (System.InvalidOperationException ex)
            //{
            //    throw ex;
            //}
            catch (Exception ex)
            {
                string errMessage = ex.GetStringLog();
                LogService.WriteLog(errMessage, LogService.TypeLevel.Error, ex);
                throw ex;
            }
        }

        /// <summary>
        /// Modifica un utente esistente
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public static bool ModifyUser(UserModel user)
        {
            try
            {
                using (UserManagerEntities entUM = new UserManagerEntities())
                {
                    // recupero l'utente
                    var updUser = entUM.Users
                        .Include("Roles_Users")
                        .Include("Roles_Users.Roles")
                        .Include("Languages")
                        .SingleOrDefault(s => s.ID == user.ID);

                    if (updUser == null)
                        return false; // not found
                    updUser.Username = user.Username;
                    updUser.FirstName = user.FirstName;
                    updUser.LastName = user.LastName;
                    updUser.Email = user.Email;
                    updUser.LanguageID = user.Language.ID;
                    updUser.Enabled = user.Enabled;

                    if (updUser.Roles_Users.Count > 0 && ((enRole)updUser.Roles_Users.First().Roles.IdRole) != user.Role)
                    {
                        // Elimino il ruolo assocciato
                        var roloToDelete = entUM.Roles_Users.Where(w => w.UserID == updUser.ID).FirstOrDefault();
                        entUM.Roles_Users.Remove(roloToDelete);
                        updUser.Roles_Users.Clear();
                        //entUM.SaveChanges();

                        var newRole = entUM.Roles.FirstOrDefault(f => f.IdRole == (int)user.Role);
                        if (newRole != null)
                            updUser.Roles_Users.Add(new Roles_Users() { ID = Guid.NewGuid(), UserID = user.ID, RoleID = newRole.ID });
                        //updUser.Roles_Users.FirstOrDefault(f => f.RoleID == (int)user.Role).RoleID = newRole.ID;

                    }

                    using (FST_FomMonitoringEntities ent = new FST_FomMonitoringEntities())
                    {
                        // Update user customer
                        var userCustomer = ent.UserCustomerMapping.FirstOrDefault(f => f.UserId == user.ID);
                        if (userCustomer != null)
                            userCustomer.CustomerName = user.CustomerName;
                        else
                            ent.UserCustomerMapping.Add(new UserCustomerMapping() { UserId = user.ID, CustomerName = user.CustomerName });

                        // Update user machines
                        if (user.Machines.Count > 0)
                        {
                            var machineIds = user.Machines.Select(s => s.Id).ToList();
                            var userMachines = ent.UserMachineMapping.Where(w => w.UserId == user.ID).ToList();

                            // Elimino le macchine non più associate
                            var machineToDelete = userMachines.Where(w => !machineIds.Contains(w.MachineId)).ToList();
                            if (machineToDelete.Count > 0) ent.UserMachineMapping.RemoveRange(machineToDelete);

                            // Aggiungo le macchine mancanti
                            foreach (var machine in user.Machines)
                            {
                                if (!userMachines.Any(a => a.MachineId == machine.Id))
                                    ent.UserMachineMapping.Add(new UserMachineMapping() { UserId = user.ID, MachineId = machine.Id, ExpirationDate = DateTime.Now });
                            }
                        }

                        ent.SaveChanges();
                    }

                    entUM.SaveChanges();
                    return true;
                }
            }
            catch (Exception ex)
            {
                string errMessage = ex.GetStringLog();
                LogService.WriteLog(errMessage, LogService.TypeLevel.Error, ex);
                throw ex;
            }
        }

        /// <summary>
        /// Modifica la password dell'utente
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="oldPassword"></param>
        /// <param name="newPassword"></param>
        /// <returns></returns>
        public static bool ChangePassword(Guid userId, string oldPassword, string newPassword)
        {
            try
            {
                string message = null;

                UserServices userServices = new UserServices();
                LoginServices ls = new LoginServices();
                return userServices.ChangePassword(userId, ls.EncryptPassword(newPassword), ls.EncryptPassword(oldPassword), out message);
            }
            catch (Exception ex)
            {
                string errMessage = ex.GetStringLog();
                LogService.WriteLog(errMessage, LogService.TypeLevel.Error, ex);
                throw ex;
            }
        }

        /// <summary>
        /// Modifica la password dell'utente
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static bool ResetPassword(Guid userId)
        {
            try
            {
                using (UserManagerEntities entUM = new UserManagerEntities())
                {
                    // recupero l'utente
                    var updUser = entUM.Users.SingleOrDefault(s => s.ID == userId);

                    if (updUser == null)
                        return false; // not found

                    LoginServices ls = new LoginServices();

                    // resetto password utente
                    var defaultPassword = ApplicationSettingService.GetWebConfigKey("DefaultPassword");
                    updUser.Password = ls.EncryptPassword(defaultPassword);

                    entUM.SaveChanges();
                    return true;
                }
            }
            catch (Exception ex)
            {
                string errMessage = ex.GetStringLog();
                LogService.WriteLog(errMessage, LogService.TypeLevel.Error, ex);
                throw ex;
            }
        }

        /// <summary>
        /// elimina l'utente
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static bool DeleteUser(Guid userId)
        {
            try
            {
                using (UserManagerEntities entUM = new UserManagerEntities())
                {
                    // recupero l'utente dal customers mapping
                    var user = entUM.Users.Where(s => s.ID == userId).SingleOrDefault();

                    if (user == null)
                        return false; // not found
                    entUM.Users.Remove(user);

                    var userRole = entUM.Roles_Users.Where(s => s.UserID == userId).SingleOrDefault();

                    if (userRole == null)
                        return false; // not found
                    entUM.Roles_Users.Remove(userRole);

                    entUM.SaveChanges();
                }

                using (FST_FomMonitoringEntities ent = new FST_FomMonitoringEntities())
                {
                    // recupero l'utente dal customers mapping
                   UserCustomerMapping userCustomer = ent.UserCustomerMapping.Where(s => s.UserId == userId).SingleOrDefault();

                    if (userCustomer == null)
                        return false; // not found
                    ent.UserCustomerMapping.Remove(userCustomer);

                    List<UserMachineMapping> userMachines = ent.UserMachineMapping.Where(s => s.UserId == userId).ToList();

                    if (userMachines.Count() == 0)
                        return false; // not found
                    ent.UserMachineMapping.RemoveRange(userMachines);

                    ent.SaveChanges();
                }

                return true;
            }
            catch (Exception ex)
            {
                string errMessage = ex.GetStringLog();
                LogService.WriteLog(errMessage, LogService.TypeLevel.Error, ex);
                throw ex;
            }
        }

    }
}
