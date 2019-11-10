using System;

namespace LionLibrary.Commands
{
    public class ModuleAttribute : Attribute
    {
        public string Name { get; }
        ///<summary>Specify parent module here if you don't want to define module children in the same file.
        public Type ParentModule { get; set; }

        ///<param name="Name">Module name.</param>
        public ModuleAttribute(string Name)
        {
            this.Name = Name;
        }
    }
}
