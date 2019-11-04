using LionLibrary.Framework;
using LionLibrary.Network;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Discord.Shared
{
    public class DiscordConnector : ServerConnector
    {
        public DiscordConnector(
            IServiceProvider services,
            DiscordServiceConnectionConfig config,
            ILogService logger) : base(services, config, logger) { }
        
        protected override void CreateApiControllers(ServiceCollection routes)
        {
            routes.AddSingleton(this);
            routes.AddSingleton<UserApi>();
            routes.AddSingleton<GuildApi>();
            routes.AddSingleton<FactApi>();
            routes.AddSingleton<FactSuggestionApi>();
        }
    }
}
