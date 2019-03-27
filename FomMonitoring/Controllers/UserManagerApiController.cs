using FomMonitoringBLL.ViewModel;
using FomMonitoringBLL.ViewServices;
using FomMonitoringCore.Framework.Common;
using FomMonitoringCore.Framework.Model;
using FomMonitoringCore.Service;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;

namespace FomMonitoring.Controllers
{
    [SessionApi]
    
    public class UserManagerApiController : ApiController
    {

        [HttpGet]
        [Authorize]
        [Route("ajax/UserManagerApi/GetUsers")]
        public HttpResponseMessage GetUsers()
        {
            ContextModel context = ContextService.GetContext();
            UserManagerViewModel userManager = new UserManagerViewModel();
            userManager = UserManagerViewService.GetUsers(context);
            return Request.CreateResponse(HttpStatusCode.OK, userManager, MediaTypeHeaderValue.Parse("application/json"));
        }

        [HttpGet]
        [Authorize(Roles = Common.Administrator + "," + Common.Customer)]
        [Route("ajax/UserManagerApi/GetUser/{id}")]
        public HttpResponseMessage GetUser(string id)
        {
            UserManagerViewModel user = new UserManagerViewModel();
            user = UserManagerViewService.GetUser(id);
            return Request.CreateResponse(HttpStatusCode.OK, user, MediaTypeHeaderValue.Parse("application/json"));
        }

        [HttpGet]
        [Authorize(Roles = Common.Administrator + "," + Common.Customer)]
        [Route("ajax/UserManagerApi/GetCustomers")]
        public HttpResponseMessage GetCustomers()
        {
            ContextModel context = ContextService.GetContext();
            List<string> customers = new List<string>();
            return Request.CreateResponse(HttpStatusCode.OK, customers, MediaTypeHeaderValue.Parse("application/json"));
        }

        [HttpPost]
        [Authorize(Roles = Common.Administrator + "," + Common.Customer)]
        [Route("ajax/UserManagerApi/InsertUser")]
        public HttpResponseMessage InsertUser(UserViewModel user)
        {
            try
            {
                var result = UserManagerViewService.CreateUser(user);
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
            var result = UserManagerViewService.GetMachinesByCustomer(id);
            return Request.CreateResponse(HttpStatusCode.OK, result, MediaTypeHeaderValue.Parse("application/json"));
        }

        [HttpPut]
        [Authorize(Roles = Common.Administrator + "," + Common.Customer)]
        [Route("ajax/UserManagerApi/EditUser")]
        public HttpResponseMessage EditUser(UserViewModel user)
        {
            var result = UserManagerViewService.EditUser(user);
            return Request.CreateResponse(HttpStatusCode.OK, result, MediaTypeHeaderValue.Parse("application/json"));
        }

        [HttpPost]
        [Authorize(Roles = Common.Operator + "," + Common.HeadWorkshop + "," + Common.Assistance + "," + Common.Administrator)]
        [Route("ajax/UserManagerApi/ChangePassword")]
        public HttpResponseMessage ChangePassword(ChangePasswordViewModel changePasswordInfo)
        {
            ContextModel context = ContextService.GetContext();
            var result = UserManagerViewService.ChangePassword(context, changePasswordInfo);
            return Request.CreateResponse(HttpStatusCode.OK, result, MediaTypeHeaderValue.Parse("application/json"));
        }

        [HttpPost]
        [Authorize(Roles = Common.Administrator + "," + Common.Customer)]
        [Route("ajax/UserManagerApi/ResetUserPassword/{id}")]
        public HttpResponseMessage ResetUserPassword(string id)
        {
            var result = UserManagerViewService.ResetUserPassword(id);
            return Request.CreateResponse(HttpStatusCode.OK, result, MediaTypeHeaderValue.Parse("application/json"));
        }

        [HttpDelete]
        [Authorize(Roles = Common.Administrator + "," + Common.Customer)]
        [Route("ajax/UserManagerApi/DeleteUser/{id}")]
        public HttpResponseMessage DeleteUser(string id)
        {
            var result = UserManagerViewService.DeleteUser(id);
            return Request.CreateResponse(HttpStatusCode.OK, result, MediaTypeHeaderValue.Parse("application/json"));
        }
    }
}
