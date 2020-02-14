using FomMonitoringCore.DAL;
using FomMonitoringCore.Framework.Model;
using System.Collections.Generic;
using System.Data.Entity;

namespace FomMonitoringCore.Service.DataMapping
{
    public interface IJsonToSqLiteService
    {
        List<JsonDataModel> GetAllJsonDataNotElaborated();
        bool MappingJsonDetailsToSqLite(JsonDataModel jsonDataModel);
        bool MappingJsonHistoryToSqLite(JsonDataModel jsonDataModel);
        bool SaveElaboration(int id, bool isLoaded);
    }
}