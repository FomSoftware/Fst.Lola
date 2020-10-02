using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Xml.Serialization;
using FomMonitoringCore.Framework.Common;
using FomMonitoringCore.Framework.Model.Xml;
using FomMonitoringCore.Service;
using FomMonitoringCore.Service.API;

namespace FomMonitoring.Controllers
{
    [SessionWeb]
    [Authorize(Roles =  Common.Administrator)]
    public class UploadConfigurationController : Controller
    {
        private readonly IXmlDataService _xmlDataService;
        private readonly IContextService _contextService;

        public UploadConfigurationController(IXmlDataService xmlDataService,
            IContextService contextService)
        {
            _xmlDataService = xmlDataService;
            _contextService = contextService;
        }
        // GET: UploadConfiguration
        public ActionResult Index(bool? success)
        {
            if (!_contextService.InitializeUploadConfigurationLevel())
                return RedirectToAction("Logout", "Account", new { returnUrl = string.Empty, exception = 4 });

            _contextService.SetActualLanguage(CultureInfo.CurrentCulture.Name);
            return View(success);
        }

        public async Task<ActionResult> SaveConfiguration(HttpPostedFileBase file)
        {
            if (file != null && file.ContentLength > 0)
                try
                {
                    using (var ms = new MemoryStream())
                    {
                        await file.InputStream.CopyToAsync(ms);
                        ms.Position = 0;

                        var contents = await new StreamReader(ms).ReadToEndAsync();
                        var serializer = new XmlSerializer(typeof(ParametersMachineModelXml));
                        using (var memStream = new MemoryStream(Encoding.UTF8.GetBytes(contents)))
                        {
                            var resultingMessage = (ParametersMachineModelXml)serializer.Deserialize(memStream);
                            if (ValidateLolaXml(resultingMessage))
                            {
                                await _xmlDataService.AddOrUpdateMachineParameterAsync(resultingMessage);
                            }
                            else
                            {
                                throw new Exception("Validazione LOLA.xml Fallita");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    var errMessage = string.Format(ex.GetStringLog());
                    LogService.WriteLog(errMessage, LogService.TypeLevel.Error, ex);
                    return RedirectToAction("Index", new { success = false });
                }
            else
            {
                return RedirectToAction("Index", new { success = false });
            }
            return RedirectToAction("Index", new { success = true});
        }


        private bool ValidateLolaXml(ParametersMachineModelXml machineModelXml)
        {
            if (!(machineModelXml.ModelCodeV997 > 0))
            {
                return false;
            }
            if (machineModelXml.Parameters.Parameter.Any(n => string.IsNullOrWhiteSpace(n.VAR_NUMBER)))
            {
                return false;
            }
            return true;
        }
    }
}