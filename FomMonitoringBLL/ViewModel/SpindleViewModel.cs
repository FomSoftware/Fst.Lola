using System;
using System.Linq;
using System.Web;

namespace FomMonitoringBLL.ViewModel
{
    public class SpindleViewModel
    {
        public SpindleVueModel vm_spindles { get; set; }
        public MotorAxesParameterVueModel vm_motoraxes_blitz { get; set; }
        public MachineInfoViewModel vm_machine_info { get; set; }
        public MotorKeopeParameterVueModel vm_motor_keope { get; set; }
        public AxesKeopeParameterVueModel vm_axes_keope { get; set; }
    }
}