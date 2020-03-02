using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace FomMonitoringCore.DataProcessing.Dto
{
    public class InfoMachine
    {

        public int Id { get; set; }
        public string KeyId { get; set; }
        public string MachineSerial { get; set; }
        public string MachineDescription { get; set; }
        public string Product { get; set; }
        public string ProductVersion { get; set; }
        public string FirmwareVersion { get; set; }
        public string PlcVersion { get; set; }
        public DateTime? LoginDate { get; set; }
        public string PlantName { get; set; }
        public string PlantAddress { get; set; }
        public DateTime? InstallationDate { get; set; }
        public DateTime? NextMaintenanceService { get; set; }
        public double? StateProductivityGreenThreshold { get; set; }
        public double? StateProductivityYellowThreshold { get; set; }
        public double? PiecesProductivityGreenThreshold { get; set; }
        public double? PiecesProductivityYellowThreshold { get; set; }
        public double? BarsProductivityGreenThreshold { get; set; }
        public double? BarsProductivityYellowThreshold { get; set; }
        public double? OverfeedGreenThreshold { get; set; }
        public double? OverfeedYellowThreshold { get; set; }
        public int? Shift1StartHour { get; set; }
        public int? Shift1StartMinute { get; set; }
        public int? Shift2StartHour { get; set; }
        public int? Shift2StartMinute { get; set; }
        public int? Shift3StartHour { get; set; }
        public int? Shift3StartMinute { get; set; }
        public double? UTC { get; set; }
        public int? MachineCode { get; set; }
        public DateTime? LastUpdate { get; set; }
    }
}