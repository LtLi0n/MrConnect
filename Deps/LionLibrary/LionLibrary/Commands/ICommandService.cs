using System.Reflection;
using System.Threading.Tasks;
using System.Collections.Generic;
using LionLibrary.Network;
using System;

namespace LionLibrary.Commands
{
    public interface ICommandService : IModuleContainer
    {
        CommandModule this[string module] { get; }
        CommandPacket this[Packet packet] { get; }

        string[] CacheTags { get; }
        bool IgnoreCase { get; }
        string ModuleSeperator { get; set; }
        string CommandSeperator { get; set; }

        void AddCachedObject(string id, object obj);
        object GetCachedObject(string id);
        void RemoveCachedObject(string id);

        Task InstallCommandsAsync(IEnumerable<TypeInfo> types, IServiceProvider services);
    }
}