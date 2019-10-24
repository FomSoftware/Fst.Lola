using FomMonitoringCore.DAL;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FomMonitoringCore.DAL_SQLite
{
    public interface IFomMonitoringSQLiteEntities : IDbContext
    {
    }

    public partial class FST_FomMonitoringSQLiteEntities : DbContext, IFomMonitoringSQLiteEntities
    {

    }
}
