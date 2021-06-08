using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NJsonSchema;

namespace ForwarderTest
{
    class Program
    {
        static void Main(string[] args)
        {
            string json =
                "{\"tool\": [{\"Id\": 1,\"Code\": 1,\"Description\": \"Mill D.5 Pos.11\",\"DateLoaded\": \"2021-06-07T11:35:09.75425Z\",\"DateReplaced\": \"\",\"CurrentLife\": 476.0,\"ExpectedLife\": 360000.0},{\"Id\": 2,\"Code\": 2,\"Description\": \"Mill D.8 Pos.12\",\"DateLoaded\": \"2021-06-07T11:35:10.75425Z\",\"DateReplaced\": \"\",\"CurrentLife\": 911.0,\"ExpectedLife\": 360000.0}]";
            json +=
                ",\"info\": [{\"KeyId\": \"2021563044\",\"MachineSerial\": \"B0900001\",\"MachineCode\": 211,\"Product\": \"FSTLine4\",\"ProductVersion\": \"4.2.0.198\",\"FirmwareVersion\": \"1.7.2 - 8/11/2019\",\"PlcVersion\": \"1.6.3.0\",\"LoginDate\": \"2021-06-07T11:35:09.75425Z\",\"UTC\": 0}]}";


            var schemaDataTool = JsonSchema.FromFileAsync(Path.Combine("C:\\JsonSchemas", "tool.json")).Result;
            var errorTool = schemaDataTool.Validate(json);
        }
    }
}
