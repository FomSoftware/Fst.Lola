using Autofac;
using Autofac.Core;
using FomMonitoringCore.DAL;
using FomMonitoringCore.Framework.Common;
using FomMonitoringCore.Framework.Model;
using FomMonitoringCore.Framework.Model.Xml;
using FomMonitoringCore.Repository;
using FomMonitoringCore.Uow;
using Mapster;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Transactions;

namespace FomMonitoringCore.Service
{
    public class MachineService : IMachineService
    {
        private IMachineTypeRepository _machineTypeRepository;
        private IMachineModelRepository _machineModelRepository;
        private IPanelRepository _panelRepository;
        private IMachineRepository _machineRepository;
        private IFomMonitoringEntities _context;
        private IParameterMachineService _parameterMachineService;
        private IUnitOfWork _unitOfWork;

        public MachineService(
            IMachineTypeRepository machineTypeRepository,
            IMachineModelRepository machineModelRepository,
            IPanelRepository panelRepository,
            IFomMonitoringEntities context,
            IMachineRepository machineRepository,
            IParameterMachineService parameterMachineService,
            IUnitOfWork unitOfWork)
        {
            _machineRepository = machineRepository;
            _machineTypeRepository = machineTypeRepository;
            _machineModelRepository = machineModelRepository;
            _panelRepository = panelRepository;
            _context = context;
            _unitOfWork = unitOfWork;
            _parameterMachineService = parameterMachineService;
        }
        #region API

        public int? GetMachineModelIdByModelCode(int? modelCode)
        {
            int? result = null;

                
                try
                {                
                    if (modelCode != null)
                    {
                        MachineModel machineModel = _machineModelRepository.Get(f => f.ModelCodev997 == modelCode).FirstOrDefault();
                        result = machineModel != null ? machineModel.Id : (int?)null;
                    }
                              
                }
                catch (Exception ex)
                {
                    string errMessage = string.Format(ex.GetStringLog());
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
                    MachineModel machineModel = _machineModelRepository.Get(f => f.ModelCodev997 == modelCode).FirstOrDefault();
                    result = machineModel != null ? machineModel.MachineTypeId : (int?)null;
                }

            }
            catch (Exception ex)
            {
                string errMessage = string.Format(ex.GetStringLog());
                LogService.WriteLog(errMessage, LogService.TypeLevel.Error, ex);
            }


            return result;
        }

        public int? GetMachineTypeIdByTypeName(string typeName)
        {
            int? result = null;

            try
            {

                if (!string.IsNullOrEmpty(typeName))
                {
                    MachineType machineType = _machineTypeRepository.Get(f => f.Name == typeName).FirstOrDefault();
                    result = machineType != null ? machineType.Id : (int?)null;
                }
            }
            catch (Exception ex)
            {
                string errMessage = string.Format(ex.GetStringLog(), typeName.ToString());
                LogService.WriteLog(errMessage, LogService.TypeLevel.Error, ex);
            }
            
            return result;
        }

