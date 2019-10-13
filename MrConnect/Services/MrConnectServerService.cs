using LionLibrary.Network;
using System;
using MrConnect.Network;
using DataServerHelpers;

namespace MrConnect
{
    public class MrConnectServerService : LionServer<SocketMrConnectUser, CustomCommandContext>
    {
        public MrConnectServerService(IServiceProvider services) : base(services)
        {

        }
    }
}
