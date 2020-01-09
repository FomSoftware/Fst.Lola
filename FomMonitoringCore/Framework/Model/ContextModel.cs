﻿using FomMonitoringCore.Framework.Common;
using System.Collections.Generic;
using UserManager.DAL;

namespace FomMonitoringCore.Framework.Model
{
    public class ContextModel
    {
        public UserModel User { get; set; }

        public enPage ActualPage { get; set; }

        public List<Languages> AllLanguages { get; set; }

        public Languages ActualLanguage { get; set; }

        public List<PlantModel> AllPlants { get; set; }

        public PlantModel ActualPlant { get; set; }

        public List<MachineInfoModel> AllMachines { get; set; }

        public MachineInfoModel ActualMachine { get; set; }

        public PeriodModel ActualPeriod { get; set; }

        public string ActualMachineGroup { get; set; }
    }
}
