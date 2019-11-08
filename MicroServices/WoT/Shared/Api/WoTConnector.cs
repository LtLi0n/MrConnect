using LionLibrary.Framework;
using LionLibrary.Network;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace WoT.Shared
{
    public class WoTConnector : ServerConnector
    {
        public UserApi Users => base.GetController<UserApi>();

        public WoTConnector(
            IServiceProvider services,
            WoTServiceConnectionConfig config,
            ILogService logger) : base(services, config, logger) { }

        protected override void CreateApiControllers(ServiceCollection routes)
        {
            routes.AddSingleton(this);
            routes.AddSingleton<UserApi>();
            routes.AddSingleton<CharacterApi>();
            routes.AddSingleton<CharacterWorkApi>();

            routes.AddSingleton<ZoneApi>();
            routes.AddSingleton<ZoneNodeApi>();
        }
    }
}
