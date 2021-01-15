using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace FomMonitoringCore.SqlServer.Repository
{
    public class ParameterMachineValueRepository : GenericRepository<ParameterMachineValue>, IParameterMachineValueRepository
    {
        public ParameterMachineValueRepository(IFomMonitoringEntities context) : base(context)
        {

        }

        public IEnumerable<ParameterMachineValue> GetByParameters(int idMachine, int? idPanel = null, int? idCluster = null)
        {
            var query = dbSet.Where(m => m.MachineId == idMachine);
            if (idPanel != null)
            {
                query = query.Where(p => p.ParameterMachine.PanelId == idPanel);
            }

            if (idCluster != null)
            {
                query = query.Where(p => p.ParameterMachine.Cluster == idCluster.ToString());
            }

            query = query.GroupBy(p => p.VarNumber).Select(t => t.OrderByDescending(i => i.UtcDateTime).FirstOrDefault()).Include("ParameterMachine");

            return query.ToList();
        }
    }

}
