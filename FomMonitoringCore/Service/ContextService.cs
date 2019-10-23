﻿using FomMonitoringCore.Framework.Common;
using FomMonitoringCore.Framework.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;
using UserManager.DAL;

namespace FomMonitoringCore.Service
{
    public class ContextService : IContextService
    {
        private IMachineService _machineService;

        public ContextService(IMachineService machineService)
        {
            _machineService = machineService;
        }

        public bool InitializeContext()
        {
            bool contextIsSet = false;

            try
            {
                UserModel ActualUser = AccountService.Init().GetLoggedUser();

                if (ActualUser == null)
                    return contextIsSet;

                ContextModel context = new ContextModel();
                context.User = ActualUser;

                context.AllLanguages = UserManagerService.GetLanguages().OrderBy(o => o.IdLanguage).ToList();

                
                context.ActualLanguage = context.User.Language == null ? context.AllLanguages.FirstOrDefault() :
                    UserManagerService.GetLanguages().Where(lan => lan.ID == context.User.Language.ID).FirstOrDefault();

                SetContext(context);


                UserIdentityModel UserIdentity = new UserIdentityModel(ActualUser.Username, new List<string> { ActualUser.Role.ToString().SHA256Encript() });
                HttpContext.Current.User = UserIdentity;

                HttpCookie cookie = new HttpCookie(ActualUser.Username, ActualUser.Role.ToString().SHA256Encript());
                HttpContext.Current.Response.Cookies.Add(cookie);

                contextIsSet = true;

            }
            catch (Exception ex)
            {
                LogService.WriteLog(ex.GetStringLog(), LogService.TypeLevel.Error, ex);
            }

            return contextIsSet;
        }

        public ContextModel GetContext()
        {
            ContextModel context = SessionService.GetSessionValue<ContextModel>("Context");
            bool isWebApiRequest = HttpContext.Current.Request.AppRelativeCurrentExecutionFilePath.StartsWith("~/ajax");
            if (isWebApiRequest)
            {
                string defaultLanguage = "en";
                string lang = context.ActualLanguage.InitialsLanguage ?? defaultLanguage;
                try
                {
                    Thread.CurrentThread.CurrentCulture = Thread.CurrentThread.CurrentUICulture = new CultureInfo(lang);
                }
                catch (Exception)
                {
                    Thread.CurrentThread.CurrentCulture = Thread.CurrentThread.CurrentUICulture = new CultureInfo(defaultLanguage);
                }
            }
            return context;
        }

        public void SetContext(ContextModel context)
        {
            SessionService.SetSessionValue("Context", context);
        }

        public bool InitializeMessagesMachineLevel()
        {
            bool isInitialize = false;

            ContextModel context = GetContext();
            context.ActualPage = enPage.PlantMessages;                       
            return isInitialize;


        }

        public bool InitializeMesLevel()
        {
            bool isInitialize = false;

            ContextModel context = GetContext();
            context.ActualPage = enPage.Mes;

            if (context.User.Role == enRole.Administrator)
                context.AllPlants = MesService.GetAllPlantsMachines();
            else
                context.AllPlants = MesService.GetUserPlants(context.User.ID);

            if (context.AllPlants.Count > 0)
            {
                if(context.ActualPlant == null)
                    context.ActualPlant = context.AllPlants.FirstOrDefault();

                if (context.User.Role == enRole.Administrator)
                    context.AllMachines = _machineService.GetAllMachines();
                else
                    context.AllMachines = _machineService.GetUserMachines(context.User.ID);

                context.ActualPeriod = new PeriodModel() {
                    LastUpdate = new DataUpdateModel() {
                        DateTime = DateTime.UtcNow
                    }
                };

                isInitialize = true;

                SetContext(context);
            }

            return isInitialize;
        }

