using FomMonitoringCore.DAL;
using FomMonitoringCore.DAL_SQLite;
using FomMonitoringCore.Framework.Common;
using FomMonitoringCore.Uow;
using Mapster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Transactions;

namespace FomMonitoringCore.Service.DataMapping
{
    public class SQLiteToSQLServerService : ISQLiteToSQLServerService
    {
        private IMachineService _machineService;
        private IJobService _jobService;
        private IBarService _barService;
        private IFomMonitoringEntities _FomMonitoringEntities;
        private IFomMonitoringSQLiteEntities _FomMonitoringSQLiteEntities;
        private IUnitOfWork _unitOfWork;
        private IMesService _mesService;

        public SQLiteToSQLServerService(IMachineService machineService, IJobService jobService,
            IFomMonitoringEntities FomMonitoringEntities, IFomMonitoringSQLiteEntities FomMonitoringSQLiteEntities,
            IUnitOfWork unitOfWork, IBarService barService, IMesService mesService)
        {
            _machineService = machineService;
            _jobService = jobService;
            _FomMonitoringEntities = FomMonitoringEntities;
            _FomMonitoringSQLiteEntities = FomMonitoringSQLiteEntities;
            _unitOfWork = unitOfWork;
            _barService = barService;
            _mesService = mesService;
        }

        public bool MappingSQLiteDetailsToSQLServer()
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
                
                barSQLite = _FomMonitoringSQLiteEntities.Set<bar>().ToList();
                errorSQLite = _FomMonitoringSQLiteEntities.Set<error>().ToList();
                historyJobSQLite = _FomMonitoringSQLiteEntities.Set<historyJob>().ToList();
                infoSQLite = _FomMonitoringSQLiteEntities.Set<info>().ToList();
                pieceSQLite = _FomMonitoringSQLiteEntities.Set<piece>().ToList();
                spindleSQLite = _FomMonitoringSQLiteEntities.Set<spindle>().ToList();
                stateSQLite = _FomMonitoringSQLiteEntities.Set<state>().ToList();
                toolSQLite = _FomMonitoringSQLiteEntities.Set<tool>().ToList();
                messageSQLite = _FomMonitoringSQLiteEntities.Set<message>().ToList();
                

                string matricola = infoSQLite.OrderByDescending(o => o.Id).FirstOrDefault().MachineSerial;
                _unitOfWork.StartTransaction(_FomMonitoringEntities);

                List<Machine> machine = infoSQLite.BuildAdapter().AddParameters("machineService", _machineService).AddParameters("mesService", _mesService).AdaptToType<List<Machine>>();                
                Machine machineActual = _FomMonitoringEntities.Set<Machine>().FirstOrDefault(f => f.Serial == matricola);
                if (machineActual == null)
                {
                    machineActual = machine.OrderByDescending(o => o.Id).FirstOrDefault();
                    machineActual.Id = 0;
                    _FomMonitoringEntities.Set<Machine>().Add(machineActual);
                    _FomMonitoringEntities.SaveChanges();
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
                    machineActual.PlantId = machineFromSQLite.PlantId;


                }
                _FomMonitoringEntities.SaveChanges();

                List<Bar> bar = barSQLite.BuildAdapter().AddParameters("machineId", machineActual.Id).AdaptToType<List<Bar>>();
                bar = bar.Where(w => w.StartTime > (machineActual.LastUpdate ?? new DateTime())).ToList();

