﻿using DataServerHelpers;
using LionLibrary.Network;
using System;
using System.Collections.Generic;
using System.Text;

namespace MrConnect.Network
{
    public class SocketMrConnectUser : SocketUserBase, IContextSocketUser
    {
        public SocketMrConnectUser(LionClient client) : base(client)
        {

        }
    }
}
