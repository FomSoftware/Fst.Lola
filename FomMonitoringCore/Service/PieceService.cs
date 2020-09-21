using FomMonitoringCore.SqlServer;
using FomMonitoringCore.Framework.Common;
using FomMonitoringCore.Framework.Model;
using Mapster;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using FomMonitoringCore.Extensions;
using FomMonitoringCore.SqlServer.Repository;

namespace FomMonitoringCore.Service
{
    public class PieceService : IPieceService
    {
        private readonly IFomMonitoringEntities _context;
        private readonly IHistoryPieceRepository _historyPieceRepository;

        public PieceService(IFomMonitoringEntities context, IHistoryPieceRepository historyPieceRepository)
        {
            _context = context;
            _historyPieceRepository = historyPieceRepository;
        }


        #region AGGREGATION
        public List<HistoryPieceModel> GetAggregationPiecesSP(MachineInfoModel machine, PeriodModel period, enDataType dataType)
        {
            List<HistoryPieceModel> result = new List<HistoryPieceModel>();

            try
            {
                List<usp_AggregationPiece_Result> query = _context.usp_AggregationPiece(machine.Id, period.StartDate, period.EndDate, (int)period.Aggregation, (int)dataType).ToList();
                result = query.Adapt<List<HistoryPieceModel>>();
            }
            catch (Exception ex)
            {
                string errMessage = string.Format(ex.GetStringLog(),
                    machine.Id.ToString(),
                    string.Concat(period.StartDate.ToString(), " - ", period.EndDate.ToString(), " - ", period.Aggregation.ToString(),
                        dataType.ToString()));
                LogService.WriteLog(errMessage, LogService.TypeLevel.Error, ex);
            }

            return result;
        }
        /// <summary>
        /// Ritorna gli aggregati pieces in base al tipo di aggregazione
        /// </summary>
        /// <param name="machine"></param>
        /// <param name="period"></param>
        /// <returns>Lista dei dettagli degli stati</returns>
        public List<HistoryPieceModel> GetAggregationPieces(MachineInfoModel machine, PeriodModel period, enDataType dataType)
        {
            List<HistoryPieceModel> result = new List<HistoryPieceModel>();

            try
            {
                var query =_context.Set<HistoryPiece>().Where(hp => hp.MachineId == machine.Id &&
                                                         hp.Day >= period.StartDate &&
                                                         hp.Day <= period.EndDate);
                if (dataType == enDataType.Dashboard)
                {
                    result = query.Where(hp => hp.Shift == null && hp.Operator == null).ToList()
                        .GroupBy(g => g.System).Select(n => new HistoryPieceModel
                        {
                            Id = n.Max(i => i.Id),
                            CompletedCount = n.Sum(i => i.CompletedCount) ,
                            Day = n.Max(i => i.Day ?? DateTime.MinValue),
                            ElapsedTime = n.Sum(i => i.ElapsedTime),
                            ElapsedTimeProducing = n.Sum(i => i.ElapsedTimeProducing),
                            ElapsedTimeCut = n.Sum(i => i.ElapsedTimeCut),
                            ElapsedTimeWorking = n.Sum(i => i.ElapsedTimeWorking),
                            ElapsedTimeTrim = n.Sum(i => i.ElapsedTimeTrim),
                            MachineId = machine.Id,
                            Operator = null,
                            Period = null,
                            PieceLengthSum = n.Sum(i => i.PieceLengthSum),
                            RedoneCount = n.Sum(i => i.RedoneCount),
                            Shift = null,
                            System = n.Key,
                            TypeHistory = "d" 
                        }).ToList();

                }
                else if (dataType == enDataType.Historical)
                {
                    if (period.Aggregation == enAggregation.Day)
                    {
                        result = query.Where(hp => hp.Shift == null && hp.Operator == null).ToList()
                            .Select(n => new HistoryPieceModel
                            {
                                Id = n.Id,
                                CompletedCount = n.CompletedCount,
                                Day = n.Day ?? DateTime.MinValue,
                                ElapsedTime = n.ElapsedTime,
                                ElapsedTimeProducing = n.ElapsedTimeProducing,
                                ElapsedTimeCut = n.ElapsedTimeCut,
                                ElapsedTimeWorking = n.ElapsedTimeWorking,
                                ElapsedTimeTrim = n.ElapsedTimeTrim,
                                MachineId = machine.Id,
                                Operator = null,
                                Period = n.Period,
                                PieceLengthSum = n.PieceLengthSum,
                                RedoneCount = n.RedoneCount,
                                Shift = null,
                                System = n.System,
                                TypeHistory = "d"
                            }).ToList();
                    }
                    else if (period.Aggregation == enAggregation.Week)
                    {
                        result = query.Where(hp => hp.Shift == null && hp.Operator == null).ToList()
                            .GroupBy(g => new { g.System, g.WeekOfYearDay })
                            .Select(n => new HistoryPieceModel
                            {
                                Id = n.Max(i => i.Id),
                                CompletedCount = n.Sum(i => i.CompletedCount),
                                Day = n.Max(i => i.Day ?? DateTime.MinValue),
                                ElapsedTime = n.Sum(i => i.ElapsedTime),
                                ElapsedTimeProducing = n.Sum(i => i.ElapsedTimeProducing),
                                ElapsedTimeCut = n.Sum(i => i.ElapsedTimeCut),
                                ElapsedTimeWorking = n.Sum(i => i.ElapsedTimeWorking),
                                ElapsedTimeTrim = n.Sum(i => i.ElapsedTimeTrim),
                                MachineId = machine.Id,
                                Operator = null,
                                Period = n.Key.WeekOfYearDay,
                                PieceLengthSum = n.Sum(i => i.PieceLengthSum),
                                RedoneCount = n.Sum(i => i.RedoneCount),
                                Shift = null,
                                System = n.Key.System,
                                TypeHistory = "w"
                            }).ToList();
                    } 
                    else if (period.Aggregation == enAggregation.Month)
                    {
                        result = query.Where(hp => hp.Shift == null && hp.Operator == null).ToList()
                            .GroupBy(g => new { g.System, g.Day.Value.Month })
                            .Select(n => new HistoryPieceModel
                            {
                                Id = n.Max(i => i.Id),
                                CompletedCount = n.Sum(i => i.CompletedCount),
                                Day = n.Max(i => i.Day ?? DateTime.MinValue),
                                ElapsedTime = n.Sum(i => i.ElapsedTime),
                                ElapsedTimeProducing = n.Sum(i => i.ElapsedTimeProducing),
                                ElapsedTimeCut = n.Sum(i => i.ElapsedTimeCut),
                                ElapsedTimeWorking = n.Sum(i => i.ElapsedTimeWorking),
                                ElapsedTimeTrim = n.Sum(i => i.ElapsedTimeTrim),
                                MachineId = machine.Id,
                                Operator = null,
                                Period = n.Key.Month,
                                PieceLengthSum = n.Sum(i => i.PieceLengthSum),
                                RedoneCount = n.Sum(i => i.RedoneCount),
                                Shift = null,
                                System = n.Key.System,
                                TypeHistory = "m"
                            }).ToList();
                    } else if (period.Aggregation == enAggregation.Quarter)
                    {
                        result = query.Where(hp => hp.Shift == null && hp.Operator == null).ToList()
                            .GroupBy(g => new { g.System, g.QuarteOfYearDay })
                            .Select(n => new HistoryPieceModel
                            {
                                Id = n.Max(i => i.Id),
                                CompletedCount = n.Sum(i => i.CompletedCount),
                                Day = n.Max(i => i.Day ?? DateTime.MinValue),
                                ElapsedTime = n.Sum(i => i.ElapsedTime),
                                ElapsedTimeProducing = n.Sum(i => i.ElapsedTimeProducing),
                                ElapsedTimeCut = n.Sum(i => i.ElapsedTimeCut),
                                ElapsedTimeWorking = n.Sum(i => i.ElapsedTimeWorking),
                                ElapsedTimeTrim = n.Sum(i => i.ElapsedTimeTrim),
                                MachineId = machine.Id,
                                Operator = null,
                                Period = n.Key.QuarteOfYearDay,
                                PieceLengthSum = n.Sum(i => i.PieceLengthSum),
                                RedoneCount = n.Sum(i => i.RedoneCount),
                                Shift = null,
                                System = n.Key.System,
                                TypeHistory = "q"
                            }).ToList();
                    }
                    else if (period.Aggregation == enAggregation.Year)
                    {
                        result = query.Where(hp => hp.Shift == null && hp.Operator == null).ToList()
                            .GroupBy(g => new { g.System, g.Day.Value.Year })
                            .Select(n => new HistoryPieceModel
                            {
                                Id = n.Max(i => i.Id),
                                CompletedCount = n.Sum(i => i.CompletedCount),
                                Day = n.Max(i => i.Day ?? DateTime.MinValue),
                                ElapsedTime = n.Sum(i => i.ElapsedTime),
                                ElapsedTimeProducing = n.Sum(i => i.ElapsedTimeProducing),
                                ElapsedTimeCut = n.Sum(i => i.ElapsedTimeCut),
                                ElapsedTimeWorking = n.Sum(i => i.ElapsedTimeWorking),
                                ElapsedTimeTrim = n.Sum(i => i.ElapsedTimeTrim),
                                MachineId = machine.Id,
                                Operator = null,
                                Period = n.Key.Year,
                                PieceLengthSum = n.Sum(i => i.PieceLengthSum),
                                RedoneCount = n.Sum(i => i.RedoneCount),
                                Shift = null,
                                System = n.Key.System,
                                TypeHistory = "y"
                            }).ToList();
                    }

                }
                else if (dataType == enDataType.Operators)
                {
                    result = query.Where(hp => hp.Shift == null && hp.Operator != null).ToList()
                        .GroupBy(g => new { g.System, g.Operator})
                        .Select(n => new HistoryPieceModel
                        {
                            Id = n.Max(i => i.Id),
                            CompletedCount = n.Sum(i => i.CompletedCount),
                            Day = n.Max(i => i.Day ?? DateTime.MinValue),
                            ElapsedTime = n.Sum(i => i.ElapsedTime),
                            ElapsedTimeProducing = n.Sum(i => i.ElapsedTimeProducing),
                            ElapsedTimeCut = n.Sum(i => i.ElapsedTimeCut),
                            ElapsedTimeWorking = n.Sum(i => i.ElapsedTimeWorking),
                            ElapsedTimeTrim = n.Sum(i => i.ElapsedTimeTrim),
                            MachineId = machine.Id,
                            Operator = n.Key.Operator,
                            Period = null,
                            PieceLengthSum = n.Sum(i => i.PieceLengthSum),
                            RedoneCount = n.Sum(i => i.RedoneCount),
                            Shift = null,
                            System = n.Key.System,
                            TypeHistory = "d"
                        }).ToList();
                }

            }
            catch (Exception ex)
            {
                string errMessage = string.Format(ex.GetStringLog(),
                    machine.Id.ToString(),
                    string.Concat(period.StartDate.ToString(), " - ", period.EndDate.ToString(), " - ", period.Aggregation.ToString(),
                    dataType.ToString()));
                LogService.WriteLog(errMessage, LogService.TypeLevel.Error, ex);
            }

            return result;
        }

