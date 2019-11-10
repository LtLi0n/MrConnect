using LionLibrary.Framework;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LionLibrary.Commands
{
    public class CommandModule : IModuleContainer
    {
        private readonly ILogService _logger;
        private readonly IServiceProvider _services;

        public string FullPath { get; internal set; }
        public string Name { get; internal set; }

        public ICommandService CommandService { get; }
        public Type ModuleType { get; }
        //Properties are stored for dependency injection.
        public ReadOnlyCollection<PropertyInfo> Properties { get; }
        public CommandModule ParentModule { get; internal set; }
        internal Dictionary<string, CommandModule> _modules;
        public IReadOnlyDictionary<string, CommandModule> Modules => _modules;
        public ReadOnlyDictionary<string, Command> Commands { get; private set; }

        public Command this[string command]
        {
            get
            {
                Commands.TryGetValue(CommandService.IgnoreCase ? command.ToLower() : command, out Command cmd);
                return cmd;
            }
        }

        internal CommandModule(ICommandService commandService, Type module_type, ILogService logger, IServiceProvider services)
        {
            CommandService = commandService;
            ModuleType = module_type;
            _logger = logger;
            _services = services;

            //Properties
            Properties = new ReadOnlyCollection<PropertyInfo>(ModuleType.GetProperties(
                BindingFlags.Public |
                BindingFlags.Instance |
                BindingFlags.GetProperty |
                BindingFlags.SetProperty));
        }

        ///<summary>Checks if children module dictionary contains a supplied module name. Note: childrens' modules will not be checked recursively.</summary>
        ///<param name="module">relative module name to this container.</param>
        public bool ContainsModule(string module) => Modules.ContainsKey(CommandService.IgnoreCase ? module.ToLower() : module);

        ///<summary>Returns module names sorted in ascending order.</summary>
        public IEnumerable<string> ModuleNames =>
            Modules
            .Select(x => x.Key)
            .OrderBy(x => x);

        ///<summary>Returns every command and their representitive description within this module container.</summary>
        public IEnumerable<ICommandIdentity> CommandsInfo => Commands.Select(x => x.Value);

        public bool HasCommand(string cmd) => Commands.ContainsKey(CommandService.IgnoreCase ? cmd.ToLower() : cmd);

        private static IList<string> GetModulePath(Type module_type)
        {
            ModuleAttribute module_attr = module_type.GetCustomAttribute<ModuleAttribute>();
            if (module_attr == null) return null;
            else
            {
                List<string> module_paths = new List<string>();
                ModuleAttribute tracked_attr = module_attr;
                Type tracked_path_type = module_type;

                while (true)
                {
                    if (tracked_attr.ParentModule != null)
                    {
                        module_paths.Add(tracked_attr.Name);
                        tracked_path_type = tracked_attr.ParentModule;
                        tracked_attr = GetModuleAttribute(tracked_path_type);
                        if (tracked_attr == null)
                        {
                            throw new ArgumentNullException("Cannot bind to a parent type that does not contain a ModuleAttribute attribute.");
                        }
                    }
                    else if ((tracked_attr = GetModuleAttribute(tracked_path_type)) != null)
                    {
                        module_paths.Add(tracked_attr.Name);
                        tracked_path_type = tracked_path_type.DeclaringType;
                    }
                    else break;
                }

                //Construct and return full module path.
                module_paths.Reverse();
                return module_paths;
            }
        }

        ///<summary> Installs every command within this module. </summary>
        ///<param name="module_type">Type which inherits from ModuleBase.</param>
        internal static async Task<CommandModule> InstallModuleAsync(
            ICommandService cService,
            Type type,
            ILogService logger,
            IServiceProvider services,
            Dictionary<string, CommandModule> modules,
            Dictionary<Type, CommandModule> confirmed_types,
            Dictionary<Type, IList<Type>> queued_types,
            CommandModule parent_module = null)
        {
            if (LionLibrary.Commands.CommandService.HasModuleBase(type))
            {
                CommandModule cm = new CommandModule(cService, type, logger, services);

                //Module Name
                ModuleAttribute module_attr = type.GetCustomAttribute<ModuleAttribute>();
                if (module_attr != null)
                {
                    cm.Name = cService.IgnoreCase ? module_attr.Name.ToLower() : module_attr.Name;
                    cm.FullPath = string.Join(cService.ModuleSeperator, GetModulePath(type));
                    //Parent module
                    if (parent_module == null)
                    {
                        if (module_attr.ParentModule != null)
                        {
                            if (queued_types.ContainsKey(module_attr.ParentModule))
                            {
                                queued_types[module_attr.ParentModule].Add(type);
                            }
                            else
                            {
                                queued_types.Add(module_attr.ParentModule, new List<Type> { type });
                            }
                            return null;
                        }
                    }
                    else
                    {
                        cm.ParentModule = parent_module;
                        parent_module._modules.Add(cm.Name, cm);
                    }
                }
                else
                {
                    return null;
                }

                Dictionary<string, Command> cmds = new Dictionary<string, Command>();
                IEnumerable<MethodInfo> methods = type.GetMethods().Where(x => x.GetCustomAttribute<CommandAttribute>() != null);

                //create commands
                foreach (MethodInfo method in methods)
                {
                    //Todo: Make hidden commands going forward
                    if (method.IsPublic)
                    {
                        CommandBuilder cb = new CommandBuilder(cm, method, logger, cService, services);
                        Command cmd = new Command(cb);

                        cmds.Add(cService.IgnoreCase ? cmd.Name.ToLower() : cmd.Name, cmd);
                    }
                }
                cm.Commands = new ReadOnlyDictionary<string, Command>(cmds);

                confirmed_types.Add(type, cm);
                logger?.LogLine(cm, $"Module: '{string.Join(cService.ModuleSeperator, GetModulePath(type))}' loaded.", LogSeverity.Verbose);

                //recursively install inner types
                Type[] nested_types = type.GetNestedTypes();
                Dictionary<string, CommandModule> nested_modules = new Dictionary<string, CommandModule>();

                foreach (Type nested_type in nested_types)
                {
                    CommandModule nested_module = await InstallModuleAsync(cService, nested_type, logger, services, modules, confirmed_types, queued_types);
                    if (nested_module == null) continue;
                    nested_module.ParentModule = cm;
                    nested_modules.Add(nested_module.Name, nested_module);
                }

                cm._modules = new Dictionary<string, CommandModule>(nested_modules);
                return cm;
            }

            return null;
        }

        internal static ModuleAttribute GetModuleAttribute(Type t)
        {
            if (t != null)
            {
                if (LionLibrary.Commands.CommandService.HasModuleBase(t))
                {
                    ModuleAttribute module_attr = t.GetCustomAttribute<ModuleAttribute>();
                    return module_attr;
                }
                else return null;
            }
            return null;
        }
    }
}
