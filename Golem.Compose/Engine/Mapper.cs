using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Golem.Compose.Engine
{
    public class Mapper
    {
        public Model.Gaom.Application MapApplication(Model.Yaml.Application yamlApp)
        {
            var result = new Model.Gaom.Application();

            result.Name = yamlApp?.Meta?.Name;
            result.Description = yamlApp?.Meta?.Description;

            foreach(var payloadName in yamlApp?.Payloads.Keys)
            {
                result.Payloads.Add(payloadName, this.MapPayload(result, payloadName, yamlApp?.Payloads[payloadName]));
            }

            foreach (var networkName in yamlApp?.Networks.Keys)
            {
                result.Networks.Add(networkName, this.MapNetwork(result, networkName, yamlApp?.Networks[networkName]));
            }

            foreach (var serviceName in yamlApp?.Services.Keys)
            {
                result.Services.Add(serviceName, this.MapService(result, serviceName, yamlApp?.Services[serviceName]));
            }


            return result;
        }

        public Model.Gaom.Payload MapPayload(Model.Gaom.Application app, string payloadName, Model.Yaml.Payload yamlPayload)
        {
            var result = new Model.Gaom.Payload()
            {
                Name = payloadName,
                Capabilities = yamlPayload.Capabilities,
                Constraints = yamlPayload.Constraints,
                Runtime = yamlPayload.Runtime,  
                Params = new Dictionary<string, string>(yamlPayload.Params)
            };

            return result;
        }

        public Model.Gaom.Network MapNetwork(Model.Gaom.Application app, string networkName, Model.Yaml.Network yamlNetwork)
        {
            var result = new Model.Gaom.Network()
            {
                Name = networkName,
                Ip = yamlNetwork.Ip,
                State = Model.Gaom.ResourceState.Pending
            };

            return result;
        }

        public Model.Gaom.Service MapService(Model.Gaom.Application app, string serviceName, Model.Yaml.Service yamlService)
        {
            var result = new Model.Gaom.Service()
            {
                Name = serviceName,
                State = Model.Gaom.ResourceState.Pending,
                Payload = app.Payloads[yamlService?.Payload],
                Network = app.Networks[yamlService?.Network],
                DependsOn = new List<Model.Gaom.Service>(),
                Entrypoint = new List<Model.Gaom.ExeScriptCommand>()
            };

            foreach(var yamlCommand in yamlService.Entrypoint)
            {
                result.Entrypoint.Add(this.MapCommand(yamlCommand));
            }

            return result;
        }

        public Model.Gaom.ExeScriptCommand MapCommand(Model.Yaml.ExeScriptCommand yamlCommand)
        {
            if(yamlCommand.Run != null)
            {
                return new Model.Gaom.RunCommand() { Args = yamlCommand.Run.Args.ToList() };
            }

            return null;
        }

    }




}