                if(bar != null)
                {
                    foreach (Bar bb in bar)
                    {
                        Bar trovato = _FomMonitoringEntities.Set<Bar>().FirstOrDefault(m => m.Index == bb.Index && m.JobCode == bb.JobCode && bb.MachineId == m.MachineId);
                        // se esiste già una barra con lo stesso jobcode e index uguale non aggiungo il record
                        if (trovato != null && trovato.JobCode != null)
                        {
                            continue;
                        }
                        else
                        {
                            _FomMonitoringEntities.Set<Bar>().Add(bb);
                            _FomMonitoringEntities.SaveChanges();
                        }                            
                    }
                }
                //_FomMonitoringEntities.Set<Bar>().AddRange(bar);
                List<MessageMachine> messageMachine = messageSQLite.BuildAdapter().AddParameters("machineId", machineActual.Id).AdaptToType<List<MessageMachine>>();
                messageMachine = messageMachine.Where(w => w.StartTime > (machineActual.LastUpdate ?? new DateTime())).ToList();

                //devo eliminare quei messaggi che hanno scope = 0 da mdb
                foreach(MessageMachine mm in messageMachine.ToList())
                {
                    int cat = _FomMonitoringEntities.Set<MachineModel>().Find(machineActual.MachineModelId).MessageCategoryId;
                    MessagesIndex msg = _FomMonitoringEntities.Set<MessagesIndex>().FirstOrDefault(f => f.MessageCode == mm.Code && f.MessageCategoryId == cat);
                    if (msg != null && msg.IsPeriodicM == true)
                        mm.IsPeriodicMsg = true;
                    ReadMessages.ReadMessageVisibilityGroup(mm, msg);
                }                                      

                _FomMonitoringEntities.Set<MessageMachine>().AddRange(messageMachine);
                _FomMonitoringEntities.SaveChanges();
                        
                List<HistoryJob> historyJob = historyJobSQLite.BuildAdapter().AddParameters("machineId", machineActual.Id).AdaptToType<List<HistoryJob>>();
                DateTime minDateHistoryJob = historyJob.Any(a => a.Day.HasValue) ? historyJob.Where(w => w.Day.HasValue && w.MachineId == machineActual.Id).Select(s => s.Day).Min().Value : new DateTime();
                List<HistoryJob> removeHistoryJob = _FomMonitoringEntities.Set<HistoryJob>().Where(w => w.Day.HasValue && w.Day.Value >= minDateHistoryJob && w.MachineId == machineActual.Id).ToList();
                _FomMonitoringEntities.Set<HistoryJob>().RemoveRange(removeHistoryJob);
                _FomMonitoringEntities.SaveChanges();
                _FomMonitoringEntities.Set<HistoryJob>().AddRange(historyJob);
                _FomMonitoringEntities.SaveChanges();
                       
                List<Piece> piece = pieceSQLite.BuildAdapter().AddParameters("barService", _barService).AddParameters("jobService", _jobService).AddParameters("machineService", _machineService).AddParameters("machineId", machineActual.Id).AdaptToType<List<Piece>>();
                piece = piece.Where(w => w.EndTime > (machineActual.LastUpdate ?? new DateTime())).ToList();
                _FomMonitoringEntities.Set<Piece>().AddRange(piece);
                _FomMonitoringEntities.SaveChanges();

                List<Spindle> spindle = spindleSQLite.BuildAdapter().AddParameters("machineId", machineActual.Id).AdaptToType<List<Spindle>>();
                List<Spindle> removeSpindles = _FomMonitoringEntities.Set<Spindle>().Where(w => w.MachineId == machineActual.Id).ToList();
                _FomMonitoringEntities.Set<Spindle>().RemoveRange(removeSpindles);
                _FomMonitoringEntities.SaveChanges();
                _FomMonitoringEntities.Set<Spindle>().AddRange(spindle);
                _FomMonitoringEntities.SaveChanges();

                List<StateMachine> state = stateSQLite.BuildAdapter().AddParameters("machineService", _machineService).AddParameters("machineId", machineActual.Id).AdaptToType<List<StateMachine>>();
                state = state.Where(w => w.EndTime > (machineActual.LastUpdate ?? new DateTime())).ToList();
                _FomMonitoringEntities.Set<StateMachine>().AddRange(state);
                _FomMonitoringEntities.SaveChanges();

