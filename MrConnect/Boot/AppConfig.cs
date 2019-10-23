using LionLibrary.Utils;
using WoT.Shared;

namespace MrConnect.Server.Boot
{
    public class AppConfig : JsonAppConfigBase, IWoTServiceConnectionConfig, ISslServerConfig, IDataModuleConfig
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

        string ISslServerConfig.CertFile => base["server:cert_file"];
        string ISslServerConfig.CertPassword => base["server:cert_pwd"];
        string IServerConfig.Host => base["server:host"];
        int IServerConfig.Port => Value<int>("server:port");
        public string AuthToken => base["server:auth_token"];

        int IDataModuleConfig.MaxEntriesPerPage { get; } = 500;
    }
}
