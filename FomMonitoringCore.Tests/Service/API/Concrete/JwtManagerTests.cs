using FomMonitoringCore.Service.API.Concrete;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FomMonitoringCore.Tests.Service.API.Concrete
{
    [TestClass()]
    public class JwtManagerTests
    {
        [TestMethod()]
        public void GenerateToken1Test()
        {
            JwtManager jwtManager = new JwtManager();

            var actual = jwtManager.GenerateToken("test", 2);

            Assert.IsNotNull(actual);
        }

        [TestMethod()]
        public void GenerateToken2Test()
        {
            JwtManager jwtManager = new JwtManager();

            var actual = jwtManager.GenerateToken("", 2);

            Assert.IsNotNull(actual);
        }
    }
}