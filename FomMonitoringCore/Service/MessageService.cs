using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using FomMonitoringCore.Extensions;
using FomMonitoringCore.Framework.Common;
using FomMonitoringCore.Framework.Model;
using FomMonitoringCore.SqlServer;
using FomMonitoringCore.SqlServer.Repository;
using Mapster;

namespace FomMonitoringCore.Service
{
    public class MessageService : IMessageService
    {
        private readonly IAccountService _accountService;
        private readonly IFomMonitoringEntities _context;
        private readonly IHistoryMessageRepository _historyMessageRepository;
        private readonly ILanguageService _languageService;
        private readonly IMachineRepository _machineRepository;
        private readonly IMessageMachineRepository _messageMachineRepository;
        private readonly IMessagesIndexRepository _messagesIndexRepository;

        public MessageService(
            IMessageMachineRepository messageMachineRepository,
            IMachineRepository machineRepository,
            IMessagesIndexRepository messagesIndexRepository,
            IFomMonitoringEntities context,
            IHistoryMessageRepository historyMessageRepository,
            ILanguageService languageService, IAccountService accountService)
        {
            _messageMachineRepository = messageMachineRepository;
            _machineRepository = machineRepository;
            _messagesIndexRepository = messagesIndexRepository;
            _context = context;
            _historyMessageRepository = historyMessageRepository;
            _languageService = languageService;
            _accountService = accountService;
        }

        #region SP AGGREGATION

        /// <summary>
        ///     Ritorna gli allarmi n base a tipo di dati da visualizzare in base al tipo di aggregazione
        /// </summary>
        /// <param name="machine"></param>
        /// <param name="period"></param>
        /// <param name="dataType"></param>
        /// <param name="actualMachineGroup"></param>
        /// <returns>Lista dei dettagli degli stati</returns>
        public List<HistoryMessageModel> GetAggregationMessages(MachineInfoModel machine, PeriodModel period,
            enDataType dataType, string actualMachineGroup = null)
        {
            try
            {
                var cl = _languageService.GetCurrentLanguage() ?? 0;

                var machineGroup = _context.Set<MachineGroup>()
                    .FirstOrDefault(n => n.MachineGroupName == actualMachineGroup)?.Id;

                var queryResult = _historyMessageRepository
                    .GetHistoryMessage(machine.Id, period.StartDate, period.EndDate, machineGroup);
                switch (dataType)
                {
                    case enDataType.Historical:
                        switch (period.Aggregation)
                        {
                            case enAggregation.Day:
                            {
                                int? Func(HistoryMessage hs)
                                {
                                    return hs.Period;
                                }

                                const string typeHistory = "d";
                                var result = BuildAggregationList(queryResult, typeHistory, Func);
                                return result.ToList();
                            }
                            case enAggregation.Week:
                            {
                                int? Func(HistoryMessage hs)
                                {
                                    return hs.Day.HasValue
                                        ? (int?) hs.Day.Value.Year * 100 + hs.Day.Value.GetWeekNumber()
                                        : null;
                                }

                                const string typeHistory = "w";
                                var result = BuildAggregationList(queryResult, typeHistory, Func);
                                return result.ToList();
                            }
                            case enAggregation.Month:
                            {
                                int? Func(HistoryMessage hs)
                                {
                                    return hs.Day.HasValue ? (int?) hs.Day.Value.Year * 100 + hs.Day.Value.Month : null;
                                }

                                const string typeHistory = "m";
                                var result = BuildAggregationList(queryResult, typeHistory, Func);
                                return result.ToList();
                            }
                            case enAggregation.Quarter:
                            {
                                int? Func(HistoryMessage hs)
                                {
                                    return hs.Day.HasValue
                                        ? (int?) hs.Day.Value.Year * 100 + GetQuarter(hs.Day ?? DateTime.UtcNow)
                                        : null;
                                }

                                const string typeHistory = "q";
                                var result = BuildAggregationList(queryResult, typeHistory, Func);
                                return result.ToList();
                            }
                            case enAggregation.Year:
                            {
                                int? Func(HistoryMessage hs)
                                {
                                    return hs.Day.HasValue ? (int?) hs.Day.Value.Year * 100 + hs.Day.Value.Year : null;
                                }

                                const string typeHistory = "y";
                                var result = BuildAggregationList(queryResult, typeHistory, Func);
                                return result.ToList();
                            }
                        }

                        break;
                    case enDataType.Summary:
                        var historyMessagesSummary = _historyMessageRepository
                            .GetHistoryMessage(machine.Id, period.StartDate, period.EndDate, machineGroup)
                            .GroupBy(g => new
                            {
                                g.MachineId,
                                Description = g.GetDescription(cl),
                                g.MessagesIndex.MessageCode,
                                g.MessagesIndex?.MessageTypeId
                            }).ToList().Select(s => new AggregationMessageModel
                            {
                                Id = s.Max(m => m.Id),
                                Code = s.Key.MessageCode,
                                Count = s.Sum(i => i.Count),
                                Day = s.Max(i => i.Day),
                                MachineId = s.Key.MachineId,
                                TypeHistory = "y",
                                Description = s.Key.Description,
                                Type = s.Key.MessageTypeId
                            }).ToList();

                        return historyMessagesSummary.BuildAdapter().AdaptToType<List<HistoryMessageModel>>();
                }
            }
            catch (Exception ex)
            {
                var errMessage = string.Format(ex.GetStringLog(),
                    machine.Id.ToString(),
                    string.Concat(period.StartDate.ToString(), " - ", period.EndDate.ToString(), " - ",
                        period.Aggregation.ToString()),
                    dataType.ToString());
                LogService.WriteLog(errMessage, LogService.TypeLevel.Error, ex);
            }

            return new List<HistoryMessageModel>();
        }

