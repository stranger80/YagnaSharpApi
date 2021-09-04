using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YagnaSharpApi.Engine;
using YagnaSharpApi.Engine.Commands;
using YagnaSharpApi.Utils;

namespace YagnaSharpApi.Examples
{


    class TurbogethExample
    {
        public static async Task MainAsync(string subnetTag)
        { 
            using (var executor = new Engine.Executor(
                null, 
                3, 
                10.0m,
                0, 
                subnetTag))
            {
                await foreach(var service in executor.RunServiceAsync<TurbogethService>())
                {
                    // Service launched
                }
            }
        }
    }
}
