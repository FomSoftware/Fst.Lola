﻿
using FomMonitoringCore.Framework.Common;
using FomMonitoringCore.Framework.Model;
using System;
using System.Collections.Generic;
using FomMonitoringCore.SqlServer;

namespace FomMonitoringBLL.ViewModel
{

    public class HeaderViewModel
    {
        public string ControllerPage { get; set; }

        public string ActionPage { get; set; }

        public PeriodModel ActualPeriod { get; set; }

        public UserModel User { get; set; }

        public List<Languages> AllLanguages { get; set; }

        public Languages ActualLanguage { get; set; }

        public List<Faq> Faqs { get; set; }
        public string CompanyName { get; set; }
        public enPage ActualPage { get; set; }
        public DataUpdateModel MinStateDate { get; set; }
        public DataUpdateModel MaxStateDate { get; set; }
    }

    public class ToolbarViewModel
    {
        public enPage page { get; set; }

        public enRole role { get; set; }

        public List<PlantInfoViewModel> plants { get; set; }

        public PlantInfoViewModel selected_plant { get; set; }

        public List<MachineInfoViewModel> machines { get; set; }

        public MachineInfoViewModel selected_machine { get; set; }

        public PeriodViewModel period { get; set; }

        public LanguageViewModel language { get; set; }
    }


    public class PlantInfoViewModel
    {
        public int id { get; set; }

        public string name { get; set; }
    }


    public class MachineInfoViewModel
    {

        public int id { get; set; }

        public string serial { get; set; }

        public string icon { get; set; }

        public int id_mtype { get; set; }

        public string mtype { get; set; }

        public string model { get; set; }

        public string description { get; set; }

        public string product_type { get; set; }

        public string product_version { get; set; }

        public string installation { get; set; }

        public bool expired { get; set; }

        public string machineName { get; set; }

        public string activation { get; set; }

        public string timeZone { get; set; }

        public int modelCode { get; set; }

    }


    public class PeriodViewModel
    {
        public DateTime start { get; set; }

        public DateTime end { get; set; }
    }


    public class LanguageViewModel
    {
        public string initial { get; set; }

        public CalendarI18nModel labels { get; set; }
    }

    public class LoadConfigurationModel
    {
        public bool? success { get; set; }
        public string errors { get; set; }
    }
}
