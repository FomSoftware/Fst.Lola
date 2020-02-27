using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Linq;

namespace FomMonitoringCore.DAL
{
    public partial class FST_FomMonitoringEntities : IFomMonitoringEntities
    {
        public void Refresh()
        {
            var refreshableObjects = ChangeTracker.Entries().Select(c => c.Entity).ToList();
            ((IObjectContextAdapter)this).ObjectContext.Refresh(RefreshMode.StoreWins, refreshableObjects);
        }
    }
}