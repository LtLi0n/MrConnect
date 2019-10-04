using DataServerHelpers;
using LionLibrary.Utils;

namespace MrConnect.Boot
{
    public class AppConfig : JsonAppConfigBase
    {
        public const string PATH_CONFIG = "data/config.json";

        public AppConfig() : base(PATH_CONFIG) { }

        public ulong OwnerId => Value<ulong>("discord:owner_id");
        public ulong ClientId => Value<ulong>("discord:client_id");
        public string ClientSecret => base["discord:client_secret"];
        public string Token => base["discord:token"];
        public string Prefix => base["discord:prefix"];
    }
}
