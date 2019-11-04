using DataServerHelpers;
using LionLibrary.Utils;

namespace MrConnect.Shared
{
    public class MrConnectServiceConnectionConfig : ServiceConnectionConfig
    {
        public MrConnectServiceConnectionConfig(
            JsonAppConfigBase config,
            string serverNamePath,
            string hostPath,
            string portPath,
            string certNamePath,
            string pingPath) :
            base(config, serverNamePath, hostPath, portPath, certNamePath, pingPath) { }
    }
}