                List<ToolMachine> tool = toolSQLite.BuildAdapter().AddParameters("machineId", machineActual.Id).AdaptToType<List<ToolMachine>>();
                List<ToolMachine> removeTools = toolSQLite.Join(_FomMonitoringEntities.Set<ToolMachine>(), to => to.Code, from => from.Code, (to, from) => new { From = from, To = to })
                    .Where(w => w.From.MachineId == machineActual.Id && w.From.Code == w.To.Code && w.From.DateLoaded == w.To.DateLoaded).Select(s => s.From).ToList();
                _FomMonitoringEntities.Set<ToolMachine>().RemoveRange(removeTools);
                _FomMonitoringEntities.SaveChanges();
                List<ToolMachine> modifyTools = _FomMonitoringEntities.Set<ToolMachine>().Where(w => w.MachineId == machineActual.Id && w.IsActive).ToList();
                foreach (ToolMachine modifyTool in modifyTools)
                {
                    modifyTool.IsActive = false;
                }
                _FomMonitoringEntities.SaveChanges();
                _FomMonitoringEntities.Set<ToolMachine>().AddRange(tool);
                _FomMonitoringEntities.SaveChanges();

                StateMachine lastState = _FomMonitoringEntities.Set<StateMachine>().Where(w => w.MachineId == machineActual.Id).OrderByDescending(o => o.EndTime).FirstOrDefault();
                machineActual = _FomMonitoringEntities.Set<Machine>().FirstOrDefault(f => f.Id == machineActual.Id);
                machineActual.LastUpdate = lastState?.EndTime ?? DateTime.UtcNow;
                _FomMonitoringEntities.SaveChanges();

                _FomMonitoringEntities.usp_HistoricizingAll(machineActual.Id);
                _FomMonitoringEntities.SaveChanges();

