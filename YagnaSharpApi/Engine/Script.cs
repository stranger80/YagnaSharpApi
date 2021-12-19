using Golem.ActivityApi.Client.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using YagnaSharpApi.Engine.Commands;

namespace YagnaSharpApi.Engine
{
    public class Script
    {
        protected WorkContext ctx;
        protected List<Command> commands = new List<Command>();

        public Script(WorkContext ctx)
        {
            this.ctx = ctx;
        }

        public void Add(Command command)
        {
            this.commands.Add(command);
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
            this.Add(command);

            return command;
        }

        public InitStep Init()
        {
            var command = new InitStep();
            this.Add(command);

            return command;
        }

        public IndexedCommand Deploy()
        {
            var command = new DeployStep();
            this.Add(command);

            return command;
        }

        public IndexedCommand Start()
        {
            var command = new StartStep();
            this.Add(command);

            return command;
        }

        public IndexedCommand SendFile(string srcPath, string destPath)
        {
            var command = new SendFile(this.ctx.Storage, srcPath, destPath);
            this.Add(command);

            return command;

        }

        public IndexedCommand SendJson(object data, string destPath)
        {
            var command = new SendJson(this.ctx.Storage, data, destPath);
            this.Add(command);

            return command;

        }

        public IndexedCommand DownloadFile(string srcPath, string destPath)
        {
            var command = new RecvFile(this.ctx.Storage, srcPath, destPath);
            this.Add(command);

            return command;

        }


    }
}
