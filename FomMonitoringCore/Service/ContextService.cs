using FomMonitoringCore.Framework.Common;
using FomMonitoringCore.Framework.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;

namespace FomMonitoringCore.Service
{
    public class ContextService : IContextService
    {
        private readonly IMachineService _machineService;
        private readonly IMesService _mesService;
        private readonly IAccountService _accountService;
        private readonly IUserManagerService _userManagerService;

        public ContextService(IMachineService machineService, IMesService mesService, IAccountService accountService, IUserManagerService userManagerService)
        {
            _machineService = machineService;
            _mesService = mesService;
            _accountService = accountService;
            _userManagerService = userManagerService;
        }

        public bool InitializeContext()
        {
            try
            {
                var actualUser = _accountService.GetLoggedUser();

                if (actualUser == null)
                    return false;

                var context = new ContextModel
                {
                    User = actualUser,
                    AllLanguages = _userManagerService.GetLanguages().OrderBy(o => o.IdLanguage).ToList()
                };
                
                context.ActualLanguage = context.User.Language == null ? context.AllLanguages.FirstOrDefault() :
                    _userManagerService.GetLanguages().FirstOrDefault(lan => lan.ID == context.User.Language.ID);

                SetContext(context);


                var userIdentity = new UserIdentityModel(actualUser.Username, new List<string> { actualUser.Role.ToString().SHA256Encript() });
                HttpContext.Current.User = userIdentity;

                var cookie = new HttpCookie(actualUser.Username, actualUser.Role.ToString().SHA256Encript());
                HttpContext.Current.Response.Cookies.Add(cookie);
                

            }
            catch (Exception ex)
            {
                LogService.WriteLog(ex.GetStringLog(), LogService.TypeLevel.Error, ex);
            }

            return true;
        }

        public ContextModel GetContext()
        {
            var context = SessionService.GetSessionValue<ContextModel>("Context");
            var isWebApiRequest = HttpContext.Current.Request.AppRelativeCurrentExecutionFilePath != null && HttpContext.Current.Request.AppRelativeCurrentExecutionFilePath.StartsWith("~/ajax");
            if (!isWebApiRequest)
                return context;

            const string defaultLanguage = "en";
            var lang = context.ActualLanguage?.InitialsLanguage ?? defaultLanguage;
            try
            {
                Thread.CurrentThread.CurrentCulture = Thread.CurrentThread.CurrentUICulture = new CultureInfo(lang);
            }
            catch (Exception)
            {
                Thread.CurrentThread.CurrentCulture = Thread.CurrentThread.CurrentUICulture = new CultureInfo(defaultLanguage);
            }
            return context;
        }

        public void SetContext(ContextModel context)
        {
            SessionService.SetSessionValue("Context", context);
        }

        public bool InitializeMessagesMachineLevel()
        {

            var context = GetContext();
            context.ActualPage = enPage.PlantMessages;                       
            return false;

        }

        public bool InitializeMesLevel()
        {
            var context = GetContext();
            context.ActualPage = enPage.Mes;

            if (context.User.Role == enRole.Administrator)
                context.AllPlants = _mesService.GetAllPlantsMachines();
            else if (context.User.Role == enRole.Demo)
                context.AllPlants = _mesService.GetAllPlantsRole(enRole.Demo);
            else
                context.AllPlants = _mesService.GetUserPlants(context.User.ID);


            context.ActualPeriod = new PeriodModel()
            {
                LastUpdate = new DataUpdateModel()
                {
                    DateTime = DateTime.UtcNow,
                    TimeZone = context.User.TimeZone
                }
            };

            if (context.AllPlants.Any())
            {
                if(context.ActualPlant == null || context.AllPlants.FirstOrDefault(p => p.Id == context.ActualPlant.Id ) == null)
                    context.ActualPlant = context.AllPlants.FirstOrDefault();

                if (context.User.Role == enRole.Administrator)
                    context.AllMachines = _machineService.GetAllMachines();
                else if(context.User.Role == enRole.Demo)
                    context.AllMachines = _machineService.GetRoleMachines(enRole.Demo);
                else
                    context.AllMachines = _machineService.GetUserMachines(context);

                SetContext(context);
            }

            return true;
        }

