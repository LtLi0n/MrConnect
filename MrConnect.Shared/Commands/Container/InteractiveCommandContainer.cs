using System;
using System.Collections.Generic;

namespace MrConnect.Shared
{
    ///<summary>For storing interactive commands.</summary>
    /// <typeparam name="T"></typeparam>
    public class InteractiveCommandContainer<T>
        where T : InteractiveCommandContext
    {
        internal Dictionary<Type, InteractiveCommand<T>> _commands;
        internal HashSet<InteractiveCommandBuilder<T>> _builders;

        public InteractiveCommandContainer()
        {
            _commands = new Dictionary<Type, InteractiveCommand<T>>();
            _builders = new HashSet<InteractiveCommandBuilder<T>>();
        }

        ///<summary>Makes it very easy to inject builders within ServiceCollection.</summary>
        public InteractiveCommandContainer<T> WithBuilders(Action<InteractiveCommandContainerBuildActionContext<T>> builderAction)
        {
            var builder = new InteractiveCommandContainerBuildActionContext<T>();
            builderAction(builder);
            builder.InjectInto(this);
            return this;
        }

        public void Build()
        {
            foreach (var builder in _builders)
            {
                _commands.Add(builder.GetType(), builder.Build());
            }

            _builders.Clear();
        }

        public InteractiveCommand<T> GetByBuilder<U>() 
            where U : InteractiveCommandBuilder<T> =>
            _commands.TryGetValue(typeof(U), out InteractiveCommand<T> cmd) ? cmd : null;
    }
}
