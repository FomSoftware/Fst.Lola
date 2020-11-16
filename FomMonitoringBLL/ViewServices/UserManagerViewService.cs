using FomMonitoringBLL.ViewModel;
using FomMonitoringCore.Framework.Common;
using FomMonitoringCore.Framework.Model;
using FomMonitoringCore.Service;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FomMonitoringBLL.ViewServices
{
    public class UserManagerViewService : IUserManagerViewService
    {
        private readonly IUserManagerService _userManagerService;

        public UserManagerViewService(IUserManagerService userManagerService)
        {
            _userManagerService = userManagerService;
        }

        public UserManagerViewModel GetUsers(ContextModel context)
        {
            var userManager = new UserManagerViewModel();
            string usernameCustomer = null;

            if (context.User.Role != enRole.Administrator)
                usernameCustomer = context.User.Username;

            // users
            var usersModel = _userManagerService.GetUsers(usernameCustomer);
            userManager.users = usersModel.Select(s => new UserViewModel
            {
                ID = s.ID,
                Username = s.Username,
                FirstName = s.FirstName,
                LastName = s.LastName,
                Email = s.Email,
                CustomerName = s.CustomerName,
                RoleCode = (int)s.Role,
                RoleName = LocalizationService.GetResource(s.Role.GetDescription()),
                LanguageId = s.Language.ID,
                LanguageName = s.Language.Name,
                Enabled = s.Enabled,
                MachineSerials = s.Machines.Select(sel => sel.Serial).ToList(),
                TimeZone = s.TimeZone
            }).ToList();

            //roles
            var rolesModel = _userManagerService.GetRoles();
            userManager.roles = rolesModel.Select(s => new UserRoleViewModel
            {
                Code = s.Code,
                Name = LocalizationService.GetResource(s.Name),
                status = s.status,
                enabled = s.enabled
            }).ToList();

            //customers
            userManager.customers = _userManagerService.GetCustomerNames();

            //machines
            if (context.User.Role == enRole.Customer)
            {
                var machinesModel = _userManagerService.GetCustomerMachines(usernameCustomer);
                userManager.machines = machinesModel.Select(s => new UserMachineViewModel { Id = s.Id, Serial = s.Serial, MachineName = s.MachineName }).ToList();
            }

            //languages
            var languagesModel = _userManagerService.GetLanguages();
            userManager.languages = languagesModel.Select(s => new UserLanguageViewModel { Id = s.ID, Name = s.Name }).ToList();

            return userManager;
        }

        public UserManagerViewModel GetUser(string idUser)
        {
            var result = new UserManagerViewModel();

            Guid userId;
            if (Guid.TryParse(idUser, out userId))
            {
                //user
                var userModel = _userManagerService.GetUser(userId);
                var user = new UserViewModel();

                user.ID = userModel.ID;
                user.Username = userModel.Username;
                user.FirstName = userModel.FirstName;
                user.LastName = userModel.LastName;
                user.Email = userModel.Email;
                user.CustomerName = userModel.CustomerName;
                user.RoleCode = (int)userModel.Role;
                user.RoleName = userModel.Role.GetDescription();
                user.LanguageId = userModel.Language.ID;
                user.LanguageName = userModel.Language.Name;
                user.Enabled = userModel.Enabled;
                user.Machines = userModel.Machines.Select(s => new UserMachineViewModel { Id = s.Id, Serial = s.Serial }).ToList();
                user.TimeZone = userModel.TimeZone;
                result.user = user;

                //machines for customer       
                result.machines = GetMachinesByCustomer(userModel.CustomerName);

                return result;
            }
            else
                return null;
        }

        public bool CreateUser(UserViewModel userModel, ContextModel context)
        {
            try
            {
                var user = new UserModel
                {
                    ID = userModel.ID,
                    Username = userModel.Username,
                    LastName = userModel.LastName,
                    FirstName = userModel.FirstName,
                    Email = userModel.Email,
                    CustomerName = userModel.CustomerName,
                    Role = (enRole)userModel.RoleCode,
                    Language = new LanguagesModel { ID = userModel.LanguageId },
                    Machines = userModel.Machines.Select(s => new MachineInfoModel { Id = s.Id }).ToList(),
                    Enabled = userModel.Enabled,
                    Password = userModel.Password,
                    TimeZone = userModel.TimeZone
                };

                if (context.User.Role != enRole.Customer)
                    user.LastDateUpdatePassword = DateTime.Now;

                var id = _userManagerService.CreateUser(user);
                // se sono customer invio la mail con la password del nuovo utente
                //rileggo le info dello user
                var u = _userManagerService.GetUser(context.User.ID);
                if (context.User.Role == enRole.Customer && u.Email != null)
                {
                    _userManagerService.SendPassword(u.Email, id, "CreateUserEmailSubject", "CreateUserEmailBody");
                }
              
                return true;
            }
            catch (Exception ex)
            {
                if (ex is InvalidOperationException)
                    return false;

                throw;
            }
        }

        public List<UserMachineViewModel> GetMachinesByCustomer(string name, bool includeExpired = true)
        {
            var result = new List<UserMachineViewModel>();

            var machinesModel = _userManagerService.GetCustomerMachines(name);

            if (!includeExpired)
                machinesModel = machinesModel
                    .Where(m => m.ExpirationDate == null || m.ExpirationDate > DateTime.UtcNow).ToList();

            if (machinesModel != null)
                result = machinesModel
                    .Select(s => new UserMachineViewModel { Id = s.Id, Serial = s.Serial, MachineName = s.MachineName }).ToList();

            return result;
        }

        public bool EditUser(UserViewModel userModel, ContextModel context)
        {
                var user = new UserModel
                {
                    ID = userModel.ID,
                    Username = userModel.Username,
                    LastName = userModel.LastName,
                    FirstName = userModel.FirstName,
                    Email = userModel.Email,
                    CustomerName = userModel.CustomerName,
                    Role = (enRole)userModel.RoleCode,
                    Language = new LanguagesModel { ID = userModel.LanguageId },
                    Machines = userModel.Machines.Select(s => new MachineInfoModel { Id = s.Id }).ToList(),
                    Enabled = userModel.Enabled,
                    TimeZone = userModel.TimeZone
                };

                string email = null;

                var usrdb = _userManagerService.GetUser(context.User.ID);
                if (context.User.Role == enRole.Customer && context.User.Email != null)
                {
                    email = usrdb.Email;
                    user.Password = userModel.Password;
                }

                return _userManagerService.ModifyUser(user, email);


        }

        public bool ChangePassword(ContextModel context, ChangePasswordViewModel changePasswordInfo)
        {
            return _userManagerService.ChangePassword(context.User.ID, changePasswordInfo.OldPassword, changePasswordInfo.NewPassword);
        }

        public bool ResetUserPassword(string userId)
        {
            var id = Guid.Parse(userId);
            return _userManagerService.ResetPassword(id);
        }

        public bool DeleteUser(string userId)
        {
            var id = Guid.Parse(userId);
            return _userManagerService.DeleteUser(id);
        }

        public void ChangeTimeZone(ContextModel context, string timezone)
        {
            _userManagerService.ChangeTimeZone(context.User.ID, timezone);
        }
    }
}
