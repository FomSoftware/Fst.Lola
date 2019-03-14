using FomMonitoringCore.Framework.Common;
using System;
using System.Collections.Generic;
using UserManager.DAL;

namespace FomMonitoringCore.Framework.Model
{
    public class UserModel
    {
        public Guid ID { get; set; }

        public string Username { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public List<enRole> Roles { get; set; }

        public Languages Language { get; set; }

    }
}
