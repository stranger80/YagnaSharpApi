using Golem.Compose.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Golem.Compose.Engine
{
    public class ModelDeserializer
    {
        private IDeserializer YamlDeserializer { get; set; }

        private Mapper Mapper { get; set; }

        public ModelDeserializer()
        {
            this.YamlDeserializer = new DeserializerBuilder()
                        .WithNamingConvention(CamelCaseNamingConvention.Instance)
                        .Build();

            this.Mapper = new Mapper();

        }

        public Model.Gaom.Application Deserialize(string yaml)
        {
            var yamlModel = this.YamlDeserializer.Deserialize<Model.Yaml.Application>(yaml);

            var application = this.Mapper.MapApplication(yamlModel);

            return application;

        }

    }
}
