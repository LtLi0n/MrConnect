using LionLibrary.Network;
using System;
using System.Collections.Generic;
using System.Text;
using DataServerHelpers;

namespace WoT.Server.Network
{
    public class SocketUser : SocketUserBase, IContextSocketUser
    {
        public SocketUser(LionClient client) : base(client)
        {

        }
    }
}
