using LionLibrary.Utils;
using Microsoft.Extensions.Configuration;
using MrConnect.Shared;

namespace WoT.Server.Boot
{
    public class AppConfig : JsonAppConfigBase, IMrConnectServiceConnectionConfig, ISslServerConfig, IDataModuleConfig, IConnectionStringConfig
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

        string LionLibrary.Network.IServiceConnectionConfig.ServerName => "MrConnect";
        string LionLibrary.Network.IServiceConnectionConfig.Host => base["services:mr_connect:host"];
        int LionLibrary.Network.IServiceConnectionConfig.Port => Value<int>("services:mr_connect:port");
        string LionLibrary.Network.IServiceConnectionConfig.CertName => base["services:mr_connect:cert_sn"];
        string LionLibrary.Network.IServiceConnectionConfig.PingRoute => base["services:mr_connect:ping_route"];

        public AppConfig() : base(PATH_CONFIG)
        {
        }
    }
}
