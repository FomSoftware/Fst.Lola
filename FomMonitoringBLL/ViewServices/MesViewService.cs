﻿using FomMonitoringBLL.ViewModel;
using FomMonitoringCore.Framework.Common;
using FomMonitoringCore.Framework.Model;
using FomMonitoringCore.Service;
using System.Collections.Generic;
using System.Linq;

namespace FomMonitoringBLL.ViewServices
{
    public class MesViewService : IMesViewService
    {
        private readonly IMesService _mesService;
        public MesViewService(IMesService mesService)
        {
            _mesService = mesService;
        }

        public MesViewModel GetMes(ContextModel context)
        {
            var result = new MesViewModel
            {
                machines = GetVueModel(context.ActualPlant, context.AllMachines, !(context.User.Role == enRole.Administrator
                                                                                    || context.User.Role == enRole.Demo
                                                                                   || context.User.Role == enRole.Assistance))
            };
            return result;
        }


        private List<MesDataViewModel> GetVueModel(PlantModel plant, List<MachineInfoModel> allMachines, bool onlyActive)
        {
            var result = new List<MesDataViewModel>();

            var dataAllMachines = _mesService.GetPlantData(plant);
            if (onlyActive)
            {
                dataAllMachines = dataAllMachines.Where(m => m.Expired == false).ToList();
            }


            foreach (var dataMachine in dataAllMachines)
            {
                var mes = new MesDataViewModel();

                var machine = allMachines.FirstOrDefault(w => w.Id == dataMachine.MachineId);

                if (machine?.Type == null || machine.Model == null)
                {
                    continue;
                }

                mes.info = new MachineInfoViewModel
                {
                    id = machine.Id,
                    description = machine.Description == string.Empty ? null : machine.Description,
                    model = machine.Model.Name,
                    machineName = machine.MachineName,
                    icon = machine.Type.Image,
                    expired = dataMachine.Expired,
                    id_mtype = machine.Type?.Id ?? 0,
                    serial = machine.Serial
                };


                var data = dataAllMachines.FirstOrDefault(w => w.MachineId == machine.Id);

                if (data == null)
                {
                    result.Add(mes);
                    continue;
                }

                if (data.enActualState != null)
                {
                    mes.state = new StateViewModel
                    {
                        code = data.enActualState.GetDescription(),
                        text = LocalizationService.GetResource(data.enActualState.ToString())
                    };

                    mes.error = data.ActualStateCode;
                }

                if (data.ActualJobPerc != null)
                {
                    mes.job = new JobDataModel
                    {
                        code = data.ActualJobCode,
                        perc = (data.ActualJobPerc ?? 0).RoundToInt()
                    };
                }

                mes.@operator = data.ActualOperator;

                if (data.StateEfficiency != null)
                {
                    mes.efficiency = new EfficiencyVueModel();
                    mes.efficiency.kpi = CommonViewService.getKpiViewModel((decimal)(data.StateEfficiency ?? 0), machine.StateProductivityGreenThreshold, machine.StateProductivityYellowThreshold);
                    mes.efficiency.overfeed = CommonViewService.getKpiViewModel((decimal)(data.StateOverfeedAvg ?? 0), null, null);
                }
                
                if (data.PieceCompletedCount != null)
                {
                    mes.productivity = new ProductivityVueModel();
                    mes.productivity.kpi = CommonViewService.getKpiViewModel(Common.GetRatioProductivity(data.PieceCompletedCount, data.PieceElapsedTime),
                                                                machine.PiecesProductivityGreenThreshold,
                                                                machine.PiecesProductivityYellowThreshold);
                    mes.productivity.piece = new PieceViewModel()
                    {
                        total = (data.PieceCompletedCount ?? 0) + (data.PieceRedoneCount ?? 0)
                    };
                }

                if (data.AlarmCount != null)
                {
                    mes.errors = new ErrorViewModel();
                    mes.errors.quantity = data.AlarmCount;
                }
                result.Add(mes);
            }

            return result;
        }

    }
}
