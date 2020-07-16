using System.Collections.Generic;
using FomMonitoringBLL.ViewModel;
using FomMonitoringCore.Framework.Model;

namespace FomMonitoringBLL.ViewServices
{
    public interface IUserManagerViewService
    {
        UserManagerViewModel GetUsers(ContextModel context);
        UserManagerViewModel GetUser(string idUser);
        bool CreateUser(UserViewModel userModel, ContextModel context);
        List<UserMachineViewModel> GetMachinesByCustomer(string name, bool includeExpired = true);
        bool EditUser(UserViewModel userModel, ContextModel context);
        bool ChangePassword(ContextModel context, ChangePasswordViewModel changePasswordInfo);
        bool ResetUserPassword(string userId);
        bool DeleteUser(string userId);
        void ChangeTimeZone(ContextModel context, string timezone);
    }
}