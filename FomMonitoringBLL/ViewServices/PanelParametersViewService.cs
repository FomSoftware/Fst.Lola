using FomMonitoringBLL.ViewModel;
using FomMonitoringCore.Framework.Common;
using FomMonitoringCore.Framework.Model;
using FomMonitoringCore.Service;
using System;
using System.Linq;

namespace FomMonitoringBLL.ViewServices
{
    public class PanelParametersViewService : IPanelParametersViewService
    {
        private readonly IMachineService _machineService;
        private readonly ISpindleViewService _spindleViewService;
        private readonly IParameterMachineService _parameterMachineService;
        private readonly IToolService _toolsService;

        public PanelParametersViewService(IMachineService machineService, ISpindleViewService spindleViewService, IParameterMachineService parameterMachineService, IToolService toolsService)
        {
            _machineService = machineService;
            _spindleViewService = spindleViewService;
            _parameterMachineService = parameterMachineService;
            _toolsService = toolsService;
        }

        public PanelParametersViewModel GetParameters(ContextModel context)
        { 
            PanelParametersViewModel result = new PanelParametersViewModel();

            if (_machineService.GetMachinePanels(context).Contains((int)enPanel.BlitzMotorAxes))
            {
                result.vm_motoraxes_blitz = GetVueModelBlitz(context.ActualMachine, true);
            }
            else
            {
                if (_machineService.GetMachinePanels(context).Contains((int)enPanel.KeopeMotors))
                {
                    result.vm_motor_keope = GetVueModelKeope(context.ActualMachine);                   
                }
                else
                {
                    result.vm_spindles = _spindleViewService.GetSpindles(context);
                }
                if (_machineService.GetMachinePanels(context).Contains((int)enPanel.KeopeAxes))
                {
                    result.vm_axes_keope = GetAxesVueModelKeope(context.ActualMachine);
                }

            }

            if (_machineService.GetMachinePanels(context).Contains((int)enPanel.Electrospindle))
            {
                result.vm_electro_spindle = GetElectroSpindleVueModel(context.ActualMachine);
            }
            if (_machineService.GetMachinePanels(context).Contains((int)enPanel.OtherMachineData))
            {
                result.vm_other_data = GetOtherDataVueModel(context.ActualMachine);
            }
            if (_machineService.GetMachinePanels(context).Contains((int)enPanel.ToolsFmcLmx))
            {
                result.vm_tools_fmc_lmx = GetToolsFmcLmxVueModel(context.ActualMachine);
            }
           

            result.vm_machine_info = new MachineInfoViewModel
            {
                model = context.ActualMachine.Model.Name,
                mtype = context.ActualMachine.Type.Name,
                id_mtype = context.ActualMachine.Type.Id,
                machineName = context.ActualMachine.MachineName
            };

            return result;
        }

        private OtherDataParameterVueModel GetOtherDataVueModel(MachineInfoModel machine)
        {
            var par = _parameterMachineService.GetParameters(machine, (int)enPanel.OtherMachineData);
            var result = new OtherDataParameterVueModel
            {
                AsseXKm = par.FirstOrDefault(p => p.VarNumber == 3000),
                AsseYKm = par.FirstOrDefault(p => p.VarNumber == 3002),
                AsseZKm = par.FirstOrDefault(p => p.VarNumber == 3004),
                CountRotationA = par.FirstOrDefault(p => p.VarNumber == 3006),
                CycleCountXFlow = par.FirstOrDefault(p => p.VarNumber == 3200),
                OreVitaMacchina = par.FirstOrDefault(p => p.VarNumber == 369),
                OreParzialiDaIngrassaggio = par.FirstOrDefault(p => p.VarNumber == 365)
            };
            return result;
        }

        private ToolsFmcLmxParameterVueModel GetToolsFmcLmxVueModel(MachineInfoModel context)
        {
            var par = _parameterMachineService.GetParameters(context, (int)enPanel.ToolsFmcLmx);
            var tools = _toolsService.GetTools(context).Where(n => n.IsActive);
            var dtos = tools.Select(n => new ToolParameterMachineValueModel
            {
                Code = par.FirstOrDefault(p => p.Cluster == n.Code)?.Cluster,
                Description = n.Description,
                ElapsedLife = par.FirstOrDefault(p => p.Cluster == n.Code)?.ConvertedValue
            }).ToList();

            var result = new ToolsFmcLmxParameterVueModel
            {
                ToolsInfo = dtos
            };
            return result;
        }