        #endregion

        #region HISTORY 

        /// <summary>
        /// Ritorna i dettagli dei pezzi dato l'ID della macchina
        /// </summary>
        /// <param name="machineId"></param>
        /// <param name="dateFrom"></param>
        /// <param name="dateTo"></param>
        /// <param name="typePeriod"></param>
        /// <returns>Lista dei dettagli dei pezzi</returns>
        public List<HistoryPieceModel> GetAllHistoryPiecesByMachineId(int machineId, DateTime dateFrom, DateTime dateTo, enAggregation typePeriod)
        {
            List<HistoryPieceModel> result = new List<HistoryPieceModel>();
            try
            {
                    List<HistoryPiece> historyPieceList = _historyPieceRepository.Get(w => w.MachineId == machineId && w.Period != null &&
                        w.Period.Value.PeriodToDate(typePeriod) >= dateFrom && w.Period.Value.PeriodToDate(typePeriod) <= dateTo).ToList();
                    result = historyPieceList.Adapt<List<HistoryPieceModel>>();
                
            }
            catch (Exception ex)
            {
                string errMessage = string.Format(ex.GetStringLog(), machineId.ToString(), dateFrom.ToString(), dateTo.ToString(), typePeriod.ToString());
                LogService.WriteLog(errMessage, LogService.TypeLevel.Error, ex);
            }
            return result;
        }

