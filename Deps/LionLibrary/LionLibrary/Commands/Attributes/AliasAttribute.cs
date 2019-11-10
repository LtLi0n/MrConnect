using System;

namespace LionLibrary.Commands
{
    public class AliasAttribute : Attribute
    {
        public string[] Alias { get; }
        public AliasAttribute(params string[] alias) => Alias = alias;
    }
}
