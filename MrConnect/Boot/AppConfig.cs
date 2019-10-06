using DataServerHelpers;
using LionLibrary.Network;
using LionLibrary.Utils;
using WoT.Shared;

namespace MrConnect.Boot
{
    public class AppConfig : JsonAppConfigBase, IWoTServiceConnectionConfig
    {
        public const string PATH_CONFIG = "data/config.json";

        public AppConfig() : base(PATH_CONFIG) { }

        public ulong OwnerId => Value<ulong>("discord:owner_id");
        public ulong ClientId => Value<ulong>("discord:client_id");
        public string ClientSecret => base["discord:client_secret"];
        public string Token => base["discord:token"];
        public string Prefix => base["discord:prefix"];

        string LionLibrary.Network.IServiceConnectionConfig.ServerName => "WoT";
        string LionLibrary.Network.IServiceConnectionConfig.Host => base["services:wot:host"];
        int LionLibrary.Network.IServiceConnectionConfig.Port => Value<int>("services:wot:port");
        string LionLibrary.Network.IServiceConnectionConfig.CertName => base["services:wot:cert_sn"];
        string LionLibrary.Network.IServiceConnectionConfig.PingRoute => base["services:wot:ping_route"];
    }
}
