using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace FomMonitoringCore.DAL
{
    public partial class MessageMachine
    {
       
        public DateTime? GetInitialSpanDate(long PeriodicSpan)
        {
            DateTime? result =  Machine.ActivationDate;
           
            if (IgnoreDate == null || PeriodicSpan == 0)
                return result;

            if (IgnoreDate != null)
                result = IgnoreDate;

            while ( result < DateTime.UtcNow)
            {
                DateTime? newInit = result?.AddHours(PeriodicSpan);

                if (newInit < DateTime.UtcNow)
                { 
                    result = result?.AddHours(PeriodicSpan);
                    if (result > IgnoreDate)
                        break;
                }
                else
                    break;
            }
            return result;
        }

        public string GetDescription(int idLanguage)
        {
            var result = MessagesIndex.MessageTranslation.FirstOrDefault(t => t.MessageLanguageId == idLanguage)?.Translation;


            if (result == null)
                return string.Empty;


            if (string.IsNullOrEmpty(Params))
                return result;

            var parDict =
                JsonConvert.DeserializeObject<Dictionary<string, string>>(Params);


            return parDict.Keys.Aggregate(result, (current, key) => current.Replace(key, parDict[key]));
        }

    }
}
