using FomMonitoringCore.DAL;
using FomMonitoringCore.Service;
using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FomMonitoringCore.Framework.Common
{
    public class ReadMessages
    {
        static string dbConn = ApplicationSettingService.GetWebConfigKey("DbMessagesConnectionString");

        public static bool ReadMessageVisibility(MessageMachine mm, FST_FomMonitoringEntities ent)
        {
            bool result = false;
            int cat = ent.Machine.Find(mm.MachineId).MachineModel.MessageCategoryId;
            MessagesIndex msg = ent.MessagesIndex.FirstOrDefault(f => f.MessageCode == mm.Code && f.MessageCategoryId == cat);
            result = msg == null ? false : msg.IsVisibileLOLA;

            return result;
        }

        public static string GetMessageDescription(string code, int machineId, string parameters, string language)
        {
            var result = "";
            

            using (var ent = new FST_FomMonitoringEntities())
            {
                var cat = ent.Machine.Find(machineId)?.MachineModel?.MessageCategoryId;

                if (!(cat > 0))
                    return string.Empty;

                var la = ent.MessageLanguages.FirstOrDefault(lan => lan.DotNetCulture.StartsWith(language));
                if (la == null)
                    return string.Empty;

                var languageId = la.Id;

                result = ent.MessageTranslation.FirstOrDefault(t => t.MessageLanguageId == languageId && t.MessagesIndex.MessageCode == code && t.MessagesIndex.MessageCategoryId == cat)?.Translation;

                if (result == null)
                    return string.Empty;
            }
            
            return result;
        }       

        public static string ReplaceFirstOccurrence(string source, string find, string replace)
        {
            int place = source.IndexOf(find);
            string result = source.Remove(place, find.Length).Insert(place, replace);

            return result;
        }

    }
}
