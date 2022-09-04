using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Golem.Compose.Model.Yaml
{
    public class Application
    {
        public Meta Meta { get; set; }
        public IDictionary<string, Payload> Payloads { get; set; }
        public IDictionary<string, Network> Networks { get; set; }
        public IDictionary<string, Service> Services { get; set; }


    }
}
