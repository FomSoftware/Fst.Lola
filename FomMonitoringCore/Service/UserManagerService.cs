using FomMonitoringCore.Framework.Common;
using FomMonitoringCore.Framework.Model;
using FomMonitoringResources;
using Mapster;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Mail;
using FomMonitoringCore.SqlServer;
using UserManager.Service;

namespace FomMonitoringCore.Service
{
    public class UserManagerService : IUserManagerService
    {
        private readonly IFomMonitoringEntities _fomMonitoringEntities;
        private readonly IUserServices _userServices;
        private readonly ILoginServices _loginServices;

        public UserManagerService(IFomMonitoringEntities fomMonitoringEntities, IUserServices userServices, ILoginServices loginServices)
        {
            _fomMonitoringEntities = fomMonitoringEntities;
            _userServices = userServices;
            _loginServices = loginServices;
        }
        /// <summary>
        /// Ritorna il singolo utente con le sue info
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public UserModel GetUser(Guid userId)
        {
            UserModel result = null;

            try
            {
                    // Recupero l'utente dallo user manager
                    var user = _fomMonitoringEntities.Set<Users>()
                        .Include("Roles_Users")
                        .Include("Roles_Users.Roles")
                        .Include("Languages").FirstOrDefault(f => f.ID == userId);
                    if (user == null) return null;

                    result = user.Adapt<Users, UserModel>();

                    // Recupero le sue macchine ed il customer associato
                        var customerName = _fomMonitoringEntities.Set<UserCustomerMapping>().FirstOrDefault(f => f.UserId == userId)?.CustomerName;
                        result.CustomerName = customerName;

                        var userMachines = _fomMonitoringEntities.Set<UserMachineMapping>().Include("Machine").Where(w => w.UserId == userId).Select(s => s.Machine).ToList();
                        result.Machines = userMachines.Adapt<List<Machine>, List<MachineInfoModel>>();
                    
                
            }
            catch (Exception ex)
            {
                var errMessage = string.Format(ex.GetStringLog(), userId.ToString());
                LogService.WriteLog(errMessage, LogService.TypeLevel.Error, ex);
            }

            return result;
        }

        /// <summary>
        /// Ritorna il singolo utente con le sue info
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public UserModel GetUser(string username)
        {
            UserModel result = null;

            try
            {
                    var user = _fomMonitoringEntities.Set<Users>().FirstOrDefault(f => f.Username == username);
                    if (user == null) return null;
                    result = user.Adapt<Users, UserModel>();
                
            }
            catch (Exception ex)
            {
                var errMessage = string.Format(ex.GetStringLog(), username);
                LogService.WriteLog(errMessage, LogService.TypeLevel.Error, ex);
            }

            return result;
        }

        public List<UserModel> GetAllCustomers()
        {
            List<UserModel> result = null;
            try
            {
                    // Recupero i dati base degli utenti
                    var users = _fomMonitoringEntities.Set<Users>()
                        .Include("Roles_Users")
                        .Include("Roles_Users.Roles")
                        .Where(w => w.Roles_Users.Any(a => a.Roles.IdRole == (int)enRole.Customer)).ToList();
                
                    result = users.Adapt<List<Users>, List<UserModel>>();
            }
            catch (Exception ex)
            {
                var errMessage = string.Format(ex.GetStringLog());
                LogService.WriteLog(errMessage, LogService.TypeLevel.Error, ex);
            }

            return result;

        }

        public void RemoveUsers(List<UserModel> users)
        {
            if (users == null || users.Count == 0) return;
            try
            {         
                    var ids = users.Select(a => a.ID).ToList();
                    var rem = _fomMonitoringEntities.Set<Users>().Where(y => ids.Contains(y.ID)).ToList();

                    _fomMonitoringEntities.Set<Users>().RemoveRange(rem);

                    _fomMonitoringEntities.SaveChanges();
                
            }
            catch (Exception ex)
            {
                var errMessage = string.Format(ex.GetStringLog());
                LogService.WriteLog(errMessage, LogService.TypeLevel.Error, ex);
            }

        }


