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
        [Description("Assistance")]
        Assistance = 3,
        [Description("Customer")]
        Customer = 4,
        [Description("UserApi")]
        UserApi = 5
    }

    public enum enPage
    {
        [Description("")]
        UserManager = 1,
        [Description("Index")]
        Machine = 2,
        [Description("Index")]
        Mes = 3
    }

    public enum enState
    {
        [Description("off")]
        Off = 0,
        [Description("prod")]
        Production = 1,
        [Description("pause")]
        Pause = 2,
        [Description("error")]
        Error = 3,
        [Description("manual")]
        Manual = 4
    }

    public enum enDataType
    {
        Dashboard = 0,
        Historical = 1,
        Operators = 2,
        Shifts = 3
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
        Error = 3
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

}
