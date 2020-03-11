using FomMonitoringCore.DAL;
using FomMonitoringCore.DAL_SQLite;
using FomMonitoringCore.Framework.Common;
using Mapster;
using System;
using System.Collections.Generic;
using System.Linq;
using HistoryMessage = FomMonitoringCore.DAL.HistoryMessage;
using MessageMachine = FomMonitoringCore.DAL.MessageMachine;
using ToolMachine = FomMonitoringCore.DAL.ToolMachine;

namespace FomMonitoringCore.Service.DataMapping
{
    public class SqLiteToSqlServerService : ISqLiteToSqlServerService
    {
        private readonly IMachineService _machineService;
        private readonly IJobService _jobService;
        private readonly IBarService _barService;
        private readonly IFomMonitoringEntities _fomMonitoringEntities;
        private readonly IFomMonitoringSQLiteEntities _fomMonitoringSqLiteEntities;
        private readonly IMesService _mesService;

        public SqLiteToSqlServerService(IMachineService machineService, IJobService jobService,
            IFomMonitoringEntities fomMonitoringEntities, IFomMonitoringSQLiteEntities fomMonitoringSqLiteEntities,
            IBarService barService, IMesService mesService)
        {
            _machineService = machineService;
            _jobService = jobService;
            _fomMonitoringEntities = fomMonitoringEntities;
            _fomMonitoringSqLiteEntities = fomMonitoringSqLiteEntities;
            _barService = barService;
            _mesService = mesService;
        }