                _unitOfWork.CommitTransaction();
                result = true;
                
                
            }
            catch (Exception ex)
            {
                string errMessage = string.Format(ex.GetStringLog());
                LogService.WriteLog(errMessage, LogService.TypeLevel.Error, ex);
                _unitOfWork.RollbackTransaction();
            }
            return result;
        }

        public bool MappingSQLiteHistoryToSQLServer()
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
               
                historyAlarmSQLite = _FomMonitoringSQLiteEntities.Set<historyAlarm>().ToList();
                historyBarSQLite = _FomMonitoringSQLiteEntities.Set<historyBar>().ToList();
                historyJobSQLite = _FomMonitoringSQLiteEntities.Set<historyJob>().ToList();
                historyPieceSQLite = _FomMonitoringSQLiteEntities.Set<historyPiece>().ToList();
                historyStateSQLite = _FomMonitoringSQLiteEntities.Set<historyState>().ToList();
                historyMessageSQLite = _FomMonitoringSQLiteEntities.Set<historyMessage>().ToList();
                infoSQLite = _FomMonitoringSQLiteEntities.Set<info>().ToList();
                spindleSQLite = _FomMonitoringSQLiteEntities.Set<spindle>().ToList();
                toolSQLite = _FomMonitoringSQLiteEntities.Set<tool>().ToList();
                
                string matricola = infoSQLite.OrderByDescending(o => o.Id).FirstOrDefault().MachineSerial;

                _unitOfWork.StartTransaction(_FomMonitoringEntities);
                List<Machine> machine = infoSQLite.Adapt<List<Machine>>();
                Machine machineActual = _FomMonitoringEntities.Set<Machine>().FirstOrDefault(f => f.Serial == matricola);
                if (machineActual == null)
                {
                    machineActual = machine.OrderByDescending(o => o.Id).FirstOrDefault();
                    machineActual.Id = 0;
                    _FomMonitoringEntities.Set<Machine>().Add(machine.OrderByDescending(o => o.Id).FirstOrDefault());
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
                _FomMonitoringEntities.SaveChanges();

                List<HistoryAlarm> historyAlarm = historyAlarmSQLite.BuildAdapter().AddParameters("machineId", machineActual.Id).AdaptToType<List<HistoryAlarm>>();
                DateTime minDateHistoryAlarm = historyAlarm.Any(a => a.Day.HasValue) ? historyAlarm.Where(w => w.Day.HasValue && w.MachineId == machineActual.Id).Select(s => s.Day).Min().Value : new DateTime();
                List<HistoryAlarm> removeHistoryAlarm = _FomMonitoringEntities.Set<HistoryAlarm>().Where(w => w.Day.HasValue && w.Day.Value >= minDateHistoryAlarm && w.MachineId == machineActual.Id).ToList();
                _FomMonitoringEntities.Set<HistoryAlarm>().RemoveRange(removeHistoryAlarm);
                _FomMonitoringEntities.SaveChanges();
                _FomMonitoringEntities.Set<HistoryAlarm>().AddRange(historyAlarm);
                _FomMonitoringEntities.SaveChanges();

                List<HistoryMessage> historyMessage = historyMessageSQLite.BuildAdapter().AddParameters("machineId", machineActual.Id).AdaptToType<List<HistoryMessage>>();
                DateTime minDateHistoryMessage = historyMessage.Any(a => a.Day.HasValue) ? historyMessage.Where(w => w.Day.HasValue && w.MachineId == machineActual.Id).Select(s => s.Day).Min().Value : new DateTime();
                List<HistoryMessage> removeHistoryMessage = _FomMonitoringEntities.Set<HistoryMessage>().Where(w => w.Day.HasValue && w.Day.Value >= minDateHistoryMessage && w.MachineId == machineActual.Id).ToList();
                _FomMonitoringEntities.Set<HistoryMessage>().RemoveRange(removeHistoryMessage);
                _FomMonitoringEntities.SaveChanges();
                _FomMonitoringEntities.Set<HistoryMessage>().AddRange(historyMessage);
                _FomMonitoringEntities.SaveChanges();

                List<HistoryBar> historyBar = historyBarSQLite.BuildAdapter().AddParameters("machineId", machineActual.Id).AdaptToType<List<HistoryBar>>();
                DateTime minDateHistoryBar = historyBar.Any(a => a.Day.HasValue) ? historyBar.Where(w => w.Day.HasValue && w.MachineId == machineActual.Id).Select(s => s.Day).Min().Value : new DateTime();
                List<HistoryBar> removeHistoryBar = _FomMonitoringEntities.Set<HistoryBar>().Where(w => w.Day.HasValue && w.Day.Value >= minDateHistoryBar && w.MachineId == machineActual.Id).ToList();
                _FomMonitoringEntities.Set<HistoryBar>().RemoveRange(removeHistoryBar);
                _FomMonitoringEntities.SaveChanges();
                _FomMonitoringEntities.Set<HistoryBar>().AddRange(historyBar);
                _FomMonitoringEntities.SaveChanges();

                List<HistoryJob> historyJob = historyJobSQLite.BuildAdapter().AddParameters("machineId", machineActual.Id).AdaptToType<List<HistoryJob>>();
                DateTime minDateHistoryJob = historyJob.Any(a => a.Day.HasValue) ? historyJob.Where(w => w.Day.HasValue && w.MachineId == machineActual.Id).Select(s => s.Day).Min().Value : new DateTime();
                List<HistoryJob> removeHistoryJob = _FomMonitoringEntities.Set<HistoryJob>().Where(w => w.Day.HasValue && w.Day.Value >= minDateHistoryJob && w.MachineId == machineActual.Id).ToList();
                _FomMonitoringEntities.Set<HistoryJob>().RemoveRange(removeHistoryJob);
                _FomMonitoringEntities.SaveChanges();
                _FomMonitoringEntities.Set<HistoryJob>().AddRange(historyJob);
                _FomMonitoringEntities.SaveChanges();

                List<HistoryPiece> historyPiece = historyPieceSQLite.BuildAdapter().AddParameters("machineId", machineActual.Id).AdaptToType<List<HistoryPiece>>();
                DateTime minDateHistoryPiece = historyPiece.Any(a => a.Day.HasValue) ? historyPiece.Where(w => w.Day.HasValue && w.MachineId == machineActual.Id).Select(s => s.Day).Min().Value : new DateTime();
                List<HistoryPiece> removeHistoryPiece = _FomMonitoringEntities.Set<HistoryPiece>().Where(w => w.Day.HasValue && w.Day.Value >= minDateHistoryPiece && w.MachineId == machineActual.Id).ToList();
                _FomMonitoringEntities.Set<HistoryPiece>().RemoveRange(removeHistoryPiece);
                _FomMonitoringEntities.SaveChanges();
                _FomMonitoringEntities.Set<HistoryPiece>().AddRange(historyPiece);
                _FomMonitoringEntities.SaveChanges();

                List<HistoryState> historyState = historyStateSQLite.BuildAdapter().AddParameters("machineId", machineActual.Id).AdaptToType<List<HistoryState>>();
                DateTime minDateHistoryState = historyState.Any(a => a.Day.HasValue) ? historyState.Where(w => w.Day.HasValue && w.MachineId == machineActual.Id).Select(s => s.Day).Min().Value : new DateTime();
                List<HistoryState> removeHistoryState = _FomMonitoringEntities.Set<HistoryState>().Where(w => w.Day.HasValue && w.Day.Value >= minDateHistoryState && w.MachineId == machineActual.Id).ToList();
                _FomMonitoringEntities.Set<HistoryState>().RemoveRange(removeHistoryState);
                _FomMonitoringEntities.SaveChanges();
                _FomMonitoringEntities.Set<HistoryState>().AddRange(historyState);
                _FomMonitoringEntities.SaveChanges();

                List<Spindle> spindle = spindleSQLite.BuildAdapter().AddParameters("machineId", machineActual.Id).AdaptToType<List<Spindle>>();
                List<Spindle> removeSpindle = _FomMonitoringEntities.Set<Spindle>().Where(w => w.MachineId == machineActual.Id).ToList();
                _FomMonitoringEntities.Set<Spindle>().RemoveRange(removeSpindle);
                _FomMonitoringEntities.SaveChanges();
                _FomMonitoringEntities.Set<Spindle>().AddRange(spindle);
                _FomMonitoringEntities.SaveChanges();

                List<ToolMachine> tool = toolSQLite.BuildAdapter().AddParameters("machineId", machineActual.Id).AdaptToType<List<ToolMachine>>();
                List<ToolMachine> removeTool = _FomMonitoringEntities.Set<ToolMachine>().Where(w => w.MachineId == machineActual.Id).ToList();
                _FomMonitoringEntities.Set<ToolMachine>().RemoveRange(removeTool);
                _FomMonitoringEntities.SaveChanges();
                _FomMonitoringEntities.Set<ToolMachine>().AddRange(tool);
                _FomMonitoringEntities.SaveChanges();

                HistoryState lastState = _FomMonitoringEntities.Set<HistoryState>().Where(w => w.MachineId == machineActual.Id).OrderByDescending(o => o.Day).FirstOrDefault();
                machineActual = _FomMonitoringEntities.Set<Machine>().FirstOrDefault(f => f.Id == machineActual.Id);
                machineActual.LastUpdate = lastState.Day;
                _FomMonitoringEntities.SaveChanges();

                _unitOfWork.CommitTransaction();
                result = true;                                    
            }
            catch (Exception ex)
            {
                string errMessage = string.Format(ex.GetStringLog());
                LogService.WriteLog(errMessage, LogService.TypeLevel.Error, ex);
                _unitOfWork.RollbackTransaction();
            }
            return result;
        }
    }
}
