using FomMonitoringCore.DAL;
using FomMonitoringCore.Framework.Common;
using FomMonitoringCore.Framework.Model;
using FomMonitoringCore.Service;
using FomMonitoringResources;
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
            config.NewConfig<MessageMachine, MessageMachineModel>()
                .Map(dest => dest.Code, src => src.MessagesIndex.MessageCode)
                .Map(dest => dest.Group, src => src.MessagesIndex.MachineGroupId)
                .Map(dest => dest.GroupName, src => src.MessagesIndex.MachineGroup.MachineGroupName)
                .Map(dest => dest.Type, src => src.MessagesIndex.MessageTypeId)
                .Map(dest => dest.IsPeriodicMsg, src => src.MessagesIndex.IsPeriodicM)
                .Map(dest => dest.Description, src => src.GetDescription((int)MapContext.Current.Parameters["idLanguage"]));

            config.NewConfig<Bar, BarModel>();
            config.NewConfig<HistoryMessage, HistoryMessageModel>()
                .Map(dest => dest.Code, src => src.MessagesIndex != null ? src.MessagesIndex.MessageCode : null)
                .Map(dest => dest.Type, src => src.MessagesIndex != null ? (int?)src.MessagesIndex.MessageTypeId : null);
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
            config.NewConfig<Plant, PlantModel>()
                .Map(dest => dest.Machines, src => src.Machine);
            config.NewConfig<Spindle, SpindleModel>()
                //.Ignore(dest => dest.ChangeCount)
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


            config.NewConfig<ParameterMachineValue, ParameterMachineValueModel>()
                .Map(d => d.UtcDateTime, src => src.UtcDateTime)
                .Map(d => d.Description, src => new System.Resources.ResourceManager(typeof(Resource)).GetString(src.ParameterMachine.Keyword))
                .Map(d => d.Value, src => src.VarValue)
                .Map(d => d.VarNumber, src => src.VarNumber)
                .Map(d => d.CnUm, src => src.ParameterMachine.CnUm)
                .Map(d => d.HmiUm, src => src.ParameterMachine.HmiUm)
                .Map(d => d.Cluster, src => src.ParameterMachine.Cluster)
                .Map(d => d.Keyword, src => src.ParameterMachine.Keyword);

            config.NewConfig<ParameterMachine, Model.Xml.ParameterMachineModelXml>()
                .Map(d => d.CLUSTER, src => src.Cluster)
                .Map(d => d.CN_TYPE, src => src.CnType)
                .Map(d => d.CN_UM, src => src.CnUm)
                .Map(d => d.DEFAULT_VALUE, src => src.DefaultValue)
                .Map(d => d.HMI_LABEL, src => src.HmiLabel)
                .Map(d => d.HMI_SECTION, src => src.HmiSection)
                .Map(d => d.HMI_UM, src => src.HmiUm)
                .Map(d => d.KEYWORD, src => src.Keyword)
                .Map(d => d.LOLA_LABEL, src => src.LolaLabel)
                .Map(d => d.MACHINE_GROUP, src => src.MachineGroup)
                .Map(d => d.PANEL_ID, src => src.PanelId)
                .Map(d => d.R_LEVEL, src => src.RLevel)
                .Map(d => d.THRESHOLD_LABEL, src => src.ThresholdLabel)
                .Map(d => d.THRESHOLD_MAX, src => src.ThresholdMax)
                .Map(d => d.THRESHOLD_MIN, src => src.ThresholdMin)
                .Map(d => d.VAR_NUMBER, src => src.VarNumber)
                .Map(d => d.W_LEVEL, src => src.WLevel)
                .Map(d => d.HISTORICIZED, src => src.Historicized);


            // SP to Model
            config.NewConfig<usp_AggregationState_Result, HistoryStateModel>()
                .Map(dest => dest.enState, src => (enState?)src.StateId ?? null);
            config.NewConfig<MachineMesDataModel, MesUserMachinesModel>()
                .Map(dest => dest.enActualState, src => (enState?)src.ActualStateId ?? null)
                .Map(dest => dest.Expired, src => src.ExpirationDate < DateTime.UtcNow);



            // Model to DAL
            config.NewConfig<JsonDataModel, JsonData>();
            config.NewConfig<UserModel, Users>();

            // SQLite to SQLServer 
            config.NewConfig<DAL_SQLite.bar, Bar>()
                .Ignore(dest => dest.Id)
                .IgnoreAllVirtual()
                .Map(dest => dest.IdOld, src => src.Id)
                .Map(dest => dest.MachineId, src => MapContext.Current.Parameters["machineId"]);

            config.NewConfig<DAL_SQLite.message, MessageMachine>()
                .Ignore(dest => dest.Id)
                .IgnoreAllVirtual()
                .Map(dest => dest.Day, src => src.Time.HasValue ? src.Time.Value : (DateTime?)null)
                .Map(dest => dest.StartTime, src => src.Time)
                //.Map(dest => dest.ElapsedTime, src => src.TimeSpanDuration)
                .Map(dest => dest.Day, src => src.Code)
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
                .Map(dest => dest.MachineModelId, src => ((IMachineService)MapContext.Current.Parameters["machineService"]).GetMachineModelIdByModelCode(src.MachineCode))
                //.Map(dest => dest.MachineTypeId, src => ((IMachineService)MapContext.Current.Parameters["machineService"]).GetMachineTypeIdByTypeName(src.MachineType));
                .Map(dest => dest.MachineTypeId, src => ((IMachineService)MapContext.Current.Parameters["machineService"]).GetMachineTypeIdByModelCode(src.MachineCode));
            config.NewConfig<DAL_SQLite.piece, Piece>()
                .Ignore(dest => dest.Id)
                .IgnoreAllVirtual()
                .Map(dest => dest.BarId, src => ((IBarService)MapContext.Current.Parameters["barService"]).GetBarIdByBarIdOldAndMachineId(src.BarId, (int)MapContext.Current.Parameters["machineId"]))
                .Map(dest => dest.ElapsedTime, src => src.TimeSpan)
                .Map(dest => dest.ElapsedTimeProducing, src => src.TimeSpanProducing)
                .Map(dest => dest.IsRedone, src => src.Redone)
                .Map(dest => dest.Day, src => src.EndTime.HasValue && src.TimeSpan.HasValue ? src.EndTime.Value.AddTicks(-src.TimeSpan.Value).ToNullIfTooEarlyForDb() : (DateTime?)null)
                .Map(dest => dest.JobId, src => ((IJobService)MapContext.Current.Parameters["jobService"]).GetJobIdByJobCode(src.JobCode, (int)MapContext.Current.Parameters["machineId"]))
                .Map(dest => dest.Operator, src => string.IsNullOrEmpty(src.Operator) || string.IsNullOrWhiteSpace(src.Operator) ? "Other" : src.Operator)
                .Map(dest => dest.Shift, src => ((IMachineService)MapContext.Current.Parameters["machineService"]).GetShiftByStartTime((int)MapContext.Current.Parameters["machineId"], (src.EndTime.HasValue && src.TimeSpan.HasValue ? src.EndTime.Value.AddTicks(-src.TimeSpan.Value) : (DateTime?)null)))
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
                .Map(dest => dest.Shift, src => ((IMachineService)MapContext.Current.Parameters["machineService"]).GetShiftByStartTime((int)MapContext.Current.Parameters["machineId"], src.StartTime))
                .Map(dest => dest.StateId, src => src.State)
                .Map(dest => dest.MachineId, src => MapContext.Current.Parameters["machineId"]);
            config.NewConfig<DAL_SQLite.tool, ToolMachine>()
                .Ignore(dest => dest.Id)
                .IgnoreAllVirtual()
                .Map(dest => dest.IsActive, src => true)
                .Map(dest => dest.MachineId, src => MapContext.Current.Parameters["machineId"]);


            config.NewConfig<AggregationMessageModel, HistoryMessageModel>();

            config.NewConfig<Model.Xml.ParameterMachineModelXml, ParameterMachine>()
                .Ignore(dest => dest.Id, dest => dest.MachineModelId)
                .IgnoreAllVirtual()
                .IgnoreNonMapped(true)
                .Map(d => d.Cluster, src => !string.IsNullOrWhiteSpace(src.CLUSTER) ? src.CLUSTER.Trim() : null)
                .Map(d => d.CnType, src => !string.IsNullOrWhiteSpace(src.CN_TYPE) ? src.CN_TYPE.Trim() : null)
                .Map(d => d.CnUm, src => !string.IsNullOrWhiteSpace(src.CN_UM) ? src.CN_UM.Trim() : null)
                .Map(d => d.DefaultValue, src => !string.IsNullOrWhiteSpace(src.DEFAULT_VALUE) ? src.DEFAULT_VALUE.Trim() : null)
                .Map(d => d.HmiLabel, src => !string.IsNullOrWhiteSpace(src.HMI_LABEL) ? src.HMI_LABEL.Trim() : null)
                .Map(d => d.HmiSection, src => !string.IsNullOrWhiteSpace(src.HMI_SECTION) ? src.HMI_SECTION.Trim() : null)
                .Map(d => d.HmiUm, src => !string.IsNullOrWhiteSpace(src.HMI_UM) ? src.HMI_UM.Trim() : null)
                .Map(d => d.Keyword, src => !string.IsNullOrWhiteSpace(src.KEYWORD) ? src.KEYWORD.Trim() : null)
                .Map(d => d.LolaLabel, src => !string.IsNullOrWhiteSpace(src.LOLA_LABEL) ? src.LOLA_LABEL.Trim() : null)
                .Map(d => d.MachineGroup, src => !string.IsNullOrWhiteSpace(src.MACHINE_GROUP) ? src.MACHINE_GROUP.Trim() : null)
                .Map(d => d.PanelId, src => src.PANEL_ID)
                .Map(d => d.RLevel, src => !string.IsNullOrWhiteSpace(src.R_LEVEL) ? src.R_LEVEL.Trim() : null)
                .Map(d => d.ThresholdLabel, src => !string.IsNullOrWhiteSpace(src.THRESHOLD_LABEL) ? src.THRESHOLD_LABEL.Trim() : null)
                .Map(d => d.ThresholdMax, src => !string.IsNullOrWhiteSpace(src.THRESHOLD_MAX) ? src.THRESHOLD_MAX.Trim() : null)
                .Map(d => d.ThresholdMin, src => !string.IsNullOrWhiteSpace(src.THRESHOLD_MIN) ? src.THRESHOLD_MIN.Trim() : null)
                .Map(d => d.VarNumber, src => !string.IsNullOrWhiteSpace(src.VAR_NUMBER) ? src.VAR_NUMBER.Trim() : null)
                .Map(d => d.WLevel, src => !string.IsNullOrWhiteSpace(src.W_LEVEL) ? src.W_LEVEL.Trim() : null)
                .Map(d => d.Historicized, src => !string.IsNullOrWhiteSpace(src.HISTORICIZED) ? src.HISTORICIZED.Trim() : null)
                .Map(d => d.ModelCode, src => MapContext.Current.Parameters["modelCode"]);
        }
    }
}
