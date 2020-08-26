using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using Autofac;
using FomMonitoringCore.Queue.Dto;
using FomMonitoringCore.Service;
using FomMonitoringCore.SqlServer;
using Mapster;

namespace FomMonitoringCore.Queue.ProcessData
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
                        var mac = context.Set<Machine>().AsNoTracking().FirstOrDefault(m => m.Serial == serial);

                        if (mac == null)
                            return false;


                        var bar = data.BarMachine.BuildAdapter().AddParameters("machineId", mac.Id)
                            .AdaptToType<List<Bar>>();


                        foreach (var bb in bar)
                        {
                            if (!(bb.StartTime > DateTime.MinValue))
                                continue;

                            var trovato = context.Set<Bar>().FirstOrDefault(m =>
                                m.Index == bb.Index && m.JobCode == bb.JobCode && bb.MachineId == m.MachineId);

                            if (trovato?.JobCode != null)
                                continue;

                            context.Set<Bar>().Add(bb);

                        }
                        context.SaveChanges();

                        var historyJob = data.HistoryJobMachine.BuildAdapter().AddParameters("machineId", mac.Id)
                            .AdaptToType<List<HistoryJob>>();


                        foreach (var jj in historyJob)
                        {
                            if (!(jj.Day > DateTime.MinValue))
                                continue;

                            var trovato = context.Set<HistoryJob>().FirstOrDefault(m =>
                                m.Code == jj.Code && m.Day == jj.Day && jj.MachineId == m.MachineId);

                            if (trovato != null)
                                continue;

                            context.Set<HistoryJob>().Add(jj);
                        }
                    

                        context.SaveChanges();

                        var piece = data.PieceMachine.BuildAdapter().AddParameters("machineId", mac.Id).AddParameters("barService", barService).AddParameters("jobService", jobService).AddParameters("machineService", machineService)
                            .AdaptToType<List<Piece>>();

                        foreach (var pp in piece)
                        {
                            if (!(pp.Day > DateTime.MinValue))
                                continue;

                            var trovato = context.Set<Piece>().FirstOrDefault(m =>
                                pp.MachineId == m.MachineId && pp.BarId == m.BarId && pp.Day == m.Day &&
                                pp.JobId == m.JobId);

                            if (trovato != null)
                                continue;

                            context.Set<Piece>().Add(pp);
                        }

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