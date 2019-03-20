﻿using FomMonitoringCore.DAL;
using FomMonitoringCore.Framework.Common;
using FomMonitoringCore.Framework.Model;
using Mapster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;

namespace FomMonitoringCore.Service
{
    public class MesService
    {
        #region API

        public static int? GetPlantIdByPlantName(string plantName, string plantAddress)
        {
            int? result = null;
            try
            {
                using (TransactionScope transaction = new TransactionScope(TransactionScopeOption.Required))
                {
                    using (FST_FomMonitoringEntities ent = new FST_FomMonitoringEntities())
                    {
                        if (!string.IsNullOrEmpty(plantName))
                        {
                            Plant plant = ent.Plant.FirstOrDefault(f => f.Name == plantName && f.Address == plantAddress);
                            if (plant == null && AddPlant(plantName, plantAddress))
                            {
                                plant = ent.Plant.FirstOrDefault(f => f.Name == plantName && f.Address == plantAddress);
                            }
                            result = plant != null ? plant.Id : (int?)null;
                        }
                        else
                        {
                            result = int.Parse(ApplicationSettingService.GetWebConfigKey("DefaultPlantID"));
                        }
                        transaction.Complete();
                    }
                }
            }
            catch (Exception ex)
            {
                string errMessage = string.Format(ex.GetStringLog(), plantName, plantAddress);
                LogService.WriteLog(errMessage, LogService.TypeLevel.Error, ex);
            }
            return result;
        }

        public static bool AddPlant(string plantName, string plantAddress)
        {
            bool result = false;
            try
            {
                using (TransactionScope transaction = new TransactionScope(TransactionScopeOption.Required))
                {
                    using (FST_FomMonitoringEntities ent = new FST_FomMonitoringEntities())
                    {
                        Plant plant = new Plant();
                        plant.Name = plantName;
                        plant.Address = plantAddress;
                        ent.Plant.Add(plant);
                        ent.SaveChanges();
                        transaction.Complete();
                        result = true;
                    }
                }
            }
            catch (Exception ex)
            {
                string errMessage = string.Format(ex.GetStringLog(), plantName, plantAddress);
                LogService.WriteLog(errMessage, LogService.TypeLevel.Error, ex);
            }
            return result;
        }

        #endregion

        #region APP WEB

        public static List<PlantModel> GetUserPlants(Guid UserID)
        {
            List<PlantModel> result = new List<PlantModel>();

            try
            {
                using (FST_FomMonitoringEntities ent = new FST_FomMonitoringEntities())
                {
                    var query = ent.UserMachineMapping.Where(w => w.UserId == UserID).Select(s => s.Machine.Plant).Distinct().ToList();
                    result = query.Adapt<List<PlantModel>>();
                }
            }
            catch (Exception ex)
            {
                string errMessage = string.Format(ex.GetStringLog(), UserID.ToString());
                LogService.WriteLog(errMessage, LogService.TypeLevel.Error, ex);
            }

            return result;
        }


        public static List<MesUserMachinesModel> GetPlantData(PlantModel plant)
        {
            List<MesUserMachinesModel> result = new List<MesUserMachinesModel>();

            try
            {
                using (FST_FomMonitoringEntities ent = new FST_FomMonitoringEntities())
                {
                    List<usp_MesUserMachines_Result> query = ent.usp_MesUserMachines(plant.Id, DateTime.UtcNow.Date).ToList();
                    result = query.Adapt<List<MesUserMachinesModel>>();
                }
            }
            catch (Exception ex)
            {
                string errMessage = string.Format("{0} (PlantID = '{1}')", ex.GetStringLog(), plant.Id);
                LogService.WriteLog(errMessage, LogService.TypeLevel.Error, ex);
            }

            return result;
        }

        public static List<PlantModel> GetAllPlantsMachines()
        {
            List<PlantModel> result = new List<PlantModel>();

            try
            {
                using (FST_FomMonitoringEntities ent = new FST_FomMonitoringEntities())
                {
                    var query = ent.Machine.Select(s => s.Plant).Distinct().ToList();
                    result = query.Adapt<List<PlantModel>>();
                }
            }
            catch (Exception ex)
            {
                string errMessage = string.Format(ex.GetStringLog());
                LogService.WriteLog(errMessage, LogService.TypeLevel.Error, ex);
            }

            return result;
        }

        #endregion

    }
}
