using FomMonitoringCore.DAL;
using FomMonitoringCore.Framework.Common;
using FomMonitoringCore.Framework.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Transactions;
using System.Xml;
using UserManager.DAL;

namespace FomMonitoringCore.Service.APIClient.Concrete
{
    public class JsonAPIClientService : IJsonAPIClientService
    {
        public enLoginResult ValidateCredentialsViaRemoteApi(string username, string password)
        {
            string apiUrl = ApplicationSettingService.GetWebConfigKey("RemoteLoginSOAPUrl");
            var apiUsername = ApplicationSettingService.GetWebConfigKey("RemoteLoginSOAPUsername");
            var apiPassword = ApplicationSettingService.GetWebConfigKey("RemoteLoginSOAPPassword");

            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(apiUrl);
            webRequest.Headers.Add("SOAPAction", "http://tempuri.org/IService/verificaUtentePassword");
            webRequest.ContentType = "text/xml;charset=\"utf-8\"";
            webRequest.Accept = "text/xml";
            webRequest.Method = "POST";
            webRequest.CookieContainer = new CookieContainer();
            webRequest.Credentials = new NetworkCredential(apiUsername, apiPassword, "");

            XmlDocument soapEnvelopeXml = new XmlDocument();
            soapEnvelopeXml.LoadXml(string.Format(@"<soapenv:Envelope xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:tem=""http://tempuri.org/""><soapenv:Header></soapenv:Header><soapenv:Body><tem:verificaUtentePassword><tem:utente>{0}</tem:utente><tem:password>{1}</tem:password></tem:verificaUtentePassword></soapenv:Body></soapenv:Envelope>", username, password));

            using (Stream stream = webRequest.GetRequestStream())
            {
                soapEnvelopeXml.Save(stream);
            }

            IAsyncResult asyncResult = webRequest.BeginGetResponse(null, null);
            asyncResult.AsyncWaitHandle.WaitOne();

            string soapResult;
            using (WebResponse webResponse = webRequest.EndGetResponse(asyncResult))
            {
                using (StreamReader rd = new StreamReader(webResponse.GetResponseStream()))
                {
                    soapResult = rd.ReadToEnd();
                }
            }

            XmlDocument document = new XmlDocument();
            document.LoadXml(soapResult);

            enLoginResult result = enLoginResult.NotExists;
            try
            {
                JsonLoginModel login = JsonConvert.DeserializeObject<JsonLoginModel>(document.InnerText);
                result = login.enResult ?? result;
            }
            catch (Exception ex)
            {
                LogService.WriteLog(ex.Message, LogService.TypeLevel.Error, ex);
            }

            return result;
        }

