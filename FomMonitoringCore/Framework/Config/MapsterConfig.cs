using FomMonitoringCore.DAL;
using FomMonitoringCore.Framework.Common;
using FomMonitoringCore.Framework.Model;
using FomMonitoringCore.Service;
using Mapster;
using System;
using System.Linq;
using UserManager.DAL;

namespace FomMonitoringCore.Framework.Config
{
    public class MapsterConfig : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            TypeAdapterConfig.GlobalSettings.AllowImplicitDestinationInheritance = true;
            TypeAdapterConfig.GlobalSettings.Default.NameMatchingStrategy(NameMatchingStrategy.Flexible);
            TypeAdapterConfig.GlobalSettings.Default.PreserveReference(true);
            TypeAdapterConfig.GlobalSettings.Default.ShallowCopyForSameType(true);
            TypeAdapterConfig.GlobalSettings.Default.AddDestinationTransform((string x) => x.Trim());

            // DAL to Model
            config.NewConfig<AlarmMachine, AlarmMachineModel>();
            config.NewConfig<MessageMachine, MessageMachineModel>();
            config.NewConfig<Bar, BarModel>();
            config.NewConfig<HistoryAlarm, HistoryAlarmModel>()
                .Map(dest => dest.enState, src => (enState)src.StateId);
            config.NewConfig<HistoryMessage, HistoryMessageModel>()
                .Map(dest => dest.enState, src => (enState)src.StateId);
            config.NewConfig<HistoryPiece, HistoryPieceModel>();
            config.NewConfig<HistoryState, HistoryStateModel>()
                .Map(dest => dest.State, src => src.State)
                .Map(dest => dest.enState, src => (enState)src.StateId);
            config.NewConfig<HistoryJob, HistoryJobModel>();
            config.NewConfig<Machine, MachineInfoModel>()
                .Map(dest => dest.Model, src => src.MachineModel)
                .Map(dest => dest.Type, src => src.MachineType)
                .Map(dest => dest.Plant, src => src.Plant);
            config.NewConfig<MachineModel, MachineModelModel>();
            config.NewConfig<MachineType, MachineTypeModel>();
            config.NewConfig<Piece, PieceModel>();
            config.NewConfig<Spindle, SpindleModel>()
                .Ignore(dest => dest.ChangeCount)
                .Map(dest => dest.AverageElapsedTimeWork, src => src.ElapsedTimeWorkTotal.HasValue && src.ElapsedTimeWorkTotal > 0 ?
                                                                    (
                                                                        ((decimal)(src.ElapsedTimeWork3K ?? 0) * 3) + ((decimal)(src.ElapsedTimeWork6K ?? 0) * 6) +
                                                                        ((decimal)(src.ElapsedTimeWork9K ?? 0) * 9) + ((decimal)(src.ElapsedTimeWork12K ?? 0) * 12) +
                                                                        ((decimal)(src.ElapsedTimeWork15K ?? 0) * 15) + ((decimal)(src.ElapsedTimeWork18K ?? 0) * 18)
                                                                    )
                                                                    * 1000 / (decimal)(src.ElapsedTimeWorkTotal)
                                                                 : 0);
            config.NewConfig<State, StateModel>()
                .Map(dest => dest.Code, src => src.Description);
            config.NewConfig<StateMachine, StateMachineModel>();
            config.NewConfig<ToolMachine, ToolMachineModel>();
            config.NewConfig<UserMachineMapping, UserMachineMappingModel>()
                .Map(dest => dest.Machine, src => src.Machine);
            config.NewConfig<JsonData, JsonDataModel>();
            config.NewConfig<UserCustomerMapping, UserCustomerModel>()
                .Map(dest => dest.Username, src => src.CustomerName);
            config.NewConfig<Users, UserModel>()
                .Map(dest => dest.Role, src => (src.Roles_Users.Count > 0) ? (enRole)src.Roles_Users.First().Roles.IdRole : enRole.Operator)
                .Map(dest => dest.Language, src => src.Languages);
            config.NewConfig<Roles, RoleModel>()
                .Map(dest => dest.Code, src => src.IdRole);
            config.NewConfig<UserCustomerMapping, UserCustomerModel>();

