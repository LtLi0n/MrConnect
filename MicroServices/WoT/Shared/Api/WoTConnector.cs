using LionLibrary.Framework;
using LionLibrary.Network;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace WoT.Shared
{
    public class WoTConnector : ServerConnector
    {
        public WoTConnector(
            IServiceProvider services,
            IWoTServiceConnectionConfig config,
            ILogService logger) : base(services, config, logger) { }

        protected override void CreateApiControllers(ServiceCollection routes)
        {
            
        }
    }
}