        #endregion



        public List<MessageMachineModel> GetMessageDetails(MachineInfoModel machine, PeriodModel period,
            string actualMachineGroup = null)
        {
            var result = new List<MessageMachineModel>();

            try
            {
                var cl = _languageService.GetCurrentLanguage() ?? 0;
                var machineGroup = _context.Set<MachineGroup>()
                    .FirstOrDefault(n => n.MachineGroupName == actualMachineGroup)?.Id;
                var query = _messageMachineRepository
                    .GetMachineMessages(machine.Id, period.StartDate, period.EndDate, machineGroup, false, true)
                    .ToList();

                result = query.BuildAdapter().AddParameters("idLanguage", cl).AdaptToType<List<MessageMachineModel>>();
            }
            catch (Exception ex)
            {
                var errMessage = string.Format(ex.GetStringLog(),
                    machine.Id.ToString(),
                    string.Concat(period.StartDate.ToString(), " - ", period.EndDate.ToString(), " - ",
                        period.Aggregation.ToString()));
                LogService.WriteLog(errMessage, LogService.TypeLevel.Error, ex);
            }

            return result;
        }

        public long? GetExpiredSpan(MessageMachineModel mm)
        {
            if (mm.IsPeriodicMsg == true)
            {
                var cat = _context.Set<Machine>().Find(mm.MachineId)?.MachineModel.MessageCategoryId;
                var msg = _context.Set<MessagesIndex>().FirstOrDefault(f =>
                    f.MessageCode == mm.Code && f.MessageCategoryId == cat);
                if (msg == null) return null;

                //caso dei messaggi arrivati da json, non hanno span, vanno visualizzati subito
                if (msg.PeriodicSpan == null && mm.IgnoreDate == null)
                    return DateTime.UtcNow.Subtract(mm.Day.Value).Ticks;

                var span = msg.PeriodicSpan ?? 0;
                var initTime = _context.Set<Machine>().Find(mm.MachineId).ActivationDate?.AddHours(span);
                if (mm.IgnoreDate != null)
                {
                    var initInterval = _context.Set<MessageMachine>().Find(mm.Id).GetInitialSpanDate(span);
                    if (mm.IgnoreDate < initInterval)
                        initTime = initInterval;
                    else
                        return null;
                }

                return DateTime.UtcNow.Subtract(initTime.Value).Ticks;
            }

            return null;
        }

