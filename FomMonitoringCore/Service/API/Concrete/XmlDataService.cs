using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FomMonitoringCore.DAL;
using FomMonitoringCore.Framework.Model;
using Mapster;

namespace FomMonitoringCore.Service.API.Concrete
{
    public class XmlDataService : IXmlDataService
    {
        public void AddOrUpdateMachineParameter(ParametersMachineModel m)
        {
            var list = m.Parameters.Parameter.BuildAdapter().AdaptToType<List<ParameterMachine>>();

            using(var db = new FST_FomMonitoringEntities())
            {
                var machine = db.Machine.FirstOrDefault(mac => mac.Serial == m.ModelCodeV997);
                if(machine != null)
                {
                    foreach (var i in list)
                    {
                        var old = db.ParameterMachine.FirstOrDefault(pm => pm.ModelCode == i.ModelCode && pm.VarNumber == i.VarNumber);
                        i.Id = old?.Id ?? 0;
                        i.MachineId = i.MachineId;
                        db.ParameterMachine.AddOrUpdate(i);
                    }
                }


                db.SaveChanges();
            }

        }
    }
}
