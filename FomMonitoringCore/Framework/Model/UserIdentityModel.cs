using System.Collections.Generic;
using System.Security.Principal;

namespace FomMonitoringCore.Framework.Model
{
    public class UserIdentityModel : IPrincipal
    {
        string Username { get; set; }
        List<string> Roles { get; set; }
        public IIdentity Identity { get; private set; }

        public UserIdentityModel(string username, List<string> roles)
        {
            Username = username;
            Roles = roles;
            Identity = new GenericIdentity(username);
        }

        public bool IsInRole(string role)
        {
            return Roles.Contains(role);
        }
    }
}
