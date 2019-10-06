using LionLibrary.Utils;
using Microsoft.Extensions.Configuration;

namespace WoT.Server.Boot
{
    public class AppConfig : JsonAppConfigBase, ISslServerConfig, IDataModuleConfig, IConnectionStringConfig
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

        public AppConfig() : base(PATH_CONFIG)
        {
        }
    }
}
