namespace FomMonitoringCore.Framework.Common
{
    public interface IReadMessages
    {
        string GetMessageDescription(string code, int machineId, string parameters, string language);
        string GetMessageGroup(string code, int machineId, int? jsonGroupId);
        int? GetMessageType(string code, int machineId);
        string ReplaceFirstOccurrence(string source, string find, string replace);
    }
}