using FomMonitoringCore.DAL;
using FomMonitoringCore.DAL_SQLite;
using FomMonitoringCore.Framework.Common;
using FomMonitoringCore.Framework.Model;
using FomMonitoringCore.Uow;
using Mapster;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;

namespace FomMonitoringCore.Service.DataMapping
{
    public class JsonToSQLiteService : IJsonToSQLiteService
    {
        private IFomMonitoringEntities _FomMonitoringEntities;
        private IFomMonitoringSQLiteEntities _FomMonitoringSQLiteEntities;
        private IUnitOfWork _unitOfWork;
        
        public JsonToSQLiteService(IFomMonitoringEntities FomMonitoringEntities, IFomMonitoringSQLiteEntities FomMonitoringSQLiteEntities,
                            IUnitOfWork unitOfWork)
        {
            _FomMonitoringEntities = FomMonitoringEntities;
            _FomMonitoringSQLiteEntities = FomMonitoringSQLiteEntities;
            _unitOfWork = unitOfWork;
        }

        public List<JsonDataModel> GetAllJsonDataNotElaborated()
        {
            List<JsonDataModel> result = new List<JsonDataModel>();
            try
            {
                List<JsonData> jsonData = new List<JsonData>();                
                jsonData = _FomMonitoringEntities.Set<JsonData>().Where(w => !w.IsElaborated).OrderBy(o => o.Id).ToList();
                result = jsonData.Adapt<List<JsonDataModel>>();
                
            }
            catch (Exception ex)
            {
                string errMessage = string.Format(ex.GetStringLog());
                LogService.WriteLog(errMessage, LogService.TypeLevel.Error, ex);
            }
            return result;
        }

