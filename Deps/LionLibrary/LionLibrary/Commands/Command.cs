using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Linq;
using LionLibrary.Framework;

namespace LionLibrary.Commands
{
    public class Command : ICommandIdentity
    {
        public CommandModule Module { get; }
        public string Name { get; }
        ///<summary>Command path.</summary>
        public string Header => $"{Module.FullPath}{_cService.CommandSeperator}{Name}";
        public IReadOnlyCollection<string> Alias { get; }
        public string Description { get; }

        public IReadOnlyCollection<string> ExpectedOptionalArgs { get; }
        public IReadOnlyCollection<string> ExpectedMandatoryArgs { get; }
        public MethodInfo Method { get; }

        private readonly Func<ModuleBase, CommandContext, Task> _callback;
        private readonly ILogService _logger;
        private readonly IServiceProvider _services;
        private readonly ICommandService _cService;

        internal Command(CommandBuilder cb)
        {
            Module = cb.Module;
            Name = cb.Name;
            Alias = cb.Alias;
            Description = cb.Description;
            ExpectedOptionalArgs = cb.PossibleOptionalArgs;
            ExpectedMandatoryArgs = cb.PossibleMandatoryArgs;
            Method = cb.Method;
            _callback = cb.Callback;
            _logger = cb.LogService;
            _services = cb.Services;
            _cService = cb.CommandService;
        }

        public async Task ExecuteAsync(SocketCommandContext context) =>
            await ExecuteAsyncFinal((ModuleBase)Activator.CreateInstance(Method.DeclaringType), context, context.Args);

        ///<summary>The default context will be used. Use if you only inherit from a default ModuleBase, the non-generic version.</summary>
        public async Task ExecuteAsync(IDictionary<string, string> args) =>
            await ExecuteAsyncFinal((ModuleBase)Activator.CreateInstance(Method.DeclaringType), new CommandContext(), new ReadOnlyDictionary<string, string>(args));

        public async Task ExecuteAsync<T>(T context, IDictionary<string, string> args) where T : CommandContext =>
            await ExecuteAsyncFinal((ModuleBase<T>)Activator.CreateInstance(Method.DeclaringType), context, new ReadOnlyDictionary<string, string>(args));

        public async Task ExecuteAsync<T>(T context, IReadOnlyDictionary<string, string> args) where T : CommandContext =>
            await ExecuteAsyncFinal((ModuleBase<T>)Activator.CreateInstance(Method.DeclaringType), context, args);

        private async Task ExecuteAsyncFinal(ModuleBase instance, CommandContext context, IReadOnlyDictionary<string, string> args)
        {
            context.Module = Module;
            context.Command = this;

            instance.Context = context;
            instance.Logger = _logger;
            
            if(_services != null)
            {
                instance.Services = _services;

                //dependency injection
                foreach (PropertyInfo pi in Module.Properties)
                {
                    object service = _services.GetService(pi.PropertyType);
                    if (service != null)
                    {
                        pi.SetValue(instance, service);
                    }
                }
            }

            //arguments
            {
                foreach (var arg in ExpectedMandatoryArgs)
                {
                    if (!args.ContainsKey(arg))
                    {
                        string ex_toReturn = "You need to supply mandatory arguments. For this command, those are:\n" + string.Join(", ", ExpectedMandatoryArgs);

                        //find still missing args
                        var still_missing = ExpectedMandatoryArgs.ToList();
                        var supplied_mandatory = args.Where(x => still_missing.Contains(x.Key)).Select(x => x.Key).ToList();
                        still_missing.RemoveAll(x => supplied_mandatory.Contains(x));

                        ex_toReturn += "\n\nYou are still missing these args:\n" + string.Join(", ", still_missing);
                        throw new ManagedCommandException(ex_toReturn);
                    }
                }

                instance.Args = args;
            }

            try
            {
                await _callback(instance, context);
            }
            catch (Exception ex)
            {
                _logger?.LogLine(this, ex.Message, ex is ManagedCommandException ? LogSeverity.Error : LogSeverity.Critical);
                throw ex;
            }
        }
    }
}
