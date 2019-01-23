using Mapster;

namespace FomMonitoring.App_Start
{
    public class MapsterConfig : FomMonitoringCore.Framework.Config.MapsterConfig
    {
        public new void Register(TypeAdapterConfig config)
        {
            base.Register(config);
        }
    }
}