        public List<MessageMachineModel> GetOldMaintenanceMessages(MachineInfoModel machine, PeriodModel period)
        {
            var result = new List<MessageMachineModel>();

            try
            {
                var query = _context.Set<MessageMachine>().Where(m => m.MachineId == machine.Id &&
                                                                      m.Machine.ActivationDate != null &&
                                                                      m.MessagesIndex.IsPeriodicM &&
                                                                      m.IgnoreDate != null)
                    .ToList()
                    .Select(s => new MessageMachine
                    {
                        Day = s.Day,
                        MessagesIndexId = s.MessagesIndexId,
                        Operator = s.Operator,
                        Id = s.Id,
                        IgnoreDate = s.IgnoreDate,
                        MachineId = machine.Id,
                        Machine = _context.Set<Machine>().Find(machine.Id),
                        MessagesIndex = _context.Set<MessagesIndex>().Find(s.MessagesIndexId),
                        UserId = s.UserId
                    }).ToList()
                    .OrderByDescending(o => o.Day).ToList();

                var cl = _languageService.GetCurrentLanguage() ?? 0;
                result = query.BuildAdapter().AddParameters("idLanguage", cl).AdaptToType<List<MessageMachineModel>>();
            }
            catch (Exception ex)
            {
                var errMessage = string.Format(ex.GetStringLog(),
                    machine.Id.ToString(),
                    string.Concat(period.StartDate.ToString(), " - ", period.EndDate.ToString(), " - ",
                        period.Aggregation.ToString()));
                LogService.WriteLog(errMessage, LogService.TypeLevel.Error, ex);
            }

            return result;
        }

        public List<MessageMachineModel> GetMaintenanceMessages(MachineInfoModel machine, PeriodModel period)
        {
            var result = new List<MessageMachineModel>();

            try
            {
                var query = _context.Set<MessageMachine>().Where(m => m.MachineId == machine.Id &&
                                                                      m.Machine.ActivationDate != null &&
                                                                      m.MessagesIndex.IsPeriodicM )
                .ToList().GroupBy(g => g.MessagesIndexId)
                    .Select(s => new MessageMachine
                    {
                        Day = s.Max(i => i.Day),
                        MessagesIndexId = s.Key,
                        Operator = s.OrderByDescending(i => i.Day).FirstOrDefault()?.Operator,
                        Id = s.OrderByDescending(i => i.Day).FirstOrDefault()?.Id ?? 0,
                        IgnoreDate = s.OrderByDescending(i => i.Day).FirstOrDefault()?.IgnoreDate,
                        MachineId = machine.Id,
                        Machine = _context.Set<Machine>().Find(machine.Id)
                    }).ToList()
                    .OrderByDescending(o => o.Day).ToList();
                
                query = query.Where(m =>
                {
                    var msg = _context.Set<MessagesIndex>().Find(m.MessagesIndexId);
                    if (msg == null) return false;
                    var span = msg.PeriodicSpan ?? 0;
                    m.MessagesIndex = msg;
                    return m.IgnoreDate == null && span > 0 &&
                           m.Machine.ActivationDate?.AddHours(span) <= DateTime.UtcNow ||
                           m.IgnoreDate == null && span == 0;
                }).ToList();

                var cl = _languageService.GetCurrentLanguage() ?? 0;
                result = query.BuildAdapter().AddParameters("idLanguage", cl).AdaptToType<List<MessageMachineModel>>();
            }
            catch (Exception ex)
            {
                var errMessage = string.Format(ex.GetStringLog(),
                    machine.Id.ToString(),
                    string.Concat(period.StartDate.ToString(), " - ", period.EndDate.ToString(), " - ",
                        period.Aggregation.ToString()));
                LogService.WriteLog(errMessage, LogService.TypeLevel.Error, ex);
            }

            return result;
        }

        public List<MessageMachineModel> GetMaintenanceNotifications(MachineInfoModel machine, PeriodModel period,
            string userId)
        {
            var notificationsUser = GetMaintenanceMessages(machine, period);
            var notificationsRead = _context.Set<MessageMachineNotification>().Where(n => n.UserId == userId)
                .Select(nu => nu.IdMessageMachine).ToList();
            notificationsUser = notificationsUser.Where(o => notificationsRead.All(i => i != o.Id)).OrderBy(n => n.Day)
                .ToList();

            return notificationsUser;
        }


        public bool IgnoreMessage(int messageId)
        {
            try
            {
                var cur = _context.Set<MessageMachine>().Find(messageId);
                cur.IgnoreDate = DateTime.UtcNow;
                cur.UserId = _accountService.GetLoggedUser().ID.ToString();
                _context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                var errMessage = string.Format(ex.GetStringLog(),
                    messageId.ToString(), "");
                LogService.WriteLog(errMessage, LogService.TypeLevel.Error, ex);
            }

            return false;
        }

        public void SetNotificationAsRead(int id, string userId)
        {
            if (_context.Set<MessageMachineNotification>()
                .Any(m => m.IdMessageMachine == id &&
                          DbFunctions.TruncateTime(m.ReadingDate) == DbFunctions.TruncateTime(DateTime.UtcNow))) return;

            _context.Set<MessageMachineNotification>().Add(new MessageMachineNotification
            {
                IdMessageMachine = id,
                ReadingDate = DateTime.UtcNow,
                UserId = userId
            });

            _context.SaveChanges();
        }

