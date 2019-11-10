using System;
using System.Collections.ObjectModel;

namespace LionLibrary.Commands
{
    public class MandatoryArgumentsAttribute : Attribute
    {
        public ReadOnlyCollection<string> Args { get; }
        public MandatoryArgumentsAttribute(params string[] args) => Args = new ReadOnlyCollection<string>(args);
    }
}
