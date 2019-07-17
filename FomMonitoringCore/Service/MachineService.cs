using FomMonitoringCore.DAL;
using FomMonitoringCore.Framework.Common;
using FomMonitoringCore.Framework.Model;
using Mapster;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Transactions;

namespace FomMonitoringCore.Service
{
    public class MachineService
    {
        #region API

        public static int? GetMachineModelIdByModelCodeOrName(int? modelCode, string modelName)
        {
            int? result = null;
            try
            {
                using (TransactionScope transaction = new TransactionScope(TransactionScopeOption.Required))
                {
                    using (FST_FomMonitoringEntities ent = new FST_FomMonitoringEntities())
                    {
                        if (modelCode != null)
                        {
                            MachineModel machineModel = ent.MachineModel.FirstOrDefault(f => f.ModelCodev997 == modelCode);
                            if (machineModel == null && AddMachineModel(modelName, (int)modelCode))
                            {
                                machineModel = ent.MachineModel.FirstOrDefault(f => f.Name == modelName);
                            }
                            result = machineModel != null ? machineModel.Id : (int?)null;
                        }
                        else if (!string.IsNullOrEmpty(modelName))
                        {
                            MachineModel machineModel = ent.MachineModel.FirstOrDefault(f => f.Name == modelName);                            
                            result = machineModel != null ? machineModel.Id : (int?)null;
                        }
                        transaction.Complete();
                    }
                }
            }
            catch (Exception ex)
            {
                string errMessage = string.Format(ex.GetStringLog(), modelName.ToString());
                LogService.WriteLog(errMessage, LogService.TypeLevel.Error, ex);
            }
            return result;
        }

        public static int? GetMachineTypeIdByTypeName(string typeName)
        {
            int? result = null;
            try
            {
                using (TransactionScope transaction = new TransactionScope(TransactionScopeOption.Required))
                {
                    using (FST_FomMonitoringEntities ent = new FST_FomMonitoringEntities())
                    {
                        if (!string.IsNullOrEmpty(typeName))
                        {
                            MachineType machineType = ent.MachineType.FirstOrDefault(f => f.Name == typeName);
                            result = machineType != null ? machineType.Id : (int?)null;
                        }
                        transaction.Complete();
                    }
                }
            }
            catch (Exception ex)
            {
                string errMessage = string.Format(ex.GetStringLog(), typeName.ToString());
                LogService.WriteLog(errMessage, LogService.TypeLevel.Error, ex);
            }
            return result;
        }

        public static DateTime GetLastUpdateByMachineSerial(string machineSerial)
        {
            DateTime result = SqlDateTime.MinValue.Value;
            try
            {
                using (FST_FomMonitoringEntities ent = new FST_FomMonitoringEntities())
                {
                    Machine machine = ent.Machine.FirstOrDefault(f => f.Serial == machineSerial);
                    if (machine != null && machine.LastUpdate != null)
                    {
                        result = machine.LastUpdate.Value;
                    }
                }
            }
            catch (Exception ex)
            {
                string errMessage = string.Format(ex.GetStringLog(), machineSerial);
                LogService.WriteLog(errMessage, LogService.TypeLevel.Error, ex);
            }
            return result;
        }