        public bool InitializeMachineLevel(int? MachineID = null)
        {
            bool isInitialize = false;

            ContextModel context = GetContext();
            context.ActualPage = enPage.Machine;

            if (context.User.Role == enRole.Administrator)
                context.AllPlants = MesService.GetAllPlantsMachines();
            else
                context.AllPlants = MesService.GetUserPlants(context.User.ID);

            if (context.AllPlants.Count > 0)
            {
                if (MachineID != null)
                {
                    MachineInfoModel machine = _machineService.GetMachineInfo(MachineID.Value);

                    if (machine == null)
                        return isInitialize;

                    context.ActualPlant = context.AllPlants.FirstOrDefault(w => w.Id == machine.PlantId);

                    if (context.User.Role == enRole.Administrator)
                        context.AllMachines = _machineService.GetAllMachines();
                    else
                        context.AllMachines = _machineService.GetUserMachines(context.User.ID);

                    if (context.AllMachines.Count == 0)
                        return isInitialize;

                    context.ActualMachine = context.AllMachines.Where(w => w.Id == MachineID.Value).FirstOrDefault();
                }
                else
                {
                    if (context.ActualPlant == null)
                        context.ActualPlant = context.AllPlants.FirstOrDefault();

                    if (context.User.Role == enRole.Administrator)
                        context.AllMachines = _machineService.GetAllMachines();
                    else
                        context.AllMachines = _machineService.GetUserMachines(context.User.ID);

                    if (context.AllMachines.Count == 0)
                        return isInitialize;

                    if (context.ActualMachine == null)
                        context.ActualMachine = context.AllMachines.FirstOrDefault();
                }
            }
            else
            {
                context.AllMachines = _machineService.GetUserMachines(context.User.ID);

                if (context.AllMachines.Count == 0)
                    return isInitialize;

                if (MachineID != null)
                    context.ActualMachine = context.AllMachines.Where(w => w.Id == MachineID.Value).FirstOrDefault();
                else
                    context.ActualMachine = context.AllMachines.FirstOrDefault();
            }

            if (context.ActualMachine != null && context.ActualMachine.LastUpdate != null)
            {
                DateTime LastUpdate = context.ActualMachine.LastUpdate.Value.AddHours(context.ActualMachine.UTC ?? 0);
                context.ActualPeriod = PeriodService.GetPeriodModel(LastUpdate.Date, LastUpdate, enAggregation.Day);
            }

            isInitialize = true;

            SetContext(context);

            return isInitialize;
        }

        public bool InitializePlantManagerLevel()
        {
            bool isInitialize = false;

            ContextModel context = GetContext();
            context.ActualPage = enPage.PlantManager;

            isInitialize = true;
            SetContext(context);

            return isInitialize;
        }

        public bool InitializeAdminLevel()
        {
            bool isInitialize = false;

            ContextModel context = GetContext();
            context.ActualPage = enPage.UserManager;

            isInitialize = true;
            SetContext(context);

            return isInitialize;
        }

        public void SetActualPage(enPage Page)
        {
            ContextModel context = GetContext();
            context.ActualPage = Page;

            SetContext(context);
        }

        public void SetActualLanguage(string LanguageNameISO)
        {
            ContextModel context = GetContext();

            if (context.ActualLanguage.InitialsLanguage != LanguageNameISO)
                context.ActualLanguage = context.AllLanguages.Where(w => w.InitialsLanguage == LanguageNameISO).FirstOrDefault();

            SetContext(context);
        }

        public void SetActualPlant(int id)
        {
            ContextModel context = GetContext();

            if (context.ActualPlant.Id != id)
            {
                context.ActualPlant = context.AllPlants.Where(w => w.Id == id).FirstOrDefault();

                if (context.User.Role == enRole.Administrator)
                    context.AllMachines = _machineService.GetAllMachines();
                else
                    context.AllMachines = _machineService.GetUserMachines(context.User.ID);
                //  context.AllMachines = MachineService.GetAllMachinesByPlantID(context.ActualPlant.Id);
            }

            SetContext(context);
        }

        public void SetActualMachine(int id)
        {
            ContextModel context = GetContext();

            if (context.ActualMachine.Id != id)
            {
                context.ActualMachine = context.AllMachines.Where(w => w.Id == id).FirstOrDefault();

                if (context.ActualMachine != null && context.ActualMachine.LastUpdate != null)
                {
                    DateTime LastUpdate = context.ActualMachine.LastUpdate.Value;
                    context.ActualPeriod = PeriodService.GetPeriodModel(LastUpdate.Date, LastUpdate, enAggregation.Day);
                }
            }

            SetContext(context);
        }

        public void SetActualPeriod(DateTime start, DateTime end)
        {
            ContextModel context = GetContext();

            context.ActualPeriod.StartDate = start;
            context.ActualPeriod.EndDate = end;
            context.ActualPeriod.Aggregation = Common.GetAggregationType(start, end);

            SetContext(context);
        }

        public void CheckLastUpdate()
        {
            ContextModel context = GetContext();

            MachineInfoModel machine = _machineService.GetMachineInfo(context.ActualMachine.Id);

            if (context.ActualPeriod.LastUpdate.DateTime != machine.LastUpdate.Value)
                context.ActualPeriod.LastUpdate.DateTime = machine.LastUpdate.Value;

            SetContext(context);
        }

        public bool CheckSecurityParameterApi(int ID, enCheckParam check)
        {
            bool isCorrect = false;

            ContextModel context = GetContext();

            switch (check)
            {
                case enCheckParam.Machine:
                    if (context.AllMachines.Count > 0 && context.AllMachines.Where(w => w.Id == ID).FirstOrDefault() != null)
                        isCorrect = true;
                    break;
                case enCheckParam.Plant:
                    if (context.AllPlants.Count > 0 && context.AllPlants.Where(w => w.Id == ID).FirstOrDefault() != null)
                        isCorrect = true;
                    break;
            }

            return isCorrect;
        }

    }
}
