using System;
using System.Linq;
using Autofac;
using FomMonitoringCore.Queue.Dto;
using FomMonitoringCore.Service;
using FomMonitoringCore.SqlServer;

namespace FomMonitoringCore.Queue.ProcessData
{
    public class InfoProcessor : IProcessor<Info>
    {
        private readonly ILifetimeScope _parentScope;


        public InfoProcessor(ILifetimeScope parentScope)
        {
            _parentScope = parentScope.BeginLifetimeScope();
        }
        public bool ProcessData(Info data)
        {
            
            using (var threadLifetime = _parentScope.BeginLifetimeScope())
            using (var context = threadLifetime.Resolve<IFomMonitoringEntities>())
            {

                    var machineService = threadLifetime.Resolve<IMachineService>();
                    foreach (var machineData in data.InfoMachine)
                    {
                        var machineActual = context.Set<Machine>()
                                                .FirstOrDefault(m => m.Serial == machineData.MachineSerial) ?? context.Set<Machine>().Add(new Machine
                                                {
                                                    Serial = machineData.MachineSerial,
                                                    LastUpdate = DateTime.UtcNow
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


                    context.SaveChanges();

                    return true;
            }
                
        }

        public void Dispose()
        {
            _parentScope?.Dispose();
        }
    }
}