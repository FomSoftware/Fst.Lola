using FomMonitoringCore.DAL;
using FomMonitoringCore.Framework.Model;
using System;

namespace FomMonitoringCore.Service.DataMapping
{
    public interface IJsonVariantsToSqlServerService
    {
        bool MappingJsonParametersToSqlServer(JsonDataModel jsonDataModel);

        void CheckVariableTresholds(Machine machine,
                        ParameterMachine par, JsonVariableValueModel value, decimal? oldValue, DateTime utcDatetime);

    }
}