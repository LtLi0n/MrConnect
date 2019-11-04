using LionLibrary.Network;
using DataServerHelpers;

namespace WoT.Server.Network
{
    public class SocketWoTUser : SocketUserBase, IContextSocketUser
    {
        public SocketWoTUser(LionClient client) : base(client)
        {
        }
    }
}
