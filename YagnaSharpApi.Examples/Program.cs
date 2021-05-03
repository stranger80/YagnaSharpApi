using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YagnaSharpApi.Engine;
using YagnaSharpApi.Engine.Commands;
using YagnaSharpApi.Utils;

namespace YagnaSharpApi.Examples
{


    class Program
    {
        static void Main(string[] args)
        {
            Task.WaitAll(Task.Run(async () => await BlenderExample.MainAsync(args[0])));
        }

    }
}
