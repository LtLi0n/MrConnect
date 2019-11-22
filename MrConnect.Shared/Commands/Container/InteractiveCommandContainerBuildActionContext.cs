using System.Collections.Generic;

namespace MrConnect.Shared
{
    ///<summary>Only a builder for Build Action within <see cref="InteractiveCommandContainer{T}"./></summary>
    public class InteractiveCommandContainerBuildActionContext<T>
        where T : InteractiveCommandContext
    {
        private readonly HashSet<InteractiveCommandBuilder<T>> _builders;

        internal InteractiveCommandContainerBuildActionContext() 
        {
            _builders = new HashSet<InteractiveCommandBuilder<T>>();
        }

        public void Add<U>() 
            where U : InteractiveCommandBuilder<T>, new() => 
            _builders.Add(new U());

        internal void InjectInto(InteractiveCommandContainer<T> container)
        {
            foreach(var builder in _builders)
            {
                container._builders.Add(builder);
            }
        }
    }
}
