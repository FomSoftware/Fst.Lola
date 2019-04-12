using FomMonitoringCore.DAL;
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

        public static int? GetOrSetPlantIdByPlantName(string plantName, string plantAddress, string machineSerial)
        {
            int? result = null;
            try
            {
                using (FST_FomMonitoringEntities ent = new FST_FomMonitoringEntities())
                {
                    Guid? userId;
                    Plant plant = null;
                    IEnumerable<Guid> userIds;
                    Machine machine = ent.Machine.FirstOrDefault(m => m.Serial == machineSerial);
                    userIds = ent.Machine.Where(m => m.Serial == machineSerial).SelectMany(n => n.UserMachineMapping).Select(um => um.UserId).ToList();

                    if(machine.Plant != null)
                    {
                        return machine.Plant.Id;
                    }

                    //devo isolare questo contesto per non abilitare il sqlServer alla transazioni distribuite essendo su un db diverso - mbelletti
                    using (TransactionScope transaction = new TransactionScope(TransactionScopeOption.RequiresNew))
                    {
                        using (UserManager.DAL.UserManagerEntities uent = new UserManager.DAL.UserManagerEntities())
                        {
                            //l'utente a cui appartiene il plant tra tutti quelli 
                            //a cui la macchina è associata sarà sempre il customer
                            //e ve ne sarà sempre e solo uno
                            userId = uent.Users.FirstOrDefault(user => userIds.Any(us => us == user.ID) && user.Roles_Users.Any(ur => ur.Roles.IdRole == (int)UserManager.Framework.Common.Enumerators.UserRole.Customer))?.ID;
                        }
                    }


                    //se c'è il pant con il nome inviato dalla macchina lo associo
                    if (!string.IsNullOrEmpty(plantName))
                    {
                        plant = ent.Plant.FirstOrDefault(f => f.Name == plantName && f.Address == plantAddress);
                    }

                    //se non c'è cerco il primo plant associato all'utente della macchina,
                    //N.B. quando non ci sarà più solo il plant di default modificare
                    if (plant == null)
                    {
                        plant = ent.Plant.FirstOrDefault(f => f.UserId == userId);
                    }

                    //se non c'è creo il plant di default per l'utente
                    if (plant == null)
                    {
                        plant = AddPlant(plantName, plantAddress, userId);
                    }

                    result = plant.Id;
                }

            }
            catch (Exception ex)
            {
                string errMessage = string.Format(ex.GetStringLog(), plantName, plantAddress);
                LogService.WriteLog(errMessage, LogService.TypeLevel.Error, ex);
            }
            return result;
        }

        public static Plant AddPlant(string plantName, string plantAddress, Guid? userId)
        {
            Plant result = null;

            if (string.IsNullOrWhiteSpace(plantName))
            {
                plantName = "DEFAULT";
            }

            try
            {
                using (TransactionScope transaction = new TransactionScope(TransactionScopeOption.Required))
                {
                    using (FST_FomMonitoringEntities ent = new FST_FomMonitoringEntities())
                    {
                        Plant plant = new Plant();
                        plant.Name = plantName;
                        plant.Address = plantAddress;
                        plant.UserId = userId;
                        ent.Plant.Add(plant);
                        ent.SaveChanges();
                        transaction.Complete();

                        result = plant;
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
                    var query = ent.Machine.Where(w => w.Plant != null).Select(s => s.Plant).Distinct().ToList();
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
