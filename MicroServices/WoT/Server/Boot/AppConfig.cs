using LionLibrary.Utils;
using Microsoft.Extensions.Configuration;

namespace WoT.Server.Boot
{
    public class AppConfig : JsonAppConfigBase, ISslServerConfig, IDataModuleConfig
    {
        public const string PATH_CONFIG = "data/config.json";

        public IConfigurationRoot ConfigRoot { get; }

        string ISslServerConfig.CertFile => base["server:cert_file"];
        string ISslServerConfig.CertPassword => base["server:cert_pwd"];
        string IServerConfig.Host => base["server:host"];
        int IServerConfig.Port => Value<int>("server:port");

        int IDataModuleConfig.MaxEntriesPerPage { get; } = 500;

        public AppConfig() : base(PATH_CONFIG)
        {
        }
    }
}
