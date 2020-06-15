using System;
using System.Diagnostics;
using System.Linq;
using Autofac;
using FomMonitoringCore.DAL;
using FomMonitoringCore.DAL_SQLite;
using FomMonitoringCore.Service;
using Mapster;
using State = FomMonitoringCoreQueue.Dto.State;

namespace FomMonitoringCoreQueue.ProcessData
{
    public class StateProcessor : IProcessor<State>
    {
        private readonly ILifetimeScope _parentScope;


        public StateProcessor(ILifetimeScope parentScope)
        {
            _parentScope = parentScope;
        }
        public bool ProcessData(State state)
        {
            try
            {
                using (var threadLifetime = _parentScope.BeginLifetimeScope())
                using (var _context = threadLifetime.Resolve<IFomMonitoringEntities>())
                using (var machineService = threadLifetime.Resolve<IMachineService>())
                {


                    var serial = state.InfoMachine.First().MachineSerial;
                    var mac = _context.Set<Machine>().AsNoTracking().FirstOrDefault(m => m.Serial == serial);
                    if (mac == null)
                        return false;

                    var maxDate = _context.Set<StateMachine>()
                        .Where(m => m.MachineId == mac.Id && m.EndTime != null).Max(et => et.EndTime) ?? new DateTime();

                    foreach (var var in state.StateMachine)
                    {
                        var.StartTime = var.StartTime.HasValue && var.StartTime.Value.Year < 1900
                            ? null
                            : var.StartTime;
                        var.EndTime = var.EndTime.HasValue && var.EndTime.Value.Year < 1900 ? null : var.EndTime;
                        if (var.StartTime != null && var.EndTime != null)
                            var.TimeSpanDuration = var.EndTime?.Subtract((DateTime) var.StartTime).Ticks;
                        else
                            var.TimeSpanDuration = null;

                        var stateM = var.BuildAdapter().AddParameters("machineService", machineService)
                            .AddParameters("machineId", mac.Id).AdaptToType<StateMachine>();

                        if(stateM.EndTime > maxDate)
                            _context.Set<StateMachine>().Add(stateM);
 

                    }

                    _context.SaveChanges();
                    _context.usp_HistoricizingStates(mac.Id);
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