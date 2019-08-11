using LionLibrary.Framework;
using MrConnect.Boot;
using System;

namespace MrConnect.Services
{
    public class ConnectorDiscord : ServerConnector
    {
        public ConnectorDiscord(IServiceProvider services, IAppConfig config, ILogService logger) : base(services, config, logger, "discord")
        {
        }
    }
}
