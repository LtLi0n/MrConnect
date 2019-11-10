using System;

namespace LionLibrary.Commands
{
    public class CommandAttribute : Attribute
    {
        public string Name { get; }

        ///<param name="name">Command name.</param>
        public CommandAttribute(string name)
        {
            Name = name;
        }
    }
}
