using FomMonitoringCore.DAL;
using FomMonitoringCore.Framework.Model;
using System.Collections.Generic;
using System.Data.Entity;

namespace FomMonitoringCore.Service.DataMapping
{
    public interface IJsonToSQLiteService
    {
        List<JsonDataModel> GetAllJsonDataNotElaborated();
        bool MappingJsonDetailsToSQLite(JsonDataModel jsonDataModel);
        bool MappingJsonHistoryToSQLite(JsonDataModel jsonDataModel);
        bool SaveElaboration(int id, bool IsLoaded);
    }
}