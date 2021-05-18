
using FomMonitoringCore.Framework.Common;
using FomMonitoringCore.Framework.Model;
using Mapster;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using FomMonitoringCore.SqlServer;
using FomMonitoringCore.SqlServer.Repository;

namespace FomMonitoringCore.Service
{
    public class MachineService : IMachineService
    {
        private readonly IMachineModelRepository _machineModelRepository;
        private readonly IPanelRepository _panelRepository;
        private readonly IMachineRepository _machineRepository;
        private readonly IFomMonitoringEntities _context;

        public MachineService(
            IMachineModelRepository machineModelRepository,
            IPanelRepository panelRepository,
            IFomMonitoringEntities context,
            IMachineRepository machineRepository)
        {
            _machineRepository = machineRepository;
            _machineModelRepository = machineModelRepository;
            _panelRepository = panelRepository;
            _context = context;
        }
        #region API

        public int? GetMachineModelIdByModelCode(int? modelCode)
        {
            int? result = null;

                
                try
                {                
                    if (modelCode != null)
                    {
                        var machineModel = _machineModelRepository.Get(f => f.ModelCodev997 == modelCode).FirstOrDefault();
                        result = machineModel?.Id;
                    }
                              
                }
                catch (Exception ex)
                {
                    var errMessage = string.Format(ex.GetStringLog());
                    LogService.WriteLog(errMessage, LogService.TypeLevel.Error, ex);                
                }

            
            return result;
        }

        public int? GetMachineTypeIdByModelCode(int? modelCode)
        {
            int? result = null;


            try
            {
                if (modelCode != null)
                {
                    var machineModel = _machineModelRepository.Get(f => f.ModelCodev997 == modelCode).FirstOrDefault();
                    result = machineModel?.MachineTypeId;
                }

            }
            catch (Exception ex)
            {
                var errMessage = string.Format(ex.GetStringLog());
                LogService.WriteLog(errMessage, LogService.TypeLevel.Error, ex);
            }


            return result;
        }

        public DateTime GetLastUpdateByMachineSerial(string machineSerial)
        {
            var result = SqlDateTime.MinValue.Value;

                try
                {
                    var machine = _machineRepository.Get(f => f.Serial == machineSerial).FirstOrDefault();
                if (machine?.LastUpdate != null)
                {
                    result = machine.LastUpdate.Value;
                }
                }
                catch (Exception ex)
                {
                    var errMessage = string.Format(ex.GetStringLog(), machineSerial);
                    LogService.WriteLog(errMessage, LogService.TypeLevel.Error, ex);
                }
            
            return result;
        }

        public int? GetShiftByStartTime(int machineId, DateTime? startTime)
        {
            int? result = null;
            try
            {
                if (startTime != null)
                {
                    var machine = _machineRepository.GetByID(machineId);
                    if (machine != null)
                    {
                        result = machine.Shift1 != null && startTime.Value.TimeOfDay > machine.Shift1.Value ? 1 : (int?)null;
                        result = machine.Shift2 != null && startTime.Value.TimeOfDay > machine.Shift2.Value ? 2 : result;
                        result = machine.Shift3 != null && startTime.Value.TimeOfDay > machine.Shift3.Value ? 3 : result;
                    }
                }
            }
            catch (Exception ex)
            {
                var errMessage = string.Format(ex.GetStringLog(), machineId.ToString(), Convert.ToString(startTime));
                LogService.WriteLog(errMessage, LogService.TypeLevel.Error, ex);
            }
            return result;
        }

        #endregion

        #region APP WEB

        public List<MachineInfoModel> GetUserMachines(ContextModel ctx)
        {
            var result = new List<MachineInfoModel>();

            try
            {
                Guid id = ctx.User.ID;
                if (ctx.AssistanceUserId != null && ctx.User.Role == enRole.Assistance || ctx.User.Role == enRole.RandD)
                {
                    id = new Guid(ctx.AssistanceUserId);
                }

               var query = _context.Set<UserMachineMapping>().Where(w => w.UserId == id).Select(s => s.Machine).ToList();
                result = query.Where(w => w.LastUpdate != null).Adapt<List<MachineInfoModel>>();
                if (ctx.User.Role == enRole.Customer)
                {
                    result.ForEach(mim => mim.TimeZone = _context.Set<UserMachineMapping>().FirstOrDefault(w => w.UserId == id && w.MachineId == mim.Id)?.TimeZone);
                }
            }
            catch (Exception ex)
            {
                var errMessage = string.Format(ex.GetStringLog(), ctx.User.ID.ToString());
                LogService.WriteLog(errMessage, LogService.TypeLevel.Error, ex);
            }

            return result;
        }

        public List<MachineInfoModel> GetPlantMachines(ContextModel ctx)
        {
            try
            {
                if (ctx.AssistanceMachineId != null)
                {
                    var plantid = _context.Set<Machine>().Include("Plant").FirstOrDefault(m => m.Id == ctx.AssistanceMachineId)?.PlantId;
                    var machines = _context.Set<Machine>().Include("Plant").Where(m => m.PlantId == plantid);
                    return machines.Adapt<List<MachineInfoModel>>();
                }
            }
            catch (Exception ex)
            {
                var errMessage = string.Format(ex.GetStringLog(), ctx.User.ID.ToString());
                LogService.WriteLog(errMessage, LogService.TypeLevel.Error, ex);
            }

            return null;
        }

