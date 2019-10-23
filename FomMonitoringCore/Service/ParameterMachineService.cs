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
        private IParameterMachineValueRepository _parameterMachineValueRepository;
        private IParameterMachineRepository _parameterMachineRepository;

        public ParameterMachineService(IParameterMachineValueRepository parameterMachineValueRepository, IParameterMachineRepository parameterMachineRepository)
        {
            _parameterMachineValueRepository = parameterMachineValueRepository;
            _parameterMachineRepository = parameterMachineRepository;
        }

        public List<ParameterMachineValueModel> GetParameters(MachineInfoModel machine, int idPanel)
        {
            var result = new List<ParameterMachineValueModel>();

            var parametersValues = _parameterMachineValueRepository.Get(p => p.MachineId == machine.Id, tracked: false).GroupBy(g => g.VarNumber);

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
                                                    && pp.PanelId != null && pp.PanelId == idPanel, tracked: false).OrderBy(pp => pp.VarNumber).ToList();
                    
                foreach(var pm in parametri)
                {
                    if (varNums.ContainsKey(pm.VarNumber))
                    {
                        result.Add(varNums[pm.VarNumber]);
                    }
                    else
                    {
                        var valoreVuoto = new ParameterMachineValueModel()
                        {
                            VarNumber = int.Parse(pm.VarNumber),
                            Description = new System.Resources.ResourceManager(typeof(Resource)).GetString(pm.Keyword),
                            Value = pm.DefaultValue
                        };
                        result.Add(valoreVuoto);
                    }
                }
                    
            }



            

            return result;
        }
    }


}
