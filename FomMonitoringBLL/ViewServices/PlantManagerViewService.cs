using System;
using System.Collections.Generic;
using System.Linq;
using FomMonitoringBLL.ViewModel;
using FomMonitoringCore.Framework.Common;
using FomMonitoringCore.Framework.Model;
using FomMonitoringCore.Service;

namespace FomMonitoringBLL.ViewServices
{
    public class PlantManagerViewService
    {

        public static PlantManagerViewModel GetPlants(ContextModel context)
        {
            var plantManager = new PlantManagerViewModel();
            string usernameCustomer = null;

            if (context.User.Role != enRole.Administrator)
                usernameCustomer = context.User.Username;

            var plantsModel = PlantManagerService.GetPlants(usernameCustomer);
            plantManager.Plants = plantsModel.Where(p => !string.IsNullOrWhiteSpace(p.CustomerName)).Select(s => new PlantViewModel
            {
                Id = s.Id,
                Name = s.Name,
                Address = s.Address,
                MachineSerials = s.Machines.Select(u => $"({u.Serial})-{u.MachineName}").ToList(),
                CustomerName = s.CustomerName,
                Machines = s.Machines.Select(n => new UserMachineViewModel
                {
                    Id = n.Id,
                    Serial = n.Serial
                }).ToList()
            }).ToList();

            plantManager.Customers = UserManagerService.GetCustomerNames();

            return plantManager;
        }


        public static PlantManagerViewModel GetPlantsByCustomer(string idCustomer)
        {
            var plantManager = new PlantManagerViewModel();

            var plantsModel = PlantManagerService.GetPlants(idCustomer);
            plantManager.Plants = plantsModel.Select(s => new PlantViewModel
            {
                Id = s.Id,
                Name = s.Name,
                Address = s.Address,
                MachineSerials = s.Machines.Select(u => u.Serial).ToList(),
                CustomerName = s.CustomerName,
                Machines = s.Machines.Select(n => new UserMachineViewModel
                {
                    Id = n.Id,
                    Serial = n.Serial
                }).ToList()
            }).ToList();
            return plantManager;
        }

        public static PlantManagerViewModel GetPlant(int id)
        {
            var result = new PlantManagerViewModel();
            var plantModel = PlantManagerService.GetPlant(id);

            var plant = new PlantViewModel
            {
                Id = plantModel.Id,
                Name = plantModel.Name,
                Address = plantModel.Address,
                CustomerName = plantModel.CustomerName,
                MachineSerials = plantModel.Machines.Select(u => u.Serial).ToList(),
                Machines = plantModel.Machines.Select(n => new UserMachineViewModel {
                    Id = n.Id,
                    Serial = n.Serial
                }).ToList()
            };

            result.Plant = plant;

            result.Machines = UserManagerViewService.GetMachinesByCustomer(plantModel.CustomerName);
            return result;

        }


        public static PlantManagerViewModel GetPlantByMachine(int id)
        {
            var result = new PlantManagerViewModel();
            var plantModel = PlantManagerService.GetPlantByMachine(id);

            var plant = new PlantViewModel
            {
                Id = plantModel.Id,
                Name = plantModel.Name,
                Address = plantModel.Address,
                CustomerName = plantModel.CustomerName,
                MachineSerials = plantModel.Machines.Select(u => u.Serial).ToList(),
                Machines = plantModel.Machines.Select(n => new UserMachineViewModel
                {
                    Id = n.Id,
                    Serial = n.Serial
                }).ToList()
            };

            result.Plant = plant;

            result.Machines = UserManagerViewService.GetMachinesByCustomer(plantModel.CustomerName);
            return result;

        }

        public static bool EditPlant(PlantViewModel plantModel)
        {
            try
            {
                var plant = new PlantModel
                {
                    Id = plantModel.Id,
                    Address = plantModel.Address,
                    Name = plantModel.Name,
                    Machines = plantModel.Machines.Select(m => new MachineInfoModel
                    {
                        Id = m.Id
                    }).ToList(),
                    CustomerName = plantModel.CustomerName,
                    LastDateUpdate = DateTime.UtcNow
                };

                return PlantManagerService.ModifyPlant(plant);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static IEnumerable<UserMachineViewModel> GetMachinesByPlant(int id)
        {
            return PlantManagerService.GetMachinesByPlant(id).Select(n => new UserMachineViewModel
            {
                Id = n.Id,
                Serial = n.Serial
            }).ToList();
        }

        public static bool CreatePlant(PlantViewModel plantModel, ContextModel context)
        {
            try
            {
                PlantModel plant = new PlantModel
                {
                    Id = plantModel.Id,
                    Name = plantModel.Name,
                    Address = plantModel.Address,
                    Machines = plantModel.Machines.Select(m => new MachineInfoModel
                    {
                        Id = m.Id
                    }).ToList(),
                    CustomerName = plantModel.CustomerName,
                    LastDateUpdate = DateTime.UtcNow
                };


                PlantManagerService.CreatePlant(plant);
                
                return true;
            }
            catch (Exception ex)
            {
                if (ex is InvalidOperationException)
                    return false;

                throw ex;
            }
        }

        public static bool DeletePlant(int id)
        {
            return PlantManagerService.DeletePlant(id);
        }
        
    }
}
