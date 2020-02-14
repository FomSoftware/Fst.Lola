namespace FomMonitoringCore.Service.DataMapping
{
    public interface ISqLiteToSqlServerService
    {
        bool MappingSqLiteDetailsToSqlServer();
        bool MappingSqLiteHistoryToSqlServer();
    }
}