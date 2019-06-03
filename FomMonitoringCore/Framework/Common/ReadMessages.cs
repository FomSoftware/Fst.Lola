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
            string result = "";
            

            using (FST_FomMonitoringEntities ent = new FST_FomMonitoringEntities())
            {
                int cat = ent.Machine.Find(machineId).MachineModel.MessageCategoryId;

                if (cat == 0) return "";

                MessageLanguages la = ent.MessageLanguages.Where(lan => lan.DotNetCulture.StartsWith(language)).FirstOrDefault();
                if (la == null) return "";
                int languageId = la.Id;

                result = (from e in ent.MessageTranslation.Where(t => t.MessageLanguageId == languageId)
                join a in ent.MessagesIndex.Where(m => m.MessageCode == code && m.MessageCategoryId == cat)
                    on e.MessageId equals a.Id
                select e.Translation).FirstOrDefault();

                if (result == null) return "";               
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
