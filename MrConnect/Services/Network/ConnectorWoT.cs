using LionLibrary.Framework;
using MrConnect.Boot;
using System;
using System.Collections.Generic;
using System.Text;

namespace MrConnect.Services
{
    public class ConnectorWoT : ServerConnector
    {
        public ConnectorWoT(IServiceProvider services, IAppConfig config, ILogService logger) : base(services, config, null, "wot")
        {
        }
    }
}
