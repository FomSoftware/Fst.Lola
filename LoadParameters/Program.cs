using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Autofac;
using FomMonitoringCore.Framework.Config;
using FomMonitoringCore.Framework.Model.Xml;
using FomMonitoringCore.Service.API;
using Mapster;

namespace LoadParameters
{
    class Program
    {
        private static IXmlDataService _xmlDataService;
        static void Main(string[] args)
        {
            TypeAdapterConfig.GlobalSettings.Scan(Assembly.GetAssembly(typeof(MapsterConfig)));
            var builder = new ContainerBuilder();
            FomMonitoringCore.Ioc.IocContainerBuilder.BuildCore(builder, false, true);
            var container = builder.Build();
            _xmlDataService = container.Resolve<IXmlDataService>();
            var path = args?.Length > 0 ? args[0] : null;
            while (string.IsNullOrWhiteSpace(path))
            {
                Console.Write("Inserire Path\n");
                path = Console.ReadLine();
            }

            var files = Directory.GetFiles(path);
            foreach (var file in files)
            {
                DeserializeObject(file);
            }

        }
        private static void DeserializeObject(string filename)
        {
            Console.WriteLine($"{filename} - Start");
            // Create an instance of the XmlSerializer.
            XmlSerializer serializer =
                new XmlSerializer(typeof(ParametersMachineModelXml));

            // Declare an object variable of the type to be deserialized.
            ParametersMachineModelXml i;

            using (Stream reader = new FileStream(filename, FileMode.Open))
            {
                // Call the Deserialize method to restore the object's state.
                i = (ParametersMachineModelXml)serializer.Deserialize(reader);
            }

            _xmlDataService.AddOrUpdateMachineParameterAsync(i).Wait();
            Console.WriteLine($"{filename} - End");
        }
    }
}
