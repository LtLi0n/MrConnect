using LionLibrary.Utils;
using Microsoft.Extensions.Configuration;

namespace WoT.Server.Boot
{
    public class AppConfig : JsonAppConfigBase, ISslServerConfig, IDataModuleConfig
    {
        public const string PATH_CONFIG = "data/config.json";

        public IConfigurationRoot ConfigRoot { get; }

        string ISslServerConfig.CertFile => base["server:host"];
        string ISslServerConfig.CertPassword => throw new System.NotImplementedException();
        string IServerConfig.Host { get; } = "localhost";
        int IServerConfig.Port { get; } = 6001;

        int IDataModuleConfig.MaxEntriesPerPage { get; } = 500;

        public AppConfig() : base(PATH_CONFIG)
        {
        }
    }
}
