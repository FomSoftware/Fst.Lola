using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FomMonitoringCore.DAL;
using FomMonitoringCore.Framework.Common;
using FomMonitoringCore.Framework.Model;
using Mapster;
using UserManager.DAL;

namespace FomMonitoringCore.Service
{
    public class PlantManagerService : IPlantManagerService
    {
        private IFomMonitoringEntities _context;

        public PlantManagerService(IFomMonitoringEntities context)
        {
            _context = context;
        }

        public List<PlantModel> GetPlants(string usernameCustomer)
        {
            List<PlantModel> result = null;
            try
            {                
                using (UserManagerEntities entUM = new UserManagerEntities())
                {
                    //ent.Configuration.LazyLoadingEnabled = false;
                    // Recupero la lista degli utenti associati al cliente
                    List<Plant> customerPlants = new List<Plant>();
                    if (!string.IsNullOrWhiteSpace(usernameCustomer))
                    {
                        var gc = entUM.Users.FirstOrDefault(f => f.Username == usernameCustomer)?.ID;
                            
                        customerPlants = _context.Set<Plant>().Where(w => w.UserId == gc).Distinct().ToList();
                        if (customerPlants.Count == 0) return result;
                        
                    }
                    else
                    {
                        customerPlants = _context.Set<Plant>().Include("Machine").ToList();
                    }

                    result = customerPlants.Adapt<List<Plant>, List<PlantModel>>();
                    // Associo il cliente all'utente
                    if (!string.IsNullOrWhiteSpace(usernameCustomer))
                    {
                        result.ForEach(fe => fe.CustomerName = usernameCustomer);
                    }
                    else
                    {
                        var userCustomer = _context.Set<UserCustomerMapping>().ToList();
                        result.ForEach(fe => fe.CustomerName = userCustomer.FirstOrDefault(w => w.UserId == fe.UserId)?.CustomerName);
                    }
                        
                }
                
            }
            catch (Exception ex)
            {
                string errMessage = string.Format(ex.GetStringLog(), usernameCustomer);
                LogService.WriteLog(errMessage, LogService.TypeLevel.Error, ex);
            }

            return result;
        }

        public PlantModel GetPlant(int plantId)
        {
            PlantModel result = null;

            try
            {               
                var plant = _context.Set<Plant>().FirstOrDefault(f => f.Id == plantId);
                if (plant == null) return result;

                result = plant.Adapt<Plant, PlantModel>();

                // Recupero le sue macchine ed il customer associato
                using (UserManagerEntities entUM = new UserManagerEntities())
                {
                    entUM.Configuration.LazyLoadingEnabled = false;
                    var customerName = entUM.Users.FirstOrDefault(f => f.ID == plant.UserId)?.Username;
                    result.CustomerName = customerName;
                        
                }
                
            }
            catch (Exception ex)
            {
                string errMessage = string.Format(ex.GetStringLog(), plantId.ToString());
                LogService.WriteLog(errMessage, LogService.TypeLevel.Error, ex);
            }

            return result;
        }

        public PlantModel GetPlantByMachine(int id)
        {
            PlantModel result = null;

            try
            {                
                var plant = _context.Set<Plant>().FirstOrDefault(f => f.Machine.Any(m => m.Id == id));
                if (plant == null) return result;

                result = plant.Adapt<Plant, PlantModel>();

                // Recupero le sue macchine ed il customer associato
                using (UserManagerEntities entUM = new UserManagerEntities())
                {
                    entUM.Configuration.LazyLoadingEnabled = false;
                    var customerName = entUM.Users.FirstOrDefault(f => f.ID == plant.UserId)?.Username;
                    result.CustomerName = customerName;

                }                
            }
            catch (Exception ex)
            {
                string errMessage = string.Format(ex.GetStringLog(), id.ToString());
                LogService.WriteLog(errMessage, LogService.TypeLevel.Error, ex);
            }

            return result;
        }

        public bool DeletePlant(int id)
        {
            try
            {               
                var plant = _context.Set<Plant>().Find(id);
                if(plant != null)
                {

                    _context.Set<Plant>().Remove(plant);
                }

                _context.SaveChanges();
                return true;
                
            }
            catch (Exception ex)
            {
                string errMessage = ex.GetStringLog();
                LogService.WriteLog(errMessage, LogService.TypeLevel.Error, ex);
                throw ex;
            }
        }

        public IEnumerable<MachineInfoModel> GetMachinesByPlant(int id)
        {
            try
            {
               var plantMachines = _context.Set<Plant>().Find(id)?.Machine.ToList() ?? new List<Machine>();
               return plantMachines.Adapt<List<Machine>, List<MachineInfoModel>>();               
            }
            catch (Exception ex)
            {
                string errMessage = ex.GetStringLog();
                LogService.WriteLog(errMessage, LogService.TypeLevel.Error, ex);
                throw ex;
            }
        }

        public int CreatePlant(PlantModel plant)
        {
            try
            {
                Guid? customerId = null;
                using (UserManagerEntities entUM = new UserManagerEntities())
                {
                    entUM.Configuration.LazyLoadingEnabled = false;
                    customerId = entUM.Users.FirstOrDefault(f => f.Username == plant.CustomerName)?.ID;
                }

                var addPlant = new Plant
                {
                    Id = plant.Id,
                    Address = plant.Address,
                    Name = plant.Name,
                    UserId = customerId
                };

                _context.Set<Plant>().Add(addPlant);
                _context.SaveChanges();

                var idsMachines = plant.Machines.Select(i => i.Id).ToList();

                var newMachine = _context.Set<Machine>().Where(m => idsMachines.Any(mi => mi == m.Id)).ToList();
                newMachine.ForEach(m => {
                    m.Plant = addPlant;
                    m.PlantId = addPlant.Id;
                });

                _context.SaveChanges();


                return addPlant.Id;
                
            }
            catch (Exception ex)
            {
                string errMessage = ex.GetStringLog();
                LogService.WriteLog(errMessage, LogService.TypeLevel.Error, ex);
                throw ex;
            }
        }

        public bool ModifyPlant(PlantModel plant)
        {
            try
            {
                Guid? customerId = null;
                using (UserManagerEntities entUM = new UserManagerEntities())
                {
                    entUM.Configuration.LazyLoadingEnabled = false;
                    customerId = entUM.Users.FirstOrDefault(f => f.Username == plant.CustomerName)?.ID;
                }               
                var updPlant = _context.Set<Plant>().Find(plant.Id) ?? new Plant();

                updPlant.Id = plant.Id;
                updPlant.Name = plant.Name;
                updPlant.Address = plant.Address;
                updPlant.UserId = customerId;

                _context.Set<Plant>().AddOrUpdate(updPlant);
                _context.SaveChanges();

                var idsMachines = plant.Machines.Select(i => i.Id).ToList();
                var oldMachine = updPlant.Machine.Where(m => !idsMachines.Any(mi => mi == m.Id)).ToList();
                oldMachine.ForEach(m => {
                    m.Plant = null;
                    m.PlantId = null;
                });
                    
                var newMachine = _context.Set<Machine>().Where(m => idsMachines.Any(mi => mi == m.Id)).ToList();
                newMachine.ForEach(m => {
                    m.Plant = updPlant;
                    m.PlantId = updPlant.Id;
                });

                _context.SaveChanges();

                return true;
                
            }
            catch (Exception ex)
            {
                string errMessage = ex.GetStringLog();
                LogService.WriteLog(errMessage, LogService.TypeLevel.Error, ex);
                throw ex;
            }
        }
    }
}
