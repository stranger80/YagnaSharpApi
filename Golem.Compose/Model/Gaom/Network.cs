using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Golem.Compose.Model.Gaom
{
    public class Network
    {
        public string Name { get; set; }

        public ResourceState State { get; set; }

        public string Ip { get; set; }
    }
}
