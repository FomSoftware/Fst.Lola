using System;
using FomMonitoringBLL.ViewModel;
using FomMonitoringBLL.ViewServices;
using FomMonitoringCore.Framework.Common;
using FomMonitoringCore.Framework.Model;
using FomMonitoringCore.Service;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using TimeZoneNames;
using UserManager.Service;
using UserManager.Service.Concrete;
using System.Linq;

namespace FomMonitoring.Controllers
{
    [SessionApi]
    
    public class UserManagerApiController : ApiController
    {
        private readonly IContextService _contextService;
        private readonly IUserManagerViewService _userManagerViewService;
        private readonly ILoginServices _loginServices;

        public UserManagerApiController(IContextService contextService, IUserManagerViewService userManagerViewService, ILoginServices loginServices)
        {
            _contextService = contextService;
            _userManagerViewService = userManagerViewService;
            _loginServices = loginServices;
        }

        [HttpGet]
        [Authorize]
        [Route("ajax/UserManagerApi/GetUsers")]
        public HttpResponseMessage GetUsers()
        {
            var context = _contextService.GetContext();
            var userManager = new UserManagerViewModel();
            userManager = _userManagerViewService.GetUsers(context);
            return Request.CreateResponse(HttpStatusCode.OK, userManager, MediaTypeHeaderValue.Parse("application/json"));
        }

        [HttpGet]
        [Authorize(Roles = Common.Administrator + "," + Common.Customer)]
        [Route("ajax/UserManagerApi/GetUser/{id}")]
        public HttpResponseMessage GetUser(string id)
        {
            var user = new UserManagerViewModel();
            user = _userManagerViewService.GetUser(id);
            return Request.CreateResponse(HttpStatusCode.OK, user, MediaTypeHeaderValue.Parse("application/json"));
        }

        [HttpGet]
        [Authorize(Roles = Common.Administrator + "," + Common.Customer)]
        [Route("ajax/UserManagerApi/GetCustomers")]
        public HttpResponseMessage GetCustomers()
        {
            var context = _contextService.GetContext();
            var customers = new List<string>();
            return Request.CreateResponse(HttpStatusCode.OK, customers, MediaTypeHeaderValue.Parse("application/json"));
        }

        [HttpPost]
        [Authorize(Roles = Common.Administrator + "," + Common.Customer)]
        [Route("ajax/UserManagerApi/InsertUser")]
        public HttpResponseMessage InsertUser(UserViewModel user)
        {
            try
            {
                var context = _contextService.GetContext();
                var result = _userManagerViewService.CreateUser(user, context);
                return Request.CreateResponse(HttpStatusCode.OK, result, MediaTypeHeaderValue.Parse("application/json"));
            }
            catch (System.InvalidOperationException ex)
            {
                return Request.CreateResponse(HttpStatusCode.Conflict, ex, MediaTypeHeaderValue.Parse("application/json"));
            }
        }

        [HttpGet]
        [Authorize(Roles = Common.Administrator + "," + Common.Customer)]
        [Route("ajax/UserManagerApi/GetMachinesByCustomer/{id}")]
        public HttpResponseMessage GetMachinesByCustomer(string id)
        {
            var result = _userManagerViewService.GetMachinesByCustomer(id);
            return Request.CreateResponse(HttpStatusCode.OK, result, MediaTypeHeaderValue.Parse("application/json"));
        }

        [HttpPut]
        [Authorize(Roles = Common.Administrator + "," + Common.Customer)]
        [Route("ajax/UserManagerApi/EditUser")]
        public HttpResponseMessage EditUser(UserViewModel user)
        {
            var context = _contextService.GetContext();
            var result = _userManagerViewService.EditUser(user, context);
            return Request.CreateResponse(HttpStatusCode.OK, result, MediaTypeHeaderValue.Parse("application/json"));
        }

