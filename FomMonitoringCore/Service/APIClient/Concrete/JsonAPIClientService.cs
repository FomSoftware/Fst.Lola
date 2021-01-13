using FomMonitoringCore.SqlServer;
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

namespace FomMonitoringCore.Service.APIClient.Concrete
{
    public class JsonAPIClientService : IJsonAPIClientService
    {
        private readonly IFomMonitoringEntities _context;
        private readonly IMesService _mesService;
        private readonly IUserManagerService _userManagerService;

        public JsonAPIClientService(IFomMonitoringEntities context, IMesService mesService, IUserManagerService userManagerService)
        {
            _context = context;
            _mesService = mesService;
            _userManagerService = userManagerService;
        }

        public enLoginResult ValidateCredentialsViaRemoteApi(string username, string password)
        {
            var apiUrl = ApplicationSettingService.GetWebConfigKey("RemoteLoginSOAPUrl");
            var apiUsername = ApplicationSettingService.GetWebConfigKey("RemoteLoginSOAPUsername");
            var apiPassword = ApplicationSettingService.GetWebConfigKey("RemoteLoginSOAPPassword");

            var webRequest = (HttpWebRequest)WebRequest.Create(apiUrl);
            webRequest.Headers.Add("SOAPAction", "http://tempuri.org/IService/verificaUtentePassword");
            webRequest.ContentType = "text/xml;charset=\"utf-8\"";
            webRequest.Accept = "text/xml";
            webRequest.Method = "POST";
            webRequest.CookieContainer = new CookieContainer();
            webRequest.Credentials = new NetworkCredential(apiUsername, apiPassword, "");

            var soapEnvelopeXml = new XmlDocument();
            soapEnvelopeXml.LoadXml(
                $@"<soapenv:Envelope xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:tem=""http://tempuri.org/""><soapenv:Header></soapenv:Header><soapenv:Body><tem:verificaUtentePassword><tem:utente>{username}</tem:utente><tem:password>{password}</tem:password></tem:verificaUtentePassword></soapenv:Body></soapenv:Envelope>");

            using (var stream = webRequest.GetRequestStream())
            {
                soapEnvelopeXml.Save(stream);
            }

            var asyncResult = webRequest.BeginGetResponse(null, null);
            asyncResult.AsyncWaitHandle.WaitOne();

            string soapResult;
            using (var webResponse = webRequest.EndGetResponse(asyncResult))
            {
                using (var rd = new StreamReader(webResponse.GetResponseStream()))
                {
                    soapResult = rd.ReadToEnd();
                }
            }

            var document = new XmlDocument();
            document.LoadXml(soapResult);

            var result = enLoginResult.NotExists;
            try
            {
                var login = JsonConvert.DeserializeObject<JsonLoginModel>(document.InnerText);
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
            var result = true;
            try
            {
                var apiUrl = ApplicationSettingService.GetWebConfigKey("UpdateCustomersAndMachinesSOAPUrl");
                var apiUsername = ApplicationSettingService.GetWebConfigKey("UpdateCustomersAndMachinesSOAPUsername");
                var apiPassword = ApplicationSettingService.GetWebConfigKey("UpdateCustomersAndMachinesSOAPPassword");

                var webRequest = (HttpWebRequest)WebRequest.Create(apiUrl);
                webRequest.Headers.Add("SOAPAction", "http://tempuri.org/IService/ottieniListaMacchineRegistrate");
                webRequest.ContentType = "text/xml;charset=\"utf-8\"";
                webRequest.Accept = "text/xml";
                webRequest.Method = "POST";
                webRequest.CookieContainer = new CookieContainer();
                webRequest.Credentials = new NetworkCredential(apiUsername, apiPassword, "");

                var soapEnvelopeXml = new XmlDocument();
                soapEnvelopeXml.LoadXml(@"<soapenv:Envelope xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:tem=""http://tempuri.org/""><soapenv:Header></soapenv:Header><soapenv:Body><tem:ottieniListaMacchineRegistrate/></soapenv:Body></soapenv:Envelope>");

                using (var stream = webRequest.GetRequestStream())
                {
                    soapEnvelopeXml.Save(stream);
                }

                var asyncResult = webRequest.BeginGetResponse(null, null);
                asyncResult.AsyncWaitHandle.WaitOne();

                string soapResult;
                using (var webResponse = webRequest.EndGetResponse(asyncResult))
                {
                    using (var rd = new StreamReader(webResponse.GetResponseStream()))
                    {
                        soapResult = rd.ReadToEnd();
                    }
                }

                var document = new XmlDocument();
                document.LoadXml(soapResult);

                var customers = JsonConvert.DeserializeObject<JsonCustomersModel>(document.InnerText, new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" });                


                    //elimino i customer non più presenti nella vip area

                    var dbCustomers = _userManagerService.GetAllCustomers();
                    var customerNames = customers.customers.Select(j => j.username).Distinct();
                    var custmerToRemove = dbCustomers.Where(e => !customerNames.Contains(e.Username)).ToList();

                    if (custmerToRemove.Any())
                    {
                        //rimuovo prima le associazioni
                        var ids = custmerToRemove.Select(a => a.ID).ToList();
                        var names = custmerToRemove.Select(a => a.Username).ToList();
                        var us = _context.Set<UserCustomerMapping>().Where(uc => names.Contains(uc.CustomerName)).ToList();

                        //utenti associati al customer
                        var usCust = new List<UserModel>();
                        foreach (var item in us)
                        {
                            usCust.AddRange(_userManagerService.GetUsers(item.CustomerName));
                        }

                        if (us.Any())
                        {
                            _context.Set<UserCustomerMapping>().RemoveRange(us);
                            _context.SaveChanges();
                        }

                        var um = _context.Set<UserMachineMapping>().Where(mh => ids.Contains(mh.UserId)).ToList();
                        if (um.Any())
                        {
                            _context.Set<UserMachineMapping>().RemoveRange(um);
                            _context.SaveChanges();
                        }

                        usCust.AddRange(custmerToRemove);
                        using (var transactionSuppress = new TransactionScope(TransactionScopeOption.Suppress))
                        {
                            _userManagerService.RemoveUsers(usCust);
                            transactionSuppress.Complete();
                        }
                    }
                    //pulizia della tabella UserCustomerMapping, potrebbero esserci record inseriti a mano con customerName non esistenti
                    
                    foreach (var customer in customers.customers)
                    {
                        try
                        {
                            //Aggiungo eventuali nuovi clienti
                            var user = new UserModel();
                            using (var transactionSuppress = new TransactionScope(TransactionScopeOption.Suppress))
                            {
                                user = _userManagerService.GetUser(customer.username);
                                if (user == null)
                                {
                                    user = new UserModel
                                    {
                                        Username = customer.username,
                                        FirstName = customer.username,
                                        LastName = customer.username,
                                        Enabled = true,
                                        Role = enRole.Customer,
                                        CustomerName = customer.username
                                    };
                                    user.ID = _userManagerService.CreateUser(user);
                                }
                                transactionSuppress.Complete();
                            }

                            //Aggiungo eventuali nuovi clienti nel DB dei dati
                            var userCustomer = _context.Set<UserCustomerMapping>().FirstOrDefault(f => f.UserId == user.ID);
                            if (userCustomer == null)
                            {
                                userCustomer = new UserCustomerMapping
                                {
                                    UserId = user.ID,
                                    CustomerName = user.Username
                                };
                                _context.Set<UserCustomerMapping>().Add(userCustomer);
                                _context.SaveChanges();
                            }

                            //Prendo la lista delle macchine esistenti nel DB a partire da quelle arrivate da JSON
                            var machinesSerial = customer.machines.Select(s => s.serial).ToList();
                            var machines = _context.Set<Machine>().Where(w => machinesSerial.Contains(w.Serial)).ToList();

                            //Rimuovo le associazioni cliente <=> macchina per macchine non più monitorate
                            var machinesId = machines.Select(s => s.Id).ToList();
                            var clientUsersMachines = _context.Set<UserMachineMapping>().Where(w => machinesId.Contains(w.MachineId)).Select(s => s.UserId).Distinct().ToList();
                            var usersMachinesToRemove = _context.Set<UserMachineMapping>().Where(w => !machinesId.Contains(w.MachineId) && clientUsersMachines.Contains(w.UserId)).ToList();
                            _context.Set<UserMachineMapping>().RemoveRange(usersMachinesToRemove);
                            _context.SaveChanges();
                            var plant = _mesService.GetOrSetPlantDefaultByUser(user.ID);

                            //Inserisco i nuovi mapping cliente <=> macchina
                            foreach (var machine in machines)
                            {
                                var jm = customer.machines.FirstOrDefault(f => f.serial == machine.Serial);
                                if (jm != null)
                                {
                                    var expirationDate = jm.expirationDate;
                                    var activationDate = jm.activationDate;
                                    var machineName = jm.machineName;
                                    var usersMachineMapped = _context.Set<UserMachineMapping>().Where(w => w.MachineId == machine.Id).ToList();
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
                                        var userMachine = new UserMachineMapping()
                                        {
                                            //ExpirationDate = expirationDate,
                                            //ActivationDate = activationDate,
                                            MachineId = machine.Id,
                                            UserId = user.ID
                                        };
                                        _context.Set<UserMachineMapping>().Add(userMachine);
                                        _context.SaveChanges();
                                    }
                                    //aggiorno l'activationDate della macchina prendendo la più vecchia
                                    // aggiorno anche il plantId
                                    var ma = _context.Set<Machine>().Find(machine.Id);
                                    if(ma.PlantId == null)
                                    {
                                        ma.PlantId = plant;
                                    }
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
                                    _context.SaveChanges();
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
            catch (Exception ex)
            {
                LogService.WriteLog(ex.Message, LogService.TypeLevel.Error, ex);
            }

            return result;
        }

    }
}
