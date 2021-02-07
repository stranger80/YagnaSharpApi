using System;
using System.Collections.Generic;
using System.Text;

namespace YagnaSharpApi.Engine
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TData">type of golem task input data</typeparam>
    /// <typeparam name="TResult">type of golem task result data</typeparam>
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
