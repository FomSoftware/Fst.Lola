using FomMonitoringCore.DAL;
using FomMonitoringCore.DAL_SQLite;
using FomMonitoringCore.Framework.Common;
using Mapster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Transactions;

namespace FomMonitoringCore.Service.DataMapping
{
    public class SQLiteToSQLServerService
    {
        public static bool MappingSQLiteDetailsToSQLServer()
        {
            bool result = false;
            try
            {
                List<bar> barSQLite = new List<bar>();
                List<error> errorSQLite = new List<error>();
                List<historyJob> historyJobSQLite = new List<historyJob>();
                List<info> infoSQLite = new List<info>();
                List<piece> pieceSQLite = new List<piece>();
                List<spindle> spindleSQLite = new List<spindle>();
                List<state> stateSQLite = new List<state>();
                List<tool> toolSQLite = new List<tool>();
                List<message> messageSQLite = new List<message>();

                using (FST_FomMonitoringSQLiteEntities ent = new FST_FomMonitoringSQLiteEntities())
                {
                    barSQLite = ent.bar.ToList();
                    errorSQLite = ent.error.ToList();
                    historyJobSQLite = ent.historyJob.ToList();
                    infoSQLite = ent.info.ToList();
                    pieceSQLite = ent.piece.ToList();
                    spindleSQLite = ent.spindle.ToList();
                    stateSQLite = ent.state.ToList();
                    toolSQLite = ent.tool.ToList();
                    messageSQLite = ent.message.ToList();
                }

                string matricola = infoSQLite.OrderByDescending(o => o.Id).FirstOrDefault().MachineSerial;
                using (TransactionScope transaction = new TransactionScope(TransactionScopeOption.Required))
                {
                    List<Machine> machine = infoSQLite.Adapt<List<Machine>>();
                    using (FST_FomMonitoringEntities ent = new FST_FomMonitoringEntities())
                    {
                        Machine machineActual = ent.Machine.FirstOrDefault(f => f.Serial == matricola);
                        if (machineActual == null)
                        {
                            machineActual = machine.OrderByDescending(o => o.Id).FirstOrDefault();
                            machineActual.Id = 0;
                            ent.Machine.Add(machineActual);
                            ent.SaveChanges();
                        }
                        else
                        {
                            Machine machineFromSQLite = machine.OrderByDescending(o => o.Id).FirstOrDefault();
                            /*foreach (PropertyInfo property in machineActual.GetType().GetProperties())
                            {
                                if (property.PropertyType.IsSerializable && (property.Name != "Id"))
                                {
                                    property.SetValue(machineActual, machineFromSQLite.GetType().GetProperty(property.Name).GetValue(machineFromSQLite));
                                }
                            }*/
                            machineActual.Description = machineFromSQLite.Description;
                            machineActual.FirmwareVersion = machineFromSQLite.FirmwareVersion;
                            machineActual.InstallationDate = machineFromSQLite.InstallationDate;
                            machineActual.KeyId = machineFromSQLite.KeyId;
                            machineActual.LoginDate = machineFromSQLite.LoginDate;
                            machineActual.MachineModelId = machineFromSQLite.MachineModelId;
                            machineActual.MachineTypeId = machineFromSQLite.MachineTypeId;
                            machineActual.NextMaintenanceService = machineFromSQLite.NextMaintenanceService;
                            machineActual.PlcVersion = machineFromSQLite.PlcVersion;
                            machineActual.Product = machineFromSQLite.Product;
                            machineActual.ProductVersion = machineFromSQLite.ProductVersion;
                            machineActual.Serial = machineFromSQLite.Serial;
                            machineActual.Shift1 = machineFromSQLite.Shift1;
                            machineActual.Shift2 = machineFromSQLite.Shift2;
                            machineActual.Shift3 = machineFromSQLite.Shift3;
                            machineActual.BarsProductivityGreenThreshold = machineFromSQLite.BarsProductivityGreenThreshold;
                            machineActual.BarsProductivityYellowThreshold = machineFromSQLite.BarsProductivityYellowThreshold;
                            machineActual.LastUpdate = machineFromSQLite.LastUpdate;
                            machineActual.OverfeedGreenThreshold = machineFromSQLite.OverfeedGreenThreshold;
                            machineActual.OverfeedYellowThreshold = machineFromSQLite.OverfeedYellowThreshold;
                            machineActual.PiecesProductivityGreenThreshold = machineFromSQLite.PiecesProductivityGreenThreshold;
                            machineActual.PiecesProductivityYellowThreshold = machineFromSQLite.PiecesProductivityYellowThreshold;
                            machineActual.StateProductivityGreenThreshold = machineFromSQLite.StateProductivityGreenThreshold;
                            machineActual.StateProductivityYellowThreshold = machineFromSQLite.StateProductivityYellowThreshold;
                            machineActual.UTC = machineFromSQLite.UTC;


                        }
                        ent.SaveChanges();

                        List<Bar> bar = barSQLite.BuildAdapter().AddParameters("machineId", machineActual.Id).AdaptToType<List<Bar>>();
                        bar = bar.Where(w => w.StartTime > (machineActual.LastUpdate ?? new DateTime())).ToList();
                        ent.Bar.AddRange(bar);
                        ent.SaveChanges();

                        List<MessageMachine> messageMachine = messageSQLite.BuildAdapter().AddParameters("machineId", machineActual.Id).AdaptToType<List<MessageMachine>>();
                        messageMachine = messageMachine.Where(w => w.StartTime > (machineActual.LastUpdate ?? new DateTime())).ToList();

                        //devo eliminare quei messaggi che hanno scope = 0 da mdb
                        foreach(MessageMachine mm in messageMachine.ToList())
                        {
                            int cat = ent.MachineModel.Find(machineActual.MachineModelId).MessageCategoryId;
                            MessagesIndex msg = ent.MessagesIndex.FirstOrDefault(f => f.MessageCode == mm.Code && f.MessageCategoryId == cat);

                            ReadMessages.ReadMessageVisibilityGroup(mm, msg);
                        }                                      

                        ent.MessageMachine.AddRange(messageMachine);
                        ent.SaveChanges();
                        
                        List<HistoryJob> historyJob = historyJobSQLite.BuildAdapter().AddParameters("machineId", machineActual.Id).AdaptToType<List<HistoryJob>>();
                        DateTime minDateHistoryJob = historyJob.Any(a => a.Day.HasValue) ? historyJob.Where(w => w.Day.HasValue && w.MachineId == machineActual.Id).Select(s => s.Day).Min().Value : new DateTime();
                        List<HistoryJob> removeHistoryJob = ent.HistoryJob.Where(w => w.Day.HasValue && w.Day.Value >= minDateHistoryJob && w.MachineId == machineActual.Id).ToList();
                        ent.HistoryJob.RemoveRange(removeHistoryJob);
                        ent.SaveChanges();
                        ent.HistoryJob.AddRange(historyJob);
                        ent.SaveChanges();

                        List<AlarmMachine> alarmMachine = errorSQLite.BuildAdapter().AddParameters("machineId", machineActual.Id).AdaptToType<List<AlarmMachine>>();
                        alarmMachine = alarmMachine.Where(w => w.StartTime > (machineActual.LastUpdate ?? new DateTime())).ToList();
                        ent.AlarmMachine.AddRange(alarmMachine);
                        ent.SaveChanges();

                        List<Piece> piece = pieceSQLite.BuildAdapter().AddParameters("machineId", machineActual.Id).AdaptToType<List<Piece>>();
                        piece = piece.Where(w => w.EndTime > (machineActual.LastUpdate ?? new DateTime())).ToList();
                        ent.Piece.AddRange(piece);
                        ent.SaveChanges();

                        List<Spindle> spindle = spindleSQLite.BuildAdapter().AddParameters("machineId", machineActual.Id).AdaptToType<List<Spindle>>();
                        List<Spindle> removeSpindles = ent.Spindle.Where(w => w.MachineId == machineActual.Id).ToList();
                        ent.Spindle.RemoveRange(removeSpindles);
                        ent.SaveChanges();
                        ent.Spindle.AddRange(spindle);
                        ent.SaveChanges();

                        List<StateMachine> state = stateSQLite.BuildAdapter().AddParameters("machineId", machineActual.Id).AdaptToType<List<StateMachine>>();
                        state = state.Where(w => w.EndTime > (machineActual.LastUpdate ?? new DateTime())).ToList();
                        ent.StateMachine.AddRange(state);
                        ent.SaveChanges();

                        List<ToolMachine> tool = toolSQLite.BuildAdapter().AddParameters("machineId", machineActual.Id).AdaptToType<List<ToolMachine>>();
                        List<ToolMachine> removeTools = toolSQLite.Join(ent.ToolMachine, to => to.Code, from => from.Code, (to, from) => new { From = from, To = to })
                            .Where(w => w.From.MachineId == machineActual.Id && w.From.Code == w.To.Code && w.From.DateLoaded == w.To.DateLoaded).Select(s => s.From).ToList();
                        ent.ToolMachine.RemoveRange(removeTools);
                        ent.SaveChanges();
                        List<ToolMachine> modifyTools = ent.ToolMachine.Where(w => w.MachineId == machineActual.Id && w.IsActive).ToList();
                        foreach (ToolMachine modifyTool in modifyTools)
                        {
                            modifyTool.IsActive = false;
                        }
                        ent.SaveChanges();
                        ent.ToolMachine.AddRange(tool);
                        ent.SaveChanges();

                        StateMachine lastState = ent.StateMachine.Where(w => w.MachineId == machineActual.Id).OrderByDescending(o => o.EndTime).FirstOrDefault();
                        machineActual = ent.Machine.FirstOrDefault(f => f.Id == machineActual.Id);
                        machineActual.LastUpdate = lastState.EndTime;
                        ent.SaveChanges();

                        ent.usp_HistoricizingAll(machineActual.Id);
                        ent.SaveChanges();

                        transaction.Complete();
                        result = true;
                    }
                }
            }
            catch (Exception ex)
            {
                string errMessage = string.Format(ex.GetStringLog());
                LogService.WriteLog(errMessage, LogService.TypeLevel.Error, ex);
            }
            return result;
        }

