using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace MrConnect.Shared
{
    public class InteractiveCommandNode<T> where T : InteractiveCommandContext
    {
        public InteractiveCommand<T> Command { get; internal set; }
        public InteractiveCommandNode<T>? Parent { get; }

        public string Tag { get; }
        public string Text { get; }
        public SharedDiscordEmbed Embed { get; }

        public ReadOnlyCollection<(string, InteractiveCommandNode<T>)> RegexLinks { get; internal set; }

        internal Func<T, InteractiveCommandCallbackInstruction> CallbackFunc { get; set; }
        internal Action NodeEntryAction { get; set; }

        internal InteractiveCommandNode(
            InteractiveCommandNode<T> parent,
            InteractiveCommandNodeBuilder<T> builder)
        {
            Parent = parent;
            Tag = builder.Tag;
            Text = builder.Text;
            Embed = builder.Embed;
            NodeEntryAction = builder.NodeEntryAction;
            CallbackFunc = builder.CallbackFunc;
            RegexLinks = new ReadOnlyCollection<(string, InteractiveCommandNode<T>)>(new List<(string, InteractiveCommandNode<T>)>());
        }
    }
}