        public bool MappingJsonDetailsToSQLite(JsonDataModel jsonDataModel)
        {
            bool result = false;
            try
            {
                JObject json = JsonConvert.DeserializeObject<JObject>(jsonDataModel.Json);
                List<bar> barSQLite = new List<bar>();
                List<error> errorSQLite = new List<error>();
                List<historyJob> historyJobSQLite = new List<historyJob>();
                List<info> infoSQLite = new List<info>();
                List<piece> pieceSQLite = new List<piece>();
                List<message> messageSQLite = new List<message>();
                List<spindle> spindleSQLite = new List<spindle>();
                List<state> stateSQLite = new List<state>();
                List<tool> toolSQLite = new List<tool>();
                _unitOfWork.StartTransaction(_FomMonitoringSQLiteEntities);
                
               /* _FomMonitoringSQLiteEntities.Database.ExecuteSqlCommand("TRUNCATE TABLE bar");
                _FomMonitoringSQLiteEntities.Database.ExecuteSqlCommand("TRUNCATE TABLE error");
                _FomMonitoringSQLiteEntities.Database.ExecuteSqlCommand("TRUNCATE TABLE historyJob");
                _FomMonitoringSQLiteEntities.Database.ExecuteSqlCommand("TRUNCATE TABLE info");
                _FomMonitoringSQLiteEntities.Database.ExecuteSqlCommand("TRUNCATE TABLE piece");
                _FomMonitoringSQLiteEntities.Database.ExecuteSqlCommand("TRUNCATE TABLE spindle");
                _FomMonitoringSQLiteEntities.Database.ExecuteSqlCommand("TRUNCATE TABLE state");
                _FomMonitoringSQLiteEntities.Database.ExecuteSqlCommand("TRUNCATE TABLE tool");
                _FomMonitoringSQLiteEntities.Database.ExecuteSqlCommand("TRUNCATE TABLE message");*/
                _FomMonitoringSQLiteEntities.Set<bar>().RemoveRange(_FomMonitoringSQLiteEntities.Set<bar>());
                _FomMonitoringSQLiteEntities.Set<error>().RemoveRange(_FomMonitoringSQLiteEntities.Set<error>());
                _FomMonitoringSQLiteEntities.Set<historyJob>().RemoveRange(_FomMonitoringSQLiteEntities.Set<historyJob>());
                _FomMonitoringSQLiteEntities.Set<info>().RemoveRange(_FomMonitoringSQLiteEntities.Set<info>());
                _FomMonitoringSQLiteEntities.Set<piece>().RemoveRange(_FomMonitoringSQLiteEntities.Set<piece>());
                _FomMonitoringSQLiteEntities.Set<spindle>().RemoveRange(_FomMonitoringSQLiteEntities.Set<spindle>());
                _FomMonitoringSQLiteEntities.Set<state>().RemoveRange(_FomMonitoringSQLiteEntities.Set<state>());
                _FomMonitoringSQLiteEntities.Set<tool>().RemoveRange(_FomMonitoringSQLiteEntities.Set<tool>());
                _FomMonitoringSQLiteEntities.Set<message>().RemoveRange(_FomMonitoringSQLiteEntities.Set<message>());
              

                foreach (JToken token in json.Root)
                    {
                        switch (token.Path.ToLower())
                        {
                            case "bar":
                                barSQLite = JsonConvert.DeserializeObject<List<bar>>(JsonConvert.SerializeObject(token.First));
                                foreach (bar bar in barSQLite)
                                {
                                    bar.StartTime = bar.StartTime.HasValue && bar.StartTime.Value.Year < 1900 ? null : bar.StartTime;
                                    bar.EndTime = bar.EndTime.HasValue && bar.EndTime.Value.Year < 1900 ? null : bar.EndTime;
                                }
                                break;
                            case "error":
                                errorSQLite = JsonConvert.DeserializeObject<List<error>>(JsonConvert.SerializeObject(token.First));
                                foreach (error error in errorSQLite)
                                {
                                    error.Time = error.Time.HasValue && error.Time.Value.Year < 1900 ? null : error.Time;
                                }
                                break;
                            case "historyjob":
                                historyJobSQLite = JsonConvert.DeserializeObject<List<historyJob>>(JsonConvert.SerializeObject(token.First));
                                foreach (historyJob historyJob in historyJobSQLite)
                                {
                                    historyJob.Day = historyJob.Day.HasValue && historyJob.Day.Value.Year < 1900 ? null : historyJob.Day;
                                }
                                break;
                            case "info":
                                infoSQLite = JsonConvert.DeserializeObject<List<info>>(JsonConvert.SerializeObject(token.First));
                                foreach (info info in infoSQLite)
                                {
                                    info.LoginDate = info.LoginDate.HasValue && info.LoginDate.Value.Year < 1900 ? null : info.LoginDate;
                                    //info.InstallationDate = info.InstallationDate.HasValue && info.InstallationDate.Value.Year < 1900 ? null : info.InstallationDate;
                                    //info.NextMaintenanceService = info.NextMaintenanceService.HasValue && info.NextMaintenanceService.Value.Year < 1900 ? null : info.NextMaintenanceService;
                                    //campi cablati che non arrivano più dal json
                                    info.StateProductivityGreenThreshold = 63.0;
                                    info.StateProductivityYellowThreshold = 40.0;
                                    info.PiecesProductivityGreenThreshold = 10.0;
                                    info.PiecesProductivityYellowThreshold = 25.0;
                                    info.BarsProductivityGreenThreshold = 90.0;
                                    info.BarsProductivityYellowThreshold = 50.0;
                                    info.OverfeedGreenThreshold = 85.0;
                                    info.OverfeedYellowThreshold = 50.0;
                                    info.Shift1StartHour = 0;
                                    info.Shift1StartMinute = 0;
                                    info.Shift2StartHour = 8;
                                    info.Shift2StartMinute = 0;
                                    info.Shift3StartHour = 16;
                                    info.Shift3StartMinute = 0;
                                }
                                break;
                            case "piece":
                                pieceSQLite = JsonConvert.DeserializeObject<List<piece>>(JsonConvert.SerializeObject(token.First));
                                foreach (piece piece in pieceSQLite)
                                {
                                    piece.EndTime = piece.EndTime.HasValue && piece.EndTime.Value.Year < 1900 ? null : piece.EndTime;
                                    piece.StartTimeCut = piece.StartTimeCut.HasValue && piece.StartTimeCut.Value.Year < 1900 ? null : piece.StartTimeCut;
                                    piece.EndTimeCut = piece.EndTimeCut.HasValue && piece.EndTimeCut.Value.Year < 1900 ? null : piece.EndTimeCut;
                                    piece.StartTimeWorking = piece.StartTimeWorking.HasValue && piece.StartTimeWorking.Value.Year < 1900 ? null : piece.StartTimeWorking;
                                    piece.EndTimeWorking = piece.EndTimeWorking.HasValue && piece.EndTimeWorking.Value.Year < 1900 ? null : piece.EndTimeWorking;
                                    piece.StartTimeTrim = piece.StartTimeTrim.HasValue && piece.StartTimeTrim.Value.Year < 1900 ? null : piece.StartTimeTrim;
                                    piece.EndTimeTrim = piece.EndTimeTrim.HasValue && piece.EndTimeTrim.Value.Year < 1900 ? null : piece.EndTimeTrim;
                                    piece.StartTimeAnuba = piece.StartTimeAnuba.HasValue && piece.StartTimeAnuba.Value.Year < 1900 ? null : piece.StartTimeAnuba;
                                    piece.EndTimeAnuba = piece.EndTimeAnuba.HasValue && piece.EndTimeAnuba.Value.Year < 1900 ? null : piece.EndTimeAnuba;
                                }
                                break;
                            case "spindle":
                                spindleSQLite = JsonConvert.DeserializeObject<List<spindle>>(JsonConvert.SerializeObject(token.First));
                                foreach (spindle spindle in spindleSQLite)
                                {
                                    spindle.InstallDate = spindle.InstallDate.HasValue && spindle.InstallDate.Value.Year < 1900 ? null : spindle.InstallDate;
                                    spindle.Replaced = spindle.Replaced.HasValue && spindle.Replaced.Value.Year < 1900 ? null : spindle.Replaced;
                                }
                                break;
                            case "state":
                                string jsonSerialized = JsonConvert.SerializeObject(token.First).Replace("\"State\":", "\"State1\":");
                                stateSQLite = JsonConvert.DeserializeObject<List<state>>(jsonSerialized);
                                foreach (state state in stateSQLite)
                                {
                                    state.StartTime = state.StartTime.HasValue && state.StartTime.Value.Year < 1900 ? null : state.StartTime;
                                    state.EndTime = state.EndTime.HasValue && state.EndTime.Value.Year < 1900 ? null : state.EndTime;
                                    if (state.StartTime != null && state.EndTime != null)
                                        state.TimeSpanDuration = state.EndTime?.Subtract((DateTime)state.StartTime).Ticks;
                                    else
                                        state.TimeSpanDuration = null;
                                }
                                break;
                            case "tool":
                                toolSQLite = JsonConvert.DeserializeObject<List<tool>>(JsonConvert.SerializeObject(token.First));
                                foreach (tool tool in toolSQLite)
                                {
                                    tool.DateLoaded = tool.DateLoaded.HasValue && tool.DateLoaded.Value.Year < 1900 ? null : tool.DateLoaded;
                                    tool.DateReplaced = tool.DateReplaced.HasValue && tool.DateReplaced.Value.Year < 1900 ? null : tool.DateReplaced;
                                }
                                break;
                            case "message":
                                List<MessageModel>messageModels = JsonConvert.DeserializeObject<List<MessageModel>>(JsonConvert.SerializeObject(token.First));
                                messageSQLite = messageModels.Select(m => new message()
                                {
                                    Id = m.Id,
                                    Time = m.Time.HasValue && m.Time.Value.Year < 1900 ? null : m.Time,
                                    Code = m.Code,
                                    Group = m.Group,
                                    Operator = m.Operator,
                                    Params = m.Parameters == null ? "" : JsonConvert.SerializeObject(m.Parameters),
                                    State = m.State,
                                    Type = m.Type
                                }).ToList();

                                    
                                break;
                            default:
                                break;
                        }
                    }

                _FomMonitoringSQLiteEntities.Set<bar>().AddRange(barSQLite);
                _FomMonitoringSQLiteEntities.SaveChanges();

                _FomMonitoringSQLiteEntities.Set<error>().AddRange(errorSQLite);
                _FomMonitoringSQLiteEntities.SaveChanges();

                _FomMonitoringSQLiteEntities.Set<historyJob>().AddRange(historyJobSQLite);
                _FomMonitoringSQLiteEntities.SaveChanges();

                _FomMonitoringSQLiteEntities.Set<info>().AddRange(infoSQLite);
                _FomMonitoringSQLiteEntities.SaveChanges();

                _FomMonitoringSQLiteEntities.Set<piece>().AddRange(pieceSQLite);
                _FomMonitoringSQLiteEntities.SaveChanges();

                _FomMonitoringSQLiteEntities.Set<spindle>().AddRange(spindleSQLite);
                _FomMonitoringSQLiteEntities.SaveChanges();

                _FomMonitoringSQLiteEntities.Set<state>().AddRange(stateSQLite);
                _FomMonitoringSQLiteEntities.SaveChanges();

                _FomMonitoringSQLiteEntities.Set<tool>().AddRange(toolSQLite);
                _FomMonitoringSQLiteEntities.SaveChanges();

                _FomMonitoringSQLiteEntities.Set<message>().AddRange(messageSQLite);
                _FomMonitoringSQLiteEntities.SaveChanges();

                _unitOfWork.CommitTransaction();
                result = true;                
            }
            catch (Exception ex)
            {
                string errMessage = string.Format(ex.GetStringLog(), jsonDataModel.Id.ToString());
                LogService.WriteLog(errMessage, LogService.TypeLevel.Error, ex);
                _unitOfWork.RollbackTransaction();
            }
            return result;
        }

