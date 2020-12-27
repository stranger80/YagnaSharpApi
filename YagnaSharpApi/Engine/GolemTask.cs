using System;
using System.Collections.Generic;
using System.Text;

namespace YagnaSharpApi.Engine
{
    public class GolemTask<TData, TResult>
    {
        public GolemTask(TData data, DateTime? expires = null, int timeout = 0)
        {

        }

        public TData Data { get; protected set; }

        public TResult Output { get; protected set; }

        public void AcceptTask(TResult result)
        {
            this.Output = result;
        }

        public void RejectTask(string reason, bool retry = false)
        {

        }
    }
}