            // SP to Model
            config.NewConfig<usp_AggregationState_Result, HistoryStateModel>()
                .Map(dest => dest.enState, src => (enState)src.StateId);
            config.NewConfig<usp_AggregationAlarm_Result, HistoryAlarmModel>()
                .Map(dest => dest.enState, src => (enState)src.StateId);
            config.NewConfig<usp_MesUserMachines_Result, MesUserMachinesModel>()
                .Map(dest => dest.enActualState, src => (enState?)src.ActualStateId);

            // Model to DAL
            config.NewConfig<JsonDataModel, JsonData>();
            config.NewConfig<UserModel, Users>();

            // SQLite to SQLServer 
            config.NewConfig<DAL_SQLite.bar, Bar>()
                .Ignore(dest => dest.Id)
                .IgnoreAllVirtual()
                .Map(dest => dest.IdOld, src => src.Id)
                .Map(dest => dest.MachineId, src => MapContext.Current.Parameters["machineId"]);
            config.NewConfig<DAL_SQLite.error, AlarmMachine>()
                .Ignore(dest => dest.Id)
                .IgnoreAllVirtual()
                .Map(dest => dest.Day, src => src.Time.HasValue ? src.Time.Value : (DateTime?)null)
                .Map(dest => dest.StartTime, src => src.Time)
                .Map(dest => dest.ElapsedTime, src => src.TimeSpanDuration)
                .Map(dest => dest.StateId, src => src.State)
                .Map(dest => dest.MachineId, src => MapContext.Current.Parameters["machineId"]);

            config.NewConfig<DAL_SQLite.message, MessageMachine>()
                .Ignore(dest => dest.Id)
                .IgnoreAllVirtual()
                .Map(dest => dest.Day, src => src.Time.HasValue ? src.Time.Value : (DateTime?)null)
                .Map(dest => dest.StartTime, src => src.Time)
                .Map(dest => dest.ElapsedTime, src => src.TimeSpanDuration)
                .Map(dest => dest.StateId, src => src.State)
                .Map(dest => dest.Day, src => src.Code)
                .Map(dest => dest.MachineId, src => MapContext.Current.Parameters["machineId"]);

            config.NewConfig<DAL_SQLite.historyAlarm, HistoryAlarm>()
                .Ignore(dest => dest.Id)
                .IgnoreAllVirtual()
                .Map(dest => dest.Period, src => src.Day != null ? ((src.Day.Value.Year * 10000) + (src.Day.Value.Month * 100) + (src.Day.Value.Day)) : (int?)null)
                .Map(dest => dest.TypeHistory, src => "d")
                .Map(dest => dest.MachineId, src => MapContext.Current.Parameters["machineId"]);

            config.NewConfig<DAL_SQLite.historyMessage, HistoryMessage>()
                .Ignore(dest => dest.Id)
                .IgnoreAllVirtual()
                .Map(dest => dest.Period, src => src.Day != null ? ((src.Day.Value.Year * 10000) + (src.Day.Value.Month * 100) + (src.Day.Value.Day)) : (int?)null)
                .Map(dest => dest.TypeHistory, src => "d")
                .Map(dest => dest.MachineId, src => MapContext.Current.Parameters["machineId"]);

