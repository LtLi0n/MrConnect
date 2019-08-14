using Discord;
using Discord.Commands;
using Discord.WebSocket;
using LionLibrary.Framework;
using LionLibrary.Network;
using MrConnect.Boot;
using SharedDiscord;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace MrConnect.Services
{
    public class DiscordService
    {
        private readonly AppConfig _config;
        private readonly ILogService _logger;
        private readonly IServiceProvider _services;
        private readonly DiscordConnector _discordConn;

        public DiscordSocketClient Client { get; }
        public CommandService Commands { get; }

        public DiscordService(IServiceProvider services, AppConfig config, ILogService logger, DiscordConnector discordConn)
        {
            _services = services;
            _config = config;
            _logger = logger;
            Client = new DiscordSocketClient(new DiscordSocketConfig { LogLevel = global::Discord.LogSeverity.Verbose });
            Commands = new CommandService();
            _discordConn = discordConn;
        }

        public async Task InstallCommandsAsync()
        {
            Client.MessageReceived += HandleCommandAsync;
            Client.Log += Log;
            await Commands.AddModulesAsync(Assembly.GetEntryAssembly(), _services);
            _logger?.LogLine(this, "Commands loaded.");
        }

        public async Task StartAsync()
        {
            await Client.LoginAsync(TokenType.Bot, _config["discord:token"]);
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
            if (!(message.HasStringPrefix(_config["discord:prefix"], ref argPos) ||
                message.HasMentionPrefix(Client.CurrentUser, ref argPos)))
            {
                if(!message.Author.IsBot)
                {
                    /*_discordConn.Client.SendPacket(x =>
                    {
                        x.Header = "user.add";
                        x[Id] = msg.Author.Id;
                        x["Username"] = msg.Author.Username;
                        x["Discriminator"] = msg.Author.Discriminator;
                    });

                    _discordConn.Client.SendPacket(x =>
                    {
                        x.Header = "message.add";
                        x[Id] = msg.Id;
                        x[ChannelId] = msg.Channel.Id;
                        x[UserId] = msg.Author.Id;
                        x[Content] = msg.Content;
                        x[TimeStamp] = msg.Timestamp.ToUnixTimeSeconds();
                        if (msg.Channel is SocketGuildChannel guildChannel)
                        {
                            x[GuildId] = guildChannel.Guild.Id;
                        }
                    });*/

                    return;
                }
            }

            // Create a WebSocket-based command context based on the message
            var context = new SocketCommandContext(Client, message);

            // Execute the command with the command context we just
            // created, along with the service provider for precondition checks.

            // Keep in mind that result does not indicate a return value
            // rather an object stating if the command executed successfully.
            var result = await Commands.ExecuteAsync(
                context: context,
                argPos: argPos,
                services: _services);

            // Optionally, we may inform the user if the command fails
            // to be executed; however, this may not always be desired,
            // as it may clog up the request queue should a user spam a
            // command.
            // if (!result.IsSuccess)
            // await context.Channel.SendMessageAsync(result.ErrorReason);
        }
    }
}
