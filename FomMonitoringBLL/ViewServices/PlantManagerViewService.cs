using System;
using System.Collections.Generic;
using System.Linq;
using FomMonitoringBLL.ViewModel;
using FomMonitoringCore.Framework.Common;
using FomMonitoringCore.Framework.Model;
using FomMonitoringCore.Service;

namespace FomMonitoringBLL.ViewServices
{
    public class PlantManagerViewService : IPlantManagerViewService
    {
        private readonly IPlantManagerService _plantManagerService;
        private readonly IUserManagerService _userManagerService;
        private readonly IUserManagerViewService _userManagerViewService;

        public PlantManagerViewService(IPlantManagerService plantManagerService, IUserManagerService userManagerService, IUserManagerViewService userManagerViewService)
        {
            _plantManagerService = plantManagerService;
            _userManagerService = userManagerService;
            _userManagerViewService = userManagerViewService;
        }

        public PlantManagerViewModel GetPlants(ContextModel context)
        {
            var plantManager = new PlantManagerViewModel();
            string usernameCustomer = null;

            if (context.User.Role != enRole.Administrator && context.User.Role != enRole.Demo)
                usernameCustomer = context.User.Username;

            var plantsModel = _plantManagerService.GetPlants(usernameCustomer);
            if (context.User.Role == enRole.Demo)
            {
                plantsModel = _plantManagerService.FilterPlantsByRole(enRole.Demo, plantsModel);
            }

            plantManager.Plants = plantsModel.Where(p => !string.IsNullOrWhiteSpace(p.CustomerName)).Select(s => new PlantViewModel
            {
                Id = s.Id,
                Name = s.Name,
                Address = s.Address,
                MachineSerials = s.Machines.Where(m => m.ExpirationDate == null || m.ExpirationDate > DateTime.UtcNow).Select(u => $"({u.Serial})-{u.MachineName}").ToList(),
                CustomerName = s.CustomerName,
                Machines = s.Machines.Where(m => m.ExpirationDate == null || m.ExpirationDate > DateTime.UtcNow).Select(n => new UserMachineViewModel
                {
                    Id = n.Id,
                    Serial = n.Serial
                }).ToList()
            }).ToList();

            plantManager.Customers = _userManagerService.GetCustomerNames();

            return plantManager;
        }


        public PlantManagerViewModel GetPlantsByCustomer(string idCustomer)
        {
            var plantManager = new PlantManagerViewModel();

            var plantsModel = _plantManagerService.GetPlants(idCustomer);
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

        public PlantManagerViewModel GetPlant(int id)
        {
            var result = new PlantManagerViewModel();
            var plantModel = _plantManagerService.GetPlant(id);

            var plant = new PlantViewModel
            {
                Id = plantModel.Id,
                Name = plantModel.Name,
                Address = plantModel.Address,
                CustomerName = plantModel.CustomerName,
                MachineSerials = plantModel.Machines.Select(u => u.Serial).ToList(),
                Machines = plantModel.Machines.Where(n => n.ExpirationDate == null || n.ExpirationDate > DateTime.UtcNow).Select(n => new UserMachineViewModel {
                    Id = n.Id,
                    Serial = n.Serial
                }).ToList()
            };

            result.Plant = plant;

            result.Machines = _userManagerViewService.GetMachinesByCustomer(plantModel.CustomerName, false);
            return result;

        }


        public PlantManagerViewModel GetPlantByMachine(int id)
        {
            var result = new PlantManagerViewModel();
            var plantModel = _plantManagerService.GetPlantByMachine(id);
            if (plantModel == null) return result;
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

            result.Machines = _userManagerViewService.GetMachinesByCustomer(plantModel.CustomerName, false);
            return result;

        }

        public bool EditPlant(PlantViewModel plantModel)
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

                return _plantManagerService.ModifyPlant(plant);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IEnumerable<UserMachineViewModel> GetMachinesByPlant(int id)
        {
            return _plantManagerService.GetMachinesByPlant(id).Select(n => new UserMachineViewModel
            {
                Id = n.Id,
                Serial = n.Serial
            }).ToList();
        }

        public bool CreatePlant(PlantViewModel plantModel, ContextModel context)
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


                _plantManagerService.CreatePlant(plant);
                
                return true;
            }
            catch (Exception ex)
            {
                if (ex is InvalidOperationException)
                    return false;

                throw ex;
            }
        }

        public bool DeletePlant(int id)
        {
            return _plantManagerService.DeletePlant(id);
        }
        
    }
}