        /// <summary>
        /// Ritorna la lista degli utenti con le relative info
        /// </summary>
        /// <returns>Lista customer</returns>
        public List<UserModel> GetUsers(string customerName)
        {
            List<UserModel> result = null;
            try
            {
                // Recupero la lista degli utenti associati al cliente
                    List<Guid> customerUsers = null;
                    if (customerName != null)
                    {
                        customerUsers = _fomMonitoringEntities.Set<UserCustomerMapping>().Where(w => w.CustomerName == customerName).Select(s => s.UserId).Distinct().ToList();
                        if (customerUsers.Count == 0) return result;
                    }
                    
                        // Recupero i dati base degli utenti
                        var userQuery = _fomMonitoringEntities.Set<Users>()
                            .Include("Roles_Users")
                            .Include("Roles_Users.Roles")
                            .Include("Languages").AsQueryable()
                            .Where(w => !w.Roles_Users.Any(a => a.Roles.IdRole == (int)enRole.Administrator
                                    || a.Roles.IdRole == (int)enRole.Customer || a.Roles.IdRole == (int)enRole.UserApi)).AsQueryable();

                        if (customerName != null) userQuery = userQuery.Where(w => customerUsers.Contains(w.ID));

                        var users = userQuery.ToList();
                        result = users.Adapt<List<Users>, List<UserModel>>();
                    

                    // Associo il cliente all'utente
                    if (customerName != null)
                    {
                        result.ForEach(fe => fe.CustomerName = customerName);
                    }
                    else
                    {
                        var userCustomer = _fomMonitoringEntities.Set<UserCustomerMapping>().ToList();
                        result.ForEach(fe => fe.CustomerName = userCustomer.Where(w => w.UserId == fe.ID).Select(s => s.CustomerName).FirstOrDefault());
                    }

                    // Recupero le macchine associate ad ogni utente
                    var machineQuery = _fomMonitoringEntities.Set<UserMachineMapping>().Include("Machine").AsQueryable();
                    if (customerName != null) machineQuery = machineQuery.Where(w => customerUsers.Contains(w.UserId));

                    var userMachines = machineQuery.ToList();
                    result.ForEach(fe => fe.Machines = userMachines.Where(w => w.UserId == fe.ID).Select(s => s.Machine).ToList().Adapt<List<Machine>, List<MachineInfoModel>>());
                
            }
            catch (Exception ex)
            {
                var errMessage = string.Format(ex.GetStringLog(), customerName);
                LogService.WriteLog(errMessage, LogService.TypeLevel.Error, ex);
            }

            return result;
        }

        /// <summary>
        /// Ritorna la lista dei clienti
        /// </summary>
        /// <returns>Lista customer</returns>
        public List<string> GetCustomerNames()
        {
            List<string> result = null;

            try
            {
                    result = _fomMonitoringEntities.Set<UserCustomerMapping>().Select(s => s.CustomerName).Distinct().ToList();
                
            }
            catch (Exception ex)
            {
                var errMessage = ex.GetStringLog();
                LogService.WriteLog(errMessage, LogService.TypeLevel.Error, ex);
            }

            return result;
        }

        /// <summary>
        /// Ritorna la lista dei ruoli
        /// </summary>
        /// <returns></returns>
        public List<RoleModel> GetRoles()
        {
            List<RoleModel> result = null;

            try
            {
                    var roles = _fomMonitoringEntities.Set<Roles>().ToList();
                    result = roles.Adapt<List<RoleModel>>();
                
            }
            catch (Exception ex)
            {
                var errMessage = ex.GetStringLog();
                LogService.WriteLog(errMessage, LogService.TypeLevel.Error, ex);
            }

            return result;
        }

        /// <summary>
        /// Ritorna la lista delle macchine associate al cliente
        /// </summary>
        /// <param name="customerName"></param>
        /// <returns></returns>
        public List<MachineInfoModel> GetCustomerMachines(string customerName)
        {
            List<MachineInfoModel> result = null;

            try
            {

                    // Recupero la lista degli utenti associati al cliente
                    var users = _fomMonitoringEntities.Set<UserCustomerMapping>().Where(w => w.CustomerName == customerName).Select(s => s.UserId).Distinct().ToList();
                    if (users.Count == 0) return null;

                    // Recupero l'utente con ruolo cliente
                    Guid customerUser;
                        // Recupero l'utente con ruolo cliente
                        customerUser = _fomMonitoringEntities.Set<Users>()
                            .Include("Roles_Users")
                            .Include("Roles_Users.Roles")
                            .Where(w => users.Contains(w.ID) && w.Roles_Users.Any(a => a.Roles.IdRole == (int)enRole.Customer))
                            .Select(s => s.ID).FirstOrDefault();
                    

                    // Recupero la lista delle macchine associate al cliente
                    var machines = _fomMonitoringEntities.Set<UserMachineMapping>().Include("Machine").Where(w => w.UserId == customerUser).Select(s => s.Machine).ToList();
                    result = machines.Adapt<List<Machine>, List<MachineInfoModel>>();
                }
            catch (Exception ex)
            {
                var errMessage = ex.GetStringLog();
                LogService.WriteLog(errMessage, LogService.TypeLevel.Error, ex);
            }

            return result;
        }

