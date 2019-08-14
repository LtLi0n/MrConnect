using DataServerHelpers;
using LionLibrary.Framework;
using System;

namespace SharedDiscord
{
    public class DiscordConnector : ServerConnector
    {
        public GuildEmojiApi GuildEmoji { get; }
        public UserApi User { get; }

        public DiscordConnector(
            IServiceProvider services, 
            IDiscordServiceConnectionConfig config, 
            ILogService logger) : base(services, config, logger)
        {
            GuildEmoji = new GuildEmojiApi(this);
            User = new UserApi(this);
        }
    }
}
