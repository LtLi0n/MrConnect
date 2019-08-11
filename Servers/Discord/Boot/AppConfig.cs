using LionLibrary.Utils;

namespace ServerDiscord.Boot
{
    public class AppConfig : AppConfigBase, ISslServerConfig, IServerConfig, IConnectionStringConfig, IDataModuleConfig
    {
        public const string PATH_CONFIG = "data/config.json";

        string IServerConfig.Host => base["server:host"];
        int IServerConfig.Port => base.Value<int>("server:port");

        string ISslServerConfig.CertFile => base["server:cert_file"];
        string ISslServerConfig.CertPassword => base["server:cert_pwd"];

        string IConnectionStringConfig.Server => base["mysql:host"];
        string IConnectionStringConfig.Database => base["mysql:database"];
        string IConnectionStringConfig.User => base["mysql:user"];
        string IConnectionStringConfig.Password => base["mysql:password"];

        int IDataModuleConfig.MaxEntriesPerPage => 500;

        public AppConfig() : base(PATH_CONFIG) { }
    }
}
