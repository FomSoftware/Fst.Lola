namespace FomMonitoringCore.Service.DataMapping
{
    public interface ISQLiteToSQLServerService
    {
        bool MappingSQLiteDetailsToSQLServer();
        bool MappingSQLiteHistoryToSQLServer();
    }
}