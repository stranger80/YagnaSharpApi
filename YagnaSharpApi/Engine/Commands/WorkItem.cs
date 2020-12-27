using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace YagnaSharpApi.Engine.Commands
{
    public abstract class WorkItem
    {
        public virtual async Task Prepare()
        {

        }

        public abstract void Register(ExeScriptBuilder commands);

        public virtual async Task Post()
        {

        }
    }
}
