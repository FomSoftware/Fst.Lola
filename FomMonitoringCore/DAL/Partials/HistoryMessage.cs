using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace FomMonitoringCore.DAL
{
    public partial class HistoryMessage
    {
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