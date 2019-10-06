using LionLibrary.Framework;
using LionLibrary.Network;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace WoT.Shared
{
    public class WoTConnector : ServerConnector, IWoTServiceConnectionConfig
    {
        public UserApi Users => base.GetController<UserApi>();

        public WoTConnector(
            IServiceProvider services,
            IWoTServiceConnectionConfig config,
            ILogService logger) : base(services, config, logger) { }

        protected override void CreateApiControllers(ServiceCollection routes)
        {
            routes.AddSingleton(new UserApi(this));
        }
    }
}
