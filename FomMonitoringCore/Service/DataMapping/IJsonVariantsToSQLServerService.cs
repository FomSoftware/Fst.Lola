using FomMonitoringCore.DAL;
using FomMonitoringCore.Framework.Model;
using System;

namespace FomMonitoringCore.Service.DataMapping
{
    public interface IJsonVariantsToSQLServerService
    {
        bool MappingJsonVariantsToSQLite(JsonDataModel jsonDataModel);

        void checkVariableTresholds(Machine machine,
                        ParameterMachine par, JsonVariableValueModel value, decimal? oldValue, DateTime utcDatetime);

    }
}