        [HttpPost]
        [Authorize(Roles = Common.Operator + "," + Common.HeadWorkshop + "," + Common.Assistance + "," + Common.Administrator)]
        [Route("ajax/UserManagerApi/ChangePassword")]
        public HttpResponseMessage ChangePassword(ChangePasswordViewModel changePasswordInfo)
        {
            var context = _contextService.GetContext();
            var result = _userManagerViewService.ChangePassword(context, changePasswordInfo);
            return Request.CreateResponse(HttpStatusCode.OK, result, MediaTypeHeaderValue.Parse("application/json"));
        }

        [HttpPost]
        [Authorize(Roles = Common.Operator + "," + Common.Customer + "," + Common.HeadWorkshop + "," + Common.Assistance + "," + Common.Administrator)]
        [Route("ajax/UserManagerApi/CheckFirstLogin")]
        public HttpResponseMessage CheckFirstLogin()
        {
            var context = _contextService.GetContext();
            var result = false;
            if (context.User.Role == enRole.Operator || context.User.Role == enRole.HeadWorkshop)
            {
                result = _loginServices.IsFirstLogin();
            }
            
            return Request.CreateResponse(HttpStatusCode.OK, result, MediaTypeHeaderValue.Parse("application/json"));
        }

        [HttpPost]
        [Authorize(Roles = Common.Administrator + "," + Common.Customer)]
        [Route("ajax/UserManagerApi/ResetUserPassword/{id}")]
        public HttpResponseMessage ResetUserPassword(string id)
        {
            var result = _userManagerViewService.ResetUserPassword(id);
            return Request.CreateResponse(HttpStatusCode.OK, result, MediaTypeHeaderValue.Parse("application/json"));
        }

        [HttpDelete]
        [Authorize(Roles = Common.Administrator + "," + Common.Customer)]
        [Route("ajax/UserManagerApi/DeleteUser/{id}")]
        public HttpResponseMessage DeleteUser(string id)
        {
            var result = _userManagerViewService.DeleteUser(id);
            return Request.CreateResponse(HttpStatusCode.OK, result, MediaTypeHeaderValue.Parse("application/json"));
        }


        [HttpGet]
        [Authorize]
        [Route("ajax/UserManagerApi/GetTimeZones")]
        public HttpResponseMessage GetTimeZones()
        {
            var languageCode = CultureInfo.CurrentUICulture.Name;


            return Request.CreateResponse(HttpStatusCode.OK, TZNames.GetCountryNames(languageCode)
                .SelectMany(x => GetTimeZonesForCountry(x.Key, DateTimeOffset.UtcNow, languageCode)
                    .Select(y => new { CountryCode = x.Key, Country = x.Value, TimeZoneId = y.Key, TimeZoneName = y.Value }))
                .GroupBy(x => x.TimeZoneId)
                .ToDictionary(x => x.Key, x => $"{x.First().Country} - {x.First().TimeZoneName}"), MediaTypeHeaderValue.Parse("application/json"));
        }

        [HttpGet]
        [Authorize]
        [Route("ajax/UserManagerApi/GetCurrentTimeZone")]
        public HttpResponseMessage GetCurrentTimeZone()
        {
            var context = _contextService.GetContext();
            
            return Request.CreateResponse(HttpStatusCode.OK, context.User.TimeZone, MediaTypeHeaderValue.Parse("application/json"));
        }

        [HttpPost]
        [Authorize]
        [Route("ajax/UserManagerApi/ChangeTimeZone")]
        public HttpResponseMessage ChangeTimeZone([FromBody] string timezone)
        {
            var context = _contextService.GetContext();
            _userManagerViewService.ChangeTimeZone(context, timezone);
            _contextService.SetActualTimeZone(timezone);
            return Request.CreateResponse(HttpStatusCode.OK, new{}, MediaTypeHeaderValue.Parse("application/json"));
        }

        private static IDictionary<string, string> GetTimeZonesForCountry(string country, DateTimeOffset? threshold, string languageCode)
        {
            return threshold == null
                ? TZNames.GetTimeZonesForCountry(country, languageCode)
                : TZNames.GetTimeZonesForCountry(country, languageCode, threshold.Value);
        }

    }
}
