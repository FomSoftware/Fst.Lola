using System;
using System.Collections.Generic;
using System.Linq;
using FomMonitoringCore.Framework.Common;
using FomMonitoringCore.Framework.Model;
using FomMonitoringCore.SqlServer;
using FomMonitoringCore.SqlServer.Repository;
using Mapster;

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

        /// <summary>
        ///     Ritorna gli aggregati pieces in base al tipo di aggregazione
        /// </summary>
        /// <param name="machine"></param>
        /// <param name="period"></param>
        /// <param name="dataType"></param>
        /// <returns>Lista dei dettagli degli stati</returns>
        public List<HistoryPieceModel> GetAggregationPieces(MachineInfoModel machine, PeriodModel period,
            enDataType dataType)
        {
            var result = new List<HistoryPieceModel>();

            try
            {
                var query = _context.Set<HistoryPiece>().Where(hp => hp.MachineId == machine.Id &&
                                                                     hp.Day >= period.StartDate &&
                                                                     hp.Day <= period.EndDate);
                switch (dataType)
                {
                    case enDataType.Dashboard:
                        result = query.Where(hp => hp.Shift == null && hp.Operator == null).ToList()
                            .GroupBy(g => g.System).Select(n => new HistoryPieceModel
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
                                Period = null,
                                PieceLengthSum = n.Sum(i => i.PieceLengthSum),
                                RedoneCount = n.Sum(i => i.RedoneCount),
                                Shift = null,
                                System = n.Key,
                                TypeHistory = "d"
                            }).ToList();
                        break;
                    case enDataType.Historical:
                    {
                        var queryResult = query.Where(hp => hp.Shift == null && hp.Operator == null).ToList();
                        switch (period.Aggregation)
                        {
                            case enAggregation.Day:
                            {
                                int? Func(HistoryPiece hs)
                                {
                                    return hs.Period;
                                }

                                const string typeHistory = "d";
                                var res = BuildAggregationList(queryResult, typeHistory, Func);
                                return res.ToList();
                            }
                            case enAggregation.Week:
                            {
                                int? Func(HistoryPiece hs)
                                {
                                    return hs.WeekOfYearDay;
                                }

                                const string typeHistory = "w";
                                var res = BuildAggregationList(queryResult, typeHistory, Func);
                                return res.ToList();
                            }
                            case enAggregation.Month:
                            {
                                int? Func(HistoryPiece hs)
                                {
                                    return hs.Day?.Month;
                                }

                                const string typeHistory = "m";
                                var res = BuildAggregationList(queryResult, typeHistory, Func);
                                return res.ToList();
                            }
                            case enAggregation.Quarter:
                            {
                                int? Func(HistoryPiece hs)
                                {
                                    return hs.QuarteOfYearDay;
                                }

                                const string typeHistory = "q";
                                var res = BuildAggregationList(queryResult, typeHistory, Func);
                                return res.ToList();
                            }
                            case enAggregation.Year:
                            {
                                int? Func(HistoryPiece hs)
                                {
                                    return hs.Day?.Year;
                                }

                                const string typeHistory = "y";
                                var res = BuildAggregationList(queryResult, typeHistory, Func);
                                return res.ToList();
                            }
                        }

                        break;
                    }
                    case enDataType.Operators:
                        result = query.Where(hp => hp.Shift == null && hp.Operator != null).ToList()
                            .GroupBy(g => new {g.System, g.Operator})
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
                        break;
                }
            }
            catch (Exception ex)
            {
                var errMessage = string.Format(ex.GetStringLog(),
                    machine.Id.ToString(),
                    string.Concat(period.StartDate.ToString(), " - ", period.EndDate.ToString(), " - ",
                        period.Aggregation.ToString(),
                        dataType.ToString()));
                LogService.WriteLog(errMessage, LogService.TypeLevel.Error, ex);
            }

            return result;
        }

        private List<HistoryPieceModel> BuildAggregationList(IEnumerable<HistoryPiece> queryResult, string typeHistory,
            Func<HistoryPiece, int?> periodFunc)
        {
            var groups = queryResult.GroupBy(hs => new
            {
                hs.MachineId,
                hs.System,
                Period = periodFunc.Invoke(hs)
            });

            var result = groups.Select(n => new HistoryPieceModel
            {
                Id = n.Max(i => i.Id),
                CompletedCount = n.Sum(i => i.CompletedCount),
                Day = n.Max(i => i.Day ?? DateTime.MinValue),
                ElapsedTime = n.Sum(i => i.ElapsedTime),
                ElapsedTimeProducing = n.Sum(i => i.ElapsedTimeProducing),
                ElapsedTimeCut = n.Sum(i => i.ElapsedTimeCut),
                ElapsedTimeWorking = n.Sum(i => i.ElapsedTimeWorking),
                ElapsedTimeTrim = n.Sum(i => i.ElapsedTimeTrim),
                MachineId = n.Key.MachineId,
                Operator = null,
                Period = n.Key.Period,
                PieceLengthSum = n.Sum(i => i.PieceLengthSum),
                RedoneCount = n.Sum(i => i.RedoneCount),
                Shift = null,
                System = n.Key.System,
                TypeHistory = typeHistory
            }).ToList();

            return result;
        }

        #endregion

        #region HISTORY 

        /// <summary>
        ///     Ritorna i dettagli dei pezzi dato l'ID della macchina
        /// </summary>
        /// <param name="machineId"></param>
        /// <param name="dateFrom"></param>
        /// <param name="dateTo"></param>
        /// <param name="typePeriod"></param>
        /// <returns>Lista dei dettagli dei pezzi</returns>
        public List<HistoryPieceModel> GetAllHistoryPiecesByMachineId(int machineId, DateTime dateFrom, DateTime dateTo,
            enAggregation typePeriod)
        {
            var result = new List<HistoryPieceModel>();
            try
            {
                var historyPieceList = _historyPieceRepository.Get(w => w.MachineId == machineId && w.Period != null &&
                                                                        w.Period.Value.PeriodToDate(typePeriod) >=
                                                                        dateFrom &&
                                                                        w.Period.Value.PeriodToDate(typePeriod) <=
                                                                        dateTo).ToList();
                result = historyPieceList.Adapt<List<HistoryPieceModel>>();
            }
            catch (Exception ex)
            {
                var errMessage = string.Format(ex.GetStringLog(), machineId.ToString(), dateFrom.ToString(),
                    dateTo.ToString(), typePeriod.ToString());
                LogService.WriteLog(errMessage, LogService.TypeLevel.Error, ex);
            }

            return result;
        }

        /// <summary>
        ///     Ritorna i dettagli dei pezzi del sistema specificato dato l'ID della macchina
        /// </summary>
        /// <param name="machineId"></param>
        /// <param name="system"></param>
        /// <param name="dateFrom"></param>
        /// <param name="dateTo"></param>
        /// <param name="typePeriod"></param>
        /// <returns>Lista dei dettagli dei pezzi</returns>
        public List<HistoryPieceModel> GetAllHistoryPiecesByMachineIdSystem(int machineId, string system,
            DateTime dateFrom, DateTime dateTo, enAggregation typePeriod)
        {
            var result = new List<HistoryPieceModel>();
            try
            {
                var historyPieceList = _historyPieceRepository.Get(w => w.MachineId == machineId && w.Period != null &&
                                                                        w.Period.Value.PeriodToDate(typePeriod) >=
                                                                        dateFrom &&
                                                                        w.Period.Value.PeriodToDate(typePeriod) <=
                                                                        dateTo && w.System == system).ToList();
                result = historyPieceList.Adapt<List<HistoryPieceModel>>();
            }
            catch (Exception ex)
            {
                var errMessage = string.Format(ex.GetStringLog(), machineId.ToString(), system, dateFrom.ToString(),
                    dateTo.ToString(), typePeriod.ToString());
                LogService.WriteLog(errMessage, LogService.TypeLevel.Error, ex);
            }

            return result;
        }

        #endregion
    }
}