using FomMonitoringCore.DAL;
using FomMonitoringCore.Framework.Common;
using FomMonitoringCore.Framework.Model;
using FomMonitoringCore.Repository;
using Mapster;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FomMonitoringCore.Service
{
    public class MessageService : IMessageService
    {
        private IMessageMachineRepository _messageMachineRepository;
        private IMachineRepository _machineRepository;
        private IMessagesIndexRepository _messagesIndexRepository;
        private IFomMonitoringEntities _context;

        public MessageService(IMessageMachineRepository messageMachineRepository, IMachineRepository machineRepository, IMessagesIndexRepository messagesIndexRepository, IFomMonitoringEntities context)
        {
            _messageMachineRepository = messageMachineRepository;
            _machineRepository = machineRepository;
            _messagesIndexRepository = messagesIndexRepository;
            _context = context;
        }

        #region SP AGGREGATION

        /// <summary>
        /// Ritorna gli allarmi n base a tipo di dati da visualizzare in base al tipo di aggregazione
        /// </summary>
        /// <param name="machine"></param>
        /// <param name="period"></param>
        /// <param name="dataType"></param>
        /// <returns>Lista dei dettagli degli stati</returns>
        public List<HistoryMessageModel> GetAggregationMessages_SP(MachineInfoModel machine, PeriodModel period, enDataType dataType)
        {
            var result = new List<HistoryMessageModel>();

            try
            {                
                    var query = _context.usp_AggregationMessage(machine.Id, period.StartDate, period.EndDate, (int)period.Aggregation, (int)dataType).ToList();
                    result = query.Adapt<List<HistoryMessageModel>>();                
            }
            catch (Exception ex)
            {
                var errMessage = string.Format(ex.GetStringLog(),
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
        public List<HistoryMessageModel> GetAggregationMessages(MachineInfoModel machine, PeriodModel period, enDataType dataType)
        {
            try
            {                
                    switch (dataType)
                    {
                        case enDataType.Historical:
                            if (period.Aggregation == enAggregation.Day)
                            {
                                var historyMessages = _context.Set<HistoryMessage>()
                                    .Where(hm => hm.MachineId == machine.Id
                                        && hm.Day.Value >= period.StartDate && hm.Day.Value <= period.EndDate).ToList()
                                    .GroupBy(g => new
                                    {
                                        g.Day,                                      
                                        g.MachineId,                                       
                                        g.Type,
                                        g.Period
                                    }).ToList()
                                    .Select(s => new AggregationMessageModel
                                    {
                                        Count = s.Sum(x => x.Count),
                                        Day = s.Key.Day,
                                        MachineId = s.Key.MachineId,
                                        Period = s.Key.Period,
                                        TypeHistory = "d",
                                        Type = s.Key.Type
                                    }).ToList();

                                return historyMessages.Adapt<List<HistoryMessageModel>>();
                            }
                            if (period.Aggregation == enAggregation.Week)
                            {
                                var historyMessagesWeek = _context.Set <HistoryMessage>()
                                    .Where(hm => hm.MachineId == machine.Id && hm.Day >= period.StartDate
                                        && hm.Day <= period.EndDate && hm.Code != null).ToList()
                                    .GroupBy(g => new
                                    {
                                        Year = g.Day.HasValue ? (int?)g.Day.Value.Year : null,
                                        Week = g.Day.Value.GetWeekNumber(),
                                        g.MachineId,                                       
                                        g.Type,
                                        Period = g.Day.HasValue ? (int?)g.Day.Value.Year * 100 + g.Day.Value.GetWeekNumber() : null,

                                    }).ToList().Select(s => new AggregationMessageModel
                                    {                                                                              
                                        Count = s.Sum(x => x.Count),
                                        Day = s.Max(i => i.Day),
                                        Type = s.Key.Type,
                                        MachineId = s.Key.MachineId,                                   
                                        Period = s.Key.Period,
                                        TypeHistory = "w"
                                    }).ToList();

                                return historyMessagesWeek.Adapt<List<HistoryMessageModel>>();
                            }
                            if (period.Aggregation == enAggregation.Month)
                            {
                                var historyMessagesMonth = _context.Set<HistoryMessage>()
                                    .Where(hm => hm.MachineId == machine.Id
                                        && hm.Day >= period.StartDate && hm.Day <= period.EndDate && hm.Code != null).ToList()
                                    .GroupBy(g => new
                                    {
                                        Year = g.Day.HasValue ? (int?)g.Day.Value.Year : null,
                                        Week = g.Day.Value.Month,
                                        g.MachineId,                                                                 
                                        g.Type,
                                        Period = g.Day.HasValue ? (int?)g.Day.Value.Year * 100 + g.Day.Value.Month : null,

                                    }).ToList().Select(s => new AggregationMessageModel
                                    {                                        
                                        Type = s.Key.Type,
                                        Count = s.Sum(x => x.Count),
                                        Day = s.Max(i => i.Day),
                                        MachineId = s.Key.MachineId,
                                        Period = s.Key.Period,
                                        TypeHistory = "m"
                                    }).ToList();

                                return historyMessagesMonth.Adapt<List<HistoryMessageModel>>();
                            }
                            if (period.Aggregation == enAggregation.Quarter)
                            {
                                var historyMessagesQuarter = _context.Set<HistoryMessage>()
                                    .Where(hm => hm.MachineId == machine.Id
                                        && hm.Day >= period.StartDate && hm.Day <= period.EndDate && hm.Code != null).ToList()
                                    .GroupBy(g => new
                                    {
                                        Year = g.Day.HasValue ? (int?)g.Day.Value.Year : null,
                                        Week = g.Day.Value.Month,
                                        g.MachineId,
                                        g.Type,
                                        Period = g.Day.HasValue ? (int?)g.Day.Value.Year * 100 + GetQuarter(g.Day ?? DateTime.UtcNow) : null,

                                    }).ToList().Select(s => new AggregationMessageModel
                                    {                                                                   
                                        Count = s.Sum(x => x.Count),
                                        Day = s.Max(i => i.Day),
                                        //ElapsedTime = s.Sum(i => i.ElapsedTime),
                                        MachineId = s.Key.MachineId,
                                        Period = s.Key.Period,
                                        TypeHistory = "q",
                                        Type = s.Key.Type
                                    }).ToList();

                                return historyMessagesQuarter.Adapt<List<HistoryMessageModel>>();
                            }
                            if (period.Aggregation == enAggregation.Year)
                            {
                                var historyMessagesYear = _context.Set<HistoryMessage>()
                                    .Where(hm => hm.MachineId == machine.Id
                                        && hm.Day >= period.StartDate && hm.Day <= period.EndDate && hm.Code != null).ToList()
                                    .GroupBy(g => new
                                    {
                                        Year = g.Day.HasValue ? (int?)g.Day.Value.Year : null,
                                        Week = g.Day.Value.Month,
                                        g.MachineId,
                                        g.Type,
                                        Period = g.Day.HasValue ? (int?)g.Day.Value.Year : null,

                                    }).ToList().Select(s => new AggregationMessageModel
                                    {                                        
                                        Count = s.Sum(x => x.Count),
                                        Day = s.Max(i => i.Day),
                                        //ElapsedTime = s.Sum(i => i.ElapsedTime),
                                        MachineId = s.Key.MachineId,
                                        Period = s.Key.Period,
                                        TypeHistory = "y",
                                        Type = s.Key.Type
                                    }).ToList();

                                return historyMessagesYear.Adapt<List<HistoryMessageModel>>();
                            }
                            break;
                        case enDataType.Summary:
                            var historyMessagesSummary = _context.Set<HistoryMessage>()
                                .Where(hm => hm.MachineId == machine.Id && hm.Day >= period.StartDate && hm.Day <= period.EndDate && hm.Code != null).ToList()
                                .GroupBy(g => new
                                {
                                    g.MachineId,
                                    //g.Params,
                                    //g.Group,
                                    //g.StateId,
                                    g.Code

                                }).ToList().Select(s => new AggregationMessageModel
                                {
                                    Id = s.Max(m => m.Id),
                                    Code = s.Key.Code,
                                    //StateId = s.Key.StateId,
                                    Count = s.Sum(i => i.Count),
                                    Day = s.Max(i => i.Day),
                                    //ElapsedTime = s.Sum(i => i.ElapsedTime),
                                    //Group = s.Key.Group,
                                    MachineId = s.Key.MachineId,
                                    //Params = s.Key.Params,
                                    TypeHistory = "y"
                                }).ToList();

                            return historyMessagesSummary.Adapt<List<HistoryMessageModel>>();
                    }                
            }
            catch (Exception ex)
            {
                var errMessage = string.Format(ex.GetStringLog(),
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
            var month = fromDate.Month - 1;
            var month2 = Math.Abs(month / 3) + 1;
            return month2;
        }

        /// <summary>
        /// Ritorna i dettagli degli allarmi in base a macchina e periodo
        /// </summary>
        /// <param name="machine"></param>
        /// <param name="period"></param>
        /// <returns>Lista dei dettagli degli stati</returns>
        public List<HistoryMessageModel> GetAllHistoryMessages(MachineInfoModel machine, PeriodModel period)
        {
            var result = new List<HistoryMessageModel>();

            try
            {                
                var aggType = period.Aggregation.GetDescription();

                var query = (from hs in _context.Set<HistoryMessage>()
                                              where hs.MachineId == machine.Id
                                                && hs.Day >= period.StartDate && hs.Day <= period.EndDate
                                                && hs.TypeHistory == aggType
                                                select hs).ToList();

                result = query.Adapt<List<HistoryMessageModel>>();
                
            }
            catch (Exception ex)
            {
                var errMessage = string.Format(ex.GetStringLog(),
                    machine.Id.ToString(),
                    string.Concat(period.StartDate.ToString(), " - ", period.EndDate.ToString(), " - ", period.Aggregation.ToString()));
                LogService.WriteLog(errMessage, LogService.TypeLevel.Error, ex);
            }

            return result;
        }

        public List<MessageMachineModel> GetMessageDetails(MachineInfoModel machine, PeriodModel period)
        {
            var result = new List<MessageMachineModel>();

            try
            {

                    var query = _messageMachineRepository.GetMachineMessages(machine.Id, period.StartDate, period.EndDate).ToList();

                var cat = _machineRepository.GetByID(machine.Id).MachineModel.MessageCategoryId;
                var msgs = _messagesIndexRepository.Get(m => m.MessageCategoryId == cat, tracked: false);

                query = query.Where(m =>
                    {
                        var msg = msgs.FirstOrDefault(i => i.MessageCode.Trim().Equals(m.Code.Trim(), StringComparison.InvariantCultureIgnoreCase));
                        if (msg == null) return false;
                        
                        return msg.IsVisibleLOLA;
                    }).ToList();

                    

                    result = query.Adapt<List<MessageMachine>, List<MessageMachineModel>>();
                
            }
            catch (Exception ex)
            {
                var errMessage = string.Format(ex.GetStringLog(),
                    machine.Id.ToString(),
                    string.Concat(period.StartDate.ToString(), " - ", period.EndDate.ToString(), " - ", period.Aggregation.ToString()));
                LogService.WriteLog(errMessage, LogService.TypeLevel.Error, ex);
            }

            return result;
        }

        public long? GetExpiredSpan(MessageMachineModel mm)
        {            
            if(mm.IsPeriodicMsg == true)
            {
                var cat = _context.Set<Machine>().Find(mm.MachineId).MachineModel.MessageCategoryId;
                var msg = _context.Set<MessagesIndex>().FirstOrDefault(f => f.MessageCode == mm.Code && f.MessageCategoryId == cat && f.PeriodicSpan != null);
                if (msg == null) return null;
                var span = msg.PeriodicSpan ?? 0;

                var initTime = _context.Set<Machine>().Find(mm.MachineId).ActivationDate?.AddHours(span);
                if(mm.IgnoreDate != null)
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

        public List<MessageMachineModel> GetMaintenanceMessages(MachineInfoModel machine, PeriodModel period)
        {
            var result = new List<MessageMachineModel>();

            try
            {                
                    var query = _context.Set<MessageMachine>().Where(m => m.MachineId == machine.Id &&                                        
                                        m.Machine.ActivationDate != null &&
                                        m.IsPeriodicMsg != null && m.IsPeriodicMsg == true).OrderByDescending(o => o.Day).ToList();
      
                    query = query.Where(m =>
                    {
                        var cat = _context.Set<Machine>().Find(m.MachineId)?.MachineModel.MessageCategoryId;
                        var msg = _context.Set<MessagesIndex>().FirstOrDefault(f => f.MessageCode == m.Code && f.MessageCategoryId == cat);
                        if (msg == null) return false;
                        var span = msg.PeriodicSpan ?? 0;
                                     
                        return (m.IgnoreDate == null && span > 0 && m.Machine.ActivationDate?.AddHours(span) <= DateTime.UtcNow) ||
                               (m.IgnoreDate != null && span > 0 && m.IgnoreDate < m.GetInitialSpanDate(span)) ||
                               (m.IgnoreDate == null && span == 0);                      

                    }).ToList();
                  
                    result = query.Adapt<List<MessageMachine>, List<MessageMachineModel>>();                
            }
            catch (Exception ex)
            {
                var errMessage = string.Format(ex.GetStringLog(),
                    machine.Id.ToString(),
                    string.Concat(period.StartDate.ToString(), " - ", period.EndDate.ToString(), " - ", period.Aggregation.ToString()));
                LogService.WriteLog(errMessage, LogService.TypeLevel.Error, ex);
            }

            return result;
        }

        public List<MessageMachineModel> GetMaintenanceNotifications(MachineInfoModel machine, PeriodModel period, string userId)
        {
            var notificationsUser = GetMaintenanceMessages(machine, period);
            var notificationsRead = _context.Set<MessageMachineNotification>().Where(n => n.UserId == userId)
                .Select(nu => nu.IdMessageMachine).ToList();
            notificationsUser = notificationsUser.Where(o => notificationsRead.All(i => i != o.Id)).ToList();
            return notificationsUser;
        }


        public List<MessageMachineModel> GetAllCurrentMessages(MachineInfoModel machine, PeriodModel period)
        {
            var result = new List<MessageMachineModel>();

            try
            {                
                    var query = (from hs in _context.Set<MessageMachine>()
                                                  where hs.MachineId == machine.Id
                                                  && hs.Day >= period.StartDate && hs.Day <= period.EndDate
                                                  select hs).ToList();

                    result = query.Adapt<List<MessageMachineModel>>();                
            }
            catch (Exception ex)
            {
                var errMessage = string.Format(ex.GetStringLog(),
                    machine.Id.ToString(),
                    string.Concat(period.StartDate.ToString(), " - ", period.EndDate.ToString(), " - ", period.Aggregation.ToString()));
                LogService.WriteLog(errMessage, LogService.TypeLevel.Error, ex);
            }
            return result;
        }

        public bool IgnoreMessage(int messageId)
        {
            try
            {                
                _context.Set<MessageMachine>().Find(messageId).IgnoreDate = DateTime.UtcNow;
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

                    var machines = _machineRepository.Get(m => m.ExpirationDate != null && m.ExpirationDate >= DateTime.UtcNow && 
                                           m.ActivationDate != null && m.ActivationDate <= DateTime.UtcNow).ToList();
                    
                    foreach(var machine in machines)
                    {
                        MachinId = machine.Id.ToString();
                        var cat = _machineRepository.GetByID(machine.Id).MachineModel.MessageCategoryId;

                        var messaggi = _messagesIndexRepository.Get(mi => mi.MessageCategoryId == cat && mi.IsPeriodicM == true && mi.PeriodicSpan != null).ToList();

                        foreach (var messaggio in messaggi)
                        {
                            //messaggi periodici in base alla data di attivazione e allo span specificato nel messageIndex
                            var mess = _messageMachineRepository.GetFirstOrDefault(mm => mm.MachineId == machine.Id && mm.IsPeriodicMsg == true &&
                                                         mm.Code == messaggio.MessageCode);

                            // i messaggi delle macchine al cui modello è associato un messaggio di default 
                            // hanno come data la data di attivazione della macchina
                            var data = (DateTime)machine.ActivationDate;

                            data = (DateTime)machine.ActivationDate?.AddHours((long)messaggio.PeriodicSpan);

                            if (mess == null)
                            {                                
                                InsertMessageMachine(machine, messaggio.MessageCode, data);                             
                            }
                            else if(mess.IgnoreDate != null)
                            {
                                // aggiorno la data di scadenza all'intervallo 
                                mess.Day = (DateTime)mess.IgnoreDate?.AddHours((long)messaggio.PeriodicSpan);
                            _context.SaveChanges();
                            }                            
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
            var msg = new MessageMachine()
            {
                Code = code,
                MachineId = machine.Id,
                IsPeriodicMsg = true,
                Day = day,
                IsVisible = true
            };
            _context.Set<MessageMachine>().Add(msg);
        }

    }
}
