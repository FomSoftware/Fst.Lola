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
        
        public JsonToSQLiteService(IFomMonitoringEntities FomMonitoringEntities, IFomMonitoringSQLiteEntities FomMonitoringSQLiteEntities)
        {
            _FomMonitoringEntities = FomMonitoringEntities;
            _FomMonitoringSQLiteEntities = FomMonitoringSQLiteEntities;
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
                List<historyJob> historyJobSQLite = new List<historyJob>();
                List<info> infoSQLite = new List<info>();
                List<piece> pieceSQLite = new List<piece>();
                List<message> messageSQLite = new List<message>();
                List<spindle> spindleSQLite = new List<spindle>();
                List<state> stateSQLite = new List<state>();
                List<tool> toolSQLite = new List<tool>();
                
               /* _FomMonitoringSQLiteEntities.Database.ExecuteSqlCommand("TRUNCATE TABLE bar");
                _FomMonitoringSQLiteEntities.Database.ExecuteSqlCommand("TRUNCATE TABLE error");
                _FomMonitoringSQLiteEntities.Database.ExecuteSqlCommand("TRUNCATE TABLE historyJob");
                _FomMonitoringSQLiteEntities.Database.ExecuteSqlCommand("TRUNCATE TABLE info");
                _FomMonitoringSQLiteEntities.Database.ExecuteSqlCommand("TRUNCATE TABLE piece");
                _FomMonitoringSQLiteEntities.Database.ExecuteSqlCommand("TRUNCATE TABLE spindle");
                _FomMonitoringSQLiteEntities.Database.ExecuteSqlCommand("TRUNCATE TABLE state");
                _FomMonitoringSQLiteEntities.Database.ExecuteSqlCommand("TRUNCATE TABLE tool");
                _FomMonitoringSQLiteEntities.Database.ExecuteSqlCommand("TRUNCATE TABLE message");*/
                if(_FomMonitoringSQLiteEntities.Set<bar>() != null)
                    _FomMonitoringSQLiteEntities.Set<bar>().RemoveRange(_FomMonitoringSQLiteEntities.Set<bar>());

                if (_FomMonitoringSQLiteEntities.Set<historyJob>() != null)
                    _FomMonitoringSQLiteEntities.Set<historyJob>().RemoveRange(_FomMonitoringSQLiteEntities.Set<historyJob>());

                if (_FomMonitoringSQLiteEntities.Set<info>() != null)
                    _FomMonitoringSQLiteEntities.Set<info>().RemoveRange(_FomMonitoringSQLiteEntities.Set<info>());

                if (_FomMonitoringSQLiteEntities.Set<piece>() != null)
                    _FomMonitoringSQLiteEntities.Set<piece>().RemoveRange(_FomMonitoringSQLiteEntities.Set<piece>());

                if (_FomMonitoringSQLiteEntities.Set<spindle>() != null)
                    _FomMonitoringSQLiteEntities.Set<spindle>().RemoveRange(_FomMonitoringSQLiteEntities.Set<spindle>());

                if (_FomMonitoringSQLiteEntities.Set<state>() != null)
                    _FomMonitoringSQLiteEntities.Set<state>().RemoveRange(_FomMonitoringSQLiteEntities.Set<state>());

                if (_FomMonitoringSQLiteEntities.Set<tool>() != null)
                    _FomMonitoringSQLiteEntities.Set<tool>().RemoveRange(_FomMonitoringSQLiteEntities.Set<tool>());

                if (_FomMonitoringSQLiteEntities.Set<message>() != null)
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
                            case "historyjob":
                                historyJobSQLite = JsonConvert.DeserializeObject<List<historyJob>>(JsonConvert.SerializeObject(token.First));
                                foreach (historyJob historyJob in historyJobSQLite)
                                {
                                    historyJob.Id = 0;
                                    historyJob.Day = historyJob.Day.HasValue && historyJob.Day.Value.Year < 1900 ? null : historyJob.Day;
                                }
                                break;
                            case "info":
                                infoSQLite = JsonConvert.DeserializeObject<List<info>>(JsonConvert.SerializeObject(token.First));
                                foreach (info info in infoSQLite)
                                {
                                    info.Id = 0;
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
                                    piece.Id = 0;
                                    piece.EndTime = piece.EndTime.HasValue && piece.EndTime.Value.Year < 1900 ? null : piece.EndTime;
                                    piece.StartTime = piece.StartTime.HasValue && piece.StartTime.Value.Year < 1900 ? null : piece.StartTime;
                            }
                                break;
                            case "spindle":
                                spindleSQLite = JsonConvert.DeserializeObject<List<spindle>>(JsonConvert.SerializeObject(token.First));
                                foreach (spindle spindle in spindleSQLite)
                                {
                                    spindle.Id = 0;
                                    spindle.InstallDate = spindle.InstallDate.HasValue && spindle.InstallDate.Value.Year < 1900 ? null : spindle.InstallDate;
                                    spindle.Replaced = spindle.Replaced.HasValue && spindle.Replaced.Value.Year < 1900 ? null : spindle.Replaced;
                                }
                                break;
                            case "state":
                                string jsonSerialized = JsonConvert.SerializeObject(token.First);
                                stateSQLite = JsonConvert.DeserializeObject<List<state>>(jsonSerialized);
                                foreach (state state in stateSQLite)
                                {
                                    state.Id = 0;
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
                                    tool.Id = 0;
                                    tool.DateLoaded = tool.DateLoaded.HasValue && tool.DateLoaded.Value.Year < 1900 ? null : tool.DateLoaded;
                                    tool.DateReplaced = tool.DateReplaced.HasValue && tool.DateReplaced.Value.Year < 1900 ? null : tool.DateReplaced;
                                }
                                break;
                            case "message":
                                List<MessageModel>messageModels = JsonConvert.DeserializeObject<List<MessageModel>>(JsonConvert.SerializeObject(token.First));
                                messageSQLite = messageModels.Select(m => new message()
                                {
                                    Id = 0,
                                    Time = m.Time.HasValue && m.Time.Value.Year < 1900 ? null : m.Time,
                                    Code = m.Code,
                                    Operator = m.Operator,
                                    Params = m.Parameters == null ? "" : JsonConvert.SerializeObject(m.Parameters)
                                }).ToList();

                                    
                                break;
                            default:
                                break;
                        }
                    }

                if (barSQLite.Any())
                {
                    _FomMonitoringSQLiteEntities.Set<bar>().AddRange(barSQLite);
                    _FomMonitoringSQLiteEntities.SaveChanges();
                }
                if (historyJobSQLite.Any())
                {
                    _FomMonitoringSQLiteEntities.Set<historyJob>().AddRange(historyJobSQLite);
                    _FomMonitoringSQLiteEntities.SaveChanges();
                }
                if (infoSQLite.Any())
                {
                    _FomMonitoringSQLiteEntities.Set<info>().AddRange(infoSQLite);
                    _FomMonitoringSQLiteEntities.SaveChanges();
                }
                if (pieceSQLite.Any())
                {
                    _FomMonitoringSQLiteEntities.Set<piece>().AddRange(pieceSQLite);
                    _FomMonitoringSQLiteEntities.SaveChanges();
                }
                if (spindleSQLite.Any())
                {
                    _FomMonitoringSQLiteEntities.Set<spindle>().AddRange(spindleSQLite);
                    _FomMonitoringSQLiteEntities.SaveChanges();
                }
                if (stateSQLite.Any())
                {
                    _FomMonitoringSQLiteEntities.Set<state>().AddRange(stateSQLite);
                    _FomMonitoringSQLiteEntities.SaveChanges();
                }
                if (toolSQLite.Any())
                {
                    _FomMonitoringSQLiteEntities.Set<tool>().AddRange(toolSQLite);
                    _FomMonitoringSQLiteEntities.SaveChanges();
                }

                if (messageSQLite.Any())
                {
                    _FomMonitoringSQLiteEntities.Set<message>().AddRange(messageSQLite);
                    _FomMonitoringSQLiteEntities.SaveChanges();
                }

                result = true;                
            }
            catch (Exception ex)
            {
                string errMessage = string.Format(ex.GetStringLog(), jsonDataModel.Id.ToString());
                LogService.WriteLog(errMessage, LogService.TypeLevel.Error, ex);
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
                List<historyMessage> historyMessageSQLite = new List<historyMessage>();
                List<historyState> historyStateSQLite = new List<historyState>();
                List<spindle> spindleSQLite = new List<spindle>();
                List<tool> toolSQLite = new List<tool>();
                List<message> messageSQLite = new List<message>();
                

                _FomMonitoringSQLiteEntities.Database.ExecuteSqlCommand("TRUNCATE TABLE historyBar");
                _FomMonitoringSQLiteEntities.Database.ExecuteSqlCommand("TRUNCATE TABLE historyPiece");
                _FomMonitoringSQLiteEntities.Database.ExecuteSqlCommand("TRUNCATE TABLE historyJob");
                _FomMonitoringSQLiteEntities.Database.ExecuteSqlCommand("TRUNCATE TABLE info");
                _FomMonitoringSQLiteEntities.Database.ExecuteSqlCommand("TRUNCATE TABLE historyState");
                _FomMonitoringSQLiteEntities.Database.ExecuteSqlCommand("TRUNCATE TABLE historyMessage");
                _FomMonitoringSQLiteEntities.Database.ExecuteSqlCommand("TRUNCATE TABLE spindle");
                _FomMonitoringSQLiteEntities.Database.ExecuteSqlCommand("TRUNCATE TABLE tool");
                _FomMonitoringSQLiteEntities.Database.ExecuteSqlCommand("TRUNCATE TABLE message");

                _FomMonitoringSQLiteEntities.SaveChanges();

                
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
                                info.Id = 0;
                                info.LoginDate = info.LoginDate.HasValue && info.LoginDate.Value.Year < 1900 ? null : info.LoginDate;
                                info.InstallationDate = info.InstallationDate.HasValue && info.InstallationDate.Value.Year < 1900 ? null : info.InstallationDate;
                                info.NextMaintenanceService = info.NextMaintenanceService.HasValue && info.NextMaintenanceService.Value.Year < 1900 ? null : info.NextMaintenanceService;
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

                _FomMonitoringSQLiteEntities.Set<historyState>().AddRange(historyStateSQLite);
                _FomMonitoringSQLiteEntities.SaveChanges();

                _FomMonitoringSQLiteEntities.Set<spindle>().AddRange(spindleSQLite);
                _FomMonitoringSQLiteEntities.SaveChanges();

                _FomMonitoringSQLiteEntities.Set<tool>().AddRange(toolSQLite);
                _FomMonitoringSQLiteEntities.SaveChanges();

                _FomMonitoringSQLiteEntities.Set<message>().AddRange(messageSQLite);
                _FomMonitoringSQLiteEntities.SaveChanges();
                
                result = true;                    
                
            }
            catch (Exception ex)
            {
                string errMessage = string.Format(ex.GetStringLog(), jsonDataModel.Id.ToString());
                LogService.WriteLog(errMessage, LogService.TypeLevel.Error, ex);
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
