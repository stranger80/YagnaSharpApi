using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Golem.Compose.Model.Gaom
{
    public class Application
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public IDictionary<string, Payload> Payloads { get; set; } = new Dictionary<string, Payload>();
        public IDictionary<string, Network> Networks { get; set; } = new Dictionary<string , Network>();
        public IDictionary<string, Service> Services { get; set; } = new Dictionary<string, Service>();
    }
}