        public bool MappingSqLiteDetailsToSqlServer()
        {
            var result = false;
            try
            {

                var barSqLite = _fomMonitoringSqLiteEntities.Set<bar>().ToList();
                var historyJobSqLite = _fomMonitoringSqLiteEntities.Set<historyJob>().ToList();
                var infoSqLite = _fomMonitoringSqLiteEntities.Set<info>().ToList();
                var pieceSqLite = _fomMonitoringSqLiteEntities.Set<piece>().ToList();
                var spindleSqLite = _fomMonitoringSqLiteEntities.Set<spindle>().ToList();
                var stateSqLite = _fomMonitoringSqLiteEntities.Set<state>().ToList();
                var toolSqLite = _fomMonitoringSqLiteEntities.Set<tool>().ToList();
                var messageSqLite = _fomMonitoringSqLiteEntities.Set<message>().ToList();
                

                var matricola = infoSqLite.OrderByDescending(o => o.Id).FirstOrDefault()?.MachineSerial;

                var machine = infoSqLite.BuildAdapter().AddParameters("machineService", _machineService).AddParameters("mesService", _mesService).AdaptToType<List<Machine>>();                
                var machineActual = _fomMonitoringEntities.Set<Machine>().FirstOrDefault(f => f.Serial == matricola);
                if (machineActual == null)
                {
                    machineActual = machine.OrderByDescending(o => o.Id).FirstOrDefault();
                    if (machineActual != null)
                    {
                        machineActual.Id = 0;
                        _fomMonitoringEntities.Set<Machine>().Add(machineActual);
                    }

                    _fomMonitoringEntities.SaveChanges();
                }
                else
                {
                    var machineFromSqLite = machine.OrderByDescending(o => o.Id).FirstOrDefault();
                    /*foreach (PropertyInfo property in machineActual.GetType().GetProperties())
                    {
                        if (property.PropertyType.IsSerializable && (property.Name != "Id"))
                        {
                            property.SetValue(machineActual, machineFromSQLite.GetType().GetProperty(property.Name).GetValue(machineFromSQLite));
                        }
                    }*/
                    if (machineFromSqLite != null)
                    {
                        machineActual.Description = machineFromSqLite.Description;
                        machineActual.FirmwareVersion = machineFromSqLite.FirmwareVersion;
                        machineActual.InstallationDate = machineFromSqLite.InstallationDate;
                        machineActual.KeyId = machineFromSqLite.KeyId;
                        machineActual.LoginDate = machineFromSqLite.LoginDate;
                        machineActual.MachineModelId = machineFromSqLite.MachineModelId;
                        machineActual.MachineTypeId = machineFromSqLite.MachineTypeId;
                        machineActual.NextMaintenanceService = machineFromSqLite.NextMaintenanceService;
                        machineActual.PlcVersion = machineFromSqLite.PlcVersion;
                        machineActual.Product = machineFromSqLite.Product;
                        machineActual.ProductVersion = machineFromSqLite.ProductVersion;
                        machineActual.Serial = machineFromSqLite.Serial;
                        machineActual.Shift1 = machineFromSqLite.Shift1;
                        machineActual.Shift2 = machineFromSqLite.Shift2;
                        machineActual.Shift3 = machineFromSqLite.Shift3;
                        machineActual.BarsProductivityGreenThreshold = machineFromSqLite.BarsProductivityGreenThreshold;
                        machineActual.BarsProductivityYellowThreshold =
                            machineFromSqLite.BarsProductivityYellowThreshold;
                        machineActual.LastUpdate = machineFromSqLite.LastUpdate;
                        machineActual.OverfeedGreenThreshold = machineFromSqLite.OverfeedGreenThreshold;
                        machineActual.OverfeedYellowThreshold = machineFromSqLite.OverfeedYellowThreshold;
                        machineActual.PiecesProductivityGreenThreshold =
                            machineFromSqLite.PiecesProductivityGreenThreshold;
                        machineActual.PiecesProductivityYellowThreshold =
                            machineFromSqLite.PiecesProductivityYellowThreshold;
                        machineActual.StateProductivityGreenThreshold =
                            machineFromSqLite.StateProductivityGreenThreshold;
                        machineActual.StateProductivityYellowThreshold =
                            machineFromSqLite.StateProductivityYellowThreshold;
                        machineActual.UTC = machineFromSqLite.UTC;
                    }
                }
                _fomMonitoringEntities.SaveChanges();

                if (machineActual != null)
                {
                    var bar = barSqLite.BuildAdapter().AddParameters("machineId", machineActual.Id).AdaptToType<List<Bar>>();
                    bar = bar.Where(w => w.StartTime > (machineActual.LastUpdate ?? new DateTime())).ToList();

                    foreach (var bb in bar)
                    {
                        var trovato = _fomMonitoringEntities.Set<Bar>().FirstOrDefault(m => m.Index == bb.Index && m.JobCode == bb.JobCode && bb.MachineId == m.MachineId);
                        // se esiste già una barra con lo stesso jobcode e index uguale non aggiungo il record
                        if (trovato != null && trovato.JobCode != null)
                        {
                        }
                        else
                        {
                            _fomMonitoringEntities.Set<Bar>().Add(bb);
                            _fomMonitoringEntities.SaveChanges();
                        }                            
                    }
                }

                //_FomMonitoringEntities.Set<Bar>().AddRange(bar);
                var cat = _fomMonitoringEntities.Set<MachineModel>().Find(machineActual.MachineModelId)?.MessageCategoryId;
                messageSqLite = messageSqLite.Where(w => w.Time > (machineActual.LastUpdate ?? new DateTime())).ToList();

                //devo eliminare quei messaggi che hanno isLolaVisible = 0 da mdb
                List<MessageMachine> messageMachine = new List<MessageMachine>();

                foreach (var mm in messageSqLite)
                {
                    MessageMachine message = mm.BuildAdapter().AddParameters("machineId", machineActual.Id).AdaptToType<MessageMachine>();
                    var msg = _fomMonitoringEntities.Set<MessagesIndex>().FirstOrDefault(f => f.MessageCode == mm.Code && f.MessageCategoryId == cat);
                    message.Id = 0;
                    if (msg != null)
                    {
                        int old = _fomMonitoringEntities.Set<MessageMachine>().Count(a => a.MessagesIndexId == msg.Id &&
                                                                                a.MachineId == message.MachineId &&
                                                                                a.StartTime.Value.CompareTo(message.StartTime.Value) == 0);
                        if (old == 0)
                        {
                            message.MessagesIndexId = msg.Id;
                            message.MessagesIndex = msg;
                            messageMachine.Add(message);
                        }
                    }
                }

                //IS-754 escludere tutti quelli che hanno isLolaVisible = false && type error o warning 
                messageMachine = messageMachine.Where(f => f.MessagesIndex!= null 
                                                           && f.MessagesIndex.IsVisibleLOLA 
                                                           && f.MessagesIndex.MessageType != null 
                                                           && (f.MessagesIndex.MessageType.Id == 11 || f.MessagesIndex.MessageType.Id == 12)).ToList();
                if (messageMachine.Count() > 0)
                {
                    _fomMonitoringEntities.Set<MessageMachine>().AddRange(messageMachine);
                    _fomMonitoringEntities.SaveChanges();
                }
               
                        
                var historyJob = historyJobSqLite.BuildAdapter().AddParameters("machineId", machineActual.Id).AdaptToType<List<HistoryJob>>();
               /* DateTime minDateHistoryJob = historyJob.Any(a => a.Day.HasValue) ? historyJob.Where(w => w.Day.HasValue && w.MachineId == machineActual.Id).Select(s => s.Day).Min().Value : DateTime.UtcNow;
                List<HistoryJob> removeHistoryJob = _FomMonitoringEntities.Set<HistoryJob>().Where(w => w.Day.HasValue && w.Day.Value >= minDateHistoryJob && w.MachineId == machineActual.Id).ToList();
                if (removeHistoryJob != null)
                {
                    _FomMonitoringEntities.Set<HistoryJob>().RemoveRange(removeHistoryJob);
                    _FomMonitoringEntities.SaveChanges();
                }*/
                if (historyJob != null)
                {
                    _fomMonitoringEntities.Set<HistoryJob>().AddRange(historyJob);
                    _fomMonitoringEntities.SaveChanges();
                }
                       
                var piece = pieceSqLite.BuildAdapter().AddParameters("barService", _barService).AddParameters("jobService", _jobService).AddParameters("machineService", _machineService).AddParameters("machineId", machineActual.Id).AdaptToType<List<Piece>>();
                piece = piece.Where(w => w.EndTime > (machineActual.LastUpdate ?? new DateTime())).ToList();
                _fomMonitoringEntities.Set<Piece>().AddRange(piece);
                _fomMonitoringEntities.SaveChanges();

                var state = stateSqLite.BuildAdapter().AddParameters("machineService", _machineService).AddParameters("machineId", machineActual.Id).AdaptToType<List<StateMachine>>();
                state = state.Where(w => w.EndTime > (machineActual.LastUpdate ?? new DateTime())).ToList();

                _fomMonitoringEntities.Set<StateMachine>().AddRange(state);
                _fomMonitoringEntities.SaveChanges();
               
                var tool = toolSqLite.BuildAdapter().AddParameters("machineId", machineActual.Id).AdaptToType<List<ToolMachine>>();
                if (tool != null && tool.Any())
                {
                    var removeTools = toolSqLite.Join(_fomMonitoringEntities.Set<ToolMachine>(), to => to.Code, from => from.Code, (to, from) => new { From = from, To = to })
                        .Where(w => w.From.MachineId == machineActual.Id && w.From.Code == w.To.Code && w.From.DateLoaded == w.To.DateLoaded).Select(s => s.From).ToList();
                    _fomMonitoringEntities.Set<ToolMachine>().RemoveRange(removeTools);
                    _fomMonitoringEntities.SaveChanges();
                    var modifyTools = _fomMonitoringEntities.Set<ToolMachine>().Where(w => w.MachineId == machineActual.Id && w.IsActive).ToList();
                    foreach (var modifyTool in modifyTools)
                    {
                        modifyTool.IsActive = false;
                    }
                    _fomMonitoringEntities.SaveChanges();
                }
                _fomMonitoringEntities.Set<ToolMachine>().AddRange(tool);
                _fomMonitoringEntities.SaveChanges();

                var lastState = _fomMonitoringEntities.Set<StateMachine>().Where(w => w.MachineId == machineActual.Id).OrderByDescending(o => o.EndTime).FirstOrDefault();
                machineActual = _fomMonitoringEntities.Set<Machine>().FirstOrDefault(f => f.Id == machineActual.Id);
                machineActual.LastUpdate = lastState?.EndTime ?? DateTime.UtcNow;
                _fomMonitoringEntities.SaveChanges();

                _fomMonitoringEntities.usp_HistoricizingAll(machineActual.Id);
                _fomMonitoringEntities.SaveChanges();
                
                result = true;
                
                
            }
            catch (Exception ex)
            {
                var errMessage = string.Format(ex.GetStringLog());
                LogService.WriteLog(errMessage, LogService.TypeLevel.Error, ex);
            }
            return result;
        }