        /// <summary>
        /// Ritorna i dettagli dei pezzi del sistema specificato dato l'ID della macchina
        /// </summary>
        /// <param name="machineId"></param>
        /// <param name="system"></param>
        /// <param name="dateFrom"></param>
        /// <param name="dateTo"></param>
        /// <param name="typePeriod"></param>
        /// <returns>Lista dei dettagli dei pezzi</returns>
        public List<HistoryPieceModel> GetAllHistoryPiecesByMachineIdSystem(int machineId, string system, DateTime dateFrom, DateTime dateTo, enAggregation typePeriod)
        {
            List<HistoryPieceModel> result = new List<HistoryPieceModel>();
            try
            {
                    List<HistoryPiece> historyPieceList = _historyPieceRepository.Get(w => w.MachineId == machineId && w.Period != null &&
                        w.Period.Value.PeriodToDate(typePeriod) >= dateFrom && w.Period.Value.PeriodToDate(typePeriod) <= dateTo && w.System == system).ToList();
                    result = historyPieceList.Adapt<List<HistoryPieceModel>>();
                
            }
            catch (Exception ex)
            {
                string errMessage = string.Format(ex.GetStringLog(), machineId.ToString(), system, dateFrom.ToString(), dateTo.ToString(), typePeriod.ToString());
                LogService.WriteLog(errMessage, LogService.TypeLevel.Error, ex);
            }
            return result;
        }


        #endregion
        
    }
}
