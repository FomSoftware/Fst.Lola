using System.Collections.Generic;
using System.Linq;
using FomMonitoringBLL.ViewModel;
using FomMonitoringCore.Framework.Common;
using FomMonitoringCore.Framework.Model;
using FomMonitoringCore.Service;

namespace FomMonitoringBLL.ViewServices
{
    public class PanelParametersViewService : IPanelParametersViewService
    {
        private readonly IMachineService _machineService;
        private readonly IParameterMachineService _parameterMachineService;
        private readonly IToolService _toolsService;

        public PanelParametersViewService(IMachineService machineService,
            IParameterMachineService parameterMachineService, IToolService toolsService)
        {
            _machineService = machineService;
            _parameterMachineService = parameterMachineService;
            _toolsService = toolsService;
        }

        public PanelParametersViewModel GetParameters(ContextModel context)
        {
            var result = new PanelParametersViewModel();
            var panels = _machineService.GetMachinePanels(context);
            if (panels.Contains((int) enPanel.BlitzMotorAxes))
            {
                result.vm_motoraxes_blitz = GetVueModelBlitz(context.ActualMachine);
            }
            else
            {
                if (panels.Contains((int) enPanel.KeopeMotors))
                    result.vm_motor_keope = GetVueModelKeope(context.ActualMachine);

                if (panels.Contains((int) enPanel.KeopeAxes))
                    result.vm_axes_keope = GetAxesVueModelKeope(context.ActualMachine);

                if (panels.Contains((int) enPanel.LinearAxesLMX))
                    result.vm_axes_lmx = GetLinearAxesLmxVueModel(context.ActualMachine);
            }

            if (panels.Contains((int) enPanel.Electrospindle) ||
                panels.Contains((int) enPanel.XSpindles))
                result.vm_electro_spindle = GetElectroSpindleVueModel(context.ActualMachine);
            if (panels.Contains((int) enPanel.OtherMachineData))
                result.vm_other_data = GetOtherDataVueModel(context.ActualMachine);
            if (panels.Contains((int) enPanel.ToolsFmcLmx))
                result.vm_tools_fmc_lmx = GetToolsFmcLmxVueModel(context.ActualMachine, (int)enPanel.ToolsFmcLmx);
            if (panels.Contains((int)enPanel.MmToolsLmx))
                result.vm_tools_fmc_lmx = GetToolsFmcLmxVueModel(context.ActualMachine, (int)enPanel.MmToolsLmx);
            if (panels.Contains((int)enPanel.XmuToolsLmx))
                result.vm_xtools_lmx = GetToolsFmcLmxVueModel(context.ActualMachine, (int)enPanel.XmuToolsLmx);
            if (panels.Contains((int) enPanel.Multispindle))
                result.vm_multi_spindle = GetMultiSpindleVueModel(context.ActualMachine, 11);
            if (panels.Contains((int)enPanel.TiltingMSAxesLMX))
                result.vm_tilting_axes = GetTiltingAxesVueModel(context.ActualMachine);
            if (panels.Contains((int)enPanel.RotaryAxesLMX))
                result.vm_rotary_axes = GetRotaryAxesVueModel(context.ActualMachine);
            if (panels.Contains((int)enPanel.XMUSp_SensorsLMX))
                result.vm_sensor_spindles = GetSensorSpindlesVueModel(context.ActualMachine);
            if (panels.Contains((int)enPanel.MotorBladeLMX))
                result.vm_motor_blade = GetMotorBladeLmxVueModel(context.ActualMachine);
            if (panels.Contains((int)enPanel.OtherMachineDataLmx))
                result.vm_other_data_lmx = GetOtherDataLmxVueModel(context.ActualMachine);


            result.vm_machine_info = new MachineInfoViewModel
            {
                model = context.ActualMachine.Model.Name,
                mtype = context.ActualMachine.Type.Name,
                id_mtype = context.ActualMachine.Type.Id,
                machineName = context.ActualMachine.MachineName
            };

            return result;
        }

        private OtherDataLmxParameterVueModel GetOtherDataLmxVueModel(MachineInfoModel machine)
        {
            var par = _parameterMachineService.GetParameters(machine, (int)enPanel.OtherMachineDataLmx);
            var result = new OtherDataLmxParameterVueModel
            {
                OreVitaMacchina = par.FirstOrDefault(p => p.VarNumber == 542),
                OreUltimoIngr = par.FirstOrDefault(p => p.VarNumber == 20095),
                NumBarreCaricate = par.FirstOrDefault(p => p.VarNumber == 544),
                EtiMancanti = par.FirstOrDefault(p => p.VarNumber == 40402),
                EtiPerse = par.FirstOrDefault(p => p.VarNumber == 40400),
                EtiStampate = par.FirstOrDefault(p => p.VarNumber == 40401)
            };
            return result;
        }

