using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using FomMonitoringCore.DAL;
using FomMonitoringCore.DAL_SQLite;
using FomMonitoringCore.Framework.Common;
using FomMonitoringCore.Framework.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace FomMonitoringCore.Service.DataMapping
{
    public class JsonVariantsToSQLServerService
    {
        public static bool MappingJsonVariantsToSQLite(JsonDataModel jsonDataModel)
        {
            bool result = false;
            List<info> infoSQLite = new List<info>();
            try
            {
                JObject json = JsonConvert.DeserializeObject<JObject>(jsonDataModel.Json);
                List<JsonVariablesModel> variableList = new List<JsonVariablesModel>();
                List<ParameterMachineValue> parameterList = new List<ParameterMachineValue>();

                using (TransactionScope transaction = new TransactionScope())
                {
                    using (FST_FomMonitoringEntities ent = new FST_FomMonitoringEntities())
                    {
                       JToken tt = json.Root.FirstOrDefault(n => n.Path.ToLower() == "info");
                       
                       info ii = JsonConvert.DeserializeObject<List<info>>(JsonConvert.SerializeObject(tt.First)).FirstOrDefault();

                       Machine mac = ent.Machine.FirstOrDefault(m => m.Serial == ii.MachineSerial);

                        foreach (JToken token in json.Root)
                        {
                            switch (token.Path.ToLower())
                            {                                
                                case "variableslist":
                                    variableList = JsonConvert.DeserializeObject<List<JsonVariablesModel>>(JsonConvert.SerializeObject(token.First));
                                    foreach (JsonVariablesModel var in variableList)
                                    {
                                        var.UtcDateTime = (var.UtcDateTime.Year < 1900) ? System.DateTime.Now : var.UtcDateTime; 
                                        if(var.Values != null && var.Values.Count > 0)
                                        {
                                            foreach(JsonVariableValueModel value in var.Values)
                                            {
                                                ParameterMachine pm = ent.ParameterMachine.FirstOrDefault(p => p.MachineModelId == mac.MachineModelId
                                                    && p.VarNumber == value.VariableNumber);

                                                //ordino per data e poi per id perchè spesso arrivano valori diversi con la stessa data
                                                decimal? previousValue = ent.ParameterMachineValue.Where(p => p.MachineId == mac.Id && p.VarNumber == pm.VarNumber)
                                                    .OrderByDescending(p => p.UtcDateTime).ThenByDescending(t => t.Id).FirstOrDefault()?.VarValue;


                                                if (mac != null && pm != null && (pm.Historicized == null || pm.Historicized == "1"))
                                                {
                                                    ParameterMachineValue pmv = new ParameterMachineValue()
                                                    {
                                                        MachineId = mac.Id,
                                                        ParameterMachineId = pm.Id,
                                                        UtcDateTime = var.UtcDateTime,
                                                        VarNumber = value.VariableNumber,
                                                        VarValue = value.VariableValue
                                                    };
                                                    ent.ParameterMachineValue.Add(pmv);
                                                }
                                                else if(mac != null && pm != null && pm.Historicized == "0")
                                                {
                                                    ParameterMachineValue pmv = ent.ParameterMachineValue.FirstOrDefault(pp => pp.MachineId == mac.Id
                                                        && pp.VarNumber == value.VariableNumber);
                                                    if(pmv != null)
                                                    {
                                                        pmv.UtcDateTime = var.UtcDateTime;
                                                        pmv.VarValue = value.VariableValue;
                                                    }
                                                    else
                                                    {
                                                        pmv = new ParameterMachineValue()
                                                        {
                                                            MachineId = mac.Id,
                                                            ParameterMachineId = pm.Id,
                                                            UtcDateTime = var.UtcDateTime,
                                                            VarNumber = value.VariableNumber,
                                                            VarValue = value.VariableValue
                                                        };
                                                        ent.ParameterMachineValue.Add(pmv);                                                       
                                                    }
                                                    
                                                }
                                                ent.SaveChanges();

                                                if (!String.IsNullOrEmpty(pm.ThresholdMax) && pm.ThresholdMax != "0" ||
                                                    !String.IsNullOrEmpty(pm.ThresholdMin) && pm.ThresholdMin != "0")
                                                {
                                                    checkVariableTresholds(ent, mac, pm, value, previousValue, var.UtcDateTime );
                                                }
                                            }
                                        }
                                    }
                                    break;                                                           
                                default:
                                    break;
                            }
                        }
                        //ent.ParameterMachineValue.AddRange(parameterList);                             

                        transaction.Complete();
                        result = true;
                    }
                }
            }
            catch (Exception ex)
            {
                string errMessage = string.Format(ex.GetStringLog(), jsonDataModel.Id.ToString());
                LogService.WriteLog(errMessage, LogService.TypeLevel.Error, ex);
            }
            return result;
        }


        private static void checkVariableTresholds(FST_FomMonitoringEntities ent, Machine machine, 
                        ParameterMachine par, JsonVariableValueModel value, decimal? oldValue, DateTime utcDatetime)
        {
            if (ent == null || machine == null || par == null || utcDatetime == null || value == null)
                return;
            if (oldValue != null && oldValue >= value.VariableValue)
                return;

            //controllo se il valore oltrepassa la soglia e non esiste già un msessaggio lo inserisco                                      
            decimal min = Convert.ToDecimal(par.ThresholdMin);
            decimal max = Convert.ToDecimal(par.ThresholdMax);

            if (value.VariableValue < min || value.VariableValue > max)
            {                   
                MessageMachine mes = ent.MessageMachine.Where(mm => mm.MachineId == machine.Id && mm.IsPeriodicMsg == true &&
                                        mm.Code == par.ThresholdLabel).FirstOrDefault();
                if (mes == null)
                {
                    MessageService.insertMessageMachine(ent, machine, par.ThresholdLabel, utcDatetime);
                }
                else if (oldValue != null)
                {
                    if (mes.Day < utcDatetime)
                    {
                        //verifico se il precedente valore era sotto la soglia inserisco un nuovo messaggio
                        if (oldValue >= min && oldValue <= max)
                        {
                            MessageService.insertMessageMachine(ent, machine, par.ThresholdLabel, utcDatetime);
                        }
                        //in questo caso ero sopra la soglia e continuo ad essere sopra la soglia
                        // controllo se il valore del parametro ha superato il prossimo multiplo del valore max
                        else if (oldValue != null && oldValue != null)
                        {                                                               
                                decimal valOld = oldValue ?? 0;
                                decimal valNew = value.VariableValue;
                                if (valOld > max && valNew > valOld)
                                {
                                    decimal multiploOld = Math.Floor(valOld / max);
                                    decimal multiploNew = Math.Floor(valNew / max);
                                    if (multiploNew > multiploOld)
                                        MessageService.insertMessageMachine(ent, machine, par.ThresholdLabel, utcDatetime);
                                }
                        }

                    }
                }
            }                

        }

    }

}
