using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Golem.Compose.Model.Yaml
{
    public class Payload
    {
        public string? Runtime { get; set; }
        public string[]? Constraints { get; set; }
        public string[]? Capabilities { get; set; }
        public IDictionary<string, string> Params { get; set; }
    }
}
