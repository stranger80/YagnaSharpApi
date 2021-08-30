using Golem.ActivityApi.Client.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YagnaSharpApi.Engine
{
    public class ExeScriptBuilder
    {
        private List<ExeScriptCommand> commands = new List<ExeScriptCommand>();

        public List<ExeScriptCommand> GetCommands()
        {
            return commands;
        }

        public int Deploy()
        {
            commands.Add(
                new DeployCommand(new object())
                );

            return commands.Count - 1;
        }

        public int Start(params string[] args)
        {
            commands.Add(
                new StartCommand(new StartCommandBody(args.ToList()))
                );

            return commands.Count - 1;
        }

        public int Transfer(string from, string to)
        {
            commands.Add(
                new TransferCommand(
                    new TransferCommandBody(from, to)
                    )
                );

            return commands.Count - 1;
        }

        public int Run(string entryPoint, params string[] args)
        {
            commands.Add(
                new RunCommand(
                    new RunCommandBody(entryPoint, args.ToList(), new Capture()  // default settings of stdout/stderr
                    {
                        Stdout = new CaptureMode()
                        {
                            Stream = new CaptureStreamBody()
                        },
                        Stderr = new CaptureMode()
                        {
                            Stream = new CaptureStreamBody()
                        }
                    })
                    )
                );

            return commands.Count - 1;
        }

    }
}
