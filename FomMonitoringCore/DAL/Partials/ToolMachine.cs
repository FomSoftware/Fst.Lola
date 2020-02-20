namespace FomMonitoringCore.DAL
{
    public partial class ToolMachine
    {
        public int CodeAsInt {
            get {
            if(int.TryParse(this.Code, out var x))
                return x;
            return 0;
            }
        }
    }
}
