using System.Collections.Generic;

namespace LionLibrary.Commands
{
    public interface IModuleContainer
    {
        IReadOnlyDictionary<string, CommandModule> Modules { get; }
        IEnumerable<string> ModuleNames { get; }

        ///<summary>Checks if the module dictionary contains a supplied module name. Note: nested modules will not be searched.</summary>
        ///<param name="module">relative module name to this container.</param>
        bool ContainsModule(string module);
    }
}
