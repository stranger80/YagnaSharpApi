using Golem.ActivityApi.Client.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace YagnaSharpApi.Exceptions
{
    public class CommandExecutionException : Exception
    {
        public ExeScriptCommand Command { get; private set; }
        public ExeScriptCommandResult Result { get; private set; }

        public CommandExecutionException(ExeScriptCommand command, ExeScriptCommandResult result) 
            : base($"Command Execution Error: {result.Stderr}")
        {
            this.Command = command;
            this.Result = result;
        }
    }
}