        public static int? GetShiftByStartTime(int machineId, DateTime? startTime)
        {
            int? result = null;
            try
            {
                using (TransactionScope transaction = new TransactionScope(TransactionScopeOption.Required))
                {
                    using (FST_FomMonitoringEntities ent = new FST_FomMonitoringEntities())
                    {
                        if (startTime != null)
                        {
                            Machine machine = ent.Machine.FirstOrDefault(f => f.Id == machineId);
                            if (machine != null)
                            {
                                result = machine.Shift1 != null && startTime.Value.TimeOfDay > machine.Shift1.Value ? 1 : result;
                                result = machine.Shift2 != null && startTime.Value.TimeOfDay > machine.Shift2.Value ? 2 : result;
                                result = machine.Shift3 != null && startTime.Value.TimeOfDay > machine.Shift3.Value ? 3 : result;
                            }
                        }
                        transaction.Complete();
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

        public static bool AddMachineModel(string modelName, int modelCode)
        {
            bool result = false;
            try
            {
                using (TransactionScope transaction = new TransactionScope(TransactionScopeOption.Required))
                {
                    using (FST_FomMonitoringEntities ent = new FST_FomMonitoringEntities())
                    {
                        MachineModel machineModel = new MachineModel();
                        machineModel.Name = modelName;
                        machineModel.ModelCodev997 = modelCode;
                        ent.MachineModel.Add(machineModel);
                        ent.SaveChanges();
                        transaction.Complete();
                        result = true;
                    }
                }
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

        public static List<MachineInfoModel> GetAllMachinesByPlantID(int PlantID)
        {
            List<Framework.Model.MachineInfoModel> result = new List<Framework.Model.MachineInfoModel>();

            try
            {
                using (FST_FomMonitoringEntities ent = new FST_FomMonitoringEntities())
                {
                    var query = ent.Machine.Where(w => w.PlantId == PlantID && w.LastUpdate != null).ToList();
                    result = query.Adapt<List<MachineInfoModel>>();
                }
            }
            catch (Exception ex)
            {
                string errMessage = string.Format(ex.GetStringLog(), PlantID.ToString());
                LogService.WriteLog(errMessage, LogService.TypeLevel.Error, ex);
            }

            return result;
        }

        public static List<MachineInfoModel> GetUserMachines(Guid UserID)
        {
            List<Framework.Model.MachineInfoModel> result = new List<Framework.Model.MachineInfoModel>();

            try
            {
                using (FST_FomMonitoringEntities ent = new FST_FomMonitoringEntities())
                {
                    var query = ent.UserMachineMapping.Where(w => w.UserId == UserID).Select(s => s.Machine).ToList();
                    result = query.Where(w => w.LastUpdate != null).Adapt<List<MachineInfoModel>>();
                }
            }
            catch (Exception ex)
            {
                string errMessage = string.Format(ex.GetStringLog(), UserID.ToString());
                LogService.WriteLog(errMessage, LogService.TypeLevel.Error, ex);
            }

            return result;
        }

        public static List<MachineInfoModel> GetAllMachines()
        {
            List<Framework.Model.MachineInfoModel> result = new List<Framework.Model.MachineInfoModel>();

            try
            {
                using (FST_FomMonitoringEntities ent = new FST_FomMonitoringEntities())
                {
                    var query = ent.Machine.ToList();
                    result = query.Where(w => w.LastUpdate != null).Adapt<List<MachineInfoModel>>();
                }
            }
            catch (Exception ex)
            {
                string errMessage = string.Format(ex.GetStringLog());
                LogService.WriteLog(errMessage, LogService.TypeLevel.Error, ex);
            }

            return result;
        }

        public static MachineInfoModel GetMachineInfo(int MachineID)
        {
            MachineInfoModel result = new MachineInfoModel();

            try
            {
                using (FST_FomMonitoringEntities ent = new FST_FomMonitoringEntities())
                {
                    var query = ent.Machine.Where(w => w.Id == MachineID).FirstOrDefault();
                    result = query.Adapt<MachineInfoModel>();
                }
            }
            catch (Exception ex)
            {
                string errMessage = string.Format(ex.GetStringLog(), MachineID.ToString());
                LogService.WriteLog(errMessage, LogService.TypeLevel.Error, ex);
            }

            return result;
        }

        public static List<int> GetMachinePanels(ContextModel context)
        {
            List<int> result = new List<int>();
            try
            {
                using (FST_FomMonitoringEntities ent = new FST_FomMonitoringEntities())
                {
                    result = ent.Panel.Where(p => p.MachineModel.Any(f => f.Id  == context.ActualMachine.MachineModelId)).Select(a => a.Id).ToList();                   
                }
            }
            catch (Exception ex)
            {
                string errMessage = string.Format(ex.GetStringLog(), context.ActualMachine.MachineModelId.ToString());
                LogService.WriteLog(errMessage, LogService.TypeLevel.Error, ex);
            }
            return result;
        }


            #endregion
        }
}
