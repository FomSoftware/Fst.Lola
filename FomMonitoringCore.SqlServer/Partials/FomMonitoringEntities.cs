using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Linq;

namespace FomMonitoringCore.SqlServer
{
    public partial class FomMonitoringEntities : IFomMonitoringEntities
    {
        public void Refresh()
        {
            var refreshableObjects = ChangeTracker.Entries().Select(c => c.Entity).ToList();
            ((IObjectContextAdapter)this).ObjectContext.Refresh(RefreshMode.StoreWins, refreshableObjects);
        }
    }
}