        /// Per ogni macchina non scaduta e con data di attivazione valida estrae i messaggi periodici da controllare 
        /// e per ognuno se non esiste già nella tabella MessageMachine, inserisce un nuovo messaggio periodico.
        public void CheckMaintenance()
        {
            string MachinId = null;
            try
            {
                var machines = _machineRepository.Get(m =>
                    m.ExpirationDate != null && m.ExpirationDate >= DateTime.UtcNow &&
                    m.ActivationDate != null && m.ActivationDate <= DateTime.UtcNow).ToList();

                foreach (var machine in machines)
                {
                    MachinId = machine.Id.ToString();
                    var cat = _machineRepository.GetByID(machine.Id).MachineModel.MessageCategoryId;

                    var messaggi = _messagesIndexRepository
                        .Get(mi => mi.MessageCategoryId == cat && 
                                   mi.IsPeriodicM && mi.PeriodicSpan != null &&
                                   mi.IsDisabled == false &&
                                   mi.IsVisibleLOLA == true).ToList();

                    foreach (var messaggio in messaggi)
                    {
                        //messaggi periodici in base alla data di attivazione e allo span specificato nel messageIndex
                        var mess = _messageMachineRepository.Get(mm =>
                            mm.MachineId == machine.Id && mm.MessagesIndex.IsPeriodicM &&
                            mm.MessagesIndex.MessageCode == messaggio.MessageCode, o => o.OrderByDescending(i => i.Id)).FirstOrDefault();

                        // i messaggi delle macchine al cui modello è associato un messaggio di default 
                        // hanno come data la data di attivazione della macchina
                        var data = (DateTime) machine.ActivationDate;

                        data = (DateTime) machine.ActivationDate?.AddHours((long) messaggio.PeriodicSpan);

                        if (mess == null)
                            InsertMessageMachine(machine, messaggio.MessageCode, data);
                        else if (mess.IgnoreDate != null)
                        {
                            DateTime nd = (DateTime) mess.IgnoreDate?.AddHours((long) messaggio.PeriodicSpan);
                            if (DateTime.UtcNow > nd)
                            {
                                InsertMessageMachine(machine, messaggio.MessageCode, nd);
                            }
                        }
                        else if (messaggio.PeriodicSpan > 0)
                        {
                            DateTime next = (DateTime)mess.Day?.AddHours((long)messaggio.PeriodicSpan);
                            if (DateTime.UtcNow > next)
                            {
                                InsertMessageMachine(machine, messaggio.MessageCode, next);
                            }
                            
                        }

                        _context.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                var errMessage = string.Format(ex.GetStringLog(),
                    MachinId, "");
                LogService.WriteLog(errMessage, LogService.TypeLevel.Error, ex);
            }
        }

        public void InsertMessageMachine(Machine machine, string code, DateTime day)
        {
            if (string.IsNullOrWhiteSpace(code)) return;
            var msgIndex = _context.Set<MessagesIndex>().FirstOrDefault(mi =>
                mi.MessageCode == code && mi.MessageCategory.Id == machine.MachineModel.MessageCategoryId);
            if (msgIndex == null) return;
            var msg = new MessageMachine
            {
                MachineId = machine.Id,
                Day = day,
                MessagesIndexId = msgIndex?.Id,
                MessagesIndex = msgIndex
            };
            _context.Set<MessageMachine>().Add(msg);
        }


        private List<HistoryMessageModel> BuildAggregationList(IEnumerable<HistoryMessage> queryResult,
            string typeHistory,
            Func<HistoryMessage, int?> periodFunc)
        {
            var groups = queryResult.GroupBy(hs => new
            {
                hs.MachineId,
                hs.MessagesIndex?.MessageTypeId,
                Period = periodFunc.Invoke(hs)
            });

            var result = groups.Select(s => new HistoryMessageModel
            {
                Type = s.Key.MessageTypeId,
                Count = s.Sum(x => x.Count),
                Day = s.Max(i => i.Day),
                MachineId = s.Key.MachineId,
                Period = s.Key.Period,
                TypeHistory = typeHistory
            }).ToList();

            return result;
        }

        private static int GetQuarter(DateTime fromDate)
        {
            var month = fromDate.Month - 1;
            var month2 = Math.Abs(month / 3) + 1;
            return month2;
        }
    }
}