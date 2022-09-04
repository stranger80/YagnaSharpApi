using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Golem.Compose.Model.Gaom
{
    public class Payload
    {
        public string Name { get; set; }
        public string Runtime { get; set; }
        public IList<string> Constraints { get; set; } = new List<string>();
        public IList<string> Capabilities { get; set; } = new List<string>();
        public IDictionary<string, string> Params { get; set; } = new Dictionary<string, string>();
    }
}
