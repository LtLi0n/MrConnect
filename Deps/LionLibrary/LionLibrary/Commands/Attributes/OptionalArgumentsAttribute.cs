using System;
using System.Collections.ObjectModel;

namespace LionLibrary.Commands
{
    public class OptionalArgumentsAttribute : Attribute
    {
        public ReadOnlyCollection<string> Args { get; }
        public OptionalArgumentsAttribute(params string[] args) => Args = new ReadOnlyCollection<string>(args);
    }
}
