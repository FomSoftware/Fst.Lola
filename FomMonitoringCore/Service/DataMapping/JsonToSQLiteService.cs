using FomMonitoringCore.DAL;
using FomMonitoringCore.DAL_SQLite;
using FomMonitoringCore.Framework.Common;
using FomMonitoringCore.Framework.Model;
using Mapster;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FomMonitoringCore.Service.DataMapping
{
    public class JsonToSqLiteService : IJsonToSqLiteService
    {
        private readonly IFomMonitoringEntities _fomMonitoringEntities;
        private readonly IFomMonitoringSQLiteEntities _fomMonitoringSqLiteEntities;
        
        public JsonToSqLiteService(IFomMonitoringEntities fomMonitoringEntities, IFomMonitoringSQLiteEntities fomMonitoringSqLiteEntities)
        {
            _fomMonitoringEntities = fomMonitoringEntities;
            _fomMonitoringSqLiteEntities = fomMonitoringSqLiteEntities;
        }

        public List<JsonDataModel> GetAllJsonDataNotElaborated()
        {
            List<JsonDataModel> result = new List<JsonDataModel>();
            try
            {
                List<JsonData> jsonData = new List<JsonData>();                
                jsonData = _fomMonitoringEntities.Set<JsonData>().Where(w => !w.IsElaborated).OrderBy(o => o.Id).ToList();
                result = jsonData.Adapt<List<JsonDataModel>>();
                
            }
            catch (Exception ex)
            {
                string errMessage = string.Format(ex.GetStringLog());
                LogService.WriteLog(errMessage, LogService.TypeLevel.Error, ex);
            }
            return result;
        }

        public bool MappingJsonDetailsToSqLite(JsonDataModel jsonDataModel)
        {
            bool result = false;
            try
            {
                JObject json = JsonConvert.DeserializeObject<JObject>(jsonDataModel.Json);
                List<bar> barSqLite = new List<bar>();
                List<historyJob> historyJobSqLite = new List<historyJob>();
                List<info> infoSqLite = new List<info>();
                List<piece> pieceSqLite = new List<piece>();
                List<message> messageSqLite = new List<message>();
                List<state> stateSqLite = new List<state>();
                List<tool> toolSqLite = new List<tool>();
                
               /* _FomMonitoringSQLiteEntities.Database.ExecuteSqlCommand("TRUNCATE TABLE bar");
                _FomMonitoringSQLiteEntities.Database.ExecuteSqlCommand("TRUNCATE TABLE error");
                _FomMonitoringSQLiteEntities.Database.ExecuteSqlCommand("TRUNCATE TABLE historyJob");
                _FomMonitoringSQLiteEntities.Database.ExecuteSqlCommand("TRUNCATE TABLE info");
                _FomMonitoringSQLiteEntities.Database.ExecuteSqlCommand("TRUNCATE TABLE piece");
                _FomMonitoringSQLiteEntities.Database.ExecuteSqlCommand("TRUNCATE TABLE spindle");
                _FomMonitoringSQLiteEntities.Database.ExecuteSqlCommand("TRUNCATE TABLE state");
                _FomMonitoringSQLiteEntities.Database.ExecuteSqlCommand("TRUNCATE TABLE tool");
                _FomMonitoringSQLiteEntities.Database.ExecuteSqlCommand("TRUNCATE TABLE message");*/
                if(_fomMonitoringSqLiteEntities.Set<bar>() != null)
                    _fomMonitoringSqLiteEntities.Set<bar>().RemoveRange(_fomMonitoringSqLiteEntities.Set<bar>());

                if (_fomMonitoringSqLiteEntities.Set<historyJob>() != null)
                    _fomMonitoringSqLiteEntities.Set<historyJob>().RemoveRange(_fomMonitoringSqLiteEntities.Set<historyJob>());

                if (_fomMonitoringSqLiteEntities.Set<info>() != null)
                    _fomMonitoringSqLiteEntities.Set<info>().RemoveRange(_fomMonitoringSqLiteEntities.Set<info>());

                if (_fomMonitoringSqLiteEntities.Set<piece>() != null)
                    _fomMonitoringSqLiteEntities.Set<piece>().RemoveRange(_fomMonitoringSqLiteEntities.Set<piece>());

                if (_fomMonitoringSqLiteEntities.Set<state>() != null)
                    _fomMonitoringSqLiteEntities.Set<state>().RemoveRange(_fomMonitoringSqLiteEntities.Set<state>());

                if (_fomMonitoringSqLiteEntities.Set<tool>() != null)
                    _fomMonitoringSqLiteEntities.Set<tool>().RemoveRange(_fomMonitoringSqLiteEntities.Set<tool>());

                if (_fomMonitoringSqLiteEntities.Set<message>() != null)
                    _fomMonitoringSqLiteEntities.Set<message>().RemoveRange(_fomMonitoringSqLiteEntities.Set<message>());
              

                foreach (JToken token in json.Root)
                    {
                        switch (token.Path.ToLower())
                        {
                            case "bar":
                                barSqLite = JsonConvert.DeserializeObject<List<bar>>(JsonConvert.SerializeObject(token.First));
                                foreach (bar bar in barSqLite)
                                {
                                    bar.StartTime = bar.StartTime.HasValue && bar.StartTime.Value.Year < 1900 ? null : bar.StartTime;
                                    bar.EndTime = bar.EndTime.HasValue && bar.EndTime.Value.Year < 1900 ? null : bar.EndTime;
                                }
                                break;
                            case "historyjob":
                                historyJobSqLite = JsonConvert.DeserializeObject<List<historyJob>>(JsonConvert.SerializeObject(token.First));
                                foreach (historyJob historyJob in historyJobSqLite)
                                {
                                    historyJob.Id = 0;
                                    historyJob.Day = historyJob.Day.HasValue && historyJob.Day.Value.Year < 1900 ? null : historyJob.Day;
                                }
                                break;
                            case "info":
                                infoSqLite = JsonConvert.DeserializeObject<List<info>>(JsonConvert.SerializeObject(token.First));
                                foreach (info info in infoSqLite)
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
                                pieceSqLite = JsonConvert.DeserializeObject<List<piece>>(JsonConvert.SerializeObject(token.First));
                                foreach (piece piece in pieceSqLite)
                                {
                                    piece.Id = 0;
                                    piece.EndTime = piece.EndTime.HasValue && piece.EndTime.Value.Year < 1900 ? null : piece.EndTime;
                                    piece.StartTime = piece.StartTime.HasValue && piece.StartTime.Value.Year < 1900 ? null : piece.StartTime;
                            }
                                break;
                            case "state":
                                string jsonSerialized = JsonConvert.SerializeObject(token.First);
                                stateSqLite = JsonConvert.DeserializeObject<List<state>>(jsonSerialized);
                                foreach (state state in stateSqLite)
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
                                toolSqLite = JsonConvert.DeserializeObject<List<tool>>(JsonConvert.SerializeObject(token.First));
                                foreach (tool tool in toolSqLite)
                                {
                                    tool.Id = 0;
                                    tool.DateLoaded = tool.DateLoaded.HasValue && tool.DateLoaded.Value.Year < 1900 ? null : tool.DateLoaded;
                                    tool.DateReplaced = tool.DateReplaced.HasValue && tool.DateReplaced.Value.Year < 1900 ? null : tool.DateReplaced;
                                }
                                break;
                            case "message":
                                List<MessageModel>messageModels = JsonConvert.DeserializeObject<List<MessageModel>>(JsonConvert.SerializeObject(token.First));
                                messageSqLite = messageModels.Select(m => new message()
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

                if (barSqLite.Any())
                {
                    _fomMonitoringSqLiteEntities.Set<bar>().AddRange(barSqLite);
                    _fomMonitoringSqLiteEntities.SaveChanges();
                }
                if (historyJobSqLite.Any())
                {
                    _fomMonitoringSqLiteEntities.Set<historyJob>().AddRange(historyJobSqLite);
                    _fomMonitoringSqLiteEntities.SaveChanges();
                }
                if (infoSqLite.Any())
                {
                    _fomMonitoringSqLiteEntities.Set<info>().AddRange(infoSqLite);
                    _fomMonitoringSqLiteEntities.SaveChanges();
                }
                if (pieceSqLite.Any())
                {
                    _fomMonitoringSqLiteEntities.Set<piece>().AddRange(pieceSqLite);
                    _fomMonitoringSqLiteEntities.SaveChanges();
                }
                if (stateSqLite.Any())
                {
                    _fomMonitoringSqLiteEntities.Set<state>().AddRange(stateSqLite);
                    _fomMonitoringSqLiteEntities.SaveChanges();
                }
                if (toolSqLite.Any())
                {
                    _fomMonitoringSqLiteEntities.Set<tool>().AddRange(toolSqLite);
                    _fomMonitoringSqLiteEntities.SaveChanges();
                }

                if (messageSqLite.Any())
                {
                    _fomMonitoringSqLiteEntities.Set<message>().AddRange(messageSqLite);
                    _fomMonitoringSqLiteEntities.SaveChanges();
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

        public bool MappingJsonHistoryToSqLite(JsonDataModel jsonDataModel)
        {
            bool result = false;
            try
            {
                JObject json = JsonConvert.DeserializeObject<JObject>(jsonDataModel.Json);
                List<historyBar> historyBarSqLite = new List<historyBar>();
                List<historyPiece> historyPieceSqLite = new List<historyPiece>();
                List<historyJob> historyJobSqLite = new List<historyJob>();
                List<info> infoSqLite = new List<info>();
                List<historyMessage> historyMessageSqLite = new List<historyMessage>();
                List<historyState> historyStateSqLite = new List<historyState>();
                List<tool> toolSqLite = new List<tool>();
                List<message> messageSqLite = new List<message>();
                

                _fomMonitoringSqLiteEntities.Database.ExecuteSqlCommand("TRUNCATE TABLE historyBar");
                _fomMonitoringSqLiteEntities.Database.ExecuteSqlCommand("TRUNCATE TABLE historyPiece");
                _fomMonitoringSqLiteEntities.Database.ExecuteSqlCommand("TRUNCATE TABLE historyJob");
                _fomMonitoringSqLiteEntities.Database.ExecuteSqlCommand("TRUNCATE TABLE info");
                _fomMonitoringSqLiteEntities.Database.ExecuteSqlCommand("TRUNCATE TABLE historyState");
                _fomMonitoringSqLiteEntities.Database.ExecuteSqlCommand("TRUNCATE TABLE historyMessage");
                _fomMonitoringSqLiteEntities.Database.ExecuteSqlCommand("TRUNCATE TABLE tool");
                _fomMonitoringSqLiteEntities.Database.ExecuteSqlCommand("TRUNCATE TABLE message");

                _fomMonitoringSqLiteEntities.SaveChanges();

                
                foreach (JToken token in json.Root)
                {
                    switch (token.Path.ToLower())
                    {
                        case "historybar":
                            historyBarSqLite = JsonConvert.DeserializeObject<List<historyBar>>(JsonConvert.SerializeObject(token.First));
                            foreach (var historyBar in historyBarSqLite)
                            {
                                historyBar.Day = historyBar.Day.HasValue && historyBar.Day.Value.Year < 1900 ? null : historyBar.Day;
                            }
                            break;
                        case "historypiece":
                            historyPieceSqLite = JsonConvert.DeserializeObject<List<historyPiece>>(JsonConvert.SerializeObject(token.First));
                            foreach (var historyPiece in historyPieceSqLite)
                            {
                                historyPiece.Day = historyPiece.Day.HasValue && historyPiece.Day.Value.Year < 1900 ? null : historyPiece.Day;
                            }
                            break;
                        case "historyjob":
                            historyJobSqLite = JsonConvert.DeserializeObject<List<historyJob>>(JsonConvert.SerializeObject(token.First));
                            foreach (var historyJob in historyJobSqLite)
                            {
                                historyJob.Day = historyJob.Day.HasValue && historyJob.Day.Value.Year < 1900 ? null : historyJob.Day;
                            }
                            break;
                        case "info":
                            infoSqLite = JsonConvert.DeserializeObject<List<info>>(JsonConvert.SerializeObject(token.First));
                            foreach (var info in infoSqLite)
                            {
                                info.Id = 0;
                                info.LoginDate = info.LoginDate.HasValue && info.LoginDate.Value.Year < 1900 ? null : info.LoginDate;
                                info.InstallationDate = info.InstallationDate.HasValue && info.InstallationDate.Value.Year < 1900 ? null : info.InstallationDate;
                                info.NextMaintenanceService = info.NextMaintenanceService.HasValue && info.NextMaintenanceService.Value.Year < 1900 ? null : info.NextMaintenanceService;
                            }
                            break;
                        case "historymessage":
                            historyMessageSqLite = JsonConvert.DeserializeObject<List<historyMessage>>(JsonConvert.SerializeObject(token.First));
                            foreach (var historyMessage in historyMessageSqLite)
                            {
                                historyMessage.Day = historyMessage.Day.HasValue && historyMessage.Day.Value.Year < 1900 ? null : historyMessage.Day;
                            }
                            break;
                        case "historystate":
                            historyStateSqLite = JsonConvert.DeserializeObject<List<historyState>>(JsonConvert.SerializeObject(token.First));
                            foreach (var historyState in historyStateSqLite)
                            {
                                historyState.Day = historyState.Day.HasValue && historyState.Day.Value.Year < 1900 ? null : historyState.Day;
                            }
                            break;
                        case "tool":
                            toolSqLite = JsonConvert.DeserializeObject<List<tool>>(JsonConvert.SerializeObject(token.First));
                            foreach (var tool in toolSqLite)
                            {
                                tool.DateLoaded = tool.DateLoaded.HasValue && tool.DateLoaded.Value.Year < 1900 ? null : tool.DateLoaded;
                                tool.DateReplaced = tool.DateReplaced.HasValue && tool.DateReplaced.Value.Year < 1900 ? null : tool.DateReplaced;
                            }
                            break;
                        case "message":
                            messageSqLite = JsonConvert.DeserializeObject<List<message>>(JsonConvert.SerializeObject(token.First));
                            foreach (var message in messageSqLite)
                            {
                                message.Time = message.Time.HasValue && message.Time.Value.Year < 1900 ? null : message.Time;
                            }
                            break;
                        default:
                            break;
                    }
                }

                _fomMonitoringSqLiteEntities.Set<historyBar>().AddRange(historyBarSqLite);
                _fomMonitoringSqLiteEntities.SaveChanges();

                _fomMonitoringSqLiteEntities.Set<historyPiece>().AddRange(historyPieceSqLite);
                _fomMonitoringSqLiteEntities.SaveChanges();

                _fomMonitoringSqLiteEntities.Set<historyMessage>().AddRange(historyMessageSqLite);
                _fomMonitoringSqLiteEntities.SaveChanges();

                _fomMonitoringSqLiteEntities.Set<historyJob>().AddRange(historyJobSqLite);
                _fomMonitoringSqLiteEntities.SaveChanges();

                _fomMonitoringSqLiteEntities.Set<info>().AddRange(infoSqLite);
                _fomMonitoringSqLiteEntities.SaveChanges();

                _fomMonitoringSqLiteEntities.Set<historyState>().AddRange(historyStateSqLite);
                _fomMonitoringSqLiteEntities.SaveChanges();

                _fomMonitoringSqLiteEntities.Set<tool>().AddRange(toolSqLite);
                _fomMonitoringSqLiteEntities.SaveChanges();

                _fomMonitoringSqLiteEntities.Set<message>().AddRange(messageSqLite);
                _fomMonitoringSqLiteEntities.SaveChanges();
                
                result = true;                    
                
            }
            catch (Exception ex)
            {
                string errMessage = string.Format(ex.GetStringLog(), jsonDataModel.Id.ToString());
                LogService.WriteLog(errMessage, LogService.TypeLevel.Error, ex);
            }
            return result;
        }

        public bool SaveElaboration(int id, bool isLoaded)
        {
            bool result = false;
            try
            {                
                JsonData jsonData = _fomMonitoringEntities.Set<JsonData>().FirstOrDefault(f => f.Id == id);
                jsonData.ElaborationDate = DateTime.UtcNow;
                jsonData.IsElaborated = true;
                jsonData.IsLoaded = isLoaded;
                _fomMonitoringEntities.SaveChanges();

                result = true;                
            }
            catch (Exception ex)
            {
                string errMessage = string.Format(ex.GetStringLog(), id.ToString(), isLoaded.ToString());
                LogService.WriteLog(errMessage, LogService.TypeLevel.Error, ex);
            }
            return result;
        }
    }
}