        /// <summary>
        /// Restituisce la lista delle lingue
        /// </summary>
        /// <returns></returns>
        public List<Languages> GetLanguages()
        {
            List<Languages> result = null;

            try
            {
                    result = _fomMonitoringEntities.Set<Languages>().ToList();
                
            }
            catch (Exception ex)
            {
                var errMessage = ex.GetStringLog();
                LogService.WriteLog(errMessage, LogService.TypeLevel.Error, ex);
            }

            return result;
        }

        /// <summary>
        /// Aggiunge un nuovo utente
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public Guid CreateUser(UserModel user)
        {
            try
            {

                var usernameUser = _fomMonitoringEntities.Set<Users>().FirstOrDefault(w => w.Username == user.Username);
                if (usernameUser != null)
                    throw new InvalidOperationException(Resource.UsernameExists);

                // creo il nuovo utente
                var defaultPassword = ApplicationSettingService.GetWebConfigKey("DefaultPassword");
                if (user.Password != null)                    
                    defaultPassword = user.Password;                        
  
                var addUser = new Users
                {
                    ID = Guid.NewGuid(),                       
                    Username = user.Username,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    CompanyName = user.CompanyName,
                    Email = user.Email,
                    LanguageID = user.Language?.ID,
                    Enabled = user.Enabled,
                    Password = _loginServices.EncryptPassword(defaultPassword),
                    LastDateUpdatePassword = user.LastDateUpdatePassword,
                    TimeZone = user.TimeZone
                };

                // user language
                if (addUser.LanguageID == null)
                {
                    var defaultLanguage = ApplicationSettingService.GetWebConfigKey("DefaultLanguage");
                    addUser.LanguageID = Guid.Parse(defaultLanguage);
                }

                _fomMonitoringEntities.Set<Users>().Add(addUser);
                _fomMonitoringEntities.SaveChanges();

                // Add user role
                var newRole = _fomMonitoringEntities.Set<Roles>().FirstOrDefault(f => f.IdRole == (int)user.Role);
                if (newRole != null)
                    addUser.Roles_Users.Add(new Roles_Users() { ID = Guid.NewGuid(), UserID = addUser.ID, RoleID = newRole.ID });

                // Add user customer
                _fomMonitoringEntities.Set<UserCustomerMapping>().Add(new UserCustomerMapping() { UserId = addUser.ID, CustomerName = user.CustomerName });

                // Add user machines
                if (user.Machines != null && user.Machines.Any())
                {
                    foreach (var machine in user.Machines)
                        _fomMonitoringEntities.Set<UserMachineMapping>().Add(new UserMachineMapping() { UserId = addUser.ID, MachineId = machine.Id });
                }
                _fomMonitoringEntities.SaveChanges();

                
                return addUser.ID;
                
            }

            catch (Exception ex)
            {
                var errMessage = ex.GetStringLog();
                LogService.WriteLog(errMessage, LogService.TypeLevel.Error, ex);
                throw ex;
            }
        }

        public bool SendPassword(string email, Guid id, string keySubject, string keyObject)
        {
            var result = true;
            try
            {
                    var user = _fomMonitoringEntities.Set<Users>().AsNoTracking().FirstOrDefault(u => u.ID == id);
                    if (user == null) return false;
                    var subject = LocalizationService.GetResource(keySubject, new CultureInfo(user.Languages.DotNetCulture)) + " " + user.Username;
                    var ruolo = _fomMonitoringEntities.Set<Roles_Users>().FirstOrDefault()?.Roles.Description;
                    var idRuolo = user.Roles_Users.First().Roles.IdRole;

                    switch (idRuolo)
                    {
                        case 1:
                            ruolo = LocalizationService.GetResource($"Operator", new CultureInfo(user.Languages.DotNetCulture));
                            break;
                        case 2:
                            ruolo = LocalizationService.GetResource($"HeadWorkshop", new CultureInfo(user.Languages.DotNetCulture));
                            break;
                    } 
                    


                    var firstPart = LocalizationService.GetResource($"{keyObject}_FirstPart", new CultureInfo(user.Languages.DotNetCulture))
                        .Replace("[TIPO_USER]", ruolo);

                    var footer = LocalizationService.GetResource($"{keyObject}_Footer", new CultureInfo(user.Languages.DotNetCulture));

                    var usernameLabel = LocalizationService.GetResource($"{keyObject}_UsernameLabel", new CultureInfo(user.Languages.DotNetCulture));

                    var passwordLabel = LocalizationService.GetResource($"{keyObject}_PasswordLabel", new CultureInfo(user.Languages.DotNetCulture));

                    var lastPart = LocalizationService.GetResource($"{keyObject}_LastPart", new CultureInfo(user.Languages.DotNetCulture))
                        .Replace("[TIPO_USER]", ruolo);

                    var modelEmail = new Renderer.EmailChangedPasswordDto
                    {
                        FirstPart = firstPart,
                        Username = user.Username,
                        Password = _loginServices.DecryptPassword(user.Password),
                        FooterText = footer,
                        UsernameLabel = usernameLabel,
                        PasswordLabel = passwordLabel,
                        LastPart = lastPart
                    };


                    var body = Renderer.RazorViewToString.RenderRazorEmailChangedPasswordViewToString(modelEmail);
                    var message = new MailMessage(ApplicationSettingService.GetWebConfigKey("EmailFromAddress"),
                        email, subject, body)
                    {
                        IsBodyHtml = true
                    };
                    EmailSender.SendEmail(message);
                    return true;
                
            }
            catch (Exception ex)
            {
                var errMessage = ex.GetStringLog();
                LogService.WriteLog(errMessage, LogService.TypeLevel.Error, ex);
                result = false;
            }

            return result;
        }

