using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using FomMonitoringCore.DAL;
using FomMonitoringCore.Service;
using FomMonitoringCoreQueue.Dto;
using Mapster;

namespace FomMonitoringCoreQueue.ProcessData
{
    public class HistoryJobPieceBarProcessor : IProcessor<HistoryJobPieceBar>
    {
        private readonly ILifetimeScope _parentScope;


        public HistoryJobPieceBarProcessor(ILifetimeScope parentScope)
        {
            _parentScope = parentScope;
        }
        public bool ProcessData(HistoryJobPieceBar data)
        {
            if (data.HistoryJobMachine.Any() && data.BarMachine.Any() && data.PieceMachine.Any())
            {
                try
                {
                    using (var threadLifetime = _parentScope.BeginLifetimeScope())
                    {
                        var context = threadLifetime.Resolve<IFomMonitoringEntities>();
                        var barService = threadLifetime.Resolve<IBarService>();
                        var jobService = threadLifetime.Resolve<IJobService>();
                        var machineService = threadLifetime.Resolve<IMachineService>();
                        var serial = data.InfoMachine.FirstOrDefault()?.MachineSerial;
                        var mac = context.Set<Machine>().FirstOrDefault(m => m.Serial == serial);

                        if (mac == null)
                            return false;


                        var bar = data.BarMachine.BuildAdapter().AddParameters("machineId", mac.Id)
                            .AdaptToType<List<Bar>>();


                        foreach (var bb in bar)
                        {
                            var trovato = context.Set<Bar>().FirstOrDefault(m =>
                                m.Index == bb.Index && m.JobCode == bb.JobCode && bb.MachineId == m.MachineId);

                            if (trovato?.JobCode != null)
                                continue;
                            context.Set<Bar>().Add(bb);
                        }
                        context.SaveChanges();

                        var historyJob = data.HistoryJobMachine.BuildAdapter().AddParameters("machineId", mac.Id)
                            .AdaptToType<List<HistoryJob>>();

                        var minDateHistoryJob = historyJob.Any(a => a.Day.HasValue) ? historyJob.Where(w => w.Day.HasValue && w.MachineId == mac.Id).Select(s => s.Day).Min() : new DateTime();
                        var removeHistoryJob = context.Set<HistoryJob>().Where(w => w.Day.HasValue && w.Day.Value >= minDateHistoryJob && w.MachineId == mac.Id).ToList();
                        context.Set<HistoryJob>().RemoveRange(removeHistoryJob);
                        context.Set<HistoryJob>().AddRange(historyJob);

                        context.SaveChanges();

                        var piece = data.PieceMachine.BuildAdapter().AddParameters("machineId", mac.Id).AddParameters("barService", barService).AddParameters("jobService", jobService).AddParameters("machineService", machineService)
                            .AdaptToType<List<Piece>>();

                        
                        context.Set<Piece>().AddRange(piece);
                        mac.LastUpdate = DateTime.UtcNow;
                        context.SaveChanges();


                        context.usp_HistoricizingPieces(mac.Id);
                        context.usp_HistoricizingBars(mac.Id);

                        context.SaveChanges();

                        return true;



                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            return true;
        }

        public void Dispose()
        {
            _parentScope?.Dispose();
        }
    }
}