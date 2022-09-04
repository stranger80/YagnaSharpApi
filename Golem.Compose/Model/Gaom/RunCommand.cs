using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Golem.Compose.Model.Gaom
{
    public class RunCommand : ExeScriptCommand
    {
        public IList<string> Args { get; set; } = new List<string>();
    }
}