        public bool InitializeMachineLevel(int? machineId = null)
        {

            var context = GetContext();
            context.ActualPage = enPage.Machine;

            if (context.User.Role == enRole.Administrator)
                context.AllPlants = _mesService.GetAllPlantsMachines();
            else if (context.User.Role == enRole.Demo)
                context.AllPlants = _mesService.GetAllPlantsRole(enRole.Demo);
            else
                context.AllPlants = _mesService.GetUserPlants(context.User.ID);

            if (context.AllPlants.Any())
            {
                if (machineId != null)
                {
                    var machine = _machineService.GetMachineInfo(machineId.Value);
                    
                    if (machine == null)
                        return false;

                    context.ActualPlant = context.AllPlants.FirstOrDefault(w => w.Id == machine.PlantId);

                    if (context.User.Role == enRole.Administrator)
                        context.AllMachines = _machineService.GetAllMachines();
                    else if (context.User.Role == enRole.Demo)
                        context.AllMachines = _machineService.GetRoleMachines(enRole.Demo);
                    else
                        context.AllMachines = _machineService.GetUserMachines(context);

                    if (context.AllMachines.Count == 0)
                        return false;

                    context.ActualMachine = context.AllMachines.FirstOrDefault(w => w.Id == machineId.Value);
                }
                else
                {
                    if (context.ActualPlant == null)
                        context.ActualPlant = context.AllPlants.FirstOrDefault();

                    if (context.User.Role == enRole.Administrator)
                        context.AllMachines = _machineService.GetAllMachines();
                    else if(context.User.Role == enRole.Demo)
                        context.AllMachines = _machineService.GetRoleMachines(enRole.Demo);
                    else
                        context.AllMachines = _machineService.GetUserMachines(context);

                    if (context.AllMachines.Count == 0)
                        return false;

                    if (context.ActualMachine == null)
                        context.ActualMachine = context.AllMachines.FirstOrDefault();
                }
            }
            else
            {
                context.AllMachines = _machineService.GetUserMachines(context);

                if (context.AllMachines.Count == 0)
                    return false;

                if (machineId != null)
                    context.ActualMachine = context.AllMachines.FirstOrDefault(w => w.Id == machineId.Value);
                else
                    context.ActualMachine = context.AllMachines.FirstOrDefault();
            }
            
            if (context.ActualMachine?.LastUpdate != null)
            {
                context.ActualPeriod = PeriodService.GetPeriodModel(context.ActualMachine?.LastUpdate?.Date ?? DateTime.UtcNow.Date, context.ActualMachine?.LastUpdate?.Date.AddDays(1).AddTicks(-1) ?? DateTime.UtcNow.Date.AddDays(1).AddTicks(-1), enAggregation.Day);
            }
            
            SetContext(context);

            return true;
        }

        public bool InitializeUploadConfigurationLevel()
        {

            var context = GetContext();
            context.ActualPage = enPage.UploadConfiguration;

            SetContext(context);

            return true;
        }

        public bool InitializePlantManagerLevel()
        {

            var context = GetContext();
            context.ActualPage = enPage.PlantManager;
            
            SetContext(context);

            return true;
        }

        public bool InitializeUserSettingLevel()
        {

            var context = GetContext();
            context.ActualPage = enPage.UserSetting;

            SetContext(context);

            return true;
        }

        public bool InitializeAdminLevel()
        {

            var context = GetContext();
            context.ActualPage = enPage.UserManager;
            
            SetContext(context);

            return true;
        }

        public void SetActualPage(enPage page)
        {
            var context = GetContext();
            context.ActualPage = page;

            SetContext(context);
        }

        public void SetActualLanguage(string languageNameIso)
        {
            var context = GetContext();

            if (context.ActualLanguage.InitialsLanguage != languageNameIso)
                context.ActualLanguage = context.AllLanguages.FirstOrDefault(w => w.InitialsLanguage == languageNameIso);

            SetContext(context);
        }

        public void SetActualTimeZone(string timezone)
        {
            var context = GetContext();

            context.User.TimeZone = timezone;

            SetContext(context);
        }


        public void SetActualPlant(int id)
        {
            var context = GetContext();

            if (context.ActualPlant.Id != id)
            {
                context.ActualPlant = context.AllPlants.FirstOrDefault(w => w.Id == id);

                if (context.User.Role == enRole.Administrator)
                    context.AllMachines = _machineService.GetAllMachines();
                else if(context.User.Role == enRole.Demo)
                    context.AllMachines = _machineService.GetRoleMachines(enRole.Demo);
                else
                    context.AllMachines = _machineService.GetUserMachines(context);
            }

            SetContext(context);
        }

        public void SetActualMachine(int id)
        {
            var context = GetContext();

            if (context.ActualMachine.Id != id)
            {
                context.ActualMachine = context.AllMachines.FirstOrDefault(w => w.Id == id);

                if (context.ActualMachine?.LastUpdate != null)
                {
                    var lastUpdate = context.ActualMachine.LastUpdate.Value;
                    context.ActualPeriod = PeriodService.GetPeriodModel(lastUpdate.Date, lastUpdate, enAggregation.Day);
                }
            }

            SetContext(context);
        }

        public void SetActualPeriod(DateTime start, DateTime end)
        {
            var context = GetContext();

            context.ActualPeriod.StartDate = start;
            context.ActualPeriod.EndDate = end;
            context.ActualPeriod.Aggregation = Common.GetAggregationType(start, end);

            SetContext(context);
        }

        public void SetActualMachineGroup(string group)
        {
            var context = GetContext();

            context.ActualMachineGroup = group;

            SetContext(context);
        }

        public void CheckLastUpdate()
        {
            var context = GetContext();

            var machine = _machineService.GetMachineInfo(context.ActualMachine.Id);

            if (machine.LastUpdate != null && context.ActualPeriod.LastUpdate.DateTime != machine.LastUpdate.Value)
                context.ActualPeriod.LastUpdate.DateTime = machine.LastUpdate.Value;

            SetContext(context);
        }

        public bool CheckSecurityParameterApi(int id, enCheckParam check)
        {
            var isCorrect = false;

            var context = GetContext();

            switch (check)
            {
                case enCheckParam.Machine:
                    if (context.AllMachines.Count > 0 && context.AllMachines.FirstOrDefault(w => w.Id == id) != null)
                        isCorrect = true;
                    break;
                case enCheckParam.Plant:
                    if (context.AllPlants.Count > 0 && context.AllPlants.FirstOrDefault(w => w.Id == id) != null)
                        isCorrect = true;
                    break;
            }

            return isCorrect;
        }

    }
}
