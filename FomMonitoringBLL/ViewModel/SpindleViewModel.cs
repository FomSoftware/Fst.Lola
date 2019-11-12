using FomMonitoringCore.Framework.Model;
using System;
using System.Linq;
using System.Web;

namespace FomMonitoringBLL.ViewModel
{
    public class PanelParametersViewModel
    {
        public SpindleVueModel vm_spindles { get; set; }
        public MotorAxesParameterVueModel vm_motoraxes_blitz { get; set; }
        public MachineInfoViewModel vm_machine_info { get; set; }
        public MotorKeopeParameterVueModel vm_motor_keope { get; set; }
        public AxesKeopeParameterVueModel vm_axes_keope { get; set; }
        public OtherDataParameterVueModel vm_other_data { get; set; }
        public ElectroSpindleParameterVueModel vm_electro_spindle { get; set; }
        
    }


    public class OtherDataParameterVueModel
    {
        public ParameterMachineValueModel CycleCountXFlow { get; set; }
        public ParameterMachineValueModel OreVitaMacchina { get; set; }
        public ParameterMachineValueModel OreParzialiDaIngrassaggio { get; set; }
        public ParameterMachineValueModel AsseXKm { get; set; }
        public ParameterMachineValueModel AsseYKm { get; set; }
        public ParameterMachineValueModel AsseZKm { get; set; }
        public ParameterMachineValueModel CountRotationA { get; set; }
    }

    public class ElectroSpindleParameterVueModel
    {

    }

}