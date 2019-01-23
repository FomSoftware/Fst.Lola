using FomMonitoringCore.Framework.Common;
using FomMonitoringCore.Service.API;
using FomMonitoringCore.Service.API.Concrete;
using System.Net;
using System.Web.Http;

namespace FomMonitoringApi.Controllers
{
    public class TokenController : ApiController
    {
        private readonly IJwtManager _jwtManager;

        public TokenController()
        {
            _jwtManager = new JwtManager();
        }

        public TokenController(IJwtManager jwtManager)
        {
            _jwtManager = jwtManager;
        }

        [BasicAuthentication]
        [HttpPost]
        public string Post()
        {
            return _jwtManager.GenerateToken(User.Identity.Name);

            throw new HttpResponseException(HttpStatusCode.Unauthorized);
        }
    }
}