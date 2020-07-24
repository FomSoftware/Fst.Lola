using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FomMonitoringCore.SqlServer;

namespace FomMonitoringCore.Service
{

    public interface ITimeZoneService
    {
        void SetOverrideTimeZoneMachine(int idMachine, Guid idUser, string timezone);
        void DeleteOverrideTimeZoneMachine(int idMachine, Guid idUser);
    }

    public class TimeZoneService : ITimeZoneService
    {
        private readonly IFomMonitoringEntities _fomMonitoringEntities;

        public TimeZoneService(IFomMonitoringEntities fomMonitoringEntities)
        {
            _fomMonitoringEntities = fomMonitoringEntities;
        }

        public void SetOverrideTimeZoneMachine(int idMachine, Guid idUser, string timezone)
        {
            var mapping = _fomMonitoringEntities.Set<UserMachineMapping>()
                .FirstOrDefault(m => m.MachineId == idMachine && m.UserId == idUser);
                if(mapping != null)
                    mapping.TimeZone = timezone;

                _fomMonitoringEntities.SaveChanges();
        }


        public void DeleteOverrideTimeZoneMachine(int idMachine, Guid idUser)
        {
            var mapping = _fomMonitoringEntities.Set<UserMachineMapping>()
                .FirstOrDefault(m => m.MachineId == idMachine && m.UserId == idUser);
            if (mapping != null)
                mapping.TimeZone = null;
            _fomMonitoringEntities.SaveChanges();
        }
    }
}
