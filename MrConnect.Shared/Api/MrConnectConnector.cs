using LionLibrary.Framework;
using LionLibrary.Network;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace MrConnect.Shared
{
    public class MrConnectConnector : ServerConnector, IMrConnectServiceConnectionConfig
    {
        public MrConnectConnector(
            IServiceProvider services,
            IMrConnectServiceConnectionConfig config,
            ILogService logger) : base(services, config, logger) { }

        protected override void CreateApiControllers(ServiceCollection routes)
        {
            routes.AddSingleton(this);
            routes.AddSingleton<UserApi>();
        }
    }
}
