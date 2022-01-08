using Golem.ActivityApi.Client.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YagnaSharpApi.Engine.Commands;

namespace YagnaSharpApi.Engine
{
    public class Script
    {
        protected WorkContext ctx;
        protected List<Command> commands = new List<Command>();

        /// <summary>
        /// Indicates if this script contains initializing steps added explicitly.
        /// </summary>
        public bool IsInitialized { get; protected set; }

        public Script(WorkContext ctx)
        {
            this.ctx = ctx;
        }

        public void Prepend(Command command)
        {
            this.commands.Insert(0, command);

            if (command is DeployStep || command is StartStep || command is InitStep)
                this.IsInitialized = true;
        }

        public void Append(Command command)
        {
            this.commands.Add(command);

            if (command is DeployStep || command is StartStep || command is InitStep)
                this.IsInitialized = true;
        }

        public async Task BeforeAsync()
        {
            foreach (var command in this.commands)
                await command.BeforeAsync();
        }

        public async Task AfterAsync()
        {
            foreach (var command in this.commands)
                await command.AfterAsync();
        }

        public void Evaluate(ExeScriptBuilder builder)
        {
            foreach (var command in this.commands)
                command.Evaluate(builder);
        }

        public void StoreResult(ExeScriptCommandResult result)
        {
            foreach (var command in this.commands)
                command.StoreResult(result);
        }

        public IndexedCommand Run(string cmd, params string[] args)
        {
            var command = new Run(cmd, args);
            this.Append(command);

            return command;
        }

        public InitStep Init()
        {
            var command = new InitStep();
            this.Append(command);

            return command;
        }

        public IndexedCommand Deploy()
        {
            var command = new DeployStep();
            this.Append(command);

            return command;
        }

        public IndexedCommand Start()
        {
            var command = new StartStep();
            this.Append(command);

            return command;
        }

        public IndexedCommand SendFile(string srcPath, string destPath)
        {
            var command = new SendFile(this.ctx.Storage, srcPath, destPath);
            this.Append(command);

            return command;

        }

        public IndexedCommand SendJson(object data, string destPath)
        {
            var command = new SendJson(this.ctx.Storage, data, destPath);
            this.Append(command);

            return command;

        }

        public IndexedCommand DownloadFile(string srcPath, string destPath)
        {
            var command = new RecvFile(this.ctx.Storage, srcPath, destPath);
            this.Append(command);

            return command;

        }

        /// <summary>
        /// Retrieve results from all commands in script.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ExeScriptCommandResult> GetResults()
        {
            return this.commands.SelectMany(command => command.GetResults());
        }

    }
}
