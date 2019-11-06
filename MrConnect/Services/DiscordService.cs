using Discord;
using Discord.Commands;
using Discord.WebSocket;
using LionLibrary.Framework;
using MrConnect.Server.Boot;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace MrConnect.Server
{
    public class DiscordService
    {
        private readonly AppConfig _config;
        private readonly ILogService _logger;
        private readonly IServiceProvider _services;

        public string Prefix => _config.Prefix;

        public DiscordSocketClient Client { get; }
        public CommandService Commands { get; }

        private readonly Dictionary<ulong, string> _groupToggledUsers = new Dictionary<ulong, string>();
        public IReadOnlyDictionary<ulong, string> GroupToggledUsers => _groupToggledUsers;

        public DiscordService(IServiceProvider services, AppConfig config, ILogService logger)
        {
            _services = services;
            _config = config;
            _logger = logger;
            
            Commands = new CommandService(
                new CommandServiceConfig 
                { 
                    LogLevel = global::Discord.LogSeverity.Debug 
                });
            
            Client = new DiscordSocketClient(
                new DiscordSocketConfig
                {
                    LogLevel = global::Discord.LogSeverity.Debug
                });
        }

        public async Task InstallCommandsAsync()
        {
            Client.MessageReceived += HandleCommandAsync;
            Client.Log += Log;
            Commands.Log += Log;
            await Commands.AddModulesAsync(Assembly.GetEntryAssembly(), _services);
            //await Commands.AddModuleAsync<GroupModule>(_services);
            _logger?.LogLine(this, "Commands loaded.");
        }

        public async Task StartAsync()
        {
            await Client.LoginAsync(TokenType.Bot, _config.Token);
            await Client.StartAsync();
        }

        private Task Log(LogMessage message)
        {
            if (!string.IsNullOrEmpty(message.Message))
            {
                _logger.LogLine(this, message.Message, FromDiscordLogSeverity(message.Severity));
            }
            return Task.CompletedTask;
        }

        private LionLibrary.Framework.LogSeverity FromDiscordLogSeverity(global::Discord.LogSeverity logSeverity) =>
            logSeverity switch
            {
                global::Discord.LogSeverity.Critical => LionLibrary.Framework.LogSeverity.Critical,
                global::Discord.LogSeverity.Error => LionLibrary.Framework.LogSeverity.Error,
                global::Discord.LogSeverity.Warning => LionLibrary.Framework.LogSeverity.Warning,
                global::Discord.LogSeverity.Info => LionLibrary.Framework.LogSeverity.Info,
                global::Discord.LogSeverity.Verbose => LionLibrary.Framework.LogSeverity.Verbose,
                global::Discord.LogSeverity.Debug => LionLibrary.Framework.LogSeverity.Debug,
                _ => LionLibrary.Framework.LogSeverity.Info
            };

        private async Task HandleCommandAsync(SocketMessage msg)
        {
            // Don't process the command if it was a system message
            SocketUserMessage message = msg as SocketUserMessage;
            if (message == null) return;

            // Create a number to track where the prefix ends and the command begins
            int argPos = 0;

            // Determine if the message is a command based on the prefix and make sure no bots trigger commands
            if (!(message.HasStringPrefix(_config.Prefix, ref argPos) ||
                message.HasMentionPrefix(Client.CurrentUser, ref argPos)) ||
                message.Author.IsBot)
            {
                return;
            }

            // Create a WebSocket-based command context based on the message
            var context = new SocketCommandContext(Client, message);

            bool groupToggleUser = false;

            if(_groupToggledUsers.ContainsKey(message.Author.Id))
            {
                groupToggleUser = true;
                string shortcut = _groupToggledUsers[message.Author.Id];
                string input = shortcut + message.Content.Substring(argPos);

                if(Commands.Search(input).IsSuccess)
                {
                    var result = await Commands.ExecuteAsync(
                        context: context,
                        input: input,
                        services: _services);

                    if (!result.IsSuccess)
                    {
                        await context.Channel.SendMessageAsync(result.ErrorReason);
                    }

                    return;
                }
            }

            var result_default = await Commands.ExecuteAsync(
                context: context,
                argPos: argPos,
                services: _services);

            if (!result_default.IsSuccess)
            {
                await context.Channel.SendMessageAsync(result_default.ErrorReason);
            }
        }

        public void AddGroupToggledUser(IUser user, string path)
        {
            if(_groupToggledUsers.ContainsKey(user.Id))
            {
                _groupToggledUsers[user.Id] = path + ' ';
            }
            else
            {
                _groupToggledUsers.Add(user.Id, path + ' ');
            }
        }
    }
}
