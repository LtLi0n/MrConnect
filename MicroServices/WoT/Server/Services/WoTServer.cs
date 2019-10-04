using System;
using System.Collections.Generic;
using System.Text;
using LionLibrary.Network;
using WoT.Server.Modules;
using WoT.Server.Network;

namespace WoT.Server.Services
{
    public class WoTServer : LionServer<SocketUser, CustomCommandContext>
    {
        public WoTServer(IServiceProvider services) : base(services)
        {

        }
    }
}
