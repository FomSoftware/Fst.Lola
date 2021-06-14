using System;
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

                //elimino i record che non esistono più
                var curparams = _context.Set<ParameterMachine>().Where(p => p.ModelCode == m.ModelCodeV997.ToString()).ToList();
                var toDelete = curparams.Where(c => !list.Any(a => a.VarNumber == c.VarNumber.ToString())).ToList();
                if (toDelete.Count > 0)
                {
                    var ids = toDelete.Select(a => a.Id).ToList();
                    var values = _context.Set<ParameterMachineValue>().Where(v => ids.Contains(v.ParameterMachineId)).ToList();
                    if(values.Count > 0)
                        _context.Set<ParameterMachineValue>().RemoveRange(values);
                    var resets = _context.Set<ParameterResetValue>().Where(v => ids.Contains(v.ParameterMachineId)).ToList();
                    if (resets.Count > 0)
                        _context.Set<ParameterResetValue>().RemoveRange(resets);
                    var thresholds = _context.Set<ParameterMachineThreshold>().Where(v => ids.Contains(v.ParameterMachineId)).ToList();
                    if (thresholds.Count > 0)
                        _context.Set<ParameterMachineThreshold>().RemoveRange(thresholds);

                    _context.Set<ParameterMachine>().RemoveRange(toDelete);
                }
            }

            await _context.SaveChangesAsync();
            
        }


        public bool CheckMachineModelCode(int ModelCodeV997)
        {
            var machineModel = _context.Set<MachineModel>().FirstOrDefault(mac => mac.ModelCodev997 == ModelCodeV997);
            if (machineModel == null)
            {
                return false;
            }
            return true;
        }

        public bool CheckVarNumber(int ModelCodeV997, string varnumber)
        {
            //controllo solo se esiste il model code, permetto di inserire nuove variabili che non esistono
            return ModelCodeV997 > 0;
            /*return _context.Set<ParameterMachine>()
                .Any(p => p.ModelCode == ModelCodeV997.ToString() && p.VarNumber == varnumber);*/
        }

        public bool CheckPanelId(int? panelId)
        {
            //se non c'è il panel lo inserisco null, non sarà visualizzato
            if (panelId == null) return true;
            return _context.Set<Panel>().Find(panelId) != null;
        }

        public bool CheckMachineGroup(string machineGroup)
        {
            if (string.IsNullOrWhiteSpace(machineGroup)) return false;
            return _context.Set<MachineGroup>().Find(Int32.Parse(machineGroup)) != null;
        }
    }
}
