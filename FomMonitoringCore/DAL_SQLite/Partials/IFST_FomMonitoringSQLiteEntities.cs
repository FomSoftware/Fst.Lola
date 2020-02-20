using System.Data.Entity;
using FomMonitoringCore.DAL;

namespace FomMonitoringCore.DAL_SQLite
{
    public interface IFomMonitoringSQLiteEntities : IDbContext
    {
    }

    public partial class FST_FomMonitoringSQLiteEntities : DbContext, IFomMonitoringSQLiteEntities
    {
        public void Refresh()
        {
            throw new System.NotImplementedException();
        }
    }
}
