using System;

namespace FomMonitoringCore.Framework.Model
{
    public class RoleModel
    {
        public Guid ID { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        public bool status { get; set; }

        public bool enabled { get; set; }
    }
}
