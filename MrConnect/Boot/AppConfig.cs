using DataServerHelpers;
using LionLibrary.Utils;
using SharedDiscord;

namespace MrConnect.Boot
{
    public class AppConfig : AppConfigBase, IDiscordServiceConnectionConfig
    {
        public const string PATH_CONFIG = "data/config.json";

        public AppConfig() : base(PATH_CONFIG) { }

        public ulong OwnerId => Value<ulong>("discord:ownerId");

        string IServiceConnectionConfig.ServerName => "Discord";
        string IServiceConnectionConfig.Host => base["servers:discord:host"];
        int IServiceConnectionConfig.Port => Value<int>("servers:discord:port");
        string IServiceConnectionConfig.CertName => base["servers:discord:cert_sn"];
    }
}
