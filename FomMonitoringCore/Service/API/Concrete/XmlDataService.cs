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

        public XmlDataService(IFomMonitoringEntities context)
        {
            _context = context;
        }

        private IFomMonitoringEntities _context { get; }

        public async Task AddOrUpdateMachineParameterAsync(ParametersMachineModelXml m)
        {

            var panelsXml = m.Parameters.Parameter.Where(p => p.PANEL_ID > 0).Select(p => p.PANEL_ID).Distinct().ToList();
            
            var machineModel = _context.Set<MachineModel>().FirstOrDefault(mac => mac.ModelCodev997 == m.ModelCodeV997);
            var list = m.Parameters.Parameter.BuildAdapter().AddParameters("idMachineModel", machineModel.Id).AddParameters("modelCode", m.ModelCodeV997).AdaptToType<List<ParameterMachine>>();
            if (machineModel != null)
            {
                foreach (var i in list)
                {
                    var old = _context.Set<ParameterMachine>().FirstOrDefault(pm => pm.ModelCode == i.ModelCode && pm.VarNumber == i.VarNumber);
                    i.Id = old?.Id ?? 0;
                    i.MachineModelId = machineModel.Id;
                    _context.Set<ParameterMachine>().AddOrUpdate(i);
                }
            }

            await _context.SaveChangesAsync();
            
        }
    }
}
