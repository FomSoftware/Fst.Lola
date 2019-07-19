using System;
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

            var panels = m.Parameters.Parameter.Where(p => !string.IsNullOrWhiteSpace(p.PANEL)).Select(p => p.PANEL.Trim()).Distinct().ToList();
            
            using (var db = new FST_FomMonitoringEntities())
            {
                var machineModel = db.MachineModel.FirstOrDefault(mac => mac.ModelCodev997 == m.ModelCodeV997);
                var list = m.Parameters.Parameter.BuildAdapter().AddParameters("idMachineModel", machineModel.Id).AdaptToType<List<ParameterMachine>>();
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

                var panelModelToRemove = machineModel.Panel.Where(p => panels.Any(p2 => p2 == p.Name)).ToList();
                foreach(var r in panelModelToRemove)
                {
                    machineModel.Panel.Remove(r);
                }

                db.SaveChanges();

            }


        }
    }
}