        public List<MachineInfoModel> GetRoleMachines(enRole role)
        {
            var result = new List<MachineInfoModel>();

            try
            {
                var idUsers = _context.Set<Roles_Customer>().Where(e => e.Roles.IdRole == (int)role).Select(e => e.CustomerID)
                    .ToList();

                var query = _context.Set<UserMachineMapping>().Where(w => idUsers.Contains(w.UserId)).Select(s => s.Machine).ToList();
                result = query.Where(w => w.LastUpdate != null).Adapt<List<MachineInfoModel>>();
            }
            catch (Exception ex)
            {
                var errMessage = string.Format(ex.GetStringLog(), role);
                LogService.WriteLog(errMessage, LogService.TypeLevel.Error, ex);
            }

            return result;
        }

        public List<MachineInfoModel> GetAllMachines()
        {
            var result = new List<MachineInfoModel>();

            try
            {
                var query = _machineRepository.Get();
                result = query.Where(w => w.LastUpdate != null).ToList().Adapt<List<MachineInfoModel>>();
                
            }
            catch (Exception ex)
            {
                var errMessage = string.Format(ex.GetStringLog());
                LogService.WriteLog(errMessage, LogService.TypeLevel.Error, ex);
            }

            return result;
        }

        public MachineInfoModel GetMachineInfo(int machineId)
        {
            var result = new MachineInfoModel();

            try
            {
                var query = _machineRepository.GetByID(machineId);
                result = query.Adapt<MachineInfoModel>();
                
            }
            catch (Exception ex)
            {
                var errMessage = string.Format(ex.GetStringLog(), machineId.ToString());
                LogService.WriteLog(errMessage, LogService.TypeLevel.Error, ex);
            }

            return result;
        }

        public List<int> GetMachinePanels(ContextModel context)
        {
            return GetMachinePanels(context.ActualMachine.MachineModelId);
        }

        public List<int> GetMachinePanels(int? machineModelId)
        {
            var result = new List<int>();
            try
            {
                result = _panelRepository.Get(p => p.ParameterMachine.Any(f => f.MachineModelId == machineModelId)).Select(a => a.Id).Distinct().ToList();
            }
            catch (Exception ex)
            {
                var errMessage = string.Format(ex.GetStringLog(), machineModelId.ToString());
                LogService.WriteLog(errMessage, LogService.TypeLevel.Error, ex);
            }
            return result;
        }

        public CurrentStateModel GetCurrentStateModel(int machineId)
        {
            var result = new CurrentStateModel();

            try
            {
                var query = _context.Set<CurrentState>().FirstOrDefault(w => w.MachineId == machineId);
                result = query?.Adapt<CurrentStateModel>();
            }
            catch (Exception ex)
            {
                var errMessage = string.Format(ex.GetStringLog(), machineId.ToString());
                LogService.WriteLog(errMessage, LogService.TypeLevel.Error, ex);
            }

            return result;
        }

        public List<ParameterResetValueDataModel> GetMachineCountersReset(int MachineId, int? IdLanguage, string search = null)
        {
            var result = new List<ParameterResetValueDataModel>();
            try
            {
                var machine = _context.Set<Machine>().Find(MachineId);
                //lista grouppi per la macchina
                var groups = _context.Set<ParameterMachine>().Where(m => m.MachineModelId == machine.MachineModelId).Select(m => m.VarComponent).Distinct().ToList();
                var machineGroups = _context.Set<MessagesIndex>().Include("MessageTranslation")
                    .Where(mi => groups.Contains(mi.MessageCode)).ToList();

                //anagrafica variabili per la macchina
                var keywords = _context.Set<ParameterMachine>().Where(m => m.MachineModelId == machine.MachineModelId).Select(m => m.Keyword).Distinct().ToList();
                var varNames = _context.Set<MessagesIndex>().Include("MessageTranslation")
                    .Where(mi => keywords.Contains(mi.MessageCode));
                if (search != null)
                {
                    varNames = varNames.Where(v => v.MessageTranslation.Any(t =>
                            t.MessageLanguageId == IdLanguage && t.Translation.Contains(search)));
                }

                var varNames2 = varNames.ToList();


                var all = _context.Set<ParameterResetValue>()
                    .Include("ParameterMachine")
                    .Where(p => p.MachineId == MachineId).OrderByDescending(c => c.ResetDate).ToList();

                foreach (var value in all)
                {
                    var translation = varNames2.FirstOrDefault(v => v.MessageCode.Trim() == value.ParameterMachine.Keyword.Trim())?.MessageTranslation;
                    if (translation == null) continue;
                    /*var VarName = translation?.FirstOrDefault(t => t.MessageLanguageId == IdLanguage)?.Translation;
                    if (search != null)
                    {
                        if (!VarName.ToLower().Contains(search.ToLower())) continue;
                    }*/

                    var translationGroup = machineGroups.FirstOrDefault(v => v.MessageCode.Trim() == value.ParameterMachine.VarComponent.Trim())?.MessageTranslation;

                    result.Add(new ParameterResetValueDataModel()
                    {
                        MachineId = value.MachineId,
                        ResetDate = value.ResetDate,
                        ResetValue = value.ResetValue,
                        ValueBeforeReset = value.ValueBeforeReset,
                        MachineGroupName = translationGroup?.FirstOrDefault(t => t.MessageLanguageId == IdLanguage)?.Translation,
                        VariableName = translation?.FirstOrDefault(t => t.MessageLanguageId == IdLanguage)?.Translation
                });
                }

                // if(search != null)
                //    query.Where(p => p.ParameterMachine.Keyword.Contains("search"));
                
            }
            catch (Exception ex)
            {
                var errMessage = string.Format(ex.GetStringLog(), MachineId.ToString());
                LogService.WriteLog(errMessage, LogService.TypeLevel.Error, ex);
            }
            
            return result;
        }

        #endregion
    }
}
