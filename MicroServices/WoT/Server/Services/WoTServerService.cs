using System;
using DataServerHelpers;
using LionLibrary.Network;
using WoT.Server.Network;

namespace WoT.Server
{
    public class WoTServerService : LionServer<SocketWoTUser, CustomCommandContext>
    {
        public WoTServerService(IServiceProvider services) : base(services)
        {

        }
    }
}
