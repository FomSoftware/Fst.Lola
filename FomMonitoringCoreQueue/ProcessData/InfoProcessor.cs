using System;
using FomMonitoringCore.DAL;
using System.Linq;
using Autofac;
using FomMonitoringCore.Service;
using FomMonitoringCoreQueue.Dto;

namespace FomMonitoringCoreQueue.ProcessData
{
    public class InfoProcessor : IProcessor<Info>
    {
        private readonly ILifetimeScope _parentScope;


        public InfoProcessor(ILifetimeScope parentScope)
        {
            _parentScope = parentScope;
        }
        public bool ProcessData(Info data)
        {
            try
            {

                using (var threadLifetime = _parentScope.BeginLifetimeScope())
                using (var _context = threadLifetime.Resolve<IFomMonitoringEntities>())
                using (var machineService = threadLifetime.Resolve<IMachineService>())
                {

                    foreach (var machineData in data.InfoMachine)
                    {
                        var machineActual = _context.Set<Machine>()
                                                .FirstOrDefault(m => m.Serial == machineData.MachineSerial) ?? _context.Set<Machine>().Add(new Machine()
                                            {
                                                Serial = machineData.MachineSerial
                                            });


                        machineData.LoginDate =
                            machineData.LoginDate.HasValue && machineData.LoginDate.Value.Year < 1900
                                ? null
                                : machineData.LoginDate;


                        //campi cablati che non arrivano più dal json
                        machineData.StateProductivityGreenThreshold = 63.0;
                        machineData.StateProductivityYellowThreshold = 40.0;
                        machineData.PiecesProductivityGreenThreshold = 10.0;
                        machineData.PiecesProductivityYellowThreshold = 25.0;
                        machineData.BarsProductivityGreenThreshold = 90.0;
                        machineData.BarsProductivityYellowThreshold = 50.0;
                        machineData.OverfeedGreenThreshold = 85.0;
                        machineData.OverfeedYellowThreshold = 50.0;
                        machineData.Shift1StartHour = 0;
                        machineData.Shift1StartMinute = 0;
                        machineData.Shift2StartHour = 8;
                        machineData.Shift2StartMinute = 0;
                        machineData.Shift3StartHour = 16;
                        machineData.Shift3StartMinute = 0;



                        machineActual.Description = machineData.MachineDescription;
                        machineActual.FirmwareVersion = machineData.FirmwareVersion;
                        machineActual.InstallationDate = machineData.InstallationDate;
                        machineActual.KeyId = machineData.KeyId;
                        machineActual.LoginDate = machineData.LoginDate;
                        machineActual.MachineModelId =
                            machineService.GetMachineModelIdByModelCode(machineData.MachineCode);
                        machineActual.MachineTypeId =
                            machineService.GetMachineTypeIdByModelCode(machineData.MachineCode);
                        machineActual.NextMaintenanceService = machineData.NextMaintenanceService;
                        machineActual.PlcVersion = machineData.PlcVersion;
                        machineActual.Product = machineData.Product;
                        machineActual.ProductVersion = machineData.ProductVersion;
                        machineActual.Serial = machineData.MachineSerial;
                        machineActual.Shift1 = new TimeSpan(machineData.Shift1StartHour.Value,
                            machineData.Shift1StartMinute.Value, 0);
                        machineActual.Shift2 = new TimeSpan(machineData.Shift2StartHour.Value,
                            machineData.Shift2StartMinute.Value, 0);
                        machineActual.Shift3 = new TimeSpan(machineData.Shift3StartHour.Value,
                            machineData.Shift3StartMinute.Value, 0);
                        machineActual.BarsProductivityGreenThreshold = machineData.BarsProductivityGreenThreshold;
                        machineActual.BarsProductivityYellowThreshold = machineData.BarsProductivityYellowThreshold;
                        machineActual.OverfeedGreenThreshold = machineData.OverfeedGreenThreshold;
                        machineActual.OverfeedYellowThreshold = machineData.OverfeedYellowThreshold;
                        machineActual.PiecesProductivityGreenThreshold =
                            machineData.PiecesProductivityGreenThreshold;
                        machineActual.PiecesProductivityYellowThreshold =
                            machineData.PiecesProductivityYellowThreshold;
                        machineActual.StateProductivityGreenThreshold =
                            machineData.StateProductivityGreenThreshold;
                        machineActual.StateProductivityYellowThreshold =
                            machineData.StateProductivityYellowThreshold;
                        machineActual.UTC = machineData.UTC;
                    }



                    _context.SaveChanges();

                    return true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void Dispose()
        {
            _parentScope?.Dispose();
        }
    }
}