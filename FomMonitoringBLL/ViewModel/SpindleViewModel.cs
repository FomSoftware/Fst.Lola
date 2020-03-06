using FomMonitoringCore.Framework.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FomMonitoringBLL.ViewModel
{
    public class PanelParametersViewModel
    {
        public MotorAxesParameterVueModel vm_motoraxes_blitz { get; set; }
        public MachineInfoViewModel vm_machine_info { get; set; }
        public MotorKeopeParameterVueModel vm_motor_keope { get; set; }
        public AxesKeopeParameterVueModel vm_axes_keope { get; set; }
        public AxesLmxParameterVueModel vm_axes_lmx { get; set; }
        public OtherDataParameterVueModel vm_other_data { get; set; }
        public ElectroSpindleParameterVueModel vm_electro_spindle { get; set; }
        public ToolsFmcLmxParameterVueModel vm_tools_fmc_lmx { get; set; }
        public MultiSpindleParameterVueModel vm_multi_spindle { get; set; }
        public TiltingAxesParameterVueModel vm_tilting_axes { get; set; }
        public RotaryAxesParameterVueModel vm_rotary_axes { get; set; }
        public SensorSpindlesParameterVueModel vm_sensor_spindles { get; set; }
        public MotorBladeLmxParameterVueModel vm_motor_blade { get; set; }
        public ToolsFmcLmxParameterVueModel vm_xtools_lmx { get; set; }

        public OtherDataLmxParameterVueModel vm_other_data_lmx { get; set; }
    }

    public class OtherDataLmxParameterVueModel
    {
        public ParameterMachineValueModel OreVitaMacchina { get; set; }
        public ParameterMachineValueModel OreUltimoIngr { get; set; }
        public ParameterMachineValueModel NumBarreCaricate { get; set; }
        public ParameterMachineValueModel EtiMancanti { get; set; }
        public ParameterMachineValueModel EtiPerse { get; set; }
        public ParameterMachineValueModel EtiStampate { get; set; }
    }

    public class MotorBladeLmxParameterVueModel
    {
        public ParameterMachineValueModel RpmRange1500 { get; set; }
        public ParameterMachineValueModel RpmRange2500 { get; set; }
        public ParameterMachineValueModel RpmRange3000 { get; set; }
        public ParameterMachineValueModel TempoSovraAss { get; set; }
        public ParameterMachineValueModel QtaSovraAss { get; set; }
        public ParameterMachineValueModel TempoTot { get; set; }
        public ParameterMachineValueModel TagliLamaTot { get; set; }
        public ParameterMachineValueModel TagliLamaPar { get; set; }
    }

    public class SensorSpindlesParameterVueModel
    {
        public ParameterMachineValueModel SoglieAmpereMandrini { get; set; }
        public ParameterMachineValueModel SoglieAmpereContatore { get; set; }
        public ParameterMachineValueModel AccelerometroINT_1 { get; set; }
        public ParameterMachineValueModel AccelerometroINT_2 { get; set; }
        public ParameterMachineValueModel AccelerometroINT_3 { get; set; }
        public ParameterMachineValueModel HSD_NumCollRilevate { get; set; }
        public ParameterMachineValueModel AccelContatoreINT_2 { get; set; }
        public ParameterMachineValueModel AccelContatoreINT_3 { get; set; }
        public ParameterMachineValueModel TemperSchedaMinutiINT { get; set; }
        public ParameterMachineValueModel TemperSchedaContatoreINT { get; set; }
        public ParameterMachineValueModel TemperStatoreMinutiINT { get; set; }
        public ParameterMachineValueModel TemperStatoreContatoreINT { get; set; }
        public ParameterMachineValueModel TemperCuscinettiMinutiINT { get; set; }
        public ParameterMachineValueModel TemperCuscinettiContatoreINT { get; set; }

    }

    public class RotaryAxesParameterVueModel
    {
        public ParameterMachineValueModel NrotazioniAsse3C1 { get; set; }
        public ParameterMachineValueModel NrotazioniAsse3C2 { get; set; }
        public ParameterMachineValueModel NsblocchiForc1 { get; set; }
        public ParameterMachineValueModel NsblocchiForc2 { get; set; }
        public ParameterMachineValueModel NsblocchiForc3 { get; set; }
    }

    public class TiltingAxesParameterVueModel
    {
        public ParameterMachineValueModel NrotazioniAsse2A1 { get; set; }
        public ParameterMachineValueModel NrotazioniAsse2A2 { get; set; }
        public ParameterMachineValueModel NrotazioniAsse2A3 { get; set; }
        public ParameterMachineValueModel NrotazioniAsse2A4 { get; set; }
        public ParameterMachineValueModel NrotazioniAsse2A5 { get; set; }
        public ParameterMachineValueModel NrotazioniAsse2A6 { get; set; }
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
        public ParameterMachineValueModel OreLavoroTotali { get; set; }
        public ParameterMachineValueModel SblocchiPinza { get; set; }
        public ParameterMachineValueModel QtaSovrassorbimento { get; set; }
        public ParameterMachineValueModel RpmRange1500 { get; set; }
        public ParameterMachineValueModel RpmRange5500 { get; set; }
        public ParameterMachineValueModel RpmRange8000 { get; set; }
        public ParameterMachineValueModel RpmRange11500 { get; set; }
        public ParameterMachineValueModel RpmRange14500 { get; set; }
        public ParameterMachineValueModel RpmRange20000 { get; set; }
        public bool ShowSovrassorbimento { get; set; }
    }

    public class MultiSpindleParameterVueModel
    {
        public ParameterMachineValueModel OreLavoroTotali { get; set; }
        public ParameterMachineValueModel RpmRange1500 { get; set; }
        public ParameterMachineValueModel RpmRange3999 { get; set; }
        public ParameterMachineValueModel RpmRange7999 { get; set; }
        public ParameterMachineValueModel RpmRange11500 { get; set; }
        public ParameterMachineValueModel RpmRange14500 { get; set; }
        public ParameterMachineValueModel RpmRange20000 { get; set; }
        public int Posizione { get; set; }
    }

    public class ToolsFmcLmxParameterVueModel
    {
        public IEnumerable<ToolParameterMachineValueModel> ToolsInfo { get; set; } = new List<ToolParameterMachineValueModel>();
        public int PanelId { get; set; }
    }

    public class ToolParameterMachineValueModel
    {
        public string Code { get; set; }
        public string Description { get; set; }
        public string ElapsedLife { get; set; }
    }

    public class AxesLmxParameterVueModel
    {
        public IEnumerable<ParameterMachineValueModel> AxesLmx { get; set; } = new List<ParameterMachineValueModel>();
    }
}