        public bool MappingJsonHistoryToSQLite(JsonDataModel jsonDataModel)
        {
            bool result = false;
            try
            {
                JObject json = JsonConvert.DeserializeObject<JObject>(jsonDataModel.Json);
                List<historyBar> historyBarSQLite = new List<historyBar>();
                List<historyPiece> historyPieceSQLite = new List<historyPiece>();
                List<historyJob> historyJobSQLite = new List<historyJob>();
                List<info> infoSQLite = new List<info>();
                List<historyAlarm> historyAlarmSQLite = new List<historyAlarm>();
                List<historyMessage> historyMessageSQLite = new List<historyMessage>();
                List<historyState> historyStateSQLite = new List<historyState>();
                List<spindle> spindleSQLite = new List<spindle>();
                List<tool> toolSQLite = new List<tool>();
                List<message> messageSQLite = new List<message>();

                _unitOfWork.StartTransaction(_FomMonitoringSQLiteEntities);

                _FomMonitoringSQLiteEntities.Database.ExecuteSqlCommand("TRUNCATE TABLE historyBar");
                _FomMonitoringSQLiteEntities.Database.ExecuteSqlCommand("TRUNCATE TABLE historyPiece");
                _FomMonitoringSQLiteEntities.Database.ExecuteSqlCommand("TRUNCATE TABLE historyJob");
                _FomMonitoringSQLiteEntities.Database.ExecuteSqlCommand("TRUNCATE TABLE info");
                _FomMonitoringSQLiteEntities.Database.ExecuteSqlCommand("TRUNCATE TABLE historyAlarm");
                _FomMonitoringSQLiteEntities.Database.ExecuteSqlCommand("TRUNCATE TABLE historyState");
                _FomMonitoringSQLiteEntities.Database.ExecuteSqlCommand("TRUNCATE TABLE historyMessage");
                _FomMonitoringSQLiteEntities.Database.ExecuteSqlCommand("TRUNCATE TABLE spindle");
                _FomMonitoringSQLiteEntities.Database.ExecuteSqlCommand("TRUNCATE TABLE tool");
                _FomMonitoringSQLiteEntities.Database.ExecuteSqlCommand("TRUNCATE TABLE message");

                _FomMonitoringSQLiteEntities.SaveChanges();
                _unitOfWork.CommitTransaction();


                _unitOfWork.StartTransaction(_FomMonitoringSQLiteEntities);
                foreach (JToken token in json.Root)
                {
                    switch (token.Path.ToLower())
                    {
                        case "historybar":
                            historyBarSQLite = JsonConvert.DeserializeObject<List<historyBar>>(JsonConvert.SerializeObject(token.First));
                            foreach (var historyBar in historyBarSQLite)
                            {
                                historyBar.Day = historyBar.Day.HasValue && historyBar.Day.Value.Year < 1900 ? null : historyBar.Day;
                            }
                            break;
                        case "historypiece":
                            historyPieceSQLite = JsonConvert.DeserializeObject<List<historyPiece>>(JsonConvert.SerializeObject(token.First));
                            foreach (var historyPiece in historyPieceSQLite)
                            {
                                historyPiece.Day = historyPiece.Day.HasValue && historyPiece.Day.Value.Year < 1900 ? null : historyPiece.Day;
                            }
                            break;
                        case "historyjob":
                            historyJobSQLite = JsonConvert.DeserializeObject<List<historyJob>>(JsonConvert.SerializeObject(token.First));
                            foreach (var historyJob in historyJobSQLite)
                            {
                                historyJob.Day = historyJob.Day.HasValue && historyJob.Day.Value.Year < 1900 ? null : historyJob.Day;
                            }
                            break;
                        case "info":
                            infoSQLite = JsonConvert.DeserializeObject<List<info>>(JsonConvert.SerializeObject(token.First));
                            foreach (var info in infoSQLite)
                            {
                                info.LoginDate = info.LoginDate.HasValue && info.LoginDate.Value.Year < 1900 ? null : info.LoginDate;
                                info.InstallationDate = info.InstallationDate.HasValue && info.InstallationDate.Value.Year < 1900 ? null : info.InstallationDate;
                                info.NextMaintenanceService = info.NextMaintenanceService.HasValue && info.NextMaintenanceService.Value.Year < 1900 ? null : info.NextMaintenanceService;
                            }
                            break;
                        case "historyalarm":
                            historyAlarmSQLite = JsonConvert.DeserializeObject<List<historyAlarm>>(JsonConvert.SerializeObject(token.First));
                            foreach (var historyAlarm in historyAlarmSQLite)
                            {
                                historyAlarm.Day = historyAlarm.Day.HasValue && historyAlarm.Day.Value.Year < 1900 ? null : historyAlarm.Day;
                            }
                            break;
                        case "historymessage":
                            historyMessageSQLite = JsonConvert.DeserializeObject<List<historyMessage>>(JsonConvert.SerializeObject(token.First));
                            foreach (var historyMessage in historyMessageSQLite)
                            {
                                historyMessage.Day = historyMessage.Day.HasValue && historyMessage.Day.Value.Year < 1900 ? null : historyMessage.Day;
                            }
                            break;
                        case "historystate":
                            historyStateSQLite = JsonConvert.DeserializeObject<List<historyState>>(JsonConvert.SerializeObject(token.First));
                            foreach (var historyState in historyStateSQLite)
                            {
                                historyState.Day = historyState.Day.HasValue && historyState.Day.Value.Year < 1900 ? null : historyState.Day;
                            }
                            break;
                        case "spindle":
                            spindleSQLite = JsonConvert.DeserializeObject<List<spindle>>(JsonConvert.SerializeObject(token.First));
                            foreach (var spindle in spindleSQLite)
                            {
                                spindle.InstallDate = spindle.InstallDate.HasValue && spindle.InstallDate.Value.Year < 1900 ? null : spindle.InstallDate;
                                spindle.Replaced = spindle.Replaced.HasValue && spindle.Replaced.Value.Year < 1900 ? null : spindle.Replaced;
                            }
                            break;
                        case "tool":
                            toolSQLite = JsonConvert.DeserializeObject<List<tool>>(JsonConvert.SerializeObject(token.First));
                            foreach (var tool in toolSQLite)
                            {
                                tool.DateLoaded = tool.DateLoaded.HasValue && tool.DateLoaded.Value.Year < 1900 ? null : tool.DateLoaded;
                                tool.DateReplaced = tool.DateReplaced.HasValue && tool.DateReplaced.Value.Year < 1900 ? null : tool.DateReplaced;
                            }
                            break;
                        case "message":
                            messageSQLite = JsonConvert.DeserializeObject<List<message>>(JsonConvert.SerializeObject(token.First));
                            foreach (var message in messageSQLite)
                            {
                                message.Time = message.Time.HasValue && message.Time.Value.Year < 1900 ? null : message.Time;
                            }
                            break;
                        default:
                            break;
                    }
                }

                _FomMonitoringSQLiteEntities.Set<historyBar>().AddRange(historyBarSQLite);
                _FomMonitoringSQLiteEntities.SaveChanges();

                _FomMonitoringSQLiteEntities.Set<historyPiece>().AddRange(historyPieceSQLite);
                _FomMonitoringSQLiteEntities.SaveChanges();

                _FomMonitoringSQLiteEntities.Set<historyMessage>().AddRange(historyMessageSQLite);
                _FomMonitoringSQLiteEntities.SaveChanges();

                _FomMonitoringSQLiteEntities.Set<historyJob>().AddRange(historyJobSQLite);
                _FomMonitoringSQLiteEntities.SaveChanges();

                _FomMonitoringSQLiteEntities.Set<info>().AddRange(infoSQLite);
                _FomMonitoringSQLiteEntities.SaveChanges();

                _FomMonitoringSQLiteEntities.Set<historyAlarm>().AddRange(historyAlarmSQLite);
                _FomMonitoringSQLiteEntities.SaveChanges();

                _FomMonitoringSQLiteEntities.Set<historyState>().AddRange(historyStateSQLite);
                _FomMonitoringSQLiteEntities.SaveChanges();

                _FomMonitoringSQLiteEntities.Set<spindle>().AddRange(spindleSQLite);
                _FomMonitoringSQLiteEntities.SaveChanges();

                _FomMonitoringSQLiteEntities.Set<tool>().AddRange(toolSQLite);
                _FomMonitoringSQLiteEntities.SaveChanges();

                _FomMonitoringSQLiteEntities.Set<message>().AddRange(messageSQLite);
                _FomMonitoringSQLiteEntities.SaveChanges();

                _unitOfWork.CommitTransaction();
                result = true;                    
                
            }
            catch (Exception ex)
            {
                string errMessage = string.Format(ex.GetStringLog(), jsonDataModel.Id.ToString());
                LogService.WriteLog(errMessage, LogService.TypeLevel.Error, ex);
                _unitOfWork.RollbackTransaction();
            }
            return result;
        }

        public bool SaveElaboration(int id, bool IsLoaded)
        {
            bool result = false;
            try
            {                
                JsonData jsonData = _FomMonitoringEntities.Set<JsonData>().FirstOrDefault(f => f.Id == id);
                jsonData.ElaborationDate = DateTime.UtcNow;
                jsonData.IsElaborated = true;
                jsonData.IsLoaded = IsLoaded;
                _FomMonitoringEntities.SaveChanges();

                result = true;                
            }
            catch (Exception ex)
            {
                string errMessage = string.Format(ex.GetStringLog(), id.ToString(), IsLoaded.ToString());
                LogService.WriteLog(errMessage, LogService.TypeLevel.Error, ex);
            }
            return result;
        }
    }
}
