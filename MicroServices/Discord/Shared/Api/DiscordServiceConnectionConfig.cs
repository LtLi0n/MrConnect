using DataServerHelpers;
using LionLibrary.Utils;

namespace Discord.Shared
{
    public class DiscordServiceConnectionConfig : ServiceConnectionConfig
    {
        public DiscordServiceConnectionConfig(
            JsonAppConfigBase config, 
            string serverNamePath,
            string hostPath,
            string portPath,
            string certNamePath,
            string pingRoutePath) :
            base(config, serverNamePath, hostPath, portPath, certNamePath, pingRoutePath) { }
    }
}