        public bool MappingSqLiteHistoryToSqlServer()
        {
            var result = false;
            try
            {
                var historyBarSqLite = _fomMonitoringSqLiteEntities.Set<historyBar>().ToList();
                var historyJobSqLite = _fomMonitoringSqLiteEntities.Set<historyJob>().ToList();
                var historyPieceSqLite = _fomMonitoringSqLiteEntities.Set<historyPiece>().ToList();
                var historyStateSqLite = _fomMonitoringSqLiteEntities.Set<historyState>().ToList();
                var historyMessageSqLite = _fomMonitoringSqLiteEntities.Set<historyMessage>().ToList();
                var infoSqLite = _fomMonitoringSqLiteEntities.Set<info>().ToList();
                var spindleSqLite = _fomMonitoringSqLiteEntities.Set<spindle>().ToList();
                var toolSqLite = _fomMonitoringSqLiteEntities.Set<tool>().ToList();
                
                var matricola = infoSqLite.OrderByDescending(o => o.Id).FirstOrDefault()?.MachineSerial;
                
                var machine = infoSqLite.Adapt<List<Machine>>();
                var machineActual = _fomMonitoringEntities.Set<Machine>().FirstOrDefault(f => f.Serial == matricola);
                if (machineActual == null)
                {
                    machineActual = machine.OrderByDescending(o => o.Id).FirstOrDefault();
                    if (machineActual != null) machineActual.Id = 0;
                    _fomMonitoringEntities.Set<Machine>().Add(machine.OrderByDescending(o => o.Id).FirstOrDefault());
                }
                else
                {
                    var machineFromSqLite = machine.OrderByDescending(o => o.Id).FirstOrDefault();
                    foreach (var property in machineActual.GetType().GetProperties())
                    {
                        if (property.PropertyType.IsSerializable && property.Name != "Id")
                        {
                            if (machineFromSqLite != null)
                                property.SetValue(machineActual,
                                    machineFromSqLite.GetType().GetProperty(property.Name)?.GetValue(machineFromSqLite));
                        }
                    }
                }
                _fomMonitoringEntities.SaveChanges();


                var historyMessage = historyMessageSqLite.BuildAdapter().AddParameters("machineId", machineActual.Id).AdaptToType<List<HistoryMessage>>();
                var minDateHistoryMessage = historyMessage.Any(a => a.Day.HasValue) ? historyMessage.Where(w => w.Day.HasValue && w.MachineId == machineActual.Id).Select(s => s.Day).Min().Value : new DateTime();
                var removeHistoryMessage = _fomMonitoringEntities.Set<HistoryMessage>().Where(w => w.Day.HasValue && w.Day.Value >= minDateHistoryMessage && w.MachineId == machineActual.Id).ToList();
                _fomMonitoringEntities.Set<HistoryMessage>().RemoveRange(removeHistoryMessage);
                _fomMonitoringEntities.SaveChanges();
                _fomMonitoringEntities.Set<HistoryMessage>().AddRange(historyMessage);
                _fomMonitoringEntities.SaveChanges();

                var historyBar = historyBarSqLite.BuildAdapter().AddParameters("machineId", machineActual.Id).AdaptToType<List<HistoryBar>>();
                var minDateHistoryBar = historyBar.Any(a => a.Day.HasValue) ? historyBar.Where(w => w.Day.HasValue && w.MachineId == machineActual.Id).Select(s => s.Day).Min().Value : new DateTime();
                var removeHistoryBar = _fomMonitoringEntities.Set<HistoryBar>().Where(w => w.Day.HasValue && w.Day.Value >= minDateHistoryBar && w.MachineId == machineActual.Id).ToList();
                _fomMonitoringEntities.Set<HistoryBar>().RemoveRange(removeHistoryBar);
                _fomMonitoringEntities.SaveChanges();
                _fomMonitoringEntities.Set<HistoryBar>().AddRange(historyBar);
                _fomMonitoringEntities.SaveChanges();

                var historyJob = historyJobSqLite.BuildAdapter().AddParameters("machineId", machineActual.Id).AdaptToType<List<HistoryJob>>();
                var minDateHistoryJob = historyJob.Any(a => a.Day.HasValue) ? historyJob.Where(w => w.Day.HasValue && w.MachineId == machineActual.Id).Select(s => s.Day).Min().Value : new DateTime();
                var removeHistoryJob = _fomMonitoringEntities.Set<HistoryJob>().Where(w => w.Day.HasValue && w.Day.Value >= minDateHistoryJob && w.MachineId == machineActual.Id).ToList();
                _fomMonitoringEntities.Set<HistoryJob>().RemoveRange(removeHistoryJob);
                _fomMonitoringEntities.SaveChanges();
                _fomMonitoringEntities.Set<HistoryJob>().AddRange(historyJob);
                _fomMonitoringEntities.SaveChanges();

                var historyPiece = historyPieceSqLite.BuildAdapter().AddParameters("machineId", machineActual.Id).AdaptToType<List<HistoryPiece>>();
                var minDateHistoryPiece = historyPiece.Any(a => a.Day.HasValue) ? historyPiece.Where(w => w.Day.HasValue && w.MachineId == machineActual.Id).Select(s => s.Day).Min().Value : new DateTime();
                var removeHistoryPiece = _fomMonitoringEntities.Set<HistoryPiece>().Where(w => w.Day.HasValue && w.Day.Value >= minDateHistoryPiece && w.MachineId == machineActual.Id).ToList();
                _fomMonitoringEntities.Set<HistoryPiece>().RemoveRange(removeHistoryPiece);
                _fomMonitoringEntities.SaveChanges();
                _fomMonitoringEntities.Set<HistoryPiece>().AddRange(historyPiece);
                _fomMonitoringEntities.SaveChanges();

                var historyState = historyStateSqLite.BuildAdapter().AddParameters("machineId", machineActual.Id).AdaptToType<List<HistoryState>>();
                var minDateHistoryState = historyState.Any(a => a.Day.HasValue) ? historyState.Where(w => w.Day.HasValue && w.MachineId == machineActual.Id).Select(s => s.Day).Min().Value : new DateTime();
                var removeHistoryState = _fomMonitoringEntities.Set<HistoryState>().Where(w => w.Day.HasValue && w.Day.Value >= minDateHistoryState && w.MachineId == machineActual.Id).ToList();
                _fomMonitoringEntities.Set<HistoryState>().RemoveRange(removeHistoryState);
                _fomMonitoringEntities.SaveChanges();
                _fomMonitoringEntities.Set<HistoryState>().AddRange(historyState);
                _fomMonitoringEntities.SaveChanges();


                var tool = toolSqLite.BuildAdapter().AddParameters("machineId", machineActual.Id).AdaptToType<List<ToolMachine>>();
                var removeTool = _fomMonitoringEntities.Set<ToolMachine>().Where(w => w.MachineId == machineActual.Id).ToList();
                _fomMonitoringEntities.Set<ToolMachine>().RemoveRange(removeTool);
                _fomMonitoringEntities.SaveChanges();
                _fomMonitoringEntities.Set<ToolMachine>().AddRange(tool);
                _fomMonitoringEntities.SaveChanges();

                var lastState = _fomMonitoringEntities.Set<HistoryState>().Where(w => w.MachineId == machineActual.Id).OrderByDescending(o => o.Day).FirstOrDefault();
                machineActual = _fomMonitoringEntities.Set<Machine>().FirstOrDefault(f => f.Id == machineActual.Id);
                if (lastState != null && machineActual != null)
                    machineActual.LastUpdate = lastState.Day;

                _fomMonitoringEntities.SaveChanges();
                
                result = true;                                    
            }
            catch (Exception ex)
            {
                var errMessage = string.Format(ex.GetStringLog());
                LogService.WriteLog(errMessage, LogService.TypeLevel.Error, ex);
            }
            return result;
        }
    }
}
