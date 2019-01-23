namespace FomMonitoringCore.Service.API
{
    public interface IJwtManager
    {
        string GenerateToken(string machineSerial, int expireMinutes = 2);
    }
}
