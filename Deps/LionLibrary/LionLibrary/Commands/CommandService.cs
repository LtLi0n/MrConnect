using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Threading.Tasks;
using LionLibrary.Network;
using LionLibrary.Framework;
using System.Collections.ObjectModel;

namespace LionLibrary.Commands
{
    public class CommandService : ICommandService
    {
        private IServiceProvider _services;
        private readonly ILogService _logger;
        ///<summary>Cache for storing objects.</summary>
        private readonly Dictionary<string, object> _cache;

        public CommandModule this[string module]
        {
            get
            {
                if(IgnoreCase) module = module.ToLower();
                Modules.TryGetValue(module, out CommandModule module_val);
                return module_val;
            }
        }

        public CommandPacket this[Packet packet]
        {
            get
            {
                string[] route = packet.Module.Split(ModuleSeperator);

                CommandModule cm = null;

                for(int i = 0; i < route.Length; i++)
                {
                    if(i == 0)
                    {
                        if (!ContainsModule(route[i])) return null;
                        cm = Modules[route[i]];
                    }
                    else
                    {
                        if(cm.Modules.ContainsKey(route[i]))
                        {
                            cm = cm.Modules[route[i]];
                        }
                        else
                        {
                            return null;
                        }
                    }
                }

                if (cm.HasCommand(packet.Command))
                {
                    return new CommandPacket(cm[packet.Command], packet);
                }
                else
                {
                    return null;
                }
            }
        }

        public string[] CacheTags => _cache.Keys.ToArray();
        public IReadOnlyDictionary<string, CommandModule> Modules { get; private set; }

        ///<summary>Returns module names sorted in ascending order.</summary>
        public IEnumerable<string> ModuleNames => 
            Modules
            .Select(x => x.Key)
            .OrderBy(x => x);

        public bool IgnoreCase { get; }
        ///<summary>Adds a seperator when there are multiple modules need to be seperated. By default it's ':'</summary>
        public string ModuleSeperator { get; set; } = ":";
        ///<summary>A command seperator that follows a command name. Example [my_module:and_child_module.add_object]. By defalt it's '.'</summary>
        public string CommandSeperator { get; set; } = ".";

        public CommandService(bool ignoreCase = false, ILogService logger = null)
        {
            IgnoreCase = ignoreCase;
            _logger = logger;
            _cache = new Dictionary<string, object>();
            ModuleSeperator = ":";
        }

        ///<summary>Checks if the module dictionary contains a supplied module name. Note: nested modules will not be searched.</summary>
        ///<param name="module">relative module name to this container.</param>
        public bool ContainsModule(string module) => Modules.ContainsKey(IgnoreCase ? module.ToLower() : module);

        #region Cache
        ///<summary>Add an object into the cache. id = to create identity for the object.</summary>
        public void AddCachedObject(string id, object obj)
        {
            if (!_cache.ContainsKey(id)) _cache.Add(id, obj);
            else throw new Exception($"Object with id {id} already exists. Id has to be unique.");
        }
        ///<summary>Remove an object from the cache. id = the object identity. Must contain such id or will throw an exception.</summary>
        public void RemoveCachedObject(string id)
        {
            if (_cache.ContainsKey(id)) _cache.Remove(id);
            else throw new Exception($"Object with id {id} doesn't exist.");
        }
        ///<summary>Receive an object from the cache. id = the object identity. Must contain such id or will throw an exception.</summary>
        public object GetCachedObject(string id) => _cache[id];
        #endregion Cache

        ///<summary>Attempts to load all commands.</summary>
        public async Task InstallCommandsAsync(Assembly assembly, IServiceProvider services = null) =>
            await InstallCommandsAsync(assembly.DefinedTypes, services);

        ///<summary>Attempts to load all commands.</summary>
        public async Task InstallCommandsAsync(IEnumerable<TypeInfo> types, IServiceProvider services = null)
        {
            Dictionary<string, CommandModule> modules = new Dictionary<string, CommandModule>();

            _services = services;
            Dictionary<Type, CommandModule> confirmed_types = new Dictionary<Type, CommandModule>();
            Dictionary<Type, IList<Type>> queued_types = new Dictionary<Type, IList<Type>>();

            foreach (Type type in types)
            {
                if (confirmed_types.ContainsKey(type)) continue;
                if(HasModuleBase(type))
                {
                    CommandModule module = await CommandModule.InstallModuleAsync(this, type, _logger, _services, modules, confirmed_types, queued_types);
                    if (module == null) continue;
                    if(module.ParentModule == null)
                    {
                        modules.Add(module.Name, module);
                    }
                }
            }

            //Install queued types that have parent modules
            while (queued_types.Count > 0)
            {
                foreach(var queued_t in queued_types)
                {
                    if(confirmed_types.ContainsKey(queued_t.Key))
                    {
                        List<Type> installed_types = new List<Type>();

                        foreach(var queued_inner_t in queued_t.Value)
                        {
                            if (HasModuleBase(queued_inner_t))
                            {
                                CommandModule module = await CommandModule.InstallModuleAsync(
                                    this, 
                                    queued_inner_t, 
                                    _logger, 
                                    _services, 
                                    modules, 
                                    confirmed_types, 
                                    queued_types, 
                                    confirmed_types[queued_t.Key]);

                                if (module == null) continue;
                                installed_types.Add(queued_inner_t);
                            }
                        }

                        //remove installed types that were queued
                        foreach (Type t in installed_types)
                        {
                            queued_t.Value.Remove(t);
                        }

                        if (queued_t.Value.Count == 0)
                        {
                            queued_types.Remove(queued_t.Key);
                        }
                    }
                }
            }

            Modules = new ReadOnlyDictionary<string, CommandModule>(modules);
            _logger?.LogLine(this, $"Commands loaded.", LogSeverity.Info);
        }

        ///<summary>Recursive module base inheritance search.</summary>
        static internal bool HasModuleBase(Type type) => type.BaseType != null ? (type.BaseType == typeof(ModuleBase) ? true : HasModuleBase(type.BaseType)) : false;
    }
}
