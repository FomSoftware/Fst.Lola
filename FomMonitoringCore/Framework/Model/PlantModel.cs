using System;
using System.Collections.Generic;

namespace FomMonitoringCore.Framework.Model
{
    public class PlantModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string CustomerName { get; set; }
        public Guid UserId { get; set; }
        public DateTime LastDateUpdate { get; set; }
        public List<MachineInfoModel> Machines { get; set; }
    }
}
