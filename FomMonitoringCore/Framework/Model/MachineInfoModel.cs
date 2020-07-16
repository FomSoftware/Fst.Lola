namespace FomMonitoringCore.Framework.Model
{
    using System;

    public class MachineInfoModel
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public string FirmwareVersion { get; set; }
        public DateTime? InstallationDate { get; set; }
        public string KeyId { get; set; }
        public DateTime? LastUpdate { get; set; }
        public DateTime? LoginDate { get; set; }
        public int? MachineModelId { get; set; }
        public MachineModelModel Model { get; set; }
        public int? MachineTypeId { get; set; }
        public MachineTypeModel Type { get; set; }
        public DateTime? NextMaintenanceService { get; set; }
        public int PlantId { get; set; }
        public PlantModel Plant { get; set; }
        public string PlcVersion { get; set; }
        public string Product { get; set; }
        public string ProductVersion { get; set; }
        public string Serial { get; set; }
        public TimeSpan? Shift1 { get; set; }
        public TimeSpan? Shift2 { get; set; }
        public TimeSpan? Shift3 { get; set; }
        public double? BarsProductivityGreenThreshold { get; set; }
        public double? BarsProductivityYellowThreshold { get; set; }
        public double? OverfeedGreenThreshold { get; set; }
        public double? OverfeedYellowThreshold { get; set; }
        public double? PiecesProductivityGreenThreshold { get; set; }
        public double? PiecesProductivityYellowThreshold { get; set; }
        public double? StateProductivityGreenThreshold { get; set; }
        public double? StateProductivityYellowThreshold { get; set; }
        public double? UTC { get; set; }
        public string MachineName { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public DateTime? ActivationDate { get; set; }
        public string TimeZone { get; set; }
    }
}
