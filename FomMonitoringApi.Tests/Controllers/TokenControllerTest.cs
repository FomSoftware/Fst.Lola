using FomMonitoringCore.Service.API;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace FomMonitoringApi.Tests.Controllers
{
    [TestClass]
    public class TokenControllerTest
    {
        [TestMethod]
        public void Post1Test()
        {
            string token = "jwtToken";
            Mock<IJwtManager> mock = new Mock<IJwtManager>();
            mock.Setup(s => s.GenerateToken(It.IsAny<string>(), It.IsAny<int>())).Returns(token);
            var tokenController = new FomMonitoringApi.Controllers.TokenController(mock.Object);

            var actual = tokenController.Post();

            Assert.AreEqual(token, actual);
        }

        [TestMethod]
        public void Post2Test()
        {
            string token = "jwtToken";
            Mock<IJwtManager> mock = new Mock<IJwtManager>();
            mock.Setup(s => s.GenerateToken(It.IsAny<string>(), It.IsAny<int>())).Returns(token);
            var tokenController = new FomMonitoringApi.Controllers.TokenController(mock.Object);

            var actual = tokenController.Post();

            mock.Verify(v => v.GenerateToken(It.IsAny<string>(), It.IsAny<int>()), Times.Once);
        }
    }
}
