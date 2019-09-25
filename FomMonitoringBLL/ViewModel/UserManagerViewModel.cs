using FomMonitoringCore.Framework.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FomMonitoringBLL.ViewModel
{
    public class UserManagerViewModel
    {
        public List<UserViewModel> users { get; set; }

        public UserViewModel user { get; set; }

        public List<UserRoleViewModel> roles { get; set; }

        public List<string> customers { get; set; }

        public List<UserLanguageViewModel> languages { get; set; }

        public List<UserMachineViewModel> machines { get; set; }
    }

    public class UserViewModel
    {
        public Guid ID { get; set; }

        public string Username { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string CustomerName { get; set; }

        public int RoleCode { get; set; }

        public string RoleName { get; set; }

        public Guid LanguageId { get; set; }

        public string LanguageName { get; set; }

        public List<UserMachineViewModel> Machines { get; set; }

        public List<string> MachineSerials { get; set; }

        public bool Enabled { get; set; }

        public string Password { get; set; }
    }

    public class UserMachineViewModel
    {
        public int Id { get; set; }
        public string Serial { get; set; }
        public string MachineName { get; set; }
    }

    public class UserRoleViewModel
    {
        public string Code { get; set; }

        public string Name { get; set; }

        public bool status { get; set; }

        public bool enabled { get; set; }

    }

    public class UserLanguageViewModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

    }

    public class ChangePasswordViewModel
    {
        public string IdUser { get; set; }
        public string OldPassword { get; set; }

        public string NewPassword { get; set; }
    }
}
