﻿using FomMonitoringCore.Framework.Common;
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

        public string Email { get; set; }

        public string CustomerName { get; set; }

        public enRole Role { get; set; }

        public LanguagesModel Language { get; set; }

        public List<MachineInfoModel> Machines { get; set; }

        public bool Enabled { get; set; }
    }
}