        private ElectroSpindleParameterVueModel GetElectroSpindleVueModel(MachineInfoModel machine)
        {
            var par = _parameterMachineService.GetParameters(machine, (int)enPanel.Electrospindle);
            var result = new ElectroSpindleParameterVueModel
            {
                OreLavoroTotali = par.FirstOrDefault(p => p.VarNumber == 368),
                SblocchiPinza = par.FirstOrDefault(p => p.VarNumber == 103),
                QtaSovrassorbimento = par.FirstOrDefault(p => p.VarNumber == 3024),
                RpmRange1500 = par.FirstOrDefault(p => p.VarNumber == 3030),
                RpmRange5500 = par.FirstOrDefault(p => p.VarNumber == 3032),
                RpmRange8000 = par.FirstOrDefault(p => p.VarNumber == 3034),
                RpmRange11500 = par.FirstOrDefault(p => p.VarNumber == 3036),
                RpmRange14500 = par.FirstOrDefault(p => p.VarNumber == 3038),
                RpmRange20000 = par.FirstOrDefault(p => p.VarNumber == 3040),
            };


            return result;
        }

        private MotorKeopeParameterVueModel GetVueModelKeope(MachineInfoModel machine, bool xmodule = false)
        {

            var par = _parameterMachineService.GetParameters(machine, (int)enPanel.KeopeMotors);

            var result = new MotorKeopeParameterVueModel
            {
                fixedHead = par.Where(p => p.VarNumber == 428 || p.VarNumber == 432).OrderBy(n => n.VarNumber).ToList(),

                mobileHead = par.Where(p => p.VarNumber == 430 || p.VarNumber == 434).OrderBy(n => n.VarNumber).ToList(),
            };

            foreach (var motFh in result.fixedHead)
            {
                motFh.Value = double.IsNaN(double.Parse(motFh.Value)) ? "" : $"{double.Parse(motFh.Value):#,0}";
            }

            foreach (var motMh in result.mobileHead)
            {
                motMh.Value = double.IsNaN(double.Parse(motMh.Value)) ? "" : $"{double.Parse(motMh.Value):#,0}";
            }


            return result;
        }

        private AxesKeopeParameterVueModel GetAxesVueModelKeope(MachineInfoModel machine, bool xmodule = false)
        {

            var par = _parameterMachineService.GetParameters(machine, (int)enPanel.KeopeAxes);

            var result = new AxesKeopeParameterVueModel
            {
                axes = par.Where(p => p.VarNumber == 450 || p.VarNumber == 452 || p.VarNumber == 454 || p.VarNumber == 456 || p.VarNumber == 458).OrderBy(n => n.VarNumber).ToList(),                
            };

            foreach (var ax in result.axes)
            {
                ax.Value = double.IsNaN(double.Parse(ax.Value)) ? "" : ax.ConvertedDistanceValue("0.000");
            }
          
            return result;
        }

        private MotorAxesParameterVueModel GetVueModelBlitz(MachineInfoModel machine, bool xmodule = false)
        {

            var par = _parameterMachineService.GetParameters(machine, (int)enPanel.BlitzMotorAxes);

            var result = new MotorAxesParameterVueModel
            {
                motors = par.Where(p => p.VarNumber == 428 || p.VarNumber == 430 || p.VarNumber == 432 || p.VarNumber == 434).OrderBy(n => n.VarNumber).ToList(),

                axes = par.Where(p => p.VarNumber == 450 || p.VarNumber == 452 || p.VarNumber == 454).OrderBy(n => n.VarNumber).ToList(),
            };

            foreach(var mot in result.motors)
            {
                mot.Value = double.IsNaN(double.Parse(mot.Value)) ? "" : double.Parse(mot.Value).ToString("0.000");
            }

            foreach (var ax in result.axes)
            {
                ax.Value = double.IsNaN(double.Parse(ax.Value)) ? "" : ax.ConvertedDistanceValue("0.000");
            }


            return result;
        }



    }
}