        /// <summary>
        /// Modifica un utente esistente
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public bool ModifyUser(UserModel user, string email)
        {
            try
            {
                
                    // recupero l'utente
                    var updUser = _fomMonitoringEntities.Set<Users>()
                        .Include("Roles_Users")
                        .Include("Roles_Users.Roles")
                        .Include("Languages")
                        .SingleOrDefault(s => s.ID == user.ID);

                    if (updUser == null)
                        return false; // not found
                    updUser.Username = user.Username;
                    updUser.FirstName = user.FirstName;
                    updUser.LastName = user.LastName;
                    updUser.CompanyName = user.CompanyName;
                    updUser.Email = user.Email;
                    updUser.LanguageID = user.Language.ID;
                    updUser.Enabled = user.Enabled;
                    updUser.TimeZone = user.TimeZone;

                    var modifiedPsw = false;
                    if (user.Password != null && !string.IsNullOrEmpty(user.Password.Trim()))
                    {
                        //ha riempito i campi password mando sempre la mail
                        modifiedPsw = true;
                        var newPsw = _loginServices.EncryptPassword(user.Password);
                        if (updUser.Password != newPsw)
                        {
                            updUser.Password = newPsw;
                            updUser.LastDateUpdatePassword = null;
                        }
                    }

                    var rolesIdRole = updUser.Roles_Users.First().Roles.IdRole;
                    if (rolesIdRole != null && (updUser.Roles_Users.Count > 0 && ((enRole)rolesIdRole) != user.Role))
                    {
                        // Elimino il ruolo assocciato
                        var roloToDelete = _fomMonitoringEntities.Set<Roles_Users>().FirstOrDefault(w => w.UserID == updUser.ID);
                        if(roloToDelete != null)
                            _fomMonitoringEntities.Set<Roles_Users>().Remove(roloToDelete);

                        updUser.Roles_Users.Clear();
                        //entUM.SaveChanges();

                        var newRole = _fomMonitoringEntities.Set<Roles>().FirstOrDefault(f => f.IdRole == (int)user.Role);
                        if (newRole != null)
                            updUser.Roles_Users.Add(new Roles_Users() { ID = Guid.NewGuid(), UserID = user.ID, RoleID = newRole.ID });
                        //updUser.Roles_Users.FirstOrDefault(f => f.RoleID == (int)user.Role).RoleID = newRole.ID;

                    }
                    
                    // Update user customer
                    var userCustomer = _fomMonitoringEntities.Set<UserCustomerMapping>().FirstOrDefault(f => f.UserId == user.ID);
                    if (userCustomer != null)
                        userCustomer.CustomerName = user.CustomerName;
                    else
                        _fomMonitoringEntities.Set<UserCustomerMapping>().Add(new UserCustomerMapping() { UserId = user.ID, CustomerName = user.CustomerName });

                    // Update user machines
                    if (user.Machines.Count > 0)
                    {
                        var machineIds = user.Machines.Select(s => s.Id).ToList();
                        var userMachines = _fomMonitoringEntities.Set<UserMachineMapping>().Where(w => w.UserId == user.ID).ToList();

                        // Elimino le macchine non più associate
                        var machineToDelete = userMachines.Where(w => !machineIds.Contains(w.MachineId)).ToList();
                        if (machineToDelete.Count > 0) _fomMonitoringEntities.Set<UserMachineMapping>().RemoveRange(machineToDelete);

                        // Aggiungo le macchine mancanti
                        foreach (var machine in user.Machines)
                        {
                            if (userMachines.All(a => a.MachineId != machine.Id))
                                _fomMonitoringEntities.Set<UserMachineMapping>().Add(new UserMachineMapping() { UserId = user.ID, MachineId = machine.Id});
                        }
                    }

                    _fomMonitoringEntities.SaveChanges();

                    if (modifiedPsw && email != null)
                        SendPassword(email, updUser.ID, "ModifyUserEmailSubject", "ModifyUserEmailBody");
         
                    return true;
                
            }
            catch (Exception ex)
            {
                var errMessage = ex.GetStringLog();
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
        public bool ChangePassword(Guid userId, string oldPassword, string newPassword)
        {
            try
            {
                string message = null;
                
                return _userServices.ChangePassword(userId, _loginServices.EncryptPassword(newPassword), _loginServices.EncryptPassword(oldPassword), out message);
            }
            catch (Exception ex)
            {
                var errMessage = ex.GetStringLog();
                LogService.WriteLog(errMessage, LogService.TypeLevel.Error, ex);
                throw ex;
            }
        }

        /// <summary>
        /// Modifica la password dell'utente
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public bool ResetPassword(Guid userId)
        {
            try
            {
                
                    // recupero l'utente
                    var updUser = _fomMonitoringEntities.Set<Users>().SingleOrDefault(s => s.ID == userId);

                    if (updUser == null)
                        return false; // not found
                    

                    // resetto password utente
                    var defaultPassword = ApplicationSettingService.GetWebConfigKey("DefaultPassword");
                    updUser.Password = _loginServices.EncryptPassword(defaultPassword);

                    _fomMonitoringEntities.SaveChanges();
                    return true;
            }
            catch (Exception ex)
            {
                var errMessage = ex.GetStringLog();
                LogService.WriteLog(errMessage, LogService.TypeLevel.Error, ex);
                throw ex;
            }
        }

        /// <summary>
        /// elimina l'utente
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public bool DeleteUser(Guid userId)
        {
            try
            {

                // recupero l'utente dal customers mapping
                var userCustomer = _fomMonitoringEntities.Set<UserCustomerMapping>().SingleOrDefault(s => s.UserId == userId);

                if (userCustomer == null)
                    return false; // not found
                _fomMonitoringEntities.Set<UserCustomerMapping>().Remove(userCustomer);

                var userMachines = _fomMonitoringEntities.Set<UserMachineMapping>().Where(s => s.UserId == userId).ToList();

                if (!userMachines.Any())
                    return false; // not found
                _fomMonitoringEntities.Set<UserMachineMapping>().RemoveRange(userMachines);

                _fomMonitoringEntities.SaveChanges();

                // recupero l'utente dal customers mapping
                var user = _fomMonitoringEntities.Set<Users>().SingleOrDefault(s => s.ID == userId);

                    if (user == null)
                        return false; // not found
                    _fomMonitoringEntities.Set<Users>().Remove(user);

                    var userRole = _fomMonitoringEntities.Set<Roles_Users>().SingleOrDefault(s => s.UserID == userId);

                    if (userRole == null)
                    {
                        //se sono qui l'utente è sbagliato perchè non ha un ruolo, vado avanti e lo cancello...?
                        return false; // not found
                    }
                    _fomMonitoringEntities.Set<Roles_Users>().Remove(userRole);

                    _fomMonitoringEntities.SaveChanges();
                

                

                return true;
            }
            catch (Exception ex)
            {
                var errMessage = ex.GetStringLog();
                LogService.WriteLog(errMessage, LogService.TypeLevel.Error, ex);
                throw ex;
            }
        }

        public void ChangeTimeZone(Guid id, string timezone)
        {
            var user = _fomMonitoringEntities.Set<Users>().SingleOrDefault(s => s.ID == id);
            if (user != null)
                user.TimeZone = timezone;
            _fomMonitoringEntities.SaveChanges();
        }

        public void UpdateUserName(UserModel userModel)
        {
            var user = _fomMonitoringEntities.Set<Users>().SingleOrDefault(s => s.ID == userModel.ID);
            if (user != null)
            {
                user.FirstName = userModel.FirstName;
                user.LastName = userModel.LastName;
                user.CompanyName = userModel.CompanyName;
                _fomMonitoringEntities.SaveChanges();
            }
           
        }
    }
}
