using LionLibrary.Utils;
using WoT.Shared;

namespace MrConnect.Server.Boot
{
    public class AppConfig : 
        JsonAppConfigBase, 
        ISslServerConfig, 
        IDataModuleConfig 
    {
        public const string PATH_CONFIG = "data/config.json";

        public ulong OwnerId => Value<ulong>("discord:owner_id");
        public ulong ClientId => Value<ulong>("discord:client_id");
        public string ClientSecret => base["discord:client_secret"];
        public string Token => base["discord:token"];
        public string Prefix => base["discord:prefix"];

        string ISslServerConfig.CertFile => base["server:cert_file"];
        string ISslServerConfig.CertPassword => base["server:cert_pwd"];
        string IServerConfig.Host => base["server:host"];
        int IServerConfig.Port => Value<int>("server:port");
        public string AuthToken => base["server:auth_token"];

        int IDataModuleConfig.MaxEntriesPerPage { get; } = 500;

        public WoTServiceConnectionConfig WoTServiceConnectionConfig { get; }

        public AppConfig() : base(PATH_CONFIG) 
        {
            WoTServiceConnectionConfig = new WoTServiceConnectionConfig(
                this,
                serverNamePath: "WoT",
                hostPath: "services:wot:host",
                portPath: "services:wot:port",
                certNamePath: "services:wot:cert_sn",
                pingPath: "services:wot:ping_route");
        }

    }
}
