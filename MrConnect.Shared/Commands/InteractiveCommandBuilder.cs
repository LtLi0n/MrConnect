using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace MrConnect.Shared
{
    public class InteractiveCommandBuilder<T> where T : InteractiveCommandContext
    {
        public string Name { get; }
        public string Path { get; }

        private readonly InteractiveCommandNodeBuilder<T> _initialNodeBuilder;
        private InteractiveCommandNode<T> _initialNode;

        ///<summary>Store builder object for a regex link reference</summary>
        private Dictionary<InteractiveCommandNodeBuilder<T>, InteractiveCommandNode<T>> LinkReadyNodes { get; }

        public InteractiveCommandBuilder(string commandName, string commandPath, Action<InteractiveCommandNodeBuilder<T>> builderAction)
        {
            Name = commandName;
            Path = commandPath;
            LinkReadyNodes = new Dictionary<InteractiveCommandNodeBuilder<T>, InteractiveCommandNode<T>>();

            _initialNodeBuilder = new InteractiveCommandNodeBuilder<T>(Name);
            builderAction(_initialNodeBuilder);

            RegisterBuilders(_initialNodeBuilder);
        }

        ///<summary>Recursively add node builders to <see cref="LinkReadyNodes"/> for further processing.</summary>
        /// <param name="builder"></param>
        private void RegisterBuilders(InteractiveCommandNodeBuilder<T> builder)
        {
            if (!LinkReadyNodes.ContainsKey(builder))
            {
                LinkReadyNodes.Add(builder, null);

                foreach (var link in builder.RegexLinks)
                {
                    RegisterBuilders(link.Item2);
                }
            }
        }

        private void BuildNodes(InteractiveCommandNode<T> parent, InteractiveCommandNodeBuilder<T> target)
        {
            var node = new InteractiveCommandNode<T>(parent, target);
            LinkReadyNodes[target] = node;

            foreach (var link in target.RegexLinks)
            {
                BuildNodes(node, link.Item2);
            }

            if (parent == null)
            {
                _initialNode = node;
            }
        }

        ///<summary>This method should be called only after BuildNodes has been called.</summary>
        private void AttachNodeRegexLinks()
        {
            foreach(var (builder, node) in LinkReadyNodes)
            {
                List<(string, InteractiveCommandNode<T>)> links = new List<(string, InteractiveCommandNode<T>)>(builder.RegexLinks.Count);

                foreach(var (regex, linkBuilderNode) in builder.RegexLinks)
                {
                    links.Add((regex, LinkReadyNodes[linkBuilderNode]));
                }

                node.RegexLinks = new ReadOnlyCollection<(string, InteractiveCommandNode<T>)>(links);
            }
        }

        private void BindCommandWithNodes(InteractiveCommand<T> command)
        {
            foreach(var node in LinkReadyNodes.Values)
            {
                node.Command = command;
            }
        }

        public InteractiveCommand<T> Build()
        {
            BuildNodes(null, _initialNodeBuilder);
            AttachNodeRegexLinks();
            var command = new InteractiveCommand<T>(Name, Path, _initialNode, LinkReadyNodes.Values);
            BindCommandWithNodes(command);
            return command;
        }

        public static InteractiveCommandBuilder<T> TestCommand { get; } =
            new InteractiveCommandBuilder<T>("test", "something:somethinger", x =>
            {
                x.WithContent(
                    "1) settings\n" +
                    "2) info");

                x.WithLink("settings", "1", x =>
                {
                    x.WithContent(
                        "1) guild\n" +
                        "2) user");

                    x.WithLink("guild", "1", x =>
                    {
                        x.WithContent(
                        "1) prefix");

                        x.WithEntryExecution(() =>
                        {
                            Console.WriteLine("made it.");
                        });

                        x.WithLink("prefix", "1", x =>
                        {
                            x.WithCallback(context => context.Content switch
                            {
                                "kill" => InteractiveCommandCallbackInstruction.Exit_Success,
                                "crash" => InteractiveCommandCallbackInstruction.Exit_Error,
                                "pass" => InteractiveCommandCallbackInstruction.Continue
                            });
                        });
                    });
                });

                x.WithLink("info", "2", x =>
                {
                    x.WithContent("nothing interesting.");
                });
            });
    }
}
