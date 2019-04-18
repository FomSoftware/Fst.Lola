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
using System.Transactions;

namespace FomMonitoringCore.Service.DataMapping
{
    public class JsonToSQLiteService
    {
        public static List<JsonDataModel> GetAllJsonDataNotElaborated()
        {
            List<JsonDataModel> result = new List<JsonDataModel>();
            try
            {
                List<JsonData> jsonData = new List<JsonData>();
                using (FST_FomMonitoringEntities ent = new FST_FomMonitoringEntities())
                {
                    jsonData = ent.JsonData.Where(w => !w.IsElaborated).OrderBy(o => o.Id).ToList();
                    result = jsonData.Adapt<List<JsonDataModel>>();
                }
            }
            catch (Exception ex)
            {
                string errMessage = string.Format(ex.GetStringLog());
                LogService.WriteLog(errMessage, LogService.TypeLevel.Error, ex);
            }
            return result;
        }

        public static bool MappingJsonDetailsToSQLite(JsonDataModel jsonDataModel)
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
                using (TransactionScope transaction = new TransactionScope())
                {
                    using (FST_FomMonitoringSQLiteEntities ent = new FST_FomMonitoringSQLiteEntities())
                    {
                        ent.Database.ExecuteSqlCommand("TRUNCATE TABLE bar");
                        ent.Database.ExecuteSqlCommand("TRUNCATE TABLE error");
                        ent.Database.ExecuteSqlCommand("TRUNCATE TABLE historyJob");
                        ent.Database.ExecuteSqlCommand("TRUNCATE TABLE info");
                        ent.Database.ExecuteSqlCommand("TRUNCATE TABLE piece");
                        ent.Database.ExecuteSqlCommand("TRUNCATE TABLE spindle");
                        ent.Database.ExecuteSqlCommand("TRUNCATE TABLE state");
                        ent.Database.ExecuteSqlCommand("TRUNCATE TABLE tool");
                        ent.Database.ExecuteSqlCommand("TRUNCATE TABLE message");

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
                                        info.InstallationDate = info.InstallationDate.HasValue && info.InstallationDate.Value.Year < 1900 ? null : info.InstallationDate;
                                        info.NextMaintenanceService = info.NextMaintenanceService.HasValue && info.NextMaintenanceService.Value.Year < 1900 ? null : info.NextMaintenanceService;
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
                                    messageSQLite = JsonConvert.DeserializeObject<List<message>>(JsonConvert.SerializeObject(token.First));
                                    foreach (message message in messageSQLite)
                                    {
                                        message.Time = message.Time.HasValue && message.Time.Value.Year < 1900 ? null : message.Time;
                                    }
                                    break;
                                default:
                                    break;
                            }
                        }

                        ent.bar.AddRange(barSQLite);
                        ent.SaveChanges();

                        ent.error.AddRange(errorSQLite);
                        ent.SaveChanges();

                        ent.historyJob.AddRange(historyJobSQLite);
                        ent.SaveChanges();

                        ent.info.AddRange(infoSQLite);
                        ent.SaveChanges();

                        ent.piece.AddRange(pieceSQLite);
                        ent.SaveChanges();

                        ent.spindle.AddRange(spindleSQLite);
                        ent.SaveChanges();

                        ent.state.AddRange(stateSQLite);
                        ent.SaveChanges();

                        ent.tool.AddRange(toolSQLite);
                        ent.SaveChanges();

                        ent.message.AddRange(messageSQLite);
                        ent.SaveChanges();

