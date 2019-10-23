using LionLibrary.Network;
using System;
using MrConnect.Server.Network;
using DataServerHelpers;

namespace MrConnect.Server
{
    public class MrConnectServerService : LionServer<SocketMrConnectUser, CustomCommandContext>
    {
        public MrConnectServerService(IServiceProvider services) : base(services)
        {

        }
    }
}
