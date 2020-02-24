using System;
using FomMonitoringCore.DAL;
using System.Linq;
using FomMonitoringCore.Service;
using FomMonitoringCoreQueue.Dto;

namespace FomMonitoringCoreQueue.ProcessData
{
    public class InfoProcessor : IProcessor<Info>
    {
        private readonly IFomMonitoringEntities _context;
        private readonly IMachineService _machineService;

        public InfoProcessor(IFomMonitoringEntities context, IMachineService machineService)
        { 
            _context = context;
            _machineService = machineService;
        }
        public bool ProcessData(Info data)
        {
            try
            {
                _context.Refresh();
                var machineActual = _context.Set<Machine>().FirstOrDefault(m => m.Serial == data.InfoMachine.MachineSerial);
                if (machineActual == null)
                    return false;


                data.InfoMachine.Id = 0;
                data.InfoMachine.LoginDate = data.InfoMachine.LoginDate.HasValue && data.InfoMachine.LoginDate.Value.Year < 1900 ? null : data.InfoMachine.LoginDate;


                //campi cablati che non arrivano più dal json
                data.InfoMachine.StateProductivityGreenThreshold = 63.0;
                data.InfoMachine.StateProductivityYellowThreshold = 40.0;
                data.InfoMachine.PiecesProductivityGreenThreshold = 10.0;
                data.InfoMachine.PiecesProductivityYellowThreshold = 25.0;
                data.InfoMachine.BarsProductivityGreenThreshold = 90.0;
                data.InfoMachine.BarsProductivityYellowThreshold = 50.0;
                data.InfoMachine.OverfeedGreenThreshold = 85.0;
                data.InfoMachine.OverfeedYellowThreshold = 50.0;
                data.InfoMachine.Shift1StartHour = 0;
                data.InfoMachine.Shift1StartMinute = 0;
                data.InfoMachine.Shift2StartHour = 8;
                data.InfoMachine.Shift2StartMinute = 0;
                data.InfoMachine.Shift3StartHour = 16;
                data.InfoMachine.Shift3StartMinute = 0;



                machineActual.Description = data.InfoMachine.MachineDescription;
                machineActual.FirmwareVersion = data.InfoMachine.FirmwareVersion;
                machineActual.InstallationDate = data.InfoMachine.InstallationDate;
                machineActual.KeyId = data.InfoMachine.KeyId;
                machineActual.LoginDate = data.InfoMachine.LoginDate;
                machineActual.MachineModelId = _machineService.GetMachineModelIdByModelCode(data.InfoMachine.MachineCode);
                machineActual.MachineTypeId = _machineService.GetMachineTypeIdByModelCode(data.InfoMachine.MachineCode);
                machineActual.NextMaintenanceService = data.InfoMachine.NextMaintenanceService;
                machineActual.PlcVersion = data.InfoMachine.PlcVersion;
                machineActual.Product = data.InfoMachine.Product;
                machineActual.ProductVersion = data.InfoMachine.ProductVersion;
                machineActual.Serial = data.InfoMachine.MachineSerial;
                machineActual.Shift1 = new TimeSpan(data.InfoMachine.Shift1StartHour.Value, data.InfoMachine.Shift1StartMinute.Value, 0);
                machineActual.Shift2 = new TimeSpan(data.InfoMachine.Shift2StartHour.Value, data.InfoMachine.Shift2StartMinute.Value, 0);
                machineActual.Shift3 = new TimeSpan(data.InfoMachine.Shift3StartHour.Value, data.InfoMachine.Shift3StartMinute.Value, 0);
                machineActual.BarsProductivityGreenThreshold = data.InfoMachine.BarsProductivityGreenThreshold;
                machineActual.BarsProductivityYellowThreshold = data.InfoMachine.BarsProductivityYellowThreshold;
                machineActual.LastUpdate = data.InfoMachine.LastUpdate ?? DateTime.UtcNow;
                machineActual.OverfeedGreenThreshold = data.InfoMachine.OverfeedGreenThreshold;
                machineActual.OverfeedYellowThreshold = data.InfoMachine.OverfeedYellowThreshold;
                machineActual.PiecesProductivityGreenThreshold =
                    data.InfoMachine.PiecesProductivityGreenThreshold;
                machineActual.PiecesProductivityYellowThreshold =
                    data.InfoMachine.PiecesProductivityYellowThreshold;
                machineActual.StateProductivityGreenThreshold =
                    data.InfoMachine.StateProductivityGreenThreshold;
                machineActual.StateProductivityYellowThreshold =
                    data.InfoMachine.StateProductivityYellowThreshold;
                machineActual.UTC = data.InfoMachine.UTC;

                _context.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}