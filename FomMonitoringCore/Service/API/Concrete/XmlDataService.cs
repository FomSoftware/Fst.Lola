using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Threading.Tasks;
using FomMonitoringCore.Framework.Model.Xml;
using FomMonitoringCore.SqlServer;
using Mapster;

namespace FomMonitoringCore.Service.API.Concrete
{
    public class XmlDataService : IXmlDataService
    {
        private readonly IFomMonitoringEntities _context;

        public XmlDataService(IFomMonitoringEntities context)
        {
            _context = context;
        }
        
        public async Task AddOrUpdateMachineParameterAsync(ParametersMachineModelXml m)
        {
            var machineModel = _context.Set<MachineModel>().FirstOrDefault(mac => mac.ModelCodev997 == m.ModelCodeV997);
            if (machineModel != null)
            {
                var list = m.Parameters.Parameter.BuildAdapter().AddParameters("idMachineModel", machineModel.Id).AddParameters("modelCode", m.ModelCodeV997).AdaptToType<List<ParameterMachine>>();

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
