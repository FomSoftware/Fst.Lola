using System.ServiceProcess;

namespace FomMonitoringQueueConsumerService
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            var servicesToRun = new ServiceBase[]
            {
                new LolaQueueConsumer()
            };
            ServiceBase.Run(servicesToRun);
        }
    }
}