                        transaction.Complete();
                        result = true;
                    }
                }
            }
            catch (Exception ex)
            {
                string errMessage = string.Format(ex.GetStringLog(), jsonDataModel.Id.ToString());
                LogService.WriteLog(errMessage, LogService.TypeLevel.Error, ex);
            }
            return result;
        }

        public static bool MappingJsonHistoryToSQLite(JsonDataModel jsonDataModel)
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

                using (TransactionScope transaction = new TransactionScope())
                {
                    using (FST_FomMonitoringSQLiteEntities ent = new FST_FomMonitoringSQLiteEntities())
                    {
                        ent.Database.ExecuteSqlCommand("TRUNCATE TABLE historyBar");
                        ent.Database.ExecuteSqlCommand("TRUNCATE TABLE historyPiece");
                        ent.Database.ExecuteSqlCommand("TRUNCATE TABLE historyJob");
                        ent.Database.ExecuteSqlCommand("TRUNCATE TABLE info");
                        ent.Database.ExecuteSqlCommand("TRUNCATE TABLE historyAlarm");
                        ent.Database.ExecuteSqlCommand("TRUNCATE TABLE historyState");
                        ent.Database.ExecuteSqlCommand("TRUNCATE TABLE spindle");
                        ent.Database.ExecuteSqlCommand("TRUNCATE TABLE tool");
                        ent.Database.ExecuteSqlCommand("TRUNCATE TABLE message");

                        foreach (JToken token in json.Root)
                        {
                            switch (token.Path.ToLower())
                            {
                                case "historybar":
                                    historyBarSQLite = JsonConvert.DeserializeObject<List<historyBar>>(JsonConvert.SerializeObject(token.First));
                                    foreach (historyBar historyBar in historyBarSQLite)
                                    {
                                        historyBar.Day = historyBar.Day.HasValue && historyBar.Day.Value.Year < 1900 ? null : historyBar.Day;
                                    }
                                    break;
                                case "historypiece":
                                    historyPieceSQLite = JsonConvert.DeserializeObject<List<historyPiece>>(JsonConvert.SerializeObject(token.First));
                                    foreach (historyPiece historyPiece in historyPieceSQLite)
                                    {
                                        historyPiece.Day = historyPiece.Day.HasValue && historyPiece.Day.Value.Year < 1900 ? null : historyPiece.Day;
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
                                        info.InstallationDate = info.InstallationDate.HasValue && info.InstallationDate.Value.Year < 1900 ? null : info.InstallationDate;
                                    }
                                    break;
                                case "historyalarm":
                                    historyAlarmSQLite = JsonConvert.DeserializeObject<List<historyAlarm>>(JsonConvert.SerializeObject(token.First));
                                    foreach (historyAlarm historyAlarm in historyAlarmSQLite)
                                    {
                                        historyAlarm.Day = historyAlarm.Day.HasValue && historyAlarm.Day.Value.Year < 1900 ? null : historyAlarm.Day;
                                    }
                                    break;
                                case "historymessage":
                                    historyMessageSQLite = JsonConvert.DeserializeObject<List<historyMessage>>(JsonConvert.SerializeObject(token.First));
                                    foreach (historyMessage historyMessage in historyMessageSQLite)
                                    {
                                        historyMessage.Day = historyMessage.Day.HasValue && historyMessage.Day.Value.Year < 1900 ? null : historyMessage.Day;
                                    }
                                    break;
                                case "historystate":
                                    historyStateSQLite = JsonConvert.DeserializeObject<List<historyState>>(JsonConvert.SerializeObject(token.First));
                                    foreach (historyState historyState in historyStateSQLite)
                                    {
                                        historyState.Day = historyState.Day.HasValue && historyState.Day.Value.Year < 1900 ? null : historyState.Day;
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
                                case "tool":
                                    toolSQLite = JsonConvert.DeserializeObject<List<tool>>(JsonConvert.SerializeObject(token.First));
                                    foreach (tool tool in toolSQLite)
                                    {
                                        tool.DateLoaded = tool.DateLoaded.HasValue && tool.DateLoaded.Value.Year < 1900 ? null : tool.DateLoaded;
                                        tool.DateReplaced = tool.DateReplaced.HasValue && tool.DateReplaced.Value.Year < 1900 ? null : tool.DateReplaced;
                                    }
                                    break;
                                case "message":
                                    messageSQLite = JsonConvert.DeserializeObject<List<message>>(JsonConvert.SerializeObject(token.First));
                                    foreach (message message in messageSQLite)
                                    {
                                        message.Time = message.Time.HasValue && message.Time.Value.Year < 1900 ? null : message.Time;
                                    }
                                    break;
                                default:
                                    break;
                            }
                        }

                        ent.historyBar.AddRange(historyBarSQLite);
                        ent.SaveChanges();

                        ent.historyPiece.AddRange(historyPieceSQLite);
                        ent.SaveChanges();

                        ent.historyMessage.AddRange(historyMessageSQLite);
                        ent.SaveChanges();

                        ent.historyJob.AddRange(historyJobSQLite);
                        ent.SaveChanges();

                        ent.info.AddRange(infoSQLite);
                        ent.SaveChanges();

                        ent.historyAlarm.AddRange(historyAlarmSQLite);
                        ent.SaveChanges();

                        ent.historyState.AddRange(historyStateSQLite);
                        ent.SaveChanges();

                        ent.spindle.AddRange(spindleSQLite);
                        ent.SaveChanges();

                        ent.tool.AddRange(toolSQLite);
                        ent.SaveChanges();

                        ent.message.AddRange(messageSQLite);
                        ent.SaveChanges();

                        transaction.Complete();
                        result = true;
                    }
                }
            }
            catch (Exception ex)
            {
                string errMessage = string.Format(ex.GetStringLog(), jsonDataModel.Id.ToString());
                LogService.WriteLog(errMessage, LogService.TypeLevel.Error, ex);
            }
            return result;
        }

        public static bool SaveElaboration(int id, bool IsLoaded)
        {
            bool result = false;
            try
            {
                using (FST_FomMonitoringEntities ent = new FST_FomMonitoringEntities())
                {
                    JsonData jsonData = ent.JsonData.FirstOrDefault(f => f.Id == id);
                    jsonData.ElaborationDate = DateTime.Now;
                    jsonData.IsElaborated = true;
                    jsonData.IsLoaded = IsLoaded;
                    ent.SaveChanges();

                    result = true;
                }
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
