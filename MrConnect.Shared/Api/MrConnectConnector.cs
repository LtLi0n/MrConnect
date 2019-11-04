using LionLibrary.Framework;
using LionLibrary.Network;
using Microsoft.Extensions.DependencyInjection;
using System;
using DataServerHelpers;

namespace MrConnect.Shared
{
    public class MrConnectConnector : ServerConnector
    {
        public MrConnectConnector(
            IServiceProvider services,
            MrConnectServiceConnectionConfig config,
            ILogService logger) : base(services, config, logger) { }

        protected override void CreateApiControllers(ServiceCollection routes)
        {
            routes.AddSingleton(this);
            routes.AddSingleton<UserApi>();
        }
    }
}
