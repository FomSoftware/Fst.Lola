using System;
using System.Linq;
using FomMonitoringCore.DAL;
using FomMonitoringCore.Service;
using Mapster;
using State = FomMonitoringCoreQueue.Dto.State;

namespace FomMonitoringCoreQueue.ProcessData
{
    public class StateProcessor : IProcessor<State>
    {
        private readonly IFomMonitoringEntities _context;
        private readonly IMachineService _machineService;

        public StateProcessor(IFomMonitoringEntities context, IMachineService machineService)
        {
            _machineService = machineService;
            _context = context;
        }

        public bool ProcessData(State state)
        {
            try
            {
                _context.Refresh();
                var mac = _context.Set<Machine>().FirstOrDefault(m => m.Serial == state.InfoMachine.MachineSerial);
                if (mac == null)
                    return false;

                foreach (var var in state.StateMachine)
                {
                    var.StartTime = var.StartTime.HasValue && var.StartTime.Value.Year < 1900 ? null : var.StartTime;
                    var.EndTime = var.EndTime.HasValue && var.EndTime.Value.Year < 1900 ? null : var.EndTime;
                    if (var.StartTime != null && var.EndTime != null)
                        var.TimeSpanDuration = var.EndTime?.Subtract((DateTime)var.StartTime).Ticks;
                    else
                        var.TimeSpanDuration = null;

                    StateMachine stateM = var.BuildAdapter().AddParameters("machineService", _machineService).AddParameters("machineId", mac.Id).AdaptToType<StateMachine>();

                    if (var.EndTime > (mac.LastUpdate ?? new DateTime()))
                    {
                        _context.Set<StateMachine>().Add(stateM);
                    }
                }

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