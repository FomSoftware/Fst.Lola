using FomMonitoringCore.DAL;
using FomMonitoringCore.Framework.Model;
using FomMonitoringCore.Repository;
using FomMonitoringResources;
using Mapster;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FomMonitoringCore.Service
{
    public class ParameterMachineService : IParameterMachineService
    {
        private readonly IParameterMachineValueRepository _parameterMachineValueRepository;
        private readonly IParameterMachineRepository _parameterMachineRepository;

        public ParameterMachineService(IParameterMachineValueRepository parameterMachineValueRepository, IParameterMachineRepository parameterMachineRepository)
        {
            _parameterMachineValueRepository = parameterMachineValueRepository;
            _parameterMachineRepository = parameterMachineRepository;
        }

        public List<ParameterMachineValueModel> GetParameters(MachineInfoModel machine, int idPanel, int? idCluster = null)
        {
            var result = new List<ParameterMachineValueModel>();

            //var parametersValues = _parameterMachineValueRepository.Get(p => p.MachineId == machine.Id, tracked: false).GroupBy(g => g.VarNumber);
            var parametersValues = _parameterMachineValueRepository.GetByParameters(machine.Id, idPanel, idCluster).GroupBy(g => g.VarNumber);

            var varNums = new Dictionary<string, ParameterMachineValueModel>();
            foreach(var i in parametersValues)
            {
                var dateMax = i.Max(d => d.UtcDateTime);
                var parameter = i.FirstOrDefault(d => d.UtcDateTime == dateMax);
                //result.Add(parameter.BuildAdapter().AdaptToType<ParameterMachineValueModel>());
                varNums.Add(parameter.VarNumber, parameter.BuildAdapter().AdaptToType<ParameterMachineValueModel>());
            }
            //se ci sono dati cerco tutti i parametri di quel modello per avere almeno le descrizioni
            if(varNums.Any())
            {
                var parametri = _parameterMachineRepository.Get(pp => pp.MachineModelId == machine.MachineModelId
                                                    && pp.PanelId != null && pp.PanelId == idPanel, tracked: false).ToList();

                if (idCluster != null)
                {
                    parametri = parametri.Where(pp => pp.Cluster == idCluster.ToString()).OrderBy(pp => pp.VarNumber).ToList();
                }
                else
                {
                    parametri = parametri.OrderBy(pp => pp.VarNumber).ToList();
                }

                foreach(var pm in parametri)
                {
                    if (varNums.ContainsKey(pm.VarNumber))
                    {
                        result.Add(varNums[pm.VarNumber]);
                    }
                    else
                    {
                        var ve = new ParameterMachineValueModel
                        {
                            VarNumber = int.Parse(pm.VarNumber),
                            Description = new System.Resources.ResourceManager(typeof(Resource)).GetString(pm.Keyword),
                            Value = pm.DefaultValue,
                            Cluster = pm.Cluster,
                            Keyword = pm.Keyword,
                            CnUm = pm.CnUm,
                            HmiUm = pm.HmiUm
                            
                        };
                        result.Add(ve);
                    }
                }
                    
            }



            

            return result;
        }
    }


}
