using DataServerHelpers;
using LionLibrary.Utils;

namespace WoT.Shared
{
    public class WoTServiceConnectionConfig : ServiceConnectionConfig
    {
        public WoTServiceConnectionConfig(
            JsonAppConfigBase config,
            string serverNamePath,
            string hostPath,
            string portPath,
            string certNamePath,
            string pingPath) :
            base(config, serverNamePath, hostPath, portPath, certNamePath, pingPath) { }
    }
}
