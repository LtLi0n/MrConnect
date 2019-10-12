using System;
using System.Collections.Generic;
using System.Text;
using DataServerHelpers;
using LionLibrary.Network;
using WoT.Server.Network;

namespace WoT.Server
{
    public class WoTServer : LionServer<SocketUser, CustomCommandContext>
    {
        public WoTServer(IServiceProvider services) : base(services)
        {

        }
    }
}
