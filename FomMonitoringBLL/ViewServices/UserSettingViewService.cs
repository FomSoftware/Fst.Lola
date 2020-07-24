using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FomMonitoringCore.Service;


namespace FomMonitoringBLL.ViewServices
{

    public interface IUserSettingViewService
    {
        void SetOverrideTimeZoneMachine(int idMachine, Guid idUser, string timezone);
        void DeleteOverrideTimeZoneMachine(int idMachine, Guid idUser);
    }

    public class UserSettingViewService: IUserSettingViewService
    {
        private readonly ITimeZoneService _timeZoneService;

        public UserSettingViewService(ITimeZoneService timeZoneService)
        {
            _timeZoneService = timeZoneService;
        }


        public void SetOverrideTimeZoneMachine(int idMachine, Guid idUser, string timezone)
        {
            _timeZoneService.SetOverrideTimeZoneMachine(idMachine, idUser, timezone);
        }

        public void DeleteOverrideTimeZoneMachine(int idMachine, Guid idUser)
        {
            _timeZoneService.DeleteOverrideTimeZoneMachine(idMachine, idUser);
        }
    }
}
