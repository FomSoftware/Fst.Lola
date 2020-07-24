using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using FomMonitoring.RequestDto;
using FomMonitoringBLL.ViewModel;
using FomMonitoringBLL.ViewServices;
using FomMonitoringCore.Framework.Common;
using FomMonitoringCore.Service;
using TimeZoneNames;
using UserManager.Service;

namespace FomMonitoring.Controllers
{
    [SessionApi]
    public class UserSettingApiController : ApiController
    {
        private readonly IContextService _contextService;
        private readonly IUserSettingViewService _userSettingViewService;
        private readonly IUserManagerViewService _userManagerViewService;
        private readonly IMachineService _machineService;

        public UserSettingApiController(IContextService contextService, IUserSettingViewService userSettingViewService, IUserManagerViewService userManagerViewService, IMachineService machineService)
        {
            _contextService = contextService;
            _userSettingViewService = userSettingViewService;
            _userManagerViewService = userManagerViewService;
            _machineService = machineService;
        }

        // GET: UserSettingApi
        [System.Web.Http.HttpGet]
        [System.Web.Http.Authorize]
        [System.Web.Http.Route("ajax/UserSettingApi/GetMachinesWithException")]
        public HttpResponseMessage GetMachinesWithException()
        {
            var context = _contextService.GetContext();
            var machine = _machineService.GetUserMachines(context);
            return Request.CreateResponse(HttpStatusCode.OK, machine.Where(m => m.TimeZone != null).Select(m => new
            {
                m.Id,
                m.Description,
                m.Serial,
                m.TimeZone,
                m.MachineName
            }).ToList(), MediaTypeHeaderValue.Parse("application/json"));
        } 

        // GET: UserSettingApi
        [System.Web.Http.HttpGet]
        [System.Web.Http.Authorize]
        [System.Web.Http.Route("ajax/UserSettingApi/GetMachinesWithoutException")]
        public HttpResponseMessage GetMachinesWithoutException()
        {
            var context = _contextService.GetContext();
            var machine = _machineService.GetUserMachines(context);
            return Request.CreateResponse(HttpStatusCode.OK, machine.Where(m => m.TimeZone == null).Select(m => new
            {
                m.Id,
                m.Description,
                m.Serial,
                m.TimeZone,
                m.MachineName
            }).ToList(), MediaTypeHeaderValue.Parse("application/json"));
        }

        [System.Web.Http.HttpGet]
        [System.Web.Http.Authorize]
        [System.Web.Http.Route("ajax/UserSettingApi/GetCurrentTimeZone")]
        public HttpResponseMessage GetCurrentTimeZone()
        {
            var context = _contextService.GetContext();

            return Request.CreateResponse(HttpStatusCode.OK, context.User.TimeZone, MediaTypeHeaderValue.Parse("application/json"));
        }


        [System.Web.Http.HttpPost]
        [System.Web.Http.Authorize(Roles = Common.Customer)]
        [System.Web.Http.Route("ajax/UserSettingApi/EditException")]
        public HttpResponseMessage EditException([FromBody] MachineTimezoneRequestDto timezoneSetting)
        {
            try
            {
                var context = _contextService.GetContext();
                _userSettingViewService.SetOverrideTimeZoneMachine(timezoneSetting.IdMachine, context.User.ID, timezoneSetting.TimeZone);
                return Request.CreateResponse(HttpStatusCode.OK, new {}, MediaTypeHeaderValue.Parse("application/json"));
            }
            catch (System.InvalidOperationException ex)
            {
                return Request.CreateResponse(HttpStatusCode.Conflict, ex, MediaTypeHeaderValue.Parse("application/json"));
            }
        }

        [System.Web.Http.HttpPost]
        [System.Web.Http.Authorize(Roles = Common.Customer)]
        [System.Web.Http.Route("ajax/UserSettingApi/DeleteException")]
        public HttpResponseMessage DeleteException([FromBody] MachineTimezoneRequestDto timezoneSetting)
        {
            try
            {
                var context = _contextService.GetContext();
                _userSettingViewService.DeleteOverrideTimeZoneMachine(timezoneSetting.IdMachine, context.User.ID);
                return Request.CreateResponse(HttpStatusCode.OK, new {}, MediaTypeHeaderValue.Parse("application/json"));
            }
            catch (System.InvalidOperationException ex)
            {
                return Request.CreateResponse(HttpStatusCode.Conflict, ex, MediaTypeHeaderValue.Parse("application/json"));
            }
        }

        [System.Web.Http.HttpGet]
        [System.Web.Http.Authorize]
        [System.Web.Http.Route("ajax/UserSettingApi/GetTimeZones")]
        public HttpResponseMessage GetTimeZones()
        {
            var languageCode = CultureInfo.CurrentUICulture.Name;


            return Request.CreateResponse(HttpStatusCode.OK, TZNames.GetCountryNames(languageCode)
                .SelectMany(x => GetTimeZonesForCountry(x.Key, DateTimeOffset.UtcNow, languageCode)
                    .Select(y => new { CountryCode = x.Key, Country = x.Value, TimeZoneId = y.Key, TimeZoneName = y.Value }))
                .GroupBy(x => x.TimeZoneId).Select(n => new {Code = n.Key, Description = $"{n.First().Country} - {n.First().TimeZoneName}" }).ToList(), MediaTypeHeaderValue.Parse("application/json"));
        }

        [System.Web.Http.HttpPost]
        [System.Web.Http.Authorize]
        [System.Web.Http.Route("ajax/UserSettingApi/ChangeTimeZone")]
        public HttpResponseMessage ChangeTimeZone([FromBody] string timezone)
        {
            var context = _contextService.GetContext();
            _userManagerViewService.ChangeTimeZone(context, timezone);
            _contextService.SetActualTimeZone(timezone);
            return Request.CreateResponse(HttpStatusCode.OK, new { }, MediaTypeHeaderValue.Parse("application/json"));
        }

        private static IDictionary<string, string> GetTimeZonesForCountry(string country, DateTimeOffset? threshold, string languageCode)
        {
            return threshold == null
                ? TZNames.GetTimeZonesForCountry(country, languageCode)
                : TZNames.GetTimeZonesForCountry(country, languageCode, threshold.Value);
        }

    }
}