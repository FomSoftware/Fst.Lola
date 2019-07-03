﻿using System;
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
        [Description(""), Controller("UserManager")]
        UserManager = 1,
        [Description("Index"), Controller("Machine")]
        Machine = 2,
        [Description("Index"), Controller("Mes")]
        Mes = 3,
        [Description("PlantMessages"), Controller("Mes")]
        PlantMessages = 4,
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

    public enum enPanel
    {
        [Description("efficiency")]
        Efficiency = 0,
        [Description("messages")]
        Messages = 1,
        [Description("productivity")]
        Productivity = 2,
        [Description("orders")]
        Orders = 3,
        [Description("maintance")]
        Maintance = 4,
        [Description("multispindle")]
        Multispindle = 5,
        [Description("tools")]
        Tools = 6
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
        Errori = 11,
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

}
