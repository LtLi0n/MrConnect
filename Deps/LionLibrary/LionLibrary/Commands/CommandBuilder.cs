using LionLibrary.Framework;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace LionLibrary.Commands
{
    internal class CommandBuilder
    {
        internal CommandModule Module { get; }
        internal MethodInfo Method { get; }
        internal ILogService LogService { get; }
        internal Func<ModuleBase, CommandContext, Task> Callback { get; }
        internal string[] PossibleMandatoryArgs { get; }
        internal string[] PossibleOptionalArgs { get; }
        internal string Name { get; }
        internal string[] Alias { get; }
        internal string Description { get; }
        internal IServiceProvider Services { get; }
        internal ICommandService CommandService { get; }

        private async Task ExecuteCallback<T1, T2>(T1 instance, T2 context) 
            where T1 : ModuleBase
            where T2 : CommandContext
        {
            instance.Context = context;

            try
            {
                await (Method.Invoke(instance, null) as Task);
            }
            finally
            {
                (instance as IDisposable)?.Dispose();
            }
        }

        internal CommandBuilder(CommandModule module, MethodInfo mi, ILogService logger, ICommandService commands, IServiceProvider services)
        {
            Module = module;
            Method = mi;
            LogService = logger;
            Services = services;
            CommandService = commands;

            //command name
            CommandAttribute cmd_attr = Method.GetCustomAttribute<CommandAttribute>();
            Name = cmd_attr.Name;

            //alias
            AliasAttribute alias = Method.GetCustomAttribute<AliasAttribute>();
            if (alias != null) Alias = alias.Alias;

            //args
            {
                //optional
                {
                    OptionalArgumentsAttribute args_attr = Method.GetCustomAttribute<OptionalArgumentsAttribute>();
                    int arg_n = args_attr == null ? 0 : args_attr.Args.Count;
                    PossibleOptionalArgs = new string[arg_n];
                    for (int i = 0; i < arg_n; i++)
                    {
                        PossibleOptionalArgs[i] = args_attr.Args[i];
                    }
                }
                //mandatory
                {
                    MandatoryArgumentsAttribute args_attr = Method.GetCustomAttribute<MandatoryArgumentsAttribute>();
                    int arg_n = args_attr == null ? 0 : args_attr.Args.Count;
                    PossibleMandatoryArgs = new string[arg_n];
                    for (int i = 0; i < arg_n; i++)
                    {
                        PossibleMandatoryArgs[i] = args_attr.Args[i];
                    }
                }

                if(PossibleOptionalArgs.Intersect(PossibleMandatoryArgs).Count() > 0)
                {
                    throw new InvalidOperationException($"Optional and Mandatory args cannot be same. At {Module}:{Name}");
                }
            }

            //description
            DescriptionAttribute desc = Method.GetCustomAttribute<DescriptionAttribute>();
            if (desc != null) Description = desc.Description;

            Callback = ExecuteCallback;
        }
    }
}
