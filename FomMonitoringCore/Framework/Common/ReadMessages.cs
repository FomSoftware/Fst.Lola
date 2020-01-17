using FomMonitoringCore.DAL;
using FomMonitoringCore.Repository;
using FomMonitoringCore.Service;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FomMonitoringCore.Framework.Common
{
    public class ReadMessages : IReadMessages
    {
        private readonly IMessageLanguagesRepository _messageLanguagesRepository;
        private readonly IMessageTranslationRepository _messageTranslationRepository;
        private readonly IMachineGroupRepository _machineGroupRepository;
        private readonly IMachineRepository _machineRepository;
        private readonly IMessagesIndexRepository _messagesIndexRepository;

        public ReadMessages(
            IMessageLanguagesRepository messageLanguagesRepository,
            IMessagesIndexRepository messagesIndexRepository,
            IMessageTranslationRepository messageTranslationRepository,
            IMachineGroupRepository machineGroupRepository,
            IMachineRepository machineRepository)
        {
            _messageLanguagesRepository = messageLanguagesRepository;
            _messageTranslationRepository = messageTranslationRepository;
            _machineGroupRepository = machineGroupRepository;
            _machineRepository = machineRepository;
            _messagesIndexRepository = messagesIndexRepository;
        }
        //public static void ReadMessageVisibilityGroup(MessageMachine mm, MessagesIndex msg)
        //{
        //    var result = msg?.IsVisibleLOLA ?? false;
        //    mm.IsVisible = result;

        //    if(mm.Group == null)
        //    {
        //        if (msg != null)
        //            mm.Group = msg.MachineGroupId;
        //    }
        //    if(msg != null)
        //        mm.Type = msg.MessageTypeId.ToString();

        //}       

        //public string GetMessageDescription(MessageMachine msg, string language)
        //{

        //        var result = "";
                

        //        var la = _messageLanguagesRepository.GetFirstOrDefault(lan => lan.DotNetCulture.StartsWith(language));
        //        if (la == null)
        //            return string.Empty;

        //        var languageId = la.Id;


        //        result = msg.MessagesIndex.MessageTranslation.FirstOrDefault(t => t.MessageLanguageId == languageId)?.Translation;


        //        if (result == null)
        //            return string.Empty;


        //        if (string.IsNullOrEmpty(msg.Params))
        //            return result;

        //        var parDict =
        //            JsonConvert.DeserializeObject<Dictionary<string, string>>(msg.Params);


        //        return parDict.Keys.Aggregate(result, (current, key) => current.Replace(key, parDict[key]));

        //}

        public string GetMessageGroup(string code, int machineId, int? jsonGroupId)
        {

                MachineGroup mg;

                if (jsonGroupId != null && jsonGroupId != 0)
                {
                    mg = _machineGroupRepository.GetByID(jsonGroupId);
                }
                else
                {
                    var cat = _machineRepository.GetByID(machineId).MachineModel.MessageCategoryId;

                    if (!(cat > 0))
                        return string.Empty;

                    var msg = _messagesIndexRepository.GetByCodeCategory(code, cat);
                    if (msg == null)
                        return string.Empty;

                    if (msg.MachineGroup == null)
                        return string.Empty;

                    mg = _machineGroupRepository.GetByID(msg.MachineGroup.Id);
                }

                if (mg == null)
                    return string.Empty;

                return mg.MachineGroupName;

            
            
        }

        public int? GetMessageType(string code, int machineId)
        {
            try
            {
                var cat = _machineRepository.GetByID(machineId).MachineModel.MessageCategoryId;

                if (!(cat > 0))
                    return null;

                var msg = _messagesIndexRepository.GetByCodeCategory(code, cat);

                return msg?.MessageType?.Id;
            }
            catch (Exception e)
            {
                Debugger.Break();
            }

            return null;
        }

        public string ReplaceFirstOccurrence(string source, string find, string replace)
        {
            var place = source.IndexOf(find);
            var result = source.Remove(place, find.Length).Insert(place, replace);

            return result;
        }

    }
}