            config.NewConfig<DAL_SQLite.historyBar, HistoryBar>()
                .Ignore(dest => dest.Id)
                .IgnoreAllVirtual()
                .Map(dest => dest.Count, src => src.BarCount)
                .Map(dest => dest.Length, src => src.BarLengthSum)
                .Map(dest => dest.OffcutLength, src => src.OffcutLengthSum)
                .Map(dest => dest.OffcutCount, src => src.OffCutCount)
                .Map(dest => dest.Period, src => src.Day != null ? ((src.Day.Value.Year * 10000) + (src.Day.Value.Month * 100) + (src.Day.Value.Day)) : (int?)null)
                .Map(dest => dest.TypeHistory, src => "d")
                .Map(dest => dest.MachineId, src => MapContext.Current.Parameters["machineId"]);
            config.NewConfig<DAL_SQLite.historyJob, HistoryJob>()
                .Ignore(dest => dest.Id)
                .IgnoreAllVirtual()
                .Map(dest => dest.Code, src => src.JobCode)
                .Map(dest => dest.TotalPieces, src => src.TotalPiecesInJob)
                .Map(dest => dest.PiecesProduced, src => src.CurrentlyProducedPieces)
                .Map(dest => dest.PiecesProducedDay, src => src.CurrentPeriodPieces)
                .Map(dest => dest.Period, src => src.Day != null ? ((src.Day.Value.Year * 10000) + (src.Day.Value.Month * 100) + (src.Day.Value.Day)) : (int?)null)
                .Map(dest => dest.TypeHistory, src => "d")
                .Map(dest => dest.MachineId, src => MapContext.Current.Parameters["machineId"]);
            config.NewConfig<DAL_SQLite.historyPiece, HistoryPiece>()
                .Ignore(dest => dest.Id)
                .IgnoreAllVirtual()
                .Map(dest => dest.Period, src => src.Day != null ? ((src.Day.Value.Year * 10000) + (src.Day.Value.Month * 100) + (src.Day.Value.Day)) : (int?)null)
                .Map(dest => dest.TypeHistory, src => "d")
                .Map(dest => dest.MachineId, src => MapContext.Current.Parameters["machineId"]);
            config.NewConfig<DAL_SQLite.historyState, HistoryState>()
                .Ignore(dest => dest.Id)
                .IgnoreAllVirtual()
                .Map(dest => dest.Period, src => src.Day != null ? ((src.Day.Value.Year * 10000) + (src.Day.Value.Month * 100) + (src.Day.Value.Day)) : (int?)null)
                .Map(dest => dest.TypeHistory, src => "d")
                .Map(dest => dest.MachineId, src => MapContext.Current.Parameters["machineId"]);
            config.NewConfig<DAL_SQLite.info, Machine>()
                .IgnoreAllVirtual()
                .Map(dest => dest.Description, src => src.MachineDescription)
                .Map(dest => dest.Serial, src => src.MachineSerial)
                .Map(dest => dest.Shift1, src => src.Shift1StartHour.HasValue && src.Shift1StartMinute.HasValue ? new TimeSpan(src.Shift1StartHour.Value, src.Shift1StartMinute.Value, 0) : (TimeSpan?)null)
                .Map(dest => dest.Shift2, src => src.Shift2StartHour.HasValue && src.Shift2StartMinute.HasValue ? new TimeSpan(src.Shift2StartHour.Value, src.Shift2StartMinute.Value, 0) : (TimeSpan?)null)
                .Map(dest => dest.Shift3, src => src.Shift3StartHour.HasValue && src.Shift3StartMinute.HasValue ? new TimeSpan(src.Shift3StartHour.Value, src.Shift3StartMinute.Value, 0) : (TimeSpan?)null)
                .Map(dest => dest.MachineModelId, src => MachineService.GetMachineModelIdByModelName(src.MachineModel))
                .Map(dest => dest.MachineTypeId, src => MachineService.GetMachineTypeIdByTypeName(src.MachineType))
                .Map(dest => dest.PlantId, src => MesService.GetOrSetPlantIdByPlantName(src.PlantName, src.PlantAddress, src.MachineSerial));
            config.NewConfig<DAL_SQLite.piece, Piece>()
                .Ignore(dest => dest.Id)
                .IgnoreAllVirtual()
                .Map(dest => dest.BarId, src => BarService.GetBarIdByBarIdOldAndMachineId(src.BarId, (int)MapContext.Current.Parameters["machineId"]))
                .Map(dest => dest.ElapsedTime, src => src.TimeSpan)
                .Map(dest => dest.ElapsedTimeProducing, src => src.TimeSpanProducing)
                .Map(dest => dest.IsCompleted, src => src.Completed)
                .Map(dest => dest.IsRedone, src => src.Redone)
                .Map(dest => dest.Day, src => src.EndTime.HasValue && src.TimeSpan.HasValue ? src.EndTime.Value.AddTicks(-src.TimeSpan.Value) : (DateTime?)null)
                .Map(dest => dest.JobId, src => JobService.GetJobIdByJobCode(src.JobCode, (int)MapContext.Current.Parameters["machineId"]))
                .Map(dest => dest.Operator, src => string.IsNullOrEmpty(src.Operator) || string.IsNullOrWhiteSpace(src.Operator) ? "Other" : src.Operator)
                .Map(dest => dest.Shift, src => MachineService.GetShiftByStartTime((int)MapContext.Current.Parameters["machineId"], (src.EndTime.HasValue && src.TimeSpan.HasValue ? src.EndTime.Value.AddTicks(-src.TimeSpan.Value) : (DateTime?)null)))
                .Map(dest => dest.ElapsedTimeCut, src => src.TimeSpanCutting)
                .Map(dest => dest.ElapsedTimeWorking, src => src.TimeSpanWorking)
                .Map(dest => dest.ElapsedTimeTrim, src => src.TimeSpanTrim)
                .Map(dest => dest.ElapsedTimeAnuba, src => src.TimeSpanAnuba)
                .Map(dest => dest.MachineId, src => MapContext.Current.Parameters["machineId"]);
            config.NewConfig<DAL_SQLite.spindle, Spindle>()
                .Ignore(dest => dest.Id)
                .IgnoreAllVirtual()
                .Map(dest => dest.ElapsedTimeWorkTotal, src => src.WorkTotalTime)
                .Map(dest => dest.ElapsedTimeWork3K, src => src.Work3KTime)
                .Map(dest => dest.ElapsedTimeWork6K, src => src.Work6KTime)
                .Map(dest => dest.ElapsedTimeWork9K, src => src.Work9KTime)
                .Map(dest => dest.ElapsedTimeWork12K, src => src.Work12KTime)
                .Map(dest => dest.ElapsedTimeWork15K, src => src.Work15KTime)
                .Map(dest => dest.ElapsedTimeWork18K, src => src.Work18KTime)
                .Map(dest => dest.WorkOverPowerCount, src => src.WorkOverPowerEvents)
                .Map(dest => dest.WorkOverheatingCount, src => src.WorkOverheatingEvents)
                .Map(dest => dest.WorkOverVibratingCount, src => src.WorkOverVibratingEvents)
                .Map(dest => dest.ReplacedDate, src => src.Replaced)
                .Map(dest => dest.MachineId, src => MapContext.Current.Parameters["machineId"]);
            config.NewConfig<DAL_SQLite.state, StateMachine>()
                .Ignore(dest => dest.Id)
                .IgnoreAllVirtual()
                .Map(dest => dest.Day, src => src.EndTime.HasValue ? src.EndTime.Value : (DateTime?)null)
                .Map(dest => dest.ElapsedTime, src => src.TimeSpanDuration)
                .Map(dest => dest.Operator, src => string.IsNullOrEmpty(src.Operator) || string.IsNullOrWhiteSpace(src.Operator) ? "Other" : src.Operator)
                .Map(dest => dest.Shift, src => MachineService.GetShiftByStartTime((int)MapContext.Current.Parameters["machineId"], src.StartTime))
                .Map(dest => dest.StateId, src => src.State1)
                .Map(dest => dest.MachineId, src => MapContext.Current.Parameters["machineId"]);
            config.NewConfig<DAL_SQLite.tool, ToolMachine>()
                .Ignore(dest => dest.Id)
                .IgnoreAllVirtual()
                .Map(dest => dest.BrokenEventsCount, src => src.BrokenEvents)
                .Map(dest => dest.RevisedEventsCount, src => src.RevisedEvents)
                .Map(dest => dest.IsActive, src => true)
                .Map(dest => dest.MachineId, src => MapContext.Current.Parameters["machineId"]);


            config.NewConfig<AggregationMessageModel, HistoryMessageModel>();
        }
    }
}
