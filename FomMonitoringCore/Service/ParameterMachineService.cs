using FomMonitoringCore.DAL;
using FomMonitoringCore.Framework.Model;
using FomMonitoringResources;
using Mapster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FomMonitoringCore.Service
{
    public class ParameterMachineService
    {
        public static List<ParameterMachineValueModel> GetParameters(MachineInfoModel machine, int idPanel)
        {
            var result = new List<ParameterMachineValueModel>();
            using (var ctx = new DAL.FST_FomMonitoringEntities())
            {
                var parametersValues = ctx.ParameterMachineValue.Where(p => p.MachineId == machine.Id).GroupBy(g => g.VarNumber);

                Dictionary <string, ParameterMachineValueModel> varNums = new Dictionary<string, ParameterMachineValueModel>();
                foreach(var i in parametersValues)
                {
                    var dateMax = i.Max(d => d.UtcDateTime);
                    var parameter = i.FirstOrDefault(d => d.UtcDateTime == dateMax);
                    //result.Add(parameter.BuildAdapter().AdaptToType<ParameterMachineValueModel>());
                    varNums.Add(parameter.VarNumber, parameter.BuildAdapter().AdaptToType<ParameterMachineValueModel>());
                }

                if(varNums != null && varNums.Count > 0)
                {
                    List<ParameterMachine> parametri = ctx.ParameterMachine.Where(pp => pp.MachineModelId == machine.MachineModelId).OrderBy(pp => pp.VarNumber).ToList();
                                                //da aggiungere quando si implementa anagrafica dei pannelli
                                                //&& pp.Panel != null && Int32.Parse(pp.Panel) == idPanel);
                    foreach(ParameterMachine pm in parametri)
                    {
                        if (varNums.ContainsKey(pm.VarNumber))
                        {
                            result.Add(varNums[pm.VarNumber]);
                        }
                        else
                        {
                            ParameterMachineValueModel vuoto = new ParameterMachineValueModel()
                            {
                                VarNumber = Int32.Parse(pm.VarNumber),
                                Description = new System.Resources.ResourceManager(typeof(Resource)).GetString(pm.Keyword)
                            };
                            result.Add(vuoto);
                        }
                    }
                    
                }



            }

            return result;
        }
    }


}
