using System;
using FomMonitoringBLL.ViewModel;
using FomMonitoringCore.Framework.Common;
using FomMonitoringCore.Framework.Model;
using FomMonitoringCore.Service;
using System.Collections.Generic;
using System.Linq;

namespace FomMonitoringBLL.ViewServices
{
    public class AssistanceViewService : IAssistanceViewService
    {
        private readonly IAssistanceService _assistanceService;
        public AssistanceViewService(IAssistanceService myService)
        {
            _assistanceService = myService;
        }

        public AssistanceViewModel GetAssistance(ContextModel context)
        {
            var result = new AssistanceViewModel
            {
                machines = GetMachines(context.AllMachines, false),
                customers = GetCustomers()
            };
            return result;
        }


        private List<UserViewModel> GetCustomers()
        {
            List<UserViewModel> result = new List<UserViewModel>();
            var lista = _assistanceService.GetCustomers();
            foreach (var user in lista)
            {
                if (user.CompanyName != null)
                {
                    var userModel = new UserViewModel
                    {
                        CompanyName = user.CompanyName,
                        Username = user.Username,
                        Enabled = user.Enabled,
                        ID = user.ID
                    };

                    result.Add(userModel);
                }
            }

            return result;
        }

        private List<MachineInfoViewModel> GetMachines(List<MachineInfoModel> allMachines, bool onlyActive)
        {
            var result = new List<MachineInfoViewModel>();
            List<MachineInfoModel> dataAllMachines = null;
            if (onlyActive)
            {
                dataAllMachines = allMachines.Where(m => m.PlantId > 0 && (m.ExpirationDate == null || m.ExpirationDate > DateTime.UtcNow)).OrderBy(m => m.Serial).ToList();
            }
            else
            {
                dataAllMachines = allMachines.Where(p => p.PlantId > 0).OrderBy(m => m.Serial).ToList();
            }

            dataAllMachines.OrderBy(m => m.Serial);
            foreach (var dataMachine in dataAllMachines)
            { 
                var machine = allMachines.FirstOrDefault(w => w.Id == dataMachine.Id);

                if (machine?.Type == null || machine.Model == null)
                {
                    continue;
                }

                var mac = new MachineInfoViewModel
                {
                    id = machine.Id,
                    description = machine.Description == string.Empty ? null : machine.Description,
                    model = machine.Model.Name,
                    machineName = machine.MachineName,
                    icon = machine.Type.Image,
                    expired = dataMachine.ExpirationDate == null || dataMachine.ExpirationDate > DateTime.UtcNow,
                    id_mtype = machine.Type?.Id ?? 0,
                    serial = machine.Serial
                };

                result.Add(mac);
        }

            result.OrderBy(m => m.serial).ToList();
            return result;
        }

        public void SetCompanyName(ContextModel context)
        {
            if (context.User.Role == enRole.Assistance || context.User.Role == enRole.RandD)
            {
                if (context.AssistanceMachineId != null)
                {
                    context.CompanyName = _assistanceService.GetMachineCustomer((int)context.AssistanceMachineId).CompanyName;
                }
                else if (context.AssistanceUserId != null)
                {
                    context.CompanyName = _assistanceService.GetUser(context.AssistanceUserId).CompanyName;
                }
            }
        }

    }
}
