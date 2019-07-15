using FomMonitoringCore.Framework.Model;
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

                foreach(var i in parametersValues)
                {
                    var dateMax = i.Max(d => d.UtcDateTime);
                    var parameter = i.FirstOrDefault(d => d.UtcDateTime == dateMax);
                    result.Add(parameter.BuildAdapter().AdaptToType<ParameterMachineValueModel>());
                }

            }

            return result;
        }
    }


}