        public DateTime GetLastUpdateByMachineSerial(string machineSerial)
        {
            DateTime result = SqlDateTime.MinValue.Value;

                try
                {
                    Machine machine = _machineRepository.Get(f => f.Serial == machineSerial).FirstOrDefault();
                if (machine != null && machine.LastUpdate != null)
                    {
                        result = machine.LastUpdate.Value;
                    }
                }
                catch (Exception ex)
                {
                    string errMessage = string.Format(ex.GetStringLog(), machineSerial);
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
                    Machine machine = _machineRepository.GetByID(machineId);
                    if (machine != null)
                    {
                        result = machine.Shift1 != null && startTime.Value.TimeOfDay > machine.Shift1.Value ? 1 : result;
                        result = machine.Shift2 != null && startTime.Value.TimeOfDay > machine.Shift2.Value ? 2 : result;
                        result = machine.Shift3 != null && startTime.Value.TimeOfDay > machine.Shift3.Value ? 3 : result;
                    }
                }
            }
            catch (Exception ex)
            {
                string errMessage = string.Format(ex.GetStringLog(), machineId.ToString(), Convert.ToString(startTime));
                LogService.WriteLog(errMessage, LogService.TypeLevel.Error, ex);
            }
            return result;
        }

        public bool AddMachineModel(string modelName, int modelCode)
        {
            bool result = false;

                try
                {                    
                    MachineModel machineModel = new MachineModel();
                    machineModel.Name = modelName;
                    machineModel.ModelCodev997 = modelCode;
                    _machineModelRepository.Insert(machineModel);
                    _context.SaveChanges();                   
                    result = true;
                }
            
                catch (Exception ex)
                {
                    string errMessage = string.Format(ex.GetStringLog(), modelName.ToString());
                    LogService.WriteLog(errMessage, LogService.TypeLevel.Error, ex);
                }
            
            return result;
        }

        #endregion

        #region APP WEB

        public List<MachineInfoModel> GetAllMachinesByPlantID(int PlantID)
        {
            List<MachineInfoModel> result = new List<MachineInfoModel>();

            try
            {
                var query = _machineRepository.Get(w => w.PlantId == PlantID && w.LastUpdate != null).ToList();
                result = query.Adapt<List<MachineInfoModel>>();
            }
            catch (Exception ex)
            {
                string errMessage = string.Format(ex.GetStringLog(), PlantID.ToString());
                LogService.WriteLog(errMessage, LogService.TypeLevel.Error, ex);
            }

            return result;
        }

        public List<MachineInfoModel> GetUserMachines(Guid UserID)
        {
            List<MachineInfoModel> result = new List<MachineInfoModel>();

            try
            {                
                    var query = _context.Set<UserMachineMapping>().Where(w => w.UserId == UserID).Select(s => s.Machine).ToList();
                    result = query.Where(w => w.LastUpdate != null).Adapt<List<MachineInfoModel>>();           
            }
            catch (Exception ex)
            {
                string errMessage = string.Format(ex.GetStringLog(), UserID.ToString());
                LogService.WriteLog(errMessage, LogService.TypeLevel.Error, ex);
            }

            return result;
        }

        public List<MachineInfoModel> GetAllMachines()
        {
            List<MachineInfoModel> result = new List<MachineInfoModel>();

            try
            {
                var query = _machineRepository.Get();
                result = query.Where(w => w.LastUpdate != null).ToList().Adapt<List<MachineInfoModel>>();
                
            }
            catch (Exception ex)
            {
                string errMessage = string.Format(ex.GetStringLog());
                LogService.WriteLog(errMessage, LogService.TypeLevel.Error, ex);
            }

            return result;
        }

        public MachineInfoModel GetMachineInfo(int MachineID)
        {
            MachineInfoModel result = new MachineInfoModel();

            try
            {
                var query = _machineRepository.GetByID(MachineID);
                result = query.Adapt<MachineInfoModel>();
                
            }
            catch (Exception ex)
            {
                string errMessage = string.Format(ex.GetStringLog(), MachineID.ToString());
                LogService.WriteLog(errMessage, LogService.TypeLevel.Error, ex);
            }

            return result;
        }

        public List<int> GetMachinePanels(ContextModel context)
        {
            return GetMachinePanels(context.ActualMachine.MachineModelId);
        }

        public List<int> GetMachinePanels(int? MachineModelId)
        {
            List<int> result = new List<int>();
            try
            {
                result = _panelRepository.Get(p => p.ParameterMachine.Any(f => f.MachineModelId == MachineModelId)).Select(a => a.Id).Distinct().ToList();
            }
            catch (Exception ex)
            {
                string errMessage = string.Format(ex.GetStringLog(), MachineModelId.ToString());
                LogService.WriteLog(errMessage, LogService.TypeLevel.Error, ex);
            }
            return result;
        }

        public ParameterMachineValueModel GetProductionValueModel(MachineInfoModel context, enPanel pp)
        {
            var par = _parameterMachineService.GetParameters(context, (int)pp).FirstOrDefault();
            return par;
        }



        #endregion
    }
}
