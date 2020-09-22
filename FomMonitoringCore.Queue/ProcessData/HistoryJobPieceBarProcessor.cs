using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using Autofac;
using FomMonitoringCore.Framework.Model;
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
                            var giorno = jj.Day != null
                                ? ((jj.Day.Value.Year * 10000) + (jj.Day.Value.Month * 100) + (jj.Day.Value.Day))
                                : (int?) null;
                            var trovato = context.Set<HistoryJob>().FirstOrDefault(m =>
                                m.Code == jj.Code && m.Period == giorno && jj.MachineId == m.MachineId
                                && m.TotalPieces == jj.TotalPieces);

                            if (trovato != null)
                            {
                                //aggiorno i valori che sono incrementali
                                trovato.Day = jj.Day;
                                trovato.ElapsedTime = jj.ElapsedTime;
                                trovato.PiecesProduced = jj.PiecesProduced;
                                
                                context.Set<HistoryJob>().AddOrUpdate(trovato);
                                continue;
                            }
                            
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

                        //context.usp_HistoricizingPieces(mac.Id);
                        HistoricizingPieces(context, mac.Id);
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

        public void HistoricizingPieces(IFomMonitoringEntities context, int idMachine)
        {
            DateTime? maxHPDate = context.Set<HistoryPiece>().Where(hp => hp.MachineId == idMachine)
                .OrderByDescending(a => a.Day).FirstOrDefault()?.Day;
            if(maxHPDate == null)
                maxHPDate = DateTime.MinValue;
            else
            {
                maxHPDate = maxHPDate.Value.Date;
            }

            List<HistoryPiece> historyPieces = context.Set<Piece>()
                .Where(p => p.Day >= maxHPDate && p.MachineId == idMachine).ToList()
                .GroupBy(g => new {g.Day.Value.Date, g.Operator})
                .Select(n => new HistoryPiece
                {
                    Id =  0,
                    Day = n.Key.Date,
                    MachineId = idMachine,
                    Operator = n.Key.Operator,
                    CompletedCount = n.Count(i => i.IsRedone == false),
                    Period = n.Key.Date.Year * 10000 + n.Key.Date.Month * 100 + n.Key.Date.Day,
                    ElapsedTime = n.Sum(i => i.ElapsedTime),
                    ElapsedTimeCut = n.Sum(i => i.ElapsedTimeCut),
                    ElapsedTimeProducing = n.Sum(i => i.ElapsedTimeProducing),
                    ElapsedTimeTrim = n.Sum(i => i.ElapsedTimeTrim),
                    ElapsedTimeWorking = n.Sum(i => i.ElapsedTimeWorking),
                    PieceLengthSum = (int)n.Sum(i => i.IsRedone == true ? 0 : i.Length),
                    RedoneCount = n.Count(i => i.IsRedone == true),
                    TypeHistory = "d"
                }).ToList();

            var aggregato = historyPieces.GroupBy(c => c.Day).Select(n => new HistoryPiece
            {
                Id = 0,
                Day = n.Key.Value,
                MachineId = idMachine,
                Operator = null,
                Period = n.Key.Value.Year * 10000 + n.Key.Value.Month * 100 + n.Key.Value.Day,
                CompletedCount = n.Sum(i => i.CompletedCount),
                ElapsedTime = n.Sum(i => i.ElapsedTime),
                ElapsedTimeCut = n.Sum(i => i.ElapsedTimeCut),
                ElapsedTimeProducing = n.Sum(i => i.ElapsedTimeProducing),
                ElapsedTimeTrim = n.Sum(i => i.ElapsedTimeTrim),
                ElapsedTimeWorking = n.Sum(i => i.ElapsedTimeWorking),
                PieceLengthSum = n.Sum(i => i.PieceLengthSum),
                RedoneCount = n.Sum(i => i.RedoneCount),
                TypeHistory = "d"
            }).ToList();

            if(aggregato != null)
                historyPieces.AddRange(aggregato);

            historyPieces.OrderBy(i => i.Day);

            foreach (var cur in historyPieces)
            {
                var row = context.Set<HistoryPiece>().FirstOrDefault(hp => hp.MachineId == idMachine &&
                                                                 hp.Day == cur.Day &&
                                                                 (hp.Operator == cur.Operator ||
                                                                  hp.Operator == null && cur.Operator == null));
                if (row != null)
                {
                    row.CompletedCount = cur.CompletedCount;
                    row.ElapsedTime = cur.ElapsedTime;
                    row.ElapsedTimeProducing = cur.ElapsedTimeProducing;
                    row.ElapsedTimeCut = cur.ElapsedTimeCut;
                    row.ElapsedTimeWorking = cur.ElapsedTimeWorking;
                    row.ElapsedTimeTrim = cur.ElapsedTimeTrim;
                    row.PieceLengthSum = cur.PieceLengthSum;
                    row.RedoneCount = cur.RedoneCount;
                }
                else
                {
                    context.Set<HistoryPiece>().Add(row);
                }
                context.SaveChanges();
            }

        }

        public void Dispose()
        {
            _parentScope?.Dispose();
        }
    }
}