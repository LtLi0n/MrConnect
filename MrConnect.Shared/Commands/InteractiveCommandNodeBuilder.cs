using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;
using System.Threading.Tasks;

namespace MrConnect.Shared
{
    public class InteractiveCommandNodeBuilder<T> where T : InteractiveCommandContext
    {
        public string Tag { get; }
        public InteractiveCommandNodeBuilder<T>? Parent { get; }

        private bool _contentSatisfied = false;
        
        public string Text { get; private set; }
        public SharedDiscordEmbed Embed { get; private set; }

        internal List<(string, InteractiveCommandNodeBuilder<T>)> RegexLinks { get; } = new List<(string, InteractiveCommandNodeBuilder<T>)>();
        internal Func<T, InteractiveCommandCallbackInstruction> CallbackFunc { get; set; }
        internal Action NodeEntryAction { get; set; }
        
        internal InteractiveCommandNodeBuilder(InteractiveCommandNodeBuilder<T> parent, string tag)
        {
            Parent = parent;
            Tag = tag;
        }

        internal InteractiveCommandNodeBuilder(string tag)
        {
            Parent = null;
            Tag = tag;
        }

        #region Regex-Links
        public InteractiveCommandNodeBuilder<T> WithLink(string tag, string regex, Action<InteractiveCommandNodeBuilder<T>> nextBuilderAction)
        {
            if (RegexLinks.Any(x => x.Item1 == regex))
            {
                throw new ArgumentException("This regex was already defined. Forcing this would result in an ignored link.");
            }

            var builder = new InteractiveCommandNodeBuilder<T>(this, tag);
            nextBuilderAction(builder);

            RegexLinks.Add((regex, builder));
            return builder;
        }

        public InteractiveCommandNodeBuilder<T> WithLink(string regex, InteractiveCommandNodeBuilder<T> nextBuilder)
        {
            RegexLinks.Add((regex, nextBuilder));
            return nextBuilder;
        }
        #endregion

        #region Content
        public InteractiveCommandNodeBuilder<T> WithContent(string text)
        {
            if(string.IsNullOrEmpty(text))
            {
                throw new ArgumentException("the text argument must not be empty or null.");
            }
            Text = text;
            _contentSatisfied = true;
            return this;
        }

        public InteractiveCommandNodeBuilder<T> WithContent(SharedDiscordEmbed embed)
        {
            Embed = embed ?? throw new ArgumentException("the embed argument must not be null.");
            _contentSatisfied = true;
            return this;
        }

        public InteractiveCommandNodeBuilder<T> WithContent(string text, SharedDiscordEmbed embed)
        {
            WithContent(text);
            return WithContent(embed);
        }
        #endregion

        #region AdditionalLogic
        public InteractiveCommandNodeBuilder<T> WithEntryExecution(Action entryAction)
        {
            NodeEntryAction = entryAction;
            return this;
        }

        public InteractiveCommandNodeBuilder<T> WithCallback(Func<T, InteractiveCommandCallbackInstruction> callbackFunc)
        {
            CallbackFunc = callbackFunc;
            return this;
        }
        #endregion
    }
}