        public static bool MappingSQLiteHistoryToSQLServer()
        {
            bool result = false;
            try
            {
                List<historyAlarm> historyAlarmSQLite = new List<historyAlarm>();
                List<historyMessage> historyMessageSQLite = new List<historyMessage>();
                List<historyBar> historyBarSQLite = new List<historyBar>();
                List<historyJob> historyJobSQLite = new List<historyJob>();
                List<historyPiece> historyPieceSQLite = new List<historyPiece>();
                List<historyState> historyStateSQLite = new List<historyState>();
                List<info> infoSQLite = new List<info>();
                List<spindle> spindleSQLite = new List<spindle>();
                List<tool> toolSQLite = new List<tool>();
                using (FST_FomMonitoringSQLiteEntities ent = new FST_FomMonitoringSQLiteEntities())
                {
                    historyAlarmSQLite = ent.historyAlarm.ToList();
                    historyBarSQLite = ent.historyBar.ToList();
                    historyJobSQLite = ent.historyJob.ToList();
                    historyPieceSQLite = ent.historyPiece.ToList();
                    historyStateSQLite = ent.historyState.ToList();
                    historyMessageSQLite = ent.historyMessage.ToList();
                    infoSQLite = ent.info.ToList();
                    spindleSQLite = ent.spindle.ToList();
                    toolSQLite = ent.tool.ToList();
                }
                string matricola = infoSQLite.OrderByDescending(o => o.Id).FirstOrDefault().MachineSerial;
                using (TransactionScope transaction = new TransactionScope(TransactionScopeOption.Required))
                {
                    using (FST_FomMonitoringEntities ent = new FST_FomMonitoringEntities())
                    {
                        List<Machine> machine = infoSQLite.Adapt<List<Machine>>();
                        Machine machineActual = ent.Machine.FirstOrDefault(f => f.Serial == matricola);
                        if (machineActual == null)
                        {
                            machineActual = machine.OrderByDescending(o => o.Id).FirstOrDefault();
                            machineActual.Id = 0;
                            ent.Machine.Add(machine.OrderByDescending(o => o.Id).FirstOrDefault());
                        }
                        else
                        {
                            Machine machineFromSQLite = machine.OrderByDescending(o => o.Id).FirstOrDefault();
                            foreach (PropertyInfo property in machineActual.GetType().GetProperties())
                            {
                                if (property.PropertyType.IsSerializable && property.Name != "Id")
                                {
                                    property.SetValue(machineActual, machineFromSQLite.GetType().GetProperty(property.Name).GetValue(machineFromSQLite));
                                }
                            }
                        }
                        ent.SaveChanges();

                        List<HistoryAlarm> historyAlarm = historyAlarmSQLite.BuildAdapter().AddParameters("machineId", machineActual.Id).AdaptToType<List<HistoryAlarm>>();
                        DateTime minDateHistoryAlarm = historyAlarm.Any(a => a.Day.HasValue) ? historyAlarm.Where(w => w.Day.HasValue && w.MachineId == machineActual.Id).Select(s => s.Day).Min().Value : new DateTime();
                        List<HistoryAlarm> removeHistoryAlarm = ent.HistoryAlarm.Where(w => w.Day.HasValue && w.Day.Value >= minDateHistoryAlarm && w.MachineId == machineActual.Id).ToList();
                        ent.HistoryAlarm.RemoveRange(removeHistoryAlarm);
                        ent.SaveChanges();
                        ent.HistoryAlarm.AddRange(historyAlarm);
                        ent.SaveChanges();

                        List<HistoryMessage> historyMessage = historyMessageSQLite.BuildAdapter().AddParameters("machineId", machineActual.Id).AdaptToType<List<HistoryMessage>>();
                        DateTime minDateHistoryMessage = historyMessage.Any(a => a.Day.HasValue) ? historyMessage.Where(w => w.Day.HasValue && w.MachineId == machineActual.Id).Select(s => s.Day).Min().Value : new DateTime();
                        List<HistoryMessage> removeHistoryMessage = ent.HistoryMessage.Where(w => w.Day.HasValue && w.Day.Value >= minDateHistoryMessage && w.MachineId == machineActual.Id).ToList();
                        ent.HistoryMessage.RemoveRange(removeHistoryMessage);
                        ent.SaveChanges();
                        ent.HistoryMessage.AddRange(historyMessage);
                        ent.SaveChanges();

                        List<HistoryBar> historyBar = historyBarSQLite.BuildAdapter().AddParameters("machineId", machineActual.Id).AdaptToType<List<HistoryBar>>();
                        DateTime minDateHistoryBar = historyBar.Any(a => a.Day.HasValue) ? historyBar.Where(w => w.Day.HasValue && w.MachineId == machineActual.Id).Select(s => s.Day).Min().Value : new DateTime();
                        List<HistoryBar> removeHistoryBar = ent.HistoryBar.Where(w => w.Day.HasValue && w.Day.Value >= minDateHistoryBar && w.MachineId == machineActual.Id).ToList();
                        ent.HistoryBar.RemoveRange(removeHistoryBar);
                        ent.SaveChanges();
                        ent.HistoryBar.AddRange(historyBar);
                        ent.SaveChanges();

                        List<HistoryJob> historyJob = historyJobSQLite.BuildAdapter().AddParameters("machineId", machineActual.Id).AdaptToType<List<HistoryJob>>();
                        DateTime minDateHistoryJob = historyJob.Any(a => a.Day.HasValue) ? historyJob.Where(w => w.Day.HasValue && w.MachineId == machineActual.Id).Select(s => s.Day).Min().Value : new DateTime();
                        List<HistoryJob> removeHistoryJob = ent.HistoryJob.Where(w => w.Day.HasValue && w.Day.Value >= minDateHistoryJob && w.MachineId == machineActual.Id).ToList();
                        ent.HistoryJob.RemoveRange(removeHistoryJob);
                        ent.SaveChanges();
                        ent.HistoryJob.AddRange(historyJob);
                        ent.SaveChanges();

                        List<HistoryPiece> historyPiece = historyPieceSQLite.BuildAdapter().AddParameters("machineId", machineActual.Id).AdaptToType<List<HistoryPiece>>();
                        DateTime minDateHistoryPiece = historyPiece.Any(a => a.Day.HasValue) ? historyPiece.Where(w => w.Day.HasValue && w.MachineId == machineActual.Id).Select(s => s.Day).Min().Value : new DateTime();
                        List<HistoryPiece> removeHistoryPiece = ent.HistoryPiece.Where(w => w.Day.HasValue && w.Day.Value >= minDateHistoryPiece && w.MachineId == machineActual.Id).ToList();
                        ent.HistoryPiece.RemoveRange(removeHistoryPiece);
                        ent.SaveChanges();
                        ent.HistoryPiece.AddRange(historyPiece);
                        ent.SaveChanges();

                        List<HistoryState> historyState = historyStateSQLite.BuildAdapter().AddParameters("machineId", machineActual.Id).AdaptToType<List<HistoryState>>();
                        DateTime minDateHistoryState = historyState.Any(a => a.Day.HasValue) ? historyState.Where(w => w.Day.HasValue && w.MachineId == machineActual.Id).Select(s => s.Day).Min().Value : new DateTime();
                        List<HistoryState> removeHistoryState = ent.HistoryState.Where(w => w.Day.HasValue && w.Day.Value >= minDateHistoryState && w.MachineId == machineActual.Id).ToList();
                        ent.HistoryState.RemoveRange(removeHistoryState);
                        ent.SaveChanges();
                        ent.HistoryState.AddRange(historyState);
                        ent.SaveChanges();

                        List<Spindle> spindle = spindleSQLite.BuildAdapter().AddParameters("machineId", machineActual.Id).AdaptToType<List<Spindle>>();
                        List<Spindle> removeSpindle = ent.Spindle.Where(w => w.MachineId == machineActual.Id).ToList();
                        ent.Spindle.RemoveRange(removeSpindle);
                        ent.SaveChanges();
                        ent.Spindle.AddRange(spindle);
                        ent.SaveChanges();

                        List<ToolMachine> tool = toolSQLite.BuildAdapter().AddParameters("machineId", machineActual.Id).AdaptToType<List<ToolMachine>>();
                        List<ToolMachine> removeTool = ent.ToolMachine.Where(w => w.MachineId == machineActual.Id).ToList();
                        ent.ToolMachine.RemoveRange(removeTool);
                        ent.SaveChanges();
                        ent.ToolMachine.AddRange(tool);
                        ent.SaveChanges();

                        HistoryState lastState = ent.HistoryState.Where(w => w.MachineId == machineActual.Id).OrderByDescending(o => o.Day).FirstOrDefault();
                        machineActual = ent.Machine.FirstOrDefault(f => f.Id == machineActual.Id);
                        machineActual.LastUpdate = lastState.Day;
                        ent.SaveChanges();

                        transaction.Complete();
                        result = true;
                    }
                }
            }
            catch (Exception ex)
            {
                string errMessage = string.Format(ex.GetStringLog());
                LogService.WriteLog(errMessage, LogService.TypeLevel.Error, ex);
            }
            return result;
        }
    }
}
