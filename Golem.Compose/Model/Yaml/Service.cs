using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Golem.Compose.Model.Yaml
{
    public class Service
    {
        public string? Payload { get; set; }
        public string? Network { get; set; }
        public ExeScriptCommand[]? Entrypoint { get; set; }
    }
}
