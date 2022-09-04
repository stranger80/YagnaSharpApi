using Golem.Compose.Engine;
using Golem.Compose.Model.Yaml;
using System.Dynamic;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Golem.Compose
{
    static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            var deserializer = new ModelDeserializer();

            var myApp = deserializer.Deserialize(File.ReadAllText("sample-app.yaml"));


            Console.WriteLine(myApp.Name);
            


        }
    }
}
