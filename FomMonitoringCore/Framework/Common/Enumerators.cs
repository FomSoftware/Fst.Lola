using System.ComponentModel;

namespace FomMonitoringCore.Framework.Common
{
    public enum enRole
    {
        [Description("Administrator")]
        Administrator = 0,
        [Description("Operator")]
        Operator = 1,
        [Description("HeadWorkshop")]
        HeadWorkshop = 2,
        [Description("FOMService")]
        Assistance = 3,
        [Description("Customer")]
        Customer = 4,
        [Description("UserApi")]
        UserApi = 5,
        [Description("RandD")]
        RandD = 6,
        [Description("Demo")]
        Demo = 9,
        [Description("FOMBranchService")]
        FOMBranchService = 7,
        [Description("FOMDealers")]
        FOMDealers = 8
    }

    public enum enPage
    {
        [Description(""), Controller("UserManager")]
        UserManager = 1,
        [Description("Index"), Controller("Machine")]
        Machine = 2,
        [Description("Index"), Controller("Mes")]
        Mes = 3,
        [Description("PlantMessages"), Controller("Mes")]
        PlantMessages = 4,
        [Description(""), Controller("PlantManager")]
        PlantManager = 5,
        [Description("Index"), Controller("UserSetting")]
        UserSetting = 6,
        [Description("Index"), Controller("UploadConfiguration")]
        UploadConfiguration = 7,
        [Description("Index"), Controller("Assistance")]
        Assistance = 8
    }

    

    public enum enState
    {
        [Description("off")]
        Offline = 0,
        [Description("prod")]
        Production = 1,
        [Description("pause")]
        Pause = 2,
        [Description("error")]
        Error = 3,
        [Description("manual")]
        Manual = 4
    }
    
    public enum enPanel
    {       
        [Description("toolsBlitz")]
        ToolsBlitz = 1,
        [Description("xtools")]
        XTools = 2,
        [Description("multispindle")]
        Multispindle = 3,
        [Description("xspindles")]
        XSpindles = 4,
        [Description("productivity")]
        Productivity = 5,
        [Description("messages")]
        Messages = 6,
        [Description("orders")]
        Orders = 7,
        [Description("maintenance")]
        Maintance = 8,
        [Description("efficiency")]
        Efficiency = 9,
        [Description("blitzMotorAxes")]
        BlitzMotorAxes = 25,
        [Description("keopeAxes")]
        KeopeAxes = 26,
        [Description("keopeMotors")]
        KeopeMotors = 27,
        [Description("otherMachineDataLmx")]
        OtherMachineDataLmx = 28,
        [Description("otherMachineData")]
        OtherMachineData = 29,
        [Description("electrospindle")]
        Electrospindle = 31,
        [Description("toolsFmcLmx")]
        ToolsFmcLmx = 32,
        [Description("xmuToolsLmx")]
        XmuToolsLmx = 34,
        [Description("mmToolsLmx")]
        MmToolsLmx = 35,
        [Description("productionFmc34")]
        ProductionFmc34 = 36,
        [Description("ordersFmc34")]
        OrdersFmc34 = 37,
       [Description("productionLMX")]
        ProductionLMX = 38,
        [Description("rotaryAxesLMX")]
        RotaryAxesLMX = 39,
        [Description("xmuSp_SensorsLMX")]
        XMUSp_SensorsLMX = 40,
        [Description("motorBladeLMX")]
        MotorBladeLMX = 41,
        [Description("linearAxesLMX")]
        LinearAxesLMX = 42,
        [Description("tiltingMSAxesLMX")]
        TiltingMSAxesLMX = 43,
        [Description("AXEL_Spindle")]
        AXEL_Spindle = 44,
        [Description("AXEL_ToolW")]
        AXEL_ToolW = 45,
        [Description("AXEL_Sp_Sensors")]
        AXEL_Sp_Sensors = 46,
        [Description("AXEL5_Axes")]
        AXEL5_Axes = 47,
        [Description("AXEL5_MachineData")]
        AXEL5_MachineData = 48

    }

    public enum enDataType
    {
        Dashboard = 0,
        Historical = 1,
        Operators = 2,
        Shifts = 3,
        Summary = 4
    }

    public enum enAggregation
    {
        [Description("d")]
        Day = 0,
        [Description("w")]
        Week = 1,
        [Description("m")]
        Month = 2,
        [Description("q")]
        Quarter = 3,
        [Description("y")]
        Year = 4
    }

    public enum enTypeAlarm
    {
        [Description("operator")]
        Operator = 2,        
        [Description("error")]
        Error = 11,
        [Description("warning")]
        Interfaccia = 8,
        [Description("warning")]
        Popup = 9,
        [Description("operator")]
        Indicazioni = 10,

        [Description("warning")]
        Warning = 12,
        [Description("operator")]
        CN = 13,
        [Description("warning")]
        PeriodicMaintenance = 14

    }


    public enum enSorting
    {
        [Description("asc")]
        Ascending = 0,
        [Description("desc")]
        Descending = 1
    }

    public enum enMeasurement
    {
        [Description("s")]
        Seconds = 0,
        [Description("min")]
        Minutes = 1,
        [Description("h")]
        Hours = 2,
        [Description("d")]
        Days = 3
    }

    public enum enSerieProd
    {
        Efficiency = 0,
        GrossTime = 1,
        NetTime = 2
    }

    public enum enToolType
    {
        [Description("bg-red")]
        Breaking = 1,
        [Description("bg-orange")]
        Replacement = 2
    }

    public enum enCheckParam
    {
        Machine = 1,
        Plant = 2
    }

    public enum enLoginResult
    {
        [Description("OK")]
        Ok,
        [Description("NOT EXISTS")]
        NotExists,
        [Description("DISABLED")]
        Disabled,
        [Description("WRONG PASSWORD")]
        WrongPassword
    }

    // I valori storicizzati di default sono non visibili mentre quelli visibili sono sempre storicizzati
    public enum enMessageScope
    {
        Ignore = 0,
        Historicised = 1,
        Visible = 2
    }

    public enum enMachineType
    {
        [Description("Centro di lavoro")]
        CentroLavoro = 1,
        [Description("Linea di taglio")]
        LineaTaglio = 2,
        [Description("Linea taglio e lavoro")]
        LineaTaglioLavoro = 3,
        [Description("Troncatrice")]
        Troncatrice = 4
    }

    public enum enUnitaMisuraLength
    {
        //lunghezze
        [Description("KM")]
        KM = 1,
        [Description("MT")]
        MT = 1000,
        [Description("MM")]
        MM = 1000000
    }
    public enum enUnitaMisuraTime
    {
        //timespan
        [Description("MS")]
        MS = 3600000,
        [Description("S")]
        S = 3600,
        [Description("M")]
        M = 60,
        [Description("H")]
        H = 1  
    }

    public enum enMachineGroup
    {
        [Description("Machine")]
        Machine = 6
    }

}
