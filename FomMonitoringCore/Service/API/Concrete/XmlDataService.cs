﻿using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FomMonitoringCore.DAL;
using FomMonitoringCore.Framework.Model;
using FomMonitoringCore.Framework.Model.Xml;
using Mapster;

namespace FomMonitoringCore.Service.API.Concrete
{
    public class XmlDataService : IXmlDataService
    {
        public void AddOrUpdateMachineParameter(ParametersMachineModelXml m)
        {

                var list = m.Parameters.Parameter.BuildAdapter().AdaptToType<List<ParameterMachine>>();

                using (var db = new FST_FomMonitoringEntities())
                {
                    var machineModel = db.MachineModel.FirstOrDefault(mac => mac.ModelCodev997 == m.ModelCodeV997);
                    if (machineModel != null)
                    {
                        foreach (var i in list)
                        {
                            var old = db.ParameterMachine.FirstOrDefault(pm => pm.ModelCode == i.ModelCode && pm.VarNumber == i.VarNumber);
                            i.Id = old?.Id ?? 0;
                            i.MachineModelId = machineModel.Id;
                            db.ParameterMachine.AddOrUpdate(i);
                        }
                    }


                    db.SaveChanges();
                }


        }
    }
}