        private MotorBladeLmxParameterVueModel GetMotorBladeLmxVueModel(MachineInfoModel machine)
        {
            var par = _parameterMachineService.GetParameters(machine, (int)enPanel.MotorBladeLMX);
            var result = new MotorBladeLmxParameterVueModel
            {
                RpmRange1500 = par.FirstOrDefault(p => p.VarNumber == 40111),
                RpmRange2500 = par.FirstOrDefault(p => p.VarNumber == 40112),
                RpmRange3000 = par.FirstOrDefault(p => p.VarNumber == 40113),
                TempoSovraAss = par.FirstOrDefault(p => p.VarNumber == 40115),
                QtaSovraAss = par.FirstOrDefault(p => p.VarNumber == 40118),
                TempoTot = par.FirstOrDefault(p => p.VarNumber == 40161),
                TagliLamaTot = par.FirstOrDefault(p => p.VarNumber == 543),
                TagliLamaPar = par.FirstOrDefault(p => p.VarNumber == 551)
            };
            return result;
        }

        public MultiSpindleParameterVueModel GetMultiSpindleVueModel(MachineInfoModel machine, int? position)
        {
            var par = _parameterMachineService.GetParameters(machine, (int) enPanel.Multispindle, position ?? 11);
            var result = GetMultiSpindleVueModelCluster(par, position ?? 11);
            return result;
        }

