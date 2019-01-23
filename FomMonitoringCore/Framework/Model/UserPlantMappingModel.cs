using System;

namespace FomMonitoringCore.Framework.Model
{
    public class UserPlantMappingModel
    {
        public int Id { get; set; }
        public Guid UserId { get; set; }
        public int? PlantId { get; set; }
        public PlantModel Plant { get; set; }
    }
}
