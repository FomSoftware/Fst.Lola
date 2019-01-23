using System;

namespace FomMonitoringCore.Framework.Model
{
    public class UserMachineMappingModel
    {
        public int Id { get; set; }
        public Guid UserId { get; set; }
        public int? MachineId { get; set; }
        public MachineInfoModel Machine { get; set; }
    }
}
