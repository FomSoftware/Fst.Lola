﻿using FomMonitoringBLL.ViewModel;
using FomMonitoringCore.Framework.Common;
using FomMonitoringCore.Framework.Model;
using FomMonitoringCore.Service;
using System;
using System.Linq;

namespace FomMonitoringBLL.ViewServices
{
    public class PanelParametersViewService : IPanelParametersViewService
    {
        private IMachineService _machineService;
        private ISpindleViewService _spindleViewService;
        private IParameterMachineService _parameterMachineService;

        public PanelParametersViewService(IMachineService machineService, ISpindleViewService spindleViewService, IParameterMachineService parameterMachineService)
        {
            _machineService = machineService;
            _spindleViewService = spindleViewService;
            _parameterMachineService = parameterMachineService;
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
            
            result.vm_machine_info = new MachineInfoViewModel
            {
                model = context.ActualMachine.Model.Name,
                mtype = context.ActualMachine.Type.Name,
                id_mtype = context.ActualMachine.Type.Id,
                machineName = context.ActualMachine.MachineName
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
                motFh.Value = double.IsNaN(double.Parse(motFh.Value)) ? "" : string.Format("{0:#,0}", double.Parse(motFh.Value));
            }

            foreach (var motMh in result.mobileHead)
            {
                motMh.Value = double.IsNaN(double.Parse(motMh.Value)) ? "" : string.Format("{0:#,0}", double.Parse(motMh.Value));
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
                ax.Value = double.IsNaN(double.Parse(ax.Value)) ? "" : ax.ConvertedValue("0.000");
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
                ax.Value = double.IsNaN(double.Parse(ax.Value)) ? "" : ax.ConvertedValue("0.000");
            }


            return result;
        }



    }
}