        public bool UpdateActiveCustomersAndMachines()
        {
            bool result = true;
            try
            {
                string apiUrl = ApplicationSettingService.GetWebConfigKey("UpdateCustomersAndMachinesSOAPUrl");
                var apiUsername = ApplicationSettingService.GetWebConfigKey("UpdateCustomersAndMachinesSOAPUsername");
                var apiPassword = ApplicationSettingService.GetWebConfigKey("UpdateCustomersAndMachinesSOAPPassword");

                HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(apiUrl);
                webRequest.Headers.Add("SOAPAction", "http://tempuri.org/IService/ottieniListaMacchineRegistrate");
                webRequest.ContentType = "text/xml;charset=\"utf-8\"";
                webRequest.Accept = "text/xml";
                webRequest.Method = "POST";
                webRequest.CookieContainer = new CookieContainer();
                webRequest.Credentials = new NetworkCredential(apiUsername, apiPassword, "");

                XmlDocument soapEnvelopeXml = new XmlDocument();
                soapEnvelopeXml.LoadXml(@"<soapenv:Envelope xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:tem=""http://tempuri.org/""><soapenv:Header></soapenv:Header><soapenv:Body><tem:ottieniListaMacchineRegistrate/></soapenv:Body></soapenv:Envelope>");

                using (Stream stream = webRequest.GetRequestStream())
                {
                    soapEnvelopeXml.Save(stream);
                }

                IAsyncResult asyncResult = webRequest.BeginGetResponse(null, null);
                asyncResult.AsyncWaitHandle.WaitOne();

                string soapResult;
                using (WebResponse webResponse = webRequest.EndGetResponse(asyncResult))
                {
                    using (StreamReader rd = new StreamReader(webResponse.GetResponseStream()))
                    {
                        soapResult = rd.ReadToEnd();
                    }
                }

                XmlDocument document = new XmlDocument();
                document.LoadXml(soapResult);

                JsonCustomersModel customers = JsonConvert.DeserializeObject<JsonCustomersModel>(document.InnerText, new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" });                

                using (FST_FomMonitoringEntities ent = new FST_FomMonitoringEntities())
                {
                    //elimino i customer non più presenti nella vip area

                    List<UserModel> dbCustomers =UserManagerService.GetAllCustomers();
                    IEnumerable<string> customerNames = customers.customers.Select(j => j.username).Distinct();
                    List<UserModel> custmerToRemove = dbCustomers.Where(e => !customerNames.Contains(e.Username)).ToList<UserModel>();

                    if (custmerToRemove != null && custmerToRemove.Count() > 0)
                    {
                        //rimuovo prima le associazioni
                        List<Guid> ids = custmerToRemove.Select(a => a.ID).ToList();
                        List<string> names = custmerToRemove.Select(a => a.Username).ToList();
                        List<UserCustomerMapping> us = ent.UserCustomerMapping.Where(uc => names.Contains(uc.CustomerName)).ToList();

                        //utenti associati al customer
                        List<UserModel> usCust = new List<UserModel>();
                        foreach (UserCustomerMapping item in us)
                        {
                            usCust.AddRange(UserManagerService.GetUsers(item.CustomerName));
                        }

                        if (us.Any())
                        { 
                            ent.UserCustomerMapping.RemoveRange(us);
                            ent.SaveChanges();
                        }

                        List<UserMachineMapping> um = ent.UserMachineMapping.Where(mh => ids.Contains(mh.UserId)).ToList();
                        if (um.Any())
                        {
                            ent.UserMachineMapping.RemoveRange(um);
                            ent.SaveChanges();
                        }

                        usCust.AddRange(custmerToRemove);
                        using (TransactionScope transactionSuppress = new TransactionScope(TransactionScopeOption.Suppress))
                        {
                            UserManagerService.RemoveUsers(usCust);
                            transactionSuppress.Complete();
                        }
                    }
                    //pulizia della tabella UserCustomerMapping, potrebbero esserci record inseriti a mano con customerName non esistenti
                    
                    foreach (var customer in customers.customers)
                    {
                        try
                        {
                            //Aggiungo eventuali nuovi clienti
                            UserModel user = new UserModel();
                            using (TransactionScope transactionSuppress = new TransactionScope(TransactionScopeOption.Suppress))
                            {
                                user = UserManagerService.GetUser(customer.username);
                                if (user == null)
                                {
                                    user = new UserModel()
                                    {
                                        Username = customer.username,
                                        FirstName = customer.username,
                                        LastName = customer.username,
                                        Enabled = true,
                                        Role = enRole.Customer,
                                        CustomerName = customer.username
                                    };
                                    user.ID = UserManagerService.CreateUser(user);
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
                            var plant = MesService.GetOrSetPlantDefaultByUser(user.ID);

                            //Inserisco i nuovi mapping cliente <=> macchina
                            foreach (var machine in machines)
                            {
                                JsonMachine jm = customer.machines.FirstOrDefault(f => f.serial == machine.Serial);
                                if (jm != null)
                                {
                                    DateTime expirationDate = jm.expirationDate;
                                    DateTime activationDate = jm.activationDate;
                                    string machineName = jm.machineName;
                                    List<UserMachineMapping> usersMachineMapped = ent.UserMachineMapping.Where(w => w.MachineId == machine.Id).ToList();
                                    if (usersMachineMapped.Any())
                                    {
                                        /*foreach (UserMachineMapping userMachineMapped in usersMachineMapped)
                                        {
                                            userMachineMapped.ExpirationDate = expirationDate;
                                            //userMachineMapped.ActivationDate = activationDate;
                                            ent.SaveChanges();
                                        }*/
                                    }
                                    else
                                    {
                                        UserMachineMapping userMachine = new UserMachineMapping()
                                        {
                                            //ExpirationDate = expirationDate,
                                            //ActivationDate = activationDate,
                                            MachineId = machine.Id,
                                            UserId = user.ID
                                        };
                                        ent.UserMachineMapping.Add(userMachine);
                                        ent.SaveChanges();
                                    }
                                    //aggiorno l'activationDate della macchina prendendo la più vecchia
                                    Machine ma = ent.Machine.Find(machine.Id);
                                    if (ma.ActivationDate == null || ma.ActivationDate > activationDate)
                                    {
                                        ma.ActivationDate = activationDate;
                                    }
                                    if (ma.ExpirationDate == null || ma.ExpirationDate < expirationDate)
                                    {
                                        ma.ExpirationDate = expirationDate;
                                    }
                                    if (!string.IsNullOrWhiteSpace(machineName))
                                    {
                                        ma.MachineName = machineName;
                                    }
                                    ent.SaveChanges();
                                }

                            }
                        }
                        catch (Exception ex)
                        {
                            LogService.WriteLog(ex.Message, LogService.TypeLevel.Error, ex);
                            result = false;
                        }
                    }
                    
                }
            }
            catch (Exception ex)
            {
                LogService.WriteLog(ex.Message, LogService.TypeLevel.Error, ex);
            }

            return result;
        }

    }
}
