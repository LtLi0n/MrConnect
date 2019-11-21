using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace MrConnect.Shared
{
    public class InteractiveCommand<T> where T : InteractiveCommandContext
    {
        public string Name { get; }
        public string Path { get; }

        public InteractiveCommandNode<T> RootNode { get; }
        public ReadOnlyCollection<InteractiveCommandNode<T>> Nodes { get; }

        public InteractiveCommand(
            string name,
            string path,
            InteractiveCommandNode<T> rootNode,
            IEnumerable<InteractiveCommandNode<T>> nodes)
        {
            Name = name;
            Path = path;
            RootNode = rootNode;
            Nodes = nodes.ToList().AsReadOnly();
        }
    }
}
