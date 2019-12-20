using FomMonitoringBLL.ViewModel;
using FomMonitoringCore.Framework.Common;
using FomMonitoringCore.Framework.Model;
using FomMonitoringCore.Service;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FomMonitoringBLL.ViewServices
{
    public class UserManagerViewService
    {
        public static UserManagerViewModel GetUsers(ContextModel context)
        {
            UserManagerViewModel userManager = new UserManagerViewModel();
            string usernameCustomer = null;

            if (context.User.Role != enRole.Administrator)
                usernameCustomer = context.User.Username;

            // users
            List<UserModel> usersModel = UserManagerService.GetUsers(usernameCustomer);
            userManager.users = usersModel.Select(s => new UserViewModel
            {
                ID = s.ID,
                Username = s.Username,
                FirstName = s.FirstName,
                LastName = s.LastName,
                Email = s.Email,
                CustomerName = s.CustomerName,
                RoleCode = (int)s.Role,
                RoleName = s.Role.GetDescription(),
                LanguageId = s.Language.ID,
                LanguageName = s.Language.Name,
                Enabled = s.Enabled,
                MachineSerials = s.Machines.Select(sel => sel.Serial).ToList()
            }).ToList();

            //roles
            List<RoleModel> rolesModel = UserManagerService.GetRoles();
            userManager.roles = rolesModel.Select(s => new UserRoleViewModel
            {
                Code = s.Code,
                Name = s.Name,
                status = s.status,
                enabled = s.enabled
            }).ToList();

            //customers
            userManager.customers = UserManagerService.GetCustomerNames();

            //machines
            if (context.User.Role == enRole.Customer)
            {
                List<MachineInfoModel> machinesModel = UserManagerService.GetCustomerMachines(usernameCustomer);
                userManager.machines = machinesModel.Select(s => new UserMachineViewModel { Id = s.Id, Serial = s.Serial }).ToList();
            }

            //languages
            List<UserManager.DAL.Languages> languagesModel = UserManagerService.GetLanguages();
            userManager.languages = languagesModel.Select(s => new UserLanguageViewModel { Id = s.ID, Name = s.Name }).ToList();

            return userManager;
        }

        public static UserManagerViewModel GetUser(string idUser)
        {
            UserManagerViewModel result = new UserManagerViewModel();

            Guid userId;
            if (Guid.TryParse(idUser, out userId))
            {
                //user
                UserModel userModel = UserManagerService.GetUser(userId);
                UserViewModel user = new UserViewModel();

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
                result.user = user;

                //machines for customer       
                result.machines = GetMachinesByCustomer(userModel.CustomerName);

                return result;
            }
            else
                return null;
        }

        public static bool CreateUser(UserViewModel userModel, ContextModel context)
        {
            try
            {
                UserModel user = new UserModel
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
                    Password = userModel.Password
                };

                if (context.User.Role != enRole.Customer)
                    user.LastDateUpdatePassword = DateTime.Now;

                Guid id = UserManagerService.CreateUser(user);
                // se sono customer invio la mail con la password del nuovo utente
                if(context.User.Role == enRole.Customer && context.User.Email != null)
                {
                    UserManagerService.SendPassword(context.User.Email, id);

                }
              
                return true;
            }
            catch (Exception ex)
            {
                if (ex is InvalidOperationException)
                    return false;

                throw ex;
            }
        }

        public static List<UserMachineViewModel> GetMachinesByCustomer(string name, bool includeExpired = true)
        {
            try
            {
                List<UserMachineViewModel> result = new List<UserMachineViewModel>();

                List<MachineInfoModel> machinesModel = UserManagerService.GetCustomerMachines(name);

                if (!includeExpired)
                    machinesModel = machinesModel
                        .Where(m => m.ExpirationDate == null || m.ExpirationDate > DateTime.UtcNow).ToList();

                if (machinesModel != null)
                    result = machinesModel
                        .Select(s => new UserMachineViewModel { Id = s.Id, Serial = s.Serial, MachineName = s.MachineName }).ToList();

                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static bool EditUser(UserViewModel userModel)
        {
            try
            {
                UserModel user = new UserModel
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
                    Enabled = userModel.Enabled
                };

                return UserManagerService.ModifyUser(user);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static bool ChangePassword(ContextModel context, ChangePasswordViewModel changePasswordInfo)
        {
            return UserManagerService.ChangePassword(context.User.ID, changePasswordInfo.OldPassword, changePasswordInfo.NewPassword);
        }

        public static bool ResetUserPassword(string userId)
        {
            Guid id = Guid.Parse(userId);
            return UserManagerService.ResetPassword(id);
        }

        public static bool DeleteUser(string userId)
        {
            Guid id = Guid.Parse(userId);
            return UserManagerService.DeleteUser(id);
        }
    }
}
