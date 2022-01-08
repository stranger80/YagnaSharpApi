using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using YagnaSharpApi.Engine;
using YagnaSharpApi.Engine.Commands;
using YagnaSharpApi.Engine.Events;
using YagnaSharpApi.Engine.MarketStrategy;
using YagnaSharpApi.Examples;
using YagnaSharpApi.Mapper;
using YagnaSharpApi.Repository;
using YagnaSharpApi.Tests.Services;
using YagnaSharpApi.Utils;

namespace YagnaSharpApi.Tests
{
    [TestClass]
    public class ExampleTests
    {

        static ExampleTests()
        {
            MapConfig.Init();
        }

        protected async Task RunExampleAsync<TExample>() where TExample : IGolemExample, new()
        {
            var example = new TExample();

            await example.RunExampleAsync();
        }


        [TestMethod]
        public async Task Example_HelloExampleRuns()
        {
            await RunExampleAsync<HelloExample>();

            Assert.IsTrue(true);
        }

    }
}
