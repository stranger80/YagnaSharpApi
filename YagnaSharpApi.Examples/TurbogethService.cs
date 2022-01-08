using System;
using System.Collections.Generic;
using System.Threading;
using YagnaSharpApi.Engine;
using YagnaSharpApi.Engine.Commands;
using YagnaSharpApi.Utils;

namespace YagnaSharpApi.Examples
{
    public class TurbogethService : ServiceBase
    {
        private bool disposedValue;

        public string rpcEndpointUrl { get; private set; }

        public TurbogethService() : base()
        {
        }

        /// <summary>
        /// Indicate payload specification.
        /// </summary>
        /// <returns></returns>
        public override IPackage GetPayload()
        {
            return new TurbogethPayload();
        }

        /// <summary>
        /// Define actions to be taken on startup of the Turbogeth service:
        /// - Deploy and extract the RPC endpoint URL.
        /// - Start
        /// </summary>
        /// <param name="ctx"></param>
        /// <returns></returns>
        public async override IAsyncEnumerable<Script> OnStartupAsync(WorkContext ctx)
        {
            var script = ctx.NewScript();
            var deployStep = script.Deploy();
            script.Start();

            yield return script; 

            var deployResult = await deployStep;

            this.rpcEndpointUrl = deployResult.Stdout; // just as simplistic example, in reality Deploy would return a JSON which would need ot be parsed
        }

        /// <summary>
        /// Define actions to be executed while the service is up.
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="cancellationToken">CancellationToken injected by the framework - 
        /// to indicate the Run has been interrupted.</param>
        /// <returns></returns>
        public async override IAsyncEnumerable<Script> OnRunAsync(WorkContext ctx, CancellationToken cancellationToken)
        {
            var gethClient = new GethClient(this.rpcEndpointUrl);

            while(!cancellationToken.IsCancellationRequested)
            {
                // do periodic Turbogeth status check
                // do periodic Activity accrued cost check

            }
            yield break;
        }

        /// <summary>
        /// Specify actions to be taken on service shutdown.
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public async override IAsyncEnumerable<Script> OnShutdownAsync(WorkContext ctx, Exception error = null)
        {
            // do nothing
            yield break;
        }
    }
}
