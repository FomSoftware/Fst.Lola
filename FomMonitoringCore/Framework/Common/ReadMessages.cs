using FomMonitoringCore.DAL;
using FomMonitoringCore.Service;
using Newtonsoft.Json;
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

        public static void ReadMessageVisibilityGroup(MessageMachine mm, MessagesIndex msg)
        {
            bool result = false;            
            result = msg == null ? false : msg.IsVisibleLOLA;
            mm.IsVisible = result;

            if(mm.Group == null)
            {
                mm.Group = msg.MachineGroupId;
            }
            if(msg != null)
                mm.Type = msg.MessageTypeId.ToString();

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

            if (string.IsNullOrEmpty(parameters))                
                return result;
            try
            {
                Dictionary<string, string> parDict = JsonConvert.DeserializeObject<Dictionary<string, string>>(parameters);

                foreach (string key in parDict.Keys)
                {
                    result = result.Replace(key, parDict[key]);
                }
            }
            catch(Exception ex)
            { }
                   
            
            return result;
        }

        public static string GetMessageGroup(string code, int machineId, int? jsonGroupId)
        {
            using (var ent = new FST_FomMonitoringEntities())
            {
                MachineGroup mg;

                if (jsonGroupId != null && jsonGroupId != 0)
                {
                    mg = ent.MachineGroup.Find(jsonGroupId);
                }
                else
                {
                    var cat = ent.Machine.Find(machineId)?.MachineModel?.MessageCategoryId;

                    if (!(cat > 0))
                        return string.Empty;

                    var msg = ent.MessagesIndex.FirstOrDefault(mi => mi.MessageCode == code && mi.MessageCategoryId == cat);
                    if (msg == null)
                        return string.Empty;

                    if (msg.MachineGroup == null)
                        return string.Empty;

                    mg = ent.MachineGroup.Find(msg.MachineGroup.Id);
                }

                if (mg == null)
                    return string.Empty;

                return mg.MachineGroupName;

            }
            
        }

        public static int? GetMessageType(string code, int machineId)
        {            
            using (var ent = new FST_FomMonitoringEntities())
            {
                var cat = ent.Machine.Find(machineId)?.MachineModel?.MessageCategoryId;

                if (!(cat > 0))
                    return null;

                var msg = ent.MessagesIndex.FirstOrDefault(mi => mi.MessageCode == code && mi.MessageCategoryId == cat);
                if (msg == null)
                    return null;

                if (msg.MessageType == null)
                    return null;

                return msg.MessageType.Id;
            }        
        }

        public static string ReplaceFirstOccurrence(string source, string find, string replace)
        {
            int place = source.IndexOf(find);
            string result = source.Remove(place, find.Length).Insert(place, replace);

            return result;
        }

    }
}
