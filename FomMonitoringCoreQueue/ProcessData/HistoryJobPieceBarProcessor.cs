using System;
using System.Collections.Generic;
using System.Linq;
using FomMonitoringCore.DAL;
using FomMonitoringCore.Service;
using FomMonitoringCoreQueue.Dto;
using Mapster;

namespace FomMonitoringCoreQueue.ProcessData
{
    public class HistoryJobPieceBarProcessor : IProcessor<HistoryJobPieceBar>
    {
        private readonly IFomMonitoringEntities _context;

        public HistoryJobPieceBarProcessor(IFomMonitoringEntities context)
        {
            _context = context;
        }
        public bool ProcessData(HistoryJobPieceBar data)
        {
            try
            {
                _context.Refresh();
                var serial = data.InfoMachine.FirstOrDefault()?.MachineSerial;
                var mac = _context.Set<Machine>().FirstOrDefault(m => m.Serial == serial);

                if (mac == null)
                    return false;


                var bar = data.BarMachine.BuildAdapter().AddParameters("machineId", mac.Id).AdaptToType<List<Bar>>();
                bar = bar.Where(w => w.StartTime > (mac.LastUpdate ?? new DateTime())).ToList();

                foreach (var bb in bar)
                {
                    var trovato = _context.Set<Bar>().FirstOrDefault(m => m.Index == bb.Index && m.JobCode == bb.JobCode && bb.MachineId == m.MachineId);

                    if (trovato?.JobCode != null)
                        continue;
                    _context.Set<Bar>().Add(bb);
                }


                var historyJob = data.HistoryJobMachine.BuildAdapter().AddParameters("machineId", mac.Id).AdaptToType<List<HistoryJob>>();
                if (historyJob != null)
                {
                    _context.Set<HistoryJob>().AddRange(historyJob);
                }

                var piece = data.PieceMachine.BuildAdapter().AddParameters("machineId", mac.Id).AdaptToType<List<Piece>>();
                piece = piece.Where(w => w.EndTime > (mac.LastUpdate ?? new DateTime())).ToList();
                _context.Set<Piece>().AddRange(piece);
                

                mac.LastUpdate = DateTime.UtcNow;

                _context.usp_HistoricizingPieces(mac.Id);
                _context.usp_HistoricizingBars(mac.Id);

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