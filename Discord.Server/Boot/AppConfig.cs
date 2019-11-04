using LionLibrary.Utils;
using Microsoft.Extensions.Configuration;
using MrConnect.Shared;

namespace Discord.Server
{
    public class AppConfig : 
        JsonAppConfigBase, 
        ISslServerConfig, 
        IDataModuleConfig, 
        IConnectionStringConfig
    {
        public const string PATH_CONFIG = "data/config.json";

        public IConfigurationRoot ConfigRoot { get; }

        string ISslServerConfig.CertFile => base["server:cert_file"];
        string ISslServerConfig.CertPassword => base["server:cert_pwd"];
        string IServerConfig.Host => base["server:host"];
        int IServerConfig.Port => Value<int>("server:port");

        int IDataModuleConfig.MaxEntriesPerPage { get; } = 500;

        string IConnectionStringConfig.Server => base["mysql:host"];
        string IConnectionStringConfig.Database => base["mysql:databases:wot:schema"];
        string IConnectionStringConfig.User => base["mysql:databases:wot:user"];
        string IConnectionStringConfig.Password => base["mysql:databases:wot:password"];

        public MrConnectServiceConnectionConfig MrConnectServiceConnectionConfig { get; }

        public AppConfig() : base(PATH_CONFIG)
        {
            MrConnectServiceConnectionConfig = new MrConnectServiceConnectionConfig(
                this,
                serverNamePath: "MrConnect",
                hostPath: "services:mr_connect:host",
                portPath: "services:mr_connect:port",
                certNamePath: "services:mr_connect:cert_sn",
                pingPath: "services:mr_connect:ping_route");
        }
    }
}
