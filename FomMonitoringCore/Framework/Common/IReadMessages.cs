using FomMonitoringCore.DAL;
using FomMonitoringCore.Framework.Model;

namespace FomMonitoringCore.Framework.Common
{
    public interface IReadMessages
    {
        //string GetMessageDescription(MessageMachineModel msg, string language);
        string GetMessageGroup(string code, int machineId, int? jsonGroupId);
        int? GetMessageType(string code, int machineId);
        string ReplaceFirstOccurrence(string source, string find, string replace);
    }
}