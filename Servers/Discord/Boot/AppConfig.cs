using Microsoft.Extensions.Configuration;

namespace ServerDiscord.Boot
{
    public class AppConfig : IAppConfig
    {
        public const string PATH_CONFIG = "data/config.json";

        public IConfigurationRoot ConfigRoot { get; }

        public AppConfig()
        {
            ConfigRoot = new ConfigurationBuilder().AddJsonFile(PATH_CONFIG).Build();
        }

        public string this[string key] { get => ConfigRoot[key]; set => ConfigRoot[key] = value; }
        public T GetValue<T>(string key) => ConfigRoot.GetValue<T>(key);
    }
}
