using FomMonitoringCore.DAL;
using FomMonitoringCore.Framework.Common;
using FomMonitoringCore.Framework.Model;
using Mapster;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FomMonitoringCore.Service
{
    public class MessageService
    {
        #region SP AGGREGATION

        /// <summary>
        /// Ritorna gli allarmi n base a tipo di dati da visualizzare in base al tipo di aggregazione
        /// </summary>
        /// <param name="machine"></param>
        /// <param name="period"></param>
        /// <param name="dataType"></param>
        /// <returns>Lista dei dettagli degli stati</returns>
        public static List<HistoryMessageModel> GetAggregationMessages_SP(MachineInfoModel machine, PeriodModel period, enDataType dataType)
        {
            List<HistoryMessageModel> result = new List<HistoryMessageModel>();

            try
            {
                using (FST_FomMonitoringEntities ent = new FST_FomMonitoringEntities())
                {
                    List<usp_AggregationMessage_Result> query = ent.usp_AggregationMessage(machine.Id, period.StartDate, period.EndDate, (int)period.Aggregation, (int)dataType).ToList();
                    result = query.Adapt<List<HistoryMessageModel>>();
                }
            }
            catch (Exception ex)
            {
                string errMessage = string.Format(ex.GetStringLog(),
                    machine.Id.ToString(),
                    string.Concat(period.StartDate.ToString(), " - ", period.EndDate.ToString(), " - ", period.Aggregation.ToString()),
                    dataType.ToString());
                LogService.WriteLog(errMessage, LogService.TypeLevel.Error, ex);
            }

            return result;
        }


        /// <summary>
        /// Ritorna gli allarmi n base a tipo di dati da visualizzare in base al tipo di aggregazione
        /// </summary>
        /// <param name="machine"></param>
        /// <param name="period"></param>
        /// <param name="dataType"></param>
        /// <returns>Lista dei dettagli degli stati</returns>
        public static List<HistoryMessageModel> GetAggregationMessages(MachineInfoModel machine, PeriodModel period, enDataType dataType)
        {
            try
            {
                using (FST_FomMonitoringEntities ent = new FST_FomMonitoringEntities())
                {
                    switch (dataType)
                    {
                        case enDataType.Historical:
                            if (period.Aggregation == enAggregation.Day)
                            {
                                var historyMessages = ent.HistoryMessage
                                    .Where(hm => hm.MachineId == machine.Id && hm.Day.Value >= period.StartDate && hm.Day.Value <= period.EndDate).ToList()
                                    .Select(s => new AggregationMessageModel
                                    {
                                        Id = s.Id,
                                        Code = null,
                                        StateId = s.StateId,
                                        Count = s.Count,
                                        Day = s.Day,
                                        ElapsedTime = s.ElapsedTime,
                                        Group = s.Group,
                                        MachineId = s.MachineId,
                                        Params = s.Params,
                                        Period = s.Period,
                                        TypeHistory = "d"
                                    }).ToList();

                                return historyMessages.Adapt<List<HistoryMessageModel>>();
                            }
                            if (period.Aggregation == enAggregation.Week)
                            {
                                var historyMessagesWeek = ent.HistoryMessage
                                    .Where(hm => hm.MachineId == machine.Id && hm.Day >= period.StartDate && hm.Day <= period.EndDate && hm.Code != null)
                                    .GroupBy(g => new
                                    {
                                        Year = g.Day.HasValue ? (int?)g.Day.Value.Year : null,
                                        Week = g.Day.Value.GetWeekNumber(),
                                        g.MachineId,
                                        g.Params,
                                        g.Group,
                                        g.StateId,
                                        Period = g.Day.HasValue ? (int?)g.Day.Value.Year * 100 + g.Day.Value.GetWeekNumber() : null,

                                    }).ToList().Select(s => new AggregationMessageModel
                                    {
                                        Id = s.Max(m => m.Id),
                                        Code = null,
                                        StateId = s.Key.StateId,
                                        Count = s.Count(),
                                        Day = s.Max(i => i.Day),
                                        ElapsedTime = s.Sum(i => i.ElapsedTime),
                                        Group = s.Key.Group,
                                        MachineId = s.Key.MachineId,
                                        Params = s.Key.Params,
                                        Period = s.Key.Period,
                                        TypeHistory = "w"
                                    }).ToList();

                                return historyMessagesWeek.Adapt<List<HistoryMessageModel>>();
                            }
                            if (period.Aggregation == enAggregation.Month)
                            {
                                var historyMessagesMonth = ent.HistoryMessage
                                    .Where(hm => hm.MachineId == machine.Id && hm.Day >= period.StartDate && hm.Day <= period.EndDate && hm.Code != null)
                                    .GroupBy(g => new
                                    {
                                        Year = g.Day.HasValue ? (int?)g.Day.Value.Year : null,
                                        Week = g.Day.Value.Month,
                                        g.MachineId,
                                        g.Params,
                                        g.Group,
                                        g.StateId,
                                        Period = g.Day.HasValue ? (int?)g.Day.Value.Year * 100 + g.Day.Value.Month : null,

                                    }).ToList().Select(s => new AggregationMessageModel
                                    {
                                        Id = s.Max(m => m.Id),
                                        Code = null,
                                        StateId = s.Key.StateId,
                                        Count = s.Count(),
                                        Day = s.Max(i => i.Day),
                                        ElapsedTime = s.Sum(i => i.ElapsedTime),
                                        Group = s.Key.Group,
                                        MachineId = s.Key.MachineId,
                                        Params = s.Key.Params,
                                        Period = s.Key.Period,
                                        TypeHistory = "m"
                                    }).ToList();

                                return historyMessagesMonth.Adapt<List<HistoryMessageModel>>();
                            }
                            if (period.Aggregation == enAggregation.Quarter)
                            {
                                var historyMessagesQuarter = ent.HistoryMessage
                                    .Where(hm => hm.MachineId == machine.Id && hm.Day >= period.StartDate && hm.Day <= period.EndDate && hm.Code != null)
                                    .GroupBy(g => new
                                    {
                                        Year = g.Day.HasValue ? (int?)g.Day.Value.Year : null,
                                        Week = g.Day.Value.Month,
                                        g.MachineId,
                                        g.Params,
                                        g.Group,
                                        g.StateId,
                                        Period = g.Day.HasValue ? (int?)g.Day.Value.Year * 100 + GetQuarter(g.Day ?? DateTime.Now) : null,

                                    }).ToList().Select(s => new AggregationMessageModel
                                    {
                                        Id = s.Max(m => m.Id),
                                        Code = null,
                                        StateId = s.Key.StateId,
                                        Count = s.Count(),
                                        Day = s.Max(i => i.Day),
                                        ElapsedTime = s.Sum(i => i.ElapsedTime),
                                        Group = s.Key.Group,
                                        MachineId = s.Key.MachineId,
                                        Params = s.Key.Params,
                                        Period = s.Key.Period,
                                        TypeHistory = "q"
                                    }).ToList();

                                return historyMessagesQuarter.Adapt<List<HistoryMessageModel>>();
                            }
                            if (period.Aggregation == enAggregation.Year)
                            {
                                var historyMessagesYear = ent.HistoryMessage
                                    .Where(hm => hm.MachineId == machine.Id && hm.Day >= period.StartDate && hm.Day <= period.EndDate && hm.Code != null)
                                    .GroupBy(g => new
                                    {
                                        Year = g.Day.HasValue ? (int?)g.Day.Value.Year : null,
                                        Week = g.Day.Value.Month,
                                        g.MachineId,
                                        g.Params,
                                        g.Group,
                                        g.StateId,
                                        Period = g.Day.HasValue ? (int?)g.Day.Value.Year : null,

                                    }).ToList().Select(s => new AggregationMessageModel
                                    {
                                        Id = s.Max(m => m.Id),
                                        Code = null,
                                        StateId = s.Key.StateId,
                                        Count = s.Count(),
                                        Day = s.Max(i => i.Day),
                                        ElapsedTime = s.Sum(i => i.ElapsedTime),
                                        Group = s.Key.Group,
                                        MachineId = s.Key.MachineId,
                                        Params = s.Key.Params,
                                        Period = s.Key.Period,
                                        TypeHistory = "y"
                                    }).ToList();

                                return historyMessagesYear.Adapt<List<HistoryMessageModel>>();
                            }
                            break;
                        case enDataType.Summary:
                            var historyMessagesSummary = ent.HistoryMessage
                                .Where(hm => hm.MachineId == machine.Id && hm.Day >= period.StartDate && hm.Day <= period.EndDate && hm.Code != null)
                                .GroupBy(g => new
                                {
                                    g.MachineId,
                                    //g.Params,
                                    //g.Group,
                                    g.StateId,
                                    g.Code

                                }).ToList().Select(s => new AggregationMessageModel
                                {
                                    Id = s.Max(m => m.Id),
                                    Code = s.Key.Code,
                                    StateId = s.Key.StateId,
                                    Count = s.Count(),
                                    Day = s.Max(i => i.Day),
                                    ElapsedTime = s.Sum(i => i.ElapsedTime),
                                    //Group = s.Key.Group,
                                    MachineId = s.Key.MachineId,
                                    //Params = s.Key.Params,
                                    TypeHistory = "y"
                                }).ToList();

                            return historyMessagesSummary.Adapt<List<HistoryMessageModel>>();
                    }
                }
            }
            catch (Exception ex)
            {
                string errMessage = string.Format(ex.GetStringLog(),
                    machine.Id.ToString(),
                    string.Concat(period.StartDate.ToString(), " - ", period.EndDate.ToString(), " - ", period.Aggregation.ToString()),
                    dataType.ToString());
                LogService.WriteLog(errMessage, LogService.TypeLevel.Error, ex);
            }

            return null;
        }
        #endregion

        private static int GetQuarter(DateTime fromDate)
        {
            int month = fromDate.Month - 1;
            int month2 = Math.Abs(month / 3) + 1;
            return month2;
        }

        /// <summary>
        /// Ritorna i dettagli degli allarmi in base a macchina e periodo
        /// </summary>
        /// <param name="machine"></param>
        /// <param name="period"></param>
        /// <returns>Lista dei dettagli degli stati</returns>
        public static List<HistoryMessageModel> GetAllHistoryMessages(MachineInfoModel machine, PeriodModel period)
        {
            List<HistoryMessageModel> result = new List<HistoryMessageModel>();

            try
            {
                using (FST_FomMonitoringEntities ent = new FST_FomMonitoringEntities())
                {
                    string aggType = period.Aggregation.GetDescription();

                    List<HistoryMessage> query = (from hs in ent.HistoryMessage
                                                  where hs.MachineId == machine.Id
                                                  && hs.Day >= period.StartDate && hs.Day <= period.EndDate
                                                  && hs.TypeHistory == aggType
                                                  select hs).ToList();

                    result = query.Adapt<List<HistoryMessageModel>>();
                }
            }
            catch (Exception ex)
            {
                string errMessage = string.Format(ex.GetStringLog(),
                    machine.Id.ToString(),
                    string.Concat(period.StartDate.ToString(), " - ", period.EndDate.ToString(), " - ", period.Aggregation.ToString()));
                LogService.WriteLog(errMessage, LogService.TypeLevel.Error, ex);
            }

            return result;
        }

        public static List<MessageMachineModel> GetMessageDetails(MachineInfoModel machine, PeriodModel period)
        {
            List<MessageMachineModel> result = new List<MessageMachineModel>();

            try
            {
                using (FST_FomMonitoringEntities ent = new FST_FomMonitoringEntities())
                {
                    List<MessageMachine> query = ent.MessageMachine.Where(m => m.MachineId == machine.Id &&
                                        m.Day >= period.StartDate && m.Day <= period.EndDate).ToList();

                    result = query.Adapt<List<MessageMachine>, List<MessageMachineModel>>();
                }
            }
            catch (Exception ex)
            {
                string errMessage = string.Format(ex.GetStringLog(),
                    machine.Id.ToString(),
                    string.Concat(period.StartDate.ToString(), " - ", period.EndDate.ToString(), " - ", period.Aggregation.ToString()));
                LogService.WriteLog(errMessage, LogService.TypeLevel.Error, ex);
            }

            return result;
        }

        public static List<MessageMachineModel> GetAllCurrentMessages(MachineInfoModel machine, PeriodModel period)
        {
            List<MessageMachineModel> result = new List<MessageMachineModel>();

            try
            {
                using (FST_FomMonitoringEntities ent = new FST_FomMonitoringEntities())
                {

                    List<MessageMachine> query = (from hs in ent.MessageMachine
                                                  where hs.MachineId == machine.Id
                                                  && hs.Day >= period.StartDate && hs.Day <= period.EndDate
                                                  select hs).ToList();

                    result = query.Adapt<List<MessageMachineModel>>();
                }
            }
            catch (Exception ex)
            {
                string errMessage = string.Format(ex.GetStringLog(),
                    machine.Id.ToString(),
                    string.Concat(period.StartDate.ToString(), " - ", period.EndDate.ToString(), " - ", period.Aggregation.ToString()));
                LogService.WriteLog(errMessage, LogService.TypeLevel.Error, ex);
            }
            return result;
        }

    }
}
