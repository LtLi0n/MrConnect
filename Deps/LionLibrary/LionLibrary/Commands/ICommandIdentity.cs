using System.Collections.Generic;

namespace LionLibrary.Commands
{
    public interface ICommandIdentity
    {
        string Name { get; }
        IReadOnlyCollection<string> Alias { get; }
        string Description { get; }
        IReadOnlyCollection<string> ExpectedMandatoryArgs { get; }
        IReadOnlyCollection<string> ExpectedOptionalArgs { get; }
    }
}
