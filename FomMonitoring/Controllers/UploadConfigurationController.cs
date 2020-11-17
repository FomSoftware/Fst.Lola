using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Results;
using System.Web.Mvc;
using System.Xml.Serialization;
using FomMonitoringBLL.ViewModel;
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
        public ActionResult Index(LoadConfigurationModel success)
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
                            List<string> errors = new List<string>();
                            if (ValidateLolaXml(resultingMessage, out errors))
                            {
                                await _xmlDataService.AddOrUpdateMachineParameterAsync(resultingMessage);
                            }
                            else
                            {
                                ViewBag.success = false;
                                ViewBag.errors = LocalizationService.GetResource("FileNotValid");
                                if (errors.Count > 0)
                                {
                                    foreach (var ee in errors)
                                    {
                                        ViewBag.errors += $"\\n {ee}";
                                    }
                                }
                                //throw new Exception("Validazione LOLA.xml Fallita");
                                return View("Index");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    var errMessage = string.Format(ex.GetStringLog());
                    LogService.WriteLog(errMessage, LogService.TypeLevel.Error, ex);
                    ViewBag.success = false;
                    ViewBag.errors = LocalizationService.GetResource("ErrorLoadXml") + "\\n" + ex.Message;
                    return View("Index");
                }
            else
            {
                ViewBag.success = false;
                ViewBag.errors = LocalizationService.GetResource("ErrorLoadXml") + "\\n" + LocalizationService.GetResource("FileNotFound");
                return View("Index");
            }
            ViewBag.success = true;
            return View("Index");
        }


        private bool ValidateLolaXml(ParametersMachineModelXml machineModelXml, out List<string> errors)
        {
            errors = new List<string>();

            if (!(machineModelXml.ModelCodeV997 > 0))
            {
                errors.Add($"ModelCodeV997: {machineModelXml.ModelCodeV997} { LocalizationService.GetResource("NotExists")}");
            }
            else if(!_xmlDataService.CheckMachineModelCode(machineModelXml.ModelCodeV997))
            {
                errors.Add($"ModelCodeV997: {machineModelXml.ModelCodeV997} { LocalizationService.GetResource("NotExists")}" );
            }

            if (errors.Count() == 0)
            {
                var wrongVarNumber = machineModelXml.Parameters.Parameter.Where(n => string.IsNullOrWhiteSpace(n.VAR_NUMBER) ||
                                                                                     !_xmlDataService.CheckVarNumber(machineModelXml.ModelCodeV997, n.VAR_NUMBER)).ToList();
                if (wrongVarNumber.Count() > 0)
                {
                    foreach (var row in wrongVarNumber)
                    {
                        errors.Add($"VarNumber {LocalizationService.GetResource("NotExists")}: {row.VAR_NUMBER} (keyword: {row.KEYWORD})");
                    }
                }
            }
                

            var wrongIdPanel = machineModelXml.Parameters.Parameter.Where(n => !(n.PANEL_ID > 0) ||
                                                                                 !_xmlDataService.CheckPanelId(n.PANEL_ID)).ToList();
            if (wrongIdPanel.Count() > 0)
            {
                foreach (var row in wrongIdPanel)
                {
                    errors.Add($"PANEL_ID {LocalizationService.GetResource("NotExists")}: {row.PANEL_ID} (keyword: {row.KEYWORD})");
                }
            }

            var wrongMachineGroup = machineModelXml.Parameters.Parameter.Where(n => string.IsNullOrWhiteSpace(n.MACHINE_GROUP) ||
                                                                               !_xmlDataService.CheckMachineGroup(n.MACHINE_GROUP)).ToList();
            if (wrongMachineGroup.Count() > 0)
            {
                foreach (var row in wrongMachineGroup)
                {
                    errors.Add($"MACHINE_GROUP {LocalizationService.GetResource("NotExists")}: {row.MACHINE_GROUP} (keyword: {row.KEYWORD})");
                }
            }

            if (errors.Count() > 0) return false;
            return true;
        }
    }
}