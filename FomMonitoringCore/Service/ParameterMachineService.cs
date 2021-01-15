using FomMonitoringCore.Framework.Model;
using Mapster;
using System.Collections.Generic;
using System.Linq;
using FomMonitoringCore.SqlServer.Repository;

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
            var parametersValues = _parameterMachineValueRepository.GetByParameters(machine.Id, idPanel, idCluster);

            var varNums = new Dictionary<string, ParameterMachineValueModel>();
            foreach(var i in parametersValues)
            {
                varNums.Add(i.VarNumber, i.BuildAdapter().AdaptToType<ParameterMachineValueModel>());
            }

            //se ci sono dati cerco tutti i parametri di quel modello per avere almeno le descrizioni
            //if(varNums.Any())
            {
                var parametri = _parameterMachineRepository.GetByParameters(machine.MachineModelId ?? 0).Where(p => p.PanelId == idPanel && (idCluster == null || p.Cluster == idCluster.ToString())).OrderBy(pp => pp.VarNumber).ToList();


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
                            Description = LocalizationService.GetResource(pm.Keyword),
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
