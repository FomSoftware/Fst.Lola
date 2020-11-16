using System;
using System.Linq;
using Autofac;
using FomMonitoringCore.Service;
using FomMonitoringCore.SqlServer;
using Mapster;
using State = FomMonitoringCore.Queue.Dto.State;
using StateMachine = FomMonitoringCore.SqlServer.StateMachine;

namespace FomMonitoringCore.Queue.ProcessData
{
    public class StateProcessor : IProcessor<Dto.State>
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

                    HistoricizingStates(_context, mac.Id);
                    _context.SaveChanges();
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public void HistoricizingStates(IFomMonitoringEntities context, int idMachine)
        {
            var maxHpDate = context.Set<HistoryState>().Where(historyState => historyState.MachineId == idMachine)
                .Max(a => a.Day);

            maxHpDate = maxHpDate?.Date ?? DateTime.MinValue;

            var historyStates = context.Set<StateMachine>()
                .Where(p => p.Day >= maxHpDate && p.MachineId == idMachine).ToList()
                .GroupBy(g => new { g.Day.Value.Date, g.Operator, g.StateId })
                .Select(n => new HistoryState
                {
                    Id = 0,
                    Day = n.Key.Date,
                    MachineId = idMachine,
                    Operator = n.Key.Operator,
                    Period = n.Key.Date.Year * 10000 + n.Key.Date.Month * 100 + n.Key.Date.Day,
                    TypeHistory = "d",
                    ElapsedTime = n.Sum(i => i.ElapsedTime),
                    StateId = n.Key.StateId,
                    OverfeedAvg = n.Key.StateId == 1 ? (n.Sum(i => i.ElapsedTime) > 0 ? n.Sum(i => i.Overfeed * i.ElapsedTime) / n.Sum(i => i.ElapsedTime) : 0) : null,


                }).ToList();

            var aggregato = historyStates.GroupBy(c =>new { c.Day, c.StateId} ).Select(n => new HistoryState
            {
                Id = 0,
                Day = n.Key.Day.Value,
                MachineId = idMachine,
                Operator = null,
                Period = n.Key.Day.Value.Year * 10000 + n.Key.Day.Value.Month * 100 + n.Key.Day.Value.Day,
                ElapsedTime = n.Sum(i => i.ElapsedTime),
                TypeHistory = "d",
                StateId = n.Key.StateId,
                OverfeedAvg = n.Key.StateId == 1 ? n.Sum(i => i.ElapsedTime)> 0 ? n.Sum(i => i.OverfeedAvg * i.ElapsedTime) / n.Sum(i => i.ElapsedTime) : 0 : null

            }).ToList();

            historyStates.AddRange(aggregato);


            foreach (var cur in historyStates)
            {
                var row = context.Set<HistoryState>().FirstOrDefault(historyState => historyState.MachineId == idMachine &&
                                                                           historyState.Day == cur.Day && historyState.StateId == cur.StateId &&
                                                                           (historyState.Operator == cur.Operator ||
                                                                            historyState.Operator == null && cur.Operator == null));
                if (row != null)
                {
                    row.ElapsedTime = cur.ElapsedTime;
                    row.OverfeedAvg = cur.OverfeedAvg;
                }
                else
                {
                    context.Set<HistoryState>().Add(cur);
                }
            }

            context.SaveChanges();
        }

        public void Dispose()
        {
            _parentScope?.Dispose();
        }
    }
}