        private OtherDataParameterVueModel GetOtherDataVueModel(MachineInfoModel machine)
        {
            var par = _parameterMachineService.GetParameters(machine, (int) enPanel.OtherMachineData);
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

        private ToolsFmcLmxParameterVueModel GetToolsFmcLmxVueModel(MachineInfoModel context, int panel)
        {
            var par = _parameterMachineService.GetParameters(context, panel);
            bool xmodule = false;
            if(panel == (int)enPanel.XmuToolsLmx)
                xmodule = true;
            var tools = _toolsService.GetTools(context, xmodule).Where(n => n.IsActive);
            var dtos = tools.Select(n => new ToolParameterMachineValueModel
            {
                Code = par.FirstOrDefault(p => p.Cluster == n.Code)?.Cluster??n.Code,
                Description = n.Description,
                ElapsedLife = par.FirstOrDefault(p => p.Cluster == n.Code)?.ConvertedValue
            }).ToList();

            var result = new ToolsFmcLmxParameterVueModel
            {
                ToolsInfo = dtos,
                PanelId = panel
            };
            return result;
        }

        private SensorSpindlesParameterVueModel GetSensorSpindlesVueModel(MachineInfoModel machine)
        {
            var panels = _machineService.GetMachinePanels(machine.Model.Id);
            SensorSpindlesParameterVueModel result = null;
            if (panels.Contains((int)enPanel.XMUSp_SensorsLMX))
            {
                var par = _parameterMachineService.GetParameters(machine, (int)enPanel.XMUSp_SensorsLMX);
                result = new SensorSpindlesParameterVueModel
                {
                    SoglieAmpereMandrini = par.FirstOrDefault(p => p.VarNumber == 40131),
                    SoglieAmpereContatore = par.FirstOrDefault(p => p.VarNumber == 40135),
                    AccelerometroINT_1 = par.FirstOrDefault(p => p.VarNumber == 39601),
                    AccelerometroINT_2 = par.FirstOrDefault(p => p.VarNumber == 39602),
                    AccelerometroINT_3 = par.FirstOrDefault(p => p.VarNumber == 39603),
                    HSD_NumCollRilevate = par.FirstOrDefault(p => p.VarNumber == 32234),
                    AccelContatoreINT_2 = par.FirstOrDefault(p => p.VarNumber == 39612),
                    AccelContatoreINT_3 = par.FirstOrDefault(p => p.VarNumber == 39613),
                    TemperSchedaMinutiINT = par.FirstOrDefault(p => p.VarNumber == 39733),
                    TemperSchedaContatoreINT = par.FirstOrDefault(p => p.VarNumber == 39723),
                    TemperStatoreMinutiINT = par.FirstOrDefault(p => p.VarNumber == 39731),
                    TemperStatoreContatoreINT = par.FirstOrDefault(p => p.VarNumber == 39721),
                    TemperCuscinettiMinutiINT = par.FirstOrDefault(p => p.VarNumber == 39732),
                    TemperCuscinettiContatoreINT = par.FirstOrDefault(p => p.VarNumber == 39722)
                };
            }

            return result;
        }

        private RotaryAxesParameterVueModel GetRotaryAxesVueModel(MachineInfoModel machine)
        {
            var panels = _machineService.GetMachinePanels(machine.Model.Id);
            RotaryAxesParameterVueModel result = null;
            if (panels.Contains((int)enPanel.RotaryAxesLMX))
            {
                var par = _parameterMachineService.GetParameters(machine, (int)enPanel.RotaryAxesLMX);
                result = new RotaryAxesParameterVueModel
                {
                    NrotazioniAsse3C1 = par.FirstOrDefault(p => p.VarNumber == 308),
                    NrotazioniAsse3C2 = par.FirstOrDefault(p => p.VarNumber == 309),
                    NsblocchiForc1 = par.FirstOrDefault(p => p.VarNumber == 40301),
                    NsblocchiForc2 = par.FirstOrDefault(p => p.VarNumber == 40302),
                    NsblocchiForc3 = par.FirstOrDefault(p => p.VarNumber == 40303)
                };
            }

            return result;
        }

        private TiltingAxesParameterVueModel GetTiltingAxesVueModel(MachineInfoModel machine)
        {
            var panels = _machineService.GetMachinePanels(machine.Model.Id);
            TiltingAxesParameterVueModel result = null;
            if (panels.Contains((int) enPanel.TiltingMSAxesLMX))
            {
                var par = _parameterMachineService.GetParameters(machine, (int)enPanel.TiltingMSAxesLMX);
                result = new TiltingAxesParameterVueModel
                {
                    NrotazioniAsse2A1 = par.FirstOrDefault(p => p.VarNumber == 302),
                    NrotazioniAsse2A2 = par.FirstOrDefault(p => p.VarNumber == 303),
                    NrotazioniAsse2A3 = par.FirstOrDefault(p => p.VarNumber == 304),
                    NrotazioniAsse2A4 = par.FirstOrDefault(p => p.VarNumber == 305),
                    NrotazioniAsse2A5 = par.FirstOrDefault(p => p.VarNumber == 306),
                    NrotazioniAsse2A6 = par.FirstOrDefault(p => p.VarNumber == 307)
                };
            }

            return result;
        }

        private ElectroSpindleParameterVueModel GetElectroSpindleVueModel(MachineInfoModel machine)
        {
            var panels = _machineService.GetMachinePanels(machine.Model.Id);
            ElectroSpindleParameterVueModel result = null;
            if (panels.Contains((int) enPanel.Electrospindle))
            {
                var par = _parameterMachineService.GetParameters(machine, (int) enPanel.Electrospindle);
                result = new ElectroSpindleParameterVueModel
                {
                    OreLavoroTotali = par.FirstOrDefault(p => p.VarNumber == 3012),
                    SblocchiPinza = par.FirstOrDefault(p => p.VarNumber == 103),
                    QtaSovrassorbimento = par.FirstOrDefault(p => p.VarNumber == 3024),
                    RpmRange1500 = par.FirstOrDefault(p => p.VarNumber == 3030),
                    RpmRange5500 = par.FirstOrDefault(p => p.VarNumber == 3032),
                    RpmRange8000 = par.FirstOrDefault(p => p.VarNumber == 3034),
                    RpmRange11500 = par.FirstOrDefault(p => p.VarNumber == 3036),
                    RpmRange14500 = par.FirstOrDefault(p => p.VarNumber == 3038),
                    RpmRange20000 = par.FirstOrDefault(p => p.VarNumber == 3040),
                    ShowSovrassorbimento = true
                };
            }
            else if (panels.Contains((int) enPanel.XSpindles))
            {
                var par = _parameterMachineService.GetParameters(machine, (int) enPanel.XSpindles);
                result = new ElectroSpindleParameterVueModel
                {
                    OreLavoroTotali = par.FirstOrDefault(p => p.VarNumber == 40162),
                    SblocchiPinza = par.FirstOrDefault(p => p.VarNumber == 40300),
                    RpmRange1500 = par.FirstOrDefault(p => p.VarNumber == 40121),
                    RpmRange5500 = par.FirstOrDefault(p => p.VarNumber == 40122),
                    RpmRange8000 = par.FirstOrDefault(p => p.VarNumber == 40123),
                    RpmRange11500 = par.FirstOrDefault(p => p.VarNumber == 40124),
                    RpmRange14500 = par.FirstOrDefault(p => p.VarNumber == 40125),
                    RpmRange20000 = par.FirstOrDefault(p => p.VarNumber == 40126),
                    ShowSovrassorbimento = false
                };
            }

            return result;
        }

        private MultiSpindleParameterVueModel GetMultiSpindleVueModelCluster(List<ParameterMachineValueModel> par,
            int cluster)
        {
            string varOreTot = "TabRpmTotMandriniMM_"+ cluster.ToString();
            var result = new MultiSpindleParameterVueModel
            {
                OreLavoroTotali = par.FirstOrDefault(p => p.Cluster == cluster.ToString() && p.Keyword.Equals(varOreTot)),
                RpmRange1500 = par.FirstOrDefault(p => p.Cluster == cluster.ToString() && p.Keyword.EndsWith("_1")),
                RpmRange3999 = par.FirstOrDefault(p => p.Cluster == cluster.ToString() && p.Keyword.Contains("_2")),
                RpmRange7999 = par.FirstOrDefault(p => p.Cluster == cluster.ToString() && p.Keyword.Contains("_3")),
                RpmRange11500 = par.FirstOrDefault(p => p.Cluster == cluster.ToString() && p.Keyword.Contains("_4")),
                RpmRange14500 = par.FirstOrDefault(p => p.Cluster == cluster.ToString() && p.Keyword.Contains("_5")),
                RpmRange20000 = par.FirstOrDefault(p => p.Cluster == cluster.ToString() && p.Keyword.Contains("_6")),
                Posizione = cluster
            };
            return result;
        }


        private AxesLmxParameterVueModel GetLinearAxesLmxVueModel(MachineInfoModel machine)
        {
            var par = _parameterMachineService.GetParameters(machine, (int) enPanel.LinearAxesLMX);

            var result = new AxesLmxParameterVueModel
            {
                AxesLmx = par.OrderBy(n => n.VarNumber).ToList()
            };


            return result;
        }


        private MotorKeopeParameterVueModel GetVueModelKeope(MachineInfoModel machine)
        {
            var par = _parameterMachineService.GetParameters(machine, (int) enPanel.KeopeMotors);

            var result = new MotorKeopeParameterVueModel
            {
                fixedHead = par.Where(p => p.VarNumber == 428 || p.VarNumber == 432).OrderBy(n => n.VarNumber).ToList(),

                mobileHead = par.Where(p => p.VarNumber == 430 || p.VarNumber == 434).OrderBy(n => n.VarNumber).ToList()
            };

            foreach (var motFh in result.fixedHead)
                motFh.Value = double.IsNaN(double.Parse(motFh.Value)) ? "" : $"{double.Parse(motFh.Value):#,0}";

            foreach (var motMh in result.mobileHead)
                motMh.Value = double.IsNaN(double.Parse(motMh.Value)) ? "" : $"{double.Parse(motMh.Value):#,0}";


            return result;
        }

        private AxesKeopeParameterVueModel GetAxesVueModelKeope(MachineInfoModel machine)
        {
            var par = _parameterMachineService.GetParameters(machine, (int) enPanel.KeopeAxes);

            var result = new AxesKeopeParameterVueModel
            {
                axes = par.Where(p =>
                    p.VarNumber == 450 || p.VarNumber == 452 || p.VarNumber == 454 || p.VarNumber == 456 ||
                    p.VarNumber == 458).OrderBy(n => n.VarNumber).ToList()
            };

            foreach (var ax in result.axes)
                ax.Value = double.IsNaN(double.Parse(ax.Value)) ? "" : ax.ConvertedDistanceValue();

            return result;
        }

        private MotorAxesParameterVueModel GetVueModelBlitz(MachineInfoModel machine)
        {
            var par = _parameterMachineService.GetParameters(machine, (int) enPanel.BlitzMotorAxes);

            var result = new MotorAxesParameterVueModel
            {
                motors = par
                    .Where(p => p.VarNumber == 428 || p.VarNumber == 430 || p.VarNumber == 432 || p.VarNumber == 434)
                    .OrderBy(n => n.VarNumber).ToList(),

                axes = par.Where(p => p.VarNumber == 450 || p.VarNumber == 452 || p.VarNumber == 454)
                    .OrderBy(n => n.VarNumber).ToList()
            };

            //per questi parametri che sono interi elimino o decimali
            foreach (var mot in result.motors)
                mot.Value = double.IsNaN(double.Parse(mot.Value)) ? "" : double.Parse(mot.Value).ToString("0");

            foreach (var ax in result.axes)
                ax.Value = double.IsNaN(double.Parse(ax.Value)) ? "" : ax.ConvertedDistanceValue();


            return result;
        }
    }
}