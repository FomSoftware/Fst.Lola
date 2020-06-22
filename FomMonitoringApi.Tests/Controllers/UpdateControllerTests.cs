using FomMonitoringApi.Controllers;
using FomMonitoringCore.Service.API;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Web.Http.Results;

namespace TokenController.Tests
{
    [TestClass]
    public class UpdateControllerTests
    {
        [TestMethod]
        public void MachineTest()
        {
            string json = "{\"test\":\"test\"}";
            string jsonResult = "{\"test\":\"test\",\"imported\":true}";
            Mock<IJsonDataService> mock = new Mock<IJsonDataService>();
            mock.Setup(s => s.AddJsonData(It.IsAny<string>(), It.IsAny<bool>())).Returns(true);
            object obj = JsonConvert.DeserializeObject(json);
            /*var updateController = new UpdateController(mock.Object);
            var actual = updateController.Machine(obj);
            Assert.AreEqual(jsonResult, JsonConvert.SerializeObject(((JsonResult<JObject>)actual).Content));
            */
        }

        [TestMethod]
        public void MachineCumulativeTest()
        {
            string json = "{\"test\":\"test\"}";
            string jsonResult = "{\"test\":\"test\",\"imported\":true}";
            Mock<IJsonDataService> mock = new Mock<IJsonDataService>();
            mock.Setup(s => s.AddJsonData(It.IsAny<string>(), It.IsAny<bool>())).Returns(true);
            object obj = JsonConvert.DeserializeObject(json);
            /*var updateController = new UpdateController(mock.Object);
            var actual = updateController.MachineCumulative(obj);
            Assert.AreEqual(jsonResult, JsonConvert.SerializeObject(((JsonResult<JObject>)actual).Content));
            */
        }

        [TestMethod]
        public void ClientTest()
        {
            //var updateController = new UpdateController();
            //string json = "{\"test\":\"test\"}";
            //object obj = JsonConvert.DeserializeObject(json);

            //var actual = updateController.Client(obj);

            //Assert.AreEqual(json, JsonConvert.SerializeObject(((JsonResult<object>)actual).Content));
        }
    }
}