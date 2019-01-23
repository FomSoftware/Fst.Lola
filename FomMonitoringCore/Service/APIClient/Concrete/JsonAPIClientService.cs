using CommonCore.Service;
using FomMonitoringCore.DAL;
using FomMonitoringCore.Framework.Common;
using FomMonitoringCore.Framework.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using UserManager.DAL;
using UserManager.Service;
using UserManager.Service.Concrete;

namespace FomMonitoringCore.Service.APIClient.Concrete
{
    public class JsonAPIClientService : IJsonAPIClientService
    {
        public string GetJsonData(string method)
        {
            string result = string.Empty;
            try
            {
                result = CUrlService.ExecutePrincipalCUrl(method);
            }
            catch (Exception ex)
            {
                string errMessage = string.Format(ex.GetStringLog());
                LogService.WriteLog(errMessage, LogService.TypeLevel.Error, ex);
            }
            return result;
        }

        public enLoginResult ElaborateLoginJsonData(string json)
        {
            enLoginResult result = enLoginResult.NotExists;
            try
            {
                JsonLoginModel login = JsonConvert.DeserializeObject<JsonLoginModel>(json);
                result = login.enResult;
            }
            catch (Exception ex)
            {
                string errMessage = string.Format(ex.GetStringLog(), json);
                LogService.WriteLog(errMessage, LogService.TypeLevel.Error, ex);
            }
            return result;
        }

        public bool ElaborateUpdateUsersJsonData(string json)
        {
            bool result = true;
            try
            {
                JsonCustomersModel customers = JsonConvert.DeserializeObject<JsonCustomersModel>(json);
                IUserServices userService = new UserServices();
                using (TransactionScope transaction = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted }))
                using (FST_FomMonitoringEntities ent = new FST_FomMonitoringEntities())
                {
                    foreach (var customer in customers.customers)
                    {
                        try
                        {
                            //Aggiungo eventuali nuovi clienti
                            Users user = new Users();
                            using (TransactionScope transactionSuppress = new TransactionScope(TransactionScopeOption.Suppress))
                            {
                                user = userService.GetUser(customer.username);
                                if (user == null)
                                {
                                    user = new Users()
                                    {
                                        Username = customer.username,
                                        Enabled = true
                                    };
                                    userService.CreateUser(user);
                                }
                                transactionSuppress.Complete();
                            }

                            //Aggiungo eventuali nuovi clienti nel DB dei dati
                            UserCustomerMapping userCustomer = ent.UserCustomerMapping.FirstOrDefault(f => f.UserId == user.ID);
                            if (userCustomer == null)
                            {
                                userCustomer = new UserCustomerMapping()
                                {
                                    UserId = user.ID,
                                    CustomerName = user.Username
                                };
                                ent.UserCustomerMapping.Add(userCustomer);
                                ent.SaveChanges();
                            }

                            //Prendo la lista delle macchine esistenti nel DB a partire da quelle arrivate da JSON
                            List<string> machinesSerial = customer.machines.Select(s => s.serial).ToList();
                            List<Machine> machines = ent.Machine.Where(w => machinesSerial.Contains(w.Serial)).ToList();

                            //Rimuovo le associazioni cliente <=> macchina per macchine non più monitorate
                            List<int> machinesId = machines.Select(s => s.Id).ToList();
                            List<Guid> clientUsersMachines = ent.UserMachineMapping.Where(w => machinesId.Contains(w.MachineId)).Select(s => s.UserId).Distinct().ToList();
                            List<UserMachineMapping> usersMachinesToRemove = ent.UserMachineMapping.Where(w => !machinesId.Contains(w.MachineId) && clientUsersMachines.Contains(w.UserId)).ToList();
                            ent.UserMachineMapping.RemoveRange(usersMachinesToRemove);
                            ent.SaveChanges();

                            //Inserisco i nuovi mapping cliente <=> macchina
                            foreach (var machine in machines)
                            {
                                DateTime expirationDate = customer.machines.FirstOrDefault(f => f.serial == machine.Serial).expirationDate;
                                List<UserMachineMapping> usersMachineMapped = ent.UserMachineMapping.Where(w => w.MachineId == machine.Id).ToList();
                                if (usersMachineMapped.Any())
                                {
                                    foreach (UserMachineMapping userMachineMapped in usersMachineMapped)
                                    {
                                        userMachineMapped.ExpirationDate = expirationDate;
                                        ent.SaveChanges();
                                    }
                                }
                                else
                                {
                                    UserMachineMapping userMachine = new UserMachineMapping()
                                    {
                                        ExpirationDate = expirationDate,
                                        MachineId = machine.Id,
                                        UserId = user.ID
                                    };
                                    ent.UserMachineMapping.Add(userMachine);
                                    ent.SaveChanges();
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            string errMessage = string.Format(ex.GetStringLog(), json);
                            LogService.WriteLog(errMessage, LogService.TypeLevel.Error, ex);
                            result = false;
                        }
                    }

                    transaction.Complete();
                }
            }
            catch (Exception ex)
            {
                string errMessage = string.Format(ex.GetStringLog(), json);
                LogService.WriteLog(errMessage, LogService.TypeLevel.Error, ex);
            }
            return result;
        }
    }
}
