using System.Collections.Generic;
using System.Linq;
using FomMonitoringCore.DAL;

namespace FomMonitoringCore.Repository.SQL
{
    public class ParameterMachineRepository : GenericRepository<ParameterMachine>, IParameterMachineRepository
    {
        public ParameterMachineRepository(IFomMonitoringEntities context) : base(context)
        {

        }

        public IEnumerable<ParameterMachine> GetByParameters(int machineModelId, int? idPanel = null, int? idCluster = null, bool tracked = true)
        {
            IQueryable<ParameterMachine> query = !tracked ? dbSet.AsNoTracking() : dbSet; 

            query = query.Where(m => m.MachineModelId == machineModelId);

            if (idPanel != null)
            {
                query = query.Where(p => p.PanelId == idPanel);
            }

            if (idCluster != null)
            {
                query = query.Where(p => p.Cluster == idCluster.ToString());
            }

            return query.ToList();
        }
    }

}
